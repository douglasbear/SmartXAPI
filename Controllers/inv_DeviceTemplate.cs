
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
     [Route("invDeviceTemplate")]
     [ApiController]
    public class invDeviceTemplateList : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
        private readonly IMyFunctions myFunctions;
        private readonly IApiFunctions _api;
        private readonly int nFormID = 1472;
        public invDeviceTemplateList(IDataAccessLayer dl,IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            _api = api;
            myFunctions=myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }



       [HttpGet("list")]
        public ActionResult invDeviceTypeList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            
           
            string sqlCommandText = "select  * from Inv_DeviceTemplate where N_CompanyID=@p1 order by N_TemplateID desc";
            Params.Add("@p1", nCompanyId);
           
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
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
                
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
       
          [HttpGet("fillDefaults")]
        public ActionResult invDeviceFillDefaults(int nTemplateID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            
           
            string sqlCommandText = "select  * from Inv_DeviceTemplateDetails where N_CompanyID=@p1 and N_TemplateID=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2",nTemplateID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
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
                
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
     }
}