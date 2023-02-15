using Microsoft.AspNetCore.Mvc;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Text;



using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

using System.Text.RegularExpressions;

using System.Security;

using System.Net;
using System.Net.Mail;
using System.Windows;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("SecUserPreferenceSettings")]
    [ApiController]



    public class Sec_UserPreferenceSettings : ControllerBase
    {
          private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly string masterDBConnectionString;

    public Sec_UserPreferenceSettings(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf,IMyAttachments myAtt)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");
        }



        [HttpGet("list")]
         public ActionResult SecUserPreferenceList (int appID )
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            try
            {
                
                using (SqlConnection cnn = new SqlConnection(masterDBConnectionString))
                 {
                    cnn.Open();
  
                     using (SqlConnection connection = new SqlConnection(connectionString))
                  {
                    connection.Open();

                     Params.Add("@appID", appID);


                     DataTable AppModules = dLayer.ExecuteDataTable("select N_ModuleID from AppModules where  N_AppID= " + appID,Params,cnn);
                      var listApps = AppModules.AsEnumerable().Select(r => r["N_ModuleID"].ToString());
                             string value = string.Join(",", listApps);
                      
                   
                     string sqlCommandText="select * from sec_menus where N_ParentMenuID in(" + value + ") and B_WShow=1 and ((ISNULL(sec_menus.x_RouteName, '') <> '') OR (ISNULL(sec_menus.x_RouteName, 0) <> 0)) and ((ISNULL(sec_menus.X_Caption, '') <> ''))";

                     dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
                  }
               
                }
             }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }

        }


                 [HttpPost("save")]
        public ActionResult Save([FromBody] DataSet ds)
        {
            DataTable MasterTable = ds.Tables["master"];
            DataRow MasterRow = MasterTable.Rows[0];

            string landingPage = MasterTable.Rows[0]["x_LandingPage"].ToString();
            int userID =  myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());
             int nCompanyId=myFunctions.GetCompanyID(User);
             int nAppID=myFunctions.getIntVAL(MasterTable.Rows[0]["nAppID"].ToString());

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;

                    transaction = connection.BeginTransaction();

                    SortedList paramList = new SortedList();
                    paramList.Add("@landingPage", landingPage);
                     paramList.Add("@userID", userID);
                     paramList.Add("@nCompanyId", nCompanyId);
                     paramList.Add("@nAppID", nAppID);
                   
                dLayer.ExecuteNonQuery("Update Sec_UserApps Set X_LandingPage=@landingPage Where N_UserID=@userID and N_CompanyID=@nCompanyId and N_AppID=@nAppID",paramList, connection, transaction);
                 transaction.Commit();
                return Ok(api.Success("Saved"));
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }


                [HttpGet("details")]
        public ActionResult GetDetails(int nAppID, int nUserID)
        {
            DataTable moduleDetails = new DataTable();
            DataTable dtDefaults = new DataTable();

            DataSet DS = new DataSet();
            SortedList Params = new SortedList();
            SortedList dParamList = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string details = " select * from Sec_UserApps where N_UserID=@p2 and N_AppID=@p1";

            Params.Add("@p1", nAppID);
            Params.Add("@p2", nUserID);
            Params.Add("@p3", nCompanyId);
         
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    moduleDetails = dLayer.ExecuteDataTable(details, Params, connection);

                }
                moduleDetails = api.Format(moduleDetails, "details");
                 DS.Tables.Add(moduleDetails);
                 return Ok(api.Success(DS));
                
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

        


    }
}