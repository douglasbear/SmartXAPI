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
    [Route("department")]
    [ApiController]
    public class Pay_Department : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID;

        public Pay_Department(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 202;//form id of cost center
        }

        [HttpGet("list")]
        public ActionResult GetJobTitle(int nDivisionID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);

            string sqlCommandText = "Select N_CompanyID,N_DepartmentID,N_DivisionID,Code,Description from vw_PayDepartment_Disp Where N_CompanyID=@nCompanyID order by Code";
            if (nDivisionID > 0)
            {
                Params.Add("@nDivisionID", nDivisionID);
                sqlCommandText = "Select N_CompanyID,N_DepartmentID,N_DivisionID,Code,Description from vw_PayDepartment_Disp Where N_CompanyID=@nCompanyID and N_DivisionID=@nDivisionID order by Code";
            }

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
         private void GetNextLevelPattern(int higerlevelid)
        {
            try
            {
                object ObjChildN_LevelPattern = null;
                object ObjChildX_LevelPattern = null;
                object ObjRowCount = null;
                object ObjChildCount = null;

                ObjRowCount = dba.ExecuteSclarNoErrorCatch("Select COUNT(*) FROM Acc_CostCentreMaster Where N_CompanyID= " + myCompanyID._CompanyID, "TEXT", new DataTable());
                if (higerlevelid > 0)
                    ObjChildCount = dba.ExecuteSclarNoErrorCatch("Select COUNT(*) FROM Acc_CostCentreMaster Where N_GroupID=" + higerlevelid + " and N_CompanyID= " + myCompanyID._CompanyID, "TEXT", new DataTable());
                else
                    ObjChildCount = 0;
                if (ObjRowCount != null)
                {
                    if (myFunctions.getIntVAL(ObjRowCount.ToString()) > 0)
                    {

                        //parent
                        if (higerlevelid == 0)
                        {
                            N_LevelPattern = 0;
                            //object ObjLevelPattern = dba.ExecuteSclarNoErrorCatch("SELECT MAX(X_LevelPattern)+1 FROM Acc_CostCentreMaster Where N_CompanyID= " + myCompanyID._CompanyID, "TEXT", new DataTable());
                            object ObjLevelPattern = dba.ExecuteSclarNoErrorCatch("SELECT MAX(X_LevelPattern)+1 FROM Acc_CostCentreMaster Where N_LevelID=1 and N_CompanyID= " + myCompanyID._CompanyID, "TEXT", new DataTable());
                            if (ObjLevelPattern != null)
                            {
                                X_LevelPattern = ObjLevelPattern.ToString();
                            }
                        }

                        //child                        
                        else if (higerlevelid > 0)
                        {
                            if (ObjChildCount != null)
                            {
                                if (myFunctions.getIntVAL(ObjChildCount.ToString()) > 0)
                                {
                                    //ObjChildN_LevelPattern = dba.ExecuteSclarNoErrorCatch("SELECT MAX(N_LevelPattern)+1 FROM Acc_CostCentreMaster Where N_GroupID=" + higerlevelid + " and N_CompanyID= " + myCompanyID._CompanyID, "TEXT", new DataTable());
                                    ObjChildN_LevelPattern = dba.ExecuteSclarNoErrorCatch("SELECT MAX(N_LevelPattern)+1 FROM Acc_CostCentreMaster Where  N_CompanyID= " + myCompanyID._CompanyID, "TEXT", new DataTable());
                                    if (ObjChildN_LevelPattern != null)
                                    {
                                        N_LevelPattern = myFunctions.getIntVAL(ObjChildN_LevelPattern.ToString());
                                    }
                                }
                                else
                                {
                                    object objN_level = dba.ExecuteSclarNoErrorCatch("SELECT N_LevelPattern FROM Acc_CostCentreMaster Where N_CostCentreID=" + higerlevelid + " and N_CompanyID= " + myCompanyID._CompanyID, "TEXT", new DataTable());
                                    if (myFunctions.getIntVAL(objN_level.ToString()) != 0)
                                         ObjChildN_LevelPattern = dba.ExecuteSclarNoErrorCatch("SELECT MAX(N_LevelPattern)+1 FROM Acc_CostCentreMaster Where  N_CompanyID= " + myCompanyID._CompanyID, "TEXT", new DataTable());
                                    else
                                        ObjChildN_LevelPattern = dba.ExecuteSclarNoErrorCatch("SELECT X_LevelPattern FROM Acc_CostCentreMaster Where N_CostCentreID=" + higerlevelid + " and N_CompanyID= " + myCompanyID._CompanyID, "TEXT", new DataTable());
                                    if (ObjChildN_LevelPattern != null)
                                    {
                                        N_LevelPattern = myFunctions.getIntVAL(ObjChildN_LevelPattern.ToString());
                                    }
                                }
                            }

                            ObjChildX_LevelPattern = dba.ExecuteSclarNoErrorCatch("SELECT X_LevelPattern FROM Acc_CostCentreMaster Where N_CostCentreID=" + higerlevelid + " and N_CompanyID= " + myCompanyID._CompanyID, "TEXT", new DataTable());
                            if (ObjChildX_LevelPattern != null)
                            {
                                X_LevelPattern = ObjChildX_LevelPattern.ToString() + N_LevelPattern.ToString();
                            }
                        }
                    }
                    else
                    {
                        N_LevelPattern = 0;
                        X_LevelPattern = "101";
                    }
                }
            }
            catch (Exception ex)
            {
                msg.msgError(ex.Message);
            }
             
        }
        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    SortedList QueryParams = new SortedList();
                    string X_CostCentreCode = MasterTable.Rows[0]["x_CostCentreCode"].ToString();
                    string X_CostcentreName = MasterTable.Rows[0]["x_CostcentreName"].ToString();
                    string N_CompanyID = MasterTable.Rows[0]["n_CompanyId"].ToString();
                    string N_FnYearID = MasterTable.Rows[0]["n_FnYearId"].ToString();
                    int N_CostCentreID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CostCentreID"].ToString());
                    int N_GroupID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_GroupID"].ToString());
                    int N_LevelID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LevelID"].ToString());

                    QueryParams.Add("@nCompanyID", N_CompanyID);
                    QueryParams.Add("@nYearID", N_FnYearID);
                    QueryParams.Add("@nCostCentreID", N_CostCentreID);
                    QueryParams.Add("@nGroupID", N_GroupID);
                    QueryParams.Add("@nLevelID", N_LevelID);


                    if (N_CostCentreID == 0)
                    {
                        if (N_GroupID == 0)
                            N_LevelID = 1;
                        else
                        {
                            object ObjNextLevelID = dLayer.ExecuteScalar("select isnull(N_LevelID,0)+1 from Acc_CostCentreMaster where N_CostCentreID=" + N_GroupID + " and N_FnYearID=@nFnYearID and N_CompanyID= @nCompanyID ", QueryParams, connection);
                            if (ObjNextLevelID == null)
                                N_LevelID = 0;
                            else
                                N_LevelID = myFunctions.getIntVAL(ObjNextLevelID.ToString());
                        }
                    }

                    if (X_CostCentreCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("N_BranchID", MasterTable.Rows[0]["n_BranchId"].ToString());
                        X_CostCentreCode = dLayer.GetAutoNumber("Acc_CostCentreMaster", "x_CostCentreCode", Params, connection, transaction);
                        if (X_CostCentreCode == "") { return Ok(_api.Error("Unable to generate Department/Cost Centre Code")); }
                        MasterTable.Rows[0]["x_CostCentreCode"] = X_CostCentreCode;

                    }
                    // else
                    // {
                    //     dLayer.DeleteData("inv_salesman", "N_SalesmanID", N_SalesmanID, "", connection, transaction);
                    // }

                    N_CostCentreID = dLayer.SaveData("Acc_CostCentreMaster", "N_CostCentreID", MasterTable, connection, transaction);
                    if (N_CostCentreID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                    }

                    return Ok(_api.Success("Department/Cost Centre Saved"));
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

    }
}
