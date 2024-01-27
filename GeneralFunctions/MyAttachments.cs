using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Security;
using System.Security.Cryptography;
using System.Net;
using System.Net.Mail;
using System.Windows;


namespace SmartxAPI.GeneralFunctions
{
    public class MyAttachments : IMyAttachments
    {
        private readonly IMyFunctions myFunctions;
        private readonly IMyReminders myReminder;
        private readonly string TempFilesPath;
        private readonly IApiFunctions api;
        private readonly string DocumentsFolder;
        private readonly IApiFunctions _api;

        public MyAttachments(IApiFunctions apifun, IMyFunctions myFun, IConfiguration conf, IMyReminders rem)
        {
            api = apifun;
            _api = api;
            myFunctions = myFun;
            TempFilesPath = conf.GetConnectionString("TempFilesPath");
            DocumentsFolder = conf.GetConnectionString("DocumentsFolder");
            myReminder = rem;
        }

        public void SaveAttachment(IDataAccessLayer dLayer, DataTable dsAttachment, string payCode, int payId, string partyname, string partycode, int partyId, string X_DMSMainFolder, ClaimsPrincipal User, SqlConnection connection, SqlTransaction transaction)
        {
            object Result = 0;
            string path = "";

            int nCompanyID = myFunctions.GetCompanyID(User);
            string xCompanyName = myFunctions.GetCompanyName(User);
            int N_AttachmentID = 0;
            string ExpiryDate = null;
            int N_remCategory = 0;
            int N_FolderID = 0;

            if (dsAttachment.Rows.Count > 0)
            {
                if (dsAttachment.Columns.Contains("deleted"))
                {
                    for (int x = dsAttachment.Rows.Count - 1; x >= 0; x--)
                    {
                        DataRow dr = dsAttachment.Rows[x];
                        if (myFunctions.getBoolVAL(dr["deleted"].ToString()) == true)
                        {
                            dLayer.DeleteData("Dms_ScreenAttachments", "N_AttachmentID", myFunctions.getIntVAL(dr["N_AttachmentID"].ToString()), "", connection, transaction);
                            dr.Delete();
                        }
                    }
                    if (dsAttachment.Columns.Contains("deleted"))
                        dsAttachment.Columns.Remove("deleted");
                    dsAttachment.AcceptChanges();

                }
                if (dsAttachment.Rows.Count == 0)
                {
                    return;
                }
                DataRow AttachmentRow = dsAttachment.Rows[0];
                int FormID = myFunctions.getIntVAL(AttachmentRow["n_FormID"].ToString());
                int FnYearID = myFunctions.getIntVAL(AttachmentRow["n_FnYearID"].ToString());

                string X_DMSSubFolder = FormID + "//" + partycode + "-" + partyname;
                string X_folderName = X_DMSMainFolder + "//" + X_DMSSubFolder;

                N_FolderID = DocFolderInsert(dLayer, xCompanyName + "//" + X_folderName + "//" + payCode + "//", 1, 0, FormID, User, connection, transaction);

                if (!Directory.Exists(DocumentsFolder))
                {
                    throw new Exception("Invalid Path");
                }
                else
                {

                    for (int i = 0; i <= dsAttachment.Rows.Count - 1; i++)
                    {


                        dsAttachment.Rows[i]["N_TransID"] = payId;
                        dsAttachment.Rows[i]["N_PartyID"] = partyId;
                        // dLayer.SaveData(ref Result, "Inv_AttachmentCategory", "N_CategoryID", "0", "N_CompanyID,X_Category", nCompanyID + "|'" + dsAttachment.Rows[i]["X_Category"].ToString() + "'", "", "", "X_Category='" + dsAttachment.Rows[i]["X_Category"].ToString() + "'", "");
                        // Result = 0;

                        path = DocumentsFolder + "\\" + payCode;  //+ "\\" + txtVendorCode.Text.Trim();
                        if(N_AttachmentID<=0)
                        {
                        N_AttachmentID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select  Isnull(max(N_AttachmentID),0)+1 from Dms_ScreenAttachments", connection, transaction).ToString());
                       
                        }
                        else                        
                        N_AttachmentID=N_AttachmentID + 1;

                        if (dsAttachment.Rows[i]["N_AttachmentID"].ToString() != "" && dsAttachment.Rows[i]["N_AttachmentID"].ToString() != "0")
                        {
                            if (FormID == 113)
                            {
                                dLayer.DeleteData("Acc_CompanyAttachments", "N_AttachmentID", myFunctions.getIntVAL(dsAttachment.Rows[i]["N_AttachmentID"].ToString()), "", connection, transaction);
                            }
                            else
                            {
                                if (dsAttachment.Rows[i]["X_Filename"].ToString() != dsAttachment.Rows[i]["X_File"].ToString())
                                {
                                    dLayer.ExecuteScalar("delete from Gen_Reminder where N_ReminderId in (Select N_ReminderId From DMS_MasterFiles Where N_AttachmentID = "+myFunctions.getIntVAL(dsAttachment.Rows[i]["N_AttachmentID"].ToString())+" and N_CompanyID = " + nCompanyID + ")  and  N_CompanyID=" + nCompanyID, connection, transaction);
                                    dLayer.DeleteData("DMS_MasterFiles", "N_AttachmentID", myFunctions.getIntVAL(dsAttachment.Rows[i]["N_AttachmentID"].ToString()), "", connection, transaction);
                                }
                                dLayer.DeleteData("Dms_ScreenAttachments", "N_AttachmentID", myFunctions.getIntVAL(dsAttachment.Rows[i]["N_AttachmentID"].ToString()), "", connection, transaction);
                            }
                            N_AttachmentID = myFunctions.getIntVAL(dsAttachment.Rows[i]["N_AttachmentID"].ToString());
                        }


                        string FileType = "";
                        if (dsAttachment.Rows[i]["X_Extension"].ToString() != "")
                            FileType = "File";
                        else
                            FileType = "Folder";

                        ExpiryDate = null;
                        N_remCategory = 0;
                         if(dsAttachment.Columns.Contains("D_ExpiryDate"))
                        {
                        if (dsAttachment.Rows[i]["D_ExpiryDate"].ToString() != "")
                            ExpiryDate = Convert.ToDateTime(dsAttachment.Rows[i]["D_ExpiryDate"].ToString()).ToString("dd/MMM/yyyy");
                        if (dsAttachment.Rows[i]["X_RemCategory"].ToString() != "")
                            N_remCategory = Convert.ToInt32(dLayer.ExecuteScalar("Select N_CategoryID from Dms_ReminderCategory where X_Category='" + dsAttachment.Rows[i]["X_RemCategory"].ToString() + "' and N_CompanyID=" + nCompanyID, connection, transaction));
                        }
                        if (FileType == "File")
                        {
                            if (dsAttachment.Rows[i]["X_Category"].ToString() != "")
                            {
                                N_FolderID = DocFolderInsert(dLayer, xCompanyName + "//" + X_folderName + "//" + payCode + "//" + dsAttachment.Rows[i]["X_Category"].ToString() + "//", 0, N_AttachmentID, FormID, User, connection, transaction);

                            }

                            if (dsAttachment.Rows[i]["X_Filename"].ToString() != dsAttachment.Rows[i]["X_File"].ToString())
                            {
                                try
                                {

                                    SortedList AutoParam = new SortedList(){
                                    {"N_CompanyID", nCompanyID},
                                    {"N_YearID", FnYearID},
                                    {"N_FormID", 641}
                                    };
                                    string fileCode = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate ", AutoParam, connection, transaction).ToString();
                                    string extension = System.IO.Path.GetExtension(dsAttachment.Rows[i]["X_File"].ToString());
                                    CopyFiles(dLayer, dsAttachment.Rows[i]["X_File"].ToString(), dsAttachment.Rows[i]["X_Subject"].ToString(), N_FolderID, true, dsAttachment.Rows[i]["X_Category"].ToString(), dsAttachment.Rows[i]["FileData"].ToString(), DocumentsFolder, fileCode, N_AttachmentID, FormID, ExpiryDate, N_remCategory, payId, partyId, 0, User, transaction, connection);
                                    dsAttachment.Rows[i]["X_refName"] = DocumentsFolder + fileCode + extension;
                                    dsAttachment.Rows[i]["X_Filename"] = dsAttachment.Rows[i]["X_File"].ToString();

                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Error While Copying");

                                }
                            }
                            // else
                            // {
                            //     object obj1 = dLayer.ExecuteScalar("select isnull(N_ReminderID,0) from DMS_MasterFiles Where N_AttachmentID = '" + N_AttachmentID + "' and N_CompanyID = " + nCompanyID, connection, transaction);
                            //     if (obj1.ToString() != "" && ExpiryDate != "" && obj1 !=null)
                            //     {
                            //         dLayer.ExecuteNonQuery("update Gen_Reminder set D_ExpiryDate = '" + Convert.ToDateTime(ExpiryDate).ToString("dd/MMM/yyyy") + "' ,N_RemCategoryID=" + N_remCategory + " where N_ReminderID=" + myFunctions.getIntVAL(obj1.ToString()) + " and N_CompanyID=" + nCompanyID, connection, transaction);
                            //         //dLayer.ExecuteNonQuery("update DMS_MasterFiles set D_ExpiryDate = '" + Convert.ToDateTime(ExpiryDate).ToString("dd/MMM/yyyy") + "' ,N_CategoryID=" + N_remCategory + " where N_ReminderID=" + myFunctions.getIntVAL(obj1.ToString()) + " and N_CompanyID=" + nCompanyID, connection, transaction);
                         
                            //     }
                            //     else
                            //     {
                            //         if (ExpiryDate != "")
                            //         {
                            //         //   int ReminderId = myReminders.ReminderSave(dba1, FormID, partyId, ExpiryDate, dsAttachment.Rows[i]["X_Subject"].ToString(), dsAttachment.Rows[i]["X_Filename"].ToString(), N_remCategory, 1, 0);
                            //         //   dLayer.ExecuteNonQuery("update DMS_MasterFiles set N_ReminderID=" + ReminderId + " where X_refName='" + Path.GetFileName(dsAttachment.Rows[i]["X_refName"].ToString()) + "' and N_CompanyID=" + nCompanyID, "TEXT", new DataTable());
                            //         }
                            //     }
                            // }
                        }

                        string FieldList = "";
                        string FieldValues = "";
                        // if (FormID == 113)
                        // {
                        //     if (ExpiryDate != "")
                        //     {
                        //         FieldList = "N_CompanyID,N_FnyearID,N_FormID,X_Subject,X_FileName,X_File,X_extension,X_refName,D_ExpiryDate,N_RemCategoryID";
                        //         FieldValues = nCompanyID + "|" + myCompanyID._FnYearID + "|" + FormID + "|'" + dsAttachment.Rows[i]["X_Subject"].ToString() + "'|'" + path + "\\" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_Extension"].ToString() + "'|'" + dsAttachment.Rows[i]["X_refName"].ToString() + "','" + ExpiryDate + "'," + N_remCategory;
                        //     }
                        //     else
                        //     {
                        //         FieldList = "N_CompanyID,N_FnyearID,N_FormID,X_Subject,X_FileName,X_File,X_extension,X_refName";
                        //         FieldValues = nCompanyID + "|" + myCompanyID._FnYearID + "|" + FormID + "|'" + dsAttachment.Rows[i]["X_Subject"].ToString() + "'|'" + path + "\\" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_Extension"].ToString() + "'|'" + dsAttachment.Rows[i]["X_refName"].ToString() + "'";
                        //     }
                        //     string refField = "N_CategoryID";
                        //     string refValue = "Inv_AttachmentCategory|N_CategoryID|X_Category='" + dsAttachment.Rows[i]["X_Category"].ToString() + "'";

                        //     string DupCriteria = "";
                        //     // dLayer.SaveData(ref Result, "Acc_CompanyAttachments", "N_AttachmentID", N_AttachmentID.ToString(), FieldList, FieldValues, refField, refValue, DupCriteria, "");
                        //     dLayer.SaveData("Acc_CompanyAttachments", "N_AttachmentID", dsAttachment, connection, transaction);

                        //     if (myFunctions.getIntVAL(Result.ToString()) > 0)
                        //     {

                        //     }

                        // }
                    }

                    // if (FormID != 113)
                    // {
                        // if (ExpiryDate == "")
                        // {
                        //     if (dsAttachment.Columns.Contains("D_ExpiryDate"))
                        //         dsAttachment.Columns.Remove("D_ExpiryDate");
                        //     if (dsAttachment.Columns.Contains("N_RemCategoryID"))
                        //         dsAttachment.Columns.Remove("N_RemCategoryID");
                        // }
                        if (dsAttachment.Columns.Contains("FileData"))
                            dsAttachment.Columns.Remove("FileData");
                        if (dsAttachment.Columns.Contains("x_RemCategory"))
                            dsAttachment.Columns.Remove("x_RemCategory");
                        if (dsAttachment.Columns.Contains("x_Category"))
                            dsAttachment.Columns.Remove("x_Category");
                        dsAttachment.AcceptChanges();
                        object result = dLayer.ExecuteScalar("SELECT SUM(isnull(n_filesize,0)) AS total_filesize FROM Dms_ScreenAttachments WHERE N_CompanyID = "+nCompanyID+"",  connection, transaction);
                        double totalSize = Convert.ToDouble(result);
                        for (int i = 0; i <= dsAttachment.Rows.Count - 1; i++)
                    {
                        totalSize=totalSize+myFunctions.getVAL(dsAttachment.Rows[i]["n_FileSize"].ToString());
                    }

                        if (totalSize > 1024){
                        {
                            throw new Exception(("The allowed 1 GB quota has been exceeded. Please purchase additional storage to continue enjoying our services. For assistance, please contact our support team."));
                            // return Ok(_api.Error(User, "The allowed 1 GB quota has been exceeded. Please purchase additional storage to continue enjoying our services. For assistance, please contact our support team."));
                        }
                        }
                        dLayer.SaveData("Dms_ScreenAttachments", "N_AttachmentID", dsAttachment, connection, transaction);
                        if (myFunctions.getIntVAL(Result.ToString()) > 0)
                        {

                        }
                    // }
                    // else{
                    //         if (ExpiryDate == "")
                    //     {
                    //         if (dsAttachment.Columns.Contains("D_ExpiryDate"))
                    //             dsAttachment.Columns.Remove("D_ExpiryDate");
                    //         if (dsAttachment.Columns.Contains("N_RemCategoryID"))
                    //             dsAttachment.Columns.Remove("N_RemCategoryID");
                    //     }
                    //     if (dsAttachment.Columns.Contains("FileData"))
                    //         dsAttachment.Columns.Remove("FileData");
                    //     if (dsAttachment.Columns.Contains("x_RemCategory"))
                    //         dsAttachment.Columns.Remove("x_RemCategory");
                    //     if (dsAttachment.Columns.Contains("x_Category"))
                    //         dsAttachment.Columns.Remove("x_Category");
                    //          if (dsAttachment.Columns.Contains("n_PartyID"))
                    //         dsAttachment.Columns.Remove("n_PartyID");
                    //         if (dsAttachment.Columns.Contains("n_TransID"))
                    //         dsAttachment.Columns.Remove("n_TransID");
                    //        dsAttachment.AcceptChanges();
                    //        dLayer.SaveData("Acc_CompanyAttachments", "N_AttachmentID", dsAttachment, connection, transaction);
                    //         if (myFunctions.getIntVAL(Result.ToString()) > 0)
                    //         {

                    //         }

                        
                    // }

                }

            }
        }



        private int DocFolderInsert(IDataAccessLayer dLayer, string Path, int type, int AttachID, int FormID, ClaimsPrincipal User, SqlConnection connection, SqlTransaction transaction)
        {
            int i = 0, k = 0, pid = 0;
            string x_path = "";
            string ParentDirectory = Path;
            string[] subFolders = ParentDirectory.Replace("//", "/").TrimEnd().Split('/');
            int nCompanyID = myFunctions.GetCompanyID(User);
            while (i < subFolders.Length - 1)
            {

                object N_Result = dLayer.ExecuteScalar("Select 1 from DMS_MasterFolder Where X_Path ='" + x_path + "' and X_Name='" + subFolders[i] + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                if (N_Result == null)
                {
                    pid = InsertFolder(dLayer, subFolders[i], pid, x_path, type, AttachID, FormID, User, connection, transaction);
                    x_path = x_path + subFolders[i] + "//";
                }
                else
                {
                    pid = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select N_FolderID from DMS_MasterFolder Where X_Path ='" + x_path + "'and X_Name='" + subFolders[i] + "' and N_CompanyID= " + nCompanyID, connection, transaction).ToString());
                    x_path = x_path + subFolders[i] + "//";
                }
                ++i;
            }

            return myFunctions.getIntVAL(pid.ToString());

        }


        private int InsertFolder(IDataAccessLayer dLayer, string Name, int ParentID, string Path, int foldertype, int attID, int fId, ClaimsPrincipal User, SqlConnection connection, SqlTransaction transaction)
        {
            int N_GroupID = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable DMS_MasterFolder = new DataTable();
            DMS_MasterFolder = myFunctions.AddNewColumnToDataTable(DMS_MasterFolder, "N_CompanyID", typeof(int), nCompanyID);
            DMS_MasterFolder = myFunctions.AddNewColumnToDataTable(DMS_MasterFolder, "X_FolderCode", typeof(string), Name);
            DMS_MasterFolder = myFunctions.AddNewColumnToDataTable(DMS_MasterFolder, "N_ParentFolderID", typeof(int), ParentID);
            DMS_MasterFolder = myFunctions.AddNewColumnToDataTable(DMS_MasterFolder, "X_Path", typeof(string), Path);
            DMS_MasterFolder = myFunctions.AddNewColumnToDataTable(DMS_MasterFolder, "X_Name", typeof(string), Name);
            DMS_MasterFolder = myFunctions.AddNewColumnToDataTable(DMS_MasterFolder, "N_UserID", typeof(int), nUserID);
            DMS_MasterFolder = myFunctions.AddNewColumnToDataTable(DMS_MasterFolder, "B_Default", typeof(bool), foldertype);
            DMS_MasterFolder = myFunctions.AddNewColumnToDataTable(DMS_MasterFolder, "N_AttachmentID", typeof(int), attID);
            DMS_MasterFolder = myFunctions.AddNewColumnToDataTable(DMS_MasterFolder, "N_FormID", typeof(int), fId);
            DMS_MasterFolder = myFunctions.AddNewColumnToDataTable(DMS_MasterFolder, "N_FolderID", typeof(int), N_GroupID);
            DMS_MasterFolder.Rows.Add();
            DMS_MasterFolder.AcceptChanges();
            try
            {
                object Result = 0;
                string DupCriteria = "N_CompanyID=" + nCompanyID + " and  X_Name='" + Name + "' and X_Path='" + Path + "'";
                Result = dLayer.SaveData("DMS_MasterFolder", "N_FolderID", DMS_MasterFolder, connection, transaction);
                if (myFunctions.getIntVAL(Result.ToString()) > 0)
                {
                    N_GroupID = myFunctions.getIntVAL(Result.ToString());
                    return N_GroupID;
                }
                else
                {
                    transaction.Rollback();
                    throw new Exception("DuplicateExist");
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return 0;
            }
        }

        public void CopyFiles(IDataAccessLayer dLayer, string filename, string subject, int folderId, bool overwriteexisting, string category, string fileData, string destpath, string filecode, int attachID, int FormID, string strExpireDate, int remCategoryId, int transId, int partyID, int settingsId, ClaimsPrincipal User, SqlTransaction transaction, SqlConnection connection)
        {
            try
            {
                DateTime dtExpire = new DateTime();
                if (strExpireDate != null)
                    dtExpire = Convert.ToDateTime(strExpireDate);

                int nUserID = myFunctions.GetUserID(User);
                int nCompanyID = myFunctions.GetCompanyID(User);
                object N_Result = dLayer.ExecuteScalar("Select 1 from DMS_MasterFiles Where X_FileCode ='" + filecode + "' and N_CompanyID= " + nCompanyID + " and N_FormID=" + FormID, connection, transaction);
                if (N_Result != null)
                    return;
                int FileID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select ISNULL(max(N_FileID),0)+1 from DMS_MasterFiles where N_CompanyID=" + nCompanyID, connection, transaction).ToString());

                //  FileInfo flinfo = new FileInfo(fls);
                string extension = System.IO.Path.GetExtension(filename);
                string refname = filecode + extension;
                int ReminderId=0,nreminderID = 0 ;
                if (strExpireDate != null)
                { 
                     dLayer.ExecuteNonQuery("insert into DMS_MasterFiles(N_CompanyID,N_FileID,X_FileCode,X_Name,X_Title,X_Contents,N_FolderID,N_UserID,X_refName,N_AttachmentID,N_FormID,D_ExpiryDate,N_CategoryID,N_TransID)values(" + nCompanyID + "," + FileID + ",'" + filecode + "','" + filename + "','" + category + "','" + subject + "'," + folderId + "," + nUserID + ",'" + refname + "'," + attachID + "," + FormID + ",'" + dtExpire.ToString("dd/MMM/yyyy") + "'," + remCategoryId + "," + transId + ")", connection, transaction);
                     //nreminderID=myFunctions.getIntVAL(dLayer.ExecuteScalar("Select N_AttachmentID from DMS_MasterFiles where N_CompanyID=" + nCompanyID+" and n_UserID="+nUserID+" and N_TransID="+transId, connection, transaction).ToString());
                    //  object obj = dLayer.ExecuteScalar("Select N_FileID From DMS_MasterFiles Where N_ReminderID ="+nreminderID+" and N_CompanyID =" + nCompanyID, connection, transaction);
                    //             if (obj != null)
                    //             {
                    //  dLayer.DeleteData("DMS_MasterFiles", "N_FileID", myFunctions.getIntVAL(obj.ToString()),"N_CompanyID="+ nCompanyID, connection, transaction);
                    //  }
                        
                     ReminderId = ReminderSave(dLayer, FormID, partyID, strExpireDate, subject, filename, remCategoryId, 1, settingsId,nreminderID, User, transaction, connection);
                   // int ReminderId =myReminder.ReminderSet(dLayer,settingsId, partyID, strExpireDate,FormID, nUserID,  User,  connection,transaction);
                    dLayer.ExecuteNonQuery("update DMS_MasterFiles set N_ReminderID=" + ReminderId + " where N_FileID=" + FileID + " and N_CompanyID=" + nCompanyID, connection, transaction);
                }
                else{
                  //ReminderId = ReminderSave(dLayer, FormID, partyID, strExpireDate, subject, filename, remCategoryId, 1, settingsId,nreminderID, User, transaction, connection);
                dLayer.ExecuteNonQuery("insert into DMS_MasterFiles(N_CompanyID,N_FileID,X_FileCode,X_Name,X_Title,X_Contents,N_FolderID,N_UserID,X_refName,N_AttachmentID,N_FormID,N_TransID,N_ReminderID)values(" + nCompanyID + "," + FileID + ",'" + filecode + "','" + filename + "','" + category + "','" + subject + "'," + folderId + "," + nUserID + ",'" + refname + "'," + attachID + "," + FormID + "," + transId +","+ReminderId+ ")", connection, transaction);
                }// System.IO.File.Copy(sourcepath, destpath + refname, overwriteexisting);

                var base64Data = Regex.Match(fileData.ToString(), @"data:(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;
                byte[] FileBytes = Convert.FromBase64String(base64Data);
                File.WriteAllBytes(destpath + refname,
                                   FileBytes);

            }
            catch (Exception ex)
            {

            }

        }

        public int ReminderSave(IDataAccessLayer dLayer, int N_FormID, int partyId, string dateval, string strSubject, string Title, int CategoryID, int Isattachment, int settingsId,int nreminderID,ClaimsPrincipal User, SqlTransaction transaction, SqlConnection connection)
        {

            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);

           // DataTable Gen_Reminder = new DataTable();
            // Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "N_CompanyId", typeof(int), nCompanyID);
            // Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "N_ReminderId", typeof(int), 0);
            // Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "N_FormID", typeof(int), N_FormID);
            // Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "N_PartyID", typeof(int), partyId);
            // Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "X_Subject", typeof(string), strSubject);
            // Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "X_Title", typeof(string), Title);
            // if (dateval != "")
            // {
            //     DateTime dtExpire;
            //     dtExpire = Convert.ToDateTime(dateval);
            //     Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "D_ExpiryDate", typeof(DateTime), dtExpire.ToString("dd/MMM/yyyy"));
            //     Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "N_RemCategoryID", typeof(int), CategoryID);

            // }
            // Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "B_IsAttachment", typeof(bool), Isattachment);
            // Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "N_SettingsID", typeof(int), settingsId);
            // Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "N_UserID", typeof(int), nUserID);
            // Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "N_Processed", typeof(int), 0);
            // Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "D_EntryDate", typeof(DateTime), DateTime.Now);

            DataTable dtSave = new DataTable();
                dtSave.Clear();
                dtSave.Columns.Add("N_CompanyID");
                dtSave.Columns.Add("N_FormID");
                dtSave.Columns.Add("N_PartyID");
                dtSave.Columns.Add("X_Subject");
                dtSave.Columns.Add("X_Title");
                dtSave.Columns.Add("D_ExpiryDate");
                dtSave.Columns.Add("N_RemCategoryID");
                dtSave.Columns.Add("B_IsAttachment");
                dtSave.Columns.Add("N_SettingsID");
                dtSave.Columns.Add("N_UserID");
                dtSave.Columns.Add("N_ReminderId");

                DataRow row = dtSave.NewRow();
                row["N_ReminderId"] = 0;
                row["N_CompanyID"] = myFunctions.GetCompanyID(User);
                row["N_FormID"] = N_FormID;
                row["N_PartyID"] = partyId;
                row["X_Subject"] = strSubject;
                row["X_Title"] = Title;
                row["D_ExpiryDate"] = dateval;
                row["N_RemCategoryID"] = CategoryID;
                row["B_IsAttachment"] = 0;
                row["N_SettingsID"] = settingsId;
                row["N_UserID"] = nUserID;
                dtSave.Rows.Add(row);

            object Result = 0;
            //dLayer.DeleteData("Gen_Reminder", "N_PartyID", partyId, "N_FormID=" + N_FormID + " and N_CompanyID=" + nCompanyID, connection, transaction);
             //dLayer.DeleteData("Gen_Reminder", "N_PartyID="+ N_FormID + "' and N_FormID= "+ N_FormID + "' and  N_CompanyID=" + myFunctions.GetCompanyID(User), connection, transaction);
            Result=dLayer.SaveData("Gen_Reminder", "N_ReminderId", dtSave, connection, transaction);
            if (myFunctions.getIntVAL(Result.ToString()) > 0)
            {
                return myFunctions.getIntVAL(Result.ToString());

            }
            return 0;
        }


        public DataTable ViewAttachment(IDataAccessLayer dLayer, int PartyId, int TransID, int FormID, int FnYearID, ClaimsPrincipal User, SqlConnection connection)
        {
            SortedList AttachmentParam = new SortedList(){
                                    {"PartyID", PartyId},
                                    {"PayID", TransID},
                                    {"FormID", FormID},
                                    {"CompanyID", myFunctions.GetCompanyID(User)},
                                    {"FnyearID",FnYearID}
                                    };

            DataTable ImageData = dLayer.ExecuteDataTablePro("SP_VendorAttachments", AttachmentParam, connection);
            ImageData = myFunctions.AddNewColumnToDataTable(ImageData, "FileData", typeof(string), null);
            ImageData = myFunctions.AddNewColumnToDataTable(ImageData, "TempFileName", typeof(string), null);

            if (ImageData.Rows.Count > 0)
            {
                ImageData = myFunctions.AddNewColumnToDataTable(ImageData, "n_CompanyID", typeof(int), myFunctions.GetCompanyID(User));
                ImageData = myFunctions.AddNewColumnToDataTable(ImageData, "n_FnYearID", typeof(int), FnYearID);
                ImageData = myFunctions.AddNewColumnToDataTable(ImageData, "n_TransID", typeof(int), TransID);
            }

            foreach (DataRow var in ImageData.Rows)
            {
                if (var["x_refName"] != null)
                {
                    var path = var["x_refName"].ToString();
                    if (File.Exists(path))
                    {
                        Byte[] bytes = File.ReadAllBytes(path);
                        var random = RandomString();
                        File.Copy(path, this.TempFilesPath + random + "." + var["x_Extension"].ToString());
                        var["TempFileName"] = random + "." + var["x_Extension"].ToString();
                        var["FileData"] = "data:" + api.GetContentType(path) + ";base64," + Convert.ToBase64String(bytes);
                    }
                }

            }
            ImageData.AcceptChanges();

            return ImageData;

        }
        private static Random random = new Random();

        public string RandomString(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void DeleteAttachment(IDataAccessLayer dLayer, int type, int nTransID, int nPartyID, int nFnYearID, int formId, ClaimsPrincipal User, SqlTransaction transaction, SqlConnection connection)
        {
            try
            {
                string s = "";
                string X_fileType = "";
                int nCompanyID = myFunctions.GetCompanyID(User);

                SortedList AttachmentParam = new SortedList(){
                                    {"PartyID", nPartyID},
                                    {"PayID", nTransID},
                                    {"FormID", formId},
                                    {"CompanyID", myFunctions.GetCompanyID(User)},
                                    {"FnyearID",nFnYearID}
                                    };

                DataTable dsAttachment = dLayer.ExecuteDataTablePro("SP_VendorAttachments", AttachmentParam, connection, transaction);

                if (DocumentsFolder != "")
                {
                    if (!Directory.Exists(DocumentsFolder))
                        Directory.CreateDirectory(DocumentsFolder);
                }
                if (type == 1)
                {
                    for (int i = 0; i <= dsAttachment.Rows.Count - 1; i++)
                    {
                        if (dsAttachment.Rows[i]["X_Extension"].ToString() != "")
                            X_fileType = "File";
                        else
                            X_fileType = "Folder";
                        if (X_fileType == "File")
                        {

                            try
                            {
                                object obj = dLayer.ExecuteScalar("Select N_FileID From DMS_MasterFiles Where X_refName='" + Path.GetFileName(dsAttachment.Rows[i]["X_refName"].ToString()) + "' and N_CompanyID =" + nCompanyID, connection, transaction);
                                if (obj != null)
                                {
                                    object objReminder = dLayer.ExecuteScalar("Select N_ReminderID From DMS_MasterFiles Where N_FileID=" + myFunctions.getIntVAL(obj.ToString()) + " and N_CompanyID =" + nCompanyID, connection, transaction);
                                    dLayer.DeleteData("DMS_MasterFiles", "N_FileID", myFunctions.getIntVAL(obj.ToString()), "X_refName='" + Path.GetFileName(dsAttachment.Rows[i]["X_refName"].ToString()) + "' and N_CompanyID=" + nCompanyID, connection, transaction);
                                    if (objReminder != null)
                                        myReminder.ReminderDelete(dLayer, myFunctions.getIntVAL(objReminder.ToString()),nCompanyID, connection, transaction);
                                    File.Delete(dsAttachment.Rows[i]["X_refName"].ToString());
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        // else if (X_fileType == "Folder")
                        // {
                        //     object obj;
                        //     obj = dLayer.ExecuteScalar("Select N_FolderID From DMS_MasterFolder Where X_Path='" + dsAttachment.Tables["Attachment"].Rows[i]["X_refName"].ToString() + "' and X_Name='" + dsAttachment.Tables["Attachment"].Rows[i]["X_File"].ToString() + "' and N_CompanyID =" + myCompanyID._CompanyID,connection,transaction);
                        //     if (obj != null)
                        //     {
                        //         deleteList(dLayer, myFunctions.getIntVAL(obj.ToString()), s, 1,connection,transaction);
                        //     }
                        // }

                    }
                    // if (formId == 113)
                    //     dLayer.DeleteData("Acc_CompanyAttachments", "N_CompanyID", nCompanyID, "N_FnyearID=" + nFnYearID, connection, transaction);
                    // else
                        dLayer.DeleteData("Dms_ScreenAttachments", "N_PartyID", nPartyID, "N_FormID=" + formId + " and N_CompanyID=" + nCompanyID + " and N_FnyearID=" + nFnYearID, connection, transaction);

                    dLayer.DeleteData("Gen_Reminder", "N_PartyID", nPartyID, "N_FormID=" + formId + " and N_CompanyID=" + nCompanyID, connection, transaction);

                }
            }
            catch (Exception ex)
            {

            }
        }




        public void DeleteAttachment(IDataAccessLayer dLayer, int formId, int type, int nFnYearID, int nAttachmentId, ClaimsPrincipal User, SqlTransaction transaction, SqlConnection connection)
        {
            try
            {
                string s = "";
                string X_fileType = "";
                SortedList AutoParam = new SortedList();
                if (DocumentsFolder != "")
                {
                    if (!Directory.Exists(DocumentsFolder))
                        Directory.CreateDirectory(DocumentsFolder);
                }


                int nCompanyID = myFunctions.GetCompanyID(User);


                DataTable dsAttachment = dLayer.ExecuteDataTable("SELECT     Dms_ScreenAttachments.N_AttachmentID, Dms_ScreenAttachments.N_PartyID, Dms_ScreenAttachments.N_FormID, Dms_ScreenAttachments.N_PartyID," +
                     " Dms_ScreenAttachments.X_Subject, Dms_ScreenAttachments.X_Filename, Dms_ScreenAttachments.X_Extension, Dms_ScreenAttachments.X_File, Inv_AttachmentCategory.N_CategoryID, Inv_AttachmentCategory.X_Category, Dms_ScreenAttachments.X_refName, Dms_ScreenAttachments.D_ExpiryDate, Dms_ReminderCategory.N_CategoryID AS N_RemCategoryID, Dms_ReminderCategory.X_Category AS X_RemCategory, 0 as N_ActionID " +
                     " FROM  Dms_ScreenAttachments LEFT OUTER JOIN Dms_ReminderCategory ON Dms_ScreenAttachments.N_CompanyID = Dms_ReminderCategory.N_CompanyID AND  Dms_ScreenAttachments.N_RemCategoryID = Dms_ReminderCategory.N_CategoryID LEFT OUTER JOIN  Inv_AttachmentCategory ON Dms_ScreenAttachments.N_CategoryID = Inv_AttachmentCategory.N_CategoryID AND Dms_ScreenAttachments.N_CompanyID = Inv_AttachmentCategory.N_CompanyID " +
                     " where [dbo].[Dms_ScreenAttachments].N_CompanyID= " + nCompanyID + " and [dbo].[Dms_ScreenAttachments].N_AttachmentID =" + nAttachmentId + "  and (Dms_ScreenAttachments.N_FormID= " + formId + "  or " + formId + " ='0')", AutoParam, connection, transaction);


                if (dsAttachment.Rows[0]["X_Extension"].ToString() != "")
                    X_fileType = "File";
                else
                    X_fileType = "Folder";
                if (type == 2)
                {
                    if (X_fileType == "File")
                    {

                        try
                        {
                            object obj = dLayer.ExecuteScalar("Select N_FileID From DMS_MasterFiles Where X_refName='" + Path.GetFileName(dsAttachment.Rows[0]["X_refName"].ToString()) + "' and N_CompanyID =" + nCompanyID, connection, transaction);
                            if (obj != null)
                            {
                                object objReminder = dLayer.ExecuteScalar("Select N_ReminderID From DMS_MasterFiles Where N_FileID=" + myFunctions.getIntVAL(obj.ToString()) + " and N_CompanyID =" + nCompanyID, connection, transaction);
                                dLayer.DeleteData("DMS_MasterFiles", "N_FileID", myFunctions.getIntVAL(obj.ToString()), "X_refName='" + Path.GetFileName(dsAttachment.Rows[0]["X_refName"].ToString()) + "' and N_CompanyID=" + nCompanyID, connection, transaction);
                                if (objReminder != null)
                                    myReminder.ReminderDelete(dLayer, myFunctions.getIntVAL(objReminder.ToString()),nCompanyID, connection, transaction);
                                //   dba.ExecuteNonQuery("delete from DMS_MasterFiles where X_refName='" + dr.Cells["dgvRefname"].FormattedValue.ToString() + "' and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());
                                File.Delete(dsAttachment.Rows[0]["X_refName"].ToString());
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    // else if (dr.Cells["Type"].FormattedValue.ToString() == "Folder")
                    // {
                    //     object obj;
                    //     obj = dba1.ExecuteSclar("Select N_FolderID From DMS_MasterFolder Where X_Path='" + dr.Cells["dgvRefname"].FormattedValue.ToString() + "' and X_Name='" + dr.Cells["dgvFile"].FormattedValue.ToString() + "' and N_CompanyID =" + myCompanyID._CompanyID, "TEXT", new DataTable());
                    //     if (obj != null)
                    //     {
                    //         deleteList(dba1, myFunctions.getIntVAL(obj.ToString()), s, 1);
                    //     }
                    // }
                    // if (formId == 113)
                    //     dLayer.DeleteData("Acc_CompanyAttachments", "N_AttachmentID", nAttachmentId, "N_CompanyID=" + nCompanyID + " and N_FnyearID=" + nFnYearID, connection, transaction);
                    // else
                    // {
                        
                      int result =  dLayer.DeleteData("Dms_ScreenAttachments", "N_AttachmentID",nAttachmentId , "", connection, transaction);
                    //}

                }
            }
            catch (Exception ex)
            {

            }


        }

    }









    public interface IMyAttachments
    {
        public void SaveAttachment(IDataAccessLayer dLayer, DataTable dsAttachment, string payCode, int payId, string partyname, string partycode, int partyId, string X_folderName, ClaimsPrincipal User, SqlConnection connection, SqlTransaction transaction);
        public void DeleteAttachment(IDataAccessLayer dLayer, int type, int nTransID, int nPartyID, int nFnYearID, int formId, ClaimsPrincipal User, SqlTransaction transaction, SqlConnection connection);
        public DataTable ViewAttachment(IDataAccessLayer dLayer, int PartyId, int TransID, int FormID, int FnYearID, ClaimsPrincipal User, SqlConnection connection);

        public void DeleteAttachment(IDataAccessLayer dLayer, int formId, int type, int nFnYearID, int nAttachmentId, ClaimsPrincipal User, SqlTransaction transaction, SqlConnection connection);

    }
}
