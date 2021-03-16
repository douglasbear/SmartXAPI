using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("payemployeapprovalcode")]
    [ApiController]
    public class Pay_EmployeApprovalCode : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID;


        public Pay_EmployeApprovalCode(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 202;
        }

        [HttpGet("actionlist")]
        public ActionResult ActionList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select N_CompanyID,N_Action,X_ActionDesc from vw_web_ApprovalAction_Disp where N_CompanyID=@nComapnyID";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        [HttpGet("approvalcodelist")]
        public ActionResult ApprovalCodeList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select N_CompanyID,N_ApprovalID,X_ApprovalCode,X_ApprovalDescription from vw_PayApprovalCodeDisp where N_CompanyID=@nComapnyID";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        //Save....
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


                    int n_ApprovalID = myFunctions.getIntVAL(MasterRow["n_ApprovalID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string x_ApprovalCode = MasterRow["X_ApprovalCode"].ToString();

                    if (x_ApprovalCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", N_FormID);
                         x_ApprovalCode = dLayer.GetAutoNumber("Web_Pay_ApprovalSystem", "X_ApprovalCode", Params, connection, transaction);
                        // x_ApprovalCode = dLayer.GetAutoNumber("Acc_CostCentreMaster", "x_CostCentreCode", Params, connection, transaction);
                        if (x_ApprovalCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate  Approval Code");
                        }
                        MasterTable.Rows[0]["X_ApprovalCode"] = x_ApprovalCode;
                        MasterTable.Columns.Remove("n_FnYearID");
                    }



                    n_ApprovalID = dLayer.SaveData("Web_Pay_ApprovalSystem", "N_ApprovalID", "", "", MasterTable, connection, transaction);
                    if (n_ApprovalID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save approval code");
                    }
                    int n_ApprovalDetailsID = dLayer.SaveData("Web_Pay_ApprovalSystemDetails", "N_ApprovalDetailsID", DetailTable, connection, transaction);
                    if (n_ApprovalDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save approval code");
                    }

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_ApprovalID", n_ApprovalID);
                    Result.Add("x_ApprovalCode", x_ApprovalCode);
                
                    return Ok(_api.Success(Result, "Approval Code Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpGet("details")]
        public ActionResult PayEmployeApprovalCode(int? nCompanyID, string xApprovalCode, int nFnYearID, int nApprovalID)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            DataTable DataTable = new DataTable();

            string Mastersql = "";
            string DetailSql = "";

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (nApprovalID > 0)
                    {
                        Params.Add("@nApprovalID", nApprovalID);
                        Mastersql = "select * from vw_PayApprovalCodeDisp where N_CompanyId=@nCompanyID and N_ApprovalID=@nApprovalID";
                        MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = _api.Format(MasterTable, "Master");
                        DetailSql = "";
                        DetailSql = "select * from vw_PayApprovalCode_dtls where N_CompanyId=@nCompanyID and N_ApprovalID=@nApprovalID ";
                        DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dt.Tables.Add(MasterTable);
                        dt.Tables.Add(DetailTable);
                        return Ok(_api.Success(dt));

                    }

                    Params.Add("@xApprovalCode", xApprovalCode);

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    MasterTable = _api.Format(MasterTable, "Master");

                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList DetailParams = new SortedList();
                    DetailParams.Add("@nCompanyID", nCompanyID);


                    DetailSql = "vw_PayApprovalCode_dtls @nCompanyID,@nFnYearID";
                    SortedList NewParams = new SortedList();
                    NewParams.Add("@nFnYearID", nFnYearID);
                    NewParams.Add("@nCompanyID", nCompanyID);
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, NewParams, connection);
                    DetailTable = _api.Format(DetailTable, "Details");

                    SortedList Param = new SortedList();
                    Param.Add("@nCompanyID", nCompanyID);
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, NewParams, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        // Delete....
        // [HttpDelete("delete")]
        // public ActionResult DeleteData(int nApprovalID,  int nFnYearID)
        // {
        //     int Results = 0;
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();
        //             var xUserCategory = myFunctions.GetUserCategory(User);// User.FindFirst(ClaimTypes.GroupSid)?.Value;
        //             var nUserID = myFunctions.GetUserID(User);// User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //             int nCompanyID = myFunctions.GetCompanyID(User);
        //             object objProcessed = dLayer.ExecuteScalar("Select Isnull(N_SalesID,0) from Inv_SalesOrder where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection, transaction);
        //             if (objProcessed == null) objProcessed = 0;
        //             if (myFunctions.getIntVAL(objProcessed.ToString()) == 0)
        //             {
        //                 SortedList DeleteParams = new SortedList(){
        //                         {"N_CompanyID",nCompanyID},
        //                         // {"X_TransType","SALES ORDER"},
        //                         // {"N_VoucherID",nSalesOrderID},
        //                         // {"N_UserID",nUserID},
        //                         // {"X_SystemName","WebRequest"},
        //                         // {"N_BranchID",nBranchID}};
        //                 Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
        //                 if (Results <= 0)
        //                 {
        //                     transaction.Rollback();
        //                     return Ok(_api.Error("Unable to delete Sales Order"));
        //                 }
        //                 else
        //                 {
        //                     transaction.Commit();
        //                     return Ok(_api.Success("Sales Order deleted"));

        //                 }
        //             }
        //             else
        //             {
        //                 transaction.Rollback();
        //                 return Ok(_api.Error("Sales invoice processed! Unable to delete Sales Order"));

        //             }


        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();
        //             Results = dLayer.DeleteData("Inv_SalesOrderDetails", "N_SalesOrderID", nSalesOrderID, "", connection, transaction);
        //             if (Results <= 0)
        //             {
        //                 transaction.Rollback();
        //                 return Ok(_api.Error("Unable to delete sales order"));
        //             }
        //             else
        //             {
        //             Results = dLayer.DeleteData("Inv_SalesOrder", "N_SalesOrderID", nSalesOrderID, "", connection, transaction);

        //             }

        //             if (Results > 0)
        //             {
        //                 transaction.Commit();
        //                 return Ok(_api.Error("Sales order deleted"));
        //             }
        //             else
        //             {
        //                 transaction.Rollback();
        //                 return Ok(_api.Error("Unable to delete sales order"));
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(_api.Error(ex));
        //     }


    }
}
