using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SmartxAPI.GeneralFunctions
{
    public class MyFunctions:IMyFunctions
    {
         public MyFunctions()
        {
        }

    public bool CheckPermission(int N_CompanyID,int N_MenuID, string admin,IDataAccessLayer dLayer)
        {
            SortedList Params=new SortedList();
            Params.Add("@p1",N_CompanyID);
            Params.Add("@p2",N_MenuID);
            Params.Add("@p3",admin);
            bool Result = Convert.ToBoolean(dLayer.ExecuteScalar("Select ISNULL(B_Visible,0) From vw_userPrevileges Where N_CompanyID=@p1 and N_MenuID = @p2 and X_UserCategory=@p3",Params));
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
            try
            {
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

            }
            catch (Exception ex)
            {

            }
            return Convert.ToBase64String(memStream.ToArray());
        }

        public string DecryptString(string inputString)
        {
            MemoryStream memStream = null;
            try
            {
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
            }
            catch (Exception ex)
            {

            }

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


    }
public interface IMyFunctions
    {
        public bool CheckPermission(int N_CompanyID,int N_MenuID, string admin,IDataAccessLayer dLayer);
        public int getIntVAL(string val);
        public string EncryptString(string inputString);
        public string DecryptString(string inputString);
    }    
}