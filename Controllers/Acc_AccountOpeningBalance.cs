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
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("accountOpening")]
    [ApiController]
    public class Acc_AccountOpeningBalance : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly IApiFunctions _api;
        private readonly int FormID;

        public Acc_AccountOpeningBalance(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 151;
        }

        [HttpGet("costCenter")]
        public ActionResult GetCostCenterValues(int N_VoucherID, int N_FnYearID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SortedList Params = new SortedList()
                    {
                        {"AccountField",2},
                        {"N_VoucherID",N_VoucherID},
                        {"N_CompanyID",myFunctions.GetCompanyID(User)},
                        {"N_FnYearID",N_FnYearID},
                    };

                    dt = dLayer.ExecuteDataTablePro("SP_Acc_AccountBalance_Disp", Params, connection);
                    dt = _api.Format(dt, "costCenter");
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }


        [HttpGet("details")]
        public ActionResult GetAccountOpeningDetails(int nFnYearID, int nBranchID, int nLanguageID)
        {
            DataSet dsOpening = new DataSet();
            DataTable OpeningBalance = new DataTable();
            DataTable Master = new DataTable();
            DataTable DetailTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int AccountField = 0;
            int nVoucherID = 0;
            string sqlCommandText = "", sqlCommandText2 = "", sqlQry = "";


            if (nLanguageID == 2) AccountField = 1;

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nBranchID", nBranchID);

            sqlCommandText = "Select * from Acc_VoucherMaster Where N_CompanyID=@nCompanyID And N_FnYearID=@nFnYearID and X_TransType='OB' and N_BranchID=@nBranchID";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Master = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    Master = _api.Format(Master, "Master");

                    if (Master.Rows.Count > 0)
                    {
                        DataRow MasterRow = Master.Rows[0];
                        nVoucherID = myFunctions.getIntVAL(MasterRow["N_VoucherID"].ToString());
                    }

                    //Details
                    SortedList ParamsList = new SortedList()
                    {
                        {"AccountField",AccountField},
                        {"N_VoucherID",nVoucherID},
                        {"N_CompanyID",myFunctions.GetCompanyID(User)},
                        {"N_FnYearID",nFnYearID},
                    };

                    DetailTable = dLayer.ExecuteDataTablePro("SP_Acc_AccountBalance_Disp", ParamsList, connection);
                    DetailTable = _api.Format(DetailTable, "Details");

                    if (DetailTable.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }

                    myFunctions.AddNewColumnToDataTable(DetailTable, "N_Dimension", typeof(int), 0);

                    sqlCommandText2 = "select * from vw_VoucherDetails_Desc where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID";
                    DataTable VoucherDetails_DescTable = dLayer.ExecuteDataTable(sqlCommandText2, Params, connection);
                    if (VoucherDetails_DescTable.Rows.Count > 0)
                    {
                        for (int i = 1; i < DetailTable.Rows.Count; i++)
                        {
                            DataRow[] dr = VoucherDetails_DescTable.Select("N_VoucherID=" + nVoucherID + " and N_LedgerID=" + myFunctions.getIntVAL(DetailTable.Rows[i]["N_LedgerID"].ToString()) + "");
                            if (dr.Length > 0)
                                DetailTable.Rows[i]["N_Dimension"] = myFunctions.getIntVAL(dr.Length.ToString());
                        }
                    }
                    VoucherDetails_DescTable = _api.Format(VoucherDetails_DescTable, "VoucherDetails_Desc");
                    sqlQry = "Select X_VoucherNo, B_IsAccPosted from Acc_VoucherMaster Where X_TransType = 'OB' and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_BranchID =@nBranchID";
                    OpeningBalance = dLayer.ExecuteDataTable(sqlQry, Params, connection);
                    OpeningBalance = _api.Format(OpeningBalance, "OpeningBalance");
                    dsOpening.Tables.Add(Master);
                    dsOpening.Tables.Add(DetailTable);
                    dsOpening.Tables.Add(VoucherDetails_DescTable);
                    dsOpening.Tables.Add(OpeningBalance);
                    return Ok(_api.Success(dsOpening));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("settings")]
        public ActionResult CheckSettings()
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);

                    dt.Clear();
                    dt.Columns.Add("B_FinancialEntryOpen");

                    int N_FinancialEntryOpen = myFunctions.getIntVAL(myFunctions.ReturnSettings("Financial", "FinancialEntryOpen", "N_Value", nCompanyID, dLayer, connection));

                    DataRow row = dt.NewRow();
                    row["B_FinancialEntryOpen"] = myFunctions.getVAL(N_FinancialEntryOpen.ToString());

                    dt.Rows.Add(row);

                    dt = api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(dt));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }

        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    object Result = 0;
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    DataTable VoucherDetails_DescTable;
                    DataTable Details_SegmentsTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataTable dt = new DataTable();

                    // VoucherDetails_DescTable = ds.Tables["VoucherDetails_Desc"];
                    // Details_SegmentsTable = ds.Tables["Details_Segments"];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    int nVoucherID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_VoucherID"].ToString());
                    int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                    string InvoiceNo = MasterTable.Rows[0]["x_VoucherNo"].ToString();
                    bool b_OpeningBalancePosted = false;

                    string sqlQry = "Select X_VoucherNo, B_IsAccPosted from Acc_VoucherMaster Where X_TransType = 'OB' and N_CompanyID = " + nCompanyID + " and N_FnYearID = " + nFnYearID + " and N_BranchID = " + nBranchID + "";
                    dt = dLayer.ExecuteDataTable(sqlQry, Params, connection, transaction);
                    if (dt.Rows.Count > 0)
                    {
                        b_OpeningBalancePosted = myFunctions.getBoolVAL(dt.Rows[0]["b_IsAccPosted"].ToString());

                    }
                    if (b_OpeningBalancePosted)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Opening Balance Posted"));
                    }
                    if (nVoucherID != 0)
                    {

                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", 46);

                        while (true)
                        {
                            InvoiceNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Acc_VoucherMaster Where X_VoucherNo ='" + InvoiceNo + "' and N_CompanyID= " + nCompanyID + " and X_TransType ='JV' and N_FnYearID=" + nFnYearID, connection, transaction);
                            if  (N_Result == null)
                                break;
                        }
                        MasterTable.Rows[0]["x_VoucherNo"] = InvoiceNo.ToString();
                    } 
                    


                    nVoucherID = dLayer.SaveData("Acc_VoucherMaster", "N_VoucherID", MasterTable, connection, transaction);
                    if (nVoucherID <= 0)
                    {
                        transaction.Rollback();
                    }
                    if (!DetailTable.Columns.Contains("N_BranchID"))
                        DetailTable = myFunctions.AddNewColumnToDataTable(DetailTable, "N_BranchID", typeof(int), null);

                    if (!DetailTable.Columns.Contains("N_AmountF"))
                        DetailTable = myFunctions.AddNewColumnToDataTable(DetailTable, "N_AmountF", typeof(double), null);

                    DetailTable.AcceptChanges();
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        if( myFunctions.getIntVAL(DetailTable.Rows[j]["N_LedgerID"].ToString())>0)
                        {

                        }
                        else
                        {
                            string asscs="";
                        }

                        SortedList DeleteParams = new SortedList(){
                                    {"N_LedgerID",myFunctions.getIntVAL(DetailTable.Rows[j]["N_LedgerID"].ToString())},
                                    {"N_VoucherID",myFunctions.getIntVAL(MasterTable.Rows[0]["n_VoucherID"].ToString())},
                                    {"N_CompanyID",nCompanyID},
                                    {"N_AccTypeID",0},
                                    {"N_FnYearID",nFnYearID}};
                                

                        dLayer.ExecuteNonQueryPro("Delete_VoucherMaster_Details", DeleteParams, connection, transaction);

                        DetailTable.Rows[j]["N_VoucherID"] = nVoucherID;
                        DetailTable.Rows[j]["N_BranchID"] = nBranchID;
                        DetailTable.Rows[j]["N_AmountF"] = DetailTable.Rows[j]["N_Amount"];

                        double nAmount = 0;
                        if (myFunctions.getVAL(DetailTable.Rows[j]["n_Debit"].ToString()) > 0)
                            nAmount = myFunctions.getVAL(DetailTable.Rows[j]["n_Debit"].ToString());
                        else
                            nAmount = (-1) * myFunctions.getVAL(DetailTable.Rows[j]["n_Credit"].ToString());
                        DetailTable.Rows[j]["N_Amount"] = myFunctions.getVAL(nAmount.ToString());
                        if (nAmount == 0)
                        {
                            DetailTable.Rows[j].Delete();

                            j--;
                            DetailTable.AcceptChanges();
                        }
                        else
                            DetailTable.Rows[j]["N_Amount"] = nAmount;
                    }
                    DetailTable.AcceptChanges();
                    if (DetailTable.Columns.Contains("n_Debit"))
                        DetailTable.Columns.Remove("n_Debit");
                    if (DetailTable.Columns.Contains("n_Credit"))
                        DetailTable.Columns.Remove("n_Credit");

                   
                    // for (int j = 0; j < DetailTable.Rows.Count; j++)
                    // {
                    //     N_VoucherDetailsID = dLayer.SaveDataWithIndex("Acc_VoucherMaster_Details", "N_VoucherDetailsID","", "", j, DetailTable, connection, transaction);
                   if(DetailTable.Rows.Count>0)
                   {
                        int N_VoucherDetailsID = 0;
                    N_VoucherDetailsID = dLayer.SaveData("Acc_VoucherMaster_Details", "N_VoucherDetailsID", DetailTable, connection, transaction);
                   
                    if (N_VoucherDetailsID > 0)
                    {
                        //         for (int k = 0; k < Details_SegmentsTable.Rows.Count; k++)
                        //         {
                        //             if (myFunctions.getIntVAL(Details_SegmentsTable.Rows[k]["rowID"].ToString()) == j)
                        //             {
                        //                 Details_SegmentsTable.Rows[k]["N_VoucherID"] = nVoucherID;
                        //                 Details_SegmentsTable.Rows[k]["N_VoucherDetailsID"] = N_VoucherDetailsID;
                        //             }
                        //         }
                        // for (int k = 0; k < VoucherDetails_DescTable.Rows.Count; k++)
                        // {
                        //     if(DetailTable.Rows[j]["n_LedgerID"].ToString()!=VoucherDetails_DescTable.Rows[k]["n_LedgerID"].ToString()) continue;

                        //     VoucherDetails_DescTable.Rows[k]["N_VoucherID"] = nVoucherID;
                        //     VoucherDetails_DescTable.Rows[k]["N_VoucherDetailsID"] = N_VoucherDetailsID;
                        // }
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                   }
                    //}
                    // if (Details_SegmentsTable.Columns.Contains("rowID"))
                    //     Details_SegmentsTable.Columns.Remove("rowID");

                    // Details_SegmentsTable.AcceptChanges();
                    // int N_VoucherSegmentID = dLayer.SaveData("Acc_VoucherMaster_Details_Segments", "N_VoucherSegmentID", Details_SegmentsTable, connection, transaction);
                    // if(VoucherDetails_DescTable.Rows.Count>0)
                    // {
                    //     int N_VoucherSegmentID = dLayer.SaveData("Acc_VoucherDetails_Desc", "N_VoucherDetailsDescID", VoucherDetails_DescTable, connection, transaction);
                    // }

                    try
                    {
                        SortedList DeleteVoucherParams = new SortedList(){
                                        {"N_VoucherID",nVoucherID},
                                        {"N_CompanyID",nCompanyID},
                                        {"N_FnYearID",nFnYearID},
                                        {"N_UserID",myFunctions.GetUserID(User)},
                                        {"X_SystemName",System.Environment.MachineName}};

                        dLayer.ExecuteNonQueryPro("SP_Delete_VoucherDetails", DeleteVoucherParams, connection, transaction);

                        SortedList PostParams = new SortedList(){
                                        {"N_CompanyID",nCompanyID},
                                        {"X_InventoryMode","OB"},
                                        {"N_InternalID",nVoucherID},
                                        {"N_UserID",myFunctions.GetUserID(User)},
                                        {"X_SystemName",System.Environment.MachineName}};

                        dLayer.ExecuteNonQueryPro("SP_Acc_InventoryPosting", PostParams, connection, transaction);

                        dLayer.ExecuteNonQuery("update Acc_VoucherMaster set B_IsAccPosted=1 where N_VoucherID=" + nVoucherID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, Params, connection, transaction);
                        dLayer.ExecuteNonQuery("update Acc_FnYear set B_AlterBalance=1 where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, Params, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        if (ex.Message == "50")
                            return Ok(_api.Error(User, "Day Closed"));
                        else if (ex.Message == "51")
                            return Ok(_api.Error(User, "Year Closed"));
                        else if (ex.Message == "52")
                            return Ok(_api.Error(User, "Year Exists"));
                        else if (ex.Message == "53")
                            return Ok(_api.Error(User, "Period Closed"));
                        else if (ex.Message == "54")
                            return Ok(_api.Error(User, "Txn Date"));
                        else if (ex.Message == "55")
                            return Ok(_api.Error(User, "Quantity exceeds!"));
                        else
                            return Ok(_api.Error(User, ex));
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Account Opening Balance Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("canclePost")]
        public ActionResult CanclePosting(int nVoucherID, int nFnYearID)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    SortedList DeleteVoucherParams = new SortedList(){
                                    {"N_VoucherID",nVoucherID},
                                    {"N_CompanyID",nCompanyID},
                                    {"N_FnYearID",nFnYearID},
                                    {"N_UserID",myFunctions.GetUserID(User)},
                                    {"X_SystemName",System.Environment.MachineName}};

                    dLayer.ExecuteNonQueryPro("SP_Delete_VoucherDetails", DeleteVoucherParams, connection);

                    dLayer.ExecuteNonQuery(" update Acc_VoucherMaster set B_IsAccPosted = 0 where N_VoucherID =" + nVoucherID + " and N_CompanyID =" + nCompanyID + " and X_TransType ='OB' and N_FnYearID=" + nFnYearID, Params, connection);

                    return Ok(_api.Success("Posting Cancelled"));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }

        }

        [HttpGet("fnYearData")]
        public ActionResult GetFnYearData()
        {

            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();

            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);

            string qry = "";
            qry = "select Top(1) * from Acc_FnYear where N_CompanyID=@nCompanyID and ISNULL(B_PreliminaryYear,0)=0 order by D_Start";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(qry, Params, connection);
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


    }
}




