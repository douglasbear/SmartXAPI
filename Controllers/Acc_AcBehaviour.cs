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

        // [HttpGet("details")]
        // public ActionResult GetAccBehaviourDetails(int nFnYearID)
        // {
        //     DataTable dt=new DataTable();
        //     DataTable DetailTable;
        //     DetailTable = ds.Tables["details"];
        //     SortedList Params=new SortedList();
        //     int SLNo = 1;
        //     DetailTable.Rows = SLNo;
        //     int nCompanyID = myFunctions.GetCompanyID(User);
        //     string sqlCommandText="select * from Acc_VoucherMaster left outer join Acc_MastLedger on Acc_VoucherMaster.N_DefLedgerID = Acc_MastLedger.N_LedgerID and Acc_VoucherMaster.N_FnYearID=Acc_MastLedger.N_FnYearID and Acc_VoucherMaster.N_CompanyID=Acc_MastLedger.N_CompanyID Where Acc_VoucherMaster.N_CompanyID=@p1 and Acc_VoucherMaster.N_FnYearID=@p2 Order By N_VoucherID";
        //     Params.Add("@p1",nCompanyID);
        //     Params.Add("@p2",nFnYearID);
        //     Params.Add("@p3",nGroupID);
        //     if (dsTransaction.Tables.Contains("Acc_MastLedger"))
        //         dsTransaction.Tables.Remove("Acc_MastLedger");
        //     if(nGroupID !=0)
        //     {
        //         sqlCommandText="Select * from Acc_MastLedger  where (Isnull(X_CashTypeBehaviour,'') <> '' or Isnull(N_TransBehavID,0) <> 0) and  Acc_MastLedger.N_CompanyID =@p1 and N_GroupID=@p3 and Acc_MastLedger.N_FnYearID=@p2 order  by X_LedgerName asc";
        //     }
        //     else{
        //         sqlCommandText="Select * from Acc_MastLedger  where (Isnull(X_CashTypeBehaviour,'') <> '' or Isnull(N_TransBehavID,0) <> 0) and  Acc_MastLedger.N_CompanyID =@p1 and Acc_MastLedger.N_FnYearID=@p2 order  by X_LedgerName asc";
        //     }

        //     try{
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //             {
        //                 connection.Open();
        //                 dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection); 
        //             }
        //             if(dt.Rows.Count==0)
        //                 {
        //                     return Ok(_api.Notice("No Results Found" ));
        //                 }else{
        //                     return Ok(_api.Success(dt));
        //                 }
        //     }catch(Exception e){
        //         return Ok(_api.Error(e));
        //     }
        // }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    object Result = 0;
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable DetailTable;
                    DetailTable = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(DetailTable.Rows[0]["n_CompanyID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(DetailTable.Rows[0]["n_FnYearID"].ToString());

                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nFnYearID);

                    for (int i = 1; i <= DetailTable.Rows.Count; i++)
                    {
                        int nLedgerID = myFunctions.getIntVAL(DetailTable.Rows[i-1]["n_LedgerID"].ToString());
                        int nCashBahavID = myFunctions.getIntVAL(DetailTable.Rows[i-1]["n_CashBahavID"].ToString());
                        int nTransBehavID = myFunctions.getIntVAL(DetailTable.Rows[i-1]["n_TransBehavID"].ToString());
                        string xCashTypeBehaviour = DetailTable.Rows[i-1]["x_CashTypeBehaviour"].ToString();
                        string isDeleted = DetailTable.Rows[i-1]["isDeleted"].ToString();

                        if (isDeleted == "False")
                        {
                            dLayer.ExecuteNonQuery("Update Acc_MastLedger Set X_CashTypeBehaviour = '',N_CashBahavID=0,N_TransBehavID=0 Where N_LedgerID= " + nLedgerID + " And N_CompanyID = @p1 and N_FnYearID = @p2", Params, connection, transaction);
                        }
                        else
                        {
                            dLayer.ExecuteNonQuery("Update Acc_MastLedger Set X_CashTypeBehaviour = '" + xCashTypeBehaviour + "',N_CashBahavID=" + nCashBahavID + ",N_TransBehavID=" + nTransBehavID + " where N_LedgerID= " + nLedgerID + " and N_CompanyID = @p1 and N_FnYearID = @p2", Params, connection, transaction);
                        }
                        if(DetailTable.Columns.Contains("isDeleted"))
                        DetailTable.Columns.Remove("isDeleted");
                    }
                    if (DetailTable.Rows.Count < 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else {
                        transaction.Commit();
                        return Ok(_api.Success("Account Behaviour Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
     

        }
    }
       

    

