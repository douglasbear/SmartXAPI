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
        private readonly string hqDBConnectionString;


        public ClientController(ICommenServiceRepo repository, IOptions<AppSettings> appSettings, IMyFunctions myFun, IApiFunctions api, IDataAccessLayer dl, IConfiguration conf)
        {
            _repository = repository;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            _appSettings = appSettings.Value;
            masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");
            connectionString = conf.GetConnectionString("SmartxConnection");
            hqDBConnectionString = conf.GetConnectionString("HqConnection");
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

                    sqlCommandCount = "select count(1) as N_Count  from ClientMaster" ;
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
                DataTable DetailsTable;
                DetailTable = ds.Tables["Details"];
             DataTable MasterTable;
           
                MasterTable = ds.Tables["master"];

                DataRow MasterRow = DetailTable.Rows[0];
                int n_ClientID = myFunctions.getIntVAL(MasterRow["n_ClientID"].ToString());
                DateTime dAppStartDate = Convert.ToDateTime(MasterTable.Rows[0]["d_AppStartDate"].ToString());
                string Sql = "";
                 using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();

                    SortedList paramList = new SortedList();
                    paramList.Add("@clientID", n_ClientID);



                   DetailsTable = dLayer.ExecuteDataTable("select top 1 (select N_Value from genSettings where N_ClientID="+n_ClientID+" and X_Description='COMPANY LIMIT')  as N_Companies,(select N_Value  from genSettings where N_ClientID="+n_ClientID+" and X_Description='BRANCH LIMIT')as N_Branches,"+
                   "(select N_Value  from genSettings where N_ClientID="+n_ClientID+" and X_Description='EMPLOYEE LIMIT')as N_Employees,"+"(select N_Value from genSettings where N_ClientID="+n_ClientID+" and X_Description='USER LIMIT') as N_Users  from  genSettings where N_ClientID="+n_ClientID+"",paramList,connection, transaction);
                    //DetailsTable = dLayer.ExecuteDataTable(,connection, transaction);
                      DetailsTable = myFunctions.AddNewColumnToDataTable(DetailsTable, "N_ClientHistoryID", typeof(int), 0);
                      DetailsTable = myFunctions.AddNewColumnToDataTable(DetailsTable, "N_ClientID", typeof(int), n_ClientID);
                      DetailsTable = myFunctions.AddNewColumnToDataTable(DetailsTable, "N_UpdatedUserID", typeof(int),myFunctions.GetUserID(User));
                      DetailsTable.AcceptChanges(); 
                     //dLayer.DeleteData("ClientHistory", "n_ClientID", n_ClientID,"",connection, transaction);
                    int nClientHistoryID = dLayer.SaveData("ClientHistory", "N_ClientHistoryID", DetailsTable, connection, transaction);
                    if (nClientHistoryID <= 0)
                    {

                        transaction.Rollback();
                        return Ok(_api.Error(User, "Something went wrong"));
                    }
                    
                    dLayer.DeleteData("genSettings", "n_ClientID", n_ClientID, "", connection, transaction);
                    int settingsID = dLayer.SaveData("genSettings", "N_SettingsID", DetailTable, connection, transaction);
                    
            //  SortedList Params = new SortedList();
           
                     if (n_ClientID >0)
                            {
                                
                                //  dLayer.ExecuteNonQuery("update ClientMaster set d_AppStartDate= "+ MasterRow["d_AppStartDate"].ToString()+"and  where N_ClientID=" + n_ClientID, Params, connection);
                                 dLayer.ExecuteNonQuery("UPDATE ClientMaster SET d_AppStartDate = '" + dAppStartDate + "' WHERE N_ClientID = " + n_ClientID, paramList, connection,transaction);


                            }
                    
                    
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
                    string  clientSettingsSql="";
                    string clinetdetailsSql=" SELECT  ClientMaster.*, CountryMaster.X_CountryName FROM  ClientMaster LEFT OUTER JOIN CountryMaster ON ClientMaster.N_CountryID = CountryMaster.N_CountryID  where N_ClientID=@nClientID ";
                    DataTable dtClientDetails = dLayer.ExecuteDataTable(clinetdetailsSql,Params, connection);
                    dtClientDetails = _api.Format(dtClientDetails, "ClientDetails");

                    dtClientDetails= myFunctions.AddNewColumnToDataTable(dtClientDetails, "x_SuperAdminUserID", typeof(string), "");
                    dtClientDetails= myFunctions.AddNewColumnToDataTable(dtClientDetails, "x_ProjectName", typeof(string), "");
                    dtClientDetails.AcceptChanges(); 
                     
                         object xSuperAdminUserID=dLayer.ExecuteScalar("select X_UserID from users where n_ClientID="+nClientID+" and N_LoginType=1", Params, connection);
                           
                            if(xSuperAdminUserID!=null )
                                {
                                  dtClientDetails.Rows[0]["x_SuperAdminUserID"]=xSuperAdminUserID;
                                   
                                }
                    // string clientSettingsSql=" SELECT  * from GenSettings where N_ClientID=@nClientID";
                    
                    object xAppType = dtClientDetails.Rows[0]["x_AppType"];
                    switch (xAppType)
                    {
                    case "erp"://PO
                        clientSettingsSql=" SELECT  * from GenSettings where N_ClientID=@nClientID and X_Description not in ('STUDENT LIMIT','EMPLOYEE LIMIT')";
                        break;
                    case "obs"://loan issue
                       clientSettingsSql=" SELECT  * from GenSettings where N_ClientID=@nClientID and X_Description not in('STUDENT LIMIT','BRANCH LIMIT','EMPLOYEE LIMIT','COMPANY LIMIT') ";
                        break;
                    case "hrp"://Salary Payment
                       clientSettingsSql=" SELECT  * from GenSettings where N_ClientID=@nClientID  and X_Description not in('STUDENT LIMIT','BRANCH LIMIT') ";
                        break;
                    case "ss"://Salary Payment 
                       clientSettingsSql=" SELECT  * from GenSettings where N_ClientID=@nClientID and X_Description not in('BRANCH LIMIT','EMPLOYEE LIMIT')";
                        break;

                    default:
                     clientSettingsSql=" SELECT  * from GenSettings where N_ClientID=@nClientID";
                    break;
                    }


                     
                    DataTable dtSettings = dLayer.ExecuteDataTable(clientSettingsSql,Params, connection);
                    dtSettings = myFunctions.AddNewColumnToDataTable(dtSettings, "N_Free", typeof(int), 0);
                    dtSettings = myFunctions.AddNewColumnToDataTable(dtSettings, "N_Additional", typeof(int), 0);
                    if(dtSettings.Rows.Count>0) 
                    {
                          for(int i=0;i<dtSettings.Rows.Count;i++)
                            {

                                int nCompanyID=myFunctions.GetCompanyID(User);
                                Object nFreeUsers=dLayer.ExecuteScalar("select SUM(isnull(N_FreeUsers,0)) as N_FreeUsers  from AppMaster where N_AppID in ( select N_AppID from ClientApps where n_ClientID="+nClientID+" )", Params, connection);
                                Object nFreeEmployees=dLayer.ExecuteScalar("select SUM(isnull(N_FreeEmployees,0)) as N_FreeEmployees  from AppMaster where N_AppID in ( select N_AppID from ClientApps where n_ClientID="+nClientID+" )", Params, connection);
                                Object NFreeLocations=dLayer.ExecuteScalar("select SUM(isnull(N_FreeLocations,0)) as N_FreeLocations  from AppMaster where N_AppID in ( select N_AppID from ClientApps where n_ClientID="+nClientID+" )", Params, connection);
                                Object NFreeStudents=dLayer.ExecuteScalar(" select SUM(isnull(N_FreeStudents,0)) as N_FreeStudents  from AppMaster where N_AppID in ( select N_AppID from ClientApps where n_ClientID="+nClientID+" )", Params, connection);
                                Object NFreeBranches=dLayer.ExecuteScalar(" select SUM(isnull(N_FreeBranches,0)) as N_FreeBranches  from AppMaster where N_AppID in ( select N_AppID from ClientApps where n_ClientID="+nClientID+" )", Params, connection);
                                //Object NFreeBranches=dLayer.ExecuteScalar(" select 0 as N_FreeCompanies from AppMaster where N_AppID in ( select N_AppID from ClientApps where n_ClientID="+nClientID+" )", Params, connection);
                                if(dtSettings.Rows[i]["x_Description"].ToString()=="USER LIMIT")
                                {
                                    if(nFreeUsers!=null)
                                    {
                                  dtSettings.Rows[i]["N_Free"]=nFreeUsers;
                                  dtSettings.Rows[i]["N_Additional"]= myFunctions.getIntVAL(dtSettings.Rows[i]["N_Value"].ToString()) - myFunctions.getIntVAL(nFreeUsers.ToString());
                                    }
                                }
                                 if(dtSettings.Rows[i]["x_Description"].ToString()=="EMPLOYEE LIMIT")
                                {
                                    if(nFreeEmployees!=null)
                                    {
                                  dtSettings.Rows[i]["N_Free"]=nFreeEmployees;
                                  dtSettings.Rows[i]["N_Additional"]= myFunctions.getIntVAL(dtSettings.Rows[i]["N_Value"].ToString())-  myFunctions.getIntVAL(nFreeEmployees.ToString());
                                
                                    }
                                  
                                }
                                  if(dtSettings.Rows[i]["x_Description"].ToString()=="BRANCH LIMIT")
                                {
                                    if(NFreeBranches!=null)
                                    {
                                  dtSettings.Rows[i]["N_Free"]=NFreeBranches;
                                  dtSettings.Rows[i]["N_Additional"]= myFunctions.getIntVAL(dtSettings.Rows[i]["N_Value"].ToString())- myFunctions.getIntVAL(NFreeBranches.ToString());
                                
                                    }
                                  
                                }
                                  if(dtSettings.Rows[i]["x_Description"].ToString()=="STUDENT LIMIT")
                                {
                                    if(NFreeStudents!=null)
                                    {
                                  dtSettings.Rows[i]["N_Free"]=NFreeStudents;
                                  dtSettings.Rows[i]["N_Additional"]= myFunctions.getIntVAL(dtSettings.Rows[i]["N_Value"].ToString())- myFunctions.getIntVAL(NFreeStudents.ToString());
                                
                                    }
                                  
                                }
                                  if(dtSettings.Rows[i]["x_Description"].ToString()=="LOCATION LIMIT")
                                {
                                    if(NFreeLocations!=null)
                                    {
                                  dtSettings.Rows[i]["N_Free"]=NFreeLocations;
                                  dtSettings.Rows[i]["N_Additional"]= myFunctions.getIntVAL(dtSettings.Rows[i]["N_Value"].ToString())- myFunctions.getIntVAL(NFreeLocations.ToString());
                                
                                    }
                                  
                                }
                                if(dtSettings.Rows[i]["x_Description"].ToString()=="COMPANY LIMIT")
                                {
                                  
                                  dtSettings.Rows[i]["N_Free"]=0;
                                  dtSettings.Rows[i]["N_Additional"]= myFunctions.getIntVAL(dtSettings.Rows[i]["N_Value"].ToString()) - 0;
                        
                                }
                                  
                            
                             
                            }

                             dtSettings = _api.Format(dtSettings, "dtSettings");
                    }
                   
                    string clientAppsSql=" SELECT  * from Vw_ClientAppDetails where N_ClientID=@nClientID";
                    DataTable dtClientApps = dLayer.ExecuteDataTable(clientAppsSql,Params, connection);
                    if(dtClientApps.Rows.Count>0) dtClientApps = _api.Format(dtClientApps, "dtClientApps");
                    string clientHistorySql=" SELECT  * from Vw_ClientHistoryDetails where N_ClientID=@nClientID";
                     DataTable dtClientHistoryDetails = dLayer.ExecuteDataTable(clientHistorySql,Params, connection);
                     
                     string appHistorySql=" SELECT  * from Vw_AppHistoryDetails where N_ClientID=@nClientID";
                     DataTable dtAppHistoryDetails = dLayer.ExecuteDataTable(appHistorySql,Params, connection);
                
                  
                   using (SqlConnection olivoCon = new SqlConnection(connectionString))
                    {
                        olivoCon.Open();
                        dtAppHistoryDetails = myFunctions.AddNewColumnToDataTable(dtAppHistoryDetails, "X_Userame", typeof(string), "");
                        dtAppHistoryDetails.AcceptChanges(); 
                        dtClientHistoryDetails = myFunctions.AddNewColumnToDataTable(dtClientHistoryDetails, "X_Userame", typeof(string), "");
                        dtClientHistoryDetails.AcceptChanges(); 
                       
                        for(int i=0;i<dtAppHistoryDetails.Rows.Count;i++)
                            {

                                int nCompanyID=myFunctions.GetCompanyID(User);
                                Object xUserID=dLayer.ExecuteScalar("select X_UserID from sec_user where N_UserID="+myFunctions.getIntVAL(dtAppHistoryDetails.Rows[i]["N_UpdatedUserID"].ToString()), Params, olivoCon);
                                if(xUserID!=null )
                                {
                                  dtAppHistoryDetails.Rows[i]["X_Userame"]=xUserID;
                                   
                                }
                             
                            }

                         for(int i=0;i<dtClientHistoryDetails.Rows.Count;i++)
                            {

                                int nCompanyID=myFunctions.GetCompanyID(User);
                                Object xUserID=dLayer.ExecuteScalar("select X_UserID from sec_user where N_UserID="+myFunctions.getIntVAL(dtClientHistoryDetails.Rows[i]["N_UpdatedUserID"].ToString()), Params, olivoCon);
                                if(xUserID!=null )
                                {
                                  dtClientHistoryDetails.Rows[i]["X_Userame"]=xUserID;
                                   
                                }
                             
                            }

                           

                       }
                        
                         if(hqDBConnectionString !=null)
                {
                    using (SqlConnection hqcon = new SqlConnection(hqDBConnectionString))
                {
                    hqcon.Open();
                    SortedList Paramshq = new SortedList();
                    Paramshq.Add("@nClientID", nClientID);
                   
                               object xProjectName=dLayer.ExecuteScalar("select X_ProjectName from Inv_customerProjects where N_ProjectID="+myFunctions.getIntVAL(dtClientDetails.Rows[0]["N_ProjectID"].ToString()),Paramshq,hqcon);
                                if(xProjectName!=null )
                                {
                                  dtClientDetails.Rows[0]["X_ProjectName"]=xProjectName;
                                   
                                }
                    }
                }
                       dtAppHistoryDetails = _api.Format(dtAppHistoryDetails, "appHistoryDetails");
                       dtClientHistoryDetails = _api.Format(dtClientHistoryDetails, "clientHistoryDetails");

                

                    dt.Tables.Add(dtSettings);
                    dt.Tables.Add(dtClientDetails);
                    dt.Tables.Add(dtClientHistoryDetails);
                    dt.Tables.Add(dtAppHistoryDetails);
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
                    // string AppDetailsSql=" SELECT     ClientApps.N_ClientID, ClientApps.N_AppID, ClientApps.X_AppUrl, ClientApps.X_DBUri, ClientApps.N_UserLimit, ClientApps.B_Inactive, ClientApps.X_Sector, ClientApps.N_RefID, ClientApps.D_ExpiryDate, ClientApps.B_Licensed, ClientApps.D_LastExpiryReminder, ClientApps.B_EnableAttachment, AppMaster.X_AppName,AppMaster.b_EnableAttachment as enableAttachment,ClientApps.N_SubscriptionAmount,ClientApps.n_DiscountAmount,ClientApps.D_StartDate FROM "+
                    // " ClientApps LEFT OUTER JOIN  AppMaster ON ClientApps.N_AppID = AppMaster.N_AppID  LEFT OUTER JOIN where N_ClientID=@nClientID and ClientApps.N_AppID=@nAppID";
                    string AppDetailsSql=" SELECT     ClientApps.N_ClientID, ClientApps.N_AppID, ClientApps.X_AppUrl, ClientApps.X_DBUri, ClientApps.N_UserLimit, ClientApps.B_Inactive, ClientApps.X_Sector, ClientApps.N_RefID,ClientApps.D_ExpiryDate, ClientApps.B_Licensed, ClientApps.D_LastExpiryReminder, ClientApps.B_EnableAttachment, AppMaster.X_AppName,AppMaster.B_EnableAttachment AS enableAttachment, ClientApps.N_SubscriptionAmount, ClientApps.N_DiscountAmount,ClientApps.B_isDisable, ClientApps.D_StartDate, ClientApps.N_CompanyID,ClientCompany.n_ClientCompanyID, ClientCompany.X_CompanyName, ClientMaster.D_AppStartDate FROM "+
                      "ClientApps LEFT OUTER JOIN   ClientCompany ON ClientApps.N_ClientID = ClientCompany.N_ClientID AND ClientApps.N_CompanyID = ClientCompany.n_CompanyID LEFT OUTER JOIN   AppMaster ON ClientApps.N_AppID = AppMaster.N_AppID LEFT OUTER JOIN   ClientMaster ON ClientApps.N_ClientID = ClientMaster.N_ClientID where ClientApps.N_ClientID=@nClientID and ClientApps.N_AppID=@nAppID";
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
        public ActionResult AppListDetails(int nClientID, int nCompanyID)
        {
            try
            {
                 using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    SortedList Params = new SortedList();
                    Params.Add("@nClientID", nClientID);
                    string companyCriteria="";
                    if(nCompanyID>0)
                    {
                        companyCriteria= " and ClientApps.N_CompanyID="+nCompanyID+"";

                    }
                   // string AppListSql=" SELECT  * from  ClientApps where N_ClientID=@nClientID";
                    string AppListSql="SELECT     ClientApps.N_ClientID, ClientApps.N_AppID, ClientApps.X_AppUrl, ClientApps.X_DBUri, ClientApps.N_UserLimit, ClientApps.B_Inactive, ClientApps.X_Sector, ClientApps.N_RefID, ClientApps.D_ExpiryDate, ClientApps.B_Licensed, ClientApps.D_LastExpiryReminder, ClientApps.B_EnableAttachment,ClientApps.N_SubscriptionAmount, AppMaster.X_AppName,AppMaster.N_FreeUsers,AppMaster.N_FreeEmployees,ClientApps.D_StartDate FROM "+
                    " ClientApps LEFT OUTER JOIN  AppMaster ON ClientApps.N_AppID = AppMaster.N_AppID where N_ClientID=@nClientID "+ companyCriteria+" ";
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

        [HttpGet("allCompanies")]
        public ActionResult allCompanies(int nClientID)
        {
            try
            {
                 using (SqlConnection connection = new SqlConnection(masterDBConnectionString))
                {
                    connection.Open();
                    SortedList Params = new SortedList();
                    Params.Add("@nClientID", nClientID);
                    string CompanyListSql=" SELECT  * from  Clientcompany where N_ClientID=@nClientID";
                 
                    DataTable CompanyList = dLayer.ExecuteDataTable(CompanyListSql,Params, connection);
                    return Ok(_api.Success(CompanyList));
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
                int n_RefID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_RefID"].ToString());
                //MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_AppUrl", typeof(string), null);
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

                  

                    if(n_RefID==0)
                    {
                  
                    object freeUserCount = dLayer.ExecuteScalar("SELECT isnull(N_FreeUsers,0)  FROM AppMaster  where N_AppID="+n_AppID+"",paramList, connection,transaction);
                    int freeUservalue=myFunctions.getIntVAL(freeUserCount.ToString());
                    object settingsUserCount = dLayer.ExecuteScalar("SELECT  isnull(N_Value,0) FROM GenSettings where N_ClientID="+n_ClientID+" and X_Description='USER LIMIT'",paramList, connection,transaction);
                    int settingsUservalue=myFunctions.getIntVAL(settingsUserCount.ToString());      
                    int sumValue= freeUservalue + settingsUservalue;    
                    dLayer.ExecuteNonQuery("Update GenSettings Set N_Value =  "+sumValue+" where  N_ClientID="+n_ClientID+" and X_Description='USER LIMIT'" , connection, transaction);
                    }

                    dLayer.DeleteData("ClientApps", "n_AppID", n_AppID, "n_ClientID="+n_ClientID+"",connection, transaction);
                    int refID = dLayer.SaveData("ClientApps", "N_RefID", MasterTable,connection, transaction);
                    if (refID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Something went wrong"));
                    }
                    else
                    {

                   
                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_AppHistoryID", typeof(int),0);
                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_UpdatedUserID", typeof(int),myFunctions.GetUserID(User));
                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "D_UpdatedDate", typeof(DateTime),DateTime.Now);
                 
                    if (myFunctions.ContainColumn("n_RefID", MasterTable))
                        MasterTable.Columns.Remove("n_RefID");
                    if (myFunctions.ContainColumn("x_AppUrl", MasterTable))
                        MasterTable.Columns.Remove("x_AppUrl");
                    if (myFunctions.ContainColumn("b_Inactive", MasterTable))
                        MasterTable.Columns.Remove("b_Inactive");
                    if (myFunctions.ContainColumn("b_Licensed", MasterTable))
                        MasterTable.Columns.Remove("b_Licensed");
                    if (myFunctions.ContainColumn("N_UserLimit", MasterTable))
                        MasterTable.Columns.Remove("N_UserLimit");
                    if (myFunctions.ContainColumn("x_DbUri", MasterTable))
                        MasterTable.Columns.Remove("x_DbUri");   
                    if (myFunctions.ContainColumn("x_Sector", MasterTable))
                        MasterTable.Columns.Remove("x_Sector");
                    if (myFunctions.ContainColumn("d_StartDate", MasterTable))
                        MasterTable.Columns.Remove("d_StartDate");
                    if (myFunctions.ContainColumn("n_CompanyID", MasterTable))
                        MasterTable.Columns.Remove("n_CompanyID");
                    if (myFunctions.ContainColumn("b_isDisable", MasterTable))
                        MasterTable.Columns.Remove("b_isDisable");
                    MasterTable.AcceptChanges();   
                    //dLayer.DeleteData("AppHistory", "n_AppID", n_AppID, "N_ClientID="+n_ClientID+"",connection, transaction);
                    int appHistoryID = dLayer.SaveData("AppHistory", "N_AppHistoryID", MasterTable, connection, transaction);
                    if (appHistoryID <= 0)
                    {

                        transaction.Rollback();
                        return Ok(_api.Error(User, "Something went wrong"));
                    }
                    
                    transaction.Commit();
                    return Ok(_api.Success(""));
                 
                    }
                    
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
                     dLayer.ExecuteNonQuery("delete from AppHistory where  n_AppID=" + nAppID + " and n_ClientID=" + nClientID, connection, transaction);
                    object freeUserCount = dLayer.ExecuteScalar("SELECT isnull(N_FreeUsers,0)  FROM AppMaster  where N_AppID="+nAppID+"",Params, connection,transaction);
                    int freeUservalue=myFunctions.getIntVAL(freeUserCount.ToString());
                    object settingsUserCount = dLayer.ExecuteScalar("SELECT  isnull(N_Value,0) FROM GenSettings where N_ClientID="+nClientID+" and X_Description='USER LIMIT'",Params, connection,transaction);
                    int settingsUservalue=myFunctions.getIntVAL(settingsUserCount.ToString());      
                    int sumValue= settingsUservalue - freeUservalue ;    
                    dLayer.ExecuteNonQuery("Update GenSettings Set N_Value =  "+sumValue+" where  N_ClientID="+nClientID+" and X_Description='USER LIMIT'" , connection, transaction);
                    
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


        
















