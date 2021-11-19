using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("freeTextReturn")]
    [ApiController]
    public class Inv_FreeTextPurchaseReturn : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;

        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        public Inv_FreeTextPurchaseReturn(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 384;

        }



        [HttpGet("list")]
        public ActionResult GetPurchaseReturnList(int? nCompanyId, int nFnYearID, string xTransType, bool bAllBranchData, int nBranchID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "", xCriteria = "", sqlCommandCount = "";
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nFnYearID);
                    Params.Add("@p3", nBranchID);
                    Params.Add("@p4", xTransType);
                    string Searchkey = "";
                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and ([Invoice No] like '%" + xSearchkey + "%' or [Invoice Date like] '%" + xSearchkey + "%')";
                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_PurchaseID desc";
                    else
                        xSortBy = " order by " + xSortBy;

                    if (bAllBranchData)
                        xCriteria = " N_PurchaseType=0 and X_TransType=@p4 and N_FnYearID=@p2 and N_CompanyID=@p1";
                    else
                        xCriteria = " N_PurchaseType=0 and X_TransType=@p4 and N_FnYearID=@p2 and N_BranchID=@p3 and N_CompanyID=@p1";

                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") [Invoice Date] as invoiceDate ,[Invoice No] as invoiceNo ,Vendor,InvoiceNetAmt,x_Description,n_InvDueDays from vw_InvPurchaseInvoiceNo_Search where " + xCriteria + Searchkey;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") [Invoice Date] as invoiceDate,[Invoice No] as invoiceNo ,Vendor,InvoiceNetAmt,x_Description,n_InvDueDays from vw_InvPurchaseInvoiceNo_Search where " + xCriteria + Searchkey + "and N_PurchaseID not in (select top(" + Count + ") N_PurchaseID from vw_InvPurchaseInvoiceNo_Search where ) " + xCriteria + Searchkey;

                    SortedList OutPut = new SortedList();

                    dt = dLayer.ExecuteDataTable(sqlCommandText + xSortBy, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from vw_InvPurchaseInvoiceNo_Search where " + xCriteria + Searchkey;
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
                    DataTable DetailTable;
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nUserID = myFunctions.GetUserID(User);
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                    int nPurchaseID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PurchaseID"].ToString());
                    string X_InvoiceNo = MasterTable.Rows[0]["X_InvoiceNo"].ToString();
                    string xTransType = "CREDIT NOTE";
                    DocNo = MasterRow["X_InvoiceNo"].ToString();
                    if (nPurchaseID > 0)
                    {
                        try
                        {
                            SortedList DelParam = new SortedList();
                            DelParam.Add("N_CompanyID", nCompanyID);
                            DelParam.Add("X_TransType", xTransType);
                            DelParam.Add("N_VoucherID", nPurchaseID);
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DelParam, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex.Message));
                        }
                    }



                    if (DocNo == "@Auto")
                    {

                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("N_BranchID", MasterRow["n_BranchId"].ToString());

                        X_InvoiceNo = dLayer.GetAutoNumber("Inv_Purchase", "x_InvoiceNo", Params, connection, transaction);
                        if (X_InvoiceNo == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to generate Invoice Number"));
                        }
                        MasterTable.Rows[0]["x_InvoiceNo"] = X_InvoiceNo;
                    }
                    nPurchaseID = dLayer.SaveData("Inv_Purchase", "N_PurchaseID", MasterTable, connection, transaction);

                    if (nPurchaseID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save Purchase Invoice!"));
                    }

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {

                        DetailTable.Rows[j]["N_PurchaseID"] = nPurchaseID;

                    }
                    //  DetailTable.Columns.Remove("X_ItemUnit"); (if item unit passes) uncomm this line
                    int N_InvoiceDetailId = dLayer.SaveData("Inv_PurchaseDetails", "n_PurchaseDetailsID", DetailTable, connection, transaction);
                    if (N_InvoiceDetailId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save Purchase Invoice!"));
                    }
                    SortedList PostingParam = new SortedList();
                    PostingParam.Add("N_CompanyID", nCompanyID);
                    PostingParam.Add("X_InventoryMode", xTransType);
                    PostingParam.Add("N_InternalID", nPurchaseID);
                    PostingParam.Add("N_UserID", nUserID);
                    PostingParam.Add("X_SystemName", "WebRequest");
                    try
                    {
                        dLayer.ExecuteNonQueryPro("SP_Acc_InventoryPosting", PostingParam, connection, transaction);
                    }
                    catch (Exception ex)
                    {

                        return Ok(_api.Error(User, ex));
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Successfully saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPurchaseID, string X_TransType)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    var nUserID = myFunctions.GetUserID(User);
                    SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType",X_TransType},
                                {"N_VoucherID",nPurchaseID},
                                {"N_UserID",nUserID},
                                 {"X_SystemName","WebRequest"}};
                    int Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);

                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete Purchase"));
                    }
                    transaction.Commit();
                    return Ok(_api.Success(" Purchase deleted"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
        [HttpGet("listdetails")]
        public ActionResult GetPurchaseeDetails(int nCompanyId, int nFnYearId, string xInvoiceNO, string xTransType, bool showAllBranch, int nBranchId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable Master = new DataTable();
                    DataTable Details = new DataTable();
                    int N_PurchaseID = 0;
                    string X_MasterSql = "";
                    string X_DetailsSql = "";
                    X_MasterSql = "Select * from vw_Inv_FreeTextPurchase_Disp  Where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and X_TransType='" + xTransType + "' and X_InvoiceNo='" + xInvoiceNO + "' ";
                    Master = dLayer.ExecuteDataTable(X_MasterSql, Params, connection);
                    if (Master.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                    N_PurchaseID = myFunctions.getIntVAL(Master.Rows[0]["N_PurchaseID"].ToString());
                    Master = _api.Format(Master, "Master");


                    X_DetailsSql="Select * From vw_Inv_Purchasedetails Where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + "   and N_PurchaseID= " + N_PurchaseID+ "";
                    
                    Details = dLayer.ExecuteDataTable(X_DetailsSql, Params, connection);
                    Details = _api.Format(Details, "Details");
                    dt.Tables.Add(Details);
                    dt.Tables.Add(Master);
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



































