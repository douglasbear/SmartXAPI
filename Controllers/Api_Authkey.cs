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
    [Route("authkey")]
    [ApiController]
    public class Api_Authkey : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly ISec_UserRepo _repository;

        public Api_Authkey(ISec_UserRepo repository, IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _repository = repository;
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString =
            conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet()]
        public string Authkey(string username, string password)
        {
            string tocken = "";
            try
            {
                SortedList Params = new SortedList();
                var genpassword = myFunctions.EncryptString(password);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    object N_UserID = dLayer.ExecuteScalar("select N_UserID from sec_user where  X_UserID='" + username + "' and x_password='" + genpassword + "'", connection, transaction);
                    if (N_UserID != null)
                    {
                        tocken = Generatetocken();
                        dLayer.ExecuteNonQuery("Update sec_user Set X_Token= '" + tocken + "' where N_UserID = " + N_UserID, Params, connection, transaction);
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        tocken = "Invalid Username or Password";
                    }
                }
                return tocken;
            }
            catch (Exception e)
            {
                return "Error";
            }
        }

        //Save....
        // [HttpPost("freetext-sales")]
        // public ActionResult SaveData([FromBody] DataSet ds)
        // {
        //     try
        //     {
        //         DataTable MasterTable;
        //         DataTable dt;
        //         MasterTable = ds.Tables["master"];
        //         int nSalesID = 0;
        //         string Auth = "";
        //         if (Request.Headers.ContainsKey("Authorization"))
        //             Auth = Request.Headers["Authorization"];
        //         MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "pkey_code", typeof(int), 0);
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();
        //             SortedList Params = new SortedList();
        //             dt = dLayer.ExecuteDataTable("select * from sec_user where  X_Token='" + Auth + "'", Params, connection, transaction);
        //             if (dt.Rows.Count > 0)
        //             {
        //                 MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_CompanyID", typeof(int), dt.Rows[0]["N_CompanyID"]);
        //                 // dLayer.ExecuteNonQuery("delete from Mig_SalesInvoice", Params, connection, transaction);
        //                 nSalesID = dLayer.SaveData("Mig_SalesInvoice", "pkey_code", MasterTable, connection, transaction);
        //                 // object N_FnyearID = dLayer.ExecuteScalar("select MAX(N_FnyearID) from Acc_Fnyear where N_CompanyID=" + dt.Rows[0]["N_CompanyID"], connection, transaction);
        //                 // Params.Add("N_CompanyID", dt.Rows[0]["N_CompanyID"]);
        //                 // Params.Add("N_FnyearID", myFunctions.getIntVAL(N_FnyearID.ToString()));
        //                 // Params.Add("N_UserID", dt.Rows[0]["N_UserID"]);
        //                 // Params.Add("N_BranchID", dt.Rows[0]["N_BranchID"]);
        //                 // Params.Add("N_LocationID", dt.Rows[0]["N_LocationID"]);
        //                 // dLayer.ExecuteNonQueryPro("SP_SalesInvoiceImport", Params, connection, transaction);
        //                 dLayer.ExecuteNonQuery("Update sec_user Set X_Token= '' where N_UserID = " + dt.Rows[0]["N_UserID"], Params, connection, transaction);

        //             }
        //             else
        //                 return Ok(api.Error(User, "Invalid Token"));



        //             if (nSalesID <= 0)
        //             {
        //                 transaction.Rollback();
        //                 return Ok(api.Error(User, "Unable to save"));
        //             }
        //             else
        //             {
        //                 transaction.Commit();
        //                 return Ok(api.Success("Sales Invoice Saved"));
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(api.Error(User, ex));
        //     }
        // }
        private string Generatetocken()
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");
            GuidString = GuidString.Replace("/", "");
            return GuidString;

        }
    }
}

