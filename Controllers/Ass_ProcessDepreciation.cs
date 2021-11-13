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

                    sqlCommandCount = "select count(*) as N_Count  from Ass_DepreciationMaster where N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 " + Searchkey;
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

            sqlCommandText = "select D_RunDate from Ass_Depreciation where N_CompanyID=@p1 and N_FnYearID=@p2 group by D_RunDate having COUNT(*)>=1 order by D_RunDate DESC ";
            SortedList OutPut = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCmd2="select count(*) as X_TotalCount from (select N_ItemID, COUNT(*) as coun,D_RunDate  from Ass_Depreciation where N_FnYearID =@p2 group by N_ItemID,D_RunDate) AS subquery ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCmd2, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok();
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

            sqlCommandText = "select * from Ass_DepreciationMaster where "+cond+" and X_DepriciationNo=@p4";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCmd2="select count(*) as X_TotalCount from (select N_ItemID, COUNT(*) as coun,D_RunDate  from Ass_Depreciation where N_FnYearID =@p2 group by N_ItemID,D_RunDate) AS subquery ";
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
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("N_BranchID", N_BranchID);
                        DepreciationNo = dLayer.GetAutoNumber("Ass_Transactions", "X_Reference", Params, connection, transaction);
                        if (DepreciationNo == "") { transaction.Rollback(); return Ok(_api.Warning("Unable to generate Depreciation Number")); }
                        MasterTable.Rows[0]["X_DepriciationNo"] = DepreciationNo;
                    }
                    else
                    {
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
                    if (N_DeprID <= 0)
                    {
                        transaction.Rollback();
                    }

                    int N_ItemID=0,N_DeprCalcID=0;
                    DateTime EndDate;
                    bool B_completed=true;
                    foreach (DataRow drow in DepTable.Rows)
                    {
                        N_ItemID = myFunctions.getIntVAL(drow["N_ItemID"].ToString());
                        N_DeprCalcID = myFunctions.getIntVAL(drow["N_DeprCalcID"].ToString());
                        EndDate = D_RunDate;
                        EndDate = EndDate.AddMonths(1);
                        EndDate = EndDate.AddDays(-(EndDate.Day));
                        SortedList ProcParams = new SortedList(){
                            {"N_CompanyID",N_CompanyID.ToString()},
                            {"N_FnYearID",N_FnYearID.ToString()},
                            {"N_ItemID",myFunctions.getIntVAL(drow["N_ItemID"].ToString())},
                            {"D_EndDate",Convert.ToDateTime(EndDate.ToString())},
                            {"N_UserID",myFunctions.GetUserID(User).ToString()},
                            {"X_DeprNo",myFunctions.getIntVAL(DepreciationNo.ToString())}
                        };
                        if (N_DeprCalcID == 195 || N_DeprCalcID == 242)
                        {
                            B_completed = Convert.ToBoolean(dLayer.ExecuteScalarPro("SP_Ass_Depreciation_WDV" ,ProcParams, connection, transaction).ToString());
                        }
                        else
                            B_completed = myFunctions.Depreciation(dLayer,N_CompanyID,N_FnYearID,N_UserID,N_ItemID, EndDate, DepreciationNo.Trim(),connection, transaction);
                            
                    }

                    if(B_completed)
                    {
                        SortedList PostParams = new SortedList(){
                                    {"N_CompanyID",N_CompanyID.ToString()},
                                    {"X_InventoryMode","Depreciation"},
                                    {"N_InternalID",N_DeprID},
                                    {"N_UserID",N_UserID}};
                        dLayer.ExecuteNonQueryPro("SP_Acc_InventoryPosting", PostParams, connection, transaction);
                    }

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
        public ActionResult DeleteData(int nFnYearID, int nCompanyId,string DepreciationNo,int nBranchID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyId.ToString()},
                                {"X_TransType","Depreciation"},
                                {"N_VoucherID",myFunctions.getIntVAL(DepreciationNo.ToString())},
                                {"N_UserID",nFnYearID.ToString()},
                                {"X_SystemName",""},
                                {"N_BranchID",nBranchID.ToString()}};
                    try
                    {
                        dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,ex));
                    }
                }

                return Ok(_api.Success("Depreciation Deleted"));

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }


        }
    }
}