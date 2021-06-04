using System.Collections.Generic;
using SmartxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System.IO;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("products")]
    [ApiController]
    public class Inv_ItemMaster : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly string reportPath;

        public Inv_ItemMaster(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            reportPath = conf.GetConnectionString("ReportPath");
        }

        //GET api/Projects/list
        [HttpGet("list")]
        public ActionResult GetAllItems(string query, int PageSize, int Page, int nCategoryID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string qry = "";
            string Category = "";
            if (query != "" && query != null)
            {
                qry = " and (Description like @query or [Item Code] like @query) ";
                Params.Add("@query", "%" + query + "%");
            }
            if (nCategoryID > 0)
                Category = " and vw_InvItem_Search.N_CategoryID =" + nCategoryID;


            string pageQry = "DECLARE @PageSize INT, @Page INT Select @PageSize=@PSize,@Page=@Offset;WITH PageNumbers AS(Select ROW_NUMBER() OVER(ORDER BY vw_InvItem_Search.N_ItemID) RowNo,";
            string pageQryEnd = ") SELECT * FROM    PageNumbers WHERE   RowNo BETWEEN((@Page -1) *@PageSize + 1)  AND(@Page * @PageSize) order by N_ItemID DESC";

            // string sqlComandText = " * from Vw_InvItem_Search where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 " + qry;

            string sqlComandText = "  vw_InvItem_Search.*,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) as N_SellingPrice,Inv_ItemUnit.N_SellingPrice as N_SellingPrice2 FROM vw_InvItem_Search LEFT OUTER JOIN " +
             " Inv_ItemUnit ON vw_InvItem_Search.N_StockUnitID = Inv_ItemUnit.N_ItemUnitID AND vw_InvItem_Search.N_CompanyID = Inv_ItemUnit.N_CompanyID where vw_InvItem_Search.N_CompanyID=@p1 and vw_InvItem_Search.B_Inactive=@p2 and vw_InvItem_Search.[Item Code]<> @p3 and vw_InvItem_Search.N_ItemTypeID<>@p4  and vw_InvItem_Search.N_ItemID=Inv_ItemUnit.N_ItemID " + qry + Category;

            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", 0);
            Params.Add("@p3", "001");
            Params.Add("@p4", 1);
            Params.Add("@PSize", PageSize);
            Params.Add("@Offset", Page);



            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = pageQry + sqlComandText + pageQryEnd;
                    dt = dLayer.ExecuteDataTable(sql, Params, connection);
                }
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
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }

        }

        [HttpGet("dashboardList")]
        public ActionResult GetDashboardList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (Description like '%" + xSearchkey + "%' or [Item Code] like '%" + xSearchkey + "%' or Category like '%" + xSearchkey + "%' or [Item Class] like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ItemID desc,[Item Code] desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "itemClass":
                        xSortBy = "[Item Class] " + xSortBy.Split(" ")[1];
                        break;
                    case "itemCode":
                        xSortBy = "N_ItemID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;

            }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_InvItem_Search where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_InvItem_Search where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 " + Searchkey + " and [Item Code] not in (select top(" + Count + ") [Item Code] from Vw_InvItem_Search where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 " + Searchkey + xSortBy + " ) " + xSortBy;


            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", 0);
            Params.Add("@p3", "001");
            Params.Add("@p4", 1);

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    string sqlCommandCount = "select count(*) as N_Count  from Vw_InvItem_Search where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 " + Searchkey;
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
                return Ok(_api.Error(e));
            }

        }





        [HttpGet("details")]
        public ActionResult GetItemDetails(string xItemCode, int nLocationID, int nBranchID)
        {
            DataTable dt = new DataTable();
            DataTable dt_LocStock = new DataTable();
            DataTable Images = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@xItemCode", xItemCode);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string _sqlQuery = "SELECT * from vw_InvItemMaster where X_ItemCode=@xItemCode and N_CompanyID=@nCompanyID";

                    dt = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    int N_BranchID = nBranchID;
                    int N_ItemID = myFunctions.getIntVAL(dt.Rows[0]["N_ItemID"].ToString());
                    QueryParams.Add("@nItemID", dt.Rows[0]["N_ItemID"].ToString());
                    QueryParams.Add("@nLocationID", nLocationID);
                    QueryParams.Add("@xStockUnit", dt.Rows[0]["X_StockUnit"].ToString());
                    double OpeningStock = 0;
                    object res = dLayer.ExecuteScalar("Select SUM(Inv_StockMaster.N_OpenStock) from vw_InvItemMaster Left Outer Join Inv_StockMaster On vw_InvItemMaster.N_ItemID=Inv_StockMaster.N_ItemID and Inv_StockMaster.X_Type = 'Opening' Where vw_InvItemMaster.N_ItemID=@nItemID and vw_InvItemMaster.N_CompanyID =@nCompanyID and N_LocationID=@nLocationID and (B_IsIMEI<>1 OR B_IsIMEI IS NULL)", QueryParams, connection);
                    if (res != null)
                        OpeningStock = myFunctions.getVAL(res.ToString());

                    double UnitQty = 0;
                    double Cost = 0;
                    res = dLayer.ExecuteScalar("Select N_Qty from Inv_ItemUnit Where N_CompanyID=@nCompanyID and N_ItemID =@nItemID and X_ItemUnit=@xStockUnit", QueryParams, connection);
                    if (res != null)
                        UnitQty = myFunctions.getVAL(res.ToString());

                    double BeginStock = OpeningStock / UnitQty;
                    res = dLayer.ExecuteScalar("Select N_LPrice from vw_InvItemMaster Left Outer Join Inv_StockMaster On vw_InvItemMaster.N_ItemID=Inv_StockMaster.N_ItemID and Inv_StockMaster.X_Type = 'Opening' Where vw_InvItemMaster.N_ItemID=@nItemID and vw_InvItemMaster.N_CompanyID =@nCompanyID and N_LocationID=@nLocationID and (B_IsIMEI<>1 OR B_IsIMEI IS NULL)", QueryParams, connection);
                    if (res != null)
                        Cost = myFunctions.getVAL(res.ToString()) * UnitQty;

                    dt = myFunctions.AddNewColumnToDataTable(dt, "N_OpeningStock", typeof(string), BeginStock.ToString(myCompanyID.DecimalPlaceString));

                    res = dLayer.ExecuteScalar("Select dbo.SP_StockByStockUnit(@nCompanyID,@nItemID,@nLocationID) As [Current Stock] from Inv_StockMaster where N_ItemId=@nItemID and N_CompanyID=@nCompanyID and N_LocationID=@nLocationID order by N_StockId desc", QueryParams, connection);
                    if (res != null)
                        dt = myFunctions.AddNewColumnToDataTable(dt, "N_CurrentStock", typeof(string), myFunctions.getVAL(res.ToString()).ToString(myCompanyID.DecimalPlaceString));
                    else
                        dt = myFunctions.AddNewColumnToDataTable(dt, "N_CurrentStock", typeof(string), "0.00");

                    object inStocks = dLayer.ExecuteScalar("Select N_ItemID From vw_InvStock_Status Where N_ItemID=@nItemID and (Type<>'O' and Type<>'PO' and Type<>'SO') and N_CompanyID=@nCompanyID", QueryParams, connection);
                    bool b_InStocks = true;
                    if (inStocks == null)
                        b_InStocks = false;

                    dt = myFunctions.AddNewColumnToDataTable(dt, "b_TxnDone", typeof(bool), b_InStocks);

                    SortedList WhParam = new SortedList(){
                                    {"N_CompanyID", myFunctions.GetCompanyID(User)},
                                    {"N_ItemID", N_ItemID},
                                    {"N_BranchID", N_BranchID},
                                    };


                    DataTable whDt = dLayer.ExecuteDataTablePro("Sp_Inv_ItemMaster_Disp ", WhParam, connection);

                    dt = myFunctions.AddNewColumnToDataTable(dt, "warehouseList", typeof(DataTable), whDt);

                    string sqlQuery = "SELECT     Inv_Location.X_LocationName, dbo.SP_LocationStock(Inv_ItemMasterWHLink.N_ItemID, Inv_Location.N_LocationID) AS N_Stock, vw_InvItemMaster.X_StockUnit,vw_InvItemMaster.N_StockUnitID FROM Inv_ItemMasterWHLink INNER JOIN  Inv_Location ON Inv_ItemMasterWHLink.N_WarehouseID = Inv_Location.N_LocationID AND Inv_ItemMasterWHLink.N_CompanyID = Inv_Location.N_CompanyID LEFT OUTER JOIN  vw_InvItemMaster ON Inv_ItemMasterWHLink.N_ItemID = vw_InvItemMaster.N_ItemID AND Inv_ItemMasterWHLink.N_CompanyID = vw_InvItemMaster.N_CompanyID where Inv_ItemMasterWHLink.N_ItemID=" + N_ItemID + " and Inv_ItemMasterWHLink.N_CompanyID=" + companyid;
                    dt_LocStock = dLayer.ExecuteDataTable(sqlQuery, QueryParams, connection);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "locationStockList", typeof(DataTable), dt_LocStock);

                    string sqlQuery1 = "Select Isnull(Sum(N_Qty),0) from Inv_PurchaseOrderDetails inner join Inv_PurchaseOrder On Inv_PurchaseOrderDetails.N_POrderID =Inv_PurchaseOrder.N_POrderID Where B_CancelOrder=0 and Inv_PurchaseOrderDetails.N_ItemID=" + N_ItemID + "and Inv_PurchaseOrderDetails.N_BranchID= " + N_BranchID + " and Inv_PurchaseOrderDetails.N_CompanyID=" + companyid + " and Inv_PurchaseOrder.N_Processed=0";
                    object purchaseQty = dLayer.ExecuteScalar(sqlQuery1, QueryParams, connection);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "n_POrderQty", typeof(string), purchaseQty);

                    //Image Retriving
                    string _sqlImageQuery = "SELECT * from Inv_DisplayImages where N_ItemID=" + dt.Rows[0]["N_ItemID"].ToString() + " and N_CompanyID=" + companyid;
                    Images = dLayer.ExecuteDataTable(_sqlImageQuery, QueryParams, connection);
                    if (Images.Rows.Count > 0)
                    {
                        Images.Columns.Add("I_Image", typeof(System.String));
                        foreach (DataRow var in Images.Rows)
                        {
                            var path = var["X_ImageLocation"].ToString() + "\\" + var["X_ImageName"].ToString();
                            if (System.IO.File.Exists(path))
                            {
                                Byte[] bytes = System.IO.File.ReadAllBytes(path);
                                var["I_Image"] = Convert.ToBase64String(bytes);

                            }
                        }
                        

                    }
                }
                dt.AcceptChanges();
                dt = _api.Format(dt);
                DataSet dataSet = new DataSet();
                        dt = _api.Format(dt, "details");
                        Images = _api.Format(Images, "Images");
                        dataSet.Tables.Add(dt);
                        dataSet.Tables.Add(Images);


                return Ok(_api.Success(dataSet));

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
                DataTable MasterTable, GeneralTable, StockUnit, SalesUnit, PurchaseUnit, AddUnit1, AddUnit2, LocationList, CategoryList;
                DataTable POS = ds.Tables["Pos"];
                DataTable ECOM = ds.Tables["Ecom"];
                MasterTable = ds.Tables["master"];
                GeneralTable = ds.Tables["general"];
                StockUnit = ds.Tables["stockUnit"];
                SalesUnit = ds.Tables["salesUnit"];
                PurchaseUnit = ds.Tables["purchaseUnit"];
                AddUnit1 = ds.Tables["addUnit1"];
                AddUnit2 = ds.Tables["addUnit2"];
                LocationList = ds.Tables["warehouseDetails"];
                CategoryList = ds.Tables["categoryListDetails"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyId"].ToString());
                int N_ItemID=0;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string ItemCode = "";
                    ItemCode = MasterTable.Rows[0]["X_ItemCode"].ToString();
                    if (ItemCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterTable.Rows[0]["N_CompanyId"].ToString());
                        Params.Add("N_YearID", GeneralTable.Rows[0]["N_FnYearId"].ToString());
                        Params.Add("N_FormID", 53);
                        ItemCode = dLayer.GetAutoNumber("Inv_ItemMaster", "X_ItemCode", Params, connection, transaction);
                        if (ItemCode == "") { transaction.Rollback(); return Ok(_api.Warning("Unable to generate product Code")); }
                        MasterTable.Rows[0]["X_ItemCode"] = ItemCode;
                    }

                    string image = MasterTable.Rows[0]["i_Image"].ToString();
                    Byte[] imageBitmap = new Byte[image.Length];
                    imageBitmap = Convert.FromBase64String(image);
                    MasterTable.Columns.Remove("i_Image");

                    string DupCriteria = "N_CompanyID=" + myFunctions.GetCompanyID(User) + " and X_ItemCode='" + ItemCode + "'";
                    N_ItemID = dLayer.SaveData("Inv_ItemMaster", "N_ItemID", DupCriteria, "", MasterTable, connection, transaction);
                    if (N_ItemID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }

                    if (image.Length > 0)
                        dLayer.SaveImage("Inv_ItemMaster", "i_Image", imageBitmap, "N_ItemID", N_ItemID, connection, transaction);

                    foreach (DataRow var in StockUnit.Rows) var["n_ItemID"] = N_ItemID;
                    foreach (DataRow var in SalesUnit.Rows) var["n_ItemID"] = N_ItemID;
                    foreach (DataRow var in PurchaseUnit.Rows) var["n_ItemID"] = N_ItemID;
                    foreach (DataRow var in AddUnit1.Rows) var["n_ItemID"] = N_ItemID;
                    foreach (DataRow var in AddUnit2.Rows) var["n_ItemID"] = N_ItemID;

                    int BaseUnitID = dLayer.SaveData("Inv_ItemUnit", "N_ItemUnitID", StockUnit, connection, transaction);
                    dLayer.ExecuteNonQuery("update  Inv_ItemMaster set N_ItemUnitID=" + BaseUnitID + " ,N_StockUnitID =" + BaseUnitID + " where N_ItemID=" + N_ItemID + " and N_CompanyID=N_CompanyID", Params, connection, transaction);

                    int N_SalesUnitID=0,N_PurchaseUnitID=0,N_AddUnitID1=0,N_AddUnitID2=0;

                    foreach (DataRow var in SalesUnit.Rows) var["n_BaseUnitID"] = BaseUnitID;
                    foreach (DataRow var in PurchaseUnit.Rows) var["n_BaseUnitID"] = BaseUnitID;
                    foreach (DataRow var in AddUnit1.Rows) var["n_BaseUnitID"] = BaseUnitID;
                    foreach (DataRow var in AddUnit2.Rows) var["n_BaseUnitID"] = BaseUnitID;

                    string xBaseUnit=StockUnit.Rows[0]["X_ItemUnit"].ToString();

                    //Purchase Unit
                    if(PurchaseUnit.Rows.Count>0)
                    {
                        if(PurchaseUnit.Rows[0]["X_ItemUnit"].ToString()==xBaseUnit)
                            N_PurchaseUnitID=BaseUnitID;
                        else
                            N_PurchaseUnitID = dLayer.SaveData("Inv_ItemUnit", "N_ItemUnitID", PurchaseUnit, connection, transaction);
                    }
                    else
                    {
                        N_PurchaseUnitID=BaseUnitID;
                    }

                    //Sales Unit
                    if(SalesUnit.Rows.Count>0)
                    {
                        if(SalesUnit.Rows[0]["X_ItemUnit"].ToString()==xBaseUnit)
                            N_SalesUnitID=BaseUnitID;
                        else if(SalesUnit.Rows[0]["X_ItemUnit"].ToString()==PurchaseUnit.Rows[0]["X_ItemUnit"].ToString())
                            N_SalesUnitID=N_PurchaseUnitID;
                        else                      
                            N_SalesUnitID = dLayer.SaveData("Inv_ItemUnit", "N_ItemUnitID", SalesUnit, connection, transaction);
                    }
                    else
                        N_SalesUnitID=BaseUnitID;

                    //Additional Unit1
                    if(AddUnit1.Rows.Count>0)
                    {
                        if(AddUnit1.Rows[0]["X_ItemUnit"].ToString()==xBaseUnit)
                            N_AddUnitID1=BaseUnitID;
                        else if(AddUnit1.Rows[0]["X_ItemUnit"].ToString()==PurchaseUnit.Rows[0]["X_ItemUnit"].ToString())
                            N_AddUnitID1=N_PurchaseUnitID;
                        else if(AddUnit1.Rows[0]["X_ItemUnit"].ToString()==SalesUnit.Rows[0]["X_ItemUnit"].ToString())
                            N_AddUnitID1=N_SalesUnitID;
                        else
                            N_AddUnitID1 = dLayer.SaveData("Inv_ItemUnit", "N_ItemUnitID", AddUnit1, connection, transaction);
                    }
                    
                    //Additional Unit2
                    if(AddUnit2.Rows.Count>0)
                    {
                        if(AddUnit2.Rows[0]["X_ItemUnit"].ToString()==xBaseUnit)
                            N_AddUnitID2=BaseUnitID;
                        else if(AddUnit2.Rows[0]["X_ItemUnit"].ToString()==PurchaseUnit.Rows[0]["X_ItemUnit"].ToString())
                            N_AddUnitID2=N_PurchaseUnitID;
                        else if(AddUnit2.Rows[0]["X_ItemUnit"].ToString()==SalesUnit.Rows[0]["X_ItemUnit"].ToString())
                            N_AddUnitID2=N_SalesUnitID;
                        else
                            N_AddUnitID2 = dLayer.SaveData("Inv_ItemUnit", "N_ItemUnitID", AddUnit2, connection, transaction);
                    }

                    dLayer.ExecuteNonQuery("update  Inv_ItemMaster set N_SalesUnitID=" + N_SalesUnitID + ",N_PurchaseUnitID=" + N_PurchaseUnitID + " where N_ItemID=" + N_ItemID + " and N_CompanyID=N_CompanyID", Params, connection, transaction);
                    if (BaseUnitID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }

                    dLayer.DeleteData("Inv_ItemMasterWHLink", "N_ItemID", N_ItemID, "", connection, transaction);
                    if (LocationList.Rows.Count > 0)
                    {
                        foreach (DataRow dRow in LocationList.Rows)
                        {
                            dRow["N_ItemID"] = N_ItemID;
                        }
                        LocationList.AcceptChanges();
                        dLayer.SaveData("Inv_ItemMasterWHLink", "N_RowID", LocationList, connection, transaction);
                    }
                    if (CategoryList.Rows.Count > 0)
                    {
                        foreach (DataRow dRow in CategoryList.Rows)
                        {
                            dRow["N_ItemID"] = N_ItemID;
                        }
                        CategoryList.AcceptChanges();
                        dLayer.SaveData("Inv_ItemCategoryDisplayMaster", "N_CategoryListID", CategoryList, connection, transaction);
                    }
                    //Saving Display Images
                    object obj = dLayer.ExecuteScalar("Select X_Value  From Gen_Settings Where N_CompanyID=" + nCompanyID + " and X_Group='188' and X_Description='EmpDocumentLocation'", connection, transaction);
                    string DocumentPath = obj != null && obj.ToString() != "" ? obj.ToString() : this.reportPath;
                    DocumentPath = DocumentPath + "DisplayImages";
                    System.IO.Directory.CreateDirectory(DocumentPath);


                    if (POS.Rows.Count > 0)
                    {
                        POS.Columns.Add("X_ImageName", typeof(System.String));
                        POS.Columns.Add("X_ImageLocation", typeof(System.String));
                        POS.Columns.Add("N_ImageID", typeof(System.Int32));


                        foreach (DataRow dRow in POS.Rows)
                        {
                            writefile(dRow["I_Image"].ToString(), DocumentPath, ItemCode);
                            dRow["X_ImageName"] = ItemCode + ".jpg";
                            dRow["X_ImageLocation"] = DocumentPath;
                            dRow["N_ItemID"] = N_ItemID;

                        }
                        POS.Columns.Remove("I_Image");
                        dLayer.SaveData("Inv_DisplayImages", "N_ImageID", POS, connection, transaction);

                    }
                    // if (ECOM.Rows.Count > 0)
                    // {
                    //     ECOM.Columns.Add("X_ImageName", typeof(System.String));
                    //     ECOM.Columns.Add("X_ImageLocation", typeof(System.String));
                    //     ECOM.Columns.Add("N_ImageID", typeof(System.Int32));
                    //     foreach (DataRow dRow in ECOM.Rows)
                    //     {
                    //         writefile(dRow["I_Image"].ToString(), DocumentPath, ItemCode);
                    //         dRow["X_ImageName"] = ItemCode + ".jpg";
                    //         dRow["X_ImageLocation"] = DocumentPath;
                   //          dRow["N_ItemID"] = N_ItemId;

                    //     }
                    //     ECOM.Columns.Remove("I_Image");
                    //     dLayer.SaveData("Inv_DisplayImages", "N_ImageID", ECOM, connection, transaction);

                    // }

                    transaction.Commit();
                }
                return Ok(_api.Success("Product Saved"));

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
        public bool writefile(string FileString, string Path, string Name)
        {

            string imageName = "\\" + Name + ".jpg";
            string imgPath = Path + imageName;

            byte[] imageBytes = Convert.FromBase64String(FileString);

            System.IO.File.WriteAllBytes(imgPath, imageBytes);
            return true;


        }

        // [HttpGet("saveimages")]
        // public ActionResult SaveDisplayImages([FromBody] DataSet ds)
        // {
        //     DataTable POS = ds.Tables["POS"];
        //     DataTable ECOM = ds.Tables["ECOM"];
        //     object Result = 0;
        //     string path = "";
        //     string s = "";
        //     using (SqlConnection connection = new SqlConnection(connectionString))
        //     {
        //         connection.Open();
        //         SqlTransaction transaction = connection.BeginTransaction();
        //         int nCompanyID = myFunctions.GetCompanyID(User);
        //         object obj = dLayer.ExecuteScalar("Select X_Value  From Gen_Settings Where N_CompanyID=" + nCompanyID + " and X_Group='188' and X_Description='EmpDocumentLocation'", connection, transaction);
        //         string DocumentPath = obj != null && obj.ToString() != "" ? obj.ToString() : this.reportPath;

        //         if (POS.Rows.Count > 0)
        //         {
        //             DocumentPath = DocumentPath + "/DisplayImages";
        //             System.IO.Directory.CreateDirectory(DocumentPath);

        //         }
        //         foreach (DataRow dRow in POS.Rows)
        //         {
        //             writefile(dRow["I_Image"].ToString(), DocumentPath,ItemCode);

        //         }
        //     }


        //     return Ok();
        // }



        //GET api/Projects/list
        [HttpGet("class")]
        public ActionResult GetItemClass()
        {
            DataTable dt = new DataTable();

            string sqlComandText = "select * from Inv_ItemClass order by N_Order ASC";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlComandText, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("no result found"));
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



        [HttpGet("dummy")]
        public ActionResult GetPurchaseInvoiceDummy(int? Id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlCommandText = "select * from Inv_ItemMaster where N_ItemID=@p1";
                    SortedList mParamList = new SortedList() { { "@p1", Id } };
                    DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList, connection);
                    masterTable = _api.Format(masterTable, "master");

                    string sqlCommandText2 = "select * from Inv_ItemMaster where N_ItemID=@p1";
                    SortedList dParamList = new SortedList() { { "@p1", Id } };
                    DataTable detailTable = dLayer.ExecuteDataTable(sqlCommandText2, dParamList, connection);
                    detailTable = _api.Format(detailTable, "details");

                    if (detailTable.Rows.Count == 0) { return Ok(new { }); }
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add(masterTable);
                    dataSet.Tables.Add(detailTable);

                    return Ok(dataSet);
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nItemID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    SqlTransaction transaction = connection.BeginTransaction();

                    object N_Result = dLayer.ExecuteScalar("Select B_YearEndProcess from Acc_FnYear Where N_CompanyID= " + nCompanyID + " and N_FnYearID= " + nFnYearID, connection, transaction);
                    if (myFunctions.getIntVAL(myFunctions.getBoolVAL(N_Result.ToString())) == 1)
                    {
                        return Ok(_api.Error("Year Closed , Unable to delete product."));
                    }

                    dLayer.DeleteData("Inv_ItemDetails", "N_MainItemID", nItemID, "", connection, transaction);
                    Results = dLayer.DeleteData("Inv_ItemMaster", "N_ItemID", nItemID, "", connection, transaction);
                    if (Results > 0)
                    {

                        dLayer.ExecuteScalar("delete from  Inv_ItemUnit  Where N_ItemID=" + nItemID + " and N_CompanyID=" + nCompanyID, connection, transaction);
                        transaction.Commit();
                        return Ok(_api.Success("Product deleted"));
                    }
                    else
                    {
                        transaction.Rollback();

                        return Ok(_api.Error("Unable to delete product category"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error("Can't be delete,It has been used!"));
            }


        }



        // public void CopyFiles(IDataAccessLayer dLayer, string filename, string subject, int folderId, bool overwriteexisting, string category, string fileData, string destpath, string filecode, int attachID, int FormID, string strExpireDate, int remCategoryId, int transId, int partyID, int settingsId, ClaimsPrincipal User, SqlTransaction transaction, SqlConnection connection)
        // {
        //     try
        //     {
        //         DateTime dtExpire = new DateTime();
        //         if (strExpireDate != "")
        //             dtExpire = Convert.ToDateTime(strExpireDate);
        //         int nUserID = myFunctions.GetUserID(User);
        //         int nCompanyID = myFunctions.GetCompanyID(User);
        //         object N_Result = dLayer.ExecuteScalar("Select 1 from DMS_MasterFiles Where X_FileCode ='" + filecode + "' and N_CompanyID= " + nCompanyID + " and N_FormID=" + FormID, connection, transaction);
        //         if (N_Result != null)
        //             return;
        //         int FileID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select ISNULL(max(N_FileID),0)+1 from DMS_MasterFiles where N_CompanyID=" + nCompanyID, connection, transaction).ToString());

        //         //  FileInfo flinfo = new FileInfo(fls);
        //         string extension = System.IO.Path.GetExtension(filename);
        //         string refname = filecode + extension;
        //         if (strExpireDate != "")
        //         {
        //             dLayer.ExecuteNonQuery("insert into DMS_MasterFiles(N_CompanyID,N_FileID,X_FileCode,X_Name,X_Title,X_Contents,N_FolderID,N_UserID,X_refName,N_AttachmentID,N_FormID,D_ExpiryDate,N_CategoryID,N_TransID)values(" + nCompanyID + "," + FileID + ",'" + filecode + "','" + filename + "','" + category + "','" + subject + "'," + folderId + "," + nUserID + ",'" + refname + "'," + attachID + "," + FormID + ",'" + dtExpire.ToString("dd/MMM/yyyy") + "'," + remCategoryId + "," + transId + ")", connection, transaction);
        //             int ReminderId = ReminderSave(dLayer, FormID, partyID, strExpireDate, subject, filename, remCategoryId, 1, settingsId, User, transaction, connection);
        //             dLayer.ExecuteNonQuery("update DMS_MasterFiles set N_ReminderID=" + ReminderId + " where N_FileID=" + FileID + " and N_CompanyID=" + nCompanyID, connection, transaction);
        //         }
        //         else
        //             dLayer.ExecuteNonQuery("insert into DMS_MasterFiles(N_CompanyID,N_FileID,X_FileCode,X_Name,X_Title,X_Contents,N_FolderID,N_UserID,X_refName,N_AttachmentID,N_FormID,N_TransID)values(" + nCompanyID + "," + FileID + ",'" + filecode + "','" + filename + "','" + category + "','" + subject + "'," + folderId + "," + nUserID + ",'" + refname + "'," + attachID + "," + FormID + "," + transId + ")", connection, transaction);
        //         // System.IO.File.Copy(sourcepath, destpath + refname, overwriteexisting);

        //         var base64Data = Regex.Match(fileData.ToString(), @"data:(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;
        //         byte[] FileBytes = Convert.FromBase64String(base64Data);
        //         File.WriteAllBytes(destpath + refname,
        //                            FileBytes);

        //     }
        //     catch (Exception ex)
        //     {

        //     }

        // }



    }


}