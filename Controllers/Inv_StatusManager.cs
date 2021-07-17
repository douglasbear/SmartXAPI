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
    [Route("invstatusmanager")]
    [ApiController]



    public class Inv_StatusManager : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Inv_StatusManager(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet]
        public ActionResult GetInventoryStatus(int? nCompanyID, int? nFnyearID ,int nLocationID,string xType,bool bAllBranchData,int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string Searchkey = "";
            string sqlCommandText="",sqlCommandCount="";

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnyearID", nFnyearID);
            Params.Add("@nLocationID", nLocationID);
            Params.Add("@xType", xType);

            //      if (cmbItemSearchBy.SelectedIndex == 0)
            //     xType = "All";
            // else if (cmbItemSearchBy.SelectedIndex == 1)
            //     xType = "N_MinQty";
            // else if (cmbItemSearchBy.SelectedIndex == 2)
            //     xType = "N_ReOrderQty";
            // else
            //     xType = "No Stock";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_ItemCode like'%" + xSearchkey + "%'or [Emp Name] like'%" + xSearchkey + "%'or X_VacType like'%" + xSearchkey + "%' or N_VacDays like'%" + xSearchkey + "%'or X_VacRemarks like'%" + xSearchkey + "%' or cast(D_VacDateTo as VarChar) like'%" + xSearchkey + "%' or cast(d_VacDateFrom as VarChar) like'%" + xSearchkey + "%' or x_CurrentStatus like'%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_ItemCode desc";
            else
                xSortBy = " order by " + xSortBy;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    bool B_PartNo = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("82", "PartNo_InGrid", "N_Value",myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    object N_LocationCount = dLayer.ExecuteScalar("Select count(1) from inv_Location where  N_CompanyID=@nCompanyID",Params,connection);
                    string X_TableName="",X_VisibleFieldList="",X_Crieteria="",X_OrderByField="";
                    if (myFunctions.getIntVAL(N_LocationCount.ToString()) > 1)
                    {
                        // X_HideFieldList = "N_CompanyID,N_CategoryID,N_LocationID,N_CurrStock,N_MinQty,N_ReOrderQty";
                        X_OrderByField = "X_ItemCode ASC";
                        X_TableName = "vw_stockstatusbylocation";
                        if (B_PartNo)
                        {
                            X_VisibleFieldList = "X_PartNo,X_ItemCode,X_ItemName,X_Category,X_PreferredVendor,X_LocationName,X_Rack,N_CurrentStock,X_ItemUnit,SOQty,X_ItemManufacturer";
                        }
                        else
                        {
                            X_VisibleFieldList = "X_ItemCode,X_ItemName,X_Category,X_PreferredVendor,X_LocationName,X_Rack,N_CurrentStock,X_ItemUnit,SOQty";
                        }
                        

                        if (bAllBranchData == true)
                        {
                            if (xType == "All")
                                X_Crieteria = " N_CompanyID =@nCompanyID";
                            else if (xType == "No Stock")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_CurrStock = 0 ";
                            else if (xType == "N_MinQty")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_CurrStock <=N_MinQty";
                            else if (xType == "N_ReOrderQty")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_CurrStock <=N_ReOrderQty";
                        }
                        else
                        {
                            if (xType == "All")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID";
                            else if (xType == "No Stock")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID and  N_CurrStock = 0 ";
                            else if (xType == "N_MinQty")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID and N_CurrStock <=N_MinQty";
                            else if (xType == "N_ReOrderQty")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID and N_CurrStock <=N_ReOrderQty";
                        }
                    }
                    else
                    {
                        //X_HideFieldList = "N_CompanyID,N_CategoryID,N_LocationID,N_CurrStock,N_MinQty,N_ReOrderQty";
                        X_OrderByField = "X_ItemCode ASC";
                        X_TableName = "vw_stockstatusbylocation";

                        if (B_PartNo)
                        {
                            X_VisibleFieldList = "X_PartNo,X_ItemCode,X_ItemName,X_Category,X_PreferredVendor,X_Rack,N_CurrentStock,X_ItemUnit,SOQty,X_ItemManufacturer";
                        }
                        else
                        {
                            X_VisibleFieldList = "X_ItemCode,X_ItemName,X_Category,X_PreferredVendor,X_Rack,N_CurrentStock,X_ItemUnit,SOQty";
                        }
                        
                        if (bAllBranchData == true)
                        {
                            if (xType == "All")
                                X_Crieteria = " N_CompanyID =@nCompanyID";
                            else if (xType == "No Stock")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_CurrStock = 0 ";
                            else if (xType == "N_MinQty")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_CurrStock <=N_MinQty";
                            else if (xType == "N_ReOrderQty")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_CurrStock <=N_ReOrderQty";
                        }
                        else
                        {
                            if (xType == "All")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID";
                            else if (xType == "No Stock")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID and  N_CurrStock = 0 ";
                            else if (xType == "N_MinQty")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID and N_CurrStock <=N_MinQty";
                            else if (xType == "N_ReOrderQty")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID and N_CurrStock <=N_ReOrderQty";
                        }
                    }
                    
                    if (Count == 0)
                        sqlCommandText = "Select top(" + nSizeperpage + ") " + X_VisibleFieldList + " from " + X_TableName + " where " + X_Crieteria + " "+Searchkey +" "+xSortBy+"";
                    else
                        sqlCommandText = "Select top(" + nSizeperpage + ") " + X_VisibleFieldList + " from " + X_TableName + " where " + X_Crieteria + " "+Searchkey +" and N_ItemID not in (select top(" + Count + ") N_ItemID from" + X_TableName + " where " + X_Crieteria + ") "+xSortBy+"";
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count From " + X_TableName + " where " + X_Crieteria + " " + Searchkey + " ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    dt = _api.Format(dt);
                    SortedList OutPut = new SortedList();
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(dt));
                    }
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("count")]
        public ActionResult GetCount(int nLocationID,bool bAllBranchData)
        {
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            string sqlAll="",sqlNoStock="",sqlMinQty="",sqlReOrder="",criteria="";

            if(bAllBranchData)
                criteria="N_CompanyID ="+nCompanyID;
            else
                criteria="N_CompanyID ="+nCompanyID+" and N_LocationID="+nLocationID;

            sqlAll = "SELECT COUNT(*) as N_Count FROM vw_stockstatusbylocation WHERE "+criteria+"";
            sqlNoStock = "SELECT COUNT(*) as N_Count FROM vw_stockstatusbylocation WHERE "+criteria+" and N_CurrStock = 0";
            sqlMinQty = "SELECT COUNT(*) as N_Count FROM vw_stockstatusbylocation WHERE "+criteria+" and N_CurrStock <=N_MinQty";
            sqlReOrder = "SELECT COUNT(*) as N_Count FROM vw_stockstatusbylocation WHERE "+criteria+" and N_CurrStock <=N_ReOrderQty";

            SortedList Data = new SortedList();
            DataTable AllItem = new DataTable();
            DataTable NoStock = new DataTable();
            DataTable MinQty = new DataTable();
            DataTable ReOrder = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    AllItem = dLayer.ExecuteDataTable(sqlAll, Params, connection);
                    NoStock = dLayer.ExecuteDataTable(sqlNoStock, Params, connection);
                    MinQty = dLayer.ExecuteDataTable(sqlMinQty, Params, connection);
                    ReOrder = dLayer.ExecuteDataTable(sqlReOrder, Params, connection);
                }

                AllItem.AcceptChanges();
                NoStock.AcceptChanges();
                MinQty.AcceptChanges();
                ReOrder.AcceptChanges();

                if (AllItem.Rows.Count > 0) Data.Add("allItemCount", AllItem);
                if (NoStock.Rows.Count > 0) Data.Add("noStocCount", NoStock);
                if (MinQty.Rows.Count > 0) Data.Add("minQtyCount", MinQty);
                if (ReOrder.Rows.Count > 0) Data.Add("reOrderCount", ReOrder);

                return Ok(_api.Success(Data));

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }




        }
    }