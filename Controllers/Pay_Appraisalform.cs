using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("appraisal")]
    [ApiController]
    public class Pay_Appraisalform : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1454;
        public Pay_Appraisalform(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetAppraisalTemplateList()
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
 
            string sqlCommandText = "select * from Vw_Pay_Appraisal where N_CompanyID=@p1 order by N_AppraisalID";
            Params.Add("@p1", nCompanyID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok( _api.Error(User,e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetAppraisalTemplateDetails(int nAppraisallD)
        {
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            DataTable CompetencyTable = new DataTable();
            DataTable TrainingneedsTable = new DataTable();

            string Mastersql="Select * from vw_PayAppraisalTemplate Where N_CompanyID=@p1 and N_AppraisalID=@p3 ";
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nAppraisallD);
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable=dLayer.ExecuteDataTable(Mastersql,Params,connection); 

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Data Found !!"));
                    }

                    MasterTable = _api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                    int N_AppraisalID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AppraisalID"].ToString());

                    string DetailSql = "select * from Vw_Pay_Appraisal where N_CompanyID=" + nCompanyID + " and N_AppraisalID=" + N_AppraisalID ;

                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);

                    string CompetencySql = "select * from Pay_AppraisalCompetency where N_CompanyID=" + nCompanyID + " and N_AppraisalID=" + N_AppraisalID ;

                    CompetencyTable = dLayer.ExecuteDataTable(CompetencySql, Params, connection);
                    CompetencyTable = _api.Format(CompetencyTable, "Competency");
                    dt.Tables.Add(CompetencyTable);

                    string TrainingneedsSql = "select * from Pay_AppraisalTrainingNeeds where N_CompanyID=" + nCompanyID + " and N_AppraisalID=" + N_AppraisalID ;

                    TrainingneedsTable = dLayer.ExecuteDataTable(TrainingneedsSql, Params, connection);
                    TrainingneedsTable = _api.Format(TrainingneedsTable, "Trainingneeds");
                    dt.Tables.Add(TrainingneedsTable);
                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                DataTable CompetencyTable;
                DataTable TrainingneedsTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                CompetencyTable = ds.Tables["competency"];
                TrainingneedsTable = ds.Tables["trainingneeds"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nAppraisalID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AppraisalID"].ToString());
                int nCategoryID = 0;
                int nCompetencyID = 0;
                int nTrainingID = 0;

                

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_AppraisalCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("N_AppraisalID", nAppraisalID);
                        Code = dLayer.GetAutoNumber("Pay_Appraisal", "X_AppraisalCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback(); return Ok(_api.Error(User,"Unable to generate Grade Code")); }
                        MasterTable.Rows[0]["X_AppraisalCode"] = Code;
                    }
                    nAppraisalID = dLayer.SaveData("Pay_Appraisal", "N_AppraisalID", MasterTable, connection, transaction);
                    if (nAppraisalID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    
                    dLayer.DeleteData("Pay_CompetencyCategory", "N_AppraisalID", nAppraisalID, "", connection, transaction);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_AppraisalID"] = nAppraisalID;  
                    }
                    nCategoryID = dLayer.SaveData("Pay_CompetencyCategory", "N_AppraisalID", DetailTable, connection, transaction);
                    if (nCategoryID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }

                    dLayer.DeleteData("Pay_AppraisalCompetency", "N_AppraisalID", nAppraisalID, "", connection, transaction);
                    for (int j = 0; j < CompetencyTable.Rows.Count; j++)
                    {
                        CompetencyTable.Rows[j]["N_AppraisalID"] = nAppraisalID;  
                    }
                    nCompetencyID = dLayer.SaveData("Pay_AppraisalCompetency", "N_AppraisalID", CompetencyTable, connection, transaction);
                    if (nCompetencyID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }

                    dLayer.DeleteData("Pay_AppraisalTrainingNeeds", "N_AppraisalID", nAppraisalID, "", connection, transaction);
                    for (int j = 0; j < TrainingneedsTable.Rows.Count; j++)
                    {
                        TrainingneedsTable.Rows[j]["N_AppraisalID"] = nAppraisalID;  
                    }
                    nTrainingID = dLayer.SaveData("Pay_AppraisalTrainingNeeds", "N_TrainingID", TrainingneedsTable, connection, transaction);
                    if (nTrainingID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }

                    transaction.Commit();
                    return Ok(_api.Success("Appraisal Template Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nAppraisalID, int nCompanyID)
        {
            int Results = 0;
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nAppraisalID", nAppraisalID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Pay_Appraisal", "N_AppraisalID", nAppraisalID, "", connection);

                    if (Results > 0)
                    {
                        dLayer.DeleteData("Pay_AppraisalCompetencyCategory", "N_AppraisalID", nAppraisalID, "", connection);
                        dLayer.DeleteData("Pay_AppraisalCompetency", "N_AppraisalID", nAppraisalID, "", connection);
                        dLayer.DeleteData("Pay_AppraisalTrainingNeeds", "N_AppraisalID", nAppraisalID, "", connection);
                        return Ok(_api.Success("Appraisal deleted"));
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