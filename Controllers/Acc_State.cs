using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("state")]
    [ApiController]
    public class Acc_State : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1695;
        public Acc_State(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetStateList(int nCountryID)
        {
            DataTable dt = new DataTable(); 
            SortedList Params = new SortedList();
            string sqlCommandText ;
          if (nCountryID>0)
          {
            sqlCommandText = "select * from Acc_State where N_CountryID=@p1 order by N_StateID";  
          }
          else {
            sqlCommandText = "select * from Acc_State order by N_StateID";
          }
          
            Params.Add("@p1", nCountryID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok( _api.Error(User,e));
            }
        }

        [HttpGet("details") ]
        public ActionResult GetStateDetails (int nStateID)
        {   
            DataTable dt=new DataTable();
            SortedList Params = new SortedList();
            
            string sqlCommandText="select * from vw_Acc_State where N_StateID=@p1 ";
            Params.Add("@p1",nStateID);
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }

                if(dt.Rows.Count==0)
                {
                    return Ok(_api.Notice("No Results Found"));
                } else {
                    return Ok(_api.Success(dt));
                }
            } 
            catch(Exception e)
            {
                return Ok(_api.Error(User,e));
            }   
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nStateID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_StateID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string StateCode = "";
                    var values = MasterTable.Rows[0]["x_StateCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.N_FormID);
                      
                        StateCode = dLayer.GetAutoNumber("Acc_State", "X_StateCode", Params, connection, transaction);
                        if (StateCode == "") { transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to generate State Code")); }
                        MasterTable.Rows[0]["x_StateCode"] = StateCode;
                    }
                    MasterTable.Columns.Remove("n_CompanyID");
                    MasterTable.Columns.Remove("n_FnYearID");

                    nStateID = dLayer.SaveData("Acc_State", "N_StateID", MasterTable, connection, transaction);
                    if (nStateID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("State Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(User,ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nStateID)
        {
            int Results=0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                connection.Open();
                Results=dLayer.DeleteData("Acc_State","N_StateID",nStateID,"",connection);

                if(Results>0) {
                    return Ok(_api.Success("State deleted"));
                } else {
                    return Ok(_api.Warning("Unable to delete State" ));
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