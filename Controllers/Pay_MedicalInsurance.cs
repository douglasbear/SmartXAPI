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
    [Route("medicalInsurance")]
    [ApiController]
    public class Pay_MedicalInsurance : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Pay_MedicalInsurance(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1105;
        }

        [HttpGet("list")]
        public ActionResult GetMedicalInsurance()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            string sqlCommandText="select * from vw_MedicalInsurance where N_CompanyID=@nCompanyID order by N_MedicalInsID";
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
                return Ok(_api.Error(User,e));
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
                int nInsuranceID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_MedicalInsID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                string xInsuranceCode = MasterTable.Rows[0]["x_InsuranceCode"].ToString();

                if(xInsuranceCode== "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        xInsuranceCode = dLayer.GetAutoNumber("Pay_Medical_Insurance", "x_InsuranceCode", Params, connection, transaction);
                        if (xInsuranceCode == "") { transaction.Rollback(); return Ok(_api.Error(User,"Unable to generate Insurance Code")); }
                        MasterTable.Rows[0]["x_InsuranceCode"] = xInsuranceCode;
                    }
                     MasterTable.Columns.Remove("n_FnYearID");
                    nInsuranceID = dLayer.SaveData("Pay_Medical_Insurance", "n_MedicalInsID", MasterTable, connection, transaction);
                   
                     if (nInsuranceID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Insurance Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }


         [HttpDelete("delete")]
        public ActionResult DeleteData(int InsuranceID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Pay_Medical_Insurance", "n_MedicalInsID", InsuranceID, "", connection);
                    if (Results > 0)
                    {
                        return Ok( _api.Success("Medical Insurances deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete "));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("details")]
        public ActionResult GetDetails(int nMedicalInsID )
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText="select * from vw_MedicalInsurance where N_CompanyID=@nCompanyID and N_MedicalInsID=@nMedicalInsID";
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nMedicalInsID",nMedicalInsID);
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
                return Ok(_api.Error(User,e));
            }
        }


    }
}