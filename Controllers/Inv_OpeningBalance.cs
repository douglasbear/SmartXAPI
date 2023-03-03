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
    [Route("invOpeningBalance")]
    [ApiController]
    public class inv_OpeningBalance : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public inv_OpeningBalance(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 208;
        }

        [HttpGet("partyList")]
        public ActionResult getPartyList(int nFlag, int nFnYearID, int nBranchID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable partylist = new DataTable();
                    DataTable settings = new DataTable();
                    DataTable details = new DataTable();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    string N_TransID = "";
                    string N_PayReceiptDetailsID = "";
                    object nBalanceAmount = "";
                    object netAmount = "";
                    object adjustedCount = "";

                    SortedList ProParams = new SortedList();
                    ProParams.Add("N_CompanyID", nCompanyID);
                    ProParams.Add("N_Flag", nFlag);
                    ProParams.Add("N_FnYearID", nFnYearID);
                    ProParams.Add("N_BranchID", nBranchID);

                    SqlTransaction transaction = connection.BeginTransaction();
                    partylist = dLayer.ExecuteDataTablePro("Sp_Inv_OpeningBalance_CloudView", ProParams, connection, transaction);
                    details = dLayer.ExecuteDataTablePro("Sp_Inv_OpeningBalance_CloudDispAll", ProParams, connection, transaction);

                    settings.Clear();
                    settings.Columns.Add("N_CompanyID");
                    settings.Columns.Add("B_FinancialEntryOpen");
                    settings.Columns.Add("B_CustomerPO");
                    settings.Columns.Add("B_VendorPO");

                    DataRow row = settings.NewRow();
                    row["N_CompanyID"] = myFunctions.GetCompanyID(User);
                    row["B_FinancialEntryOpen"] = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Financial", "FinancialEntryOpen", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection, transaction)));
                    row["B_CustomerPO"] = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("64", "EnableCustomerPO", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection, transaction)));
                    row["B_VendorPO"] = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("65", "EnableVendorPO", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection, transaction)));

                    partylist = _api.Format(partylist);
                    details = _api.Format(details);


                    details = myFunctions.AddNewColumnToDataTable(details, "customerFlag", typeof(bool), false);
                    details = myFunctions.AddNewColumnToDataTable(details, "advAdjustmentFlag", typeof(bool), false);

                    if (nFlag == 0)
                    {
                        foreach (DataRow dtRow in details.Rows)
                        {
                            bool custFlag = false;

                            N_TransID = dtRow["N_TransID"].ToString();
                            N_PayReceiptDetailsID = dtRow["N_PayReceiptDetailsID"].ToString();
                            if (N_TransID != "" && N_TransID != null)
                            {
                                nBalanceAmount = dLayer.ExecuteScalar("select N_BalanceAmount from vw_invReceivables where N_CompanyID=" + nCompanyID + " and N_SalesId = " + N_TransID, connection, transaction);
                                if (nBalanceAmount == null) { nBalanceAmount = 0; }

                                netAmount = dLayer.ExecuteScalar("select NetAmount from vw_invReceivables where N_CompanyID=" + nCompanyID + " and N_SalesId = " + N_TransID, connection, transaction);
                                if (netAmount == null) { netAmount = 0; }


                                if(N_PayReceiptDetailsID!= "" && N_PayReceiptDetailsID != null)
                                {
                                adjustedCount = dLayer.ExecuteScalar("select count(1) from Inv_PayReceiptDetails where N_CompanyID=" + nCompanyID + " and X_TransType='SA' and N_InventoryId = " + N_TransID, connection, transaction);
                                if (adjustedCount == null) { adjustedCount = 0; }
                                }


                            }

                            if (myFunctions.getVAL(nBalanceAmount.ToString()) < myFunctions.getVAL(netAmount.ToString()) && myFunctions.getIntVAL(N_PayReceiptDetailsID.ToString())<=0)
                            {
                                dtRow["customerFlag"] = true;
                            }

                            if (myFunctions.getVAL(adjustedCount.ToString()) >1)
                            {
                                dtRow["advAdjustmentFlag"] = true;
                            }


                        }
                  


                    }

                    else
                    {
                        foreach (DataRow dtRow in details.Rows)
                        {
                            bool custFlag = false;

                            N_TransID = dtRow["N_TransID"].ToString();
                            N_PayReceiptDetailsID = dtRow["N_PayReceiptDetailsID"].ToString();
                            if (N_TransID != "")
                            {
                                nBalanceAmount = dLayer.ExecuteScalar("select N_BalanceAmount from vw_InvPayables where N_CompanyID=" + nCompanyID + " and N_PurchaseID = " + N_TransID, connection, transaction);
                                netAmount = dLayer.ExecuteScalar("select NetAmount from vw_InvPayables where N_CompanyID=" + nCompanyID + " and N_PurchaseID = " + N_TransID, connection, transaction);
                                if(N_PayReceiptDetailsID!= "" && N_PayReceiptDetailsID != null)
                                {
                                adjustedCount = dLayer.ExecuteScalar("select count(1) from Inv_PayReceiptDetails where N_CompanyID=" + nCompanyID + " and X_TransType='PA' and N_InventoryId = " + N_TransID, connection, transaction);
                                if (adjustedCount == null) { adjustedCount = 0; }
                                }
                            }
                            if(nBalanceAmount==null){nBalanceAmount="";}if(netAmount==null){netAmount="";}
                            if (myFunctions.getVAL(nBalanceAmount.ToString()) < myFunctions.getVAL(netAmount.ToString()) && myFunctions.getIntVAL(N_PayReceiptDetailsID.ToString())<=0)
                            {
                                dtRow["customerFlag"] = true;
                            }
                               if (myFunctions.getVAL(adjustedCount.ToString()) >1)
                            {
                                dtRow["advAdjustmentFlag"] = true;
                            }
                        }
                    }



                    if (partylist.Rows.Count == 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        SortedList Output = new SortedList();
                        Output.Add("partylist", partylist);
                        // Output.Add("settings",settings);
                        Output.Add("details", details);
                        transaction.Commit();
                        return Ok(_api.Success(Output));
                    }
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
                DataTable SaveCustTable;
                DataTable PartyListTable;
                DataTable SaveVendorTable;
                DataTable SaveVendorPaymentMasterTable;
                DataTable SaveVendorPaymentDetailsTable;
                DataTable SaveCustomerPaymentMasterTable;
                DataTable SaveCustomerPaymentDetailsTable;
                DataTable DeleteData;
                SaveCustTable = ds.Tables["custdetails"];
                SaveVendorTable = ds.Tables["vendorDetails"];
                SaveVendorPaymentMasterTable = ds.Tables["vendorPaymentMaster"];
                SaveVendorPaymentDetailsTable = ds.Tables["vendorPaymentDetails"];
                SaveCustomerPaymentMasterTable = ds.Tables["customerPaymentMaster"];
                SaveCustomerPaymentDetailsTable = ds.Tables["customerPaymentDetails"];
                PartyListTable = ds.Tables["partylist"];
                DeleteData = ds.Tables["DeleteData"];
                int nCompanyID = myFunctions.GetCompanyID(User);
                int nFnYearID = myFunctions.getIntVAL(PartyListTable.Rows[0]["n_FnYearID"].ToString());
                int nUserID = myFunctions.GetUserID(User);
                int nBranchID = myFunctions.getIntVAL(PartyListTable.Rows[0]["n_BranchID"].ToString());
                int nFlag = myFunctions.getIntVAL(PartyListTable.Rows[0]["nFlag"].ToString());
                string xTransType = PartyListTable.Rows[0]["x_TransType"].ToString();
                object Count = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();





                    if (nFlag == 0)
                    {

                         if(SaveCustomerPaymentMasterTable.Rows.Count>0)
                        {

                        for (int j = 0; j < SaveCustomerPaymentMasterTable.Rows.Count; j++)
                        {

                        int nReceiptID = dLayer.SaveDataWithIndex("Inv_PayReceipt", "N_PayReceiptId", "", "", j, SaveCustomerPaymentMasterTable, connection, transaction);
                        if (nReceiptID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to save"));
                        }

                        if (nReceiptID > 0)
                        {
                            for (int k = 0; k < SaveCustomerPaymentDetailsTable.Rows.Count; k++)
                            {
                                if (SaveCustomerPaymentDetailsTable.Rows[k]["x_VoucherNo"].ToString() == SaveCustomerPaymentMasterTable.Rows[j]["x_VoucherNo"].ToString())
                                { 
                                    SaveCustomerPaymentDetailsTable.Rows[k]["N_PayReceiptId"] = nReceiptID;
                                    SaveCustomerPaymentDetailsTable.Rows[k]["n_InventoryID"] = nReceiptID;
                                                                     
                                    // SaveCustomerPaymentDetailsTable.Rows[k]["n_Amount"] = -1 * myFunctions.getVAL(SaveCustomerPaymentDetailsTable.Rows[k]["n_Amount"].ToString());                                 
                                    // SaveCustomerPaymentDetailsTable.Rows[k]["n_AmountF"] = -1 * myFunctions.getVAL(SaveCustomerPaymentDetailsTable.Rows[k]["n_AmountF"].ToString());                                        
                                }
                            }
                         }
                        }
                        SaveCustomerPaymentDetailsTable.Columns.Remove("x_VoucherNo");
                        int nReceiptDetailsID = dLayer.SaveData("Inv_PayReceiptDetails", "N_PayReceiptDetailsId", SaveCustomerPaymentDetailsTable, connection, transaction);
                        if (nReceiptDetailsID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to save"));
                        }
                        }

                    if (DeleteData.Rows.Count > 0)
                    {
                    
                      int N_VoucherID=0;
                       foreach (DataRow deleteItem in DeleteData.Rows)
                      {
                        if(myFunctions.getIntVAL(deleteItem["n_PayReceiptDetailsID"].ToString())>0)
                        {

                         N_VoucherID =myFunctions.getIntVAL(dLayer.ExecuteScalar("select  N_VoucherID from Acc_VoucherMaster where X_TransType = 'OB' and N_CompanyID = "+nCompanyID+" and N_FnYearID = "+nFnYearID+" and N_BranchID = "+nBranchID+"", connection, transaction).ToString());
                         dLayer.ExecuteNonQuery("delete from  Inv_PayReceiptDetails  where N_CompanyID=" + nCompanyID + " and N_PayReceiptID=" + myFunctions.getIntVAL(deleteItem["n_TransID"].ToString()) + " and N_PayReceiptDetailsID=" + myFunctions.getIntVAL(deleteItem["N_PayReceiptDetailsID"].ToString()) + "", connection, transaction);
                         dLayer.ExecuteNonQuery("delete from  Inv_PayReceipt  where N_CompanyID=" + nCompanyID + " and N_PayReceiptID=" + myFunctions.getIntVAL(deleteItem["n_TransID"].ToString()) + "", connection, transaction);
                         dLayer.ExecuteNonQuery("DELETE FROM acc_vouchermaster_details  WHERE  n_companyid = "+nCompanyID+"   AND n_voucherid = "+N_VoucherID+" AND  n_branchid = "+nBranchID+"   AND n_acctype = 2 and N_InventoryID="+myFunctions.getIntVAL(deleteItem["n_TransID"].ToString())+"",connection, transaction);
                        }
                        else if (DeleteData.Columns.Contains("N_SalesID"))
                        {
                         N_VoucherID =myFunctions.getIntVAL(dLayer.ExecuteScalar("select  N_VoucherID from Acc_VoucherMaster where X_TransType = 'OB' and N_CompanyID = "+nCompanyID+" and N_FnYearID = "+nFnYearID+" and N_BranchID = "+nBranchID+"", connection, transaction).ToString());
                         dLayer.ExecuteNonQuery("delete from  Inv_Sales  where N_CompanyID=" + nCompanyID + " and N_SalesID=" + myFunctions.getIntVAL(deleteItem["n_SalesId"].ToString()) + "", connection, transaction);
                         dLayer.ExecuteNonQuery("DELETE FROM acc_vouchermaster_details  WHERE  n_companyid = "+nCompanyID+"   AND n_voucherid = "+N_VoucherID+" AND  n_branchid = "+nBranchID+"   AND n_acctype = 2 and N_InventoryID="+myFunctions.getIntVAL(deleteItem["n_SalesId"].ToString())+"",connection, transaction);
	
                    
                        }
                      }
                    }

                        int nSalesID = dLayer.SaveData("Inv_Sales", "N_SalesID", SaveCustTable, connection, transaction);

                        if (nSalesID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to save"));
                        }

                    }
                    else
                    {

                        if(SaveVendorPaymentMasterTable.Rows.Count>0)
                        {

                        for (int j = 0; j < SaveVendorPaymentMasterTable.Rows.Count; j++)
                        {

                        int nReceiptID = dLayer.SaveDataWithIndex("Inv_PayReceipt", "N_PayReceiptId", "", "", j, SaveVendorPaymentMasterTable, connection, transaction);
                        if (nReceiptID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to save"));
                        }

                        if (nReceiptID > 0)
                        {
                            for (int k = 0; k < SaveVendorPaymentDetailsTable.Rows.Count; k++)
                            {
                                if (SaveVendorPaymentDetailsTable.Rows[k]["x_VoucherNo"].ToString() == SaveVendorPaymentMasterTable.Rows[j]["x_VoucherNo"].ToString())
                                { 
                                    SaveVendorPaymentDetailsTable.Rows[k]["N_PayReceiptId"] = nReceiptID;
                                    SaveVendorPaymentDetailsTable.Rows[k]["n_InventoryID"] = nReceiptID;
                                                                     
                                    // SaveVendorPaymentDetailsTable.Rows[k]["n_Amount"] = -1 * myFunctions.getVAL(SaveVendorPaymentDetailsTable.Rows[k]["n_Amount"].ToString());                                 
                                    // SaveVendorPaymentDetailsTable.Rows[k]["n_AmountF"] = -1 * myFunctions.getVAL(SaveVendorPaymentDetailsTable.Rows[k]["n_AmountF"].ToString());                                        
                                }
                            }
                         }
                        }
                        SaveVendorPaymentDetailsTable.Columns.Remove("x_VoucherNo");
                        int nReceiptDetailsID = dLayer.SaveData("Inv_PayReceiptDetails", "N_PayReceiptDetailsId", SaveVendorPaymentDetailsTable, connection, transaction);
                        if (nReceiptDetailsID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to save"));
                        }

                         }
                          if (DeleteData.Rows.Count > 0)
                    {
               
                     int N_VoucherID=0;  
                      foreach (DataRow deleteItem in DeleteData.Rows)
                    {
                        if(myFunctions.getIntVAL(deleteItem["n_PayReceiptDetailsID"].ToString())>0)
                        {

                         N_VoucherID =myFunctions.getIntVAL(dLayer.ExecuteScalar("select  N_VoucherID from Acc_VoucherMaster where X_TransType = 'OB' and N_CompanyID = "+nCompanyID+" and N_FnYearID = "+nFnYearID+" and N_BranchID = "+nBranchID+"", connection, transaction).ToString());
                         dLayer.ExecuteNonQuery("delete from  Inv_PayReceiptDetails  where N_CompanyID=" + nCompanyID + " and N_PayReceiptID=" + myFunctions.getIntVAL(deleteItem["n_TransID"].ToString()) + " and N_PayReceiptDetailsID=" + myFunctions.getIntVAL(deleteItem["N_PayReceiptDetailsID"].ToString()) + "", connection, transaction);
                         dLayer.ExecuteNonQuery("delete from  Inv_PayReceipt  where N_CompanyID=" + nCompanyID + " and N_PayReceiptID=" + myFunctions.getIntVAL(deleteItem["n_TransID"].ToString()) + " ", connection, transaction);
                         dLayer.ExecuteNonQuery("DELETE FROM acc_vouchermaster_details  WHERE  n_companyid = "+nCompanyID+"   AND n_voucherid = "+N_VoucherID+" AND  n_branchid = "+nBranchID+"   AND n_acctype = 2 and N_InventoryID="+myFunctions.getIntVAL(deleteItem["n_TransID"].ToString())+"",connection, transaction);
                        }
                        else if (DeleteData.Columns.Contains("N_PurchaseID"))
                        {
                            N_VoucherID =myFunctions.getIntVAL(dLayer.ExecuteScalar("select  N_VoucherID from Acc_VoucherMaster where X_TransType = 'OB' and N_CompanyID = "+nCompanyID+" and N_FnYearID = "+nFnYearID+" and N_BranchID = "+nBranchID+"", connection, transaction).ToString());
                            dLayer.ExecuteNonQuery("delete from  Inv_Purchase  where N_CompanyID=" + nCompanyID + " and N_PurchaseID=" + myFunctions.getIntVAL(deleteItem["N_PurchaseID"].ToString()) + "", connection, transaction);
                             dLayer.ExecuteNonQuery("DELETE FROM acc_vouchermaster_details  WHERE  n_companyid = "+nCompanyID+"   AND n_voucherid = "+N_VoucherID+" AND  n_branchid = "+nBranchID+"   AND n_acctype = 2 and N_InventoryID="+myFunctions.getIntVAL(deleteItem["N_PurchaseID"].ToString())+"",connection, transaction);
                        }
                       
                    }
                   }
                        int npurchaseID = dLayer.SaveData("Inv_Purchase", "N_PurchaseID", SaveVendorTable, connection, transaction);


                        if (npurchaseID <= 0)
                        { 
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to save"));
                        }

                    }



                    for (int j = 0; j < PartyListTable.Rows.Count; j++)
                    {
                        int nPartyID = myFunctions.getIntVAL(PartyListTable.Rows[j]["n_PartyID"].ToString());

                        SortedList ProcParam = new SortedList();
                        ProcParam.Add("N_CompanyID", nCompanyID);
                        ProcParam.Add("N_FnYearID", nFnYearID);
                        ProcParam.Add("Mode", xTransType);
                        ProcParam.Add("N_UserID", nUserID);
                        ProcParam.Add("N_PartyID", nPartyID);
                        ProcParam.Add("N_BranchID", nBranchID);
                        ProcParam.Add("X_EntryFrom", "cob");
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Acc_BeginingBalancePosting_Ins", ProcParam, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }
                    }

                    transaction.Commit();
                    return Ok(_api.Success("Opening Balance Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
    }
}
