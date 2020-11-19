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
        private readonly int FormID;


        public Inv_StatusManager(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 82;
        }

        [HttpGet]
        public ActionResult GetAllSalesExecutives(int? nCompanyID, int? nFnyearID ,int nLocationID,string xType,bool bAllBranchData)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

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
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    bool B_PartNo = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("82", "PartNo_InGrid", "N_Value",myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    object N_LocationCount = dLayer.ExecuteScalar("Select count(1) from inv_Location where  N_CompanyID=@nCompanyID",Params,connection);
                    string X_HideFieldList,X_TableName,X_VisibleFieldList,X_Crieteria,X_OrderByField;
                    if (myFunctions.getIntVAL(N_LocationCount.ToString()) > 1)
                    {
                        X_HideFieldList = "N_CompanyID,N_CategoryID,N_LocationID,N_CurrStock,N_MinQty,N_ReOrderQty";
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
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_CurrStock <=@xType";
                            else
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_CurrStock <=@xType";
                        }
                        else
                        {
                            if (xType == "All")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID";
                            else if (xType == "No Stock")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID and  N_CurrStock = 0 ";
                            else if (xType == "N_MinQty")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID and N_CurrStock <=@xType";
                            else
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID and N_CurrStock <=@xType";
                        }
                    }
                    else
                    {
                        X_HideFieldList = "N_CompanyID,N_CategoryID,N_LocationID,N_CurrStock,N_MinQty,N_ReOrderQty";
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
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_CurrStock <=@xType";
                            else
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_CurrStock <=@xType";
                        }
                        else
                        {
                            if (xType == "All")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID";
                            else if (xType == "No Stock")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID and  N_CurrStock = 0 ";
                            else if (xType == "N_MinQty")
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID and N_CurrStock <=@xType";
                            else
                                X_Crieteria = " N_CompanyID =@nCompanyID and N_LocationID =@nLocationID and N_CurrStock <=@xType";
                        }
                    }
                    string sqlCommandText = "Select " + X_VisibleFieldList + " from " + X_TableName + " where " + X_Crieteria + " Order by " + X_OrderByField;
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
                return Ok(_api.Error(e));
            }
        }

    




        }
    }