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
    [Route("leaverequest")]
    [ApiController]



    public class Ess_LeaveRequest : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Ess_LeaveRequest(IDataAccessLayer dl, IApiFunctions apiFun, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1229;
        }



        [HttpGet("leaveList")]
        public ActionResult GetEmployeeLeaveRequest(string xReqType)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int nUserID = api.GetUserID(User);
            int nCompanyID = api.GetCompanyID(User);
            QueryParams.Add("@nCompanyID", nCompanyID);
            QueryParams.Add("@nUserID", nUserID);
            string sqlCommandText = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object nEmpID = dLayer.ExecuteScalar("Select N_EmpID From Sec_User where N_UserID=@nUserID and N_CompanyID=@nCompanyID", QueryParams, connection);
                    if (nEmpID != null)
                    {
                        QueryParams.Add("@nEmpID", myFunctions.getIntVAL(nEmpID.ToString()));
                        QueryParams.Add("@xStatus", xReqType);
                        if (xReqType.ToLower() == "all")
                            sqlCommandText = "Select * From vw_PayVacationList where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID order by VacationRequestDate Desc";
                        else
                        if (xReqType.ToLower() == "pending")
                            sqlCommandText = "select * from vw_PayVacationList where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and X_Status not in ('Reject','Approved')  order by VacationRequestDate Desc ";
                        else
                            sqlCommandText = "Select * From vw_PayVacationList where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and X_Status=@xStatus order by VacationRequestDate Desc";

                        dt = dLayer.ExecuteDataTable(sqlCommandText, QueryParams, connection);
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
                return BadRequest(api.Error(e));
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

            int companyid = api.GetCompanyID(User);

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

                        ds.Tables.Add(Master);
                        Condition = "";
                        if (bShowAllBranchData == true)
                            Condition = "n_Companyid=@nCompanyID and N_VacationGroupID =@nVacationGroupID and N_TransType=1 and X_Type='B'";
                        else
                            Condition = "n_Companyid=@nCompanyID and N_VacationGroupID =@nVacationGroupID and N_BranchID=@nBranchID  and N_TransType=1 and X_Type='B'";

                        _sqlQuery = "Select * from vw_PayVacationDetails_Disp Where " + Condition + "";
                        Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        Detail = api.Format(Detail, "details");
                        if (Detail.Rows.Count == 0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }
                        ds.Tables.Add(Detail);

                        return Ok(api.Success(ds));
                    }


                }


            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }
        }


        //List
        [HttpGet("vacationList")]
        public ActionResult GetVacationList(string nEmpID)
        {
            DataTable dt = new DataTable();
            SortedList QueryParams = new SortedList();
            int companyid = api.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@nEmpID", nEmpID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable("Select * from vw_pay_EmpVacation_Alowance where X_Type='B' and N_EmpId=@nEmpID and N_CompanyID=@nCompanyID", QueryParams, connection);

                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                    return Ok(api.Notice("No Results Found"));
                else
                    return Ok(api.Success(dt));

            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }
        }

        //List
        [HttpGet("vacationBenefits")]
        public ActionResult GetVacationBenefits(string nEmpID, int nMainVacationID)
        {
            DataTable dt = new DataTable();
            SortedList QueryParams = new SortedList();
            int companyid = api.GetCompanyID(User);

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
                    if (nMainVacationID > 0)
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
                return BadRequest(api.Error(e));
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
                SortedList Params = new SortedList();
                DataRow MasterRow = MasterTable.Rows[0];
                var x_VacationGroupCode = MasterRow["x_VacationGroupCode"].ToString();
                int n_VacationGroupID = myFunctions.getIntVAL(MasterRow["n_VacationGroupID"].ToString());
                int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyId"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                int nEmpID = myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());
                int nBranchID = myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction(); ;
                    if (x_VacationGroupCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", "210");
                        Params.Add("N_BranchID", nBranchID);
                        x_VacationGroupCode = dLayer.GetAutoNumber("Pay_VacationMaster", "x_VacationGroupCode", Params, connection, transaction);
                        if (x_VacationGroupCode == "") { return Ok(api.Error("Unable to generate leave Request Code")); }
                        MasterTable.Rows[0]["x_VacationGroupCode"] = x_VacationGroupCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Pay_VacationMaster", "n_VacationGroupID", n_VacationGroupID, "", connection, transaction);
                    }
                    MasterTable.AcceptChanges();


                    n_VacationGroupID = dLayer.SaveData("Pay_VacationMaster", "n_VacationGroupID", MasterTable, connection, transaction);
                    if (n_VacationGroupID > 0)
                    {
                        for (int j = 0; j < DetailTable.Rows.Count; j++)
                        {
                            DetailTable.Rows[j]["n_VacationGroupID"] = n_VacationGroupID;
                        }
                        int N_PurchaseOrderDetailId = dLayer.SaveData("Pay_VacationDetails", "n_VacationID", DetailTable, connection, transaction);
                        if (N_PurchaseOrderDetailId > 0)
                        {
                            transaction.Commit();
                        }
                        else
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
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("x_RequestCode", x_VacationGroupCode.ToString());
                    return Ok(api.Success(res, "Leave Request saved"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }


        [HttpDelete()]
        public ActionResult DeleteData(int n_VacationGroupID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Pay_VacationMaster", "n_VacationGroupID", n_VacationGroupID, "", connection, transaction);
                    if (Results > 0)
                    {
                        Results = dLayer.DeleteData("Pay_VacationDetails", "n_VacationGroupID", n_VacationGroupID, "", connection, transaction);
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to delete Leave Request"));
                    }

                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("n_VacationGroupID", n_VacationGroupID.ToString());
                    return Ok(api.Success(res, "Leave Request Deleted Successfully"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }



    }
}