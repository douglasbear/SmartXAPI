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
    [Route("taskManager")]
    [ApiController]
    public class Tsk_TaskMaster : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        
        public Tsk_TaskMaster(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
             _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1056;
        }   
        private readonly string connectionString;
     
     
     
       [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nTaskId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TaskID"].ToString());
                    string X_TaskCode = MasterTable.Rows[0]["X_TaskCode"].ToString();
                    // int nUserID = myFunctions.getIntVAL(DetailTable.Rows[0]["n_UserID"].ToString());
             
                    if (nTaskId > 0)
                    {
                      SortedList deleteParams = new SortedList()
                            {
                                {"N_CompanyID",nCompanyID},
                                {"n_TaskID",nTaskId}
                            };
                        dLayer.ExecuteNonQueryPro("Tsk_TaskMaster", deleteParams, connection, transaction);
                        dLayer.ExecuteNonQueryPro("Tsk_TaskStatus", deleteParams, connection, transaction);
                    }
                    DocNo = MasterRow["X_TaskCode"].ToString();
                    if (X_TaskCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", this.FormID);
                                           
                        
                      while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Tsk_TaskMaster Where X_TaskCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_TaskCode=DocNo;


                        if (X_TaskCode == "") { transaction.Rollback();return Ok(_api.Error("Unable to generate")); }
                        MasterTable.Rows[0]["X_TaskCode"] = X_TaskCode;

                    }
                    else
                    {
                        dLayer.DeleteData("Tsk_TaskMaster", "N_TaskID", nTaskId, "", connection, transaction);
                    }
                     DetailTable.Columns.Remove("X_Assignee");
                     DetailTable.Columns.Remove("x_Closed");
                     DetailTable.Columns.Remove("x_Submitter");

                                                                                              
                    nTaskId=dLayer.SaveData("Tsk_TaskMaster","N_TaskID",MasterTable,connection,transaction);
                    if (nTaskId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    } 

                     for (int i = 0; i < DetailTable.Rows.Count; i++)
                     {
                        DetailTable.Rows[0]["N_TaskID"] = nTaskId;
                     }
                    int nTaskStatusID = dLayer.SaveData("Tsk_TaskStatus", "N_TaskStatusID", DetailTable, connection, transaction);
                    if (nTaskStatusID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    } 

                    transaction.Commit();
                    return Ok(_api.Success("Saved")) ;
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        } 
    }
}
