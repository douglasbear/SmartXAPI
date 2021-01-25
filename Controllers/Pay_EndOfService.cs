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
    [Route("endofservice")]
    [ApiController]
    public class Pay_EndOfService : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 455;

        public Pay_EndOfService(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("list")]
        public ActionResult GetEndOfService()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            string sqlCommandText="Select X_ServiceEndCode,X_EmpCode,X_EmpName,X_EndType from vw_EndOfService Where N_CompanyID=@nCompanyID";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
        [HttpGet("listemployee")]
        public ActionResult ListEmployee(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nFnYearID",nFnYearID);
            string sqlCommandText="Select N_EmpID,X_EmpCode,X_EmpName from vw_PayEmployee Where N_CompanyID=@nCompanyID and N_Status<2 and N_FnyearID=@nFnYearID";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
        [HttpGet("listEndType")]
        public ActionResult ListEndType()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            string sqlCommandText="Select N_EndTypeID,X_EndType from Pay_ServiceEndType";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
        [HttpGet("listReason")]
        public ActionResult ListReason()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText="Select N_PkeyId,X_PkeyCode,X_Name from Gen_LookupTable where N_ReferId = 455 order by N_ReferId asc";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
        public ActionResult EndOfServiceDetails(int nLoanID,int nFnYearId,bool bAllBranchData,int nBranchID)
        {
            DataTable dtAdjustment = new DataTable();
            DataTable dtLoan = new DataTable();
            DataSet DS=new DataSet();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string xCondition="";
            string sqlAdjustment="";
            string sqlLoan="";

            if(nFnYearId==0)
            {
                if(bAllBranchData==true)    
                    xCondition="(dbo.Pay_LoanIssue.N_CompanyID = @p1)  AND (dbo.Pay_LoanIssue.n_LoanID =@p3) AND (dbo.Pay_LoanIssue.N_FnYearID =@p2) and dbo.Pay_LoanIssue.N_LoanStatus=0";
                else
                    xCondition="(dbo.Pay_LoanIssue.N_CompanyID = @p1)  AND (dbo.Pay_LoanIssue.n_LoanID =@p3) AND (dbo.Pay_LoanIssue.N_FnYearID =@p2) AND (dbo.Pay_LoanIssue.N_BranchID = @p4) and dbo.Pay_LoanIssue.N_LoanStatus=0";

                sqlAdjustment="SELECT     dbo.Pay_LoanIssue.*, dbo.Pay_Employee.X_EmpCode, dbo.Pay_Employee.X_EmpName, dbo.Pay_PayMaster.X_Description ,Acc_MastLedger.X_LedgerCode FROM         dbo.Pay_LoanIssue INNER JOIN dbo.Pay_Employee ON dbo.Pay_LoanIssue.N_EmpID = dbo.Pay_Employee.N_EmpID AND dbo.Pay_LoanIssue.N_CompanyID = dbo.Pay_Employee.N_CompanyID AND dbo.Pay_LoanIssue.N_FnYearID=dbo.Pay_Employee.N_FnYearID INNER JOIN dbo.Pay_PayMaster ON dbo.Pay_LoanIssue.N_PayID = dbo.Pay_PayMaster.N_PayID AND dbo.Pay_LoanIssue.N_CompanyID = dbo.Pay_PayMaster.N_CompanyID LEFT Outer Join Acc_MastLedger On Pay_LoanIssue.N_DefLedgerID = Acc_MastLedger.N_LedgerID AND dbo.Pay_LoanIssue.N_FnYearID=Acc_MastLedger.N_FnYearID AND dbo.Pay_LoanIssue.N_CompanyID=Acc_MastLedger.N_CompanyID  WHERE " + xCondition+"";
            }
            else
            {
                if(bAllBranchData==true) 
                    xCondition="(dbo.Pay_LoanIssue.N_CompanyID = @p1)  AND (dbo.Pay_LoanIssue.n_LoanID =@p3) AND (dbo.Pay_LoanIssue.N_FnYearID =@p2) and dbo.Pay_LoanIssue.N_LoanStatus=0";
                else
                    xCondition="(dbo.Pay_LoanIssue.N_CompanyID = @p1)  AND (dbo.Pay_LoanIssue.n_LoanID =@p3) AND (dbo.Pay_LoanIssue.N_FnYearID =@p2) AND (dbo.Pay_LoanIssue.N_BranchID = @p4) and dbo.Pay_LoanIssue.N_LoanStatus=0";

                sqlAdjustment="SELECT     dbo.Pay_LoanIssue.*, dbo.Pay_Employee.X_EmpCode, dbo.Pay_Employee.X_EmpName, dbo.Pay_PayMaster.X_Description ,Acc_MastLedger.X_LedgerCode FROM         dbo.Pay_LoanIssue INNER JOIN dbo.Pay_Employee ON dbo.Pay_LoanIssue.N_EmpID = dbo.Pay_Employee.N_EmpID AND dbo.Pay_LoanIssue.N_CompanyID = dbo.Pay_Employee.N_CompanyID AND dbo.Pay_LoanIssue.N_FnYearID=dbo.Pay_Employee.N_FnYearID INNER JOIN dbo.Pay_PayMaster ON dbo.Pay_LoanIssue.N_PayID = dbo.Pay_PayMaster.N_PayID AND dbo.Pay_LoanIssue.N_CompanyID = dbo.Pay_PayMaster.N_CompanyID LEFT Outer Join Acc_MastLedger On Pay_LoanIssue.N_DefLedgerID = Acc_MastLedger.N_LedgerID AND dbo.Pay_LoanIssue.N_FnYearID=Acc_MastLedger.N_FnYearID AND dbo.Pay_LoanIssue.N_CompanyID=Acc_MastLedger.N_CompanyID  WHERE " + xCondition+"";

            }
            
            
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", nLoanID);
            Params.Add("@p4", nBranchID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dtAdjustment = dLayer.ExecuteDataTable(sqlAdjustment, Params,connection);
                    int nLoanTransID=0;
                    Params.Add("@p5", nLoanTransID);
                    nLoanTransID=myFunctions.getIntVAL(dtAdjustment.Rows[0]["N_LoanTransID"].ToString());
                    sqlLoan="SELECT  ROW_NUMBER() over(ORDER BY  N_LoanTransDetailsID) as SlNo,Pay_LoanIssueDetails.N_LoanTransDetailsID,Pay_LoanIssueDetails.N_InstAmount,Pay_LoanIssueDetails.N_InstActualAmt,Pay_LoanIssueDetails.N_RefundAmount,Pay_LoanIssueDetails.D_DateFrom,Pay_LoanIssueDetails.D_DateTo,CONVERT(VARCHAR(3),Pay_LoanIssueDetails.D_DateFrom,100)+' - '+ CAST(datepart(year,Pay_LoanIssueDetails.D_DateFrom)As varchar) As X_Month FROm Pay_LoanIssueDetails Where Pay_LoanIssueDetails.N_LoanTransID=@p5 and  Pay_LoanIssueDetails.N_CompanyID = @p1 and N_LoanTransDetailsID not in (Select N_LoanTransDetailsID From Pay_LoanIssueDetails Where N_LoanTransID=@p5 and(N_RefundAmount=0 OR N_RefundAmount IS NULL) and N_TransDetailsID IS NOT NULL and B_IsLoanClose=1)";
                    dtLoan = dLayer.ExecuteDataTable(sqlLoan, Params,connection);

                }
                dtAdjustment = api.Format(dtAdjustment);
                dtLoan = api.Format(dtLoan);
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
                return Ok(api.Error(e));
            }
        }



         [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                int N_SaveDraft=0;
                int N_Status=0;
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nServiceEndID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ServiceEndID"].ToString());
                int nEmpID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_EmpID"].ToString());
                var dEndDate=MasterTable.Rows[0]["D_EndDate"].ToString();
                string xMethod=MasterTable.Rows[0]["X_Method"].ToString();
                int  nEOSDetailID=0;
                

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    SortedList QueryParams = new SortedList();
                   
                    // Auto Gen
                    string ServiceEndCode = "";
                    var values = MasterTable.Rows[0]["X_ServiceEndCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("N_ServiceEndID", nServiceEndID);
                        ServiceEndCode = dLayer.GetAutoNumber("pay_EndOFService", "X_ServiceEndCode", Params, connection, transaction);
                        if (ServiceEndCode == "") { return Ok(api.Error("Unable to generate Service End Code")); }
                        MasterTable.Rows[0]["X_ServiceEndCode"] = ServiceEndCode;
                    }
                    MasterTable.Columns.Remove("X_Method");
                    nServiceEndID = dLayer.SaveData("pay_EndOFService", "N_ServiceEndID", MasterTable, connection, transaction);
                    if (nServiceEndID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    QueryParams.Add("@nCompanyID", nCompanyID);
                    QueryParams.Add("@nFnYearID", nFnYearId);
                    QueryParams.Add("@N_ServiceEndID", nServiceEndID);
                    QueryParams.Add("@N_EmpID", nEmpID);
                    QueryParams.Add("@X_Method", xMethod);
                    object Savedraft = dLayer.ExecuteScalar("select CAST(B_IsSaveDraft as INT) from pay_EndOFService where N_CompanyID=@nCompanyID and N_ServiceEndID=@N_ServiceEndID", QueryParams, connection, transaction);
                    if(Savedraft!=null)
                        N_SaveDraft=myFunctions.getIntVAL(Savedraft.ToString());
                    object Status = dLayer.ExecuteScalar("select N_Status  from Pay_EmployeeStatus where X_Description=@X_Method", QueryParams, connection, transaction);
                    if(Status!=null)
                        N_Status=myFunctions.getIntVAL(Status.ToString());

                    object PositionID = dLayer.ExecuteScalar("select N_PositionID from vw_PayEmployee where N_CompanyID=@nCompanyID and N_EMPID=@N_EmpID", QueryParams, connection, transaction); 

                    if (N_SaveDraft == 0) 
                    {
                        dLayer.ExecuteNonQuery("Update Pay_Employee Set N_Status = " + N_Status + ",D_StatusDate='" + dEndDate.ToString() + "' Where N_CompanyID =" + nCompanyID + " And N_EmpID =" + nEmpID.ToString(), QueryParams, connection, transaction);
                        dLayer.ExecuteNonQuery("Update Pay_SuperVisor Set N_EmpID = 0  Where N_CompanyID =" + nCompanyID + " And N_PositionID =" + PositionID.ToString(), QueryParams, connection, transaction);
                    }
                    dLayer.DeleteData("pay_EndOfServiceSDetails", "N_ServiceEndID", nServiceEndID, "", connection, transaction);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                          nEOSDetailID = dLayer.SaveData("pay_EndOfServiceSDetails", "N_EOSDetailID", DetailTable, connection, transaction);
                    }
                    transaction.Commit();
                    return Ok(api.Success("Terminated"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nServiceEndID, int nEmpID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nEmpID", nEmpID);
                QueryParams.Add("@nServiceEndID", nServiceEndID);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                     dLayer.ExecuteNonQuery("Update Pay_LoanIssueDetails Set n_RefundAmount =0  Where N_CompanyID = @nCompanyID and N_PayrunID = 0", QueryParams, connection, transaction);
                     dLayer.ExecuteNonQuery("Update Pay_Employee Set N_Status = 0,D_StatusDate = null Where N_CompanyID =@nCompanyID And N_EmpID =@nEmpID", QueryParams, connection, transaction);
                     Results = dLayer.DeleteData("pay_EndOfServiceSDetails", "N_ServiceEndID", nServiceEndID, "", connection, transaction);
                     transaction.Commit();
                }
                if (Results > 0)
                {                    
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_ServiceEndID",nServiceEndID.ToString());
                    return Ok(api.Success(res,"EOS deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to EoS"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }


        }

        [HttpGet("listEmployeeDetails")]
        public ActionResult ListEmployeeDetails(int nFnYearID,bool bAllBranchData,int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string X_Crieteria = "";
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@bAllBranchData", bAllBranchData);
            Params.Add("@nBranchID", nBranchID);
            if (bAllBranchData == true)
                    X_Crieteria = "N_CompanyID=@nCompanyID and N_Status<2 and N_FnyearID =@nFnYearID";
                else
                    X_Crieteria = "N_CompanyID=@nCompanyID and N_Status<2 and N_FnyearID =@nFnYearID and N_BranchID=@nBranchID";

            string sqlCommandText="select X_EmpCode,X_EmpName,N_CompanyID,N_EmpID,X_Position,X_Department,D_HireDate,N_Status,N_FnyearID,N_BranchID from vw_PayEmployee where "+X_Crieteria;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
    }

    
    

}