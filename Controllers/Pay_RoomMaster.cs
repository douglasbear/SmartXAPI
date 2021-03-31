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
    [Route("roomMaster")]
    [ApiController]
    public class Pay_RoomMaster : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 957;


        public Pay_RoomMaster(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetRoomMaster()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            string sqlCommandText="Select N_CompanyID, N_RoomId, X_RoomCode, X_RoomName, N_VillaID, X_Location, N_RentAmount, X_Remarks, D_Entrydate, N_Electricity, N_Water, N_Internet, N_Capasity from vw_Pay_RoomMaster Where N_CompanyID=@nCompanyID order by X_RoomCode";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nRoomId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_RoomId"].ToString());


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string RoomCode = "";
                    var values = MasterTable.Rows[0]["X_RoomCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        RoomCode = dLayer.GetAutoNumber("Pay_RoomMaster", "X_RoomCode", Params, connection, transaction);
                        if (RoomCode == "") { return Ok(_api.Error("Unable to generate Room Code")); }
                        MasterTable.Rows[0]["X_RoomCode"] = RoomCode;
                        

                    }
                    MasterTable.Columns.Remove("n_FnYearId");
                    MasterTable.Columns.Remove("n_OccupiedRooms");
                    MasterTable.Columns.Remove("n_AvaialbleSpace");
                    // if(n_OccupiedRooms <= 0)
                    // {
                    // MasterTable.Columns.Remove("n_OccupiedRooms");
                    // MasterTable.Columns.Remove("n_AvailableSpace");
                    // }
                  
                   nRoomId = dLayer.SaveData("Pay_RoomMaster", "n_RoomId", MasterTable, connection, transaction);
                   

                   if (nRoomId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Room Information Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(ex));
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