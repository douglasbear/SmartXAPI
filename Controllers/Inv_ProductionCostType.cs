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
    [Route("productionCostType")]
    [ApiController]
    public class Inv_ProductionCostType : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;

        public Inv_ProductionCostType(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1831;
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
                    int nCostTypeID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CostTypeID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    string xCostTypeCode = MasterTable.Rows[0]["x_CostTypeCode"].ToString();      

                    if (xCostTypeCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
 
                        xCostTypeCode = dLayer.GetAutoNumber("Inv_ProductionCostType", "x_CostTypeCode", Params, connection, transaction);
                        if (xCostTypeCode == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to generate CostType Code"));
                        }
                        MasterTable.Rows[0]["x_CostTypeCode"] = xCostTypeCode;
                    }
                    MasterTable.Columns.Remove("n_FnYearID");
                    nCostTypeID = dLayer.SaveData("Inv_ProductionCostType", "N_CostTypeID", MasterTable, connection, transaction);

                    if (nCostTypeID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("Unable to save"));
                    }
                    else
                        transaction.Commit();
                    {
                        return Ok(_api.Success("Save Successfully"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCostTypeID,int nfnYearID)
        {
            int Results = 0;
            SortedList Params = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // DataTable TransData = new DataTable();
                    Params.Add("@N_CostTypeID", nCostTypeID);

                    // string Sql = "select N_CostTypeID,X_CostTypeCode from Inv_ProductionCostType where N_CostTypeID=@N_CostTypeID and N_CompanyID=N_CompanyID";
                    // string xButtonAction="Delete";
                    // string xCostTypeCode = "";
                    // TransData = dLayer.ExecuteDataTable(Sql, Params, connection);
                    SqlTransaction transaction = connection.BeginTransaction();

                    Results = dLayer.DeleteData("Inv_ProductionCostType", "N_CostTypeID", nCostTypeID, "", connection, transaction);
                    
                    // // Activity Log
                    // string ipAddress = "";
                    // if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    //     ipAddress = Request.Headers["X-Forwarded-For"];
                    // else
                    //     ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    //     myFunctions.LogScreenActivitys(myFunctions.getIntVAL( nfnYearID.ToString()),nCostTypeID,TransData.Rows[0]["X_CostTypeCode"].ToString(),1831,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                    
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Production CostType Deleted"));
                    }
                    else
                    {
                       transaction.Rollback();
                       return Ok(_api.Error(User, "Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("list")]
        public ActionResult CostTypeList()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@p1", nCompanyID);

                    string sqlCommandText = "select * from Inv_ProductionCostType where N_CompanyID=@p1";
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    dt = _api.Format(dt);
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
                return BadRequest(_api.Error(User, e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetData(int nCostTypeID, int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from VW_OtherCostType where N_CompanyID=@p1 and X_CostTypeCode=@p2 and n_fnyearID=@p3";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nCostTypeID);
            Params.Add("@p3", nFnYearId);
            


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt =_api.Format(dt);
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
                return Ok(_api.Error(User, e));
            }
        }

    }

}