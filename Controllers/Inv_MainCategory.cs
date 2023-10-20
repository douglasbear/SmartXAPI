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
        private readonly int FormID;

        public Inv_MainCategory(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1809;
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
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int N_MainCategoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_MainCategoryID"].ToString());
                    //  Auto Gen;
                    string X_MainCategoryCode = MasterTable.Rows[0]["X_MainCategoryCode"].ToString();
                    String xButtonAction="";
                    if (X_MainCategoryCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterTable.Rows[0]["N_CompanyID"].ToString());
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", 1809);
                        X_MainCategoryCode = dLayer.GetAutoNumber("Inv_MainCategory", "X_MainCategoryCode", Params, connection, transaction);
                        xButtonAction = "Insert";
                        if (X_MainCategoryCode == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to generate Main Category Code"));
                        }
                        MasterTable.Rows[0]["X_MainCategoryCode"] = X_MainCategoryCode;
                    }
                    else{
                        xButtonAction = "Update";
                    }
                    MasterTable.Columns.Remove("N_FnYearID");

                    N_MainCategoryID = dLayer.SaveData("Inv_MainCategory", "N_MainCategoryID", MasterTable, connection, transaction);
                    if (N_MainCategoryID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save...Main Category Name Exists"));
                    }
                    else
                    {
                        // Activity Log
                        string ipAddress = "";
                        if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                            ipAddress = Request.Headers["X-Forwarded-For"];
                        else
                            ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                            myFunctions.LogScreenActivitys(N_FnYearID, N_MainCategoryID, X_MainCategoryCode, 1809, xButtonAction, ipAddress, "", User, dLayer, connection, transaction);
                        
                        transaction.Commit();
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
                    
                    string xButtonAction = "Delete";
                    string X_MainCategoryCode = "";

                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    SqlTransaction transaction = connection.BeginTransaction();

                    //Activity Log
                    string ipAddress = "";
                    if (Request.Headers.ContainsKey("X-Forwarded-For"))
                        ipAddress = Request.Headers["X-Forwarded-For"];
                    else
                        ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                        myFunctions.LogScreenActivitys(myFunctions.getIntVAL(nFnYearID.ToString()), nMainCategoryID, TransData.Rows[0]["X_MainCategoryCode"].ToString(), 1809, xButtonAction, ipAddress, "", User, dLayer, connection, transaction);
                    
                    DataRow TransRow = TransData.Rows[0];
                    Results = dLayer.DeleteData("Inv_MainCategory", "N_MainCategoryID", nMainCategoryID, "", connection, transaction);

                    if (Results > 0)
                    {
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
            int nCompanyID= myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from VW_MainCategory_List_Cloud Where N_CompanyID=@p1 and N_MainCategoryID=@p2";

            Params.Add("@p1", nCompanyID);
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
        public ActionResult GetAllMainCategory()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID= myFunctions.GetCompanyID(User);
          
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

           
