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
        public ActionResult GetAppraisalList(int? nCompanyId,int nFnYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_AppraisalCode like '%" + xSearchkey + "%' or X_EmpName like '%" + xSearchkey + "%' or X_Position like '%" + xSearchkey + "%' or X_Department like '%" + xSearchkey + "%' or X_TemplateName like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_AppraisalCode desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_AppraisalCode":
                        xSortBy = "X_AppraisalCode " + xSortBy.Split(" ")[1];
                        break;
                    case "D_DocDate":
                        xSortBy = "Cast(D_DocDate as DateTime )" + xSortBy.Split(" ")[1];
                        break;
                    case "D_PeriodTo":
                        xSortBy = "X_EmpName" + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_Pay_Appraisal where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearID "  + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_Pay_Appraisal where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearID "  + Searchkey + " and N_AppraisalID not in (select top(" + Count + ") N_AppraisalID from Vw_Pay_Appraisal where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearID " + xSortBy + " ) " + " " + xSortBy;

            Params.Add("@nCompanyId", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(*) as N_Count  from Vw_Pay_Appraisal where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearID " + Searchkey + "";
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
        public ActionResult GetAppraisalDetails(string xAppraisalCode)
        {
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable MasterTable = new DataTable();
            DataTable CompetencyCategoryTable = new DataTable();
            DataTable CompetencyTable = new DataTable();
            DataTable TrainingneedsTable = new DataTable();

            string Mastersql="Select * from Vw_Pay_Appraisal Where N_CompanyID=@nCompanyID and x_AppraisalCode=@xAppraisalCode ";
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@xAppraisalCode",xAppraisalCode);
            
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

                    string CompetencycategorySql = "select * from Vw_Pay_Appraisal where N_CompanyID=" + nCompanyID + " and N_AppraisalID=" + N_AppraisalID ;

                    CompetencyCategoryTable = dLayer.ExecuteDataTable(CompetencycategorySql, Params, connection);
                    CompetencyCategoryTable = _api.Format(CompetencyCategoryTable, "Competencycategory");
                    dt.Tables.Add(CompetencyCategoryTable);

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
                DataTable CompetencyCategoryTable;
                DataTable CompetencyCategoryCopyTable;
                DataTable CompetencyTable;
                DataTable TrainingneedsTable;
                MasterTable = ds.Tables["master"];
                CompetencyCategoryTable = ds.Tables["competencycategory"];
                CompetencyCategoryCopyTable = CompetencyCategoryTable.Clone();
                CompetencyTable = ds.Tables["competency"];
                TrainingneedsTable = ds.Tables["trainingneeds"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nAppraisalID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AppraisalID"].ToString());
                int nCategoryID = 0;
                int nCompetencyID = 0;
                int nTrainingID = 0;

                CompetencyCategoryCopyTable.Clear();
                CompetencyCategoryCopyTable.Columns.Add("N_CompanyID");
                CompetencyCategoryCopyTable.Columns.Add("N_AppraisalID");
                CompetencyCategoryCopyTable.Columns.Add("N_CategoryID");
                CompetencyCategoryCopyTable.Columns.Add("X_Category");
                CompetencyCategoryCopyTable.Columns.Add("N_Weightage");
                CompetencyCategoryCopyTable.Columns.Add("N_EntryTypeID");
                CompetencyCategoryCopyTable.Columns.Add("N_GradeTypeID");
                CompetencyCategoryCopyTable.Columns.Add("N_TotalPerc");
                CompetencyCategoryCopyTable.Columns.Add("X_ID");

                int nCount = CompetencyCategoryTable.Rows.Count;
                foreach (DataRow dRow in CompetencyCategoryTable.Rows)
                {
                    DataRow row = CompetencyCategoryCopyTable.NewRow();
                    row["N_CompanyID"] = dRow["N_CompanyID"];
                    row["N_AppraisalID"] = dRow["N_AppraisalID"];
                    row["N_CategoryID"] = dRow["N_CategoryID"];
                    row["X_Category"] = dRow["X_Category"];
                    row["N_Weightage"] = dRow["N_Weightage"];
                    if (CompetencyCategoryTable.Columns.Contains("N_EntryTypeID"))
                        row["N_EntryTypeID"] = dRow["N_EntryTypeID"];
                    if (CompetencyCategoryTable.Columns.Contains("N_GradeTypeID"))
                        row["N_GradeTypeID"] = dRow["N_GradeTypeID"];
                    row["N_TotalPerc"] = dRow["N_TotalPerc"];
                    row["X_ID"] = dRow["X_ID"];
                    CompetencyCategoryCopyTable.Rows.Add(row);
                }
                CompetencyCategoryCopyTable.AcceptChanges();

                if (MasterTable.Columns.Contains("n_FnYearID"))
                    MasterTable.Columns.Remove("n_FnYearID");

                if (CompetencyCategoryTable.Columns.Contains("x_ID"))
                    CompetencyCategoryTable.Columns.Remove("x_ID");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    if (nAppraisalID > 0)
                    {
                        dLayer.DeleteData("Pay_AppraisalTrainingNeeds", "nAppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Pay_AppraisalCompetency", "nAppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Pay_AppraisalCompetencyCategory", "nAppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Pay_Appraisal", "nAppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection, transaction);
                    }

                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_AppraisalCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("nAppraisalID", nAppraisalID);
                        Code = dLayer.GetAutoNumber("Pay_Appraisal", "X_AppraisalCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Grade Code")); }
                        MasterTable.Rows[0]["X_AppraisalCode"] = Code;
                    }
                    nAppraisalID = dLayer.SaveData("Pay_Appraisal", "nAppraisalID", MasterTable, connection, transaction);
                    if (nAppraisalID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                    for (int j = 0; j < CompetencyCategoryTable.Rows.Count; j++)
                    {
                        int p=0;
                        CompetencyCategoryTable.Rows[j]["nAppraisalID"] = nAppraisalID;

                        nCategoryID = dLayer.SaveDataWithIndex("Pay_AppraisalCompetencyCategory", "N_CategoryID", "", "", j, CompetencyCategoryTable, connection, transaction);
                        if (nCategoryID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to save"));
                        }

                        for (int i = 0; i < CompetencyTable.Rows.Count; i++)
                        {
                            if (CompetencyTable.Rows[i]["x_ID"].ToString() == CompetencyCategoryCopyTable.Rows[j]["X_ID"].ToString())
                            {
                                CompetencyTable.Rows[i]["nAppraisalID"] = nAppraisalID;
                                CompetencyTable.Rows[i]["N_CategoryID"] = nCategoryID;
                            }
                        }
                        p=p+1;
                    }
                    if (CompetencyTable.Columns.Contains("x_ID"))
                        CompetencyTable.Columns.Remove("x_ID");
                    nCompetencyID = dLayer.SaveData("Pay_AppraisalCompetency", "N_CompetencyID", CompetencyTable, connection, transaction);
                    if (nCompetencyID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                    for (int j = 0; j < TrainingneedsTable.Rows.Count; j++)
                    {
                        TrainingneedsTable.Rows[j]["nAppraisalID"] = nAppraisalID;
                    }
                    nTrainingID = dLayer.SaveData("Pay_AppraisalTrainingNeeds", "N_TrainingID", TrainingneedsTable, connection, transaction);

                    transaction.Commit();
                    return Ok(_api.Success("Appraisal Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
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
                    Results = dLayer.DeleteData("Pay_Appraisal", "N_AppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection);

                    if (Results > 0)
                    {
                        dLayer.DeleteData("Pay_AppraisalCompetencyCategory", "N_AppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection);
                        dLayer.DeleteData("Pay_AppraisalCompetency", "N_AppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection);
                        dLayer.DeleteData("Pay_AppraisalTrainingNeeds", "N_AppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection);
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

        [HttpGet("listemployee")]
        public ActionResult ListEmployee(int nFnYearID,DateTime dPeriod,int nUserID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@dPeriod", dPeriod);
            Params.Add("@nUserID", nUserID);
            string sqlCommandText = "Select N_EmpID,X_EmpCode,X_EmpName,X_Department,X_Position,N_TemplateID,X_TemplateCode,X_TemplateName from vw_EmployeesOfEvaluators "+
                                    " Where N_CompanyID=@nCompanyID and N_FnyearID=@nFnYearID and N_UserID=@nUserID and D_PeriodFrom>=@dPeriod and D_PeriodTo<=@dPeriod";
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