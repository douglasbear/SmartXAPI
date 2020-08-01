using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("salesinvoice")]
    [ApiController]
    public class Inv_SalesInvoice : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        public Inv_SalesInvoice(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("list")]
        public ActionResult GetSalesInvoiceList(int? nCompanyId, int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_InvSalesInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params);
                    dt = _api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        return Ok(dt);
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        [HttpGet("details")]
        public ActionResult GetSalesInvoiceDetails(int nCompanyId, int nFnYearId, int nBranchId, string xInvoiceNo)
        {

            try
            {
                using (SqlConnection Con = new SqlConnection(connectionString))
                {
                    SortedList QueryParamsList = new SortedList();
                    QueryParamsList.Add("@nCompanyID", nCompanyId);
                    QueryParamsList.Add("@nFnYearID", nFnYearId);
                    QueryParamsList.Add("@nBranchId", nBranchId);
                    QueryParamsList.Add("@xInvoiceNo", xInvoiceNo);

                    SortedList mParamsList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"X_ReceiptNo",xInvoiceNo},
                        {"X_TransType","SALES"},
                        {"N_FnYearID",nFnYearId},
                        {"N_BranchId",nBranchId}
                    };
                    DataTable masterTable = dLayer.ExecuteDataTablePro("SP_InvSales_Disp", mParamsList);
                    masterTable = _api.Format(masterTable, "Master");
                    if (masterTable.Rows.Count == 0) { return Ok(new { }); }
                    DataRow MasterRow = masterTable.Rows[0];


                    int N_TruckID = myFunctions.getIntVAL(MasterRow["N_TruckID"].ToString());
                    object objPlateNo = null;
                    if (N_TruckID > 0)
                    {
                        DataColumn ColPlateNo = new DataColumn("X_PlateNo", typeof(string));
                        ColPlateNo.DefaultValue = "";
                        masterTable.Columns.Add(ColPlateNo);
                        QueryParamsList.Add("@nTruckID", myFunctions.getIntVAL(masterTable.Rows[0]["N_TermsID"].ToString()));
                        objPlateNo = dLayer.ExecuteScalar("Select X_PlateNumber from Inv_TruckMaster where N_TruckID=@nTruckID and N_companyID=@nCompanyID", mParamsList, Con);
                        if (objPlateNo != null)
                            masterTable.Rows[0][ColPlateNo] = objPlateNo.ToString();

                    }

                    if (masterTable.Rows[0]["X_TandC"].ToString() == "")
                        masterTable.Rows[0]["X_TandC"] = myFunctions.RetunSettings("@nCompanyID", "64", "TermsandConditions", "X_Value", "N_UserCategoryID", "0", mParamsList, dLayer, Con);

                    int N_TermsID = myFunctions.getIntVAL(masterTable.Rows[0]["N_TermsID"].ToString());
                    if (N_TermsID > 0)
                    {
                        QueryParamsList.Add("@nTermsID", myFunctions.getIntVAL(masterTable.Rows[0]["N_TermsID"].ToString()));                        
                        DataColumn ColTerms = new DataColumn("X_Terms", typeof(string));
                        ColTerms.DefaultValue = "";
                        masterTable.Columns.Add(ColTerms);
                        masterTable.Rows[0][ColTerms] = myFunctions.ReturnValue("Inv_Terms", "X_Terms", "N_TermsID =@nTermsID and N_CompanyID =@nCompanyID", mParamsList, dLayer, Con));
                    }

                    if (myFunctions.getIntVAL(masterTable.Rows[0]["N_DeliveryNoteId"].ToString()) > 0)
                    {
                        QueryParamsList.Add("@nDeliveryNoteId", myFunctions.getIntVAL(masterTable.Rows[0]["N_DeliveryNoteId"].ToString()));

                    }

                    SortedList dParamList = new SortedList()
                    {
                        {"N_CompanyID",nCompanyId},
                        {"N_SalesID",masterTable.Rows[0]["n_SalesId"].ToString()}
                    };
                    DataTable detailTable = dLayer.ExecuteDataTablePro("SP_InvSalesDtls_Disp", dParamList);
                    detailTable = _api.Format(detailTable, "Details");
                    if (detailTable.Rows.Count == 0) { return Ok(new { }); }
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add(masterTable);
                    dataSet.Tables.Add(detailTable);

                    return Ok(dataSet);

                }
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
                // Auto Gen
                string InvoiceNo = "";
                DataRow masterRow = MasterTable.Rows[0];
                var values = masterRow["x_ReceiptNo"].ToString();

                if (values == "@Auto")
                {
                    Params.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
                    Params.Add("N_YearID", masterRow["n_FnYearId"].ToString());
                    Params.Add("N_FormID", 80);
                    Params.Add("N_BranchID", masterRow["n_BranchId"].ToString());
                    InvoiceNo = dLayer.GetAutoNumber("Inv_Sales", "x_ReceiptNo", Params);
                    if (InvoiceNo == "") { return StatusCode(409, _api.Response(409, "Unable to generate Invoice Number")); }
                    MasterTable.Rows[0]["x_ReceiptNo"] = InvoiceNo;
                }

                dLayer.setTransaction();
                int N_InvoiceId = dLayer.SaveData("Inv_Sales", "N_SalesId", 0, MasterTable);
                if (N_InvoiceId <= 0)
                {
                    dLayer.rollBack();
                }
                for (int j = 0; j < DetailTable.Rows.Count; j++)
                {
                    DetailTable.Rows[j]["N_SalesId"] = N_InvoiceId;
                }
                int N_InvoiceDetailId = dLayer.SaveData("Inv_SalesDetails", "n_SalesDetailsID", 0, DetailTable);
                dLayer.commit();
                return GetSalesInvoiceDetails(int.Parse(masterRow["n_CompanyId"].ToString()), int.Parse(masterRow["n_FnYearId"].ToString()), int.Parse(masterRow["n_BranchId"].ToString()), InvoiceNo);
            }
            catch (Exception ex)
            {
                dLayer.rollBack();
                return StatusCode(403, ex);
            }
        }
        //Delete....
        [HttpDelete()]
        public ActionResult DeleteData(int N_InvoiceID)
        {
            int Results = 0;
            try
            {
                dLayer.setTransaction();
                Results = dLayer.DeleteData("Inv_SalesInvoice", "n_InvoiceID", N_InvoiceID, "");
                if (Results <= 0)
                {
                    dLayer.rollBack();
                    return StatusCode(409, _api.Response(409, "Unable to delete sales Invoice"));
                }
                else
                {
                    dLayer.DeleteData("Inv_SalesInvoiceDetails", "n_InvoiceID", N_InvoiceID, "");
                }

                if (Results > 0)
                {
                    dLayer.commit();
                    return StatusCode(200, _api.Response(200, "Sales Invoice deleted"));
                }
                else
                {
                    dLayer.rollBack();
                    return StatusCode(409, _api.Response(409, "Unable to delete sales Invoice"));
                }

            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.ErrorResponse(ex));
            }


        }


        [HttpGet("dummy")]
        public ActionResult GetSalesInvoiceDummy(int? nSalesId)
        {
            try
            {
                string sqlCommandText = "select * from Inv_Sales where N_SalesId=@p1";
                SortedList mParamList = new SortedList() { { "@p1", nSalesId } };
                DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList);
                masterTable = _api.Format(masterTable, "master");

                string sqlCommandText2 = "select * from Inv_SalesDetails where N_SalesId=@p1";
                SortedList dParamList = new SortedList() { { "@p1", nSalesId } };
                DataTable detailTable = dLayer.ExecuteDataTable(sqlCommandText2, dParamList);
                detailTable = _api.Format(detailTable, "details");

                if (detailTable.Rows.Count == 0) { return Ok(new { }); }
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(masterTable);
                dataSet.Tables.Add(detailTable);

                return Ok(dataSet);

            }
            catch (Exception e)
            {
                return StatusCode(403, _api.ErrorResponse(e));
            }
        }

    }
}