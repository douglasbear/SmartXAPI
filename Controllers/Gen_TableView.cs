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
                    // string menus = "select X_TableViewCode,N_TableViewID,N_MenuID as formID,X_TitleLanControlNo as titleLbl,X_MenuLanControlNo as menuLbl,B_IsDefault as isDefault,B_SearchEnabled as searchEnabled,B_AttachementSearch as attachementSearch,X_PKey,N_Type,N_Order,X_PCode from Gen_TableView where N_MenuID=@nMenuID order by N_Order";

                    string menus = "select * from "+
                    "(select X_TableViewCode,N_TableViewID,N_MenuID as formID,X_TitleLanControlNo as titleLbl,X_MenuLanControlNo as menuLbl, "+
                    "B_IsDefault as isDefault,B_SearchEnabled as searchEnabled,B_AttachementSearch as attachementSearch,X_PKey,N_Type,N_Order,X_PCode from "+
                    "Gen_TableView where N_MenuID=@nMenuID  "+
                    "and N_Type not in(select N_Type from Gen_TableView where N_MenuID=@nMenuID and N_CompanyID=@nCompanyID and N_UserID=@nUserID) "+
                    "union all "+
                    "select X_TableViewCode,N_TableViewID,N_MenuID as formID,X_TitleLanControlNo as titleLbl,X_MenuLanControlNo as menuLbl, "+
                    "B_IsDefault as isDefault,B_SearchEnabled as searchEnabled,B_AttachementSearch as attachementSearch,X_PKey,N_Type,N_Order,X_PCode from "+
                    "Gen_TableView where N_MenuID=@nMenuID  and N_CompanyID=@nCompanyID and N_UserID=@nUserID) as Tbl_Gen_TableView "+
                    "order by N_Order ";

                    SortedList TviewParams = new SortedList();
                    TviewParams.Add("@nMenuID", nMenuID);
                    TviewParams.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    TviewParams.Add("@nUserID", myFunctions.GetUserID(User));

                    DataTable tableViewResult = dLayer.ExecuteDataTable(menus, TviewParams, connection);

                    tableViewResult = myFunctions.AddNewColumnToDataTable(tableViewResult, "columns", typeof(DataTable), null);
                    tableViewResult.AcceptChanges();

                    foreach (DataRow dRow in tableViewResult.Rows)
                    {
                        TviewParams["@nTableViewID"] = dRow["N_TableViewID"].ToString();
                        string tableHeaderSql = "select X_FieldName as dataField,X_LanControlNo as text,B_EditLink as editLink,B_Sort as sort,X_HeaderAlign as headerAlign,X_Align as align,N_RefFormID,isnull(B_IsHidden,0) as hidden,X_DataType as formatTo from Gen_TableViewDetails where N_TableViewID=@nTableViewID order by N_Order";

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

            DefaultMastersql = "select b_AttachementSearch,n_MenuID,n_Type,n_UserID,n_CompanyID,n_TableViewID from Gen_TableView where N_CompanyID=-1 and N_UserID=0 and N_MenuID=@nMenuID and N_Type=@nTypeID";
            DefaultDetailSql = "select * from vw_Gen_TableViewDetails where N_CompanyID=-1 and N_MenuID=@nMenuID and N_Type=@nTypeID and N_TableViewID=(select max(N_TableViewID) as N_TableViewID from Gen_TableView where N_CompanyID=-1 and N_UserID=0 and N_MenuID=@nMenuID and N_Type=@nTypeID)";

            Mastersql = "select b_AttachementSearch,n_MenuID,n_Type,n_UserID,n_CompanyID,n_TableViewID from Gen_TableView where N_CompanyID=@nCompanyID and N_UserID=@nUserID and N_MenuID=@nMenuID and N_Type=@nTypeID";
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


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    string masterSql = "SELECT " + nCompanyID + " as N_CompanyID,(select max(N_TableViewID)+1 from Gen_TableView ) as X_TableViewCode,0 as N_TableViewID, N_MenuID, X_TitleLanControlNo, X_MenuLanControlNo, X_ActionBtnLanControlNo, " +
                    " B_IsDefault, X_DataSource, X_DefaultCriteria, X_BranchCriteria, X_LocationCriteria, X_PKey, X_PCode," + nUserID + " as N_UserID, B_SearchEnabled, B_AttachementSearch, " +
                    " X_TotalField, N_Type, N_Order, X_DefaultSortField FROM Gen_TableView where N_CompanyID=-1 and N_MenuID=" + nMenuID + " and N_Type=" + nTypeID + " and N_UserID=0";
                    MasterTable = dLayer.ExecuteDataTable(masterSql, Params, connection,transaction);
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


    }
}