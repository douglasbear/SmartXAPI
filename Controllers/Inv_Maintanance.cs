using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("maintanance")]
    [ApiController]
    public class Inv_Maintanance : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;

        public Inv_Maintanance(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1394;
        }
        private readonly string connectionString;
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
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nServiceID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ServiceID"].ToString());
                    int nUserID = myFunctions.GetUserID(User);
                    string X_ServiceCode = MasterTable.Rows[0]["X_ServiceCode"].ToString();
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    if (nServiceID > 0)
                    {
                        SortedList DeleteParams = new SortedList(){
                                    {"N_CompanyID",nCompanyID},
                                    {"N_UserID",nUserID},
                                    {"X_TransType","SERVICE MAINTANANCE"},
                                    {"X_SystemName","WebRequest"},
                                    {"N_VoucherID",nServiceID}};

                        dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                        // dLayer.DeleteData("Inv_ServiceMaster", "N_ServiceID", nServiceID, "", connection, transaction);
                        // dLayer.DeleteData("Inv_ServiceDetails", "N_ServiceID", nServiceID, "", connection, transaction);
                    }
                    DocNo = MasterRow["X_ServiceCode"].ToString();
                    if (X_ServiceCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", this.FormID);



                        while (true)
                        {

                            object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_ServiceMaster Where X_ServiceCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null) DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            break;
                        }
                        X_ServiceCode = DocNo;


                        if (X_ServiceCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate")); }
                        MasterTable.Rows[0]["X_ServiceCode"] = X_ServiceCode;


                    }

                    nServiceID = dLayer.SaveData("Inv_ServiceMaster", "N_ServiceID", MasterTable, connection, transaction);
                    if (nServiceID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }
                    for (int i = 0; i < DetailTable.Rows.Count; i++)
                    {
                        DetailTable.Rows[i]["N_ServiceID"] = nServiceID;
                    }
                    int nServiceDetailsID = dLayer.SaveData("Inv_ServiceDetails", "N_ServiceDetailsID", DetailTable, connection, transaction);
                    if (nServiceDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }
                    else
                    {
                        SortedList ParamsStock = new SortedList();
                        ParamsStock.Add("N_CompanyID", nCompanyID);
                        ParamsStock.Add("N_ServiceId", nServiceID);
                        ParamsStock.Add("N_UserID", nUserID);
                        ParamsStock.Add("X_SystemName", "ERP Cloud");
                        object posting = dLayer.ExecuteScalarPro("SP_Inv_ServiceStockOut", ParamsStock, connection, transaction);


                        SortedList PostingParam = new SortedList();
                        PostingParam.Add("N_CompanyID", nCompanyID);
                        PostingParam.Add("X_InventoryMode", "Service Maintanance");
                        PostingParam.Add("N_InternalID", nServiceID);
                        PostingParam.Add("N_UserID", nUserID);
                        PostingParam.Add("X_SystemName", "ERP Cloud");
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Sales_Posting", PostingParam, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }
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

        [HttpGet("list")]
        public ActionResult ProductionOrderList(int nFnYearId, int nFormID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "", xCondition = "";
            string Searchkey = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", nUserID);
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ( X_ServiceCode like '%" + xSearchkey + "%' or  D_EntryDate like '%" + xSearchkey + "%' or  X_CustomerName like '%" + xSearchkey + "%' or  X_Remarks like '%" + xSearchkey + "%' or  X_Status like '%" + xSearchkey + "%' ) ";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ServiceID desc";
            // xSortBy = " order by batch desc,D_TransDate desc";
            else
                xSortBy = " order by " + xSortBy;

            if (nFormID == 1394)
                xCondition = " and N_ServiceID in (select N_ServiceID from Vw_InvServiceDetails where N_companyID=@p1 and N_WarrantyType=371) and N_ServiceID in (select N_ServiceID from Vw_InvServiceDetails where N_companyID=@p1)";// and ( N_AssigneeID=@p3 or N_AssigneeID is null or N_UserID=@p3)
            else
                xCondition = " and N_ServiceID in (select N_ServiceID from Vw_InvServiceDetails where N_companyID=@p1 and N_WarrantyType=372) and N_ServiceID in (select N_ServiceID from Vw_InvServiceDetails where N_companyID=@p1) "; //and ( N_AssigneeID=@p3 or N_AssigneeID is null or N_UserID=@p3)

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ")  *,Case isNull(N_Status,0) When 1 Then 'Completed' When 0 Then 'Ongoing' End as X_Status  from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2  " + xCondition + " " + Searchkey;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") *,Case isNull(N_Status,0) When 1 Then 'Completed' When 0 Then 'Ongoing' End as X_Status from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2 " + xCondition + " " + Searchkey + "and N_ServiceID not in (select top(" + Count + ") N_ServiceID from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2 " + xCondition + " ) " + Searchkey;


            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText + xSortBy, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2 " + xCondition + " " + xCondition + " " + Searchkey;
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
        [HttpGet("details")]
        public ActionResult ServiceDetails(string xServiceCode, int nWarrantyID)
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
                    string Mastersql = "";
                    string DetailSql = "";
                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@xServiceCode", xServiceCode);
                    int WarrantyID = 0;
                    if (xServiceCode != null && xServiceCode != null)
                    {
                       
                        Mastersql = "select * from Vw_InvService where N_CompanyId=@nCompanyID and X_ServiceCode=@xServiceCode  ";
                        object n_WarrantyID = dLayer.ExecuteScalar("select N_WarrantyID from Vw_InvService where N_CompanyId=@nCompanyID and X_ServiceCode=@xServiceCode", Params, connection);
                        if (n_WarrantyID != null)
                        {
                            if (myFunctions.getIntVAL(n_WarrantyID.ToString()) > 0)
                                WarrantyID = (myFunctions.getIntVAL(n_WarrantyID.ToString()));
                        }

                    }
                    if (nWarrantyID > 0)
                    {
                        Mastersql = "select * from Vw_WarrantyToMaintananceMaster where N_CompanyId=@nCompanyID and N_WarrantyID=" + nWarrantyID + "  ";
                        MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                        MasterTable = _api.Format(MasterTable, "Master");
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_WarrantyNo", typeof(string), "");
                        MasterTable.Rows[0]["x_WarrantyNo"] = MasterTable.Rows[0]["X_WarrantyCode"].ToString();

                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable.AcceptChanges();
                        MasterTable = _api.Format(MasterTable, "Master");


                        DetailSql = "select * from Vw_WarrantyToMaintananceDetails where N_CompanyId=@nCompanyID and  N_WarrantyID=" + nWarrantyID + " ";
                        DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                        DetailTable = _api.Format(DetailTable, "Details");



                        dt.Tables.Add(MasterTable);
                        dt.Tables.Add(DetailTable);
                        return Ok(_api.Success(dt));

                    }

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int ServiceID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ServiceID"].ToString());
                    int salesID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_SalesID"].ToString());
                    object periodTo;
                    object warrantyNo;
                    object N_salesWID = 0;
                    object deviceDescription = null;
                    object serialNo = null;
                   
                    if (WarrantyID > 0)
                    {
                        periodTo = dLayer.ExecuteScalar("select D_PeriodTo as D_PeriodTo from Inv_WarrantyContract where N_CompanyId=@nCompanyID  and N_WarrantyID=" + WarrantyID + "", Params, connection);
                        warrantyNo = dLayer.ExecuteScalar("select X_WarrantyCode as x_WarrantyNo from Inv_WarrantyContract where N_CompanyId=@nCompanyID  and N_WarrantyID=" + WarrantyID + "", Params, connection);
                        N_salesWID = dLayer.ExecuteScalar("select N_SalesID as x_WarrantyNo from Inv_WarrantyContract where N_CompanyId=@nCompanyID  and N_WarrantyID=" + WarrantyID + "", Params, connection);

                    }
                    else
                    {
                        periodTo = dLayer.ExecuteScalar("select D_PeriodTo as D_PeriodTo from Inv_WarrantyContract where N_CompanyId=@nCompanyID  and N_SalesID=" + salesID + "", Params, connection);
                        warrantyNo = dLayer.ExecuteScalar("select X_WarrantyCode as x_WarrantyNo from Inv_WarrantyContract where N_CompanyId=@nCompanyID  and N_SalesID=" + salesID + "", Params, connection);
                    }// MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "D_PeriodTo", typeof(string), "");

                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "x_WarrantyNo", typeof(string), "");
                    if (periodTo != null)
                        MasterTable.Rows[0]["D_PeriodTo"] = periodTo.ToString();
                    if (warrantyNo != null)
                        MasterTable.Rows[0]["x_WarrantyNo"] = warrantyNo.ToString();
                    if (myFunctions.getIntVAL(N_salesWID.ToString()) > 0)
                    {
                        deviceDescription = dLayer.ExecuteScalar("select X_SerialNo  from Inv_SalesAddInfo_ServicePackages where N_CompanyId=@nCompanyID  and N_InvoiceID=" + myFunctions.getIntVAL(N_salesWID.ToString()) + "", Params, connection);
                        serialNo = dLayer.ExecuteScalar("select X_Description  from Inv_SalesAddInfo_ServicePackages where N_CompanyId=@nCompanyID  and N_InvoiceID=" + myFunctions.getIntVAL(N_salesWID.ToString()) + "", Params, connection);
                    }
                    if (deviceDescription != null)
                        MasterTable.Rows[0]["X_DeviceDescription"] = serialNo.ToString();
                    if (serialNo != null)
                        MasterTable.Rows[0]["X_SerialNo"] = deviceDescription.ToString();



                    MasterTable.AcceptChanges();
                    Params.Add("@nServiceID", ServiceID);
                    MasterTable = _api.Format(MasterTable, "Master");
                    //Detail
                    DetailSql = "select * from Vw_InvServiceDetails where N_CompanyId=@nCompanyID and N_ServiceID=@nServiceID ";
                     object objSalesInvoice = dLayer.ExecuteScalar("select x_ReceiptNo from inv_sales where N_ServiceID =@nServiceID and N_CompanyID=@nCompanyID", Params, connection);
                      myFunctions.AddNewColumnToDataTable(MasterTable, "x_SalesReceiptNo", typeof(string), objSalesInvoice);

                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");

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
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nServiceID, int nFnyearID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Params.Add("@nServiceID", nServiceID);
                    // Params.Add("@nFnyearID", nFnyearID);
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nUserID = myFunctions.GetUserID(User);

                    SortedList DeleteParams = new SortedList(){
                                    {"N_CompanyID",nCompanyID},
                                    {"N_UserID",nUserID},
                                    {"X_TransType","SERVICE MAINTANANCE"},
                                    {"X_SystemName","WebRequest"},
                                    {"N_VoucherID",nServiceID}};

                    Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);

                    // Results = dLayer.DeleteData("Inv_ServiceDetails", "N_ServiceID", nServiceID, "", connection);
                    // dLayer.ExecuteNonQuery("Delete from Inv_ServiceMaster Where N_ServiceID=@nServiceID and N_FnYearID=@nFnyearID", Params, connection);
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete"));
                    }

                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
        [HttpGet("UpdateStatus")]
        public ActionResult UpdateStatus(string remarks, int nStatus, int nServiceID, string xStatus, string dClosingDate, int nClosedUserID,int statusID , string xServiceDetailsID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    SortedList Params = new SortedList();
                     DataTable DetailTable = new DataTable();
                    Params.Add("@nCompanyID", nCompanyID);
                    bool flag=true;
                    //DateTime dCloseDate = Convert.ToDateTime(dClosingDate.ToString());
                    string xCriteria="";
                    if(xStatus =="update")
                    {
                     xCriteria=xServiceDetailsID;
                    }
                    else
                    {
                     xCriteria=" select N_ServiceDetailsID from Inv_ServiceDetails where N_CompanyID=@nCompanyID and N_AssigneeID="+nClosedUserID+"" ;
                    }
                     dLayer.ExecuteNonQuery("Update Inv_ServiceDetails set N_StatusID = " + statusID + "  where N_CompanyID = @nCompanyID and N_ServiceDetailsID in  (" + xCriteria + ")", Params, connection);
                     string detailSql="select * from Inv_ServiceDetails where n_ServiceID ="+nServiceID+" and N_CompanyID="+nCompanyID+" ";
                      DetailTable = dLayer.ExecuteDataTable(detailSql, Params, connection);

                  foreach (DataRow row in DetailTable.Rows)
                    {
                        if(row["N_StatusID"].ToString()== "2" || row["N_StatusID"].ToString() == "0")
                        {
                            flag=false;

                        }

                    }
                    

                    if(flag)
                    {
                    dLayer.ExecuteNonQuery("Update Inv_ServiceMaster set N_Status = " + nStatus + " , D_ClosingDate='" + dClosingDate + "' , N_ClosedUserID='" + nClosedUserID + "' where N_CompanyID = @nCompanyID and N_ServiceID = " + nServiceID + "", Params, connection);
                    if (remarks != "" || remarks != null)
                    {
                        dLayer.ExecuteNonQuery("Update Inv_ServiceMaster set X_ClosedRemarks ='" + remarks + "', D_ClosingDate='" + dClosingDate + "', N_ClosedUserID='" + nClosedUserID + "' where N_CompanyID = @nCompanyID and N_ServiceID = " + nServiceID + "", Params, connection);
                    }
                    return Ok(_api.Success("Closed"));
                    }
                    else{
                          return Ok(_api.Success("updated"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
    }
}
