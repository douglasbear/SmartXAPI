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
                    object x_UserCategory = dLayer.ExecuteScalar("Select X_UserCategory from Sec_UserCategory Where N_UserCategoryID =" + userCategoryID + "", Params, connection);
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
                    object x_UserCategory = dLayer.ExecuteScalar("Select X_UserCategory from Sec_UserCategory Where N_UserCategoryID =" + userCategoryID + "", Params, connection);
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
        [HttpGet("fillData")]
        public ActionResult GetScreen(string perModules, string permUserCategory, int nLanguageID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    string x_UserCategoryName = "";
                    int userCategoryID = myFunctions.GetUserCategory(User);
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);


                    object x_UserCategory = dLayer.ExecuteScalar("Select X_UserCategory from Sec_UserCategory Where N_UserCategoryID =" + userCategoryID + "", Params, connection);
                    if (x_UserCategory != null)
                    {
                        x_UserCategoryName = x_UserCategory.ToString();
                    }


                    DataTable SecUserPermissions = new DataTable();
                    DataTable SecAllMenus = new DataTable();


                    SortedList PostingParam = new SortedList();
                    SortedList secParams = new SortedList();
                    int N_MenuID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_MenuID from vw_UserMenusAll_Disp where X_Text='" + perModules + "' and N_ParentMenuID=0", Params, connection).ToString());



                    secParams.Add("@nCompanyID", nCompanyID);
                    secParams.Add("@xUserCategory", x_UserCategoryName);
                    secParams.Add("@nMenuID", N_MenuID);
                    secParams.Add("@nLanguageID", nLanguageID);
                    string SecAllSql = "SP_Sec_UserMenus_Sel @nCompanyID,@xUserCategory,@nMenuID,@nLanguageID";
                    SecAllMenus = dLayer.ExecuteDataTable(SecAllSql, secParams, connection);
                    SecAllMenus = _api.Format(SecAllMenus, "SecAllMenus");





                    PostingParam.Add("@nCompanyID", nCompanyID);
                    PostingParam.Add("@xUserCategory", permUserCategory);
                    PostingParam.Add("@nMenuID", N_MenuID);
                    string SecUsersql = "SP_Sec_UserMenus_Permissions @nCompanyID,@xUserCategory,@nMenuID";
                    SecUserPermissions = dLayer.ExecuteDataTable(SecUsersql, PostingParam, connection);
                    SecUserPermissions = _api.Format(SecUserPermissions, "SecUserPermissions");
                

                    dt.Tables.Add(SecUserPermissions);
                    dt.Tables.Add(SecAllMenus);



                    return Ok(_api.Success(dt));
                }



            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }


        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;


                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    int nCompanyID = myFunctions.GetCompanyID(User);

                    SortedList Params = new SortedList();

                    int N_UserCategoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_UserCategoryID"].ToString());
                    int N_MenuID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_MenuID"].ToString());
                    int flag = myFunctions.getIntVAL(MasterTable.Rows[0]["flag"].ToString());
                    int N_IsAdmin = myFunctions.getIntVAL(MasterTable.Rows[0]["n_IsAdmin"].ToString());

                    dLayer.DeleteData("Sec_userPrevileges", "N_UserCategoryID", N_UserCategoryID, "N_MenuID=" + N_MenuID, connection, transaction);
                    if (DetailTable.Rows.Count > 0)
                    {
                        foreach (DataRow Rows in DetailTable.Rows)
                        {
                            dLayer.DeleteData("Sec_userPrevileges", "N_UserCategoryID", N_UserCategoryID, "N_MenuID=" + Rows["n_MenuID"], connection, transaction);

                        }
                    }
                    if (DetailTable.Rows.Count > 0)
                    {

                        int nInternalID = dLayer.SaveData("Sec_UserPrevileges", "N_InternalID", DetailTable, connection, transaction);

                        if (nInternalID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable to save"));
                        }
                    }


                    if (flag == 1)
                    {
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("N_InternalID");
                        dt.Columns.Add("N_MenuID");
                        dt.Columns.Add("N_UserCategoryID");
                        dt.Columns.Add("B_Visible");
                        dt.Columns.Add("B_Save");
                        dt.Columns.Add("B_Edit");
                        dt.Columns.Add("B_Delete");


                        DataRow row = dt.NewRow();
                        row["N_InternalID"] = 0;
                        row["N_MenuID"] = N_MenuID;
                        row["N_UserCategoryID"] = N_UserCategoryID;
                        row["B_Visible"] = 1;
                        row["B_Save"] = 0;
                        row["B_Edit"] = 0;
                        row["B_Delete"] = 0;
                        dt.Rows.Add(row);

                        int N_InternalID = dLayer.SaveData("Sec_UserPrevileges", "N_InternalID", dt, connection, transaction);
                        if (N_InternalID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable to save"));
                        }


                    }
                    if (N_IsAdmin == 1)
                    {
                        dLayer.ExecuteNonQuery("delete from Sec_UserPrevileges where N_UserCategoryID not in (select N_UserCategoryID from Sec_UserCategory where X_UserCategory in ('Olivo','Administrator') and N_CompanyID=" + nCompanyID + ") and N_MenuID not in (select N_MenuID from Sec_UserPrevileges where N_UserCategoryID=" + N_UserCategoryID + ")and N_MenuID in (select N_MenuID from Sec_Menus where N_ParentMenuID=" + N_MenuID + ")", Params, connection, transaction);
                    }
                    transaction.Commit();
                }
                return Ok(_api.Success("Saved"));
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
    }
}































































