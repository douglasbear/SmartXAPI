using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("terms")]
    [ApiController]
    public class Inv_Terms : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID;

        public Inv_Terms(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 202;//form id of cost center
        }

        
        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    
                    int N_TermsID = dLayer.SaveData("Inv_TermsMaster", "N_TermsID", MasterTable, connection, transaction);
                    if (N_TermsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                    }

                    return Ok(_api.Success("Terms Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }



        [HttpDelete("delete")]
        public ActionResult DeleteData(int nDepartmentID, int nFnYearID)
        {
            int Results = 0;
             int nCompanyID=myFunctions.GetCompanyID(User);
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nCostCentreID", nDepartmentID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object Objcount = dLayer.ExecuteScalar("Select count(*) From vw_Acc_CostCentreMaster_List where N_CostCentreID=@nCostCentreID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection);
                    if (Objcount != null)
                    {
                        if (myFunctions.getIntVAL(Objcount.ToString()) <= 0)
                        {
                            Results = dLayer.DeleteData("Acc_CostCentreMaster", "N_CostCentreID", nDepartmentID, "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection);
                        }
                        else
                        {
                             return Ok(_api.Error("Department Allready Used"));
                        }
                    }
                }
                if (Results > 0)
                {
                    return Ok(_api.Success("Department/Cost centre deleted"));
                }
                else
                {
                    return Ok(_api.Error("Unable to delete"));
                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }

        }
    }
}
