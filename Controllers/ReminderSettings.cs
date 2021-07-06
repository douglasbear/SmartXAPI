using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("reminderSettings")]
    [ApiController]

    public class ReminderSettingsController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public ReminderSettingsController(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 978;
        }

        [HttpGet("list")]
        public ActionResult GetReminderSettingsList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@p1", nCompanyID);
            string sqlCommandText = "select * from Gen_ReminderSettings where N_CompanyID=@p1";
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
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("module")]
        public ActionResult GetModuleList(int nLanguageId, string xUserCategory)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nLanguageId);
            Params.Add("@p3", xUserCategory);
            string sqlCommandText = "";
            if (xUserCategory=="Olivo") {
                sqlCommandText = "select N_MenuID as N_ModuleID,X_Module from vw_UserMenus_List where N_CompanyID=@p1 and N_LanguageId=@p2 and N_ParentMenuID=0 and X_ControlNo='0' group by N_MenuID,X_Module";
            } else {
                sqlCommandText = "select N_MenuID as N_ModuleID,X_Module from vw_UserMenus_List where N_CompanyID=@p1 and N_LanguageId=@p2 and X_UserCategory=@p3 and N_ParentMenuID=0 and X_ControlNo='0' group by N_MenuID,X_Module";
            }
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
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("screen")]
        public ActionResult GetScreenList(int nLanguageId, int nModuleID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1", nLanguageId);
            Params.Add("@p2", nModuleID);
            string sqlCommandText = "";
            if (nModuleID > 0) {
                sqlCommandText = "select * from vw_ReminderSettingsScreen where N_LanguageId=@p1 and N_ParentMenuID=@p2 order by N_MenuID";
            } else {
                sqlCommandText = "select * from vw_ReminderSettingsScreen where N_LanguageId=@p1 order by N_MenuID";
            }
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
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("field")]
        public ActionResult GetFieldList(int nLanguageId, int nMenuID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nLanguageId);
            Params.Add("@p3", nMenuID);
            string sqlCommandText = "select * from vw_Gen_ReminderFields where N_CompanyID=@p1 and N_LanguageId=@p2 and N_FormID=@p3";
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
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("category")]
        public ActionResult GetCategoryList(int nLanguageId, int nMenuID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@p1", nCompanyID);
            string sqlCommandText = "select * from Dms_ReminderCategory where N_CompanyID=@p1";
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
                return Ok(_api.Error(e));
            }
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    object Result = 0;
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    string xSubject = MasterTable.Rows[0]["x_Subject"].ToString();
                    int nCategoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CategoryID"].ToString());
                    int nID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ID"].ToString());
                    int nFormID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FormID"].ToString());

                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nID);
                    Params.Add("@p3", nFormID);

                    dLayer.ExecuteNonQuery("Update Gen_ReminderSettings Set X_Subject='" + xSubject + "', N_CategoryID='" + nCategoryID +"' where N_CompanyID=@p1 and N_ID=@p2 and N_FormID=@p3 ", Params, connection, transaction);

                    if (MasterTable.Rows.Count < 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else {
                        transaction.Commit();
                        return Ok(_api.Success("Reminder Settings Saved"));
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