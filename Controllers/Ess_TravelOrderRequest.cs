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
    [Route("travelorderrequest")]
    [ApiController]



    public class Ess_TravelOrderRequest : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Ess_TravelOrderRequest(IDataAccessLayer dl, IApiFunctions _api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            api = _api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1235;
        }


        //List

        [HttpGet("list")]
        public ActionResult GetTravelOrderList(string xReqType)
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
                            sqlCommandText = "Select * From vw_Pay_EmpBussinessTripRequestList where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID order by D_RequestDate Desc";
                        else
                        if (xReqType.ToLower() == "pending")
                            sqlCommandText = "select * from vw_Pay_EmpBussinessTripRequestList where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and X_Status not in ('Reject','Approved')  order by D_RequestDate Desc ";
                        else
                            sqlCommandText = "Select * From vw_Pay_EmpBussinessTripRequestList where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and X_Status=@xStatus order by D_RequestDate Desc";

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
        public ActionResult GetTravelOrderDetails(string xRequestCode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);
            QueryParams.Add("@nCompanyID", nCompanyID);
            QueryParams.Add("@xRequestCode", xRequestCode);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string _sqlQuery = "SELECT     Pay_EmpBussinessTripRequest.*,Pay_Employee.X_EmpCode, Pay_Employee.X_EmpName, Pay_Employee.N_EmpID FROM  Pay_EmpBussinessTripRequest LEFT OUTER JOIN Pay_Employee ON Pay_EmpBussinessTripRequest.N_EmpID = Pay_Employee.N_EmpID AND Pay_EmpBussinessTripRequest.N_CompanyID = Pay_Employee.N_CompanyID  where Pay_EmpBussinessTripRequest.X_RequestCode=@xRequestCode and Pay_EmpBussinessTripRequest.N_CompanyID=@nCompanyID";

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


        //Save....
        [HttpPost("save")]
        public ActionResult SaveTORequest([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                SortedList Params = new SortedList();
                DataRow MasterRow = MasterTable.Rows[0];
                DataTable Approvals;
                Approvals = ds.Tables["approval"];
                DataRow ApprovalRow = Approvals.Rows[0];

                var x_RequestCode = MasterRow["x_RequestCode"].ToString();
                int nRequestID = myFunctions.getIntVAL(MasterRow["n_RequestID"].ToString());
                int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyId"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                int nEmpID =    myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());

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
                        myFunctions.UpdateApproverEntry(Approvals, "Pay_EmpBussinessTripRequest", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        myFunctions.LogApprovals(Approvals, nFnYearID, "Travel Order Request", N_PkeyID, x_RequestCode, 1, objEmpName.ToString(), 0, "", User, dLayer, connection, transaction);
                        transaction.Commit();
                        return Ok(api.Success("Travel Order Request Approval updated" + "-" + x_RequestCode));
                    }
                    if (x_RequestCode == "@Auto")
                    {
                        Params.Add("@nCompanyID", nCompanyID);
                        object objReqCode = dLayer.ExecuteScalar("Select max(isnull(N_RequestID,0))+1 as N_RequestID from Pay_EmpBussinessTripRequest where N_CompanyID=@nCompanyID", Params, connection, transaction);
                        if (objReqCode.ToString() == "" || objReqCode.ToString() == null) { x_RequestCode = "1"; }
                        else
                        {
                            x_RequestCode = objReqCode.ToString();
                        }
                        MasterTable.Rows[0]["x_RequestCode"] = x_RequestCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Pay_EmpBussinessTripRequest", "n_RequestID", nRequestID, "", connection, transaction);
                    }
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_RequestType", typeof(int), this.FormID);
                    MasterTable.AcceptChanges();

                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);
                    nRequestID = dLayer.SaveData("Pay_EmpBussinessTripRequest", "n_RequestID", MasterTable, connection, transaction);
                    if (nRequestID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        myFunctions.LogApprovals(Approvals, nFnYearID, "Travel Order Request", nRequestID, x_RequestCode, 1, objEmpName.ToString(), 0, "", User, dLayer, connection, transaction);

                        transaction.Commit();
                    }
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("x_RequestCode", x_RequestCode.ToString());
                    return Ok(api.Success(res, "Travel Order request saved"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }


        [HttpDelete()]
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
                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,isNull(N_EmpID,0) as N_EmpID,X_RequestCode from Pay_EmpBussinessTripRequest where N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID and N_RequestID=@nTransID";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(api.Error("Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, this.FormID, nRequestID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID, myFunctions.getIntVAL(TransRow["N_EmpID"].ToString()), 2004, User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), "");
                    SqlTransaction transaction = connection.BeginTransaction(); ;

                    string X_Criteria = "N_RequestID=" + nRequestID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnYearID;
                                                            string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus=myFunctions.getIntVAL(ButtonTag.ToString());

                    string status = myFunctions.UpdateApprovals(Approvals, nFnYearID, "Travel Order Request", nRequestID, TransRow["X_RequestCode"].ToString(),ProcStatus, "Pay_EmpBussinessTripRequest", X_Criteria, "", User, dLayer, connection, transaction);
                    if (status != "Error" )
                    {
                    transaction.Commit();
                    return Ok(api.Success("Travel Order Request "+status+" Successfully"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to delete Travel Order Request"));
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