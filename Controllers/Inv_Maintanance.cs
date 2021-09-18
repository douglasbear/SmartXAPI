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
    [Route("maintanance")]
    [ApiController]
    public class Inv_Maintanance : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;

        public Inv_Maintanance(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1394;
        }
        private readonly string connectionString;
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nServiceID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ServiceID"].ToString());
                    string X_ServiceCode = MasterTable.Rows[0]["X_ServiceCode"].ToString();
                    if (nServiceID > 0)
                    {
                        dLayer.DeleteData("Inv_ServiceMaster", "N_ServiceID", nServiceID, "", connection, transaction);
                        dLayer.DeleteData("Inv_ServiceDetails", "N_ServiceID", nServiceID, "", connection, transaction);
                    }
                    DocNo = MasterRow["X_ServiceCode"].ToString();
                    if (X_ServiceCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", this.FormID);



                        while (true)
                        {

                            object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_ServiceMaster Where X_ServiceCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null) DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            break;
                        }
                        X_ServiceCode = DocNo;


                        if (X_ServiceCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate")); }
                        MasterTable.Rows[0]["X_ServiceCode"] = X_ServiceCode;


                    }

                    nServiceID = dLayer.SaveData("Inv_ServiceMaster", "N_ServiceID", MasterTable, connection, transaction);
                    if (nServiceID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }
                    for (int i = 0; i < DetailTable.Rows.Count; i++)
                    {
                        DetailTable.Rows[1]["N_ServiceID"] = nServiceID;
                    }
                    int nServiceDetailsID = dLayer.SaveData("Inv_ServiceDetails", "N_ServiceDetailsID", DetailTable, connection, transaction);
                    if (nServiceDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("list")]
        public ActionResult ProductionOrderList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearId);
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ( X_ServiceCode like '%" + xSearchkey + "%' or  D_EntryDate like '%" + xSearchkey + "%'  ) ";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ServiceID desc";
            // xSortBy = " order by batch desc,D_TransDate desc";
            else
                xSortBy = " order by " + xSortBy;
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ")  * from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2  " + Searchkey;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "and N_ServiceID not in (select top(" + Count + ") N_ServiceID from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2) " + Searchkey;


            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText + xSortBy, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey;
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
                return BadRequest(_api.Error(User, e));
            }
        }
        [HttpGet("details")]
        public ActionResult ServiceDetails(string xServiceCode)
        {


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    string Mastersql = "";
                    string DetailSql = "";
                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@xServiceCode", xServiceCode);
                    Mastersql = "select * from Vw_InvService where N_CompanyId=@nCompanyID and X_TaskCode=@xServiceCode  ";
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int ServiceID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ServiceID"].ToString());
                    MasterTable.AcceptChanges();
                    Params.Add("@nServiceID", ServiceID);
                    MasterTable = _api.Format(MasterTable, "Master");
                    //Detail
                    DetailSql = "select * from Vw_InvServiceDetails where N_CompanyId=@nCompanyID and N_ServiceID=@nServiceID ";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");

                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
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

