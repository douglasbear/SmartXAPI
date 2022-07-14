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
    [Route("schFeeSetup")]
    [ApiController]
    public class Sch_FeeSetup : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;

        public Sch_FeeSetup(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 159;
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
                    int N_FeeSetupID = myFunctions.getIntVAL(MasterRow["n_FeeSetupID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_AcYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string x_FeeSetupCode = MasterRow["x_FeeSetupCode"].ToString();

                    if (N_FeeSetupID > 0)
                    {
                        dLayer.DeleteData("Sch_ClassFeeSetupMaster", "N_FeeSetupID", N_FeeSetupID, "N_CompanyID=" + N_CompanyID + " and N_FeeSetupID=" + N_FnYearID + "", connection, transaction);
                        dLayer.DeleteData("Sch_ClassFeeSetup", "N_FeeSetupID", N_FeeSetupID, "N_CompanyID=" + N_CompanyID + "", connection, transaction);
                    }

                    if (x_FeeSetupCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", 159);
                        x_FeeSetupCode = dLayer.GetAutoNumber("Sch_ClassFeeSetupMaster", "x_FeeSetupCode", Params, connection, transaction);
                        if (x_FeeSetupCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Week Code");
                        }
                        Master.Rows[0]["x_FeeSetupCode"] = x_FeeSetupCode;
                    }
                    string DupCriteria = "";


                    N_FeeSetupID = dLayer.SaveData("Sch_ClassFeeSetupMaster", "N_FeeSetupID", DupCriteria, "", Master, connection, transaction);
                    if (N_FeeSetupID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                    for (int i = 0; i < Details.Rows.Count; i++)
                    {
                        Details.Rows[i]["N_FeeSetupID"] = N_FeeSetupID;

                    }

                    dLayer.SaveData("Sch_ClassFeeSetup", "N_ClassFeeID", Details, connection, transaction);
                    transaction.Commit();
                    SortedList Result = new SortedList();

                    return Ok(_api.Success(Result, "Fee Setup Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
    }
}

