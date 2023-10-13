
using Microsoft.AspNetCore.Mvc;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Text;



using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

using System.Text.RegularExpressions;

using System.Security;

using System.Net;
using System.Net.Mail;
using System.Windows;



namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("company")]
    [ApiController]
    public class Acc_Company : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly string masterDBConnectionString;
        private readonly string TempFilesPath;
         private string AppURL;

        public Acc_Company(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf,IMyAttachments myAtt)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            TempFilesPath = conf.GetConnectionString("TempFilesPath");
            connectionString = conf.GetConnectionString("SmartxConnection");
            masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");
            AppURL = conf.GetConnectionString("AppURL");
        }

        //GET api/Company/list5
        // [AllowAnonymous]
        [HttpGet("list")]
        public ActionResult GetAllCompanys()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            // string sqlCommandText = "select N_CompanyId as nCompanyId,X_CompanyName as xCompanyName,X_CompanyCode as xCompanyCode,I_Logo,X_Country from Acc_Company where B_Inactive =@inactive and N_ClientID=@nClientID order by X_CompanyName";
            string sqlCommandText = "select Acc_Company.N_CompanyId as nCompanyId,Acc_Company.X_CompanyName as xCompanyName,Acc_Company.X_CompanyCode as xCompanyCode,Acc_Company.I_Logo,Acc_Company.X_Country " +
 " from Acc_Company LEFT OUTER JOIN Sec_User ON Acc_Company.N_CompanyID = Sec_User.N_CompanyID  where B_Inactive =@inactive and N_ClientID=@nClientID and Sec_User.X_UserID=@xUserID order by Acc_Company.N_CompanyId";
            Params.Add("@inactive", 0);
            Params.Add("@nClientID", myFunctions.GetClientID(User));
            Params.Add("@xUserID", myFunctions.GetUserLoginName(User));

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "I_CompanyLogo", typeof(string), null);

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["I_Logo"] != null)
                        {
                            string ImageData = row["I_Logo"].ToString();
                            if (ImageData != "")
                            {
                                byte[] Image = (byte[])row["I_Logo"];
                                row["I_CompanyLogo"] = "data:image/png;base64," + Convert.ToBase64String(Image, 0, Image.Length);
                            }
                        }
                    }
                    dt.Columns.Remove("I_Logo");

                    dt.AcceptChanges();


                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }

        }



        [HttpGet("TimeZonelist")]
        public ActionResult GetTimeZonelist()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select N_TimeZoneID,B_IsDST, (X_ZoneName+' '+'GMT'+X_UtcOffSet) as X_ZoneName from Gen_TimeZone";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }


        [HttpGet("details")]
        public ActionResult GetCompanyInfo(int nCompanyID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList Output = new SortedList();
            bool setReadOnly=false;

            string sqlCommandText = "SELECT  Acc_Company.N_CompanyID, Acc_Company.X_CompanyName, Acc_Company.X_ShortName,Acc_Company.X_ContactPerson, Acc_Company.X_EmailID, Acc_Company.X_Address, Acc_Company.X_Website, Acc_Company.X_Country, Acc_Company.X_ZipCode, Acc_Company.X_Phone1, Acc_Company.X_Phone2, Acc_Company.X_CompanyCode, Acc_Company.B_Inactive, Acc_Company.I_Logo,"+ 
                      "Acc_Company.X_FaxNo, Acc_Company.X_Slogan, Acc_Company.X_History, Acc_Company.X_Certifications, Acc_Company.X_OperatingSince, Acc_Company.X_CompanyName_Ar, Acc_Company.N_CurrencyID, Acc_Company.N_CountryID, Acc_Company.N_Country, Acc_Company.X_TaxRegistrationNo, Acc_Company.X_TaxRegistrationName,"+
                     " Acc_Company.X_ShippingName, Acc_Company.X_ShippingStreet, Acc_Company.X_ShippingCity, Acc_Company.X_ShippingAppartment, Acc_Company.X_ShippingState, Acc_Company.X_ShippingZIP, Acc_Company.X_ShippingPhone, Acc_Company.I_Header, Acc_Company.I_Footer, Acc_Company.X_ColorCode, Acc_Company.X_CRNumber, "+
                     " Acc_Company.B_AllowMultipleCom, Acc_Company.N_ClientID, Acc_Company.B_IsDefault, Acc_Company.N_TimeZoneID, Acc_Company.N_Decimal, Acc_Company.N_LocationLimit, Acc_Company.N_BranchLimit, Acc_Company.N_BankID,Acc_TaxType.X_TypeName,Acc_CurrencyMaster.X_CurrencyName AS X_Currency, Gen_TimeZone.X_ZoneName + ' ' + 'GMT' + Gen_TimeZone.X_UtcOffSet AS X_ZoneName, Acc_BankMaster.X_BankName,lan_language.X_Language,Acc_Company.N_LangID As N_LanguageID FROM Acc_Company LEFT OUTER JOIN Acc_BankMaster ON Acc_Company.N_CompanyID = Acc_BankMaster.N_CompanyID AND Acc_Company.N_BankID = Acc_BankMaster.N_BankID LEFT OUTER JOIN Acc_CurrencyMaster ON Acc_Company.N_CompanyID = Acc_CurrencyMaster.N_CompanyID AND Acc_Company.N_CurrencyID = Acc_CurrencyMaster.N_CurrencyID  LEFT OUTER JOIN Gen_TimeZone ON Acc_Company.N_TimeZoneID = Gen_TimeZone.N_TimeZoneID LEFT OUTER JOIN Acc_FnYear ON Acc_Company.N_CompanyID = Acc_FnYear.N_CompanyID LEFT OUTER JOIN Acc_TaxType ON Acc_Company.N_CompanyID = Acc_TaxType.N_CompanyID AND Acc_FnYear.N_TaxType = Acc_TaxType.N_TypeID LEFT OUTER JOIN lan_language ON Acc_Company.N_LangID=lan_language.N_LanguageID where Acc_Company.B_Inactive =@p1 and Acc_Company.N_CompanyID=@p2 and Acc_FnYear.N_FnYearID=(select Top(1) N_FnYearID from Acc_FnYear where N_CompanyID=@p2 order by D_Start Desc) and Acc_Company.N_ClientID=@nClientID";
            Params.Add("@p1", 0);
            Params.Add("@p2", nCompanyID);
            Params.Add("@nClientID", myFunctions.GetClientID(User));
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    DataTable AdminInfo = dLayer.ExecuteDataTable("Select N_UserID,X_UserID as x_AdminName from Sec_User Inner Join Sec_UserCategory on Sec_User.N_UserCategoryID= Sec_UserCategory.N_UserCategoryID and X_UserCategory ='Administrator' and Sec_User.X_UserID='Admin' and Sec_User.N_CompanyID=Sec_UserCategory.N_CompanyID  and Sec_User.N_CompanyID=@p2", Params, connection);
                    // DataTable TaxInfo = dLayer.ExecuteDataTable("Select N_Value as n_PkeyID,X_Value as x_DisplayName from Gen_Settings where N_CompanyID=@p2 and X_Group='Inventory' and X_Description='DefaultTaxCategory'" , Params, connection);

                    DataTable FnYearInfo = dLayer.ExecuteDataTable("Select D_Start as 'd_FromDate',D_End as 'd_ToDate',N_FnYearID, (select top 1 N_FnYearID from vw_CheckTransaction Where N_FnYearID = Acc_FnYear.N_FnYearID and N_CompanyID = Acc_FnYear.N_CompanyID) As 'TransAction',N_TaxType from Acc_FnYear Where N_FnYearID=(select Top(1) N_FnYearID from Acc_FnYear where N_CompanyID=@p2 order by D_Start Desc)  and  N_CompanyID=@p2", Params, connection);
                    if (FnYearInfo.Rows.Count == 0)
                    {
                        FnYearInfo = dLayer.ExecuteDataTable("Select D_Start as 'd_FromDate',D_End as 'd_ToDate',N_FnYearID,0 as 'TransAction',N_TaxType from Acc_FnYear Where N_FnYearID=(select Top(1) N_FnYearID from Acc_FnYear where N_CompanyID=@p2 order by D_Start Desc)  and  N_CompanyID=@p2", Params, connection);
                    }

                    int N_FnYearID = myFunctions.getIntVAL(FnYearInfo.Rows[0]["N_FnYearID"].ToString());

                    DataTable TaxInfo = dLayer.ExecuteDataTable("SELECT  Top(1) Gen_Settings.N_Value AS n_PkeyID, Acc_TaxCategory.X_CategoryName AS x_DisplayName FROM Acc_TaxCategory INNER JOIN Acc_FnYear ON Acc_TaxCategory.N_TaxTypeID = Acc_FnYear.N_TaxType AND Acc_TaxCategory.N_CompanyID = Acc_FnYear.N_CompanyID INNER JOIN Gen_Settings ON Acc_TaxCategory.X_PkeyCode = Gen_Settings.N_Value AND Acc_TaxCategory.N_CompanyID = Gen_Settings.N_CompanyID WHERE (Gen_Settings.N_CompanyID = @p2) AND (Gen_Settings.X_Group = 'Inventory') AND (Gen_Settings.X_Description = 'DefaultTaxCategory') and Acc_FnYear.N_FnYearID=" + N_FnYearID, Params, connection);
                       Params.Add("@nCompanyID", nCompanyID);
                     object CompanyCount = dLayer.ExecuteScalar("select count(1) from Acc_VoucherDetails where N_CompanyID= " + nCompanyID, connection);
                      
                       if(myFunctions.getIntVAL(CompanyCount.ToString())>0){
                          setReadOnly=true;
                       } 
                    DataTable Attachments =new DataTable();
                    
                    SortedList ProParam = new SortedList();
                    ProParam.Add("@CompanyID", nCompanyID);
                   // DataTable Details = dLayer.ExecuteSettingsPro("SP_GenSettings_Disp", QList, myFunctions.GetCompanyID(User), nFnYearID, connection);
                    //Attachments = dLayer.ExecuteDataTablePro("SP_CompanyAttachments",ProParam, connection);
                    Attachments = myAttachments.ViewAttachment(dLayer,nCompanyID,nCompanyID, 113,N_FnYearID, User, connection);
                    Attachments = api.Format(Attachments, "attachments");
                    // Attachments = myFunctions.AddNewColumnToDataTable(Attachments, "FileData", typeof(string), null);
                    // Attachments = myFunctions.AddNewColumnToDataTable(Attachments, "TempFileName", typeof(string), null);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "b_AllowEdit", typeof(bool), setReadOnly);

            // if (Attachments.Rows.Count > 0)
            // {
            //     Attachments = myFunctions.AddNewColumnToDataTable(Attachments, "n_CompanyID", typeof(int), myFunctions.GetCompanyID(User));
            //     Attachments = myFunctions.AddNewColumnToDataTable(Attachments, "n_FnYearID", typeof(int), N_FnYearID);
               
            // }

            // foreach (DataRow var in Attachments.Rows)
            // {
            //     if (var["x_refName"] != null)
            //     {
            //         var path = var["x_refName"].ToString();
            //         if (System.IO.File.Exists(path))
            //         {
            //             Byte[] bytes = System.IO.File.ReadAllBytes(path);
            //             var random = RandomString();
            //             System.IO.File.Copy(path, this.TempFilesPath + random + "." + var["x_Extension"].ToString());
            //             var["TempFileName"] = random + "." + var["x_Extension"].ToString();
            //             var["FileData"] = "data:" + api.GetContentType(path) + ";base64," + Convert.ToBase64String(bytes);
            //         }
            //     }

            // }
            Attachments.AcceptChanges();
                 
                    Output.Add("CompanyInfo", dt);
                    Output.Add("AdminInfo", AdminInfo);
                    Output.Add("TaxInfo", TaxInfo);
                    Output.Add("FnYearInfo", FnYearInfo);
                    Output.Add("Attachments", Attachments);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(Output));
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }

        }
      private static Random random = new Random();
      public string RandomString(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Acc_Company", "N_CompanyID", nCompanyID, "", connection);
                }
                if (Results > 0)
                {
                    return Ok(api.Success("Company Deleted"));
                }
                else
                {
                    return Ok(api.Warning("Unable to Delete Company"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }



        [AllowAnonymous]
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable GeneralTable;
                 DataTable userApps = new DataTable();
                MasterTable = ds.Tables["master"];
                GeneralTable = ds.Tables["general"];
                DataTable Attachment = ds.Tables["attachments"];
                string xUserName = GeneralTable.Rows[0]["X_AdminName"].ToString();
                string xCompanyName = MasterTable.Rows[0]["X_CompanyName"].ToString();
                string xPassword = myFunctions.EncryptString(GeneralTable.Rows[0]["X_AdminPwd"].ToString());
                int n_userType=0;
                int n_GBUserID=0;
                int userID=0;
                int n_ClientID=0;
                int appID=0;
                 DataTable clientCompany = new DataTable();
                 DataTable clientApps = new DataTable();


                string x_DisplayName = myFunctions.ContainColumn("x_DisplayName", GeneralTable) ? GeneralTable.Rows[0]["x_DisplayName"].ToString() : "";
                int n_PkeyID = 0;
                if (GeneralTable.Columns.Contains("n_PkeyID"))
                    n_PkeyID = Convert.ToInt32(GeneralTable.Rows[0]["n_PkeyID"].ToString());
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    object CompanyCode = "";
                    var values = MasterTable.Rows[0]["x_CompanyCode"].ToString();
                    if (values == "@Auto")
                    {
                        CompanyCode = dLayer.ExecuteScalar("Select ISNULL((N_CompanyID),0) + 100 from Acc_Company", connection, transaction);//Need Auto Genetaion here
                        if (CompanyCode.ToString() == "") { return Ok(api.Warning("Unable to generate Company Code")); }
                        MasterTable.Rows[0]["x_CompanyCode"] = CompanyCode;
                    }
                    Params.Add("@p1", xUserName);
                    Params.Add("@p2", xPassword);
                    object CompanyCount = dLayer.ExecuteScalar("select count(1) from Acc_Company where B_IsDefault=1 and N_ClientID=" + myFunctions.GetClientID(User), connection, transaction);

                    int Count = myFunctions.getIntVAL(CompanyCount.ToString());
                    if (Count == 0)
                    {
                        MasterTable.Rows[0]["b_IsDefault"] = 1;
                    }
                    string logo = myFunctions.ContainColumn("i_Logo", MasterTable) ? MasterTable.Rows[0]["i_Logo"].ToString() : "";
                    string footer = myFunctions.ContainColumn("i_Footer", MasterTable) ? MasterTable.Rows[0]["i_Footer"].ToString() : "";
                    string header = myFunctions.ContainColumn("i_Header", MasterTable) ? MasterTable.Rows[0]["i_Header"].ToString() : "";

                    Byte[] logoBitmap = new Byte[logo.Length];
                    Byte[] footerBitmap = new Byte[footer.Length];
                    Byte[] headerBitmap = new Byte[header.Length];

                    logoBitmap = Convert.FromBase64String(logo);
                    footerBitmap = Convert.FromBase64String(footer);
                    headerBitmap = Convert.FromBase64String(header);

                    if (myFunctions.ContainColumn("i_Logo", MasterTable))
                        MasterTable.Columns.Remove("i_Logo");
                    if (myFunctions.ContainColumn("i_Footer", MasterTable))
                        MasterTable.Columns.Remove("i_Footer");
                    if (myFunctions.ContainColumn("i_Header", MasterTable))
                        MasterTable.Columns.Remove("i_Header");
                    MasterTable.AcceptChanges();

                    //object paswd=myFunctions.EncryptString(GeneralTable.Rows[0]["x_AdminPwd"].ToString())

                    int N_CompanyId = dLayer.SaveData("Acc_Company", "N_CompanyID", MasterTable, connection, transaction);
                    if (N_CompanyId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Warning("Unable to save"));
                    }
                    else
                    {
                        if (logo.Length > 0)
                            dLayer.SaveImage("Acc_Company", "I_Logo", logoBitmap, "N_CompanyID", N_CompanyId, connection, transaction);
                        if (footer.Length > 0)
                            dLayer.SaveImage("Acc_Company", "i_Footer", footerBitmap, "N_CompanyID", N_CompanyId, connection, transaction);
                        if (header.Length > 0)
                            dLayer.SaveImage("Acc_Company", "i_Header", headerBitmap, "N_CompanyID", N_CompanyId, connection, transaction);
                        object N_FnYearId = myFunctions.getIntVAL(GeneralTable.Rows[0]["n_FnYearID"].ToString());
                        string pwd = "";
                        string xUsrName = "", xPhoneNo = "";
                        using (SqlConnection cnn = new SqlConnection(masterDBConnectionString))
                        {
                            cnn.Open();
                            SortedList Param = new SortedList();
                            string sqlGUserInfo = "SELECT X_Password FROM Users where x_EmailID='" + GeneralTable.Rows[0]["x_AdminName"].ToString() + "'";
                            pwd = dLayer.ExecuteScalar(sqlGUserInfo, cnn).ToString();

                             string sqlGBUserInfo = "SELECT N_UserID FROM Users where x_EmailID='" + GeneralTable.Rows[0]["x_AdminName"].ToString() + "'";
                            n_GBUserID =myFunctions.getIntVAL(dLayer.ExecuteScalar(sqlGBUserInfo, cnn).ToString());

                          
                            string sqlClientmaster = "SELECT TOP 1 * FROM clientmaster where x_EmailID='" + GeneralTable.Rows[0]["x_AdminName"].ToString() + "'";
                            DataTable dtClientmaster = dLayer.ExecuteDataTable(sqlClientmaster, Param, cnn);
                            xUsrName = dtClientmaster.Rows[0]["X_ClientName"].ToString();
                            xPhoneNo = dtClientmaster.Rows[0]["X_ContactNumber"].ToString();
                            //appID = myFunctions.getIntVAL(dtClientmaster.Rows[0]["N_AppID"].ToString());
                            n_userType = myFunctions.getIntVAL(dtClientmaster.Rows[0]["N_DefaultAppID"].ToString());
                           n_ClientID =myFunctions.getIntVAL(dtClientmaster.Rows[0]["N_ClientID"].ToString());
                           


                        }


                        if (values == "@Auto")
                        {


                            SortedList proParams1 = new SortedList(){
                                        {"N_CompanyID",N_CompanyId},
                                        {"X_ModuleCode","500"},
                                        {"N_UserID",0},
                                        {"X_AdminName",GeneralTable.Rows[0]["x_AdminName"].ToString()},
                                        {"X_AdminPwd",pwd},
                                        {"X_Currency",MasterTable.Rows[0]["x_Currency"].ToString()},
                                        {"N_AppID",myFunctions.getIntVAL(GeneralTable.Rows[0]["n_AppType"].ToString())},
                                        // {"X_UserName",xUsrName}
                                        };
                            dLayer.ExecuteNonQueryPro("SP_NewAdminCreation", proParams1, connection, transaction);
                                string usersql = "SELECT N_UserID FROM Sec_User where X_UserID='" + GeneralTable.Rows[0]["x_AdminName"].ToString() + "'";
                            userID = myFunctions.getIntVAL(dLayer.ExecuteScalar(usersql, connection, transaction).ToString());
                           using (SqlConnection cnn4 = new SqlConnection(masterDBConnectionString))
                           {
                            cnn4.Open();
                            appID = myFunctions.getIntVAL(dLayer.ExecuteScalar( "SELECT Top(1) N_AppID FROM ClientApps where N_ClientID='" +n_ClientID + "'", cnn4).ToString());
                            //insert companywise apps in clientapps //26-09
                            SortedList companyParams = new SortedList(){
                            {"N_ClientID",n_ClientID}};

                            //
                            // inserting new app to this coompany

                           object count = dLayer.ExecuteScalar("select count(1) as N_Count from ClientApps where  N_ClientID=" +n_ClientID + " and N_CompanyID="+N_CompanyId+"",  cnn4);
                           object MaxRefID = dLayer.ExecuteScalar("select Max(N_RefID)+1  from ClientApps ",  cnn4);
                           object Negcount = dLayer.ExecuteScalar("select count(1) as N_Count from ClientApps where  N_ClientID=" +n_ClientID + " and N_CompanyID=-1",  cnn4);

                           


                           SqlTransaction clienttransaction = cnn4.BeginTransaction();
                           if(myFunctions.getIntVAL(count.ToString())==0 && myFunctions.getIntVAL(Negcount.ToString())==0)
                           {
                            clientApps.Clear();
                            clientApps.Columns.Add("N_ClientID");
                            clientApps.Columns.Add("N_AppID");
                            clientApps.Columns.Add("X_AppUrl");
                            clientApps.Columns.Add("X_DbUri");

                            clientApps.Columns.Add("N_UserLimit");
                            clientApps.Columns.Add("B_InActive");
                            clientApps.Columns.Add("X_Sector");
                            clientApps.Columns.Add("N_RefID");

                            clientApps.Columns.Add("D_ExpiryDate");
                            clientApps.Columns.Add("B_Licensed");
                            clientApps.Columns.Add("B_EnableAttachment");
                            clientApps.Columns.Add("B_EnableApproval");

                            clientApps.Columns.Add("N_SubscriptionAmount");
                            clientApps.Columns.Add("N_DiscountAmount");
                            clientApps.Columns.Add("D_StartDate");
                            clientApps.Columns.Add("D_CreatedDate");

                            clientApps.Columns.Add("N_CompanyID");
                            clientApps.Columns.Add("B_IsDisable");

                            DataRow row2 = clientApps.NewRow();
                            row2["N_ClientID"] = n_ClientID;
                            row2["N_AppID"] = appID;
                            row2["X_AppUrl"] =AppURL;
                            row2["X_DbUri"] ="SmartxConnection";

                            row2["N_UserLimit"] = 1;
                            row2["B_InActive"] = 0;
                            row2["X_Sector"] ="Service";
                            row2["N_RefID"] =myFunctions.getIntVAL(MaxRefID.ToString());

                            row2["D_ExpiryDate"] = DateTime.Today.AddDays(14);;
                            row2["B_Licensed"] = 0;
                            row2["B_EnableAttachment"] =0;
                            row2["B_EnableApproval"] =0.ToString();

                            row2["N_SubscriptionAmount"] = 0;
                            row2["N_DiscountAmount"] = 0;
                            row2["D_StartDate"] =DateTime.Today;
                            row2["D_CreatedDate"] =DateTime.Today;

                            row2["N_CompanyID"] =N_CompanyId;
                            row2["B_IsDisable"] = 0;
                            clientApps.Rows.InsertAt(row2, 0);
                          

                            int clientppID = dLayer.SaveData("ClientApps", "N_RefID", clientApps, cnn4, clienttransaction);
                           
                           }
                           else
                           {
                             string companyAppUpdate = "Update ClientApps set N_CompanyID= "+N_CompanyId+"where N_ClientID="+n_ClientID+" and N_AppID="+appID+"";
                            dLayer.ExecuteScalar(companyAppUpdate, companyParams, cnn4,clienttransaction);
                            
                           }
                            //client company creation
                            clientCompany.Clear();
                            clientCompany.Columns.Add("N_ClientCompanyID");
                            clientCompany.Columns.Add("N_ClientID");
                            clientCompany.Columns.Add("N_CompanyID");
                            clientCompany.Columns.Add("X_CompanyName");
                            DataRow row1 = clientCompany.NewRow();
                            row1["N_ClientCompanyID"] = 0;
                            row1["N_ClientID"] = n_ClientID;
                            row1["N_CompanyID"] =N_CompanyId;
                            row1["X_CompanyName"] =xCompanyName.ToString();
                            clientCompany.Rows.InsertAt(row1, 0);
                            int ClientCompanyID = dLayer.SaveData("ClientCompany", "N_ClientCompanyID", clientCompany, cnn4, clienttransaction);
                            clienttransaction.Commit();

                             }




                             

                            SortedList proParams2 = new SortedList(){
                                        {"N_CompanyID",N_CompanyId},
                                        {"N_FnYearID",N_FnYearId},
                                        {"D_Start",GeneralTable.Rows[0]["d_FromDate"].ToString()},
                                        {"D_End",GeneralTable.Rows[0]["d_ToDate"].ToString()}};
                            N_FnYearId = dLayer.ExecuteScalarPro("SP_FinancialYear_Create", proParams2, connection, transaction);
                           dLayer.ExecuteNonQuery("insert into Sec_UserApps select "+N_CompanyId+",isnull(max(N_APPMappingID),0)+1,"+appID+","+userID+","+n_GBUserID+", NULL from Sec_UserApps",connection, transaction);
                            SortedList proParams3 = new SortedList(){
                                        {"N_CompanyID",N_CompanyId},
                                        {"N_FnYearID",N_FnYearId}};
                            dLayer.ExecuteNonQueryPro("SP_AccGruops_Accounts_Create", proParams3, connection, transaction);
                               using (SqlConnection cnn1 = new SqlConnection(masterDBConnectionString))
                        {
                            cnn1.Open();
                           SortedList proParams6 = new SortedList(){
                                  {"N_ClientID",n_ClientID}};
                          object SettingsCount = dLayer.ExecuteScalar("select count(1) from GenSettings where  N_ClientID=" + myFunctions.GetClientID(User), cnn1);
                          if(myFunctions.getIntVAL(SettingsCount.ToString())>0)
                          {
                            string settingsUserUpdate = "Update GenSettings set N_Value= (SELECT N_Value + (select isnull(N_FreeUsers,0) from AppMaster where N_AppID="+appID+") as N_Value FROM GenSettings where N_ClientID="+n_ClientID+" and X_Description='USER LIMIT') WHERE N_ClientID="+n_ClientID+" and X_Description='USER LIMIT'";
                            dLayer.ExecuteScalar(settingsUserUpdate, proParams6, cnn1);

                            string settingsCompanyUpdate = "Update GenSettings set N_Value= (SELECT N_Value + 1 FROM GenSettings where N_ClientID="+n_ClientID+" and X_Description='COMPANY LIMIT') WHERE N_ClientID="+n_ClientID+" and X_Description='COMPANY LIMIT'";
                            dLayer.ExecuteScalar(settingsCompanyUpdate, proParams6, cnn1);

                            string settingsBranchUpdate = "Update GenSettings set N_Value= (SELECT N_Value + (select isnull(N_FreeBranches,0) from AppMaster where N_AppID="+appID+") as N_Value FROM GenSettings where N_ClientID="+n_ClientID+" and X_Description='BRANCH LIMIT') WHERE N_ClientID="+n_ClientID+" and X_Description='BRANCH LIMIT'";
                            dLayer.ExecuteScalar(settingsBranchUpdate, proParams6, cnn1);
                            string settingsEmpUpdate = "Update GenSettings set N_Value= (SELECT N_Value + (select isnull(N_FreeEmployees,0) from AppMaster where N_AppID="+appID+") as N_Value FROM GenSettings where N_ClientID="+n_ClientID+" and X_Description='EMPLOYEE LIMIT') WHERE N_ClientID="+n_ClientID+" and X_Description='EMPLOYEE LIMIT'";
                            dLayer.ExecuteScalar(settingsEmpUpdate, proParams6, cnn1);
                            string settingsStuUpdate = "Update GenSettings set N_Value= (SELECT N_Value + (select isnull(N_FreeStudents,0) from AppMaster where N_AppID="+appID+") as N_Value FROM GenSettings where N_ClientID="+n_ClientID+" and X_Description='STUDENT LIMIT') WHERE N_ClientID="+n_ClientID+" and X_Description='STUDENT LIMIT'";
                            dLayer.ExecuteScalar(settingsStuUpdate, proParams6, cnn1);
                          }
                          else
                          dLayer.ExecuteNonQueryPro("Sp_GenSettingInsert", proParams6, cnn1);
                        }

                           userApps.Clear();
                           userApps.Columns.Add("N_CompanyID");
                           userApps.Columns.Add("N_AppMappingID");
                           userApps.Columns.Add("N_AppID");
                           userApps.Columns.Add("N_UserID");
                           userApps.Columns.Add("N_GlobalUserID");
                     

                        DataRow row = userApps.NewRow();
                        row["N_CompanyID"] = N_CompanyId;
                        row["N_AppMappingID"] = 0;
                        row["N_AppID"] =n_userType;
                        row["N_UserID"] = userID;
                        row["N_GlobalUserID"] =n_GBUserID;
                        userApps.Rows.InsertAt(row, 0);
                        int userAppsID = dLayer.SaveData("sec_userApps", "n_AppMappingID", userApps, connection, transaction);

              

                        }
                        SortedList taxParams = new SortedList(){
                                        {"@nCompanyID",N_CompanyId},
                                        {"@nFnYearID",N_FnYearId},
                                        {"@nTaxType",myFunctions.getIntVAL(GeneralTable.Rows[0]["n_TaxType"].ToString())}};
                        dLayer.ExecuteNonQuery("UPDATE Acc_FnYear set N_TaxType=@nTaxType where N_FnYearID=@nFnYearID and N_CompanyID=@nCompanyID", taxParams, connection, transaction);

                        // int SalesManrows = dLayer.ExecuteNonQuery("INSERT INTO Inv_Salesman(N_CompanyID, N_SalesmanID, X_SalesmanCode, X_SalesmanName, X_PhoneNo1, X_Email, B_Inactive, N_FnYearID, D_Entrydate, N_BranchID)"+
                        //                                             "select "+N_CompanyId+",MAX(ISNULL(N_SalesmanID,0))+1,MAX(ISNULL(X_SalesmanCode,100))+1,'"+xUsrName+"','"+xPhoneNo+"','"+GeneralTable.Rows[0]["x_AdminName"].ToString()+"',0,"+N_FnYearId+",GETDATE(),0 from Inv_Salesman", Params, connection,transaction);
                        // if (SalesManrows <= 0)
                        // {
                        //     return Ok(api.Warning("Salesman Creation failed"));
                        // } 

                        SortedList proParams4 = new SortedList(){
                                        {"N_CompanyID",N_CompanyId},
                                        {"X_Group","Inventory"},
                                        {"X_Description","DefaultTaxCategory"},
                                        {"N_Value",n_PkeyID},
                                        {"X_Value",x_DisplayName},};
                        dLayer.ExecuteNonQueryPro("SP_GeneralDefaults_ins", proParams4, connection, transaction);

                        SortedList proParams5 = new SortedList(){
                                  {"N_CompanyID",N_CompanyId}};

                        
                         dLayer.ExecuteNonQueryPro("UTL_UpdateGenSettings", proParams5, connection, transaction);
                      


                if (Attachment.Rows.Count > 0)
                    {
                        try
                        {
                            myAttachments.SaveAttachment(dLayer, Attachment,MasterTable.Rows[0]["X_CompanyCode"].ToString(),N_CompanyId, MasterTable.Rows[0]["X_CompanyName"].ToString().Trim(), MasterTable.Rows[0]["X_CompanyCode"].ToString(), N_CompanyId, "Company Document", User, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User, ex));
                        }
                    }
                    transaction.Commit();

                        return Ok(api.Success("Company successfully saved"));
                    }
                }
            }
            catch (Exception ex)
            {

                return Ok(api.Error(User, ex));
            }
        }

        public static string FixBase64ForImage(string image)
        {
            StringBuilder sbText = new StringBuilder(image, image.Length);
            //sbText.Replace("\r\n", String.Empty);
            //sbText.Replace(" ", String.Empty);
            return sbText.ToString();
        }


         [HttpGet("getCompanyAPIKey")]
        public ActionResult GetAPIKey()
        {
            string seperator = "$$";
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string apiKey = myFunctions.EncryptStringForUrl(myFunctions.GetCompanyID(User).ToString() + seperator + myFunctions.GetClientID(User), System.Text.Encoding.Unicode);
            return Ok(api.Success(apiKey));
        }


    }


     

                                  
}