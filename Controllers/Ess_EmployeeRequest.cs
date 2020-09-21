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
    [Route("employeerequest")]
    [ApiController]



    public class Ess_EmployeeRequest : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Ess_EmployeeRequest(IDataAccessLayer dl, IApiFunctions _api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            api = _api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1234;
        }


        //List
        [HttpGet("list")]
        public ActionResult GetEmployeeRequest(int? nCompanyID, string xReqType)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_EmployeeRequestApprovalDashBoard";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, connection);
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


        [HttpGet("requesttypes")]
        public ActionResult GetEmployeeRequestTypes(int? nCompanyID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "select N_RequestTypeID,X_RequestTypeDesc from Pay_EmployeeRequestType where N_CompanyID=@nCompanyID and N_RequestTypeID <> 2004 and N_RequestTypeID<>2003";
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
                return BadRequest(api.Error(e));
            }
        }


        [HttpGet("details")]
        public ActionResult GetEmployeeLoanDetails(int nRequestID, int nEmpID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@nRequestID", nRequestID);
            QueryParams.Add("@nEmpID", nEmpID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string _sqlQuery = "SELECT     Pay_EmpAnyRequest.N_CompanyID, Pay_EmpAnyRequest.N_BranchID, Pay_EmpAnyRequest.N_FnYearID, Pay_EmpAnyRequest.N_RequestID, Pay_EmpAnyRequest.N_RequestType, Pay_EmpAnyRequest.N_EmpID, Pay_EmpAnyRequest.D_RequestDate, Pay_EmpAnyRequest.D_EntryDate, Pay_EmpAnyRequest.D_DateFrom, Pay_EmpAnyRequest.D_DateTo, Pay_EmpAnyRequest.X_Notes, Pay_EmpAnyRequest.N_ApprovalLevelID, Pay_EmpAnyRequest.N_ProcStatus, Pay_EmpAnyRequest.B_IsSaveDraft, Pay_EmpAnyRequest.N_UserID, Pay_EmpAnyRequest.X_FileName, Pay_EmpAnyRequest.B_IsAttach, Pay_EmpAnyRequest.X_Comments, Pay_EmpAnyRequest.N_RequestStatus, Pay_EmpAnyRequest.N_EntryUserID, Pay_EmpAnyRequest.X_RequestCode, Pay_Employee.X_EmpName, Pay_Employee.X_EmpCode, Pay_EmployeeRequestType.X_RequestTypeDesc FROM         Pay_EmpAnyRequest INNER JOIN Pay_EmployeeRequestType ON Pay_EmpAnyRequest.N_CompanyID = Pay_EmployeeRequestType.N_CompanyID AND  Pay_EmpAnyRequest.N_RequestType = Pay_EmployeeRequestType.N_RequestTypeID LEFT OUTER JOIN Pay_Employee ON Pay_EmpAnyRequest.N_CompanyID = Pay_Employee.N_CompanyID AND Pay_EmpAnyRequest.N_EmpID = Pay_Employee.N_EmpID  where Pay_EmpAnyRequest.N_RequestID=@nRequestID and Pay_EmpAnyRequest.N_EmpID=@nEmpID and Pay_EmpAnyRequest.N_CompanyID=@nCompanyID";

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
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                DataRow MasterRow = MasterTable.Rows[0];
                DataTable Approvals;
                Approvals = ds.Tables["approval"];
                DataRow ApprovalRow = Approvals.Rows[0];

                int nRequestID = myFunctions.getIntVAL(MasterRow["n_RequestID"].ToString());
                int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                string xReqCode = MasterRow["X_RequestCode"].ToString();
                int nEmpID = myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());



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
                        int N_PkeyID = nRequestID;
                        string X_Criteria = "N_RequestID=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                        myFunctions.UpdateApproverEntry(Approvals, "Pay_EmpAnyRequest", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        myFunctions.LogApprovals(Approvals, nFnYearID, "Employee Request", N_PkeyID, xReqCode, 1, objEmpName.ToString(), 0, "", User, dLayer, connection, transaction);
                        transaction.Commit();
                        return Ok(api.Success("Employee Request Approval updated" + "-" + xReqCode));
                    }

                    if (xReqCode == "@Auto")
                    {
                        SortedList Params = new SortedList();
                        Params.Add("@nCompanyID", nCompanyID);
                        string XReqCode = dLayer.ExecuteScalar("Select max(isnull(N_RequestID,0))+1 as N_RequestID from Pay_EmpAnyRequest where N_CompanyID=@nCompanyID", Params, connection, transaction).ToString();
                        MasterTable.Rows[0]["X_RequestCode"] = XReqCode;
                    }


                    if (nRequestID > 0)
                    {
                        dLayer.DeleteData("Pay_EmpAnyRequest", "N_RequestID", nRequestID, "", connection, transaction);
                    }

                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);

                    nRequestID = dLayer.SaveData("Pay_EmpAnyRequest", "N_RequestID", MasterTable, connection, transaction);
                    if (nRequestID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        myFunctions.LogApprovals(Approvals, nFnYearID, "Employee Request", nRequestID, xReqCode, 1, objEmpName.ToString(), 0, "", User, dLayer, connection, transaction);
                        transaction.Commit();
                        Dictionary<string, string> res = new Dictionary<string, string>();
                        res.Add("x_RequestCode", xReqCode.ToString());
                        return Ok(api.Success(res, "Request successfully created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nRequestID, int nFnYearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nTransID", nRequestID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    ParamList.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,isNull(N_EmpID,0) as N_EmpID,X_RequestCode from Pay_EmpAnyRequest where N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID and N_RequestID=@nTransID";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(api.Error("Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, this.FormID, nRequestID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID, myFunctions.getIntVAL(TransRow["N_EmpID"].ToString()), 2001, User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), "Auto Generated Comment");
                    SqlTransaction transaction = connection.BeginTransaction(); ;

                    string X_Criteria = "N_RequestID=" + nRequestID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnYearID;
                    if (myFunctions.UpdateApprovals(Approvals, nFnYearID, "Employee Request", nRequestID, TransRow["X_RequestCode"].ToString(), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), "Pay_EmpAnyRequest", X_Criteria, "", User, dLayer, connection, transaction))
                    {
                        //Delete Attachement
                        transaction.Commit();
                        return Ok(api.Success("Employee Request Deleted Successfully"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to delete Employee Request"));
                    }


                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }
    }
}