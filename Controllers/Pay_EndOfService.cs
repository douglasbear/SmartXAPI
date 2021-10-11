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
    [Route("endofservice")]
    [ApiController]
    public class Pay_EndOfService : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 455;

        public Pay_EndOfService(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpGet("list")]
        public ActionResult GetEndOfServiceList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_ServiceEndCode like '%" + xSearchkey + "%' or X_EmpCode like '%" + xSearchkey + "%' or X_EmpName like '%" + xSearchkey + "%' or X_EndType like '%" + xSearchkey + "%' or cast([D_EndDate] as VarChar) like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ServiceEndID desc";
            else
            {
                   switch (xSortBy.Split(" ")[0])
                        {
                             case "x_ServiceEndCode":
                                xSortBy = "N_ServiceEndID " + xSortBy.Split(" ")[1];
                                break;
                            case "d_EndDate":
                                xSortBy = "Cast(D_EndDate as DateTime ) " + xSortBy.Split(" ")[1];
                                break;
                           
                            default: break;
                        }
            
                xSortBy = " order by " + xSortBy;
        }
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") X_ServiceEndCode,X_EmpCode,X_EmpName,D_EndDate,X_EndType from vw_EndOfService Where N_CompanyID=@nCompanyID " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") X_ServiceEndCode,X_EmpCode,X_EmpName,D_EndDate,X_EndType from vw_EndOfService Where N_CompanyID=@nCompanyID " + Searchkey + " and N_ServiceEndID not in (select top(" + Count + ") N_ServiceEndID from vw_EndOfService where N_CompanyID=@nCompanyID" + xSearchkey + xSortBy + " ) " + xSortBy;
            Params.Add("@nCompanyID", nCompanyID);

            SortedList OutPut = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    string sqlCommandCount = "select count(*) as N_Count  from vw_EndOfService where N_CompanyID=@nCompanyID " + Searchkey + "";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(OutPut));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
        [HttpGet("listemployee")]
        public ActionResult ListEmployee(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            string sqlCommandText = "Select N_EmpID,X_EmpCode,X_EmpName from vw_PayEmployee Where N_CompanyID=@nCompanyID and N_Status<2 and N_FnyearID=@nFnYearID";
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
        [HttpGet("listEndType")]
        public ActionResult ListEndType()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "Select N_EndTypeID,X_EndType from Pay_ServiceEndType";
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
        [HttpGet("listReason")]
        public ActionResult ListReason()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "Select N_PkeyId,X_PkeyCode,X_Name from Gen_LookupTable where N_ReferId = 455 order by N_ReferId asc";
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

        [HttpGet("details")]
        public ActionResult EndofServiceDetails(string xServiceEndCode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "Select * from vw_EndOfService where X_ServiceEndCode=@p3 and N_CompanyID=@p1";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p3", xServiceEndCode);

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
                return Ok(api.Error(User,e));
            }
        }


        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                DataTable PayMasterTable;
                DataTable PayDetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                PayMasterTable = ds.Tables["payMaster"];
                PayDetailTable = ds.Tables["payDetails"];
                int N_SaveDraft = 0;
                int N_Status = 0;
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nServiceEndID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ServiceEndID"].ToString());
                int nEmpID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_EmpID"].ToString());
                var dEndDate = MasterTable.Rows[0]["D_EndDate"].ToString();
                DateTime dDateEnd = Convert.ToDateTime(MasterTable.Rows[0]["D_EndDate"].ToString());
                string xMethod = MasterTable.Rows[0]["X_Method"].ToString();
                int nSalaryPayMethod = myFunctions.getIntVAL(MasterTable.Rows[0]["N_SalaryPayMethod"].ToString());
                double nPayRate = myFunctions.getVAL(MasterTable.Rows[0]["N_PayRate"].ToString());
                int nSalTransID = myFunctions.getIntVAL(PayMasterTable.Rows[0]["n_TransID"].ToString());
                int nEOSDetailID = 0;
                string PayrunID = dDateEnd.Year.ToString("00##") + dDateEnd.Month.ToString("0#");
                string X_SalBatch="";
                bool B_SalProcessed =false;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    SortedList QueryParams = new SortedList();

                    // Auto Gen
                    string ServiceEndCode = "";
                    var Salvalue = PayMasterTable.Rows[0]["X_Batch"].ToString();
                    var values = MasterTable.Rows[0]["X_ServiceEndCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("N_ServiceEndID", nServiceEndID);
                        ServiceEndCode = dLayer.GetAutoNumber("pay_EndOFService", "X_ServiceEndCode", Params, connection, transaction);
                        if (ServiceEndCode == "") { transaction.Rollback(); return Ok(api.Error(User,"Unable to generate Service End Code")); }
                        MasterTable.Rows[0]["X_ServiceEndCode"] = ServiceEndCode;
                    }
                    
                    object _salProcessed = dLayer.ExecuteScalar("SELECT COUNT(*) FROM Pay_PaymentDetails INNER JOIN Pay_PaymentMaster ON Pay_PaymentDetails.N_TransID = Pay_PaymentMaster.N_TransID AND Pay_PaymentDetails.N_CompanyID = Pay_PaymentMaster.N_CompanyID where N_EmpID = " + myFunctions.getIntVAL(MasterTable.Rows[0]["N_EmpID"].ToString()) + " and Pay_PaymentMaster.N_FormID = 190 and isnull(Pay_PaymentMaster.N_RefBatchID,0) = 0 and N_PayRunID = " + PayrunID + " group by  Pay_PaymentMaster.N_TransID,N_PayRunID,X_Batch,N_EmpID", QueryParams, connection, transaction);
                    if (_salProcessed != null)
                    {
                        if (myFunctions.getIntVAL(_salProcessed.ToString()) > 0)
                            B_SalProcessed = true;
                    }

                    if (Salvalue == "" && !B_SalProcessed)
                    {
                        bool OK = true;
                        int NewNo = 0, loop = 1;
                        while (OK)
                        {
                            NewNo = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(Count(*),0) + " + loop + " As Count FRom Pay_PaymentMaster Where N_CompanyID=" + nCompanyID + " And N_FnyearID = " + nFnYearId + " And N_PayRunID = " + PayrunID, QueryParams, connection, transaction).ToString());
                            X_SalBatch = dDateEnd.Year.ToString("00##") + dDateEnd.Month.ToString("0#") + NewNo.ToString("0#");
                            if (myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(Count(*),0) FRom Pay_PaymentMaster Where N_CompanyID=" + nCompanyID + " And N_FnyearID = " + nFnYearId + " And X_Batch = '" + X_SalBatch + "'", QueryParams, connection, transaction).ToString()) == 0)
                            {
                                OK = false;
                            }
                            loop += 1;
                        }
                    }

                    MasterTable.Columns.Remove("X_Method");
                    MasterTable.Columns.Remove("N_SalaryPayMethod");
                    MasterTable.Columns.Remove("N_PayRate");

                    if(nServiceEndID>0)
                    {
                        dLayer.DeleteData("Pay_PaymentDetails", "N_TransID", nServiceEndID, "N_CompanyID=" + nCompanyID + " and N_FormID=" + this.N_FormID, connection, transaction);
                        dLayer.DeleteData("Pay_PaymentDetails", "N_TransID", nSalTransID, "N_CompanyID=" + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Pay_PaymentMaster", "N_TransID", nSalTransID, "N_CompanyID=" + nCompanyID, connection, transaction);
                    }

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and X_ServiceEndCode='" + ServiceEndCode + "' and N_FnyearID=" + nFnYearId;
                    nServiceEndID = dLayer.SaveData("pay_EndOFService", "N_ServiceEndID", DupCriteria, "", MasterTable, connection, transaction);
                    if (nServiceEndID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    QueryParams.Add("@nCompanyID", nCompanyID);
                    QueryParams.Add("@nFnYearID", nFnYearId);
                    QueryParams.Add("@N_ServiceEndID", nServiceEndID);
                    QueryParams.Add("@N_EmpID", nEmpID);
                    QueryParams.Add("@X_Method", xMethod);
                    object Savedraft = dLayer.ExecuteScalar("select CAST(B_IsSaveDraft as INT) from pay_EndOFService where N_CompanyID=@nCompanyID and N_ServiceEndID=@N_ServiceEndID", QueryParams, connection, transaction);
                    if (Savedraft != null)
                        N_SaveDraft = myFunctions.getIntVAL(Savedraft.ToString());
                    object Status = "3";// dLayer.ExecuteScalar("select N_Status  from Pay_EmployeeStatus where X_Description=@X_Method", QueryParams, connection, transaction);
                    if (Status != null)
                        N_Status = myFunctions.getIntVAL(Status.ToString());

                    object PositionID = dLayer.ExecuteScalar("select N_PositionID from vw_PayEmployee where N_CompanyID=@nCompanyID and N_EMPID=@N_EmpID", QueryParams, connection, transaction);

                    if (N_SaveDraft == 0)
                    {
                        dLayer.ExecuteNonQuery("Update Pay_Employee Set N_Status = " + N_Status + ",D_StatusDate='" + dEndDate.ToString() + "' Where N_CompanyID =" + nCompanyID + " And N_EmpID =" + nEmpID.ToString(), QueryParams, connection, transaction);
                        dLayer.ExecuteNonQuery("Update Pay_SuperVisor Set N_EmpID = 0  Where N_CompanyID =" + nCompanyID + " And N_PositionID =" + PositionID.ToString(), QueryParams, connection, transaction);
                    }
                    dLayer.DeleteData("pay_EndOfServiceSDetails", "N_ServiceEndID", nServiceEndID, "", connection, transaction);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                         DetailTable.Rows[j]["N_ServiceEndID"] = nServiceEndID;
                    }
                    nEOSDetailID = dLayer.SaveData("pay_EndOfServiceSDetails", "N_EOSDetailID", DetailTable, connection, transaction);
       
                    if(!B_SalProcessed)
                    {
                        string payruntxt = dDateEnd.ToString("MMM'-'yyyy");

                        DateTime firstDayOfMonth = new DateTime(dDateEnd.Year, dDateEnd.Month, 1);
                        DateTime lastDayOfMonth = new DateTime(dDateEnd.Year, dDateEnd.Month, DateTime.DaysInMonth(dDateEnd.Year, dDateEnd.Month));

                        PayMasterTable.Rows[0]["N_RefID"] = nServiceEndID;
                        PayMasterTable.Rows[0]["N_PayrunID"] = PayrunID;
                        PayMasterTable.Rows[0]["X_Batch"] = X_SalBatch;
                        PayMasterTable.Rows[0]["X_PayrunText"] = payruntxt;
                        PayMasterTable.Rows[0]["D_SalFromDate"] = firstDayOfMonth;
                        PayMasterTable.Rows[0]["D_SalToDate"] = lastDayOfMonth;

                        int nTransID = dLayer.SaveData("Pay_PaymentMaster", "N_TransID", "", "", PayMasterTable, connection, transaction);
                        if (nTransID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User,"Unable to save"));
                        }
                        for (int j = 0; j < PayDetailTable.Rows.Count; j++)
                        {
                            PayDetailTable.Rows[j]["N_TransID"] = nTransID;
                            // if(myFunctions.getIntVAL(PayDetailTable.Rows[j]["IsAccrued"].ToString())==1)
                            //     PayDetailTable.Rows[j]["N_PayID"]=MasterTable.Rows[0]["N_RefPayID"];

                            // if(myFunctions.getIntVAL(PayDetailTable.Rows[j]["N_PayID"].ToString())==0)
                            //     PayDetailTable.Rows[j].Delete();
                        }
                        PayDetailTable.Columns.Remove("IsAccrued");
                        PayDetailTable.AcceptChanges();

                        int nTransDetailsID=0;
                        nTransDetailsID = dLayer.SaveData("Pay_PaymentDetails", "N_TransDetailsID", "", "", PayDetailTable, connection, transaction);
                        if (nTransDetailsID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User,"Unable to save"));
                        }

                        SortedList SPVoucheDelParams = new SortedList();
                        SPVoucheDelParams.Add("N_CompanyID", nCompanyID);
                        SPVoucheDelParams.Add("N_FnYearID", nFnYearId);
                        SPVoucheDelParams.Add("X_TransType", "ESI");
                        SPVoucheDelParams.Add("X_ReferenceNo", PayMasterTable.Rows[0]["X_Batch"]);
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Pay_SalryProcessingVoucher_Del", SPVoucheDelParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User,ex));
                        }

                        SortedList SPYrPayParams = new SortedList();
                        SPYrPayParams.Add("N_CompanyID", nCompanyID);
                        SPYrPayParams.Add("TransId", nTransID);
                        SPYrPayParams.Add("N_Month", dDateEnd.Month.ToString("0#"));
                        SPYrPayParams.Add("N_Year", dDateEnd.Year.ToString("00##"));
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Pay_YearlyPay", SPYrPayParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User,ex));
                        }

                        SortedList SPPayProcessingParams = new SortedList();
                        SPPayProcessingParams.Add("N_CompanyID", nCompanyID);
                        SPPayProcessingParams.Add("N_TransID", nTransID);
                        SPPayProcessingParams.Add("D_Date",myFunctions.getDateVAL(dDateEnd));
                        SPPayProcessingParams.Add("D_EntryDate", myFunctions.getDateVAL(Convert.ToDateTime(MasterTable.Rows[0]["D_EntryDate"].ToString())));
                        SPPayProcessingParams.Add("N_UserID", myFunctions.GetUserID(User));
                        SPPayProcessingParams.Add("X_SystemName", System.Environment.MachineName);
                        SPPayProcessingParams.Add("X_EntryFrom", "Salary Processing");
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Pay_PayrollProcessing", SPPayProcessingParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User,ex));
                        }

                        SortedList SPPayAccrualParams = new SortedList();
                        SPPayAccrualParams.Add("N_CompanyID", nCompanyID);
                        SPPayAccrualParams.Add("N_Month", dDateEnd.Month.ToString("0#"));
                        SPPayAccrualParams.Add("N_Year", dDateEnd.Year.ToString("00##"));
                        SPPayAccrualParams.Add("N_ProcessID", nTransID);
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Pay_AccrualProcess", SPPayAccrualParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User,ex));
                        }
                    }

                    if(nPayRate>0)
                    {
                        //EOS Post
                        DataTable dtPayDetails = new DataTable();
                        dtPayDetails.Clear();
                        dtPayDetails.Columns.Add("N_CompanyID");
                        dtPayDetails.Columns.Add("N_TransID");
                        dtPayDetails.Columns.Add("N_EmpID");
                        dtPayDetails.Columns.Add("N_PayID");
                        dtPayDetails.Columns.Add("N_PayFactor");
                        dtPayDetails.Columns.Add("N_PayRate");
                        dtPayDetails.Columns.Add("N_SalaryPayMethod");
                        dtPayDetails.Columns.Add("B_BeginingBalEntry");
                        dtPayDetails.Columns.Add("N_FormID");
                        dtPayDetails.Columns.Add("N_TransDetailsID");

                        int PayID = 0;
                        object  PID =dLayer.ExecuteScalar("select ISNULL(N_PayID,0) from Pay_PayMaster where N_CompanyID="+nCompanyID+" and N_FnYearID="+nFnYearId+" and N_PayTypeID=11", QueryParams, connection, transaction);
                        if (PID != null)
                        {
                            PayID = myFunctions.getIntVAL(PID.ToString());
                        }

                        DataRow row = dtPayDetails.NewRow();
                        row["N_CompanyID"] = nCompanyID;
                        row["N_TransID"] = nServiceEndID;
                        row["N_EmpID"] = nEmpID;
                        row["N_PayID"] = PayID;
                        row["N_PayFactor"] = 0;
                        row["N_PayRate"] = nPayRate;
                        row["N_SalaryPayMethod"] = nSalaryPayMethod;
                        row["B_BeginingBalEntry"] = 0;
                        row["N_FormID"] = this.N_FormID;
                        dtPayDetails.Rows.Add(row);

                        int nTransDetailID = dLayer.SaveData("Pay_PaymentDetails", "N_TransDetailsID", "", "", dtPayDetails, connection, transaction);
                    }

                    transaction.Commit();
                    return Ok(api.Success("Terminated"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nServiceEndID, int nEmpID,int nSalTransID,int nFnYearID,String xSalBatch)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            if (xSalBatch == null) xSalBatch = "";
            try
            {
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nEmpID", nEmpID);
                QueryParams.Add("@nServiceEndID", nServiceEndID);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    dLayer.ExecuteNonQuery("Update Pay_LoanIssueDetails Set n_RefundAmount =0  Where N_CompanyID = @nCompanyID and N_PayrunID = 0", QueryParams, connection, transaction);
                    dLayer.ExecuteNonQuery("Update Pay_Employee Set N_Status = 0,D_StatusDate = null Where N_CompanyID =@nCompanyID And N_EmpID =@nEmpID", QueryParams, connection, transaction);
                    Results = dLayer.DeleteData("pay_EndOfServiceSDetails", "N_ServiceEndID", nServiceEndID, "", connection, transaction);
                    Results = dLayer.DeleteData("pay_EndOFService", "N_ServiceEndID", nServiceEndID, "", connection, transaction);

                    
                    SortedList SPDeleteParams = new SortedList();
                    SPDeleteParams.Add("N_CompanyID", nCompanyID);
                    SPDeleteParams.Add("N_FnYearID", nFnYearID);
                    SPDeleteParams.Add("X_TransType","ESI");
                    SPDeleteParams.Add("X_ReferenceNo", xSalBatch);

                    try
                    {
                        dLayer.ExecuteNonQueryPro("SP_Pay_SalryProcessingVoucher_Del", SPDeleteParams, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,ex));
                    }


                    dLayer.DeleteData("Pay_PaymentDetails", "N_TransID", nServiceEndID, "N_CompanyID=" + nCompanyID + " and N_FormID=" + this.N_FormID, connection, transaction);
                    dLayer.DeleteData("Pay_PaymentDetails", "N_TransID", nSalTransID, "N_CompanyID=" + nCompanyID, connection, transaction);
                    dLayer.DeleteData("Pay_PaymentMaster", "N_TransID", nSalTransID, "N_CompanyID=" + nCompanyID, connection, transaction);

                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("N_ServiceEndID", nServiceEndID.ToString());
                    return Ok(api.Success(res, "EOS deleted"));
                }
                else
                {
                    return Ok(api.Error(User,"Unable to EoS"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }


        }

        [HttpGet("listEmployeeDetails")]
        public ActionResult ListEmployeeDetails(int nFnYearID, bool bAllBranchData, int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string X_Crieteria = "";
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@bAllBranchData", bAllBranchData);
            Params.Add("@nBranchID", nBranchID);
            if (bAllBranchData == true)
                X_Crieteria = "N_CompanyID=@nCompanyID and N_Status<2 and N_FnyearID =@nFnYearID";
            else
                X_Crieteria = "N_CompanyID=@nCompanyID and N_Status<2 and N_FnyearID =@nFnYearID and N_BranchID=@nBranchID";

            string sqlCommandText = "select X_EmpCode,X_EmpName,N_CompanyID,N_EmpID,X_Position,X_Department,D_HireDate,N_Status,N_FnyearID,N_BranchID,N_SalaryPayMethod from vw_PayEmployee where " + X_Crieteria;
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

        [HttpGet("listDescription")]
        public ActionResult ListDescription(int nCountryID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nCountryID", nCountryID);
            string sqlCommandText = "select N_ServiceEndID,X_ServiceEndCode,N_ServiceEndStatusID,X_ServiceEndStatusDesc,ServiceEndStatus from vw_ServiceEndSettings where N_CompanyID=@nCompanyID group by N_ServiceEndID,X_ServiceEndCode,N_ServiceEndStatusID,X_ServiceEndStatusDesc,ServiceEndStatus order by X_ServiceEndCode";
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

        [HttpGet("paymentDetails")]
        public ActionResult GetPaymentDetails(int nEmpID,int nFnYearID,DateTime dDate,int nPayID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nEmpID", nEmpID);
            //string sqlCommandText = "select X_Description,N_Payrate,N_Type,IsEOF from vw_Pay_PendingAmtsForTermination where N_CompanyID=@nCompanyID and N_EmpID=@nEmpID";

          

            SortedList OutPut = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SortedList proParams2 = new SortedList(){
                                    {"N_CompanyID",nCompanyID},
                                    {"N_EmpID",nEmpID},
                                    {"N_FnYearID",nFnYearID},
                                    {"D_Date",dDate},
                                    {"N_PayID",nPayID}};

                    dt=dLayer.ExecuteDataTablePro("SP_Pay_PendingAmtsForTermination", proParams2, connection);

                    // dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    // string sqlCommandCount = "select count(*) as N_Count from vw_Pay_PendingAmtsForTermination where N_CompanyID=@nCompanyID and N_EmpID=@nEmpID";
                    //DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    // string TotalCount = "0";

                    // if (Summary.Rows.Count > 0)
                    // {
                    //     DataRow drow = Summary.Rows[0];
                    //     TotalCount = drow["N_Count"].ToString();

                    // }
                    OutPut.Add("Details", api.Format(dt));
                    //OutPut.Add("TotalCount", TotalCount);
                }
                dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(OutPut));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }




        [HttpGet("employeeSalaryDetails")]
        public ActionResult EmployeeSalary(DateTime dtpEndDate, int nFnYearID, int nEmpID, int n_ServiceEndSettingsID, string byEmp,DateTime dtpHireDate)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    //DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    string X_Crieteria = "";
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nFnYearID", nFnYearID);

                    string sqlCommandText = "SELECT * FROM Pay_EmployeePayHistory where N_EmpID=" + nEmpID + " and D_EffectiveDate = (select MAX(D_EffectiveDate) from Pay_EmployeePayHistory where N_EmpID = " + nEmpID + " and N_PayID=1) and N_PayID=1";

                    DataTable EmployeeTable = new DataTable();
                    DataTable RelationTable = new DataTable();

                    EmployeeTable = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    if (EmployeeTable.Rows.Count == 0) { return Ok(api.Warning("No data found")); }

                    EmployeeTable = myFunctions.AddNewColumnToDataTable(EmployeeTable, "N_AdjustAmount", typeof(double), 0);
                    EmployeeTable = myFunctions.AddNewColumnToDataTable(EmployeeTable, "X_ServiceEnd", typeof(string), "");
                    EmployeeTable = myFunctions.AddNewColumnToDataTable(EmployeeTable, "N_GrossAmt", typeof(double), 0);
                    EmployeeTable = myFunctions.AddNewColumnToDataTable(EmployeeTable, "N_ServiceInDays", typeof(double), 0);
                    EmployeeTable = myFunctions.AddNewColumnToDataTable(EmployeeTable, "N_ServiceInYears", typeof(double), 0);

                    if (n_ServiceEndSettingsID == 0)
                    {
                        n_ServiceEndSettingsID = 1;
                    }
                    object obj = dLayer.ExecuteScalar("Select dbo.[SP_GetEOSAmount_Settings](" + myFunctions.GetCompanyID(User) + "," + nEmpID + "," + dtpEndDate.Year.ToString("00##") + dtpEndDate.Month.ToString("0#") + "," + nFnYearID + ",0," + n_ServiceEndSettingsID + ")", Params, connection);
                    if (obj != null)
                    {
                        EmployeeTable.Rows[0]["N_AdjustAmount"] = myFunctions.getVAL(obj.ToString());
                    }
                    if (byEmp == "emp")
                    {
                        object obj1 = dLayer.ExecuteScalar("select X_ServiceEndStatusDesc from vw_ServiceEndSettings where N_ServiceEndID=1", Params, connection);
                        if (obj1 != null)
                        {
                            EmployeeTable.Rows[0]["X_ServiceEnd"] = obj1.ToString();
                        }
                    }
                    else
                    {
                        EmployeeTable.Rows[0]["X_ServiceEnd"] = "choosen";
                    }

                    object objGross = dLayer.ExecuteScalar("SELECT SUM(Pay_EmployeePayHistory.N_Amount) AS N_GrossAmt FROM Pay_EmployeePayHistory INNER JOIN Pay_PayMaster ON Pay_EmployeePayHistory.N_CompanyID = Pay_PayMaster.N_CompanyID AND Pay_EmployeePayHistory.N_PayID = Pay_PayMaster.N_PayID where Pay_EmployeePayHistory.N_CompanyID="+nCompanyID+" and Pay_EmployeePayHistory.N_EmpID=" + nEmpID + " and Pay_EmployeePayHistory.D_EffectiveDate = (select MAX(D_EffectiveDate) from Pay_EmployeePayHistory where N_EmpID = " + nEmpID + " and N_CompanyID="+nCompanyID+" ) and Pay_PayMaster.N_PaymentID=5 and (Pay_PayMaster.N_Paymethod=0 or Pay_PayMaster.N_Paymethod=3 or Pay_PayMaster.N_PayMethod=4) and Pay_PayMaster.B_InActive=0", Params, connection);
                    if (objGross != null)
                    {
                        EmployeeTable.Rows[0]["N_GrossAmt"] = myFunctions.getVAL(objGross.ToString());
                    }

                    double N_ServiceInDays=0,N_ServiceInYears=0;
                    N_ServiceInDays=(dtpEndDate.Date - dtpHireDate.Date).TotalDays;
                    N_ServiceInYears=((dtpEndDate.Date - dtpHireDate.Date).Days / 365.25);

                    EmployeeTable.Rows[0]["N_ServiceInDays"] = myFunctions.getVAL(N_ServiceInDays.ToString());
                    EmployeeTable.Rows[0]["N_ServiceInYears"] = myFunctions.getVAL(N_ServiceInYears.ToString());

                    EmployeeTable.AcceptChanges();
                    EmployeeTable = api.Format(EmployeeTable, "EmpTable");


                    dt.Tables.Add(EmployeeTable);
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("listPaycode")]
        public ActionResult GetListPaycode(int nEmpID,int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nEmpID", nEmpID);
            string sqlCommandText = "select * FROM Pay_PayMaster WHERE N_PayID not in (select N_PayID from Pay_PaySetup where N_EmpID =@nEmpID) and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_PayMethod=3 and ISNULL(B_InActive,0)=0 and N_PayTypeID not in (11,14)";
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

    }
}