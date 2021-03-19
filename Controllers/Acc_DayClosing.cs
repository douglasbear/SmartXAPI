using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("dayClosing")]
    [ApiController]
    public class Acc_DayClosing : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Acc_DayClosing(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 763;
        }

        [HttpGet("salesSummary")]
        public ActionResult GetSalesSummaryDetails(int nCompanyID,int nFnYearID,int nBranchID,DateTime dTransDate)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nFnYearID);
            Params.Add("@p3",nBranchID);
            Params.Add("@p4",dTransDate);
            string sqlCommandText="Select N_CompanyID,SUM(N_AmountDr)as N_AmountDr,SUM(N_AmountCr) as N_AmountCr,D_TransDate,N_BranchID,N_FnYearID,X_CustomerName from  Vw_DailySalesSummary where N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchID=@p3 and D_TransDate=@p4 group by N_CompanyID,D_TransDate,N_BranchID,N_FnYearID,X_CustomerName";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
                return Ok(_api.Error(e));
            }
        }
    }
}