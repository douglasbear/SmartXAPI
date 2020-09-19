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
using System.Linq;
using System.Security.Claims;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("employeeloanrequest")]
    [ApiController]



    public class Ess_EmployeeLoanRequest : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly string xTransType;
        private readonly int FormID;

        public Ess_EmployeeLoanRequest(IDataAccessLayer dl, IApiFunctions _api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            api = _api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 212;
            xTransType = "LOAN ISSUE";
        }


      
        [HttpGet("loanList")]
        public ActionResult GetEmployeeLoanRequest(string xReqType)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int nUserID = api.GetUserID(User);
            int nCompanyID = api.GetCompanyID(User);
            QueryParams.Add("@nCompanyID", nCompanyID);
            QueryParams.Add("@nUserID", nUserID);
            string sqlCommandText = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object nEmpID = dLayer.ExecuteScalar("Select N_EmpID From Sec_User where N_UserID=@nUserID and N_CompanyID=@nCompanyID", QueryParams, connection);
                    if (nEmpID != null)
                    {
                        QueryParams.Add("@nEmpID", myFunctions.getIntVAL(nEmpID.ToString()));
                        QueryParams.Add("@xStatus", xReqType);
                        if (xReqType.ToLower() == "all")
                            sqlCommandText = "Select * From vw_Pay_LoanIssueList where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID order by D_LoanIssueDate Desc";
                        else
                        if (xReqType.ToLower() == "pending")
                            sqlCommandText = "select * from vw_Pay_LoanIssueList where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and X_Status not in ('Reject','Approved')  order by D_LoanIssueDate Desc ";
                        else
                            sqlCommandText = "Select * From vw_Pay_LoanIssueList where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and X_Status=@xStatus order by D_LoanIssueDate Desc";

                        dt = dLayer.ExecuteDataTable(sqlCommandText, QueryParams, connection);
                    }


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
                return BadRequest(api.Error(e));
            }
        }

         [HttpGet("details")]
        public ActionResult GetEmployeeLoanDetails(int nLoanID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

           int companyid = api.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@nLoanID", nLoanID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string _sqlQuery = "SELECT     Pay_LoanIssue.N_CompanyID, Pay_LoanIssue.N_EmpID, Pay_LoanIssue.N_LoanTransID, Pay_LoanIssue.D_LoanIssueDate, Pay_LoanIssue.D_EntryDate,  Pay_LoanIssue.X_Remarks, Pay_LoanIssue.D_LoanPeriodFrom, Pay_LoanIssue.D_LoanPeriodTo, Pay_LoanIssue.N_LoanAmount, Pay_LoanIssue.N_LoanID, Pay_LoanIssue.N_PayID, Pay_LoanIssue.N_Installments, Pay_LoanIssue.N_DefLedgerID, Pay_LoanIssue.X_Paymentmethod, Pay_LoanIssue.X_ChequeNo, Pay_LoanIssue.D_ChequeDate, Pay_LoanIssue.N_UserID, Pay_LoanIssue.X_BankName, Pay_LoanIssue.N_FnYearID, Pay_LoanIssue.N_LoanStatus,  Pay_LoanIssue.B_OpeningBal, Pay_LoanIssue.N_BranchID, Pay_LoanIssue.N_WebLoanId, Pay_LoanIssue.N_ApprovalLevelId, Pay_LoanIssue.N_ProcStatus, Pay_LoanIssue.N_NextApprovalID, Pay_LoanIssue.B_IsSaveDraft, Pay_LoanIssue.X_Comments, Pay_LoanIssue.X_Guarantor1, Pay_LoanIssue.X_Guarantor2, Pay_LoanIssue.X_RefFrom, Pay_LoanIssue.N_RefID,Pay_Employee.X_EmpCode, Pay_Employee.X_EmpName, Pay_Employee.N_EmpID FROM         Pay_LoanIssue LEFT OUTER JOIN Pay_Employee ON Pay_LoanIssue.N_EmpID = Pay_Employee.N_EmpID AND Pay_LoanIssue.N_CompanyID = Pay_Employee.N_CompanyID  where Pay_LoanIssue.N_LoanID=@nLoanID and Pay_LoanIssue.N_CompanyID=@nCompanyID";
                
                        dt = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);


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
                return BadRequest(api.Error(e));
            }
        }
        
        [HttpPost("save")]
        public ActionResult SaveLoanRequest([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable Approvals;
                MasterTable = ds.Tables["master"];
                Approvals = ds.Tables["approval"];
                DataRow ApprovalRow = Approvals.Rows[0];
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();
                // Auto Gen
                DataRow MasterRow = MasterTable.Rows[0];

                int nUserID=api.GetUserID(User);
               

                string xLoanID = MasterRow["n_LoanID"].ToString();
                int nLoanTransID = myFunctions.getIntVAL(MasterRow["n_LoanTransID"].ToString());
                int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyId"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                int nEmpID = myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());
                var dDateFrom = MasterRow["d_LoanPeriodFrom"].ToString();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nEmpID", nEmpID);
                //QueryParams.Add("@nLoanTransID", nLoanTransID);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList EmpParams = new SortedList();
                        EmpParams.Add("@nCompanyID", nCompanyID);
                        EmpParams.Add("@nEmpID", nEmpID);
                    object objEmpName = dLayer.ExecuteScalar("Select X_EmpName From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID", EmpParams, connection,transaction);

                    if(!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString())){
                        int N_PkeyID = nLoanTransID;
                        string X_Criteria = "N_LoanTransID=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                        myFunctions.UpdateApproverEntry(Approvals,"Pay_LoanIssue", X_Criteria,N_PkeyID,nCompanyID,nUserID,dLayer,connection,transaction );
                        myFunctions.logApprovals(Approvals,api.GetCompanyID(User),nFnYearID,this.xTransType, N_PkeyID, xLoanID, DateTime.Now,api.GetUserID(User),api.GetUserCategory(User),1,objEmpName.ToString(),0,"",dLayer,connection,transaction);
                        transaction.Commit();
                        return Ok(api.Success("Loan request Approval updated" + "-" + xLoanID));
                    }


                    if (xLoanID == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        xLoanID = dLayer.GetAutoNumber("Pay_LoanIssue", "n_LoanID", Params, connection, transaction);
                        if (xLoanID == "") { return Ok( api.Error( "Unable to generate Loan ID")); }
                        MasterTable.Rows[0]["n_LoanID"] = xLoanID;
                    }
                    else
                    {
                        dLayer.DeleteData("Pay_LoanIssue", "n_LoanTransID", nLoanTransID, "", connection, transaction);
                    }

                    int nInstAmount = myFunctions.getIntVAL( MasterTable.Rows[0]["n_InstallmentAmount"].ToString());
                    int nInstNos = myFunctions.getIntVAL( MasterTable.Rows[0]["n_Installments"].ToString());
                    MasterTable.Columns.Remove("n_InstallmentAmount");
                    MasterTable.AcceptChanges();

                    MasterTable = myFunctions.saveApprovals(MasterTable,Approvals,dLayer,connection,transaction);
                    nLoanTransID = dLayer.SaveData("Pay_LoanIssue", "n_LoanTransID", MasterTable, connection, transaction);
                    if (nLoanTransID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save Loan Request"));
                    }
                    else
                    {


                        myFunctions.logApprovals(Approvals,api.GetCompanyID(User),nFnYearID,this.xTransType, nLoanTransID, xLoanID, DateTime.Now,api.GetUserID(User),api.GetUserCategory(User),1,objEmpName.ToString(),0,"",dLayer,connection,transaction);
                    
                        DataTable dt=new DataTable(); 
                        dt.Clear();
                        dt.Columns.Add("N_LoanTransDetailsID");
                        dt.Columns.Add("N_CompanyID");
                        dt.Columns.Add("N_LoanTransID");
                        dt.Columns.Add("D_DateFrom");
                        dt.Columns.Add("D_DateTo");
                        dt.Columns.Add("N_InstAmount");

                        DateTime Start = new DateTime(Convert.ToDateTime(dDateFrom.ToString()).Year, Convert.ToDateTime(dDateFrom.ToString()).Month, 1);

                        for (int i = 1; i <= nInstNos; i++)
                        {
                            DateTime End = new DateTime(Start.AddMonths(1).Year, Start.AddMonths(1).Month, 1).AddDays(-1);
                            DataRow row = dt.NewRow();
                            row["N_LoanTransDetailsID"] = 0;
                            row["N_CompanyID"] = nCompanyID;
                            row["N_LoanTransID"] = nLoanTransID;
                            row["D_DateFrom"] = myFunctions.getDateVAL(Start);
                            row["D_DateTo"] = myFunctions.getDateVAL(End);
                            row["N_InstAmount"] = nInstAmount;
                            dt.Rows.Add(row);   
                            Start = Start.AddMonths(1);
                        }
                        
                        int N_LoanTransDeatilsID = dLayer.SaveData("Pay_LoanIssueDetails", "N_LoanTransDetailsID", dt, connection, transaction);
                        if (N_LoanTransDeatilsID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(api.Error("Unable to save Loan Request"));
                        }

                         transaction.Commit();
                    }
                    return Ok(api.Success("Loan request saved" + ":" + xLoanID));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }


         [HttpDelete()]
        public ActionResult DeleteData(int nLoanTransID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",api.GetCompanyID(User)},
                                {"X_TransType","EMPLOYEE LOAN"},
                                {"N_VoucherID",nLoanTransID}};
                            Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return Ok( api.Error( "Unable to delete Loan request"));
                    }
                        transaction.Commit();
                        return Ok( api.Success("Loan request Deleted Successfully"));
                   
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }


        }



    }
}