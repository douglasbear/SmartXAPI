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
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("freetext-sales")]
    [ApiController]
    public class Api_FTSales : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly ISec_UserRepo _repository;

        public Api_FTSales(ISec_UserRepo repository, IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _repository = repository;
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString =
            conf.GetConnectionString("SmartxConnection");
        }
        
        [HttpPost()]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable dt;
                MasterTable = ds.Tables["master"];
                int nSalesID = 0;
                string Auth = "";
                if (Request.Headers.ContainsKey("Authorization"))
                    Auth = Request.Headers["Authorization"];
                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "pkey_code", typeof(int), 0);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    dt = dLayer.ExecuteDataTable("select * from sec_user where  X_Token='" + Auth + "'", Params, connection, transaction);
                    if (dt.Rows.Count > 0)
                    {
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_CompanyID", typeof(int), dt.Rows[0]["N_CompanyID"]);
                        
                        Params.Add("N_CompanyID", MasterTable.Rows[0]["N_CompanyID"]);
                        Params.Add("N_FnyearID", MasterTable.Rows[0]["N_FnyearID"]);
                        Params.Add("N_UserID", MasterTable.Rows[0]["N_UserID"]);
                        Params.Add("N_BranchID", MasterTable.Rows[0]["N_BranchID"]);
                        Params.Add("N_LocationID", MasterTable.Rows[0]["N_LocationID"]);
                        Params.Add("X_Transtype", "FTSALES");
                        MasterTable.Columns.Remove("N_FnyearID");
                        MasterTable.Columns.Remove("N_BranchID");
                        MasterTable.Columns.Remove("N_UserID");
                        MasterTable.Columns.Remove("N_LocationID");
                        

                        dLayer.ExecuteNonQuery("delete from Mig_SalesInvoice", Params, connection, transaction);
                        nSalesID = dLayer.SaveData("Mig_SalesInvoice", "pkey_code", MasterTable, connection, transaction);
                        dLayer.ExecuteNonQueryPro("SP_SalesInvoiceImport", Params, connection, transaction);
                        dLayer.ExecuteNonQuery("Update sec_user Set X_Token= '' where N_UserID = " + dt.Rows[0]["N_UserID"], Params, connection, transaction);

                    }
                    else
                        return Ok(api.Error(User, "Invalid Token"));



                    if (nSalesID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Freetext Sales Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }
    }
}

