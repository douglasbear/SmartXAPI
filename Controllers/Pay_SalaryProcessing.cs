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
using System.IO;
using System.Threading.Tasks;
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
        private readonly string TempFilesPath;




        public Pay_SalaryProcessing(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 190;
            TempFilesPath = conf.GetConnectionString("TempFilesPath");

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
                        MainMst = dLayer.ExecuteDataTable("SELECT Pay_PaymentMaster.*, Acc_BankMaster.X_BankName FROM Pay_PaymentMaster LEFT OUTER JOIN Acc_BankMaster ON Pay_PaymentMaster.N_BankID = Acc_BankMaster.N_BankID AND Pay_PaymentMaster.N_CompanyID = Acc_BankMaster.N_CompanyID WHERE (Pay_PaymentMaster.N_CompanyID = @nCompanyID) AND (Pay_PaymentMaster.N_FnYearID = @nFnYearID) AND (Pay_PaymentMaster.X_Batch = @xBatch) ", batchParams, connection);
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

                    dt = myFunctions.AddNewColumnToDataTable(dt, "isHidden", typeof(bool), false);

                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        if (myFunctions.getVAL(dt.Rows[i]["N_PayRate"].ToString()) == 0)
                        {
                            dt.Rows[i].Delete();
                            continue;
                        }
                        if (B_ShowBenefitsInGrid)
                        {
                            // if (ValidateBenefits(myFunctions.getIntVAL(dt.Rows[i]["N_PayID"].ToString()), myFunctions.getIntVAL(dt.Rows[i]["N_Type"].ToString()), PayPayMaster))
                            if (myFunctions.getIntVAL(dt.Rows[i]["b_ISBenefit"].ToString()) == 1)
                            {
                                dt.Rows[i]["isHidden"] = true;
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
                            // foreach (DataRow empVar in drEmpDetails)
                            // {
                            //     DataRow[] payTypeRow = payType.Select("N_PayTypeID = " + empVar["N_PayTypeID"]);
                            //     if (payTypeRow.Length > 0)
                            //     {
                            //         empVar["N_Type"] = payTypeRow[0]["N_Type"];
                            //     }
                            // }

                            dtNode = drEmpDetails.CopyToDataTable();
                            dtNode.AcceptChanges();
                            mstVar["details"] = dtNode;
                        }
                    }
                    mst.AcceptChanges();
                    mst = _api.Format(mst);

                    foreach (DataRow Kvar in mst.Rows)
                    {
                        if (myFunctions.getBoolVAL(Kvar["B_ExcludeInSalary"].ToString()) == true && Kvar["details"] == null)
                        {
                            Kvar.Delete();
                            continue;
                        }
                    }
                    mst.AcceptChanges();


                    // if (mst.Rows.Count == 0)
                    // {
                    //     return Ok(_api.Notice("No Results Found"));
                    // }
                    // else
                    // {
                        SortedList Output = new SortedList();
                        Output.Add("master", MainMst);
                        Output.Add("details", mst);
                        return Ok(_api.Success(Output));
                    // }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
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
        public ActionResult SalaryProcessingDashboardList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, bool bAllBranchData, int nBranchID)
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
                Searchkey = "and (N_TransID like '%" + xSearchkey + "%' or Batch like '%" + xSearchkey + "%' or  [Payrun ID] like '%" + xSearchkey + "%' or x_BankName like '%" + xSearchkey + "%' or x_AddDedBatch like '%" + xSearchkey + "%' or cast(d_TransDate as Varchar) like '%" + xSearchkey + "%') ";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by [Payrun ID] desc,cast(Batch as Numeric) desc";
            // xSortBy = " order by batch desc,D_TransDate desc";
            else
                xSortBy = " order by " + xSortBy;

            if (bAllBranchData == true)
            {
                Searchkey = Searchkey + " ";
            }
            else
            {
                Searchkey = Searchkey + " and N_BranchID=" + nBranchID + " ";
            }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ")  n_CompanyID,N_TransID,batch as x_Batch,[Payrun ID] as x_PayrunText,d_TransDate,x_BankName,x_AddDedBatch from vw_PayTransaction_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") n_CompanyID,N_TransID,batch as x_Batch,[Payrun ID] as x_PayrunText,d_TransDate,x_BankName,x_AddDedBatch from vw_PayTransaction_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and N_TransID not in (select top(" + Count + ") N_TransID from vw_PayTransaction_Disp where N_CompanyID=@p1 and N_FnYearID=@p2  " + Searchkey + xSortBy + ") " + xSortBy;


            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(1) as N_Count  from vw_PayTransaction_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey;
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
                return BadRequest(_api.Error(User, e));
            }
        }
        public int GenerateCSVFiles(int nBankID, int nCompanyID, string x_batchID, int nFnYearID, string N_salaryAmt, DateTime dtPayrun, DateTime dtpSalFrom, DateTime dtpSalTo)
        {
            DataTable dtBankCSV = new DataTable();
            DataTable Paycodes = new DataTable();
            SortedList Params = new SortedList();
            SortedList ProParams = new SortedList();
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nBankID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    DataSet dsBank = new DataSet();
                    if (dsBank.Tables.Contains("BankCSV"))
                        dsBank.Tables.Remove("BankCSV");
                    string BankCSV = "Select  N_BankID,X_BankName,X_CSVTemplatePath,N_CompanyID from Acc_BankMaster where N_CompanyID=@p1 and N_BankID=@p2";
                    dtBankCSV = dLayer.ExecuteDataTable(BankCSV, Params, connection);
                    if (dtBankCSV.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtBankCSV.Rows)
                        {
                            switch (dr["X_CSVTemplatePath"].ToString())
                            {
                                case "ARB":
                                    AlRajhiBank(x_batchID, myFunctions.getIntVAL(dr["N_BankID"].ToString()));
                                    break;
                                case "RBT":
                                    AlRajhiBankTahweel(x_batchID, myFunctions.getIntVAL(dr["N_BankID"].ToString()));
                                    break;
                                case "BSF":
                                    BanqueSaudiFransi(x_batchID, myFunctions.getIntVAL(dr["N_BankID"].ToString()), dtPayrun);
                                    break;
                                case "NCB":
                                    NationalCommercialBank(x_batchID, myFunctions.getIntVAL(dr["N_BankID"].ToString()), dtPayrun);
                                    break;
                                case "SBB":
                                    SaudiBritishBank(x_batchID, myFunctions.getIntVAL(dr["N_BankID"].ToString()), dtPayrun, dtpSalFrom, dtpSalTo);
                                    break;
                                case "ARNB":
                                    ARNBBank(x_batchID, myFunctions.getIntVAL(dr["N_BankID"].ToString()));
                                    break;
                                case "BMCT":
                                    BMCTBank(x_batchID, myFunctions.getIntVAL(dr["N_BankID"].ToString()), dtPayrun);
                                    break;

                                default:
                                    GenerateCSV(x_batchID, myFunctions.getIntVAL(dr["N_BankID"].ToString()), N_salaryAmt, nFnYearID);
                                    break;
                            }
                        }
                    }
                    return 1;
                }
            }
            catch (Exception e)
            {
                return 0;

            }
        }
        public int ARNBBank(string x_batchID, int BankID)
        {

            try
            {
                DataTable dtTransaction = new DataTable();
                int nCompanyID = myFunctions.GetCompanyID(User);

                StringBuilder sb = new StringBuilder();
                SortedList Params = new SortedList();
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", BankID);

                string FileCreateTime = DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                string X_WpsFileName = this.TempFilesPath + myFunctions.GetCompanyID(User) + "-" + x_batchID + ".csv";
                string CSVData = "Select X_BankName,X_BankAccountNo,X_EmpName,X_EmpCode,X_Nationality,(N_BasicSalary+N_HA+N_OtherEarnings-N_OtherDeductions)as totalsalary ,X_Address,N_Payrate,X_BankCode,X_PaymentDescription,X_ReturnCode,N_BasicSalary,N_HA,N_OtherEarnings,N_OtherDeductions,X_IqamaNo,X_Transactionnumber,X_Transactionstatus,X_TransDate,X_Department,X_BranchName,X_BranchCode,X_PayrunText,D_TransDate,x_CompanyBank,X_Currency,x_CompanyBankAccountNo from [vw_pay_ProcessedDetails_CSV] where X_Batch='" + x_batchID + "' and N_EmpTypeID<>183 and TransBankID=" + BankID;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dtTransaction = dLayer.ExecuteDataTable(CSVData, Params, connection);
                    int index = 0;
                    double TotAmount = 0;
                    foreach (DataRow drow in dtTransaction.Rows)
                    {
                        TotAmount = TotAmount + myFunctions.getVAL(drow["totalsalary"].ToString());
                    }
                    foreach (DataRow drow in dtTransaction.Rows)
                    {


                        if (!System.IO.File.Exists(X_WpsFileName))
                        {
                            System.IO.File.Create(X_WpsFileName).Close();
                        }
                        else
                        {
                            System.IO.File.WriteAllText(X_WpsFileName, String.Empty);
                        }
                        string delimiter = ",";
                        string[][] header = new string[][]
                        {new string[]{"H",drow["x_CompanyBank"].ToString(),"1249","N",DateTime.Now.ToString("ddMyyyy")+".EX1",drow["x_CompanyBankAccountNo"].ToString(),drow["X_Currency"].ToString(),Convert.ToDateTime(drow["D_TransDate"]).ToString("ddMyyyy"),TotAmount.ToString(),Convert.ToDateTime(drow["D_TransDate"]).ToString("ddMyyyy"),"1-12779","Salary For " +drow["X_PayrunText"].ToString()}
                        };
                        string[][] output = new string[][]
                        {
                       new string[]{"D",drow["totalsalary"].ToString(),drow["X_BankAccountNo"].ToString(),drow["X_EmpName"].ToString(),drow["X_BankName"].ToString(),"Salary For " +drow["X_PayrunText"].ToString(),drow["N_BasicSalary"].ToString(),drow["N_HA"].ToString(),drow["N_OtherEarnings"].ToString(),drow["N_OtherDeductions"].ToString(),drow["X_IqamaNo"].ToString()}
                     };
                        int length = output.GetLength(0);
                        if (index == 0)
                        {
                            sb.AppendLine(string.Join(delimiter, header[0]));
                        }
                        for (index = 0; index < length; index++)
                            sb.AppendLine(string.Join(delimiter, output[index]));

                    }
                }

                System.IO.File.AppendAllText(X_WpsFileName, sb.ToString());

                return 1;
            }
            catch (Exception ex)
            {
                if (ex is DirectoryNotFoundException)
                {

                    return 0;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int AlRajhiBank(string x_batchID, int BankID)
        {

            try
            {
                DataTable dtTransaction = new DataTable();
                int nCompanyID = myFunctions.GetCompanyID(User);

                StringBuilder sb = new StringBuilder();
                SortedList Params = new SortedList();
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", BankID);

                string FileCreateTime = DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                string X_WpsFileName = this.TempFilesPath + myFunctions.GetCompanyID(User) + "-" + x_batchID + ".csv";
                string CSVData = "Select X_BankName,X_BankAccountNo,X_EmpName,X_EmpCode,X_Nationality,(N_BasicSalary+N_HA+N_OtherEarnings-N_OtherDeductions)as totalsalary ,X_Address,N_Payrate,X_BankCode,X_PaymentDescription,X_ReturnCode,N_BasicSalary,N_HA,N_OtherEarnings,N_OtherDeductions,X_IqamaNo,X_Transactionnumber,X_Transactionstatus,X_TransDate,X_Department,X_BranchName,X_BranchCode,X_PayrunText from [vw_pay_ProcessedDetails_CSV] where X_Batch='" + x_batchID + "' and N_EmpTypeID<>183 and TransBankID=" + BankID;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dtTransaction = dLayer.ExecuteDataTable(CSVData, Params, connection);
                    int index = 0;
                    foreach (DataRow drow in dtTransaction.Rows)
                    {


                        if (!System.IO.File.Exists(X_WpsFileName))
                        {
                            System.IO.File.Create(X_WpsFileName).Close();
                        }
                        else
                        {
                            System.IO.File.WriteAllText(X_WpsFileName, String.Empty);
                        }
                        string delimiter = ",";
                        string[][] header = new string[][]
                        {new string[]{"Bank Name ","Account Number(34N)","Employee Name","Employee Number","National ID Number (15N)","Salary (15N)","Basic Salary","Housing Allowance","Other Earnings","Deductions","Branch Code","Branch Name","Employee Remarks","Employee Department"}
                        };
                        string[][] output = new string[][]
                        {
                       new string[]{drow["X_BankName"].ToString(),"'"+drow["X_BankAccountNo"].ToString(),drow["X_EmpName"].ToString(),drow["X_EmpCode"].ToString(),"'"+drow["X_IqamaNo"].ToString(),drow["totalsalary"].ToString(),drow["N_BasicSalary"].ToString(),drow["N_HA"].ToString(),drow["N_OtherEarnings"].ToString(),drow["N_OtherDeductions"].ToString(),drow["X_BranchCode"].ToString(),drow["X_BranchName"].ToString(),drow["X_PayrunText"].ToString()+" - Salary",drow["X_Department"].ToString()}
                     };
                        int length = output.GetLength(0);
                        if (index == 0)
                        {
                            sb.AppendLine(string.Join(delimiter, header[0]));
                        }
                        for (index = 0; index < length; index++)
                            sb.AppendLine(string.Join(delimiter, output[index]));

                    }
                }

                System.IO.File.AppendAllText(X_WpsFileName, sb.ToString());

                return 1;
            }
            catch (Exception ex)
            {
                if (ex is DirectoryNotFoundException)
                {

                    return 0;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int AlRajhiBankTahweel(string x_batchID, int BankID)
        {

            try
            {
                DataTable dtTransaction = new DataTable();
                int nCompanyID = myFunctions.GetCompanyID(User);

                StringBuilder sb = new StringBuilder();
                SortedList Params = new SortedList();
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", BankID);


                string FileCreateTime = DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                string X_WpsFileName = this.TempFilesPath + myFunctions.GetCompanyID(User) + "-" + x_batchID + ".csv";

                string CSVData = "Select X_BankName,X_BankAccountNo,X_EmpName,X_EmpCode,X_Nationality,(N_BasicSalary+N_HA+N_OtherEarnings-N_OtherDeductions)as totalsalary ,X_Address,N_Payrate,X_BankCode,X_PaymentDescription,X_ReturnCode,N_BasicSalary,N_HA,N_OtherEarnings,N_OtherDeductions,X_IqamaNo,X_Transactionnumber,X_Transactionstatus,X_TransDate,X_Department,X_BranchName,X_BranchCode,X_PayrunText from [vw_pay_ProcessedDetails_CSV] where X_Batch='" + x_batchID + "' and N_EmpTypeID<>183 and TransBankID=" + BankID;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dtTransaction = dLayer.ExecuteDataTable(CSVData, Params, connection);


                    int index = 0;
                    foreach (DataRow drow in dtTransaction.Rows)
                    {
                        if (!System.IO.File.Exists(X_WpsFileName))
                        {
                            System.IO.File.Create(X_WpsFileName).Close();
                        }
                        else
                        {
                            System.IO.File.WriteAllText(X_WpsFileName, String.Empty);
                        }
                        string delimiter = ",";
                        string[][] header = new string[][]
                        {new string[]{"Bank Name ","Account Number(34N)","Employee Name","Employee Number","National ID Number (15N)","Salary (15N)","Basic Salary","Housing Allowance","Other Earnings","Deductions","Branch Code","Branch Name","Employee Remarks","Employee Department"}
                        };
                        string[][] output = new string[][]
                        {
                      new string[]{drow["X_EmpCode"].ToString().PadLeft(12,'0')+drow["X_BankAccountNo"].ToString().PadLeft(24,'0')+drow["X_EmpName"].ToString().PadRight(50)+drow["X_IqamaNo"].ToString().PadLeft(10,'0')+ myFunctions.getVAL( drow["totalsalary"].ToString()).ToString("0.00").Replace(".","").PadLeft(15,'0')+FileCreateTime.PadRight(15,'0')+myFunctions.getVAL(drow["N_BasicSalary"].ToString()).ToString("0.00").Replace(".","").PadLeft(12,'0')+myFunctions.getVAL(drow["N_HA"].ToString()).ToString("0.00").Replace(".","").PadLeft(12,'0')+myFunctions.getVAL(drow["N_OtherEarnings"].ToString()).ToString("0.00").Replace(".","").PadLeft(12,'0')+myFunctions.getVAL(drow["N_OtherDeductions"].ToString()).ToString("0.00").Replace(".","").PadLeft(12,'0')}
                       };
                        int length = output.GetLength(0);
                        for (index = 0; index < length; index++)
                            sb.AppendLine(string.Join(delimiter, output[index]));

                    }
                }

                System.IO.File.AppendAllText(X_WpsFileName, sb.ToString());

                return 1;
            }
            catch (Exception ex)
            {
                if (ex is DirectoryNotFoundException)
                {

                    return 0;
                }
                else
                {
                    return 0;
                }
            }
        }


        public int BanqueSaudiFransi(string x_batchID, int BankID, DateTime dtPayrun)
        {

            try
            {
                DataTable CSVMaster = new DataTable();
                DataTable CSVData = new DataTable();
                string FileCreateDate = "";
                string FileCreateTime = "";
                string SalaryProcessCode = "";
                double TotalAmount = 0.0;
                int TotlRecords = 0;
                int nCompanyID = myFunctions.GetCompanyID(User);
                FileCreateDate = DateTime.Now.ToString("yyyyMMdd");
                FileCreateTime = DateTime.Now.ToString("HHmm");
                SalaryProcessCode = dtPayrun.ToString("yyyyMM");
                //X_WpsFileName = X_WpsPath + FileCreateDate + "-" + FileCreateTime + ".txt";
                string X_WpsFileName = this.TempFilesPath + myFunctions.GetCompanyID(User) + "-" + x_batchID + ".csv";
                StringBuilder sb = new StringBuilder();
                SortedList Params = new SortedList();
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", BankID);


                if (!System.IO.File.Exists(X_WpsFileName))
                {
                    System.IO.File.Create(X_WpsFileName).Close();
                }
                else
                {
                    System.IO.File.WriteAllText(X_WpsFileName, String.Empty);
                }

                int days = DateTime.DaysInMonth(dtPayrun.Year, dtPayrun.Month);
                string MOLNo = "1-1933106";
                string delimiter = "\t";
                int length;
                string Address = "", Address1 = "", Address2 = "";
                string CSVMastersql = "Select  X_AccountHolderName,N_PayRunID,X_AccountNo,X_TransDate,sum(Salary) as Salary,X_SwiftNo,X_BankName,X_IBAN from [vw_CSVTransBankDetailsByTranID] where X_Batch='" + x_batchID + "' and TransBankID=" + BankID + " group by X_AccountHolderName,N_PayRunID,X_AccountNo,X_TransDate,X_SwiftNo,X_BankName,X_IBAN";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    CSVMaster = dLayer.ExecuteDataTable(CSVMastersql, Params, connection);

                    if (CSVMaster.Rows.Count > 0)
                    {
                        DataRow dr = CSVMaster.Rows[0];
                        string[][] main_heading_val = new string[][]
                        {

                     new string[]{dr["X_BankName"].ToString(),dr["X_SwiftNo"].ToString(),dr["X_AccountNo"].ToString(), myCompanyID._CurrencyName, dr["X_TransDate"].ToString(), myFunctions.getVAL( dr["Salary"].ToString()).ToString("0.00").Replace(".",","), dr["X_TransDate"].ToString(), DateTime.Now.ToString("yyyyMMddHHmmsstt"),"",MOLNo} /*add the values that you want inside a csv file. Mostly this function can be used in a foreach loop.*/,
                        };
                        length = main_heading_val.GetLength(0);
                        for (int index = 0; index < length; index++)
                            sb.AppendLine(string.Join(delimiter, main_heading_val[index]));
                    }

                    string CSVDatasql = "Select  N_EmpID,N_Payrate,X_BankAccountNo,X_EmpName,X_EmpPatakaName,X_EmpCode,X_BankName,X_BankCode,X_PaymentDescription,X_ReturnCode,N_BasicSalary,N_HA,N_OtherEarnings,N_OtherDeductions,X_IqamaNo,X_Transactionnumber,X_Transactionstatus,X_TransDate,X_Address,'' as X_Address1,'' as X_Address2, X_Nationality from [vw_pay_ProcessedDetails_CSV] where X_Batch='" + x_batchID + "' and N_EmpTypeID<>183 and TransBankID=" + BankID;
                    CSVData = dLayer.ExecuteDataTable(CSVDatasql, Params, connection);

                    int row = 1;
                    foreach (DataRow drow in CSVData.Rows)
                    {

                        string[][] output = new string[][]
                        {
                        new string[]{myFunctions.getVAL( drow["N_Payrate"].ToString()).ToString("0.00").Replace(".",","),drow["X_BankAccountNo"].ToString(),(drow["X_EmpName"].ToString()+drow["X_Address"].ToString()+drow["X_Address1"].ToString()+drow["X_Address2"].ToString()).PadRight(150,' '), drow["X_BankName"].ToString(),"Salary","", myFunctions.getVAL(drow["N_BasicSalary"].ToString()).ToString("0.00").Replace(".",","),myFunctions.getVAL(drow["N_HA"].ToString()).ToString("0.00").Replace(".",","),myFunctions.getVAL(drow["N_OtherEarnings"].ToString()).ToString("0.00").Replace(".",","),myFunctions.getVAL(drow["N_OtherDeductions"].ToString()).ToString("0.00").Replace(".",","),drow["X_IqamaNo"].ToString()}
                        };
                        length = output.GetLength(0);
                        for (int index = 0; index < length; index++)
                            sb.AppendLine(string.Join(delimiter, output[index]));

                        row = row + 1;
                    }
                }

                System.IO.File.AppendAllText(X_WpsFileName, sb.ToString());
                return 1;
            }
            catch (Exception ex)
            {
                if (ex is DirectoryNotFoundException)
                {
                    return 0;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int NationalCommercialBank(string x_batchID, int BankID, DateTime dtPayrun)
        {

            try
            {

                string X_WpsFileName = this.TempFilesPath + myFunctions.GetCompanyID(User) + "-" + x_batchID + ".csv";

                StringBuilder sb = new StringBuilder();
                DataTable CSVData = new DataTable();
                SortedList Params = new SortedList();
                int nCompanyID = myFunctions.GetCompanyID(User);
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", BankID);
                string CSVDatasql = "Select X_BankCodeRef,X_BankName,X_BankAccountNo,X_EmpName,X_EmpCode,X_Nationality,(N_BasicSalary+N_HA+N_OtherEarnings-N_OtherDeductions)as totalsalary ,X_Address,N_Payrate,X_BankCode,X_PaymentDescription,X_ReturnCode,N_BasicSalary,N_HA,N_OtherEarnings,N_OtherDeductions,X_IqamaNo,X_Transactionnumber,X_Transactionstatus,X_TransDate,X_Department,X_BranchName,X_BranchCode,X_PayrunText from [vw_pay_ProcessedDetails_CSV] where X_Batch=" + x_batchID + " and N_EmpTypeID<>183 and TransBankID=" + BankID;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    CSVData = dLayer.ExecuteDataTable(CSVDatasql, Params, connection);

                    int index = 0;
                    foreach (DataRow drow in CSVData.Rows)
                    {


                        if (!System.IO.File.Exists(X_WpsFileName))
                        {
                            System.IO.File.Create(X_WpsFileName).Close();
                        }
                        else
                        {
                            System.IO.File.WriteAllText(X_WpsFileName, String.Empty);
                        }
                        string delimiter = ",";

                        string[][] header = new string[][]
                        {new string[]{"Bank Code","Employee/Payee's Account Number","Amount","Transaction Reference","Employee/Payee's Name","Employee/Payee's Address 1","Employee/Payee's Address 2","Employee/Payee's Address 3","Beneficiary ID (National/Iqama ID)","Basic Salary","Housing Allowance","Other Earnings","Deductions"}
                        };
                        string[][] output = new string[][]
                        {
                        new string[]{drow["X_BankCodeRef"].ToString(),""+ drow["X_BankAccountNo"].ToString(),drow["totalsalary"].ToString(),drow["X_PaymentDescription"].ToString(),drow["X_EmpName"].ToString(),drow["X_Address"].ToString()," "," ",""+ drow["X_IqamaNo"].ToString(),drow["N_BasicSalary"].ToString(),drow["N_HA"].ToString(),drow["N_OtherEarnings"].ToString(),drow["N_OtherDeductions"].ToString()}
                       };

                        int length = output.GetLength(0);


                        if (index == 0)
                        {
                            sb.AppendLine(string.Join(delimiter, header[0]));
                        }
                        for (index = 0; index < length; index++)
                            sb.AppendLine(string.Join(delimiter, output[index]));

                    }
                }

                System.IO.File.AppendAllText(X_WpsFileName, sb.ToString());

                return 1;
            }
            catch (Exception ex)
            {
                if (ex is DirectoryNotFoundException)
                {

                    return 0;
                }
                else
                {

                    return 0;
                }
            }
        }
        public int BMCTBank(string x_batchID, int BankID, DateTime dtPayrun)
        {

            try
            {

                string X_WpsFileName = this.TempFilesPath + myFunctions.GetCompanyID(User) + "-" + x_batchID + ".csv";

                StringBuilder sb = new StringBuilder();
                DataTable CSVData = new DataTable();
                SortedList Params = new SortedList();
                int nCompanyID = myFunctions.GetCompanyID(User);
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", BankID);
                string CSVDatasql = "Select ROW_NUMBER() OVER (ORDER BY X_EmpCode) as N_Row,X_CRNumber,X_IqamaNo,X_BankCodeRef,X_BankName,X_BankAccountNo,X_EmpName,X_EmpCode,X_Nationality,(N_BasicSalary+N_HA+N_OtherEarnings-N_OtherDeductions)as totalsalary ,X_Address,N_Payrate,X_BankCode,X_PaymentDescription,X_ReturnCode,N_BasicSalary,N_HA,N_OtherEarnings,N_OtherDeductions,X_IqamaNo,X_Transactionnumber,X_Transactionstatus,X_TransDate,X_Department,X_BranchName,X_BranchCode,X_PayrunText,X_Transbankname,X_TransbankAccnumber,N_SalMonth,N_SalYear from [vw_pay_ProcessedDetails_CSV] where X_Batch=" + x_batchID + " and N_EmpTypeID<>183 and TransBankID=" + BankID;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    CSVData = dLayer.ExecuteDataTable(CSVDatasql, Params, connection);
                    object TotalCount = dLayer.ExecuteScalar("Select count(*) from [vw_pay_ProcessedDetails_CSV] where X_Batch=" + x_batchID + " and N_EmpTypeID<>183 and TransBankID=" + BankID, Params, connection);
                    object Netsalary = dLayer.ExecuteScalar("Select sum(N_BasicSalary+N_HA+N_OtherEarnings-N_OtherDeductions)as totalsalary from [vw_pay_ProcessedDetails_CSV] where X_Batch=" + x_batchID + " and N_EmpTypeID<>183 and TransBankID=" + BankID, Params, connection);


                    int index = 0;
                    foreach (DataRow drow in CSVData.Rows)
                    {


                        if (!System.IO.File.Exists(X_WpsFileName))
                        {
                            System.IO.File.Create(X_WpsFileName).Close();
                        }
                        else
                        {
                            System.IO.File.WriteAllText(X_WpsFileName, String.Empty);
                        }
                        string delimiter = ",";

                        string[][] mainheader = new string[][]
                        {new string[]{"Employer CR-NO","Payer CR-NO","Payer Bank Short Name","Payer Account  Number","Salary Year","Salary Month","Total Salaries","Number Of Records","Payment Type"}

                        };
                        string[][] mainoutput = new string[][]
                        {
                        new string[]{drow["X_CRNumber"].ToString(),drow["X_CRNumber"].ToString(),"BMCT",drow["X_TransbankAccnumber"].ToString(),drow["N_SalYear"].ToString(),drow["N_SalMonth"].ToString(),Netsalary.ToString(),TotalCount.ToString(),"Salary"}
                        };

                        string[][] header = new string[][]
                        {new string[]{"Employee ID Type","Employee ID","Reference Number","Employee Name","Employee BIC Code","Employee Account","Salary Frequency","Number Of Working days","Net Salary","Basic Salary","Extra Hours","Extra Income","Deductions","Social Security Deductions","Notes / Comments"}
                        };

                        string[][] output = new string[][]
                        {
                        new string[]{"C",drow["X_IqamaNo"].ToString(),drow["N_Row"].ToString(), drow["X_EmpName"].ToString(),drow["X_BankCodeRef"].ToString(),drow["X_BankAccountNo"].ToString(),"M","30",drow["totalsalary"].ToString(),drow["N_BasicSalary"].ToString(),"0",(double.Parse(drow["N_HA"].ToString())+double.Parse(drow["N_OtherEarnings"].ToString())).ToString(),drow["N_OtherDeductions"].ToString(),"0",drow["X_PayrunText"].ToString()+" Salary"}
                        };
                        int length = output.GetLength(0);


                        if (index == 0)
                        {
                            sb.AppendLine(string.Join(delimiter, mainheader[0]));
                            sb.AppendLine(string.Join(delimiter, mainoutput[0]));
                            sb.AppendLine(string.Join(delimiter, header[0]));
                        }

                        for (index = 0; index < length; index++)
                            sb.AppendLine(string.Join(delimiter, output[index]));

                    }
                }

                System.IO.File.AppendAllText(X_WpsFileName, sb.ToString());

                return 1;
            }
            catch (Exception ex)
            {
                if (ex is DirectoryNotFoundException)
                {

                    return 0;
                }
                else
                {

                    return 0;
                }
            }
        }
        public int SaudiBritishBank(string x_batchID, int BankID, DateTime dtPayrun, DateTime dtpSalFrom, DateTime dtpSalTo)
        {
            string CsvPayrunID = "";
            string X_CompBankCode = "", X_CompRefNo = "", X_Currency = "", X_BankAccountNo = "";
            int N_EDRCount = 0;
            double N_TotalAmt = 0;
            DataTable CSVData = new DataTable();
            DataTable CompanyBank = new DataTable();
            try
            {
                string X_WpsFileName = this.TempFilesPath + myFunctions.GetCompanyID(User) + "-" + x_batchID.Trim() + ".csv";

                DateTime FromDate = dtpSalFrom;
                DateTime ToDate = dtpSalTo;


                StringBuilder sb = new StringBuilder();
                TimeSpan tspan = ToDate - FromDate;

                int differenceInDays = tspan.Days;

                CsvPayrunID = dtPayrun.Month.ToString("0#") + dtPayrun.Year.ToString("00##");
                SortedList Params = new SortedList();
                int nCompanyID = myFunctions.GetCompanyID(User);
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", BankID);

                string differenceAsString = differenceInDays.ToString();
                string CSVDatasql = "Select * from [vw_EmployeeSalaryDetails_SIF] where X_Batch=" + x_batchID + " and N_EmpTypeID<>183 and TransBankID=" + BankID;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    CSVData = dLayer.ExecuteDataTable(CSVDatasql, Params, connection);
                    int index = 0;
                    int row = 0;

                    foreach (DataRow drow in CSVData.Rows)
                    {
                        row = row + 1;

                        if (!System.IO.File.Exists(X_WpsFileName))
                        {
                            System.IO.File.Create(X_WpsFileName).Close();
                        }
                        else
                        {
                            System.IO.File.WriteAllText(X_WpsFileName, String.Empty);
                        }
                        string delimiter = ",";
                        string[][] output = new string[][]
                        {
                        new string[]{"EDR",drow["X_IqamaNo"].ToString(),drow["X_BankCode"].ToString(),drow["X_BankAccountNo"].ToString(),dtpSalFrom.ToString("dd/MMM/yyyy"),dtpSalTo.ToString("dd/MMM/yyyy"),differenceInDays.ToString(),(myFunctions.getVAL(drow["BasicSalary"].ToString())+ myFunctions.getVAL(drow["OtherFixedSalary"].ToString())).ToString(),(myFunctions.getVAL(drow["N_VariableIncome"].ToString())+ myFunctions.getVAL(drow["N_OtherVariableIncome"].ToString())).ToString(),"0"}
                        };
                        int length = output.GetLength(0);

                        for (index = 0; index < length; index++)
                            sb.AppendLine(string.Join(delimiter, output[index]));
                        N_EDRCount = row;
                        N_TotalAmt = N_TotalAmt + ((myFunctions.getVAL(drow["BasicSalary"].ToString()) + myFunctions.getVAL(drow["OtherFixedSalary"].ToString()))) + (myFunctions.getVAL(drow["N_VariableIncome"].ToString()) + myFunctions.getVAL(drow["N_OtherVariableIncome"].ToString()));

                    }
                    DataSet dsCompany = new DataSet();
                    string CompanyBanksql = "Select * from Vw_BankDetails_rpt where N_CompanyID=" + nCompanyID + " and B_IsCompany=1 and N_BankID=" + BankID;
                    CompanyBank = dLayer.ExecuteDataTable(CompanyBanksql, Params, connection);

                    if (CompanyBank.Rows.Count > 0)
                    {
                        foreach (DataRow drow1 in dsCompany.Tables["CompanyBank"].Rows)
                        {
                            X_CompBankCode = drow1["X_SwiftNo"].ToString();
                            X_CompRefNo = "1";
                            X_BankAccountNo = drow1["X_AccountNo"].ToString();
                            X_Currency = "";
                        }
                    }

                    string[][] output1 = new string[][]
                        {
                        new string[]{"SCR",X_CompRefNo,X_CompBankCode,dtPayrun.ToString("dd/MMM/yyyy"),dtPayrun.ToString("hhmm"),CsvPayrunID,dtpSalFrom.ToString("dd/MMM/yyyy"),N_EDRCount.ToString(),N_TotalAmt.ToString(),X_Currency,""}
                        };
                    string delimiter1 = ",";
                    int length1 = output1.GetLength(0);
                    for (index = 0; index < length1; index++)
                        sb.AppendLine(string.Join(delimiter1, output1[index]));
                }

                System.IO.File.AppendAllText(X_WpsFileName, sb.ToString());
                System.IO.File.Move(X_WpsFileName, Path.ChangeExtension(X_WpsFileName, ".sif"));
                return 1;
            }
            catch (Exception ex)
            {
                if (ex is DirectoryNotFoundException)
                {

                    return 0;
                }
                else
                {

                    return 0;
                }
            }
        }
        public int GenerateCSV(string x_batchID, int BankID, string N_salaryAmt, int nFnYearID)
        {

            try
            {
                DataTable dtTransaction = new DataTable();
                int nCompanyID = myFunctions.GetCompanyID(User);

                StringBuilder sb = new StringBuilder();
                SortedList Params = new SortedList();
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", BankID);

                string FileCreateTime = DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                string X_WpsFileName = this.TempFilesPath + myFunctions.GetCompanyID(User) + "-" + x_batchID + ".csv";
                string CSVData = "Select X_BankName,X_BankAccountNo,X_EmpName,X_EmpCode,X_Nationality,(N_BasicSalary+N_HA+N_OtherEarnings-N_OtherDeductions)as totalsalary ,X_Address,N_Payrate,X_BankCode,X_PaymentDescription,X_ReturnCode,N_BasicSalary,N_HA,N_OtherEarnings,N_OtherDeductions,X_IqamaNo,X_Transactionnumber,X_Transactionstatus,X_TransDate,X_Department,X_BranchName,X_BranchCode,X_PayrunText,X_IBAN,x_CompanyBank,X_Currency,x_CompanyBankAccountNo from [vw_pay_ProcessedDetails_CSV] where X_Batch='" + x_batchID + "' and N_EmpTypeID<>183 and TransBankID=" + BankID;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dtTransaction = dLayer.ExecuteDataTable(CSVData, Params, connection);
                    int index = 0;
                    foreach (DataRow drow in dtTransaction.Rows)
                    {


                        if (!System.IO.File.Exists(X_WpsFileName))
                        {
                            System.IO.File.Create(X_WpsFileName).Close();
                        }
                        else
                        {
                            System.IO.File.WriteAllText(X_WpsFileName, String.Empty);
                        }
                        string delimiter = ",";
                        string[][] header = new string[][]
                        {new string[]{"Bank Name ","Account Number(34N)","Employee Name","Employee Number","National ID Number (15N)","Salary (15N)","Basic Salary","Housing Allowance","Other Earnings","Deductions","Branch Code","Branch Name","Employee Remarks","Employee Department","Currency","Date","IBAN","Company Bankname","Company Bank Account"},
                        };
                        string[][] output = new string[][]
                        {
                       new string[]{drow["X_BankName"].ToString(),"'"+drow["X_BankAccountNo"].ToString(),drow["X_EmpName"].ToString(),drow["X_EmpCode"].ToString(),"'"+drow["X_IqamaNo"].ToString(),drow["totalsalary"].ToString(),drow["N_BasicSalary"].ToString(),drow["N_HA"].ToString(),drow["N_OtherEarnings"].ToString(),drow["N_OtherDeductions"].ToString(),drow["X_BranchCode"].ToString(),drow["X_BranchName"].ToString(),drow["X_PayrunText"].ToString()+" - Salary",drow["X_Department"].ToString(),drow["X_Currency"].ToString(),drow["X_TransDate"].ToString(),drow["X_IBAN"].ToString(),drow["x_CompanyBank"].ToString(),drow["x_CompanyBankAccountNo"].ToString()}
                     };
                        int length = output.GetLength(0);
                        if (index == 0)
                        {
                            sb.AppendLine(string.Join(delimiter, header[0]));
                        }
                        for (index = 0; index < length; index++)
                            sb.AppendLine(string.Join(delimiter, output[index]));

                    }
                }

                System.IO.File.AppendAllText(X_WpsFileName, sb.ToString());

                return 1;
            }
            catch (Exception ex)
            {
                if (ex is DirectoryNotFoundException)
                {

                    return 0;
                }
                else
                {
                    return 0;
                }
            }


            // string CsvPayrunID = "";
            // string X_CompBankCode = "", X_RefNo = "", X_Currency = "", X_BankAccountNo = "";
            // int N_EDRCount = 0;
            // double N_TotalAmt = 0;
            // int index = 0;
            // SortedList Params = new SortedList();
            // int nCompanyID = myFunctions.GetCompanyID(User);
            // Params.Add("@p1", nCompanyID);
            // Params.Add("@p2", BankID);
            // DataTable CompanyBank = new DataTable();
            // DataTable CSVData = new DataTable();
            // try
            // {


            //     string X_WpsFileName = this.TempFilesPath +myFunctions.GetCompanyID(User)+"-"+ x_batchID.Trim() + ".csv";

            //     StringBuilder sb = new StringBuilder();
            //     DataSet dsCompany = new DataSet();

            //     string CompanyBanksql = "Select * from Vw_BankDetails_rpt where N_CompanyID=" + nCompanyID + " and  N_FnYearID=" + nFnYearID + " and N_BankID=" + BankID;
            //     using (SqlConnection connection = new SqlConnection(connectionString))
            //     {
            //         connection.Open();

            //         CompanyBank = dLayer.ExecuteDataTable(CompanyBanksql, Params, connection);

            //         if (CompanyBank.Rows.Count > 0)
            //         {
            //             foreach (DataRow drow1 in dsCompany.Tables["CompanyBank"].Rows)
            //             {
            //                 X_CompBankCode = drow1["X_AccountHolderName"].ToString();
            //                 X_RefNo = drow1["X_SwiftNo"].ToString();
            //                 X_BankAccountNo = drow1["X_AccountNo"].ToString();
            //                 X_Currency = myCompanyID._CurrencyName;
            //             }
            //         }
            //         object objdate = dLayer.ExecuteScalar("select GETDATE()", Params, connection);

            //         string[][] output1 = new string[][]
            //             {
            //             new string[]{X_CompBankCode,x_batchID,X_BankAccountNo,X_Currency,x_batchID,N_salaryAmt.ToString(),x_batchID.Trim(), Convert.ToDateTime(objdate).ToString("yyyyMMddhhmmsstt"),X_RefNo}
            //             };
            //         string delimiter1 = ",";
            //         int length1 = output1.GetLength(0);
            //         for (index = 0; index < length1; index++)
            //             sb.AppendLine(string.Join(delimiter1, output1[index]));

            //         string CSVDatasql = "Select X_BankName,(N_BasicSalary+N_HA+N_OtherEarnings-N_OtherDeductions)as totalsalary ,X_Address,N_Payrate,X_BankAccountNo,X_EmpName,X_BankCode,X_PaymentDescription,X_ReturnCode,N_BasicSalary,N_HA,N_OtherEarnings,N_OtherDeductions,X_IqamaNo,X_Transactionnumber,X_Transactionstatus,X_TransDate,X_EmpCode from [vw_pay_ProcessedDetails_CSV] where X_Batch='" + x_batchID + "' and N_EmpTypeID<>183";
            //         CSVData = dLayer.ExecuteDataTable(CSVDatasql, Params, connection);
            //         foreach (DataRow drow in CSVData.Rows)
            //         {


            //             if (!System.IO.File.Exists(X_WpsFileName))
            //             {
            //                 System.IO.File.Create(X_WpsFileName).Close();
            //             }
            //             else
            //             {
            //                 System.IO.File.WriteAllText(X_WpsFileName, String.Empty);
            //             }

            //             string delimiter = ",";

            //             string[][] output = new string[][]
            //             {
            //             new string[]{drow["totalsalary"].ToString(),drow["X_BankAccountNo"].ToString(),drow["X_EmpName"].ToString(),drow["X_Address"].ToString(),drow["X_PaymentDescription"].ToString(),drow["X_BankName"].ToString(),drow["N_BasicSalary"].ToString(),drow["N_HA"].ToString(),drow["N_OtherEarnings"].ToString(),drow["N_OtherDeductions"].ToString(),drow["X_EmpCode"].ToString()}
            //          };
            //             int length = output.GetLength(0);

            //             for (index = 0; index < length; index++)
            //                 sb.AppendLine(string.Join(delimiter, output[index]));

            //         }
            //     }

            //     System.IO.File.AppendAllText(X_WpsFileName, sb.ToString());

            //     return 1;
            // }
            // catch (Exception ex)
            // {
            //     if (ex is DirectoryNotFoundException)
            //     {

            //         return 0;
            //     }
            //     else
            //     {
            //         return 0;
            //     }
            // }
        }
        [HttpGet("loadCSV")]
        public async Task<IActionResult> loadCSV(string x_Batch)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@p1", nCompanyID);

            try
            {


                string X_WpsFileName = this.TempFilesPath + myFunctions.GetCompanyID(User) + "-" + x_Batch + ".csv";


                var path = X_WpsFileName;

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, System.IO.FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, _api.GetContentType(path), Path.GetFileName(path));
            }

            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("sendEmail")]
        public ActionResult SendEmail(int nTransID, int nCompanyID, DateTime dProcessDate, int nFnYearID, int nType)
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
                    ProParams.Add("N_PayrunID", dProcessDate.Year.ToString("00##") + dProcessDate.Month.ToString("0#"));
                    ProParams.Add("N_Month", dProcessDate.Month.ToString());
                    ProParams.Add("N_Year", dProcessDate.Year.ToString());
                    ProParams.Add("N_FnYearId", nFnYearID);
                    ProParams.Add("N_Days", DateTime.DaysInMonth(dProcessDate.Year, dProcessDate.Month));
                    ProParams.Add("N_BatchID", 0);

                    Paycodes = dLayer.ExecuteDataTablePro("SP_Pay_SelSalaryDetailsForProcess", ProParams, connection);

                    foreach (DataRow MasterVar in dt.Rows)
                    {
                        string Toemail = MasterVar["X_EmailID"].ToString().Trim();
                        string Subject = "";
                        if (Toemail != "")
                        {
                            string Toemailnames = MasterVar["X_EmpName"].ToString();
                            string Body = Boody(Toemailnames, myFunctions.getIntVAL(MasterVar["N_EmpId"].ToString()), nTransID, dProcessDate, Paycodes, nType);
                            if (nType == 1)
                                Subject = "PAYSLIP (For Your Review)";
                            else
                                Subject = "Your salary payments for the month " + dProcessDate.ToString("MMM-yyyy");
                            myFunctions.SendMail(Toemail, Body, Subject, dLayer,FormID,nTransID,nCompanyID,false);
                            message.Clear();
                        }

                    }
                    return Ok(_api.Success("Payslip Mail Send"));

                }

            }
            catch (Exception e)
            {
                return BadRequest(_api.Error(User, e));

            }
        }
        public string Boody(string Toemailnames, int ManagerID, int transID, DateTime DT, DataTable Datatable, int nType)
        {
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
            SalaryDetails(ManagerID, 0, Datatable);
            message = message.Append("<tr><td> <colspan=3><hr></td><br/><br/>");
            message = message.Append("<tr><td><b>Deductions</td><td><b>" + Total_ddn.ToString(myCompanyID.DecimalPlaceString) + "</b></td>");
            SalaryDetails(ManagerID, 1, Datatable);
            message = message.Append("<tr><td> <colspan=3><hr></td>");
            message = message.Append("<tr><td><b>Net Amount</b></td><td><b>" + Total.ToString(myCompanyID.DecimalPlaceString) + "</b></td>");
            if (nType == 1)
                message = message.Append("<tr><td><i>Please reply to the department if any Concern rises in your review.</i></td>");
            message = message.Append("<tr><left>Sincerly,</left><br><left>" + myFunctions.GetUserName(User) + "</left></table>");
            return message.ToString();
        }
        private void SalaryDetails(int ManagerID, int type, DataTable Datatable)
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
                int nTransID =myFunctions.getIntVAL(MasterTable.Rows[0]["n_TransID"].ToString());
                int nBankID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BankID"].ToString());
                int IsSaveDraft = myFunctions.getIntVAL(MasterTable.Rows[0]["b_IsSaveDraft"].ToString());
                var dCreatedDate = MasterTable.Rows[0]["d_TransDate"].ToString();
                string x_Batch = MasterTable.Rows[0]["x_Batch"].ToString();
                var d_SalFromDate = MasterTable.Rows[0]["d_TransDate"].ToString();
                var d_SalToDate = MasterTable.Rows[0]["d_TransDate"].ToString();
                int N_OldTransID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_TransID"].ToString());
                int N_AddDedID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_RefBatchID"].ToString());
                int year = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PayRunID"].ToString().Substring(0, 4));
                int month = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PayRunID"].ToString().Substring(4, 2));
                double N_TotalSalary = 0;
                string xButtonAction = "";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nPayRunID", nPayRunID);

                    if (!myFunctions.CheckActiveYearTransaction(nCompanyID, nFnYearId, DateTime.ParseExact(MasterTable.Rows[0]["d_TransDate"].ToString(), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture), dLayer, connection, transaction))
                    {
                        object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID=" + nCompanyID + " and convert(date ,'" + MasterTable.Rows[0]["d_TransDate"].ToString() + "') between D_Start and D_End", connection, transaction);
                        if (DiffFnYearID != null)
                        {
                            MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
                            nFnYearId = myFunctions.getIntVAL(DiffFnYearID.ToString());
                            //QueryParams["@nFnYearID"] = nFnYearID;
                        }
                        else
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
                        }
                    }
                    int FormID = 0;
                    int N_IsAuto = 0;
                    int N_TransDetailsID = 0;
                    if (x_Batch.Trim() == "@Auto")
                    {
                        if (!myFunctions.CheckActiveYearTransaction(nCompanyID, nFnYearId, Convert.ToDateTime(MasterTable.Rows[0]["d_TransDate"].ToString()), dLayer, connection, transaction))
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Processing date must be in the active Financial Year."));
                        }
                        bool OK = true;
                        int NewNo = 0, loop = 1;
                        while (OK)
                        {
                            NewNo = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(count(1),0) + " + loop + " As Count FRom Pay_PaymentMaster Where N_CompanyID=@nCompanyID  And N_PayRunID =@nPayRunID", Params, connection, transaction).ToString());
                            x_Batch = nPayRunID + "" + NewNo.ToString("0#");
                            xButtonAction = "Insert";
                            if (myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(count(1),0) FRom Pay_PaymentMaster Where N_CompanyID=@nCompanyID And X_Batch = '" + x_Batch + "'", Params, connection, transaction).ToString()) == 0)
                            {
                                OK = false;
                            }
                            loop += 1;
                        }
                        if (x_Batch == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to generate batch"));

                        }
                        MasterTable.Rows[0]["x_Batch"] = x_Batch;
                    }
                    else
                    {
                        xButtonAction = "Update";
                    }
                    x_Batch = MasterTable.Rows[0]["x_Batch"].ToString();



                    if (MasterTable.Columns.Contains("N_FormID"))
                        MasterTable.Rows[0]["N_FormID"] = 190;
                    else
                    {
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_FormID", typeof(int), 190);
                        MasterTable.AcceptChanges();
                    }

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " And N_FnyearID = " + nFnYearId + " and X_Batch='" + x_Batch + "'";
                    int N_TransID = dLayer.SaveData("Pay_PaymentMaster", "N_TransID", DupCriteria, "", MasterTable, connection, transaction);
                    if (N_TransID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }
                    else
                    {
                        //Delete Existing Data
                        dLayer.DeleteData("Pay_PaymentDetails", "N_TransID", N_TransID, "N_CompanyID =" + nCompanyID + " and N_FormID=190", connection, transaction);
                        dLayer.ExecuteScalar("Update Pay_LoanIssueDetails Set N_RefundAmount =Null,D_RefundDate =Null,N_PayRunID =Null,N_TransDetailsID =Null,B_IsLoanClose =Null  Where N_CompanyID =" + nCompanyID + " and N_PayrunID = " + N_TransID, connection, transaction);

                        int row = 0;
                        foreach (DataRow MasterVar in EmployeesTable.Rows)
                        {

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
                                    return Ok(_api.Error(User, "-ve Salary"));
                                }
                            }
                        }
                        if (DetailsTable.Columns.Contains("n_PayTypeID"))
                            DetailsTable.Columns.Remove("n_PayTypeID");
                        if (DetailsTable.Columns.Contains("n_IsLoan"))
                            DetailsTable.Columns.Remove("n_IsLoan");
                        if (DetailsTable.Columns.Contains("n_LoanTransDetailsID"))
                            DetailsTable.Columns.Remove("n_LoanTransDetailsID");
                        DetailsTable.AcceptChanges();
                        N_TransDetailsID = dLayer.SaveData("Pay_PaymentDetails", "N_TransDetailsID", DetailsTable, connection, transaction);
                        if (myFunctions.getIntVAL(N_TransDetailsID.ToString()) <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Error"));

                        }

                        //Activity Log
                        string ipAddress = "";
                        if (Request.Headers.ContainsKey("X-Forwarded-For"))
                            ipAddress = Request.Headers["X-Forwarded-For"];
                        else
                            ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                        myFunctions.LogScreenActivitys(nFnYearId, N_TransID, x_Batch, 190, xButtonAction, ipAddress, "", User, dLayer, connection, transaction);





                        if (N_TransDetailsID > 0 && IsSaveDraft == 0)
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
                        GenerateCSVFiles(nBankID, nCompanyID, x_Batch, nFnYearId, N_TotalSalary.ToString(), Convert.ToDateTime(dCreatedDate.ToString()), Convert.ToDateTime(d_SalFromDate), Convert.ToDateTime(d_SalToDate));
                        return Ok(_api.Success("Saved"));

                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nTransID, int nFnYearID, string xBatch, int nCompanyID, int nPayRunID)
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
                    SortedList ParamList = new SortedList();
                    DataTable TransData = new DataTable();
                    ParamList.Add("@nFnYearID", nFnYearID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    ParamList.Add("@nTransID", nTransID);
                    string xButtonAction = "Delete";
                    string x_Batch = "";
                    string Sql = "select n_TransID,x_Batch from Pay_PaymentMaster where n_TransID=@nTransID and N_CompanyID=@nCompanyID";

                    int count = myFunctions.getIntVAL(dLayer.ExecuteNonQuery("Select count(1) from Acc_VoucherMaster Where N_CompanyID=" + myFunctions.GetCompanyID(User) + " And N_FnyearID =@nFnYearID and X_TransType = 'ESI' and B_IsAccPosted = 1 and X_ReferenceNo=@xBatch", dltParams, connection, transaction).ToString());
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection, transaction);

                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    if (count > 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete ,Transactions Exist"));

                    }

                    //  Activity Log
                    string ipAddress = "";
                    if (Request.Headers.ContainsKey("X-Forwarded-For"))
                        ipAddress = Request.Headers["X-Forwarded-For"];
                    else
                        ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    myFunctions.LogScreenActivitys(myFunctions.getIntVAL(nFnYearID.ToString()), nTransID, TransRow["x_Batch"].ToString(), 190, xButtonAction, ipAddress, "", User, dLayer, connection, transaction);



                    dLayer.ExecuteNonQuery("Update Pay_LoanIssueDetails Set N_RefundAmount =Null,D_RefundDate =Null,N_PayRunID =Null,N_TransDetailsID =Null,B_IsLoanClose =Null  Where N_CompanyID =" + myFunctions.GetCompanyID(User) + " and N_PayrunID = " + nTransID, connection, transaction);
                    dLayer.ExecuteNonQuery("SP_Pay_SalryProcessingVoucher_Del " + myFunctions.GetCompanyID(User) + ",@nFnYearID,'ESI',@xBatch", dltParams, connection, transaction);
                    Results = dLayer.DeleteData("Pay_PaymentDetails", "N_TransID", nTransID, "N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FormID = 190", connection, transaction);

                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete batch"));
                    }
                    Results = dLayer.DeleteData("Pay_PaymentMaster", "N_TransID", nTransID, "N_CompanyID=" + myFunctions.GetCompanyID(User), connection, transaction);

                    if (Results <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete batch"));
                    }

                    transaction.Commit();
                    return Ok(_api.Success("Batch deleted"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }

        }

    }
}