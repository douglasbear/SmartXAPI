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
    [Route("tskUserMapping")]
    [ApiController]
    public class tsk_UserMapping : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =1583 ;


        public tsk_UserMapping(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = 
            conf.GetConnectionString("SmartxConnection");
        }


           
           [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nUserMappingID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserMappingID"].ToString());
                int nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_UserMappingCode"].ToString();

                      if (nUserMappingID > 0)
                    {

                        dLayer.ExecuteNonQuery("delete from tsk_UserMapping Where N_CompanyID = "+nCompanyID+" and N_UserID = "+nUserID+"", connection, transaction);
                        dLayer.ExecuteNonQuery("delete from tsk_UserMappingDetails Where N_CompanyID = "+nCompanyID+"  and N_UserID ="+nUserID+"", connection, transaction);

                    }

                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("tsk_UserMapping", "X_UserMappingCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Route Code")); }
                        MasterTable.Rows[0]["X_UserMappingCode"] = Code;
                    }
                    MasterTable.Columns.Remove("n_FnYearID");

                    nUserMappingID = dLayer.SaveData("tsk_UserMapping", "N_UserMappingID", MasterTable, connection, transaction);
                    if (nUserMappingID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_UserMappingID"] = nUserMappingID;
                       
                       
                    }
                    int nRouteDetailID = dLayer.SaveData("tsk_UserMappingDetails", "N_UserMappingDetailID", DetailTable, connection, transaction);
                    // if (nRouteDetailID <= 0)
                    // {
                    //     transaction.Rollback();
                    //     return Ok("Unable to save ");
                    // }
                    transaction.Commit();
                    return Ok(api.Success("Saved Successfully"));

                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }


        
             [HttpGet("details")]
        public ActionResult BusRegDetails(string nUserID)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_tsk_UserMapping where N_CompanyID=@p1 and N_UserID=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", nUserID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    if (MasterTable.Rows.Count == 0)
                    {
                       // return Ok(api.Success(OutPut));
                    }
                
                    MasterTable = api.Format(MasterTable, "Master");
                    string DetailSql = "select * from tsk_UserMappingDetails where N_CompanyID=@p1 and N_UserID=@p2";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = api.Format(DetailTable, "Details");

                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(api.Success(dt));
                }           
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
        [HttpGet("list")]
        public ActionResult GetteamList(int nUserID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyId", nCompanyID);
            Params.Add("@nUserID", nUserID);
            string sqlCommandText ="";
            if(nUserID>0)
                sqlCommandText = "Select *  from tsk_UserMapping Where N_CompanyID= " + nCompanyID + " and n_userid="+nUserID + " and X_Team is not null";
            else
                sqlCommandText = "Select *  from tsk_UserMapping Where N_CompanyID= " + nCompanyID + " and X_Team is not null";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
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
                return Ok(api.Error(User, e));
            }
        }
    }
}
