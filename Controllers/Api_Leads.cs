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
    [Route("leadsdata")]
    [ApiController]
    public class Api_Leads : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly ISec_UserRepo _repository;

        public Api_Leads(ISec_UserRepo repository, IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _repository = repository;
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString =
            conf.GetConnectionString("SmartxConnection");
        }

        [HttpPost()]
        public string Authkey(string contactname, string email, string mobile, string company, string industry, string date, string time, string Lead)
        {
            try
            {
                DataTable MasterTable = new DataTable();
                DataRow row = MasterTable.NewRow();
                MasterTable.Rows.Add(row);
                int N_LeadID = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    Params.Add("N_CompanyID", 1);
                    Params.Add("N_YearID", 9);
                    Params.Add("N_FormID", 1305);
                    string LeadCode = dLayer.GetAutoNumber("CRM_Leads", "X_LeadCode", Params, connection, transaction);
                    if (LeadCode == "") { transaction.Rollback(); return "0"; }
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_CompanyId", typeof(int), 1);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_FnyearID", typeof(int), 9);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_LeadID", typeof(int), 0);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_LeadCode", typeof(int), LeadCode);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_Lead", typeof(string), Lead);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_ContactName", typeof(string), contactname);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_Email", typeof(string), email);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_Phone1", typeof(string), mobile);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_Company", typeof(string), company);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_ProjectDescription", typeof(string), industry);
                    MasterTable.Rows[0]["X_LeadCode"] = LeadCode;



                    if (MasterTable.Rows.Count > 0)
                    {
                        N_LeadID = dLayer.SaveData("CRM_Leads", "N_LeadID", MasterTable, connection, transaction);

                    }
                    else
                        return "1";



                    if (N_LeadID <= 0)
                    {
                        transaction.Rollback();
                        return "1";
                    }
                    else
                    {
                        transaction.Commit();
                        return "1";
                    }
                }
            }
            catch (Exception ex)
            {
                return "1";
            }
        }
    }
}

