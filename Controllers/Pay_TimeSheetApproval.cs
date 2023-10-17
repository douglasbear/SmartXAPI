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
    [Route("timeSheetApproval")]
    [ApiController]
    public class Pay_TimeSheetApproval : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        StringBuilder message = new StringBuilder();
        public Pay_TimeSheetApproval(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 216;

        }

        [HttpGet("list")]
        public ActionResult GetTimsheetList(int? nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_PayrunText like '%" + xSearchkey + "%' or X_BatchCode like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_BatchCode desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_PayrunText":
                        xSortBy = "X_PayrunText " + xSortBy.Split(" ")[1];
                        break;
                    case "X_BatchCode":
                        xSortBy = "X_BatchCode" + xSortBy.Split(" ")[1];
                        break;
                    case "D_DateFrom":
                        xSortBy = "Cast(D_DateFrom as DateTime )" + xSortBy.Split(" ")[1];
                        break;
                    case "D_DateTo":
                        xSortBy = "Cast(D_DateTo as DateTime )" + xSortBy.Split(" ")[1];
                        break;
                    case "D_SalaryDate":
                        xSortBy = "Cast(D_SalaryDate as DateTime )" + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from Pay_TimeSheetApproveMaster where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from Pay_TimeSheetApproveMaster where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and N_TimeSheetApproveID not in (select top(" + Count + ") N_TimeSheetApproveID from Pay_TimeSheetApproveMaster where N_CompanyID=@p1 and N_FnYearID=@p2 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearID);
            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count  from Pay_TimeSheetApproveMaster where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "";
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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("employeeList")]
        public ActionResult GetEmpList(int nFnYearID, bool b_AllBranchData, int nBranchID, int nAdditionPayID, int nDeductionPayID, int nDefaultAbsentID, int payRunID, int batchcode)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();

                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    DataTable EmpTable = new DataTable();
                    DataTable defaultPaycode = new DataTable();
                      string empSql = "";
                    string criteria="";
                    if(payRunID!=0){
                    criteria= " and N_EMPID not in (select N_EmpID from Pay_TimeSheetMaster where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_BatchID=" + batchcode + " and ISNULL(N_TotalWorkingDays,0)>0)";
                    }
                    if (b_AllBranchData == true)
                        empSql = "select N_CompanyID,N_EmpID,N_BranchID,N_Status,N_FnYearID,N_CatagoryId,X_DefEmpCode,X_Position,X_Department,N_DepartmentID,N_PositionID,Name as X_EmpName,[Employee Code] as X_EmpCode from vw_PayEmployee_Disp where isnull(B_ExcludeInAttendance,0)=0 and  N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and (N_Status = 0 OR N_Status = 1) "+criteria+" group by N_CompanyID,N_EmpID,N_BranchID,N_Status,N_FnYearID,N_CatagoryId,X_DefEmpCode,X_Position,X_Department,N_DepartmentID,N_PositionID,Name,[Employee Code]  order by X_EmpCode";
                    else
                        empSql = "select N_CompanyID,N_EmpID,N_BranchID,N_Status,N_FnYearID,N_CatagoryId,X_DefEmpCode,X_Position,X_Department,N_DepartmentID,N_PositionID,Name as X_EmpName,[Employee Code] as X_EmpCode from vw_PayEmployee_Disp where isnull(B_ExcludeInAttendance,0)=0 and  N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and (N_Status = 0 OR N_Status = 1) "+criteria+" group by N_CompanyID,N_EmpID,N_BranchID,N_Status,N_FnYearID,N_CatagoryId,X_DefEmpCode,X_Position,X_Department,N_DepartmentID,N_PositionID,Name,[Employee Code]  order by X_EmpCode";

                    EmpTable = dLayer.ExecuteDataTable(empSql, Params, connection);
                    if (EmpTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    EmpTable.AcceptChanges();

                    defaultPaycode.Clear();
                    defaultPaycode.Columns.Add("X_Addition");
                    defaultPaycode.Columns.Add("X_Deductions ");
                    defaultPaycode.Columns.Add("X_DefaultAbsentCode");
                    //defaultPaycode.Columns.Add("txtAdjustment");

                    object additions = dLayer.ExecuteScalar("Select X_Description from Pay_PayMaster Where N_PayID =" + nDeductionPayID + "and N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + "", Params, connection);
                    object deductions = dLayer.ExecuteScalar("Select X_Description from Pay_PayMaster Where N_PayID =" + nAdditionPayID + "and N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + "", Params, connection);
                    object AbsentCode = dLayer.ExecuteScalar("Select X_VacType from Pay_VacationType Where N_VacTypeID =" + nDefaultAbsentID + "and N_CompanyID= " + nCompanyID, Params, connection);
                    //object obj = dLayer.ExecuteScalar("Select X_Description from vw_PayCodelist_MonthWise Where N_PayRunID =" + payRunID + " and N_CompanyID= " + nCompanyID + " and N_EmpID=" + nEmpID, Params, connection);



                    DataRow row = defaultPaycode.NewRow();
                    if (additions != null)
                        row["X_Addition"] = additions.ToString();
                    if (deductions != null)
                        row["X_Deductions "] = deductions.ToString();
                    if (AbsentCode != null)
                        row["X_DefaultAbsentCode"] = AbsentCode.ToString();
                    //row["txtAdjustment "] = obj.ToString();
                    defaultPaycode.Rows.Add(row);


                    EmpTable = _api.Format(EmpTable, "EmpTable");
                    defaultPaycode = _api.Format(defaultPaycode, "defaultPaycode");
                    dt.Tables.Add(EmpTable);
                    dt.Tables.Add(defaultPaycode);
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }

        }
        public double HoursToMinutes(double Hours)
        {
            double Minutes = 0;
            Minutes = (((int)Hours * 60) + (Hours % 1) % .60 * 100);
            return Minutes;
        }

        public double MinutesToHours(double Minutes)
        {
            double Hours = 0;
            //Minutes = Round(Minutes, 2);
            Hours = (((int)Minutes / 60) + (Minutes % 60) / 100);
            return Hours;
        }

        [HttpGet("employeeDetails")]
        public ActionResult GetEmpDetails(int nFnYearID, int nEmpID, int nCategoryID, string payRunID, DateTime dtpFromdate, DateTime dtpTodate, DateTime systemDate,bool isTimesheet)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    SortedList secParams = new SortedList();
                    SortedList payRateParams = new SortedList();
                    SortedList payParams = new SortedList();
                    bool bCategoryWiseDeduction = false;
                    bool bCategoryWiseAddition = false;
                    bool bCategoryWiseComp = false;

                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    DataTable ElementsTable = new DataTable();
                    DataTable EmpGrpWorkhours = new DataTable();
                    DataTable settingsTable = new DataTable();
                    DataTable PayAttendence = new DataTable();
                    DataTable PayOffDays = new DataTable();
                    DataTable PayWorkingHours = new DataTable();
                    DataTable SummaryTable = new DataTable();
                    DataTable payRate = new DataTable();

                    DataTable TimeSheetMaster = new DataTable();
                    DataTable TimeSheetDetails = new DataTable();

                    int N_AdditionPayID = 0;
                    string X_Additions = "";
                    int N_DeductionPayID = 0;
                    string X_Deductions = "";
                    int N_DefaultAbsentID = 0;
                    string X_DefaultAbsentCode = "";
                    int N_TimeSheetID = 0;
                    int N_BatchID = 0;
                     int count=0;

                    SortedList Master = new SortedList();

                    Double N_Diffrence = 0, N_NonDedApp = 0, txtAdjustment = 0, N_WorkdHrs = 0, N_WorkHours = 0,N_TotalDays=0;

                    object N_Result;
                    object nCatID;
                    object nBatchID;
                    if(isTimesheet==true && nCategoryID == 0){
                    nCatID = dLayer.ExecuteScalar("Select n_CatagoryId from Pay_Employee Where  N_CompanyID= " + nCompanyID + " and N_EmpID=" + nEmpID + " and N_FnYearID=" + nFnYearID + "", Params, connection);
                    if (nCatID != null){
                        nCategoryID=myFunctions.getIntVAL(nCatID.ToString());;
                    }
                    //  nBatchID = dLayer.ExecuteScalar("Select n_BatchID from Pay_TimeSheetMaster Where  N_CompanyID= " + nCompanyID + " and N_EmpID=" + nEmpID + " and N_FnYearID=" + nFnYearID + "", Params, connection);
                    //   if (nCatID != null){
                    //     payRunID = nBatchID.ToString();
                    // }
                    }

                    N_Result = dLayer.ExecuteScalar("Select N_Value from Gen_Settings Where X_Description ='Default Addition' and N_CompanyID= " + nCompanyID + " and X_Group='HR'", Params, connection);
                    if (N_Result != null)
                    {
                        if (myFunctions.getIntVAL(N_Result.ToString()) != 0)
                        {
                            N_AdditionPayID = myFunctions.getIntVAL(N_Result.ToString());
                            object additions = dLayer.ExecuteScalar("Select X_Description from Pay_PayMaster Where N_PayID =" + N_AdditionPayID + "and N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + "", Params, connection);
                            if (additions != null)
                                X_Additions = additions.ToString();
                        }
                    }
                    else
                        X_Additions = "";
                    N_Result = dLayer.ExecuteScalar("Select N_Value from Gen_Settings Where X_Description ='Default Deduction' and N_CompanyID= " + nCompanyID + " and X_Group='HR'", Params, connection);
                    if (N_Result != null)
                    {
                        if (myFunctions.getIntVAL(N_Result.ToString()) != 0)
                        {
                            N_DeductionPayID = myFunctions.getIntVAL(N_Result.ToString());
                            object deductions = dLayer.ExecuteScalar("Select X_Description from Pay_PayMaster Where N_PayID =" + N_DeductionPayID + "and N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + "", Params, connection);
                            if (deductions != null)
                                X_Deductions = deductions.ToString();
                        }
                    }
                    else
                        X_Deductions = "";


                    N_Result = dLayer.ExecuteScalar("Select N_Value from Gen_Settings Where X_Description ='Default AbsentType' and N_CompanyID= " + nCompanyID + " and X_Group='HR'", Params, connection);
                    if (N_Result != null)
                    {
                        if (myFunctions.getIntVAL(N_Result.ToString()) != 0)
                        {
                            N_DefaultAbsentID = myFunctions.getIntVAL(N_Result.ToString());
                            object AbsentCode = dLayer.ExecuteScalar("Select X_VacType from Pay_VacationType Where N_VacTypeID =" + N_DefaultAbsentID + "and N_CompanyID= " + nCompanyID, Params, connection);
                            if (AbsentCode != null)
                                X_DefaultAbsentCode = AbsentCode.ToString();

                        }
                    }
                    else
                        X_DefaultAbsentCode = "";

                    bool B_MonthlyaddordedProcess = Convert.ToBoolean(dLayer.ExecuteScalar("Select N_Value from Gen_Settings Where X_Description ='Salary Process' and N_CompanyID= " + nCompanyID + " and X_Group='HR'", Params, connection));
                    bool B_ManualEntry_InGrid = Convert.ToBoolean(dLayer.ExecuteScalar("Select N_Value from Gen_Settings Where X_Description ='ManualEntryInGrid' and N_CompanyID= " + nCompanyID + " and X_Group='HR'", Params, connection));
                    bool B_DoubleEntry = Convert.ToBoolean(dLayer.ExecuteScalar("Select N_Value from Gen_Settings Where X_Description ='DoubleShiftEntry' and N_CompanyID= " + nCompanyID + " and X_Group='HR'", Params, connection));

                    string ElementSql = "";
                    
                    object obj = dLayer.ExecuteScalar("Select isnull(Count(X_BatchCode),0) from Pay_TimeSheetMaster where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_EmpID=" + nEmpID + " and N_BatchID=" + payRunID + " and ISNULL(N_TotalWorkingDays,0)>0", Params, connection);
                    if (obj != null)
                    {
                        
                        if (myFunctions.getIntVAL(obj.ToString()) > 0 )
                        {
                            double additionTime = 0, deductionTime = 0, CompsateDed = 0, OfficeHours = 0, AbsentCount = 0;
                            double N_additionTime = 0, N_deductionTime = 0, N_CompsateDed = 0, N_OfficeHours = 0, N_ExtraHours = 0;
                            double balanc = 0, N_NetDeduction = 0;
                            double N_NetAddApp=0,N_NetDedApp=0;

                            N_BatchID = myFunctions.getIntVAL(payRunID);
                            if (N_BatchID > 0 && nEmpID > 0)
                            {
                                string Sql6 = "Select * from vw_TimeSheetMaster_Disp where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_BatchID=" + N_BatchID.ToString() + " and N_EmpID=" + nEmpID.ToString() + "";
                                TimeSheetMaster = dLayer.ExecuteDataTable(Sql6, Params, connection);

                                 //checksettings---------------------------------------------------------------------------------------
                                string Sql1 = "Select B_Addition,B_Deduction,B_Compensation from Pay_EmployeeGroup where N_CompanyID=" + nCompanyID + " and N_PkeyId=" + nCategoryID;
                                settingsTable = dLayer.ExecuteDataTable(Sql1, Params, connection);
                                settingsTable.AcceptChanges();

                                if (settingsTable.Rows.Count == 0)
                                {
                                    bCategoryWiseAddition = true;
                                    bCategoryWiseDeduction = true;
                                    bCategoryWiseComp = false;
                                }
                                else
                                {
                                    bCategoryWiseAddition = myFunctions.getBoolVAL(settingsTable.Rows[0]["B_Addition"].ToString());
                                    bCategoryWiseDeduction = myFunctions.getBoolVAL(settingsTable.Rows[0]["B_Deduction"].ToString());
                                    bCategoryWiseComp = myFunctions.getBoolVAL(settingsTable.Rows[0]["B_Compensation"].ToString());
                                }

                                TimeSheetMaster = myFunctions.AddNewColumnToDataTable(TimeSheetMaster, "TransDetailID", typeof(int), 0);
                                TimeSheetMaster = myFunctions.AddNewColumnToDataTable(TimeSheetMaster, "B_IsAdditionEntry", typeof(bool), false);

                                //PayRate
                                payRateParams.Add("@nCompanyID", nCompanyID);
                                payRateParams.Add("@nFnYearID", nFnYearID);
                                payRateParams.Add("@dtpFromdate", dtpFromdate);
                                payRateParams.Add("@dtpTodate", dtpTodate);
                                payRateParams.Add("@N_EmpID", nEmpID);
                                string payRateSql = "SP_Pay_SelAddOrDed_Emp " + nCompanyID + "," +payRunID + "," +nFnYearID + "," + nEmpID;
                                payRate = dLayer.ExecuteDataTable(payRateSql, Params,connection);
                              
                                //Emp Work hours
                                string sql7 = "Select * From vw_EmpGrp_Workhours Where N_CompanyID = " + nCompanyID + " and N_PkeyId = " + nCategoryID + "";
                                EmpGrpWorkhours = dLayer.ExecuteDataTable(sql7, Params, connection);
                                N_TimeSheetID = myFunctions.getIntVAL(TimeSheetMaster.Rows[0]["N_TimeSheetID"].ToString());

                                if (myFunctions.getIntVAL(TimeSheetMaster.Rows[0]["N_UserID"].ToString()) > 0)
                                {
                                    object obj8 = dLayer.ExecuteScalar("select D_EntryDate from Pay_MonthlyAddOrDedDetails where N_TransID=" + myFunctions.getIntVAL(TimeSheetMaster.Rows[0]["N_AddorDedID"].ToString()) + " and N_EmpID=" + nEmpID.ToString() + " and N_refID=" + N_TimeSheetID + " and N_FormID=" + 216, Params, connection);
                                    object res = dLayer.ExecuteScalar("Select X_UserName from Sec_User where N_UserID=" + myFunctions.getIntVAL(TimeSheetMaster.Rows[0]["N_UserID"].ToString()) + " and N_CompanyID=" + nCompanyID, Params, connection);

                                    if (res != null)
                                    {
                                        //bool B_IsAdditionEntry = false;
                                        int N_AddorDedID = myFunctions.getIntVAL(TimeSheetMaster.Rows[0]["N_AddorDedID"].ToString());
                                        if (N_AddorDedID > 0)
                                        {
                                            object TransDetailID = dLayer.ExecuteScalar("SELECT isnull(Count(Pay_MonthlyAddOrDedDetails.N_TransDetailsID),0) from Pay_MonthlyAddOrDedDetails where N_TransID=" + N_AddorDedID + " and N_EmpID=" + nEmpID.ToString() + " and N_refID=" + N_TimeSheetID + " and N_FormID=" + 216, Params, connection);
                                            if (myFunctions.getIntVAL(TransDetailID.ToString()) > 0)
                                            {
                                                TimeSheetMaster.Rows[0]["B_IsAdditionEntry"] = true;
                                            }
                                        }
                                        else if (N_AddorDedID == 0)
                                            TimeSheetMaster.Rows[0]["B_IsAdditionEntry"] = true;
                                    }
                                }
                                TimeSheetMaster.AcceptChanges();
                                TimeSheetMaster = _api.Format(TimeSheetMaster);

                                string Sql7 = "Select * from vw_EmpTimeSheetBatch  Where N_CompanyID=" + nCompanyID + " and N_BatchID=" + N_BatchID + " and N_EmpID=" + nEmpID.ToString() + " and N_FnYearID=" + nFnYearID + "  Order By D_Date ASC";
                                TimeSheetDetails = dLayer.ExecuteDataTable(Sql7, Params, connection);
                                TimeSheetDetails = myFunctions.AddNewColumnToDataTable(TimeSheetDetails, "N_Vacation", typeof(int), 0);
                                TimeSheetDetails = myFunctions.AddNewColumnToDataTable(TimeSheetDetails, "N_Workhours", typeof(double), null);
                                TimeSheetDetails = myFunctions.AddNewColumnToDataTable(TimeSheetDetails, "Attandance", typeof(string), null);
                                TimeSheetDetails = myFunctions.AddNewColumnToDataTable(TimeSheetDetails, "X_Type", typeof(string), null);
                                TimeSheetDetails = myFunctions.AddNewColumnToDataTable(TimeSheetDetails, "OverTime", typeof(double), null);
                                TimeSheetDetails = myFunctions.AddNewColumnToDataTable(TimeSheetDetails, "Deduction", typeof(double), null);
                                TimeSheetDetails = myFunctions.AddNewColumnToDataTable(TimeSheetDetails, "CompMinutes", typeof(double), null);
                                TimeSheetDetails = myFunctions.AddNewColumnToDataTable(TimeSheetDetails, "N_TotHours", typeof(double), null);
                                TimeSheetDetails = myFunctions.AddNewColumnToDataTable(TimeSheetDetails, "X_RemarkStatus", typeof(string), null);

                                string Sql8 = "Select * from vw_pay_OffDays Where N_CompanyID =" + nCompanyID + " and (N_FnyearID= " + nFnYearID + " or N_FnyearID=0)  ";
                                PayOffDays = dLayer.ExecuteDataTable(Sql8, secParams, connection);

                                string Sql10 = "Select * from vw_pay_WorkingHours Where N_CompanyID =" + nCompanyID;
                                PayWorkingHours = dLayer.ExecuteDataTable(Sql10, secParams, connection);

                                foreach (DataRow Avar in TimeSheetDetails.Rows)
                                {
                                    Avar["X_RemarkStatus"] = Avar["x_Remarks"];
                                    DateTime Date5 = Convert.ToDateTime(Avar["D_Date"].ToString());

                                    Avar["N_TotHours"] = Avar["N_TotalWorkHour"];

                                    foreach (DataRow Bvar in PayOffDays.Rows)
                                    {
                                        if (((int)Date5.DayOfWeek) + 1 == myFunctions.getIntVAL(Bvar["N_DayID"].ToString()) || myFunctions.getDateVAL(Date5) == myFunctions.getDateVAL(Convert.ToDateTime(Bvar["D_Date"].ToString())))
                                        {
                                            Avar["X_Remarks"] = Bvar["X_Remarks"];
                                            Avar["N_Vacation"] = 2;
                                            Avar["Attandance"] = "";
                                        }
                                    }
                                    foreach (DataRow Cvar in PayWorkingHours.Rows)
                                    {
                                        if (((int)Date5.DayOfWeek) + 1 == myFunctions.getIntVAL(Cvar["N_WHID"].ToString()))
                                        {
                                            Avar["N_Workhours"] = Cvar["N_Workhours"];
                                        }
                                    }
                                    TimeSheetDetails.AcceptChanges();

                                    if (bCategoryWiseAddition)
                                    {
                                        Avar["OverTime"] = myFunctions.getVAL(Avar["DailyOT"].ToString()).ToString("0.00");
                                    }
                                    else
                                    {
                                        Avar["OverTime"] = "0.00";
                                    }
                                    if (bCategoryWiseDeduction)
                                    {
                                        Avar["Deduction"] = myFunctions.getVAL(Avar["DailyOT"].ToString()).ToString("0.00");
                                        Avar["CompMinutes"] = myFunctions.getVAL(Avar["N_CompMinutes"].ToString()).ToString("0.00");
                                    }
                                    else
                                    {
                                        Avar["Deduction"] = "0.00";
                                        Avar["CompMinutes"] = "0.00";
                                    }

                                    if (!bCategoryWiseDeduction && N_Diffrence < 0)
                                    {
                                        N_Diffrence = HoursToMinutes(Convert.ToDouble(Avar["N_Diff"].ToString()));
                                        N_NonDedApp = HoursToMinutes(N_NonDedApp);
                                        N_NonDedApp += N_Diffrence;

                                        N_NonDedApp = MinutesToHours(N_NonDedApp);
                                    }

                                    object objPayID = dLayer.ExecuteScalar("Select X_Description from PAy_PayMaster where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_PayID=" + myFunctions.getIntVAL(Avar["N_OTPayID"].ToString()), Params, connection);
                                    if (objPayID != null)
                                        Avar["X_Type"] = objPayID.ToString();
                                    if (myFunctions.getIntVAL(Avar["N_Status"].ToString()) == 1)
                                    {
                                        Avar["Attandance"] = "P";
                                    }
                                    else if (myFunctions.getIntVAL(Avar["N_Status"].ToString()) == 2)
                                    {
                                        Avar["N_Vacation"] = 2;
                                        Avar["Attandance"] = "A";
                                    }

                                    N_OfficeHours = myFunctions.getVAL(Avar["N_DutyHours"].ToString());///////////////////////////////////
                                    if(myFunctions.getVAL(Avar["DailyOT"].ToString())>0)
                                    {
                                        N_additionTime = myFunctions.getVAL(Avar["DailyOT"].ToString());
                                        N_deductionTime = 0;
                                    }
                                    else if(myFunctions.getVAL(Avar["DailyOT"].ToString())<0)
                                    {
                                        N_additionTime = 0;
                                        N_deductionTime = myFunctions.getVAL(Avar["DailyOT"].ToString());
                                    }
                                    else
                                    {
                                        N_additionTime = 0;
                                        N_deductionTime = 0;
                                    }
                                    
                                    N_CompsateDed = myFunctions.getVAL(Avar["N_Compensate"].ToString());
                                    if (N_additionTime > 0)
                                        additionTime += HoursToMinutes(N_additionTime);
                                    if (N_deductionTime > 0)
                                        deductionTime += HoursToMinutes(N_deductionTime);
                                    if (N_CompsateDed != 0)
                                        CompsateDed += HoursToMinutes(N_CompsateDed);
                                    if (N_OfficeHours != 0)
                                        OfficeHours += HoursToMinutes(N_OfficeHours);
                                    if (Avar["Attandance"].ToString() == "A")
                                        AbsentCount++;
                                            

                                    if(myFunctions.getIntVAL(Avar["N_AddorDedID"].ToString())>0)
                                    {
                                        string Sql11 = "select N_HrsOrDays from Pay_MonthlyAddOrDedDetails where N_TransID=" + myFunctions.getIntVAL(Avar["N_AddorDedID"].ToString()) + " and N_EmpID=" + nEmpID + " and N_CompanyID=" + nCompanyID + " and B_TimeSheetEntry=1 and N_refID=" + myFunctions.getIntVAL(Avar["N_TimeSheetID"].ToString())  + " and N_FormID=" + this.FormID;
                                        DataTable dtApplicable = dLayer.ExecuteDataTable(Sql11, secParams, connection);

                                        if (dtApplicable != null && dtApplicable.Rows.Count > 0)
                                        {
                                            foreach (DataRow row in dtApplicable.Rows)
                                            {
                                                if (myFunctions.getVAL(row["N_HrsOrDays"].ToString()) >= 0)
                                                    N_NetAddApp = myFunctions.getVAL(row["N_HrsOrDays"].ToString());
                                                if (myFunctions.getVAL(row["N_HrsOrDays"].ToString()) <= 0)
                                                    N_NetDedApp = (-1 * myFunctions.getVAL(row["N_HrsOrDays"].ToString()));
                                            }
                                        }
                                    }
                                }

                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_EmpID", typeof(int), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_OfficeHours", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_AdditionTime", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_DeductionTime", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_CompsateDed", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_NetDeduction", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_ExtraHours", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_AbsentCount", typeof(int), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_NonDepApp", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_WorkdHrs", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_WorkHours", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_NonCompMin", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_CompDed", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_Balance", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_Adjustment", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_NetDedApp", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_NetAddition", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_NetAddApp", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_NonDedApp", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_TotalDays", typeof(double), 0);
                       


                                OfficeHours = MinutesToHours(OfficeHours);
                                additionTime = MinutesToHours(additionTime);
                                deductionTime = MinutesToHours(deductionTime);
                                CompsateDed = MinutesToHours(CompsateDed);
                                N_NetDeduction = MinutesToHours(N_NetDeduction);
                                N_ExtraHours = MinutesToHours(N_ExtraHours);
                                N_WorkdHrs = MinutesToHours(N_WorkdHrs);
                                N_WorkHours = MinutesToHours(N_WorkHours);

                                double N_NonCompMin = (-1 * CompsateDed);
                                double N_CompDed = (-1 * CompsateDed);
                                double N_Balance = additionTime + deductionTime;
                                double Adjustment = 0, Balance = 0;
                                if ((N_Balance % 60) < 0)
                                {
                                    Balance = ((int)N_Balance / 1) + (-1 * N_Balance % 1) * .60;
                                    Adjustment = ((int)N_Balance / 1) + (-1 * N_Balance % 1) * .60;
                                }
                                else
                                {
                                    Balance = ((int)N_Balance / 1) + (N_Balance % 1) * .60;
                                    Adjustment = ((int)N_Balance / 1) + (N_Balance % 1) * .60;
                                }
                                     object obj1 = dLayer.ExecuteScalar("SELECT  [dbo].[SP_TimeSheetCalc_TotalHours](" + nCompanyID + ",'" +dtpFromdate + "','" + dtpTodate + "'," + nEmpID + ")",Params,connection );
                                if (obj1 != null)
                                  N_TotalDays=myFunctions.getVAL(obj1.ToString());
                                else 
                                  N_TotalDays=N_WorkHours;


                                DataRow newRow = SummaryTable.NewRow();

                                newRow["N_EmpID"] = myFunctions.getIntVAL(nEmpID.ToString());
                                newRow["N_OfficeHours"] = myFunctions.getVAL(OfficeHours.ToString());
                                newRow["N_AdditionTime"] = myFunctions.getVAL(additionTime.ToString());
                                newRow["N_DeductionTime"] = myFunctions.getVAL(deductionTime.ToString());
                                newRow["N_CompsateDed"] = myFunctions.getVAL(CompsateDed.ToString());
                                newRow["N_NetDeduction"] = myFunctions.getVAL(N_NetDeduction.ToString());
                                newRow["N_NetDedApp"] = myFunctions.getVAL(N_NetDedApp.ToString());
                                newRow["N_NetAddition"] = myFunctions.getVAL(additionTime.ToString());
                                newRow["N_NetAddApp"] = myFunctions.getVAL(N_NetAddApp.ToString());
                                newRow["N_ExtraHours"] = myFunctions.getVAL(N_ExtraHours.ToString());
                                newRow["N_AbsentCount"] = myFunctions.getIntVAL(AbsentCount.ToString());
                                newRow["N_NonDedApp"] = myFunctions.getVAL(N_NonDedApp.ToString());
                                newRow["N_WorkdHrs"] = myFunctions.getVAL(N_WorkdHrs.ToString());
                                newRow["N_WorkHours"] = myFunctions.getVAL(N_WorkHours.ToString());
                                newRow["N_NonCompMin"] = myFunctions.getVAL(N_NonCompMin.ToString());
                                newRow["N_CompDed"] = myFunctions.getVAL(N_CompDed.ToString());
                                newRow["N_Balance"] = myFunctions.getVAL(Balance.ToString());
                                newRow["N_Adjustment"] = myFunctions.getVAL(Adjustment.ToString());
                                newRow["N_TotalDays"] = myFunctions.getVAL(N_TotalDays.ToString());
                                SummaryTable.Rows.Add(newRow);

                                SummaryTable.AcceptChanges();

                                EmpGrpWorkhours = _api.Format(EmpGrpWorkhours, "EmpGrpWorkhours");
                                settingsTable = _api.Format(settingsTable, "settingsTable");
                                TimeSheetMaster = _api.Format(TimeSheetMaster, "TimeSheetMaster");
                                TimeSheetDetails = _api.Format(TimeSheetDetails, "TimeSheetDetails");
                                PayOffDays = _api.Format(PayOffDays, "PayOffDays");
                                PayWorkingHours = _api.Format(PayWorkingHours, "PayWorkingHours");
                                SummaryTable = _api.Format(SummaryTable, "SummaryTable");//Accept this line ==>Aswin
                                payRate= _api.Format(payRate, "payRate");//Accept this line ==>Aswin

                                // Master = _api.Format(Master, "Master");

                                dt.Tables.Add(EmpGrpWorkhours);
                                dt.Tables.Add(settingsTable);
                                dt.Tables.Add(TimeSheetMaster);
                                dt.Tables.Add(TimeSheetDetails);
                                dt.Tables.Add(PayOffDays);
                                dt.Tables.Add(PayWorkingHours);
                                dt.Tables.Add(SummaryTable);
                                dt.Tables.Add(payRate);

                            }
                        }
                        else//New Entry
                        {
                            if (nEmpID > 0)
                            {
                                double additionTime = 0, deductionTime = 0, CompsateDed = 0, OfficeHours = 0, AbsentCount = 0;
                                double N_additionTime = 0, N_deductionTime = 0, N_CompsateDed = 0, N_OfficeHours = 0, N_ExtraHours = 0;
                                double balanc = 0, N_NetDeduction = 0, N_ActualWorkHours=0 ,ActualWorkHours=0;

                                string detailSql = "Select * From vw_EmpGrp_Workhours Where N_CompanyID = " + nCompanyID + " and N_PkeyId = " + nCategoryID + "";
                                EmpGrpWorkhours = dLayer.ExecuteDataTable(detailSql, Params, connection);
                                //if (EmpGrpWorkhours.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                                //(Row 0 check ["B_Compensation"] == false then extrahrs and noncompnsrs visible false )


                                //checksettings---------------------------------------------------------------------------------------
                                string Sql9 = "Select B_Addition,B_Deduction,B_Compensation from Pay_EmployeeGroup where N_CompanyID=" + nCompanyID + " and N_PkeyId=" + nCategoryID;
                                settingsTable = dLayer.ExecuteDataTable(Sql9, Params, connection);
                                settingsTable.AcceptChanges();

                                if (settingsTable.Rows.Count == 0)
                                {
                                    bCategoryWiseAddition = true;
                                    bCategoryWiseDeduction = true;
                                    bCategoryWiseComp = false;
                                }
                                else
                                {
                                    bCategoryWiseAddition = myFunctions.getBoolVAL(settingsTable.Rows[0]["B_Addition"].ToString());
                                    bCategoryWiseDeduction = myFunctions.getBoolVAL(settingsTable.Rows[0]["B_Deduction"].ToString());
                                    bCategoryWiseComp = myFunctions.getBoolVAL(settingsTable.Rows[0]["B_Compensation"].ToString());
                                }

                                // settingsTable = _api.Format(settingsTable);
                                // dt.Tables.Add(settingsTable);



                                // true or false 3 field check erp code ===>CategorywiseSettings for front end validation
                                //-----------------------------------------------------------------------------------------------------

                                secParams.Add("@nCompanyID", nCompanyID);
                                secParams.Add("@nFnYearID", nFnYearID);
                                secParams.Add("@dtpFromdate", dtpFromdate);
                                secParams.Add("@dtpTodate", dtpTodate);
                                secParams.Add("@N_EmpID", nEmpID);

                                DateTime D_HireDate = Convert.ToDateTime(dLayer.ExecuteScalar("select D_HireDate from Pay_Employee where N_CompanyID="+nCompanyID+" and N_EmpID="+nEmpID+" and N_FnYearID="+nFnYearID, secParams, connection).ToString());
                                DateTime D_ResignDate = Convert.ToDateTime(dLayer.ExecuteScalar("select isnull(D_StatusDate,'"+dtpTodate+"') from Pay_Employee where N_CompanyID="+nCompanyID+" and N_EmpID="+nEmpID+" and N_FnYearID="+nFnYearID, secParams, connection).ToString());

                                if(D_HireDate>dtpFromdate)
                                    secParams["@dtpFromdate"]=D_HireDate;

                                if(D_ResignDate<dtpTodate)
                                    secParams["@dtpTodate"]=D_ResignDate;

                                string payAttendanceSql = "SP_Pay_TimeSheet @nCompanyID,@nFnYearID,@dtpFromdate,@dtpTodate,@N_EmpID";
                                PayAttendence = dLayer.ExecuteDataTable(payAttendanceSql, secParams, connection);

                                //PayRate
                                payRateParams.Add("@nCompanyID", nCompanyID);
                                payRateParams.Add("@nFnYearID", nFnYearID);
                                payRateParams.Add("@dtpFromdate", dtpFromdate);
                                payRateParams.Add("@dtpTodate", dtpTodate);
                                payRateParams.Add("@N_EmpID", nEmpID);
                                string payRateSql = "SP_Pay_SelAddOrDed_Emp " + nCompanyID + "," +payRunID + "," +nFnYearID + "," + nEmpID;
                                payRate = dLayer.ExecuteDataTable(payRateSql, Params,connection);

                                if(payRate.Rows.Count==0){
                                     payRateSql = "select * from Pay_PayMaster where N_CompanyID="+nCompanyID+" and N_FnYearID="+nFnYearID;
                                payRate = dLayer.ExecuteDataTable(payRateSql, Params,connection);
                                }

                                PayAttendence = myFunctions.AddNewColumnToDataTable(PayAttendence, "N_Vacation", typeof(int), 0);
                                // PayAttendence = myFunctions.AddNewColumnToDataTable(PayAttendence, "N_Workhours", typeof(double), null);
                                PayAttendence = myFunctions.AddNewColumnToDataTable(PayAttendence, "Attandance", typeof(string), null);
                                PayAttendence = myFunctions.AddNewColumnToDataTable(PayAttendence, "X_Type", typeof(string), null);
                                PayAttendence = myFunctions.AddNewColumnToDataTable(PayAttendence, "N_PayID", typeof(int), 0);

                                //GetOffDays-------------------------------------------------------------------------------------------

                                string Sql3 = "Select * from vw_pay_OffDays Where N_CompanyID =" + nCompanyID + " and (N_FNyearID= " + nFnYearID + " or N_FNyearID=0)  ";
                                PayOffDays = dLayer.ExecuteDataTable(Sql3, secParams, connection);

                                //-----------------------------------------------------------------------------------------------------
                                string Sql4 = "Select * from vw_pay_WorkingHours Where N_CompanyID =" + nCompanyID +" and N_CatagoryID="+nCategoryID;
                                PayWorkingHours = dLayer.ExecuteDataTable(Sql4, secParams, connection);
                                //-------------------------------------------------------------------------------------------------------
                                DateTime Date = dtpFromdate;
                                do
                                {
                                    DataRow[] CheckDate = PayAttendence.Select("D_date = '" + Date + "'");
                                    if (CheckDate.Length == 0)
                                    {
                                        DataRow rowPA = PayAttendence.NewRow();
                                        rowPA["D_date"] = Date;
                                        rowPA["N_EmpId"] = nEmpID;

                                        float N_AddWH=0,N_ShiftWH=0,N_WH=0;
                                        int N_HolidayCount=0;
                                        object HoliCount= dLayer.ExecuteScalar("Select COUNT(1) from vw_pay_OffDays Where N_CompanyID =" + nCompanyID + " and (N_FNyearID= " + nFnYearID + " or N_FNyearID=0) and D_Date='"+Date+"'", secParams, connection);
                                        if(HoliCount != null) N_HolidayCount=myFunctions.getIntVAL(HoliCount.ToString());

                                        object additionWh;
                                        object shiftwh;
                                        object wh;
                                                                               
                                        if(HoliCount != null) N_HolidayCount=myFunctions.getIntVAL(HoliCount.ToString());
                                        if(D_HireDate<=Date && D_ResignDate>=Date && N_HolidayCount==0)
                                        {
                                            additionWh=dLayer.ExecuteScalar("select ISNULL(N_Workhours,0) from Pay_AdditionalWorkingDays where N_CompanyID="+nCompanyID+" and D_WorkingDate='"+Date+"' and N_CatagoryID="+nCategoryID, secParams, connection);
                                            if(additionWh==null){N_AddWH=0;}else{N_AddWH=myFunctions.getFloatVAL(additionWh.ToString());}
                                            if(N_AddWH==0)
                                            {
                                                shiftwh=dLayer.ExecuteScalar("select ISNULL(N_Workhours,0) from Pay_AdditionalWorkingDays where N_CompanyID="+nCompanyID+" and D_WorkingDate='"+Date+"' and N_CatagoryID="+nCategoryID, secParams, connection);
                                                if(shiftwh==null){N_ShiftWH=0;}else{N_ShiftWH=myFunctions.getFloatVAL(shiftwh.ToString());}
                                                
                                                if(N_ShiftWH==0)
                                                {
                                                    wh=dLayer.ExecuteScalar("select N_Workhours from Pay_WorkingHours where N_CompanyID="+nCompanyID+" and N_WHID="+((int)Date.DayOfWeek+ 1)+" and N_CatagoryID ="+nCategoryID, secParams, connection);
                                                   if(wh==null){N_WH=0;}else{N_WH=myFunctions.getFloatVAL(wh.ToString());}
                                                    
                                                    if(N_WH!=0)
                                                        rowPA["N_ActWorkHours"] = N_WH;
                                                }
                                                else
                                                    rowPA["N_ActWorkHours"] = N_ShiftWH;
                                            }
                                            else
                                                rowPA["N_ActWorkHours"] = N_AddWH;
                                        }
                                        if(  myFunctions.getVAL(rowPA["N_ActWorkHours"].ToString())==3.3 )
                                        {
                                            int a=0;
                                        }

                                        PayAttendence.Rows.Add(rowPA);
                                    }
                                    Date = Date.AddDays(1);
                                } while (Date <= dtpTodate);

                                PayAttendence.AcceptChanges();
                                PayAttendence = myFunctions.AddNewColumnToDataTable(PayAttendence, "X_Days", typeof(string),null);
                                PayAttendence.AcceptChanges();
                                foreach (DataRow row in PayAttendence.Rows)
                                {
                                    DateTime Date5 = Convert.ToDateTime(row["D_date"].ToString());
                                    //Default Paycodes

                                  row["X_Days"] = Date5.ToString("dddd");
                                  //myFunctions.AddNewColumnToDataTable(PayAttendence, "X_Days", typeof(string),Date5.ToString("dddd"));
                                //   PayAttendence.AcceptChanges();
                                // foreach (DataRow Var2 in PayWorkingHours.Rows)
                                // {
                                //     if (((int)Date5.DayOfWeek) + 1 == myFunctions.getIntVAL(Var2["N_WHID"].ToString()))
                                //     {
                                //         row["N_Workhours"] = Var2["N_Workhours"];
                                //     }
                                // }
                                    PayAttendence.AcceptChanges();
                                    foreach (DataRow Var1 in PayOffDays.Rows)
                                    {
                                        if (nCategoryID == myFunctions.getIntVAL(Var1["N_CategoryID"].ToString()) && ((int)Date5.DayOfWeek) + 1 == myFunctions.getIntVAL(Var1["N_DayID"].ToString()) || myFunctions.getDateVAL(Date5) == myFunctions.getDateVAL(Convert.ToDateTime(Var1["D_Date"].ToString())))
                                        {
                                            row["X_Remarks"] = Var1["X_Remarks"];
                                            row["N_Vacation"] = 2;
                                        }
                                    }
                                    PayAttendence.AcceptChanges();
                                    // foreach (DataRow Var2 in PayWorkingHours.Rows)
                                    // {
                                    //     if (((int)Date5.DayOfWeek) + 1 == myFunctions.getIntVAL(Var2["N_WHID"].ToString()))
                                    //     {
                                    //         row["N_Workhours"] = Var2["N_Workhours"];
                                    //     }
                                    // }
                                    PayAttendence.AcceptChanges();


                                    if (bCategoryWiseAddition)
                                    {
                                        row["OverTime"] = myFunctions.getVAL(row["OverTime"].ToString()).ToString("0.00");

                                    }
                                    else
                                    {
                                        row["OverTime"] = "0.00";
                                    }
                                    if (bCategoryWiseDeduction)
                                    {
                                        row["Deduction"] = myFunctions.getVAL(row["Deduction"].ToString()).ToString("0.00");
                                        row["CompMinutes"] = myFunctions.getVAL(row["CompMinutes"].ToString()).ToString("0.00");

                                    }
                                    else
                                    {
                                        row["Deduction"] = "0.00";
                                        row["CompMinutes"] = "0.00";
                                    }


                                    if (!bCategoryWiseDeduction && N_Diffrence < 0)
                                    {
                                        N_Diffrence = HoursToMinutes(Convert.ToDouble(row["N_Diff"].ToString()));
                                        N_NonDedApp = HoursToMinutes(N_NonDedApp);
                                        N_NonDedApp += N_Diffrence;

                                        N_NonDedApp = MinutesToHours(N_NonDedApp);
                                    }

                                    if (row["B_Isvacation"].ToString() == "1")
                                    {
                                        row["Attandance"] = "A";

                                    }
                                    else
                                    {
                                        if (myFunctions.getVAL(row["N_Tothours"].ToString()) > 0)
                                        {
                                            if (myFunctions.getVAL(row["N_TotHours"].ToString()) < myFunctions.getVAL(row["N_MinWorkhours"].ToString()))
                                            {
                                                if (myFunctions.getBoolVAL(row["B_IsApproved"].ToString()) == true)
                                                {

                                                    row["X_Type"] = row["X_Description"];
                                                    row["N_PayID"] = myFunctions.getIntVAL(row["N_OTPayID"].ToString());
                                                    row["Attandance"] = "P";


                                                }
                                                else
                                                {
                                                    row["X_Type"] = X_Deductions;
                                                    row["N_PayID"] = myFunctions.getIntVAL(N_DeductionPayID.ToString());
                                                    row["Attandance"] = "A";
                                                }
                                            }
                                            else if (myFunctions.getVAL(row["N_TotHours"].ToString()) > myFunctions.getVAL(row["N_MinWorkhours"].ToString()))
                                            {
                                                if (myFunctions.getBoolVAL(row["B_IsApproved"].ToString()) == true)
                                                {
                                                    row["X_Type"] = row["X_Description"];
                                                    row["N_PayID"] = myFunctions.getIntVAL(row["N_OTPayID"].ToString());
                                                    row["Attandance"] = "P";
                                                }
                                                else
                                                {
                                                    if (myFunctions.getVAL(row["OverTime"].ToString()) > 0)
                                                    {
                                                        row["X_Type"] = X_Additions;
                                                        row["N_PayID"] = myFunctions.getIntVAL(N_AdditionPayID.ToString());
                                                        row["Attandance"] = "P";


                                                    }
                                                    else if (myFunctions.getVAL(row["Deduction"].ToString()) > 0)
                                                    {
                                                        row["X_Type"] = X_Deductions;
                                                        row["N_PayID"] = myFunctions.getIntVAL(N_DeductionPayID.ToString());
                                                        row["Attandance"] = "P";
                                                    }
                                                    else
                                                    {
                                                        row["X_Type"] = "";
                                                        row["N_PayID"] = 0;
                                                        row["Attandance"] = "P";

                                                    }
                                                }
                                            }
                                            else
                                            {

                                                row["X_Type"] = "";
                                                row["N_PayID"] = 0;
                                                row["Attandance"] = "P";
                                            }
                                        }
                                    }
                                    if ((row["x_Remarks"].ToString() == "" || row["x_Remarks"].ToString() == null) && row["Attandance"].ToString() != "P")
                                    {
                                        DateTime Date3 = Convert.ToDateTime(row["D_date"].ToString());
                                        if (Date3 > Convert.ToDateTime(systemDate.ToString()))
                                            row["Attandance"] = "";
                                        else
                                        {
                                            object ShiftGroup = dLayer.ExecuteScalar("select X_GroupName from Pay_EmpShiftDetails where N_EmpID=" + nEmpID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and D_Date='" + myFunctions.getDateVAL(Date3) + "' and D_In1='00:00:00.0000000' and D_Out1='00:00:00.0000000' and D_In2='00:00:00.0000000' and D_Out2='00:00:00.0000000'", Params, connection);
                                            if (ShiftGroup != null)
                                            {
                                                row["x_Remarks"] = ShiftGroup.ToString();
                                                row["Attandance"] = "";
                                            }
                                            else
                                            {
                                                row["X_Type"] = X_DefaultAbsentCode;
                                                row["N_PayID"] = N_DefaultAbsentID;
                                                row["Attandance"] = "A";
                                            }
                                        }
                                    }

                                    foreach (DataRow Xvar in PayOffDays.Rows)
                                    {
                                        DateTime Date4 = Convert.ToDateTime(row["D_date"].ToString());

                                        if (nCategoryID == myFunctions.getIntVAL(Xvar["N_CategoryID"].ToString()) && ((int)Date4.DayOfWeek) + 1 == myFunctions.getIntVAL(Xvar["N_DayID"].ToString()) || myFunctions.getDateVAL(Date4) == myFunctions.getDateVAL(Convert.ToDateTime(Xvar["D_Date"].ToString())))
                                        {
                                            object obj5 = dLayer.ExecuteScalar("Select N_Workhours from Pay_AdditionalWorkingDays Where D_WorkingDate='" + Date4.ToString("yyyy-MM-dd") + "' and N_CatagoryID=" + nCategoryID + " and N_CompanyID=" + nCompanyID, Params, connection);
                                            if (obj != null) continue;
                                            if (myFunctions.getIntVAL(row["B_HolidayFlag"].ToString()) != 1)
                                            {
                                                row["X_Remarks"] = Xvar["X_Remarks"];
                                                row["N_Vacation"] = 2;

                                            }
                                            if (myFunctions.getVAL(row["N_TotHours"].ToString()) != 0)
                                            {
                                                double hours = myFunctions.getVAL(row["N_TotHours"].ToString());
                                                row["X_Type"] = X_Additions;
                                                row["N_PayID"] = myFunctions.getIntVAL(N_AdditionPayID.ToString());
                                                row["Attandance"] = "P";
                                            }
                                            else if (myFunctions.getIntVAL(row["B_HolidayFlag"].ToString()) != 1)

                                            {
                                                row["Attandance"] = "";
                                                row["X_Type"] = "";
                                                row["Deduction"] = "";
                                                //in out 1 & 2 should be " validate in front end
                                            }
                                        }
                                    }

                                    if (myFunctions.getDateVAL(Convert.ToDateTime(row["D_Date"].ToString())) == myFunctions.getDateVAL(Date5))
                                    {
                                      if (row["Attandance"].ToString() != "A" )
                                        {
                                            N_WorkdHrs += HoursToMinutes(myFunctions.getVAL(row["N_Tothours"].ToString()));
                                            N_WorkHours += HoursToMinutes(myFunctions.getVAL(row["N_Workhours"].ToString()));

                                        }
                                    }

                                    N_OfficeHours = myFunctions.getVAL(row["N_DutyHours"].ToString());///////////////////////////////////
                                    N_additionTime = myFunctions.getVAL(row["OverTime"].ToString());
                                    N_deductionTime = myFunctions.getVAL(row["Deduction"].ToString());
                                    N_CompsateDed = myFunctions.getVAL(row["CompMinutes"].ToString());
                                    N_ActualWorkHours = myFunctions.getVAL(row["N_ActWorkHours"].ToString());
                                   
                                    if(N_ActualWorkHours!=0)
                                    {
                                        count=count+1;

                                    }
                              
                                    
                                    if (N_additionTime > 0)
                                        additionTime += HoursToMinutes(N_additionTime);
                                    if (N_deductionTime > 0)
                                        deductionTime += HoursToMinutes(N_deductionTime);
                                    if (N_CompsateDed != 0)
                                        CompsateDed += HoursToMinutes(N_CompsateDed);
                                    if (N_OfficeHours != 0)
                                        OfficeHours += HoursToMinutes(N_OfficeHours);
                                     if (N_ActualWorkHours != 0)
                                         ActualWorkHours += HoursToMinutes(N_ActualWorkHours);
                                    if (row["Attandance"].ToString() == "A")
                                        AbsentCount++;

                                }
                      

                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_EmpID", typeof(int), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_OfficeHours", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_AdditionTime", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_DeductionTime", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_CompsateDed", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_NetDeduction", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_ExtraHours", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_AbsentCount", typeof(int), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_NonDepApp", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_WorkdHrs", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_WorkHours", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_NonCompMin", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_CompDed", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_Balance", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_Adjustment", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_NetDedApp", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_NetAddition", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_NetAddApp", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_NonDedApp", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_TotalDays", typeof(double), 0);
                                SummaryTable = myFunctions.AddNewColumnToDataTable(SummaryTable, "N_ActWorkHours", typeof(double), 0);
                       
                                   
                                if (CompsateDed < 0)
                                {
                                    double netded = -1 * CompsateDed;
                                    N_NetDeduction = deductionTime + netded;
                                    N_ExtraHours = 0;
                                }
                                else
                                {
                                    N_ExtraHours = CompsateDed;
                                    CompsateDed = 0;
                                    N_NetDeduction = deductionTime;

                                }

                                OfficeHours = MinutesToHours(OfficeHours);
                                additionTime = MinutesToHours(additionTime);
                                deductionTime = MinutesToHours(deductionTime);
                                CompsateDed = MinutesToHours(CompsateDed);
                                N_NetDeduction = MinutesToHours(N_NetDeduction);
                                N_ExtraHours = MinutesToHours(N_ExtraHours);
                                N_WorkdHrs = MinutesToHours(N_WorkdHrs);
                                N_WorkHours = MinutesToHours(N_WorkHours);
                               ActualWorkHours=MinutesToHours(ActualWorkHours);

                                double N_NonCompMin = (-1 * CompsateDed);
                                double N_CompDed = (-1 * CompsateDed);
                                double N_Balance = additionTime + deductionTime;
                                double Adjustment = 0, Balance = 0;
                                if ((N_Balance % 60) < 0)
                                {
                                    Balance = ((int)N_Balance / 1) + (-1 * N_Balance % 1) * .60;
                                    Adjustment = ((int)N_Balance / 1) + (-1 * N_Balance % 1) * .60;
                                }
                                else
                                {
                                    Balance = ((int)N_Balance / 1) + (N_Balance % 1) * .60;
                                    Adjustment = ((int)N_Balance / 1) + (N_Balance % 1) * .60;
                                }
                                object obj1 = dLayer.ExecuteScalar("SELECT  [dbo].[SP_TimeSheetCalc_TotalHours](" + nCompanyID + ",'" +dtpFromdate + "','" + dtpTodate + "'," + nEmpID + ")",Params,connection );
                                if (obj1 != null)
                                  N_TotalDays=myFunctions.getVAL(obj1.ToString());
                                else 
                                  N_TotalDays=N_WorkHours;

                                DataRow newRow = SummaryTable.NewRow();

                                newRow["N_EmpID"] = myFunctions.getIntVAL(nEmpID.ToString());
                                newRow["N_OfficeHours"] = myFunctions.getVAL(OfficeHours.ToString());
                                newRow["N_AdditionTime"] = myFunctions.getVAL(additionTime.ToString());
                                newRow["N_DeductionTime"] = myFunctions.getVAL(deductionTime.ToString());
                                newRow["N_CompsateDed"] = myFunctions.getVAL(CompsateDed.ToString());
                                newRow["N_NetDeduction"] = myFunctions.getVAL(N_NetDeduction.ToString());
                                newRow["N_NetDedApp"] = myFunctions.getVAL(N_NetDeduction.ToString());
                                newRow["N_NetAddition"] = myFunctions.getVAL(additionTime.ToString());
                                newRow["N_NetAddApp"] = myFunctions.getVAL(additionTime.ToString());
                                newRow["N_ExtraHours"] = myFunctions.getVAL(N_ExtraHours.ToString());
                                newRow["N_AbsentCount"] = myFunctions.getIntVAL(AbsentCount.ToString());
                                newRow["N_NonDedApp"] = myFunctions.getVAL(N_NonDedApp.ToString());
                                newRow["N_WorkdHrs"] = myFunctions.getVAL(N_WorkdHrs.ToString());
                                newRow["N_WorkHours"] = myFunctions.getVAL(N_WorkHours.ToString());
                                newRow["N_NonCompMin"] = myFunctions.getVAL(N_NonCompMin.ToString());
                                newRow["N_CompDed"] = myFunctions.getVAL(N_CompDed.ToString());
                                newRow["N_Balance"] = myFunctions.getVAL(Balance.ToString());
                                newRow["N_Adjustment"] = myFunctions.getVAL(Adjustment.ToString());
                                newRow["N_TotalDays"] = myFunctions.getVAL(N_TotalDays.ToString());
                                newRow["N_ActWorkHours"] = myFunctions.getVAL(ActualWorkHours.ToString());
                                SummaryTable.Rows.Add(newRow);

                                SummaryTable.AcceptChanges();

                                EmpGrpWorkhours = _api.Format(EmpGrpWorkhours, "EmpGrpWorkhours");
                                settingsTable = _api.Format(settingsTable, "settingsTable");
                                PayAttendence = _api.Format(PayAttendence, "PayAttendence");
                                PayOffDays = _api.Format(PayOffDays, "PayOffDays");
                                PayWorkingHours = _api.Format(PayWorkingHours, "PayWorkingHours");
                                SummaryTable = _api.Format(SummaryTable, "SummaryTable");//Accept this line ==>Aswin
                                payRate= _api.Format(payRate, "payRate");//Accept this line ==>Aswin

                                // Master = _api.Format(Master, "Master");

                                dt.Tables.Add(EmpGrpWorkhours);
                                dt.Tables.Add(settingsTable);
                                dt.Tables.Add(PayAttendence);
                                dt.Tables.Add(PayOffDays);
                                dt.Tables.Add(PayWorkingHours);
                                dt.Tables.Add(SummaryTable);
                                dt.Tables.Add(payRate);

                                //dt.Tables.Add(Master);

                                //return Ok(_api.Success(dt));
                            }

                        }
                    }

                    return Ok(_api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable MasterDetailTable;
                DataTable DetailTable;
                DataTable AddOrDedTable;
                DataTable AddOrDedDetailTable;
                MasterTable = ds.Tables["master"];
                MasterDetailTable = ds.Tables["masterDetails"];
                DetailTable = ds.Tables["details"];
                AddOrDedTable = ds.Tables["AddOrDed"];
                AddOrDedDetailTable = ds.Tables["AddOrDedDetails"];
                String xButtonAction="";
                bool bSavePaycode = false;
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_BranchID"].ToString());
                int N_TimeSheetApproveID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TimeSheetApproveID"].ToString());
                // int N_SProcessType = myFunctions.getIntVAL(MasterTable.Rows[0]["N_SProcessType"].ToString());
                MasterTable.Columns.Remove("N_SProcessType");
               int N_SProcessType=0;
                DateTime dSalDate = Convert.ToDateTime(MasterTable.Rows[0]["D_SalaryDate"].ToString());
                DateTime dFromDate = Convert.ToDateTime(MasterTable.Rows[0]["D_DateFrom"].ToString());
                DateTime dToDate = Convert.ToDateTime(MasterTable.Rows[0]["D_DateTo"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    SortedList QueryParams = new SortedList();

                    // if (!myFunctions.CheckActiveYearTransaction(nCompanyID, nFnYearId, dFromDate, dLayer, connection, transaction))
                    // {
                    //     transaction.Rollback();
                    //     return Ok(_api.Error(User, "Transaction date must be in the active Financial Year."));
                    // } //jan

                    // Auto Gen
                    string X_BatchCode = "";
                    var values = MasterTable.Rows[0]["X_BatchCode"].ToString();
                    if (values == "@Auto")
                    {
                        xButtonAction="Insert"; 
                        bool OK = true;
                        int NewNo = 0, loop = 1;
                        string X_TmpBatchCode = "";
                        while (OK)
                        {
                            NewNo = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(count(1),0) + " + loop + " As Count FRom Pay_TimeSheetApproveMaster Where N_CompanyID=" + nCompanyID + " And N_FnyearID = " + nFnYearId + " And N_BatchID = " + myFunctions.getIntVAL(MasterTable.Rows[0]["N_BatchID"].ToString()), connection, transaction).ToString());
                            X_TmpBatchCode = dSalDate.Year.ToString("00##") + dSalDate.Month.ToString("0#") + NewNo.ToString("0#");
                            if (myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(count(1),0) FRom Pay_TimeSheetApproveMaster Where N_CompanyID=" + nCompanyID + " And N_FnyearID = " + nFnYearId + " And X_BatchCode = '" + X_TmpBatchCode + "'", connection, transaction).ToString()) == 0)
                                OK = false;
                            loop += 1;
                        }
                        MasterTable.Rows[0]["X_BatchCode"] = X_TmpBatchCode;
                    }
                    else{
                        xButtonAction="Update"; 
                    }

                    if (N_TimeSheetApproveID > 0)
                    {



                        //take the employeeIDs of saving datas in masterDetails
                        string empIds = string.Join(",", MasterDetailTable.AsEnumerable().Select(row => row["N_EmpID"].ToString()));
                        if(empIds==""){empIds="0";}
                        dLayer.DeleteData("Pay_TimeSheet", "N_TimeSheetApproveID", N_TimeSheetApproveID, "N_CompanyID=" + nCompanyID+" and N_EmpID in ("+empIds+")", connection, transaction);
                        dLayer.DeleteData("Pay_TimeSheetMaster", "N_TimeSheetApproveID", N_TimeSheetApproveID, "N_CompanyID=" + nCompanyID + " and N_FnyearID=" + nFnYearId+" and N_EmpID in ("+empIds+")", connection, transaction);
                        dLayer.DeleteData("Pay_TimeSheetApproveMaster", "N_TimeSheetApproveID", N_TimeSheetApproveID, "N_CompanyID=" + nCompanyID + " and N_FnyearID=" + nFnYearId, connection, transaction);
                        //del add or ded

                           dLayer.ExecuteNonQuery("DELETE FROM Pay_MonthlyAddOrDedDetails where N_CompanyID="+nCompanyID+ " and  B_TimeSheetEntry=1 and N_FormID=216 and N_EmpID in ("+empIds+")", connection, transaction);
                        // dLayer.DeleteData("Pay_MonthlyAddOrDedDetails ", "N_RefID", nTimeSheetApproveID,"N_CompanyID="+nCompanyID+ " and B_TimeSheetEntry=1 and N_FormID=216", connection,transaction);
                        // dLayer.DeleteData("Pay_MonthlyAddOrDed", "N_TransID", myFunctions.getIntVAL(DedID.ToString()),"N_CompanyID="+nCompanyID+ "", connection,transaction);
                   
                   
                    }

                    int N_AddOrDedID = 0;

                    if(N_SProcessType==0)
                    {
                        object obj = dLayer.ExecuteScalar(" select N_TransID from Pay_MonthlyAddOrDed where N_CompanyID=" + nCompanyID + " and N_PayrunID=" + myFunctions.getIntVAL(MasterTable.Rows[0]["N_BatchID"].ToString())+ "", connection, transaction);
                        if (obj == null)
                            N_AddOrDedID = 0;
                        else if (myFunctions.getIntVAL(obj.ToString()) > 0)
                            N_AddOrDedID = myFunctions.getIntVAL(obj.ToString());

                        if(N_AddOrDedID==0)
                             N_AddOrDedID = dLayer.SaveData("Pay_MonthlyAddOrDed", "N_TransID", AddOrDedTable, connection, transaction);
                    }            

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and X_BatchCode='" + X_BatchCode + "' and N_FnyearID=" + nFnYearId;
                    N_TimeSheetApproveID = dLayer.SaveData("Pay_TimeSheetApproveMaster", "N_TimeSheetApproveID", DupCriteria, "", MasterTable, connection, transaction);
                    if (N_TimeSheetApproveID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                    int nTimesheetmasterID = 0;
                    for (int j = 0; j < MasterDetailTable.Rows.Count; j++)
                    {
                        MasterDetailTable.Rows[j]["N_TimeSheetApproveID"] = N_TimeSheetApproveID;
                        MasterDetailTable.Rows[j]["N_AddOrDedID"] = N_AddOrDedID;
                        MasterDetailTable.Rows[j]["X_BatchCode"] = (myFunctions.getIntVAL(MasterTable.Rows[0]["X_BatchCode"].ToString()) + j).ToString();

                        // DataTable dtRFQ = dLayer.ExecuteDataTable("select * from Pay_MonthlyAddOrDed where N_CompanyID is null",Params, connection,transaction);

                        nTimesheetmasterID = dLayer.SaveDataWithIndex("Pay_TimeSheetMaster", "N_TimeSheetID", "", "", j, MasterDetailTable, connection, transaction);
                        if (nTimesheetmasterID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to save"));
                        }

                        foreach (DataRow var in DetailTable.Rows)
                        {
                            if (MasterDetailTable.Rows[j]["N_EmpID"].ToString() != var["N_EmpID"].ToString()) continue;

                            var["N_EmpID"] = MasterDetailTable.Rows[j]["N_EmpID"];
                            var["N_TimeSheetID"] = nTimesheetmasterID;
                            var["N_TimeSheetApproveID"] = N_TimeSheetApproveID;
                        }
                        foreach (DataRow var1 in AddOrDedDetailTable.Rows)
                        {
                            if (MasterDetailTable.Rows[j]["N_EmpID"].ToString() != var1["N_EmpID"].ToString()) continue;

                            var1["N_TransID"] = N_AddOrDedID;
                            var1["N_RefID"] = N_TimeSheetApproveID;
                        }
                    }

                    int nTimesheetID = 0, nAddOrDedDetailID = 0;

                    if(N_SProcessType==0)
                    {
                        nAddOrDedDetailID = dLayer.SaveData("Pay_MonthlyAddOrDedDetails", "N_TransDetailsID", AddOrDedDetailTable, connection, transaction);
                    }   

                    nTimesheetID = dLayer.SaveData("Pay_TimeSheet", "N_SheetID", DetailTable, connection, transaction);
                    if (nTimesheetID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                    // Activity Log
                        string ipAddress = "";
                        if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                            ipAddress = Request.Headers["X-Forwarded-For"];
                        else
                            ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                            myFunctions.LogScreenActivitys(nFnYearId,N_TimeSheetApproveID,X_BatchCode,216,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);

                    transaction.Commit();
                    return Ok(_api.Success("Saved Successfully"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("vacType")]
        public ActionResult GetVacType(string xAttandance, int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "";
            int nCompanyID = myFunctions.GetCompanyID(User);
            if (xAttandance == "A")
            {
                sqlCommandText = "select X_Description as X_Type,N_VacTypeID as N_PayID,* from Pay_VacationType where N_CompanyID=" + nCompanyID + " and X_Type= 'B'";
            }
            else
            {
                sqlCommandText = "select X_Description as X_Type,* from Pay_PayMaster where N_CompanyID=" + nCompanyID + " and N_FnyearID= " + nFnYearID + " and N_paymethod <> 0 and (N_PayTypeID <>8 and N_PayTypeID <>13 and N_PayTypeID <>14)";

            }

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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetTimesheetApprovalDetails(string xBatchCode)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable ApproveMasterTable = new DataTable();
            DataTable TimesheetMasterTable = new DataTable();

            string Mastersql = "Select * from Pay_TimeSheetApproveMaster Where N_CompanyID=@p1 and X_BatchCode=@xBatchCode ";
            Params.Add("@p1", nCompanyID);
            Params.Add("@xBatchCode", xBatchCode);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    ApproveMasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);

                    if (ApproveMasterTable.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Data Found !!"));
                    }

                    ApproveMasterTable = _api.Format(ApproveMasterTable, "master");
                    dt.Tables.Add(ApproveMasterTable);

                    int N_TimeSheetApproveID = myFunctions.getIntVAL(ApproveMasterTable.Rows[0]["N_TimeSheetApproveID"].ToString());

                    string TimesheetMasterSql = "select * from Pay_TimeSheetMaster where N_CompanyID=" + nCompanyID + " and N_TimeSheetApproveID=" + N_TimeSheetApproveID;

                    TimesheetMasterTable = dLayer.ExecuteDataTable(TimesheetMasterSql, Params, connection);                  
                    TimesheetMasterTable = _api.Format(TimesheetMasterTable, "masterDetails");
                    dt.Tables.Add(TimesheetMasterTable);                 
                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nTransID,int nCompanyID,int nTimeSheetApproveID,string X_PayrunText,DateTime dFromDate,DateTime dToDate,int nBatchID,int nFnYearID)
        {
            int Results = 0;
            string criteria = "";
            try
            {
                SortedList QueryParams = new SortedList();
                // DataTable MasterTable;
                DataTable TransData = new DataTable();
                //  MasterTable = ds.Tables["master"];
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nTimeSheetApproveID", nTimeSheetApproveID);
                QueryParams.Add("@nTransID", nTransID);
                QueryParams.Add("@X_PayrunText", X_PayrunText);
                QueryParams.Add("@nBatchID", nBatchID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                // dFromDate = Convert.ToDateTime(MasterTable.Rows[0]["D_DateFrom"].ToString());
                // dToDate = Convert.ToDateTime(MasterTable.Rows[0]["D_DateTo"].ToString());
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    string xButtonAction = "Delete";
                    criteria = "select X_BatchCode from Pay_TimesheetMaster where n_TimeSheetApproveID="+nTimeSheetApproveID+" and n_CompanyID="+nCompanyID;
                    string X_BatchCode = "";
                    TransData = dLayer.ExecuteDataTable(criteria, QueryParams, connection);
                    SqlTransaction transaction = connection.BeginTransaction();
                    object Count = dLayer.ExecuteScalar("select count(1) from vw_PayProcessingDetails where n_EmpID in ("+criteria+") and X_PayrunText=@X_PayrunText", QueryParams, connection,transaction);               
                    if (myFunctions.getIntVAL(Count.ToString()) >0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Delete"));
                    }

                   object DedID = dLayer.ExecuteScalar("select Top(1) N_TransID from Pay_MonthlyAddOrDedDetails where N_CompanyID="+nCompanyID+ " and B_TimeSheetEntry=1 and N_FormID=216 and N_RefID="+nTimeSheetApproveID, QueryParams, connection,transaction);                                  
                   object CountDedID=0;
                   
                    if(DedID==null)
                        DedID=0;
                    else
                    {
                        CountDedID = dLayer.ExecuteScalar("select COUNT(1) from Pay_MonthlyAddOrDedDetails where N_CompanyID="+nCompanyID+ " and ISNULL(N_FormID,0)<>216 and N_TransID="+DedID, QueryParams, connection,transaction);               
                        if(CountDedID==null)
                            CountDedID=0;
                    }

                    //Activity Log
                        string ipAddress = "";
                        if (Request.Headers.ContainsKey("X-Forwarded-For"))
                            ipAddress = Request.Headers["X-Forwarded-For"];
                        else
                            ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                        myFunctions.LogScreenActivitys(nFnYearID, nTimeSheetApproveID, TransData.Rows[0]["X_BatchCode"].ToString(), 216, xButtonAction, ipAddress, "", User, dLayer, connection, transaction);

                    Results = dLayer.DeleteData("Pay_TimeSheetApproveMaster", "N_TimeSheetApproveID", nTimeSheetApproveID,"N_CompanyID="+nCompanyID+ "", connection,transaction);

                    if (Results > 0)
                    {
                        dLayer.DeleteData("Pay_MonthlyAddOrDedDetails ", "N_RefID", nTimeSheetApproveID,"N_CompanyID="+nCompanyID+ " and B_TimeSheetEntry=1 and N_FormID=216", connection,transaction);
                        if(myFunctions.getIntVAL(CountDedID.ToString())==0)
                            dLayer.DeleteData("Pay_MonthlyAddOrDed", "N_TransID", myFunctions.getIntVAL(DedID.ToString()),"N_CompanyID="+nCompanyID+ "", connection,transaction);
                        // dLayer.DeleteData("Pay_VacationDetails", "N_TransID", nTransID,"N_CompanyID="+nCompanyID+" and N_EmpID in("+criteria+") and N_FormID=" + this.FormID+" and d_VacDateFrom>='"+Convert.ToDateTime(dFromDate.ToString()) + "' and D_VacDateTo<='"+Convert.ToDateTime(dToDate.ToString()) + "'", connection,transaction);
                        dLayer.DeleteData("Pay_TimeSheet", "N_batchID", nBatchID,"N_CompanyID="+nCompanyID+ " and N_timeSheetApproveID="+nTimeSheetApproveID+" and N_FnYearID="+nFnYearID+ "", connection,transaction);
                        dLayer.DeleteData("Pay_TimeSheetMaster", "N_batchID", nBatchID,"N_CompanyID="+nCompanyID+ " and N_TimeSheetApproveID="+nTimeSheetApproveID+" and N_FnYearID="+nFnYearID+ "", connection,transaction);
                        

                        transaction.Commit();
                        return Ok(_api.Success("deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
    
    
        [HttpDelete("deleteEmployee")]
        public ActionResult DeleteEmployeeData(int nTransID,int nCompanyID,int nTimeSheetApproveID,string X_PayrunText,DateTime dFromDate,DateTime dToDate,int nBatchID,int nFnYearID,int n_EmpID)
        {
            int Results = 0;
            string criteria = "";
            try
            {
                SortedList QueryParams = new SortedList();
                 DataTable TransData = new DataTable();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nTimeSheetApproveID", nTimeSheetApproveID);
                QueryParams.Add("@nTransID", nTransID);
                QueryParams.Add("@X_PayrunText", X_PayrunText);
                QueryParams.Add("@nBatchID", nBatchID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                object employee;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                 connection.Open();
                 SqlTransaction transaction = connection.BeginTransaction();
                 object employeeCount= dLayer.ExecuteScalar( "select n_EmpID from Pay_TimesheetMaster where n_TimeSheetApproveID="+nTimeSheetApproveID+" and n_CompanyID="+nCompanyID,QueryParams, connection,transaction);    
                 if(employeeCount==null)
                 {
                     transaction.Rollback();
                     return Ok(_api.Success(_api.Success( "Approval Timesheet of the employee Already Deleted")));
                 }
              
                employee=dLayer.ExecuteScalar("select  X_EmpName from Pay_Employee where n_EmpID ="+n_EmpID+" and N_CompanyID="+nCompanyID+" and N_FnYearID="+nFnYearID+"", QueryParams, connection,transaction);               
                 object Count = dLayer.ExecuteScalar("select count(1) from vw_PayProcessingDetails where n_EmpID ="+n_EmpID+" and X_PayrunText=@X_PayrunText", QueryParams, connection,transaction);               
                 if (myFunctions.getIntVAL(Count.ToString()) >0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Employee Salary Parocessed"));
                    }

                    {
                        dLayer.DeleteData("Pay_MonthlyAddOrDedDetails ", "N_RefID", nTimeSheetApproveID,"N_CompanyID="+nCompanyID+ " and B_TimeSheetEntry=1 and N_FormID=216 and N_EmpID="+n_EmpID+"", connection,transaction);
                        dLayer.DeleteData("Pay_TimeSheet", "N_batchID", nBatchID,"N_CompanyID="+nCompanyID+ " and N_timeSheetApproveID="+nTimeSheetApproveID+" and N_FnYearID="+nFnYearID+ " and N_EmpID="+n_EmpID+"", connection,transaction);
                        dLayer.DeleteData("Pay_TimeSheetMaster", "N_batchID", nBatchID,"N_CompanyID="+nCompanyID+ " and N_TimeSheetApproveID="+nTimeSheetApproveID+" and N_FnYearID="+nFnYearID+ " and N_EmpID="+n_EmpID+"", connection,transaction);
                        transaction.Commit();
                        return Ok(_api.Success("Approval Timesheet of the employee "+employee+" Deleted"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
    }
}


