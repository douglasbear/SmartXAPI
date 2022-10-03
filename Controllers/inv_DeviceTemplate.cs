
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
     [Route("invDeviceTemplate")]
     [ApiController]
    public class invDeviceTemplateList : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
        private readonly IMyFunctions myFunctions;
        private readonly IApiFunctions _api;
        private readonly int nFormID = 1472;
        public invDeviceTemplateList(IDataAccessLayer dl,IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            _api = api;
            myFunctions=myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }



       [HttpGet("list")]
        public ActionResult invDeviceTypeList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            
           
            string sqlCommandText = "select  * from Inv_DeviceTemplate where N_CompanyID=@p1 order by N_TemplateID desc";
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
       
          [HttpGet("fillDefaults")]
        public ActionResult invDeviceFillDefaults(int nTemplateID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            
           
            string sqlCommandText = "select  * from Inv_DeviceTemplateDetails where N_CompanyID=@p1 and N_TemplateID=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2",nTemplateID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                    dt = api.Format(dt);
                    return Ok(api.Success(dt));

                }
                
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }


                    [HttpDelete("delete")]
        public ActionResult DeleteData(int nTemplateID, int nCompanyID)
        {
            int Results = 0;
             nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Inv_DeviceTemplate", "N_TemplateID", nTemplateID, "N_CompanyID=" + nCompanyID  + "", connection);
                    dLayer.DeleteData("Inv_DeviceTemplateDetails", "N_TemplateID", nTemplateID, "N_CompanyID=" + nCompanyID + "", connection);

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
                    int N_TemplateID = myFunctions.getIntVAL(MasterRow["N_TemplateID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["N_CompanyID"].ToString());
                    int N_UserID = myFunctions.getIntVAL(MasterRow["n_UserID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    string X_TemplateCode = MasterRow["X_TemplateCode"].ToString();

                    
               
                    if (X_TemplateCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_FormID", nFormID);
                        Params.Add("N_YearID", N_FnYearID);
                        X_TemplateCode = dLayer.GetAutoNumber("Inv_DeviceTemplate", "X_TemplateCode", Params, connection, transaction);
                        if (X_TemplateCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Device Number");
                        }
                        Master.Rows[0]["X_TemplateCode"] = X_TemplateCode;
                    }
                  
                      if (N_TemplateID > 0)
                    {
                        dLayer.DeleteData("Inv_DeviceTemplateDetails", "N_TemplateID", N_TemplateID, "N_CompanyID=" + N_CompanyID + " and N_TemplateID=" + N_TemplateID, connection, transaction);
                        dLayer.DeleteData("Inv_DeviceTemplate", "N_TemplateID", N_TemplateID, "N_CompanyID=" + N_CompanyID + " and N_TemplateID=" + N_TemplateID, connection, transaction);
                    }
                    // string DupCriteria = "";
                     MasterTable.Columns.Remove("n_FnYearId");

                     N_TemplateID = dLayer.SaveData("Inv_DeviceTemplate", "N_TemplateID","","", Master, connection, transaction);
                    if (N_TemplateID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                    for (int i = 0; i < Details.Rows.Count; i++)
                    {
                        Details.Rows[i]["N_TemplateID"] = N_TemplateID;

                    }

                    dLayer.SaveData("Inv_DeviceTemplateDetails", "N_TemplateDetailsID", Details, connection, transaction);
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


               [HttpGet("details")]
        public ActionResult device(int n_TemplateID, int nCompanyID)
        {
             DataTable Master = new DataTable();
              DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            nCompanyID = myFunctions.GetCompanyID(User);
            string xCriteria = "", 
            sqlCommandText = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", n_TemplateID);
            // Params.Add("@p3",x_SerialNo);
           
         try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    sqlCommandText = "select * from Inv_DeviceTemplate Where N_CompanyID = @p1 and N_TemplateID = @p2";
                 Master = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                 Master = api.Format(Master, "master");

                  
                    if (Master.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        Params.Add("@n_TemplateID", Master.Rows[0]["n_TemplateID"].ToString());
                       ds.Tables.Add(Master);
                        sqlCommandText = "Select * from Inv_DeviceTemplateDetails Where N_CompanyID=@p1 and N_TemplateID=@p2";
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