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
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("amendment")]
    [ApiController]
    public class Pay_EmployeePayIncrement : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Pay_EmployeePayIncrement(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("list")]
        public ActionResult GetSalaryRevisionList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "select * from VW_SalaryRivisionDisp where N_CompanyID=@nCompanyID";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
        [HttpGet("defaultdetails")]
        public ActionResult GetSalaryRevisionDefaultDetails(int nEmpID, int nFnYearID, DateTime EffectiveDate)
        {
            DataTable dtOtherinfo = new DataTable();
            DataTable dtSalaryHistory = new DataTable();
            DataTable dtAccrual = new DataTable();
            DataTable dtBenefits = new DataTable();

            DataSet DS = new DataSet();
            SortedList Params = new SortedList();
            SortedList dParamList = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);


            string sqlAcrual = "Select *,(Select COUNT(*) from Pay_VacationDetails Where N_CompanyID = vw_Pay_EmployeeAccrul.N_CompanyID AND N_EmpID = vw_Pay_EmployeeAccrul.N_EmpID AND N_VacTypeID = vw_Pay_EmployeeAccrul.N_VacTypeID ) AS N_NoEdit from vw_Pay_EmployeeAccrul Where N_CompanyID=@p1 and N_EmpID=@p3";
            string sqlBenefits = "Select *,(Select COUNT(*) from Pay_PaymentDetails Where N_CompanyID = vw_EmpPayInformationAmendments.N_CompanyID AND N_EmpID = vw_EmpPayInformationAmendments.N_EmpID AND N_PayID = vw_EmpPayInformationAmendments.N_PayID AND N_Value = vw_EmpPayInformationAmendments.N_value ) AS N_NoEdit from vw_EmpPayInformationAmendments  Where N_CompanyID=@p1 and N_EmpID=@p3 and N_FnYearID=@p2 and N_PaymentID in (6,7)";
            string sqlOtherinfo = "select * from vw_SalaryRevision Where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3 order by n_type";


            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearID);
            Params.Add("@p3", nEmpID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dParamList.Add("@N_CompanyID", nCompanyId);
                    dParamList.Add("@N_FnYearID", nFnYearID);
                    dParamList.Add("@N_EmpID", nEmpID);
                    dParamList.Add("@Date", Convert.ToDateTime(EffectiveDate));

                    dtSalaryHistory = dLayer.ExecuteDataTablePro("SP_Pay_SalaryRevisionDisp", dParamList, connection);
                    dtAccrual = dLayer.ExecuteDataTable(sqlAcrual, Params, connection);
                    dtBenefits = dLayer.ExecuteDataTable(sqlBenefits, Params, connection);
                    dtOtherinfo = dLayer.ExecuteDataTable(sqlOtherinfo, Params, connection);

                }
                dtSalaryHistory = api.Format(dtSalaryHistory, "Salaryhistory");
                dtAccrual = api.Format(dtAccrual, "Accrual");
                dtBenefits = api.Format(dtBenefits, "Benefits");
                dtOtherinfo = api.Format(dtOtherinfo, "Otherinfo");

                DS.Tables.Add(dtSalaryHistory);
                DS.Tables.Add(dtAccrual);
                DS.Tables.Add(dtBenefits);
                DS.Tables.Add(dtOtherinfo);

                if (dtOtherinfo.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(DS));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }


        [HttpGet("details")]
        public ActionResult GetSalaryRevisionDetails(int nEmpID, int nFnYearID, int nHistoryID)
        {
            DataTable dtOtherinfo = new DataTable();
            DataTable dtSalaryHistory = new DataTable();
            DataTable dtAccrual = new DataTable();
            DataTable dtBenefits = new DataTable();

            DataSet DS = new DataSet();
            SortedList Params = new SortedList();
            SortedList dParamList = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);


            string sqlAcrual = "Select *,(Select COUNT(*) from Pay_VacationDetails Where N_CompanyID = vw_Pay_EmployeeAccrul.N_CompanyID AND N_EmpID = vw_Pay_EmployeeAccrul.N_EmpID AND N_VacTypeID = vw_Pay_EmployeeAccrul.N_VacTypeID ) AS N_NoEdit from vw_Pay_EmployeeAccrul Where N_CompanyID=@p1 and N_EmpID=@p2";
            string sqlBenefits = "Select *,(Select COUNT(*) from Pay_PaymentDetails Where N_CompanyID = vw_EmpPayInformationAmendments.N_CompanyID AND N_EmpID = vw_EmpPayInformationAmendments.N_EmpID AND N_PayID = vw_EmpPayInformationAmendments.N_PayID AND N_Value = vw_EmpPayInformationAmendments.N_value ) AS N_NoEdit from vw_EmpPayInformationAmendments  Where N_CompanyID=@p1 and N_EmpID=@p2 and N_FnYearID=@p3";
            string sqlSalaryHistory = "select top 1 * from vw_Pay_EmployeeAdditionalInfo Where N_CompanyID=@p1 and N_HistoryID=@p4";


            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nEmpID);
            Params.Add("@p3", nFnYearID);
            Params.Add("@p4", nHistoryID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dParamList.Add("@p1", nCompanyId);
                    dParamList.Add("@p2", nFnYearID);
                    dParamList.Add("@p3", nEmpID);
                    dParamList.Add("@p4", Convert.ToDateTime(dtOtherinfo.Rows[0]["d_SalesDate"].ToString()));

                    dtSalaryHistory = dLayer.ExecuteDataTablePro("SP_Pay_SalaryRevisionDisp", dParamList, connection);
                    dtAccrual = dLayer.ExecuteDataTable(sqlAcrual, Params, connection);
                    dtBenefits = dLayer.ExecuteDataTable(sqlBenefits, Params, connection);
                    dtOtherinfo = dLayer.ExecuteDataTable(sqlSalaryHistory, Params, connection);

                }
                dtSalaryHistory = api.Format(dtSalaryHistory, "SalaryHistory");
                dtAccrual = api.Format(dtAccrual, "Accrual");
                dtBenefits = api.Format(dtBenefits, "Benefits");
                dtOtherinfo = api.Format(dtOtherinfo, "Otherinfo");

                DS.Tables.Add(dtSalaryHistory);
                DS.Tables.Add(dtAccrual);
                DS.Tables.Add(dtBenefits);
                DS.Tables.Add(dtOtherinfo);

                if (dtOtherinfo.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(DS));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();


                    DataTable SalaryHistory = ds.Tables["SalaryHistory"];
                    DataTable Accrual = ds.Tables["Accrual"];
                    DataTable Benefits = ds.Tables["Benefits"];
                    DataTable Otherinfo = ds.Tables["Otherinfo"];
                    DataTable MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    DataRow MasterRow = MasterTable.Rows[0];



                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string x_HistoryNo = MasterRow["x_HistoryNo"].ToString();
                    int N_EmpID = myFunctions.getIntVAL(MasterRow["N_EmpID"].ToString());

                    if (x_HistoryNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", 305);
                        Params.Add("N_BranchID", 1);
                        x_HistoryNo = dLayer.GetAutoNumber("Pay_PayHistoryMaster", "X_HistoryCode", Params, connection, transaction);
                        if (x_HistoryNo == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Invoicec Number");
                        }
                        MasterTable.Rows[0]["X_HistoryCode"] = x_HistoryNo;
                    }
                    string DupCriteria = "";


                    int n_HistoryId = dLayer.SaveData("Pay_PayHistoryMaster", "N_HistoryID", DupCriteria, "", MasterTable, connection, transaction);
                    if (n_HistoryId <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }

                    for (int i = 0; i < SalaryHistory.Rows.Count; i++)
                    {
                        SalaryHistory.Rows[i]["N_HistoryID"] = n_HistoryId;

                    }
                    //Salary & Benefits Save

                    dLayer.SaveData("Pay_PaySetup", "N_PaySetupID", SalaryHistory, connection, transaction);
                    dLayer.SaveData("Pay_EmployeePayHistory", "N_PayHistoryID", Benefits, connection, transaction);

                    // Other Details

                    for (int i = 0; i < Otherinfo.Rows.Count; i++)
                    {

                        if (Otherinfo.Rows[i]["X_Value"].ToString() == "") continue;

                        if (i == 1)
                            dLayer.ExecuteNonQuery("update Pay_Employee set N_PositionID=" + Otherinfo.Rows[i]["X_Value"] + " where N_EmpID =" + N_EmpID + " and  N_CompanyID =" +N_CompanyID + " and N_FnYearID=" + N_FnYearID, connection, transaction);
                        if (i == 2)
                            dLayer.ExecuteNonQuery("update Pay_Employee set N_DepartmentID=" + Otherinfo.Rows[i]["X_Value"] + " where N_EmpID =" + N_EmpID + " and  N_CompanyID =" + N_CompanyID + " and N_FnYearID=" + N_FnYearID, connection, transaction);
                        if (i == 3)
                            dLayer.ExecuteNonQuery("update Pay_Employee set N_ProjectID=" + Otherinfo.Rows[i]["X_Value"] + " where N_EmpID =" + N_EmpID + " and  N_CompanyID =" +  N_CompanyID  + " and N_FnYearID=" + N_FnYearID, connection, transaction);
                        if (i == 4)
                            dLayer.ExecuteNonQuery("update Pay_Employee set N_BranchID=" + Otherinfo.Rows[i]["X_Value"] + " where N_EmpID =" + N_EmpID + " and  N_CompanyID =" +  N_CompanyID  + " and N_FnYearID=" +  N_FnYearID, connection, transaction);
                        if (i == 5)
                            dLayer.ExecuteNonQuery("update Pay_Employee set N_EmpTypeID=" + Otherinfo.Rows[i]["X_Value"] + " where N_EmpID =" + N_EmpID + " and  N_CompanyID =" + N_CompanyID + " and N_FnYearID=" +  N_FnYearID, connection, transaction);
                        if (i == 6)
                            dLayer.ExecuteNonQuery("update pay_employee set N_WorkLocationID='" + Otherinfo.Rows[i]["X_Value"] + "' where N_EmpID =" + N_EmpID + " and  N_CompanyID =" + N_CompanyID, connection, transaction);
                        if (i == 7)
                            dLayer.ExecuteNonQuery("update Pay_Employee set N_InsClassID=" + Otherinfo.Rows[i]["X_Value"] + " where N_EmpID =" + N_EmpID + " and  N_CompanyID =" + N_CompanyID + " and N_FnYearID=" + N_FnYearID, connection, transaction);

                    }
                    dLayer.SaveData("Pay_EmployeeAdditionalInfo", "N_DetailsID", Otherinfo, connection, transaction);
                    //Accrual Save
                     dLayer.SaveData("Pay_EmpAccruls", "N_EmpAccID", Accrual, connection, transaction);

                    transaction.Commit();
                    SortedList Result = new SortedList();

                    return Ok(api.Success(Result, "Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }
    }
}



