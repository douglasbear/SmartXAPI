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
        private readonly int nFormID = 1471;

        public Inv_Device(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("list")]
        public ActionResult invDeviceTypeList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            
           
            string sqlCommandText = "select  * from Inv_Device where N_CompanyID=@p1 order by X_DeviceCode desc";
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
                    int N_DeviceID = myFunctions.getIntVAL(MasterRow["N_DeviceID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["N_CompanyID"].ToString());
                    int N_UserID = myFunctions.getIntVAL(MasterRow["n_UserID"].ToString());
                    string x_SerialNo =MasterRow["X_SerialNo"].ToString();
                    string x_DeviceCode = MasterRow["X_DeviceCode"].ToString();
                        


                    if (x_DeviceCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_FormID", this.nFormID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_UserID", N_UserID);
                        x_DeviceCode = dLayer.GetAutoNumber("Inv_Device", "X_DeviceCode", Params, connection, transaction);
                        if (x_DeviceCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Device Number");
                        }
                        Master.Rows[0]["X_DeviceCode"] = x_DeviceCode;


            object serialNoCount = dLayer.ExecuteScalar("select count(N_DeviceID) from Inv_Device  Where X_SerialNo ='" + x_SerialNo.Trim() + "' and N_CompanyID=" + N_CompanyID, Params, connection,transaction);
                         if( myFunctions.getIntVAL(serialNoCount.ToString())>0){
                              transaction.Rollback();
                             return Ok(api.Error(User, "Serial Number Already Exist"));
                         }


                         if(MasterRow["X_DeviceName"]==null||MasterRow["X_DeviceName"]==""){
                            MasterRow["X_DeviceName"]=MasterRow["X_SerialNo"];
                         }
                    }
                    if (MasterTable.Columns.Contains("n_FnYearID"))
                    {

                        MasterTable.Columns.Remove("n_FnYearID");

                    }
                    if (Details.Columns.Contains("n_FormID"))
                    {

                        Details.Columns.Remove("n_FormID");

                    }

                    if (N_DeviceID > 0)
                    {
                        dLayer.DeleteData("Inv_DeviceDetails", "N_DeviceID", N_DeviceID, "N_CompanyID=" + N_CompanyID + " and N_DeviceID=" + N_DeviceID, connection, transaction);
                        dLayer.DeleteData("Inv_Device", "N_DeviceID", N_DeviceID, "N_CompanyID=" + N_CompanyID + " and N_DeviceID=" + N_DeviceID, connection, transaction);
                    }
                    // string DupCriteria = "";


                    N_DeviceID = dLayer.SaveData("Inv_Device", "N_DeviceID", "", "", Master, connection, transaction);
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
                    Result.Add("N_DeviceID",N_DeviceID);

                    return Ok(api.Success(Result, "Device Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nDeviceID, int nCompanyID)
        {
            int Results = 0;
            nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                  object N_ServiceInfoID = dLayer.ExecuteScalar("select (N_ServiceInfoID) from Inv_ServiceInfo  Where N_CompanyID=" + nCompanyID + " and N_DeviceID="+nDeviceID, connection);
                  if(N_ServiceInfoID!=null){
                                   object serialNoCount = dLayer.ExecuteScalar("select count(N_ServiceID) from Inv_SalesOrderDetails  Where N_CompanyID=" + nCompanyID + " and N_ServiceID="+N_ServiceInfoID, connection);

                      if( myFunctions.getIntVAL(serialNoCount.ToString())>0){

                             return Ok(api.Error(User, "Unable To Delete!"));
                         }

                  }

                    Results = dLayer.DeleteData("Inv_Device", "N_DeviceID", nDeviceID, "N_CompanyID=" + nCompanyID + "", connection);
                    dLayer.DeleteData("Inv_DeviceDetails", "N_DeviceID", nDeviceID, "N_CompanyID=" + nCompanyID + "", connection);

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
        public ActionResult device(int n_DeviceID, int nCompanyID, string x_SerialNo, bool list)
        {
            DataTable Master = new DataTable();
            DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            nCompanyID = myFunctions.GetCompanyID(User);
            string xCriteria = "",
            sqlCommandText = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", n_DeviceID);

            Params.Add("@p3", x_SerialNo);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    sqlCommandText = "select * from Vw_Inv_Device Where N_CompanyID = @p1 and x_SerialNo = @p3";

                    if (list)
                    {
                        sqlCommandText = "select * from Vw_Inv_Device Where N_CompanyID = @p1 ";
                    }
                    Master = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    Master = api.Format(Master, "master");


                    if (Master.Rows.Count == 0)
                    {
                        ds.Tables.Add(Master);
                        return Ok(api.Success(ds));
                    }
                    else
                    {
                        Params.Add("@n_DeviceID", Master.Rows[0]["n_DeviceID"].ToString());
                        ds.Tables.Add(Master);
                        sqlCommandText = "Select * from Vw_Inv_DeviceDetails Where N_CompanyID=@p1 and n_DeviceID=@n_DeviceID";
                        Detail = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    
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
