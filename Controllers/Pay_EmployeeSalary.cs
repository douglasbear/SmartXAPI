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

            string sqlCommandText = "Select * from vw_PayMaster Where N_CompanyID=@p1 and N_PayMethod = 0 and (N_PayTypeID <>11 and N_PayTypeID <>12 ) and N_FnYearID=@p2 and N_PaymentID=5";

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
                return Ok(_api.Error(e));
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

            string sqlCommandText = "Select * from vw_PayMaster Where  N_CompanyID=@p1 and  N_FnYearID=@p2 and (N_PaymentID=6 or N_PaymentID=7 )and N_PaytypeID<>14  and (N_Paymethod=0 or N_Paymethod=3)";

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
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("accrualSettings")]
        public ActionResult GetAccrualSettings(int nFnYearID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1", nCompanyID);

            string sqlCommandText = "select N_vacTypeID,Name,N_Accrued,X_Type,X_Period from vw_PayAccruedCode_List Where N_CompanyID=@p1 and isnull(N_CountryID,0)=1 order by X_Type desc";

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
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetEmpGradeDetails(int nFnYearID, string xGradeCode)
        {
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            string Mastersql="Select * from vw_Pay_SalaryGrade Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and X_GradeCode=@xGradeCode";
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@xGradeCode",xGradeCode);
            
            try{
                using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        MasterTable=dLayer.ExecuteDataTable(Mastersql,Params,connection); 

                        if (MasterTable.Rows.Count == 0)
                        {
                        return Ok(_api.Warning("No Data Found !!"));
                        }

                        MasterTable = _api.Format(MasterTable, "Master");
                        dt.Tables.Add(MasterTable);

                        int N_GradeID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_GradeID"].ToString());

                        string DetailSql = "select * from Pay_SalaryGradeDetails where N_CompanyID=" + nCompanyID + " and N_GradeID=" + N_GradeID;

                        DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dt.Tables.Add(DetailTable);
                    }
                    return Ok(_api.Success(dt));
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
                int  nGradeDetailsID=0;
                

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
                        if (GradeCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Grade Code")); }
                        MasterTable.Rows[0]["x_GradeCode"] = GradeCode;
                    }

                    if(nGradeID>0)
                    {
                         SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType","Employee Salary Grade"},
                                {"N_VoucherID",nGradeID},};
                         try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(ex));
                        }
                    }


                    nGradeID = dLayer.SaveData("Pay_SalaryGrade", "N_GradeID", MasterTable, connection, transaction);
                    
                    if (nGradeID > 0)
                    {
                        double nPayValue = 0;
                        for (int j = 0; j < SalaryTable.Rows.Count; j++)
                        {
                            string SalarySelect = SalaryTable.Rows[j]["b_SalarySelect"].ToString();
                            if (SalarySelect == "True")
                            {
                                nPayValue = myFunctions.getIntVAL(SalaryTable.Rows[j]["n_Value"].ToString());;
                                nGradeDetailsID = dLayer.SaveData("Pay_SalaryGradeDetails", "N_GradeDetailsID", SalaryTable, connection, transaction);
                            }
                        }
                        SalaryTable.Columns.Remove("b_SalarySelect");

                        for (int j = 0; j < BenefitTable.Rows.Count; j++)
                        {
                            string BenefitSelect = BenefitTable.Rows[j]["b_BenefitSelect"].ToString();
                            if (BenefitSelect == "True")
                            {
                                nPayValue = myFunctions.getIntVAL(BenefitTable.Rows[j]["n_Value"].ToString());;
                                nGradeDetailsID = dLayer.SaveData("Pay_SalaryGradeDetails", "N_GradeDetailsID", BenefitTable, connection, transaction);
                            }
                        }
                        BenefitTable.Columns.Remove("b_BenefitSelect");

                        for (int j = 0; j < AccrualTable.Rows.Count; j++)
                        {
                            string AccrualSelect = AccrualTable.Rows[j]["b_AccrualSelect"].ToString();
                            if (AccrualSelect == "True")
                            {
                                nPayValue = myFunctions.getIntVAL(AccrualTable.Rows[j]["n_Accrued"].ToString());;
                                nGradeDetailsID = dLayer.SaveData("Pay_SalaryGradeDetails", "N_GradeDetailsID", AccrualTable, connection, transaction);
                            }
                        }
                        AccrualTable.Columns.Remove("b_AccrualSelect");
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    
                   // dLayer.DeleteData("Pay_SalaryGradeDetails", "N_GradeID", nGradeID, "", connection, transaction);
                    
                    transaction.Commit();
                    return Ok(_api.Success("Employee Grade Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID, int nGradeID)
        {
            int nUserID = myFunctions.GetUserID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList deleteParams = new SortedList()
                            {
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType","Employee Salary Grade"},
                                {"N_VoucherID",nGradeID},
                                {"N_UserID",nUserID},
                                {"X_SystemName","WebRequest"},

                            };
                    dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", deleteParams, connection, transaction);
                    transaction.Commit();
                }
                return Ok(_api.Success("Deleted"));
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
          
    }

}

