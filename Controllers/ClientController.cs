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
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Criteria = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "where ([X_EmailID] like '%" + xSearchkey + "%' )";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ClientID desc";
   


            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from ClientMaster  " + Searchkey  + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from ClientMaster "  + Searchkey + Criteria + " and N_ClientID not in (select top(" + Count + ") N_ClientID from ClientMaster  " + Criteria + xSortBy + " ) " + xSortBy;
            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
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
               
                int clientID = myFunctions.getIntVAL(MasterRow["n_ClientID"].ToString());

                 using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();

                    SortedList paramList = new SortedList();
                    paramList.Add("@clientID", clientID);

                    clientID = dLayer.SaveData("ClientMaster", "N_ClientID", DetailTable, connection, transaction);
                    if (clientID <= 0)
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
                    string AppDetailsSql=" SELECT  * from  ClientApps where N_ClientID=@nClientID and N_AppID=@nAppID";
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


    }
}















