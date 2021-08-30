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
        public ActionResult GetAllProjectTimesheet(int nFnYearId,int nComapanyId, int nEmpId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (N_TimeSheetID like '%" + xSearchkey + "%'or cast(D_Date as VarChar) like '%" + xSearchkey + "%' or  X_ProjectName like '%" + xSearchkey + "%' or X_Name like '%" + xSearchkey + "%' or N_Hours like '%" + xSearchkey + "%' or X_Description like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_TimeSheetID desc";
            else
            
             xSortBy = " order by " + xSortBy;
            


            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Prj_TimeSheet where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Prj_TimeSheet where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3" + Searchkey + " and N_TimeSheetID not in (select top(" + Count + ") N_TimeSheetID from vw_Prj_TimeSheet where N_CompanyID=@p1 and N_FnYearID=@p2 and N_EmpID=@p3 " + xSearchkey + xSortBy + " ) " + xSortBy;

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", nEmpId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount="select count(*) as N_Count from vw_Prj_TimeSheet where N_CompanyId=@p1 and N_FnYearID=@p2 and N_EmpID=@p3 "+ Searchkey +" ";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                   
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                      
                    }
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                   
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
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
                  int nEmpId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_EmpID"].ToString());
                  string hours = MasterTable.Rows[0]["n_Hours"].ToString();

                  string[] splitTime = hours.Split(".");

                  string TimeSpanFormat="00:00:00";
                  if(splitTime.Length==2)
                   TimeSpanFormat = splitTime[0]+":"+splitTime[1]+":00";
                   else
                   if(splitTime.Length==1)
                   TimeSpanFormat = splitTime[0]+":00:00";

                   MasterTable=myFunctions.AddNewColumnToDataTable(MasterTable,"D_Hours",typeof(string),TimeSpanFormat);
                   MasterTable.AcceptChanges();


                
                    nTimeSheetID = dLayer.SaveData("prj_timesheet", "n_TimeSheetID", MasterTable, connection, transaction);
                    
                    transaction.Commit();
                    return Ok(_api.Success("Project Timesheet Saved")) ;
                }
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("Conversion failed when converting date and/or time from character string"))
                return Ok(_api.Error(User,"Invalid Time Value"));
else
                return Ok(_api.Error(User,ex));
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
                return Ok(_api.Error(User,e));
            }
        }
    }
}



       
