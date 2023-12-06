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
using System.Threading;

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
                    Params.Add("N_YearID", 16);
                    Params.Add("N_FormID", 1305);
                    string LeadCode = dLayer.GetAutoNumber("CRM_Leads", "X_LeadCode", Params, connection, transaction);
                    if (LeadCode == "") { transaction.Rollback(); return Ok(api.Warning("Unable to Generate Lead Number")); }

                    if (!Details.Columns.Contains("companyname"))
                        Details = myFunctions.AddNewColumnToDataTable(Details, "companyname", typeof(string), null);

                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_CompanyId", typeof(int), 1);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_FnyearID", typeof(int), 16);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_LeadID", typeof(int), 0);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_Createduser", typeof(int), 0);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_LeadCode", typeof(int), LeadCode);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_Lead", typeof(string), Details.Rows[0]["companyname"].ToString());
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_ContactName", typeof(string), Details.Rows[0]["contactname"].ToString());
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_Email", typeof(string), Details.Rows[0]["email"].ToString());
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_Phone1", typeof(string), Details.Rows[0]["phone"].ToString());
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_Company", typeof(string), Details.Rows[0]["companyname"].ToString());
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_Referredby", typeof(string), Details.Rows[0]["referredby"].ToString());
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_City", typeof(string), Details.Rows[0]["city"].ToString());
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_subsource", typeof(int), 304);


                    string Description = "";
                    if (Details.Columns.Contains("product"))
                    {
                        Description = "Product : " + Details.Rows[0]["product"].ToString() ;
                        MasterTable.Rows[0]["X_Lead"]= Details.Rows[0]["product"].ToString() + " for " +  Details.Rows[0]["companyname"].ToString();
                    }
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_ProjectDescription", typeof(string), Description);
                    MasterTable.Rows[0]["X_LeadCode"] = LeadCode;
                    DataTable Maildata = dLayer.ExecuteDataTable("select * from gen_mailtemplates where N_CompanyId=-1 and X_TemplateName='Schedule Demo Request'", Params, connection, transaction);

                    if (MasterTable.Rows.Count > 0)
                    {
                        N_LeadID = dLayer.SaveData("CRM_Leads", "N_LeadID", MasterTable, connection, transaction);
                        if (Maildata.Rows.Count > 0)
                        {
                            string Data = "";
                            if (Details.Columns.Contains("product"))
                                Data = "Product : " + Details.Rows[0]["product"].ToString() + "<br>Company : " + Details.Rows[0]["companyname"].ToString() + "<br>Contact : " + Details.Rows[0]["contactname"].ToString() + "<br>Email : " + Details.Rows[0]["email"].ToString() + "<br>Phone : " + Details.Rows[0]["phone"].ToString() + "<br>";
                            else
                                Data = "Company : " + Details.Rows[0]["companyname"].ToString() + "<br>Contact : " + Details.Rows[0]["contactname"].ToString() + "<br>Email : " + Details.Rows[0]["email"].ToString() + "<br>Phone : " + Details.Rows[0]["phone"].ToString() + "<br>";
                            
                            
                            string body = Maildata.Rows[0]["x_body"].ToString().Replace("@message", Data);
                            
                            //User MAIL
                            DataTable Maildata1 = dLayer.ExecuteDataTable("select * from gen_mailtemplates where N_CompanyId=-1 and X_TemplateName='Schedule User Request'", Params, connection, transaction);
                            string Userbody = Maildata1.Rows[0]["x_body"].ToString().Replace("@party", Details.Rows[0]["contactname"].ToString());

                            myFunctions.SendMail("sales@olivotech.com", body, Maildata.Rows[0]["x_subject"].ToString(), dLayer, 0, 0, 0, false);
                            myFunctions.SendMail("manzoor@upgrodigital.com", body, Maildata.Rows[0]["x_subject"].ToString(), dLayer, 0, 0, 0, false);
                            myFunctions.SendMail(Details.Rows[0]["email"].ToString(), Userbody, Maildata1.Rows[0]["x_subject"].ToString(), dLayer, 0, 0, 0, false);
                            //Whatsapp
                            string Company = myFunctions.GetCompanyName(User);
                            object WhatsappAPI = dLayer.ExecuteScalar("select X_Value from Gen_Settings where N_CompanyID=1 and X_Group='1334' and X_Description='Whatsapp Message'", Params, connection, transaction);
                            //object Receip = "+966506195881,+919895213365";
                           
                            // string[] splitStrings = Receip.ToString().Split(',');
                            // int i = 0;
                            // Data = "*DEMO REQUEST*%0A%0A" + Data.Replace("<br>", "%0A");
                            // foreach (string str in splitStrings)
                            // {
                            //     string URLAPI = "https://api.textmebot.com/send.php?recipient=" + str + "&apikey=" + WhatsappAPI + "&text=" + Data;
                            //     var handler = new HttpClientHandler
                            //     {
                            //         ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                            //     };
                            //     var client = new HttpClient(handler);
                            //     var clientFile = new HttpClient(handler);
                            //     var MSG = client.GetAsync(URLAPI);
                            //     MSG.Wait();
                            //     if (i == 0)
                            //         Thread.Sleep(6000);
                            //     i = +1;
                            // }


                            ///WHATSAPP
                            string CompanyNO = "+966506195881";
                            string UserNO = "+"+Details.Rows[0]["phone"].ToString();
                            Data = "*DEMO REQUEST*%0A%0A" + Data.Replace("<br>", "%0A");
                            
                            Userbody="Hi "+Details.Rows[0]["contactname"].ToString()+",<br>Exciting news! 🚀 Olivo Technologies has a powerful ERP solution tailored for your business needs. Experience it firsthand with a personalized demo.<br><br>🌐 Demo Link: https://demo.olivoerp.com<br>🔐 User: demo@olivoerp.com<br>🔑 Password: demo@olivoerp.com<br><br>Let us know your availability, and we'll schedule a demo at your convenience. Ready to revolutionize your business processes?<br>Best,<br>Olivo Technologies";
                            Userbody = Userbody.Replace("<br>", "%0A");

                                string URLAPI = "https://api.textmebot.com/send.php?recipient=" + CompanyNO + "&apikey=" + WhatsappAPI + "&text=" + Data;
                                var handler = new HttpClientHandler
                                {
                                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                                };
                                var client = new HttpClient(handler);
                                var clientFile = new HttpClient(handler);
                                var MSG = client.GetAsync(URLAPI);
                                MSG.Wait();
                                Thread.Sleep(6000);

                                URLAPI = "https://api.textmebot.com/send.php?recipient=" + UserNO + "&apikey=" + WhatsappAPI + "&text=" + Userbody;
                                MSG = client.GetAsync(URLAPI);
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

