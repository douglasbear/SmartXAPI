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
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

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
        private readonly string connectionString;
        private readonly string reportApi;
        private readonly string reportPath;
        private readonly string reportLocation;
        // private string X_CompanyField = "", X_YearField = "", X_BranchField="", X_UserField="",X_DefReportFile = "", X_GridPrevVal = "", X_SelectionFormula = "", X_ProcName = "", X_ProcParameter = "", X_ReprtTitle = "",X_Operator="";
        public Reports(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            reportApi = conf.GetConnectionString("ReportAPI");
            reportPath = conf.GetConnectionString("ReportPath");
            reportLocation = conf.GetConnectionString("ReportLocation");
        }
        [HttpGet("list")]
        public ActionResult GetReportList(int? nMenuId, int? nLangId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "Select vwUserMenus.*,Lan_MultiLingual.X_Text from vwUserMenus Inner Join Sec_UserPrevileges On vwUserMenus.N_MenuID=Sec_UserPrevileges.N_MenuID And Sec_UserPrevileges.N_UserCategoryID = vwUserMenus.N_UserCategoryID And  Sec_UserPrevileges.N_UserCategoryID=@nUserCatID and vwUserMenus.B_Show=1 inner join Lan_MultiLingual on vwUserMenus.N_MenuID=Lan_MultiLingual.N_FormID and Lan_MultiLingual.N_LanguageId=@nLangId and X_ControlNo ='0' Where LOWER(vwUserMenus.X_Caption) <>'seperator' and vwUserMenus.N_ParentMenuID=@nMenuId Order By vwUserMenus.N_Order";
            Params.Add("@nMenuId", nMenuId == 0 ? 318 : nMenuId);
            Params.Add("@nLangId", nLangId);
            Params.Add("@nUserCatID", myFunctions.GetUserCategory(User));

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    DataTable dt1 = new DataTable();

                    string sqlCommandText1 = "select n_CompID,n_LanguageId,n_MenuID,x_CompType,x_FieldList,x_FieldType,x_Text,X_FieldtoReturn,X_DefVal1,X_DefVal2,X_Operator,N_LinkCompID,X_LinkField,B_Range from vw_WebReportMenus where N_LanguageId=@nLangId group by n_CompID,n_LanguageId,n_MenuID,x_CompType,x_FieldList,x_FieldType,x_Text,X_FieldtoReturn,X_DefVal1,X_DefVal2,X_Operator,N_ListOrder,N_LinkCompID,X_LinkField,B_Range order by N_ListOrder";
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
                        catch
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
                return Ok(_api.Error(e));
            }
        }


        [HttpGet("dynamiclist")]
        public ActionResult GetDynamicList(int nMenuId, int nCompId, int nLangId, string cval, string bval, string fval, string qry)
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
                return Ok(_api.Error(e));
            }
        }

        // [HttpGet("getreport")]
        // public async Task<IActionResult> GetReport(string reportName, string critiria)
        // {
        //     //var client = new HttpClient();

        //     var handler = new HttpClientHandler
        //     {
        //         ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
        //     };
        //     var client = new HttpClient(handler);
        //     //HttpClient client = new HttpClient(clientHandler);

        //     var path = client.GetAsync("https://localhost:4439/api/report?reportname=" + reportName + "&critiria=" + critiria + "&con=" + connectionString);

        //     path.Wait();
        //     string ReportPath = "C:\\" + reportName.Trim() + ".pdf";
        //     var memory = new MemoryStream();

        //     using (var stream = new FileStream(ReportPath, FileMode.Open))
        //     {
        //         await stream.CopyToAsync(memory);
        //     }
        //     memory.Position = 0;
        //     return File(memory, _api.GetContentType(ReportPath), Path.GetFileName(ReportPath));



        //     // ReportPath="C:\\"+ reportName + ".pdf";
        //     // Stream fileStream = System.IO.File.Open(ReportPath, FileMode.Open);
        //     // if(fileStream==null){return StatusCode(403,"Report Generation Error");}
        //     // return File(fileStream, "application/octet-stream",reportName+".pdf");
        // }


        [HttpGet("getreport")]
        public IActionResult GetModuleReports(string reportName, string critiria)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                };
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();

                    var client = new HttpClient(handler);
                    var random = RandomString();
                    var dbName = connection.Database;
                    string URL = reportApi + "/api/report?reportName=" + reportName + "&critiria=" + critiria + "&path=" + reportPath + "&reportLocation=" + reportLocation + "&dbval=" + dbName + "&random=" + random +"&x_comments=&x_Reporttitle=&extention=pdf";;
                    var path = client.GetAsync(URL);
                    path.Wait();
                    return Ok(_api.Success(new SortedList() { { "FileName", reportName.Trim() + random + ".pdf" } }));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("getscreenprint")]
        public IActionResult GetModulePrint(int nFormID, int nPkeyID, int nFnYearID)
        {
            string RPTLocation = reportLocation;
            string ReportName = "";
            string critiria = "";
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
                    QueryParams.Add("@p2", nFormID);
                    QueryParams.Add("@p3", nFnYearID);
                    string TableName = "";
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                    };

                    object ObjPath = dLayer.ExecuteScalar("SELECT Acc_TaxType.X_RepPathCaption FROM Acc_TaxType LEFT OUTER JOIN Acc_FnYear ON Acc_TaxType.N_TypeID = Acc_FnYear.N_TaxType where Acc_FnYear.N_CompanyID=@p1 and Acc_FnYear.N_FnYearID=@p3", QueryParams, connection, transaction);
                    string TaxType = ObjPath + "/";
                    if (nFormID == 80)
                    {
                        critiria = "{Inv_SalesQuotation.N_QuotationId}=" + nPkeyID;
                        RPTLocation = reportLocation + "printing/quotation/vat/";
                        object Template = dLayer.ExecuteScalar("SELECT X_Value FROM Gen_Settings WHERE N_CompanyID =@p1 AND X_Group = @p2 AND X_Description = 'PrintTemplate'", QueryParams, connection, transaction);
                        if (Template != null)
                        {
                            ReportName = Template.ToString();
                            ReportName = ReportName.Remove(ReportName.Length - 4);
                        }
                        else
                            ReportName = "Sales_Quatation";
                    }
                    if (nFormID == 81)
                    {
                        critiria = "{vw_InvSalesOrderDetails.N_SalesOrderId}=" + nPkeyID;
                        TableName = "vw_InvSalesOrderDetails";
                        RPTLocation = reportLocation + "printing/SalesOrder/" + TaxType;
                        object Template = dLayer.ExecuteScalar("SELECT X_Value FROM Gen_Settings WHERE N_CompanyID =@p1 AND X_Group = @p2 AND X_Description = 'PrintTemplate'", QueryParams, connection, transaction);
                        if (Template != null)
                        {
                            ReportName = Template.ToString();
                            ReportName = ReportName.Remove(ReportName.Length - 4);
                        }
                        else
                            ReportName = "Sales_order";
                    }
                    if (nFormID == 884)
                    {
                        critiria = "{vw_InvDeliveryNoteDetails.N_DeliveryNoteID}=" + nPkeyID;
                        TableName = "vw_InvDeliveryNoteDetails";

                        RPTLocation = reportLocation + "printing/deliverynote/vat/";
                        object Template = dLayer.ExecuteScalar("SELECT X_Value FROM Gen_Settings WHERE N_CompanyID =@p1 AND X_Group = @p2 AND X_Description = 'PrintTemplate'", QueryParams, connection, transaction);
                        if (Template != null)
                        {
                            ReportName = Template.ToString();
                            ReportName = ReportName.Remove(ReportName.Length - 4);
                        }
                        else
                            ReportName = "Sales_DeliveryNote";
                    }

                    if (nFormID == 64)
                    {
                        critiria = "{Inv_Sales.N_SalesId}=" + nPkeyID;
                        TableName = "Inv_Sales";

                        RPTLocation = reportLocation + "printing/salesinvoice/" + TaxType;
                        object Template = dLayer.ExecuteScalar("SELECT X_Value FROM Gen_Settings WHERE N_CompanyID =@p1 AND X_Group = @p2 AND X_Description = 'PrintTemplate' and N_UserCategoryID=2", QueryParams, connection, transaction);
                        if (Template != null)
                        {

                            ReportName = Template.ToString();
                            ReportName = ReportName.Remove(ReportName.Length - 4);
                        }
                        else
                            ReportName = "SalesInvoice";
                    }
                    if (nFormID == 55)
                    {
                        critiria = "{vw_InvSalesReturn_rpt.N_DebitNoteId}=" + nPkeyID;
                        TableName = "vw_InvSalesReturn_rpt";

                        RPTLocation = reportLocation + "printing/SalesReturn/vat/";
                        object Template = dLayer.ExecuteScalar("SELECT X_Value FROM Gen_Settings WHERE N_CompanyID =@p1 AND X_Group = @p2 AND X_Description = 'PrintTemplate' and N_UserCategoryID=2", QueryParams, connection, transaction);
                        if (Template != null)
                        {

                            ReportName = Template.ToString();
                            ReportName = ReportName.Remove(ReportName.Length - 4);
                        }
                        else
                            ReportName = "Sales_Return";
                    }
                    if (nFormID == 66)
                    {
                        critiria = "{vw_InvPartyBalance.N_AccType}=2 and {vw_InvCustomerPayment_rpt.N_PayReceiptId}=" + nPkeyID;
                        TableName = "vw_InvPartyBalance";

                        RPTLocation = reportLocation + "printing/";
                        ReportName = "CustomerReceiptVoucher";
                    }
                    //Purchase Module
                    if (nFormID == 65)
                    {
                        critiria = "{vw_InvPurchaseDetailsView_Rpt.N_PurchaseId}=" + nPkeyID;
                        TableName = "vw_InvPurchaseDetailsView_Rpt";

                        RPTLocation = reportLocation + "printing/PurchaseInvoice/" + TaxType;
                        object Template = dLayer.ExecuteScalar("SELECT X_Value FROM Gen_Settings WHERE N_CompanyID =@p1 AND X_Group = @p2 AND X_Description = 'PrintTemplate' and N_UserCategoryID=2", QueryParams, connection, transaction);
                        if (Template != null)
                        {
                            ReportName = Template.ToString();
                            ReportName = ReportName.Remove(ReportName.Length - 4);
                        }
                        else
                            ReportName = "PurchaseEntry_invoice";
                    }
                    if (nFormID == 82)
                    {
                        critiria = "{Inv_PurchaseOrder.N_POrderID}=" + nPkeyID;
                        TableName = "Inv_PurchaseOrder";

                        RPTLocation = reportLocation + "printing/PurchaseOrder/" + TaxType;
                        object Template = dLayer.ExecuteScalar("SELECT X_Value FROM Gen_Settings WHERE N_CompanyID =@p1 AND X_Group = @p2 AND X_Description = 'PrintTemplate' and N_UserCategoryID=2", QueryParams, connection, transaction);
                        if (Template != null)
                        {
                            ReportName = Template.ToString();
                            ReportName = ReportName.Remove(ReportName.Length - 4);
                        }
                        else
                            ReportName = "Purchase_order";
                    }
                    if (nFormID == 68)
                    {
                        critiria = "{Inv_PurchaseReturnMaster.N_CreditNoteId}=" + nPkeyID;
                        TableName = "Inv_PurchaseReturnMaster";

                        RPTLocation = reportLocation + "printing/PurchaseReturn/vat/";
                        object Template = dLayer.ExecuteScalar("SELECT X_Value FROM Gen_Settings WHERE N_CompanyID =@p1 AND X_Group = @p2 AND X_Description = 'PrintTemplate' and N_UserCategoryID=2", QueryParams, connection, transaction);
                        if (Template != null)
                        {
                            ReportName = Template.ToString();
                            ReportName = ReportName.Remove(ReportName.Length - 4);
                        }
                        else
                            ReportName = "Purchase_Return";
                    }
                    if (nFormID == 67)
                    {
                        critiria = "{vw_InvVendorPayment_rpt.N_PayReceiptId}=" + nPkeyID + " and {vw_InvPartyBalance.N_AccType}=1";
                        TableName = "vw_InvVendorPayment_rpt";

                        RPTLocation = reportLocation + "printing/";
                        ReportName = "VendorPaymentVoucher";
                    }
                    //Finance Module
                    if (nFormID == 44)
                    {
                        critiria = "{vw_AccVoucherJrnlCC.X_TransType}='PV' and {vw_AccVoucherJrnlCC.N_VoucherID}=" + nPkeyID;
                        TableName = "vw_AccVoucherJrnlCC";

                        RPTLocation = reportLocation + "printing/";
                        ReportName = "PaymentVoucher_VAT";
                    }
                    if (nFormID == 45)
                    {
                        critiria = "{vw_AccVoucherJrnlCC.X_TransType}='RV' and {vw_AccVoucherJrnlCC.N_VoucherID}=" + nPkeyID;
                        TableName = "vw_AccVoucherJrnlCC";

                        RPTLocation = reportLocation + "printing/";
                        ReportName = "ReceiptVoucher_VAT";
                    }
                    if (nFormID == 46)
                    {
                        critiria = "{vw_AccVoucherJrnlCC.X_TransType}='JV' and {vw_AccVoucherJrnlCC.N_VoucherID}=" + nPkeyID;
                        TableName = "vw_AccVoucherJrnlCC";

                        RPTLocation = reportLocation + "printing/";
                        ReportName = "JournalVoucher_VAT";
                    }
                     if (nFormID == 1229)//leave
                    {
                        critiria = "{Vw_VacationApprovalSlip_Rpt.N_VacationGroupID}=" + nPkeyID;
                        TableName = "Vw_VacationApprovalSlip_Rpt";

                        RPTLocation = reportLocation + "printing/";
                        ReportName = "Leaveapprovalslip";
                    }
                    if (nFormID == 1226)//loan
                    {
                        critiria = "{vw_AccVoucherJrnlCC.X_TransType}='JV' and {vw_AccVoucherJrnlCC.N_VoucherID}=" + nPkeyID;
                        TableName = "vw_AccVoucherJrnlCC";

                        RPTLocation = reportLocation + "printing/";
                        ReportName = "JournalVoucher_VAT";
                    }


                    var client = new HttpClient(handler);
                    var dbName = connection.Database;
                    var random = RandomString();
                    if (TableName != "" && critiria != "")
                    {
                        critiria = critiria + " and {" + TableName + ".N_CompanyID}=" + myFunctions.GetCompanyID(User);
                    }
                    string URL = reportApi + "/api/report?reportName=" + ReportName + "&critiria=" + critiria + "&path=" + reportPath + "&reportLocation=" + RPTLocation + "&dbval=" + dbName + "&random=" + random + "&x_comments=&x_Reporttitle=&extention=pdf";
                    var path = client.GetAsync(URL);
                    path.Wait();
                    return Ok(_api.Success(new SortedList() { { "FileName", ReportName.Trim() + random + ".pdf" } }));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
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
                            
                            critiria = "{vw_Pay_Empshiftdetails.D_PeriodFrom}>=Date('" + dPeriodFrom.Year + "," + dPeriodFrom.Month + "," + dPeriodFrom.Day + "') and {vw_Pay_Empshiftdetails.D_PeriodTo}<=Date('" + dPeriodTo.Year + "," + dPeriodTo.Month + "," + dPeriodTo.Day + "') and  {vw_Pay_Empshiftdetails.N_DepartmentID}=" + nDepartmentID + "";
                            TableName = "vw_Pay_Empshiftdetails";


                            RPTLocation = reportLocation + "printing";
                            ReportName = "Employee_ShiftSchedule";


                        }
                        else if (xCriteria == "all")
                        {
                            critiria = "{vw_Pay_Empshiftdetails.D_PeriodFrom}>=Date('" + dPeriodFrom.Year + "," + dPeriodFrom.Month + "," + dPeriodFrom.Day + "') and {vw_Pay_Empshiftdetails.D_PeriodTo}<=Date('" + dPeriodTo.Year + "," + dPeriodTo.Month + "," + dPeriodTo.Day + "')";
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
                        string URL = reportApi + "/api/report?reportName=" + ReportName + "&critiria=" + critiria + "&path=" + reportPath + "&reportLocation=" + RPTLocation + "&dbval=" + dbName + "&random=" + random + "&x_comments=&x_Reporttitle=&extention=pdf";
                        var path = client.GetAsync(URL);
                        path.Wait();
                        
                    }
                    return Ok(_api.Success(new SortedList() { { "FileName", ReportName.Trim() + random + ".pdf" } }));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }


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

            try
            {
                String Criteria = "";
                String reportName = "";
                String CompanyData = "";
                String YearData = "";
                String FieldName = "";
                String UserData = "";

                var dbName = "";
                string Extention="";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //int MenuID = myFunctions.getIntVAL(MasterTable.Rows[0]["moduleID"].ToString());
                    int MenuID = myFunctions.getIntVAL(MasterTable.Rows[0]["reportCategoryID"].ToString());
                    int ReportID = myFunctions.getIntVAL(MasterTable.Rows[0]["reportID"].ToString());
                    int FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["nFnYearID"].ToString());
                    Extention = MasterTable.Rows[0]["extention"].ToString();

                    SortedList Params1 = new SortedList();
                    Params1.Add("@nMenuID", MenuID);
                    Params1.Add("@xType", "RadioButton");
                    Params1.Add("@nCompID", ReportID);


                    reportName = dLayer.ExecuteScalar("select X_rptFile from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID and B_Active=1", Params1, connection).ToString();

                    reportName = reportName.Substring(0, reportName.Length - 4);


                    foreach (DataRow var in DetailTable.Rows)
                    {
                        int compID = myFunctions.getIntVAL(var["compId"].ToString());
                        string type = var["type"].ToString();
                        string value = var["value"].ToString();
                        string valueTo = var["valueTo"].ToString();

                        SortedList Params = new SortedList();
                        Params.Add("@nMenuID", MenuID);
                        Params.Add("@xType", type.ToLower());
                        Params.Add("@nCompID", compID);
                        Params.Add("@xMain", "MainForm");
                        string xFeild = dLayer.ExecuteScalar("select X_DataField from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID", Params, connection).ToString();
                        bool bRange = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select isNull(B_Range,0) from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID", Params, connection).ToString());
                        string xOperator = dLayer.ExecuteScalar("select isNull(X_Operator,'') from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID", Params, connection).ToString();
                        string xProCode = dLayer.ExecuteScalar("select X_ProcCode from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();
                        CompanyData = dLayer.ExecuteScalar("select X_DataFieldCompanyID from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();
                        YearData = dLayer.ExecuteScalar("select X_DataFieldYearID from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();
                        FieldName = dLayer.ExecuteScalar("select X_Text from vw_WebReportMenus where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID and N_LanguageId=1", Params, connection).ToString();
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
                            string procParam = "";
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

                            if (xProCode != "")
                            {

                                SortedList mParamsList = new SortedList()
                            {
                            {"N_CompanyID",nCompanyID},
                            {"N_FnYearID",FnYearID},
                            {"N_PeriodID",0},
                            {"X_Code",xProCode},
                            {"X_Parameter", procParam },
                            {"N_UserID",myFunctions.GetUserID(User)},
                            {"N_BranchID",0}
                            };
                                dLayer.ExecuteDataTablePro("SP_OpeningBalanceGenerate", mParamsList, connection);

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
                                    Criteria = Criteria == "" ? xFeild + " " + xOperator + " '" + value + "' " : Criteria + " and " + xFeild + " " + xOperator + " '" + value + "' ";
                            }
                            x_Reporttitle = x_Reporttitle + FieldName + value;
                        }


                        //{table.fieldname} in {?Start date} to {?End date}
                    }
                    if (Criteria == "" && CompanyData != "")
                    {
                        Criteria = Criteria + CompanyData + "=" + nCompanyID;
                        if (YearData != "")
                            Criteria = Criteria + " and " + YearData + "=" + FnYearID;
                    }
                    else if ( CompanyData != "")
                    {
                        Criteria = Criteria + " and " + CompanyData + "=" + nCompanyID;
                        if (YearData != "")
                            Criteria = Criteria + " and " + YearData + "=" + FnYearID;
                    }
                    if (UserData != "")
                    {
                        Criteria = Criteria + " and " + UserData + "=" + nUserID;
                    }
                    dbName = connection.Database;
                }


                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                };
                var client = new HttpClient(handler);
                var random = RandomString();
                //HttpClient client = new HttpClient(clientHandler);
                string URL = reportApi + "/api/report?reportName=" + reportName + "&critiria=" + Criteria + "&path=" + reportPath + "&reportLocation=" + reportLocation + "&dbval=" + dbName + "&random=" + random + "&x_comments=" + x_comments + "&x_Reporttitle=" + x_Reporttitle + "&extention="+Extention;//+ connectionString;
                var path = client.GetAsync(URL);

                path.Wait();
                return Ok(_api.Success(new SortedList() { { "FileName", reportName.Trim() + random + "."+Extention} }));
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
                return Ok(_api.Error(e));
            }
        }


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



    }
}