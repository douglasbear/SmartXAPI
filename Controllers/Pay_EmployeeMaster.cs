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
    [Route("employee")]
    [ApiController]
    public class Pay_EmployeeMaster : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly string masterDBConnectionString;
        private readonly int FormID;
        private readonly IMyAttachments myAttachments;


        public Pay_EmployeeMaster(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IMyAttachments myAtt, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");
            FormID = 188;
        }

        [HttpGet("listNew")]
        public ActionResult GetEmployeeList(int nFnYearID, bool bAllBranchData, int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@bAllBranchData", bAllBranchData);
            Params.Add("@nBranchID", nBranchID);
            string sqlCommandText = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (bAllBranchData == true)
                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code] as X_EmpCode,Name as X_EmpName,X_Position,X_Department,X_BranchName from vw_PayEmployee_Disp Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID group by N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName";
                    else
                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code] as X_EmpCode ,Name as X_EmpName,X_Position,X_Department,X_BranchName from vw_PayEmployee_Disp Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and (N_BranchID=0 or N_BranchID=@nBranchID) group by N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName";
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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("list")]
        public ActionResult GetEmployeeList(int? nCompanyID, int nFnYearID, bool bAllBranchData, int nBranchID, int nEmpID, int nProjectID, int nUserEmpID, bool isRentalemp,bool essLeave,int loginEmpID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@bAllBranchData", bAllBranchData);
            Params.Add("@nBranchID", nBranchID);
            Params.Add("@nEmpID", nEmpID);
            string sqlCommandText = "";
            string projectFilter = "";
            string RentalEmp = "";
           string managers="";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object nReportToID = dLayer.ExecuteScalar("Select N_SuperVisorID From pay_supervisor Where N_CompanyID ="+nCompanyID+" and N_EmpID=" + loginEmpID + " ", Params, connection);
                    
                    if(loginEmpID>0)
                    {
                         if(nReportToID==null || nReportToID.ToString()=="")
                         { managers="and N_EmpID in (select N_EmpID from Pay_employee where N_CompanyID="+nCompanyID+" and (N_ReportingToID="+loginEmpID+" or N_EmpID="+loginEmpID+"))";
                         }
                         else
                         {
                         managers="and N_EmpID in (select N_EmpID from Pay_employee where N_CompanyID="+nCompanyID+" and (N_ReportToID="+myFunctions.getIntVAL(nReportToID.ToString())+" or N_ReportingToID="+loginEmpID+" or N_EmpID="+loginEmpID+"))";
                         }
                    }
                    object nReportingToID1 = dLayer.ExecuteScalar("Select count(N_EmpID) From Pay_Employee Where N_CompanyID =@nCompanyID and N_ReportingToID=" + loginEmpID + " ", Params, connection);
                    int nReportingToID = myFunctions.getIntVAL(nReportingToID1.ToString());


                    int filterByProject = myFunctions.getIntVAL(dLayer.ExecuteScalar("select isNull(max(N_Value),0) as val from gen_settings where x_Group='HR' and x_Description='FilterDelegateEmployeeByProject' and n_CompanyID=" + nCompanyID, connection).ToString());

                    if (isRentalemp == true)
                    {
                        RentalEmp = RentalEmp + " and n_EmpID NOT IN (select isnull(n_RentalEmpID,0) from inv_itemmaster where N_CompanyId=@nCompanyID)";
                    }
                    if (nEmpID > 0 && filterByProject > 0)
                        projectFilter = " and N_ProjectID =(select max(isNull(N_ProjectID,0)) from vw_PayEmployee_Disp where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID ) and n_EmpID<>@nEmpID ";
                    if (bAllBranchData == true)
                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code] as X_EmpCode,Name as X_EmpName,X_Position,X_Department,X_BranchName,X_EmergencyContctPersonH,X_EmergencyNumH,X_HCMobileNo,X_HCTelNo,X_Phone1,X_PassportNo,N_NationalityID,x_Nationality,D_HireDate,X_EmailID,X_Sex,D_DOB,D_JoinDate from vw_PayEmployee_Disp Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID " + projectFilter + " and (N_Status = 0 OR N_Status = 1) " + RentalEmp + "  "+managers+" group by N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName,X_EmergencyContctPersonH,X_EmergencyNumH,X_HCMobileNo,X_HCTelNo,X_Phone1,X_PassportNo,N_NationalityID,x_Nationality,D_HireDate,X_EmailID,X_Sex,D_DOB,D_JoinDate";
                    else
                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code] as X_EmpCode ,Name as X_EmpName,X_Position,X_Department,X_BranchName,X_EmergencyContctPersonH,X_EmergencyNumH,X_HCMobileNo,X_HCTelNo,X_Phone1,X_PassportNo,N_NationalityID,x_Nationality,D_HireDate,X_EmailID,X_Sex,D_DOB,D_JoinDate from vw_PayEmployee_Disp Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and (N_BranchID=0 or N_BranchID=@nBranchID)  " + projectFilter + "  and (N_Status = 0 OR N_Status = 1) " + RentalEmp + "  "+managers+"  group by N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName,X_EmergencyContctPersonH,X_EmergencyNumH,X_HCMobileNo,X_HCTelNo,X_Phone1,X_PassportNo,N_NationalityID,x_Nationality,D_HireDate,X_EmailID,X_Sex,D_DOB,D_JoinDate";
                    if (nProjectID > 0)
                    {
                        bool flag = false;
                        object nIsManager = dLayer.ExecuteScalar("select N_ProjectManager from Vw_InvCustomerProjects where N_ProjectID=" + nProjectID + " and N_CompanyID=" + nCompanyID + "", Params, connection);
                        object nIsCoordinator = dLayer.ExecuteScalar("select N_ProjectCoordinator from Vw_InvCustomerProjects where N_ProjectID=" + nProjectID + " and N_CompanyID=" + nCompanyID + "", Params, connection);
                        if (nIsManager != null)
                        {
                            if (nUserEmpID == myFunctions.getIntVAL(nIsManager.ToString()))
                                flag = false;
                        }
                        if (nIsCoordinator != null)
                        {
                            if (nUserEmpID == myFunctions.getIntVAL(nIsCoordinator.ToString()))
                                flag = false;
                        }
                        if (flag == true)
                        {
                            if (bAllBranchData == true)
                                sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code] as X_EmpCode,Name as X_EmpName,X_Position,X_Department,X_BranchName,X_EmergencyContctPersonH,X_EmergencyNumH,X_HCMobileNo,X_HCTelNo from vw_PayEmployee_Disp Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID " + projectFilter + " and (N_Status = 0 OR N_Status = 1) " + RentalEmp + " group by N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName,X_EmergencyContctPersonH,X_EmergencyNumH,X_HCMobileNo,X_HCTelNo";
                            else
                                sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code] as X_EmpCode ,Name as X_EmpName,X_Position,X_Department,X_BranchName,X_EmergencyContctPersonH,X_EmergencyNumH,X_HCMobileNo,X_HCTelNo from vw_PayEmployee_Disp Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and (N_BranchID=0 or N_BranchID=@nBranchID)  " + projectFilter + "  and (N_Status = 0 OR N_Status = 1) " + RentalEmp + " group by N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName,X_EmergencyContctPersonH,X_EmergencyNumH,X_HCMobileNo,X_HCTelNo";
                        }
                        else
                        {
                            sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code] as X_EmpCode,Name as X_EmpName,X_Position,X_Department,X_BranchName,X_EmergencyContctPersonH,X_EmergencyNumH,X_HCMobileNo,X_HCTelNo from vw_PayEmployee_Disp Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID " + projectFilter + " and (N_Status = 0 OR N_Status = 1) and N_EmpID=" + nUserEmpID + " " + RentalEmp + " group by N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName,X_EmergencyContctPersonH,X_EmergencyNumH,X_HCMobileNo,X_HCTelNo";
                        }

                    }



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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("delegateList")]
        public ActionResult GetEmployeeList(int nFnYearID, bool bAllBranchData, int nBranchID, int nEmpID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@bAllBranchData", bAllBranchData);
            Params.Add("@nBranchID", nBranchID);
            Params.Add("@nEmpID", nEmpID);
            string sqlCommandText = "";
            string projectFilter = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int filterByProject = myFunctions.getIntVAL(dLayer.ExecuteScalar("select isNull(N_Value,0) as val from gen_settings where x_Group='HR' and x_Description='FilterDelegateEmployeeByProject' and n_CompanyID=" + nCompanyID, connection).ToString());
                    if (filterByProject > 0)
                        projectFilter = " and N_ProjectID =(select max(isNull(N_ProjectID,0)) from vw_PayEmployee_Disp where N_EmpID=@nEmpID and  (N_Status = 0 OR N_Status = 1) and  N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID ) and n_EmpID<>@nEmpID ";
                    if (bAllBranchData == true)
                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code] as X_EmpCode,Name as X_EmpName,X_Position,X_Department,X_BranchName from vw_PayEmployee_Disp Where N_CompanyID=@nCompanyID and (N_Status = 0 OR N_Status = 1) and N_EmpID<>" + nEmpID + " AND  N_FnYearID=@nFnYearID " + projectFilter + "  group by N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName";
                    else
                        sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code] as X_EmpCode ,Name as X_EmpName,X_Position,X_Department,X_BranchName from vw_PayEmployee_Disp Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID AND (N_Status = 0 OR N_Status = 1) and N_EmpID<>" + nEmpID + " and (N_BranchID=0 or N_BranchID=@nBranchID)  " + projectFilter + "   group by N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName";


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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetEmployeeDetails(string xEmpCode, int nFnYearID, bool bAllBranchData, int nBranchID, string xRecruitmentCode)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable Pay_Employee, pay_EmpAddlInfo, pay_EmployeeDependence, pay_EmployeeAlerts, acc_OtherInformation, pay_EmpAccruls, pay_EmployeeSub, pay_Getsalary, rec_Recruitment,
            pay_EmployeeEducation, pay_EmploymentHistory, pay_EmpStatus;
            SortedList Result = new SortedList();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@bAllBranchData", bAllBranchData);
            Params.Add("@nBranchID", nBranchID);
            Params.Add("@xEmpCode", xEmpCode);

            string branchSql = bAllBranchData == false ? " and (vw_PayEmployee.N_BranchID=0 or vw_PayEmployee.N_BranchID=@nBranchID ) " : "";
            string EmployeeSql = "Select X_LedgerName_Ar As X_LedgerName,[Loan Ledger Name_Ar] As [Loan Ledger Name],[Loan Ledger Name_Ar] AS X_LoanLedgerName_Ar,[Loan Ledger Code] AS X_LoanLedgerCode,[Loan Ledger Name] AS X_LoanLedgerName, *,CASE WHEN dbo.Pay_VacationDetails.D_VacDateFrom<=CONVERT(date, GETDATE()) AND dbo.Pay_VacationDetails.D_VacDateTo>=CONVERT(date, GETDATE()) AND dbo.Pay_VacationDetails.N_VacDays<0 and dbo.Pay_VacationDetails.B_IsSaveDraft=0 Then '1' Else vw_PayEmployee.N_Status end AS [Status] ,(Select count(1) from Pay_PaymentDetails Where N_CompanyID = vw_PayEmployee.N_CompanyID AND N_EmpID = vw_PayEmployee.N_EmpID and ISNULL(B_BeginingBalEntry,0) = 0 and N_FormID = 190 ) AS N_NoEdit from vw_PayEmployee Left Outer Join Pay_Supervisor On vw_PayEmployee.N_ReportToID= Pay_Supervisor.N_SupervisorID and vw_PayEmployee.N_CompanyID= Pay_Supervisor.N_CompanyID  Left Outer Join Pay_Employee On Pay_Supervisor.N_EmpID=Pay_Employee.N_EmpID and Pay_employee.N_FnYearID=@nFnYearID Left Outer Join  dbo.Pay_VacationDetails ON vw_PayEmployee.N_EmpID = dbo.Pay_VacationDetails.N_EmpID AND dbo.Pay_VacationDetails.D_VacDateFrom <= CONVERT(date, GETDATE()) AND dbo.Pay_VacationDetails.D_VacDateTo >=CONVERT(date, GETDATE()) AND dbo.Pay_VacationDetails.N_VacDays<0  Where vw_PayEmployee.N_CompanyID=@nCompanyID and vw_PayEmployee.N_FnYearID=@nFnYearID and vw_PayEmployee.X_EmpCode=@xEmpCode " + branchSql;
            string contactSql = "Select * from vw_ContactDetails where N_CompanyID =@nCompanyID and N_EmpID=@nEmpID order by N_ContactDetailsID desc";
            string salarySql = "Select *,(Select count(1) from Pay_PaymentDetails Where N_CompanyID = vw_EmpPayInformation.N_CompanyID AND N_EmpID = vw_EmpPayInformation.N_EmpID AND N_PayID = vw_EmpPayInformation.N_PayID AND N_Value = vw_EmpPayInformation.N_value ) AS N_NoEdit from vw_EmpPayInformation Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_EmpID=@nEmpID and D_EffectiveDate= (select max(D_EffectiveDate) from vw_EmpPayInformation where  N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_EmpID=@nEmpID ) order by vw_EmpPayInformation.N_PaySetupID,vw_EmpPayInformation.N_Value";
            string accrualSql = "Select *,(Select count(1) from Pay_VacationDetails Where N_CompanyID = vw_Pay_EmployeeAccrul.N_CompanyID AND N_EmpID = vw_Pay_EmployeeAccrul.N_EmpID AND N_VacTypeID = vw_Pay_EmployeeAccrul.N_VacTypeID ) AS N_NoEdit from vw_Pay_EmployeeAccrul Where N_CompanyID=@nCompanyID  and N_EmpID=@nEmpID";
            string empDepedenceSql = "Select * from Pay_EmployeeDependence Inner Join Pay_Relation on Pay_EmployeeDependence.N_RelationID = Pay_Relation.N_RelationID and Pay_EmployeeDependence.N_CompanyID = Pay_Relation.N_CompanyID    Where Pay_EmployeeDependence.N_CompanyID=@nCompanyID and N_EmpID=@nEmpID";
            string empEducationSql = "Select * from Pay_EmployeeEducation where N_CompanyID =@nCompanyID and N_EmpID=@nEmpID";
            string employementHistorySql = "Select * from Pay_EmploymentHistory where N_CompanyID=@nCompanyID and N_EmpID=@nEmpID";
            string empAddlInfoSql = "Select * from vw_EmpAddlInfo where N_CompanyID=@nCompanyID and N_EmpID=@nEmpID and N_FnYearID=@nFnYearID";
            string empAlertSql = "Select * from Pay_EmployeeAlerts where N_CompanyID=@nCompanyID and N_EmpID=@nEmpID";
            string empStatusSql = "Select * from vw_EmpStatus where N_CompanyID=@nCompanyID and N_EmpID=@nEmpID and N_FnYearID=@nFnYearID";
            string RecritmentSql = "Select * from vw_RecruitmentToEmployee Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and X_EmpCode=@xRecruitmentCode";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (xRecruitmentCode != null)
                    {
                        Params.Add("@xRecruitmentCode", xRecruitmentCode);
                        Params["@xEmpCode"] = "";
                        accrualSql = " select N_vacTypeID,Name,N_Accrued,X_Type,X_Period,B_InActive from [vw_PayAccruedCode_List] Where N_CompanyID=@nCompanyID and isnull(N_CountryID,0)=1 and isnull(B_InActive,0)=0 order by X_Type desc";
                        // string paySetupSql = "Select * from vw_PayMaster Where  N_CompanyID=@nCompanyID  and (N_PayTypeID <>11 and N_PayTypeID <>12 and N_PayTypeID <>14) and N_FnYearID=@nFnYearID  and N_PaymentID=5 and (N_Paymethod=0 or N_Paymethod=3) and B_InActive=0";
                        // string payBenifitsSql = "Select * from vw_PayMaster Where ( N_CompanyID=@nCompanyID and  N_FnYearID=@nFnYearID and N_PaymentID in (6,7)  and (N_PaytypeID <>14 ) and (N_Paymethod=0 or N_Paymethod=3) or N_PayTypeID = 11 and N_CompanyID=@nCompanyID and  N_FnYearID=@nFnYearID) and isnull(B_InActive,0)=0 order by N_PayTypeID";
                        // string PayCodeSql = "Select * From [vw_Pay_Sal4perPaycodes] Where N_CompanyID=@nCompanyID and N_FnyearID =@nFnYearID";
                        empAddlInfoSql = "Select N_OtherCode,X_subject from Acc_OtherInformationMaster Where  N_CompanyID=@nCompanyID and  N_FormID=188";

                        Pay_Employee = dLayer.ExecuteDataTable(RecritmentSql, Params, connection);
                    }
                    else
                        Pay_Employee = dLayer.ExecuteDataTable(EmployeeSql, Params, connection);

                    Pay_Employee = _api.Format(Pay_Employee);
                    if (Pay_Employee.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        Params.Add("@nEmpID", Pay_Employee.Rows[0]["N_EmpID"].ToString());
                        pay_EmployeeSub = dLayer.ExecuteDataTable(contactSql, Params, connection);
                        pay_Getsalary = dLayer.ExecuteDataTable(salarySql, Params, connection);
                        pay_EmpAccruls = dLayer.ExecuteDataTable(accrualSql, Params, connection);
                        pay_EmployeeDependence = dLayer.ExecuteDataTable(empDepedenceSql, Params, connection);
                        pay_EmployeeEducation = dLayer.ExecuteDataTable(empEducationSql, Params, connection);
                        pay_EmploymentHistory = dLayer.ExecuteDataTable(employementHistorySql, Params, connection);
                        pay_EmpAddlInfo = dLayer.ExecuteDataTable(empAddlInfoSql, Params, connection);
                        pay_EmployeeAlerts = dLayer.ExecuteDataTable(empAlertSql, Params, connection);
                        pay_EmpStatus = dLayer.ExecuteDataTable(empStatusSql, Params, connection);

                        DataTable Attachements = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(Pay_Employee.Rows[0]["N_EmpID"].ToString()), myFunctions.getIntVAL(Pay_Employee.Rows[0]["N_EmpID"].ToString()), this.FormID, myFunctions.getIntVAL(Pay_Employee.Rows[0]["N_FnYearID"].ToString()), User, connection);


                        Result.Add("pay_Employee", Pay_Employee);
                        Result.Add("pay_EmployeeSub", pay_EmployeeSub);
                        Result.Add("pay_Getsalary", pay_Getsalary);
                        Result.Add("pay_EmpAccruls", pay_EmpAccruls);
                        Result.Add("pay_EmployeeDependence", pay_EmployeeDependence);
                        Result.Add("pay_EmployeeEducation", pay_EmployeeEducation);
                        Result.Add("pay_EmploymentHistory", pay_EmploymentHistory);
                        Result.Add("pay_EmpAddlInfo", pay_EmpAddlInfo);
                        Result.Add("pay_EmployeeAlerts", pay_EmployeeAlerts);
                        Result.Add("attachments", Attachements);
                        Result.Add("pay_EmpStatus", pay_EmpStatus);

                        return Ok(_api.Success(Result));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("default")]
        public ActionResult GetEmployeeDefault(int nFnYearID, int nBranchID, int nCountryID, string xRecruitmentCode)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable pay_Codes, pay_benifits, pay_EmpAccruls, pay_OtherInfo, pay_PaySetup, Pay_Employee, pay_EmployeeEducation, pay_EmploymentHistory;

            SortedList Result = new SortedList();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nBranchID", nBranchID);
            Params.Add("@nCountryID", nCountryID);

            string accrualSql = " select N_vacTypeID,Name,N_Accrued,X_Type,X_Period,B_InActive,N_PayID from [vw_PayAccruedCode_List] Where N_CompanyID=@nCompanyID and isnull(N_CountryID,0)=@nCountryID and isnull(B_InActive,0)=0 order by X_Type desc";
            string paySetupSql = "Select * from vw_PayMaster Where  N_CompanyID=@nCompanyID  and (N_PayTypeID <>11 and N_PayTypeID <>12 and N_PayTypeID <>14) and N_FnYearID=@nFnYearID  and N_PaymentID=5 and (N_Paymethod=0 or N_Paymethod=3 or N_PayMethod=4) and B_InActive=0";
            // string payBenifitsSql = "Select * from vw_PayMaster Where  N_CompanyID=@nCompanyID and  N_FnYearID=@nFnYearID and (N_PaymentID=6 or N_PaymentID=7 )and N_PaytypeID<>14  and (N_Paymethod=0 or N_Paymethod=3)";
            string payBenifitsSql = "Select * from vw_PayMaster Where ( N_CompanyID=@nCompanyID and  N_FnYearID=@nFnYearID and N_PaymentID in (6,7)  and (N_PaytypeID <>14 ) and (N_Paymethod=0 or N_Paymethod=3) or N_PayTypeID = 11 and N_CompanyID=@nCompanyID and  N_FnYearID=@nFnYearID) and isnull(B_InActive,0)=0 order by N_PayTypeID";
            string PayCodeSql = "Select * From [vw_Pay_Sal4perPaycodes] Where N_CompanyID=@nCompanyID and N_FnyearID =@nFnYearID";
            string payOthInfoSql = "Select N_OtherCode,X_subject from Acc_OtherInformationMaster Where  N_CompanyID=@nCompanyID and  N_FormID=188";
            string RecritmentSql = "Select * from vw_RecruitmentToEmployee Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and X_EmpCode=@xRecruitmentCode";
            string empEducationSql = "select * from Rec_CandidateEducation where N_CompanyID=@nCompanyID  and N_RecruitmentID=@recID";
            string employementHistorySql = "select * from Rec_EmploymentHistory where N_CompanyID=@nCompanyID  and N_RecruitmentID=@recID";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    pay_PaySetup = dLayer.ExecuteDataTable(paySetupSql, Params, connection);
                    pay_EmpAccruls = dLayer.ExecuteDataTable(accrualSql, Params, connection);
                    pay_benifits = dLayer.ExecuteDataTable(payBenifitsSql, Params, connection);
                    pay_Codes = dLayer.ExecuteDataTable(PayCodeSql, Params, connection);
                    pay_OtherInfo = dLayer.ExecuteDataTable(payOthInfoSql, Params, connection);
                    pay_PaySetup = myFunctions.AddNewColumnToDataTable(pay_PaySetup, "summeryInfo", typeof(DataTable), null);
                    foreach (DataRow dRow in pay_PaySetup.Rows)
                    {
                        DataTable dtNode = new DataTable();
                        int N_PayID = myFunctions.getIntVAL(dRow["N_PayID"].ToString());
                        string Pay_SummaryPercentageSql = "SELECT    * From Pay_SummaryPercentage inner join Pay_PayType on Pay_SummaryPercentage.N_PayTypeID = Pay_PayType.N_PayTypeID and Pay_SummaryPercentage.N_CompanyID = Pay_PayType.N_CompanyID  Where Pay_SummaryPercentage.N_PayID =" + N_PayID + " and Pay_SummaryPercentage.N_CompanyID=" + myFunctions.GetCompanyID(User);
                        DataTable summeryInfo = dLayer.ExecuteDataTable(Pay_SummaryPercentageSql, connection);

                        dRow["summeryInfo"] = summeryInfo;

                    }
                    pay_benifits = myFunctions.AddNewColumnToDataTable(pay_benifits, "summeryInfo", typeof(DataTable), null);
                    foreach (DataRow dRow in pay_benifits.Rows)
                    {
                        DataTable dtNode = new DataTable();
                        int N_PayID = myFunctions.getIntVAL(dRow["N_PayID"].ToString());
                        string Pay_SummaryPercentageSql = "SELECT    * From Pay_SummaryPercentage inner join Pay_PayType on Pay_SummaryPercentage.N_PayTypeID = Pay_PayType.N_PayTypeID and Pay_SummaryPercentage.N_CompanyID = Pay_PayType.N_CompanyID  Where Pay_SummaryPercentage.N_PayID =" + N_PayID + " and Pay_SummaryPercentage.N_CompanyID=" + myFunctions.GetCompanyID(User);
                        DataTable summeryInfo = dLayer.ExecuteDataTable(Pay_SummaryPercentageSql, connection);

                        dRow["summeryInfo"] = summeryInfo;

                    }
                    if (xRecruitmentCode != null)
                    {
                        Params.Add("@xRecruitmentCode", xRecruitmentCode);
                        Pay_Employee = dLayer.ExecuteDataTable(RecritmentSql, Params, connection);
                        DataRow MasterRow = Pay_Employee.Rows[0];
                        Params.Add("@recID", MasterRow["N_RecruitmentID"]);
                        pay_EmployeeEducation = dLayer.ExecuteDataTable(empEducationSql, Params, connection);
                        pay_EmploymentHistory = dLayer.ExecuteDataTable(employementHistorySql, Params, connection);
                        Pay_Employee = _api.Format(Pay_Employee);
                        Result.Add("pay_Employee", Pay_Employee);
                        Result.Add("pay_EmployeeEducation", pay_EmployeeEducation);
                        Result.Add("pay_EmploymentHistory", pay_EmploymentHistory);
                    }

                    pay_PaySetup.AcceptChanges();
                    pay_PaySetup = _api.Format(pay_PaySetup);
                    pay_EmpAccruls = _api.Format(pay_EmpAccruls);
                    pay_benifits = _api.Format(pay_benifits);
                    pay_Codes = _api.Format(pay_Codes);
                    pay_OtherInfo = _api.Format(pay_OtherInfo);
                    Result.Add("pay_PaySetup", pay_PaySetup);
                    Result.Add("pay_EmpAccruls", pay_EmpAccruls);
                    Result.Add("pay_benifits", pay_benifits);
                    Result.Add("pay_Codes", pay_Codes);
                    Result.Add("pay_OtherInfo", pay_OtherInfo);


                    return Ok(_api.Success(Result));

                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        [HttpGet("dashboardList")]
        public ActionResult GetEmployeeDashboardList(int nFnYearID, bool bAllBranchData, int nBranchID, int EmpStatus, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, string screen)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandCount = "";
            string sqlCommandText = "";
            string Searchkey = "";
            string Criteria = " where N_CompanyID =@nCompanyID and N_FnYearID =@nFnYearID ";

            if (bAllBranchData == false)
            {
                Criteria = Criteria + " and N_BranchID=@nBranchID ";
                Params.Add("@nBranchID", nBranchID);
            }

            if (screen == "Employee Information")
                Criteria = Criteria + "and  N_Status<>3 and N_Status<>2 ";
            if (screen == "Separated Employees")
                Criteria = Criteria + "and  (N_Status=3 or N_Status=2) ";
            if (screen == "Probation Employees")
                Criteria = Criteria + "and  D_ProbationEndDate> GETDATE() and  D_ProbationEndDate < (GETDATE()+14) ";
            if(screen=="Employees On Leave")
                Criteria=Criteria+" and N_EmpID in ( select N_EmpID from Pay_VacationDetails where N_VacDays < 0 and B_IsAdjustEntry =0 and (Cast(D_VacDateFrom as DATE)<=Cast(GETDATE() as DATE) and Cast(D_VacDateTo as DATE)>=cast(GETDATE() as DATE)) and N_CompanyID ="+nCompanyID+" and N_FnYearId= "+nFnYearID+")";

            if (EmpStatus == 0)
                Criteria = Criteria + " and N_Status<>3 and N_Status<>2 ";
            else if (EmpStatus == 1)
                Criteria = Criteria + " and (N_Status =3 or N_Status =2) ";



            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_EmployeeCode like '%" + xSearchkey + "%' or X_EmployeeName like '%" + xSearchkey + "%' or X_Position like '%" + xSearchkey + "%' or X_Department like '%" + xSearchkey + "%' or X_BranchName like '%" + xSearchkey + "%' or cast(D_HireDate as VarChar) like '%" + xSearchkey + "%' or X_Nationality like '%" + xSearchkey + "%' or X_TypeName like '%" + xSearchkey + "%' or X_EmailID like '%" + xSearchkey + "%' or Cast(D_IqamaExpiry as VarChar) like '%" + xSearchkey + "%' or X_IqamaNo like '%" + xSearchkey + "%' or X_PhoneNo like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_EmployeeCode desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {

                    case "d_HireDate":
                        xSortBy = "Cast(D_HireDate as DateTime )" + xSortBy.Split(" ")[1];
                        break;
                    case "d_IqamaExpiry":
                        xSortBy = "Cast(D_IqamaExpiry as DateTime )" + xSortBy.Split(" ")[1];
                        break;

                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }


            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_FnYearID,N_Branchid,B_Inactive,N_EmpID,N_Status,N_EmpTypeID,X_EmployeeCode,X_EmployeeName,X_Position,X_Department,X_BranchName,D_HireDate,X_TypeName,X_Nationality,X_IqamaNo,X_Sex,X_PhoneNo,N_TicketCount,X_EmailID,D_IqamaExpiry,D_ProbationEndDate from vw_PayEmployee_Dashboard " + Criteria + Searchkey + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_FnYearID,N_Branchid,B_Inactive,N_EmpID,N_Status,N_EmpTypeID,X_EmployeeCode,X_EmployeeName,X_Position,X_Department,X_BranchName,D_HireDate,X_TypeName,X_Nationality,X_IqamaNo,X_Sex,X_PhoneNo,N_TicketCount,X_EmailID,D_IqamaExpiry,D_ProbationEndDate from vw_PayEmployee_Dashboard " + Criteria + Searchkey + " and N_EmpID not in (select top(" + Count + ") N_EmpID from vw_PayEmployee_Dashboard " + Criteria + Searchkey + xSortBy + " ) " + xSortBy;

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count  from vw_PayEmployee_Dashboard " + Criteria + Searchkey;
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




        [HttpPost("updateEmployee")]
        public ActionResult UpdateEmployee_New([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable ContactsTable;
                DataTable DependenceTable;
                DataTable EduTable;
                DataTable HistoryTable;
                MasterTable = ds.Tables["master"];
                ContactsTable = ds.Tables["contacts"];
                DependenceTable = ds.Tables["dependence"];
                EduTable = ds.Tables["education"];
                HistoryTable = ds.Tables["history"];
                SortedList Params = new SortedList();
                DataRow MasterRow = MasterTable.Rows[0];
                DataTable Approvals;
                Approvals = ds.Tables["approval"];
                DataRow ApprovalRow = Approvals.Rows[0];
                DataTable Attachment = ds.Tables["attachments"];

                var X_EmpUpdateCode = MasterRow["X_EmpUpdateCode"].ToString();
                int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyId"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                int nEmpID = myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());
                int nEmpUpdateID = myFunctions.getIntVAL(MasterRow["n_EmpUpdateID"].ToString());
                int N_UserID = myFunctions.getIntVAL(MasterRow["N_UserID"].ToString());
                int N_NextApproverID = 0;
                int N_SaveDraft = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList EmpParams = new SortedList();
                    EmpParams.Add("@nCompanyID", nCompanyID);
                    EmpParams.Add("@nEmpID", nEmpID);
                    EmpParams.Add("@nFnYearID", nFnYearID);
                    EmpParams.Add("@nEmpUpdateID", nEmpUpdateID);
                    object objEmpName = dLayer.ExecuteScalar("Select X_EmpName From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID  and N_FnYearID=@nFnYearID", EmpParams, connection, transaction);

                    if ((myFunctions.getIntVAL(ApprovalRow["isApprovalSystem"].ToString()) == 0 || !myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString())) && nEmpUpdateID > 0)
                    {
                        int N_PkeyID = nEmpUpdateID;
                        string X_Criteria = "N_EmpUpdateID=" + nEmpUpdateID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                        myFunctions.UpdateApproverEntry(Approvals, "Pay_EmployeeUpdate", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "EMPLOYEE", N_PkeyID, X_EmpUpdateCode, 1, objEmpName.ToString(), 0, "", 0, User, dLayer, connection, transaction);

                        N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select isnull(CAST(B_IsSaveDraft as INT),0) from Pay_EmployeeUpdate where N_CompanyID=@nCompanyID and N_EmpUpdateID=@nEmpUpdateID", EmpParams, connection, transaction).ToString());

                        if (N_SaveDraft == 0)
                        {
                            SortedList QueryParams = new SortedList();
                            DataTable tmpEmployee = dLayer.ExecuteDataTable("Select * From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID  and N_FnYearID=@nFnYearID", EmpParams, connection, transaction);

                            string empImage1 = myFunctions.ContainColumn("i_Employe_Image", MasterTable) ? MasterTable.Rows[0]["i_Employe_Image"].ToString() : "";
                            Byte[] empImageBitmap1 = new Byte[empImage1.Length];
                            empImageBitmap1 = Convert.FromBase64String(empImage1);
                            if (myFunctions.ContainColumn("i_Employe_Image", MasterTable))
                                MasterTable.Columns.Remove("i_Employe_Image");
                            if (myFunctions.ContainColumn("i_Employe_Image", tmpEmployee))
                                tmpEmployee.Columns.Remove("i_Employe_Image");
                            if (myFunctions.ContainColumn("I_Employe_Sign", MasterTable))
                                MasterTable.Columns.Remove("I_Employe_Sign");
                            if (myFunctions.ContainColumn("I_Employe_Sign", tmpEmployee))
                                tmpEmployee.Columns.Remove("I_Employe_Sign");

                            if (tmpEmployee.Rows.Count > 0)
                            {
                                for (int k = 0; k < tmpEmployee.Columns.Count; k++)
                                {
                                    for (int l = 0; l < MasterTable.Columns.Count; l++)
                                    {
                                        if (tmpEmployee.Columns[k].ColumnName.ToString().ToLower() == MasterTable.Columns[l].ColumnName.ToString().ToLower())
                                        {
                                            if (MasterTable.Columns[l].ColumnName.ToString().Contains("n_") && MasterTable.Rows[0][MasterTable.Columns[l].ColumnName].ToString() == "")
                                                MasterTable.Rows[0][MasterTable.Columns[l].ColumnName] = 0;
                                            tmpEmployee.Rows[0][tmpEmployee.Columns[k].ColumnName] = MasterTable.Rows[0][MasterTable.Columns[l].ColumnName].ToString();
                                        }
                                    }
                                }
                            }

                            string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID =" + nFnYearID + " and X_EmpCode='" + tmpEmployee.Rows[0]["x_EmpCode"].ToString() + "'";
                            string X_Crieteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID =" + nFnYearID;
                            nEmpID = dLayer.SaveData("pay_Employee", "n_EmpID", DupCriteria, X_Crieteria, tmpEmployee, connection, transaction);
                            if (nEmpID <= 0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, "Unable to save"));
                            }
                            else
                            {
                                if (empImage1.Length > 0)
                                    dLayer.SaveImage("pay_Employee", "i_Employe_Image", empImageBitmap1, "n_EmpID", nEmpID, connection, transaction);

                            }
                            //Update Contacts
                            if (ContactsTable.Rows.Count > 0 && myFunctions.getIntVAL(ContactsTable.Rows[0]["N_ContactDetailsID"].ToString()) > 0)
                            {
                                QueryParams.Add("@N_ContactDetailsID", myFunctions.getIntVAL(ContactsTable.Rows[0]["N_ContactDetailsID"].ToString()));

                                string ContactQry = "update Pay_EmployeeSub set X_EmergencyContctPerson=" + ContactsTable.Rows[0]["X_EmergencyContctPerson"].ToString() + ", X_EmergencyContctPersonH=" + ContactsTable.Rows[0]["X_EmergencyContctPersonH"].ToString() + ", X_EmergencyRelation=" + ContactsTable.Rows[0]["X_EmergencyRelation"].ToString() + ", X_EmergencyRelationH=" + ContactsTable.Rows[0]["X_EmergencyRelationH"].ToString() + ", X_EmergencyEmail=" + ContactsTable.Rows[0]["X_EmergencyEmail"].ToString() + ", X_EmergencyEmailH=" + ContactsTable.Rows[0]["X_EmergencyEmailH"].ToString() + ","
                                                    + "X_EmergencyAddress=" + ContactsTable.Rows[0]["X_EmergencyAddress"].ToString() + ", X_EmergencyAddressH=" + ContactsTable.Rows[0]["X_EmergencyAddressH"].ToString() + ", X_EmergencyNum=" + ContactsTable.Rows[0]["X_EmergencyNum"].ToString() + ", X_EmergencyNumH=" + ContactsTable.Rows[0]["X_EmergencyNumH"].ToString() + ", X_EmergencyTelNo=" + ContactsTable.Rows[0]["X_EmergencyTelNo"].ToString() + ", X_EmergencyPOBoxNo=" + ContactsTable.Rows[0]["X_EmergencyPOBoxNo"].ToString() + ","
                                                    + " X_EmergencyCity=" + ContactsTable.Rows[0]["X_EmergencyCity"].ToString() + ", X_EmergencyTelNoH=" + ContactsTable.Rows[0]["X_EmergencyTelNoH"].ToString() + ", X_EmergencyPOBoxNoH=" + ContactsTable.Rows[0]["X_EmergencyPOBoxNoH"].ToString() + ", X_EmergencyCityH=" + ContactsTable.Rows[0]["X_EmergencyCityH"].ToString() + ", X_WCAddress=" + ContactsTable.Rows[0]["X_WCAddress"].ToString() + ", X_WCCity=" + ContactsTable.Rows[0]["X_WCCity"].ToString() + ", X_WCMobileNo=" + ContactsTable.Rows[0]["X_WCMobileNo"].ToString() + ","
                                                    + " X_WCTelNo=" + ContactsTable.Rows[0]["X_WCTelNo"].ToString() + ", X_WCEmail=" + ContactsTable.Rows[0]["X_WCEmail"].ToString() + ", X_WCPOBoxNo=" + ContactsTable.Rows[0]["X_WCPOBoxNo"].ToString() + ", X_HCAddress=" + ContactsTable.Rows[0]["X_HCAddress"].ToString() + ", X_HCCity=" + ContactsTable.Rows[0]["X_HCCity"].ToString() + ", X_HCMobileNo=" + ContactsTable.Rows[0]["X_HCMobileNo"].ToString() + ", X_HCTelNo=" + ContactsTable.Rows[0]["X_HCTelNo"].ToString() + ", "
                                                    + "X_HCEmail=" + ContactsTable.Rows[0]["X_HCEmail"].ToString() + ", X_HCPOBoxNo=" + ContactsTable.Rows[0]["X_HCPOBoxNo"].ToString() + ", X_KinContctPerson=" + ContactsTable.Rows[0]["X_KinContctPerson"].ToString() + ", X_KinRelation=" + ContactsTable.Rows[0]["X_KinRelation"].ToString() + ", X_KinContactNo=" + ContactsTable.Rows[0]["X_KinContactNo"].ToString() + ", X_KinTelNo=" + ContactsTable.Rows[0]["X_KinTelNo"].ToString() + ", X_KinEmail=" + ContactsTable.Rows[0]["X_KinEmail"].ToString() + ","
                                                    + " X_KinAddress=" + ContactsTable.Rows[0]["X_KinAddress"].ToString() + ", X_KinPOBoxNo=" + ContactsTable.Rows[0]["X_KinPOBoxNo"].ToString() + ", X_KinCity=" + ContactsTable.Rows[0]["X_KinCity"].ToString() + ", X_KinCountry=" + ContactsTable.Rows[0]["X_KinCountry"].ToString() + ", N_KinCountryID=" + myFunctions.getIntVAL(ContactsTable.Rows[0]["N_KinCountryID"].ToString()) + ", X_RefName=" + ContactsTable.Rows[0]["X_RefName"].ToString() + ", "
                                                    + "X_RefRelation=" + ContactsTable.Rows[0]["X_RefRelation"].ToString() + ", X_RefRelationInfo=" + ContactsTable.Rows[0]["X_RefRelationInfo"].ToString() + ", X_PrevRefName=" + ContactsTable.Rows[0]["X_PrevRefName"].ToString() + ", X_PrevRefJob=" + ContactsTable.Rows[0]["X_PrevRefJob"].ToString() + ", X_PrevRefDepartment=" + ContactsTable.Rows[0]["X_PrevRefDepartment"].ToString() + ", X_PrevRefCompany=" + ContactsTable.Rows[0]["X_PrevRefCompany"].ToString() + ", X_PrevRefContactNo=" + ContactsTable.Rows[0]["X_PrevRefContactNo"].ToString() + ", X_PrevRefEmail=" + ContactsTable.Rows[0]["X_PrevRefEmail"].ToString() + " "
                                                    + " where N_CompanyID=@N_CompanyID and N_EmpID=@nEmpID and N_ContactDetailsID=@N_ContactDetailsID";

                                dLayer.ExecuteNonQuery(ContactQry, QueryParams, connection, transaction);
                            }
                            else
                            {
                                dLayer.DeleteData("Pay_EmployeeSub", "N_EmpID", nEmpID, "", connection, transaction);

                                if (ContactsTable.Columns.Contains("N_ContactDetailsUpdateID"))
                                    ContactsTable.Columns.Remove("N_ContactDetailsUpdateID");
                                if (ContactsTable.Columns.Contains("N_EmpUpdateID"))
                                    ContactsTable.Columns.Remove("N_EmpUpdateID");

                                foreach (DataRow dRow in ContactsTable.Rows)
                                {
                                    dRow["N_EmpID"] = nEmpID;
                                }

                                ContactsTable.AcceptChanges();
                                if (ContactsTable.Rows.Count > 0)
                                {
                                    int nContactsID = dLayer.SaveData("Pay_EmployeeSub", "N_ContactDetailsID", ContactsTable, connection, transaction);
                                    if (nContactsID <= 0)
                                    {
                                        transaction.Rollback();
                                        return Ok(_api.Error(User, "Unable to save"));
                                    }
                                }
                            }

                            //Update Dependencies
                            dLayer.DeleteData("Pay_EmployeeDependence", "N_EmpID", nEmpID, "", connection, transaction);

                            if (DependenceTable.Rows.Count > 0)
                            {
                                if (DependenceTable.Columns.Contains("N_DependenceUpdateID"))
                                    DependenceTable.Columns.Remove("N_DependenceUpdateID");
                                if (DependenceTable.Columns.Contains("N_EmpUpdateID"))
                                    DependenceTable.Columns.Remove("N_EmpUpdateID");

                                foreach (DataRow dRow in DependenceTable.Rows)
                                {
                                    dRow["N_EmpID"] = nEmpID;
                                }

                                DependenceTable.AcceptChanges();

                                int nDependenceID = dLayer.SaveData("Pay_EmployeeDependence", "N_DependenceID", DependenceTable, connection, transaction);
                                if (nDependenceID <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Unable to save"));
                                }
                            }

                            //Update Education
                            dLayer.DeleteData("Pay_EmployeeEducation", "N_EmpID", nEmpID, "", connection, transaction);

                            if (EduTable.Rows.Count > 0)
                            {
                                if (EduTable.Columns.Contains("N_EduUpdateID"))
                                    EduTable.Columns.Remove("N_EduUpdateID");
                                if (EduTable.Columns.Contains("N_EmpUpdateID"))
                                    EduTable.Columns.Remove("N_EmpUpdateID");

                                foreach (DataRow dRow in EduTable.Rows)
                                {
                                    dRow["N_EmpID"] = nEmpID;
                                }

                                EduTable.AcceptChanges();

                                int nEduID = dLayer.SaveData("Pay_EmployeeEducation", "N_EduID", EduTable, connection, transaction);
                                if (nEduID <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Unable to save"));
                                }
                            }

                            //Update History
                            dLayer.DeleteData("Pay_EmploymentHistory", "N_EmpID", nEmpID, "", connection, transaction);

                            if (HistoryTable.Rows.Count > 0)
                            {
                                if (HistoryTable.Columns.Contains("N_JobUpdateID"))
                                    HistoryTable.Columns.Remove("N_JobUpdateID");
                                if (HistoryTable.Columns.Contains("N_EmpUpdateID"))
                                    HistoryTable.Columns.Remove("N_EmpUpdateID");

                                foreach (DataRow dRow in HistoryTable.Rows)
                                {
                                    dRow["N_EmpID"] = nEmpID;
                                }

                                HistoryTable.AcceptChanges();

                                int nEduID = dLayer.SaveData("Pay_EmploymentHistory", "N_JobID", HistoryTable, connection, transaction);
                                if (nEduID <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Unable to save"));
                                }
                            }

                            if (Attachment.Rows.Count > 0)
                            {
                                foreach (DataRow dRow in Attachment.Rows)
                                {
                                    dRow["n_FormID"] = 188;
                                }
                                myAttachments.SaveAttachment(dLayer, Attachment, MasterTable.Rows[0]["x_EmpCode"].ToString(), nEmpID, MasterTable.Rows[0]["x_EmpName"].ToString(), MasterTable.Rows[0]["x_EmpCode"].ToString(), nEmpID, "Employee", User, connection, transaction);
                            }
                        }

                        // myFunctions.SendApprovalMail(N_NextApproverID, FormID, nEmpUpdateID, "EMPLOYEE", X_EmpUpdateCode, dLayer, connection, transaction, User);
                        transaction.Commit();
                        return Ok(_api.Success("Employee update Approved" + "-" + X_EmpUpdateCode));
                    }
                    if (X_EmpUpdateCode == "@Auto")
                    {
                        Params.Add("@nCompanyID", nCompanyID);
                        object objReqCode = dLayer.ExecuteScalar("Select ISNULL(max(isnull(X_EmpUpdateCode,0)),1000)+1 as X_EmpUpdateCode from Pay_EmployeeUpdate where N_CompanyID=@nCompanyID", Params, connection, transaction);
                        if (objReqCode.ToString() == "" || objReqCode.ToString() == null) { X_EmpUpdateCode = "1"; }
                        else
                        {
                            X_EmpUpdateCode = objReqCode.ToString();
                        }
                        MasterTable.Rows[0]["X_EmpUpdateCode"] = X_EmpUpdateCode;
                    }
                    if (nEmpUpdateID > 0)
                    {
                        dLayer.DeleteData("Pay_EmployeeSubUpdate", "N_EmpUpdateID", nEmpUpdateID, "", connection, transaction);
                        dLayer.DeleteData("Pay_EmployeeDependenceUpdate", "N_EmpUpdateID", nEmpUpdateID, "", connection, transaction);
                        dLayer.DeleteData("Pay_EmploymentHistoryUpdate", "N_EmpUpdateID", nEmpUpdateID, "", connection, transaction);
                        dLayer.DeleteData("Pay_EmployeeEducationUpdate", "N_EmpUpdateID", nEmpUpdateID, "", connection, transaction);
                        dLayer.DeleteData("Pay_EmployeeUpdate", "N_EmpUpdateID", nEmpUpdateID, "", connection, transaction);
                    }
                    MasterTable.Rows[0]["N_UserID"] = myFunctions.GetUserID(User);

                    // MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_RequestType", typeof(int), this.FormID);
                    if (MasterTable.Columns.Contains("N_ApprovalLevelID"))
                        MasterTable.Columns.Remove("N_ApprovalLevelID");
                    if (MasterTable.Columns.Contains("N_Procstatus"))
                        MasterTable.Columns.Remove("N_Procstatus");
                    if (MasterTable.Columns.Contains("B_IsSaveDraft"))
                        MasterTable.Columns.Remove("B_IsSaveDraft");


                    string empImage = myFunctions.ContainColumn("i_Employe_Image", MasterTable) ? MasterTable.Rows[0]["i_Employe_Image"].ToString() : "";
                    Byte[] empImageBitmap = new Byte[empImage.Length];
                    empImageBitmap = Convert.FromBase64String(empImage);
                    if (myFunctions.ContainColumn("i_Employe_Image", MasterTable))
                        MasterTable.Columns.Remove("i_Employe_Image");

                    MasterTable.AcceptChanges();

                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);
                    nEmpUpdateID = dLayer.SaveData("Pay_EmployeeUpdate", "N_EmpUpdateID", MasterTable, connection, transaction);
                    if (nEmpUpdateID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }
                    else
                    {
                        if (empImage.Length > 0)
                            dLayer.SaveImage("Pay_EmployeeUpdate", "i_Employe_Image", empImageBitmap, "n_EmpID", nEmpID, connection, transaction);

                        // EmpParams.Add("@nEmpUpdateID", nEmpUpdateID);
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "EMPLOYEE", nEmpUpdateID, X_EmpUpdateCode, 1, objEmpName.ToString(), 0, "", 0, User, dLayer, connection, transaction);
                        object bSaveDraft = dLayer.ExecuteScalar("select isnull(B_IsSaveDraft,0) from Pay_EmployeeUpdate where N_CompanyID=@nCompanyID and N_EmpUpdateID=@nEmpUpdateID", EmpParams, connection, transaction);
                        if (bSaveDraft == null)
                        {
                            N_SaveDraft = 0;
                        }
                        else
                        {
                           // N_SaveDraft = myFunctions.getIntVAL(bSaveDraft.ToString());
                            N_SaveDraft = Convert.ToInt32(bSaveDraft);
                        }
                        if (ContactsTable.Rows.Count > 0)
                        {
                            foreach (DataRow dRow in ContactsTable.Rows)
                            {
                                dRow["N_EmpID"] = nEmpID;
                                dRow["N_EmpUpdateID"] = nEmpUpdateID;
                            }
                            ContactsTable.AcceptChanges();
                            int nContactsUpdateID = dLayer.SaveData("Pay_EmployeeSubUpdate", "N_ContactDetailsUpdateID", ContactsTable, connection, transaction);
                            if (nContactsUpdateID <= 0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, "Unable to save"));
                            }
                        }
                        if (DependenceTable.Rows.Count > 0)
                        {
                            foreach (DataRow dRow in DependenceTable.Rows)
                            {
                                dRow["N_EmpID"] = nEmpID;
                                dRow["N_EmpUpdateID"] = nEmpUpdateID;
                            }
                            DependenceTable.AcceptChanges();
                            int nDependenceUpdateID = dLayer.SaveData("Pay_EmployeeDependenceUpdate", "N_DependenceUpdateID", DependenceTable, connection, transaction);
                            if (nDependenceUpdateID <= 0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, "Unable to save"));
                            }
                        }
                        if (EduTable.Rows.Count > 0)
                        {
                            foreach (DataRow dRow in EduTable.Rows)
                            {
                                dRow["N_EmpID"] = nEmpID;
                                dRow["N_EmpUpdateID"] = nEmpUpdateID;
                            }
                            EduTable.AcceptChanges();
                            int nEduUpdateID = dLayer.SaveData("Pay_EmployeeEducationUpdate", "N_EduUpdateID", EduTable, connection, transaction);
                            if (nEduUpdateID <= 0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, "Unable to save"));
                            }
                        }
                        if (HistoryTable.Rows.Count > 0)
                        {
                            foreach (DataRow dRow in HistoryTable.Rows)
                            {
                                dRow["N_EmpID"] = nEmpID;
                                dRow["N_EmpUpdateID"] = nEmpUpdateID;
                            }
                            HistoryTable.AcceptChanges();
                            int nHistoryUpdateID = dLayer.SaveData("Pay_EmploymentHistoryUpdate", "N_JobUpdateID", HistoryTable, connection, transaction);
                            if (nHistoryUpdateID <= 0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, "Unable to save"));
                            }
                        }

                        // if (Attachment.Rows.Count > 0)
                        // {
                        //     foreach (DataRow dRow in Attachment.Rows)
                        //     {
                        //         dRow["n_FormID"] = 1228;
                        //     }
                        //     myAttachments.SaveAttachment(dLayer, Attachment, X_EmpUpdateCode, nEmpUpdateID, MasterTable.Rows[0]["x_EmpName"].ToString(), MasterTable.Rows[0]["X_EmpCode"].ToString(), nEmpID, "Employee Update", User, connection, transaction);
                        // }

                        if (N_SaveDraft == 0)
                        {
                            SortedList QueryParams = new SortedList();
                            //Update Master
                            DataTable tmpEmployee = dLayer.ExecuteDataTable("Select * From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID  and N_FnYearID=@nFnYearID", EmpParams, connection, transaction);

                            string empImageupdate = myFunctions.ContainColumn("i_Employe_Image", MasterTable) ? MasterTable.Rows[0]["i_Employe_Image"].ToString() : "";
                            Byte[] empImageBitmapUpdate = new Byte[empImageupdate.Length];
                            empImageBitmapUpdate = Convert.FromBase64String(empImageupdate);
                            if (myFunctions.ContainColumn("i_Employe_Image", MasterTable))
                                MasterTable.Columns.Remove("i_Employe_Image");
                            if (myFunctions.ContainColumn("i_Employe_Image", tmpEmployee))
                                tmpEmployee.Columns.Remove("i_Employe_Image");
                            if (myFunctions.ContainColumn("I_Employe_Sign", MasterTable))
                                MasterTable.Columns.Remove("I_Employe_Sign");
                            if (myFunctions.ContainColumn("I_Employe_Sign", tmpEmployee))
                                tmpEmployee.Columns.Remove("I_Employe_Sign");

                            if (tmpEmployee.Rows.Count > 0)
                            {
                                for (int k = 0; k < tmpEmployee.Columns.Count; k++)
                                {
                                    for (int l = 0; l < MasterTable.Columns.Count; l++)
                                    {
                                        if (tmpEmployee.Columns[k].ColumnName.ToString().ToLower() == MasterTable.Columns[l].ColumnName.ToString().ToLower())
                                        {
                                            if (MasterTable.Columns[l].ColumnName.ToString().Contains("n_") && MasterTable.Rows[0][MasterTable.Columns[l].ColumnName].ToString() == "")
                                                MasterTable.Rows[0][MasterTable.Columns[l].ColumnName] = 0;

                                            tmpEmployee.Rows[0][tmpEmployee.Columns[k].ColumnName] = MasterTable.Rows[0][MasterTable.Columns[l].ColumnName].ToString();
                                        }
                                    }
                                }
                            }
                        if (Attachment.Rows.Count > 0)
                        {
                    dLayer.ExecuteNonQuery("update Dms_ScreenAttachments set N_TransID = " + nEmpID + " where N_PartyID=" +nEmpID+ " and N_CompanyID=" + nCompanyID, connection, transaction);
                      
                        }
                            string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID =" + nFnYearID + " and X_EmpCode='" + tmpEmployee.Rows[0]["x_EmpCode"].ToString() + "'";
                            string X_Crieteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID =" + nFnYearID;
                            nEmpID = dLayer.SaveData("pay_Employee", "n_EmpID", DupCriteria, X_Crieteria, tmpEmployee, connection, transaction);
                            if (nEmpID <= 0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, "Unable to save"));
                            }
                            else
                            {
                                if (empImage.Length > 0)
                                dLayer.SaveImage("pay_Employee", "i_Employe_Image", empImageBitmap, "n_EmpID", nEmpID, connection, transaction);
                                    //dLayer.SaveImage("pay_Employee", "i_Employe_Image", empImageBitmapUpdate, "n_EmpID", nEmpID, connection, transaction);

                            }

                            //Update Contacts
                            if (ContactsTable.Rows.Count > 0)
                            {
                                if (myFunctions.getIntVAL(ContactsTable.Rows[0]["N_ContactDetailsID"].ToString()) > 0)
                                {
                                    QueryParams.Add("@N_ContactDetailsID", myFunctions.getIntVAL(ContactsTable.Rows[0]["N_ContactDetailsID"].ToString()));

                                    string ContactQry = "update Pay_EmployeeSub set X_EmergencyContctPerson=" + ContactsTable.Rows[0]["X_EmergencyContctPerson"].ToString() + ", X_EmergencyContctPersonH=" + ContactsTable.Rows[0]["X_EmergencyContctPersonH"].ToString() + ", X_EmergencyRelation=" + ContactsTable.Rows[0]["X_EmergencyRelation"].ToString() + ", X_EmergencyRelationH=" + ContactsTable.Rows[0]["X_EmergencyRelationH"].ToString() + ", X_EmergencyEmail=" + ContactsTable.Rows[0]["X_EmergencyEmail"].ToString() + ", X_EmergencyEmailH=" + ContactsTable.Rows[0]["X_EmergencyEmailH"].ToString() + ","
                                                        + "X_EmergencyAddress=" + ContactsTable.Rows[0]["X_EmergencyAddress"].ToString() + ", X_EmergencyAddressH=" + ContactsTable.Rows[0]["X_EmergencyAddressH"].ToString() + ", X_EmergencyNum=" + ContactsTable.Rows[0]["X_EmergencyNum"].ToString() + ", X_EmergencyNumH=" + ContactsTable.Rows[0]["X_EmergencyNumH"].ToString() + ", X_EmergencyTelNo=" + ContactsTable.Rows[0]["X_EmergencyTelNo"].ToString() + ", X_EmergencyPOBoxNo=" + ContactsTable.Rows[0]["X_EmergencyPOBoxNo"].ToString() + ","
                                                        + " X_EmergencyCity=" + ContactsTable.Rows[0]["X_EmergencyCity"].ToString() + ", X_EmergencyTelNoH=" + ContactsTable.Rows[0]["X_EmergencyTelNoH"].ToString() + ", X_EmergencyPOBoxNoH=" + ContactsTable.Rows[0]["X_EmergencyPOBoxNoH"].ToString() + ", X_EmergencyCityH=" + ContactsTable.Rows[0]["X_EmergencyCityH"].ToString() + ", X_WCAddress=" + ContactsTable.Rows[0]["X_WCAddress"].ToString() + ", X_WCCity=" + ContactsTable.Rows[0]["X_WCCity"].ToString() + ", X_WCMobileNo=" + ContactsTable.Rows[0]["X_WCMobileNo"].ToString() + ","
                                                        + " X_WCTelNo=" + ContactsTable.Rows[0]["X_WCTelNo"].ToString() + ", X_WCEmail=" + ContactsTable.Rows[0]["X_WCEmail"].ToString() + ", X_WCPOBoxNo=" + ContactsTable.Rows[0]["X_WCPOBoxNo"].ToString() + ", X_HCAddress=" + ContactsTable.Rows[0]["X_HCAddress"].ToString() + ", X_HCCity=" + ContactsTable.Rows[0]["X_HCCity"].ToString() + ", X_HCMobileNo=" + ContactsTable.Rows[0]["X_HCMobileNo"].ToString() + ", X_HCTelNo=" + ContactsTable.Rows[0]["X_HCTelNo"].ToString() + ", "
                                                        + "X_HCEmail=" + ContactsTable.Rows[0]["X_HCEmail"].ToString() + ", X_HCPOBoxNo=" + ContactsTable.Rows[0]["X_HCPOBoxNo"].ToString() + ", X_KinContctPerson=" + ContactsTable.Rows[0]["X_KinContctPerson"].ToString() + ", X_KinRelation=" + ContactsTable.Rows[0]["X_KinRelation"].ToString() + ", X_KinContactNo=" + ContactsTable.Rows[0]["X_KinContactNo"].ToString() + ", X_KinTelNo=" + ContactsTable.Rows[0]["X_KinTelNo"].ToString() + ", X_KinEmail=" + ContactsTable.Rows[0]["X_KinEmail"].ToString() + ","
                                                        + " X_KinAddress=" + ContactsTable.Rows[0]["X_KinAddress"].ToString() + ", X_KinPOBoxNo=" + ContactsTable.Rows[0]["X_KinPOBoxNo"].ToString() + ", X_KinCity=" + ContactsTable.Rows[0]["X_KinCity"].ToString() + ", X_KinCountry=" + ContactsTable.Rows[0]["X_KinCountry"].ToString() + ", N_KinCountryID=" + myFunctions.getIntVAL(ContactsTable.Rows[0]["N_KinCountryID"].ToString()) + ", X_RefName=" + ContactsTable.Rows[0]["X_RefName"].ToString() + ", "
                                                        + "X_RefRelation=" + ContactsTable.Rows[0]["X_RefRelation"].ToString() + ", X_RefRelationInfo=" + ContactsTable.Rows[0]["X_RefRelationInfo"].ToString() + ", X_PrevRefName=" + ContactsTable.Rows[0]["X_PrevRefName"].ToString() + ", X_PrevRefJob=" + ContactsTable.Rows[0]["X_PrevRefJob"].ToString() + ", X_PrevRefDepartment=" + ContactsTable.Rows[0]["X_PrevRefDepartment"].ToString() + ", X_PrevRefCompany=" + ContactsTable.Rows[0]["X_PrevRefCompany"].ToString() + ", X_PrevRefContactNo=" + ContactsTable.Rows[0]["X_PrevRefContactNo"].ToString() + ", X_PrevRefEmail=" + ContactsTable.Rows[0]["X_PrevRefEmail"].ToString() + " "
                                                        + " where N_CompanyID=@N_CompanyID and N_EmpID=@nEmpID and N_ContactDetailsID=@N_ContactDetailsID";

                                    dLayer.ExecuteNonQuery(ContactQry, QueryParams, connection, transaction);
                                }
                                else
                                {
                                    if (ContactsTable.Columns.Contains("N_ContactDetailsUpdateID"))
                                        ContactsTable.Columns.Remove("N_ContactDetailsUpdateID");
                                    if (ContactsTable.Columns.Contains("N_EmpUpdateID"))
                                        ContactsTable.Columns.Remove("N_EmpUpdateID");

                                    foreach (DataRow dRow in ContactsTable.Rows)
                                    {
                                        dRow["N_EmpID"] = nEmpID;
                                    }

                                    ContactsTable.AcceptChanges();

                                    int nContactsID = dLayer.SaveData("Pay_EmployeeSub", "N_ContactDetailsID", ContactsTable, connection, transaction);
                                    if (nContactsID <= 0)
                                    {
                                        transaction.Rollback();
                                        return Ok(_api.Error(User, "Unable to save"));
                                    }
                                }
                            }

                            //Update Dependencies
                            dLayer.DeleteData("Pay_EmployeeDependence", "N_EmpID", nEmpID, "", connection, transaction);

                            if (DependenceTable.Rows.Count > 0)
                            {
                                if (DependenceTable.Columns.Contains("N_DependenceUpdateID"))
                                    DependenceTable.Columns.Remove("N_DependenceUpdateID");
                                if (DependenceTable.Columns.Contains("N_EmpUpdateID"))
                                    DependenceTable.Columns.Remove("N_EmpUpdateID");

                                foreach (DataRow dRow in DependenceTable.Rows)
                                {
                                    dRow["N_EmpID"] = nEmpID;
                                }

                                DependenceTable.AcceptChanges();

                                int nDependenceID = dLayer.SaveData("Pay_EmployeeDependence", "N_DependenceID", DependenceTable, connection, transaction);
                                if (nDependenceID <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Unable to save"));
                                }
                            }

                            //Update Education
                            dLayer.DeleteData("Pay_EmployeeEducation", "N_EmpID", nEmpID, "", connection, transaction);

                            if (EduTable.Rows.Count > 0)
                            {
                                if (EduTable.Columns.Contains("N_EduUpdateID"))
                                    EduTable.Columns.Remove("N_EduUpdateID");
                                if (EduTable.Columns.Contains("N_EmpUpdateID"))
                                    EduTable.Columns.Remove("N_EmpUpdateID");

                                foreach (DataRow dRow in EduTable.Rows)
                                {
                                    dRow["N_EmpID"] = nEmpID;
                                }

                                EduTable.AcceptChanges();

                                int nEduID = dLayer.SaveData("Pay_EmployeeEducation", "N_EduID", EduTable, connection, transaction);
                                if (nEduID <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Unable to save"));
                                }
                            }

                            //Update History
                            dLayer.DeleteData("Pay_EmploymentHistory", "N_EmpID", nEmpID, "", connection, transaction);

                            if (HistoryTable.Rows.Count > 0)
                            {
                                if (HistoryTable.Columns.Contains("N_JobUpdateID"))
                                    HistoryTable.Columns.Remove("N_JobUpdateID");
                                if (HistoryTable.Columns.Contains("N_EmpUpdateID"))
                                    HistoryTable.Columns.Remove("N_EmpUpdateID");

                                foreach (DataRow dRow in HistoryTable.Rows)
                                {
                                    dRow["N_EmpID"] = nEmpID;
                                }

                                HistoryTable.AcceptChanges();

                                int nEduID = dLayer.SaveData("Pay_EmploymentHistory", "N_JobID", HistoryTable, connection, transaction);
                                if (nEduID <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error(User, "Unable to save"));
                                }
                            }

                            // if (Attachment.Rows.Count > 0)
                            // {
                            //     foreach (DataRow dRow in Attachment.Rows)
                            //     {
                            //         dRow["n_FormID"] = 188;
                            //     }
                            //     myAttachments.SaveAttachment(dLayer, Attachment, MasterTable.Rows[0]["x_EmpCode"].ToString(), nEmpID, MasterTable.Rows[0]["x_EmpName"].ToString(), MasterTable.Rows[0]["x_EmpCode"].ToString(), nEmpID, "Employee", User, connection, transaction);
                            // }
                        }

                        //myFunctions.SendApprovalMail(N_NextApproverID, FormID, nEmpUpdateID, "EMPLOYEE", X_EmpUpdateCode, dLayer, connection, transaction, User);
                        transaction.Commit();
                    }
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("X_EmpUpdateCode", X_EmpUpdateCode.ToString());
                    return Ok(_api.Success(res, "Employee Update Requested"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("updatedDetails")]
        public ActionResult GetUpdatedDetails(string xEmpUpdateCode, int nFnYearID)
        {
            DataTable Master = new DataTable();
            DataTable Contacts = new DataTable();
            DataTable Dependence = new DataTable();
            DataTable Education = new DataTable();
            DataTable History = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@xEmpUpdateCode", xEmpUpdateCode);
            QueryParams.Add("@nFnYearID", nFnYearID);
            string Condition = "";
            string _sqlQuery = "";
            string Contacts_sqlQuery = "";
            string Dependence_sqlQuery = "";
            string Edu_sqlQuery = "";
            string History_sqlQuery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Condition = "n_Companyid=@nCompanyID and X_EmpUpdateCode =@xEmpUpdateCode";

                    _sqlQuery = "Select * from vw_PayEmployeeUpdate Where " + Condition + "";

                    Master = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                    Master = _api.Format(Master, "pay_Employee");

                    if (Master.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        QueryParams.Add("@nEmpID", Master.Rows[0]["N_EmpID"].ToString());
                        QueryParams.Add("@nEmpUpdateID", Master.Rows[0]["N_EmpUpdateID"].ToString());

                        ds.Tables.Add(Master);

                        Contacts_sqlQuery = "Select * from vw_ContactUpdateDetails where N_CompanyID =@nCompanyID and N_EmpUpdateID=@nEmpUpdateID";
                        Dependence_sqlQuery = "Select * from Pay_EmployeeDependenceUpdate Inner Join Pay_Relation on Pay_EmployeeDependenceUpdate.N_RelationID = Pay_Relation.N_RelationID and Pay_EmployeeDependenceUpdate.N_CompanyID = Pay_Relation.N_CompanyID Where Pay_EmployeeDependenceUpdate.N_CompanyID=@nCompanyID and Pay_EmployeeDependenceUpdate.N_EmpUpdateID=@nEmpUpdateID and Pay_EmployeeDependenceUpdate.N_EmpID=@nEmpID";
                        Edu_sqlQuery = "Select * from Pay_EmployeeEducationUpdate where N_CompanyID =@nCompanyID and N_EmpUpdateID=@nEmpUpdateID";
                        History_sqlQuery = "Select * from Pay_EmploymentHistoryUpdate where N_CompanyID=@nCompanyID and N_EmpUpdateID=@nEmpUpdateID";

                        Contacts = dLayer.ExecuteDataTable(Contacts_sqlQuery, QueryParams, connection);
                        Dependence = dLayer.ExecuteDataTable(Dependence_sqlQuery, QueryParams, connection);
                        Education = dLayer.ExecuteDataTable(Edu_sqlQuery, QueryParams, connection);
                        History = dLayer.ExecuteDataTable(History_sqlQuery, QueryParams, connection);
                        DataTable Attachements = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(Master.Rows[0]["N_EmpID"].ToString()), myFunctions.getIntVAL(Master.Rows[0]["N_EmpUpdateID"].ToString()), 1228, nFnYearID, User, connection);

                        Contacts = _api.Format(Contacts, "pay_EmployeeSub");
                        Dependence = _api.Format(Dependence, "pay_EmployeeDependence");
                        Education = _api.Format(Education, "pay_EmployeeEducation");
                        History = _api.Format(History, "pay_EmploymentHistory");
                        Attachements = _api.Format(Attachements, "attachments");

                        ds.Tables.Add(Contacts);
                        ds.Tables.Add(Dependence);
                        ds.Tables.Add(Education);
                        ds.Tables.Add(History);
                        ds.Tables.Add(Attachements);

                        return Ok(_api.Success(ds));
                    }


                }


            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpDelete("approvalUpdate")]
        public ActionResult DeleteUpdatedData(int nEmpUpdateID, int nFnYearID, string comments)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nTransID", nEmpUpdateID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    ParamList.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,isNull(N_EmpID,0) as N_EmpID,X_EmpUpdateCode from Pay_EmployeeUpdate where N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID and N_EmpUpdateID=@nTransID";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];
                    int EmpID = myFunctions.getIntVAL(TransRow["N_EmpID"].ToString());
                    SortedList EmpParams = new SortedList();
                    EmpParams.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    EmpParams.Add("@nEmpID", EmpID);
                    EmpParams.Add("@nFnYearID", nFnYearID);
                    object objEmpName = dLayer.ExecuteScalar("Select X_EmpName From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", EmpParams, connection);

                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, 1228, nEmpUpdateID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID, myFunctions.getIntVAL(TransRow["N_EmpID"].ToString()), 1228, User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), comments);
                    SqlTransaction transaction = connection.BeginTransaction(); ;

                    string X_Criteria = "N_EmpUpdateID=" + nEmpUpdateID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnYearID;
                    string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());

                    if (ProcStatus != 6 && ProcStatus != 0)
                    {
                        string status = myFunctions.UpdateApprovals(Approvals, nFnYearID, "EMPLOYEE", nEmpUpdateID, TransRow["X_EmpUpdateCode"].ToString(), ProcStatus, "Pay_EmployeeUpdate", X_Criteria, objEmpName.ToString(), User, dLayer, connection, transaction);
                        if (status != "Error")
                        {
                            transaction.Commit();
                            return Ok(_api.Success("Employee Update " + status + " Successfully"));
                        }
                        else
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to delete Employee Update"));
                        }
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to delete Employee Update"));
                    }


                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }


        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {

                DataTable dtMasterTable, dtPay_EmpAddlInfo, dtpay_EmployeeDependence, dtpay_EmployeeAlerts, dtacc_OtherInformation, dtpay_EmpAccruls, dtpay_EmployeePayHistory, dtpay_PaySetup, dtpay_EmployeeSub, dtPay_Employee_Log, dtInv_Salesman, dtVeh_Drivers, dtSch_Teacher, dtPay_EmployeeEducation, dtPay_EmploymentHistory;//,dtSec_User;
                // if(ds.Tables.Contains("pay_Employee"))
                dtMasterTable = ds.Tables["pay_Employee"];
                // if(ds.Tables.Contains("pay_EmpAddlInfo"))
                dtPay_EmpAddlInfo = ds.Tables["pay_EmpAddlInfo"];
                // if(ds.Tables.Contains("pay_EmployeeDependence"))
                dtpay_EmployeeDependence = ds.Tables["pay_EmployeeDependence"];
                // if(ds.Tables.Contains("pay_EmployeeAlerts"))
                dtpay_EmployeeAlerts = ds.Tables["pay_EmployeeAlerts"];
                // if(ds.Tables.Contains("acc_OtherInformation"))
                dtacc_OtherInformation = ds.Tables["acc_OtherInformation"];
                // if(ds.Tables.Contains("pay_EmpAccruls"))
                dtpay_EmpAccruls = ds.Tables["pay_EmpAccruls"];
                // if(ds.Tables.Contains("pay_EmployeePayHistory"))
                dtpay_EmployeePayHistory = ds.Tables["pay_EmployeePayHistory"];
                // if(ds.Tables.Contains("pay_PaySetup"))
                dtpay_PaySetup = ds.Tables["pay_PaySetup"];
                // if(ds.Tables.Contains("pay_EmployeeSub"))
                dtpay_EmployeeSub = ds.Tables["pay_EmployeeSub"];
                dtPay_Employee_Log = ds.Tables["Pay_Employee_Log"];
                dtInv_Salesman = ds.Tables["Inv_Salesman"];
                dtVeh_Drivers = ds.Tables["Veh_Drivers"];
                dtSch_Teacher = ds.Tables["Sch_Teacher"];
                dtPay_EmployeeEducation = ds.Tables["Pay_EmployeeEducation"];
                dtPay_EmploymentHistory = ds.Tables["Pay_EmploymentHistory"];
                //dtSec_User = ds.Tables["Sec_User"];
                DataTable Attachment = ds.Tables["attachments"];


                int nCompanyID = myFunctions.getIntVAL(dtMasterTable.Rows[0]["n_CompanyID"].ToString());
                int nEmpID = myFunctions.getIntVAL(dtMasterTable.Rows[0]["n_EmpID"].ToString());
                int nSavedEmpID = myFunctions.getIntVAL(dtMasterTable.Rows[0]["n_EmpID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(dtMasterTable.Rows[0]["n_FnYearID"].ToString());
                int nDepartmentID = myFunctions.getIntVAL(dtMasterTable.Rows[0]["n_DepartmentID"].ToString());
                string xEmpCode = dtMasterTable.Rows[0]["x_EmpCode"].ToString();
                string xEmpName = dtMasterTable.Rows[0]["x_EmpName"].ToString();
                string xPhone1 = dtMasterTable.Rows[0]["x_Phone1"].ToString();
                string xEmailID = dtMasterTable.Rows[0]["x_EmailID"].ToString();
                int nPositionID = myFunctions.getIntVAL(dtMasterTable.Rows[0]["n_PositionID"].ToString());
                int nEmpTypeID = myFunctions.getIntVAL(dtMasterTable.Rows[0]["n_EmpTypeID"].ToString());
                int nUserID = myFunctions.GetUserID(User);
                // string X_BtnAction = "";
                string xButtonAction="";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    SortedList QueryParams = new SortedList();

                    QueryParams.Add("@nCompanyID", nCompanyID);
                    QueryParams.Add("@nFnYearID", nFnYearID);
                    QueryParams.Add("@nFormID", this.FormID);
                    QueryParams.Add("@nPositionID", myFunctions.getIntVAL(dtMasterTable.Rows[0]["n_PositionID"].ToString()));

                    if (nEmpID == 0 && xEmpCode != "@Auto")
                    {
                        object N_DocNumber = dLayer.ExecuteScalar("Select 1 from pay_Employee Where X_EmpCode ='" + xEmpCode + "' and N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection, transaction);
                        if (N_DocNumber == null)
                        {
                            N_DocNumber = 0;
                        }
                        if (myFunctions.getVAL(N_DocNumber.ToString()) == 1)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Employee Code already exist"));
                        }

                       object N_TitleCount = dLayer.ExecuteScalar("select count(*) from Pay_Position where N_CompanyID= " + nCompanyID + " and N_PositionID= " + nPositionID + " and B_IsSupervisor = 1", connection, transaction);
                        if (myFunctions.getVAL(N_TitleCount.ToString()) > 0)
                        {
                            object N_EmpCount = dLayer.ExecuteScalar("select count(*) from Pay_Employee where N_CompanyID= " + nCompanyID + " and N_PositionID= " + nPositionID + " and N_EmpID <> " + nEmpID, connection, transaction);
                            if (myFunctions.getVAL(N_EmpCount.ToString()) > 0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, "Job title already Exist"));
                            }
                        }
                    }

                    if (xPhone1 != "")
                    {
                        object NPhnCount = dLayer.ExecuteScalar("Select count(1) from pay_Employee Where X_Phone1 ='" + xPhone1 + "' and N_EmpID <> '" + nEmpID + "' and N_Status not in (2,3) and N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection, transaction);
                        if (NPhnCount == null)
                        {
                            NPhnCount = 0;
                        }
                        if (myFunctions.getVAL(NPhnCount.ToString()) >= 1)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Phone Number already exist"));
                        }


                    }
                    if (xEmailID != "")
                    {
                        object NEmailCount = dLayer.ExecuteScalar("Select count(1) from pay_Employee Where X_EmailID ='" + xEmailID + "' and N_EmpID <> '" + nEmpID + "' and N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection, transaction);
                        if (NEmailCount == null)
                        {
                            NEmailCount = 0;
                        }
                        if (myFunctions.getVAL(NEmailCount.ToString()) >= 1)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Email already exist"));
                        }


                    }
                    // Auto Gen
                    xButtonAction="Insert";
                    if (xEmpCode == "@Auto")
                    {

                        object X_Type = dLayer.ExecuteScalar("select X_Description from Pay_EmploymentType where  N_CompanyID = " + nCompanyID + " and n_EmploymentID=" + nEmpTypeID, connection, transaction);

                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("X_Type", X_Type);
                        xEmpCode = dLayer.GetAutoNumber("pay_Employee", "x_EmpCode", Params, connection, transaction);
                        if (xEmpCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Employee Code")); }
                        dtMasterTable.Rows[0]["x_EmpCode"] = xEmpCode;
                        // X_BtnAction = "INSERT";
                    }
                    else if (nEmpID != 0)
                    {
                        xButtonAction="Update"; 
                        xEmpCode = dtMasterTable.Rows[0]["x_EmpCode"].ToString();
                        //dLayer.DeleteData("pay_Employee", "n_EmpID", nEmpID, "", connection, transaction);
                        // X_BtnAction = "UPDATE";

                    }
                    if (dtMasterTable.Rows[0]["N_LedgerID"].ToString() == "0")
                    {
                        object N_LedgerID = dLayer.ExecuteScalar("select N_FieldValue from acc_accountdefaults where X_FieldDescr='Employee Account Ledger' and N_CompanyID = " + nCompanyID + " and n_fnyearid=" + nFnYearID, connection, transaction);
                        if (N_LedgerID != null)
                            dtMasterTable.Rows[0]["N_LedgerID"] = N_LedgerID;
                    }

                    string empImage = myFunctions.ContainColumn("i_Employe_Image", dtMasterTable) ? dtMasterTable.Rows[0]["i_Employe_Image"].ToString() : "";
                    Byte[] empImageBitmap = new Byte[empImage.Length];
                    empImageBitmap = Convert.FromBase64String(empImage);
                    if (myFunctions.ContainColumn("i_Employe_Image", dtMasterTable))
                        dtMasterTable.Columns.Remove("i_Employe_Image");


                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID =" + nFnYearID + " and X_EmpCode='" + xEmpCode.Trim() + "'";
                    string X_Crieteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID =" + nFnYearID;
                    string n_LoanAmountLimit = dtMasterTable.Rows[0]["n_LoanAmountLimit"].ToString();
                    string n_LoanCountLimit = dtMasterTable.Rows[0]["n_LoanCountLimit"].ToString();
                    string n_LoanEligible = dtMasterTable.Rows[0]["n_LoanEligible"].ToString();


                    //RentalProductcreation
                    bool b_RentalProduct = false;
                    if (dtMasterTable.Columns.Contains("b_RentalProduct"))
                    {
                        b_RentalProduct = myFunctions.getBoolVAL(dtMasterTable.Rows[0]["b_RentalProduct"].ToString());

                    }



                    if (myFunctions.ContainColumn("b_RentalProduct", dtMasterTable))
                        dtMasterTable.Columns.Remove("b_RentalProduct");
                    // if (myFunctions.ContainColumn("d_PassportExpiry", dtMasterTable) && dtMasterTable.Rows[0]["d_PassportExpiry"] == DBNull.Value)
                    //     dtMasterTable.Rows[0]["d_PassportExpiry"] = DBNull.Value;
                    nEmpID = dLayer.SaveData("pay_Employee", "n_EmpID", DupCriteria, X_Crieteria, dtMasterTable, connection, transaction);
                    if (nEmpID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }
                    else
                    {

                        if (b_RentalProduct)
                        {
                            SortedList ProductParams = new SortedList();
                            ProductParams.Add("N_CompanyID", nCompanyID);

                            ProductParams.Add("N_FnYearID", nFnYearID);
                            ProductParams.Add("N_TransID", nEmpID);
                            ProductParams.Add("N_LocationID", myFunctions.getIntVAL(dtMasterTable.Rows[0]["N_LocationID"].ToString()));
                            ProductParams.Add("N_ItemTypeID", 8);

                            ProductParams.Add("X_ItemName", xEmpName);
                            dLayer.ExecuteScalarPro("SP_CreateProduct", ProductParams, connection, transaction);


                        }
                        if (empImage.Length > 0)
                            dLayer.SaveImage("pay_Employee", "i_Employe_Image", empImageBitmap, "n_EmpID", nEmpID, connection, transaction);

                        nSavedEmpID = nEmpID;
                        QueryParams.Add("@nSavedEmpID", nEmpID);

                        if (n_LoanAmountLimit == "")
                            dLayer.ExecuteNonQuery("Update pay_Employee Set n_LoanAmountLimit=null Where N_CompanyID =@nCompanyID And N_EmpID =@nSavedEmpID ", QueryParams, connection, transaction);
                        if (n_LoanCountLimit == "")
                            dLayer.ExecuteNonQuery("Update pay_Employee Set n_LoanCountLimit=null Where N_CompanyID =@nCompanyID And N_EmpID =@nSavedEmpID ", QueryParams, connection, transaction);
                        if (n_LoanEligible == "")
                            dLayer.ExecuteNonQuery("Update pay_Employee Set n_LoanEligible=null Where N_CompanyID =@nCompanyID And N_EmpID =@nSavedEmpID ", QueryParams, connection, transaction);


                        //inserting to [Log_ScreenActivity
                        string ipAddress = "";
                        if (Request.Headers.ContainsKey("X-Forwarded-For"))
                            ipAddress = Request.Headers["X-Forwarded-For"];
                        else
                            ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                          myFunctions.LogScreenActivitys(nFnYearID,nEmpID,xEmpCode,this.FormID,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                       
                        // SortedList LogParams = new SortedList();
                        // LogParams.Add("N_CompanyID", nCompanyID);
                        // LogParams.Add("N_FnYearID", nFnYearID);
                        // LogParams.Add("N_TransID", nEmpID);
                        // LogParams.Add("N_FormID", this.FormID);
                        // LogParams.Add("N_UserId", nUserID);
                        // LogParams.Add("X_Action", X_BtnAction);
                        // LogParams.Add("X_SystemName", "ERP Cloud");
                        // LogParams.Add("X_IP", ipAddress);
                        // LogParams.Add("X_TransCode", xEmpCode);
                        // LogParams.Add("X_Remark", " ");
                        // dLayer.ExecuteNonQueryPro("SP_Log_SysActivity", LogParams, connection, transaction);

                        int pay_EmpAddlInfoRes = 0;
                        if (dtPay_EmpAddlInfo.Rows.Count > 0)
                        {
                            dLayer.DeleteData("Pay_EmpAddlInfo", "N_EmpID", nEmpID, "", connection, transaction);
                            foreach (DataRow dRow in dtPay_EmpAddlInfo.Rows)
                            {
                                dRow["N_EmpID"] = nEmpID;
                            }
                            dtPay_EmpAddlInfo.AcceptChanges();
                            pay_EmpAddlInfoRes = dLayer.SaveData("Pay_EmpAddlInfo", "N_InfoID", dtPay_EmpAddlInfo, connection, transaction);
                        }
                        int Pay_EmployeeSubRes = 0;
                        if (dtpay_EmployeeSub.Rows.Count > 0)
                        {
                            dLayer.DeleteData("Pay_EmployeeSub", "N_EmpID", nEmpID, "", connection, transaction);
                            foreach (DataRow dRow in dtpay_EmployeeSub.Rows)
                            {
                                dRow["N_EmpID"] = nEmpID;
                            }
                            dtpay_EmployeeSub.AcceptChanges();
                            Pay_EmployeeSubRes = dLayer.SaveData("Pay_EmployeeSub", "N_ContactDetailsID", dtpay_EmployeeSub, connection, transaction);
                        }
                        int Pay_Employee_LogRes = 0;
                        if (dtPay_Employee_Log.Rows.Count > 0)
                        {
                            foreach (DataRow dRow in dtPay_Employee_Log.Rows)
                            {
                                dRow["N_EmpID"] = nEmpID;
                            }
                            dtPay_Employee_Log.AcceptChanges();
                            Pay_Employee_LogRes = dLayer.SaveData("Pay_Employee_Log", "N_EmployeeLogID", dtPay_Employee_Log, connection, transaction);
                        }
                        dLayer.ExecuteNonQuery("Update Pay_SuperVisor Set N_EmpID = 0 Where N_CompanyID =@nCompanyID And N_EmpID =@nSavedEmpID", QueryParams, connection, transaction);

                        bool B_Inactive = myFunctions.getIntVAL(dtMasterTable.Rows[0]["b_Inactive"].ToString()) == 1 ? true : false;
                        if (B_Inactive)
                            dLayer.ExecuteNonQuery("Update Pay_SuperVisor Set N_EmpID = 0 Where N_CompanyID =@nCompanyID And N_EmpID =@nPositionID", QueryParams, connection, transaction);
                        else
                            dLayer.ExecuteNonQuery("Update Pay_SuperVisor Set N_EmpID = @nSavedEmpID Where N_CompanyID =@nCompanyID And N_PositionID =@nPositionID", QueryParams, connection, transaction);



                        //SAving EMPLOYEE SALARY/BENEFITS
                        int NewEmp = 0, n_NoEdit = 0;
                        object PayEmp = dLayer.ExecuteScalar("select N_EmpID from Pay_PaySetup where N_EmpID=" + nSavedEmpID, connection, transaction);
                        if (PayEmp != null)
                            NewEmp = 1;
                        if (NewEmp == 1)
                        {
                            object Processed = dLayer.ExecuteScalar("Select count(1) from Pay_PaymentDetails Where N_CompanyID = " + nCompanyID + " AND N_EmpID = " + nSavedEmpID, connection, transaction);
                            if (myFunctions.getIntVAL(Processed.ToString()) != 0)
                                n_NoEdit = 1;
                        }
                        if (n_NoEdit != 1)
                        {
                            int pay_PaySetupRes = 0;
                            if (dtpay_PaySetup.Rows.Count > 0)
                            {
                                if (NewEmp == 1)
                                {
                                    dLayer.DeleteData("Pay_PaySetup", "N_EmpID", nSavedEmpID, "N_CompanyID = " + nCompanyID + "", connection, transaction);
                                    dLayer.DeleteData("Pay_EmployeePayHistory", "N_EmpID", nSavedEmpID, "N_CompanyID = " + nCompanyID + "", connection, transaction);
                                }

                                foreach (DataRow dRow in dtpay_PaySetup.Rows)
                                {
                                    dRow["N_EmpID"] = nEmpID;
                                    if (dRow["n_Value"] == DBNull.Value || double.IsNaN(Convert.ToDouble(dRow["n_Value"])))
                                    {
                                      dRow["N_Value"] = 0;
                                    }
                                }
                                dtpay_PaySetup.AcceptChanges();
                                pay_PaySetupRes = dLayer.SaveData("Pay_PaySetup", "N_PaySetupID", dtpay_PaySetup, connection, transaction);
                                if (pay_PaySetupRes > 0)
                                {
                                    int Pay_EmployeePayHistoryRes = 0;
                                    foreach (DataRow dRow in dtpay_EmployeePayHistory.Rows)
                                    {
                                        dRow["N_EmpID"] = nEmpID;
                                        if (dRow["N_Amount"] == DBNull.Value || double.IsNaN(Convert.ToDouble(dRow["N_Amount"])))
                                         {
                                          dRow["N_Amount"] = 0;
                                          }
                                    }
                                    dtpay_EmployeePayHistory.AcceptChanges();
                                    if (dtpay_EmployeePayHistory.Rows.Count > 0)
                                    {
                                        Pay_EmployeePayHistoryRes = dLayer.SaveData("Pay_EmployeePayHistory", "N_PayHistoryID", dtpay_EmployeePayHistory, connection, transaction);
                                    }
                                }
                            }

                        }

                        //Employee Accruals
                        //object AccrualUsed = dLayer.ExecuteScalar("Select count(1) from Pay_VacationDetails Where N_VacDays < 0 and N_CompanyID = " + nCompanyID + " AND N_EmpID = " + nSavedEmpID, connection, transaction);
                        int pay_EmpAccrulsRes = 0;
                        if (dtpay_EmpAccruls.Rows.Count > 0)
                        {
                            dLayer.DeleteData("Pay_EmpAccruls", "N_EmpID", nSavedEmpID, "N_CompanyID = " + nCompanyID + "", connection, transaction);
                            foreach (DataRow dRow in dtpay_EmpAccruls.Rows)
                            {
                                dRow["N_EmpID"] = nEmpID;
                            }
                            dtpay_EmpAccruls.AcceptChanges();
                            pay_EmpAccrulsRes = dLayer.SaveData("Pay_EmpAccruls", "N_EmpAccID", dtpay_EmpAccruls, connection, transaction);
                        }

                        int Acc_OtherInformationRes = 0;
                        // if(myFunctions.getIntVAL(dtacc_OtherInformation.Rows[0]["n_OtherCode"].ToString())>0)
                        // {
                        if (dtacc_OtherInformation.Rows.Count > 0)
                            Acc_OtherInformationRes = dLayer.SaveData("Acc_OtherInformation", "N_OtherDtlsID", dtacc_OtherInformation, connection, transaction);
                        //  }
                        //REMINDER SAVING

                        int Pay_EmployeeAlertsRes = 0;
                        if (dtpay_EmployeeAlerts.Rows.Count > 0)
                            foreach (DataRow dRow in dtpay_EmployeeAlerts.Rows)
                            {
                                dRow["N_EmpID"] = nEmpID;
                            }
                        dtpay_EmployeeAlerts.AcceptChanges();
                        Pay_EmployeeAlertsRes = dLayer.SaveData("Pay_EmployeeAlerts", "N_AlertID", dtpay_EmployeeAlerts, connection, transaction);

                        int Pay_EmployeeDependenceRes = 0;
                        if (dtpay_EmployeeDependence.Rows.Count > 0)
                        {
                            dLayer.DeleteData("Pay_EmployeeDependence", "N_EmpID", nSavedEmpID, "N_CompanyID = " + nCompanyID + "", connection, transaction);
                            foreach (DataRow dRow in dtpay_EmployeeDependence.Rows)
                            {
                                dRow["N_EmpID"] = nEmpID;
                            }
                            dtpay_EmployeeDependence.AcceptChanges();
                            Pay_EmployeeDependenceRes = dLayer.SaveData("Pay_EmployeeDependence", "N_DependenceID", dtpay_EmployeeDependence, connection, transaction);
                            if (Pay_EmployeeDependenceRes > 0)
                            {
                                //SaveFamilyAttachements
                                //DependenceReminderSave
                            }
                        }

                        string xDepartment = "";
                        object objDept = dLayer.ExecuteScalar("Select X_Department from Pay_Department Where N_DepartmentID =" + nDepartmentID + " and N_CompanyID= " + nCompanyID + "and N_FnYearID =" + nFnYearID, connection, transaction);
                        if (objDept != null)
                            xDepartment = objDept.ToString();

                        SortedList ParamsAccount = new SortedList();
                        ParamsAccount.Add("N_CompanyID", nCompanyID);
                        ParamsAccount.Add("N_EmpID", nSavedEmpID);
                        ParamsAccount.Add("X_EmpCode", xEmpCode);
                        ParamsAccount.Add("X_Department", xDepartment);
                        ParamsAccount.Add("X_EmpName", xEmpName);
                        ParamsAccount.Add("N_UserID", nUserID);
                        ParamsAccount.Add("N_FnYearID", nFnYearID);
                        ParamsAccount.Add("X_Form", "Pay_EmployeeMaster");

                        if (myFunctions.getIntVAL(dtMasterTable.Rows[0]["N_LedgerID"].ToString()) == 0)
                            dLayer.ExecuteScalarPro("SP_Pay_CreateEmployeeAccount", ParamsAccount, connection, transaction).ToString();
                        // if (myFunctions.getIntVAL(dtMasterTable.Rows[0]["N_LoanLedgerID"].ToString()) == 0)
                        //     dLayer.ExecuteScalarPro("SP_Pay_CreateEmployeeLoanAccount", ParamsAccount, connection, transaction).ToString();

                        bool B_EnableSalesExec = myFunctions.CheckPermission(nCompanyID, 290, myFunctions.GetUserCategory(User).ToString(), "N_UserCategoryID", dLayer, connection, transaction);
                        if (B_EnableSalesExec)
                        {
                            int Inv_SalesmanRes = 0;
                            if (dtInv_Salesman.Rows.Count > 0)
                                Inv_SalesmanRes = dLayer.SaveData("Inv_Salesman", "N_SalesmanID", dtInv_Salesman, connection, transaction);
                        }

                        bool B_CheckDriver = false;
                        if (B_CheckDriver)
                        {
                            int Veh_DriversRes = 0;
                            if (dtVeh_Drivers.Rows.Count > 0)
                                Veh_DriversRes = dLayer.SaveData("Inv_Salesman", "N_SalesmanID", dtVeh_Drivers, connection, transaction);
                        }
                        bool B_Teacher = myFunctions.CheckPermission(nCompanyID, 155, myFunctions.GetUserCategory(User).ToString(), "N_UserCategoryID", dLayer, connection, transaction);
                        if (B_Teacher)
                        {
                            int Sch_TeacherRes = 0;
                            if (dtSch_Teacher.Rows.Count > 0)
                                Sch_TeacherRes = dLayer.SaveData("Sch_Teacher", "N_TeacherID", dtSch_Teacher, connection, transaction);
                        }

                           if (nEmpID > 0)
                          {
                          dLayer.DeleteData("Pay_EmployeeEducation", "N_EmpID", nEmpID, "N_CompanyID =" + nCompanyID, connection, transaction);
                           }
                        int Pay_EmployeeEducationRes = 0;
                        if (dtPay_EmployeeEducation.Rows.Count > 0)
                            foreach (DataRow dRow in dtPay_EmployeeEducation.Rows)
                            {
                                dRow["N_EmpID"] = nEmpID;
                            }
                        dtPay_EmployeeEducation.AcceptChanges();
                        Pay_EmployeeEducationRes = dLayer.SaveData("Pay_EmployeeEducation", "N_EduID", dtPay_EmployeeEducation, connection, transaction);

                          if (nEmpID > 0)
                          {
                          dLayer.DeleteData("Pay_EmploymentHistory", "N_EmpID", nEmpID, "N_CompanyID =" + nCompanyID, connection, transaction);
                           }
                        int Pay_EmploymentHistoryRes = 0;
                        if (dtPay_EmploymentHistory.Rows.Count > 0)
                            foreach (DataRow dRow in dtPay_EmploymentHistory.Rows)
                            {
                                dRow["N_EmpID"] = nEmpID;
                            }
                        dtPay_EmploymentHistory.AcceptChanges();
                        Pay_EmploymentHistoryRes = dLayer.SaveData("Pay_EmploymentHistory", "N_JobID", dtPay_EmploymentHistory, connection, transaction);

                        // //User Information Save
                        // int n_UserID = 0 ;
                        // int n_UserCategoryID = myFunctions.getIntVAL(dtSec_User.Rows[0]["n_UserCategoryID"].ToString());
                        // int n_LoginFlag = myFunctions.getIntVAL(dtSec_User.Rows[0]["n_LoginFlag"].ToString());
                        // string x_UserCategoryIDList = dtSec_User.Rows[0]["x_UserCategoryIDList"].ToString();
                        // bool b_EnableApplicationLogin = myFunctions.getIntVAL(dtSec_User.Rows[0]["b_EnableApplicationLogin"].ToString()) == 1 ? true : false;
                        // bool b_EnablePortalLogin = myFunctions.getIntVAL(dtSec_User.Rows[0]["b_EnablePortalLogin"].ToString()) == 1 ? true : false;
                        // bool b_UserActive = myFunctions.getIntVAL(dtSec_User.Rows[0]["b_Active"].ToString()) == 1 ? true : false;

                        // if(dtSec_User.Rows.Count > 0){
                        // object objUser = dLayer.ExecuteScalar("Select N_UserID from Sec_User where N_CompanyID=" + nCompanyID + "  and N_EmpID=" + nEmpID, connection, transaction);                        
                        // if (objUser != null)
                        //     n_UserID = myFunctions.getIntVAL(objUser.ToString());
                        // else{
                        //     object objEmpUser = dLayer.ExecuteScalar("Select N_UserID from Sec_User where N_CompanyID=" + nCompanyID + " and X_UserID='" + xEmpCode  + "'  and N_EmpID is null", connection, transaction);                       
                        //     if (objEmpUser != null)
                        //         n_UserID = myFunctions.getIntVAL(objEmpUser.ToString());
                        //     else
                        //         n_UserID = 0;
                        // }
                        // if(n_UserID == 0) {
                        //     n_UserID = dLayer.SaveData("Sec_User", "N_UserID", dtSec_User, connection, transaction);
                        // }
                        // else {
                        //     dLayer.ExecuteNonQuery("update  Sec_User set N_EmpID=" + nEmpID + ",N_UserCategoryID=" + n_UserCategoryID + ",N_LoginFlag=" + n_LoginFlag + ",X_UserCategoryList=" + x_UserCategoryIDList + " where N_UserID=" + n_UserID + " and N_CompanyID= " + nCompanyID, Params, connection, transaction);
                        // }
                        // }
                        // else{
                        //     dLayer.ExecuteNonQuery("update  Sec_User set N_LoginFlag=" + n_LoginFlag + ",B_Active= 0 where N_UserID=" + n_UserID + " and N_CompanyID= " + nCompanyID, Params, connection, transaction);
                        // }
                        string xEmail = dtMasterTable.Rows[0]["X_EmailID"].ToString();
                        if (xEmail != "")
                        {
                            using (SqlConnection olivoCon = new SqlConnection(masterDBConnectionString))
                            {
                                olivoCon.Open();
                                SqlTransaction olivoTxn = olivoCon.BeginTransaction();
                                string Pwd = myFunctions.EncryptString(xEmail);
                                int nClientID = myFunctions.GetClientID(User);
                                object glogalUserID = dLayer.ExecuteScalar("SELECT N_UserID FROM Users where x_EmailID='" + xEmail.ToString() + "' and N_ClientID=" + nClientID, olivoCon, olivoTxn);
                                if (glogalUserID == null)
                                {
                                    DataTable dtGobal = new DataTable();
                                    dtGobal.Clear();
                                    dtGobal.Columns.Add("X_EmailID");
                                    dtGobal.Columns.Add("N_UserID");
                                    dtGobal.Columns.Add("X_UserName");
                                    dtGobal.Columns.Add("N_ClientID");
                                    dtGobal.Columns.Add("N_ActiveAppID");
                                    dtGobal.Columns.Add("X_Password");
                                    dtGobal.Columns.Add("B_Inactive");
                                    dtGobal.Columns.Add("X_UserID");
                                    dtGobal.Columns.Add("B_EmailVerified");
                                    dtGobal.Columns.Add("N_UserType");

                                    DataRow rowGb = dtGobal.NewRow();
                                    rowGb["X_EmailID"] = xEmail;
                                    rowGb["X_UserName"] = xEmail;
                                    rowGb["N_ClientID"] = nClientID;
                                    rowGb["N_ActiveAppID"] = 2;
                                    rowGb["X_Password"] = Pwd;
                                    rowGb["B_Inactive"] = 0;
                                    rowGb["X_UserID"] = xEmail;
                                    rowGb["B_EmailVerified"] = 1;
                                    rowGb["N_UserType"] = 2;
                                    dtGobal.Rows.Add(rowGb);

                                    int GlobalUserID = dLayer.SaveData("Users", "N_UserID", dtGobal, olivoCon, olivoTxn);
                                    if (GlobalUserID > 0)
                                    {
                                        olivoTxn.Commit();
                                    }
                                }
                                object objUserID = dLayer.ExecuteScalar("Select N_UserID from Sec_User where N_CompanyID=" + nCompanyID + "  and N_EmpID=" + nEmpID + " and X_UserID='" + xEmail.ToString() + "'", connection, transaction);
                                if (objUserID == null)
                                {

                                    object objUserCat = dLayer.ExecuteScalar("Select N_UserCategoryID from Sec_UserCategory where N_CompanyID=" + nCompanyID + "  and N_AppID=2", connection, transaction);
                                    if (objUserCat != null)
                                    {
                                        object objUserCheck = dLayer.ExecuteScalar("Select X_UserID from Sec_User where N_CompanyID=" + nCompanyID + "  and X_UserID='" + xEmail.ToString() + "' and N_EmpID=" + nEmpID + " and N_UserCategoryID=" + myFunctions.getIntVAL(objUserCat.ToString()), connection, transaction);
                                        if (objUserCheck == null)
                                        {
                                            object objUserCheckng = dLayer.ExecuteScalar("Select X_UserID from Sec_User where N_CompanyID=" + nCompanyID + " and N_EmpID=" + nEmpID, connection, transaction);
                                            if (objUserCheckng == null)
                                            {
                                                object objUser = dLayer.ExecuteScalar("Select X_UserID from Sec_User where N_CompanyID=" + nCompanyID + "  and X_UserID='" + xEmail.ToString() + "'", connection, transaction);
                                                if (objUser != null)
                                                {
                                                    dLayer.ExecuteNonQuery("update  Sec_User set N_EmpID=" + nEmpID + ",B_Active= 1,N_UserCategoryID=" + myFunctions.getIntVAL(objUserCat.ToString()) + ",X_UserCategoryList=" + objUserCat.ToString() + " where X_UserID='" + xEmail.ToString() + "' and N_CompanyID= " + nCompanyID, Params, connection, transaction);
                                                }
                                                else
                                                {
                                                    DataTable dt = new DataTable();
                                                    dt.Clear();
                                                    dt.Columns.Add("N_CompanyID");
                                                    dt.Columns.Add("N_UserID");
                                                    dt.Columns.Add("X_UserID");
                                                    dt.Columns.Add("X_Password");
                                                    dt.Columns.Add("N_UserCategoryID");
                                                    dt.Columns.Add("B_Active");
                                                    dt.Columns.Add("N_BranchID");
                                                    dt.Columns.Add("N_LocationID");
                                                    dt.Columns.Add("X_UserName");
                                                    dt.Columns.Add("N_EmpID");
                                                    dt.Columns.Add("N_LoginFlag");
                                                    dt.Columns.Add("X_UserCategoryList");
                                                    dt.Columns.Add("X_Email");
                                                    dt.Columns.Add("N_TypeID");

                                                    DataRow row = dt.NewRow();
                                                    row["N_CompanyID"] = nCompanyID;
                                                    row["X_UserID"] = xEmail;
                                                    row["X_Password"] = Pwd;
                                                    row["N_UserCategoryID"] = myFunctions.getIntVAL(objUserCat.ToString());
                                                    row["B_Active"] = 1;
                                                    row["N_BranchID"] = myFunctions.getIntVAL(dtMasterTable.Rows[0]["N_BranchID"].ToString());
                                                    row["N_LocationID"] = myFunctions.getIntVAL(dtMasterTable.Rows[0]["N_LocationID"].ToString());
                                                    row["X_UserName"] = dtMasterTable.Rows[0]["X_EmpName"].ToString();
                                                    row["N_EmpID"] = nEmpID;
                                                    row["N_LoginFlag"] = 2;
                                                    row["X_UserCategoryList"] = objUserCat.ToString();
                                                    row["X_Email"] = xEmail;
                                                    row["N_TypeID"] = 3;
                                                    dt.Rows.Add(row);

                                                    int UserID = dLayer.SaveData("Sec_User", "N_UserID", dt, connection, transaction);

                                                  object SUAUserID = dLayer.ExecuteScalar("SELECT N_UserID FROM Users where x_EmailID='" + xEmail.ToString() + "' and N_ClientID=" + nClientID, olivoCon, olivoTxn);
                                                  DataTable dtUA = new DataTable();
                                                    dtUA.Clear();
                                                    dtUA.Columns.Add("N_CompanyID");
                                                    dtUA.Columns.Add("N_UserID");
                                                    dtUA.Columns.Add("N_AppMappingID");
                                                    dtUA.Columns.Add("N_AppID");
                                                     dtUA.Columns.Add("X_LandingPage");
                                                     dtUA.Columns.Add("N_GlobalUserID");
                                                    DataRow rowUA = dtUA.NewRow();
                                                    rowUA["N_CompanyID"] = nCompanyID;
                                                    rowUA["N_UserID"] = UserID;
                                                    rowUA["N_AppMappingID"] = 0;
                                                    rowUA["N_AppID"] =2;
                                                    rowUA["X_LandingPage"] = null;
                                                     rowUA["N_GlobalUserID"] = SUAUserID;
                                                    dtUA.Rows.Add(rowUA);
                                                    int UAUserID = dLayer.SaveData("sec_userapps", "N_AppMappingID", dtUA, connection, transaction);

                                                    
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //ATTACHMENT SAVING
                        myAttachments.SaveAttachment(dLayer, Attachment, xEmpCode, nEmpID, dtMasterTable.Rows[0]["x_EmpName"].ToString(), xEmpCode, nEmpID, "Employee", User, connection, transaction);


                        transaction.Commit();
                        return Ok(value: _api.Success("Employee Information Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }


        [HttpGet("managerList")]
        public ActionResult GetManagerList(int nFnYearID, string byDeptManager)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            string sqlCommandText = "";
            // if (byDeptManager != null && byDeptManager != "")
            // {
            sqlCommandText = "Select N_CompanyID,N_FnYearID,N_EmpID,Code as [Employee Code],[Employee Name],N_SupervisorID from vw_Supervisor_ReportTo Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID order by Code";
            // }
            // else
            // {
            //     sqlCommandText = "Select N_CompanyID,N_SupervisorID,N_EmpID,Code,N_BranchID,N_FnYearID,[Employee Code],[Employee Name],Description from vw_Supervisor_ReportTo Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID order by [Employee Code]";
            // }
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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("reportingToList")]
        public ActionResult GetReportingToList(int nFnYearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);

                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nFnYearID", nFnYearID);
                    if (nFnYearID == 0)
                    {
                        object nFnYearIDto = dLayer.ExecuteScalar("select Top(1) N_FnYearID from Acc_FnYear where N_CompanyID=" + nCompanyID + " order by D_Start Desc", Params, connection, transaction);
                        nFnYearID = myFunctions.getIntVAL(nFnYearIDto.ToString());
                    }
                    string sqlCommandText = "Select N_CompanyID, N_BranchID, N_FnYearID, N_EmpID, X_EmpCode, X_EmpName, N_Status from vw_ReportingTo Where N_CompanyID=@nCompanyID and N_FnYearID=" + nFnYearID + " and N_Status not in(2,3) order by X_EmpCode";


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection, transaction);

                    dt = _api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success(dt));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("relationList")]
        public ActionResult GetRelationList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "Select  n_RelationID,x_Relation from Pay_Relation Where N_CompanyID=@nCompanyID  order by n_RelationID";
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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("employeeType")]
        public ActionResult GetEmployeeType()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "Select N_EmploymentID,N_TypeId,B_EnableGosi,N_CompanyID,N_Months,X_Description,X_Prefix from Pay_EmploymentType Where N_CompanyID=@nCompanyID  order by N_EmploymentID";
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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("salaryGrade")]
        public ActionResult GetSalaryGrade(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            string sqlCommandText = "Select X_GradeCode,X_Gradename,N_CompanyID,N_GradeID,B_Active,B_Edit,N_SalaryFrom,N_SalaryTo from Pay_SalaryGrade Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and B_Active=1 order by X_Gradename";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [AllowAnonymous]
        [HttpPost("dummy")]
        public ActionResult GetVoucherDummy([FromBody] DataSet ds)
        {
            DataTable master = ds.Tables["master"];
            try
            {
                return Ok(myFunctions.DecryptString(master.Rows[0]["pwd"].ToString()));
            }
            catch (Exception e)
            {
                return StatusCode(403, _api.Error(User, e));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nEmpID, int nFnyearID)
        {
            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);
            string EmployeeSql = " select * from vw_payemployee Where N_CompanyID=@nCompanyID and N_Empid=@nEmpID and N_FnyearID=@nFnYearID";
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnyearID);
            Params.Add("@nEmpID", nEmpID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable Employee = dLayer.ExecuteDataTable(EmployeeSql, Params, connection, transaction);
                    SortedList ParamList = new SortedList();
                    DataTable TransData = new DataTable();
                    ParamList.Add("@nTransID", nEmpID);
                    ParamList.Add("@nFnyearID", nFnyearID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    string xButtonAction="Delete";
                    string X_EmpCode="";
                    string Sql = "select N_EmpID,X_EmpCode from pay_Employee where N_EmpID=@nTransID and N_CompanyID=@nCompanyID ";
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
                       myFunctions.LogScreenActivitys(myFunctions.getIntVAL( nFnyearID.ToString()),nEmpID,TransRow["X_EmpCode"].ToString(),this.FormID,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
             

                    object EmployeeLedger = dLayer.ExecuteScalar("Select 1 from vw_Pay_EmployeeLedger Where  N_CompanyID= @nCompanyID and (LedgerID=" + Employee.Rows[0]["n_ledgerID"] + " OR LedgerID=" + Employee.Rows[0]["n_loanledgerid"] + ")", Params, connection, transaction);
                    // if (EmployeeLedger != null)
                    //     return Ok(_api.Error(User,"Can't delete,it has been used"));

                    // else
                    // {
                    object loanCount = dLayer.ExecuteScalar("Select N_EmpID From pay_loanissue Where N_CompanyID=@nCompanyID and N_Empid=@nEmpID and N_FnyearID=@nFnYearID", Params, connection, transaction);
                    if (loanCount != null)
                    {
                        return Ok(_api.Error(User, "unable to delete the employee"));
                    }

                    object obj = dLayer.ExecuteScalar("Select N_EmpID From vw_PayPendingLoans_List Where N_CompanyID=@nCompanyID and N_Empid=@nEmpID and N_FnyearID=@nFnYearID", Params, connection, transaction);
                    if (obj != null)
                    {
                        return Ok(_api.Error(User, "Unpaid Dues Exists"));
                    }

                    DataTable SalaryLedger = dLayer.ExecuteDataTable("Select Pay_Employee.N_LedgerID,Pay_Employee.X_EmpName,Acc_MastLedger.X_LedgerName From Pay_Employee inner join Acc_MastLedger on Pay_Employee.N_LedgerID=Acc_MastLedger.N_LedgerID Where N_EmpID=@nEmpID and Pay_Employee.N_CompanyID=@nCompanyID and Pay_Employee.N_FnYearID=@nFnYearID", Params, connection, transaction);
                    if (SalaryLedger.Rows.Count > 0)
                    {
                        if (SalaryLedger.Rows[0]["X_EmpName"].ToString() == SalaryLedger.Rows[0]["X_LedgerName"].ToString())
                        {
                            dLayer.DeleteData("Acc_Mastledger", "N_LedgerID", myFunctions.getIntVAL(SalaryLedger.Rows[0]["N_LedgerID"].ToString()), "", connection, transaction);
                        }
                    }
                    DataTable LoanLedger = dLayer.ExecuteDataTable("Select Pay_Employee.N_LoanLedgerID,Pay_Employee.X_EmpName,Acc_MastLedger.X_LedgerName From Pay_Employee inner join Acc_MastLedger on Pay_Employee.N_LoanLedgerID=Acc_MastLedger.N_LedgerID Where N_EmpID=@nEmpID and Pay_Employee.N_CompanyID=@nCompanyID and Pay_Employee.N_FnYearID=@nFnYearID", Params, connection, transaction);
                    if (LoanLedger.Rows.Count > 0)
                    {
                        if (LoanLedger.Rows[0]["X_EmpName"].ToString() + " " + "Loan" == LoanLedger.Rows[0]["X_LedgerName"].ToString())
                        {
                            dLayer.DeleteData("Acc_Mastledger", "N_LedgerID", myFunctions.getIntVAL(LoanLedger.Rows[0]["N_LoanLedgerID"].ToString()), "", connection, transaction);
                        }
                    }
                    //Delete
                    dLayer.DeleteData("Pay_PaySetup", "N_EmpID", nEmpID, "", connection, transaction);
                    dLayer.DeleteData("Pay_EmployeeDependence", "N_EmpID", nEmpID, "", connection, transaction);
                    dLayer.DeleteData("Pay_EmployeeAlerts", "N_EmpID", nEmpID, "", connection, transaction);
                    dLayer.DeleteData("Pay_EmployeePayHistory", "N_EmpID", nEmpID, "", connection, transaction);
                    dLayer.DeleteData("Pay_VacationDetails", "N_EmpID", nEmpID, "", connection, transaction);
                    dLayer.DeleteData("Web_Pay_EmployeeLogin", "N_EmpID", nEmpID, "", connection, transaction);
                    dLayer.DeleteData("Inv_Salesman", "N_EmpRefID", nEmpID, "", connection, transaction);
                    dLayer.DeleteData("Sec_User", "N_UserID", nEmpID, "", connection, transaction);
                    dLayer.DeleteData("Pay_EmployeeAttachments", "N_EmpID", nEmpID, "", connection, transaction);
                    dLayer.DeleteData("Pay_EmpAddlInfo", "N_EmpID", nEmpID, "", connection, transaction);
                    dLayer.DeleteData("Pay_Employee", "N_EmpID", nEmpID, "", connection, transaction);

                    myAttachments.DeleteAttachment(dLayer, 1, nEmpID, nEmpID, nFnyearID, FormID, User, transaction, connection);

                    transaction.Commit();
                }
                return Ok(_api.Success("Employee Deleted"));
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }


  [HttpGet("empShortlist")]
        public ActionResult GetEmployeeList(int nFnYearID, bool bAllBranchData, int nBranchID,string qry,string nEmpID,int loginEmpID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@bAllBranchData", bAllBranchData);
            Params.Add("@nBranchID", nBranchID);
            string sqlCommandText = "";
             string criteria = "";
              int nEmployeeId = 0;
               if(loginEmpID>0)
                    {
                         Params.Add("@nEmpID", loginEmpID);
                         criteria = " and N_EmpID =@nEmpID ";
               
               
                    }
            if (nEmpID != "" && nEmpID != null)
            {
                criteria = " and N_EmpID =@nEmpID ";
                nEmployeeId = myFunctions.getIntVAL(nEmpID.ToString());
                 Params.Add("@nEmpID", nEmpID);
            }
           
             string qryCriteria = "";
            if (qry != "" && qry != null)
            {
                qryCriteria = " and ([Employee Code] like @qry or Name like @qry ) ";
                Params.Add("@qry", "%" + qry + "%");
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (bAllBranchData == true)
                        sqlCommandText = "Select top (20) N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code] as X_EmpCode,Name as X_EmpName,X_Position,X_Department,X_BranchName from vw_PayEmployee_Disp Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and (N_Status = 0 OR N_Status = 1) "+qryCriteria+criteria+" group by N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName";
                    else
                        sqlCommandText = "Select top (20) N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code] as X_EmpCode ,Name as X_EmpName,X_Position,X_Department,X_BranchName from vw_PayEmployee_Disp Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and (N_BranchID=0 or N_BranchID=@nBranchID) and (N_Status = 0 OR N_Status = 1) "+qryCriteria+criteria+" group by N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName";
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
                return Ok(_api.Error(User, e));
            }
        }





    }
}