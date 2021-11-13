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
using System.Net.Mail;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("email")]
    [ApiController]
    public class Gen_EmailSettings : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1348;

        public Gen_EmailSettings(IDataAccessLayer dl, IApiFunctions _api, IMyFunctions myFun, IConfiguration conf)
        {
            api = _api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [AllowAnonymous]


        [HttpPost("send")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    int companyid = myFunctions.GetCompanyID(User);
                    DataTable Master = ds.Tables["master"];
                    DataRow MasterRow = Master.Rows[0];
                    SortedList Params = new SortedList();
                    string Toemail = "";
                    string Email = MasterRow["X_ContactEmail"].ToString();
                    string Body = MasterRow["X_Body"].ToString();
                    string Subjectval = MasterRow["X_Subject"].ToString();
                    Toemail = Email.ToString();
                    object companyemail = "";
                    object companypassword = "";

                    companyemail = dLayer.ExecuteScalar("select X_Value from Gen_Settings where X_Group='210' and X_Description='EmailAddress' and N_CompanyID=" + companyid, Params, connection, transaction);
                    companypassword = dLayer.ExecuteScalar("select X_Value from Gen_Settings where X_Group='210' and X_Description='EmailPassword' and N_CompanyID=" + companyid, Params, connection, transaction);
                    if (Toemail.ToString() != "")
                    {
                        if (companyemail.ToString() != "")
                        {
                            object body = null;
                            string MailBody;
                            body = Body;
                            if (body != null)
                            {
                                body = body.ToString();
                            }
                            else
                                body = "";


                            string Sender = companyemail.ToString();
                            MailBody = body.ToString();
                            string Subject = Subjectval;



                            SmtpClient client = new SmtpClient
                            {
                                Host = "smtp.gmail.com",
                                Port = 587,
                                EnableSsl = true,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                Credentials = new System.Net.NetworkCredential(companyemail.ToString(), companypassword.ToString()),
                                Timeout = 10000,
                            };

                            MailMessage message = new MailMessage();
                            message.To.Add(Toemail.ToString()); // Add Receiver mail Address  
                            message.From = new MailAddress(Sender);
                            message.Subject = Subject;
                            message.Body = MailBody;

                            message.IsBodyHtml = true; //HTML email  
                            string CC = GetCCMail(256, companyid, connection, transaction, dLayer);
                            if (CC != "")
                                message.CC.Add(CC);

                            string Bcc = GetBCCMail(256, companyid, connection, transaction, dLayer);
                            if (Bcc != "")
                                message.Bcc.Add(Bcc);
                            client.Send(message);

                        }
                    }
                   return Ok(api.Success("Email Send"));

                }
            }

            catch (Exception ie)
            {
                return Ok(api.Error(User,ie));
            }
        }
         public static string GetCCMail(int ID, int nCompanyID, SqlConnection connection, SqlTransaction transaction, IDataAccessLayer dLayer)
        {
            SortedList Params = new SortedList();
            object CCMail = dLayer.ExecuteScalar("select X_CCMail from Gen_EmailAddresses where N_subjectID =" + ID + " and N_CompanyID=" + nCompanyID, Params, connection, transaction);
            if (CCMail != null)
                return CCMail.ToString();
            else
                return "";
        }
        public static string GetBCCMail(int ID, int nCompanyID, SqlConnection connection, SqlTransaction transaction, IDataAccessLayer dLayer)
        {
            SortedList Params = new SortedList();
            object BCCMail = dLayer.ExecuteScalar("select X_BCCMail from Gen_EmailAddresses where N_subjectID =" + ID + " and N_CompanyID=" + nCompanyID, Params, connection, transaction);
            if (BCCMail != null)
                return BCCMail.ToString();
            else
                return "";
        }

    }
}