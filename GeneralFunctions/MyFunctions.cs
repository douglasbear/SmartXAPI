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

using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace SmartxAPI.GeneralFunctions
{
    public class MyFunctions : IMyFunctions
    {
        private readonly string ApprovalLink;
        private readonly string masterDBConnectionString;
        private readonly string connectionString;
        private readonly IConfiguration config;
        private readonly string TempFilesPath;
        private readonly string uploadedImagesPath;
        public MyFunctions(IConfiguration conf)
        {
            ApprovalLink = conf.GetConnectionString("AppURL");
            masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");
            connectionString = conf.GetConnectionString("SmartxConnection");
            config = conf;
            uploadedImagesPath = conf.GetConnectionString("UploadedImagesPath");
            TempFilesPath = conf.GetConnectionString("TempFilesPath");

        }

        public bool CheckPermission(int N_CompanyID, int N_MenuID, string admin, string FieldName, IDataAccessLayer dLayer, SqlConnection connection)
        {
            SortedList Params = new SortedList();
            Params.Add("@p1", N_CompanyID);
            Params.Add("@p2", N_MenuID);
            Params.Add("@p3", admin);
            bool Result = Convert.ToBoolean(dLayer.ExecuteScalar("Select ISNULL(B_Visible,0) From vw_userPrevileges Where N_CompanyID=@p1 and N_MenuID = @p2 and " + FieldName + "=@p3", Params, connection));
            return Result;
        }

        public bool CheckPermission(int N_CompanyID, int N_MenuID, string admin, string FieldName, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction)
        {
            SortedList Params = new SortedList();
            Params.Add("@p1", N_CompanyID);
            Params.Add("@p2", N_MenuID);
            Params.Add("@p3", admin);
            bool Result = Convert.ToBoolean(dLayer.ExecuteScalar("Select ISNULL(B_Visible,0) From vw_userPrevileges Where N_CompanyID=@p1 and N_MenuID = @p2 and  " + FieldName + "=@p3", Params, connection, transaction));
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
            if (val == null || val.Trim() == "")
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
            object obj = dLayer.ExecuteScalar("select " + ValueColumn + " from Gen_Settings where X_Group='" + Group + "' and X_Description='" + Description + "' and N_CompanyID=" + nCompanyID + " ", Connection);
            if (obj != null)
                Result = obj.ToString();
            return Result;
        }
        public string ReturnSettings(string Group, string Description, string ValueColumn, int nCompanyID, IDataAccessLayer dLayer, SqlConnection Connection, SqlTransaction transaction)
        {
            string Result = "";
            object obj = dLayer.ExecuteScalar("select " + ValueColumn + " from Gen_Settings where X_Group='" + Group + "' and X_Description='" + Description + "' and N_CompanyID=" + nCompanyID + " ", Connection, transaction);
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

        public DataTable GetSettingsTable()
        {
            DataTable QList = new DataTable();
            QList.Columns.Add(new DataColumn("X_Group", typeof(string)));
            QList.Columns.Add(new DataColumn("X_Description", typeof(string)));
            DataColumn NewCol = new DataColumn("N_UserCategoryID", typeof(int));
            NewCol.DefaultValue = 0;
            QList.Columns.Add(NewCol);
            return QList;
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

        public bool CheckClosedYear(int nCompanyID, int nFnYearID, IDataAccessLayer dLayer, SqlConnection connection)
        {
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);

            bool CheckClosedYear = Convert.ToBoolean(dLayer.ExecuteScalar("Select B_YearEndProcess From Acc_FnYear Where N_CompanyID=@nCompanyID and N_FnYearID = @nFnYearID", Params, connection));
            if (CheckClosedYear)
                return true;
            else
                return false;
        }

        public bool CheckActiveYearTransaction(int nCompanyID, int nFnYearID, DateTime dTransDate, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction)
        {
            DateTime dStart;
            DateTime dEnd;

            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);

            dStart = Convert.ToDateTime(dLayer.ExecuteScalar("select D_Start from Acc_FnYear where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", Params, connection, transaction));
            dEnd = Convert.ToDateTime(dLayer.ExecuteScalar("select D_End from Acc_FnYear where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", Params, connection, transaction));

            if (dTransDate < dStart || dTransDate > dEnd)
                return false;
            else
                return true;
        }

        public SortedList GetApprovals(int nIsApprovalSystem, int nFormID, int nTransID, int nTransUserID, int nTransStatus, int nTransApprovalLevel, int nNextApprovalLevel, int nApprovalID, int nGroupID, int nFnYearID, int nEmpID, int nActionID, ClaimsPrincipal User, IDataAccessLayer dLayer, SqlConnection connection)
        {
            DataTable SecUserLevel = new DataTable();
            DataTable GenStatus = new DataTable();
            int nMinLevel = 1, nMaxLevel = 0, nActionLevelID = 0, nSubmitter = 0;
            int nNextApprovalID = nTransApprovalLevel + 1;
            string xLastUserName = "", xEntryTime = "";
            int nTempStatusID = 0;

            int loggedInUserID = this.GetUserID(User);


            /* Approval Response Set */
            SortedList Response = new SortedList();
            Response.Add("btnSaveText", "Save");
            Response.Add("btnDeleteText", "Delete");
            Response.Add("saveEnabled", true);
            Response.Add("deleteEnabled", true);
            Response.Add("saveVisible", true);
            Response.Add("deleteVisible", true);
            Response.Add("saveTag", 0);
            Response.Add("deleteTag", 0);
            Response.Add("isApprovalSystem", nIsApprovalSystem);
            Response.Add("isEditable", true);
            Response.Add("nextApprovalLevel", nNextApprovalLevel);
            Response.Add("lblVisible", false);
            Response.Add("lblText", "");
            Response.Add("approvalID", nApprovalID);
            Response.Add("formID", nFormID);

            /* Approval Param Set */
            SortedList ApprovalParams = new SortedList();
            int nCompanyID = this.GetCompanyID(User);
            ApprovalParams.Add("@nCompanyID", nCompanyID);
            ApprovalParams.Add("@nFormID", nFormID);
            ApprovalParams.Add("@nTransID", nTransID);
            ApprovalParams.Add("@nApprovalID", nApprovalID);
            ApprovalParams.Add("@nNextApprovalID", nNextApprovalID);
            ApprovalParams.Add("@nTransUserID", nTransUserID);
            ApprovalParams.Add("@nTransApprovalLevel", nTransApprovalLevel);
            ApprovalParams.Add("@nTransStatus", nTransStatus);
            ApprovalParams.Add("@nGroupID", nGroupID);
            ApprovalParams.Add("@loggedInUserID", loggedInUserID);

            object objUserCategory = dLayer.ExecuteScalar("Select X_UserCategoryList from Sec_User where N_CompanyID=" + nCompanyID + " and N_UserID=" + loggedInUserID, ApprovalParams, connection);

            objUserCategory = objUserCategory != null ? objUserCategory : 0;


            if (nApprovalID == 0)
            {
                if (nEmpID.ToString() != null && nActionID.ToString() != null && nEmpID.ToString() != "" && nActionID.ToString() != "" && nEmpID != 0)
                {
                    SortedList EmpParams = new SortedList();
                    EmpParams.Add("@nCompanyID", nCompanyID);
                    EmpParams.Add("@nFnYearID", nFnYearID);
                    EmpParams.Add("@nEmpID", nEmpID);
                    EmpParams.Add("@nActionID", nActionID);
                    object objApproval = dLayer.ExecuteScalar("Select isnull(N_ApprovalID,0) as N_ApprovalID from vw_EmpApprovalSettings where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_EmpID=@nEmpID and N_ActionID=@nActionID", EmpParams, connection);
                    if (objApproval != null)
                    {
                        nApprovalID = this.getIntVAL(objApproval.ToString());
                        ApprovalParams["@nApprovalID"] = nApprovalID;
                    }
                    else
                    {
                        object ApprovalCode = dLayer.ExecuteScalar("Select N_ApprovalID from Sec_ApprovalSettings_General where N_FormID=@nFormID and N_CompanyID=@nCompanyID", ApprovalParams, connection);
                        if (ApprovalCode != null)
                        {
                            nApprovalID = this.getIntVAL(ApprovalCode.ToString());
                            ApprovalParams["@nApprovalID"] = nApprovalID;
                        }
                    }

                }
                else
                {
                    object ApprovalCode = dLayer.ExecuteScalar("Select N_ApprovalID from Sec_ApprovalSettings_General where N_FormID=@nFormID and N_CompanyID=@nCompanyID", ApprovalParams, connection);
                    if (ApprovalCode != null)
                    {
                        nApprovalID = this.getIntVAL(ApprovalCode.ToString());
                        ApprovalParams["@nApprovalID"] = nApprovalID;
                    }
                }
            }

            if (nApprovalID > 0)
            {
                nIsApprovalSystem = 1;
                Response["isApprovalSystem"] = nIsApprovalSystem;
            }

            if (nIsApprovalSystem == -1)
            {

                object res = dLayer.ExecuteScalar("Select COUNT(N_FormID) from Sec_ApprovalSettings_General Where N_FormID=@nFormID and N_CompanyID=@nCompanyID", ApprovalParams, connection);

                if (this.getIntVAL(res.ToString()) > 0)
                {
                    nIsApprovalSystem = 1;
                    Response["isApprovalSystem"] = nIsApprovalSystem;
                }
                else
                {
                    Response["btnSaveText"] = "Save";
                    Response["btnDeleteText"] = "Delete";
                    Response["saveEnabled"] = true;
                    if (nTransID == 0)
                    { Response["deleteEnabled"] = false; }
                    else
                    { Response["deleteEnabled"] = true; }

                    Response["saveTag"] = 0;
                    Response["deleteTag"] = 0;
                    Response["isApprovalSystem"] = 0;
                    Response["ApprovalID"] = nApprovalID;
                    Response["isEditable"] = true;
                    return Response;
                }
            }


            if (nIsApprovalSystem == 1)
            {
                nTempStatusID = nTransStatus;


                object MaxLevel = null;
                object ActionLevel = null;
                object OB_Submitter = null;
                if (nTransID == 0)
                {
                    MaxLevel = dLayer.ExecuteScalar("Select Isnull (MAX(N_level),0) from Gen_ApprovalCodesDetails where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID", ApprovalParams, connection);

                    if ((nTransApprovalLevel > nNextApprovalLevel) && nTransStatus != 4 && nTransStatus != 3)
                    {
                        ActionLevel = dLayer.ExecuteScalar("Select Isnull (N_ActionTypeId,0) from Gen_ApprovalCodesDetails where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_UserID=@loggedInUserID  and N_Level=( @nNextApprovalID + 1 )", ApprovalParams, connection);
                    }
                    else
                        ActionLevel = dLayer.ExecuteScalar("Select Isnull (N_ActionTypeId,0) from Gen_ApprovalCodesDetails where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_UserID=@loggedInUserID  and N_Level=@nNextApprovalID", ApprovalParams, connection);

                    OB_Submitter = dLayer.ExecuteScalar("Select isnull(N_level,0) as N_level from Gen_ApprovalCodesDetails where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_ActionTypeId=111", ApprovalParams, connection);
                }
                else
                {
                    MaxLevel = dLayer.ExecuteScalar("Select Isnull (MAX(N_LevelID),0) from Gen_ApprovalCodesTrans where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_FormID=@nFormID and N_TransID=@nTransID", ApprovalParams, connection);

                    if ((nTransApprovalLevel > nNextApprovalLevel) && nTransStatus != 4 && nTransStatus != 3)
                    {
                        ActionLevel = dLayer.ExecuteScalar("Select Isnull (N_ActionTypeID,0) from Gen_ApprovalCodesTrans where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_UserID=@loggedInUserID  and N_LevelID=( @nNextApprovalID + 1 ) and N_FormID=@nFormID and N_TransID=@nTransID", ApprovalParams, connection);
                    }
                    else
                        ActionLevel = dLayer.ExecuteScalar("Select Isnull (N_ActionTypeID,0) from Gen_ApprovalCodesTrans where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_UserID=@loggedInUserID  and N_LevelID=@nNextApprovalID and N_FormID=@nFormID and N_TransID=@nTransID", ApprovalParams, connection);

                    OB_Submitter = dLayer.ExecuteScalar("Select isnull(N_LevelID,0) as N_level from Gen_ApprovalCodesTrans where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_ActionTypeId=111 and N_FormID=@nFormID and N_TransID=@nTransID", ApprovalParams, connection);
                }



                if (MaxLevel != null)
                    nMaxLevel = this.getIntVAL(MaxLevel.ToString());

                if (ActionLevel != null)
                    nActionLevelID = this.getIntVAL(ActionLevel.ToString());


                if (OB_Submitter != null)
                    nSubmitter = this.getIntVAL(OB_Submitter.ToString());
                else
                    nSubmitter = nMaxLevel;

                object RewCheck = null;
                RewCheck = dLayer.ExecuteScalar("Select Isnull (N_ActionTypeID,0) from Gen_ApprovalCodesTrans where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID  and N_UserID=@loggedInUserID and N_FormID=@nFormID and N_TransID=@nTransID and N_ActionTypeID=110", ApprovalParams, connection);
                if (RewCheck != null)
                    nActionLevelID = this.getIntVAL(RewCheck.ToString());

                int nNextActionLevelID = 0;
                object NextRewChec = null;
                NextRewChec = dLayer.ExecuteScalar("Select Isnull (N_ActionTypeID,0) from Gen_ApprovalCodesTrans where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_FormID=@nFormID  and N_TransID=@nTransID and N_LevelID=@nNextApprovalID", ApprovalParams, connection);//+ " and N_ActionTypeID=110"
                if (NextRewChec != null)
                    nNextActionLevelID = this.getIntVAL(NextRewChec.ToString());

                object bIsEditable = false;
                bIsEditable = dLayer.ExecuteScalar("Select Isnull (B_IsEditable,0) from Gen_ApprovalCodesTrans where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_FormID=@nFormID  and N_TransID=@nTransID and N_UserID=@loggedInUserID", ApprovalParams, connection);//+ " and N_ActionTypeID=110"
                if (bIsEditable == null)
                    bIsEditable = false;


                if (nTransID > 0)
                {
                    if (nTransUserID == loggedInUserID)
                    {
                        if (nTransStatus != 3 && nTransStatus != 4)
                        {
                            if (nMinLevel == nTransApprovalLevel)
                            {
                                if (nNextActionLevelID == 327)
                                {
                                    nTransStatus = 926;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                                else
                                {
                                    nTransStatus = 911;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                            }
                            else
                            {

                                if (nNextActionLevelID == 327)
                                {
                                    nTransStatus = 927;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                                else
                                {
                                    nTransStatus = 912;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                            }

                            if (nActionLevelID == 327)
                            {
                                if (nNextActionLevelID == 327)
                                {
                                    nTransStatus = 928;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                                else
                                {
                                    nTransStatus = 925;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                            }
                        }
                        else if (nTransStatus == 3)
                        {
                            nTransStatus = 918;
                            ApprovalParams["@nTransStatus"] = nTransStatus;
                        }
                        else if (nTransStatus == 4)
                        {
                            nTransStatus = 919;
                            ApprovalParams["@nTransStatus"] = nTransStatus;
                        }
                        nNextApprovalLevel = nTransApprovalLevel;
                        Response["nextApprovalLevel"] = nNextApprovalLevel;
                    }
                    else
                    {

                        SecUserLevel = dLayer.ExecuteDataTable("SELECT TOP (1) N_FormID, N_TransID, Approved_User, N_ApprovalLevelId,D_ApprovedDate, N_CompanyID, N_FnYearID, N_ProcStatus, N_UserID, N_IssaveDraft, N_NextApprovalLevelId, X_Status, N_CurrentApprover, N_NextApproverID FROM vw_ApprovalDashBoard WHERE (N_FormID = @nFormID ) AND (N_CompanyID = @nCompanyID) AND (N_TransID = @nTransID ) AND (N_NextApproverID = @loggedInUserID )", ApprovalParams, connection);
                        if (SecUserLevel.Rows.Count > 0)
                        {
                            DataRow Drow = SecUserLevel.Rows[0];
                            xLastUserName = Drow["Approved_User"].ToString();
                            xEntryTime = Drow["D_ApprovedDate"].ToString();
                            nTransStatus = this.getIntVAL(Drow["N_ProcStatus"].ToString());
                            ApprovalParams["@nTransStatus"] = nTransStatus;
                            //nTransStatus = 912;`
                            nNextApprovalLevel = this.getIntVAL(Drow["N_NextApprovalLevelId"].ToString());
                            Response["nextApprovalLevel"] = nNextApprovalLevel;
                            if (nTransStatus == 8)
                            {
                                nTransStatus = 924;
                                ApprovalParams["@nTransStatus"] = nTransStatus;
                            }
                            if (nActionLevelID == 110)
                            {
                                if (((nMaxLevel == nTransApprovalLevel || nSubmitter == nTransApprovalLevel)) && nTransStatus != 3)
                                {
                                    nTransStatus = 921;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                                //else
                                //    nTransStatus = 7;
                            }
                            else if (nActionLevelID == 327)
                            {
                                if (nTransStatus == 924)
                                {
                                    nTransStatus = 930;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                                else
                                {
                                    nTransStatus = 8;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                            }
                            if (nTransStatus == 3)
                            {
                                if (nMinLevel == nNextApprovalLevel)
                                {
                                    nTransStatus = 923;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                                else
                                {
                                    nTransStatus = 917;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                            }
                            if (nTransStatus == 4)
                            {
                                nTransStatus = 920;
                                ApprovalParams["@nTransStatus"] = nTransStatus;
                                nNextApprovalLevel = nNextApprovalLevel - 1;
                                Response["nextApprovalLevel"] = nNextApprovalLevel;
                            }
                        }
                        else
                        {
                            if (nTransStatus != 3 && nTransStatus != 4)
                            {
                                if (nNextActionLevelID == 327)
                                {
                                    nTransStatus = 929;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                                else
                                {
                                    nTransStatus = 913;
                                    ApprovalParams["@nTransStatus"] = nTransStatus;
                                }
                            }
                            else if (nTransStatus == 3)
                            {
                                nTransStatus = 918;
                                ApprovalParams["@nTransStatus"] = nTransStatus;
                            }
                            else if (nTransStatus == 4)
                            {
                                nTransStatus = 920;
                                ApprovalParams["@nTransStatus"] = nTransStatus;
                            }
                        }
                    }
                }
                else if (nTransID == 0)
                {
                    nActionLevelID = 0;

                    int nUsrCatID = 0;
                    object UsrCatID = null;
                    UsrCatID = dLayer.ExecuteScalar("Select N_UserCategoryID from Gen_ApprovalCodesDetails Where N_CompanyID=@nCompanyID  and N_ApprovalID=@nApprovalID and N_level=1 ", ApprovalParams, connection);
                    if (UsrCatID != null)
                        nUsrCatID = this.getIntVAL(UsrCatID.ToString());

                    if (nUsrCatID == 0)
                        SecUserLevel = dLayer.ExecuteDataTable("Select N_UserID from Gen_ApprovalCodesDetails Where N_CompanyID=@nCompanyID and N_ApprovalID=@nApprovalID and N_level=1 and (N_UserID in (-11,@loggedInUserID ))", ApprovalParams, connection);
                    else
                        SecUserLevel = dLayer.ExecuteDataTable("Select N_UserID from Gen_ApprovalCodesDetails Where N_CompanyID=@nCompanyID and N_ApprovalID=@nApprovalID and N_level=1 and (N_UserID in (-11,@loggedInUserID ))  and N_UserCategoryID in (" + objUserCategory + ")", ApprovalParams, connection);


                    if (SecUserLevel.Rows.Count > 0)
                    {
                        nNextApprovalLevel = 1;
                        Response["nextApprovalLevel"] = nNextApprovalLevel;
                        nTransStatus = 901;
                        ApprovalParams["@nTransStatus"] = nTransStatus;
                    }
                    else
                    {
                        nTransStatus = 900;
                        ApprovalParams["@nTransStatus"] = nTransStatus;
                    }
                }



                object NextApprovalUser = null;

                if (xLastUserName.Trim() == "" && nTransID > 0)
                {
                    if ((nMaxLevel == nTransApprovalLevel || nSubmitter == nTransApprovalLevel) && nTransUserID == loggedInUserID)
                    {
                        if (nMaxLevel == nMinLevel || nSubmitter == nMinLevel)
                        {
                            nTransStatus = 922;
                            ApprovalParams["@nTransStatus"] = nTransStatus;
                        }
                        else if (nTransStatus != 3 && nTransStatus != 918 && nTransStatus != 4 && nTransStatus != 919)
                        {
                            nTransStatus = 914;
                            ApprovalParams["@nTransStatus"] = nTransStatus;
                        }
                        NextApprovalUser = dLayer.ExecuteScalar("SELECT X_UserName FROM Sec_User Where N_UserID=@nTransUserID  and N_CompanyID=@nCompanyID", ApprovalParams, connection);
                        if (NextApprovalUser != null)
                            xLastUserName = NextApprovalUser.ToString();
                    }
                    else if ((nMaxLevel == nTransApprovalLevel || nSubmitter == nTransApprovalLevel) && nTransUserID != loggedInUserID)
                    {
                        if (nTransStatus != 918 && nTransStatus != 919 && nTransStatus != 920 && nTransStatus != 929)
                        {
                            if (nTransStatus == 913 || nTransStatus == 7)
                            {
                                nTransStatus = 916;
                                ApprovalParams["@nTransStatus"] = nTransStatus;
                            }
                            else
                            {
                                nTransStatus = 915;
                                ApprovalParams["@nTransStatus"] = nTransStatus;
                            }
                        }
                        NextApprovalUser = dLayer.ExecuteScalar("SELECT X_UserName FROM Sec_User Where N_UserID=@nTransUserID  and N_CompanyID=@nCompanyID", ApprovalParams, connection);
                        if (NextApprovalUser != null)
                            xLastUserName = NextApprovalUser.ToString();
                    }
                    else
                    {
                        if (nTransStatus == 3 || nTransStatus == 918 || nTransStatus == 917 || nTransStatus == 4 || nTransStatus == 919)
                        {
                            NextApprovalUser = dLayer.ExecuteScalar("SELECT X_UserName FROM Sec_User Where N_UserID=@nTransUserID  and N_CompanyID=@nCompanyID", ApprovalParams, connection);
                            if (NextApprovalUser != null)
                                xLastUserName = NextApprovalUser.ToString();
                        }
                        else
                        {
                            NextApprovalUser = dLayer.ExecuteScalar("SELECT Sec_User.X_UserName FROM vw_ApprovalDashBoard INNER JOIN Sec_User ON vw_ApprovalDashBoard.N_NextApproverId = Sec_User.N_UserID AND vw_ApprovalDashBoard.N_CompanyID = Sec_User.N_CompanyID WHERE vw_ApprovalDashBoard.N_CompanyID =@nCompanyID AND vw_ApprovalDashBoard.N_TransID = @nTransID AND vw_ApprovalDashBoard.N_ApprovalLevelId =@nTransApprovalLevel AND vw_ApprovalDashBoard.N_FormID=@nFormID", ApprovalParams, connection);
                            if (NextApprovalUser != null)
                                xLastUserName = NextApprovalUser.ToString();
                        }

                    }
                }
                //}
                GenStatus = dLayer.ExecuteDataTable("SELECT * FROM vw_GenApproval_Status WHERE N_CompanyId=@nCompanyID and N_StatusId=@nTransStatus and N_GroupID=@nGroupID", ApprovalParams, connection);
                if (GenStatus.Rows.Count == 1)
                {
                    DataRow Status = GenStatus.Rows[0];
                    if (Status["X_NoStatusCaption"].ToString().Trim() == "")
                    {
                        Response["btnDeleteText"] = "Delete";
                        Response["deleteVisible"] = true;
                        Response["deleteEnabled"] = false;

                    }
                    else
                    {
                        Response["btnDeleteText"] = Status["X_NoStatusCaption"].ToString();
                        Response["deleteVisible"] = true;
                        Response["deleteEnabled"] = true;
                        Response["deleteTag"] = Status["N_NoAction"].ToString();

                    }
                    if (Status["X_YesStatusCaption"].ToString().Trim() == "")
                    {
                        Response["btnSaveText"] = "Save";
                        Response["saveVisible"] = true;
                        Response["saveEnabled"] = false;
                    }
                    else
                    {
                        Response["btnSaveText"] = Status["X_YesStatusCaption"].ToString();
                        Response["saveEnabled"] = true;
                        Response["saveTag"] = Status["N_YesAction"].ToString();

                    }
                    if (nActionLevelID == 110)
                    {
                        Response["saveEnabled"] = false;
                        Response["deleteEnabled"] = true;
                        Response["btnDeleteText"] = "Review";
                        Response["deleteTag"] = 7;
                    }

                    if (Status != null)
                    {
                        Response["lblVisible"] = true;
                        Response["lblText"] = Status["X_MsgStatus"].ToString();
                        Response["lblText"] = Response["lblText"].ToString().Replace("#NAME", xLastUserName);
                        if (xEntryTime.Trim() != "")
                            xEntryTime = Convert.ToDateTime(xEntryTime).ToString("dd/MM/yyyy HH:mm");
                        Response["lblText"] = Response["lblText"].ToString().Replace("#DATE", xEntryTime);
                    }

                }

                //Blocking edit control of Approvers
                if (this.getBoolVAL(bIsEditable.ToString()) || nNextApprovalLevel == 1)
                {
                    Response["isEditable"] = true;
                }
                else
                {
                    Response["isEditable"] = false;
                }
            }
            Response["ApprovalID"] = nApprovalID;
            return Response;
        }



        public DataTable SaveApprovals(DataTable MasterTable, DataTable Approvals, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction)
        {
            DataRow MasterRow = MasterTable.Rows[0];
            DataRow ApprovalRow = Approvals.Rows[0];

            int N_MaxLevelID = 0, N_Submitter = 0;
            int N_ApprovalID = 0, N_IsApprovalSystem = 0, FormID = 0, N_CompanyID = 0, N_userLevel = 0, N_SaveDraft = 0, N_ProcStatus = 0;

            N_ApprovalID = this.getIntVAL(ApprovalRow["approvalID"].ToString());
            N_IsApprovalSystem = this.getIntVAL(ApprovalRow["isApprovalSystem"].ToString());
            FormID = this.getIntVAL(ApprovalRow["formID"].ToString());
            N_ProcStatus = this.getIntVAL(ApprovalRow["saveTag"].ToString());
            N_CompanyID = this.getIntVAL(MasterRow["N_CompanyID"].ToString());
            N_userLevel = this.getIntVAL(ApprovalRow["nextApprovalLevel"].ToString());

            SortedList Params = new SortedList();
            Params.Add("@FormID", FormID);
            Params.Add("@nCompanyID", N_CompanyID);
            Params.Add("@nApprovalID", N_ApprovalID);

            if (N_ApprovalID == 0)
            {
                object ApprovalCode = dLayer.ExecuteScalar("Select N_ApprovalID from Sec_ApprovalSettings_General where N_FormID=@FormID and N_CompanyID=@nCompanyID", Params, connection, transaction);
                if (ApprovalCode != null)
                    Params["@nApprovalID"] = this.getIntVAL(ApprovalCode.ToString());
            }

            if (N_IsApprovalSystem == 1)
            {
                N_MaxLevelID = this.getIntVAL(dLayer.ExecuteScalar("Select Isnull (max(N_level),0) from Gen_ApprovalCodesDetails where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID", Params, connection, transaction).ToString());

                object OB_Submitter = dLayer.ExecuteScalar("Select isnull(N_level,0) as N_level from Gen_ApprovalCodesDetails where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID and N_ActionTypeId=111", Params, connection, transaction);
                if (OB_Submitter != null)
                    N_Submitter = this.getIntVAL(OB_Submitter.ToString());
                else
                    N_Submitter = N_MaxLevelID;

                if (N_userLevel == N_MaxLevelID || N_userLevel == N_Submitter)
                    N_SaveDraft = 0;
                else
                    N_SaveDraft = 1;
                if (MasterTable.Columns.Contains("N_ApprovalLevelId"))
                {
                    MasterTable.Rows[0]["N_ApprovalLevelId"] = N_userLevel;
                }
                else
                {
                    MasterTable = this.AddNewColumnToDataTable(MasterTable, "N_ApprovalLevelId", typeof(int), N_userLevel);
                }

                if (MasterTable.Columns.Contains("N_ProcStatus"))
                {
                    MasterTable.Rows[0]["N_ProcStatus"] = N_ProcStatus;
                }
                else
                {
                    MasterTable = this.AddNewColumnToDataTable(MasterTable, "N_ProcStatus", typeof(int), N_ProcStatus);
                }

                if (MasterTable.Columns.Contains("B_IssaveDraft"))
                {
                    MasterTable.Rows[0]["B_IssaveDraft"] = N_SaveDraft;
                }
                else
                {
                    MasterTable = this.AddNewColumnToDataTable(MasterTable, "B_IssaveDraft", typeof(int), N_SaveDraft);
                }
            }
            else if (N_IsApprovalSystem == 0)
            {
                MasterTable = this.AddNewColumnToDataTable(MasterTable, "B_IssaveDraft", typeof(int), 0);
            }
            return MasterTable;
        }
        public bool SendApprovalMail(int N_NextApproverID, int FormID, int TransID, string TransType, string TransCode, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction, ClaimsPrincipal User)
        {
            try
            {
                int companyid = GetCompanyID(User);
                int nUserID = GetUserID(User);
                SortedList Params = new SortedList();
                string Toemail = "";
                object Email = dLayer.ExecuteScalar("select ISNULL(X_Email,'') from vw_UserEmp where N_CompanyID=" + companyid + " and N_UserID=" + N_NextApproverID + " order by n_fnyearid desc", Params, connection, transaction);
                Toemail = Email.ToString();
                object CurrentStatus = dLayer.ExecuteScalar("select ISNULL(X_CurrentStatus,'') from vw_ApprovalPending where N_FormID=" + FormID + " and X_TransCode='" + TransCode + "' and N_TransID=" + TransID + " and X_Type='" + TransType + "'", Params, connection, transaction);
                object EmployeeName = dLayer.ExecuteScalar("select x_empname from vw_UserDetails where N_UserID=" + nUserID + " and N_CompanyID=" + companyid, Params, connection, transaction);
                object companyemail = "";
                object companypassword = "";

                companyemail = dLayer.ExecuteScalar("select X_Value from Gen_Settings where X_Group='210' and X_Description='EmailAddress' and N_CompanyID=" + companyid, Params, connection, transaction);
                companypassword = dLayer.ExecuteScalar("select X_Value from Gen_Settings where X_Group='210' and X_Description='EmailPassword' and N_CompanyID=" + companyid, Params, connection, transaction);
                if (Toemail.ToString() != "")
                {
                    if (companyemail.ToString() != "")
                    {
                        object body = null;
                        string MailBody;
                        body = "Greetings," + "<br/><br/>" + EmployeeName + " has requested for your approval on " + TransType + ". To approve or reject this request, please click on the following link<br/>" + ApprovalLink;
                        if (body != null)
                        {
                            body = body.ToString();
                        }
                        else
                            body = "";


                        string Sender = companyemail.ToString();
                        MailBody = body.ToString();
                        string Subject = "Request for Approval";



                        SmtpClient client = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            Credentials = new System.Net.NetworkCredential(companyemail.ToString(), companypassword.ToString()),
                            Timeout = 10000,
                        };

                        MailMessage message = new MailMessage();
                        message.To.Add(Toemail.ToString()); // Add Receiver mail Address  
                        message.From = new MailAddress(Sender);
                        message.Subject = Subject;
                        message.Body = MailBody;

                        message.IsBodyHtml = true; //HTML email  
                        string CC = GetCCMail(256, companyid, connection, transaction, dLayer);
                        if (CC != "")
                            message.CC.Add(CC);

                        string Bcc = GetBCCMail(256, companyid, connection, transaction, dLayer);
                        if (Bcc != "")
                            message.Bcc.Add(Bcc);
                        client.Send(message);

                    }
                }
                return true;

            }

            catch (Exception ie)
            {
                return false;
            }
        }
        public static string GetCCMail(int ID, int nCompanyID, SqlConnection connection, SqlTransaction transaction, IDataAccessLayer dLayer)
        {
            SortedList Params = new SortedList();
            object CCMail = dLayer.ExecuteScalar("select X_CCMail from Gen_EmailAddresses where N_subjectID =" + ID + " and N_CompanyID=" + nCompanyID, Params, connection, transaction);
            if (CCMail != null)
                return CCMail.ToString();
            else
                return "";
        }
        public static string GetBCCMail(int ID, int nCompanyID, SqlConnection connection, SqlTransaction transaction, IDataAccessLayer dLayer)
        {
            SortedList Params = new SortedList();
            object BCCMail = dLayer.ExecuteScalar("select X_BCCMail from Gen_EmailAddresses where N_subjectID =" + ID + " and N_CompanyID=" + nCompanyID, Params, connection, transaction);
            if (BCCMail != null)
                return BCCMail.ToString();
            else
                return "";
        }

        public bool SendMail(string ToMail, string Body, string Subjectval, IDataAccessLayer dLayer, int FormID, int ReferID, int CompanyID)
        {

            try
            {
                ToMail = ToMail.ToString();
                object companyemail = "";
                object companypassword = "";
                SortedList Params = new SortedList();
                DataTable Attachments;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Attachments = dLayer.ExecuteDataTable("select * from Dms_ScreenAttachments where N_CompanyID=" + CompanyID + " and n_formid=" + FormID + " and N_TransID=" + ReferID, Params, connection);

                }

                // companyemail = dLayer.ExecuteScalar("select X_Value from Gen_Settings where X_Group='210' and X_Description='EmailAddress' and N_CompanyID=" + companyid, connection, transaction);
                // companypassword = dLayer.ExecuteScalar("select X_Value from Gen_Settings where X_Group='210' and X_Description='EmailPassword' and N_CompanyID=" + companyid, connection, transaction);
                using (SqlConnection olivCnn = new SqlConnection(masterDBConnectionString))
                {
                    olivCnn.Open();
                    companyemail = dLayer.ExecuteScalar("select X_Value from GenSettings where N_ClientID=-1 and X_Description='OlivoEmailAddress'", olivCnn);
                    companypassword = dLayer.ExecuteScalar("select X_Value from GenSettings where N_ClientID=-1 and X_Description='OlivoEmailPassword'", olivCnn);

                }
                if (ToMail.ToString() != "")
                {
                    if (companyemail.ToString() != "")
                    {
                        object body = null;
                        string MailBody;
                        body = Body;
                        if (body != null)
                        {
                            body = body.ToString();
                        }
                        else
                            body = "";


                        string Sender = companyemail.ToString();
                        MailBody = body.ToString();
                        string Subject = Subjectval;



                        SmtpClient client = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            Credentials = new System.Net.NetworkCredential(companyemail.ToString(), companypassword.ToString()),
                            Timeout = 10000,
                        };

                        MailMessage message = new MailMessage();
                        message.To.Add(ToMail.ToString()); // Add Receiver mail Address  
                        message.From = new MailAddress(Sender);
                        message.Subject = Subject;
                        message.Body = MailBody;

                        message.IsBodyHtml = true; //HTML email  
                        if (Attachments.Rows.Count > 0)
                        {
                            foreach (DataRow var in Attachments.Rows)
                            {
                                message.Attachments.Add(new Attachment(var["x_refName"].ToString()));

                            }
                        }
                        // string CC = GetCCMail(256, companyid, connection, transaction, dLayer);
                        // if (CC != "")
                        //     message.CC.Add(CC);

                        // string Bcc = GetBCCMail(256, companyid, connection, transaction, dLayer);
                        // if (Bcc != "")
                        //     message.Bcc.Add(Bcc);

                        client.Send(message);

                    }
                }
                return true;
            }

            catch (Exception ie)
            {
                return false;
            }
        }

        public int LogApprovals(DataTable Approvals, int N_FnYearID, string X_TransType, int N_TransID, string X_TransCode, int GroupID, string PartyName, int EmpID, string DepLevel, ClaimsPrincipal User, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction)
        {
            DataRow ApprovalRow = Approvals.Rows[0];
            string X_Action = ApprovalRow["btnSaveText"].ToString();
            int N_ApprovalLevelID = this.getIntVAL(ApprovalRow["nextApprovalLevel"].ToString());
            int N_ProcStatusID = this.getIntVAL(ApprovalRow["saveTag"].ToString());
            int N_IsApprovalSystem = this.getIntVAL(ApprovalRow["isApprovalSystem"].ToString());
            int N_ApprovalID = this.getIntVAL(ApprovalRow["approvalID"].ToString());
            int N_FormID = this.getIntVAL(ApprovalRow["formID"].ToString());
            string Comments = "";
            DataColumnCollection columns = Approvals.Columns;
            if (columns.Contains("comments"))
            {
                Comments = ApprovalRow["comments"].ToString();
            }
            if (Comments == null)
            {
                Comments = "";
            }

            int N_GroupID = 1, N_NxtUserID = 0;
            N_GroupID = GroupID;
            int N_CompanyID = this.GetCompanyID(User);
            int N_ApprovalUserID = this.GetUserID(User);
            int N_ApprovalUserCatID = this.GetUserCategory(User);

            SortedList LogParams = new SortedList();
            LogParams.Add("@nCompanyID", N_CompanyID);
            LogParams.Add("@nFormID", N_FormID);
            LogParams.Add("@nApprovalUserID", N_ApprovalUserID);
            LogParams.Add("@nTransID", N_TransID);
            LogParams.Add("@nApprovalLevelID", N_ApprovalLevelID);
            LogParams.Add("@nProcStatusID", N_ProcStatusID);
            LogParams.Add("@nApprovalID", N_ApprovalID);
            LogParams.Add("@nGroupID", N_GroupID);
            LogParams.Add("@nFnYearID", N_FnYearID);
            LogParams.Add("@xAction", X_Action);
            LogParams.Add("@nEmpID", EmpID);
            LogParams.Add("@xDepLevel", DepLevel);
            LogParams.Add("@dTransDate", DateTime.Now.ToString("dd/MMM/yyyy"));

            if (N_IsApprovalSystem == 1)
            {
                dLayer.ExecuteNonQuery("SP_Gen_ApprovalCodesTrans @nCompanyID,@nFormID,@nApprovalUserID,@nTransID,@nApprovalLevelID,@nProcStatusID,@nApprovalID,@nGroupID,@nFnYearID,@xAction,@nEmpID,@xDepLevel,@dTransDate,0,0", LogParams, connection, transaction);

                object NxtUser = null;
                NxtUser = dLayer.ExecuteScalar("select N_UserID from Gen_ApprovalCodesTrans where N_CompanyID=@nCompanyID and N_FormID=@nFormID and N_TransID=@nTransID and N_Status=0", LogParams, connection, transaction);
                if (NxtUser != null)
                    N_NxtUserID = this.getIntVAL(NxtUser.ToString());

                LogParams.Add("@xTransType", X_TransType);
                LogParams.Add("@nApprovalUserCatID", N_ApprovalUserCatID);
                LogParams.Add("@xSystemName", "WebRequest");
                LogParams.Add("@xTransCode", X_TransCode);
                LogParams.Add("@xComments", Comments);
                LogParams.Add("@xPartyName", PartyName);
                LogParams.Add("@nNxtUserID", N_NxtUserID);
                dLayer.ExecuteNonQuery("SP_Log_Approval_Status @nCompanyID,@nFnYearID,@xTransType,@nTransID,@nFormID,@nApprovalUserID,@nApprovalUserCatID,@xAction,@xSystemName,@xTransCode,@dTransDate,@nApprovalLevelID,@nApprovalUserID,@nProcStatusID,@xComments,@xPartyName,@nNxtUserID", LogParams, connection, transaction);

                object Count = null;
                SortedList NewParam = new SortedList();
                NewParam.Add("@nCompanyID", N_CompanyID);
                NewParam.Add("@nFormID", N_FormID);
                NewParam.Add("@nTransID", N_TransID);

                Count = dLayer.ExecuteScalar("select COUNT(N_HierarchyID) from Gen_ApprovalCodesTrans where N_CompanyID=@nCompanyID and N_FormID=@nFormID and N_TransID=@nTransID and (N_Status=0 or N_Status=-1)", NewParam, connection, transaction);
                if (Count != null)
                {
                    if (this.getIntVAL(Count.ToString()) == 0)
                    {
                        string TableName = dLayer.ExecuteScalar("select X_ScreenTable from vw_ScreenTables where N_FormID=@nFormID", NewParam, connection, transaction).ToString();
                        string TableID = dLayer.ExecuteScalar("select X_IDName from vw_ScreenTables where N_FormID=@nFormID", NewParam, connection, transaction).ToString();

                        dLayer.ExecuteScalar("update " + TableName + " set B_IssaveDraft=0 where " + TableID + "=@nTransID and N_CompanyID=@nCompanyID", NewParam, connection, transaction);
                    }
                }
            }
            return N_NxtUserID;
        }

        public void UpdateApproverEntry(DataTable Approvals, string ScreenTable, string Criterea, int N_TransID, ClaimsPrincipal User, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction)
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
            int N_CompanyID = this.GetCompanyID(User);
            int N_UserID = this.GetUserID(User);

            SortedList Params = new SortedList();
            Params.Add("@nFormID", FormID);
            Params.Add("@nCompanyID", N_CompanyID);
            Params.Add("@nApprovalID", N_ApprovalID);
            Params.Add("@nTransID", N_TransID);
            Params.Add("@nProcStatus", N_ProcStatus);
            Params.Add("@nApprovalLevelId", N_ApprovalLevelId);
            Params.Add("@nUserID", N_UserID);

            if (N_ApprovalID == 0)
            {
                object ApprovalCode = dLayer.ExecuteScalar("Select N_ApprovalID from Sec_ApprovalSettings_General where N_FormID=@nFormID and N_CompanyID=@nCompanyID", Params, connection, transaction);
                if (ApprovalCode != null)
                {
                    N_ApprovalID = this.getIntVAL(ApprovalCode.ToString());
                    Params["@nApprovalID"] = N_ApprovalID;
                }
            }

            if (N_IsApprovalSystem == 1)
            {
                N_MaxLevelID = this.getIntVAL(dLayer.ExecuteScalar("select ISNULL(MAX(N_LevelID),0) from Gen_ApprovalCodesTrans where N_CompanyID=@nCompanyID and N_ApprovalID=@nApprovalID and N_FormID=@nFormID  and N_TransID=@nTransID", Params, connection, transaction).ToString());

                object OB_Submitter = dLayer.ExecuteScalar("select ISNULL(N_LevelID,0) from Gen_ApprovalCodesTrans where N_CompanyID=@nCompanyID and N_ApprovalID=@nApprovalID and N_FormID=@nFormID and N_TransID=@nTransID and N_ActionTypeId=111", Params, connection, transaction);
                if (OB_Submitter != null)
                    N_Submitter = this.getIntVAL(OB_Submitter.ToString());
                else
                    N_Submitter = N_MaxLevelID;

                if (N_ApprovalLevelId == N_MaxLevelID || N_ApprovalLevelId == N_Submitter)
                    Params.Add("@nSaveDraft", 0);
                else
                    Params.Add("@nSaveDraft", 1);


                dLayer.ExecuteNonQuery("UPDATE " + ScreenTable + " SET N_ProcStatus=@nProcStatus, N_ApprovalLevelId=@nApprovalLevelId,N_UserID=@nUserID,B_IssaveDraft=@nSaveDraft where " + Criterea, Params, connection, transaction);
            }
        }


        public string UpdateApprovals(DataTable Approvals, int N_FnYearID, string X_TransType, int N_TransID, string X_TransCode, int N_ProcStatusID, string X_ScreenTable, string X_Criteria, string PartyName, ClaimsPrincipal User, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction)
        {
            DataRow ApprovalRow = Approvals.Rows[0];
            int N_ApprovalLevelID = this.getIntVAL(ApprovalRow["nextApprovalLevel"].ToString());
            string ButtonTag = ApprovalRow["deleteTag"].ToString();
            int N_IsApprovalSystem = this.getIntVAL(ApprovalRow["isApprovalSystem"].ToString());
            int ApprovalID = this.getIntVAL(ApprovalRow["approvalID"].ToString());
            int FormID = this.getIntVAL(ApprovalRow["formID"].ToString());
            string X_Action = "";
            string X_Message = "Error";
            bool B_IsDelete = false;
            int N_MaxLevelID = 0;
            int N_ApprovalID = 0;
            N_ApprovalID = ApprovalID;
            int N_CompanyID = this.GetCompanyID(User);
            int N_ApprovalUserID = this.GetUserID(User);

            SortedList UpdateParams = new SortedList();
            UpdateParams.Add("@nFormID", FormID);
            UpdateParams.Add("@nCompanyID", N_CompanyID);
            UpdateParams.Add("@xButtonTag", ButtonTag);
            UpdateParams.Add("@nApprovalID", N_ApprovalID);
            UpdateParams.Add("@nApprovalLevelID", N_ApprovalLevelID);
            UpdateParams.Add("@nApprovalUserID", N_ApprovalUserID);



            if (N_ApprovalID == 0)
            {
                object ApprovalCode = dLayer.ExecuteScalar("Select N_ApprovalID from Sec_ApprovalSettings_General where N_FormID=@nFormID and N_CompanyID=@nCompanyID", UpdateParams, connection, transaction);
                if (ApprovalCode != null)
                    N_ApprovalID = this.getIntVAL(ApprovalCode.ToString());
            }
            N_MaxLevelID = this.getIntVAL(dLayer.ExecuteScalar("Select Isnull (max(N_level),0) from Gen_ApprovalCodesDetails where N_ApprovalID=@nApprovalID and N_CompanyID=@nCompanyID", UpdateParams, connection, transaction).ToString());

            if (ButtonTag == "3")
            {
                if (N_ProcStatusID == 4)
                {
                    dLayer.ExecuteNonQuery("UPDATE " + X_ScreenTable + " SET N_ProcStatus=@xButtonTag, N_ApprovalLevelId= (@nApprovalLevelID - 2),N_UserID=@nApprovalUserID,B_IssaveDraft=1 where " + X_Criteria, UpdateParams, connection, transaction);
                    N_ApprovalLevelID = N_ApprovalLevelID - 1;
                    UpdateParams["@nApprovalLevelID"] = N_ApprovalLevelID;
                }
                else
                    dLayer.ExecuteNonQuery("UPDATE " + X_ScreenTable + " SET N_ProcStatus=@xButtonTag, N_ApprovalLevelId=(@nApprovalLevelID - 1),N_UserID=@nApprovalUserID,B_IssaveDraft=1 where " + X_Criteria, UpdateParams, connection, transaction);
                X_Action = "Reject";
                B_IsDelete = false;
                X_Message = "Rejected";
            }
            else if (ButtonTag == "4")
            {
                dLayer.ExecuteNonQuery("UPDATE " + X_ScreenTable + " SET N_ApprovalLevelId=@nApprovalLevelID,N_ProcStatus=@xButtonTag,N_UserID=@nApprovalUserID,B_IssaveDraft=1 where " + X_Criteria, UpdateParams, connection, transaction);
                X_Action = "Revoke";
                B_IsDelete = false;
                X_Message = "Revoked";
            }
            else if (ButtonTag == "7")
            {
                //dLayer.ExecuteNonQuery("UPDATE " + X_ScreenTable + " SET N_ApprovalLevelId=@nApprovalLevelID,N_ProcStatus=@xButtonTag,N_UserID=@nApprovalUserID,B_IssaveDraft=1 where " + X_Criteria, UpdateParams, connection, transaction);
                X_Action = "Review";
                B_IsDelete = false;
                X_Message = "Reviewed";
            }
            else if (ButtonTag == "6" || ButtonTag == "0")
            {
                SortedList DeleteParams = new SortedList();
                DeleteParams.Add("@xTransType", X_TransType);
                DeleteParams.Add("@nCompanyID", N_CompanyID);
                DeleteParams.Add("@nTransID", N_TransID);
                DeleteParams.Add("@nFnYearID", N_FnYearID);
                DeleteParams.Add("@xTransCode", X_TransCode);

                SortedList DeleteParamsPro = new SortedList() { { "N_CompanyID", N_CompanyID }, { "X_TransType", X_TransType }, { "N_VoucherID", N_TransID } };

                X_Action = "Delete";
                X_Message = "Deleted";
                int DeleteStatus = 0;
                switch (FormID)
                {
                    case 82://PO
                        DeleteStatus = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParamsPro, connection, transaction);
                        X_Action = "Delete";
                        B_IsDelete = true;
                        break;
                    case 212://loan issue
                        DeleteStatus = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParamsPro, connection, transaction);
                        X_Action = "Delete";
                        B_IsDelete = true;
                        break;
                    case 198://Salary Payment
                        DeleteStatus = dLayer.DeleteData("Pay_EmployeePaymentDetails", "N_ReceiptID", N_TransID, "N_CompanyID=" + N_CompanyID, connection, transaction);
                        DeleteStatus = dLayer.DeleteData("Pay_EmployeePayment", "N_ReceiptID", N_TransID, "N_CompanyID=" + N_CompanyID, connection, transaction);
                        B_IsDelete = true;
                        break;
                    case 586://Paycode Adjustment
                        DeleteStatus = dLayer.ExecuteNonQuery("SP_Pay_EndOfServiceBatchPosting_Del @nCompanyID,@nFnYearID,'EOS',@xTransCode", DeleteParams, connection, transaction);
                        B_IsDelete = true;
                        break;
                    case 578://Gosi Payment
                        DeleteStatus = dLayer.DeleteData("Pay_GOSIPaymentDetails", "N_ReceiptID", N_TransID, "N_CompanyID=" + N_CompanyID, connection, transaction);
                        DeleteStatus = dLayer.DeleteData("Pay_GOSIPayment", "N_ReceiptID", N_TransID, "N_CompanyID=" + N_CompanyID, connection, transaction);
                        B_IsDelete = true;
                        break;
                    case 208://addition Dedution
                        DeleteStatus = dLayer.DeleteData("Pay_MonthlyAddOrDed", "N_TransID", N_TransID, "N_CompanyID=" + N_CompanyID, connection, transaction);
                        DeleteStatus = dLayer.DeleteData("Pay_MonthlyAddOrDedDetails", "N_TransID", N_TransID, "N_CompanyID=" + N_CompanyID, connection, transaction);
                        B_IsDelete = true;
                        break;
                    case 1032://Employee Clearance
                        DeleteStatus = dLayer.DeleteData("Pay_EmployeeClearanceDetails", "N_ClearanceID", N_TransID, "N_CompanyID=" + N_CompanyID, connection, transaction);
                        DeleteStatus = dLayer.DeleteData("Pay_EmployeeClearance", "N_ClearanceID", N_TransID, "N_CompanyID=@nFnYearID and N_FnYearID=" + N_FnYearID, connection, transaction);
                        B_IsDelete = true;
                        break;
                    case 1068://Employee Evaluation
                        DeleteStatus = dLayer.DeleteData("Pay_EmpEvaluationDetails", "N_EvalID", N_TransID, "N_CompanyID=" + N_CompanyID, connection, transaction);
                        DeleteStatus = dLayer.DeleteData("Pay_EmpEvaluation", "N_EvalID", N_TransID, "N_CompanyID=@nFnYearID and N_FnYearID=" + N_FnYearID, connection, transaction);
                        B_IsDelete = true;
                        break;
                    case 684://Material Dispatch
                        DeleteStatus = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParamsPro, connection, transaction);
                        B_IsDelete = true;
                        break;
                    case 81://Sales Order
                    case 1045://BOM
                        DeleteStatus = dLayer.ExecuteNonQuery("UPDATE Prj_BOMDetails SET B_BOMProcessed=0 where N_BOMDetailID in (select N_BOMDetailID from Inv_SalesOrderDetails where N_SalesOrderID=@nTransID and N_CompanyID=@nFnYearID and N_FnYearId=@nFnYearID)", DeleteParams, connection, transaction);
                        DeleteStatus = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParamsPro, connection, transaction);
                        B_IsDelete = true;
                        break;
                    case 1015://PROJECT TRANSFER
                        DeleteStatus = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParamsPro, connection, transaction);
                        B_IsDelete = true;
                        break;
                    case 44:
                    case 45:
                    case 46:
                        DeleteStatus = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParamsPro, connection, transaction);
                        X_Action = "Delete";
                        B_IsDelete = true;
                        break;
                    case 80://Sales Quotation
                        DeleteStatus = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParamsPro, connection, transaction);
                        X_Action = "Delete";
                        B_IsDelete = true;
                        break;
                    default:
                        DeleteStatus = dLayer.ExecuteNonQuery("DELETE FROM " + X_ScreenTable + " where " + X_Criteria, connection, transaction);
                        X_Action = "Delete";
                        B_IsDelete = true;
                        break;

                }
                if (DeleteStatus == 0)
                {
                    X_Message = "Error";
                }
            }

            Approvals.Rows[0]["btnSaveText"] = X_Action;
            Approvals.Rows[0]["nextApprovalLevel"] = N_ApprovalLevelID;
            Approvals.Rows[0]["saveTag"] = N_ProcStatusID;
            Approvals.Rows[0]["approvalID"] = N_ApprovalID;
            Approvals.AcceptChanges();

            this.LogApprovals(Approvals, N_FnYearID, X_TransType, N_TransID, X_TransCode, 1, PartyName, 0, "", User, dLayer, connection, transaction);
            return X_Message;
        }

        public bool ContainColumn(string columnName, DataTable table)
        {
            DataColumnCollection columns = table.Columns;
            if (columns.Contains(columnName))
            {
                return true;
            }
            return false;
        }
        public DataTable ListToTable(SortedList List)
        {
            DataTable ResultTable = new DataTable();
            if (List.Count > 0)
            {
                ICollection Keys = List.Keys;
                foreach (string key in Keys)
                {
                    ResultTable = this.AddNewColumnToDataTable(ResultTable, key, typeof(string), List[key].ToString());
                }
            }
            DataRow Row = ResultTable.NewRow();
            ResultTable.Rows.Add(Row);
            ResultTable.AcceptChanges();
            return ResultTable;
        }

        public int GetUserID(ClaimsPrincipal User)
        {
            return this.getIntVAL(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
        public string GetUserPattern(ClaimsPrincipal User)
        {
            return (User.FindFirst(ClaimTypes.SerialNumber)?.Value).ToString();
        }
        public int GetCompanyID(ClaimsPrincipal User)
        {
            object cmpID = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (cmpID == null) cmpID = 0;
            return this.getIntVAL(cmpID.ToString());
        }
        public string GetCompanyName(ClaimsPrincipal User)
        {
            object cmpName = User.FindFirst(ClaimTypes.StreetAddress)?.Value;
            if (cmpName == null) cmpName = "";
            return cmpName.ToString();
        }
        public string GetEmailID(ClaimsPrincipal User)
        {
            return User.FindFirst(ClaimTypes.Email)?.Value;
        }
        public int GetUserCategory(ClaimsPrincipal User)
        {
            return this.getIntVAL(User.FindFirst(ClaimTypes.GroupSid)?.Value);
        }
        public int GetClientID(ClaimsPrincipal User)
        {
            return this.getIntVAL(User.FindFirst(ClaimTypes.PrimaryGroupSid)?.Value);
        }

        public int GetGlobalUserID(ClaimsPrincipal User)
        {
            return this.getIntVAL(User.FindFirst(ClaimTypes.PrimarySid)?.Value);
        }

        public string GetConnectionString(ClaimsPrincipal User)
        {
            return config.GetConnectionString(User.FindFirst(ClaimTypes.Uri)?.Value);
        }

        public string GetUserName(ClaimsPrincipal User)
        {
            return User.FindFirst(ClaimTypes.Name)?.Value;
        }

        public string GetUserLoginName(ClaimsPrincipal User)
        {
            return User.FindFirst(ClaimTypes.Upn)?.Value;
        }

        public string GetUploadsPath(ClaimsPrincipal User, string DocType)
        {
            string folderName = "General";
            switch (DocType.ToLower())
            {
                case "productcategory":
                    folderName = "Product_Category_Images";
                    break;
                case "ecomproductimages":
                    folderName = "Ecom_Product_Images";
                    break;
                case "posproductimages":
                    folderName = "Pos_Product_Images";
                    break;
                default: break;
            }
            string docPath = uploadedImagesPath + this.GetClientID(User) + "/" + this.GetCompanyID(User) + "/" + folderName + "/";
            if (!Directory.Exists(docPath))
                Directory.CreateDirectory(docPath);

            return docPath;
        }

        public bool writeImageFile(string FileString, string Path, string Name)
        {

            string imageName = "\\" + Name + ".jpg";
            string imgPath = Path + imageName;

            byte[] imageBytes = Convert.FromBase64String(FileString);

            System.IO.File.WriteAllBytes(imgPath, imageBytes);
            return true;


        }

        public string GetTempFileName(ClaimsPrincipal User, string DocType, string fileName)
        {
            string tempFileName = this.RandomString() + fileName;
            string filePath = this.GetUploadsPath(User, DocType);
            if (Directory.Exists(filePath) && File.Exists(filePath + fileName) && Directory.Exists(this.TempFilesPath))
            {
                // File.Copy(filePath+fileName, this.tempFilePath+tempFileName, true);
                try
                {
                    File.Copy(Path.Combine(filePath, fileName), Path.Combine(this.TempFilesPath, tempFileName), true);
                }
                catch (IOException copyError)
                {
                    Console.WriteLine(copyError.Message);
                }
            }
            return tempFileName;
        }

        public string GetTempFilePath()
        {
            return this.TempFilesPath;
        }

        private static Random random = new Random();
        public string RandomString(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool ExportToExcel(ClaimsPrincipal User, string _fillquery, string _filename, IDataAccessLayer dLayer, SqlConnection connection)
        {
            bool result = false;

            try
            {
                DataTable ExportTable = dLayer.ExecuteDataTable(_fillquery, connection);
                if (ExportTable.Rows.Count > 0)
                {
                    // foreach (DataRow dr in ExportTable.Rows)
                    GenerateExportFile(this.TempFilesPath.ToString(), ExportTable, _filename);
                }
                result = true;
            }
            catch (Exception ex)
            {
                return result;
            }


            return result;
        }

        public static bool GenerateExportFile(string _filepath, DataTable ExportTable, string _filename)
        {
            bool res = false;

            try
            {

                StringBuilder sb = new StringBuilder();
                string xExportFileName = "";
                string FileCreateTime = DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                xExportFileName = _filepath + _filename + ".csv";
                int index = 0;
                foreach (DataRow drow in ExportTable.Rows)
                {


                    if (!File.Exists(xExportFileName))
                        File.Create(xExportFileName).Close();
                    else
                        File.WriteAllText(xExportFileName, String.Empty);
                    string delimiter = ",";
                    string[][] header = new string[][]
                    {
                        new string[]{"Ledger Code","Ledger Name","Remarks","Debit","Credit"}
                    };
                    string[][] output = new string[][]
                    {

                       new string[]{drow["X_LedgerCode"].ToString(),drow["X_LedgerName"].ToString(),drow["X_Remarks"].ToString(),drow["Debit"].ToString().Replace(",","").Trim(),drow["Credit"].ToString().Replace(",","").Trim()}
                    };
                    int length = output.GetLength(0);

                    if (index == 0)
                        sb.AppendLine(string.Join(delimiter, header[0]));
                    for (index = 0; index < length; index++)
                        sb.AppendLine(string.Join(delimiter, output[index]));

                }

                File.AppendAllText(xExportFileName, sb.ToString());

                res = true;
            }
            catch (Exception ex)
            {
                if (ex is DirectoryNotFoundException)
                {
                    res = false;
                }
                else
                {
                    res = false;
                }
            }
            return res;
        }

        public bool CheckVersion(string xSrcVersion,IDataAccessLayer dLayer,SqlConnection connection)
        {
            SortedList Params = new SortedList();
            string xAppVersion="";
            String xAPIVersion=myCompanyID._APIVersion;

            object AppVersion = dLayer.ExecuteScalar("select TOP 1 X_AppVersion from Gen_SystemSettings order by D_EntryDate DESC", Params, connection);
            if(AppVersion!=null)xAppVersion=AppVersion.ToString();

            if(xAppVersion!="")
            {
                if((xAppVersion!=xSrcVersion)||(xAppVersion!=xAPIVersion)||(xSrcVersion!=xAPIVersion))
                {                   
                    return false;
                }
            }
            return true;
        }

        public bool Depreciation(IDataAccessLayer dLayer,int N_CompanyID,int N_FnYearID,int N_UserID, int N_ItemID, DateTime D_EndDate, String X_DeprNo,SqlConnection connection, SqlTransaction transaction)
        {

            DataSet dsFunction = new DataSet();

            string X_lastRundate = "";
            object Result = "";
            DateTime StartDate, EndDate, Suspnsn_FromDate, Suspnsn_ToDate, RunDate;
            TimeSpan suspnsn = TimeSpan.Zero;
            Double DepreciationAmt = 0;
            Double TotalDepAmt = 0;
            int N_DeprID = 0, N_ActionID = 0, N_SuspendID = 0;
            bool B_completed = true;
            RunDate = D_EndDate;
            DataTable DepTable;
            DataTable AssSuspensionTable;
            DataTable AssTransactionTable;
            string SqlCmd1="",SqlCmd2="",SqlCmd3="";
            SortedList Params = new SortedList();

            SqlCmd1="SELECT max(dbo.Ass_Depreciation.D_EndDate) AS  D_EndDate,dbo.Ass_AssetMaster.N_ItemID,dbo.Ass_AssetMaster.X_ItemCode, dbo.Ass_AssetMaster.N_BookValue, dbo.Ass_AssetMaster.N_LifePeriod, dbo.Ass_PurchaseDetails.D_PurchaseDate, dbo.Ass_AssetMaster.N_BranchID, dbo.Ass_PurchaseDetails.N_Price,dbo.Ass_AssetMaster.D_PlacedDate,dbo.Ass_AssetMaster.N_CategoryID,dbo.Ass_AssetMaster.N_SalvageAmt FROM   dbo.Ass_AssetMaster INNER JOIN dbo.Ass_PurchaseDetails ON dbo.Ass_AssetMaster.N_AssetInventoryDetailsID = dbo.Ass_PurchaseDetails.N_AssetInventoryDetailsID left outer join Ass_Depreciation on Ass_Depreciation.N_ItemID =Ass_AssetMaster.N_ItemID and Ass_Depreciation.N_CompanyID=Ass_AssetMaster.N_CompanyID Where Ass_AssetMaster.N_ItemID=" + N_ItemID + " group by dbo.Ass_AssetMaster.N_ItemID,dbo.Ass_AssetMaster.X_ItemCode, dbo.Ass_AssetMaster.N_BookValue, dbo.Ass_AssetMaster.N_LifePeriod, dbo.Ass_PurchaseDetails.D_PurchaseDate, dbo.Ass_AssetMaster.N_BranchID, dbo.Ass_PurchaseDetails.N_Price,dbo.Ass_AssetMaster.D_PlacedDate,dbo.Ass_AssetMaster.N_CategoryID,dbo.Ass_AssetMaster.N_SalvageAmt";
            DepTable = dLayer.ExecuteDataTable(SqlCmd1, Params, connection,transaction);

            if (DepTable.Rows.Count > 0)
            {
                String ItemCode = DepTable.Rows[0]["X_ItemCode"].ToString();
                int BranchId = this.getIntVAL(DepTable.Rows[0]["N_BranchID"].ToString());

                Double N_Price = this.getVAL(DepTable.Rows[0]["N_Price"].ToString());
                Double BookValue = this.getVAL(DepTable.Rows[0]["N_BookValue"].ToString());
                Double LifePeriod = this.getVAL(DepTable.Rows[0]["N_LifePeriod"].ToString());
                DateTime D_PurchaseDate = Convert.ToDateTime(DepTable.Rows[0]["D_PurchaseDate"]);
                DateTime D_PlacedDate = Convert.ToDateTime(DepTable.Rows[0]["D_PlacedDate"]);

                if (DepTable.Rows[0]["D_EndDate"].ToString() != "")
                    X_lastRundate = Convert.ToDateTime(DepTable.Rows[0]["D_EndDate"]).ToShortDateString();
                else
                    X_lastRundate = "";

                int N_CategoryID = this.getIntVAL(DepTable.Rows[0]["N_CategoryID"].ToString());
                double N_lifetime = 0.0;
                if ((LifePeriod * 12) > 0)
                {
                    string[] temp = (LifePeriod * 12).ToString().Split('.');

                    N_lifetime = this.getIntVAL(temp[0]);
                }

                DateTime D_Lifeperiod = D_PurchaseDate.AddMonths(this.getIntVAL(N_lifetime.ToString()));
                int N_CurLife = (D_Lifeperiod - D_PlacedDate).Days;

                // ---- Set the start and end date of depreciation 

                if (X_lastRundate != "")
                {
                    StartDate = Convert.ToDateTime(X_lastRundate.ToString());
                    StartDate = StartDate.AddDays(1);
                    if (Convert.ToDateTime(X_lastRundate.ToString()).Year < StartDate.Year)
                    {
                        if (Convert.ToDateTime(X_lastRundate.ToString()) != StartDate.AddDays(-1))
                            StartDate = Convert.ToDateTime(X_lastRundate.ToString());
                    }
                }
                else
                {
                    DateTime Fn_startdate = Convert.ToDateTime(D_PlacedDate);
                    if (D_PlacedDate.Date < Fn_startdate)
                        StartDate = Fn_startdate;
                    else
                        StartDate = D_PlacedDate;
                }
                if (StartDate.Year > D_Lifeperiod.Year && StartDate.Month > D_Lifeperiod.Month)
                {
                    return false;
                }
                if (LifePeriod == 0)
                {
                    return false;
                }
                EndDate = D_EndDate;

                //-------- SUSPENSION 

                SqlCmd2="select N_SuspendID,D_FromDate,D_ToDate from Ass_Suspension Where N_ItemID ='" + N_ItemID + "'  and D_FromDate <='" + StartDate.ToString("s") + "'";
                AssSuspensionTable= dLayer.ExecuteDataTable(SqlCmd2, Params, connection,transaction);

                if (D_PlacedDate <= EndDate)
                {
                    int endmonth = EndDate.Month;
                    bool flag = false;
                    if (StartDate.Month == EndDate.Month)
                        flag = true;
                    while (StartDate <= D_EndDate)
                    {
                        if (StartDate.Month == endmonth)
                            EndDate = D_EndDate;
                        else
                        {
                            EndDate = StartDate.AddMonths(1);
                            EndDate = EndDate.AddDays(-(EndDate.Day));
                        }
                        if (EndDate > D_PlacedDate.AddDays((LifePeriod * 365) - 1))
                        {
                            EndDate = D_PlacedDate.AddDays((LifePeriod * 365) - 1);
                            flag = true;
                        }
                        if (AssSuspensionTable.Rows.Count > 0)
                        {
                            foreach (DataRow dSpnrow in AssSuspensionTable.Rows)
                            {
                                Suspnsn_FromDate = Convert.ToDateTime(dSpnrow["D_FromDate"]);
                                Suspnsn_ToDate = Convert.ToDateTime(dSpnrow["D_ToDate"]);

                                N_SuspendID = this.getIntVAL(dSpnrow["N_SuspendID"].ToString());
                                if (StartDate > Suspnsn_FromDate && StartDate > Suspnsn_ToDate)
                                    suspnsn = TimeSpan.Zero;
                                else
                                {
                                    if (Suspnsn_ToDate < EndDate)
                                    {
                                        DateTime dt;
                                        if (StartDate > Suspnsn_FromDate)
                                            suspnsn += Suspnsn_ToDate.AddDays(1) - StartDate;
                                        else
                                            suspnsn += Suspnsn_ToDate.AddDays(1) - Suspnsn_FromDate;
                                    }
                                    else
                                    {
                                        if (StartDate > Suspnsn_FromDate)
                                            suspnsn += EndDate - StartDate.AddDays(-1);
                                        else
                                            suspnsn += EndDate - Suspnsn_FromDate.AddDays(-1);
                                    }
                                }

                            }
                        }

                        //--Taking Current book value amount
                        SqlCmd3="select MAX(N_LifePeriod) AS N_LifePeriod, SUM(N_Amount) AS N_BookValue from Ass_Transactions where X_Type <> 'Depreciation' and D_EndDate <='" + EndDate.ToString("yyyy-MM-dd") + "' and  N_ItemID=" + N_ItemID;
                        AssTransactionTable = dLayer.ExecuteDataTable(SqlCmd3, Params, connection,transaction);

                        BookValue = this.getVAL(AssTransactionTable.Rows[0]["N_BookValue"].ToString());
                        LifePeriod = this.getVAL(AssTransactionTable.Rows[0]["N_LifePeriod"].ToString());

                        //--Taking Total Depreciation processed amount
                        TotalDepAmt = this.getVAL(dLayer.ExecuteScalar("select SUM(N_Amount) from Ass_Depreciation where N_ItemID = " + N_ItemID,Params,connection,transaction).ToString());

                        //--Check Total Dep Amount with Book value
                        if (BookValue >= TotalDepAmt)
                        {
                            //--- Depreciation Amount Calculation
                            int TotalDays = 0;
                            TimeSpan ts = EndDate - StartDate - suspnsn;

                            if ((StartDate.Month == 2 && ts.Days >= 27) || ts.Days >= 29)
                                TotalDays = 30;
                            else
                                TotalDays = this.getIntVAL(ts.Days.ToString()) + 1;
                            
                            DepreciationAmt = Math.Round(((this.getVAL((TotalDays).ToString()) / (LifePeriod * 12.0 * 30.0))) * BookValue, 2);
                            
                            double SalvageAmt = this.getVAL(DepTable.Rows[0]["N_SalvageAmt"].ToString());

                            if (DepreciationAmt > BookValue - SalvageAmt - TotalDepAmt)
                                DepreciationAmt = BookValue - SalvageAmt - TotalDepAmt; 

   
                            if (BookValue - TotalDepAmt + DepreciationAmt > SalvageAmt)
                            {
                                DataTable DepreciationTable = new DataTable();
                                DepreciationTable.Clear();
                                DepreciationTable.Columns.Add("N_CompanyID");
                                DepreciationTable.Columns.Add("N_FnYearID");
                                DepreciationTable.Columns.Add("X_DepriciationNo");
                                DepreciationTable.Columns.Add("D_StartDate");
                                DepreciationTable.Columns.Add("D_EndDate");
                                DepreciationTable.Columns.Add("D_RunDate");
                                DepreciationTable.Columns.Add("N_Amount");
                                DepreciationTable.Columns.Add("N_UserID");
                                DepreciationTable.Columns.Add("N_SuspendID");
                                DepreciationTable.Columns.Add("N_ItemID");
                                DepreciationTable.Columns.Add("N_DeprID");

                                DataRow row = DepreciationTable.NewRow();
                                row["N_DeprID"] = 0;
                                row["N_CompanyID"] = N_CompanyID;
                                row["N_FnYearID"] = N_FnYearID;
                                row["X_DepriciationNo"] = X_DeprNo;
                                row["D_StartDate"] = getDateVAL(StartDate);
                                row["D_EndDate"] = getDateVAL(EndDate);
                                row["D_RunDate"] = getDateVAL(RunDate);
                                row["N_Amount"] = DepreciationAmt;
                                row["N_UserID"] = N_UserID;
                                row["N_SuspendID"] = N_SuspendID;
                                row["N_ItemID"] = N_ItemID;
                                DepreciationTable.Rows.Add(row);

                                int N_DprID = dLayer.SaveData("Ass_Depreciation", "N_DeprID", DepreciationTable, connection, transaction);
                                if (N_DprID <= 0)
                                {
                                    transaction.Rollback();
                                }

                                DataTable TransTable = new DataTable();
                                TransTable.Clear();
                                TransTable.Columns.Add("N_CompanyID");
                                TransTable.Columns.Add("N_FnYearID");
                                TransTable.Columns.Add("N_Price");
                                TransTable.Columns.Add("N_LifePeriod");
                                TransTable.Columns.Add("D_StartDate");
                                TransTable.Columns.Add("D_EndDate");
                                TransTable.Columns.Add("X_Type");
                                TransTable.Columns.Add("N_Amount");
                                TransTable.Columns.Add("N_ItemID");
                                TransTable.Columns.Add("N_AssetInventoryDetailsID");
                                TransTable.Columns.Add("N_ActionID");

                                DataRow row1 = TransTable.NewRow();
                                row1["N_ActionID"] = 0;
                                row1["N_CompanyID"] = N_CompanyID;
                                row1["N_FnYearID"] = N_FnYearID;
                                row1["N_Price"] = N_Price;
                                row1["N_LifePeriod"] = LifePeriod;
                                row1["D_StartDate"] = getDateVAL(StartDate);
                                row1["D_EndDate"] = getDateVAL(EndDate);
                                row1["X_Type"] = "Depreciation";
                                row1["N_Amount"] = DepreciationAmt;
                                row1["N_ItemID"] = N_ItemID;
                                row1["N_AssetInventoryDetailsID"] = N_DprID;       
                                TransTable.Rows.Add(row);

                                int N_TransID = dLayer.SaveData("Ass_Transactions", "N_ActionID", TransTable, connection, transaction);
                                if (N_TransID <= 0)
                                {
                                    transaction.Rollback();
                                }  
                            }
                        }

                        if (!flag)
                            StartDate = EndDate.AddDays(1);
                        else
                            break;
                    }
                }
            }

            // --- Posting Depreciation 
            if (B_completed)
            {
                SortedList PostParams = new SortedList(){
                                    {"N_CompanyID",N_CompanyID.ToString()},
                                    {"X_InventoryMode","Depreciation"},
                                    {"N_InternalID",N_DeprID},
                                    {"N_UserID",N_UserID}};
                dLayer.ExecuteNonQueryPro("SP_Acc_InventoryPosting", PostParams, connection, transaction);

            }
            return B_completed;
        }

    }



    public interface IMyFunctions
    {
        public bool CheckPermission(int N_CompanyID, int N_MenuID, string admin, string FieldName, IDataAccessLayer dLayer, SqlConnection connection);
        public bool CheckPermission(int N_CompanyID, int N_MenuID, string admin, string FieldName, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction);
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
        public string ReturnSettings(string Group, string Description, string ValueColumn, int nCompanyID, IDataAccessLayer dLayer, SqlConnection Connection, SqlTransaction transaction);
        public string ReturnSettings(string Group, string Description, string ValueColumn, string ConditionColumn, string Value, int nCompanyID, IDataAccessLayer dLayer, SqlConnection Connection, SqlTransaction transaction);
        public string ReturnValue(string TableName, string ColumnReturn, string Condition, SortedList Params, IDataAccessLayer dLayer, SqlConnection connection);
        public DataTable AddNewColumnToDataTable(DataTable MasterDt, string ColName, Type dataType, object Value);
        public string getDateVAL(DateTime val);
        public DateTime GetFormatedDate(string val);
        public DataTable SaveApprovals(DataTable MasterTable, DataTable Approvals, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction);
        public int LogApprovals(DataTable Approvals, int N_FnYearID, string X_TransType, int N_TransID, string X_TransCode, int GroupID, string PartyName, int EmpID, string DepLevel, ClaimsPrincipal User, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction);
        public void UpdateApproverEntry(DataTable Approvals, string ScreenTable, string Criterea, int N_TransID, ClaimsPrincipal User, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction);
        public string UpdateApprovals(DataTable Approvals, int N_FnYearID, string X_TransType, int N_TransID, string X_TransCode, int N_ProcStatusID, string X_ScreenTable, string X_Criteria, string PartyName, ClaimsPrincipal User, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction);
        public SortedList GetApprovals(int nIsApprovalSystem, int nFormID, int nTransID, int nTransUserID, int nTransStatus, int nTransApprovalLevel, int nNextApprovalLevel, int nApprovalID, int nGroupID, int nFnYearID, int nEmpID, int nActionID, ClaimsPrincipal User, IDataAccessLayer dLayer, SqlConnection connection);
        public DataTable ListToTable(SortedList List);
        public int GetUserID(ClaimsPrincipal User);
        public string GetUserPattern(ClaimsPrincipal User);
        public int GetCompanyID(ClaimsPrincipal User);
        public string GetCompanyName(ClaimsPrincipal User);
        public int GetUserCategory(ClaimsPrincipal User);
        public int GetClientID(ClaimsPrincipal User);
        public int GetGlobalUserID(ClaimsPrincipal User);
        public string GetUserName(ClaimsPrincipal User);
        public string GetUserLoginName(ClaimsPrincipal User);
        public string GetEmailID(ClaimsPrincipal User);


        public string GetConnectionString(ClaimsPrincipal User);

        public bool ContainColumn(string columnName, DataTable table);
        public DataTable GetSettingsTable();
        public bool SendApprovalMail(int N_NextApproverID, int FormID, int TransID, string TransType, string TransCode, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction, ClaimsPrincipal User);
        public bool SendMail(string ToMail, string Body, string Subjectval, IDataAccessLayer dLayer, int FormID, int ReferID, int CompanyID);
        public bool CheckClosedYear(int N_CompanyID, int nFnYearID, IDataAccessLayer dLayer, SqlConnection connection);
        public bool CheckActiveYearTransaction(int nCompanyID, int nFnYearID, DateTime dTransDate, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction);
        public bool ExportToExcel(ClaimsPrincipal User, string _fillquery, string _filename, IDataAccessLayer dLayer, SqlConnection connection);
        public string GetUploadsPath(ClaimsPrincipal User, string DocType);
        public string GetTempFileName(ClaimsPrincipal User, string DocType, string FileName);
        public string GetTempFilePath();
        public string RandomString(int length = 6);
        public bool writeImageFile(string FileString, string Path, string Name);
        public bool CheckVersion(string xSrcVersion,IDataAccessLayer dLayer,SqlConnection connection);
        public bool Depreciation(IDataAccessLayer dLayer,int N_CompanyID,int N_FnYearID,int N_UserID, int N_ItemID, DateTime D_EndDate, String X_DeprNo,SqlConnection connection, SqlTransaction transaction);
    }
}