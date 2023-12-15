
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
    [Route("documentCode")]
    [ApiController]
    public class Gen_CodeGenerate : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Gen_CodeGenerate(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            //FormID = 188;
        }


        [HttpGet("code")]
        public ActionResult GetCode(string docNo, int nFnYearID, int formID, string xDescription,int nBranchID,int nDivisionID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    string newCode = "";
                    string masterTable = "";
                    string column = "";
                    if (formID == 188)
                    {
                        masterTable = "Pay_Employee";
                        column = "x_EmpCode";
                    }
                    if (formID == 64 || formID == 1346 || formID == 1665)
                    {
                        masterTable = "Inv_Sales";
                        column = "X_ReceiptNo";                        
                    }
                    if (formID == 53)
                    {
                        masterTable = "Inv_ItemMaster";
                        column = "X_ItemCode";
                    }
                    if (formID == 65)
                    {
                        masterTable = "Inv_Purchase";
                        column = "X_InvoiceNo";
                        //Params.Add("N_BranchID", nBranchID);
                        
                    }
                     if (formID == 155)
                    {
                        masterTable = "Sch_Admission";
                        column = "X_AdmissionNo";
                    }
                   if (formID == 74)
                    {
                        masterTable = "inv_CustomerProjects";
                        column = "X_ProjectCode";
                    }
                   if (formID == 1636)
                    {
                        masterTable = "Inv_Location";
                        column = "X_LocationCode";
                    }
                   if (formID == 1637)
                    {
                        masterTable = "Inv_Location";
                        column = "X_LocationCode";
                    }
                    if (formID == 66)
                    {
                        masterTable = "Inv_PayReceipt";
                        column = "X_VoucherNo";
                    }
                    if (formID == 67)
                    {
                        masterTable = "Inv_PayReceipt";
                        column = "X_VoucherNo";
                    }
                    if(formID==729){
                        masterTable = "Inv_DeliveryNote";
                        column = "x_ReceiptNo";
                    }
                    if(formID==81){
                        masterTable = "Inv_SalesOrder";
                        column = "x_OrderNo";
                    }
                      if(formID==82){
                        masterTable = "Inv_PurchaseOrder";
                        column = "x_PorderNo";
                    }
                        if(formID==1572){
                        masterTable = "Inv_DeliveryNote";
                        column = "x_ReceiptNo";
                    }





                    // if (docNo == "@Auto" || docNo == "new")
                    // {
                        if (formID == 188)
                        {
                            if (xDescription == null || xDescription == "")
                            {
                                Params.Add("N_CompanyID", nCompanyID);
                                Params.Add("N_YearID", nFnYearID);
                                Params.Add("N_FormID", 188);
                                Params.Add("X_Type", "");
                            }
                            else
                            {
                                Params.Add("N_CompanyID", nCompanyID);
                                Params.Add("N_YearID", nFnYearID);
                                Params.Add("N_FormID", 188);
                                Params.Add("X_Type", xDescription);
                            }

                            while (true)
                            {

                                newCode = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                                object N_Result = dLayer.ExecuteScalar("Select 1 from Pay_Employee Where X_EmpCode ='" + newCode + "' and N_CompanyID= " + nCompanyID, Params, connection, transaction);
                                if (N_Result == null)
                                    break;
                            }
                        }
                        else
                        {
                            Params.Add("N_CompanyID", nCompanyID);
                            Params.Add("N_YearID", nFnYearID);
                            Params.Add("N_FormID", formID);
                            Params.Add("N_BranchID", nBranchID);
                            Params.Add("N_DivisionID", nDivisionID);

                            if (formID==1636)
                            {
                                Params.Add("N_TypeId", 6);
                                newCode = dLayer.GetAutoNumberLoc(masterTable, column, Params, connection, transaction);
                            }
                            else if (formID==1637)
                            {
                                Params.Add("N_TypeId", 5);
                                newCode = dLayer.GetAutoNumberLoc(masterTable, column, Params, connection, transaction);
                            }
                            else
                            {
                                newCode = dLayer.GetAutoNumber(masterTable, column, Params, connection, transaction);
                            };
                            if (newCode == "") { transaction.Rollback(); return Ok(_api.Warning("Unable to generate ")); }
                        }

                    // }



                    SortedList output = new SortedList();
                    output.Add("newCode", newCode);


                    return Ok(_api.Success(output));

                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("checkTransaction")]
        public ActionResult GetTransaction(int nCustomerID, int nCompanyID, int nFnYearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    int Count = 0;
                    connection.Open();
                    SortedList Params = new SortedList();
                    Params.Add("N_CompanyID", nCompanyID);
                    string sqlCommand = "";
                    if (nCustomerID > 0)
                    {
                        sqlCommand = "Select count(1) from vw_Inv_CheckCustomer Where N_CompanyID=" + nCompanyID + " and N_CustomerID=" + nCustomerID + "";
                    }
                    Count = myFunctions.getIntVAL(dLayer.ExecuteScalar(sqlCommand, Params, connection).ToString());
                    SortedList output = new SortedList();
                    output.Add("Count", Count);
                    return Ok(_api.Success(output));

                }
            }

            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        
    }
}

