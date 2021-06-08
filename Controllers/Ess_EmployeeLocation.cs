using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("employeelocation")]
    [ApiController]



    public class Ess_EmployeeLocation : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 64;


        public Ess_EmployeeLocation(IDataAccessLayer dl, IApiFunctions apiFun, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("listlocation")] 
        public ActionResult OpportunityList(int nEmpID, int nFnYearID,string deviceID)
        {
            DataTable location = new DataTable();
            DataTable devices = new DataTable();
            DataTable Approvals= new DataTable();
            // Approvals = ds.Tables["approval"];
            DataRow ApprovalRow = Approvals.Rows[0];
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);


            string sqlLocation = "SELECT Pay_WorkLocation.* FROM  Pay_WorkLocation LEFT OUTER JOIN  Pay_Employee ON Pay_WorkLocation.N_LocationId = Pay_Employee.N_WorkLocationID AND Pay_WorkLocation.N_CompanyId = Pay_Employee.N_CompanyID where Pay_Employee.N_CompanyID=@p1 and Pay_Employee.N_EmpID=@p2 and Pay_Employee.N_FnYearID=@nFnYearID ";
            string sqlDevices = "SELECT * FROM  Pay_EmpDeviceIDRegistration where N_CompanyID=@p1 and N_EmpID=@p2 and X_DeviceID=@deviceID and B_Active=1";
           
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nEmpID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@deviceID", deviceID);



            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    location = dLayer.ExecuteDataTable(sqlLocation, Params, connection);
                    devices = dLayer.ExecuteDataTable(sqlDevices, Params, connection);

                    OutPut.Add("locations", api.Format(location));
                    OutPut.Add("devices", api.Format(devices));

                    return Ok(api.Success(OutPut));

                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nLocationId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LocationId"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string LocationCode = "";
                    var values = MasterTable.Rows[0]["X_LocationCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        LocationCode = dLayer.GetAutoNumber("Pay_WorkLocation", "X_LocationCode", Params, connection, transaction);
                        if (LocationCode == "") { transaction.Rollback(); return Ok(api.Error("Unable to generate Location Code")); }
                        MasterTable.Rows[0]["X_LocationCode"] = LocationCode;
                    }


                    nLocationId = dLayer.SaveData("Pay_WorkLocation", "N_LocationId", MasterTable, connection, transaction);
                    if (nLocationId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Location Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }
    }

}