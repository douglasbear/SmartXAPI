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
                        detailsSql = "SELECT      ROW_NUMBER() over(ORDER BY  N_Type , B_Paid DESC, D_SalesDate) as SlNo,* from vw_Sch_AdmissionFee Where N_RefID="+nAdmissionID+" and N_CompanyID = " +nCompanyID + " and N_FnYearId="+nFnYearID+" and B_IsRemoved=0 ORDER By N_Type , B_Paid DESC, D_SalesDate ";
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

        [HttpPost("save")]
        public ActionResult UpdateStatus([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable DetailTable;
                    DetailTable = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    bool flag=false;
                    int N_AcYearID=myFunctions.getIntVAL(DetailTable.Rows[0]["N_AcYearID"].ToString());
                    int N_RefSalesID=0;
                       //GetYearID
                    //   dLayer.ExecuteScalar("Select * from Sch_AcademicYear Where N_CompanyID=" + nCompanyID + " And X_AcYear ='" + cmbAcademicYear.Text + "'", Params,connection,transaction);
                    //   if (dsUserCategory.Tables["Sch_AcademicYear"].Rows.Count > 0)
                    //   {
                    //      N_AcYearID = myFunctions.getIntVAL(dsUserCategory.Tables["Sch_AcademicYear"].Rows[0]["N_AcYearID"].ToString());
                    //      D_AcYearDate = Convert.ToDateTime(dsUserCategory.Tables["Sch_AcademicYear"].Rows[0]["D_YearFrom"]);
                    //   }
                    foreach (DataRow row in DetailTable.Rows)
                    {
                          DateTime DateFrom = Convert.ToDateTime(row["d_DateFrom"].ToString());
                          DateTime DateTo = Convert.ToDateTime(row["d_DateTill"].ToString()); 
                    if (DateTo < DateFrom)
                    {   
                           transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }
                    if (row["Status"].ToString() == "Not Paid")
                    {
                        dLayer.ExecuteNonQuery("Update Sch_Sales set D_SalesDate = '" + row["d_DateFrom"] + "', N_SalesAmt = " + myFunctions.getVAL( row["Fee"].ToString())  + " where N_CompanyID = " +nCompanyID + " and N_FnYearId= " + N_AcYearID + " and N_SalesId=" + myFunctions.getIntVAL(row["N_SalesId"].ToString()) + "", Params,connection,transaction);
                        dLayer.ExecuteNonQuery("Update Sch_SalesDetails set D_DateFrom = '" + row["d_DateFrom"] + "',D_DateTill = '" +row["d_DateTill"] + "',N_Amount = '" +  myFunctions.getVAL(row["Fee"].ToString()) + "' where N_SalesId=" +   myFunctions.getIntVAL(row["N_SalesId"].ToString()) + "  and N_ItemID=" +  myFunctions.getIntVAL(row["N_ItemID"].ToString()) + "",  Params,connection,transaction);
                    }
                  
                    if (row["b_Paid"].ToString() == "False" && row["Status"].ToString() == "Not Paid")//unchecked
                    {
                        dLayer.ExecuteNonQuery("Update Sch_Sales set B_IsRemoved=1 where N_CompanyID = " + nCompanyID + " and N_FnYearId= " +  N_AcYearID + " and N_SalesId=" +  myFunctions.getIntVAL(row["N_SalesId"].ToString())  + "",  Params,connection,transaction);
                        N_RefSalesID= myFunctions.getIntVAL(row["N_RefSalesID"].ToString());
                          SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType","SALES"},
                                {"N_VoucherID",N_RefSalesID}};
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }




                    }
                 }
                     transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
             catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

          [HttpDelete("delete")]
        public ActionResult DeleteData(int nSalesId,bool bPaid,string Status)
        {
          
            int Results = 0;
            int nCompanyID=myFunctions.GetCompanyID(User);
            int N_AcYearID=0;
         
            try
            {                        
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    
                    Results = dLayer.DeleteData("Sch_Sales ", "N_SalesId", nSalesId, "N_CompanyID =" + nCompanyID, connection, transaction);
                    
                      if (bPaid == false && Status == "Not Paid")//unchecked
                    {
                       Results= dLayer.ExecuteNonQuery("Update Sch_Sales set B_IsRemoved=1 where N_CompanyID = " + nCompanyID + " and N_FnYearId= " +  N_AcYearID + " and N_SalesId=" + nSalesId + "",  Params,connection,transaction);
                          SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType","SALES"},
                                {"N_VoucherID",nSalesId}};
                    }
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Fee details deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete Fee details"));
                    }
                    }
                

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }



        }
    }
}