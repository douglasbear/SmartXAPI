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
    [Route("city")]
    [ApiController]
    public class Acc_City : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1429;
        public Acc_City(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetCityList(int nCountryID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable(); 
            SortedList Params = new SortedList();
            string sqlCommandText ;
          if(nCountryID>0)
          {
               sqlCommandText = "select * from Acc_City where N_CompanyID=@p1 and N_CountryID=@p2 order by N_CityID";  
          }
          else{
               sqlCommandText = "select * from Acc_City where N_CompanyID=@p1 order by N_CityID";
          }
          
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nCountryID);
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
        public ActionResult GetCityDetails (int nCityID)
        {   
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt=new DataTable();
            SortedList Params = new SortedList();
            
            string sqlCommandText="select * from vw_Acc_City where N_CompanyID=@p1 and N_CityID=@p2 ";
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nCityID);
            
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
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nCityID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CityID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string CityCode = "";
                    var values = MasterTable.Rows[0]["x_CityCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.N_FormID);
                      
                        CityCode = dLayer.GetAutoNumber("Acc_City", "X_CityCode", Params, connection, transaction);
                        if (CityCode == "") { transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to generate City Code")); }
                        MasterTable.Rows[0]["x_CityCode"] = CityCode;
                    }
                    MasterTable.Columns.Remove("n_FnYearID");

                    nCityID = dLayer.SaveData("Acc_City", "N_CityID", MasterTable, connection, transaction);
                    if (nCityID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("City Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(User,ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCityID)
        {
            int Results=0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                connection.Open();
                Results=dLayer.DeleteData("Acc_City","N_CityID",nCityID,"",connection);

                    if(Results>0){
                        return Ok(_api.Success("City deleted" ));
                    } else {
                        return Ok(_api.Warning("Unable to delete City" ));
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