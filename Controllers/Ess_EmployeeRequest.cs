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
using System.Security.Claims;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("employeerequest")]
    [ApiController]



    public class Ess_EmployeeRequest : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly IMyAttachments myAttachments;
        private readonly int FormID;


        public Ess_EmployeeRequest(IDataAccessLayer dl, IApiFunctions _api, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            dLayer = dl;
            api = _api;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1234;
        }


        //List
       [HttpGet("list")]
        public ActionResult GetEmpReqList(string xReqType, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();
            string sqlCommandCount = "";
            int nUserID = myFunctions.GetUserID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);
            QueryParams.Add("@nCompanyID", nCompanyID);
            QueryParams.Add("@nUserID", nUserID);
            string sqlCommandText = "";
            int Count = (nPage - 1) * nSizeperpage;
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_RequestCode like'%" + xSearchkey + "%'or X_RequestTypeDesc like'%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_RequestCode desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Pay_EmpAnyRequestList where  N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and X_Status='Approved'" + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Pay_EmpAnyRequestList where  N_EmpID=@nEmpID and N_CompanyID=@nCompanyID " + Searchkey + " and N_RequestID not in (select top(" + Count + ") N_RequestID from vw_Pay_EmpAnyRequestList where  N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and X_Status='Approved'" + xSortBy + " ) " + xSortBy;

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object nEmpID = dLayer.ExecuteScalar("Select N_EmpID From Sec_User where N_UserID=@nUserID and N_CompanyID=@nCompanyID", QueryParams, connection);
                    if (nEmpID != null)
                    {
                        QueryParams.Add("@nEmpID", myFunctions.getIntVAL(nEmpID.ToString()));
                        dt = dLayer.ExecuteDataTable(sqlCommandText, QueryParams, connection);
                        sqlCommandCount = "select count(*) as N_Count from vw_Pay_EmpAnyRequestList where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and X_Status='Approved' " + Searchkey + "";
                        object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, QueryParams, connection);
                        OutPut.Add("Details", api.Format(dt));
                        OutPut.Add("TotalCount", TotalCount);
                    }
                    else
                    {
                        return Ok(api.Notice("No Results Found"));
                    }


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


        [HttpGet("requesttypes")]
        public ActionResult GetEmployeeRequestTypes(int? nCompanyID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "select N_RequestTypeID,X_RequestTypeDesc from Pay_EmployeeRequestType where N_CompanyID=@nCompanyID and N_RequestTypeID <> 2004 and N_RequestTypeID<>2003";
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


        [HttpGet("details")]
        public ActionResult GetEmployeeLoanDetails(string xRequestCode)
        {
            DataSet Result = new DataSet();
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@xRequestCode", xRequestCode);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string _sqlQuery = "SELECT     Pay_EmpAnyRequest.N_CompanyID, Pay_EmpAnyRequest.N_BranchID, Pay_EmpAnyRequest.N_FnYearID, Pay_EmpAnyRequest.N_RequestID, Pay_EmpAnyRequest.N_RequestType, Pay_EmpAnyRequest.N_EmpID, Pay_EmpAnyRequest.D_RequestDate, Pay_EmpAnyRequest.D_EntryDate, Pay_EmpAnyRequest.D_DateFrom, Pay_EmpAnyRequest.D_DateTo, Pay_EmpAnyRequest.X_Notes, Pay_EmpAnyRequest.X_Remarks, Pay_EmpAnyRequest.N_ApprovalLevelID, Pay_EmpAnyRequest.N_ProcStatus, Pay_EmpAnyRequest.B_IsSaveDraft, Pay_EmpAnyRequest.N_UserID, Pay_EmpAnyRequest.X_FileName, Pay_EmpAnyRequest.B_IsAttach, Pay_EmpAnyRequest.X_Comments, Pay_EmpAnyRequest.N_RequestStatus, Pay_EmpAnyRequest.N_EntryUserID, Pay_EmpAnyRequest.X_RequestCode, Pay_Employee.X_EmpName, Pay_Employee.X_EmpCode, Pay_EmployeeRequestType.X_RequestTypeDesc FROM         Pay_EmpAnyRequest INNER JOIN Pay_EmployeeRequestType ON Pay_EmpAnyRequest.N_CompanyID = Pay_EmployeeRequestType.N_CompanyID AND  Pay_EmpAnyRequest.N_RequestType = Pay_EmployeeRequestType.N_RequestTypeID LEFT OUTER JOIN Pay_Employee ON Pay_EmpAnyRequest.N_CompanyID = Pay_Employee.N_CompanyID AND Pay_EmpAnyRequest.N_EmpID = Pay_Employee.N_EmpID  where Pay_EmpAnyRequest.X_RequestCode=@xRequestCode and Pay_EmpAnyRequest.N_CompanyID=@nCompanyID";

                    dt = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);



                    dt = api.Format(dt, "master");

                    DataTable Attachements = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(dt.Rows[0]["N_EmpID"].ToString()), myFunctions.getIntVAL(dt.Rows[0]["N_RequestID"].ToString()), this.FormID, myFunctions.getIntVAL(dt.Rows[0]["N_FnYearID"].ToString()), User, connection);
                    Attachements = api.Format(Attachements, "attachments");
                    Result.Tables.Add(Attachements);
                    Result.Tables.Add(dt);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(Result));
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
                DataRow MasterRow = MasterTable.Rows[0];
                DataTable Approvals;
                Approvals = ds.Tables["approval"];
                DataRow ApprovalRow = Approvals.Rows[0];
                DataTable Attachment = ds.Tables["attachments"];

                int nRequestID = myFunctions.getIntVAL(MasterRow["n_RequestID"].ToString());
                int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                string xReqCode = MasterRow["x_RequestCode"].ToString();
                int nEmpID = myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                int N_NextApproverID = 0;



                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList EmpParams = new SortedList();
                    EmpParams.Add("@nCompanyID", nCompanyID);
                    EmpParams.Add("@nEmpID", nEmpID);
                    EmpParams.Add("@nFnYearID", nFnYearID);
                    object objEmpName = dLayer.ExecuteScalar("Select X_EmpName From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", EmpParams, connection, transaction);
                    object objEmpCode = dLayer.ExecuteScalar("Select X_EmpCode From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", EmpParams, connection, transaction);

                    if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()))
                    {
                        int N_PkeyID = nRequestID;
                        string X_Criteria = "N_RequestID=" + N_PkeyID + " and N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                        myFunctions.UpdateApproverEntry(Approvals, "Pay_EmpAnyRequest", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "Employee Request", N_PkeyID, xReqCode, 1, objEmpName.ToString(), 0, "", User, dLayer, connection, transaction);
                        myAttachments.SaveAttachment(dLayer, Attachment, xReqCode, nRequestID, objEmpName.ToString(), objEmpCode.ToString(), nEmpID, "Employee", User, connection, transaction);
                        transaction.Commit();
                        myFunctions.SendApprovalMail(N_NextApproverID, FormID, nRequestID, "Employee Request", xReqCode, dLayer, connection, transaction, User);
                        return Ok(api.Success("Employee Request Approved" + "-" + xReqCode));
                    }

                    if (xReqCode == "@Auto")
                    {
                        SortedList Params = new SortedList();
                        Params.Add("@nCompanyID", nCompanyID);
                        xReqCode = dLayer.ExecuteScalar("Select max(isnull(N_RequestID,0))+1 as N_RequestID from Pay_EmpAnyRequest where N_CompanyID=@nCompanyID", Params, connection, transaction).ToString();
                        if (xReqCode == null || xReqCode == "") { xReqCode = "1"; }
                        MasterTable.Rows[0]["X_RequestCode"] = xReqCode;
                    }


                    if (nRequestID > 0)
                    {
                        dLayer.DeleteData("Pay_EmpAnyRequest", "N_RequestID", nRequestID, "", connection, transaction);
                    }

                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);

                    nRequestID = dLayer.SaveData("Pay_EmpAnyRequest", "N_RequestID", MasterTable, connection, transaction);
                    if (nRequestID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearID, "Employee Request", nRequestID, xReqCode, 1, objEmpName.ToString(), 0, "", User, dLayer, connection, transaction);

                        // DataTable Files = ds.Tables["files"];
                        // if (Files.Rows.Count > 0)
                        // {
                        //     if (!dLayer.SaveFiles(Files, "Pay_EmpAnyRequest", "N_RequestID", nRequestID, nEmpID.ToString(), nCompanyID, connection, transaction))
                        //     {
                        //         transaction.Rollback();
                        //         return Ok(api.Error("Unable to save"));
                        //     }
                        // }


                        myAttachments.SaveAttachment(dLayer, Attachment, xReqCode, nRequestID, objEmpName.ToString(), objEmpCode.ToString(), nEmpID, "Employee", User, connection, transaction);
                        transaction.Commit();
                        myFunctions.SendApprovalMail(N_NextApproverID, FormID, nRequestID, "Employee Request", xReqCode, dLayer, connection, transaction, User);
                        Dictionary<string, string> res = new Dictionary<string, string>();
                        res.Add("x_RequestCode", xReqCode.ToString());
                        return Ok(api.Success(res, "Employee Request successfully created with Request No" + "-" + xReqCode));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }




        [HttpPost("saveRequestType")]
        public ActionResult SaveReqType([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                DataRow MasterRow = MasterTable.Rows[0];

                int nRequestID = myFunctions.getIntVAL(MasterRow["n_RequestTypeID"].ToString());



                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlTransaction transaction = connection.BeginTransaction();

                    nRequestID = dLayer.SaveData("Pay_EmployeeRequestType", "N_RequestTypeID", MasterTable, connection, transaction);
                    if (nRequestID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Request Type Successfully Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nRequestID, int nFnYearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nTransID", nRequestID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    ParamList.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,isNull(N_EmpID,0) as N_EmpID,X_RequestCode,N_RequestType from Pay_EmpAnyRequest where N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID and N_RequestID=@nTransID";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(api.Error("Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    int EmpID = myFunctions.getIntVAL(TransRow["N_EmpID"].ToString());

                    SortedList EmpParams = new SortedList();
                    EmpParams.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    EmpParams.Add("@nEmpID", EmpID);
                    EmpParams.Add("@nFnYearID", nFnYearID);
                    object objEmpName = dLayer.ExecuteScalar("Select X_EmpName From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", EmpParams, connection);

                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, this.FormID, nRequestID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID, myFunctions.getIntVAL(TransRow["N_EmpID"].ToString()), myFunctions.getIntVAL(TransRow["N_RequestType"].ToString()), User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), "");
                    SqlTransaction transaction = connection.BeginTransaction(); ;

                    string X_Criteria = "N_RequestID=" + nRequestID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnYearID;

                    string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());
                    //myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString())

                    string status = myFunctions.UpdateApprovals(Approvals, nFnYearID, "Employee Request", nRequestID, TransRow["X_RequestCode"].ToString(), ProcStatus, "Pay_EmpAnyRequest", X_Criteria, objEmpName.ToString(), User, dLayer, connection, transaction);
                    if (status != "Error")
                    {
                        transaction.Commit();
                        return Ok(api.Success("Employee Request " + status + " Successfully"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to delete Employee Request"));
                    }


                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }



                // private void SaveDocs(DataTable Attachment, string EmpCode, string EmpName, int nEmpID, string xRequestCode, int nRequestID,ClaimsPrincipal user, SqlConnection connection, SqlTransaction transaction)
                //     {
                //         if (Attachment.Rows.Count > 0)
                //         {
                //             string X_DMSMainFolder = "Employee";
                //             string X_DMSSubFolder = this.FormID + "//" + EmpCode + "-" + EmpName;
                //             string X_folderName = X_DMSMainFolder + "//" + X_DMSSubFolder;
                //             try
                //             {
                //                 myAttachments.SaveAttachment(dLayer, Attachment, xRequestCode, nRequestID, EmpName, EmpCode, nEmpID, X_folderName, user, connection, transaction);
                //             }
                //             catch (Exception ex)
                //             {
                //                 transaction.Rollback();
                //             }
                //         }
                //     }




        [HttpDelete("deleteReqType")]
        public ActionResult DeleteReqType(int nRequestTypeID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nRequestTypeID", nRequestTypeID);
                    ParamList.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    string Sql = "select count(1) from Pay_EmpAnyRequest where N_CompanyId=@nCompanyID and N_RequestType=@nRequestTypeID";
                    object reqUsed = dLayer.ExecuteScalar(Sql, ParamList, connection);
                    if (myFunctions.getIntVAL(reqUsed.ToString()) > 0)
                    {
                        return Ok(api.Error("Request Type In Use"));
                    }
                    int res = dLayer.DeleteData("Pay_EmployeeRequestType", "N_RequestTypeID", nRequestTypeID, "N_CompanyId=" + myFunctions.GetCompanyID(User) + " and N_RequestTypeID=" + nRequestTypeID, connection);

                    if (res > 0)
                        return Ok(api.Success("Request Type Deleted"));
                    else
                        return Ok(api.Error("Unable to delete Request Type"));



                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }
    }
}