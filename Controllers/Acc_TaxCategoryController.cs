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
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(_api.Format(dt)));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
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
                    return Ok(_api.Warning("No Results Found"));
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
                        if (CategoryCode == "") { return Ok( _api.Warning("Unable to generate Customer Code")); }
                        MasterTable.Rows[0]["X_PkeyCode"] = CategoryCode;

                    }
                    MasterTable.Columns.Remove("n_FnYearId");
                    MasterTable.Columns.Remove("n_BranchId");
                    int N_TaxCategoryID = dLayer.SaveData("Acc_TaxCategory", "N_PkeyID", MasterTable, connection, transaction);
                    if (N_TaxCategoryID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("Unable to save"));
                    }
                    else
                        transaction.Commit();

                    return GetAllTaxTypesDetails(int.Parse(MasterTable.Rows[0]["n_CompanyId"].ToString()), N_TaxCategoryID);
                }

            }
            catch (Exception ex)
            {
                return Ok( _api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCategoryID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Acc_TaxCategory", "N_PkeyID", nCategoryID, "",connection);

                    if (Results > 0)
                    {
                        return Ok(_api.Success( "Tax category deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete Tax category"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }


        }

        [HttpGet("taxType")]
        public ActionResult GetTaxType()
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1",nCompanyId);
            string sqlCommandText = "Select * from Acc_TaxType where N_CompanyID=@p1";

            try{
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                    if (dt.Rows.Count==0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }else{
                        return Ok(_api.Success(dt));
                    }
            }catch(Exception e){
                return Ok(_api.Error(e));
            }
        }



    }
}