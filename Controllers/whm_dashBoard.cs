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
    [Route("whmdashBoard")]
    [ApiController]
    public class whmdashBoard : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public whmdashBoard(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


       [HttpGet("pendingAsn")]
        public ActionResult GetOrderList(int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
             int nCompanyID = myFunctions.GetCompanyID(User);

        

 
            string sqlpendingAsnList = "Select count(*) as N_Count  from vw_Wh_AsnMaster_Disp Where N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearId  + " and N_AsnID Not in (select N_AsnID from Wh_Grn  Where N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearId+" )";
            string sqlPendingPickup ="select count(*) as N_Pickup from Wh_PickList where N_CompanyID= " + nCompanyID + " and N_FnYearID="+nFnYearId +"and N_FormID=1460";
            string sqlPendingPicklist ="select count(*) as N_Pick from Wh_PickList where N_CompanyID= " + nCompanyID + " and N_FnYearID="+nFnYearId +"and N_FormID=1459";
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearId",nFnYearId);

            SortedList OutPut = new SortedList();
            DataTable PendingAsnList = new DataTable();
            DataTable PendingPickup = new DataTable();
            DataTable PendingPickList = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                     PendingAsnList = dLayer.ExecuteDataTable(sqlpendingAsnList, Params, connection);
                      PendingPickup = dLayer.ExecuteDataTable(sqlPendingPickup, Params, connection);
                       PendingPickList = dLayer.ExecuteDataTable(sqlPendingPicklist, Params, connection);

                }
                 PendingAsnList.AcceptChanges();
                 PendingPickup.AcceptChanges();
                 PendingPickList.AcceptChanges();

                 if (PendingAsnList.Rows.Count > 0) OutPut.Add("PendingAsnList", PendingAsnList);
                 if (PendingPickup.Rows.Count > 0) OutPut.Add("PendingPickup", PendingPickup);
                 if (PendingPickList.Rows.Count > 0) OutPut.Add("PendingPickList", PendingPickList);
  

                 return Ok(api.Success(OutPut));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        


    }

}