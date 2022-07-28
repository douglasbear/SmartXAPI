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

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("schFeeDetails")]
    [ApiController]
    public class Sch_FeeDetails : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;

        public Sch_FeeDetails(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 444;
        }
        private readonly string connectionString;

        [HttpGet("details")]
        public ActionResult GetDetails( int nFnYearID, int nCompanyID, int nBranchID, bool bShowAllBranchData, int nAdmissionID)
        {
            DataTable Master = new DataTable();
            DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();
            DataTable Attachments = new DataTable();

            QueryParams.Add("@nCompanyID", nCompanyID);
            QueryParams.Add("@nBranchID", nBranchID);
            QueryParams.Add("@nFnYearID", nFnYearID);
            QueryParams.Add("@nAdmissionID", nAdmissionID);
            string Condition = "";
            string masterSql = "";
            string detailsSql = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
    
                    if (bShowAllBranchData == true)
                       masterSql = "Select * from vw_SchAdmission Where N_CompanyID=@nCompanyID and  N_AcYearID=@nFnYearID and N_AdmissionID=@nAdmissionID"; 
                    else 
                        masterSql = "Select * from vw_SchAdmission Where N_CompanyID=@nCompanyID and  N_AcYearID=@nFnYearID and N_BranchId=@nBranchID and N_AdmissionID=@nAdmissionID";              
                    Master = dLayer.ExecuteDataTable(masterSql, QueryParams, connection);
                    Master = _api.Format(Master, "master");
                    if (Master.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        detailsSql = "SELECT      ROW_NUMBER() over(ORDER BY  N_Type , B_Paid DESC, D_SalesDate) as SlNo,* from vw_Sch_AdmissionFee Where N_CustomerID="+nAdmissionID+" and N_CompanyID = " +nCompanyID + " and N_FnYearId="+nFnYearID+" and B_IsRemoved=0 ORDER By N_Type , B_Paid DESC, D_SalesDate ";
                        Detail = dLayer.ExecuteDataTable(detailsSql, QueryParams, connection);
                        Detail = _api.Format(Detail, "details");
                        if (Detail.Rows.Count == 0)
                        {
                            return Ok(_api.Notice("No Results Found"));
                        }
                        ds.Tables.Add(Detail);
                        ds.Tables.Add(Master);
                        return Ok(_api.Success(ds));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        // [HttpPost("save")]
        // public ActionResult UpdateStatus([FromBody] DataSet ds)
        // {
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();
        //             DataTable DetailTable;
        //               DetailTable = ds.Tables["details"];
        //                 SortedList Params = new SortedList();
        //                int nCompanyID = myFunctions.GetCompanyID(User);

        //                //GetYearID
        //             //   dLayer.ExecuteScalar("Select * from Sch_AcademicYear Where N_CompanyID=" + nCompanyID + " And X_AcYear ='" + cmbAcademicYear.Text + "'", Params,connection,transaction);
        //             //   if (dsUserCategory.Tables["Sch_AcademicYear"].Rows.Count > 0)
        //             //   {
        //             //      N_AcYearID = myFunctions.getIntVAL(dsUserCategory.Tables["Sch_AcademicYear"].Rows[0]["N_AcYearID"].ToString());
        //             //      D_AcYearDate = Convert.ToDateTime(dsUserCategory.Tables["Sch_AcademicYear"].Rows[0]["D_YearFrom"]);
        //             //   }
        //             foreach (DataRow row in DetailTable.Rows)
        //             {
        //                   DateTime DateFrom = Convert.ToDateTime(row["PeriodFrom"].ToString());
        //                   DateTime DateTo = Convert.ToDateTime(row["PeriodTo"].ToString()); 
        //             if (DateTo < DateFrom)
        //             {
        //                 msg.msgInformation(MYG.ReturnMultiLingualVal("-1111", "X_ControlNo", "Period To must greater than Period From"));
        //                 dba.Rollback();
        //                 return;
        //             }




















    }
}