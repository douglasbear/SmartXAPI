using SmartxAPI.Data;
using SmartxAPI.Dtos.Login;
using Microsoft.AspNetCore.Mvc;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SmartxAPI.Controllers
{
    
    [Route("clients")]
    [ApiController]
    public class Clients : ControllerBase
    {
        private readonly ISec_UserRepo _repository;
        private readonly IApiFunctions _api;

        private readonly IMyFunctions myFunctions;
        private readonly IDataAccessLayer dLayer;
        private readonly string connectionString;
        private readonly string olivoClientConnectionString;

        public Clients(ISec_UserRepo repository, IApiFunctions api, IMyFunctions myFun, IDataAccessLayer dl, IConfiguration conf)
        {
            _repository = repository;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            olivoClientConnectionString = conf.GetConnectionString("OlivoClientConnection");
        }
        [HttpPost("register")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                
               
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;


                    transaction = connection.BeginTransaction();
                    MasterTable.Columns.Add("x_Password", typeof(System.String));
                    DataRow MasterRow = MasterTable.Rows[0];
                    string Password =MasterRow["x_UserID"].ToString();
                    Password=myFunctions.EncryptString(Password);
                    MasterTable.Rows[0]["x_Password"] = Password;
                    int Result = dLayer.SaveData("Sec_User", "n_UserID", MasterTable, connection, transaction);
                    if (Result > 0)
                    {
                        //MULTI COMPANY USER CREATION
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    transaction.Commit();
                }
                 return Ok(_api.Success("User Saved"));
            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.Error(ex));
            }
        }

    }
}