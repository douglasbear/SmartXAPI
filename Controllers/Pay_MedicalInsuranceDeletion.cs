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
    [Route("medicalInsuranceDeletion")]
    [ApiController]
    public class Pay_MedicalInsuranceDeletion : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Pay_MedicalInsuranceDeletion(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1137;
        }

        [HttpGet("list")]
        public ActionResult MedInsuranceDelList(int nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Criteria = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and X_DeletionCode like '%" + xSearchkey + "%'";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_DeletionID asc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Pay_MedicalInsuranceDeletion where N_CompanyId=@nCompanyId " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Pay_MedicalInsuranceDeletion where N_CompanyId=@nCompanyId " + Searchkey + " " + " and N_DeletionID not in (select top(" + Count + ") N_DeletionID from vw_Pay_MedicalInsuranceDeletion where N_CompanyId=@nCompanyId " + xSearchkey + xSortBy + " ) " + xSortBy;
            Params.Add("@nCompanyId", nCompanyId);

            SortedList OutPut = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*)x as N_Count  from vw_Pay_MedicalInsuranceDeletion where N_CompanyId=@nCompanyId " + Searchkey + "";
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
        [HttpGet("loadEmployee")]
        public ActionResult GetEmployeeRelationList(int nFnYearID, int nCompanyID, int nMedicalInsID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nCompanyID", nCompanyID);


            string sqlCommandText = "select * from vw_MedicalInsDeletionEmployee where N_CompanyID=@nCompanyID and N_InsuranceID= " + nMedicalInsID + " ";

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
        public ActionResult GetEmpDetails(int nEmpID, string xEmployeeCode, string xType, int xDepId, int nFnYearID, int N_MedicalInsID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    SortedList EmpParams = new SortedList();
                    SortedList DepParams = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nFnYearID", nFnYearID);
                    Params.Add("@xType", xType);
                    Params.Add("@nEmpID", nEmpID);
                    Params.Add("@xEmployeeCode", xEmployeeCode);
                    Params.Add("@xDepId", xDepId);

                    EmpParams.Add("@nCompanyID", nCompanyID);
                    EmpParams.Add("@nFnYearID", nFnYearID);

                    DepParams.Add("@nCompanyID", nCompanyID);


                    DataTable EmployeeTable = new DataTable();
                    DataTable FamilyTable = new DataTable();

                    string EmployeeSql = "";
                    string FamilySql = "";

                    if (xType == "EMP")
                    {
                        FamilyTable = _api.Format(FamilyTable, "FamilyTable");

                        EmployeeSql = "select * from vw_MedicalInsuranceAdditionEmp where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID  and X_EmpCode ='" + xEmployeeCode + "'";
                        EmployeeTable = dLayer.ExecuteDataTable(EmployeeSql, EmpParams, connection);
                        if (EmployeeTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }

                        EmployeeTable = myFunctions.AddNewColumnToDataTable(EmployeeTable, "N_Price", typeof(double), 0);
                        EmployeeTable = myFunctions.AddNewColumnToDataTable(EmployeeTable, "N_Cost", typeof(double), 0);
                        EmployeeTable = myFunctions.AddNewColumnToDataTable(EmployeeTable, "N_ActPrice", typeof(double), 0);
                        EmployeeTable = myFunctions.AddNewColumnToDataTable(EmployeeTable, "N_ActCost", typeof(double), 0);
                        foreach (DataRow dvar in EmployeeTable.Rows)
                        {
                            object nPrice = dLayer.ExecuteScalar("Select N_Price From vw_MedicalInsDeletionEmployee Where N_CompanyID =@nCompanyID  and N_EmpID=" + nEmpID + " and N_DependenceID = 0 order by D_AdditionDate Desc ", EmpParams, connection);
                            object nCost = dLayer.ExecuteScalar("Select N_Cost From  vw_MedicalInsDeletionEmployee Where N_CompanyID =@nCompanyID  and N_EmpID=" + nEmpID + " and N_DependenceID = 0 order by D_AdditionDate Desc ", EmpParams, connection);
                            object nActPrice = dLayer.ExecuteScalar("select N_ActPrice from  vw_MedicalInsDeletionEmployee Where N_CompanyID =@nCompanyID  and N_EmpID=" + nEmpID + " and N_DependenceID = 0 order by D_AdditionDate Desc ", EmpParams, connection);
                            object nActCost = dLayer.ExecuteScalar("select N_ActCost from  vw_MedicalInsDeletionEmployee Where N_CompanyID =@nCompanyID  and N_EmpID=" + nEmpID + " and N_DependenceID = 0 order by D_AdditionDate Desc ", EmpParams, connection);

                            if (nPrice != null)
                            {
                                dvar["N_Price"] = myFunctions.getVAL(nPrice.ToString());
                            }
                            if (nCost != null)
                            {
                                dvar["N_Cost"] = myFunctions.getVAL(nCost.ToString());
                            }
                            if (nActPrice != null)
                            {
                                dvar["N_ActPrice"] = myFunctions.getDateVAL(Convert.ToDateTime(nActPrice));
                            }
                            if (nActPrice != null)
                            {
                                dvar["N_ActCost"] = myFunctions.getDateVAL(Convert.ToDateTime(nActCost));
                            }

                            EmployeeTable.AcceptChanges();
                            EmployeeTable = _api.Format(EmployeeTable, "EmpTable");
                        }
                    }
                    else if (xType == "DEP")
                    {
                        FamilyTable = _api.Format(FamilyTable, "FamilyTable");
                        EmployeeTable = _api.Format(EmployeeTable, "EmpTable");

                        FamilySql = "select * from vw_EmployeeDependenceDetails where N_CompanyID=@nCompanyID and N_EmpID =" + nEmpID + " and  N_DependenceID=" + xDepId + " and N_FnYearId=" + nFnYearID + "";
                        FamilyTable = myFunctions.AddNewColumnToDataTable(FamilyTable, "N_Price", typeof(double), 0);
                        FamilyTable = myFunctions.AddNewColumnToDataTable(FamilyTable, "N_Cost", typeof(double), 0);
                        FamilyTable = myFunctions.AddNewColumnToDataTable(FamilyTable, "N_ActPrice", typeof(double), 0);
                        FamilyTable = myFunctions.AddNewColumnToDataTable(FamilyTable, "N_ActCost", typeof(double), 0);


                        if (FamilyTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }

                        foreach (DataRow kvar in FamilyTable.Rows)
                        {
                            object nPrice = dLayer.ExecuteScalar("Select N_Price From vw_MedicalInsDeletionEmployee Where N_CompanyID =@nCompanyID  and N_EmpID=" + nEmpID + " and  N_DependenceID =" + xDepId + " order by D_AdditionDate Desc ", EmpParams, connection);
                            object nCost = dLayer.ExecuteScalar("Select N_Cost From  vw_MedicalInsDeletionEmployee Where N_CompanyID =@nCompanyID  and N_EmpID=" + nEmpID + " and  N_DependenceID =" + xDepId + " order by D_AdditionDate Desc ", EmpParams, connection);
                            object nActPrice = dLayer.ExecuteScalar("select N_ActPrice from  vw_MedicalInsDeletionEmployee Where N_CompanyID =@nCompanyID  and N_EmpID=" + nEmpID + " and  N_DependenceID =" + xDepId + " order by D_AdditionDate Desc ", EmpParams, connection);
                            object nActCost = dLayer.ExecuteScalar("select N_ActCost from  vw_MedicalInsDeletionEmployee Where N_CompanyID =@nCompanyID  and N_EmpID=" + nEmpID + " and  N_DependenceID =" + xDepId + " order by D_AdditionDate Desc ", EmpParams, connection);

                            if (nPrice != null)
                            {
                                kvar["N_Price"] = myFunctions.getVAL(nPrice.ToString());
                            }
                            if (nCost != null)
                            {
                                kvar["N_Cost"] = myFunctions.getVAL(nCost.ToString());
                            }
                            if (nActPrice != null)
                            {
                                kvar["N_ActPrice"] = myFunctions.getDateVAL(Convert.ToDateTime(nActPrice));
                            }
                            if (nActPrice != null)
                            {
                                kvar["N_ActCost"] = myFunctions.getDateVAL(Convert.ToDateTime(nActCost));
                            }
                        }
                        FamilyTable.AcceptChanges();
                        FamilyTable = _api.Format(FamilyTable, "RelationTable");


                    }

                    dt.Tables.Add(EmployeeTable);
                    dt.Tables.Add(FamilyTable);

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



