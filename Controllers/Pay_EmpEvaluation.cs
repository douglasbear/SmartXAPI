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
    [Route("empEvaluation")]
    [ApiController]
    public class Pay_EmpEvaluation : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;

        public Pay_EmpEvaluation(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1068;
        }

        [HttpGet("list")]
        public ActionResult GetEmpEvaluationList(int nFnYearID, int nPage, bool bAllBranchData, int nBranchID, int nSizeperpage, string xSearchkey, string xSortBy)
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

                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nFnYearID);
                    Params.Add("@p3", nBranchID);

                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and (X_EvalCode like '%" + xSearchkey + "%' or X_Description like '%" + xSearchkey + "%')";

                    if (bAllBranchData == true)
                    {
                        Searchkey = Searchkey + " and N_CompanyID=@p1 and N_FnYearID=@p2 ";
                    }
                    else
                    {
                        Searchkey = Searchkey + " and N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 ";
                    }

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_EvalID desc";
                    else
                    {
                        xSortBy = " order by " + xSortBy;
                    }

                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from Pay_EmpEvaluation where " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from Pay_EmpEvaluation where " + Searchkey + " and N_EvalID not in (select top(" + Count + ") N_EvalID from Pay_EmpEvaluation where " + xSearchkey + xSortBy + " ) " + xSortBy;

                    SortedList OutPut = new SortedList();

                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count from Pay_EmpEvaluation where " + Searchkey + "";
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

        [HttpGet("details")]
        public ActionResult GetEmpEvaluationDetails(int nFnYearID, int nEvalID, bool bAllBranchData, int nBranchID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            string Mastersql = "";

            if (bAllBranchData == true)
                Mastersql = "select * from Pay_EmpEvaluation where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EvalID=@p4 ";
            else
                Mastersql = "select * from Pay_EmpEvaluation where N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 and N_EvalID=@p4 ";

            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearID);
            Params.Add("@p3", nBranchID);
            Params.Add("@p4", nEvalID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Data Found !!"));
                    }

                    MasterTable = _api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                    int n_EvalID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_EvalID"].ToString());

                    string DetailSql = "";

                    DetailSql = "Select * from Pay_EmpEvaluationDetails where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EvalID=" + n_EvalID;
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);
                }
                return Ok(_api.Success(dt));
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
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nEvalID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_EvalID"].ToString());
                int nEvalDetailsID = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();

                    // Auto Gen
                    string xEvalCode = "";
                    var values = MasterTable.Rows[0]["x_EvalCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("N_EvalID", nEvalID);
                        xEvalCode = dLayer.GetAutoNumber("Pay_EmpEvaluation", "X_EvalCode", Params, connection, transaction);
                        if (xEvalCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Code")); }
                        MasterTable.Rows[0]["X_EvalCode"] = xEvalCode;
                    }
                    nEvalID = dLayer.SaveData("Pay_EmpEvaluation", "N_EvalID", MasterTable, connection, transaction);
                    if (nEvalID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                    dLayer.DeleteData("Pay_EmpEvaluationDetails", "N_EvalID", nEvalID, "", connection, transaction);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_EvalID"] = nEvalID;
                    }
                    nEvalDetailsID = dLayer.SaveData("Pay_EmpEvaluationDetails", "N_EvalDetailsID", DetailTable, connection, transaction);
                    if (nEvalDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable To Save"));
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Employee Evaluation Settings Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nEvalID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                int nCompanyID = myFunctions.GetCompanyID(User);
                SortedList Params = new SortedList();
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", nFnYearID);
                Params.Add("@p3", nEvalID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Pay_EmpEvaluation", "N_EvaluID", nEvalID, "", connection);

                    if (Results > 0)
                    {
                        dLayer.DeleteData("Pay_EmpEvaluationDetails", "N_EvalID", nEvalID, "", connection);
                        return Ok(_api.Success("Employee Evaluation deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

    }
}











