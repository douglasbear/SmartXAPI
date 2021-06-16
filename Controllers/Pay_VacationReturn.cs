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
    [Route("vacationReturn")]
    [ApiController]
    public class Pay_VacationReturn : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Pay_VacationReturn(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 463;
        }

        [HttpGet("vacationList")]
        public ActionResult GetVacationList(int nBranchID, bool bAllBranchData, int nEmpID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nBranchID", nBranchID);
            Params.Add("@nEmpID", nEmpID);
            string strBranch = (bAllBranchData == false) ? " and vw_PayVacationMaster_Disp.N_BranchID=@nBranchID " : "";
            string sqlCommandText = "SELECT    vw_PayVacationEmployee.[Employee Code] AS X_EmpCode, vw_PayVacationEmployee.Name AS X_Emp_Name, " +
            "vw_PayVacationEmployee.N_VacationGroupID, vw_PayVacationEmployee.X_VacationGroupCode, vw_PayVacationEmployee.X_VacType, " +
            "vw_PayVacationEmployee.N_EmpID, vw_PayVacationEmployee.N_Status " +
"FROM         vw_PayVacationEmployee RIGHT OUTER JOIN " +
                      "vw_PayVacationMaster_Disp ON vw_PayVacationEmployee.N_EmpID = vw_PayVacationMaster_Disp.N_EmpID AND " +
                      "vw_PayVacationEmployee.X_VacationGroupCode = vw_PayVacationMaster_Disp.X_VacationGroupCode AND " +
                      "vw_PayVacationEmployee.N_CompanyID = vw_PayVacationMaster_Disp.N_CompanyID AND " +
                      "vw_PayVacationEmployee.N_VacationGroupID = vw_PayVacationMaster_Disp.N_VacationGroupID " +
"WHERE     (vw_PayVacationEmployee.N_VacationStatus = 0) and vw_PayVacationMaster_Disp.N_CompanyID=@nCompanyID And vw_PayVacationMaster_Disp.N_Transtype =1 and vw_PayVacationMaster_Disp.N_EmpID=@nEmpID and vw_PayVacationMaster_Disp.B_IsSaveDraft=0 " + strBranch;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt=myFunctions.AddNewColumnToDataTable(dt,"D_VacDateFrom",typeof(DateTime),null);
                    dt=myFunctions.AddNewColumnToDataTable(dt,"D_VacDateTo",typeof(DateTime),null);
                    dt=myFunctions.AddNewColumnToDataTable(dt,"D_ReturnDate",typeof(DateTime),null);
                    foreach (DataRow var in dt.Rows)
                    {
                        DataTable VacDate = dLayer.ExecuteDataTable("Select Min(D_VacDateFrom) As FromDate ,Max(D_VacDateTo) as ToDate from Pay_VacationDetails Where N_VacationGroupID ="+var["N_VacationGroupID"].ToString()+"", connection);
                   if(VacDate.Rows.Count>0){
                    var["D_VacDateFrom"] = Convert.ToDateTime(VacDate.Rows[0]["FromDate"].ToString());
                    var["D_VacDateTo"] =Convert.ToDateTime( VacDate.Rows[0]["ToDate"].ToString());
                    var["X_VacType"] = var["X_VacType"].ToString() + " [" +Convert.ToDateTime(VacDate.Rows[0]["FromDate"].ToString()).ToString("dd-MMM-yyyy") +" To "+ Convert.ToDateTime( VacDate.Rows[0]["ToDate"].ToString()).ToString("dd-MMM-yyyy") +"]";
                    var["D_ReturnDate"] = Convert.ToDateTime(VacDate.Rows[0]["ToDate"].ToString());}
                    }
                    dt.AcceptChanges();
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
                return Ok(_api.Error(e));
            }
        }



        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    DataRow MasterRow = MasterTable.Rows[0];
                    transaction = connection.BeginTransaction();


                    int N_VacationReturnID = myFunctions.getIntVAL(MasterRow["n_VacationReturnID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int N_BranchID = myFunctions.getIntVAL(MasterRow["n_BranchID"].ToString());

                    QueryParams.Add("@nCompanyID", N_CompanyID);
                    QueryParams.Add("@nFnYearID", N_FnYearID);
                    QueryParams.Add("@nVacationReturnID", N_VacationReturnID);


                    // Auto Gen
                    string X_VacationReturnCode = MasterTable.Rows[0]["X_VacationReturnCode"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    if (X_VacationReturnCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", Master["n_CompanyId"].ToString());
                        Params.Add("N_YearID", Master["n_FnYearId"].ToString());
                        Params.Add("N_FormID", this.FormID);
                        X_VacationReturnCode = dLayer.GetAutoNumber("Pay_VacationReturn", "X_VacationReturnCode", Params, connection, transaction);
                        if (X_VacationReturnCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Quotation Number")); }
                        MasterTable.Rows[0]["X_VacationReturnCode"] = X_VacationReturnCode;

                    }


                    N_VacationReturnID = dLayer.SaveData("Pay_VacationReturn", "n_VacationReturnID", MasterTable, connection, transaction);
                    if (N_VacationReturnID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Vacation Return"));
                    }


                    int N_QuotationDetailId = dLayer.SaveData("Pay_VacationDetails", "N_VacationID", DetailTable, connection, transaction);
                    if (N_QuotationDetailId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Vacation Return"));
                    }

                    SortedList Result = new SortedList();
                    Result.Add("N_VacationReturnID", N_VacationReturnID);
                    Result.Add("X_VacationReturnCode", X_VacationReturnCode);
                    return Ok(_api.Success(Result, "Vacation Return saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }




    }
}