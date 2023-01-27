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
    [Route("apiimport")]
    [ApiController]
    public class Api_Import : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly ISec_UserRepo _repository;

        public Api_Import(ISec_UserRepo repository, IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _repository = repository;
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString =
            conf.GetConnectionString("SmartxConnection");
        }
        
        [HttpGet("details")]
        public ActionResult ContactListDetails(int nCompanyId,DateTime invoicedate)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "select * from Mig_SalesInvoice where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", invoicedate);


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
                    return Ok(api.Warning("No Results Found"));
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

        [HttpPost("import")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nSalesID = 0;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    if (MasterTable.Rows.Count > 0)
                    {
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

                        dLayer.ExecuteNonQuery("delete from Mig_SalesInvoice where B_Skipped<>1", Params, connection, transaction);
                        nSalesID = dLayer.SaveData("Mig_SalesInvoice", "pkey_code", MasterTable, connection, transaction);

                        dLayer.ExecuteNonQueryPro("SP_SalesInvoiceImport", Params, connection, transaction);

                    }

                    if (nSalesID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Imported"));
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

