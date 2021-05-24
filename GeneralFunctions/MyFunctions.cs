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

namespace SmartxAPI.GeneralFunctions
{
    public class MyFunctions : IMyFunctions
    {
        private readonly string ApprovalLink;
        private readonly IConfiguration config;
        public MyFunctions(IConfiguration conf)
        {
            ApprovalLink = conf.GetConnectionString("ApprovalLink");
            config = conf;
            
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

        public SortedList GetApprovals(int nIsApprovalSystem, int nFormID, int nTransID, int nTransUserID, int nTransStatus, int nTransApprovalLevel, int nNextApprovalLevel, int nApprovalID, int nGroupID, int nFnYearID, int nEmpID, int nActionID, ClaimsPrincipal User, IDataAccessLayer dLayer, SqlConnection connection)
        {
            DataTable SecUserLevel = new DataTable();
            DataTable GenStatus = new DataTable();
            int nMinLevel = 1, nMaxLevel = 0, nActionLevelID = 0, nSubmitter = 0;
            int nNextApprovalID = nTransApprovalLevel + 1;
            string xLastUserName = "", xEntryTime = "";
            int nTempStatusID = 0;
            bool bIsEditable = false;
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
                        if (nTransStatus != 918 && nTransStatus != 919 && nTransStatus != 920)
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
                            xEntryTime = Convert.ToDateTime(xEntryTime).ToString("dd/MM/yyyy HH:mm:ss");
                        Response["lblText"] = Response["lblText"].ToString().Replace("#DATE", xEntryTime);
                    }

                }

                //Blocking edit control of Approvers
                if (!bIsEditable)
                {
                    if (nNextApprovalLevel != 1)
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

                MasterTable = this.AddNewColumnToDataTable(MasterTable, "N_ApprovalLevelId", typeof(int), N_userLevel);
                MasterTable = this.AddNewColumnToDataTable(MasterTable, "N_ProcStatus", typeof(int), N_ProcStatus);
                MasterTable = this.AddNewColumnToDataTable(MasterTable, "B_IssaveDraft", typeof(int), N_SaveDraft);
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
                    case 82:
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
        public int GetCompanyID(ClaimsPrincipal User)
        {
            return this.getIntVAL(User.FindFirst(ClaimTypes.Sid)?.Value);
        }
        public string GetCompanyName(ClaimsPrincipal User)
        {
            return User.FindFirst(ClaimTypes.StreetAddress)?.Value;
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
        public int GetCompanyID(ClaimsPrincipal User);
        public string GetCompanyName(ClaimsPrincipal User);
        public int GetUserCategory(ClaimsPrincipal User);
        public int GetClientID(ClaimsPrincipal User);
        public int GetGlobalUserID(ClaimsPrincipal User);
        public string GetEmailID(ClaimsPrincipal User);

        public string GetConnectionString(ClaimsPrincipal User);

        public bool ContainColumn(string columnName, DataTable table);
        public DataTable GetSettingsTable();
        public bool SendApprovalMail(int N_NextApproverID, int FormID, int TransID, string TransType, string TransCode, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction, ClaimsPrincipal User);
        public bool CheckClosedYear(int N_CompanyID, int nFnYearID, IDataAccessLayer dLayer, SqlConnection connection);
    }
}