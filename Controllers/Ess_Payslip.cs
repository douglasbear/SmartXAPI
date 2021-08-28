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
using System.Linq;
using System.Security.Claims;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("payslip")]
    [ApiController]



    public class Ess_Payslip : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Ess_Payslip(IDataAccessLayer dl, IApiFunctions _api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            api = _api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1235;
        }


        
        [HttpGet("details")]
        public ActionResult GetPayslipDetails(int nFnYearID,int nEmpID,string xPayrun)
        {
            DataSet dt=new DataSet();
                                    
            DataTable EmployeeDetails = new DataTable();
            DataTable Earnings = new DataTable();
            DataTable Deduction = new DataTable();
            DataTable BasicSalary = new DataTable();


            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);



            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string _EmployeDetails = "SELECT N_EmpID, X_EmpCode,X_EmpName, REPLACE(CONVERT(VARCHAR(11),D_DOB,103), ' ', '-') AS  D_DOB , " +
                            " X_Phone1, X_EmailID,Pay_Position.X_Position,X_EmpImageNAme,  " +
                            " Pay_Department.X_Department, X_Sex, REPLACE(CONVERT(VARCHAR(11),D_HireDate,103), ' ', '-') AS  D_HireDate, X_Phone2 " +
                            " X_Phone2, X_NickName, X_AlternateName, X_PassportNo, " +
                            " REPLACE(CONVERT(VARCHAR(11),D_PassportExpiry,103), ' ', '-') AS  D_PassportExpiry, X_IqamaNo, " +
                            " REPLACE(CONVERT(VARCHAR(11),D_IqamaExpiry,103), ' ', '-') AS  D_IqamaExpiry , X_MaritalStatus FROM Pay_Employee  " +
                            " INNER JOIN Pay_Position ON Pay_Employee.N_PositionID = Pay_Position.N_PositionID " +
                            " LEFT OUTER JOIN Pay_Department ON Pay_Employee.N_DepartmentID = Pay_Department.N_DepartmentID and Pay_Employee.N_FnYearID = Pay_Department.N_FnYearID " +
                            " WHERE Pay_Employee.N_EmpID = @nEmpID AND Pay_Employee.N_CompanyID = @nCompanyID and Pay_Employee.N_FnYearID = @nFnYearID";

                    string _Earnings="select X_paycodeDescription,N_Payrate from vw_Pay_EmployeePayments_RPT where N_Type = 0 and N_PayTypeID<>11 and  X_PayrunText =@xPayrun  and N_CompanyID  = @nCompanyID and  N_EmpID =@nEmpID";
                    string _Deduction="select X_paycodeDescription,N_Payrate from vw_Pay_EmployeePayments_RPT where N_Type = 1  and  X_PayrunText =@xPayrun  and N_CompanyID  = @nCompanyID and  N_EmpID =@nEmpID";


                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nFnYearID", nFnYearID);
                    Params.Add("@nEmpID", nEmpID);
                    Params.Add("@xPayrun", xPayrun);

                    EmployeeDetails = dLayer.ExecuteDataTable(_EmployeDetails, Params, connection);
                    EmployeeDetails=api.Format(EmployeeDetails,"EmployeeDetails");
                    Earnings = dLayer.ExecuteDataTable(_Earnings, Params, connection);
                    Earnings=api.Format(Earnings,"Earnings");
                    Deduction = dLayer.ExecuteDataTable(_Deduction, Params, connection);
                    Deduction=api.Format(Deduction,"Deductions");

                    
                }
               
                dt.Tables.Add(EmployeeDetails);
                dt.Tables.Add(Earnings);
                dt.Tables.Add(Deduction);

                return Ok(api.Success(dt));
               

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        //Save....
        [HttpPost("save")]
        public ActionResult SaveTORequest([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                SortedList Params = new SortedList();
                DataRow MasterRow = MasterTable.Rows[0];
                DataTable Approvals;
                Approvals = ds.Tables["approval"];
                DataRow ApprovalRow = Approvals.Rows[0];

                var x_RequestCode = MasterRow["x_RequestCode"].ToString();
                int nRequestID = myFunctions.getIntVAL(MasterRow["n_RequestID"].ToString());
                int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyId"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                int nEmpID = myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList EmpParams = new SortedList();
                    EmpParams.Add("@nCompanyID", nCompanyID);
                    EmpParams.Add("@nEmpID", nEmpID);
                    object objEmpName = dLayer.ExecuteScalar("Select X_EmpName From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID", EmpParams, connection, transaction);

                    if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()))
                    {
                        int N_PkeyID = nRequestID;
                        string X_Criteria = "N_RequestID=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                        myFunctions.UpdateApproverEntry(Approvals, "Pay_EmpBussinessTripRequest", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        myFunctions.LogApprovals(Approvals, nFnYearID, "Travel Order Request", N_PkeyID, x_RequestCode, 1, objEmpName.ToString(), 0, "", User, dLayer, connection, transaction);
                        transaction.Commit();
                        return Ok(api.Success("Travel Order Request Approval updated" + "-" + x_RequestCode));
                    }
                    if (x_RequestCode == "@Auto")
                    {
                        Params.Add("@nCompanyID", nCompanyID);
                        object objReqCode = dLayer.ExecuteScalar("Select max(isnull(N_RequestID,0))+1 as N_RequestID from Pay_EmpBussinessTripRequest where N_CompanyID=@nCompanyID", Params, connection, transaction);
                        if (objReqCode.ToString() == "" || objReqCode.ToString() == null) { x_RequestCode = "1"; }
                        else
                        {
                            x_RequestCode = objReqCode.ToString();
                        }
                        MasterTable.Rows[0]["x_RequestCode"] = x_RequestCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Pay_EmpBussinessTripRequest", "n_RequestID", nRequestID, "", connection, transaction);
                    }
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_RequestType", typeof(int), this.FormID);
                    MasterTable.AcceptChanges();

                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);
                    nRequestID = dLayer.SaveData("Pay_EmpBussinessTripRequest", "n_RequestID", MasterTable, connection, transaction);
                    if (nRequestID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        myFunctions.LogApprovals(Approvals, nFnYearID, "Travel Order Request", nRequestID, x_RequestCode, 1, objEmpName.ToString(), 0, "", User, dLayer, connection, transaction);

                        transaction.Commit();
                    }
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("x_RequestCode", x_RequestCode.ToString());
                    return Ok(api.Success(res, "Travel Order request saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }
    }
}