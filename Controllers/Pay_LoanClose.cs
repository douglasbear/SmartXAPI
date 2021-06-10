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
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("loanClose")]
    [ApiController]
    public class Pay_LoanClose : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 553;

        public Pay_LoanClose(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("details")]
        public ActionResult LoanCloseDetails(string xLoanCloseCode, int nLoanID, int nBranchID, bool bAllBranchData)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nLoanTransID = 0;
            string sqlCommandText = "";
            string X_condition = "";
            if (xLoanCloseCode == null) xLoanCloseCode = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    if (nLoanID != 0)
                    {
                        object ID = dLayer.ExecuteScalar("Select N_LoanTransID  From Pay_LoanIssue Where N_LoanID=" + nLoanID + " and N_CompanyID = " + nCompanyId + "", connection, transaction);
                        if (ID != null)
                            nLoanTransID = myFunctions.getIntVAL(ID.ToString());
                    }

                    if (nLoanTransID != 0)
                    {
                        object Code = dLayer.ExecuteScalar("Select X_LoanClosingCode  From Pay_LoanClose Where N_LoanTransID=" + nLoanTransID + " and N_CompanyID = " + nCompanyId + "", connection, transaction);
                        if (Code != null)
                            xLoanCloseCode = Code.ToString();

                        if (bAllBranchData == true)
                            X_condition = "(dbo.Pay_LoanIssue.N_CompanyID = @p1)  AND (dbo.Pay_LoanIssue.n_LoanTransID =@p3) AND(dbo.Pay_LoanIssue.N_LoanStatus=0)";
                        else
                            X_condition = "(dbo.Pay_LoanIssue.N_CompanyID = @p1)  AND (dbo.Pay_LoanIssue.n_LoanTransID =@p3) AND (dbo.Pay_LoanIssue.N_BranchID =@p4)AND(dbo.Pay_LoanIssue.N_LoanStatus=0)";


                        sqlCommandText = "SELECT     dbo.Pay_LoanIssue.*, dbo.Pay_Employee.X_EmpCode, dbo.Pay_Employee.X_EmpName, dbo.Pay_PayMaster.X_Description ,Acc_MastLedger.X_LedgerCode FROM  dbo.Pay_LoanIssue INNER JOIN dbo.Pay_Employee ON dbo.Pay_LoanIssue.N_EmpID = dbo.Pay_Employee.N_EmpID AND dbo.Pay_LoanIssue.N_CompanyID = dbo.Pay_Employee.N_CompanyID AND dbo.Pay_LoanIssue.N_FnYearID=dbo.Pay_Employee.N_FnYearID INNER JOIN dbo.Pay_PayMaster ON dbo.Pay_LoanIssue.N_PayID = dbo.Pay_PayMaster.N_PayID AND dbo.Pay_LoanIssue.N_CompanyID = dbo.Pay_PayMaster.N_CompanyID " +
                            "LEFT Outer Join Acc_MastLedger On Pay_LoanIssue.N_DefLedgerID = Acc_MastLedger.N_LedgerID AND dbo.Pay_LoanIssue.N_FnYearID=Acc_MastLedger.N_FnYearID AND dbo.Pay_LoanIssue.N_CompanyID=Acc_MastLedger.N_CompanyID  WHERE " + X_condition + "";
                    }

                    if (xLoanCloseCode != "")
                        sqlCommandText = "select *,Name AS X_EmpName,[Paid Amount] AS n_PaidAmount from vw_PayLoanClose where N_CompanyID=@p1 and Code=@p2";

                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", xLoanCloseCode);
                    Params.Add("@p3", nLoanTransID);
                    Params.Add("@p4", nBranchID);

                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection, transaction);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    double N_RefundAmount = 0;
                    if (xLoanCloseCode != "")
                    {
                        nLoanTransID = myFunctions.getIntVAL(dt.Rows[0]["N_LoanTransID"].ToString());
                    }
                    N_RefundAmount = myFunctions.getVAL(dLayer.ExecuteScalar("Select SUM(N_RefundAmount) As N_RefundAmount  From Pay_LoanIssueDetails Where Pay_LoanIssueDetails.N_LoanTransID=" + nLoanTransID + " and  Pay_LoanIssueDetails.N_CompanyID = " + nCompanyId + "", connection, transaction).ToString());

                    double balance = myFunctions.getVAL(dt.Rows[0]["N_LoanAmount"].ToString()) - N_RefundAmount;
                    dt = myFunctions.AddNewColumnToDataTable(dt, "x_Balance", typeof(string), balance);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "n_RefundAmount", typeof(string), N_RefundAmount);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }



        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                SortedList QueryParams = new SortedList();
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nLoanCloseID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LoanCloseID"].ToString());
                int nLoanTransID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LoanTransID"].ToString());
                int nLoanID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LoanID"].ToString());
                var dDateFrom = MasterTable.Rows[0]["D_PaidDate"].ToString();
                int nRateOfAmount = myFunctions.getIntVAL(MasterTable.Rows[0]["n_RateOfAmount"].ToString());
                int nPaidAmount = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PaidAmount"].ToString());
                int nBalanceAmount = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BalanceAmount"].ToString());
                DateTime dStartDate = Convert.ToDateTime(MasterTable.Rows[0]["D_PaidDate"].ToString());
                MasterTable.Columns.Remove("n_BalanceAmount");


                QueryParams.Add("N_CompanyID", nCompanyID);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();

                    // Auto Gen





                    string LoanCloseCode = "";
                    var values = MasterTable.Rows[0]["X_LoanClosingCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("N_LoanCloseID", nLoanCloseID);
                        LoanCloseCode = dLayer.GetAutoNumber("Pay_LoanClose", "X_LoanClosingCode", Params, connection, transaction);
                        if (LoanCloseCode == "") { transaction.Rollback(); return Ok(api.Error("Unable to generate Lead Code")); }
                        MasterTable.Rows[0]["X_LoanClosingCode"] = LoanCloseCode;
                    }
                    SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"N_FnYearID", nFnYearId},
                                {"X_TransType","ELC"},
                                {"X_ReferenceNo",values},
                                };
                    if (!ValidateCloseDate(nLoanID, dDateFrom, QueryParams, connection, transaction))
                    {
                        return Ok(api.Warning("Invalid Loan Close by date!!!"));
                    }

                    if (nLoanCloseID > 0)
                    {

                        object N_LoanTransID = dLayer.ExecuteScalar("Select N_LoanTransID from vw_PayLoanClose where N_CompanyID=N_CompanyID and N_LoanCloseID=N_LoanCloseID", Params, connection);
                        Params.Add("N_LoanTransID", N_LoanTransID);

                        dLayer.ExecuteNonQueryPro("SP_Pay_LoanClosingVoucher_Del", DeleteParams, connection, transaction);
                        dLayer.ExecuteNonQuery("Update Pay_LoanIssueDetails set N_RefundAmount =Null,D_RefundDate =Null,N_PayRunID =Null,N_TransDetailsID =Null,B_IsLoanClose =Null  where N_LoanTransID= N_LoanTransID and N_CompanyID=N_CompanyID and B_IsLoanClose=1 and N_TransDetailsID=N_LoanCloseID", Params, connection, transaction);
                        dLayer.ExecuteNonQuery("Delete from Pay_LoanClose where N_LoanCloseID=N_LoanCloseID and N_CompanyID=N_CompanyID", Params, connection, transaction);
                    }


                    nLoanCloseID = dLayer.SaveData("Pay_LoanClose", "N_LoanCloseID", MasterTable, connection, transaction);
                    if (nLoanCloseID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    dLayer.ExecuteNonQuery("Update Pay_LoanIssueDetails Set B_IsLoanClose =1,N_TransDetailsID=" + nLoanCloseID + "  Where N_CompanyID =" + nCompanyID + " and N_LoanTransID=" + nLoanTransID + " and(N_RefundAmount=0 OR N_RefundAmount IS Null)", Params, connection, transaction);

                    DataTable dt = new DataTable();
                    dt.Clear();
                    dt.Columns.Add("N_LoanTransDetailsID");
                    dt.Columns.Add("N_CompanyID");
                    dt.Columns.Add("N_LoanTransID");
                    dt.Columns.Add("D_DateFrom");
                    dt.Columns.Add("D_DateTo");
                    dt.Columns.Add("D_RefundDate");
                    dt.Columns.Add("B_IsLoanClose");
                    dt.Columns.Add("N_TransDetailsID");
                    dt.Columns.Add("N_TransDetailsID");
                    dt.Columns.Add("N_RefundAmount");

                    DateTime Start = new DateTime(Convert.ToDateTime(dDateFrom.ToString()).Year, Convert.ToDateTime(dDateFrom.ToString()).Month, 1);

                    nLoanCloseID = myFunctions.getIntVAL(nLoanCloseID.ToString());
                    double Balance = 0;
                    object N_LoanTransDetailsID = 0;
                    // for (int i = 1; i <= nInstNos; i++)
                    // {
                    DateTime End = new DateTime(Start.AddMonths(1).Year, Start.AddMonths(1).Month, 1).AddDays(-1);
                    DataRow row = dt.NewRow();
                    row["N_LoanTransDetailsID"] = 0;
                    row["N_CompanyID"] = nCompanyID;
                    row["N_LoanTransID"] = nLoanTransID;
                    row["D_DateFrom"] = myFunctions.getDateVAL(Start);
                    row["D_DateTo"] = myFunctions.getDateVAL(End);
                    row["D_RefundDate"] = myFunctions.getDateVAL(End);
                    row["B_IsLoanClose"] = "1";
                    row["N_TransDetailsID"] = nLoanCloseID;
                    row["N_LoanTransDetailsID"] = 0;
                    row["N_RefundAmount"] = 0;
                    Start = Start.AddMonths(1);

                    if (nPaidAmount == 0 && nRateOfAmount != 0)
                    {
                        row["N_RefundAmount"] = nRateOfAmount;
                        dt.Rows.Add(row);
                        N_LoanTransDetailsID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select MAX(N_LoanTransDetailsID)+1 From Pay_LoanIssueDetails Where N_CompanyID=" + nCompanyID, QueryParams, connection, transaction).ToString());

                        N_LoanTransDetailsID = dLayer.SaveData("Pay_LoanIssueDetails", "N_LoanTransDetailsID", dt, connection, transaction);
                        Balance = (nBalanceAmount - nRateOfAmount);
                    }
                    else if (nPaidAmount != 0 && nRateOfAmount == 0)
                    {
                        row["N_RefundAmount"] = nPaidAmount;
                        dt.Rows.Add(row);
                        N_LoanTransDetailsID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select MAX(N_LoanTransDetailsID)+1 From Pay_LoanIssueDetails Where N_CompanyID=" + nCompanyID, QueryParams, connection, transaction).ToString());
                        N_LoanTransDetailsID = dLayer.SaveData("Pay_LoanIssueDetails", "N_LoanTransDetailsID", dt, connection, transaction);

                        Balance = (nBalanceAmount - nPaidAmount);
                    }
                    else if (nPaidAmount != 0 && nRateOfAmount != 0)
                    {
                        row["N_RefundAmount"] = nPaidAmount;
                        dt.Rows.Add(row);
                        N_LoanTransDetailsID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select MAX(N_LoanTransDetailsID)+1 From Pay_LoanIssueDetails Where N_CompanyID=" + nCompanyID, QueryParams, connection, transaction).ToString());
                        dLayer.SaveData("Pay_LoanIssueDetails", "N_LoanTransDetailsID", dt, connection, transaction);
                        if (myFunctions.getIntVAL(N_LoanTransDetailsID.ToString()) <= 0)
                        {
                            transaction.Rollback();
                            return Ok(api.Error("Unable to save"));
                        }
                        else
                        {
                            DataTable table = new DataTable();
                            table.Clear();
                            table.Columns.Add("N_LoanTransDetailsID");
                            table.Columns.Add("N_CompanyID");
                            table.Columns.Add("N_LoanTransID");
                            table.Columns.Add("D_DateFrom");
                            table.Columns.Add("D_DateTo");
                            table.Columns.Add("D_RefundDate");
                            table.Columns.Add("B_IsLoanClose");
                            table.Columns.Add("N_TransDetailsID");
                            table.Columns.Add("N_TransDetailsID");
                            table.Columns.Add("N_RefundAmount");

                            DateTime Start1 = new DateTime(Convert.ToDateTime(dDateFrom.ToString()).Year, Convert.ToDateTime(dDateFrom.ToString()).Month, 1);

                            nLoanCloseID = myFunctions.getIntVAL(nLoanCloseID.ToString());
                            DateTime End1 = new DateTime(Start.AddMonths(1).Year, Start.AddMonths(1).Month, 1).AddDays(-1);
                            DataRow row1 = table.NewRow();
                            row1["N_LoanTransDetailsID"] = 0;
                            row1["N_CompanyID"] = nCompanyID;
                            row1["N_LoanTransID"] = nLoanTransID;
                            row1["D_DateFrom"] = myFunctions.getDateVAL(Start);
                            row1["D_DateTo"] = myFunctions.getDateVAL(End);
                            row1["D_RefundDate"] = myFunctions.getDateVAL(End);
                            row1["B_IsLoanClose"] = "1";
                            row1["N_TransDetailsID"] = nLoanCloseID;
                            row1["N_RefundAmount"] = nRateOfAmount;

                            N_LoanTransDetailsID = myFunctions.getIntVAL(N_LoanTransDetailsID.ToString()) + 1;
                            row1["N_LoanTransDetailsID"] = N_LoanTransDetailsID;
                            table.Rows.Add(row1);


                            N_LoanTransDetailsID = dLayer.SaveData("Pay_LoanIssueDetails", "N_LoanTransDetailsID", table, connection, transaction);


                        }
                        Balance = nBalanceAmount - (nPaidAmount) + nRateOfAmount;
                    }
                    if (Balance > 0)
                    {
                        object N_LoanTransDetails = myFunctions.getIntVAL(N_LoanTransDetailsID.ToString()) + 1;
                        DateTime StartDate;
                        if (dStartDate.Month == 12)
                            StartDate = new DateTime(dStartDate.Year + 1, 1, 1);
                        else
                            StartDate = new DateTime(dStartDate.Year, dStartDate.Month + 1, 1);

                        DataTable newTable = new DataTable();
                        newTable.Clear();
                        newTable.Columns.Add("N_LoanTransDetailsID");
                        newTable.Columns.Add("N_CompanyID");
                        newTable.Columns.Add("N_LoanTransID");
                        newTable.Columns.Add("D_DateFrom");
                        newTable.Columns.Add("D_DateTo");
                        newTable.Columns.Add("N_TransDetailsID");
                        newTable.Columns.Add("N_InstAmount");


                        DateTime Start1 = new DateTime(Convert.ToDateTime(dDateFrom.ToString()).Year, Convert.ToDateTime(dDateFrom.ToString()).Month, 1);

                        nLoanCloseID = myFunctions.getIntVAL(nLoanCloseID.ToString());
                        DateTime End1 = new DateTime(Start.AddMonths(1).Year, Start.AddMonths(1).Month, 1).AddDays(-1);
                        DataRow newRow = newTable.NewRow();
                        newRow["N_CompanyID"] = nCompanyID;
                        newRow["N_LoanTransID"] = nLoanTransID;
                        newRow["D_DateFrom"] = myFunctions.getDateVAL(Start);
                        newRow["D_DateTo"] = myFunctions.getDateVAL(End);
                        newRow["N_TransDetailsID"] = 0;
                        N_LoanTransDetailsID = myFunctions.getIntVAL(N_LoanTransDetailsID.ToString()) + 1;
                        newRow["N_LoanTransDetailsID"] = N_LoanTransDetailsID;
                        newRow["N_InstAmount"] = myFunctions.getVAL(Balance.ToString());
                        newTable.Rows.Add(newRow);




                        N_LoanTransDetailsID = dLayer.SaveData("Pay_LoanIssueDetails", "N_LoanTransDetailsID", newTable, connection, transaction);


                    }
                    if (myFunctions.getIntVAL(N_LoanTransDetailsID.ToString()) <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save Loan Request"));
                    }

                    SortedList ClosingParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"N_TransID", nLoanCloseID},
                                {"N_UserID","2"},
                                {"X_SystemName","Online"},
                                {"X_EntryFrom","Online"},
                                };

                    dLayer.ExecuteNonQueryPro("SP_Pay_LoanClosingVoucher_Del", DeleteParams, connection, transaction);
                     dLayer.ExecuteNonQueryPro("SP_Pay_LoanClosing", ClosingParams, connection, transaction);

                    transaction.Commit();


                    return Ok(api.Success("Loan Closed"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int xLoanClose, int nFnYearId)
        {

            int Results = 0;
            int nCompanyId = myFunctions.GetCompanyID(User);
            try
            {
                SortedList Params = new SortedList();
                int N_LoanCloseID = 0;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyId},
                                {"N_YearID", nFnYearId},
                                {"X_TransType","ELC"},
                                {"X_ReferenceNo",xLoanClose},
                                };
                    N_LoanCloseID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select N_LoanCloseID from Pay_LoanClose where N_CompanyID=N_CompanyID and X_LoanClosingCode=X_ReferenceNo", Params, connection).ToString());
                    dLayer.ExecuteNonQueryPro("SP_Pay_LoanClosingVoucher_Del", DeleteParams, connection, transaction);
                    dLayer.ExecuteNonQuery("Update Pay_LoanIssueDetails set N_RefundAmount =Null,D_RefundDate =Null,N_PayRunID =Null,N_TransDetailsID =Null,B_IsLoanClose =Null  where N_LoanTransID= N_LoanTransID and N_CompanyID=N_CompanyID and B_IsLoanClose=1 and N_TransDetailsID=N_LoanCloseID", Params, connection, transaction);
                    Results = dLayer.DeleteData("Pay_LoanIssueDetails", "N_LoanTransID", N_LoanCloseID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("N_LoanCloseID", N_LoanCloseID.ToString());
                    return Ok(api.Success(res, "Loan Close deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to Loan Close"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }



        }

        private bool ValidateCloseDate(int nLoanID, string dDateFrom, SortedList QueryParams, SqlConnection connection, SqlTransaction transaction)
        {
            object issue = dLayer.ExecuteScalar("select Pay_LoanIssue.D_LoanIssueDate from Pay_LoanIssue left join Pay_LoanClose ON Pay_LoanClose.N_LoanTransID=Pay_LoanIssue.N_LoanTransID where Pay_LoanIssue.N_LoanID=" + nLoanID, QueryParams, connection, transaction);
            DateTime issue1 = Convert.ToDateTime(issue.ToString());
            DateTime dDateFrom1 = Convert.ToDateTime(dDateFrom.ToString());

            if (dDateFrom1 < issue1)
                return false;
            return true;



        }
    }
}