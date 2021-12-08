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
    [Route("dayReopen")]
    [ApiController]
    public class Acc_DayReopen : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Acc_DayReopen(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 763;
        }

        [HttpGet("list")]
        public ActionResult GetDayReopen(int nFnYearID,int nBranchID)
        {
            DataTable dt = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);
            SortedList Params = new SortedList();
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nFnYearID);
            Params.Add("@p3",nBranchID);

            SortedList Result = new SortedList();
            string sqlCommandText= "Select * From [vw_Acc_DayClosing] Where N_CompanyID=@p1 and N_FnYearID=@p2 and N_BranchId=@p3 and B_Closed='True' order by convert(datetime, D_ClosedDate, 103) DESC";
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
                    int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());

                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nBranchID);
                
                   for (int i = 0; i < MasterTable.Rows.Count; i++)
                    {
                        if (MasterTable.Rows[i]["b_Select"]=="True")
                        {
                            int N_ClosedID = myFunctions.getIntVAL(MasterTable.Rows[i]["n_CloseID"].ToString());
                            dLayer.ExecuteNonQuery("update  Acc_Dayclosing set B_Closed=0 where N_CloseID=" + N_ClosedID + " and N_CompanyID=@p1 and N_BranchID=@p2 ", Params, connection, transaction);
                        };
                    }
                        transaction.Commit();
                        return Ok(_api.Success("Saved")) ;
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }
    }
}