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
    [Route("loanClose")]
    [ApiController]
    public class Pay_LoanClose : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1303;

        public Pay_LoanClose(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("details")]
        public ActionResult LoanCloseDetails(string xLoanCode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_PayLoanClose where N_CompanyID=@p1 and Code=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", xLoanCode);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
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



        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nLoanCloseID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LoanCloseID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                   
                    // Auto Gen
                    string LoanCloseCode = "";
                    var values = MasterTable.Rows[0]["X_LoanClosingCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("N_LoanCloseID", nLoanCloseID);
                        LoanCloseCode = dLayer.GetAutoNumber("Pay_LoanClose", "X_LoanClosingCode", Params, connection, transaction);
                        if (LoanCloseCode == "") { return Ok(api.Error("Unable to generate Lead Code")); }
                        MasterTable.Rows[0]["X_LoanClosingCode"] = LoanCloseCode;
                    }
                    if (nLoanCloseID > 0)
                        {
                            SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"N_YearID", nFnYearId},
                                {"X_TransType","ELC"},
                                {"X_ReferenceNo",values},
                                };
                            object N_LoanTransID= dLayer.ExecuteScalar("Select N_LoanTransID from vw_PayLoanClose where N_CompanyID=N_CompanyID and N_LoanCloseID=N_LoanCloseID", Params, connection);
                             Params.Add("N_LoanTransID", N_LoanTransID);
                            
                            dLayer.ExecuteNonQueryPro("SP_Pay_LoanClosingVoucher_Del", DeleteParams, connection, transaction);
                            dLayer.ExecuteNonQuery("Update Pay_LoanIssueDetails set N_RefundAmount =Null,D_RefundDate =Null,N_PayRunID =Null,N_TransDetailsID =Null,B_IsLoanClose =Null  where N_LoanTransID= N_LoanTransID and N_CompanyID=N_CompanyID and B_IsLoanClose=1 and N_TransDetailsID=N_LoanCloseID",Params, connection, transaction);
                            dLayer.ExecuteNonQuery("Delete from Pay_LoanClose where N_LoanCloseID=@p2 and N_CompanyID=@p1", Params, connection, transaction);
                        }


                    nLoanCloseID = dLayer.SaveData("Pay_LoanClose", "nLoanCloseID", MasterTable, connection, transaction);
                    if (nLoanCloseID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    dLayer.ExecuteNonQuery("Update Pay_LoanIssueDetails Set B_IsLoanClose =1,N_TransDetailsID=N_LoanCloseID  Where N_CompanyID =N_CompanyID and N_LoanTransID=N_LoanTransID and (N_RefundAmount=0 OR N_RefundAmount IS Null)",Params, connection, transaction);
                    int nLoanIsueDetailsID = dLayer.SaveData("Pay_LoanIssueDetails", "N_LoanTransDetailsID", MasterTable, connection, transaction);
                    if (nLoanIsueDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to Save");
                    }
                    else
                    {
                        transaction.Commit();
                    }

                    return Ok(api.Success("Loan Closed"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }

      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int xLoanClose,int nFnYearId)
        {

             int Results = 0;
             int nCompanyId=myFunctions.GetCompanyID(User);
            try
            {                        
                SortedList Params = new SortedList();
                int N_LoanCloseID=0;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                     SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyId},
                                {"N_YearID", nFnYearId},
                                {"X_TransType","ELC"},
                                {"X_ReferenceNo",xLoanClose},
                                };
                            N_LoanCloseID=myFunctions.getIntVAL(dLayer.ExecuteScalar("Select N_LoanCloseID from Pay_LoanClose where N_CompanyID=N_CompanyID and X_LoanClosingCode=X_ReferenceNo", Params, connection).ToString());
                            dLayer.ExecuteNonQueryPro("SP_Pay_LoanClosingVoucher_Del", DeleteParams, connection, transaction);
                            dLayer.ExecuteNonQuery("Update Pay_LoanIssueDetails set N_RefundAmount =Null,D_RefundDate =Null,N_PayRunID =Null,N_TransDetailsID =Null,B_IsLoanClose =Null  where N_LoanTransID= N_LoanTransID and N_CompanyID=N_CompanyID and B_IsLoanClose=1 and N_TransDetailsID=N_LoanCloseID",Params, connection, transaction);
                            Results = dLayer.DeleteData("Pay_LoanIssueDetails", "N_LoanTransID", N_LoanCloseID, "", connection, transaction);
                            transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_LoanCloseID",N_LoanCloseID.ToString());
                    return Ok(api.Success(res,"Loan Close deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to Loan Close"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }



        }
    }
}