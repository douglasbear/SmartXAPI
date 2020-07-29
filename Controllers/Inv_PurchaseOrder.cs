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
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Inv_PurchaseOrderController(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");

        }


        [HttpGet("list")]
        public ActionResult GetPurchaseOrderList(int? nCompanyId, int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_InvPurchaseOrderNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 order by D_POrderDate DESC,[Order No]";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

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
                    return StatusCode(200, _api.Response(200, "No Results Found"));
                }
                else
                {
                    return Ok(dt);
                }
            }
            catch (Exception e)
            {
                return StatusCode(404, _api.Response(404, e.StackTrace));
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

                    MasterTable = _api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                    //PurchaseOrder Details
                    int N_POrderID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_POrderID"].ToString());

                    string DetailSql = "";
                    bool MaterailRequestVisible = myFunctions.CheckPermission(nCompanyId, 556, "Administrator", dLayer);
                    bool PurchaseRequestVisible = myFunctions.CheckPermission(nCompanyId, 1049, "Administrator", dLayer);
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
                    var values = MasterTable.Rows[0]["x_POrderNo"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    int nCompanyId = myFunctions.getIntVAL(Master["n_CompanyId"].ToString());

                    int N_POrderID = myFunctions.getIntVAL(Master["n_POrderID"].ToString());

                    if (!myFunctions.checkExistence(Master,"n_POTypeID"))
                        MasterTable.Rows[0]["n_POTypeID"] = 174;

                    if (!myFunctions.checkExistence(Master,"n_POType"))
                        MasterTable.Rows[0]["n_POType"] = 121;

                    transaction = connection.BeginTransaction();

                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyId);
                        Params.Add("N_YearID", Master["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 80);
                        Params.Add("N_BranchID", Master["n_BranchId"].ToString());

                        PorderNo = dLayer.GetAutoNumber("Inv_PurchaseOrder", "x_POrderNo", Params, connection, transaction);
                        if (PorderNo == "") { return StatusCode(409, _api.Response(409, "Unable to generate Quotation Number")); }
                        MasterTable.Rows[0]["x_POrderNo"] = PorderNo;

                        MasterTable.Columns.Remove("n_POrderID");
                        MasterTable.AcceptChanges();
                        DetailTable.Columns.Remove("n_POrderDetailsID");
                        DetailTable.AcceptChanges();
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
                                return StatusCode(400, "Payment Request Processed");
                            }
                        }


                        if (N_POrderID > 0)
                        {
                            MasterTable.Columns.Remove("n_POrderID");
                            MasterTable.AcceptChanges();
                            DetailTable.Columns.Remove("n_POrderDetailsID");
                            DetailTable.AcceptChanges();

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


                    int N_PurchaseOrderId = dLayer.SaveData("Inv_PurchaseOrder", "n_POrderID", N_POrderID, MasterTable, connection, transaction);
                    if (N_PurchaseOrderId <= 0)
                    {
                        transaction.Rollback();
                        return StatusCode(403, "Error");
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_POrderID"] = N_PurchaseOrderId;
                    }
                    int N_PurchaseOrderDetailId = dLayer.SaveData("Inv_PurchaseOrderDetails", "n_POrderDetailsID", 0, DetailTable, connection, transaction);
                    transaction.Commit();
                }
                return Ok("Purchase Order Saved");
            }
            catch (Exception ex)
            {
                return StatusCode(403, ex);
            }
        }

        [HttpDelete()]
        public ActionResult DeleteData(int nPOrderID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Inv_PurchaseOrder", "n_POrderID", nPOrderID, "", connection, transaction);
                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return StatusCode(409, _api.Response(409, "Unable to delete PurchaseOrder"));
                    }
                    else
                    {
                        Results = dLayer.DeleteData("Inv_PurchaseOrderDetails", "n_POrderDetailsID", nPOrderID, "",connection,transaction);
                    }


                    if (Results > 0)
                    {
                        transaction.Commit();
                        return StatusCode(200, _api.Response(200, "PurchaseOrder deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }

                return StatusCode(409, _api.Response(409, "Unable to Delete PurchaseOrder"));


            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.ErrorResponse(ex));
            }


        }

    }
}