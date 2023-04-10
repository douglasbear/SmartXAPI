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
using System.IO;

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

        private readonly IMyAttachments myAttachments;
        public Gen_Attachments(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 0;
        }



        [HttpGet("AttachmentCategory")]
        public ActionResult CategoryList(int nFormID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlCommandText = "";
            if (nFormID > 0)
                sqlCommandText = "Select N_CategoryID,X_Category From Inv_AttachmentCategory where N_CompanyID=@nCompanyID and ( N_FormID=@nFormID or N_FormID is null )";
            else
                sqlCommandText = "Select N_CategoryID,X_Category From Inv_AttachmentCategory where N_CompanyID=@nCompanyID and N_FormID is null";
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFormID", nFormID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("ReminderCategory")]
        public ActionResult ReminderCategoryList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlCommandText = "Select N_CategoryID,X_Category From Dms_ReminderCategory where N_CompanyID=@nCompanyID";
            Params.Add("@nCompanyID", nCompanyID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(api.Error(User, e));
            }
        }

        // Delete....
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nAttachmentId, int formId, int nFnyearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nUserID = myFunctions.GetUserID(User);
                    // if(formId==113)
                    // {
                    //      dLayer.DeleteData("Acc_CompanyAttachments", "N_AttachmentID", nAttachmentId, "N_CompanyID=" + nCompanyID + " and N_FnyearID=" + nFnyearID, connection, transaction);
                    // }
                    // else
                    // {
                        myAttachments.DeleteAttachment(dLayer, formId, 2, nFnyearID, nAttachmentId, User, transaction, connection);
                    //}
                  
                    transaction.Commit();
                    return Ok(api.Success("Attachment Deleted"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }

        }







    }
}