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

        [HttpGet("details")]
        public ActionResult GetProcessDepreciation(int nFnYearID, int nBranchID, string xDepriciationNo)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearID);
            Params.Add("@p3", nBranchID);
            Params.Add("@p4", xDepriciationNo);
            string sqlCommandText = "select * from Ass_Depreciation where N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 and X_DepriciationNo=@p4";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
    }
}