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
    [Route("puchaseDashboard")]
    [ApiController]
    public class PurchaseDashboard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public PurchaseDashboard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
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

            string sqlCurrentOrder = "SELECT COUNT(*) as N_ThisMonth,sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvPurchaseOrderNo_Search WHERE MONTH(Cast([Order Date] as DateTime))= MONTH(CURRENT_TIMESTAMP) AND YEAR(Cast([Order Date] as DateTime))= YEAR(CURRENT_TIMESTAMP)";
            string sqlCurrentInvoice = "SELECT COUNT(*) as N_ThisMonth,sum(Cast(REPLACE(N_InvoiceAmt,',','') as Numeric(10,2)) ) as TotalAmount FROM vw_InvPurchaseInvoiceNo_Search WHERE MONTH(Cast([Invoice Date] as DateTime)) = MONTH(CURRENT_TIMESTAMP) AND YEAR(Cast([Invoice Date] as DateTime)) = YEAR(CURRENT_TIMESTAMP)";
            //string sqlCurrentPurchase =""

            SortedList Data=new SortedList();
            DataTable CurrentOrder = new DataTable();
            DataTable CurrentInvoice = new DataTable();
           
           

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    CurrentOrder = dLayer.ExecuteDataTable(sqlCurrentOrder, Params, connection);
                    CurrentInvoice = dLayer.ExecuteDataTable(sqlCurrentInvoice, Params, connection);
                    

                  
                }
               

               CurrentOrder.AcceptChanges();
               CurrentInvoice.AcceptChanges();



                if(CurrentOrder.Rows.Count>0)Data.Add("orderData",CurrentOrder);
                if(CurrentInvoice.Rows.Count>0)Data.Add("invoiceData",CurrentInvoice);
               
                
               

                return Ok(api.Success(Data));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
    }
}