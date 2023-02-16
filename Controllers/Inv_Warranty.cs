using AutoMapper;
using SmartxAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("warranty")]
    [ApiController]
    public class Inv_Warranty : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;

        public Inv_Warranty(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1395;
        }
        // [HttpPost("save")]
        // public ActionResult SaveData([FromBody] DataSet ds)
        // {
        //     try
        //     {
        // using (SqlConnection connection = new SqlConnection(connectionString))
        // {

        //     connection.Open();
        //     SqlTransaction transaction = connection.BeginTransaction();
        //     DataTable MasterTable;
        //     DataTable DetailTable;
        //     string DocNo = "";
        //     MasterTable = ds.Tables["master"];
        //     DetailTable = ds.Tables["details"];
        //     DataRow MasterRow = MasterTable.Rows[0];
        //     SortedList Params = new SortedList();
        //     int nCompanyID = myFunctions.GetCompanyID(User);
        //     int nServiceID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ServiceID"].ToString());
        //     string X_ServiceCode = MasterTable.Rows[0]["X_ServiceCode"].ToString();
        //     if (nServiceID > 0)
        //     {
        //         dLayer.DeleteData("Inv_ServiceMaster", "N_ServiceID", nServiceID, "", connection, transaction);
        //         dLayer.DeleteData("Inv_ServiceDetails", "N_ServiceID", nServiceID, "", connection, transaction);
        //     }
        //     DocNo = MasterRow["X_ServiceCode"].ToString();
        //     if (X_ServiceCode == "@Auto")
        //     {
        //         Params.Add("N_CompanyID", nCompanyID);
        //         Params.Add("N_FormID", this.FormID);



        //         while (true)
        //         {

        //             object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_ServiceMaster Where X_TaskCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
        //             if (N_Result == null) DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
        //             break;
        //         }
        //         X_ServiceCode = DocNo;


        //         if (X_ServiceCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate")); }
        //         MasterTable.Rows[0]["X_ServiceCode"] = X_ServiceCode;


        //     }

        //     nServiceID = dLayer.SaveData("Inv_ServiceMaster", "N_ServiceID", MasterTable, connection, transaction);
        //     if (nServiceID <= 0)
        //     {
        //         transaction.Rollback();
        //         return Ok(_api.Error(User, "Unable To Save"));
        //     }
        //     for (int i = 0; i < DetailTable.Rows.Count; i++)
        //     {
        //         DetailTable.Rows[1]["N_ServiceID"] = nServiceID;
        //     }
        //     int nServiceDetailsID = dLayer.SaveData("Inv_ServiceDetails", "N_ServiceDetailsID", DetailTable, connection, transaction);
        //     if (nServiceDetailsID <= 0)
        //     {
        //         transaction.Rollback();
        //         return Ok(_api.Error(User, "Unable To Save"));
        //     }
        //     transaction.Commit();
        //     return Ok(_api.Success("Saved"));
        // }
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(_api.Error(User, ex));
        //     }
        // }

        // [HttpGet("list")]
        // public ActionResult ProductionOrderList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        // {
        //     DataTable dt = new DataTable();
        //     SortedList Params = new SortedList();
        //     int nCompanyID = myFunctions.GetCompanyID(User);
        //     string sqlCommandCount = "";
        //     int Count = (nPage - 1) * nSizeperpage;
        //     string sqlCommandText = "";
        //     string Searchkey = "";
        //     Params.Add("@p1", nCompanyID);
        //     Params.Add("@p2", nFnYearId);
        //     if (xSearchkey != null && xSearchkey.Trim() != "")
        //         Searchkey = "and ( X_ServiceCode like '%" + xSearchkey + "%' or  D_EntryDate like '%" + xSearchkey + "%'  ) ";

        //     if (xSortBy == null || xSortBy.Trim() == "")
        //         xSortBy = " order by N_ServiceID desc";
        //     // xSortBy = " order by batch desc,D_TransDate desc";
        //     else
        //         xSortBy = " order by " + xSortBy;
        //     if (Count == 0)
        //         sqlCommandText = "select top(" + nSizeperpage + ")  * from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2  " + Searchkey;
        //     else
        //         sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "and N_ServiceID not in (select top(" + Count + ") N_ServiceID from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2) " + Searchkey;


        //     SortedList OutPut = new SortedList();


        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             dt = dLayer.ExecuteDataTable(sqlCommandText + xSortBy, Params, connection);

        //             sqlCommandCount = "select count(1) as N_Count  from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey;
        //             object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
        //             OutPut.Add("Details", _api.Format(dt));
        //             OutPut.Add("TotalCount", TotalCount);
        //             if (dt.Rows.Count == 0)
        //             {
        //                 return Ok(_api.Warning("No Results Found"));
        //             }
        //             else
        //             {
        //                 return Ok(_api.Success(OutPut));
        //             }

        //         }

        //     }
        //     catch (Exception e)
        //     {
        //         return BadRequest(_api.Error(User, e));
        //     }
        // }
        [HttpGet("details")]
        public ActionResult ServiceDetails(string xWarrantyRefCode)
        {


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable ProductInfoTable = new DataTable();

                    List<SortedList> ProductInfo = new List<SortedList>();


                    string Mastersql = "";
                    string DetailSql = "";
                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@xWarrantyRefCode", xWarrantyRefCode);
                    Mastersql = ""
                    + "SELECT   Top(1)     Inv_WarrantyContract.*, Inv_Customer.X_CustomerCode, Inv_Customer.X_CustomerName "
                    + "FROM            Inv_WarrantyContract LEFT OUTER JOIN "
                    + "                         Inv_Customer ON Inv_WarrantyContract.N_FnYearID = Inv_Customer.N_FnYearID AND Inv_WarrantyContract.N_CompanyID = Inv_Customer.N_CompanyID AND "
                    + "                         Inv_WarrantyContract.N_CustomerID = Inv_Customer.N_CustomerID where Inv_WarrantyContract.X_WarrantyNo=@xWarrantyRefCode and Inv_WarrantyContract.N_CompanyID=@nCompanyID and isnull(Inv_WarrantyContract.B_InActive,0)=0  order by N_WarrantyID desc";
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    if(MasterTable.Rows.Count==1)
                    {
                        int N_SalesID=myFunctions.getIntVAL(dLayer.ExecuteScalar("select Top 1 N_SalesID from Inv_Sales where  N_CompanyID="+myFunctions.GetCompanyID(User)+" and X_Barcode='"+MasterTable.Rows[0]["X_WarrantyNo"]+"' order By N_SalesID desc",Params, connection).ToString());
                        int nCount =myFunctions.getIntVAL(dLayer.ExecuteScalar(" select count(1)  from Inv_SaleAmountDetails where  N_CompanyID="+myFunctions.GetCompanyID(User)+" and N_SalesID="+N_SalesID+" ",Params, connection).ToString());
                        string x_PayMode="";
                        if(nCount==1)
                            x_PayMode=dLayer.ExecuteScalar("SELECT Inv_Customer.X_CustomerName FROM  Inv_SaleAmountDetails INNER JOIN Inv_Customer ON Inv_SaleAmountDetails.N_CompanyID = Inv_Customer.N_CompanyID AND Inv_SaleAmountDetails.N_CustomerID = Inv_Customer.N_CustomerID where  Inv_SaleAmountDetails.N_CompanyID="+myFunctions.GetCompanyID(User)+" and Inv_SaleAmountDetails.N_SalesID="+N_SalesID+"",Params, connection).ToString();
                        else if(nCount >  1)
                           x_PayMode="Multiple PayMode";
                        string X_ReceiptNo=dLayer.ExecuteScalar("select Top 1 X_ReceiptNo from Inv_Sales where  N_CompanyID="+myFunctions.GetCompanyID(User)+" and X_Barcode='"+MasterTable.Rows[0]["X_WarrantyNo"]+"' order By N_SalesID desc",Params, connection).ToString();
                        string D_SalesDate=dLayer.ExecuteScalar("select Top 1 D_SalesDate from Inv_Sales where  N_CompanyID="+myFunctions.GetCompanyID(User)+" and X_Barcode='"+MasterTable.Rows[0]["X_WarrantyNo"]+"' order By N_SalesID desc",Params, connection).ToString();
                        string x_Balance=dLayer.ExecuteScalar("select Top 1 isnull(N_BalanceAmt,0) from [vw_InvSalesInvoiceNo_Search_Cloud] where  N_CompanyID="+myFunctions.GetCompanyID(User)+" and N_SalesID="+N_SalesID+" order By N_SalesID desc",Params, connection).ToString();
                       
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_ReceiptNo", typeof(string), X_ReceiptNo);
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "D_SalesDate", typeof(string), D_SalesDate);
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_Balance", typeof(string), x_Balance);
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_PayMode", typeof(string), x_PayMode);
                        

                    }
                                      
                    int N_WarrantyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_WarrantyID"].ToString());
                    MasterTable.AcceptChanges();
                    Params.Add("@nWarrantyID", N_WarrantyID);
                    MasterTable = _api.Format(MasterTable, "Master");
                    //Detail
                    // DetailSql = ""
                    // + "SELECT        Inv_WarrantyContractDetails.N_CompanyID, Inv_WarrantyContractDetails.N_WarrantyID, Inv_WarrantyContractDetails.N_WarrantyDetailsID, Inv_WarrantyContractDetails.N_ItemID, Inv_WarrantyContractDetails.N_MainItemID, "
                    // + "                         Inv_WarrantyContractDetails.N_Qty, Inv_WarrantyContractDetails.N_BranchID, Inv_WarrantyContractDetails.N_LocationID, Inv_WarrantyContractDetails.N_ItemUnitID, Inv_WarrantyContractDetails.X_ItemRemarks, "
                    // + "                         Inv_ItemUnit.X_ItemUnit, Inv_ItemMaster.X_ItemCode, Inv_ItemMaster.X_ItemName "
                    // + "FROM            Inv_WarrantyContractDetails LEFT OUTER JOIN "
                    // + "                         Inv_ItemMaster ON Inv_WarrantyContractDetails.N_ItemID = Inv_ItemMaster.N_ItemID AND Inv_WarrantyContractDetails.N_CompanyID = Inv_ItemMaster.N_CompanyID LEFT OUTER JOIN "
                    // + "                         Inv_ItemUnit ON Inv_ItemMaster.N_CompanyID = Inv_ItemUnit.N_CompanyID AND Inv_ItemMaster.N_ItemUnitID = Inv_ItemUnit.N_ItemUnitID where Inv_WarrantyContractDetails.N_CompanyID=@nCompanyID and Inv_WarrantyContractDetails.N_WarrantyID=@nWarrantyID";

                    DetailSql = "SELECT Inv_WarrantyContractDetails.N_CompanyID, Inv_WarrantyContractDetails.N_WarrantyID, Inv_WarrantyContractDetails.N_WarrantyDetailsID, Inv_WarrantyContractDetails.N_ItemID, Inv_WarrantyContractDetails.N_MainItemID, " +
                         " Inv_WarrantyContractDetails.N_BranchID, Inv_WarrantyContractDetails.N_LocationID, Inv_WarrantyContractDetails.N_ItemUnitID, Inv_WarrantyContractDetails.X_ItemRemarks, Inv_ItemUnit.X_ItemUnit, " +
                         " Inv_ItemMaster_1.X_ItemCode, Inv_ItemMaster_1.X_ItemName, Inv_ItemMaster_1.N_ClassID, Inv_WarrantyContractDetails.N_ServiceItemID, Inv_ItemMaster.X_ItemCode AS X_ServiceItemCode, " +
                         " Inv_ItemMaster.X_ItemName AS X_ServiceItem, ISNULL(Inv_WarrantyContractDetails.N_Qty, 0) - ISNULL(vw_Inv_WarrantyQtyDetails.N_UsedQty, 0) " +
                         " AS N_AvlQty, 0 as N_Qty, ISNULL(vw_Inv_WarrantyQtyDetails.N_UsedQty, 0) AS N_UsedQty " +
                        " FROM Inv_ItemMaster RIGHT OUTER JOIN " +
                         " Inv_WarrantyContractDetails LEFT OUTER JOIN " +
                         " vw_Inv_WarrantyQtyDetails ON Inv_WarrantyContractDetails.N_ItemID = vw_Inv_WarrantyQtyDetails.N_ItemID AND Inv_WarrantyContractDetails.N_CompanyID = vw_Inv_WarrantyQtyDetails.N_CompanyID AND " +
                         " Inv_WarrantyContractDetails.N_WarrantyID = vw_Inv_WarrantyQtyDetails.N_WarrantyID ON Inv_ItemMaster.N_ItemID = Inv_WarrantyContractDetails.N_ServiceItemID AND " +
                         " Inv_ItemMaster.N_CompanyID = Inv_WarrantyContractDetails.N_CompanyID LEFT OUTER JOIN " +
                         " Inv_ItemUnit RIGHT OUTER JOIN " +
                         " Inv_ItemMaster AS Inv_ItemMaster_1 ON Inv_ItemUnit.N_CompanyID = Inv_ItemMaster_1.N_CompanyID AND Inv_ItemUnit.N_ItemUnitID = Inv_ItemMaster_1.N_ItemUnitID ON " +
                         " Inv_WarrantyContractDetails.N_ItemID = Inv_ItemMaster_1.N_ItemID AND Inv_WarrantyContractDetails.N_CompanyID = Inv_ItemMaster_1.N_CompanyID " +
                        " where Inv_WarrantyContractDetails.N_CompanyID=@nCompanyID and Inv_WarrantyContractDetails.N_WarrantyID=@nWarrantyID " +
                        " GROUP BY Inv_WarrantyContractDetails.N_CompanyID, Inv_WarrantyContractDetails.N_WarrantyID, Inv_WarrantyContractDetails.N_WarrantyDetailsID, Inv_WarrantyContractDetails.N_ItemID, Inv_WarrantyContractDetails.N_MainItemID, " +
                         " Inv_WarrantyContractDetails.N_BranchID, Inv_WarrantyContractDetails.N_LocationID, Inv_WarrantyContractDetails.N_ItemUnitID, Inv_WarrantyContractDetails.X_ItemRemarks, Inv_ItemUnit.X_ItemUnit, " +
                         " Inv_ItemMaster_1.X_ItemCode, Inv_ItemMaster_1.X_ItemName, Inv_ItemMaster_1.N_ClassID, Inv_WarrantyContractDetails.N_ServiceItemID, Inv_ItemMaster.X_ItemCode, " +
                         " Inv_ItemMaster.X_ItemName, Inv_WarrantyContractDetails.N_Qty, vw_Inv_WarrantyQtyDetails.N_UsedQty ";


                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");


//                     foreach (DataRow DRows in DetailTable.Rows)
//                     {
//                         if (myFunctions.getIntVAL(DRows["N_ClassID"].ToString()) == 1)
//                         {

//                             string balanceSql = " SELECT        SUM(ISNULL(Inv_ServiceDetails.N_Qty, 0)) AS N_UsedQty FROM            Inv_ItemDetails RIGHT OUTER JOIN "+
//                          " Inv_ItemMaster ON Inv_ItemDetails.N_MainItemID = Inv_ItemMaster.N_ItemID AND Inv_ItemDetails.N_CompanyID = Inv_ItemMaster.N_CompanyID RIGHT OUTER JOIN "+
//                          " Inv_ServiceDetails ON Inv_ItemDetails.N_ItemID = Inv_ServiceDetails.N_ItemID AND Inv_ItemMaster.N_CompanyID = Inv_ServiceDetails.N_CompanyID AND  "+
//                          " Inv_ItemDetails.N_CompanyID = Inv_ServiceDetails.N_CompanyID RIGHT OUTER JOIN "+
//                          " Inv_ServiceMaster ON Inv_ServiceDetails.N_CompanyID = Inv_ServiceMaster.N_CompanyID AND Inv_ServiceDetails.N_ServiceID = Inv_ServiceMaster.N_ServiceID " +
// "where Inv_ItemMaster.N_ClassID=1 and  Inv_ServiceDetails.N_ItemID=" + DRows["N_ItemID"].ToString() + " and Inv_ServiceMaster.N_CompanyID=" + myFunctions.GetCompanyID(User) + " and Inv_ServiceMaster.N_WarrantyID=" + N_WarrantyID +
// " GROUP BY Inv_ServiceMaster.N_CompanyID, Inv_ServiceMaster.N_WarrantyID ";

//                             object balance = dLayer.ExecuteScalar(balanceSql, Params, connection);
//                             if (balance == null)
//                                 balance = 0;

//                             int qty = myFunctions.getIntVAL(DRows["N_UsedQty"].ToString());
//                             DRows["N_AvlQty"] = qty - myFunctions.getIntVAL(balance.ToString());


//                         }

//                     }
DetailTable.AcceptChanges();

                    //Product Information 
                    SortedList element = new SortedList();
                    if (DetailTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int N_ItemID = myFunctions.getIntVAL(DetailTable.Rows[0]["N_MainItemID"].ToString());
                    object productName = dLayer.ExecuteScalar("Select X_ItemName from Inv_ItemMaster Where N_ItemID =" + N_ItemID + " and N_CompanyID= @nCompanyID ", Params, connection);
                    element.Add("Product", productName.ToString());
                    string[] name = { "Model", "Colour", "PhoneCategory", "Device", "Category" };
                    int i = 0;

                    //int nParentID = myFunctions.getIntVAL(DetailTable.Rows[0]["N_CategoryDisplayID"].ToString());
                    int nParentID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_CategoryDisplayID from Inv_ItemCategoryDisplayMaster where N_ItemID=" + N_ItemID + "", Params, connection).ToString());
                    while (nParentID > 0 && i <= 4)
                    {

                        object phoneName = dLayer.ExecuteScalar("Select X_CategoryDisplay from Inv_ItemCategoryDisplay Where N_CategoryDisplayID =" + nParentID + " and N_CompanyID= @nCompanyID ", Params, connection);
                        element.Add(name[i], phoneName.ToString());
                        nParentID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select N_ParentID from Inv_ItemCategoryDisplay Where N_CategoryDisplayID =" + nParentID + " and N_CompanyID= @nCompanyID ", Params, connection).ToString());
                        i = i + 1;
                    }
                    ProductInfo.Add(element);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "ProductInformations", typeof(List<SortedList>), ProductInfo);



                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
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
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();

                    int nClaimID = myFunctions.getIntVAL(MasterRow["n_ClaimID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string x_WarrantyNo=(MasterRow["x_WarrantyNo"].ToString());
                    MasterTable.Columns.Remove("x_WarrantyNo");
                    string xClaimCode = MasterRow["x_ClaimCode"].ToString();

                    if (xClaimCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        xClaimCode = dLayer.GetAutoNumber("Inv_WarrantyClaim", "X_ClaimCode", Params, connection, transaction);
                        if (xClaimCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Warranty Process code");
                        }
                        MasterTable.Rows[0]["X_ClaimCode"] = xClaimCode;
                    }

                    nClaimID = dLayer.SaveData("Inv_WarrantyClaim", "N_ClaimID", "", "", MasterTable, connection, transaction);
                    if (nClaimID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Warranty Process");
                    }
                    dLayer.DeleteData("Inv_WarrantyClaimDetails", "N_ClaimID", nClaimID, "", connection, transaction);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_ClaimID"] = nClaimID;
                    }
                    int nClaimDetailID = dLayer.SaveData("Inv_WarrantyClaimDetails", "N_ClaimDetailID", DetailTable, connection, transaction);
                    if (nClaimDetailID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Warranty Process");
                    }
                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_ClaimID", nClaimID);
                    Result.Add("x_ClaimCode", xClaimCode);
                    Result.Add("n_ClaimDetailID", nClaimDetailID);
                    Result.Add("x_WarrantyNo", x_WarrantyNo);


                    return Ok(_api.Success(Result, "Warranty Process saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("list")]
        public ActionResult WarrantyList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_ClaimCode like '%" + xSearchkey + "%' or X_CustomerName like '%" + xSearchkey + "%' or X_WarrantyNo like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ClaimID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "N_ClaimID":
                        xSortBy = "N_ClaimID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_WarrantyClaim where N_CompanyID=@nCompanyID  " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_WarrantyClaim where N_CompanyID=@nCompanyID  " + Searchkey + " and N_ClaimID not in (select top(" + Count + ") N_ClaimID from vw_Inv_WarrantyClaim where N_CompanyID=@nCompanyID " + xSortBy + " ) " + " " + xSortBy;

            Params.Add("@nCompanyID", nCompanyID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(1) as N_Count  from vw_Inv_WarrantyClaim where N_CompanyID=@nCompanyID " + Searchkey + "";
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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("warrantydetails")]
        public ActionResult WarrantyListDetails(string xClaimCode)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable DataTable = new DataTable();

                    string Mastersql = "";
                    string DetailSql = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@xClaimCode", xClaimCode);
                    Mastersql = "select * from vw_Inv_WarrantyClaim where N_CompanyID=@nCompanyID and X_ClaimCode=@xClaimCode ";
                   
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int nClaimID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ClaimID"].ToString());
                    Params.Add("@nClaimID", nClaimID);

                    MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from vw_Inv_WarrantyClaimDetails where N_CompanyID=@nCompanyID and N_ClaimID=@nClaimID ";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
        [HttpGet("cancelWarranty")]
        public ActionResult CancelWarranty(int nWarrantyID )
        {
             SortedList Params = new SortedList();
             DataTable dt = new DataTable();
             int nCompanyID=myFunctions.GetCompanyID(User);
             Params.Add("@nCompanyID",nCompanyID);
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                   dLayer.ExecuteNonQuery("Update Inv_WarrantyContract set B_InActive=1 where N_CompanyID=@nCompanyID and N_WarrantyID="+nWarrantyID+" ", Params, connection);
                }
     
                return Ok(_api.Success( "Warranty Process Canceled"));

             
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
        
    }
    }

