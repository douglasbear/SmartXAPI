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
    [Route("accountMaster")]
    [ApiController]
    public class Acc_GroupandLedger : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Acc_GroupandLedger(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("group")]
        public ActionResult MasterGroupList(int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText ="";
            sqlCommandText = "Select * from Acc_MastGroup Where N_CompanyID= @p1 and N_FnYearID=@p2 Order By X_GroupCode";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(api.Format(dt)));
                    }

                }
                
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpGet("account")]
        public ActionResult MasterAccountist(int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText ="";
            sqlCommandText = "Select * from Acc_MastLedger Where N_CompanyID= @p1 and N_FnYearID=@p2 Order By N_GroupID";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(api.Format(dt)));
                    }

                }
                
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        //  [HttpGet("chart")]
        // public ActionResult ChartOfAccountList(int nFnYearId)
        // {
        //     DataTable dt = new DataTable();
        //     SortedList Params = new SortedList();
        //     int nCompanyId = myFunctions.GetCompanyID(User);
        //     string sqlCommandText ="";
        //     sqlCommandText = "Select * from Acc_MastGroup Where N_CompanyID= @p1 and N_FnYearID=@p2 and N_ParantGroup=0 Order By X_GroupCode";
        //     Params.Add("@p1", nCompanyId);
        //     Params.Add("@p2", nFnYearId);

        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
        //             if (dt.Rows.Count == 0)
        //             {
        //                 return Ok(api.Warning("No Results Found"));
        //             }
        //             else
        //             {
        //                 return Ok(api.Success(api.Format(dt)));
        //             }

        //         }
                
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(api.Error(e));
        //     }
        // }
    }
}