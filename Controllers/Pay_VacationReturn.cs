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
        private readonly int FormID;

        public Pay_VacationReturn(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1361;//463;
        }

        [HttpGet("vacationList")]
        public ActionResult GetVacationList(int nBranchID, bool bAllBranchData, int nEmpID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nBranchID", nBranchID);
            Params.Add("@nEmpID", nEmpID);
            string strBranch = (bAllBranchData == false) ? " and vw_PayVacationMaster_Disp.N_BranchID=@nBranchID " : "";
            string sqlCommandText = "SELECT    vw_PayVacationEmployee.[Employee Code] AS X_EmpCode, vw_PayVacationEmployee.Name AS X_Emp_Name, " +
            "vw_PayVacationEmployee.N_VacationGroupID, vw_PayVacationEmployee.X_VacationGroupCode, vw_PayVacationEmployee.X_VacType, " +
            "vw_PayVacationEmployee.N_EmpID, vw_PayVacationEmployee.N_Status " +
"FROM         vw_PayVacationEmployee RIGHT OUTER JOIN " +
                      "vw_PayVacationMaster_Disp ON vw_PayVacationEmployee.N_EmpID = vw_PayVacationMaster_Disp.N_EmpID AND " +
                      "vw_PayVacationEmployee.X_VacationGroupCode = vw_PayVacationMaster_Disp.X_VacationGroupCode AND " +
                      "vw_PayVacationEmployee.N_CompanyID = vw_PayVacationMaster_Disp.N_CompanyID AND " +
                      "vw_PayVacationEmployee.N_VacationGroupID = vw_PayVacationMaster_Disp.N_VacationGroupID " +
"WHERE     (vw_PayVacationEmployee.N_VacationStatus = 0) and (vw_PayVacationMaster_Disp.B_IsAdjustEntry = 0)  and vw_PayVacationMaster_Disp.N_CompanyID=@nCompanyID And vw_PayVacationMaster_Disp.N_Transtype =1 and vw_PayVacationMaster_Disp.N_EmpID=@nEmpID and vw_PayVacationMaster_Disp.B_IsSaveDraft=0 " + strBranch;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    sqlCommandText = "select X_EmpCode,X_Emp_Name,N_VacationGroupID,X_VacationGroupCode,X_VacType,N_EmpID,N_Status from (" + sqlCommandText + ") as dt group by X_EmpCode,X_Emp_Name,N_VacationGroupID,X_VacationGroupCode,X_VacType,N_EmpID,N_Status";
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "D_VacDateFrom", typeof(DateTime), null);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "D_VacDateTo", typeof(DateTime), null);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "D_ReturnDate", typeof(DateTime), null);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "X_VacDetails", typeof(String), "");
                    foreach (DataRow var in dt.Rows)
                    {
                        DataTable VacDate = dLayer.ExecuteDataTable("Select Min(D_VacDateFrom) As FromDate ,Max(D_VacDateTo) as ToDate from Pay_VacationDetails Where N_VacationGroupID =" + var["N_VacationGroupID"].ToString() + "", connection);
                        if (VacDate.Rows.Count > 0)
                        {
                            var["D_VacDateFrom"] = Convert.ToDateTime(VacDate.Rows[0]["FromDate"].ToString());
                            var["D_VacDateTo"] = Convert.ToDateTime(VacDate.Rows[0]["ToDate"].ToString());
                            var["X_VacDetails"] = var["X_VacType"].ToString() + " [" + Convert.ToDateTime(VacDate.Rows[0]["FromDate"].ToString()).ToString("dd-MMM-yyyy") + " To " + Convert.ToDateTime(VacDate.Rows[0]["ToDate"].ToString()).ToString("dd-MMM-yyyy") + "]  ";
                            var["D_ReturnDate"] = Convert.ToDateTime(VacDate.Rows[0]["ToDate"].ToString());
                        }
                    }
                    dt.AcceptChanges();
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
                DataTable Approvals;
                Approvals = ds.Tables["approval"];
                DataRow ApprovalRow = Approvals.Rows[0];

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
                    int nEmpID = myFunctions.getIntVAL(MasterRow["n_EmpID"].ToString());

                    int N_NextApproverID = 0;

                    QueryParams.Add("@nCompanyID", N_CompanyID);
                    QueryParams.Add("@nFnYearID", N_FnYearID);
                    QueryParams.Add("@nVacationReturnID", N_VacationReturnID);


                    // Auto Gen
                    string X_VacationReturnCode = MasterTable.Rows[0]["X_VacationReturnCode"].ToString();
                    DataRow Master = MasterTable.Rows[0];


                    SortedList EmpParams = new SortedList();
                    EmpParams.Add("@nCompanyID", N_CompanyID);
                    EmpParams.Add("@nEmpID", nEmpID);
                    EmpParams.Add("@nFnYearID", N_FnYearID);
                    object objEmpName = dLayer.ExecuteScalar("Select X_EmpName From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", EmpParams, connection, transaction);
                    object objEmpCode = dLayer.ExecuteScalar("Select X_EmpCode From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", EmpParams, connection, transaction);


                    if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()))
                    {
                        int N_PkeyID = N_VacationReturnID;
                        string X_Criteria = "N_VacationReturnID=" + N_PkeyID + " and N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID;
                        myFunctions.UpdateApproverEntry(Approvals, "Pay_VacationReturn", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, N_FnYearID, "VACATION RETURN", N_PkeyID, X_VacationReturnCode, 1, objEmpName.ToString(), 0, "", User, dLayer, connection, transaction);
                        transaction.Commit();
                        myFunctions.SendApprovalMail(N_NextApproverID, FormID, N_VacationReturnID, "VACATION RETURN", X_VacationReturnCode, dLayer, connection, transaction, User);
                        return Ok(_api.Success("Request Approved " + "-" + X_VacationReturnCode));
                    }

                    if (X_VacationReturnCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", Master["n_CompanyId"].ToString());
                        Params.Add("N_YearID", Master["n_FnYearId"].ToString());
                        Params.Add("N_FormID", 463);
                        X_VacationReturnCode = dLayer.GetAutoNumber("Pay_VacationReturn", "X_VacationReturnCode", Params, connection, transaction);
                        if (X_VacationReturnCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Quotation Number")); }
                        MasterTable.Rows[0]["X_VacationReturnCode"] = X_VacationReturnCode;

                    }

                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);

                    N_VacationReturnID = dLayer.SaveData("Pay_VacationReturn", "n_VacationReturnID", MasterTable, connection, transaction);
                    if (N_VacationReturnID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save Vacation Return"));
                    }

                    N_NextApproverID = myFunctions.LogApprovals(Approvals, N_FnYearID, "VACATION RETURN", N_VacationReturnID, X_VacationReturnCode, 1, objEmpName.ToString(), 0, "", User, dLayer, connection, transaction);

                    if (N_VacationReturnID > 0)
                        dLayer.ExecuteNonQuery("delete from Pay_VacationDetails where N_VoucherID=" + N_VacationReturnID.ToString() + "and N_FormID=463 and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearId=" + N_FnYearID, connection, transaction);
                    if (DetailTable.Rows.Count > 0)
                    {
                        DetailTable.Rows[0]["n_VoucherID"] = N_VacationReturnID.ToString();
                        DetailTable.Rows[0]["n_FormID"] = "463";
                        DetailTable.AcceptChanges();
                        int N_QuotationDetailId = dLayer.SaveData("Pay_VacationDetails", "N_VacationID", DetailTable, connection, transaction);
                        if (N_QuotationDetailId <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable to save Vacation Return"));
                        }
                    }
                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("N_VacationReturnID", N_VacationReturnID);
                    Result.Add("X_VacationReturnCode", X_VacationReturnCode);
                    return Ok(_api.Success(Result, "Vacation Return saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }



        [HttpGet("details")]
        public ActionResult GetDetails(int nCompanyID, string xVacationReturnCode, int nFnYearID, bool bAllBranchData, int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@xVacationReturnCode", xVacationReturnCode);

            string sqlCommandText = "";

            if (bAllBranchData == true)
            { sqlCommandText = "SELECT        vw_PayVacationReturn.*,vw_PayVacationReturn.ReturnId as N_VacationReturnID, vw_PayVacationDetails_Disp.VacTypeId as n_VacTypeID, vw_PayVacationDetails_Disp.N_VacDays as n_Delay,vw_PayVacationDetails_Disp.[Vacation Type] as x_VacTypeName FROM vw_PayVacationReturn LEFT OUTER JOIN vw_PayVacationDetails_Disp ON vw_PayVacationReturn.N_VacationGroupID = vw_PayVacationDetails_Disp.N_VacationGroupID AND vw_PayVacationReturn.N_FnYearID = vw_PayVacationDetails_Disp.N_FnYearID AND vw_PayVacationReturn.ReturnId = vw_PayVacationDetails_Disp.N_VoucherID Where  vw_PayVacationReturn.N_FnyearID=@nFnYearID and   vw_PayVacationReturn.N_CompanyID=@nCompanyID and vw_PayVacationReturn.x_VacationReturnCode=@xVacationReturnCode"; }
            else
            {
                sqlCommandText = "SELECT        vw_PayVacationReturn.*,vw_PayVacationReturn.ReturnId as N_VacationReturnID, vw_PayVacationDetails_Disp.VacTypeId as n_VacTypeID, vw_PayVacationDetails_Disp.N_VacDays as n_Delay,vw_PayVacationDetails_Disp.[Vacation Type] as x_VacTypeName FROM vw_PayVacationReturn LEFT OUTER JOIN vw_PayVacationDetails_Disp ON vw_PayVacationReturn.N_VacationGroupID = vw_PayVacationDetails_Disp.N_VacationGroupID AND vw_PayVacationReturn.N_FnYearID = vw_PayVacationDetails_Disp.N_FnYearID AND vw_PayVacationReturn.ReturnId = vw_PayVacationDetails_Disp.N_VoucherID Where  vw_PayVacationReturn.N_FnyearID=@nFnYearID and  vw_PayVacationReturn.N_CompanyID=@nCompanyID and vw_PayVacationReturn.N_BranchID=@nBranchID  and vw_PayVacationReturn.x_VacationReturnCode=@xVacationReturnCode";
                Params.Add("@nBranchID", nBranchID);
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
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


        [HttpGet("Dashboardlist")]
        public ActionResult PayVacationList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);

            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            Params.Add("@p1", nCompanyId);
            Params.Add("@nUserID", nUserID);
            if (xSearchkey != null && xSearchkey.Trim() != "")

                Searchkey = " and (X_VacationReturnCode like '%" + xSearchkey + "%'or cast(D_Date as VarChar) like'%" + xSearchkey + "%'or cast(D_ReturnDate as VarChar) like'%" + xSearchkey + "%'or x_VacType like'%" + xSearchkey + "%'or x_Remarks like'%" + xSearchkey + "%')";
            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by ReturnId desc";
            else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                           
                            case "d_Date":
                                xSortBy = "Cast(D_Date as DateTime )" + xSortBy.Split(" ")[1];
                                break;
                            case "d_ReturnDate":
                                xSortBy = "Cast(D_ReturnDate as DateTime )" + xSortBy.Split(" ")[1];
                                break;
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_PayVacationReturn where N_CompanyID=@p1 and N_EmpID=@nEmpID" + Searchkey + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_PayVacationReturn where N_EmpID=@nEmpID and N_CompanyID=" + nCompanyId + "  " + Searchkey + " and ReturnId not in (select top(" + Count + ") ReturnId from vw_PayVacationReturn where N_EmpID=@nEmpID and N_CompanyID=" + nCompanyId + "  " + xSortBy + " ) " + xSortBy;



            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    object nEmpID = dLayer.ExecuteScalar("Select N_EmpID From Sec_User where N_UserID=@nUserID and N_CompanyID=@p1", Params, connection);

                    if (nEmpID == null)
                        nEmpID = 0;

                    Params.Add("@nEmpID", nEmpID);


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_PayVacationReturn where N_CompanyID=@p1 and  N_EmpID=@nEmpID " + Searchkey;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nVacationReturnID, int nFnYearID, int nCompanyID, int nVacTypeID, string comments)
        {
            if (comments == null)
            {
                comments = "";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nVacationReturnID", nVacationReturnID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    ParamList.Add("@nCompanyID", myFunctions.GetCompanyID(User));

                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,isNull(N_EmpID,0) as N_EmpID,X_VacationReturnCode,N_VacationGroupID from Pay_VacationReturn where N_CompanyId=@nCompanyID and N_FnYearID=@nFnYearID and N_VacationReturnID=@nVacationReturnID";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error("Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    int EmpID = myFunctions.getIntVAL(TransRow["N_EmpID"].ToString());

                    SortedList EmpParams = new SortedList();
                    EmpParams.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    EmpParams.Add("@nEmpID", EmpID);
                    EmpParams.Add("@nFnYearID", nFnYearID);
                    object objEmpName = dLayer.ExecuteScalar("Select X_EmpName From Pay_Employee where N_EmpID=@nEmpID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", EmpParams, connection);

                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, this.FormID, nVacationReturnID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID, myFunctions.getIntVAL(TransRow["N_EmpID"].ToString()), nVacTypeID, User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), comments);

                    SqlTransaction transaction = connection.BeginTransaction();

                    string X_Criteria = "N_VacationReturnID=" + nVacationReturnID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnYearID;
                    string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());
                    string status = myFunctions.UpdateApprovals(Approvals, nFnYearID, "VACATION RETURN", nVacationReturnID, TransRow["X_VacationReturnCode"].ToString(), ProcStatus, "Pay_VacationReturn", X_Criteria, objEmpName.ToString(), User, dLayer, connection, transaction);
                    if (status != "Error")
                    {
                        if (ButtonTag == "6" || ButtonTag == "0")
                        {
                            if (nVacTypeID != 0)
                                dLayer.DeleteData("Pay_VacationDetails", "N_VacationGroupID", myFunctions.getIntVAL(TransRow["N_VacationGroupID"].ToString()), "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_VoucherID=" + nVacationReturnID, connection, transaction);

                            dLayer.DeleteData("Pay_VacationReturn", "N_VacationReturnID", nVacationReturnID, "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID, connection, transaction);
                        }

                        transaction.Commit();
                        return Ok(_api.Success("Vacation Return " + status + " Successfully"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to delete Vacation Return"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }

        }
    }
}