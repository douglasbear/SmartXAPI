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
    [Route("projectMobilization")]
    [ApiController]
    public class Prj_Mobilisation : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID;


        public Prj_Mobilisation(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 1006;
        }
        //Save....
        [HttpPost("Save")]
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
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    DataRow DetailRow = DetailTable.Rows[0];
                    SortedList Params = new SortedList();

                    int n_MobilizationID = myFunctions.getIntVAL(MasterRow["N_MobilizationID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string x_MobilizationCode = MasterRow["X_MobilizationCode"].ToString();
                    int N_FromPrj = myFunctions.getIntVAL(MasterRow["N_FromPrj"].ToString());
                    int N_ProjectID = myFunctions.getIntVAL(MasterRow["N_ProjectID"].ToString());

                    if (n_MobilizationID > 0)
                    {
                        dLayer.DeleteData("Mnp_MobilizationDetails", "N_MobilizationID", n_MobilizationID, "", connection,transaction);
                        dLayer.DeleteData("Mnp_Mobilization", "N_MobilizationID", n_MobilizationID, "", connection,transaction);

                    }

                    if (x_MobilizationCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", N_FormID);
                        x_MobilizationCode = dLayer.GetAutoNumber("Mnp_Mobilization", "X_MobilizationCode", Params, connection, transaction);
                        if (x_MobilizationCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate");
                        }
                        MasterTable.Rows[0]["X_MobilizationCode"] = x_MobilizationCode;
                    }

                    n_MobilizationID = dLayer.SaveData("Mnp_Mobilization", "n_MobilizationID", "", "", MasterTable, connection, transaction);
                    if (n_MobilizationID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }

                    for (int j = 0; j < DetailTable.Rows.Count; j++)


                        DetailTable.Rows[j]["N_MobilizationID"] = n_MobilizationID;

                    int n_MobilizationDetailsID = dLayer.SaveData("Mnp_MobilizationDetails", "n_MobilizationDetailsID", DetailTable, connection, transaction);
                    if (n_MobilizationDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save mobilisation ");
                    }

                    // DataTable Gen = dLayer.ExecuteDataTable("Select * from Mnp_MobilizationDetails_Disp where N_CompanyID=@N_CompanyID and N_FnYearID=@N_YearID", Params, connection, transaction);
                    DataTable Gen = dLayer.ExecuteDataTable("Select * from Mnp_MobilizationDetails_Disp", Params, connection, transaction);
                    int nCompanyID = 0;
                    int nEmpID = 0;
                    string EmpsID = "";
                    string empName = "";
                    string nationality = "";
                    string jobTitle = "";

                    foreach (DataRow var in Gen.Rows)
                    {
                        nCompanyID = myFunctions.getIntVAL(var["n_CompanyID"].ToString());
                        nEmpID = myFunctions.getIntVAL(var["n_EmpID"].ToString());
                        empName = var["X_EmployeeName"].ToString();
                        nationality = var["X_Nationality"].ToString();
                        jobTitle = var["X_Position"].ToString();

                        dLayer.ExecuteNonQuery("UPDATE Mnp_EmployeeMaintenance SET B_Mobilized = 1 FROM Mnp_EmployeeMaintenance INNER JOIN Mnp_MobilizationDetails ON Mnp_EmployeeMaintenance.N_CompanyID = Mnp_MobilizationDetails.N_CompanyID AND Mnp_EmployeeMaintenance.N_MaintenanceID = Mnp_MobilizationDetails.N_MaintenanceID INNER JOIN Mnp_Mobilization ON Mnp_MobilizationDetails.N_MobilizationID = Mnp_Mobilization.N_MobilizationID AND Mnp_MobilizationDetails.N_CompanyID = Mnp_Mobilization.N_CompanyID AND Mnp_EmployeeMaintenance.N_FnYearID = Mnp_Mobilization.N_FnYearID Where Mnp_Mobilization.N_CompanyID=" + N_CompanyID + " and Mnp_Mobilization.N_MobilizationID=" + n_MobilizationID + " and Mnp_Mobilization.N_FnYearID=" + N_FnYearID, connection, transaction);
                        if (N_FromPrj > 0)
                            dLayer.ExecuteNonQuery("UPDATE Inv_CustomerProjects SET X_EmployeeName='" + empName.ToString() + "',X_EmpsID='" + EmpsID.ToString() + "' Where N_CompanyID=" + N_CompanyID + " and N_ProjectID=" + N_ProjectID, connection, transaction);
                    }
                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_MobilizationID", n_MobilizationID);
                    Result.Add("x_MobilizationCode", x_MobilizationCode);
                    Result.Add("n_MobilizationDetailsID", n_MobilizationDetailsID);
                    return Ok(_api.Success(Result, "Project Mobilisation saved"));



                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("employList")]
        public ActionResult ProjectList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select N_MaintenanceID,N_CompanyID,B_Mobilized,X_EmployeeCode,X_EmployeeName,X_Nationality,X_Position,N_DailyRate from vw_Employeemaintenance where N_CompanyID=@nComapnyID";
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
                return Ok(_api.Error(User,e));
            }
        }
        [HttpGet("dashboardlist")]
        public ActionResult DashboardList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_CustomerName like '% " + xSearchkey + ")";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_MobilizationID desc";
            else
                xSortBy = " order by " + xSortBy;


            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from Mnp_Mobilization_Disp where N_CompanyID=@p1 ";
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from Mnp_Mobilization_Disp where N_CompanyID=@nCompanyId and  N_MobilizationID not in (select top(" + Count + ") N_MobilizationID from Mnp_Mobilization_Disp  where N_CompanyID=@p1 )";

            Params.Add("@p1", nCompanyId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from Mnp_Mobilization_Disp where N_CompanyID=@p1 ";
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
                return Ok(_api.Error(User,e));
            }
        }


        [HttpGet("details")]
        public ActionResult Details(string xMobilizationCode)
        {


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable DataTable = new DataTable();

                    string Mastersql = "";
                    string DetailSql = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@xMobilizationCode", xMobilizationCode);
                    Mastersql = "select * from Mnp_Mobilization_Disp where N_CompanyId=@nCompanyID and X_MobilizationCode=@xMobilizationCode  ";

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int nMobilizationID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_MobilizationID"].ToString());
                    Params.Add("@nMobilizationID", nMobilizationID);

                    MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from Mnp_MobilizationDetails_Disp where N_CompanyId=@nCompanyID and N_MobilizationID=@nMobilizationID ";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nMobilizationID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nMobilizationID", nMobilizationID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Mnp_Mobilization", "N_MobilizationID", nMobilizationID, "", connection);


                    if (Results > 0)
                    {
                        dLayer.DeleteData("Mnp_MobilizationDetails", "N_MobilizationID", nMobilizationID, "", connection);
                        return Ok(_api.Success("Project Mobilisation deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete"));
                    }

                }

            }
            catch (Exception ex)

            {
                return Ok(_api.Error(User,ex));
            }


        }


    }
}
