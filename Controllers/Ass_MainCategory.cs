
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
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("assetmaincategory")]
    [ApiController]
    public class Ass_MainCategory : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1114;

        public Ass_MainCategory(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString =
            conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult AssetMainList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "Select * from vw_InvMainAssetCategory_Disp";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
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
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("details")]
        public ActionResult AssetMainDetails(int nMainCategoryID, int nFnYearID, int? nLangID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int N_Flag = 0;
            if (nLangID == 2)
            {
                N_Flag = 1;
            }
            string sqlCommandText = "SP_Inv_MainAssetItemcategory_Disp  " + nCompanyID + "," + nFnYearID + "," + N_Flag + "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearID);
            Params.Add("@p3", nMainCategoryID);
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    DataTable output = new DataTable();
                     DataRow[] dr=dt.Select("N_MainCategoryID = " + nMainCategoryID + " and  N_CompanyID = " + nCompanyID + " and N_FnYearID=" + nFnYearID + "");
                    output = dr.CopyToDataTable();  
                    output = api.Format(output);
                    if (output.Rows.Count == 0)
                        return Ok(api.Warning("No Results Found"));
                    else
                        return Ok(api.Success(output));
                }
            }
            catch (Exception e)
            {
                return BadRequest(api.Error(User,e));
            }
        }



        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nAssetMainId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_MainCategoryId"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string AssetMainCode = "";
                    var values = MasterTable.Rows[0]["X_MainCategoryCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        AssetMainCode = dLayer.GetAutoNumber("Ass_AssetMainCategory", "X_MainCategoryCode", Params, connection, transaction);
                        if (AssetMainCode == "") { transaction.Rollback(); return Ok(api.Error(User,"Unable to generate Asset Code")); }
                        MasterTable.Rows[0]["X_MainCategoryCode"] = AssetMainCode;
                    }


                    nAssetMainId = dLayer.SaveData("Ass_AssetMainCategory", "N_MainCategoryId", MasterTable, connection, transaction);
                    if (nAssetMainId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Asset Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(User,ex));
            }
        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nAssetMainId)
        {

            int Results = 0;
            try
            {
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Ass_AssetMainCategory ", "N_MainCategoryId", nAssetMainId, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("N_MainCategoryId", nAssetMainId.ToString());
                    return Ok(api.Success(res, "Asset deleted"));
                }
                else
                {
                    return Ok(api.Error(User,"Unable to delete Asset"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }



        }
    }
}
