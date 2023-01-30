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

                    dLayer.DeleteData("Sec_GeneralScreenSettings", "N_MenuID", N_MenuID, "N_CompanyID=" + nCompanyID, connection);

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
                    string SecUsersql = "select * from vw_GeneralScreenSettings where N_CompanyID=@nCompanyID";
                    SecScreensettings = dLayer.ExecuteDataTable(SecUsersql, PostingParam, connection);
                    SecScreensettings = _api.Format(SecScreensettings, "SecScreensettings");

                    SecAllMenus.Columns.Add("X_WhatsappNumber", typeof(System.String));
                    SecAllMenus.Columns.Add("X_WhatsappKey", typeof(System.String));
                    SecAllMenus.Columns.Add("N_TemplateID", typeof(System.Int32));
                    SecAllMenus.Columns.Add("X_TemplateName", typeof(System.String));
                    SecAllMenus.Columns.Add("B_AttachPdf", typeof(System.Boolean));
                    SecAllMenus.Columns.Add("B_AutoSend", typeof(System.Boolean));
                    SecAllMenus.Columns.Add("X_Email", typeof(System.String));
                    SecAllMenus.Columns.Add("X_Password", typeof(System.String));
                    SecAllMenus.Columns.Add("N_Type", typeof(System.Int32));


                    foreach (DataRow Rows in SecAllMenus.Rows)
                    {

                        foreach (DataRow KRows in SecScreensettings.Rows)
                        {
                            if (Rows["n_MenuID"].ToString() == KRows["n_MenuID"].ToString())
                            {
                                Rows["X_WhatsappNumber"] = KRows["X_WhatsappNumber"].ToString();
                                Rows["X_WhatsappKey"] = KRows["X_WhatsappKey"].ToString();
                                Rows["N_TemplateID"] = KRows["N_TemplateID"].ToString();
                                Rows["X_TemplateName"] = KRows["X_TemplateName"].ToString();
                                Rows["B_AttachPdf"] = Convert.ToBoolean(KRows["B_AttachPdf"].ToString());
                                Rows["B_AutoSend"] = Convert.ToBoolean(KRows["B_AutoSend"].ToString());
                                Rows["X_Email"] = KRows["X_Email"].ToString();
                                Rows["X_Password"] = KRows["X_Password"].ToString();
                                Rows["N_Type"] = KRows["N_Type"].ToString();
                                SecAllMenus.AcceptChanges();
                            }
                            else
                            {
                                 Rows.Delete();

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
                    DataTable DetailTable;

                    DetailTable = ds.Tables["details"];

                    int nCompanyID = myFunctions.GetCompanyID(User);

                    SortedList Params = new SortedList();

                    int N_InternalID = myFunctions.getIntVAL(DetailTable.Rows[0]["N_InternalID"].ToString());
                    int N_MenuID = myFunctions.getIntVAL(DetailTable.Rows[0]["n_MenuID"].ToString());


                    dLayer.DeleteData("Sec_GeneralScreenSettings", "N_InternalID", N_InternalID, "N_MenuID=" + N_MenuID, connection, transaction);
                    if (DetailTable.Rows.Count > 0)
                    {

                        N_InternalID = dLayer.SaveData("Sec_GeneralScreenSettings", "N_InternalID", DetailTable, connection, transaction);

                        if (N_InternalID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to save"));
                        }
                    }

                    if (N_InternalID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
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































































