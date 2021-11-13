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
    [Route("Inventorytransfer")]
    [ApiController]
    public class Inv_WHTransfer : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;

        public Inv_WHTransfer(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1056;
        }
        private readonly string connectionString;

        [HttpGet("list")]
        public ActionResult GetLocationDetails(int? nCompanyId, string prs,bool bLocationRequired,bool bAllBranchData,int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "";
            if (prs == null || prs == "")
                sqlCommandText = "select * from vw_InvLocation_Disp where N_CompanyID=@p1 order by N_LocationID DESC";
            else
            {
                if (!bLocationRequired)
                {
                    if (bAllBranchData == true)
                       sqlCommandText = "select * from vw_InvLocation_Disp where N_MainLocationID =0 and N_CompanyID=" + nCompanyId;
                    
                    else
                        sqlCommandText = "select * from vw_InvLocation_Disp where  N_MainLocationID =0 and N_CompanyID=" +nCompanyId + " and  N_BranchID=" + nBranchID;
                    
                }
                else
                {
                   sqlCommandText = "select * from vw_InvLocation_Disp where  N_MainLocationID =0 and N_CompanyID=" + nCompanyId + " and  N_BranchID=" + nBranchID;
                }
            }

            Params.Add("@p1", nCompanyId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(_api.Format(dt)));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }


        [HttpGet("productInformation")]
        public ActionResult ProductInfo(int? nCompanyID, int nLocationIDFrom)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nLocationIDFrom", nLocationIDFrom);
            string sqlCommandText = "";

            sqlCommandText = "select * from vw_UC_ItemWithStockQty where N_CompanyID=@nCompanyID and B_Inactive=0 and [Product Code]<>'001' and N_ClassID<>4 and N_ClassID<>5 and N_LocationID=@nLocationIDFrom and B_Inactive=0 ";

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



        [HttpGet("details")]
        public ActionResult EmpMaintenanceList(int nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            //int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Criteria = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and X_WarehouseNameFrom like '%" + xSearchkey + "%'";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_TransferID asc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvTransferStockDetails where N_CompanyID=@nCompanyId " + Searchkey + Criteria + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvTransferStockDetails where N_CompanyID=@nCompanyId " + Searchkey + Criteria + " and N_TransferID not in (select top(" + Count + ") N_TransferID from vw_Man_EmployeeMaintenance where N_CompanyID=@nCompanyId " + Criteria + xSortBy + " ) " + xSortBy;
            Params.Add("@nCompanyId", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_InvTransferStockDetails where N_CompanyID=@nCompanyId " + Searchkey + Criteria;
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
                return Ok(_api.Error(User,e));
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
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nTransferId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TransferId"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());
                    string X_ReferenceNo = MasterTable.Rows[0]["X_ReferenceNo"].ToString();
                    string X_TransType = "TRANSFER";

                    // int nUsercategoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserCategoryID"].ToString());
                    // int nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());
                    // int nLevelID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_Level"].ToString());
                    // int nActionID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ActionTypeID"].ToString());
                    if (nTransferId > 0)
                    {
                        SortedList deleteParams = new SortedList()
                            {
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType",X_TransType},
                                {"N_TransferId",nTransferId}
                            };
                        dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", deleteParams, connection, transaction);
                    }
                    DocNo = MasterRow["X_ReferenceNo"].ToString();
                    int nSavedraft = myFunctions.getIntVAL(MasterTable.Rows[0]["N_SaveDraft"].ToString());
                    if (X_ReferenceNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("N_YearID", nFnYearID);

                        while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_TransferStock Where X_ReferenceNo ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_ReferenceNo = DocNo;


                        if (X_ReferenceNo == "") { transaction.Rollback(); return Ok(_api.Error(User,"Unable to generate")); }
                        MasterTable.Rows[0]["X_ReferenceNo"] = X_ReferenceNo;

                    }
                    else
                    {
                        dLayer.DeleteData("Inv_TransferStock", "N_TransferId", nTransferId, "", connection, transaction);
                    }

                    MasterTable.Columns.Remove("N_FnYearID");

                    nTransferId = dLayer.SaveData("Inv_TransferStock", "N_TransferId", MasterTable, connection, transaction);
                    if (nTransferId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable To Save"));
                    }

                    for (int i = 0; i < DetailTable.Rows.Count; i++)
                    {
                        DetailTable.Rows[0]["N_TransferId"] = nTransferId;
                    }
                    int nTransferDetailsID = dLayer.SaveData("Inv_TransferStockDetails", "N_TransferDetailsID", DetailTable, connection, transaction);
                    if (nTransferDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable To Save"));
                    }
                    else
                    {
                        if (nSavedraft != 1)
                        {

                            dLayer.ExecuteScalarPro("SP_Inv_StockTransfer ", Params, connection, transaction).ToString();
                            dLayer.ExecuteScalarPro("SP_Acc_InventoryPosting ", Params, connection, transaction).ToString();
                        }

                    }



                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }



        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID, int nTransferId)
        {
            int Results = 0;
            string xTransType = "TRANSFER";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))

                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();


                    SortedList deleteParams = new SortedList()
                            {
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType",xTransType},
                                {"N_VoucherID",nTransferId}

                            };
                    Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", deleteParams, connection, transaction);
                    if (Results > 0)
                    {
                        return Ok(_api.Success("Deleted"));
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


        [HttpGet("viewdetails")]
        public ActionResult viewDetails(string xReceiptNo, int nBranchID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    Params.Add("@xReceiptNo", xReceiptNo);
                    Params.Add("@nBranchID", nBranchID);
                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));

                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable Details = new DataTable();
                    string Mastersql = "";
                    //string DetailSql = "";
                    string DetailGetSql = "";


                    // if (bAllBranchData)
                    //  xCondition="X_ReceiptNo=@xReceiptNo and N_CompanyId=@nCompanyID";
                    // else
                    //     xCondition="X_ReceiptNo=@xReceiptNo and N_CompanyId=@nCompanyID and N_BranchID=@nBranchID";

                    Mastersql = "select * from Select Inv_TransferStock.*,Inv_warehouseMasterFrom.X_LocationName As X_WarehouseNameFrom,Inv_warehouseMasterFrom.X_LocationCode As X_LocationCodeFrom,Inv_warehouseMasterTo.X_LocationName As X_WarehouseNameTo,Inv_warehouseMasterTo.X_LocationCode As X_LocationCodeTo,Inv_PRS.X_PRSNo,Inv_PRS.X_Purpose,Inv_PRS.N_PRSID,Inv_Department.N_DepartmentID,Inv_Department.X_DepartmentCode,Inv_Department.X_Department from Inv_TransferStock  left outer Join Inv_Location As Inv_warehouseMasterFrom on Inv_TransferStock.N_LocationIDFrom = Inv_warehouseMasterFrom.N_LocationID And Inv_TransferStock.N_CompanyID = Inv_warehouseMasterFrom.N_CompanyID  left outer  Join Inv_Location As Inv_warehouseMasterTo on Inv_TransferStock.N_LocationIDTo  = Inv_warehouseMasterTo.N_LocationID And Inv_TransferStock.N_CompanyID = Inv_warehouseMasterTo.N_CompanyID " +
                "left outer join Inv_PRS on Inv_TransferStock.N_PRSID=Inv_PRS.N_PRSID left outer join Inv_Department On Inv_PRS.N_DepartmentID=Inv_Department.N_DepartmentID  where Inv_TransferStock.N_CompanyID=@nCompanyID and Inv_TransferStock.N_TransferId=@nTransferId";

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    MasterTable = _api.Format(MasterTable, "Master");
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int nTransferId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TransferId"].ToString());
                    // DateTime dTransdate = Convert.ToDateTime(MasterTable.Rows[0]["D_ReceiptDate"].ToString());
                    Params.Add("@nTransferId", nTransferId);
                    DetailGetSql = "Select vw_InvTransferStockDetails.*,dbo.[SP_LocationStock](vw_InvTransferStockDetails.N_ItemID," + ") As N_Stock ,dbo.SP_Cost_Loc(vw_InvTransferStockDetails.N_ItemID,vw_InvTransferStockDetails.N_CompanyID,'', vw_InvTransferStockDetails.N_LocationIDFrom " + ") As N_LPrice,dbo.SP_SellingPrice(vw_InvTransferStockDetails.N_ItemID,vw_InvTransferStockDetails.N_CompanyID) As N_UnitSPrice " +
                    " from vw_InvTransferStockDetails where vw_InvTransferStockDetails.N_CompanyID=@nCompanyID and vw_InvTransferStockDetails.N_TransferId=@nTransferId";
                    Details = dLayer.ExecuteDataTable(DetailGetSql, Params, connection);
                    //Details = _api.Format(Details, "Details");


                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

    }


}
//       