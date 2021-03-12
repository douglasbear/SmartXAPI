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
    [Route("trainingRequest")]
    [ApiController]
    public class PayEmpTrainingRequest : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
         private readonly IMyFunctions myFunctions;
          private readonly int N_FormID = 1083;

        public PayEmpTrainingRequest(IDataAccessLayer dl,IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions=myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
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
                int nRequestID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_RequestID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string RequestCode = "";
                    var values = MasterTable.Rows[0]["X_RequestCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        RequestCode = dLayer.GetAutoNumber("Pay_TrainingRequest", "X_RequestCode", Params, connection, transaction);
                        if (RequestCode == "") { transaction.Rollback();return Ok(api.Error("Unable to generate Training Request")); }
                        MasterTable.Rows[0]["X_RequestCode"] = RequestCode;
                    }
                    


                    nRequestID = dLayer.SaveData("Pay_TrainingRequest", "N_RequestID", MasterTable, connection, transaction);
                    if (nRequestID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Training Request Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }

        [HttpGet("Details") ]
        public ActionResult GetTrainingRequestList ()
        {    int nCompanyID=myFunctions.GetCompanyID(User);
  
            SortedList param = new SortedList(){{"@p1",nCompanyID}};
            
            DataTable dt=new DataTable();
            
            string sqlCommandText="select * from vw_TrainingRequest where N_CompanyID=@p1";
                
            try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
                }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }else{
                            return Ok(api.Success(dt));
                        }
                
            }catch(Exception e){
                return Ok(api.Error(e));
            }   
        }


         [HttpDelete("delete")]
        public ActionResult DeleteData(int nRequestID)
        {

             int Results = 0;
            try
            {                        
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Pay_TrainingRequest", "N_RequestID", nRequestID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_RequestID",nRequestID.ToString());
                    return Ok(api.Success(res,"Training Request deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete Training Request"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }

        }

    }
}
