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
    [Route("jobtitle")]
    [ApiController]
    public class Pay_JobTitle : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Pay_JobTitle(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 195;
        }

        [HttpGet("list")]
        public ActionResult GetJobTitle()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "Select N_CompanyID,N_PositionID,N_EmpID,N_SalaryGradeID,B_Edit,Code,Description from vw_PayPosition_DispAdvanced Where N_CompanyID=@nCompanyID order by Code";
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
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable dtSupervisor;
                MasterTable = ds.Tables["master"];
                dtSupervisor = ds.Tables["supervisor"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    SortedList QueryParams = new SortedList();
                    string X_PositionCode = MasterTable.Rows[0]["x_PositionCode"].ToString();
                    string X_Position = MasterTable.Rows[0]["x_Position"].ToString();
                    int N_CompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                    int N_PositionID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PositionID"].ToString());
                    int N_SupervisorID = myFunctions.getIntVAL(dtSupervisor.Rows[0]["n_SupervisorID"].ToString());
                    bool B_IsSupervisor = myFunctions.getBoolVAL(MasterTable.Rows[0]["b_IsSupervisor"].ToString());
                    QueryParams.Add("@nCompanyID", N_CompanyID);
                    //QueryParams.Add("@nFnYearID", N_FnYearID);
                    QueryParams.Add("@nPositionID", N_PositionID);

                    if (X_PositionCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", this.FormID);
                        //Params.Add("N_BranchID", MasterTable.Rows[0]["n_BranchId"].ToString());
                        X_PositionCode = dLayer.GetAutoNumber("Pay_Position", "x_PositionCode", Params, connection, transaction);
                        if (X_PositionCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Job title Code")); }
                        MasterTable.Rows[0]["x_PositionCode"] = X_PositionCode;


                    }
                    else
                    {
                        dLayer.DeleteData("Pay_Position", "n_positionID", N_PositionID, "N_CompanyID=" + N_CompanyID + "", connection, transaction);

                        if (B_IsSupervisor)
                        {
                            N_SupervisorID = dLayer.SaveData("Pay_Supervisor", "N_SupervisorID", dtSupervisor, connection, transaction);

                        }
                        else
                            dLayer.DeleteData("Pay_Supervisor", "N_SupervisorID", N_SupervisorID, "N_CompanyID=" + N_CompanyID + "", connection, transaction);
                    }
                    N_PositionID = dLayer.SaveData("Pay_Position", "N_PositionID", MasterTable, connection, transaction);
                    if (N_PositionID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {
                        // if (B_IsSupervisor)
                        // {
                        //     N_SupervisorID = dLayer.SaveData("Pay_Supervisor", "N_SupervisorID", dtSupervisor, connection, transaction);
                        // }
                        // else
                        //     dLayer.DeleteData("Pay_Supervisor", "N_SupervisorID", N_SupervisorID, "N_CompanyID=" + N_CompanyID + "", connection, transaction);
                        transaction.Commit();
                    }

                    return Ok(_api.Success("Job title Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }



        [HttpDelete("delete")]
        public ActionResult DeleteData(int nDepartmentID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nCostCentreID", nDepartmentID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object Objcount = dLayer.ExecuteScalar("Select count(*) From vw_Acc_CostCentreMaster_List where N_CostCentreID=@nCostCentreID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection);
                    if (Objcount != null)
                    {
                        if (myFunctions.getIntVAL(Objcount.ToString()) <= 0)
                        {
                            Results = dLayer.DeleteData("Acc_CostCentreMaster", "N_CostCentreID", nDepartmentID, "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection);
                        }
                    }
                }
                if (Results > 0)
                {
                    return Ok(_api.Success("Department/Cost centre deleted"));
                }
                else
                {
                    return Ok(_api.Error("Unable to delete"));
                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }


        }
        [HttpGet("dummy")]
        // public ActionResult GetDepartmentDummy()
        // {
           
        //     try
        //     {
        //         DataTable masterTable;
        //         DataTable dtSupervisor;
        //         using (SqlConnection Con = new SqlConnection(connectionString))
        //         {
        //             Con.Open();
        //             string sqlCommandText = "select * from Pay_Empshiftdetails where N_ShiftDetailsID=1";
        //             masterTable=dLayer.ExecuteDataTable(sqlCommandText,Con);
        //             masterTable = _api.Format(masterTable, "master");


                   
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(_api.Error(ex));
        //     }
        // }
        // [HttpPost("save")]
        // public ActionResult SaveData([FromBody]DataSet ds)
        // { 
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();
        //             DataTable MasterTable;
        //             MasterTable = ds.Tables["master"];
        //             SortedList Params = new SortedList();
        //         int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
        //         int nPositionID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PositionID"].ToString());
        //         int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
        //         string xPositionCode = MasterTable.Rows[0]["x_PositionCode"].ToString();
        //         MasterTable.Columns.Remove("n_FnYearID");
        //         MasterTable.AcceptChanges();
        //          if (xPositionCode == "@Auto")
        //             {
        //                 Params.Add("N_CompanyID", nCompanyID);
        //                 Params.Add("N_YearID", nFnYearID);
        //                 Params.Add("N_FormID", this.FormID);
        //                 xPositionCode = dLayer.GetAutoNumber("Pay_Position", "x_PositionCode", Params, connection, transaction);
        //                 if (xPositionCode == "") { return Ok(_api.Error("Unable to generate Position Code")); }
        //                 MasterTable.Rows[0]["x_PositionCode"] = xPositionCode;
        //             }
        //             else
        //             {
        //                 dLayer.DeleteData("Pay_Position", "N_PositionID", nPositionID, "", connection, transaction);
        //             }

        //             nPositionID=dLayer.SaveData("Pay_Position","N_PositionID",MasterTable,connection,transaction);  
        //             transaction.Commit();
        //             return Ok(_api.Success("Job Title Saved")) ;
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(_api.Error(ex));
        //     }
        // }

        [HttpGet("details")]
        public ActionResult GetJobTitleDetails(int nPositionID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_PayPosition_DispAdvanced where N_CompanyID=@nCompanyID and N_PositionID=@nPositionID";
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nPositionID", nPositionID);
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

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPositionID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Pay_Position", "N_PositionID", nPositionID, "", connection);
                    if (Results > 0)
                    {
                        return Ok(_api.Success("Job Title deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete Job Title"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }



    }
}