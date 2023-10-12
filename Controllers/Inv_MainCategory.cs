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
    [Route("mainCategory")]
    [ApiController]
    public class Inv_MainCategory : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;



        public Inv_MainCategory(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


  [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                string xButtonAction = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int N_MainCategoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_MainCategoryID"].ToString());
                    
                    SortedList Params = new SortedList();
                    //  Auto Gen;
                    string MainCategoryCode = "";
                    var values = MasterTable.Rows[0]["X_MainCategoryCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterTable.Rows[0]["N_CompanyID"].ToString());
                        Params.Add("N_FnYearID", N_FnYearID);
                        // Params.Add("N_FormID", 73);
                        MainCategoryCode = dLayer.GetAutoNumber("Inv_MainCategory", "X_MainCategoryCode", Params, connection, transaction);
                        xButtonAction = "update";


                        if (MainCategoryCode == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to generate Main Category Code"));
                        }
                        MasterTable.Rows[0]["X_MainCategoryCode"] = MainCategoryCode;
                    }
                    MasterTable.Columns.Remove("N_FnYearID");
                    string X_MainCategory = MasterTable.Rows[0]["X_MainCategory"].ToString();
                    if (N_MainCategoryID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save...Main Category Name Exists"));
                    }

                    else

                    {
                        xButtonAction = "Insert";
                        string ipAddress = "";
                        if (Request.Headers.ContainsKey("X-Forwarded-For"))
                            ipAddress = Request.Headers["X-Forwarded-For"];
                        else
                            ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                        myFunctions.LogScreenActivitys(N_FnYearID, N_MainCategoryID, X_MainCategory, 73, xButtonAction, ipAddress, "", User, dLayer, connection, transaction);
                        transaction.Commit();
                        SortedList Result = new SortedList();
                        Result.Add("N_MainCategoryID", N_MainCategoryID);
                        Result.Add("X_MainCategory", X_MainCategory);
                        return Ok(_api.Success("Main Category Saved"));
                    }

                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nMainCategoryID, int nFnYearID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nMainCategoryID", nMainCategoryID);
                    ParamList.Add("@nCompanyID", nCompanyID);

                    string Sql = "select X_MainCategoryCode,X_MainCategory from Inv_MainCategory where N_MainCategoryID=@nMainCategoryID and N_CompanyID=@nCompanyID";
                    object xMainCategory = dLayer.ExecuteScalar("Select X_MainCategory From Inv_MainCategory Where N_MainCategoryID=" + nMainCategoryID + " and N_CompanyID =" + myFunctions.GetCompanyID(User), connection);
                    // object Objcount = dLayer.ExecuteScalar("Select count(1) From Inv_ItemMaster where N_MainCategoryID=" + nMainCategoryID + " and N_CompanyID =" + myFunctions.GetCompanyID(User), connection);
                    // int Obcount = myFunctions.getIntVAL(Objcount.ToString());
                    string xButtonAction = "Delete";
                    string N_MainCategoryID = "";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);

                    SqlTransaction transaction = connection.BeginTransaction();

                    string ipAddress = "";
                    if (Request.Headers.ContainsKey("X-Forwarded-For"))
                        ipAddress = Request.Headers["X-Forwarded-For"];
                    else
                        ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    myFunctions.LogScreenActivitys(myFunctions.getIntVAL(nFnYearID.ToString()), nMainCategoryID, TransData.Rows[0]["X_MainCategoryCode"].ToString(), 73, xButtonAction, ipAddress, "", User, dLayer, connection, transaction);
                    // if (Obcount != 0)
                    // {
                    //     transaction.Commit();
                    //     return Ok(_api.Error(User, "Unable to Delete.. Main Category Already Used"));
                    // }
                    TransData.Columns.Remove("N_FnYearID");
                    DataRow TransRow = TransData.Rows[0];
                    Results = dLayer.DeleteData("Inv_MainCategory", "N_MainCategoryID", nMainCategoryID, "", connection, transaction);


                    if (Results > 0)
                    {

                        // dLayer.ExecuteNonQuery("Update  Gen_Settings SET  X_Value='' Where X_Group ='Inventory' and X_Description='Default Item Category' and X_Value='" + xMainCategory.ToString() + "'", connection, transaction);
                        transaction.Commit();
                        return Ok(_api.Success("Main category deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete Main category"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }


        }

        [HttpGet("details")]
        public ActionResult GetMainCategoryDetails(int nMainCategoryID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select X_MainCategoryCode from vw_InvItemCategory Where N_CompanyID=@p1 and n_MainCategoryID=@p2 Order By N_MainCategoryID";

            Params.Add("@p1", myFunctions.GetCompanyID(User));
            Params.Add("@p2", nMainCategoryID);


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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("list")]
        public ActionResult GetAllMainCategory(int? nCompanyID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();


            string sqlCommandText = "select * from VW_MainCategory_List_Cloud where N_CompanyID=@p1";

            if(nCompanyID==-1){
                sqlCommandText = "select * from VW_MainCategory_List_Cloud where N_CompanyID=@p1 ";
            }

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
                    return Ok(_api.Success(_api.Format(dt)));
                    //return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(_api.Format(dt)));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
    }
}

           
