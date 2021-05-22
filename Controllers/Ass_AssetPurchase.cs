
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
    [Route("assetPurchase")]
    [ApiController]
    public class Ass_AssetPurchase : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int N_FormID;
        public Ass_AssetPurchase(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 129;
        }


        [HttpGet("list")]
        public ActionResult GetAssetPurchaseList(int? nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            DataTable CountTable = new DataTable();
            SortedList Params = new SortedList();
            DataSet dataSet = new DataSet();
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([Invoice No] like '%" + xSearchkey + "%' or Vendor like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_AssetInventoryID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "invoiceNo":
                        xSortBy = "N_AssetInventoryID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }
            int Count = (nPage - 1) * nSizeperpage;
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") N_AssetInventoryID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],NetAmount,X_Description from vw_InvAssetInventoryInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_AssetInventoryID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],NetAmount,X_Description from vw_InvAssetInventoryInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and N_AssetInventoryID not in (select top(" + Count + ") N_AssetInventoryID from vw_InvAssetInventoryInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + xSortBy + " ) " + xSortBy;

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count,sum(Cast(REPLACE(NetAmount,',','') as Numeric(10,2)) ) as TotalAmount from vw_InvAssetInventoryInvoiceNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    string TotalSum = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                        TotalSum = drow["TotalAmount"].ToString();
                    }
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    OutPut.Add("TotalSum", TotalSum);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(OutPut));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
  [HttpGet("categorylist")]
        public ActionResult ListCategory(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nFnYearID",nFnYearID);
            string sqlCommandText="Select * from vw_InvAssetCategory_Disp Where N_CompanyID=@nCompanyID and N_FnyearID=@nFnYearID";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
                return Ok(_api.Error(e));
            }
        }


        [HttpGet("ponoList")]
        public ActionResult ListPonumber(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nFnYearID",nFnYearID);
            string sqlCommandText="Select * from vw_Ass_POSearch Where N_POType=266 and N_Processed=0 and B_IsSaveDraft <> 1";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
                return Ok(_api.Error(e));
            }
        }
 
        
        [HttpGet("assetList")]
        public ActionResult ListAssetName(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nFnYearID",nFnYearID);
            string sqlCommandText="Select * from Vw_AssetDashboard Where N_CompanyID=@nCompanyID and N_FnyearID=@nFnYearID";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
                return Ok(_api.Error(e));
            }
        }

        [HttpPost("Save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    // Auto Gen
                                try{
                 using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                    connection.Open();
                    SqlTransaction transaction=connection.BeginTransaction();
                    string ReturnNo="";
                    int N_CreditNoteID=myFunctions.getIntVAL(MasterTable.Rows[0]["N_CreditNoteId"].ToString());
                    int N_UserID=myFunctions.GetUserID(User);
                    double N_TotalReceived=myFunctions.getVAL(MasterTable.Rows[0]["n_TotalReceived"].ToString());
                    MasterTable.Rows[0]["n_TotalReceived"] = N_TotalReceived;
                    double N_TotalReceivedF=myFunctions.getVAL(MasterTable.Rows[0]["n_TotalReceivedF"].ToString());
                    MasterTable.Rows[0]["n_TotalReceivedF"] = N_TotalReceivedF;
                    var values = MasterTable.Rows[0]["X_CreditNoteNo"].ToString();
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID",MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID",80);
                        Params.Add("N_BranchID",MasterTable.Rows[0]["n_BranchId"].ToString());
                        ReturnNo =  dLayer.GetAutoNumber("Inv_PurchaseReturnMaster","X_CreditNoteNo", Params,connection,transaction);
                        if(ReturnNo==""){transaction.Rollback(); return Ok(_api.Warning("Unable to generate Quotation Number"));}
                        MasterTable.Rows[0]["X_CreditNoteNo"] = ReturnNo;
                    }

                    if(N_CreditNoteID>0)
                    {
                        SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString()},
                                {"X_TransType","PURCHASE RETURN"},
                                {"N_VoucherID",N_CreditNoteID}};
                         try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_PurchaseAccounts", DeleteParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(ex));
                        }
                    }

                    N_CreditNoteID=dLayer.SaveData("Inv_PurchaseReturnMaster","N_CreditNoteID",MasterTable,connection,transaction);                    
                    if(N_CreditNoteID<=0){
                        transaction.Rollback();
                        }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                        {
                            DetailTable.Rows[j]["N_CreditNoteID"]=N_CreditNoteID;
                        }
                    int N_QuotationDetailId=dLayer.SaveData("Inv_PurchaseReturnDetails","n_CreditNoteDetailsID",DetailTable,connection,transaction);                    
                    transaction.Commit();

                         SortedList InsParams = new SortedList(){
                                {"N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString()},
                                {"N_CreditNoteID",N_CreditNoteID}};    
                        dLayer.ExecuteNonQueryPro("[SP_PurchaseReturn_Ins]", InsParams, connection, transaction);

                    SortedList PostParams = new SortedList(){
                                {"N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString()},
                                {"X_InventoryMode","PURCHASE RETURN"},
                                {"N_InternalID",N_CreditNoteID},
                                {"N_UserID",N_UserID}};
                    dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Purchase_Posting", PostParams, connection, transaction);

                    SortedList Result = new SortedList();
                    Result.Add("n_PurchaseReturnID",N_CreditNoteID);
                    Result.Add("x_PurchaseReturnNo",ReturnNo);
                    return Ok(_api.Success(Result,"Purchase Return Saved"));
                    }
                }
                catch (Exception ex)
                {
                    return Ok(_api.Error(ex));
                }
        }

       [HttpDelete("delete")]
        public ActionResult DeleteData(int nBranchID)
        {
            int Results = 0;

            SortedList Params = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    Params.Add("@nBranchID", nBranchID);
                    object count = dLayer.ExecuteScalar("select count(*) as N_Count from Vw_Acc_BranchMaster_Disp where N_BranchID=@nBranchID and N_CompanyID=N_CompanyID", Params, connection);
                    int N_Count = myFunctions.getIntVAL(count.ToString());
                    if (N_Count <= 0)
                    {
                        Results = dLayer.DeleteData("Acc_BranchMaster", "N_BranchID", nBranchID, "", connection);

                        Results = dLayer.DeleteData("Inv_Location", "N_BranchID", nBranchID, "B_IsDefault=1", connection);


                    }
                    else
                    {
                        return Ok(_api.Success("unable to delete this branch"));
                    }
                    if (Results >= 0)
                    {
                        return Ok(_api.Success("Branch deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete Branch"));
                    }


                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
   } 
  
 }