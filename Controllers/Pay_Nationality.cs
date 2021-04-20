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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nNationalityID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_NationalityID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    string xNationalityCode = MasterTable.Rows[0]["x_NationalityCode"].ToString();

                    if (xNationalityCode == "@Auto")
                    {
                        Params.Add("n_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);

                        xNationalityCode = dLayer.GetAutoNumber("Pay_Nationality", "x_NationalityCode", Params, connection, transaction);

                        if (xNationalityCode == "")
                        { transaction.Rollback(); return Ok(_api.Error("Unable to generate Nationality Code")); }
                        MasterTable.Rows[0]["x_NationalityCode"] = xNationalityCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Pay_Nationality", "N_NationalityID", nNationalityID, "", connection, transaction);

                    }
                    MasterTable.Columns.Remove("n_CompanyID");
                    MasterTable.Columns.Remove("n_FnYearID");

                    nNationalityID = dLayer.SaveData("Pay_Nationality", "N_NationalityID", MasterTable, connection, transaction);


                    transaction.Commit();
                    return Ok(_api.Success("Nationality Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpGet("details")]
        public ActionResult GetDetails(int nNationalityID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            // int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_PayNationality_Disp where N_NationalityID=@nNationalityID";
            // Params.Add("@nCompanyID", nCompanyID);
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
                return Ok(_api.Error(e));
            }
        }


        [HttpGet("list")]
        public ActionResult GetNationality()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            // int nCompanyID=myFunctions.GetCompanyID(User);
            // Params.Add("@nCompanyID",nCompanyID);
            string sqlCommandText="select N_NationalityID, X_Nationality, X_NationalityLocale, X_NationalityCode, D_Entrydate, X_Country, B_Default, X_Currency, N_CountryID from Pay_Nationality";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
                return Ok(_api.Error(e));
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
                        return Ok( _api.Success("Nationality deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete "));
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