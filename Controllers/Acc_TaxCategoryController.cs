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
    [Route("taxcategory")]
    [ApiController]



    public class AccTaxCategoryController : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public AccTaxCategoryController(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        //List
        [HttpGet("list")]
        public ActionResult GetAllTaxTypes(int? nCompanyID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_TaxCategory_Disp where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return StatusCode(200, _api.Response(200, "No Results Found"));
                }
                else
                {
                    return Ok(dt);
                }

            }
            catch (Exception e)
            {
                return StatusCode(403, _api.ErrorResponse(e));
            }
        }

        //List
        [HttpGet("listdetails")]
        public ActionResult GetAllTaxTypesDetails(int? nCompanyID, int? N_PkeyID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_TaxCategory_Disp where N_CompanyID=@p1 and N_PkeyID=@p2";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", N_PkeyID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return StatusCode(200, _api.Response(200, "No Results Found"));
                }
                else
                {
                    return Ok(dt);
                }

            }
            catch (Exception e)
            {
                return StatusCode(403, _api.ErrorResponse(e));
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    // Auto Gen
                    SortedList Params = new SortedList();
                    string CategoryCode = "";
                    var values = MasterTable.Rows[0]["X_PkeyCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID", MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 852);
                        Params.Add("N_BranchID", MasterTable.Rows[0]["n_BranchId"].ToString());
                        CategoryCode = dLayer.GetAutoNumber("Acc_TaxCategory", "X_PkeyCode", Params, connection, transaction);
                        if (CategoryCode == "") { return StatusCode(409, _api.Response(409, "Unable to generate Customer Code")); }
                        MasterTable.Rows[0]["X_PkeyCode"] = CategoryCode;

                    }
                    MasterTable.Columns.Remove("n_FnYearId");
                    MasterTable.Columns.Remove("n_BranchId");
                    int N_TaxCategoryID = dLayer.SaveData("Acc_TaxCategory", "N_PkeyID", 0, MasterTable, connection, transaction);
                    if (N_TaxCategoryID <= 0)
                    {
                        transaction.Rollback();
                        return StatusCode(404, _api.Response(404, "Unable to save"));
                    }
                    else
                        transaction.Commit();

                    return GetAllTaxTypesDetails(int.Parse(MasterTable.Rows[0]["n_CompanyId"].ToString()), N_TaxCategoryID);
                }

            }
            catch (Exception ex)
            {
                dLayer.rollBack();
                return StatusCode(403, _api.ErrorResponse(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCategoryID)
        {
            int Results = 0;
            try
            {

                Results = dLayer.DeleteData("Acc_TaxCategory", "N_PkeyID", nCategoryID, "");

                if (Results > 0)
                {
                    return StatusCode(200, _api.Response(200, "Tax category deleted"));
                }
                else
                {
                    return StatusCode(409, _api.Response(409, "Unable to delete Tax category"));
                }

            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.ErrorResponse(ex));
            }


        }




    }
}