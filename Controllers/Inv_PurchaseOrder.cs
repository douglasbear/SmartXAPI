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
        private readonly string connectionString;


        public Inv_PurchaseOrderController(IDataAccessLayer dl, IApiFunctions _api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            api = _api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");

        }


        [HttpGet("list")]
        public ActionResult GetPurchaseOrderList(int? nCompanyId, int nFnYearId,int nPage,int nSizeperpage)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
            string sqlCommandCount="";
            if(Count==0)
                sqlCommandText = "select  top("+ nSizeperpage +") * from vw_InvPurchaseOrderNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2";
            else
                sqlCommandText = "select  top("+ nSizeperpage +") * from vw_InvPurchaseOrderNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_POrderID not in(select top("+ Count +") N_POrderID from vw_InvPurchaseOrderNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2)";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from vw_InvPurchaseOrderNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details",api.Format(dt));
                    OutPut.Add("TotalCount",TotalCount);
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

                string pageQry = "DECLARE @PageSize INT, @Page INT Select @PageSize=@PSize,@Page=@Offset;WITH PageNumbers AS(Select ROW_NUMBER() OVER(ORDER BY N_ItemID) RowNo,";
                string pageQryEnd = ") SELECT * FROM    PageNumbers WHERE   RowNo BETWEEN((@Page -1) *@PageSize + 1)  AND(@Page * @PageSize)";

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
                    
                    sqlCommandText = "select "+Feilds+","+X_VisibleFieldList+" from vw_ItemDisplay where "+X_Crieteria + " Order by [Item Code]";
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }

                dt = api.Format(dt);
                return Ok(api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
        [HttpGet("listDetails")]
        public ActionResult GetPurchaseOrderDetails(int nCompanyId, string xPOrderId, int nFnYearId, string nLocationID, string xPRSNo, bool bAllBranchData, int nBranchID)
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
                Mastersql = "SELECT Inv_PurchaseOrder.*, Inv_Location.X_LocationName,Gen_Defaults.X_TypeName, Inv_Vendor.X_VendorName FROM Inv_PurchaseOrder INNER JOIN Inv_Vendor ON Inv_PurchaseOrder.N_CompanyID = Inv_Vendor.N_CompanyID AND Inv_PurchaseOrder.N_VendorID = Inv_Vendor.N_VendorID AND  Inv_PurchaseOrder.N_FnYearID = Inv_Vendor.N_FnYearID LEFT OUTER JOIN Gen_Defaults ON Inv_PurchaseOrder.N_POType = Gen_Defaults.N_TypeId LEFT OUTER JOIN Inv_Location ON Inv_PurchaseOrder.N_LocationID = Inv_Location.N_LocationID Where Inv_PurchaseOrder.N_CompanyID=@p1 and Inv_PurchaseOrder.N_FnYearID=@p2 and Inv_PurchaseOrder.X_POrderNo=@p3";
            }
            else
            {
                Mastersql = "SELECT Inv_PurchaseOrder.*, Inv_Location.X_LocationName,Gen_Defaults.X_TypeName, Inv_Vendor.X_VendorName FROM Inv_PurchaseOrder INNER JOIN Inv_Vendor ON Inv_PurchaseOrder.N_CompanyID = Inv_Vendor.N_CompanyID AND Inv_PurchaseOrder.N_VendorID = Inv_Vendor.N_VendorID AND  Inv_PurchaseOrder.N_FnYearID = Inv_Vendor.N_FnYearID LEFT OUTER JOIN Gen_Defaults ON Inv_PurchaseOrder.N_POType = Gen_Defaults.N_TypeId LEFT OUTER JOIN Inv_Location ON Inv_PurchaseOrder.N_LocationID = Inv_Location.N_LocationID Where Inv_PurchaseOrder.N_CompanyID=@p1 and Inv_PurchaseOrder.X_POrderNo=@p3 and Inv_PurchaseOrder.N_BranchID=@nBranchID and Inv_PurchaseOrder.N_FnYearID=@p2";
                Params.Add("@nBranchID", nBranchID);
            }

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", xPOrderId);

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);

                    MasterTable = api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                    //PurchaseOrder Details
                    int N_POrderID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_POrderID"].ToString());

                    string DetailSql = "";
                    bool MaterailRequestVisible = myFunctions.CheckPermission(nCompanyId, 556, "Administrator", dLayer, connection);
                    bool PurchaseRequestVisible = myFunctions.CheckPermission(nCompanyId, 1049, "Administrator", dLayer, connection);
                    if (MaterailRequestVisible || PurchaseRequestVisible)
                    {
                        B_PRSVisible = true;
                        DataColumn prsCol = new DataColumn("B_PRSVisible", typeof(System.Boolean));
                        prsCol.DefaultValue = B_PRSVisible;
                        DataTable.Columns.Add(prsCol);
                    }
                    if (B_PRSVisible)
                        if (xPRSNo != "")
                        {
                            DetailSql = "Select *,dbo.SP_GenGetStock(vw_InvPurchaseOrderDetailsForPRS.N_ItemID,@p4,'','Location') As N_Stock,dbo.SP_Cost(vw_InvPurchaseOrderDetailsForPRS.N_ItemID,vw_InvPurchaseOrderDetailsForPRS.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseOrderDetailsForPRS.N_ItemID,vw_InvPurchaseOrderDetailsForPRS.N_CompanyID) As N_UnitSPrice from vw_InvPurchaseOrderDetailsForPRS Where N_CompanyID=@p1 and (X_PRSNo In (Select X_PRSNo from vw_InvPurchaseOrderDetailsForPRS Where N_POrderID = @p5)  or X_PRSNo In (Select 0 from vw_InvPurchaseOrderDetailsForPRS Where N_POrderID = @p5)) and  (N_POrderID =@p5 OR N_POrderID IS Null)";
                            Params.Add("@p4", nLocationID);
                            Params.Add("@p5", N_POrderID);
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
                                DetailSql = "Select *,dbo.SP_GenGetStock(vw_InvPurchaseOrderDetails.N_ItemID,@p4,'','Location') As N_Stock,dbo.SP_Cost(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID) As N_UnitSPrice from vw_InvPurchaseOrderDetails Where N_CompanyID=@p1 and N_POrderID=nPOrderID and N_BranchID=@nBranchID";
                                Params.Add("@p4", nLocationID);
                                Params.Add("@p5", N_POrderID);
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
                            DetailSql = "Select *,dbo.SP_GenGetStock(vw_InvPurchaseOrderDetails.N_ItemID,@p4,'','Location') As N_Stock,dbo.SP_Cost(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID) As N_UnitSPrice from vw_InvPurchaseOrderDetails Where N_CompanyID=@p1 and N_POrderID=@p5 and N_BranchID=@nBranchID";
                            Params.Add("@p4", nLocationID);
                            Params.Add("@p5", N_POrderID);
                        }
                    }
                    //DetailSql="Select * from Inv_PurchaseOrderDetails Where N_CompanyID=@p1 and N_POrderID=@p5";



                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);
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
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                SortedList Params = new SortedList();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;


                    // Auto Gen
                    string PorderNo = "";
                    if (MasterTable.Rows.Count > 0)
                    {

                    }
                    var values = MasterTable.Rows[0]["x_POrderNo"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    int nCompanyId = myFunctions.getIntVAL(Master["n_CompanyId"].ToString());

                    int N_POrderID = myFunctions.getIntVAL(Master["n_POrderID"].ToString());

                    if (myFunctions.checkIsNull(Master, "n_POTypeID"))
                        MasterTable.Rows[0]["n_POTypeID"] = 174;

                    if (myFunctions.checkIsNull(Master, "n_POType"))
                        MasterTable.Rows[0]["n_POType"] = 121;

                    transaction = connection.BeginTransaction();

                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyId);
                        Params.Add("N_YearID", Master["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 80);
                        Params.Add("N_BranchID", Master["n_BranchId"].ToString());

                        PorderNo = dLayer.GetAutoNumber("Inv_PurchaseOrder", "x_POrderNo", Params, connection, transaction);
                        if (PorderNo == "") { return Ok(api.Warning("Unable to generate Quotation Number")); }
                        MasterTable.Rows[0]["x_POrderNo"] = PorderNo;
                    }
                    else
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
                            bool MaterailRequestVisible = myFunctions.CheckPermission(nCompanyId, 556, "Administrator", dLayer, connection, transaction);
                            bool PurchaseRequestVisible = myFunctions.CheckPermission(nCompanyId, 1049, "Administrator", dLayer, connection, transaction);

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


                    int N_PurchaseOrderId = dLayer.SaveData("Inv_PurchaseOrder", "n_POrderID", MasterTable, connection, transaction);
                    if (N_PurchaseOrderId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Error"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_POrderID"] = N_PurchaseOrderId;
                    }
                    int N_PurchaseOrderDetailId = dLayer.SaveData("Inv_PurchaseOrderDetails", "n_POrderDetailsID", DetailTable, connection, transaction);
                    transaction.Commit();
                }
                return Ok(api.Success("Purchase Order Saved"));
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPOrderID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Inv_PurchaseOrderDetails", "n_POrderID", nPOrderID, "", connection, transaction);
                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to delete PurchaseOrder"));
                    }
                    else
                    {
                        Results = dLayer.DeleteData("Inv_PurchaseOrder", "n_POrderID", nPOrderID, "", connection, transaction);
                    }


                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("PurchaseOrder deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }

                return Ok(api.Error("Unable to Delete PurchaseOrder"));


            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }


        }

    }
}