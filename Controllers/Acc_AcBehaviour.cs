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
    [Route("accountbehaviour")]
    [ApiController]
    public class Acc_AccountBehaviour : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 151;

        public Acc_AccountBehaviour(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }






 [HttpGet("Account") ]
        public ActionResult GetContractTypeList ()
        {    int nCompanyID=myFunctions.GetCompanyID(User);
  
            SortedList param = new SortedList(){{"@p1",nCompanyID}};
            
            DataTable dt=new DataTable();
            
            string sqlCommandText="select * from vw_accmastledger where N_CompanyID=@p1";
                
            try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
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



    //   [HttpPost("Save")]
    //     public ActionResult SaveData([FromBody] DataSet ds)
    //     {
    //         try
    //         {
    //              DataTable DetailTable;
    //              DetailTable = ds.Tables["details"];
    //                SortedList Params = new SortedList();
    //             SortedList QueryParams = new SortedList();

    //              using (SqlConnection connection = new SqlConnection(connectionString))
    //             {
    //                 connection.Open();
    //                   DataRow Detailss = DetailTable.Rows[0];

    //               int n_LedgerID = myFunctions.getIntVAL(Detailss["n_LedgerID"].ToString());
    //                int N_FnYearID = myFunctions.getIntVAL(Detailss["n_FnYearID"].ToString());
    //                 int N_CompanyID = myFunctions.getIntVAL(Detailss["n_CompanyID"].ToString());

    //                SortedList QueryParamsList = new SortedList();
    //                  QueryParams.Add("@nCompanyID", N_CompanyID);
    //                 QueryParams.Add("@nFnYearID", N_FnYearID);
    //                 QueryParams.Add("@nLedgerID",n_LedgerID);
                   
                    
    //                  for (int j = 0; j < DetailTable.Rows.Count; j++)
    //                     {

    //                 dLayer.ExecuteNonQuery("Update Acc_MastLedger Set X_CashTypeBehaviour = '',N_CashBahavID=0,N_TransBehavID=0 Where N_LedgerID= " + myFunctions.getIntVAL(flxPayTransactions.get_TextMatrix(i, mcLedgerID)).ToString() + " And N_CompanyID = " + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID, "TEXT", new DataTable());
    //         }
    //     }

    //     }
    //     }
    



        
        [HttpGet("Payment") ]
        public ActionResult GetCustomerProjectList (int nType)
        {    int nCompanyID=myFunctions.GetCompanyID(User);
          SortedList param = new SortedList(){{"@p1",nType}};
  
            
            DataTable dt=new DataTable();
            
            string sqlCommandText="select * from acc_LedgerBehaviour where  N_Type=@p1";
                
            try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
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

       
        }
    }