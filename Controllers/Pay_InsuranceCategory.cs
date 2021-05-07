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
    [Route("insuranceCategory")]
    [ApiController]
    public class InsuranceCategory : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public InsuranceCategory(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1133;
        }

        [HttpGet("list")]
        public ActionResult GetInsuranceCategoryList()
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1", nCompanyID);

            string sqlCommandText = "Select * from vw_Pay_InsuranceCategory Where N_CompanyID=@p1 order by N_CategoryId";

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    
                    dt=_api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(dt));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
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
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    int nCategoryId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CategoryId"].ToString());
                    string xCategoryCode = MasterTable.Rows[0]["x_CategoryCode"].ToString();
                    MasterTable.Columns.Remove("n_FnYearID");
                    if (xCategoryCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        xCategoryCode = dLayer.GetAutoNumber("Pay_InsuranceCategory", "x_CategoryCode", Params, connection, transaction);
                        if (xCategoryCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Insurance Category Code")); }
                        MasterTable.Rows[0]["x_CategoryCode"] = xCategoryCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Pay_InsuranceCategory", "N_CategoryId", nCategoryId, "", connection, transaction);
                    }
                    
                    nCategoryId=dLayer.SaveData("Pay_InsuranceCategory","N_CategoryId",MasterTable,connection,transaction);  
                    transaction.Commit();
                    return Ok(_api.Success("Insurance Category Saved")) ;
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpGet("details")]
        public ActionResult GetInsuranceCategoryDetails(string xCategoryCode)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText="select * from Pay_InsuranceCategory where N_CompanyID=@p1 and X_CategoryCode=@p2";
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",xCategoryCode);
            try{
                using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection); 
                    }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(_api.Notice("No Results Found" ));
                        }else{
                            return Ok(_api.Success(dt));
                        }
            }catch(Exception e){
                return Ok(_api.Error(e));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCategoryId)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Pay_InsuranceCategory", "N_CategoryId", nCategoryId, "", connection);
                    if (Results > 0)
                    {
                        return Ok( _api.Success("Insurance Category deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete Insurance Category"));
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
