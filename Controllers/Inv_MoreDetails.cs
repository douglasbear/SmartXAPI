

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
    [Route("deviceInfo")]
    [ApiController]
    public class Inv_DeviceInfo : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int nFormID = 1545;

        public Inv_DeviceInfo(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
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
                     DataTable MasterTable;
                      MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    DataRow MasterRow = Master.Rows[0];
                     // DataRow MasterRow = MasterTable.Rows[0];
                    int N_DeviceInfoID = myFunctions.getIntVAL(MasterRow["N_DeviceInfoID"].ToString());
                     int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["N_CompanyID"].ToString());
                    int N_UserID = myFunctions.getIntVAL(MasterRow["n_UserID"].ToString());
                    string X_DeviceInfoCode = MasterRow["X_DeviceInfoCode"].ToString();

                    
               
                    if (X_DeviceInfoCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_FormID", nFormID);
                          Params.Add("N_YearID", nFnYearID);
                        // Params.Add("N_UserID", N_UserID);
                        X_DeviceInfoCode = dLayer.GetAutoNumber("Inv_DeviceInfo", "X_DeviceInfoCode", Params, connection, transaction);
                        if (X_DeviceInfoCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Device Number");
                        }
                        Master.Rows[0]["X_DeviceInfoCode"] = X_DeviceInfoCode;
                    }
                     if (MasterTable.Columns.Contains("n_FnYearID"))
                    {

                        MasterTable.Columns.Remove("n_FnYearID");

                    }
                      if (N_DeviceInfoID > 0)
                    {
                        dLayer.DeleteData("Inv_DeviceInfoDetails", "N_DeviceInfoID", N_DeviceInfoID, "N_CompanyID=" + N_CompanyID + " and N_DeviceInfoID=" + N_DeviceInfoID, connection, transaction);
                        dLayer.DeleteData("Inv_DeviceInfo", "N_DeviceInfoID", N_DeviceInfoID, "N_CompanyID=" + N_CompanyID + " and N_DeviceInfoID=" + N_DeviceInfoID, connection, transaction);
                    }
                    // string DupCriteria = "";


                     N_DeviceInfoID = dLayer.SaveData("Inv_DeviceInfo", "N_DeviceInfoID","","", Master, connection, transaction);
                    if (N_DeviceInfoID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                    for (int i = 0; i < Details.Rows.Count; i++)
                    {
                        Details.Rows[i]["N_DeviceInfoID"] = N_DeviceInfoID;

                    }

                    dLayer.SaveData("Inv_DeviceInfoDetails", "N_DeviceDetailsID", Details, connection, transaction);
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
        public ActionResult DeleteData(int nDeviceInfoID, int nCompanyID)
        {
            int Results = 0;
             nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Inv_DeviceInfo", "nDeviceInfoID", nDeviceInfoID, "N_CompanyID=" + nCompanyID  + "", connection);
                    dLayer.DeleteData("Inv_DeviceInfoDetails", "nDeviceInfoID", nDeviceInfoID, "N_CompanyID=" + nCompanyID + "", connection);

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
        public ActionResult device(int nDeviceInfoID, int nCompanyID,string x_SerialNo, bool list)
        {
             DataTable Master = new DataTable();
              DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            nCompanyID = myFunctions.GetCompanyID(User);
            string xCriteria = "", 
            sqlCommandText = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nDeviceInfoID);
           
            Params.Add("@p3",x_SerialNo);
           
         try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    sqlCommandText = "select * from Vw_Inv_DeviceInfo Where N_CompanyID = @p1 and x_SerialNo = @p3";
                 
                   if(list)
                    {
                        sqlCommandText= "select * from Vw_Inv_DeviceInfo Where N_CompanyID = @p1 ";
                    }
                    Master = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    Master = api.Format(Master, "master");

                  
                    if (Master.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        Params.Add("@nDeviceInfoID", Master.Rows[0]["nDeviceInfoID"].ToString());
                       ds.Tables.Add(Master);
                        sqlCommandText = "Select * from Vw_Inv_DeviceDetails Where N_CompanyID=@p1 and n_DeviceInfoID=@n_DeviceInfoID";
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


          [HttpGet("list")]
        public ActionResult List()
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();



            string sqlCommandText = "select  * from [Inv_ServiceInfo] where N_CompanyID=@p1";

            Params.Add("@p1", nCompanyId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

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
    