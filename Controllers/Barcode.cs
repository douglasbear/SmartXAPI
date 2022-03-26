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
// using iTextSharp.text.pdf;
// using iTextSharp.text;


// namespace SmartxAPI.Controllers
// {
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
//             MasterTable = ds.Tables["master"];
//             int nCompanyID = myFunctions.GetCompanyID(User);
//             int nUserID = myFunctions.GetUserID(User);

//             try
//             {
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     connection.Open();
//                     SqlTransaction transaction;
//                     transaction = connection.BeginTransaction();

//                     return Ok(_api.Success(""));
//                 }
//             }
//             catch (Exception e)
//             {
//                 return Ok(_api.Error(User, e));
//             }
//         }
//         [HttpPost("Save")]
//         public ActionResult SaveData([FromBody] DataSet ds)
//         {
//             try
//             {
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     connection.Open();
//                     SqlTransaction transaction = connection.BeginTransaction();


//                     DataTable Master = ds.Tables["master"];
//                     DataTable Details = ds.Tables["details"];
//                     SortedList Params = new SortedList();
//                     DataRow MasterRow = Master.Rows[0];

//                     int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
//                     int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
//                     int N_BranchID = myFunctions.getIntVAL(MasterRow["n_BranchID"].ToString());
//                     string x_templatecode = MasterRow["x_templatecode"].ToString();

//                     Master.Columns.Remove("n_BranchID");

//                     if (x_templatecode == "@Auto")
//                     {
//                         Params.Add("N_CompanyID", N_CompanyID);
//                         Params.Add("N_YearID", N_FnYearID);
//                         Params.Add("N_FormID", 1063);
//                         Params.Add("N_BranchID", N_BranchID);
//                         x_templatecode = dLayer.GetAutoNumber("Inv_BarcodeTemplate", "x_templatecode", Params, connection, transaction);
//                         if (x_templatecode == "")
//                         {
//                             transaction.Rollback();
//                             return Ok("Unable to generate Templatecode Number");
//                         }
//                         Master.Rows[0]["x_templatecode"] = x_templatecode;
//                     }
//                     string DupCriteria = "";


//                     int N_TemplateID = dLayer.SaveData("Inv_BarcodeTemplate", "N_TemplateID", DupCriteria, "", Master, connection, transaction);
//                     if (N_TemplateID <= 0)
//                     {
//                         transaction.Rollback();
//                         return Ok("Unable to save");
//                     }
//                     for (int i = 0; i < Details.Rows.Count; i++)
//                     {
//                         Details.Rows[i]["N_TemplateID"] = N_TemplateID;

//                     }

//                     dLayer.SaveData("Inv_BarcodeTemplateDetails", "N_TemplateDetailsID", Details, connection, transaction);
//                     transaction.Commit();
//                     SortedList Result = new SortedList();

//                     return Ok(_api.Success(Result, "Template Saved"));
//                 }
//             }
//             catch (Exception ex)
//             {
//                 return Ok(_api.Error(User, ex));
//             }
//         }
//         public static string WithMaxLength(this string value, int maxLength)
//         {
//             if (string.IsNullOrEmpty(value)) return value;
//             return value.Length <= maxLength ? value : value.Substring(0, maxLength);
//         }
//         public void ExportToPDFforThermalPrinter(ref DataSet ds, string formId)
//         {
//             iTextSharp.text.Document pdfdoc = new iTextSharp.text.Document();

//             double DocTotalWidtht = 0;

//             double DocLabelHeight = 0;
//             double DocLabelWidth = 0;

//             double DocMarginLeft = 0;
//             double DocMarginRight = 0;
//             double DocMarginTop = 0;
//             double DocMarginBottom = 0;

//             int DocNumOfColumns = 0;

//             double DocBarcodeHeight = 0;
//             double DocBarcodeSize = 0;
//             double DocBarcodeBaseLine = 0;

//             double DocSpaceBetweenLabels = 0;

//             float DocCellPadding = 2;

//             float DocFieldSpace = 0;
//             float DocSpaceAtStart = 0;
//             float DocSpaceAtEnd = 0;
//             int DocFieldLength = 0;
//             using (SqlConnection connection = new SqlConnection(connectionString))
//             {
//                 SortedList Params = new SortedList();

//                 object obj_TemplateID = null;
//                 int N_TemplateID = 0;
//                 if (formId != "")
//                     obj_TemplateID = dLayer.ExecuteScalar("Select Isnull(N_Value,1) from Gen_Settings where N_CompanyID=" + myCompanyID._CompanyID + " and X_Group='" + formId + "' and X_Description='BarcodeTemplate'", Params, connection);
//                 if (obj_TemplateID != null)
//                     N_TemplateID = myFunctions.getIntVAL(obj_TemplateID.ToString());
//                 else
//                     N_TemplateID = 1;

//                 DataTable dsMaster = new DataTable();
//                 dsMaster = dLayer.ExecuteDataTable("SELECT * FROM vw_BarcodeTemplate where B_Show=1 and N_TemplateID=" + N_TemplateID + " and N_CompanyID=" + myCompanyID._CompanyID + " and N_FormID=" + formId + " order by N_order ASC", Params, connection);
//                 //GetBarcodeTemplate(formId, N_TemplateID.ToString());


//                 if (dsMaster.Rows.Count > 0)
//                 {

//                     DataRow row = dsMaster.Rows[0];

//                     DocTotalWidtht = myFunctions.getVAL(row["N_DocTotalWidth"].ToString());

//                     DocLabelHeight = myFunctions.getVAL(row["N_LabelHeight"].ToString());
//                     DocLabelWidth = myFunctions.getVAL(row["N_LabelWidth"].ToString());

//                     DocMarginLeft = myFunctions.getVAL(row["N_LabelMarginLeft"].ToString());
//                     DocMarginRight = myFunctions.getVAL(row["N_LabelMarginRight"].ToString());
//                     DocMarginTop = myFunctions.getVAL(row["N_LabelMarginTop"].ToString());
//                     DocMarginBottom = myFunctions.getVAL(row["N_LabelMarginBottom"].ToString());

//                     DocNumOfColumns = myFunctions.getIntVAL(row["N_ColumnsPerRow"].ToString());

//                     DocBarcodeHeight = myFunctions.getVAL(row["N_BarcodeHeight"].ToString());
//                     DocBarcodeSize = myFunctions.getVAL(row["N_BarcodeSize"].ToString());
//                     DocBarcodeBaseLine = myFunctions.getVAL(row["N_BarcodeBaseLine"].ToString());

//                     DocSpaceBetweenLabels = myFunctions.getVAL(row["N_SpaceBetweenLabels"].ToString());

//                     DocFieldSpace = myFunctions.getFloatVAL(row["N_FieldSpace"].ToString());
//                     DocSpaceAtStart = myFunctions.getFloatVAL(row["N_StartSpace"].ToString());
//                     DocSpaceAtEnd = myFunctions.getFloatVAL(row["N_EndSpace"].ToString());


//                 }
//                 else
//                 {
//                     DocTotalWidtht = 216;

//                     DocLabelHeight = 227;
//                     DocLabelWidth = 65;

//                     DocMarginLeft = 6;
//                     DocMarginRight = 6;
//                     DocMarginTop = 0;
//                     DocMarginBottom = 0;

//                     DocNumOfColumns = 2;

//                     DocBarcodeHeight = 16;
//                     DocBarcodeSize = 9;
//                     DocBarcodeBaseLine = 9;

//                     DocSpaceBetweenLabels = 8;

//                     DocFieldSpace = 4;
//                     DocSpaceAtStart = 4;
//                     DocSpaceAtEnd = 4;
//                 }
//                 try
//                 {

//                     if (System.IO.File.Exists("C:\\OlivoServer2020\\Barcode\\bcprint.pdf"))
//                         System.IO.File.Delete("C:\\OlivoServer2020\\Barcode\\bcprint.pdf");


//                     int N_NoOfLabels = DocNumOfColumns;
//                     if (N_NoOfLabels == 0)
//                         N_NoOfLabels = 1;
//                     iTextSharp.text.Rectangle pgSize = new iTextSharp.text.Rectangle(142, 70);
//                     float[] fltParentWidth = new float[] { 108f };
//                     if (N_NoOfLabels == 1)
//                     {
//                         fltParentWidth = new float[] { 108f };
//                         pgSize = new iTextSharp.text.Rectangle(142, 70);
//                     }
//                     else if (N_NoOfLabels == 2)
//                     {
//                         fltParentWidth = new float[] { 108f, 108f };
//                         pgSize = new iTextSharp.text.Rectangle(235, 74);
//                     }
//                     PdfWriter writer = null;
//                     pdfdoc = new Document(pgSize, myFunctions.getFloatVAL(DocMarginLeft.ToString()), myFunctions.getFloatVAL(DocMarginRight.ToString()), myFunctions.getFloatVAL(DocMarginTop.ToString()), myFunctions.getFloatVAL(DocMarginBottom.ToString()));

//                     writer = PdfWriter.GetInstance(pdfdoc, new FileStream("C:\\OlivoServer2020\\Barcode\\bcprint.pdf", FileMode.Create));

//                     PdfPTable tbl = new PdfPTable(N_NoOfLabels);
//                     tbl.TotalWidth = myFunctions.getFloatVAL(DocTotalWidtht.ToString());
//                     tbl.LockedWidth = true;
//                     tbl.SetWidths(fltParentWidth);
//                     tbl.DefaultCell.FixedHeight = 57;
//                     tbl.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
//                     tbl.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
//                     tbl.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
//                     pdfdoc.Open();
//                     int intotalCount = 0;
//                     int inCopies = 0;

//                     for (int i = 0; i <= ds.Tables["DataSource"].Rows.Count - 1; i++)
//                     {
//                         inCopies = myFunctions.getIntVAL(ds.Tables["DataSource"].Rows[i]["N_PrintQty"].ToString());
//                         for (int j = 0; j < inCopies; j++)
//                         {
//                             Phrase phrase = new Phrase();
//                             PdfPCell cell = new PdfPCell(phrase);


//                             if (dsMaster.Rows.Count > 0)
//                             {
//                                 foreach (DataRow row in dsMaster.Rows)
//                                 {
//                                     DocFieldSpace = myFunctions.getFloatVAL(row["N_FieldSpace"].ToString());
//                                     DocFieldLength = myFunctions.getIntVAL(row["N_FieldLength"].ToString());
//                                     String Field = ds.Tables["DataSource"].Rows[i][row["X_FieldName"].ToString()].ToString();
//                                     iTextSharp.text.Font FieldFont = new iTextSharp.text.Font(FontFactory.GetFont(row["X_Font"].ToString(), myFunctions.getFloatVAL(row["N_FontSize"].ToString()), myFunctions.getIntVAL(row["N_FontStyle"].ToString())));

//                                     if (DocFieldLength > 0)
//                                         Field = WithMaxLength(Field, DocFieldLength);
//                                     if (row["X_FieldName"].ToString() == "N_SPrice" || row["X_FieldName"].ToString() == "N_Rate" || row["X_FieldName"].ToString() == "N_Price")
//                                         Field = myCompanyID._CurrencyName + " : " + myFunctions.getVAL(Field.ToString()).ToString(myCompanyID.DecimalPlaceString);

//                                     if (row["X_FieldName"].ToString() == "X_Barcode")
//                                     {
//                                         PdfContentByte pdfcb = writer.DirectContent;
//                                         Barcode128 code128 = new Barcode128();
//                                         code128.Code = Field;
//                                         code128.Extended = false;
//                                         code128.CodeType = iTextSharp.text.pdf.Barcode.CODE128;
//                                         code128.AltText = Field;
//                                         code128.BarHeight = myFunctions.getFloatVAL(DocBarcodeHeight.ToString());
//                                         code128.Size = myFunctions.getFloatVAL(DocBarcodeSize.ToString());
//                                         code128.Baseline = myFunctions.getFloatVAL(DocBarcodeBaseLine.ToString());
//                                         code128.TextAlignment = Element.ALIGN_CENTER;
//                                         iTextSharp.text.Image image128 = code128.CreateImageWithBarcode(pdfcb, null, null);
//                                         Field = image128.ToString();

//                                         cell.HorizontalAlignment = Element.ALIGN_CENTER;
//                                         cell.VerticalAlignment = Element.ALIGN_MIDDLE;
//                                         cell.Border = iTextSharp.text.Rectangle.NO_BORDER;

//                                         cell.PaddingLeft = DocCellPadding;
//                                         cell.PaddingRight = DocCellPadding;
//                                         cell.PaddingTop = DocCellPadding;
//                                         cell.PaddingBottom = DocCellPadding;

//                                         if (tbl.NumberOfColumns > 1)
//                                         {
//                                             if (j % tbl.NumberOfColumns == 0)
//                                                 phrase.Add(new Chunk(image128, 0, 0));
//                                             else
//                                                 phrase.Add(new Chunk(image128, myFunctions.getFloatVAL(DocSpaceBetweenLabels.ToString()), 0));
//                                         }
//                                         else
//                                             phrase.Add(new Chunk(image128, 0, 0));
//                                         if (DocFieldSpace > 0)
//                                             phrase.Add(new Chunk(Environment.NewLine + Environment.NewLine, new iTextSharp.text.Font(-1, DocFieldSpace)));
//                                     }
//                                     else
//                                     {
//                                         phrase.Add(new Chunk(Field, FieldFont));
//                                         if (DocFieldSpace > 0)
//                                             phrase.Add(new Chunk(Environment.NewLine + Environment.NewLine, new iTextSharp.text.Font(-1, DocFieldSpace)));
//                                     }
//                                 }
//                             }


//                             tbl.AddCell(cell);
//                             intotalCount++;
//                         }
//                     }
//                     //create a blank label for multi label sheet and print count is odd number. 
//                     if (N_NoOfLabels > 1)
//                     {
//                         int reminder = intotalCount % 2;
//                         if (reminder != 0)
//                         {
//                             for (int i = reminder; i < 2; ++i)
//                             {
//                                 tbl.AddCell("");
//                             }
//                         }
//                     }
//                     if (tbl.Rows.Count != 0)
//                     {
//                         pdfdoc.Add(tbl);
//                         pdfdoc.Close();
//                         System.Diagnostics.Process.Start("C:\\OLIVOSERVER2020\\Barcode\\bcprint.pdf");

//                     }
//                 }
//                 catch (Exception ex)
//                 {
//                 }
//             }



//         }
//     }
// }