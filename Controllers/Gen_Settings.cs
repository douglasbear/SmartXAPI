using System.Collections.Generic;
using SmartxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

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

        public Gen_Settings(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("invSettingsDetails")]
        public ActionResult GetDetails(int nFnYearID,int nLangID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    DataTable QList = myFunctions.GetSettingsTable();
                    QList.Rows.Add("Inventory", "Default Item Class");
                    QList.Rows.Add("Inventory", "Default Item Unit");
                    QList.Rows.Add("Inventory", "Default Item Category" );
                    QList.Rows.Add("65", "Enable Selling Price In Opening Stock");
                    QList.Rows.Add("506", "Enable Qty On Hand In Inventory Adjustment");
                    QList.Rows.Add("367", "ShowStkQtyInUC");
                    QList.Rows.Add("Inventory", "EnablePattern");
                    QList.Rows.Add("556", "IsPartNoEnable");
                    QList.Rows.Add("53", "Enable Product Category In Product Maintenance");
                    QList.Rows.Add("88", "Enable Current Stock In Opening Stock");
                    QList.Rows.Add("729", "ShowTrckInDN");
                    QList.Rows.Add("556", "IdDelDaysingrid");
                    QList.Rows.Add("556", "IsRemarksingrid");
                    QList.Rows.Add("345", "EnableAlternativeProduct");
                    QList.Rows.Add("345", "EnableWasteQuantity");
                    QList.Rows.Add("345", "EnableRecycleQuantity");
                    QList.Rows.Add("Item", "EnablePattern");


                    QList.Rows.Add("DEFAULT_ACCOUNTS", "I Material Transfer Account");
                    QList.Rows.Add("DEFAULT_ACCOUNTS", "I OB Account");

                    QList.AcceptChanges();

                    DataTable Details = dLayer.ExecuteSettingsPro("SP_GenSettings_Disp", QList, myFunctions.GetCompanyID(User),nFnYearID, connection);

                    SortedList mParamsList = new SortedList()
                    {
                        {"N_CompanyID",myFunctions.GetCompanyID(User)},
                        {"N_FnYearID",nFnYearID},
                        {"N_LanguageID",nLangID},
                        {"N_UserCategoryID",myFunctions.GetUserCategory(User)},
                        {"N_ParentmenuID",311}
                    };

                        DataTable MasterTable = dLayer.ExecuteDataTablePro("SP_InvInvoiceCounter_Disp", mParamsList, connection);
                        SortedList OutPut = new SortedList(){
                            {"Settings",_api.Format(Details)},
                            {"InvoiceCounter",_api.Format(MasterTable)}
                        };
                    return Ok(_api.Success(OutPut));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }


        [HttpGet("allSettings")]
        public ActionResult GetAllSettingsDetails(int nFnYearID,int nBranchID)
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
                    string sql ="select X_Group,X_Description as name,N_Value,X_Value,N_UserCategoryID from Gen_Settings where N_CompanyID=@nCompanyID group by X_Group,X_Description,N_Value,X_Value,N_UserCategoryID union all "+
                                "SELECT Acc_AccountDefaults.X_FieldDescr as X_Group, vw_AccMastLedger.Account as name, vw_AccMastLedger.N_LedgerID as N_Value, vw_AccMastLedger.[Account Code] as X_Value ,0 as N_UserCategoryID  "+
                                "FROM vw_AccMastLedger INNER JOIN Acc_AccountDefaults ON vw_AccMastLedger.N_CompanyID = Acc_AccountDefaults.N_CompanyID AND vw_AccMastLedger.N_LedgerID = Acc_AccountDefaults.N_FieldValue AND   "+
                                "vw_AccMastLedger.N_FnYearID = Acc_AccountDefaults.N_FnYearID  Where vw_AccMastLedger.N_CompanyID=@nCompanyID and vw_AccMastLedger.N_FnYearID=@nFnYearID and vw_AccMastLedger.B_Inactive=0 ";

                    DataTable Details = dLayer.ExecuteDataTable(sql, mParamsList, connection);
                    return Ok(_api.Success(_api.Format(Details)));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        

                //Save....
        [HttpPost("saveInventorySettings")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable InvoiceCounter = ds.Tables["invoiceCounter"];
                DataTable Settings = ds.Tables["settings"];
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    transaction.Commit();
                }
                return Ok(_api.Success("Saved"));

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

 [HttpGet("checkScreenAccess")]
        public ActionResult GetFormAccess(int nFormID)
        {
            bool Allowed=false;
            SortedList Out =new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Allowed = myFunctions.CheckPermission(myFunctions.GetCompanyID(User), nFormID, myFunctions.GetUserCategory(User).ToString(),"N_UserCategoryID", dLayer, connection);
                    Out.Add("Allowed",Allowed);
                    Out.Add("FormID",nFormID);
                    return Ok(_api.Success(Out));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }

        }
       



        [HttpGet("settingsList")]
        public ActionResult GetAllItems()
        {
            DataTable dt =new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable("select  REPLACE(UPPER(X_Description+'_'+ X_Group),' ','_') as Name,X_Description,X_Group from Gen_Settings where X_Description is not null  group by X_Group,X_Description "+
                    " UNION ALL select  REPLACE(UPPER(X_FieldDescr),' ','_') as Name,X_FieldDescr as X_Description,'' as X_Group from Acc_AccountDefaults group by X_FieldDescr", connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    SortedList Out =new SortedList();
                     foreach (DataRow var in dt.Rows)
                    {
                        SortedList child = new SortedList();
                        child.Add("name",var["X_Description"]);
                        child.Add("x_Group",var["X_Group"]);
                        Out.Add(var["Name"].ToString().ToUpper(),child);
                    }
                    var json = JsonConvert.SerializeObject(Out);
                    return Ok(json);
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }

        }

        [HttpGet("saveSettings")]
        public ActionResult SaveSettingsData(int nFormID, string xDescription, int nValue, string xValue)
        {
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
                    ParamSettings_Ins.Add("N_Value",nValue);
                    ParamSettings_Ins.Add("X_Value",xValue);

                    try
                    {
                        dLayer.ExecuteNonQueryPro("SP_GeneralDefaults_ins", ParamSettings_Ins, connection, transaction);
                        return Ok(_api.Success("Terms & Conditions Saved"));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save!"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }



        



    }


}