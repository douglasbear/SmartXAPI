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
    [Route("InvAdjustmentReasons")]
    [ApiController]
    public class Inv_AdjustmentReasonMaster : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 611;


        public Inv_AdjustmentReasonMaster(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetInvAdjustmentReason(int nFnYearID,bool adjustment)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@p2", nFnYearID);
            string sqlCommandText="";
            sqlCommandText="Select * from vw_Inv_StockAdjstmentReason_Disp Where N_CompanyID=@nCompanyID and N_FnYearID=@p2";
            if(adjustment)
              sqlCommandText="Select X_Description as X_Reason,N_ReasonID,X_ReasonCode,b_ISstockIn from vw_Inv_StockAdjstmentReason_Disp Where N_CompanyID=@nCompanyID and N_FnYearID=@p2";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

       [HttpGet("Grouplist")]
        public ActionResult GetGroupList(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@p2", nFnYearID);
            string sqlCommandText="Select * from vw_AccMastGroup Where N_CompanyID=@nCompanyID and N_FnYearID=@p2";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

      


          [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nReasonID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ReasonID"].ToString());
                int nGroupID=myFunctions.getIntVAL(MasterTable.Rows[0]["n_GroupID"].ToString());
                 MasterTable.Columns.Remove("n_GroupID");


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string ReasonCode = "";
                    var values = MasterTable.Rows[0]["X_ReasonCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        ReasonCode = dLayer.GetAutoNumber("Inv_StockAdjstmentReason", "X_ReasonCode", Params, connection, transaction);
                        if (ReasonCode == "") { return Ok(_api.Error(User,"Unable to generate Reason Code")); }
                        MasterTable.Rows[0]["X_ReasonCode"] = ReasonCode;
                        

                    }
                   
                   
                   nReasonID = dLayer.SaveData("Inv_StockAdjstmentReason", "n_ReasonID", MasterTable, connection, transaction);
                   

                   if (nReasonID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success(" Inventory Adjustment Reason Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(User,ex));
            }
        }
      
             
        [HttpGet("details")]
        public ActionResult GetDetails(int nReasonID)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText="select * from vw_Inv_StockAdjstmentReason_Disp where N_CompanyID=@nCompanyID and N_ReasonID=@nReasonID";
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nReasonID",nReasonID);


              

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection); 
                       
                    }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(_api.Notice("No Results Found" ));
                        } 
                        else
                        {
                            return Ok(_api.Success(dt));
                        }
            }
            catch(Exception e)
            {

                return Ok(_api.Error(User,e));
            }
        }

          [HttpDelete("delete")]
        public ActionResult DeleteData(int nReasonID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Inv_StockAdjstmentReason", "n_ReasonID", nReasonID, "", connection);
                    if (Results > 0)
                    {
                        return Ok( _api.Success("deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete "));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }
       

     
    }
}