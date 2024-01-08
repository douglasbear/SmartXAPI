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
    [Route("productionOrder")]
    [ApiController]
    public class Inv_Process : ControllerBase
    {

        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int N_FormID;

        public Inv_Process(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 54;
        }
        [HttpGet("list")]
        public ActionResult ProductionOrderList(int nFnYearId, bool b_IsProcess, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearId);
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ( X_ReferenceNo like '%" + xSearchkey + "%' or cast([d_Date] as VarChar)   like '%" + xSearchkey + "%'  or x_ItemName like '%" + xSearchkey + "%'  or x_LocationName like '%" + xSearchkey + "%'   or status like '%" + xSearchkey + "%' ) ";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_AssemblyID desc";
            // xSortBy = " order by batch desc,D_TransDate desc";
            else
                xSortBy = " order by " + xSortBy;
            if (b_IsProcess == true)
            {
                if (Count == 0)
                    sqlCommandText = "select top(" + nSizeperpage + ")  * from vw_InvAssembly where N_CompanyID=@p1 and N_FnYearID=@p2 and  X_Action='Build' " + Searchkey;
                else
                    sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvAssembly where N_CompanyID=@p1 and N_FnYearID=@p2 and  X_Action='Build'" + Searchkey + "and N_AssemblyID not in (select top(" + Count + ") N_AssemblyID from vw_InvAssembly where N_CompanyID=@p1 and N_FnYearID=@p2 and  X_Action='Build' ) " + Searchkey;

            }
            else
            {
                if (Count == 0)
                    sqlCommandText = "select top(" + nSizeperpage + ")  * from vw_InvAssembly where N_CompanyID=@p1 and N_FnYearID=@p2 and  X_Action='Build' and  B_IsProcess=0   " + Searchkey;
                else
                    sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvAssembly where N_CompanyID=@p1 and N_FnYearID=@p2 and  X_Action='Build'  and  B_IsProcess=0" + Searchkey + "and N_AssemblyID not in (select top(" + Count + ") N_AssemblyID from vw_InvAssembly where N_CompanyID=@p1 and N_FnYearID=@p2 and  X_Action='Build' and  B_IsProcess=0) " + Searchkey;

            }
            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText + xSortBy, Params, connection);

                    sqlCommandCount = "select count(1) as N_Count  from vw_InvAssembly where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return BadRequest(_api.Error(User, e));
            }
        }
        [HttpGet("productList")]
        public ActionResult ProductList(int nFnYearID, int n_LocationID, bool b_IsProcess)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);

            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@n_LocationID", n_LocationID);

            string sqlCommandText = "";
            if (b_IsProcess == true)
                sqlCommandText = "Select * from vw_InvItem_Search_WHLink_PRS  Where N_CompanyID=@nCompanyID and N_WarehouseID=@n_LocationID and [Item Class]='Assembly Item'";
            else
                sqlCommandText = "Select * from vw_InvItem_Search_WHLink  Where N_CompanyID=@nCompanyID and N_WarehouseID=@n_LocationID and [Item Class]='Assembly Item'";

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
                return Ok(_api.Error(User, e));
            }

        }
        [HttpGet("getItemDetails")]
        public ActionResult GetItem(int nLocationID, int nBranchID, string xInputVal, int nCustomerID, string xBatch, int reqUnitId, int reqQty, int nAssemblyID, string x_Action)
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
                    DataTable ItemDetails = new DataTable();
                    DataTable ItemStock = new DataTable();
                    DataTable ProducionLabourCost = new DataTable();
                    DataTable ProductionMachineCost = new DataTable();
                    DataTable ByProductDetails = new DataTable();
                    DataTable ScrapDetails = new DataTable();
                    // DataTable ItemStock = new DataTable();
                    DataTable ItemStockUnit = new DataTable();

                    string StockQuerry = "";
                    string ScrapDetailssql = "";






                    string ItemCondition = "([Item Code] ='" + xInputVal + "' OR X_Barcode ='" + xInputVal + "')";
                    string SQL = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID," + nLocationID + ",'','Location') As N_Stock ,dbo.SP_Cost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_ItemUnit) As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice  From vw_InvItem_Search Where " + ItemCondition + " and N_CompanyID=" + nCompanyID;

                    ItemDetails = dLayer.ExecuteDataTable(SQL, Params, connection);
                    // if (ElementsTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    ItemDetails.AcceptChanges();
                    ItemDetails = myFunctions.AddNewColumnToDataTable(ItemDetails, "N_TxtQty", typeof(int), 0);
                    int N_ItemID = myFunctions.getIntVAL(ItemDetails.Rows[0]["N_ItemID"].ToString());
                    if (ItemDetails.Rows.Count > 0)
                    {
                        if (reqUnitId != 0 && reqQty != 0)
                        {
                            if (myFunctions.getIntVAL(ItemDetails.Rows[0]["N_BOMUnitID"].ToString()) == reqUnitId)
                            {
                                ItemDetails.Rows[0]["N_TxtQty"] = reqQty;

                            }
                            else
                            {
                                string qry = "select N_Qty from Inv_ItemUnit inner join Inv_ItemMaster on Inv_ItemUnit.N_ItemUnitID=Inv_ItemMaster.n_BOMUnitID  where Inv_ItemMaster.N_ItemID=" + N_ItemID + " and Inv_ItemUnit.N_CompanyID=" + nCompanyID + " and Inv_ItemUnit.N_ItemUnitID=" + myFunctions.getIntVAL(ItemDetails.Rows[0]["N_BOMUnitID"].ToString());
                                object BOMQty = dLayer.ExecuteScalar(qry, Params, connection);
                                if (BOMQty != null)
                                {
                                    int bomQty = myFunctions.getIntVAL(BOMQty.ToString());
                                    int mod = reqQty % bomQty;
                                    if (mod > 0)
                                        ItemDetails.Rows[0]["N_TxtQty"] = ((reqQty / bomQty) + 1).ToString();
                                    else if (mod == 0)
                                        ItemDetails.Rows[0]["N_TxtQty"] = (reqQty / bomQty).ToString();

                                }

                            }
                        }
                        //loading stockitem
                        StockQuerry = "select N_ItemID,dbo.SP_GenGetStock(Inv_StockMaster.N_ItemID," + nLocationID + ",'','Location') as N_CurrentStock from Inv_StockMaster group by N_ItemID having N_ItemID=" + N_ItemID;
                        ItemStock = dLayer.ExecuteDataTable(StockQuerry, Params, connection);
                        ItemStock.AcceptChanges();


                        if (ItemStock.Rows.Count > 0)
                        {
                            string sql = "select X_ItemUnit from Inv_ItemUnit inner join Inv_ItemMaster on Inv_ItemMaster.N_ItemUnitID=Inv_ItemUnit.N_ItemUnitID where Inv_ItemMaster.N_ItemID= " + N_ItemID;

                            ItemStockUnit = dLayer.ExecuteDataTable(sql, Params, connection);
                            ItemStockUnit.AcceptChanges();

                        }

                    }

                    ScrapDetailssql = "select *,dbo.SP_GenGetStock(vw_InvItemDetails.N_ItemID," + nLocationID + ",'','Location') As N_Stock,dbo.SP_Cost(vw_InvItemDetails.N_ItemID,vw_InvItemDetails.N_CompanyID,'') As N_Cost,dbo.SP_SellingPrice(vw_InvItemDetails.N_ItemID,vw_InvItemDetails.N_CompanyID) As N_SPrice From vw_InvItemDetails Where N_MainItemID =" + N_ItemID + " and N_CompanyID=" + nCompanyID + " and N_Type=2";

                    ScrapDetails = dLayer.ExecuteDataTable(ScrapDetailssql, Params, connection);
                    // if (ElementsTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    ScrapDetails.AcceptChanges();

                    ItemDetails.AcceptChanges();


                    string sql1 = "Select *,dbo.SP_GenGetStock(vw_InvItemDetails.N_ItemID," + nLocationID + ",'','Location') As N_Stock,dbo.[SP_Cost_Loc](vw_InvItemDetails.N_ItemID,vw_InvItemDetails.N_CompanyID,''," + nLocationID + ") As N_Cost,dbo.SP_Cost(vw_InvItemDetails.N_ItemID,vw_InvItemDetails.N_CompanyID,'') As N_Cost1,dbo.SP_SellingPrice(vw_InvItemDetails.N_ItemID,vw_InvItemDetails.N_CompanyID) As N_SPrice From vw_InvItemDetails Where N_MainItemID =" + N_ItemID + " and N_CompanyID=" + nCompanyID + " and N_Type=1";
                    ByProductDetails = dLayer.ExecuteDataTable(sql1, Params, connection);
                    // if (ElementsTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    ByProductDetails.AcceptChanges();
                    // string StockQuerry = "Select Inv_ItemDetails.N_MainItemID,Inv_ItemDetails.N_ItemID," + nLocationID + " as N_locationID,dbo.SP_GenGetStock(Inv_ItemDetails.N_ItemID," + nLocationID + ",'','Location') as N_CurrentStock from Inv_ItemDetails inner join Inv_StockMaster on Inv_ItemDetails.N_ItemID=Inv_StockMaster.N_ItemID  group by Inv_ItemDetails.N_ItemID,Inv_ItemDetails.N_MainItemID having Inv_ItemDetails.N_MainItemID=" + N_ItemID;
                    // ItemStock = dLayer.ExecuteDataTable(StockQuerry, Params, connection);
                    // // if (ElementsTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    // ItemStock.AcceptChanges();

                    string LabourCost = "Select * from Inv_ProductionCost where N_AssembleID=" + nAssemblyID + " and N_FormTypeID=" + 1 + "";
                    ProducionLabourCost = dLayer.ExecuteDataTable(LabourCost, Params, connection);
                    ProducionLabourCost.AcceptChanges();

                    string machinecost = "Select * from Inv_ProductionCost where N_AssembleID=" + nAssemblyID + " and N_FormTypeID=" + 2 + "";
                    ProductionMachineCost = dLayer.ExecuteDataTable(machinecost, Params, connection);
                    ProductionMachineCost.AcceptChanges();




                    ItemDetails = _api.Format(ItemDetails, "ItemDetails");
                    ItemStock = _api.Format(ItemStock, "ItemStock");
                    ProducionLabourCost = _api.Format(ProducionLabourCost, "ProducionLabourCost");
                    ProductionMachineCost = _api.Format(ProductionMachineCost, "ProductionMachineCost");
                    ItemStockUnit = _api.Format(ItemStockUnit, "ItemStockUnit");
                    ByProductDetails = _api.Format(ByProductDetails, "ByProductDetails");
                    ScrapDetails = _api.Format(ScrapDetails, "ScrapDetails");
                    dt.Tables.Add(ItemDetails);
                    dt.Tables.Add(ItemStock);
                    dt.Tables.Add(ProducionLabourCost);
                    dt.Tables.Add(ProductionMachineCost);
                    dt.Tables.Add(ByProductDetails);
                    dt.Tables.Add(ItemStockUnit);
                    dt.Tables.Add(ScrapDetails);



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
                    DataTable DetailTable;
                    DataTable ScrapDetails;
                    DataTable ProductionLabourCost;
                    DataTable ProductionMachineCost;
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    ScrapDetails = ds.Tables["scrapDetails"];
                    ProductionLabourCost = ds.Tables["ProductionLabourCost"];
                    ProductionMachineCost = ds.Tables["ProductionMachineCost"];
                    DataTable OtherCost = ds.Tables["otherCostDetails"];



                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nAssemblyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_AssemblyID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    string X_ReferenceNo = MasterTable.Rows[0]["X_ReferenceNo"].ToString();
                    int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                    int N_ItemID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ItemID"].ToString());
                    int N_BOMUnitId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BOMUnitId"].ToString());
                    int N_ReqId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ReqId"].ToString());
                    string xAction = MasterTable.Rows[0]["x_Action"].ToString();
                    int N_LabourCostID = 0;
                    int N_MachineCostID = 0;
                    if (MasterTable.Columns.Contains("N_LabourCostID"))
                        N_LabourCostID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LabourCostID"].ToString());
                    if (MasterTable.Columns.Contains("N_MachineCostID"))
                        N_MachineCostID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_MachineCostID"].ToString());
                    int N_LocationID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LocationID"].ToString());
                    bool B_IsProcess = myFunctions.getBoolVAL(MasterTable.Rows[0]["B_IsProcess"].ToString());
                    if (MasterTable.Columns.Contains("N_LabourCostID"))
                        MasterTable.Columns.Remove("N_LabourCostID");
                    if (MasterTable.Columns.Contains("N_MachineCostID"))
                        MasterTable.Columns.Remove("N_MachineCostID");
                    MasterTable.Columns.Remove("N_BOMUnitId");
                    bool B_AddItem = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("825", "AddItem", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    SqlTransaction transaction = connection.BeginTransaction();
                    DocNo = MasterRow["X_ReferenceNo"].ToString();
                  if (!myFunctions.CheckActiveYearTransaction(nCompanyID,nFnYearID, DateTime.ParseExact(MasterTable.Rows[0]["D_Date"].ToString(), "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture), dLayer, connection, transaction))
                 
                  {
                   object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID="+nCompanyID+" and convert(date ,'" + MasterTable.Rows[0]["D_Date"].ToString() + "') between D_Start and D_End", connection, transaction);
                  if (DiffFnYearID != null)
                   {
                     MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
                    nFnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());
                   
                  }
                  else
                  {
                
                      return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
                   }
                 }
                 object B_YearEndProcess=dLayer.ExecuteScalar("Select B_YearEndProcess from Acc_FnYear Where N_CompanyID="+nCompanyID+" and convert(date ,'" + MasterTable.Rows[0]["D_Date"].ToString() + "') between D_Start and D_End", connection, transaction);
                 if(myFunctions.getBoolVAL(B_YearEndProcess.ToString()))
                 {
                     return Ok(_api.Error(User, "Year Closed"));
                 }


                    if (X_ReferenceNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", N_FormID);
                        Params.Add("N_YearID", nFnYearID);

                        while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_Assembly Where X_ReferenceNo ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_ReferenceNo = DocNo;


                        if (X_ReferenceNo == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate")); }
                        MasterTable.Rows[0]["X_ReferenceNo"] = X_ReferenceNo;

                    }
                    if (nAssemblyID > 0)
                    {
                        object result = dLayer.ExecuteScalar("[SP_BuildorUnbuild] 'delete'," + nAssemblyID.ToString() + "," + N_LocationID.ToString() + ",'PRODUCTION ORDER'", connection, transaction);
                        dLayer.ExecuteNonQuery("delete from Inv_OtherCost where  N_CompanyID=" +nCompanyID+ "and N_TransID=" + nAssemblyID + "and X_TransType='Build'", Params, connection, transaction);

                        if (result == null || myFunctions.getIntVAL(result.ToString()) < 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable To Save"));
                        }
                        // transaction.Commit();
                        // return Ok(_api.Success("Saved Successfully"));
                        // N_AssemblyID = 0;
                    }


                    if (B_AddItem)
                    {
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt.Columns.Add("N_ItemDetailsID");
                        dt.Columns.Add("N_CompanyID");
                        dt.Columns.Add("N_MainItemID");
                        dt.Columns.Add("N_ItemID");
                        dt.Columns.Add("N_Qty");
                        dt.Columns.Add("N_Type");

                        for (int i = 0; i < DetailTable.Rows.Count; i++)
                        {
                            if (myFunctions.getIntVAL(DetailTable.Rows[i]["n_ItemID"].ToString()) <= 0) continue;
                            string sql = "select count(1) from Inv_ItemDetails where N_CompanyID=" + nCompanyID + " and N_MainItemID=" + N_ItemID + " and N_ItemID=" + myFunctions.getIntVAL(DetailTable.Rows[i]["n_ItemID"].ToString()) + " and N_Type=1";
                            object Itemcount = dLayer.ExecuteScalar(sql, connection, transaction);
                            if (myFunctions.getIntVAL(Itemcount.ToString()) != 0) continue;
                            int type = 1;  //bom items
                            //object N_ItemDetailsID = 0;

                            DataRow row = dt.NewRow();
                            row["N_ItemDetailsID"] = 0;
                            row["N_CompanyID"] = nCompanyID;
                            row["N_MainItemID"] = N_ItemID;
                            row["N_ItemID"] = myFunctions.getIntVAL(DetailTable.Rows[i]["n_ItemID"].ToString());
                            row["N_Qty"] = myFunctions.getVAL(DetailTable.Rows[i]["n_Qty"].ToString());
                            row["N_Type"] = type;
                            dt.Rows.Add(row);

                        }
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            string DupCriteriaDetails = "N_CompanyID=" + nCompanyID + " and N_MainItemID=" + N_ItemID + " and N_ItemID=" + myFunctions.getIntVAL(dt.Rows[j]["n_ItemID"].ToString()) + " and N_Type=1";
                            int N_ItemDetailsID = dLayer.SaveDataWithIndex("Inv_ItemDetails", "N_ItemDetailsID", DupCriteriaDetails, "", j, dt, connection, transaction);





                        }
                    }
                    string qry = "select N_Qty from Inv_ItemUnit inner join Inv_ItemMaster on Inv_ItemUnit.N_ItemUnitID=Inv_ItemMaster.n_BOMUnitID  where Inv_ItemMaster.N_ItemID=" + N_ItemID + " and Inv_ItemUnit.N_CompanyID=" + nCompanyID + " and Inv_ItemUnit.N_ItemUnitID=" + N_BOMUnitId;
                    object BOMQty = dLayer.ExecuteScalar(qry, connection, transaction);
                    double TotalQty = myFunctions.getVAL(MasterTable.Rows[0]["N_Qty"].ToString());
                    if (BOMQty != null)
                        TotalQty = myFunctions.getVAL(BOMQty.ToString()) * TotalQty;
                    MasterTable.Rows[0]["n_Qty"] = TotalQty;
                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and X_ReferenceNo='" + X_ReferenceNo + "'";
                    nAssemblyID = dLayer.SaveData("Inv_Assembly", "N_AssemblyID", DupCriteria, "", MasterTable, connection, transaction);
                    if (nAssemblyID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save ");
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_AssemblyID"] = nAssemblyID;
                    }
                    if (N_ReqId > 0)
                        dLayer.ExecuteNonQuery("Update Inv_PRS Set  N_Processed=1 Where N_PRSID=" + N_ReqId + " and N_FnYearID=" + nFnYearID + " and N_CompanyID=" + nCompanyID.ToString(), connection, transaction);
                    //////////////////  Saving to Assembly Details Table and Stock Table

                    int n_AssemblyDetailsID = dLayer.SaveData("Inv_AssemblyDetails", "N_AssemblyDetailsID", DetailTable, connection, transaction);
                    if (n_AssemblyDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                    //////////////////  Saving to Other Cost
                     if (OtherCost.Rows.Count > 0)
                        {
                            foreach (DataRow var in OtherCost.Rows)
                            {
                                var["N_TransID"] = nAssemblyID;
                                var["X_TransType"] = xAction;
                              
                            }
                        int nOtherCostID = myFunctions.getIntVAL(OtherCost.Rows[0]["n_OtherCostID"].ToString());
                        dLayer.SaveData("Inv_OtherCost", "N_OtherCostID", OtherCost, connection, transaction);
                    }
                    //////////////////  Saving to scrap item details
                    if (ScrapDetails.Rows.Count > 0)
                    {
                        for (int j = 0; j < ScrapDetails.Rows.Count; j++)
                        {
                            ScrapDetails.Rows[j]["N_AssemblyID"] = nAssemblyID;
                        }
                        int nAssemblyDetailsID = dLayer.SaveData("Inv_AssemblyDetails", "N_AssemblyDetailsID", DetailTable, connection, transaction);
                        if (nAssemblyDetailsID <= 0)
                        {
                            transaction.Rollback();
                            return Ok("Unable to save");
                        }
                    }
                    if (N_LabourCostID != 0)
                    {
                        for (int k = 0; k < ProductionLabourCost.Rows.Count; k++)
                        {
                            ProductionLabourCost.Rows[k]["N_AssemblyID"] = nAssemblyID;
                        }
                        int N_TransID = dLayer.SaveData("Inv_ProductionCost", "N_TransID", DetailTable, connection, transaction);
                        if (N_TransID <= 0)
                        {
                            transaction.Rollback();
                            return Ok("Unable to save");
                        }


                    }
                    if (N_MachineCostID != 0)
                    {
                        for (int k = 0; k < ProductionMachineCost.Rows.Count; k++)
                        {
                            ProductionMachineCost.Rows[k]["N_AssemblyID"] = nAssemblyID;
                        }
                        int N_TransID = dLayer.SaveData("Inv_ProductionCost", "N_TransID", DetailTable, connection, transaction);
                        if (N_TransID <= 0)
                        {
                            transaction.Rollback();
                            return Ok("Unable to save");
                        }
                    }
                    if (B_IsProcess)
                    {
                        SortedList InsertParams = new SortedList(){
                    {"X_Task","insert"},
                    {"N_AssemblyID",nAssemblyID},
                    {"N_LocationID",N_LocationID} };//,


                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_BuildorUnbuild", InsertParams, connection, transaction);
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();

                            return Ok(_api.Error(User, "No Enough Quantity for Product"));
                        }
                    }

                    SortedList PostingParam = new SortedList();
                    PostingParam.Add("N_CompanyID", nCompanyID);
                    PostingParam.Add("X_InventoryMode", "PRODUCTION ORDER");
                    PostingParam.Add("N_InternalID", nAssemblyID);
                    PostingParam.Add("N_UserID", myFunctions.GetUserID(User).ToString());
                    PostingParam.Add("X_SystemName", "ERP Cloud");
                    try
                    {
                        dLayer.ExecuteNonQueryPro("SP_Acc_InventoryPosting", PostingParam, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, ex));
                    }




                    transaction.Commit();
                    return Ok(_api.Success("Saved Successfully"));

                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        [HttpGet("itemList")]
        public ActionResult ItemList(int nFnYearID, int n_LocationID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);

            string sqlCommandText = "Select * from vw_InvItem_Search  Where N_CompanyID=@nCompanyID and ([Item Class]='Stock Item' or [Item Class]='Assembly Item') and B_InActive=0";
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
                return Ok(_api.Error(User, e));
            }

        }
        [HttpGet("details")]
        public ActionResult ListDetails(string xReferenceNo, int nLocationID)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList QueryParamsList = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    QueryParamsList.Add("@nCompanyID", nCompanyID);
                    DataTable Master = new DataTable();
                    DataTable Details = new DataTable();
                    DataTable ScrapDetails = new DataTable();
                    DataTable otherCostDetails = new DataTable();
                    DataTable ProducionLabourCost = new DataTable();
                    DataTable ProductionMachineCost = new DataTable();
                    DataTable ItemStockUnit = new DataTable();
                    string Condition = "";
                    string _sqlQuery = "";
                    Condition = "Select * from vw_InvAssembly  Where N_CompanyID=" + nCompanyID + " and X_Action='Build' and X_ReferenceNo='" + xReferenceNo + "'";
                    Master = dLayer.ExecuteDataTable(Condition, QueryParamsList, connection);


                    int N_ItemID = myFunctions.getIntVAL(Master.Rows[0]["N_ItemID"].ToString());
                    string sql = "select N_BOMUnitId from Inv_ItemMaster where N_ItemID=" + N_ItemID;
                    object N_BOMUnitIdLoc = dLayer.ExecuteScalar(sql, QueryParamsList, connection);
                    int N_BOMUnitId = 0;
                    Master = myFunctions.AddNewColumnToDataTable(Master, "n_BOMUnitId", typeof(int), 0);
                    if (N_BOMUnitIdLoc != null)
                    {
                        N_BOMUnitId = myFunctions.getIntVAL(N_BOMUnitIdLoc.ToString());
                        Master.Rows[0]["N_BOMUnitId"] = N_BOMUnitId;
                        // Master = myFunctions.AddNewColumnToDataTable(Master, "n_BOMUnitId", typeof(int), 0);
                    }




                    string qry = "select N_Qty from Inv_ItemUnit inner join Inv_ItemMaster on Inv_ItemUnit.N_ItemUnitID=Inv_ItemMaster.n_BOMUnitID  where Inv_ItemMaster.N_ItemID=" + N_ItemID + " and Inv_ItemUnit.N_CompanyID=" + nCompanyID + " and Inv_ItemUnit.N_ItemUnitID=" + N_BOMUnitId;
                    object BOMQty = dLayer.ExecuteScalar(qry, QueryParamsList, connection);
                    if (BOMQty != null)
                    {
                        double quantity = myFunctions.getVAL(Master.Rows[0]["n_Qty"].ToString()) / myFunctions.getVAL(BOMQty.ToString());
                        Master.Rows[0]["n_Qty"] = quantity.ToString();
                    }
                    Master.AcceptChanges();
                    string StockQuerry = "select N_ItemID,dbo.SP_GenGetStock(Inv_StockMaster.N_ItemID," + nLocationID + ",'','Location') as N_CurrentStock from Inv_StockMaster group by N_ItemID having N_ItemID=" + N_ItemID;
                    DataTable ItemStock = dLayer.ExecuteDataTable(StockQuerry, QueryParamsList, connection);
                    ItemStock.AcceptChanges();


                    if (ItemStock.Rows.Count > 0)
                    {
                        string sql89 = "select X_ItemUnit from Inv_ItemUnit inner join Inv_ItemMaster on Inv_ItemMaster.N_ItemUnitID=Inv_ItemUnit.N_ItemUnitID where Inv_ItemMaster.N_ItemID= " + N_ItemID;

                        ItemStockUnit = dLayer.ExecuteDataTable(sql89, QueryParamsList, connection);
                        ItemStockUnit.AcceptChanges();

                    }

                    int N_AssemblyID = myFunctions.getIntVAL(Master.Rows[0]["N_AssemblyID"].ToString());

                    _sqlQuery = "Select * from vw_InvAssemblyDetails Where N_CompanyID=" + nCompanyID + " and N_AssemblyID=" + N_AssemblyID + " and N_Type=1";
                    Details = dLayer.ExecuteDataTable(_sqlQuery, QueryParamsList, connection);


                    string sql4 = "Select * from vw_InvAssemblyDetails Where N_CompanyID=" + nCompanyID + " and N_AssemblyID=" + N_AssemblyID + " and N_Type=2";
                    ScrapDetails = dLayer.ExecuteDataTable(sql4, QueryParamsList, connection);

                    string sqlOtherCost = "select * from vw_Inv_OtherCost where N_CompanyID=" + nCompanyID + " and N_TransID=" + N_AssemblyID + " and X_TransType='Build'";
                    otherCostDetails = dLayer.ExecuteDataTable(sqlOtherCost, QueryParamsList, connection);

                    string LabourCost = "Select * from Inv_ProductionCost where N_AssembleID=" + N_AssemblyID + " and N_FormTypeID=" + 1 + "";
                    ProducionLabourCost = dLayer.ExecuteDataTable(LabourCost, QueryParamsList, connection);
                    ProducionLabourCost.AcceptChanges();

                    string machinecost = "Select * from Inv_ProductionCost where N_AssembleID=" + N_AssemblyID + " and N_FormTypeID=" + 2 + "";
                    ProductionMachineCost = dLayer.ExecuteDataTable(machinecost, QueryParamsList, connection);
                    ProductionMachineCost.AcceptChanges();


                    Details = _api.Format(Details, "Details");
                    Master = _api.Format(Master, "Master");
                    ItemStock = _api.Format(ItemStock, "ItemStock");
                    ProducionLabourCost = _api.Format(ProducionLabourCost, "ProducionLabourCost");
                    ProductionMachineCost = _api.Format(ProductionMachineCost, "ProductionMachineCost");
                    ItemStockUnit = _api.Format(ItemStockUnit, "ItemStockUnit");
                    otherCostDetails = _api.Format(otherCostDetails, "otherCostDetails");

                    dt.Tables.Add(Details);
                    dt.Tables.Add(Master);
                    dt.Tables.Add(ItemStock);
                    dt.Tables.Add(ProductionMachineCost);
                    dt.Tables.Add(ProducionLabourCost);
                    dt.Tables.Add(ItemStockUnit);
                    dt.Tables.Add(otherCostDetails);
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpPost("saveRelease")]
        public ActionResult SaveDataRelease([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    DataTable OtherCost = ds.Tables["otherCostDetails"];
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    bool B_IsProcess = myFunctions.getBoolVAL(MasterTable.Rows[0]["B_IsProcess"].ToString());
                    int n_AssemblyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_AssemblyID"].ToString());
                    int N_locationID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_locationID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    int N_ReqId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ReqId"].ToString());
                    double nOtherCharges = myFunctions.getVAL(MasterTable.Rows[0]["N_OtherCharges"].ToString());
                    int n_FormID = 772;
                    string xAction = MasterTable.Rows[0]["x_Action"].ToString();
                    var ReleaseDate = MasterTable.Rows[0]["d_ReleaseDate"].ToString();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    SqlTransaction transaction = connection.BeginTransaction();


            //  if (!myFunctions.CheckActiveYearTransaction(nCompanyID,nFnYearID, Convert.ToDateTime(MasterTable.Rows[0]["d_ReleaseDate"].ToString()), dLayer, connection, transaction))
            //       {
            //        object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID="+nCompanyID+" and convert(date ,'" + MasterTable.Rows[0]["d_ReleaseDate"].ToString() + "') between D_Start and D_End", connection, transaction);
            //       if (DiffFnYearID != null)
            //        {
            //          MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
            //         nFnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());
                   
            //       }
            //       else
            //       {
                
            //           return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
            //        }
            //      }
            //      object B_YearEndProcess=dLayer.ExecuteScalar("Select B_YearEndProcess from Acc_FnYear Where N_CompanyID="+nCompanyID+" and convert(date ,'" + MasterTable.Rows[0]["d_ReleaseDate"].ToString() + "') between D_Start and D_End", connection, transaction);
            //      if(myFunctions.getBoolVAL(B_YearEndProcess.ToString()))
            //      {
            //          return Ok(_api.Error(User, "Year Closed"));
            //      }



                    if (!B_IsProcess)
                    {  
                        object result = dLayer.ExecuteScalar("[SP_BuildorUnbuild] 'deleteAdd'," + n_AssemblyID.ToString() + "," + N_locationID.ToString() + ",'PRODUCTION RELEASE'", connection, transaction);
                        if (result == null || myFunctions.getIntVAL(result.ToString()) < 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to edit"));

                        }

                        //N_AssemblyID = 0;
                    }
                    string qry = "";
                    if (B_IsProcess)
                    {

                        qry = "update Inv_Assembly set B_IsProcess=0, D_ReleaseDate='" + ReleaseDate + "', N_OtherCharges=" + nOtherCharges + "where N_AssemblyID=" + n_AssemblyID + " and N_CompanyID=" + nCompanyID;
                    }
                    else
                    {

                        qry = "update Inv_Assembly set B_IsProcess=0, D_ReleaseDate='" + ReleaseDate + "', N_OtherCharges=" + nOtherCharges + " where N_AssemblyID=" + n_AssemblyID + " and N_CompanyID=" + nCompanyID;
                    }
                    object Result = dLayer.ExecuteNonQuery(qry, Params, connection, transaction);

                    if (myFunctions.getIntVAL(Result.ToString()) <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");

                    }
                    string qry1 = "update Inv_AssemblyDetails set B_IsProcess=0  where N_AssemblyID=" + n_AssemblyID + " and N_CompanyID=" + nCompanyID ;
                    object Result1 = dLayer.ExecuteNonQuery(qry1, Params, connection, transaction);
                    if (myFunctions.getIntVAL(Result1.ToString()) <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");

                    }
                    object Res = null;
                    //update status at Inv_prs
                    if (N_ReqId > 0)    //N_Processed=1 if processed, N_Processed=2 if production completed.
                        Res = dLayer.ExecuteNonQuery("Update Inv_PRS Set  N_Processed=2 Where N_PRSID=" + N_ReqId + " and N_FnYearID=" + nFnYearID + " and N_CompanyID=" + nCompanyID.ToString(), Params, connection, transaction);


                    //if(B_IsProcess)
                    dLayer.ExecuteNonQuery("[SP_BuildorUnbuild] 'insertAdd'," + n_AssemblyID.ToString() + "," + N_locationID, Params, connection, transaction);
                    dLayer.ExecuteNonQuery("delete from Inv_OtherCost where  N_CompanyID=" +nCompanyID+ "and N_TransID=" + n_AssemblyID + "and X_TransType='Build'", Params, connection, transaction);
                    dLayer.ExecuteNonQuery("SP_Acc_InventoryPosting " + nCompanyID.ToString() + ",'PRODUCTION RELEASE'," + n_AssemblyID.ToString() + "," + myFunctions.GetUserID(User).ToString() + ",'" + System.Environment.MachineName + "'", Params, connection, transaction);
                     if (OtherCost.Rows.Count > 0)
                        {
                            foreach (DataRow var in OtherCost.Rows)
                            {
                                var["N_TransID"] = n_AssemblyID;
                                var["X_TransType"] = xAction;
                            }
                        int nOtherCostID = myFunctions.getIntVAL(OtherCost.Rows[0]["n_OtherCostID"].ToString());
                        dLayer.SaveData("Inv_OtherCost", "N_OtherCostID", OtherCost, connection, transaction);
                    }


                    transaction.Commit();
                    return Ok(_api.Success("Saved Successfully"));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }

        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nAssemblyID, int nLocationID, bool b_isProcessed)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    if (b_isProcessed == true)
                    {
                        SortedList DeleteParams = new SortedList()
                    {
                    {"X_Task","delete"},
                    { "N_AssemblyID",nAssemblyID},
                    { "N_LocationID",nLocationID}

                    };  
                        dLayer.ExecuteNonQuery("delete from Inv_OtherCost where  N_CompanyID=" +nCompanyID+ "and N_TransID=" + nAssemblyID + "and N_FormID="+N_FormID, DeleteParams, connection, transaction);
                        int Results = dLayer.ExecuteNonQueryPro("SP_BuildorUnbuild", DeleteParams, connection, transaction);

                        if (Results <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to delete Purchase"));
                        }
                        transaction.Commit();
                        return Ok(_api.Success(" Production Order deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Production already released"));


                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
        [HttpDelete("deleteRelease")]
        public ActionResult DeleteDataRelease(int nAssemblyID, int nLocationID)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int n_FormID = 772;
                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID", nCompanyID);

                        SortedList DeleteParams = new SortedList()
                    {
                    {"X_Task","deleteAdd"},
                    { "N_AssemblyID",nAssemblyID},
                    { "N_LocationID",nLocationID},
                    {"@X_TransType","PRODUCTION RELEASE"}
                    };
                        int Results = dLayer.ExecuteNonQueryPro("SP_BuildorUnbuild", DeleteParams, connection, transaction);

                        if (Results <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to delete Purchase"));
                        }

                        string qry = "update Inv_Assembly set B_IsProcess=1, D_ReleaseDate=NULL where N_AssemblyID=" + nAssemblyID + " and N_CompanyID=" + nCompanyID;
                        dLayer.ExecuteNonQuery(qry, Params, connection, transaction);
                        string qry1 = "update Inv_AssemblyDetails set B_IsProcess=1  where N_AssemblyID=" + nAssemblyID + " and N_CompanyID=" + nCompanyID;
                        dLayer.ExecuteNonQuery(qry1, Params, connection, transaction);
                        dLayer.ExecuteNonQuery("update Inv_OtherCost set N_FormID=54 where N_CompanyID=" +nCompanyID+ "and N_TransID=" + nAssemblyID + "and N_FormID="+n_FormID, DeleteParams, connection, transaction);


                        transaction.Commit();
                        return Ok(_api.Success(" Production release deleted"));
                   
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }














    }
}
























