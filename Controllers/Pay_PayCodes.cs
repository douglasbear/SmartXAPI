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
using System.IO;
using System.Threading.Tasks;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("paycodes")]
    [ApiController]
    
    public class Pay_PayCodes : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
         private readonly int N_FormID =186 ;

        public Pay_PayCodes(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
         [HttpGet("details")]
        public ActionResult PayCodeDetails(string xPaycode,int n_FnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from  vw_Pay_PayMaster where N_CompanyID=@p1 and n_FnYearID=@p2 and X_PayCode=@p3";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", n_FnYearID);
            Params.Add("@p3", xPaycode);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }
        }

          //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nPayID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PayID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string PayCode = "";
                    var values = MasterTable.Rows[0]["X_PayCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        PayCode = dLayer.GetAutoNumber("Pay_PayMaster", "X_PayCode", Params, connection, transaction);
                        if (PayCode == "") { return Ok(api.Error("Unable to generate Pay Code")); }
                        MasterTable.Rows[0]["X_PayCode"] = PayCode;
                    }


                    nPayID = dLayer.SaveData("Pay_PayMaster", "N_PayID", MasterTable, connection, transaction);
                    if (nPayID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Pay Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }


        [HttpGet("list/{type}") ]
        public ActionResult GetPayCodeList (string type)
        {
            int id=0;
            switch(type){
                case "Gosi": id=14;
                break;
                
                default: return Ok("Invalid Type");
            }
            string X_Criteria="N_PayTypeID=@p1";
            SortedList param = new SortedList(){{"@p1",id}};
            
            DataTable dt=new DataTable();
            
            string sqlCommandText="select * from Pay_PayMaster where "+X_Criteria;
                
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

        [HttpGet("payCodeType")]
        public ActionResult GetPayCodeType()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            string sqlCommandText = "Select * from Pay_PayType where N_CompanyID=@nCompanyID order by N_PayTypeID";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPayCodeId)
        {

             int Results = 0;
            try
            {                        
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Pay_SummaryPercentage ", "N_PerCalcID", nPayCodeId, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_PayID",nPayCodeId.ToString());
                    return Ok(api.Success(res,"PayCode deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete PayCode"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }



        }

        [HttpGet("calculationMethod")]
        public ActionResult GetCalculationMethod()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "Select * from Pay_PayCalulationMethod where B_Active=1 order by N_SortOrder";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }
    } 
     
}

