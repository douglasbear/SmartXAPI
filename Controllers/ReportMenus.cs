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

namespace SmartxAPI.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        public ReportMenus(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            reportApi = conf.GetConnectionString("ReportAPI");
            reportPath = conf.GetConnectionString("ReportPath");
        }
        [HttpGet("list")]
        public ActionResult GetReportList(int? nMenuId, int? nLangId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "Select vwUserMenus.*,Lan_MultiLingual.X_Text from vwUserMenus Inner Join Sec_UserPrevileges On vwUserMenus.N_MenuID=Sec_UserPrevileges.N_MenuID And Sec_UserPrevileges.N_UserCategoryID = vwUserMenus.N_UserCategoryID And  Sec_UserPrevileges.N_UserCategoryID=@nUserCatID inner join Lan_MultiLingual on vwUserMenus.N_MenuID=Lan_MultiLingual.N_FormID and Lan_MultiLingual.N_LanguageId=@nLangId and X_ControlNo ='0' Where LOWER(vwUserMenus.X_Caption) <>'seperator' and vwUserMenus.N_ParentMenuID=@nMenuId Order By vwUserMenus.N_Order";
            Params.Add("@nMenuId", nMenuId);
            Params.Add("@nLangId", nLangId);
            Params.Add("@nUserCatID", 2);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    DataTable dt1 = new DataTable();

                    string sqlCommandText1 = "select n_CompID,n_LanguageId,n_MenuID,x_CompType,x_FieldList,x_FieldType,x_Text,X_FieldtoReturn from vw_WebReportMenus where N_LanguageId=@nLangId";
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
                return BadRequest(_api.Error(e));
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
                return BadRequest(_api.Error(e));
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
        public  IActionResult GetModuleReports(string reportName, string critiria)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                };
                var client = new HttpClient(handler);
                string URL = reportApi + "/api/report?reportName=" + reportName + "&critiria=" + critiria + "&con=&path="+reportPath ;//+ connectionString;
                var path = client.GetAsync(URL);
                path.Wait();
                return Ok(_api.Success(new SortedList(){{"FileName",reportName.Trim() + ".pdf"}}));
            }
            catch (Exception e)
            {
                return BadRequest(_api.Error(e));
            }
        }

        [HttpPost("getModuleReport")]
        public  IActionResult GetModuleReports([FromBody] DataSet ds)
        {
            DataTable MasterTable;
            DataTable DetailTable;

            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];

            try
            {
                String Criteria = "";
                String reportName = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //int MenuID = myFunctions.getIntVAL(MasterTable.Rows[0]["moduleID"].ToString());
                    int MenuID = myFunctions.getIntVAL(MasterTable.Rows[0]["reportCategoryID"].ToString());
                    int ReportID = myFunctions.getIntVAL(MasterTable.Rows[0]["reportID"].ToString());

                    SortedList Params1 = new SortedList();
                    Params1.Add("@nMenuID", MenuID);
                    Params1.Add("@xType", "RadioButton");
                    Params1.Add("@nCompID", ReportID);

                    reportName = dLayer.ExecuteScalar("select X_rptFile from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID", Params1, connection).ToString();

                    reportName = reportName.Substring(0,reportName.Length-4);
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
                        string xFeild = dLayer.ExecuteScalar("select X_DataField from Sec_ReportsComponents where N_MenuID=@nMenuID and X_CompType=@xType and N_CompID=@nCompID", Params, connection).ToString();

                        if (type == "datepicker")
                        {
                            DateTime dateFrom = Convert.ToDateTime(value);
                            DateTime dateTo = Convert.ToDateTime(valueTo);

                            string DateCrt = xFeild + " >= Date('" + dateFrom.Year + "," + dateFrom.Month + "," + dateFrom.Day + "') And " + xFeild + " <= Date('" + dateTo.Year + "," + dateTo.Month + "," + dateTo.Day + "') ";
                            Criteria = Criteria == "" ? DateCrt : Criteria + " and " + DateCrt;
                        }
                        else
                        {
                            Criteria = Criteria == "" ? xFeild + "='" + value + "' " : Criteria + " and " + xFeild + "='" + value + "' ";
                        }

                        //{table.fieldname} in {?Start date} to {?End date}
                    }
                }

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                };
                var client = new HttpClient(handler);
                //HttpClient client = new HttpClient(clientHandler);
                string URL = reportApi + "/api/report?reportName=" + reportName + "&critiria=" + Criteria + "&con=&path="+reportPath ;//+ connectionString;
                var path = client.GetAsync(URL);

                path.Wait();
                return Ok(_api.Success(new SortedList(){{"FileName",reportName.Trim() + ".pdf"}}));
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
                return BadRequest(_api.Error(e));
            }
        }



    }
}