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
    [Route("paymentMethod")]
    [ApiController]
    public class Acc_PaymentMethod : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        public Acc_PaymentMethod(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("list")]
        public ActionResult GetPayMethodList(string type)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string feild = "N_PaymentMethodID,N_CompanyID,N_TypeID,B_IsCheque,X_PayMethod";
            string crieteria = " N_CompanyID=@nCompanyID";
            switch (type.ToLower())
            {
                case "vendorpayment":
                    crieteria = crieteria + " and B_VenderPayment='True'";
                    feild = feild + ",B_VenderPayment";
                    break;
                case "customerpayment":
                    crieteria = crieteria + " and B_CustomerReceipt='True'";
                    feild = feild + ",B_CustomerReceipt";
                    break;
                case "paymentvoucher":
                    crieteria = crieteria + " and B_PaymentVoucher='True'";
                    feild = feild + ",B_PaymentVoucher";
                    break;
                case "receiptvoucher":
                    crieteria = crieteria + " and B_SalaryPayment='True'";
                    feild = feild + ",B_SalaryPayment";
                    break;
                case "salarypayment":
                    crieteria = crieteria + " and B_SalaryPayment='True'";
                    feild = feild + ",B_ReceiptVoucher";
                    break;
                default: break;
            }


            string sqlCommandText = "select " + feild + " from Acc_PaymentMethodMaster where " + crieteria + " order by X_PayMethod";
            Params.Add("@nCompanyID", nCompanyId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(_api.Format(dt)));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }


    }
}