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
        private readonly IApiFunctions api;
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
         private readonly int N_FormID = 612;
        
        public Acc_Currency(IDataAccessLayer dl, IApiFunctions _api, IMyFunctions myFun, IConfiguration conf)
        {
            api=_api;
            
            dLayer = dl;
             myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [AllowAnonymous]
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
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                    
                }
                dt = api.Format(dt);
                     if(dt.Rows.Count==0)
                    {
                       return Ok(api.Warning("No Results Found")); }
                       else{return Ok(dt);}
                }catch(Exception e){
                    return Ok(api.Error(e));
                }
        }

       
       

        

          [HttpGet("Details") ]
        public ActionResult GetCurrencyDetails (int nCurrencyID,int nCompanyID)
          
        {   DataTable dt=new DataTable();
            SortedList Params = new SortedList();
           //  int nCompanyID=myFunctions.GetCompanyID(User);
              string sqlCommandText="select * from Acc_CurrencyMaster where N_CompanyID=@nCompanyID  and N_CurrencyID=@nCurrencyID order by X_CurrencyCode";
               Params.Add("@nCompanyID",nCompanyID);
             Params.Add("@nCurrencyID",nCurrencyID);
            
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
                return Ok(api.Error(e));
            }   
        }

        [HttpGet("currencyExchangeRate")]
        public ActionResult GetCurrencyExchangeRate(int nCurrencyCode)
        {
            try
            {
                SortedList Params = new SortedList();
                Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                Params.Add("@nCurrencyCode", nCurrencyCode);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DataTable AccCurrencyMaster = dLayer.ExecuteDataTable("Select X_ShortName,N_ExchangeRate,N_Decimal From Acc_CurrencyMaster Where N_CompanyID=@nCompanyID and N_CurrencyID=@nCurrencyCode", Params, connection);
                    return Ok(api.Success(api.Format(AccCurrencyMaster)));
                }
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

       //Save....
    //    [HttpPost("save")]
    //     public ActionResult SaveData([FromBody]DataSet ds)
    //     { 
    //         try{

    //              using (SqlConnection connection = new SqlConnection(connectionString))
    //             {
    //                 connection.Open();
    //                 SqlTransaction transaction = connection.BeginTransaction();
    //                 DataTable MasterTable;
    //                 MasterTable = ds.Tables["master"];
    //                 SortedList Params = new SortedList();
    //                 // Auto Gen
    //                 string X_CurrencyCode="";
    //                 var values = MasterTable.Rows[0]["X_CurrencyCode"].ToString();
    //                 if(values=="@Auto"){
    //                     Params.Add("N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString());
    //                      Params.Add("N_CurrencyID",MasterTable.Rows[0]["n_CurrencyID"].ToString());
    //                     Params.Add("N_YearID",MasterTable.Rows[0]["n_FnYearId"].ToString());
    //                     Params.Add("N_FormID",612);
    //                     Params.Add("N_BranchID",MasterTable.Rows[0]["n_BranchId"].ToString());
    //                     X_CurrencyCode =  dLayer.GetAutoNumber("Acc_CurrencyMaster","X_CurrencyCode", Params,connection,transaction);
    //                     if(X_CurrencyCode==""){
    //                         transaction.Rollback();
    //                         return Ok(api.Warning("Unable to generate Category Code"));
    //                         }
    //                     MasterTable.Rows[0]["X_CurrencyCode"] = X_CurrencyCode;
    //                 }

    //                 MasterTable.Columns.Remove("n_FnYearId");
    //                 MasterTable.Columns.Remove("n_BranchId");

    //                 int N_CurrencyID=dLayer.SaveData("Acc_CurrencyMaster","N_CurrencyID",MasterTable,connection,transaction);                    
    //                 if(N_CurrencyID<=0){
    //                     transaction.Rollback();
    //                     return Ok(api.Warning("Unable to save"));
    //                     }else{
    //                 transaction.Commit();
    //                 return  GetCurrencyDetails(int.Parse(MasterTable.Rows[0]["n_CompanyId"].ToString()),N_CurrencyID);
    //                     }
    //             }
    //             }
    //             catch (Exception ex)
    //             {
                   
    //                 return Ok(api.Error(ex));
    //             }
    //     }



         [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nCurrencyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CurrencyID"].ToString());
                int n_BranchId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_BranchID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string CurrencyCode = "";
                    var values = MasterTable.Rows[0]["X_CurrencyCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("N_BranchID",MasterTable.Rows[0]["n_BranchId"].ToString());
                        CurrencyCode = dLayer.GetAutoNumber("Acc_CurrencyMaster", "X_CurrencyCode", Params, connection, transaction);
                        if (CurrencyCode == "") { transaction.Rollback();return Ok(api.Error("Unable to generate Currency Master")); }
                        MasterTable.Rows[0]["X_CurrencyCode"] = CurrencyCode;
                    }
                     MasterTable.Columns.Remove("n_FnYearId");
                      MasterTable.Columns.Remove("n_BranchId");



                    nCurrencyID = dLayer.SaveData("Acc_CurrencyMaster", "N_CurrencyID", MasterTable, connection, transaction);
                    if (nCurrencyID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Currency Master Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCurrencyId)
        {
             int Results=0;
            try
            {
                                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                Results=dLayer.DeleteData("Acc_CurrencyMaster","N_CurrencyID",nCurrencyId,"",connection);
                if(Results>0){
                    return Ok(api.Success("Currency deleted" ));
                }else{
                    return Ok(api.Warning("Unable to delete Currency" ));
                }
                }
                
            }
            catch (Exception ex)
                {
                    return Ok(api.Error(ex));
                }
        }
    }
}