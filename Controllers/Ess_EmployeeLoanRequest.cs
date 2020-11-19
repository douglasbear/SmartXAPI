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

            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);
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
                return Ok(api.Error(e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetEmployeeLoanDetails(int nLoanID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@nLoanID", nLoanID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string _sqlQuery = "SELECT Pay_LoanIssue.*, Pay_Employee.X_EmpCode, Pay_Employee.X_EmpName, Pay_Position.X_Position,Pay_Employee.X_EmpNameLocale FROM  Pay_Position RIGHT OUTER JOIN Pay_Employee ON Pay_Position.N_PositionID = Pay_Employee.N_PositionID AND Pay_Position.N_CompanyID = Pay_Employee.N_CompanyID RIGHT OUTER JOIN Pay_LoanIssue ON Pay_Employee.N_EmpID = Pay_LoanIssue.N_EmpID AND Pay_Employee.N_CompanyID = Pay_LoanIssue.N_CompanyID where Pay_LoanIssue.N_LoanID=@nLoanID and Pay_LoanIssue.N_CompanyID=@nCompanyID";

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
                return Ok(api.Error(e));
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

                int nUserID = myFunctions.GetUserID(User);


                string xLoanID = MasterRow["n_LoanID"].ToString();
                int nLoanTransID = myFunctions.getIntVAL(MasterRow["n_LoanTransID"].ToString());
                int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyId"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                int nEmpID = myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());
                var dDateFrom = MasterRow["d_LoanPeriodFrom"].ToString();
                var dLoanPeriodTo = MasterRow["d_LoanPeriodTo"].ToString();
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
                    object objEmpName = dLayer.ExecuteScalar("Select X_EmpName From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID", EmpParams, connection, transaction);

                    if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()))
                    {
                        int N_PkeyID = nLoanTransID;
                        string X_Criteria = "N_LoanTransID=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                        myFunctions.UpdateApproverEntry(Approvals, "Pay_LoanIssue", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        myFunctions.LogApprovals(Approvals, nFnYearID, this.xTransType, N_PkeyID, xLoanID, 1,objEmpName.ToString(), 0, "",User, dLayer, connection, transaction);
                        transaction.Commit();
                        return Ok(api.Success("Loan Request Approved " + "-" + xLoanID));
                    }


                    if (xLoanID == "@Auto")
                    {
                        if(!EligibleForLoan(dDateFrom,QueryParams,connection,transaction)){
                           return Ok(api.Warning("Not Eligible For Loan!"));  
                        }
                        // if(LoanCountLimitExceed(QueryParams,connection,transaction)){
                        //    return Ok(api.Warning("Loan Limit Exceeded!"));  
                        // }
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        xLoanID = dLayer.GetAutoNumber("Pay_LoanIssue", "n_LoanID", Params, connection, transaction);
                        if (xLoanID == "") { return Ok(api.Error("Unable to generate Loan ID")); }
                        MasterTable.Rows[0]["n_LoanID"] = xLoanID;
                    }
                    else
                    {
                        dLayer.DeleteData("Pay_LoanIssueDetails", "n_LoanTransID", nLoanTransID, "", connection, transaction);
                        dLayer.DeleteData("Pay_LoanIssue", "n_LoanTransID", nLoanTransID, "", connection, transaction);
                    }

                    decimal nInstAmount = myFunctions.getDecimalVAL(MasterTable.Rows[0]["n_InstallmentAmount"].ToString());
                    int nInstNos = myFunctions.getIntVAL(MasterTable.Rows[0]["n_Installments"].ToString());
                    MasterTable.Columns.Remove("n_InstallmentAmount");
                    MasterTable.AcceptChanges();

                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);
                    nLoanTransID = dLayer.SaveData("Pay_LoanIssue", "n_LoanTransID", MasterTable, connection, transaction);
                    if (nLoanTransID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save Loan Request"));
                    }
                    else
                    {


                        myFunctions.LogApprovals(Approvals, nFnYearID, this.xTransType, nLoanTransID, xLoanID,1, objEmpName.ToString(), 0, "",User, dLayer, connection, transaction);

                        DataTable dt = new DataTable();
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
                return Ok(api.Error(ex));
            }
        }


        [HttpDelete()]
        public ActionResult DeleteData(int nLoanTransID, int nFnYearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nTransID", nLoanTransID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    ParamList.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,isNull(N_EmpID,0) as N_EmpID,N_loanID from Pay_LoanIssue where N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID and N_LoanTransID=@nTransID";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(api.Error("Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, this.FormID, nLoanTransID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID, myFunctions.getIntVAL(TransRow["N_EmpID"].ToString()), 2001,User, dLayer, connection));
                    Approvals=myFunctions.AddNewColumnToDataTable(Approvals,"comments",typeof(string),"");
                    SqlTransaction transaction = connection.BeginTransaction();;

                    string X_Criteria = "N_LoanTransID=" + nLoanTransID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnYearID;
                    
                    string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus=myFunctions.getIntVAL(ButtonTag.ToString());
                    //myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString())
                    string status = myFunctions.UpdateApprovals(Approvals, nFnYearID, this.xTransType, nLoanTransID,TransRow["N_loanID"].ToString(),ProcStatus,"Pay_LoanIssue",X_Criteria,"",User,dLayer,connection,transaction);
                    if (status != "Error" )
                    {
                        transaction.Commit();
                    return Ok(api.Success("Loan Request "+status+" Successfully"));
                    }else{
                        transaction.Rollback();
                        return Ok(api.Error("Unable to delete Loan request"));
                    }
                    

                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }


        }


        private bool LoanCountLimitExceed(SortedList Params,SqlConnection connection,SqlTransaction transaction)
        {
            int N_EmpLoanCount = 0, N_LoanLimitCount=0;
            object obj=dLayer.ExecuteScalar("SELECT isnull(N_LoanCountLimit,0) From Pay_Employee Where N_CompanyID=@nCompanyID and N_EmpId =@nEmpID",Params,connection,transaction);
            if(obj!=null)
                N_LoanLimitCount = myFunctions.getIntVAL(obj.ToString());
            object EmpLoanCount = dLayer.ExecuteScalar("SELECT isnull(COUNT(N_LoanTransID),0) From Pay_LoanIssue Where N_CompanyID=@nCompanyID and N_EmpId =@nEmpID",Params,connection,transaction);
            if (EmpLoanCount != null)
                N_EmpLoanCount = myFunctions.getIntVAL(EmpLoanCount.ToString());
            if ((N_EmpLoanCount + 1) > N_LoanLimitCount && N_EmpLoanCount!=0)
            {
                return true;
            }
                return false;
        }
        private bool EligibleForLoan(string fromDate,SortedList Params,SqlConnection connection,SqlTransaction transaction)
        {
            object obj = null;
            double N_EmpLoanEligible = 0;
            DateTime D_HireDate = DateTime.Now ;

                obj = dLayer.ExecuteScalar("SELECT isnull(N_LoanEligible,0) From Pay_Employee Where N_CompanyID=@nCompanyID and N_EmpId =@nEmpID and N_FnyearID=@nFnYearID",Params,connection,transaction);
                if (obj != null)
                    N_EmpLoanEligible = myFunctions.getVAL(obj.ToString());
                object EmpHireDate = dLayer.ExecuteScalar("SELECT D_HireDate From Pay_Employee Where N_CompanyID=@nCompanyID and N_EmpId =@nEmpID and N_FnyearID=@nFnYearID",Params,connection,transaction);
                if (EmpHireDate != null)
                    D_HireDate = Convert.ToDateTime(EmpHireDate.ToString());

                TimeSpan TS = Convert.ToDateTime(fromDate.ToString()) - D_HireDate;


                double Years = TS.TotalDays / 365.25;

                if (N_EmpLoanEligible > Years)
                {
                    return false;
                }
            return true;
        }



    }
}