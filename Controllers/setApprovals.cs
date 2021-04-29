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
            FormID = 1056;
        }
        private readonly string connectionString;


        [HttpGet("modulelist")]
        public ActionResult GetModulelist(int nCompanyId)
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
            string sqlCommandText = "select N_CompanyID,N_LanguageID,N_ParentMenuID,N_MenuID,X_Text,X_Module FROM vw_UserMenus_List WHERE N_CompanyID=@p1 and N_MenuID not in (85,1,83) group by N_CompanyID,N_LanguageID,N_ParentMenuID,N_MenuID,X_Text,X_Module";
            Params.Add("@p1", nCompanyId);
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
                return Ok(_api.Error(e));
            }
        }



        [HttpGet("screenlist")]
        public ActionResult GetUserlist(int nCompanyId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            //int nCompanyId=myFunctions.GetCompanyID(User);
            
            string sqlCommandText = "select N_MenuID,N_LanguageId,N_ParentMenuID,B_AllowApproval,X_Text from vw_UserPrevilegesDisp where N_CompanyID=@p1";
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
                return Ok(_api.Error(e));
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
                return Ok(_api.Error(e));
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
                return Ok(_api.Error(e));
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
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();

                    int n_SecApprovalID = myFunctions.getIntVAL(MasterRow["N_SecApprovalID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                   // string x_ServiceEndCode = MasterRow["X_ServiceEndCode"].ToString();
                    
                    if (n_SecApprovalID>0)
                    {
                        
                         dLayer.DeleteData("Sec_ApprovalSettings_General", "N_SecApprovalID", n_SecApprovalID, "", connection,transaction);

                    }
                    // if (x_ServiceEndCode == "@Auto")
                    // {
                    //     Params.Add("N_CompanyID", N_CompanyID);
                    //     Params.Add("N_YearID", N_FnYearID);
                    //     Params.Add("N_FormID", N_FormID);
                    //     x_ServiceEndCode = dLayer.GetAutoNumber("Pay_ServiceEnd", "x_ServiceEndCode", Params, connection, transaction);
                    //     if (x_ServiceEndCode == "")
                    //     {
                    //         transaction.Rollback();
                    //         return Ok("Unable to generate Code");
                    //     }
                    //     MasterTable.Rows[0]["X_ServiceEndCode"] = x_ServiceEndCode;
                    // }

                    n_SecApprovalID = dLayer.SaveData("Sec_ApprovalSettings_General", "N_SecApprovalID", "", "", MasterTable, connection, transaction);
                    if (n_SecApprovalID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save ");
                    }
                   

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    // Result.Add("n_ServiceEndID", n_ServiceEndID);
                    // Result.Add("x_ServiceEndCode", x_ServiceEndCode);
                    // Result.Add("n_EndSettiingsID", n_EndSettiingsID);

                    return Ok(_api.Success(Result, "set Approvals Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nSecApprovalID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    
                    connection.Open();
                    Results = dLayer.DeleteData("Sec_ApprovalSettings_General", "N_SecApprovalID", nSecApprovalID, "", connection);
                    if (Results > 0)
                    {
                    
                        
                        return Ok(_api.Success("Set Approvald eleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }


          [HttpGet("details")]
        public ActionResult GenApprovalCode(int nSecApprovalID)
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
                    Params.Add("@nSecApprovalID", nSecApprovalID);
                    // Mastersql = "select * from Gen_ApprovalCodes where N_CompanyId=@nCompanyID and N_SecApprovalID=@nSecApprovalID  ";

                    // MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    // if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                  
                    // MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from vw_SecAppSettings where N_CompanyId=@nCompanyID and N_SecApprovalID=@nSecApprovalID ";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

    }
}
