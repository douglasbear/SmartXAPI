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
using System.Text.RegularExpressions;
namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("whpick")]
    [ApiController]
    public class WhPickList : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
        private readonly IMyFunctions myFunctions;
        private readonly IApiFunctions _api;

        public WhPickList(IDataAccessLayer dl, IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetDashboardList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy, int nFnYearID, int nFormID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_PickListCode like '%" + xSearchkey + "%' or X_CustomerName like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_PickListID desc";
            else
            {
                xSortBy = " order by " + xSortBy;
            }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_WhPickListMaster where N_CompanyID=@nCompanyID  and isnull(N_FormID,0)=@nFormID " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_WhPickListMaster where N_CompanyID=@nCompanyID  and isnull(N_FormID,0)=@nFormID " + Searchkey + " and N_PickListID not in (select top(" + Count + ") N_PickListID from vw_WhPickListMaster where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID " + Searchkey + xSortBy + " ) " + xSortBy;

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nFormID", nFormID);

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    string sqlCommandCount = "select count(1) as N_Count  from vw_WhPickListMaster where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and isnull(N_FormID,0)=@nFormID " + Searchkey + "";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Success(OutPut));
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
        [HttpGet("pickuplist")]
        public ActionResult GetDashboardList(int nFnYearID, string xbarcode,int nFormID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string query = "";
            if (xbarcode != "" && xbarcode != null)
                query = " and X_PickListCode=" + xbarcode;


            string sqlCommandText = "select * from vw_WhPickList where N_CompanyID=@nCompanyID and isnull(N_FormID,0)<>"+nFormID+" and N_PickListID not in (select isnull(n_RefID,0) from vw_WhPickList where N_CompanyID=@nCompanyID)" + query;

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Success(dt));
                    }
                    else
                    {
                        return Ok(_api.Success(dt));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        

        [HttpPost("Save")]
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

                    int nPickListID = myFunctions.getIntVAL(MasterRow["n_PickListID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string xPickListCode = MasterRow["x_PickListCode"].ToString();
                    int nFormID = myFunctions.getIntVAL(MasterRow["n_FormID"].ToString());

                    string x_PickListCode = "";
                     string i_Signature = "";
                     bool SigEnable = false;
                    if (xPickListCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", nFormID);
                        x_PickListCode = dLayer.GetAutoNumber("Wh_PickList", "X_PickListCode", Params, connection, transaction);
                        if (x_PickListCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Picklist Code");
                        }
                        MasterTable.Rows[0]["x_PickListCode"] = x_PickListCode;
                    }

                    if (nPickListID > 0)
                    {
                        dLayer.DeleteData("Wh_PickListDetails", "n_PickListID", nPickListID, "N_CompanyID=" + nCompanyID + " and n_PickListID=" + nPickListID, connection, transaction);
                        dLayer.DeleteData("Wh_PickList", "n_PickListID", nPickListID, "N_CompanyID=" + nCompanyID + " and n_PickListID=" + nPickListID, connection, transaction);
                    }
                   //Signature
                    Byte[] ImageBitmap = new Byte[i_Signature.Length];
                      if (MasterTable.Columns.Contains("i_signature"))
                    {
                    if(!MasterRow["i_signature"].ToString().Contains("undefined"))
                    {
                    i_Signature = Regex.Replace(MasterRow["i_signature"].ToString(), @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
                    if (myFunctions.ContainColumn("i_signature", MasterTable))
                        MasterTable.Columns.Remove("i_signature");
                    ImageBitmap = new Byte[i_Signature.Length];
                    ImageBitmap = Convert.FromBase64String(i_Signature);
                    SigEnable=true;
                    }
                    }

                    int n_PickListID = dLayer.SaveData("Wh_PickList", "N_PickListID", "", "", MasterTable, connection, transaction);
                     
                        
                    
                    
                   
                    //Saving Signature
                   if(SigEnable)
                   {
                    if (i_Signature.Length > 0)
                        dLayer.SaveImage("Wh_PickList", "i_signature", ImageBitmap, "N_PickListID", nPickListID, connection, transaction);
                    }

                    if (n_PickListID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Warehouse Picklist");
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_PickListID"] = n_PickListID;
                    }
                    int n_PickListDetailsID = dLayer.SaveData("Wh_PickListDetails", "N_PickListDetailsID", DetailTable, connection, transaction);
                    if (n_PickListDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Warehouse Picklist");
                    }

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_PickListID", n_PickListID);
                    Result.Add("x_PickListCode", x_PickListCode);
                    Result.Add("n_PickListDetailsID", n_PickListDetailsID);

                    return Ok(_api.Success(Result, "Warehouse Picklist Created"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpGet("details")]
        public ActionResult EmployeeEvaluation(string xPickListCode, bool bConvert)
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
                    Params.Add("@xPickListCode", xPickListCode);
                    if (bConvert)
                        Mastersql = "select * from vw_WhPickupListMaster where N_CompanyID=@nCompanyID and x_pickcode=@xPickListCode ";
                    else
                        Mastersql = "select * from vw_WhPickListMaster where N_CompanyID=@nCompanyID and X_PickListCode=@xPickListCode ";

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int nPickListID = 0;
                    if (bConvert)
                        nPickListID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PickID"].ToString());
                    else
                        nPickListID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PickListID"].ToString());
                    Params.Add("@nPickListID", nPickListID);

                    MasterTable = _api.Format(MasterTable, "Master");
                    if (bConvert)
                        DetailSql = "select * from vw_WhPickupListDetails where N_CompanyID=@nCompanyID and N_PickID=@nPickListID ";
                    else
                        DetailSql = "select * from vw_WhPickListDetails where N_CompanyID=@nCompanyID and N_PickListID=@nPickListID ";
                   
                    object Count = dLayer.ExecuteScalar("select count(1)  from Inv_TransferStock where N_CompanyID=@nCompanyID and N_PickListID ="+myFunctions.getIntVAL(MasterTable.Rows[0]["n_PickListID"].ToString())+" ", Params, connection);
                    if(myFunctions.getIntVAL(Count.ToString())>0)
                    {
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "n_productTransfered", typeof(double), 0);
                    MasterTable.Rows[0]["n_productTransfered"] = 1;
                    }
                   

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

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPickListID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nPickListID", nPickListID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Wh_PickList", "N_PickListID", nPickListID, "", connection);

                    if (Results > 0)
                    {
                        dLayer.DeleteData("Wh_PickListDetails", "N_PickListID", nPickListID, "", connection);
                        return Ok(_api.Success("Warehouse Picklist deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
    }
}
