using AutoMapper;
using SmartxAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("vendor")]
    [ApiController]
    public class Inv_Vendor : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        public Inv_Vendor(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 52;
        }


        //GET api/customer/list?....
        [HttpGet("list")]
        public ActionResult GetVendorList(int? nCompanyId, int nFnYearId, bool bAllBranchesData, string vendorId, string qry, string msg)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string criteria = "";
            int nVendorId = 0;
            if (vendorId != "" && vendorId != null)
            {
                criteria = " and N_VendorID =@nVendorID ";
                nVendorId = myFunctions.getIntVAL(vendorId.ToString());
            }
            Params.Add("@nVendorID", nVendorId);

            string qryCriteria = "";
            if (qry != "" && qry != null)
            {
                qryCriteria = " and (X_VendorCode like @qry or X_VendorName like @qry ) ";
                Params.Add("@qry", "%" + qry + "%");
            }
            string sqlCommandText = "select TOP 20 * from vw_InvVendor where B_Inactive=@bInactive and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID " + criteria + " " + qryCriteria + " order by N_VendorID DESC";
            Params.Add("@bInactive", 0);
            Params.Add("@nCompanyID", nCompanyId);
            Params.Add("@nFnYearID", nFnYearId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    dt = _api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        if (nVendorId > 0)
                        {
                            bool B_IsUsed = false;
                            object objIsUsed = dLayer.ExecuteScalar("Select count(*) From Acc_VoucherDetails where N_AccID=@nVendorID and N_AccType=1", Params, connection);
                            if (objIsUsed != null)
                                if (myFunctions.getIntVAL(objIsUsed.ToString()) > 0)
                                    B_IsUsed = true;
                            myFunctions.AddNewColumnToDataTable(dt, "B_IsUsed", typeof(Boolean), B_IsUsed);

                            object objUsedCount = dLayer.ExecuteScalar("Select Count(*) from vw_Inv_CheckVendor Where N_CompanyID=@nCompanyID and N_VendorID=@nVendorID", Params, connection);
                            if (objUsedCount != null)
                                myFunctions.AddNewColumnToDataTable(dt, "N_UsedCount", typeof(int), myFunctions.getIntVAL(objUsedCount.ToString()));
                        }

                        if (msg == "")
                            return Ok(_api.Success(dt));
                        else
                            return Ok(_api.Success(dt, msg));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("listWithCurrency")]
        public ActionResult GetVendorDispList(int? nCompanyId, int nFnYearId, bool bAllBranchesData, string vendorId, string qry)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string criteria = "";
            int nVendorId = 0;
            if (vendorId != "" && vendorId != null)
            {
                criteria = " and N_VendorID =@nVendorID ";
                nVendorId = myFunctions.getIntVAL(vendorId.ToString());
            }
            Params.Add("@nVendorID", nVendorId);

            string qryCriteria = "";
            if (qry != "" && qry != null)
            {
                qryCriteria = " and (X_VendorCode like @qry or X_VendorName like @qry ) ";
                Params.Add("@qry", "%" + qry + "%");
            }
            string sqlCommandText = "SELECT Top 20 X_VendorCode, X_VendorName, X_ContactName, X_PhoneNo1, X_PhoneNo2, N_LedgerID, N_InvDueDays, N_FnYearID, N_CurrencyID, B_DirPosting, N_TypeID,X_CountryCode, X_ReminderMsg, X_CurrencyCode, X_CurrencyName, N_ExchangeRate, N_CountryID, B_AllowCashPay, X_TaxRegistrationNo, X_Country, N_VendorID, N_CompanyID FROM vw_Inv_VendorCurrency_Disp where B_Inactive=@bInactive and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID " + criteria + " " + qryCriteria + " order by X_VendorName,X_VendorCode";
            Params.Add("@bInactive", 0);
            Params.Add("@nCompanyID", nCompanyId);
            Params.Add("@nFnYearID", nFnYearId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    dt = _api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(dt));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
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
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();
                // Auto Gen
                DataRow MasterRow = MasterTable.Rows[0];
                string VendorCode = "";
                var xVendorCode = MasterRow["x_VendorCode"].ToString();
                int nVendorID = myFunctions.getIntVAL(MasterRow["n_VendorID"].ToString());
                int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyId"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                QueryParams.Add("@nCompanyID", MasterRow["n_CompanyId"].ToString());
                QueryParams.Add("@nFnYearID", MasterRow["n_FnYearId"].ToString());
                QueryParams.Add("@nFormID", 52);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (myFunctions.getBoolVAL(myFunctions.checkProcessed("Acc_FnYear", "B_YearEndProcess", "N_FnYearID", "@nFnYearID", "N_CompanyID=@nCompanyID ", QueryParams, dLayer, connection)))
                        return Ok(_api.Warning("Year is closed, Cannot create new Vendor..."));

                    SqlTransaction transaction = connection.BeginTransaction(); ;
                    if (xVendorCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        VendorCode = dLayer.GetAutoNumber("Inv_Vendor", "x_VendorCode", Params, connection, transaction);
                        if (VendorCode == "") { return Ok(_api.Error("Unable to save")); }
                        MasterTable.Rows[0]["x_VendorCode"] = VendorCode;
                    }
                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and X_VendorCode='" + xVendorCode + "'";
                    string X_Crieteria="N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                    nVendorID = dLayer.SaveData("Inv_Vendor", "N_VendorID",DupCriteria,X_Crieteria, MasterTable, connection, transaction);
                    if (nVendorID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {

                        SortedList nParams = new SortedList();
                        nParams.Add("@nCompanyID", nCompanyID);
                        nParams.Add("@nFnYearID", nFnYearID);
                        nParams.Add("@nVendorID", nVendorID);
                        string sqlCommandText = "select * from vw_InvVendor where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_VendorID =@nVendorID  order by X_VendorName,X_VendorCode";
                        DataTable outputDt = dLayer.ExecuteDataTable(sqlCommandText, nParams, connection, transaction);
                        outputDt = _api.Format(outputDt, "NewVendor");

                        if (outputDt.Rows.Count == 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable to save"));
                        }
                        DataRow NewRow = outputDt.Rows[0];
                        transaction.Commit();
                        return Ok(_api.Success(NewRow.Table, "Vendor successfully created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nVendorID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nFormID", 52);
                QueryParams.Add("@nVendorID", nVendorID);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (myFunctions.getBoolVAL(myFunctions.checkProcessed("Acc_FnYear", "B_YearEndProcess", "N_FnYearID", "@nFnYearID", "N_CompanyID=@nCompanyID ", QueryParams, dLayer, connection)))
                        return Ok(_api.Error("Year is closed, Cannot create new Vendor..."));

                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Inv_Vendor", "N_VendorID", nVendorID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("n_VendorID", nVendorID.ToString());
                    return Ok(_api.Success(res, "Vendor deleted"));
                }
                else
                {
                    return Ok(_api.Error("Unable to delete vendor"));
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("REFERENCE constraint"))
                    return Ok(_api.Error("Unable to delete vendor! It has been used."));
                else
                    return Ok(_api.Error(ex));
            }


        }
  [HttpGet("details")]
        public ActionResult ActivityListDetails(string xVendorCode,int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "Select * from vw_InvVendor Where N_CompanyID=@p1 and N_FnYearID=@nFnYearID and X_VendorCode=@xVendorCode";
            Params.Add("@p1", nCompanyId);
            Params.Add("@xVendorCode", xVendorCode);
            Params.Add("@nFnYearID", nFnYearID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

          [HttpGet("default")]
        public ActionResult GetDefault(int nFnYearID,int nLangID,int nFormID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID",myFunctions.GetCompanyID(User));

                    DataTable QList = myFunctions.GetSettingsTable();
                    QList.Rows.Add("DEFAULT_ACCOUNTS", "Creditor Account");
                    QList.Rows.Add("DEFAULT_ACCOUNTS", "S Cash Account");

                    QList.AcceptChanges();

                    DataTable Details = dLayer.ExecuteSettingsPro("SP_GenSettings_Disp", QList, myFunctions.GetCompanyID(User),nFnYearID, connection);

                        SortedList OutPut = new SortedList(){
                            {"settings",_api.Format(Details)}
                        };
                    return Ok(_api.Success(OutPut));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        
    }
}