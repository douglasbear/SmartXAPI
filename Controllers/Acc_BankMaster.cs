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
    [Route("banks")]
    [ApiController]
    public class Acc_BankMaster : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Acc_BankMaster(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 217;
        }

        [HttpGet("list")]
        public ActionResult GetBankList(int isCompany)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@isCompany",isCompany);
            string sqlCommandText=" select N_CompanyID,N_BankID,X_BankCode,X_BankName,X_BankNameLocale from Acc_BankMaster where N_CompanyID=@nCompanyID  and B_isCompany =@isCompany order by X_BankCode";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
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
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nBankID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BankID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                string xBankCode = MasterTable.Rows[0]["x_BankCode"].ToString();
                 if (xBankCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        xBankCode = dLayer.GetAutoNumber("Acc_BankMaster", "x_BankCode", Params, connection, transaction);
                        if (xBankCode == "") { return Ok(_api.Error("Unable to generate Bank Code")); }
                        MasterTable.Rows[0]["x_BankCode"] = xBankCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Acc_BankMaster", "N_BankID", nBankID, "", connection, transaction);
                    }
                    
                    nBankID=dLayer.SaveData("Acc_BankMaster","N_BankID",MasterTable,connection,transaction);  
                    transaction.Commit();
                    return Ok(_api.Success("Bank Saved")) ;
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        }
    }