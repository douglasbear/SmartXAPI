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

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("nationality")]
    [ApiController]
    public class Pay_Nationality : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Pay_Nationality(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 218;
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.GetCompanyID(User);
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nNationalityID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_NationalityID"].ToString());
                string xNationality = MasterTable.Rows[0]["x_Nationality"].ToString();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string xNationalityCode = "";
                    var values = MasterTable.Rows[0]["x_NationalityCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                       
                        while (true)
                        {
                            xNationalityCode = (string)dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction);

                            object N_Result = dLayer.ExecuteScalar("Select 1 from Pay_Nationality Where N_NationalityID ='" + xNationalityCode+"'", connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        if (xNationalityCode == "") { transaction.Rollback(); return Ok(_api.Error(User,"Unable to generate Nationality Code")); }
                        MasterTable.Rows[0]["x_NationalityCode"] = xNationalityCode;
                    }
                   
                    MasterTable.Columns.Remove("n_FnYearID");
                    MasterTable.AcceptChanges();
                    string X_Nationality= MasterTable.Rows[0]["X_Nationality"].ToString();
                    string DupCriteria = "X_Nationality='" + X_Nationality + "'";
                    nNationalityID = dLayer.SaveData("Pay_Nationality", "n_NationalityID",DupCriteria,"", MasterTable, connection, transaction);
                    if (nNationalityID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Nationality Created"));
                    }
                        
                   
                    
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(User,ex));
            }
        }




        [HttpGet("details")]
        public ActionResult GetDetails(int nNationalityID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "select * from Pay_Nationality where N_NationalityID=@nNationalityID";
            Params.Add("@nNationalityID", nNationalityID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }


        [HttpGet("list")]
        public ActionResult GetNationality()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "select N_NationalityID, X_Nationality, X_NationalityLocale, X_NationalityCode, D_Entrydate, X_Country, B_Default, X_Currency, N_CountryID from Pay_Nationality";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }




        [HttpDelete("delete")]
        public ActionResult DeleteData(int nNationalityID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Pay_Nationality", "n_NationalityID", nNationalityID, "", connection);
                    if (Results > 0)
                    {
                        return Ok(_api.Success("Nationality deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete "));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }



    }
}