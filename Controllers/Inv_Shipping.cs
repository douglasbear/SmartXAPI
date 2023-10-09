using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;
using System.Net;
using ZXing;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("shippingInvoice")]
    [ApiController]
    public class Inv_Shipping : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID = 1807;
        private readonly IApiFunctions api;

        public Inv_Shipping(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
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
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nShippingID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ShippingID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    string X_ShippingCode = MasterTable.Rows[0]["x_ShippingCode"].ToString();
                    int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                    string xButtonAction = "";


                    if ( X_ShippingCode != "@Auto")
                    {
                    object N_DocNumber = dLayer.ExecuteScalar("Select 1 from Inv_Shipping Where X_ShippingCode ='" + X_ShippingCode + "' and N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection, transaction);
                    if(N_DocNumber == null)
                    {
                        N_DocNumber = 0;
                    }
                    if (myFunctions.getVAL(N_DocNumber.ToString()) >= 1)
                    {
                        // transaction.Rollback();
                        return Ok(_api.Error(User, "Invoice number already in use"));
                    //    transaction.Rollback();
                    //     return Ok(_api.Error(User, "Invoice No is Already exist"));
                    }
                    }
                    if (nShippingID > 0)
                    {
                        dLayer.DeleteData("Inv_ShippingDetails", "N_ShippingID", nShippingID, "N_CompanyID = " + nCompanyID, connection, transaction);
                        xButtonAction = "Update";
                    }

                    DocNo = MasterRow["x_ShippingCode"].ToString();
                    if (X_ShippingCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", FormID);
                        Params.Add("N_YearID", nFnYearID);


                        while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            xButtonAction = "Insert";
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_Shipping Where X_ShippingCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);

                            if (N_Result == null)
                                break;
                        }
                        X_ShippingCode = DocNo;


                        if (X_ShippingCode == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to generate"));
                        }
                        MasterTable.Rows[0]["x_ShippingCode"] = X_ShippingCode;

                    }
                    X_ShippingCode = MasterTable.Rows[0]["x_ShippingCode"].ToString();


                    nShippingID = dLayer.SaveData("Inv_Shipping", "N_ShippingID", MasterTable, connection, transaction);
                    
                    if (nShippingID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_ShippingID"] = nShippingID;
                    }
                    if (DetailTable.Rows.Count > 0)
                    {
                        int nShippingDetailsID = dLayer.SaveData("Inv_ShippingDetails", "N_ShippingDetailsID", DetailTable, connection, transaction);

                        if (nShippingDetailsID <= 0)
                        {


                            transaction.Rollback();
                            return Ok("Unable To Save");

                        }

                        string ipAddress = "";
                        if (Request.Headers.ContainsKey("X-Forwarded-For"))
                            ipAddress = Request.Headers["X-Forwarded-For"];
                        else
                            ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                        myFunctions.LogScreenActivitys(nFnYearID, nShippingID, X_ShippingCode, 1807, xButtonAction, ipAddress, "", User, dLayer, connection, transaction);

                        //  Activity Log
                        //  SortedList PostingParam = new SortedList();
                        //     PostingParam.Add("N_CompanyID", nCompanyID);
                        //     PostingParam.Add("N_ShippingID", nShippingID);
                        //     PostingParam.Add("N_UserID", myFunctions.GetUserID(User));
                        //     PostingParam.Add("X_EntryFrom", nFnYearID);
                        //     PostingParam.Add("X_SystemName", System.Environment.MachineName);
                        //     try
                        //     {
                        //         dLayer.ExecuteNonQueryPro("Inv_Shipping", PostingParam, connection, transaction);
                        //     }
                        //     catch (Exception ex)
                        //     {
                        //         transaction.Rollback();
                        //         return Ok(_api.Error(User,ex));
                        //     }

                    }

                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }





        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID, string X_ShippingCode, int nFnYearID, int nShippingID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    SortedList PostingDelParam = new SortedList();
                    SortedList Params = new SortedList();
                    SortedList PostingDelParam1 = new SortedList();
                    String detailSql = "";
                    DataTable DetailTable = new DataTable();
                    SortedList ParamList = new SortedList();
                    DataTable TransData = new DataTable();
                    ParamList.Add("@nTransID", nShippingID);
                    ParamList.Add("@nAcYearID", nFnYearID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    string xButtonAction = "Delete";
                    // string X_ReceiptNo="";
                    Params.Add("@nCompanyID", nCompanyID);

                    PostingDelParam.Add("N_CompanyID", nCompanyID);
                    PostingDelParam.Add("X_TransType", "ELI");
                    PostingDelParam.Add("X_ReferenceNo", X_ShippingCode);
                    PostingDelParam.Add("N_FnYearID", nFnYearID);

                    PostingDelParam1.Add("N_CompanyID", nCompanyID);
                    PostingDelParam1.Add("X_TransType", "ESP");
                    PostingDelParam1.Add("X_ReferenceNo", X_ShippingCode);
                    PostingDelParam1.Add("N_FnYearID", nFnYearID);

                    detailSql = "select * from Inv_ShippingDetails where N_ShippingID=" + nShippingID + " ";
                    DetailTable = dLayer.ExecuteDataTable(detailSql, Params, connection);
                    SqlTransaction transaction = connection.BeginTransaction();
                    ;
                    string Sql = "select n_ShippingID,X_ShippingCode from Inv_Shipping where n_ShippingID=@nTransID and N_CompanyID=@nCompanyID ";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection, transaction);

                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];
                    //  Activity Log
                    string ipAddress = "";
                    if (Request.Headers.ContainsKey("X-Forwarded-For"))
                        ipAddress = Request.Headers["X-Forwarded-For"];
                    else
                        ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    myFunctions.LogScreenActivitys(myFunctions.getIntVAL(nFnYearID.ToString()), nShippingID, TransRow["X_ShippingCode"].ToString(), 1807, xButtonAction, ipAddress, "", User, dLayer, connection, transaction);



                    // for (int i = DetailTable.Rows.Count - 1; i >= 0; i--)
                    // {


                    //     if (myFunctions.getIntVAL(DetailTable.Rows[0]["n_Entryfrom"].ToString()) == 212)
                    //     {



                    //         try
                    //         {
                    //             Results = dLayer.ExecuteNonQueryPro("Inv_Shipping", PostingDelParam, connection, transaction);
                    //         }
                    //         catch (Exception ex)
                    //         {
                    //             transaction.Rollback();
                    //             return Ok(_api.Error(User, ex));
                    //         }
                    //     }

                    //     else
                    //     {


                    //         try
                    //         {
                    //             Results = dLayer.ExecuteNonQueryPro("Inv_Shipping", PostingDelParam1, connection, transaction);
                    //         }
                    //         catch (Exception ex)
                    //         {
                    //             transaction.Rollback();
                    //             return Ok(_api.Error(User, ex));
                    //         }

                    //     }
                    // }
                    Results = dLayer.DeleteData("Inv_ShippingDetails", "N_ShippingID", nShippingID, "", connection, transaction);
                    if (Results > 0)
                    {
                        dLayer.DeleteData("Inv_Shipping", "N_ShippingID", nShippingID, "", connection, transaction);
                        transaction.Commit();
                        return Ok(_api.Success( " Shipping Invoice Deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Unable to delete"));
                    }

                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("details")]
        public ActionResult ShippingInvoiceDetails(string xShippingCode, bool bAllBranchData, int nFnYearID, int n_DeliveryNoteId, string xDeliveryNoteID, int nFormID, int nBranchId, string xInvoiceNo)
        {
            DataSet dt = new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", xShippingCode);
            // Params.Add("@nFnYearID", nFnYearID);
            // // Params.Add("@nClassID", nClassID);
            // Params.Add("@nCompanyID", nCompanyId);

            // Params.Add("@nFormID", nFormID);
            object N_SalesOrderID = 0;
            string DetailGetSql = "";
            int N_DeliveryNote = 0;
            string x_DeliveryNoteNo = "";

            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                   
                    DataSet dsSalesInvoice = new DataSet();
                    SortedList QueryParamsList = new SortedList();
                    DataTable DelDetails = new DataTable();
                    
                    if (n_DeliveryNoteId > 0 || (xDeliveryNoteID != "" && xDeliveryNoteID != null))
                    {
                        // DataTable MasterTable = new DataTable();
                        DataTable DeliveryNoteNumber = new DataTable();
                        int N_salesOrderID = 0;
                        string Mastersql = "";
                        // string xDeliveryNo="";



                        if (n_DeliveryNoteId > 0)
                        {
                            Params.Add("@n_DeliveryNoteId", n_DeliveryNoteId);
                           // Params.Add("@nCompanyID", nCompanyId);
                            Mastersql = "select * from vw_DeliveryNoteToShippingMaster where N_CompanyId=" + nCompanyId + " and N_DeliveryNoteId=" + n_DeliveryNoteId+"";
                            MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, connection);
                            if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                            MasterTable = _api.Format(MasterTable, "Master");
                            N_salesOrderID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_salesOrderID"].ToString());
                            xDeliveryNoteID = n_DeliveryNoteId.ToString();
                           

                            if (xDeliveryNoteID != null)
                            {
                                object purchaseOrderNo = dLayer.ExecuteScalar("select N_SalesOrderID from vw_DeliveryNoteToShippingDetails where N_CompanyID=" + nCompanyId + " and N_DeliveryNoteID=" + xDeliveryNoteID, QueryParamsList, connection);
                                if (purchaseOrderNo == null)
                                    purchaseOrderNo = 0;
                            }
                        }
                        else
                        {
                            QueryParamsList.Add("@n_DeliveryNoteId", n_DeliveryNoteId);
                            string[] X_Delivery = xDeliveryNoteID.Split(",");
                            N_DeliveryNote = myFunctions.getIntVAL(X_Delivery[0].ToString());

                            //  MasterTable = dLayer.ExecuteDataTable(xDeliveryNo, QueryParamsList, Con);

                            Mastersql = "select * from vw_DeliveryNoteToShippingMaster where N_CompanyId=@nCompanyID and N_DeliveryNoteId=" + N_DeliveryNote + "";
                            MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, connection);
                            if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                            MasterTable = _api.Format(MasterTable, "Master");


                        }

                         string DetailSql = "";
                        //  DetailSql = "select * from vw_DeliveryNoteToShippingDetails where N_CompanyId=@nCompanyID and N_DeliveryNoteID=@n_DeliveryNoteId ";



                        string DeliveryNoteAppend = "0";
                        DataTable DeliveryNoteID = new DataTable();
                        if (nFormID != 1601)
                        {
                            if (N_salesOrderID > 0)
                                DeliveryNoteID = dLayer.ExecuteDataTable("select N_DeliveryNoteID from Inv_SalesDetails Where N_SalesOrderID=" + N_salesOrderID + "", QueryParamsList, connection);
                        }

                        if (DeliveryNoteID.Rows.Count > 0)
                        {

                            foreach (DataRow Avar in DeliveryNoteID.Rows)
                            {
                                if (Avar["N_DeliveryNoteID"].ToString() != "0")
                                    DeliveryNoteAppend = DeliveryNoteAppend + "," + Avar["N_DeliveryNoteID"].ToString();
                            }
                            if (xDeliveryNoteID == "" || xDeliveryNoteID == null)
                                DetailSql = "select * from vw_DeliveryNoteToShippingDetails where N_CompanyId="+ nCompanyId +" and N_SalesOrderID =" + N_salesOrderID + " and N_DeliveryNoteID not in( " + DeliveryNoteAppend + ") order by N_SOdetailsID ASC ";
                            else
                                DetailSql = "select * from vw_DeliveryNoteToShippingDetails where N_CompanyId="+ nCompanyId +" and N_DeliveryNoteID IN (" + xDeliveryNoteID + ") order by N_SOdetailsID ASC ";

                        }
                        else
                        {
                            if ((xDeliveryNoteID == "" || xDeliveryNoteID == null) && N_salesOrderID > 0)
                                DetailSql = "select * from vw_DeliveryNoteToShippingDetails where N_CompanyId="+ nCompanyId +" and N_SalesOrderID =" + N_salesOrderID + " order by N_SOdetailsID ASC ";
                            else
                                DetailSql = "select * from vw_DeliveryNoteToShippingDetails where N_CompanyId=" + nCompanyId + "  and N_DeliveryNoteID IN (" + xDeliveryNoteID + ")  order by N_SOdetailsID ASC ";

                        }
                        SortedList mParamsList = new SortedList()
                        {
                            {"N_CompanyID",nCompanyId},
                            {"X_ReceiptNo",xInvoiceNo},
                            {"X_TransType","SALES"},
                            {"N_FnYearID",nFnYearID},
                        };
                        if (!bAllBranchData)
                        {
                            mParamsList.Add("N_BranchId", nBranchId);
                        }
                        else
                        {
                            mParamsList.Add("N_BranchId", 0);
                        }
                        // DataTable masterTable = dLayer.ExecuteDataTablePro("SP_InvSales_Disp", mParamsList, connection);
                        // masterTable = _api.Format(masterTable, "Master");
                        // if (masterTable.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                        // DataRow MasterRow = masterTable.Rows[0];
                        // int nSalesID = myFunctions.getIntVAL(MasterRow["N_SalesID"].ToString());
                        // QueryParamsList.Add("@nSalesID", nSalesID);






                        // DetailGetSql = "select X_ReceiptNo from Inv_DeliveryNote where N_DeliveryNoteID in ( select N_DeliveryNoteID from Inv_SalesDetails where  N_SalesID=@nSalesID)";
                        // DelDetails = dLayer.ExecuteDataTable(DetailGetSql, QueryParamsList, connection);
                        // if (DelDetails.Rows.Count > 0)
                        // {
                        //     x_DeliveryNoteNo = DelDetails.Rows[0]["X_ReceiptNo"].ToString();
                        //     for (int j = 1; j < DelDetails.Rows.Count; j++)
                        //     {
                        //         x_DeliveryNoteNo = x_DeliveryNoteNo + "," + DelDetails.Rows[j]["X_ReceiptNo"].ToString();
                        //     }
                        // }
                        // myFunctions.AddNewColumnToDataTable(MasterTable, "X_DeliveryNoteNo", typeof(string), x_DeliveryNoteNo);
                        // MasterTable.AcceptChanges();


                        // if (myFunctions.getIntVAL(MasterTable.Rows[0]["N_DeliveryNoteId"].ToString()) > 0)
                        // {
                        //     QueryParamsList.Add("@n_DeliveryNoteId", myFunctions.getIntVAL(MasterTable.Rows[0]["N_DeliveryNoteId"].ToString()));
                        //     myFunctions.AddNewColumnToDataTable(MasterTable, "X_FileNo", typeof(string), "");
                        //     SortedList ProParamList = new SortedList()
                        // {
                        //     {"N_CompanyID",nCompanyId},
                        //     {"N_FnYearID",nFnYearID},
                        //     {"N_PkID",myFunctions.getIntVAL(MasterTable.Rows[0]["N_DeliveryNoteId"].ToString())},
                        //     {"Type","DN"}
                        // };
                        //     object objFileNo = dLayer.ExecuteScalarPro("SP_GetSalesOrder", ProParamList, connection);
                        //     if (objFileNo != null)
                        //         MasterTable.Rows[0]["X_FileNo"] = objFileNo.ToString();
                        // }



                        DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, connection);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dsSalesInvoice.Tables.Add(MasterTable);
                        dsSalesInvoice.Tables.Add(DetailTable);
                        // object shippingDone = null;
                        // shippingDone = dLayer.ExecuteScalar("select n_DeliveryNoteId from Inv_Shipping where N_CompanyID=@nCompanyID and N_ShippingID="+N_ShippingID+"",  connection);



                        return Ok(_api.Success(dsSalesInvoice));

                    }

                    else{

                         string sqlCommandText = "select * from VW_Inv_Shipping_master where N_CompanyID=@p1 and x_ShippingCode=@p2";
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    int N_ShippingID = 0;

                    // if (MasterTable.Rows.Count == 0) 
                    // { return Ok(_api.Warning("No data found"));
                    // }

                    MasterTable = _api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                    N_ShippingID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ShippingID"].ToString());
                    Params.Add("@p3", N_ShippingID);

                    string DetailSql = "select * from VW_Inv_Shipping_details where N_CompanyID= "+ nCompanyId + " and N_ShippingID="+N_ShippingID+"";

                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);


                    }
                    
                   

                    

                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }


// [HttpGet("details")]
//         public ActionResult ShippingInvoiceDetails(string xShippingCode)
//         {
//             DataTable Master = new DataTable();
//             DataTable Detail = new DataTable();
//             DataSet ds = new DataSet();
//             SortedList Params = new SortedList();
//             int nCompanyID = myFunctions.GetCompanyID(User);
//             string xCriteria = "",
//             sqlCommandText = "";
//             Params.Add("@p1", nCompanyID);
//             Params.Add("@p2", xShippingCode);
           
            
//             try
//             {
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     connection.Open();
//                     sqlCommandText = "select * from VW_Inv_Shipping_master where N_CompanyID=@p1 and x_ShippingCode=@p2";
                   
//                     Master = dLayer.ExecuteDataTable(sqlCommandText,Params, connection);
//                     Master = _api.Format(Master, "master");
//                     if (Master.Rows.Count == 0)
//                     {
//                         ds.Tables.Add(Master);
//                         return Ok(_api.Success(ds));
//                     }
//                     else
//                     {
                       
//                         int N_ShippingID = myFunctions.getIntVAL(Master.Rows[0]["N_ShippingID"].ToString());
//                         Params.Add("@p3", N_ShippingID);
//                         ds.Tables.Add(Master);
//                         sqlCommandText = "select * from VW_Inv_Shipping_details where N_CompanyID=@p1 and N_ShippingID=@p3";
//                         Detail = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
//                         if (Detail.Rows.Count == 0)
//                         {
//                             ds.Tables.Add(Detail);
//                         }
//                         Detail = _api.Format(Detail, "Details");
//                         ds.Tables.Add(Detail);
//                     }
//                 }
//                 return Ok(_api.Success(ds));
//             }
//             catch (Exception e)
//             {
//                 return Ok(api.Error(User, e));
//             }
//         }


    }
}


