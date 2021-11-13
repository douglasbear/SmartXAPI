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
    [Route("vehicle")]
    [ApiController]
    public class Inv_TruckMaster : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 638;

        public Inv_TruckMaster(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)

        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            // FormID = 188;
        }

        [HttpGet("vehicleList")]
        public ActionResult GetVehileList()

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);

            string sqlCommandText = "Select *  from Inv_TruckMaster Where N_CompanyID= " + nCompanyID + "";


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("driverList")]
        public ActionResult GetDriverList()

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);

            string sqlCommandText = "Select *  from Veh_Drivers Where N_CompanyID= " + nCompanyID + "";


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(api.Error(User, e));
            }
        }
        [HttpGet("assetList")]
        public ActionResult GetAssetList(bool b_AllBranchData, int nFnYearID, int nBranchID)

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);

            string sqlCommandText = "";
            if (b_AllBranchData)
                sqlCommandText = "Select *  from vw_Ass_AssetItem_Search Where N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_Status<>2";
            else
                sqlCommandText = "Select *  from vw_Ass_AssetItem_Search Where N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_BranchID=" + nBranchID + " and N_Status<>2";


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(api.Error(User, e));
            }
        }

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
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];

                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nTruckID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TruckID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    string X_TruckCode = MasterTable.Rows[0]["X_TruckCode"].ToString();
                    string X_PlateNumber = MasterTable.Rows[0]["X_PlateNumber"].ToString();

                    DocNo = MasterRow["X_TruckCode"].ToString();
                    MasterTable.Columns.Remove("N_FnYearID");
                    if (DocNo == "@Auto")
                    {

                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", 638);

                        X_TruckCode = dLayer.GetAutoNumber("Inv_TruckMaster", "X_TruckCode", Params, connection, transaction);
                        if (X_TruckCode == "")
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User, "Unable to generate Invoice Number"));
                        }
                        MasterTable.Rows[0]["X_TruckCode"] = X_TruckCode;
                    }
                    string DupCriteria = "N_companyID=" + nCompanyID + " And X_PlateNumber = '" + X_PlateNumber + "' and X_TruckCode='" + X_TruckCode + "'";


                    nTruckID = dLayer.SaveData("Inv_TruckMaster", "N_TruckID", DupCriteria, "", MasterTable, connection, transaction);
                    if (nTruckID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                    transaction.Commit();

                    return Ok(api.Success("successfully created"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }

        [HttpGet("details")]
        public ActionResult VehicleDetails(int nTruckID)
        {


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@N_CompanyID", nCompanyID);

                    string sqlCommandText = "select * from vwInv_TruckMaster  where N_CompanyID=@N_CompanyID and N_TruckID=" + nTruckID + "";
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(dt));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nTruckID, int nCompanyID, int fnYearID)

        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SortedList ParamList = new SortedList();
                    DataTable transferStockDetail = new DataTable();
                    ParamList.Add("@nCompanyID", myFunctions.GetCompanyID(User));

                    string transferStock = "Select  N_TruckID From Inv_TransferStockDetails Where N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_TruckID =" + nTruckID + "";
                    transferStockDetail = dLayer.ExecuteDataTable(transferStock, ParamList, connection);
                    SqlTransaction transaction = connection.BeginTransaction();
                    if (transferStockDetail.Rows.Count > 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Warning("Unable to delete"));

                    }
                    bool B_IsTruckUsedDN = Convert.ToBoolean(dLayer.ExecuteScalar("Select N_TruckID From Inv_DeliveryNote where N_TruckID='" + nTruckID + "'", ParamList, connection, transaction));
                    bool B_IsTruckUsedSI = Convert.ToBoolean(dLayer.ExecuteScalar("Select N_TruckID From Inv_Sales where N_TruckID='" + nTruckID + "'", ParamList, connection, transaction));
                    if (B_IsTruckUsedDN || B_IsTruckUsedSI)
                    {
                        transaction.Rollback();
                        return Ok(api.Warning("Unable to delete"));
                    }
                    int Results = dLayer.DeleteData("Inv_TruckMaster", "N_TruckID", nTruckID, "", connection, transaction);
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User, "Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }
    }
}























