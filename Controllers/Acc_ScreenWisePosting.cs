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
    [Route("screenWisePosting")]
    [ApiController]
    public class Acc_ScreenWisePosting : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Acc_ScreenWisePosting(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1126;
        }

        [HttpGet("details")]
        public ActionResult GetPostingDetails(int nFnYearID,string xScreen,string xVoucherNo)
        {
            DataSet ds = new DataSet();
            DataTable dt=new DataTable();
            DataTable dtTotal=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlTotalText="";
            string sqlCommandText="select * from vw_ScreenWisePosting where N_CompanyID=@nCompanyID and X_Code=@xScreen and X_VoucherNo=@xVoucherNo and N_FnYearID=@nFnYearID";
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nFnYearID",nFnYearID);
            Params.Add("@xScreen",xScreen);
            Params.Add("@xVoucherNo",xVoucherNo);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection); 
                    dt = _api.Format(dt, "post");

                    sqlTotalText="select CONVERT(VARCHAR,SUM(CAST(Debit AS money)),1) as N_TotDebit,CONVERT(VARCHAR,SUM(CAST(Credit AS money)),1) AS N_TotCredit from vw_ScreenWisePosting_Salary Where N_CompanyID=@nCompanyID and X_Code=@xScreen and X_ReferenceNo=@xVoucherNo";
                    dtTotal=dLayer.ExecuteDataTable(sqlTotalText,Params,connection); 
                    dtTotal = _api.Format(dtTotal, "total");

                    ds.Tables.Add(dt);
                    ds.Tables.Add(dtTotal);
                }
                if(dt.Rows.Count==0)
                {
                    return Ok(_api.Notice("No Results Found" ));
                }
                else
                {
                    return Ok(_api.Success(ds));
                }
            }
            catch(Exception e)
            {
                return Ok(_api.Error(e));
            }
          
        }

        

    }
}