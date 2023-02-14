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
    [Route("appraisaltemplate")]
    [ApiController]
    public class Pay_AppraisalTemplate : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1444;
        public Pay_AppraisalTemplate(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetAppraisalTemplateList(string xFrom,DateTime dPeriodFrom,DateTime dPeriodTo)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText ="";
            if(xFrom==null)xFrom="";

            if(xFrom=="EvaluationSettings")
            {
                //sqlCommandText= "select * from Pay_AppraisalTemplate where N_CompanyID=@p1 and N_TemplateID not in (select N_TemplateID from pay_evaluationsettings where n_companyid=@p1 and ((D_PeriodFrom<=@dPeriodFrom and D_PeriodTo>=@dPeriodFrom) or (D_PeriodFrom<=@dPeriodTo and D_PeriodTo>=@dPeriodTo))) order by N_TemplateID"; 
                sqlCommandText= "select * from Pay_AppraisalTemplate where N_CompanyID=@p1 and N_TemplateID not in (select N_TemplateID from pay_evaluationsettings where n_companyid=@p1 and ((D_PeriodFrom<= @dPeriodFrom and D_PeriodTo>=@dPeriodFrom) or (D_PeriodFrom<=@dPeriodTo and D_PeriodTo>=@dPeriodTo) OR (D_PeriodFrom>=@dPeriodFrom AND D_PeriodFrom<=@dPeriodTo ) OR (D_PeriodTo>=@dPeriodFrom AND D_PeriodTo<=@dPeriodTo ))) order by N_TemplateID"; 
                Params.Add("@dPeriodFrom", dPeriodFrom);
                Params.Add("@dPeriodTo", dPeriodTo);
            }
            else
                sqlCommandText= "select * from Pay_AppraisalTemplate where N_CompanyID=@p1 order by N_TemplateID"; 

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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("dashboardlist")]
        public ActionResult GetAppraisalTemplateDashboardList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";
            string sqlCondition = "";

                Params.Add("@p1", nCompanyID);

                   if (xSearchkey != null && xSearchkey.Trim() != "")
                      Searchkey = "and (X_Code like'%" + xSearchkey + "%'or X_TemplateName like'%" + xSearchkey + "%' or d_EntryDate like'%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_Code desc";
           else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_Code":
                        xSortBy = "X_Code " + xSortBy.Split(" ")[1];
                        break;
                    case "d_EntryDate":
                xSortBy = "Cast([D_EntryDate] as DateTime ) " + xSortBy.Split(" ")[1];
                        break;
                    case "x_TemplateName":
                        xSortBy = "X_TemplateName " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                 xSortBy = " order by " + xSortBy;
            }
             sqlCondition = " N_CompanyID=@p1 ";
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from Pay_AppraisalTemplate where " + sqlCondition + " " + Searchkey +" "+ xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from Pay_AppraisalTemplate where " + sqlCondition + " " + Searchkey +" "+ " and  N_TemplateID not in (select top(" + Count + ") N_TemplateID from Pay_AppraisalTemplate where " + sqlCondition + " " + Searchkey +" "+xSortBy+")" + Searchkey +" "+ xSortBy;
           
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(1) as N_Count from Pay_AppraisalTemplate where " + sqlCondition + " " + Searchkey + " ";
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
        public ActionResult GetAppraisalTemplateDetails(string xCode)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable MasterTable = new DataTable();
            DataTable CompetencyCategoryTable = new DataTable();
            DataTable CompetencyTable = new DataTable();
            DataTable TrainingneedsTable = new DataTable();
            DataTable GradeTypeTable = new DataTable();

            string Mastersql = "Select * from vw_Pay_AppraisalTemplate Where N_CompanyID=@p1 and X_Code=@p2 ";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", xCode);

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

                    int N_TemplateID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TemplateID"].ToString());

                    string CompetencyCategorySql = "select * from vw_Pay_CompetencyCategory where N_CompanyID=" + nCompanyID + " and N_TemplateID=" + N_TemplateID;

                    CompetencyCategoryTable = dLayer.ExecuteDataTable(CompetencyCategorySql, Params, connection);                  
                    CompetencyCategoryTable = _api.Format(CompetencyCategoryTable, "Competencycategory");
                    dt.Tables.Add(CompetencyCategoryTable);

                    string CompetencySql = "select * from vw_Pay_Competency where N_CompanyID=" + nCompanyID + " and N_TemplateID=" + N_TemplateID;

                    CompetencyTable = dLayer.ExecuteDataTable(CompetencySql, Params, connection);

                    string GradeTypeSql = "select * from Pay_GradeTypeDetails where N_CompanyID="+nCompanyID+" and N_GradeID in (select N_GradeTypeID from Pay_CompetencyCategory where N_CompanyID="+nCompanyID+" and N_TemplateID="+N_TemplateID+" and N_GradeTypeID is not null group by N_GradeTypeID)";

                    GradeTypeTable = dLayer.ExecuteDataTable(GradeTypeSql, Params, connection);
                    GradeTypeTable = _api.Format(GradeTypeTable, "GradeType");
                    dt.Tables.Add(GradeTypeTable);

                    for(int i=0;i<CompetencyTable.Rows.Count;i++)
                    {
                        if(myFunctions.getIntVAL(CompetencyTable.Rows[i]["N_GradeTypeID"].ToString())>0)
                        {
                            for(int j=0;j<GradeTypeTable.Rows.Count;j++)
                            {
                                if((myFunctions.getIntVAL(CompetencyTable.Rows[i]["N_GradeTypeID"].ToString())==myFunctions.getIntVAL(GradeTypeTable.Rows[j]["N_GradeID"].ToString())) && (myFunctions.getVAL(CompetencyTable.Rows[i]["N_ToBeAchieved"].ToString())==myFunctions.getVAL(GradeTypeTable.Rows[j]["N_Weightage"].ToString())))
                                {
                                    CompetencyTable.Rows[i]["X_Name"]=GradeTypeTable.Rows[j]["X_Name"].ToString();
                                    CompetencyTable.Rows[i]["N_ToBeAchievedGradeDetailsID"]=GradeTypeTable.Rows[j]["N_GradeDetailsID"].ToString();
                                }
                            }
                        }
                    }

                    CompetencyTable = _api.Format(CompetencyTable, "Competency");
                    dt.Tables.Add(CompetencyTable);

                    string TrainingneedsSql = "select * from Pay_TrainingNeeds where N_CompanyID=" + nCompanyID + " and N_TemplateID=" + N_TemplateID;

                    TrainingneedsTable = dLayer.ExecuteDataTable(TrainingneedsSql, Params, connection);
                    TrainingneedsTable = _api.Format(TrainingneedsTable, "Trainingneeds");
                    dt.Tables.Add(TrainingneedsTable);
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
                int nTemplateID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_TemplateID"].ToString());
                int nCategoryID = 0;
                int nCompetencyID = 0;
                int nTrainingID = 0;

                CompetencyCategoryCopyTable.Clear();
                CompetencyCategoryCopyTable.Columns.Add("N_CompanyID");
                CompetencyCategoryCopyTable.Columns.Add("N_TemplateID");
                CompetencyCategoryCopyTable.Columns.Add("N_CategoryID");
                CompetencyCategoryCopyTable.Columns.Add("X_Category");
                CompetencyCategoryCopyTable.Columns.Add("N_Weightage");
                CompetencyCategoryCopyTable.Columns.Add("N_EntryTypeID");
                CompetencyCategoryCopyTable.Columns.Add("N_GradeTypeID");
                CompetencyCategoryCopyTable.Columns.Add("X_ID");

                int nCount = CompetencyCategoryTable.Rows.Count;
                
                 foreach(DataRow dRow in CompetencyCategoryTable.Rows)
                {
                    DataRow row = CompetencyCategoryCopyTable.NewRow();
                    row["N_CompanyID"] = dRow["N_CompanyID"];
                    row["N_TemplateID"] = dRow["N_TemplateID"];
                    row["N_CategoryID"] = dRow["N_CategoryID"];
                    row["X_Category"] = dRow["X_Category"];
                    row["N_Weightage"] = dRow["N_Weightage"];
                    if (CompetencyCategoryTable.Columns.Contains("N_EntryTypeID"))
                        row["N_EntryTypeID"] = dRow["N_EntryTypeID"];
                    if (CompetencyCategoryTable.Columns.Contains("N_GradeTypeID"))
                        row["N_GradeTypeID"] = dRow["N_GradeTypeID"];
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
                    if (nTemplateID > 0)
                    {
                        dLayer.DeleteData("Pay_TrainingNeeds", "N_TemplateID", nTemplateID, "N_CompanyID =" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Pay_Competency", "N_TemplateID", nTemplateID, "N_CompanyID =" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Pay_CompetencyCategory", "N_TemplateID", nTemplateID, "N_CompanyID =" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Pay_AppraisalTemplate", "N_TemplateID", nTemplateID, "N_CompanyID =" + nCompanyID, connection, transaction);
                    }

                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["x_Code"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("N_TemplateID", nTemplateID);
                        Code = dLayer.GetAutoNumber("Pay_AppraisalTemplate", "X_Code", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Grade Code")); }
                        MasterTable.Rows[0]["X_Code"] = Code;
                    }

                     string X_TemplateName= MasterTable.Rows[0]["X_TemplateName"].ToString();
                    string DupCriteria = "X_TemplateName='" + X_TemplateName + "' and N_CompanyID=" + nCompanyID;

                    nTemplateID = dLayer.SaveData("Pay_AppraisalTemplate", "N_TemplateID",DupCriteria,"", MasterTable, connection, transaction);
                    if (nTemplateID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                    for (int j = 0; j < CompetencyCategoryTable.Rows.Count; j++)
                    {
                        int p=0;
                        CompetencyCategoryTable.Rows[j]["N_TemplateID"] = nTemplateID;

                        nCategoryID = dLayer.SaveDataWithIndex("Pay_CompetencyCategory", "N_CategoryID", "", "", j, CompetencyCategoryTable, connection, transaction);
                        if (nCategoryID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to save"));
                        }

                        for (int i = 0; i < CompetencyTable.Rows.Count; i++)
                        {
                            if (CompetencyTable.Rows[i]["x_ID"].ToString() == CompetencyCategoryCopyTable.Rows[j]["X_ID"].ToString())
                            {
                                CompetencyTable.Rows[i]["N_TemplateID"] = nTemplateID;
                                CompetencyTable.Rows[i]["N_CategoryID"] = nCategoryID;
                            }
                        }
                        p=p+1;
                    }
                    if (CompetencyTable.Columns.Contains("x_ID"))
                        CompetencyTable.Columns.Remove("x_ID");
                    nCompetencyID = dLayer.SaveData("Pay_Competency", "N_CompetencyID", CompetencyTable, connection, transaction);
                    if (nCompetencyID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                    for (int j = 0; j < TrainingneedsTable.Rows.Count; j++)
                    {
                        TrainingneedsTable.Rows[j]["N_TemplateID"] = nTemplateID;
                    }
                    nTrainingID = dLayer.SaveData("Pay_TrainingNeeds", "N_TrainingID", TrainingneedsTable, connection, transaction);
                    // if (nTrainingID <= 0)
                    // {
                    //     transaction.Rollback();
                    //     return Ok(_api.Error(User, "Unable to save"));
                    // }

                    transaction.Commit();
                    return Ok(_api.Success("Appraisal Template Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nTemplateID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nTemplateID", nTemplateID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                     object tempcount = dLayer.ExecuteScalar("select count(1) from Pay_EvaluationSettings where N_TemplateID=" + nTemplateID + " and N_CompanyID=" + nCompanyID + "", connection);
                     object tempempcount= dLayer.ExecuteScalar("select count(1) from Pay_Employee where N_TemplateID=" + nTemplateID + " and N_CompanyID=" + nCompanyID + "", connection);
                      
                         int ntempcount = myFunctions.getIntVAL(tempcount.ToString());
                         int ntempempcount = myFunctions.getIntVAL(tempempcount.ToString());
                         if(ntempcount==0 && ntempempcount==0) 
                      {
                     Results = dLayer.DeleteData("Pay_AppraisalTemplate", "N_TemplateID", nTemplateID, "N_CompanyID =" + nCompanyID, connection);
                      }
                      else{
                    return Ok(_api.Error(User, "Unable to delete template already in use"));
                      }
                     
                    if (Results > 0)
                    {
                        dLayer.DeleteData("Pay_CompetencyCategory", "N_TemplateID", nTemplateID, "N_CompanyID =" + nCompanyID, connection);
                        dLayer.DeleteData("Pay_Competency", "N_TemplateID", nTemplateID, "N_CompanyID =" + nCompanyID, connection);
                        dLayer.DeleteData("Pay_TrainingNeeds", "N_TemplateID", nTemplateID, "N_CompanyID =" + nCompanyID, connection);
                        return Ok(_api.Success("Appraisal Template deleted"));
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