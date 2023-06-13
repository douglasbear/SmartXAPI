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
    [Route("currencyRate")]
    [ApiController]
    public class Exchange_RateHistory : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =1688;


        public Exchange_RateHistory(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = 
            conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("list") ]
        public ActionResult GetExchangeRateList (int nCurrencyID,int nSizeperpage,int nPage)
          
        {   DataTable dt=new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            int nCompanyID=myFunctions.GetCompanyID(User);
          //  string sqlCommandText="select * from Inv_ExchangeRateHistory where N_CompanyID=@nCompanyID  and N_CurrencyID=@nCurrencyID order by X_ExchangeRateCode";
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nCurrencyID",nCurrencyID);
            string sqlCommandText="";
            SortedList OutPut = new SortedList();
            
            try{
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                
             
              if (Count == 0)
              
                        sqlCommandText = "select  top(" + nSizeperpage + ") * from Inv_ExchangeRateHistory where N_CompanyID=@nCompanyID and N_CurrencyID=@nCurrencyID" ;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ")* from Inv_ExchangeRateHistory where N_CompanyID=@nCompanyID and N_CurrencyID=@nCurrencyID   and N_ExchangeRateID not in (select top(" + Count + ") N_ExchangeRateID from Inv_ExchangeRateHistory where N_CompanyID=@nCompanyID  and N_CurrencyID=@nCurrencyID)";
              
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count from Inv_ExchangeRateHistory where N_CompanyID=@nCompanyID and N_CurrencyID=@nCurrencyID   and N_ExchangeRateID  Not in (select top(" + Count + ") N_ExchangeRateID from Inv_ExchangeRateHistory where N_CompanyID=@nCompanyID  and N_CurrencyID=@nCurrencyID)";

                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }
                }
                
            }catch(Exception e){
                return Ok(api.Error(User,e));
            }   
        }


         [HttpGet("Details") ]
        public ActionResult GetExchangeRateDetails (int nExchangeRateID,int nCompanyID)
          
        {   DataTable dt=new DataTable();
            SortedList Params = new SortedList();
           //int nCompanyID=myFunctions.GetCompanyID(User);
            string sqlCommandText="select * from Inv_ExchangeRateHistory where N_CompanyID=@nCompanyID  and N_ExchangeRateID=@nExchangeRateID order by X_ExchangeRateCode";
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nExchangeRateID",nExchangeRateID);
            
            try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }else{
                            return Ok(api.Success(dt));
                        }
                
            }catch(Exception e){
                return Ok(api.Error(User,e));
            }   
        }




        //Save....
       
         [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nExchangeRateID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ExchangeRateID"].ToString());
                int n_BranchId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_BranchID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string ExchangeRateCode = "";
                    var values = MasterTable.Rows[0]["X_ExchangeRateCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("N_BranchID",MasterTable.Rows[0]["n_BranchId"].ToString());
                        ExchangeRateCode = dLayer.GetAutoNumber("Inv_ExchangeRateHistory", "X_ExchangeRateCode", Params, connection, transaction);
                        if (ExchangeRateCode == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Currency Master")); }
                        MasterTable.Rows[0]["X_ExchangeRateCode"] = ExchangeRateCode;
                    }
                     MasterTable.Columns.Remove("n_FnYearId");
                      MasterTable.Columns.Remove("n_BranchId");

                    
                    nExchangeRateID = dLayer.SaveData("Inv_ExchangeRateHistory", "N_ExchangeRateID","","", MasterTable, connection, transaction);
                    if (nExchangeRateID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Exchange Rate History Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(User,ex));
            }
        }    
    }
}

