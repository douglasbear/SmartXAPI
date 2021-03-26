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
    [Route("salesDashboard")]
    [ApiController]
    public class SalesDashboard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public SalesDashboard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("details")]
        public ActionResult GetDashboardDetails()
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);

            string sqlCurrentOrder = "SELECT COUNT(*) as N_ThisMonth,sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvSalesOrderNo_Search WHERE MONTH(D_OrderDate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_OrderDate)= YEAR(CURRENT_TIMESTAMP)";
            string sqlCurrentInvoice = "SELECT COUNT(*) as N_ThisMonth,sum(Cast(REPLACE(X_BillAmt,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvSalesInvoiceNo_Search WHERE MONTH(Cast([Invoice Date] as DateTime)) = MONTH(CURRENT_TIMESTAMP) AND YEAR(Cast([Invoice Date] as DateTime)) = YEAR(CURRENT_TIMESTAMP)";
            string sqlCurrentQuotation = "SELECT COUNT(*) as N_ThisMonth,sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvSalesQuotationNo_Search WHERE MONTH(D_QuotationDate) = MONTH(CURRENT_TIMESTAMP) AND YEAR(D_QuotationDate) = YEAR(CURRENT_TIMESTAMP)";
            //string sqlCurrentSales =""

            SortedList Data=new SortedList();
            DataTable CurrentOrder = new DataTable();
            DataTable CurrentInvoice = new DataTable();
            DataTable CurrentQuotation = new DataTable();
           

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    CurrentOrder = dLayer.ExecuteDataTable(sqlCurrentOrder, Params, connection);
                    CurrentInvoice = dLayer.ExecuteDataTable(sqlCurrentInvoice, Params, connection);
                    CurrentQuotation = dLayer.ExecuteDataTable(sqlCurrentQuotation, Params, connection);

                  
                }
               

               CurrentOrder.AcceptChanges();
               CurrentInvoice.AcceptChanges();
               CurrentQuotation.AcceptChanges();



                if(CurrentOrder.Rows.Count>0)Data.Add("orderData",CurrentOrder);
                if(CurrentInvoice.Rows.Count>0)Data.Add("invoiceData",CurrentInvoice);
                if(CurrentQuotation.Rows.Count>0)Data.Add("quotationData",CurrentQuotation);
                
               

                return Ok(api.Success(Data));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
    }
}