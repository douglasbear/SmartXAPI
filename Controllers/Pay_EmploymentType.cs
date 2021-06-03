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
    [Route("employmentType")]
    [ApiController]
    public class Pay_EmploymentType : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Pay_EmploymentType(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1272;
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
                int nEmploymentID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_EmploymentID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                string xEmploymentCode = MasterTable.Rows[0]["x_EmploymentCode"].ToString();
                string bCreateEmpSeries = MasterTable.Rows[0]["b_CreateEmpSeries"].ToString();
                 if (xEmploymentCode == "@Auto")
                    {
                        if (bCreateEmpSeries=="True")
                        {
                            DataTable dt = new DataTable();
                            string xDescription=MasterTable.Rows[0]["x_Description"].ToString();
                            string xPrefix=MasterTable.Rows[0]["x_Prefix"].ToString();
                                                        SortedList proParams2 = new SortedList(){
                                        {"N_CompanyID",nCompanyID},
                                        {"N_FormID",1272},
                                        {"N_FnYearID",nFnYearID},
                                        {"N_BranchID",0},
                                        {"X_Prefix",xPrefix},
                                        {"X_Type",xDescription}};

                           dLayer.ExecuteScalarPro("Sp_CreateInvoiceCounter", proParams2, connection, transaction);

                        }
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        xEmploymentCode = dLayer.GetAutoNumber("Pay_EmploymentType", "x_EmploymentCode", Params, connection, transaction);
                        if (xEmploymentCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Employment Type Code")); }
                        MasterTable.Rows[0]["x_EmploymentCode"] = xEmploymentCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Pay_EmploymentType", "N_EmploymentID", nEmploymentID, "", connection, transaction);
                    }
                    
                    nEmploymentID=dLayer.SaveData("Pay_EmploymentType","N_EmploymentID",MasterTable,connection,transaction);  
                    transaction.Commit();
                    return Ok(_api.Success("Employment Type Saved")) ;
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpGet("details")]
        public ActionResult GetEmploymentTypeDetails(int nEmploymentID)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText="SELECT Pay_EmploymentType.*, Gen_Defaults.X_TypeName FROM Pay_EmploymentType INNER JOIN Gen_Defaults ON Pay_EmploymentType.N_TypeID = Gen_Defaults.N_TypeId where Pay_EmploymentType.N_CompanyID=@nCompanyID and Pay_EmploymentType.N_EmploymentID=@nEmploymentID";
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nEmploymentID",nEmploymentID);
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
        public ActionResult DeleteData(int nEmploymentID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Pay_EmploymentType", "N_EmploymentID", nEmploymentID, "", connection);
                    if (Results > 0)
                    {
                        return Ok( _api.Success("Employment Type deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete Employment Type"));
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