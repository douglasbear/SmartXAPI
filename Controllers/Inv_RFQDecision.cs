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
    [Route("rfqDecision")]
    [ApiController]
    public class Inv_RFQDecision : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
         private readonly IMyAttachments myAttachments;
        private readonly int FormID = 955;

        public Inv_RFQDecision(IApiFunctions apiFun, IDataAccessLayer dl, IMyFunctions myFun,IMyAttachments myAtt, IConfiguration conf)
        {
            api = apiFun;
            dLayer = dl;
            myAttachments = myAtt;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID=955;
        }



        [HttpGet("list")]
        public ActionResult GetList(int nCompanyId, int nFnYearId, int nBranchID, bool bAllBranchData, int FormID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string sqlCondition = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_RFQDecisionCode like '%" + xSearchkey + "%' OR X_QuotationNo like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_RFQDecisionID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_RFQDecisionCode":
                        xSortBy = "X_RFQDecisionCode " + xSortBy.Split(" ")[1];
                        break;
                    case "N_RFQDecisionID":
                        xSortBy = "N_RFQDecisionID " + xSortBy.Split(" ")[1];
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

                    if (bAllBranchData)
                        sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId";
                    else
                        sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and N_BranchID=@nBranchID";


                    if (Count == 0)
                        sqlCommandText = "select  top(" + nSizeperpage + ") * from vw_RFQDecisionMaster where " + sqlCondition + " " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select  top(" + nSizeperpage + ") * from vw_RFQDecisionMaster where " + sqlCondition + " " + Searchkey + " and N_RFQDecisionID not in (select top(" + Count + ") N_RFQDecisionID from vw_RFQDecisionMaster where " + sqlCondition + " " + xSortBy + " ) " + xSortBy;


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(1) as N_Count from vw_RFQDecisionMaster where " + sqlCondition + " " + Searchkey + "";
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

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nRFQDecisionID,int nFnYearID,int nCustomerId, string comments)
        {
            int Results = 0;
             DataTable TransData = new DataTable();
             SortedList ParamList=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            //  DataTable TransData = new DataTable();
            //  DataRow TransRow = TransData.Rows[0];
            //  int N_RFQDecisionID = myFunctions.getIntVAL(TransRow["N_RFQDecisionID"].ToString());
             SortedList Params = new SortedList();
              string xButtonAction="Delete";
              String X_RFQDecisionCode="";
            if (comments == null)
            {
                comments = "";
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                   

                    
              object n_FnYearID = dLayer.ExecuteScalar("select N_FnyearID from Inv_RFQDecisionMaster where N_RFQDecisionID =" + nRFQDecisionID + " and N_CompanyID=" + nCompanyID, Params, connection); 
                   
                    
                    ParamList.Add("@nRFQDecisionID", nRFQDecisionID);
                    ParamList.Add("@nCompanyID", nCompanyID);
                    ParamList.Add("@nFnYearID", nFnYearID);

               string Sql = "select N_RFQDecisionID,X_RFQDecisionCode,N_UserID,N_ProcStatus,N_ApprovalLevelId from Inv_RFQDecisionMaster where n_RFQDecisionID=@nRFQDecisionID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID";
               TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];
                   DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, 1278, nRFQDecisionID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID, 0, 0, User, dLayer, connection));
                   Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), comments);
                   SqlTransaction transaction = connection.BeginTransaction();
                    //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(myFunctions.getIntVAL( n_FnYearID.ToString()),nRFQDecisionID,X_RFQDecisionCode,955,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);

                    string X_Criteria = "N_RFQDecisionID=" + nRFQDecisionID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                    string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());
                    
                     string status = myFunctions.UpdateApprovals(Approvals, nFnYearID, "RFQ DECISION", nRFQDecisionID, TransRow["X_RFQDecisionCode"].ToString(), ProcStatus, "Inv_RFQDecisionMaster", X_Criteria, "", User, dLayer, connection, transaction);

                      if (status != "Error")
                    {
                    if (ButtonTag == "6" || ButtonTag == "0")
                 {
                    dLayer.DeleteData("Inv_RFQDecisionDetails", "N_RFQDecisionID", nRFQDecisionID, "N_CompanyID=" + nCompanyID + " and N_RFQDecisionID=" + nRFQDecisionID, connection, transaction);
                    Results = dLayer.DeleteData("Inv_RFQDecisionMaster", "N_RFQDecisionID", nRFQDecisionID, "N_CompanyID=" + nCompanyID + " and N_RFQDecisionID=" + nRFQDecisionID, connection, transaction);
                 }}

                         transaction.Commit();
                    return Ok(api.Success("RFQ Decision " + status + " Successfully"));

                    if (Results > 0)
                    {

                         myAttachments.DeleteAttachment(dLayer, 1, nRFQDecisionID, nCustomerId, nFnYearID, this.FormID, User, transaction, connection);
                        transaction.Commit();
                        return Ok(api.Success("RFQ Decision deleted"));
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
            DataTable CustomerInfo;
            DataTable Approvals;
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            Approvals = ds.Tables["approval"];
             DataRow ApprovalRow = Approvals.Rows[0];
            SortedList Params = new SortedList();
            String xButtonAction="";
            int N_NextApproverID = 0;
            int N_SaveDraft = 0;
            // Auto Gen
            try
            {

               
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable Attachment = ds.Tables["attachments"];
                    int N_RFQDecisionID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_RFQDecisionID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                     string X_RFQDecisionCode =MasterTable.Rows[0]["X_RFQDecisionCode"].ToString();
                    int n_RFQDecisionID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_RFQDecisionID"].ToString());
                    int N_UserID = myFunctions.GetUserID(User);
                    int N_CustomerId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CustomerId"].ToString());
                    CustomerInfo = dLayer.ExecuteDataTable("Select X_CustomerCode,X_CustomerName from Inv_Customer where N_CustomerID="+ N_CustomerId, Params, connection, transaction);

                     if (MasterTable.Columns.Contains("n_CustomerId"))
                      MasterTable.Columns.Remove("n_CustomerId");

                    var values = MasterTable.Rows[0]["X_RFQDecisionCode"].ToString();


                       if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()) && N_RFQDecisionID>0)
                     {                                     
                        int N_PkeyID = N_RFQDecisionID;
                        var value = MasterTable.Rows[0]["x_RFQDecisionCode"].ToString();
                        string X_Criteria = "N_RFQDecisionID=" + N_PkeyID + " and N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID;
                        myFunctions.UpdateApproverEntry(Approvals, "Inv_RFQDecisionMaster", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, N_FnYearID, "RFQ DECISION", N_PkeyID, value, 1, "", 0, "", 0, User, dLayer, connection, transaction);
                        // N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Inv_PurchaseOrder where n_POrderID=" + N_POrderID + " and N_CompanyID=" + nCompanyId + " and N_FnYearID=" + N_FnYearID, connection, transaction).ToString());
                          transaction.Commit();
                        return Ok(api.Success("RFQ Decision Approved " + "-" + X_RFQDecisionCode));
                    }



                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", FormID);
                        X_RFQDecisionCode = dLayer.GetAutoNumber("Inv_RFQDecisionMaster", "X_RFQDecisionCode", Params, connection, transaction);
                         xButtonAction="Insert"; 
                        if (X_RFQDecisionCode == "") { transaction.Rollback(); return Ok(api.Warning("Unable to generate Request Decision Number")); }
                        MasterTable.Rows[0]["X_RFQDecisionCode"] = X_RFQDecisionCode;
                    }

                    if (N_RFQDecisionID > 0)
                    {
                        dLayer.DeleteData("Inv_RFQDecisionDetails", "N_RFQDecisionID", N_RFQDecisionID, "N_CompanyID=" + N_CompanyID + " and N_RFQDecisionID=" + N_RFQDecisionID, connection, transaction);
                        dLayer.DeleteData("Inv_RFQDecisionMaster", "N_RFQDecisionID", N_RFQDecisionID, "N_CompanyID=" + N_CompanyID + " and N_RFQDecisionID=" + N_RFQDecisionID, connection, transaction);
                    }

                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);
                    N_RFQDecisionID = dLayer.SaveData("Inv_RFQDecisionMaster", "N_RFQDecisionID", MasterTable, connection, transaction);



                      if (CustomerInfo.Rows.Count > 0)
                    {
                        try
                        {
                            myAttachments.SaveAttachment(dLayer, Attachment,X_RFQDecisionCode, N_RFQDecisionID, CustomerInfo.Rows[0]["X_CustomerName"].ToString().Trim(), CustomerInfo.Rows[0]["X_CustomerCode"].ToString(), N_CustomerId, "Customer Document", User, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User, ex));
                        }
                    }

                   xButtonAction="Update"; 
                    if (N_RFQDecisionID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_RFQDecisionID"] = N_RFQDecisionID;
                    }
            

            // Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(N_FnYearID,N_RFQDecisionID,X_RFQDecisionCode,955,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                          
                          
                    int N_RFQDecisionDetailsID = dLayer.SaveData("Inv_RFQDecisionDetails", "N_RFQDecisionDetailsID", DetailTable, connection, transaction);

                    if (N_RFQDecisionDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }

                     N_NextApproverID = myFunctions.LogApprovals(Approvals, N_FnYearID, "RFQ DECISION", N_RFQDecisionID, X_RFQDecisionCode, 1, "", 0, "",0, User, dLayer, connection, transaction);
                    transaction.Commit();

                    SortedList Result = new SortedList();
                    Result.Add("N_RFQDecisionID", N_RFQDecisionID);
                    Result.Add("X_RFQDecisionCode", X_RFQDecisionCode);
                    return Ok(api.Success(Result, "Request Saved"));
                }
                
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }


        [HttpGet("details")]
        public ActionResult GetDetails(string xRFQDecisionCode, int nFnYearID, int nBranchID, bool bShowAllBranchData)
        {
            DataTable Master = new DataTable();
            DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();
            
            

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@xRFQDecisionCode", xRFQDecisionCode);
            QueryParams.Add("@nBranchID", nBranchID);
            QueryParams.Add("@nFnYearID", nFnYearID);
            
            string Condition = "";
            string _sqlQuery = "";
            string Mastersql = "";


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (bShowAllBranchData == true)
                        Condition = "n_Companyid=@nCompanyID and X_RFQDecisionCode =@xRFQDecisionCode and N_FnYearID=@nFnYearID";
                    else
                        Condition = "n_Companyid=@nCompanyID and X_RFQDecisionCode =@xRFQDecisionCode and N_FnYearID=@nFnYearID and N_BranchID=@nBranchID";


                    Mastersql = "Select * from vw_RFQDecisionMaster Where " + Condition + "";

                    Master = dLayer.ExecuteDataTable(Mastersql, QueryParams, connection);

                    Master = api.Format(Master, "master");

                     DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(Master.Rows[0]["N_CustomerID"].ToString()), myFunctions.getIntVAL(Master.Rows[0]["N_RFQDecisionID"].ToString()), this.FormID, myFunctions.getIntVAL(Master.Rows[0]["N_FnYearID"].ToString()), User, connection);
                    Attachments = api.Format(Attachments, "attachments");


                //    object value = dLayer.ExecuteScalar("select ISNULL(N_QuotationID,0) from Inv_SalesQuotation where N_RFQID = '" + Master.Rows[0]["N_RFQDecisionID"].ToString() + "' and N_CompanyID = '" + companyid + "'", connection);
                //    if (value==null)
                //    value=0;
                //    if(myFunctions.getVAL(value.ToString())!=0){
                //         myFunctions.AddNewColumnToDataTable(Master, "quotationProcessed", typeof(Boolean), true);
                //    }
                //    else{
                //         myFunctions.AddNewColumnToDataTable(Master, "quotationProcessed", typeof(Boolean), false);
                //    }
                  
                    if (Master.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        QueryParams.Add("@N_RFQDecisionID", Master.Rows[0]["N_RFQDecisionID"].ToString());

                        ds.Tables.Add(Master);

                        _sqlQuery = "Select * from vw_RFQDecisionDetails Where N_CompanyID=@nCompanyID and N_RFQDecisionID=@N_RFQDecisionID";
                        Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        Detail = api.Format(Detail, "details");
                        if (Detail.Rows.Count == 0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }
                        ds.Tables.Add(Detail);
                        ds.Tables.Add(Attachments);

                            

                        return Ok(api.Success(ds));
                    }
          
                 
                 

  


                }


            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("VendorList")]
        public ActionResult GetVendorList(int nQuotationDetailsID,int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nQuotationDetailsID", nQuotationDetailsID);
            Params.Add("@nFnYearID", nFnYearID);

            string sqlCommandText = "";

            sqlCommandText = "Select * from vw_RFQVendorListDetails where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_QuotationDetailsID=@nQuotationDetailsID";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
                return Ok(api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }


    }
}