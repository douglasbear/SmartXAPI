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
    [Route("purchaseinvoice")]
    [ApiController]
    public class Inv_PurchaseInvoice : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        public Inv_PurchaseInvoice(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("list")]
        public ActionResult GetPurchaseInvoiceList(int? nCompanyId, int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select N_PurchaseID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],InvoiceNetAmt from vw_InvPurchaseInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

            try
            {
                dt = dLayer.ExecuteDataTable(sqlCommandText, Params);
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return StatusCode(200, _api.Response(200, "No Results Found"));
                }
                else
                {
                    return Ok(dt);
                }
            }
            catch (Exception e)
            {
                return StatusCode(404, _api.Response(404, e.Message));
            }
        }
        [HttpGet("listdetails")]
        public ActionResult GetPurchaseInvoiceDetails(int nCompanyId, int nFnYearId, int nPurchaseNO, bool showAllBranch, int nBranchId)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable dtPurchaseInvoice = new DataTable();
            DataTable dtPurchaseInvoiceDetails = new DataTable();
            int N_PurchaseID=0;

            Params.Add("@CompanyID", nCompanyId);
            Params.Add("@YearID", nFnYearId);
            Params.Add("@PurchaseNo", nPurchaseNO);
            Params.Add("@TransType", "PURCHASE");
            Params.Add("@BranchID", nBranchId);
            string X_Crieteria = "";
            string X_MasterSql = "";
            string X_DetailsSql = "";

            if (showAllBranch == true)
            {
                X_Crieteria = "where N_CompanyID=@CompanyID and X_InvoiceNo=@PurchaseNo and N_FnYearID=@YearID and X_TransType=@TransType";
            }
            else
            {
                X_Crieteria = "where N_CompanyID=@CompanyID and X_InvoiceNo=@PurchaseNo and N_FnYearID=@YearID and X_TransType=@TransType  and  N_BranchId=@BranchID";
                
            }
            X_MasterSql = "select * from vw_Inv_PurchaseDisp " + X_Crieteria + "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                connection.Open();
                
                dtPurchaseInvoice = dLayer.ExecuteDataTable(X_MasterSql, Params,connection);
                if (dtPurchaseInvoice.Rows.Count == 0) { return Ok(new { }); }
                dtPurchaseInvoice = _api.Format(dtPurchaseInvoice, "Master");
                dtPurchaseInvoice.Rows[0]["N_PurchaseID"] = N_PurchaseID;
                dt.Tables.Add(dtPurchaseInvoice);


                //PURCHASE INVOICE DETAILS
                bool B_MRNVisible = myFunctions.CheckPermission(nCompanyId, 556, "Administrator", dLayer,connection);

                if (B_MRNVisible)
                {
                    if (showAllBranch == true)
                        X_DetailsSql = "Select vw_InvPurchaseDetails.*,Inv_PurchaseOrder.X_POrderNo,Inv_MRN.X_MRNNo,dbo.SP_Cost(vw_InvPurchaseDetails.N_ItemID,vw_InvPurchaseDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseDetails.N_ItemID,vw_InvPurchaseDetails.N_CompanyID) As N_UnitSPrice   from vw_InvPurchaseDetails Left Outer Join Inv_PurchaseOrder On vw_InvPurchaseDetails.N_POrderID=Inv_PurchaseOrder.N_POrderID Left Outer Join Inv_MRN On vw_InvPurchaseDetails.N_RsID=Inv_MRN.N_MRNID  Where vw_InvPurchaseDetails.N_CompanyID=@CompnayID and vw_InvPurchaseDetails.N_PurchaseID="+ N_PurchaseID;
                    else
                        X_DetailsSql = "Select vw_InvPurchaseDetails.*,Inv_PurchaseOrder.X_POrderNo,Inv_MRN.X_MRNNo,dbo.SP_Cost(vw_InvPurchaseDetails.N_ItemID,vw_InvPurchaseDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseDetails.N_ItemID,vw_InvPurchaseDetails.N_CompanyID) As N_UnitSPrice   from vw_InvPurchaseDetails Left Outer Join Inv_PurchaseOrder On vw_InvPurchaseDetails.N_POrderID=Inv_PurchaseOrder.N_POrderID Left Outer Join Inv_MRN On vw_InvPurchaseDetails.N_RsID=Inv_MRN.N_MRNID Where vw_InvPurchaseDetails.N_CompanyID=@CompnayID and vw_InvPurchaseDetails.N_PurchaseID="+ N_PurchaseID +" and vw_InvPurchaseDetails.N_BranchId=@BranchID";
                }
                else
                {
                    if (showAllBranch == true)
                        X_DetailsSql = "Select vw_InvPurchaseDetails.*,Inv_PurchaseOrder.X_POrderNo,dbo.SP_Cost(vw_InvPurchaseDetails.N_ItemID,vw_InvPurchaseDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseDetails.N_ItemID,vw_InvPurchaseDetails.N_CompanyID) As N_UnitSPrice   from vw_InvPurchaseDetails Left Outer Join Inv_PurchaseOrder On vw_InvPurchaseDetails.N_POrderID=Inv_PurchaseOrder.N_POrderID Where vw_InvPurchaseDetails.N_CompanyID=@CompanyID and vw_InvPurchaseDetails.N_PurchaseID="+N_PurchaseID;
                    else
                        X_DetailsSql = "Select vw_InvPurchaseDetails.*,Inv_PurchaseOrder.X_POrderNo,dbo.SP_Cost(vw_InvPurchaseDetails.N_ItemID,vw_InvPurchaseDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseDetails.N_ItemID,vw_InvPurchaseDetails.N_CompanyID) As N_UnitSPrice   from vw_InvPurchaseDetails Left Outer Join Inv_PurchaseOrder On vw_InvPurchaseDetails.N_POrderID=Inv_PurchaseOrder.N_POrderID Where vw_InvPurchaseDetails.N_CompanyID=@CompanyID and vw_InvPurchaseDetails.N_PurchaseID="+N_PurchaseID+" and vw_InvPurchaseDetails.N_BranchId=@BranchID ";
                }
                dtPurchaseInvoiceDetails = dLayer.ExecuteDataTable(X_DetailsSql,Params,connection);
                dtPurchaseInvoiceDetails = _api.Format(dtPurchaseInvoiceDetails, "Details");
                dt.Tables.Add(dtPurchaseInvoiceDetails);
                }

                return Ok(dt);

            }
            catch (Exception e)
            {
                return StatusCode(404, _api.Response(404, e.Message));
            }
        }

        //Save....
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {

            DataTable MasterTable;
            DataTable DetailTable;
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            SortedList Params = new SortedList();
            // Auto Gen
            string InvoiceNo = "";
            DataRow masterRow = MasterTable.Rows[0];
            var values = masterRow["x_InvoiceNo"].ToString();
            try
            {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

            if (values == "@Auto")
            {
                Params.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
                Params.Add("N_YearID", masterRow["n_FnYearId"].ToString());
                Params.Add("N_FormID", 80);
                Params.Add("N_BranchID", masterRow["n_BranchId"].ToString());
                InvoiceNo = dLayer.GetAutoNumber("Inv_Purchase", "x_InvoiceNo", Params);
                if (InvoiceNo == "") { return StatusCode(409, _api.Response(409, "Unable to generate Invoice Number")); }
                MasterTable.Rows[0]["x_InvoiceNo"] = InvoiceNo;
            }
           
                
                dLayer.setTransaction();
                int N_InvoiceId = dLayer.SaveData("Inv_Purchase", "N_PurchaseID", 0, MasterTable);
                if (N_InvoiceId <= 0)
                {
                    dLayer.rollBack();
                }
                for (int j = 0; j < DetailTable.Rows.Count; j++)
                {
                    DetailTable.Rows[j]["N_PurchaseID"] = N_InvoiceId;
                }
                int N_InvoiceDetailId = dLayer.SaveData("Inv_PurchaseDetails", "n_PurchaseDetailsID", 0, DetailTable);
                dLayer.commit();
            }
                return Ok("Data Saved");
            }
            catch (Exception ex)
            {
                dLayer.rollBack();
                return StatusCode(403, ex);
            }
        }
        //Delete....
        [HttpDelete()]
        public ActionResult DeleteData(int nPurchaseID)
        {
            int Results = 0;
            try
            {
                dLayer.setTransaction();
                Results = dLayer.DeleteData("Inv_Purchase", "n_PurchaseID", nPurchaseID, "");
                if (Results <= 0)
                {
                    dLayer.rollBack();
                    return StatusCode(409, _api.Response(409, "Unable to Delete PurchaseInvoice"));
                }
                else
                {
                    Results = dLayer.DeleteData("Inv_PurchaseDetails", "n_PurchaseID", nPurchaseID, "");
                }

                if (Results > 0)
                {
                    dLayer.commit();
                    return StatusCode(200, _api.Response(200, "Purchase Invoice deleted"));
                }
                else
                {
                    dLayer.rollBack();
                    return StatusCode(409, _api.Response(409, "Unable to delete Purchase Invoice"));
                }

            }
            catch (Exception ex)
            {
                return StatusCode(404, _api.Response(404, ex.Message));
            }


        }
        [HttpGet("dummy")]
        public ActionResult GetPurchaseInvoiceDummy(int? Id)
        {
            try
            {
                string sqlCommandText = "select * from Inv_Purchase where N_PurchaseId=@p1";
                SortedList mParamList = new SortedList() { { "@p1", Id } };
                DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList);
                masterTable = _api.Format(masterTable, "master");

                string sqlCommandText2 = "select * from Inv_PurchaseDetails where N_PurchaseId=@p1";
                SortedList dParamList = new SortedList() { { "@p1", Id } };
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