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
    [Route("trainingRequest")]
    [ApiController]
    public class PayEmpTrainingRequest : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
         private readonly IMyFunctions myFunctions;
          private readonly int N_FormID = 1085;

        public PayEmpTrainingRequest(IDataAccessLayer dl,IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions=myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nRequestID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_RequestID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string RequestCode = "";
                    var values = MasterTable.Rows[0]["X_RequestCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        RequestCode = dLayer.GetAutoNumber("Pay_TrainingRequest", "X_RequestCode", Params, connection, transaction);
                        if (RequestCode == "") { transaction.Rollback();return Ok(api.Error("Unable to generate Training Request")); }
                        MasterTable.Rows[0]["X_RequestCode"] = RequestCode;
                    }
                    
                     // MasterTable.Columns.Remove("n_UserID");

                    nRequestID = dLayer.SaveData("Pay_TrainingRequest", "N_RequestID", MasterTable, connection, transaction);
                    if (nRequestID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Training Request Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }



        [HttpGet("Details") ]
        public ActionResult GetTrainingRequestDetails (int xRequestCode)
          
        {   DataTable dt=new DataTable();
            SortedList Params = new SortedList();
             int nCompanyID=myFunctions.GetCompanyID(User);
              string sqlCommandText="select * from vw_TrainingRequest where N_CompanyID=@nCompanyID  and X_RequestCode=@xRequestCode";
               Params.Add("@nCompanyID",nCompanyID);
             Params.Add("@xRequestCode",xRequestCode);
            
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

        [HttpGet("list")]
        public ActionResult GetAllRequest(int? nCompanyID, int? nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_TrainingRequest where N_CompanyID=@p1 and N_FnYearID=@p2 order by X_RequestCode";
            Params.Add("@p1", nCompanyID);
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

         [HttpGet("dashboardlist")]
        public ActionResult TrainingList(int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyId=myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and x_RequestCode like '%" + xSearchkey + "%'or x_EmpName like'%" + xSearchkey + "%' or x_CourseName like '%" + xSearchkey + "%' or Date like '%" + xSearchkey + "%' ";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_RequestID desc";
            else
                xSortBy = " order by " + xSortBy;
             
             if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +")x_RequestCode,X_EmpName,X_CourseName,[Date] from vw_TrainingRequest where N_CompanyID=@p1 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top("+ nSizeperpage +")X_RequestCode,X_EmpName,X_CourseName,[Date] from vw_TrainingRequest where N_CompanyID=@p1  " + Searchkey + " and N_RequestID not in (select top("+ Count +") N_RequestID from vw_TrainingRequest where N_CompanyID=@p1 "+Searchkey + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    string sqlCommandCount = "select count(*) as N_Count  from vw_TrainingRequest where N_CompanyID=@p1 ";
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


         [HttpDelete("delete")]
        public ActionResult DeleteData(int nRequestID)
        {

             int Results = 0;
            try
            {                        
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Pay_TrainingRequest", "N_RequestID", nRequestID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_RequestID",nRequestID.ToString());
                    return Ok(api.Success(res,"Training Request deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete Training Request"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }

        }

    }
}
