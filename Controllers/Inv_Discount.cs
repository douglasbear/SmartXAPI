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
    [Route("discount")]
    [ApiController]
    public class Inv_Discount : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Inv_Discount(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetDiscountList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "select * from Inv_DiscountMaster where N_CompanyID=@nCompanyID";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
        [HttpGet("details")]
        public ActionResult GetDiscountDetails(int N_DiscID, int nFnYearID)
        {
            DataTable dtDiscountMaster = new DataTable();
            DataTable dtDiscountDetails = new DataTable();

            DataSet DS = new DataSet();
            SortedList Params = new SortedList();
            SortedList dParamList = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string MasterDiscount = "Select * from Inv_DiscountMaster Where N_CompanyID = @p1 and N_FnYearID = @p2 and N_DiscID = @p3";
            string DetailsDiscount = "Select * from vw_Discount Where N_CompanyID = @p1 and N_FnYearID = @p2 and N_DiscID = @p3";

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearID);
            Params.Add("@p3", N_DiscID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dtDiscountMaster = dLayer.ExecuteDataTable(MasterDiscount, Params, connection);
                    dtDiscountDetails = dLayer.ExecuteDataTable(DetailsDiscount, Params, connection);

                }
                dtDiscountMaster = api.Format(dtDiscountMaster, "Master");
                dtDiscountDetails = api.Format(dtDiscountDetails, "Details");

                SortedList Data = new SortedList();
                Data.Add("Master", dtDiscountMaster);
                Data.Add("Details", dtDiscountDetails);

                if (dtDiscountMaster.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(Data));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
        [HttpGet("pricelist")]
        public ActionResult GetPriceListDetails(int nCustomerID, int nFnYearID, int nItemID, int nCategoryID, int nItemUnitID, int nQty)
        {
            DataTable dtPriceList = new DataTable();

            DataSet DS = new DataSet();
            SortedList Params = new SortedList();
            SortedList dParamList = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string pricelist = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Params.Add("@nCompanyID", nCompanyId);
                    Params.Add("@nFnYearID", nFnYearID);
                    Params.Add("@nItemID", nItemID);
                    Params.Add("@nItemUnitID", nItemUnitID);
                    Params.Add("@nCategoryID", nCategoryID);

                    object N_PriceTypeID = dLayer.ExecuteScalar("select N_DiscsettingsID from inv_customer where N_CustomerID=" + nCustomerID + " and N_CompanyID=" + nCompanyId + " and N_FnyearID=" + nFnYearID, Params, connection);

                    if (N_PriceTypeID != null)
                    {
                        Params.Add("@nPriceTypeID", N_PriceTypeID.ToString());
                        string pricelistAll = "Select * from vw_Discount Where N_CompanyID = @nCompanyID and N_FnYearID = @nFnYearID and N_DiscID = @nPriceTypeID and N_ItemID=@nItemID and N_ItemUnitID=@nItemUnitID";
                        
                        string pricelistItem = "Select * from vw_Discount Where N_CompanyID = @nCompanyID and N_FnYearID = @nFnYearID and N_DiscID = @nPriceTypeID and N_ItemID=@nItemID";
                        string pricelistCategory = "Select * from vw_Discount Where N_CompanyID = @nCompanyID and N_FnYearID = @nFnYearID and N_DiscID = @nPriceTypeID and N_CategoryID=@nCategoryID";
                        string pricelistUnit = "Select * from vw_Discount Where N_CompanyID = @nCompanyID and N_FnYearID = @nFnYearID and N_DiscID = @nPriceTypeID and N_ItemUnitID=@nItemUnitID";

                        dtPriceList = dLayer.ExecuteDataTable(pricelistAll, Params, connection);
                        if (dtPriceList.Rows.Count == 0)
                        {
                            dtPriceList = dLayer.ExecuteDataTable(pricelistItem, Params, connection);
                            if (dtPriceList.Rows.Count == 0)
                            {
                                dtPriceList = dLayer.ExecuteDataTable(pricelistCategory, Params, connection);
                                if (dtPriceList.Rows.Count == 0)
                                {
                                    dtPriceList = dLayer.ExecuteDataTable(pricelistUnit, Params, connection);
                                }
                            }
                        }
                    }
                    else
                    {
                        string pricelistAll = "Select * from vw_Discount Where N_CompanyID = @nCompanyID and N_FnYearID = @nFnYearID and N_ItemID=0 and N_ItemUnitID=0";
                        dtPriceList = dLayer.ExecuteDataTable(pricelistAll, Params, connection);
                    }

                }

                if (dtPriceList.Rows.Count == 0)
                {
                    return Ok(api.Success(dtPriceList));
                }


                if (nQty < myFunctions.getVAL(dtPriceList.Rows[0]["N_MinQty"].ToString()))
                {
                    return Ok(api.Warning("No Results Found"));
                }
                dtPriceList = api.Format(dtPriceList, "pricelist");



                return Ok(api.Success(dtPriceList));

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
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


                    DataTable Master = ds.Tables["master"];
                    DataTable Details = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    DataRow MasterRow = Master.Rows[0];

                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int N_BranchID = myFunctions.getIntVAL(MasterRow["n_BranchID"].ToString());
                    string x_DiscountNo = MasterRow["X_DiscCode"].ToString();

                    Master.Columns.Remove("n_BranchID");

                    if (x_DiscountNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", 858);                        
                        Params.Add("N_BranchID", N_BranchID);
                        x_DiscountNo = dLayer.GetAutoNumber("Inv_DiscountMaster", "X_DiscCode", Params, connection, transaction);
                        if (x_DiscountNo == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Discount Number");
                        }
                        Master.Rows[0]["X_DiscCode"] = x_DiscountNo;
                    }
                    string DupCriteria = "";


                    int n_DiscountId = dLayer.SaveData("Inv_DiscountMaster", "N_DiscID", DupCriteria, "", Master, connection, transaction);
                    if (n_DiscountId <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                    for (int i = 0; i < Details.Rows.Count; i++)
                    {
                        Details.Rows[i]["N_DiscID"] = n_DiscountId;

                    }

                    dLayer.SaveData("Inv_DiscountDetails", "N_DiscDetailsID", Details, connection, transaction);
                    transaction.Commit();
                    SortedList Result = new SortedList();

                    return Ok(api.Success(Result, "Discount Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int n_DiscountId, int nFnYearID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Inv_DiscountMaster", "N_DiscID", n_DiscountId, "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection);

                }
                if (Results > 0)
                {
                    return Ok(api.Success("Discount deleted"));
                }
                else
                {
                    return Ok(api.Error(User, "Unable to delete"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }

        }

    }
}



