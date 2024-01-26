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
        public ActionResult GetRequestList(int nCompanyId, int nFnYearId, int nBranchID, bool bAllBranchData, int FormID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nLocationID,bool isRecieved)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string sqlCondition = "";
            string Searchkey = "";
            string transferCondition = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([PRS No] like '%" + xSearchkey + "%' or X_ProjectName like '%" + xSearchkey + "%' or location like '%" + xSearchkey + "%' or X_DeliveryPlace like '%" + xSearchkey + "%' or  X_Purpose like '%" + xSearchkey + "%' or  [PRS No] like '%" + xSearchkey + "%' or  D_PRSDate like '%" + xSearchkey + "%' or  X_requestType like '%" + xSearchkey + "%' or  X_CustomerDocNO like '%" + xSearchkey + "%' )";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_PRSID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "x_PRSNo":
                         xSortBy = "[PRS No] " + xSortBy.Split(" ")[1];
                        break;
                    case "d_PRSDate":
                        xSortBy = "d_PRSDate " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }
            if(FormID==1592 && isRecieved)
            {
              transferCondition=" and N_Processed=2 ";
            }
            else if(FormID==1592 && !isRecieved)
            {
                transferCondition=" and N_Processed!=2 ";
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
                    if (!myFunctions.CheckClosedYear(nCompanyId, nFnYearId, dLayer, connection))
                    {
                        if(FormID==1592)
                        { 
                         sqlCondition= "N_CompanyID=@nCompanyId and B_YearEndProcess=0 and N_FormID=@FormID and (N_LocationIDFrom ="+nLocationID+" or N_LocationID="+nLocationID+")  ";
                        }
                        else 
                        {
                            if (bAllBranchData)
                                sqlCondition = "N_CompanyID=@nCompanyId and B_YearEndProcess=0 and N_FormID=@FormID";
                            else
                                sqlCondition = "N_CompanyID=@nCompanyId and N_BranchID=@nBranchID and B_YearEndProcess=0 and N_FormID=@FormID";
                        }
                    }
                    else
                    {
                        if(FormID==1592)
                        { 
                         sqlCondition= "N_CompanyID=@nCompanyId and N_FormID=@FormID and (N_LocationIDFrom ="+nLocationID+" or N_LocationID="+nLocationID+" ) ";
                        }
                        else
                        {
                        if (bAllBranchData)
                            sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and N_FormID=@FormID";
                        else
                            sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and N_BranchID=@nBranchID and N_FormID=@FormID";

                        }
                    }

                    if (Count == 0)
                        sqlCommandText = "select  top(" + nSizeperpage + ") [PRS No] as x_PRSNo,* from vw_InvPRSNo_UCSearch where " + sqlCondition + " " + transferCondition + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select  top(" + nSizeperpage + ") [PRS No] as x_PRSNo,* from vw_InvPRSNo_UCSearch where " + sqlCondition + " " + transferCondition + Searchkey + " and N_PRSID not in (select top(" + Count + ") N_PRSID from vw_InvPRSNo_UCSearch where " + sqlCondition + " "+ transferCondition + xSortBy + " ) " + xSortBy;


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count from vw_InvPRSNo_UCSearch where " + sqlCondition + " " + transferCondition + Searchkey + "";
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
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("settings")]
        public ActionResult CheckSettings(string FormID, int nBranchID, bool bAllBranchData)
        {
            double N_decimalPlace = 0;
            bool B_MultipleLocation = false, B_LocationRequired = false, B_IsPartNoInGrid = false, B_DeptEnable = false, B_DelDays = false, B_Remarks = false, B_CustomerProjectEnabled = false;
            bool B_FileNoVisible = false, B_ShortcutKeyF2 = false, B_ShowProject = false, B_ShowProjectInGrid = false, B_FreeDescription = false;

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
                    ////


                    bool B_EmpFilterByPrj = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("371", "EnablePrjWiseEmp", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    N_decimalPlace = myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "Decimal_Place", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection));
                    B_MultipleLocation = LocationCount(nBranchID, bAllBranchData, dLayer, connection);
                    B_LocationRequired = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("64", "Location_InGrid", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_IsPartNoInGrid = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings(FormID, "IsPartNoEnable", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_DeptEnable = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("PRS", "Dep_Enable", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    B_DelDays = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings(FormID, "IdDelDaysingrid", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_Remarks = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings(FormID, "IsRemarksingrid", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_CustomerProjectEnabled = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "CustomerProject Enabled", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    B_FileNoVisible = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("FileNo", "FileNo_Visible", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    // B_ShortcutKeyF2 = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("64", "ShortcutKeyF2", "N_Value", "N_UserCategoryID", myCompanyID._UserCategoryID.ToString(),myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    B_ShowProject = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Purchase", "Enable Project", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_ShowProjectInGrid = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Purchase", "Enable Project In Request Grid", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_FreeDescription = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("82", "FreeDescription_InPurchase", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    DataRow row = dt.NewRow();
                    row["N_decimalPlace"] = myFunctions.getVAL(N_decimalPlace.ToString());
                    row["B_MultipleLocation"] = B_MultipleLocation;
                    row["B_LocationRequired"] = B_LocationRequired;
                    row["B_IsPartNoInGrid"] = B_IsPartNoInGrid;
                    row["B_DeptEnable"] = B_DeptEnable;
                    row["B_DelDays"] = B_DelDays;
                    row["B_Remarks"] = B_Remarks;
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
                return Ok(api.Error(User,e));
            }

        }
        private bool LocationCount(int nBranchID, bool bAllBranchData, IDataAccessLayer dLayer, SqlConnection connection)
        {
            object branch_count = null;
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nBranchID", nBranchID);

            if (bAllBranchData)
                branch_count = dLayer.ExecuteScalar("SELECT count(1) FROm Inv_Location  Where N_CompanyID=@nCompanyID", Params, connection);
            else
                branch_count = dLayer.ExecuteScalar("SELECT count(1) FROm Inv_Location  Where N_CompanyID=@nCompanyID and N_BranchID=@nBranchID", Params, connection);


            if (myFunctions.getIntVAL(branch_count.ToString()) == 1)
            {

                return false;
            }
            return true;
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPRSID, int nSalesOrderID, int nFnYearID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    SqlTransaction transaction = connection.BeginTransaction();
                     ParamList.Add("@nTransID", nPRSID);
                     ParamList.Add("@nCompanyID", nCompanyID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    string xButtonAction="Delete";
                    string X_PRSNo="";
                     string Sql = "select N_PRSID,X_PRSNo from Inv_PRS where N_PRSID=@nTransID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID";

                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection,transaction);
                     
                     
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(nFnYearID,nPRSID,TransRow["X_PRSNo"].ToString(),1309,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                         


                    dLayer.DeleteData("Inv_PRSDetails", "N_PRSID", nPRSID, "N_CompanyID=" + nCompanyID + " and N_PRSID=" + nPRSID, connection, transaction);
                    Results = dLayer.DeleteData("Inv_PRS", "N_PRSID", nPRSID, "N_CompanyID=" + nCompanyID + " and N_PRSID=" + nPRSID, connection, transaction);
                  object objMDProcessed = dLayer.ExecuteScalar("Select Isnull(N_RSID,0) from Inv_MaterialDispatch where N_CompanyId=" + nCompanyID + " and N_RSID=" +nPRSID  + " ", connection, transaction);
                    if (objMDProcessed == null)
                        objMDProcessed = 0;
                    if (nSalesOrderID > 0)
                    {
                        SortedList DeleteParams = new SortedList(){
                            {"N_CompanyID",nCompanyID},
                            {"N_YearID",nFnYearID},
                            {"N_SalesOrderID",nSalesOrderID}};

                        dLayer.ExecuteNonQueryPro("SP_SalesOrderProcessUpdate", DeleteParams, connection, transaction);
                    }
           
                       if (myFunctions.getIntVAL(objMDProcessed.ToString()) > 0)
                       {
                       return Ok(api.Error(User, "Unable to delete"));
                       }
                            

                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("Request deleted"));
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
                return Ok(api.Error(User,ex));
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
            string xButtonAction="";
            // Auto Gen
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    string X_PRSNo = "", X_TransType = "";
                    int N_PRSID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PRSID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                     int n_OldFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                     int N_FormID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FormID"].ToString());
                    var N_FnYearID = MasterTable.Rows[0]["N_FnYearID"].ToString();
                    int N_SalesOrderId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_SalesOrderId"].ToString());
                    int N_TransTypeID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TransTypeID"].ToString());
                    int N_UserID = myFunctions.GetUserID(User);
                    int N_NextApproverID = 0;
                 
                          if (!myFunctions.CheckActiveYearTransaction(myFunctions.getIntVAL(N_CompanyID.ToString()),myFunctions.getIntVAL(N_FnYearID.ToString()), DateTime.ParseExact(MasterTable.Rows[0]["d_PRSDate"].ToString(), "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture), dLayer, connection, transaction))
                        {
                            object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID="+N_CompanyID+" and convert(date ,'" + MasterTable.Rows[0]["d_PRSDate"].ToString() + "') between D_Start and D_End", connection, transaction);
                            if (DiffFnYearID != null)
                            {
                                MasterTable.Rows[0]["N_FnYearID"] = DiffFnYearID.ToString();
                                 foreach (DataRow var in MasterTable.Rows)
                                 {
                                    var["n_FnYearID"]=DiffFnYearID.ToString();
                                 }
                                MasterTable.AcceptChanges();
                                N_FnYearID = DiffFnYearID.ToString();
                                  
                            }
                            else
                            {
                                transaction.Rollback();
                                return Ok(api.Error(User, "Transaction date must be in the active Financial Year."));
                            }
                        }

                    // if(N_FormID==556)
                    var values = MasterTable.Rows[0]["X_PRSNo"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", N_FormID);
                        Params.Add("N_BranchID", MasterTable.Rows[0]["n_BranchId"].ToString());
                        X_PRSNo = dLayer.GetAutoNumber("Inv_PRS", "X_PRSNo", Params, connection, transaction);
                         xButtonAction="Insert"; 
                        if (X_PRSNo == "") { transaction.Rollback(); return Ok(api.Warning("Unable to generate Request Number")); }
                        MasterTable.Rows[0]["X_PRSNo"] = X_PRSNo;
                    }
                      X_PRSNo = MasterTable.Rows[0]["X_PRSNo"].ToString();

                    if (N_PRSID > 0)
                    {
                        dLayer.DeleteData("Inv_PRSDetails", "N_PRSID", N_PRSID, "N_CompanyID=" + N_CompanyID + " and N_PRSID=" + N_PRSID, connection, transaction);
                        dLayer.DeleteData("Inv_PRS", "N_PRSID", N_PRSID, "N_CompanyID=" + N_CompanyID + " and N_PRSID=" + N_PRSID, connection, transaction);
                          xButtonAction="Update"; 
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

                    if (N_SalesOrderId > 0)
                    {
                        SortedList QueryParams = new SortedList();
                        QueryParams.Add("@N_TransTypeID", N_TransTypeID);
                        QueryParams.Add("@N_SalesOrderId", N_SalesOrderId);
                        QueryParams.Add("@N_CompanyID", N_CompanyID);
                        QueryParams.Add("@N_FnYearID", N_FnYearID);

                        dLayer.ExecuteNonQuery("Update Inv_SalesOrder set N_Processed=1,N_TranTypeID=@N_TransTypeID Where N_SalesOrderId=@N_SalesOrderId and N_CompanyID=@N_CompanyID and N_FnYearID=@N_FnYearID", QueryParams, connection, transaction);
                       
                    }
                //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(myFunctions.getIntVAL(N_FnYearID.ToString()),N_PRSID,X_PRSNo,N_FormID,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                          
                          

                    transaction.Commit();

                    SortedList Result = new SortedList();
                    Result.Add("N_PRSID", N_PRSID);
                    Result.Add("X_PRSNo", X_PRSNo);
                    return Ok(api.Success(Result, "Request Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }


        [HttpGet("details")]
        public ActionResult GetRequestDetails(string xPRSNo, int nFnYearID, int N_LocationID, int nBranchID, bool bShowAllBranchData,int formID)
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
                    if(formID<=0)
                    {
                    if (bShowAllBranchData == true)
                        Condition = "n_Companyid=@nCompanyID and X_PRSNo =@xPRSNo and N_FnYearID=@nFnYearID";
                    else
                        Condition = "n_Companyid=@nCompanyID and X_PRSNo =@xPRSNo and N_FnYearID=@nFnYearID and N_BranchID=@nBranchID";
                    }
                    else
                    {
                         Condition = "n_Companyid=@nCompanyID and X_PRSNo =@xPRSNo and N_FnYearID=@nFnYearID";
                    }

                    _sqlQuery = "Select * from vw_Inv_SRSDetails Where " + Condition + "";

                    Master = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                    Master = api.Format(Master, "master");

                     object RfqID=0;
                      RfqID = dLayer.ExecuteScalar("Select count(*) from Inv_VendorRequestDetails where N_PRSID=" + myFunctions.getIntVAL(Master.Rows[0]["N_PRSID"].ToString()) + " and N_CompanyID=" + companyid, connection);
                      if (RfqID==null){
                        RfqID=0;
                      }
                         if (myFunctions.getIntVAL(RfqID.ToString())>0){
                            Master = myFunctions.AddNewColumnToDataTable(Master, "isreqstProcessed", typeof(Boolean), true);
                         }
                         else{
                            Master = myFunctions.AddNewColumnToDataTable(Master, "isreqstProcessed", typeof(Boolean), false);
                         }
              
                           Master.AcceptChanges();

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
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("soDetails")]
        public ActionResult GetSODetails(string xOrderNo, int nFnYearID, int N_LocationID, int nBranchID, bool bShowAllBranchData)
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
                return Ok(api.Error(User,e));
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
                    sqlCommandText = "Select *  from vw_PayEmployeeUser where  N_CompanyID=" + nCompanyID + " and (N_Status = 0 OR N_Status = 1) and N_FnYearID=" + nFnYearID + "";
                else
                    sqlCommandText = "Select *  from vw_PayEmployeeUser where  N_CompanyID=" + nCompanyID + " and (N_Status = 0 OR N_Status = 1) and N_FnYearID=" + nFnYearID + " and (N_BranchID=0 OR N_BranchID=" + nBranchID + ")";
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
                return Ok(api.Error(User,e));
            }
        }
        [HttpGet("department")]
        public ActionResult GetDepartment(string xLevelPattern,int nFnYearID)

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
             Params.Add("@nFnYearID", nFnYearID);


            string sqlCommandText = "";


            if (xLevelPattern != "")
                sqlCommandText = "Select *  from vw_PayDepartment_Disp Where N_CompanyID= " + nCompanyID + "and  N_FnYearID= " + nFnYearID + " and  X_LevelPattern like '" + xLevelPattern + "%' and isnull(B_Inactive,0)<>1";
            else
                sqlCommandText = "Select *  from vw_PayDepartment_Disp Where N_CompanyID= " + nCompanyID + "and  N_FnYearID= " + nFnYearID + " and isnull(B_Inactive,0)<>1";
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
                return Ok(api.Error(User,e));
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
                return Ok(api.Error(User,e));
            }

        }
        
  [HttpGet("TimeLinelist")]
        public ActionResult GetList(int nPRSID)

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nPRSID",nPRSID);
            string sqlCommandText="Select FORMAT (d_Date, 'dd-MMM-yyyy') as date,type as name,x_DocNo as s,'' as t from vw_PRSTimeLine where P_KeyID=@nPRSID";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
                return Ok(api.Error(User,e));
            }
        }

    }
} 
 