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
    [Route("invAsnReciept")]
    [ApiController]

    public class Inv_AsnReciept : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
         private readonly IMyAttachments myAttachments;


        public Inv_AsnReciept(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf,IMyAttachments myAtt)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1406;
             myAttachments = myAtt;

        }


        [HttpGet("list")]
        public ActionResult GetAsnReceipt(int nComapanyId, int nFnYearId, int nBranchID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, bool bAllBranchData)
        {

            int nCompanyId = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string sqlCondition = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_AsnDocNo like '%" + xSearchkey + "%' OR N_AsnID like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_AsnID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_AsnDocNo":
                        xSortBy = "X_AsnDocNo " + xSortBy.Split(" ")[1];
                        break;
                    case "N_AsnID":
                        xSortBy = "N_AsnID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nFnYearId", nFnYearId);
            Params.Add("@FormID", FormID);
            Params.Add("@nBranchID", nBranchID);

            SortedList OutPut = new SortedList();



            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // if (bAllBranchData)
                    sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId";
                    // else
                    //     sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and N_BranchID=@nBranchID";


                    if (Count == 0)
                        sqlCommandText = "select  top(" + nSizeperpage + ") * from vw_Wh_AsnMaster_Disp where " + sqlCondition + " " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select  top(" + nSizeperpage + ") * from vw_Wh_AsnMaster_Disp where " + sqlCondition + " " + Searchkey + " and N_AsnID not in (select top(" + Count + ") N_AsnID from vw_Wh_AsnMaster_Disp where " + sqlCondition + " " + xSortBy + " ) " + xSortBy;


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count from vw_Wh_AsnMaster_Disp where " + sqlCondition + " " + Searchkey + "";
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
        [HttpGet("asnlist")]
        public ActionResult GetAsnList(int nFnYearID, string xBarcode)

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string QRY = "";
            if (xBarcode != null && xBarcode != "")
                QRY = " and x_asndocno=" + xBarcode;

            string sqlCommandText = "Select *  from vw_Wh_AsnMaster_Disp Where N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + QRY + " and N_AsnID Not in (select N_AsnID from Wh_Grn  Where N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID+" )";


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
                    DataTable Attachment = ds.Tables["attachments"];
                    SortedList Params = new SortedList();

                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int nAsnID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AsnID"].ToString());
                    int N_CustomerID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CustomerID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int N_BranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_BranchID"].ToString());
                 
                    string X_AsnDocNo = "";
                    var values = MasterTable.Rows[0]["X_AsnDocNo"].ToString();

                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", 1406);
                        Params.Add("N_YearID", N_FnYearID);
                         



                        X_AsnDocNo = dLayer.GetAutoNumber("Wh_AsnMaster", "X_AsnDocNo", Params, connection, transaction);
                        if (X_AsnDocNo == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Warning("Unable to generate"));
                        }
                        MasterTable.Rows[0]["X_AsnDocNo"] = X_AsnDocNo;
                    }

                    if (nAsnID > 0)
                    {
                        dLayer.DeleteData("Wh_AsnDetails", "N_AsnID", nAsnID, "N_CompanyID=" + nCompanyID + " and N_AsnID=" + nAsnID, connection, transaction);
                        dLayer.DeleteData("Wh_AsnMaster", "N_AsnID", nAsnID, "N_CompanyID=" + nCompanyID + " and N_AsnID=" + nAsnID, connection, transaction);
                    }

                    nAsnID = dLayer.SaveData("Wh_AsnMaster", "N_AsnID", MasterTable, connection, transaction);

                    if (nAsnID <= 0)
                    {                                                                         
                        transaction.Rollback();  
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                        SortedList CustomerParams = new SortedList();
                        CustomerParams.Add("@nCustomerID", N_CustomerID);

                        DataTable CustomerInfo = dLayer.ExecuteDataTable("Select X_CustomerCode,X_CustomerName from Inv_Customer where N_CustomerID=@nCustomerID", CustomerParams, connection, transaction);
                        if (CustomerInfo.Rows.Count > 0)
                        {
                            try
                            {
                                myAttachments.SaveAttachment(dLayer, Attachment, X_AsnDocNo, nAsnID, CustomerInfo.Rows[0]["X_CustomerName"].ToString().Trim(), CustomerInfo.Rows[0]["X_CustomerCode"].ToString(), N_CustomerID, "WareHouse Document", User, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, ex));
                            }
                        }
                  

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_AsnID"] = nAsnID;
                        DetailTable.Rows[j]["X_BatchCode"] = DetailTable.Rows[j]["X_BatchCode"].ToString().Replace("@Auto",X_AsnDocNo);
                    }

                    int N_AsnDetailsID = dLayer.SaveData("Wh_AsnDetails", "N_AsnDetailsID", DetailTable, connection, transaction);
                    if (N_AsnDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));

                    }

                    // SortedList ProductParams = new SortedList();
                    // ProductParams.Add("N_CompanyID", nCompanyID);
                    // ProductParams.Add("N_ASNID", nAsnID);
                    // ProductParams.Add("N_FnYearID", N_FnYearID);
                    // ProductParams.Add("N_BranchID", N_BranchID);
                    // try
                    // {
                    //     dLayer.ExecuteNonQueryPro("SP_ASNProductInsert", ProductParams, connection, transaction);
                    // }
                    // catch (Exception ex)
                    // {
                    //     transaction.Rollback();
                    //     return Ok(_api.Error(User, ex));
                    // }


                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("N_AsnDetailsID", nAsnID);
                    Result.Add("X_AsnDocNo", X_AsnDocNo);
                    return Ok(_api.Success(Result, "Saved successfully"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }


        [HttpGet("details")]
        public ActionResult GetDetails(string xAsnDocNo, int nFnYearID, int nBranchID, bool bShowAllBranchData)
        {
            DataTable Master = new DataTable();
            DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@xAsnDocNo", xAsnDocNo);
            QueryParams.Add("@nBranchID", nBranchID);
            QueryParams.Add("@nFnYearID", nFnYearID);

            string Condition = "";
            string _sqlQuery = "";
            string sql = "";
            bool InvoiceProcessed = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (bShowAllBranchData == true)
                        Condition = "n_Companyid=@nCompanyID and X_AsnDocNo =@xAsnDocNo ";
                    else
                        Condition = "n_Companyid=@nCompanyID and X_AsnDocNo =@xAsnDocNo and N_BranchID=@nBranchID";


                    _sqlQuery = "Select * from vw_Wh_AsnMaster_Disp Where " + Condition + "";

                    Master = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);





                    Master = _api.Format(Master, "master");

                    if (Master.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        QueryParams.Add("@N_AsnID", Master.Rows[0]["N_AsnID"].ToString());

                        // QueryParams.Add("@N_AsnDetailsID",N_AsnDetailsID);


                        ds.Tables.Add(Master);

                        // Master= myFunctions.AddNewColumnToDataTable(Master, "N_Processed", typeof(int), 0);

                        object GroupCodeObj = dLayer.ExecuteScalar("select count(1) as N_Count from Wh_GRN Where N_CompanyID=@nCompanyID and N_AsnID=@N_AsnID", QueryParams, connection);
                        if (myFunctions.getIntVAL(GroupCodeObj.ToString()) > 0)
                        {
                            Master.Rows[0]["N_Processed"] = 1;

                        }
                        else
                        {
                            Master.Rows[0]["N_Processed"] = 0;

                        }

                        _sqlQuery = "Select * from vw_Wh_AsnDetails_Disp Where N_CompanyID=@nCompanyID and N_AsnID=@N_AsnID";
                        Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        Detail = _api.Format(Detail, "details");

                         DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(Master.Rows[0]["N_CustomerID"].ToString()), myFunctions.getIntVAL(Master.Rows[0]["N_AsnID"].ToString()), this.FormID, myFunctions.getIntVAL(Master.Rows[0]["N_FnYearID"].ToString()), User, connection);
                        Attachments = _api.Format(Attachments, "attachments");


                        if (Detail.Rows.Count == 0)
                        {
                            return Ok(_api.Notice("No Results Found"));
                        }
                        ds.Tables.Add(Detail);
                        ds.Tables.Add(Attachments);


                   
                     
                        return Ok(_api.Success(ds));
                    }


                }





            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        [HttpGet("SkuDetails")]
        public ActionResult GetDetails(string xSKU, int nCustomerID)
        {
            DataTable Master = new DataTable();
            DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@companyid", companyid);
            QueryParams.Add("@nCustomerID", nCustomerID);
            QueryParams.Add("@xSKU", xSKU);

            string _sqlQuery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object Count = dLayer.ExecuteScalar("select count(1)  from vw_Wh_AsnDetails_Disp where N_CompanyID=@companyid and X_SKU=@xSKU and N_CustomerID=@nCustomerID", QueryParams, connection);
                    int NCount = myFunctions.getIntVAL(Count.ToString());
                    if (NCount > 0)
                    {
                        _sqlQuery = "Select top 1 * from vw_Wh_AsnDetails_Disp Where N_CompanyID=@companyid and X_SKU=@xSKU and N_CustomerID=@nCustomerID";
                        Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);
                        if (Detail.Rows.Count > 0)
                        {
                            foreach (DataRow detVar in Detail.Rows)
                            {
                                //detVar["X_SKU"]=detVar["X_SKU"].ToString() + " - " + detVar["X_AsnDocNo"].ToString();
                                detVar["x_Barcode"] = detVar["X_SKU"].ToString() + " - " + detVar["X_AsnDocNo"].ToString();
                                detVar["x_BatchCode"] = detVar["X_SKU"].ToString() + " - " + detVar["X_AsnDocNo"].ToString();

                            }
                        }


                    }
                    else
                    {
                        object code = dLayer.ExecuteScalar("select X_CustomerCode from vw_Wh_AsnDetails_Disp where N_CompanyID=@companyid and  X_SKU=@xSKU and  N_CustomerID not in (@nCustomerID)", QueryParams, connection);

                        _sqlQuery = "Select top 1 CONCAT(X_SKU, '  - '," + code.ToString() + " ) AS X_SKU from vw_Wh_AsnDetails_Disp Where N_CompanyID=@companyid and X_SKU=@xSKU";
                        Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);
                    }

                    Detail = _api.Format(Detail, "detail");
                    if (Detail.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        ds.Tables.Add(Detail);

                        return Ok(_api.Success(ds));
                    }


                }


            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nAsnID, int nFnYearID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            SortedList Params = new SortedList();
            Params.Add("@companyid", nCompanyID);
            DataTable items = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    string sqlCommandCount3 = "select count(1) as N_Count from Wh_GRN where N_CompanyID= " + nCompanyID + "and N_AsnID=" + nAsnID + " ";
                    object count = dLayer.ExecuteScalar(sqlCommandCount3, Params, connection, transaction);
                    if (myFunctions.getIntVAL(count.ToString()) > 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("GRN Already Processed"));
                    }
                    items = dLayer.ExecuteDataTable("select N_ItemID from Wh_AsnDetails where N_AsnID = " + nAsnID + " and N_CompanyID = " + nCompanyID + " ", Params, connection, transaction);
                    if (items.Rows.Count > 0)
                    {
                        foreach (DataRow detVar in items.Rows)
                        {
                            int _itemID = myFunctions.getIntVAL(detVar["N_ItemID"].ToString());
                            object N_Result = dLayer.ExecuteScalar("Select B_YearEndProcess from Acc_FnYear Where N_CompanyID= " + nCompanyID + " and N_FnYearID= " + nFnYearID, connection, transaction);
                            if (myFunctions.getIntVAL(myFunctions.getBoolVAL(N_Result.ToString())) == 1)
                            {
                                return Ok(_api.Error(User, "Year Closed , Unable to delete product."));
                            }
                            // dLayer.DeleteData("Inv_ItemDetails", "N_MainItemID", _itemID, "", connection, transaction);
                            // Results = dLayer.DeleteData("Inv_ItemMaster", "N_ItemID", _itemID, "", connection, transaction);
                            // if (Results > 0)
                            // {

                            //     dLayer.ExecuteScalar("delete from  Inv_ItemUnit  Where N_ItemID=" + _itemID + " and N_CompanyID=" + nCompanyID, Params,connection, transaction);
                            //     dLayer.ExecuteScalar("delete from  Inv_BOMEmployee  Where N_MainItem=" + _itemID + " and N_CompanyID=" + nCompanyID, Params,connection, transaction);
                            //     dLayer.ExecuteScalar("delete from  Inv_BOMAsset  Where N_MainItemID=" + _itemID + " and N_CompanyID=" + nCompanyID, Params,connection, transaction);

                            //     //  myAttachments.DeleteAttachment(dLayer, 1, nAsnID, N_CustomerId, nFnYearID, N_FormID, User, transaction, connection);
                            //     // transaction.Commit();
                            //     // return Ok(_api.Success("Product deleted"));
                            // }
                            // else
                            // {
                            //     transaction.Rollback();

                            //     return Ok(_api.Error(User, "Unable to delete product"));
                            // }



                        }
                    }

                    
                    dLayer.DeleteData("Wh_AsnDetails", "N_AsnID", nAsnID, "N_CompanyID=" + nCompanyID + " and N_AsnID=" + nAsnID, connection, transaction);
                    Results = dLayer.DeleteData("Wh_AsnMaster", "N_AsnID", nAsnID, "N_CompanyID=" + nCompanyID + " and N_AsnID=" + nAsnID, connection, transaction);

                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("deleted Successfully"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("Unable to delete Request"));
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