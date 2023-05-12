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
     [Route("invMRNReturn")]
     [ApiController]
    public class InvMRNReturn : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
        private readonly IMyFunctions myFunctions;
        private readonly IApiFunctions _api;
        public InvMRNReturn(IDataAccessLayer dl,IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            _api = api;
            myFunctions=myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
           
        [HttpGet("dashboardList")]
        public ActionResult GetMRNReturnDashboardList(int nFormID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

           
            if (xSearchkey != null && xSearchkey.Trim() != "")
                      Searchkey = "and (X_ReturnNo like'%" + xSearchkey + "%'or X_VendorName like'%" + xSearchkey + "%' or REPLACE(CONVERT(varchar(11), D_ReturnDate, 106), ' ', '-') like'%" + xSearchkey + "%')";

         
                    if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_MRNReturnID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_ReturnNo":
                        xSortBy = "[X_ReturnNo] " + xSortBy.Split(" ")[1];
                        break;
                    case "d_ReturnDate":
                        xSortBy = "Cast([D_ReturnDate] as DateTime )" + xSortBy.Split(" ")[1];
                        break;
               
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_MRNReturn where N_CompanyID=@nCompanyID and N_FormID=@nFormID " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_MRNReturn where N_CompanyID=@nCompanyID and N_FormID=@nFormID " + Searchkey + " and N_MRNReturnID not in (select top(" + Count + ") N_MRNReturnID from vw_Inv_MRNReturn where N_CompanyID=@nCompanyID and N_FormID=@nFormID " + xSortBy + " ) " + " " + xSortBy;

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFormID", nFormID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(1) as N_Count  from vw_Inv_MRNReturn where N_CompanyID=@nCompanyID and N_FormID=@nFormID " + Searchkey + "";
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
                return Ok(api.Error(User, e));
            }
        }

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
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();

                    int nMRNReturnID = myFunctions.getIntVAL(MasterRow["n_MRNReturnID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int nFormID = myFunctions.getIntVAL(MasterRow["n_FormID"].ToString());
                    string xReturnNo = MasterRow["x_ReturnNo"].ToString();
                    int nMRNID = myFunctions.getIntVAL(MasterRow["n_MRNID"].ToString());

                    if (xReturnNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", nFormID);
                        xReturnNo = dLayer.GetAutoNumber("Inv_MRNReturn", "X_ReturnNo", Params, connection, transaction);
                        if (xReturnNo == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate MRN Return No.");
                        }
                        MasterTable.Rows[0]["X_ReturnNo"] = xReturnNo;
                    }

                    nMRNReturnID = dLayer.SaveData("Inv_MRNReturn", "N_MRNReturnID", "", "", MasterTable, connection, transaction);
                    if (nMRNReturnID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save MRN Return");
                    }
                    if (nMRNID > 0)
                    {
                        dLayer.ExecuteNonQuery("Update Inv_MRN Set N_Processed=1 Where N_MRNID=" + nMRNID + " and N_FnYearID=" + nFnYearID + " and N_CompanyID=" + nCompanyID, connection, transaction);
                    }
                    dLayer.DeleteData("Inv_MRNReturnDetails", "N_MRNReturnID", nMRNReturnID, "", connection, transaction);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_MRNReturnID"] = nMRNReturnID;
                    }
                    int nMRNReturnDetailsID = dLayer.SaveData("Inv_MRNReturnDetails", "N_MRNReturnDetailsID", DetailTable, connection, transaction);
                    if (nMRNReturnDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save MRN Return");
                    }
                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_MRNReturnID", nMRNReturnID);
                    Result.Add("x_ReturnNo", xReturnNo);
                    Result.Add("n_MRNReturnDetailsID", nMRNReturnDetailsID);

                    return Ok(_api.Success(Result, "MRN Return Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("details")]
        public ActionResult MRNReturnDetails(string xReturnNo, int nMRNID)
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
                    DataTable DataTable = new DataTable();

                    string Mastersql = "";
                    string DetailSql = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));

                    if (nMRNID > 0)
                    {
                        Params.Add("@nMRNID", nMRNID);
                        Mastersql = "select * from vw_MRNtoReturn where N_CompanyId=@nCompanyID and N_MRNID=@nMRNID";
                        MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = _api.Format(MasterTable, "Master");
                        DetailSql = "select * from vw_MRNtoReturnDetails where N_CompanyId=@nCompanyID and N_MRNID=@nMRNID";
                        DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dt.Tables.Add(MasterTable);
                        dt.Tables.Add(DetailTable);
                        return Ok(_api.Success(dt));
                    } else {
                        Params.Add("@xReturnNo", xReturnNo);
                        Mastersql = "select * from vw_Inv_MRNReturn where N_CompanyID=@nCompanyID and X_ReturnNo=@xReturnNo  ";
                   
                        MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        int nMRNReturnID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_MRNReturnID"].ToString());
                        Params.Add("@nMRNReturnID", nMRNReturnID);

                        MasterTable = _api.Format(MasterTable, "Master");
                        DetailSql = "select * from vw_Inv_MRNReturnDetails where N_CompanyID=@nCompanyID and N_MRNReturnID=@nMRNReturnID ";
                        DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                        DetailTable = _api.Format(DetailTable, "Details");
                        dt.Tables.Add(MasterTable);
                        dt.Tables.Add(DetailTable);
                        return Ok(_api.Success(dt));
                    };
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
            
        [HttpGet("list")]
        public ActionResult GetMRNReturnList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select * from vw_Inv_MRNReturn where N_CompanyID=@nComapnyID";
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
                return Ok(_api.Error(User,e));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nMRNReturnID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nMRNReturnID", nMRNReturnID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Inv_MRNReturn", "N_MRNReturnID", nMRNReturnID, "", connection);

                    if (Results > 0)
                    {
                        dLayer.DeleteData("Inv_MRNReturnDetails", "N_MRNReturnID", nMRNReturnID, "", connection);
                        return Ok(_api.Success("MRN Return deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

    }
}
    