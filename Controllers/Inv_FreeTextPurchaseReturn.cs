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
    [Route("freeTextReturn")]
    [ApiController]
    public class Inv_FreeTextPurchaseReturn : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;

        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly IMyAttachments myAttachments;
        public Inv_FreeTextPurchaseReturn(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 384;
            myAttachments = myAtt;

        }



        [HttpGet("list")]
        public ActionResult GetPurchaseReturnList(int? nCompanyId, int nFnYearID, string xTransType, bool bAllBranchData, int nBranchID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "", xCriteria = "", sqlCommandCount = "";
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nFnYearID);
                    Params.Add("@p3", nBranchID);
                    Params.Add("@p4", xTransType);
                    string Searchkey = "";
                    // if (xSearchkey != null && xSearchkey.Trim() != "")
                    //     Searchkey = "and ([Invoice No] like '%" + xSearchkey + "%' or [Invoice Date] like '%" + xSearchkey + "%')";
                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and ([Invoice No] like '%" + xSearchkey + "%' or invoiceDate like '%" + xSearchkey + "%')";


                    // if (xSortBy == null || xSortBy.Trim() == "")
                    //     xSortBy = " order by N_PurchaseID desc";
                    // else
                    //     xSortBy = " order by " + xSortBy;

                    
                    if (bAllBranchData)
                        xCriteria = " N_PurchaseType=0 and X_TransType=@p4 and N_FnYearID=@p2 and N_CompanyID=@p1";
                    else
                        xCriteria = " N_PurchaseType=0 and X_TransType=@p4 and N_FnYearID=@p2 and N_BranchID=@p3 and N_CompanyID=@p1";
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") [Invoice Date] as invoiceDate ,[Invoice No] as invoiceNo ,Vendor,InvoiceNetAmt,x_Description,n_InvDueDays,X_VendorName_Ar,X_UserName from vw_InvFreeTextPurchaseReturn_Search where " + xCriteria +" "+ Searchkey;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") [Invoice Date] as invoiceDate,[Invoice No] as invoiceNo ,Vendor,InvoiceNetAmt,x_Description,n_InvDueDays,X_VendorName_Ar,X_UserName from vw_InvFreeTextPurchaseReturn_Search where " + xCriteria + Searchkey + " and N_PurchaseID not in (select top(" + Count + ") N_PurchaseID from vw_InvFreeTextPurchaseReturn_Search where " + xCriteria + Searchkey + " ) ";

                    SortedList OutPut = new SortedList();
                    dt = dLayer.ExecuteDataTable(sqlCommandText + xSortBy, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count  from vw_InvFreeTextPurchaseReturn_Search where " + xCriteria + Searchkey;
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
                    DataTable CostCenterTable;
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    CostCenterTable = ds.Tables["CostCenterTable"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nUserID = myFunctions.GetUserID(User);
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                    int nPurchaseID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PurchaseID"].ToString());
                    string X_InvoiceNo = MasterTable.Rows[0]["X_InvoiceNo"].ToString();
                    int nPurchaseMapID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FreeTextReturnID"].ToString());
                    string xTransType = "CREDIT NOTE";
                    DocNo = MasterRow["X_InvoiceNo"].ToString();
                    DataTable Attachment = ds.Tables["attachments"];
                    String xButtonAction="";


                     if (!myFunctions.CheckActiveYearTransaction(nCompanyID, nFnYearID, Convert.ToDateTime(MasterTable.Rows[0]["D_InvoiceDate"].ToString()), dLayer, connection, transaction))
                    {
                        object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID=" + nCompanyID + " and convert(date ,'" + MasterTable.Rows[0]["D_InvoiceDate"].ToString() + "') between D_Start and D_End", Params, connection, transaction);
                        if (DiffFnYearID != null)
                        {
                            MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
                            nFnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());
                        }
                        else
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
                        }
                    }


                    
                    if (nPurchaseID > 0)
                    {
                        try
                        {
                            SortedList DelParam = new SortedList();
                            DelParam.Add("N_CompanyID", nCompanyID);
                            DelParam.Add("X_TransType", xTransType);
                            DelParam.Add("N_VoucherID", nPurchaseID);
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DelParam, connection, transaction);
                            int dltRes = dLayer.DeleteData("Inv_CostCentreTransactions", "N_InventoryID", nPurchaseID, "N_CompanyID = " + nCompanyID + " and N_FnYearID=" + nFnYearID, connection, transaction);
                            xButtonAction="Update"; 
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex.Message));
                        }
                    }



                    if (DocNo == "@Auto")
                    {

                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        //Params.Add("N_BranchID", MasterRow["n_BranchId"].ToString());

                        X_InvoiceNo = dLayer.GetAutoNumber("Inv_Purchase", "x_InvoiceNo", Params, connection, transaction);
                          xButtonAction="Insert"; 
                        if (X_InvoiceNo == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to generate Invoice Number"));
                        }
                        MasterTable.Rows[0]["x_InvoiceNo"] = X_InvoiceNo;
                    }

                     X_InvoiceNo = MasterTable.Rows[0]["X_InvoiceNo"].ToString();
                     
                    nPurchaseID = dLayer.SaveData("Inv_Purchase", "N_PurchaseID", MasterTable, connection, transaction);
                    // dLayer.ExecuteNonQuery("Update Inv_Purchase  Set N_FreeTextReturnID=" + nPurchaseID + "   Where N_PurchaseID=" + nPurchaseMapID + " and N_FnYearID=" + nFnYearID + " and N_CompanyID=" + nCompanyID, connection, transaction);



                    if (nPurchaseID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save Free text purchase!"));
                    }
                    if (DetailTable.Columns.Contains("X_ItemUnit"))
                        DetailTable.Columns.Remove("X_ItemUnit");
                    CostCenterTable = myFunctions.AddNewColumnToDataTable(CostCenterTable, "N_LedgerID", typeof(int), 0);

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {

                        DetailTable.Rows[j]["N_PurchaseID"] = nPurchaseID;
                        int N_InvoiceDetailId = dLayer.SaveDataWithIndex("Inv_PurchaseDetails", "n_PurchaseDetailsID", "", "", j, DetailTable, connection, transaction);

                        if (N_InvoiceDetailId > 0)
                        {
                            for (int k = 0; k < CostCenterTable.Rows.Count; k++)
                            {
                                if (myFunctions.getIntVAL(CostCenterTable.Rows[k]["rowID"].ToString()) == j)
                                {
                                    CostCenterTable.Rows[k]["N_VoucherID"] = nPurchaseID;
                                    CostCenterTable.Rows[k]["N_VoucherDetailsID"] = N_InvoiceDetailId;
                                    CostCenterTable.Rows[k]["N_LedgerID"] = myFunctions.getIntVAL(DetailTable.Rows[j]["n_LedgerID"].ToString());
                                }
                            }
                        }

                    }
                    CostCenterTable.AcceptChanges();
                    DataTable costcenter = new DataTable();
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_CostCenterTransID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_CompanyID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_FnYearID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_InventoryType", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_InventoryID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_InventoryDetailsID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_CostCentreID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_Amount", typeof(double), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_LedgerID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_BranchID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "X_Narration", typeof(string), "");
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "X_Naration", typeof(string), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "D_Entrydate", typeof(DateTime), null);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_GridLineNo", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_EmpID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_ProjectID", typeof(int), 0);
                    costcenter = myFunctions.AddNewColumnToDataTable(costcenter, "N_DivisionID", typeof(int), 0);

                    foreach (DataRow dRow in CostCenterTable.Rows)
                    {
                        DataRow row = costcenter.NewRow();
                        row["N_CostCenterTransID"] = dRow["N_VoucherSegmentID"];
                        row["N_CompanyID"] = dRow["N_CompanyID"];
                        row["N_FnYearID"] = dRow["N_FnYearID"];
                        row["N_InventoryType"] = 2;
                        row["N_InventoryID"] = dRow["N_VoucherID"];
                        row["N_InventoryDetailsID"] = dRow["N_VoucherDetailsID"];
                        row["N_CostCentreID"] = dRow["n_Segment_2"];
                        row["N_Amount"] = -1*myFunctions.getVAL(dRow["N_Amount"].ToString());
                        row["N_LedgerID"] = dRow["N_LedgerID"];
                        row["N_BranchID"] = dRow["N_BranchID"];
                        row["X_Narration"] = "";
                        row["X_Naration"] = dRow["X_Naration"];
                        row["D_Entrydate"] = dRow["D_Entrydate"];
                        row["N_GridLineNo"] = dRow["rowID"];
                        row["N_EmpID"] = myFunctions.getIntVAL(dRow["N_Segment_4"].ToString());
                        row["N_ProjectID"] = myFunctions.getIntVAL(dRow["N_Segment_3"].ToString());
                        row["N_DivisionID"] = myFunctions.getIntVAL(dRow["N_Segment_6"].ToString());
                        costcenter.Rows.Add(row);
                    }
                        //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(nFnYearID,nPurchaseID,X_InvoiceNo,384,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                          
                          
                          SortedList freeTextPurchaseReturnParams = new SortedList();
                           freeTextPurchaseReturnParams.Add("@N_PurchaseID", nPurchaseID);

                     DataTable freeTextPurchaseReturnInfo = dLayer.ExecuteDataTable("Select X_InvoiceNo,X_TransType from Inv_Purchase where N_PurchaseID=@N_PurchaseID", freeTextPurchaseReturnParams, connection, transaction);
                        if (freeTextPurchaseReturnInfo.Rows.Count > 0)
                        {
                            try
                            {
                                myAttachments.SaveAttachment(dLayer, Attachment, X_InvoiceNo, nPurchaseID, freeTextPurchaseReturnInfo.Rows[0]["X_TransType"].ToString().Trim(), freeTextPurchaseReturnInfo.Rows[0]["X_InvoiceNo"].ToString(), nPurchaseID, "Free Text Purchase Return Document", User, connection, transaction);
                            }
                             catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, ex));
                            }
                        }

                    int N_SegmentId = dLayer.SaveData("Inv_CostCentreTransactions", "N_CostCenterTransID", "", "", costcenter, connection, transaction);
                    SortedList PostingParam = new SortedList();
                    PostingParam.Add("N_CompanyID", nCompanyID);
                    PostingParam.Add("X_InventoryMode", xTransType);
                    PostingParam.Add("N_InternalID", nPurchaseID);
                    PostingParam.Add("N_UserID", nUserID);
                    PostingParam.Add("X_SystemName", "WebRequest");
                    try
                    {
                        dLayer.ExecuteNonQueryPro("SP_Acc_InventoryPosting", PostingParam, connection, transaction);
                    }
                    catch (Exception ex)
                    {

                        return Ok(_api.Error(User, ex));
                    }

                      SortedList Result = new SortedList();
                        Result.Add("N_PurchaseID", nPurchaseID);
                        Result.Add("x_InvoiceNo", X_InvoiceNo);

                    transaction.Commit();
                    return Ok(_api.Success(Result,"Free text Purchase  Return Successfully saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPurchaseID, string X_TransType,int nFnYearID)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    DataTable TransData = new DataTable();
                     SortedList ParamList=new SortedList();
                    SqlTransaction transaction = connection.BeginTransaction();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    var nUserID = myFunctions.GetUserID(User);
                      SortedList Params = new SortedList();
                     ParamList.Add("@nTransID", nPurchaseID);
                     ParamList.Add("@nFnYearID",nFnYearID);
                     ParamList.Add("@nCompanyID", nCompanyID);
                     string Sql = "select X_InvoiceNo,n_PurchaseID from Inv_Purchase where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearID and n_PurchaseID=@nTransID";
                     TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection,transaction);
                    string xButtonAction="Delete";
                     String X_InvoiceNo="";
           
                   
           
                        if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                     object n_FnYearID = dLayer.ExecuteScalar("select N_FnyearID from Inv_Purchase where N_PurchaseID =" + nPurchaseID + " and N_CompanyID=" + nCompanyID, Params, connection,transaction);
                    //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(myFunctions.getIntVAL( nFnYearID.ToString()),nPurchaseID,TransRow["X_InvoiceNo"].ToString(),384,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                   
                     object count = dLayer.ExecuteScalar("select count(1) from Inv_Purchase where N_FreeTextReturnID =" + nPurchaseID + " and N_CompanyID=" + nCompanyID, Params, connection,transaction);
                     if (myFunctions.getVAL(count.ToString())>0)
                     {
                         return Ok(_api.Error(User, "Unable to delete Free text Purchase"));
                     }

                    object objPaymentProcessed = dLayer.ExecuteScalar("Select Isnull(N_PayReceiptId,0) from Inv_PayReceiptDetails where N_InventoryId=" + nPurchaseID + " and X_TransType='CREDIT NOTE'", connection, transaction);
                      if (objPaymentProcessed == null)
                        objPaymentProcessed = 0;
                        
                      if (myFunctions.getIntVAL(objPaymentProcessed.ToString()) != 0){
                        return Ok(_api.Error(User, "Payment processed! Unable to delete"));
                    }

                    SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType",X_TransType},
                                {"N_VoucherID",nPurchaseID},
                                {"N_UserID",nUserID},
                                 {"X_SystemName","WebRequest"}};
                 
                    int Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);

                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete Free text Purchase"));
                    }
                    transaction.Commit();
                    return Ok(_api.Success(" Free text Purchase  Return deleted"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
        [HttpGet("listdetails")]
        public ActionResult GetPurchaseeDetails(int nCompanyId, int nFnYearId, string xInvoiceNO, string xTransType, bool showAllBranch, int nBranchId, string xPath)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable Master = new DataTable();
                    DataTable Details = new DataTable();
                    DataTable ReturnDetails = new DataTable();
                    DataTable Acc_CostCentreTrans = new DataTable();
                    int N_PurchaseID = 0;
                    string X_MasterSql = "";
                    string X_DetailsSql = "";
                    X_MasterSql = "Select * from vw_Inv_FreeTextPurchase_Disp  Where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and X_TransType='" + xTransType + "' and X_InvoiceNo='" + xInvoiceNO + "' ";

                    Master = dLayer.ExecuteDataTable(X_MasterSql, Params, connection);
                    N_PurchaseID = myFunctions.getIntVAL(Master.Rows[0]["N_PurchaseID"].ToString());
                    if (Master.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                    Master = myFunctions.AddNewColumnToDataTable(Master, "isReturnDone", typeof(bool), false);


                      string paymentcount = dLayer.ExecuteScalar("select count(1) from Inv_PayReceiptDetails where N_CompanyID=" + nCompanyId + " and N_InventoryId=" + N_PurchaseID+"and x_TransType='CREDIT NOTE'", connection).ToString();
                    
                     if (myFunctions.getVAL(paymentcount.ToString())>0){
                        Master = myFunctions.AddNewColumnToDataTable(Master, "paymentDone", typeof(bool), true);
                     }
                       else{
                        Master = myFunctions.AddNewColumnToDataTable(Master, "paymentDone", typeof(bool), false);
                     }


                    if (xPath != null && xPath != "")
                    {
                        // object returnID = dLayer.ExecuteScalar("Select N_PurchaseID from vw_Inv_FreeTextPurchase_Disp  Where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + "  and N_FreeTextReturnID=" + N_PurchaseID + " ", Params, connection);
                        // if (returnID!=null)
                        // {
                        //     object purchaseAmount = dLayer.ExecuteScalar("Select N_InvoiceAmt from vw_Inv_FreeTextPurchase_Disp  Where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and X_TransType='" + xTransType + "' and X_InvoiceNo='" + xInvoiceNO + "' ", Params, connection);
                        //     object returnAmout = dLayer.ExecuteScalar("Select Sum(N_InvoiceAmt)  from vw_Inv_FreeTextPurchase_Disp  Where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + " and X_TransType='CREDIT NOTE' and N_FreeTextReturnID=" + N_PurchaseID + " ", Params, connection);

                        //     if (myFunctions.getVAL(purchaseAmount.ToString()) == myFunctions.getVAL(returnAmout.ToString()))
                        //     {
                        //         Master.Rows[0]["N_FreeTextReturnID"] = N_PurchaseID;
                        //         Master.Rows[0]["isReturnDone"] = true;
                        //         Master.AcceptChanges();

                        //     }
                        //     else
                        //     {
                        //         Master.Rows[0]["N_FreeTextReturnID"] = N_PurchaseID;
                        //         Master.Rows[0]["N_PurchaseID"] = 0;
                        //         Master.Rows[0]["X_InvoiceNo"] = "@Auto";
                        //         Master.AcceptChanges();
                        //     }
                        //     string X_PurchaseDetails = "Select * From vw_Inv_FreeTextPurchaseToReturnDetails Where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + "  and N_FreeTextReturnID=" + N_PurchaseID + " ";
                        //     ReturnDetails = dLayer.ExecuteDataTable(X_PurchaseDetails, Params, connection);

                        // }
                        // else
                        // {
 string X_PurchaseDetails = "Select * From vw_Inv_FreeTextPurchaseToReturnDetails Where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + "  and N_PurchaseID=" + N_PurchaseID + " ";
                            ReturnDetails = dLayer.ExecuteDataTable(X_PurchaseDetails, Params, connection);
                            Master.Rows[0]["N_FreeTextReturnID"] = N_PurchaseID;
                            Master.Rows[0]["N_PurchaseID"] = 0;
                            Master.Rows[0]["X_InvoiceNo"] = "@Auto";
                            Master.AcceptChanges();

                        // }

                        Master = _api.Format(Master, "Master");

ReturnDetails = _api.Format(ReturnDetails, "Details");
                    dt.Tables.Add(ReturnDetails);
                    dt.Tables.Add(Master);
                    return Ok(_api.Success(dt));


                    }



                    Master = _api.Format(Master, "Master");


                    X_DetailsSql = "Select * From vw_Inv_Purchasedetails Where N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId + "   and N_PurchaseID= " + N_PurchaseID + "";



                    Details = dLayer.ExecuteDataTable(X_DetailsSql, Params, connection);
                    Details = myFunctions.AddNewColumnToDataTable(Details, "flag", typeof(int), 0);
                    if (xPath != null && xPath != "")
                    {
                        if (ReturnDetails.Rows.Count > 0)
                        {
                            foreach (DataRow var1 in Details.Rows)
                            {
                                foreach (DataRow var2 in ReturnDetails.Rows)
                                {
                                    if (myFunctions.getIntVAL(var1["N_LedgerID"].ToString()) == myFunctions.getIntVAL(var2["N_LedgerID"].ToString()))
                                    {
                                        if (myFunctions.getVAL(var1["N_PpriceF"].ToString()) > myFunctions.getVAL(var2["N_PpriceF"].ToString()))
                                        {
                                            var1["N_PpriceF"] = (myFunctions.getVAL(var1["N_PpriceF"].ToString()) - myFunctions.getVAL(var2["N_PpriceF"].ToString())).ToString();
                                            var1["N_Pprice"] = (myFunctions.getVAL(var1["N_Pprice"].ToString()) - myFunctions.getVAL(var2["N_Pprice"].ToString())).ToString();
                                        }
                                        else if (myFunctions.getVAL(var1["N_PpriceF"].ToString()) == myFunctions.getVAL(var2["N_PpriceF"].ToString()))
                                        {

                                            var1["flag"] = 1;


                                        }

                                    }


                                }
                            }
                        }
                    }
                    foreach (DataRow var3 in Details.Rows)
                    {
                        if(myFunctions.getIntVAL(var3["flag"].ToString())==1)
                        {
                         var3.Delete();

                        }
                    }
                    Details.AcceptChanges();

                    Details = _api.Format(Details, "Details");
                    SortedList ProParams = new SortedList();
                    ProParams.Add("N_CompanyID", nCompanyId);
                    ProParams.Add("N_FnYearID", nFnYearId);
                    ProParams.Add("N_VoucherID", N_PurchaseID);
                    ProParams.Add("N_Flag", 1);
                    ProParams.Add("X_Type", "PURCHASE");


                    // Acc_CostCentreTrans = dLayer.ExecuteDataTablePro("SP_Acc_Voucher_Disp_CLOUD", ProParams, connection);

                    string CostcenterSql = "SELECT X_EmpCode, X_EmpName, N_ProjectID as N_Segment_3,N_EmpID as N_Segment_4,N_DivisionID,X_DivisionName, X_ProjectCode,X_ProjectName,N_EmpID,N_ProjectID,N_CompanyID,N_FnYearID, " +
                    " N_VoucherID, N_VoucherDetailsID, N_CostCentreID,X_CostCentreName,X_CostCentreCode,N_BranchID,X_BranchName,X_BranchCode , " +
                    " N_Amount, N_LedgerID, N_CostCenterTransID, N_GridLineNo,X_Naration,0 AS N_AssetID, '' As X_AssetCode, " +
                    " GETDATE() AS D_RepaymentDate, '' AS X_AssetName,'' AS X_PayCode,0 AS N_PayID,0 AS N_Inst,CAST(0 AS BIT) AS B_IsCategory,D_Entrydate " +
                    " FROM   vw_InvFreeTextPurchaseCostCentreDetails where N_InventoryID = " + N_PurchaseID + " And N_InventoryType=2 And N_FnYearID=" + nFnYearId +
                    " And N_CompanyID=" + nCompanyId + " Order By N_InventoryID,N_VoucherDetailsID ";

                    Acc_CostCentreTrans = dLayer.ExecuteDataTable(CostcenterSql, ProParams, connection);

                    Acc_CostCentreTrans = _api.Format(Acc_CostCentreTrans, "costCenterTrans");
                    dt.Tables.Add(Acc_CostCentreTrans);

                    DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(Master.Rows[0]["N_PurchaseID"].ToString()), myFunctions.getIntVAL(Master.Rows[0]["N_PurchaseID"].ToString()), this.FormID, myFunctions.getIntVAL(Master.Rows[0]["N_FnYearID"].ToString()), User, connection);
                    Attachments = _api.Format(Attachments, "attachments");
                    dt.Tables.Add(Attachments);

                    dt.Tables.Add(Details);
                    dt.Tables.Add(Master);
                    return Ok(_api.Success(dt));

                }
            }



            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
    }
}



































