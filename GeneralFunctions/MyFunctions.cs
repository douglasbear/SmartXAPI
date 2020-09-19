using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.GeneralFunctions
{
    public class MyFunctions : IMyFunctions
    {
        public MyFunctions()
        {
        }

        public bool CheckPermission(int N_CompanyID, int N_MenuID, string admin, IDataAccessLayer dLayer, SqlConnection connection)
        {
            SortedList Params = new SortedList();
            Params.Add("@p1", N_CompanyID);
            Params.Add("@p2", N_MenuID);
            Params.Add("@p3", admin);
            bool Result = Convert.ToBoolean(dLayer.ExecuteScalar("Select ISNULL(B_Visible,0) From vw_userPrevileges Where N_CompanyID=@p1 and N_MenuID = @p2 and X_UserCategory=@p3", Params, connection));
            return Result;
        }

        public bool CheckPermission(int N_CompanyID, int N_MenuID, string admin, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction)
        {
            SortedList Params = new SortedList();
            Params.Add("@p1", N_CompanyID);
            Params.Add("@p2", N_MenuID);
            Params.Add("@p3", admin);
            bool Result = Convert.ToBoolean(dLayer.ExecuteScalar("Select ISNULL(B_Visible,0) From vw_userPrevileges Where N_CompanyID=@p1 and N_MenuID = @p2 and X_UserCategory=@p3", Params, connection, transaction));
            return Result;
        }
        public int getIntVAL(string val)
        {
            if (val.Trim() == "")
                return 0;
            else
                return Convert.ToInt32(val);
        }

        public string EncryptString(string inputString)
        {
            MemoryStream memStream = null;

            byte[] key = { };
            byte[] IV = { 12, 21, 43, 17, 57, 35, 67, 27 };
            string encryptKey = "aXb2uy4z";
            key = Encoding.UTF8.GetBytes(encryptKey);
            byte[] byteInput = Encoding.UTF8.GetBytes(inputString);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            memStream = new MemoryStream();
            ICryptoTransform transform = provider.CreateEncryptor(key, IV);
            CryptoStream cryptoStream = new CryptoStream(memStream, transform, CryptoStreamMode.Write);
            cryptoStream.Write(byteInput, 0, byteInput.Length);
            cryptoStream.FlushFinalBlock();

            return Convert.ToBase64String(memStream.ToArray());
        }

        public string DecryptString(string inputString)
        {
            MemoryStream memStream = null;

            byte[] key = { };
            byte[] IV = { 12, 21, 43, 17, 57, 35, 67, 27 };
            string encryptKey = "aXb2uy4z";
            key = Encoding.UTF8.GetBytes(encryptKey);
            byte[] byteInput = new byte[inputString.Length];
            byteInput = Convert.FromBase64String(inputString);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            memStream = new MemoryStream();
            ICryptoTransform transform = provider.CreateDecryptor(key, IV);
            CryptoStream cryptoStream = new CryptoStream(memStream, transform, CryptoStreamMode.Write);
            cryptoStream.Write(byteInput, 0, byteInput.Length);
            cryptoStream.FlushFinalBlock();


            Encoding encoding1 = Encoding.UTF8;
            if (memStream != null)
            {
                if (memStream.Length > 0)
                    return encoding1.GetString(memStream.ToArray());
                else
                    return inputString;
            }

            else
                return inputString;

        }

        public double getVAL(string val)
        {
            try
            {
                if (val.Trim() == "" || val.Trim() == "." || val.Trim() == "-")
                    return 0;
                else
                {
                    string ValWithFormat = val;
                    //if (val.Substring(ValWithFormat.Length - 3, 1) == ",")
                    //    ValWithFormat = ValWithFormat.Substring(0, ValWithFormat.Length - 3) + ValWithFormat.Substring(ValWithFormat.Length - 3).Replace(",", ".");
                    return Convert.ToDouble(ValWithFormat);
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public double Round(double val, int RoundDigit)
        {
            decimal RoundVal = 0;
            RoundVal = Decimal.Round(Convert.ToDecimal(val), RoundDigit);
            return (Convert.ToDouble(RoundVal.ToString()));

        }
        public double Round(string val, int RoundDigit)
        {
            decimal RoundVal = 0;
            RoundVal = Decimal.Round(getDecimalVAL(val), RoundDigit);
            return (getVAL(RoundVal.ToString()));

        }
        public string decimalPlaceString(int decimalNo)
        {
            string PlaceString = "";
            if (decimalNo == 2 || decimalNo == 0)
                PlaceString = "#,##0.00";
            else if (decimalNo == 3)
                PlaceString = "#,##0.000";
            else if (decimalNo == 4)
                PlaceString = "#,##0.0000";
            else
                PlaceString = "#,##0.00000";

            return PlaceString;

        }

        public bool getBoolVAL(string val)
        {
            if (val.Trim() == "")
                return false;
            else
                return Convert.ToBoolean(val);
        }
        public int getIntVAL(bool val)
        {
            if (val)
                return 1;
            else
                return 0;
        }
        public float getFloatVAL(string val)
        {
            if (val.Trim() == ".")
                return 0;
            if (val.Trim() == "")
                return 0;
            else
                return float.Parse(val);
        }
        public decimal getDecimalVAL(string val)
        {
            if (val.Trim() == ".")
                return 0;
            if (val.Trim() == "")
                return 0;
            else
                return Convert.ToDecimal(val);
        }
        public decimal getDecimalVAL(double val)
        {
            return Convert.ToDecimal(val);
        }

        public bool checkIsNull(DataRow Row, String KeyName)
        {
            if (Row[KeyName].ToString() == null || this.getIntVAL(Row[KeyName].ToString()) == 0)
            { return true; }
            else { return false; }
        }
        public string checkProcessed(string TableName, string ColumnReturn, string ColumnValidate, string ValidateValue, string Condition, SortedList Params, IDataAccessLayer dLayer, SqlConnection connection)
        {
            string Result = "";
            object obj = dLayer.ExecuteScalar("select " + ColumnReturn + " from " + TableName + " where " + ColumnValidate + "=" + ValidateValue + " and " + Condition + "", Params, connection);
            if (obj != null)
                Result = obj.ToString();
            return Result;
        }
        public string ReturnSettings(string Group, string Description, string ValueColumn, string ConditionColumn, string Value, SortedList Params, IDataAccessLayer dLayer, SqlConnection Connection)
        {
            string Result = "";
            object obj = dLayer.ExecuteScalar("select " + ValueColumn + " from Gen_Settings where X_Group='" + Group + "' and X_Description='" + Description + "' and N_CompanyID=@nCompanyID and "
             + ConditionColumn + "=" + Value + " ", Params, Connection);
            if (obj != null)
                Result = obj.ToString();
            return Result;
        }
        public string ReturnSettings(string Group, string Description, string ValueColumn, string ConditionColumn, string Value, SortedList Params, IDataAccessLayer dLayer, SqlConnection Connection, SqlTransaction transaction)
        {
            string Result = "";
            object obj = dLayer.ExecuteScalar("select " + ValueColumn + " from Gen_Settings where X_Group='" + Group + "' and X_Description='" + Description + "' and N_CompanyID=@nCompanyID and "
             + ConditionColumn + "=" + Value + " ", Params, Connection, transaction);
            if (obj != null)
                Result = obj.ToString();
            return Result;
        }
        public string ReturnSettings(string Group, string Description, string ValueColumn, string ConditionColumn, string Value, int nCompanyID, IDataAccessLayer dLayer, SqlConnection Connection)
        {
            string Result = "";
            object obj = dLayer.ExecuteScalar("select " + ValueColumn + " from Gen_Settings where X_Group='" + Group + "' and X_Description='" + Description + "' and N_CompanyID=" + nCompanyID + " and "
             + ConditionColumn + "=" + Value + " ", Connection);
            if (obj != null)
                Result = obj.ToString();
            return Result;
        }

        public string ReturnSettings(string Group, string Description, string ValueColumn, int nCompanyID, IDataAccessLayer dLayer, SqlConnection Connection)
        {
            string Result = "";
            object obj = dLayer.ExecuteScalar("select " + ValueColumn + " from Gen_Settings where X_Group='" + Group + "' and X_Description='" + Description + "' and N_CompanyID="+nCompanyID+ " ", Connection);
                    if (obj != null)
                Result = obj.ToString();
            return Result;
        }
        public string ReturnSettings(string Group, string Description, string ValueColumn, string ConditionColumn, string Value, int nCompanyID, IDataAccessLayer dLayer, SqlConnection Connection, SqlTransaction transaction)
        {
            string Result = "";
            object obj = dLayer.ExecuteScalar("select " + ValueColumn + " from Gen_Settings where X_Group='" + Group + "' and X_Description='" + Description + "' and N_CompanyID=" + nCompanyID + " and "
             + ConditionColumn + "=" + Value + " ", Connection, transaction);
            if (obj != null)
                Result = obj.ToString();
            return Result;
        }

        public string ReturnValue(string TableName, string ColumnReturn, string Condition, SortedList Params, IDataAccessLayer dLayer, SqlConnection connection)
        {
            string Result = "";
            object obj = dLayer.ExecuteScalar("select " + ColumnReturn + " from " + TableName + " where  " + Condition + "", Params, connection);
            if (obj != null)
                Result = obj.ToString();
            return Result;
        }
        public DataTable AddNewColumnToDataTable(DataTable MasterDt, string ColName, Type dataType, object Value)
        {
            DataColumn NewCol = new DataColumn(ColName, dataType);
            NewCol.DefaultValue = Value;
            MasterDt.Columns.Add(NewCol);
            return MasterDt;
        }

        public string getDateVAL(DateTime val)
        {
            return val.ToString(myCompanyID._SystemDateFormat, myCompanyID._EnglishCulture);

        }

        public DateTime GetFormatedDate(string val)
        {
            DateTime dt;
            DateTimeFormatInfo sysFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            DateTime.TryParseExact(val, sysFormat.ShortDatePattern, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt);

            return dt;
        }



        public DataTable saveApprovals(DataTable MasterTable,DataTable Approvals,IDataAccessLayer dLayer ,SqlConnection connection,SqlTransaction transaction)
        {
            DataRow MasterRow= MasterTable.Rows[0];
            DataRow ApprovalRow= Approvals.Rows[0];

            int N_MaxLevelID = 0, N_Submitter = 0;
            int N_ApprovalID = 0, N_IsApprovalSystem = 0 ,FormID =0,N_CompanyID=0,N_userLevel=0, N_SaveDraft=0,N_ProcStatus =0;

            N_ApprovalID = this.getIntVAL(ApprovalRow["approvalID"].ToString());
            N_IsApprovalSystem = this.getIntVAL(ApprovalRow["isApprovalSystem"].ToString());
            FormID = this.getIntVAL(ApprovalRow["formID"].ToString());
            N_ProcStatus = this.getIntVAL(ApprovalRow["saveTag"].ToString());
            N_CompanyID = this.getIntVAL(MasterRow["N_CompanyID"].ToString());
            N_userLevel = this.getIntVAL(ApprovalRow["nextApprovalLevel"].ToString());

            SortedList Params = new SortedList();
            Params.Add("@FormID",FormID);
            Params.Add("@nCompanyID",N_CompanyID);
            Params.Add("@nApprovalID",N_ApprovalID);

            if (N_ApprovalID == 0)
            {
                object ApprovalCode = dLayer.ExecuteScalar("Select N_ApprovalID from Sec_ApprovalSettings_General where N_FormID=@FormID and N_CompanyID=@nCompanyID",Params,connection,transaction);
                if (ApprovalCode != null)
                    Params["@nApprovalID"] = this.getIntVAL(ApprovalCode.ToString());
            }

            if (N_IsApprovalSystem == 1)
            {
                N_MaxLevelID = this.getIntVAL(dLayer.ExecuteScalar("Select Isnull (max(N_level),0) from Gen_ApprovalCodesDetails where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID",Params,connection,transaction).ToString());

                object OB_Submitter = dLayer.ExecuteScalar("Select isnull(N_level,0) as N_level from Gen_ApprovalCodesDetails where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_ActionTypeId=111",Params,connection,transaction);
                if (OB_Submitter != null)
                    N_Submitter = this.getIntVAL(OB_Submitter.ToString());
                else
                    N_Submitter = N_MaxLevelID;

                if (N_userLevel == N_MaxLevelID || N_userLevel == N_Submitter)
                    N_SaveDraft = 0;
                else
                    N_SaveDraft = 1;

                MasterTable = this.AddNewColumnToDataTable(MasterTable,"N_ApprovalLevelId",typeof(int),N_userLevel);
                MasterTable = this.AddNewColumnToDataTable(MasterTable,"N_ProcStatus",typeof(int),N_ProcStatus);
                MasterTable = this.AddNewColumnToDataTable(MasterTable,"B_IssaveDraft",typeof(int),N_SaveDraft);
            }
            else if (N_IsApprovalSystem == 0)
            {
                MasterTable = this.AddNewColumnToDataTable(MasterTable,"B_IssaveDraft",typeof(int),0);
            }
            return MasterTable;
        }

        public void logApprovals(DataTable Approvals,int N_CompanyID,int N_FnYearID, string X_TransType, int N_TransID,string X_TransCode, DateTime D_TransDate, int N_ApprovalUserID, int N_ApprovalUserCatID,int GroupID,string PartyName,int EmpID,string DepLevel,IDataAccessLayer dLayer,SqlConnection connection,SqlTransaction transaction )
        {
            DataRow ApprovalRow = Approvals.Rows[0];
            string X_Action =ApprovalRow["btnSaveText"].ToString();
            int N_ApprovalLevelID = this.getIntVAL(ApprovalRow["nextApprovalLevel"].ToString());
            int N_ProcStatusID = this.getIntVAL(ApprovalRow["saveTag"].ToString());
            int N_IsApprovalSystem = this.getIntVAL(ApprovalRow["isApprovalSystem"].ToString());
            int N_ApprovalID = this.getIntVAL(ApprovalRow["approvalID"].ToString());
            int N_FormID = this.getIntVAL(ApprovalRow["formID"].ToString());
            string Comments = ApprovalRow["comments"].ToString();
            
            int N_GroupID = 1,N_NxtUserID=0;
            N_GroupID = GroupID;

            SortedList LogParams = new SortedList();
            LogParams.Add("@nCompanyID",N_CompanyID);
            LogParams.Add("@nFormID",N_FormID);
            LogParams.Add("@nApprovalUserID",N_ApprovalUserID);
            LogParams.Add("@nTransID",N_TransID);
            LogParams.Add("@nApprovalLevelID",N_ApprovalLevelID);
            LogParams.Add("@nProcStatusID",N_ProcStatusID);
            LogParams.Add("@nApprovalID",N_ApprovalID);
            LogParams.Add("@nGroupID",N_GroupID);
            LogParams.Add("@nFnYearID",N_FnYearID);
            LogParams.Add("@xAction",X_Action);
            LogParams.Add("@nEmpID",EmpID);
            LogParams.Add("@xDepLevel",DepLevel);
            
            if (N_IsApprovalSystem == 1)
            {
                dLayer.ExecuteNonQuery("SP_Gen_ApprovalCodesTrans @nCompanyID,@nFormID,@nApprovalUserID,@nTransID,@nApprovalLevelID,@nProcStatusID,@nApprovalID,@nGroupID,@nFnYearID,@xAction,@nEmpID,@xDepLevel",LogParams,connection,transaction);

                object NxtUser = null;
                NxtUser = dLayer.ExecuteScalar("select N_UserID from Gen_ApprovalCodesTrans where N_CompanyID=@nCompanyID and N_FormID=@nFormID and N_TransID=@nTransID and N_Status=0",LogParams,connection,transaction);
                if (NxtUser != null)
                    N_NxtUserID = this.getIntVAL(NxtUser.ToString());
            
            LogParams.Add("@xTransType",X_TransType);
            LogParams.Add("@nApprovalUserCatID",N_ApprovalUserCatID);
            LogParams.Add("@xSystemName","WebRequest");
            LogParams.Add("@xTransCode",X_TransCode);
            LogParams.Add("@dTransDate",D_TransDate.ToString("dd/MMM/yyyy"));
            LogParams.Add("@xComments",Comments);
            LogParams.Add("@xPartyName",PartyName);
            LogParams.Add("@nNxtUserID",N_NxtUserID);
                dLayer.ExecuteNonQuery("SP_Log_Approval_Status @nCompanyID,@nFnYearID,@xTransType,@nTransID,@nFormID,@nApprovalUserID,@nApprovalUserCatID,@xAction,@xSystemName,@xTransCode,@dTransDate,@nApprovalLevelID,@nApprovalUserID,@nProcStatusID,@xComments,@xPartyName,@nNxtUserID",LogParams,connection,transaction);
                
                object Count = null;
                SortedList NewParam = new SortedList();
                NewParam.Add("@nCompanyID",N_CompanyID);
                NewParam.Add("@nFormID",N_FormID);
                NewParam.Add("@nTransID",N_TransID);

                Count = dLayer.ExecuteScalar("select COUNT(N_HierarchyID) from Gen_ApprovalCodesTrans where N_CompanyID=@nCompanyID and N_FormID=@nFormID and N_TransID=@nTransID and (N_Status=0 or N_Status=-1)",NewParam,connection,transaction);
                if (Count != null)
                {
                    if (this.getIntVAL(Count.ToString()) == 0 )
                    {
                        string TableName = dLayer.ExecuteScalar("select X_ScreenTable from vw_ScreenTables where N_FormID=@nFormID",NewParam,connection,transaction).ToString();
                        string TableID = dLayer.ExecuteScalar("select X_IDName from vw_ScreenTables where N_FormID=@nFormID",NewParam,connection,transaction).ToString();
                        
                        dLayer.ExecuteScalar("update " + TableName + " set B_IssaveDraft=0 where " + TableID + "=@nTransID and N_CompanyID=@nCompanyID",NewParam,connection,transaction);
                    }
                }
            }
        }

        public void UpdateApproverEntry(DataTable Approvals,string ScreenTable, string Criterea,int N_TransID,int N_CompanyID,int N_UserID,IDataAccessLayer dLayer,SqlConnection connection,SqlTransaction transaction )
        {
            DataRow ApprovalRow = Approvals.Rows[0];
            int N_ApprovalLevelId = this.getIntVAL(ApprovalRow["nextApprovalLevel"].ToString());
            int N_ProcStatus = this.getIntVAL(ApprovalRow["saveTag"].ToString());
            int N_IsApprovalSystem = this.getIntVAL(ApprovalRow["isApprovalSystem"].ToString());
            int ApprovalID = this.getIntVAL(ApprovalRow["approvalID"].ToString());
            int FormID = this.getIntVAL(ApprovalRow["formID"].ToString());
            int N_MaxLevelID = 0, N_Submitter = 0;
            int N_ApprovalID = 0;
            N_ApprovalID = ApprovalID;

            SortedList Params = new SortedList();
            Params.Add("@nFormID",FormID);
            Params.Add("@nCompanyID",N_CompanyID);
            Params.Add("@nApprovalID",N_ApprovalID);
            Params.Add("@nTransID",N_TransID);
            Params.Add("@nProcStatus",N_ProcStatus);
            Params.Add("@nApprovalLevelId",N_ApprovalLevelId);
            Params.Add("@nUserID",N_UserID);

            if (N_ApprovalID == 0)
            {
                object ApprovalCode = dLayer.ExecuteScalar("Select N_ApprovalID from Sec_ApprovalSettings_General where N_FormID=@nFormID and N_CompanyID=@nCompanyID",Params,connection,transaction);
                if (ApprovalCode != null){
                    N_ApprovalID = this.getIntVAL(ApprovalCode.ToString());
                    Params["@nApprovalID"]=N_ApprovalID;
                    }
            }

            if (N_IsApprovalSystem == 1)
            {
                N_MaxLevelID = this.getIntVAL(dLayer.ExecuteScalar("select ISNULL(MAX(N_LevelID),0) from Gen_ApprovalCodesTrans where N_CompanyID=@nCompanyID and N_ApprovalID=@nApprovalID and N_FormID=@nFormID  and N_TransID=@nTransID",Params,connection,transaction).ToString());

                object OB_Submitter = dLayer.ExecuteScalar("select ISNULL(N_LevelID,0) from Gen_ApprovalCodesTrans where N_CompanyID=@nCompanyID and N_ApprovalID=@nApprovalID and N_FormID=@nFormID and N_TransID=@nTransID and N_ActionTypeId=111",Params,connection,transaction);
                if (OB_Submitter != null)
                    N_Submitter = this.getIntVAL(OB_Submitter.ToString());
                else
                    N_Submitter = N_MaxLevelID;

                if (N_ApprovalLevelId == N_MaxLevelID || N_ApprovalLevelId == N_Submitter)
                 Params.Add("@nSaveDraft",0);
                else
                 Params.Add("@nSaveDraft",1);


                dLayer.ExecuteNonQuery("UPDATE " + ScreenTable + " SET N_ProcStatus=@nProcStatus, N_ApprovalLevelId=@nApprovalLevelId,N_UserID=@nUserID,B_IssaveDraft=@nSaveDraft where " + Criterea,Params,connection,transaction);
            }
        }

    }

    

    public interface IMyFunctions
    {
        public bool CheckPermission(int N_CompanyID, int N_MenuID, string admin, IDataAccessLayer dLayer, SqlConnection connection);
        public bool CheckPermission(int N_CompanyID, int N_MenuID, string admin, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction);
        public int getIntVAL(string val);
        public double getVAL(string val);
        public double Round(double val, int RoundDigit);
        public double Round(string val, int RoundDigit);
        public string decimalPlaceString(int decimalNo);
        public decimal getDecimalVAL(double val);
        public decimal getDecimalVAL(string val);
        public float getFloatVAL(string val);
        public int getIntVAL(bool val);
        public bool getBoolVAL(string val);
        public string EncryptString(string inputString);
        public string DecryptString(string inputString);
        public bool checkIsNull(DataRow Row, String KeyName);
        public string checkProcessed(string TableName, string ColumnReturn, string ColumnValidate, string ValidateValue, string Condition, SortedList Params, IDataAccessLayer dLayer, SqlConnection Connection);
        public string ReturnSettings(string Group, string Description, string ValueColumn, string ConditionColumn, string Value, SortedList Params, IDataAccessLayer dLayer, SqlConnection Connection);
        public string ReturnSettings(string Group, string Description, string ValueColumn, string ConditionColumn, string Value, SortedList Params, IDataAccessLayer dLayer, SqlConnection Connection, SqlTransaction transaction);
        public string ReturnSettings(string Group, string Description, string ValueColumn, string ConditionColumn, string Value, int nCompanyID, IDataAccessLayer dLayer, SqlConnection Connection);
        public string ReturnSettings(string Group, string Description, string ValueColumn, int nCompanyID, IDataAccessLayer dLayer, SqlConnection Connection);

        public string ReturnSettings(string Group, string Description, string ValueColumn, string ConditionColumn, string Value, int nCompanyID, IDataAccessLayer dLayer, SqlConnection Connection, SqlTransaction transaction);
        public string ReturnValue(string TableName, string ColumnReturn, string Condition, SortedList Params, IDataAccessLayer dLayer, SqlConnection connection);
        public DataTable AddNewColumnToDataTable(DataTable MasterDt, string ColName, Type dataType, object Value);
        public string getDateVAL(DateTime val);
        public DateTime GetFormatedDate(string val);
        public DataTable saveApprovals(DataTable MasterTable,DataTable Approvals,IDataAccessLayer dLayer ,SqlConnection connection,SqlTransaction transaction);
        public void logApprovals(DataTable Approvals,int N_CompanyID,int N_FnYearID, string X_TransType, int N_TransID,string X_TransCode, DateTime D_TransDate, int N_ApprovalUserID, int N_ApprovalUserCatID,int GroupID,string PartyName,int EmpID,string DepLevel,IDataAccessLayer dLayer,SqlConnection connection,SqlTransaction transaction );
        public void UpdateApproverEntry(DataTable Approvals,string ScreenTable, string Criterea,int N_TransID,int N_CompanyID,int N_UserID,IDataAccessLayer dLayer,SqlConnection connection,SqlTransaction transaction );
        
    }
}