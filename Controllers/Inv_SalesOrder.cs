using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    [Route("salesorder")]
    [ApiController]
    public class Inv_SalesOrderController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Inv_SalesOrderController(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 81;
        }


        [HttpGet("list")]
        public ActionResult GetSalesOrderotationList(int? nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and [Order No] like '%" + xSearchkey + "%' or Customer like '%" + xSearchkey + "%'";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_SalesOrderId desc";
            else
                xSortBy = " order by " + xSortBy;


            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvSalesOrderNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvSalesOrderNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and N_SalesOrderId not in (select top(" + Count + ") N_SalesOrderId from vw_InvSalesOrderNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + xSortBy + " ) " + xSortBy;

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from vw_InvSalesOrderNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
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
        [HttpGet("details")]
<<<<<<< HEAD
        public ActionResult GetSalesOrderDetails(int? nCompanyID,string xOrderNo, int nFnYearID, int nLocationID, bool bAllBranchData, int nBranchID, string xQuotationNo)
=======
        public ActionResult GetSalesOrderDetails(int? nCompanyID, string xOrderNo, int nFnYearID, int nLocationID, bool bAllBranchData, int nBranchID,int nQuotationID)
>>>>>>> b551efed1768aa1307bcf9a37429a3077351cefa
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            DataTable DataTable = new DataTable();

            string Mastersql = "";
<<<<<<< HEAD
            Params.Add("@xQuotationNo",xQuotationNo);
=======
            if(nQuotationID>0)
                 Mastersql = "select * from vw_Inv_SalesQuotationMaster_Disp where N_CompanyId=@nCompanyID and N_QuotationId=@nQuotationID";

>>>>>>> b551efed1768aa1307bcf9a37429a3077351cefa

            if (bAllBranchData == true)
            {
                if (xQuotationNo != null)
                {
                    Mastersql = "select * from vw_Inv_SalesQuotationMaster_Disp where X_QuotationNo=@xQuotationNo and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID";
                }
                else
                {
                    Mastersql = "SP_InvSalesOrder_Disp @nCompanyID,@xOrderNo,1,0,@nFnYearID";
                }
            }
            else
            {
                Mastersql = "SP_InvSalesOrder_Disp @nCompanyID,@xOrderNo,1,@nBranchID,@nFnYearID";
                Params.Add("@nBranchID", nBranchID);
            }

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@xOrderNo", xOrderNo);
            Params.Add("@nQuotationID", nQuotationID);
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    MasterTable = _api.Format(MasterTable, "Master");

                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList DetailParams = new SortedList();
                    int N_OthTaxCategoryID = myFunctions.getIntVAL(MasterRow["N_OthTaxCategoryID"].ToString());
                    DetailParams.Add("@nOthTaxCategoryID", N_OthTaxCategoryID);

                    object X_OtherTax = dLayer.ExecuteScalar("Select X_DisplayName from Acc_TaxCategory where N_PkeyID=@nOthTaxCategoryID", DetailParams, connection);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_DisplayName", typeof(string), X_OtherTax);
                    int N_SOrderID = myFunctions.getIntVAL(MasterRow["n_SalesOrderId"].ToString());

                    DetailParams.Add("@nSOrderID", N_SOrderID);
                    DetailParams.Add("@nCompanyID", nCompanyID);
                    object N_SalesOrderTypeID = dLayer.ExecuteScalar("Select N_OrderTypeID from Inv_SalesOrder where N_SalesOrderId=@nSOrderID and N_CompanyID=@nCompanyID", DetailParams, connection);
                    DetailParams.Add("@nSalesOrderTypeID", N_SalesOrderTypeID);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_OrderTypeID", typeof(string), N_SalesOrderTypeID);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "SalesOrderType", typeof(string), "");
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "D_ContractEndDate", typeof(string), null);
                    MasterTable.Rows[0]["SalesOrderType"] = "";
                    if (N_SalesOrderTypeID.ToString() != "")
                    {
                        MasterTable.Rows[0]["SalesOrderType"] = dLayer.ExecuteScalar("Select X_TypeName from Gen_Defaults where N_DefaultId=50 and N_TypeId=@nSalesOrderTypeID", DetailParams, connection);
                        MasterTable.Rows[0]["D_ContractEndDate"] = dLayer.ExecuteScalar("Select D_ContractEndDate from Inv_SalesOrder where N_SalesOrderId=@nSOrderID and N_CompanyID=@nCompanyID", DetailParams, connection);
                    }
                    DetailParams.Add("n_LocationID", MasterRow["N_LocationID"]);
                    string Location = Convert.ToString(dLayer.ExecuteScalar("select X_LocationName from Inv_Location where N_CompanyID=@nCompanyID and N_LocationID=@n_LocationID", DetailParams, connection));
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_LocationName", typeof(string), Location);
                    object InSales = null, InDeliveryNote = null, CancelStatus = null;
                    if (myFunctions.getIntVAL(N_SalesOrderTypeID.ToString()) != 175)
                    {
                        if (Convert.ToBoolean(MasterRow["N_Processed"]))
                        {
                            InSales = dLayer.ExecuteScalar("select x_ReceiptNo from Inv_Sales where N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);
                            InDeliveryNote = dLayer.ExecuteScalar("select x_ReceiptNo from Inv_DeliveryNote where N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);
                            CancelStatus = dLayer.ExecuteScalar("select 1 from Inv_SalesOrder where B_CancelOrder=1 and N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);

                        }
                    }
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "SalesDone", typeof(int), InSales != null ? 1 : 0);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_SalesReceiptNo", typeof(string), InSales);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "DeliveryNoteDone", typeof(int), InDeliveryNote != null ? 1 : 0);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_DeliveryNoteNo", typeof(string), InDeliveryNote);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "SalesOrderCanceled", typeof(string), CancelStatus);

                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "ChkCancelOrderEnabled", typeof(bool), true);


                    if (InSales != null)
                    {
                        object InvoicedQty = dLayer.ExecuteScalar("select SUM(Inv_SalesOrderDetails.N_Qty) from Inv_SalesOrderDetails where N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);
                        object OrderQty = dLayer.ExecuteScalar("select SUM(Inv_SalesDetails.N_Qty) from Inv_SalesDetails where N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);
                        if (InvoicedQty != null && OrderQty != null)
                        {
                            if (InvoicedQty.ToString() != OrderQty.ToString())
                                MasterTable.Rows[0]["ChkCancelOrderEnabled"] = true;
                        }
                    }

                    int N_ProjectID = myFunctions.getIntVAL(MasterRow["N_ProjectID"].ToString());
                    //MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_ProjectName", typeof(string), "");

                    // if (N_ProjectID > 0)
                    // {
                    //     DetailParams.Add("@nProjectID", N_ProjectID);
                    //     MasterTable.Rows[0]["X_ProjectName"] = Convert.ToString(dLayer.ExecuteScalar("select X_ProjectName from Inv_CustomerProjects where N_CompanyID=@nCompanyID and N_ProjectID=@nProjectID", DetailParams, connection));
                    // }
                    //MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_SalesmanName", typeof(string), "");

                    // if (MasterRow["N_SalesmanID"].ToString() != "")
                    // {
                    //     DetailParams.Add("@nSalesmanID", MasterRow["N_SalesmanID"].ToString());
                    //     MasterTable.Rows[0]["X_SalesmanName"] = Convert.ToString(dLayer.ExecuteScalar("select X_SalesmanName from Inv_Salesman where N_CompanyID=@nCompanyID and N_SalesmanID=@nSalesmanID", DetailParams, connection));
                    // }






                    string DetailSql = "";
                    SortedList NewParams = new SortedList();
                    NewParams.Add("@xQuotationNo",xQuotationNo);
                    if (xQuotationNo != null)
                    {
                        DetailSql = "select * from vw_Inv_SalesQuotationDetails_Disp where X_QuotationNo=@xQuotationNo and N_CompanyID=@nCompanyID and N_LocationID=@nLocationID N_BranchID=@nBranchID and N_FnYearID=@nFnYearID";
                    }
                    else
                    {
                        DetailSql = "SP_InvSalesOrderDtls_Disp @nCompanyID,@nSOrderID,@nFnYearID,1,@nLocationID";
                    }
                    NewParams.Add("@nLocationID", nLocationID);
                    NewParams.Add("@nFnYearID", nFnYearID);
                    NewParams.Add("@nCompanyID", nCompanyID);
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, NewParams, connection);
                    DetailTable = _api.Format(DetailTable, "Details");

                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_CustomerName", typeof(string), "");
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "customer_PONo", typeof(string), "");
                    DetailTable = myFunctions.AddNewColumnToDataTable(DetailTable, "X_UpdatedSPrice", typeof(string), "");
                    if (DetailTable.Rows.Count != 0)
                    {
                        MasterTable.Rows[0]["x_CustomerName"] = DetailTable.Rows[0]["x_CustomerName"];
                        MasterTable.Rows[0]["customer_PONo"] = DetailTable.Rows[0]["customer_PONo"];
                    }
                    SortedList Param = new SortedList();
                    Param.Add("@nCompanyID", nCompanyID);
                    Param.Add("@nSPriceTypeID", "");
                    foreach (DataRow var in DetailTable.Rows)
                    {
                        if (var["N_SPriceTypeID"].ToString() != "")
                        {
                            Params["@nSPriceTypeID"] = var["N_SPriceTypeID"].ToString();
                            var["X_UpdatedSPrice"] = Convert.ToString(dLayer.ExecuteScalar("select X_Name from Gen_LookupTable where N_CompanyID=@nCompanyID and N_ReferId=3 and N_PkeyId=@nSPriceTypeID", Param, connection));
                        }
                    }


                    dt.Tables.Add(MasterTable);
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
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();


                    int n_SalesOrderId = myFunctions.getIntVAL(MasterRow["n_SalesOrderId"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int N_BranchID = myFunctions.getIntVAL(MasterRow["n_BranchID"].ToString());
                    int N_LocationID = myFunctions.getIntVAL(MasterRow["n_LocationID"].ToString());
                    int N_QuotationID= myFunctions.getIntVAL(MasterRow["n_QuotationID"].ToString());
                    string x_OrderNo = MasterRow["x_OrderNo"].ToString();

                    if (x_OrderNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("N_BranchID", N_BranchID);
                        x_OrderNo = dLayer.GetAutoNumber("Inv_SalesOrder", "X_OrderNo", Params, connection, transaction);
                        if (x_OrderNo == "") { return Ok("Unable to generate Sales Order Number"); }
                        MasterTable.Rows[0]["X_OrderNo"] = x_OrderNo;
                    }

                      if (n_SalesOrderId > 0)
                        {
                            dLayer.ExecuteScalar("SP_Delete_Trans_With_Accounts " + N_CompanyID + ",'Sales Order'," + n_SalesOrderId.ToString(), connection,transaction);
                            dLayer.ExecuteScalar("delete from Inv_DeliveryDispatch where N_SOrderID=" + n_SalesOrderId.ToString() + " and N_CompanyID=" + N_CompanyID,connection,transaction);
                        }

string DupCriteria = "N_CompanyID=" + N_CompanyID + " and X_OrderNo='" + x_OrderNo + "' and N_FnYearID=" + N_FnYearID+ "";


                    n_SalesOrderId = dLayer.SaveData("Inv_SalesOrder", "N_SalesOrderID",DupCriteria,"", MasterTable, connection, transaction);
                    if (n_SalesOrderId <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save sales order");
                    }
                    if (N_QuotationID > 0)
                        dLayer.ExecuteNonQuery("Update Inv_SalesQuotation Set  N_Processed=1 Where N_QuotationID=" + N_QuotationID + " and N_FnYearID=" + N_FnYearID + " and N_CompanyID=" + N_CompanyID.ToString(),connection,transaction);

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_SalesOrderId"] = n_SalesOrderId;
                    }
                    int N_QuotationDetailId = dLayer.SaveData("Inv_SalesOrderDetails", "N_SalesOrderDetailsID", DetailTable, connection, transaction);
                    if (N_QuotationDetailId <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save sales order");
                    }
                    else
                    {
                        transaction.Commit();
                    }
                    SortedList Result = new SortedList();
                Result.Add("n_SalesOrderID",n_SalesOrderId);
                Result.Add("x_SalesOrderNo",x_OrderNo);
                return Ok(_api.Success(Result,"Sales Order Saved"));
                }
            }
            catch (Exception ex)
            {

                return Ok(_api.Error(ex));
            }
        }
        //Delete....
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nSalesOrderID, int nBranchID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    var xUserCategory = myFunctions.GetUserCategory(User);// User.FindFirst(ClaimTypes.GroupSid)?.Value;
                    var nUserID = myFunctions.GetUserID(User);// User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    object objProcessed = dLayer.ExecuteScalar("Select Isnull(N_SalesID,0) from Inv_SalesOrder where N_CompanyID=" + nCompanyID + " and N_SalesOrderId=" + nSalesOrderID + " and N_FnYearID=" + nFnYearID + "", connection, transaction);
                    if(objProcessed==null)objProcessed=0;
                    if (myFunctions.getIntVAL(objProcessed.ToString()) == 0)
                    {
                        SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType","SALES ORDER"},
                                {"N_VoucherID",nSalesOrderID},
                                {"N_UserID",nUserID},
                                {"X_SystemName","WebRequest"},
                                {"N_BranchID",nBranchID}};
                        Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                        if (Results <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable to delete Sales Order"));
                        }
                        else
                        {
                            transaction.Commit();
                            return Ok(_api.Success("Sales Order deleted"));

                        }
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Sales invoice processed! Unable to delete Sales Order"));

                    }


                    // connection.Open();
                    // SqlTransaction transaction = connection.BeginTransaction();
                    // Results = dLayer.DeleteData("Inv_SalesOrderDetails", "N_SalesOrderID", nSalesOrderID, "", connection, transaction);
                    // if (Results <= 0)
                    // {
                    //     transaction.Rollback();
                    //     return Ok(_api.Error("Unable to delete sales order"));
                    // }
                    // else
                    // {
                    // Results = dLayer.DeleteData("Inv_SalesOrder", "N_SalesOrderID", nSalesOrderID, "", connection, transaction);

                    // }

                    // if (Results > 0)
                    // {
                    //     transaction.Commit();
                    //     return Ok(_api.Error("Sales order deleted"));
                    // }
                    // else
                    // {
                    //     transaction.Rollback();
                    //     return Ok(_api.Error("Unable to delete sales order"));
                    // }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }


        }




        [HttpGet("getsettings")]
        public ActionResult GetSettings(int? nCompanyID)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    bool B_SAlesmanEnabled = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "LastSPrice_InGrid", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    bool B_CustomerProjectEnabled = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "CustomerProject Enabled", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    bool X_NotesEnabled = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "Notes Enabled", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID", nCompanyID);
                    bool B_SPRiceType = false;
                    object res = dLayer.ExecuteScalar("Select Isnull(N_Value,0) from Gen_Settings where N_CompanyID=@nCompanyID and X_Group='Inventory' and X_Description='Selling Price Calculation'", Params, connection);
                    if (res != null)
                    {
                        if (myFunctions.getIntVAL(res.ToString()) == 4)
                            B_SPRiceType = true;
                    }
                    SortedList Results = new SortedList();
                    Results.Add("B_SAlesmanEnabled", B_SAlesmanEnabled);
                    Results.Add("B_CustomerProjectEnabled", B_CustomerProjectEnabled);
                    Results.Add("X_NotesEnabled", X_NotesEnabled);
                    Results.Add("B_SPRiceType", B_SPRiceType);
                    return Ok(_api.Success(Results));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }


        [HttpGet("getItem")]
        public ActionResult GetItem(int nCompanyID, int nLocationID, int nBranchID, string dDate, string InputVal, int nCustomerID)
        {
            string ItemCondition = "";
            object subItemPrice, subPprice, subMrp;
            string sql = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int N_DefSPriceID = 0;
                var UserCategoryID = myFunctions.GetUserCategory(User);
                N_DefSPriceID = myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "DefSPriceTypeID", "N_Value", "N_UserCategoryID", UserCategoryID.ToString(), myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection));
                int nSPriceID = N_DefSPriceID;
                DateTime dateVal = myFunctions.GetFormatedDate(dDate.ToString());
                SortedList paramList = new SortedList();
                paramList.Add("@nCompanyID", nCompanyID);
                paramList.Add("@nLocationID", nLocationID);
                paramList.Add("@nBranchID", nBranchID);
                paramList.Add("@date", myFunctions.getDateVAL(dateVal));
                paramList.Add("@nSPriceID", N_DefSPriceID);

                ItemCondition = "[Item Code] ='" + InputVal + "'";
                DataTable SubItems = new DataTable();
                DataTable LastSalesPrice = new DataTable();
                DataTable ItemDetails = new DataTable();
                // if (B_BarcodeBilling)
                //     ItemCondition = "([Item Code] ='" + InputVal + "' OR X_Barcode ='" + InputVal + "')";
                bool B_SPRiceType = false;

                object res = dLayer.ExecuteScalar("Select Isnull(N_Value,0) from Gen_Settings where N_CompanyID=@nCompanyID and X_Group='Inventory' and X_Description='Selling Price Calculation'", paramList, connection);
                if (res != null)
                {
                    if (myFunctions.getIntVAL(res.ToString()) == 4)
                        B_SPRiceType = true;
                    else
                        B_SPRiceType = false;

                }

                string X_DefSPriceType = "";

                if (B_SPRiceType)
                {
                    X_DefSPriceType = "";

                    res = dLayer.ExecuteScalar("select X_Name from Gen_LookupTable where N_PkeyId=@nDefSPriceID and N_ReferId=3 and N_CompanyID=@nCompanyID", paramList, connection);
                    if (res != null)
                        X_DefSPriceType = res.ToString();

                }

                sql = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_Stock, dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as N_StockTotal, dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit) As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nCompanyID";

                if (B_SPRiceType)
                {
                    if (nSPriceID > 0)
                    {
                        sql = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_Stock, dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as N_StockTotal ,dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit) As N_LPrice ,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nSPriceID,@nBranchID) As N_SPrice From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nCompanyID";
                    }
                    else
                        sql = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_Stock, dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as N_StockTotal, dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit) As N_LPrice ,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nDefSPriceID, @nBranchID) As N_SPrice From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=@nComapanyID";

                }

                ItemDetails = dLayer.ExecuteDataTable(sql, paramList, connection);

                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "LastSalesPrice", typeof(string), "0.00");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "X_DefSPriceType", typeof(string), "");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "N_DefSPriceID", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "LastPurchaseCost", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "SpriceSum", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "PpriceSum", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "Mrpsum", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "N_BaseUnitQty", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "N_SellingPrice", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "CustomerDiscount", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "N_PriceID", typeof(string), "0");
                ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "N_PriceVal", typeof(string), "0");

                if (ItemDetails.Rows.Count == 1)
                {
                    DataRow ItemDetailRow = ItemDetails.Rows[0];
                    string ItemClass = ItemDetailRow["N_ClassID"].ToString();

                    int N_ItemID = myFunctions.getIntVAL(ItemDetailRow["N_ItemID"].ToString());
                    SortedList qParam2 = new SortedList();
                    qParam2.Add("@nItemID", N_ItemID);
                    qParam2.Add("@nCompanyID", nCompanyID);


                    if (ItemClass == "1")
                    {

                        SortedList qParam3 = new SortedList();
                        qParam3.Add("@nItemID", 0);
                        qParam3.Add("@nCompanyID", nCompanyID);
                        SubItems = dLayer.ExecuteDataTable("Select *,dbo.SP_Stock(vw_invitemdetails.N_ItemID) As N_Stock from vw_invitemdetails where N_MainItemId=@nItemID and N_CompanyID=@nCompanyID order by X_Itemname", qParam3, connection);
                        double SpriceSum = 0, PpriceSum = 0, Mrpsum = 0;
                        foreach (DataRow var in SubItems.Rows)
                        {

                            if (var["N_ItemDetailsID"].ToString() != "")
                            {
                                qParam3["@nItemID"] = var["N_ItemId"].ToString();
                                subItemPrice = dLayer.ExecuteScalar("Select top 1 N_Sprice from Inv_StockMaster where N_ItemId=@nItemID and N_CompanyID=@nCompanyID order by n_stockid desc", qParam3, connection);
                                subPprice = dLayer.ExecuteScalar("Select top 1 N_Sprice from Inv_StockMaster where N_ItemId=@nItemID and N_CompanyID=@nCompanyID order by n_stockid desc", qParam3, connection);
                                subMrp = dLayer.ExecuteScalar("Select top 1 N_Mrp from Inv_PurchaseDetails where N_ItemId=@nItemID and N_CompanyID=@nCompanyID Order By N_PurchaseDetailsId desc", qParam3, connection);
                                if (subItemPrice != null) SpriceSum = myFunctions.getVAL(subItemPrice.ToString()) * myFunctions.getVAL(var["N_Qty"].ToString()) + SpriceSum;
                                if (subPprice != null) PpriceSum = myFunctions.getVAL(subPprice.ToString()) + PpriceSum;
                                if (subMrp != null) Mrpsum = myFunctions.getVAL(subMrp.ToString()) + Mrpsum;
                            }
                        }
                        ItemDetails.Rows[0]["SpriceSum"] = SpriceSum;
                        ItemDetails.Rows[0]["PpriceSum"] = PpriceSum;
                        ItemDetails.Rows[0]["Mrpsum"] = Mrpsum;
                    }







                    object objSPrice = dLayer.ExecuteScalar("Select Isnull(N_Value,0) from Gen_Settings where N_CompanyID=1 and X_Group='Inventory' and X_Description='Selling Price Calculation'", connection);

                    string X_ItemUnit = ItemDetailRow["X_ItemUnit"].ToString();
                    qParam2.Add("@X_ItemUnit", X_ItemUnit);
                    DataTable SellingPrice = dLayer.ExecuteDataTable("Select N_Qty,N_SellingPrice from Inv_ItemUnit Where N_CompanyID=@nCompanyID and N_ItemID = @nItemID and X_ItemUnit=@X_ItemUnit", qParam2, connection);

                    if (SellingPrice.Rows.Count > 0)
                    {
                        ItemDetails.Rows[0]["N_Qty"] = SellingPrice.Rows[0]["N_Qty"].ToString();
                        ItemDetails.Rows[0]["N_SellingPrice"] = SellingPrice.Rows[0]["N_SellingPrice"].ToString();
                        if (myFunctions.getVAL(SellingPrice.Rows[0]["N_SellingPrice"].ToString()) > 0)
                        {
                            bool B_LastPurchaseCost = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings(this.FormID.ToString(), "LastPurchaseCost", "N_Value", "N_UserCategoryID", UserCategoryID.ToString(), myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                            if (B_LastPurchaseCost)
                            {
                                object LastPurchaseCost = dLayer.ExecuteScalar("Select TOP(1) ISNULL(N_LPrice,0) from Inv_StockMaster Where N_ItemID=@nItemID and N_CompanyID=@nCompanyID and  (X_Type='Purchase' or X_Type='Opening') Order by N_StockID Desc", qParam2, connection);
                                ItemDetails.Rows[0]["LastPurchaseCost"] = B_LastPurchaseCost;
                            }

                        }
                    }

                    qParam2.Add("@nCustomerID", nCustomerID);
                    object value = dLayer.ExecuteScalar("select N_DiscPerc from inv_CustomerDiscount where N_ProductID =@nItemID and N_CustomerID =@nCustomerID and N_CompanyID =@nCompanyID", qParam2, connection);
                    if (value != null)
                    {
                        ItemDetails.Rows[0]["CustomerDiscount"] = value;
                    }

                    SortedList paramList4 = new SortedList();
                    paramList4.Add("@nBranchID", nBranchID);
                    paramList4.Add("@nItemID", N_ItemID);
                    paramList4.Add("@nCompanyID", nCompanyID);
                    paramList4.Add("@xDefultSPriceID", X_DefSPriceType);

                    DataTable PriceMaster = dLayer.ExecuteDataTable("Select N_PriceID,N_PriceVal From Inv_ItemPriceMaster  Where N_CompanyID=@nCompanyID and N_BranchID=@nBranchID and N_itemId=@nItemID and N_PriceID in(Select N_PkeyId from Gen_LookupTable where X_Name=@xDefultSPriceID and N_CompanyID=@nCompanyID)", paramList4, connection);


                    if (PriceMaster.Rows.Count > 0)
                    {
                        ItemDetails.Rows[0]["SpriceSum"] = PriceMaster.Rows[0]["N_PriceID"].ToString();
                        ItemDetails.Rows[0]["PpriceSum"] = PriceMaster.Rows[0]["N_PriceVal"].ToString();
                    }


                }



                return Ok(_api.Success(_api.Format(ItemDetails)));
            }
        }



        [HttpGet("validateItemPrice")]
        public ActionResult SalesPriceValidation(int nCompanyID, int nLocationID, int nBranchID, int nItemID, int nCustomerID)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    bool B_SPRiceType = false;
                    string X_DefSPriceType = "";
                    int N_DefSPriceID = 0;
                    DataTable SalePrice = new DataTable();
                    SortedList Params = new SortedList();
                    SortedList OutPut = new SortedList();
                    Params.Add("@nCompanyID", nCompanyID);

                    object res = dLayer.ExecuteScalar("Select Isnull(N_Value,0) from Gen_Settings where N_CompanyID=@nCompanyID and X_Group='Inventory' and X_Description='Selling Price Calculation'", Params, connection);
                    if (res != null)
                    {
                        if (myFunctions.getIntVAL(res.ToString()) == 4)
                            B_SPRiceType = true;
                        else
                            B_SPRiceType = false;

                    }
                    if (B_SPRiceType)
                    {
                        X_DefSPriceType = "";
                        var UserCategoryID = User.FindFirst(ClaimTypes.GroupSid)?.Value;
                        N_DefSPriceID = myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "DefSPriceTypeID", "N_Value", "N_UserCategoryID", UserCategoryID, myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection));
                        Params.Add("@nDefSPriceID", N_DefSPriceID);
                        object res2 = dLayer.ExecuteScalar("select X_Name from Gen_LookupTable where N_PkeyId=@nDefSPriceID and N_ReferId=3 and N_CompanyID=@nCompanyID", Params, connection);
                        if (res2 != null)
                            X_DefSPriceType = res.ToString();
                        else X_DefSPriceType = "";

                    }
                    Params.Add("@nBranchID", nBranchID);
                    Params.Add("@nItemID", nItemID);
                    Params.Add("@xDefSPriceType", X_DefSPriceType);
                    SalePrice = dLayer.ExecuteDataTable("Select N_PriceID,N_PriceVal From Inv_ItemPriceMaster  Where N_CompanyID=@nCompanyID and N_BranchID=@nBranchID and N_itemId=@nItemID and N_PriceID in(Select N_PkeyId from Gen_LookupTable where X_Name=@xDefSPriceType and N_CompanyID=@nCompanyID)", Params, connection);


                    if (SalePrice.Rows.Count > 0)
                    {
                        OutPut.Add("N_PriceID", SalePrice.Rows[0]["N_PriceID"]);
                        OutPut.Add("N_PriceVal", SalePrice.Rows[0]["N_PriceVal"]);

                    }
                    else
                    {
                        OutPut.Add("N_PriceID", 0);
                        OutPut.Add("N_PriceVal", "");
                    }

                    Params.Add("@nCustomerID", nCustomerID);
                    object value = dLayer.ExecuteScalar("select N_DiscPerc from inv_CustomerDiscount where N_ProductID =@nItemID and N_CustomerID =@nCustomerID and N_CompanyID =@nCompanyID", Params, connection);
                    if (value != null)
                    {
                        OutPut.Add("N_DiscPerc", value);
                    }
                    else
                    {
                        OutPut.Add("N_DiscPerc", "");
                    }



                    return Ok(_api.Success(OutPut));
                }
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }



        [HttpGet("getItemList")]
        public ActionResult GetItemList(int nCompanyID, int nLocationID, int nBranchID, string query, int PageSize, int Page)
        {
            string sql = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int N_DefSPriceID = 0;
                var UserCategoryID = User.FindFirst(ClaimTypes.GroupSid)?.Value;
                N_DefSPriceID = myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "DefSPriceTypeID", "N_Value", "N_UserCategoryID", UserCategoryID, myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection));
                int nSPriceID = N_DefSPriceID;
                SortedList paramList = new SortedList();
                paramList.Add("@nCompanyID", nCompanyID);
                paramList.Add("@nLocationID", nLocationID);
                paramList.Add("@nBranchID", nBranchID);
                paramList.Add("@PSize", PageSize);
                paramList.Add("@Offset", Page);



                DataTable ItemDetails = new DataTable();
                string qry = "";
                if (query != "" && query != null)
                {
                    qry = " and (Description like @query or [Item Code] like @query) order by [Item Code],Description";
                    paramList.Add("@query", "%" + query + "%");
                }

                string pageQry = "DECLARE @PageSize INT, @Page INT Select @PageSize=@PSize,@Page=@Offset;WITH PageNumbers AS(Select ROW_NUMBER() OVER(ORDER BY N_ItemID) RowNo,";
                string pageQryEnd = ") SELECT * FROM    PageNumbers WHERE   RowNo BETWEEN((@Page -1) *@PageSize + 1)  AND(@Page * @PageSize)";

                bool B_SPRiceType = false;

                object res = dLayer.ExecuteScalar("Select Isnull(N_Value,0) from Gen_Settings where N_CompanyID=@nCompanyID and X_Group='Inventory' and X_Description='Selling Price Calculation'", paramList, connection);
                if (res != null)
                {
                    if (myFunctions.getIntVAL(res.ToString()) == 4)
                        B_SPRiceType = true;
                    else
                        B_SPRiceType = false;

                }

                sql = " *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_Stock, dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as N_StockTotal, dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit) As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice From vw_InvItem_Search Where  N_CompanyID=@nCompanyID ";

                if (B_SPRiceType)
                {
                    if (nSPriceID > 0)
                    {
                        sql = " *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_Stock, dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as N_StockTotal ,dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit) As N_LPrice ,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nSPriceID,@nBranchID) As N_SPrice From vw_InvItem_Search Where N_CompanyID=@nCompanyID ";
                    }
                    else
                        sql = " *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_Stock, dbo.SP_Stock(vw_InvItem_Search.N_ItemID) as N_StockTotal, dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_SalesUnit) As N_LPrice ,dbo.SP_SellingPrice_Select(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,@nDefSPriceID, @nBranchID) As N_SPrice From vw_InvItem_Search Where N_CompanyID=@nComapanyID ";

                }

                ItemDetails = dLayer.ExecuteDataTable(pageQry + sql + pageQryEnd + qry, paramList, connection);
                if (ItemDetails.Rows.Count == 0)
                {
                    return Ok(_api.Error("No Items Found"));
                }
                return Ok(_api.Success(_api.Format(ItemDetails)));
            }
        }


    }
}