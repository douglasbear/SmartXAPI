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
    [Route("waiverequest")]
    [ApiController]



    public class Ess_WaiveRequest : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Ess_WaiveRequest(IDataAccessLayer dl, IApiFunctions apiFun, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1232;
        }


        //List
        [HttpGet("list")]
        public ActionResult GetTravelOrderRequest(int? nCompanyID, string xReqType)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int userid = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int companyid = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid).Value);
            string companyname = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.StreetAddress).Value;
            string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@nUserID", userid);
            string sqlCommandText = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object objEmpID = dLayer.ExecuteScalar("Select N_EmpID From Sec_User where N_UserID=@nUserID and N_CompanyID=@nCompanyID", QueryParams, connection);
                    if (objEmpID != null)
                    {
                        QueryParams.Add("@nEmpID", myFunctions.getIntVAL(objEmpID.ToString()));
                        QueryParams.Add("@xStatus", xReqType);
                        if (xReqType.ToLower() == "all")
                            sqlCommandText = "select * from vw_PayLoanApprovals where N_CompanyID=@nCompanyID order by D_LoanPeriodTo Desc";
                        else

                            sqlCommandText = "select * from vw_PayLoanApprovals where N_CompanyID=@nCompanyID and X_Status like '%@xStatus%'  order by D_LoanPeriodTo Desc ";

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
        public ActionResult GetEmployeeLoanDetails(int nRequestID, int nEmpID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

           int companyid = api.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@nRequestID", nRequestID);
            QueryParams.Add("@nEmpID", nEmpID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string _sqlQuery = "SELECT     Pay_AnytimeRequest.N_CompanyID, Pay_AnytimeRequest.N_UserID, Pay_AnytimeRequest.N_BranchID, Pay_AnytimeRequest.N_RequestID,  Pay_AnytimeRequest.N_EmpID, Pay_AnytimeRequest.X_Time, Pay_AnytimeRequest.D_RequestDate, Pay_AnytimeRequest.D_EntryDate, Pay_AnytimeRequest.X_Notes, Pay_AnytimeRequest.N_ApprovalLevelID, Pay_AnytimeRequest.N_ProcStatus, Pay_AnytimeRequest.B_IsSaveDraft,  Pay_AnytimeRequest.D_Date, Pay_AnytimeRequest.X_FileName, Pay_AnytimeRequest.B_IsAttach, Pay_AnytimeRequest.N_RequestType, Pay_AnytimeRequest.N_FnYearID, Pay_AnytimeRequest.X_Comments, Pay_AnytimeRequest.D_Time, Pay_AnytimeRequest.D_Shift1_In, Pay_AnytimeRequest.D_Shift1_Out, Pay_AnytimeRequest.D_Shift2_In, Pay_AnytimeRequest.D_Shift2_Out, Pay_AnytimeRequest.X_RequestCode, Pay_Employee.X_EmpCode, Pay_Employee.X_EmpName, Pay_Employee.N_EmpID AS Expr1 FROM         Pay_AnytimeRequest LEFT OUTER JOIN Pay_Employee ON Pay_AnytimeRequest.N_EmpID = Pay_Employee.N_EmpID AND Pay_AnytimeRequest.N_CompanyID = Pay_Employee.N_CompanyID  where Pay_AnytimeRequest.N_RequestID=@nRequestID and Pay_AnytimeRequest.N_EmpID=@nEmpID and Pay_AnytimeRequest.N_CompanyID=@nCompanyID";
                
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
                var x_RequestCode = MasterRow["x_RequestCode"].ToString();
                int nRequestID = myFunctions.getIntVAL(MasterRow["n_RequestID"].ToString());
                int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyId"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                int nEmpID = myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());
                int nBranchID = myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction(); ;
                    if (x_RequestCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("N_BranchID", nBranchID);

                        x_RequestCode = dLayer.GetAutoNumber("Pay_AnytimeRequest", "x_RequestCode", Params, connection, transaction);
                        if (x_RequestCode == "") { return Ok(api.Error("Unable to generate Waive Request Number")); }
                            MasterTable.Rows[0]["x_RequestCode"] = x_RequestCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Pay_AnytimeRequest", "n_RequestID", nRequestID, "", connection, transaction);
                    }
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable,"N_RequestType",typeof(int),this.FormID);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable,"N_UserID",typeof(int),api.GetUserID(User));
                    MasterTable.Columns.Remove("n_RequestID");
                    MasterTable.AcceptChanges();


                    nRequestID = dLayer.SaveData("Pay_AnytimeRequest", "n_RequestID", nRequestID, MasterTable, connection, transaction);
                    if (nRequestID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                         transaction.Commit();
                    }
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("x_RequestCode",x_RequestCode.ToString());
                    return Ok(api.Success(res,"Waive Request saved"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }


          [HttpDelete()]
        public ActionResult DeleteData(int nRequestID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Pay_AnytimeRequest", "n_RequestID", nRequestID, "", connection, transaction);
                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return Ok( api.Error( "Unable to delete Waive Request"));
                    }
                        transaction.Commit();
                         Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("n_RequestID",nRequestID.ToString());
                    return Ok(api.Success(res,"Waive Request Deleted Successfully"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }



    }
}