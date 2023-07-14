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
     [Route("financialPeriod")]
     [ApiController]
    public class FinancialPeriodList : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
          private readonly IMyAttachments myAttachments;
        private readonly IMyFunctions myFunctions;
        private readonly IApiFunctions _api;
        private readonly int nFormID = 519;
        public FinancialPeriodList(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            nFormID = 519;
        }
        [HttpGet("list")]
        public ActionResult GetFinPeriodList(int? nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    DataTable CountTable = new DataTable();
                    SortedList Params = new SortedList();
                    DataSet dataSet = new DataSet();
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";

                    int nUserID = myFunctions.GetUserID(User);


                    // if (xSearchkey != null && xSearchkey.Trim() != "")
                    //     Searchkey = "and ([X_GRNNo] like '%" + xSearchkey + "%' or N_GRNID like '%" + xSearchkey + "%')";

                    // if (xSortBy == null || xSortBy.Trim() == "")
                    //     xSortBy = " order by N_GRNID desc";
                    // else
                    // {
                    //     switch (xSortBy.Split(" ")[0])
                    //     {
                    //         case "X_GRNNo":
                    //             xSortBy = "X_GRNNo " + xSortBy.Split(" ")[1];
                    //             break;
                    //         case "N_GRNID":
                    //             xSortBy = "N_GRNID " + xSortBy.Split(" ")[1];
                    //             break;
                    //         default: break;
                    //     }
                    //     xSortBy = " order by " + xSortBy;
                    // }

                    int Count = (nPage - 1) * nSizeperpage;
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") [X_Period] AS X_Period,* from vw_AccPeriod_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") [X_Period] AS X_Period,* from vw_AccPeriod_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and N_PeriodID not in (select top(" + Count + ") N_PeriodID from vw_AccPeriod_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + xSortBy + " ) " + xSortBy;

                    // sqlCommandText = "select * from Inv_MRNDetails where N_CompanyID=@p1";
                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearId);
                    SortedList OutPut = new SortedList();


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count from vw_AccPeriod_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                    }
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);

                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
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
                    MasterTable = ds.Tables["master"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();

                    int nPeriodID = myFunctions.getIntVAL(MasterRow["N_PeriodID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["N_FnYearID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string xPeriodCode = MasterRow["x_PeriodCode"].ToString();
                    string x_PeriodCode = "";
                    if (xPeriodCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", nFormID);
                        x_PeriodCode = dLayer.GetAutoNumber("Acc_Period", "x_PeriodCode", Params, connection, transaction);
                        if (x_PeriodCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Code");
                        }
                        MasterTable.Rows[0]["x_PeriodCode"] = x_PeriodCode;
                    }
                    else
                    {
                         dLayer.DeleteData("Acc_Period", "N_PeriodID", nPeriodID, "", connection,transaction);
           
                    }
                   
                    int n_PeriodID = dLayer.SaveData("Acc_Period", "N_PeriodID", MasterTable, connection, transaction);
                    if (n_PeriodID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                 

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_PeriodID", n_PeriodID);
                    Result.Add("x_PeriodCode", x_PeriodCode);
                    return Ok(_api.Success(Result, "Financial Period Created"));
                }
            }
            
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

         [HttpGet("details")]
        public ActionResult EmployeeEvaluation(string PeriodCode, int nFnYearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DataTable = new DataTable();
                    string Mastersql = "";
                    string DetailSql = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@PeriodCode", PeriodCode);
                    Params.Add("@nFnYearID", nFnYearID);
                    Mastersql = "select * from Acc_Period where N_CompanyID=@nCompanyID and X_PeriodCode=@PeriodCode and N_FnyearID=@nFnYearID";
                   
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int nPeriodID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PeriodID"].ToString());
                    Params.Add("@nPeriodID", nPeriodID);

                    MasterTable = _api.Format(MasterTable, "Master");
                 
                    dt.Tables.Add(MasterTable);
                    
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

       
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPeriodID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nPeriodID", nPeriodID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Acc_Period", "N_PeriodID", nPeriodID, "", connection);

                    if (Results > 0)
                    {
                       // dLayer.DeleteData("Wh_BinTranHistoryDetails", "N_BinTransID", nBinTransID, "", connection);
                        return Ok(_api.Success(" deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }
             [HttpPost("monthlyPeriod")]
          public ActionResult MonthlyPeriod([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();

                    int nPeriodID = myFunctions.getIntVAL(MasterRow["N_PeriodID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["N_FnYearID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());

                       SortedList PostingParam = new SortedList();
                            PostingParam.Add("N_CompanyID", nCompanyID);
                            PostingParam.Add("N_FnYearID", nFnYearID);
                            PostingParam.Add("D_Start", MasterRow["d_start"].ToString());
                            PostingParam.Add("D_End",MasterRow["d_End"].ToString());
                            PostingParam.Add("X_Type",MasterRow["type"].ToString());
                            try
                            {
                                dLayer.ExecuteNonQueryPro("SP_FinancialYear_Create_New", PostingParam, connection, transaction);
                            }  
                         catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,ex));
                        }
                   

                    transaction.Commit();
                    return Ok(_api.Success("Financial Period Created")) ;
                }
            }
            
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }
    }
}


  