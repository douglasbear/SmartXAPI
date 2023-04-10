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
    [Route("tskSprint")]
    [ApiController]
    public class Tsk_sprintmaster : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1478;

        public Tsk_sprintmaster(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


      
       [HttpPost("save")]
      public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {

                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                int nSprintID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_SprintID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Sprintcode = "";
                    string X_SprintCode = MasterTable.Rows[0]["X_SprintCode"].ToString();
                    if (X_SprintCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_SprintID",nSprintID);
                        Params.Add("N_FormID",1478);
                        Sprintcode = dLayer.GetAutoNumber("Tsk_SprintMaster", "X_SprintCode", Params, connection, transaction);
                        if (Sprintcode == "") { transaction.Rollback(); return Ok(api.Error(User,"Unable to generate Sprintcode")); }
                        MasterTable.Rows[0]["X_SprintCode"] = Sprintcode;
                    }
                        if (MasterTable.Columns.Contains("n_FnYearID"))
                    {

                        MasterTable.Columns.Remove("n_FnYearID");

                    }

                    nSprintID = dLayer.SaveData("Tsk_SprintMaster", "N_SprintID", MasterTable, connection, transaction);
                    if (nSprintID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Saved successfully"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(User,ex));
            }
        }
    [HttpGet("list")]
        public ActionResult GetItemCategory()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from VW_Tsk_SprintMaster where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyId);

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

      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nSPrintID, int nCompanyID)
        {
            int Results = 0;
             nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Tsk_SprintMaster", "N_SprintID", nSPrintID, "N_CompanyID=" + nCompanyID  + "", connection);
                    

                }
                if (Results > 0)
                {
                    return Ok(api.Success("Sprint deleted"));
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
        public ActionResult device(int nSprintID, int nCompanyID,string xSprintCode)
        {
             DataTable Master = new DataTable();
              DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            nCompanyID = myFunctions.GetCompanyID(User);
            string xCriteria = "", 
            sqlCommandText = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nSprintID);
            Params.Add("@p3",xSprintCode);
           
         try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    sqlCommandText = "select * from VW_Tsk_SprintMaster Where N_CompanyID = @p1 and x_SprintCode = @p3";
                 Master = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                 Master = api.Format(Master, "master");

                  
                    if (Master.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        Params.Add("@nSprintID", Master.Rows[0]["N_SprintID"].ToString());
                       ds.Tables.Add(Master);

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

    
