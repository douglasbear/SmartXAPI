using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartxAPI.GeneralFunctions
{
    public class MyAttachments : IMyAttachments
    {
        private readonly IMyFunctions myFunctions;
        private readonly string reportPath;
        private readonly string startupPath;
        public MyAttachments(IMyFunctions myFun, IConfiguration conf)
        {
            myFunctions = myFun;
            reportPath = conf.GetConnectionString("ReportPath");
            startupPath = conf.GetConnectionString("StartupPath");
        }

        public void SaveAttachment(IDataAccessLayer dLayer, DataTable dsAttachment, string payCode, int payId, string partyname, string partycode, int partyId, string X_folderName, ClaimsPrincipal User, SqlConnection connection, SqlTransaction transaction)
        {
            object Result = 0;
            string path = "";
            string s = "";
            DataRow AttachmentRow = dsAttachment.Rows[0];
            int FormID = myFunctions.getIntVAL(AttachmentRow["n_FormID"].ToString());
            int FnYearID = myFunctions.getIntVAL(AttachmentRow["n_FnYearID"].ToString());
            int nCompanyID = myFunctions.GetCompanyID(User);
            string xCompanyName = myFunctions.GetCompanyName(User);
            int N_AttachmentID = 0;
            string ExpiryDate = "";
            int N_remCategory = 0;
            int N_FolderID = 0;
            object obj = dLayer.ExecuteScalar("Select X_Value  From Gen_Settings Where N_CompanyID=" + nCompanyID + " and X_Group='188' and X_Description='EmpDocumentLocation'", connection, transaction);
            string DocumentPath = obj != null && obj.ToString() != "" ? obj.ToString() : this.reportPath;
            if (dsAttachment.Rows.Count > 0)
            {

                N_FolderID = DocFolderInsert(dLayer, xCompanyName + "//" + X_folderName + "//" + payCode + "//", 1, 0, FormID,User, connection, transaction);
                if (DocumentPath != "")
                {
                    if (!Directory.Exists(DocumentPath + myCompanyID._DocumtFolder))
                        Directory.CreateDirectory(DocumentPath + myCompanyID._DocumtFolder);
                    s = DocumentPath + myCompanyID._DocumtFolder + "\\";
                }
                else
                    s = this.startupPath + myCompanyID._DocumtFolder + "\\";
                if (s == "0" || !Directory.Exists(s))
                {
                    throw new Exception("Invalid Path");
                }
                else
                {
                    for (int i = 0; i <= dsAttachment.Rows.Count - 1; i++)
                    {


                        dsAttachment.Rows[i]["N_TransID"] = payId;
                        // dLayer.SaveData(ref Result, "Inv_AttachmentCategory", "N_CategoryID", "0", "N_CompanyID,X_Category", nCompanyID + "|'" + dsAttachment.Rows[i]["X_Category"].ToString() + "'", "", "", "X_Category='" + dsAttachment.Rows[i]["X_Category"].ToString() + "'", "");
                        // Result = 0;

                        path = s + "\\" + payCode;  //+ "\\" + txtVendorCode.Text.Trim();
                        N_AttachmentID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select  Isnull(max(N_AttachmentID),0)+1 from Dms_ScreenAttachments", connection, transaction).ToString());
                        if (dsAttachment.Rows[i]["N_AttachmentID"].ToString() != "" && dsAttachment.Rows[i]["N_AttachmentID"].ToString() != "0")
                        {
                            if (FormID == 113)
                            {
                                dLayer.DeleteData("Acc_CompanyAttachments", "N_AttachmentID", myFunctions.getIntVAL(dsAttachment.Rows[i]["N_AttachmentID"].ToString()), "", connection, transaction);
                            }
                            else
                            {
                                dLayer.DeleteData("Dms_ScreenAttachments", "N_AttachmentID", myFunctions.getIntVAL(dsAttachment.Rows[i]["N_AttachmentID"].ToString()), "", connection, transaction);
                            }
                            N_AttachmentID = myFunctions.getIntVAL(dsAttachment.Rows[i]["N_AttachmentID"].ToString());
                        }
                        string FileType = "";
                        if (dsAttachment.Rows[i]["X_Extension"].ToString() != "")
                            FileType = "File";
                        else
                            FileType = "Folder";

                        ExpiryDate = "";
                        N_remCategory = 0;
                        if (dsAttachment.Rows[i]["D_ExpiryDate"].ToString() != "")
                            ExpiryDate = Convert.ToDateTime(dsAttachment.Rows[i]["D_ExpiryDate"].ToString()).ToString("dd/MMM/yyyy");
                        if (dsAttachment.Rows[i]["X_RemCategory"].ToString() != "")
                            N_remCategory = Convert.ToInt32(dLayer.ExecuteScalar("Select N_CategoryID from Dms_ReminderCategory where X_Category='" + dsAttachment.Rows[i]["X_RemCategory"].ToString() + "' and N_CompanyID=" + nCompanyID, connection, transaction));

                        if (FileType == "File")
                        {
                            if (dsAttachment.Rows[i]["X_Category"].ToString() != "")
                            {
                                N_FolderID = DocFolderInsert(dLayer, xCompanyName + "//" + X_folderName + "//" + payCode + "//" + dsAttachment.Rows[i]["X_Category"].ToString() + "//", 0, N_AttachmentID, FormID,User, connection, transaction);

                            }

                            if (dsAttachment.Rows[i]["X_Filename"].ToString() != s + "\\" + payCode + "\\" + dsAttachment.Rows[i]["X_File"].ToString())
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
                                    CopyFiles(dLayer, dsAttachment.Rows[i]["X_File"].ToString(), dsAttachment.Rows[i]["X_Subject"].ToString(), N_FolderID, true, dsAttachment.Rows[i]["X_Category"].ToString(), dsAttachment.Rows[i]["FileData"].ToString(), s, fileCode, N_AttachmentID, FormID, ExpiryDate, N_remCategory, payId, partyId, 0,User, transaction, connection);
                                    dsAttachment.Rows[i]["X_refName"] = s + fileCode + extension;
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Error While Copying");

                                }
                            }
                            else
                            {
                                object obj1 = dLayer.ExecuteScalar("select N_ReminderID from DMS_MasterFiles Where X_refName = '" + Path.GetFileName(dsAttachment.Rows[i]["X_refName"].ToString()) + "' and N_CompanyID = " + nCompanyID, connection, transaction);
                                if (obj1.ToString() != "")
                                {
                                    dLayer.ExecuteNonQuery("update Gen_Reminder set D_ExpiryDate = '" + Convert.ToDateTime(ExpiryDate).ToString("dd/MMM/yyyy") + "' ,N_RemCategoryID=" + N_remCategory + " where N_ReminderID=" + myFunctions.getIntVAL(obj.ToString()) + " and N_CompanyID=" + nCompanyID, connection, transaction);
                                }
                                else
                                {
                                    if (ExpiryDate != "")
                                    {
                                        // int ReminderId = myReminders.ReminderSave(dba1, FormID, partyId, ExpiryDate, dsAttachment.Rows[i]["X_Subject"].ToString(), dsAttachment.Rows[i]["X_Filename"].ToString(), N_remCategory, 1, 0);
                                        // dLayer.ExecuteNonQuery("update DMS_MasterFiles set N_ReminderID=" + ReminderId + " where X_refName='" + Path.GetFileName(dsAttachment.Rows[i]["X_refName"].ToString()) + "' and N_CompanyID=" + nCompanyID, "TEXT", new DataTable());
                                    }
                                }
                            }
                        }
                        string FieldList = "";
                        string FieldValues = "";
                        if (FormID == 113)
                        {
                            if (ExpiryDate != "")
                            {
                                FieldList = "N_CompanyID,N_FnyearID,N_FormID,X_Subject,X_FileName,X_File,X_extension,X_refName,D_ExpiryDate,N_RemCategoryID";
                                FieldValues = nCompanyID + "|" + myCompanyID._FnYearID + "|" + FormID + "|'" + dsAttachment.Rows[i]["X_Subject"].ToString() + "'|'" + path + "\\" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_Extension"].ToString() + "'|'" + dsAttachment.Rows[i]["X_refName"].ToString() + "','" + ExpiryDate + "'," + N_remCategory;
                            }
                            else
                            {
                                FieldList = "N_CompanyID,N_FnyearID,N_FormID,X_Subject,X_FileName,X_File,X_extension,X_refName";
                                FieldValues = nCompanyID + "|" + myCompanyID._FnYearID + "|" + FormID + "|'" + dsAttachment.Rows[i]["X_Subject"].ToString() + "'|'" + path + "\\" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_File"].ToString() + "'|'" + dsAttachment.Rows[i]["X_Extension"].ToString() + "'|'" + dsAttachment.Rows[i]["X_refName"].ToString() + "'";
                            }
                            string refField = "N_CategoryID";
                            string refValue = "Inv_AttachmentCategory|N_CategoryID|X_Category='" + dsAttachment.Rows[i]["X_Category"].ToString() + "'";

                            string DupCriteria = "";
                            // dLayer.SaveData(ref Result, "Acc_CompanyAttachments", "N_AttachmentID", N_AttachmentID.ToString(), FieldList, FieldValues, refField, refValue, DupCriteria, "");
                            dLayer.SaveData("Acc_CompanyAttachments", "N_AttachmentID", dsAttachment, connection, transaction);

                            if (myFunctions.getIntVAL(Result.ToString()) > 0)
                            {

                            }

                        }
                        else
                        {
                            if (ExpiryDate == "")
                            {
                            dsAttachment.Columns.Remove("D_ExpiryDate");
                            dsAttachment.Columns.Remove("N_RemCategoryID");
 }
                          dsAttachment.Columns.Remove("FileData");
                            dsAttachment.Columns.Remove("x_RemCategory");
                            dsAttachment.Columns.Remove("x_Category");
                            dsAttachment.AcceptChanges();
                            dLayer.SaveData("Dms_ScreenAttachments", "N_AttachmentID", dsAttachment, connection, transaction);
                            if (myFunctions.getIntVAL(Result.ToString()) > 0)
                            {

                            }
                        }
                    }
                }

            }
        }



        private int DocFolderInsert(IDataAccessLayer dLayer, string Path, int type, int AttachID, int FormID,ClaimsPrincipal User, SqlConnection connection, SqlTransaction transaction)
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
                    pid = InsertFolder(dLayer, subFolders[i], pid, x_path, type, AttachID, FormID,User, connection, transaction);
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


        private int InsertFolder(IDataAccessLayer dLayer, string Name, int ParentID, string Path, int foldertype, int attID, int fId,ClaimsPrincipal User, SqlConnection connection, SqlTransaction transaction)
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

        public void CopyFiles(IDataAccessLayer dLayer, string filename, string subject, int folderId, bool overwriteexisting, string category, string fileData, string destpath, string filecode, int attachID, int FormID, string strExpireDate, int remCategoryId, int transId, int partyID, int settingsId,ClaimsPrincipal User, SqlTransaction transaction, SqlConnection connection)
        {
            try
            {
                DateTime dtExpire = new DateTime();
                if (strExpireDate != "")
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
                if (strExpireDate != "")
                {
                    dLayer.ExecuteNonQuery("insert into DMS_MasterFiles(N_CompanyID,N_FileID,X_FileCode,X_Name,X_Title,X_Contents,N_FolderID,N_UserID,X_refName,N_AttachmentID,N_FormID,D_ExpiryDate,N_CategoryID,N_TransID)values(" + nCompanyID + "," + FileID + ",'" + filecode + "','" + filename + "','" + category + "','" + subject + "'," + folderId + "," + nUserID + ",'" + refname + "'," + attachID + "," + FormID + ",'" + dtExpire.ToString("dd/MMM/yyyy") + "'," + remCategoryId + "," + transId + ")", connection, transaction);
                    int ReminderId = ReminderSave(dLayer, FormID, partyID, strExpireDate, subject, filename, remCategoryId, 1, settingsId,User, transaction, connection);
                    dLayer.ExecuteNonQuery("update DMS_MasterFiles set N_ReminderID=" + ReminderId + " where N_FileID=" + FileID + " and N_CompanyID=" + nCompanyID, connection, transaction);
                }
                else
                    dLayer.ExecuteNonQuery("insert into DMS_MasterFiles(N_CompanyID,N_FileID,X_FileCode,X_Name,X_Title,X_Contents,N_FolderID,N_UserID,X_refName,N_AttachmentID,N_FormID,N_TransID)values(" + nCompanyID + "," + FileID + ",'" + filecode + "','" + filename + "','" + category + "','" + subject + "'," + folderId + "," + nUserID + ",'" + refname + "'," + attachID + "," + FormID + "," + transId + ")", connection, transaction);
                // System.IO.File.Copy(sourcepath, destpath + refname, overwriteexisting);

                var base64Data = Regex.Match(fileData.ToString(), @"data:(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;
                byte[] FileBytes = Convert.FromBase64String(base64Data);
                File.WriteAllBytes(destpath + refname,
                                   FileBytes);

            }
            catch (Exception ex)
            {

            }

        }

        public int ReminderSave(IDataAccessLayer dLayer, int N_FormID, int partyId, string dateval, string strSubject, string Title, int CategoryID, int Isattachment, int settingsId,ClaimsPrincipal User, SqlTransaction transaction, SqlConnection connection)
        {
            string FieldList = "";
            string FieldValues = "";
            int N_AssemblyID = 0;
            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);

            DataTable Gen_Reminder = new DataTable();
            Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "N_CompanyId", typeof(int), nCompanyID);
            Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "N_FormID", typeof(int), N_FormID);
            Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "N_PartyID", typeof(int), partyId);
            Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "X_Subject", typeof(string), strSubject);
            Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "X_Title", typeof(string), Title);
            if (dateval != "")
            {
                DateTime dtExpire;
                dtExpire = Convert.ToDateTime(dateval);
                Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "D_ExpiryDate", typeof(DateTime), dtExpire.ToString("dd/MMM/yyyy"));
                Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "N_RemCategoryID", typeof(int), CategoryID);

            }
            Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "B_IsAttachment", typeof(bool), Isattachment);
            Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "N_SettingsID", typeof(int), settingsId);
            Gen_Reminder = myFunctions.AddNewColumnToDataTable(Gen_Reminder, "N_UserID", typeof(int), nUserID);

            object Result = 0;
            dLayer.SaveData("Gen_Reminder", "N_ReminderId", Gen_Reminder, connection, transaction);
            if (myFunctions.getIntVAL(Result.ToString()) > 0)
            {
                return myFunctions.getIntVAL(Result.ToString());

            }
            return 0;
        }


        public DataTable ViewAttachment(IDataAccessLayer dLayer, int PartyId, int TransID, int FormID ,int FnYearID,ClaimsPrincipal User,SqlConnection connection)
        {
                                                SortedList AttachmentParam = new SortedList(){
                                    {"PartyID", PartyId},
                                    {"PayID", TransID},
                                    {"FormID", FormID},
                                    {"CompanyID", myFunctions.GetCompanyID(User)},
                                    {"FnyearID",FnYearID}
                                    };

            return dLayer.ExecuteDataTablePro( "SP_VendorAttachments",AttachmentParam,connection);
    
        }




    }



    public interface IMyAttachments
    {
        public void SaveAttachment(IDataAccessLayer dLayer, DataTable dsAttachment, string payCode, int payId, string partyname, string partycode, int partyId, string X_folderName, ClaimsPrincipal User, SqlConnection connection, SqlTransaction transaction);
        public DataTable ViewAttachment(IDataAccessLayer dLayer, int PartyId, int TransID, int FormID ,int FnYearID,ClaimsPrincipal User,SqlConnection connection);

    }
}