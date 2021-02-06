using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("projectTimesheetEntry")]
    [ApiController]
    
        public class Pay_ProjectTimesheetEntry : ControllerBase
    {
         private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        // private readonly int FormID;

        public Pay_ProjectTimesheetEntry(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            // FormID = 370;

        }

       
       [HttpGet("list")]
        public ActionResult GetAllProjectTimesheet(int  nComapanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            Params.Add("@p1",nCompanyId); 
            Params.Add("@p2",nFnYearId); 
            string sqlCommandCount="";
            string sqlCommandText="";
            
            SortedList OutPut=new SortedList();

         
            sqlCommandText="select N_CompanyId,N_TimeSheetID,N_ProjectID,D_Date,X_Description,N_Hours,X_ProjectName,X_Name from vw_prj_timesheet where N_CompanyId=@p1 and N_FnYearID=@p2 ";

            try
            {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                            {                               
                                connection.Open();
                             
                                dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                                sqlCommandCount="select count(*) as N_Count from prj_timesheet where N_CompanyId=@p1 and N_FnYearID=@p2 ";
          
                                object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection); 
                              
                                OutPut.Add("Details", _api.Format(dt));
                                
                                OutPut.Add("TotalCount", TotalCount);
                                
                                
                            } 
                     dt=_api.Format(dt);

                    if(dt.Rows.Count==0)
                        {
                            return Ok(_api.Notice("No Results Found" ));
                        }else{
                            return Ok(_api.Success(OutPut)); 
                        }
            }catch(Exception e){
                return Ok(_api.Error(e));
            }
           
        }
        
          //Save....
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
                  int nTimeSheetID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_TimeSheetID"].ToString());
                
                    nTimeSheetID = dLayer.SaveData("prj_timesheet", "n_TimeSheetID", MasterTable, connection, transaction);
                    
                    transaction.Commit();
                    return Ok(_api.Success("Project Timesheet Saved")) ;
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }


         [HttpDelete("delete")]
        public ActionResult DeleteData(int nTimeSheetID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("prj_timesheet", "n_TimeSheetID", nTimeSheetID, "", connection);
                    if (Results > 0)
                    {
                        return Ok( _api.Success("deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete "));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpGet("details")]
        public ActionResult GetDetails(int nTimeSheetID)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText="select * from vw_prj_timesheet where N_CompanyID=@nCompanyID and N_TimeSheetID=@nTimeSheetID";
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nTimeSheetID",nTimeSheetID);
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
    }
}



       
