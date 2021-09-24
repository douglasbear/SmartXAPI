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
    [Route("warranty")]
    [ApiController]
    public class Inv_Warranty : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;

        public Inv_Warranty(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1395;
        }
        // [HttpPost("save")]
        // public ActionResult SaveData([FromBody] DataSet ds)
        // {
        //     try
        //     {
        // using (SqlConnection connection = new SqlConnection(connectionString))
        // {

        //     connection.Open();
        //     SqlTransaction transaction = connection.BeginTransaction();
        //     DataTable MasterTable;
        //     DataTable DetailTable;
        //     string DocNo = "";
        //     MasterTable = ds.Tables["master"];
        //     DetailTable = ds.Tables["details"];
        //     DataRow MasterRow = MasterTable.Rows[0];
        //     SortedList Params = new SortedList();
        //     int nCompanyID = myFunctions.GetCompanyID(User);
        //     int nServiceID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ServiceID"].ToString());
        //     string X_ServiceCode = MasterTable.Rows[0]["X_ServiceCode"].ToString();
        //     if (nServiceID > 0)
        //     {
        //         dLayer.DeleteData("Inv_ServiceMaster", "N_ServiceID", nServiceID, "", connection, transaction);
        //         dLayer.DeleteData("Inv_ServiceDetails", "N_ServiceID", nServiceID, "", connection, transaction);
        //     }
        //     DocNo = MasterRow["X_ServiceCode"].ToString();
        //     if (X_ServiceCode == "@Auto")
        //     {
        //         Params.Add("N_CompanyID", nCompanyID);
        //         Params.Add("N_FormID", this.FormID);



        //         while (true)
        //         {

        //             object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_ServiceMaster Where X_TaskCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
        //             if (N_Result == null) DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
        //             break;
        //         }
        //         X_ServiceCode = DocNo;


        //         if (X_ServiceCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate")); }
        //         MasterTable.Rows[0]["X_ServiceCode"] = X_ServiceCode;


        //     }

        //     nServiceID = dLayer.SaveData("Inv_ServiceMaster", "N_ServiceID", MasterTable, connection, transaction);
        //     if (nServiceID <= 0)
        //     {
        //         transaction.Rollback();
        //         return Ok(_api.Error(User, "Unable To Save"));
        //     }
        //     for (int i = 0; i < DetailTable.Rows.Count; i++)
        //     {
        //         DetailTable.Rows[1]["N_ServiceID"] = nServiceID;
        //     }
        //     int nServiceDetailsID = dLayer.SaveData("Inv_ServiceDetails", "N_ServiceDetailsID", DetailTable, connection, transaction);
        //     if (nServiceDetailsID <= 0)
        //     {
        //         transaction.Rollback();
        //         return Ok(_api.Error(User, "Unable To Save"));
        //     }
        //     transaction.Commit();
        //     return Ok(_api.Success("Saved"));
        // }
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(_api.Error(User, ex));
        //     }
        // }

        // [HttpGet("list")]
        // public ActionResult ProductionOrderList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        // {
        //     DataTable dt = new DataTable();
        //     SortedList Params = new SortedList();
        //     int nCompanyID = myFunctions.GetCompanyID(User);
        //     string sqlCommandCount = "";
        //     int Count = (nPage - 1) * nSizeperpage;
        //     string sqlCommandText = "";
        //     string Searchkey = "";
        //     Params.Add("@p1", nCompanyID);
        //     Params.Add("@p2", nFnYearId);
        //     if (xSearchkey != null && xSearchkey.Trim() != "")
        //         Searchkey = "and ( X_ServiceCode like '%" + xSearchkey + "%' or  D_EntryDate like '%" + xSearchkey + "%'  ) ";

        //     if (xSortBy == null || xSortBy.Trim() == "")
        //         xSortBy = " order by N_ServiceID desc";
        //     // xSortBy = " order by batch desc,D_TransDate desc";
        //     else
        //         xSortBy = " order by " + xSortBy;
        //     if (Count == 0)
        //         sqlCommandText = "select top(" + nSizeperpage + ")  * from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2  " + Searchkey;
        //     else
        //         sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey + "and N_ServiceID not in (select top(" + Count + ") N_ServiceID from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2) " + Searchkey;


        //     SortedList OutPut = new SortedList();


        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             dt = dLayer.ExecuteDataTable(sqlCommandText + xSortBy, Params, connection);

        //             sqlCommandCount = "select count(*) as N_Count  from Vw_InvService where N_CompanyID=@p1 and N_FnYearID=@p2 " + Searchkey;
        //             object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
        //             OutPut.Add("Details", _api.Format(dt));
        //             OutPut.Add("TotalCount", TotalCount);
        //             if (dt.Rows.Count == 0)
        //             {
        //                 return Ok(_api.Warning("No Results Found"));
        //             }
        //             else
        //             {
        //                 return Ok(_api.Success(OutPut));
        //             }

        //         }

        //     }
        //     catch (Exception e)
        //     {
        //         return BadRequest(_api.Error(User, e));
        //     }
        // }
        [HttpGet("details")]
        public ActionResult ServiceDetails(string xWarrantyRefCode)
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
                    DataTable ProductInfoTable = new DataTable();

                    List<SortedList> ProductInfo = new List<SortedList>();


                    string Mastersql = "";
                    string DetailSql = "";
                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@xWarrantyRefCode", xWarrantyRefCode);
                    Mastersql = ""
                    + "SELECT        Inv_WarrantyContract.*, Inv_Customer.X_CustomerCode, Inv_Customer.X_CustomerName "
                    + "FROM            Inv_WarrantyContract LEFT OUTER JOIN "
                    + "                         Inv_Customer ON Inv_WarrantyContract.N_FnYearID = Inv_Customer.N_FnYearID AND Inv_WarrantyContract.N_CompanyID = Inv_Customer.N_CompanyID AND "
                    + "                         Inv_WarrantyContract.N_CustomerID = Inv_Customer.N_CustomerID where Inv_WarrantyContract.X_WarrantyNo=@xWarrantyRefCode and Inv_WarrantyContract.N_CompanyID=@nCompanyID";
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int N_WarrantyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_WarrantyID"].ToString());
                    MasterTable.AcceptChanges();
                    Params.Add("@nWarrantyID", N_WarrantyID);
                    MasterTable = _api.Format(MasterTable, "Master");
                    //Detail
                    DetailSql = ""
                    + "SELECT        Inv_WarrantyContractDetails.N_CompanyID, Inv_WarrantyContractDetails.N_WarrantyID, Inv_WarrantyContractDetails.N_WarrantyDetailsID, Inv_WarrantyContractDetails.N_ItemID, Inv_WarrantyContractDetails.N_MainItemID, "
                    + "                         Inv_WarrantyContractDetails.N_Qty, Inv_WarrantyContractDetails.N_BranchID, Inv_WarrantyContractDetails.N_LocationID, Inv_WarrantyContractDetails.N_ItemUnitID, Inv_WarrantyContractDetails.X_ItemRemarks, "
                    + "                         Inv_ItemUnit.X_ItemUnit, Inv_ItemMaster.X_ItemCode, Inv_ItemMaster.X_ItemName "
                    + "FROM            Inv_WarrantyContractDetails LEFT OUTER JOIN "
                    + "                         Inv_ItemMaster ON Inv_WarrantyContractDetails.N_ItemID = Inv_ItemMaster.N_ItemID AND Inv_WarrantyContractDetails.N_CompanyID = Inv_ItemMaster.N_CompanyID LEFT OUTER JOIN "
                    + "                         Inv_ItemUnit ON Inv_ItemMaster.N_CompanyID = Inv_ItemUnit.N_CompanyID AND Inv_ItemMaster.N_ItemUnitID = Inv_ItemUnit.N_ItemUnitID where Inv_WarrantyContractDetails.N_CompanyID=@nCompanyID and Inv_WarrantyContractDetails.N_WarrantyID=@nWarrantyID";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");


                    //Product Information 
                    SortedList element = new SortedList();
                    int N_ItemID = myFunctions.getIntVAL(DetailTable.Rows[0]["N_MainItemID"].ToString());
                    object productName = dLayer.ExecuteScalar("Select X_ItemName from Inv_ItemMaster Where N_ItemID =" + N_ItemID + " and N_CompanyID= @nCompanyID ", Params, connection);
                    element.Add("Product", productName.ToString());
                    string[] name = {"Model", "Colour","PhoneCategory", "Device", "Category"};
                    int i=0;

                    //int nParentID = myFunctions.getIntVAL(DetailTable.Rows[0]["N_CategoryDisplayID"].ToString());
                    int nParentID=  myFunctions.getIntVAL(dLayer.ExecuteScalar("select N_CategoryDisplayID from Inv_ItemCategoryDisplayMaster where N_ItemID="+N_ItemID+"",Params,connection).ToString());
                    while (nParentID >0)
                    {
                        
                        object phoneName = dLayer.ExecuteScalar("Select X_CategoryDisplay from Inv_ItemCategoryDisplay Where N_CategoryDisplayID =" + nParentID + " and N_CompanyID= @nCompanyID ", Params, connection);
                        element.Add( name[i], phoneName.ToString());
                        nParentID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select N_ParentID from Inv_ItemCategoryDisplay Where N_CategoryDisplayID =" + nParentID + " and N_CompanyID= @nCompanyID ", Params, connection).ToString());
                        i=i+1;
                    }
                    ProductInfo.Add(element);
                    MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "ProductInformations", typeof(List<SortedList>), ProductInfo);

                  

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




    }
}

