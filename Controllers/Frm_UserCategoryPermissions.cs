using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;
using System.Net;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("userGroupPermissions")]
    [ApiController]
    public class Frm_UserGroupPermissions : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID = 136;

        public Frm_UserGroupPermissions(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("userGroup")]
        public ActionResult GetUserGroup()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    string x_UserCategoryName = "";
                    string sqlCommandText = "";
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int userCategoryID = myFunctions.GetUserCategory(User);
                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID", nCompanyID);
                    object x_UserCategory = dLayer.ExecuteScalar("Select X_UserCatgory from Sec_UserCategory Where N_UserCategoryID =" + userCategoryID + "", Params, connection);
                    if (x_UserCategory != null)
                    {
                        x_UserCategoryName = x_UserCategory.ToString();
                    }
                    DataTable dt = new DataTable();
                    if (x_UserCategoryName == "Olivo")
                    {
                        sqlCommandText = "Select * from Sec_UserCategory Where X_UserCategory <> 'Olivo' and N_CompanyID=" + nCompanyID;
                    }
                    else
                    {
                        sqlCommandText = "Select * from Sec_UserCategory Where  X_UserCategory <> 'Olivo' and X_UserCategory <> 'Administrator' and N_CompanyID=" + nCompanyID;
                    }
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    dt = _api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(dt));
                    }
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }



        [HttpGet("moduleList")]
        public ActionResult GetModuleList(int nLanguageID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    string x_UserCategoryName = "";
                    string sqlCommandText = "";
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int userCategoryID = myFunctions.GetUserCategory(User);
                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID", nCompanyID);
                    object x_UserCategory = dLayer.ExecuteScalar("Select X_UserCatgory from Sec_UserCategory Where N_UserCategoryID =" + userCategoryID + "", Params, connection);
                    if (x_UserCategory != null)
                    {
                        x_UserCategoryName = x_UserCategory.ToString();
                    }
                    DataTable dt = new DataTable();
                    if (x_UserCategoryName == "Olivo")
                    {
                        sqlCommandText = "Select * from vw_UserMenusAll_Disp Where  N_LanguageID=" + nLanguageID + "  and N_ParentMenuID = 0 and X_ControlNo = '0'";
                    }
                    else
                    {
                        sqlCommandText = "Select * from vw_UserMenus_Disp Where  N_LanguageID=" + nLanguageID + "  and N_ParentMenuID = 0 and X_ControlNo = '0' and X_UserCategory='" + x_UserCategoryName + "' and N_CompanyID=" + nCompanyID + "";
                    }
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    dt = _api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(dt));
                    }
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        // [HttpGet("fillData")]
        // public ActionResult GetScreen(string perModules, string permUserCategory)
        // {
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();

        //             int nCompanyID = myFunctions.GetCompanyID(User);
        //             DataSet dt = new DataSet();
        //             DataTable SecUserPermissions = new DataTable();
        //             DataTable SecAllMenus = new DataTable();

        //             SortedList Params = new SortedList();
        //             SortedList PostingParam = new SortedList();
        //             SortedList secParams = new SortedList();
        //             Params.Add("@nCompanyID", nCompanyID);
        //             secParams.Add("N_CompanyID", nCompanyID);
        //             secParams.Add("x_UserCategory", "permUserCategory");
        //             secParams.Add("N_MenuID", N_MenuID);
        //             dba.FillDataSet(ref dsPpermissions, "Sec_AllMenus", "SP_Sec_UserMenus_Sel " + myCompanyID._CompanyID + ",'" + myCompanyID._UserCategoryName + "'," + N_MenuID + "," + myCompanyID._LanguageID, "TEXT", new DataTable());






        //             int N_MenuID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_MenuID from vw_UserMenusAll_Disp where X_Text='" + perModules + "' and N_ParentMenuID=0", Params, connection).ToString());
        //             PostingParam.Add("@nCompanyID", nCompanyID);
        //             PostingParam.Add("@xUserCategory", "permUserCategory");
        //             PostingParam.Add("@nMenuID", N_MenuID);
        //             string SecUsersql="SP_InvSalesOrderDtls_Disp @nCompanyID,@xUserCategory,@nMenuID";
        //             SecUserPermissions = dLayer.ExecuteDataTable(SecUsersql, PostingParam, connection);
        //             SecUserPermissions = _api.Format(SecUserPermissions, "SecUserPermissions");
        //             transaction.Commit();
        //             return Ok(_api.Success(""));
        //         }



        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(_api.Error(e));
        //     }
        // }
    }
}
// [HttpPost("save")]
// public ActionResult SaveData([FromBody] DataSet ds)
// {
//     try
//     {
//         using (SqlConnection connection = new SqlConnection(connectionString))
//         {
//             connection.Open();
//             SqlTransaction transaction = connection.BeginTransaction();
//             DataTable MasterTable;
//             DataTable DetailTable;
//             MasterTable = ds.Tables["master"];
//             DetailTable = ds.Tables["details"];
//             DataRow MasterRow = MasterTable.Rows[0];
//             SortedList Params = new SortedList();
//             int N_UserCategoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_UserCategoryID"].ToString());
//             int N_MenuID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_MenuID"].ToString());
//             dLayer.DeleteData("Sec_userPrevileges", "N_UserCategoryID", N_UserCategoryID, "N_MenuID=" + N_MenuID, connection, transaction);
//             if (DetailTable.Rows.Count > 0)
//             {
//                 foreach (DataRow Rows in DetailTable.Rows)
//                 {
//                     dLayer.DeleteData("Sec_userPrevileges", "N_UserCategoryID", N_UserCategoryID.ToString(), "N_MenuID=" + Rows["n_MenuID"],connection, transaction);

//                 }
















