
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
    [Route("chequePrint")]
    [ApiController]
    public class ACC_ChequePrint : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

         private readonly int nFormID = 1477;

        public ACC_ChequePrint(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            nFormID = 1477;
        }

        [HttpGet("list")]
        public ActionResult GetChequePrintList(int? nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
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


                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and ([X_ChequeNO] like '%" + xSearchkey + "%' or X_BankName like '%" + xSearchkey + "%')";

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_ChequeTranID desc";
                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "X_ChequeNO":
                                xSortBy = "X_ChequeNO" + xSortBy.Split(" ")[1];
                                break;
                            case "N_ChequeTranID":
                                xSortBy = "N_ChequeTranID" + xSortBy.Split(" ")[1];
                                break;
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }

                    int Count = (nPage - 1) * nSizeperpage;
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") [X_ChequeNO] AS X_ChequeNO,* from vw_AccChequeDetails where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") [X_ChequeNO] AS X_ChequeNO,* from vw_AccChequeDetails where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and N_ChequeTranID not in (select top(" + Count + ") N_ChequeTranID from vw_AccChequeDetails where N_CompanyID=@p1 and N_FnYearID=@p2 " + xSortBy + " ) " + xSortBy;

                    // sqlCommandText = "select * from Inv_MRNDetails where N_CompanyID=@p1";
                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearId);
                    SortedList OutPut = new SortedList();

             
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count from vw_AccChequeDetails where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                    }
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
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
       

      
         [HttpGet("details")]
        public ActionResult ChequePrint(string x_ChequeNO,int n_Companyid)
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
                    Params.Add("@x_ChequeNO", x_ChequeNO);
                    Mastersql = "select * from vw_AccChequeDetails where N_CompanyID=@nCompanyID and X_ChequeNO=@x_ChequeNO ";
                   
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(api.Warning("No data found")); }
                    int nChequeTranID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ChequeTranID"].ToString());
                    Params.Add("@nChequeTranID", nChequeTranID);

                    MasterTable = api.Format(MasterTable, "Master");
                 
                    dt.Tables.Add(MasterTable);
                    
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
          [HttpGet("bankList")]
        public ActionResult accBankList(int nFnYearID )
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            
           
              string sqlCommandText = "select * from vw_AccBank_Disp where N_CompanyID=@nCompanyId and N_FnYearID=@n_FnYearID";
              Params.Add("@nCompanyId", nCompanyId);
               Params.Add("@n_FnYearID", nFnYearID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
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
                
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
         [HttpGet("beneficiaryList")]
        public ActionResult accBeneficiaryList( )
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            
           
              string sqlCommandText = "select * from vw_InvBeneficiary_WithArabic_Disp where N_CompanyID=" + nCompanyId  ;
              Params.Add("@p1", nCompanyId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
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
                
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
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

                    int nChequeTranID = myFunctions.getIntVAL(MasterRow["N_ChequeTranID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["N_FnYearID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string xChequeNO = MasterRow["x_ChequeNO"].ToString();
                    string x_ChequeNO = "";
                    if (xChequeNO == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", nFormID);
                        x_ChequeNO = dLayer.GetAutoNumber("Acc_ChequeTransaction", "x_ChequeNO", Params, connection, transaction);
                        if (x_ChequeNO == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Code");
                        }
                        MasterTable.Rows[0]["x_ChequeNO"] = x_ChequeNO;
                    }
                    else
                    {
                         dLayer.DeleteData("Acc_ChequeTransaction", "N_ChequeTranID", nChequeTranID, "", connection,transaction);
           
                    }
                   
                    int n_ChequeTranID = dLayer.SaveData("Acc_ChequeTransaction", "N_ChequeTranID", "", "", MasterTable, connection, transaction);
                    if (n_ChequeTranID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                 

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_ChequeTranID", n_ChequeTranID);
                    Result.Add("x_ChequeNO", x_ChequeNO);
                    return Ok(api.Success(Result, "Cheque Print Created"));
                }
            }
            
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

   

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nChequeTranID)
        {

             int Results = 0;
            try
            {                        
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();                
                QueryParams.Add("@nFormID", 1445);
                QueryParams.Add("@nChequeTranID", nChequeTranID);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Acc_ChequeTransaction", "n_ChequeTranID", nChequeTranID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_ChequeTranID",nChequeTranID.ToString());
                    return Ok(api.Success(res," deleted"));
                }
                else
                {
                    return Ok(api.Error(User,"Unable to delete "));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }  
    }
    }