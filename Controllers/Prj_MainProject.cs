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
    [Route("mainproject")]
    [ApiController]
    public class Prj_MainProject : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1113;

        public Prj_MainProject(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        
        [HttpGet("details")]
        public ActionResult MainProjectListDetails(string xProjectNo)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_PrjMainProject where N_CompanyID=@p1 and n_MainProjectID=@p3";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p3",xProjectNo );
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return BadRequest(api.Error(e));
            }
        }


 [HttpGet("Type") ]
        public ActionResult GetContractTypeList ()
        {    int nCompanyID=myFunctions.GetCompanyID(User);
  
            SortedList param = new SortedList(){{"@p1",nCompanyID}};
            
            DataTable dt=new DataTable();
            
            string sqlCommandText="select * from Prj_ContractType where N_CompanyID=@p1";
                
            try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
                }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }else{
                            return Ok(api.Success(dt));
                        }
                
            }catch(Exception e){
                return Ok(api.Error(e));
            }   
        }
        
        [HttpGet("list") ]
        public ActionResult GetCustomerProjectList ()
        {    int nCompanyID=myFunctions.GetCompanyID(User);
  
            SortedList param = new SortedList(){{"@p1",nCompanyID}};
            
            DataTable dt=new DataTable();
            
            string sqlCommandText="select x_MainProjectName,N_MainProjectID from Prj_MainProject where N_CompanyID=@p1";
                
            try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
                }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }else{
                            return Ok(api.Success(dt));
                        }
                
            }catch(Exception e){
                return Ok(api.Error(e));
            }   
        }

         [HttpGet("Dashboardlist")]
        public ActionResult ContractDashboardList(int nPage,int nSizeperpage,string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (N_MainProjectID like '%" + xSearchkey + "%'or X_MainProjectCode like '%" + xSearchkey + "%' or  X_MainProjectName like '%" + xSearchkey + "%' or N_ContractAmt like '%" + xSearchkey + "%' or N_EstimateCost like '%" + xSearchkey + "%' or ActualBudget like '%" + xSearchkey + "%' or RemainingBudget like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_MainProjectID desc";
            else
            
             xSortBy = " order by " + xSortBy;
             if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +")  X_MainProjectCode,X_MainProjectName,N_ContractAmt,N_EstimateCost,ActualBudget,RemainingBudget from vw_InvMainProjectDashBoard where N_CompanyID=@p1 ";
            else
                sqlCommandText = "select top("+ nSizeperpage +")  X_MainProjectCode,X_MainProjectName,N_ContractAmt,N_EstimateCost,ActualBudget,RemainingBudget from vw_InvMainProjectDashBoard where N_CompanyID=@p1  and N_MainProjectID not in (select top("+ Count +") N_MainProjectID from vw_InvMainProjectDashBoard where N_CompanyID=@p1 )";
            Params.Add("@p1", nCompanyId);
         

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_InvMainProjectDashBoard where N_CompanyID=@p1 ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
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
                return BadRequest(api.Error(e));
            }
        }

        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nMainProjectID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_MAINPROJECTID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string ProjectCode = "";
                    var values = MasterTable.Rows[0]["X_MainProjectCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        ProjectCode = dLayer.GetAutoNumber("Prj_MainProject", "X_MainProjectCode", Params, connection, transaction);
                        if (ProjectCode == "") { transaction.Rollback();return Ok(api.Error("Unable to generate Main Project")); }
                        MasterTable.Rows[0]["X_MainProjectCode"] = ProjectCode;
                    }
                     MasterTable.Columns.Remove("n_FnYearId");


                    nMainProjectID = dLayer.SaveData("Prj_MainProject", "N_MainProjectID", MasterTable, connection, transaction);
                    if (nMainProjectID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Main Project Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }

      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nMainProjectID)
        {

             int Results = 0;
            try
            {                        
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Prj_MainProject", "N_MainProjectID", nMainProjectID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_MainProjectID",nMainProjectID.ToString());
                    return Ok(api.Success(res,"Main Project deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete Main Project"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }



        }
    }
}