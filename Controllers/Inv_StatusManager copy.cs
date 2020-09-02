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
    [Route("statementofaccounts")]
    [ApiController]



    public class Acc_StatementOfAccounts : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Acc_StatementOfAccounts(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 82;
        }

        [HttpGet("list")]
        public ActionResult GetStatementOfAccounts(int? nCompanyID,string xQuery)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@xQuery", "%"+xQuery+"%");


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    bool B_PartNo = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("82", "PartNo_InGrid", "N_Value",myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    object N_LocationCount = dLayer.ExecuteScalar("Select count(1) from inv_Location where  N_CompanyID=@nCompanyID",Params,connection);
                    string X_HideFieldList="",X_TableName="",X_VisibleFieldList="",X_Crieteria="";
                    if (myFunctions.getIntVAL(N_LocationCount.ToString()) > 1)
                    {

                            X_HideFieldList = "N_CompanyID,N_LedgerID";
                            X_TableName = "vw_StatementsOfAccounts";
                            X_VisibleFieldList = "X_LedgerCode,X_LedgerName,N_Debit,N_Credit,N_Balance";
                            X_Crieteria = "X_LedgerCode  LIKE @xQuery OR X_LedgerName LIKE @xQuery OR N_Debit LIKE @xQuery OR N_Credit LIKE @xQuery OR N_Balance LIKE @xQuery";

                    }else{
                        return Ok(_api.Notice("No Results Found"));
                    }
                    string sqlCommandText = "Select " + X_VisibleFieldList +","+X_HideFieldList+ " from " + X_TableName + " where " + X_Crieteria ;
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return BadRequest(_api.Error(e));
            }
        }

    




        }
    }