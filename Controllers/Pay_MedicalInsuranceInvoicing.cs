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
namespace SmartxAPI.Controllers
{

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("medicalInsuranceInvoicing")]
    [ApiController]
    public class Pay_MedicalInsuranceInvoicing : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Pay_MedicalInsuranceInvoicing(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1132;
        }
        [HttpGet("additionList")]
        public ActionResult GetPolicyList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_MedicalInsuranceAdditionSearch where N_CompanyID=" + nCompanyID + " ";
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

        [HttpGet("employeeDetails")]
        public ActionResult GetEmpDetails(int nAdditionCode, int nFnYearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    SortedList EmpParams = new SortedList();

                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int N_AdditionID = 0;

                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nFnYearID", nFnYearID);


                    EmpParams.Add("@nCompanyID", nCompanyID);
                    EmpParams.Add("@nFnYearID", nFnYearID);

                    DataTable AdditionTable = new DataTable();
                    DataTable AdditionDetailTable = new DataTable();
                    string EmployeeSql = "";
                    string AdditionSql = "";

                    EmployeeSql = " Select * From Pay_MedicalInsuranceAddition Where N_CompanyID=" + nCompanyID + " and X_PolicyCode='" + nAdditionCode + "'";
                    AdditionTable = dLayer.ExecuteDataTable(EmployeeSql, EmpParams, connection);
                    if (AdditionTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    N_AdditionID = myFunctions.getIntVAL(AdditionTable.Rows[0]["N_AdditionId"].ToString());
                    AdditionTable = myFunctions.AddNewColumnToDataTable(AdditionTable, "X_ProjectName", typeof(string), "");
                    if ((AdditionTable.Rows[0]["N_ProjectID"].ToString()) != "")
                    {
                        object X_ProjectName = dLayer.ExecuteScalar("Select X_ProjectName From vw_ProjectAndCompany Where N_CompanyID=" + nCompanyID + " and N_ProjectID=" + myFunctions.getIntVAL(AdditionTable.Rows[0]["N_ProjectID"].ToString()), EmpParams, connection);
                        AdditionTable.Rows[0]["X_ProjectName"] = (X_ProjectName.ToString());
                    }

                    AdditionSql = "Select * From vw_MedicalInsuranceAddition_Invoicing Where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + "and N_AdditionId=" + N_AdditionID + " Order by N_EmpID ";
                    AdditionDetailTable = dLayer.ExecuteDataTable(AdditionSql, EmpParams, connection);
                    if (AdditionDetailTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }


                    dt.Tables.Add(AdditionTable);
                    dt.Tables.Add(AdditionDetailTable);

                    return Ok(_api.Success(dt));
                }
            }

            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
    }
}

        // [HttpPost("save")]
        // public ActionResult SaveData([FromBody] DataSet ds)
        // {
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();
        //             DataTable MasterTable;
        //             DataTable DetailTable;
        //             string DocNo = "";
        //             MasterTable = ds.Tables["master"];
        //             DetailTable = ds.Tables["details"];
        //             DataRow MasterRow = MasterTable.Rows[0];
        //             SortedList Params = new SortedList();
        //             int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
        //             int nInvoiceID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_InvoiceID"].ToString());
        //             int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
        //             string X_InvoiceCode = MasterTable.Rows[0]["x_InvoiceCode"].ToString();
        //             int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                   
                
        //             DocNo = MasterRow["x_PolicyCode"].ToString();
        //             if (X_InvoiceCode == "@Auto")
        //             {
        //                 Params.Add("N_CompanyID", nCompanyID);
        //                 Params.Add("N_FormID", FormID);
        //                 Params.Add("N_YearID", nFnYearID);
        //                 while (true)
        //                 {
        //                     DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
        //                     object N_Result = dLayer.ExecuteScalar("Select 1 from Pay_MedicalInsuranceAddition Where X_PolicyCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
        //                     if (N_Result == null)
        //                         break;
        //                 }
        //                 X_InvoiceCode = DocNo;
        //                 if (X_InvoiceCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate")); }
        //                 MasterTable.Rows[0]["x_InvoiceCode"] = X_InvoiceCode;

        //             }
        //                 if (nAdditionID > 0)
        //             {
        //                 dLayer.DeleteData("Pay_MedicalInsuranceAddition", "N_ReceiptId", nAdditionID, "N_CompanyID = " + nCompanyID, connection, transaction);
        //             }




























