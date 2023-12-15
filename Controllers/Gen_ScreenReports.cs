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
    [Route("screenReports")]
    [ApiController]
    public class Gen_ScreenReports : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;

        public Gen_ScreenReports(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1056;
        }



        [HttpGet("reportsList")]
        public ActionResult TaskList(int nMenuID, int nLangID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    String tableViewSql = ""
                    + "SELECT Sec_TableViewComponents.N_TableViewID, Sec_TableViewComponents.N_MenuID, Sec_TableViewComponents.b_IsDefault, "
                    + "Sec_TableViewComponents.X_PKey,Sec_TableViewComponents.X_ActionKey, En_Lan_MultiLingual.X_WText AS En_WText, Ar_Lan_MultiLingual.X_WText AS Ar_WText FROM Sec_TableViewComponents LEFT OUTER JOIN "
                    + "Lan_MultiLingual AS En_Lan_MultiLingual ON Sec_TableViewComponents.N_MenuID = En_Lan_MultiLingual.N_FormID AND Sec_TableViewComponents.X_WLanControllNo = En_Lan_MultiLingual.X_WControlName AND En_Lan_MultiLingual.N_LanguageId =1 LEFT OUTER JOIN "
                    + "Lan_MultiLingual AS Ar_Lan_MultiLingual ON Sec_TableViewComponents.X_WLanControllNo = Ar_Lan_MultiLingual.X_WControlName AND Sec_TableViewComponents.N_MenuID = Ar_Lan_MultiLingual.N_FormID AND Ar_Lan_MultiLingual.N_LanguageId =2 where Sec_TableViewComponents.N_MenuID=@nMenuID";
                    SortedList TviewParams = new SortedList();
                    TviewParams.Add("@nMenuID", nMenuID);
                    TviewParams.Add("@nLangID", nLangID);
                    TviewParams.Add("@nTableViewID", 0);

                    DataTable tableViewResult = dLayer.ExecuteDataTable(tableViewSql, TviewParams, connection);

                    tableViewResult = myFunctions.AddNewColumnToDataTable(tableViewResult, "columns", typeof(DataTable), null);
                    tableViewResult.AcceptChanges();

                    foreach (DataRow dRow in tableViewResult.Rows)
                    {
                        TviewParams["@nTableViewID"] = dRow["N_TableViewID"].ToString();
                        string tableHeaderSql = ""
                        + "SELECT  LOWER(LEFT(Sec_TableViewComponentDetails.X_DataField,1))+SUBSTRING(Sec_TableViewComponentDetails.X_DataField,2,LEN(Sec_TableViewComponentDetails.X_DataField)) as dataField, Lan_MultiLingual.X_WText as text,Sec_TableViewComponentDetails.B_Sort as sort, Sec_TableViewComponentDetails.B_IsLink as editLink, "
                        + "                         Sec_TableViewComponentDetails.B_IsHidden as hidden, Sec_TableViewComponentDetails.N_Width as width, Sec_TableViewComponentDetails.B_IsAction as action,Sec_TableViewComponentDetails.X_HeaderAlign as headerAlign, "
                        + "	Sec_TableViewComponentDetails.X_Align as align,Sec_TableViewComponentDetails.X_Formatter as formatter "
                        + "FROM            Sec_TableViewComponentDetails LEFT OUTER JOIN "
                        + "                         Sec_TableViewComponents ON Sec_TableViewComponentDetails.N_TableViewID = Sec_TableViewComponents.N_TableViewID LEFT OUTER JOIN "
                        + "                         Lan_MultiLingual ON Sec_TableViewComponentDetails.X_WLanControllNo = Lan_MultiLingual.X_WControlName AND Sec_TableViewComponents.N_MenuID = Lan_MultiLingual.N_FormID "
                        + "						 where Lan_MultiLingual.N_LanguageID=@nLangID and Lan_MultiLingual.N_LanguageID=@nLangID Sec_TableViewComponentDetails.N_TableViewID=@nTableViewID order by Sec_TableViewComponentDetails.N_Order";

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


        [HttpGet("list")]
        public ActionResult TaskList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy, int nTableViewID, int nMenuID, int n_UserID, DateTime d_Date, bool isMyTeam,string xScreen)
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
            string Criteria3 = "";
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
                            if(patternCode!=null)
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

                        if(xScreen=="crmMyTask"){
                          Criteria3=" and ISNULL(n_opportunityID,0)<>0";
                        }



                        if (Count == 0)
                            sqlCommandText = "select top(" + nSizeperpage + ") * from " + Table + " where " + DefaultCriteria + Criteria3 + Pattern + " " + Searchkey + Criteria + Criteria2 + xSortBy;
                        else
                            sqlCommandText = "select top(" + nSizeperpage + ") * from " + Table + " where " + DefaultCriteria + Criteria3 + Pattern + " " + Searchkey + Criteria + Criteria2 + " and " + PKey + " not in (select top(" + Count + ") N_TaskID from " + Table + " where " + DefaultCriteria + Criteria3 +" " + Criteria + Criteria2 + xSortBy + " ) " + xSortBy;

                        SortedList Params = new SortedList();
                        if (DefaultCriteria.Contains("@cVal"))
                            Params.Add("@cVal", nCompanyId);
                        if (DefaultCriteria.Contains("@uVal"))
                            Params.Add("@uVal", nUserID);
                        if (DefaultCriteria.Contains("@dVal"))
                            Params.Add("@dVal", d_Date);



                        dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                        sqlCommandCount = "select count(1) as N_Count  from " + Table + " where " + DefaultCriteria + Criteria3 + Pattern;
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
        public ActionResult GetTableViewDetails(int nMenuID, int nUserID,int nLangID)
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