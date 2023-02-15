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
        private readonly string AppURL;
        private readonly int N_FormID = 1348;
        private readonly IMyAttachments myAttachments;

        public Gen_EmailTemplate(IDataAccessLayer dl, IApiFunctions _api, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            api = _api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            myAttachments = myAtt;

            AppURL = conf.GetConnectionString("AppURL");
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
                    string xRecruitmentCode = "";
                    string x_PartyName = "";
                    string Email = MasterRow["X_ContactEmail"].ToString();
                    string Body = MasterRow["X_Body"].ToString();
                    string Subjectval = MasterRow["x_TempSubject"].ToString();
                    int nTemplateID = myFunctions.getIntVAL(MasterRow["n_TemplateID"].ToString());
                    int nopportunityID = myFunctions.getIntVAL(MasterRow["N_OpportunityID"].ToString());
                    if (Master.Columns.Contains("x_RecruitmentCode"))
                        xRecruitmentCode = MasterRow["x_RecruitmentCode"].ToString();
                    if (Master.Columns.Contains("x_TemplateName"))
                        x_PartyName = MasterRow["x_TemplateName"].ToString();
                    Toemail = Email.ToString();
                    object companyemail = "";
                    object companypassword = "";
                    object Company, Oppportunity, Contact, CustomerID;
                    int nCompanyId = myFunctions.GetCompanyID(User);

                    companyemail = dLayer.ExecuteScalar("select X_Value from Gen_Settings where X_Group='210' and X_Description='EmailAddress' and N_CompanyID=" + companyid, Params, connection, transaction);
                    companypassword = dLayer.ExecuteScalar("select X_Value from Gen_Settings where X_Group='210' and X_Description='EmailPassword' and N_CompanyID=" + companyid, Params, connection, transaction);

                    string Subject = "";
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

                            if (nopportunityID > 0)
                            {
                                Oppportunity = dLayer.ExecuteScalar("select x_Opportunity from vw_CRMOpportunity where N_CompanyID =" + nCompanyId + " and N_OpportunityID=" + nopportunityID, Params, connection, transaction);
                                Contact = dLayer.ExecuteScalar("Select x_Contact from vw_CRMOpportunity where N_CompanyID=" + nCompanyId + " and N_OpportunityID=" + nopportunityID, Params, connection, transaction);
                                Company = dLayer.ExecuteScalar("select x_customer from vw_CRMOpportunity where N_CompanyID =" + nCompanyId + " and N_OpportunityID=" + nopportunityID, Params, connection, transaction);
                                CustomerID = dLayer.ExecuteScalar("select N_CustomerID from vw_CRMOpportunity where N_CompanyID =" + nCompanyId + " and N_OpportunityID=" + nopportunityID, Params, connection, transaction);


                                Body = Body.ToString().Replace("@CompanyName", Company.ToString());
                                Body = Body.ToString().Replace("@ContactName", Contact.ToString());
                                Body = Body.ToString().Replace("@LeadName", Oppportunity.ToString());

                                Subjectval = Subjectval.ToString().Replace("@CompanyName", Company.ToString());
                                Subjectval = Subjectval.ToString().Replace("@ContactName", Contact.ToString());
                                Subjectval = Subjectval.ToString().Replace("@LeadName", Oppportunity.ToString());


                            }
                            if (xRecruitmentCode != "")
                            {
                                object JobName = dLayer.ExecuteScalar("select x_postingtitle from vw_RecRegistrartion where N_CompanyID="+nCompanyId+"  and x_RecruitmentCode="+xRecruitmentCode, Params, connection, transaction);
                                object d_intDate = dLayer.ExecuteScalar("select D_InterviewDate from vw_RecRegistrartion where N_CompanyID="+nCompanyId+"  and x_RecruitmentCode="+xRecruitmentCode, Params, connection, transaction);
                                Body = Body.ToString().Replace("@CompanyName", myFunctions.GetCompanyName(User));
                                Body = Body.ToString().Replace("@PartyName", x_PartyName);
                                Body = Body.ToString().Replace("@JobName", JobName.ToString());
                                Body = Body.ToString().Replace("@InterviewDate", d_intDate.ToString());

                                Subjectval = Subjectval.ToString().Replace("@CompanyName", myFunctions.GetCompanyName(User));
                                Subjectval = Subjectval.ToString().Replace("@PartyName", x_PartyName);
                                Subjectval = Subjectval.ToString().Replace("@JobName", JobName.ToString());
                                Subjectval = Subjectval.ToString().Replace("@InterviewDate", d_intDate.ToString());


                            }

                            string Sender = companyemail.ToString();
                            Subject = Subjectval;
                            MailBody = body.ToString();
                            myFunctions.SendMail(Toemail, Body, Subject, dLayer, 1348, nTemplateID, companyid);

                        }
                    }
                    Master.Columns.Remove("x_TemplateCode");
                    Master.Columns.Remove("x_TemplateName");
                    Master.Columns.Remove("n_TemplateID");
                    Master.Columns.Remove("x_TempSubject");
                    if (Master.Columns.Contains("n_PkeyId"))
                        Master.Columns.Remove("n_PkeyId");
                    if (Master.Columns.Contains("n_PkeyIdSub"))
                        Master.Columns.Remove("n_PkeyIdSub");
                    if (Master.Columns.Contains("x_RecruitmentCode"))
                        Master.Columns.Remove("x_RecruitmentCode");
                    Master = myFunctions.AddNewColumnToDataTable(Master, "N_MailLogID", typeof(int), 0);
                    Master = myFunctions.AddNewColumnToDataTable(Master, "X_Subject", typeof(string), Subject);
                    Master.Columns.Remove("X_Body");

                    int N_LogID = dLayer.SaveData("Gen_MailLog", "N_MailLogID", Master, connection, transaction);
                    transaction.Commit();

                    return Ok(api.Success("Email Send"));


                }
            }

            catch (Exception ie)
            {
                return Ok(api.Error(User, ie));
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
                DataTable Attachment = ds.Tables["attachments"];
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
                        if (TemplateCode == "") { transaction.Rollback(); return Ok(api.Error(User, "Unable to generate Code")); }
                        MasterTable.Rows[0]["X_TemplateCode"] = TemplateCode;
                    }
                    var X_Body = MasterTable.Rows[0]["X_Body"].ToString();
                    MasterTable.Columns.Remove("X_Body");
                    if (MasterTable.Columns.Contains("n_PkeyIdSub"))
                        MasterTable.Columns.Remove("n_PkeyIdSub");
                    if (MasterTable.Columns.Contains("n_PkeyId"))
                        MasterTable.Columns.Remove("n_PkeyId");

                    nTemplateID = dLayer.SaveData("Gen_MailTemplates", "N_TemplateID", MasterTable, connection, transaction);

                    string payCode = MasterTable.Rows[0]["X_TemplateCode"].ToString();
                    int payId = nTemplateID;


                    //  string partyName= Attachment.Rows[0]["x_PartyName"].ToString();
                    if (Attachment.Rows.Count > 0)
                    {
                        string partyCode = Attachment.Rows[0]["x_PartyCode"].ToString();
                        int partyID = myFunctions.getIntVAL(Attachment.Rows[0]["n_PartyID"].ToString());
                        Attachment.Columns.Remove("x_FolderName");

                        Attachment.Columns.Remove("x_PartyCode");
                        Attachment.Columns.Remove("x_TransCode");
                        if (Attachment.Columns.Contains("n_PartyID1"))
                            Attachment.Columns.Remove("n_PartyID1");
                        if (Attachment.Columns.Contains("n_ActionID"))
                            Attachment.Columns.Remove("n_ActionID");
                        if (Attachment.Columns.Contains("tempFileName"))
                            Attachment.Columns.Remove("tempFileName");

                        Attachment.AcceptChanges();
                        myAttachments.SaveAttachment(dLayer, Attachment, payCode, payId, "", partyCode, partyID, "Email", User, connection, transaction);
                    }

                    if (nTemplateID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                    else
                    {
                        dLayer.ExecuteNonQuery("update Gen_MailTemplates set X_Body='" + X_Body + "' where N_CompanyID=@N_CompanyID and N_TemplateID=" + nTemplateID, Params, connection, transaction);
                        transaction.Commit();

                        return Ok(api.Success("Mail Template Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
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
                        // return Ok(api.Warning("No Results Found"));
                        return Ok(api.Success(OutPut));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
        [HttpGet("emaildetails")]
        public ActionResult EmailTemplateDetails(string nFormID)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "";

            sqlCommandText = "select  * from vw_MailGeneralScreenSettings where N_CompanyID=@p1 and N_MenuID=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFormID);

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
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("details")]
        public ActionResult TemplateListDetails(string n_TemplateID)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "";

            sqlCommandText = "select  * from Gen_MailTemplates where N_CompanyID=@p1 and N_TemplateID=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", n_TemplateID);

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
                return Ok(api.Error(User, e));
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
                    return Ok(api.Error(User, "Unable to delete Email Template"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }



        }


        [HttpGet("mailList")]
        public ActionResult GenMailList(string type, int pKeyID, int nFnYearID)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);

            string sqlCommandText = "";

            if (type.ToLower() == "rfq")
            {

                sqlCommandText = " SELECT        Inv_RFQVendorList.N_QuotationID AS N_PKeyID, Inv_RFQVendorList.N_VendorID AS N_PartyID, Inv_Vendor.X_VendorCode AS X_PartyCode, Sec_User.N_UserID, Sec_User.X_UserID, Inv_Vendor.X_Email, " +
"                         Inv_Vendor.X_VendorName AS X_PartyName, 'Vendor' AS X_PartyType, 'RFQ' AS X_TxnType, Inv_Vendor.N_FnYearID, Inv_VendorRequest.X_QuotationNo as X_DocNo " +
" FROM            Inv_VendorRequest RIGHT OUTER JOIN " +
"                         Inv_RFQVendorList ON Inv_VendorRequest.N_QuotationId = Inv_RFQVendorList.N_QuotationID AND Inv_VendorRequest.N_CompanyId = Inv_RFQVendorList.N_CompanyID LEFT OUTER JOIN " +
"                         Sec_User RIGHT OUTER JOIN " +
"                         Inv_Vendor ON Sec_User.N_CustomerID = Inv_Vendor.N_VendorID AND Sec_User.N_CompanyID = Inv_Vendor.N_CompanyID ON Inv_RFQVendorList.N_VendorID = Inv_Vendor.N_VendorID AND " +
"                         Inv_RFQVendorList.N_CompanyID = Inv_Vendor.N_CompanyID " +
     " where Inv_Vendor.N_FnYearID=@nFnYearID and Inv_RFQVendorList.N_CompanyID=@nCompanyID and Inv_RFQVendorList.N_QuotationID=@nPkeyID group by Inv_RFQVendorList.N_QuotationID,Inv_RFQVendorList.N_VendorID ,Inv_Vendor.X_VendorCode, Sec_User.N_UserID, Sec_User.X_UserID, Inv_Vendor.X_Email, Inv_Vendor.X_VendorName,Inv_Vendor.N_FnYearID, Inv_VendorRequest.X_QuotationNo ";
            }
            else if (type.ToLower() == "purchaseorder")
            {
                sqlCommandText = "SELECT        Inv_PurchaseOrder.N_POrderID AS N_PKeyID, Inv_PurchaseOrder.N_VendorID AS N_PartyID, Inv_Vendor.X_VendorCode AS X_PartyCode, Sec_User.N_UserID, Inv_Vendor.X_Email, Sec_User.X_UserID,Inv_Vendor.X_VendorName AS X_PartyName, 'Vendor' AS X_PartyType, 'Purchase Order' AS X_TxnType,Inv_PurchaseOrder.N_FnYearID, Inv_PurchaseOrder.X_POrderNo as X_DocNo " +
             " FROM            Inv_PurchaseOrder LEFT OUTER JOIN " +
             "                        Sec_User ON Inv_PurchaseOrder.N_CompanyID = Sec_User.N_CompanyID AND Inv_PurchaseOrder.N_VendorID = Sec_User.N_CustomerID LEFT OUTER JOIN " +
             "                        Inv_Vendor ON Inv_PurchaseOrder.N_CompanyID = Inv_Vendor.N_CompanyID AND Inv_PurchaseOrder.N_VendorID = Inv_Vendor.N_VendorID AND Inv_PurchaseOrder.N_FnYearID = Inv_Vendor.N_FnYearID" +
             " where Inv_Vendor.N_FnYearID=@nFnYearID and Inv_PurchaseOrder.N_CompanyID=@nCompanyID and Inv_PurchaseOrder.N_POrderID=@nPkeyID group by  Inv_PurchaseOrder.N_POrderID, Inv_PurchaseOrder.N_VendorID, Inv_Vendor.X_VendorCode,Sec_User.N_UserID, Inv_Vendor.X_Email,  Sec_User.X_UserID, Inv_Vendor.X_VendorName,Inv_PurchaseOrder.N_FnYearID, Inv_PurchaseOrder.X_POrderNo";
            }
            else if (type.ToLower() == "customer portal")
            {
                sqlCommandText = "SELECT N_CustomerID AS N_PKeyID,N_CustomerID AS N_PartyID,X_CustomerCode AS X_PartyCode,X_Email,X_CustomerName AS X_PartyName ,'customer' AS X_PartyType,'Customer Portal' AS X_TxnType,N_FnYearID,N_FnYearID  From Inv_Customer where  N_CompanyID=@nCompanyID and N_CustomerID=@nPkeyID";

            }
            else if (type.ToLower() == "amendment")
            {
                sqlCommandText = "SELECT        Pay_PayHistoryMaster.N_HistoryID AS N_PKeyID, Pay_PayHistoryMaster.N_EmpID AS N_PartyID, Pay_Employee.X_EmpCode AS X_PartyCode, Pay_Employee.X_EmailID as X_Email,Pay_Employee.X_EmpName AS X_PartyName, 'Employee' AS X_PartyType, 'Amendment' AS X_TxnType, Pay_PayHistoryMaster.X_HistoryCode as X_DocNo " +
              " FROM            Pay_PayHistoryMaster LEFT OUTER JOIN " +
              "                        Pay_Employee ON Pay_PayHistoryMaster.N_CompanyID = Pay_Employee.N_CompanyID AND Pay_PayHistoryMaster.N_EmpID = Pay_Employee.N_EmpID " +
              " where Pay_PayHistoryMaster.N_CompanyID=@nCompanyID and Pay_PayHistoryMaster.N_HistoryID=@nPkeyID group by  Pay_PayHistoryMaster.N_HistoryID, Pay_PayHistoryMaster.N_EmpID, Pay_Employee.X_EmpCode, Pay_Employee.X_EmailID, Pay_Employee.X_EmpName, Pay_PayHistoryMaster.X_HistoryCode";

            }

            DataTable dt = new DataTable();
            SortedList Params = new SortedList();


            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nCompanyID", nCompanyId);
            Params.Add("@nPkeyID", pKeyID);


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    return Ok(api.Success(api.Format(dt, "details")));
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }


        [HttpPost("processMailList")]
        public ActionResult ProcessMailList([FromBody] DataSet ds)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    int companyid = myFunctions.GetCompanyID(User);
                    DataTable Master = ds.Tables["master"];


                    foreach (DataRow row in Master.Rows)
                    {
                        string xBodyText = "";
                        string xSubject = "";
                        string xURL = "";
                        if (row["x_TxnType"].ToString().ToLower() == "rfq")
                        {
                            if (myFunctions.CreatePortalUser(companyid, myFunctions.getIntVAL(row["N_BranchID"].ToString()), row["X_PartyName"].ToString(), row["X_Email"].ToString(), row["X_PartyType"].ToString(), row["X_PartyCode"].ToString(), myFunctions.getIntVAL(row["N_PartyID"].ToString()), true, dLayer, connection, transaction))
                            {
                                //xSubject = "RFQ Inward";

                                object xInwardCode = dLayer.ExecuteScalar("select X_InwardsCode from Inv_RFQVendorListMaster where N_QuotationID=" + myFunctions.getIntVAL(row["N_PKeyID"].ToString()) + " and N_CompanyID=" + companyid + " and N_VendorID=" + myFunctions.getIntVAL(row["N_PartyID"].ToString()), connection, transaction);
                                if (xInwardCode == null)
                                {
                                    string inwardInsert = "insert into Inv_RFQVendorListMaster " +
                                    "select N_CompanyID,(select isnull(max(N_VendorListMasterID),0)+1 from Inv_RFQVendorListMaster) ,(select isnull(max(X_InwardsCode),0)+1 from Inv_RFQVendorListMaster),N_QuotationID,Getdate(),Getdate()," + myFunctions.getIntVAL(row["N_PartyID"].ToString()) + ",0    from Inv_VendorRequest where N_QuotationID=" + myFunctions.getIntVAL(row["N_PKeyID"].ToString()) + " and N_CompanyID=" + companyid;
                                    object inwardID = dLayer.ExecuteNonQuery(inwardInsert, connection, transaction);
                                    if (inwardID == null)
                                        inwardID = 0;

                                    if (myFunctions.getIntVAL(inwardID.ToString()) > 0)
                                    {
                                        xInwardCode = dLayer.ExecuteScalar("select isNull(X_InwardsCode,'') from Inv_RFQVendorListMaster where N_QuotationID=" + myFunctions.getIntVAL(row["N_PKeyID"].ToString()) + " and N_CompanyID=" + companyid + " and N_VendorID=" + myFunctions.getIntVAL(row["N_PartyID"].ToString()), connection, transaction).ToString();

                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        return Ok(api.Error(User, "Email Error"));

                                    }
                                }
                                string seperator = "$$";
                                xURL = myFunctions.EncryptStringForUrl(companyid + seperator + row["N_PartyID"].ToString() + seperator + row["X_TxnType"].ToString() + seperator + row["N_PKeyID"].ToString(), System.Text.Encoding.Unicode);
                                xURL = AppURL + "/client/vendor/14/" + xURL + "/rfqVendorInward/" + xInwardCode;

                                xSubject = dLayer.ExecuteScalar("select X_Subject from Gen_MailTemplates where N_CompanyId=" + companyid + " and X_Type='rfq'", connection, transaction).ToString();
                                xBodyText = dLayer.ExecuteScalar("select X_Body from Gen_MailTemplates where N_CompanyId=" + companyid + " and X_Type='rfq'", connection, transaction).ToString();

                                SortedList Params = new SortedList();
                                DataTable dtRFQ = dLayer.ExecuteDataTable("select D_DueDate,datename(dw,D_DueDate) AS X_DueDay from Inv_VendorRequest where N_CompanyID=" + companyid + " and N_QuotationID=" + myFunctions.getIntVAL(row["N_PKeyID"].ToString()), Params, connection, transaction);

                                xBodyText = xBodyText.Replace("@PartyName", row["X_PartyName"].ToString());
                                xBodyText = xBodyText.Replace("@URL", xURL);
                                xBodyText = xBodyText.Replace("@CompanyName", myFunctions.GetCompanyName(User));
                                xBodyText = xBodyText.Replace("@DueDate", dtRFQ.Rows[0]["D_DueDate"].ToString());
                                xBodyText = xBodyText.Replace("@DueDay", dtRFQ.Rows[0]["X_DueDay"].ToString());
                                // xBodyText = " Honored," +
                                //             " Through this email, I wish to formally request a price quotation for a selection of goods from your esteemed company." +
                                //             " Please fill out price quotation throug below link " +
                                //             xURL +
                                //             " In case you require any further information, or due to company policy we need to fill out a quotation form, do not hesitate to contact me." +
                                //             " I look forward to hearing from you and possibly doing business in the future.";
                                myFunctions.SendMailWithAttachments(618, myFunctions.getIntVAL(row["N_FnYearID"].ToString()), myFunctions.getIntVAL(row["N_PKeyID"].ToString()), myFunctions.getIntVAL(row["N_PartyID"].ToString()), row["X_PartyName"].ToString(), xSubject, row["X_DocNo"].ToString(), row["X_Email"].ToString(), xBodyText, dLayer, User);
                            }
                        }
                        else if (row["x_TxnType"].ToString().ToLower() == "purchase order")
                        {

                            xSubject = dLayer.ExecuteScalar("select X_Subject from Gen_MailTemplates where N_CompanyId=" + companyid + " and X_Type='Purchase Order'", connection, transaction).ToString();
                            xBodyText = dLayer.ExecuteScalar("select X_Body from Gen_MailTemplates where N_CompanyId=" + companyid + " and X_Type='Purchase Order'", connection, transaction).ToString();
                            xBodyText = xBodyText.Replace("@PartyName", row["X_PartyName"].ToString());

                            myFunctions.SendMailWithAttachments(82, myFunctions.getIntVAL(row["N_FnYearID"].ToString()), myFunctions.getIntVAL(row["N_PKeyID"].ToString()), myFunctions.getIntVAL(row["N_PartyID"].ToString()), row["X_PartyName"].ToString(), xSubject, row["X_DocNo"].ToString(), row["X_Email"].ToString(), xBodyText, dLayer, User);
                        }
                        else if (row["x_TxnType"].ToString().ToLower() == "customer portal")
                        {
                            xSubject = dLayer.ExecuteScalar("select X_Subject from Gen_MailTemplates where N_CompanyId=" + companyid + " and X_Type='customer portal'", connection, transaction).ToString();
                            xBodyText = dLayer.ExecuteScalar("select X_Body from Gen_MailTemplates where N_CompanyId=" + companyid + " and X_Type='customer portal'", connection, transaction).ToString();
                            xBodyText = xBodyText.Replace("@ContactName", row["X_PartyName"].ToString());
                            xBodyText = xBodyText.Replace("@CompanyName", myFunctions.GetCompanyName(User));
                            string seperator = "$$";
                            xURL = myFunctions.EncryptStringForUrl(myFunctions.GetCompanyID(User).ToString() + seperator + row["N_PartyID"].ToString() + seperator + "HOME" + seperator + "0", System.Text.Encoding.Unicode);
                            xURL = AppURL + "/client/customer/13/" + xURL + "/home/new";
                            xBodyText = xBodyText.Replace("@URL", xURL);
                            xSubject = xSubject.Replace("@CompanyName", myFunctions.GetCompanyName(User));



                            myFunctions.SendMailWithAttachments(0, myFunctions.getIntVAL(row["N_FnYearID"].ToString()), myFunctions.getIntVAL(row["N_PKeyID"].ToString()), myFunctions.getIntVAL(row["N_PartyID"].ToString()), row["X_PartyName"].ToString(), xSubject, "0", row["X_Email"].ToString(), xBodyText, dLayer, User);

                        }
                        else if (row["x_TxnType"].ToString().ToLower() == "amendment")
                        {
                            xSubject = dLayer.ExecuteScalar("select X_Subject from Gen_MailTemplates where N_CompanyId=" + companyid + " and X_Type='amendment'", connection, transaction).ToString();
                            xBodyText = dLayer.ExecuteScalar("select X_Body from Gen_MailTemplates where N_CompanyId=" + companyid + " and X_Type='amendment'", connection, transaction).ToString();
                            xBodyText = xBodyText.Replace("@ContactName", row["X_PartyName"].ToString());
                            xBodyText = xBodyText.Replace("@CompanyName", myFunctions.GetCompanyName(User));
                            string seperator = "$$";

                            xSubject = xSubject.Replace("@CompanyName", myFunctions.GetCompanyName(User));



                            myFunctions.SendMailWithAttachments(0, myFunctions.getIntVAL(row["N_FnYearID"].ToString()), myFunctions.getIntVAL(row["N_PKeyID"].ToString()), myFunctions.getIntVAL(row["N_PartyID"].ToString()), row["X_PartyName"].ToString(), xSubject, "0", row["X_Email"].ToString(), xBodyText, dLayer, User);

                        }
                    }

                    transaction.Commit();


                    return Ok(api.Success("Email Send"));


                }
            }

            catch (Exception ie)
            {
                return Ok(api.Error(User, ie));
            }
        }
    }
}