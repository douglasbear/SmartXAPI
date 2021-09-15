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
    [Route("projectReview")]
    [ApiController]
    public class Prj_ProjectReview : ControllerBase
    {

        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int N_FormID;

        public Prj_ProjectReview(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 54;
        }

        [HttpPost("Save")]
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
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    string DocNo = "";
                    int n_ProjectID = myFunctions.getIntVAL(MasterRow["N_ProjectID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int N_StarRating = myFunctions.getIntVAL(MasterRow["n_StarRating"].ToString());
                    string X_ReviewCaption = MasterRow["X_ReviewCaption"].ToString();

                   if(n_ProjectID>0)
                   {
                    dLayer.ExecuteNonQuery("update Inv_CustomerProjects set X_ReviewCaption='" +X_ReviewCaption+ "' where N_CompanyID="+N_CompanyID+" and N_ProjectID=" +n_ProjectID, Params, connection);
                    dLayer.ExecuteNonQuery("update Inv_CustomerProjects set N_StarRating=" +N_StarRating+ " where N_CompanyID="+N_CompanyID+" and N_ProjectID=" +n_ProjectID, Params, connection);
                   }

                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int n_ProjectReviewID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    Results = dLayer.DeleteData("Prj_ProjectReview", "N_ProjectReviewID", n_ProjectReviewID, "", connection);
                    if (Results > 0)
                    {
                        return Ok(_api.Success("deleted"));
                    }
                    else
                    {
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
        public ActionResult PrjReview(string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            string sqlCommandText = "";
            string Searchkey = "";
            Params.Add("@p1", nCompanyId);
            if (xSearchkey != null && xSearchkey.Trim() != "")

                Searchkey = " and (x_ReviewCode like '%" + xSearchkey + "%' or X_ProjectName like'%" + xSearchkey + "%'or X_CustomerName like'%" + xSearchkey + "%' )";
            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ProjectID desc";



            sqlCommandText = "select * from vw_InvCustomerProjects where N_CompanyID=@p1 " + xSortBy + Searchkey;

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_InvCustomerProjects where N_CompanyID=@p1" + Searchkey;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        //return Ok(_api.Warning("No Results Found"));
                        return Ok(_api.Success(OutPut));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return BadRequest(_api.Error(User, e));
            }
        }



    }
}


