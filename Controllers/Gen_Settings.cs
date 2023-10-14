using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("genSettings")]
    [ApiController]
    public class Gen_Settings : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly string logPath;
        private readonly IWebHostEnvironment env;
        private readonly string appUpdateVersion;

        public Gen_Settings(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IWebHostEnvironment envn)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            env = envn;
            connectionString = conf.GetConnectionString("SmartxConnection");
            logPath = conf.GetConnectionString("LogPath");
            appUpdateVersion = conf.GetConnectionString("AppUpdateVersion");
        }

        [HttpGet("settingsDetails")]
        public ActionResult GetDetails(int nFnYearID, int nLangID, int nFormID, int nCompanyID, int nBranchID, int nDivisionID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SortedList Params = new SortedList();
                    DataTable OffDays = new DataTable();
                    DataTable payrollDetails = new DataTable();
                    Params.Add("@nFormID", nFormID);
                    Params.Add("@nLangID", nLangID);
                    Params.Add("@nFnYearID", nFnYearID);
                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));

                    string settingsSql = "";
                    string defaultAccountsSql = "";
                    string offDaysSql = "";
                    string payrollSql = "";

                    if (env.EnvironmentName == "Development")
                    {
                        settingsSql = "SELECT ROW_NUMBER() OVER(ORDER BY Gen_Settings.X_Group,Gen_Settings.X_Description,Gen_Settings.N_UserCategoryID ASC) AS N_RowID,Gen_Settings.X_Group, Gen_Settings.X_Description, isnull(max(Gen_Settings.N_Value),0) as N_Value, Gen_Settings.X_Value, Gen_Settings.N_UserCategoryID, Gen_Settings.X_FieldType, Gen_Settings.X_SettingsTabCode,Lan_MultiLingual.X_WText + ' [ '+Gen_Settings.X_Description+' - ' + Gen_Settings.X_Group + ' ]' as X_WText,Gen_Settings.X_DataSource FROM Gen_Settings LEFT OUTER JOIN Lan_MultiLingual ON Gen_Settings.N_SettingsFormID = Lan_MultiLingual.N_FormID AND Gen_Settings.X_WLanControlNo = Lan_MultiLingual.X_WControlName WHERE (Gen_Settings.B_WShow = 1) AND (Gen_Settings.N_SettingsFormID = @nFormID) AND (Gen_Settings.N_CompanyID = @nCompanyID) and (Lan_MultiLingual.N_LanguageId=@nLangID) group by Gen_Settings.X_Group,Gen_Settings.X_Description, Gen_Settings.X_Value, Gen_Settings.N_UserCategoryID, Gen_Settings.X_FieldType, Gen_Settings.X_SettingsTabCode,Lan_MultiLingual.X_WText,Gen_Settings.X_DataSource,Gen_Settings.N_Order order by X_SettingsTabCode,N_Order,N_UserCategoryID";
                        defaultAccountsSql = "SELECT Acc_AccountDefaults.X_FieldDescr as X_Group, Acc_AccountDefaults.X_Type AS X_Type,vw_AccMastLedger.Account  as name, vw_AccMastLedger.N_LedgerID  as N_Value, vw_AccMastLedger.[Account Code] as X_Value, Acc_AccountDefaults.N_CompanyID, Acc_AccountDefaults.N_FieldValue, Acc_AccountDefaults.N_Type, Acc_AccountDefaults.N_FnYearID, Acc_AccountDefaults.D_Entrydate, Acc_AccountDefaults.N_BranchID, Acc_AccountDefaults.N_FormID, Acc_AccountDefaults.X_WLanControlNo, Acc_AccountDefaults.N_Order, Acc_AccountDefaults.X_AccountCriteria, Lan_MultiLingual.X_WText + ' [ '+ Acc_AccountDefaults.X_FieldDescr + ' ]' as X_WText FROM Acc_AccountDefaults LEFT OUTER JOIN Lan_MultiLingual ON Acc_AccountDefaults.X_WLanControlNo = Lan_MultiLingual.X_WControlName AND Acc_AccountDefaults.N_FormID = Lan_MultiLingual.N_FormID LEFT OUTER JOIN vw_AccMastLedger ON Acc_AccountDefaults.N_FnYearID = vw_AccMastLedger.N_FnYearID AND Acc_AccountDefaults.N_CompanyID = vw_AccMastLedger.N_CompanyID AND Acc_AccountDefaults.N_FieldValue = vw_AccMastLedger.N_LedgerID WHERE (Acc_AccountDefaults.N_FormID = @nFormID) AND (Acc_AccountDefaults.N_CompanyID = @nCompanyID) AND (Acc_AccountDefaults.N_FnYearID = @nFnYearID) AND (Lan_MultiLingual.N_LanguageID = @nLangID) and (Acc_AccountDefaults.X_Type is null or Acc_AccountDefaults.X_Type='account')  union SELECT Acc_AccountDefaults.X_FieldDescr as X_Group, Acc_AccountDefaults.X_Type AS X_Type,vw_AccMastGroup.X_GroupName  as name,vw_AccMastGroup.N_GroupID  as N_Value, vw_AccMastGroup.X_GroupCode as X_Value, Acc_AccountDefaults.N_CompanyID, Acc_AccountDefaults.N_FieldValue, Acc_AccountDefaults.N_Type,  Acc_AccountDefaults.N_FnYearID, Acc_AccountDefaults.D_Entrydate, Acc_AccountDefaults.N_BranchID,  Acc_AccountDefaults.N_FormID, Acc_AccountDefaults.X_WLanControlNo, Acc_AccountDefaults.N_Order, Acc_AccountDefaults.X_AccountCriteria, Lan_MultiLingual.X_WText + ' [ '+ Acc_AccountDefaults.X_FieldDescr + ' ]' as X_WText FROM Acc_AccountDefaults LEFT OUTER JOIN Lan_MultiLingual ON Acc_AccountDefaults.X_WLanControlNo = Lan_MultiLingual.X_WControlName  AND Acc_AccountDefaults.N_FormID = Lan_MultiLingual.N_FormID LEFT OUTER JOIN vw_AccMastGroup ON Acc_AccountDefaults.N_FnYearID = vw_AccMastGroup.N_FnYearID AND Acc_AccountDefaults.N_CompanyID = vw_AccMastGroup.N_CompanyID  AND Acc_AccountDefaults.N_FieldValue = vw_AccMastGroup.N_GroupID WHERE (Acc_AccountDefaults.N_FormID = 1380) AND (Acc_AccountDefaults.N_CompanyID = 46)AND (Acc_AccountDefaults.N_FnYearID = 111) AND (Lan_MultiLingual.N_LanguageID = 1) and (Acc_AccountDefaults.X_Type='group')order by N_Order";

                    }
                    else
                    {
                        settingsSql = "SELECT ROW_NUMBER() OVER(ORDER BY Gen_Settings.X_Group,Gen_Settings.X_Description,Gen_Settings.N_UserCategoryID ASC) AS N_RowID,Gen_Settings.X_Group, Gen_Settings.X_Description, isnull(max(Gen_Settings.N_Value),0) as N_Value, Gen_Settings.X_Value, Gen_Settings.N_UserCategoryID, Gen_Settings.X_FieldType, Gen_Settings.X_SettingsTabCode,Lan_MultiLingual.X_WText,Gen_Settings.X_DataSource FROM Gen_Settings LEFT OUTER JOIN Lan_MultiLingual ON Gen_Settings.N_SettingsFormID = Lan_MultiLingual.N_FormID AND Gen_Settings.X_WLanControlNo = Lan_MultiLingual.X_WControlName WHERE (Gen_Settings.B_WShow = 1) AND (Gen_Settings.N_SettingsFormID = @nFormID) AND (Gen_Settings.N_CompanyID = @nCompanyID) and (Lan_MultiLingual.N_LanguageId=@nLangID) group by Gen_Settings.X_Group,Gen_Settings.X_Description, Gen_Settings.X_Value, Gen_Settings.N_UserCategoryID, Gen_Settings.X_FieldType, Gen_Settings.X_SettingsTabCode,Lan_MultiLingual.X_WText,Gen_Settings.X_DataSource,Gen_Settings.N_Order order by X_SettingsTabCode,N_Order,N_UserCategoryID";
                        defaultAccountsSql = "SELECT Acc_AccountDefaults.X_FieldDescr as X_Group, Acc_AccountDefaults.X_Type AS X_Type,vw_AccMastLedger.Account  as name, vw_AccMastLedger.N_LedgerID  as N_Value, vw_AccMastLedger.[Account Code] as X_Value, Acc_AccountDefaults.N_CompanyID, Acc_AccountDefaults.N_FieldValue, Acc_AccountDefaults.N_Type, Acc_AccountDefaults.N_FnYearID, Acc_AccountDefaults.D_Entrydate, Acc_AccountDefaults.N_BranchID, Acc_AccountDefaults.N_FormID, Acc_AccountDefaults.X_WLanControlNo, Acc_AccountDefaults.N_Order, Acc_AccountDefaults.X_AccountCriteria, Lan_MultiLingual.X_WText + ' [ '+ Acc_AccountDefaults.X_FieldDescr + ' ]' as X_WText FROM Acc_AccountDefaults LEFT OUTER JOIN Lan_MultiLingual ON Acc_AccountDefaults.X_WLanControlNo = Lan_MultiLingual.X_WControlName AND Acc_AccountDefaults.N_FormID = Lan_MultiLingual.N_FormID LEFT OUTER JOIN vw_AccMastLedger ON Acc_AccountDefaults.N_FnYearID = vw_AccMastLedger.N_FnYearID AND Acc_AccountDefaults.N_CompanyID = vw_AccMastLedger.N_CompanyID AND Acc_AccountDefaults.N_FieldValue = vw_AccMastLedger.N_LedgerID WHERE (Acc_AccountDefaults.N_FormID = @nFormID) AND (Acc_AccountDefaults.N_CompanyID = @nCompanyID) AND (Acc_AccountDefaults.N_FnYearID = @nFnYearID) AND (Lan_MultiLingual.N_LanguageID = @nLangID) and (Acc_AccountDefaults.X_Type is null or Acc_AccountDefaults.X_Type='account')  union SELECT Acc_AccountDefaults.X_FieldDescr as X_Group, Acc_AccountDefaults.X_Type AS X_Type,vw_AccMastGroup.X_GroupName  as name,vw_AccMastGroup.N_GroupID  as N_Value, vw_AccMastGroup.X_GroupCode as X_Value, Acc_AccountDefaults.N_CompanyID, Acc_AccountDefaults.N_FieldValue, Acc_AccountDefaults.N_Type,  Acc_AccountDefaults.N_FnYearID, Acc_AccountDefaults.D_Entrydate, Acc_AccountDefaults.N_BranchID,  Acc_AccountDefaults.N_FormID, Acc_AccountDefaults.X_WLanControlNo, Acc_AccountDefaults.N_Order, Acc_AccountDefaults.X_AccountCriteria, Lan_MultiLingual.X_WText + ' [ '+ Acc_AccountDefaults.X_FieldDescr + ' ]' as X_WText FROM Acc_AccountDefaults LEFT OUTER JOIN Lan_MultiLingual ON Acc_AccountDefaults.X_WLanControlNo = Lan_MultiLingual.X_WControlName  AND Acc_AccountDefaults.N_FormID = Lan_MultiLingual.N_FormID LEFT OUTER JOIN vw_AccMastGroup ON Acc_AccountDefaults.N_FnYearID = vw_AccMastGroup.N_FnYearID AND Acc_AccountDefaults.N_CompanyID = vw_AccMastGroup.N_CompanyID  AND Acc_AccountDefaults.N_FieldValue = vw_AccMastGroup.N_GroupID WHERE (Acc_AccountDefaults.N_FormID = 1380) AND (Acc_AccountDefaults.N_CompanyID = 46)AND (Acc_AccountDefaults.N_FnYearID = 111) AND (Lan_MultiLingual.N_LanguageID = 1) and (Acc_AccountDefaults.X_Type='group')order by N_Order";


                    }

                    if (nFormID == 1475)
                    {
                        offDaysSql = "Select * from pay_YearlyOffDays Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID order by N_OffID";
                        OffDays = dLayer.ExecuteDataTable(offDaysSql, Params, connection);
                    }

                    if (nFormID == 1584)
                    {
                        payrollSql = "select *,N_Payable_LedgerID as N_PayableDefAccountID from vw_Pay_PaymasterAccounts where N_companyID=@nCompanyID and N_FnYearID=@nFnYearID";
                        payrollDetails = dLayer.ExecuteDataTable(payrollSql, Params, connection);
                    }

                    DataTable Settings = dLayer.ExecuteDataTable(settingsSql, Params, connection);
                    DataTable AccountMap = dLayer.ExecuteDataTable(defaultAccountsSql, Params, connection);
                    int NParentMenuId = 0;

                    Settings = myFunctions.AddNewColumnToDataTable(Settings, "listItems", typeof(DataTable), null);

                    foreach (DataRow row in Settings.Rows)
                    {
                        if (row["X_DataSource"] != null && row["X_DataSource"].ToString() != "" && row["X_DataSource"].ToString() != "null" && row["X_DataSource"].ToString() != "0")
                        {
                            string sql = row["X_DataSource"].ToString();
                            SortedList lParamsList = new SortedList()
                            {
                                {"@Cval",myFunctions.GetCompanyID(User)},
                                {"@Fval",nFnYearID},
                                {"@Ctrval",nCompanyID},
                                // {"@Lval",nLangID},
                                // {"@UCval",myFunctions.GetUserCategory(User)}
                            };

                            // StringBuilder sb = new StringBuilder();
                            // sb.AppendLine(settingsSql + " --- " + sql + "  -  " + row["X_Group"] + "---" + row["X_Description"]);
                            // if (!Directory.Exists(logPath))
                            //     Directory.CreateDirectory(logPath);

                            // System.IO.File.AppendAllText(logPath + "Settings Log.log", sb.ToString());
                            // sb.Clear();
                            row["listItems"] = dLayer.ExecuteDataTable(sql, lParamsList, connection);

                        }
                    }
                    Settings.Columns.Remove("X_DataSource");

                    Settings.AcceptChanges();

                    if (nFormID == 1373)
                        NParentMenuId = 311;
                    if (nFormID == 1379)
                        NParentMenuId = 315;
                    if (nFormID == 1380)
                        NParentMenuId = 48;
                    if (nFormID == 1402)
                        NParentMenuId = 133;
                    if (nFormID == 1464)
                        NParentMenuId = 6;
                    if (nFormID == 589)
                        NParentMenuId = 185;
                    if (nFormID == 1475)
                        NParentMenuId = 1372;
                    if (nFormID == 1476)
                        NParentMenuId = 1372;
                    if (nFormID == 1584)
                        NParentMenuId = 285;
                    //   if (nFormID == 1372)
                    // NParentMenuId = 1372;

                    SortedList mParamsList = new SortedList()
                    {
                        {"N_CompanyID",myFunctions.GetCompanyID(User)},
                        {"N_FnYearID",nFnYearID},
                        {"N_LanguageID",nLangID},
                        {"X_UserCategoryIDList",myFunctions.GetUserCategoryList(User)},
                        {"N_ParentmenuID",NParentMenuId},
                         {"N_BranchID",nBranchID},
                         {"N_DivisionID",nDivisionID}
                    };

                    DataTable MasterTable = dLayer.ExecuteDataTablePro("SP_InvInvoiceCounter_Disp_Cloud", mParamsList, connection);
                    SortedList OutPut = new SortedList(){
                            {"Settings",_api.Format(Settings)},
                            {"InvoiceCounter",_api.Format(MasterTable)},
                            {"AccountMap",_api.Format(AccountMap)},
                            {"OffDays",_api.Format(OffDays)},
                            {"payrollDetails",_api.Format(payrollDetails)}
                        };
                    return Ok(_api.Success(OutPut));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        [HttpGet("allSettings")]
        public ActionResult GetAllSettingsDetails(int nFnYearID, int nBranchID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SortedList mParamsList = new SortedList()
                    {
                        {"@nCompanyID",myFunctions.GetCompanyID(User)},
                        {"@nFnYearID",nFnYearID},
                    };
                    string sql = "select X_Group,X_Description as name ,N_Value,X_Value,N_UserCategoryID from Gen_Settings where N_CompanyID=@nCompanyID group by X_Group,X_Description,N_Value,X_Value,N_UserCategoryID union all " +
                                "SELECT Acc_AccountDefaults.X_FieldDescr as X_Group,vw_AccMastLedger.Account as name, vw_AccMastLedger.N_LedgerID as N_Value, vw_AccMastLedger.[Account Code] as X_Value ,0 as N_UserCategoryID  " +
                                "FROM vw_AccMastLedger INNER JOIN Acc_AccountDefaults ON vw_AccMastLedger.N_CompanyID = Acc_AccountDefaults.N_CompanyID AND vw_AccMastLedger.N_LedgerID = Acc_AccountDefaults.N_FieldValue AND   " +
                                "vw_AccMastLedger.N_FnYearID = Acc_AccountDefaults.N_FnYearID  Where vw_AccMastLedger.N_CompanyID=@nCompanyID and vw_AccMastLedger.N_FnYearID=@nFnYearID and vw_AccMastLedger.B_Inactive=0 ";

                    DataTable Details = dLayer.ExecuteDataTable(sql, mParamsList, connection);
                    return Ok(_api.Success(_api.Format(Details)));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }





        [HttpGet("checkScreenAccess")]
        public ActionResult GetFormAccess(int nFormID)
        {
            bool Allowed = false;
            SortedList Out = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Allowed = myFunctions.CheckPermission(myFunctions.GetCompanyID(User), nFormID, myFunctions.GetUserCategory(User).ToString(), "N_UserCategoryID", dLayer, connection);
                    Out.Add("Allowed", Allowed);
                    Out.Add("FormID", nFormID);
                    return Ok(_api.Success(Out));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }

        }




        [HttpGet("settingsList")]
        public ActionResult GetAllItems()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable("select  REPLACE(UPPER(X_Description+'_'+ X_Group),' ','_') as Name,X_Description,X_Group from Gen_Settings where X_Description is not null  group by X_Group,X_Description " +
                    " UNION ALL select  REPLACE(UPPER(X_FieldDescr),' ','_') as Name,X_FieldDescr as X_Description,'' as X_Group from Acc_AccountDefaults group by X_FieldDescr", connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    SortedList Out = new SortedList();
                    foreach (DataRow var in dt.Rows)
                    {
                        SortedList child = new SortedList();
                        child.Add("name", var["X_Description"]);
                        child.Add("x_Group", var["X_Group"]);
                        Out.Add(var["Name"].ToString().ToUpper(), child);
                    }
                    var json = JsonConvert.SerializeObject(Out);
                    return Ok(json);
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }

        }

        [HttpGet("saveSettings")]
        public ActionResult SaveSettingsData(int nFormID, string xDescription, int nValue, string xValue, string xUerCategory)
        {
            if (xValue == null)
                xValue = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    SortedList ParamSettings_Ins = new SortedList();

                    ParamSettings_Ins.Add("N_CompanyID", nCompanyID);
                    ParamSettings_Ins.Add("X_Group", nFormID);
                    ParamSettings_Ins.Add("X_Description", xDescription);
                    ParamSettings_Ins.Add("N_Value", nValue);
                    ParamSettings_Ins.Add("X_Value", xValue);
                    ParamSettings_Ins.Add("X_UerCategory", xUerCategory);

                    try
                    {
                        dLayer.ExecuteNonQueryPro("SP_GeneralDefaults_ins", ParamSettings_Ins, connection, transaction);
                        transaction.Commit();
                        return Ok(_api.Success("saved successfully"));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save!"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }



        [HttpPost("saveGeneralSettings")]
        public ActionResult SaveGeneralSettings([FromBody] DataSet ds)
        {

            DataTable GenSettinngs = ds.Tables["genSettings"];
            DataTable InvoiceCounter = ds.Tables["invoiceCounter"];
            DataTable AccountMaps = ds.Tables["accountMaps"];
            DataTable General = ds.Tables["general"];
            DataTable OffDays = ds.Tables["offdays"];
            DataTable SalaryExpense = ds.Tables["salaryExpense"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    int nFnYearID = myFunctions.getIntVAL(General.Rows[0]["n_FnYearID"].ToString());
                    int nBranchID = myFunctions.getIntVAL(General.Rows[0]["n_BranchID"].ToString());
                    int nDivisionID = 0;
                    if (General.Columns.Contains("n_DivisionID"))
                    {
                        nDivisionID = myFunctions.getIntVAL(General.Rows[0]["n_DivisionID"].ToString());
                    }

                    int nCompanyID = myFunctions.GetCompanyID(User);


                    if (nDivisionID > 0)
                    {

                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("N_CompanyID");
                        dt.Columns.Add("N_FormID");
                        dt.Columns.Add("X_Prefix");
                        dt.Columns.Add("N_StartNo");
                        dt.Columns.Add("N_LastUsedNo");
                        dt.Columns.Add("B_AutoInvoiceEnabled");
                        dt.Columns.Add("N_MenuID");
                        dt.Columns.Add("N_FnYearID");
                        dt.Columns.Add("B_YearWise");
                        dt.Columns.Add("N_BranchID");
                        dt.Columns.Add("N_MinimumLen");
                        dt.Columns.Add("B_Suffix");
                        dt.Columns.Add("X_Suffix");
                        dt.Columns.Add("B_ResetYearly");
                        dt.Columns.Add("X_Type");
                        dt.Columns.Add("X_Type2");
                        dt.Columns.Add("N_DivisionID");
                        int BEnabled = 0;
                        foreach (DataRow var in InvoiceCounter.Rows)
                        {
                            object divisionCount = dLayer.ExecuteScalar("select Count(1) from Inv_InvoiceCounter where N_CompanyID=" + nCompanyID + " and N_BranchID=" + nBranchID + " and N_DivisionID=" + nDivisionID + " and N_MenuID=" + myFunctions.getVAL(var["n_FormID"].ToString()) + "", connection, transaction);
                            if (myFunctions.getIntVAL(divisionCount.ToString()) == 0)
                            {

                                if (Convert.ToBoolean(var["Enabled"].ToString()))
                                    BEnabled = 1;

                                DataRow row = dt.NewRow();
                                row["N_CompanyID"] = nCompanyID;
                                row["N_FormID"] = myFunctions.getVAL(var["n_FormID"].ToString());
                                row["X_Prefix"] = var["prefix"].ToString();
                                row["N_StartNo"] = myFunctions.getVAL(var["startingNo"].ToString());
                                row["N_LastUsedNo"] = myFunctions.getVAL(var["StartingNo"].ToString()) - 1;
                                row["B_AutoInvoiceEnabled"] = BEnabled;
                                row["N_MenuID"] = myFunctions.getVAL(var["n_FormID"].ToString());
                                row["N_FnYearID"] = nFnYearID;
                                row["B_YearWise"] = 0;
                                row["N_BranchID"] = nBranchID;
                                row["N_MinimumLen"] = var["MinLength"].ToString();
                                row["B_Suffix"] = 0;
                                row["X_Suffix"] = var["Suffix"].ToString();
                                row["B_ResetYearly"] = 0;
                                row["X_Type"] = "";
                                row["X_Type2"] = "";
                                row["N_DivisionID"] = nDivisionID;
                                dt.Rows.Add(row);
                            }
                            else

                            {
                                int Enabled = 0, Reset = 0;
                                if (Convert.ToBoolean(var["Enabled"].ToString()))
                                    Enabled = 1;

                                Reset = 0;

                                if (myFunctions.getVAL(var["StartingNo"].ToString()) > myFunctions.getVAL(var["LastUsedNo"].ToString()))
                                    dLayer.ExecuteNonQuery("update Inv_InvoiceCounter set N_StartNo=" + myFunctions.getVAL(var["StartingNo"].ToString()).ToString() + ",N_LastUsedNo=" + (myFunctions.getVAL(var["StartingNo"].ToString()) - 1) + ",X_Prefix='" + var["Prefix"].ToString() + "',X_Suffix='" + var["Suffix"].ToString() + "',N_MinimumLen='" + var["MinLength"].ToString() + "',B_AutoInvoiceEnabled=" + Enabled.ToString() + ",B_ResetYearly=" + Reset.ToString() + " Where N_FormID= " + var["N_FormID"].ToString() + " and N_CompanyId=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_BranchID=" + nBranchID + " and N_DivisionID=" + nDivisionID, connection, transaction);
                                else
                                    dLayer.ExecuteNonQuery("update Inv_InvoiceCounter set X_Prefix='" + var["Prefix"].ToString() + "',X_Suffix='" + var["Suffix"].ToString() + "',N_MinimumLen='" + var["MinLength"].ToString() + "',B_AutoInvoiceEnabled=" + Enabled.ToString() + ",B_ResetYearly=" + Reset.ToString() + ",N_LastUsedNo=" + myFunctions.getVAL(var["lastUsedNo"].ToString()) + " Where N_FormID= " + var["N_FormID"].ToString() + " and N_CompanyId=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_BranchID=" + nBranchID + "  and N_DivisionID=" + nDivisionID, connection, transaction);



                            }
                            if (dt.Rows.Count > 0)
                            {

                                dLayer.ExecuteNonQuery("insert into Inv_InvoiceCounter(N_CompanyID,N_FormID,X_Prefix,N_StartNo,N_LastUsedNo,B_AutoInvoiceEnabled,N_MenuID,N_FnYearID,B_YearWise,N_BranchID,N_MinimumLen,B_Suffix,X_Suffix,B_ResetYearly,X_Type,X_Type2,N_DivisionID)values(" + nCompanyID + "," + myFunctions.getVAL(var["n_FormID"].ToString()) + ",'" + var["prefix"].ToString() + "'," + myFunctions.getVAL(var["startingNo"].ToString()) + "," + (myFunctions.getVAL(var["StartingNo"].ToString()) - 1) + "," + BEnabled + "," + myFunctions.getVAL(var["n_FormID"].ToString()) + "," + nFnYearID + ",0," + nBranchID + "," + var["MinLength"].ToString() + ",0,'" + var["Suffix"].ToString() + "',0,'',''," + nDivisionID + ")", connection, transaction);
                            }
                        }


                    }
                    else
                    {


                        foreach (DataRow Kvar in InvoiceCounter.Rows)
                        {
                            int Enabled = 0, Reset = 0;
                            if (Convert.ToBoolean(Kvar["Enabled"].ToString()))
                                Enabled = 1;

                            Reset = 0;

                            if (myFunctions.getVAL(Kvar["StartingNo"].ToString()) > myFunctions.getVAL(Kvar["LastUsedNo"].ToString()))
                                dLayer.ExecuteNonQuery("update Inv_InvoiceCounter set N_StartNo=" + myFunctions.getVAL(Kvar["StartingNo"].ToString()).ToString() + ",N_LastUsedNo=" + (myFunctions.getVAL(Kvar["StartingNo"].ToString()) - 1) + ",X_Prefix='" + Kvar["Prefix"].ToString() + "',X_Suffix='" + Kvar["Suffix"].ToString() + "',N_MinimumLen='" + Kvar["MinLength"].ToString() + "',B_AutoInvoiceEnabled=" + Enabled.ToString() + ",B_ResetYearly=" + Reset.ToString() + " Where N_FormID= " + Kvar["N_FormID"].ToString() + " and N_CompanyId=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_BranchID=" + nBranchID, connection, transaction);
                            else
                                dLayer.ExecuteNonQuery("update Inv_InvoiceyCounter set X_Prefix='" + Kvar["Prefix"].ToString() + "',X_Suffix='" + Kvar["Suffix"].ToString() + "',N_MinimumLen='" + Kvar["MinLength"].ToString() + "',B_AutoInvoiceEnabled=" + Enabled.ToString() + ",B_ResetYearly=" + Reset.ToString() + ",N_LastUsedNo=" + myFunctions.getVAL(Kvar["lastUsedNo"].ToString()) + " Where N_FormID= " + Kvar["N_FormID"].ToString() + " and N_CompanyId=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_BranchID=" + nBranchID, connection, transaction);

                        }
                    }

                    foreach (DataRow var in GenSettinngs.Rows)
                    {
                        string settingsSql = "SP_GeneralDefaults_ins_cloud " + nCompanyID + ",'" + var["x_Group"].ToString() + "','" + var["x_Description"].ToString() + "' ," + myFunctions.getIntVAL(var["n_Value"].ToString()) + ",'" + var["x_Value"].ToString() + "'," + var["n_UserCategoryID"].ToString();
                        dLayer.ExecuteNonQuery(settingsSql, connection, transaction);
                    }

                    foreach (DataRow var in AccountMaps.Rows)
                    {
                        int b_IsDefault = AccountMaps.Columns.Contains("b_IsDefault") ? myFunctions.getIntVAL(AccountMaps.Rows[0]["b_IsDefault"].ToString()) : 0;
                        int nType = 0;
                        if (var["x_Type"].ToString() == "Group")
                        {
                            nType = 1;
                        }
                        else
                        {
                            nType = 0;
                        }
                        string defaultsSql = "SP_AccountDefaults_ins " + nCompanyID + ",'" + var["x_Group"].ToString() + "','" + var["x_Value"].ToString() + "'," + nFnYearID + "," + nType + "";
                        dLayer.ExecuteNonQuery(defaultsSql, connection, transaction);

                        if (b_IsDefault == 1)
                        {
                            dLayer.ExecuteNonQuery("update Acc_PaymentMethodMaster set B_IsDefault=0 where N_CompanyID=" + nCompanyID + "", connection, transaction);
                            dLayer.ExecuteNonQuery("update Acc_PaymentMethodMaster set B_IsDefault=1 where N_CompanyID=" + nCompanyID + " and N_TypeID= " + var["n_TypeID"].ToString() + "and N_PaymentMethodID=" + var["n_PaymentMethodID"].ToString() + "", connection, transaction);
                        }
                    }
                    if (OffDays != null)
                    {
                        object N_OffID = 0;
                        N_OffID = dLayer.SaveData("pay_YearlyOffDays", "N_OffID", OffDays, connection, transaction);
                    }
                    if (SalaryExpense != null)
                    {
                        foreach (DataRow var in SalaryExpense.Rows)
                        {
                            dLayer.ExecuteNonQuery("Update Pay_Paymaster  Set N_ExpenseDefAccountID = " + myFunctions.getIntVAL(var["N_ExpenseDefAccountID"].ToString()) + " ,  N_PayableDefAccountID = " + myFunctions.getIntVAL(var["N_PayableDefAccountID"].ToString()) + "Where N_PayID=" + myFunctions.getIntVAL(var["N_PayID"].ToString()) + " and N_CompanyID=" + myFunctions.getIntVAL(var["N_CompanyID"].ToString()) + " and N_FnYearID=" + myFunctions.getIntVAL(var["N_FnYearID"].ToString()), connection, transaction);
                        }
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Settings Saved"));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("defaultAc")]
        public ActionResult GetDefaultAcDetails(int nFnYearID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select Account as X_LedgerName,[Account Code] as X_LedgerCode,* from vw_DefaultAccount_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 ";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }

                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpPost("saveDefaultAccounts")]
        public ActionResult SaveDefaultAccounts([FromBody] DataSet ds)
        {

            DataTable AccountMaps = ds.Tables["accountMaps"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    int nFnYearID = myFunctions.getIntVAL(AccountMaps.Rows[0]["n_FnYearID"].ToString());
                    int nCompanyID = myFunctions.GetCompanyID(User);

                    foreach (DataRow var in AccountMaps.Rows)
                    {
                        bool bIsDefault = myFunctions.getBoolVAL(var["b_IsDefault"].ToString());

                        string defaultsSql = "SP_AccountDefaults_ins " + nCompanyID + ",'" + var["x_Group"].ToString() + "','" + var["x_Value"].ToString() + "'," + nFnYearID + "," + var["n_PaymentMethodID"].ToString() + "";
                        dLayer.ExecuteNonQuery(defaultsSql, connection, transaction);

                        if (bIsDefault == true)
                        {
                            dLayer.ExecuteNonQuery("update Acc_PaymentMethodMaster set B_IsDefault=0 where N_CompanyID=" + nCompanyID + "", connection, transaction);
                            dLayer.ExecuteNonQuery("update Acc_PaymentMethodMaster set B_IsDefault=1 where N_CompanyID=" + nCompanyID + " and N_TypeID= " + var["n_TypeID"].ToString() + "and N_PaymentMethodID=" + var["n_PaymentMethodID"].ToString() + "", connection, transaction);
                        }
                    }
                    transaction.Commit();

                    return Ok(_api.Success("Default Accounts Saved"));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        [HttpGet("appupdateversion")]
        public ActionResult GetAppupdateVersion()
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            dt.Columns.Add("appUpdateVersion");
            DataRow row = dt.NewRow();
            row["appUpdateVersion"] = appUpdateVersion;
            dt.Rows.Add(row);
            try
            {
                return Ok(_api.Success(dt));

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }





    }


}