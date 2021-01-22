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
        public ActionResult GetAllProjectTimesheet()
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText="select N_CompanyId,N_ProjectTimeSheetID,N_ProjectID from Pay_ProjectTimesheetEntry where N_CompanyId=@p1";
            Params.Add("@p1",nCompanyId);
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
                int nProjectTimeSheetID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ProjectTimeSheetID"].ToString());
                

                // if(xProjectCode== "@Auto")
                //     {
                //         Params.Add("N_CompanyID", nCompanyID);
                //         Params.Add("N_YearID", nFnYearID);
                //         Params.Add("N_FormID", this.FormID);
                //         xProjectCode = dLayer.GetAutoNumber("Pay_ProjectTimesheetEntry", "x_ProjectCode", Params, connection, transaction);
                //         if (xProjectCode == "") { return Ok(_api.Error("Unable to generate Project Code")); }
                //         MasterTable.Rows[0]["x_ProjectCode"] = xProjectCode;
                //     }
                //     else
                //     {
                //         dLayer.DeleteData("Pay_ProjectTimesheetEntry", "n_ProjectTimeSheetID", nProjectTimeSheetID, "", connection, transaction);
                        
                //     }
                    
                     
                    nProjectTimeSheetID = dLayer.SaveData("Pay_ProjectTimesheetEntry", "n_ProjectTimeSheetID", MasterTable, connection, transaction);
                    
                    
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
        public ActionResult DeleteData(int nProjectTimeSheetID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Pay_ProjectTimesheetEntry", "n_ProjectTimeSheetID", nProjectTimeSheetID, "", connection);
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
        public ActionResult GetDetails(int nProjectTimeSheetID)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText="select * from Pay_ProjectTimesheetEntry where N_CompanyID=@nCompanyID and N_ProjectTimeSheetID=@nProjectTimeSheetID";
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nProjectTimeSheetID",nProjectTimeSheetID);
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



       
