using System.Collections.Generic;
using SmartxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("products")]
    [ApiController]
    public class Inv_ItemMaster : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Inv_ItemMaster(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        //GET api/Projects/list
        [HttpGet("list")]
        public ActionResult GetAllItems(string query, int PageSize, int Page)
        {
            int nCompanyID =myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string qry = "";
                if (query != "" && query != null)
                {
                    qry = " and (Description like @query or [Item Code] like @query) ";
                    Params.Add("@query", "%" + query + "%");
                }

            string pageQry = "DECLARE @PageSize INT, @Page INT Select @PageSize=@PSize,@Page=@Offset;WITH PageNumbers AS(Select ROW_NUMBER() OVER(ORDER BY N_ItemID) RowNo,";
            string pageQryEnd = ") SELECT * FROM    PageNumbers WHERE   RowNo BETWEEN((@Page -1) *@PageSize + 1)  AND(@Page * @PageSize) order by [Item Code],Description";

            string sqlComandText = " * from Vw_InvItem_Search where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 " + qry;
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", 0);
            Params.Add("@p3", "001");
            Params.Add("@p4", 1);
            Params.Add("@PSize", PageSize);
            Params.Add("@Offset", Page);

            

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = pageQry + sqlComandText + pageQryEnd;
                    dt = dLayer.ExecuteDataTable(sql, Params, connection);
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

        [HttpGet("dashboardList")]
        public ActionResult GetDashboardList(int nFnYearId,int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID =myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and Description like '%" + xSearchkey + "%'";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by [Item Code] desc";
            else
                xSortBy = " order by " + xSortBy;

            if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +") * from Vw_InvItem_Search where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top("+ nSizeperpage +") * from Vw_InvItem_Search where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 " + Searchkey + " and [Item Code] not in (select top("+ Count +") [Item Code] from Vw_InvItem_Search where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 " + Searchkey + xSortBy + " ) " + xSortBy;


            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", 0);
            Params.Add("@p3", "001");
            Params.Add("@p4", 1);

            SortedList OutPut = new SortedList();
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    string sqlCommandCount = "select count(*) as N_Count  from Vw_InvItem_Search where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 " + Searchkey;
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
                return Ok(_api.Error(e));
            }

        }



        

         [HttpGet("details")]
        public ActionResult GetItemDetails(string xItemCode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@xItemCode", xItemCode);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string _sqlQuery = "SELECT * from vw_InvItemMaster where X_ItemCode=@xItemCode and N_CompanyID=@nCompanyID";

                    dt = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);


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

        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable, GeneralTable,UnitTable;
                MasterTable = ds.Tables["master"];
                GeneralTable = ds.Tables["general"];
                UnitTable = ds.Tables["itemunit"];
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string ItemCode = "";
                    var values = MasterTable.Rows[0]["X_ItemCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterTable.Rows[0]["N_CompanyId"].ToString());
                        Params.Add("N_YearID", GeneralTable.Rows[0]["N_FnYearId"].ToString());
                        Params.Add("N_FormID", 53);
                        ItemCode = dLayer.GetAutoNumber("Inv_ItemMaster", "X_ItemCode", Params, connection, transaction);
                        if (ItemCode == "") { return Ok(_api.Warning("Unable to generate product Code")); }
                        MasterTable.Rows[0]["X_ItemCode"] = ItemCode;
                    }


                    int N_ItemID = dLayer.SaveData("Inv_ItemMaster", "N_ItemID", MasterTable, connection, transaction);
                    if (N_ItemID <= 0)
                    {
                        transaction.Rollback();
                        return Ok( _api.Warning( "Unable to save"));
                    }


                     foreach (DataRow var in UnitTable.Rows)
                    {
                        var["n_ItemID"] = N_ItemID;
                    }
                    UnitTable.AcceptChanges();
                    DataRow[] BaseUnitRow = UnitTable.Select("B_BaseUnit = 1 ");
DataTable BaseUnitTable=BaseUnitRow.CopyToDataTable();
UnitTable.Rows.RemoveAt(0);
UnitTable.AcceptChanges();
BaseUnitTable.AcceptChanges();
                    int BaseUnitID = dLayer.SaveData("Inv_ItemUnit", "N_ItemUnitID", BaseUnitTable, connection, transaction);
                    foreach (DataRow var in UnitTable.Rows)
                    {
                        var["n_BaseUnitID"] = BaseUnitID;
                    }
                    int UnitID = dLayer.SaveData("Inv_ItemUnit", "N_ItemUnitID", UnitTable, connection, transaction);
                    if (UnitID <= 0)
                    {
                        transaction.Rollback();
                        return Ok( _api.Warning( "Unable to save"));
                    }
                    
                    
                    
                    transaction.Commit();
                }
                return Ok(_api.Success("Product Saved"));

            }
            catch (Exception ex)
            {
                return Ok( _api.Error(ex));
            }
        }



        //GET api/Projects/list
        [HttpGet("class")]
        public ActionResult GetItemClass()
        {
            DataTable dt = new DataTable();

            string sqlComandText = "select * from Inv_ItemClass order by N_Order ASC";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlComandText, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("no result found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return Ok( _api.Error(e));
            }

        }

        [HttpGet("dummy")]
        public ActionResult GetPurchaseInvoiceDummy(int? Id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                string sqlCommandText = "select * from Inv_ItemMaster where N_ItemID=@p1";
                SortedList mParamList = new SortedList() { { "@p1", Id } };
                DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList,connection);
                masterTable = _api.Format(masterTable, "master");

                string sqlCommandText2 = "select * from Inv_ItemMaster where N_ItemID=@p1";
                SortedList dParamList = new SortedList() { { "@p1", Id } };
                DataTable detailTable = dLayer.ExecuteDataTable(sqlCommandText2, dParamList,connection);
                detailTable = _api.Format(detailTable, "details");

                if (detailTable.Rows.Count == 0) { return Ok(new { }); }
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(masterTable);
                dataSet.Tables.Add(detailTable);

                return Ok(dataSet);
                }

            }
            catch (Exception e)
            {
                return Ok( _api.Error(e));
            }
        }



    }


}