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
    [Route("whgrn")]
    [ApiController]
    public class whGRN : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int N_FormID;
        public whGRN(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 1407;
        }
        [HttpGet("list")]
        public ActionResult GetwhGRNList(int? nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
              try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
            DataTable dt = new DataTable();
            DataTable CountTable = new DataTable();
            SortedList Params = new SortedList();
            DataSet dataSet = new DataSet();
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            int nUserID = myFunctions.GetUserID(User);


            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([X_GRNNo] like '%" + xSearchkey + "%' or X_CustomerName like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_GRNID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "[X_GRNNo]":
                        xSortBy = "N_GRNID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }
            int Count = (nPage - 1) * nSizeperpage;
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_CustomerID,N_GRNID,N_FnYearID,D_GRNDate,N_BranchID,[X_GRNNo] AS X_GRNNo,X_CustomerName,D_GRNDate from vw_Wh_GRN where N_CompanyID=@p1 and N_FnYearID=@p2"  + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_CustomerID,N_GRNID,N_FnYearID,D_GRNDate,N_BranchID,[X_GRNNo] AS X_GRNNo,X_CustomerName,D_GRNDate from vw_Wh_GRN where N_CompanyID=@p1 and N_FnYearID=@p2" + Searchkey + " and N_GRNID not in (select top(" + Count + ") N_GRNID from vw_Wh_GRN where N_CompanyID=@p1 and N_FnYearID=@p2 " + xSortBy + " ) " + xSortBy;

            // sqlCommandText = "select * from Inv_MRNDetails where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            SortedList OutPut = new SortedList();

          
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count from vw_Wh_GRN where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];                        TotalCount = drow["N_Count"].ToString();
                    }
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



 [HttpGet("details")]
        public ActionResult GetGoodsReceiveDetails(int nCompanyId, int nFnYearId, string nGRNNo, bool showAllBranch, int nBranchId, string poNo)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable dtGoodReceive = new DataTable();
            DataTable dtGoodReceiveDetails = new DataTable();
            int N_GRNID = 0;
            int N_POrderID = 0;

            Params.Add("@CompanyID", nCompanyId);
            Params.Add("@YearID", nFnYearId);
            Params.Add("@TransType", "GRN");
            Params.Add("@BranchID", nBranchId);
            string X_MasterSql = "";
            string X_DetailsSql = "";
           

            if (nGRNNo != null)
            {
                Params.Add("@GRNNo", nGRNNo);
                X_MasterSql = "select N_CompanyID,N_CustomerID,N_GRNID,N_FnYearID,D_GRNDate,N_BranchID,[GRN No] AS x_GRNNo,X_CustomerName,GRNDate from vw_Wh_GRN as where N_CompanyID=@CompanyID and [GRN No]=@GRNNo and N_FnYearID=@YearID " + (showAllBranch ? "" : " and  N_BranchId=@BranchID");
            }
            if (poNo != null)
            {
                X_MasterSql = "Select Inv_PurchaseOrder.*,Inv_Location.X_LocationName,Inv_Vendor.* from Inv_PurchaseOrder Inner Join Inv_Vendor On Inv_PurchaseOrder.N_VendorID=Inv_Vendor.N_VendorID and Inv_PurchaseOrder.N_CompanyID=Inv_Vendor.N_CompanyID and Inv_PurchaseOrder.N_FnYearID=Inv_Vendor.N_FnYearID LEFT OUTER JOIN Inv_Location ON Inv_Location.N_LocationID=Inv_PurchaseOrder.N_LocationID Where Inv_PurchaseOrder.N_CompanyID=" + nCompanyId + " and X_POrderNo='" + poNo + "' and Inv_PurchaseOrder.B_IsSaveDraft<>1";
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dtGoodReceive = dLayer.ExecuteDataTable(X_MasterSql, Params, connection);
                    if (dtGoodReceive.Rows.Count == 0) { return Ok(_api.Warning("No Data Found")); }
                    dtGoodReceive = _api.Format(dtGoodReceive, "Master");

                    if (poNo != null)
                    {
                        N_POrderID = myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_POrderid"].ToString());
                    }
                    else
                    {
                        N_GRNID = myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_GRNID"].ToString());

                    }
                    if (N_GRNID != 0)
                    {
                        X_DetailsSql = "Select * from vw_InvMRNDetails  Where vw_InvMRNDetails.N_CompanyID=@CompanyID and vw_InvMRNDetails.N_RNID=" + N_GRNID + (showAllBranch ? "" : " and vw_InvMRNDetails.N_BranchId=@BranchID");
                    }
                    if (N_POrderID != 0)
                    {
                        X_DetailsSql = "Select *,dbo.SP_Cost(vw_POMrn_PendingDetail.N_ItemID,vw_POMrn_PendingDetail.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_POMrn_PendingDetail.N_ItemID,vw_POMrn_PendingDetail.N_CompanyID) As N_UnitSPrice  from vw_POMrn_PendingDetail Where N_CompanyID=" + nCompanyId + " and N_POrderID=" + N_POrderID + "";

                    }


                    dtGoodReceiveDetails = dLayer.ExecuteDataTable(X_DetailsSql, Params, connection);
                    dtGoodReceiveDetails = _api.Format(dtGoodReceiveDetails, "Details");

                  
                    if (N_POrderID != 0)
                    {
                    }
                    else
                    {
                        DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_VendorID"].ToString()), myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_MRNID"].ToString()), this.N_FormID, myFunctions.getIntVAL(dtGoodReceive.Rows[0]["N_FnYearID"].ToString()), User, connection);
                        Attachments = _api.Format(Attachments, "attachments");
                          dt.Tables.Add(Attachments);
                       
                    }
                    dt.Tables.Add(dtGoodReceive);
                     dt.Tables.Add(dtGoodReceiveDetails);
                    

                }
                return Ok(_api.Success(dt));
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
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int nGrnID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_GRNID"].ToString());
                    int N_UserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    string X_GRNNo = "";
                    var values = MasterTable.Rows[0]["X_GRNNo"].ToString();

            
            
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", 370);
                        Params.Add("N_YearID", N_FnYearID);

                X_GRNNo = dLayer.GetAutoNumber("wh_GRN", "X_GRNNo", Params, connection, transaction);
                        if (X_GRNNo == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Warning("Unable to generate"));
                        }
                        MasterTable.Rows[0]["X_GRNNo"] = X_GRNNo;
                    }
                    nGrnID = dLayer.SaveData("wh_GRN", "N_GRNID", MasterTable, connection, transaction);

                    if (nGrnID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_GrnID"] = nGrnID;
                    }
                      int N_GRNDetailsID = dLayer.SaveData("wh_GRNDetails", "N_GRNDetailsID", DetailTable, connection, transaction);
                    if (N_GRNDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));

                    }

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("N_GRNDetailsID", nGrnID);
                    Result.Add("X_GRNNo", X_GRNNo);
                    return Ok(_api.Success(Result, "Saved"));
                }
            }
        
          catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
    }
}
                

            