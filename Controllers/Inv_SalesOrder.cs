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

    [Route("salesorder")]
    [ApiController]
    public class Inv_SalesOrderController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Inv_SalesOrderController(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("list")]
        public ActionResult GetSalesOrderotationList(int? nCompanyId, int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_InvSalesOrderNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 order by D_OrderDate DESC,[Order No]";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = _api.Format(dt);
                }
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
                return BadRequest(_api.Error(e));
            }
        }
        [HttpGet("details")]
        public ActionResult GetSalesOrderDetails(int? nCompanyID, string xOrderNo, int nFnYearID, int nLocationID, bool bAllBranchData, int nBranchID)
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
                Mastersql = "SP_InvSalesOrder_Disp @nCompanyID,@xOrderNo,1,0,@nFnYearID";
            }
            else
            {
                Mastersql = "SP_InvSalesOrder_Disp @nCompanyID,@xOrderNo,1,@nBranchID,@nFnYearID";
                Params.Add("@nBranchID", nBranchID);
            }

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@xOrderNo", xOrderNo);
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
                    bool B_Processed = false;
                    object InSales = false, InDeliveryNote = false, CancelStatus = false;
                    if (myFunctions.getIntVAL(N_SalesOrderTypeID.ToString()) != 175)
                    {
                        if (Convert.ToBoolean(MasterRow["N_Processed"]))
                        {
                            B_Processed = true;
                            InSales = dLayer.ExecuteScalar("select 1 from Inv_Sales where N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);
                            InDeliveryNote = dLayer.ExecuteScalar("select 1 from Inv_DeliveryNote where N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);
                            CancelStatus = dLayer.ExecuteScalar("select 1 from Inv_SalesOrder where B_CancelOrder=1 and N_CompanyID=@nCompanyID and N_SalesOrderId=@nSOrderID", DetailParams, connection);

                        }
                    }
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "SalesDone", typeof(string), InSales);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "DeliveryNoteDone", typeof(string), InDeliveryNote);
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
                    DetailSql = "SP_InvSalesOrderDtls_Disp @nCompanyID,@nSOrderID,@nFnYearID,1,@nLocationID";
                    SortedList NewParams = new SortedList();
                    NewParams.Add("@nLocationID", nLocationID);
                    NewParams.Add("@nFnYearID", nFnYearID);
                    NewParams.Add("@nCompanyID", nCompanyID);
                    NewParams.Add("@nSOrderID", N_SOrderID);
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, NewParams,connection);
                    DetailTable = _api.Format(DetailTable, "Details");


                    DetailTable = myFunctions.AddNewColumnToDataTable(DetailTable, "X_UpdatedSPrice", typeof(string), "");
                    SortedList Param =new SortedList();
                    Param.Add("@nCompanyID",nCompanyID);
                    Param.Add("@nSPriceTypeID","");
                    foreach (DataRow var in DetailTable.Rows)
                    {
                        if (var["N_SPriceTypeID"].ToString() != ""){
                            Params["@nSPriceTypeID"]=var["N_SPriceTypeID"].ToString();
                            var["X_UpdatedSPrice"] = Convert.ToString(dLayer.ExecuteScalar("select X_Name from Gen_LookupTable where N_CompanyID=@nCompanyID and N_ReferId=3 and N_PkeyId=@nSPriceTypeID",Param,connection));
                        }
                    }


                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return BadRequest(_api.Error(e));
            }
        }

        //Save....
        // [HttpPost("Save")]
        // public ActionResult SaveData([FromBody] DataSet ds)
        // {
        //     try
        //     {
        //         DataTable MasterTable;
        //         DataTable DetailTable;
        //         MasterTable = ds.Tables["master"];
        //         DetailTable = ds.Tables["details"];
        //         SortedList Params = new SortedList();
        //         dLayer.setTransaction();
        //         // Auto Gen
        //         string xOrderNo = "";
        //         var values = MasterTable.Rows[0]["X_OrderNo"].ToString();
        //         DataRow Master = MasterTable.Rows[0];
        //         if (values == "@Auto")
        //         {
        //             Params.Add("N_CompanyID", Master["n_CompanyId"].ToString());
        //             Params.Add("N_YearID", Master["n_FnYearId"].ToString());
        //             Params.Add("N_FormID", 81);
        //             Params.Add("N_BranchID", Master["n_BranchId"].ToString());
        //             xOrderNo = dLayer.GetAutoNumber("Inv_SalesOrder", "X_OrderNo", Params);
        //             if (xOrderNo == "") { return StatusCode(409, _api.Response(409, "Unable to generate Sales Order Number")); }
        //             MasterTable.Rows[0]["X_OrderNo"] = xOrderNo;
        //         }


        //         int nSalesOrderID = dLayer.SaveData("Inv_SalesOrder", "N_SalesOrderID", 0, MasterTable);
        //         if (nSalesOrderID <= 0)
        //         {
        //             dLayer.rollBack();
        //             return StatusCode(409, _api.Response(409, "Unable to save sales order"));
        //         }
        //         for (int j = 0; j < DetailTable.Rows.Count; j++)
        //         {
        //             DetailTable.Rows[j]["N_SalesOrderID"] = nSalesOrderID;
        //         }
        //         int N_QuotationDetailId = dLayer.SaveData("Inv_SalesOrderDetails", "N_SalesOrderDetails", 0, DetailTable);
        //         if (N_QuotationDetailId <= 0)
        //         {
        //             dLayer.rollBack();
        //             return StatusCode(409, _api.Response(409, "Unable to save sales order"));
        //         }
        //         else
        //         {
        //             dLayer.commit();
        //         }

        //         return GetSalesOrderDetails(int.Parse(Master["n_CompanyId"].ToString()), MasterTable.Rows[0]["X_OrderNo"].ToString(), int.Parse(Master["n_FnYearId"].ToString()), int.Parse(Master["N_LocationID"].ToString()), true, 1);
        //     }
        //     catch (Exception ex)
        //     {
        //         dLayer.rollBack();
        //         return StatusCode(403, _api.ErrorResponse(ex));
        //     }
        // }
        //Delete....
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nSalesOrderID)
        {
            int Results = 0;
            try
            {
                dLayer.setTransaction();
                Results = dLayer.DeleteData("Inv_SalesOrder", "N_SalesOrderID", nSalesOrderID, "");
                if (Results <= 0)
                {
                    dLayer.rollBack();
                    return StatusCode(409, _api.Response(409, "Unable to delete sales order"));
                }
                else
                {
                    Results = dLayer.DeleteData("Inv_SalesOrderDetails", "N_SalesOrderID", nSalesOrderID, "");
                }

                if (Results > 0)
                {
                    dLayer.commit();
                    return StatusCode(200, _api.Response(200, "Sales order deleted"));
                }
                else
                {
                    dLayer.rollBack();
                    return StatusCode(409, _api.Response(409, "Unable to delete sales order"));
                }

            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.ErrorResponse(ex));
            }


        }

        
        // [HttpGet("dummy")]
        // public ActionResult GetQtyDummy(int? Id)
        // {
        //     try
        //     {
        //         string sqlCommandText = "select * from Inv_SalesOrder where N_SalesOrderID=@p1";
        //         SortedList mParamList = new SortedList() { { "@p1", Id } };
        //         DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList);
        //         masterTable = _api.Format(masterTable, "master");

        //         string sqlCommandText2 = "select * from Inv_SalesOrderDetails where N_SalesOrderID=@p1";
        //         SortedList dParamList = new SortedList() { { "@p1", Id } };
        //         DataTable detailTable = dLayer.ExecuteDataTable(sqlCommandText2, dParamList);
        //         detailTable = _api.Format(detailTable, "details");

        //         if (detailTable.Rows.Count == 0) { return Ok(new { }); }
        //         DataSet dataSet = new DataSet();
        //         dataSet.Tables.Add(masterTable);
        //         dataSet.Tables.Add(detailTable);

        //         return Ok(dataSet);

        //     }
        //     catch (Exception e)
        //     {
        //         return StatusCode(403, _api.ErrorResponse(e));
        //     }
        // }


    }
}