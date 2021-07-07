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
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("emailtemplate")]
    [ApiController]
    public class Gen_EmailTemplate : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1348;

        public Gen_EmailTemplate(IDataAccessLayer dl, IApiFunctions _api, IMyFunctions myFun, IConfiguration conf)
        {
            api = _api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [AllowAnonymous]


        [HttpPost("send")]
        public ActionResult SendData([FromBody] DataSet ds)
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
                    string Email = MasterRow["X_ClientMail"].ToString();
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
                    return Ok("SUCCESS");

                }
            }

            catch (Exception ie)
            {
                return Ok(api.Error(ie));
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
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nTemplateID = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string TemplateCode = "";
                    var values = MasterTable.Rows[0]["X_TemplateCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", 1302);
                        TemplateCode = dLayer.GetAutoNumber("Gen_MailTemplates", "X_TemplateCode", Params, connection, transaction);
                        if (TemplateCode == "") { transaction.Rollback(); return Ok(api.Error("Unable to generate Code")); }
                        MasterTable.Rows[0]["X_TemplateCode"] = TemplateCode;
                    }

                    nTemplateID = dLayer.SaveData("Gen_MailTemplates", "N_TemplateID", MasterTable, connection, transaction);
                    if (nTemplateID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();

                        return Ok(api.Success("Mail Template Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }

        [HttpGet("list")]
        public ActionResult TemplateList()
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";

            string sqlCommandText = "";
            string Criteria = "";


            sqlCommandText = "select  * from Gen_MailTemplates where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from Gen_MailTemplates where N_CompanyID=@p1";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
        [HttpGet("details")]
        public ActionResult TemplateListDetails(string x_TemplateCode)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "";


            sqlCommandText = "select  * from Gen_MailTemplates where N_CompanyID=@p1 and x_TemplateCode=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", x_TemplateCode);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(dt));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
         [HttpDelete("delete")]
        public ActionResult DeleteData(int nTemplateID)
        {

            int Results = 0;
            try
            {


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Gen_MailTemplates", "N_TemplateID", nTemplateID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("N_TemplateID", nTemplateID.ToString());
                    return Ok(api.Success(res, "Email Template deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete Email Template"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }



        }



    }
}