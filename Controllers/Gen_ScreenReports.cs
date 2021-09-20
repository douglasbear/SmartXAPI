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
        public ActionResult TaskList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy, int nTableViewID, int nMenuID)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
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
            // if (byUser == true)
            // {
            //     Criteria = " and N_AssigneeID=@nUserID ";
            // }

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_TaskSummery like '%" + xSearchkey + "%' OR X_TaskDescription like '%" + xSearchkey + "%' OR X_Assignee like '%" + xSearchkey + "%' OR X_Submitter like '%" + xSearchkey + "%' OR X_ClosedUser like '%" + xSearchkey + "%'  OR X_ProjectName like '%" + xSearchkey + "%' )";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_TaskID desc";
            else
                xSortBy = " order by " + xSortBy;




            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    string tableViewSql = "select X_HiddenFields,X_Fields,X_DataSource,X_DefaultCriteria,X_PKey from Sec_TableViewComponents where N_MenuID=@nMenuID and N_TableViewID=@nTableViewID";
                    SortedList TviewParams = new SortedList();
                    TviewParams.Add("@nMenuID", nMenuID);
                    TviewParams.Add("@nTableViewID", nTableViewID);
                    DataTable tableViewResult = dLayer.ExecuteDataTable(tableViewSql, TviewParams, connection);

                    if (tableViewResult.Rows.Count > 0)
                    {
                        DataRow dRow = tableViewResult.Rows[0];
                        string Table = dRow["X_DataSource"].ToString();
                        string HiddenFields = dRow["X_HiddenFields"].ToString();
                        string DefaultCriteria = dRow["X_DefaultCriteria"].ToString();
                        Header = dRow["X_Fields"].ToString();
                        string PKey = dRow["X_PKey"].ToString();
                        string UserPattern = myFunctions.GetUserPattern(User);
                        string Pattern = "";
                        if (UserPattern != "")
                        {
                            Pattern = " and Left(X_Pattern,Len(" + UserPattern + "))=" + UserPattern;
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


                        connection.Open();
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


    }
}