using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("salaryRevisionHistory")]
    [ApiController]
    public class Pay_PayHistoryMaster : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Pay_PayHistoryMaster(IApiFunctions apiFun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apiFun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetSalRevHistoryList(int? nCompanyId,int nEmpID,int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
            string sqlCommandCount="";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and X_HistoryCode like '%" + xSearchkey + "%'";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_HistoryID desc";
            else
            xSortBy = " order by " + xSortBy;

            if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_PayHistoryMaster where N_CompanyID=@p1 and N_EmpID=@p2" + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_PayHistoryMaster where N_CompanyID=@p1 and N_EmpID=@p2" + Searchkey + " and N_HistoryID not in(select top("+ Count +")  N_HistoryID from vw_PayHistoryMaster where N_CompanyID=@p1 and N_EmpID=@p2" + xSortBy + " ) " + xSortBy;
            
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nEmpID);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count from vw_PayHistoryMaster where N_CompanyID=@p1 and N_EmpID=@p2" + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount="0";
                    if(Summary.Rows.Count>0){
                    DataRow drow = Summary.Rows[0];
                    TotalCount = drow["N_Count"].ToString();
                    }
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(OutPut));
                }  
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
    }
}