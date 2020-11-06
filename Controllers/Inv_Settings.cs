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

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("invSettings")]
    [ApiController]
    public class Inv_Settings : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Inv_Settings(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }



        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
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
                return BadRequest(_api.Error(ex));
            }
        }


        [HttpGet("details")]
        public ActionResult GetDetails()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable SettingsList = new DataTable();
                    SettingsList.Columns.Add(new DataColumn("X_Group", typeof(string)));
                    SettingsList.Columns.Add(new DataColumn("X_Description", typeof(string)));
                    SettingsList.Columns.Add(new DataColumn("N_UserCategoryID", typeof(int)));
                    SettingsList.Columns.Add(new DataColumn("X_ReturnFeild", typeof(string)));

                    SettingsList.Rows.Add("Inventory", "Default Item Class", 0, "X_Value");
                    SettingsList.Rows.Add("Inventory", "Default Item Unit", 0, "X_Value");
                    SettingsList.Rows.Add("Inventory", "Default Item Category", 0, "X_Value");
                    SettingsList.Rows.Add("65", "Enable Selling Price In Opening Stock", 0, "N_Value");
                    SettingsList.Rows.Add("506", "Enable Qty On Hand In Inventory Adjustment", 0, "N_Value");
                    SettingsList.Rows.Add("367", "ShowStkQtyInUC", 0, "N_Value");
                    SettingsList.Rows.Add("Inventory", "EnablePattern", 0, "N_Value");
                    SettingsList.Rows.Add("556", "IsPartNoEnable", 0, "N_Value");
                    SettingsList.Rows.Add("53", "Enable Product Category In Product Maintenance", 0, "N_Value");
                    SettingsList.Rows.Add("88", "Enable Current Stock In Opening Stock", 0, "N_Value");
                    SettingsList.Rows.Add("729", "ShowTrckInDN", 0, "N_Value");
                    SettingsList.Rows.Add("556", "IdDelDaysingrid", 0, "N_Value");
                    SettingsList.Rows.Add("556", "IsRemarksingrid", 0, "N_Value");
                    SettingsList.Rows.Add("345", "EnableAlternativeProduct", 0, "N_Value");
                    SettingsList.Rows.Add("345", "EnableWasteQuantity", 0, "N_Value");
                    SettingsList.Rows.Add("345", "EnableRecycleQuantity", 0, "N_Value");

                    SettingsList.AcceptChanges();
                    DataTable Details = dLayer.ExecuteSettingsPro("SP_GenSettings_Disp", SettingsList, myFunctions.GetCompanyID(User), connection);
                    return Ok(_api.Success(Details));
                }

            }
            catch (Exception e)
            {
                return BadRequest(_api.Error(e));
            }
        }



    }


}