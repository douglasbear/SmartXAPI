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
    [Route("projects")]
    [ApiController]
    public class InvCustomerProjectsController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
         private readonly IMyFunctions myFunctions;
          private readonly int N_FormID = 74;

        public InvCustomerProjectsController(IDataAccessLayer dl,IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions=myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        //GET api/Projects/list
        [HttpGet("list")]
        public ActionResult GetAllProjects(int? nCompanyId, int? nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from Vw_InvCustomerProjects where N_CompanyID=@p1 and N_FnYearID=@p2 order by X_ProjectCode";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearID);

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
               return Ok(api.Error(e));
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
                int nProjectID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ProjectID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string ProjectCode = "";
                    var values = MasterTable.Rows[0]["X_ProjectCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        ProjectCode = dLayer.GetAutoNumber("inv_CustomerProjects", "X_ProjectCode", Params, connection, transaction);
                        if (ProjectCode == "") { transaction.Rollback();return Ok(api.Error("Unable to generate Project Information")); }
                        MasterTable.Rows[0]["X_ProjectCode"] = ProjectCode;
                    }
                     MasterTable.Columns.Remove("n_FnYearId");


                    nProjectID = dLayer.SaveData("inv_CustomerProjects", "N_ProjectID", MasterTable, connection, transaction);
                    if (nProjectID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Project Information Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }

         [HttpDelete("delete")]
        public ActionResult DeleteData(int nProjectID)
        {

             int Results = 0;
            try
            {                        
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("inv_CustomerProjects", "N_ProjectID", nProjectID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_ProjectID",nProjectID.ToString());
                    return Ok(api.Success(res,"Project Information deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete Project Information"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }

        }

        
        [HttpGet("Details") ]
        public ActionResult GetCustomerProjectDetails (string xProjectCode,int nFnYearId)
          
        {   DataTable dt=new DataTable();
            SortedList Params = new SortedList();
             int nCompanyID=myFunctions.GetCompanyID(User);
              string sqlCommandText="select * from Vw_InvCustomerProjects  where N_CompanyID=@nCompanyID and N_FnYearID=@YearID  and X_ProjectCode=@xProjectCode";
               Params.Add("@nCompanyID",nCompanyID);
                Params.Add("@YearID", nFnYearId);
             Params.Add("@xProjectCode",xProjectCode);
            
            try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
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
         [HttpGet("AccountList") ]
        public ActionResult GetAccountList ()
        {    int nCompanyID=myFunctions.GetCompanyID(User);
  
            SortedList param = new SortedList(){{"@p1",nCompanyID}};
            
            DataTable dt=new DataTable();
            
            string sqlCommandText="select * from prj_AccountSettings where N_CompanyID=@p1";
                
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
    
    


    
    


       [HttpGet("dashboardlist")]
        public ActionResult ProjectList(int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyId=myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (x_projectcode like '%" + xSearchkey + "%'or x_projectname like'%" + xSearchkey + "%' or x_CustomerName like '%" + xSearchkey + "%' or x_District like '%" + xSearchkey + "%' or d_StartDate like '%" + xSearchkey + "%' or n_ContractAmt like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ProjectID desc";
            else
                xSortBy = " order by " + xSortBy;
             
             if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +") X_ProjectCode,X_ProjectName,X_CustomerName,X_District,X_Name,D_StartDate,N_ContractAmt,N_EstimateCost,AwardedBudget,ActualBudget,CommittedBudget,RemainingBudget,X_PO,N_Progress,N_CompanyID,N_Branchid,N_CustomerID,B_IsSaveDraft,B_Inactive,N_ProjectID from vw_InvProjectDashBoard where N_CompanyID=@p1 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top("+ nSizeperpage +") X_ProjectCode,X_ProjectName,X_CustomerName,X_District,X_Name,D_StartDate,N_ContractAmt,N_EstimateCost,AwardedBudget,ActualBudget,CommittedBudget,RemainingBudget,X_PO,N_Progress,N_CompanyID,N_Branchid,N_CustomerID,B_IsSaveDraft,B_Inactive,N_ProjectID from vw_InvProjectDashBoard where N_CompanyID=@p1 " + Searchkey + " and N_ProjectID not in (select top("+ Count +") N_ProjectID from vw_InvProjectDashBoard where N_CompanyID=@p1 "+Searchkey + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    string sqlCommandCount = "select count(*) as N_Count  from vw_InvProjectDashBoard where N_CompanyID=@p1 ";
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
                return Ok(api.Error(e));
            }
        }
    }
}
    
    
    
