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
    [Route("salaryPayment")]
    [ApiController]
    public class Pay_SalaryPayment : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;

        private readonly string connectionString;
        private readonly int FormID=198;


        public Pay_SalaryPayment(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("batch")]
        public ActionResult GetSalaryPayBatch(int nFnYearID, bool bAllBranchData, int nBranchID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            if (bAllBranchData == false)
                Params.Add("@nBranchID", nBranchID);


            string sqlCommandText = "Select N_CompanyID,N_TransID,N_FnYearID,N_BranchID,TotalSalary,TotalSalaryCollected,X_Batch,X_PayrunText,D_TransDate from vw_PayEmployeeSalaryPaymentsByBatch Where TotalSalary>TotalSalaryCollected and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID " + (bAllBranchData == false ? "and N_BranchID=@nBranchID" : "");
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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

        [HttpGet("payEmpList")]
        public ActionResult GetSalaryPayEmpList(int nFnYearID, bool bAllBranchData, int nBranchID, string xBatchCode)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);


            if (bAllBranchData == false)
                Params.Add("@nBranchID", nBranchID);
            if (xBatchCode == null || xBatchCode == "")
                xBatchCode = "";
            string sqlCommandText = "";
            if (xBatchCode == "")
            {
                sqlCommandText = "select * from vw_PayEmployeeSalaryPaymentsByEmployeeGroup where  N_CompanyID=@nCompanyID ";
            }
            else
            {
                Params.Add("@xBatchCode", xBatchCode);
                sqlCommandText = "select * from vw_PayEmployeeSalaryPaymentsByEmployee where x_Batch = @xBatchCode and TotalSalaryCollected<>TotalSalary and N_CompanyID=@nCompanyID ";


            }


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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

        [HttpGet("batchDetails")]
        public ActionResult GetTransDetails(int nFnYearID, int nReceiptID, int nEmpID, int nBatchID, string xPaymentID, bool showDueOnly, DateTime transDate)
        {
            string sql1 = "";
            SortedList Params = new SortedList();
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nReceiptID", nReceiptID);
            Params.Add("@nEmpID", nEmpID);
            Params.Add("@nBatchID", nBatchID);
            Params.Add("@transDate", transDate);
            Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
            DataTable dt;
            string[] temp = new string[10];
            if (xPaymentID.ToString() != null || xPaymentID.ToString() != "")
            {
                temp = xPaymentID.ToString().Split(',');
            }


            for (int j = 0; j < temp.Length; j++)
            {
                // if (sql1 != "")
                //     sql1 = sql1 + " UNION ALL ";

                if (nReceiptID > 0)
                {

                    // sql1 = sql1 + " Select * from vw_SalaryPaid_Disp where N_PaymentID=@nReceiptID";
                    sql1 = " Select * from vw_SalaryPaid_Disp where N_PaymentID=@nReceiptID";
                }
                else
                {
                    string X_DueCondition = "";
                    string X_Condition = "";
                    if (nEmpID == 0 && nBatchID != 0)
                        X_Condition = "dbo.vw_PayAmountDetailsForPay.N_TransID =@nBatchID and dbo.vw_PayAmountDetailsForPay.N_CompanyID =@nCompanyID and vw_PayAmountDetailsForPay.N_PaymentId=" + temp[j] + " and vw_PayAmountDetailsForPay.N_Entryfrom=190";
                    else if (nBatchID == 0 && nEmpID != 0)
                        X_Condition = "dbo.vw_PayAmountDetailsForPay.N_EmpID =@nEmpID and dbo.vw_PayAmountDetailsForPay.N_CompanyID =@nCompanyID and vw_PayAmountDetailsForPay.N_PaymentId=" + temp[j];
                    else
                        X_Condition = "dbo.vw_PayAmountDetailsForPay.N_TransID =@nBatchID and dbo.vw_PayAmountDetailsForPay.N_EmpID =@nEmpID and dbo.vw_PayAmountDetailsForPay.N_CompanyID =@nCompanyID and vw_PayAmountDetailsForPay.N_PaymentId=" + temp[j];

                    if (showDueOnly == true)
                        X_DueCondition = " And dbo.vw_PayAmountDetailsForPay.D_TransDate <= '" + transDate + "' ";

                    sql1 = " SELECT     dbo.vw_PayAmountDetailsForPay.N_TransID,dbo.vw_PayAmountDetailsForPay.N_PayrunID,dbo.vw_PayAmountDetailsForPay.D_TransDate,dbo.vw_PayAmountDetailsForPay.X_PayrunText,ABS(dbo.vw_PayAmountDetailsForPay.N_PayRate) AS N_PayRate,ABS(dbo.vw_PayAmountDetailsForPay.N_PayRate)-(sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Amount ,0))+ sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Discount,0))) As N_InvoiceDueAmt,ABS(dbo.vw_PayAmountDetailsForPay.N_PayRate)-(sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Amount ,0))+ sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Discount,0))) As N_DueAmount,0 As N_Amount,0 As N_Discount,vw_PayAmountDetailsForPay.N_Entryfrom,vw_PayAmountDetailsForPay.X_Description,vw_PayAmountDetailsForPay.N_PayTypeID,vw_PayAmountDetailsForPay.N_PaymentId,vw_PayAmountDetailsForPay.X_EmpName,vw_PayAmountDetailsForPay.N_EmpID,vw_PayAmountDetailsForPay.X_Batch,vw_PayAmountDetailsForPay.X_EmpCode" +
                                  " FROM         dbo.vw_PayAmountDetailsForPay " +
                                  " LEFT OUTER JOIN vw_PayEmployeePaidTotal On dbo.vw_PayAmountDetailsForPay.N_TransID  =dbo.vw_PayEmployeePaidTotal.N_SalesID and dbo.vw_PayAmountDetailsForPay.N_EmpID =dbo.vw_PayEmployeePaidTotal.N_AdmissionID and dbo.vw_PayAmountDetailsForPay.N_CompanyID =dbo.vw_PayEmployeePaidTotal.N_CompanyID and dbo.vw_PayEmployeePaidTotal.N_Entryfrom = dbo.vw_PayAmountDetailsForPay.N_EntryFrom and dbo.vw_PayEmployeePaidTotal.N_PayTypeID = dbo.vw_PayAmountDetailsForPay.N_PayTypeID" +
                                  " Where ISNULL(dbo.vw_PayAmountDetailsForPay.B_IsSaveDraft,0)=0 and " + X_Condition + " " + X_DueCondition + " " +
                                  " group by     dbo.vw_PayAmountDetailsForPay.N_TransID,dbo.vw_PayAmountDetailsForPay.N_PayrunID,dbo.vw_PayAmountDetailsForPay.D_TransDate,dbo.vw_PayAmountDetailsForPay.X_PayrunText,dbo.vw_PayAmountDetailsForPay.N_PayRate,vw_PayAmountDetailsForPay.N_Entryfrom,vw_PayAmountDetailsForPay.X_Description,vw_PayAmountDetailsForPay.N_PayTypeID,vw_PayAmountDetailsForPay.N_PaymentId,vw_PayAmountDetailsForPay.X_EmpName,vw_PayAmountDetailsForPay.N_EmpID,vw_PayAmountDetailsForPay.X_Batch,vw_PayAmountDetailsForPay.X_EmpCode" +
                                  " having  (ABS(dbo.vw_PayAmountDetailsForPay.N_Payrate)-(sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Amount ,0))+ sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Discount,0))) > 0) Order By dbo.vw_PayAmountDetailsForPay.D_TransDate";
                }
            }
            if (sql1 == "") { return Ok(_api.Notice("No Results Found")); }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sql1, Params, connection);
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



        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {



                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nReceiptID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ReceiptID"].ToString());
                    int nAcYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_AcYearID"].ToString());
                    string X_ReceiptNo = MasterTable.Rows[0]["x_ReceiptNo"].ToString();
                    int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                    // QueryParams.Add("@nCompanyID", N_CompanyID);
                    // QueryParams.Add("@nFnYearID", N_FnYearID);
                    // QueryParams.Add("@nReceiptID", N_ReceiptID);
                    // QueryParams.Add("@nBranchID", N_BranchID);

                    if (nReceiptID > 0)
                    {
                        SortedList deleteParams = new SortedList()
                            {
                                {"N_CompanyID",nCompanyID},

                                {"N_ReceiptidId",nReceiptID}
                            };
                        dLayer.DeleteData("Pay_EmployeePayment", "nReceiptID", nReceiptID, "", connection, transaction);
                    }

                    DocNo = MasterRow["x_ReceiptNo"].ToString();
                    if (X_ReceiptNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID",FormID);
                       Params.Add("N_YearID", nAcYearID);       

                        while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Pay_EmployeePayment Where X_ReceiptNo ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_ReceiptNo = DocNo;


                        if (X_ReceiptNo == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate")); }
                        MasterTable.Rows[0]["x_ReceiptNo"] = X_ReceiptNo;

                    }
                    else
                    {
                        dLayer.DeleteData("Pay_EmployeePayment", "N_ReceiptId", nReceiptID, "", connection, transaction);
                    }



                    nReceiptID = dLayer.SaveData("Pay_EmployeePayment", "N_ReceiptId", MasterTable, connection, transaction);
                    if (nReceiptID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    }

                    for (int i = 0; i < DetailTable.Rows.Count; i++)
                    {
                        DetailTable.Rows[0]["N_ReceiptID"] = nReceiptID;
                    }
                    int nReceiptDetailsID = dLayer.SaveData("Pay_EmployeePaymentDetails", "N_ReceiptDetailsID", DetailTable, connection, transaction);
                    if (nReceiptDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    }

                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
    }
}

// [HttpPost("save")]
// public ActionResult SaveData([FromBody] DataSet ds)
// {

//     double N_TotalAmountPAid = 0;
//     string X_TotalAmountPAid = "";



//     N_TotalAmountPAid = ReturnTotal();
//     ToWord toWord = new ToWord(Convert.ToDecimal(N_TotalAmountPAid.ToString(myCompanyID.DecimalPlaceString)), new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
//     X_TotalAmountPAid = toWord.ConvertToArabic();

//     bool B_Completed = true;
//     N_BranchId = myCompanyID._BranchID;
//     dba.SetTransaction();
//     try
//     {
//         if (txtReference.Text == "@Auto")
//         {
//             if (!B_AutoInvoice)
//             {
//                 dba.Rollback();
//                 msg.msgInformation(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "AutoInvoice"));
//                 return;
//             }
//             while (true)
//             {
//                 txtReference.Text = dba.ExecuteSclarNoErrorCatch("SP_AutoNumberGenerate " + myCompanyID._CompanyID + "," + myCompanyID._FnYearID + "," + MYG.ReturnFormID(this.Text), "TEXT", new DataTable()).ToString();
//                 object N_Result = dba.ExecuteSclarNoErrorCatch("Select 1 from Pay_EmployeePayment Where N_CompanyID = " + myCompanyID._CompanyID + " and X_ReceiptNo = '" + txtReference.Text.Trim() + "' and N_AcYearID=" + myCompanyID._AcYearID.ToString(), "TEXT", new DataTable());
//                 if (N_Result == null)
//                     break;
//             }
//             if (txtReference.Text == "")
//             {
//                 dba.Rollback();
//                 msg.msgInformation(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "AutoInvoice"));
//                 return;
//             }
//         }
//         if (txtRemarks.Text.ToString() == "Notes" || txtRemarks.Text.ToString() == "�������")
//             txtRemarks.Text = "";


//         {
//             if (!dba.DeleteDataNoTry("Pay_EmployeePaymentDetails", "N_ReceiptID", N_ReceiptID.ToString(), "N_CompanyID=" + myCompanyID._CompanyID)) { dba.Rollback(); return; }
//         }

//         string PaymentMethod = "", BankName = "";
//         string ChequeNo = "", CheQueDate = "";
//         if (N_PaymentModeID == 4) PaymentMethod = "Cash";
//         else if (N_PaymentModeID == 6) PaymentMethod = "Credit Card";
//         else
//         {
//             PaymentMethod = "Bank";
//         }
//         BankName = txtDefaultAccount.Text;

//         if (txtChequeNo.Text.Trim() == "Cheque No.")
//             ChequeNo = "";
//         else
//             ChequeNo = txtChequeNo.Text.Trim();
//         CheQueDate = myFunctions.getDateVAL(dtpChequeDate.Value);

//         X_BtnAction = "Save";
//         if (N_ReceiptID > 0)
//         {
//             X_BtnAction = "Update";
//         }

//         object Result = 0;

//         string FieldList = "N_CompanyID,N_AcYearID,X_ReceiptNo,D_ReceiptDate,N_AdmissionID,N_userID,X_PaymentMethod,X_ChequeNo,D_ChequeDate,X_BankName,X_Remarks,N_TotalAmount,X_TotalAmount_Ar,N_BranchID,N_TransID";
//         string FieldValues = myCompanyID._CompanyID + "|" + myCompanyID._FnYearID.ToString() + "|'" + txtReference.Text + "'|'" + myFunctions.getDateVAL(dtpDate.Value) + "'|" + N_AdmissionID.ToString() + "|" + myCompanyID._UserID + "|'" + txtPaymentMode.Text.Trim() + "'|'" + ChequeNo + "'|'" + CheQueDate + "'|'" + BankName + "'|'" + txtRemarks.Text + "'|" + N_TotalAmountPAid + "|'" + X_TotalAmountPAid + "'|" + N_BranchId + "|" + N_BatchId;

//         myFunctions.saveApprovals(ref dba, ref FieldList, ref FieldValues, N_NextApprovalLevel, myFunctions.getIntVAL(btnSave.Tag.ToString()), N_SaveDraft, N_IsApprovalSystem, myFunctions.getIntVAL(MYG.ReturnFormID(this.Text)), 0);

//         string DupCriteria = "N_CompanyID = " + myCompanyID._CompanyID + " and X_ReceiptNo = '" + txtReference.Text.Trim() + "' and N_AcYearID=" + myCompanyID._AcYearID.ToString();

//         string RefFieldList = "N_DefLedgerID,N_PaymentMethodID";
//         string RefFileldDescr = "Acc_MastLedger|N_LedgerID|X_LedgerCode='" + X_DefLedgerCode + "' and N_CompanyID =" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID + "|Acc_PaymentMethodMaster|N_PaymentMethodID|X_PayMethod  ='" + txtPaymentMode.Text.Trim() + "' and N_CompanyID=" + myCompanyID._CompanyID;

//         dba.SaveData(ref Result, "Pay_EmployeePayment", "N_ReceiptID", N_ReceiptID.ToString(), FieldList, FieldValues, RefFieldList, RefFileldDescr, DupCriteria, "");
//         double N_Amount = 0;
//         int N_LoanFlag = 0;
//         if (myFunctions.getIntVAL(Result.ToString()) > 0)
//         {
//             N_ReceiptID = myFunctions.getIntVAL(Result.ToString());

//             myFunctions.logApprovals(ref dba, X_TransType, N_ReceiptID, myFunctions.getIntVAL(MYG.ReturnFormID(this.Text)), X_Action, txtReference.Text, DateTime.Now, N_NextApprovalLevel, myCompanyID._UserID, myFunctions.getIntVAL(btnSave.Tag.ToString()), N_IsApprovalSystem, "", 0);

//             N_SaveDraft = myFunctions.getIntVAL(dba.ExecuteSclarNoErrorCatch("select CAST(B_IsSaveDraft as INT) from Pay_EmployeePayment where N_CompanyID=" + myCompanyID._CompanyID + " and N_ReceiptID=" + N_ReceiptID, "TEXT", new DataTable()).ToString());

//             dba.ExecuteNonQuery("SP_Log_SysActivity " + myCompanyID._CompanyID.ToString() + "," + myCompanyID._FnYearID.ToString() + "," + N_ReceiptID + "," + myFunctions.getIntVAL(MYG.ReturnFormID(this.Text).ToString()) + "," + myCompanyID._UserID + ",'" + X_BtnAction + "','" + myCompanyID._SystemName + "','" + Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString() + "','" + txtReference.Text.Trim() + "',''", "TEXT", new DataTable());
//             X_BtnAction = "";
//             for (int i = 1; i < flxPurchase.Rows; i++)
//             {
//                 if (flxPurchase.get_TextMatrix(i, mcPay) == "P")
//                 {
//                     Result = 0;

//                     FieldList = "N_CompanyID,N_AcYearID,N_ReceiptID,N_SalesID,N_amount,N_Discount,N_BranchID,N_Entryfrom,N_PayTypeID,N_PaymentID,N_EmpId,X_Description";
//                     FieldValues = myCompanyID._CompanyID + "|" + myCompanyID._FnYearID.ToString() + "|" + N_ReceiptID + "|" + flxPurchase.get_TextMatrix(i, mcPayrunID) + "|" + myFunctions.getVAL(flxPurchase.get_TextMatrix(i, mcAmount)).ToString() + "|" + myFunctions.getVAL(flxPurchase.get_TextMatrix(i, mcDisount)).ToString() + "|" + N_BranchId + "|" + myFunctions.getIntVAL(flxPurchase.get_TextMatrix(i, mcEntryFrom)) + "|" + myFunctions.getIntVAL(flxPurchase.get_TextMatrix(i, mcPayTypeID)) + "|" + myFunctions.getIntVAL(flxPurchase.get_TextMatrix(i, mcPaymentID)) + "|" + myFunctions.getIntVAL(flxPurchase.get_TextMatrix(i, mcEmpId)) + "|'" + flxPurchase.get_TextMatrix(i, mcDescription).ToString() + "'";
//                     DupCriteria = "";

//                     RefFieldList = "";
//                     RefFileldDescr = "";
//                     dba.SaveData(ref Result, "Pay_EmployeePaymentDetails", "N_ReceiptDetailsID", "0", FieldList, FieldValues, RefFieldList, RefFileldDescr, DupCriteria, "", "N_CompanyID=" + myCompanyID._CompanyID + " and N_AcYearID=" + myCompanyID._FnYearID.ToString());

//                     if (myFunctions.getIntVAL(Result.ToString()) <= 0)
//                     {
//                         B_Completed = false;
//                         break;
//                     }
//                     if (myFunctions.getIntVAL(flxPurchase.get_TextMatrix(i, mcEntryFrom)) == 212)
//                         N_LoanFlag = 1;
//                 }
//             }
//             if (B_Completed)
//             {
//                 if (N_SaveDraft == 0)
//                 {
//                     for (int i = 1; i < flxPurchase.Rows; i++)
//                     {
//                         if (flxPurchase.get_TextMatrix(i, mcPay) == "P")
//                         {
//                             if (myFunctions.getIntVAL(flxPurchase.get_TextMatrix(i, mcEntryFrom)) == 212)
//                                 dba.ExecuteNonQuery("SP_Pay_SalaryPaid_Voucher_Del " + myCompanyID._CompanyID + ",'ELI','" + txtReference.Text.Trim() + "'," + N_FnYearID + "", "TEXT", new DataTable());
//                             else
//                                 dba.ExecuteNonQuery("SP_Pay_SalaryPaid_Voucher_Del " + myCompanyID._CompanyID + ",'ESP','" + txtReference.Text.Trim() + "'," + N_FnYearID + "", "TEXT", new DataTable());
//                         }
//                     }

//                     dba.ExecuteNonQuery("SP_Pay_SalaryPaid_Voucher_Ins " + myCompanyID._CompanyID.ToString() + "," + N_ReceiptID.ToString() + "," + myCompanyID._UserID.ToString() + ",'" + this.Text + "','" + System.Environment.MachineName + "'," + N_LoanFlag, "TEXT", new DataTable());
//                 }
//                 dba.Commit();
//                 PrintCheque();
//                 PrintVoucher();
//                 InvoiceSearchSettings();

//             }
//             else
//             {
//                 dba.Rollback();
//                 MYG.ResultMessage(lblResult, lblResultDescr, "Error!", MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "UnExpectedErr"));
//             }
//         }
//         else
//         {
//             dba.Rollback();
//             MYG.ResultMessage(lblResult, lblResultDescr, "Alert!", MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "Duplicatememo"));
//         }
//         Cursor.Current = Cursors.Default;

//     }
//     catch (Exception ex)
//     {
//         dba.Rollback();
//         MYG.ResultMessage(lblResult, lblResultDescr, "Error!", ex.Message);
//     }
// }



