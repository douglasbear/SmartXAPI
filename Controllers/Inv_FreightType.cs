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
    [Route("freightType")]
    [ApiController]
    public class Inv_FreightType : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;

        public Inv_FreightType(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 395;
        }

        [HttpGet("list")]
        public ActionResult FreightTypeList()
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

                    string sqlCommandText = "select * from Inv_PurchaseFreightReason where N_CompanyID=@p1 order by N_ReasonID asc";

                    SortedList OutPut = new SortedList();
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

        public ActionResult GetData(int xReasonCode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from Inv_PurchaseFreightReason where N_CompanyID=@p1 and x_ReasonCode=@p2";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", xReasonCode);


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
                    int nResonID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ReasonID"].ToString());
                     int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    string xReasonCode = MasterTable.Rows[0]["x_ReasonCode"].ToString();
                     MasterTable.Columns.Remove("n_FnYearID");


                    if (xReasonCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                          Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);

                        
                        
                        xReasonCode = dLayer.GetAutoNumber("Inv_PurchaseFreightReason", "x_ReasonCode", Params, connection, transaction);
                        if (xReasonCode == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to generate Freight Code"));
                        }
                        MasterTable.Rows[0]["x_ReasonCode"] = xReasonCode;
                    }

                    else
                    {
                        dLayer.DeleteData("Inv_PurchaseFreightReason", "N_ReasonID", nResonID, "", connection, transaction);
                    }
                    nResonID = dLayer.SaveData("Inv_PurchaseFreightReason", "N_ReasonID", MasterTable, connection, transaction);

                    if (nResonID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("save successfully"));
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











