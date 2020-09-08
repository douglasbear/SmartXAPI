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
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("SalesFunnel")]
    [ApiController]
    public class SalesFunnel : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public SalesFunnel(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("listDetails")]
        public ActionResult GetCustomerDetails(int nCompanyID, int nFnyearID)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();

            string sqlCommandSalesFunnel = "select X_Name as name,value from vw_SalesFunnel where N_CompanyID=@p1 and N_FnYearID=@p2 order by value desc";
            string sqlCommandProjectCount = "select COUNT(N_ProjectID) as ProjectCount from Inv_CustomerProjects";
            string sqlCommandContractAmt= "select SUM(N_ContractAmt) as ContractAmt from Inv_CustomerProjects";
            string sqlCommandBudgetAmt = "select SUM(N_EstimateCost) as BudgetAmt from Inv_CustomerProjects";
            string sqlCommandProjectDetails = "SELECT  CAST(CONVERT(DATE, Min(Inv_CustomerProjects.D_StartDate)) as varchar(50))  As D_Dateval,  Prj_MainProject.X_MainProjectName as X_ProjectName,SUM(Prj_MainProject.N_ContractAmt) as N_ContractAmt,SUM(N_EstimateCost) as N_EstimateCost FROM Prj_MainProject INNER JOIN Inv_CustomerProjects ON Prj_MainProject.N_MainProjectID = Inv_CustomerProjects.N_MainProjectID AND  Prj_MainProject.N_CompanyID = Inv_CustomerProjects.N_CompanyID group by Prj_MainProject.X_MainProjectName ,Inv_CustomerProjects.D_StartDate order by Inv_CustomerProjects.D_StartDate ";
            string sqlCommandEmployeeCount = "select COUNT(N_EmpID) as EmployeeCount from Pay_Employee where N_FnYearID=@p2";
            string sqlCommandEmployeeDetails = "select COUNT(N_EmpID) as EmployeeCount,X_Country from Pay_Employee where N_FnYearID=@p2 group by X_Country";

            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnyearID);

            DataTable SalesFunnel = new DataTable();
            DataTable ProjectCount = new DataTable();
            DataTable ContractAmt = new DataTable();
            DataTable BudgetAmt = new DataTable();
            DataTable ProjectDetails = new DataTable();
            DataTable EmployeeCount = new DataTable();
            DataTable EmployeeDetails = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SalesFunnel = dLayer.ExecuteDataTable(sqlCommandSalesFunnel, Params, connection);
                    SalesFunnel=api.Format(SalesFunnel,"SalesFunnel");
                    ProjectCount = dLayer.ExecuteDataTable(sqlCommandProjectCount, Params, connection);
                    ProjectCount=api.Format(ProjectCount,"ProjectCount");
                    ContractAmt = dLayer.ExecuteDataTable(sqlCommandContractAmt, Params, connection);
                    ContractAmt=api.Format(ContractAmt,"ContractAmt");
                    BudgetAmt = dLayer.ExecuteDataTable(sqlCommandBudgetAmt, Params, connection);
                    BudgetAmt=api.Format(BudgetAmt,"BudgetAmt");
                    ProjectDetails = dLayer.ExecuteDataTable(sqlCommandProjectDetails, Params, connection);
                    ProjectDetails=api.Format(ProjectDetails,"ProjectDetails");
                    EmployeeCount = dLayer.ExecuteDataTable(sqlCommandEmployeeCount, Params, connection);
                    EmployeeCount=api.Format(EmployeeCount,"EmployeeCount");
                    EmployeeDetails = dLayer.ExecuteDataTable(sqlCommandEmployeeDetails, Params, connection);
                    EmployeeDetails=api.Format(EmployeeDetails,"EmployeeDetails");
                    
                }
                 dt.Tables.Add(SalesFunnel);
                 dt.Tables.Add(ProjectCount);
                 dt.Tables.Add(ContractAmt);
                 dt.Tables.Add(BudgetAmt);
                 dt.Tables.Add(ProjectDetails);
                 dt.Tables.Add(EmployeeCount);
                 dt.Tables.Add(EmployeeDetails);

                 return Ok(api.Success(dt));

            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }
        }
        

        
    }
}