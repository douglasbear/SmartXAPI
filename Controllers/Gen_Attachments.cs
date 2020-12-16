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
    [Route("genAttachments")]
    [ApiController]
    public class Gen_Attachments : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Gen_Attachments(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 0;
        }


        
        [HttpGet("AttachmentCategory")]
        public ActionResult CategoryList(int nFormID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int nCompanyID=myFunctions.GetCompanyID(User);
  
            string sqlCommandText = "";
            if (nFormID > 0)
                sqlCommandText = "Select distinct X_Category From Inv_AttachmentCategory where N_CompanyID=@nCompanyID and ( N_FormID=@nFormID or N_FormID is null )";
            else
                sqlCommandText = "Select distinct X_Category From Inv_AttachmentCategory where N_CompanyID=@nCompanyID and N_FormID is null";
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFormID", nFormID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                }
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
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpGet("ReminderCategory")]
        public ActionResult ReminderCategoryList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int nCompanyID=myFunctions.GetCompanyID(User);
            
            string sqlCommandText = "Select distinct X_Category From Dms_ReminderCategory where N_CompanyID=@nCompanyID";
            Params.Add("@nCompanyID", nCompanyID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                }
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
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }



       


    }
}