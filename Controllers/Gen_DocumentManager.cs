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
        public Gen_DocumentManager(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
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


    }
}