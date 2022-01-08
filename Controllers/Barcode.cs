// using AutoMapper;
// using SmartxAPI.Data;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using System;
// using SmartxAPI.GeneralFunctions;
// using System.Data;
// using System.Collections;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Data.SqlClient;
// using System.Collections.Generic;
// using System.Net.Http;
// using System.IO;
// using System.Net;
// using System.Threading.Tasks;
// using System.Linq;
// using System.Net.Mail;
// using System.Text;
// using zatca.einvoicing;

// namespace SmartxAPI.Controllers
// {
//     //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//     [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//     [Route("barcode")]
//     [ApiController]
//     public class Barcode : ControllerBase
//     {
//         private readonly IApiFunctions _api;
//         private readonly IDataAccessLayer dLayer;
//         private readonly IMyFunctions myFunctions;
//         private readonly string connectionString;
//         private readonly string reportApi;
//         private readonly string TempFilesPath;
//         private readonly string reportLocation;
//         string RPTLocation = "";
//         string ReportName = "";
//         string critiria = "";
//         string TableName = "";
//         string QRurl = "";

//         public Barcode(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
//         {
//             _api = api;
//             dLayer = dl;
//             myFunctions = myFun;
//             connectionString = conf.GetConnectionString("SmartxConnection");
//             reportApi = conf.GetConnectionString("ReportAPI");
//             TempFilesPath = conf.GetConnectionString("TempFilesPath");
//             reportLocation = conf.GetConnectionString("ReportLocation");
//         }

//         [HttpPost("printbarcode")]
//         public IActionResult PrintBarCode([FromBody] DataSet ds)
//         {
//             DataTable MasterTable;
//             DataTable DetailTable;

//             MasterTable = ds.Tables["master"];
//             int nCompanyID = myFunctions.GetCompanyID(User);
//             string x_comments = "";
//             string x_Reporttitle = "";
//             int nUserID = myFunctions.GetUserID(User);

//             try
//             {
//                 String Criteria = "";
//                 String reportName = "";
//                 var handler = new HttpClientHandler
//                 {
//                     ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
//                 };

//                 var dbName = "";
//                 string Extention = "pdf";
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     connection.Open();
//                     SqlTransaction transaction;
//                     transaction = connection.BeginTransaction();
//                     var client = new HttpClient(handler);

//                     string Barcode = MasterTable.Rows[0]["x_barcode"].ToString();
//                     string ItemCode = MasterTable.Rows[0]["X_Itemcode"].ToString();
//                     string ItemName = MasterTable.Rows[0]["X_ItemName"].ToString();


//                     var url = string.Format("http://chart.apis.google.com/chart?cht=qr&chs={1}x{2}&chl={0}", plainTextBytes.Replace("&", "%26"), "500", "500");
//                     WebResponse response = default(WebResponse);
//                     Stream remoteStream = default(Stream);
//                     StreamReader readStream = default(StreamReader);
//                     WebRequest request = WebRequest.Create(url);
//                     response = request.GetResponse();
//                     remoteStream = response.GetResponseStream();
//                     readStream = new StreamReader(remoteStream);
//                     string path = "C://OLIVOSERVER2020/QR/";
//                     DirectoryInfo info = new DirectoryInfo(path);
//                     if (!info.Exists)
//                     {
//                         info.Create();
//                     }
//                     string pathfile = Path.Combine(path, "QR.png");
//                     using (FileStream outputFileStream = new FileStream(pathfile, FileMode.Create))
//                     {
//                         remoteStream.CopyTo(outputFileStream);
//                     }


//                     string URL = reportApi + "api/report?reportName=" + reportName + "&critiria=" + Criteria + "&path=" + this.TempFilesPath + "&reportLocation=" + actReportLocation + "&dbval=" + dbName + "&random=" + random + "&x_comments=" + x_comments + "&x_Reporttitle=" + x_Reporttitle + "&extention=" + Extention;
//                     var path = client.GetAsync(URL);

//                     path.Wait();
//                     return Ok(_api.Success(new SortedList() { { "FileName", reportName.Trim() + "." + Extention } }));

//                 }
//             }
//             catch (Exception e)
//             {
//                 return Ok(_api.Error(User, e));
//             }
//         }


//     }
// }