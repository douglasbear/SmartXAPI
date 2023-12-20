// using Microsoft.AspNetCore.Mvc;
//  using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
//  using SmartxAPI.GeneralFunctions;
// using System;
//  using System.Data;
//  using System.Collections;
//  using Microsoft.Extensions.Configuration;
//  using Microsoft.Data.SqlClient;
//   using System.Security.Claims;

//  namespace SmartxAPI.Controllers
//  {
//           [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//      [Route("payEmpShiftSchedule")]
//      [ApiController]
//      public class Pay_EmpShiftSchedule : ControllerBase
//      {
//          private readonly IDataAccessLayer dLayer;
//          private readonly IApiFunctions _api;
//          private readonly IMyFunctions myFunctions;
//          private readonly IMyAttachments myAttachments;
//         private readonly string connectionString;
//         private readonly int N_FormID;
//          public Pay_EmpShiftSchedule(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
//          {
//              dLayer = dl;
//              _api = api;
//              myFunctions = myFun;
//              myAttachments = myAtt;
//              connectionString = conf.GetConnectionString("SmartxConnection");
//              N_FormID =1260 ;
//          }
//          [HttpGet("details")]
//          public ActionResult GetShiftScheduleeDetails(int nCompanyId, int nFnYearId, int nShiftDetailsID,string xDepartment, string xPosition, bool bAllBranchData, int nBranchID, DateTime dFromDate,DateTime dToDate)
//          {
//              DataTable mst = new DataTable();
//             // DataTable dt = new DataTable();
//              int nCompanyID = myFunctions.GetCompanyID(User);

//              string X_Cond = "";
//              if (xDepartment != null && xDepartment != "")
//                  X_Cond = " and X_Department = ''" + xDepartment + "''";
//              if (xPosition != null && xPosition != "")
//              {
//                  if (X_Cond == "")
//                    X_Cond = " and X_Position = ''" + xPosition + "''";
//                 else
//                      X_Cond += " and X_Position = ''" + xPosition + "''";
//              }

//              SortedList ProParams = new SortedList();
//              ProParams.Add("N_CompanyID", nCompanyID);
//              ProParams.Add("X_Cond", X_Cond);
//              ProParams.Add("N_FnYearID", nFnYearId);
//              if (bAllBranchData == false)
//                 ProParams.Add("N_BranchID", nBranchID);
//              else
//                  ProParams.Add("N_BranchID", 0);

//              try
//              {
//                   using (SqlConnection Con = new SqlConnection(connectionString))
//                  {
//                      Con.Open();
//                      mst = dLayer.ExecuteDataTablePro("vw_PayEmployee_Disp", ProParams, Con);


//                      DataSet dsEmpShiftSchedule = new DataSet();
//                      SortedList QueryParamsList = new SortedList();
//                      QueryParamsList.Add("@nCompanyID", nCompanyId);
// //                     QueryParamsList.Add("@nFnYearID", nFnYearId);
// //                     QueryParamsList.Add("@nShiftDetailsId", nShiftDetailsID);
// //                     QueryParamsList.Add("@dFromDate", dFromDate);
// //                     QueryParamsList.Add("@dToDate", dToDate);
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
    [Route("payEmpShiftSchedule")]
    [ApiController]
    public class Pay_EmpShiftSchedule : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Pay_EmpShiftSchedule(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1260;
        }
        [HttpGet("details")]
        public ActionResult Details(DateTime d_Date)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_Pay_Empshiftdetails where N_CompanyID=@p1 and D_Date=@p2";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", d_Date);
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




        [HttpGet("list")]
        public ActionResult ShiftScheduleList(int? nCompanyID, int nFnYearID, bool bAllBranchData, int nBranchID, DateTime d_Date,int nUserID, DateTime d_DateFrom,DateTime d_DateTo,int nEmpID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nFnYearID", nFnYearID);
                    Params.Add("@bAllBranchData", bAllBranchData);
                    Params.Add("@nBranchID", nBranchID);
                    Params.Add("@p2", d_Date);
                    Params.Add("@d_DateFrom", d_DateFrom);
                    Params.Add("@d_DateTo", d_DateTo);
                    Params.Add("@nEmpID", nEmpID);
                    string sqlCommandText = "";

                    bool B_ShowManagerWise = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("1260", "ShowManagerWiseEmployee", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    if (B_ShowManagerWise == false)
                    {
                        if (bAllBranchData == true)
                            sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and  (D_Date>=@d_DateFrom and D_Date<=@d_DateTo) and N_EmpID=@nEmpID union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and   isnull(D_Date,'1900-01-01')='1900-01-01' and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and  (D_Date>=@d_DateFrom and D_Date<=@d_DateTo) and  N_EmpID=@nEmpID order by D_Date ";
                        else
                            sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,x_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID  and N_EmpID=@nEmpID   and (N_BranchID=0 or N_BranchID=@nBranchID) and (D_Date>=@d_DateFrom and D_Date<=@d_DateTo or   isnull(D_Date,'1900-01-01')='1900-01-01')  ";
                    }
                    else
                    {

                        object userCategory = dLayer.ExecuteScalar("Select N_UserCategoryID From Sec_User Where N_CompanyID =@nCompanyID and N_UserID=" + nUserID + " ", Params, connection);
                        if (myFunctions.getIntVAL(userCategory.ToString()) == 2)
                        {
                            if (bAllBranchData == true)
                                sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and  (D_Date>=@d_DateFrom and D_Date<=@d_DateTo) and N_EmpID=@nEmpID union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and   isnull(D_Date,'1900-01-01')='1900-01-01' and N_EmpID not in (select N_EmpID from vw_Pay_Empshiftdetails where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and D_Date>=@d_DateFrom and D_Date<=@d_DateTo   )";
                            else
                                sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,x_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID    and (N_BranchID=0 or N_BranchID=@nBranchID) and N_EmpID=@nEmpID and (D_Date>=@d_DateFrom and D_Date<=@d_DateTo or   isnull(D_Date,'1900-01-01')='1900-01-01')  ";

                        }
                        else
                        {
                            object empID = dLayer.ExecuteScalar("Select N_EmpID From Sec_User Where N_CompanyID =@nCompanyID and N_UserID=" + nUserID + " ", Params, connection);
                            object nReportToID = dLayer.ExecuteScalar("Select N_SuperVisorID From pay_supervisor Where N_CompanyID =@nCompanyID and N_EmpID=" + myFunctions.getVAL(empID.ToString()) + " ", Params, connection);
                            object nReportingToID1 = dLayer.ExecuteScalar("Select count(N_EmpID) From Pay_Employee Where N_CompanyID =@nCompanyID and N_ReportingToID=" + myFunctions.getVAL(empID.ToString()) + " ", Params, connection);
                            int nReportingToID = myFunctions.getIntVAL(nReportingToID1.ToString());

                            if (nReportToID != null || nReportingToID > 0)
                            {
                                if (nReportToID == null)
                                {
                                    if (bAllBranchData == true)
                                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and   N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + "  AND  (D_Date>=@d_DateFrom and D_Date<=@d_DateTo) and N_EmpID=@nEmpID  union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + "  AND  isnull(D_Date,'1900-01-01')='1900-01-01' and D_Date>=@d_DateFrom and D_Date<=@d_DateTo and N_EmpID=@nEmpID order by D_Date";
                                    else
                                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and   N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + "  AND  (D_Date>=@d_DateFrom and D_Date<=@d_DateTo) and N_EmpID=@nEmpID union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + "  AND  isnull(D_Date,'1900-01-01')='1900-01-01' and D_Date>=@d_DateFrom and D_Date<=@d_DateTo and N_EmpID=@nEmpID order by D_Date";
                                }
                                else if (nReportingToID == 0)
                                {
                                    if (bAllBranchData == true)
                                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and   N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + "  AND  (D_Date>=@d_DateFrom and D_Date<=@d_DateTo) and N_EmpID=@nEmpID union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and  N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + "  AND  isnull(D_Date,'1900-01-01')='1900-01-01' and  D_Date>=@d_DateFrom and D_Date<=@pd_DateTo and N_EmpID=@nEmpID order by D_Date";
                                    else
                                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and   N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + "  AND  (D_Date>=@d_DateFrom and D_Date<=@d_DateTo) and N_EmpID=@nEmpID union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and  N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + "  AND  isnull(D_Date,'1900-01-01')='1900-01-01' D_Date>=@d_DateFrom and D_Date<=@pd_DateTo and N_EmpID=@nEmpID order by D_Date";
                                }
                                else
                                {

                                if (bAllBranchData == true)
                                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and ( N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + " OR N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + " ) AND  (D_Date>=@d_DateFrom and D_Date<=@d_DateTo) and N_EmpID=@nEmpID union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and ( N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + " OR N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + " ) AND  (D_Date>=@d_DateFrom and D_Date<=@d_DateTo)  and N_EmpID=@nEmpID order by D_Date";
                                    else
                                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and ( N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + " OR N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + " ) AND  (D_Date>=@d_DateFrom and D_Date<=@d_DateTo) and N_EmpID=@nEmpID  union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and ( N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + " OR N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + " ) AND  (D_Date>=@d_DateFrom and D_Date<=@d_DateTo)  and N_EmpID=@nEmpID ) order by D_Date";
                                        
                                     }
                                    }
                                        
                            else
                            {
                                return Ok(_api.Error(User,"No Result found"));
                            }
                        }
                    }


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                     dt = myFunctions.AddNewColumnToDataTable(dt, "X_Day", typeof(string), null);
                     dt = myFunctions.AddNewColumnToDataTable(dt, "D_CurrentIn", typeof(string), null);
                     dt = myFunctions.AddNewColumnToDataTable(dt, "D_CurrentOut", typeof(string), null);

                    DateTime Date = d_DateFrom;
                          do
                                {
                                    DataRow[] CheckDate = dt.Select("D_date = '" + Date + "'");
                                    if (CheckDate.Length == 0)
                                    {
                                        DataRow rowPA = dt.NewRow();
                                        rowPA["D_date"] = Date;
                                        rowPA["N_EmpId"] = nEmpID;
                                        

                                        DateTime Date5 = Convert.ToDateTime(Date.ToString());
                                        string day = Date5.DayOfWeek.ToString();
                                        dt.Rows.Add(rowPA);
                                    }
                                     Date = Date.AddDays(1);
                                } while (Date <= d_DateTo);
                      
                        dt.AcceptChanges();
                       foreach (DataRow row in dt.Rows){
                        DateTime Date5 = Convert.ToDateTime(row["D_date"].ToString());
                        string day = Date5.DayOfWeek.ToString();
                                    //Default Paycodes

                                  row["X_Day"] = day;
                                     object ncatID = dLayer.ExecuteScalar("Select n_CatagoryID from Pay_Employee Where N_CompanyID =" + nCompanyID + " and N_FNyearID= " + nFnYearID + "  and N_EmpID="+nEmpID+"", Params, connection);
                                        object DIn1= dLayer.ExecuteScalar("Select D_In1 from Pay_WorkingHours Where N_CompanyID =" + nCompanyID + " and n_CatagoryID= " + ncatID + " and x_day='"+day+"'", Params, connection);
                                        // object DIn2= dLayer.ExecuteScalar("Select D_In2 from Pay_WorkingHours Where N_CompanyID =" + nCompanyID + " and n_CatagoryID= " + ncatID + " and x_day='"+day+"'", Params, connection);
                                        // object Dout1= dLayer.ExecuteScalar("Select D_Out1 from Pay_WorkingHours Where N_CompanyID =" + nCompanyID + " and n_CatagoryID= " + ncatID + " and x_day='"+day+"'", Params, connection);
                                        object Dout2= dLayer.ExecuteScalar("Select D_Out2 from Pay_WorkingHours Where N_CompanyID =" + nCompanyID + " and n_CatagoryID= " + ncatID + " and x_day='"+day+"'", Params, connection);
                                         row["D_CurrentIn"] = DIn1;
                                        // rowPA["D_In2"] = DIn2;
                                        // rowPA["D_Out1"] = Dout1;
                                        row["D_CurrentOut"] = Dout2;
                        
                    }
                    dt.AcceptChanges();

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
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }


        [HttpGet("workHours")]
        public ActionResult ShiftWorkHours(int? nCategoryID, DateTime d_FromDate)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1", nCategoryID);
            DateTime day1 = Convert.ToDateTime(d_FromDate.ToString());
            string s = day1.DayOfWeek.ToString();
            string sqlCommandText = "";

            sqlCommandText = "select D_In1,D_Out1,D_In2,D_Out2 from Pay_WorkingHours where N_CatagoryID=@p1 and X_Day='" + day1.DayOfWeek.ToString() + "'";

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


        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();  
                    DataTable detailsTable = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    DataTable dt = new DataTable();
                    string Sql = "";
                    int N_CompanyID = myFunctions.getIntVAL(detailsTable.Rows[0]["n_CompanyID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(detailsTable.Rows[0]["n_FnYearID"].ToString());
                    Params.Add("@nCompanyID", N_CompanyID);
                    Params.Add("@nFnYearID", N_FnYearID);

                    var empIds = string.Join(",", detailsTable.AsEnumerable().Select(row => row["N_EmpID"].ToString()));
                    DateTime dtIn1, dtOut1, dtIn2, dtOut2;
                     DateTime dDateFrom = Convert.ToDateTime(detailsTable.Rows[0]["D_PeriodFrom"].ToString());
                     DateTime dDateTo = Convert.ToDateTime(detailsTable.Rows[0]["D_PeriodTo"].ToString());


                        DataTable d1=dLayer.ExecuteDataTable("select x_EmpName from pay_employee where n_empID in (select distinct(n_empId) from Pay_TimeSheetImport  Where d_date>='" + dDateFrom +"' and d_date<='"+dDateTo+"' and N_CompanyID=" + N_CompanyID+" and N_EmpID in ("+empIds+") "+"and ISNULL(N_TimeSheetID,0)>0" + ")",Params, connection, transaction);
                        if (d1.Rows.Count > 0){
                          var empList = d1.AsEnumerable().Select(r => r.Field<string>("x_EmpName"));
                             string value = string.Join(",", empList);
                             return Ok(_api.Notice("Time sheet entered for this employee, " + value + " in this date range "));
                        }
                       
                 else{
                 dLayer.ExecuteNonQuery("Delete from Pay_Empshiftdetails where D_PeriodFrom>='" + dDateFrom +"' and D_PeriodTo<='"+dDateTo+"' and N_EmpID in ("+empIds+")" + "  and N_CompanyID=N_CompanyID", Params, connection, transaction);

                    dLayer.SaveData("Pay_Empshiftdetails", "N_ShiftDetailsID", detailsTable, connection, transaction);
                    dLayer.ExecuteNonQuery("update pay_empshiftDetails set N_SHIFTid=N_ShiftDetailsID where D_PeriodFrom='" + dDateFrom +"' and D_PeriodTo='"+dDateTo+"' and N_EmpID in ("+empIds+")" + "  and N_CompanyID=N_CompanyID", connection, transaction);
                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                 }

                  
                }
            }

            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }


         [HttpGet("AllList")]
        public ActionResult ShiftScheduleList(int? nCompanyID, int nFnYearID, bool bAllBranchData, int nBranchID, DateTime d_Date, int nUserID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nFnYearID", nFnYearID);
                    Params.Add("@bAllBranchData", bAllBranchData);
                    Params.Add("@nBranchID", nBranchID);
                    Params.Add("@p2", d_Date);
                    string sqlCommandText = "";

                    bool B_ShowManagerWise = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("1260", "ShowManagerWiseEmployee", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    if (B_ShowManagerWise == false)
                    {
                        if (bAllBranchData == true)
                            sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and  (D_Date=@p2) union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and   isnull(D_Date,'1900-01-01')='1900-01-01' and N_EmpID not in (select N_EmpID from vw_Pay_Empshiftdetails where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and D_Date=@p2   )";
                        else
                            sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,x_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID    and (N_BranchID=0 or N_BranchID=@nBranchID) and (D_Date=@p2 or   isnull(D_Date,'1900-01-01')='1900-01-01')  ";
                    }
                    else
                    {

                        object userCategory = dLayer.ExecuteScalar("Select N_UserCategoryID From Sec_User Where N_CompanyID =@nCompanyID and N_UserID=" + nUserID + " ", Params, connection);
                        if (myFunctions.getIntVAL(userCategory.ToString()) == 2)
                        {
                            if (bAllBranchData == true)
                                sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and  (D_Date=@p2) union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and   isnull(D_Date,'1900-01-01')='1900-01-01' and N_EmpID not in (select N_EmpID from vw_Pay_Empshiftdetails where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and D_Date=@p2   )";
                            else
                                sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,x_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID    and (N_BranchID=0 or N_BranchID=@nBranchID) and (D_Date=@p2 or   isnull(D_Date,'1900-01-01')='1900-01-01')  ";

                        }
                        else
                        {
                            object empID = dLayer.ExecuteScalar("Select N_EmpID From Sec_User Where N_CompanyID =@nCompanyID and N_UserID=" + nUserID + " ", Params, connection);
                            object nReportToID = dLayer.ExecuteScalar("Select N_SuperVisorID From pay_supervisor Where N_CompanyID =@nCompanyID and N_EmpID=" + myFunctions.getVAL(empID.ToString()) + " ", Params, connection);
                            object nReportingToID1 = dLayer.ExecuteScalar("Select count(N_EmpID) From Pay_Employee Where N_CompanyID =@nCompanyID and N_ReportingToID=" + myFunctions.getVAL(empID.ToString()) + " ", Params, connection);
                            int nReportingToID = myFunctions.getIntVAL(nReportingToID1.ToString());

                            if (nReportToID != null || nReportingToID > 0)
                            {
                                if (nReportToID == null)
                                {
                                    if (bAllBranchData == true)
                                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and   N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + "  AND  (D_Date=@p2) union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + "  AND  isnull(D_Date,'1900-01-01')='1900-01-01' and N_EmpID not in (select N_EmpID from vw_Pay_Empshiftdetails where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and D_Date=@p2 AND  N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + "  )";
                                    else
                                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and   N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + "  AND  (D_Date=@p2) union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + "  AND  isnull(D_Date,'1900-01-01')='1900-01-01' and N_EmpID not in (select N_EmpID from vw_Pay_Empshiftdetails where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and D_Date=@p2 AND  N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + "  )";
                                }
                                else if (nReportingToID == 0)
                                {
                                    if (bAllBranchData == true)
                                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and   N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + "  AND  (D_Date=@p2) union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and  N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + "  AND  isnull(D_Date,'1900-01-01')='1900-01-01' and N_EmpID not in (select N_EmpID from vw_Pay_Empshiftdetails where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and D_Date=@p2 AND   N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + "  )";
                                    else
                                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and   N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + "  AND  (D_Date=@p2) union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and  N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + "  AND  isnull(D_Date,'1900-01-01')='1900-01-01' and N_EmpID not in (select N_EmpID from vw_Pay_Empshiftdetails where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and D_Date=@p2 AND   N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + "  )";
                                }
                                else
                                {

                                    if (bAllBranchData == true)
                                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and ( N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + " OR N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + " ) AND  (D_Date=@p2) union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and ( N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + " OR N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + " ) AND  isnull(D_Date,'1900-01-01')='1900-01-01' and N_EmpID not in (select N_EmpID from vw_Pay_Empshiftdetails where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and D_Date=@p2 AND (N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + " OR N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + ")  )";
                                    else
                                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and ( N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + " OR N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + " ) AND  (D_Date=@p2) union  Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],N_ReportToID,N_ReportingToID,Name,X_Position,X_Division,X_Department,X_BranchName,N_GroupID,X_GroupName,D_Date,D_PeriodFrom,D_PeriodTo,D_In1,D_Out1,D_In2,D_Out2 from vw_Pay_Empshiftdetails Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and ( N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + " OR N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + " ) AND  isnull(D_Date,'1900-01-01')='1900-01-01' and N_EmpID not in (select N_EmpID from vw_Pay_Empshiftdetails where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and D_Date=@p2 AND (N_ReportToID=" + myFunctions.getVAL(nReportToID.ToString()) + " OR N_ReportingToID =" + myFunctions.getIntVAL(empID.ToString()) + ")  )";
                                }
                            }
                            else
                            {
                                return Ok(_api.Error(User,"No Result found"));
                            }
                        }
                    }


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }




    }
}