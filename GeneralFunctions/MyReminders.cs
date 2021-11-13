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
    public class MyReminders : IMyReminders
    {
        private readonly IMyFunctions myFunctions;
        private readonly IApiFunctions api;
        public MyReminders(IApiFunctions apifun, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            myFunctions = myFun;
        }
        public void ReminderSet(IDataAccessLayer dLayer,int settingsID, int partyId, string dateval,int formId,int UserID, ClaimsPrincipal User, SqlConnection connection, SqlTransaction transaction)
        {
            DataSet dsSettings = new DataSet();
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string str="";
            string strSub="";
            int categoryId=0;

            Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
            Params.Add("@settingsID", settingsID);

            str="Select X_Subject,N_CategoryID from Gen_ReminderSettings where N_CompanyID=@nCompanyID and N_ID=@settingsID";

            dt = dLayer.ExecuteDataTable(str, Params, connection,transaction);
            dt = api.Format(dt, "ReminderSettings");
            dsSettings.Tables.Add(dt);

            if (dsSettings != null && dsSettings.Tables != null && dsSettings.Tables["ReminderSettings"].Rows.Count > 0)
            {
                strSub = dsSettings.Tables["ReminderSettings"].Rows[0]["X_Subject"].ToString();
                categoryId = myFunctions.getIntVAL(dsSettings.Tables["ReminderSettings"].Rows[0]["N_CategoryID"].ToString());
            }
            if(strSub!="" && categoryId>0)
            {
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
                row["N_FormID"] = formId;
                row["N_PartyID"] = partyId;
                row["X_Subject"] = strSub;
                row["X_Title"] = strSub;
                row["D_ExpiryDate"] = dateval;
                row["N_RemCategoryID"] = categoryId;
                row["B_IsAttachment"] = 0;
                row["N_SettingsID"] = settingsID;
                row["N_UserID"] = UserID;
                dtSave.Rows.Add(row);

                ReminderSave(dLayer,dtSave, connection,transaction);
            }
            
        }

        public  DataSet ReminderSettings(IDataAccessLayer dLayer, int settingsID,int formId, ClaimsPrincipal User, SqlConnection connection, SqlTransaction transaction)
        {
            DataSet dsSettings = new DataSet();
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string str="";

            Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
            Params.Add("@settingsID", settingsID);
            Params.Add("@formId", formId);

            str = "Select X_Subject,N_CategoryID from Gen_ReminderSettings where N_CompanyID=@nCompanyID and N_ID=@settingsID and N_FormID=@formId";

            dt = dLayer.ExecuteDataTable(str, Params, connection,transaction);
            dt = api.Format(dt, "ReminderSettings");
            dsSettings.Tables.Add(dt);

            return dsSettings;
        }

        public int ReminderSave(IDataAccessLayer dLayer,  DataTable dsAttachment, SqlConnection connection, SqlTransaction transaction)
        {
            int N_ReminderId = 0;

            N_ReminderId = dLayer.SaveData("Gen_Reminder", "N_ReminderId", dsAttachment, connection, transaction);
            if (N_ReminderId <= 0)
            {
                transaction.Rollback();
                return myFunctions.getIntVAL(N_ReminderId.ToString());
            }
            return 0;
        }
        public  void ReminderDelete(IDataAccessLayer dLayer, int ReminderID,SqlConnection connection, SqlTransaction transaction)
        {
            dLayer.DeleteData("Gen_Reminder", "N_ReminderId", ReminderID, "N_ReminderId=" + ReminderID.ToString() + " and N_CompanyID=" + myCompanyID._CompanyID ,connection, transaction);
        }
        public  void ReminderDelete(IDataAccessLayer dLayer, int PartyID,int FormID,SqlConnection connection, SqlTransaction transaction)
        {
            dLayer.ExecuteNonQuery("delete from Gen_Reminder where N_FormID=" + FormID + " and N_PartyID=" + PartyID + " and N_CompanyID=" + myCompanyID._CompanyID + " and B_IsAttachment=0",connection, transaction);
        }
        public  void ReminderDelete(IDataAccessLayer dLayer, int PartyID, int FormID,int settingsid,SqlConnection connection, SqlTransaction transaction)
        {
            dLayer.ExecuteNonQuery("delete from Gen_Reminder where N_FormID=" + FormID + " and N_PartyID=" + PartyID + " and N_CompanyID=" + myCompanyID._CompanyID + " and B_IsAttachment=1 and N_SettingsID="+ settingsid ,connection, transaction);
        }
    }
    public interface IMyReminders
    {
        public void ReminderSet(IDataAccessLayer dLayer,int settingsID, int partyId, string dateval,int formId,int UserID, ClaimsPrincipal User, SqlConnection connection, SqlTransaction transaction);
        public  DataSet ReminderSettings(IDataAccessLayer dLayer, int settingsID,int formId, ClaimsPrincipal User, SqlConnection connection, SqlTransaction transaction);
        public int ReminderSave(IDataAccessLayer dLayer,  DataTable dsAttachment, SqlConnection connection, SqlTransaction transaction);
        public  void ReminderDelete(IDataAccessLayer dLayer, int ReminderID,SqlConnection connection, SqlTransaction transaction);
        public  void ReminderDelete(IDataAccessLayer dLayer, int PartyID,int FormID,SqlConnection connection, SqlTransaction transaction);
        public  void ReminderDelete(IDataAccessLayer dLayer, int PartyID, int FormID,int settingsid,SqlConnection connection, SqlTransaction transaction);
    
    }
}
