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
    [Route("medicalInsuranceInvoicing")]
    [ApiController]
    public class Pay_MedicalInsuranceInvoicing : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Pay_MedicalInsuranceInvoicing(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1132;
        }
     

        [HttpGet("additionList")]
        public ActionResult GetPolicyList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_MedicalInsuranceAdditionSearch where N_CompanyID=" + nCompanyID + " ";
            Params.Add("@nCompanyID", nCompanyID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
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
         [HttpGet("dashboardList")]
        public ActionResult DashboardList(int nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
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
                Searchkey = "and X_InvoiceCode like '%" + xSearchkey + "%'";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_InvoiceID asc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ")  N_InvoiceID,X_InvoiceCode,X_PolicyNo,X_VendorName,X_CardNo,X_StartDate,X_EndDate from vw_MedicalInsuranceInvoiceSearch where N_CompanyID=@nCompanyId  group By  N_InvoiceID,X_InvoiceCode,X_PolicyNo,X_VendorName,X_CardNo,X_StartDate,X_EndDate " + Searchkey + Criteria + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_InvoiceID,X_InvoiceCode,X_PolicyNo,X_VendorName,X_CardNo,X_StartDate,X_EndDate from vw_MedicalInsuranceInvoiceSearch where N_CompanyID=@nCompanyId " + Searchkey + Criteria + " and N_InvoiceID not in (select top(" + Count + ") N_InvoiceID from vw_MedicalInsuranceInvoiceSearch where N_CompanyID=@nCompanyId  group By  N_InvoiceID,X_InvoiceCode,X_PolicyNo,X_VendorName,X_CardNo,X_StartDate,X_EndDate  " + Criteria + xSortBy + " ) " + xSortBy;
            Params.Add("@nCompanyId", nCompanyId);

            SortedList OutPut = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*)  as N_Count  from vw_MedicalInsuranceInvoiceSearch where N_CompanyID=@nCompanyId " + Searchkey + Criteria;
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
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("employeeDetails")]
        public ActionResult GetEmpDetails(int nAdditionCode, int nFnYearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    SortedList EmpParams = new SortedList();

                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int N_AdditionID = 0;

                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nFnYearID", nFnYearID);


                    EmpParams.Add("@nCompanyID", nCompanyID);
                    EmpParams.Add("@nFnYearID", nFnYearID);

                    DataTable AdditionTable = new DataTable();
                    DataTable AdditionDetailTable = new DataTable();
                    string EmployeeSql = "";
                    string AdditionSql = "";

                    EmployeeSql = " Select * From Pay_MedicalInsuranceAddition Where N_CompanyID=" + nCompanyID + " and X_PolicyCode='" + nAdditionCode + "'";
                    AdditionTable = dLayer.ExecuteDataTable(EmployeeSql, EmpParams, connection);
                    if (AdditionTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    N_AdditionID = myFunctions.getIntVAL(AdditionTable.Rows[0]["N_AdditionId"].ToString());
                    AdditionTable = myFunctions.AddNewColumnToDataTable(AdditionTable, "X_ProjectName", typeof(string), "");
                    if ((AdditionTable.Rows[0]["N_ProjectID"].ToString()) != "")
                    {
                        object X_ProjectName = dLayer.ExecuteScalar("Select X_ProjectName From vw_ProjectAndCompany Where N_CompanyID=" + nCompanyID + " and N_ProjectID=" + myFunctions.getIntVAL(AdditionTable.Rows[0]["N_ProjectID"].ToString()), EmpParams, connection);
                        AdditionTable.Rows[0]["X_ProjectName"] = (X_ProjectName.ToString());
                    }

                    AdditionSql = "Select * From vw_MedicalInsuranceAddition_Invoicing Where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + "and N_AdditionId=" + N_AdditionID + " Order by N_EmpID ";
                    AdditionDetailTable = dLayer.ExecuteDataTable(AdditionSql, EmpParams, connection);
                    if (AdditionDetailTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }


                    dt.Tables.Add(AdditionTable);
                    dt.Tables.Add(AdditionDetailTable);

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
                    SortedList QryParams = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nInvoiceID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_InvoiceID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    string X_InvoiceCode = MasterTable.Rows[0]["x_InvoiceCode"].ToString();
                    int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                    int N_MedicalInsID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_MedicalInsID"].ToString());
                    QryParams.Add("N_CompanyID", nCompanyID);


                    DocNo = MasterRow["x_PolicyCode"].ToString();
                    if (X_InvoiceCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", FormID);
                        Params.Add("N_YearID", nFnYearID);
                        while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Pay_MedicalInsuranceInvoicing Where X_PolicyCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_InvoiceCode = DocNo;
                        if (X_InvoiceCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate")); }
                        MasterTable.Rows[0]["x_InvoiceCode"] = X_InvoiceCode;

                    }
                    if (nInvoiceID > 0)
                    {
                        dLayer.ExecuteNonQuery("SP_Delete_Trans_With_PurchaseAccounts " + nCompanyID.ToString() + ",'INSURANCE INVOICING'," + nInvoiceID.ToString() + ",0,'',0", connection, transaction);
                        DeleteAmortization(nInvoiceID, MasterTable, connection, transaction);
                    }
                    string DupCriteria = "";
                    string X_Criteria = "";

                    nInvoiceID = dLayer.SaveData("Pay_MedicalInsuranceInvoicing", "N_InvoiceID", DupCriteria, X_Criteria, MasterTable, connection, transaction);
                    if (nInvoiceID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    }
                    for (int i = DetailTable.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow mstVar = DetailTable.Rows[i];

                        DetailTable.Rows[i]["N_AdditionID"] = nInvoiceID;


                    }

                    int nInvoiceDetailsID = dLayer.SaveData("Pay_MedicalInsuranceInvoicingDetails", "N_InvoiceDetailsID", DetailTable, connection, transaction);
                    if (nInvoiceDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    }
                    foreach (DataRow var in DetailTable.Rows)
                    {
                        int i = 0;
                        if (i > 0)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            i = 0;

                        }

                        int paycodeID = 0;
                        int n_empid = myFunctions.getIntVAL(var["n_EmpID"].ToString());
                        object Prj = dLayer.ExecuteScalar("Select N_ProjectID From Pay_Employee Where N_EmpID ='" + n_empid + "' and N_CompanyID=" + nCompanyID, QryParams, connection, transaction);
                        if (Prj != null)
                        {
                            int ProjectID = myFunctions.getIntVAL(Prj.ToString());
                            if (ProjectID > 0)
                            {
                                object Paycode = dLayer.ExecuteScalar("Select N_PayCodeID From Prj_ProjectParameters Where N_ProjectID ='" + ProjectID + "' and N_CompanyID=" + nCompanyID, QryParams, connection, transaction);
                                if (Paycode != null)
                                {
                                    paycodeID = myFunctions.getIntVAL(Paycode.ToString());
                                }
                                else
                                {
                                    object PolicyPaycode = dLayer.ExecuteScalar("Select N_PayCodeID From Pay_Medical_Insurance Where N_MedicalInsID ='" + N_MedicalInsID + "' and N_CompanyID=" + nCompanyID, QryParams, connection, transaction);
                                    if (PolicyPaycode != null)
                                    {
                                        paycodeID = myFunctions.getIntVAL(PolicyPaycode.ToString());
                                    }

                                }
                            }
                            else
                            {
                                object PolicyPaycode = dLayer.ExecuteScalar("Select N_PayCodeID From Pay_Medical_Insurance Where N_MedicalInsID ='" + N_MedicalInsID + "' and N_CompanyID=" + nCompanyID, QryParams, connection, transaction);
                                if (PolicyPaycode != null)
                                {
                                    paycodeID = myFunctions.getIntVAL(PolicyPaycode.ToString());
                                }
                            }
                        }
                        else
                        {
                            object PolicyPaycode = dLayer.ExecuteScalar("Select N_PayCodeID From Pay_Medical_Insurance Where N_MedicalInsID ='" + N_MedicalInsID + "' and N_CompanyID=" + nCompanyID, QryParams, connection, transaction);
                            if (PolicyPaycode != null)
                            {
                                paycodeID = myFunctions.getIntVAL(PolicyPaycode.ToString());
                            }
                        }
                        bool B_Amortized = false;
                        bool B_isPrePaid = false;
                        bool B_isOnetimeInvoice = false;
                        bool B_isInvoice = false;
                        object Ammortized = dLayer.ExecuteScalar("select B_Amortized from Pay_PayMaster  Where N_PayID =" + paycodeID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, QryParams, connection, transaction);
                        object PrePaid = dLayer.ExecuteScalar("select B_isPrePaid from Pay_PayMaster  Where N_PayID =" + paycodeID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, QryParams, connection, transaction);
                        object isOnetimeInvoice = dLayer.ExecuteScalar("select B_isOnetimeInvoice from Pay_PayMaster  Where N_PayID =" + paycodeID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, QryParams, connection, transaction);
                        object isInvoice = dLayer.ExecuteScalar("select B_isInvoice from Pay_PayMaster  Where N_PayID =" + paycodeID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, QryParams, connection, transaction);

                        if (isOnetimeInvoice != null)
                            B_isOnetimeInvoice = myFunctions.getBoolVAL(isOnetimeInvoice.ToString());

                        if (Ammortized != null)
                            B_Amortized = myFunctions.getBoolVAL(Ammortized.ToString());
                        if (PrePaid != null)
                            B_isPrePaid = myFunctions.getBoolVAL(PrePaid.ToString());

                        if (isInvoice != null)
                            B_isInvoice = myFunctions.getBoolVAL(isInvoice.ToString());

                        if (B_isPrePaid)
                        {
                            //SaveAmortization(nInvoiceID, N_MedicalInsID, paycodeID, n_empid, i, "S", false, myFunctions.getIntVAL(var["n_InvoiceDetailsID"].ToString()), MasterTable, DetailTable, connection, transaction);
                        }
                        if (B_isInvoice)
                        {
                            //SaveAmortization(plPkeyID, N_MedicalInsID, paycodeID, n_empid, i, "I", B_isOnetimeInvoice, myFunctions.getIntVAL(ObjInvoiceDetailsID.ToString()));
                        }
                        //UPDATING INSURENCE IN EMPLOYEE/DEPENDENCE
                        try
                        {
                            // int n_insclassid = myFunctions.getIntVAL(flxMedicalInsurance.get_TextMatrix(i, mcInsClassID));
                            // if (myFunctions.getIntVAL(flxMedicalInsurance.get_TextMatrix(i, mcDependentID)) > 0)
                            //     UpdateInsuranceInEmployee("save", i, myFunctions.getIntVAL(flxMedicalInsurance.get_TextMatrix(i, mcDependentID)), "dep", N_MedicalInsID, n_insclassid, flxMedicalInsurance.get_TextMatrix(i, mcStartDate), flxMedicalInsurance.get_TextMatrix(i, mcEndDate));
                            // else
                            //     UpdateInsuranceInEmployee("save", i, myFunctions.getIntVAL(flxMedicalInsurance.get_TextMatrix(i, mcEmpID).ToString()), "emp", N_MedicalInsID, n_insclassid, flxMedicalInsurance.get_TextMatrix(i, mcStartDate), flxMedicalInsurance.get_TextMatrix(i, mcEndDate));
                        }
                        catch (Exception e)
                        {
                            return Ok(_api.Error(e));
                        }
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
        private void DeleteAmortization(int N_InvoiceID, DataTable MasterTable, SqlConnection connection, SqlTransaction transaction)
        {
            int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
            int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
            SortedList EmpParams = new SortedList();
            EmpParams.Add("@nCompanyID", nCompanyID);
            object res = dLayer.ExecuteScalar("select N_PrePayScheduleID from Inv_PrePaymentScheduleMaster where N_PrePaymentID =" + N_InvoiceID + " and N_FormID =1251  and N_CompanyID =" + nCompanyID + " and N_fnyearID =" + nFnYearID, EmpParams, connection, transaction);
            if (res != null)
            {
                dLayer.ExecuteNonQuery("delete from Inv_PrePaymentSchedule where N_PrePayScheduleID in(select N_PrePayScheduleID from Inv_PrePaymentScheduleMaster where N_PrePaymentID =" + N_InvoiceID + " and N_FormID =1251  and N_CompanyID =" + nCompanyID + " and N_fnyearID =" + nFnYearID + ")", EmpParams, connection, transaction);
                dLayer.ExecuteNonQuery("delete from Inv_PrePaymentScheduleMaster where N_PrePaymentID =" + N_InvoiceID + " and N_FormID =1251  and N_CompanyID =" + nCompanyID + " and N_fnyearID =" + nFnYearID, EmpParams, connection, transaction);
                //dba.DeleteData("Inv_PrePaymentSchedule", "N_PrePayScheduleID", N_AmortizationID.ToString(), "");
                //dba.DeleteData("Inv_PrePaymentScheduleMaster", "N_PrePayScheduleID", N_AmortizationID.ToString(), "N_CompanyID=" + myCompanyID._CompanyID);
            }
        }
        private void SaveAmortization(int _additionid, int _policyid, int _paycodeid, int _empid, int _row, string _type, bool B_OneTme, int _DetailID, DataTable MasterTable, DataTable DetailTable, SqlConnection connection, SqlTransaction transaction)
        {
            int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
            int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
            int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
            int nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_UserID"].ToString());
            //Datetime dtpInvoiceDate = Convert.ToDateTime(MasterTable.Rows[0]["D_Date"].ToString());

            SortedList EmpParams = new SortedList();
            EmpParams.Add("@nCompanyID", nCompanyID);
            bool result = false;
            int AmortizationID_Loc = 0;
            DataTable amortizationTable;

            DataTable MedIns;
            DataTable NewTable;



            int N_DependentID = myFunctions.getIntVAL(DetailTable.Rows[_row]["n_DependenceID"].ToString());


            try
            {
                int frquency = 0, payid = 0, frequencyCount = 0, N_CategoryID = 0, N_LedgerID = 0;
                double AmtSplit = 0;


                if (_type == "I")
                {
                    if (B_OneTme)
                    {
                        object res = dLayer.ExecuteScalar("select N_AmrIncLedgerID FROM Pay_PayMaster WHERE N_PayID =" + _paycodeid + "  AND N_CompanyID =" + nCompanyID, EmpParams, connection, transaction);
                        if (res != null)
                            N_LedgerID = myFunctions.getIntVAL(res.ToString());
                    }
                    else
                    {
                        object res = dLayer.ExecuteScalar("select N_IncomeLedgerID FROM Pay_PayMaster WHERE N_PayID =" + _paycodeid + "  AND N_CompanyID =" + nCompanyID, EmpParams, connection, transaction);
                        if (res != null)
                            N_LedgerID = myFunctions.getIntVAL(res.ToString());
                    }
                }
                if (_type == "S")
                {
                    int DepratmentID = 0;
                    object DepID = dLayer.ExecuteScalar("select N_DepartmentID FROM Pay_Employee WHERE N_EmpID =" + _empid + "  AND N_CompanyID =" + nCompanyID, EmpParams, connection, transaction);
                    if (DepID != null)
                        DepratmentID = myFunctions.getIntVAL(DepID.ToString());
                    object res = dLayer.ExecuteScalar("select dbo.SP_GetPayLedgerIdEmployee(" + nCompanyID + "," + nFnYearID + "," + nFnYearID + "," + _paycodeid + ",'Expense')", EmpParams, connection, transaction);
                    if (res != null)
                        N_LedgerID = myFunctions.getIntVAL(res.ToString());
                }


                double amt = 0;
                if (_type == "I")
                    amt = myFunctions.getVAL(DetailTable.Rows[_row]["n_Price"].ToString());
                else if (_type == "S")
                    amt = myFunctions.getVAL(DetailTable.Rows[_row]["n_Cost"].ToString());

                DateTime Start = new DateTime();
                DateTime DtpAdd = new DateTime();

                DtpAdd = Convert.ToDateTime(DetailTable.Rows[_row]["d_AdditionDate"].ToString());
                DateTime DtpEndDate = Convert.ToDateTime(DetailTable.Rows[_row]["d_EndDate"].ToString()).AddDays(1);

                frquency = ((DtpEndDate.Year - DtpAdd.Year) * 12) + DtpEndDate.Month - DtpAdd.Month;
                Start = new DateTime(DtpAdd.Year, DtpAdd.Month, 1);
                if (B_OneTme)
                    frquency = 1;


                object ObjAmortizationID = 0;
                object N_AmortizationDetailsID = 0;
                object lobjResult = null;
                lobjResult = 0;
                String PrjInv = "Insurance Addition";

                string qry = "Select " + nCompanyID + " as N_CompanyID," + nFnYearID + " as N_FnYearID," + _additionid + " as N_PrePaymentID," + nBranchID + " as N_BranchID," + nUserID + " as N_UserID ,'" + PrjInv + "' as  X_Type," + 1251 + " as N_FormID," + N_LedgerID + " as N_LedgerID ," + _paycodeid + " as N_PayID ,'" + _type + "' as  X_EntryType," + _empid + " as N_EmpID," + amt + " as N_InvoiceAmt,'" + MasterTable.Rows[0]["D_Date"] + "' as D_Date," + frquency + " as N_Frequency ," + _DetailID + " as N_InvoiceDetailsID ,'" + N_DependentID + "' as  N_DependentID ";
                amortizationTable = dLayer.ExecuteDataTable(qry, EmpParams, connection, transaction);
                //string FieldList = "N_CompanyID, N_FnYearID, N_PrePaymentID, N_BranchID, N_UserID,X_Type,N_FormID,N_LedgerID,N_PayID,X_EntryType,N_EmpID,N_InvoiceAmt,D_Date,N_Frequency,N_InvoiceDetailsID,N_DependentID";
                //string FieldValues = myCompanyID._CompanyID + "|" + myCompanyID._FnYearID + "|" + _additionid + "|" + myCompanyID._BranchID + "|" + myCompanyID._UserID + "|'Insurance Addition'|" + MYG.ReturnFormID(this.Text) + "|" + N_LedgerID + "|" + _paycodeid + "|'" + _type + "'|" + _empid + "|" + amt + "|'" + myFunctions.getDateVAL(dtpInvoiceDate.Value.Date) + "'|" + frquency + "|" + _DetailID + "|" + N_DependentID;
                lobjResult = dLayer.SaveData("Inv_PrePaymentScheduleMaster", "N_PrePayScheduleID", amortizationTable, connection, transaction);
                // dba.SaveData(ref lobjResult, "Inv_PrePaymentScheduleMaster", "N_PrePayScheduleID", AmortizationID_Loc.ToString(), FieldList, FieldValues, RefField, RefFieldDescr, DupCriteria, "", "N_CompanyID=" + myCompanyID._CompanyID + " and N_FnYearID=" + myCompanyID._FnYearID);
                if (myFunctions.getIntVAL(lobjResult.ToString()) > 0)
                {
                    ObjAmortizationID = myFunctions.getIntVAL(lobjResult.ToString());

                    DtpAdd = Convert.ToDateTime(DetailTable.Rows[_row]["d_AdditionDate"]);
                    DtpEndDate = Convert.ToDateTime(DetailTable.Rows[_row]["d_EndDate"]).AddDays(1);

                    frquency = ((DtpEndDate.Year - DtpAdd.Year) * 12) + DtpEndDate.Month - DtpAdd.Month;
                    Start = new DateTime(DtpAdd.Year, DtpAdd.Month, 1);
                    //}
                    if (B_OneTme)
                        frquency = 1;

                    if (_type == "I")
                        AmtSplit = myFunctions.getVAL(DetailTable.Rows[_row]["n_Price"].ToString()) / frquency;
                    else if (_type == "S")
                        AmtSplit = myFunctions.getVAL(DetailTable.Rows[_row]["n_Cost"].ToString()) / frquency;




                    for (int j = 1; j <= frquency; j++)
                    {
                        if (j > 1)
                        {
                            Start = new DateTime(DtpAdd.AddMonths(1).Year, DtpAdd.AddMonths(j - 1).Month, 1);
                            if (DtpAdd.AddMonths(j - 1).Month == 12) { DtpAdd = DtpAdd.AddYears(1); }
                        }
                        DateTime End = new DateTime(Start.AddMonths(1).Year, Start.AddMonths(1).Month, 1).AddDays(-1);
                        N_AmortizationDetailsID = 0;

                        string newqry = "Select " + nCompanyID + " as N_CompanyID," + _additionid + " as N_PrePaymentID," + ObjAmortizationID + " as N_PrePayScheduleID,'" + myFunctions.getDateVAL(Start) + "' as D_DateFrom,'" + myFunctions.getDateVAL(End) + "' as D_DateTo ," + AmtSplit + " as  N_InstAmount," + DetailTable.Rows[_row]["n_EmpID"] + " as N_RefID," + _paycodeid + " as N_PaycodeID ";
                        NewTable = dLayer.ExecuteDataTable(newqry, EmpParams, connection, transaction);
                        // FieldList  "N_CompanyID,N_PrePaymentID,N_PrePayScheduleID,D_DateFrom,D_DateTo,N_InstAmount,N_RefID,N_PaycodeID";
                        //FieldValues = myCompanyID._CompanyID + "|" + _additionid + "|" + ObjAmortizationID + "|'" + myFunctions.getDateVAL(Start) + "'|'" + myFunctions.getDateVAL(End) + "'|" + AmtSplit + "|" + flxMedicalInsurance.get_TextMatrix(_row, mcEmpID) + "|" + _paycodeid;
                        N_AmortizationDetailsID = dLayer.SaveData("Inv_PrePaymentSchedule", "N_PrePaymentDetailsID", NewTable, connection, transaction);
                        //dba.SaveData(ref N_AmortizationDetailsID, "Inv_PrePaymentSchedule", "N_PrePaymentDetailsID", N_AmortizationDetailsID.ToString(), FieldList, FieldValues, RefField, RefFieldDescr, DupCriteria, "");
                        if (myFunctions.getIntVAL(N_AmortizationDetailsID.ToString()) <= 0)
                        {
                            transaction.Rollback();
                           // return Ok(_api.Error("Unable To Save Amortization"));
                        }
                    }

                }
            }
            catch (Exception ex)
            {
               // return Ok(_api.Error(ex));
            }

        }







    }
}























//             }





































