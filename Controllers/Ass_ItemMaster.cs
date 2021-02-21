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
    [Route("assetmaster")]
    [ApiController]
    public class Ass_ItemMaster : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1305;

         public Ass_ItemMaster(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
           // FormID = 188;
        }


    [HttpGet("list")]
        public ActionResult ItemMasterList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (AssetLedgerID like '%" + xSearchkey + "%' or x_Category like '%" + xSearchkey + "%' or x_ItemName like '%" + xSearchkey + "%' or x_Model like '%" + xSearchkey + "%' or d_PurchaseDate like '%" + xSearchkey + "%' or n_LifePeriod like '%" + xSearchkey + "%' or n_BookValue like '%" + xSearchkey + "%' or x_PlateNumber like '%" + xSearchkey + "%' or x_SerialNo like '%" + xSearchkey + "%' or x_BranchName like '%" + xSearchkey + "%' or x_EmpCode like '%" + xSearchkey + "%' or x_EmpName like '%" + xSearchkey + "%' or x_Department like '%" + xSearchkey + "%' or status like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by AssetLedgerID desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_AssetDashBoard where N_CompanyID=@p1 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_AssetDashBoard where N_CompanyID=@p1 " + Searchkey + " and N_ItemID not in (select top(" + Count + ") N_ItemID from vw_AssetDashBoard where N_CompanyID=@p1 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_AssetDashBoard where N_CompanyID=@p1 ";
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
                return Ok(api.Error(e));
            }
        }
     

     [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                DataTable DetailsTable;
                DetailsTable = ds.Tables["details"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nAddlInfoID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AddlInfoID"].ToString());

                int nCompanyID1 =myFunctions.getIntVAL(DetailsTable.Rows[0]["N_CompanyId"].ToString());
                int nItemID = myFunctions.getIntVAL(DetailsTable.Rows[0]["N_ItemID"].ToString());
                string dPlacedDate = DetailsTable.Rows[0]["D_PlacedDate"].ToString();
                string xGISRefNo = DetailsTable.Rows[0]["X_GISRefNo"].ToString();
                string xBarcode = DetailsTable.Rows[0]["X_Barcode"].ToString();
                int nDepartmentID = myFunctions.getIntVAL(DetailsTable.Rows[0]["N_DepartmentID"].ToString());
                int nEmpID = myFunctions.getIntVAL(DetailsTable.Rows[0]["N_EmpID"].ToString());
                int nprojectID = myFunctions.getIntVAL(DetailsTable.Rows[0]["N_projectID"].ToString());
                string xmake = DetailsTable.Rows[0]["X_make"].ToString();
                string xModel = DetailsTable.Rows[0]["X_Model"].ToString(); 
                string dWarExpDate = DetailsTable.Rows[0]["D_WarExpDate"].ToString(); 
                string dLastMaintanance = DetailsTable.Rows[0]["D_LastMaintanance"].ToString(); 
                string dNextMaintanance = DetailsTable.Rows[0]["D_NextMaintanance"].ToString(); 
                string xDisposalRestrictions = DetailsTable.Rows[0]["X_DisposalRestrictions"].ToString(); 
                string xLocation = DetailsTable.Rows[0]["X_Location"].ToString();
                int nMaxMaintenance = myFunctions.getIntVAL(DetailsTable.Rows[0]["N_MaxMaintenance"].ToString()); 
                int nStatusID = myFunctions.getIntVAL(DetailsTable.Rows[0]["N_StatusID"].ToString());
                string xNotes = DetailsTable.Rows[0]["X_Notes"].ToString();
                string xPlateNumber = DetailsTable.Rows[0]["X_PlateNumber"].ToString();
                string xSerialNo = DetailsTable.Rows[0]["X_SerialNo"].ToString();
                string xContractNo = DetailsTable.Rows[0]["X_ContractNo"].ToString(); 
                string xHypothecatedTo = DetailsTable.Rows[0]["X_HypothecatedTo"].ToString();
                int nDwnPayemnt = myFunctions.getIntVAL(DetailsTable.Rows[0]["N_DwnPayemnt"].ToString());
                int nEmiAmount = myFunctions.getIntVAL(DetailsTable.Rows[0]["N_EmiAmount"].ToString());
                string dEmiDueDate = DetailsTable.Rows[0]["D_EmiDueDate"].ToString();
                string dEmiStartDate = DetailsTable.Rows[0]["D_EmiStartDate"].ToString();
                string dEmiEndDate = DetailsTable.Rows[0]["D_EmiEndDate"].ToString();
                int nSalvageAmt = myFunctions.getIntVAL(DetailsTable.Rows[0]["N_SalvageAmt"].ToString());
                int nDeprCalcID = myFunctions.getIntVAL(DetailsTable.Rows[0]["N_DeprCalcID"].ToString());
                int nDeprRate = myFunctions.getIntVAL(DetailsTable.Rows[0]["N_DeprRate"].ToString());
                int nCostCentreID = myFunctions.getIntVAL(DetailsTable.Rows[0]["N_CostCentreID"].ToString());
                int nBranchID = myFunctions.getIntVAL(DetailsTable.Rows[0]["N_BranchID"].ToString());
                


                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    Params.Add("N_CompanyID", nCompanyID1);

                    Params.Add("N_ItemID", nItemID);
                    Params.Add("D_PlacedDate", dPlacedDate);
                    Params.Add("X_GISRefNo", xGISRefNo);
                    Params.Add("X_Barcode", xBarcode);
                    Params.Add("N_DepartmentID", nDepartmentID);
                    Params.Add("N_EmpID", nEmpID);
                    Params.Add("N_projectID", nprojectID);
                    Params.Add("X_make", xmake);
                    Params.Add("X_Model", xModel);
                    Params.Add("D_WarExpDate", dWarExpDate);
                    Params.Add("D_LastMaintanance", dLastMaintanance);
                    Params.Add("D_NextMaintanance", dNextMaintanance);
                    Params.Add("X_DisposalRestrictions", xDisposalRestrictions);
                    Params.Add("X_Location",xLocation );
                    Params.Add("N_MaxMaintenance", nMaxMaintenance);
                    Params.Add("N_StatusID", nStatusID);
                    Params.Add("X_Notes", xNotes);
                    Params.Add("X_PlateNumber", xPlateNumber); 
                    Params.Add("X_SerialNo", xSerialNo);
                    Params.Add("X_ContractNo ", xContractNo );
                    Params.Add("X_HypothecatedTo", xHypothecatedTo);
                    Params.Add("N_DwnPayemnt", nDwnPayemnt);
                    Params.Add("N_EmiAmount", nEmiAmount);
                    Params.Add("D_EmiDueDate", dEmiDueDate);
                    Params.Add("D_EmiStartDate",dEmiStartDate);
                    Params.Add("D_EmiEndDate", dEmiEndDate);
                    Params.Add("N_SalvageAmt", nSalvageAmt);
                    Params.Add("N_DeprCalcID", nDeprCalcID);
                    Params.Add("N_DeprRate", nDeprRate);
                    Params.Add("N_CostCentreID", nCostCentreID);
                    Params.Add("N_BranchID", nBranchID);
                    
                   
                     
                    dLayer.ExecuteNonQuery(" Update Ass_AssetMaster Set D_PlacedDate=D_PlacedDate,X_GISRefNo=X_GISRefNo,X_Barcode=X_Barcode, N_DepartmentID=N_DepartmentID,N_EmpID=N_EmpID, N_ProjectID=N_ProjectID, X_make=X_make,X_Model=X_Model,D_WarExpDate=D_WarExpDate,D_LastMaintanance=D_LastMaintanance ,D_NextMaintanance=D_NextMaintanance, X_DisposalRestrictions=X_DisposalRestrictions,X_Location=X_Location,N_MaxMaintenance=N_MaxMaintenance,N_StatusID=N_StatusID,X_Notes=X_Notes,X_PlateNumber=X_PlateNumber,X_SerialNo = X_SerialNo,X_ContractNo =X_ContractNo,X_HypothecatedTo=X_HypothecatedTo,N_DwnPayemnt=N_DwnPayemnt,N_EmiAmount=N_EmiAmount,D_EmiDueDate=D_EmiDueDate,D_EmiStartDate=D_EmiStartDate,D_EmiEndDate=D_EmiEndDate,N_SalvageAmt=N_SalvageAmt,N_DeprCalcID=N_DeprCalcID ,N_DeprRate=N_DeprRate,N_CostCentreID=N_CostCentreID,N_BranchID=N_BranchID Where N_ItemID=N_ItemID and N_CompanyID=N_CompanyID",Params, connection,transaction);
                    
                    if(nItemID>0)
                    {
                       dLayer.DeleteData("Ass_AssetAddlInfo", "N_ItemID", nItemID, "N_CompanyID=" + nCompanyID , connection,transaction);
                    }

                    nAddlInfoID = dLayer.SaveData("Ass_AssetAddlInfo", "n_AddlInfoID", MasterTable, connection, transaction);
                   
                    if (nAddlInfoID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success(" Saved Successfully"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nAddlInfoID)
        {

            int Results = 0;
            try
            {
                SortedList Params = new SortedList();
               
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Ass_AssetAddlInfo", "N_AddlInfoID", nAddlInfoID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("N_AddlInfoID", nAddlInfoID.ToString());
                    return Ok(api.Success(res, "Item deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete Lead"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }
             
 [HttpGet("details")]
        public ActionResult ItemMasterListDetails(int xItemCode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_AssetMaster where N_CompanyID=@p1 and X_ItemCode=@p2 ";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", xItemCode);
            
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
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

    }
}

