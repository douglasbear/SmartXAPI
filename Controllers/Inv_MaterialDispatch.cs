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
    [Route("materialDisptach")]
    [ApiController]
    public class Inv_MaterialDispatch : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int FormID;
        public Inv_MaterialDispatch(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 684;
        }

        [HttpGet("list")]
        public ActionResult GetMaterialDispatchList(int nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,int FormID,int nLocationID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_DispatchNo like '%" + xSearchkey + "%' or X_ProjectName like '%" + xSearchkey + "%' or D_DispatchDate like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_DispatchNo desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_DispatchNo":
                        xSortBy = "X_DispatchNo " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_MaterialDispatchDisp where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=@p3" + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_MaterialDispatchDisp where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=@p3" + Searchkey + " and N_DispatchId not in (select top(" + Count + ") N_DispatchId from vw_MaterialDispatchDisp where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=@p2" + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", FormID);
            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from vw_MaterialDispatchDisp where N_CompanyID=@p1 and N_FnYearID=@p2 and N_FormID=@p3" + Searchkey + "";
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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetMaterialDispatchDetails(int nFnYearId, string xDispatchNo, int nLocationID, int nBranchId, bool B_AllBranchData, string x_PrsNo)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            bool B_ProjectExists = true;
            try
            {
                using (SqlConnection Con = new SqlConnection(connectionString))
                {
                    Con.Open();


                    DataSet dsMaterailDispatch = new DataSet();
                    SortedList QueryParamsList = new SortedList();
                    QueryParamsList.Add("@nCompanyID", nCompanyId);
                    QueryParamsList.Add("@nFnYearID", nFnYearId);
                    QueryParamsList.Add("@nBranchId", nBranchId);
                    QueryParamsList.Add("@xDispatchNo", xDispatchNo);
                    QueryParamsList.Add("@nLocationID", nLocationID);

                    string Mastersql = "";

                    if (B_AllBranchData)
                        Mastersql = "Select * From vw_MaterialDispatchDisp Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and X_DispatchNo=@xDispatchNo";
                    else
                        Mastersql = "Select * From vw_MaterialDispatchDisp Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and X_DispatchNo=@xDispatchNo and N_BranchId=@nBranchId";
                    if (x_PrsNo != "" && x_PrsNo != null)
                    {
                        Mastersql = "Select * From vw_Inv_PrsToDispatch Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and X_PrsNo=" + x_PrsNo + " ";

                    }
                    DataTable MasterTable = dLayer.ExecuteDataTable(Mastersql, QueryParamsList, Con);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    MasterTable = _api.Format(MasterTable, "Master");
                    int N_DispatchID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_DispatchId"].ToString());
                    int N_PRSId = 0;
                    if (x_PrsNo != "" && x_PrsNo != null)
                    {
                        N_PRSId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PRSID"].ToString());
                    }
                    QueryParamsList.Add("@N_DispatchID", N_DispatchID);

                    string DetailSql = "";

                    if (B_ProjectExists)
                        DetailSql = "Select *,dbo.SP_BatchStock(vw_MaterialDispatchDetailDisp.N_ItemID,vw_MaterialDispatchDetailDisp.N_LocationID,'',vw_MaterialDispatchDetailDisp.N_ProjectID) as N_stock  from vw_MaterialDispatchDetailDisp  where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_DispatchId=@N_DispatchID";
                    else
                        DetailSql = "Select *,dbo.SP_BatchStock(vw_MaterialDispatchDetailDisp.N_ItemID,@nLocationID,'') as N_stock  from vw_MaterialDispatchDetailDisp  where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_DispatchId=@N_DispatchID";


                    if (x_PrsNo != "" && x_PrsNo != null)
                    {
                        DetailSql="select * from vw_InvPRSDetailsToDispatch where  N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_RSID="+N_PRSId+"";
                    }
                    DataTable DetailTable = dLayer.ExecuteDataTable(DetailSql, QueryParamsList, Con);

                    DetailTable = _api.Format(DetailTable, "Details");
                    dsMaterailDispatch.Tables.Add(MasterTable);
                    dsMaterailDispatch.Tables.Add(DetailTable);
                    return Ok(_api.Success(dsMaterailDispatch));

                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        //Save....
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
                    DataTable Approvals;
                    string values = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                     Approvals = ds.Tables["approval"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    DataRow ApprovalRow = Approvals.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    bool bDeptEnabled = false;
                    int nDispatchID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_DispatchID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int N_RSID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_RSID"].ToString());
                    int nSaveDraft = myFunctions.getIntVAL(MasterTable.Rows[0]["B_IsSaveDraft"].ToString());
                    int N_UserID = myFunctions.getIntVAL(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    string X_DispatchNo = MasterTable.Rows[0]["X_DispatchNo"].ToString();
                    string transType = MasterTable.Rows[0]["transType"].ToString();
                    int nLastActionID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LastActionID"].ToString());
                     int FormID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FormID"].ToString());
                    int N_NextApproverID = 0;
                    int N_DispatchDetailsID=0;
                     values = MasterRow["X_DispatchNo"].ToString();

                     if( myFunctions.getIntVAL(MasterTable.Rows[0]["n_DepartmentID"].ToString())>0){
                        bDeptEnabled=true;
                     }
                     else{
                        bDeptEnabled=false;
                     }

                   

                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID", MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID",FormID);
                        //Params.Add("N_BranchID", MasterTable.Rows[0]["n_BranchId"].ToString());
                        X_DispatchNo = dLayer.GetAutoNumber("Inv_MaterialDispatch", "X_DispatchNo", Params, connection, transaction);
                        if (X_DispatchNo == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Return Number")); }
                        MasterTable.Rows[0]["X_DispatchNo"] = X_DispatchNo;
                    }

                     if (MasterTable.Columns.Contains("transType"))
                        MasterTable.Columns.Remove("transType");

                          
                          
                if(nLastActionID==2){

                     if (nDispatchID > 0)
                    {
                         dLayer.ExecuteNonQuery("update Inv_MaterialDispatch set N_LastActionID="+nLastActionID+" where N_CompanyID=" + nCompanyID + " and N_DispatchID=" + nDispatchID, Params, connection, transaction);
                         transaction.Commit();
                         return Ok(_api.Success("Material Request saved"));
                    }
                 }

                   if (nDispatchID > 0)
                    {
                        SortedList DeleteParams = new SortedList(){
                                    {"N_CompanyID",nCompanyID},
                                    {"N_UserID",N_UserID},
                                    {"X_TransType","MATERIAL DISPATCH"},
                                    {"X_SystemName","WebRequest"},
                                    {"N_VoucherID",nDispatchID}};

                        dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                    }


                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);
                    nDispatchID = dLayer.SaveData("Inv_MaterialDispatch", "N_DispatchID", MasterTable, connection, transaction);
                    if (nDispatchID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }
                    for (int i = 0; i < DetailTable.Rows.Count; i++)
                    {
                        DetailTable.Rows[i]["N_DispatchID"] = nDispatchID;
                    }
                     N_DispatchDetailsID = dLayer.SaveData("Inv_MaterialDispatchDetails", "N_DispatchDetailsID", DetailTable, connection, transaction);
                    if (N_DispatchDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }

                    if(nLastActionID==3){
                             if ((!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString())))
                    {
                        int N_PkeyID = nDispatchID;
                        string X_Criteria = "N_DispatchID=" + N_PkeyID + " and N_CompanyID=" + nCompanyID;
                        myFunctions.UpdateApproverEntry(Approvals, "Inv_MaterialDispatch", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID,transType, N_PkeyID, values, 1,"", 0, "",0, User, dLayer, connection, transaction);
                    nSaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IsSaveDraft as INT) from Inv_MaterialDispatch where N_DispatchID=" + nDispatchID + " and N_CompanyID=" + nCompanyID , connection, transaction).ToString());
                   if (nSaveDraft == 0){
                        
             
                      SortedList UpdateStockParam = new SortedList();
                    UpdateStockParam.Add("N_CompanyID", nCompanyID);
                    UpdateStockParam.Add("N_DispatchId", nDispatchID);
                    UpdateStockParam.Add("N_UserID", N_UserID);

                    if (!bDeptEnabled)
                        dLayer.ExecuteNonQueryPro("SP_Inv_MaterialDispatch", UpdateStockParam, connection, transaction);
                    else
                        dLayer.ExecuteNonQueryPro("SP_Inv_MaterialDispatch_Department", UpdateStockParam, connection, transaction);

                    SortedList PostParam = new SortedList();
                    PostParam.Add("N_CompanyID", nCompanyID);
                    PostParam.Add("X_InventoryMode", "MATERIAL DISPATCH");
                    PostParam.Add("N_InternalID", nDispatchID);
                    PostParam.Add("N_UserID", N_UserID);

                     dLayer.ExecuteNonQueryPro("SP_Acc_Inventory_Sales_Posting", PostParam, connection, transaction);
                    }
                
                        transaction.Commit();
                        //myFunctions.SendApprovalMail(N_NextApproverID, FormID, N_PkeyID, this.xTransType, values, dLayer, connection, transaction, User);
                        return Ok(_api.Success("Material Request Approved " + "-" + values));
                    }
                    }
                    
                    
         
                     N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, transType, nDispatchID, X_DispatchNo, 1, "", 0, "",0, User, dLayer, connection, transaction);
                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        //Delete....
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nDispatchID, int nCompanyID, int nFormID,int nFnYearID, int nBranchID, string comments)
        {
             if (comments == null)
            {
                comments = "";
            }
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                   
                    var nUserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                     DataTable TransData = new DataTable();
                     SortedList Params = new SortedList();
                     string xTransType="";
                     if (nFormID == 1309)
                     xTransType = "material Request";
                    else if(nFormID == 1592)
                    xTransType = "transfer Request";
                    else
                     xTransType = "purchase Request";

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nDispatchID", nDispatchID);

                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,X_DispatchNo,N_DispatchID from Inv_MaterialDispatch where N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID and N_DispatchId=@nDispatchID";
                    TransData = dLayer.ExecuteDataTable(Sql,Params,connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];
                    string X_Criteria = "N_DispatchID=" + nDispatchID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) ;

                DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, nFormID, nDispatchID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID,0, 0, User, dLayer, connection));
                 Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), comments);
                  SqlTransaction transaction = connection.BeginTransaction();
                   string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());
                  
                   string status = myFunctions.UpdateApprovals(Approvals, nFnYearID, xTransType, nDispatchID, TransRow["X_DispatchNo"].ToString(), ProcStatus, "Inv_MaterialDispatch", X_Criteria, "", User, dLayer, connection, transaction);
                   
                   if (status != "Error"){
                    if(ButtonTag=="3"){
                        dLayer.ExecuteNonQuery("update Inv_MaterialDispatch set N_LastActionID=4 where N_CompanyID=" + nCompanyID + " and N_DispatchID=" + nDispatchID, Params, connection, transaction);
                         transaction.Commit();
                         return Ok(_api.Success("Material Request saved"));
                    }

                     if (ButtonTag == "6" || ButtonTag == "0"){
                              SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"N_UserID",nUserID},
                                {"X_TransType","MATERIAL DISPATCH"},
                                {"X_SystemName","WebRequest"},
                                {"N_VoucherID",nDispatchID}};

                    Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete Material Dispatch."));
                    }
                      transaction.Commit();
                    return Ok(_api.Success("Material Dispatch deleted"));
                     }

                     else{
                        transaction.Commit();
                            return Ok(_api.Success("Material " + status + " Successfully"));
                     }
                
                    // else
                    // {
                    //     if (N_RSID > 0)
                    //         dLayer.ExecuteScalar("update Inv_PRS set N_Processed=0 where N_PRSID=" + N_RSID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, connection, transaction);
                    // }
                  
                   }
                    else
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to delete Material Dispatch"));
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