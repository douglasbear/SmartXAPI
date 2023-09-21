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
    [Route("teamSettings")]
    [ApiController]
    public class Tsk_TeamSettings : ControllerBase
    {
 private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID=1799;

        public Tsk_TeamSettings(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
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
                int nTeamSettingsID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TeamSettingsID"].ToString());
                int nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_TeamCode"].ToString();

                   
                      if (nUserID > 0)
                    {

                        
                        dLayer.ExecuteNonQuery("delete from Tsk_TeamSettingsDetails Where N_CompanyID = "+nCompanyID+"  and N_UserID ="+nUserID+"", connection, transaction);
                        dLayer.ExecuteNonQuery("delete from Tsk_TeamSettings Where N_CompanyID = "+nCompanyID+"  and N_UserID ="+nUserID+"", connection, transaction);

                    }

                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Tsk_TeamSettings", "X_TeamCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Route Code")); }
                        MasterTable.Rows[0]["X_TeamCode"] = Code;
                    }
                    MasterTable.Columns.Remove("n_FnYearID");

                    nTeamSettingsID = dLayer.SaveData("Tsk_TeamSettings", "N_TeamSettingsID", MasterTable, connection, transaction);
                    if (nTeamSettingsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_TeamSettingsID"] = nTeamSettingsID;
                       
                       
                    }
                    int nRouteDetailID = dLayer.SaveData("Tsk_TeamSettingsDetails", "N_TeamDetailID", DetailTable, connection, transaction);
                    // if (nRouteDetailID <= 0)
                    // {
                    //     transaction.Rollback();
                    //     return Ok("Unable to save ");
                    // }
                    transaction.Commit();
                    return Ok(api.Success("Saved Successfully"));

                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }


             
             [HttpGet("teamList")]
        public ActionResult teamList()
        {
            DataSet dt=new DataSet();
            DataTable DetailTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            
            Params.Add("@p1", nCompanyId);  
        
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // string usersID = "select n_usersID from tsk_UserMappingDetails where N_CompanyID=@p1 and N_UserID=@p2";
                    string DetailSql = "select * from tsk_usermapping where N_CompanyID=@p1 and ISNULL(X_Team,'')<>''";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);
                    return Ok(api.Success(dt));
                }           
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

                
             [HttpGet("details")]
        public ActionResult teamSettingsDetails(int nUserID)
        {
            DataSet dt=new DataSet();
            DataTable DetailTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            
            
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", nUserID);  
        
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();
                   
                    string DetailSql = "select tsk_usermapping.X_Team,Tsk_TeamSettingsDetails.n_TeamID,Tsk_TeamSettingsDetails.b_Edit,Tsk_TeamSettingsDetails.b_View,tsk_usermapping.N_UserMappingID" +
                    " from Tsk_TeamSettingsDetails left outer join tsk_usermapping ON Tsk_TeamSettingsDetails.N_TeamID=tsk_usermapping.n_UsermappingID "+
                    "and Tsk_TeamSettingsDetails.n_CompanyID=tsk_usermapping.n_CompanyID where Tsk_TeamSettingsDetails.N_CompanyID=@p1 and Tsk_TeamSettingsDetails.N_UserID=@p2";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = api.Format(DetailTable, "Details");

                    dt.Tables.Add(DetailTable);
                    return Ok(api.Success(dt));
                }           
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }


        
                [HttpGet("dashBoardList")]
        public ActionResult GetDashboardList(int? nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    DataTable CountTable = new DataTable();
                    SortedList Params = new SortedList();
                    DataSet dataSet = new DataSet();
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";

                    int nUserID = myFunctions.GetUserID(User);


                     if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and  ( X_TeamCode like'%" + xSearchkey + "%'or x_Team like'%" + xSearchkey + "%')";

                    if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_TeamCode desc";
                    else
                xSortBy = " order by " + xSortBy;

                    int Count = (nPage - 1) * nSizeperpage;
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") X_TeamCode AS X_TeamCode,* from Vw_TskTemSettings where   X_Team is  not null and N_CompanyID=@p1  " + Searchkey + " ";
                    else
                    sqlCommandText = "select top(" + nSizeperpage + ") X_TeamCode AS X_TeamCode,* from Vw_TskTemSettings where   X_Team is  not null and N_CompanyID=@p1  " + Searchkey + " ";
                        // sqlCommandText = "select top(" + nSizeperpage + ") X_TeamCode AS X_TeamCode,* from Vw_TskTemSettings where   X_Team is  not null and N_CompanyID=@p1 " + Searchkey + " and n_UserMappingID not in (select top(" + Count + ") n_UserMappingID from Vw_TskTemSettings where N_CompanyID=@p1 " + " ) ";

                    // sqlCommandText = "select * from Inv_MRNDetails where N_CompanyID=@p1";
                    Params.Add("@p1", nCompanyId);
                  // Params.Add("@p2", nFnYearId);
                    SortedList OutPut = new SortedList();


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count from Vw_TskTemSettings where N_CompanyID=@p1 " + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                    }
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);

                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
       
    }
}