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
using System.Net;
using System.Net.Http;

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
        public ActionResult Leadsave([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable = new DataTable();
                DataRow row = MasterTable.NewRow();
                MasterTable.Rows.Add(row);
                int N_LeadID = 0;
                DataTable Details = ds.Tables["master"];

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    Params.Add("N_CompanyID", 1);
                    Params.Add("N_YearID", 9);
                    Params.Add("N_FormID", 1305);
                    string LeadCode = dLayer.GetAutoNumber("CRM_Leads", "X_LeadCode", Params, connection, transaction);
                    if (LeadCode == "") { transaction.Rollback(); return Ok(api.Warning("Unable to Generate Lead Number")); }

                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_CompanyId", typeof(int), 1);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_FnyearID", typeof(int), 9);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_LeadID", typeof(int), 0);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_Createduser", typeof(int), 0);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_LeadCode", typeof(int), LeadCode);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_Lead", typeof(string), Details.Rows[0]["companyname"].ToString());
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_ContactName", typeof(string), Details.Rows[0]["contactname"].ToString());
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_Email", typeof(string), Details.Rows[0]["email"].ToString());
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_Phone1", typeof(string), Details.Rows[0]["phone"].ToString());
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_Company", typeof(string), Details.Rows[0]["companyname"].ToString());
                    string Description = "Product "+Details.Rows[0]["product"].ToString()+ ", Preferred Date " + Details.Rows[0]["date"].ToString() + ", Preferred Time " + Details.Rows[0]["time"].ToString() ;
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_ProjectDescription", typeof(string), Description);
                    MasterTable.Rows[0]["X_LeadCode"] = LeadCode;
                    DataTable Maildata = dLayer.ExecuteDataTable("select * from gen_mailtemplates where N_CompanyId=-1 and X_TemplateName='Schedule Demo Request'", Params, connection, transaction);

                    if (MasterTable.Rows.Count > 0)
                    {
                        N_LeadID = dLayer.SaveData("CRM_Leads", "N_LeadID", MasterTable, connection, transaction);
                        if (Maildata.Rows.Count > 0)
                        {
                            string Data = "Product : "+Details.Rows[0]["product"].ToString()+"<br>Date : " + Details.Rows[0]["date"].ToString() + "<br>Time : " + Details.Rows[0]["time"].ToString()+ "<br>Company : " + Details.Rows[0]["companyname"].ToString() + "<br>Contact : " + Details.Rows[0]["contactname"].ToString() + "<br>Email : " + Details.Rows[0]["email"].ToString() + "<br>Phone : " + Details.Rows[0]["phone"].ToString() + "<br>";
                            string body = Maildata.Rows[0]["x_body"].ToString().Replace("@message", Data);

                            myFunctions.SendMail("sales@olivotech.com", body, Maildata.Rows[0]["x_subject"].ToString(), dLayer, 0, 0, 0);
                            //Whatsapp
                            string Company = myFunctions.GetCompanyName(User);
                            object WhatsappAPI = dLayer.ExecuteScalar("select X_Value from Gen_Settings where N_CompanyID=1 and X_Group='1334' and X_Description='Whatsapp Message'", Params, connection, transaction);
                            //object Employee = dLayer.ExecuteScalar("select x_empname from vw_PayEmployee where n_companyid=" + myFunctions.GetCompanyID(User) + " and  n_empid=" + nEmpID, Params, connection, transaction);
                            object Receip = dLayer.ExecuteScalar("select x_phone1 from acc_company where n_companyid=1", Params, connection, transaction);
                            Data ="*DEMO REQUEST*%0A%0A"+ Data.Replace("<br>", "%0A");
                            string URLAPI = "https://api.textmebot.com/send.php?recipient=" + Receip + "&apikey=" + WhatsappAPI + "&text=" + Data;
                            var handler = new HttpClientHandler
                            {
                                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                            };
                            var client = new HttpClient(handler);
                            var clientFile = new HttpClient(handler);
                            var MSG = client.GetAsync(URLAPI);
                            MSG.Wait();

                        }

                    }
                    else
                        return Ok(api.Error("Unable to save"));



                    if (N_LeadID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Lead saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error("Unable to save"));
            }
        }
    }
}

