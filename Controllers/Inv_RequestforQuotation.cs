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

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("Requestforquotation")]
    [ApiController]
    public class Inv_RequestforQuotation : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int FormID = 618;
        public Inv_RequestforQuotation(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
           // N_FormID = 64;
        }

        [HttpGet("list")]
        public ActionResult GetSalesInvoiceList(int nFnYearId, int nPage, bool bAllBranchData, int nBranchID,int N_RequestId, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int nCompanyId = myFunctions.GetCompanyID(User);
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";
                    string X_TransType = "";
                    
                 
                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by D_RequestDate  desc";
                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "invoiceNo":
                                xSortBy = "D_RequestDate " + xSortBy.Split(" ")[1];
                                break;
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }

                    if (myCompanyID._B_AllBranchData == true)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_RequestQuotation_Disp where N_CompanyID=@p1 and N_FnYearID=@p2  " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_RequestQuotation_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 and n_branchid= "   + xSearchkey + xSortBy + " ) " + xSortBy;

                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearId);
                    Params.Add("@p3",nBranchID);

                    SortedList OutPut = new SortedList();

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
                return Ok(_api.Error(e));
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
                    DataTable MultiVendorTable;
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    MultiVendorTable = ds.Tables["vendorDetails"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nQuotationID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_QuotationID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    string X_QuotationNo = MasterTable.Rows[0]["x_QuotationNo"].ToString();
                    int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                    int nFormID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FormID"].ToString());

                    if (MasterTable.Columns.Contains("nFormID"))
                        MasterTable.Columns.Remove("nFormID");
                    MasterTable.AcceptChanges();

                    if (nFormID == 618)
                    {
                        if (nQuotationID > 0)
                        {
                            dLayer.DeleteData("Inv_RFQVendorList", "N_QuotationID", nQuotationID, "N_CompanyID = " + nCompanyID, connection, transaction);
                            dLayer.DeleteData("Inv_VendorRequestDetails", "N_QuotationID", nQuotationID, "N_CompanyID = " + nCompanyID, connection, transaction);
                            dLayer.DeleteData("Inv_VendorRequest", "N_QuotationID", nQuotationID, "N_CompanyID = " + nCompanyID, connection, transaction);
                        }
                        DocNo = MasterRow["x_QuotationNo"].ToString();
                        if (X_QuotationNo == "@Auto")
                        {
                            Params.Add("N_CompanyID", nCompanyID);
                            Params.Add("N_FormID", nFormID);
                            Params.Add("N_YearID", nFnYearID);

                            while (true)
                            {
                                DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                                object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_VendorRequest Where X_ReceiptNo ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                                if (N_Result == null)
                                    break;
                            }
                            X_QuotationNo = DocNo;

                            if (X_QuotationNo == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate")); }
                            MasterTable.Rows[0]["x_QuotationNo"] = X_QuotationNo;
                        }

                        string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_AcYearID=" + nFnYearID + " and X_ReceiptNo='" + X_QuotationNo + "'";
                        string X_Criteria = "N_CompanyID=" + nCompanyID + " and N_AcYearID=" + nFnYearID;
                        nQuotationID = dLayer.SaveData("Inv_VendorRequest", "N_QuotationID", DupCriteria, X_Criteria, MasterTable, connection, transaction);
                        if (nQuotationID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable To Save"));
                        }
                        for (int j = 0; j < DetailTable.Rows.Count; j++)
                        {
                            DetailTable.Rows[j]["n_QuotationID"] = nQuotationID;
                        }
                    
                        for (int j = 0; j < DetailTable.Rows.Count; j++)
                        {

                            int n_QuotationDetailsID = dLayer.SaveDataWithIndex("Inv_VendorRequestDetails", "n_QuotationDetailsID", "", "", j, DetailTable, connection, transaction);
                            if (n_QuotationDetailsID <= 0)
                            {
                                transaction.Rollback();
                                return Ok("Unable to save ");
                            }

                            if (MultiVendorTable.Rows.Count > 0)
                            {
                                for (int k = 0; k < MultiVendorTable.Rows.Count; k++)
                                {
                                    if(myFunctions.getIntVAL(MultiVendorTable.Rows[k]["n_RowID"].ToString())==j)
                                    {
                                        MultiVendorTable.Rows[k]["N_QuotationID"] = nQuotationID;
                                        MultiVendorTable.Rows[k]["N_QuotationDetailsID"] = n_QuotationDetailsID;
                                    }
                                }
                            }
                        }
                        if (MultiVendorTable.Rows.Count > 0)
                        {
                            if (MultiVendorTable.Columns.Contains("n_RowID"))
                                MultiVendorTable.Columns.Remove("n_RowID");
                            MultiVendorTable.AcceptChanges();

                            int N_VendorListID = dLayer.SaveData("Inv_RFQVendorList", "N_VendorListID", DupCriteria, X_Criteria, MultiVendorTable, connection, transaction);
                        }
                        transaction.Commit();
                        return Ok(_api.Success("RFQ Saved"));
                    }
                    else
                    {
                        return Ok(_api.Success("RFQ Saved"));
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

           
        [HttpGet("details")]
        public ActionResult GetDetails(string  X_QuotationNo,int nFnYearID,int nBranchID, bool bShowAllBranchData,int nFormID)
        {
            DataTable Master = new DataTable();
            DataTable Detail = new DataTable();
            DataTable VendorListMaster = new DataTable();
            DataTable VendorListDetails = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@X_QuotationNo", X_QuotationNo);
            QueryParams.Add("@nBranchID", nBranchID);
            QueryParams.Add("@nFnYearID", nFnYearID);
            string Condition = "";
            string _sqlQuery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if(nFormID==168)
                    {
                        if (bShowAllBranchData == true)
                            Condition = "n_Companyid=@nCompanyID and X_QuotationNo =@X_QuotationNo";
                        else
                            Condition = "n_Companyid=@nCompanyID and X_QuotationNo =@X_QuotationNo and N_BranchID=@nBranchID";


                        _sqlQuery = "Select * from Inv_VendorRequest Where " + Condition + "";

                        Master = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        Master = _api.Format(Master, "master");

                        if (Master.Rows.Count == 0)
                        {
                            return Ok(_api.Notice("No Results Found"));
                        }
                        else
                        {
                            QueryParams.Add("@N_QuotationID", Master.Rows[0]["N_QuotationID"].ToString());

                            ds.Tables.Add(Master);

                            _sqlQuery = "Select *,dbo.SP_Cost(vw_InvVendorRequestDetails.N_ItemID,vw_InvVendorRequestDetails.N_CompanyID,'') As N_LPrice,dbo.SP_SellingPrice(vw_InvVendorRequestDetails.N_ItemID,vw_InvVendorRequestDetails.N_CompanyID) As N_UnitSPrice  from vw_InvVendorRequestDetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_QuotationID=@N_QuotationID";
                            Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                            Detail = _api.Format(Detail, "details");
                            if (Detail.Rows.Count == 0)
                            {
                                return Ok(_api.Notice("No Results Found"));
                            }
                            ds.Tables.Add(Detail);

                           // VendorListDetails=GetVendorListTable(myFunctions.getIntVAL(Master.Rows[0]["N_QuotationID"].ToString()),0,companyid,dLayer,connection);
                            VendorListDetails = _api.Format(VendorListDetails, "vendorList");
                            ds.Tables.Add(VendorListDetails);

                            return Ok(_api.Success(ds));
                        }
                    }
                    else
                    {
                        return Ok(_api.Success(ds));
                    }
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        // public DataTable GetVendorListTable(int nQuotationID,int nVendorID,int N_CompanyID, IDataAccessLayer dLayer, SqlConnection connection)
        // {
        //     DataTable VendorListDetails = new DataTable();
        //     string sqlCommand="";

        //     SortedList Params = new SortedList();
        //     Params.Add("@nQuotationID", nQuotationID);
        //     Params.Add("@nCompanyID", N_CompanyID);
        //     Params.Add("@nVendorID", nVendorID);

        //     if(nVendorID!=0)
        //         sqlCommand = "Select * from vw_RFQVendorListDetails Where N_CompanyID=@nCompanyID and N_QuotationID=@nQuotationID and N_VendorID=@nVendorID";
        //     else
        //         sqlCommand = "Select * from vw_RFQVendorListDetails Where N_CompanyID=@nCompanyID and N_QuotationID=@nQuotationID";

        //     VendorListDetails = dLayer.ExecuteDataTable(sqlCommand, Params, connection);

        //     VendorListDetails = _api.Format(VendorListDetails, "vendorList");


        //     return VendorListDetails;
        // }

    }
}
