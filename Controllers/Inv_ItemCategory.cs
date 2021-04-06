using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("itemcategory")]
    [ApiController]
    public class Inv_ItemCategory : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;



        public Inv_ItemCategory(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        //GET api/productcategory/list?....
        [HttpGet("list")]
        public ActionResult GetItemCategory()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);

            string sqlCommandText = "select Code as N_CategoryID,Category as X_Category,CategoryCode as X_CategoryCode from vw_InvItemCategory_Disp where N_CompanyID=@p1 order by N_CategoryID DESC";
            Params.Add("@p1", nCompanyId);

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

        [HttpGet("details")]
        public ActionResult GetItemCategoryDetails(int nFnYearID, int nCategoryId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "Select TOP 1 *,Code as X_CategoryCode from vw_InvItemCategory Where N_CompanyID=@p1 and (N_FnYearID =@p3 or N_FnYearID is null ) and n_CategoryID=@p2 Order By N_CategoryID";

            Params.Add("@p1", myFunctions.GetCompanyID(User));
            Params.Add("@p2", nCategoryId);
            Params.Add("@p3", nFnYearID);
            

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Data Found"));
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
                    int N_CategoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CategoryID"].ToString());
                    int N_FnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearId"].ToString());
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string CategoryCode = "";
                    var values = MasterTable.Rows[0]["X_CategoryCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterTable.Rows[0]["N_CompanyId"].ToString());
                        Params.Add("N_YearID", N_FnYearId);
                        Params.Add("N_FormID", 73);
                        CategoryCode = dLayer.GetAutoNumber("Inv_ItemCategory", "X_CategoryCode", Params, connection, transaction);
                        if (CategoryCode == "") {transaction.Rollback();  return Ok(_api.Error("Unable to generate Category Code")); }
                        MasterTable.Rows[0]["X_CategoryCode"] = CategoryCode;
                    }
                    MasterTable.Columns.Remove("N_FnYearId");
                    N_CategoryID = dLayer.SaveData("Inv_ItemCategory", "N_CategoryID", MasterTable, connection, transaction);
                    if (N_CategoryID <= 0)
                    {
                        transaction.Rollback();
                        return Ok( _api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok( _api.Success("Product Category Saved"));
                    }
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
                object xCategory = dLayer.ExecuteScalar("Select X_Category From Inv_ItemCategory Where N_CategoryID=" + nCategoryID + " and N_CompanyID =" + myFunctions.GetCompanyID(User),connection);
                
                Results = dLayer.DeleteData("Inv_ItemCategory", "N_CategoryID", nCategoryID, "",connection);
                if (Results > 0)
                {

                        dLayer.ExecuteNonQuery("Update  Gen_Settings SET  X_Value='' Where X_Group ='Inventory' and X_Description='Default Item Category' and X_Value='" + xCategory.ToString()+"'",connection);

                    return Ok(_api.Success("Product category deleted"));
                }
                else
                {
                    return Ok(_api.Error( "Unable to delete product category"));
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