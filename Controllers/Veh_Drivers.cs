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
namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("vehicleDrivers")]
    [ApiController]
    public class Veh_Drivers : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;

        public Veh_Drivers(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1160;
        }

        [HttpGet("list")]
        public ActionResult VehicleDriversList(int nBranchID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, bool bAllBranchData)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "", xCriteria = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nBranchID);

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ( X_DriversCode like '%" + xSearchkey + "%' or X_DriversName like '%" + xSearchkey + "%' or X_Address like '%" + xSearchkey + "%' or X_PhoneNo like '%" + xSearchkey + "%') ";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_DriversID desc";
            else
                xSortBy = " order by " + xSortBy;

            if (bAllBranchData)
                xCriteria = " N_CompanyID=@p1 ";
            else
                xCriteria = " N_CompanyID=@p1 and N_BranchID=@p2 ";

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from Veh_Drivers where" + xCriteria + Searchkey + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from Veh_Drivers where" + xCriteria + Searchkey + "and N_DriversID not in (select top(" + Count + ") N_DriversID from Veh_Drivers where" + xCriteria + xSortBy + " ) " + xSortBy;

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count from Veh_Drivers where" + xCriteria + Searchkey;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetVehicleDriver(int nFnYearID, int nBranchID, string xDriversCode, bool bAllBranchData)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string xCriteria = "", sqlCommandText = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nBranchID);
            Params.Add("@p3", xDriversCode);

            if (bAllBranchData)
                xCriteria = " N_CompanyID=@p1";
            else
                xCriteria = " N_CompanyID=@p1 and N_BranchID=@p2";

            sqlCommandText = "select * from Veh_Drivers where" + xCriteria + "and xDriversCode=@p3";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(dt));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
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
                    int nDriversID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_DriversID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    string xDriversCode = MasterTable.Rows[0]["x_DriversCode"].ToString();

                    if (xDriversCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        xDriversCode = dLayer.GetAutoNumber("Veh_Drivers", "x_DriversCode", Params, connection, transaction);
                        if (xDriversCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Drivers Code")); }
                        MasterTable.Rows[0]["x_DriversCode"] = xDriversCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Veh_Drivers", "N_DriversID", nDriversID, "", connection, transaction);
                    }

                    nDriversID = dLayer.SaveData("Veh_Drivers", "N_DriversID", MasterTable, connection, transaction);
                    if (nDriversID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Vehicle Driver Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nDriversID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Veh_Drivers", "N_DriversID", nDriversID, "", connection);
                    if (Results > 0)
                    {
                        return Ok(_api.Success("Vehicle Driver deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Unable to delete Vehicle Driver"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

    }
}