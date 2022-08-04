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
    [Route("schSubjectMapping")]
    [ApiController]
    public class sch_subjectMapping : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =1481 ;


        public sch_subjectMapping(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = 
            conf.GetConnectionString("SmartxConnection");
        }



           [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nSubMappingID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_SubMappingID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_SubMappingCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("sch_subjectMapping", "X_SubMappingCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Route Code")); }
                        MasterTable.Rows[0]["X_SubMappingCode"] = Code;
                    }
                    MasterTable.Columns.Remove("n_FnYearID");

                    if (nSubMappingID > 0) 
                    {  
                        dLayer.DeleteData("sch_subjectMapping", "N_SubMappingID", nSubMappingID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                        dLayer.DeleteData("Sch_SubjectMappingDetails", "N_SubMappingID", nSubMappingID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    }

                    nSubMappingID = dLayer.SaveData("sch_subjectMapping", "N_SubMappingID", MasterTable, connection, transaction);
                    if (nSubMappingID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_SubMappingID"] = nSubMappingID;
                    }
                    int nRouteDetailID = dLayer.SaveData("Sch_SubjectMappingDetails", "N_SubMappingDetailsID", DetailTable, connection, transaction);
                    if (nRouteDetailID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save ");
                    }
                    transaction.Commit();
                    return Ok(api.Success("Subject Created"));

                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }
    }
}