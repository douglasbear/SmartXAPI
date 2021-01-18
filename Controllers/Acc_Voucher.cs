using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("voucher")]
    [ApiController]
    public class Acc_PaymentVoucher : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Acc_PaymentVoucher(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString(myCompanyID._ConnectionString);
        }


        [HttpGet("list")]
        public ActionResult GetPaymentVoucherList(int? nCompanyId, int nFnYearId, string voucherType, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and [Voucher No] like '%" + xSearchkey + "%' or [Account] like '%" + xSearchkey + "%' or X_Remarks like '%" + xSearchkey + "%'";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_VoucherID desc";
            else
            {
             switch (xSortBy.Split(" ")[0]){ 
                    case "voucherNo" : xSortBy ="[Voucher No] " + xSortBy.Split(" ")[1] ;
                    break;
                    case "voucherDate" : xSortBy ="[Voucher Date] " + xSortBy.Split(" ")[1] ;
                    break;
                    default : break;
                }
             xSortBy = " order by " + xSortBy;
            }
               
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_AccVoucher_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 and X_TransType=@p3 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_AccVoucher_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and X_TransType=@p3 and N_VoucherId not in (select top(" + Count + ") N_VoucherId from vw_AccVoucher_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 and X_TransType=@p3 " + xSortBy + " ) " + xSortBy;


            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", voucherType);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from vw_AccVoucher_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 and X_TransType=@p3";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(OutPut));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
        [HttpGet("details")]
        public ActionResult GetVoucherDetails(int? nCompanyId, int nFnYearId, string xVoucherNo, string xTransType)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();

            string sqlCommandText = "Select * from Acc_VoucherMaster left outer join Acc_MastLedger on Acc_VoucherMaster.N_DefLedgerID = Acc_MastLedger.N_LedgerID and Acc_VoucherMaster.N_FnYearID=Acc_MastLedger.N_FnYearID and Acc_VoucherMaster.N_CompanyID=Acc_MastLedger.N_CompanyID    Where  Acc_VoucherMaster.X_VoucherNo=@VoucherNo and Acc_VoucherMaster.N_CompanyID=@CompanyID and X_TransType=@TransType  AND Acc_VoucherMaster.N_FnYearID =@FnYearID Order By D_VoucherDate";
            Params.Add("@CompanyID", nCompanyId);
            Params.Add("@FnYearID", nFnYearId);
            Params.Add("@VoucherNo", xVoucherNo);
            Params.Add("@TransType", xTransType);
            int nFormID = 0;

            if (xTransType.ToLower() == "pv")
                nFormID = 1;
            else if (xTransType.ToLower() == "rv")
                nFormID = 2;
            else if (xTransType.ToLower() == "jv")
                nFormID = 3;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable Voucher = new DataTable();
                    Voucher = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    Voucher = api.Format(Voucher, "Master");
                    dt.Tables.Add(Voucher);
                    int nVoucherID = myFunctions.getIntVAL(Voucher.Rows[0]["N_VoucherID"].ToString());
                    Params.Add("@VoucherID", nVoucherID);

                    string sqlCommandText2 = "Select Acc_VoucherMaster_Details.*,Acc_MastLedger.*,Acc_MastGroup.X_Type,Acc_TaxCategory.X_DisplayName,Acc_TaxCategory.N_Amount,Acc_CashFlowCategory.X_Description AS X_TypeCategory from Acc_VoucherMaster_Details inner join Acc_MastLedger on Acc_VoucherMaster_Details.N_LedgerID = Acc_MastLedger.N_LedgerID and Acc_VoucherMaster_Details.N_CompanyID=Acc_MastLedger.N_CompanyID inner join Acc_MastGroup On Acc_MastLedger.N_GroupID=Acc_MastGroup.N_GroupID and Acc_MastLedger.N_CompanyID=Acc_MastGroup.N_CompanyID  and Acc_MastLedger.N_FnYearID=Acc_MastGroup.N_FnYearID  LEFT OUTER JOIN Acc_TaxCategory ON Acc_VoucherMaster_Details.N_TaxCategoryID1 = Acc_TaxCategory.N_PkeyID LEFT OUTER JOIN Acc_CashFlowCategory on Acc_VoucherMaster_Details.N_TypeID=Acc_CashFlowCategory.N_CategoryID Where N_VoucherID =@VoucherID and Acc_VoucherMaster_Details.N_CompanyID=@CompanyID and Acc_MastGroup.N_FnYearID=@FnYearID";
                    DataTable VoucherDetails = new DataTable();
                    VoucherDetails = dLayer.ExecuteDataTable(sqlCommandText2, Params, connection);
                    VoucherDetails = api.Format(VoucherDetails, "details");
                    dt.Tables.Add(VoucherDetails);

                    DataTable Acc_CostCentreTrans = new DataTable();
                    SortedList ProParams = new SortedList();
                    ProParams.Add("N_CompanyID", nCompanyId);
                    ProParams.Add("N_FnYearID", nFnYearId);
                    ProParams.Add("N_VoucherID", nVoucherID);
                    ProParams.Add("N_Flag", nFormID);

                    Acc_CostCentreTrans = dLayer.ExecuteDataTablePro("SP_Acc_Voucher_Disp", ProParams, connection);
                    Acc_CostCentreTrans = api.Format(Acc_CostCentreTrans, "costCenterTrans");
                    dt.Tables.Add(Acc_CostCentreTrans);


                }
                return Ok(api.Success(dt));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        //Save....
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {

            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                DataTable InfoTable;
                DataTable CostCenterTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                CostCenterTable = ds.Tables["costcenter"];
                InfoTable = ds.Tables["info"];
                SortedList Params = new SortedList();

                DataRow masterRow = MasterTable.Rows[0];
                var xVoucherNo = masterRow["x_VoucherNo"].ToString();
                var xTransType = masterRow["x_TransType"].ToString();
                var InvoiceNo = masterRow["x_TransType"].ToString();
                var nCompanyId = masterRow["n_CompanyId"].ToString();
                var nFnYearId = masterRow["n_FnYearId"].ToString();
                int N_VoucherID = myFunctions.getIntVAL(masterRow["n_VoucherID"].ToString());
                var nUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var nFormID = 0;
                if (xTransType.ToLower() == "pv")
                    nFormID = 44;
                else if (xTransType.ToLower() == "rv")
                    nFormID = 45;
                else if (xTransType.ToLower() == "jv")
                    nFormID = 46;

                var xAction = "INSERT";
                if (N_VoucherID > 0)
                {
                    xAction = "UPDATE";
                }

                string ipAddress = "";
                if (Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    if (xVoucherNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyId);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", nFormID);
                        Params.Add("N_BranchID", masterRow["n_BranchId"].ToString());
                        while (true)
                        {
                            xVoucherNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerateBranch", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Acc_VoucherMaster Where X_VoucherNo ='" + xVoucherNo + "' and N_CompanyID= " + nCompanyId + " and X_TransType ='" + xTransType + "' and N_FnYearID =" + nFnYearId, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        if (xVoucherNo == "")
                        {
                            transaction.Rollback();
                            return Ok(api.Error("Unable to generate Invoice Number"));
                        }
                        // xVoucherNo = dLayer.GetAutoNumber("Acc_VoucherMaster", "x_VoucherNo", Params, connection, transaction);
                        // if (xVoucherNo == "") { return Ok(api.Error("Unable to generate Invoice Number")); }

                        MasterTable.Rows[0]["x_VoucherNo"] = xVoucherNo;
                    }
                    else
                    {
                        if (N_VoucherID > 0)
                        {   int dltRes = dLayer.DeleteData("Acc_VoucherDetails", "N_InventoryID", N_VoucherID, "x_transtype='" + xTransType + "' and x_voucherno ='" + xVoucherNo + "' and N_CompanyID =" + nCompanyId + " and N_FnYearID =" + nFnYearId, connection, transaction);
                            // if (dltRes <= 0){transaction.Rollback();return Ok(api.Error("Unable to Update"));}
                             dltRes = dLayer.DeleteData("Acc_VoucherMaster_Details_Segments", "N_VoucherID", N_VoucherID , "N_VoucherID= " + N_VoucherID + " and N_CompanyID = " + nCompanyId + " and N_FnYearID=" + nFnYearId,connection,transaction);
                            // if (dltRes <= 0){transaction.Rollback();return Ok(api.Error("Unable to Update"));}
                             dltRes = dLayer.DeleteData("Acc_VoucherMaster_Details", "N_VoucherID", N_VoucherID, "N_VoucherID= " + N_VoucherID + " and N_CompanyID = " + nCompanyId,connection,transaction);
                            // if (dltRes <= 0){transaction.Rollback();return Ok(api.Error("Unable to Update"));}
                             
                        }
                    }

                    N_VoucherID = dLayer.SaveData("Acc_VoucherMaster", "N_VoucherId", MasterTable, connection, transaction);
                    if (N_VoucherID > 0)
                    {
                        SortedList LogParams = new SortedList();
                        LogParams.Add("N_CompanyID", nCompanyId);
                        LogParams.Add("N_FnYearID", nFnYearId);
                        LogParams.Add("N_TransID", N_VoucherID);
                        LogParams.Add("N_FormID", nFormID);
                        LogParams.Add("N_UserId", nUserId);
                        LogParams.Add("X_Action", xAction);
                        LogParams.Add("X_SystemName", "WebRequest");
                        LogParams.Add("X_IP", ipAddress);
                        LogParams.Add("X_TransCode", xVoucherNo);
                        LogParams.Add("X_Remark", "");

                        //dLayer.ExecuteNonQuery("SP_Log_SysActivity ",LogParams,connection,transaction);

                        for (int j = 0; j < DetailTable.Rows.Count; j++)
                        {
                            DetailTable.Rows[j]["N_VoucherId"] = N_VoucherID;
                        }
                        int N_InvoiceDetailId = dLayer.SaveData("Acc_VoucherMaster_Details", "N_VoucherDetailsID", DetailTable, connection, transaction);

                        if (N_InvoiceDetailId > 0)
                        {
                            SortedList PostingParams = new SortedList();
                            PostingParams.Add("N_CompanyID", nCompanyId);
                            PostingParams.Add("X_InventoryMode", xTransType);
                            PostingParams.Add("N_InternalID", N_VoucherID);
                            PostingParams.Add("N_UserID", nUserId);
                            PostingParams.Add("X_SystemName", "ERP Cloud");
                            object posting = dLayer.ExecuteScalarPro("SP_Acc_InventoryPosting", PostingParams, connection, transaction);
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                            return Ok(api.Error("Unable to Save"));
                        }

                    }
                    //return Ok(api.Success("Data Saved"));
                    // return Ok(api.Success("Data Saved" + ":" + xVoucherNo));
                SortedList Result = new SortedList();
                Result.Add("n_VoucherID",N_VoucherID);
                Result.Add("x_POrderNo",xVoucherNo);
                return Ok(api.Success(Result,"Data Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }
        //Delete....
        [HttpDelete()]
        public ActionResult DeleteData(int nVoucherID, string xTransType)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    SortedList Params = new SortedList();
                    Params.Add("N_CompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("X_TransType", xTransType);
                    Params.Add("N_VoucherID", nVoucherID);
                    Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", Params, connection, transaction);

                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("Voucher deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to delete Voucher"));
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }


        }
        [HttpGet("dummy")]
        public ActionResult GetVoucherDummy(int? nVoucherID)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(connectionString))
                {
                    Con.Open();
                    string sqlCommandText = "select * from Acc_VoucherMaster where N_VoucherID=@p1";
                    SortedList mParamList = new SortedList() { { "@p1", nVoucherID } };
                    DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList, Con);
                    masterTable = api.Format(masterTable, "master");

                    string sqlCommandText2 = "select * from Acc_VoucherMaster_Details where N_VoucherID=@p1";
                    SortedList dParamList = new SortedList() { { "@p1", nVoucherID } };
                    DataTable detailTable = dLayer.ExecuteDataTable(sqlCommandText2, dParamList, Con);
                    detailTable = api.Format(detailTable, "details");

                    // string sqlCommandText3 = "select * from Inv_SaleAmountDetails where N_SalesId=@p1";
                    // DataTable dtAmountDetails = dLayer.ExecuteDataTable(sqlCommandText3, dParamList, Con);
                    // dtAmountDetails = _api.Format(dtAmountDetails, "saleamountdetails");

                    if (detailTable.Rows.Count == 0) { return Ok(new { }); }
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add(masterTable);
                    dataSet.Tables.Add(detailTable);
                    //dataSet.Tables.Add(dtAmountDetails);

                    return Ok(dataSet);

                }
            }
            catch (Exception e)
            {
                return StatusCode(403, api.Error(e));
            }
        }

        [HttpGet("default")]
        public ActionResult GetDefault(int nFnYearID,int nLangID,int nFormID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID",myFunctions.GetCompanyID(User));

            string X_Condn = "";
            if (nFormID == 44)
                X_Condn = " and N_CompanyID=@nCompanyID and B_PaymentVoucher='True'";
            else if (nFormID == 45)
                X_Condn = " and N_CompanyID=@nCompanyID and B_ReceiptVoucher='True'";

            string PaymentType = dLayer.ExecuteScalar("Select X_PayMethod from Acc_PaymentMethodMaster  where  B_isDefault='True'" + X_Condn,Params,connection).ToString();
            int N_BehID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select N_PaymentMethodID from Acc_PaymentMethodMaster where  B_isDefault='True'" + X_Condn,Params, connection).ToString());
            string FieldName = "V " + N_BehID;

                    DataTable QList = myFunctions.GetSettingsTable();
                    QList.Rows.Add("DEFAULT_ACCOUNTS", FieldName);

                    QList.AcceptChanges();

                    DataTable Details = dLayer.ExecuteSettingsPro("SP_GenSettings_Disp", QList, myFunctions.GetCompanyID(User),nFnYearID, connection);
                        SortedList Default = new SortedList(){
                            {"defultPaymentMethodID",N_BehID},
                            {"defultPaymentMethod",PaymentType}
                        };
                        SortedList OutPut = new SortedList(){
                            {"settings",api.Format(Details)},
                            {"default",Default}
                        };
                    return Ok(api.Success(OutPut));
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }


    }
}