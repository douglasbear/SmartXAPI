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
    [Route("employeesalary")]
    [ApiController]
    public class EmployeeSalary : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID;



        public EmployeeSalary(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 974;
        }

        [HttpGet("salaryDetails")]
        public ActionResult GetSalaryDetails(int nFnYearID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearID);

            string sqlCommandText = "Select * from vw_PayMaster Where N_CompanyID=@p1 and (N_Paymethod=0 or N_Paymethod=3) and (N_PayTypeID <>11 and N_PayTypeID <>12 ) and N_FnYearID=@p2 and N_PaymentID=5 and B_InActive=0";


            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    OutPut.Add("Details", _api.Format(dt));
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
                return Ok(_api.Error(User,e));
            }
        }

        [HttpGet("benefitDetails")]
        public ActionResult GetBenefitDetails(int nFnYearID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearID);

            string sqlCommandText = "Select * from vw_PayMaster Where ( N_CompanyID=@p1 and  N_FnYearID=@p2 and N_PaymentID in (6,7)  and (N_PaytypeID <>14 ) and (N_Paymethod=0 or N_Paymethod=3) or N_PayTypeID = 11 and N_CompanyID=@p1 and  N_FnYearID=@p2) and isnull(B_InActive,0)=0 order by N_PayTypeID";

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    OutPut.Add("Details", _api.Format(dt));
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
                return Ok(_api.Error(User,e));
            }
        }

        [HttpGet("accrualSettings")]
        public ActionResult GetAccrualSettings(int nFnYearID, int nCountryID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nCountryID);

            string sqlCommandText = "select N_vacTypeID,Name,N_Accrued,X_Type,X_Period from vw_PayAccruedCode_List Where N_CompanyID=@p1 and N_CountryID=@p2 and isnull(B_InActive,0)=0 order by X_Type desc";

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    OutPut.Add("Details", _api.Format(dt));
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
                return Ok(_api.Error(User,e));
            }
        }

         [HttpGet("default")]
        public ActionResult GetEmployeeDefault(int nFnYearID, int nCountryID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable  pay_benifits, pay_EmpAccruls, pay_PaySetup;

            SortedList Result = new SortedList();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nCountryID", nCountryID);

            string accrualSql = "select N_vacTypeID,Name,N_Accrued,X_Type,X_Period,B_InActive from vw_PayAccruedCode_List Where N_CompanyID=@nCompanyID and isnull(N_CountryID,0)=@nCountryID and isnull(B_InActive,0)=0 order by X_Type desc";
            string paySetupSql = "Select * from vw_PayMaster Where N_CompanyID=@nCompanyID and (N_Paymethod=0 or N_Paymethod=3) and (N_PayTypeID <>11 and N_PayTypeID <>12 ) and N_FnYearID=@nFnYearID and N_PaymentID=5 and B_InActive=0"; 
            string payBenifitsSql = "Select * from vw_PayMaster Where ( N_CompanyID=@nCompanyID and  N_FnYearID=@nFnYearID and N_PaymentID in (6,7)  and (N_PaytypeID <>14 ) and (N_Paymethod=0 or N_Paymethod=3) or N_PayTypeID = 11 and N_CompanyID=@nCompanyID and  N_FnYearID=@nFnYearID) and isnull(B_InActive,0)=0 order by N_PayTypeID";
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    pay_PaySetup = dLayer.ExecuteDataTable(paySetupSql, Params, connection);
                    pay_EmpAccruls = dLayer.ExecuteDataTable(accrualSql, Params, connection);
                    pay_benifits = dLayer.ExecuteDataTable(payBenifitsSql, Params, connection);
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
                    pay_PaySetup.AcceptChanges();
                    pay_PaySetup = _api.Format(pay_PaySetup);
                    pay_EmpAccruls = _api.Format(pay_EmpAccruls);
                    pay_benifits = _api.Format(pay_benifits);
                    Result.Add("salary", pay_PaySetup);
                    Result.Add("accrual", pay_EmpAccruls);
                    Result.Add("benefit", pay_benifits);

                    return Ok(_api.Success(Result));

                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetEmpGradeDetails(int nFnYearID, int nGradeID)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable MasterTable = new DataTable();
            DataTable SG_Salary, SG_Benefits, SG_Accruals;
            string Mastersql = "Select * from vw_Pay_SalaryGrade Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_GradeID=@nGradeID";
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nGradeID", nGradeID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Data Found !!"));
                    }

                    MasterTable = _api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                    int N_GradeID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_GradeID"].ToString());

                    string SG_SalarySql = "select * from Pay_SalaryGradeDetails where N_CompanyID=" + nCompanyID + " and N_GradeID=" + N_GradeID+" and N_ReferenceID=1";
                    string SG_BenefitsSql = "select * from Pay_SalaryGradeDetails where N_CompanyID=" + nCompanyID + " and N_GradeID=" + N_GradeID+" and N_ReferenceID=2";
                    string SG_AccrualsSql = "select * from Pay_SalaryGradeDetails where N_CompanyID=" + nCompanyID + " and N_GradeID=" + N_GradeID+" and N_ReferenceID=3";

                    SG_Salary = dLayer.ExecuteDataTable(SG_SalarySql, Params, connection);
                    SG_Benefits = dLayer.ExecuteDataTable(SG_BenefitsSql, Params, connection);
                    SG_Accruals = dLayer.ExecuteDataTable(SG_AccrualsSql, Params, connection);

                    SG_Salary = _api.Format(SG_Salary, "pay_Salary");
                    SG_Benefits = _api.Format(SG_Benefits, "pay_Benefits");
                    SG_Accruals = _api.Format(SG_Accruals, "pay_Accruals");

                    dt.Tables.Add(SG_Salary);
                    dt.Tables.Add(SG_Benefits);
                    dt.Tables.Add(SG_Accruals);
                }
                return Ok(_api.Success(dt));
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
                DataTable SalaryTable;
                DataTable BenefitTable;
                DataTable AccrualTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                SalaryTable = ds.Tables["salary"];
                BenefitTable = ds.Tables["benefit"];
                AccrualTable = ds.Tables["accrual"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nGradeID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_GradeID"].ToString());
                int nGradeDetailsID = 0;


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // SortedList QueryParams = new SortedList();

                    // Auto Gen
                    string GradeCode = "";
                    var values = MasterTable.Rows[0]["x_GradeCode"].ToString();

                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("N_GradeID", nGradeID);
                        GradeCode = dLayer.GetAutoNumber("Pay_SalaryGrade", "X_GradeCode", Params, connection, transaction);
                        if (GradeCode == "") { transaction.Rollback(); return Ok(_api.Error(User,"Unable to generate Grade Code")); }
                        MasterTable.Rows[0]["x_GradeCode"] = GradeCode;
                    }

                    if (nGradeID > 0)
                    {
                        SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType","Employee Salary Grade"},
                                {"N_VoucherID",nGradeID}};
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,ex));
                        }
                    }


                    nGradeID = dLayer.SaveData("Pay_SalaryGrade", "N_GradeID", MasterTable, connection, transaction);

                    if (nGradeID > 0)
                    {
                        //Salary
                        DetailTable.Rows.Clear();
                        for (int j = 0; j < SalaryTable.Rows.Count; j++)
                        {
                            DataRow row = DetailTable.NewRow();
                            row["N_CompanyID"] = nCompanyID;
                            row["N_GradeID"] = nGradeID;
                            row["N_PayID"] = myFunctions.getIntVAL(SalaryTable.Rows[j]["n_PayID"].ToString());
                            row["N_PayFactor"] = 0;
                            row["B_StartDate"] = 0;
                            row["B_EndDate"] = 0;
                            row["B_InActive"] = 0;
                            row["X_Method"] = "";
                            row["N_Value"] = myFunctions.getVAL(SalaryTable.Rows[j]["n_Value"].ToString());
                            row["N_LedgerID"] = 0;
                            row["N_ReferenceID"] = 1;
                            // row["X_TicketType"] = 1;
                            // row["N_TicketAmount"] = 1;
                            // row["X_TicketNotes"] = 1;
                            row["N_MinRange"] = myFunctions.getVAL(SalaryTable.Rows[j]["n_MinValue"].ToString());
                            row["N_MaxRange"] = myFunctions.getVAL(SalaryTable.Rows[j]["n_MaxValue"].ToString());
                            DetailTable.Rows.Add(row);
                        }
                        int nSalDetailsID = dLayer.SaveData("Pay_SalaryGradeDetails", "N_GradeDetailsID", DetailTable, connection, transaction);

                        //Benefit
                        DetailTable.Rows.Clear();
                        for (int j = 0; j < BenefitTable.Rows.Count; j++)
                        {
                            DataRow row = DetailTable.NewRow();
                            row["N_CompanyID"] = nCompanyID;
                            row["N_GradeID"] = nGradeID;
                            row["N_PayID"] = myFunctions.getIntVAL(BenefitTable.Rows[j]["n_PayID"].ToString());
                            row["N_PayFactor"] = 0;
                            row["B_StartDate"] = 0;
                            row["B_EndDate"] = 0;
                            row["B_InActive"] = 0;
                            row["X_Method"] = "";
                            row["N_Value"] = myFunctions.getVAL(BenefitTable.Rows[j]["n_Value"].ToString());
                            row["N_LedgerID"] = 0;
                            row["N_ReferenceID"] = 2;
                            DetailTable.Rows.Add(row);
                          
                        }
                        int nBenefitDetailsID = dLayer.SaveData("Pay_SalaryGradeDetails", "N_GradeDetailsID", DetailTable, connection, transaction);

                        //Accrual
                        DetailTable.Rows.Clear();
                        for (int j = 0; j < AccrualTable.Rows.Count; j++)
                        {
                            DataRow row = DetailTable.NewRow();
                            row["N_CompanyID"] = nCompanyID;
                            row["N_GradeID"] = nGradeID;
                            row["N_PayID"] = myFunctions.getIntVAL(AccrualTable.Rows[j]["n_VacTypeID"].ToString());
                            row["N_PayFactor"] = 0;
                            row["B_StartDate"] = 0;
                            row["B_EndDate"] = 0;
                            row["B_InActive"] = 0;
                            row["X_Method"] = "";
                            row["N_Value"] = myFunctions.getVAL(AccrualTable.Rows[j]["n_Accrued"].ToString());
                            row["N_LedgerID"] = 0;
                            row["N_ReferenceID"] = 3;
                            DetailTable.Rows.Add(row);
                        }
                        int nAccrualDetailsID = dLayer.SaveData("Pay_SalaryGradeDetails", "N_GradeDetailsID", DetailTable, connection, transaction);
                           
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }

                    //dLayer.DeleteData("Pay_SalaryGradeDetails", "N_GradeID", nGradeID, "", connection, transaction);

                    transaction.Commit();
                    return Ok(_api.Success("Employee Grade Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID, int nGradeID)
        {
            int nUserID = myFunctions.GetUserID(User);
            int Results=0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType","Employee Salary Grade"},
                                {"N_VoucherID",nGradeID}};
                    try
                    {
                        Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection);
                    }
                    catch (Exception ex)
                    {
                        return Ok(_api.Error(User,ex));
                    }
                    if (Results <= 0)
                    {
                        return Ok(_api.Error(User,"Unable to Delete"));
                    }
                }
                return Ok(_api.Success("Deleted"));
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

    }

}

