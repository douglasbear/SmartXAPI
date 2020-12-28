
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

        public Acc_Company(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        //GET api/Company/list
        [AllowAnonymous]
        [HttpGet("list")]
        public ActionResult GetAllCompanys()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select N_CompanyId as nCompanyId,X_CompanyName as xCompanyName,X_CompanyCode as xCompanyCode from Acc_Company where B_Inactive =@p1 order by X_CompanyName";
            Params.Add("@p1", 0);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(api.Error(e));
            }

        }

  [HttpGet("details")]
        public ActionResult GetCompanyInfo(int nCompanyID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList Output =new SortedList();

            string sqlCommandText = "select * from Acc_Company where B_Inactive =@p1 and N_CompanyID=@p2";
            Params.Add("@p1", 0);
            Params.Add("@p2", nCompanyID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);




            DataTable AdminInfo = dLayer.ExecuteDataTable("Select N_UserID,X_UserID as x_AdminName from Sec_User Inner Join Sec_UserCategory on Sec_User.N_UserCategoryID= Sec_UserCategory.N_UserCategoryID and X_UserCategory ='Administrator' and Sec_User.X_UserID='Admin' and Sec_User.N_CompanyID=Sec_UserCategory.N_CompanyID  and Sec_User.N_CompanyID=@p2",Params,connection);

            DataTable FnYearInfo = dLayer.ExecuteDataTable("Select D_Start as 'd_FromDate',D_End as 'd_ToDate',N_FnYearID, (select top 1 N_FnYearID from vw_CheckTransaction Where N_FnYearID = Acc_FnYear.N_FnYearID and N_CompanyID = Acc_FnYear.N_CompanyID) As 'TransAction' from Acc_FnYear Where ('" + myFunctions.getDateVAL(myFunctions.GetFormatedDate(myCompanyID._SystemDate)) + "' between D_Start and D_End) and  N_CompanyID=@p2", Params,connection);
            if (FnYearInfo.Rows.Count == 0)
            {
                FnYearInfo = dLayer.ExecuteDataTable("Select D_Start as 'd_FromDate',D_End as 'd_ToDate',N_FnYearID,0 as 'TransAction' from Acc_FnYear Where ('" + myFunctions.getDateVAL(myFunctions.GetFormatedDate(myCompanyID._SystemDate)) + "' between D_Start and D_End) and  N_CompanyID=@p2", Params,connection);
            }

            Output.Add("CompanyInfo",dt);
            Output.Add("AdminInfo",AdminInfo);
            Output.Add("FnYearInfo",FnYearInfo);

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
                return Ok(api.Error(e));
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
                return Ok(api.Error(ex));
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
                    string logo= MasterTable.Rows[0]["i_Logo"].ToString();
                    string footer= MasterTable.Rows[0]["i_Footer"].ToString();
                    string header= MasterTable.Rows[0]["i_Header"].ToString();

                    Byte[] logoBitmap = new Byte[logo.Length];
                    Byte[] footerBitmap = new Byte[footer.Length];
                    Byte[] headerBitmap = new Byte[header.Length];

                    logoBitmap = Convert.FromBase64String(logo);
                    footerBitmap = Convert.FromBase64String(footer);
                    headerBitmap = Convert.FromBase64String(header);

                        MasterTable.Columns.Remove("i_Logo");
                        MasterTable.Columns.Remove("i_Footer");
                        MasterTable.Columns.Remove("i_Header");
                        MasterTable.AcceptChanges();

                    int N_CompanyId = dLayer.SaveData("Acc_Company", "N_CompanyID", MasterTable, connection, transaction);
                    if (N_CompanyId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Warning("Unable to save"));
                    }
                    else
                    {
                        if(logo.Length>0)
                        dLayer.SaveImage("Acc_Company","I_Logo",logoBitmap,"N_CompanyID",N_CompanyId,connection,transaction);
                        if(footer.Length>0)
                        dLayer.SaveImage("Acc_Company","i_Footer",footerBitmap,"N_CompanyID",N_CompanyId,connection,transaction);
                        if(header.Length>0)
                        dLayer.SaveImage("Acc_Company","i_Header",headerBitmap,"N_CompanyID",N_CompanyId,connection,transaction);

                            if (values == "@Auto")
                            {
                                SortedList proParams1 = new SortedList(){
                                        {"N_CompanyID",N_CompanyId},
                                        {"X_ModuleCode","500"},
                                        {"N_UserID",0},
                                        {"X_AdminName",GeneralTable.Rows[0]["x_AdminName"].ToString()},
                                        {"X_AdminPwd",myFunctions.EncryptString(GeneralTable.Rows[0]["x_AdminPwd"].ToString())},
                                        {"X_Currency",MasterTable.Rows[0]["x_Currency"].ToString()}};
                                dLayer.ExecuteNonQueryPro("SP_NewAdminCreation", proParams1, connection, transaction);

                                object N_FnYearId = 0;
            
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

                        transaction.Commit();
                        
                        return Ok(api.Success("Company successfully saved"));
                    }
                }
            }
            catch (Exception ex)
            {

                return Ok(api.Error(ex));
            }
        }

        public static string FixBase64ForImage(string image) {
    StringBuilder sbText = new StringBuilder(image, image.Length);
    //sbText.Replace("\r\n", String.Empty);
    //sbText.Replace(" ", String.Empty);
    return sbText.ToString();
}


    }
}