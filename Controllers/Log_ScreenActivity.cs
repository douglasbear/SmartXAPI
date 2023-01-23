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

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("log")]
    [ApiController]
    public class Log_ScreenActivity : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;

        public Log_ScreenActivity(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");

        }

        [HttpGet("list")]
        public ActionResult GetDashboardList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy, int nFnYearId)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_EntryForm like '%" + xSearchkey + "%' or X_DocNo like '%" + xSearchkey + "%' or X_ActionUser like '%" + xSearchkey + "%' or X_ActionType like '%" + xSearchkey + "%' or cast(D_ActionDate as VarChar) like '%" + xSearchkey + "%'";


            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by D_ActionDate desc,X_EntryForm,X_DocNo";
            else
            {
                xSortBy = " order by " + xSortBy;
            }
            string selection="X_EntryForm, X_DocNo, X_ActionUser, X_ActionType, D_ActionDate, X_SystemName, X_IP, X_Remarks,   REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(X_DataLog, ':::', ' - '),'~~~',CHAR(13)+CHAR(10)) ,'N_',''),'X_',''),'D_',''),'B_',''),'_',' ') as X_DataLog";

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") "+selection+" from Log_ScreenActivity where N_CompanyID=@p1 and N_FnYearID=@nFnYearId " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") "+selection+" from Log_ScreenActivity where N_CompanyID=@p1 and N_FnYearID=@nFnYearId " + Searchkey + " and N_ActionID not in (select top(" + Count + ") N_ActionID from Log_ScreenActivity where N_CompanyID=@p1 and N_FnYearID=@nFnYearId " + Searchkey + xSortBy + " ) " + xSortBy;;

            Params.Add("@p1", nCompanyID);
            Params.Add("@nFnYearId", nFnYearId);

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    string sqlCommandCount = "select count(*) as N_Count  from Log_ScreenActivity where N_CompanyID=@p1  and N_FnYearID=@nFnYearId " + Searchkey + "";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    return Ok(_api.Success(OutPut));                
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

    }
}