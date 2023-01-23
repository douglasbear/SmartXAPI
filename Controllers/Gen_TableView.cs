using AutoMapper;
using SmartxAPI.Data;
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
    [Route("listing")]
    [ApiController]
    public class Gen_TableView : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;

        public Gen_TableView(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1056;
        }



        [HttpGet("menus")]
        public ActionResult TaskList(int nMenuID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string menus = "select X_TableViewCode,N_TableViewID,N_MenuID as formID,X_TitleLanControlNo as titleLbl,X_MenuLanControlNo as menuLbl,B_IsDefault as isDefault,B_SearchEnabled as searchEnabled,B_AttachementSearch as attachementSearch,X_PKey,N_Type,N_Order,X_PCode from Gen_TableView where N_MenuID=@nMenuID order by N_Order";
                    SortedList TviewParams = new SortedList();
                    TviewParams.Add("@nMenuID", nMenuID);

                    DataTable tableViewResult = dLayer.ExecuteDataTable(menus, TviewParams, connection);

                    tableViewResult = myFunctions.AddNewColumnToDataTable(tableViewResult, "columns", typeof(DataTable), null);
                    tableViewResult.AcceptChanges();

                    foreach (DataRow dRow in tableViewResult.Rows)
                    {
                        TviewParams["@nTableViewID"] = dRow["N_TableViewID"].ToString();
                        string tableHeaderSql = "select X_FieldName as dataField,X_LanControlNo as text,B_EditLink as editLink,B_Sort as sort,X_HeaderAlign as headerAlign,X_Align as align,N_RefFormID,isnull(B_IsHidden,0) as hidden from Gen_TableViewDetails where N_TableViewID=@nTableViewID order by N_Order";

                        DataTable tableColumnResult = dLayer.ExecuteDataTable(tableHeaderSql, TviewParams, connection);
                        dRow["columns"] = tableColumnResult;

                    }


                    return Ok(_api.Success(_api.Format(tableViewResult)));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("dashboardList")]
        public ActionResult GetDashboardList(int nFnYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, int nBranchID, bool bAllBranchData, int nFormID, int nTableViewID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            SortedList OutPut = new SortedList();
            SortedList Params = new SortedList();
            Params.Add("@cVal", nCompanyID);
            Params.Add("@fVal", nFnYearID);
            Params.Add("@bVal", nBranchID);
            Params.Add("@tbvVal", nTableViewID);
            Params.Add("@mnuVal", nFormID);


            string sqlCommandCount = "";

            DataTable dt = new DataTable();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            string Criterea = "";
            string FieldList = "";
            string DataSource = "";
            string PKey = "";
            string BranchCriterea = "";
            string SortBy = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    string SortListSql = "SELECT isnull(Gen_TableViewDetails.X_FieldName,'') as X_FieldName,isnull(Gen_TableViewDetails.X_DataType,'text') as X_DataType,isnull(Gen_TableViewDetails.B_Search,0) as B_Search FROM Gen_TableViewDetails INNER JOIN Gen_TableView ON Gen_TableViewDetails.N_CompanyID = Gen_TableView.N_CompanyID AND Gen_TableViewDetails.N_TableViewID = Gen_TableView.N_TableViewID WHERE (Gen_TableViewDetails.N_TableViewID=@tbvVal) AND (Gen_TableView.N_MenuID=@mnuVal)";
                    DataTable SortList = dLayer.ExecuteDataTable(SortListSql, Params, connection);
                    foreach (DataRow cRow in SortList.Rows)
                    {
                        if (xSearchkey != null && xSearchkey.Trim() != "")
                        {
                            if (myFunctions.getBoolVAL(cRow["B_Search"].ToString()))
                            {
                                Searchkey = Searchkey + " or " + cRow["X_FieldName"].ToString() + " like '%" + xSearchkey + "%'";
                            }
                        }

                        FieldList = FieldList + "," + cRow["X_FieldName"].ToString();

                    }

                    if (Searchkey.Length > 3)
                    { Searchkey = Searchkey.Substring(3); }
                    if (FieldList.Length > 1)
                    { FieldList = FieldList.Substring(1); }
                    else
                    { return Ok(_api.Error(User, "Invalid Field Definition")); }
                    if (Searchkey.Length > 0)
                    { Searchkey = " ( " + Searchkey + " ) "; }



                    if (xSortBy != null && xSortBy.Trim() != "")
                    {
                        SortBy = " order by " + xSortBy;
                    }

                    string CriteriaSql = "select isnull(X_DataSource,'') as X_DataSource,isnull(X_DefaultCriteria,'') as X_DefaultCriteria,isnull(X_BranchCriteria,'') as X_BranchCriteria,isnull(X_DefaultSortField,'') as X_DefaultSortField,isnull(X_PKey,'') as X_PKey from Gen_TableView where (N_TableViewID = @tbvVal) AND (N_MenuID=@mnuVal)";
                    DataTable CriteriaList = dLayer.ExecuteDataTable(CriteriaSql, Params, connection);

                    if (CriteriaList.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "List Definition not found"));
                    }

                    DataRow dRow = CriteriaList.Rows[0];

                    if ((SortBy == null || SortBy.Trim() == "") && dRow["X_DefaultSortField"].ToString() != "")
                    {
                        SortBy = " order by " + dRow["X_DefaultSortField"].ToString();
                    }

                    if (dRow["X_DefaultCriteria"].ToString() != "")
                    {
                        Criterea = " ( " + dRow["X_DefaultCriteria"].ToString() + " ) ";
                    }

                    if (bAllBranchData == false && dRow["X_BranchCriteria"].ToString() != "")
                    {
                        BranchCriterea = " ( " + dRow["X_BranchCriteria"].ToString() + " ) ";
                    }

                    if (BranchCriterea.Trim().Length > 0)
                    {
                        if (Criterea.Trim().Length > 0)
                        {
                            Criterea = Criterea + " and " + BranchCriterea;
                        }
                    }

                    if (Searchkey.Trim().Length > 0)
                    {
                        if (Criterea.Trim().Length > 0)
                        {
                            Criterea = Criterea + " and " + Searchkey;
                        }
                    }

                    if (Criterea.Trim().Length > 0)
                    {
                        Criterea = " where " + Criterea;
                    }



                    if (dRow["X_DataSource"].ToString() != "")
                    {
                        DataSource = dRow["X_DataSource"].ToString();
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Data Source Not Found"));
                    }

                    if (dRow["X_PKey"].ToString() != "")
                    {
                        PKey = dRow["X_PKey"].ToString();
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Key Value Not Found"));
                    }

                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") " + FieldList + " from " + DataSource + Criterea + SortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") " + FieldList + " from " + DataSource + Criterea + " and " + PKey + " not in " + "(select top(" + Count + ") " + PKey + " from " + DataSource + Criterea + SortBy + " ) " + SortBy;


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from " + DataSource + Criterea;

                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);

                    return Ok(_api.Success(OutPut));

                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }




        [HttpGet("list")]
        public ActionResult TaskList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy, int nTableViewID, int nMenuID, int n_UserID, DateTime d_Date, bool isMyTeam)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID;
            if (n_UserID > 0)
            {
                nUserID = n_UserID;
            }
            else
            {
                nUserID = myFunctions.GetUserID(User);
            }

            DataTable dt = new DataTable();
            SortedList OutPut = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";
            string Criteria = "";
            string Criteria2 = "";
            object TotalCount = 0;
            string Header = "[]";
            string UserPattern = "";
            // if (byUser == true)
            // {
            //     Criteria = " and N_AssigneeID=@nUserID ";
            // }

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_TaskCode like '%" + xSearchkey + "%' OR X_TaskSummery like '%" + xSearchkey + "%' OR X_TaskSummery like '%" + xSearchkey + "%' OR X_TaskDescription like '%" + xSearchkey + "%' OR X_Assignee like '%" + xSearchkey + "%' OR X_Submitter like '%" + xSearchkey + "%' OR X_ClosedUser like '%" + xSearchkey + "%'  OR X_ProjectName like '%" + xSearchkey + "%' OR X_CategoryName like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_TaskID desc";
            else
                xSortBy = " order by " + xSortBy;




            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string tableViewSql = "select X_HiddenFields,X_Fields,X_DataSource,X_DefaultCriteria,X_PKey from Sec_TableViewComponents where N_MenuID=@nMenuID and N_TableViewID=@nTableViewID and N_CompanyID=@nCompanyID";
                    SortedList TviewParams = new SortedList();
                    TviewParams.Add("@nMenuID", nMenuID);
                    TviewParams.Add("@nTableViewID", nTableViewID);
                    TviewParams.Add("@nCompanyID", nCompanyId);
                    TviewParams.Add("@nUserID", nUserID);

                    DataTable tableViewResult = dLayer.ExecuteDataTable(tableViewSql, TviewParams, connection);

                    if (tableViewResult.Rows.Count > 0)
                    {
                        DataRow dRow = tableViewResult.Rows[0];
                        string Table = dRow["X_DataSource"].ToString();
                        string HiddenFields = dRow["X_HiddenFields"].ToString();
                        string DefaultCriteria = dRow["X_DefaultCriteria"].ToString();
                        Header = dRow["X_Fields"].ToString();
                        string PKey = dRow["X_PKey"].ToString();
                        if (isMyTeam == true)
                        {
                            object patternCode = dLayer.ExecuteScalar("select X_Pattern From Sec_UserHierarchy where N_CompanyID =@nCompanyID and N_UserID =@nUserID", TviewParams, connection);
                            UserPattern = patternCode.ToString();
                        }
                        else
                        {
                            UserPattern = myFunctions.GetUserPattern(User);
                        }



                        string Pattern = "";

                        if (UserPattern != "")
                        {
                            if (nTableViewID == 1)
                                Pattern = " and ( Left(X_Pattern,Len(" + UserPattern + "))=" + UserPattern + " or N_CreatorID=" + myFunctions.GetUserID(User) + ")";
                            else if (nTableViewID == 6)
                            {
                                Pattern = " and ( Left(X_Pattern,Len(" + UserPattern + "))=" + UserPattern + " or N_CreaterID=" + myFunctions.GetUserID(User) + ")";

                            }
                            else
                                Pattern = " and ( Left(X_Pattern,Len(" + UserPattern + "))=" + UserPattern + ")";

                        }
                        if (nTableViewID == 7 || nTableViewID == 8 || nTableViewID == 5)
                        {
                            Pattern = "";
                        }



                        if (Count == 0)
                            sqlCommandText = "select top(" + nSizeperpage + ") * from " + Table + " where " + DefaultCriteria + Pattern + " " + Searchkey + Criteria + Criteria2 + xSortBy;
                        else
                            sqlCommandText = "select top(" + nSizeperpage + ") * from " + Table + " where " + DefaultCriteria + Pattern + " " + Searchkey + Criteria + Criteria2 + " and " + PKey + " not in (select top(" + Count + ") N_TaskID from " + Table + " where " + DefaultCriteria + " " + Criteria + Criteria2 + xSortBy + " ) " + xSortBy;

                        SortedList Params = new SortedList();
                        if (DefaultCriteria.Contains("@cVal"))
                            Params.Add("@cVal", nCompanyId);
                        if (DefaultCriteria.Contains("@uVal"))
                            Params.Add("@uVal", nUserID);
                        if (DefaultCriteria.Contains("@dVal"))
                            Params.Add("@dVal", d_Date);



                        dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                        sqlCommandCount = "select count(*) as N_Count  from " + Table + " where " + DefaultCriteria + Pattern;
                        TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    }



                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("Header", Newtonsoft.Json.JsonConvert.SerializeObject(Header));
                    OutPut.Add("TotalCount", TotalCount);

                    return Ok(_api.Success(OutPut));


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
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nTableViewID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_TableViewID"].ToString());
                int nTableViewDetailsID = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();

                    // Auto Gen
                    string xTableViewCode = "";
                    var values = MasterTable.Rows[0]["X_TableViewCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", 1642);
                        Params.Add("N_TableViewID", nTableViewID);
                        xTableViewCode = dLayer.GetAutoNumber("Sec_TableView", "X_TableViewCode", Params, connection, transaction);
                        if (xTableViewCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Code")); }
                        MasterTable.Rows[0]["X_TableViewCode"] = xTableViewCode;
                    }
                    MasterTable.Columns.Remove("N_FnYearId");

                    //Delete Existing Data
                    dLayer.DeleteData("Sec_TableView", "N_TableViewID", nTableViewID, "", connection, transaction);
                    dLayer.DeleteData("Sec_TableViewDetails", "N_TableViewID", nTableViewID, "", connection, transaction);

                    nTableViewID = dLayer.SaveData("Sec_TableView", "N_TableViewID", MasterTable, connection, transaction);
                    if (nTableViewID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_TableViewID"] = nTableViewID;
                    }
                    nTableViewDetailsID = dLayer.SaveData("Sec_TableViewDetails", "N_TableViewDetailsID", DetailTable, connection, transaction);
                    if (nTableViewDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Table View Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("viewdetails")]
        public ActionResult GetTableViewDetails(int nMenuID, int nUserID, int nLangID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            string Mastersql = "";
            string DetailSql = "";


            Mastersql = "select * from Sec_TableView where N_CompanyID=@p1 and N_UserID=@p2 and N_MenuID=@p3";

            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nUserID);
            Params.Add("@p3", nMenuID);
            Params.Add("@p4", nLangID);

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
                    DetailSql = "select * from vw_ListSettings where N_CompanyID=@p1 and N_UserID=@p2 and N_MenuID=@p3 and N_LanguageId=@p4";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    if (DetailTable.Rows.Count == 0)
                    {
                        DetailSql = "select * from vw_ListSettings where N_CompanyID=@p1 and N_UserID=0 and N_MenuID=@p3 and N_LanguageId=@p4";
                        DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    }

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


    }
}