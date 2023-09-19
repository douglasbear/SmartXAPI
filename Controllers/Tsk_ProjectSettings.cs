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
    [Route("projectSettings")]
    [ApiController]
    public class Tsk_ProjectSettings : ControllerBase
    {
 private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID=1803;

        public Tsk_ProjectSettings(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
           api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = 
            conf.GetConnectionString("SmartxConnection");
        }


         [HttpGet("ProjectList")]
        public ActionResult ProjectList()
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
                    string DetailSql = "select * from Inv_CustomerProjects where N_CompanyID=@p1";
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
                int nprjctSettingsID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PrjctSettingsID"].ToString());
                int nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_PrjctSettingsCode"].ToString();

                   
                      if (nUserID > 0)
                    {

                        
                        dLayer.ExecuteNonQuery("delete from Tsk_ProjectSettingsDetails Where N_CompanyID = "+nCompanyID+"  and N_UserID ="+nUserID+"", connection, transaction);
                        dLayer.ExecuteNonQuery("delete from Tsk_ProjectSettings Where N_CompanyID = "+nCompanyID+"  and N_UserID ="+nUserID+"", connection, transaction);

                    }

                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Tsk_ProjectSettings", "X_PrjctSettingsCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Route Code")); }
                        MasterTable.Rows[0]["X_PrjctSettingsCode"] = Code;
                    }
                    MasterTable.Columns.Remove("n_FnYearID");

                    nprjctSettingsID = dLayer.SaveData("Tsk_ProjectSettings", "N_PrjctSettingsID", MasterTable, connection, transaction);
                    if (nprjctSettingsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_PrjctSettingsID"] = nprjctSettingsID;
                       
                       
                    }
                    int nRouteDetailID = dLayer.SaveData("Tsk_ProjectSettingsDetails", "N_PrjctDetailID", DetailTable, connection, transaction);
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


        [HttpGet("details")]
        public ActionResult prjctSettingsDetails(int nUserID)
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
                   
                    string DetailSql = "SELECT Inv_CustomerProjects.X_ProjectName, Tsk_ProjectSettingsDetails.N_ProjectID, Tsk_ProjectSettingsDetails.B_View, Tsk_ProjectSettingsDetails.B_Edit, Tsk_ProjectSettingsDetails.N_UserID,Tsk_ProjectSettingsDetails.N_CompanyID"+
                    " FROM Inv_CustomerProjects LEFT OUTER JOIN Tsk_ProjectSettingsDetails ON Inv_CustomerProjects.N_CompanyID = Tsk_ProjectSettingsDetails.N_CompanyID AND Inv_CustomerProjects.N_ProjectID = Tsk_ProjectSettingsDetails.N_ProjectID where Tsk_ProjectSettingsDetails.N_CompanyID=@p1 and Tsk_ProjectSettingsDetails.N_UserID=@p2";
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
    }
}
 
                