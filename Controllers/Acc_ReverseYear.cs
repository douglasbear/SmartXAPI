
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Net;
using System.IO;
using System.Drawing.Imaging;
using QRCoder;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("reverseYear")]
    [ApiController]
    public class Acc_ReverseYear : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int N_FormID;
        public Acc_ReverseYear(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 64;
        }





[HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
        try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                   SortedList PostingParam = new SortedList();
                   int Results = 0;
                   int N_CompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                   var nUserID = myFunctions.GetUserID(User);
                            PostingParam.Add("N_CompanyID", N_CompanyID);
                            PostingParam.Add("N_UserID", nUserID);
                            PostingParam.Add("@N_FnYearID_Close", nFnYearID);
                          
                            Results=dLayer.ExecuteNonQueryPro("SP_Acc_ReverseCloseFinYear", PostingParam, connection, transaction);

                             if (Results <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to Save"));
                        }
                        else{
                             transaction.Commit();
                            return Ok(_api.Success("Saved successfully"));

                        }
                }
            }
                            catch (Exception ex)
                            {
                                 return Ok(_api.Error(User, ex));
                            }
        }
    }
}
