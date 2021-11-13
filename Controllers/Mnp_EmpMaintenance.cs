using AutoMapper;
using SmartxAPI.Data;
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
    [Route("mnpEmpMaintenance")]
    [ApiController]
    public class Mnp_EmpMaintenance : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Mnp_EmpMaintenance(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1005;
        }


        [HttpGet("list")]
        public ActionResult EmpMaintenanceList(int nCompanyId,int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            //int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Criteria ="";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and X_EmployeeName like '%" + xSearchkey + "%'";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_MaintenanceID desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Man_EmployeeMaintenance where N_CompanyID=@nCompanyId " + Searchkey + Criteria + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Man_EmployeeMaintenance where N_CompanyID=@nCompanyId " + Searchkey + Criteria + " and N_MaintenanceID not in (select top(" + Count + ") N_MaintenanceID from vw_Man_EmployeeMaintenance where N_CompanyID=@nCompanyId " + Criteria + xSortBy + " ) " + xSortBy;
            Params.Add("@nCompanyId", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_Man_EmployeeMaintenance where N_CompanyID=@nCompanyId " + Searchkey + Criteria ;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("employeelist")]
        public ActionResult GetEmployeeList(int? nCompanyID, int nFnYearID, bool bAllBranchData, int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@bAllBranchData", bAllBranchData);
            Params.Add("@nBranchID", nBranchID);
            string sqlCommandText = "";
            if (bAllBranchData == true)
                sqlCommandText = "Select N_CompanyID,N_EmpID,X_EmpCode,X_EmpName,X_Nationality,N_PositionID,X_Position,X_Phone1,N_FnYearID from vw_PayEmployee Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and  N_EmpID not in (select N_EmpID from Mnp_EmployeeMaintenance where N_CompanyID=@nCompanyID)";
            else
                sqlCommandText = "Select N_CompanyID,N_EmpID,X_EmpCode,X_EmpName,X_Nationality,N_PositionID,X_Position,X_Phone1,N_FnYearID from vw_PayEmployee Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and (N_BranchID=0 or N_BranchID=@nBranchID) and  N_EmpID not in (select N_EmpID from Mnp_EmployeeMaintenance where N_CompanyID=@nCompanyID and (N_BranchID=0 or N_BranchID=@nBranchID))";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("details")]
        public ActionResult EmpMaintenanceListDetails(int nCompanyId,string xMaintenanceCode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            //int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_Man_EmployeeMaintenance where N_CompanyID=@nCompanyId and X_MaintenanceCode=@xMaintenanceCode";
            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@xMaintenanceCode", xMaintenanceCode);


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
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
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nMaintenanceID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_MaintenanceID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    string MaintenanceCode = "";
                    var values = MasterTable.Rows[0]["x_MaintenanceCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.FormID);
                        MaintenanceCode = dLayer.GetAutoNumber("Mnp_EmployeeMaintenance", "x_MaintenanceCode", Params, connection, transaction);
                        if (MaintenanceCode == "") {transaction.Rollback(); return Ok(api.Error(User,"Unable to generate Activity Code")); }
                        MasterTable.Rows[0]["x_MaintenanceCode"] = MaintenanceCode;
                    }
                    if(nMaintenanceID>0)
                    {
                        dLayer.DeleteData("Mnp_EmployeeMaintenance", "N_MaintenanceID", nMaintenanceID, "", connection, transaction);
                    }
                    nMaintenanceID = dLayer.SaveData("Mnp_EmployeeMaintenance", "n_MaintenanceID", MasterTable, connection, transaction);
                    if (nMaintenanceID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Manpower Maintenance Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nMaintenanceID)
        {

            int Results = 0;
            try
            {


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Mnp_EmployeeMaintenance", "N_MaintenanceID", nMaintenanceID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("N_MaintenanceID", nMaintenanceID.ToString());
                    return Ok(api.Success(res, "Manpower Maintenance deleted"));
                }
                else
                {
                    return Ok(api.Error(User,"Unable to delete Manpower Maintenance"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }



        }
    }
}