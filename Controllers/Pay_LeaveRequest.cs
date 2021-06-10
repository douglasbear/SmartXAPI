using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Claims;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("leaverequest")]
    [ApiController]



    public class Pay_LeaveRequest : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        private readonly IMyAttachments myAttachments;

        public Pay_LeaveRequest(IDataAccessLayer dl, IApiFunctions apiFun, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 210;
        }



        [HttpGet("leaveList")]
        public ActionResult GetEmployeeLeaveRequest(int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nFnyearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();
            string sqlCommandCount = "";
            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);   
            QueryParams.Add("@nCompanyID", nCompanyID);
            QueryParams.Add("@nUserID", nUserID);
            QueryParams.Add("@nFnyearID", nFnyearID);
            string sqlCommandText = "";
            int Count = (nPage - 1) * nSizeperpage;
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_VacationGroupCode like'%" + xSearchkey + "%'or [Emp Name] like'%" + xSearchkey + "%'or X_VacType like'%" + xSearchkey + "%'+)";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by CAST(ISNULL(B_IsSaveDraft,0) as int) desc,X_VacationGroupCode desc";
            else if(xSortBy.Contains("vacationRequestDate"))
                xSortBy =" order by cast(vacationRequestDate as DateTime) " + xSortBy.Split(" ")[1];
            else
                xSortBy = " order by " + xSortBy;

            //  sqlCommandText = "Select x_VacationGroupCode,vacationRequestDate,x_VacType,min(d_VacDateFrom) as d_VacDateFrom,max(d_VacDateTo) as d_VacDateTo,x_VacRemarks,X_CurrentStatus From vw_PayVacationList where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and X_Status=@xStatus group by x_VacationGroupCode,vacationRequestDate,x_VacType,x_VacRemarks,X_CurrentStatus order by VacationRequestDate Desc";

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") x_VacationGroupCode,vacationRequestDate,x_VacType,min(d_VacDateFrom) as d_VacDateFrom,max(d_VacDateTo) as d_VacDateTo,x_VacRemarks,X_CurrentStatus,sum(abs(N_VacDays)) as N_VacDays,ISNULL(B_IsSaveDraft,0) AS B_IsSaveDraft  From vw_PayVacationList where N_CompanyID=@nCompanyID and  N_EmpID=@nEmpID and N_FnYearID=@nFnyearID and B_IsAdjustEntry<>1  " + Searchkey + "  group by x_VacationGroupCode,vacationRequestDate,x_VacType,x_VacRemarks,X_CurrentStatus,B_IsSaveDraft  " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") x_VacationGroupCode,vacationRequestDate,x_VacType,min(d_VacDateFrom) as d_VacDateFrom,max(d_VacDateTo) as d_VacDateTo,x_VacRemarks,X_CurrentStatus,sum(abs(N_VacDays)) as N_VacDays,ISNULL(B_IsSaveDraft,0) AS B_IsSaveDraft From vw_PayVacationList where N_CompanyID=@nCompanyID and N_EmpID=@nEmpID and N_FnYearID=@nFnyearID  and B_IsAdjustEntry<>1  " + Searchkey + " and N_VacationGroupID not in (select top(" + Count + ") N_VacationGroupID from vw_PayVacationList where  N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and B_IsAdjustEntry<>1   group by x_VacationGroupCode,vacationRequestDate,x_VacType,x_VacRemarks,X_CurrentStatus,B_IsSaveDraft  " + xSortBy + "  group by x_VacationGroupCode,vacationRequestDate,x_VacType,x_VacRemarks,X_CurrentStatus,B_IsSaveDraft ) " + xSortBy;

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object nEmpID = dLayer.ExecuteScalar("Select N_EmpID From Sec_User where N_UserID=@nUserID and N_CompanyID=@nCompanyID", QueryParams, connection);
                    if (nEmpID != null)
                    {
                        QueryParams.Add("@nEmpID", myFunctions.getIntVAL(nEmpID.ToString()));
                        dt = dLayer.ExecuteDataTable(sqlCommandText, QueryParams, connection);
                        sqlCommandCount = "select count(*) as N_Count From vw_PayVacationList where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnyearID and B_IsAdjustEntry<>1 " + Searchkey + " ";
                        object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, QueryParams, connection);
                        OutPut.Add("Details", api.Format(dt));
                        OutPut.Add("TotalCount", TotalCount);
                    }
                    else
                    {
                        return Ok(api.Notice("No Results Found"));
                    }


                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

         [HttpGet("leaveListAll")]
        public ActionResult GetLeaveRequestList(int nPage, int nSizeperpage,int nFnyearID, string xSearchkey, string xSortBy,bool isAdjestment)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();
            string sqlCommandCount = "";
            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);
            QueryParams.Add("@nCompanyID", nCompanyID);
            QueryParams.Add("@nUserID", nUserID);
            QueryParams.Add("@nFnyearID", nFnyearID);
            string sqlCommandText = "";
            int Count = (nPage - 1) * nSizeperpage;
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_VacationGroupCode like'%" + xSearchkey + "%'or [Emp Name] like'%"+ xSearchkey + "%'or X_VacType like'%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by CAST(ISNULL(B_IsSaveDraft,0) as int) desc,cast(X_VacationGroupCode as numeric) desc";
            else if(xSortBy.Contains("vacationRequestDate"))
                xSortBy =" order by cast(vacationRequestDate as DateTime) " + xSortBy.Split(" ")[1];
            else
                xSortBy = " order by " + xSortBy;

            string isAdjestmentCriteria="  and B_IsAdjustEntry<>1 ";

            if(isAdjestment==true){
                isAdjestmentCriteria="  and B_IsAdjustEntry=1 ";
            }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") [Emp Name],x_VacationGroupCode,vacationRequestDate,x_VacType,min(d_VacDateFrom) as d_VacDateFrom,max(d_VacDateTo) as d_VacDateTo,x_VacRemarks,X_CurrentStatus,sum(abs(N_VacDays)) as N_VacDays,ISNULL(B_IsSaveDraft,0) AS B_IsSaveDraft  From vw_PayVacationList where N_CompanyID=@nCompanyID and N_FnYearID=@nFnyearID  " + isAdjestmentCriteria + Searchkey + "  group by [Emp Name],x_VacationGroupCode,vacationRequestDate,x_VacType,x_VacRemarks,X_CurrentStatus,B_IsSaveDraft  " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") [Emp Name],x_VacationGroupCode,vacationRequestDate,x_VacType,min(d_VacDateFrom) as d_VacDateFrom,max(d_VacDateTo) as d_VacDateTo,x_VacRemarks,X_CurrentStatus,sum(abs(N_VacDays)) as N_VacDays,ISNULL(B_IsSaveDraft,0) AS B_IsSaveDraft From vw_PayVacationList where N_CompanyID=@nCompanyID and N_FnYearID=@nFnyearID " + isAdjestmentCriteria + Searchkey + " and N_VacationGroupID not in (select top(" + Count + ") N_VacationGroupID from vw_PayVacationList where  N_CompanyID=@nCompanyID "+ isAdjestmentCriteria +"   group by [Emp Name],x_VacationGroupCode,vacationRequestDate,x_VacType,x_VacRemarks,X_CurrentStatus,B_IsSaveDraft  " + xSortBy + "  group by [Emp Name],x_VacationGroupCode,vacationRequestDate,x_VacType,x_VacRemarks,X_CurrentStatus,B_IsSaveDraft ) " + xSortBy;

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                        dt = dLayer.ExecuteDataTable(sqlCommandText, QueryParams, connection);
                        sqlCommandCount = "select count(*) as N_Count From vw_PayVacationList where N_CompanyID=@nCompanyID "+ isAdjestmentCriteria  + Searchkey + " ";
                        object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, QueryParams, connection);
                        OutPut.Add("Details", api.Format(dt));
                        OutPut.Add("TotalCount", TotalCount);
                   


                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }


        [HttpGet("details")]
        public ActionResult GetEmployeeVacationDetails(string xVacationGroupCode, int nBranchID, bool bShowAllBranchData)
        {
            DataTable Master = new DataTable();
            DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@xVacationGroupCode", xVacationGroupCode);
            QueryParams.Add("@nBranchID", nBranchID);
            string Condition = "";
            string _sqlQuery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (bShowAllBranchData == true)
                        Condition = "n_Companyid=@nCompanyID and X_VacationGroupCode =@xVacationGroupCode and N_TransType=1";
                    else
                        Condition = "n_Companyid=@nCompanyID and X_VacationGroupCode =@xVacationGroupCode and N_BranchID=@nBranchID and N_TransType=1";


                    _sqlQuery = "Select * from vw_PayVacationMaster Where " + Condition + "";

                    Master = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                    Master = api.Format(Master, "master");

                    if (Master.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        QueryParams.Add("@nVacationGroupID", Master.Rows[0]["N_VacationGroupID"].ToString());
                        QueryParams.Add("@nEmpID", Master.Rows[0]["N_EmpID"].ToString());
                        
                        
                        ds.Tables.Add(Master);
                        Condition = "";
                        if (bShowAllBranchData == true)
                            Condition = "n_Companyid=@nCompanyID and N_VacationGroupID =@nVacationGroupID and N_TransType=1 and X_Type='B'";
                        else
                            Condition = "n_Companyid=@nCompanyID and N_VacationGroupID =@nVacationGroupID and N_BranchID=@nBranchID  and N_TransType=1 and X_Type='B'";

                        _sqlQuery = "Select *,dbo.Fn_CalcAvailDays(N_CompanyID,VacTypeId,@nEmpID,D_VacDateFrom,N_VacationGroupID,2) As n_AvailDays,dbo.Fn_CalcAvailDays(N_CompanyID,VacTypeId,@nEmpID,D_VacDateFrom,N_VacationGroupID,1) As n_AvailUptoDays from vw_PayVacationDetails_Disp Where " + Condition + "";
                        Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        Detail = api.Format(Detail, "details");
                        if (Detail.Rows.Count == 0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }

                        DataTable benifits = FillCodeList(companyid, myFunctions.getIntVAL(Master.Rows[0]["N_EmpID"].ToString()), myFunctions.getIntVAL(Master.Rows[0]["N_VacationGroupID"].ToString()), connection);

                        ds.Tables.Add(api.Format(benifits, "benifits"));
                        ds.Tables.Add(Detail);


                        DataTable Attachements = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(Master.Rows[0]["N_EmpID"].ToString()), myFunctions.getIntVAL(Master.Rows[0]["N_VacationGroupID"].ToString()), this.FormID, myFunctions.getIntVAL(Master.Rows[0]["N_FnYearID"].ToString()), User, connection);
                        Attachements = api.Format(Attachements, "attachments");
                        ds.Tables.Add(Attachements);

                        return Ok(api.Success(ds));
                    }


                }


            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }


        //List
        [HttpGet("vacationList")]
        public ActionResult GetVacationList(string nEmpID)
        {
            DataTable dt = new DataTable();
            SortedList QueryParams = new SortedList();
            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@nEmpID", nEmpID);
            QueryParams.Add("@today", DateTime.Today);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable("Select *,dbo.Fn_CalcAvailDays(@nCompanyID,N_VacTypeID,@nEmpID,@today,0,2) As AvlDays,dbo.Fn_CalcAvailDays(@nCompanyID,N_VacTypeID,@nEmpID,@today,0,1) As Accrude from vw_pay_Vacation_List where X_Type='B' and N_EmpId=@nEmpID and N_CompanyID=@nCompanyID", QueryParams, connection);

                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                    return Ok(api.Notice("No Results Found"));
                else
                    return Ok(api.Success(dt));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpGet("getAvailable_Old")]
        public ActionResult GetAvailableDays_Old(int nVacTypeID, DateTime dDateFrom, double nAccrued, int nEmpID, int nVacationGroupID)
        {
            DateTime toDate;
            int days = 0;
            double totalDays = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    double AvlDays = Convert.ToDouble(CalculateGridAnnualDays(nVacTypeID, nEmpID, nCompanyID, nVacationGroupID, connection));

                    SortedList paramList = new SortedList();
                    paramList.Add("@nCompanyID", nCompanyID);
                    paramList.Add("@nEmpID", nEmpID);
                    paramList.Add("@nVacTypeID", nVacTypeID);
                    paramList.Add("@nVacationGroupID", nVacationGroupID);

                    toDate = Convert.ToDateTime(dLayer.ExecuteScalar("Select isnull(Max(D_VacDateTo),getdate()) from Pay_VacationDetails Where N_CompanyID =@nCompanyID and  N_EmpID  =@nEmpID and N_VacTypeID =@nVacTypeID and N_VacationStatus = 0 and N_VacDays>0 ", paramList, connection).ToString());
                    if (toDate < dDateFrom)
                    {
                        string daySql = "select  DATEDIFF(day,'" + toDate.ToString("yyyy-MM-dd") + "','" + dDateFrom.ToString("yyyy-MM-dd") + "')";
                        days = Convert.ToInt32(dLayer.ExecuteScalar(daySql, connection).ToString());
                    }
                    else
                    {
                        days = 0;
                    }
                    if (nVacTypeID == 6)
                    {
                        totalDays = Math.Round(AvlDays + ((days / 30.458) * nAccrued), 0);
                    }
                    else
                        totalDays = Math.Round(AvlDays + ((days / 30.458)), 0);
                }
                Dictionary<string, string> res = new Dictionary<string, string>();
                res.Add("availableDays", totalDays.ToString());

                return Ok(api.Success(res));
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }


        [HttpGet("getAvailable_Old2")]
        public ActionResult GetAvailableDays_2(int nVacTypeID, DateTime dDateFrom, double nAccrued, int nEmpID, int nVacationGroupID)
        {
            DateTime toDate;
            int days = 0;
            double totalDays = 0;
            object Date = null;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    double AvlDays = Convert.ToDouble(CalculateGridAnnualDays(nVacTypeID, nEmpID, nCompanyID, nVacationGroupID, connection));

                    Date = dLayer.ExecuteScalar("Select MAX(Pay_VacationDetails.D_VacDateTo) as D_Date from Pay_VacationDetails Where N_CompanyID =" + nCompanyID + " and  N_EmpID  =" + nEmpID.ToString() + " and N_VacTypeID =" + nVacTypeID + " and N_VacationStatus = 0 and N_VacDays>0 ", connection);
                    if (Date.ToString().Trim() != "")
                    {
                        if (Date.ToString() != "")
                            toDate = Convert.ToDateTime(Date.ToString());
                        else
                            toDate = Convert.ToDateTime(dLayer.ExecuteScalar("Select D_HireDate from Pay_Employee Where N_CompanyID =" + nCompanyID + " and  N_EmpID  =" + nEmpID.ToString() + "", connection).ToString());

                        if (toDate < dDateFrom)
                            days = myFunctions.getIntVAL(dLayer.ExecuteScalar("select  DATEDIFF(day,'" + myFunctions.getDateVAL(toDate) + "','" + myFunctions.getDateVAL(dDateFrom) + "')", connection).ToString());
                        else
                            days = 0;
                    }
                    else
                    {
                        toDate = Convert.ToDateTime(dLayer.ExecuteScalar("Select D_HireDate from Pay_Employee Where N_CompanyID =" + nCompanyID + " and  N_EmpID  =" + nEmpID.ToString() + "", connection).ToString());
                        if (toDate < dDateFrom)
                            days = myFunctions.getIntVAL(dLayer.ExecuteScalar("select  DATEDIFF(day,'" + myFunctions.getDateVAL(toDate) + "','" + myFunctions.getDateVAL(dDateFrom) + "')", connection).ToString());
                        else
                            days = 0;
                    }
                    if (nVacTypeID == 6)
                    {
                        totalDays = Math.Round(AvlDays + ((days / 30.458) * nAccrued), 0);
                    }
                    else
                        totalDays = Math.Round(AvlDays + ((days / 30.458)), 0);

                    double N_MaxDays = 0;
                    object MaxDays = dLayer.ExecuteScalar("select N_MaxAccrued from Pay_VacationType where N_VacTypeID=" + nVacTypeID + " and N_CompanyID=" + nCompanyID, connection);
                    if (MaxDays != null)
                        N_MaxDays = myFunctions.getVAL(MaxDays.ToString());
                    double N_Acrued = 0;
                    object Acrued = dLayer.ExecuteScalar("select sum(N_VacDays) from  Pay_VacationDetails INNER JOIN  Pay_VacationType ON Pay_VacationDetails.N_VacTypeID = Pay_VacationType.N_VacTypeID where Pay_VacationDetails.N_EmpID =" + nEmpID + " and Pay_VacationType.N_VacTypeID= " + nVacTypeID + " and Pay_VacationDetails.N_CompanyID=" + nCompanyID, connection);

                    if (Acrued != null)
                        N_Acrued = myFunctions.getVAL(Acrued.ToString());


                    if (totalDays > myFunctions.getVAL(MaxDays.ToString()))
                    {
                        totalDays = myFunctions.getVAL(MaxDays.ToString());
                    }

                }
                Dictionary<string, string> res = new Dictionary<string, string>();
                res.Add("availableDays", totalDays.ToString());

                return Ok(api.Success(res));
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpGet("getAvailable")]
        public ActionResult GetAvailableDays(int nVacTypeID, DateTime dDateFrom,int nEmpID, int nVacationGroupID)
        {
            DataTable dt = new DataTable();
            SortedList output = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            SortedList QueryParams = new SortedList();

            QueryParams.Add("@nCompanyID", nCompanyID);
            QueryParams.Add("@nEmpID", nEmpID);
            QueryParams.Add("@nVacationGroupID", nVacationGroupID);
            QueryParams.Add("@today", dDateFrom);
            QueryParams.Add("@nVacTypeID", nVacTypeID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable("Select dbo.Fn_CalcAvailDays(@nCompanyID,@nVacTypeID,@nEmpID,@today,@nVacationGroupID,2) As AvlDays,dbo.Fn_CalcAvailDays(@nCompanyID,@nVacTypeID,@nEmpID,@today,@nVacationGroupID,1) As Accrude", QueryParams, connection);


                }
                if (dt.Rows.Count > 0)
                {
                    output.Add("accrude", myFunctions.getDecimalVAL(dt.Rows[0]["Accrude"].ToString()));
                    output.Add("avlDays", myFunctions.getDecimalVAL(dt.Rows[0]["AvlDays"].ToString()));
                    output.Add("nEmpID", nEmpID);
                    output.Add("nVacTypeID", nVacTypeID);
                }
                else
                {
                    output.Add("accrude", 0);
                    output.Add("avlDays", 0);
                }

                return Ok(api.Success(output));
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        private String CalculateGridAnnualDays(int VacTypeID, int empID, int compID, int nVacationGroupID, SqlConnection connection)
        {
            double vacation;
            const double tolerance = 8e-14;
            SortedList paramList = new SortedList();
            paramList.Add("@nCompanyID", compID);
            paramList.Add("@nEmpID", empID);
            paramList.Add("@nVacTypeID", VacTypeID);
            paramList.Add("@nVacationGroupID", nVacationGroupID);


            DateTime toDate = Convert.ToDateTime(dLayer.ExecuteScalar("Select isnull(Max(D_VacDateTo),getdate()) from Pay_VacationDetails Where N_CompanyID =@nCompanyID and  N_EmpID  =@nEmpID and N_VacTypeID =@nVacTypeID and N_VacDays>0 ", paramList, connection).ToString());

            if (nVacationGroupID == 0)
                vacation = Convert.ToDouble(dLayer.ExecuteScalar("Select isnull(SUM(N_VacDays),0) as N_VacDays from Pay_VacationDetails Where N_CompanyID =@nCompanyID and  N_EmpID  =@nEmpID and N_VacTypeID =@nVacTypeID", paramList, connection).ToString());
            else
                vacation = Convert.ToDouble(dLayer.ExecuteScalar("Select isnull(SUM(N_VacDays),0) as N_VacDays from Pay_VacationDetails Where N_CompanyID =@nCompanyID and  N_EmpID  =@nEmpID and N_VacTypeID =@nVacTypeID and isnull(N_VacationGroupID,0) < @nVacationGroupID", paramList, connection).ToString());

            String AvlDays = RoundApproximate(vacation, 0, tolerance, MidpointRounding.AwayFromZero).ToString();


            return AvlDays;
        }

        private static double RoundApproximate(double dbl, int digits, double margin, MidpointRounding mode)
        {
            double fraction = dbl * Math.Pow(10, digits);
            double value = Math.Truncate(fraction);
            fraction = fraction - value;
            if (fraction == 0)
                return dbl;

            double tolerance = margin * dbl;
            // Any remaining fractional value greater than .5 is not a midpoint value. 
            if (fraction > .5)
                return (value + 0.5) / Math.Pow(10, digits);
            else if (fraction < -(0.5))
                return (value + 0.5) / Math.Pow(10, digits);
            else if (fraction == .5)
                return Math.Round(dbl, 1);
            else
                return value / Math.Pow(10, digits);
        }

        //List
        [HttpGet("vacationBenefits")]
        public ActionResult GetVacationBenefits(string nEmpID, int nVacationGroupID)
        {
            DataTable dt = new DataTable();
            SortedList QueryParams = new SortedList();
            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@nEmpID", nEmpID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    object res = dLayer.ExecuteScalar("Select ISNULL(sum(N_VacDays),0) as N_VacDays from Pay_VacationDetails where (B_Allowance=1) and  N_CompanyID=@nCompanyID and N_EmpId=@nEmpID", QueryParams, connection);

                    if (res == null)
                        res = 0;
                    string sql = "";
                    if (nVacationGroupID > 0)
                        sql = "SELECT    N_VacTypeID,X_VacCode,X_VacType as Code,N_Accrued as Value,N_Accrued+isnull(" + res.ToString() + ",0) as Avlbl,CONVERT(bit,0) As Mark,B_IsReturn,X_Type,0 as DetailsID  from vw_pay_EmpVacation_Alowance  where (X_Type='A' or X_Type='T') AND  N_CompanyID=@nCompanyID and N_EmpId=@nEmpID and N_VacDays<=0";
                    else
                        sql = "SELECT    N_VacTypeID,X_VacCode,X_VacType as Code,N_Accrued as Value,N_Accrued+isnull(" + res.ToString() + ",0) as Avlbl,CONVERT(bit,0) As Mark,B_IsReturn,X_Type,0 as DetailsID  from vw_pay_EmpVacation_Alowance  where (X_Type='A' or X_Type='T') AND  N_CompanyID=@nCompanyID and N_EmpId=@nEmpID and N_VacDays>0";

                    dt = dLayer.ExecuteDataTable(sql, QueryParams, connection);

                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                    return Ok(api.Notice("No Results Found"));
                else
                    return Ok(api.Success(dt));

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        //Save....
        [HttpPost("save")]
        public ActionResult SaveTORequest([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable, DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                DataTable Approvals;
                Approvals = ds.Tables["approval"];
                DataRow ApprovalRow = Approvals.Rows[0];
                DataTable Attachment = ds.Tables["attachments"];

                DataTable Benifits = ds.Tables["benifits"];


                SortedList Params = new SortedList();
                DataRow MasterRow = MasterTable.Rows[0];
                var x_VacationGroupCode = MasterRow["x_VacationGroupCode"].ToString();
                int n_VacationGroupID = myFunctions.getIntVAL(MasterRow["n_VacationGroupID"].ToString());
                int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyId"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                int nEmpID = myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());
                int nBranchID = myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());
                int N_NextApproverID = 0;

                

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList EmpParams = new SortedList();

                    if (vacationalreadygiven(DetailTable, connection, transaction))
                    {
                        return Ok(api.Warning("Vacation already requested in this date range."));
                    }

                    EmpParams.Add("@nCompanyID", nCompanyID);
                    EmpParams.Add("@nEmpID", nEmpID);
                    EmpParams.Add("@nFnYearID", nFnYearID);
                    object objEmpName = dLayer.ExecuteScalar("Select X_EmpName From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", EmpParams, connection, transaction);
                    object objEmpCode = dLayer.ExecuteScalar("Select X_EmpCode From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", EmpParams, connection, transaction);

                    if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()))
                    {
                        int N_PkeyID = n_VacationGroupID;
                        string X_Criteria = "N_VacationGroupID=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                        myFunctions.UpdateApproverEntry(Approvals, "Pay_VacationMaster", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "LEAVE REQUEST", N_PkeyID, x_VacationGroupCode, 1, objEmpName.ToString(), 0, "", User, dLayer, connection, transaction);
                        // SaveDocs(Attachment, objEmpCode.ToString(), objEmpName.ToString(), nEmpID, x_VacationGroupCode, n_VacationGroupID,User, connection, transaction);
                        myAttachments.SaveAttachment(dLayer, Attachment, x_VacationGroupCode, n_VacationGroupID, objEmpName.ToString(), objEmpCode.ToString(), nEmpID, "Employee", User, connection, transaction);
                        transaction.Commit();
                        myFunctions.SendApprovalMail(N_NextApproverID, FormID, n_VacationGroupID, "LEAVE REQUEST", x_VacationGroupCode, dLayer, connection, transaction, User);
                        return Ok(api.Success("Leave Request Updated " + "-" + x_VacationGroupCode));
                    }

                    if (x_VacationGroupCode == "@Auto")
                    {


                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", "210");
                        Params.Add("N_BranchID", nBranchID);
                        x_VacationGroupCode = dLayer.GetAutoNumber("Pay_VacationMaster", "x_VacationGroupCode", Params, connection, transaction);
                        if (x_VacationGroupCode == "") { transaction.Rollback(); return Ok(api.Error("Unable to generate leave Request Code")); }
                        MasterTable.Rows[0]["x_VacationGroupCode"] = x_VacationGroupCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Pay_VacationMaster", "n_VacationGroupID", n_VacationGroupID, "", connection, transaction);
                        dLayer.DeleteData("Pay_VacationDetails", "n_VacationGroupID", n_VacationGroupID, "", connection, transaction);
                    }

                    MasterTable.Rows[0]["N_VacTypeID"] =DetailTable.Rows[0]["N_VacTypeID"];
                    MasterTable.AcceptChanges();

                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);
                    n_VacationGroupID = dLayer.SaveData("Pay_VacationMaster", "n_VacationGroupID", MasterTable, connection, transaction);
                    if (n_VacationGroupID > 0)
                    {
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "LEAVE REQUEST", n_VacationGroupID, x_VacationGroupCode, 1, objEmpName.ToString(), 0, "", User, dLayer, connection, transaction);



                        foreach (DataRow var in Benifits.Rows)
                        {
                            bool ticketSelected = false;
                            if (!myFunctions.getBoolVAL(var["Mark"].ToString())) continue;
                            ticketSelected = true;



                            SortedList benifitParam = new SortedList();
                            benifitParam.Add("@nVacTypeID", myFunctions.getIntVAL(var["N_VacTypeID"].ToString()));
                            benifitParam.Add("@nCompanyID", nCompanyID);
                            benifitParam.Add("@nFnYearID", nFnYearID);
                            benifitParam.Add("@nEmpID", nEmpID);

                            object result = dLayer.ExecuteScalar("select ISNULL(Sum(N_VacDays),0) from Pay_VacationDetails where  N_VacTypeID=@nVacTypeID and  N_CompanyID =@nCompanyID and N_EmpID=@nEmpID and N_FnYearID=@nFnYearID", benifitParam, connection, transaction);
                            if (myFunctions.getIntVAL(result.ToString()) <= 0)
                            {
                                if (myFunctions.getIntVAL(var["Value"].ToString()) < 0) continue;

                            }


                            DataRow BenifitsRow = DetailTable.NewRow();
                            var existingRow = DetailTable.Rows[0];
                            BenifitsRow.ItemArray = existingRow.ItemArray.Clone() as object[];

                            BenifitsRow["N_VacDays"] = -1;
                            BenifitsRow["N_TransType"] = 1;
                            BenifitsRow["N_VacationStatus"] = 1;
                            BenifitsRow["B_IsAdjustEntry"] = 0;
                            BenifitsRow["B_Ticket"] = ticketSelected;
                            BenifitsRow["X_VacationCode"] = x_VacationGroupCode;
                            BenifitsRow["B_Allowance"] = 1;
                            BenifitsRow["N_FormID"] = 210;
                            BenifitsRow["N_VacTypeID"] = var["n_VacTypeID"].ToString();
                            BenifitsRow["d_VacDateFrom"] = DBNull.Value;
                            BenifitsRow["d_VacDateTo"] = DBNull.Value;
                            BenifitsRow["n_VacationID"] = 0;
                            DetailTable.Rows.Add(BenifitsRow);
                            DetailTable.AcceptChanges();
                        }
                        for (int j = 0; j < DetailTable.Rows.Count; j++)
                        {
                            DetailTable.Rows[j]["n_VacationGroupID"] = n_VacationGroupID;
                            DetailTable.Rows[j]["X_VacationCode"] = x_VacationGroupCode;
                        }

                        int DetailID = dLayer.SaveData("Pay_VacationDetails", "n_VacationID", DetailTable, connection, transaction);
                        if (DetailID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(api.Error("Unable to save"));
                        }
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }

                    // DataTable Files = ds.Tables["files"];
                    // if (Files.Rows.Count > 0)
                    // {
                    //     if (!dLayer.SaveFiles(Files, "Pay_VacationMaster", "N_VacationGroupID", n_VacationGroupID, nEmpID.ToString(), nCompanyID, connection, transaction))
                    //     {
                    //         transaction.Rollback();
                    //         return Ok(api.Error("Unable to save"));
                    //     }
                    // }

                    // SaveDocs(Attachment, objEmpCode.ToString(), objEmpName.ToString(), nEmpID, x_VacationGroupCode, n_VacationGroupID,User, connection, transaction);
                    myAttachments.SaveAttachment(dLayer, Attachment, x_VacationGroupCode, n_VacationGroupID, objEmpName.ToString(), objEmpCode.ToString(), nEmpID, "Employee", User, connection, transaction);


                    transaction.Commit();
                    myFunctions.SendApprovalMail(N_NextApproverID, FormID, n_VacationGroupID, "LEAVE REQUEST", x_VacationGroupCode, dLayer, connection, transaction, User);
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("x_RequestCode", x_VacationGroupCode.ToString());
                    return Ok(api.Success(res, "Leave Request saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }


        private bool vacationalreadygiven(DataTable Details, SqlConnection connection, SqlTransaction transaction)
        {
            foreach (DataRow var in Details.Rows)
            {
                if (myFunctions.getVAL(var["b_IsAdjustEntry"].ToString()) != 0) { continue; }
                if (myFunctions.getVAL(var["n_VacationGroupID"].ToString()) != 0) { continue; }
                SortedList Params = new SortedList();
                Params.Add("N_CompanyID", myFunctions.getVAL(var["n_CompanyID"].ToString()));
                Params.Add("N_FnYearID", myFunctions.getVAL(var["n_FnYearID"].ToString()));
                Params.Add("N_EmpID", myFunctions.getVAL(var["n_EmpID"].ToString()));
                Params.Add("D_VacDateFrom", var["d_VacDateFrom"].ToString());
                Params.Add("D_VacDateTo", var["d_VacDateTo"].ToString());
                Params.Add("N_VacationID", myFunctions.getVAL(var["n_VacationGroupID"].ToString()));

                DataTable Validation = dLayer.ExecuteDataTablePro("SP_Pay_VacationEntryDateValidation", Params, connection, transaction);
                if (Validation.Rows.Count > 0)
                {
                    return true;
                }
            }
            return false;

        }


        private DataTable FillCodeList(int nCompanyID, int nEmpID, int nVacGroupID, SqlConnection connection)
        {
            DataTable Benifits = new DataTable();

            SortedList benifitParam = new SortedList();
            benifitParam.Add("@nCompanyID", nCompanyID);
            benifitParam.Add("@nEmpID", nEmpID);
            benifitParam.Add("@nVacGroupID", nVacGroupID);

            object res = dLayer.ExecuteScalar("Select ISNULL(sum(N_VacDays),0) as N_VacDays from Pay_VacationDetails where (B_Allowance=1) and  N_CompanyID=@nCompanyID and N_EmpId=@nEmpID", benifitParam, connection);

            if (res == null)
                res = 0;
            string sql = "";
            if (nVacGroupID > 0)
                sql = "SELECT    N_VacTypeID,X_VacCode,X_VacType as Code,N_Accrued as Value,N_Accrued+isnull(" + res.ToString() + ",0) as Avlbl,CONVERT(bit,0) As Mark,B_IsReturn,X_Type,0 as DetailsID  from vw_pay_EmpVacation_Alowance  where (X_Type='A' or X_Type='T') AND  N_CompanyID=@nCompanyID and N_EmpId=@nEmpID and N_VacDays<=0  Group by N_VacTypeID,X_VacCode,X_VacType,N_Accrued,B_IsReturn,X_Type";
            else
                sql = "SELECT    N_VacTypeID,X_VacCode,X_VacType as Code,N_Accrued as Value,N_Accrued+isnull(" + res.ToString() + ",0) as Avlbl,CONVERT(bit,0) As Mark,B_IsReturn,X_Type,0 as DetailsID  from vw_pay_EmpVacation_Alowance  where (X_Type='A' or X_Type='T') AND  N_CompanyID=@nCompanyID and N_EmpId=@nEmpID and N_VacDays>0 Group by N_VacTypeID,X_VacCode,X_VacType,N_Accrued,B_IsReturn,X_Type";
            Benifits = dLayer.ExecuteDataTable(sql, benifitParam, connection);

            DataTable PayVacDetails = dLayer.ExecuteDataTable("Select * from vw_PayVacationDetails Where N_CompanyID=@nCompanyID and N_EmpID =@nEmpID and N_VacationGroupID=@nVacGroupID and (X_Type='A' or X_Type='T')", benifitParam, connection);
            foreach (DataRow var in PayVacDetails.Rows)
            {
                for (int i = 0; i < Benifits.Rows.Count; i++)
                {

                    if (Benifits.Rows[i]["N_VacTypeID"].ToString() == var["N_VacTypeID"].ToString())
                    {
                        Benifits.Rows[i]["DetailsID"] = var["N_VacationID"].ToString();
                        Benifits.Rows[i]["Mark"] = true;
                    }

                }
            }
            Benifits.AcceptChanges();

            return Benifits;
        }

        [HttpDelete()]
        public ActionResult DeleteData(int n_VacationGroupID, int nFnYearID, string comments)
        {
            if (comments == null)
            {
                comments = "";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nTransID", n_VacationGroupID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,isNull(N_EmpID,0) as N_EmpID,X_VacationGroupCode,N_vacTypeID from Pay_VacationMaster where N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID and N_VacationGroupID=@nTransID";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(api.Error("Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    int EmpID = myFunctions.getIntVAL(TransRow["N_EmpID"].ToString());

                    SortedList EmpParams = new SortedList();
                    EmpParams.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    EmpParams.Add("@nEmpID", EmpID);
                    EmpParams.Add("@nFnYearID", nFnYearID);
                    object objEmpName = dLayer.ExecuteScalar("Select X_EmpName From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", EmpParams, connection);


                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, this.FormID, n_VacationGroupID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID, myFunctions.getIntVAL(TransRow["N_EmpID"].ToString()), myFunctions.getIntVAL(TransRow["N_vacTypeID"].ToString()), User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), comments);
                    SqlTransaction transaction = connection.BeginTransaction(); ;

                    string X_Criteria = "N_VacationGroupID=" + n_VacationGroupID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnYearID;
                    string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());
                    //myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString())
                    string status = myFunctions.UpdateApprovals(Approvals, nFnYearID, "LEAVE REQUEST", n_VacationGroupID, TransRow["X_VacationGroupCode"].ToString(), ProcStatus, "Pay_VacationMaster", X_Criteria, objEmpName.ToString(), User, dLayer, connection, transaction);
                    if (status != "Error")
                    {
                        if (ButtonTag == "6" || ButtonTag == "0")
                        {
                            dLayer.DeleteData("Pay_VacationDetails", "N_VacationGroupID", n_VacationGroupID, "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, connection, transaction);
                        }

                        transaction.Commit();
                        return Ok(api.Success("Leave Request " + status + " Successfully"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to delete Leave Request"));
                    }


                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }

        //                 private void SaveDocs(DataTable Attachment, string EmpCode, string EmpName, int nEmpID, string xRequestCode, int nRequestID,ClaimsPrincipal user, SqlConnection connection, SqlTransaction transaction)
        // {
        //     if (Attachment.Rows.Count > 0)
        //     {
        //         string X_DMSMainFolder = "Employee";
        //         string X_DMSSubFolder = this.FormID + "//" + EmpCode + "-" + EmpName;
        //         string X_folderName = X_DMSMainFolder + "//" + X_DMSSubFolder;
        //         try
        //         {
        //             myAttachments.SaveAttachment(dLayer, Attachment, xRequestCode, nRequestID, EmpName, EmpCode, nEmpID, X_folderName, user, connection, transaction);
        //         }
        //         catch (Exception ex)
        //         {
        //             transaction.Rollback();
        //         }
        //     }
        // }



    }
}