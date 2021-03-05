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
    [Route("myTeam")]
    [ApiController]
    public class Ess_MyTeam : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Ess_MyTeam(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("listold")]
        public ActionResult GetEmployeeListOld(int nFnYearID, bool bAllBranchData, int nBranchID,int nEmpID,int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@bAllBranchData", bAllBranchData);
            Params.Add("@nEmpID", nEmpID);
            
            string sqlCommandCount = "";
            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
            string Searchkey = "";
            string Criteria = " Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_ReportToID=@nEmpID ";
            string groupBy = "  group by N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName,X_Nationality,X_EmailID,X_ProjectName ";
            if (bAllBranchData == false)
            {
                Criteria = Criteria + " and N_BranchID=@nBranchID ";
                Params.Add("@nBranchID", nBranchID);
            }

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and ( Name like '%" + xSearchkey + "%' or [Employee Code] like '%" + xSearchkey + "%' )";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by [Employee Code] desc";
            else
                xSortBy = " order by " + xSortBy;

            if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +")  N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName,X_Nationality,X_EmailID,X_ProjectName from vw_PayEmployee_Disp " + Criteria + Searchkey + groupBy + xSortBy;
            else
                sqlCommandText = "select top("+ nSizeperpage +")  N_CompanyID,N_EmpID,N_BranchID,N_FnYearID,[Employee Code],Name,X_Position,X_Department,X_BranchName,X_Nationality,X_EmailID,X_ProjectName from vw_PayEmployee_Disp " + Criteria + Searchkey + " and N_EmpID not in (select top("+ Count +") N_EmpID from vw_PayEmployee_Disp " + Criteria + Searchkey + groupBy + xSortBy + " ) "  + groupBy + xSortBy;
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from vw_PayEmployee_Disp " + Criteria + Searchkey + groupBy;
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

[HttpGet("list")]
        public ActionResult GetEmployeeList(int nFnYearID, bool bAllBranchData, int nBranchID,int nEmpID,int nPage,int nSizeperpage, string xSearchkey, string xSortBy,string listType)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@bAllBranchData", bAllBranchData);
            Params.Add("@nEmpID", nEmpID);
            
            string sqlCommandCount = "";
            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
            string Searchkey = "";
           
            // string Criteria = " Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_SupervisorID=@nEmpID and N_EmpID<>@nEmpID "; //and N_EmpID<>@nEmpID and (N_ManagerID=@nEmpID or N_SupervisorID=@nEmpID)
            string Criteria = " Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID ";

         if(listType=="myDepartment")
            {
                Criteria = Criteria + " and N_ManagerID=@nEmpID";
            }
            else if(listType=="myTeam")
            {
            Criteria = Criteria + " and N_SupervisorID=@nEmpID and N_EmpID<>@nEmpID ";
              
            }

           if (bAllBranchData == false)
            {
                Criteria = Criteria + " and N_BranchID=@nBranchID ";
                Params.Add("@nBranchID", nBranchID);
            }


            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and ( X_EmpName like '%" + xSearchkey + "%' or X_EmpCode like '%" + xSearchkey + "%' or X_Sex like '%" + xSearchkey + "%' or X_Position like '%" + xSearchkey + "%' or X_Department like '%" + xSearchkey + "%' or D_HireDate like '%" + xSearchkey + "%' or X_Nationality like '%" + xSearchkey + "%' or X_EmailID like '%" + xSearchkey + "%' or X_IqamaNo like '%" + xSearchkey + "%' or D_IqamaExpiry like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_EmpCode desc";
            else if(xSortBy.Contains("d_HireDate"))
                xSortBy =" order by cast(D_HireDate as DateTime) " + xSortBy.Split(" ")[1];
            else
                xSortBy = " order by " + xSortBy;

            if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +") * from Vw_Web_MyTeamList " + Criteria + Searchkey + xSortBy;
            else
                sqlCommandText = "select top("+ nSizeperpage +") * from Vw_Web_MyTeamList " + Criteria + Searchkey + " and N_EmpID not in (select top("+ Count +") N_EmpID from Vw_Web_MyTeamList " + Criteria + Searchkey + xSortBy + " ) "  + xSortBy;
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from Vw_Web_MyTeamList " + Criteria + Searchkey ;
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

       

    }
}