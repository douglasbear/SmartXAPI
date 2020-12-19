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
// using System.IO;

// namespace SmartxAPI.Controllers
// {
//     [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//     [Route("genAttachments")]
//     [ApiController]
//     public class Gen_Attachments : ControllerBase
//     {
//         private readonly IApiFunctions api;
//         private readonly IDataAccessLayer dLayer;
//         private readonly IMyFunctions myFunctions;
//         private readonly string connectionString;
//         private readonly int FormID;
// private readonly string reportPath;
// private readonly string startupPath;
//         public Gen_Attachments(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
//         {
//             api = apifun;
//             dLayer = dl;
//             myFunctions = myFun;
//             connectionString = conf.GetConnectionString("SmartxConnection");
//             FormID = 0;
//             reportPath = conf.GetConnectionString("ReportPath");
//             startupPath= conf.GetConnectionString("StartupPath");
//         }


        
//         [HttpGet("AttachmentCategory")]
//         public ActionResult CategoryList(int nFormID)
//         {
//             DataTable dt = new DataTable();
//             SortedList Params = new SortedList();

//             int nCompanyID=myFunctions.GetCompanyID(User);
  
//             string sqlCommandText = "";
//             if (nFormID > 0)
//                 sqlCommandText = "Select distinct X_Category From Inv_AttachmentCategory where N_CompanyID=@nCompanyID and ( N_FormID=@nFormID or N_FormID is null )";
//             else
//                 sqlCommandText = "Select distinct X_Category From Inv_AttachmentCategory where N_CompanyID=@nCompanyID and N_FormID is null";
//             Params.Add("@nCompanyID", nCompanyID);
//             Params.Add("@nFormID", nFormID);

//             try
//             {
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     connection.Open();
//                     dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
//                 }
//                 dt = api.Format(dt);
//                 if (dt.Rows.Count == 0)
//                 {
//                     return Ok(api.Warning("No Results Found"));
//                 }
//                 else
//                 {
//                     return Ok(api.Success(dt));
//                 }
//             }
//             catch (Exception e)
//             {
//                 return Ok(api.Error(e));
//             }
//         }

//         [HttpGet("ReminderCategory")]
//         public ActionResult ReminderCategoryList()
//         {
//             DataTable dt = new DataTable();
//             SortedList Params = new SortedList();

//             int nCompanyID=myFunctions.GetCompanyID(User);
            
//             string sqlCommandText = "Select distinct X_Category From Dms_ReminderCategory where N_CompanyID=@nCompanyID";
//             Params.Add("@nCompanyID", nCompanyID);

//             try
//             {
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     connection.Open();
//                     dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
//                 }
//                 dt = api.Format(dt);
//                 if (dt.Rows.Count == 0)
//                 {
//                     return Ok(api.Warning("No Results Found"));
//                 }
//                 else
//                 {
//                     return Ok(api.Success(dt));
//                 }
//             }
//             catch (Exception e)
//             {
//                 return Ok(api.Error(e));
//             }
//         }


//         public void SaveAttachment(DataAccessLayer dLayer, DataTable dsAttachment, string payCode, int payId, string partyname, string partycode, int partyId, string X_folderName,SqlConnection connection,SqlTransaction transaction)
//         {
//             object Result = 0;
//             string G_path = "";
//             string path = "";
//             string s = "";
//             DataRow AttachmentRow = dsAttachment.Rows[0];
//             int FormID = myFunctions.getIntVAL(AttachmentRow["formID"].ToString());
//             int nCompanyID=myFunctions.GetCompanyID(User);
//             string xCompanyName=myFunctions.GetCompanyName(User);
//             int N_AttachmentID = 0;
//             string ExpiryDate = "";
//             int N_remCategory = 0;
//             object obj = dLayer.ExecuteScalar("Select X_Value  From Gen_Settings Where N_CompanyID=" + nCompanyID + " and X_Group='188' and X_Description='EmpDocumentLocation'",connection,transaction);
//             string DocumentPath = obj != null && obj != "" ? obj.ToString() : this.reportPath;
//             if (dsAttachment.Rows.Count > 0)
//             {
                 
//                 DocFolderInsert(dLayer, xCompanyName + "//" + X_folderName + "//" + payCode + "//", 1, 0, FormID,connection,transaction);
//                 if (DocumentPath != "")
//                 {
//                     if (!Directory.Exists(DocumentPath + myCompanyID._DocumtFolder))
//                         Directory.CreateDirectory(DocumentPath + myCompanyID._DocumtFolder);
//                     s = DocumentPath+ myCompanyID._DocumtFolder+"\\";
//                 }
//                 else
//                     s = this.startupPath + myCompanyID._DocumtFolder+"\\";
//                 if (s == "0" || !Directory.Exists(s))
//                 {
//                     throw new Exception("Invalid Path");
//                 }
//                 else
//                 {
//                     for (int i = 0; i <= dsAttachment.Rows.Count - 1; i++)
//                     {

//                         dLayer.SaveData(ref Result, "Inv_AttachmentCategory", "N_CategoryID", "0", "N_CompanyID,X_Category", myCompanyID._CompanyID + "|'" + dsAttachment.Rows[i]["X_Category"].ToString() + "'", "", "", "X_Category='" + dsAttachment.Rows[i]["X_Category"].ToString() + "'", "");
//                         Result = 0;

//                         path = s + "\\" + payCode;  //+ "\\" + txtVendorCode.Text.Trim();
//                         N_AttachmentID = myFunctions.getIntVAL(dba1.ExecuteSclarNoErrorCatch("Select  Isnull(max(N_AttachmentID),0)+1 from Dms_ScreenAttachments", "TEXT", new DataTable()).ToString());
//                         if (dsAttachment.Rows[i]["N_AttachmentID"].ToString() != "" && dsAttachment.Rows[i]["N_AttachmentID"].ToString() != "0")
//                         {
//                             // if (myFunctions.getIntVAL(MYG.ReturnFormID(frm.Text)) == 113)
//                             {
//                                 dba1.DeleteData("Acc_CompanyAttachments", "N_AttachmentID", myFunctions.getIntVAL(dsAttachment.Rows[i]["N_AttachmentID"].ToString()).ToString(), "");
//                             } 
//                             else
//                             {
//                                 dba1.DeleteData("Dms_ScreenAttachments", "N_AttachmentID", myFunctions.getIntVAL(dsAttachment.Rows[i]["N_AttachmentID"].ToString()).ToString(), "");
//                             }
//                                 N_AttachmentID = myFunctions.getIntVAL(dsAttachment.Rows[i]["N_AttachmentID"].ToString());
//                         }
//                         string FileType = "";
//                         if (dsAttachment.Rows[i]["X_Extension"].ToString() != "")
//                             FileType = "File";
//                         else
//                             FileType = "Folder";

//                         ExpiryDate = "";
//                         N_remCategory = 0;
//                         if (dsAttachment.Rows[i]["D_ExpiryDate"].ToString() != "")
//                             ExpiryDate = Convert.ToDateTime(dsAttachment.Rows[i]["D_ExpiryDate"].ToString()).ToString("dd/MMM/yyyy");
//                         if (dsAttachment.Rows[i]["X_RemCategory"].ToString() != "")
//                             N_remCategory = Convert.ToInt32(dba1.ExecuteSclarNoErrorCatch("Select N_CategoryID from Dms_ReminderCategory where X_Category='" + dsAttachment.Rows[i]["X_RemCategory"].ToString() + "' and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable()));

//                         if (FileType == "File")
//                         {
//                             if (dsAttachment.Rows[i]["X_Category"].ToString() != "")
//                             {
//                                 DocFolderInsert(dba1, xCompanyName + "//" + X_folderName + "//" + payCode + "//" + dsAttachment.Rows[i]["X_Category"].ToString() + "//", 0, N_AttachmentID, myFunctions.getIntVAL(MYG.ReturnFormID(frm.Text)));

//                             }

//                             if (dsAttachment.Rows[i]["X_Filename"].ToString() != s + "\\" + payCode + "\\" + dsAttachment.Rows[i]["X_File"].ToString())
//                             {
//                                 try
//                                 {
//                                     string fileCode = dba1.ExecuteSclarNoErrorCatch("SP_AutoNumberGenerate " + myCompanyID._CompanyID + "," + myCompanyID._FnYearID + ",641 ", "TEXT", new DataTable()).ToString();
//                                     string extension = System.IO.Path.GetExtension(dsAttachment.Rows[i]["X_File"].ToString());
//                                     CopyFiles(dba1, dsAttachment.Rows[i]["X_File"].ToString(), dsAttachment.Rows[i]["X_Subject"].ToString(), N_FolderID, true, dsAttachment.Rows[i]["X_Category"].ToString(), dsAttachment.Rows[i]["X_Filename"].ToString(), s, fileCode, N_AttachmentID, myFunctions.getIntVAL(MYG.ReturnFormID(frm.Text)), ExpiryDate, N_remCategory, payId, partyId, 0);
//                                     refname = s + fileCode + extension;
//                                     dsAttachment.Rows[i]["X_refName"] = refname;
//                                 }
//                                 catch (Exception ex)
//                                 {
//                                     msg.msgError(ex.ToString());

//                                 }
//                             }
//                             else
//                             {
//                                 object obj = dba1.ExecuteSclar("select N_ReminderID from DMS_MasterFiles Where X_refName = '" + Path.GetFileName(dsAttachment.Rows[i]["X_refName"].ToString()) +"' and N_CompanyID = " + myCompanyID._CompanyID, "TEXT", new DataTable());
//                                 if (obj.ToString() != "")
//                                 {
//                                     dba1.ExecuteNonQuery("update Gen_Reminder set D_ExpiryDate = '" + Convert.ToDateTime(ExpiryDate).ToString("dd/MMM/yyyy") + "' ,N_RemCategoryID=" + N_remCategory + " where N_ReminderID=" + myFunctions.getIntVAL(obj.ToString()) + " and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());
//                                 }
//                                 else
//                                 {
//                                     if (ExpiryDate != "")
//                                     {
//                                         int ReminderId = myReminders.ReminderSave(dba1, myFunctions.getIntVAL(MYG.ReturnFormID(frm.Text)), partyId, ExpiryDate, dsAttachment.Rows[i]["X_Subject"].ToString(), dsAttachment.Rows[i]["X_Filename"].ToString(), N_remCategory, 1, 0);
//                                         dba1.ExecuteNonQuery("update DMS_MasterFiles set N_ReminderID=" + ReminderId + " where X_refName='" + Path.GetFileName(dsAttachment.Rows[i]["X_refName"].ToString()) + "' and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());
//                                     }
//                                 }
//                             }
//                         }
//                         string FieldList="";
//                         string FieldValues = "";
//                         if (myFunctions.getIntVAL(MYG.ReturnFormID(frm.Text)) == 113)
//                         {
//                             if (ExpiryDate != "")
//                             {
//                                 FieldList = "N_CompanyID,N_FnyearID,N_FormID,X_Subject,X_FileName,X_File,X_extension,X_refName,D_ExpiryDate,N_RemCategoryID";
//                                 FieldValues = myCompanyID._CompanyID + "|" + myCompanyID._FnYearID + "|" + myFunctions.getIntVAL(MYG.ReturnFormID(frm.Text)) + "|'" + dsAttachment.Rows[i]["X_Subject"].ToString() + "'|'" + path + "\\" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_Extension"].ToString() + "'|'" + dsAttachment.Rows[i]["X_refName"].ToString() + "','" + ExpiryDate + "'," + N_remCategory;
//                             }
//                             else
//                             {
//                                 FieldList = "N_CompanyID,N_FnyearID,N_FormID,X_Subject,X_FileName,X_File,X_extension,X_refName";
//                                 FieldValues = myCompanyID._CompanyID + "|" + myCompanyID._FnYearID + "|" + myFunctions.getIntVAL(MYG.ReturnFormID(frm.Text)) + "|'" + dsAttachment.Rows[i]["X_Subject"].ToString() + "'|'" + path + "\\" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_Extension"].ToString() + "'|'" + dsAttachment.Rows[i]["X_refName"].ToString() + "'";
//                             }
//                             string refField = "N_CategoryID";
//                             string refValue = "Inv_AttachmentCategory|N_CategoryID|X_Category='" + dsAttachment.Rows[i]["X_Category"].ToString() + "'";

//                             string DupCriteria = "";
//                             dba1.SaveData(ref Result, "Acc_CompanyAttachments", "N_AttachmentID", N_AttachmentID.ToString(), FieldList, FieldValues, refField, refValue, DupCriteria, "");
//                             if (myFunctions.getIntVAL(Result.ToString()) > 0)
//                             {

//                             }

//                         }
//                         else
//                         {
//                             if (ExpiryDate != "")
//                             {
//                                 FieldList = "N_CompanyID,N_FnyearID,N_PartyID,N_FormID,N_TransID,X_Subject,X_FileName,X_extension,X_File,X_refName,D_ExpiryDate,N_RemCategoryID";
//                                 FieldValues = myCompanyID._CompanyID + "|" + myCompanyID._FnYearID + "|" + partyId + "|" + myFunctions.getIntVAL(MYG.ReturnFormID(frm.Text)) + "|" + payId + "|'" + dsAttachment.Rows[i]["X_Subject"].ToString() + "'|'" + path + "\\" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_Extension"].ToString() + "'|'" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_refName"].ToString() + "','" + ExpiryDate + "'," + N_remCategory;
//                             }
//                             else
//                             {
//                                 FieldList = "N_CompanyID,N_FnyearID,N_PartyID,N_FormID,N_TransID,X_Subject,X_FileName,X_extension,X_File,X_refName";
//                                 FieldValues = myCompanyID._CompanyID + "|" + myCompanyID._FnYearID + "|" + partyId + "|" + myFunctions.getIntVAL(MYG.ReturnFormID(frm.Text)) + "|" + payId + "|'" + dsAttachment.Rows[i]["X_Subject"].ToString() + "'|'" + path + "\\" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_Extension"].ToString() + "'|'" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_refName"].ToString() + "'";
//                             }
//                             string refField = "N_CategoryID";
//                             string refValue = "Inv_AttachmentCategory|N_CategoryID|X_Category='" + dsAttachment.Rows[i]["X_Category"].ToString() + "'";

//                             string DupCriteria = "";
//                             dba1.SaveData(ref Result, "Dms_ScreenAttachments", "N_AttachmentID", N_AttachmentID.ToString(), FieldList, FieldValues, refField, refValue, DupCriteria, "");
//                             if (myFunctions.getIntVAL(Result.ToString()) > 0)
//                             {

//                             }
//                         }
//                     }
//                 }

//             }
//         }



//         public void DocFolderInsert(DataAccessLayer dLayer, string Path, int type, int AttachID, int FormID,SqlConnection connection,SqlTransaction transaction)
//         {
//             int i = 0, k = 0, pid = 0;
//             string x_path = "";
//             string ParentDirectory = Path;
//             string[] subFolders = ParentDirectory.Replace("//", "/").TrimEnd().Split('/');
//             while (i < subFolders.Length - 1)
//             {

//                 object N_Result = dLayer.ExecuteScalar("Select 1 from DMS_MasterFolder Where X_Path ='" + x_path + "' and X_Name='" + subFolders[i] + "' and N_CompanyID= " + myCompanyID._CompanyID,connection,transaction);
//                 if (N_Result == null)
//                 {
//                     pid = InsertFolder(dLayer,subFolders[i], pid, x_path, type, AttachID, FormID);
//                     x_path = x_path + subFolders[i] + "//";
//                 }
//                 else
//                 {
//                     pid = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select N_FolderID from DMS_MasterFolder Where X_Path ='" + x_path + "'and X_Name='" + subFolders[i] + "' and N_CompanyID= " + myCompanyID._CompanyID,connection,transaction).ToString());
//                     x_path = x_path + subFolders[i] + "//";
//                 }
//                 ++i;
//             }

//             N_FolderID = myFunctions.getIntVAL(pid.ToString());

//         }


//         private static int InsertFolder(DataAccessLayer dLayer, string Name, int ParentID, string Path, int foldertype, int attID, int fId)
//         {
//             int N_GroupID = 0;
//             string FieldList = "N_CompanyID,X_FolderCode,N_ParentFolderID,X_Path,X_Name,N_UserID,B_Default,N_AttachmentID,N_FormID";
//             string FieldValues = myCompanyID._CompanyID + "|'" + Name + "'|" + ParentID + "|'" + Path + "'|'" + Name + "'|" + myCompanyID._UserID + "|" + foldertype + "|" + attID + "|" + fId;
//             try
//             {
//                 object Result = 0;
//                 string DupCriteria = "N_CompanyID=" + myCompanyID._CompanyID + " and  X_Name='" + Name + "' and X_Path='" + Path + "'";
//                 dLayer.SaveData("DMS_MasterFolder", "N_FolderID", N_GroupID.ToString(), FieldList, FieldValues, DupCriteria, "", "N_CompanyID=" + myCompanyID._CompanyID);
//                 if (myFunctions.getIntVAL(Result.ToString()) > 0)
//                 {
//                     N_GroupID = myFunctions.getIntVAL(Result.ToString());
//                     return N_GroupID;
//                 }
//                 else
//                 {
//                     dba1.Rollback();
//                     throw new Exception("DuplicateExist");
//                 }
//             }
//             catch (Exception ex)
//             {
//                 dba1.Rollback();
//                 msg.msgError(ex.Message);
//                 return 0;
//             }
//         }




       


//     }
// }