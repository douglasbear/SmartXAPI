using AutoMapper;
using SmartxAPI.Data;
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
    [Route("vendor")]
    [ApiController]
    public class Inv_Vendor : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        public Inv_Vendor(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        //GET api/customer/list?....
        [HttpGet("list")]
        public ActionResult GetVendorList(int? nCompanyId, int nFnYearId, bool bAllBranchesData, string vendorId, string qry, string msg)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string criteria = "";
            if (vendorId != "" && vendorId != null)
            {
                criteria = " and N_VendorID =@vendorId ";
                Params.Add("@vendorId", vendorId);
            }

            string qryCriteria = "";
            if (qry != "" && qry != null)
            {
                qryCriteria = " and (X_VendorCode like @qry or X_VendorName like @qry ) ";
                Params.Add("@qry", "%" + qry + "%");
            }
            string sqlCommandText = "select * from vw_InvVendor where B_Inactive=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3 " + criteria + " " + qryCriteria + " order by X_VendorName,X_VendorCode";
            Params.Add("@p1", 0);
            Params.Add("@p2", nCompanyId);
            Params.Add("@p3", nFnYearId);
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
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    if (msg == "")
                        return Ok(_api.Success(dt));
                    else
                        return Ok(_api.Success(dt, msg));
                }
            }
            catch (Exception e)
            {
                return BadRequest(_api.Error(e));
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
                SortedList Params = new SortedList();
                // Auto Gen
                DataRow MasterRow = MasterTable.Rows[0];
                string VendorCode = "";
                var xVendorCode = MasterRow["x_VendorCode"].ToString();
                int nVendorID = myFunctions.getIntVAL(MasterRow["n_VendorID"].ToString());
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction(); ;
                    if (xVendorCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterRow["n_CompanyId"].ToString());
                        Params.Add("N_YearID", MasterRow["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 52);
                        VendorCode = dLayer.GetAutoNumber("Inv_Vendor", "x_VendorCode", Params, connection, transaction);
                        if (VendorCode == "") { return StatusCode(409, _api.Response(409, "Unable to generate Vendor Code")); }
                        MasterTable.Rows[0]["x_VendorCode"] = VendorCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Inv_Vendor", "N_VendorID", nVendorID, "", connection, transaction);
                    }
                    MasterTable.Columns.Remove("n_VendorID");
                    MasterTable.AcceptChanges();


                    int N_VendorId = dLayer.SaveData("Inv_Vendor", "N_VendorID", nVendorID, MasterTable, connection, transaction);
                    if (N_VendorId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        string msg = "Vendor successfully created";
                        return GetVendorList(myFunctions.getIntVAL(MasterRow["n_CompanyId"].ToString()), myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString()), true, N_VendorId.ToString(), "", msg);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nVendorID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Inv_Vendor", "N_VendorID", nVendorID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    return StatusCode(200, _api.Response(200, "vendor deleted"));
                }
                else
                {
                    return StatusCode(409, _api.Response(409, "Unable to delete vendor"));
                }

            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.ErrorResponse(ex));
            }


        }
    }
}