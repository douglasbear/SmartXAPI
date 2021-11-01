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
    [Route("freeTextPurchase")]
    [ApiController]
    public class Inv_FreeTextPurchase : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;

        public Inv_FreeTextPurchase(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 380;
        }
        private readonly string connectionString;
        [HttpGet("list")]
        public ActionResult FreeTextPurchaseList(int nFnYearID, int nBranchID, int nPage, int nSizeperpage, bool b_AllBranchData, string xSearchkey, string xSortBy)
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
                    string xTransType = "FTPURCHASE";
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
                        xCriteria = " and N_PurchaseType=0 and X_TransType=@p4 and B_YearEndProcess=0 and N_CompanyID=@p1";
                     else
                        xCriteria = "and N_PurchaseType=0 and X_TransType=@p4 and B_YearEndProcess=0 and N_BranchID=@p3 and N_CompanyID=@p1";
                    }
                else
                    {
                    if (b_AllBranchData)
                        xCriteria = "and N_PurchaseType=0 and X_TransType=@p4 and N_FnYearID=@p2 and N_CompanyID=@p1";
                    else
                        xCriteria = "and N_PurchaseType=0 and X_TransType=@p4 and N_FnYearID=@p2 and N_BranchID=@p3 and N_CompanyID=@p1";
                    }

                if (xSearchkey != null && xSearchkey.Trim() != "")
                    Searchkey = "and ( X_ServiceCode like '%" + xSearchkey + "%' or  D_EntryDate like '%" + xSearchkey + "%' or  X_CustomerName like '%" + xSearchkey + "%' or  X_Remarks like '%" + xSearchkey + "%' ) ";

                if (xSortBy == null || xSortBy.Trim() == "")
                    xSortBy = " order by N_purchaseID desc";
                else
                    xSortBy = " order by " + xSortBy;
                if (Count == 0)
                    sqlCommandText = "select top(" + nSizeperpage + ")  * from vw_InvPurchaseInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2  " + Searchkey + xCriteria;
                else
                    sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvPurchaseInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + xCriteria + "and N_PurchaseID not in (select top(" + Count + ") N_PurchaseID from vw_InvPurchaseInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2) " + Searchkey + xCriteria;

                SortedList OutPut = new SortedList();

                dt = dLayer.ExecuteDataTable(sqlCommandText + xSortBy, Params, connection);
                sqlCommandCount = "select count(*) as N_Count  from vw_InvPurchaseInvoiceNo_Search where N_FnYearID=@p2 " + xCriteria + Searchkey;
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