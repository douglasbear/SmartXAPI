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
    [Route("CopyProducts")]
    [ApiController]
    public class Inv_CopyProducts : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;

        public Inv_CopyProducts(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 395;
        }

         [HttpGet("list")]
        public ActionResult CompanyList(string XUserID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@XUserID", XUserID);

                    string sqlCommandText = "select * from Acc_Company where N_CompanyID in ( select N_CompanyID from Sec_User where x_userID=@XUserID)";

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

        public ActionResult GetData(int nCompanyID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();      
            string sqlCommandCount = "";
            string sqlCommandText = "select * from Inv_ItemMaster where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyID);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from Inv_ItemMaster where N_CompanyID=@p1";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                     OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
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
      public ActionResult UpdateStatus([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                { 
                     connection.Open();
                    DataTable mastertable = new DataTable();
                    mastertable = ds.Tables["master"];
                    SortedList OutPut = new SortedList();
                   SqlTransaction transaction = connection.BeginTransaction();
         
                            SortedList Params = new SortedList();
                            Params.Add("N_CompanyID", myFunctions.getIntVAL(mastertable.Rows[0]["N_CompanyID"].ToString()));
                            Params.Add("N_FnYearID", myFunctions.getIntVAL(mastertable.Rows[0]["N_FnYearID"].ToString()));
                            Params.Add("N_BranchID", myFunctions.getIntVAL(mastertable.Rows[0]["N_BranchID"].ToString()));
                            Params.Add("N_LocationID", myFunctions.getIntVAL(mastertable.Rows[0]["N_LocationID"].ToString()));
                            try
                            {
                             dLayer.ExecuteNonQueryPro("SP_SetupData_cloud", Params, connection, transaction);
                           transaction.Commit();
                    return Ok(_api.Success("Product Copied"));
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, ex));
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
