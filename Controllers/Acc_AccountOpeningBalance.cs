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
    [Route("accountOpening")]
    [ApiController]
    public class Acc_AccountOpeningBalance : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly IApiFunctions _api;
        private readonly int FormID;

        public Acc_AccountOpeningBalance(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 151;
        }

        [HttpGet("costCenter")]
        public ActionResult GetCostCenterValues(int N_VoucherID,int N_FnYearID)
        {
            DataTable dt = new DataTable();
             try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SortedList Params = new SortedList()
                    {
                        {"AccountField",2},
                        {"N_VoucherID",N_VoucherID},
                        {"N_CompanyID",myFunctions.GetCompanyID(User)},
                        {"N_FnYearID",N_FnYearID},                       
                    };

                    dt = dLayer.ExecuteDataTablePro("SP_Acc_AccountBalance_Disp", Params, connection);
                    dt = _api.Format(dt, "costCenter");
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
                return Ok(api.Error(User,e));
            }
        }

       
        [HttpGet("details")]
        public ActionResult GetAccountOpeningDetails(int nFnYearID,int nBranchID,int nLanguageID)
        {
            DataSet dsOpening = new DataSet();
            DataTable Master=new DataTable();
            DataTable DetailTable=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int AccountField=0;
            int nVoucherID=0;
            string sqlCommandText="",sqlCommandText2="";
            
            if(nLanguageID==2)AccountField=1;
           
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nFnYearID",nFnYearID);
            Params.Add("@nBranchID",nBranchID);

            sqlCommandText="Select * from Acc_VoucherMaster Where N_CompanyID=@nCompanyID And N_FnYearID=@nFnYearID and X_TransType='OB' and N_BranchID=@nBranchID";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Master=dLayer.ExecuteDataTable(sqlCommandText,Params,connection); 
                    Master = _api.Format(Master, "Master");

                    if (Master.Rows.Count > 0)
                    {
                        DataRow MasterRow = Master.Rows[0];
                        nVoucherID = myFunctions.getIntVAL(MasterRow["N_VoucherID"].ToString());
                    }

                    //Details
                     SortedList ParamsList = new SortedList()
                    {
                        {"AccountField",AccountField},
                        {"N_VoucherID",nVoucherID},
                        {"N_CompanyID",myFunctions.GetCompanyID(User)},
                        {"N_FnYearID",nFnYearID},                       
                    };

                    DetailTable = dLayer.ExecuteDataTablePro("SP_Acc_AccountBalance_Disp", ParamsList, connection);
                    DetailTable = _api.Format(DetailTable, "Details");

                    if(DetailTable.Rows.Count==0)
                    {
                        return Ok(_api.Notice("No Results Found" ));
                    }

                    myFunctions.AddNewColumnToDataTable(DetailTable, "N_Dimension", typeof(int), 0);

                    sqlCommandText2="select * from vw_VoucherDetails_Desc where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID";
                    DataTable DimDetails=dLayer.ExecuteDataTable(sqlCommandText2,Params,connection); 
                    if (DimDetails.Rows.Count > 0)
                    {
                        for (int i = 1; i < DetailTable.Rows.Count; i++)
                        {
                            DataRow[] dr = DimDetails.Select("N_VoucherID=" + nVoucherID + " and N_LedgerID=" + myFunctions.getIntVAL(DetailTable.Rows[i]["N_LedgerID"].ToString()) + "");
                            if (dr.Length > 0)
                                DetailTable.Rows[i]["N_Dimension"]=myFunctions.getIntVAL(dr.Length.ToString());
                        }
                    }

                    //dsOpening.Tables.Add(Master);
                    dsOpening.Tables.Add(DetailTable);

                    return Ok(_api.Success(dsOpening));
                }
               
            }catch(Exception e){
                return Ok(_api.Error(User,e));
            }
        }

        // [HttpPost("save")]
        // public ActionResult SaveData([FromBody]DataSet ds)
        // { 
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             object Result = 0;
        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();
        //             DataTable DetailTable;
        //             DetailTable = ds.Tables["details"];
        //             SortedList Params = new SortedList();
        //             int nCompanyID = myFunctions.getIntVAL(DetailTable.Rows[0]["n_CompanyID"].ToString());
        //             int nFnYearID = myFunctions.getIntVAL(DetailTable.Rows[0]["n_FnYearID"].ToString());

        //             Params.Add("@p1", nCompanyID);
        //             Params.Add("@p2", nFnYearID);

        //             for (int i = 1; i <= DetailTable.Rows.Count; i++)
        //             {
        //                 int nLedgerID = myFunctions.getIntVAL(DetailTable.Rows[i-1]["n_LedgerID"].ToString());
        //                 int nCashBahavID = myFunctions.getIntVAL(DetailTable.Rows[i-1]["n_CashBahavID"].ToString());
        //                 int nTransBehavID = myFunctions.getIntVAL(DetailTable.Rows[i-1]["n_TransBehavID"].ToString());
        //                 string xCashTypeBehaviour = DetailTable.Rows[i-1]["x_CashTypeBehaviour"].ToString();
        //                 string isDeleted = DetailTable.Rows[i-1]["isDeleted"].ToString();

        //                 if (isDeleted == "False")
        //                 {
        //                     dLayer.ExecuteNonQuery("Update Acc_MastLedger Set X_CashTypeBehaviour = '',N_CashBahavID=0,N_TransBehavID=0 Where N_LedgerID= " + nLedgerID + " And N_CompanyID = @p1 and N_FnYearID = @p2", Params, connection, transaction);
        //                 }
        //                 else
        //                 {
        //                     dLayer.ExecuteNonQuery("Update Acc_MastLedger Set X_CashTypeBehaviour = '" + xCashTypeBehaviour + "',N_CashBahavID=" + nCashBahavID + ",N_TransBehavID=" + nTransBehavID + " where N_LedgerID= " + nLedgerID + " and N_CompanyID = @p1 and N_FnYearID = @p2", Params, connection, transaction);
        //                 }
        //                 if(DetailTable.Columns.Contains("isDeleted"))
        //                 DetailTable.Columns.Remove("isDeleted");
        //             }
        //             if (DetailTable.Rows.Count < 0)
        //             {
        //                 transaction.Rollback();
        //                 return Ok(api.Error(User,"Unable to save"));
        //             }
        //             else {
        //                 transaction.Commit();
        //                 return Ok(_api.Success("Account Behaviour Saved"));
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(_api.Error(User,ex));
        //     }
        // }
     

        }
    }
       

    

