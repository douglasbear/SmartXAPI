using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("itemunit")]
    [ApiController]
    public class Inv_ItemUnit : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Inv_ItemUnit(IApiFunctions apiFun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apiFun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }



        [HttpGet("list")]
        public ActionResult GetItemUnitList(int? nCompanyId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select Code,[Unit Code],Description from vw_InvItemUnit_Disp where N_CompanyID=@p1 and N_ItemID is null order by ItemCode,[Unit Code]";
            Params.Add("@p1", nCompanyId);

            try
            {
                dt = dLayer.ExecuteDataTable(sqlCommandText, Params);
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return StatusCode(200, api.Response(200, "No Results Found"));
                }
                else
                {
                    return Ok(dt);
                }
            }
            catch (Exception e)
            {
                return StatusCode(403, api.ErrorResponse(e));
            }
        }

        [HttpGet("listdetails")]
        public ActionResult GetItemUnitListDetails(int? nCompanyId, int? nItemUnitID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select Code,[Unit Code],Description from vw_InvItemUnit_Disp where N_CompanyID=@p1 and code=@p2 order by ItemCode,[Unit Code]";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nItemUnitID);

            try
            {
                dt = dLayer.ExecuteDataTable(sqlCommandText, Params);
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return StatusCode(200, api.Response(200, "No Results Found"));
                }
                else
                {
                    return Ok(dt);
                }
            }
            catch (Exception e)
            {
                return StatusCode(403, api.ErrorResponse(e));
            }
        }

        //Save....
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];

                SortedList Params = new SortedList();
                dLayer.setTransaction();
                int N_ItemUnitID = dLayer.SaveData("Inv_ItemUnit", "N_ItemUnitID", 0, MasterTable);
                if (N_ItemUnitID <= 0)
                {
                    dLayer.rollBack();
                    return StatusCode(409, api.Response(409, "Unable to save ItemUnit"));
                }
                else
                {
                    dLayer.commit();
                }
                return GetItemUnitListDetails(int.Parse(MasterTable.Rows[0]["n_CompanyId"].ToString()), N_ItemUnitID);

            }

            catch (Exception ex)
            {
                dLayer.rollBack();
                return StatusCode(403, api.ErrorResponse(ex));
            }
        }


        [HttpGet("itemwiselist")]
        public ActionResult GetItemWiseUnitList(int? nCompanyId, string baseUnit, int itemId)
        {
            if (baseUnit == null) { baseUnit = ""; }
            try
            {
                SortedList mParamsList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"X_ItemUnit",baseUnit},
                        {"N_ItemID",itemId}
                    };
                DataTable masterTable = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    masterTable = dLayer.ExecuteDataTablePro("SP_FillItemUnit", mParamsList, connection);
                }
                if (masterTable.Rows.Count == 0) { return Ok(api.Notice("No Data Found")); }
                return Ok(api.Success(masterTable));
            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nItemUnitID)
        {
            int Results = 0;
            try
            {

                Results = dLayer.DeleteData("Inv_ItemUnit", "N_ItemUnitID", nItemUnitID, "");
                if (Results > 0)
                {
                    return StatusCode(200, api.Response(200, "Product Unit deleted"));
                }
                else
                {
                    return StatusCode(409, api.Response(409, "Unable to delete product Unit"));
                }

            }
            catch (Exception ex)
            {
                return StatusCode(403, api.ErrorResponse(ex));
            }


        }
    }
}