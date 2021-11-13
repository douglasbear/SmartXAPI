using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("relationMaster")]
    [ApiController]
    public class RelationMaster : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;



        public RelationMaster(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
       
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                int nRelationID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_RelationID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                  
                    nRelationID = dLayer.SaveData("Pay_Relation", "N_RelationID", MasterTable, connection, transaction);
                    if (nRelationID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Relation Master Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(User,ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nRelationID)
        {
            int Results=0;

            try
            {
              using (SqlConnection connection = new SqlConnection(connectionString))
                {
                connection.Open();
                Results=dLayer.DeleteData("Pay_Relation","N_RelationID",nRelationID,"",connection);
                if(Results>0){
                    return Ok(_api.Success("Deleted Sucessfully" ));
                }else{
                    return Ok(_api.Warning("Unable to delete" ));
                }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("details") ]
        public ActionResult GetRelationDetails (int nRelationID, int nCompanyID)
          
        {   DataTable dt=new DataTable();
            SortedList Params = new SortedList();
           //  int nCompanyID=myFunctions.GetCompanyID(User);
            string sqlCommandText="select * from Pay_Relation where N_CompanyID=@nCompanyID  and N_RelationID=@nRelationID ";
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nRelationID",nRelationID);
            
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
                    }else{
                        return Ok(_api.Success(dt));
                    }
            }catch(Exception e){
                return Ok(_api.Error(User,e));
            }   
        }
    }
}