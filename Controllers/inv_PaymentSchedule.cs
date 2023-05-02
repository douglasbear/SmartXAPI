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
    [Route("invPaymentSchedule")]
    [ApiController]

    public class Inv_PaymentSchedule : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
         private readonly IMyAttachments myAttachments;

        public Inv_PaymentSchedule(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf,IMyAttachments myAtt)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1261;
             myAttachments = myAtt;

        }

          [HttpGet("list")]
        public ActionResult GetInvPaymentSchedule(int? nCompanyId, int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    DataTable CountTable = new DataTable();
                    SortedList Params = new SortedList();
                    DataSet dataSet = new DataSet();
                    string sqlCommandText = "";
                    string sqlCommandCount = "";
                    string Searchkey = "";

                    int nUserID = myFunctions.GetUserID(User);


                    if (xSearchkey != null && xSearchkey.Trim() != "")
                        Searchkey = " and (X_VendorName like '%" + xSearchkey + "%' or X_ReferenceNo like '%" + xSearchkey + "%'  or FORMAT(D_ScheduleDate, 'dd-MMM-yyyy') like '%" + xSearchkey + "%')";

                    if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by D_ScheduleDate asc";
                    else
                    {
                        switch (xSortBy.Split(" ")[0])
                        {
                            case "X_ReferenceNo":
                                xSortBy = "X_ReferenceNo " + xSortBy.Split(" ")[1];
                                break;
                           
                            default: break;
                        }
                        xSortBy = " order by " + xSortBy;
                    }

                    int Count = (nPage - 1) * nSizeperpage;
                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvPayables_Schedule where N_CompanyID=@p1 and N_FnYearID=@p2 and  X_Type='PURCHASE' and  D_ScheduleDate is not null " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvPayables_Schedule_Schedule where N_CompanyID=@p1 and N_FnYearID=@p2  and  X_Type='PURCHASE' and  D_ScheduleDate is not null " + Searchkey + " and N_PurchaseID not in (select top(" + Count + ") N_PurchaseID from vw_InvVendorPaymentSchedule where N_CompanyID=@p1 and N_FnYearID=@p2  X_Type='PURCHASE' and N_BalanceAmount>0 and  D_ScheduleDate is not null" + xSortBy + " ) " + xSortBy;

                    // sqlCommandText = "select * from Inv_MRNDetails where N_CompanyID=@p1";
                    Params.Add("@p1", nCompanyId);
                    Params.Add("@p2", nFnYearId);
                    SortedList OutPut = new SortedList();


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count from vw_InvPayables_Schedule where N_CompanyID=@p1 and N_FnYearID=@p2 and X_Type='PURCHASE' and  D_ScheduleDate is not null" + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                    }
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
                return Ok(_api.Error(User, e));
            }
        }


        //   [HttpGet("list")]
        // public ActionResult GetInvPaymentSchedule(int? nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        // {
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             DataTable dt = new DataTable();
        //             DataTable CountTable = new DataTable();
        //             SortedList Params = new SortedList();
        //             DataSet dataSet = new DataSet();
        //             string sqlCommandText = "";
        //             string sqlCommandCount = "";
        //             string Searchkey = "";

        //             int nUserID = myFunctions.GetUserID(User);
        //               if (xSearchkey != null && xSearchkey.Trim() != "")
        //               Searchkey = "and (X_ScheduleCode like'%" + xSearchkey + "%'or X_StoreName like'%" + xSearchkey + "%')";

        //                if (xSortBy == null || xSortBy.Trim() == "")
        //                 xSortBy = " order by N_ScheduleID desc";
        //                else
        //                 xSortBy = " order by " + xSortBy;

                    
        //             int Count = (nPage - 1) * nSizeperpage;
        //             if (Count == 0)
        //                 sqlCommandText = "select top(" + nSizeperpage + ") [X_ScheduleCode] AS X_ScheduleCode,* from vw_InvVendorPaymentSchedule where N_CompanyID=@p1" + Searchkey + " " + xSortBy;
        //             else
        //                 sqlCommandText = "select top(" + nSizeperpage + ") [X_ScheduleCode] AS X_ScheduleCode,* from vw_InvVendorPaymentSchedule where N_CompanyID=@p1" + Searchkey + " and N_ScheduleID not in (select top(" + Count + ") N_ScheduleID from vw_InvVendorPaymentSchedule where N_CompanyID=@p1" + xSortBy + " ) " + xSortBy;

        //             Params.Add("@p1", nCompanyId);
        //           //  Params.Add("@p2", nFnYearId);
        //             SortedList OutPut = new SortedList();

        //             dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
        //             sqlCommandCount = "select count(*) as N_Count from vw_InvVendorPaymentSchedule where N_CompanyID=@p1 " + Searchkey + "";
        //             DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
        //             string TotalCount = "0";
        //             if (Summary.Rows.Count > 0)
        //             {
        //                 DataRow drow = Summary.Rows[0];
        //                 TotalCount = drow["N_Count"].ToString();
        //             }
        //             OutPut.Add("Details", _api.Format(dt));
        //             OutPut.Add("TotalCount", TotalCount);

        //             if (dt.Rows.Count == 0)
        //             {
        //                 return Ok(_api.Warning("No Results Found"));
        //             }
        //             else
        //             {
        //                 return Ok(_api.Success(OutPut));
        //             }
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(_api.Error(User, e));
        //     }
        // }


 

//  [HttpGet("fillDetails")]
//         public ActionResult InvPaymentScheduleDetails(int nCompanyID,int N_VendorID)
//         {
//             DataTable dt = new DataTable();
//             SortedList Params = new SortedList();
//             nCompanyID = myFunctions.GetCompanyID(User);
//             string sqlCommandText = "Select * from vw_InvPurhcaseForVendorPayment where N_CompanyID=@p1  and N_VendorID=@p2";
//             Params.Add("@p1", nCompanyID);
//            // Params.Add("@p2", N_FnYearID);
//              Params.Add("@p2", N_VendorID);
            


//             try
//             {
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     connection.Open();
//                     dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
//                 }
//                 dt = _api.Format(dt);
//                 if (dt.Rows.Count == 0)
//                 {
//                     return Ok(_api.Warning("No Results Found"));
//                 }
//                 else
//                 {
//                     return Ok(_api.Success(dt));
//                 }
//             }
//             catch (Exception e)
//             {
//                 return Ok(_api.Error(User, e));
//             }
//         }


        [HttpGet("fillDetails")]
        public ActionResult InvPaymentScheduleDetails(int nCompanyID,int N_VendorID,bool Schedule,DateTime dDate)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText=" ";
            // if(Schedule)
            // {
                sqlCommandText = "select * from vw_InvPayables_Schedule where N_CompanyID=@p1  and X_Type='PURCHASE' and N_BalanceAmount>0 and D_Date<='" + dDate + "'";
           // }
            // else{
            //     sqlCommandText = "select * from vw_InvPayables_Schedule where N_CompanyID=@p1  and X_Type='PURCHASE' and N_BalanceAmount>0 and D_ScheduleDate is null "; 
            // }
            Params.Add("@p1", nCompanyID);
           
            Params.Add("@p2", N_VendorID);
            


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


    [HttpGet("details")]
        public ActionResult BusRegDetails(string xScheduleCode,int nFnYearID)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_InvVendorPaymentSchedule where N_CompanyID=@p1  and X_ScheduleCode=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", xScheduleCode);
             Params.Add("@nFnYearID", nFnYearID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
 
                   

                }
                return Ok(_api.Success(dt));               
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }

        }



              [HttpPost("Save")]
          public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                     DataRow DetailRow = DetailTable.Rows[0];
                    SortedList Params = new SortedList();

                    int nScheduleID = myFunctions.getIntVAL(MasterRow["N_ScheduleID"].ToString());
                    int nScheduleDetailsID = myFunctions.getIntVAL(DetailRow["N_ScheduleDetailsID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string xScheduleCode = MasterRow["X_ScheduleCode"].ToString();

                     for (int j = 0; j < DetailTable.Rows.Count; j++)
                     {

                        int purchaseID = myFunctions.getIntVAL(DetailTable.Rows[j]["n_PurchaseID"].ToString());
                        if (purchaseID > 0)
                        {
                            dLayer.ExecuteNonQuery("Update Inv_Purchase SET D_ScheduleDate='" + DetailTable.Rows[j]["D_ScheduleDate"].ToString() + "',N_ScheduledAmtF=" + myFunctions.getVAL(DetailTable.Rows[j]["N_ScheduledAmtF"].ToString()) + " , N_ScheduledAmt=" + myFunctions.getVAL(DetailTable.Rows[j]["N_ScheduledAmt"].ToString())+" WHERE N_PurchaseID=" + myFunctions.getIntVAL(DetailTable.Rows[j]["n_PurchaseID"].ToString()) + " and N_CompanyID=" + nCompanyID + "", Params, connection, transaction);
                        
                        }
                     

                    }
                    DetailTable.AcceptChanges();

                    transaction.Commit();
                 
                    return Ok(_api.Success("Payment Schedule Created"));
                }
            }
            
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }





        //     [HttpPost("Save")]
        //   public ActionResult SaveData([FromBody] DataSet ds)
        // {
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();
        //             DataTable MasterTable;
        //             DataTable DetailTable;
        //             MasterTable = ds.Tables["master"];
        //             DetailTable = ds.Tables["details"];
        //             DataRow MasterRow = MasterTable.Rows[0];
        //              DataRow DetailRow = DetailTable.Rows[0];
        //             SortedList Params = new SortedList();

        //             int nScheduleID = myFunctions.getIntVAL(MasterRow["N_ScheduleID"].ToString());
        //             int nScheduleDetailsID = myFunctions.getIntVAL(DetailRow["N_ScheduleDetailsID"].ToString());
        //             int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
        //             int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
        //             string xScheduleCode = MasterRow["X_ScheduleCode"].ToString();

        //             string X_ScheduleCode = "";
        //             if (xScheduleCode == "@Auto")
        //             {
        //                 Params.Add("N_CompanyID", nCompanyID);
        //                 Params.Add("N_YearID", nFnYearID);
        //                 Params.Add("N_FormID", this.FormID);
        //                 X_ScheduleCode = dLayer.GetAutoNumber("Inv_VendorPaymentSchedule", "X_ScheduleCode", Params, connection, transaction);
        //                 if (X_ScheduleCode == "")
        //                 {
        //                     transaction.Rollback();
        //                     return Ok("Unable to generate Payment schedule");
        //                 }
        //                 MasterTable.Rows[0]["X_ScheduleCode"] = X_ScheduleCode;
        //             }
        //             else
        //             {
        //                  dLayer.DeleteData("Inv_VendorPaymentSchedule", "N_ScheduleID", nScheduleID, "", connection,transaction);
        //                   dLayer.DeleteData("Inv_VendorPaymentScheduleDetails", "N_ScheduleID", nScheduleID, "", connection,transaction);
        //             }
        //             MasterTable.Columns.Remove("n_FnYearID");

        //             int N_ScheduleID = dLayer.SaveData("Inv_VendorPaymentSchedule", "N_ScheduleID", "", "", MasterTable, connection, transaction);
        //             if (N_ScheduleID <= 0)
        //             {
        //                 transaction.Rollback();
        //                 return Ok("Unable to save Payment Schedule ");
        //             }
        //             for (int j = 0; j < DetailTable.Rows.Count; j++)
        //             {
        //                 DetailTable.Rows[j]["N_ScheduleID"] = N_ScheduleID;
        //             }
        //             int N_ScheduleDetailsID = dLayer.SaveData("Inv_VendorPaymentScheduleDetails", "N_ScheduleDetailsID", DetailTable, connection, transaction);
        //             if (N_ScheduleDetailsID <= 0)
        //             {
        //                 transaction.Rollback();
        //                 return Ok("Unable to save Payment Schedule");
        //             }

        //             transaction.Commit();
        //             SortedList Result = new SortedList();
        //             Result.Add("N_ScheduleID", N_ScheduleID);
        //             Result.Add("X_ScheduleCode", X_ScheduleCode);
        //             Result.Add("N_ScheduleDetailsID", N_ScheduleDetailsID);

        //             return Ok(_api.Success(Result, "Payment Schedule Created"));
        //         }
        //     }
            
        //     catch (Exception ex)
        //     {
        //         return Ok(_api.Error(User,ex));
        //     }
        // }

     [HttpDelete("delete")]
        public ActionResult DeleteData(int nScheduleID)
        {

             int Results = 0;
            try
            {                        
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();                
                QueryParams.Add("@nFormID", 1261);
                QueryParams.Add("@nScheduleID", nScheduleID);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Inv_VendorPaymentSchedule", "N_ScheduleID", nScheduleID, "", connection,transaction);
                
                    if (Results > 0)
                    {
                        dLayer.DeleteData("Inv_VendorPaymentScheduleDetails", "N_ScheduleID", nScheduleID, "", connection,transaction);
                        transaction.Commit();
                        return Ok(_api.Success("Payment Schedule deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete "));
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