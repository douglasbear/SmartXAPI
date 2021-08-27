using AutoMapper;
using SmartxAPI.Data;
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
    [Route("usercategory")]
    [ApiController]
    public class Frm_Usercategory : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly string masterDBConnectionString;

        public Frm_Usercategory(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");            
            masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");


        }
        [HttpGet("list")]
        public ActionResult GetCategoryList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            int nClientID = myFunctions.GetClientID(User);
            string appsSql = "SELECT Top(1) X_AppsIDList=STUFF  ((SELECT DISTINCT ',' + CAST(N_AppID AS VARCHAR(MAX)) FROM ClientApps t2 WHERE t2.N_ClientID = "+nClientID+" FOR XML PATH('') ),1,1,''  )  FROM ClientApps t1  where t1.N_ClientID="+nClientID;
            
            string Apps = "";

                        using (SqlConnection cnn2 = new SqlConnection(masterDBConnectionString))
                        {
                            cnn2.Open();
                            object appsObj = dLayer.ExecuteScalar(appsSql, cnn2);
                            if (appsObj == null)
                            {
                                Apps = "-1";
                            }
                            else
                            {
                                Apps = appsObj.ToString();
                            }

                        }
                         

            string sqlCommandText = "SELECT X_UserCategory AS Category, X_UserCategoryCode AS Code, N_UserCategoryID, N_CompanyID,N_AppID FROM dbo.Sec_UserCategory where N_CompanyID=@p1 and X_UserCategory!='Olivo'  order by Code DESC";
            Params.Add("@p1", nCompanyId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    connection.Open();
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else { return Ok(_api.Success(dt)); }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }



        [HttpGet("listdetails")]
        public ActionResult GetCategoryDetails(int? nCategoryId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from Sec_UserCategory where N_CompanyID=@p1 and N_UserCategoryID=@p2 order by N_UserCategoryID DESC";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nCategoryId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                { return Ok(_api.Notice("No Results Found")); }
                else { return Ok(_api.Success(dt)); }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }


        //Save....
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
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string X_UserCategoryCode = "";
                    int FromUserCatID =myFunctions.getIntVAL(MasterTable.Rows[0]["N_FromUserCatID"].ToString());
                    int UserCatID =myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserCategoryID"].ToString());
                    var values = MasterTable.Rows[0]["X_UserCategoryCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID", MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 40);
                        Params.Add("N_BranchID", MasterTable.Rows[0]["n_BranchId"].ToString());
                        X_UserCategoryCode = dLayer.GetAutoNumber("sec_usercategory", "X_UserCategoryCode", Params, connection, transaction);
                        if (X_UserCategoryCode == "") { transaction.Rollback();return Ok( _api.Error( "Unable to generate Category Code")); }
                        MasterTable.Rows[0]["X_UserCategoryCode"] = X_UserCategoryCode;
                    }

                    MasterTable.Columns.Remove("n_FnYearId");
                    MasterTable.Columns.Remove("n_BranchId");
                    MasterTable.Columns.Remove("N_FromUserCatID");

                    int N_UserCategoryID = dLayer.SaveData("sec_usercategory", "N_UserCategoryID", MasterTable, connection, transaction);
                    if (N_UserCategoryID <= 0)
                    {
                        transaction.Rollback();
                        return Ok( _api.Error( "Unable to save"));
                    }
                    else
                    {
                        if(UserCatID==0)
                        {
                            DataTable UserPrevilegesDT = new DataTable();
                            string sqlCommandText = "select 0 AS N_InternalID,"+N_UserCategoryID+" AS N_UserCategoryID, N_MenuID, B_Visible, B_Edit, B_Delete, B_Save, B_View from Sec_UserPrevileges where N_UserCategoryID="+FromUserCatID;
                            UserPrevilegesDT = dLayer.ExecuteDataTable(sqlCommandText, Params, connection,transaction);
                            if(UserPrevilegesDT.Rows.Count==0)
                            {
                                DataRow row = UserPrevilegesDT.NewRow();
                                row["N_InternalID"] = 0;
                                row["N_UserCategoryID"] = N_UserCategoryID;
                                row["N_MenuID"] = 1;
                                row["B_Visible"] = 1;
                                row["B_Edit"] = 1;
                                row["B_Delete"] = 1;
                                row["B_Save"] = 1;
                                row["B_View"] = null;
                                UserPrevilegesDT.Rows.Add(row);

                                DataRow row2 = UserPrevilegesDT.NewRow();
                                row2["N_InternalID"] = 0;
                                row2["N_UserCategoryID"] = N_UserCategoryID;
                                row2["N_MenuID"] = 70;
                                row2["B_Visible"] = 1;
                                row2["B_Edit"] = 1;
                                row2["B_Delete"] = 1;
                                row2["B_Save"] = 1;
                                row2["B_View"] = null;
                                UserPrevilegesDT.Rows.Add(row2);

                                DataRow row3 = UserPrevilegesDT.NewRow();
                                row3["N_InternalID"] = 0;
                                row3["N_UserCategoryID"] = N_UserCategoryID;
                                row3["N_MenuID"] = 148;
                                row3["B_Visible"] = 1;
                                row3["B_Edit"] = 1;
                                row3["B_Delete"] = 1;
                                row3["B_Save"] = 1;
                                row3["B_View"] = null;
                                UserPrevilegesDT.Rows.Add(row2);
                            }
                            int N_InternalID = dLayer.SaveData("Sec_UserPrevileges", "N_InternalID", UserPrevilegesDT, connection, transaction);
                            if (N_InternalID <= 0)
                            {
                                transaction.Rollback();
                                return Ok( _api.Error( "Unable to save"));
                            }
                        }

                        transaction.Commit();
                        return GetCategoryDetails(N_UserCategoryID);
                    }
                }
            }
            catch (Exception ex)
            {

                return Ok(_api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nUsercategoryId)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    int nCompanyID = myFunctions.GetCompanyID(User);

                    SortedList QueryParams = new SortedList();
                    QueryParams.Add("@nCompanyID", nCompanyID);
                    QueryParams.Add("@nUsercategoryID", nUsercategoryId);
                    object Category = dLayer.ExecuteScalar("Select N_UserCategoryID From Sec_UserCategory Where N_UserCategoryID=@nUsercategoryID and N_CompanyID=@nCompanyID", QueryParams, connection,transaction);
                    if(Category==null)
                        return Ok(_api.Error("Invalid Category"));
                    
                    Results = dLayer.DeleteData("Sec_UserPrevileges", "N_UserCategoryID", nUsercategoryId, "", connection,transaction);
                    if(Results<0)
                        return Ok(_api.Error("Unable to delete Category"));

                    object InUser = dLayer.ExecuteScalar("select N_UserID from Sec_User where N_UserCategoryID=@nUsercategoryID", QueryParams, connection,transaction);

                    if(InUser!=null)
                        return Ok(_api.Error("Unable to delete Category"));

                    Results = dLayer.DeleteData("sec_usercategory", "N_UserCategoryID", nUsercategoryId, "", connection);
                    if (Results > 0)
                    {
                        dLayer.ExecuteNonQuery("DELETE FROM Gen_Settings where N_UserCategoryID=@nUsercategoryID and N_CompanyID=@nCompanyID", QueryParams, connection,transaction);
                        return Ok( _api.Success("Category deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete Category"));
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