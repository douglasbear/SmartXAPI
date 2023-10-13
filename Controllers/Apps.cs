using SmartxAPI.Data;
using SmartxAPI.Dtos.Login;
using Microsoft.AspNetCore.Mvc;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("apps")]
    [ApiController]
    public class Apps : ControllerBase
    {
        private readonly ISec_UserRepo _repository;
        private readonly IApiFunctions _api;

        private readonly IMyFunctions myFunctions;
        private readonly IDataAccessLayer dLayer;
        private readonly string masterDBConnectionString;
        private readonly string connectionString;

        public Apps(ISec_UserRepo repository, IApiFunctions api, IMyFunctions myFun, IDataAccessLayer dl, IConfiguration conf)
        {
            _repository = repository;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("list")]
        public ActionResult GetAllApps(bool showAll, string AppName,int companyID)
        {
            DataTable dt = new DataTable();
             DataTable uniqueDataTable =new DataTable();
            int ClientID = myFunctions.GetClientID(User);
            int GUserID = myFunctions.GetGlobalUserID(User);
      




            try
            {
                using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    int appID = 0;
                    object userType = dLayer.ExecuteScalar("SELECT isnull(N_UserType,0) as N_UserType FROM Users where N_ClientID=" + ClientID + " and N_UserID=" + GUserID, connection);
                    if (myFunctions.getIntVAL(userType.ToString()) != 0)
                    {
                        appID = myFunctions.getIntVAL(userType.ToString());
                    }

                    string xCritreria="";

                    if(appID>0){
                        using (SqlConnection con = new SqlConnection(connectionString))
                        {
                            con.Open();
                               int userID = myFunctions.GetUserID(User);
                                     int nCompanyID = myFunctions.GetCompanyID(User);
                            string appString="select N_AppID from Sec_UserApps where N_UserID="+userID+" and N_CompanyID="+nCompanyID+"";
                            DataTable userApps=dLayer.ExecuteDataTable(appString,con);
                            var listApps = userApps.AsEnumerable().Select(r => r["N_AppID"].ToString());
                             string value = string.Join(",", listApps);
                            xCritreria = " and (ClientApps.N_AppID in ("+value+")) ";
              
                        }

                    }

                    string sqlCommandText = "select * from (SELECT AppMaster.*, ClientApps.N_ClientID FROM AppMaster LEFT OUTER JOIN ClientApps ON AppMaster.N_AppID = ClientApps.N_AppID" +
                        " WHERE (AppMaster.B_Inactive = 0) and (ClientApps.N_ClientID =" + ClientID + " )" +
                        " Union all" +
                        " SELECT *, null as N_ClientID FROM AppMaster WHERE  N_AppID not in (SELECT N_AppID FROM ClientApps WHERE N_ClientID =" + ClientID + " )) a where N_AppID in (SELECT  N_AppID FROM AppConfig where X_BuildType='" + AppName + "')  order by N_Order";
                    if (showAll == false)
                    {
                        sqlCommandText = "SELECT  AppMaster.N_AppID ,AppMaster.X_AppName,AppMaster.X_AppDescription,AppMaster.X_Version,"
                                         +" AppMaster.B_Inactive,AppMaster.N_Order,AppMaster.N_TrialPeriod,AppMaster.X_HelpUrl,AppMaster.X_InitialDataUrl,"
                                          +" AppMaster.B_EnableAttachment,ClientApps.N_ClientID,ClientApps.D_ExpiryDate"
                                           +",isnull(DATEDIFF(day, GETDATE(),min(D_ExpiryDate)),0) as expiry"
                                          +" FROM AppMaster "
                                           +"INNER JOIN ClientApps ON AppMaster.N_AppID = ClientApps.N_AppID "
                                          +"  where ClientApps.N_ClientID=" + ClientID + " and  ClientApps.N_CompanyID=" + myFunctions.GetCompanyID(User) + " and AppMaster.B_Inactive =0 " + xCritreria + " "
                                           +"Group By AppMaster.N_AppID ,AppMaster.X_AppName,AppMaster.X_AppDescription,AppMaster.X_Version, "
                                          +" AppMaster.B_Inactive,AppMaster.N_Order,AppMaster.N_TrialPeriod,AppMaster.X_HelpUrl,AppMaster.X_InitialDataUrl, "
                                         +"  AppMaster.B_EnableAttachment,ClientApps.N_ClientID,ClientApps.D_ExpiryDate  order by AppMaster.N_Order";
                    }

                    dt = dLayer.ExecuteDataTable(sqlCommandText, connection);
                     uniqueDataTable = dt.Clone();
                    HashSet<object> seenAppIDs = new HashSet<object>();
                     foreach (DataRow row in dt.Rows)
                     {
                        object app_ID = row["N_AppID"];
                        if (!seenAppIDs.Contains(app_ID))
                            {
                                seenAppIDs.Add(app_ID);
                                uniqueDataTable.ImportRow(row);

                            }

                     }

                     uniqueDataTable.AcceptChanges();

                
                }
                if (uniqueDataTable.Rows.Count == 0)
                { 
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(uniqueDataTable));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }

        }

        [HttpGet("login")]
        public ActionResult AutoLogin(int appID, int nCompanyID)
        {
            DataTable dt = new DataTable();
            SortedList res = new SortedList();
            SortedList appParams = new SortedList();
            appParams.Add("@nClientID", myFunctions.GetClientID(User));
            appParams.Add("@nAppID", appID);
            string sqlCommandText = "select X_DBUri,X_AppUrl from ClientApps where N_ClientID=@nClientID and N_AppID=@nAppID";
            try
            {
                using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, appParams, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    // App not yet registerd...
                    res.Add("AppStatus", "NotRegistered");
                    res.Add("AppID", appID);
                }
                else
                {
                    res.Add("AppStatus", "Registered");
                    res.Add("AppID", appID);


                }
                return Ok(_api.Success(res));

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }

        }
        

       
    }
}