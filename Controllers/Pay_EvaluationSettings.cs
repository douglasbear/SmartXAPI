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
    [Route("evaluationSettings")]
    [ApiController]
    public class Pay_EvaluationSettings : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Pay_EvaluationSettings(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1457;
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
                int nCompanyID = myFunctions.GetCompanyID(User);
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int N_EvalSettingsID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_EvalSettingsID"].ToString());  
                DateTime d_DateFrom = Convert.ToDateTime(MasterTable.Rows[0]["D_PeriodFrom"].ToString());
                DateTime d_DateTo = Convert.ToDateTime(MasterTable.Rows[0]["D_PeriodTo"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();

                    object objProcessed = dLayer.ExecuteScalar("select COUNT(N_TemplateID) from pay_evaluationsettings where n_companyid="+nCompanyID+" and ((D_PeriodFrom<= convert(VARCHAR ,'"+d_DateFrom+"',23) and D_PeriodTo>=convert(VARCHAR ,'"+d_DateFrom+"',23)) or (D_PeriodFrom<=convert(VARCHAR ,'"+d_DateTo+"',23) and D_PeriodTo>=convert(VARCHAR ,'"+d_DateTo+"',23)) OR (D_PeriodFrom>=convert(VARCHAR ,'"+d_DateFrom+"',23) AND D_PeriodFrom<=convert(VARCHAR ,'"+d_DateTo+"',23) ) OR (D_PeriodTo>=convert(VARCHAR ,'"+d_DateFrom+"',23) AND D_PeriodTo<=convert(VARCHAR ,'"+d_DateTo+"',23) )) and N_TemplateID="+myFunctions.getIntVAL(MasterTable.Rows[0]["N_TemplateID"].ToString())+" and N_EvalSettingsID<>"+N_EvalSettingsID, connection,transaction);
                    if (objProcessed == null) objProcessed = 0;

                    if(myFunctions.getIntVAL(objProcessed.ToString())>0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Evaluation settings of this template already done in this date range!"));
                    }

                    // Auto Gen
                    string xEvalSettingsCode = "";
                    var values = MasterTable.Rows[0]["X_EvalSettingsCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                       
                        while (true)
                        {
                            xEvalSettingsCode = (string)dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction);

                            object N_Result = dLayer.ExecuteScalar("Select 1 from Pay_EvaluationSettings Where X_EvalSettingsCode ='" + xEvalSettingsCode+"'", connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        if (xEvalSettingsCode == "") { transaction.Rollback(); return Ok(_api.Error(User,"Unable to generate Evaluation Settings Code")); }
                        MasterTable.Rows[0]["X_EvalSettingsCode"] = xEvalSettingsCode;
                    }

                    if (N_EvalSettingsID > 0) 
                    {  
                        dLayer.DeleteData("Pay_Evaluators", "N_EvalSettingsID", N_EvalSettingsID, "N_CompanyID =" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Pay_EvaluationSettings", "N_EvalSettingsID", N_EvalSettingsID, "N_CompanyID =" + nCompanyID, connection, transaction);
                    }
                   
                    MasterTable.Columns.Remove("n_FnYearID");
                    MasterTable.AcceptChanges();

                    N_EvalSettingsID = dLayer.SaveData("Pay_EvaluationSettings", "N_EvalSettingsID","","", MasterTable, connection, transaction);
                    if (N_EvalSettingsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        for (int j = 0; j < DetailTable.Rows.Count; j++)
                        {
                            DetailTable.Rows[j]["N_EvalSettingsID"] = N_EvalSettingsID;
                        }
                        int N_EvaluatorID = dLayer.SaveData("Pay_Evaluators", "N_ID","","", DetailTable, connection, transaction);

                        transaction.Commit();
                        return Ok(_api.Success("Evaluation Settings Created"));
                    }                                                   
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(User,ex));
            }
        }

        [HttpGet("details")] 
        public ActionResult GetDetails(string xEvalSettingsCode)
        {
            DataSet ds = new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable dtEvaluator = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nEvalSettingsID=0;
            string sqlCommandText = "select * from Vw_Pay_EvaluationSettings where N_CompanyID=@nCompanyId and x_EvalSettingsCode=@xEvalSettingsCode";
            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@xEvalSettingsCode", xEvalSettingsCode);
           
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = _api.Format(MasterTable, "Master");
                    
                    nEvalSettingsID=myFunctions.getIntVAL(MasterTable.Rows[0]["n_EvalSettingsID"].ToString());
                    SortedList dParamList = new SortedList()
                    {
                        {"CompanyID",nCompanyId},
                        {"EvalSettingsID",nEvalSettingsID}
                    };
                    dtEvaluator = dLayer.ExecuteDataTablePro("SP_Pay_Evaluators", dParamList, connection);
                    dtEvaluator = _api.Format(dtEvaluator, "Evaluators");
                    if (dtEvaluator.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }

                    ds.Tables.Add(MasterTable);
                    ds.Tables.Add(dtEvaluator);

                    return Ok(_api.Success(ds));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }


        [HttpGet("list")]
        public ActionResult GetEvalSettingsList(int? nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {

             try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
            
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";

                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and (X_EvalSettingsCode like '%" + xSearchkey + "%' or X_Title like '%" + xSearchkey + "%' or X_TemplateName like '%" + xSearchkey + "%' or D_PeriodFrom like '%" + xSearchkey + "%' or D_PeriodTo like '%" + xSearchkey + "%')";

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by X_EvalSettingsCode desc";
                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "X_EvalSettingsCode":
                                xSortBy = "X_EvalSettingsCode " + xSortBy.Split(" ")[1];
                                break;
                            case "D_PeriodFrom":
                                xSortBy = "Cast(D_PeriodFrom as DateTime )" + xSortBy.Split(" ")[1];
                                break;
                            case "D_PeriodTo":
                                xSortBy = "Cast(D_PeriodTo as DateTime )" + xSortBy.Split(" ")[1];
                                break;
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_Pay_EvaluationSettings where N_CompanyID=@p1  "  + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_Pay_EvaluationSettings where N_CompanyID=@p1  "  + Searchkey + " and N_EvalSettingsID not in (select top(" + Count + ") N_EvalSettingsID from Vw_Pay_EvaluationSettings where N_CompanyID=@p1 " + xSortBy + " ) " + " " + xSortBy;
                    Params.Add("@p1", nCompanyId);
                    SortedList OutPut = new SortedList();
        
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count  from Vw_Pay_EvaluationSettings where N_CompanyID=@p1 " + Searchkey + "";
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

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nEvalSettingsID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int nCompanyId = myFunctions.GetCompanyID(User);

                    object objProcessed = dLayer.ExecuteScalar("select count(1) from Pay_Appraisal where N_CompanyID="+nCompanyId+" and N_EvalSettingsID="+nEvalSettingsID, connection);
                    if (objProcessed == null) objProcessed = 0;

                    if(myFunctions.getIntVAL(objProcessed.ToString())>0)
                    {
                        return Ok(_api.Error(User, "Already Used! Unable to delete."));
                    }

                    Results = dLayer.DeleteData("Pay_EvaluationSettings", "N_EvalSettingsID", nEvalSettingsID, "N_CompanyID="+nCompanyId, connection);
                     if (Results > 0)
                    {
                        dLayer.DeleteData("Pay_Evaluators", "N_EvalSettingsID", nEvalSettingsID, "N_CompanyID="+nCompanyId, connection);
                        return Ok(_api.Success("Evaluation Setings deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("listemployee")]
        public ActionResult ListEmployee(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);

            string sqlCommandText = "Select N_EmpID,X_EmpCode,X_EmpName,X_Department,X_Position,N_TemplateID,X_TemplateCode,X_TemplateName from vw_PayEmployee Where N_CompanyID=@nCompanyID and N_Status<2 and N_FnyearID=@nFnYearID and N_EmpID in (select ISNULL(N_EmpID,0) from sec_user where N_CompanyID=@nCompanyID)";

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

    }
}