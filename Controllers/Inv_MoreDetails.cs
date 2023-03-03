

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
    [Route("deviceInfo")]
    [ApiController]
    public class Inv_DeviceInfo : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int nFormID = 1545;

        public Inv_DeviceInfo(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();



                    DataTable Master = ds.Tables["master"];
                    DataTable Details = ds.Tables["details"];


                    DataTable MasterTable;
                    DataTable ServiceConditionTable;
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    DataRow MasterRow = Master.Rows[0];
                    ServiceConditionTable = ds.Tables["servicecondition"];
                    // DataRow MasterRow = MasterTable.Rows[0];
                    int N_ServiceInfoID = myFunctions.getIntVAL(MasterRow["N_ServiceInfoID"].ToString());
                    int N_ServiceConditionID=0;
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["N_CompanyID"].ToString());
                    int N_UserID = myFunctions.getIntVAL(MasterRow["n_UserID"].ToString());
                    string X_ServiceInfoCode = MasterRow["X_ServiceInfoCode"].ToString();



                    if (X_ServiceInfoCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_FormID", nFormID);
                        Params.Add("N_YearID", nFnYearID);
                        X_ServiceInfoCode = dLayer.GetAutoNumber("Inv_ServiceInfo", "X_ServiceInfoCode", Params, connection, transaction);
                        if (X_ServiceInfoCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Device Number");
                        }
                        Master.Rows[0]["X_ServiceInfoCode"] = X_ServiceInfoCode;
                    }
                    if (MasterTable.Columns.Contains("n_FnYearID"))
                    {

                        MasterTable.Columns.Remove("n_FnYearID");

                    }
                    if (N_ServiceInfoID > 0)
                    {
                        dLayer.DeleteData("Inv_ServiceCondition", "N_ServiceInfoID", N_ServiceInfoID, "N_CompanyID=" + N_CompanyID + " and N_ServiceInfoID=" + N_ServiceInfoID, connection, transaction);
                        dLayer.DeleteData("Inv_ServiceInfo", "N_ServiceInfoID", N_ServiceInfoID, "N_CompanyID=" + N_CompanyID + " and N_ServiceInfoID=" + N_ServiceInfoID, connection, transaction);
                        dLayer.DeleteData("Inv_ServiceMaterials", "N_ServiceInfoID", N_ServiceInfoID, "N_CompanyID=" + N_CompanyID + " and N_ServiceInfoID=" + N_ServiceInfoID, connection, transaction);
                         
                    }
                    // string DupCriteria = "";


                    N_ServiceInfoID = dLayer.SaveData("Inv_ServiceInfo", "N_ServiceInfoID", "", "", Master, connection, transaction);
                    if (N_ServiceInfoID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                    for (int i = 0; i < Details.Rows.Count; i++)
                    {
                        Details.Rows[i]["N_ServiceInfoID"] = N_ServiceInfoID;

                    }

                    dLayer.SaveData("Inv_ServiceMaterials", "n_MaterialID", Details, connection, transaction);

                    for (int i = 0; i < ServiceConditionTable.Rows.Count; i++)
                    {
                        ServiceConditionTable.Rows[i]["N_ServiceInfoID"] = N_ServiceInfoID;

                    }
                    N_ServiceConditionID = dLayer.SaveData("Inv_ServiceCondition", "N_ServiceConditionID", ServiceConditionTable, connection, transaction);

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("N_ServiceInfoID", N_ServiceInfoID);
                    return Ok(api.Success(Result, "Service Info Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nServiceInfoID, int nCompanyID)
        {
            int Results = 0;
            nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    object N_ServiceInfoID = dLayer.ExecuteScalar("select count(N_ServiceID) from Inv_SalesOrderDetails  Where N_CompanyID=" + nCompanyID + " and N_ServiceID="+nServiceInfoID, connection);

                        if( myFunctions.getIntVAL(N_ServiceInfoID.ToString())>0){

                             return Ok(api.Error(User, "Unable To Delete!"));
                         }

                    Results = dLayer.DeleteData("Inv_ServiceInfo", "N_ServiceInfoID", nServiceInfoID, "N_CompanyID=" + nCompanyID + "", connection);
                    dLayer.DeleteData("Inv_ServiceCondition", "N_ServiceInfoID", nServiceInfoID, "N_CompanyID=" + nCompanyID + "", connection);
                    dLayer.DeleteData("Inv_ServiceMaterials", "N_ServiceInfoID", nServiceInfoID, "N_CompanyID=" + nCompanyID + "", connection);
                        

                }
                if (Results > 0)
                {
                    return Ok(api.Success("Deleted Sucessfully"));
                }
                else
                {
                    return Ok(api.Error(User, "Unable to delete"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }

        }

        [HttpGet("details")]
        public ActionResult device(int nServiceInfoID, int nCompanyID)
        {
            DataTable Master = new DataTable();
            DataTable DeviceDetails = new DataTable();
            DataTable ReqMaterials = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            nCompanyID = myFunctions.GetCompanyID(User);
            string xCriteria = "",
            sqlCommandText = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nServiceInfoID);

            // Params.Add("@p3",x_SerialNo);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();



                    sqlCommandText = "select * from vw_Service_Info Where N_CompanyID = @p1 and N_ServiceInfoID=@p2";


                    Master = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    Master = api.Format(Master, "Master");
                   

                    // if (Master.Rows.Count == 0)
                    // {
                    //     //return Ok(api.Notice("No Results Found"));
                    //     return Ok(api.Success(ds));
                    // }
                    // else
                    // {
                        // Params.Add("@nServiceInfoID", Master.Rows[0]["nServiceInfoID"].ToString());
                       
                        ds.Tables.Add(Master);
                        string DeviceDetailsSql = "select * from vw_Inv_ServiceCondition where N_CompanyID=" + nCompanyID + " and N_ServiceInfoID=" + nServiceInfoID;

                        DeviceDetails = dLayer.ExecuteDataTable(DeviceDetailsSql, Params, connection);
                        DeviceDetails = api.Format(DeviceDetails, "DeviceDetails");
                        ds.Tables.Add(DeviceDetails);

                        //  int N_MaterialID = myFunctions.getIntVAL(ReqMaterials.Rows[0]["N_MaterialID"].ToString());

                        string ReqMaterialsSql = "select * from vw_Inv_ServiceMaterials where N_CompanyID=" + nCompanyID + " and N_ServiceInfoID=" + nServiceInfoID;

                        ReqMaterials = dLayer.ExecuteDataTable(ReqMaterialsSql, Params, connection);
                        ReqMaterials = api.Format(ReqMaterials, "ReqMaterials");
                        ds.Tables.Add(ReqMaterials);


                    //}

                }
                return Ok(api.Success(ds));
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }

        }


        [HttpGet("list")]
        public ActionResult List()
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();



            string sqlCommandText = "select  * from [Inv_ServiceInfo] where N_CompanyID=@p1";

            Params.Add("@p1", nCompanyId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    return Ok(api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
    }
}
