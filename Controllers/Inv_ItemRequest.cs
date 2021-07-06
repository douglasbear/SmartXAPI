using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("itemRequest")]
    [ApiController]
    public class Inv_ItemRequest : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Inv_ItemRequest(IApiFunctions apiFun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apiFun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }



        [HttpGet("list")]
        public ActionResult GetRequestList(int nCompanyId, int nFnYearId,int nBranchID,bool bAllBranchData,int FormID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string sqlCondition = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([PRS No] like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_PRSID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "PRSNo":
                        xSortBy = "[PRS No] " + xSortBy.Split(" ")[1];
                        break;
                    case "N_PRSID":
                        xSortBy = "N_PRSID " + xSortBy.Split(" ")[1];
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
                    if (!myFunctions.CheckClosedYear(nCompanyId,nFnYearId,dLayer,connection))
                    {
                        if (bAllBranchData)
                            sqlCondition = "N_CompanyID=@nCompanyId and B_YearEndProcess=0 and N_FormID=@FormID";
                        else
                            sqlCondition = "N_CompanyID=@nCompanyId and N_BranchID=@nBranchID and B_YearEndProcess=0 and N_FormID=@FormID";
                    }
                    else
                    {
                        if (bAllBranchData)
                            sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and N_FormID=@FormID";
                        else
                            sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and N_BranchID=@nBranchID and N_FormID=@FormID";
                    }

                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvPRSNo_UCSearch where " + sqlCondition + " " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvPRSNo_UCSearch where "+ sqlCondition +" " + Searchkey + " and N_PRSID not in (select top(" + Count + ") N_PRSID from vw_InvPRSNo_UCSearch where "+ sqlCondition +" " + xSortBy + " ) " + xSortBy;

                    
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count from vw_InvPRSNo_UCSearch where "+ sqlCondition +" " + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    string TotalSum = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                        //TotalSum = drow["TotalAmount"].ToString();
                    }
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    //OutPut.Add("TotalSum", TotalSum);
                }
                // dt = api.Format(dt);
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
                return StatusCode(403, api.Error(e));
            }
        }

        [HttpGet("settings")]
        public ActionResult CheckSettings(string FormID,int nBranchID,bool bAllBranchData)
        {
            double N_decimalPlace=0;
            bool  B_MultipleLocation=false,B_LocationRequired=false,B_IsPartNoInGrid=false,B_DeptEnable=false,B_DelDays=false,B_Remarks=false,B_CustomerProjectEnabled=false;
            bool B_FileNoVisible=false,B_ShortcutKeyF2=false,B_ShowProject=false,B_ShowProjectInGrid=false,B_FreeDescription=false;

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
                    dt.Columns.Add("B_MultipleLocation");
                    dt.Columns.Add("B_LocationRequired");
                    dt.Columns.Add("B_IsPartNoInGrid");
                    dt.Columns.Add("B_DeptEnable");
                    dt.Columns.Add("B_DelDays");
                    dt.Columns.Add("B_Remarks");
                    dt.Columns.Add("B_CustomerProjectEnabled");
                    dt.Columns.Add("B_FileNoVisible");
                   // dt.Columns.Add("B_ShortcutKeyF2");
                    dt.Columns.Add("B_ShowProject");
                    dt.Columns.Add("B_ShowProjectInGrid");
                    dt.Columns.Add("B_FreeDescription");

                    bool B_EmpFilterByPrj = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("371", "EnablePrjWiseEmp", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    N_decimalPlace = myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "Decimal_Place", "N_Value",myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection));
                    B_MultipleLocation = LocationCount(nBranchID,bAllBranchData, dLayer,connection);
                    B_LocationRequired = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings( "64", "Location_InGrid", "N_Value",myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_IsPartNoInGrid = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings( FormID, "IsPartNoEnable", "N_Value",myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_DeptEnable = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings( "PRS", "Dep_Enable", "N_Value",myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                
                    B_DelDays = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings( FormID, "IdDelDaysingrid", "N_Value",myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_Remarks = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings( FormID, "IsRemarksingrid", "N_Value",myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_CustomerProjectEnabled = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings( "Inventory", "CustomerProject Enabled", "N_Value",myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    B_FileNoVisible = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings( "FileNo", "FileNo_Visible", "N_Value",myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                   // B_ShortcutKeyF2 = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("64", "ShortcutKeyF2", "N_Value", "N_UserCategoryID", myCompanyID._UserCategoryID.ToString(),myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    B_ShowProject = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Purchase", "Enable Project", "N_Value",myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_ShowProjectInGrid = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings( "Purchase", "Enable Project In Request Grid", "N_Value",myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_FreeDescription = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("82", "FreeDescription_InPurchase", "N_Value",myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    DataRow row = dt.NewRow();
                    row["N_decimalPlace"] =myFunctions.getVAL(N_decimalPlace.ToString()) ;
                    row["B_MultipleLocation"] =  B_MultipleLocation;
                    row["B_LocationRequired"] = B_LocationRequired;
                    row["B_IsPartNoInGrid"] = B_IsPartNoInGrid ;
                    row["B_DeptEnable"] = B_DeptEnable;
                    row["B_DelDays"] = B_DelDays;
                    row["B_Remarks"] = B_Remarks   ;
                    row["B_CustomerProjectEnabled"] = B_CustomerProjectEnabled;
                    row["B_FileNoVisible"] = B_FileNoVisible;
                    //row["B_ShortcutKeyF2"] = B_ShortcutKeyF2;
                    row["B_ShowProject"] = B_ShowProject;
                    row["B_ShowProjectInGrid"] = B_ShowProjectInGrid;
                    row["B_FreeDescription"] = B_FreeDescription;
             
                    dt.Rows.Add(row);

                    dt = api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(dt));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }

        }
        private bool LocationCount(int nBranchID,bool bAllBranchData, IDataAccessLayer dLayer, SqlConnection connection)
        {
            object branch_count = null;
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nBranchID", nBranchID);

            if (bAllBranchData)
                branch_count = dLayer.ExecuteScalar("SELECT count(*) FROm Inv_Location  Where N_CompanyID=@nCompanyID", Params, connection);
            else
                branch_count = dLayer.ExecuteScalar("SELECT count(*) FROm Inv_Location  Where N_CompanyID=@nCompanyID and N_BranchID=@nBranchID", Params, connection);


            if (myFunctions.getIntVAL(branch_count.ToString()) == 1)
            {

                return false;
            }
            return true;
        }
 
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPRSID,int nSalesOrderID,int nFnYearID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    dLayer.DeleteData("Inv_PRSDetails", "N_PRSID", nPRSID, "N_CompanyID=" + nCompanyID + " and N_PRSID=" + nPRSID, connection,transaction);
                    Results=dLayer.DeleteData("Inv_PRS", "N_PRSID", nPRSID, "N_CompanyID=" + nCompanyID + " and N_PRSID=" + nPRSID, connection,transaction);

                    if(nSalesOrderID>0)
                    {
                        SortedList DeleteParams = new SortedList(){
                            {"N_CompanyID",nCompanyID},
                            {"N_YearID",nFnYearID},
                            {"N_SalesOrderID",nSalesOrderID}};

                        dLayer.ExecuteNonQueryPro("SP_SalesOrderProcessUpdate", DeleteParams, connection, transaction);
                    }
                
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success( "Request deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(api.Warning("Unable to delete Request"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }


        }

        //Save....
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {

            DataTable MasterTable;
            DataTable DetailTable;
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            // DataTable Approvals;
            // Approvals = ds.Tables["approval"];
            SortedList Params = new SortedList();
            // Auto Gen
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    string X_PRSNo = "",X_TransType="";
                    int N_PRSID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PRSID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int N_FormID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FormID"].ToString());
                    int N_SalesOrderId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_SalesOrderId"].ToString());
                    int N_TransTypeID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TransTypeID"].ToString());
                    int N_UserID = myFunctions.GetUserID(User);
                    int N_NextApproverID=0;
                   // if(N_FormID==556)
                    var values = MasterTable.Rows[0]["X_PRSNo"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID",N_CompanyID);
                        Params.Add("N_YearID",N_FnYearID);
                        Params.Add("N_FormID", N_FormID);
                        Params.Add("N_BranchID", MasterTable.Rows[0]["n_BranchId"].ToString());
                        X_PRSNo = dLayer.GetAutoNumber("Inv_PRS", "X_PRSNo", Params, connection, transaction);
                        if (X_PRSNo == "") { transaction.Rollback(); return Ok(api.Warning("Unable to generate Request Number")); }
                        MasterTable.Rows[0]["X_PRSNo"] = X_PRSNo;
                    }

                    if (N_PRSID > 0)
                    {
                       dLayer.DeleteData("Inv_PRSDetails", "N_PRSID", N_PRSID, "N_CompanyID=" + N_CompanyID + " and N_PRSID=" + N_PRSID, connection, transaction);
                       dLayer.DeleteData("Inv_PRS", "N_PRSID", N_PRSID, "N_CompanyID=" + N_CompanyID + " and N_PRSID=" + N_PRSID, connection, transaction);
                    }

                    N_PRSID = dLayer.SaveData("Inv_PRS", "N_PRSID", MasterTable, connection, transaction);
                    if (N_PRSID <= 0)
                    {
                        transaction.Rollback();
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_PRSID"] = N_PRSID;
                    }
                    int N_PRSDetailsID = dLayer.SaveData("Inv_PRSDetails", "N_PRSDetailsID", DetailTable, connection, transaction);

                    if(N_SalesOrderId>0)
                    {
                        SortedList QueryParams = new SortedList();
                        QueryParams.Add("@N_TransTypeID", N_TransTypeID);
                        QueryParams.Add("@N_SalesOrderId", N_SalesOrderId);
                        QueryParams.Add("@N_CompanyID", N_CompanyID);
                        QueryParams.Add("@N_FnYearID", N_FnYearID);

                        dLayer.ExecuteNonQuery("Update Inv_SalesOrder set N_Processed=1,N_TranTypeID=@N_TransTypeID Where N_SalesOrderId=@N_SalesOrderId and N_CompanyID=@N_CompanyID and N_FnYearID=@N_FnYearID", QueryParams, connection, transaction);
                    }

                    transaction.Commit();

                    SortedList Result = new SortedList();
                    Result.Add("N_PRSID", N_PRSID);
                    Result.Add("X_PRSNo", X_PRSNo);
                    return Ok(api.Success(Result, "Request Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }

        
        [HttpGet("details")]
        public ActionResult GetRequestDetails(string  xPRSNo,int nFnYearID,int N_LocationID, int nBranchID, bool bShowAllBranchData)
        {
            DataTable Master = new DataTable();
            DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@xPRSNo", xPRSNo);
            QueryParams.Add("@nBranchID", nBranchID);
            QueryParams.Add("@nFnYearID", nFnYearID);
            QueryParams.Add("@N_LocationID", N_LocationID);
            string Condition = "";
            string _sqlQuery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (bShowAllBranchData == true)
                        Condition = "n_Companyid=@nCompanyID and X_PRSNo =@xPRSNo and N_FnYearID=@nFnYearID";
                    else
                        Condition = "n_Companyid=@nCompanyID and X_PRSNo =@xPRSNo and N_FnYearID=@nFnYearID and N_BranchID=@nBranchID";


                    _sqlQuery = "Select * from vw_Inv_SRSDetails Where " + Condition + "";

                    Master = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                    Master = api.Format(Master, "master");

                    if (Master.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        QueryParams.Add("@N_PRSID", Master.Rows[0]["N_PRSID"].ToString());

                        ds.Tables.Add(Master);

                        _sqlQuery = "Select *,dbo.SP_Cost(vw_InvPRSDetails.N_ItemID,vw_InvPRSDetails.N_CompanyID,'') As N_UnitLPrice,dbo.[SP_GenGetStock](vw_InvPRSDetails.N_ItemID,@N_LocationID,'','location' ) as Stock,dbo.[SP_Stock](vw_InvPRSDetails.N_ItemID ) as BranchStock from vw_InvPRSDetails Where vw_InvPRSDetails.N_CompanyID=@nCompanyID and vw_InvPRSDetails.N_PRSID=@N_PRSID";
                        Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        Detail = api.Format(Detail, "details");
                        if (Detail.Rows.Count == 0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }
                        ds.Tables.Add(Detail);


                        // DataTable Attachements = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(Master.Rows[0]["N_EmpID"].ToString()), myFunctions.getIntVAL(Master.Rows[0]["N_VacationGroupID"].ToString()), this.FormID, myFunctions.getIntVAL(Master.Rows[0]["N_FnYearID"].ToString()), User, connection);
                        // Attachements = api.Format(Attachements, "attachments");
                        // ds.Tables.Add(Attachements);

                        return Ok(api.Success(ds));
                    }


                }


            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

     [HttpGet("soDetails")]
        public ActionResult GetSODetails(string xOrderNo ,int nFnYearID,int N_LocationID, int nBranchID, bool bShowAllBranchData)
        {
            DataTable Master = new DataTable();
            DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@xOrderNo", xOrderNo);
            QueryParams.Add("@nBranchID", nBranchID);
            QueryParams.Add("@nFnYearID", nFnYearID);
            QueryParams.Add("@N_LocationID", N_LocationID);

            string _sqlQuery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (bShowAllBranchData == true)
                        _sqlQuery = "SP_InvSalesOrder_Disp @nCompanyID,@xOrderNo,0,0";
                    else
                        _sqlQuery = "SP_InvSalesOrder_Disp @nCompanyID,@xOrderNo,0,@nBranchID";

                    Master = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                    Master = api.Format(Master, "master");

                    if (Master.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        QueryParams.Add("@N_SalesOrderID", Master.Rows[0]["N_SalesOrderID"].ToString());

                        ds.Tables.Add(Master);

                        _sqlQuery = "SP_InvSalesOrderDtls_Disp @nCompanyID,@N_SalesOrderID,@nFnYearID,0,@N_LocationID";
                        Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        Detail = api.Format(Detail, "details");
                        if (Detail.Rows.Count == 0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }
                        ds.Tables.Add(Detail);

                        return Ok(api.Success(ds));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }    
        
 [HttpGet("empUserList")]
        public ActionResult GetEmpUser(int nDepartmentID, int nFnYearID, bool bAllBranchData, int nBranchID)

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);

            string sqlCommandText = "";
            if (nDepartmentID > 0)
            {
                if (bAllBranchData == true)
                    sqlCommandText = "Select *  from vw_PayEmployeeUser where N_CompanyID=" + nCompanyID + " and (N_Status = 0 OR N_Status = 1) and N_FnYearID=" + nFnYearID + " and N_DepartmentID =" + nDepartmentID;
                else
                    sqlCommandText = "Select *  from vw_PayEmployeeUser where N_CompanyID=" + nCompanyID + " and (N_Status = 0 OR N_Status = 1) and N_FnYearID=" + nFnYearID + " and (N_BranchID=0 OR N_BranchID=" + nBranchID + ") and N_DepartmentID =" + nDepartmentID;
            }
            else
            {
                if (bAllBranchData == true)
                    sqlCommandText = "Select *  from vw_PayEmployeeUser N_CompanyID=" + nCompanyID + " and (N_Status = 0 OR N_Status = 1) and N_FnYearID=" + nFnYearID + "";
                else
                    sqlCommandText = "Select *  from vw_PayEmployeeUser N_CompanyID=" + nCompanyID + " and (N_Status = 0 OR N_Status = 1) and N_FnYearID=" + nFnYearID + " and (N_BranchID=0 OR N_BranchID=" + nBranchID + ")";
            }

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
                    return Ok(api.Notice("No Results Found"));
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
        [HttpGet("department")]
        public ActionResult GetDepartment(string xLevelPattern)

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);


            string sqlCommandText = "";
            

            if (xLevelPattern != "")
               sqlCommandText = "Select *  from vw_PayDepartment_Disp Where N_CompanyID= " + nCompanyID + " and  X_LevelPattern like '" + xLevelPattern + "%' and isnull(B_Inactive,0)<>1";
            else
                sqlCommandText = "Select *  from vw_PayDepartment_Disp Where N_CompanyID= " + nCompanyID + " and isnull(B_Inactive,0)<>1";
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
                    return Ok(api.Notice("No Results Found"));
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
        [HttpGet("location")]
        public ActionResult GetLocation(int nMainLocID,int nBranchID,bool bAllBranchData)

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nMainLocID", nMainLocID);
            Params.Add("@nBranchID", nBranchID);

            string sqlCommandText = "";
            string sqlCondition = "";
            
            if(nMainLocID>0)
                sqlCondition=" and (N_MainLocationID = @nMainLocID or N_LocationID=@nMainLocID )";

            if (bAllBranchData)
               sqlCommandText = "Select *  from vw_InvLocation Where N_CompanyID= @nCompanyID "+sqlCondition;
            else
                sqlCommandText = "Select *  from vw_InvLocation Where N_CompanyID= @nCompanyID and N_BranchID=@nBranchID "+sqlCondition;
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
                    return Ok(api.Notice("No Results Found"));
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