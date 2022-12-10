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
    [Route("genImportRule")]
    [ApiController]
    public class Gen_ImportRule : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int nFormID = 1175;

        public Gen_ImportRule(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("defaultImportDetails")]
        public ActionResult GetImportListDetails(int nFormID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select 0 as N_RuleID," + myFunctions.GetCompanyID(User) + " as N_CompanyID,0 as N_RuleDetailsID, X_FieldName,N_FieldID,'' as X_ColumnRefName from vw_GenImportRuleList where N_CompanyID=-1 and N_FormID=@p2 and b_IsDefault=1";
            Params.Add("@p2", nFormID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                    return Ok(api.Success(dt));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

         [HttpGet("list")]
        public ActionResult GetImportList(int nFormID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from Gen_ImportRuleMaster where N_CompanyID=@p1 and isnull(b_IsDefault,0)=0 and N_FormID=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFormID);
           
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                  return Ok(api.Success(dt));
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


             [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();



                    DataTable Master = ds.Tables["master"];
                    DataTable Details = ds.Tables["details"];
                     DataTable MasterTable;
                      MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    DataRow MasterRow = Master.Rows[0];
                     // DataRow MasterRow = MasterTable.Rows[0];
                    int N_RuleID = myFunctions.getIntVAL(MasterRow["N_RuleID"].ToString());
                     int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["N_CompanyID"].ToString());
                    string X_RuleCode = MasterRow["X_RuleCode"].ToString();
                    if (X_RuleCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_FormID", nFormID);
                          Params.Add("N_YearID", nFnYearID);
                      
                        X_RuleCode = dLayer.GetAutoNumber("Gen_ImportRuleMaster", "X_RuleCode", Params, connection, transaction);
                        if (X_RuleCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Device Number");
                        }
                        Master.Rows[0]["X_RuleCode"] = X_RuleCode;
                    }
                     if (MasterTable.Columns.Contains("n_FnYearID"))
                    {

                        MasterTable.Columns.Remove("n_FnYearID");

                    }
                      if (N_RuleID > 0)
                    {
                        dLayer.DeleteData("Gen_ImportRuleDetails", "N_RuleID", N_RuleID, "N_CompanyID=" + N_CompanyID + " and N_RuleID=" + N_RuleID, connection, transaction);
                        dLayer.DeleteData("Gen_ImportRuleMaster", "N_RuleID", N_RuleID, "N_CompanyID=" + N_CompanyID + " and N_RuleID=" + N_RuleID, connection, transaction);
                    }
                    // string DupCriteria = "";


                     N_RuleID = dLayer.SaveData("Gen_ImportRuleMaster", "N_RuleID","","", Master, connection, transaction);
                    if (N_RuleID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                    for (int i = 0; i < Details.Rows.Count; i++)
                    {
                        Details.Rows[i]["N_RuleID"] = N_RuleID;

                    }

                    dLayer.SaveData("Gen_ImportRuleDetails", "N_RuleDetailsID", Details, connection, transaction);
                    transaction.Commit();
                    SortedList Result = new SortedList();

                    return Ok(api.Success(Result, "Device Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nRuleID, int nCompanyID)
        {
            int Results = 0;
             nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Gen_ImportRuleMaster", "N_RuleID", nRuleID, "N_CompanyID=" + nCompanyID  + "", connection);
                    dLayer.DeleteData("Gen_ImportRuleDetails", "N_RuleID", nRuleID, "N_CompanyID=" + nCompanyID + "", connection);

                }
                if (Results > 0)
                {
                    return Ok(api.Success("Import rule deleted"));
                }
                else
                {
                    return Ok(api.Error(User, "Unable to delete"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }

        }


    [HttpGet("details")]
        public ActionResult GenImportRuleDetails(int nRuleID, int nCompanyID)
        {
             DataTable Master = new DataTable();
              DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            nCompanyID = myFunctions.GetCompanyID(User);
            string xCriteria = "", 
            sqlCommandText = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nRuleID);
           
           
         try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    sqlCommandText = "select * from Gen_ImportRuleMaster Where N_CompanyID = @p1 and N_RuleID = @p2";
                 
                
                    Master = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    Master = api.Format(Master, "master");

                  
                    if (Master.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        Params.Add("@nRuleID", Master.Rows[0]["n_RuleID"].ToString());
                       ds.Tables.Add(Master);
                        sqlCommandText = "Select * from Gen_ImportRuleDetails Where N_CompanyID=@p1 and N_RuleID=@nRuleID";
                        Detail = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                  
                       if (Detail.Rows.Count == 0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }
                             Detail = api.Format(Detail, "Detail");
                        ds.Tables.Add(Detail);
                      

                    }
                 

                
              
                }
                 return Ok(api.Success(ds));
            }
            catch (Exception e)
            { 
                return Ok(api.Error(User, e));
            }

        }   
    }
}