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
     [Route("serviceTimesheet")]
     [ApiController]
    public class InvServiceTimesheet : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
        private readonly IMyFunctions myFunctions;
        private readonly IApiFunctions _api;
        // private readonly int N_FormID = 1145;
        public InvServiceTimesheet(IDataAccessLayer dl,IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            _api = api;
            myFunctions=myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("dashboardList")]
        public ActionResult DashboardList(int? nCompanyId, int nFnYearID,int nFormID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if(nFormID==1145)
            {
                if (xSearchkey != null && xSearchkey.Trim() != "")
                    Searchkey = "and (X_ServiceSheetCode like '%" + xSearchkey + "%' or X_OrderNo like '%" + xSearchkey + "%' or X_ProjectName like '%" + xSearchkey + "%' or X_CustomerName like '%" + xSearchkey + "%' or D_Invoicedate like '%" + xSearchkey + "%')";

                if (xSortBy == null || xSortBy.Trim() == "")
                    xSortBy = " order by X_ServiceSheetCode desc";
                else
                {
                    switch (xSortBy.Split(" ")[0])
                    {
                        case "X_ServiceSheetCode":
                            xSortBy = "X_ServiceSheetCode " + xSortBy.Split(" ")[1];
                            break;
                        default: break;
                    }
                    xSortBy = " order by " + xSortBy;
                }

                if (Count == 0)
                    sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_ServiceSheetMaster where N_CompanyID=@nCompanyId   " + Searchkey + " " + xSortBy;
                else
                    sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_ServiceSheetMaster where N_CompanyID=@nCompanyId  " + Searchkey + " and N_ServiceSheetID not in (select top(" + Count + ") N_ServiceSheetID from vw_Inv_ServiceSheetMaster where N_CompanyID=@nCompanyId " + xSortBy + " ) " + " " + xSortBy;
            }
            else
            {
                if (xSearchkey != null && xSearchkey.Trim() != "")
                    Searchkey = "and (X_ServiceSheetCode like '%" + xSearchkey + "%' or X_POrderNo like '%" + xSearchkey + "%' or X_ProjectName like '%" + xSearchkey + "%' or X_VendorName like '%" + xSearchkey + "%' or D_Invoicedate like '%" + xSearchkey + "%')";

                if (xSortBy == null || xSortBy.Trim() == "")
                    xSortBy = " order by X_ServiceSheetCode desc";
                else
                {
                    switch (xSortBy.Split(" ")[0])
                    {
                        case "X_ServiceSheetCode":
                            xSortBy = "X_ServiceSheetCode " + xSortBy.Split(" ")[1];
                            break;
                        default: break;
                    }
                    xSortBy = " order by " + xSortBy;
                }

                if (Count == 0)
                    sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_VendorServiceSheetMaster where N_CompanyID=@nCompanyId   " + Searchkey + " " + xSortBy;
                else
                    sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_VendorServiceSheetMaster where N_CompanyID=@nCompanyId  " + Searchkey + " and N_ServiceSheetID not in (select top(" + Count + ") N_ServiceSheetID from vw_Inv_VendorServiceSheetMaster where N_CompanyID=@nCompanyId " + xSortBy + " ) " + " " + xSortBy;
            }
            Params.Add("@nCompanyId", nCompanyID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    if(nFormID==1145)
                        sqlCommandCount = "select count(*) as N_Count  from vw_Inv_ServiceSheetMaster where N_CompanyID=@nCompanyId " + Searchkey + "";
                    else
                        sqlCommandCount = "select count(*) as N_Count  from vw_Inv_VendorServiceSheetMaster where N_CompanyID=@nCompanyId " + Searchkey + "";

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
                    DataTable ItemTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    ItemTable = ds.Tables["items"];
                    DetailTable = ds.Tables["details"];
                    SortedList Params = new SortedList();

                    int nServiceSheetID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ServiceSheetID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nFormID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FormID"].ToString());
                    string xServiceSheetCode = MasterTable.Rows[0]["x_ServiceSheetCode"].ToString();

                    if (xServiceSheetCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", nFormID);
                        xServiceSheetCode = dLayer.GetAutoNumber("Inv_ServiceTimesheet", "X_ServiceSheetCode", Params, connection, transaction);
                        if (xServiceSheetCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Service Timesheet");
                        }
                        MasterTable.Rows[0]["X_ServiceSheetCode"] = xServiceSheetCode;
                    }

                    nServiceSheetID = dLayer.SaveData("Inv_ServiceTimesheet", "N_ServiceSheetID", "", "", MasterTable, connection, transaction);
                    if (nServiceSheetID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Service Timesheet!");
                    }

                    dLayer.DeleteData("Inv_ServiceTimesheetItems", "N_ServiceSheetID", nServiceSheetID, "", connection, transaction);
                    for (int j = 0; j < ItemTable.Rows.Count; j++)
                    {
                        ItemTable.Rows[j]["N_ServiceSheetID"] = nServiceSheetID;
                    }
                    int nServiceSheetItemID = dLayer.SaveData("Inv_ServiceTimesheetItems", "N_ServiceSheetItemID", ItemTable, connection, transaction);
                    if (nServiceSheetItemID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Service Timesheet!");
                    }

                    dLayer.DeleteData("Inv_ServiceTimesheetDetails", "N_ServiceSheetItemID", nServiceSheetItemID, "", connection, transaction);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_ServiceSheetItemID"] = nServiceSheetItemID;
                    }
                    int nServiceSheetDetailsID = dLayer.SaveData("Inv_ServiceTimesheetDetails", "N_ServiceSheetDetailsID", DetailTable, connection, transaction);
                    if (nServiceSheetDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Service Timesheet!");
                    }
                   
                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("N_ServiceSheetID", nServiceSheetID);
                    Result.Add("X_ServiceSheetCode", xServiceSheetCode);

                    return Ok(_api.Success(Result, "Service Timesheet saved successfully!"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("details")]
        public ActionResult ServiceSheetDetails(int nFormID,string xServiceSheeetCode)
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
                    DataTable DataTable = new DataTable();

                    string Mastersql = "";
                    string DetailSql = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@xServiceSheeetCode", xServiceSheeetCode);
                    Mastersql = "select * from vw_PayEvaluation_Details where N_CompanyId=@nCompanyID and X_EvalCode=@xServiceSheeetCode   ";
                   
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int EvaID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_EvalID"].ToString());
                    Params.Add("@nEvalID", EvaID);

                    MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from vw_PayEvaluation_Details where N_CompanyId=@nCompanyID and N_EvalID=@nEvalID ";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(_api.Success(dt));

                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
            
        [HttpGet("List")]

        
        public ActionResult EmployeeEvaluationList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select N_CompanyID,N_EvalID,X_EvalCode,X_Description from vw_PayEmpEvauation_List where N_CompanyID=@nComapnyID";
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
        

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nEvalID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nEvalID", nEvalID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Pay_EmpEvaluation", "N_EvalID", nEvalID, "", connection);


                    if (Results > 0)
                    {
                        dLayer.DeleteData("Pay_EmpEvaluationDetails", "N_EvalID", nEvalID, "", connection);
                        return Ok(_api.Success("Employee Evaluation deleted"));
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

        [HttpGet("orderitemdetails")]
        public ActionResult GetOrderItemDetails(int nFnYearID, int nFormID, int nBranchID, int nLocationID, DateTime dDateFrom, DateTime dDateTo, int nSOID, string xType)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable ItemTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    string Itemsql = "";

                    Params.Add("@N_TransID", nSOID);
                    Params.Add("@N_CompanyId", myFunctions.GetCompanyID(User));
                    Params.Add("@N_FnyearID", nFnYearID);
                    Params.Add("@N_BranchID", nBranchID);
                    Params.Add("@D_DateFrom", dDateFrom);
                    Params.Add("@D_DateTo", dDateTo);
                    Params.Add("@N_LocationID", nLocationID);
                    Params.Add("@N_UserID", myFunctions.GetUserID(User));
                    Params.Add("@X_Type", xType);
                    Params.Add("@N_FormID", nFormID);
                    
                    
                    Itemsql = "select * from vw_SOItemsForTimesheet where N_CompanyID=@N_CompanyId and N_SalesOrderId=@N_TransID and ((D_DeliveryDate<=@D_DateFrom) or (D_DeliveryDate>=@D_DateFrom AND D_DeliveryDate<=@D_DateTo) AND ISNULL(D_ReturnDate,@D_DateTo)>=@D_DateTo)";
                    ItemTable = dLayer.ExecuteDataTable(Itemsql, Params, connection);
                    ItemTable = _api.Format(ItemTable, "Items");

                    DetailTable = dLayer.ExecuteDataTablePro("SP_InvItemWiseDateList", Params, connection);

                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(ItemTable);
                    dt.Tables.Add(DetailTable);
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
    