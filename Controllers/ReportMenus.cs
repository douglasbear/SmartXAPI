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
    public class ReportMenus : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly string reportApi;
        private readonly string reportPath;
        private readonly string reportLocation;

        public ReportMenus(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
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

                    string sqlCommandText1 = "select n_CompID,n_LanguageId,n_MenuID,x_CompType,x_FieldList,x_FieldType,x_Text,X_FieldtoReturn,X_DefVal1,X_DefVal2,X_Operator from vw_WebReportMenus where N_LanguageId=@nLangId group by n_CompID,n_LanguageId,n_MenuID,x_CompType,x_FieldList,x_FieldType,x_Text,X_FieldtoReturn,X_DefVal1,X_DefVal2,X_Operator";
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
        public ActionResult GetDynamicList(int nMenuId, int nCompId, int nLangId, string cval, string bval, string fval)
        {
            DataTable dt = new DataTable();
            DataTable outTable = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select TOP 1 X_TableName,X_FieldList,X_Criteria from vw_WebReportMenus where N_MenuID=@p1 and N_LanguageId=@p2 and N_CompID=@p3 and X_CompType=@p4";
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
                        ListSqlParams.Add("@BVal", bval);
                        ListSqlParams.Add("@CVal", cval);
                        ListSqlParams.Add("@FVal", fval);
                        string ListSql = "select " + fields + " from " + table + " " + Criteria;

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
                    string URL = reportApi + "/api/report?reportName=" + reportName + "&critiria=" + critiria + "&path=" + reportPath + "&reportLocation=" + reportLocation + "&dbval=" + dbName + "&random=" + random;
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
        public IActionResult GetModulePrint(int nFormID, int nPkeyID)
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

                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                    };
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
                        RPTLocation = reportLocation + "printing/SalesOrder/vat/";
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
                        RPTLocation = reportLocation + "printing/salesinvoice/vat/";
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
                        RPTLocation = reportLocation + "printing/";
                        ReportName = "CustomerReceiptVoucher";
                    }
                    //Purchase Module
                    if (nFormID == 65)
                    {
                        critiria = "{vw_InvPurchaseDetailsView_Rpt.N_PurchaseId}=" + nPkeyID;
                        RPTLocation = reportLocation + "printing/PurchaseInvoice/vat/";
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
                        RPTLocation = reportLocation + "printing/PurchaseOrder/vat/";
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
                        RPTLocation = reportLocation + "printing/";
                        ReportName = "VendorPaymentVoucher";
                    }
                    //Finance Module
                    if (nFormID == 44)
                    {
                        critiria = "{vw_AccVoucherJrnlCC.X_TransType}='PV' and {vw_AccVoucherJrnlCC.N_VoucherID}=" + nPkeyID;
                        RPTLocation = reportLocation + "printing/";
                        ReportName = "PaymentVoucher_VAT";
                    }
                    if (nFormID == 45)
                    {
                        critiria = "{vw_AccVoucherJrnlCC.X_TransType}='RV' and {vw_AccVoucherJrnlCC.N_VoucherID}=" + nPkeyID;
                        RPTLocation = reportLocation + "printing/";
                        ReportName = "ReceiptVoucher_VAT";
                    }
                    if (nFormID == 46)
                    {
                        critiria = "{vw_AccVoucherJrnlCC.X_TransType}='JV' and {vw_AccVoucherJrnlCC.N_VoucherID}=" + nPkeyID;
                        RPTLocation = reportLocation + "printing/";
                        ReportName = "JournalVoucher_VAT";
                    }


                    var client = new HttpClient(handler);
                    var dbName = connection.Database;
                    var random = RandomString();
                    string URL = reportApi + "/api/report?reportName=" + ReportName + "&critiria=" + critiria + "&path=" + reportPath + "&reportLocation=" + RPTLocation + "&dbval=" + dbName + "&random=" + random;
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

        [HttpPost("getModuleReport")]
        public IActionResult GetModuleReports([FromBody] DataSet ds)
        {
            DataTable MasterTable;
            DataTable DetailTable;

            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            int nCompanyID = myFunctions.GetCompanyID(User);
            string x_comments="";

            try
            {
                String Criteria = "";
                String reportName = "";
                String CompanyData = "";
                String YearData = "";

                var dbName = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //int MenuID = myFunctions.getIntVAL(MasterTable.Rows[0]["moduleID"].ToString());
                    int MenuID = myFunctions.getIntVAL(MasterTable.Rows[0]["reportCategoryID"].ToString());
                    int ReportID = myFunctions.getIntVAL(MasterTable.Rows[0]["reportID"].ToString());
                    int FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["nFnYearID"].ToString());

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
                        Params.Add("@xType", type);
                        Params.Add("@nCompID", compID);
                        Params.Add("@xMain", "MainForm");
                        string xFeild = dLayer.ExecuteScalar("select X_DataField from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID", Params, connection).ToString();
                        string xProCode = dLayer.ExecuteScalar("select X_ProcCode from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();
                        CompanyData = dLayer.ExecuteScalar("select X_DataFieldCompanyID from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();
                        YearData = dLayer.ExecuteScalar("select X_DataFieldYearID from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xMain", Params, connection).ToString();



                        if (type == "datepicker")
                        {
                            DateTime dateFrom = Convert.ToDateTime(value);
                            DateTime dateTo = Convert.ToDateTime(valueTo);
                            if (xProCode != "")
                            {
                                if(dateFrom!=null && dateTo!=null)
                                    x_comments=dateFrom.ToString("dd-MMM-yyyy") + "to" + dateTo.ToString("dd-MMM-yyyy");
                                else if(dateFrom!=null)
                                    x_comments=dateFrom.ToString("dd-MMM-yyyy");
                                else if(dateFrom!=null)
                                    x_comments=dateTo.ToString("dd-MMM-yyyy");
                                
                                SortedList mParamsList = new SortedList()
                            {
                            {"N_CompanyID",nCompanyID},
                            {"N_FnYearID",FnYearID},
                            {"N_PeriodID",0},
                            {"X_Code",xProCode},
                            {"X_Parameter", dateFrom.ToString("dd-MMM-yyyy")+"|"+dateTo.ToString("dd-MMM-yyyy")+"|"},
                            {"N_UserID",myFunctions.GetUserID(User)},
                            {"N_BranchID",0}
                            };
                                dLayer.ExecuteDataTablePro("SP_OpeningBalanceGenerate", mParamsList, connection);

                            }
                            string DateCrt = "";
                            if (xFeild != "")
                            {
                                DateCrt = xFeild + " >= Date('" + dateFrom.Year + "," + dateFrom.Month + "," + dateFrom.Day + "') And " + xFeild + " <= Date('" + dateTo.Year + "," + dateTo.Month + "," + dateTo.Day + "') ";
                                Criteria = Criteria == "" ? DateCrt : Criteria + " and " + DateCrt;
                            }
                        }
                        else
                        {
                            if (xFeild != "")
                            {
                                Criteria = Criteria == "" ? xFeild + "='" + value + "' " : Criteria + " and " + xFeild + "='" + value + "' ";
                            }
                        }


                        //{table.fieldname} in {?Start date} to {?End date}
                    }
                    if(Criteria=="")
                        Criteria=Criteria + CompanyData +"="+nCompanyID+" and "+YearData+"="+FnYearID;
                    else
                        Criteria= Criteria +" and "+ CompanyData +"="+nCompanyID+" and "+YearData+"="+FnYearID;
                    dbName = connection.Database;
                }
                

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                };
                var client = new HttpClient(handler);
                var random = RandomString();
                //HttpClient client = new HttpClient(clientHandler);
                string URL = reportApi + "/api/report?reportName=" + reportName + "&critiria=" + Criteria + "&path=" + reportPath + "&reportLocation=" + reportLocation + "&dbval=" + dbName + "&random=" + random+"&x_comments="+x_comments;//+ connectionString;
                var path = client.GetAsync(URL);

                path.Wait();
                return Ok(_api.Success(new SortedList() { { "FileName", reportName.Trim() + random + ".pdf" } }));
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
        

        private static Random random = new Random();
        public string RandomString(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }



    }
}