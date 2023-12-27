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
    [Route("yearEndWizard")]
    [ApiController]
    public class Frm_YearEndWizard : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;

        public Frm_YearEndWizard(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 142;
        }

        [HttpGet("loadNextYear")]
        public ActionResult LoadNextYear()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("N_CompanyID", nCompanyID);

                    SortedList OutPut = new SortedList();
                    dt = dLayer.ExecuteDataTablePro("SP_Acc_NextYear_Sel", Params, connection, transaction);
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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("checkYearCreated")]
        public ActionResult YearCreated(int nFnYearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nFnYearID", nFnYearID);


                    DataTable ValidationTable = new DataTable();
                    DataTable NextYear = new DataTable();



                    int N_FnYearID = 0, N_MaxFnYearID = 0;
                    bool N_CurFnYear = false, B_CurTransferProcess = false, B_TransactionStarted = false, B_PreliminaryYr = false, B_EntryOpeningBalance = false;

                    ValidationTable.Clear();
                    ValidationTable.Columns.Add("N_FnYearID");
                    ValidationTable.Columns.Add("N_MaxFnYearID");
                    ValidationTable.Columns.Add("N_CurFnYear");
                    ValidationTable.Columns.Add("B_CurTransferProcess");
                    ValidationTable.Columns.Add("B_TransactionStarted");
                    ValidationTable.Columns.Add("B_PreliminaryYr");
                    ValidationTable.Columns.Add("B_EntryOpeningBalance");

                    N_CurFnYear = myFunctions.getBoolVAL((dLayer.ExecuteScalar("Select ISNULL(B_YearEndProcess,'') as B_YearEndProcess FRom Acc_FnYear Where N_FnYearID =@nFnYearID  and N_CompanyID =@nCompanyID", Params, connection)).ToString());
                    N_MaxFnYearID = myFunctions.getIntVAL((dLayer.ExecuteScalar("Select top 1 N_FnYearID from Acc_FnYear where N_CompanyID =@nCompanyID order by D_Start desc", Params, connection)).ToString());
                    if (N_MaxFnYearID > nFnYearID)
                    {
                        object Obj_TransactionStarted = dLayer.ExecuteScalar("Select convert(bit,1) FRom Acc_VoucherMaster Where N_FnYearID =" + N_MaxFnYearID + " and N_CompanyID =@nCompanyID and X_transType = 'OB'", Params, connection);
                        if (Obj_TransactionStarted != null)
                            B_TransactionStarted = myFunctions.getBoolVAL(Obj_TransactionStarted.ToString());
                    }
                    B_CurTransferProcess = myFunctions.getBoolVAL(dLayer.ExecuteScalar("Select ISNULL(B_TransferProcess,'') as B_TransferProcess FRom Acc_FnYear Where N_FnYearID =@nFnYearID and N_CompanyID =@nCompanyID", Params, connection).ToString());
                    B_PreliminaryYr = myFunctions.getBoolVAL(dLayer.ExecuteScalar("Select ISNULL(B_PreliminaryYear,'') as B_PreliminaryYear FRom Acc_FnYear Where N_FnYearID =@nFnYearID and N_CompanyID =@nCompanyID ", Params, connection).ToString());
                    object EntryValBal = null;
                    EntryValBal = dLayer.ExecuteScalar("Select ISNULL(B_EntryOpeningBalance,'') as B_EntryOpeningBalance FRom Acc_FnYear Where  N_CompanyID =@nCompanyID  and D_Start > ( Select D_Start FRom Acc_FnYear Where N_FnYearID =@nFnYearID  and N_CompanyID =@nCompanyID) order by D_Start", Params, connection);
                    if (EntryValBal != null)
                    {
                        B_EntryOpeningBalance = myFunctions.getBoolVAL(dLayer.ExecuteScalar("Select ISNULL(B_EntryOpeningBalance,'') as B_EntryOpeningBalance FRom Acc_FnYear Where  N_CompanyID =@nCompanyID  and D_Start > ( Select D_Start FRom Acc_FnYear Where N_FnYearID =@nFnYearID and N_CompanyID =@nCompanyID) order by D_Start", Params, connection).ToString());
                    }



                    DataRow row = ValidationTable.NewRow();
                    row["N_FnYearID"] = nFnYearID;
                    row["N_MaxFnYearID"] = N_MaxFnYearID;
                    row["N_CurFnYear"] = N_CurFnYear;
                    row["B_CurTransferProcess"] = B_CurTransferProcess;
                    row["B_TransactionStarted"] = B_TransactionStarted;
                    row["B_PreliminaryYr"] = B_PreliminaryYr;
                    row["B_EntryOpeningBalance"] = B_EntryOpeningBalance;

                    ValidationTable.Rows.Add(row);
                    ValidationTable = _api.Format(ValidationTable, "ValidationTable");
                    //  Disable create new year if new year exists.......


                    string sql = "Select top 1 ISNULL(N_FnYearID,0) as N_FnYearID,B_YearEndProcess from Acc_FnYear Where N_CompanyID =@nCompanyID  and D_Start > ( Select D_Start FRom Acc_FnYear Where N_FnYearID =@nFnYearID and N_CompanyID =@nCompanyID) order by D_Start";
                    NextYear = dLayer.ExecuteDataTable(sql, Params, connection);
                    NextYear = _api.Format(NextYear, "NextYear");


                    dt.Tables.Add(ValidationTable);
                    dt.Tables.Add(NextYear);



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
                    DataTable MasterTable;

                    SqlTransaction transaction = connection.BeginTransaction();
                    MasterTable = ds.Tables["master"];
                    int nCompanyID = myFunctions.GetCompanyID(User);

                    SortedList Params = new SortedList();



                    int n_FnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                    int nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_UserID"].ToString());
                    bool b_NewYear = false;
                    string accountCode = "";
                    if (MasterTable.Columns.Contains("b_NewYear"))
                        b_NewYear = myFunctions.getBoolVAL(MasterTable.Rows[0]["b_NewYear"].ToString());
                    bool b_closeYear = false;
                    if (MasterTable.Columns.Contains("b_closeYear"))
                        b_closeYear = myFunctions.getBoolVAL(MasterTable.Rows[0]["b_closeYear"].ToString());
                    bool b_TransferBalance = false;
                    if (MasterTable.Columns.Contains("b_TransferClosingBal"))
                        b_TransferBalance = myFunctions.getBoolVAL(MasterTable.Rows[0]["b_TransferClosingBal"].ToString());
                    var d_DateFrom = "";
                    if (MasterTable.Columns.Contains("d_DateFrom"))
                        d_DateFrom = (MasterTable.Rows[0]["d_DateFrom"].ToString());
                    var d_DateTo = "";
                    if (MasterTable.Columns.Contains("d_DateTo"))
                        d_DateTo = (MasterTable.Rows[0]["d_DateTo"].ToString());
                    bool B_CurTransferProcess = myFunctions.getBoolVAL(dLayer.ExecuteScalar("Select ISNULL(B_TransferProcess,'') as B_TransferProcess FRom Acc_FnYear Where N_FnYearID =" + n_FnYearId + " and N_CompanyID =" + nCompanyID + "", Params, connection, transaction).ToString());
                    object accCode = null;
                    if(b_TransferBalance==true){
                           if (MasterTable.Columns.Contains("N_LedgerID"))
                    {

                        accCode = dLayer.ExecuteScalar("select [Account Code] from vw_AccMastLedger where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + n_FnYearId + " and N_LedgerID=" + myFunctions.getIntVAL(MasterTable.Rows[0]["N_LedgerID"].ToString()) + " ", Params, connection, transaction);
                        accountCode = accCode.ToString();
                    }
                    if (accCode == null && b_NewYear == true)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Please select retained income account"));
                    }
                    }


                    //var dEndDate = (MasterTable.Rows[0]["d_EndDate"].ToString());
                    string X_CustomerVal = "";
                    string X_AccountVal = "";
                    string X_VendorVal = "";
                    int n_TaxTypeID = 0;
                    if (MasterTable.Columns.Contains("x_CustomerVal"))
                        X_CustomerVal = (MasterTable.Rows[0]["x_CustomerVal"].ToString());
                    if (MasterTable.Columns.Contains("x_AccountVal"))
                        X_AccountVal = (MasterTable.Rows[0]["x_AccountVal"].ToString());
                    if (MasterTable.Columns.Contains("x_VendorVal"))
                        X_VendorVal = (MasterTable.Rows[0]["x_VendorVal"].ToString());
                    if (MasterTable.Columns.Contains("n_TaxTypeID"))
                        n_TaxTypeID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_TaxTypeID"].ToString());
                    string Condn = "";
                    bool B_Depreciation = false;
                    int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());

                    int nFnYearID = 0;
                    if (b_NewYear)
                    {
                        SortedList Params2 = new SortedList(){
                            {"N_CompanyID", nCompanyID},
                            {"N_FnYearID_Current", n_FnYearId},
                            {"D_Start", d_DateFrom},
                            {"D_End", d_DateTo},
                            {"Accounts", X_AccountVal},
                            {"Customers", X_CustomerVal},
                            {"Vendors", X_VendorVal},
                            {"N_TaxType", n_TaxTypeID}
                        };
                        nFnYearID = myFunctions.getIntVAL(dLayer.ExecuteScalarPro("SP_FinancialYear_Create_wizard", Params2, connection, transaction).ToString());
                        //g65dLayer.ExecuteScalar("update Acc_FnYear set D_Start = '2023-01-01 00:00:00' where D_Start='2023-01-01 23:59:00'", Params, connection, transaction);
                    }
                    if (b_closeYear)
                    {
                        SortedList Params3 = new SortedList(){
                            {"N_CompanyID", nCompanyID},
                            {"N_FnYearID_Close", n_FnYearId},
                            {"N_FnYearID_New", nFnYearID},
                            {"X_RtainedIncomeLedgerCode", ""},
                            {"N_UserID", nUserID},
                            {"X_Operation", "Close"}
                        };
                        dLayer.ExecuteNonQueryPro("SP_Acc_CloseFinYear", Params3, connection, transaction);
                    }

                    if (nBranchID == 0)

                        Condn = "dbo.Ass_PurchaseDetails.N_FnYearID=" + n_FnYearId + " and dbo.Ass_AssetMaster.N_CompanyID=" + nCompanyID;
                    else
                        Condn = "dbo.Ass_AssetMaster.N_CompanyID=" + nCompanyID + "  and dbo.Ass_AssetMaster.N_BranchID=" + nBranchID + " and dbo.Ass_PurchaseDetails.N_FnYearID=" + n_FnYearId;


                    // DataTable Ass_ItemMaster = dLayer.ExecuteDataTable("SELECT max(dbo.Ass_Depreciation.D_EndDate) AS  D_EndDate,dbo.Ass_AssetMaster.N_ItemID,dbo.Ass_AssetMaster.X_ItemCode, dbo.Ass_AssetMaster.N_BookValue, dbo.Ass_AssetMaster.N_LifePeriod, dbo.Ass_PurchaseDetails.D_PurchaseDate, dbo.Ass_AssetMaster.N_BranchID, dbo.Ass_PurchaseDetails.N_Price,dbo.Ass_AssetMaster.D_PlacedDate,dbo.Ass_AssetMaster.N_CategoryID FROM   dbo.Ass_AssetMaster INNER JOIN dbo.Ass_PurchaseDetails ON dbo.Ass_AssetMaster.N_AssetInventoryDetailsID = dbo.Ass_PurchaseDetails.N_AssetInventoryDetailsID left outer join Ass_Depreciation on Ass_Depreciation.N_ItemID =Ass_AssetMaster.N_ItemID and Ass_Depreciation.N_CompanyID=Ass_AssetMaster.N_CompanyID Where " + Condn + " and dbo.Ass_AssetMaster.N_Status<2  group by dbo.Ass_AssetMaster.N_ItemID,dbo.Ass_AssetMaster.X_ItemCode, dbo.Ass_AssetMaster.N_BookValue, dbo.Ass_AssetMaster.N_LifePeriod, dbo.Ass_PurchaseDetails.D_PurchaseDate, dbo.Ass_AssetMaster.N_BranchID, dbo.Ass_PurchaseDetails.N_Price,dbo.Ass_AssetMaster.D_PlacedDate,dbo.Ass_AssetMaster.N_CategoryID", Params, connection, transaction);

                    // if (Ass_ItemMaster.Rows.Count > 0)
                    // {
                    //     DateTime EndDate = Convert.ToDateTime(MasterTable.Rows[0]["d_EndDate"]);


                    //     foreach (DataRow dRow in Ass_ItemMaster.Rows)
                    //     {
                    //         if (dRow["D_EndDate"].ToString() == "")
                    //         {
                    //             B_Depreciation = true;

                    //         }

                    //         else if (dRow["D_EndDate"].ToString() != EndDate.ToString())
                    //         {
                    //             B_Depreciation = true;
                    //         }
                    //     }
                    // }


                    // if (B_Depreciation)
                    // {
                    //     transaction.Rollback();
                    //     return Ok(_api.Error(User, "NeedDepreciation"));
                    // }







                    if (b_TransferBalance)
                    {
                        nFnYearID = myFunctions.getIntVAL((dLayer.ExecuteScalar("Select top 1 ISNULL(N_FnYearID,0) from Acc_FnYear Where N_CompanyID = " + nCompanyID + "   and D_Start > ( Select D_Start FRom Acc_FnYear Where N_FnYearID =" + n_FnYearId + " and N_CompanyID =" + nCompanyID + ") order by D_Start", Params, connection, transaction)).ToString());

                        SortedList PostingParam1 = new SortedList();
                        PostingParam1.Add("@N_CompanyID", nCompanyID);
                        PostingParam1.Add("@N_FnYearID_Close", n_FnYearId);
                        PostingParam1.Add("@N_FnYearID_New", nFnYearID);
                        PostingParam1.Add("@X_RtainedIncomeLedgerCode", accountCode);
                        PostingParam1.Add("@N_UserID", nUserID);
                        PostingParam1.Add("@X_Operation", "Transfer");




                        bool YearProcessed = Convert.ToBoolean(dLayer.ExecuteScalar("select B_YearEndProcess FRom Acc_FnYear Where N_FnYearID =  " + n_FnYearId + " and N_CompanyID =" + nCompanyID + "", Params, connection, transaction));
                        if (YearProcessed == false)
                        {

                            return Ok(_api.Warning("Year not closed"));

                        }
                        dLayer.ExecuteNonQueryPro("SP_Acc_CloseFinYear ", PostingParam1, connection, transaction);
                        // dLayer.ExecuteNonQueryPro("SP_Acc_CloseFinYear " + myCompanyID._CompanyID + "," + myCompanyID._FnYearID + "," + N_FnYearID.ToString() + ",'" + txtDefaultAccount.Text.Trim() + "'," + myCompanyID._UserID + ",'Transfer'", "TEXT", new DataTable());
                    }


                    transaction.Commit();
                    return Ok(_api.Success("saved Sucessfully"));

                }
            }

            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex.Message));
            }
        }















    }
}











