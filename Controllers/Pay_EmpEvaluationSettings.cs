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
    [Route("evalSettings")]
    [ApiController]
    public class Pay_EmpEvaluationSettings : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;

        public Pay_EmpEvaluationSettings(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1067;
        }

        [HttpGet("list")]
        public ActionResult GetEmpEvalSettingsList(int nFnYearID, int nPage, bool bAllBranchData, int nBranchID, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";
                    string criteria = "";

                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nFnYearID);
                    Params.Add("@p3", nBranchID);

                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and (X_EvaluationCode like '%" + xSearchkey + "%' or X_Name like '%" + xSearchkey + "%' or x_EmpDep like '%" + xSearchkey + "%' or cast(D_PeriodFrom as VarChar) like '%" + xSearchkey + "%' or cast(D_PeriodTo as VarChar) like '%" + xSearchkey + "%')";

                    if (bAllBranchData == true)
                        {
                            Searchkey = Searchkey + " and N_CompanyID=@p1 and N_FnYearID=@p2 ";
                        }
                    else
                        {
                            Searchkey = Searchkey + " and N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 ";
                        }

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_EvaluationID desc";
                    else
                    {
                        xSortBy = " order by " + xSortBy;
                    }

                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from Pay_EmpEvaluationSettings where " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from Pay_EmpEvaluationSettings where " + Searchkey + " and N_EvaluationID not in (select top(" + Count + ") N_EvaluationID from Pay_EmpEvaluationSettings where " + xSearchkey + xSortBy + " ) " + xSortBy;

                    SortedList OutPut = new SortedList();

                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count from Pay_EmpEvaluationSettings where " + Searchkey + "";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);

                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);

                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                DataTable EmpEvalTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                EmpEvalTable = ds.Tables["evaluators"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nEvaluationID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_EvaluationID"].ToString());
                int  nEvaluationDetailsID = 0;
                int nEvaluatorsDetailsID = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                   
                    // Auto Gen
                    string xEvaluationCode = "";
                    var values = MasterTable.Rows[0]["x_EvaluationCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("N_EvaluationID", nEvaluationID);
                        xEvaluationCode = dLayer.GetAutoNumber("Pay_EmpEvaluationSettings", "X_EvaluationCode", Params, connection, transaction);
                        if (xEvaluationCode == "") { transaction.Rollback(); return Ok(_api.Error(User,"Unable to generate Code")); }
                        MasterTable.Rows[0]["X_EvaluationCode"] = xEvaluationCode;
                    }
                    nEvaluationID = dLayer.SaveData("Pay_EmpEvaluationSettings", "N_EvaluationID", MasterTable, connection, transaction);
                    if (nEvaluationID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    
                    dLayer.DeleteData("Pay_EmpEvaluationSettingsDetails", "N_EvaluationID", nEvaluationID, "", connection, transaction);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        nEvaluationDetailsID = dLayer.SaveData("Pay_EmpEvaluationSettingsDetails", "N_EvaluationDetailsID", DetailTable, connection, transaction);
                    }

                    dLayer.DeleteData("Pay_EmpEvaluators", "N_EvaluationID", nEvaluationID, "", connection, transaction);
                    for (int j = 0; j < EmpEvalTable.Rows.Count; j++)
                    {
                        nEvaluatorsDetailsID = dLayer.SaveData("Pay_EmpEvaluators", "N_EvaluatorsDetailsID", EmpEvalTable, connection, transaction);
                    }

                    transaction.Commit();
                    return Ok(_api.Success("Employee Evaluation Settings Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nEvaluationID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                int nCompanyID = myFunctions.GetCompanyID(User);
                SortedList Params = new SortedList();
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", nFnYearID);
                Params.Add("@p3", nEvaluationID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Pay_EmpEvaluationSettings", "N_EvaluationID", nEvaluationID, "", connection);

                    if (Results > 0)
                    {
                        dLayer.DeleteData("Pay_EmpEvaluationSettingsDetails", "N_EvaluationID", nEvaluationID, "", connection);
                        dLayer.DeleteData("Pay_EmpEvaluators", "N_EvaluationID", nEvaluationID, "", connection);
                        return Ok(_api.Success("Employee Evaluation Settings deleted"));
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











