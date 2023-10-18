using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("bankreconcilation")]
    [ApiController]
    public class Acc_BankReconcilation : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Acc_BankReconcilation(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1806;
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int nReconcilID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ReconcilID"].ToString());
                    int nfnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_fnYearID"].ToString());
                    
                    string xReconcileCode = MasterTable.Rows[0]["X_ReconcileCode"].ToString();
                    String xButtonAction="";

                    MasterTable.AcceptChanges();

                    if (xReconcileCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nfnYearID);
                        Params.Add("N_FormID", 1806);
                        Params.Add("N_ReconcilID",nReconcilID);
                        
                        xReconcileCode = dLayer.GetAutoNumber("Acc_BankReconcilation", "X_ReconcileCode", Params, connection, transaction);
                        xButtonAction="Insert";
                        if (xReconcileCode == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to generate Bank Code"));
                        }
                        MasterTable.Rows[0]["X_ReconcileCode"] = xReconcileCode;
                    }
                    else
                    {
                        xButtonAction="Update"; 
                    }
                    nReconcilID = dLayer.SaveData("Acc_BankReconcilation", "N_ReconcilID", MasterTable, connection, transaction);
                    if (nReconcilID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("Unable to save"));
                    }


                     for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {

                        DetailTable.Rows[j]["N_ReconcilID"] = nReconcilID;
                    }

                    int nReconcilDetailID = dLayer.SaveData("Acc_BankReconcilDetails", "N_ReconcildetailID",DetailTable, connection, transaction);
                    if (nReconcilDetailID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        // Activity Log
                        string ipAddress = "";
                        if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                           ipAddress = Request.Headers["X-Forwarded-For"];
                        else
                           ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                           myFunctions.LogScreenActivitys(nfnYearID,nReconcilID,xReconcileCode,1806,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                        
                        transaction.Commit();
                        return Ok(_api.Success("BankReconcilation Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nReconcilID,int nfnYearID)
        {
            int Results = 0;
            SortedList Params = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    Params.Add("@N_ReconcilID", nReconcilID);
                    string Sql = "select N_ReconcilID,X_ReconcileCode from Acc_BankReconcilation where N_ReconcilID=@N_ReconcilID and N_CompanyID=N_CompanyID";
                    string xButtonAction="Delete";
                    string xReconcileCode = "";
                    TransData = dLayer.ExecuteDataTable(Sql, Params, connection);
                    SqlTransaction transaction = connection.BeginTransaction();

                    dLayer.DeleteData("Acc_BankReconcilDetails", "N_ReconcilID", nReconcilID, "", connection, transaction);
                    Results = dLayer.DeleteData("Acc_BankReconcilation", "N_ReconcilID", nReconcilID, "", connection, transaction);
                    
                    // Activity Log
                    string ipAddress = "";
                    if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                        ipAddress = Request.Headers["X-Forwarded-For"];
                    else
                        ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                        myFunctions.LogScreenActivitys(myFunctions.getIntVAL( nfnYearID.ToString()),nReconcilID,TransData.Rows[0]["X_ReconcileCode"].ToString(),1806,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                    
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("BankReconcilation deleted"));
                    }
                    else
                    {
                       transaction.Rollback();
                       return Ok(_api.Error(User, "Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }



        [HttpGet("filldetails")]
        public ActionResult GetBankReconcilValues(int N_FnYearID, int N_UserID,string X_DateFrom,string X_DateTo,int N_LedgerID)
        {
            DataTable dt = new DataTable();
            int N_CompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            SortedList Params = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SortedList Param = new SortedList()
                    {
                        {"N_CompanyID", N_CompanyID},
                        {"N_FnYearID", N_FnYearID},
                        {"N_UserID", N_UserID},
                        {"X_DateFrom", X_DateFrom},
                        {"X_DateTo", X_DateTo},
                        {"N_LedgerID", N_LedgerID},
                          {"IsReconcil", 0}
                     
                       
                    };
                    dLayer.ExecuteDataTablePro("SP_BankReconcilation_Details", Param, connection);

                    sqlCommandText = "select * from Acc_BankAccountStatement where N_CompanyID="+N_CompanyID+"";
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection); 
                    
                    dt = _api.Format(dt, "BankReconcil");
                    if (dt.Rows.Count == 0)
                    { 
                        return Ok(_api.Warning("No data found")); 
                    }
                    else
                    {
                        return Ok(_api.Success(dt));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        [HttpGet("details")]
        public ActionResult GetReconcilDetails1(string xReconcileCode,int nCompanyID,int nFnYearID,int nUserID,DateTime xDateFrom, string xDateTo,int nLedgerID,int NCompanyID )
        {
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    SortedList Param = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable DtTable = new DataTable();
                    string Mastersql = "";
                    string DetailSql = "";
                    string DtSql ="";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@X_ReconcileCode",xReconcileCode);
                    Mastersql = "select * from vw_BankReconcilation_Master  where N_CompanyID=@nCompanyID and X_ReconcileCode=@X_ReconcileCode";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                     
                   MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                      MasterTable = _api.Format(MasterTable, "Master");
                    if (MasterTable.Rows.Count == 0)
                     {
                         return Ok(_api.Warning("No data found")); 
                     }
      
                    int nReconcilID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ReconcilID"].ToString());
                    Params.Add("@nReconcilID", nReconcilID);
                    bool bReconcil = myFunctions.getBoolVAL(MasterTable.Rows[0]["b_Reconcil"].ToString());
                     DateTime d_DateFrom = Convert.ToDateTime(MasterTable.Rows[0]["D_FromDate"]);
                     DateTime d_Dateto = Convert.ToDateTime(MasterTable.Rows[0]["D_ToDate"]);
                     
                    // string dDateTo = MasterTable.Rows[0]["d_ToDate"].ToString(); 

                    string dDateFrom=myFunctions.getDateVAL(d_DateFrom);
                    string dDateTo=myFunctions.getDateVAL(d_Dateto);
                    int nLedgUD = myFunctions.getIntVAL(MasterTable.Rows[0]["n_LedgerID"].ToString());
                    
                    
                    
                     if(bReconcil == true){
                        DetailSql = "select * from vw_BankReconcilation_Detail  where N_CompanyID=@nCompanyID and X_ReconcileCode=@X_ReconcileCode";
                        DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                        DetailTable = _api.Format(DetailTable, "Details");
                           
                    }
                      else
                      {

                   SortedList ParamsList = new SortedList()
                    {
                        {"N_CompanyID", nCompanyID},
                        {"N_FnYearID", nFnYearID},
                        {"N_UserID", nUserID},
                        {"X_DateFrom", dDateFrom},
                        {"X_DateTo", dDateTo},
                        {"N_LedgerID", nLedgUD},
                        {"IsReconcil", 1}
                       
                       

                    };

                     Param.Add("@NCompanyID", myFunctions.GetCompanyID(User));
                     
                   
                         DetailTable = dLayer.ExecuteDataTablePro("SP_BankReconcilation_Details", ParamsList, connection);
                         DetailSql = "select * from Acc_BankAccountStatement where N_CompanyID="+nCompanyID+"";
                         DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection); 
                         DetailTable = _api.Format(DetailTable, "Details");
                         DtSql= "select * from vw_Bankreconcilate where N_CompanyID="+@NCompanyID+"";
                         DtTable = dLayer.ExecuteDataTable(DtSql, Param, connection);
                         DtTable = _api.Format(DtTable, "Detail");
                      }


                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    dt.Tables.Add(DtTable);
                    
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
       


       
        
        [HttpGet("Reconcil")]
        public ActionResult GetReconcilValues()
        {

            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@N_CompanyID", nCompanyID);
            string qry = "";

            qry = "select (max([D_ToDate]+1)) as D_FromDate from Acc_BankReconcilation where N_CompanyID=@N_CompanyID" ;
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(qry, Params, connection);
                    dt = _api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(dt));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

    }
}