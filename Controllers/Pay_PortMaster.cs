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
    [Route("port")]
    [ApiController]

    public class PortMasterController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public PortMasterController(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 890;
        }

        [HttpGet("modeoftransaction")]
        public ActionResult GetTransactionList(int nAirportID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "select * from Ffw_Gen_Defaults where N_DefaultId=8 order by N_TypeId";
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

        [HttpGet("details")]
        public ActionResult GetPortDetails(int nAirportID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_AirPort where N_CompanyId=@p1 and N_IrportID=@nBranchID";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nAirportID);
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
                    int nAirportID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_AirportID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    string xAirportCode = MasterTable.Rows[0]["x_AirportCode"].ToString();

                    MasterTable.Columns.Remove("n_FnYearID");
                    MasterTable.AcceptChanges();
                    if (xAirportCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        xAirportCode = dLayer.GetAutoNumber("Ffw_Airport", "x_AirportCode", Params, connection, transaction);
                        if (xAirportCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Airport Code")); }
                        MasterTable.Rows[0]["x_AirportCode"] = xAirportCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Ffw_Airport", "N_AirportID", nAirportID, "", connection, transaction);
                    }
                    nAirportID = dLayer.SaveData("Ffw_Airport", "N_AirportID", MasterTable, connection, transaction);
                    if (nAirportID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Airport"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Airport Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nAirportID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Ffw_Airport", "N_AirportID", nAirportID, "", connection);
                    if (Results > 0)
                    {
                        return Ok( _api.Success("Airport deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete Airport"));
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