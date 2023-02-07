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
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("genDocuments")]
    [ApiController]
    public class Gen_DocumentManager : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly string reportLocation;
        private readonly int FormID;
        private readonly IMyAttachments myAttachments;
        public string RPTLocation = "";
        public Gen_DocumentManager(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            reportLocation = conf.GetConnectionString("ReportLocation");
            FormID = 0;
        }



        [HttpGet("chart")]
        public ActionResult CategoryList()
        {
            DataTable dtMasterFolder = new DataTable();
            DataTable dtMasterFiles = new DataTable();
            SortedList Params = new SortedList();

            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlMasterFolder = "Select * from DMS_MasterFolder Where N_CompanyID=@nCompanyID Order By X_Name";
            string sqlMasterFiles = "Select * From DMS_MasterFiles Where  N_CompanyID=@nCompanyID Order By N_FolderID,X_FileCode";

            Params.Add("@nCompanyID", nCompanyID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dtMasterFolder = dLayer.ExecuteDataTable(sqlMasterFolder, Params, connection);
                    dtMasterFiles = dLayer.ExecuteDataTable(sqlMasterFiles, Params, connection);
                }
                dtMasterFolder = api.Format(dtMasterFolder);
                dtMasterFiles = api.Format(dtMasterFiles);
                SortedList OutPut = new SortedList();
                OutPut.Add("MasterFolder", dtMasterFolder);
                OutPut.Add("MasterFiles", dtMasterFiles);

                return Ok(api.Success(OutPut));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("getFile")]
        public async Task<IActionResult> Download(int fileID, string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SortedList param = new SortedList();
                    param.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    path = dLayer.ExecuteScalar("select ISNULL(X_Value,'') AS X_Value from Gen_Settings where X_Description ='EmpDocumentLocation' and N_CompanyID =@nCompanyID", param, connection).ToString();
                }


            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
            path = path + filename;

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, api.GetContentType(path), Path.GetFileName(path));
        }
        private bool LoadReportDetails(int nFnYearID, int nFormID)
        {
            SortedList QueryParams = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            QueryParams.Add("@nCompanyId", nCompanyId);
            QueryParams.Add("@nFnYearID", nFnYearID);
            QueryParams.Add("@nFormID", nFormID);
            RPTLocation = "";
            string xUserCategoryList = myFunctions.GetUserCategoryList(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction();
                    object ObjTaxType = dLayer.ExecuteScalar("SELECT Acc_TaxType.X_RepPathCaption FROM Acc_TaxType LEFT OUTER JOIN Acc_FnYear ON Acc_TaxType.N_TypeID = Acc_FnYear.N_TaxType where Acc_FnYear.N_CompanyID=@nCompanyId and Acc_FnYear.N_FnYearID=@nFnYearID", QueryParams, connection, transaction);
                    if (ObjTaxType == null)
                        ObjTaxType = "";
                    if (ObjTaxType.ToString() == "")
                        ObjTaxType = "none";
                    string TaxType = ObjTaxType.ToString();

                    object ObjPath = dLayer.ExecuteScalar("SELECT X_RptFolder FROM Gen_PrintTemplates WHERE N_CompanyID =@nCompanyId and N_FormID=@nFormID", QueryParams, connection, transaction);
                    if (ObjPath != null)
                    {
                        if (ObjPath.ToString() != "")
                            RPTLocation = reportLocation + "printing/" + ObjPath + "/" + TaxType + "/";
                        else
                            RPTLocation = reportLocation + "printing/";
                    }
                    object Custom = dLayer.ExecuteScalar("SELECT isnull(b_Custom,0) FROM Gen_PrintTemplates WHERE N_CompanyID =@nCompanyId and N_FormID=@nFormID and N_UsercategoryID in (" + xUserCategoryList + ")", QueryParams, connection, transaction);
                    int N_Custom = myFunctions.getIntVAL(Custom.ToString());
                    if (N_Custom == 1)
                        RPTLocation = RPTLocation + "custom/";
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;

            }

        }



        [HttpPost("saveGeneralDocs")]
        public ActionResult SaveEmployeeGeneralDocuments([FromBody] DataSet ds)
        {
            try
            {
                DataTable Attachment = ds.Tables["attachments"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string payCode = Attachment.Rows[0]["x_TransCode"].ToString();
                    int payId = myFunctions.getIntVAL(Attachment.Rows[0]["n_TransID"].ToString());
                    string partyCode = Attachment.Rows[0]["x_PartyCode"].ToString();
                    int partyID = myFunctions.getIntVAL(Attachment.Rows[0]["n_PartyID"].ToString());
                    string folderName = Attachment.Rows[0]["x_FolderName"].ToString();
                    string partyName = Attachment.Rows[0]["x_PartyName"].ToString();

                    if (Attachment.Rows[0]["x_FolderName"].ToString() == "reports")
                    {
                        var base64Data = Regex.Match(Attachment.Rows[0]["FileData"].ToString(), @"data:(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;
                        byte[] FileBytes = Convert.FromBase64String(base64Data);
                        System.IO.File.WriteAllBytes(reportLocation + Attachment.Rows[0]["x_File"].ToString(),
                                           FileBytes);
                        return Ok(api.Success("Report Updated"));
                    }
                    if (Attachment.Rows[0]["x_FolderName"].ToString() == "print")
                    {
                        int nFormID = myFunctions.getIntVAL(Attachment.Rows[0]["nFormID"].ToString());
                        int nFnYearID = myFunctions.getIntVAL(Attachment.Rows[0]["nFnYearID"].ToString());
                        if (LoadReportDetails(nFnYearID, nFormID))
                        {
                            var base64Data = Regex.Match(Attachment.Rows[0]["FileData"].ToString(), @"data:(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;
                            byte[] FileBytes = Convert.FromBase64String(base64Data);
                            System.IO.File.WriteAllBytes(RPTLocation + Attachment.Rows[0]["x_File"].ToString(),
                                               FileBytes);
                            return Ok(api.Success("Print Updated"));
                        }
                    }

                    Attachment.Columns.Remove("x_FolderName");
                    Attachment.Columns.Remove("x_PartyName");
                    Attachment.Columns.Remove("x_PartyCode");
                    Attachment.Columns.Remove("x_TransCode");
                    Attachment.AcceptChanges();

                    SqlTransaction transaction = connection.BeginTransaction();
                    myAttachments.SaveAttachment(dLayer, Attachment, payCode, payId, partyName, partyCode, partyID, folderName, User, connection, transaction);
                    transaction.Commit();

                }
                return Ok(api.Success("Documents Updated"));
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }



        [HttpGet("getAttachments")]
        public ActionResult GetSalesInvoiceDetails(int nTransID, int nPartyID, int nFormID, int nFnYearID)
        {

            try
            {
                using (SqlConnection Con = new SqlConnection(connectionString))
                {
                    Con.Open();
                    DataTable Attachments = myAttachments.ViewAttachment(dLayer, nPartyID, nTransID, nFormID, nFnYearID, User, Con);
                    Attachments = api.Format(Attachments, "attachments");
                    return Ok(api.Success(Attachments));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }



    }
}