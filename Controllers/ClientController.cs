using SmartxAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Linq;
using System.Security.Claims;
using SmartxAPI.GeneralFunctions;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using SmartxAPI.Profiles;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("ClientController")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ICommenServiceRepo _repository;
        private readonly IApiFunctions _api;
        private readonly IConfiguration cofig;
        private readonly IMyFunctions myFunctions;
        private readonly AppSettings _appSettings;

        private string connectionString;
        private string masterDBConnectionString;
        private string AppURL;
        private readonly IDataAccessLayer dLayer;



        public ClientController(ICommenServiceRepo repository, IOptions<AppSettings> appSettings, IMyFunctions myFun, IApiFunctions api, IDataAccessLayer dl, IConfiguration conf)
        {
            _repository = repository;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            _appSettings = appSettings.Value;
            masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");
            connectionString = conf.GetConnectionString("SmartxConnection");
            AppURL = conf.GetConnectionString("AppURL");
            cofig = conf;
        }
        [HttpGet("dashboardList")]
        public ActionResult ClientDashboardList( int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {

            DataTable dt = new DataTable();
            DataTable settings = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            string sqlSettings = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Criteria = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "where ([X_EmailID] like '%" + xSearchkey + "%'  or x_ClientName  like '%" + xSearchkey + "%'  or  x_CompanyName like '%" + xSearchkey + "%' )";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ClientID desc";
           
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                 
                    case "X_CompanyName":
                        xSortBy = "X_CompanyName " + xSortBy.Split(" ")[1];
                        break;
                          case "X_ClientName":
                        xSortBy = "X_ClientName " + xSortBy.Split(" ")[1];
                        break;
                          case "X_EmailID":
                        xSortBy = "X_EmailID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

           
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from ClientMaster  " + Searchkey  + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from ClientMaster  where N_ClientID not in (select top(" + Count + ") N_ClientID from ClientMaster  " + Criteria + xSortBy + " ) " + xSortBy;
            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from ClientMaster" ;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    { 
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }


         [HttpPost("save")]
         public ActionResult UpdateStatus([FromBody] DataSet ds)
        {
            try
            {
                DataTable DetailTable;
                DetailTable = ds.Tables["Details"];
                DataRow MasterRow = DetailTable.Rows[0];
                int n_ClientID = myFunctions.getIntVAL(MasterRow["n_ClientID"].ToString());

                 using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();

                    SortedList paramList = new SortedList();
                    paramList.Add("@clientID", n_ClientID);


                    dLayer.DeleteData("genSettings", "n_ClientID", n_ClientID, "", connection, transaction);
                    int settingsID = dLayer.SaveData("genSettings", "N_SettingsID", DetailTable, connection, transaction);
                    if (settingsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Something went wrong"));
                    }
                       transaction.Commit();
                    return Ok(_api.Success(""));
                 
                }
            }
           catch (Exception ex)
            {
                return StatusCode(403, _api.Error(User, ex));
            }


        }
 
       [HttpGet("clientList")]
        public ActionResult ClientList()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection olivoCon = new SqlConnection(masterDBConnectionString))
                {
                    olivoCon.Open();
                    dt = dLayer.ExecuteDataTable("Select * from ClientMaster", olivoCon);

                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(dt));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("clientDetails")]
        public ActionResult clientDetails(int nClientID)
        {
            try
            {
                 using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    Params.Add("@nClientID", nClientID);
                    string clinetdetailsSql=" SELECT  ClientMaster.*, CountryMaster.X_CountryName FROM  ClientMaster LEFT OUTER JOIN CountryMaster ON ClientMaster.N_CountryID = CountryMaster.N_CountryID where N_ClientID=@nClientID ";
                    DataTable dtClientDetails = dLayer.ExecuteDataTable(clinetdetailsSql,Params, connection);
                    dtClientDetails = _api.Format(dtClientDetails, "ClientDetails");
                    string clientSettingsSql=" SELECT  * from GenSettings where N_ClientID=@nClientID";
                    DataTable dtSettings = dLayer.ExecuteDataTable(clientSettingsSql,Params, connection);
                    if(dtSettings.Rows.Count>0) dtSettings = _api.Format(dtSettings, "dtSettings");
                    string clientAppsSql=" SELECT  * from Vw_ClientAppDetails where N_ClientID=@nClientID";
                    DataTable dtClientApps = dLayer.ExecuteDataTable(clientAppsSql,Params, connection);
                    if(dtClientApps.Rows.Count>0) dtClientApps = _api.Format(dtClientApps, "dtClientApps");
                    dt.Tables.Add(dtSettings);
                    dt.Tables.Add(dtClientDetails);
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }

        }
   
        [HttpGet("appDetails")]
        public ActionResult AppDetails(int nClientID,int nAppID)
        {
            try
            {
                 using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    SortedList Params = new SortedList();
                    Params.Add("@nClientID", nClientID);
                    Params.Add("@nAppID", nAppID);
                    string AppDetailsSql=" SELECT     ClientApps.N_ClientID, ClientApps.N_AppID, ClientApps.X_AppUrl, ClientApps.X_DBUri, ClientApps.N_UserLimit, ClientApps.B_Inactive, ClientApps.X_Sector, ClientApps.N_RefID, ClientApps.D_ExpiryDate, ClientApps.B_Licensed, ClientApps.D_LastExpiryReminder, ClientApps.B_EnableAttachment, AppMaster.X_AppName,AppMaster.b_EnableAttachment as enableAttachment FROM "+
                    " ClientApps LEFT OUTER JOIN  AppMaster ON ClientApps.N_AppID = AppMaster.N_AppID where N_ClientID=@nClientID and ClientApps.N_AppID=@nAppID";
                    DataTable AppDetails = dLayer.ExecuteDataTable(AppDetailsSql,Params, connection);
                    AppDetails = _api.Format(AppDetails, "AppDetails");
                    return Ok(_api.Success(AppDetails));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }

        }
       [HttpGet("clientApps")]
        public ActionResult AppDetails(int nClientID)
        {
            try
            {
                 using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    SortedList Params = new SortedList();
                    Params.Add("@nClientID", nClientID);
                   // string AppListSql=" SELECT  * from  ClientApps where N_ClientID=@nClientID";
                    string AppListSql="SELECT     ClientApps.N_ClientID, ClientApps.N_AppID, ClientApps.X_AppUrl, ClientApps.X_DBUri, ClientApps.N_UserLimit, ClientApps.B_Inactive, ClientApps.X_Sector, ClientApps.N_RefID, ClientApps.D_ExpiryDate, ClientApps.B_Licensed, ClientApps.D_LastExpiryReminder, ClientApps.B_EnableAttachment, AppMaster.X_AppName FROM "+
                    " ClientApps LEFT OUTER JOIN  AppMaster ON ClientApps.N_AppID = AppMaster.N_AppID where N_ClientID=@nClientID";
                    DataTable AppList = dLayer.ExecuteDataTable(AppListSql,Params, connection);
                    return Ok(_api.Success(AppList));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }

        }
        [HttpGet("allApps")]
        public ActionResult allappdetails(int nClientID)
        {
            try
            {
                 using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    SortedList Params = new SortedList();
                     Params.Add("@nClientID", nClientID);
                   // string AppListSql=" SELECT  * from  ClientApps where N_ClientID=@nClientID";
                    string AppListSql="select * from AppMaster";
                    DataTable AppList = dLayer.ExecuteDataTable(AppListSql,Params, connection);
                    return Ok(_api.Success(AppList));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }

        }
        
         [HttpPost("saveApps")]
         public ActionResult saveApps([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["Master"];
               
                int n_AppID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_AppID"].ToString());
                int n_ClientID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ClientID"].ToString());
                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_AppUrl", typeof(string), null);
                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_DbUri", typeof(string), "SmartxConnection");
                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_UserLimit", typeof(int), 1);
                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_Sector", typeof(string), "Service");
                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "D_LastExpiryReminder", typeof(DateTime), null);
                MasterTable.AcceptChanges();    
                

                 using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();

                    SortedList paramList = new SortedList();
                    paramList.Add("@clientID", n_ClientID);


                    dLayer.DeleteData("ClientApps", "n_AppID", n_AppID, "n_ClientID="+n_ClientID+"",connection, transaction);
                    int refID = dLayer.SaveData("ClientApps", "N_RefID", MasterTable,connection, transaction);
                    if (refID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Something went wrong"));
                    }
                       transaction.Commit();
                    return Ok(_api.Success(""));
                 
                }
            }
           catch (Exception ex)
            {
                return StatusCode(403, _api.Error(User, ex));
            }


        }

        


[HttpDelete("appRemove")]
        public ActionResult DeleteData(int nClientID,int nAppID,int nUserID,int nCompanyId)
        {

            int Results = 0;
            SortedList Params = new SortedList();
                 Params.Add("@nClientID", nClientID);
                 Params.Add("@nAppID", nAppID);
                 Params.Add("@nUserID", nUserID);
                 Params.Add("@nCompanyId",nCompanyId);
            int nCompanyID=myFunctions.GetCompanyID(User);
            try
            {                        
                
                 using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();
                     Results=dLayer.ExecuteNonQuery("delete from ClientApps where  n_AppID=" + nAppID + " and n_ClientID=" + nClientID, connection, transaction);
                   
                   using (SqlConnection olivoCon = new SqlConnection(connectionString))
                    {
                        olivoCon.Open();
                        dLayer.ExecuteNonQuery("delete from Sec_UserApps where n_AppID=" + nAppID + " and N_UserID in (SELECT Sec_User.N_UserID from Acc_Company INNER JOIN  Sec_User ON Acc_Company.N_CompanyID = Sec_User.N_CompanyID where Acc_Company.N_ClientID=" + nClientID +")", olivoCon);
                      }
               
                   
                   if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("App removed"));
                    }
                    else
                    {
                         transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to delete Add New App"));
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }
    }
}


        
















