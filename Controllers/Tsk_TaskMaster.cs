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
     
        [HttpGet("list")]
        public ActionResult TaskList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_TaskSummery like '% " + xSearchkey + " OR X_TaskDescription like '% " + xSearchkey + " OR X_Assignee like '% " + xSearchkey + " OR X_Submitter like '% " + xSearchkey + " OR X_ClosedUser like '% " + xSearchkey + " OR X_Status like '% " + xSearchkey + ")";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_TaskID desc";
            else
                xSortBy = " order by " + xSortBy;


            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Tsk_TaskCurrentStatus where N_CompanyID=@p1 ";
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Tsk_TaskCurrentStatus where N_CompanyID=@nCompanyId and  N_TaskID not in (select top(" + Count + ") N_TaskID from vw_Tsk_TaskCurrentStatus  where N_CompanyID=@p1 )";

            Params.Add("@p1", nCompanyId);
            // Params.Add("@nFnYearId", nFnYearId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from vw_Tsk_TaskCurrentStatus where N_CompanyID=@p1 ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
     
        [HttpGet("details")]
        public ActionResult TaskDetails(string xTaskCode)
        {


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable HistoryTable = new DataTable();

                    string Mastersql = "";
                    string DetailSql = "";
                    string HistorySql = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@xTaskCode", xTaskCode);
                    Mastersql = "select * from vw_Tsk_TaskMaster where N_CompanyId=@nCompanyID and X_TaskCode=@xTaskCode  ";

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int TaskID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TaskID"].ToString());
                    Params.Add("@nTaskID", TaskID);

                    MasterTable = _api.Format(MasterTable, "Master");
                    //Detail
                    DetailSql = "select * from vw_Tsk_TaskCurrentStatus where N_CompanyId=@nCompanyID and N_TaskID=@nTaskID ";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    //History
                    HistorySql = "select * from vw_Tsk_TaskStatus where N_CompanyId=@nCompanyID and N_TaskID=@nTaskID ";
                    HistoryTable = dLayer.ExecuteDataTable(HistorySql, Params, connection);
                    HistoryTable = _api.Format(HistoryTable, "History");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    dt.Tables.Add(HistoryTable);
                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }


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
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nTaskId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TaskID"].ToString());
                    string X_TaskCode = MasterTable.Rows[0]["X_TaskCode"].ToString();
                    //int nUserID = myFunctions.GetUserID(User);
             
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

        [HttpPost("UpdateStatus")]
        public ActionResult UpdateStatus([FromBody]DataSet ds)
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
                    DataRow MasterRow = MasterTable.Rows[0];
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nTaskID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TaskID"].ToString());
             
                     for (int i = 0; i < DetailTable.Rows.Count; i++)
                     {
                        DetailTable.Rows[0]["N_TaskID"] = nTaskID;
                        DetailTable.Rows[0]["N_TaskStatusID"] = 0;
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

         [HttpDelete("delete")]
        public ActionResult DeleteData(int nTaskID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    
                    connection.Open();
                    dLayer.DeleteData("Tsk_TaskStatus", "N_TaskID", nTaskID, "", connection);
                    Results = dLayer.DeleteData("Tsk_TaskMaster", "N_TaskID", nTaskID, "", connection);
                    if (Results > 0)
                    {
                        return Ok(_api.Success("deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
    }
}
