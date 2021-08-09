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
    [Route("branch")]
    [ApiController]



    public class AccBranchController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public AccBranchController(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 370;

        }


        [HttpGet("list")]
        public ActionResult GetAllBranches()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select N_BranchID,N_CompanyId,X_BranchName,X_BranchCode,Active from Acc_BranchMaster where N_CompanyId=@p1 order by N_BranchID DESC";
            Params.Add("@p1", nCompanyId);
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


        [HttpGet("details")]
        public ActionResult GetBranchDetails(int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from Acc_BranchMaster where N_CompanyId=@nCompanyId and N_BranchID=@nBranchID";
            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nBranchID", nBranchID);
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

        [HttpPost("change")]
        public ActionResult ChangeData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nBranchID", nBranchID);

                    dLayer.ExecuteNonQuery("update Acc_BranchMaster set IsCurrent=0 where N_CompanyID=@nCompanyID", Params, connection);
                    dLayer.ExecuteNonQuery("update Acc_BranchMaster set IsCurrent=1 where N_BranchID=@nBranchID and N_CompanyID=@nCompanyID", Params, connection);

                    return Ok(_api.Success("Branch Changed"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    SortedList Params1 = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    string xBranchCode = MasterTable.Rows[0]["x_BranchCode"].ToString();
                    int nLocationID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                    string xLocationCode = MasterTable.Rows[0]["x_BranchCode"].ToString();
                    string xLocationName = MasterTable.Rows[0]["x_BranchName"].ToString();
                    if(MasterTable.Columns.Contains("n_FnYearID"))
                    MasterTable.Columns.Remove("n_FnYearID");
                    MasterTable.AcceptChanges();
                    if (xBranchCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        xBranchCode = dLayer.GetAutoNumber("Acc_BranchMaster", "x_BranchCode", Params, connection, transaction);
                        if (xBranchCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Branch Code")); }
                        MasterTable.Rows[0]["x_BranchCode"] = xBranchCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Acc_BranchMaster", "N_BranchID", nBranchID, "", connection, transaction);
                    }

                    nBranchID = dLayer.SaveData("Acc_BranchMaster", "N_BranchID", MasterTable, connection, transaction);
                    if (nBranchID <= 0)
                    {

                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Branch"));
                    }
                    else
                    {
                        if (xLocationCode == "@Auto")
                        {
                            String sql = "select N_CompanyID,0 as N_LocationID,'' as X_LocationCode,X_BranchName as X_LocationName,N_BranchID,0 as B_IsCurrent,2 as N_typeId,1 as B_IsDefault,X_Address,X_PhoneNo,0 as B_PhysicalSite,0 as N_MainLocationID from Acc_BranchMaster where N_CompanyID=@nCompanyID and N_branchID=@nBranchID";
                            SortedList BranchParams = new SortedList();
                            BranchParams.Add("@nCompanyID", nCompanyID);
                            BranchParams.Add("@nBranchID", nBranchID);
                            DataTable LocationTable = dLayer.ExecuteDataTable(sql, BranchParams, connection, transaction);
                            if (LocationTable.Rows.Count == 0)
                            {
                                transaction.Rollback(); return Ok(_api.Error("Unable to Create location"));
                            }
                            Params1.Add("N_CompanyID", nCompanyID);
                            Params1.Add("N_YearID", nFnYearID);
                            Params1.Add("N_FormID", 450);
                            xLocationCode = dLayer.GetAutoNumber("Inv_Location", "x_LocationCode", Params1, connection, transaction);
                            if (xLocationCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate location Code")); }
                            LocationTable.Rows[0]["x_LocationCode"] = xLocationCode;
                            LocationTable.AcceptChanges();
                            String DupCriteria = "N_BranchID=" + nBranchID + " and X_LocationName= '" + xLocationName + "' and N_CompanyID=" + nCompanyID;
                            nLocationID = dLayer.SaveData("Inv_Location", "N_LocationID", DupCriteria, "", LocationTable, connection, transaction);
                            if (nLocationID <= 0)
                            {
                                transaction.Rollback(); return Ok(_api.Error("Unable to Create location"));
                            }

                        }

                        transaction.Commit();
                        return Ok(_api.Success("Branch Saved"));
                    }


                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nBranchID)
        {
            int Results = 0;

            SortedList Params = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    Params.Add("@nBranchID", nBranchID);
                    object count = dLayer.ExecuteScalar("select count(*) as N_Count from Vw_Acc_BranchMaster_Disp where N_BranchID=@nBranchID and N_CompanyID=N_CompanyID", Params, connection);
                    int N_Count = myFunctions.getIntVAL(count.ToString());
                    if (N_Count <= 0)
                    {
                        Results = dLayer.DeleteData("Acc_BranchMaster", "N_BranchID", nBranchID, "", connection);

                        Results = dLayer.DeleteData("Inv_Location", "N_BranchID", nBranchID, "B_IsDefault=1", connection);


                    }
                    else
                    {
                        return Ok(_api.Success("unable to delete this branch"));
                    }
                    if (Results >= 0)
                    {
                        return Ok(_api.Success("Branch deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete Branch"));
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