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
    [Route("currency")]
    [ApiController]
    public class Acc_Currency : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly string conString;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        
        public Acc_Currency(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            _api=api;
            dLayer = dl;
             myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
         [HttpGet("list")]
        public ActionResult GetCurrencyList(int? nCompanyId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from Acc_CurrencyMaster where N_CompanyID=@p1 order by X_CurrencyCode";
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
        public ActionResult GetCurrencyDetails(int? nCompanyId,int? nCurrencyId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from Acc_CurrencyMaster where N_CompanyID=@p1 and N_CurrencyID=@p2 order by X_CurrencyCode";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nCurrencyId);

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
                    string X_CurrencyCode="";
                    var values = MasterTable.Rows[0]["X_CurrencyCode"].ToString();
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID",MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID",612);
                        Params.Add("N_BranchID",MasterTable.Rows[0]["n_BranchId"].ToString());
                        X_CurrencyCode =  dLayer.GetAutoNumber("Acc_CurrencyMaster","X_CurrencyCode", Params,connection,transaction);
                        if(X_CurrencyCode==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Category Code" ));}
                        MasterTable.Rows[0]["X_CurrencyCode"] = X_CurrencyCode;
                    }

                    MasterTable.Columns.Remove("n_FnYearId");
                    MasterTable.Columns.Remove("n_BranchId");

                    int N_CurrencyID=dLayer.SaveData("Acc_CurrencyMaster","N_CurrencyID",0,MasterTable,connection,transaction);                    
                    if(N_CurrencyID<=0){
                        transaction.Rollback();
                        return StatusCode(404,_api.Response(404 ,"Unable to save" ));
                        }else{
                    transaction.Commit();
                    return  GetCurrencyDetails(int.Parse(MasterTable.Rows[0]["n_CompanyId"].ToString()),N_CurrencyID);
                        }
                }
                }
                catch (Exception ex)
                {
                   
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCurrencyId)
        {
             int Results=0;
            try
            {
                Results=dLayer.DeleteData("Acc_CurrencyMaster","N_CurrencyID",nCurrencyId,"");
                if(Results>0){
                    return StatusCode(200,_api.Response(200 ,"Currency deleted" ));
                }else{
                    return StatusCode(409,_api.Response(409 ,"Unable to delete Currency" ));
                }
                
            }
            catch (Exception ex)
                {
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
            

        }
    }
}