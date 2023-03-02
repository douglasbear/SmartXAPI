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

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("schAddFee")]
    [ApiController]
    public class Sch_AddFee : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;

        public Sch_AddFee(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 851;
        }
        private readonly string connectionString;

    
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    DataTable Master = ds.Tables["master"];
                    DataTable Details = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    DataRow MasterRow = Master.Rows[0];
                    int N_RefId = myFunctions.getIntVAL(MasterRow["N_RefId"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string X_InvoiceNo = MasterRow["X_InvoiceNo"].ToString();

                  
                    if (X_InvoiceNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", 851);
                        Params.Add("N_BranchID", 0);
                        X_InvoiceNo = dLayer.GetAutoNumber("Sch_Sales", "X_InvoiceNo", Params, connection, transaction);
                        if (X_InvoiceNo == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate");
                        }
                        Master.Rows[0]["X_InvoiceNo"] = X_InvoiceNo;
                    }
                    string DupCriteria = "";


                    N_RefId = dLayer.SaveData("Sch_Sales","N_RefId", DupCriteria, "", Master, connection, transaction);
                    if (N_RefId <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }

                    dLayer.SaveData("Sch_SalesDetails", "N_SalesDetailsID", Details, connection, transaction);
                    transaction.Commit();
                    SortedList Result = new SortedList();

                    return Ok(_api.Success(Result, "Add fee Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
          }
}