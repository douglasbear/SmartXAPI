
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
    [Route("batchPosting")]
    [ApiController]
    public class Acc_BatchProcessing : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int N_FormID;
        public Acc_BatchProcessing(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 152;
        }


        [HttpGet("list")]
        public ActionResult GetAssetSalesList(int? nCompanyId, int nFnYearId,int nCustomerID,int nBranchID,bool bAllBranchData, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            DataTable CountTable = new DataTable();
            SortedList Params = new SortedList();
            DataSet dataSet = new DataSet();
            string sqlCommandText = "";
            string sqlCondition = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_InvoiceNo like '%" + xSearchkey + "%' or X_CustomerName like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_AssetInventoryID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "invoiceNo":
                        xSortBy = "N_AssetInventoryID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }
            
            if (bAllBranchData)
            {
                if (nCustomerID > 0)
                    sqlCondition = "N_CompanyID=@p1 and N_FnYearID=@p2 and N_CustomerID=@p4";
                else
                    sqlCondition = "N_CompanyID=@p1 and N_FnYearID=@p2";
            }
            else
            {
                if (nCustomerID > 0)
                    sqlCondition = "N_CompanyID=@p1 and N_FnYearID=@p2 and N_CustomerID=@p4 and N_BranchID=@p3";
                else
                    sqlCondition = "N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3";
            }
        

            int Count = (nPage - 1) * nSizeperpage;
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_CustomerID,N_AssetInventoryID,N_FnYearID,N_BranchID,X_InvoiceNo,D_InvoiceDate,X_CustomerName,X_Type from vw_InvAssetInventoryReceiptNo_Search where "+sqlCondition+" " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_CustomerID,N_AssetInventoryID,N_FnYearID,N_BranchID,X_InvoiceNo,D_InvoiceDate,X_CustomerName,X_Type from vw_InvAssetInventoryReceiptNo_Search where "+sqlCondition+" " + Searchkey + " and N_AssetInventoryID not in (select top(" + Count + ") N_AssetInventoryID from vw_InvAssetInventoryReceiptNo_Search where "+sqlCondition+" " + xSortBy + " ) " + xSortBy;

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", nBranchID);
            Params.Add("@p4", nCustomerID);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count from vw_InvAssetInventoryReceiptNo_Search where "+sqlCondition+" " + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    //string TotalSum = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                       // TotalSum = drow["TotalAmount"].ToString();
                    }
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                   // OutPut.Add("TotalSum", TotalSum);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
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

        [HttpGet("transType")]
        public ActionResult TransactionTypeList(int nCompanyId, int nFnYearID,int nBranchID,bool bAllBranchData,int FormID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nFnYearID",nFnYearID);
            Params.Add("@nBranchID",nBranchID);

            string sqlCommandText="";

            if(FormID==152)
            {
                if (bAllBranchData)
                    sqlCommandText="SELECT * FROM vw_BatchPosting_Disp WHERE N_CompanyID=@nCompanyID and X_ID<>'OB' and N_FnYearID=@nFnYearID";
                else
                    sqlCommandText="SELECT * FROM vw_BatchPosting_Disp WHERE N_CompanyID=@nCompanyID and X_ID<>'OB' and N_FnYearID=@nFnYearID and N_BranchID=@nBranchID";
            }
            else
            {
                if (bAllBranchData)
                    sqlCommandText="SELECT * FROM vw_BatchPosting_Disp WHERE N_CompanyID=@nCompanyID And B_IsAccPosted=1 and N_FnYearID=@nFnYearID";
                else
                    sqlCommandText="SELECT * FROM vw_BatchPosting_Disp WHERE N_CompanyID=@nCompanyID And B_IsAccPosted=1 and N_FnYearID=@nFnYearID and N_BranchID=@nBranchID";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
                return Ok(_api.Error(e));
            }
        }
 
        
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            
            DataTable MasterTable;
            DataTable DetailTable;
            DataTable SalesTable = new DataTable();
            DataTable SalesAmountTable = new DataTable();
            DataTable TransactionTable = new DataTable();
            DataTable AssMasterTable = new DataTable();
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            TransactionTable = ds.Tables["transaction"];
            AssMasterTable = ds.Tables["assMaster"];
            SortedList Params = new SortedList();
            // Auto Gen
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction=connection.BeginTransaction();
                    string ReturnNo="",xTransType="";
                    int nCompanyID =myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int N_AssetInventoryID =myFunctions.getIntVAL(MasterTable.Rows[0]["N_AssetInventoryID"].ToString());
                    int TypeID =myFunctions.getIntVAL(MasterTable.Rows[0]["N_TypeID"].ToString());
                    int nLocationID =myFunctions.getIntVAL(MasterTable.Rows[0]["N_LocationID"].ToString());
                    int N_UserID=myFunctions.GetUserID(User);
                    var X_InvoiceNo = MasterTable.Rows[0]["X_InvoiceNo"].ToString();
                    MasterTable.Columns.Remove("N_LocationID");

                    if(X_InvoiceNo=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID",MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID",N_FormID);
                        Params.Add("N_BranchID",MasterTable.Rows[0]["n_BranchId"].ToString());
                        ReturnNo =  dLayer.GetAutoNumber("Ass_SalesMaster","X_InvoiceNo", Params,connection,transaction);
                        if(ReturnNo==""){transaction.Rollback(); return Ok(_api.Warning("Unable to generate Invoice Number"));}
                        MasterTable.Rows[0]["X_InvoiceNo"] = ReturnNo;
                    }

                    if(N_AssetInventoryID>0)
                    {
                        SortedList DeleteParams = new SortedList(){
                            {"N_CompanyID",nCompanyID},                   
                            {"X_TransType","ASSET SALES"},
                            {"N_VoucherID",N_AssetInventoryID},
                            {"N_UserID",N_UserID},
                            {"X_SystemName",System.Environment.MachineName}};
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(ex));
                        }
                    }

                    N_AssetInventoryID=dLayer.SaveData("Ass_SalesMaster","N_AssetInventoryID",MasterTable,connection,transaction);                    
                    if(N_AssetInventoryID<=0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Error"));
                    }
                    int SalesID=0;
                    if (TypeID==287)
                    {
                        SortedList SalesParams = new SortedList();
                        SalesParams.Add("@N_AssetInventoryID",N_AssetInventoryID);
                        SalesParams.Add("@nCompanyID",nCompanyID);
                        SalesParams.Add("@nLocationID",nLocationID);
                        string sqlCommandText="select N_CompanyID,N_FnYearID,X_InvoiceNo AS X_ReceiptNo,D_InvoiceDate AS D_SalesDate,D_EntryDate,N_InvoiceAmt AS N_BillAmt,N_InvoiceAmt AS N_BillAmtF,N_DiscountAmt,N_DiscountAmt AS N_DiscountAmtf,N_CashReceived,N_CashReceived AS N_CashReceivedf,N_UserID,'ASSET SALES' AS X_TransType,4 AS N_SalesType,N_AssetInventoryID AS N_SalesRefID,N_BranchID,@nLocationID AS N_LocationID,N_CustomerID AS N_CustomerId from Ass_SalesMaster where N_CompanyID=@nCompanyID and N_AssetInventoryID=@N_AssetInventoryID";

                        SalesTable = dLayer.ExecuteDataTable(sqlCommandText, SalesParams, connection);
                        SalesTable = _api.Format(SalesTable, "Sales");

                        SalesID=dLayer.SaveData("Inv_Sales","N_SalesId",SalesTable,connection,transaction); 
                        if(SalesID<=0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Error"));
                        }
                        else
                        {
                            double N_SChrgAmt = 0,N_ServiceCharge=0;
                            object ServiceCharge =dLayer.ExecuteScalar("Select ISNULL(N_ServiceCharge , 0) from Inv_Customer where N_CustomerID=" + nCompanyID + " and N_CompanyID=" + nCompanyID + "and N_FnYearID=" + myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString()), connection, transaction);                                  
                            N_ServiceCharge=myFunctions.getVAL(ServiceCharge.ToString());
                            if(N_ServiceCharge>0)
                            {
                                N_SChrgAmt = (myFunctions.getVAL(MasterTable.Rows[0]["n_InvoiceAmt"].ToString()) * N_ServiceCharge / 100);
                            }

                            SortedList SalesAmtParams = new SortedList();
                            SalesAmtParams.Add("@SalesID",SalesID);
                            SalesAmtParams.Add("@nCompanyID",nCompanyID);
                            SalesAmtParams.Add("@nSChrgAmt",N_SChrgAmt);
                            SalesAmtParams.Add("@nServiceCharge",N_ServiceCharge);
                            string sqlAmtcmdText="select N_CompanyID,N_BranchID,N_SalesId,N_CustomerID,N_BillAmt AS N_Amount,N_BillAmtF AS N_AmountF,@nSChrgAmt AS N_CommissionAmt,@nSChrgAmt AS N_CommissionAmtF,@nServiceCharge AS N_CommissionPer from Inv_Sales where N_CompanyID=@nCompanyID and N_SalesID=@SalesID";

                            SalesAmountTable = dLayer.ExecuteDataTable(sqlAmtcmdText, SalesAmtParams, connection);
                            SalesAmountTable = _api.Format(SalesAmountTable, "SalesAmt");

                        }
                    }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                    {
                        DetailTable.Rows[j]["N_AssetInventoryID"]=N_AssetInventoryID;
                    }
                    int N_AssetInventoryDetailsID=dLayer.SaveData("Ass_SalesDetails","N_AssetInventoryDetailsID",DetailTable,connection,transaction);                    
                    if(N_AssetInventoryDetailsID<=0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Error"));
                    }

                    string X_Type = "";
                    if (TypeID == 288)
                        X_Type = "Disposal";
                    else
                        X_Type = "Sales";

                    for (int k = 0 ;k < TransactionTable.Rows.Count;k++)
                    {
                        TransactionTable.Rows[k]["N_AssetInventoryID"]=N_AssetInventoryID;
                        TransactionTable.Rows[k]["X_Reference"]=ReturnNo;
                        TransactionTable.Rows[k]["X_Type"]=X_Type;
                        TransactionTable.Rows[k]["N_AssetInventoryDetailsID"]=DetailTable.Rows[k]["N_AssetInventoryDetailsID"];
                    }
                    int N_ActionID=dLayer.SaveData("Ass_Transactions","N_ActionID",TransactionTable,connection,transaction);                    
                    if(N_ActionID<=0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Error"));
                    }
                   
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                    {
                        if (TypeID == 288)
                            dLayer.ExecuteNonQuery("update Ass_AssetMaster set N_Status=5 where N_ItemID=" + DetailTable.Rows[j]["N_ItemID"], connection, transaction);                                  
                        else
                            dLayer.ExecuteNonQuery("update Ass_AssetMaster set N_Status=2 where N_ItemID=" + DetailTable.Rows[j]["N_ItemID"], connection, transaction);                                  
                    }

                    SortedList PostingParam = new SortedList();
                    PostingParam.Add("N_CompanyID", MasterTable.Rows[0]["n_CompanyId"].ToString());
                    PostingParam.Add("X_InventoryMode", "ASSET SALES");
                    PostingParam.Add("N_InternalID", N_AssetInventoryID);
                    PostingParam.Add("N_UserID", N_UserID);
                    try
                    {
                        dLayer.ExecuteNonQueryPro("SP_Acc_InventoryPosting", PostingParam, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(ex));
                    }

                    SortedList Result = new SortedList();
                    Result.Add("N_AssetInventoryID",N_AssetInventoryID);
                    Result.Add("X_InvoiceNo",ReturnNo);
                    transaction.Commit();
                    return Ok(_api.Success(Result,"Asset Purchase Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpGet("details")]
        public ActionResult AssSalesDetails(string xDescription,int nFnYearID,int nBranchId, bool bAllBranchData,DateTime dDateFrom,DateTime dDateTo,int flag,int FormID,string xRefNo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction=connection.BeginTransaction();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();

                    string Mastersql = "";
                    string X_Trans = "";
                    if(xRefNo==null)xRefNo="";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@nFnYearID", nFnYearID);
                    Params.Add("@nBranchId", nBranchId);
                    Params.Add("@flag", flag);
                    Params.Add("@dDateFrom", dDateFrom);
                    Params.Add("@dDateTo", dDateTo);
                    //Params.Add("@xRefNo", xRefNo);
           
                    object obj1 = dLayer.ExecuteScalar("Select X_ID from Acc_VoucherTypes where (X_Description='" + xDescription + "' or X_Description_Ar='" + xDescription + "')", connection, transaction);
                    if (obj1 != null)
                        X_Trans = obj1.ToString();

                    Params.Add("@X_Trans", X_Trans);

                    if(FormID==152)
                    {
                        if (bAllBranchData)
                            Mastersql = "SP_Acc_BatchProcessing_Disp @flag,@nCompanyID,@nFnYearID,@X_Trans,@dDateFrom,@dDateTo,0";
                        else
                            Mastersql = "SP_Acc_BatchProcessing_Disp @flag,@nCompanyID,@nFnYearID,@X_Trans,@dDateFrom,@dDateTo,@nBranchId";
                    }
                    else
                    {
                        if(flag==0)
                        {
                            if (bAllBranchData)
                                Mastersql = "Select ROW_NUMBER() OVER (ORDER BY D_voucherDate) as 'S/N', N_VoucherID,X_TransType,X_VoucherNo As [Voucher No],Replace(Convert(Varchar(11),D_VoucherDate,106),' ','-') AS [Voucher Date],X_Remarks as [Remarks],X_ReferenceNo As [Reference No], " +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " Else (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount >0)" +                      
                                            " END As [Debit]," +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " else -1*(Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount <0)" +                                              
                                            " END As [Credit], CONVERT(bit,0) As [Select] from Acc_VoucherMaster"+
                                            " Where Acc_VoucherMaster.B_IsAccPosted = 1 and Acc_VoucherMaster.N_CompanyID= @nCompanyID and Acc_VoucherMaster.N_FnYearID = @nFnYearID and Acc_VoucherMaster.X_TransType=@X_Trans And X_ReferenceNo like '%"+xRefNo+"%' Order By Acc_VoucherMaster.D_VoucherDate";
                            else
                                Mastersql = "Select ROW_NUMBER() OVER (ORDER BY D_voucherDate) as 'S/N', N_VoucherID,X_TransType,X_VoucherNo As [Voucher No],Replace(Convert(Varchar(11),D_VoucherDate,106),' ','-') AS [Voucher Date],X_Remarks as [Remarks],X_ReferenceNo As [Reference No], " +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " Else (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount >0)" +                                              
                                            " END As [Debit]," +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " else -1*(Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount <0)" +        
                                            " END As [Credit], CONVERT(bit,0) As [Select] from Acc_VoucherMaster"+
                                            " Where Acc_VoucherMaster.B_IsAccPosted = 1 and Acc_VoucherMaster.N_CompanyID= @nCompanyID and Acc_VoucherMaster.N_FnYearID = @nFnYearID and Acc_VoucherMaster.X_TransType=@X_Trans And X_ReferenceNo like '%"+xRefNo+"%' and N_BranchID=@nBranchId  Order By Acc_VoucherMaster.D_VoucherDate";
                        }
                        else
                        {
                            if (bAllBranchData)
                                Mastersql = "Select ROW_NUMBER() OVER (ORDER BY D_voucherDate) as 'S/N', N_VoucherID,X_TransType,X_VoucherNo As [Voucher No],Replace(Convert(Varchar(11),D_VoucherDate,106),' ','-') AS [Voucher Date],X_Remarks as [Remarks],X_ReferenceNo As [Reference No], " +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " Else (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount >0)" +                                               
                                            " END As [Debit]," +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            "Else -1*(Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount <0)" +                                                   
                                            " END As [Credit], CONVERT(bit,0) As [Select] from Acc_VoucherMaster"+
                                            " Where Acc_VoucherMaster.B_IsAccPosted = 1 and Acc_VoucherMaster.N_CompanyID= @nCompanyID and Acc_VoucherMaster.N_FnYearID = @nFnYearID and Convert(Varchar(11),D_VoucherDate,23) Between '"+dDateFrom+"' and '"+@dDateTo+"' and Acc_VoucherMaster.X_TransType=@X_Trans And X_ReferenceNo like '%" + xRefNo + "%' Order By Acc_VoucherMaster.D_VoucherDate ";
                            else
                                Mastersql = "Select ROW_NUMBER() OVER (ORDER BY D_voucherDate) as 'S/N', N_VoucherID,X_TransType,X_VoucherNo As [Voucher No],Replace(Convert(Varchar(11),D_VoucherDate,106),' ','-') AS [Voucher Date],X_Remarks as [Remarks],X_ReferenceNo As [Reference No], " +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " Else (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount >0)" +                                               
                                            " END As [Debit]," +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            "Else -1*(Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount <0)" +                                                   
                                            " END As [Credit], CONVERT(bit,0) As [Select] from Acc_VoucherMaster"+
                                            " Where Acc_VoucherMaster.B_IsAccPosted = 1 and Acc_VoucherMaster.N_CompanyID= @nCompanyID and Acc_VoucherMaster.N_FnYearID = @nFnYearID and Convert(Varchar(11),D_VoucherDate,23) Between '"+dDateFrom+"' and '"+@dDateTo+"' and Acc_VoucherMaster.X_TransType=@X_Trans And X_ReferenceNo like '%" + xRefNo + "%' and N_BranchID=@nBranchId Order By Acc_VoucherMaster.D_VoucherDate ";
                        }
                    }

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection,transaction);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }

                    MasterTable = _api.Format(MasterTable, "Master");

                    dt.Tables.Add(MasterTable);
                    return Ok(_api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }


       [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID,int N_AssetInventoryID)
        {
            int Results = 0;
            var nUserID = myFunctions.GetUserID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                   if(N_AssetInventoryID>0)
                    {
                        SortedList DeleteParams = new SortedList(){
                            {"N_CompanyID",nCompanyID},                   
                            {"X_TransType","ASSET SALES"},
                            {"N_VoucherID",N_AssetInventoryID},
                            {"N_UserID",nUserID},
                            {"X_SystemName",System.Environment.MachineName}};
                        try
                        {
                            Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(ex));
                        }
                    }
                    if (Results >= 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Asset Sales deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to delete Asset Sales"));
                    }


                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
   } 
  
 }