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
        private readonly int FormID;
        private readonly string reportPath;
        private readonly string startupPath;
        private readonly IMyAttachments myAttachments;
        public Gen_DocumentManager(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 0;
            reportPath = conf.GetConnectionString("ReportPath");
            startupPath = conf.GetConnectionString("StartupPath");
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
                return Ok(api.Error(e));
            }
        }

        [HttpGet("getFile")]
        public async Task<IActionResult> Download(int fileID,string filename)
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
                return Ok(api.Error(e));
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



        [HttpPost("saveGeneralDocs")]
        public ActionResult SaveEmployeeGeneralDocuments([FromBody] DataSet ds)
        {
            try
            {
                DataTable Attachment = ds.Tables["attachments"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlTransaction transaction = connection.BeginTransaction();
                    myAttachments.SaveAttachment(dLayer, Attachment, "0", 0 , "General Documents", "0", 0, "General Documents", User, connection, transaction);

                }
                return Ok(api.Success("Documents Updated"));
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }


    }
}