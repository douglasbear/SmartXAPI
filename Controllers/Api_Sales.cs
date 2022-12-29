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
    [Route("sync")]
    [ApiController]
    public class Api_Sales : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly ISec_UserRepo _repository;

        public Api_Sales(ISec_UserRepo repository, IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _repository = repository;
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString =
            conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("GetAPIKey")]
        public string GetAPIKey(string username, string password)
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
        [HttpPost("sales")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
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
                    object N_UserID = dLayer.ExecuteScalar("select N_UserID from sec_user where  X_Token='" + Auth + "'", connection, transaction);
                    if (N_UserID != null)
                        dLayer.ExecuteNonQuery("Update sec_user Set X_Token= '' where N_UserID = " + N_UserID, Params, connection, transaction);
                    else
                        return Ok(api.Error(User, "Invalid Token"));

                    nSalesID = dLayer.SaveData("Mig_SalesInvoice", "pkey_code", MasterTable, connection, transaction);

                    if (nSalesID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Sales Invoice Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }
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

