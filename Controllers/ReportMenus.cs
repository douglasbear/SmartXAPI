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
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
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
            _api=api;
            dLayer = dl;
             myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
         [HttpGet("list")]
        public ActionResult GetReportList(int? nMenuId,int? nLangId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();

            string sqlCommandText="select * from vw_Menus where N_ParentMenuID=@p1 and N_LanguageId=@p2";
            Params.Add("@p1",nMenuId);
            Params.Add("@p2",nLangId);

            try{
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                    DataTable dt1 = new DataTable();
                    string sqlCommandText1="select N_MenuID,x_Text,x_FieldType,X_rptFile from vw_ReportMenus where N_LanguageId=@p2";
                    dt1=dLayer.ExecuteDataTable(sqlCommandText1,Params,connection);

                    dt.Columns.Add("ChildMenus", typeof(DataTable));
                    dt.Columns.Add("Filter", typeof(DataTable));
                    for(int i=0;i<dt.Rows.Count;i++)
                    {
                     DataTable ChildMenus = new DataTable();  
                     DataTable Filter = new DataTable();  
                     string N_MenuID =dt.Rows[i]["N_MenuID"].ToString();
                     try{
                     DataRow[] dr = dt1.Select("N_MenuID = " + N_MenuID +" and x_FieldType='RadioButton'"); 
                     DataRow[] dr1 = dt1.Select("N_MenuID = " + N_MenuID +" and x_FieldType<>'RadioButton'"); 
                     if(dr!=null)
                     {ChildMenus = dr.CopyToDataTable();
                     dt.Rows[i]["ChildMenus"]=ChildMenus;}
                     if(dr1!=null){Filter = dr1.CopyToDataTable();
                     dt.Rows[i]["Filter"]=Filter;}
                     
                     }
                     catch
                     {
                     
                     }
                     
                     
                    
                     
                     }
                  

                }

                     if(dt.Rows.Count==0)
                    {
                       return StatusCode(200, _api.Response(200, "No Results Found")); }
                       else{return Ok(dt);}
                }catch(Exception e){
                    return StatusCode(403,_api.ErrorResponse(e));
                }
        }

       

        [HttpGet("listdetails")]
        public ActionResult GetCategoryDetails(int? nCompanyId,int? nCategoryId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from Sec_UserCategory where N_CompanyID=@p1 and N_UserCategoryID=@p2 order by N_UserCategoryID DESC";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nCategoryId);

            try{
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                if(dt.Rows.Count==0)
                {return StatusCode(200,new { StatusCode = 200 , Message= "No Results Found" });}
                else{return Ok(dt);}
                }
            catch(Exception e){
                    return StatusCode(403,_api.ErrorResponse(e));}
        }


    }
}