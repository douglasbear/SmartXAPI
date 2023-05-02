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
    [Route("address")]
    [ApiController]
    public class Inv_Address : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1720;
        public Inv_Address(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetAddressList(int nCountryID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable(); 
            SortedList Params = new SortedList();
            string sqlCommandText ;
          if(nCountryID>0)
          {
               sqlCommandText = "select * from Inv_Address where N_CompanyID=@p1 and N_CountryID=@p2 order by N_AddressID";  
          }
          else{
               sqlCommandText = "select * from Inv_Address where N_CompanyID=@p1 order by N_AddressID";
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
        public ActionResult GetAddressDetails (int nAddressID)
        {   
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt=new DataTable();
            SortedList Params = new SortedList();
            
            string sqlCommandText="select * from vw_Inv_Address where N_CompanyID=@p1 and N_AddressID=@p2 ";
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nAddressID);
            
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
                int nAddressID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_AddressID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string AddressCode = "";
                    var values = MasterTable.Rows[0]["x_AddressCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.N_FormID);
                      
                        AddressCode = dLayer.GetAutoNumber("Inv_Address", "X_AddressCode", Params, connection, transaction);
                        if (AddressCode == "") { transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to generate Address Code")); }
                        MasterTable.Rows[0]["x_AddressCode"] = AddressCode;
                    }
                    MasterTable.Columns.Remove("n_FnYearID");

                    nAddressID = dLayer.SaveData("Inv_Address", "N_AddressID", MasterTable, connection, transaction);
                    if (nAddressID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Address Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(User,ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nAddressID)
        {
            int Results=0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                connection.Open();
                Results=dLayer.DeleteData("Inv_Address","N_AddressID",nAddressID,"",connection);

                    if(Results>0){
                        return Ok(_api.Success("Address deleted"));
                    } else {
                        return Ok(_api.Warning("Unable to delete Address"));
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