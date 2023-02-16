using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

using System.Text.RegularExpressions;

using System.Security;

using System.Net;
using System.Net.Mail;
using System.Windows;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("myReminder")]
    [ApiController]

    public class myReminder : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        private readonly string TempFilesPath;

        public myReminder(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            TempFilesPath = conf.GetConnectionString("TempFilesPath");
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1406;

        }


        [HttpGet("dashboardList")]
        public ActionResult myReminderList(int nPage,int nSizeperpage,string xSearchkey, string xSortBy,bool x_Expire,DateTime d_Date,int nLanguageID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText ="";
            string sqlCommandCount = "";
            string Criteria = "";
            string Searchkey = "";
            int Count= (nPage - 1) * nSizeperpage;
            int nCompanyID = myFunctions.GetCompanyID(User);
              Params.Add("@nCompanyID", nCompanyID);
              Params.Add("@nLanguageID", nLanguageID);
        //   if (x_Expire == true)
        //     {
        //         Criteria = "and D_ExpiryDate=@d_Date";
        //          Params.Add("@d_Date", d_Date);
        //     }
           
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and (X_Subject like '%" + xSearchkey + "%' or X_PartyName like '%" + xSearchkey + "%' or D_ExpiryDate like '%" + xSearchkey + "%' or D_ReminderDate like '%" + xSearchkey + "%'or X_FormName like '%" + xSearchkey + "%' or X_Category like '%" + xSearchkey + "%' )";


             if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by D_ReminderDate asc";
            else
            
             xSortBy = " order by " + xSortBy;
             if (x_Expire == true)
            {
                if (Count == 0)
                    sqlCommandText = "select * from vw_Gen_ReminderDashboardExpired where N_CompanyID=@nCompanyID and N_LanguageID=@nLanguageID and D_ExpiryDate<='"+d_Date+"'" +Searchkey+ xSortBy;
                else
                    sqlCommandText = "select * from vw_Gen_ReminderDashboardExpired where N_CompanyID=@nCompanyID and N_LanguageID=@nLanguageID and D_ExpiryDate<='"+d_Date+"'" +Searchkey+ xSortBy;
            }
            else{
                 if (Count == 0)
                 sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Gen_ReminderDashboard where N_CompanyID=@nCompanyID and N_LanguageID=@nLanguageID" + Searchkey + xSortBy ;
                  else
                    sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Gen_ReminderDashboard where N_CompanyID=@nCompanyID and N_LanguageID=@nLanguageID" + Searchkey + xSortBy;
            }

            SortedList OutPut = new SortedList();

           try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    sqlCommandCount = "select count(1) as N_Count from vw_Gen_ReminderDashboardExpired where N_CompanyID=@nCompanyID " +Searchkey;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }

                }
                
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


      private static Random random = new Random();
      public string RandomString(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpGet("getAttachments")]
        public ActionResult myAttachmentList(int nAttachmentId,int nTransID,int nPartyID,int nFormID,int nFnYearID)
        {

             try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable DataTable = new DataTable();
                
                    string DetailSql = "";

                    Params.Add("@nAttachmentId", nAttachmentId);
            Params.Add("@nTransID", nTransID);
            Params.Add("@nPartyID", nPartyID);
            Params.Add("@nFormID", nFormID);
            Params.Add("@nFnYearID", nFnYearID);
                  
                    // MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from Dms_ScreenAttachments where n_AttachmentId = @nAttachmentId and n_TransID = @nTransID and N_FormID=@nFormID and n_FnYearID= @nFnYearID ";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);

            DetailTable = myFunctions.AddNewColumnToDataTable(DetailTable, "FileData", typeof(string), null);
            DetailTable = myFunctions.AddNewColumnToDataTable(DetailTable, "TempFileName", typeof(string), null);

            if (DetailTable.Rows.Count > 0)
            {
                DetailTable = myFunctions.AddNewColumnToDataTable(DetailTable, "n_CompanyID", typeof(int), myFunctions.GetCompanyID(User));
                DetailTable = myFunctions.AddNewColumnToDataTable(DetailTable, "n_FnYearID", typeof(int), nFnYearID);
                DetailTable = myFunctions.AddNewColumnToDataTable(DetailTable, "n_TransID", typeof(int), nTransID);
            }

            foreach (DataRow var in DetailTable.Rows)
            {
                if (var["x_refName"] != null)
                {
                    var path = var["x_refName"].ToString();
                    if (System.IO.File.Exists(path))
                    {
                        Byte[] bytes = System.IO.File.ReadAllBytes(path);
                        var random = RandomString();
                        System.IO.File.Copy(path, this.TempFilesPath + random + "." + var["x_Extension"].ToString());
                        var["TempFileName"] = random + "." + var["x_Extension"].ToString();
                        var["FileData"] = "data:" + _api.GetContentType(path) + ";base64," + Convert.ToBase64String(bytes);
                    }
                }

            }


                    DetailTable = _api.Format(DetailTable, "Attachments");
                    dt.Tables.Add(DetailTable);
                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }



        //     DataTable dt = new DataTable();
        //     SortedList Params = new SortedList();
        //     string sqlCommandText ="";
        //     string sqlCommandCount = "";
           
        //     using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //     DataTable ImageData = dLayer.ExecuteDataTablePro("select * from Dms_ScreenAttachments where n_AttachmentId = @nAttachmentId and n_TransID = @nTransID and N_FormID=@nFormID and n_FnYearID= @nFnYearID", Params, connection);
        //     ImageData = myFunctions.AddNewColumnToDataTable(ImageData, "FileData", typeof(string), null);
        //     ImageData = myFunctions.AddNewColumnToDataTable(ImageData, "TempFileName", typeof(string), null);

        //     if (ImageData.Rows.Count > 0)
        //     {
        //         ImageData = myFunctions.AddNewColumnToDataTable(ImageData, "n_CompanyID", typeof(int), myFunctions.GetCompanyID(User));
        //         ImageData = myFunctions.AddNewColumnToDataTable(ImageData, "n_FnYearID", typeof(int), FnYearID);
        //         ImageData = myFunctions.AddNewColumnToDataTable(ImageData, "n_TransID", typeof(int), TransID);
        //     }

        //     foreach (DataRow var in ImageData.Rows)
        //     {
        //         if (var["x_refName"] != null)
        //         {
        //             var path = var["x_refName"].ToString();
        //             if (System.IO.File.Exists(path))
        //             {
        //                 Byte[] bytes = System.IO.File.ReadAllBytes(path);
        //                 var random = RandomString();
        //                 System.IO.File.Copy(path, this.TempFilesPath + random + "." + var["x_Extension"].ToString());
        //                 var["TempFileName"] = random + "." + var["x_Extension"].ToString();
        //                 var["FileData"] = "data:" + _api.GetContentType(path) + ";base64," + Convert.ToBase64String(bytes);
        //             }
        //         }

        //     }

        //     ImageData.AcceptChanges();

        //     return ImageData;
        //         }
            


        }
    }
}

  