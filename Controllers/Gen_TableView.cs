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
using System.Linq;

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
        private readonly string cliConnectionString;

        public Gen_TableView(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            cliConnectionString = conf.GetConnectionString("OlivoClientConnection");
            FormID = 1056;
        }



        [HttpGet("menus")]
        public ActionResult TaskList(int nMenuID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // string menus = "select X_TableViewCode,N_TableViewID,N_MenuID as formID,X_TitleLanControlNo as titleLbl,X_MenuLanControlNo as menuLbl,B_IsDefault as isDefault,B_SearchEnabled as searchEnabled,B_AttachementSearch as attachementSearch,X_PKey,N_Type,N_Order,X_PCode from Gen_TableView where N_MenuID=@nMenuID order by N_Order";

                    string menus = "select * from " +
                    "(select X_TableViewCode,N_TableViewID,N_MenuID as formID,X_TitleLanControlNo as titleLbl,X_MenuLanControlNo as menuLbl, " +
                    "B_IsDefault as isDefault,B_SearchEnabled as searchEnabled,B_AttachementSearch as attachementSearch,X_PKey,N_Type,N_Order,X_PCode,N_UserID,X_TotalField,isnull(B_IsCustomList,0) as B_IsCustomList from " +
                    "Gen_TableView where N_MenuID=@nMenuID and N_CompanyID=-1  " +
                    "and N_Type not in(select N_Type from Gen_TableView where N_MenuID=@nMenuID and N_CompanyID=@nCompanyID and N_UserID=@nUserID) " +
                    "union all " +
                    "select X_TableViewCode,N_TableViewID,N_MenuID as formID,X_TitleLanControlNo as titleLbl,X_MenuLanControlNo as menuLbl, " +
                    "B_IsDefault as isDefault,B_SearchEnabled as searchEnabled,B_AttachementSearch as attachementSearch,X_PKey,N_Type,N_Order,X_PCode,N_UserID,X_TotalField,isnull(B_IsCustomList,0) as B_IsCustomList from " +
                    "Gen_TableView where N_MenuID=@nMenuID  and N_CompanyID=@nCompanyID and N_UserID=@nUserID ) as Tbl_Gen_TableView " +
                    "order by N_Order ";

                    SortedList TviewParams = new SortedList();
                    TviewParams.Add("@nMenuID", nMenuID);
                    TviewParams.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    TviewParams.Add("@nUserID", myFunctions.GetUserID(User));

                    DataTable tableViewResult = dLayer.ExecuteDataTable(menus, TviewParams, connection);

                    tableViewResult = myFunctions.AddNewColumnToDataTable(tableViewResult, "columns", typeof(DataTable), null);
                    tableViewResult.AcceptChanges();

                    bool isCustomDefaults = false;

                    foreach (DataRow dRow in tableViewResult.Rows)
                    {
                        TviewParams["@nTableViewID"] = dRow["N_TableViewID"].ToString();
                        string tableHeaderSql = "select REPLACE(X_FieldName,' ','') as dataField,X_LanControlNo as text,B_EditLink as editLink,B_Sort as sort,X_HeaderAlign as headerAlign,X_Align as align,N_RefFormID,isnull(B_IsHidden,0) as hidden,X_DataType as formatTo,B_SystemField,X_Formatter,N_Width as width from Gen_TableViewDetails where N_TableViewID=@nTableViewID order by N_Order";

                        DataTable tableColumnResult = dLayer.ExecuteDataTable(tableHeaderSql, TviewParams, connection);
                        dRow["columns"] = tableColumnResult;

                        if (myFunctions.getIntVAL(dRow["N_UserID"].ToString()) > 0 && myFunctions.getBoolVAL(dRow["isDefault"].ToString()) == true)
                        {
                            isCustomDefaults = true;
                        }

                    }
                    tableViewResult.AcceptChanges();
                    if (isCustomDefaults)
                    {
                        foreach (DataRow dRow in tableViewResult.Rows)
                        {
                            if (myFunctions.getIntVAL(dRow["N_UserID"].ToString()) == 0 && myFunctions.getBoolVAL(dRow["isDefault"].ToString()) == true)
                            {
                                dRow["isDefault"] = false;
                            }
                        }
                        tableViewResult.AcceptChanges();
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
        public ActionResult GetDashboardList(int nFnYearID, int nPage, int nSizeperpage, string xSearchkey, string xSearchField, string xSortBy, int nBranchID, int nEmpID, int nUserID, int nDecimalPlace, bool bAllBranchData, int nFormID, int nTableViewID,int nLocationID, bool export, int nCountryID, int customerID, int deptID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            SortedList OutPut = new SortedList();
            SortedList Params = new SortedList();
            Params.Add("@cVal", nCompanyID);
            Params.Add("@fVal", nFnYearID);
            Params.Add("@bVal", nBranchID);
            Params.Add("@tbvVal", nTableViewID);
            Params.Add("@mnuVal", nFormID);
            Params.Add("@empVal", nEmpID);
            Params.Add("@userVal", nUserID);
            Params.Add("@lVal", nLocationID);
            Params.Add("@ctrVal", nCountryID);
            Params.Add("@custVal", customerID);
            Params.Add("@deptVal", deptID);

            string UserPattern = myFunctions.GetUserPattern(User);
            if (UserPattern != "")
                Params.Add("@userPattern", UserPattern);



            string sqlCommandCount = "";

            DataTable dt = new DataTable();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            string Criterea = "";
            string FieldList = "";
            string DataSource = "";
            string DataSource2 = "";
            string PKey = "";
            string BranchCriterea = "";
            string LocationCriterea = "";
            string SortBy = "";
            string PatternCriteria = "";
            string SumField = "";
            string expFieldList1 = "";
            string expFieldList = "";

            string dataType = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    string SortListSql = "SELECT isnull(Gen_TableViewDetails.X_FieldName,'') as X_FieldName,isnull(Gen_TableViewDetails.X_DataType,'text') as X_DataType,isnull(Gen_TableViewDetails.B_Search,0) as B_Search FROM Gen_TableViewDetails INNER JOIN Gen_TableView ON Gen_TableViewDetails.N_CompanyID = Gen_TableView.N_CompanyID AND Gen_TableViewDetails.N_TableViewID = Gen_TableView.N_TableViewID WHERE (Gen_TableViewDetails.N_TableViewID=@tbvVal) AND (Gen_TableView.N_MenuID=@mnuVal)";
                    DataTable SortList = dLayer.ExecuteDataTable(SortListSql, Params, connection);
                    foreach (DataRow cRow in SortList.Rows)
                    {
                        if (xSearchkey != null && xSearchkey.Trim() != "" && xSearchField == "All")
                        {
                            if (myFunctions.getBoolVAL(cRow["B_Search"].ToString()))
                            {
                                //if(cRow["X_FieldName"].ToString()) contain _ cut before _ // if underscore on  middle then replace into space '' 
                                if (cRow["X_FieldName"].ToString().Contains("Date") || cRow["X_FieldName"].ToString().ToString().Contains("date"))
                                {
                                    Searchkey = Searchkey + " or REPLACE(CONVERT(varchar(11), [" + cRow["X_FieldName"].ToString() + "], 106), ' ', '-') like '%" + xSearchkey + "%'";
                                }
                                else
                                    Searchkey = Searchkey + " or [" + cRow["X_FieldName"].ToString() + "] like '%" + xSearchkey + "%'";
                            }
                        }
                        if(cRow["X_FieldName"].ToString().Contains("_"))
                        {
                           expFieldList1=  cRow["X_FieldName"].ToString().Substring(cRow["X_FieldName"].ToString().IndexOf('_') + 1);
                           
                        }
                        else
                        {
                            expFieldList1 =  cRow["X_FieldName"].ToString();
                        }

                        FieldList = FieldList + ",[" + cRow["X_FieldName"].ToString() + "]";
                        expFieldList= expFieldList+ ",[" + cRow["X_FieldName"].ToString() + "]" + " AS " + expFieldList1;

                    }
                    if (xSearchField != "All")
                    {
                        Searchkey = Searchkey + " or [" + xSearchField + "] like '%" + xSearchkey + "%'";
                    }
                    if (expFieldList.Length > 1)
                    { expFieldList = expFieldList.Substring(1); }
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
                        if (xSortBy.ToString().Contains("Date") || xSortBy.ToString().Contains("date"))
                        {
                            xSortBy = "Convert(varchar,CAST("+xSortBy.Split(" ")[0]+" AS datetime), 112) " + xSortBy.Split(" ")[1];
                        }
                        xSortBy = " order by " + xSortBy;
                    }

                    string CriteriaSql = "select isnull(X_DataSource,'') as X_DataSource,isnull(X_DefaultCriteria,'') as X_DefaultCriteria,isnull(X_BranchCriteria,'') as X_BranchCriteria,isnull(X_LocationCriteria,'') as X_LocationCriteria,isnull(X_DefaultSortField,'') as X_DefaultSortField,isnull(X_PKey,'') as X_PKey,isnull(X_PatternCriteria,'') as X_PatternCriteria,X_TotalField,isnull(X_DataSource2,'') as X_DataSource2 from Gen_TableView where (N_TableViewID = @tbvVal) AND (N_MenuID=@mnuVal)";
                    DataTable CriteriaList = dLayer.ExecuteDataTable(CriteriaSql, Params, connection);

                    if (CriteriaList.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "List Definition not found"));
                    }

                    DataRow dRow = CriteriaList.Rows[0];

                    SumField = dRow["X_TotalField"].ToString();
                    if (xSortBy == null)
                    {
                        if ((SortBy == null || SortBy.Trim() == "") && dRow["X_DefaultSortField"].ToString() != "")
                        {
                            SortBy = " order by " + dRow["X_DefaultSortField"].ToString();
                        }
                    }

                    if (dRow["X_DefaultCriteria"].ToString() != "")
                    {
                        if (nFormID == 81)
                            Criterea = "N_CompanyID=@cVal and N_FnYearID=@fVal and N_FormID in (81,1459) and N_CompanyID=@cVal and N_FnYearID=@fVal";
                        else if (nFormID == 1459)
                            Criterea = "N_CompanyID=@cVal and N_FnYearID=@fVal and N_FormID in (81,1459) and N_CompanyID=@cVal and N_FnYearID=@fVal and N_CustomerID=@custVal";
                        else
                            Criterea = " ( " + dRow["X_DefaultCriteria"].ToString() + " ) ";

                        if (nFormID == 1732)
                            Criterea = "N_CompanyID=@cVal and N_FnYearID=@fVal and N_FormID in (64) and N_CompanyID=@cVal and N_FnYearID=@fVal and N_CustomerID=@custVal";

                        if (nFormID == 1335 && deptID == 0)
                            Criterea = "N_FnYearID=@fVal and N_BranchID=@bVal";
                    }

                    if (bAllBranchData == false && dRow["X_BranchCriteria"].ToString() != "")
                    {
                        BranchCriterea = " ( " + dRow["X_BranchCriteria"].ToString() + " ) ";
                    }

                    if (UserPattern != "" && dRow["X_PatternCriteria"].ToString() != "")
                    {
                        if(nFormID==1305)
                        {
                             PatternCriteria = " (  Left(X_Pattern,Len(@userPattern))=@userPattern OR N_CreatedUser=0) ";

                        }
                        else
                        {
                             PatternCriteria = " (  Left(X_Pattern,Len(@userPattern))=@userPattern ) ";
                        }
                       
                    }
                      if (dRow["X_LocationCriteria"].ToString() != "")
                    {
                        LocationCriterea = " ( " + dRow["X_LocationCriteria"].ToString() + " ) ";
                    }

                    if (BranchCriterea.Trim().Length > 0)
                    {
                        if (Criterea.Trim().Length > 0)
                        {
                            Criterea = Criterea + " and " + BranchCriterea;
                        }
                        else
                        {
                            Criterea = BranchCriterea;
                        }
                    }

                    if (Searchkey.Trim().Length > 0)
                    {
                        if (Criterea.Trim().Length > 0)
                        {
                            Criterea = Criterea + " and " + Searchkey;
                        }
                        else
                        {
                            Criterea = Searchkey;
                        }
                    }

                    if (PatternCriteria.Trim().Length > 0)
                    {
                        if (Criterea.Trim().Length > 0)
                        {
                            Criterea = Criterea + " and " + PatternCriteria;
                        }
                        else
                        {
                            Criterea = PatternCriteria;
                        }
                    }


                      if (LocationCriterea.Trim().Length > 0)
                    {
                        if (Criterea.Trim().Length > 0)
                        {
                            Criterea = Criterea + " and " + LocationCriterea;
                        }
                        else
                        {
                            Criterea = LocationCriterea;
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
                    if (dRow["X_DataSource2"].ToString() != "")
                    {
                        DataSource2 = dRow["X_DataSource2"].ToString();
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Data Source2 Not Found"));
                    }
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") " + FieldList + " from " + DataSource + Criterea + SortBy + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") " + FieldList + " from " + DataSource + Criterea + " and " + PKey + " not in " + "(select top(" + Count + ") " + PKey + " from " + DataSource2 + Criterea + SortBy + xSortBy + " ) " + SortBy + xSortBy;
                    // if (Count == 0)
                    //     sqlCommandText = "select top(" + nSizeperpage + ") " + FieldList + " from " + DataSource + Criterea + SortBy;
                    // else
                    //     sqlCommandText = "select top(" + nSizeperpage + ") " + FieldList + " from " + DataSource + Criterea + " and " + PKey + " not in " + "(select top(" + Count + ") " + PKey + " from " + DataSource2 + Criterea + SortBy + " ) " + SortBy;

                    if (export)
                    {
                        sqlCommandText = "select " + expFieldList + " from " + DataSource + Criterea + SortBy;
                        string fileName = "Exported_List_" + RandomString();
                        if (nFormID == 1650)
                        {
                            using (SqlConnection cliConn = new SqlConnection(cliConnectionString))
                            {
                                cliConn.Open();
                                myFunctions.QryToExcel(User, sqlCommandText, fileName, Params, dLayer, cliConn);
                            }
                        }
                        else
                        {
                            myFunctions.QryToExcel(User, sqlCommandText, fileName, Params, dLayer, connection);
                        }

                        fileName = fileName + ".xls";
                        return Ok(_api.Success(new SortedList() { { "FileName", fileName } }));
                    }
                    else
                    {

                        if (SumField == null)
                        { SumField = ""; }

                        if (nFormID == 1650)
                        {
                            if (Count != 0)
                              {
                               sqlCommandText = "select top(" + nSizeperpage + ") " + FieldList + " from " + DataSource + " where " + PKey + " not in " + "(select top(" + Count + ") " + PKey + " from " + DataSource2 + Criterea + SortBy + " ) " + SortBy;
                              }
                            using (SqlConnection cliConn = new SqlConnection(cliConnectionString))
                            {
                                cliConn.Open();
                                dt = dLayer.ExecuteDataTable(sqlCommandText, Params, cliConn);
                            }
                        }
                        else
                        {
                            dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                        }

                        sqlCommandCount = "select count(1) as N_Count,0 as TotalAmount  from " + DataSource + Criterea;

                        if (SumField.Trim() != "")
                        {
                            sqlCommandCount = "select count(1) as N_Count ,sum(Cast(REPLACE(" + SumField + ",',','') as Numeric(16," + nDecimalPlace + ")) ) as TotalAmount  from " + DataSource + Criterea;
                        }
                        DataTable Summary = new DataTable();
                        if (nFormID == 1650)
                        {
                             
                            using (SqlConnection cliConn = new SqlConnection(cliConnectionString))
                            {
                                cliConn.Open();
                                Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, cliConn);
                            }
                        }
                        else
                        {
                            Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                        }


                        string TotalCount = "0";
                        string TotalSum = "0";
                        if (Summary.Rows.Count > 0)
                        {
                            DataRow drow = Summary.Rows[0];
                            TotalCount = drow["N_Count"].ToString();
                            TotalSum = drow["TotalAmount"].ToString();
                        }

                        OutPut.Add("TotalSum", TotalSum);
                        OutPut.Add("Details", _api.Format(dt));
                        OutPut.Add("TotalCount", TotalCount);

                        return Ok(_api.Success(OutPut));
                    }



                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("viewdetails")]
        public ActionResult GetTableViewDetails(int nMenuID, int nTableViewID, int nTypeID, int nUserID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable DefaultMasterTable = new DataTable();
            DataTable MasterTable = new DataTable();
            DataTable DefaultDetailTable = new DataTable();
            DataTable DetailTable = new DataTable();

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nMenuID", nMenuID);
            Params.Add("@nTableViewID", nTableViewID);
            Params.Add("@nTypeID", nTypeID);
            Params.Add("@nUserID", nUserID);


            string DefaultMastersql = "", DefaultDetailSql = "", Mastersql = "", DetailSql = "";

            DefaultMastersql = "select b_AttachementSearch,n_MenuID,n_Type,n_UserID,n_CompanyID,n_TableViewID,X_DefaultSortField,case when (select count(B_IsDefault) as userDefaults from Gen_TableView where N_CompanyID=@nCompanyID and N_UserID=@nUserID and N_MenuID=@nMenuID)>0 then cast(0 as bit) else B_IsDefault end as B_IsDefault from Gen_TableView where N_CompanyID=-1 and N_UserID=0 and N_MenuID=@nMenuID and N_Type=@nTypeID";
            DefaultDetailSql = "select * from vw_Gen_TableViewDetails where N_CompanyID=-1 and N_MenuID=@nMenuID and N_Type=@nTypeID and N_TableViewID=(select max(N_TableViewID) as N_TableViewID from Gen_TableView where N_CompanyID=-1 and N_UserID=0 and N_MenuID=@nMenuID and N_Type=@nTypeID)";

            Mastersql = "select b_AttachementSearch,n_MenuID,n_Type,n_UserID,n_CompanyID,n_TableViewID,X_DefaultSortField,B_IsDefault from Gen_TableView where N_CompanyID=@nCompanyID and N_UserID=@nUserID and N_MenuID=@nMenuID and N_Type=@nTypeID";
            DetailSql = "select * from vw_Gen_TableViewDetails where N_CompanyID=@nCompanyID and N_MenuID=@nMenuID and N_Type=@nTypeID and N_TableViewID=(select max(N_TableViewID) as N_TableViewID from Gen_TableView where N_CompanyID=@nCompanyID and N_UserID=@nUserID and N_MenuID=@nMenuID and N_Type=@nTypeID)";



            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DefaultMasterTable = dLayer.ExecuteDataTable(DefaultMastersql, Params, connection);
                    DefaultMasterTable = _api.Format(DefaultMasterTable, "DefaultMasterTable");
                    DefaultDetailTable = dLayer.ExecuteDataTable(DefaultDetailSql, Params, connection);
                    DefaultDetailTable = _api.Format(DefaultDetailTable, "DefaultDetailTable");
                    dt.Tables.Add(DefaultMasterTable);
                    dt.Tables.Add(DefaultDetailTable);

                    if (DefaultMasterTable.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Listing Configuration not found"));
                    }

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    MasterTable = _api.Format(MasterTable, "MasterTable");
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "DetailTable");
                    dt.Tables.Add(MasterTable);
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
                int nCompanyID = myFunctions.GetCompanyID(User);
                int nUserID = myFunctions.GetUserID(User);
                int nMenuID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_MenuID"].ToString());
                int nTypeID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_Type"].ToString());
                int nTableViewID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_TableViewID"].ToString());
                bool isDefault = myFunctions.getBoolVAL(MasterTable.Rows[0]["b_IsDefault"].ToString());
                string xSortFeild = MasterTable.Rows[0]["x_Sort"].ToString();
                string xSortBy = MasterTable.Rows[0]["x_SortBy"].ToString();


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    string SortFeild = xSortFeild == "Created_Time_ID" ? " X_DefaultSortField" : "'" + xSortFeild + " " + xSortBy + "' as X_DefaultSortField";
                    string masterSql = "SELECT " + nCompanyID + " as N_CompanyID,(select max(N_TableViewID)+1 from Gen_TableView ) as X_TableViewCode,0 as N_TableViewID, N_MenuID, X_TitleLanControlNo, X_MenuLanControlNo, X_ActionBtnLanControlNo, " +
                    " 0 as B_IsDefault, X_DataSource, X_DefaultCriteria, X_BranchCriteria, X_LocationCriteria, X_PKey, X_PCode," + nUserID + " as N_UserID, B_SearchEnabled, B_AttachementSearch, " +
                    " X_TotalField, N_Type, N_Order," + SortFeild + ",X_PatternCriteria,B_IsCustomList,X_DataSource2 FROM Gen_TableView where N_CompanyID=-1 and N_MenuID=" + nMenuID + " and N_Type=" + nTypeID + " and N_UserID=0";
                    MasterTable = dLayer.ExecuteDataTable(masterSql, Params, connection, transaction);
                    dLayer.ExecuteScalar("delete from Gen_TableViewDetails where N_TableViewID in (select N_TableViewID from Gen_TableView where N_CompanyID=" + nCompanyID + " and N_MenuID=" + nMenuID + " and N_Type=" + nTypeID + " and N_UserID=" + nUserID + ") and N_CompanyID=" + nCompanyID, connection, transaction);
                    dLayer.ExecuteScalar("delete from Gen_TableView where N_CompanyID=" + nCompanyID + " and N_MenuID=" + nMenuID + " and N_Type=" + nTypeID + " and N_UserID=" + nUserID, connection, transaction);

                    int newTableViewID = dLayer.SaveData("Gen_TableView", "N_TableViewID", MasterTable, connection, transaction);
                    if (newTableViewID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save configurations");
                    }

                    if (DetailTable.Columns.Contains("n_MenuID"))
                    {
                        DetailTable.Columns.Remove("n_MenuID");
                    }
                    if (DetailTable.Columns.Contains("n_Type"))
                    {
                        DetailTable.Columns.Remove("n_Type");
                    }

                    foreach (DataRow Avar in DetailTable.Rows)
                    {
                        Avar["N_CompanyID"] = nCompanyID;
                        Avar["n_TableViewID"] = newTableViewID;
                        Avar["n_TableViewDetailsID"] = 0;
                    }
                    DetailTable.AcceptChanges();
                    int newTableViewDetailID = dLayer.SaveData("Gen_TableViewDetails", "n_TableViewDetailsID", DetailTable, connection, transaction);
                    if (newTableViewDetailID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save configurations");
                    }
                    if (isDefault)
                    {
                        dLayer.ExecuteNonQuery("Update Gen_TableView set b_IsDefault=0 where N_CompanyID=" + nCompanyID + " and N_MenuID=" + nMenuID + " and N_UserID=" + nUserID, connection, transaction);
                        dLayer.ExecuteNonQuery("Update Gen_TableView set b_IsDefault=1 where N_CompanyID=" + nCompanyID + " and N_MenuID=" + nMenuID + " and N_Type=" + nTypeID + " and N_UserID=" + nUserID, connection, transaction);
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Listing Configuration Updated"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("reset")]
        public ActionResult DeleteData(int nMenuID, int nType)
        {
            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dLayer.ExecuteScalar("delete from Gen_TableViewDetails where N_TableViewID in (select N_TableViewID from Gen_TableView where N_CompanyID=" + nCompanyID + " and N_MenuID=" + nMenuID + " and N_Type=" + nType + " and N_UserID=" + nUserID + ") and N_CompanyID=" + nCompanyID, connection);
                    dLayer.ExecuteScalar("delete from Gen_TableView where N_CompanyID=" + nCompanyID + " and N_MenuID=" + nMenuID + " and N_Type=" + nType + " and N_UserID=" + nUserID, connection);


                }

                return Ok(_api.Success("Listing Configuration Restored"));
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }

        }
        private static Random random = new Random();
        public string RandomString(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


    }
}