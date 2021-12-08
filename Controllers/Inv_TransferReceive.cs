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
    [Route("InvtransferReceive")]
    [ApiController]
    public class Inv_TransferReceive : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;

        public Inv_TransferReceive(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1056;
        }
        private readonly string connectionString;

        [HttpGet("list")]
        public ActionResult TransferReceiveList(int nCompanyId, int nFnYearID, bool bAllBranchData, int nBranchID, int nLocationID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //int nCompanyId = myFunctions.GetCompanyID(User);
                    int nUserID = myFunctions.GetUserID(User);
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    string sqlCommandCount = "";
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string Criteria = "";
                    string Searchkey = "";
                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearID);
                    Params.Add("@p3", nBranchID);
                    Params.Add("@p4", nLocationID);


                    bool CheckClosedYear = Convert.ToBoolean(dLayer.ExecuteScalar("Select B_YearEndProcess From Acc_FnYear Where N_CompanyID=@p1 and N_FnYearID=@p2 ", Params, connection));

                    if (!CheckClosedYear)
                    {
                        if (bAllBranchData)
                            Criteria = "and N_FnYearID=@p2 and B_YearEndProcess=0 and N_CompanyID=@p1 ";
                        else
                            Criteria = "and N_FnYearID=@p2 and  B_YearEndProcess=0 and N_BranchID=@p3 and N_CompanyID=@p1 and N_LocationID=@p4";
                    }
                    else
                    {
                        if (bAllBranchData)
                            Criteria = "and N_PurchaseType=0 and X_TransType=@p4 and N_FnYearID=@p2 and N_CompanyID=@p1";
                        else
                            Criteria = "and N_PurchaseType=0 and X_TransType=@p4 and N_FnYearID=@p2 and N_LocationID=@p4 and N_BranchID=@p3 and N_CompanyID=@p1";
                    }


                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = "and [Site from] like '%" + xSearchkey + "%'";

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_ReceivableId asc";
                    else
                        xSortBy = " order by " + xSortBy;

                    if (Count == 0)
                    {

                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvReceivableStock_Search where N_CompanyID=@nCompanyId and N_FnYearID=" + nFnYearID + "   " + Searchkey + Criteria + xSortBy;

                    }
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvReceivableStock_Search where N_CompanyID=@nCompanyId " + Searchkey + Criteria + " and N_ReceivableId not in (select top(" + Count + ") N_ReceivableId from vw_Man_EmployeeMaintenance where N_CompanyID=@nCompanyId " + Criteria + xSortBy + " ) " + xSortBy;
                    Params.Add("@nCompanyId", nCompanyId);

                    SortedList OutPut = new SortedList();



                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_InvReceivableStock_Search where N_CompanyID=@nCompanyId " + Searchkey + Criteria;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        [HttpGet("transferList")]
        public ActionResult GettransferList( int nFnYearID )
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearID);
            string sqlCommandText = "select [reference No] as x_reference,* from vw_InvTransfer_Search where N_CompanyID=@p1 and N_FnYearID=@p2 and N_Processed=0";
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
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }



        
    }
}