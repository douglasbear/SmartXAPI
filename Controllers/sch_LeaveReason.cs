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
    [Route("schLeaveReason")]
    [ApiController]
    public class Sch_LeaveReason : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =1538 ;


        public Sch_LeaveReason(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = 
            conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("details")]
        public ActionResult LeaveReasonDetails(int n_ReasonID)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from Sch_LeavingReason where N_CompanyID=@p1  and N_ReasonID=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", n_ReasonID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                
                    MasterTable = api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);
                }
                return Ok(api.Success(dt));               
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }



        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int n_FnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nReasonID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ReasonID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    
                    string Code = "";
                    string x_ReasonCode = MasterTable.Rows[0]["x_ReasonCode"].ToString();
                    if (x_ReasonCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", n_FnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        x_ReasonCode = dLayer.GetAutoNumber("Sch_LeavingReason", "x_ReasonCode", Params, connection, transaction);
                        if (x_ReasonCode == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Reason Code")); }
                        MasterTable.Rows[0]["x_ReasonCode"] = x_ReasonCode;
                    }
                    MasterTable.Columns.Remove("n_FnYearId");

                    if (nReasonID > 0) 
                    {  
                        dLayer.DeleteData("Sch_LeavingReason", "n_ReasonID", nReasonID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    }

                    nReasonID = dLayer.SaveData("Sch_LeavingReason", "n_ReasonID", MasterTable, connection, transaction);
                    if (nReasonID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Reason Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        [HttpGet("list") ]
        public ActionResult ReasonList(int nCompanyID)
        {    
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="";

            sqlCommandText="select * from Sch_LeavingReason where N_CompanyID=@p1";

            param.Add("@p1", nCompanyID);                 
                
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
                }
                if(dt.Rows.Count==0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
                
            }
            catch(Exception e)
            {
                return Ok(api.Error(User,e));
            }   
        }   
      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nReasonID)
        {

            int Results = 0;
            int nCompanyID=myFunctions.GetCompanyID(User);
            try
            {                        
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Sch_LeavingReason ", "N_ReasonID", nReasonID, "N_CompanyID =" + nCompanyID, connection, transaction);
                
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("Reason deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User,"Unable to delete Reason"));
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }



        }
    }
}

