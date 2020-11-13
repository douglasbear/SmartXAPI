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
    [Route("crmcustomer")]
    [ApiController]
    public class CRM_Customer : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public CRM_Customer(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("list")]
        public ActionResult LeadList(int? nCompanyId, int nFnYearId,int nPage,int nSizeperpage)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string criteria = "";
            string sqlCommandCount = "";
            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
             
             if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_CRMCustomer where N_CompanyID=@p1 and N_FnyearID=@p2 ";
            else
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_CRMCustomer where N_CompanyID=@p1 and N_FnyearID=@p2 and N_CustomerID not in (select top("+ Count +") N_CustomerID from vw_CRMCustomer where N_CompanyID=@p1 and N_FnyearID=@p2)";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_CRMCustomer where N_CompanyID=@p1 and N_FnyearID=@p2";
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

        [HttpGet("listDetails")]
        public ActionResult LeadListDetails(int? nCompanyId, int nFnYearId,int nCustomerID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string criteria = "";
  
            string sqlCommandText = "select * from vw_CRMCustomer where N_CompanyID=@p1 and N_FnyearID=@p2 and N_CustomerID=@p3";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", nCustomerID);


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
                int nCustomerID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CustomerID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string LeadCode = "";
                    var values = MasterTable.Rows[0]["X_CustomerCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", 1306);
                        LeadCode = dLayer.GetAutoNumber("CRM_Customer", "X_CustomerCode", Params, connection, transaction);
                        if (LeadCode == "") { return Ok(api.Error("Unable to generate Customer Code")); }
                        MasterTable.Rows[0]["X_CustomerCode"] = LeadCode;
                    }
                    else
                    {
                        dLayer.DeleteData("CRM_Customer", "N_CustomerID", nCustomerID, "", connection, transaction);
                    }
                    MasterTable.Columns.Remove("N_CustomerID");
                    MasterTable.AcceptChanges();


                    nCustomerID = dLayer.SaveData("CRM_Customer", "N_CustomerID", nCustomerID, MasterTable, connection, transaction);
                    if (nCustomerID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return LeadListDetails(nCompanyID, nFnYearId, nCustomerID);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(api.Error(ex));
            }
        }

      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCustomerID,int nCompanyID, int nFnYearID)
        {

             int Results = 0;
            try
            {                        
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();                
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nFormID", 1305);
                QueryParams.Add("@nCustomerID", nCustomerID);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (myFunctions.getBoolVAL(myFunctions.checkProcessed("Acc_FnYear", "B_YearEndProcess", "N_FnYearID", "@nFnYearID", "N_CompanyID=@nCompanyID ", QueryParams, dLayer, connection)))
                        return Ok(api.Error("Year is closed, Cannot create new Lead..."));
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("CRM_Customer", "N_CustomerID", nCustomerID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_CustomerID",nCustomerID.ToString());
                    return Ok(api.Success(res,"Customer deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete Customer"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error("Unable to delete Customer"));
            }



        }
    }
}