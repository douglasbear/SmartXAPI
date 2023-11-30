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
    [Route("customer")]
    [ApiController]
    public class Inv_Customer : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly IMyAttachments myAttachments;
        private readonly string AppURL;

        public Inv_Customer(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            myAttachments = myAtt;
            AppURL = conf.GetConnectionString("AppURL");
        }



        //GET api/customer/list?....
        [HttpGet("list")]
        public ActionResult GetCustomerList(int? nCompanyId, int nFnYearId, int nBranchId, bool bAllBranchesData, string customerId, string qry)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string criteria = "";
            if (customerId != "" && customerId != null)
            {
                criteria = " and N_CustomerID =@customerId ";
                Params.Add("@customerId", customerId);
            }

            string qryCriteria = "";
            if (qry != "" && qry != null)
            {
                qryCriteria = " and (X_CustomerCode like @qry or X_CustomerName like @qry or X_PhoneNo1 = CAST("+qry+" as VARCHAR) or X_TaxRegistrationNo = CAST("+qry+" as VARCHAR)) ";
               
                Params.Add("@qry", "%" + qry + "%");
            }

            string X_Crieteria = "";
            if (bAllBranchesData == true)
            { X_Crieteria = " where B_Inactive=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3 and ISNULL(N_EnablePopup,0)=0"; }
            else
            {
                X_Crieteria = " where B_Inactive=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3  and (N_BranchID=@p4 or N_BranchID=@p5) and ISNULL(N_EnablePopup,0)=0";
                Params.Add("@p4", 0);
                Params.Add("@p5", nBranchId);
            }
            string sqlCommandText = "select * from vw_InvCustomer " + X_Crieteria + " " + criteria + " " + qryCriteria + " order by N_CustomerID DESC";
            Params.Add("@p1", 0);
            Params.Add("@p2", nCompanyId);
            Params.Add("@p3", nFnYearId);

          
            

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
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("dashboardList")]
        public ActionResult GetDashboardList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nBranchID,bool bAllBranchData,bool bActiveCustomer)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
             string sqlCommandCount = "";

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_CustomerName like '%" + xSearchkey + "%' or X_CustomerCode like '%" + xSearchkey + "%' or X_ContactName like '%" + xSearchkey + "%' or X_Address like '%" + xSearchkey + "%' or X_PhoneNo1 like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_CustomerID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "x_CustomerCode":
                        xSortBy = "X_CustomerCode " + xSortBy.Split(" ")[1];
                        break;

                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }
              if (bAllBranchData == false)
                   
                    {
                        Searchkey = Searchkey + " and ( N_BranchID= "+nBranchID+" or N_BranchID=0 ) ";
                    }
            // if(nBranchID>0)
            // Searchkey= Searchkey + " and N_BranchID= "+nBranchID+" ";
             if(bActiveCustomer==false){
                    if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") N_CustomerID,X_CustomerCode,X_CustomerName,N_CountryID,X_Country,N_TypeID,X_TypeName,N_BranchID,X_BranchName,X_ContactName,X_Address,X_PhoneNo1,X_CustomerName_Ar from vw_InvCustomer where N_CompanyID=@p1  and N_FnYearId=@p3 and ISNULL(N_EnablePopup,0)=0 " + Searchkey + " and B_Inactive=0 " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_CustomerID,X_CustomerCode,X_CustomerName,N_CountryID,X_Country,N_TypeID,X_TypeName,N_BranchID,X_BranchName,X_ContactName,X_Address,X_PhoneNo1,X_CustomerName_Ar from vw_InvCustomer where N_CompanyID=@p1  and N_FnYearId=@p3 and ISNULL(N_EnablePopup,0)=0 " + Searchkey + " and N_CustomerID not in (select top(" + Count + ") N_CustomerID from vw_InvCustomer where N_CompanyID=@p1 and B_Inactive=@p2 and ISNULL(N_EnablePopup,0)=0 " + Searchkey + xSortBy + " ) " + xSortBy;

             }
               if(bActiveCustomer==true){
             if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") N_CustomerID,X_CustomerCode,X_CustomerName,N_CountryID,X_Country,N_TypeID,X_TypeName,N_BranchID,X_BranchName,X_ContactName,X_Address,X_PhoneNo1,X_CustomerName_Ar from vw_InvCustomer where N_CompanyID=@p1  and N_FnYearId=@p3 and ISNULL(N_EnablePopup,0)=0 " + Searchkey +" and B_Inactive=1 " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_CustomerID,X_CustomerCode,X_CustomerName,N_CountryID,X_Country,N_TypeID,X_TypeName,N_BranchID,X_BranchName,X_ContactName,X_Address,X_PhoneNo1,X_CustomerName_Ar from vw_InvCustomer where N_CompanyID=@p1  and N_FnYearId=@p3 and ISNULL(N_EnablePopup,0)=0 " + Searchkey +" and B_Inactive=1 " + xSortBy;

             }
      
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", 0);
            Params.Add("@p3", nFnYearId);

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                      if(bActiveCustomer==true){
                       sqlCommandCount = "select count(1) as N_Count  from vw_InvCustomer where N_CompanyID=@p1 and N_FnYearId=@p3 and ISNULL(N_EnablePopup,0)=0 and B_Inactive=1" + Searchkey + "";
                      }
                      else{
                       sqlCommandCount = "select count(1) as N_Count  from vw_InvCustomer where N_CompanyID=@p1 and N_FnYearId=@p3 and ISNULL(N_EnablePopup,0)=0 " + Searchkey + "";
                      }
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
                return Ok(api.Error(User, e));
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
                 bool b_AutoGenerate=false;
                string CustomerCode = "";
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nBranchId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchId"].ToString());
                int nCustomerID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CustomerId"].ToString());
                 bool isSave = myFunctions.getBoolVAL(MasterTable.Rows[0]["isSave"].ToString());
                int nLedgerID=0;
                object CustCount="";
                String xButtonAction="";
               
                if(MasterTable.Columns.Contains("N_LedgerID"))
                {
                    nLedgerID=myFunctions.getIntVAL(MasterTable.Rows[0]["N_LedgerID"].ToString());

                }
                 
                int nCrmCustomerID = 0;
                int flag=0;
                int customerFlag=0;
                bool showConfirmationCustomer=false;

                if(MasterTable.Columns.Contains("b_AutoGenerate"))
                {
                       b_AutoGenerate = myFunctions.getBoolVAL(MasterTable.Rows[0]["b_AutoGenerate"].ToString());
                        MasterTable.Columns.Remove("b_AutoGenerate");
                }
              
                string x_CustomerName = (MasterTable.Rows[0]["x_CustomerName"].ToString());
                if(MasterTable.Columns.Contains("flag"))
                {
                 flag = myFunctions.getIntVAL(MasterTable.Rows[0]["flag"].ToString());
                 MasterTable.Columns.Remove("flag");
                }

                if(MasterTable.Columns.Contains("customerFlag")){
                    customerFlag=myFunctions.getIntVAL(MasterTable.Rows[0]["customerFlag"].ToString());
                    MasterTable.Columns.Remove("customerFlag");
                }
                
                if(MasterTable.Columns.Contains("isSave")){
                    isSave=myFunctions.getBoolVAL(MasterTable.Rows[0]["isSave"].ToString());
                    MasterTable.Columns.Remove("isSave");
                }

                bool showConformationLedger = false;


                int bEnableLogin = 0;
                if (MasterTable.Columns.Contains("B_EnablePortalLogin"))
                    bEnableLogin = Convert.ToInt32(MasterTable.Rows[0]["B_EnablePortalLogin"].ToString());
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    // Auto Gen
                    //var values = MasterTable.Rows[0]["X_CustomerCode"].ToString();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    
                    CustomerCode = MasterTable.Rows[0]["X_CustomerCode"].ToString();
                    if(nCustomerID==0 && CustomerCode!="@Auto"){
                     CustCount = dLayer.ExecuteScalar("select count(N_customerID) from Inv_Customer  Where N_CompanyID=" + nCompanyID + " and  X_CustomerCode='" + CustomerCode+"'",  Params, connection,transaction);
                      if( myFunctions.getIntVAL(CustCount.ToString())>0){
                       CustomerCode = "@Auto";
                    }
                    } 
                   
                    if (CustomerCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", 51);
                        Params.Add("N_BranchID", nBranchId);
                        CustomerCode = dLayer.GetAutoNumber("Inv_Customer", "X_CustomerCode", Params, connection, transaction);
                             xButtonAction="Insert"; 
                       
                        if (CustomerCode == "") { transaction.Rollback(); return Ok(api.Error(User, "Unable to generate Customer Code")); }
                        MasterTable.Rows[0]["X_CustomerCode"] = CustomerCode;
       
                       SortedList CustNewParams = new SortedList();
                       CustNewParams.Add("@x_CustomerName", x_CustomerName);
                        object CustomerCount = dLayer.ExecuteScalar("select count(N_customerID) from Inv_Customer  Where X_CustomerName =@x_CustomerName and N_CompanyID=" + nCompanyID, CustNewParams, connection,transaction);

                             if( myFunctions.getIntVAL(CustomerCount.ToString())>0)
                                    {
                                           if (customerFlag == 2)
                                    {
                                         showConfirmationCustomer =true;
                                        transaction.Rollback();
                                        return Ok(api.Success(2));
                                    }
                                    }
                                    

                    }else {
                          xButtonAction="Update"; 
                    }
                         CustomerCode = MasterTable.Rows[0]["X_CustomerCode"].ToString();
                       

                    if (!MasterTable.Columns.Contains("b_DirPosting"))
                    {
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "b_DirPosting", typeof(int), 0);
                    }


                    if (isSave== true)
                                    {
                                      nCustomerID = dLayer.SaveData("Inv_Customer", "n_CustomerID", MasterTable, connection, transaction);
                                    }
                                    else
                                 {
                    //string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and X_CustomerCode='" + CustomerCode + "'";
                   string  DupCriteria = "x_CustomerName='" + x_CustomerName.Replace("'", "''") + "' and N_CompanyID=" + nCompanyID+ "and  X_CustomerCode='" + CustomerCode + "'";
                   string  X_Criteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId;
                        nCustomerID = dLayer.SaveData("Inv_Customer", "n_CustomerID", DupCriteria, X_Criteria, MasterTable, connection, transaction);
                        dLayer.ExecuteNonQuery("Update Inv_Customer_Contacts Set N_CustomerID =" + nCustomerID + " Where N_ContactID =" + myFunctions.getIntVAL(MasterTable.Rows[0]["N_ContactID"].ToString()) + " and N_CompanyID=" + nCompanyID, Params, connection, transaction);
                         
                     }
                 
                                   
              
              if(MasterTable.Columns.Contains("N_CrmCompanyID"))
              {
                nCrmCustomerID=myFunctions.getIntVAL(MasterTable.Rows[0]["N_CrmCompanyID"].ToString());
              }
                  
             //if( myFunctions.getIntVAL(MasterTable.Rows[0]["N_CrmCompanyID"].ToString())<=0)
           if( nCrmCustomerID==0)
                {    
                       SortedList customerParams = new SortedList();
                        customerParams.Add("@nCompanyID", nCompanyID);
                        customerParams.Add("@nFnYearId", nFnYearId);
                        customerParams.Add("@nCustomerID", nCustomerID);
                        customerParams.Add("@nFormID", 1306);
                        
                         DataTable CustomerMaster = dLayer.ExecuteDataTable(
                                " select N_CompanyID,N_FnYearId,0 as N_CustomerId,'@Auto' as X_CustomerCode,X_CustomerName as X_Customer,"
                                + "X_PhoneNo1 as X_Phone,X_FaxNo as X_Fax,X_WebSite,D_EntryDate,X_Address,X_Email, N_CurrencyID,X_CustomerName_Ar"
                                + " from Inv_Customer where  N_CustomerID =@nCustomerID and N_CompanyID=@nCompanyID and N_FnYearId = @nFnYearID ", customerParams, connection, transaction);


                             if(CustomerMaster.Rows.Count > 0 )
                             {
                                string X_CrmCustomerCode = "";
                              
                                 while (true)
                                {   SortedList crmParams = new SortedList();
                                     crmParams.Add("N_CompanyID", nCompanyID);
                                     //crmParams.Add("N_FnYearID", nFnYearId);
                                     crmParams.Add("N_FormID", 1306);
                                    X_CrmCustomerCode = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", crmParams, connection, transaction).ToString();
                                    break;
                                }


                                if (X_CrmCustomerCode == "") { transaction.Rollback(); return Ok(api.Error(User, "Unable to generate crm Customer")); }
                                CustomerMaster.Rows[0]["X_CustomerCode"] = X_CrmCustomerCode;

                     
                                int ncrmCustomerID = dLayer.SaveData("CRM_Customer", "N_CustomerId", CustomerMaster, connection, transaction);
                                dLayer.ExecuteNonQuery("Update Inv_Customer Set n_CrmCompanyID =" + ncrmCustomerID + " Where N_CustomerID =" + nCustomerID + " and N_CompanyID=" + nCompanyID + " and N_FnyearID= " + nFnYearId, Params, connection, transaction);
      
                                if (ncrmCustomerID <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(api.Error(User, "Unable to generate CRM Customer entry."));
                                }

                             }

                }

                    if (nCustomerID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                    else
                    {
                        object N_GroupID = dLayer.ExecuteScalar("Select Isnull(N_FieldValue,0) From Acc_AccountDefaults Where N_CompanyID=" + nCompanyID + " and X_FieldDescr ='Customer Account Group' and N_FnYearID=" + nFnYearId, Params, connection, transaction);
                        string X_LedgerName = "";
                        if (b_AutoGenerate)
                        {
                        X_LedgerName = x_CustomerName; 
                       SortedList ledgerParams = new SortedList();
                       ledgerParams.Add("@X_LedgerName", X_LedgerName);
                      


                            if (N_GroupID != null)
                            {
                                object N_LedgerID = dLayer.ExecuteScalar("Select Isnull(N_LedgerID,0) From Acc_MastLedger Where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and X_LedgerName=@X_LedgerName and N_GroupID=" + myFunctions.getIntVAL(N_GroupID.ToString()), ledgerParams, connection, transaction);
                                if (N_LedgerID != null)
                                {
                                    if (flag == 2)//for confirmation of same ledger creattion 
                                    {
                                        showConformationLedger = true;
                                        transaction.Rollback();
                                        return Ok(api.Success(showConformationLedger));
                                    }

                                    if (flag == 1)//for same account for olready exist 
                                    {
                                        dLayer.ExecuteNonQuery("SP_Inv_CreateCustomerAccount " + nCompanyID + "," + nCustomerID + ",'" + CustomerCode + "','" + X_LedgerName + "'," + myFunctions.GetUserID(User) + "," + nFnYearId + "," + "Customer", Params, connection, transaction);
                                    }
                                    else// update ledger id
                                    {
                                        dLayer.ExecuteNonQuery("Update Inv_Customer Set N_LedgerID =" + myFunctions.getIntVAL(N_LedgerID.ToString()) + " Where N_CustomerID =" + nCustomerID + " and N_CompanyID=" + nCompanyID + " and N_FnyearID= " + nFnYearId, Params, connection, transaction);
                                  
                                    }
                                }
                                else
                                {
                               
                                    dLayer.ExecuteNonQuery("SP_Inv_CreateCustomerAccount " + nCompanyID + "," + nCustomerID + ",'" + CustomerCode + "',@X_LedgerName," + myFunctions.GetUserID(User) + "," + nFnYearId + "," + "Customer", ledgerParams, connection, transaction);
                                 
                              
                                }
                            }
                            // else
                            // msg.msgError("No DefaultGroup");
                        }
                        else
                        {
                            if(nLedgerID==0)
                            {
                            object N_DefLedgerID = dLayer.ExecuteScalar("Select Isnull(N_FieldValue,0) From Acc_AccountDefaults Where N_CompanyID=" + nCompanyID + " and X_FieldDescr ='Debtor Account' and N_FnYearID=" + nFnYearId, Params, connection, transaction);
                            dLayer.ExecuteNonQuery("Update Inv_Customer Set N_LedgerID =" + myFunctions.getIntVAL(N_DefLedgerID.ToString()) + " Where N_CustomerID =" + nCustomerID + " and N_CompanyID=" + nCompanyID + " and N_FnyearID= " + nFnYearId, Params, connection, transaction);
                            }
                        }


                        
                              //Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(nFnYearId,nCustomerID,CustomerCode,51,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);
                       



                        int UserID = 0, UserCatID = 0;
                        string Pwd = myFunctions.EncryptString(CustomerCode);
                        if (bEnableLogin == 1)
                        {
                            object objUser = dLayer.ExecuteScalar("Select N_UserID from Sec_User where N_CompanyID=" + nCompanyID + "  and N_CustomerID=" + nCustomerID, Params, connection, transaction);
                            if (objUser != null)
                            {
                                UserID = myFunctions.getIntVAL(objUser.ToString());
                            }
                            else
                            {
                                object objCustUser = dLayer.ExecuteScalar("Select N_UserID from Sec_User where N_CompanyID=" + nCompanyID + " and X_UserID='" + CustomerCode + "'  and N_CustomerID is null", Params, connection, transaction);
                                if (objUser != null)
                                {
                                    UserID = myFunctions.getIntVAL(objCustUser.ToString());
                                }
                            }

                            object objUserCat = dLayer.ExecuteScalar("select N_UserCategoryID from Sec_UserCategory where N_CompanyID=" + nCompanyID + " and N_AppID=13", Params, connection, transaction);
                            if (objUserCat != null)
                            {
                                UserCatID = myFunctions.getIntVAL(objUserCat.ToString());
                            }
                            else
                            {
                                int nUserCat = dLayer.ExecuteNonQuery("insert into Sec_UserCategory SELECT " + nCompanyID + ", MAX(N_UserCategoryID)+1, (select X_UserCategory from Sec_UserCategory where N_CompanyID=-1 and N_AppID=13), MAX(N_UserCategoryID)+1, 12, 13, 3, NULL FROM Sec_UserCategory ", Params, connection, transaction);
                                if (nUserCat <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(api.Warning("User category creation failed"));
                                }
                                object CatID = dLayer.ExecuteScalar("select MAX(N_UserCategoryID) from Sec_UserCategory", Params, connection, transaction);
                                if (CatID != null)
                                {
                                    UserCatID = myFunctions.getIntVAL(CatID.ToString());
                                }
                                if (UserCatID > 0)
                                {
                                    int Prevrows = dLayer.ExecuteNonQuery("Insert into Sec_UserPrevileges (N_InternalID,N_UserCategoryID,N_menuID,B_Visible,B_Edit,B_Delete,B_Save,B_View)" +
                                                                                "Select ROW_NUMBER() over(order by N_InternalID)+(select MAX(N_InternalID) from Sec_UserPrevileges)," + UserCatID + ",N_menuID,B_Visible,B_Edit,B_Delete,B_Save,B_View " +
                                                                                "from Sec_UserPrevileges inner join Sec_UserCategory on Sec_UserPrevileges.N_UserCategoryID = Sec_UserCategory.N_UserCategoryID where Sec_UserPrevileges.N_UserCategoryID = (-13) and N_CompanyID = -1", Params, connection, transaction);
                                    // if (Prevrows <= 0)
                                    // {
                                    //     transaction.Rollback();
                                    //     return Ok(api.Warning("Screen permission failed"));
                                    // } 
                                }
                            }
   

                            if (UserID == 0)
                            {
                                DataTable dt = new DataTable();
                                dt.Clear();
                                dt.Columns.Add("N_CompanyID");
                                dt.Columns.Add("N_UserID");
                                dt.Columns.Add("X_UserID");
                                dt.Columns.Add("X_Password");
                                dt.Columns.Add("N_UserCategoryID");
                                dt.Columns.Add("B_Active");
                                dt.Columns.Add("N_BranchID");
                                dt.Columns.Add("X_UserName");
                                dt.Columns.Add("N_CustomerID");
                                dt.Columns.Add("N_LoginFlag");
                                dt.Columns.Add("X_UserCategoryList");

                                DataRow row = dt.NewRow();
                                row["N_CompanyID"] = nCompanyID;
                                row["X_UserID"] = CustomerCode;
                                row["X_Password"] = Pwd;
                                row["N_UserCategoryID"] = UserCatID;
                                row["B_Active"] = 1;
                                row["N_BranchID"] = myFunctions.getIntVAL(MasterTable.Rows[0]["N_BranchID"].ToString());
                                row["X_UserName"] = MasterTable.Rows[0]["x_CustomerName"].ToString();
                                row["N_CustomerID"] = nCustomerID;
                                row["N_LoginFlag"] = 5;
                                row["X_UserCategoryList"] = UserCatID.ToString();
                                dt.Rows.Add(row);

                                int nUserID = dLayer.SaveData("Sec_User", "N_UserID", dt, connection, transaction);
                            }
                            else
                            {
                                dLayer.ExecuteNonQuery("update Sec_User set N_CustomerID=" + nCustomerID + ",B_Active=1,N_LoginFlag=5 where N_CompanyID=" + nCompanyID + "  and N_UserID=" + UserID, Params, connection, transaction);
                            }
                        }
                        else
                        {
                            object objUser = dLayer.ExecuteScalar("Select N_UserID from Sec_User where N_CompanyID=" + nCompanyID + "  and N_CustomerID=" + nCustomerID, Params, connection, transaction);
                            if (objUser != null)
                            {
                                UserID = myFunctions.getIntVAL(objUser.ToString());
                                dLayer.ExecuteNonQuery("update Sec_User set B_Active=0,N_LoginFlag=5  where N_CompanyID=" + nCompanyID + "  and N_CustomerID=" + nCustomerID, Params, connection, transaction);
                            }
                        }

                        try
                        {
                            myAttachments.SaveAttachment(dLayer, Attachment, MasterTable.Rows[0]["X_CustomerCode"].ToString() + "-" + MasterTable.Rows[0]["X_CustomerName"].ToString(), 0, MasterTable.Rows[0]["X_CustomerName"].ToString(), MasterTable.Rows[0]["X_CustomerCode"].ToString(), nCustomerID, "Customer Document", User, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User, ex));
                        }
                    
                        transaction.Commit();
                        // return GetCustomerList(nCompanyID, nFnYearId, nBranchId, true, nCustomerID.ToString(), "");
                        return Ok(api.Success("Customer Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }

        [HttpGet("customerType")]
        public ActionResult GetPayMethod()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from Inv_CustomerType order by X_TypeName";

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
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("paymentType")]
        public ActionResult getPaymentType(int nFnyearID, int nBranchID , bool inactive)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText="";
            int nCompanyID = myFunctions.GetCompanyID(User);
            if(inactive)
            {
            sqlCommandText = "select N_CompanyID,N_FnYearID,N_BranchID,[Customer Name] as X_CustomerName,[Customer Code] as X_CustomerCode,N_CustomerID,N_ServiceCharge,N_ServiceChargeLimit,N_LedgerID,X_LedgerCode,X_LedgerName,N_TaxCategoryID,X_CategoryName,0 as N_Amount,X_TypeName,N_PaymentMethodID as N_TypeID,I_Image,B_Inactive,B_DefaultPay from vw_PaymentType_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 and (N_BranchID=@p3 or N_BranchID = @p4)  and N_EnablePopup=1";
           
            }
            else
            {
             sqlCommandText = "select N_CompanyID,N_FnYearID,N_BranchID,[Customer Name] as X_CustomerName,[Customer Code] as X_CustomerCode,N_CustomerID,N_ServiceCharge,N_ServiceChargeLimit,N_LedgerID,X_LedgerCode,X_LedgerName,N_TaxCategoryID,X_CategoryName,0 as N_Amount,X_TypeName,N_PaymentMethodID as N_TypeID,I_Image,B_Inactive,B_DefaultPay from vw_PaymentType_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 and (N_BranchID=@p3 or N_BranchID = @p4)  and N_EnablePopup=1 and ISNULL(B_Inactive,0)=0 ";
             
            }
             Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnyearID);
            Params.Add("@p3", nBranchID);
            Params.Add("@p4", 0);
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
                   // return Ok(api.Notice("No Results Found"));
                    return Ok(api.Success(dt));
                }
                else
                {
                    return Ok(api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
        [HttpGet("getdetails")]
        public ActionResult GetCustomerDetails(int nCustomerID, int nCompanyID, int nFnyearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from Inv_Customer where N_CompanyID=@p1 and N_FnYearID=@p2 and N_CustomerID=@p3 ";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnyearID);
            Params.Add("@p3", nCustomerID);

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
                    return Ok(dt);
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCustomerID, int nCompanyID, int nFnYearID,int nCrmCustomerID)
        {

            int Results = 0;
             object CustomerCount,customerPaymntCount,customertxnCount =0;
             object GRNCustCount=0;
              object crmcustomer=0;
            try
            {
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();
                 SortedList ParamList = new SortedList();
                 DataTable TransData = new DataTable();
                 DataTable dt = new DataTable();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nFormID", 51);
                QueryParams.Add("@nCustomerID", nCustomerID);
                 QueryParams.Add("@nCrmCustomerID", nCrmCustomerID);
                   ParamList.Add("@nTransID", nCustomerID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    ParamList.Add("@nCompanyID", nCompanyID);

                  string sqlCommandCount = "";
                    string Sql = "select N_CustomerID,X_CustomerCode from Inv_Customer where N_CustomerID=@nTransID and N_CompanyID=@nCompanyID ";
                    string xButtonAction="Delete";
                     string X_CustomerCode="";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //   dt = dLayer.ExecuteDataTable(sqlCommandText, QueryParams, connection);

                   

                    if (myFunctions.getBoolVAL(myFunctions.checkProcessed("Acc_FnYear", "B_YearEndProcess", "N_FnYearID", "@nFnYearID", "N_CompanyID=@nCompanyID ", QueryParams, dLayer, connection)))
                        return Ok(api.Error(User, "Year is closed, Cannot create new Customer..."));
                    CustomerCount = dLayer.ExecuteScalar("select count(N_customerID) from inv_CustomerProjects  Where N_CompanyID=" + nCompanyID + " and  N_CustomerID=" + nCustomerID,  QueryParams, connection);

                    if( myFunctions.getIntVAL(CustomerCount.ToString())>0)
                   {
                      return Ok(api.Error(User, "Can not Delete Customer"));
                   }

                    customerPaymntCount = dLayer.ExecuteScalar("select count(N_PartyID) from Inv_PayReceipt  Where N_CompanyID=" + nCompanyID + " and  N_PartyID=" + nCustomerID,  QueryParams, connection);

                        if( myFunctions.getIntVAL(customerPaymntCount.ToString())>0)
                    {
                        return Ok(api.Error(User, "Unable to delete Customer! transaction started"));
                    }
                   customertxnCount = dLayer.ExecuteScalar("select count(N_PartyID) from Inv_BalanceAdjustmentMaster  Where N_CompanyID=" + nCompanyID + " and  N_PartyID=" + nCustomerID,  QueryParams, connection);
                        if( myFunctions.getIntVAL(customertxnCount.ToString())>0)
                    {
                        return Ok(api.Error(User, "Unable to delete Customer! It has been used."));
                    }
                    

                  GRNCustCount = dLayer.ExecuteScalar("select count(N_GRNID) from wh_GRN  Where N_CompanyID=" + nCompanyID + " and  N_CustomerID=" + nCustomerID,  QueryParams, connection);
                    if( myFunctions.getIntVAL(GRNCustCount.ToString())>0)
                   {
                      return Ok(api.Error(User, "Unable to delete customer! It has been used."));
                   }

                
                   dLayer.ExecuteNonQuery("update Inv_ItemMaster set N_CustomerID=0,x_CustomerSKU=null where N_CompanyID=" + nCompanyID + "  and N_CustomerID=" + nCustomerID, Params, connection);

                    crmcustomer = dLayer.ExecuteScalar("select count(N_CrmCompanyID) from Inv_SalesQuotation  Where N_CompanyID=" + nCompanyID + " and  N_CrmCompanyID=" + nCrmCustomerID,  QueryParams, connection);

                    SqlTransaction transaction = connection.BeginTransaction();

                      TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection,transaction);
                    
                      if (TransData.Rows.Count == 0)
                    {
                        return Ok(api.Error(User, "Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];
                       //  Activity Log
                string ipAddress = "";
                if (  Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                       myFunctions.LogScreenActivitys(myFunctions.getIntVAL( nFnYearID.ToString()),nCustomerID,TransRow["X_CustomerCode"].ToString(),51,xButtonAction,ipAddress,"",User,dLayer,connection,transaction);



                    if( myFunctions.getIntVAL(crmcustomer.ToString())<=0)
                   {
                     dLayer.DeleteData("CRM_Customer", "N_CustomerID", nCrmCustomerID, "", connection, transaction);
                   }

                   
                  
                    Results = dLayer.DeleteData("Inv_Customer", "N_CustomerID", nCustomerID, "", connection, transaction);
                  
                    myAttachments.DeleteAttachment(dLayer, 1, 0, nCustomerID, nFnYearID, 51, User, transaction, connection);
                    transaction.Commit();
                }


                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("n_CustomerID", nCustomerID.ToString());
                    return Ok(api.Success("Customer deleted"));
                }
                else
                {
                    return Ok(api.Error(User, "Unable to delete Customer"));
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("REFERENCE constraint"))
                    return Ok(api.Error(User, "Unable to delete customer! It has been used."));
                else
                    return Ok(api.Error(User, ex));
            }



        }

        [HttpGet("details")]
        public ActionResult GetCustomerDetail(int nCustomerID, int crmcustomerID, int nFnYearID)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            object customerCount = null;
            object customerCounts = null;
            object customerCountSQ = null;
            object customerCountSO = null;
            object customerCountDN = null;

            if (crmcustomerID > 0)
            {
                sqlCommandText = "select   X_Customer as X_CustomerName,X_Phone as X_PhoneNo1,X_Address,* from vw_CRMCustomer where N_CompanyID=@nCompanyID and N_CustomerID=" + crmcustomerID + "";
            }
            else
            {
                sqlCommandText = "select * from  vw_InvCustomer  where N_CompanyID=@nCompanyID and N_CustomerID=@nCustomerID and N_FnYearID=@nFnYearID";
                
            Params.Add("@nCustomerID", nCustomerID);
            }
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "portalURL", typeof(string), "");
                    if(nCustomerID>0){
                    object Count = dLayer.ExecuteScalar("select count(1)  from vw_Inv_CheckCustomer where N_CompanyID=@nCompanyID and N_CustomerID=@nCustomerID", Params, connection);
                    int NCount = myFunctions.getIntVAL(Count.ToString());
                    if (NCount > 0)
                    {
                       
                        if (dt.Rows.Count > 0)
                        {
                            dt.Columns.Add("b_isUsed");
                            dt.Rows[0]["b_isUsed"]=true;
                            
                          
                        }


                    }
                    }

                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        if (crmcustomerID > 0)
                        {
                            object res = dLayer.ExecuteScalar("select x_CustomerCode from CRM_Customer where N_CompanyID=@nCompanyID and N_CustomerID="+crmcustomerID+" ", Params, connection);
                            dt.Rows[0]["x_CustomerCode"] = res;

                        }
                        else
                        {
                            string seperator = "$$";
                            // dt.Rows[0]["customerKey"] = myFunctions.EncryptString(myFunctions.GetCompanyID(User).ToString()) + seperator + myFunctions.EncryptString(dt.Rows[0]["n_CustomerID"].ToString());

                            string xURL = myFunctions.EncryptStringForUrl(myFunctions.GetCompanyID(User).ToString() + seperator + dt.Rows[0]["n_CustomerID"].ToString() + seperator + "HOME" + seperator + "0", System.Text.Encoding.Unicode);
                                   xURL = AppURL + "/client/customer/13/" + xURL + "/home/new";
                                   dt.Rows[0]["portalURL"] = xURL;
                        }
                        dt.AcceptChanges();

                        // myFunctions.AddNewColumnToDataTable(dt, "b_CustomerCount", typeof(bool), false);
                          if(nCustomerID>0)
                          {
                         customerCount = dLayer.ExecuteScalar("select Isnull(Count(N_SalesId),0) from inv_sales where N_CustomerID=@nCustomerID and N_CompanyID=@nCompanyID", Params, connection);
                         customerCounts = dLayer.ExecuteScalar("select Isnull(Count(N_PayReceiptId),0) from Inv_PayReceipt where N_PartyID=@nCustomerID and N_CompanyID=@nCompanyID", Params, connection);
                         customerCountSQ = dLayer.ExecuteScalar("select Isnull(Count(N_QuotationId),0) from Inv_SalesQuotation where N_CustomerId=@nCustomerID and N_CompanyID=@nCompanyID", Params, connection);
                         customerCountSO = dLayer.ExecuteScalar("select Isnull(Count(N_SalesOrderID),0) from Inv_SalesOrder where N_CustomerId=@nCustomerID and N_CompanyID=@nCompanyID", Params, connection);
                         customerCountDN = dLayer.ExecuteScalar("select Isnull(Count(N_DeliveryNoteId),0) from Inv_DeliveryNote where N_CustomerId=@nCustomerID and N_CompanyID=@nCompanyID", Params, connection);

                         if (myFunctions.getIntVAL(customerCount.ToString()) > 0 || myFunctions.getIntVAL(customerCounts.ToString()) > 0 || myFunctions.getIntVAL(customerCountSQ.ToString()) > 0 || myFunctions.getIntVAL(customerCountSO.ToString()) > 0 || myFunctions.getIntVAL(customerCountDN.ToString()) > 0 )
                         {
                                myFunctions.AddNewColumnToDataTable(dt, "b_CustomerCount", typeof(bool), true);
                         }
                          DataTable Attachments = myAttachments.ViewAttachment(dLayer, nCustomerID, 0, 51, nFnYearID, User, connection);
                          Attachments = api.Format(Attachments, "attachments");
                          ds.Tables.Add(Attachments);


                          }
                        //  myFunctions.AddNewColumnToDataTable(dt, "b_CustomerCount", typeof(bool), customerCount);
                         dt.AcceptChanges();

                        if(crmcustomerID>0)
                        {
                            object crmCustomerCountSQ = dLayer.ExecuteScalar("select Isnull(Count(N_QuotationId),0) from Inv_SalesQuotation where N_CrmCompanyID="+crmcustomerID+" and N_CompanyID=@p1", Params, connection);
                            if ( myFunctions.getIntVAL(crmCustomerCountSQ.ToString()) > 0 )
                            {
                                myFunctions.AddNewColumnToDataTable(dt, "b_CustomerCount", typeof(bool), true);
                            }
                        }
                        dt.AcceptChanges();

                        dt = api.Format(dt, "master");
                        ds.Tables.Add(dt);
                    
                        return Ok(api.Success(ds));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
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
                    DataTable QList = myFunctions.GetSettingsTable();
                    QList.Rows.Add("DEFAULT_ACCOUNTS", "Debtor Account");
                    QList.Rows.Add("DEFAULT_ACCOUNTS", "S Cash Account");

                    QList.AcceptChanges();

                    DataTable Details = dLayer.ExecuteSettingsPro("SP_GenSettings_Disp", QList, myFunctions.GetCompanyID(User), nFnYearID, connection);

                    SortedList OutPut = new SortedList(){
                            {"settings",api.Format(Details)}
                        };
                    return Ok(api.Success(OutPut));
                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        } 

        [HttpGet("totalInvoiceAmount")]
        public ActionResult GetCustomerDetail(int nCustomerID, int nFnYearID,bool isQuotation,int nSaleOrderID, int nsalesQuotationID )
        {
            DataTable dt = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommmand = "";
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
     
            Params.Add("@nFnYearID", nFnYearID);
            sqlCommmand = "select sum(Cast(REPLACE(x_BillAmt,',','') as Numeric(10,2)) ) as TotalInvoiceAmount from vw_InvSalesInvoiceNo_Search where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID="+nCustomerID+"";
            SortedList OutPut = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommmand, Params, connection);
                    
                    //checking Quotaton--crm Customer 
                    object customerID=null;
                    object creditLimit=null;
                    if(isQuotation)
                    {
                        
                       
                         customerID = dLayer.ExecuteScalar("select N_CustomerID from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CrmCompanyID="+nCustomerID+" ", Params, connection);
                         if(customerID!=null){
                         nCustomerID=myFunctions.getIntVAL(customerID.ToString());
                         creditLimit = dLayer.ExecuteScalar("select N_CreditLimit from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID="+nCustomerID+" ", Params, connection);
                         }
                        if(creditLimit==null)
                             dt = myFunctions.AddNewColumnToDataTable(dt, "n_CreditLimit",typeof(double), 0.00);
                        else 
                          dt = myFunctions.AddNewColumnToDataTable(dt, "n_CreditLimit",typeof(double), myFunctions.getVAL(creditLimit.ToString()));
                        dt.AcceptChanges();
                    }
                         
                    if(isQuotation && (customerID==null || myFunctions.getIntVAL(customerID.ToString())==0 ))
                    {
                            dt = myFunctions.AddNewColumnToDataTable(dt, "isDirectQuotationEnabled",typeof(bool), true);
                              dt.AcceptChanges();
                    }

                   double quotationAmt=0.0;

                   if(!isQuotation || (customerID!=null && myFunctions.getIntVAL(customerID.ToString())>0))
                   {
                    //crm customer from Customer for SQ 

                    object sqCustomerID= dLayer.ExecuteScalar("select N_CrmCompanyID from Inv_Customer where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID="+nCustomerID+" ", Params, connection);
                    
                    if(sqCustomerID==null)
                    sqCustomerID=0;

                    if(myFunctions.getIntVAL(sqCustomerID.ToString())>0 && isQuotation )
                    {

                        object pendingSQAmt=dLayer.ExecuteScalar("select sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as pendingSQAmt from VW_PendingSalesQuotation where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CrmCompanyID="+myFunctions.getIntVAL(sqCustomerID.ToString())+" and N_QuotationID not in ("+nsalesQuotationID+")", Params, connection);
                        if(pendingSQAmt!=null)
                           quotationAmt=myFunctions.getVAL(pendingSQAmt.ToString());
                    }
                    Params.Add("@nCustomerID", nCustomerID);
                    object invoiceamt = dLayer.ExecuteScalar("select sum(Cast(REPLACE(N_BalanceAmount,',','') as Numeric(10,2)) ) as TotalInvoiceAmount from Vw_InvReceivables where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID AND X_Type='SALES'", Params, connection);
                    object pendingSOAmt=dLayer.ExecuteScalar("select sum(Cast(REPLACE(N_Amount,',','') as Numeric(10,2)) ) as pendingSOAmt from vw_pendingSO where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID and N_SalesOrderID Not in ("+nSaleOrderID+") ", Params, connection);
                    //object paidAmount =dLayer.ExecuteScalar("select sum(Cast(REPLACE(Amount,',','') as Numeric(10,2)) ) as PaidAmount from vw_InvReceipt_Search where N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and (X_type='SR') ", Params, connection);
                    object returnamt = dLayer.ExecuteScalar("select sum(Cast(REPLACE(N_TotalPaidAmount,',','') as Numeric(10,2)) ) as TotalReturnAmount from vw_InvDebitNo_Search where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID", Params, connection);
                    double  currentBalance=  myFunctions.getVAL(dLayer.ExecuteScalar("SELECT  Sum(n_Amount)  as N_BalanceAmount from  vw_InvCustomerStatement Where N_AccType=2 and N_AccID=" + nCustomerID + " and N_CompanyID=" + nCompanyID,Params,connection).ToString());
                    object invoiceDate=dLayer.ExecuteScalar("select Min(D_SalesDate) from Vw_SalesInvoice_DateSearch  where N_CompanyID=@nCompanyID  and N_CustomerID=@nCustomerID and N_PaymentMethodID<>1 and N_SalesID NOt in (Select N_SalesID from Vw_InvReceivables where N_CompanyID="+nCompanyID+"  and  N_CustomerID="+nCustomerID+" and N_BalanceAmount=0 )", Params, connection);
                  
                  
                    {
                        returnamt = "0";
                    }
                    if (invoiceamt == null)
                    {
                        invoiceamt = "0";
                    }
                    double amount = (myFunctions.getVAL(invoiceamt.ToString()) + myFunctions.getVAL(pendingSOAmt.ToString()) + quotationAmt) - myFunctions.getVAL(returnamt.ToString());
                    dt.Rows[0]["TotalInvoiceAmount"] = amount.ToString();
                    dt = myFunctions.AddNewColumnToDataTable(dt, "currentBalance", typeof(double), currentBalance);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "d_invDate",typeof(string), invoiceDate);
                    dt.AcceptChanges();   
                   }
                    
                    
                    return Ok(api.Success(dt));

                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

}
}