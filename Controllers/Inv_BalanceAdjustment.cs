using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("BalanceAdjustment")]
    [ApiController]

 

    public class Inv_BalanceAdjustment : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Inv_BalanceAdjustment(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        //List
        [HttpGet("list")]
        public ActionResult GetBalanceDetails(int nFnyearID, int nPartyType, int nPartyID, int N_TransType, int nPage, int nSizeperpage,string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string Searchkey = "";
            if (nPartyType ==1){
                if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([Invoice No] like '%" + xSearchkey + "%' or [Customer Name] like '%"+ xSearchkey + "%' or cast([Adjustment Date] as VarChar) like '%" + xSearchkey + "%' or X_Notes like '%"+ xSearchkey + "%' or x_VendorName like '%"+ xSearchkey + "%' or [Net Amount] like '%"+ xSearchkey + "%')";
            }
            else {
                if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([Invoice No] like '%" + xSearchkey + "%' or X_VendorName like '%"+ xSearchkey + "%' or cast([Adjustment Date] as VarChar) like '%" + xSearchkey + "%' or X_Notes like '%"+ xSearchkey + "%' or Netamt like '%"+ xSearchkey + "%')";
            }
            

            if (xSortBy == null || xSortBy.Trim() == ""){
                xSortBy = " order by [Invoice No] desc";
            }
            else
            {
             switch (xSortBy.Split(" ")[0]){
                  
                   case "customerName" : xSortBy =" [Customer Name] " + xSortBy.Split(" ")[1] ;
                   break;
                   case "invoiceNo" : xSortBy =" [Invoice No] " + xSortBy.Split(" ")[1] ;
                   break;
                   case "adjustmentDate":
                   xSortBy = "Cast([Adjustment Date] as DateTime )" + xSortBy.Split(" ")[1];
                    break;
                   case "netAmount":
                    xSortBy = "Cast(REPLACE([Net Amount],',','') as Numeric(10,2))" + xSortBy.Split(" ")[1];
                    break; 
                   
                   default : break;
               }
             xSortBy = " order by " + xSortBy;
            }

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            if (Count == 0)
            {
                if (nPartyType ==1)
                    sqlCommandText = "select top(" + nSizeperpage + ") [Adjustment Date],[Invoice No],[Customer Name],[Net Amount],X_Notes from vw_CustomerBalanceAdjustment where N_CompanyID=@p1  and N_TransType=@p4 and B_YearEndProcess=0 and N_PartyType=@p5 " + Searchkey + " " + xSortBy;
                else
                    sqlCommandText = "select top(" + nSizeperpage + ") [Adjustment Date],[Invoice No],X_VendorName,Netamt as netAmount,X_Notes from vw_VendorBalanceAdjustment where N_CompanyID=@p1 and N_TransType=@p4 and B_YearEndProcess=0 and N_PartyType=@p5 " + Searchkey + " " + xSortBy;
            }
            else
            {
                if (nPartyType ==1)
                    sqlCommandText = "select top(" + nSizeperpage + ") [Adjustment Date],[Invoice No],[Customer Name],[Net Amount],X_Notes from vw_CustomerBalanceAdjustment where N_CompanyID=@p1 " + Searchkey + " and N_TransType=@p4 and B_YearEndProcess=0 and N_PartyType=@p5 and [Invoice No] not in (select top(" + Count + ") [Invoice No] from vw_CustomerBalanceAdjustment where N_CompanyID=@p1 and N_TransType=@p4 and B_YearEndProcess=0 and N_PartyType=@p5 " + xSortBy + " ) " + xSortBy;
                else
                    sqlCommandText = "select top(" + nSizeperpage + ") [Adjustment Date],[Invoice No],X_VendorName,Netamt as netAmount,X_Notes from vw_VendorBalanceAdjustment where N_CompanyID=@p1 " + Searchkey + " and N_TransType=@p4 and B_YearEndProcess=0 and N_PartyType=@p5 and [Invoice No] not in (select top(" + Count + ") [Invoice No] from vw_VendorBalanceAdjustment where N_CompanyID=@p1 and N_TransType=@p4 and B_YearEndProcess=0 and N_PartyType=@p5 " + xSortBy + " ) " + xSortBy;
            }
            Params.Add("@p1", nCompanyID);
            Params.Add("@p4", N_TransType);
            Params.Add("@p5", nPartyType);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    if (nPartyType ==1)
                        sqlCommandCount = "select count(*) as N_Count,sum(Cast(REPLACE([Net Amount],',','') as Numeric(10,2)) ) as TotalAmount from vw_CustomerBalanceAdjustment where N_CompanyID=@p1 and N_TransType=@p4 and B_YearEndProcess=0 and N_PartyType=@p5 " + Searchkey + "";
                    else
                        sqlCommandCount = "select count(*) as N_Count,sum(Cast(REPLACE(Netamt,',','') as Numeric(10,2)) ) as TotalAmount from vw_VendorBalanceAdjustment where N_CompanyID=@p1 and N_TransType=@p4 and B_YearEndProcess=0 and N_PartyType=@p5 " + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount="0";
                    string TotalSum="0";
                    if(Summary.Rows.Count>0){
                    DataRow drow = Summary.Rows[0];
                    TotalCount = drow["N_Count"].ToString();
                    TotalSum = drow["TotalAmount"].ToString();
                    }
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    OutPut.Add("TotalSum", TotalSum);
                }
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
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        [HttpGet("listDetails")]
        public ActionResult GetBalanceListDetails(int N_PartyType, string N_TransType, int nFnYearId, string X_ReceiptNo, bool bAllBranchData, int nBranchID)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            DataTable DataTable = new DataTable();

            string Mastersql = "";

            if (bAllBranchData == true)
            {
                if (N_PartyType == 1)
                    Mastersql = "Select Inv_Customer.*,Inv_BalanceAdjustmentMaster.* from Inv_BalanceAdjustmentMaster Inner Join  Inv_Customer ON Inv_BalanceAdjustmentMaster.N_PartyID = Inv_Customer.N_CustomerID And Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_Customer.N_CompanyID and Inv_BalanceAdjustmentMaster.N_FnYearID=Inv_Customer.N_FnYearID Where  Inv_BalanceAdjustmentMaster.N_CompanyID=@p1 and Inv_BalanceAdjustmentMaster.N_TransType =@p2 and Inv_BalanceAdjustmentMaster.N_FnYearID=@p3 and Inv_BalanceAdjustmentMaster.X_VoucherNo=@p4 and Inv_BalanceAdjustmentMaster.N_PartyType=@p5";
                else if (N_PartyType == 0)
                    Mastersql = "SELECT     Inv_BalanceAdjustmentMaster.N_CompanyID, Inv_BalanceAdjustmentMaster.N_FnYearID, Inv_BalanceAdjustmentMaster.X_VoucherNo, Inv_BalanceAdjustmentMaster.D_AdjustmentDate, Inv_BalanceAdjustmentMaster.D_EntryDate, Inv_BalanceAdjustmentMaster.N_Amount, Inv_BalanceAdjustmentMaster.N_UserID, Inv_BalanceAdjustmentMaster.N_BranchID, Inv_BalanceAdjustmentMaster.N_TransType, Inv_BalanceAdjustmentMaster.X_notes, Inv_BalanceAdjustmentMaster.N_PartyType, Inv_BalanceAdjustmentMaster.N_PartyID, Inv_BalanceAdjustmentMaster.N_AdjustmentId, Inv_BalanceAdjustmentMaster.N_AmountF, Inv_BalanceAdjustmentMaster.N_CurExchRate, Inv_BalanceAdjustmentMaster.N_WOID, Inv_Vendor.N_CompanyID AS Expr1, Inv_Vendor.N_VendorID, Inv_Vendor.X_VendorCode, Inv_Vendor.X_VendorName, Inv_Vendor.X_ContactName, Inv_Vendor.X_Address, Inv_Vendor.X_ZipCode, Inv_Vendor.X_PhoneNo1, Inv_Vendor.X_PhoneNo2, Inv_Vendor.X_FaxNo, Inv_Vendor.X_Email, Inv_Vendor.X_WebSite, Inv_Vendor.N_CreditLimit, Inv_Vendor.B_Inactive, Inv_Vendor.N_LedgerID, Inv_Vendor.N_InvDueDays, Inv_Vendor.N_FnYearID AS Expr2, Inv_Vendor.D_Entrydate AS Expr3, Inv_Vendor.N_CountryID, Inv_Vendor.N_TypeID, Inv_Vendor.B_DirPosting, Inv_Vendor.N_CurrencyID, Inv_Vendor.X_ReminderMsg, Inv_Vendor.X_VendorName_Ar, Inv_Vendor.N_CountryID, Inv_Vendor.X_TaxRegistrationNo, Inv_Vendor.N_TaxCategoryID, Inv_Vendor.B_AllowCashPay, Inv_Vendor.N_PartnerTypeID, Inv_Vendor.N_VendorTypeID, Inv_Vendor.N_GoodsDeliveryIn, Inv_Vendor.X_TandC, Acc_CurrencyMaster.N_CompanyID AS Expr4, Acc_CurrencyMaster.N_CurrencyID AS Expr5, Acc_CurrencyMaster.X_CurrencyCode, Acc_CurrencyMaster.X_CurrencyName, Acc_CurrencyMaster.X_ShortName, Acc_CurrencyMaster.N_ExchangeRate, Acc_CurrencyMaster.B_default, Acc_CurrencyMaster.N_Decimal, Prj_WorkOrder.N_WorkOrderId, Prj_WorkOrder.X_WorkOrderNo FROM         Inv_BalanceAdjustmentMaster INNER JOIN Inv_Vendor ON Inv_BalanceAdjustmentMaster.N_PartyID = Inv_Vendor.N_VendorID AND Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_Vendor.N_CompanyID AND  Inv_BalanceAdjustmentMaster.N_FnYearID = Inv_Vendor.N_FnYearID INNER JOIN Acc_CurrencyMaster ON Inv_Vendor.N_CurrencyID = Acc_CurrencyMaster.N_CurrencyID LEFT OUTER JOIN Prj_WorkOrder ON Inv_BalanceAdjustmentMaster.N_CompanyID = Prj_WorkOrder.N_CompanyId AND Inv_BalanceAdjustmentMaster.N_WOID = Prj_WorkOrder.N_WorkOrderId Where  Inv_BalanceAdjustmentMaster.N_CompanyID=@p1 and Inv_BalanceAdjustmentMaster.N_FnYearID=@p3 and Inv_BalanceAdjustmentMaster.N_TransType=@p2 and Inv_BalanceAdjustmentMaster.X_VoucherNo=@p4 and Inv_BalanceAdjustmentMaster.N_PartyType=@p5";

            }
            else
            {
                if (N_PartyType == 1)
                    Mastersql = "Select Inv_Customer.*,Inv_BalanceAdjustmentMaster.* from Inv_BalanceAdjustmentMaster Inner Join  Inv_Customer ON Inv_BalanceAdjustmentMaster.N_PartyID = Inv_Customer.N_CustomerID And Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_Customer.N_CompanyID and Inv_BalanceAdjustmentMaster.N_FnYearID=Inv_Customer.N_FnYearID Where  Inv_BalanceAdjustmentMaster.N_CompanyID=@p1 and Inv_BalanceAdjustmentMaster.N_TransType =@p2 and Inv_BalanceAdjustmentMaster.N_FnYearID=@p3 and Inv_BalanceAdjustmentMaster.X_VoucherNo=@p4 and Inv_BalanceAdjustmentMaster.N_PartyType=@p5 and Inv_BalanceAdjustmentMaster.N_BranchID=@p6";
                else if (N_PartyType == 0)
                    Mastersql = "SELECT     Inv_BalanceAdjustmentMaster.N_CompanyID, Inv_BalanceAdjustmentMaster.N_FnYearID, Inv_BalanceAdjustmentMaster.X_VoucherNo, Inv_BalanceAdjustmentMaster.D_AdjustmentDate, Inv_BalanceAdjustmentMaster.D_EntryDate, Inv_BalanceAdjustmentMaster.N_Amount, Inv_BalanceAdjustmentMaster.N_UserID, Inv_BalanceAdjustmentMaster.N_BranchID, Inv_BalanceAdjustmentMaster.N_TransType, Inv_BalanceAdjustmentMaster.X_notes, Inv_BalanceAdjustmentMaster.N_PartyType, Inv_BalanceAdjustmentMaster.N_PartyID, Inv_BalanceAdjustmentMaster.N_AdjustmentId, Inv_BalanceAdjustmentMaster.N_AmountF, Inv_BalanceAdjustmentMaster.N_CurExchRate, Inv_BalanceAdjustmentMaster.N_WOID, Inv_Vendor.N_CompanyID AS Expr1, Inv_Vendor.N_VendorID, Inv_Vendor.X_VendorCode, Inv_Vendor.X_VendorName, Inv_Vendor.X_ContactName, Inv_Vendor.X_Address, Inv_Vendor.X_ZipCode, Inv_Vendor.X_PhoneNo1, Inv_Vendor.X_PhoneNo2, Inv_Vendor.X_FaxNo, Inv_Vendor.X_Email, Inv_Vendor.X_WebSite, Inv_Vendor.N_CreditLimit, Inv_Vendor.B_Inactive, Inv_Vendor.N_LedgerID, Inv_Vendor.N_InvDueDays, Inv_Vendor.N_FnYearID AS Expr2, Inv_Vendor.D_Entrydate AS Expr3, Inv_Vendor.N_CountryID, Inv_Vendor.N_TypeID, Inv_Vendor.B_DirPosting, Inv_Vendor.N_CurrencyID, Inv_Vendor.X_ReminderMsg, Inv_Vendor.X_VendorName_Ar, Inv_Vendor.N_CountryID, Inv_Vendor.X_TaxRegistrationNo, Inv_Vendor.N_TaxCategoryID, Inv_Vendor.B_AllowCashPay, Inv_Vendor.N_PartnerTypeID, Inv_Vendor.N_VendorTypeID, Inv_Vendor.N_GoodsDeliveryIn, Inv_Vendor.X_TandC, Acc_CurrencyMaster.N_CompanyID AS Expr4, Acc_CurrencyMaster.N_CurrencyID AS Expr5, Acc_CurrencyMaster.X_CurrencyCode, Acc_CurrencyMaster.X_CurrencyName, Acc_CurrencyMaster.X_ShortName, Acc_CurrencyMaster.N_ExchangeRate, Acc_CurrencyMaster.B_default, Acc_CurrencyMaster.N_Decimal, Prj_WorkOrder.N_WorkOrderId, Prj_WorkOrder.X_WorkOrderNo FROM         Inv_BalanceAdjustmentMaster INNER JOIN Inv_Vendor ON Inv_BalanceAdjustmentMaster.N_PartyID = Inv_Vendor.N_VendorID AND Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_Vendor.N_CompanyID AND  Inv_BalanceAdjustmentMaster.N_FnYearID = Inv_Vendor.N_FnYearID INNER JOIN Acc_CurrencyMaster ON Inv_Vendor.N_CurrencyID = Acc_CurrencyMaster.N_CurrencyID LEFT OUTER JOIN Prj_WorkOrder ON Inv_BalanceAdjustmentMaster.N_CompanyID = Prj_WorkOrder.N_CompanyId AND Inv_BalanceAdjustmentMaster.N_WOID = Prj_WorkOrder.N_WorkOrderId Where  Inv_BalanceAdjustmentMaster.N_CompanyID=@p1 and Inv_BalanceAdjustmentMaster.N_FnYearID=@p3 and Inv_BalanceAdjustmentMaster.N_TransType=@p2 and Inv_BalanceAdjustmentMaster.X_VoucherNo=@p4 and Inv_BalanceAdjustmentMaster.N_PartyType=@p5 and Inv_BalanceAdjustmentMaster.N_BranchID=@p6";
                Params.Add("@p6", nBranchID);
            }
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", N_TransType);
            Params.Add("@p3", nFnYearId);
            Params.Add("@p4", X_ReceiptNo);
            Params.Add("@p5", N_PartyType);
            

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Data Found !!"));
                    }

                    MasterTable = _api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                    //BalaceAdjustment Details
                    int N_AdjustmentId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AdjustmentId"].ToString());

                    string DetailSql = "";

        //             DetailSql = "Select Inv_BalanceAdjustmentMasterDetails.*,Acc_MastLedger.* from Inv_BalanceAdjustmentMasterDetails " +
        // " Left Outer JOIN Acc_MastLedger On Inv_BalanceAdjustmentMasterDetails.N_LedgerID= Acc_MastLedger.N_LedgerID and Inv_BalanceAdjustmentMasterDetails.N_CompanyID = Acc_MastLedger.N_CompanyID" +
        // " Where Inv_BalanceAdjustmentMasterDetails.N_CompanyID=@p1 and  Acc_MastLedger.N_FnYearID=@p3 and Inv_BalanceAdjustmentMasterDetails.N_AdjustmentId=" + N_AdjustmentId;
DetailSql = "Select * from vw_InvBalanceAdjustmentDetaiils  Where N_CompanyID=@p1 and  N_FnYearID=@p3 and N_AdjustmentId=" + N_AdjustmentId;
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);
                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                SortedList Params = new SortedList();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    DataRow MasterRow = MasterTable.Rows[0];
                    transaction = connection.BeginTransaction();


                    int N_AdjustmentID = myFunctions.getIntVAL(MasterRow["n_AdjustmentId"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int N_BranchID = myFunctions.getIntVAL(MasterRow["n_BranchID"].ToString());
                    int N_TransType = myFunctions.getIntVAL(MasterRow["n_TransType"].ToString());
                    int N_PartyType = myFunctions.getIntVAL(MasterRow["n_PartyType"].ToString());
                    string X_Trasnaction = "";
                    int N_FormID =0;



                    if (N_PartyType == 0)//Vendor
                    {
                        if (N_TransType == 1)
                            {X_Trasnaction = "VENDOR CREDIT NOTE";
                            N_FormID=507;}
                        else if (N_TransType == 0)
                            {X_Trasnaction = "VENDOR DEBIT NOTE";
                            N_FormID=508;}
                    }
                    if (N_PartyType == 1)//customer
                    {
                        if (N_TransType == 1)
                           { X_Trasnaction = "CUSTOMER CREDIT NOTE";
                            N_FormID=504;}
                        else if (N_TransType == 0)
                            {X_Trasnaction = "CUSTOMER DEBIT NOTE";
                            N_FormID=505;}
                    }

                    // Auto Gen
                    string AdjustmentNo = "";
                    var values = MasterTable.Rows[0]["X_VoucherNo"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", Master["n_CompanyId"].ToString());
                        Params.Add("N_YearID", Master["n_FnYearId"].ToString());
                        Params.Add("N_FormID", N_FormID);
                        Params.Add("N_BranchID", Master["n_BranchId"].ToString());

                        AdjustmentNo = dLayer.GetAutoNumber("Inv_BalanceAdjustmentMaster", "X_VoucherNo", Params, connection, transaction);
                        if (AdjustmentNo == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Adjustment Number")); }
                        MasterTable.Rows[0]["X_VoucherNo"] = AdjustmentNo;

                    }
                    else
                    {
                        if (N_AdjustmentID > 0)
                        {
                            SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",N_CompanyID},
                                {"X_TransType",X_Trasnaction},
                                {"N_VoucherID",N_AdjustmentID}};
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                        }
                    }

                    N_AdjustmentID = dLayer.SaveData("Inv_BalanceAdjustmentMaster", "N_AdjustmentID", MasterTable, connection, transaction);
                    if (N_AdjustmentID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Adjustment"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_AdjustmentID"] = N_AdjustmentID;
                    }

                    int N_AdjustmentDetailsId = dLayer.SaveData("Inv_BalanceAdjustmentMasterDetails", "N_AdjustmentDetailsId", DetailTable, connection, transaction);
                    if (N_AdjustmentDetailsId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Adjustment"));
                    }
                    else
                    {
                        transaction.Commit();
                    }
                    return Ok(_api.Success("Adjustment saved" + ":" + N_AdjustmentID));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID, int nAdjustmentId, string xTransType)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList deleteParams = new SortedList()
                            {
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType",xTransType},
                                {"N_VoucherID",nAdjustmentId}

                            };
                    dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", deleteParams, connection, transaction);
                    transaction.Commit();
                }
                return Ok(_api.Success("Deleted"));
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }






    }
}