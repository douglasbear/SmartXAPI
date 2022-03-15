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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("myReminder")]
    [ApiController]

    public class myReminder : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public myReminder(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1406;

        }


        [HttpGet("dashboardList")]
   public ActionResult myReminderList(int nPage,int nSizeperpage,string xSearchkey, string xSortBy,bool x_Expire)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText ="";
             string sqlCommandCount = "";
              string Criteria = "";
             int Count= (nPage - 1) * nSizeperpage;
            int nCompanyID = myFunctions.GetCompanyID(User);
              Params.Add("@nCompanyID", nCompanyID);

          if (x_Expire == true)
            {
                Criteria = "and N_ReminderId=@nReminderId ";
            }
           


             if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ReminderId desc";
            else
            
             xSortBy = " order by " + xSortBy;
             if (x_Expire == true)
            {
                if (Count == 0)
                    sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Gen_ReminderDashboardExpired where N_CompanyID=@nCompanyID  "  + Criteria  + xSortBy;
                else
                    sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Gen_ReminderDashboardExpired where N_CompanyID=@nCompanyID"  + Criteria  + " and N_ReminderId not in (select top(" + Count + ") N_ReminderId from vw_Gen_ReminderDashboardExpired where N_CompanyID=@nCompanyID " + Criteria  + xSortBy + " ) " + xSortBy;
            }
            else{
                 if (Count == 0)
                 sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Gen_ReminderDashboard where N_CompanyID=@nCompanyID  "  + Criteria  + xSortBy;
                  else
                    sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Gen_ReminderDashboard where N_CompanyID=@nCompanyID"  + Criteria  + " and N_ReminderId not in (select top(" + Count + ") N_ReminderId from vw_Gen_ReminderDashboard where N_CompanyID=@nCompanyID " + Criteria  + xSortBy + " ) " + xSortBy;
            }

            SortedList OutPut = new SortedList();

           try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_Gen_ReminderDashboardExpired where N_CompanyID=@nCompanyID ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
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
                return Ok(_api.Error(User, e));
            }
        }
    }
}
    