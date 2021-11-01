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
    [Route("freeTextSales")]
    [ApiController]
    public class Inv_FreeTextSales : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;

        public Inv_FreeTextSales(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 372;
        }
        private readonly string connectionString;
        [HttpGet("list")]
        public ActionResult FreeTextSalesList(int nFnYearID, int nBranchID, int nPage, int nSizeperpage, bool b_AllBranchData, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    string sqlCommandCount = "", xCriteria = "";
                    string xTransType = "FTSALES";
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string Searchkey = "";
                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nFnYearID);
                    Params.Add("@p3", nBranchID);
                    Params.Add("@p4", xTransType);
                    bool CheckClosedYear = Convert.ToBoolean(dLayer.ExecuteScalar("Select B_YearEndProcess From Acc_FnYear Where N_CompanyID=@p1 and N_FnYearID=@p2 ", Params, connection));

                if (!CheckClosedYear)
                    {
                    if (b_AllBranchData)
                        xCriteria = "N_SalesType=0 and X_TransType=@p4 and B_YearEndProcess=0 and N_CompanyId=@p1";
                     else
                        xCriteria = "N_SalesType=0 and X_TransType=@p4 and B_YearEndProcess=0 and N_BranchId=@p3 and N_CompanyId=@p1";
                    }
                else
                    {
                    if (b_AllBranchData)
                        xCriteria = "N_SalesType=0 and X_TransType=@p4 and N_FnYearID=@p2 and N_CompanyId=@p1";
                    else
                        xCriteria = "N_SalesType=0 and X_TransType=@p4 and N_FnYearID=@p2 and N_BranchId=@p3 and N_CompanyId=@p1";
                    }

                if (xSearchkey != null && xSearchkey.Trim() != "")
                    Searchkey = "and ( [Invoice No] like '%" + xSearchkey + "%' ) ";

                if (xSortBy == null || xSortBy.Trim() == "")
                    xSortBy = " order by N_SalesId desc";
                else
                    xSortBy = " order by " + xSortBy;
                if (Count == 0)
                    sqlCommandText = "select top(" + nSizeperpage + ")  * from vw_InvSalesInvoiceNo_Search where " + xCriteria + Searchkey;
                else
                    sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvSalesInvoiceNo_Search where " + xCriteria + Searchkey + "and N_SalesId not in (select top(" + Count + ") N_SalesId from vw_InvSalesInvoiceNo_Search where ) " + xCriteria + Searchkey;

                SortedList OutPut = new SortedList();

                dt = dLayer.ExecuteDataTable(sqlCommandText + xSortBy, Params, connection);
                sqlCommandCount = "select count(*) as N_Count  from vw_InvSalesInvoiceNo_Search where " + xCriteria + Searchkey;
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
                return BadRequest(_api.Error(User, e));
            }
        }

    }
}