using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Net.Http;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using zatca.einvoicing;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Net.Cache;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Threading;
using ZatcaIntegrationSDK;
using ZatcaIntegrationSDK.APIHelper;
using ZatcaIntegrationSDK.BLL;
using ZatcaIntegrationSDK.HelperContracts;
using ZXing;
using ZXing.Common;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("report")]
    [ApiController]
    public class Reports : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private string connectionString;
        private readonly string connectionStringClinet;
        private readonly string reportApi;
        private readonly string TempFilesPath;
        private readonly string reportLocation;
        private readonly string AppURL;
        private readonly IWebHostEnvironment env;
        string RPTLocation = "";
        string ReportName = "";
        string FileName = "";
        string critiria = "";
        string TableName = "";
        string QRurl = "";
        string FormName = "";

        string Xmlpath="";
        //Zatca Phase 2 START
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal TotalPriceAfterDiscount { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal VatValue { get; set; }
        public decimal TotalWithVat { get; set; }
        private Mode mode = Mode.developer;
        private class InvoiceItems
        {
            public string ProductName { get; set; }
            public decimal ProductPrice { get; set; }
            public decimal ProductQuantity { get; set; }
            public decimal TotalPrice { get; set; }
            public decimal DiscountValue { get; set; }
            public decimal TotalPriceAfterDiscount { get; set; }
            public decimal VatPercentage { get; set; }
            public decimal VatValue { get; set; }
            public decimal TotalWithVat { get; set; }
        }
        List<InvoiceItems> invlines;
        // private string X_CompanyField = "", X_YearField = "", X_BranchField="", X_UserField="",X_DefReportFile = "", X_GridPrevVal = "", X_SelectionFormula = "", X_ProcName = "", X_ProcParameter = "", X_ReprtTitle = "",X_Operator="";
        public Reports(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IWebHostEnvironment envn, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            connectionStringClinet = conf.GetConnectionString("OlivoClientConnection");
            reportApi = conf.GetConnectionString("ReportAPI");
            TempFilesPath = conf.GetConnectionString("TempFilesPath");
            reportLocation = conf.GetConnectionString("ReportLocation");
            AppURL = conf.GetConnectionString("AppURL");
            env = envn;
        }
        [HttpGet("list")]
        public ActionResult GetReportList(int? nMenuId, int? nLangId, int? nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select N_CompanyID,N_MenuID,X_MenuName,X_Caption,N_ParentMenuID,N_Order,N_HasChild ,B_Visible,B_Edit,B_Delete,B_Save,B_View,X_ShortcutKey,X_CaptionAr,X_FormNameWithTag,N_IsStartup,B_Show,B_ShowOnline,X_RouteName,B_WShow,X_Text from ("
            + "Select vwUserMenus.N_CompanyID, vwUserMenus.N_MenuID, vwUserMenus.X_MenuName, vwUserMenus.X_Caption, vwUserMenus.N_ParentMenuID, vwUserMenus.N_Order, vwUserMenus.N_HasChild ,CAST(MAX(1 * vwUserMenus.B_Visible) AS BIT) as B_Visible, CAST(MAX(1 * vwUserMenus.B_Edit) AS BIT) as B_Edit, CAST(MAX(1 * vwUserMenus.B_Delete) AS BIT) as B_Delete,CAST(MAX(1 * vwUserMenus.B_Save) AS BIT) as B_Save, CAST(MAX(1 * vwUserMenus.B_View) AS BIT) as B_View, vwUserMenus.X_ShortcutKey, vwUserMenus.X_CaptionAr, vwUserMenus.X_FormNameWithTag, vwUserMenus.N_IsStartup, vwUserMenus.B_Show, vwUserMenus.B_ShowOnline, vwUserMenus.X_RouteName, vwUserMenus.B_WShow,Lan_MultiLingual.X_Text from vwUserMenus Inner Join Sec_UserPrevileges On vwUserMenus.N_MenuID=Sec_UserPrevileges.N_MenuID And Sec_UserPrevileges.N_UserCategoryID = vwUserMenus.N_UserCategoryID And  Sec_UserPrevileges.N_UserCategoryID in ( " + myFunctions.GetUserCategoryList(User) + " ) and Sec_UserPrevileges.B_Visible=1 and vwUserMenus.B_Show=1 inner join Lan_MultiLingual on vwUserMenus.N_MenuID=Lan_MultiLingual.N_FormID and Lan_MultiLingual.N_LanguageId=@nLangId and X_ControlNo ='0' Where LOWER(vwUserMenus.X_Caption) <>'seperator' and vwUserMenus.N_ParentMenuID=@nMenuId and vwUserMenus.N_CountryID is null group by vwUserMenus.N_CompanyID, vwUserMenus.N_MenuID, vwUserMenus.X_MenuName, vwUserMenus.X_Caption, vwUserMenus.N_ParentMenuID, vwUserMenus.N_Order, vwUserMenus.N_HasChild, vwUserMenus.X_ShortcutKey, vwUserMenus.X_CaptionAr, vwUserMenus.X_FormNameWithTag, vwUserMenus.N_IsStartup, vwUserMenus.B_Show, vwUserMenus.B_ShowOnline, vwUserMenus.X_RouteName, vwUserMenus.B_WShow,Lan_MultiLingual.X_Text,vwUserMenus.N_CountryID "
            + " union all "
            + "Select vwUserMenus.N_CompanyID, vwUserMenus.N_MenuID, vwUserMenus.X_MenuName, vwUserMenus.X_Caption, vwUserMenus.N_ParentMenuID, vwUserMenus.N_Order, vwUserMenus.N_HasChild ,CAST(MAX(1 * vwUserMenus.B_Visible) AS BIT) as B_Visible, CAST(MAX(1 * vwUserMenus.B_Edit) AS BIT) as B_Edit, CAST(MAX(1 * vwUserMenus.B_Delete) AS BIT) as B_Delete,CAST(MAX(1 * vwUserMenus.B_Save) AS BIT) as B_Save, CAST(MAX(1 * vwUserMenus.B_View) AS BIT) as B_View, vwUserMenus.X_ShortcutKey, vwUserMenus.X_CaptionAr, vwUserMenus.X_FormNameWithTag, vwUserMenus.N_IsStartup, vwUserMenus.B_Show, vwUserMenus.B_ShowOnline, vwUserMenus.X_RouteName, vwUserMenus.B_WShow,Lan_MultiLingual.X_Text from vwUserMenus Inner Join Sec_UserPrevileges On vwUserMenus.N_MenuID=Sec_UserPrevileges.N_MenuID And Sec_UserPrevileges.N_UserCategoryID = vwUserMenus.N_UserCategoryID And  Sec_UserPrevileges.N_UserCategoryID in ( " + myFunctions.GetUserCategoryList(User) + " ) and Sec_UserPrevileges.B_Visible=1 and vwUserMenus.B_Show=1 inner join Lan_MultiLingual on vwUserMenus.N_MenuID=Lan_MultiLingual.N_FormID and Lan_MultiLingual.N_LanguageId=@nLangId and X_ControlNo ='0' Where LOWER(vwUserMenus.X_Caption) <>'seperator' and vwUserMenus.N_ParentMenuID=@nMenuId and isnull(vwUserMenus.N_CountryID,0)=@nCountryID group by vwUserMenus.N_CompanyID, vwUserMenus.N_MenuID, vwUserMenus.X_MenuName, vwUserMenus.X_Caption, vwUserMenus.N_ParentMenuID, vwUserMenus.N_Order, vwUserMenus.N_HasChild, vwUserMenus.X_ShortcutKey, vwUserMenus.X_CaptionAr, vwUserMenus.X_FormNameWithTag, vwUserMenus.N_IsStartup, vwUserMenus.B_Show, vwUserMenus.B_ShowOnline, vwUserMenus.X_RouteName, vwUserMenus.B_WShow,Lan_MultiLingual.X_Text,vwUserMenus.N_CountryID "
            + ") as Menus order by N_Order";
            
            if (nMenuId == 1680)
                sqlCommandText = "select N_CompanyID,N_MenuID,X_MenuName,X_Caption,N_ParentMenuID,N_Order,N_HasChild ,B_Visible,B_Edit,B_Delete,B_Save,B_View,X_ShortcutKey,X_CaptionAr,X_FormNameWithTag,N_IsStartup,B_Show,B_ShowOnline,X_RouteName,B_WShow,X_Text from ("
                + "Select vwUserMenus.N_CompanyID, vwUserMenus.N_MenuID, vwUserMenus.X_MenuName, vwUserMenus.X_Caption, vwUserMenus.N_ParentMenuID, vwUserMenus.N_Order, vwUserMenus.N_HasChild ,CAST(MAX(1 * vwUserMenus.B_Visible) AS BIT) as B_Visible, CAST(MAX(1 * vwUserMenus.B_Edit) AS BIT) as B_Edit, CAST(MAX(1 * vwUserMenus.B_Delete) AS BIT) as B_Delete,CAST(MAX(1 * vwUserMenus.B_Save) AS BIT) as B_Save, CAST(MAX(1 * vwUserMenus.B_View) AS BIT) as B_View, vwUserMenus.X_ShortcutKey, vwUserMenus.X_CaptionAr, vwUserMenus.X_FormNameWithTag, vwUserMenus.N_IsStartup, vwUserMenus.B_Show, vwUserMenus.B_ShowOnline, vwUserMenus.X_RouteName, vwUserMenus.B_WShow,Lan_MultiLingual.X_Text from vwUserMenus Inner Join Sec_UserPrevileges On vwUserMenus.N_MenuID=Sec_UserPrevileges.N_MenuID And Sec_UserPrevileges.N_UserCategoryID = vwUserMenus.N_UserCategoryID And  Sec_UserPrevileges.N_UserCategoryID in ( " + myFunctions.GetUserCategoryList(User) + " ) and Sec_UserPrevileges.B_Visible=1 and vwUserMenus.B_Show=1 inner join Lan_MultiLingual on vwUserMenus.N_MenuID=Lan_MultiLingual.N_FormID and Lan_MultiLingual.N_LanguageId=@nLangId and X_ControlNo ='0' Where LOWER(vwUserMenus.X_Caption) <>'seperator' and vwUserMenus.N_ParentMenuID=@nMenuId and vwUserMenus.N_CountryID is null group by vwUserMenus.N_CompanyID, vwUserMenus.N_MenuID, vwUserMenus.X_MenuName, vwUserMenus.X_Caption, vwUserMenus.N_ParentMenuID, vwUserMenus.N_Order, vwUserMenus.N_HasChild, vwUserMenus.X_ShortcutKey, vwUserMenus.X_CaptionAr, vwUserMenus.X_FormNameWithTag, vwUserMenus.N_IsStartup, vwUserMenus.B_Show, vwUserMenus.B_ShowOnline, vwUserMenus.X_RouteName, vwUserMenus.B_WShow,Lan_MultiLingual.X_Text,vwUserMenus.N_CountryID "
               + ") as Menus order by N_Order";

            Params.Add("@nMenuId", nMenuId == 0 ? 318 : nMenuId);
            Params.Add("@nLangId", nLangId);
            Params.Add("@nUserCatID", myFunctions.GetUserCategoryList(User));
            Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
            Params.Add("@nBranchID", nBranchID);
            int CountryID = 0;

            try
            {
                if (nMenuId == 1680)
                    connectionString = connectionStringClinet;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlCountryID = "select n_CountryID from Acc_Company where N_CompanyID=@nCompanyID";
                    if (nMenuId != 1680)
                        CountryID = myFunctions.getIntVAL(dLayer.ExecuteScalar(sqlCountryID, Params, connection).ToString());
                    Params.Add("@nCountryID", CountryID);

                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    DataTable dt1 = new DataTable();
                    object B_Consolidated = "false";
                    if (nMenuId != 1680)
                        B_Consolidated = dLayer.ExecuteScalar("select isnull(B_DefaultBranch,0) from Acc_BranchMaster where N_CompanyID=@nCompanyID and N_branchID=@nBranchID", Params, connection);

                    string sqlCommandText1 = "";
                    if (myFunctions.getBoolVAL(B_Consolidated.ToString()))
                        sqlCommandText1 = "select n_CompID,n_LanguageId,n_MenuID,x_CompType,x_FieldList,x_FieldType,x_Text,X_FieldtoReturn,X_DefVal1,X_DefVal2,X_Operator,N_LinkCompID,X_LinkField,B_Range,isnull(B_Consolidated,0) as B_Consolidated,b_enablemultiselect from vw_WebReportMenus where N_LanguageId=@nLangId group by n_CompID,n_LanguageId,n_MenuID,x_CompType,x_FieldList,x_FieldType,x_Text,X_FieldtoReturn,X_DefVal1,X_DefVal2,X_Operator,N_ListOrder,N_LinkCompID,X_LinkField,B_Range,B_Consolidated,b_enablemultiselect order by N_ListOrder";
                    else
                        sqlCommandText1 = "select n_CompID,n_LanguageId,n_MenuID,x_CompType,x_FieldList,x_FieldType,x_Text,X_FieldtoReturn,X_DefVal1,X_DefVal2,X_Operator,N_LinkCompID,X_LinkField,B_Range,isnull(B_Consolidated,0) as B_Consolidated,b_enablemultiselect from vw_WebReportMenus where N_LanguageId=@nLangId and B_Consolidated<>1 group by n_CompID,n_LanguageId,n_MenuID,x_CompType,x_FieldList,x_FieldType,x_Text,X_FieldtoReturn,X_DefVal1,X_DefVal2,X_Operator,N_ListOrder,N_LinkCompID,X_LinkField,B_Range,B_Consolidated,b_enablemultiselect order by N_ListOrder";
                    dt1 = dLayer.ExecuteDataTable(sqlCommandText1, Params, connection);

                    dt.Columns.Add("ChildMenus", typeof(DataTable));
                    dt.Columns.Add("Filter", typeof(DataTable));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataTable ChildMenus = new DataTable();
                        DataTable Filter = new DataTable();
                        string N_MenuID = dt.Rows[i]["N_MenuID"].ToString();
                        try
                        {
                            DataRow[] dr = dt1.Select("N_MenuID = " + N_MenuID + " and x_FieldType='RadioButton'");
                            DataRow[] dr1 = dt1.Select("N_MenuID = " + N_MenuID + " and x_FieldType<>'RadioButton'");
                            if (dr != null)
                            {
                                ChildMenus = dr.CopyToDataTable();
                                dt.Rows[i]["ChildMenus"] = ChildMenus;
                            }
                            if (dr1 != null)
                            {
                                Filter = dr1.CopyToDataTable();
                                dt.Rows[i]["Filter"] = Filter;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else { return Ok(_api.Success(dt)); }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        [HttpGet("dynamiclist")]
        public ActionResult GetDynamicList(int nMenuId, int nCompId, int nLangId, string cval, string bval, string fval, string qry, int nMainMenuID)
        {
            DataTable dt = new DataTable();
            DataTable outTable = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select TOP 1 X_TableName,X_FieldList,X_Criteria from vw_WebReportMenus where N_MenuID=@p1 and N_LanguageId=@p2 and N_CompID=@p3 and X_CompType=@p4";
            //             string sqlCommandText = "Select Sec_ReportsComponents.*, Lan_Multilingual.X_Text from Sec_ReportsComponents Left Outer Join Lan_Multilingual on " +
            //  " Sec_ReportsComponents.N_MenuID = Lan_Multilingual.N_FormID and  Sec_ReportsComponents.X_LangControlNo = Lan_Multilingual.X_ControlNo and  " +
            // " Lan_Multilingual.N_LanguageID=@p2  Where Sec_ReportsComponents.N_MenuID=@p1 AND Sec_ReportsComponents.B_Active =1 and N_CompID=@p3 and X_CompType=@p4 ";

            Params.Add("@p1", nMenuId);
            Params.Add("@p2", nLangId);
            Params.Add("@p3", nCompId);
            Params.Add("@p4", "ListControl");

            try
            {
                if (nMainMenuID == 1680)
                    connectionString = connectionStringClinet;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow QueryString = dt.Rows[0];
                        SortedList ListSqlParams = new SortedList();
                        string fields = QueryString["X_FieldList"].ToString();
                        string table = QueryString["X_TableName"].ToString();
                        // if(table=="Acc_VoucherDetails"){
                        //     string a=table;
                        // }
                        string Criteria = QueryString["X_Criteria"].ToString();
                        if (Criteria != "")
                            Criteria = " Where " + QueryString["X_Criteria"].ToString().Replace("'CVal'", "@CVal ").Replace("'BVal'", "@BVal ").Replace("'FVal'", "@FVal ");

                        if (qry != null)
                        {
                            if (Criteria != "")
                            {
                                Criteria = Criteria + " and " + qry;
                            }
                            else
                            {
                                Criteria = " Where " + qry;
                            }
                        }
                        ListSqlParams.Add("@BVal", bval);
                        ListSqlParams.Add("@CVal", cval);
                        ListSqlParams.Add("@FVal", fval);
                        string ListSql = "select " + fields + " from " + table + " " + Criteria + " group by " + fields + " order by " + fields;

                        outTable = dLayer.ExecuteDataTable(ListSql, ListSqlParams, connection);
                    }


                }
                if (outTable.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    //Dictionary<string,Dictionary<string,DataTable>> Menu = new Dictionary<string,Dictionary<string,DataTable>>();
                    outTable = _api.Format(outTable);
                    //Dictionary<string,DataTable> Component = new Dictionary<string,DataTable>();
                    SortedList Component = new SortedList();
                    Component.Add(nCompId.ToString(), outTable);
                    //Menu.Add(nMenuId.ToString(),Component);

                    return Ok(_api.Success(Component));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        [HttpGet("getreport")]
        public IActionResult GetModuleReports(string reportName, string critiria)
        {
            try
            {
                var handler1 = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                };
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();

                    var client = new HttpClient(handler1);
                    var random = RandomString();
                    var dbName = connection.Database;
                    //string URL = reportApi + "api/report?reportName=" + reportName + "&critiria=" + critiria + "&path=" + this.TempFilesPath + "&reportLocation=" + reportLocation + "&dbval=" + dbName + "&random=" + random + "&x_comments=&x_Reporttitle=&extention=pdf";
                    string URL = reportApi + "api/report?reportName=" + reportName + "&critiria=" + critiria + "&path=" + this.TempFilesPath + "&reportLocation=" + reportLocation + "&dbval=" + dbName + "&random=" + random + "&x_comments=&x_Reporttitle=&extention=pdf&N_FormID=0&QRUrl=&N_PkeyID=0&partyName=&docNumber=&formName=&xml="+Xmlpath;;
                    var path = client.GetAsync(URL);
                    path.Wait();
                    return Ok(_api.Success(new SortedList() { { "FileName", reportName.Trim() + random + ".pdf" } }));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        private bool LoadReportDetails(int nFnYearID, int nFormID, int nPkeyID, int nPreview, string xRptname, int nLangId)
        {
            SortedList QueryParams = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            QueryParams.Add("@nCompanyId", nCompanyId);
            QueryParams.Add("@nFnYearID", nFnYearID);
            QueryParams.Add("@nFormID", nFormID);
            RPTLocation = "";
            critiria = "";
            TableName = "";
            ReportName = "";
            //int N_UserCategoryID=myFunctions.GetUserCategory(User);
            bool b_Custom = false;
            string xUserCategoryList = myFunctions.GetUserCategoryList(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();
                    object ObjTaxType = dLayer.ExecuteScalar("SELECT Acc_TaxType.X_RepPathCaption FROM Acc_TaxType LEFT OUTER JOIN Acc_FnYear ON Acc_TaxType.N_TypeID = Acc_FnYear.N_TaxType where Acc_FnYear.N_CompanyID=@nCompanyId and Acc_FnYear.N_FnYearID=@nFnYearID", QueryParams, connection, transaction);
                    if (ObjTaxType == null)
                        ObjTaxType = "";
                    if (ObjTaxType.ToString() == "")
                        ObjTaxType = "none";
                    string TaxType = ObjTaxType.ToString();

                    object ObjPath = dLayer.ExecuteScalar("SELECT X_RptFolder FROM Gen_PrintTemplates WHERE N_CompanyID =@nCompanyId and N_FormID=@nFormID", QueryParams, connection, transaction);

                    if (ObjPath != null)
                    {
                        RPTLocation = reportLocation.Remove(reportLocation.Length - 2, 2) + nLangId + "/";
                        if (ObjPath.ToString() != "")
                            RPTLocation = RPTLocation + "printing/" + ObjPath + "/" + TaxType + "/";
                        else
                            RPTLocation = RPTLocation + "printing/";
                    }

                    object Templatecritiria = dLayer.ExecuteScalar("SELECT X_PkeyField FROM Gen_PrintTemplates WHERE N_CompanyID =@nCompanyId and N_FormID=@nFormID", QueryParams, connection, transaction);
                    TableName = Templatecritiria.ToString().Substring(0, Templatecritiria.ToString().IndexOf(".")).Trim();
                    object Custom = dLayer.ExecuteScalar("SELECT isnull(b_Custom,0) FROM Gen_PrintTemplates WHERE N_CompanyID =@nCompanyId and N_FormID=@nFormID and N_UsercategoryID in (" + xUserCategoryList + ")", QueryParams, connection, transaction);
                    int N_Custom = myFunctions.getIntVAL(Custom.ToString());
                    object ObjReportName = dLayer.ExecuteScalar("SELECT X_RptName FROM Gen_PrintTemplates WHERE N_CompanyID =@nCompanyId and N_FormID=@nFormID and N_UsercategoryID in (" + xUserCategoryList + ")", QueryParams, connection, transaction);
                    object objFormName = dLayer.ExecuteScalar("SELECT X_FormName FROM Gen_PrintTemplates WHERE N_CompanyID =@nCompanyId and N_FormID=@nFormID and N_UsercategoryID in (" + xUserCategoryList + ")", QueryParams, connection, transaction);
                    FormName = objFormName.ToString();
                    // object ObjFileName = dLayer.ExecuteScalar("SELECT X_FileName FROM Gen_PrintTemplates WHERE N_CompanyID =@nCompanyId and N_FormID=@nFormID and N_UsercategoryID in (" + xUserCategoryList + ")", QueryParams, connection, transaction);
                    // FileName=ObjFileName.ToString();
                    if (N_Custom == 1)
                    {

                        RPTLocation = RPTLocation + "custom/";
                        ObjReportName = (ObjReportName.ToString().Remove(ObjReportName.ToString().Length - 4)).Trim();
                        ObjReportName = ObjReportName + "_" + myFunctions.GetClientID(User) + "_" + myFunctions.GetCompanyID(User) + "_" + myFunctions.GetCompanyName(User) + ".rpt";
                    }
                    ReportName = ObjReportName.ToString();
                    ReportName = ReportName.Remove(ReportName.Length - 4);

                    if (nPreview == 1)
                    {
                        ReportName = xRptname;
                        if (ReportName.Contains(".rpt"))
                        {
                            ReportName = ReportName.Remove(ReportName.Length - 4);
                        }
                        object pkeyID = dLayer.ExecuteScalar("SELECT max(" + Templatecritiria + ") FROM " + TableName + " WHERE N_CompanyID =@nCompanyId", QueryParams, connection, transaction);
                        if (pkeyID != null)
                            nPkeyID = myFunctions.getIntVAL(pkeyID.ToString());
                    }

                    critiria = "{" + Templatecritiria + "}=" + nPkeyID;
                    object Othercritiria = dLayer.ExecuteScalar("SELECT X_Criteria FROM Gen_PrintTemplates WHERE N_CompanyID =@nCompanyId and N_FormID=@nFormID", QueryParams, connection, transaction);

                    if (Othercritiria != null)
                    {
                        if (Othercritiria.ToString() != "")
                            critiria = critiria + " and " + Othercritiria.ToString();

                    }


                    if (nFormID == 64 || nFormID == 894 || nFormID == 372 || nFormID == 55 || nFormID == 504 || nFormID == 1601||nFormID == 1651)
                    {
                        //QR Code Generate For Invoice
                        object Total = "";
                        object TaxAmount = "";
                        object VatNumber = dLayer.ExecuteScalar("select x_taxregistrationNo from acc_company where N_CompanyID=@nCompanyId", QueryParams, connection, transaction);
                        object SalesDate = "";
                        if (nFormID == 64 || nFormID == 894 || nFormID == 372 || nFormID == 1601 || nFormID == 1651)
                        {
                            Total = dLayer.ExecuteScalar("select n_BillAmt + ISNULL(N_taxamtF,0) + isnull(N_Cessamtf,0) from inv_sales where N_CompanyID=@nCompanyId and N_SalesID=" + nPkeyID, QueryParams, connection, transaction);
                            TaxAmount = dLayer.ExecuteScalar("select ISNULL(N_taxamtF,0) from inv_sales where N_CompanyID=@nCompanyId and N_SalesID=" + nPkeyID, QueryParams, connection, transaction);
                            SalesDate = dLayer.ExecuteScalar("select D_SalesDate from inv_sales where N_CompanyID=@nCompanyId and N_SalesID=" + nPkeyID, QueryParams, connection, transaction);
                        }
                        else if (nFormID == 504)
                        {
                            Total = dLayer.ExecuteScalar("select N_AmountF+isnull(N_TaxAmtF,0) from Inv_BalanceAdjustmentMaster where N_CompanyID=@nCompanyId and N_AdjustmentId=" + nPkeyID, QueryParams, connection, transaction);
                            TaxAmount = dLayer.ExecuteScalar("select N_TaxAmtF from Inv_BalanceAdjustmentMaster where N_CompanyID=@nCompanyId and N_AdjustmentId=" + nPkeyID, QueryParams, connection, transaction);
                            SalesDate = dLayer.ExecuteScalar("select D_AdjustmentDate from Inv_BalanceAdjustmentMaster where N_CompanyID=@nCompanyId and N_AdjustmentId=" + nPkeyID, QueryParams, connection, transaction);
                        }
                        else if (nFormID == 55)
                        {
                            Total = dLayer.ExecuteScalar("select N_TotalReturnAmountF+isnull(N_TaxAmtF,0) from Inv_SalesReturnMaster where N_CompanyID=@nCompanyId and N_DebitNoteId=" + nPkeyID, QueryParams, connection, transaction);
                            TaxAmount = dLayer.ExecuteScalar("select isnull(N_TaxAmtF,0) as N_TaxAmtF from Inv_SalesReturnMaster where N_CompanyID=@nCompanyId and N_DebitNoteId=" + nPkeyID, QueryParams, connection, transaction);
                            SalesDate = dLayer.ExecuteScalar("select D_ReturnDate from Inv_SalesReturnMaster where N_CompanyID=@nCompanyId and N_DebitNoteId=" + nPkeyID, QueryParams, connection, transaction);
                        }
                        else if (nFormID == 1492)
                        {
                            Total = dLayer.ExecuteScalar("select SUM(N_AmountPaidF) from vw_InvCustomerPayment_rpt where N_CompanyID=@nCompanyId and N_PayReceiptId=" + nPkeyID, QueryParams, connection, transaction);
                            TaxAmount = dLayer.ExecuteScalar("select isnull(N_TaxAmt1F,0) as N_TaxAmtF from Inv_PayReceipt where N_CompanyID=@nCompanyId and N_PayReceiptId=" + nPkeyID, QueryParams, connection, transaction);
                            SalesDate = dLayer.ExecuteScalar("select D_Date from Inv_PayReceipt where N_CompanyID=@nCompanyId and N_PayReceiptId=" + nPkeyID, QueryParams, connection, transaction);
                            Total = myFunctions.getVAL(Total.ToString()) + myFunctions.getVAL(TaxAmount.ToString());
                        }
                        DateTime dt = DateTime.Parse(SalesDate.ToString());
                        string Amount = Convert.ToDecimal(Total).ToString("0.00");
                        string VatAmount = Convert.ToDecimal(TaxAmount.ToString()).ToString("0.00");
                        string Company = myFunctions.GetCompanyName(User);
                        TLVCls tlv = new TLVCls(Company, VatNumber.ToString(), dt, Convert.ToDouble(Amount), Convert.ToDouble(VatAmount));
                        var plainTextBytes = tlv.ToBase64();
                        QRurl = string.Format(plainTextBytes);

                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                return false;

            }

        }
        private string StringToHex(string hexstring)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char t in hexstring)
            {
                //Note: X for upper, x for lower case letters
                sb.Append(Convert.ToInt32(t).ToString("x"));
            }
            return sb.ToString();
        }

        [HttpGet("getscreenprint")]
        public IActionResult GetModulePrint(int nFormID, int nPkeyID, int nFnYearID, int nPreview, string xrptname, string docNumber, string partyName, bool printSave, bool sendWtsapMessage, int n_LanguageID)
        {
            SortedList QueryParams = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string xUserCategoryList = myFunctions.GetUserCategoryList(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();
                    QueryParams.Add("@p1", nCompanyId);
                    QueryParams.Add("@p2", nFormID);
                    QueryParams.Add("@p3", nFnYearID);
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                    };
                    if (nPreview != 1)
                    {
                        object printAfterSave = dLayer.ExecuteScalar("select B_PrintAfterSave from Gen_PrintTemplates where N_CompanyID= " + nCompanyId + " and N_FormID=" + nFormID + " and  N_UsercategoryID in (" + xUserCategoryList + ")", connection, transaction);
                        if (printAfterSave != null && printSave)
                        {
                            if (!myFunctions.getBoolVAL(printAfterSave.ToString()))
                            {
                                return Ok(_api.Error(User, "No-Print"));
                            }
                        }

                    }
                    if (LoadReportDetails(nFnYearID, nFormID, nPkeyID, nPreview, xrptname, n_LanguageID))
                    {

                        var client = new HttpClient(handler);

                        var dbName = connection.Database;
                        var random = RandomString();
                        if (TableName != "" && critiria != "")
                        {
                            critiria = critiria + " and {" + TableName + ".N_CompanyID}=" + myFunctions.GetCompanyID(User);
                        }
                        ReportName = ReportName.Replace("&", "");

                        if (nPreview == 2)
                        {
                            string fileToCopy = RPTLocation + ReportName + ".rpt";
                            string destinationFile = this.TempFilesPath + ReportName + ".rpt";
                            string ZipLocation = this.TempFilesPath + ReportName + ".rpt.zip";
                            if (CopyFiles(fileToCopy, destinationFile, ReportName + ".rpt"))
                                return Ok(_api.Success(new SortedList() { { "FileName", ReportName.Trim() + ".rpt.zip" } }));
                        }

                        if (partyName == "" || partyName == null)
                            partyName = "customer";
                        if (docNumber == "" || docNumber == null)
                            docNumber = "DocNo";
                        partyName = partyName.Replace("&", "");
                        partyName = partyName.Replace(":", "");
                        partyName = partyName.ToString().Substring(0, Math.Min(2, partyName.ToString().Length));
                        if (docNumber == null)
                            docNumber = "";
                        docNumber = Regex.Replace(docNumber, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                        if (!Regex.IsMatch(partyName, @"\p{IsArabic}"))
                            partyName = Regex.Replace(partyName, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                        partyName = Regex.Replace(partyName, ":", "", RegexOptions.Compiled);

                        if (docNumber.Contains("/"))
                            docNumber = docNumber.ToString().Substring(0, Math.Min(3, docNumber.ToString().Length));

                        DateTime currentTime;
                        string x_comments = "";

                        object TimezoneID = dLayer.ExecuteScalar("select isnull(n_timezoneid,82) from acc_company where N_CompanyID= " + nCompanyId, connection, transaction);
                        object Timezone = dLayer.ExecuteScalar("select X_ZoneName from Gen_TimeZone where n_timezoneid=" + TimezoneID, connection, transaction);
                        if (Timezone != null && Timezone.ToString() != "")
                        {

                            currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(Timezone.ToString()));
                            x_comments = currentTime.ToString();
                        }
                        if (nFormID == 1406)
                        {
                            object ASNdoc = dLayer.ExecuteScalar("select x_asndocno from vw_Wh_AsnMaster_Disp where n_companyid=" + nCompanyId + " and n_asnid=" + nPkeyID, connection, transaction);
                            CreateBarcode(ASNdoc.ToString());
                            DataTable AsnDetails = dLayer.ExecuteDataTable("select X_Barcode from vw_Wh_Asndetails_disp where n_companyid=" + nCompanyId + " and n_asnid=" + nPkeyID, QueryParams, connection, transaction);
                            foreach (DataRow var in AsnDetails.Rows)
                            {
                                CreateBarcode(var["X_Barcode"].ToString());
                            }
                        }
                        if (nFormID == 1463 || nFormID == 1461)
                        {
                            object PICKList = dLayer.ExecuteScalar("select X_PickListCode from vw_WhPickListMaster where n_companyid=" + nCompanyId + " and N_PickListID=" + nPkeyID, connection, transaction);
                            CreateBarcode(PICKList.ToString());

                        }


                        if (nFormID == 1407)
                        {
                            SqlCommand cmd = new SqlCommand("select i_signature from Wh_GRN where N_GRNID=" + nPkeyID, connection, transaction);
                           try
                           {
                           if ((cmd.ExecuteScalar().ToString()) != "" && cmd.ExecuteScalar().ToString() != "0x" )
                                {
                                byte[] content = (byte[])cmd.ExecuteScalar();
                                MemoryStream stream = new MemoryStream(content);
                                Image Sign = Image.FromStream(stream);

                                using (var b = new Bitmap(Sign.Width, Sign.Height))
                                {
                                    b.SetResolution(Sign.HorizontalResolution, Sign.VerticalResolution);

                                    using (var g = Graphics.FromImage(b))
                                    {
                                        g.Clear(Color.White);
                                        g.DrawImageUnscaled(Sign, 0, 0);
                                    }
                                    b.Save("C://OLIVOSERVER2020/Images/" + nPkeyID + "-wch.png");
                                }
                            }
                            }
                            catch
                            {}
                           try
                           {
                            SqlCommand cmd2 = new SqlCommand("select i_signature2 from Wh_GRN where N_GRNID=" + nPkeyID, connection, transaction);
                            if ((cmd2.ExecuteScalar().ToString()) != "" && cmd2.ExecuteScalar().ToString() != "0x" )
                                {
                                byte[] content = (byte[])cmd2.ExecuteScalar();
                                MemoryStream stream = new MemoryStream(content);
                                Image Sign = Image.FromStream(stream);

                                using (var b = new Bitmap(Sign.Width, Sign.Height))
                                {
                                    b.SetResolution(Sign.HorizontalResolution, Sign.VerticalResolution);

                                    using (var g = Graphics.FromImage(b))
                                    {
                                        g.Clear(Color.White);
                                        g.DrawImageUnscaled(Sign, 0, 0);
                                    }
                                    b.Save("C://OLIVOSERVER2020/Images/" + nPkeyID + "-wrec.png");
                                }
                            }
                            }
                            catch
                            {}
                        }
                        if (nFormID == 1426)
                        {
                            try{
                            SqlCommand cmd = new SqlCommand("select i_signature from Inv_Deliverynote where N_DeliveryNoteID=" + nPkeyID, connection, transaction);
                            if ((cmd.ExecuteScalar().ToString()) != "" && cmd.ExecuteScalar().ToString() != "0x" )
                                {
                                byte[] content = (byte[])cmd.ExecuteScalar();
                                MemoryStream stream = new MemoryStream(content);
                                Image Sign = Image.FromStream(stream);

                                using (var b = new Bitmap(Sign.Width, Sign.Height))
                                {
                                    b.SetResolution(Sign.HorizontalResolution, Sign.VerticalResolution);

                                    using (var g = Graphics.FromImage(b))
                                    {
                                        g.Clear(Color.White);
                                        g.DrawImageUnscaled(Sign, 0, 0);
                                    }
                                    b.Save("C://OLIVOSERVER2020/Images/" + nPkeyID + "-ch.png");
                                }
                            }
                            }
                            catch
                            {}
                             try
                             {
                            SqlCommand cmd1 = new SqlCommand("select i_signature2 from Inv_Deliverynote where N_DeliveryNoteID=" + nPkeyID, connection, transaction);
                            if ((cmd1.ExecuteScalar().ToString()) != "" && cmd1.ExecuteScalar().ToString() != "0x" )
                                {
                                byte[] content = (byte[])cmd1.ExecuteScalar();
                                MemoryStream stream = new MemoryStream(content);
                                Image Sign = Image.FromStream(stream);

                                using (var b = new Bitmap(Sign.Width, Sign.Height))
                                {
                                    b.SetResolution(Sign.HorizontalResolution, Sign.VerticalResolution);

                                    using (var g = Graphics.FromImage(b))
                                    {
                                        g.Clear(Color.White);
                                        g.DrawImageUnscaled(Sign, 0, 0);
                                    }
                                    b.Save("C://OLIVOSERVER2020/Images/" + nPkeyID + "-rc.png");
                                }
                                }
                             }
                             catch
                            {}
                        }
                        // if (nFormID == 1426)
                        // {
                        //     SqlCommand cmd = new SqlCommand("select i_signature from Inv_Deliverynote where N_DeliveryNoteID=" + nPkeyID, connection, transaction);
                        //     object output = cmd.ExecuteScalar();
                        //     if ((cmd.ExecuteScalar().ToString()) != "")
                        //     {
                        //         byte[] content = (byte[])cmd.ExecuteScalar();
                        //         MemoryStream stream = new MemoryStream(content);
                        //         Image Sign = Image.FromStream(stream);

                        //         using (var b = new Bitmap(Sign.Width, Sign.Height))
                        //         {
                        //             b.SetResolution(Sign.HorizontalResolution, Sign.VerticalResolution);

                        //             using (var g = Graphics.FromImage(b))
                        //             {
                        //                 g.Clear(Color.White);
                        //                 g.DrawImageUnscaled(Sign, 0, 0);
                        //             }
                        //             //b = resizeImage(Sign, new Size(400, 300));
                        //             b.Save("C://OLIVOSERVER2020/Images/" + nPkeyID + ".png");
                        //         }
                        //     }
                        // }

                        if (nFormID == 1454)
                        {
                            DataTable dt = dLayer.ExecuteDataTable("select i_sign,N_ActionID from vw_Log_ApprovalAppraisal where n_transid=" + nPkeyID + " and N_CompanyID=" + nCompanyId, QueryParams, connection, transaction);
                            foreach (DataRow var in dt.Rows)
                            {
                                SqlCommand cmd = new SqlCommand("Select isnull(i_sign,'') as  i_sign from vw_Log_ApprovalAppraisal where N_ActionID=" + var["N_ActionID"].ToString(), connection, transaction);
                                if ((cmd.ExecuteScalar().ToString()) != "" && cmd.ExecuteScalar().ToString() != "0x" && cmd.ExecuteScalar().ToString() != "System.Byte[]")
                                {
                                    byte[] content = (byte[])cmd.ExecuteScalar();
                                    MemoryStream stream = new MemoryStream(content);
                                    Image Sign = Image.FromStream(stream);
                                    using (var b = new Bitmap(Sign.Width, Sign.Height))
                                    {
                                        b.SetResolution(Sign.HorizontalResolution, Sign.VerticalResolution);

                                        using (var g = Graphics.FromImage(b))
                                        {
                                            g.Clear(Color.White);
                                            g.DrawImageUnscaled(Sign, 0, 0);
                                        }
                                        b.Save("C://OLIVOSERVER2020/Images/" + var["N_ActionID"].ToString() + ".png");
                                    }
                                }
                            }
                        }

                        //Approval Signature

                        DataTable ApprovalSig = dLayer.ExecuteDataTable("select i_sign,N_ActionID from vw_ApprovalLog where n_transid=" + nPkeyID + " and N_FormID="+nFormID+" and N_CompanyID=" + nCompanyId + " and i_sign is not null", QueryParams, connection, transaction);

                        if (ApprovalSig.Rows.Count>0)
                        {
                            foreach (DataRow var in ApprovalSig.Rows)
                            {
                                SqlCommand cmd = new SqlCommand("Select isnull(i_sign,'') as  i_sign from vw_ApprovalLog where N_ActionID=" + var["N_ActionID"].ToString(), connection, transaction);
                                if ((cmd.ExecuteScalar().ToString()) != "" && cmd.ExecuteScalar().ToString() != "0x")
                                {
                                    try{
                                    byte[] content = (byte[])cmd.ExecuteScalar();
                                    MemoryStream stream = new MemoryStream(content);
                                    Image Sign = Image.FromStream(stream);
                                    using (var b = new Bitmap(Sign.Width, Sign.Height))
                                    {
                                        b.SetResolution(Sign.HorizontalResolution, Sign.VerticalResolution);

                                        using (var g = Graphics.FromImage(b))
                                        {
                                            g.Clear(Color.White);
                                            g.DrawImageUnscaled(Sign, 0, 0);
                                        }
                                        b.Save("C://OLIVOSERVER2020/Images/" + var["N_ActionID"].ToString() + ".png");
                                    }
                                    }
                                    catch{}
                                }
                            }
                        }
                        
                        object b_EnableZatcaValidation = dLayer.ExecuteScalar("select isnull(b_EnableZatcaValidation,0) from acc_company where n_companyid=" + nCompanyId, QueryParams, connection, transaction);
                        if(myFunctions.getBoolVAL(b_EnableZatcaValidation.ToString()))
                        {
                             object QR = dLayer.ExecuteScalar("select isnull(X_ZatcaQr,'') from Inv_Sales where n_companyid=" + nCompanyId + " and N_SalesID="+nPkeyID, QueryParams, connection, transaction);
                             object XML = dLayer.ExecuteScalar("select isnull(X_ZatcaXml,'') from Inv_Sales where n_companyid=" + nCompanyId + " and N_SalesID="+nPkeyID, QueryParams, connection, transaction);
                              if(QR.ToString()!="")
                                QRurl=QR.ToString();
                              if(XML.ToString()!="")
                                 Xmlpath=XML.ToString();
                        }



                        string URL = reportApi + "api/report?reportName=" + ReportName + "&critiria=" + critiria + "&path=" + this.TempFilesPath + "&reportLocation=" + RPTLocation + "&dbval=" + dbName + "&random=" + random + "&x_comments=" + x_comments + "&x_Reporttitle=&extention=pdf&N_FormID=" + nFormID + "&QRUrl=" + QRurl + "&N_PkeyID=" + nPkeyID + "&partyName=" + partyName + "&docNumber=" + docNumber + "&formName=" + FormName + "&xml="+Xmlpath+"&company="+myFunctions.GetCompanyName(User);
                        var path = client.GetAsync(URL);

                        //WHATSAPP MODE
                        DataTable Whatsapp = dLayer.ExecuteDataTable("select * from vw_GeneralScreenSettings where N_CompanyID=" + nCompanyId + " and N_FnyearID=" + nFnYearID + " and N_MenuID=" + nFormID + " and N_Type=2", QueryParams, connection, transaction);
                        if (Whatsapp.Rows.Count > 0)
                        {
                            if (myFunctions.getBoolVAL(Whatsapp.Rows[0]["B_AutoSend"].ToString()) && printSave)
                            {
                                string Company = myFunctions.GetCompanyName(User);
                                string FILEPATH = AppURL + "/temp/" + FormName + "_" + docNumber + "_" + partyName.Trim() + "_" + random + ".pdf";
                                object WhatsappAPI = Whatsapp.Rows[0]["X_WhatsappKey"].ToString();
                                object Currency = dLayer.ExecuteScalar("select x_currency from acc_company  where n_companyid=" + nCompanyId, QueryParams, connection, transaction);
                                string Body = Whatsapp.Rows[0]["X_Body"].ToString();
                                Body = Body.Replace("</p>", "");
                                Body = Body.Replace("<br>", "");
                                Body = Body.Replace("<p>", "%0A");
                                string body = Body;//+ " %0A%0ARegards, %0A" + Company;
                                object Mobile = "";
                                object Date = "";
                                object Party = "";
                                if (nFormID == 64)
                                {
                                    Mobile = dLayer.ExecuteScalar("select x_Phoneno1 from vw_InvSalesInvoiceNo_Search_Cloud where n_salesid=" + nPkeyID + " and N_CompanyID=" + nCompanyId, QueryParams, connection, transaction);
                                    Date = dLayer.ExecuteScalar("select d_salesdate from vw_InvSalesInvoiceNo_Search_Cloud where n_salesid=" + nPkeyID + " and N_CompanyID=" + nCompanyId, QueryParams, connection, transaction);
                                    Party = dLayer.ExecuteScalar("select Customer from vw_InvSalesInvoiceNo_Search_Cloud where n_salesid=" + nPkeyID + " and N_CompanyID=" + nCompanyId, QueryParams, connection, transaction);
                                }
                                else if (nFormID == 80)
                                {
                                    Mobile = dLayer.ExecuteScalar("select x_Phone from VW_InvSalesQuotationMaster where n_quotationid=" + nPkeyID + " and N_CompanyID=" + nCompanyId, QueryParams, connection, transaction);
                                    Date = dLayer.ExecuteScalar("select d_quotationdate from VW_InvSalesQuotationMaster where n_quotationid=" + nPkeyID + " and N_CompanyID=" + nCompanyId, QueryParams, connection, transaction);
                                    Party = dLayer.ExecuteScalar("select x_crmcustomer from VW_InvSalesQuotationMaster where n_quotationid=" + nPkeyID + " and N_CompanyID=" + nCompanyId, QueryParams, connection, transaction);

                                }
                                else if (nFormID == 81)

                                {
                                    Mobile = dLayer.ExecuteScalar("select x_Phoneno1 from vw_InvSalesOrder where n_salesorderID=" + nPkeyID + " and N_CompanyID=" + nCompanyId, QueryParams, connection, transaction);
                                    Date = dLayer.ExecuteScalar("select D_OrderDate from vw_InvSalesOrder where n_salesorderID=" + nPkeyID + " and N_CompanyID=" + nCompanyId, QueryParams, connection, transaction);
                                    Party = dLayer.ExecuteScalar("select x_customername from vw_InvSalesOrder where n_salesorderID=" + nPkeyID + " and N_CompanyID=" + nCompanyId, QueryParams, connection, transaction);
                                }
                                Body = Body.Replace("@PartyName", Party.ToString());
                                Body = Body.Replace("@CompanyName", Company.ToString());
                                Body = Body.Replace("@Date", Convert.ToDateTime(Date).ToString("dd-M-yyyy"));


                                var clientFile = new HttpClient(handler);
                                string URLAPI = "https://api.textmebot.com/send.php?recipient=+" + Mobile + "&apikey=" + WhatsappAPI + "&text=" + Body;
                                var FileMSG = clientFile.GetAsync(URLAPI);
                                FileMSG.Wait();

                                if (myFunctions.getBoolVAL(Whatsapp.Rows[0]["B_AttachPdf"].ToString()))
                                {
                                    Thread.Sleep(6000);
                                    string URLFILE = "https://api.textmebot.com/send.php?recipient=+" + Mobile + "&apikey=" + WhatsappAPI + "&document=" + FILEPATH;
                                    var MSG1 = client.GetAsync(URLFILE);
                                    MSG1.Wait();
                                }





                            }
                        }



                        if (nFormID == 80)
                        {
                            SortedList Params = new SortedList();
                            object N_OpportunityID = dLayer.ExecuteScalar("select N_OpportunityID from inv_salesquotation where N_CompanyID =" + myFunctions.GetCompanyID(User) + " and N_QuotationID=" + nPkeyID, Params, connection, transaction);
                            if (N_OpportunityID != null)
                            {
                                if (myFunctions.getIntVAL(N_OpportunityID.ToString()) > 0)
                                {

                                    object Mailsend = dLayer.ExecuteScalar("select B_MailSend from inv_salesquotation where N_CompanyID =" + myFunctions.GetCompanyID(User) + " and N_QuotationID=" + nPkeyID, Params, connection, transaction);
                                    object Mail = dLayer.ExecuteScalar("select X_Email from vw_crmopportunity where N_CompanyID =" + myFunctions.GetCompanyID(User) + " and N_OpportunityID=" + N_OpportunityID, Params, connection, transaction);
                                    if (Mailsend.ToString() == "")
                                    {
                                        if (sendmail(this.TempFilesPath + ReportName + random + ".pdf", Mail.ToString()))
                                        {
                                            dLayer.ExecuteNonQuery("update inv_salesquotation set B_MailSend=1 where N_CompanyID=" + nCompanyId + " and N_QuotationID=" + nPkeyID, Params, connection, transaction);
                                            transaction.Commit();
                                        }
                                    }

                                }
                            }

                        }


                        // ReportName = FormName + "_" + docNumber + "_" + partyName.Trim()+".pdf";
                        if(Xmlpath!="")
                            ReportName = FormName + "_" + docNumber + "_" + partyName.Trim() + "_" + random + "xml.pdf";
                        else
                            ReportName = FormName + "_" + docNumber + "_" + partyName.Trim() + "_" + random + ".pdf";
                        path.Wait();

                        if (env.EnvironmentName != "Development" && !System.IO.File.Exists(this.TempFilesPath + ReportName))
                            return Ok(_api.Error(User, "Report Generation Failed"));
                        else
                            return Ok(_api.Success(new SortedList() { { "FileName", ReportName } }));

                    }
                    else
                    {
                        return Ok(_api.Error(User, "Report Generation Failed"));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }

        }
        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }
        public bool CreateBarcode(string Data)
        {
            if (Data != "")
            {
                Zen.Barcode.Code128BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
                Image img = barcode.Draw(Data, 50);
                img.Save("C://OLIVOSERVER2020/Barcode/" + Data + ".png", ImageFormat.Png);
            }
            return true;
        }

        public bool sendmail(string url, string mail)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SortedList Params = new SortedList();
                    SqlTransaction transaction = connection.BeginTransaction();
                    int companyid = myFunctions.GetCompanyID(User);
                    string Toemail = "";

                    Toemail = mail;
                    object companyemail = "";
                    object companypassword = "";

                    companyemail = dLayer.ExecuteScalar("select X_Value from Gen_Settings where X_Group='210' and X_Description='EmailAddress' and N_CompanyID=" + companyid, Params, connection, transaction);
                    companypassword = dLayer.ExecuteScalar("select X_Value from Gen_Settings where X_Group='210' and X_Description='EmailPassword' and N_CompanyID=" + companyid, Params, connection, transaction);

                    string Subject = "";
                    if (Toemail.ToString() != "")
                    {
                        if (companyemail.ToString() != "")
                        {
                            object body = null;
                            string MailBody;
                            body = "Hi,<br> please find the attached quotation for your review";
                            if (body != null)
                            {
                                body = body.ToString();
                            }
                            else
                                body = "";

                            string Sender = companyemail.ToString();
                            Subject = "Quotation";
                            MailBody = body.ToString();


                            SmtpClient client = new SmtpClient
                            {
                                Host = "smtp.gmail.com",
                                Port = 587,
                                EnableSsl = true,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                Credentials = new System.Net.NetworkCredential(companyemail.ToString(), companypassword.ToString()),
                                Timeout = 10000,
                            };

                            MailMessage message = new MailMessage();
                            message.To.Add(Toemail.ToString()); // Add Receiver mail Address  
                            message.From = new MailAddress(Sender);
                            message.Subject = Subject;
                            message.Body = MailBody;
                            message.From = new MailAddress("sanjay.kv@olivotech.com", "Al Raza Photography");
                            message.IsBodyHtml = true; //HTML email  
                            message.Attachments.Add(new Attachment(url));
                            client.Send(message);

                        }
                    }


                }
                return true;
            }

            catch (Exception ie)
            {
                return false;
            }
        }

        [HttpGet("shiftSchedulePrint")]
        public IActionResult GetshiftSchedulePrint(int nFormID, DateTime dPeriodFrom, DateTime dPeriodTo, int nFnYearID, string xCriteria, int nDepartmentID)
        {
            string RPTLocation = reportLocation;
            string ReportName = "";
            string critiria = "";
            var random = RandomString();
            SortedList QueryParams = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();
                    QueryParams.Add("@p1", nCompanyId);
                    QueryParams.Add("@p3", nFnYearID);


                    string TableName = "";
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                    };

                    object ObjPath = dLayer.ExecuteScalar("SELECT Acc_TaxType.X_RepPathCaption FROM Acc_TaxType LEFT OUTER JOIN Acc_FnYear ON Acc_TaxType.N_TypeID = Acc_FnYear.N_TaxType where Acc_FnYear.N_CompanyID=@p1 and Acc_FnYear.N_FnYearID=@p3", QueryParams, connection, transaction);
                    string TaxType = ObjPath + "/";


                    if (nFormID == 1260)
                    {
                        if (xCriteria == "department")
                        {

                            critiria = "{vw_Pay_Empshiftdetails.D_Date}>=Date('" + dPeriodFrom.Year + "," + dPeriodFrom.Month + "," + dPeriodFrom.Day + "') and {vw_Pay_Empshiftdetails.D_Date}<=Date('" + dPeriodTo.Year + "," + dPeriodTo.Month + "," + dPeriodTo.Day + "') and  {vw_Pay_Empshiftdetails.N_DepartmentID}=" + nDepartmentID + "";
                            TableName = "vw_Pay_Empshiftdetails";


                            RPTLocation = reportLocation + "printing/";
                            ReportName = "Employee_ShiftSchedule";


                        }
                        else if (xCriteria == "all")
                        {
                            critiria = "{vw_Pay_Empshiftdetails.D_Date}>=Date('" + dPeriodFrom.Year + "," + dPeriodFrom.Month + "," + dPeriodFrom.Day + "') and {vw_Pay_Empshiftdetails.D_Date}<=Date('" + dPeriodTo.Year + "," + dPeriodTo.Month + "," + dPeriodTo.Day + "')";
                            TableName = "vw_Pay_Empshiftdetails";


                            RPTLocation = reportLocation + "printing/";
                            ReportName = "Employee_ShiftSchedule";



                        }
                        var client = new HttpClient(handler);
                        var dbName = connection.Database;

                        if (TableName != "" && critiria != "")
                        {
                            critiria = critiria + " and {" + TableName + ".N_CompanyID}=" + myFunctions.GetCompanyID(User);
                        }
                        //string URL = reportApi + "api/report?reportName=" + ReportName + "&critiria=" + critiria + "&path=" + this.TempFilesPath + "&reportLocation=" + RPTLocation + "&dbval=" + dbName + "&random=" + random + "&x_comments=&x_Reporttitle=&extention=pdf";
                        string URL = reportApi + "api/report?reportName=" + ReportName + "&critiria=" + critiria + "&path=" + this.TempFilesPath + "&reportLocation=" + RPTLocation + "&dbval=" + dbName + "&random=" + random + "&x_comments=&x_Reporttitle=&extention=pdf&N_FormID=0&QRUrl=&N_PkeyID=0&partyName=&docNumber=&formName=&xml="+Xmlpath;;
                        var path = client.GetAsync(URL);
                        path.Wait();

                    }
                    return Ok(_api.Success(new SortedList() { { "FileName", ReportName.Trim() + random + ".pdf" } }));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }


        }
        public bool CopyFiles(string fileToCopy, string destinationFile, string reportName)
        {
            try
            {
                string ZipLocation = destinationFile + ".zip";
                if (System.IO.File.Exists(destinationFile))
                {
                    System.IO.File.Delete(destinationFile);
                }
                if (System.IO.File.Exists(ZipLocation))
                {
                    System.IO.File.Delete(ZipLocation);
                }
                System.IO.File.Copy(fileToCopy, destinationFile);
                using (FileStream fs = new FileStream(ZipLocation, FileMode.Create))
                using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
                {
                    arch.CreateEntryFromFile(destinationFile, reportName);
                }
                if (System.IO.File.Exists(destinationFile))
                {
                    System.IO.File.Delete(destinationFile);
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
        public static DateTime GetFastestNISTDate()
        {
            var result = DateTime.MinValue;
            // Initialize the list of NIST time servers
            // http://tf.nist.gov/tf-cgi/servers.cgi
            string[] servers = new string[] {
"nist1-ny.ustiming.org",
"nist1-nj.ustiming.org",
"nist1-pa.ustiming.org",
"time-a.nist.gov",
"time-b.nist.gov",
"nist1.aol-va.symmetricom.com",
"nist1.columbiacountyga.gov",
"nist1-chi.ustiming.org",
"nist.expertsmi.com",
"nist.netservicesgroup.com"
};

            // Try 5 servers in random order to spread the load
            Random rnd = new Random();
            foreach (string server in servers.OrderBy(s => rnd.NextDouble()).Take(5))
            {
                try
                {
                    // Connect to the server (at port 13) and get the response
                    string serverResponse = string.Empty;
                    using (var reader = new StreamReader(new System.Net.Sockets.TcpClient(server, 13).GetStream()))
                    {
                        serverResponse = reader.ReadToEnd();
                    }

                    // If a response was received
                    if (!string.IsNullOrEmpty(serverResponse))
                    {
                        // Split the response string ("55596 11-02-14 13:54:11 00 0 0 478.1 UTC(NIST) *")
                        string[] tokens = serverResponse.Split(' ');

                        // Check the number of tokens
                        if (tokens.Length >= 6)
                        {
                            // Check the health status
                            string health = tokens[5];
                            if (health == "0")
                            {
                                // Get date and time parts from the server response
                                string[] dateParts = tokens[1].Split('-');
                                string[] timeParts = tokens[2].Split(':');

                                // Create a DateTime instance
                                DateTime utcDateTime = new DateTime(
                                    Convert.ToInt32(dateParts[0]) + 2000,
                                    Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[2]),
                                    Convert.ToInt32(timeParts[0]), Convert.ToInt32(timeParts[1]),
                                    Convert.ToInt32(timeParts[2]));

                                // Convert received (UTC) DateTime value to the local timezone
                                result = utcDateTime.ToLocalTime();

                                return result;
                                // Response successfully received; exit the loop

                            }
                        }

                    }

                }
                catch
                {
                    // Ignore exception and try the next server
                }
            }
            return result;
        }

        [HttpPost("getModuleReport")]
        public IActionResult GetModuleReports([FromBody] DataSet ds)
        {
            DataTable MasterTable;
            DataTable DetailTable;

            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            int nCompanyID = myFunctions.GetCompanyID(User);
            string x_comments = "";
            string x_Reporttitle = "";
            string X_TextforAll = "=all";
            int nUserID = myFunctions.GetUserID(User);
            var random = RandomString();
            DateTime currentTime;

            try
            {
                String Criteria = "";
                String reportName = "";
                String CompanyData = "";
                String YearData = "";
                String BranchData = "";
                String FieldName = "";
                String UserData = "";

                var dbName = "";
                string Extention = "";
                int LanguageID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_LanguageID"].ToString());
                int MainMenuID = myFunctions.getIntVAL(MasterTable.Rows[0]["moduleID"].ToString());
                if (MainMenuID == 1680)
                    connectionString = connectionStringClinet;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MainMenuID = myFunctions.getIntVAL(MasterTable.Rows[0]["moduleID"].ToString());

                    int MenuID = myFunctions.getIntVAL(MasterTable.Rows[0]["reportCategoryID"].ToString());
                    int ReportID = myFunctions.getIntVAL(MasterTable.Rows[0]["reportID"].ToString());
                    int FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["nFnYearID"].ToString());
                    int BranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["nBranchID"].ToString());
                    int ActionID = myFunctions.getIntVAL(MasterTable.Rows[0]["action"].ToString());

                    int SalesmanID = 0;
                    string procParam = "";
                    string xProCode = "";
                    Extention = MasterTable.Rows[0]["extention"].ToString();

                    SortedList Params1 = new SortedList();
                    Params1.Add("@nMenuID", MenuID);
                    Params1.Add("@xType", "RadioButton");
                    Params1.Add("@nCompID", ReportID);


                    reportName = dLayer.ExecuteScalar("select X_rptFile from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID and B_Active=1", Params1, connection).ToString();

                    if (ActionID == 2)
                    {
                        string fileToCopy = reportLocation + reportName;
                        string destinationFile = this.TempFilesPath + reportName;
                        string ZipLocation = this.TempFilesPath + reportName + ".zip";
                        if (CopyFiles(fileToCopy, destinationFile, reportName))
                            return Ok(_api.Success(new SortedList() { { "FileName", reportName.Trim() + ".zip" } }));
                        // if (System.IO.File.Exists(destinationFile))
                        // {
                        //     System.IO.File.Delete(destinationFile);
                        // }
                        // if (System.IO.File.Exists(ZipLocation))
                        // {
                        //     System.IO.File.Delete(ZipLocation);
                        // }
                        // System.IO.File.Copy(fileToCopy, destinationFile);
                        // using (FileStream fs = new FileStream(ZipLocation, FileMode.Create))
                        // using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
                        // {
                        //     arch.CreateEntryFromFile(destinationFile, reportName);
                        // }
                        // if (System.IO.File.Exists(destinationFile))
                        // {
                        //     System.IO.File.Delete(destinationFile);
                        // }

                    }
                    reportName = reportName.Substring(0, reportName.Length - 4);
                    SortedList Params = new SortedList();
                    Params.Add("@xMain", "MainForm");
                    Params.Add("@nMenuID", MenuID);
                    CompanyData = dLayer.ExecuteScalar("select X_DataFieldCompanyID from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();
                    YearData = dLayer.ExecuteScalar("select X_DataFieldYearID from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();
                    BranchData = dLayer.ExecuteScalar("select X_DataFieldBranchID from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();

                    Params.Add("@xType", "");
                    Params.Add("@nCompID", 0);
                    foreach (DataRow var in DetailTable.Rows)
                    {
                        int compID = myFunctions.getIntVAL(var["compId"].ToString());
                        string type = var["type"].ToString();
                        string value = var["value"].ToString();
                        string valueTo = var["valueTo"].ToString();

                        Params["@xType"] = type.ToLower();
                        Params["@nCompID"] = compID;


                        string xFeild = dLayer.ExecuteScalar("select X_DataField from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID", Params, connection).ToString();
                        bool bRange = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select isNull(B_Range,0) from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID", Params, connection).ToString());
                        string xOperator = dLayer.ExecuteScalar("select isNull(X_Operator,'') from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID", Params, connection).ToString();
                        xProCode = dLayer.ExecuteScalar("select X_ProcCode from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();
                        string xInstanceCode = dLayer.ExecuteScalar("select isNull(X_DataField,'') from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();
                        FieldName = dLayer.ExecuteScalar("select X_Text from vw_WebReportMenus where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID and N_LanguageId=" + LanguageID, Params, connection).ToString();
                        UserData = dLayer.ExecuteScalar("select X_DataFieldUserID from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();
                        FieldName = FieldName + "=";

                        if (xOperator == null || xOperator == "")
                            xOperator = "=";

                        if (x_Reporttitle != "")
                            x_Reporttitle += ", ";

                        if (type.ToLower() == "datepicker")
                        {
                            DateTime dateFrom = Convert.ToDateTime(value);
                            DateTime dateTo = Convert.ToDateTime(valueTo);
                            x_comments = dateFrom.ToString("dd-MM-yyyy") + " to " + dateTo.ToString("dd-MM-yyyy");

                            if (dateFrom != null && (bRange && dateTo != null))
                            {
                                x_Reporttitle = x_Reporttitle + FieldName + dateFrom.ToString("dd-MMM-yyyy") + " - " + dateTo.ToString("dd-MMM-yyyy");
                                x_comments = dateFrom.ToString("dd-MMM-yyyy") + " to " + dateTo.ToString("dd-MMM-yyyy");
                                procParam = dateFrom.ToString("dd-MMM-yyyy") + "|" + dateTo.ToString("dd-MMM-yyyy") + "|";
                            }
                            else if (dateFrom != null && !bRange)
                            {
                                x_Reporttitle = x_Reporttitle + FieldName + dateFrom.ToString("dd-MMM-yyyy");
                                x_comments = dateFrom.ToString("dd-MMM-yyyy");
                                procParam = dateFrom.ToString("dd-MMM-yyyy");
                            }
                            else if (bRange && dateTo != null)
                            {
                                x_Reporttitle = x_Reporttitle + FieldName + dateTo.ToString("dd-MMM-yyyy");
                                x_comments = dateTo.ToString("dd-MMM-yyyy");
                                procParam = dateTo.ToString("dd-MMM-yyyy") + "|";
                            }

                            string DateCrt = "";
                            if (xFeild != "")
                            {
                                if (bRange)
                                {
                                    DateCrt = xFeild + " >= Date('" + dateFrom.Year + "," + dateFrom.Month + "," + dateFrom.Day + "') And " + xFeild + " <= Date('" + dateTo.Year + "," + dateTo.Month + "," + dateTo.Day + "') ";
                                }
                                else
                                {
                                    DateCrt = xFeild + " " + xOperator + " Date('" + dateFrom.Year + "," + dateFrom.Month + "," + dateFrom.Day + "') ";
                                }
                                Criteria = Criteria == "" ? DateCrt : Criteria + " and " + DateCrt;
                            }
                        }
                        else
                        {
                            if (xFeild != "")
                            {
                                if (xFeild.Contains("#"))
                                    Criteria = Criteria == "" ? xFeild.Replace("#", value) : Criteria + " and " + xFeild.Replace("#", value);
                                else
                                {
                                    if (xFeild == "{Inv_Salesman.N_SalesmanID}")
                                    {
                                        Criteria = Criteria == "" ? xFeild + " " + xOperator + " " + value + " " : Criteria + " and " + xFeild + " " + xOperator + " " + value + " ";
                                        SalesmanID = myFunctions.getIntVAL(value.ToString());
                                    }
                                    else
                                    {
                                        if (bRange && valueTo != "")
                                            Criteria = Criteria == "" ? xFeild + " " + ">= '" + value + "' and " + xFeild + " " + "<= '" + valueTo + "'" : Criteria + " and " + xFeild + " " + ">= '" + value + "' and " + xFeild + " " + "<= '" + valueTo + "'";
                                        else
                                        {
                                            if (value.Contains(","))
                                            {
                                                string[] values = value.Split(',');
                                                string filterval = "";
                                                for (int i = 0; i < values.Length; i++)
                                                {
                                                    filterval = filterval + xFeild + " " + xOperator + " '" + values[i] + "' or ";

                                                }
                                                filterval = filterval.Substring(0, filterval.Length - 3);
                                                Criteria = Criteria == "" ? "( " + filterval + " )" : Criteria + " and ( " + filterval + " ) ";

                                            }
                                            else
                                                Criteria = Criteria == "" ? xFeild + " " + xOperator + " '" + value + "' " : Criteria + " and " + xFeild + " " + xOperator + " '" + value + "' ";
                                        }
                                    }
                                }
                            }
                            if (bRange && valueTo != "")
                                x_Reporttitle = x_Reporttitle + FieldName + value + '-' + valueTo;
                            else
                                x_Reporttitle = x_Reporttitle + FieldName + value;
                        }



                        //{table.fieldname} in {?Start date} to {?End date}
                    }
                    if (Criteria == "" && CompanyData != "")
                    {
                        Criteria = Criteria + CompanyData + "=" + nCompanyID;
                        if (YearData != "")
                            Criteria = Criteria + " and " + YearData + "=" + FnYearID;
                        if (BranchData != "")
                        {
                            bool mainBranch = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select isnull(B_ShowallData,0) as B_ShowallData from Acc_BranchMaster where N_CompanyID=" + nCompanyID + " and N_BranchID=" + BranchID, Params, connection).ToString());
                            if (mainBranch == false)
                                Criteria = Criteria + " and ( " + BranchData + "=" + BranchID + " or " + BranchData + "=0 )";
                            // if (mainBranch == false)
                            //     Criteria = Criteria + " and ( " + BranchData + "=" + BranchID + " or " + BranchData + "=0 )";
                            // else if (xProCode == "11")
                            //     Criteria = Criteria + " and " + BranchData + "=" + BranchID;
                        }
                    }
                    else if (CompanyData != "")
                    {
                        // bool Consolidated = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select isnull(B_IsConsolidated,0) as B_IsConsolidated from acc_company where N_CompanyID=" + nCompanyID, Params, connection).ToString()); ;

                        // if (MenuID == 859 && Consolidated == false)
                        // {
                        //     string FnYear = dLayer.ExecuteScalar("select X_FnYearDescr from Acc_FnYear where N_CompanyID=" + nCompanyID + " and N_FnyearID=" + FnYearID, Params, connection).ToString();
                        //     int ClientID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_ClientID from Acc_Company where N_CompanyID=" + nCompanyID, Params, connection).ToString());
                        //     DataTable dt = dLayer.ExecuteDataTable("select * from vw_ConsolidatedCompany where n_clientID=" + ClientID + " and X_FnYearDescr='" + FnYear + "' order by N_CompanyID desc", Params, connection);
                        //     foreach (DataRow dr in dt.Rows)
                        //     {
                        //         Criteria = Criteria + " and " + CompanyData + "=" + dr["n_CompanyID"];
                        //     }
                        // }
                        // else
                        // {
                            Criteria = Criteria + " and " + CompanyData + "=" + nCompanyID;
                            if (YearData != "")
                                Criteria = Criteria + " and " + YearData + "=" + FnYearID;
                            if (BranchData != "")
                            {
                                bool mainBranch = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select isnull(B_ShowallData,0) as B_ShowallData from Acc_BranchMaster where N_CompanyID=" + nCompanyID + " and N_BranchID=" + BranchID, Params, connection).ToString());
                                if (mainBranch == false)
                                    Criteria = Criteria + " and ( " + BranchData + "=" + BranchID + " or " + BranchData + "=0 )";
                                // if (mainBranch == false)
                                //     Criteria = Criteria + " and ( " + BranchData + "=" + BranchID + " or " + BranchData + "=0 )";
                                // else if (xProCode == "11")
                                //     Criteria = Criteria + " and " + BranchData + "=" + BranchID;
                            }
                        // }
                    }
                    if (UserData != "")
                    {
                        if (Criteria == "")
                            Criteria = UserData + "=" + nUserID;
                        else
                            Criteria = Criteria + " and " + UserData + "=" + nUserID;
                    }
                    if (xProCode != "")
                    {
                        bool mainBranch = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select isnull(B_ShowallData,0) as B_ShowallData from Acc_BranchMaster where N_CompanyID=" + nCompanyID + " and N_BranchID=" + BranchID, Params, connection).ToString());
                        bool Consolidated = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select isnull(B_IsConsolidated,0) as B_IsConsolidated from acc_company where N_CompanyID=" + nCompanyID, Params, connection).ToString()); ;
                        dLayer.ExecuteNonQuery("delete from Acc_LedgerBalForReporting", connection);
                        dLayer.ExecuteNonQuery("delete from Acc_AccountStatement", connection);

                        if (Consolidated)
                        {
                            string FnYear = dLayer.ExecuteScalar("select X_FnYearDescr from Acc_FnYear where N_CompanyID=" + nCompanyID + " and N_FnyearID=" + FnYearID, Params, connection).ToString();
                            int ClientID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_ClientID from Acc_Company where N_CompanyID=" + nCompanyID, Params, connection).ToString());
                            DataTable dt = dLayer.ExecuteDataTable("select * from vw_ConsolidatedCompany where n_clientID=" + ClientID + " and X_FnYearDescr='" + FnYear + "' order by N_CompanyID desc", Params, connection);
                            foreach (DataRow dr in dt.Rows)
                            {
                                SortedList mParamsList = new SortedList()
                            {
                            {"N_CompanyID",dr["n_CompanyID"]},
                            {"N_FnYearID",dr["n_FnyearID"]},
                            {"N_PeriodID",0},
                            {"X_Code",xProCode},
                            {"X_Parameter", procParam },
                            {"N_UserID",myFunctions.GetUserID(User)},
                            {"N_BranchID",mainBranch ?0:BranchID},
                            };
                                dLayer.ExecuteDataTablePro("SP_OpeningBalanceGenerate", mParamsList, connection);
                            }
                        }
                        else
                        {
                            SortedList mParamsList = new SortedList()
                            {
                            {"N_CompanyID",nCompanyID},
                            {"N_FnYearID",FnYearID},
                            {"N_PeriodID",0},
                            {"X_Code",xProCode},
                            {"X_Parameter", procParam },
                            {"N_UserID",myFunctions.GetUserID(User)},
                            {"N_BranchID",mainBranch ?0:BranchID},
                            // {"N_SalesmanID",SalesmanID},
                            // {"X_InstanceCode",random},
                            };
                            dLayer.ExecuteDataTablePro("SP_OpeningBalanceGenerate", mParamsList, connection);
                            // if(xInstanceCode!="")
                            // Criteria = Criteria == "" ? xInstanceCode + "='"+random+"' " : Criteria + " and "+xInstanceCode+"='"+random+"' ";
                         }
                    }

                    dbName = connection.Database;
                    if (x_comments == "" && MainMenuID != 1680)
                    {
                        object TimezoneID = dLayer.ExecuteScalar("select isnull(n_timezoneid,82) from acc_company where N_CompanyID= " + nCompanyID, connection);
                        object Timezone = dLayer.ExecuteScalar("select X_ZoneName from Gen_TimeZone where n_timezoneid=" + TimezoneID, connection);
                        if (Timezone != null && Timezone.ToString() != "")
                        {
                            x_comments = DateTime.Now.ToString("dd/MM/yyyy");
                        }
                    }
                }



                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                };
                var client = new HttpClient(handler);

                //HttpClient client = new HttpClient(clientHandler);

                var rptArray = reportName.Split(@"\");
                string actReportLocation = reportLocation.Remove(reportLocation.Length - 2, 2) + LanguageID + "/";
                if (rptArray.Length > 1)
                {
                    reportName = rptArray[1].ToString();
                    actReportLocation = actReportLocation + rptArray[0].ToString() + "/";
                }


                //string URL = reportApi + "api/report?reportName=" + reportName + "&critiria=" + Criteria + "&path=" + this.TempFilesPath + "&reportLocation=" + actReportLocation + "&dbval=" + dbName + "&random=" + random + "&x_comments=" + x_comments + "&x_Reporttitle=" + x_Reporttitle + "&extention=" + Extention;
                string URL = reportApi + "api/report?reportName=" + reportName + "&critiria=" + Criteria + "&path=" + this.TempFilesPath + "&reportLocation=" + actReportLocation + "&dbval=" + dbName + "&random=" + random + "&x_comments=" + x_comments + "&x_Reporttitle=" + x_Reporttitle + "&extention=" + Extention + "&N_FormID=0&QRUrl=&N_PkeyID=0&partyName=&docNumber=&formName=&xml="+Xmlpath;;
                var path = client.GetAsync(URL);

                path.Wait();
                return Ok(_api.Success(new SortedList() { { "FileName", reportName.Trim() + random + "." + Extention } }));
                //string RptPath = reportPath + reportName.Trim() + ".pdf";
                // var memory = new MemoryStream();

                // using (var stream = new FileStream(RptPath, FileMode.Open))
                // {
                //     await stream.CopyToAsync(memory);
                // }
                // memory.Position = 0;
                // return File(memory, _api.GetContentType(RptPath), Path.GetFileName(RptPath));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        public void ZatcaIntegration(int N_SalesID)
        {
            Xmlpath="";
            int nCompanyId = myFunctions.GetCompanyID(User);
            string TotalPrice="";
            string invoicediscountDetails="";
            string TotalPriceAfterDiscount = "";
            string TotalVat="";
            string InvoiceTotalWithVAT="";
            int N_CustomerID=0;

            SortedList QueryParams = new SortedList();
            using (SqlConnection connection = new SqlConnection(connectionString))
                {
                connection.Open();
                SqlTransaction transaction;
                transaction = connection.BeginTransaction();

            //Products

            try
            {
                invlines = new List<InvoiceItems>();
                DataTable SalesDetailsMaster = dLayer.ExecuteDataTable("select x_itemname,n_sprice,n_qty,(n_qty*n_sprice) as TotalPrice,isnull(n_itemdiscamt,0) as n_itemdiscamt,(n_qty*n_sprice)-isnull(n_itemdiscamt,0) as TotalPriceAfterDiscount,N_TaxPercentage1,N_TaxAmt1,(n_qty*n_sprice)-isnull(n_itemdiscamt,0)+ISNULL(N_TaxAmt1,0) as TotalWithVat,N_CustomerID from vw_invsalesdetails where N_SalesID=" + N_SalesID + " and N_CompanyID=" + nCompanyId, QueryParams, connection, transaction);
                foreach (DataRow DetailsMaster in SalesDetailsMaster.Rows)
                {
                   
                        var line = new InvoiceItems();
                        line.ProductName = DetailsMaster["x_itemname"].ToString();
                        line.ProductPrice = Convert.ToDecimal(DetailsMaster["n_sprice"].ToString());
                        line.ProductQuantity = Convert.ToDecimal(DetailsMaster["n_qty"].ToString());
                        line.TotalPrice = Convert.ToDecimal(DetailsMaster["TotalPrice"].ToString());
                        line.DiscountValue = Convert.ToDecimal(DetailsMaster["n_itemdiscamt"].ToString());
                        line.TotalPriceAfterDiscount = Convert.ToDecimal(DetailsMaster["TotalPriceAfterDiscount"].ToString());
                        line.VatPercentage = Convert.ToDecimal(DetailsMaster["N_TaxPercentage1"].ToString());
                        line.VatValue = Convert.ToDecimal(DetailsMaster["N_TaxAmt1"].ToString());
                        line.TotalWithVat = Convert.ToDecimal(DetailsMaster["TotalWithVat"].ToString());
                        invlines.Add(line);
                        N_CustomerID=myFunctions.getIntVAL(DetailsMaster["N_CustomerID"].ToString());

                }
                TotalPrice = invlines.Sum(m => m.TotalPrice).ToString();
                invoicediscountDetails = invlines.Sum(m => m.DiscountValue).ToString();
                TotalPriceAfterDiscount = invlines.Sum(m => m.TotalPriceAfterDiscount).ToString();
                TotalVat = invlines.Sum(m => m.VatValue).ToString();
                InvoiceTotalWithVAT = invlines.Sum(m => m.TotalWithVat).ToString();

            }
            catch (Exception ex)
            {
                
            }



            UBLXML ubl = new UBLXML();
            Invoice inv = new Invoice();
            ZatcaIntegrationSDK.Result res = new ZatcaIntegrationSDK.Result();

            DataTable SalesMaster = dLayer.ExecuteDataTable("select * from Inv_Sales where N_SalesID=" + N_SalesID + " and N_CompanyID=" + nCompanyId, QueryParams, connection, transaction);
            foreach (DataRow salesvar in SalesMaster.Rows)
            {
            inv.ID = salesvar["x_receiptno"].ToString(); // مثال SME00010
            DateTime salesDate = (DateTime)salesvar["D_Salesdate"];
            inv.IssueDate = salesDate.ToString("yyyy-MM-dd"); // "2023-02-07"
            inv.IssueTime = salesDate.ToString("HH:mm:ss"); // "09:32:40"
            inv.invoiceTypeCode.id = 388;
            inv.invoiceTypeCode.Name = "0100000";
            inv.DocumentCurrencyCode = "SAR";
            inv.TaxCurrencyCode = "SAR";
               // هنا ممكن اضيف ال pih من قاعدة البيانات  
            inv.AdditionalDocumentReferencePIH.EmbeddedDocumentBinaryObject = "NWZlY2ViNjZmZmM4NmYzOGQ5NTI3ODZjNmQ2OTZjNzljMmRiYzIzOWRkNGU5MWI0NjcyOWQ3M2EyN2ZiNTdlOQ==";
            // قيمة عداد الفاتورة
            inv.AdditionalDocumentReferenceICV.UUID = Int32.Parse(salesvar["N_SalesID"].ToString()); // لابد ان يكون ارقام فقط
            // فى حالة فاتورة مبسطة وفاتورة ملخصة هانكتب تاريخ التسليم واخر تاريخ التسليم
            inv.delivery.ActualDeliveryDate =  salesDate.ToString("yyyy-MM-dd");
            inv.delivery.LatestDeliveryDate =  salesDate.ToString("yyyy-MM-dd");
            // 
            // بيانات الدفع 
            // اكواد معين
            // اختيارى كود الدفع
            string paymentcode = "10";
            if (!string.IsNullOrEmpty(paymentcode))
            {
                PaymentMeans paymentMeans = new PaymentMeans();
                paymentMeans.PaymentMeansCode = paymentcode; // اختيارى
                paymentMeans.InstructionNote = "Payment Notes"; // اجبارى فى الاشعارات
                inv.paymentmeans.Add(paymentMeans);
            }
            //DiscountValue=myFunctions.getVAL(var["N_MainDiscountF"].ToString());


            }

            DataTable CompanyMaster = dLayer.ExecuteDataTable("select * from Acc_Company where N_CompanyID=" + nCompanyId, QueryParams, connection, transaction);
            foreach (DataRow var in CompanyMaster.Rows)
            {
                // بيانات البائع 
            inv.SupplierParty.partyIdentification.ID = var["N_CompanyID"].ToString(); // رقم السجل التجارى الخاض بالبائع
            inv.SupplierParty.partyIdentification.schemeID = "CRN"; // رقم السجل التجارى
            inv.SupplierParty.postalAddress.StreetName = "street"; // اجبارى
            //inv.SupplierParty.postalAddress.AdditionalStreetName = ""; // اختيارى
            inv.SupplierParty.postalAddress.BuildingNumber = "1234"; // اجبارى رقم المبنى
            inv.SupplierParty.postalAddress.PlotIdentification = "9833"; //اختيارى
            inv.SupplierParty.postalAddress.CityName = "CityName"; // اسم المدينة
            inv.SupplierParty.postalAddress.PostalZone = "12345"; // الرقم البريدي
            //inv.SupplierParty.postalAddress.CountrySubentity = "Riyadh Region"; // اسم المحافظة او المدينة مثال (مكة) اختيارى
            inv.SupplierParty.postalAddress.CitySubdivisionName = "CitySubdivisionName"; // اسم المنطقة او الحى 
            inv.SupplierParty.postalAddress.country.IdentificationCode = "SA";
            inv.SupplierParty.partyLegalEntity.RegistrationName = var["X_Companyname"].ToString(); // اسم الشركة المسجل فى الهيئة
            inv.SupplierParty.partyTaxScheme.CompanyID = var["x_taxregistrationno"].ToString();  // رقم التسجيل الضريبي
            }

            DataTable PartyMaster = dLayer.ExecuteDataTable("select * from Inv_Customer where N_CompanyID=" + nCompanyId + " and N_CustomerID="+N_CustomerID, QueryParams, connection, transaction);
            foreach (DataRow var in PartyMaster.Rows)
            {
            //if (inv.invoiceTypeCode.Name == "0100000")
            //{
            // بيانات المشترى
            inv.CustomerParty.partyIdentification.ID = var["N_CustomerID"].ToString(); // رقم القومى الخاض بالمشترى
            inv.CustomerParty.partyIdentification.schemeID = "NAT"; // الرقم القومى
            inv.CustomerParty.postalAddress.StreetName = "street"; // اجبارى
            //inv.CustomerParty.postalAddress.AdditionalStreetName = "street name"; // اختيارى
            inv.CustomerParty.postalAddress.BuildingNumber = "1234"; // اجبارى رقم المبنى
           // inv.CustomerParty.postalAddress.PlotIdentification = "9833"; // اختيارى رقم القطعة
            inv.CustomerParty.postalAddress.CityName = "Jeddah"; // اسم المدينة
            inv.CustomerParty.postalAddress.PostalZone = "12345"; // الرقم البريدي
            //inv.CustomerParty.postalAddress.CountrySubentity = "Makkah"; // اسم المحافظة او المدينة مثال (مكة) اختيارى
            inv.CustomerParty.postalAddress.CitySubdivisionName = "CitySubdivisionName"; // اسم المنطقة او الحى 
            inv.CustomerParty.postalAddress.country.IdentificationCode = "SA";
            inv.CustomerParty.partyLegalEntity.RegistrationName = var["X_Customername"].ToString(); // اسم الشركة المسجل فى الهيئة
            inv.CustomerParty.partyTaxScheme.CompanyID = var["x_taxregistrationno"].ToString(); // رقم التسجيل الضريبي
                                                                         //}
            inv.CustomerParty.contact.Name = var["X_Customername"].ToString();  
            inv.CustomerParty.contact.Telephone =  var["x_phoneno1"].ToString(); 
            inv.CustomerParty.contact.ElectronicMail =  var["x_email"].ToString(); 
            inv.CustomerParty.contact.Note = "notes";

            }
            
         
            

          
            decimal invoicediscount = 0;
            Decimal.TryParse(invoicediscountDetails, out invoicediscount);
            if (invoicediscount > 0)
            {
                AllowanceCharge allowance = new AllowanceCharge();
                allowance.ChargeIndicator = false;
                //write this lines in case you will make discount as percentage
                allowance.MultiplierFactorNumeric = 0; //dscount percentage like 10
                allowance.BaseAmount = 0; // the amount we will apply percentage on example (MultiplierFactorNumeric=10 ,BaseAmount=1000 then AllowanceAmount will be 100 SAR)

                // in case we will make discount as Amount 
                allowance.Amount =  invoicediscount; // 
                allowance.AllowanceChargeReasonCode = ""; //سبب الخصم
                allowance.AllowanceChargeReason = "discount"; //سبب الخصم
                allowance.taxCategory.ID = "S";// كود الضريبة
                allowance.taxCategory.Percent = 15;// نسبة الضريبة
                //فى حالة عندى اكثر من خصم بعمل loop على الاسطر السابقة
                inv.allowanceCharges.Add(allowance);
            }
            decimal payableroundingamount = 0;
            Decimal.TryParse("", out payableroundingamount);

            inv.legalMonetaryTotal.PayableRoundingAmount = payableroundingamount;

            inv.legalMonetaryTotal.PrepaidAmount = 0;
            
            decimal payableamount = 0;
            Decimal.TryParse("", out payableamount);

            inv.legalMonetaryTotal.PayableAmount = payableamount;
            // فى حالة فى اكتر من منتج فى الفاتورة هانعمل ليست من invoiceline مثال الكود التالى

            foreach (InvoiceItems item in invlines)
            {
                InvoiceLine invline = new InvoiceLine();
               
                invline.InvoiceQuantity = item.ProductQuantity;
                invline.item.Name = item.ProductName;
                if (item.VatPercentage == 0)
                {
                    invline.item.classifiedTaxCategory.ID = "Z"; // كود الضريبة
                    invline.taxTotal.TaxSubtotal.taxCategory.ID = "Z"; // كود الضريبة
                    invline.taxTotal.TaxSubtotal.taxCategory.TaxExemptionReasonCode = "VATEX-SA-HEA"; // كود الضريبة
                    invline.taxTotal.TaxSubtotal.taxCategory.TaxExemptionReason = "Private healthcare to citizen"; // كود الضريبة
                }
                else
                {
                    invline.item.classifiedTaxCategory.ID = "S"; // كود الضريبة
                    invline.taxTotal.TaxSubtotal.taxCategory.ID = "S"; // كود الضريبة
                }
                invline.item.classifiedTaxCategory.Percent = item.VatPercentage; // نسبة الضريبة
                invline.taxTotal.TaxSubtotal.taxCategory.Percent = item.VatPercentage; // نسبة الضريبة

                invline.price.EncludingVat = false;
                invline.price.PriceAmount = item.ProductPrice;

                if (item.DiscountValue > 0)
                {
                    AllowanceCharge allowanceCharge = new AllowanceCharge();
                    // فى حالة الرسوم
                    // allowanceCharge.ChargeIndicator = true;
                    // فى حالة الخصم
                    allowanceCharge.ChargeIndicator = false;

                    allowanceCharge.AllowanceChargeReason = "discount"; // سبب الخصم على مستوى المنتج
                    // allowanceCharge.AllowanceChargeReasonCode = "90"; // سبب الخصم على مستوى المنتج
                    allowanceCharge.Amount = item.DiscountValue; // قيم الخصم

                    allowanceCharge.MultiplierFactorNumeric = 0;
                    allowanceCharge.BaseAmount = 0;
                    invline.allowanceCharges.Add(allowanceCharge);
                }
                inv.InvoiceLines.Add(invline);
            }
            InvoiceTotal invoiceTotal = ubl.CalculateInvoiceTotal(inv.InvoiceLines, inv.allowanceCharges);
            // txtTotalPrice.Text = invoiceTotal.LineExtensionAmount.ToString("0.00");
            // txtTotalPriceAfterDiscount.Text = invoiceTotal.TaxExclusiveAmount.ToString("0.00");
            // txtInvoiceTotalWithVAT.Text = invoiceTotal.TaxInclusiveAmount.ToString("0.00");
            // txtVAT.Text = (invoiceTotal.TaxInclusiveAmount - invoiceTotal.TaxExclusiveAmount).ToString("0.00");

            // here you can pass csid data
            //this is csid or publickey
            inv.cSIDInfo.CertPem = "MIIFADCCBKWgAwIBAgITbQAAGw/UXgsmTms9LgABAAAbDzAKBggqhkjOPQQDAjBiMRUwEwYKCZImiZPyLGQBGRYFbG9jYWwxEzARBgoJkiaJk/IsZAEZFgNnb3YxFzAVBgoJkiaJk/IsZAEZFgdleHRnYXp0MRswGQYDVQQDExJQRVpFSU5WT0lDRVNDQTItQ0EwHhcNMjMwOTIxMDgxODAyWhcNMjUwOTIxMDgyODAyWjBcMQswCQYDVQQGEwJTQTEMMAoGA1UEChMDVFNUMRYwFAYDVQQLEw1SaXlhZGggQnJhbmNoMScwJQYDVQQDEx5UU1QtMjA1MDAxMjA5NS0zMDAwMDAxMzUyMjAwMDMwVjAQBgcqhkjOPQIBBgUrgQQACgNCAASbUK/x5nG7tMATY9Z/u60/eKzfGtdM2WbAFe654OPM1Fb1aBj/JEqgSp5dJQtuahldiKPfJ8aCH8I1tN0cbRxBo4IDQTCCAz0wJwYJKwYBBAGCNxUKBBowGDAKBggrBgEFBQcDAjAKBggrBgEFBQcDAzA8BgkrBgEEAYI3FQcELzAtBiUrBgEEAYI3FQiBhqgdhND7EobtnSSHzvsZ08BVZoGc2C2D5cVdAgFkAgETMIHNBggrBgEFBQcBAQSBwDCBvTCBugYIKwYBBQUHMAKGga1sZGFwOi8vL0NOPVBFWkVJTlZPSUNFU0NBMi1DQSxDTj1BSUEsQ049UHVibGljJTIwS2V5JTIwU2VydmljZXMsQ049U2VydmljZXMsQ049Q29uZmlndXJhdGlvbixEQz1leHRnYXp0LERDPWdvdixEQz1sb2NhbD9jQUNlcnRpZmljYXRlP2Jhc2U/b2JqZWN0Q2xhc3M9Y2VydGlmaWNhdGlvbkF1dGhvcml0eTAdBgNVHQ4EFgQU6PKLogVxfkECr0gYpM0CSaBn1m8wDgYDVR0PAQH/BAQDAgeAMIGtBgNVHREEgaUwgaKkgZ8wgZwxOzA5BgNVBAQMMjEtVFNUfDItVFNUfDMtOTVjNjRhZjgtYTI4NS00ZGFlLTg4MDMtYWYwNzNhZmU4ZjBkMR8wHQYKCZImiZPyLGQBAQwPMzAwMDAwMTM1MjIwMDAzMQ0wCwYDVQQMDAQxMTAwMQ4wDAYDVQQaDAVNYWtrYTEdMBsGA1UEDwwUTWVkaWNhbCBMYWJvcmF0b3JpZXMwgeQGA1UdHwSB3DCB2TCB1qCB06CB0IaBzWxkYXA6Ly8vQ049UEVaRUlOVk9JQ0VTQ0EyLUNBKDEpLENOPVBFWkVpbnZvaWNlc2NhMixDTj1DRFAsQ049UHVibGljJTIwS2V5JTIwU2VydmljZXMsQ049U2VydmljZXMsQ049Q29uZmlndXJhdGlvbixEQz1leHRnYXp0LERDPWdvdixEQz1sb2NhbD9jZXJ0aWZpY2F0ZVJldm9jYXRpb25MaXN0P2Jhc2U/b2JqZWN0Q2xhc3M9Y1JMRGlzdHJpYnV0aW9uUG9pbnQwHwYDVR0jBBgwFoAUgfKje3J7vVCjap/x6NON1nuccLUwHQYDVR0lBBYwFAYIKwYBBQUHAwIGCCsGAQUFBwMDMAoGCCqGSM49BAMCA0kAMEYCIQD52GbWVIWpbdu7B4BnDe+fIKlrAxRUjnGtcc8HiKCEDAIhAJqHLuv0Krp5+HiNCB6w5VPXBPhTKbKidRkZHeb2VTJ+";
            inv.cSIDInfo.PrivateKey = @"MHQCAQEEIFMxGrBBfmGxmv3rAmuAKgGrqnyNQYAfKqr7OVKDzgDYoAcGBSuBBAAKoUQDQgAEm1Cv8eZxu7TAE2PWf7utP3is3xrXTNlmwBXuueDjzNRW9WgY/yRKoEqeXSULbmoZXYij3yfGgh/CNbTdHG0cQQ==";
            string secretkey = "lHntHtEGWi+ZJtssv167Dy+R64uxf/PTMXg3CEGYfvM=";
            try
            {
                //string g=Guid.NewGuid().ToString();
                //if you need to save xml file true if not false;
                bool savexmlfile = true;
                res = ubl.GenerateInvoiceXML(inv, Directory.GetCurrentDirectory(), savexmlfile);
                 
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message.ToString() + "\n\n" + ex.InnerException.ToString());
            }

            if (!res.IsValid)
            {
                //return res.ErrorMessage;
            }
            //


            // if (rdb_simulation.Checked)
            //     mode = Mode.Simulation;
            // else if (rdb_production.Checked)
            //     mode = Mode.Production;
            // else
                mode = Mode.developer;


            // zatca call api
            ApiRequestLogic apireqlogic = new ApiRequestLogic(mode);

            InvoiceReportingRequest invrequestbody = new InvoiceReportingRequest();
            invrequestbody.invoice = res.EncodedInvoice;
            invrequestbody.invoiceHash = res.InvoiceHash;
            invrequestbody.uuid = res.UUID;
            // if (rdb_compliance.Checked)
            // {
                ComplianceCsrResponse tokenresponse = new ComplianceCsrResponse();
                string csr = @"-----BEGIN CERTIFICATE REQUEST-----
MIIB5DCCAYoCAQAwVTELMAkGA1UEBhMCU0ExFjAUBgNVBAsMDUVuZ2F6YXRCcmFu
Y2gxEDAOBgNVBAoMB0VuZ2F6YXQxHDAaBgNVBAMME1RTVC0zMDAzMDA4Njg2MDAw
MDMwVjAQBgcqhkjOPQIBBgUrgQQACgNCAARYvqwxwBzinhARQZYQnWBoSr8wMmmw
CdfTSleD+rZoh/NeJMF8reXaBFrMCrlPK0hTRXmCyXuc6nFUfjSvZU/goIHVMIHS
BgkqhkiG9w0BCQ4xgcQwgcEwIgYJKwYBBAGCNxQCBBUTE1RTVFpBVENBQ29kZVNp
Z25pbmcwgZoGA1UdEQSBkjCBj6SBjDCBiTE7MDkGA1UEBAwyMS1UU1R8Mi1UU1R8
My1lZDIyZjFkOC1lNmEyLTExMTgtOWI1OC1kOWE4ZjExZTQ0NWYxHzAdBgoJkiaJ
k/IsZAEBDA8zMDAzMDA4Njg2MDAwMDMxDTALBgNVBAwMBDExMDAxDDAKBgNVBBoM
A1RTVDEMMAoGA1UEDwwDVFNUMAoGCCqGSM49BAMCA0gAMEUCIQDRroaukEGwwRXW
RhOudGrd/OGrcUnnn2ftb6Jk4dDGFgIgaV+sXmaZlKbxR7k/lMhnf/2j95XHDkso
hup1ROPc+cc=
-----END CERTIFICATE REQUEST-----
";
                tokenresponse = apireqlogic.GetComplianceCSIDAPI("12345", csr);
                if (String.IsNullOrEmpty(tokenresponse.ErrorMessage))
                {
                    InvoiceReportingResponse invoicereportingmodel = apireqlogic.CallComplianceInvoiceAPI(tokenresponse.BinarySecurityToken, tokenresponse.Secret, invrequestbody);
                    if (invoicereportingmodel.IsSuccess)
                    {
                        //MessageBox.Show(invoicereportingmodel.ReportingStatus + invoicereportingmodel.ClearanceStatus); //REPORTED
                        QRurl= res.QRCode;
                        Xmlpath = res.SingedXMLFileNameFullPath;
                    }
                    else
                    {
                        //MessageBox.Show(invoicereportingmodel.ErrorMessage);
                    }
                }
                else
                {
                   // MessageBox.Show(tokenresponse.ErrorMessage);
                }
                }

        }


        // [HttpPost("getModuleReport")]
        // public IActionResult GetModuleReports([FromBody] DataSet ds)
        // {
        //     DataTable MasterTable;
        //     DataTable DetailTable;

        //     MasterTable = ds.Tables["master"];
        //     DetailTable = ds.Tables["details"];
        //     int nCompanyID = myFunctions.GetCompanyID(User);
        //     string x_comments = "";
        //     string x_Reporttitle = "";
        //     string X_TextforAll = "=all";
        //     int nUserID = myFunctions.GetUserID(User);

        //     try
        //     {
        //         String Criteria = "";
        //         String reportName = "";
        //         String CompanyData = "";
        //         String YearData = "";
        //         String FieldName = "";
        //         String UserData = "";

        //         var dbName = "";
        //         string Extention = "";
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             //int MenuID = myFunctions.getIntVAL(MasterTable.Rows[0]["moduleID"].ToString());
        //             int MenuID = myFunctions.getIntVAL(MasterTable.Rows[0]["reportCategoryID"].ToString());
        //             int ReportID = myFunctions.getIntVAL(MasterTable.Rows[0]["reportID"].ToString());
        //             int FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["nFnYearID"].ToString());
        //             Extention = MasterTable.Rows[0]["extention"].ToString();

        //             SortedList Params1 = new SortedList();
        //             Params1.Add("@nMenuID", MenuID);
        //             Params1.Add("@xType", "RadioButton");
        //             Params1.Add("@nCompID", ReportID);


        //             reportName = dLayer.ExecuteScalar("select X_rptFile from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID and B_Active=1", Params1, connection).ToString();

        //             reportName = reportName.Substring(0, reportName.Length - 4);


        //             foreach (DataRow var in DetailTable.Rows)
        //             {
        //                 int compID = myFunctions.getIntVAL(var["compId"].ToString());
        //                 string type = var["type"].ToString();
        //                 string value = var["value"].ToString();
        //                 string valueTo = var["valueTo"].ToString();

        //                 SortedList Params = new SortedList();
        //                 Params.Add("@nMenuID", MenuID);
        //                 Params.Add("@xType", type.ToLower());
        //                 Params.Add("@nCompID", compID);
        //                 Params.Add("@xMain", "MainForm");
        //                 string xFeild = dLayer.ExecuteScalar("select X_DataField from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID", Params, connection).ToString();
        //                 bool bRange = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select isNull(B_Range,0) from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID", Params, connection).ToString());
        //                 string xOperator = dLayer.ExecuteScalar("select isNull(X_Operator,'') from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID", Params, connection).ToString();
        //                 string xProCode = dLayer.ExecuteScalar("select X_ProcCode from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();
        //                 CompanyData = dLayer.ExecuteScalar("select X_DataFieldCompanyID from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();
        //                 YearData = dLayer.ExecuteScalar("select X_DataFieldYearID from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();
        //                 FieldName = dLayer.ExecuteScalar("select X_Text from vw_WebReportMenus where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID and N_LanguageId=1", Params, connection).ToString();
        //                 UserData = dLayer.ExecuteScalar("select X_DataFieldUserID from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();
        //                 FieldName = FieldName + "=";

        //                 if (xOperator == null || xOperator == "")
        //                     xOperator = "=";

        //                 if (x_Reporttitle != "")
        //                     x_Reporttitle += ", ";

        //                 if (type.ToLower() == "datepicker")
        //                 {
        //                     DateTime dateFrom = Convert.ToDateTime(value);
        //                     DateTime dateTo = Convert.ToDateTime(valueTo);
        //                     string procParam = "";
        //                     if (dateFrom != null && (bRange && dateTo != null))
        //                     {
        //                         x_Reporttitle = x_Reporttitle + FieldName + dateFrom.ToString("dd-MMM-yyyy") + " - " + dateTo.ToString("dd-MMM-yyyy");
        //                         x_comments = dateFrom.ToString("dd-MMM-yyyy") + " to " + dateTo.ToString("dd-MMM-yyyy");
        //                         procParam = dateFrom.ToString("dd-MMM-yyyy") + "|" + dateTo.ToString("dd-MMM-yyyy") + "|";
        //                     }
        //                     else if (dateFrom != null && !bRange)
        //                     {
        //                         x_Reporttitle = x_Reporttitle + FieldName + dateFrom.ToString("dd-MMM-yyyy");
        //                         x_comments = dateFrom.ToString("dd-MMM-yyyy");
        //                         procParam = dateFrom.ToString("dd-MMM-yyyy");
        //                     }
        //                     else if (bRange && dateTo != null)
        //                     {
        //                         x_Reporttitle = x_Reporttitle + FieldName + dateTo.ToString("dd-MMM-yyyy");
        //                         x_comments = dateTo.ToString("dd-MMM-yyyy");
        //                         procParam = dateTo.ToString("dd-MMM-yyyy") + "|";
        //                     }

        //                     if (xProCode != "")
        //                     {

        //                         SortedList mParamsList = new SortedList()
        //                     {
        //                     {"N_CompanyID",nCompanyID},
        //                     {"N_FnYearID",FnYearID},
        //                     {"N_PeriodID",0},
        //                     {"X_Code",xProCode},
        //                     {"X_Parameter", procParam },
        //                     {"N_UserID",myFunctions.GetUserID(User)},
        //                     {"N_BranchID",0}
        //                     };
        //                         dLayer.ExecuteDataTablePro("SP_OpeningBalanceGenerate", mParamsList, connection);

        //                     }
        //                     string DateCrt = "";
        //                     if (xFeild != "")
        //                     {
        //                         if (bRange)
        //                         {
        //                             DateCrt = xFeild + " >= Date('" + dateFrom.Year + "," + dateFrom.Month + "," + dateFrom.Day + "') And " + xFeild + " <= Date('" + dateTo.Year + "," + dateTo.Month + "," + dateTo.Day + "') ";
        //                         }
        //                         else
        //                         {
        //                             DateCrt = xFeild + " " + xOperator + " Date('" + dateFrom.Year + "," + dateFrom.Month + "," + dateFrom.Day + "') ";
        //                         }
        //                         Criteria = Criteria == "" ? DateCrt : Criteria + " and " + DateCrt;
        //                     }
        //                 }
        //                 else
        //                 {
        //                     if (xFeild != "")
        //                     {
        //                         if (xFeild.Contains("#"))
        //                             Criteria = Criteria == "" ? xFeild.Replace("#", value) : Criteria + " and " + xFeild.Replace("#", value);
        //                         else
        //                             Criteria = Criteria == "" ? xFeild + " " + xOperator + " '" + value + "' " : Criteria + " and " + xFeild + " " + xOperator + " '" + value + "' ";
        //                     }
        //                     x_Reporttitle = x_Reporttitle + FieldName + value;
        //                 }


        //                 //{table.fieldname} in {?Start date} to {?End date}
        //             }
        //             if (Criteria == "" && CompanyData != "")
        //             {
        //                 Criteria = Criteria + CompanyData + "=" + nCompanyID;
        //                 if (YearData != "")
        //                     Criteria = Criteria + " and " + YearData + "=" + FnYearID;
        //             }
        //             else if (CompanyData != "")
        //             {
        //                 Criteria = Criteria + " and " + CompanyData + "=" + nCompanyID;
        //                 if (YearData != "")
        //                     Criteria = Criteria + " and " + YearData + "=" + FnYearID;
        //             }
        //             if (UserData != "")
        //             {
        //                 Criteria = Criteria + " and " + UserData + "=" + nUserID;
        //             }
        //             dbName = connection.Database;
        //         }


        //         var handler = new HttpClientHandler
        //         {
        //             ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
        //         };
        //         var client = new HttpClient(handler);
        //         var random = RandomString();
        //         //HttpClient client = new HttpClient(clientHandler);

        //         var rptArray = reportName.Split(@"\");
        //         string actReportLocation = reportLocation;
        //         if (rptArray.Length > 1)
        //         {
        //             reportName = rptArray[1].ToString();
        //             actReportLocation = actReportLocation + rptArray[0].ToString() + "/";
        //         }


        //         string URL = reportApi + "api/report?reportName=" + reportName + "&critiria=" + Criteria + "&path=" + this.TempFilesPath + "&reportLocation=" + actReportLocation + "&dbval=" + dbName + "&random=" + random + "&x_comments=" + x_comments + "&x_Reporttitle=" + x_Reporttitle + "&extention=" + Extention;//+ connectionString;
        //         var path = client.GetAsync(URL);

        //         path.Wait();
        //         return Ok(_api.Success(new SortedList() { { "FileName", reportName.Trim() + random + "." + Extention } }));
        //         //string RptPath = reportPath + reportName.Trim() + ".pdf";
        //         // var memory = new MemoryStream();

        //         // using (var stream = new FileStream(RptPath, FileMode.Open))
        //         // {
        //         //     await stream.CopyToAsync(memory);
        //         // }
        //         // memory.Position = 0;
        //         // return File(memory, _api.GetContentType(RptPath), Path.GetFileName(RptPath));
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(_api.Error(User, e));
        //     }
        // }


        // private void LoadSelectionFormulae(int nFnYearID,int nBranchID,bool bAllBranchData)
        // {

        //     X_SelectionFormula = "";
        //     if (myFunctions.GetCompanyID(User) != 0)
        //     {
        //         X_SelectionFormula = X_CompanyField + " = " + myFunctions.GetCompanyID(User);
        //         if (X_YearField != "")
        //             X_SelectionFormula += " and (" + X_YearField + " = " + nFnYearID + ")";
        //         if (X_UserField != "")
        //             X_SelectionFormula += " and (" + X_UserField + " = " + myFunctions.GetUserID(User) + ")";
        //         if (X_BranchField != "")
        //         {
        //             if (bAllBranchData == false)
        //                 X_SelectionFormula += " and (" + X_BranchField  + " = " + nBranchID + ")";
        //         }


        //     }
        //     else if (X_YearField != "")
        //         X_SelectionFormula = "(" + X_YearField + " = " + nFnYearID + ")";

        //     if (flxListFilter.Rows >= 2)
        //     {
        //         for (int i = 1; i < flxListFilter.Rows; i++)
        //         {
        //             if (flxListFilter.get_TextMatrix(i, mcF_BRange).ToLower() == "true")
        //             {
        //                 if ((flxListFilter.get_TextMatrix(i, mcF_Filter1) == "" && flxListFilter.get_TextMatrix(i, mcF_Filter2) == "") || flxListFilter.get_TextMatrix(i, mcF_DataField) == "")
        //                 {
        //                     if (flxListFilter.get_TextMatrix(i, mcF_FieldType) == "Date")
        //                     {
        //                         ////X_ProcParameter = MYG.DateToDB(flxListFilter.get_TextMatrix(i, mcF_Filter1)) + "|" + MYG.DateToDB(flxListFilter.get_TextMatrix(i, mcF_Filter2)) + "|";
        //                         ////X_ProcParameter =  myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter1)).ToString(myCompanyID._SystemDateFormat, myCompanyID._EnglishCulture) + "|" +  myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter2)).ToString(myCompanyID._SystemDateFormat, myCompanyID._EnglishCulture) + "|";
        //                         //X_ProcParameter = myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter1)) + "|" + myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter2)) + "|";
        //                         if (flxListFilter.get_TextMatrix(i, mcF_Filter1) == "")
        //                             X_ProcParameter = " |";
        //                         else
        //                         {
        //                             X_ProcParameter = myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter1)) + "|";
        //                             X_Rptcomments = myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter1));

        //                         }
        //                         if (flxListFilter.get_TextMatrix(i, mcF_Filter2) == "")
        //                             X_ProcParameter = X_ProcParameter + " |";
        //                         else
        //                         {
        //                             X_ProcParameter = X_ProcParameter + myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter2)) + "|";
        //                             if (myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter2)) != myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter1)))
        //                             X_Rptcomments = X_Rptcomments + " to " + myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter2));
        //                         }
        //                     }
        //                     continue;
        //                 }
        //                 else
        //                 {
        //                     //if (X_SelectionFormula != "")
        //                     //    X_SelectionFormula += " And ";
        //                     if (flxListFilter.get_TextMatrix(i, mcF_FieldType) == "Date")
        //                     {
        //                         // //X_SelectionFormula += flxListFilter.get_TextMatrix(i, mcF_DataField) + " " + flxListFilter.get_TextMatrix(i, mcF_Operator1) + " Date('" + Convert.ToDateTime(flxListFilter.get_TextMatrix(i, mcF_Filter1)).Year.ToString() + "," + Convert.ToDateTime(flxListFilter.get_TextMatrix(i, mcF_Filter1)).Month.ToString() + "," + Convert.ToDateTime(flxListFilter.get_TextMatrix(i, mcF_Filter1)).Day.ToString() + "') And " + flxListFilter.get_TextMatrix(i, mcF_DataField) + " " + flxListFilter.get_TextMatrix(i, mcF_Operator2) + " Date('" + Convert.ToDateTime(flxListFilter.get_TextMatrix(i, mcF_Filter2)).Year.ToString() + "," + Convert.ToDateTime(flxListFilter.get_TextMatrix(i, mcF_Filter2)).Month.ToString() + "," + Convert.ToDateTime(flxListFilter.get_TextMatrix(i, mcF_Filter2)).Day.ToString() + "') ";
        //                         // //X_ProcParameter = MYG.DateToDB(flxListFilter.get_TextMatrix(i, mcF_Filter1)) + "|" + MYG.DateToDB(flxListFilter.get_TextMatrix(i, mcF_Filter2)) + "|";
        //                         //X_SelectionFormula += flxListFilter.get_TextMatrix(i, mcF_DataField) + " " + flxListFilter.get_TextMatrix(i, mcF_Operator1) + " Date('" + myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter1)).Year.ToString() + "," + myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter1)).Month.ToString() + "," + myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter1)).Day.ToString() + "') And " + flxListFilter.get_TextMatrix(i, mcF_DataField) + " " + flxListFilter.get_TextMatrix(i, mcF_Operator2) + " Date('" + myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter2)).Year.ToString() + "," + myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter2)).Month.ToString() + "," + myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter2)).Day.ToString() + "') ";
        //                         if (flxListFilter.get_TextMatrix(i, mcF_Filter1) != "")
        //                         {
        //                             if (X_SelectionFormula != "")
        //                                 X_SelectionFormula += " And ";
        //                             X_SelectionFormula += flxListFilter.get_TextMatrix(i, mcF_DataField) + " " + flxListFilter.get_TextMatrix(i, mcF_Operator1) + " Date('" + myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter1)).Year.ToString() + "," + myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter1)).Month.ToString() + "," + myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter1)).Day.ToString() + "')";
        //                             X_Rptcomments = myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter1));
        //                         }
        //                         if (flxListFilter.get_TextMatrix(i, mcF_Filter2) != "")
        //                         {
        //                             if (X_SelectionFormula != "")
        //                                 X_SelectionFormula += " And ";
        //                             X_SelectionFormula += flxListFilter.get_TextMatrix(i, mcF_DataField) + " " + flxListFilter.get_TextMatrix(i, mcF_Operator2) + " Date('" + myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter2)).Year.ToString() + "," + myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter2)).Month.ToString() + "," + myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter2)).Day.ToString() + "') ";
        //                             if (myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter2)) != myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter1)))
        //                                 X_Rptcomments = X_Rptcomments + " to " + myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter2));

        //                         }

        //                        // //X_ProcParameter = myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter1)).ToString(myCompanyID._SystemDateFormat, myCompanyID._EnglishCulture) + "|" + myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter2)).ToString(myCompanyID._SystemDateFormat, myCompanyID._EnglishCulture) + "|";
        //                         //X_ProcParameter = myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter1)) + "|" + myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter2)) + "|";
        //                         if (flxListFilter.get_TextMatrix(i, mcF_Filter1) != "")
        //                             X_ProcParameter = myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter1)) + "|";
        //                         else
        //                             X_ProcParameter = " |";

        //                         if (flxListFilter.get_TextMatrix(i, mcF_Filter2) != "")
        //                             X_ProcParameter += myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter2)) + "|";
        //                         else
        //                             X_ProcParameter +=  " |";
        //                     }
        //                     else if (flxListFilter.get_TextMatrix(i, mcF_FieldType) == "Text")
        //                     {
        //                         if (X_SelectionFormula != "")
        //                             X_SelectionFormula += " And ";
        //                         X_SelectionFormula += flxListFilter.get_TextMatrix(i, mcF_DataField) + " " + flxListFilter.get_TextMatrix(i, mcF_Operator1) + "'" + flxListFilter.get_TextMatrix(i, mcF_Filter1) + "' And " + flxListFilter.get_TextMatrix(i, mcF_DataField) + " " + flxListFilter.get_TextMatrix(i, mcF_Operator2) + " '" + flxListFilter.get_TextMatrix(i, mcF_Filter2) + "' ";
        //                     }
        //                     else
        //                     {
        //                         if (X_SelectionFormula != "")
        //                             X_SelectionFormula += " And ";
        //                         X_SelectionFormula += flxListFilter.get_TextMatrix(i, mcF_DataField) + flxListFilter.get_TextMatrix(i, mcF_Operator1) + flxListFilter.get_TextMatrix(i, mcF_Filter1) + " And " + flxListFilter.get_TextMatrix(i, mcF_DataField) + " " + flxListFilter.get_TextMatrix(i, mcF_Operator2) + " " + flxListFilter.get_TextMatrix(i, mcF_Filter2) + " ";
        //                     }
        //                 }
        //             }
        //             else
        //             {
        //                 if (flxListFilter.get_TextMatrix(i, mcF_Filter1) == "" || flxListFilter.get_TextMatrix(i, mcF_DataField) == "")
        //                 {
        //                     if (flxListFilter.get_TextMatrix(i, mcF_FieldType) == "Date")
        //                     {
        //                         ////X_ProcParameter = MYG.DateToDB(flxListFilter.get_TextMatrix(i, mcF_Filter1));
        //                         ////X_ProcParameter = myFunctions.GetFormatedDate(flxListFilter.get_TextMatrix(i, mcF_Filter1)).ToString(myCompanyID._SystemDateFormat, myCompanyID._EnglishCulture);                                
        //                         //X_ProcParameter = myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter1));
        //                         if (flxListFilter.get_TextMatrix(i, mcF_Filter1) != "")
        //                             X_ProcParameter = myFunctions.GetFormatedDate_Ret_string(flxListFilter.get_TextMatrix(i, mcF_Filter1));
        //                     }
        //                     continue;
        //                 }
        //                 else
        //                 {
        //                     if (X_SelectionFormula != "")
        //                         X_SelectionFormula += " And ";
        //                     if (flxListFilter.get_TextMatrix(i, mcF_FieldType) == "Date")
        //                     {
        //                         if(flxListFilter.get_TextMatrix(i, mcF_Filter1)!="")
        //                             X_SelectionFormula += flxListFilter.get_TextMatrix(i, mcF_DataField) + " " + flxListFilter.get_TextMatrix(i, mcF_Operator1) + " Date('" + Convert.ToDateTime(flxListFilter.get_TextMatrix(i, mcF_Filter1)).Year.ToString() + "," + Convert.ToDateTime(flxListFilter.get_TextMatrix(i, mcF_Filter1)).Month.ToString() + "," + Convert.ToDateTime(flxListFilter.get_TextMatrix(i, mcF_Filter1)).Day.ToString() + "') ";
        //                     }
        //                     else if (flxListFilter.get_TextMatrix(i, mcF_FieldType) == "Text")
        //                     {
        //                         if (myFunctions.getBoolVAL(flxListFilter.get_TextMatrix(i, mcF_EnableMultiselect)))
        //                         {
        //                             string[] temp = flxListFilter.get_TextMatrix(i, mcF_Filter1).Split(',');
        //                             X_SelectionFormula += " (";
        //                             for (int w = 0; w < temp.Length; w++)
        //                             {
        //                                 if (w != 0)
        //                                     X_SelectionFormula += " OR ";
        //                                 X_SelectionFormula += flxListFilter.get_TextMatrix(i, mcF_DataField) + " " + flxListFilter.get_TextMatrix(i, mcF_Operator1) + "'" + temp[w] + "' ";
        //                             }
        //                             X_SelectionFormula += " )";
        //                         }
        //                         else
        //                             X_SelectionFormula += flxListFilter.get_TextMatrix(i, mcF_DataField) + " " + flxListFilter.get_TextMatrix(i, mcF_Operator1) + "'" + flxListFilter.get_TextMatrix(i, mcF_Filter1) + "' ";
        //                     }
        //                     else
        //                     {
        //                         if (flxListFilter.get_TextMatrix(i, mcF_DataField).Contains("#"))
        //                         {
        //                             X_SelectionFormula += flxListFilter.get_TextMatrix(i, mcF_DataField).Replace("#", flxListFilter.get_TextMatrix(i, mcF_Filter1));
        //                         }
        //                         else
        //                             X_SelectionFormula += flxListFilter.get_TextMatrix(i, mcF_DataField) + flxListFilter.get_TextMatrix(i, mcF_Operator1) + flxListFilter.get_TextMatrix(i, mcF_Filter1) + " ";
        //                     }
        //                 }
        //             }
        //         }
        //     }

        // }

        private static Random random = new Random();
        public string RandomString(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost("sendmessage")]
        public IActionResult SendMessage([FromBody] DataSet ds)
        {
            DataTable dt = new DataTable();
            dt = ds.Tables["master"];
            SortedList QueryParams = new SortedList();
            DataColumnCollection columns = dt.Columns;
            int nCompanyId = myFunctions.GetCompanyID(User);
            string Company = myFunctions.GetCompanyName(User);
            string x_Mobile = "";
            string body = "";
            //var client = new WebClient();
            var content = "";
            string URL = "";
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };
            var client = new HttpClient(handler);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                SqlTransaction transaction;
                transaction = connection.BeginTransaction();
                object Currency = dLayer.ExecuteScalar("select x_currency from acc_company  where n_companyid=" + nCompanyId, QueryParams, connection, transaction);
                if (columns.Contains("n_ServiceID"))
                {
                    x_Mobile = "+" + dt.Rows[0]["x_MobileNo"].ToString();
                    DateTime deldate = Convert.ToDateTime(dt.Rows[0]["d_Deliverydate"].ToString());
                    body = "Dear " + dt.Rows[0]["x_CustomerName"].ToString() + ",%0A%0AThe *Repair Order* for your Device is *" + dt.Rows[0]["x_ServiceCode"].ToString() + "* opened on " + dt.Rows[0]["d_Entrydate"].ToString() + ".%0A%0AEstimated time of delivery (ETD) is " + deldate.ToString("dd/MM/yyyy") + " and estimated amount is " + dt.Rows[0]["n_BillAmountF"].ToString() + " " + Currency + " %0A%0ARegards, %0A" + dt.Rows[0]["x_UserName"].ToString();
                    URL = "https://api.textmebot.com/send.php?recipient=" + x_Mobile + "&apikey=wnmyMLo9QV2K&text=" + body;
                    var path1 = client.GetAsync(URL);
                    path1.Wait();

                }
                object x_mobilenumber = dLayer.ExecuteScalar("select X_PhoneNo1 from Inv_Customer where n_companyid=" + nCompanyId + " and n_fnyearid=" + dt.Rows[0]["n_fnyearid"].ToString() + " and N_CustomerID=" + dt.Rows[0]["n_CustomerID"].ToString(), QueryParams, connection, transaction);
                double TotalAmt = myFunctions.getVAL(dt.Rows[0]["n_BillAmtF"].ToString()) - myFunctions.getVAL(dt.Rows[0]["n_DiscountDisplay"].ToString()) + myFunctions.getVAL(dt.Rows[0]["n_TaxAmtF"].ToString()) - myFunctions.getVAL(dt.Rows[0]["n_DiscountAmtF"].ToString());
                x_Mobile = "+" + x_mobilenumber.ToString();
                body = "Dear " + dt.Rows[0]["x_CustomerName"].ToString() + ",%0A%0A*_Thank you for your purchase._*%0A%0ADoc No : " + dt.Rows[0]["x_ReceiptNo"].ToString() + "%0ATotal Amount : " + dt.Rows[0]["n_BillAmtF"].ToString() + "%0ADiscount : " + dt.Rows[0]["n_DiscountDisplay"].ToString() + "%0AVAT Amount : " + dt.Rows[0]["n_TaxAmtF"].ToString() + "%0ARound Off : " + dt.Rows[0]["n_DiscountAmtF"].ToString() + "%0ANet Amount : " + TotalAmt + " " + Currency + " %0A%0ARegards, %0A" + Company;
                URL = "https://api.textmebot.com/send.php?recipient=" + x_Mobile + "&apikey=KjFdsG2hRjfK&text=" + body;
                var path = client.GetAsync(URL);
                path.Wait();
                //content = client.DownloadString("https://api.textmebot.com/send.php?recipient=" + x_Mobile + "&apikey=KjFdsG2hRjfK&text=" + body);
                return Ok(_api.Success("Message Sent"));


                //content = client.DownloadString("https://api.textmebot.com/send.php?recipient=" + x_Mobile + "&apikey=wnmyMLo9QV2K&text=" + body);

            }

            return Ok(_api.Success("Message Sent"));

        }



    }
}