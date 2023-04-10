using AutoMapper;
using SmartxAPI.Data;
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
    [Route("processDepreciation")]
    [ApiController]
    public class Inv_ProcessDepreciation : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;

        public Inv_ProcessDepreciation(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 403;
        }

        [HttpGet("list")]
        public ActionResult ProcessDepreciationList(int nFnYearID, int nBranchID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearID);
            Params.Add("@p3", nBranchID);
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ( X_DepriciationNo like '%" + xSearchkey + "%' or D_RunDate like '%" + xSearchkey + "%' or D_EntryDate like '%" + xSearchkey + "%' ) ";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_DeprID desc";
            else if(xSortBy.Contains("d_RunDate"))
                xSortBy =" order by cast(D_RunDate as DateTime) " + xSortBy.Split(" ")[1];
            else if(xSortBy.Contains("d_EntryDate"))
                xSortBy =" order by cast(D_EntryDate as DateTime) " + xSortBy.Split(" ")[1];
            else
                xSortBy = " order by " + xSortBy;
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from Ass_DepreciationMaster where N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 " + Searchkey + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from Ass_DepreciationMaster where N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 " + Searchkey + "and N_DeprID not in (select top(" + Count + ") N_DeprID from Ass_DepreciationMaster where N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 " + xSortBy + " ) " + xSortBy;

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(1) as N_Count  from Ass_DepreciationMaster where N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 " + Searchkey;
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
                return Ok(_api.Error(User,e));
            }
        }

        [HttpGet("lastDate")]
        public ActionResult GetLatDate(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText="",sqlCmd2="";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearID);

            sqlCommandText = "select D_RunDate from Ass_Depreciation where N_CompanyID=@p1 and N_FnYearID=@p2 group by D_RunDate having count(1)>=1 order by D_RunDate DESC ";

            SortedList OutPut = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    object lastDepNo = dLayer.ExecuteScalar("select top(1) X_DepriciationNo from Ass_Depreciation where N_CompanyID=@p1 and N_FnYearID=@p2 order by D_RunDate DESC", Params, connection);
                    if (lastDepNo == null) lastDepNo = "";
                    sqlCmd2="select count(1) as X_TotalCount from (select N_ItemID, count(1) as coun,D_RunDate  from Ass_Depreciation where N_CompanyID=@p1 and N_FnYearID =@p2 and X_DepriciationNo='"+lastDepNo+"' group by N_ItemID,D_RunDate) AS subquery ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCmd2, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Success(OutPut));
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
        public ActionResult GetProcessDepreciation(int nFnYearID, int nBranchID, string xDepriciationNo,bool b_AllBranchData)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string cond="",sqlCommandText="",sqlCmd2="";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearID);
            Params.Add("@p3", nBranchID);
            Params.Add("@p4", xDepriciationNo);

            if(b_AllBranchData)
                cond="N_FnYearID=@p2 and N_CompanyID=@p1";
            else
                cond="N_FnYearID=@p2 and N_CompanyID=@p1 and N_BranchID=@p3";

            sqlCommandText = "select * from Vw_AssetDepreciationMaster where "+cond+" and X_DepriciationNo=@p4";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCmd2="select count(1) as X_TotalCount from (select N_ItemID, count(1) as coun,D_RunDate  from Ass_Depreciation where N_CompanyID=@p1 and N_FnYearID =@p2 and X_DepriciationNo=@p4 group by N_ItemID,D_RunDate) AS subquery ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCmd2, Params, connection);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "n_assetEffected", typeof(int), TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(dt));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        } 

           //Save....
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {

            DataTable MasterTable;
            DataTable DepTable;
            MasterTable = ds.Tables["master"];
            SortedList Params = new SortedList();
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    string DepreciationNo = "";
                    int N_CompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int N_BranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_BranchID"].ToString());
                    int N_DeprID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_DeprID"].ToString());
                    int N_AllBranchData = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AllBranchData"].ToString());
                    DateTime D_RunDate = Convert.ToDateTime(MasterTable.Rows[0]["D_RunDate"].ToString());
                    int N_UserID = myFunctions.GetUserID(User);
                    MasterTable.Columns.Remove("N_AllBranchData");
                    var values = MasterTable.Rows[0]["X_DepriciationNo"].ToString();
                    string xButtonAction="";
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("N_BranchID", N_BranchID);
                        DepreciationNo = dLayer.GetAutoNumber("Ass_Transactions", "X_Reference", Params, connection, transaction);
                       xButtonAction="Insert"; 
                        if (DepreciationNo == "") { 
                            transaction.Rollback();
                             return Ok(_api.Warning("Unable to generate Depreciation Number")); }
                        MasterTable.Rows[0]["X_DepriciationNo"] = DepreciationNo;
                        
                    }
                   
                      
                    else
                    
                    
                    {

                         DepreciationNo=MasterTable.Rows[0]["X_DepriciationNo"].ToString();
                        SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",N_CompanyID.ToString()},
                                {"X_TransType","Depreciation"},
                                {"N_VoucherID",myFunctions.getIntVAL(DepreciationNo.ToString())},
                                {"N_UserID",N_FnYearID.ToString()},
                                {"X_SystemName",""},
                                {"N_BranchID",N_BranchID.ToString()}};
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                             xButtonAction="Update"; 
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,ex));
                        }
                    }

             


                    string Condn="",SqlCmd="";
                    if (N_AllBranchData == 1)
                        Condn = "dbo.Ass_AssetMaster.N_CompanyID=" + N_CompanyID;
                    else
                        Condn = "dbo.Ass_AssetMaster.N_CompanyID=" + N_CompanyID + "  and dbo.Ass_AssetMaster.N_BranchID=" + N_BranchID;

                    SqlCmd = "SELECT max(dbo.Ass_Depreciation.D_EndDate) AS  D_EndDate,dbo.Ass_AssetMaster.N_ItemID,dbo.Ass_AssetMaster.X_ItemCode, dbo.Ass_AssetMaster.N_BookValue, dbo.Ass_AssetMaster.N_LifePeriod, dbo.Ass_PurchaseDetails.D_PurchaseDate, dbo.Ass_AssetMaster.N_BranchID, dbo.Ass_PurchaseDetails.N_Price,dbo.Ass_AssetMaster.D_PlacedDate,dbo.Ass_AssetMaster.N_CategoryID,ISNULL(Ass_AssetMaster.N_DeprCalcID,0) AS N_DeprCalcID FROM   dbo.Ass_AssetMaster INNER JOIN dbo.Ass_PurchaseDetails ON dbo.Ass_AssetMaster.N_AssetInventoryDetailsID = dbo.Ass_PurchaseDetails.N_AssetInventoryDetailsID left outer join Ass_Depreciation on Ass_Depreciation.N_ItemID =Ass_AssetMaster.N_ItemID and Ass_Depreciation.N_CompanyID=Ass_AssetMaster.N_CompanyID Where " + Condn + " and dbo.Ass_AssetMaster.N_Status<2 and isnull(dbo.Ass_AssetMaster.N_SaveDraft,0) = 0  group by dbo.Ass_AssetMaster.N_ItemID,dbo.Ass_AssetMaster.X_ItemCode, dbo.Ass_AssetMaster.N_BookValue, dbo.Ass_AssetMaster.N_LifePeriod, dbo.Ass_PurchaseDetails.D_PurchaseDate, dbo.Ass_AssetMaster.N_BranchID, dbo.Ass_PurchaseDetails.N_Price,dbo.Ass_AssetMaster.D_PlacedDate,dbo.Ass_AssetMaster.N_CategoryID,Ass_AssetMaster.N_DeprCalcID,dbo.Ass_AssetMaster.N_SaveDraft";
                    DepTable = dLayer.ExecuteDataTable(SqlCmd, Params, connection,transaction);

                    N_DeprID = dLayer.SaveData("Ass_DepreciationMaster", "N_DeprID", MasterTable, connection, transaction);
                   // xButtonAction="Update"; 
                    if (N_DeprID <= 0)
                    {
                        transaction.Rollback();
                    }

                    int N_ItemID=0,N_DeprCalcID=0;
                    DateTime EndDate;
                    bool B_completed=true;
                    // foreach (DataRow drow in DepTable.Rows)
                    // {
                    //     N_ItemID = myFunctions.getIntVAL(drow["N_ItemID"].ToString());
                    //     N_DeprCalcID = myFunctions.getIntVAL(drow["N_DeprCalcID"].ToString());
                    //     EndDate = D_RunDate;
                    //     EndDate = EndDate.AddMonths(1);
                    //     EndDate = EndDate.AddDays(-(EndDate.Day));
                    //     SortedList ProcParams = new SortedList(){
                    //         {"N_CompanyID",N_CompanyID.ToString()},
                    //         {"N_FnYearID",N_FnYearID.ToString()},
                    //         {"N_ItemID",myFunctions.getIntVAL(drow["N_ItemID"].ToString())},
                    //         {"D_EndDate",Convert.ToDateTime(EndDate.ToString())},
                    //         {"N_UserID",myFunctions.GetUserID(User).ToString()},
                    //         {"X_DeprNo",myFunctions.getIntVAL(DepreciationNo.ToString())}
                    //     };
                    //     if (N_DeprCalcID == 195 || N_DeprCalcID == 242)
                    //     {
                    //         B_completed = Convert.ToBoolean(dLayer.ExecuteScalarPro("SP_Ass_Depreciation_WDV" ,ProcParams, connection, transaction).ToString());
                    //     }
                    //     else
                    //         B_completed = myFunctions.Depreciation(dLayer,N_CompanyID,N_FnYearID,N_UserID,N_ItemID, EndDate, DepreciationNo.Trim(),connection, transaction);
                            
                    // }
                    EndDate = D_RunDate;
                    EndDate = EndDate.AddMonths(1);
                    EndDate = EndDate.AddDays(-(EndDate.Day));

                    SortedList ProcParams = new SortedList(){
                        {"N_CompanyID",N_CompanyID.ToString()},
                        {"N_FnYearID",N_FnYearID.ToString()},
                        {"N_UserID",myFunctions.GetUserID(User).ToString()},
                        {"D_EndDate",Convert.ToDateTime(EndDate.ToString())},
                        {"B_AllBranchData",N_AllBranchData},
                        {"N_BranchID",N_BranchID},
                        {"X_DeprNo",myFunctions.getIntVAL(DepreciationNo.ToString())}
                    };

                    dLayer.ExecuteScalarPro("SP_Ass_DepreciationMaster" ,ProcParams, connection, transaction);
                    B_completed=true;
                   
                           //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(N_FnYearID,N_DeprID,DepreciationNo,403,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);


                    // if(B_completed)
                    // {
                    //     SortedList PostParams = new SortedList(){
                    //                 {"N_CompanyID",N_CompanyID.ToString()},
                    //                 {"X_InventoryMode","Depreciation"},
                    //                 {"N_InternalID",N_DeprID},
                    //                 {"N_UserID",N_UserID}};
                    //     dLayer.ExecuteNonQueryPro("SP_Acc_InventoryPosting", PostParams, connection, transaction);
                    // }

                    SortedList Result = new SortedList();
                    Result.Add("N_DeprID", N_DeprID);
                    Result.Add("X_DepriciationNo", DepreciationNo);
                    transaction.Commit();
                    return Ok(_api.Success(Result, "Process Depreciation completed"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        //Delete....
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nFnYearID, int nCompanyId,string DepreciationNo,int nBranchID,int N_DeprID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    SqlTransaction transaction = connection.BeginTransaction();
                     ParamList.Add("@nTransID", N_DeprID);
                    ParamList.Add("@nCompanyID", nCompanyId);
                    ParamList.Add("@nFnYearID", nFnYearID);
                     string Sql = "select N_DeprID,X_DepriciationNo from Ass_DepreciationMaster where N_DeprID=@nTransID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID";
                     string xButtonAction="Delete";
                     string X_DepriciationNo="";

                     TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection,transaction);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

               
            // object n_FnYearID = dLayer.ExecuteScalar("select N_FnyearID from Ass_DepreciationMaster where N_DeprID =" + N_DeprID + " and N_CompanyID=" + nCompanyID, Params, connection,transaction);

                       
               //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(myFunctions.getIntVAL( nFnYearID.ToString()),N_DeprID,TransRow["X_DepriciationNo"].ToString(),403,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);


                    SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyId},
                                {"X_TransType","Depreciation"},
                                {"N_VoucherID",myFunctions.getIntVAL(DepreciationNo.ToString())},
                                {"N_UserID",nFnYearID},
                                {"X_SystemName",""},
                                {"N_BranchID",nBranchID}};
                    try
                    {
                        dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,ex));
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Depreciation Deleted"));
                }             
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }


        }
    }
}