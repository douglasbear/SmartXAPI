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
    [Route("approval")]
    [ApiController]
    public class Gen_Approvals : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        public Gen_Approvals(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("dashboard")]
        public ActionResult GetApprovalDetails( bool bShowAll, bool bShowAllBranch, int N_Branchid, int nApprovalType)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "";
            string DateCol="D_ApprovedDate";
            
            if(nApprovalType == 0)
            {
                DateCol = "D_ApprovedDate";
                if (bShowAllBranch)
                {
                    if (bShowAll)
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1";
                    else
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1 and N_ReqUserID=@p2";
                }
                else
                {
                    if (bShowAll)
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1 ";
                    else
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1 and N_ReqUserID=@p2 ";
                    Params.Add("@p3", N_Branchid);
                }
            }

            if (nApprovalType == 1)
            {
                DateCol = "D_ApprovedDate";

                if (bShowAllBranch)
                {
                    if (bShowAll)
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1";
                    else
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1 and N_NextApproverID=@p2";
                }
                else
                {
                    if (bShowAll)
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1 ";
                    else
                        sqlCommandText = "select * from vw_ApprovalPending where N_CompanyID=@p1 and N_NextApproverID=@p2 ";
                    Params.Add("@p3", N_Branchid);
                }
    
            }
            else if (nApprovalType == 2)
            {
                DateCol = "X_RequestDate";
                if (bShowAllBranch)
                    sqlCommandText = "select * from vw_ApprovalSummary where N_CompanyID=@p1 and N_ActionUserID=@p2 and N_ProcStatusID<>6";
                else
                {
                    sqlCommandText = "select * from vw_ApprovalSummary where N_CompanyID=@p1 and N_ActionUserID=@p2 and N_ProcStatusID<>6 ";
                    Params.Add("@p3", N_Branchid);
                }

            }
            else if (nApprovalType == 3)
            {
                DateCol = "X_RequestDate";
                if (bShowAllBranch)
                    sqlCommandText = "select * from vw_ApprovalPastRqst where N_CompanyID=@p1 and N_ReqUserID=@p2 and N_ProcStatusID<>6";
                else
                {
                    sqlCommandText = "select * from vw_ApprovalPastRqst where N_CompanyID=@p1 and N_ReqUserID=@p2 and N_ProcStatusID<>6 ";
                    Params.Add("@p3", N_Branchid);
                }

            }else if (nApprovalType == 4)
            {
                DateCol = "X_RequestDate";
                if (bShowAllBranch)
                    //sqlCommandText = "select * from vw_ApprovalReview_Disp where N_CompanyID=@p1 and N_NextApproverID=@p2";
                    sqlCommandText = "SELECT     vw_ApprovalReview_Disp.*, Log_ApprovalProcess.N_ActionUserID FROM         vw_ApprovalReview_Disp LEFT OUTER JOIN Log_ApprovalProcess ON vw_ApprovalReview_Disp.N_FormID = Log_ApprovalProcess.N_FormID AND "
                     + "vw_ApprovalReview_Disp.N_TransID = Log_ApprovalProcess.N_TransID AND vw_ApprovalReview_Disp.N_CompanyID = Log_ApprovalProcess.N_CompanyID AND "
                      + "vw_ApprovalReview_Disp.N_FnYearID = Log_ApprovalProcess.N_FnYearID AND vw_ApprovalReview_Disp.X_TransCode = Log_ApprovalProcess.X_TransCode AND " 
                      + "vw_ApprovalReview_Disp.N_NextApproverID = Log_ApprovalProcess.N_ActionUserID where vw_ApprovalReview_Disp.N_NextApproverID=@p2 and Log_ApprovalProcess.N_ActionUserID is null";
                else
                {
                    // sqlCommandText = "select * from vw_ApprovalReview_Disp where N_CompanyID=@p1 and N_NextApproverID=@p2 ";
                    sqlCommandText = "SELECT     vw_ApprovalReview_Disp.*, Log_ApprovalProcess.N_ActionUserID FROM         vw_ApprovalReview_Disp LEFT OUTER JOIN Log_ApprovalProcess ON vw_ApprovalReview_Disp.N_FormID = Log_ApprovalProcess.N_FormID AND "
                     + "vw_ApprovalReview_Disp.N_TransID = Log_ApprovalProcess.N_TransID AND vw_ApprovalReview_Disp.N_CompanyID = Log_ApprovalProcess.N_CompanyID AND "
                      + "vw_ApprovalReview_Disp.N_FnYearID = Log_ApprovalProcess.N_FnYearID AND vw_ApprovalReview_Disp.X_TransCode = Log_ApprovalProcess.X_TransCode AND " 
                      + "vw_ApprovalReview_Disp.N_NextApproverID = Log_ApprovalProcess.N_ActionUserID where vw_ApprovalReview_Disp.N_NextApproverID=@p2 and Log_ApprovalProcess.N_ActionUserID is null";
                    Params.Add("@p3", N_Branchid);
                }

            }
            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);

            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nUserID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string Col="n_FormID";
                    if(nApprovalType==4){
                        Col="vw_ApprovalReview_Disp.n_FormID";
                    }
                    connection.Open();
                 // dt = dLayer.ExecuteDataTable(sqlCommandText + " and " + Col + " in (212,210,1226,1229,1232,1234,1235,1236,1239,2001,2002,2003,2004,2005,1289,1291) order by "+ DateCol +" desc", Params, connection);
                 dt = dLayer.ExecuteDataTable(sqlCommandText + " order by "+ DateCol +" desc", Params, connection);
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

        [HttpGet("GetApprovalSettings")]
        public ActionResult GetApprovalSettings(int nIsApprovalSystem, int nFormID, int nTransID, int nTransUserID, int nTransStatus, int nTransApprovalLevel, int nNextApprovalLevel, int nApprovalID, int nGroupID, int nFnYearID, int nEmpID, int nActionID)
        {
            



            try
            {
            using (SqlConnection connection = new SqlConnection(connectionString))
                {
                connection.Open();
                SortedList Response = myFunctions.GetApprovals(nIsApprovalSystem, nFormID, nTransID, nTransUserID, nTransStatus, nTransApprovalLevel, nNextApprovalLevel, nApprovalID, nGroupID, nFnYearID, nEmpID, nActionID,User,dLayer, connection);
                return Ok(api.Success(Response));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }

        }

        [HttpGet("list")]
        public ActionResult GetApprovalCode ()
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            
            DataTable dt=new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1", nCompanyId);
            
            sqlCommandText="Select * from Sec_ApprovalSettings_Employee where N_CompanyID=@p1";
                
            try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(api.Notice("No Results Found"));
                        }else{
                            return Ok(api.Success(dt));
                        }
                
            }catch(Exception e){
                return Ok(api.Error(e));
            }   
        }


        [HttpPost("updateApproval")]
        public ActionResult UpdateApproval([FromBody] DataSet ds)
        {
            try
            {
                DataTable Approvals;
                Approvals = ds.Tables["approval"];
                DataRow ApprovalRow = Approvals.Rows[0];

                int N_NextApproverID = 0;

string status="";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlTransaction transaction = connection.BeginTransaction();
                    
                    // if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()))
                    // {
                        int N_PkeyID = myFunctions.getIntVAL(ApprovalRow["n_TransID"].ToString());
                        int nCompanyID = myFunctions.getIntVAL(ApprovalRow["n_CompanyID"].ToString());
                        int nFnYearID = myFunctions.getIntVAL(ApprovalRow["n_FnYearID"].ToString());
                        string type = ApprovalRow["type"].ToString();
                        // string X_Criteria = "N_RequestID=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                        string X_Criteria = "";
                        string tableName="";
                        object partyId =null;
                        
                        switch(ApprovalRow["n_FormID"].ToString()){
                            case "82":
                            X_Criteria=" N_POrderID=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                            tableName="Inv_PurchaseOrder";
                            partyId = dLayer.ExecuteScalar("Select N_VendorID from Inv_PurchaseOrder where "+X_Criteria,connection, transaction);
                            break;
                            case "65":
                            X_Criteria=" N_PurchaseID=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                            tableName="Inv_Purchase";
                            partyId = dLayer.ExecuteScalar("Select N_VendorID from Inv_Purchase where "+X_Criteria,connection, transaction);
                            break;
                            case "68":
                            X_Criteria=" N_CreditNoteId=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                            tableName="Inv_PurchaseReturnMaster";
                            partyId = dLayer.ExecuteScalar("Select N_VendorID from Inv_PurchaseReturnMaster where "+X_Criteria,connection, transaction);
                            break;
                            case "80":
                            X_Criteria=" N_QuotationId=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                            tableName="Inv_SalesQuotation";
                            partyId = dLayer.ExecuteScalar("Select N_CustomerID from Inv_SalesQuotation where "+X_Criteria,connection, transaction);

                            break;
                            case "81":
                            X_Criteria=" N_SalesOrderId=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                            tableName="Inv_SalesOrder";
                            partyId = dLayer.ExecuteScalar("Select N_CustomerID from Inv_SalesOrder where "+X_Criteria,connection, transaction);

                            break;
                            case "64":
                            X_Criteria=" N_SalesId=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                            tableName="Inv_Sales";
                            partyId = dLayer.ExecuteScalar("Select N_CustomerID from Inv_Sales where "+X_Criteria,connection, transaction);

                            break;
                            case "55":
                            X_Criteria=" N_DebitNoteId=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                            tableName="Inv_SalesReturnMaster";
                            partyId = dLayer.ExecuteScalar("Select N_CustomerID from Inv_SalesReturnMaster where "+X_Criteria,connection, transaction);

                            break;
                            default:
                            return Ok(api.Error("Invalid Form"));
                            

                        }
                        if(type=="approve")
                        {myFunctions.UpdateApproverEntry(Approvals, tableName, X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        status="Approved";}
                        else
                        {
                            string ButtonTag = ApprovalRow["deleteTag"].ToString();
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());

                     status = myFunctions.UpdateApprovals(Approvals, nFnYearID, ApprovalRow["x_EntryForm"].ToString(), N_PkeyID,ApprovalRow["x_TransCode"].ToString(), ProcStatus, tableName, X_Criteria, "", User, dLayer, connection, transaction);
                   
                        }
if(partyId==null){
    partyId=0;
}
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID,  ApprovalRow["x_EntryForm"].ToString(), N_PkeyID, ApprovalRow["x_TransCode"].ToString(), 1, ApprovalRow["x_PartyName"].ToString(), myFunctions.getIntVAL(partyId.ToString()), "", User, dLayer, connection, transaction);
                        transaction.Commit();
                        
               
                    return Ok(api.Success(status));

                    
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }
    }
}