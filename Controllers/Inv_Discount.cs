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
            string sqlCommandText = "select * from Inv_DiscountMaster where N_CompanyID=@nCompanyID and isNull(B_Inactive,0)=0 order by D_Startdate desc";
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
            int Count = (nPage - 1) * nSizeperpage;

            string MasterDiscount = "Select * from vw_inv_DiscountMaster Where N_CompanyID = @p1 and N_FnYearID = @p2 and N_DiscID = @p3";
            string DetailsDiscount = "Select top(22) * from vw_Discount Where N_CompanyID = @p1 and N_FnYearID = @p2 and N_DiscID = @p3";
            // string DetailsDiscount = "";

                    //             if (Count == 0)
                    //     DetailsDiscount = "select top(" + nSizeperpage + ") * from vw_Discount Where N_CompanyID = @p1 and N_FnYearID = @p2 and N_DiscID = @p3";
                    // else
                    //     DetailsDiscount = "select top(" + nSizeperpage + ") * from vw_Discount Where N_CompanyID = @p1 and N_FnYearID = @p2 and N_DiscID = @p3 and N_DiscDetailsID not in (select top(" + Count + ") N_DiscDetailsID from vw_Discount where N_CompanyID = @p1 and N_FnYearID = @p2 and N_DiscID = @p3)";

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
        public ActionResult GetPriceListDetails(int nCustomerID, int nBranchID, int nFnYearID, int nItemID, int nCategoryID, int nItemUnitID, double nQty, int nBrandID,int n_PriceTypeID,int nLocationID ,DateTime dSalesDate)
        {
            DataTable dtPriceList = new DataTable();

            DataSet DS = new DataSet();
            SortedList Params = new SortedList();
            SortedList dParamList = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string pricelist = "";
            string Condition = "";
            string ConditionBrand = "";
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
                    Params.Add("@nBranchID", nBranchID);
                    Params.Add("@nBrandID", nBrandID);
                     Params.Add("@dSalesDate", dSalesDate);
                   // object N_PriceTypeID = null;
                    if (nBranchID > 0)
                    {
                        Condition = " and N_BranchID=@nBranchID";
                    }
                    else
                    {
                        Condition = "";
                    }
                    if (nBrandID > 0)
                    {
                        ConditionBrand = " and N_BrandID=@nBrandID";
                    }
                    else
                    {
                        ConditionBrand = "";
                    }


                    // N_PriceTypeID = dLayer.ExecuteScalar("select isnull(N_Value,'') as N_Value from Gen_Settings where X_Group=64 and N_CompanyID=" + nCompanyId + " and X_Description='DefaultPriceList'", Params, connection);

                    // if (nCustomerID > 0)
                    //      = dLayer.ExecuteScalar("select isnull(N_DiscsettingsID,'') as N_DiscsettingsID from inv_customer where N_CustomerID=" + nCustomerID + " and N_CompanyID=" + nCompanyId + " and N_FnyearID=" + nFnYearID, Params, connection);


                    if (n_PriceTypeID.ToString() != "" && n_PriceTypeID!=0)
                    {
                        Params.Add("@nPriceTypeID", n_PriceTypeID);
                        string pricelistAll = "Select  CAST(N_Price as  varchar )as X_Price,* from vw_Discount Where N_CompanyID = @nCompanyID  and  N_DiscID =@nPriceTypeID and N_ItemID=@nItemID and N_ItemUnitID=@nItemUnitID and  D_Startdate <=@dSalesDate and  D_Enddate>=@dSalesDate" + Condition + "";

                        string pricelistItem = "Select CAST(N_Price as  varchar )as X_Price,* from vw_Discount Where N_CompanyID = @nCompanyID   and N_DiscID = @nPriceTypeID and N_ItemID=@nItemID and D_Startdate <= @dSalesDate and  D_Enddate>=@dSalesDate " + Condition + " ";
                        string pricelistCategory = "Select CAST(N_Price as  varchar )as X_Price,* from vw_Discount Where N_CompanyID = @nCompanyID  and  N_DiscID = @nPriceTypeID and N_CategoryID=@nCategoryID and D_Startdate <= @dSalesDate and  D_Enddate>=@dSalesDate " + Condition + "";
                        string pricelistUnit = "Select CAST(N_Price as  varchar )as X_Price,* from vw_Discount Where N_CompanyID = @nCompanyID and   N_DiscID = @nPriceTypeID and N_ItemUnitID=@nItemUnitID and D_Startdate <= @dSalesDate and  D_Enddate>=@dSalesDate" + Condition + "";
                        string pricelistBrand = "Select CAST(N_Price as  varchar )as X_Price,* from vw_Discount Where N_CompanyID = @nCompanyID  and N_DiscID = @nPriceTypeID and N_BrandID=@nBrandID and N_BrandID<>0 and D_Startdate <= @dSalesDate and  D_Enddate>=@dSalesDate" + Condition + "";

                        dtPriceList = dLayer.ExecuteDataTable(pricelistAll, Params, connection);
                        if (dtPriceList.Rows.Count == 0)
                        {
                            dtPriceList = dLayer.ExecuteDataTable(pricelistItem, Params, connection);
                            if (dtPriceList.Rows.Count == 0)
                            {
                                dtPriceList = dLayer.ExecuteDataTable(pricelistCategory, Params, connection);
                                if (dtPriceList.Rows.Count == 0)
                                {
                                   dtPriceList = dLayer.ExecuteDataTable(pricelistBrand, Params, connection);
                                    if (dtPriceList.Rows.Count == 0)
                                    {
                                        dtPriceList = dLayer.ExecuteDataTable(pricelistUnit, Params, connection);  
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        string pricelistAll = "Select * from vw_Discount Where N_CompanyID = @nCompanyID and N_FnYearID = @nFnYearID and N_ItemID=0 and N_ItemUnitID=0 " + Condition + "";
                        dtPriceList = dLayer.ExecuteDataTable(pricelistAll, Params, connection);
                    }
                    dtPriceList = myFunctions.AddNewColumnToDataTable(dtPriceList, "N_MinimumPrice", typeof(double), 0.0);

                    dtPriceList = myFunctions.AddNewColumnToDataTable(dtPriceList, "N_MinAmount", typeof(double), 0.0);
                    dtPriceList.AcceptChanges();
                     if(dtPriceList.Rows.Count>0)
                    {
                                            
                    foreach (DataRow row in dtPriceList.Rows)
                    {
                        string xItemUnit = "select X_ItemUnit from Inv_ItemUnit where N_ItemUnitID="+nItemUnitID+" and N_CompanyID="+nCompanyId+"";
                        object unitName=dLayer.ExecuteScalar(xItemUnit,Params,connection);
                        if(myFunctions.getVAL(row["N_MarginPerc"].ToString()) >0)
                        {
                            
                                object LPrice= dLayer.ExecuteScalar("select dbo.Fn_LastCost("+nItemID+","+nCompanyId+",'')", Params, connection);  
                                double lastcost=myFunctions.getVAL(LPrice.ToString());
                                double percentageCost =(lastcost*myFunctions.getVAL(row["N_MarginPerc"].ToString()))/100;
                                lastcost=lastcost+percentageCost;
                                
                                row["X_Price"]=lastcost;

                        }
                        dtPriceList.AcceptChanges();
                        if(myFunctions.getVAL(row["N_MinMargin"].ToString()) >0)
                        {
                              
                                object cost= dLayer.ExecuteScalar("select dbo.SP_Cost_Loc("+nItemID+","+nCompanyId+",'"+unitName.ToString()+"'," + nLocationID + ")", Params, connection);  
                                double finalCost=myFunctions.getVAL(cost.ToString());
                                double minPercentageCost =((finalCost*myFunctions.getVAL(row["N_MinMargin"].ToString()))/100);
                                finalCost=finalCost+minPercentageCost;
                                
                                row["N_MinimumPrice"]=finalCost;

                        }
                                if(myFunctions.getVAL(row["N_MinMarkup"].ToString()) >0)
                        {
                              
                                object cost= dLayer.ExecuteScalar("select dbo.SP_Cost_Loc("+nItemID+","+nCompanyId+",'"+unitName.ToString()+"'," + nLocationID + ")", Params, connection);  
                                double finalCost=myFunctions.getVAL(cost.ToString());
                                
                                row["N_MinAmount"]=finalCost + myFunctions.getVAL(row["N_MinMarkup"].ToString());

                        }

                        }

                    
                }
                if (dtPriceList.Rows.Count == 0)
                {
                    return Ok(api.Success(dtPriceList));
                }


                if (nQty < myFunctions.getVAL(dtPriceList.Rows[0]["N_MinQty"].ToString()))
                {
                    string pricelistArray = ""; 
                   return Ok(api.Success(pricelistArray));
                     //return Ok(api.Warning("No Results Found"));
                }
                dtPriceList = api.Format(dtPriceList, "pricelist");



                return Ok(api.Success(dtPriceList));
                }

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
                    int N_DiscID = myFunctions.getIntVAL(MasterRow["N_DiscID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int N_BranchID = myFunctions.getIntVAL(MasterRow["n_BranchID"].ToString());
                    string x_DiscountNo = MasterRow["X_DiscCode"].ToString();

                    if(N_DiscID>0)
                     {
                      dLayer.DeleteData("Inv_DiscountDetails", "N_DiscID", N_DiscID, "N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection,transaction);
                     }
               
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

                    object objSInvoice = dLayer.ExecuteScalar("select count(1) from Inv_Sales where N_CompanyId="+nCompanyID+" and N_FnYearId="+nFnYearID+" and N_DiscID="+n_DiscountId, connection);
                    object objSInvoiceDetails = dLayer.ExecuteScalar("select count(1) from Inv_SalesDetails where N_CompanyID="+nCompanyID+" and N_PriceListID="+n_DiscountId, connection);
                    object objSOrder = dLayer.ExecuteScalar("select count(1) from Inv_SalesOrder where N_CompanyId="+nCompanyID+" and N_FnYearId="+nFnYearID+" and N_PriceTypeID="+n_DiscountId, connection);
                    object objSOrderDetails = dLayer.ExecuteScalar("select count(1) from Inv_SalesOrderDetails where N_CompanyID="+nCompanyID+" and N_PriceListID="+n_DiscountId, connection);
                    object objCustomer = dLayer.ExecuteScalar("select count(1) from Inv_Customer where N_CompanyID="+nCompanyID+" and N_FnYearID="+nFnYearID+" and N_DiscID="+n_DiscountId, connection);
                    if (objSInvoice == null) objSInvoice = 0; if (objSInvoiceDetails == null) objSInvoiceDetails = 0; if (objSOrder == null) objSOrder = 0;
                    if (objSOrderDetails == null) objSOrderDetails = 0; if (objCustomer == null) objCustomer = 0;

                    if(myFunctions.getIntVAL(objSInvoice.ToString())>0 || myFunctions.getIntVAL(objSInvoiceDetails.ToString())>0 || myFunctions.getIntVAL(objSOrder.ToString())>0 || myFunctions.getIntVAL(objSOrderDetails.ToString())>0 || myFunctions.getIntVAL(objCustomer.ToString())>0)
                    {
                        return Ok(api.Error(User, "Already Used! Unable to delete."));
                    }

                    Results = dLayer.DeleteData("Inv_DiscountMaster", "N_DiscID", n_DiscountId, "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection);
                    dLayer.DeleteData("Inv_DiscountDetails", "N_DiscID", n_DiscountId, "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection);

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

        [HttpGet("dashboardList")]
        public ActionResult GetGuardianList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_DiscCode like '%" + xSearchkey + "%' or X_Discount like '%" + xSearchkey + "%' or X_BranchName like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_Status,D_Startdate desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_DiscCode":
                        xSortBy = "X_DiscCode " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_inv_DiscountMaster where N_CompanyID=@nCompanyID  " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_inv_DiscountMaster where N_CompanyID=@nCompanyID  " + Searchkey + " and N_DiscID not in (select top(" + Count + ") N_DiscID from vw_inv_DiscountMaster where N_CompanyID=@nCompanyID " + xSortBy + " ) " + " " + xSortBy;

            Params.Add("@nCompanyID", nCompanyID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(1) as N_Count  from vw_inv_DiscountMaster where N_CompanyID=@nCompanyID " + Searchkey + "";
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

    }
}


