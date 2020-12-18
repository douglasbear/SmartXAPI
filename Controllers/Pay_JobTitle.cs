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
    [Route("jobtitle")]
    [ApiController]
    public class Pay_JobTitle : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Pay_JobTitle(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 195;
        }

        [HttpGet("list")]
        public ActionResult GetJobTitle()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            string sqlCommandText="Select N_CompanyID,N_PositionID,N_EmpID,N_SalaryGradeID,B_Edit,Code,Description from vw_PayPosition_DispAdvanced Where N_CompanyID=@nCompanyID order by Code";
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

        [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
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
                int nPositionID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PositionID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                string xPositionCode = MasterTable.Rows[0]["x_PositionCode"].ToString();
                MasterTable.Columns.Remove("n_FnYearID");
                MasterTable.AcceptChanges();
                 if (xPositionCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        xPositionCode = dLayer.GetAutoNumber("Pay_Position", "x_PositionCode", Params, connection, transaction);
                        if (xPositionCode == "") { return Ok(_api.Error("Unable to generate Position Code")); }
                        MasterTable.Rows[0]["x_PositionCode"] = xPositionCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Pay_Position", "N_PositionID", nPositionID, "", connection, transaction);
                    }
                    
                    nPositionID=dLayer.SaveData("Pay_Position","N_PositionID",MasterTable,connection,transaction);  
                    transaction.Commit();
                    return Ok(_api.Success("Job Title Saved")) ;
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }


        }
    }