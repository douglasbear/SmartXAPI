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
    [Route("medicalInsuranceAddition")]
    [ApiController]
    public class Pay_MedicalInsuranceAddition : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Pay_MedicalInsuranceAddition(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1132;
        }

        [HttpGet("policyList")]
        public ActionResult GetPolicyList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_MedicalInsurance where N_CompanyID=@nCompanyID ";
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
                return Ok(_api.Error(e));
            }
        }
        [HttpGet("policyAgent")]
        public ActionResult GePolicyAgent()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_MedicalInsuranceAddition_Vendor  ";
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
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("employeeRelationList")]
        public ActionResult GetEmployeeRelationList(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from vw_EmployeeRealationMaster where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID  and N_Status not in(2,3)  ";
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
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
                return Ok(_api.Error(e));
            }
        }


        [HttpGet("classList")]
        public ActionResult GetClassList(int nMedicalInsID, int nDependentID, int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nMedicalInsID", nMedicalInsID);
            Params.Add("@nDependentID", nDependentID);
            string sqlCommandText = "";
            if (nDependentID > 0)
            {
                sqlCommandText = "select * from vw_InsuranceAmountCategoryWise where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID  and N_InsuranceID =@nMedicalInsID and  EmpType=172 or EmpType=0  ";

            }
            else
            {
                sqlCommandText = "select * from vw_InsuranceAmountCategoryWise where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID  and N_InsuranceID =@nMedicalInsID and  EmpType=171 or EmpType=0  ";
            }
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
                return Ok(_api.Error(e));
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
                    DataTable MasterTable;
                    DataTable DetailTable;
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nAdditionID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_AdditionID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    string X_PolicyCode = MasterTable.Rows[0]["x_PolicyCode"].ToString();
                    int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                    if (nAdditionID > 0)
                    {
                        dLayer.DeleteData("Pay_MedicalInsuranceAddition", "N_ReceiptId", nAdditionID, "N_CompanyID = " + nCompanyID, connection, transaction);
                    }
                    DocNo = MasterRow["x_PolicyCode"].ToString();
                    if (X_PolicyCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", FormID);
                        Params.Add("N_YearID", nFnYearID);
                        while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Pay_MedicalInsuranceAddition Where X_PolicyCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_PolicyCode = DocNo;
                        if (X_PolicyCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate")); }
                        MasterTable.Rows[0]["x_PolicyCode"] = X_PolicyCode;

                    }
                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and X_PolicyCode='" + X_PolicyCode + "'";
                    string X_Criteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;

                    nAdditionID = dLayer.SaveData("Pay_MedicalInsuranceAddition", "N_AdditionID", DupCriteria, X_Criteria, MasterTable, connection, transaction);
                    if (nAdditionID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    }
                    int nAdditionDetailsID = dLayer.SaveData("Pay_MedicalInsuranceAdditionDetails", "N_AdditionDetailsID", DetailTable, connection, transaction);
                    if (nAdditionDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
        [HttpGet("list")]
        public ActionResult SalaryPayList(int nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            //int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Criteria = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and Receipt No like '%" + xSearchkey + "%'";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_AdditionID asc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_MedicalInsuranceAdditionSearch where N_CompanyID=@nCompanyId " + Searchkey + Criteria + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_MedicalInsuranceAdditionSearch where N_CompanyID=@nCompanyId " + Searchkey + Criteria + " and N_AdditionID not in (select top(" + Count + ") N_AdditionID from vw_MedicalInsuranceAdditionSearch where N_CompanyID=@nCompanyId " + Criteria + xSortBy + " ) " + xSortBy;
            Params.Add("@nCompanyId", nCompanyId);

            SortedList OutPut = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*)x as N_Count  from vw_MedicalInsuranceAdditionSearch where N_CompanyID=@nCompanyId " + Searchkey + Criteria;
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
                return Ok(_api.Error(e));
            }
        }
//          [HttpGet("employeeDetails")]
//         public ActionResult GetEmpDetails(int nEmpID, string xEmployeeCode, string xType, int xDepId ,int nFnYearID)
//         {
//             DataTable dt = new DataTable();
//             SortedList Params = new SortedList();
//             int nCompanyID = myFunctions.GetCompanyID(User);
//             Params.Add("@nCompanyID", nCompanyID);
//             Params.Add("@nFnYearID", nFnYearID);
//             Params.Add("@xType", xType);
//             Params.Add("@nEmpID", nEmpID);
//             Params.Add("@xEmployeeCode", xEmployeeCode);
//             Params.Add("@xDepId", xDepId);
          
//             string sqlCommandText = "";
//             if (xType =="EMP" )
//             {
//                 sqlCommandText = "select * from vw_MedicalInsuranceAdditionEmp where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID  and X_EmpCode =@xEmployeeCode  ";
//                 object InsAmt = dba.ExecuteSclar("Select N_Price From vw_InsuranceAmountCategoryWise Where N_CompanyID = " + myCompanyID._CompanyID + " and N_InsuranceID=" + N_MedicalInsID + " and X_InsuranceClass='" + EmployeeClass + "' and EmpType=171 and N_InsuranceSettingsDetailsID=" + myFunctions.getIntVAL(dsMaster.Tables["EmployeeDetails"].Rows[0]["N_EmpInsClassID"].ToString()) + "", "TEXT", new DataTable());
//                  object InsCost = dba.ExecuteSclar("Select N_Cost From vw_InsuranceAmountCategoryWise Where N_CompanyID = " + myCompanyID._CompanyID + " and N_InsuranceID=" + N_MedicalInsID + " and X_InsuranceClass='" + EmployeeClass + "' and EmpType=171and N_InsuranceSettingsDetailsID=" + myFunctions.getIntVAL(dsMaster.Tables["EmployeeDetails"].Rows[0]["N_EmpInsClassID"].ToString()) + "", "TEXT", new DataTable());
//              object date = dba.ExecuteSclar("select D_EndDate from Pay_Medical_Insurance where N_MedicalInsID = " + N_MedicalInsID + " and N_CompanyID = " + myCompanyID._CompanyID + "", "TEXT", new DataTable());
//             }
//             else if (xType == "DEP")
//             {
//                 sqlCommandText = "select * from vw_EmployeeDependenceDetails where N_CompanyID=@nCompanyID and N_EmpID =@nEmpID and  N_DependenceID==@xDepId  ";
//                 object InsAmt = dba.ExecuteSclar("Select N_Price From vw_InsuranceAmountCategoryWise Where N_CompanyID = " + myCompanyID._CompanyID + " and N_InsuranceID=" + N_MedicalInsID + " and X_InsuranceClass='" + DependentClass + "' and EmpType=172 and N_InsuranceSettingsDetailsID=" + myFunctions.getIntVAL(dsMaster.Tables["EmployeeDetails"].Rows[0]["N_DepInsClassID"].ToString()) + "", "TEXT", new DataTable());
//                  object InsCost = dba.ExecuteSclar("Select N_Cost From vw_InsuranceAmountCategoryWise Where N_CompanyID = " + myCompanyID._CompanyID + " and N_InsuranceID=" + N_MedicalInsID + " and X_InsuranceClass='" + DependentClass + "' and EmpType=172and N_InsuranceSettingsDetailsID=" + myFunctions.getIntVAL(dsMaster.Tables["EmployeeDetails"].Rows[0]["N_DepInsClassID"].ToString()) + "", "TEXT", new DataTable());
//             object date = dba.ExecuteSclar("select D_EndDate from Pay_Medical_Insurance where N_MedicalInsID = " + N_MedicalInsID + " and N_CompanyID = " + myCompanyID._CompanyID + "", "TEXT", new DataTable());
//             }
//            try
//             {
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     connection.Open();
//                     dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
//                 }
//                 if (dt.Rows.Count == 0)
//                 {
//                     return Ok(_api.Notice("No Results Found"));
//                 }
//                 else
//                 {
//                     return Ok(_api.Success(dt));
//                 }
//             }
//             catch (Exception e)
//             {
//                 return Ok(_api.Error(e));
//             }
//         }
// // 






    }
}
