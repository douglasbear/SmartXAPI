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
        //             SortedList QryParams = new SortedList();
        //             int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
        //             int nInvoiceID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_InvoiceID"].ToString());
        //             int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
        //             string X_InvoiceCode = MasterTable.Rows[0]["x_InvoiceCode"].ToString();
        //             int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
        //             int N_MedicalInsID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_MedicalInsID"].ToString());
        //             QryParams.Add("N_CompanyID", nCompanyID);


        //             DocNo = MasterRow["x_PolicyCode"].ToString();
        //             if (X_InvoiceCode == "@Auto")
        //             {
        //                 Params.Add("N_CompanyID", nCompanyID);
        //                 Params.Add("N_FormID", FormID);
        //                 Params.Add("N_YearID", nFnYearID);
        //                 while (true)
        //                 {
        //                     DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
        //                     object N_Result = dLayer.ExecuteScalar("Select 1 from Pay_MedicalInsuranceInvoicing Where X_PolicyCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
        //                     if (N_Result == null)
        //                         break;
        //                 }
        //                 X_InvoiceCode = DocNo;
        //                 if (X_InvoiceCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate")); }
        //                 MasterTable.Rows[0]["x_InvoiceCode"] = X_InvoiceCode;

        //             }
        //             if (nInvoiceID > 0)
        //             {
        //                 dLayer.ExecuteNonQuery("SP_Delete_Trans_With_PurchaseAccounts " + nCompanyID.ToString() + ",'INSURANCE INVOICING'," + nInvoiceID.ToString() + ",0,'',0", connection, transaction);
        //                 //DeleteAmorization
        //             }
        //             string DupCriteria = "";
        //             string X_Criteria = "";

        //             nInvoiceID = dLayer.SaveData("Pay_MedicalInsuranceInvoicing", "N_InvoiceID", DupCriteria, X_Criteria, MasterTable, connection, transaction);
        //             if (nInvoiceID <= 0)
        //             {
        //                 transaction.Rollback();
        //                 return Ok(_api.Error("Unable To Save"));
        //             }
        //             for (int i = DetailTable.Rows.Count - 1; i >= 0; i--)
        //             {
        //                 DataRow mstVar = DetailTable.Rows[i];

        //                 DetailTable.Rows[i]["N_AdditionID"] = nInvoiceID;


        //             }

        //             int nInvoiceDetailsID = dLayer.SaveData("Pay_MedicalInsuranceInvoicingDetails", "N_InvoiceDetailsID", DetailTable, connection, transaction);
        //             if (nInvoiceDetailsID <= 0)
        //             {
        //                 transaction.Rollback();
        //                 return Ok(_api.Error("Unable To Save"));
        //             }
        //             foreach (DataRow var in DetailTable.Rows)
        //             {
        //                 int paycodeID = 0;
        //                 int n_empid = myFunctions.getIntVAL(var["n_EmpID"].ToString());
        //                 object Prj = dLayer.ExecuteScalar("Select N_ProjectID From Pay_Employee Where N_EmpID ='" + n_empid + "' and N_CompanyID=" + nCompanyID, QryParams, connection, transaction);
        //                 if (Prj != null)
        //                 {
        //                     int ProjectID = myFunctions.getIntVAL(Prj.ToString());
        //                     if (ProjectID > 0)
        //                     {
        //                         object Paycode = dLayer.ExecuteScalar("Select N_PayCodeID From Prj_ProjectParameters Where N_ProjectID ='" + ProjectID + "' and N_CompanyID=" + nCompanyID, QryParams, connection, transaction);
        //                         if (Paycode != null)
        //                         {
        //                             paycodeID = myFunctions.getIntVAL(Paycode.ToString());
        //                         }
        //                         else
        //                         {
        //                             object PolicyPaycode = dLayer.ExecuteScalar("Select N_PayCodeID From Pay_Medical_Insurance Where N_MedicalInsID ='" + N_MedicalInsID + "' and N_CompanyID=" + nCompanyID, QryParams, connection, transaction);
        //                             if (PolicyPaycode != null)
        //                             {
        //                                 paycodeID = myFunctions.getIntVAL(PolicyPaycode.ToString());
        //                             }

        //                         }
        //                     }
        //                     else
        //                     {
        //                         object PolicyPaycode = dLayer.ExecuteScalar("Select N_PayCodeID From Pay_Medical_Insurance Where N_MedicalInsID ='" + N_MedicalInsID + "' and N_CompanyID=" + nCompanyID, QryParams, connection, transaction);
        //                         if (PolicyPaycode != null)
        //                         {
        //                             paycodeID = myFunctions.getIntVAL(PolicyPaycode.ToString());
        //                         }
        //                     }
        //                 }
        //                 else
        //                 {
        //                     object PolicyPaycode = dLayer.ExecuteScalar("Select N_PayCodeID From Pay_Medical_Insurance Where N_MedicalInsID ='" + N_MedicalInsID + "' and N_CompanyID=" + nCompanyID, QryParams, connection, transaction);
        //                     if (PolicyPaycode != null)
        //                     {
        //                         paycodeID = myFunctions.getIntVAL(PolicyPaycode.ToString());
        //                     }
        //                 }
        //                 bool B_Amortized = false;
        //                 bool B_isPrePaid = false;
        //                 bool B_isOnetimeInvoice = false;
        //                 bool B_isInvoice = false;
        //                 object Ammortized = dLayer.ExecuteScalar("select B_Amortized from Pay_PayMaster  Where N_PayID =" + paycodeID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, QryParams, connection, transaction);
        //                 object PrePaid = dLayer.ExecuteScalar("select B_isPrePaid from Pay_PayMaster  Where N_PayID =" + paycodeID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, QryParams, connection, transaction);
        //                 object isOnetimeInvoice = dLayer.ExecuteScalar("select B_isOnetimeInvoice from Pay_PayMaster  Where N_PayID =" + paycodeID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, QryParams, connection, transaction);
        //                 object isInvoice = dLayer.ExecuteScalar("select B_isInvoice from Pay_PayMaster  Where N_PayID =" + paycodeID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, QryParams, connection, transaction);

        //                 if (isOnetimeInvoice != null)
        //                     B_isOnetimeInvoice = myFunctions.getBoolVAL(isOnetimeInvoice.ToString());

        //                 if (Ammortized != null)
        //                     B_Amortized = myFunctions.getBoolVAL(Ammortized.ToString());
        //                 if (PrePaid != null)
        //                     B_isPrePaid = myFunctions.getBoolVAL(PrePaid.ToString());

        //                 if (isInvoice != null)
        //                     B_isInvoice = myFunctions.getBoolVAL(isInvoice.ToString());












        //             }





































