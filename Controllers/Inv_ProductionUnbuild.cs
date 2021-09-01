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
    [Route("productionUnbuild")]
    [ApiController]
    public class Inv_ProductionUnbuild : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 876;


        public Inv_ProductionUnbuild(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetUnbuildList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_Action like '%" + xSearchkey + "%' or X_ReferenceNo like '%" + xSearchkey + "%' or X_TypeName like '%" + xSearchkey + "%' or X_ItemCode like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_ReferenceNo desc";
            else

                xSortBy = " order by " + xSortBy;



            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvAssembly where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvAssembly where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + " and N_AssemblyID not in (select top(" + Count + ") N_AssemblyID from vw_InvAssembly where N_CompanyID=@p1 and N_FnYearID=@p2" + xSearchkey + xSortBy + " ) " + xSortBy;

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count from vw_InvAssembly where N_CompanyId=@p1 and N_FnYearID=@p2 " + Searchkey + " ";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";

                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();

                    }
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
                return Ok(_api.Error(User,e));
            }
        }

        [HttpGet("settings")]
        public ActionResult CheckSettings(string FormID, int nBranchID, bool bAllBranchData)
        {
            double N_decimalPlace = 0,N_DecimalPlaceQty=0;
            bool B_AddItem = false, B_ShowBOMQty = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    //Params.Add("@nFnYearID", nFnYearID);

                    dt.Clear();
                    dt.Columns.Add("N_decimalPlace");
                    dt.Columns.Add("N_DecimalPlaceQty");
                    dt.Columns.Add("B_AddItem");
                    dt.Columns.Add("B_ShowBOMQty");

                    N_decimalPlace = myFunctions.getIntVAL(myFunctions.ReturnSettings("Production", "Decimal_Place", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection));
                    N_DecimalPlaceQty = myFunctions.getIntVAL(myFunctions.ReturnSettings("Production", "Decimal_Place_Qty", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection));
                    B_AddItem = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("825", "AddItem", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_ShowBOMQty = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("825", "Show_BOM_Qty", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                   
                    DataRow row = dt.NewRow();
                    row["N_decimalPlace"] = myFunctions.getVAL(N_decimalPlace.ToString());
                    row["N_DecimalPlaceQty"] = myFunctions.getVAL(N_DecimalPlaceQty.ToString());
                    row["B_AddItem"] = B_AddItem;
                    row["B_ShowBOMQty"] = B_ShowBOMQty;
                    
                    dt.Rows.Add(row);

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
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }

        }

        [HttpGet("itemDetails")]
        public ActionResult GetItemDetails(int nItemID,int nFnYearId,int nLocationID)
        {

            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable SubItemsTable = new DataTable();
            string SubItemssql = "";

            int nCompanyId = myFunctions.GetCompanyID(User);

            SubItemssql = "Select *,dbo.SP_GenGetStock(vw_InvItemDetails.N_ItemID,@nLocationID,'','Location') As N_Stock,dbo.SP_Cost(vw_InvItemDetails.N_ItemID,vw_InvItemDetails.N_CompanyID,'') As N_Cost,dbo.SP_SellingPrice(vw_InvItemDetails.N_ItemID,vw_InvItemDetails.N_CompanyID) As N_SPrice From vw_InvItemDetails Where N_MainItemID =@nItemID and N_CompanyID=@nCompanyId and N_Type=1";

            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nFnYearId", nFnYearId);
            Params.Add("@nItemID", nItemID);
            Params.Add("@nLocationID", nLocationID);

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SubItemsTable = dLayer.ExecuteDataTable(SubItemssql, Params, connection);

                    SubItemsTable = _api.Format(SubItemsTable, "SubItems");
                    dt.Tables.Add(SubItemsTable);
                  
                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        //Save....
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {

            DataTable MasterTable;
            DataTable DetailTable;
            // DataTable ProductCostTable;
            // DataTable MachineCostTable;
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            // ProductCostTable = ds.Tables["productCost"];
            // MachineCostTable = ds.Tables["machineCost"];
            SortedList Params = new SortedList();
            // Auto Gen
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    string X_ReferenceNo = "";
                    int N_AssemblyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AssemblyID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int N_LocationID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LocationID"].ToString());
                    int N_UserID = myFunctions.GetUserID(User);
                    var values = MasterTable.Rows[0]["X_ReferenceNo"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID",N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", this.N_FormID);
                        X_ReferenceNo = dLayer.GetAutoNumber("Inv_Assembly", "X_ReferenceNo", Params, connection, transaction);
                        if (X_ReferenceNo == "") { transaction.Rollback(); return Ok(_api.Warning("Unable to generate Quotation Number")); }
                        MasterTable.Rows[0]["X_ReferenceNo"] = X_ReferenceNo;
                    }

                    if (N_AssemblyID > 0)
                    {
                        SortedList DeleteParams = new SortedList(){
                                {"X_Task","delete"},
                                {"N_AssemblyID",N_AssemblyID},
                                {"N_LocationID",N_LocationID}//,
                                //{"X_TransType",N_CreditNoteID}
                                };
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_BuildorUnbuild", DeleteParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,ex));
                        }
                    }

                    N_AssemblyID = dLayer.SaveData("Inv_Assembly", "N_AssemblyID", MasterTable, connection, transaction);
                    if (N_AssemblyID <= 0)
                    {
                        transaction.Rollback();
                    }

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_AssemblyID"] = N_AssemblyID;
                    }
                    int N_QuotationDetailId = dLayer.SaveData("Inv_AssemblyDetails", "N_AssemblyDetailsID", DetailTable, connection, transaction);
                    
                    // for (int j = 0; j < ProductCostTable.Rows.Count; j++)
                    // {
                    //     ProductCostTable.Rows[j]["N_AssemblyID"] = N_AssemblyID;
                    // }
                    // int N_ProductCostID = dLayer.SaveData("Inv_ProductionCost", "N_TransID", ProductCostTable, connection, transaction);
                    
                    // for (int j = 0; j < MachineCostTable.Rows.Count; j++)
                    // {
                    //     MachineCostTable.Rows[j]["N_AssemblyID"] = N_AssemblyID;
                    // }
                    // int N_PMachineCostID = dLayer.SaveData("Inv_ProductionCost", "N_TransID", MachineCostTable, connection, transaction);
                    

                    SortedList InsertParams = new SortedList(){
                    {"X_Task","insert"},
                    {"N_AssemblyID",N_AssemblyID},
                    {"N_LocationID",N_LocationID}//,
                    //{"X_TransType",N_CreditNoteID}
                    };
                    try
                    {
                        dLayer.ExecuteNonQueryPro("SP_BuildorUnbuild", InsertParams, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,ex));
                    }

                    SortedList Result = new SortedList();
                    Result.Add("N_AssemblyID", N_AssemblyID);
                    Result.Add("X_ReferenceNo", X_ReferenceNo);
                    transaction.Commit();
                    return Ok(_api.Success(Result, "Product Unbilded"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("details")]
        public ActionResult GetDetails(int nCompanyId, string xReferenceNo, int nFnYearId)
        {

            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            // DataTable ProductCostTable = new DataTable();
            // DataTable MachineCostTable = new DataTable();
            string Mastersql = "";

            Mastersql = "Select * from vw_InvAssembly  Where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and X_Action = 'UnBuild' and X_ReferenceNo=@xReferenceNo";

            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nFnYearId", nFnYearId);
            Params.Add("@xReferenceNo", xReferenceNo);

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);

                    MasterTable = _api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);
                    Params.Add("@N_AssemblyID", MasterTable.Rows[0]["N_AssemblyID"].ToString());

                    string DetailSql = "",ProductCostSql="",MachineCostSql="";

                    DetailSql = "Select * from vw_InvAssemblyDetails Where N_CompanyID=@nCompanyId and N_AssemblyID=@N_AssemblyID and N_Type=1";

                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);

                    // ProductCostSql = "Select * from Inv_ProductionCost where N_AssembleID=@N_AssemblyID and N_FormTypeID=1";

                    // ProductCostTable = dLayer.ExecuteDataTable(ProductCostSql, Params, connection);
                    // ProductCostTable = _api.Format(ProductCostTable, "productCost");
                    // dt.Tables.Add(ProductCostTable);

                    // MachineCostSql = "Select * from Inv_ProductionCost where N_AssembleID=@N_AssemblyID and N_FormTypeID=1";

                    // MachineCostTable = dLayer.ExecuteDataTable(MachineCostSql, Params, connection);
                    // MachineCostTable = _api.Format(MachineCostTable, "machineCost");
                    // dt.Tables.Add(MachineCostTable);
                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int N_AssemblyID,int N_LocationID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    if (N_AssemblyID > 0)
                    {
                        SortedList DeleteParams = new SortedList(){
                                {"X_Task","delete"},
                                {"N_AssemblyID",N_AssemblyID},
                                {"N_LocationID",N_LocationID}};
                        try
                        {
                            Results=dLayer.ExecuteNonQueryPro("SP_BuildorUnbuild", DeleteParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,ex));
                        }
                        transaction.Commit();
                       
                    }                
                }  
                 if (Results > 0)
                {
                    
                    return Ok(_api.Success( "Production Unbuild deleted"));
                }
                else
                {
                    return Ok(_api.Warning("Unable to delete production unbuild"));
                }     
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

         [HttpGet("itemData")]
        public ActionResult GetItemData(int nCompanyId, string X_ItemCode ,int nLocationID)
        {

            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable ItemTable = new DataTable();
            DataTable DetailTable = new DataTable();

            string Itemsql = "";
            string condition = "([Item Code] =@X_ItemCode OR X_Barcode =@X_ItemCode)";

            Itemsql = "Select *,dbo.SP_GenGetStock(vw_InvItem_Search.N_ItemID,@nLocationID,'','Location') As N_Stock ,dbo.SP_Cost_Loc(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID,vw_InvItem_Search.X_ItemUnit,@nLocationID) As N_LPrice ,dbo.SP_SellingPrice(vw_InvItem_Search.N_ItemID,vw_InvItem_Search.N_CompanyID) As N_SPrice  From vw_InvItem_Search Where "+condition+" and N_CompanyID=@nCompanyId";

            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nLocationID", nLocationID);
            Params.Add("@X_ItemCode", X_ItemCode);

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    ItemTable = dLayer.ExecuteDataTable(Itemsql, Params, connection);

                    ItemTable = _api.Format(ItemTable, "itemDetails");
                    Params.Add("@N_ItemID", ItemTable.Rows[0]["N_ItemID"].ToString());
                    ItemTable = myFunctions.AddNewColumnToDataTable(ItemTable, "N_QtyInHand", typeof(double), 0);
                    object objStock = dLayer.ExecuteScalar("select dbo.SP_GenGetStock(Inv_StockMaster.N_ItemID,@nLocationID,'','Location') as N_CurrentStock from Inv_StockMaster group by N_ItemID having N_ItemID=@N_ItemID", Params, connection);
                    if (objStock != null)
                    {
                        ItemTable.Rows[0]["N_QtyInHand"] = myFunctions.getVAL(objStock.ToString());
                    }
                    dt.Tables.Add(ItemTable);
                   
                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

    }
}