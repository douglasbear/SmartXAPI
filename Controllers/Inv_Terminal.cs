using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("posterminal")]
    [ApiController]
    public class Inv_Terminal : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly string reportPath;



        public Inv_Terminal(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            reportPath = conf.GetConnectionString("ReportPath");
        }


        //GET api/productcategory/list?....
        [HttpGet("list")]
        public ActionResult GetTerminalList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from vw_InvTerminal where N_CompanyID=@p1";
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
        [HttpGet("printlist")]
        public ActionResult GetTerminalPrintList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            dt = myFunctions.AddNewColumnToDataTable(dt,"x_PrintTemplate",typeof(string),"");
            Params.Add("@p1", nCompanyId);
            string Path = reportPath + "printing/Salesinvoice/VAT/";

            try
            {
                string[] files = Directory.GetFiles(Path);
                int row = 0;
                foreach (string file in files)
                {

                    dt.Rows.Add();
                    dt.Rows[row]["x_PrintTemplate"] = System.IO.Path.GetFileName(file);
                    row = row + 1;
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
        public ActionResult GetTerminalDetails(int nTerminalID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_InvTerminal where N_CompanyID=@p1 and N_TerminalID=@p2";

            Params.Add("@p1", myFunctions.GetCompanyID(User));
            Params.Add("@p2", nTerminalID);


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
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    int N_FnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearId"].ToString());
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string TerminalCode = "";
                    var values = MasterTable.Rows[0]["X_TerminalCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterTable.Rows[0]["N_CompanyId"].ToString());
                        Params.Add("N_YearID", N_FnYearId);
                        Params.Add("N_FormID", 895);
                        TerminalCode = dLayer.GetAutoNumber("Inv_Terminal", "X_TerminalCode", Params, connection, transaction);
                        if (TerminalCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Terminal Code")); }
                        MasterTable.Rows[0]["X_TerminalCode"] = TerminalCode;
                    }
                    MasterTable.Columns.Remove("N_FnYearId");
                    int N_TerminalID = dLayer.SaveData("Inv_Terminal", "N_TerminalID", MasterTable, connection, transaction);
                    if (N_TerminalID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {
                        dLayer.SaveData("Inv_Terminaldetails", "N_SettingsID", DetailTable, connection, transaction);
                        transaction.Commit();
                        return Ok(_api.Success("Terminal Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nTerminalID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Inv_Terminal", "N_TerminalID", nTerminalID, "", connection);
                    if (Results > 0)
                    {
                        return Ok(_api.Success("Terminal deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete Terminal"));
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