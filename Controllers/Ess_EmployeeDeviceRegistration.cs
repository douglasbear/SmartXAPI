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
    [Route("deviceRegiistration")]
    [ApiController]



    public class Ess_EmployeeDeviceRegistration : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Ess_EmployeeDeviceRegistration(IDataAccessLayer dl, IApiFunctions _api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            api = _api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1336;
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
                    string _sqlQuery = "SELECT Pay_EmpBussinessTripRequest.*, Pay_Employee.X_EmpCode, Pay_Employee.X_EmpName, Pay_Employee.N_EmpID AS Expr1, Gen_Defaults.X_TypeName FROM Pay_EmpBussinessTripRequest LEFT OUTER JOIN Gen_Defaults ON Pay_EmpBussinessTripRequest.N_TravelTypeID = Gen_Defaults.N_TypeId LEFT OUTER JOIN Pay_Employee ON Pay_EmpBussinessTripRequest.N_EmpID = Pay_Employee.N_EmpID AND  Pay_EmpBussinessTripRequest.N_CompanyID = Pay_Employee.N_CompanyID where Pay_EmpBussinessTripRequest.X_RequestCode=@xRequestCode and Pay_EmpBussinessTripRequest.N_CompanyID=@nCompanyID";

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

                var x_DeviceCode = MasterRow["x_DeviceCode"].ToString();
                int nDeviceID = myFunctions.getIntVAL(MasterRow["n_DeviceID"].ToString());
                int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                int nEmpID = myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                int N_NextApproverID = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList EmpParams = new SortedList();
                    EmpParams.Add("@nCompanyID", nCompanyID);
                    EmpParams.Add("@nEmpID", nEmpID);
                    EmpParams.Add("@nFnYearID", nFnYearID);
                    object objEmpName = dLayer.ExecuteScalar("Select X_EmpName From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID  and N_FnYearID=@nFnYearID", EmpParams, connection, transaction);

                    if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()))
                    {
                        int N_PkeyID = nDeviceID;
                        string X_Criteria = "n_DeviceID=" + N_PkeyID + " and N_CompanyID=" + nCompanyID;
                        myFunctions.UpdateApproverEntry(Approvals, "Pay_EmpDeviceIDRegistration", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "Device Registration", N_PkeyID, x_DeviceCode, 1, objEmpName.ToString(), 0, "", User, dLayer, connection, transaction);
                        transaction.Commit();
                        myFunctions.SendApprovalMail(N_NextApproverID, this.FormID, nDeviceID, "Device Registration", x_DeviceCode, dLayer, connection, transaction, User);
                        return Ok(api.Success("Device Registration Approval updated" + "-" + x_DeviceCode));
                    }
                    if (x_DeviceCode == "@Auto")
                    {
                        Params.Add("@nCompanyID", nCompanyID);
                        object objReqCode = dLayer.ExecuteScalar("Select max(isnull(N_DeviceID,0))+1 as N_DeviceID from Pay_EmpDeviceIDRegistration where N_CompanyID=@nCompanyID", Params, connection, transaction);
                        if (objReqCode.ToString() == "" || objReqCode.ToString() == null) { x_DeviceCode = "1"; }
                        else
                        {
                            x_DeviceCode = objReqCode.ToString();
                        }
                        MasterTable.Rows[0]["x_DeviceCode"] = x_DeviceCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Pay_EmpDeviceIDRegistration", "n_DeviceID", nDeviceID, "", connection, transaction);
                    }
                    MasterTable.Columns.Remove("n_FnYearID");
                    MasterTable.AcceptChanges();
                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);
                    nDeviceID = dLayer.SaveData("Pay_EmpDeviceIDRegistration", "n_DeviceID", MasterTable, connection, transaction);
                    if (nDeviceID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "Device Registration", nDeviceID, x_DeviceCode, 1, objEmpName.ToString(), 0, "", User, dLayer, connection, transaction);
                        transaction.Commit();
                        myFunctions.SendApprovalMail(N_NextApproverID, FormID, nDeviceID, "Device Registration", x_DeviceCode, dLayer, connection, transaction, User);
                    }
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("x_DeviceCode", x_DeviceCode.ToString());
                    return Ok(api.Success(res, "Device Registration saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
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
                    int EmpID = myFunctions.getIntVAL(TransRow["N_EmpID"].ToString());
                    SortedList EmpParams = new SortedList();
                    EmpParams.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    EmpParams.Add("@nEmpID", EmpID);
                    EmpParams.Add("@nFnYearID", nFnYearID);
                    object objEmpName = dLayer.ExecuteScalar("Select X_EmpName From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", EmpParams, connection);


                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, this.FormID, nRequestID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID, myFunctions.getIntVAL(TransRow["N_EmpID"].ToString()), 2004, User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), "");
                    SqlTransaction transaction = connection.BeginTransaction(); ;

                    string X_Criteria = "N_RequestID=" + nRequestID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnYearID;
                    string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());

                    string status = myFunctions.UpdateApprovals(Approvals, nFnYearID, "Device Registration", nRequestID, TransRow["X_RequestCode"].ToString(), ProcStatus, "Pay_EmpBussinessTripRequest", X_Criteria, objEmpName.ToString(), User, dLayer, connection, transaction);
                    if (status != "Error")
                    {
                        transaction.Commit();
                        return Ok(api.Success("Device Registration " + status + " Successfully"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to delete Device Registration"));
                    }


                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }



    }
}