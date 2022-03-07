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
    [Route("invAsnReciept")]
    [ApiController]

    public class Inv_AsnReciept : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Inv_AsnReciept(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1406;

        }

        [HttpPost("save")]
       
          public ActionResult SaveData([FromBody] DataSet ds)
          {
               try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                       connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    SortedList Params = new SortedList();

                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int nAsnID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AsnID"].ToString());
                    int N_CustomerID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CustomerID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    string X_AsnDocNo = "";
                    var values = MasterTable.Rows[0]["X_AsnDocNo"].ToString();

                       if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", 370);
                        Params.Add("N_YearID", N_FnYearID);


                        X_AsnDocNo = dLayer.GetAutoNumber("Wh_AsnMaster", "X_AsnDocNo", Params, connection, transaction);
                        if (X_AsnDocNo == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Warning("Unable to generate"));
                        }
                        MasterTable.Rows[0]["X_AsnDocNo"] = X_AsnDocNo;
                    }

                     nAsnID = dLayer.SaveData("Wh_AsnMaster", "N_AsnID", MasterTable, connection, transaction);

                      if (nAsnID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                     for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_AsnDetailsID"] = nAsnID;
                    }

                     int N_AsnDetailsID = dLayer.SaveData("Wh_AsnDetails", "N_AsnDetailsID", DetailTable, connection, transaction);
                    if (N_AsnDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));

                    }

                     transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("N_AsnDetailsID", nAsnID);
                    Result.Add("X_AsnDocNo", X_AsnDocNo);
                    return Ok(_api.Success(Result, "Saved successfully"));
                }
            }
               catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
          }



    }
}