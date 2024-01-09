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
    [Route("timeSheetEntry")]
    [ApiController]
    public class Pay_TimeSheetEdit : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        StringBuilder message = new StringBuilder();
        public Pay_TimeSheetEdit(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 793;

        }
        [HttpGet("list")]
        public ActionResult TimeSheetDashboardList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
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
                Searchkey = "and ( X_BatchCode like '%" + xSearchkey + "%' or  X_PayrunText like '%" + xSearchkey + "%' or X_DateFrom like '%" + xSearchkey + "%' or X_DateTo like '%" + xSearchkey + "%' ) ";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_TimeSheetID desc";
            // xSortBy = " order by batch desc,D_TransDate desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ")  * from vw_TimeSheetEntry where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_TimeSheetEntry where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "and N_TimeSheetID not in (select top(" + Count + ") N_TimeSheetID from vw_TimeSheetEntry where N_CompanyID=@p1 and N_FnYearID=@p2 ) " + Searchkey;


            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText + xSortBy, Params, connection);

                    sqlCommandCount = "select count(1) as N_Count  from vw_TimeSheetEntry where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey;
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
                return BadRequest(_api.Error(User,e));
            }
        }


        [HttpGet("deptList")]
        public ActionResult DepartmentList(int nFnYearID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);

            string sqlCommandText = "Select * from Pay_Department Where  and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and isnull(B_Inactive,0)<>1";
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

        [HttpGet("employeeDetails")]
        public ActionResult GetEmpDetails(int nFnYearID, int nEmpID, string xEmployeeCode, DateTime dtpSalaryFromdate, DateTime dtpSalaryToDate, int nCatID)
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
                    DataTable ElementsTable = new DataTable();
                    DataTable ActualTable = new DataTable();
                    DataTable WaiveTable = new DataTable();
                    DataTable PayOffDays = new DataTable();
                    string ElementSql = "";
                    string ActualSql = "";
                    string waiveSql="";
                    int saveDraft= 0;

                    ElementSql = " Select N_EmpID as N_EmpId,* from vw_TimesheetImport_Disp  Where N_CompanyID=" + nCompanyID + " and  D_Date >= '" + dtpSalaryFromdate + "' and D_Date<=' " + dtpSalaryToDate + "' and N_EmpID=" + nEmpID + " order by D_Date";
                    ElementsTable = dLayer.ExecuteDataTable(ElementSql, Params, connection);
                    ElementsTable = myFunctions.AddNewColumnToDataTable(ElementsTable, "X_Day", typeof(string),null);
                   // ElementsTable = myFunctions.AddNewColumnToDataTable(ElementsTable, "X_Remarks", typeof(string),null);
                    // if (ElementsTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    ElementsTable.AcceptChanges();


                    
                                string Sql8 = "Select * from vw_pay_OffDays Where N_CompanyID =" + nCompanyID + " and (N_FnyearID= " + nFnYearID + " or N_FnyearID=0)  ";
                                PayOffDays = dLayer.ExecuteDataTable(Sql8, Params, connection);

                    DateTime Date = dtpSalaryFromdate;
                    do
                    {

                        object objDatePresent = dLayer.ExecuteScalar("Select N_EmpID from vw_TimesheetImport_Disp  Where N_CompanyID=" + nCompanyID + " and D_Date = '" + Date + "' and N_EmpID=" + nEmpID, Params, connection);
                        if (objDatePresent == null)
                        {
                            DataRow rowET = ElementsTable.NewRow();
                            rowET["D_Date"] = Date;
                            rowET["N_EmpId"] = nEmpID;


                            ElementsTable.Rows.Add(rowET);
                        }
                        Date = Date.AddDays(1);
                    } while (Date <= dtpSalaryToDate);


                    foreach (DataRow var in ElementsTable.Rows)
                    {
                         DateTime Date5 = Convert.ToDateTime(var["D_date"].ToString());
                         var["X_Day"] = Date5.ToString("dddd");
                             

                        ActualSql = "Select * from Pay_EmpShiftDetails  Where N_CompanyID=" + nCompanyID + " and N_EmpID=" + nEmpID + " and D_Date='" + var["D_Date"].ToString() + "' and N_ShiftID=(select Max(N_ShiftID) from Pay_EmpShiftDetails Where N_CompanyID=" + nCompanyID + " and N_EmpID=" + nEmpID + " and D_Date='" + var["D_Date"].ToString() + "')";
                        ActualTable = dLayer.ExecuteDataTable(ActualSql, Params, connection);
                        ActualTable.AcceptChanges();
                        if (ActualTable.Rows.Count != 0)
                        {
                            nCatID = myFunctions.getIntVAL(ActualTable.Rows[0]["N_GroupID"].ToString());

                            var["D_ActIn1"] = ActualTable.Rows[0]["D_In1"].ToString();
                            var["D_ActOut1"] = ActualTable.Rows[0]["D_Out1"].ToString();
                            var["D_ActIn2"] = ActualTable.Rows[0]["D_In2"].ToString();
                            var["D_ActOut2"] = ActualTable.Rows[0]["D_Out2"].ToString();
                        }
                        else if (nCatID > 0)
                        {
                            var["D_ActIn1"] = dLayer.ExecuteScalar("select D_In1 from Pay_WorkingHours where DATEPART(DW, '" + var["D_Date"].ToString() + "') = Pay_WorkingHours.N_WHID and Pay_WorkingHours.N_CatagoryId =" + nCatID + " and N_CompanyID=" + nCompanyID, Params, connection).ToString();
                            var["D_ActOut1"] = dLayer.ExecuteScalar("select D_Out1 from Pay_WorkingHours where DATEPART(DW, '" + var["D_Date"].ToString() + "') = Pay_WorkingHours.N_WHID and Pay_WorkingHours.N_CatagoryId =" + nCatID + " and N_CompanyID=" + nCompanyID, Params, connection).ToString();
                            var["D_ActIn2"] = dLayer.ExecuteScalar("select D_In2 from Pay_WorkingHours where DATEPART(DW, '" + var["D_Date"].ToString() + "') = Pay_WorkingHours.N_WHID and Pay_WorkingHours.N_CatagoryId =" + nCatID + " and N_CompanyID=" + nCompanyID, Params, connection).ToString();
                            var["D_ActOut2"] = dLayer.ExecuteScalar("select D_Out2 from Pay_WorkingHours where DATEPART(DW, '" + var["D_Date"].ToString() + "') = Pay_WorkingHours.N_WHID and Pay_WorkingHours.N_CatagoryId =" + nCatID + " and N_CompanyID=" + nCompanyID, Params, connection).ToString();
                        }
                        else
                        {
                            var["D_ActIn1"] = "00:00:00";
                            var["D_ActOut1"] = "00:00:00";
                            var["D_ActIn2"] = "00:00:00";
                            var["D_ActOut2"] = "00:00:00";
                        }
                        if (nCatID > 0)
                            var["N_BreakHrs"] = myFunctions.getFloatVAL(dLayer.ExecuteScalar("select N_BreakHours from Pay_WorkingHours where DATEPART(DW, '" + var["D_Date"].ToString() + "') = Pay_WorkingHours.N_WHID and Pay_WorkingHours.N_CatagoryId =" + nCatID + " and N_CompanyID=" + nCompanyID, Params, connection).ToString());

                        if ((var["D_ActIn1"].ToString() != "" || var["D_ActIn1"].ToString() != null) && (var["D_In"].ToString() == "" || var["D_In"].ToString() == null))
                            var["D_In"] = "00:00:00";
                        if ((var["D_ActOut1"].ToString() != "" || var["D_ActOut1"].ToString() != null) && (var["D_Out"].ToString() == "" || var["D_Out"].ToString() == null))
                            var["D_Out"] = "00:00:00";
                        if ((var["D_ActIn2"].ToString() != "" || var["D_ActIn2"].ToString() != null) && (var["D_Shift2_In"].ToString() == "" || var["D_Shift2_In"].ToString() == null))
                            var["D_Shift2_In"] = "00:00:00";
                        if ((var["D_ActOut2"].ToString() != "" || var["D_ActOut2"].ToString() != null) && (var["D_Shift2_Out"].ToString() == "" || var["D_Shift2_Out"].ToString() == null))
                            var["D_Shift2_Out"] = "00:00:00";



                               foreach (DataRow Var1 in PayOffDays.Rows)
                                    {
                                        if (nCatID == myFunctions.getIntVAL(Var1["N_CategoryID"].ToString()) && ((int)Date5.DayOfWeek) + 1 == myFunctions.getIntVAL(Var1["N_DayID"].ToString()) || myFunctions.getDateVAL(Date5) == myFunctions.getDateVAL(Convert.ToDateTime(Var1["D_Date"].ToString())))
                                        {
                                            var["X_Remarks"] = Var1["X_Remarks"];
                                            
                                        }
                                    }
                                    
                    waiveSql="Select * from Pay_AnytimeRequest where N_CompanyID=" + nCompanyID + " and N_EmpID=" + nEmpID;
                    WaiveTable = dLayer.ExecuteDataTable(waiveSql, Params, connection);
                    WaiveTable.AcceptChanges();
                     if (WaiveTable.Rows.Count != 0){
                        foreach (DataRow row in WaiveTable.Rows){
                            row["B_IsSaveDraft"]=saveDraft;
                            if (saveDraft==0 &&(row["D_Date"]==var["d_Date"]))
                            {
                                var["D_In"] = row["D_Shift1_In"].ToString();
                                var["D_Out"] = row["D_Shift1_Out"].ToString();
                                var["d_Shift2_In"]=row["D_Shift2_In"].ToString();
                                var["d_Shift2_Out"]=row["D_Shift2_Out"].ToString();
                            }
                        }
                     }

                     ElementsTable.AcceptChanges();
                             
                    }

                    


                    ElementsTable = _api.Format(ElementsTable);
                    dt.Tables.Add(ElementsTable);

                    return Ok(_api.Success(ElementsTable));

                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
        [HttpGet("employeeList")]
        public ActionResult GetEmpList(int nFnYearID, DateTime dtpSalaryFromdate, DateTime dtpSalaryToDate, bool b_AllBranchData, int nBranchID)
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
                    string empSql = "";


                    if (b_AllBranchData == true)
                        empSql = "select N_CompanyID,N_FnYearID,N_EmpId,X_EmpCode,X_EmpName,X_DepartMent,X_Position,N_PositionID,N_DepartmentID,N_CatagoryID AS N_CatID from vw_PayEmployee where isnull(B_ExcludeInAttendance,0)=0  and N_status<>2 and N_status<>3 and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and D_HireDate<='" + dtpSalaryToDate + "' and ISNULL(D_TerminationDate,'" + dtpSalaryFromdate + "')>='" + dtpSalaryFromdate + "' and N_EmpID NOT IN (select N_EmpID from Pay_TimesheetEntryEmp where N_CompanyID=" + nCompanyID + " and N_TimesheetID>0 and D_DateFrom<='" + dtpSalaryFromdate + "' and D_DateTo>='" + dtpSalaryToDate + "')  order by X_EmpCode";
                    else
                        empSql = "select N_CompanyID,N_FnYearID,N_EmpId,X_EmpCode,X_EmpName,X_DepartMent,X_Position,N_PositionID,N_DepartmentID,N_CatagoryID AS N_CatID from vw_PayEmployee where isnull(B_ExcludeInAttendance,0)=0 and N_status<>2 and N_status<>3 and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and D_HireDate<='" + dtpSalaryToDate + "' and N_BranchID=" + nBranchID + "  and ISNULL(D_TerminationDate,'" + dtpSalaryFromdate + "')>='" + dtpSalaryFromdate + "' and N_EmpID NOT IN (select N_EmpID from Pay_TimesheetEntryEmp where N_CompanyID=" + nCompanyID + " and N_TimesheetID>0 and D_DateFrom<='" + dtpSalaryFromdate + "' and D_DateTo>='" + dtpSalaryToDate + "') order by X_EmpCode";

                    EmpTable = dLayer.ExecuteDataTable(empSql, Params, connection);
                    if (EmpTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    EmpTable.AcceptChanges();
                    EmpTable = _api.Format(EmpTable, "EmpTable");
                    dt.Tables.Add(EmpTable);
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
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

                String xButtonAction="";
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
                        xButtonAction="Insert"; 
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
                            NewNo = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(count(1),0) + " + loop + " As Count FRom Pay_TimeSheetEntry Where N_CompanyID=" + nCompanyID + " And N_FnyearID = " + nFnYearId + " And N_BatchID = " + myFunctions.getIntVAL(MasterTable.Rows[0]["N_BatchID"].ToString()), connection, transaction).ToString());
                            X_TmpBatchCode = dSalDate.Year.ToString("00##") + dSalDate.Month.ToString("0#") + NewNo.ToString("0#");
                            if (myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(count(1),0) FRom Pay_TimeSheetEntry Where N_CompanyID=" + nCompanyID + " And N_FnyearID = " + nFnYearId + " And X_BatchCode = '" + X_TmpBatchCode + "'", connection, transaction).ToString()) == 0)
                                OK = false;
                            loop += 1;
                        }
                        MasterTable.Rows[0]["X_BatchCode"] = X_TmpBatchCode;
                    }
                    else{
                        xButtonAction="Update"; 
                    }

                    if (nTimesheetID > 0)
                    {
                        dLayer.DeleteData("Pay_TimesheetEntryEmp", "N_TimesheetID", nTimesheetID, "N_CompanyID=" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Pay_TimeSheetEntry", "N_TimesheetID", nTimesheetID, "N_CompanyID=" + nCompanyID + " and N_FnyearID=" + nFnYearId, connection, transaction);
                    }
                    int tempEMP=0;
                    for (int l = 0; l < DetailTable.Rows.Count; l++)
                    {
                        int EmpID=myFunctions.getIntVAL(DetailTable.Rows[l]["N_EmpID"].ToString());
                        if(EmpID==tempEMP)
                        continue;
                        dLayer.ExecuteNonQuery("delete from Pay_TimeSheetImport where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and  D_Date >= '" + myFunctions.getDateVAL(dFromDate) + "' and D_Date<=' " + myFunctions.getDateVAL(dToDate) + "' and N_EmpID=" + EmpID, connection, transaction);
                        tempEMP=EmpID;
                    }

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and X_BatchCode='" + X_BatchCode + "' and N_FnyearID=" + nFnYearId;
                    nTimesheetID = dLayer.SaveData("Pay_TimeSheetEntry", "N_TimesheetID", DupCriteria, "", MasterTable, connection, transaction);
                    if (nTimesheetID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }

                    for (int j = 0; j < EmpTable.Rows.Count; j++)
                    {
                        EmpTable.Rows[j]["N_TimesheetID"] = nTimesheetID;
                    }
                    int nTimesheetEmpID = dLayer.SaveData("Pay_TimesheetEntryEmp", "N_TimeSheetEmpID", EmpTable, connection, transaction);
                    if (nTimesheetEmpID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }

                    for (int k = 0; k < DetailTable.Rows.Count; k++)
                    {
                        DetailTable.Rows[k]["N_TimesheetID"] = nTimesheetID;
                    }
                    int nImportDetailID = dLayer.SaveData("Pay_TimeSheetImport", "N_SheetID", DetailTable, connection, transaction);
                    if (nImportDetailID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }

                     // Activity Log
                         string ipAddress = "";
                        if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                         ipAddress = Request.Headers["X-Forwarded-For"];
                         else
                       ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(nFnYearId,nTimesheetID,X_BatchCode,793,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);

                    transaction.Commit();
                    return Ok(_api.Success("Saved Successfully"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("details")]
        public ActionResult GetDetails(string xBatchCode, int nFnYearID)
        {
            DataTable Master = new DataTable();
            DataTable EmpTable = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@xBatchCode", xBatchCode);
            QueryParams.Add("@nFnYearID", nFnYearID);

            string _sqlQuery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    _sqlQuery = "Select * from vw_Pay_TimeSheetEntry where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and X_BatchCode=@xBatchCode";

                    Master = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                    Master = _api.Format(Master, "master");

                    if (Master.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        QueryParams.Add("@N_TimesheetID", Master.Rows[0]["N_TimesheetID"].ToString());

                        ds.Tables.Add(Master);

                        _sqlQuery = "select N_CompanyID,N_FnYearID,N_EmpId,X_EmpCode,X_EmpName,X_DepartMent,X_Position,N_PositionID,N_DepartmentID,N_CatagoryID AS N_CatID from vw_PayEmployee Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_EmpID in (select N_EmpID from Pay_TimesheetEntryEmp where N_CompanyID=@nCompanyID and N_TimesheetID=@N_TimesheetID)";
                        EmpTable = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        EmpTable = _api.Format(EmpTable, "EmpTable");
                        if (EmpTable.Rows.Count == 0)
                        {
                            return Ok(_api.Notice("No Results Found"));
                        }
                        ds.Tables.Add(EmpTable);

                        return Ok(_api.Success(ds));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nTimesheetID,int nCompanyID,int nFnYearId)
        {
            int Results = 0;
            string criteria = "";
            try
            {
                SortedList QueryParams = new SortedList();
                DataTable TransData = new DataTable();
                //  MasterTable = ds.Tables["master"];
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nTimesheetID", nTimesheetID);
                QueryParams.Add("@nFnYearId", nFnYearId);
              
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    string xButtonAction = "Delete";  
                    string X_BatchCode = "";                 
                    string Sql = "select X_BatchCode from Pay_TimesheetEntry where N_TimesheetID=@nTimesheetID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearId"; 
                    
                    TransData = dLayer.ExecuteDataTable(Sql, QueryParams, connection);
                    SqlTransaction transaction = connection.BeginTransaction();

                    // Activity Log
                        string ipAddress = "";
                        if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                            ipAddress = Request.Headers["X-Forwarded-For"];
                        else
                            ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                            myFunctions.LogScreenActivitys(myFunctions.getIntVAL( nFnYearId.ToString()),nTimesheetID,TransData.Rows[0]["X_BatchCode"].ToString(),793,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);

                    Results = dLayer.DeleteData("Pay_TimesheetEntryEmp", "N_TimesheetID", nTimesheetID,"N_CompanyID=" + nCompanyID, connection, transaction);
                    if (Results > 0)
                    {
                        dLayer.DeleteData("Pay_TimeSheetEntry", "N_TimesheetID", nTimesheetID, "N_CompanyID=" + nCompanyID + " and N_FnyearID=" + nFnYearId, connection, transaction);
                        transaction.Commit();
                        return Ok(_api.Success("timeSheetEntry deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Unable to delete"));
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

