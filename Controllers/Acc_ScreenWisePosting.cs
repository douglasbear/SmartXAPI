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
using System.Linq;

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
        public ActionResult GetPostingDetails(int nFnYearID,string xScreen,string xVoucherNo,int value)
        {
            DataSet ds = new DataSet();
            DataTable dt=new DataTable();
            DataTable dtTotal=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            if(value==null)value=0;
            string sqlTotalText="";
            string sqlCommandText="";

            string fileName = "";
            if(value==1)
                sqlCommandText="select * from vw_ScreenWisePosting_Salary where N_CompanyID=@nCompanyID and X_Code=@xScreen and X_ReferenceNo=@xVoucherNo";
            else
                sqlCommandText="select * from vw_ScreenWisePosting where N_CompanyID=@nCompanyID and X_Code=@xScreen and X_VoucherNo=@xVoucherNo and N_FnYearID=@nFnYearID";
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

                    sqlTotalText="";
                    if(value==1){
                        fileName = "SalaryPosting_"+xVoucherNo+"_" + RandomString();
                        
                        sqlTotalText="select CONVERT(VARCHAR,SUM(CAST(Debit AS money)),1) as N_TotDebit,CONVERT(VARCHAR,SUM(CAST(Credit AS money)),1) AS N_TotCredit,'"+fileName+"' as fileName from vw_ScreenWisePosting_Salary Where N_CompanyID=@nCompanyID and X_Code=@xScreen and X_ReferenceNo=@xVoucherNo";
                        string strFillQuery = "select * from vw_ScreenWisePosting_Salary where N_CompanyID=" + myFunctions.GetCompanyID(User) + " and X_Code='" + xScreen + "' and X_ReferenceNo='" + xVoucherNo + "'";
                        
                        myFunctions.ExportToExcel(User,strFillQuery, fileName ,dLayer,connection);
                        fileName = fileName+".csv";
                        }
                    else
                        sqlTotalText="select CONVERT(VARCHAR,SUM(CAST(Debit AS money)),1) as N_TotDebit,CONVERT(VARCHAR,SUM(CAST(Credit AS money)),1) AS N_TotCredit from vw_ScreenWisePosting Where N_CompanyID=@nCompanyID and X_Code=@xScreen and X_VoucherNo=@xVoucherNo and N_FnYearID=@nFnYearID";
                        
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
                return Ok(_api.Error(User,e));
            }
          
        }

                private static Random random = new Random();
        public string RandomString(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }



        

        

    }
}