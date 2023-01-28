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
    [Route("genScreenSettings")]
    [ApiController]
    public class Gen_ScreenSettings : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID = 136;

        public Gen_ScreenSettings(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("fillData")]
        public ActionResult GetScreen(string perModules, string permUserCategory, int nLanguageID, int nUserCategoryID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    //string x_UserCategoryName = "";
                    string x_UserCategoryName = myFunctions.GetUserCategoryList(User);
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    Boolean bool1 = true;
                    Boolean bool2 = false;

                    x_UserCategoryName = x_UserCategoryName + "," + nUserCategoryID;



                    DataTable SecScreensettings = new DataTable();
                    DataTable SecAllMenus = new DataTable();


                    SortedList PostingParam = new SortedList();
                    SortedList secParams = new SortedList();
                    int N_MenuID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_MenuID from vw_UserMenusAll_Disp where X_Text='" + perModules + "' and N_ParentMenuID=0", Params, connection).ToString());



                    secParams.Add("@nCompanyID", nCompanyID);
                    secParams.Add("@xUserCategory", x_UserCategoryName);
                    secParams.Add("@nMenuID", N_MenuID);
                    secParams.Add("@nLanguageID", nLanguageID);
                    secParams.Add("@nIsCategoryID", 1);
                    secParams.Add("@userGroupCategoryID", nUserCategoryID);


                    string SecAllSql = "SP_Sec_UserMenus_Sel @nCompanyID,@xUserCategory,@nMenuID,@nLanguageID,@nIsCategoryID";
                    SecAllMenus = dLayer.ExecuteDataTable(SecAllSql, secParams, connection);

                    PostingParam.Add("@nCompanyID", nCompanyID);
                    PostingParam.Add("@xUserCategory", permUserCategory);
                    PostingParam.Add("@nMenuID", N_MenuID);
                    string SecUsersql = "select * from vw_GeneralScreenSettings where N_CompanyID=@nCompanyID and N_MenuID=@nMenuID";
                    SecScreensettings = dLayer.ExecuteDataTable(SecUsersql, PostingParam, connection);
                    SecScreensettings = _api.Format(SecScreensettings, "SecScreensettings");
                    SecScreensettings.Columns.Add("X_WhatsappNumber", typeof(System.String));
                    SecScreensettings.Columns.Add("X_WhatsappKey", typeof(System.String));
                    SecScreensettings.Columns.Add("N_TemplateID", typeof(System.Int32));
                    SecScreensettings.Columns.Add("X_TemplateName", typeof(System.String));
                    SecScreensettings.Columns.Add("B_AttachPdf", typeof(System.Boolean));
                    SecScreensettings.Columns.Add("B_AutoSend", typeof(System.Boolean));

                    // if (SecAllMenus.Rows.Count > 0)
                    // {
                    //     foreach (DataRow Rows in SecAllMenus.Rows)
                    //     {

                    //         Rows["b_Delete"] = bool2;
                    //         Rows["b_Edit"] = bool2;
                    //         Rows["b_Save"] = bool2;

                    //         Rows["b_Visible"] = bool2;
                    //     }
                    // }
                    // SecAllMenus.AcceptChanges();
                    foreach (DataRow Rows in SecAllMenus.Rows)
                    {

                        foreach (DataRow KRows in SecScreensettings.Rows)
                        {
                            if (Rows["n_MenuID"].ToString() == KRows["n_MenuID"].ToString())
                            {
                                Rows["X_WhatsappNumber"] =KRows["X_WhatsappNumber"].ToString();
                                Rows["X_WhatsappKey"] = KRows["X_WhatsappKey"].ToString();
                                Rows["N_TemplateID"] = KRows["N_TemplateID"].ToString();
                                Rows["X_TemplateName"] = KRows["X_TemplateName"].ToString();
                                Rows["B_AttachPdf"] = Convert.ToBoolean(KRows["B_AttachPdf"].ToString());
                                Rows["B_AutoSend"] = Convert.ToBoolean(KRows["B_AutoSend"].ToString());


                                SecAllMenus.AcceptChanges();
                            }
                            SecAllMenus.AcceptChanges();





                        }
                    }
                    SecAllMenus.AcceptChanges();


                    foreach (DataRow PRows in SecAllMenus.Rows)
                    {
                        if (PRows["X_Text"].ToString() == "Seperator" || PRows["X_Text"].ToString() == "seprator" || PRows["X_Text"].ToString() == "Seperator ")
                        {
                            PRows.Delete();
                            continue;
                        }

                    }
                    SecAllMenus.AcceptChanges();

                    SecAllMenus = _api.Format(SecAllMenus, "SecAllMenus");




                    dt.Tables.Add(SecScreensettings);
                    dt.Tables.Add(SecAllMenus);



                    return Ok(_api.Success(dt));
                }



            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
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
                            dLayer.DeleteData("Sec_userPrevileges", "N_UserCategoryID", N_UserCategoryID, "N_MenuID=" + Rows["n_MenuID"].ToString(), connection, transaction);

                        }
                    }
                    //  transaction.Commit();
                    //   return Ok(_api.Success("Saved"));

                    if (DetailTable.Rows.Count > 0)
                    {

                        int nInternalID = dLayer.SaveData("Sec_UserPrevileges", "N_InternalID", DetailTable, connection, transaction);

                        if (nInternalID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to save"));
                        }
                    }



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
                        return Ok(_api.Error(User, "Unable to save"));
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
                return Ok(_api.Error(User, ex));
            }
        }
    }
}































































