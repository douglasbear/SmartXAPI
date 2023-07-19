using AutoMapper;
using SmartxAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("vendor")]
    [ApiController]
    public class Inv_Vendor : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        private readonly IMyAttachments myAttachments;
        public Inv_Vendor(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 52;
            myAttachments = myAtt;
        }


        //GET api/customer/list?....
        [HttpGet("list")]
        public ActionResult GetVendorList(int? nCompanyId, int nFnYearId, bool bAllBranchesData, string vendorId, string qry, string msg, int nQuotationID,int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string criteria = "";
           
            int nVendorId = 0;


            if (vendorId != "" && vendorId != null)
            {
                criteria = " and N_VendorID =@nVendorID ";
                nVendorId = myFunctions.getIntVAL(vendorId.ToString());
            }
            Params.Add("@nVendorID", nVendorId);

            string qryCriteria = "";
            if (qry != "" && qry != null)
            {
                qryCriteria = " and (X_VendorCode like @qry or X_VendorName like @qry ) ";
                Params.Add("@qry", "%" + qry + "%");
            }
            if (nQuotationID > 0) // Added for RFQ Vendor filltering in PO
            {
                Params.Add("@N_QuotationID", nQuotationID);
                criteria = criteria + " and N_VendorID in ( Select N_VendorID  from vw_RFQDecisionDetails where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and N_QuotationID=@N_QuotationID ) ";
            }

            string sqlCommandText = "select TOP 20 * from vw_InvVendor where B_Inactive=@bInactive and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID " + criteria + " " + qryCriteria + " order by N_VendorID DESC";
            Params.Add("@bInactive", 0);
            Params.Add("@nCompanyID", nCompanyId);
            Params.Add("@nFnYearID", nFnYearId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    dt = _api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Success(dt));
                    }
                    else
                    {
                        if (nVendorId > 0)
                        {
                            bool B_IsUsed = false;
                            object objIsUsed = dLayer.ExecuteScalar("Select count(1) From Acc_VoucherDetails where N_AccID=@nVendorID and N_AccType=1", Params, connection);
                            if (objIsUsed != null)
                                if (myFunctions.getIntVAL(objIsUsed.ToString()) > 0)
                                    B_IsUsed = true;
                            myFunctions.AddNewColumnToDataTable(dt, "B_IsUsed", typeof(Boolean), B_IsUsed);

                            object objUsedCount = dLayer.ExecuteScalar("Select count(1) from vw_Inv_CheckVendor Where N_CompanyID=@nCompanyID and N_VendorID=@nVendorID", Params, connection);
                            if (objUsedCount != null)
                                myFunctions.AddNewColumnToDataTable(dt, "N_UsedCount", typeof(int), myFunctions.getIntVAL(objUsedCount.ToString()));
                        }

                        if (msg == "")
                            return Ok(_api.Success(dt));
                        else
                            return Ok(_api.Success(dt, msg));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("listWithCurrency")]
        public ActionResult GetVendorDispList(int? nCompanyId, int nFnYearId, bool bAllBranchesData, string vendorId, string qry)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string criteria = "";
            int nVendorId = 0;
            if (vendorId != "" && vendorId != null)
            {
                criteria = " and N_VendorID =@nVendorID ";
                nVendorId = myFunctions.getIntVAL(vendorId.ToString());
            }
            Params.Add("@nVendorID", nVendorId);

            string qryCriteria = "";
            if (qry != "" && qry != null)
            {
                qryCriteria = " and (X_VendorCode like @qry or X_VendorName like @qry ) ";
                Params.Add("@qry", "%" + qry + "%");
            }
            string sqlCommandText = "SELECT Top 20 X_VendorCode, X_VendorName, X_ContactName, X_PhoneNo1, X_PhoneNo2, N_LedgerID, N_InvDueDays, N_FnYearID, N_CurrencyID, B_DirPosting, N_TypeID,X_CountryCode, X_ReminderMsg, X_CurrencyCode, X_CurrencyName, N_ExchangeRate, N_CountryID, B_AllowCashPay, X_TaxRegistrationNo, X_Country, N_VendorID, N_CompanyID FROM vw_Inv_VendorCurrency_Disp where B_Inactive=@bInactive and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID " + criteria + " " + qryCriteria + " order by X_VendorName,X_VendorCode";
            Params.Add("@bInactive", 0);
            Params.Add("@nCompanyID", nCompanyId);
            Params.Add("@nFnYearID", nFnYearId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

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
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("dashboardList")]
        public ActionResult GetDashboardList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy, int nFnYearId,bool isEnable)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_VendorName like '%" + xSearchkey + "%' or X_VendorCode like '%" + xSearchkey + "%' or X_ContactName like '%" + xSearchkey + "%' or X_Address like '%" + xSearchkey + "%' or X_VendorType like '%" + xSearchkey + "%'or X_Country like '%" + xSearchkey + "%'or X_CurrencyName like '%" + xSearchkey + "%' or x_PhoneNo1 like '%" + xSearchkey + "%')";


            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_VendorID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "x_VendorCode":
                        xSortBy = "N_VendorID " + xSortBy.Split(" ")[1];
                        break;

                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }
             if(isEnable==false){
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") N_VendorID,X_VendorCode,X_VendorName,N_CountryID,X_Country,N_TypeID,X_TypeName,N_CurrencyID,X_CurrencyName,X_ContactName,X_Address,X_PhoneNo1,X_VendorType,X_VendorName_Ar from vw_InvVendor where N_CompanyID=@p1 and N_FnYearID=@nFnYearId and b_Inactive=0 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_VendorID,X_VendorCode,X_VendorName,N_CountryID,X_Country,N_TypeID,X_TypeName,N_CurrencyID,X_CurrencyName,X_ContactName,X_Address,X_PhoneNo1,X_VendorType,X_VendorName_Ar from vw_InvVendor where N_CompanyID=@p1 and N_FnYearID=@nFnYearId and b_Inactive=0 " + Searchkey + " and N_VendorID not in (select top(" + Count + ") N_VendorID from vw_InvVendor where N_CompanyID=@p1 and N_FnYearID=@nFnYearId " + Searchkey + xSortBy + " ) " + xSortBy;
             }

             if(isEnable==true){
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") N_VendorID,X_VendorCode,X_VendorName,N_CountryID,X_Country,N_TypeID,X_TypeName,N_CurrencyID,X_CurrencyName,X_ContactName,X_Address,X_PhoneNo1,X_VendorType,X_VendorName_Ar from vw_InvVendor where N_CompanyID=@p1 and N_FnYearID=@nFnYearId and b_Inactive=1" + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_VendorID,X_VendorCode,X_VendorName,N_CountryID,X_Country,N_TypeID,X_TypeName,N_CurrencyID,X_CurrencyName,X_ContactName,X_Address,X_PhoneNo1,X_VendorType,X_VendorName_Ar from vw_InvVendor where N_CompanyID=@p1 and N_FnYearID=@nFnYearId  and b_Inactive=1" + Searchkey + " " + xSortBy;
             }
            Params.Add("@p1", nCompanyID);
            Params.Add("@nFnYearId", nFnYearId);
            Params.Add("@p2", 0);

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    string sqlCommandCount = "select count(1) as N_Count  from vw_InvVendor where N_CompanyID=@p1  and N_FnYearID=@nFnYearId " + Searchkey + "";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        // return Ok(_api.Warning("No Results Found"));
                        return Ok(_api.Success(OutPut));
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



        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                DataTable Attachment = ds.Tables["attachments"];
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();
                // Auto Gen
                DataRow MasterRow = MasterTable.Rows[0];
                string VendorCode = "";
                var xVendorCode = MasterRow["x_VendorCode"].ToString();
                int nVendorID = myFunctions.getIntVAL(MasterRow["n_VendorID"].ToString());
                int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyId"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearId"].ToString());
                bool b_AutoGenerate = false;
                int flag = 0;
                String xButtonAction="";
                bool showConfirmationVendor=false;
                int vendorFlag=0;
                //gLAccount AutoGen
                if (MasterTable.Columns.Contains("b_AutoGenerate"))
                {
                    b_AutoGenerate = myFunctions.getBoolVAL(MasterTable.Rows[0]["b_AutoGenerate"].ToString());
                    MasterTable.Columns.Remove("b_AutoGenerate");
                }

                string x_VendorName = (MasterTable.Rows[0]["x_VendorName"].ToString());
                if (MasterTable.Columns.Contains("flag"))
                {
                    flag = myFunctions.getIntVAL(MasterTable.Rows[0]["flag"].ToString());
                    MasterTable.Columns.Remove("flag");
                }
                  if(MasterTable.Columns.Contains("vendorFlag")){
                    vendorFlag=myFunctions.getIntVAL(MasterTable.Rows[0]["vendorFlag"].ToString());
                    MasterTable.Columns.Remove("vendorFlag");
                }

                bool showConformationLedger = false;

                QueryParams.Add("@nCompanyID", MasterRow["n_CompanyId"].ToString());
                QueryParams.Add("@nFnYearID", MasterRow["n_FnYearId"].ToString());
                QueryParams.Add("@nFormID", 52);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (myFunctions.getBoolVAL(myFunctions.checkProcessed("Acc_FnYear", "B_YearEndProcess", "N_FnYearID", "@nFnYearID", "N_CompanyID=@nCompanyID ", QueryParams, dLayer, connection)))
                        return Ok(_api.Warning("Year is closed, Cannot create new Vendor..."));

                    SqlTransaction transaction = connection.BeginTransaction(); ;
                    if (xVendorCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        VendorCode = dLayer.GetAutoNumber("Inv_Vendor", "x_VendorCode", Params, connection, transaction);
                         xButtonAction="Insert"; 
                       
                        if (VendorCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to save")); }
                        MasterTable.Rows[0]["x_VendorCode"] = VendorCode;

                        SortedList vendorNewParams = new SortedList();
                       vendorNewParams.Add("@x_VendorName", x_VendorName);

                     object VendorCount = dLayer.ExecuteScalar("select count(N_VendorID) from Inv_Vendor  Where X_VendorName =@x_VendorName and N_CompanyID=" + nCompanyID, vendorNewParams, connection,transaction);
                        if( myFunctions.getIntVAL(VendorCount.ToString())>0)
                                    {
                                           if (vendorFlag == 2)
                                    {
                                         showConfirmationVendor =true;
                                        transaction.Rollback();
                                        return Ok(_api.Success(2));
                                    }
                                    }


                    }else {
                         xButtonAction="Update"; 

                    }
                     VendorCode = MasterTable.Rows[0]["x_VendorCode"].ToString();

                    if (MasterTable.Columns.Contains("b_DirPosting"))
                    {
                        MasterTable.Rows[0]["b_DirPosting"] = 0;
                    }
                    else
                    {
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "b_DirPosting", typeof(int), 0);
                    }

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and X_VendorCode='" + VendorCode + "'";
                    string X_Crieteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                    nVendorID = dLayer.SaveData("Inv_Vendor", "N_VendorID", DupCriteria, X_Crieteria, MasterTable, connection, transaction);
                  
                    if (nVendorID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }
                    else
                    {
                        object N_GroupID = dLayer.ExecuteScalar("select Isnull(N_FieldValue,0) FRom Acc_AccountDefaults Where X_FieldDescr ='Vendor Account Group' and N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID, Params, connection, transaction);
                        string X_LedgerName = "";
                        if (b_AutoGenerate)
                        {
                            X_LedgerName = x_VendorName;
                            if (N_GroupID != null)
                            {
                                object N_LedgerID = dLayer.ExecuteScalar("select N_LedgerID FRom Acc_MastLedger Where N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + " and N_GroupID =" + N_GroupID + " and X_LedgerName ='" + X_LedgerName + "' ", Params, connection, transaction);
                                if (N_LedgerID != null)
                                {
                                    if (flag == 2)//for confirmation of same ledger creattion 
                                    {
                                        showConformationLedger = true;
                                        return Ok(_api.Success(showConformationLedger));
                                    }

                                    if (flag == 1)//for same account for olready exist 
                                    {
                                        dLayer.ExecuteNonQuery("SP_Inv_CreateVendorAccount " + nCompanyID + "," + nVendorID + ",'" + nVendorID + "','" + X_LedgerName + "'," + myFunctions.GetUserID(User) + "," + nFnYearID + "," + "Vendor", Params, connection, transaction);
                                    }
                                    else// update ledger id
                                    {
                                        dLayer.ExecuteNonQuery("Update Inv_Vendor Set N_LedgerID =" + myFunctions.getIntVAL(N_LedgerID.ToString()) + " Where N_VendorID=" + nVendorID + "and N_CompanyID= " + nCompanyID + " and  N_FnYearID = " + nFnYearID, Params, connection, transaction);
                                        
                                    }
                                }
                                else
                                {
                                    dLayer.ExecuteNonQuery("SP_Inv_CreateVendorAccount " + nCompanyID + "," + nVendorID + ",'" + nVendorID + "','" + X_LedgerName + "'," + myFunctions.GetUserID(User) + "," + nFnYearID + "," + "Vendor", Params, connection, transaction);
                               
                                }
                            }
                            // else
                            // msg.msgError("No DefaultGroup");
                        }

                      
                        
                               //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(nFnYearID,nVendorID,VendorCode,52,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                       

          


                        SortedList nParams = new SortedList();
                        nParams.Add("@nCompanyID", nCompanyID);
                        nParams.Add("@nFnYearID", nFnYearID);
                        nParams.Add("@nVendorID", nVendorID);
                        string sqlCommandText = "select * from vw_InvVendor where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_VendorID =@nVendorID  order by X_VendorName,X_VendorCode";
                        DataTable outputDt = dLayer.ExecuteDataTable(sqlCommandText, nParams, connection, transaction);
                        outputDt = _api.Format(outputDt, "NewVendor");

                        if (outputDt.Rows.Count == 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to save"));
                        }
                        DataRow NewRow = outputDt.Rows[0];


                     
                        try
                        {
                            myAttachments.SaveAttachment(dLayer, Attachment, MasterTable.Rows[0]["x_VendorCode"].ToString() + "-" + MasterTable.Rows[0]["x_VendorName"].ToString(), 0, MasterTable.Rows[0]["x_VendorName"].ToString(), MasterTable.Rows[0]["x_VendorCode"].ToString(), nVendorID, "Vendor Document", User, connection, transaction);
                        }

                        
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, ex));
                        }

                        transaction.Commit();
                        return Ok(_api.Success(NewRow.Table, "Vendor successfully created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nVendorID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();
                SortedList ParamList = new SortedList();
                DataTable TransData = new DataTable();
                nCompanyID = myFunctions.GetCompanyID(User);
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nFormID", 52);
                QueryParams.Add("@nVendorID", nVendorID);
                  ParamList.Add("@nTransID", nVendorID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    ParamList.Add("@nCompanyID", nCompanyID);


                     string Sql = "select N_VendorID,X_VendorCode from Inv_Vendor where N_VendorID=@nTransID and N_CompanyID=@nCompanyID ";
                  string xButtonAction="Delete";
                  string X_VendorCode="";
                  object vendorCount="";
                  object vendortxnCount="";

                   

       
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                   if (myFunctions.getBoolVAL(myFunctions.checkProcessed("Acc_FnYear", "B_YearEndProcess", "N_FnYearID", "@nFnYearID", "N_CompanyID=@nCompanyID ", QueryParams, dLayer, connection)))
                            return Ok(_api.Error(User, "Year is closed, Cannot create new Vendor..."));
                            vendorCount = dLayer.ExecuteScalar("select count(N_PartyID) from Inv_PayReceipt  Where N_CompanyID=" + nCompanyID + " and  N_PartyID=" + nVendorID,  QueryParams, connection);

                        if( myFunctions.getIntVAL(vendorCount.ToString())>0)
                    {
                        return Ok(_api.Error(User, "Unable to delete vendor! transaction started"));
                    }
                    vendortxnCount = dLayer.ExecuteScalar("select count(N_PartyID) from Inv_BalanceAdjustmentMaster  Where N_CompanyID=" + nCompanyID + " and  N_PartyID=" + nVendorID,  QueryParams, connection);

                        if( myFunctions.getIntVAL(vendortxnCount.ToString())>0)
                    {
                        return Ok(_api.Error(User, "Unable to delete vendor! It has been used."));
                    }
                    


                        vendorCount = dLayer.ExecuteScalar("select count(N_PartyID) from Inv_PayReceipt  Where N_CompanyID=" + nCompanyID + " and  N_PartyID=" + nVendorID,  QueryParams, connection);
                        if( myFunctions.getIntVAL(vendorCount.ToString())>0)
                    {
                        return Ok(_api.Error(User, "Unable to delete vendor! transaction started"));
                    }
                    vendortxnCount = dLayer.ExecuteScalar("select count(N_PartyID) from Inv_BalanceAdjustmentMaster  Where N_CompanyID=" + nCompanyID + " and  N_PartyID=" + nVendorID,  QueryParams, connection);
                        if( myFunctions.getIntVAL(vendortxnCount.ToString())>0)
                    {
                        return Ok(_api.Error(User, "Unable to delete vendor! It has been used."));
                    }

                    SqlTransaction transaction = connection.BeginTransaction();
                      TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection,transaction);
                    
                      if (TransData.Rows.Count == 0)
                    {
                        return Ok(_api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];


                     //  Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(myFunctions.getIntVAL( nFnYearID.ToString()),nVendorID,TransRow["X_VendorCode"].ToString(),52,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);


                   

                    Results = dLayer.DeleteData("Inv_Vendor", "N_VendorID", nVendorID, "N_CompanyID="+nCompanyID, connection, transaction);
            
                   
                   
                    myAttachments.DeleteAttachment(dLayer, 1, 0, nVendorID, nFnYearID, this.FormID, User, transaction, connection);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("n_VendorID", nVendorID.ToString());
                    return Ok(_api.Success(res, "Vendor deleted"));
                }
                else
                {
                    return Ok(_api.Error(User, "Unable to delete vendor"));
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("REFERENCE constraint"))
                    return Ok(_api.Error(User, "Unable to delete vendor! It has been used."));
                else
                    return Ok(_api.Error(User, ex));
            }


        }
        [HttpGet("details")]
        public ActionResult ActivityListDetails(string xVendorCode, int nFnYearID)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "Select * from vw_InvVendor Where N_CompanyID=@p1 and N_FnYearID=@nFnYearID and X_VendorCode=@xVendorCode";
            Params.Add("@p1", nCompanyId);
            Params.Add("@xVendorCode", xVendorCode);
            Params.Add("@nFnYearID", nFnYearID);
            // Params.Add("@nVendorID",nVendorID);
           
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    dt = _api.Format(dt);

                    int nVendorID = myFunctions.getIntVAL(dt.Rows[0]["N_VendorID"].ToString());

                    Params.Add("@nVendorID", nVendorID);

                    object Count = dLayer.ExecuteScalar("select count(1)  from vw_Inv_CheckVendor where N_CompanyID=@p1 and X_VendorCode=@xVendorCode", Params, connection);
                    int NCount = myFunctions.getIntVAL(Count.ToString());
                    if (NCount > 0)
                    {
                       
                        if (dt.Rows.Count > 0)
                        {
                            dt.Columns.Add("b_isUsed");
                            dt.Rows[0]["b_isUsed"]=true;
                            
                          
                        }

                          object VendorCount = dLayer.ExecuteScalar("select Isnull(Count(N_PurchaseID),0) from Inv_Purchase  INNER JOIN  Acc_FnYear ON Inv_Purchase.N_FnYearID = Acc_FnYear.N_FnYearID AND Inv_Purchase.N_CompanyID = Acc_FnYear.N_CompanyID  where Inv_Purchase.N_VendorID=@nVendorID and Inv_Purchase.N_CompanyID=@p1 and ISNULL(Acc_FnYear.B_PreliminaryYear,0)=0", Params, connection);
                    
                          object vendorCounts = dLayer.ExecuteScalar("select Isnull(Count(N_PayReceiptId),0) from Inv_PayReceipt where N_PartyID=@nVendorID and N_CompanyID=@p1", Params, connection);

                         if (myFunctions.getIntVAL(VendorCount.ToString()) > 0 || myFunctions.getIntVAL(vendorCounts.ToString()) > 0)
                         {
                                myFunctions.AddNewColumnToDataTable(dt, "b_VendorCount", typeof(bool), true);
                         }


                    }

                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        DataTable Attachments = myAttachments.ViewAttachment(dLayer, myFunctions.getIntVAL(dt.Rows[0]["n_VendorID"].ToString()), 0, this.FormID, nFnYearID, User, connection);
                        Attachments = _api.Format(Attachments, "attachments");
                        dt = _api.Format(dt, "master");

                        ds.Tables.Add(dt);
                        ds.Tables.Add(Attachments);

                        return Ok(_api.Success(ds));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("default")]
        public ActionResult GetDefault(int nFnYearID, int nLangID, int nFormID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));

                    DataTable QList = myFunctions.GetSettingsTable();
                    QList.Rows.Add("DEFAULT_ACCOUNTS", "Creditor Account");
                    QList.Rows.Add("DEFAULT_ACCOUNTS", "S Cash Account");

                    QList.AcceptChanges();

                    DataTable Details = dLayer.ExecuteSettingsPro("SP_GenSettings_Disp", QList, myFunctions.GetCompanyID(User), nFnYearID, connection);

                    SortedList OutPut = new SortedList(){
                            {"settings",_api.Format(Details)}
                        };
                    return Ok(_api.Success(OutPut));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        [HttpGet("listdetails")]
        public ActionResult ListDetails(int nCompanyID, int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearID);
            string sqlCommandText = "select * from vw_InvVendor where N_CompanyID=@p1 and N_FnYearID=@p2";


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

              [HttpGet("vatCodelist")]
        public ActionResult GetVatCodeList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            string sqlCommandText="Select * from Inv_TaxCategoryType Where N_CompanyID=@nCompanyID ";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
                return Ok(_api.Error(User,e));
            }
        }
         [HttpGet("vendorBalance")]
        public ActionResult GetCustomerDetail(int nVendorID )
        {
            DataTable dt = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommmand = "";
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);


            SortedList OutPut = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    Params.Add("@nVendorID", nVendorID);
                      sqlCommmand = "select top 1 X_VendorName from Inv_Vendor where N_CompanyID=@nCompanyID and  N_VendorID="+nVendorID+"";
                         dt = dLayer.ExecuteDataTable(sqlCommmand, Params, connection);
                    double  currentBalance=  myFunctions.getVAL(dLayer.ExecuteScalar("SELECT  Sum(n_Amount)  as N_BalanceAmount from  vw_InvVendorStatement Where N_AccType=1 and N_AccID=" + nVendorID + " and N_CompanyID=" + nCompanyID,Params,connection).ToString());
                    
                    dt = myFunctions.AddNewColumnToDataTable(dt, "currentBalance", typeof(double), currentBalance);
                  
                    dt.AcceptChanges();   
                   }
                    
                    
                    return Ok(_api.Success(dt));

                }
                    catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
            }
        


    }
}