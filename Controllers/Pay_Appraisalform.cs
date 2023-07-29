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
        public ActionResult GetAppraisalList(int? nCompanyId, int nFnYearID, int nType, int nPage, int nSizeperpage, string xSearchkey,string xUserCategory,string xSortBy,int nUserID,int nFormID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";
            string criteria="";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
              
                    SortedList OutPut = new SortedList();
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nUserID", nUserID);

                    string userCategoryID = dLayer.ExecuteScalar("Select X_UserCategoryList from Sec_User Where N_CompanyID =" + nCompanyID + " and N_UserID=" + myFunctions.GetUserID(User) + "", Params, connection).ToString();
                    object UserCategory = dLayer.ExecuteScalar("Select count(1) from Sec_UserCategory Where N_UserCategoryID in  (" + userCategoryID + ") and X_UserCategory='Admin'", Params, connection);
                    
                    if(nFormID==1772){
                      criteria=" ";
                    }
                    else{
                         if (myFunctions.getIntVAL(UserCategory.ToString()) >0)
                        criteria=" ";
                    else 
                        criteria=" and (N_EntryUserID=@nUserID or N_UserID=@nUserID or N_EvalUserID=@nUserID or N_EmpUserID=@nUserID)  ";
                    }

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
                        sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_Pay_Appraisal where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearID and N_Type=@nType  " + Searchkey + criteria + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_Pay_Appraisal where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearID and N_Type=@nType  " + Searchkey + criteria +" and N_AppraisalID not in (select top(" + Count + ") N_AppraisalID from Vw_Pay_Appraisal where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearID and N_Type=@nType "+criteria + xSortBy + " ) " + " " + xSortBy;

                    Params.Add("@nFnYearID", nFnYearID);
                    Params.Add("@nType", nType);

                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(1) as N_Count  from Vw_Pay_Appraisal where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearID and N_Type=@nType " + criteria + Searchkey + "";
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
                return Ok(_api.Error(User, e));
            }
        }
        [HttpGet("details")]
        public ActionResult GetAppraisalDetails(string xAppraisalCode, int nFnYearID)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable MasterTable = new DataTable();
            DataTable CompetencyCategoryTable = new DataTable();
            DataTable CompetencyTable = new DataTable();
            DataTable TrainingneedsTable = new DataTable();
            DataTable GradeTypeTable = new DataTable();

            string Mastersql = "Select * from Vw_Pay_Appraisal Where N_CompanyID=@nCompanyID and x_AppraisalCode=@xAppraisalCode and n_FnYearID=@nFnYearID ";
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@xAppraisalCode", xAppraisalCode);
            Params.Add("@nFnYearID", nFnYearID);

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

                    int N_AppraisalID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AppraisalID"].ToString());
                    int N_Type = myFunctions.getIntVAL(MasterTable.Rows[0]["N_Type"].ToString());

                    string CompetencycategorySql = "select * from Vw_Pay_AppraisalCompetencyCategory where N_CompanyID=" + nCompanyID + " and N_AppraisalID=" + N_AppraisalID;

                    CompetencyCategoryTable = dLayer.ExecuteDataTable(CompetencycategorySql, Params, connection);
                    CompetencyCategoryTable = _api.Format(CompetencyCategoryTable, "Competencycategory");
                    dt.Tables.Add(CompetencyCategoryTable);

                    string CompetencySql = "select * from vw_Pay_AppraisalCompetency where N_CompanyID=" + nCompanyID + " and N_AppraisalID=" + N_AppraisalID;

                    CompetencyTable = dLayer.ExecuteDataTable(CompetencySql, Params, connection);

                    string GradeTypeSql = "select * from Pay_GradeTypeDetails where N_CompanyID=" + nCompanyID + " and N_GradeID in (select N_GradeTypeID from Pay_AppraisalCompetencyCategory where N_CompanyID=" + nCompanyID + " and N_AppraisalID=" + N_AppraisalID + " and N_GradeTypeID is not null group by N_GradeTypeID)";

                    GradeTypeTable = dLayer.ExecuteDataTable(GradeTypeSql, Params, connection);
                    GradeTypeTable = _api.Format(GradeTypeTable, "GradeType");
                    dt.Tables.Add(GradeTypeTable);

                    for (int i = 0; i < CompetencyTable.Rows.Count; i++)
                    {
                        if (myFunctions.getIntVAL(CompetencyTable.Rows[i]["N_GradeTypeID"].ToString()) > 0)
                        {
                            for (int j = 0; j < GradeTypeTable.Rows.Count; j++)
                            {
                                if (myFunctions.getIntVAL(CompetencyTable.Rows[i]["N_GradeTypeID"].ToString()) == myFunctions.getIntVAL(GradeTypeTable.Rows[j]["N_GradeID"].ToString()))
                                {
                                    if (myFunctions.getVAL(CompetencyTable.Rows[i]["N_ToBeAchieved"].ToString()) == myFunctions.getVAL(GradeTypeTable.Rows[j]["N_Weightage"].ToString()))
                                    {
                                        CompetencyTable.Rows[i]["X_Name"] = GradeTypeTable.Rows[j]["X_Name"].ToString();
                                        CompetencyTable.Rows[i]["N_ToBeAchievedGradeDetailsID"] = GradeTypeTable.Rows[j]["N_GradeDetailsID"].ToString();
                                    }

                                    if (myFunctions.getVAL(CompetencyTable.Rows[i]["N_AchievedPerc"].ToString()) == myFunctions.getVAL(GradeTypeTable.Rows[j]["N_Weightage"].ToString()))
                                    {
                                        CompetencyTable.Rows[i]["X_AchievedPerc"] = GradeTypeTable.Rows[j]["X_Name"].ToString();
                                        CompetencyTable.Rows[i]["N_AchievedGradeDetailsID"] = GradeTypeTable.Rows[j]["N_GradeDetailsID"].ToString();
                                    }
                                }
                            }
                        }
                    }

                    CompetencyTable = _api.Format(CompetencyTable, "Competency");
                    dt.Tables.Add(CompetencyTable);

                    string TrainingneedsSql = "select * from Pay_AppraisalTrainingNeeds where N_CompanyID=" + nCompanyID + " and N_AppraisalID=" + N_AppraisalID;

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

                DataTable Approvals;
                Approvals = ds.Tables["approval"];
                DataRow ApprovalRow = Approvals.Rows[0]; 

                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nAppraisalID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AppraisalID"].ToString());
                int nCategoryID = 0;
                int nCompetencyID = 0;
                int nTrainingID = 0;

                int N_SaveDraft = myFunctions.getIntVAL(MasterTable.Rows[0]["b_IsSaveDraft"].ToString());
                int nUserID = myFunctions.GetUserID(User);
                int N_NextApproverID=0;

                CompetencyCategoryCopyTable.Clear();
                CompetencyCategoryCopyTable.Columns.Add("N_CompanyID");
                CompetencyCategoryCopyTable.Columns.Add("N_AppraisalID");
                CompetencyCategoryCopyTable.Columns.Add("N_CategoryID");
                CompetencyCategoryCopyTable.Columns.Add("X_CategoryName");
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
                    row["X_CategoryName"] = dRow["X_CategoryName"];
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
  

                    SortedList EmpParams = new SortedList();
                    EmpParams.Add("@nCompanyID", nCompanyID);
                    EmpParams.Add("@n_PartyID",myFunctions.getIntVAL(MasterTable.Rows[0]["N_EmpID"].ToString()));
                    EmpParams.Add("@nFnYearID", nFnYearID);
                    object objEmpName = dLayer.ExecuteScalar("Select X_EmpName From Pay_Employee where N_EmpID=@n_PartyID and N_CompanyID=@nCompanyID  and N_FnYearID=@nFnYearID", EmpParams, connection, transaction);                     
                    object objUserID = dLayer.ExecuteScalar("select N_UserID from Sec_User where N_CompanyID=@nCompanyID and N_EmpID=@n_PartyID", EmpParams, connection, transaction);     
                    if(objUserID==null) objUserID=0;                

                    if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()) && nAppraisalID > 0)
                    {
                        int N_PkeyID = nAppraisalID;
                        string X_Criteria = "N_AppraisalID=" + nAppraisalID + " and N_CompanyID=" + nCompanyID;
                        myFunctions.UpdateApproverEntry(Approvals, "Pay_Appraisal", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        N_NextApproverID = myFunctions.LogApprovals(Approvals,myFunctions.getIntVAL(nFnYearID.ToString()), "APPRAISAL", N_PkeyID, MasterTable.Rows[0]["X_AppraisalCode"].ToString(), 1, objEmpName.ToString(), myFunctions.getIntVAL(MasterTable.Rows[0]["N_EmpID"].ToString()), "",myFunctions.getIntVAL(objUserID.ToString()), User, dLayer, connection, transaction);

                        N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Pay_Appraisal where N_AppraisalID=" + nAppraisalID + " and N_CompanyID=" + nCompanyID , connection, transaction).ToString());

                        transaction.Commit();
                        return Ok(_api.Success("Appraisal Approved " + "-" + MasterTable.Rows[0]["X_AppraisalCode"].ToString()));
                    }     

                    if (nAppraisalID > 0)
                    {
                        dLayer.DeleteData("Pay_AppraisalTrainingNeeds", "n_AppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Pay_AppraisalCompetency", "n_AppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Pay_AppraisalCompetencyCategory", "n_AppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Pay_Appraisal", "n_AppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection, transaction);
                    }   

                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_AppraisalCode"].ToString();
                    if (values == "@Auto")
                    {
                        if( myFunctions.getIntVAL(MasterTable.Rows[0]["N_Type"].ToString())==1)
                        {
                            object Count = dLayer.ExecuteScalar("select count(1) from Pay_Appraisal where N_CompanyID="+nCompanyID+" and N_EmpID="+ myFunctions.getIntVAL(MasterTable.Rows[0]["N_EmpID"].ToString())+" and N_EntryUserID="+ myFunctions.getIntVAL(MasterTable.Rows[0]["N_EntryUserID"].ToString())+" and N_Type=1 and N_EvalSettingsID="+myFunctions.getIntVAL(MasterTable.Rows[0]["N_EvalSettingsID"].ToString()), connection, transaction);
                            if(myFunctions.getIntVAL(Count.ToString())>0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, "Evaluation already done for this employee! Unable to save"));
                            }
                        }

                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.N_FormID);
                    
                        Code = dLayer.GetAutoNumber("Pay_Appraisal", "X_AppraisalCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Grade Code")); }
                        MasterTable.Rows[0]["X_AppraisalCode"] = Code;
                    }
                    MasterTable.Rows[0]["n_UserID"] = nUserID;
                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);

                    nAppraisalID = dLayer.SaveData("Pay_Appraisal", "n_AppraisalID", MasterTable, connection, transaction);
                    if (nAppraisalID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                    N_NextApproverID = myFunctions.LogApprovals(Approvals,myFunctions.getIntVAL(nFnYearID.ToString()), "APPRAISAL", nAppraisalID, MasterTable.Rows[0]["X_AppraisalCode"].ToString(), 1, objEmpName.ToString(), myFunctions.getIntVAL(MasterTable.Rows[0]["N_EmpID"].ToString()), "",myFunctions.getIntVAL(objUserID.ToString()), User, dLayer, connection, transaction);
                    N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Pay_Appraisal where N_AppraisalID=" + nAppraisalID + " and N_CompanyID=" + nCompanyID , connection, transaction).ToString());

                    for (int j = 0; j < CompetencyCategoryTable.Rows.Count; j++)
                    {
                        CompetencyCategoryTable.Rows[j]["n_AppraisalID"] = nAppraisalID;

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
                                CompetencyTable.Rows[i]["n_AppraisalID"] = nAppraisalID;
                                CompetencyTable.Rows[i]["N_CategoryID"] = nCategoryID;
                            }
                        }
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
                        TrainingneedsTable.Rows[j]["n_AppraisalID"] = nAppraisalID;
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
        public ActionResult DeleteData(int nAppraisalID, int nCompanyID,int nFnyearID, string comments)
        {
            int Results = 0;
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList(); 
                    ParamList.Add("@nTransID", nAppraisalID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    string xButtonAction = "Delete";
                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,X_AppraisalCode,N_AppraisalID,N_EmpID,X_EmpName from Vw_Pay_Appraisal where N_CompanyId=@nCompanyID and N_AppraisalID=@nTransID";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, this.N_FormID, nAppraisalID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnyearID, myFunctions.getIntVAL(TransRow["N_EmpID"].ToString()), 1454, User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), comments);
                    SqlTransaction transaction = connection.BeginTransaction();

                    string X_Criteria = "N_AppraisalID=" + nAppraisalID + " and N_CompanyID=" + nCompanyID;
                    string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());
            
                    // Results = dLayer.DeleteData("Pay_Appraisal", "N_AppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection);

                    // if (Results > 0)
                    // {
                    //     dLayer.DeleteData("Pay_AppraisalCompetencyCategory", "N_AppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection);
                    //     dLayer.DeleteData("Pay_AppraisalCompetency", "N_AppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection);
                    //     dLayer.DeleteData("Pay_AppraisalTrainingNeeds", "N_AppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection);
                    //     return Ok(_api.Success("Appraisal deleted"));
                    // }
                    // else
                    // {
                    //     return Ok(_api.Error(User, "Unable to delete"));
                    // }


                    string status = myFunctions.UpdateApprovals(Approvals, nFnyearID, "APPRAISAL", nAppraisalID, TransRow["X_AppraisalCode"].ToString(), ProcStatus, "Pay_Appraisal", X_Criteria, TransRow["X_EmpName"].ToString(), User, dLayer, connection, transaction);
                    if (status != "Error")
                    {
                        if (ButtonTag == "6" || ButtonTag == "0")
                        {
                            if (nAppraisalID > 0)
                            {
                                dLayer.DeleteData("Pay_AppraisalCompetencyCategory", "N_AppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection,transaction);
                                dLayer.DeleteData("Pay_AppraisalCompetency", "N_AppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection,transaction);
                                dLayer.DeleteData("Pay_AppraisalTrainingNeeds", "N_AppraisalID", nAppraisalID, "N_CompanyID =" + nCompanyID, connection,transaction);      
                                transaction.Commit();
                                return Ok(_api.Success("Appraisal " + status + " Successfully"));
                            }
                        
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to delete Appraisal"));
                        }
                        transaction.Commit();
                        return Ok(_api.Success("Appraisal " + status + " Successfully"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete Appraisal"));
                    }

                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("listemployee")]
        public ActionResult ListEmployee(int nFnYearID, DateTime dPeriod, int nUserID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@dPeriod", dPeriod);
            Params.Add("@nUserID", nUserID);
            string sqlCommandText = "Select N_EmpID,X_EmpCode,X_EmpName,X_Department,X_Position,N_TemplateID,X_TemplateCode,X_TemplateName,N_EvalSettingsID from vw_EmployeesOfEvaluators "+
                                    " Where N_CompanyID=@nCompanyID and N_FnyearID=@nFnYearID and N_UserID=@nUserID and D_PeriodTo>=@dPeriod and D_PeriodFrom<=@dPeriod";
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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("summarydetails")]
        public ActionResult GetAppraisalSummaryDetails(string xAppraisalCode)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable MasterTable = new DataTable();
            DataTable CompetencyCategoryTable = new DataTable();
            DataTable CompetencyTable = new DataTable();
            DataTable CompetencySummaryTable = new DataTable();
            DataTable EvaluatorsTable = new DataTable();
            DataTable TrainingneedsTable = new DataTable();

            string Mastersql = "Select * from Vw_Pay_Appraisal Where N_CompanyID=@nCompanyID and x_AppraisalCode=@xAppraisalCode ";
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@xAppraisalCode", xAppraisalCode);

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

                    int N_AppraisalID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AppraisalID"].ToString());
                    int N_TemplateID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TemplateID"].ToString());
                    int N_EmpID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_EmpID"].ToString());
                    DateTime dDate = Convert.ToDateTime(MasterTable.Rows[0]["D_DocDate"]);

                    string CompetencycategorySql = "select * from vw_Pay_CompetencyCategory where N_CompanyID=" + nCompanyID + " and N_TemplateID=" + N_TemplateID;

                    CompetencyCategoryTable = dLayer.ExecuteDataTable(CompetencycategorySql, Params, connection);
                    CompetencyCategoryTable = _api.Format(CompetencyCategoryTable, "Competencycategory");
                    dt.Tables.Add(CompetencyCategoryTable);

                    string CompetencySql = "select * from Pay_Competency where N_CompanyID=" + nCompanyID + " and N_TemplateID=" + N_TemplateID;

                    CompetencyTable = dLayer.ExecuteDataTable(CompetencySql, Params, connection);
                    CompetencyTable = _api.Format(CompetencyTable, "Competency");
                    dt.Tables.Add(CompetencyTable);

                    string EvaluatorSql = "select * from vw_Pay_EvaluatorUser where N_CompanyID=" + nCompanyID + " and N_TemplateID=" + N_TemplateID + " and D_PeriodFrom<='" + dDate + "' and  D_PeriodTo>='" + dDate + "'";

                    EvaluatorsTable = dLayer.ExecuteDataTable(EvaluatorSql, Params, connection);

                    string CompetencySummarySql = "select * from vw_Pay_AppraisalCompetencySummary where N_CompanyID=" + nCompanyID + " and N_TemplateID=" + N_TemplateID + " and N_EmpID=" + N_EmpID + " and D_DocDate<='" + EvaluatorsTable.Rows[0]["D_PeriodTo"].ToString() + "' and D_DocDate>='" + EvaluatorsTable.Rows[0]["D_PeriodFrom"].ToString() + "'";

                    CompetencySummaryTable = dLayer.ExecuteDataTable(CompetencySummarySql, Params, connection);

                    for (int i = 0; i < CompetencySummaryTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < EvaluatorsTable.Rows.Count; j++)
                        {
                            if (CompetencySummaryTable.Rows[i]["N_UserID"].ToString() == EvaluatorsTable.Rows[j]["N_UserID"].ToString())
                                CompetencySummaryTable.Rows[i]["EvaluatorWeightage"] = EvaluatorsTable.Rows[j]["N_Weightage"].ToString();
                        }
                    }

                    CompetencySummaryTable = _api.Format(CompetencySummaryTable, "CompetencySummary");
                    dt.Tables.Add(CompetencySummaryTable);

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
    }
}