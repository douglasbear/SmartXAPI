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
    [Route("country")]
    [ApiController]
    public class Acc_Country : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1234;
        public Acc_Country(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [AllowAnonymous]
        [HttpGet("list")]
        public ActionResult GetCountryList(int nCompanyId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "select X_CountryCode,X_CountryName,x_Currency,N_CompanyID,N_CountryID,B_TaxImplement from Acc_Country where N_CompanyID=@p1  order by N_CountryID";
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
  [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nCountryID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CountryID"].ToString());
               

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string CountryCode = "";
                    var values = MasterTable.Rows[0]["X_CountryCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                      
                        CountryCode = dLayer.GetAutoNumber("Acc_Country", "X_CountryCode", Params, connection, transaction);
                        if (CountryCode == "") { transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to generate Country Master")); }
                        MasterTable.Rows[0]["X_CountryCode"] = CountryCode;
                    }
                     MasterTable.Columns.Remove("n_FnYearId");
                     

                    string X_CountryName= MasterTable.Rows[0]["X_CountryName"].ToString();
                    string DupCriteria = "X_CountryName='" + X_CountryName + "'";

                    nCountryID = dLayer.SaveData("Acc_Country", "N_CountryID",DupCriteria,"", MasterTable, connection, transaction);
                    if (nCountryID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Country Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(User,ex));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCountryId)
        {
             int Results=0;
            try
            {
                                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                Results=dLayer.DeleteData("Acc_Country","N_CountryID",nCountryId,"",connection);
                if(Results>0){
                    return Ok(_api.Success("Country deleted" ));
                }else{
                    return Ok(_api.Warning("Unable to delete Country" ));
                }
                }
                
            }
            catch (Exception ex)
                {
                    return Ok(_api.Error(User,ex));
                }
        }
    
       [HttpGet("Details") ]
        public ActionResult GetCountryDetails (int nCountryID,int nCompanyID)
          
        {   DataTable dt=new DataTable();
            SortedList Params = new SortedList();
           //  int nCompanyID=myFunctions.GetCompanyID(User);
              string sqlCommandText="select * from Acc_Country where N_CompanyID=@nCompanyID  and N_CountryID=@nCountryID ";
               Params.Add("@nCompanyID",nCompanyID);
             Params.Add("@nCountryID",nCountryID);
            
            try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(_api.Notice("No Results Found"));
                        }else{
                            return Ok(_api.Success(dt));
                        }
                
            }catch(Exception e){
                return Ok(_api.Error(User,e));
            }   
        }

    }
}