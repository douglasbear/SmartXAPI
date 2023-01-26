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
using System.Net;
using System.Web;
using System.Drawing.Imaging;
using ZXing;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


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
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly string TempFilesPath;

        public Inv_ItemMaster(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            TempFilesPath = conf.GetConnectionString("TempFilesPath");
        }

        //GET api/Projects/list
        [HttpGet("list")]
        public ActionResult GetAllItems(string query, int PageSize, int Page, int nCategoryID, string xClass, int nNotItemID, int nNotGridItemID, bool b_AllBranchData, bool partNoEnable, int nLocationID, bool isStockItem, bool isCustomerMaterial, int nItemUsedFor, bool isServiceItem, bool b_whGrn, bool b_PickList, int n_CustomerID, bool b_Asn, int nPriceListID, bool isSalesItems, bool isRentalItem, bool rentalItems, bool purchaseRentalItems,bool showStockInlist,int nitemType,bool isAssetItem)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string qry = "";
            string Category = "";
            string Condition = "";
            string xCriteria = "";
            string warehouseSql = "";
            string itemTypeCondition = "";
            string priceListCondition = "";
            string ownAssent = "";
            string RentalItem = "";
            string RentalPOItem = "";
            string xOrder = "";
            string xOrderNew = "";
            //nItemUsedFor -> 1-Purchase, 2-Sales, 3-Both, 4-Raw Material
             string showStock="";
             string otherItem="";
              string sqlComandText ="";

             if(showStockInlist)
             {
                showStock= "dbo.SP_GenGetStock(vw_InvItem_Search_cloud.N_ItemID," + nLocationID + ",'', 'location') As N_AvlStock,";
             }

            if (b_whGrn == true && n_CustomerID > 0)
            {
                warehouseSql = "and vw_InvItem_Search_cloud.N_ItemID in (select N_ItemID from  Vw_wh_AsnDetails_disp where N_CompanyID=@p1 and N_CustomerID=" + n_CustomerID + ")";
            }
            if (b_PickList == true && n_CustomerID > 0)
            {
                warehouseSql = "and vw_InvItem_Search_cloud.N_ItemID in (select N_ItemID from  vw_Wh_GRNDetails where N_CompanyID=@p1 and N_CustomerID=" + n_CustomerID + ")";
            }
            if (b_Asn == true && n_CustomerID > 0)
            {
                warehouseSql = "and vw_InvItem_Search_cloud.N_CustomerID =" + n_CustomerID + "";
            }

            if (query != "" && query != null)
            {
                if (partNoEnable)
                {
                    qry = " and (Description like @query or vw_InvItem_Search_cloud.[Part No] like @query) ";
                    Params.Add("@query", "%" + query + "%");
                }
                else
                {
                    qry = " and (Description like @query or [Item Code] like @query or vw_InvItem_Search_cloud.X_Barcode like @query or vw_InvItem_Search_cloud.[Part No] like @query) ";
                    Params.Add("@query", "%" + query + "%");
                }
            }
            if (nCategoryID > 0)
                Category = " and vw_InvItem_Search_cloud.N_CategoryID =" + nCategoryID;

            if (xClass == null) xClass = "";
            if (xClass != "")
                Condition = Condition + " and vw_InvItem_Search_cloud.[Item Class] in (" + xClass + ")";

            if (nNotItemID != 0)
                Condition = Condition + " and vw_InvItem_Search_cloud.N_ItemID<> " + nNotItemID;

            if (nNotGridItemID != 0)
                Condition = Condition + " and vw_InvItem_Search_cloud.N_ItemID<> " + nNotGridItemID;

            if (nLocationID != 0)
                Condition = Condition + "  and vw_InvItem_Search_cloud.N_ItemID in (Select N_ItemID from Inv_ItemMasterWHLink where N_CompanyID=@p1 and N_WarehouseID=" + nLocationID + " )  ";
            if (isStockItem)
                Condition = Condition + " and N_ClassID =2";
            if (isServiceItem)
                Condition = Condition + " and N_ClassID =4";
            if (isCustomerMaterial)
                itemTypeCondition = itemTypeCondition + " and N_ItemTypeID=5 ";
            if (isSalesItems)
                itemTypeCondition = itemTypeCondition + " and N_ItemTypeID<>5 ";
            if (isRentalItem)
                ownAssent = ownAssent + " and N_ItemTypeID=7 ";
            if (rentalItems)
                RentalItem = RentalItem + " and (N_ItemTypeID=7 or N_ItemTypeID=8 or N_ItemTypeID=9)";
            if (purchaseRentalItems)
                RentalPOItem = RentalPOItem + " and (N_ItemTypeID=9)";

            if (nItemUsedFor != 0)
            {
                if (nItemUsedFor == 1)
                    Condition = Condition + " and vw_InvItem_Search_cloud.B_CanBePurchased =1";
                else if (nItemUsedFor == 2)
                    Condition = Condition + " and vw_InvItem_Search_cloud.B_CanbeSold =1";
                else if (nItemUsedFor == 3)
                    Condition = Condition + " and vw_InvItem_Search_cloud.B_CanBePurchased =1 and vw_InvItem_Search_cloud.B_CanbeSold =1";
                else if (nItemUsedFor == 4)
                    Condition = Condition + " and vw_InvItem_Search_cloud.B_CanbeRawMaterial =1";
            }

            if (nPriceListID > 0)
            {

                priceListCondition = " and vw_InvItem_Search_cloud.N_ItemID in (Select N_ItemID from Inv_DiscountDetails where N_CompanyID=@p1 and n_DiscID=" + nPriceListID + " )";
            }
            if(isAssetItem==true){
                if(nitemType==1){
                otherItem = otherItem + " and N_ItemTypeID=1";
            }
            else if(nitemType==6){
                otherItem = otherItem + " and N_ItemTypeID=6";
            }
            }
            else{
                otherItem = otherItem + " and N_ItemTypeID<>1";
            }
         
         
                 sqlComandText = "  vw_InvItem_Search_cloud.*,"+showStock+" dbo.SP_SellingPrice(vw_InvItem_Search_cloud.N_ItemID,vw_InvItem_Search_cloud.N_CompanyID) as N_SellingPrice,Inv_ItemUnit.N_SellingPrice as N_SellingPrice2 FROM vw_InvItem_Search_cloud LEFT OUTER JOIN " +
             " Inv_ItemUnit ON vw_InvItem_Search_cloud.N_StockUnitID = Inv_ItemUnit.N_ItemUnitID AND vw_InvItem_Search_cloud.N_CompanyID = Inv_ItemUnit.N_CompanyID where vw_InvItem_Search_cloud.N_CompanyID=@p1 and vw_InvItem_Search_cloud.B_Inactive=@p2 and vw_InvItem_Search_cloud.[Item Code]<> @p3  and vw_InvItem_Search_cloud.N_ItemID=Inv_ItemUnit.N_ItemID and  vw_InvItem_Search_cloud.N_ClassID!=6 " + ownAssent + RentalItem + RentalPOItem + qry + Category + Condition + itemTypeCondition + warehouseSql + priceListCondition+otherItem;
            // string sqlComandText = " * from vw_InvItem_Search_cloud where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 " + qry;

     



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
                    bool b_Order = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "ProductListOrder", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    if (b_Order)
                    {
                        xOrder = "ORDER BY vw_InvItem_Search_cloud.[Item Code]";
                        xOrderNew = "ORDER BY [Item Code] ";
                    }
                    else
                    {
                        xOrder = "ORDER BY vw_InvItem_Search_cloud.Description asc";
                        xOrderNew = "ORDER BY Description asc";
                    }

                    string pageQry = "DECLARE @PageSize INT, @Page INT Select @PageSize=@PSize,@Page=@Offset;WITH PageNumbers AS(Select ROW_NUMBER() OVER(" + xOrder + ") RowNo,";
                    string pageQryEnd = ") SELECT * FROM    PageNumbers WHERE   RowNo BETWEEN((@Page -1) *@PageSize + 1)  AND(@Page * @PageSize) " + xOrderNew + " ";
                    string sql = pageQry + sqlComandText + pageQryEnd;
                    dt = dLayer.ExecuteDataTable(sql, Params, connection);


                    dt = myFunctions.AddNewColumnToDataTable(dt, "SubItems", typeof(DataTable), null);

                    foreach (DataRow item in dt.Rows)
                    {
                        if (myFunctions.getIntVAL(item["N_ClassID"].ToString()) == 1)//|| myFunctions.getIntVAL(item["N_ClassID"].ToString()) == 3
                        {

                            string subItemSql = "SELECT     vw_InvItem_Search_cloud.*, dbo.SP_SellingPrice(vw_InvItem_Search_cloud.N_ItemID, vw_InvItem_Search_cloud.N_CompanyID) AS N_SellingPrice, Inv_ItemUnit.N_SellingPrice AS N_SellingPrice2, Inv_ItemUnit.X_ItemUnit AS Expr1, Inv_ItemDetails.N_MainItemID,Inv_ItemDetails.B_HideInInv as B_GroupHideInInv, Inv_ItemDetails.N_Qty, Inv_ItemDetails.N_Qty as N_SubItemQty FROM  Inv_ItemUnit RIGHT OUTER JOIN Inv_ItemDetails RIGHT OUTER JOIN vw_InvItem_Search_cloud ON Inv_ItemDetails.N_CompanyID = vw_InvItem_Search_cloud.N_CompanyID AND Inv_ItemDetails.N_ItemID = vw_InvItem_Search_cloud.N_ItemID ON Inv_ItemUnit.N_CompanyID = vw_InvItem_Search_cloud.N_CompanyID AND Inv_ItemUnit.N_ItemID = vw_InvItem_Search_cloud.N_ItemID AND Inv_ItemUnit.N_ItemUnitID = vw_InvItem_Search_cloud.N_SalesUnitID WHERE(vw_InvItem_Search_cloud.N_CompanyID = " + nCompanyID + ") AND(vw_InvItem_Search_cloud.B_InActive = 0) and Inv_ItemDetails.N_MainItemID =" + myFunctions.getIntVAL(item["N_ItemID"].ToString()) + "";
                            DataTable subTbl = dLayer.ExecuteDataTable(subItemSql, connection);
                            item["SubItems"] = subTbl;
                        }
                    }
                    dt.AcceptChanges();
                }
                // dt = _api.Format(dt);

                // SortedList Result = new SortedList();
                // Result.Add("details", dt);
                // Result.Add("qry", query);
                // return Ok(_api.Success(Result));

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
                return Ok(_api.Error(User, e));
            }

        }
        [HttpGet("dashboardList")]
        public ActionResult GetDashboardList(int nFnYearId, bool b_AllBranchData, int nBranchID, int nLocationID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, bool bActiveItem)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            string xCriteria = "";
            string sqlCommandCount = "";

            // if (b_AllBranchData)
            //     xCriteria = "";
            // else
            //     xCriteria = " and  N_BranchID=@p5 ";

            string view = " vw_InvItem_Search_cloud ";

            if (b_AllBranchData)
            {
                nLocationID = 0;
                xCriteria = "";
            }


            if (nLocationID != 0) xCriteria = xCriteria + " and N_ItemID in (Select N_ItemID from Inv_ItemMasterWHLink where N_CompanyID=@p1 and N_WarehouseID=@p6 ) ";


            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and (Description like '%" + xSearchkey + "%' or [Item Code] like '%" + xSearchkey + "%' or [Item Class] like '%" + xSearchkey + "%' or X_Barcode like '%" + xSearchkey + "%' or X_CustomerName like '%" + xSearchkey + "%' or  X_CustomerSKU like '%" + xSearchkey + "%' or [Part No] like '%" + xSearchkey + "%')";
            // Searchkey = " and (Description like '%" + xSearchkey + "%' or [Item Code] like '%" + xSearchkey + "%' or Category like '%" + xSearchkey + "%' or [Item Class] like '%" + xSearchkey + "%' or N_Rate like '%" + xSearchkey + "%' or X_StockUnit like '%" + xSearchkey + "%' or X_Barcode like '%" + xSearchkey + "%' or [Part No] like '%" + xSearchkey + "%')";

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
                    case "n_Rate":
                        xSortBy = "Cast(REPLACE(n_Rate,',','') as Numeric(10,2)) " + xSortBy.Split(" ")[1];
                        break;
                    case "partNo":
                        xSortBy = "[Part No] desc ";
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;

            }

            string feildList = " N_CompanyID, N_ItemID, [Item Code], Description, Description_Ar, Category, [Item Class], N_Rate, [Part No], X_ItemUnit, N_Qty, X_SalesUnit, X_PurchaseUnit, X_StockUnit, Rate, N_StockUnitID, [Product Code], Stock,X_CustomerName,X_CustomerSKU ";

            // if (Count == 0)
            //     sqlCommandText = "select top(" + nSizeperpage + ") " + feildList + " from vw_InvItem_Search_cloud where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 " + xCriteria + Searchkey + " group by " + feildList + xSortBy;
            // else
            //     sqlCommandText = "select top(" + nSizeperpage + ") " + feildList + " from vw_InvItem_Search_cloud where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 " + Searchkey + " and [Item Code] not in (select top(" + Count + ") [Item Code] from vw_InvItem_Search_cloud where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 " + " group by " + feildList + Searchkey + xSortBy + " ) " + " group by " + feildList + xSortBy;
            int OFFSET = (nPage * nSizeperpage) - nSizeperpage;
            string PageString = " OFFSET " + OFFSET + " ROWS FETCH NEXT " + nSizeperpage + " ROWS ONLY";
            if (bActiveItem == false)
            {
                sqlCommandText = "select " + feildList + " from " + view + " where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 " + xCriteria + Searchkey + xSortBy + PageString;
            }
            else
            {
                sqlCommandText = "select * from vw_InvItem_Search_cloud where N_CompanyID=@p1 and B_Inactive=1 and [Item Code]<> @p3 and N_ItemTypeID<>@p4" + Searchkey + xSortBy;
            }

            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", 0);
            Params.Add("@p3", "001");
            Params.Add("@p4", 1);
            Params.Add("@p5", nBranchID);
            Params.Add("@p6", nLocationID);

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    if (bActiveItem == false)
                    {
                        sqlCommandCount = "select Count(1)  from Inv_ItemMaster where N_CompanyID=@p1 and B_Inactive=@p2 and X_ItemCode<> @p3 and N_ItemTypeID<>@p4 " + xCriteria;
                    }
                    else
                    {
                        sqlCommandCount = "select Count(1)  from Inv_ItemMaster where N_CompanyID=@p1 and B_Inactive=1 and X_ItemCode<> @p3 and N_ItemTypeID<>@p4 " + xCriteria;
                    }
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
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

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }

        }

        [HttpPost("generatebarcode")]
        public ActionResult GenerateBarcode([FromBody] DataSet ds)
        {
            DataTable products = new DataTable();
            // Datatable products = new Datatable();
            products = ds.Tables["details"];

            string path = this.TempFilesPath + "//barcode.pdf";
            Document doc = new Document(PageSize.A4);
            var output = new FileStream(path, FileMode.Create);
            var writer = PdfWriter.GetInstance(doc, output);
            doc.Open();
            for (int k = 0; k < products.Rows.Count; k++)
            {
                string xItemName = products.Rows[k]["x_ItemName"].ToString();
                string xBarcode = products.Rows[k]["x_ItemCode"].ToString();
                string nPrice = products.Rows[k]["n_Cost"].ToString();

                if (CreateBarcode(xBarcode))
                {
                    string bimageloc = "C://Olivoserver2020/Barcode/";

                    //Font
                    BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 15, iTextSharp.text.Font.NORMAL);
                    Paragraph p1 = new Paragraph(new Chunk(xItemName, font));
                    p1.IndentationRight = 100;
                    p1.IndentationLeft = 100;
                    doc.Add(p1);

                    bimageloc = bimageloc + xBarcode + ".png";
                    var logo = iTextSharp.text.Image.GetInstance(bimageloc);
                    logo.ScaleAbsoluteHeight(100);
                    logo.ScaleAbsoluteWidth(280);
                    doc.Add(logo);

                    Paragraph p2 = new Paragraph(new Chunk(xBarcode, font));
                    p2.IndentationRight = 100;
                    p2.IndentationLeft = 100;
                    doc.Add(p2);
                    doc.NewPage();

                }
            }
            doc.Close();
            return Ok(_api.Success(new SortedList() { { "FileName", "barcode.pdf" } }));

        }


        public void createpdf(string bimage, string productname, string price, string xBarcode)
        {
            Document doc = new Document(PageSize.A4);
            string path = this.TempFilesPath + "//barcode" + xBarcode + ".pdf";
            var output = new FileStream(path, FileMode.Create);
            var writer = PdfWriter.GetInstance(doc, output);
            doc.Open();

            Chunk c1 = new Chunk(productname);
            doc.Add(c1);

            var logo = iTextSharp.text.Image.GetInstance(bimage);
            logo.SetAbsolutePosition(0, 550);
            logo.ScaleAbsoluteHeight(200);
            logo.ScaleAbsoluteWidth(280);
            doc.Add(logo);
            doc.NewPage();
            doc.Close();
        }


        public bool CreateBarcode(string Data)
        {
            if (Data != "")
            {
                Zen.Barcode.Code128BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
                System.Drawing.Image img = barcode.Draw(Data, 50);
                img.Save("C://OLIVOSERVER2020/Barcode/" + Data + ".png", ImageFormat.Png);
            }
            return true;
        }





        [HttpGet("details")]
        public ActionResult GetItemDetails(int nItemID, int nLocationID, int nBranchID)
        {
            DataTable dt = new DataTable();
            DataTable multiCategory = new DataTable();
            DataTable dt_LocStock = new DataTable();
            DataTable Images = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();
            DataTable dtVariantList = new DataTable();
            DataTable dtItemUnits = new DataTable();
            DataTable SubItemsTable = new DataTable();
            DataTable ScrapItemsTable = new DataTable();
            DataTable BOMEmpTable = new DataTable();
            DataTable BOMAssetTable = new DataTable();
            DataTable ItemWarrantyTable = new DataTable();
            DataTable Attachments = new DataTable();
            int N_ItemID = nItemID;
            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@nItemID", N_ItemID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string _sqlQuery = "SELECT * from vw_InvItemMaster where N_ItemID=@nItemID and N_CompanyID=@nCompanyID";

                    dt = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                    string multiqry = "SELECT * from vw_ItemCategoryDisplay where N_ItemID=@nItemID and N_CompanyID=@nCompanyID";
                    multiCategory = dLayer.ExecuteDataTable(multiqry, QueryParams, connection);
                    multiCategory = _api.Format(multiCategory, "multiCategory");


                    Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(dt.Rows[0]["N_ItemID"].ToString()), myFunctions.getIntVAL(dt.Rows[0]["N_ItemID"].ToString()), 53, 0, User, connection);
                    Attachments = _api.Format(Attachments, "attachments");

                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    int N_BranchID = nBranchID;
                    // int N_ItemID = myFunctions.getIntVAL(dt.Rows[0]["N_ItemID"].ToString());
                    // QueryParams.Add("@nItemID", dt.Rows[0]["N_ItemID"].ToString());
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
                    object CalcCost = 0;
                    // if (N_BranchID == 0) // 05/12/2020 Cost caucation made usinf procedure by Zainab under the instruction of Anees sir
                    //     CalcCost = myFunctions.getVAL(dLayer.ExecuteScalar("Select dbo.SP_Cost(" + N_ItemID + "," + myFunctions.GetCompanyID(User) + ",'" + dt.Rows[0]["X_StockUnit"].ToString() + "') As N_LPrice", connection).ToString());
                    // else
                    //     CalcCost = myFunctions.getVAL(dLayer.ExecuteScalar("Select dbo.SP_Cost_Loc(" + N_ItemID + "," + myFunctions.GetCompanyID(User) + ",'" + dt.Rows[0]["X_StockUnit"].ToString() + "'," + nLocationID + ") As N_LPrice", connection).ToString());

                    if (N_BranchID == 0) // 07/04/2022 Cost calculation made using procedure by Akshay under the instruction of Ratheesh sir
                        CalcCost = myFunctions.getVAL(dLayer.ExecuteScalar("Select dbo.SP_CostAvg(" + N_ItemID + "," + myFunctions.GetCompanyID(User) + ",'" + dt.Rows[0]["X_StockUnit"].ToString() + "') As N_LPrice", connection).ToString());
                    else
                        CalcCost = myFunctions.getVAL(dLayer.ExecuteScalar("Select dbo.SP_CostAvg_Loc(" + N_ItemID + "," + myFunctions.GetCompanyID(User) + ",'" + dt.Rows[0]["X_StockUnit"].ToString() + "'," + nLocationID + ") As N_LPrice", connection).ToString());

                    dt.Rows[0]["n_ItemCost"] = CalcCost;
                    dt.AcceptChanges();
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


                    string variant = "SELECT * from vw_InvItemMaster where N_ItemID<>@nItemID and N_GroupID = @nItemID and N_CompanyID=@nCompanyID";
                    dtVariantList = dLayer.ExecuteDataTable(variant, QueryParams, connection);
                    dtVariantList = _api.Format(dtVariantList, "variantList");

                    string itemUnits = "SELECT Inv_ItemUnit.*, Gen_Defaults.X_TypeName FROM   Inv_ItemUnit LEFT OUTER JOIN Gen_Defaults ON Inv_ItemUnit.N_DefaultType = Gen_Defaults.N_TypeCode and  N_DefaultId =104  where  N_ItemID = @nItemID and N_CompanyID=@nCompanyID order by N_Defaulttype";
                    dtItemUnits = dLayer.ExecuteDataTable(itemUnits, QueryParams, connection);
                    dtItemUnits = _api.Format(dtItemUnits, "itemUnits");


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

                    //SubItems
                    string subItems = "Select * from vw_InvItemDetails Where N_CompanyID=@nCompanyID and N_MainItemID=@nItemID and N_Type=1";
                    SubItemsTable = dLayer.ExecuteDataTable(subItems, QueryParams, connection);
                    SubItemsTable = _api.Format(SubItemsTable, "subItems");

                    //ScrapItems
                    string scrapItems = "Select * from vw_InvItemDetails Where N_CompanyID=@nCompanyID and N_MainItemID=@nItemID and N_Type=2";
                    ScrapItemsTable = dLayer.ExecuteDataTable(scrapItems, QueryParams, connection);
                    ScrapItemsTable = _api.Format(ScrapItemsTable, "scrapItems");

                    //BOMEmp
                    string bomEmp = "Select * from vw_BOMEmployee_Disp Where N_CompanyID=@nCompanyID and N_MainItem=@nItemID";
                    BOMEmpTable = dLayer.ExecuteDataTable(bomEmp, QueryParams, connection);
                    BOMEmpTable = _api.Format(BOMEmpTable, "bomEmp");

                    //BOMAsset
                    string bomAsset = "Select * from vw_BOMAsset_Disp Where N_CompanyID=@nCompanyID and N_MainItemID=@nItemID";
                    BOMAssetTable = dLayer.ExecuteDataTable(bomAsset, QueryParams, connection);
                    BOMAssetTable = _api.Format(BOMAssetTable, "bomAsset");

                    string itemWarranty = "Select * from vw_Inv_ItemWarranty Where N_CompanyID=@nCompanyID and N_MainItemID=@nItemID";
                    ItemWarrantyTable = dLayer.ExecuteDataTable(itemWarranty, QueryParams, connection);
                    ItemWarrantyTable = _api.Format(ItemWarrantyTable, "itemWarranty");





                }
                dt.AcceptChanges();
                // multiCategory.AcceptChanges();
                dt = _api.Format(dt);
                //multiCategory = _api.Format(multiCategory);

                DataSet dataSet = new DataSet();
                dt = _api.Format(dt, "details");
                //multiCategory = _api.Format(multiCategory, "multiCategory");
                dataSet.Tables.Add(multiCategory);
                Images = _api.Format(Images, "Images");
                dataSet.Tables.Add(dt);

                dataSet.Tables.Add(Images);
                dataSet.Tables.Add(dtItemUnits);
                dataSet.Tables.Add(dtVariantList);

                dataSet.Tables.Add(SubItemsTable);
                dataSet.Tables.Add(ScrapItemsTable);
                dataSet.Tables.Add(BOMEmpTable);
                dataSet.Tables.Add(BOMAssetTable);
                dataSet.Tables.Add(ItemWarrantyTable);
                dataSet.Tables.Add(Attachments);

                return Ok(_api.Success(dataSet));

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable = new DataTable();
                DataTable MasterTableNew, GeneralTable, StockUnit, SalesUnit, PurchaseUnit, AddUnit1, AddUnit2, LocationList, CategoryList, VariantList, ItemUnits, itemWarranty, storeAllocation;
                DataTable SubItemTable = new DataTable();
                DataTable ScrapItemTable = new DataTable();
                DataTable BOMEmpTable = new DataTable();
                DataTable BOMAssetTable = new DataTable();
                DataTable POS = ds.Tables["Pos"];
                DataTable ECOM = ds.Tables["Ecom"];
                DataTable Attachment = ds.Tables["attachments"];
                MasterTableNew = ds.Tables["master"];
                GeneralTable = ds.Tables["general"];
                StockUnit = ds.Tables["stockUnit"];
                SalesUnit = ds.Tables["salesUnit"];
                PurchaseUnit = ds.Tables["purchaseUnit"];
                AddUnit1 = ds.Tables["addUnit1"];
                AddUnit2 = ds.Tables["addUnit2"];
                LocationList = ds.Tables["warehouseDetails"];
                CategoryList = ds.Tables["categoryListDetails"];
                VariantList = ds.Tables["variantList"];
                ItemUnits = ds.Tables["itemUnits"];
                SubItemTable = ds.Tables["subItems"];
                ScrapItemTable = ds.Tables["scrapItems"];
                BOMEmpTable = ds.Tables["bomEmp"];
                BOMAssetTable = ds.Tables["bomAsset"];
                itemWarranty = ds.Tables["itemWarranty"];
                storeAllocation = ds.Tables["store"];
                int nCompanyID = myFunctions.getIntVAL(MasterTableNew.Rows[0]["N_CompanyId"].ToString());
                int N_ItemID = myFunctions.getIntVAL(MasterTableNew.Rows[0]["N_ItemID"].ToString());
                string XItemName = MasterTableNew.Rows[0]["X_ItemName"].ToString();
                object n_MinQty = "";
                object n_ReOrderQty = "";
                if (MasterTableNew.Columns.Contains("n_MinQty"))
                {
                    n_MinQty = MasterTableNew.Rows[0]["n_MinQty"] == System.DBNull.Value ? "" : MasterTableNew.Rows[0]["n_MinQty"];

                }
                if (MasterTableNew.Columns.Contains("n_ReOrderQty"))
                {
                    n_ReOrderQty = MasterTableNew.Rows[0]["n_ReOrderQty"] == System.DBNull.Value ? "" : MasterTableNew.Rows[0]["n_ReOrderQty"];
                }




                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    int N_VariantGrpType = 0;
                    string ItemCode = "";
                    int ItemType = 0;
                    ItemCode = MasterTableNew.Rows[0]["X_ItemCode"].ToString();
                    ItemType = myFunctions.getIntVAL(MasterTableNew.Rows[0]["N_CLassID"].ToString());



                    if (ItemCode != "@Auto")
                    {
                        object N_DocNumber = dLayer.ExecuteScalar("Select 1 from Inv_ItemMaster Where X_ItemCode ='" + ItemCode + "' and N_CompanyID= " + nCompanyID + " and N_ItemID<>" + N_ItemID, connection, transaction);
                        if (N_DocNumber == null)
                        {
                            N_DocNumber = 0;
                        }
                        if (myFunctions.getVAL(N_DocNumber.ToString()) >= 1)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Product code already in use"));
                        }
                    }

                    if (ItemCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterTableNew.Rows[0]["N_CompanyId"].ToString());
                        Params.Add("N_YearID", GeneralTable.Rows[0]["N_FnYearId"].ToString());
                        Params.Add("N_FormID", 53);

                        ItemCode = dLayer.GetAutoNumber("Inv_ItemMaster", "X_ItemCode", Params, connection, transaction);
                        if (ItemCode == "") { transaction.Rollback(); return Ok(_api.Warning("Unable to generate product Code")); }
                        MasterTableNew.Rows[0]["X_ItemCode"] = ItemCode;
                    }

                    if (myFunctions.getIntVAL(MasterTableNew.Rows[0]["N_ItemID"].ToString()) > 0)
                    {
                        int N_PkeyID = N_ItemID;
                        dLayer.DeleteData("Inv_ItemDetails", "N_MainItemID", myFunctions.getIntVAL(MasterTableNew.Rows[0]["N_ItemID"].ToString()), "N_CompanyID=" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Inv_BOMEmployee", "N_MainItem", myFunctions.getIntVAL(MasterTableNew.Rows[0]["N_ItemID"].ToString()), "N_CompanyID=" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Inv_BOMAsset", "N_MainItemID", myFunctions.getIntVAL(MasterTableNew.Rows[0]["N_ItemID"].ToString()), "N_CompanyID=" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Inv_ItemWarranty", "N_MainItemID", myFunctions.getIntVAL(MasterTableNew.Rows[0]["N_ItemID"].ToString()), "N_CompanyID=" + nCompanyID, connection, transaction);

                    }
                    //Adding variant product in master table
                    MasterTable = MasterTableNew.Clone();
                    var ActRow = MasterTable.NewRow();
                    ActRow.ItemArray = MasterTableNew.Rows[0].ItemArray;
                    MasterTable.Rows.Add(ActRow);
                    if (VariantList.Rows.Count > 0)
                    {

                        int j = 1;
                        string VariantCode = "";
                        for (int i = 0; i < VariantList.Rows.Count; i++)
                        {

                            var newRow = MasterTable.NewRow();
                            if (myFunctions.getIntVAL(VariantList.Rows[i]["N_ItemID"].ToString()) == 0)
                            {
                                newRow.ItemArray = MasterTableNew.Rows[0].ItemArray;
                                MasterTable.Rows.Add(newRow);
                                MasterTable.Rows[j]["X_ItemName"] = VariantList.Rows[i]["X_ItemName"].ToString();
                                if (VariantList.Rows[i]["X_Barcode"].ToString() != "")
                                    MasterTable.Rows[j]["X_Barcode"] = VariantList.Rows[i]["X_Barcode"].ToString();

                                if (MasterTable.Columns.Contains("N_Rate") && VariantList.Columns.Contains("N_Rate"))
                                    if (VariantList.Rows[i]["N_Rate"].ToString() != "")
                                        MasterTable.Rows[j]["N_Rate"] = myFunctions.getVAL(VariantList.Rows[i]["N_Rate"].ToString());
                                MasterTable.Rows[j]["N_CLassID"] = "2";



                                VariantCode = dLayer.GetAutoNumber("Inv_ItemMaster", "X_ItemCode", Params, connection, transaction);
                                if (VariantCode == "") { transaction.Rollback(); return Ok(_api.Warning("Unable to generate product Code")); }
                                MasterTable.Rows[j]["X_ItemCode"] = VariantCode;
                                VariantCode = "";
                            }
                            else
                            {
                                newRow.ItemArray = VariantList.Rows[i].ItemArray;
                                MasterTable.Rows.Add(newRow);
                            }
                            j++;
                        }
                    }

                    for (int k = 0; k < MasterTable.Rows.Count; k++)
                    {
                        SortedList QueryParams = new SortedList();
                        QueryParams.Add("@nCompanyID", nCompanyID);
                        QueryParams.Add("@nItemID", myFunctions.getIntVAL(MasterTable.Rows[k]["N_ItemID"].ToString()));
                        QueryParams.Add("@xItemName", MasterTable.Rows[k]["X_ItemName"].ToString());
                        int count = 0;
                        object res = dLayer.ExecuteScalar("Select count(*) as count from Inv_ItemMaster where X_ItemName =@xItemName and N_ItemID <> @nItemID and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                        if (res != null)
                            count = myFunctions.getIntVAL(res.ToString());

                        if (count > 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to save, Product name already exist"));
                        }

                        if (MasterTable.Columns.Contains("X_CustomerSKU"))
                        {
                            int NCustomerID = myFunctions.getIntVAL(MasterTableNew.Rows[0]["N_CustomerID"].ToString());
                            string XCustomerSKU = MasterTableNew.Rows[0]["x_CustomerSKU"].ToString();
                            if (XCustomerSKU != "")
                            {
                                object SKUCount = dLayer.ExecuteScalar("Select Count(*) from Inv_ItemMaster Where N_CustomerID =" + NCustomerID + " and N_ItemID <> " + myFunctions.getIntVAL(MasterTable.Rows[k]["N_ItemID"].ToString()) + " and X_CustomerSKU='" + XCustomerSKU + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                                if (myFunctions.getVAL(SKUCount.ToString()) >= 1)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Customer sku already exists"));
                                }

                            }
                        }




                        string xBarcode = "";
                        if (MasterTable.Rows[k]["X_Barcode"].ToString() == "")
                        {
                            xBarcode = AutoGenerateBarCode(MasterTable.Rows[k]["X_ItemCode"].ToString(), myFunctions.getIntVAL(MasterTable.Rows[k]["N_CategoryID"].ToString()), nCompanyID, dLayer, connection, transaction);
                            MasterTable.Rows[k]["X_Barcode"] = xBarcode;
                        }
                        string image = "";

                        if (MasterTable.Columns.Contains("i_Image"))
                        {

                            image = MasterTable.Rows[0]["i_Image"].ToString();

                            MasterTable.Rows[k]["i_Image"] = "";

                        }
                        Byte[] imageBitmap = new Byte[image.Length];
                        imageBitmap = Convert.FromBase64String(image);
                        //MasterTable.Columns.Remove("i_Image");

                        string DupCriteria = "";// "N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_ItemID="+N_ItemID;  // and X_ItemCode='" + ItemCode + "'";
                        N_ItemID = dLayer.SaveDataWithIndex("Inv_ItemMaster", "N_ItemID", DupCriteria, "", k, MasterTable, connection, transaction);
                        if (N_ItemID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to save"));
                        }

                        if (n_MinQty == null || n_MinQty == "")
                        {

                            dLayer.ExecuteNonQuery("update  Inv_ItemMaster set n_MinQty = null where N_ItemID=" + N_ItemID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + "", Params, connection, transaction);
                        }
                        if (n_ReOrderQty == null || n_ReOrderQty == "")
                        {

                            dLayer.ExecuteNonQuery("update  Inv_ItemMaster set n_ReOrderQty = null where N_ItemID=" + N_ItemID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + "", Params, connection, transaction);
                        }
                        if (k == 0 && ItemType == 6)
                        {
                            N_VariantGrpType = N_ItemID;
                            dLayer.ExecuteNonQuery("update  Inv_ItemMaster set B_Show = 0 ,b_CanbeSold=0,b_CanbePurchased=0 where N_ItemID=" + N_ItemID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + "", Params, connection, transaction);
                        }
                        else
                        {
                            dLayer.ExecuteNonQuery("update  Inv_ItemMaster set B_Show = 1  where N_ItemID=" + N_ItemID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + "", Params, connection, transaction);
                        }
                        if (ItemType == 6 || k > 0)
                            dLayer.ExecuteNonQuery("update  Inv_ItemMaster set N_GroupID=" + N_VariantGrpType + "  where N_ItemID=" + N_ItemID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + "", Params, connection, transaction);


                        if (image.Length > 0)
                            dLayer.SaveImage("Inv_ItemMaster", "i_Image", imageBitmap, "N_ItemID", N_ItemID, connection, transaction);

                        foreach (DataRow var in StockUnit.Rows) var["n_ItemID"] = N_ItemID;
                        // foreach (DataRow var in SalesUnit.Rows) var["n_ItemID"] = N_ItemID;
                        // foreach (DataRow var in PurchaseUnit.Rows) var["n_ItemID"] = N_ItemID;
                        // foreach (DataRow var in AddUnit1.Rows) var["n_ItemID"] = N_ItemID;
                        // foreach (DataRow var in AddUnit2.Rows) var["n_ItemID"] = N_ItemID;

                        foreach (DataRow var in ItemUnits.Rows) var["n_ItemID"] = N_ItemID;

                        if (k > 0 && MasterTable.Columns.Contains("N_Rate"))
                            foreach (DataRow var in StockUnit.Rows) var["N_SellingPrice"] = MasterTable.Rows[k]["N_Rate"].ToString();


                        int BaseUnitID = 0;
                        if (k == 0)
                            BaseUnitID = dLayer.SaveData("Inv_ItemUnit", "N_ItemUnitID", StockUnit, connection, transaction);
                        else
                        {
                            int _unitID = 0;
                            object unitID = dLayer.ExecuteScalar("select N_ItemUnitID from inv_itemunit  where N_ItemID = " + N_ItemID + " and X_ItemUnit = '" + StockUnit.Rows[0]["x_ItemUnit"].ToString() + "' and B_BaseUnit = 1  and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                            if (unitID != null)
                                _unitID = myFunctions.getIntVAL(unitID.ToString());
                            foreach (DataRow var in StockUnit.Rows) var["n_ItemUnitID"] = _unitID;
                            BaseUnitID = dLayer.SaveData("Inv_ItemUnit", "N_ItemUnitID", StockUnit, connection, transaction);

                        }


                        dLayer.ExecuteNonQuery("update  Inv_ItemMaster set N_ItemUnitID=" + BaseUnitID + " ,N_StockUnitID =" + BaseUnitID + " where N_ItemID=" + N_ItemID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + "", Params, connection, transaction);


                        foreach (DataRow var in ItemUnits.Rows) var["n_BaseUnitID"] = BaseUnitID;

                        dLayer.ExecuteNonQuery("update  Inv_ItemUnit set N_BaseUnitID=" + BaseUnitID + " where N_ItemID=" + N_ItemID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_ItemUnitID=" + BaseUnitID, Params, connection, transaction);

                        //int N_SalesUnitID = 0, N_PurchaseUnitID = 0, N_AddUnitID1 = 0, N_AddUnitID2 = 0;

                        // foreach (DataRow var in SalesUnit.Rows) var["n_BaseUnitID"] = BaseUnitID;
                        // foreach (DataRow var in PurchaseUnit.Rows) var["n_BaseUnitID"] = BaseUnitID;
                        // foreach (DataRow var in AddUnit1.Rows) var["n_BaseUnitID"] = BaseUnitID;
                        // foreach (DataRow var in AddUnit2.Rows) var["n_BaseUnitID"] = BaseUnitID;

                        string xBaseUnit = StockUnit.Rows[0]["X_ItemUnit"].ToString();
                        if (k == 0)
                            dLayer.SaveData("Inv_ItemUnit", "N_ItemUnitID", ItemUnits, connection, transaction);
                        else
                        {
                            for (int l = 0; l < ItemUnits.Rows.Count; l++)
                            {
                                int _unitID = 0;
                                object unitID = dLayer.ExecuteScalar("select N_ItemUnitID from inv_itemunit  where N_ItemID = " + N_ItemID + " and X_ItemUnit = '" + ItemUnits.Rows[l]["x_ItemUnit"].ToString() + "' and isnull(n_DefaultType,0) =" + ItemUnits.Rows[l]["n_DefaultType"].ToString() + " and  N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                                if (unitID != null)
                                    _unitID = myFunctions.getIntVAL(unitID.ToString());
                                ItemUnits.Rows[l]["n_ItemUnitID"] = _unitID;
                                dLayer.SaveDataWithIndex("Inv_ItemUnit", "N_ItemUnitID", "", "", l, ItemUnits, connection, transaction);

                            }
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
                        dLayer.DeleteData("Inv_ItemCategoryDisplayMaster", "N_ItemID", N_ItemID, "", connection, transaction);
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
                        dLayer.DeleteData("Inv_DisplayImages", "N_ItemID", N_ItemID, "", connection, transaction);

                        if (POS.Rows.Count > 0)
                        {
                            POS.Columns.Add("X_ImageName", typeof(System.String));
                            POS.Columns.Add("X_ImageLocation", typeof(System.String));
                            POS.Columns.Add("N_ImageID", typeof(System.Int32));

                            int i = 1;
                            foreach (DataRow dRow in POS.Rows)
                            {
                                myFunctions.writeImageFile(dRow["I_Image"].ToString(), myFunctions.GetUploadsPath(User, "PosProductImages"), ItemCode + "-POS-" + i);
                                dRow["X_ImageName"] = ItemCode + "-POS-" + i + ".jpg";
                                dRow["X_ImageLocation"] = myFunctions.GetUploadsPath(User, "PosProductImages");
                                dRow["N_ItemID"] = N_ItemID;
                                i++;

                            }
                            POS.Columns.Remove("I_Image");
                            dLayer.SaveData("Inv_DisplayImages", "N_ImageID", POS, connection, transaction);

                        }
                        if (ECOM.Rows.Count > 0)
                        {
                            ECOM.Columns.Add("X_ImageName", typeof(System.String));
                            ECOM.Columns.Add("X_ImageLocation", typeof(System.String));
                            ECOM.Columns.Add("N_ImageID", typeof(System.Int32));
                            int j = 1;
                            foreach (DataRow dRow in ECOM.Rows)
                            {
                                myFunctions.writeImageFile(dRow["I_Image"].ToString(), myFunctions.GetUploadsPath(User, "EcomProductImages"), ItemCode + "-ECOM-" + j);
                                dRow["X_ImageName"] = ItemCode + "-ECOM-" + j + ".jpg";
                                dRow["X_ImageLocation"] = myFunctions.GetUploadsPath(User, "EcomProductImages");
                                dRow["N_ItemID"] = N_ItemID;
                                j++;

                            }
                            ECOM.Columns.Remove("I_Image");
                            dLayer.SaveData("Inv_DisplayImages", "N_ImageID", ECOM, connection, transaction);

                        }

                        //SubItems
                        if (SubItemTable.Rows.Count > 0)
                        {
                            foreach (DataRow dRow in SubItemTable.Rows)
                            {
                                dRow["N_MainItemID"] = N_ItemID;
                            }
                            SubItemTable.AcceptChanges();
                            dLayer.SaveData("Inv_ItemDetails", "N_ItemDetailsID", SubItemTable, connection, transaction);
                        }
                        //ScarpItems
                        if (ScrapItemTable.Rows.Count > 0)
                        {
                            foreach (DataRow dRow in ScrapItemTable.Rows)
                            {
                                dRow["N_MainItemID"] = N_ItemID;
                            }
                            ScrapItemTable.AcceptChanges();
                            dLayer.SaveData("Inv_ItemDetails", "N_ItemDetailsID", ScrapItemTable, connection, transaction);
                        }
                        //BOMEmp
                        if (BOMEmpTable.Rows.Count > 0)
                        {
                            foreach (DataRow dRow in BOMEmpTable.Rows)
                            {
                                dRow["N_MainItem"] = N_ItemID;
                            }
                            BOMEmpTable.AcceptChanges();
                            dLayer.SaveData("Inv_BOMEmployee", "N_BOMEmpDetailID", BOMEmpTable, connection, transaction);
                        }
                        //BOMAsset
                        if (BOMAssetTable.Rows.Count > 0)
                        {
                            foreach (DataRow dRow in BOMAssetTable.Rows)
                            {
                                dRow["N_MainItemID"] = N_ItemID;
                            }
                            BOMAssetTable.AcceptChanges();
                            dLayer.SaveData("Inv_BOMAsset", "N_BOMAssetDetailID", BOMAssetTable, connection, transaction);
                        }
                        if (itemWarranty.Rows.Count > 0)
                        {
                            foreach (DataRow dRow in itemWarranty.Rows)
                            {
                                dRow["N_MainItemID"] = N_ItemID;
                            }
                            itemWarranty.AcceptChanges();
                            dLayer.SaveData("Inv_ItemWarranty", "N_ItemDetailsID", itemWarranty, connection, transaction);
                        }
                        if (storeAllocation.Rows.Count > 0)
                        {



                            for (int l = 0; l < storeAllocation.Rows.Count; l++)
                            {
                                int n_StoreID = myFunctions.getIntVAL(storeAllocation.Rows[l]["N_StoreID"].ToString());
                                int n_StoreDetailID = myFunctions.getIntVAL(storeAllocation.Rows[l]["N_StoreDetailID"].ToString());
                                object storeCount = dLayer.ExecuteScalar("select count(*) from Inv_OnlineStoreDetail  where N_StoreID = " + n_StoreID + " and N_StoreDetailID=" + n_StoreDetailID + " and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);

                                if (myFunctions.getIntVAL(storeCount.ToString()) > 0)
                                {

                                    dLayer.DeleteData("Inv_OnlineStoreDetail", "n_StoreDetailID", n_StoreDetailID, "", connection, transaction);
                                }

                                storeAllocation.Rows[l]["N_ItemID"] = N_ItemID;
                                dLayer.SaveDataWithIndex("Inv_OnlineStoreDetail", "n_StoreDetailID", "", "", l, storeAllocation, connection, transaction);

                            }

                        }

                    }


                    if (Attachment.Rows.Count > 0)
                    {
                        try
                        {
                            myAttachments.SaveAttachment(dLayer, Attachment, ItemCode, N_ItemID, XItemName, ItemCode, N_ItemID, "Product Document", User, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }
                    }

                    transaction.Commit();
                }
                return Ok(_api.Success("Product Saved"));

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        private string AutoGenerateBarCode(string xItemCode, int nCatID, int nCompanyID, IDataAccessLayer dLayer, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                int i = 0;
                string X_CatCode = "00";
                string X_ItemCategory = "", X_Barcode = "";
                if (nCatID != 0)
                {
                    X_ItemCategory = dLayer.ExecuteScalar("select X_CategoryCode from Inv_ItemCategory WHERE N_CategoryID=" + nCatID + " and N_CompanyID=" + nCompanyID, connection, transaction).ToString();
                    X_CatCode = X_ItemCategory;
                }
                do
                {
                    i += 1;
                    xItemCode = xItemCode.Length < 6 ? xItemCode.PadLeft(6, '0') : xItemCode.Substring(xItemCode.Length - 6);
                    X_CatCode = X_CatCode.Length < 2 ? X_CatCode.PadLeft(2, '0') : X_CatCode.Substring(X_CatCode.Length - 2);
                    string X_StartWith = "99";
                    string X_CheckSum = CalculateChecksum(X_StartWith + (i.ToString()).PadLeft(2, '0') + X_CatCode + xItemCode).ToString();
                    X_Barcode = X_StartWith + (i.ToString()).PadLeft(2, '0') + X_CatCode + xItemCode + X_CheckSum;
                } while (dLayer.ExecuteScalar("SELECT X_BARCODE FROM Inv_ItemMaster WHERE X_BARCODE = '" + X_Barcode + "'", connection, transaction) != null);


                return X_Barcode;
            }
            catch (Exception ex)
            {
                //msg.msgError(ex.Message);
                return "";
            }
        }

        public int CalculateChecksum(string code)
        {
            try
            {
                if (code == null || code.Length != 12)
                    throw new ArgumentException("Code length should be 12, i.e. excluding the checksum digit");

                int sum = 0;
                for (int i = 0; i < 12; i++)
                {
                    int v;
                    if (!int.TryParse(code[i].ToString(), out v))
                        throw new ArgumentException("Invalid character encountered in specified code.");
                    sum += (i % 2 == 0 ? v : v * 3);
                }
                int check = 10 - (sum % 10);
                return check % 10;
            }
            catch (Exception)
            {
                return 0;
            }
        }

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
                return Ok(_api.Error(User, e));
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
                return Ok(_api.Error(User, e));
            }
        }
        [HttpDelete("unitDelete")]
        public ActionResult DeleteUnit(int nUnitID, int nItemID, string xUnitName, int nDefaultType, int nFnYearID)
        {


            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable dtItems = new DataTable();
                    SortedList QueryParams = new SortedList();
                    QueryParams.Add("@nCompanyID", nCompanyID);
                    QueryParams.Add("@nItemID", nItemID);
                    object usedCheck = null;
                    usedCheck = dLayer.ExecuteScalar("Select N_ItemID From vw_InvStock_Status Where N_ItemID= @nItemID and (Type<>'O' and Type<>'PO' and Type<>'SO') and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                    if (usedCheck != null)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Can't be delete,It has been used!"));
                    }

                    int classID = 0;
                    object res = dLayer.ExecuteScalar("Select N_ClassID from Inv_ItemMaster where N_ItemID = @nItemID and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                    if (res != null)
                        classID = myFunctions.getIntVAL(res.ToString());

                    if (classID == 6)
                    {
                        dLayer.ExecuteScalar("delete from Inv_ItemUnit where N_ItemUnitID in (select N_ItemUnitID from Inv_ItemUnit where N_itemID in (select N_ItemID from Inv_ItemMaster where N_GroupID = " + nItemID + " and N_CompanyID = " + nCompanyID + ") and X_ItemUnit = '" + xUnitName + "' and isnull(N_DefaultType,0) = " + nDefaultType + " and N_CompanyID =" + nCompanyID + ") and  N_CompanyID=" + nCompanyID, connection, transaction);
                    }
                    else
                    {
                        dLayer.ExecuteScalar("delete from Inv_ItemUnit where N_ItemUnitID = " + nUnitID + " and N_CompanyID = " + nCompanyID + "", connection, transaction);
                    }

                    transaction.Commit();
                    return Ok(_api.Success("Unit deleted"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, "Can't be delete,It has been used!"));
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
                    DataTable dtItems = new DataTable();
                    SortedList QueryParams = new SortedList();
                    QueryParams.Add("@nCompanyID", nCompanyID);
                    QueryParams.Add("@nItemID", nItemID);
                    int classID = 0;
                    object res = dLayer.ExecuteScalar("Select N_ClassID from Inv_ItemMaster where N_ItemID = @nItemID and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                    if (res != null)
                        classID = myFunctions.getIntVAL(res.ToString());

                    if (classID == 6)
                    {
                        string Items = "SELECT N_ItemID from Inv_ItemMaster where N_GroupID = @nItemID and N_CompanyID=@nCompanyID";

                        dtItems = dLayer.ExecuteDataTable(Items, QueryParams, connection, transaction);
                    }
                    else
                    {
                        dtItems.Columns.Add("N_ItemID", typeof(System.Int64));
                        var newRow = dtItems.NewRow();
                        dtItems.Rows.Add(newRow);
                        dtItems.Rows[0]["N_ItemID"] = nItemID;
                    }



                    for (int i = 0; i < dtItems.Rows.Count; i++)
                    {
                        int _itemID = myFunctions.getIntVAL(dtItems.Rows[i]["N_ItemID"].ToString());

                        object N_Result = dLayer.ExecuteScalar("Select B_YearEndProcess from Acc_FnYear Where N_CompanyID= " + nCompanyID + " and N_FnYearID= " + nFnYearID, connection, transaction);
                        if (myFunctions.getIntVAL(myFunctions.getBoolVAL(N_Result.ToString())) == 1)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Year Closed , Unable to delete product."));
                        }

                        dLayer.DeleteData("Inv_ItemDetails", "N_MainItemID", _itemID, "", connection, transaction);
                        Results = dLayer.DeleteData("Inv_ItemMaster", "N_ItemID", _itemID, "", connection, transaction);
                        if (Results > 0)
                        {

                            dLayer.ExecuteScalar("delete from  Inv_ItemUnit  Where N_ItemID=" + _itemID + " and N_CompanyID=" + nCompanyID, connection, transaction);
                            dLayer.ExecuteScalar("delete from  Inv_BOMEmployee  Where N_MainItem=" + _itemID + " and N_CompanyID=" + nCompanyID, connection, transaction);
                            dLayer.ExecuteScalar("delete from  Inv_BOMAsset  Where N_MainItemID=" + _itemID + " and N_CompanyID=" + nCompanyID, connection, transaction);
                            dLayer.ExecuteScalar("delete from  Inv_ItemCategoryDisplayMaster  Where N_ItemID=" + _itemID + " and N_CompanyID=" + nCompanyID, connection, transaction);
                            // transaction.Commit();
                            // return Ok(_api.Success("Product deleted"));
                        }
                        else
                        {
                            transaction.Rollback();

                            return Ok(_api.Error(User, "Unable to delete product"));
                        }
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Product deleted"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, "Can't be delete,It has been used!"));
            }
        }

        [HttpGet("batchList")]
        public ActionResult GetBatchList(int nLocationID, int itemId, bool isWHM)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            try
            {
                SortedList mParamsList = new SortedList()
                    {
                        {"@N_CompanyID",nCompanyId},
                        {"@N_LocationID",nLocationID},
                        {"@N_ItemID",itemId}
                    };
                DataTable masterTable = new DataTable();

                string x_Criteria = "";
                string sql = "";

                if (isWHM)
                    x_Criteria = " (select N_LocationID from Inv_Location where Left(X_Pattern,(select len(X_Pattern) from Inv_Location where N_CompanyID=@N_CompanyID and N_LocationID=@N_LocationID)) =(select cast(X_Pattern as varchar) from Inv_Location where N_CompanyID=@N_CompanyID and N_LocationID=@N_LocationID)) ";
                else
                    x_Criteria = " (@N_LocationID) ";

                if (isWHM)
                    sql = "select  N_CompanyID,N_ItemID,N_LocationID,X_BatchCode,' Exp Date : ' + CONVERT(varchar(110),D_ExpiryDate,106) + ', Qty : '+ cast(n_GRNQty as varchar)+' '+ X_ItemUnit as Stock_Disp,D_ExpiryDate,Stock,X_ItemUnit,N_Qty as N_BaseUnitQty,X_LocationName,N_ItemUnitID,X_Bin,X_Row,X_Rack,X_Room,x_Shelf,N_LPrice from vw_BatchwiseStockDisp_MRNDetails where N_CompanyID=@N_CompanyID and N_ItemID=@N_ItemID and N_LocationID in " + x_Criteria + " and CurrentStock>0 and ISNULL(X_BatchCode,'')<>'' order by D_ExpiryDate ASC";
                else
                    sql = "select  N_CompanyID,N_ItemID,N_LocationID,X_BatchCode,' Exp Date : ' + CONVERT(varchar(110),D_ExpiryDate,106) + ', Qty : '+ cast(Stock as varchar)+' '+ X_ItemUnit as Stock_Disp,D_ExpiryDate,Stock,X_ItemUnit,N_Qty as N_BaseUnitQty,X_LocationName,N_ItemUnitID,X_Bin,X_Row,X_Rack,X_Room,x_Shelf,N_LPrice from vw_BatchwiseStockDisp where N_CompanyID=@N_CompanyID and N_ItemID=@N_ItemID and N_LocationID in " + x_Criteria + " and CurrentStock>0 and ISNULL(X_BatchCode,'')<>'' order by D_ExpiryDate ASC";


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    masterTable = dLayer.ExecuteDataTable(sql, mParamsList, connection);
                }
                if (masterTable.Rows.Count == 0) { return Ok(_api.Notice("No Data Found")); }
                return Ok(_api.Success(masterTable));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        //  [HttpGet("costAndStock_Test")]
        // public ActionResult GetCostAndStock(int nItemID,int nCompanyID)
        // {
        //     DataTable dt = new DataTable();
        //     SortedList OutPut = new SortedList();
        //     string sqlCostCommand="",sqlStockCommad="";
        //     double UnitQty=1, nLastCost = 0;
        //     int nUnitID = 0;

        //     sqlCostCommand ="Select top 1 Inv_PurchaseDetails.N_PPrice,Inv_PurchaseDetails.N_ItemUnitID from Inv_Purchase INNER JOIN Inv_PurchaseDetails ON Inv_Purchase.N_CompanyID = Inv_PurchaseDetails.N_CompanyID and Inv_Purchase.N_PurchaseID = Inv_PurchaseDetails.N_PurchaseID where Inv_PurchaseDetails.N_ItemID = " + nItemID + " and Inv_Purchase.N_CompanyID = " + nCompanyID + " order by Inv_Purchase.N_PurchaseID desc";
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             dt = dLayer.ExecuteDataTable(sqlCostCommand, connection);

        //             if (dt.Rows.Count > 0)
        //             {
        //                 DataRow drow = dt.Rows[0];
        //                 nLastCost =myFunctions.getVAL(drow["N_PPrice"].ToString());
        //                 nUnitID =myFunctions.getIntVAL(drow["N_ItemUnitID"].ToString());
        //             }

        //             object res = dLayer.ExecuteScalar("Select N_Qty from Inv_ItemUnit Where N_CompanyID=" + nCompanyID + " and N_ItemID = " + nItemID + " and N_ItemUnitID=" + nUnitID,  connection);
        //             if (res != null)
        //                 UnitQty = myFunctions.getVAL(res.ToString());

        //             nLastCost = nLastCost / UnitQty;
        //         }
        //         OutPut.Add("LastCost", nLastCost);
        //         if (dt.Rows.Count == 0)
        //         {
        //             return Ok(_api.Warning("no result found"));
        //         }
        //         else
        //         {
        //             return Ok(_api.Success(OutPut));
        //         }

        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(_api.Error(User, e));
        //     }

        // }

        [HttpGet("costAndStock")]
        public ActionResult GetCostAndStock(int nItemID, int nLocationID, string xBatch, DateTime dDate,int nCustomerID)
        {
            DataTable dt = new DataTable();
            string sqlCommandText = "";
            if (xBatch == null) xBatch = "";
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (xBatch != "")
                        sqlCommandText = "Select vw_InvItem_Search.N_ItemID,dbo.SP_BatchStock(vw_InvItem_Search.N_ItemID," + nLocationID + ",'" + xBatch + "',0)As N_AvlStock ,dbo.SP_LastPCost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID," + nLocationID + ") As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice  From vw_InvItem_Search Where N_ItemID=" + nItemID + " and N_CompanyID=" + nCompanyID + " and ISNULL(N_ItemTypeID,0)=0";
                    else
                    {
                        bool bStockByDate = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "ShowAvlStockByDate", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                        if (!bStockByDate)
                            sqlCommandText = "Select vw_InvItem_Search.N_ItemID,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID," + nLocationID + ",'" + xBatch + "', 'location')As N_AvlStock ,dbo.SP_LastPCost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID," + nLocationID + ") As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice From vw_InvItem_Search Where N_ItemID=" + nItemID + " and N_CompanyID=" + nCompanyID;
                        else
                            sqlCommandText = "Select vw_InvItem_Search.N_ItemID,dbo.SP_GenGetStockByDate(vw_InvItem_Search.N_ItemID," + nLocationID + ",'" + xBatch + "', 'location','" + myFunctions.getDateVAL(dDate) + "')As N_AvlStock ,dbo.SP_LastPCost(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID," + nLocationID + ") As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice  From vw_InvItem_Search Where N_ItemID=" + nItemID + " and N_CompanyID=" + nCompanyID + " and ISNULL(N_ItemTypeID,0)=0";
                    }
                  


                    dt = dLayer.ExecuteDataTable(sqlCommandText, connection);
                      dt = myFunctions.AddNewColumnToDataTable(dt, "N_LastSoldPrice", typeof(double), 0);
                    object lastSoldPrice=dLayer.ExecuteScalar("select top 1 N_Sprice from vw_Inv_CustomerTransactionByItem where N_CompanyID="+nCompanyID+" and  N_ItemID="+nItemID+" and N_CustomerID="+nCustomerID+" and X_Type='SALES' order by D_SalesDate,N_SalesId desc",connection);
                    if(lastSoldPrice!=null)
                    {
                        dt.Rows[0]["N_LastSoldPrice"]=myFunctions.getVAL(lastSoldPrice.ToString());
                    }

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
                return Ok(_api.Error(User, e));
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

        [HttpGet("productHistory")]
        public ActionResult GetProductHistoryList(int nItemID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, int nCustomerID, bool nShowCustomer, string xType)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";

                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nItemID);
                    Params.Add("@p3", nCustomerID);
                    Params.Add("@p4", xType);

                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and (X_CustomerName like '%" + xSearchkey + "%' or X_ReceiptNo like '%" + xSearchkey + "%' or N_Qty like '%" + xSearchkey + "%' or cast(D_SalesDate as VarChar) like '%" + xSearchkey + "%')";

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_SalesDetailsID desc";
                    else
                    {
                        xSortBy = " order by " + xSortBy;
                    }

                    // if (Count == 0)
                    //     sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_CustomerTransactionByItem where N_CompanyID=@p1 and N_ItemID=@p2 and N_CustomerID=@p3 " + Searchkey + " " + xSortBy;
                    // else
                    //     sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_CustomerTransactionByItem where N_CompanyID=@p1 and n_ItemID=@p2 and N_CustomerID=@p3 " + Searchkey + " and N_SalesDetailsID not in (select top(" + Count + ") N_SalesDetailsID from vw_Inv_CustomerTransactionByItem where N_CompanyID=@p1 and n_ItemID=@p2 and N_CustomerID=@p3 " + xSearchkey + xSortBy + " ) " + xSortBy;
                    if (nCustomerID > 0)
                    {
                        if (Count == 0)
                            sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_CustomerTransactionByItem where N_CompanyID=@p1 and  N_ItemID=@p2 and N_CustomerID=@p3 and X_Type=@p4 " + Searchkey + " " + xSortBy;
                        else
                            sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_CustomerTransactionByItem where N_CompanyID=@p1 and n_ItemID=@p2 and N_CustomerID=@p3 and X_Type@p4 " + Searchkey + " and N_SalesDetailsID not in (select top(" + Count + ") N_SalesDetailsID from vw_Inv_CustomerTransactionByItem where N_CompanyID=@p1 and n_ItemID=@p2 and N_CustomerID=@p3  " + xSearchkey + " ) " + xSortBy;
                    }
                    else
                    {
                        if (nShowCustomer == true)
                        {
                            if (Count == 0)
                                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_CustomerTransactionByItem where N_CompanyID=@p1 and  N_ItemID=@p2 and X_Type=@p4 " + Searchkey + " " + xSortBy;
                            else
                                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_CustomerTransactionByItem where N_CompanyID=@p1 and n_ItemID=@p2 and X_Type=@p4 " + Searchkey + " and N_SalesDetailsID not in (select top(" + Count + ") N_SalesDetailsID from vw_Inv_CustomerTransactionByItem where N_CompanyID=@p1 and n_ItemID=@p2 " + xSearchkey + " ) " + xSortBy;

                        }

                    }
                    SortedList OutPut = new SortedList();

                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    // sqlCommandCount = "select count(*) as N_Count from vw_Inv_CustomerTransactionByItem where N_CompanyID=@p1 and n_ItemID=@p2 and N_CustomerID=@p3 " + Searchkey + "";
                    if (nShowCustomer == true)
                        sqlCommandCount = "select count(*) as N_Count from vw_Inv_CustomerTransactionByItem where N_CompanyID=@p1 and n_ItemID=@p2  and X_Type=@p4 " + Searchkey + "";
                    else
                        sqlCommandCount = "select count(*) as N_Count from vw_Inv_CustomerTransactionByItem where N_CompanyID=@p1 and n_ItemID=@p2 and N_CustomerID=@p3  and X_Type=@p4 " + Searchkey + "";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);

                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    return Ok(_api.Success(OutPut));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("productPurchaseHistory")]
        public ActionResult GetProductPurchaseHistoryList(int nItemID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, bool bIncludePriceQuote, int nVendorID, bool nShowVendor)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "", Cond = "";

                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nItemID);
                    Params.Add("@p3", nVendorID);



                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and (X_CustomerName like '%" + xSearchkey + "%' or X_ReceiptNo like '%" + xSearchkey + "%' or N_Qty like '%" + xSearchkey + "%' or cast(D_SalesDate as VarChar) like '%" + xSearchkey + "%')";

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_PurchaseDetailsID desc";
                    else
                    {
                        xSortBy = " order by " + xSortBy;
                    }

                    if (!bIncludePriceQuote)
                        Cond = " and ISNULL(B_IsPriceQuote,0)=0";

                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_inv_vendorTransactionByitem where N_CompanyID=@p1 and N_ItemID=@p2 " + Searchkey + " " + Cond + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_inv_vendorTransactionByitem where N_CompanyID=@p1 and n_ItemID=@p2 " + Searchkey + " and N_PurchaseDetailsID not in (select top(" + Count + ") N_PurchaseDetailsID from vw_inv_vendorTransactionByitem where N_CompanyID=@p1 and n_ItemID=@p2 " + xSearchkey + Cond + xSortBy + " ) " + Cond + " " + xSortBy;



                    if (nVendorID > 0)
                    {
                        if (Count == 0)
                            sqlCommandText = "select top(" + nSizeperpage + ") * from vw_inv_vendorTransactionByitem where N_CompanyID=@p1 and  N_ItemID=@p2 and N_VendorID=@p3" + Searchkey + " " + xSortBy;
                        else
                            sqlCommandText = "select top(" + nSizeperpage + ") * from vw_inv_vendorTransactionByitem where N_CompanyID=@p1 and n_ItemID=@p2 and N_VendorID=@p3" + Searchkey + " and N_PurchaseDetailsID not in (select top(" + Count + ") N_PurchaseDetailsID from vw_inv_vendorTransactionByitem where N_CompanyID=@p1 and n_ItemID=@p2 and N_VendorID=@p3  " + xSearchkey + xSortBy + " ) " + xSortBy;
                    }
                    else
                    {
                        if (nShowVendor == true)
                        {
                            if (Count == 0)
                                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_inv_vendorTransactionByitem where N_CompanyID=@p1 and  N_ItemID=@p2" + Searchkey + " " + xSortBy;
                            else
                                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_inv_vendorTransactionByitem where N_CompanyID=@p1 and n_ItemID=@p2" + Searchkey + " and N_PurchaseDetailsID not in (select top(" + Count + ") N_PurchaseDetailsID from vw_inv_vendorTransactionByitem where N_CompanyID=@p1 and n_ItemID=@p2 " + xSearchkey + xSortBy + " ) " + xSortBy;

                        }

                    }



                    SortedList OutPut = new SortedList();

                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    // sqlCommandCount = "select count(*) as N_Count from vw_inv_vendorTransactionByitem where N_CompanyID=@p1 and n_ItemID=@p2 " + Searchkey + "" + Cond + " ";
                    sqlCommandCount = "select count(*) as N_Count from vw_inv_vendorTransactionByitem where N_CompanyID=@p1 and n_ItemID=@p2 and N_VendorID=@p3" + Searchkey + "";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);

                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    return Ok(_api.Success(OutPut));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("translate")]
        public ActionResult Translate(string xText)
        {
            try
            {
                SortedList OutPut = new SortedList();
                string Artext = Translate(xText, "en", "ar");
                if (xText == null)
                    Artext = "";
                OutPut.Add("arabic", Artext);
                return Ok(_api.Success(OutPut));

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        public String Translate(String text, string fromLanguage, string toLanguage)
        {
            //Translate("test","ar");
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={HttpUtility.UrlEncode(text)}";
            var webClient = new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
            var result = webClient.DownloadString(url);
            try
            {
                result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                return result;
            }
            catch
            {
                return "Error";
            }
        }
        private string Translate(string text, string l)
        {
            string translated = null;
            HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create
            ("http://translate.google.com/#en/ar/test");
            HttpWebResponse res = (HttpWebResponse)hwr.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream());
            string html = sr.ReadToEnd();
            int rawlength1 = html.IndexOf("<span id=otq><b>");
            string rawStr1 = html.Substring(rawlength1);
            int rawlength2 = rawStr1.IndexOf("</b>");
            string rawstr2 = rawStr1.Substring(0, rawlength2);
            translated = rawstr2.Replace("<span id=otq><b>", "");
            //tbStringToTranslate.Text = text;
            return translated;
        }


        
             [HttpGet("ProductTypeList")]
        public ActionResult ProductType()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
           
            string sqlCommandText = "select * from vw_InvPRSProductType where n_Type=1";
           

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
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
                return Ok(_api.Error(User,e));
            }
        }





    }


}