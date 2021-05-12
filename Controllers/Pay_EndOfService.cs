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
            int nCompanyID=myFunctions.GetCompanyID(User);
            int Count = (nPage - 1) * nSizeperpage;
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_ServiceEndCode like '%" + xSearchkey + "%' or X_EmpCode like '%" + xSearchkey + "%' or X_EmpName like '%" + xSearchkey + "%' or X_EndType like '%" + xSearchkey + "%' or cast([D_EndDate] as VarChar) like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ServiceEndID desc";
            else
                xSortBy = " order by " + xSortBy;
            Params.Add("@nCompanyID",nCompanyID);
            string sqlCommandText="Select X_ServiceEndCode,X_EmpCode,X_EmpName,D_EndDate,X_EndType from vw_EndOfService Where N_CompanyID=@nCompanyID";
            SortedList OutPut = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);

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
                return Ok(api.Error(e));
            }
        }
        [HttpGet("listemployee")]
        public ActionResult ListEmployee(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nFnYearID",nFnYearID);
            string sqlCommandText="Select N_EmpID,X_EmpCode,X_EmpName from vw_PayEmployee Where N_CompanyID=@nCompanyID and N_Status<2 and N_FnyearID=@nFnYearID";
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
                return Ok(api.Error(e));
            }
        }
        [HttpGet("listEndType")]
        public ActionResult ListEndType()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            string sqlCommandText="Select N_EndTypeID,X_EndType from Pay_ServiceEndType";
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
                return Ok(api.Error(e));
            }
        }
        [HttpGet("listReason")]
        public ActionResult ListReason()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText="Select N_PkeyId,X_PkeyCode,X_Name from Gen_LookupTable where N_ReferId = 455 order by N_ReferId asc";
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
                return Ok(api.Error(e));
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
                return Ok(api.Error(e));
            }
        }


         [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                int N_SaveDraft=0;
                int N_Status=0;
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nServiceEndID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ServiceEndID"].ToString());
                int nEmpID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_EmpID"].ToString());
                var dEndDate=MasterTable.Rows[0]["D_EndDate"].ToString();
                string xMethod=MasterTable.Rows[0]["X_Method"].ToString();
                int  nEOSDetailID=0;
                

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    SortedList QueryParams = new SortedList();
                   
                    // Auto Gen
                    string ServiceEndCode = "";
                    var values = MasterTable.Rows[0]["X_ServiceEndCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Params.Add("N_ServiceEndID", nServiceEndID);
                        ServiceEndCode = dLayer.GetAutoNumber("pay_EndOFService", "X_ServiceEndCode", Params, connection, transaction);
                        if (ServiceEndCode == "") { transaction.Rollback(); return Ok(api.Error("Unable to generate Service End Code")); }
                        MasterTable.Rows[0]["X_ServiceEndCode"] = ServiceEndCode;
                    }
                    MasterTable.Columns.Remove("X_Method");
                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and X_ServiceEndCode='" + ServiceEndCode + "' and N_FnyearID=" + nFnYearId;
                    nServiceEndID = dLayer.SaveData("pay_EndOFService", "N_ServiceEndID", DupCriteria, "", MasterTable, connection, transaction);
                    if (nServiceEndID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    QueryParams.Add("@nCompanyID", nCompanyID);
                    QueryParams.Add("@nFnYearID", nFnYearId);
                    QueryParams.Add("@N_ServiceEndID", nServiceEndID);
                    QueryParams.Add("@N_EmpID", nEmpID);
                    QueryParams.Add("@X_Method", xMethod);
                    object Savedraft = dLayer.ExecuteScalar("select CAST(B_IsSaveDraft as INT) from pay_EndOFService where N_CompanyID=@nCompanyID and N_ServiceEndID=@N_ServiceEndID", QueryParams, connection, transaction);
                    if(Savedraft!=null)
                        N_SaveDraft=myFunctions.getIntVAL(Savedraft.ToString());
                    object Status = dLayer.ExecuteScalar("select N_Status  from Pay_EmployeeStatus where X_Description=@X_Method", QueryParams, connection, transaction);
                    if(Status!=null)
                        N_Status=myFunctions.getIntVAL(Status.ToString());

                    object PositionID = dLayer.ExecuteScalar("select N_PositionID from vw_PayEmployee where N_CompanyID=@nCompanyID and N_EMPID=@N_EmpID", QueryParams, connection, transaction); 

                    if (N_SaveDraft == 0) 
                    {
                        dLayer.ExecuteNonQuery("Update Pay_Employee Set N_Status = " + N_Status + ",D_StatusDate='" + dEndDate.ToString() + "' Where N_CompanyID =" + nCompanyID + " And N_EmpID =" + nEmpID.ToString(), QueryParams, connection, transaction);
                        dLayer.ExecuteNonQuery("Update Pay_SuperVisor Set N_EmpID = 0  Where N_CompanyID =" + nCompanyID + " And N_PositionID =" + PositionID.ToString(), QueryParams, connection, transaction);
                    }
                    dLayer.DeleteData("pay_EndOfServiceSDetails", "N_ServiceEndID", nServiceEndID, "", connection, transaction);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                          nEOSDetailID = dLayer.SaveData("pay_EndOfServiceSDetails", "N_EOSDetailID", DetailTable, connection, transaction);
                    }
                    transaction.Commit();
                    return Ok(api.Success("Terminated"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nServiceEndID, int nEmpID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
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



                     transaction.Commit();
                }
                if (Results > 0)
                {                    
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_ServiceEndID",nServiceEndID.ToString());
                    return Ok(api.Success(res,"EOS deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to EoS"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }


        }

        [HttpGet("listEmployeeDetails")]
        public ActionResult ListEmployeeDetails(int nFnYearID,bool bAllBranchData,int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string X_Crieteria = "";
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@bAllBranchData", bAllBranchData);
            Params.Add("@nBranchID", nBranchID);
            if (bAllBranchData == true)
                    X_Crieteria = "N_CompanyID=@nCompanyID and N_Status<2 and N_FnyearID =@nFnYearID";
                else
                    X_Crieteria = "N_CompanyID=@nCompanyID and N_Status<2 and N_FnyearID =@nFnYearID and N_BranchID=@nBranchID";

            string sqlCommandText="select X_EmpCode,X_EmpName,N_CompanyID,N_EmpID,X_Position,X_Department,D_HireDate,N_Status,N_FnyearID,N_BranchID from vw_PayEmployee where "+X_Crieteria;
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
                return Ok(api.Error(e));
            }
        }

        [HttpGet("listDescription")]
        public ActionResult ListDescription(int nCountryID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nCountryID",nCountryID);
            string sqlCommandText="select N_ServiceEndID,X_ServiceEndCode,N_ServiceEndStatusID,X_ServiceEndStatusDesc,ServiceEndStatus from vw_ServiceEndSettings where N_CompanyID=@nCompanyID and N_CountryID=@nCountryID group by N_ServiceEndID,X_ServiceEndCode,N_ServiceEndStatusID,X_ServiceEndStatusDesc,ServiceEndStatus";
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
                return Ok(api.Error(e));
            }
        }

        [HttpGet("paymentDetails")]
        public ActionResult GetPaymentDetails(int nEmpID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nEmpID", nEmpID);
            string sqlCommandText="select X_Description,N_Payrate,N_Type,IsEOF from vw_Pay_PendingAmtsForTermination where N_CompanyID=@nCompanyID and N_EmpID=@nEmpID";
            SortedList OutPut = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
                    string sqlCommandCount="select count(*) as N_Count from vw_Pay_PendingAmtsForTermination where N_CompanyID=@nCompanyID and N_EmpID=@nEmpID";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";

                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                      
                    }
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
                return Ok(api.Error(e));
            }
        }
    }
}