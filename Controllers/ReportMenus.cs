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

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("report")]
    [ApiController]
    public class ReportMenus : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly string conString;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public ReportMenus(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("list")]
        public ActionResult GetReportList(int? nMenuId, int? nLangId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_Menus where N_ParentMenuID=@p1 and N_LanguageId=@p2";
            Params.Add("@p1", nMenuId);
            Params.Add("@p2", nLangId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    DataTable dt1 = new DataTable();


                    string sqlCommandText1 = "select * from vw_ReportMenus where N_LanguageId=@p2";
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
                    return StatusCode(200, _api.Response(200, "No Results Found"));
                }
                else { return Ok(dt); }
            }
            catch (Exception e)
            {
                return StatusCode(403, _api.ErrorResponse(e));
            }
        }


        [HttpGet("dynamiclist")]
        public ActionResult GetDynamicList(int nMenuId, int nCompId, int nLangId, string cval, string bval, string fval)
        {
            DataTable dt = new DataTable();
            DataTable outTable = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select TOP 1 X_TableName,X_FieldList,X_Criteria from vw_ReportMenus where N_MenuID=@p1 and N_LanguageId=@p2 and N_CompID=@p3 and X_CompType=@p4";
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
                        if(Criteria!="")
                        Criteria = " Where "+QueryString["X_Criteria"].ToString().Replace("'CVal'", "@CVal").Replace("'BVal'", "@BVal").Replace("'FVal'", "@FVal");
                        ListSqlParams.Add("@BVal", bval);
                        ListSqlParams.Add("@CVal", cval);
                        ListSqlParams.Add("@FVal", fval);
                        string ListSql = "select " + fields + " from " + table + Criteria;
                        
                        outTable = dLayer.ExecuteDataTable(ListSql, ListSqlParams, connection);
                    }


                }
                if (outTable.Rows.Count == 0)
                {
                    return StatusCode(200, _api.Response(200, "No Results Found"));
                }
                else {
                    //Dictionary<string,Dictionary<string,DataTable>> Menu = new Dictionary<string,Dictionary<string,DataTable>>();
                    outTable = _api.Format(outTable);
                    Dictionary<string,DataTable> Component = new Dictionary<string,DataTable>();
                    Component.Add(nCompId.ToString(),outTable); 
                    //Menu.Add(nMenuId.ToString(),Component);

                     return Ok(Component); 
                     }
            }
            catch (Exception e)
            {
                return StatusCode(403, _api.ErrorResponse(e));
            }
        }
    }
}