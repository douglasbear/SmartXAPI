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
    [Route("purchaseorder")]
    [ApiController]
    public class Inv_PurchaseOrderController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int FormID;


        public Inv_PurchaseOrderController(IDataAccessLayer dl, IApiFunctions _api, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            dLayer = dl;
            api = _api;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 82;
        }


        [HttpGet("list")]
        public ActionResult GetPurchaseOrderList(int? nCompanyId, int nFnYearId, bool bAllBranchData, int nBranchID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,string screen,int nFormID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";
                    string criteria = "";
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int N_decimalPlace = 2;
                    // string UserPattern = myFunctions.GetUserPattern(User);
                    int nUserID = myFunctions.GetUserID(User);
                    N_decimalPlace = myFunctions.getIntVAL(myFunctions.ReturnSettings("Purchase", "Decimal_Place", "N_Value", nCompanyID, dLayer, connection));
                    N_decimalPlace = N_decimalPlace == 0 ? 2 : N_decimalPlace;
                //     // string Pattern = "";
                // if (UserPattern != "")
                // {
                //     Pattern = " and Left(X_Pattern,Len(@UserPattern))=@UserPattern ";
                //     Params.Add("@UserPattern",UserPattern);

                // }
                // else
                // {
                //     object HierarchyCount = dLayer.ExecuteScalar("select count(N_HierarchyID) from Sec_UserHierarchy where N_CompanyID="+nCompanyId,Params,connection);

                //     if(myFunctions.getIntVAL(HierarchyCount.ToString())>0)
                //     Pattern = " and N_CreatedUser=" + nUserID;
                // }



                    bool CheckClosedYear = Convert.ToBoolean(dLayer.ExecuteScalar("Select B_YearEndProcess From Acc_FnYear Where N_CompanyID=" + nCompanyId + " and N_FnYearID = " + nFnYearId, Params, connection));

                     if (screen == "Order")
                        criteria = "and MONTH(Cast(D_POrderDate as DateTime)) = MONTH(CURRENT_TIMESTAMP) and YEAR(D_POrderDate)= YEAR(CURRENT_TIMESTAMP)";
                    if (screen == "DelayedOrder")
                        criteria = "and MONTH(Cast(D_POrderDate as DateTime)) = MONTH(CURRENT_TIMESTAMP) and YEAR(D_POrderDate)= YEAR(CURRENT_TIMESTAMP) and ISNULL(N_Processed,0)=0 and (D_ExDelvDate < getDate())";

                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and ([Order No] like '%" + xSearchkey + "%' or Vendor like '%" + xSearchkey + "%' or Cast([Order Date] as VarChar) like '%" + xSearchkey + "%'or N_Amount like '%" + xSearchkey + "%' or X_Description like '%" + xSearchkey + "%' or X_ActionStatus like '%" + xSearchkey + "%')";

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_POrderID desc";
                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "orderNo":
                                xSortBy = "N_POrderID " + xSortBy.Split(" ")[1];
                                break;
                            case "orderDate":
                                xSortBy = "Cast([Order Date] as DateTime ) " + xSortBy.Split(" ")[1];
                                break;
                            case "n_Amount":
                                xSortBy = "Cast(REPLACE(n_Amount,',','') as Numeric(10,"+N_decimalPlace+")) " + xSortBy.Split(" ")[1];
                                break;
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }
                    if (CheckClosedYear == false)
                    {
                        if (bAllBranchData == true)
                        {
                            Searchkey = Searchkey + " and  N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " ";
                        }
                        else
                        {
                            Searchkey = Searchkey + " and  N_CompanyID=" + nCompanyId + " and N_BranchID=" + nBranchID + " and N_FnYearID=" + nFnYearId + " and B_YearEndProcess =0";
                        }
                    }
                    else
                    {
                        if (bAllBranchData == true)
                        {
                            Searchkey = Searchkey + " and  N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " ";
                        }
                        else
                        {
                            Searchkey = Searchkey + " and  N_CompanyID=" + nCompanyId + " and N_BranchID=" + nBranchID + " and N_FnYearID=" + nFnYearId + " and B_YearEndProcess =0";
                        }
                    }

                    if (Count == 0)
                        sqlCommandText = "select  top(" + nSizeperpage + ") * from vw_InvPurchaseOrderNo_Search_Cloud where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=@p7 "  + criteria+ Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select  top(" + nSizeperpage + ") * from vw_InvPurchaseOrderNo_Search_Cloud where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=@p7 "  + criteria+ Searchkey + " and N_POrderID not in(select top(" + Count + ") N_POrderID from vw_InvPurchaseOrderNo_Search_Cloud where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=@p7  " +criteria+ xSortBy + " ) " + xSortBy;
                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearId);
                    Params.Add("@p7", nFormID);
                 
                    SortedList OutPut = new SortedList();


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count,sum(Cast(REPLACE(n_Amount,',','') as Numeric(10,"+N_decimalPlace+")) ) as TotalAmount from vw_InvPurchaseOrderNo_Search_Cloud where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=@p7 " +criteria+ Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    string TotalSum = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                        TotalSum = drow["TotalAmount"].ToString();
                    }
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    OutPut.Add("TotalSum", TotalSum);

                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Success(OutPut));
                        //return Ok(api.Warning(OutPut));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }

                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }


        [HttpGet("itemList")]
        public ActionResult GetPurchaseOrderList(int nLocationID, string type, string query, int PageSize, int Page)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            string Feilds = "";
            string X_Crieteria = "";
            string X_VisibleFieldList = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Params.Add("@Type", type);
                    Params.Add("@CompanyID", nCompanyId);
                    Params.Add("@LocationID", nLocationID);
                    Params.Add("@PSize", PageSize);
                    Params.Add("@Offset", Page);

                    // string pageQry = "DECLARE @PageSize INT, @Page INT Select @PageSize=@PSize,@Page=@Offset;WITH PageNumbers AS(Select ROW_NUMBER() OVER(ORDER BY N_ItemID) RowNo,";
                    // string pageQryEnd = ") SELECT * FROM    PageNumbers WHERE   RowNo BETWEEN((@Page -1) *@PageSize + 1)  AND(@Page * @PageSize)";

                    int N_POTypeID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select ISNULL(N_TypeId,0) From Gen_Defaults Where X_TypeName=@Type and N_DefaultId=36", Params, connection).ToString());
                    X_VisibleFieldList = myFunctions.ReturnSettings("65", "Item Search List", "X_Value", myFunctions.getIntVAL(nCompanyId.ToString()), dLayer, connection);
                    if (N_POTypeID == 121)
                    {
                        Feilds = "N_CompanyID,N_ItemID,[Item Class],B_Inactive,N_WarehouseID,N_BranchID,[Item Code],N_ItemTypeID";
                        X_Crieteria = "N_CompanyID=@CompanyID and B_Inactive=0 and [Item Code]<>'001' and ([Item Class]='Stock Item' Or [Item Class]='Non Stock Item' Or [Item Class]='Expense Item' Or [Item Class]='Assembly Item' ) and N_WarehouseID=@LocationID and N_ItemTypeID<>1";
                    }
                    else if (N_POTypeID == 122)
                    {
                        Feilds = "N_CompanyID,N_ItemID,[Item Class],B_Inactive,N_WarehouseID,N_BranchID,[Item Code],N_ItemTypeID";
                        X_Crieteria = "N_CompanyID=@CompanyID and B_Inactive=0 and [Item Code]<>'001' and ([Item Class]='Stock Item' Or [Item Class]='Non Stock Item' Or [Item Class]='Expense Item' Or [Item Class]='Assembly Item' ) and N_WarehouseID=@LocationID and N_ItemTypeID=1";
                    }

                    sqlCommandText = "select " + Feilds + "," + X_VisibleFieldList + " from vw_ItemDisplay where " + X_Crieteria + " Order by [Item Code]";
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }

                dt = api.Format(dt);
                return Ok(api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
        [HttpGet("listDetails")]
        public ActionResult GetPurchaseOrderDetails(int nCompanyId, string xPOrderId, int nFnYearId, string nLocationID, string xPRSNo, bool bAllBranchData, int nBranchID,int nQuotationID,int nVendorID,int nSalesOrderId)
        {
              if (xPOrderId != null)
                xPOrderId = xPOrderId.Replace("%2F", "/");
            bool B_PRSVisible = false;
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            DataTable DataTable = new DataTable();
            DataTable RentalSchedule = new DataTable();
            if (xPRSNo == null) xPRSNo = "";
            if (xPOrderId == null) xPOrderId = "";
            string Mastersql = "";
            string RentalScheduleSql = "";

         


           
           if (bAllBranchData == true)
            {
               
                if(nSalesOrderId>0)
                    Mastersql = "SELECT * from vw_RentalOrderMasterToPurchaseRO Where N_CompanyID=@p1  and N_SalesOrderId=@p8";
                else
                    Mastersql = "SELECT * from vw_Inv_PurchaseOrderMaster Where N_CompanyID=@p1 and N_FnYearID=@p2 and X_POrderNo=@p3";
            }
            else
            {
                if(nSalesOrderId>0)
                    Mastersql = "SELECT * from vw_RentalOrderMasterToPurchaseRO Where N_CompanyID=@p1  and N_SalesOrderId=@p8 and N_BranchID=@nBranchID";
                else
                    Mastersql = "SELECT * from vw_Inv_PurchaseOrderMaster Where N_CompanyID=@p1 and X_POrderNo=@p3 and N_BranchID=@nBranchID and N_FnYearID=@p2";

                Params.Add("@nBranchID", nBranchID);
            }

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", xPOrderId);   
            Params.Add("@p8", nSalesOrderId); 

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    int N_POrderID = 0 ;
                    if(MasterTable.Rows.Count>0)
                    { 
                        N_POrderID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_POrderID"].ToString());
                        object InPurchase = dLayer.ExecuteScalar("select 1 from Inv_Purchase where N_CompanyID=" + nCompanyId + " and N_POrderID=" + N_POrderID, Params, connection);
                        if (InPurchase != null)
                            MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "TxnStatus", typeof(string), "Invoice Processed");
                    }

                    MasterTable = api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                    //PurchaseOrder Details


                    string DetailSql = "";

                    int nUserID = myFunctions.GetUserID(User);
                    bool MaterailRequestVisible = myFunctions.CheckPermission(nCompanyId, 556, "Admin", "X_UserCategory", dLayer, connection);
                    //bool PurchaseRequestVisible = myFunctions.CheckPermission(nCompanyId, 1049, "Admin", "X_UserCategory", dLayer, connection);
                     bool PurchaseRequestVisible = false;
                     if(nVendorID>0 && nQuotationID>0){
                      PurchaseRequestVisible = Convert.ToBoolean(dLayer.ExecuteScalar("Select ISNULL(B_Visible,0) From vw_userPrevileges where N_MenuID=1049 and N_UserCategoryID  in(select N_UserCategoryID from Sec_User where N_UserID="+myFunctions.GetUserID(User)+") and N_CompanyID=" + nCompanyId, Params, connection));
                
                     }
                     
                    if (MaterailRequestVisible || PurchaseRequestVisible)
                    {
                        B_PRSVisible = true;
                        DataColumn prsCol = new DataColumn("B_PRSVisible", typeof(System.Boolean));
                        prsCol.DefaultValue = B_PRSVisible;
                        DataTable.Columns.Add(prsCol);
                    }

                    if(nSalesOrderId>0)
                    {

                     DetailSql = "select * from vw_RentalOrderDetailsToRPO where N_CompanyID=@p1  and N_SalesOrderId=@p8 and N_ItemTypeID in (9)";

                    RentalScheduleSql = "SELECT * FROM  vw_RentalScheduleItems  Where N_CompanyID="+ nCompanyId +" and N_TransID="+ nSalesOrderId +" and N_FormID=1571";
                    RentalSchedule = dLayer.ExecuteDataTable(RentalScheduleSql, Params, connection);
                    RentalSchedule = api.Format(RentalSchedule, "RentalSchedule");

                    }
                    else
                    {

                   
                    if (B_PRSVisible)
                        if (xPRSNo != "")
                        {
                            DetailSql = "Select *,dbo.SP_GenGetStock(vw_InvPurchaseOrderDetailsForPRS.N_ItemID,@p4,'','Location') As N_Stock,dbo.SP_Cost(vw_InvPurchaseOrderDetailsForPRS.N_ItemID,vw_InvPurchaseOrderDetailsForPRS.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseOrderDetailsForPRS.N_ItemID,vw_InvPurchaseOrderDetailsForPRS.N_CompanyID) As N_UnitSPrice from vw_InvPurchaseOrderDetailsForPRS Where N_CompanyID=@p1 and (X_PRSNo In (Select X_PRSNo from vw_InvPurchaseOrderDetailsForPRS Where N_POrderID = @p5)  or X_PRSNo In (Select 0 from vw_InvPurchaseOrderDetailsForPRS Where N_POrderID = @p5)) and  (N_POrderID =@p5 OR N_POrderID IS Null)";
                            Params.Add("@p4", nLocationID);
                            Params.Add("@p5", N_POrderID);
                        }
                        else if(nVendorID>0 && nQuotationID>0){
                                DetailSql = "Select *,dbo.SP_GenGetStock(vw_RFQToPODetails.N_ItemID,@p4,'','Location') As N_Stock,dbo.SP_Cost(vw_RFQToPODetails.N_ItemID,vw_RFQToPODetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_RFQToPODetails.N_ItemID,vw_RFQToPODetails.N_CompanyID) As N_UnitSPrice from vw_RFQToPODetails Where N_CompanyID=@p1 and N_VendorID=@p5 and N_QuotationID=@p6";
                                Params.Add("@p6", nQuotationID);
                                Params.Add("@p5", nVendorID);
                                Params.Add("@p4", nLocationID);
                            

                        }else
                        {
                            if (bAllBranchData == true)
                            {
                                DetailSql = "Select *,dbo.SP_GenGetStock(vw_InvPurchaseOrderDetails.N_ItemID,@p4,'','Location') As N_Stock,dbo.SP_Cost(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID) As N_UnitSPrice from vw_InvPurchaseOrderDetails Where N_CompanyID=@p1 and N_POrderID=@p5";
                                Params.Add("@p4", nLocationID);
                                Params.Add("@p5", N_POrderID);
                            }
                            else
                            {
                                DetailSql = "Select *,dbo.SP_GenGetStock(vw_InvPurchaseOrderDetails.N_ItemID,@p4,'','Location') As N_Stock,dbo.SP_Cost(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID) As N_UnitSPrice from vw_InvPurchaseOrderDetails Where N_CompanyID=@p1 and N_POrderID=@p5 and N_BranchID=@p6";
                                Params.Add("@p4", nLocationID);
                                Params.Add("@p5", N_POrderID);
                                Params.Add("@p6", nBranchID);
                            }
                        }
                    else
                    {

                        if (bAllBranchData == true)

                        {
                            DetailSql = "Select *,dbo.SP_GenGetStock(vw_InvPurchaseOrderDetails.N_ItemID,@p4,'','Location') As N_Stock,dbo.SP_Cost(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID) As N_UnitSPrice from vw_InvPurchaseOrderDetails Where N_CompanyID=@p1 and N_POrderID=@p5";
                            Params.Add("@p4", nLocationID);
                            Params.Add("@p5", N_POrderID);
                        }
                        else
                        {
                            DetailSql = "Select *,dbo.SP_GenGetStock(vw_InvPurchaseOrderDetails.N_ItemID,@p4,'','Location') As N_Stock,dbo.SP_Cost(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID) As N_UnitSPrice from vw_InvPurchaseOrderDetails Where N_CompanyID=@p1 and N_POrderID=@p5 and N_BranchID=@p6";
                            Params.Add("@p4", nLocationID);
                            Params.Add("@p5", N_POrderID);
                            Params.Add("@p6", nBranchID);
                        }
                    }
                    //DetailSql="Select * from Inv_PurchaseOrderDetails Where N_CompanyID=@p1 and N_POrderID=@p5";
                    }


                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = api.Format(DetailTable, "Details");

                    bool InvoiceProcessed=false;
                    if (MasterTable.Rows.Count > 0)
                    {
                        if(myFunctions.getIntVAL(MasterTable.Rows[0]["N_Processed"].ToString())==1)
                        {
                            object InvoiceNotProcessed=false;
                            for(int i=0;i<DetailTable.Rows.Count;i++)
                            {
                                Object POQty=dLayer.ExecuteScalar("select SUM(N_Qty) from Inv_PurchaseOrderdetails where n_porderid="+N_POrderID+" and N_PorderDetailsID="+myFunctions.getIntVAL(DetailTable.Rows[i]["N_PorderDetailsID"].ToString())+" and N_CompanyID="+nCompanyId, Params, connection);
                                Object InvQty=dLayer.ExecuteScalar("select SUM(N_Qty) from Inv_PurchaseDetails where n_porderid="+N_POrderID+" and N_POrderDetailsID="+myFunctions.getIntVAL(DetailTable.Rows[i]["N_PorderDetailsID"].ToString())+" and N_CompanyID="+nCompanyId, Params, connection);
                                if(POQty!=null && InvQty!=null)
                                {
                                    if(myFunctions.getVAL(POQty.ToString())!= myFunctions.getVAL(InvQty.ToString()))
                                    {
                                    // InvoiceNotProcessed = true;
                                        MasterTable.Rows[0]["N_Processed"]=0;
                                        InvoiceProcessed=false;
                                        break;
                                    }
                                    else
                                    {
                                        MasterTable.Rows[0]["N_Processed"]=1;
                                        InvoiceProcessed=true;
                                    }
                                }
                                else
                                {
                                    MasterTable.Rows[0]["N_Processed"]=1;
                                    InvoiceProcessed=true;
                                }
                            }
                        }

                        if(!InvoiceProcessed)
                        {
                            object GRNNotProcessed=false;
                            for(int i=0;i<DetailTable.Rows.Count;i++)
                            {
                                Object POQty=dLayer.ExecuteScalar("select SUM(N_Qty) from Inv_PurchaseOrderdetails where n_porderid="+N_POrderID+" and N_PorderDetailsID="+myFunctions.getIntVAL(DetailTable.Rows[i]["N_PorderDetailsID"].ToString())+" and N_CompanyID="+nCompanyId, Params, connection);
                                Object GRNQty=dLayer.ExecuteScalar("select SUM(N_Qty) from Inv_MRNDetails where N_PONo="+N_POrderID+" and N_PorderDetailsID="+myFunctions.getIntVAL(DetailTable.Rows[i]["N_PorderDetailsID"].ToString())+" and N_CompanyID="+nCompanyId, Params, connection);
                                if(POQty!=null && GRNQty!=null)
                                {
                                    if(myFunctions.getVAL(POQty.ToString())!= myFunctions.getVAL(GRNQty.ToString()))
                                    {
                                        //GRNNotProcessed = true;
                                        MasterTable.Rows[0]["N_Processed"]=0;
                                        break;
                                    }
                                    else
                                    {
                                        MasterTable.Rows[0]["N_Processed"]=1;
                                    }
                                }
                                else
                                {
                                    MasterTable.Rows[0]["N_Processed"]=1;
                                }
                            }
                        }
                    }

                    Object PoCount=dLayer.ExecuteScalar("select count(1) from Inv_MRNDetails where N_PONo="+N_POrderID+" and N_CompanyID="+nCompanyId, Params, connection);
                    int nPoCount = myFunctions.getIntVAL(PoCount.ToString());
                    if (nPoCount > 0)
                    {
                       
                        if (MasterTable.Rows.Count > 0)
                        {
                            MasterTable.Columns.Add("b_GRNProcessed");
                            MasterTable.Rows[0]["b_GRNProcessed"]=true;
                            
                          
                        }


                    }  

                    Object VTSCount=dLayer.ExecuteScalar("select count(*) from Inv_ServiceTimesheet where N_POID="+N_POrderID+" and N_CompanyID="+nCompanyId, Params, connection);
                    int nVTSCount = myFunctions.getIntVAL(VTSCount.ToString());
                    if (nVTSCount > 0)
                    {
                        if (MasterTable.Rows.Count > 0)
                        {
                            MasterTable.Columns.Add("b_VTSProcessed");
                            MasterTable.Rows[0]["b_VTSProcessed"]=true;
                        }
                    }      
                    // MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "GRNNotProcessed", typeof(bool),GRNNotProcessed);
                    // MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "InvoiceNotProcessed", typeof(bool),InvoiceNotProcessed);

                     DataTable Attachments =new DataTable();
                    if(MasterTable.Rows.Count>0 && MasterTable.Columns.Contains("N_VendorID"))
                    {

                   
                    Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(MasterTable.Rows[0]["N_VendorID"].ToString()), myFunctions.getIntVAL(MasterTable.Rows[0]["N_POrderID"].ToString()), this.FormID, myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString()), User, connection);
                    Attachments = api.Format(Attachments, "attachments");
                    }

                    if(nSalesOrderId>0)
                    {
                        RentalScheduleSql = "SELECT * FROM  vw_RentalScheduleItems  Where N_CompanyID="+ nCompanyId +" and N_TransID="+ nSalesOrderId +" and N_FormID=1571 and N_ItemTypeID=9";
                        RentalSchedule = dLayer.ExecuteDataTable(RentalScheduleSql, Params, connection);
                        RentalSchedule = api.Format(RentalSchedule, "RentalSchedule");
                    } else {
                        RentalScheduleSql = "SELECT * FROM  vw_RentalScheduleItems  Where N_CompanyID="+ nCompanyId +" and N_TransID="+ N_POrderID +" and N_FormID=1586 and N_ItemTypeID=9";
                        RentalSchedule = dLayer.ExecuteDataTable(RentalScheduleSql, Params, connection);
                        RentalSchedule = api.Format(RentalSchedule, "RentalSchedule");
                    };

                    dt.Tables.Add(Attachments);
                    dt.Tables.Add(DetailTable);
                    dt.Tables.Add(RentalSchedule);
                }
                return Ok(api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
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
                DataTable DetailsToImport;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                DataTable Attachment = ds.Tables["attachments"];
                DetailsToImport = ds.Tables["detailsImport"];
                DataTable rentalItem = ds.Tables["segmentTable"];
                bool B_isImport = false;
                SortedList Params = new SortedList();
                String xButtonAction="";
                int N_POrderID = 0; var X_POrderNo = "";
                int nDivisionID = 0;
              

                if (ds.Tables.Contains("detailsImport"))
                    B_isImport = true;
       
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;


                    // Auto Gen
                    string PorderNo = "";
                    if (MasterTable.Rows.Count > 0)
                    {

                    }
                    X_POrderNo = MasterTable.Rows[0]["x_POrderNo"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    int nCompanyId = myFunctions.getIntVAL(Master["n_CompanyId"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(Master["N_FnYearID"].ToString());
                    if (MasterTable.Columns.Contains("n_DivisionID"))
                    {
                    nDivisionID=myFunctions.getIntVAL(Master["n_DivisionID"].ToString());
                   }
                    N_POrderID = myFunctions.getIntVAL(Master["n_POrderID"].ToString());
                    if (N_POrderID > 0)
                    {
                        if (CheckProcessed(N_POrderID))
                            return Ok(api.Error(User, "Transaction Started!"));
                    }
                    int N_VendorID = myFunctions.getIntVAL(Master["n_VendorID"].ToString());
                    if (myFunctions.checkIsNull(Master, "n_POTypeID"))
                        MasterTable.Rows[0]["n_POTypeID"] = 174;

                    if (myFunctions.checkIsNull(Master, "n_POType"))
                        MasterTable.Rows[0]["n_POType"] = 121;

                    transaction = connection.BeginTransaction();
                  if (!myFunctions.CheckActiveYearTransaction(nCompanyId,myFunctions.getIntVAL(Master["n_FnYearId"].ToString()), DateTime.ParseExact(MasterTable.Rows[0]["D_PorderDate"].ToString(), "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture), dLayer, connection, transaction))
                  {
                   object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID="+nCompanyId+" and convert(date ,'" + MasterTable.Rows[0]["D_PorderDate"].ToString() + "') between D_Start and D_End", connection, transaction);
                  if (DiffFnYearID != null)
                   {
                     Master["n_FnYearId"] = DiffFnYearID.ToString();
                    MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
                    N_FnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());

                           SortedList QueryParams = new SortedList();
                            QueryParams["@nFnYearID"] = N_FnYearID;
                            QueryParams["@nCompanyId"] = nCompanyId;
                            QueryParams["@N_VendorID"] = N_VendorID;
                            
                              SortedList PostingParam = new SortedList();
                              PostingParam.Add("N_PartyID", N_VendorID);
                              PostingParam.Add("N_FnyearID", N_FnYearID);
                              PostingParam.Add("N_CompanyID", nCompanyId);
                              PostingParam.Add("X_Type", "vendor");


                             object vendorCount = dLayer.ExecuteScalar("Select count(*) From Inv_Vendor where N_FnYearID=@nFnYearID and N_CompanyID=@nCompanyID and N_VendorID=@N_VendorID", QueryParams, connection, transaction);
                      
                               if(myFunctions.getIntVAL(vendorCount.ToString())==0){
                                try 
                                  {
                                     dLayer.ExecuteNonQueryPro("SP_CratePartyBackYear", PostingParam, connection, transaction);
                                  }
                                  catch (Exception ex)
                                  {
                                    transaction.Rollback();
                                     throw ex;
                                  }
                                  }
                  
                  }
                  else
                  {
                    // transaction.Rollback();
                    // return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
                    // Result.Add("b_IsCompleted", 0);
                    // Result.Add("x_Msg", "Transaction date must be in the active Financial Year.");
                      return Ok(api.Error(User, "Transaction date must be in the active Financial Year."));
                   }
                }
                 object B_YearEndProcess=dLayer.ExecuteScalar("Select B_YearEndProcess from Acc_FnYear Where N_CompanyID="+nCompanyId+" and convert(date ,'" + MasterTable.Rows[0]["D_POrderDate"].ToString() + "') between D_Start and D_End", connection, transaction);
                 if(myFunctions.getBoolVAL(B_YearEndProcess.ToString()))
                 {
                     return Ok(api.Error(User, "Year Closed"));
                 }





                    if (X_POrderNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyId);
                        Params.Add("N_YearID", Master["n_FnYearId"].ToString());
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("N_BranchID",Master["n_BranchID"].ToString());

                        X_POrderNo = dLayer.GetAutoNumber("Inv_PurchaseOrder", "x_POrderNo", Params, connection, transaction);
                        xButtonAction="Insert"; 
                        if (X_POrderNo == "") { transaction.Rollback(); return Ok(api.Warning("Unable to generate Quotation Number")); }
                        MasterTable.Rows[0]["x_POrderNo"] = X_POrderNo;
                    }
                    else
                          X_POrderNo = MasterTable.Rows[0]["x_POrderNo"].ToString();
                    {
                        SortedList AdvParams = new SortedList();
                        AdvParams.Add("@companyId", Master["n_CompanyId"].ToString());
                        AdvParams.Add("@PorderId", Master["n_POrderID"].ToString());
                        object AdvancePRProcessed = dLayer.ExecuteScalar("Select COUNT(N_TransID) From Inv_PaymentRequest Where  N_CompanyID=@companyId and N_TransID=@PorderId and N_FormID=82", AdvParams, connection, transaction);
                        if (AdvancePRProcessed != null)
                        {
                            if (myFunctions.getIntVAL(AdvancePRProcessed.ToString()) > 0)
                            {
                                transaction.Rollback();
                                return Ok(api.Success("Payment Request Processed"));
                            }
                        }


                        if (N_POrderID > 0)
                        {

                            bool B_PRSVisible = false;
                            bool MaterailRequestVisible = myFunctions.CheckPermission(nCompanyId, 556, "Administrator", "X_UserCategory", dLayer, connection, transaction);
                            bool PurchaseRequestVisible = myFunctions.CheckPermission(nCompanyId, 1049, "Administrator", "X_UserCategory", dLayer, connection, transaction);
                             xButtonAction="Update"; 
                            if (MaterailRequestVisible || PurchaseRequestVisible)
                                B_PRSVisible = true;

                            if (B_PRSVisible)
                            {
                                // if (txtPRSNo.Text != "")
                                // {
                                //     for (int i = 0; i < flxPurchase.Rows; i++)
                                //     {
                                //         if (flxPurchase.get_TextMatrix(i, mcPay) != "P") continue;
                                //         if (flxPurchase.get_TextMatrix(i, mcPrsID) == "") continue;
                                //         if (flxPurchase.get_TextMatrix(i, mcTransType) == "PRS")
                                //         {
                                //             dba.ExecuteNonQuery("Update Inv_PRSDetails set N_Processed=0 Where N_PRSID=" + flxPurchase.get_TextMatrix(i, mcPrsID) + " and   N_PRSDetailsID=" + flxPurchase.get_TextMatrix(i, mcPrsDeatilsID) + " and N_ItemID=" + flxPurchase.get_TextMatrix(i, mcItemID) + "", "TEXT", new DataTable());
                                //             dba.ExecuteNonQuery("Update Inv_PRS set N_Processed=0 Where N_PRSID=" + flxPurchase.get_TextMatrix(i, mcPrsID) + " and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());
                                //         }
                                //     }
                                // }
                            }
                            // if (B_RFQ)
                            // {
                            //     if (txtPRSNo.Text != "")
                            //     {
                            //         for (int i = 0; i < flxPurchase.Rows; i++)
                            //         {
                            //             if (flxPurchase.get_TextMatrix(i, mcPay) != "P") continue;
                            //             if (flxPurchase.get_TextMatrix(i, mcPrsID) == "") continue;
                            //             if (flxPurchase.get_TextMatrix(i, mcTransType) == "RFQ")
                            //                 dba.ExecuteNonQuery("Update Inv_VendorRequestDetails set N_Processed=0 Where N_QuotationID=" + flxPurchase.get_TextMatrix(i, mcPrsID) + " and   N_QuotationDetailsID=" + flxPurchase.get_TextMatrix(i, mcPrsDeatilsID) + " and N_ItemID=" + flxPurchase.get_TextMatrix(i, mcItemID) + "", "TEXT", new DataTable());

                            //         }
                            //     }
                            // }

                            SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyId},
                                {"X_TransType","Purchase Order"},
                                {"N_VoucherID",N_POrderID}};
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                        }
                    }


                    N_POrderID = dLayer.SaveData("Inv_PurchaseOrder", "n_POrderID", MasterTable, connection, transaction);
                    if (N_POrderID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Error"));
                    }

                    if (B_isImport)
                    {
                        foreach (DataColumn col in DetailsToImport.Columns)
                        {
                            col.ColumnName = col.ColumnName.Replace(" ", "_");
                            col.ColumnName = col.ColumnName.Replace("*", "");
                            col.ColumnName = col.ColumnName.Replace("/", "_");
                        }

                        DetailsToImport.Columns.Add("N_MasterID");
                        DetailsToImport.Columns.Add("X_Type");
                        DetailsToImport.Columns.Add("N_CompanyID");
                        DetailsToImport.Columns.Add("PkeyID");
                        foreach (DataRow dtRow in DetailsToImport.Rows)
                        {
                            dtRow["N_MasterID"] = N_POrderID;
                            dtRow["N_CompanyID"] = nCompanyId;
                            dtRow["PkeyID"] = 0;
                        }

                        dLayer.ExecuteNonQuery("delete from Mig_PurchaseOrder ", connection, transaction);
                        dLayer.SaveData("Mig_PurchaseOrder", "PkeyID", "", "", DetailsToImport, connection, transaction);

                        SortedList ProParam = new SortedList();
                        ProParam.Add("N_CompanyID", nCompanyId);
                        ProParam.Add("N_PKeyID", N_POrderID);

                        SortedList ValidationParam = new SortedList();
                        ValidationParam.Add("N_CompanyID", myFunctions.getIntVAL(Master["n_CompanyId"].ToString()));
                        ValidationParam.Add("N_FnYearID", Master["n_FnYearId"].ToString());
                        ValidationParam.Add("X_Type", "purchase order");
                            try
                            {
                                dLayer.ExecuteNonQueryPro("SP_SetupData_Validation", ValidationParam, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(api.Error(User, ex));
                            }
                         
                        ProParam.Add("X_Type", "purchase order");
                        DetailTable = dLayer.ExecuteDataTablePro("SP_ScreenDataImport", ProParam, connection,transaction);
                    }

                    if(nDivisionID>0)
                    {
                    if (DetailTable.Rows.Count > 0)
                    {
                     object xLevelsql = dLayer.ExecuteScalar("select X_LevelPattern from Inv_DivisionMaster where N_CompanyID=" + nCompanyId + " and N_DivisionID=" + nDivisionID + " and N_GroupID=0", Params, connection,transaction);
                      
                       if (xLevelsql != null && xLevelsql.ToString() != "")
                        {
                        for (int j = 0; j < DetailTable.Rows.Count; j++)
                        {
                            object xLevelPattern = dLayer.ExecuteScalar("SELECT  Inv_DivisionMaster.X_LevelPattern FROM         Inv_DivisionMaster LEFT OUTER JOIN    Inv_ItemCategory ON Inv_DivisionMaster.N_DivisionID = Inv_ItemCategory.N_DivisionID AND Inv_DivisionMaster.N_CompanyID = Inv_ItemCategory.N_CompanyID RIGHT OUTER JOIN "+
                            "Inv_ItemMaster ON Inv_ItemCategory.N_CompanyID = Inv_ItemMaster.N_CompanyID AND Inv_ItemCategory.N_CategoryID = Inv_ItemMaster.N_CategoryID  where Inv_ItemMaster.N_ItemID="+ DetailTable.Rows[j]["N_ItemID"]+" and Inv_ItemMaster.N_CompanyID="+nCompanyId+"", Params, connection,transaction);
                            if (xLevelsql.ToString() != xLevelPattern.ToString().Substring(0, 3))
                             {
                                  transaction.Rollback();
                                  return Ok(api.Error(User, "Unable To save!...Division Mismatch"));
                             }
                           
                        }
                        }
                    }}
                    
                    // for (int j = 0; j < DetailTable.Rows.Count; j++)
                    // {
                    //     int UnitID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_ItemUnitID from inv_itemunit where N_ItemID=" + myFunctions.getIntVAL(DetailTable.Rows[j]["N_ItemID"].ToString()) + " and N_CompanyID=" + myFunctions.getIntVAL(DetailTable.Rows[j]["N_CompanyID"].ToString()) + " and X_ItemUnit='" + DetailTable.Rows[j]["X_ItemUnit"].ToString() + "'", connection, transaction).ToString());
                    //     DetailTable.Rows[j]["n_POrderID"] = N_POrderID;
                    //     DetailTable.Rows[j]["N_ItemUnitID"] = UnitID;
                    // }
                    DetailTable.Columns.Remove("X_ItemUnit");
                    // int N_PurchaseOrderDetailId = dLayer.SaveData("Inv_PurchaseOrderDetails", "n_POrderDetailsID", DetailTable, connection, transaction);

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        // int UnitID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_ItemUnitID from inv_itemunit where N_ItemID=" + myFunctions.getIntVAL(DetailTable.Rows[j]["N_ItemID"].ToString()) + " and N_CompanyID=" + myFunctions.getIntVAL(DetailTable.Rows[j]["N_CompanyID"].ToString()) + " and X_ItemUnit='" + DetailTable.Rows[j]["X_ItemUnit"].ToString() + "'", connection, transaction).ToString());
                        
                        DetailTable.Rows[j]["N_POrderID"] = N_POrderID;
                        // DetailTable.Rows[j]["N_ItemUnitID"] = UnitID;
                        // DataRow row = DetailTable.Rows[j];
                        // row.Table.Columns.Remove("X_ItemUnit");


                        int N_PurchaseOrderDetailId = dLayer.SaveDataWithIndex("Inv_PurchaseOrderDetails", "n_POrderDetailsID", "", "", j, DetailTable, connection, transaction);

                        if (N_PurchaseOrderDetailId > 0)
                        {
                            for (int k = 0; k < rentalItem.Rows.Count; k++)
                            {
                                if (myFunctions.getIntVAL(rentalItem.Rows[k]["rowID"].ToString()) == j)
                                {

                                    rentalItem.Rows[k]["n_TransID"] = N_POrderID;
                                    rentalItem.Rows[k]["n_TransDetailsID"] = N_PurchaseOrderDetailId;

                                    rentalItem.AcceptChanges();
                                }
                                rentalItem.AcceptChanges();
                            }
                            rentalItem.AcceptChanges();
                        }
                        DetailTable.AcceptChanges();
                    }

                    if (rentalItem.Columns.Contains("rowID"))
                    rentalItem.Columns.Remove("rowID");
                    rentalItem.AcceptChanges();

                    if(rentalItem.Rows.Count > 0)
                    {
                        if (N_POrderID > 0)
                        {
                            int FormID = myFunctions.getIntVAL(rentalItem.Rows[0]["n_FormID"].ToString());
                            dLayer.ExecuteScalar("delete from Inv_RentalSchedule where N_TransID=" + N_POrderID.ToString() + " and N_FormID="+ FormID + " and N_CompanyID=" + nCompanyId, connection, transaction);
                        }
                        dLayer.SaveData("Inv_RentalSchedule", "N_ScheduleID", rentalItem, connection, transaction);

                    }

                    if (N_POrderID > 0)
                    {
                        // if(!myFunctions.UpdateTxnStatus(nCompanyId,N_POrderID,82,false,dLayer,connection,transaction))
                        // {
                        //     transaction.Rollback();
                        //     return Ok(api.Error(User, "Unable To Update Txn Status"));
                        // }
                        if (myFunctions.getIntVAL(MasterTable.Rows[0]["n_FormID"].ToString()) == 1586)
                        {
                            if(!myFunctions.UpdateTxnStatus(nCompanyId,N_POrderID,1586,false,dLayer,connection,transaction))
                            {
                                transaction.Rollback();
                                return Ok(api.Error(User, "Unable To Update Txn Status"));
                            }
                        }
                        else
                        {
                            if(!myFunctions.UpdateTxnStatus(nCompanyId,N_POrderID,82,false,dLayer,connection,transaction))
                            {
                                transaction.Rollback();
                                return Ok(api.Error(User, "Unable To Update Txn Status"));
                            }
                        }
                    }

                    if(N_POrderID > 0 && myFunctions.getIntVAL(MasterTable.Rows[0]["n_QuotationID"].ToString())>0){
                           dLayer.ExecuteScalar("Update Inv_RFQDecisionMaster Set N_PorderID = "+N_POrderID+" Where  N_QuotationID=" + myFunctions.getIntVAL(MasterTable.Rows[0]["n_QuotationID"].ToString()) + " and N_CompanyID=" + nCompanyId, connection, transaction);
                        }
                    
                 
                    SortedList VendorParams = new SortedList();
                    VendorParams.Add("@nVendorID", N_VendorID);
                    DataTable VendorInfo = dLayer.ExecuteDataTable("Select X_VendorCode,X_VendorName from Inv_Vendor where N_VendorID=@nVendorID", VendorParams, connection, transaction);
                    if (VendorInfo.Rows.Count > 0)
                    {
                        try
                        {
                            myAttachments.SaveAttachment(dLayer, Attachment, PorderNo, N_POrderID, VendorInfo.Rows[0]["X_VendorName"].ToString().Trim(), VendorInfo.Rows[0]["X_VendorCode"].ToString(), N_VendorID, "Vendor Document", User, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User, ex));
                        }
                    }
                         
                    // Activity Log
                   string ipAddress = "";
                   if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                  else
                       ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(N_FnYearID,N_POrderID,X_POrderNo,82,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                        

                    transaction.Commit();
                }
                SortedList Result = new SortedList();
                Result.Add("n_POrderID", N_POrderID);
                Result.Add("x_POrderNo", X_POrderNo);
                return Ok(api.Success(Result, "Purchase Order Saved"));
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

        private bool CheckProcessed(int nPOrderID)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                object AdvancePRProcessed = dLayer.ExecuteScalar("Select COUNT(N_TransID) From Inv_PaymentRequest Where  N_CompanyID=" + nCompanyId + " and N_TransID=" + nPOrderID + " and N_FormID=82", connection);
                if (AdvancePRProcessed != null)
                {
                    if (myFunctions.getIntVAL(AdvancePRProcessed.ToString()) > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPOrderID, int nBranchID, int nFnYearID,int FormID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nTransID", nPOrderID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    string Sql = "select N_POrderID,N_VendorID,X_POrderNo from Inv_PurchaseOrder where N_POrderID=@nTransID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID";
                     string xButtonAction="Delete";
                     string X_POrderNo="";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    int VendorID = myFunctions.getIntVAL(TransRow["N_VendorID"].ToString());

                    SqlTransaction transaction = connection.BeginTransaction();

                    var xUserCategory = myFunctions.GetUserCategory(User);// User.FindFirst(ClaimTypes.GroupSid)?.Value;
                    var nUserID = myFunctions.GetUserID(User);// User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    object objPurchaseProcessed = dLayer.ExecuteScalar("Select Isnull(N_PurchaseID,0) from Inv_Purchase where N_CompanyID=" + nCompanyID + " and N_POrderID=" + nPOrderID + " and B_IsSaveDraft = 0", connection, transaction);
                    if (objPurchaseProcessed == null)
                        objPurchaseProcessed = 0;

                    object objGRNProcessed = dLayer.ExecuteScalar("Select Isnull(N_MRNID,0) from Inv_MRN where N_CompanyID=" + nCompanyID + " and N_POrderID=" + nPOrderID + " and ISNULL(B_IsSaveDraft,0) = 0", connection, transaction);
                    if (objGRNProcessed == null)
                        objGRNProcessed = 0;
                
               // Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(myFunctions.getIntVAL( nFnYearID.ToString()),nPOrderID,TransRow["X_POrderNo"].ToString(),82,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);



                    if (myFunctions.getIntVAL(objPurchaseProcessed.ToString()) == 0 && myFunctions.getIntVAL(objGRNProcessed.ToString()) == 0)
                    {
                        SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType","PURCHASE ORDER"},
                                {"N_VoucherID",nPOrderID},
                                {"N_UserID",nUserID},
                                {"X_SystemName","WebRequest"},
                                {"N_BranchID",nBranchID}};
                        Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                        if (Results <= 0)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User, "Unable to delete Purchase Order"));
                        }
                        else
                        {
                            dLayer.ExecuteScalar("delete from Inv_RentalSchedule where N_TransID=" + nPOrderID.ToString() + " and N_FormID=1586 and N_CompanyID=" + nCompanyID, connection, transaction);
                            myAttachments.DeleteAttachment(dLayer, 1, nPOrderID, VendorID, nFnYearID, this.FormID, User, transaction, connection);

                            transaction.Commit();
                          if (FormID == 82)
                            {
                          return Ok(api.Success( " Purchase Order deleted"));
                            }

                       else if (FormID == 1586) 
                        {
                            return Ok(api.Success("Rental Purchase Order deleted Successfully"));
                        }
                            return Ok(api.Success("Purchase Order deleted"));

                        }
                    }
                    else
                    {
                        transaction.Rollback();
                        if (myFunctions.getIntVAL(objPurchaseProcessed.ToString()) > 0)
                            return Ok(api.Error(User, "Purchase invoice processed! Unable to delete"));
                        else if (myFunctions.getIntVAL(objGRNProcessed.ToString()) > 0)
                            return Ok(api.Error(User, "Goods Receive Note processed! Unable to delete"));
                        else
                            return Ok(api.Error(User, "Unable to delete!"));

                    }

                    //     Results = dLayer.DeleteData("Inv_PurchaseOrderDetails", "n_POrderID", nPOrderID, "", connection, transaction);
                    //     if (Results <= 0)
                    //     {
                    //         transaction.Rollback();
                    //         return Ok(api.Error(User,"Unable to delete PurchaseOrder"));
                    //     }
                    //     else
                    //     {
                    //         Results = dLayer.DeleteData("Inv_PurchaseOrder", "n_POrderID", nPOrderID, "", connection, transaction);
                    //     }


                    //     if (Results > 0)
                    //     {
                    //         transaction.Commit();
                    //         return Ok(api.Success("PurchaseOrder deleted"));
                    //     }
                    //     else
                    //     {
                    //         transaction.Rollback();
                    //     }
                    // }

                    // return Ok(api.Error(User,"Unable to Delete PurchaseOrder"));


                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }


        }

        [HttpGet("purchaseOrderNo")]
        public ActionResult GetInvPurchaseOrderNo( DateTime dDateFrom, DateTime dDateTo, int nFormID )
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                   
                    string sqlCommandText = "";

                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nFormID);
                    Params.Add("@p3", dDateFrom);
                    Params.Add("@p4", dDateTo);
    
                    sqlCommandText = "select * from vw_PurchaseOrderReceive	where N_CompanyID=@p1 and ((D_ReceiveDate<=@p3) or (D_ReceiveDate>=@p3 AND D_ReceiveDate<=@p4)) AND ISNULL(D_ReceiveReturnDate,@p3)>=@p3 and N_POrderID NOT IN (select N_POID from Inv_ServiceTimesheet where N_FormID=@p2 and N_CompanyID=@p1 and D_DateFrom=@p3 and D_DateTo=@p4)";

                    SortedList OutPut = new SortedList();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    OutPut.Add("Details", api.Format(dt));
                    return Ok(api.Success(OutPut));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
    }
}