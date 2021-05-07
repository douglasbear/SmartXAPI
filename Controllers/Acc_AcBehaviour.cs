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
        private readonly IApiFunctions _api;
        private readonly int FormID;

        public Acc_AccountBehaviour(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 151;
        }

        [HttpGet("behaviourList")]
        public ActionResult GetBehaviourList(int nType)
        {
            DataTable dt = new DataTable();
            SortedList Params=new SortedList();
            string sqlCommandText = "select * from Acc_LedgerBehaviour where N_Type=@p1 order by N_LedgerBehaviourID";
            Params.Add("@p1",nType);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpGet("accountList")]
        public ActionResult GetAccountList(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_AccMastLedger where N_CompanyID=@p1 and N_FnYearID=@p2 order by N_LedgerID";
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nFnYearID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetAccBehaviourDetails(int nFnYearID)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText="select * from Acc_VoucherMaster left outer join Acc_MastLedger on Acc_VoucherMaster.N_DefLedgerID = Acc_MastLedger.N_LedgerID and Acc_VoucherMaster.N_FnYearID=Acc_MastLedger.N_FnYearID and Acc_VoucherMaster.N_CompanyID=Acc_MastLedger.N_CompanyID Where Acc_VoucherMaster.N_CompanyID=@p1 and Acc_VoucherMaster.N_FnYearID=@p2 Order By N_VoucherID";
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nFnYearID);
            try{
                using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection); 
                    }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(_api.Notice("No Results Found" ));
                        }else{
                            return Ok(_api.Success(dt));
                        }
            }catch(Exception e){
                return Ok(_api.Error(e));
            }
        }
     
     
    //   [HttpPost("Save")]
    //     public ActionResult SaveData([FromBody] DataSet ds)
    //     {
    //         try
    //         {
    //              DataTable DetailTable;
    //              DetailTable = ds.Tables["details"];
    //              SortedList Params = new SortedList();
    //              SortedList QueryParams = new SortedList();
                 

    //              using (SqlConnection connection = new SqlConnection(connectionString))
    //             {
    //                 connection.Open();
    //                 DataRow Detailss = DetailTable.Rows[0];
    //                 SqlTransaction transaction;
    //                 transaction = connection.BeginTransaction();

    //               int n_LedgerID = myFunctions.getIntVAL(Detailss["n_LedgerID"].ToString());
    //                int N_FnYearID = myFunctions.getIntVAL(Detailss["n_FnYearID"].ToString());
    //                 int N_CompanyID = myFunctions.getIntVAL(Detailss["n_CompanyID"].ToString());
    //                 int  N_CashBahavID  = myFunctions.getIntVAL(Detailss["N_CashBahavID"].ToString());
    //                 int  N_TransBehavID  = myFunctions.getIntVAL(Detailss["N_TransBehavID"].ToString());
    //                 int  N_PostingBehavID  = myFunctions.getIntVAL(Detailss["N_PostingBehavID"].ToString());
    //                SortedList QueryParamsList = new SortedList();
    //                  QueryParams.Add("@nCompanyID", N_CompanyID);
    //                 QueryParams.Add("@nFnYearID", N_FnYearID);
    //                 QueryParams.Add("@nLedgerID",n_LedgerID);
    //                  QueryParams.Add("@nCashBahavID", N_CashBahavID);
    //                 QueryParams.Add("@nTransBehavID", N_TransBehavID);
    //                 QueryParams.Add("@nPostingBehavID",N_PostingBehavID);
                    
                   
                    
    //                  for (int j = 0; j < DetailTable.Rows.Count; j++)
    //                     {
    //                          //dLayer.ExecuteNonQuery("Update Acc_MastLedger Set X_CashTypeBehaviour = '',N_CashBahavID=0,N_TransBehavID=0 Where N_LedgerID= @nLedgerID And N_CompanyID = @nCompanyID and N_FnYearID=@nFnYearID",Params,connection, transaction);
                        
    //                     // int  N_CashBahavID, N_TransBehavID, N_PostingBehavID;
    //                      string x_behaviour="",x_Dr="";
    //                      x_behaviour=DetailTable.Rows[j]["x_behaviour"];
    //                      DetailTable.Rows[j]["N_LedgerBehaviourID"];
                         

    //                      if(DetailTable.Rows[j]["N_LedgerBehaviourID"]==0)
    //                         N_CashBahavID=0;
    //                     else
    //                         N_CashBahavID =myFunctions.getIntVAL(dLayer.ExecuteScalar("SELECT N_LedgerBehaviourID FROM Acc_LedgerBehaviour where  X_Description="+x_behaviour,Params,connection, transaction));
    //                     if (x_Dr == "")
    //                         N_TransBehavID = 0;
    //                     else
    //                     N_TransBehavID = myFunctions.getIntVAL(dLayer.ExecuteScalar("SELECT N_LedgerBehaviourID FROM Acc_LedgerBehaviour where X_Description="+x_behaviour,Params,connection, transaction));

    //                     if (flxPayTransactions.get_TextMatrix(i, mcBehaviour) == "")
    //                         X_CashTypeBehaviour = "";
    //                     else
    //                         X_CashTypeBehaviour = dLayer.ExecuteSclar("SELECT  X_Description FROM Acc_LedgerBehaviour where  N_LedgerBehaviourID=" + N_CashBahavID, "TEXT", new DataTable()).ToString();
    //                         dLayer.ExecuteNonQuery("Update Acc_MastLedger Set X_CashTypeBehaviour = '" + X_CashTypeBehaviour + "',N_CashBahavID=" + N_CashBahavID + ",N_TransBehavID=" + N_TransBehavID + " Where N_LedgerID= " + myFunctions.getIntVAL(flxPayTransactions.get_TextMatrix(i, mcLedgerID)).ToString() + " And N_CompanyID = " + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID, "TEXT", new DataTable());
                    
    //                     }
    //                     transaction.Commit();

    //               //  return Ok(_api.Success(Result, " Employee Evaluation Created"));
    //             }
            
        
    //         }
    //       catch (Exception ex)
    //         {
    //             return Ok(_api.Error(ex));
    //         }
        
    //         }
        }
        }

            
        
    



        
       

    

