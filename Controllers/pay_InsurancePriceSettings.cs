using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("insurancePriceSettings")]
    [ApiController]
    public class InsurancePriceSettings : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID;
      


        public InsurancePriceSettings(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 1135;
        }

        [HttpGet("list")]
        public ActionResult GetInsurancePriceSettingsList(int nFnYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearID);
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_InsuranceSettingsCode like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_InsuranceSettingsID desc";
            else
                xSortBy = " order by " + xSortBy;

            string sqlCommandText = " Select * from vw_InsuranceSettings Where N_CompanyID=@p1 and N_FnYearID=@p2 order by N_InsuranceSettingsID";

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    string sqlCommandCount = "select count(*) as N_Count from vw_InsuranceSettings where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    
                }
                dt=_api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(OutPut));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetInsuranceSettingsDetails(int nFnYearID, string xInsuranceSettingsCode)
        {
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            string Mastersql="Select * from vw_InsuranceSettings Where N_CompanyID=@p1 and N_FnYearID=@p2 and X_InsuranceSettingsCode=@p3";
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2", nFnYearID);
            Params.Add("@p3",xInsuranceSettingsCode);
            
            try{
                using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        MasterTable=dLayer.ExecuteDataTable(Mastersql,Params,connection); 

                        if (MasterTable.Rows.Count == 0)
                        {
                        return Ok(_api.Warning("No Data Found !!"));
                        }

                        MasterTable = _api.Format(MasterTable, "Master");
                        dt.Tables.Add(MasterTable);

                        int N_InsuranceSettingsID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_InsuranceSettingsID"].ToString());

                        string DetailSql = "select * from Pay_InsuranceSettingsDetails where N_CompanyID=" + nCompanyID + " and N_InsuranceSettingsID=" + N_InsuranceSettingsID;

                        DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dt.Tables.Add(DetailTable);
                    }
                    return Ok(_api.Success(dt));
                }
                catch (Exception e)
                {
                    return Ok(_api.Error(e));
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
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nInsuranceSettingsID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_InsuranceSettingsID"].ToString());
                int  nInsuranceSettingsDetailsID=0;
                

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                   
                    // Auto Gen
                    string InsuranceSettingsCode = "";
                    var values = MasterTable.Rows[0]["x_InsuranceSettingsCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("N_InsuranceSettingsID", nInsuranceSettingsID);
                        InsuranceSettingsCode = dLayer.GetAutoNumber("Pay_InsuranceSettings", "X_InsuranceSettingsCode", Params, connection, transaction);
                        if (InsuranceSettingsCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Grade Code")); }
                        MasterTable.Rows[0]["X_InsuranceSettingsCode"] = InsuranceSettingsCode;
                    }
                    nInsuranceSettingsID = dLayer.SaveData("Pay_InsuranceSettings", "N_InsuranceSettingsID", MasterTable, connection, transaction);
                    if (nInsuranceSettingsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    
                    dLayer.DeleteData("Pay_InsuranceSettingsDetails", "N_InsuranceSettingsID", nInsuranceSettingsID, "", connection, transaction);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                          nInsuranceSettingsDetailsID = dLayer.SaveData("Pay_InsuranceSettingsDetails", "N_InsuranceSettingsDetailsID", DetailTable, connection, transaction);
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Insurance Price Settings Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nInsuranceSettingsID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                SortedList Params = new SortedList();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                     Results = dLayer.DeleteData("Pay_InsuranceSettingsDetails", "N_InsuranceSettingsID", nInsuranceSettingsID, "", connection, transaction);
                     Results = dLayer.DeleteData("Pay_InsuranceSettings", "N_InsuranceSettingsID", nInsuranceSettingsID, "", connection, transaction);

                     transaction.Commit();
                }
                if (Results > 0)
                {                    
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_InsuranceSettingsID",nInsuranceSettingsID.ToString());
                    return Ok(_api.Success(res,"Insurance Price Settings Deleted"));
                }
                else
                {
                    return Ok(_api.Error("Unable to Delete"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

    }
}