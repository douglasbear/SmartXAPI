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
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Ess_EmployeeLoanRequest(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1234;
        }


        //List
        [HttpGet("list")]
        public ActionResult GetEmployeeLoanRequest(int? nCompanyID, string xReqType)
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
        [HttpGet("dummy")]
        public ActionResult GetPurchaseInvoiceDummy(int? Id)
        {
            try
            {
                string sqlCommandText = "select * from Pay_LoanIssue where N_LoanTransID=@p1";
                SortedList mParamList = new SortedList() { { "@p1", Id } };
                DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList);
                masterTable = _api.Format(masterTable, "master");

                string sqlCommandText2 = "select * from Pay_LoanIssueDetails where N_LoanTransID=@p1";
                SortedList dParamList = new SortedList() { { "@p1", Id } };
                DataTable detailTable = dLayer.ExecuteDataTable(sqlCommandText2, dParamList);
                detailTable = _api.Format(detailTable, "details");

                if (detailTable.Rows.Count == 0) { return Ok(new { }); }
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(masterTable);
                dataSet.Tables.Add(detailTable);

                return Ok(dataSet);

            }
            catch (Exception e)
            {
                return StatusCode(403, _api.ErrorResponse(e));
            }
        }
        //Save....
        [HttpPost("save")]
        public ActionResult SaveLoanRequest([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();
                // Auto Gen
                DataRow MasterRow = MasterTable.Rows[0];
                string xLoanID = "";

                var nLoanID = MasterRow["n_LoanID"].ToString();
                int nLoanTransID = myFunctions.getIntVAL(MasterRow["N_LoanTransID"].ToString());
                int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyId"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                int nEmpID = myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());

                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nEmpID", nEmpID);
                //QueryParams.Add("@nLoanTransID", nLoanTransID);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // if (myFunctions.getBoolVAL(myFunctions.checkProcessed("Acc_FnYear", "B_YearEndProcess", "N_FnYearID", "@nFnYearID", "N_CompanyID=@nCompanyID ", QueryParams, dLayer, connection)))
                    //     return Ok(_api.Warning("Year is closed, Cannot create new Vendor..."));

                    SqlTransaction transaction = connection.BeginTransaction(); ;
                    if (nLoanID == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        xLoanID = dLayer.GetAutoNumber("Pay_LoanIssue", "n_LoanID", Params, connection, transaction);
                        if (xLoanID == "") { return StatusCode(409, _api.Response(409, "Unable to generate Loan ID")); }
                        MasterTable.Rows[0]["n_LoanID"] = xLoanID;
                    }
                    else
                    {
                        dLayer.DeleteData("Pay_LoanIssue", "n_LoanTransID", nLoanTransID, "", connection, transaction);
                    }

                    MasterTable.Columns.Remove("n_LoanTransID");
                    MasterTable.AcceptChanges();


                    nLoanTransID = dLayer.SaveData("Pay_LoanIssue", "n_LoanTransID", nLoanTransID, MasterTable, connection, transaction);
                    if (nLoanTransID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {


                        for (int j = 0; j < DetailTable.Rows.Count; j++)
                        {
                            DetailTable.Rows[j]["n_LoanTransID"] = nLoanTransID;
                        }
                        int N_LoanTransDeatilsID = dLayer.SaveData("Pay_LoanIssueDetails", "n_LoanTransDeatilsID", 0, DetailTable, connection, transaction);
                        if (N_LoanTransDeatilsID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable to save Loan Request"));
                        }

                         transaction.Commit();
                    }
                    return Ok(_api.Success("Loan request saved" + ":" + xLoanID));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(ex));
            }
        }



    }
}