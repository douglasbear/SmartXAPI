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
    [Route("employeeClearanceSettings")]
    [ApiController]
    public class EmployeeClearanceSettings : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1062;


        public EmployeeClearanceSettings(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
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
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();

                    int n_ClearanceSettingsID = myFunctions.getIntVAL(MasterRow["N_ClearanceSettingsID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string x_ClearanceCode = MasterRow["X_ClearanceCode"].ToString();

                    if (x_ClearanceCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", N_FormID);
                        x_ClearanceCode = dLayer.GetAutoNumber("Pay_EmployeeClearanceSettings", "x_ClearanceCode", Params, connection, transaction);
                        if (x_ClearanceCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Clearance Code");
                        }
                        MasterTable.Rows[0]["x_ClearanceCode"] = x_ClearanceCode;
                    }
                    MasterTable.Columns.Remove("N_FormID");

                    n_ClearanceSettingsID = dLayer.SaveData("Pay_EmployeeClearanceSettings", "n_ClearanceSettingsID", "", "", MasterTable, connection, transaction);
                    if (n_ClearanceSettingsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Clearance Code");
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_ClearanceSettingsID"] = n_ClearanceSettingsID;
                    }
                    int n_ClearanceSettingsDetailsID = dLayer.SaveData("Pay_EmployeeClearanceSettingsDetails", "n_ClearanceSettingsDetailsID", DetailTable, connection, transaction);
                    if (n_ClearanceSettingsDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Clearance Code");
                    }


                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_ClearanceSettingsID", n_ClearanceSettingsID);
                    Result.Add("x_ClearanceCode", x_ClearanceCode);
                    Result.Add("n_ClearanceSettingsDetailsID", n_ClearanceSettingsDetailsID);

                    return Ok(_api.Success(Result, "Clearance Code Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

             
        [HttpGet("details")]
        public ActionResult GetDetails(int nRoomID)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText="select * from vw_Pay_RoomMaster where N_CompanyID=@nCompanyID and N_RoomID=@nRoomID";
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nRoomID",nRoomID);


              

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection); 
                         object OccupiedRooms = dLayer.ExecuteScalar("select count(N_RoomID) from Pay_Employee where N_AccEndDate>'" + myFunctions.getDateVAL(System.DateTime.Now) + "' and  N_CompanyID=@nCompanyID and N_RoomID=@nRoomID", Params,connection);
                if (OccupiedRooms == null)
                {
                    OccupiedRooms="0";
                }
                    int  N_OccupiedRooms= myFunctions.getIntVAL(OccupiedRooms.ToString());
                    int AvailableSpace = myFunctions.getIntVAL(dt.Rows[0]["N_Capasity"].ToString()) - myFunctions.getIntVAL(OccupiedRooms.ToString());
                   
                    if (AvailableSpace < 0)
                    {
                        AvailableSpace = 0;
                    }
                    dt.Columns.Add("N_OccupiedRooms",typeof(System.Int32));
                    dt.Columns.Add("N_AvaialbleSpace",typeof(System.Int32));
                    foreach(DataRow row in dt.Rows)
                    {
    //need to set value to NewColumn column
                   row["N_OccupiedRooms"] = N_OccupiedRooms;
                   row["N_AvaialbleSpace"] = AvailableSpace;
    }


                    }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(_api.Notice("No Results Found" ));
                        } 
                        else
                        {
                            return Ok(_api.Success(dt));
                        }
            }
            catch(Exception e)
            {

                return Ok(_api.Error(e));
            }
        }

          [HttpDelete("delete")]
        public ActionResult DeleteData(int nRoomID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Pay_RoomMaster", "n_RoomID", nRoomID, "", connection);
                    if (Results > 0)
                    {
                        return Ok( _api.Success("deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete "));
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