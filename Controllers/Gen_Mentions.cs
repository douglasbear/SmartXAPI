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
    [Route("genMentions")]
    [ApiController]
    public class Gen_mentions : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Gen_mentions(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 0;
        }
    


            [HttpGet("count")]
        public ActionResult MentionsCount()
        {
            SortedList Params = new SortedList();

            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlCommandText = "";
            sqlCommandText = "select count(1) from Gen_Mentions where N_CompanyID=@nCompanyID and N_UserID=@nUserID and B_HideNotification=0";
          
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nUserID", nUserID);

            try
            {
                int count = 0;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    count = myFunctions.getIntVAL(dLayer.ExecuteScalar(sqlCommandText, Params, connection).ToString());
                }
                SortedList res = new SortedList();
                res.Add("Count", count);
                return Ok(api.Success(res));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

            [HttpGet("mentionList")]
        public ActionResult GetMentionList(int nMentionType)
        {
            SortedList Params = new SortedList();
            DataTable dt = new DataTable();
            string sqlCommandText = "";

            if(nMentionType==1)
            {
                sqlCommandText = "select * from Vw_genMentionList where b_HideNotification=0 and N_CompanyID=@nCompanyID and N_UserID=@nUserID"; 
            }
            else if(nMentionType==2)
            {
                sqlCommandText = "select * from Vw_genMentionList where b_HideNotification=1 and N_CompanyID=@nCompanyID and N_UserID=@nUserID";  
            }        

            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);
            
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nUserID", nUserID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    
                    SortedList res = new SortedList();
                    res.Add("Details", api.Format(dt));
                    // if (dt.Rows.Count == 0)
                    // {
                    //     return Ok(api.Warning("No Results Found"));
                    // }
                    // else
                    // {
                        return Ok(api.Success(res));
                    // }
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("update")]
        public ActionResult ChangeData(string xDocCode)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();

                    string sqlCommandText = "";
                    sqlCommandText = "update Gen_Mentions set b_HideNotification=1 where N_CompanyID=@nCompanyID and N_UserID=@nUserID and X_DocCode=@xDocCode";     

                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nUserID = myFunctions.GetUserID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nUserID", nUserID);
                    Params.Add("@xDocCode", xDocCode);
                    
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection, transaction);

                    transaction.Commit();
                    return Ok(dt);
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }

    }
}
