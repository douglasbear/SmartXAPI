using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("salaryRevisionHistory")]
    [ApiController]
    public class Pay_PayHistoryMaster : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Pay_PayHistoryMaster(IApiFunctions apiFun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apiFun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetSalRevHistoryList(int? nCompanyId,int nEmpID,int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
            string sqlCommandCount="";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and X_HistoryCode like '%" + xSearchkey + "%'";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_HistoryID desc";
            else
            xSortBy = " order by " + xSortBy;

            if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_PayHistoryMaster where N_CompanyID=@p1 and N_EmpID=@p2" + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_PayHistoryMaster where N_CompanyID=@p1 and N_EmpID=@p2" + Searchkey + " and N_HistoryID not in(select top("+ Count +")  N_HistoryID from vw_PayHistoryMaster where N_CompanyID=@p1 and N_EmpID=@p2" + xSortBy + " ) " + xSortBy;
            
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nEmpID);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count from vw_PayHistoryMaster where N_CompanyID=@p1 and N_EmpID=@p2" + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount="0";
                    if(Summary.Rows.Count>0){
                    DataRow drow = Summary.Rows[0];
                    TotalCount = drow["N_Count"].ToString();
                    }
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(OutPut));
                }  
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpGet("listDetails")]
        public ActionResult GetSalRevHistoryDetails(int nCompanyId, string xHistoryCode, int nFnYearId,int nEmpID, bool bAllBranchData, int nBranchID)
        {
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            DataTable DataTable = new DataTable();
            string Mastersql = "";
            string DetailSql = "";

            if (bAllBranchData == true)
            {
                Mastersql = "Select * from vw_PayHistoryMaster Where N_CompanyID=@p1 and X_HistoryCode=@p3";
            }
            else
            {
                Mastersql = "Select * from vw_PayHistoryMaster Where N_CompanyID=@p1 and X_HistoryCode=@p3 and N_BranchID=@nBranchID";
                Params.Add("@nBranchID", nBranchID);
            }

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", xHistoryCode);

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);

                    MasterTable = api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                    int N_EmpID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_EmpID"].ToString());

                        if (bAllBranchData == true)
                        {
                            DetailSql = "SELECT ROW_NUMBER() OVER (ORDER BY vw_PayEmployeePayHistory.N_PayID) As Srl,vw_PayEmployeePayHistory.* FROM dbo.vw_PayEmployeePayHistory WHERE (dbo.vw_PayEmployeePayHistory.N_CompanyID =@p1) AND (dbo.vw_PayEmployeePayHistory.N_EmpID =@p4) AND (dbo.vw_PayEmployeePayHistory.N_FnYearID =@p2) Order by D_EffectiveDate Desc,vw_PayEmployeePayHistory.N_PayTypeID";
                            Params.Add("@p4", nEmpID);
                        }
                        else
                        {
                            DetailSql = "SELECT ROW_NUMBER() OVER (ORDER BY vw_PayEmployeePayHistory.N_PayID) As Srl,vw_PayEmployeePayHistory.* FROM dbo.vw_PayEmployeePayHistory WHERE (dbo.vw_PayEmployeePayHistory.N_CompanyID =@p1) AND (dbo.vw_PayEmployeePayHistory.N_EmpID =@p4) AND (dbo.vw_PayEmployeePayHistory.N_FnYearID =@p2) Order by D_EffectiveDate Desc,vw_PayEmployeePayHistory.N_PayTypeID Where N_BranchID=@nBranchID";
                            Params.Add("@p4", nEmpID);
                        }
 
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);
                }
                return Ok(api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        // [HttpPost("Save")]
        // public ActionResult SaveData([FromBody] DataSet ds)
        // {

        //     try
        //     {
        //         DataTable MasterTable;
        //         DataTable DetailTable;
        //         MasterTable = ds.Tables["master"];
        //         DetailTable = ds.Tables["details"];
        //         SortedList Params = new SortedList();

        //         string xHistoryCode = "";
        //         DataRow masterRow = MasterTable.Rows[0];
        //         var values = masterRow["X_HistoryCode"].ToString();
        //         int UserID = myFunctions.GetUserID(User);
        //         int nHistoryID=0;

        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             SqlTransaction transaction;
        //             transaction = connection.BeginTransaction();
        //             int N_HistoryID = myFunctions.getIntVAL(masterRow["N_HistoryID"].ToString());
        //             if (values == "@Auto")
        //             {
        //                 Params.Add("N_CompanyID", masterRow["n_CompanyId"].ToString());
        //                 Params.Add("N_YearID", masterRow["n_FnYearId"].ToString());
        //                 // Params.Add("N_FormID", this.FormID);
        //                 Params.Add("N_BranchID", masterRow["n_BranchId"].ToString());
        //                 xHistoryCode = dLayer.GetAutoNumber("Pay_PayHistoryMaster", "X_HistoryCode", Params, connection, transaction);
        //                 if (xHistoryCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate History code")); }
        //                 MasterTable.Rows[0]["X_HistoryCode"] = InvxHistoryCodeoiceNo;
        //             }

        //             nHistoryID = dLayer.SaveData("Pay_PayHistoryMaster", "N_HistoryID", MasterTable, connection, transaction);
        //             if (nHistoryID <= 0)
        //             {
        //                 transaction.Rollback();
        //             }
        //             for (int j = 0; j < DetailTable.Rows.Count; j++)
        //             {
        //                 DetailTable.Rows[j]["N_HistoryID"] = nHistoryID;
        //             }
        //             int N_InvoiceDetailId = dLayer.SaveData("Inv_SalesReturnDetails", "N_DebitnoteDetailsID", DetailTable, connection, transaction);
                    
        //             transaction.Commit();
        //         }
        //         SortedList Result = new SortedList();
        //         Result.Add("n_SalRevHistoryID",nHistoryID);
        //         Result.Add("x_SalRevHistoryNo",xHistoryCode);
        //         return Ok(_api.Success(Result,"Salary Revision History Saved"));
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(ex);
        //     }
        // }
    }
}