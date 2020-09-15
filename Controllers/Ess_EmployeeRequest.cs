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
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Ess_EmployeeRequest(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
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
                return BadRequest(_api.Error(e));
            }
        }


        [HttpGet("requesttypes")]
        public ActionResult GetEmployeeRequestTypes(int? nCompanyID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID",nCompanyID);
            string sqlCommandText = "select N_RequestTypeID,X_RequestTypeDesc from Pay_EmployeeRequestType where N_CompanyID=@nCompanyID and N_RequestTypeID <> 2004 and N_RequestTypeID<>2003";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText,Params, connection);
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
                return BadRequest(_api.Error(e));
            }
        }

        
 [HttpGet("details")]
        public ActionResult GetEmployeeLoanDetails(int nRequestID, int nEmpID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

           int companyid = _api.GetCompanyID(User);

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
                return BadRequest(_api.Error(e));
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
                int nRequestID = myFunctions.getIntVAL(MasterRow["n_RequestID"].ToString());
                int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                string xReqCode = MasterRow["X_RequestCode"].ToString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlTransaction transaction = connection.BeginTransaction(); ;
                    if (nRequestID >0)
                    {
                        dLayer.DeleteData("Pay_EmpAnyRequest", "N_RequestID", nRequestID, "", connection, transaction);
                    }

                    if(xReqCode == "@Auto"){
                        SortedList Params = new SortedList();
                        Params.Add("@nCompanyID",nCompanyID);
                        string XReqCode = dLayer.ExecuteScalar("Select max(isnull(N_RequestID,0))+1 as N_RequestID from Pay_EmpAnyRequest where N_CompanyID=@nCompanyID",Params,connection,transaction).ToString();
                        MasterTable.Rows[0]["X_RequestCode"] = XReqCode;
                    }

                    MasterTable.Columns.Remove("n_RequestID");
                    MasterTable.AcceptChanges();


                    nRequestID = dLayer.SaveData("Pay_EmpAnyRequest", "N_RequestID", nRequestID, MasterTable, connection, transaction);
                    if (nRequestID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Request successfully created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nRequestID)
        {
            int Results = 0;
            try
            {
                SortedList Params = new SortedList();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Pay_EmpAnyRequest", "N_RequestID", nRequestID,"", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {                    
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("n_RequestID",nRequestID.ToString());
                    return Ok(_api.Success(res,"Request deleted"));
                }
                else
                {
                    return Ok(_api.Error("Unable to delete request"));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(ex));
            }


        }


        }
    }