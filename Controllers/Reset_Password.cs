using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.IO;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text.Encodings;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("resetpassword")]
    [ApiController]
    public class Reset_Password : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly string masterDBConnectionString;



        public Reset_Password(IApiFunctions apiFun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apiFun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");

        }

        //Save....
        [HttpGet("ChangePassword")]
        public ActionResult savedata(string xOldPasswd,string xNewpasswd,string xConfirmpasswd)
        {
            try
            {   
                DataSet dt = new DataSet();       
                int nUserID = myFunctions.GetUserID(User);
                int nCompanyID = myFunctions.GetCompanyID(User);

                SortedList Params = new SortedList();
                int ID=0;
                ID=ValidatePasswordMatching(xOldPasswd,xNewpasswd,xConfirmpasswd);
                if(ID==1)
                    return Ok(api.Warning("Invalid Password"));
                if(ID==2)
                    return Ok(api.Warning("Password Not Matching"));
                if(ID==3)
                    return Ok(api.Warning("Please Choose Different Password"));

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //  SqlTransaction transaction = connection.BeginTransaction();
                    int days=0;
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nUserID", nUserID);
                    object pObjExpPrdCat = dLayer.ExecuteScalar("SELECT Sec_UserCategory.N_PwdExpPeriod FROM  Sec_UserCategory INNER JOIN Sec_User ON Sec_UserCategory.N_CompanyID = Sec_User.N_CompanyID AND Sec_UserCategory.N_UserCategoryID = Sec_User.N_UserCategoryID where Sec_User.N_UserID=@nUserID and Sec_User.N_CompanyID=@nCompanyID", Params, connection);
                    if (pObjExpPrdCat == null || pObjExpPrdCat.ToString() == "0" || pObjExpPrdCat.ToString() == "")
                    {
                    object pObjExpPrd = dLayer.ExecuteScalar("SELECT N_Value FROM  Gen_Settings where N_CompanyID=@nCompanyID and X_Group ='Application' and X_Description ='PasswordExpiryDays'", Params, connection);
                    if (pObjExpPrd != null)
                        days = myFunctions.getIntVAL(pObjExpPrd.ToString());
                    }
                    else
                    days = myFunctions.getIntVAL(pObjExpPrdCat.ToString());
                    string password=EncryptString(xNewpasswd);

                    dLayer.ExecuteNonQuery("Update Sec_User set X_Password='" + password + "',D_ExpireDate='" + myFunctions.getDateVAL(Convert.ToDateTime(System.DateTime.Today).AddDays(days)) + "' where N_UserID=@nUserID and N_CompanyID=@nCompanyID", Params, connection);
                    
                    using (SqlConnection olivCon = new SqlConnection(masterDBConnectionString))
                {
                    olivCon.Open();
                    
                    dLayer.ExecuteNonQuery("Update Users set X_Password='" + password + "' where N_UserID="+myFunctions.GetGlobalUserID(User)+" and N_ClientID="+myFunctions.GetClientID(User)+" and X_EmailID='"+myFunctions.GetEmailID(User)+"'", olivCon);

                }
                    
                    
                    return Ok(api.Success(dt));
                  
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }


        }
        private int ValidatePasswordMatching(string xOldPasswd,string xNewpasswd,string xConfirmpasswd)
        {
            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);
            SortedList Params = new SortedList();

            using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string _CurrentPasswd = "Select X_Password from Sec_User where N_UserID=@nUserID and N_CompanyID=@nCompanyID";
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nUserID", nUserID);
                    object pObjCurrpwd = dLayer.ExecuteScalar(_CurrentPasswd, Params, connection);
                     if (DecryptString( pObjCurrpwd.ToString()) != xOldPasswd)
                     {
                         return 1;
                     }
                     if (xNewpasswd != xConfirmpasswd)
                     {
                        return 2;
                     }
                     if (xNewpasswd== DecryptString(pObjCurrpwd.ToString()))
                     {
                         return 3;
                     }
            return 0;
                }
        }

         public string DecryptString(string inputString)
        {
            MemoryStream memStream = null;
            try
            {
                byte[] key = { };
                byte[] IV = { 12, 21, 43, 17, 57, 35, 67, 27 };
                string encryptKey = "aXb2uy4z";
                key = System.Text.Encoding.UTF8.GetBytes(encryptKey);
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

            System.Text.Encoding encoding1 = System.Text.Encoding.UTF8;
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
        public string EncryptString(string inputString)
        {
            MemoryStream memStream = null;
            try
            {
                byte[] key = { };
                byte[] IV = { 12, 21, 43, 17, 57, 35, 67, 27 };
                string encryptKey = "aXb2uy4z";
                key =System.Text.Encoding.UTF8.GetBytes(encryptKey);
                byte[] byteInput = System.Text.Encoding.UTF8.GetBytes(inputString);
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


      
    }
}