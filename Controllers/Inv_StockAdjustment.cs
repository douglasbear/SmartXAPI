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
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("invStockAdjustment")]
    [ApiController]
    public class Inv_StockAdjustment : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly IMyAttachments myAttachments;

        public Inv_StockAdjustment(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            myAttachments = myAtt;
        }


 
 
         [HttpGet("list")]
        public ActionResult InvAdjustmentList(int nFnYearID,int nLocationID, int nBranchID, int nPage, int nSizeperpage, bool b_AllBranchData, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    string sqlCommandCount = "", xCriteria = "";
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string Searchkey = "";
                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nFnYearID);
                    Params.Add("@p3", nBranchID);
                    Params.Add("@p4", nLocationID);
                    
                    if (b_AllBranchData)
                    {
                        if (nLocationID>0)
                           xCriteria = " N_FnYearID=@p2 and N_CompanyID=@p1 and N_LoactionID=@p4 ";
                
                        else
                            xCriteria = " N_FnYearID=@p2 and N_CompanyID=@p1 ";
                    }
                    else
                    {
                        if (nLocationID>0)
                            xCriteria = "and N_FnYearID=@p2 and N_BranchID=@p3 and N_CompanyID=@p1 and N_LoactionID=@p4";
                           
                        else
                           xCriteria = "and N_FnYearID=@p2 and N_CompanyID=@p1 and N_BranchID=@p3";
                    }

                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and ( N_ItemID like '%" + xSearchkey + "%') ";

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_ItemID desc";
                    else
                        xSortBy = " order by " + xSortBy;
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvStockAdjustment_Disp where " + xCriteria + Searchkey;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvStockAdjustment_Disp where " + xCriteria + Searchkey + "and N_ItemID not in (select top(" + Count + ") N_ItemID from vw_InvStockAdjustment_Disp where " + xCriteria + Searchkey + " ) ";
                    SortedList OutPut = new SortedList();

                    dt = dLayer.ExecuteDataTable(sqlCommandText + xSortBy, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from vw_InvStockAdjustment_Disp where " + xCriteria + Searchkey;
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






         [HttpDelete("delete")]
        public ActionResult DeleteData(int nItemID)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    var nUserID = myFunctions.GetUserID(User);
                    SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"N_ItemID",nItemID},
                                {"N_UserID",nUserID},
                                };
                    int Results = dLayer.ExecuteNonQueryPro("vw_InvStockAdjustment_Disp", DeleteParams, connection, transaction);

                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete"));
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Deleted Sucessfully"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

    }
}