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
    [Route("loanAdjustment")]
    [ApiController]
    public class Pay_LoanAdjustments : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Pay_LoanAdjustments(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("list")]
        public ActionResult GetLoanAdjustment()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            string sqlCommandText="Select [Loan ID],[Employee No],Name,Position,[Loan Amount],[Issue Date],[Status],RefundAmoount AS N_RefundAmount,N_LoanTransID from vw_PayLoanIssue_Status Where N_CompanyID=@nCompanyID and N_LoanStatus=0 order by D_LoanIssueDate DESC";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }


        // [HttpGet("status")]
        // public ActionResult LoanAdjustmentStatus(int nLoanID, int nFnYearId)
        // {
        //     DataSet ST = new DataSet();

        //     SortedList Params = new SortedList();
        //     int nCompanyID = myFunctions.GetCompanyID(User);
        //     Params.Add("@nCompanyID", nCompanyID);
        //     DataTable statusTable = new DataTable();
        //     string sqlCommandText = "select SUM(N_RefundAmount) as Total from Pay_LoanIssueDetails inner join Pay_LoanIssue on Pay_LoanIssueDetails.N_LoanTransID=Pay_LoanIssue.N_LoanTransID and Pay_LoanIssueDetails.N_CompanyID=Pay_LoanIssue.N_CompanyID where Pay_LoanIssue.N_LoanID =" + nLoanID + " and Pay_LoanIssue.N_CompanyID=" + nCompanyID + " and Pay_LoanIssue.N_FnYearID =" + nFnYearId;

        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             statusTable = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
        //         }
        //         statusTable = api.Format(statusTable, "status");
        //         ST.Tables.Add(statusTable);

        //         if (statusTable.Rows.Count == 0)
        //         {
        //             return Ok(api.Warning("No Results Found"));
        //         }
        //         else
        //         {
        //             return Ok(api.Success(ST));
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(api.Error(User,e));
        //     }

        // }








        [HttpGet("details")]
        public ActionResult LoanAdjustmentDetails(int nLoanID, int nFnYearId, bool bAllBranchData, int nBranchID)
        {

            DataSet DS = new DataSet();
            SortedList Params = new SortedList();
            SortedList Params1 = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string xCondition = "";
            string sqlAdjustment = "";
            string sqlLoan = "";

            DataTable dtAdjustment = new DataTable();
            DataTable dtLoan = new DataTable();

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", nLoanID);
            Params.Add("@p4", nBranchID);
            Params1.Add("@p1", nCompanyId);
           


            if (nFnYearId == 0)
            {
                if (bAllBranchData == true)
                    xCondition = "(dbo.Pay_LoanIssue.N_CompanyID = @p1)  AND (dbo.Pay_LoanIssue.n_LoanID =@p3) AND (dbo.Pay_LoanIssue.N_FnYearID =@p2) and dbo.Pay_LoanIssue.N_LoanStatus=0";
                else
                    xCondition = "(dbo.Pay_LoanIssue.N_CompanyID = @p1)  AND (dbo.Pay_LoanIssue.n_LoanID =@p3) AND (dbo.Pay_LoanIssue.N_FnYearID =@p2) AND (dbo.Pay_LoanIssue.N_BranchID = @p4) and dbo.Pay_LoanIssue.N_LoanStatus=0";

                sqlAdjustment = "SELECT     dbo.Pay_LoanIssue.*, dbo.Pay_Employee.X_EmpCode, dbo.Pay_Employee.X_EmpName, dbo.Pay_PayMaster.X_Description ,Acc_MastLedger.X_LedgerCode FROM         dbo.Pay_LoanIssue INNER JOIN dbo.Pay_Employee ON dbo.Pay_LoanIssue.N_EmpID = dbo.Pay_Employee.N_EmpID AND dbo.Pay_LoanIssue.N_CompanyID = dbo.Pay_Employee.N_CompanyID AND dbo.Pay_LoanIssue.N_FnYearID=dbo.Pay_Employee.N_FnYearID INNER JOIN dbo.Pay_PayMaster ON dbo.Pay_LoanIssue.N_PayID = dbo.Pay_PayMaster.N_PayID AND dbo.Pay_LoanIssue.N_CompanyID = dbo.Pay_PayMaster.N_CompanyID LEFT Outer Join Acc_MastLedger On Pay_LoanIssue.N_DefLedgerID = Acc_MastLedger.N_LedgerID AND dbo.Pay_LoanIssue.N_FnYearID=Acc_MastLedger.N_FnYearID AND dbo.Pay_LoanIssue.N_CompanyID=Acc_MastLedger.N_CompanyID  WHERE " + xCondition + "";
            }
            else
            {
                if (bAllBranchData == true)
                    xCondition = "(dbo.Pay_LoanIssue.N_CompanyID = @p1)  AND (dbo.Pay_LoanIssue.n_LoanID =@p3) AND (dbo.Pay_LoanIssue.N_FnYearID =@p2) and dbo.Pay_LoanIssue.N_LoanStatus=0";
                else
                    xCondition = "(dbo.Pay_LoanIssue.N_CompanyID = @p1)  AND (dbo.Pay_LoanIssue.n_LoanID =@p3) AND (dbo.Pay_LoanIssue.N_FnYearID =@p2) AND (dbo.Pay_LoanIssue.N_BranchID = @p4) and dbo.Pay_LoanIssue.N_LoanStatus=0";

                sqlAdjustment = "SELECT dbo.Pay_LoanIssue.*, dbo.Pay_Employee.X_EmpCode, dbo.Pay_Employee.X_EmpName, dbo.Pay_PayMaster.X_Description ,Acc_MastLedger.X_LedgerCode FROM dbo.Pay_LoanIssue INNER JOIN dbo.Pay_Employee ON dbo.Pay_LoanIssue.N_EmpID = dbo.Pay_Employee.N_EmpID AND dbo.Pay_LoanIssue.N_CompanyID = dbo.Pay_Employee.N_CompanyID AND dbo.Pay_LoanIssue.N_FnYearID=dbo.Pay_Employee.N_FnYearID INNER JOIN dbo.Pay_PayMaster ON dbo.Pay_LoanIssue.N_PayID = dbo.Pay_PayMaster.N_PayID AND dbo.Pay_LoanIssue.N_CompanyID = dbo.Pay_PayMaster.N_CompanyID LEFT Outer Join Acc_MastLedger On Pay_LoanIssue.N_DefLedgerID = Acc_MastLedger.N_LedgerID AND dbo.Pay_LoanIssue.N_FnYearID=Acc_MastLedger.N_FnYearID AND dbo.Pay_LoanIssue.N_CompanyID=Acc_MastLedger.N_CompanyID WHERE " + xCondition + "";

            }



            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dtAdjustment = dLayer.ExecuteDataTable(sqlAdjustment, Params, connection);
                    object InsAmt = dLayer.ExecuteScalar("select SUM(N_RefundAmount)  from Pay_LoanIssueDetails inner join Pay_LoanIssue on Pay_LoanIssueDetails.N_LoanTransID=Pay_LoanIssue.N_LoanTransID and Pay_LoanIssueDetails.N_CompanyID=Pay_LoanIssue.N_CompanyID where Pay_LoanIssue.N_LoanID =" + nLoanID + " and Pay_LoanIssue.N_CompanyID=" + nCompanyId + " and Pay_LoanIssue.N_FnYearID =" + nFnYearId + "", Params1, connection);
                    int nLoanTransID = 0;
                    nLoanTransID = myFunctions.getIntVAL(dtAdjustment.Rows[0]["N_LoanTransID"].ToString());
                    Params.Add("@p5", nLoanTransID);

                    sqlLoan = "SELECT  ROW_NUMBER() over(ORDER BY  N_LoanTransDetailsID) as SlNo,Pay_LoanIssueDetails.N_LoanTransDetailsID,Pay_LoanIssueDetails.N_InstAmount,Pay_LoanIssueDetails.N_InstActualAmt,Pay_LoanIssueDetails.N_RefundAmount,Pay_LoanIssueDetails.D_DateFrom,Pay_LoanIssueDetails.D_DateTo,CONVERT(VARCHAR(3),Pay_LoanIssueDetails.D_DateFrom,100)+' - '+ CAST(datepart(year,Pay_LoanIssueDetails.D_DateFrom)As varchar) As X_Month FROm Pay_LoanIssueDetails Where Pay_LoanIssueDetails.N_LoanTransID=" + nLoanTransID + " and  Pay_LoanIssueDetails.N_CompanyID =" + nCompanyId + " and N_LoanTransDetailsID not in (Select N_LoanTransDetailsID From Pay_LoanIssueDetails Where N_LoanTransID=" + nLoanTransID + " and(N_RefundAmount=0 OR N_RefundAmount IS NULL) and N_TransDetailsID IS NOT NULL and B_IsLoanClose=1)";
                    dtLoan = dLayer.ExecuteDataTable(sqlLoan, connection);

                    if (InsAmt != null)
                    {
                        dtAdjustment = myFunctions.AddNewColumnToDataTable(dtAdjustment, "N_TotalAmount", typeof(double), myFunctions.getVAL(InsAmt.ToString()));
                    }
                    else
                    {
                         dtAdjustment = myFunctions.AddNewColumnToDataTable(dtAdjustment, "N_TotalAmount", typeof(double), 0);

                    }

                }


                dtAdjustment = api.Format(dtAdjustment, "master");
                dtLoan = api.Format(dtLoan, "details");
                DS.Tables.Add(dtAdjustment);
                DS.Tables.Add(dtLoan);

                if (dtAdjustment.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(DS));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }



        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable DetailTable;
                DetailTable = ds.Tables["details"];
               int nCompanyId = myFunctions.GetCompanyID(User);;
                //int nFnYearId = myFunctions.getIntVAL(DetailTable.Rows[0]["n_FnYearId"].ToString());
                int nLoanTransID = myFunctions.getIntVAL(DetailTable.Rows[0]["N_LoanTransID"].ToString());
                 DetailTable.Columns.Remove("n_CalculatedAmt");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();


                    if (nLoanTransID > 0)
                    {
                        foreach (DataRow var in DetailTable.Rows)
                        {
                            int nLoanTransDetailsID = dLayer.SaveData("Pay_LoanIssueDetails", "N_LoanTransDetailsID", DetailTable, connection, transaction);
                            if (nLoanTransDetailsID <= 0)
                            {
                                transaction.Rollback();
                                return Ok(api.Error(User,"Unable to save"));
                            }
                        }

                        transaction.Commit();
                    }
                    return Ok(api.Success("Adjustment Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }
    }
}