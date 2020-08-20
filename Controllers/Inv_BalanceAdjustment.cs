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
        private readonly int FormID;


        public Inv_BalanceAdjustment(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 290;
        }


        //List
        [HttpGet("list")]
        public ActionResult GetBalanceDetails(int? nCompanyID, int? nFnyearID,int? nPartyType,int nPartyID,int N_TransType)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText="";
            if(nPartyID>0)

            sqlCommandText = "select [Adjustment Date],[Invoice No],[Customer Name],[Net Amount] from vw_CustomerBalanceAdjustment where N_CompanyID=@p1  and N_CustomerID=@p3 and N_TransType=@p4 and B_YearEndProcess=0 and N_PartyType=@p5 group by [Adjustment Date],[Invoice No],[Customer Name],[Net Amount]";

            else
            sqlCommandText = "select [Adjustment Date],[Invoice No],[Customer Name],[Net Amount] from vw_CustomerBalanceAdjustment where N_CompanyID=@p1  and N_TransType=@p4 and B_YearEndProcess=0 and N_PartyType=@p5 group by [Adjustment Date],[Invoice No],[Customer Name],[Net Amount]";

            Params.Add("@p1", nCompanyID);
            Params.Add("@p3", nPartyID);
            Params.Add("@p4", N_TransType);
            Params.Add("@p5", nPartyType);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return BadRequest(_api.Error(e));
            }
        }
          [HttpGet("listDetails")]
        public ActionResult GetBalanceListDetails(int nCompanyId, int N_PartyType,string N_TransType, int nFnYearId, string X_ReceiptNo, bool bAllBranchData, int nBranchID)
        {
            bool B_PRSVisible = false;
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            DataTable DataTable = new DataTable();

            string Mastersql = "";

            if (bAllBranchData == true)
            {
                if(N_PartyType==1)
                Mastersql = "Select Inv_Customer.*,Inv_BalanceAdjustmentMaster.* from Inv_BalanceAdjustmentMaster Inner Join  Inv_Customer ON Inv_BalanceAdjustmentMaster.N_PartyID = Inv_Customer.N_CustomerID And Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_Customer.N_CompanyID and Inv_BalanceAdjustmentMaster.N_FnYearID=Inv_Customer.N_FnYearID Where  Inv_BalanceAdjustmentMaster.N_CompanyID=@p1 and Inv_BalanceAdjustmentMaster.N_TransType =@p2 and Inv_BalanceAdjustmentMaster.N_FnYearID=@p3 and Inv_BalanceAdjustmentMaster.X_VoucherNo=@p4 and Inv_BalanceAdjustmentMaster.N_PartyType=@p5";
            else if(N_PartyType==0)
            Mastersql ="SELECT     Inv_BalanceAdjustmentMaster.N_CompanyID, Inv_BalanceAdjustmentMaster.N_FnYearID, Inv_BalanceAdjustmentMaster.X_VoucherNo, Inv_BalanceAdjustmentMaster.D_AdjustmentDate, Inv_BalanceAdjustmentMaster.D_EntryDate, Inv_BalanceAdjustmentMaster.N_Amount, Inv_BalanceAdjustmentMaster.N_UserID, Inv_BalanceAdjustmentMaster.N_BranchID, Inv_BalanceAdjustmentMaster.N_TransType, Inv_BalanceAdjustmentMaster.X_notes, Inv_BalanceAdjustmentMaster.N_PartyType, Inv_BalanceAdjustmentMaster.N_PartyID, Inv_BalanceAdjustmentMaster.N_AdjustmentId, Inv_BalanceAdjustmentMaster.N_AmountF, Inv_BalanceAdjustmentMaster.N_CurExchRate, Inv_BalanceAdjustmentMaster.N_WOID, Inv_Vendor.N_CompanyID AS Expr1, Inv_Vendor.N_VendorID, Inv_Vendor.X_VendorCode, Inv_Vendor.X_VendorName, Inv_Vendor.X_ContactName, Inv_Vendor.X_Address, Inv_Vendor.X_ZipCode, Inv_Vendor.X_Country, Inv_Vendor.X_PhoneNo1, Inv_Vendor.X_PhoneNo2, Inv_Vendor.X_FaxNo, Inv_Vendor.X_Email, Inv_Vendor.X_WebSite, Inv_Vendor.N_CreditLimit, Inv_Vendor.B_Inactive, Inv_Vendor.N_LedgerID, Inv_Vendor.N_InvDueDays, Inv_Vendor.N_FnYearID AS Expr2, Inv_Vendor.D_Entrydate AS Expr3, Inv_Vendor.X_CountryCode, Inv_Vendor.N_TypeID, Inv_Vendor.B_DirPosting, Inv_Vendor.N_CurrencyID, Inv_Vendor.X_ReminderMsg, Inv_Vendor.X_VendorName_Ar, Inv_Vendor.N_CountryID, Inv_Vendor.X_TaxRegistrationNo, Inv_Vendor.N_TaxCategoryID, Inv_Vendor.B_AllowCashPay, Inv_Vendor.N_PartnerTypeID, Inv_Vendor.N_VendorTypeID, Inv_Vendor.N_GoodsDeliveryIn, Inv_Vendor.X_TandC, Acc_CurrencyMaster.N_CompanyID AS Expr4, Acc_CurrencyMaster.N_CurrencyID AS Expr5, Acc_CurrencyMaster.X_CurrencyCode, Acc_CurrencyMaster.X_CurrencyName, Acc_CurrencyMaster.X_ShortName, Acc_CurrencyMaster.N_ExchangeRate, Acc_CurrencyMaster.B_default, Acc_CurrencyMaster.N_Decimal, Prj_WorkOrder.N_WorkOrderId, Prj_WorkOrder.X_WorkOrderNo FROM         Inv_BalanceAdjustmentMaster INNER JOIN Inv_Vendor ON Inv_BalanceAdjustmentMaster.N_PartyID = Inv_Vendor.N_VendorID AND Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_Vendor.N_CompanyID AND  Inv_BalanceAdjustmentMaster.N_FnYearID = Inv_Vendor.N_FnYearID INNER JOIN Acc_CurrencyMaster ON Inv_Vendor.N_CurrencyID = Acc_CurrencyMaster.N_CurrencyID LEFT OUTER JOIN Prj_WorkOrder ON Inv_BalanceAdjustmentMaster.N_CompanyID = Prj_WorkOrder.N_CompanyId AND Inv_BalanceAdjustmentMaster.N_WOID = Prj_WorkOrder.N_WorkOrderId Where  Inv_BalanceAdjustmentMaster.N_CompanyID=@p1 and Inv_BalanceAdjustmentMaster.N_FnYearID=@p3 and Inv_BalanceAdjustmentMaster.N_TransType=@p2 and Inv_BalanceAdjustmentMaster.X_VoucherNo=@p4 and Inv_BalanceAdjustmentMaster.N_PartyType=@p5";
            
            }
            else
            {
                 if(N_PartyType==1)
                Mastersql = "Select Inv_Customer.*,Inv_BalanceAdjustmentMaster.* from Inv_BalanceAdjustmentMaster Inner Join  Inv_Customer ON Inv_BalanceAdjustmentMaster.N_PartyID = Inv_Customer.N_CustomerID And Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_Customer.N_CompanyID and Inv_BalanceAdjustmentMaster.N_FnYearID=Inv_Customer.N_FnYearID Where  Inv_BalanceAdjustmentMaster.N_CompanyID=@p1 and Inv_BalanceAdjustmentMaster.N_TransType =@p2 and Inv_BalanceAdjustmentMaster.N_FnYearID=@p3 and Inv_BalanceAdjustmentMaster.X_VoucherNo=@p4 and Inv_BalanceAdjustmentMaster.N_PartyType=@p5 and Inv_BalanceAdjustmentMaster.N_BranchID=@p6";
            else if(N_PartyType==0)
            Mastersql ="SELECT     Inv_BalanceAdjustmentMaster.N_CompanyID, Inv_BalanceAdjustmentMaster.N_FnYearID, Inv_BalanceAdjustmentMaster.X_VoucherNo, Inv_BalanceAdjustmentMaster.D_AdjustmentDate, Inv_BalanceAdjustmentMaster.D_EntryDate, Inv_BalanceAdjustmentMaster.N_Amount, Inv_BalanceAdjustmentMaster.N_UserID, Inv_BalanceAdjustmentMaster.N_BranchID, Inv_BalanceAdjustmentMaster.N_TransType, Inv_BalanceAdjustmentMaster.X_notes, Inv_BalanceAdjustmentMaster.N_PartyType, Inv_BalanceAdjustmentMaster.N_PartyID, Inv_BalanceAdjustmentMaster.N_AdjustmentId, Inv_BalanceAdjustmentMaster.N_AmountF, Inv_BalanceAdjustmentMaster.N_CurExchRate, Inv_BalanceAdjustmentMaster.N_WOID, Inv_Vendor.N_CompanyID AS Expr1, Inv_Vendor.N_VendorID, Inv_Vendor.X_VendorCode, Inv_Vendor.X_VendorName, Inv_Vendor.X_ContactName, Inv_Vendor.X_Address, Inv_Vendor.X_ZipCode, Inv_Vendor.X_Country, Inv_Vendor.X_PhoneNo1, Inv_Vendor.X_PhoneNo2, Inv_Vendor.X_FaxNo, Inv_Vendor.X_Email, Inv_Vendor.X_WebSite, Inv_Vendor.N_CreditLimit, Inv_Vendor.B_Inactive, Inv_Vendor.N_LedgerID, Inv_Vendor.N_InvDueDays, Inv_Vendor.N_FnYearID AS Expr2, Inv_Vendor.D_Entrydate AS Expr3, Inv_Vendor.X_CountryCode, Inv_Vendor.N_TypeID, Inv_Vendor.B_DirPosting, Inv_Vendor.N_CurrencyID, Inv_Vendor.X_ReminderMsg, Inv_Vendor.X_VendorName_Ar, Inv_Vendor.N_CountryID, Inv_Vendor.X_TaxRegistrationNo, Inv_Vendor.N_TaxCategoryID, Inv_Vendor.B_AllowCashPay, Inv_Vendor.N_PartnerTypeID, Inv_Vendor.N_VendorTypeID, Inv_Vendor.N_GoodsDeliveryIn, Inv_Vendor.X_TandC, Acc_CurrencyMaster.N_CompanyID AS Expr4, Acc_CurrencyMaster.N_CurrencyID AS Expr5, Acc_CurrencyMaster.X_CurrencyCode, Acc_CurrencyMaster.X_CurrencyName, Acc_CurrencyMaster.X_ShortName, Acc_CurrencyMaster.N_ExchangeRate, Acc_CurrencyMaster.B_default, Acc_CurrencyMaster.N_Decimal, Prj_WorkOrder.N_WorkOrderId, Prj_WorkOrder.X_WorkOrderNo FROM         Inv_BalanceAdjustmentMaster INNER JOIN Inv_Vendor ON Inv_BalanceAdjustmentMaster.N_PartyID = Inv_Vendor.N_VendorID AND Inv_BalanceAdjustmentMaster.N_CompanyID = Inv_Vendor.N_CompanyID AND  Inv_BalanceAdjustmentMaster.N_FnYearID = Inv_Vendor.N_FnYearID INNER JOIN Acc_CurrencyMaster ON Inv_Vendor.N_CurrencyID = Acc_CurrencyMaster.N_CurrencyID LEFT OUTER JOIN Prj_WorkOrder ON Inv_BalanceAdjustmentMaster.N_CompanyID = Prj_WorkOrder.N_CompanyId AND Inv_BalanceAdjustmentMaster.N_WOID = Prj_WorkOrder.N_WorkOrderId Where  Inv_BalanceAdjustmentMaster.N_CompanyID=@p1 and Inv_BalanceAdjustmentMaster.N_FnYearID=@p3 and Inv_BalanceAdjustmentMaster.N_TransType=@p2 and Inv_BalanceAdjustmentMaster.X_VoucherNo=@p4 and Inv_BalanceAdjustmentMaster.N_PartyType=@p5 and Inv_BalanceAdjustmentMaster.N_BranchID=@p6";
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

                    MasterTable = _api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                    //BalaceAdjustment Details
                    int N_AdjustmentId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AdjustmentId"].ToString());

                    string DetailSql = "";
                    
                                DetailSql = "Select Inv_BalanceAdjustmentMasterDetails.*,Acc_MastLedger.* from Inv_BalanceAdjustmentMasterDetails " +
                    " Left Outer JOIN Acc_MastLedger On Inv_BalanceAdjustmentMasterDetails.N_LedgerID= Acc_MastLedger.N_LedgerID and Inv_BalanceAdjustmentMasterDetails.N_CompanyID = Acc_MastLedger.N_CompanyID" +
                    " Where Inv_BalanceAdjustmentMasterDetails.N_CompanyID=@p1 and  Acc_MastLedger.N_FnYearID=@p3 and Inv_BalanceAdjustmentMasterDetails.N_AdjustmentId="+N_AdjustmentId;
                            
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);
                }
                return Ok(dt);
            }
            catch (Exception e)
            {
                return StatusCode(403, _api.ErrorResponse(e));
            }
        }
        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    string ExecutiveCode = MasterTable.Rows[0]["X_SalesmanCode"].ToString();
                    string SalesmanName = MasterTable.Rows[0]["X_SalesmanName"].ToString();
                    string nCompanyID = MasterTable.Rows[0]["n_CompanyId"].ToString();
                    string nFnYearID = MasterTable.Rows[0]["n_FnYearId"].ToString();
                    int N_SalesmanID= myFunctions.getIntVAL(MasterTable.Rows[0]["n_SalesmanID"].ToString());
                    if (ExecutiveCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("N_BranchID", MasterTable.Rows[0]["n_BranchId"].ToString());
                        ExecutiveCode = dLayer.GetAutoNumber("inv_salesman", "X_SalesmanCode", Params, connection, transaction);
                        if (ExecutiveCode == "") { return Ok(_api.Error("Unable to generate Sales Executive Code")); }
                        MasterTable.Rows[0]["X_SalesmanCode"] = ExecutiveCode;

                    }else
                    {
                        dLayer.DeleteData("inv_salesman", "N_SalesmanID", N_SalesmanID, "", connection, transaction);
                    }

                        MasterTable.Columns.Remove("n_SalesmanID");
                        MasterTable.AcceptChanges();


                    N_SalesmanID = dLayer.SaveData("inv_salesman", "N_SalesmanID", N_SalesmanID, MasterTable, connection, transaction);
                    if (N_SalesmanID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {

                        SortedList nParams = new SortedList();
                        nParams.Add("@p1",nCompanyID);
                        nParams.Add("@p2",nFnYearID);
                        nParams.Add("@p3",N_SalesmanID);
                        string sqlCommandText = "select * from vw_InvSalesman where N_CompanyID=@p1 and N_FnYearID=@p2 and n_SalesmanID=@p3";
                        DataTable outputDt = dLayer.ExecuteDataTable(sqlCommandText, nParams, connection,transaction);
                        outputDt = _api.Format(outputDt, "NewSalesMan");

                        if(outputDt.Rows.Count==0){
                            transaction.Rollback();
                            return Ok(_api.Error("Unable to save"));
                        }
                        DataRow NewRow = outputDt.Rows[0];
                        return Ok(_api.Success(NewRow.Table, "Salesman Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(ex));
            }
        }



                [HttpDelete("delete")]
        public ActionResult DeleteData(int nSalesmanID)
            {
                int Results = 0;
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("inv_salesman", "N_SalesmanID", nSalesmanID, "",connection);
                }
                    if (Results > 0)
                    {
                        return Ok(_api.Success("Sales Executive deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete Sales Executive"));
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(_api.ErrorResponse(ex));
                }


            }

           




        }
    }