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
    [Route("vacationReturn")]
    [ApiController]
    public class Pay_VacationReturn : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Pay_VacationReturn(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("vacationList")]
        public ActionResult GetVacationList(int nBranchID,bool bAllBranchData,int nEmpID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nBranchID",nBranchID);
            Params.Add("@nEmpID",nEmpID);
            string strBranch=(bAllBranchData==false)?" and N_BranchID=@nBranchID ":"";
            string sqlCommandText="select X_VacationGroupCode,VacationREquestDate,X_VacType,N_CompanyID,N_EmpID,N_BranchID,N_VacationGroupID,N_Transtype,N_VacTypeID,B_IsSaveDraft from vw_PayVacationMaster_Disp where N_CompanyID=@nCompanyID And N_Transtype =1 and N_EmpID=@nEmpID and B_IsSaveDraft=0 "+strBranch;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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


         [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    DataRow MasterRow = MasterTable.Rows[0];
                    transaction = connection.BeginTransaction();


                    int N_VacationReturnID = myFunctions.getIntVAL(MasterRow["n_VacationReturnID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int N_BranchID = myFunctions.getIntVAL(MasterRow["n_BranchID"].ToString());
                    int N_LocationID = myFunctions.getIntVAL(MasterRow["n_LocationID"].ToString());

                    QueryParams.Add("@nCompanyID", N_CompanyID);
                    QueryParams.Add("@nFnYearID", N_FnYearID);
                    QueryParams.Add("@nVacationReturnID", N_VacationReturnID);
                    QueryParams.Add("@nBranchID", N_BranchID);
                    QueryParams.Add("@nLocationID", N_LocationID);


                    bool B_SalesEnquiry = myFunctions.CheckPermission(N_CompanyID, 724, "Administrator", "X_UserCategory", dLayer, connection, transaction);


                    // Auto Gen
                    string X_VacationReturnCode = MasterTable.Rows[0]["X_VacationReturnCode"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    if (X_VacationReturnCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", Master["n_CompanyId"].ToString());
                        Params.Add("N_YearID", Master["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 80);
                        Params.Add("N_BranchID", Master["n_BranchId"].ToString());
                        X_VacationReturnCode = dLayer.GetAutoNumber("Pay_VacationReturn", "X_VacationReturnCode", Params, connection, transaction);
                        if (X_VacationReturnCode == "") { return Ok(_api.Error("Unable to generate Quotation Number")); }
                        MasterTable.Rows[0]["X_VacationReturnCode"] = X_VacationReturnCode;

                    }
                    

                    N_VacationReturnID = dLayer.SaveData("Inv_SalesQuotation", "N_QuotationId", DupCriteria ,"",MasterTable, connection, transaction);
                    if (N_VacationReturnID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Quotation"));
                    }


                    int N_QuotationDetailId = dLayer.SaveData("Inv_SalesQuotationDetails", "n_QuotationDetailsID", DetailTable, connection, transaction);
                    if (N_QuotationDetailId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Quotation"));
                    }
                    else
                    {
                        QueryParams.Add("@nItemID", 0);
                        QueryParams.Add("@nCRMID", 0);
                        QueryParams.Add("@nPurchaseCost", 0);
                        for (int k = 0; k < DetailTable.Rows.Count; k++)
                        {
                            QueryParams["@nItemID"] = myFunctions.getIntVAL(DetailTable.Rows[k]["n_ItemID"].ToString());
                            QueryParams["@nCRMID"] = myFunctions.getIntVAL(DetailTable.Rows[k]["n_CRMID"].ToString());
                            QueryParams["@nPurchaseCost"] = myFunctions.getVAL(DetailTable.Rows[k]["n_PurchaseCost"].ToString());

                            if (myFunctions.getVAL(QueryParams["@nPurchaseCost"].ToString()) > 0)
                                dLayer.ExecuteNonQuery("Update Inv_ItemMaster Set N_PurchaseCost=@nPurchaseCost Where N_ItemID=@nItemID and N_CompanyID=@nCompanyID", QueryParams, connection, transaction);
                            if (myFunctions.getIntVAL(QueryParams["@nCRMID"].ToString()) > 0)
                                dLayer.ExecuteNonQuery("Update Inv_CRMDetails Set B_Processed=1 Where N_CRMID=@nCRMID and N_ItemID=@nItemID and N_CompanyID=@nCompanyID and N_BranchID=@nBranchID", QueryParams, connection, transaction);
                        }
                        transaction.Commit();
                    }
                SortedList Result = new SortedList();
                Result.Add("n_QuotationID",N_QuotationID);
                Result.Add("x_QuotationNo",QuotationNo);
                return Ok(_api.Success(Result,"Sales quotation saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }




        }
    }