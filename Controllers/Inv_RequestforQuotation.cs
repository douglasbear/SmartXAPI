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
        public ActionResult GetRFQList(int nFormID,int nFnYearId, int nPage, bool bAllBranchData, int nBranchID, int nSizeperpage, string xSearchkey, string xSortBy)
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
                    string sqlCondition = "";
                    
                    if(nFormID==618)
                    {
                        if (xSortBy == null || xSortBy.Trim() == "")
                            xSortBy = " order by [Quotation No]  desc";
                        else
                        {
                            switch (xSortBy.Split(" ")[0])
                            {
                                case "invoiceNo":
                                    xSortBy = "[Quotation No] " + xSortBy.Split(" ")[1];
                                    break;
                                default: break;
                            }
                            xSortBy = " order by " + xSortBy;
                        }

                        if (bAllBranchData == true)
                            sqlCondition="N_CompanyID=@p1 and N_FnYearID=@p2";
                        else
                            sqlCondition="N_CompanyID=@p1 and N_FnYearID=@p2 and n_branchid=@p3";

                        if (Count == 0)
                            sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvVendorRequestNo_Search where "+sqlCondition+"  " + Searchkey + " " + xSortBy;
                        else
                            sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvVendorRequestNo_Search where "+sqlCondition+" "   + xSearchkey + " and N_QuotationId not in (select top(" + Count + ") N_QuotationId from vw_InvVendorRequestNo_Search where " + sqlCondition + " " + xSortBy + " ) " + xSortBy;

                        Params.Add("@p1", nCompanyId);
                        Params.Add("@p2", nFnYearId);
                        Params.Add("@p3",nBranchID);
                    
                        dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                        sqlCommandCount = "select count(*) as N_Count from vw_InvVendorRequestNo_Search where " + sqlCondition + " " + Searchkey + "";
                    }
                    else
                    {
                        if (xSortBy == null || xSortBy.Trim() == "")
                            xSortBy = " order by [Quotation No]  desc";
                        else
                        {
                            switch (xSortBy.Split(" ")[0])
                            {
                                case "invoiceNo":
                                    xSortBy = "[Quotation No] " + xSortBy.Split(" ")[1];
                                    break;
                                default: break;
                            }
                            xSortBy = " order by " + xSortBy;
                        }

                        if (Count == 0)
                            sqlCommandText = "select top(" + nSizeperpage + ") * from vw_RFQVendorListMaster where N_CompanyID=@p1 " + Searchkey + " " + xSortBy;
                        else
                            sqlCommandText = "select top(" + nSizeperpage + ") * from vw_RFQVendorListMaster where N_CompanyID=@p1 "   + xSearchkey + " and N_VendorListMasterID not in (select top(" + Count + ") N_VendorListMasterID from vw_RFQVendorListMaster where N_CompanyID=@p1 " + xSortBy + " ) " + xSortBy;

                        Params.Add("@p1", nCompanyId);
                    
                        dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                        sqlCommandCount = "select count(*) as N_Count from vw_InvVendorRequestNo_Search where N_CompanyID=@p1 " + Searchkey + "";
                    }
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    SortedList OutPut = new SortedList();
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
                return Ok(_api.Error(User,e));
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
                    DataTable GeneralTable;
                    DataTable VendorMasterTable;
                    string DocNo = "";
                    GeneralTable = ds.Tables["general"];
                    int nFormID = myFunctions.getIntVAL(GeneralTable.Rows[0]["n_FormID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(GeneralTable.Rows[0]["n_FnYearID"].ToString());
                    if (nFormID == 618)
                    {
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
                                object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_VendorRequest Where x_QuotationNo ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                                if (N_Result == null)
                                    break;
                            }
                            X_QuotationNo = DocNo;

                            if (X_QuotationNo == "") { transaction.Rollback(); return Ok(_api.Error(User,"Unable to generate")); }
                            MasterTable.Rows[0]["x_QuotationNo"] = X_QuotationNo;
                        }

                        string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and x_QuotationNo='" + X_QuotationNo + "'";
                        string X_Criteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                        nQuotationID = dLayer.SaveData("Inv_VendorRequest", "N_QuotationID", DupCriteria, X_Criteria, MasterTable, connection, transaction);
                        if (nQuotationID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,"Unable To Save"));
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

                            int N_VendorListID = dLayer.SaveData("Inv_RFQVendorList", "N_VendorListID", "", "", MultiVendorTable, connection, transaction);
                        }
                        transaction.Commit();
                        return Ok(_api.Success("RFQ Saved"));
                    }
                    else
                    {
                        VendorMasterTable = ds.Tables["vendorMaster"];
                        MultiVendorTable = ds.Tables["vendorDetails"];
                        DataRow VendorMasterTableRow = VendorMasterTable.Rows[0];
                        SortedList Params = new SortedList();
                        int nCompanyID = myFunctions.getIntVAL(VendorMasterTable.Rows[0]["n_CompanyID"].ToString());
                        int nQuotationID = myFunctions.getIntVAL(VendorMasterTable.Rows[0]["n_QuotationID"].ToString());
                        int N_VendorListMasterID = myFunctions.getIntVAL(VendorMasterTable.Rows[0]["N_VendorListMasterID"].ToString());
                        string X_InwardsCode = VendorMasterTableRow["X_InwardsCode"].ToString();

                        if (N_VendorListMasterID > 0)
                        {
                            dLayer.DeleteData("Inv_RFQVendorListMaster", "N_VendorListMasterID", N_VendorListMasterID, "N_CompanyID = " + nCompanyID, connection, transaction);
                        }
                        DocNo = VendorMasterTableRow["x_QuotationNo"].ToString();
                        if (X_InwardsCode == "@Auto")
                        {
                            Params.Add("N_CompanyID", nCompanyID);
                            Params.Add("N_FormID", nFormID);
                            Params.Add("N_YearID", N_FnYearID);

                            while (true)
                            {
                                DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                                object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_RFQVendorListMaster Where X_InwardsCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                                if (N_Result == null)
                                    break;
                            }
                            X_InwardsCode = DocNo;

                            if (X_InwardsCode == "") { transaction.Rollback(); return Ok(_api.Error(User,"Unable to generate")); }
                            VendorMasterTable.Rows[0]["X_InwardsCode"] = X_InwardsCode;
                        }

                        string DupCriteria = "N_CompanyID=" + nCompanyID + " and X_InwardsCode='" + X_InwardsCode + "'";
                        string X_Criteria = "N_CompanyID=" + nCompanyID ;
                        N_VendorListMasterID = dLayer.SaveData("Inv_RFQVendorListMaster", "N_VendorListMasterID", DupCriteria, X_Criteria, VendorMasterTable, connection, transaction);
                        if (N_VendorListMasterID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,"Unable To Save"));
                        }

                        for (int k = 0; k < MultiVendorTable.Rows.Count; k++)
                        {
                            SortedList QueryParams = new SortedList();
                            QueryParams.Add("@N_CompanyID", nCompanyID);
                            QueryParams.Add("@N_VendorListID", myFunctions.getIntVAL(MultiVendorTable.Rows[0]["N_VendorListID"].ToString()));
                            QueryParams.Add("@N_Price", myFunctions.getVAL(MultiVendorTable.Rows[0]["N_Price"].ToString()));
                            QueryParams.Add("@X_Remarks", MultiVendorTable.Rows[0]["X_Remarks"].ToString());

                            dLayer.ExecuteNonQuery("update Inv_RFQVendorList set N_Price=@N_Price,X_Remarks=@X_Remarks where N_CompanyID=@N_CompanyID and N_VendorListID=@N_VendorListID", QueryParams, connection, transaction);
                        }
                        transaction.Commit();
                        return Ok(_api.Success("Vendor Inwards Saved"));
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

           
        [HttpGet("details")]
        public ActionResult GetDetails(string  X_QuotationNo,int nFnYearID,int nBranchID, bool bShowAllBranchData,int nFormID,int nPRSID)
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

                    if(nFormID==618)
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

                                VendorListDetails=GetVendorListTable(myFunctions.getIntVAL(Master.Rows[0]["N_QuotationID"].ToString()),0,companyid,dLayer,connection);
                                VendorListDetails = _api.Format(VendorListDetails, "vendorList");
                                ds.Tables.Add(VendorListDetails);

                            return Ok(_api.Success(ds));
                        }
                    }
                    else
                    {
                        _sqlQuery = "Select * from vw_RFQVendorListMaster Where N_CompanyID=@nCompanyID and X_InwardsCode=@X_QuotationNo";

                        VendorListMaster = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        VendorListMaster = _api.Format(VendorListMaster, "vendorMaster");

                        if (Master.Rows.Count == 0)
                        {
                            return Ok(_api.Notice("No Results Found"));
                        }

                        ds.Tables.Add(VendorListMaster);

                        VendorListDetails=GetVendorListTable(myFunctions.getIntVAL(Master.Rows[0]["N_QuotationID"].ToString()),0,companyid,dLayer,connection);
                        VendorListDetails = _api.Format(VendorListDetails, "vendorList");
                        ds.Tables.Add(VendorListDetails);

                        return Ok(_api.Success(ds));
                    }
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
          
        [HttpGet("prsList")]
        public ActionResult GetPRSList(bool bAllBranchData, int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nBranchID", nBranchID);

            string sqlCommandText = "";
            if (bAllBranchData == true)
                sqlCommandText = "Select *  from vw_PrsPoMrn_GroupBy where N_CompanyID=@nCompanyID and N_Type=2";
            else
                sqlCommandText = "Select *  from vw_PrsPoMrn_GroupBy where N_CompanyID=@nCompanyID and N_Type=2 and N_BranchID=@nBranchID";
 

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
                return Ok(_api.Error(User,e));
            }
        }

        [HttpGet("prsDetails")]
        public ActionResult GetPRSDetails(string xPRSNo)
        {
            DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            string[] temp = xPRSNo.Split(',');
            string X_PRSID="";
            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);

            string _sqlQuery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                   // SqlTransaction transaction = connection.BeginTransaction();

                    for (int i = 0; i < temp.Length; i++)
                    {
                        int  N_PRSID=0;
                        object  PRSID =dLayer.ExecuteScalar("Select N_PRSID from Inv_PRS Where Inv_PRS.N_CompanyID=" + companyid + " and X_PRSNo='" + temp[i] + "'", connection);
                        if(PRSID!=null)
                        {
                            N_PRSID=myFunctions.getIntVAL(PRSID.ToString());
                            if (X_PRSID == "")
                                X_PRSID = N_PRSID.ToString();
                            else
                                X_PRSID = X_PRSID + "," + N_PRSID.ToString();
                        }
                    }
                    QueryParams.Add("@X_PRSID", X_PRSID);

                    _sqlQuery = "Select * from vw_PrsPoMrn_Detail Where N_CompanyID=@nCompanyID and N_PRSID in (@X_PRSID)";
                    Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                    Detail = _api.Format(Detail, "prsDetails");
                    if (Detail.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    ds.Tables.Add(Detail);

                    return Ok(_api.Success(ds));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpGet("rfqList")]
        public ActionResult GetRFQList(bool bAllBranchData, int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nBranchID", nBranchID);

            string sqlCommandText = "";
            if (bAllBranchData == true)
                sqlCommandText = "Select *  from vw_Vendor_Request where N_CompanyID=@nCompanyID";
            else
                sqlCommandText = "Select *  from vw_Vendor_Request where N_CompanyID=@nCompanyID and N_BranchID=@nBranchID";
 
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
                return Ok(_api.Error(User,e));
            }
        }
      

        [HttpGet("VendorList")]
        public ActionResult GetVendorList(int N_QuotationID,int N_FnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@N_QuotationID", N_QuotationID);
            Params.Add("@N_FnYearID", N_FnYearID);

            string sqlCommandText = "";

            if(N_QuotationID>0)
                sqlCommandText = "Select *  from vw_InvVendorRFQ_Disp where N_CompanyID=@nCompanyID and N_FnYearID=@N_FnYearID and B_Inactive = 0 and N_RequestId=@N_QuotationID";
            else
                sqlCommandText = "Select *  from vw_InvVendorRFQ_Disp where N_CompanyID=@nCompanyID and N_FnYearID=@N_FnYearID and B_Inactive = 0";

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
                return Ok(_api.Error(User,e));
            }
        }

        [HttpGet("rfqDetails")]
        public ActionResult GetRFQDetails(int nQuotationID,int nVendorID)
        {
            DataTable Detail = new DataTable();
            DataTable RecepientTable = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@nQuotationID", nQuotationID);
            QueryParams.Add("@nVendorID", nVendorID);
            string _sqlQuery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if(nVendorID!=0)
                        _sqlQuery = "Select * from vw_RFQVendorListDetails Where N_CompanyID=@nCompanyID and N_QuotationID=@nQuotationID and N_VendorID=@nVendorID";
                    else
                        _sqlQuery = "Select * from vw_RFQVendorListDetails Where N_CompanyID=@nCompanyID and N_QuotationID=@nQuotationID";

                    Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                    Detail = _api.Format(Detail, "rfqDetails");

                    ds.Tables.Add(Detail);

                    return Ok(_api.Success(ds));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        private DataTable GetVendorListTable(int nQuotationID,int nVendorID,int N_CompanyID, IDataAccessLayer dLayer, SqlConnection connection)
        {
            DataTable VendorListDetails = new DataTable();
            string sqlCommand="";

            SortedList Params = new SortedList();
            Params.Add("@nQuotationID", nQuotationID);
            Params.Add("@nCompanyID", N_CompanyID);
            Params.Add("@nVendorID", nVendorID);

            if(nVendorID!=0)
                sqlCommand = "Select * from vw_RFQVendorListDetails Where N_CompanyID=@nCompanyID and N_QuotationID=@nQuotationID and N_VendorID=@nVendorID";
            else
                sqlCommand = "Select * from vw_RFQVendorListDetails Where N_CompanyID=@nCompanyID and N_QuotationID=@nQuotationID";

            VendorListDetails = dLayer.ExecuteDataTable(sqlCommand, Params, connection);

            VendorListDetails = _api.Format(VendorListDetails, "vendorList");


            return VendorListDetails; 
        }

    }
}
