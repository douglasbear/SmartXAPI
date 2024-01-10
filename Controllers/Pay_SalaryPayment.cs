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
using System.Net;

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
        private readonly int FormID = 198;


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



            string sqlCommandText = "Select N_CompanyID,N_TransID,N_FnYearID,N_BranchID,TotalSalary,TotalSalaryCollected,X_Batch,X_PayrunText,D_TransDate from vw_PayEmployeeSalaryPaymentsByBatch Where TotalSalary>TotalSalaryCollected and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID";
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
                return Ok(_api.Error(User,e));
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
                sqlCommandText = "select N_CompanyID,N_EmpID,X_EmpCode,N_BranchID,X_EmpName  from vw_PayEmployeeSalaryPaymentsByEmployeeGroup where  N_CompanyID=@nCompanyID Group By N_CompanyID,N_EmpID,X_EmpCode,N_BranchID,X_EmpName ";
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
                return Ok(_api.Error(User,e));
            }
        }

        [HttpGet("batchDetails")]
        public ActionResult GetTransDetails(int nFnYearID, int nReceiptID, int nEmpID, int nBatchID, string xPaymentID, bool showDueOnly, DateTime transDate)
        {
            string sql1 = "";
            string sql2 = "";
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
                if (sql1 != "")
                    sql2 = sql1;
                //sql1 = sql1 + " UNION ALL ";

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
                    if (sql1 != "")
                    {
                        sql1 = " SELECT     dbo.vw_PayAmountDetailsForPay.N_TransID,dbo.vw_PayAmountDetailsForPay.N_PayrunID,dbo.vw_PayAmountDetailsForPay.D_TransDate,dbo.vw_PayAmountDetailsForPay.X_PayrunText,ABS(dbo.vw_PayAmountDetailsForPay.N_PayRate) AS N_PayRate,ABS(dbo.vw_PayAmountDetailsForPay.N_PayRate)-(sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Amount ,0))+ sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Discount,0))) As N_InvoiceDueAmt,ABS(dbo.vw_PayAmountDetailsForPay.N_PayRate)-(sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Amount ,0))+ sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Discount,0))) As N_DueAmount,0 As N_Amount,0 As N_Discount,vw_PayAmountDetailsForPay.N_Entryfrom,vw_PayAmountDetailsForPay.X_Description,vw_PayAmountDetailsForPay.N_PayTypeID,vw_PayAmountDetailsForPay.N_PaymentId,vw_PayAmountDetailsForPay.X_EmpName,vw_PayAmountDetailsForPay.N_EmpID,vw_PayAmountDetailsForPay.X_Batch,vw_PayAmountDetailsForPay.X_EmpCode" +
                                   " FROM         dbo.vw_PayAmountDetailsForPay " +
                                   " LEFT OUTER JOIN vw_PayEmployeePaidTotal On dbo.vw_PayAmountDetailsForPay.N_TransID  =dbo.vw_PayEmployeePaidTotal.N_SalesID and dbo.vw_PayAmountDetailsForPay.N_EmpID =dbo.vw_PayEmployeePaidTotal.N_AdmissionID and dbo.vw_PayAmountDetailsForPay.N_CompanyID =dbo.vw_PayEmployeePaidTotal.N_CompanyID and dbo.vw_PayEmployeePaidTotal.N_Entryfrom = dbo.vw_PayAmountDetailsForPay.N_EntryFrom and dbo.vw_PayEmployeePaidTotal.N_PayTypeID = dbo.vw_PayAmountDetailsForPay.N_PayTypeID" +
                                   " Where ISNULL(dbo.vw_PayAmountDetailsForPay.B_IsSaveDraft,0)=0 and " + X_Condition + " " + X_DueCondition + " " +
                                   " group by     dbo.vw_PayAmountDetailsForPay.N_TransID,dbo.vw_PayAmountDetailsForPay.N_PayrunID,dbo.vw_PayAmountDetailsForPay.D_TransDate,dbo.vw_PayAmountDetailsForPay.X_PayrunText,dbo.vw_PayAmountDetailsForPay.N_PayRate,vw_PayAmountDetailsForPay.N_Entryfrom,vw_PayAmountDetailsForPay.X_Description,vw_PayAmountDetailsForPay.N_PayTypeID,vw_PayAmountDetailsForPay.N_PaymentId,vw_PayAmountDetailsForPay.X_EmpName,vw_PayAmountDetailsForPay.N_EmpID,vw_PayAmountDetailsForPay.X_Batch,vw_PayAmountDetailsForPay.X_EmpCode" +
                                   " having  (ABS(dbo.vw_PayAmountDetailsForPay.N_Payrate)-(sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Amount ,0))+ sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Discount,0))) > 0)  UNION ALL "+sql2+" ";
                    }
                    else
                    {
                        sql1 = " SELECT     dbo.vw_PayAmountDetailsForPay.N_TransID,dbo.vw_PayAmountDetailsForPay.N_PayrunID,dbo.vw_PayAmountDetailsForPay.D_TransDate,dbo.vw_PayAmountDetailsForPay.X_PayrunText,ABS(dbo.vw_PayAmountDetailsForPay.N_PayRate) AS N_PayRate,ABS(dbo.vw_PayAmountDetailsForPay.N_PayRate)-(sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Amount ,0))+ sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Discount,0))) As N_InvoiceDueAmt,ABS(dbo.vw_PayAmountDetailsForPay.N_PayRate)-(sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Amount ,0))+ sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Discount,0))) As N_DueAmount,0 As N_Amount,0 As N_Discount,vw_PayAmountDetailsForPay.N_Entryfrom,vw_PayAmountDetailsForPay.X_Description,vw_PayAmountDetailsForPay.N_PayTypeID,vw_PayAmountDetailsForPay.N_PaymentId,vw_PayAmountDetailsForPay.X_EmpName,vw_PayAmountDetailsForPay.N_EmpID,vw_PayAmountDetailsForPay.X_Batch,vw_PayAmountDetailsForPay.X_EmpCode" +
                                      " FROM         dbo.vw_PayAmountDetailsForPay " +
                                      " LEFT OUTER JOIN vw_PayEmployeePaidTotal On dbo.vw_PayAmountDetailsForPay.N_TransID  =dbo.vw_PayEmployeePaidTotal.N_SalesID and dbo.vw_PayAmountDetailsForPay.N_EmpID =dbo.vw_PayEmployeePaidTotal.N_AdmissionID and dbo.vw_PayAmountDetailsForPay.N_CompanyID =dbo.vw_PayEmployeePaidTotal.N_CompanyID and dbo.vw_PayEmployeePaidTotal.N_Entryfrom = dbo.vw_PayAmountDetailsForPay.N_EntryFrom and dbo.vw_PayEmployeePaidTotal.N_PayTypeID = dbo.vw_PayAmountDetailsForPay.N_PayTypeID" +
                                      " Where ISNULL(dbo.vw_PayAmountDetailsForPay.B_IsSaveDraft,0)=0 and " + X_Condition + " " + X_DueCondition + " " +
                                      " group by     dbo.vw_PayAmountDetailsForPay.N_TransID,dbo.vw_PayAmountDetailsForPay.N_PayrunID,dbo.vw_PayAmountDetailsForPay.D_TransDate,dbo.vw_PayAmountDetailsForPay.X_PayrunText,dbo.vw_PayAmountDetailsForPay.N_PayRate,vw_PayAmountDetailsForPay.N_Entryfrom,vw_PayAmountDetailsForPay.X_Description,vw_PayAmountDetailsForPay.N_PayTypeID,vw_PayAmountDetailsForPay.N_PaymentId,vw_PayAmountDetailsForPay.X_EmpName,vw_PayAmountDetailsForPay.N_EmpID,vw_PayAmountDetailsForPay.X_Batch,vw_PayAmountDetailsForPay.X_EmpCode" +
                                      " having  (ABS(dbo.vw_PayAmountDetailsForPay.N_Payrate)-(sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Amount ,0))+ sum(Isnull(dbo.vw_PayEmployeePaidTotal.N_Discount,0))) > 0) Order By dbo.vw_PayAmountDetailsForPay.D_TransDate";
                    }
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
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetDetails(string xReceiptNo, int nBranchID, bool bAllBranchData)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    Params.Add("@xReceiptNo", xReceiptNo);
                    Params.Add("@nBranchID", nBranchID);
                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));

                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable Details = new DataTable();
                    string Mastersql = "";
                    //string DetailSql = "";
                    string DetailGetSql = "";
                    string xCondition = "";

                    if (bAllBranchData)
                        xCondition = "X_ReceiptNo=@xReceiptNo and N_CompanyId=@nCompanyID";
                    else
                        xCondition = "X_ReceiptNo=@xReceiptNo and N_CompanyId=@nCompanyID and N_BranchID=@nBranchID";

                    Mastersql = "select * from vw_EmppaymentMaster where " + xCondition;

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    MasterTable = _api.Format(MasterTable, "Master");
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int nReceiptID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ReceiptID"].ToString());
                    // DateTime dTransdate = Convert.ToDateTime(MasterTable.Rows[0]["D_ReceiptDate"].ToString());
                    Params.Add("@nReceiptID", nReceiptID);

                    DetailGetSql = "Select N_PaymentID,X_TypeName from Pay_EmployeePaymentDetails Inner Join Gen_Defaults ON Pay_EmployeePaymentDetails.N_PaymentID=Gen_Defaults.N_TypeId and Gen_Defaults.N_DefaultId=2 Where  Pay_EmployeePaymentDetails.N_CompanyID=@nCompanyID and Pay_EmployeePaymentDetails.N_ReceiptID=@nReceiptID";
                    Details = dLayer.ExecuteDataTable(DetailGetSql, Params, connection);
                    // Details = _api.Format(Details, "Details");
                    string xPaymentID = "";
                    for (int j = 0; j < Details.Rows.Count; j++)
                    {
                        if (xPaymentID == "")
                            xPaymentID = Details.Rows[0]["N_PaymentID"].ToString();
                        else
                            xPaymentID = xPaymentID + "," + Details.Rows[0]["N_PaymentID"].ToString();

                        string X_TypeName = Details.Rows[0]["X_TypeName"].ToString();
                    }

                    //DetailTable=GetTransDetails(0,nReceiptID, 0, 0,xPaymentID, true, null);

                    string[] temp = new string[10];
                    if (xPaymentID.ToString() != null || xPaymentID.ToString() != "")
                    {
                        temp = xPaymentID.ToString().Split(',');
                    }

                    // string X_Condition="";
                    // string X_DueCondition="";
                    string sql1 = "";

                    for (int j = 0; j < temp.Length; j++)
                    {
                        sql1 = "Select N_TransID as N_SalesID,N_DueAmount as N_TotalDueAmount,N_InvoiceDueAmt as N_DueAmount,* from vw_SalaryPaid_Disp where N_ReceiptID=@nReceiptID  and N_CompanyID =@nCompanyID  and N_PaymentId =" + temp[j] + "";



                    }
                    DetailTable = dLayer.ExecuteDataTable(sql1, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");

                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);

                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpGet("dashboardList")]
        public ActionResult SalaryPayList(int nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, int nFnYearID,bool bAllBranchData,int nBranchID)
        {
            //int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Criteria = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([Receipt No] like '%" + xSearchkey + "%' or [Employee Name] like '%" + xSearchkey + "%' or cast(Date as Varchar) like '%" + xSearchkey + "%' or X_PaymentMethod like '%" + xSearchkey + "%' or X_BankName like '%" + xSearchkey + "%' or N_TotalAmount like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ReceiptID desc";
           else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "receiptNo":
                                xSortBy = "N_ReceiptID " + xSortBy.Split(" ")[1];
                                break;
                            case "date":
                                xSortBy = "Cast(Date as DateTime ) " + xSortBy.Split(" ")[1];
                                break;
                          
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }
                      if (bAllBranchData == true)
                        {
                            Searchkey = Searchkey + " ";
                        }
                        else
                        {
                            Searchkey = Searchkey + " and N_BranchID=" + nBranchID + " ";
                        }    


            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_PayEmployeePayment_Search where N_CompanyID=@nCompanyId and n_AcYearID=@nFnYearID " + Searchkey + Criteria + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_PayEmployeePayment_Search where N_CompanyID=@nCompanyId and n_AcYearID=@nFnYearID " + Searchkey + Criteria + " and N_ReceiptID not in (select top(" + Count + ") N_ReceiptID from vw_PayEmployeePayment_Search where N_CompanyID=@nCompanyId and n_AcYearID=@nFnYearID" + Criteria + xSortBy + " ) " + xSortBy;
            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nFnYearID", nFnYearID);
            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(1) as N_Count  from vw_PayEmployeePayment_Search where N_CompanyID=@nCompanyId and n_AcYearID=@nFnYearID " ;
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


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID, string X_ReceiptNo, int nAcYearID, int nReceiptId)
        {
            int Results = 0;
            int Results1 = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    SortedList PostingDelParam = new SortedList();
                    SortedList Params = new SortedList();
                    SortedList PostingDelParam1 = new SortedList();
                    String detailSql = "";
                    DataTable DetailTable = new DataTable();
                      SortedList ParamList = new SortedList();
                    DataTable TransData = new DataTable();
                    ParamList.Add("@nTransID", nReceiptId);
                    ParamList.Add("@nAcYearID", nAcYearID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    string xButtonAction="Delete";
                   // string X_ReceiptNo="";
                    Params.Add("@nCompanyID", nCompanyID);

                    PostingDelParam.Add("N_CompanyID", nCompanyID);
                    PostingDelParam.Add("X_TransType", "ELI");
                    PostingDelParam.Add("X_ReferenceNo", X_ReceiptNo);
                    PostingDelParam.Add("N_FnYearID", nAcYearID);

                    PostingDelParam1.Add("N_CompanyID", nCompanyID);
                    PostingDelParam1.Add("X_TransType", "ESP");
                    PostingDelParam1.Add("X_ReferenceNo", X_ReceiptNo);
                    PostingDelParam1.Add("N_FnYearID", nAcYearID);

                    detailSql = "select * from Pay_EmployeePaymentDetails where N_ReceiptID=" + nReceiptId + " ";
                    DetailTable = dLayer.ExecuteDataTable(detailSql, Params, connection);
                    SqlTransaction transaction = connection.BeginTransaction();
                   ;
                     string Sql = "select n_ReceiptID,X_ReceiptNo from Pay_EmployeePayment where n_ReceiptID=@nTransID and N_CompanyID=@nCompanyID ";
                      TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection,transaction);
                     
                      if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];
                                        //  Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(myFunctions.getIntVAL( nAcYearID.ToString()),nReceiptId,TransRow["X_ReceiptNo"].ToString(),198,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
             


                    for (int i = DetailTable.Rows.Count - 1; i >= 0; i--)
                    {


                        if (myFunctions.getIntVAL(DetailTable.Rows[0]["n_Entryfrom"].ToString()) == 212)
                        {



                            try
                            {
                                Results = dLayer.ExecuteNonQueryPro("SP_Pay_SalaryPaid_Voucher_Del", PostingDelParam, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User,ex));
                            }
                        }

                        else
                        {


                            try
                            {
                                Results = dLayer.ExecuteNonQueryPro("SP_Pay_SalaryPaid_Voucher_Del", PostingDelParam1, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User,ex));
                            }

                        }
                        // if (Results <= 0)
                        // {
                        //     transaction.Rollback();
                        //     return Ok(_api.Error(User,"Unable to delete "));
                        // }

                    }
                    Results1 = dLayer.DeleteData("Pay_EmployeePaymentDetails", "N_ReceiptID", nReceiptId, "", connection, transaction);
                    if (Results1 > 0)
                    {
                        dLayer.DeleteData("Pay_EmployeePayment", "N_ReceiptID", nReceiptId, "", connection, transaction);
                        transaction.Commit();
                        return Ok(_api.Success(" deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete"));
                    }

                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
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
                    int nLoanFlag = 0;
                     string xButtonAction="";
                    // QueryParams.Add("@nCompanyID", N_CompanyID);
                    // QueryParams.Add("@nFnYearID", N_FnYearID);
                    // QueryParams.Add("@nReceiptID", N_ReceiptID);
                    // QueryParams.Add("@nBranchID", N_BranchID);

                    if (nReceiptID > 0)
                    {
                        // SortedList deleteParams = new SortedList()
                        //     {
                        //         {"N_CompanyID",nCompanyID},

                        //         {"N_ReceiptId",nReceiptID}
                        //     };
                        dLayer.DeleteData("Pay_EmployeePaymentDetails", "N_ReceiptId", nReceiptID, "N_CompanyID = " + nCompanyID, connection, transaction);
                        xButtonAction="Update"; 
                    }

                    DocNo = MasterRow["x_ReceiptNo"].ToString();
                    if (X_ReceiptNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", FormID);
                        Params.Add("N_YearID", nAcYearID);
                      

                        while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                             xButtonAction="Insert"; 
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Pay_EmployeePayment Where X_ReceiptNo ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                           
                            if (N_Result == null)
                                break;
                        }
                        X_ReceiptNo = DocNo;
                        

                        if (X_ReceiptNo == "") { transaction.Rollback(); return Ok(_api.Error(User,"Unable to generate")); }
                        MasterTable.Rows[0]["x_ReceiptNo"] = X_ReceiptNo;
                        
                    }
                     X_ReceiptNo = MasterTable.Rows[0]["x_ReceiptNo"].ToString();
           


                    // else
                    // {
                    //     dLayer.DeleteData("Pay_EmployeePayment", "N_ReceiptId", nReceiptID, "", connection, transaction);
                    // }
                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_AcYearID=" + nAcYearID + " and X_ReceiptNo='" + X_ReceiptNo + "'";
                    string X_Criteria = "N_CompanyID=" + nCompanyID + " and N_AcYearID=" + nAcYearID;
                //      string ipAddress = "";
                // if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                //     ipAddress = Request.Headers["X-Forwarded-For"];
                // else
                //     ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                //        myFunctions.LogScreenActivitys(nAcYearID,nReceiptID,X_ReceiptNo,198,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
          

                    nReceiptID = dLayer.SaveData("Pay_EmployeePayment", "N_ReceiptId", DupCriteria, X_Criteria, MasterTable, connection, transaction);
                    if (nReceiptID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable To Save"));
                    }

                    for (int i = DetailTable.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow mstVar = DetailTable.Rows[i];
                        double Amount = myFunctions.getVAL(mstVar["n_Amount"].ToString());
                        if (Amount == 0)
                        {
                            DetailTable.Rows[i].Delete();
                            continue;
                        }
                        if (myFunctions.getIntVAL(DetailTable.Rows[0]["n_Entryfrom"].ToString()) == 212)
                            nLoanFlag = 1;

                        DetailTable.Rows[i]["N_ReceiptID"] = nReceiptID;

                    }
           

                    int nReceiptDetailsID = dLayer.SaveData("Pay_EmployeePaymentDetails", "N_ReceiptDetailsID", DetailTable, connection, transaction);
                    if (nReceiptDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Pay not selected"));
                    }

           

                    if (myFunctions.getIntVAL(MasterTable.Rows[0]["b_IsSaveDraft"].ToString()) == 0)
                    {
                        for (int i = DetailTable.Rows.Count - 1; i >= 0; i--)
                        {
                            SortedList PostingDelParam = new SortedList();

                            if (myFunctions.getIntVAL(DetailTable.Rows[0]["n_Entryfrom"].ToString()) == 212)
                            {
                                PostingDelParam.Add("N_CompanyID", nCompanyID);
                                PostingDelParam.Add("X_TransType", "ELI");
                                PostingDelParam.Add("X_ReferenceNo", X_ReceiptNo);
                                PostingDelParam.Add("N_FnYearID", nAcYearID);
                                try
                                {
                                    dLayer.ExecuteNonQueryPro("SP_Pay_SalaryPaid_Voucher_Del", PostingDelParam, connection, transaction);
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User,ex));
                                }

                            }
                            else
                            {
                                PostingDelParam.Add("N_CompanyID", nCompanyID);
                                PostingDelParam.Add("X_TransType", "ESP");
                                PostingDelParam.Add("X_ReferenceNo", X_ReceiptNo);
                                PostingDelParam.Add("N_FnYearID", nAcYearID);
                                try
                                {
                                    dLayer.ExecuteNonQueryPro("SP_Pay_SalaryPaid_Voucher_Del", PostingDelParam, connection, transaction);
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User,ex));
                                }
                            }
                        }

                             //Activity Log
                // string ipAddress = "";
                // if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                //     ipAddress = Request.Headers["X-Forwarded-For"];
                // else
                //     ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                //        myFunctions.LogScreenActivitys(nAcYearID,nReceiptID,X_ReceiptNo,198,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                       
         
                        SortedList PostingParam = new SortedList();
                        PostingParam.Add("N_CompanyID", nCompanyID);
                        PostingParam.Add("N_ReceiptID", nReceiptID);
                        PostingParam.Add("N_UserId", myFunctions.GetUserID(User));
                        PostingParam.Add("X_EntryFrom", nAcYearID);
                        PostingParam.Add("X_SystemName", System.Environment.MachineName);
                        PostingParam.Add("N_LoanFlag", nLoanFlag);
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Pay_SalaryPaid_Voucher_Ins", PostingParam, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,ex));
                        }

                    }

                    string ipAddress = "";
                    if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                        ipAddress = Request.Headers["X-Forwarded-For"];
                    else
                        ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                        myFunctions.LogScreenActivitys(nAcYearID,nReceiptID,X_ReceiptNo,198,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);

                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }
    }
}