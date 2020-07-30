
using Microsoft.AspNetCore.Mvc;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("company")]
    [ApiController]
    public class Acc_Company : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Acc_Company(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        //GET api/Company/list
        [AllowAnonymous]
        [HttpGet("list")]
        public ActionResult GetAllCompanys()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select N_CompanyId as nCompanyId,X_CompanyName as xCompanyName,X_CompanyCode as xCompanyCode from Acc_Company where B_Inactive =@p1 order by X_CompanyName";
            Params.Add("@p1", 0);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(dt);
                }

            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }

        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Acc_Company", "N_CompanyID", nCompanyID, "", connection);
                }
                if (Results > 0)
                {
                    return Ok(api.Success("Company Deleted"));
                }
                else
                {
                    return Ok(api.Warning("Unable to Delete Company"));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }




        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable GeneralTable;
                MasterTable = ds.Tables["master"];
                GeneralTable = ds.Tables["general"];

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    // Auto Gen
                    //var values = MasterTable.Rows[0]["X_CompanyCode"].ToString();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    object CompanyCode = "";
                    var values = MasterTable.Rows[0]["x_CompanyCode"].ToString();
                    if (values == "@Auto")
                    {
                        CompanyCode = dLayer.ExecuteScalar("Select ISNULL(MAX(N_CompanyID),0) + 100 from Acc_Company", connection, transaction);//Need Auto Genetaion here
                        if (CompanyCode.ToString() == "") { return Ok(api.Warning("Unable to generate Company Code")); }
                        MasterTable.Rows[0]["x_CompanyCode"] = CompanyCode;
                    }
                    int N_CompanyId = dLayer.SaveData("Acc_Company", "N_CompanyID", 0, MasterTable, connection, transaction);
                    if (N_CompanyId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Warning("Unable to save"));
                    }
                    else
                    {
                        SortedList proParams1 = new SortedList(){
                                {"N_CompanyID",N_CompanyId},
                                {"X_ModuleCode","500"},
                                {"N_UserID",0},
                                {"X_AdminName",GeneralTable.Rows[0]["x_AdminName"].ToString()},
                                {"X_AdminPwd",GeneralTable.Rows[0]["x_AdminPwd"].ToString()},
                                {"X_Currency",MasterTable.Rows[0]["x_Currency"].ToString()}};
                        dLayer.ExecuteNonQueryPro("SP_NewAdminCreation", proParams1, connection, transaction);

                        object N_FnYearId = 0;
     
                        SortedList proParams2 = new SortedList(){
                                {"N_CompanyID",N_CompanyId},
                                {"N_FnYearID",N_FnYearId},
                                {"D_Start",GeneralTable.Rows[0]["d_FromDate"].ToString()},
                                {"D_End",GeneralTable.Rows[0]["d_ToDate"].ToString()}};
                        N_FnYearId = dLayer.ExecuteScalarPro("SP_FinancialYear_Create", proParams2, connection, transaction);

                        SortedList proParams3 = new SortedList(){
                                {"N_CompanyID",N_CompanyId},
                                {"N_FnYearID",N_FnYearId}};
                        dLayer.ExecuteNonQueryPro("SP_AccGruops_Accounts_Create", proParams3, connection, transaction);

                        transaction.Commit();
                        
                        return Ok(api.Success("Company created successfully"));
                    }
                }
            }
            catch (Exception ex)
            {

                return BadRequest(api.Error(ex));
            }
        }

    }
}