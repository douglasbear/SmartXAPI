using AutoMapper;
using SmartxAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("setApprovals")]
    [ApiController]
    public class SetApprovals : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;

        public SetApprovals(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1058;
        }
        private readonly string connectionString;


        [HttpGet("modulelist")]
        public ActionResult GetModulelist(int nCompanyId,int nLanguageId )
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
           
                    // int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    // int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    // int nLanguageID = myFunctions.getIntVAL(MasterRow["N_LanguageId"].ToString());
                    // int nUserCategoryID = myFunctions.getIntVAL(MasterRow["N_UserCategoryID"].ToString());
                    
                    // string x_ServiceEndCode = MasterRow["X_ServiceEndCode"].ToString();

            //int nCompanyId=myFunctions.GetCompanyID(User);ParentMenuID,N_MenuID,X_Text,X_Module FROM vw_UserMenus_
           // string sqlCommandText = "select N_CompanyID,N_LanguageID,N_ParentMenuID,N_MenuID,X_Text,X_Module FROM vw_UserMenus_List WHERE N_LanguageID = " + LanguageID + " and N_ParentMenuID = 0 and (N_UserCategoryID in (" + UserCategoryIDList + ") or N_UserCategoryID=" + UserCategoryID + ") and N_CompanyID=" + CompanyID + "and N_MenuID not in (85,1,83) group by N_CompanyID,N_LanguageID,N_ParentMenuID,N_MenuID,X_Text,X_Module";
            string sqlCommandText = "select Top 10  N_CompanyID,N_LanguageID,N_ParentMenuID,N_MenuID,X_Text,X_Module FROM vw_UserMenus_List WHERE N_CompanyID=@p1 and N_LanguageID=@p2 and N_ParentMenuID = 0 and N_MenuID not in (85,1,83) group by N_CompanyID,N_LanguageID,N_ParentMenuID,N_MenuID,X_Text,X_Module";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nLanguageId);
            // Params.Add("@p2", nLanguageID);
            // Params.Add("@p3", nUserCategoryID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    connection.Open();
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else { return Ok(_api.Success(dt)); }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }



        [HttpGet("screenlist")]
        public ActionResult GetScreenlist(int nLanguageId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            //int nCompanyId=myFunctions.GetCompanyID(User);
             Params.Add("@p1", nLanguageId);
            string sqlCommandText = "select N_MenuID,N_LanguageId,N_ParentMenuID,B_AllowApproval,X_Text from vw_UserPrevilegesDisp where N_LanguageId=@p1 ";
           

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    connection.Open();
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else { return Ok(_api.Success(dt)); }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
 [HttpGet("approvalcodelist")]
        public ActionResult GetApprovalCodelist(int nCompanyId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            //int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select N_CompanyID,N_ApprovalID,N_UserID,X_ApprovalCode,X_ApprovalDescription from Gen_ApprovalCodes where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyId);


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    connection.Open();
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else { return Ok(_api.Success(dt)); }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

[HttpGet("projectlist")]
        public ActionResult GetProjectList(int nCompanyId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            //int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "Select N_CompanyID,N_ProjectID,B_IsSaveDraft,B_InActive,X_ProjectCode,X_ProjectName from Inv_CustomerProjects where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyId);


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    connection.Open();
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else { return Ok(_api.Success(dt)); }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

       

[HttpGet("list")]
        public ActionResult GetsetApprovalsList(int nCompanyId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            //int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "Select * from Sec_ApprovalSettings_General where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyId);


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    connection.Open();
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else { return Ok(_api.Success(dt)); }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }


 [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    
                    DataTable DetailTable;
                    
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = DetailTable.Rows[0];
                    SortedList Params = new SortedList();

                    int n_SecApprovalID = myFunctions.getIntVAL(MasterRow["N_SecApprovalID"].ToString());
                  
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int n_ModuleID = myFunctions.getIntVAL(MasterRow["N_ModuleID"].ToString());
                    if (n_ModuleID>0)
                    {
                        
                         dLayer.DeleteData("Sec_ApprovalSettings_General", "N_ModuleID", n_ModuleID, "", connection,transaction);

                    }
                    n_SecApprovalID = dLayer.SaveData("Sec_ApprovalSettings_General", "N_SecApprovalID", "", "", DetailTable, connection, transaction);
                    if (n_SecApprovalID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save ");
                    }
                   
                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_SecApprovalID", n_SecApprovalID);
                    return Ok(_api.Success(Result, "set Approvals Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nModuleID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    
                    connection.Open();
                    Results = dLayer.DeleteData("Sec_ApprovalSettings_General", "N_ModuleID", nModuleID, "", connection);
                    if (Results > 0)
                    {
                    
                        
                        return Ok(_api.Success("Set Approvald eleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }


          [HttpGet("details")]
        public ActionResult GenApprovalCode(int nModuleID)
        {


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable DataTable = new DataTable();
                    //int n_SecApprovalID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_SecApprovalID"].ToString());
                   

                    //string Mastersql = "";
                    string DetailSql = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@nModuleID", nModuleID);
                    // Mastersql = "select * from Gen_ApprovalCodes where N_CompanyId=@nCompanyID and N_SecApprovalID=@nSecApprovalID  ";

                    // MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    // if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                  
                    // MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from vw_SecAppSettings where N_CompanyId=@nCompanyID and N_ModuleID=@nModuleID ";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

    }
}
