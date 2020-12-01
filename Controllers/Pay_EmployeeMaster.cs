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
        private readonly int FormID;


        public Pay_EmployeeMaster(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 0;
        }

        [HttpGet("list")]
        public ActionResult GetEmployeeList(int? nCompanyID, int nFnYearID, bool bAllBranchData, int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@bAllBranchData", bAllBranchData);
            Params.Add("@nBranchID", nBranchID);
            string sqlCommandText = "";
            if (bAllBranchData == true)
                sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName from vw_PayEmployee_Disp Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID  group by N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName";
            else
                sqlCommandText = "Select N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName from vw_PayEmployee_Disp Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and (N_BranchID=0 or N_BranchID=@nBranchID)  group by N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName";
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
                return Ok(_api.Error(e));
            }
        }


        [HttpGet("dashboardList")]
        public ActionResult GetEmployeeDashboardList(int nFnYearID, bool bAllBranchData, int nBranchID, int EmpStatus, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandCount = "";
            string sqlCommandText ="";
            string Searchkey = "";
            string Criteria = " where N_CompanyID =@nCompanyID and N_FnYearID =@nFnYearID ";

            if (bAllBranchData == false)
            {
                Criteria = Criteria + " and N_BranchID=@nBranchID ";
                Params.Add("@nBranchID", nBranchID);
            }

            if (EmpStatus == 0)
                Criteria = Criteria + " and N_Status<>3 and N_Status<>2 ";
            else if (EmpStatus == 1)
                Criteria = Criteria + " and N_Status =3 or N_Status =2 ";



            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and X_EmployeeCode like '%" + xSearchkey + "%' or X_EmployeeName like '%" + xSearchkey + "%' ";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_EmployeeCode desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_FnYearID,N_Branchid,B_Inactive,N_EmpID,N_Status,N_EmpTypeID,X_EmployeeCode,X_EmployeeName,X_Position,X_Department,X_BranchName,D_HireDate,X_TypeName,X_Nationality,X_IqamaNo,X_Sex,X_PhoneNo,N_TicketCount from vw_PayEmployee_Dashboard " + Criteria + Searchkey + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_CompanyID,N_FnYearID,N_Branchid,B_Inactive,N_EmpID,N_Status,N_EmpTypeID,X_EmployeeCode,X_EmployeeName,X_Position,X_Department,X_BranchName,D_HireDate,X_TypeName,X_Nationality,X_IqamaNo,X_Sex,X_PhoneNo,N_TicketCount from vw_PayEmployee_Dashboard " + Criteria + Searchkey + " and N_EmpID not in (select top(" + Count + ") N_EmpID from vw_PayEmployee_Dashboard "+ Criteria + Searchkey + xSortBy + " ) " + xSortBy;

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from vw_PayEmployee_Dashboard where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID "+Searchkey;
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

        [HttpGet("managerList")]
        public ActionResult GetManagerList(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            string sqlCommandText = "Select N_CompanyID,N_SupervisorID,N_EmpID,Code,N_BranchID,N_FnYearID,[Employee Code],[Employee Name],Description from vw_Supervisor_ReportTo Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID order by [Employee Code]";
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
                return Ok(_api.Error(e));
            }
        }


        [HttpGet("employeeType")]
        public ActionResult GetEmployeeType()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "Select N_EmploymentID,N_TypeId,B_EnableGosi,N_CompanyID,N_Months,X_Description from Pay_EmploymentType Where N_CompanyID=@nCompanyID  order by X_Description";
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
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("salaryGrade")]
        public ActionResult GetSalaryGrade()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "Select X_GradeCode,X_Gradename,N_CompanyID,N_GradeID,B_Active,B_Edit from Pay_SalaryGrade Where N_CompanyID=@nCompanyID   and B_Active=1 order by X_Gradename";
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
                return Ok(_api.Error(e));
            }
        }

         [HttpGet("dummy")]
        public ActionResult GetVoucherDummy(int? id)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(connectionString))
                {
                    Con.Open();
                    string sqlCommandText = "select * from Pay_Employee where N_EmpID=@p1";
                    SortedList mParamList = new SortedList() { { "@p1", id } };
                    DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList, Con);
                    masterTable = _api.Format(masterTable, "Pay_Employee");

                    string sqlCommandText2 = "select * from Pay_EmpAddlInfo where N_EmpID=@p1";
                    SortedList dParamList = new SortedList() { { "@p1", id } };
                    DataTable detailTable = dLayer.ExecuteDataTable(sqlCommandText2, dParamList, Con);
                    detailTable = _api.Format(detailTable, "Pay_EmpAddlInfo");

                    // string sqlCommandText3 = "select * from Inv_SaleAmountDetails where N_SalesId=@p1";
                    // DataTable dtAmountDetails = dLayer.ExecuteDataTable(sqlCommandText3, dParamList, Con);
                    // dtAmountDetails = _api.Format(dtAmountDetails, "saleamountdetails");

                    //if (detailTable.Rows.Count == 0) { return Ok(new { }); }
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add(masterTable);
                    dataSet.Tables.Add(detailTable);
                    //dataSet.Tables.Add(dtAmountDetails);

                    return Ok(dataSet);

                }
            }
            catch (Exception e)
            {
                return StatusCode(403, _api.Error(e));
            }
        }


    }
}