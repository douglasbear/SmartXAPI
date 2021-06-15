using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Net.Mail;
using System.Text;
namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("salaryProcessing")]
    [ApiController]
    public class Pay_SalaryProcessing : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        StringBuilder message = new StringBuilder();


        public Pay_SalaryProcessing(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 190;

        }

        [HttpGet("empList")]
        public ActionResult GetEmpList(string xBatch, int nFnYearID, string payRunID, string xDepartment, string xPosition, int nAddDedID, bool bAllBranchData, int nBranchID, int month, int year)
        {
            DataTable mst = new DataTable();
            DataTable MainMst = new DataTable();
            DataTable dt = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);

            string X_Cond = "";
            if (xDepartment != null && xDepartment != "")
                X_Cond = " and X_Department = ''" + xDepartment + "''";
            if (xPosition != null && xPosition != "")
            {
                if (X_Cond == "")
                    X_Cond = " and X_Position = ''" + xPosition + "''";
                else
                    X_Cond += " and X_Position = ''" + xPosition + "''";
            }

            SortedList ProParams = new SortedList();
            ProParams.Add("N_CompanyID", nCompanyID);
            ProParams.Add("N_PAyrunID", payRunID);
            ProParams.Add("X_Cond", X_Cond);
            ProParams.Add("N_FnYearID", nFnYearID);
            if (bAllBranchData == false)
                ProParams.Add("N_BranchID", nBranchID);
            else
                ProParams.Add("N_BranchID", 0);

            ProParams.Add("N_AddBatchID", nAddDedID);


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SortedList batchParams = new SortedList();
                    if (xBatch != null)
                    {
                        batchParams.Add("@nCompanyID", nCompanyID);
                        batchParams.Add("@nFnYearID", nFnYearID);
                        batchParams.Add("@xBatch", xBatch);
                        batchParams.Add("@nBranchID", nBranchID);
                        MainMst = dLayer.ExecuteDataTable("select * from Pay_PaymentMaster where n_CompanyID=@nCompanyID and N_FnYearId=@nFnYearID and X_Batch=@xBatch", batchParams, connection);
                        if (MainMst.Rows.Count == 0)
                        {
                            return Ok(_api.Notice("No Results Found"));
                        }
                        string nBatchID = MainMst.Rows[0]["N_TransID"].ToString();
                        if (nBatchID != null)
                        {
                            ProParams.Add("N_TransID", nBatchID);
                        }

                        year = myFunctions.getIntVAL(MainMst.Rows[0]["N_PayRunID"].ToString().Substring(0, 4));
                        month = myFunctions.getIntVAL(MainMst.Rows[0]["N_PayRunID"].ToString().Substring(4, 2));
                        ProParams["N_PAyrunID"] = MainMst.Rows[0]["N_PayRunID"].ToString();
                        payRunID = MainMst.Rows[0]["N_PayRunID"].ToString();
                        ProParams["N_AddBatchID"] = MainMst.Rows[0]["N_RefBatchID"].ToString();
                        if (MainMst.Rows[0]["N_RefBatchID"].ToString() != "")
                            nAddDedID = myFunctions.getIntVAL(MainMst.Rows[0]["N_RefBatchID"].ToString());
                        else
                            nAddDedID = 0;

                    }
                    else
                    {
                        ProParams.Add("N_TransID", 0);
                    }

                    mst = dLayer.ExecuteDataTablePro("SP_Pay_SelEmployeeList4Process", ProParams, connection);

                    SortedList ProParam2 = new SortedList();
                    ProParam2.Add("N_CompanyID", nCompanyID);
                    ProParam2.Add("N_PayrunID", payRunID);

                    ProParam2.Add("N_Month", month);
                    ProParam2.Add("N_Year", year);
                    ProParam2.Add("N_FnYearID", nFnYearID);
                    ProParam2.Add("N_Days", DateTime.DaysInMonth(year, month));
                    ProParam2.Add("N_BatchID", nAddDedID > 0 ? 1 : 0);

                    dt = dLayer.ExecuteDataTablePro("SP_Pay_SelSalaryDetailsForProcess", ProParam2, connection);
                    if (dt.Rows.Count > 0)
                    {
                        dt = myFunctions.AddNewColumnToDataTable(dt, "N_SaveChanges", typeof(int), 0);
                        dt = myFunctions.AddNewColumnToDataTable(dt, "N_Additions", typeof(string), "");
                        dt = myFunctions.AddNewColumnToDataTable(dt, "N_Deductions", typeof(string), "");
                    }
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        if (myFunctions.getIntVAL(dtRow["N_Type"].ToString()) == 0)
                            dtRow["N_Additions"] = dtRow["n_Payrate"].ToString();
                        else
                            dtRow["N_Deductions"] = dtRow["n_Payrate"].ToString();
                    }


                    bool B_ShowBenefitsInGrid = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Payroll", "Show Benefits", "N_Value", nCompanyID, dLayer, connection)));
                    DataTable PayPayMaster = new DataTable();
                    if (B_ShowBenefitsInGrid)
                    {
                        PayPayMaster = dLayer.ExecuteDataTable("Select N_PaymentId,N_PayID from Pay_PayMaster where N_CompanyID=" + nCompanyID + " and N_FnyearID=" + nFnYearID, connection);
                    }

                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        if (myFunctions.getVAL(dt.Rows[i]["N_PayRate"].ToString()) == 0)
                            dt.Rows[i].Delete();
                        if (B_ShowBenefitsInGrid)
                        {
                            if (ValidateBenefits(myFunctions.getIntVAL(dt.Rows[i]["N_PayID"].ToString()), myFunctions.getIntVAL(dt.Rows[i]["N_Type"].ToString()), PayPayMaster))
                            {
                                dt.Rows[i].Delete();
                            }
                        }

                    }
                    dt.AcceptChanges();
                    SortedList paytypeParam = new SortedList();
                    paytypeParam.Add("@nCompanyID", nCompanyID);
                    DataTable payType = dLayer.ExecuteDataTable("Select N_PayTypeID,N_Type from Pay_PayType Where  N_CompanyID=@nCompanyID", paytypeParam, connection);

                    mst = myFunctions.AddNewColumnToDataTable(mst, "details", typeof(DataTable), null);
                    mst.AcceptChanges();

                    foreach (DataRow mstVar in mst.Rows)
                    {
                        DataTable dtNode = new DataTable();
                        DataRow[] drEmpDetails = dt.Select("N_EmpID = " + mstVar["N_EmpID"].ToString());
                        if (drEmpDetails.Length > 0)
                        {
                            foreach (DataRow empVar in drEmpDetails)
                            {
                                DataRow[] payTypeRow = payType.Select("N_PayTypeID = " + empVar["N_PayTypeID"]);
                                if (payTypeRow.Length > 0)
                                {
                                    empVar["N_Type"] = payTypeRow[0]["N_Type"];
                                }
                            }

                            dtNode = drEmpDetails.CopyToDataTable();
                            dtNode.AcceptChanges();
                            mstVar["details"] = dtNode;
                        }
                    }
                    mst.AcceptChanges();
                    mst = _api.Format(mst);
                    if (mst.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        SortedList Output = new SortedList();
                        Output.Add("master", MainMst);
                        Output.Add("details", mst);
                        return Ok(_api.Success(Output));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        private bool ValidateBenefits(int PayID, int type, DataTable PayPayMaster)
        {
            if (type == 1) return false;
            DataRow[] dr = PayPayMaster.Select("N_PayID=" + PayID);
            if (dr != null && dr.Length > 0)
            {
                string obj = dr[0]["N_PaymentId"].ToString();
                if (myFunctions.getIntVAL(obj.ToString()) == 6 || myFunctions.getIntVAL(obj.ToString()) == 264 || myFunctions.getIntVAL(obj.ToString()) == 7)
                    return true;
            }
            return false;
        }


        [HttpGet("Dashboardlist")]
        public ActionResult SalaryProcessingDashboardList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearId);

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (N_TransID like '%" + xSearchkey + "%' or Batch like '%" + xSearchkey + "%' or  [Payrun ID] like '%" + xSearchkey + "%' or x_BankName like '%" + xSearchkey + "%' or x_AddDedBatch like '%" + xSearchkey + "%' ) ";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by batch desc,D_TransDate desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ")  n_CompanyID,N_TransID,batch as x_Batch,[Payrun ID] as x_PayrunText,d_TransDate,x_BankName,x_AddDedBatch from vw_PayTransaction_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") n_CompanyID,N_TransID,batch as x_Batch,[Payrun ID] as x_PayrunText,d_TransDate,x_BankName,x_AddDedBatch from vw_PayTransaction_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "and N_TransID not in (select top(" + Count + ") N_TransID from vw_PayTransaction_Disp where N_CompanyID=@p1 ) " + Searchkey;


            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText + xSortBy, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_PayTransaction_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey;
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
                return BadRequest(_api.Error(e));
            }
        }
        public void SendEmail(int nTransID, int nCompanyID, DateTime DT,int nFnYearID)
        {
            DataTable dt = new DataTable();
            DataTable Paycodes = new DataTable();
            SortedList Params = new SortedList();
            SortedList ProParams = new SortedList();
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nTransID);
            string sqlCommandText = "Select N_EmpID,X_EmpName,X_EmailID from vw_EmpMail_Disp where N_CompanyId= @p1 and  N_TransID=@p2";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    ProParams.Add("N_CompanyID", nCompanyID);
                    ProParams.Add("N_PayrunID", DT.Year.ToString("00##") + DT.Month.ToString("0#"));
                    ProParams.Add("N_Month", DT.Month.ToString());
                    ProParams.Add("N_Year",  DT.Year.ToString());
                    ProParams.Add("N_FnYearId", nFnYearID);
                    ProParams.Add("N_Days", DateTime.DaysInMonth(DT.Year, DT.Month));
                    ProParams.Add("N_BatchID", 1);

                    Paycodes = dLayer.ExecuteDataTablePro("SP_Pay_SelSalaryDetailsForProcess", ProParams, connection);


                    object companyemail = dLayer.ExecuteScalar("select X_Value from Gen_Settings where X_Group='210' and X_Description='EmailAddress' and N_CompanyID=" + nCompanyID, Params, connection);
                    object companypassword = dLayer.ExecuteScalar("select X_Value from Gen_Settings where X_Group='210' and X_Description='EmailPassword' and N_CompanyID=" + nCompanyID, Params, connection);
                    //StringBuilder message = new StringBuilder();

                    foreach (DataRow MasterVar in dt.Rows)
                    {
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                        mail.From = new MailAddress(companyemail.ToString());
                        DataSet dsMail = new DataSet();
                        DataTable dtMailDetails = new DataTable();
                        string condition = "";
                        string Toemail = MasterVar["X_EmailID"].ToString().Trim();
                        string expression = "X_Type='TO'";
                        string OrderByField = "";
                        if (Toemail != "")
                        {
                            mail.To.Add(Toemail);
                            string Toemailnames = MasterVar["X_EmpName"].ToString();
                            mail.Subject = "Your salary payments for the month " + DT.ToString("MMM-yyyy");
                            mail.Body = Boody(Toemailnames, myFunctions.getIntVAL(MasterVar["N_EmpId"].ToString()), nTransID, DT, Paycodes);
                            if (mail.Body == "") continue;
                            mail.IsBodyHtml = true;
                            SmtpServer.Port = 587;
                            SmtpServer.Credentials = new System.Net.NetworkCredential(companyemail.ToString(), companypassword.ToString());   // From address
                            SmtpServer.EnableSsl = true;
                            SmtpServer.Send(mail);
                            message.Length = 0;
                            mail.Body = "";
                            mail.Dispose();
                            // if (X_FormFor == "Send Email Payslip")
                            //     msg.waitMsg(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "Success"));
                        }

                    }



                }

            }
            catch (Exception e)
            {

            }
        }
        public string Boody(string Toemailnames, int ManagerID, int transID, DateTime DT, DataTable Datatable)
        {
            //StringBuilder message = new StringBuilder();

            message = message.Append("<body style='font-family:Georgia; font-size:9pt;'>");
            double Total_addn = 0.0;
            double Total_ddn = 0.0;
            double Total = 0.0;
            message = message.Append("Dear " + Toemailnames + ",<br/><br/>");
            message = message.Append("Your Salary has been Processed,please find the details below; <br/><br/>");


            foreach (DataRow var in Datatable.Rows)
            {
                double Amount = 0;
                if (var["N_EmpID"].ToString() != ManagerID.ToString()) continue;
                if (myFunctions.getIntVAL(var["N_PayTypeID"].ToString()) == 11) continue; // --- TO BLOCK END OF SERVICE AMOUNT
                Amount = myFunctions.getVAL(var["N_PayRate"].ToString());
                if (myFunctions.getIntVAL(var["N_Type"].ToString()) == 0)
                {
                    Total_addn += Amount;
                }
                else
                    Total_ddn += Amount;
            }
            Total = Total_addn - Total_ddn;
            message = message.Append(DT.ToString("MMM yyyy") + "&nbsp;&nbsp;<br/><br/>");
            message = message.Append("<table style=font-family:Georgia border=0 align=Left span=7 cellpadding=6 cellspacing=0>");
            message = message.Append("<col width=300><col width=100>");
            message = message.Append("<tr><td><b>Additions</td><td><b>" + Total_addn.ToString(myCompanyID.DecimalPlaceString) + "</b></td>");
            SalaryDetails(ManagerID, 0,Datatable);
            message = message.Append("<tr><td> <colspan=3><hr></td><br/><br/>");
            message = message.Append("<tr><td><b>Deductions</td><td><b>" + Total_ddn.ToString(myCompanyID.DecimalPlaceString) + "</b></td>");
            SalaryDetails(ManagerID, 1,Datatable);
            message = message.Append("<tr><td> <colspan=3><hr></td>");
            message = message.Append("<tr><td><b>Net Amount</b></td><td><b>" + Total.ToString(myCompanyID.DecimalPlaceString) + "</b></td>");
            message = message.Append("<tr><left>Sincerly,</left><br><left>" + "" + "</left></table>");
            return message.ToString();
        }
        private void SalaryDetails(int ManagerID, int type,DataTable Datatable)
        {
            foreach (DataRow var in Datatable.Rows)
            {
                double Amount = 0; string PayCode = "";
                if (var["N_EmpID"].ToString() != ManagerID.ToString()) continue;
                if (myFunctions.getIntVAL(var["N_PayTypeID"].ToString()) == 11) continue; // --- TO BLOCK END OF SERVICE AMOUNT
                Amount = myFunctions.getVAL(var["N_PayRate"].ToString());
                PayCode = var["X_Description"].ToString();

                if (type == 0)
                {
                    if (myFunctions.getIntVAL(var["N_Type"].ToString()) == 0)
                        message = message.Append("<tr><td>" + PayCode + "</td><td>" + Amount.ToString(myCompanyID.DecimalPlaceString) + "</td>");
                }
                else
                {
                    if (myFunctions.getIntVAL(var["N_Type"].ToString()) != 0)
                        message = message.Append("<tr><td>" + PayCode + "</td><td>" + Amount.ToString(myCompanyID.DecimalPlaceString) + "</td>");
                }

            }
        }




        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {

            try
            {
                DataTable MasterTable = ds.Tables["master"];
                DataTable DetailsTable = ds.Tables["details"];
                DataTable EmployeesTable = ds.Tables["employees"];

                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nPayRunID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PayrunID"].ToString());
                var dCreatedDate = MasterTable.Rows[0]["d_TransDate"].ToString();
                string x_Batch = MasterTable.Rows[0]["x_Batch"].ToString();
                int N_OldTransID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_TransID"].ToString());
                int N_AddDedID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_RefBatchID"].ToString());
                int year = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PayRunID"].ToString().Substring(0, 4));
                int month = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PayRunID"].ToString().Substring(4, 2));
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nPayRunID", nPayRunID);
                    int FormID = 0;
                    int N_IsAuto = 0;
                    int N_TransDetailsID = 0;
                    if (x_Batch.Trim() == "@Auto")
                    {
                        bool OK = true;
                        int NewNo = 0, loop = 1;
                        while (OK)
                        {
                            NewNo = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(Count(*),0) + " + loop + " As Count FRom Pay_PaymentMaster Where N_CompanyID=@nCompanyID  And N_PayRunID =@nPayRunID", Params, connection, transaction).ToString());
                            x_Batch = nPayRunID + "" + NewNo.ToString("0#");
                            if (myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(Count(*),0) FRom Pay_PaymentMaster Where N_CompanyID=@nCompanyID And X_Batch = '" + x_Batch + "'", Params, connection, transaction).ToString()) == 0)
                            {
                                OK = false;
                            }
                            loop += 1;
                        }
                        if (x_Batch == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable to generate batch"));

                        }
                        MasterTable.Rows[0]["x_Batch"] = x_Batch;
                    }

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " And N_FnyearID = " + nFnYearId + " and X_Batch='" + x_Batch + "'";
                    int N_TransID = dLayer.SaveData("Pay_PaymentMaster", "N_TransID", DupCriteria, "", MasterTable, connection, transaction);
                    if (N_TransID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {
                        //Delete Existing Data
                        dLayer.DeleteData("Pay_PaymentDetails", "N_TransID", N_TransID, "N_CompanyID =" + nCompanyID + " and N_FormID=190", connection, transaction);
                        dLayer.ExecuteScalar("Update Pay_LoanIssueDetails Set N_RefundAmount =Null,D_RefundDate =Null,N_PayRunID =Null,N_TransDetailsID =Null,B_IsLoanClose =Null  Where N_CompanyID =" + nCompanyID + " and N_PayrunID = " + N_TransID, connection, transaction);

                        int row = 0;
                        foreach (DataRow MasterVar in EmployeesTable.Rows)
                        {
                            double N_TotalSalary = 0;
                            double N_EOSAmt = 0;
                            foreach (DataRow var in DetailsTable.Rows)
                            {

                                if (MasterVar["N_EmpID"].ToString() != var["N_EmpID"].ToString()) continue;
                                double Amount = 0;
                                if (myFunctions.getVAL(var["N_PayRate"].ToString()) < 0)
                                    Amount = (-1) * myFunctions.getVAL(var["N_PayRate"].ToString());
                                else
                                    Amount = myFunctions.getVAL(var["N_PayRate"].ToString());

                                if (Amount == 0 && myFunctions.getVAL(var["N_Value"].ToString()) == 0) continue;
                                //if (Amount == 0)
                                //    Amount = myFunctions.getVAL(var["N_Value"].ToString());
                                if (myFunctions.getIntVAL(var["N_Type"].ToString()) == 0)
                                    N_TotalSalary += Amount;
                                else
                                    N_TotalSalary -= Amount;

                                if (myFunctions.getIntVAL(var["N_PayTypeID"].ToString()) == 11)
                                {
                                    N_EOSAmt += Amount;
                                }
                                var["N_PayRate"] = Amount;
                                var["N_TransID"] = N_TransID;

                                if (var["n_IsLoan"].ToString() == "1")
                                {
                                    DataTable loanDetails = new DataTable();
                                    SortedList loanParams = new SortedList();
                                    loanParams.Add("@nCompanyID", nCompanyID);
                                    loanParams.Add("@nLoanTransDetailsID", var["n_LoanTransDetailsID"].ToString());
                                    string loanSql = "select * from Pay_LoanIssueDetails Where N_CompanyID =@nCompanyID and N_LoanTransDetailsID =@nLoanTransDetailsID";
                                    DataTable dt = dLayer.ExecuteDataTable(loanSql, loanParams, connection, transaction);
                                    if (dt.Rows.Count > 0)
                                    {
                                        dt.Rows[0]["D_RefundDate"] = dCreatedDate.ToString();
                                        dt.Rows[0]["N_RefundAmount"] = Amount.ToString();
                                        dt.Rows[0]["N_PayRunID"] = N_TransID;
                                        dt.Rows[0]["N_TransDetailsID"] = -1;
                                        dt.Rows[0]["B_IsLoanClose"] = 0;
                                        int N_LoanTransDeatilsID = dLayer.SaveData("Pay_LoanIssueDetails", "n_LoanTransDetailsID", dt, connection, transaction);

                                    }


                                }




                            }
                            if (N_TotalSalary < 0)
                            {
                                if (N_TotalSalary + N_EOSAmt < 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error("-ve Salary"));
                                }
                            }
                        }
                        DetailsTable.Columns.Remove("n_PayTypeID");
                        DetailsTable.Columns.Remove("n_IsLoan");
                        DetailsTable.Columns.Remove("n_LoanTransDetailsID");
                        DetailsTable.AcceptChanges();
                        N_TransDetailsID = dLayer.SaveData("Pay_PaymentDetails", "N_TransDetailsID", DetailsTable, connection, transaction);
                        if (myFunctions.getIntVAL(N_TransDetailsID.ToString()) <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Error"));

                        }


                        if (N_TransDetailsID > 0)
                        {
                            dLayer.ExecuteNonQuery("Update Pay_LoanIssueDetails Set N_TransDetailsID =" + N_TransDetailsID + "  Where N_CompanyID =" + nCompanyID + " and N_TransDetailsID=-1 and D_RefundDate = '" + dCreatedDate.ToString() + "'", connection, transaction);

                            dLayer.ExecuteNonQuery("SP_Pay_SalryProcessingVoucher_Del " + nCompanyID + "," + nFnYearId + ",'ESI','" + x_Batch + "'", connection, transaction);

                            //---- GOSI Insertion                        
                            if (N_AddDedID == 0)
                                dLayer.ExecuteNonQuery("SP_Pay_GOSICalc " + nCompanyID + "," + month + "," + year + "," + N_TransID, connection, transaction);
                            //-----

                            ///Pay By Frequency
                            dLayer.ExecuteNonQuery("SP_Pay_YearlyPay " + nCompanyID + "," + N_TransID + "," + month + "," + year, connection, transaction);



                            //--- Posting

                            dLayer.ExecuteNonQuery("SP_Pay_PayrollProcessing " + nCompanyID + "," + N_TransID + ",'" + dCreatedDate.ToString() + "','" + myFunctions.GetFormatedDate(Convert.ToDateTime(DateTime.Now).ToShortDateString()) + "'," + myFunctions.GetUserID(User) + ",'Cloud','Salary Processing'", connection, transaction);
                            dLayer.ExecuteNonQuery("SP_Pay_AccrualProcess  " + nCompanyID + "," + month + ", " + year + " , " + N_TransID, connection, transaction);


                        }



                        transaction.Commit();

                        SendEmail(N_TransID,nCompanyID,  DateTime.Now,nFnYearId);
                        return Ok(_api.Success("Saved"));

                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nTransID, int nFnYearID, string xBatch)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    SortedList dltParams = new SortedList();
                    dltParams.Add("@nTransID", nTransID);
                    dltParams.Add("@nFnYearID", nFnYearID);
                    dltParams.Add("@xBatch", xBatch);

                    int count = myFunctions.getIntVAL(dLayer.ExecuteNonQuery("Select count(*) from Acc_VoucherMaster Where N_CompanyID=" + myFunctions.GetCompanyID(User) + " And N_FnyearID =@nFnYearID and X_TransType = 'ESI' and B_IsAccPosted = 1 and X_ReferenceNo=@xBatch", dltParams, connection, transaction).ToString());


                    if (count > 0)
                    {
                        return Ok(_api.Error("Unable to delete ,Transactions Exist"));

                    }

                    dLayer.ExecuteNonQuery("Update Pay_LoanIssueDetails Set N_RefundAmount =Null,D_RefundDate =Null,N_PayRunID =Null,N_TransDetailsID =Null,B_IsLoanClose =Null  Where N_CompanyID =" + myFunctions.GetCompanyID(User) + " and N_PayrunID = " + nTransID, connection, transaction);
                    dLayer.ExecuteNonQuery("SP_Pay_SalryProcessingVoucher_Del " + myFunctions.GetCompanyID(User) + ",@nFnYearID,'ESI',@xBatch", dltParams, connection, transaction);
                    Results = dLayer.DeleteData("Pay_PaymentDetails", "N_TransID", nTransID, "N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FormID = 190", connection, transaction);

                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to delete batch"));
                    }
                    Results = dLayer.DeleteData("Pay_PaymentMaster", "N_TransID", nTransID, "N_CompanyID=" + myFunctions.GetCompanyID(User), connection, transaction);

                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to delete batch"));
                    }

                    transaction.Commit();
                    return Ok(_api.Success("Batch deleted"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }

        }

    }
}
