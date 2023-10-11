using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("deliverynote")]
    [ApiController]
    public class Inv_DeliveryNote : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int FormID;
        public Inv_DeliveryNote(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 884;
        }

        [HttpGet("list")]
        public ActionResult GetDeliveryNoteList(int? nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, bool bAllBranchData, int nBranchID, int nFormID)
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
                    int nUserID = myFunctions.GetUserID(User);
                    string UserPattern = myFunctions.GetUserPattern(User);
                    string Pattern = "";

                    if (UserPattern != "")
                    {
                        Pattern = " and Left(X_Pattern,Len(@UserPattern))=@UserPattern ";
                        Params.Add("@UserPattern", UserPattern);

                    }
                    // else
                    // {
                    //     object HierarchyCount = dLayer.ExecuteScalar("select count(N_HierarchyID) from Sec_UserHierarchy where N_CompanyID="+nCompanyId,Params,connection);

                    //     if(myFunctions.getIntVAL(HierarchyCount.ToString())>0)
                    //     Pattern = " and N_CreatedUser=" + nUserID;
                    // }

                    //     if(myFunctions.getIntVAL(HierarchyCount.ToString())>0)
                    //     Pattern = " and N_UserID=" + nUserID;
                    // }

                    //     if(myFunctions.getIntVAL(HierarchyCount.ToString())>0)
                    //     Pattern = " and N_CreatedUser=" + nUserID;
                    // }



                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and ([Invoice No] like '%" + xSearchkey + "%' or Customer like '%" + xSearchkey + "%' or x_Notes like '%" + xSearchkey + "%' or x_CustPONo like '%" + xSearchkey + "%' or X_OrderNo like '%" + xSearchkey + "%' or [Invoice Date] like '%" + xSearchkey + "%' or D_DeliveryDate like '%" + xSearchkey + "%' or X_ActionStatus like '%" + xSearchkey + "%' )";

                    if (bAllBranchData == true)
                    {
                        Searchkey = Searchkey + " ";
                    }
                    else
                    {
                        Searchkey = Searchkey + " and N_BranchID=" + nBranchID + " ";
                    }

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_DeliveryNoteID desc";
                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "invoiceNo":
                                xSortBy = "[Invoice No] " + xSortBy.Split(" ")[1];
                                break;
                            case "invoiceDate":
                                xSortBy = "Cast([Invoice Date] as DateTime )" + xSortBy.Split(" ")[1];
                                break;
                            case "d_DeliveryDate":
                                xSortBy = "Cast(D_DeliveryDate as DateTime )" + xSortBy.Split(" ")[1];
                                break;
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }
                    if (nFormID == 1572)
                    {
                        if (Count == 0)
                            sqlCommandText = "select top(" + nSizeperpage + ") [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=1572 " + Pattern + Searchkey + " " + " group by [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar" + xSortBy;
                        else
                            sqlCommandText = "select top(" + nSizeperpage + ") [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=1572 " + Pattern + Searchkey + " and N_DeliveryNoteID not in (select top(" + Count + ") N_DeliveryNoteID from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=1572 " + xSortBy + " ) " + "Group By [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar" + xSortBy;
                    }
                    else if (nFormID == 1603)
                    {
                        if (Count == 0)
                            sqlCommandText = "select top(" + nSizeperpage + ") [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=1603 " + Pattern + Searchkey + " " + " group by [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar" + xSortBy;
                        else
                            sqlCommandText = "select top(" + nSizeperpage + ") [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=1603 " + Pattern + Searchkey + " and N_DeliveryNoteID not in (select top(" + Count + ") N_DeliveryNoteID from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=1603 " + xSortBy + " ) " + "Group By [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar" + xSortBy;
                    }
                    else if (nFormID == 884)
                    {
                        if (Count == 0)
                            sqlCommandText = "select top(" + nSizeperpage + ") [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_ActionName,X_ActionStatus,X_StatusColour,X_ClosingRemarks,X_CustomerName_Ar  from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=884 " + Pattern + Searchkey + " " + " group by [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_ActionName,X_ActionStatus,X_StatusColour,X_ClosingRemarks,X_CustomerName_Ar" + xSortBy;
                        else
                            sqlCommandText = "select top(" + nSizeperpage + ") [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID ,X_ActionName,X_ActionStatus,X_StatusColour,X_ClosingRemarks,X_CustomerName_Ar  from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=884 " + Pattern + Searchkey + " and N_DeliveryNoteID not in (select top(" + Count + ") N_DeliveryNoteID from Inv_DeliveryNote where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=884 " + xSortBy + " ) " + "Group By [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_ActionName,X_ActionStatus,X_StatusColour,X_ClosingRemarks,X_CustomerName_Ar" + xSortBy;
                    }
                    else if (nFormID == 1426)
                    {
                        if (Count == 0)
                            sqlCommandText = "select top(" + nSizeperpage + ") [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=1426 " + Pattern + Searchkey + " " + " group by [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar" + xSortBy;
                        else
                            sqlCommandText = "select top(" + nSizeperpage + ") [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=1426 " + Pattern + Searchkey + " and N_DeliveryNoteID not in (select top(" + Count + ") N_DeliveryNoteID from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=1426 " + xSortBy + " ) " + "Group By [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar" + xSortBy;
                    }
                    else
                    {
                        if (Count == 0)
                            sqlCommandText = "select top(" + nSizeperpage + ") [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID!=1572 " + Pattern + Searchkey + " " + " group by [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar" + xSortBy;
                        else
                            sqlCommandText = "select top(" + nSizeperpage + ") [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID!=1572 " + Pattern + Searchkey + " and N_DeliveryNoteID not in (select top(" + Count + ") N_DeliveryNoteID from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID!=1572 " + xSortBy + " ) " + "Group By [invoice No],[Invoice Date],customer,d_DeliveryDate,x_CustPONo,x_Notes,x_OrderNo,b_IsSaveDraft,N_DeliveryNoteID,X_CustomerName_Ar" + xSortBy;
                    };

                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearId);
                    SortedList OutPut = new SortedList();



                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    if (nFormID == 1572)
                        sqlCommandCount = "select count(1) as N_Count  from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=1572 " + Searchkey + "";
                    else if (nFormID == 1603)
                    {
                        sqlCommandCount = "select count(1) as N_Count  from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=1603 " + Searchkey + "";
                    }
                    else
                        sqlCommandCount = "select count(1) as N_Count  from vw_InvDeliveryNoteNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID!=1572 " + Searchkey + "";
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
        [HttpGet("details")]
        public ActionResult GetDeliveryNoteDetails(int nFnYearId, bool bAllBranchData, int nBranchId, string xInvoiceNo, int nSalesOrderID, int nProformaID, int nPickListID, string xSalesOrderID, int nTransferId)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection Con = new SqlConnection(connectionString))
                {
                    Con.Open();


                    DataSet dsSalesInvoice = new DataSet();
                    SortedList QueryParamsList = new SortedList();
                    QueryParamsList.Add("@nCompanyID", nCompanyId);
                    QueryParamsList.Add("@nFnYearID", nFnYearId);
                    QueryParamsList.Add("@nBranchId", nBranchId);
                    QueryParamsList.Add("@xTransType", "DELIVERY");

                    SortedList mParamsList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"X_ReceiptNo",xInvoiceNo},
                        {"X_TransType","DELIVERY"},
                        {"N_FnYearID",nFnYearId},

                    };
                    if (!bAllBranchData)
                    {
                        mParamsList.Add("N_BranchId", nBranchId);
                    }
                    else
                    {
                        mParamsList.Add("N_BranchId", 0);
                    }
                    if (nSalesOrderID > 0 || (xSalesOrderID != "" && xSalesOrderID != null))
                    {
                        string Mastersql = "";
                        DataTable MasterTable = new DataTable();
                        DataTable DetailTable = new DataTable();
                        DataTable RentalScheduleData = new DataTable();
                        string DetailSql = "";
                        string RentalSql = "";
                        if (nSalesOrderID > 0)
                        {
                            QueryParamsList.Add("@nSalesorderID", nSalesOrderID);
                            Mastersql = "select * from vw_SalesOrdertoDeliveryNote where N_CompanyId=@nCompanyID and N_SalesOrderId=@nSalesorderID";
                            MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                            if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                            MasterTable = _api.Format(MasterTable, "Master");
                            DetailSql = "select * from vw_SalesOrdertoDeliveryNoteDetails where N_CompanyId=@nCompanyID and N_SalesOrderId=@nSalesorderID";
                            DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);

                            RentalSql = "SELECT * FROM  vw_RentalScheduleItems  Where N_CompanyID=@nCompanyID and N_TransID=@nSalesorderID and N_FormID=1571";
                            RentalScheduleData = dLayer.ExecuteDataTable(RentalSql, QueryParamsList, Con);
                            RentalScheduleData = _api.Format(RentalScheduleData, "RentalSchedule");


                            SortedList DelParams = new SortedList();
                            DelParams.Add("N_CompanyID", nCompanyId);
                            DelParams.Add("N_SalesOrderID", nSalesOrderID);
                            DelParams.Add("FnYearID", nFnYearId);
                            DelParams.Add("@N_Type", 0);
                            DataTable OrderToDel = dLayer.ExecuteDataTablePro("SP_InvSalesOrderDtlsInDelNot_Disp", DelParams, Con);
                            DetailTable = myFunctions.AddNewColumnToDataTable(DetailTable, "N_SOQty", typeof(string), "");
                            foreach (DataRow Avar in OrderToDel.Rows)
                            {
                                foreach (DataRow Kvar in DetailTable.Rows)
                                {
                                    if (myFunctions.getIntVAL(Avar["N_SalesOrderDetailsID"].ToString()) == myFunctions.getIntVAL(Kvar["N_SalesOrderDetailsID"].ToString()))
                                    {
                                        if (myFunctions.getVAL(Avar["N_QtyDisplay"].ToString()) <= 0)
                                            Kvar["N_QtyDisplay"] = 0;
                                        else
                                            Kvar["N_QtyDisplay"] = Avar["N_QtyDisplay"];
                                        Kvar["N_SOQty"] = Avar["N_QtyDisplay"];
                                        if (myFunctions.getVAL(Avar["N_QtyDisplay"].ToString()) <= 0)
                                            Kvar["N_QtyDisplay"] = 0;
                                        else
                                            Kvar["N_Qty"] = Avar["N_Qty"];

                                    }
                                }
                            }
                            DetailTable.AcceptChanges();
                            foreach (DataRow Kvar in DetailTable.Rows)
                            {
                                if (myFunctions.getVAL(Kvar["N_QtyDisplay"].ToString()) == 0)
                                {
                                    Kvar.Delete();
                                    continue;
                                }
                            }
                            DetailTable.AcceptChanges();
                            RentalScheduleData = myFunctions.AddNewColumnToDataTable(RentalScheduleData, "N_Flag", typeof(int), 0);

                            foreach (DataRow Avar in DetailTable.Rows)
                            {
                                foreach (DataRow Rentvar in RentalScheduleData.Rows)
                                {
                                    if (myFunctions.getIntVAL(Avar["N_SalesOrderDetailsID"].ToString()) == myFunctions.getIntVAL(Rentvar["N_TransDetailsID"].ToString()) && myFunctions.getIntVAL(Avar["N_ItemID"].ToString()) == myFunctions.getIntVAL(Rentvar["N_ItemID"].ToString()))
                                    {
                                        Rentvar["N_Flag"] = 1;
                                    }
                                }
                            }
                            RentalScheduleData.AcceptChanges();

                            foreach (DataRow Rentvar in RentalScheduleData.Rows)
                            {
                                if (myFunctions.getVAL(Rentvar["N_Flag"].ToString()) != 1)
                                {
                                    Rentvar.Delete();
                                    continue;
                                }
                            }

                            if (myFunctions.ContainColumn("N_Flag", RentalScheduleData))
                                RentalScheduleData.Columns.Remove("N_Flag");

                            RentalScheduleData.AcceptChanges();
                        }
                        else
                        {
                            string[] X_SalesOrderID = xSalesOrderID.Split(",");
                            int N_SOID = myFunctions.getIntVAL(X_SalesOrderID[0].ToString());
                            Mastersql = "select * from vw_SalesOrdertoDeliveryNote where N_CompanyId=@nCompanyID and N_SalesOrderId =" + N_SOID + "";
                            MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                            if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                            MasterTable = _api.Format(MasterTable, "Master");
                            DetailSql = "select * from vw_SalesOrdertoDeliveryNoteDetails where N_CompanyId=@nCompanyID and N_SalesOrderId in(" + xSalesOrderID + ")";
                            DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);


                            SortedList MultiParams = new SortedList();
                            MultiParams.Add("N_CompanyID", nCompanyId);
                            MultiParams.Add("N_SalesOrderID", N_SOID);
                            MultiParams.Add("X_SalesOrderID", xSalesOrderID);
                            MultiParams.Add("FnYearID", nFnYearId);
                            MultiParams.Add("@N_Type", 0);
                            DataTable OrderToDel = dLayer.ExecuteDataTablePro("SP_InvSalesOrderDtlsInMultiDelNot_Disp", MultiParams, Con);
                            DetailTable = myFunctions.AddNewColumnToDataTable(DetailTable, "N_SOQty", typeof(string), "");
                            foreach (DataRow Avar in OrderToDel.Rows)
                            {
                                foreach (DataRow Kvar in DetailTable.Rows)
                                {
                                    if (myFunctions.getIntVAL(Avar["N_SalesOrderDetailsID"].ToString()) == myFunctions.getIntVAL(Kvar["N_SalesOrderDetailsID"].ToString()))
                                    {
                                        if (myFunctions.getVAL(Avar["N_QtyDisplay"].ToString()) <= 0)
                                            Kvar["N_QtyDisplay"] = 0;
                                        else
                                        {
                                            Kvar["N_QtyDisplay"] = Avar["N_QtyDisplay"];
                                            Kvar["N_SOQty"] = Avar["N_QtyDisplay"];
                                        }
                                        if (myFunctions.getVAL(Avar["N_Qty"].ToString()) <= 0)
                                            Kvar["N_Qty"] = 0;
                                        else
                                            Kvar["N_Qty"] = Avar["N_Qty"];

                                    }
                                }
                            }
                            DetailTable.AcceptChanges();
                            foreach (DataRow Kvar in DetailTable.Rows)
                            {
                                if (myFunctions.getVAL(Kvar["N_QtyDisplay"].ToString()) == 0)
                                {
                                    Kvar.Delete();
                                    continue;
                                }
                            }
                            DetailTable.AcceptChanges();


                        }
                        DetailTable = _api.Format(DetailTable, "Details");
                        dsSalesInvoice.Tables.Add(MasterTable);
                        dsSalesInvoice.Tables.Add(DetailTable);
                        dsSalesInvoice.Tables.Add(RentalScheduleData);
                        return Ok(_api.Success(dsSalesInvoice));
                    }
                    else if (nProformaID > 0)
                    {
                        QueryParamsList.Add("@nProformaID", nProformaID);
                        string Mastersql = "select * from vw_ProformaToDeliveryNote where N_CompanyId=@nCompanyID and N_ProformaID=@nProformaID";
                        DataTable MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = _api.Format(MasterTable, "Master");
                        string DetailSql = "";
                        DetailSql = "select * from vw_ProformatoDeliveryNoteDetails where N_CompanyId=@nCompanyID and N_ProformaID=@nProformaID";
                        DataTable DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);

                        DetailTable = _api.Format(DetailTable, "Details");
                        dsSalesInvoice.Tables.Add(MasterTable);
                        dsSalesInvoice.Tables.Add(DetailTable);
                        return Ok(_api.Success(dsSalesInvoice));
                    }
                    else if (nPickListID > 0)
                    {
                        QueryParamsList.Add("@nPickListID", nPickListID);


                        string Mastersql = "select N_CompanyID,N_FnYearID,0 as N_DeliveryNoteId,'@Auto' as X_ReceiptNo,GETDATE() as D_DeliveryDate,GETDATE() as D_EntryDate,N_CustomerId,X_CustomerName,0 as B_BiginingBalEntry,0 as N_DeliveryType,N_LocationID,'DELIVERY' as X_TransType,0 as B_IsSaveDraft,X_LocationName from vw_WhPickListMaster where N_CompanyId=@nCompanyID and N_PickListID=@nPickListID";
                        DataTable MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = _api.Format(MasterTable, "Master");
                        string DetailSql = "";
                        DetailSql = "select N_CompanyID,0 as N_DeliveryNoteID,0 as N_DeliveryNoteDetailsID,0 as N_SalesQuotationID,N_ItemID,X_ItemName,X_ItemCode,X_BatchCode,D_ExpiryDate,N_ItemUnitID,X_ItemUnit,N_Qty,N_Qty, N_QtyDisplay,0 as N_Sprice,0 as N_IteDiscAmt,2 as N_ClassID,N_Qty as n_QtyDisplay,0 as N_Cost,N_LocationID,X_CustomerSKU,X_Temperature,X_Dimesnsion from vw_WhPickListDetails where N_CompanyId=@nCompanyID and N_PickListID=@nPickListID";
                        DataTable DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dsSalesInvoice.Tables.Add(MasterTable);
                        dsSalesInvoice.Tables.Add(DetailTable);

                        return Ok(_api.Success(dsSalesInvoice));
                    }
                    else if (nTransferId > 0)
                    {
                        QueryParamsList.Add("@nTransferId", nTransferId);
                        string Mastersql = "select N_CompanyID,N_FnYearID,0 as N_DeliveryNoteId,'@Auto' as X_ReceiptNo,GETDATE() as D_DeliveryDate,GETDATE() as D_EntryDate,N_CustomerId,X_CustomerName,0 as B_BiginingBalEntry,0 as N_DeliveryType,'DELIVERY' as X_TransType,0 as B_IsSaveDraft from vw_ProductTransferToDeliveryNoteMaster where N_CompanyId=@nCompanyID and N_TransferId=@nTransferId";
                        DataTable MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = _api.Format(MasterTable, "Master");
                        string DetailSql = "";
                        DetailSql = "select N_CompanyID,0 as N_DeliveryNoteID,0 as N_DeliveryNoteDetailsID,0 as N_SalesQuotationID,N_ItemID,description AS X_ItemName,X_BatchCode,D_ExpiryDate,N_ItemUnitID,X_ItemUnit,0 as N_Sprice,0 as N_IteDiscAmt,2 as N_ClassID,N_Qty,N_AvlQty AS N_QtyDisplay,0 as N_Cost,N_LocationIDTo AS N_LocationID ,X_Temperature,X_CustomerSKU,X_Dimesnsion,N_Stock,X_LocationName from vw_ProductTransferToDeliveryNote where N_CompanyId=@nCompanyID and N_TransferId=@nTransferId";
                        DataTable DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dsSalesInvoice.Tables.Add(MasterTable);
                        dsSalesInvoice.Tables.Add(DetailTable);
                        return Ok(_api.Success(dsSalesInvoice));
                    }
                    else
                    {
                        QueryParamsList.Add("@xInvoiceNo", xInvoiceNo);
                    }
                    DataTable masterTable = dLayer.ExecuteDataTablePro("SP_InvDeliveryNote_Disp", mParamsList, Con);
                    DataRow MasterRow = masterTable.Rows[0];

                    masterTable = _api.Format(masterTable, "Master");
                    if (masterTable.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                    SortedList DetailParams = new SortedList();
                    DetailParams.Add("@nCompanyID", MasterRow["N_companyId"]);
                    DetailParams.Add("@nDelID", MasterRow["N_deliverynoteid"]);


                    var nFormID = this.FormID;
                    int N_DelID = myFunctions.getIntVAL(MasterRow["N_deliverynoteid"].ToString());
                    int N_SalesOrderID = myFunctions.getIntVAL(MasterRow["n_SalesOrderID"].ToString());
                    QueryParamsList.Add("@nDelID", N_DelID);
                    QueryParamsList.Add("@nSaleOrderID", N_SalesOrderID);
                    object InSales = "";
                    string code = Convert.ToString(dLayer.ExecuteScalar("select x_ShippingCode from Inv_Shipping where N_CompanyID=@nCompanyID and N_deliverynoteid=@nDelID", DetailParams, Con));
                    masterTable = myFunctions.AddNewColumnToDataTable(masterTable, "x_ShippingCode", typeof(string), code);
                    object inSaleID = dLayer.ExecuteScalar("select N_SalesID from Inv_SalesDetails where N_CompanyID=@nCompanyID and N_deliverynoteid=@nDelID ", QueryParamsList, Con);
                    if (inSaleID != null && inSaleID != "")
                        InSales = dLayer.ExecuteScalar("select x_ReceiptNo from Inv_Sales where N_CompanyID=@nCompanyID and  N_SalesID=" + myFunctions.getIntVAL(inSaleID.ToString()) + " and N_FnYearID=@nFnYearID", QueryParamsList, Con);
                    masterTable = myFunctions.AddNewColumnToDataTable(masterTable, "x_SalesReceiptNo", typeof(string), InSales);

                    object InSalesOrder = dLayer.ExecuteScalar("select x_OrderNo from Inv_SalesOrder where N_CompanyID=@nCompanyID and N_SalesOrderID=@nSaleOrderID and N_FnYearID=@nFnYearID", QueryParamsList, Con);
                    masterTable = myFunctions.AddNewColumnToDataTable(masterTable, "x_OrderNo", typeof(string), InSalesOrder);


                    QueryParamsList.Add("@nSalesID", myFunctions.getIntVAL(MasterRow["N_TruckID"].ToString()));




                    masterTable = myFunctions.AddNewColumnToDataTable(masterTable, "N_SalesId", typeof(int), 0);
                    masterTable = myFunctions.AddNewColumnToDataTable(masterTable, "isSalesDone", typeof(bool), false);
                    masterTable = myFunctions.AddNewColumnToDataTable(masterTable, "isProformaDone", typeof(bool), false);
                    masterTable = myFunctions.AddNewColumnToDataTable(masterTable, "isDeliveryReturnDone", typeof(bool), false);

                    if (myFunctions.getIntVAL(masterTable.Rows[0]["N_DeliveryNoteId"].ToString()) > 0)
                    {
                        QueryParamsList.Add("@nDeliveryNoteId", myFunctions.getIntVAL(masterTable.Rows[0]["N_DeliveryNoteId"].ToString()));
                        if (inSaleID != null && inSaleID != "")
                        {
                            DataTable SalesData = dLayer.ExecuteDataTable("select X_ReceiptNo,N_SalesId from Inv_Sales where  N_CompanyId=@nCompanyID and N_SalesID=" + myFunctions.getIntVAL(inSaleID.ToString()) + " and  N_FnYearID=@nFnYearID", QueryParamsList, Con);
                            if (SalesData.Rows.Count > 0)
                            {
                                masterTable.Rows[0]["X_SalesReceiptNo"] = SalesData.Rows[0]["X_ReceiptNo"].ToString();
                                masterTable.Rows[0]["N_SalesId"] = myFunctions.getIntVAL(SalesData.Rows[0]["N_SalesId"].ToString());
                                masterTable.Rows[0]["isSalesDone"] = true;
                            }
                        }
                        else if (myFunctions.getIntVAL(masterTable.Rows[0]["n_SalesOrderID"].ToString()) > 0)
                        {
                            // QueryParamsList.Add("@nSOID", myFunctions.getIntVAL(masterTable.Rows[0]["n_SalesOrderID"].ToString()));
                            // DataTable NewSalesData = dLayer.ExecuteDataTable("select X_ReceiptNo,N_SalesId from Inv_Sales where N_SalesOrderID=@nSOID and N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID", QueryParamsList, Con);
                            // if (NewSalesData.Rows.Count > 0)
                            // {
                            //     masterTable.Rows[0]["X_SalesReceiptNo"] = NewSalesData.Rows[0]["X_ReceiptNo"].ToString();
                            //     masterTable.Rows[0]["N_SalesId"] = myFunctions.getIntVAL(NewSalesData.Rows[0]["N_SalesId"].ToString());
                            //     masterTable.Rows[0]["isProformaDone"] = true;
                            // }
                        }
                        DataTable returnData = dLayer.ExecuteDataTable("select N_DeliveryNoteId from vw_DeliveryNoteToDeliveryReturn where N_DeliveryNoteId=@nDeliveryNoteId and N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID and N_BalanceQty=0", QueryParamsList, Con);
                        if (returnData.Rows.Count > 0)
                        {

                            masterTable.Rows[0]["isDeliveryReturnDone"] = false;
                        }
                        else
                        {
                            masterTable.Rows[0]["isDeliveryReturnDone"] = true;
                        }
                    }

                    //Details
                    SortedList dParamList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"N_SalesID",masterTable.Rows[0]["n_DeliveryNoteId"].ToString()}
                    };
                    DataTable detailTable = dLayer.ExecuteDataTablePro("SP_InvDeliveryNoteDtls_Disp", dParamList, Con);
                    detailTable = _api.Format(detailTable, "Details");
                    DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(masterTable.Rows[0]["N_CustomerID"].ToString()), myFunctions.getIntVAL(masterTable.Rows[0]["n_DeliveryNoteId"].ToString()), this.FormID, myFunctions.getIntVAL(masterTable.Rows[0]["N_FnYearID"].ToString()), User, Con);
                    Attachments = _api.Format(Attachments, "attachments");
                    string RentalScheduleSql = "SELECT * FROM  vw_RentalScheduleItems  Where N_CompanyID=N_CompanyID and N_TransID=" + masterTable.Rows[0]["N_DeliveryNoteId"].ToString();
                    DataTable RentalSchedule = dLayer.ExecuteDataTable(RentalScheduleSql, QueryParamsList, Con);
                    RentalSchedule = _api.Format(RentalSchedule, "RentalSchedule");
                    if (detailTable.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                    dsSalesInvoice.Tables.Add(masterTable);
                    dsSalesInvoice.Tables.Add(detailTable);
                    dsSalesInvoice.Tables.Add(Attachments);
                    dsSalesInvoice.Tables.Add(RentalSchedule);
                    return Ok(_api.Success(dsSalesInvoice));

                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        //Save....
        [HttpPost("Save")]
        public ActionResult SaveData( [FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;

                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                DataTable Attachment = ds.Tables["attachments"];
                DataTable rentalItem = ds.Tables["segmentTable"];
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();
                // Auto Gen 
                string InvoiceNo = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable TransData = new DataTable();
                    DataRow MasterRow = MasterTable.Rows[0];


                    int N_DeliveryNoteID = myFunctions.getIntVAL(MasterRow["n_DeliveryNoteId"].ToString());
                    int N_DNoteID = myFunctions.getIntVAL(MasterRow["n_DeliveryNoteId"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int N_BranchID = myFunctions.getIntVAL(MasterRow["n_BranchID"].ToString());
                    int N_LocationID = myFunctions.getIntVAL(MasterRow["n_LocationID"].ToString());
                    int N_CustomerID = myFunctions.getIntVAL(MasterRow["n_CustomerID"].ToString());
                    int N_UserID = myFunctions.getIntVAL(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    int UserCategoryID = myFunctions.getIntVAL(User.FindFirst(ClaimTypes.GroupSid)?.Value);
                    //int N_AmtSplit = 0;
                    int N_SaveDraft = myFunctions.getIntVAL(MasterRow["b_IsSaveDraft"].ToString());
                    bool B_AllBranchData = false, B_AllowCashPay = false;
                    // bool B_SalesOrder = myFunctions.CheckPermission(N_CompanyID, 81, "Administrator", "X_UserCategory", dLayer, connection, transaction);
                    object SalesOrderCount = dLayer.ExecuteScalar("select count(1) from vw_userPrevileges where N_CompanyID=" + N_CompanyID + " and N_MenuID=81", QueryParams, connection, transaction);
                    bool B_SalesOrder = false;
                    if (myFunctions.getIntVAL(SalesOrderCount.ToString()) > 0) B_SalesOrder = true;

                    bool B_SRS = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("729", "SRSinDeliveryNote", "N_Value", N_CompanyID, dLayer, connection, transaction)));
                    string i_Signature = "";
                    string i_signature2 = "";
                    bool SigEnable = false;
                    String xButtonAction = "";

                    if (!myFunctions.CheckActiveYearTransaction(N_CompanyID, N_FnYearID, DateTime.ParseExact(MasterTable.Rows[0]["D_DeliveryDate"].ToString(), "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture), dLayer, connection, transaction))
                    {
                        object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID=" + N_CompanyID + " and convert(date ,'" + MasterTable.Rows[0]["D_DeliveryDate"].ToString() + "') between D_Start and D_End", connection, transaction);
                        if (DiffFnYearID != null)
                        {
                            MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
                            int nFnYearID = myFunctions.getIntVAL(DiffFnYearID.ToString());


                            QueryParams["@N_FnYearID"] = nFnYearID;
                            QueryParams["@N_CustomerID"] = N_CustomerID;
                            QueryParams["@N_CompanyID"] = N_CompanyID;


                            SortedList PostingParam = new SortedList();
                            PostingParam.Add("N_PartyID", N_CustomerID);
                            PostingParam.Add("N_FnyearID", N_FnYearID);
                            PostingParam.Add("N_CompanyID", N_CompanyID);
                            PostingParam.Add("X_Type", "customer");


                            object custCount = dLayer.ExecuteScalar("Select count(*) From Inv_Customer where N_FnYearID=@N_FnYearID and N_CompanyID=@N_CompanyID and N_CustomerID=@N_CustomerID", QueryParams, connection, transaction);

                            if (myFunctions.getIntVAL(custCount.ToString()) == 0)
                            {
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
                            return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
                        }
                    }
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nDeliveryNoteID", N_DNoteID);
                    ParamList.Add("@nCompanyID",N_CompanyID);
                    string DeliveryNoteID = "select * from Inv_Shipping where N_CompanyID=@nCompanyID and N_deliverynoteid=@nDeliveryNoteID ";
                    TransData = dLayer.ExecuteDataTable(DeliveryNoteID, ParamList, connection, transaction);

                    if (TransData.Rows.Count > 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Please delete Shipping Invoice for Updating this Delivery Note"));
                    }


                    QueryParams.Add("@nCompanyID", N_CompanyID);
                    QueryParams.Add("@nFnYearID", N_FnYearID);
                    QueryParams.Add("@nSalesID", N_DeliveryNoteID);
                    QueryParams.Add("@nBranchID", N_BranchID);
                    QueryParams.Add("@nLocationID", N_LocationID);
                    QueryParams.Add("@nCustomerID", N_CustomerID);
                    int N_FormID = 0;
                    if (MasterTable.Columns.Contains("N_FormID"))
                    {
                        N_FormID = myFunctions.getIntVAL(MasterRow["N_FormID"].ToString());
                    }



                    //B_DirectPosting = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select B_DirPosting from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID", QueryParams, connection, transaction).ToString());
                    object objAllBranchData = dLayer.ExecuteScalar("Select B_ShowAllData From Acc_BranchMaster where N_BranchID=@nBranchID and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                    if (objAllBranchData != null)
                        B_AllBranchData = myFunctions.getBoolVAL(objAllBranchData.ToString());

                    if (B_AllBranchData)
                        B_AllowCashPay = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select cast(count(N_CustomerID) as bit) from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID  and N_AllowCashPay=1", QueryParams, connection, transaction).ToString());
                    else
                        B_AllowCashPay = myFunctions.getBoolVAL(dLayer.ExecuteScalar("select cast(count(N_CustomerID) as bit) from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID  and N_AllowCashPay=1 and (N_BranchId=@nBranchID or N_BranchId=0)", QueryParams, connection, transaction).ToString());


                    //saving data
                    var values = MasterRow["x_ReceiptNo"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterRow["n_CompanyId"].ToString());
                        Params.Add("N_YearID", MasterRow["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 729);
                        Params.Add("N_BranchID", MasterRow["n_BranchId"].ToString());
                        InvoiceNo = dLayer.GetAutoNumber("Inv_DeliveryNote", "x_ReceiptNo", Params, connection, transaction);
                        xButtonAction = "Insert";

                        if (InvoiceNo == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Delivery Number")); }
                        MasterTable.Rows[0]["x_ReceiptNo"] = InvoiceNo;
                    }


                    else
                    {
                        InvoiceNo = MasterTable.Rows[0]["x_ReceiptNo"].ToString();
                        if (N_DeliveryNoteID > 0)
                        {
                            SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",N_CompanyID},
                                {"X_TransType","DELIVERY"},
                                {"N_VoucherID",N_DeliveryNoteID}};
                            try
                            {
                                dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                                xButtonAction = "Update";
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, ex));
                            }
                        }
                    }





                    DataTable count = new DataTable();
                    SortedList Paramss = new SortedList();
                    string sql = "select * from Inv_DeliveryNote where x_ReceiptNo='" + values + "' and N_CompanyID=" + N_CompanyID + "";
                    count = dLayer.ExecuteDataTable(sql, Paramss, connection, transaction);
                    if (count.Rows.Count > 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Reciept Number Already in Use"));
                    }




                    Byte[] ImageBitmap = new Byte[i_Signature.Length];
                    if (MasterTable.Columns.Contains("i_signature"))
                    {
                        if (!MasterRow["i_signature"].ToString().Contains("undefined"))
                        {
                            i_Signature = Regex.Replace(MasterRow["i_signature"].ToString(), @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
                            if (myFunctions.ContainColumn("i_signature", MasterTable))
                                MasterTable.Columns.Remove("i_signature");
                            ImageBitmap = new Byte[i_Signature.Length];
                            ImageBitmap = Convert.FromBase64String(i_Signature);
                            SigEnable = true;
                        }
                    }

                    Byte[] ImageBitmap2 = new Byte[i_signature2.Length];
                    if (MasterTable.Columns.Contains("i_signature2"))
                    {
                        if (!MasterRow["i_signature2"].ToString().Contains("undefined"))
                        {
                            i_signature2 = Regex.Replace(MasterRow["i_signature2"].ToString(), @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
                            if (myFunctions.ContainColumn("i_signature2", MasterTable))
                                MasterTable.Columns.Remove("i_signature2");
                            ImageBitmap2 = new Byte[i_signature2.Length];
                            ImageBitmap2 = Convert.FromBase64String(i_signature2);
                            SigEnable = true;
                        }
                    }

                    //Saving Signature

                    N_DeliveryNoteID = dLayer.SaveData("Inv_DeliveryNote", "N_DeliveryNoteId", MasterTable, connection, transaction);


                    if (N_DeliveryNoteID > 0)
                    {
                        SortedList statusParams = new SortedList();
                        statusParams.Add("@N_CompanyID", N_CompanyID);
                        statusParams.Add("@N_TransID", N_DeliveryNoteID);
                        statusParams.Add("@N_FormID", 884);
                        statusParams.Add("@N_ForceUpdate", 1);
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_TxnStatusUpdate", statusParams, connection, transaction);

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }
                    }
                    // if(MasterTable.x_ShippingCode > 0)

                    if (SigEnable)
                    {
                        if (i_Signature.Length > 0)
                            dLayer.SaveImage("Inv_DeliveryNote", "i_signature", ImageBitmap, "N_DeliveryNoteId", N_DeliveryNoteID, connection, transaction);
                        if (i_signature2.Length > 0)
                            dLayer.SaveImage("Inv_DeliveryNote", "i_signature2", ImageBitmap2, "N_DeliveryNoteId", N_DeliveryNoteID, connection, transaction);
                    }

                    if (N_DeliveryNoteID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save Delivery Invoice!"));
                    }
                    // if (B_UserLevel)
                    // {
                    //     Inv_WorkFlowCatalog saving code here
                    // }
                    int N_PRSID = 0;
                    int N_SalesOrderID = 0;
                    int N_SalesQuotationID = 0;
                    int N_DeliveryNoteDetailsID = 0;
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_DeliveryNoteID"] = N_DeliveryNoteID;
                        N_PRSID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_RsID"].ToString());
                        N_SalesOrderID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesOrderID"].ToString());
                        N_SalesQuotationID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesQuotationID"].ToString());
                        if (B_SalesOrder)
                        {
                            if (B_SRS)
                            {
                                if (N_PRSID > 0)
                                    dLayer.ExecuteNonQuery("  Inv_PRS set N_DeliveryNoteID=" + N_DeliveryNoteID + ", N_Processed=3 where N_PRSID=" + N_PRSID + " and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection, transaction);
                            }
                            if (N_SalesOrderID > 0)
                            {
                                dLayer.ExecuteNonQuery("update  Inv_SalesOrder set N_SalesID=" + N_DeliveryNoteID + ", N_Processed=1 where N_SalesOrderID=" + N_SalesOrderID + " and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection, transaction);
                            }
                        }

                        else
                        {
                            if (N_SalesQuotationID > 0)
                                dLayer.ExecuteNonQuery("update  Inv_SalesQuotation set N_SalesID=" + N_DeliveryNoteID + ", N_Processed=1 where N_QuotationID=" + N_SalesQuotationID + " and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection, transaction);
                        }


                        N_DeliveryNoteDetailsID = dLayer.SaveDataWithIndex("Inv_DeliveryNoteDetails", "n_DeliveryNoteDetailsID", "", "", j, DetailTable, connection, transaction);
                        if (N_DeliveryNoteDetailsID > 0)
                        {
                            for (int k = 0; k < rentalItem.Rows.Count; k++)
                            {

                                if (myFunctions.getIntVAL(rentalItem.Rows[k]["rowID"].ToString()) == j)
                                {

                                    rentalItem.Rows[k]["n_TransID"] = N_DeliveryNoteID;
                                    rentalItem.Rows[k]["n_TransDetailsID"] = N_DeliveryNoteDetailsID;


                                    rentalItem.AcceptChanges();
                                }
                                rentalItem.AcceptChanges();
                            }



                            rentalItem.AcceptChanges();
                        }
                        DetailTable.AcceptChanges();
                    }
                    rentalItem.AcceptChanges();
                    if (rentalItem.Columns.Contains("rowID"))
                        rentalItem.Columns.Remove("rowID");
                    if (N_DNoteID > 0 && rentalItem.Rows.Count > 0)
                    {
                        N_FormID = myFunctions.getIntVAL(rentalItem.Rows[0]["n_FormID"].ToString());
                        dLayer.ExecuteScalar("delete from Inv_RentalSchedule where N_TransID=" + N_DNoteID.ToString() + " and N_FormID=" + N_FormID + " and N_CompanyID=" + N_CompanyID, connection, transaction);

                    }
                    if (rentalItem.Rows.Count > 0)
                        dLayer.SaveData("Inv_RentalSchedule", "N_ScheduleID", rentalItem, connection, transaction);

                    //int N_DeliveryNoteDetailsID = dLayer.SaveData("Inv_DeliveryNoteDetails", "n_DeliveryNoteDetailsID", DetailTable, connection, transaction);
                    if (N_DeliveryNoteDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save Delivery Note!"));
                    }
                    else
                    {
                        if (N_SaveDraft == 0)
                        {

                            SortedList ParamInsNew = new SortedList();
                            ParamInsNew.Add("N_CompanyID", N_CompanyID);
                            ParamInsNew.Add("N_SalesID", N_DeliveryNoteID);
                            ParamInsNew.Add("N_SaveDraft", 0);

                            SortedList ParamSales_Posting = new SortedList();
                            ParamSales_Posting.Add("N_CompanyID", N_CompanyID);
                            ParamSales_Posting.Add("X_InventoryMode", "DELIVERY");
                            ParamSales_Posting.Add("N_InternalID", N_DeliveryNoteID);
                            ParamSales_Posting.Add("N_UserID", N_UserID);
                            ParamSales_Posting.Add("X_SystemName", "ERP Cloud");
                            try
                            {
                                dLayer.ExecuteNonQueryPro("SP_DeliveryNoteDetails_InsNew", ParamInsNew, connection, transaction);
                                dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Sales_Posting", ParamSales_Posting, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                if (ex.Message == "50")
                                    return Ok(_api.Error(User, "Day Closed"));
                                else if (ex.Message == "51")
                                    return Ok(_api.Error(User, "Year Closed"));
                                else if (ex.Message == "52")
                                    return Ok(_api.Error(User, "Year Exists"));
                                else if (ex.Message == "53")
                                    return Ok(_api.Error(User, "Period Closed"));
                                else if (ex.Message == "54")
                                    return Ok(_api.Error(User, "Txn Date"));
                                else if (ex.Message == "55")
                                    return Ok(_api.Error(User, "Product is not available for delivery"));
                                else return Ok(_api.Error(User, ex));
                            }

                            //Activity Log
                            string ipAddress = "";
                            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                                ipAddress = Request.Headers["X-Forwarded-For"];
                            else
                                ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                            myFunctions.LogScreenActivitys(N_FnYearID, N_DeliveryNoteID, InvoiceNo, 884, xButtonAction, ipAddress, "", User, dLayer, connection, transaction);


                            SortedList CustomerParams = new SortedList();
                            CustomerParams.Add("@nCustomerID", N_CustomerID);
                            DataTable CustomerInfo = dLayer.ExecuteDataTable("Select X_CustomerCode,X_CustomerName from Inv_Customer where N_CustomerID=@nCustomerID", CustomerParams, connection, transaction);
                            if (CustomerInfo.Rows.Count > 0)
                            {
                                try
                                {
                                    myAttachments.SaveAttachment(dLayer, Attachment, InvoiceNo, N_DeliveryNoteID, CustomerInfo.Rows[0]["X_CustomerName"].ToString().Trim(), CustomerInfo.Rows[0]["X_CustomerCode"].ToString(), N_CustomerID, "Customer Document", User, connection, transaction);
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, ex));
                                }
                            }

                            //StatusUpdate
                            int tempSOID = 0, tempSQID = 0;
                            int N_SQID = 0;
                            for (int j = 0; j < DetailTable.Rows.Count; j++)
                            {
                                N_SalesOrderID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesOrderID"].ToString());
                                if (N_SalesOrderID > 0 && N_SalesOrderID != tempSOID)
                                {
                                    if (!myFunctions.UpdateTxnStatus(N_CompanyID, N_SalesOrderID, 81, false, dLayer, connection, transaction))
                                    {
                                        transaction.Rollback();
                                        return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                    }
                                }
                                tempSOID = N_SalesOrderID;

                                N_SQID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesQuotationID"].ToString());
                                if (N_SQID > 0 && N_SQID != tempSQID)
                                {
                                    if (!myFunctions.UpdateTxnStatus(N_CompanyID, N_SQID, 80, false, dLayer, connection, transaction))
                                    {
                                        transaction.Rollback();
                                        return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                    }
                                }
                                tempSQID = N_SQID;
                            };
                        }

                        for (int k = 0; k < rentalItem.Rows.Count; k++)
                        {
                            if (!myFunctions.getBoolVAL(rentalItem.Rows[k]["B_Select"].ToString())) continue;
                            int nItemID = myFunctions.getIntVAL(rentalItem.Rows[k]["n_ItemID"].ToString());
                            int nAssItemID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select isNull(N_AssItemID,0) from Inv_ItemMaster where N_CompanyID=@nCompanyID and N_ItemID=" + nItemID, QueryParams, connection, transaction).ToString());
                            int nRentalEmpID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select isNull(N_RentalEmpID,0) from Inv_ItemMaster where N_CompanyID=@nCompanyID and N_ItemID=" + nItemID, QueryParams, connection, transaction).ToString());

                            if (nAssItemID > 0)
                                dLayer.ExecuteNonQuery("update Ass_AssetMaster Set N_RentalStatus=1 where N_ItemID=" + nAssItemID + " and N_CompanyID=@nCompanyID ", QueryParams, connection, transaction);
                            if (nRentalEmpID > 0)
                                dLayer.ExecuteNonQuery("update Pay_Employee Set N_RentalStatus=1 where N_EmpID=" + nRentalEmpID + " and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID ", QueryParams, connection, transaction);
                        }

                        SortedList Result = new SortedList();
                        Result.Add("n_DeliveryNoteID", N_DeliveryNoteID);
                        Result.Add("InvoiceNo", InvoiceNo);
                        transaction.Commit();
                        if (N_FormID == 1572)
                        {
                            return Ok(_api.Success(Result, "Rental Delivery Saved"));
                        }
                        else if (N_FormID == 1426)
                        {
                            return Ok(_api.Success(Result, "Wh deliveryNote Saved Successfully"));
                        }
                        else if (N_FormID == 1758)
                        {
                            return Ok(_api.Success(Result, "Book Delivery Saved Successfully"));
                        }
                        return Ok(_api.Success(Result, "Delivery Note saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
        // private bool ValidateIMEIs(int row,DataSet ds,int nSalesID,string imeiFrom,string imeiTo,SortedList QueryParams,SqlConnection connection,SqlTransaction transaction)
        // {
        //     if (ds.Tables.Contains("ValidateIMEIs"))
        //         ds.Tables.Remove("ValidateIMEIs");
        //     if (nSalesID == 0)
        //     {
        //         if (Regex.Matches(imeiFrom, @"[a-zA-Z]").Count > 0)
        //         dLayer.ExecuteScalar("Select TOP (1) ISNULL(N_CustomerID,0) from vw_SalesAmount_Customer where N_SalesID=@nSalesID", QueryParams, connection, transaction);
        //         ds.Tables["ValidateIMEIs"] = dLayer.ExecuteDataTable("Select TOP (1) ISNULL(N_CustomerID,0) from vw_SalesAmount_Customer where N_SalesID=@nSalesID", QueryParams, connection);
        //             dba.FillDataSet(ref ds, "ValidateIMEIs", "select Isnull(count(1),0) As N_IMEICount from Inv_StockMaster_IMEI where N_IMEI='" + flxSales.get_TextMatrix(row, mcSerialFrom) + "' and N_Status = 0 and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());
        //         else
        //             dba.FillDataSet(ref ds, "ValidateIMEIs", "select Isnull(count(1),0) As N_IMEICount from Inv_StockMaster_IMEI where ISNUMERIC(N_IMEI)>0  and convert(decimal(38),N_IMEI) between  " + flxSales.get_TextMatrix(row, mcSerialFrom) + " and  " + flxSales.get_TextMatrix(row, mcSerialTo) + " and N_Status = 0 and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());
        //         if (ds.Tables["ValidateIMEIs"].Rows.Count != 0)
        //         {
        //             if (myFunctions.getLongIntVAL(ds.Tables["ValidateIMEIs"].Rows[0]["N_IMEICount"].ToString()) != myFunctions.getLongIntVAL(flxSales.get_TextMatrix(row, mcQuantity)))
        //             {
        //                 msg.msgInformation("Could not sale some items (Already sold out/Item not found) entered in row number " + row.ToString());
        //                 return false;
        //             }
        //         }
        //     }
        //     else
        //     {
        //         if (Regex.Matches(imeiFrom, @"[a-zA-Z]").Count > 0)
        //         {
        //             if (imeiFrom != imeiTo)
        //             {
        //                 int N_Status = myFunctions.getIntVAL(dba.ExecuteSclar("Select N_Status from Inv_StockMaster_IMEI Where N_IMEI='" + flxSales.get_TextMatrix(row, mcSerialTo) + "' and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable()).ToString());
        //                 if (N_Status == 1)
        //                 {
        //                     msg.msgInformation("Could not sale some items (Already sold out/Item not found) entered in row number " + row.ToString());
        //                     return false;
        //                 }
        //                 else
        //                 {
        //                     dba.FillDataSet(ref ds, "ValidateIMEIs", "select Isnull(count(1),0) As N_IMEICount from Inv_StockMaster_IMEI where N_IMEI='" + flxSales.get_TextMatrix(row, mcSerialFrom) + "' and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());
        //                     if (ds.Tables["ValidateIMEIs"].Rows.Count != 0)
        //                     {
        //                         if (myFunctions.getLongIntVAL(ds.Tables["ValidateIMEIs"].Rows[0]["N_IMEICount"].ToString()) != myFunctions.getLongIntVAL(flxSales.get_TextMatrix(row, mcQuantity)))
        //                         {
        //                             msg.msgInformation("Could not sale some items (Already sold out/Item not found) entered in row number " + row.ToString());
        //                             return false;
        //                         }
        //                     }
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             if (X_PrevImeito != myFunctions.getVAL(flxSales.get_TextMatrix(row, mcSerialTo)))
        //             {
        //                 int N_Status = myFunctions.getIntVAL(dba.ExecuteSclar("Select N_Status from Inv_StockMaster_IMEI Where N_IMEI ='" + flxSales.get_TextMatrix(row, mcSerialTo) + "' and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable()).ToString());
        //                 if (N_Status == 1)
        //                 {
        //                     msg.msgInformation("Could not sale some items (Already sold out/Item not found) entered in row number " + row.ToString());
        //                     return false;
        //                 }
        //                 else
        //                 {
        //                     dba.FillDataSet(ref dsSales, "ValidateIMEIs", "select Isnull(count(1),0) As N_IMEICount from Inv_StockMaster_IMEI where ISNUMERIC(N_IMEI)>0  and convert(decimal(38),N_IMEI) between  " + flxSales.get_TextMatrix(row, mcSerialFrom) + " and  " + flxSales.get_TextMatrix(row, mcSerialTo) + " and N_CompanyID=" + myCompanyID._CompanyID, "TEXT", new DataTable());
        //                     if (dsSales.Tables["ValidateIMEIs"].Rows.Count != 0)
        //                     {
        //                         if (myFunctions.getLongIntVAL(dsSales.Tables["ValidateIMEIs"].Rows[0]["N_IMEICount"].ToString()) != myFunctions.getLongIntVAL(flxSales.get_TextMatrix(row, mcQuantity)))
        //                         {
        //                             msg.msgInformation("Could not sale some items (Already sold out/Item not found) entered in row number " + row.ToString());
        //                             return false;
        //                         }
        //                     }
        //                 }
        //             }
        //         }
        //     }         
        //     return true;
        // }
        //Delete....
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nDeliveryNoteID, int nCustomerID, int nCompanyID, int nFnYearID, int nBranchID, int NShippingID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    SqlTransaction transaction = connection.BeginTransaction();
                    var xUserCategory = User.FindFirst(ClaimTypes.GroupSid)?.Value;
                    var nUserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    ParamList.Add("@nTransID", nDeliveryNoteID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    string Sql = "select N_DeliveryNoteId,X_ReceiptNo,N_FormID from Inv_DeliveryNote where N_DeliveryNoteId=@nTransID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID";
                    string xButtonAction = "Delete";
                    String X_ReceiptNo = "";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection, transaction);



                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];


                    //Activity Log
                    string ipAddress = "";
                    if (Request.Headers.ContainsKey("X-Forwarded-For"))
                        ipAddress = Request.Headers["X-Forwarded-For"];
                    else
                        ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    myFunctions.LogScreenActivitys(myFunctions.getIntVAL(nFnYearID.ToString()), nDeliveryNoteID, TransRow["X_ReceiptNo"].ToString(), 884, xButtonAction, ipAddress, "", User, dLayer, connection, transaction);


                    //Results = dLayer.DeleteData("Inv_SalesInvoice", "n_InvoiceID", N_InvoiceID, "",connection,transaction);
                    SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"N_UserID",nUserID},
                                {"X_TransType","DELIVERY"},
                                {"X_SystemName","WebRequest"},
                                {"N_VoucherID",nDeliveryNoteID}};

                    SortedList QueryParams = new SortedList(){
                                {"@nCompanyID",nCompanyID},
                                {"@nFnYearID",nFnYearID},
                                {"@nUserID",nUserID},
                                {"@xTransType","DELIVERY"},
                                {"@xSystemName","WebRequest"},
                                {"@nDeliveryNoteID",nDeliveryNoteID},
                                {"@nPartyID",nCustomerID},
                                {"@nBranchID",nBranchID}};

                    ParamList.Add("@NShippingID", NShippingID);
                    ParamList.Add("@nDeliveryNoteID", nDeliveryNoteID);
                    DataTable DetailTable = dLayer.ExecuteDataTable("select n_SalesOrderID,n_SalesQuotationID from Inv_DeliveryNoteDetails where N_CompanyID=@nCompanyID and N_DeliveryNoteID=@nDeliveryNoteID group by n_SalesOrderID,n_SalesQuotationID order by n_SalesOrderID,n_SalesQuotationID", QueryParams, connection, transaction);
                    DataTable rentalItem = dLayer.ExecuteDataTable("select * from Inv_RentalSchedule where N_CompanyID=@nCompanyID and N_FormID=1572 and N_TransID=@nDeliveryNoteID ", QueryParams, connection, transaction);
                    Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                    string DeliveryNoteID = "select * from Inv_Shipping where N_CompanyID=@nCompanyID and N_deliverynoteid=@nDeliveryNoteID ";
                    TransData = dLayer.ExecuteDataTable(DeliveryNoteID, ParamList, connection, transaction);
                    if (TransData.Rows.Count > 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Please delete Shipping Invoice for deleting this Delivery Note"));
                    }


                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete delivery note"));
                    }
                    else
                    {
                        dLayer.ExecuteNonQuery("delete from Inv_StockMaster where N_SalesID=@nDeliveryNoteID and n_CompanyID=@nCompanyID", QueryParams, connection, transaction);

                        dLayer.DeleteData("Inv_RentalSchedule", "N_TransID", nDeliveryNoteID, "  N_CompanyID=" + nCompanyID + " and N_FormID=1572 ", connection, transaction);

                        myAttachments.DeleteAttachment(dLayer, 1, nDeliveryNoteID, nCustomerID, nFnYearID, this.FormID, User, transaction, connection);
                    }

                    //Attachment delete code here

                    //TxnUpdate
                    int tempSOID = 0, tempSQID = 0;
                    int N_SQID = 0, N_SalesOrderID = 0;
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        N_SalesOrderID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesOrderID"].ToString());
                        if (N_SalesOrderID > 0 && N_SalesOrderID != tempSOID)
                        {

                            if (!myFunctions.UpdateTxnStatus(nCompanyID, N_SalesOrderID, 81, true, dLayer, connection, transaction))
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, "Unable To Update Txn Status"));
                            }

                            else
                            {
                                if (!myFunctions.UpdateTxnStatus(nCompanyID, N_SalesOrderID, 81, false, dLayer, connection, transaction))
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Unable To Update Txn Status"));
                                }
                            }
                        }
                        tempSOID = N_SalesOrderID;

                        N_SQID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_SalesQuotationID"].ToString());
                        if (N_SQID > 0 && N_SQID != tempSQID)
                        {
                            if (!myFunctions.UpdateTxnStatus(nCompanyID, N_SQID, 80, true, dLayer, connection, transaction))
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, "Unable To Update Txn Status"));
                            }
                        }
                        tempSQID = N_SQID;
                    };

                    for (int k = 0; k < rentalItem.Rows.Count; k++)
                    {
                        if (!myFunctions.getBoolVAL(rentalItem.Rows[k]["B_Select"].ToString())) continue;
                        int nItemID = myFunctions.getIntVAL(rentalItem.Rows[k]["n_ItemID"].ToString());
                        int nAssItemID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select isNull(N_AssItemID,0) from Inv_ItemMaster where N_CompanyID=@nCompanyID and N_ItemID=" + nItemID, QueryParams, connection, transaction).ToString());
                        int nRentalEmpID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select isNull(N_RentalEmpID,0) from Inv_ItemMaster where N_CompanyID=@nCompanyID and N_ItemID=" + nItemID, QueryParams, connection, transaction).ToString());

                        if (nAssItemID > 0)
                            dLayer.ExecuteNonQuery("update Ass_AssetMaster Set N_RentalStatus=0 where N_ItemID=" + nAssItemID + " and N_CompanyID=@nCompanyID ", QueryParams, connection, transaction);
                        if (nRentalEmpID > 0)
                            dLayer.ExecuteNonQuery("update Pay_Employee Set N_RentalStatus=0 where N_EmpID=" + nRentalEmpID + " and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID ", QueryParams, connection, transaction);
                    }

                    transaction.Commit();
                    if (myFunctions.getIntVAL(TransRow["N_FormID"].ToString()) == 1758)
                    {
                        return Ok(_api.Success("Book Delivery deleted successfully"));
                    }
                    else if (myFunctions.getIntVAL(TransRow["N_FormID"].ToString()) == 1572)
                    {
                        return Ok(_api.Success("Rental Delivery deleted successfully"));
                    }
                    else
                    {
                        return Ok(_api.Success("Delivery note deleted successfully"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }


        }

        [HttpGet("deliveryNoteSearch")]
        public ActionResult GetInvoiceList(int? nCompanyId, int nCustomerId, bool bAllBranchData, int nBranchId, int nLocationId)
        {
            SortedList Params = new SortedList();

            string crieteria = "";


            if (bAllBranchData == true)
            {
                if (nCustomerId > 0)
                    crieteria = " where X_TransType='DELIVERY' and N_DeliveryType = 0 and N_CustomerID=@nCustomerId and N_CompanyID=@nCompanyId and B_IsSaveDraft=0";
                else
                    crieteria = " where X_TransType='DELIVERY' and N_DeliveryType = 0 and N_CompanyID=@nCompanyId and B_IsSaveDraft=0";
            }
            else
            {
                if (nCustomerId > 0)
                    crieteria = " where X_TransType='DELIVERY' and N_DeliveryType = 0 and N_CustomerID=@nCustomerId and N_CompanyID=@nCompanyId and N_BranchID=@nBranchId and N_LocationID=@nLocationId and B_IsSaveDraft=0";
                else
                    crieteria = " where X_TransType='DELIVERY' and N_DeliveryType = 0 and N_CompanyID=@nCompanyId and N_BranchID=@nBranchId and N_LocationID=@nLocationId and B_IsSaveDraft=0";
            }

            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nCustomerId", nCustomerId);
            Params.Add("@bAllBranchData", bAllBranchData);
            Params.Add("@nBranchId", nBranchId);
            Params.Add("@nLocationId", nLocationId);
            string sqlCommandText = "select [Invoice No],[Invoice Date],[Customer] as X_CustomerName,N_CompanyID,N_CustomerID,N_DeliveryNoteId,N_DeliveryType,X_TransType,N_FnYearID,N_BranchID,X_LocationName,N_LocationID,B_IsSaveDraft from vw_InvDeliveryNote_Search " + crieteria + " order by N_DeliveryNoteId DESC,[Invoice No]";
            try
            {
                DataTable SalesInvoiceList = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SalesInvoiceList = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SalesInvoiceList = _api.Format(SalesInvoiceList);
                    if (SalesInvoiceList.Rows.Count == 0) { return Ok(_api.Notice("No Sales Invoices Found")); }
                }
                return Ok(_api.Success(SalesInvoiceList));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }



        [HttpGet("multipleSalesOrder")]
        public ActionResult ProductList(int nFnYearID, int nCustomerID, bool bAllbranchData, int nBranchID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);

            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nCustomerID", nCustomerID);


            string sqlCommandText = "";
            if (bAllbranchData)
                sqlCommandText = "Select * from vw_pendingSO Where N_CompanyID=@nCompanyID and N_CustomerID=@nCustomerID";
            else
                sqlCommandText = "Select * from vw_pendingSO Where N_CompanyID=@nCompanyID and N_CustomerID=@nCustomerID";

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
    }
}