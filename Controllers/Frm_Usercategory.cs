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
    [Route("usercategory")]
    [ApiController]
    public class Frm_Usercategory : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly string conString;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        
        public Frm_Usercategory(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            _api=api;
            dLayer = dl;
             myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
         [HttpGet("list")]
        public ActionResult GetCategoryList(int? nCompanyId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from vw_UserRole_Disp where N_CompanyID=@p1 order by Category DESC";
            Params.Add("@p1",nCompanyId);

            try{
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                    connection.Open();
                }
                dt = _api.Format(dt);
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


       //Save....
       [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            try{

                 using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string X_UserCategoryCode="";
                    var values = MasterTable.Rows[0]["X_UserCategoryCode"].ToString();
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID",MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID",40);
                        Params.Add("N_BranchID",MasterTable.Rows[0]["n_BranchId"].ToString());
                        X_UserCategoryCode =  dLayer.GetAutoNumber("sec_usercategory","X_UserCategoryCode", Params,connection,transaction);
                        if(X_UserCategoryCode==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Category Code" ));}
                        MasterTable.Rows[0]["X_UserCategoryCode"] = X_UserCategoryCode;
                    }

                    MasterTable.Columns.Remove("n_FnYearId");
                    MasterTable.Columns.Remove("n_BranchId");

                    int N_UserCategoryID=dLayer.SaveData("sec_usercategory","N_UserCategoryID",0,MasterTable,connection,transaction);                    
                    if(N_UserCategoryID<=0){
                        transaction.Rollback();
                        return StatusCode(404,_api.Response(404 ,"Unable to save" ));
                        }else{
                    transaction.Commit();
                    return  GetCategoryDetails(int.Parse(MasterTable.Rows[0]["n_CompanyId"].ToString()),N_UserCategoryID);
                        }
                }
                }
                catch (Exception ex)
                {
                   
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nUsercategoryId)
        {
             int Results=0;
            try
            {
                Results=dLayer.DeleteData("sec_usercategory","N_UserCategoryID",nUsercategoryId,"");
                if(Results>0){
                    return StatusCode(200,_api.Response(200 ,"Category deleted" ));
                }else{
                    return StatusCode(409,_api.Response(409 ,"Unable to delete Category" ));
                }
                
            }
            catch (Exception ex)
                {
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
            

        }
    }
}