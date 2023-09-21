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
        private readonly string connectionString1;
        private readonly ISec_UserRepo _repository;

        public Api_Authkey(ISec_UserRepo repository, IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _repository = repository;
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString =
            conf.GetConnectionString("OlivoClientConnection");
            connectionString1=conf.GetConnectionString("SmartxConnection");
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
                    object N_UserID = dLayer.ExecuteScalar("select N_UserID from users where  x_emailid='" + username + "' and x_password='" + genpassword + "'", connection, transaction);
                    if (N_UserID != null)
                    {
                        tocken = Generatetocken();
                    }
                }
                using (SqlConnection connection1 = new SqlConnection(connectionString1))
                {
                    connection1.Open();
                    SqlTransaction transaction = connection1.BeginTransaction();
                    object N_UserID = dLayer.ExecuteScalar("select N_UserID from Sec_User where  X_UserID='" + username + "'", connection1, transaction);
                    if(tocken!="")
                    {
                        dLayer.ExecuteNonQuery("Update sec_user Set X_Token= '" + tocken + "' where N_UserID = " + N_UserID, Params, connection1, transaction);
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

