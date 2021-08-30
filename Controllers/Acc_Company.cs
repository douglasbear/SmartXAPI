
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
        private readonly string connectionString;
        private readonly string masterDBConnectionString;


        public Acc_Company(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");
        }

        //GET api/Company/list
        // [AllowAnonymous]
        [HttpGet("list")]
        public ActionResult GetAllCompanys()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            // string sqlCommandText = "select N_CompanyId as nCompanyId,X_CompanyName as xCompanyName,X_CompanyCode as xCompanyCode,I_Logo,X_Country from Acc_Company where B_Inactive =@inactive and N_ClientID=@nClientID order by X_CompanyName";
            string sqlCommandText = "select Acc_Company.N_CompanyId as nCompanyId,Acc_Company.X_CompanyName as xCompanyName,Acc_Company.X_CompanyCode as xCompanyCode,Acc_Company.I_Logo,Acc_Company.X_Country "+
 " from Acc_Company LEFT OUTER JOIN Sec_User ON Acc_Company.N_CompanyID = Sec_User.N_CompanyID  where B_Inactive =@inactive and N_ClientID=@nClientID and Sec_User.X_UserID=@xUserID order by X_CompanyName";
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
                return Ok(api.Error(User,e));
            }

        }



[HttpGet("TimeZonelist")]
        public ActionResult GetTimeZonelist()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select N_TimeZoneID,B_IsDST, (X_ZoneName+' '+'GMT'+X_UtcOffSet) as X_ZoneName from Gen_TimeZone";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
                return Ok(api.Error(User,e));
            }
        }


        [HttpGet("details")]
        public ActionResult GetCompanyInfo(int nCompanyID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList Output = new SortedList();

            string sqlCommandText = "SELECT Acc_Company.*, Acc_TaxType.X_TypeName, Gen_TimeZone.X_ZoneName +' '+ Gen_TimeZone.X_UtcOffSet as X_ZoneName FROM Acc_Company LEFT OUTER JOIN Gen_TimeZone ON Acc_Company.N_TimeZoneID = Gen_TimeZone.N_TimeZoneID LEFT OUTER JOIN Acc_FnYear ON Acc_Company.N_CompanyID = Acc_FnYear.N_CompanyID LEFT OUTER JOIN Acc_TaxType ON Acc_Company.N_CompanyID = Acc_TaxType.N_CompanyID AND Acc_FnYear.N_TaxType = Acc_TaxType.N_TypeID where Acc_Company.B_Inactive =@p1 and Acc_Company.N_CompanyID=@p2 and Acc_FnYear.N_FnYearID=(select max(N_FnYearID) from Acc_FnYear where N_CompanyID=@p2) and Acc_Company.N_ClientID=@nClientID";
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

                    DataTable FnYearInfo = dLayer.ExecuteDataTable("Select D_Start as 'd_FromDate',D_End as 'd_ToDate',N_FnYearID, (select top 1 N_FnYearID from vw_CheckTransaction Where N_FnYearID = Acc_FnYear.N_FnYearID and N_CompanyID = Acc_FnYear.N_CompanyID) As 'TransAction',N_TaxType from Acc_FnYear Where N_FnYearID=(select max(N_FnYearID) from Acc_FnYear where N_CompanyID=@p2)  and  N_CompanyID=@p2", Params, connection);
                    if (FnYearInfo.Rows.Count == 0)
                    {
                        FnYearInfo = dLayer.ExecuteDataTable("Select D_Start as 'd_FromDate',D_End as 'd_ToDate',N_FnYearID,0 as 'TransAction',N_TaxType from Acc_FnYear Where N_FnYearID=(select max(N_FnYearID) from Acc_FnYear where N_CompanyID=@p2)  and  N_CompanyID=@p2", Params, connection);
                    }

                    Output.Add("CompanyInfo", dt);
                    Output.Add("AdminInfo", AdminInfo);
                    Output.Add("FnYearInfo", FnYearInfo);

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
                return Ok(api.Error(User,e));
            }

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
                return Ok(api.Error(User,ex));
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
                MasterTable = ds.Tables["master"];
                GeneralTable = ds.Tables["general"];
                string xUserName = GeneralTable.Rows[0]["X_AdminName"].ToString();
                string xPassword = myFunctions.EncryptString(GeneralTable.Rows[0]["X_AdminPwd"].ToString());


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
                        CompanyCode = dLayer.ExecuteScalar("Select ISNULL(MAX(N_CompanyID),0) + 100 from Acc_Company", connection, transaction);//Need Auto Genetaion here
                        if (CompanyCode.ToString() == "") { return Ok(api.Warning("Unable to generate Company Code")); }
                        MasterTable.Rows[0]["x_CompanyCode"] = CompanyCode;
                    }
                    Params.Add("@p1", xUserName);
                    Params.Add("@p2", xPassword);
                    object CompanyCount=dLayer.ExecuteScalar("select count(*) from Acc_Company where B_IsDefault=1 and N_ClientID="+myFunctions.GetClientID(User), connection,transaction);

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
                        using (SqlConnection cnn = new SqlConnection(masterDBConnectionString))
                        {
                            cnn.Open();
                            string sqlGUserInfo = "SELECT X_Password FROM Users where x_EmailID='" + GeneralTable.Rows[0]["x_AdminName"].ToString() + "'";

                            pwd = dLayer.ExecuteScalar(sqlGUserInfo, cnn).ToString();
                        }


                        if (values == "@Auto")
                        {
                            SortedList proParams1 = new SortedList(){
                                        {"N_CompanyID",N_CompanyId},
                                        {"X_ModuleCode","500"},
                                        {"N_UserID",0},
                                        {"X_AdminName",GeneralTable.Rows[0]["x_AdminName"].ToString()},
                                        {"X_AdminPwd",pwd},
                                        {"X_Currency",MasterTable.Rows[0]["x_Currency"].ToString()}};
                            dLayer.ExecuteNonQueryPro("SP_NewAdminCreation", proParams1, connection, transaction);



                            SortedList proParams2 = new SortedList(){
                                        {"N_CompanyID",N_CompanyId},
                                        {"N_FnYearID",N_FnYearId},
                                        {"D_Start",GeneralTable.Rows[0]["d_FromDate"].ToString()},
                                        {"D_End",GeneralTable.Rows[0]["d_ToDate"].ToString()}};
                            N_FnYearId = dLayer.ExecuteScalarPro("SP_FinancialYear_Create", proParams2, connection, transaction);

                            SortedList proParams3 = new SortedList(){
                                        {"N_CompanyID",N_CompanyId},
                                        {"N_FnYearID",N_FnYearId}};
                            dLayer.ExecuteNonQueryPro("SP_AccGruops_Accounts_Create", proParams3, connection, transaction);
                        }
                        SortedList taxParams = new SortedList(){
                                        {"@nCompanyID",N_CompanyId},
                                        {"@nFnYearID",N_FnYearId},
                                        {"@nTaxType",myFunctions.getIntVAL(GeneralTable.Rows[0]["n_TaxType"].ToString())}};
                        dLayer.ExecuteNonQuery("UPDATE Acc_FnYear set N_TaxType=@nTaxType where N_FnYearID=@nFnYearID and N_CompanyID=@nCompanyID", taxParams, connection, transaction);
                        transaction.Commit();

                        return Ok(api.Success("Company successfully saved"));
                    }
                }
            }
            catch (Exception ex)
            {

                return Ok(api.Error(User,ex));
            }
        }

        public static string FixBase64ForImage(string image)
        {
            StringBuilder sbText = new StringBuilder(image, image.Length);
            //sbText.Replace("\r\n", String.Empty);
            //sbText.Replace(" ", String.Empty);
            return sbText.ToString();
        }


    }
}