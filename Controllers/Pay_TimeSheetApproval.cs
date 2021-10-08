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
        [HttpGet("employeeList")]
        public ActionResult GetEmpList(int nFnYearID, bool b_AllBranchData, int nBranchID, int nAdditionPayID, int nDeductionPayID, int nDefaultAbsentID, int payRunID)
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


                    if (b_AllBranchData == true)
                        empSql = "select N_CompanyID,N_EmpID,N_BranchID,N_Status,N_FnYearID,N_CatagoryId,X_DefEmpCode,X_Position,X_Department,N_DepartmentID,N_PositionID,Name as X_EmpName,[Employee Code] as X_EmpCode from vw_PayEmployee_Disp where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and (N_Status = 0 OR N_Status = 1)   order by X_EmpCode";
                    else
                        empSql = "select N_CompanyID,N_EmpID,N_BranchID,N_Status,N_FnYearID,N_CatagoryId,X_DefEmpCode,X_Position,X_Department,N_DepartmentID,N_PositionID,Name as X_EmpName,[Employee Code] as X_EmpCode from vw_PayEmployee_Disp where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and (N_Status = 0 OR N_Status = 1)  order by X_EmpCode";

                    EmpTable = dLayer.ExecuteDataTable(empSql, Params, connection);
                    if (EmpTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    EmpTable.AcceptChanges();


                    defaultPaycode.Clear();
                    defaultPaycode.Columns.Add("X_Addition");
                    defaultPaycode.Columns.Add("X_Deductions ");
                    defaultPaycode.Columns.Add("X_DefaultAbsentCode");
                    //defaultPaycode.Columns.Add("txtAdjustment");

                    object additions = dLayer.ExecuteScalar("Select X_Description from Pay_PayMaster Where N_PayID =" + nAdditionPayID + "and N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + "", Params, connection);
                    object deductions = dLayer.ExecuteScalar("Select X_Description from Pay_PayMaster Where N_PayID =" + nDeductionPayID + "and N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + "", Params, connection);
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
        public ActionResult GetEmpDetails(int nFnYearID, int nEmpID, int nCategoryID, string payRunID, DateTime dtpFromdate, DateTime dtpTodate, DateTime systemDate)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    SortedList secParams = new SortedList();
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



                    DataTable TimeSheetMaster = new DataTable();
                    DataTable TimeSheetDetails = new DataTable();


                    int N_AdditionPayID = 0;
                    string X_Additions = "";
                    int N_DeductionPayID = 0;
                    string X_Deductions = "";
                    int N_DefaultAbsentID = 0;
                    string X_DefaultAbsentCode = "";
                    int N_TimeSheetID = 0;

                    SortedList Master = new SortedList();


                    Double N_Diffrence = 0, N_NonDedApp = 0, txtAdjustment = 0, N_WorkdHrs = 0, N_WorkHours = 0;


                    object N_Result;

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
                    int N_BatchID = 0;
                    object obj = dLayer.ExecuteScalar("Select isnull(Count(X_BatchCode),0) from Pay_TimeSheetMaster where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_EmpID=" + nEmpID + " and N_BatchID=" + payRunID + " and ISNULL(N_TotalWorkingDays,0)>0", Params, connection);
                    if (obj != null)
                    {
                        if (myFunctions.getIntVAL(obj.ToString()) > 0)
                        {
                            N_BatchID = myFunctions.getIntVAL(payRunID);
                            if (N_BatchID > 0 && nEmpID > 0)
                            {
                                string Sql6 = "Select * from vw_TimeSheetMaster_Disp where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_BatchID=" + N_BatchID.ToString() + " and N_EmpID=" + nEmpID.ToString() + "";
                                TimeSheetMaster = dLayer.ExecuteDataTable(Sql6, Params, connection);


                                TimeSheetMaster = myFunctions.AddNewColumnToDataTable(TimeSheetMaster, "TransDetailID", typeof(int), 0);
                                TimeSheetMaster = myFunctions.AddNewColumnToDataTable(TimeSheetMaster, "B_IsAdditionEntry", typeof(bool), false);

                                string Sql1 = "Select B_Addition,B_Deduction,B_Compensation from Pay_EmployeeGroup where N_CompanyID=" + nCompanyID + " and N_PkeyId=" + nCategoryID;
                                settingsTable = dLayer.ExecuteDataTable(Sql1, Params, connection);
                                settingsTable.AcceptChanges();
                                // settingsTable = _api.Format(settingsTable);
                                // dt.Tables.Add(settingsTable);

                                string sql7 = "Select * From vw_EmpGrp_Workhours Where N_CompanyID = " + nCompanyID + " and N_PkeyId = " + nCategoryID + "";
                                EmpGrpWorkhours = dLayer.ExecuteDataTable(sql7, Params, connection);
                                N_TimeSheetID = myFunctions.getIntVAL(TimeSheetMaster.Rows[0]["N_TimeSheetID"].ToString());

                                if (myFunctions.getIntVAL(TimeSheetMaster.Rows[0]["N_UserID"].ToString()) > 0)
                                {
                                    object obj8 = dLayer.ExecuteScalar("select D_EntryDate from Pay_MonthlyAddOrDedDetails where N_TransID=" + TimeSheetMaster.Rows[0]["N_AddorDedID"].ToString() + " and N_EmpID=" + nEmpID.ToString() + " and N_refID=" + N_TimeSheetID + " and N_FormID=" + 216, Params, connection);
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
                                TimeSheetDetails = myFunctions.AddNewColumnToDataTable(TimeSheetDetails, "N_Vacation", typeof(int), 0);

                                string Sql8 = "Select * from vw_pay_OffDays Where N_CompanyID =" + nCompanyID + " and (N_FNyearID= " + nFnYearID + " or N_FNyearID=0)  ";
                                PayOffDays = dLayer.ExecuteDataTable(Sql8, secParams, connection);

                                string Sql10 = "Select * from vw_pay_WorkingHours Where N_CompanyID =" + nCompanyID;
                                PayWorkingHours = dLayer.ExecuteDataTable(Sql10, secParams, connection);

                                foreach (DataRow Avar in TimeSheetDetails.Rows)
                                {
                                    DateTime Date5 = Convert.ToDateTime(Avar["D_Date"].ToString());
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

                                }
                            }
                        }
                        else//New Entry
                        {
                            if (nEmpID > 0)
                            {
                                double additionTime = 0, deductionTime = 0, CompsateDed = 0, OfficeHours = 0,AbsentCount=0;
                                double N_additionTime = 0, N_deductionTime = 0, N_CompsateDed = 0, N_OfficeHours = 0, N_ExtraHours = 0;
                                double balanc = 0, N_NetDeduction = 0;

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

                                string payAttendanceSql = "SP_Pay_TimeSheet @nCompanyID,@nFnYearID,@dtpFromdate,@dtpTodate,@N_EmpID";
                                PayAttendence = dLayer.ExecuteDataTable(payAttendanceSql, secParams, connection);


                                PayAttendence = myFunctions.AddNewColumnToDataTable(PayAttendence, "N_Vacation", typeof(int), 0);
                                PayAttendence = myFunctions.AddNewColumnToDataTable(PayAttendence, "N_Workhours", typeof(double), null);
                                PayAttendence = myFunctions.AddNewColumnToDataTable(PayAttendence, "Attandance", typeof(string), null);
                                PayAttendence = myFunctions.AddNewColumnToDataTable(PayAttendence, "X_Type", typeof(string), null);
                                PayAttendence = myFunctions.AddNewColumnToDataTable(PayAttendence, "N_PayID", typeof(int), 0);

                                //GetOffDays-------------------------------------------------------------------------------------------

                                string Sql3 = "Select * from vw_pay_OffDays Where N_CompanyID =" + nCompanyID + " and (N_FNyearID= " + nFnYearID + " or N_FNyearID=0)  ";
                                PayOffDays = dLayer.ExecuteDataTable(Sql3, secParams, connection);

                                //-----------------------------------------------------------------------------------------------------
                                string Sql4 = "Select * from vw_pay_WorkingHours Where N_CompanyID =" + nCompanyID;
                                PayWorkingHours = dLayer.ExecuteDataTable(Sql4, secParams, connection);
                                //-------------------------------------------------------------------------------------------------------
                                DateTime Date = dtpFromdate;
                                do
                                {                                 
                                    DataRow[] CheckDate = PayAttendence.Select("D_date = '" + Date + "'");
                                    if(CheckDate.Length == 0)
                                    {
                                        DataRow rowPA = PayAttendence.NewRow();
                                        rowPA["D_date"] = Date;
                                        rowPA["N_EmpId"] = nEmpID;

                                        PayAttendence.Rows.Add(rowPA);
                                    }
                                    Date = Date.AddDays(1);
                                } while (Date <= dtpTodate);

                                PayAttendence.AcceptChanges();

                                foreach (DataRow row in PayAttendence.Rows)
                                {
                                    DateTime Date5 = Convert.ToDateTime(row["D_date"].ToString());
                                    //Default Paycodes
                                    foreach (DataRow Var1 in PayOffDays.Rows)
                                    {
                                        if (nCategoryID == myFunctions.getIntVAL(Var1["N_CategoryID"].ToString()) && ((int)Date5.DayOfWeek) + 1 == myFunctions.getIntVAL(Var1["N_DayID"].ToString()) || myFunctions.getDateVAL(Date5) == myFunctions.getDateVAL(Convert.ToDateTime(Var1["D_Date"].ToString())))
                                        {
                                            row["X_Remarks"] = Var1["X_Remarks"];
                                            row["N_Vacation"] = 2;
                                        }
                                    }
                                    PayAttendence.AcceptChanges();
                                    foreach (DataRow Var2 in PayWorkingHours.Rows)
                                    {
                                        if (((int)Date5.DayOfWeek) + 1 == myFunctions.getIntVAL(Var2["N_WHID"].ToString()))
                                        {
                                            row["N_Workhours"] = Var2["N_Workhours"];
                                        }
                                    }
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
                                        row["CompMinutes"] = myFunctions.getVAL(row["Deduction"].ToString()).ToString("0.00");

                                    }
                                    else
                                    {
                                        row["Deduction"] = "0.00";
                                        row["CompMinutes"] = "0.00";
                                    }


                                    if (!bCategoryWiseDeduction && N_Diffrence < 0)
                                    {
                                        N_Diffrence = HoursToMinutes(Convert.ToDouble(row["N_Diff"].ToString()));
                                        N_NonDedApp=HoursToMinutes(N_NonDedApp);
                                        N_NonDedApp += N_Diffrence;
                                        
                                        N_NonDedApp = MinutesToHours(N_NonDedApp);
                                    }

                                    if (row["B_Isvacation"].ToString() == "1")
                                    {
                                        row["Attandance"] = "A";

                                    }
                                    else
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
                                                if (myFunctions.getIntVAL(row["OverTime"].ToString()) > 0)
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
                                    if ((row["x_Remarks"].ToString() == "" || row["x_Remarks"].ToString() == null) && row["Attandance"].ToString() != "P")
                                    {
                                        DateTime Date3 = Convert.ToDateTime(row["D_date"].ToString());
                                        if (Date3 > Convert.ToDateTime(myFunctions.GetFormatedDate(systemDate.ToString())))
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
                                        if (row["Attandance"].ToString() != "A")
                                        {
                                            N_WorkdHrs += HoursToMinutes(myFunctions.getVAL(row["N_Tothours"].ToString()));
                                            N_WorkHours += HoursToMinutes(myFunctions.getVAL(row["N_Workhours"].ToString()));

                                        }
                                    }

                                    N_OfficeHours = myFunctions.getVAL(row["N_Workhours"].ToString());
                                    N_additionTime = myFunctions.getVAL(row["OverTime"].ToString());
                                    N_deductionTime = myFunctions.getVAL(row["Deduction"].ToString());
                                    N_CompsateDed = myFunctions.getVAL(row["CompMinutes"].ToString());
                                    if (N_additionTime > 0)
                                        additionTime += HoursToMinutes(N_additionTime);
                                    if (N_deductionTime > 0)
                                        deductionTime += HoursToMinutes(N_deductionTime);
                                    if (N_CompsateDed != 0)
                                        CompsateDed += HoursToMinutes(N_CompsateDed);
                                    if (N_OfficeHours != 0)
                                        OfficeHours += HoursToMinutes(N_OfficeHours);
                                    if(row["Attandance"].ToString() == "A")
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

                                double N_NonCompMin=(-1 * CompsateDed);                          
                                double N_CompDed=(-1 * CompsateDed);   
                                double N_Balance=additionTime + deductionTime;
                                double Adjustment=0,Balance=0;
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

                                SummaryTable.AcceptChanges();

                                EmpGrpWorkhours = _api.Format(EmpGrpWorkhours, "EmpGrpWorkhours");
                                settingsTable = _api.Format(settingsTable, "settingsTable");
                                PayAttendence = _api.Format(PayAttendence, "PayAttendence");
                                PayOffDays = _api.Format(PayOffDays, "PayOffDays");
                                PayWorkingHours = _api.Format(PayWorkingHours, "PayWorkingHours");
                                SummaryTable = _api.Format(SummaryTable, "SummaryTable");//Accept this line ==>Aswin

                                // Master = _api.Format(Master, "Master");

                                dt.Tables.Add(EmpGrpWorkhours);
                                dt.Tables.Add(settingsTable);
                                dt.Tables.Add(PayAttendence);
                                dt.Tables.Add(PayOffDays);
                                dt.Tables.Add(PayWorkingHours);
                                dt.Tables.Add(SummaryTable);

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
                DataTable DetailTable;
                DataTable EmpTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                EmpTable = ds.Tables["employees"];

                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_BranchID"].ToString());
                int nTimesheetID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TimesheetID"].ToString());
                if (MasterTable.Columns.Contains("n_EmpId"))
                {
                    int nEmpID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_EmpId"].ToString());
                }

                DateTime dSalDate = Convert.ToDateTime(MasterTable.Rows[0]["D_SalaryDate"].ToString());
                DateTime dFromDate = Convert.ToDateTime(MasterTable.Rows[0]["D_DateFrom"].ToString());
                DateTime dToDate = Convert.ToDateTime(MasterTable.Rows[0]["D_DateTo"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    SortedList QueryParams = new SortedList();

                    // Auto Gen
                    string X_BatchCode = "";
                    var values = MasterTable.Rows[0]["X_BatchCode"].ToString();
                    if (values == "@Auto")
                    {
                        // Params.Add("N_CompanyID", nCompanyID);
                        // Params.Add("N_YearID", nFnYearId);
                        // Params.Add("N_FormID", this.FormID);
                        // Params.Add("N_BranchID", nBranchID);
                        // X_BatchCode = dLayer.GetAutoNumber("Pay_TimeSheetEntry", "X_BatchCode", Params, connection, transaction);
                        // if (X_BatchCode == "") { transaction.Rollback(); return Ok(_api.Error(User,"Unable to generate timesheet entry Code")); }
                        // MasterTable.Rows[0]["X_BatchCode"] = X_BatchCode;

                        bool OK = true;
                        int NewNo = 0, loop = 1;
                        string X_TmpBatchCode = "";
                        while (OK)
                        {
                            NewNo = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(Count(*),0) + " + loop + " As Count FRom Pay_TimeSheetEntry Where N_CompanyID=" + nCompanyID + " And N_FnyearID = " + nFnYearId + " And N_BatchID = " + myFunctions.getIntVAL(MasterTable.Rows[0]["N_BatchID"].ToString()), connection, transaction).ToString());
                            X_TmpBatchCode = dSalDate.Year.ToString("00##") + dSalDate.Month.ToString("0#") + NewNo.ToString("0#");
                            if (myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(Count(*),0) FRom Pay_TimeSheetEntry Where N_CompanyID=" + nCompanyID + " And N_FnyearID = " + nFnYearId + " And X_BatchCode = '" + X_TmpBatchCode + "'", connection, transaction).ToString()) == 0)
                                OK = false;
                            loop += 1;
                        }
                        MasterTable.Rows[0]["X_BatchCode"] = X_TmpBatchCode;
                    }

                    if (nTimesheetID > 0)
                    {
                        dLayer.DeleteData("Pay_TimesheetEntryEmp", "N_TimesheetID", nTimesheetID, "N_CompanyID=" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Pay_TimeSheetEntry", "N_TimesheetID", nTimesheetID, "N_CompanyID=" + nCompanyID + " and N_FnyearID=" + nFnYearId, connection, transaction);
                    }
                    for (int l = 0; l < EmpTable.Rows.Count; l++)
                    {
                        dLayer.ExecuteNonQuery("delete from Pay_TimeSheetImport where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and  D_Date >= '" + myFunctions.getDateVAL(dFromDate) + "' and D_Date<=' " + myFunctions.getDateVAL(dToDate) + "' and N_EmpID=" + myFunctions.getIntVAL(EmpTable.Rows[l]["N_EmpID"].ToString()), connection, transaction);
                    }

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and X_BatchCode='" + X_BatchCode + "' and N_FnyearID=" + nFnYearId;
                    nTimesheetID = dLayer.SaveData("Pay_TimeSheetEntry", "N_TimesheetID", DupCriteria, "", MasterTable, connection, transaction);
                    if (nTimesheetID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                    for (int j = 0; j < EmpTable.Rows.Count; j++)
                    {
                        EmpTable.Rows[j]["N_TimesheetID"] = nTimesheetID;
                    }
                    int nTimesheetEmpID = dLayer.SaveData("Pay_TimesheetEntryEmp", "N_TimeSheetEmpID", EmpTable, connection, transaction);
                    if (nTimesheetEmpID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                    for (int k = 0; k < DetailTable.Rows.Count; k++)
                    {
                        DetailTable.Rows[k]["N_TimesheetID"] = nTimesheetID;
                    }
                    int nImportDetailID = dLayer.SaveData("Pay_TimeSheetImport", "N_SheetID", DetailTable, connection, transaction);
                    if (nImportDetailID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                    transaction.Commit();
                    return Ok(_api.Success("Saved Successfully"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

    }
}