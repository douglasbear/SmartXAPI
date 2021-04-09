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
    [Route("tenderinformation")]
    [ApiController]
    public class Tender_Information : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 960;

        public Tender_Information(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


         [HttpGet("Dashboardlist")]
        public ActionResult TenderDashboardList(int nPage,int nSizeperpage,string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (N_TenderID like '%" + xSearchkey + "%'or X_TenderCode like '%" + xSearchkey + "%' or  X_TenderName like '%" + xSearchkey + "%' )";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_TenderID desc";
            else
            
             xSortBy = " order by " + xSortBy;
             if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +") X_TenderCode,X_CustomerName,X_ProjectName,ProjectType,X_ProjectPlace,EnquiryType,N_ProposedAmt,X_Type from vw_Prj_Tender where N_CompanyID=@p1  ";
            else
                sqlCommandText = "select top("+ nSizeperpage +") X_TenderCode,X_CustomerName,X_ProjectName,ProjectType,X_ProjectPlace,EnquiryType,N_ProposedAmt,X_Type from vw_Prj_Tender where N_CompanyID=@p1  and N_TenderID not in (select top("+ Count +") N_TenderID from vw_Prj_Tender where N_CompanyID=@p1 )";
            Params.Add("@p1", nCompanyId);
         

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_Prj_Tender where N_CompanyID=@p1 ";
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
      


       [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {

                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nTenderID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TenderID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string TenderCode = "";
                    var values = MasterTable.Rows[0]["X_TenderCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        TenderCode = dLayer.GetAutoNumber("Prj_Tender", "X_TenderCode", Params, connection, transaction);
                        if (TenderCode == "") { transaction.Rollback(); return Ok(api.Error("Unable to generate TenderCode")); }
                        MasterTable.Rows[0]["X_TenderCode"] = TenderCode;
                    }


                    nTenderID = dLayer.SaveData("Prj_Tender", "N_TenderID", MasterTable, connection, transaction);
                    if (nTenderID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("TenderInformation Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }


        [HttpGet("details")]
        public ActionResult TenderInformationDetails(string xTenderCode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_Prj_Tender where N_CompanyID=@p1 and x_TenderCode=@p3";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p3",xTenderCode );
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
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nTenderid)
        {

             int Results = 0;
            try
            {                        
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Prj_Tender ", "N_TenderID", nTenderid, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_TenderID",nTenderid.ToString());
                    return Ok(api.Success(res,"Tender Information deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete Tender Information"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }



        }
    }
}