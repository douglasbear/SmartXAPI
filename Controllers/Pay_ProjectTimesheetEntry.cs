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
         private readonly int FormID;

        public Pay_ProjectTimesheetEntry(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
             FormID = 370;

        }
 [HttpGet("list")]
        public ActionResult GetAllProjectTimesheet(int nComapanyId, int nEmpId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
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
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Prj_TimeSheet where N_CompanyID=@p1 and N_EmpID=@p3 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Prj_TimeSheet where N_CompanyID=@p1 and N_EmpID=@p3" + Searchkey + " and N_TimeSheetID not in (select top(" + Count + ") N_TimeSheetID from vw_Prj_TimeSheet where N_CompanyID=@p1 and N_EmpID=@p3 " + xSearchkey + xSortBy + " ) " + xSortBy;

            Params.Add("@p1", nCompanyId);
            Params.Add("@p3", nEmpId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount="select count(*) as N_Count from vw_Prj_TimeSheet where N_CompanyId=@p1 and N_EmpID=@p3 "+ Searchkey +" ";
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
                     DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable=ds.Tables["details"];
                    SortedList Params = new SortedList();
                    
                  int N_CompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                  int N_PrjTimeSheetID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PrjTimeSheetID"].ToString());
                  int N_UserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());
                   string X_PrjTimesheetCode = "";
                   var values = MasterTable.Rows[0]["X_PrjTimesheetCode"].ToString();

                     if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID",N_CompanyID);
                         Params.Add("N_PrjTimeSheetID",N_PrjTimeSheetID);
                          Params.Add("N_UserID",N_UserID);
                        Params.Add("N_FormID", FormID);
                    
                        X_PrjTimesheetCode = dLayer.GetAutoNumber("Prj_TimeSheetEntryMaster", "X_PrjTimesheetCode", Params, connection, transaction);
                        if (X_PrjTimesheetCode == "") {
                             transaction.Rollback(); 
                             return Ok(_api.Warning("Unable to generate"));
                         }
                        MasterTable.Rows[0]["X_PrjTimesheetCode"] = X_PrjTimesheetCode;
                    }
                    //  if (nTimeSheetID > 0)
                    // {
                    //     dLayer.DeleteData("Prj_TimeSheetEntry", "n_PrjTimeSheetID", nTimeSheetID, "n_CompanyID=" + nCompanyID + " and n_PrjTimeSheetID=" + nTimeSheetID, connection, transaction);
                    //     dLayer.DeleteData("Prj_TimeSheetEntryMaster", "n_PrjTimeSheetID", nTimeSheetID, "n_CompanyID=" + nCompanyID + " and n_PrjTimeSheetID=" + nTimeSheetID, connection, transaction);
                    // }
                
                    N_PrjTimeSheetID = dLayer.SaveData("Prj_TimeSheetEntryMaster", "N_PrjTimeSheetID", MasterTable, connection, transaction);

                      if (N_PrjTimeSheetID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_PrjTimeSheetID"] = N_PrjTimeSheetID;
                    }
                    int nPrjTimeSheetID = dLayer.SaveData("Prj_TimeSheetEntry", "N_PrjTimeSheetID", DetailTable, connection, transaction);
                    if (nPrjTimeSheetID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));

                    }

                     transaction.Commit();

                    SortedList Result = new SortedList();
                    Result.Add("N_PrjTimeSheetID", N_PrjTimeSheetID);
                    Result.Add("X_PrjTimesheetCode", X_PrjTimesheetCode);
                    return Ok(_api.Success(Result, "Project Time sheet saved"));
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
                    Results = dLayer.DeleteData("prj_timesheetEntry", "n_TimeSheetID", nTimeSheetID, "", connection);
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



       
