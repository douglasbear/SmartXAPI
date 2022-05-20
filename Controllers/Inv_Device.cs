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
    [Route("device")]
    [ApiController]
    public class Inv_Device : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int nFormID = 1463;

        public Inv_Device(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
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
                    SortedList Params = new SortedList();
                    DataRow MasterRow = Master.Rows[0];
                    int N_DeviceID = myFunctions.getIntVAL(MasterRow["N_DeviceID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["N_CompanyID"].ToString());
                    int N_UserID = myFunctions.getIntVAL(MasterRow["n_UserID"].ToString());
                    string x_DeviceCode = MasterRow["X_DeviceCode"].ToString();

                    
               
                    if (x_DeviceCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_FormID", nFormID);
                        Params.Add("N_UserID", N_UserID);
                        x_DeviceCode = dLayer.GetAutoNumber("Inv_Device", "X_DeviceCode", Params, connection, transaction);
                        if (x_DeviceCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Device Number");
                        }
                        Master.Rows[0]["X_DeviceCode"] = x_DeviceCode;
                    }

                      if (N_DeviceID > 0)
                    {
                        dLayer.DeleteData("Inv_DeviceDetails", "N_DeviceID", N_DeviceID, "N_CompanyID=" + N_CompanyID + " and N_DeviceID=" + N_DeviceID, connection, transaction);
                        dLayer.DeleteData("Inv_Device", "N_DeviceID", N_DeviceID, "N_CompanyID=" + N_CompanyID + " and N_DeviceID=" + N_DeviceID, connection, transaction);
                    }
                    // string DupCriteria = "";


                     N_DeviceID = dLayer.SaveData("Inv_Device", "N_DeviceID","","", Master, connection, transaction);
                    if (N_DeviceID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                    for (int i = 0; i < Details.Rows.Count; i++)
                    {
                        Details.Rows[i]["N_DeviceID"] = N_DeviceID;

                    }

                    dLayer.SaveData("Inv_DeviceDetails", "N_DeviceDetailsID", Details, connection, transaction);
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
        public ActionResult DeleteData(int n_DeviceID, int nCompanyID)
        {
            int Results = 0;
             nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Inv_Device", "N_DeviceID", n_DeviceID, "N_CompanyID=" + nCompanyID  + "", connection);
                    dLayer.DeleteData("Inv_DeviceDetails", "N_DeviceID", n_DeviceID, "N_CompanyID=" + nCompanyID + "", connection);

                }
                if (Results > 0)
                {
                    return Ok(api.Success("Device deleted"));
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
        public ActionResult EmployeeEvaluation(int n_DeviceID, int nCompanyID)
        {
             DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            nCompanyID = myFunctions.GetCompanyID(User);
            string xCriteria = "", sqlCommandText = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", n_DeviceID);

              sqlCommandText = "select * from vw_inv_device Where N_CompanyID = @p1 and n_DeviceID = @p2";
               sqlCommandText = "select * from vw_inv_deviceDetails Where N_CompanyID = @p1 and n_DeviceID = @p2";
           
         try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

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
                return Ok(api.Error(User, e));
            }
        }
    }
}