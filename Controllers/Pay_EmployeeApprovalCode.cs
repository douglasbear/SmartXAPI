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
    [Route("payemployeapprovalcode")]
    [ApiController]
    public class Pay_EmployeApprovalCode : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID;


        public Pay_EmployeApprovalCode(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 202;
        }

        [HttpGet("actionlist")]
        public ActionResult ActionList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select N_CompanyID,N_Action,X_ActionDesc from vw_web_ApprovalAction_Disp where N_CompanyID=@nComapnyID";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(_api.Error(User,e));
            }
        }
        [HttpGet("approvalcodelist")]
        public ActionResult ApprovalCodeList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select N_CompanyID,N_ApprovalID,X_ApprovalCode,X_ApprovalDescription from Gen_ApprovalCodes where N_CompanyID=@nComapnyID";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(_api.Error(User,e));
            }
        }
        //Save....
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();

                    int n_ApprovalSettingsID = myFunctions.getIntVAL(MasterRow["N_ApprovalSettingsID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string x_ApprovalSettingsCode = MasterRow["X_ApprovalSettingsCode"].ToString();
                    
                    if (n_ApprovalSettingsID>0)
                    {
                         dLayer.DeleteData("Sec_ApprovalSettings_EmployeeDetails", "N_ApprovalSettingsID", n_ApprovalSettingsID, "", connection,transaction);
                         dLayer.DeleteData("Sec_ApprovalSettings_Employee", "N_ApprovalSettingsID", n_ApprovalSettingsID, "", connection,transaction);

                    }
                    if (x_ApprovalSettingsCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", N_FormID);
                        x_ApprovalSettingsCode = dLayer.GetAutoNumber("Sec_ApprovalSettings_Employee", "X_ApprovalSettingsCode", Params, connection, transaction);
                        if (x_ApprovalSettingsCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate  Approval Code");
                        }
                        MasterTable.Rows[0]["X_ApprovalSettingsCode"] = x_ApprovalSettingsCode;
                    }

                    n_ApprovalSettingsID = dLayer.SaveData("Sec_ApprovalSettings_Employee", "n_ApprovalSettingsID", "", "", MasterTable, connection, transaction);
                    if (n_ApprovalSettingsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save approval code");
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_ApprovalSettingsID"] = n_ApprovalSettingsID;
                    }
                    int n_ApprovalSettingsDetailsID = dLayer.SaveData("Sec_ApprovalSettings_EmployeeDetails", "n_ApprovalSettingsDetailsID", DetailTable, connection, transaction);
                    if (n_ApprovalSettingsDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save approval code");
                    }


                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_ApprovalSettingsID", n_ApprovalSettingsID);
                    Result.Add("x_ApprovalSettingsCode", x_ApprovalSettingsCode);
                    Result.Add("n_ApprovalSettingsDetailsID", n_ApprovalSettingsDetailsID);

                    return Ok(_api.Success(Result, "Approval Code Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("details")]
        public ActionResult PayEmployeApprovalCode(int nApprovalSettingsID)
        {


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable DataTable = new DataTable();

                    string Mastersql = "";
                    string DetailSql = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                     Params.Add("@nApprovalSettingsID", nApprovalSettingsID);
                    Mastersql = "select * from Sec_ApprovalSettings_Employee where N_CompanyId=@nCompanyID and N_ApprovalSettingsID=@nApprovalSettingsID";
                   
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    // int ApproovalSettingsID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ApprovalSettingsID"].ToString());
                    // Params.Add("@nApproovalSettingsID", ApproovalSettingsID);

                    MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from vw_Sec_ApprovalSettings_EmployeeDetails where N_CompanyId=@nCompanyID and N_ApprovalSettingsID=@nApprovalSettingsID";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nApprovalSettingsID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nApprovalSettingsID", nApprovalSettingsID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object apprObj = dLayer.ExecuteScalar("select count(*) From Pay_Employee where N_ApprovalID = @nApprovalSettingsID and N_CompanyID=@nCompanyID",QueryParams, connection);
                    int  N_Count= myFunctions.getIntVAL(apprObj.ToString());
                    if(N_Count > 0)
                    {
                    return Ok( _api.Error(User,"Unable to delete its already in use"));
                    }
                    Results = dLayer.DeleteData("Sec_ApprovalSettings_Employee", "N_ApprovalSettingsID", nApprovalSettingsID, "", connection);


                    if (Results > 0)
                    {
                        dLayer.DeleteData("Sec_ApprovalSettings_EmployeeDetails", "N_ApprovalSettingsID", nApprovalSettingsID, "", connection);
                        return Ok(_api.Success("Approval Code deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete"));
                    }

                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }


        }


    }
}
