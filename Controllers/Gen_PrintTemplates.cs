using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("genPrintTemplates")]
    [ApiController]
    public class Gen_PrintTemplates : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID = 1344;
        private readonly string reportPath;

        public Gen_PrintTemplates(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            reportPath = conf.GetConnectionString("ReportPath");
        }
        [HttpGet("policyList")]
        public ActionResult GetPolicyList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select X_InsuranceCode,X_CardNo,X_InsuranceName,X_VendorName,X_StartDate,X_EndDate,N_MedicalInsID,N_CompanyID,N_VendorID from vw_MedicalInsurance where N_CompanyID=@nCompanyID  group By  X_InsuranceCode,X_CardNo,X_InsuranceName,X_VendorName,X_StartDate,X_EndDate,N_MedicalInsID,N_CompanyID,N_VendorID ";
            Params.Add("@nCompanyID", nCompanyID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        [HttpGet("user")]
        public ActionResult GetUser()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_UserRole_Disp where N_CompanyID=" + nCompanyID + " and Category <> 'Olivo'";
            Params.Add("@nCompanyID", nCompanyID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        [HttpGet("screen")]
        public ActionResult GetScreen(int nLanguageID, int n_UserCategoryID, int nModuleID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_PrintSelectDispRpt where  N_LanguageID = " + nLanguageID + " and B_Visible = 'true' and N_UserCategoryID=" + n_UserCategoryID + " and N_CompanyID=" + nCompanyID + "and N_ParentMenuID=" + nModuleID + "";
            Params.Add("@nCompanyID", nCompanyID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        [HttpGet("fillData")]
        public ActionResult GetPrintTemp(int reportSelectingScreenID, int languageID, int n_TaxTypeID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string X_ReportFilePath = "";
                    string X_FolderName = "";


                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);

                    //DataTable reportTable = new DataTable();
                    List<SortedList> templates = new List<SortedList>();

                    X_ReportFilePath += reportPath + languageID + @"\printing\";
                    object ObjFolderName = dLayer.ExecuteScalar("SELECT X_RptFolder FROM Gen_PrintTemplates WHERE N_CompanyID = '" + nCompanyID + "' AND N_FormID = " + reportSelectingScreenID, Params, connection);
                    if (ObjFolderName != null)
                        X_FolderName = ObjFolderName.ToString();
                    else
                        //X_FolderName = MYG.ReturnMultiLingualVal(ReportSelectingScreenID.ToString(), "X_ControlNo", "0").Replace(" ", "");
                        X_FolderName = "";
                    if (reportSelectingScreenID > 0)
                    {
                        X_ReportFilePath += X_FolderName + "\\";
                        if (n_TaxTypeID == 1)
                            X_ReportFilePath += @"vat\";
                        else if (n_TaxTypeID == 2)
                            X_ReportFilePath += @"gst\";
                        else if (n_TaxTypeID == 3)
                            X_ReportFilePath += @"gst+cess\";
                        else
                            X_ReportFilePath += @"none\";
                        if (!System.IO.Directory.Exists(X_ReportFilePath))
                            Directory.CreateDirectory(X_ReportFilePath);
                        int index;
                        foreach (var files in Directory.GetFiles(@X_ReportFilePath, "*.rpt"))
                        {
                            SortedList element = new SortedList();
                            index = Path.GetFileName(files).IndexOf(".");
                            if (index > 0)
                            {
                                element.Add("templateName", Path.GetFileName(files).Substring(0, index).ToString());
                            }
                            templates.Add(element);
                        }

                        







                    }
                    return Ok(_api.Success(templates));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }

        }
    }
}









