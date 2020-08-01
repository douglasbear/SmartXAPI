using System;
using System.Collections;
using System.Data;
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

        public bool CheckPermission(int N_CompanyID, int N_MenuID, string admin, IDataAccessLayer dLayer)
        {
            SortedList Params = new SortedList();
            Params.Add("@p1", N_CompanyID);
            Params.Add("@p2", N_MenuID);
            Params.Add("@p3", admin);
            bool Result = Convert.ToBoolean(dLayer.ExecuteScalar("Select ISNULL(B_Visible,0) From vw_userPrevileges Where N_CompanyID=@p1 and N_MenuID = @p2 and X_UserCategory=@p3", Params));
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
        public string RetunSettings(int CompanyID, string Group, string Description, string ValueColumn, string ConditionColumn, string Value, SortedList Params, IDataAccessLayer dLayer, SqlConnection Connection)
        {
            string Result = "";
            object obj = dLayer.ExecuteScalar("select " + ValueColumn + " from Gen_Settings where X_Group='" + Group + "' and X_Description='" + Description + "' and N_CompanyID=" + CompanyID + " and " + ConditionColumn + "=" + Value + " ", Params, Connection);
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
    }
    public interface IMyFunctions
    {
        public bool CheckPermission(int N_CompanyID, int N_MenuID, string admin, IDataAccessLayer dLayer);
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
        public string RetunSettings(string CompanyID, string Group, string Description, string ValueColumn, string ConditionColumn, string Value, SortedList Params, IDataAccessLayer dLayer, SqlConnection Connection);
        public string ReturnValue(string TableName, string ColumnReturn, string Condition, SortedList Params, IDataAccessLayer dLayer, SqlConnection connection);
    }
}