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

        public Inv_Customer(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            myAttachments = myAtt;
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
                qryCriteria = " and (X_CustomerCode like @qry or X_CustomerName like @qry ) ";
                Params.Add("@qry", "%" + qry + "%");
            }

            string X_Crieteria = "";
            if (bAllBranchesData == true)
            { X_Crieteria = " where B_Inactive=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3"; }
            else
            {
                X_Crieteria = " where B_Inactive=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3  and (N_BranchID=@p4 or N_BranchID=@p5)";
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
        public ActionResult GetDashboardList(int nFnYearId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_CustomerName like '%" + xSearchkey + "%' or X_CustomerCode like '%" + xSearchkey + "%' or X_ContactName like '%" + xSearchkey + "%' or X_Address like '%" + xSearchkey + "%' or X_PhoneNo1 like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_CustomerCode desc";
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

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") N_CustomerID,X_CustomerCode,X_CustomerName,N_CountryID,X_Country,N_TypeID,X_TypeName,N_BranchID,X_BranchName,X_ContactName,X_Address,X_PhoneNo1 from vw_InvCustomer where N_CompanyID=@p1 and B_Inactive=@p2 and N_FnYearId=@p3 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_CustomerID,X_CustomerCode,X_CustomerName,N_CountryID,X_Country,N_TypeID,X_TypeName,N_BranchID,X_BranchName,X_ContactName,X_Address,X_PhoneNo1 from vw_InvCustomer where N_CompanyID=@p1 and B_Inactive=@p2 and N_FnYearId=@p3 " + Searchkey + " and N_CustomerID not in (select top(" + Count + ") N_CustomerID from vw_InvCustomer where N_CompanyID=@p1 and B_Inactive=@p2 " + Searchkey + xSortBy + " ) " + xSortBy;

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

                    string sqlCommandCount = "select count(*) as N_Count  from vw_InvCustomer where N_CompanyID=@p1 and B_Inactive=@p2 and N_FnYearId=@p3 " + Searchkey + "";
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
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nBranchId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchId"].ToString());
                int nCustomerID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CustomerId"].ToString());
                int flag=0;

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
                    string CustomerCode = "";
                    CustomerCode = MasterTable.Rows[0]["X_CustomerCode"].ToString();
                    if (CustomerCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", 51);
                        Params.Add("N_BranchID", nBranchId);
                        CustomerCode = dLayer.GetAutoNumber("Inv_Customer", "X_CustomerCode", Params, connection, transaction);
                        if (CustomerCode == "") { transaction.Rollback(); return Ok(api.Error(User, "Unable to generate Customer Code")); }
                        MasterTable.Rows[0]["X_CustomerCode"] = CustomerCode;
                    }



                    if (!MasterTable.Columns.Contains("b_DirPosting"))
                    {
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "b_DirPosting", typeof(int), 0);
                    }

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and X_CustomerCode='" + CustomerCode + "'";
                    string X_Criteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId;
                    nCustomerID = dLayer.SaveData("Inv_Customer", "n_CustomerID", DupCriteria, X_Criteria, MasterTable, connection, transaction);
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
                            if (N_GroupID != null)
                            {
                                object N_LedgerID = dLayer.ExecuteScalar("Select Isnull(N_LedgerID,0) From Acc_MastLedger Where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and X_LedgerName='" + X_LedgerName + "' and N_GroupID=" + myFunctions.getIntVAL(N_GroupID.ToString()), Params, connection, transaction);
                                if (N_LedgerID != null)
                                {
                                    if (flag == 2)//for confirmation of same ledger creattion 
                                    {
                                        showConformationLedger = true;
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
                                    dLayer.ExecuteNonQuery("SP_Inv_CreateCustomerAccount " + nCompanyID + "," + nCustomerID + ",'" + CustomerCode + "','" + X_LedgerName + "'," + myFunctions.GetUserID(User) + "," + nFnYearId + "," + "Customer", Params, connection, transaction);
                                }
                            }
                            // else
                            // msg.msgError("No DefaultGroup");
                        }



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

                            object objUserCat = dLayer.ExecuteScalar("select N_UserCategoryID from Sec_UserCategory where N_CompanyID=" + nCompanyID + " and N_AppID=10", Params, connection, transaction);
                            if (objUserCat != null)
                            {
                                UserCatID = myFunctions.getIntVAL(objUserCat.ToString());
                            }
                            else
                            {
                                int nUserCat = dLayer.ExecuteNonQuery("insert into Sec_UserCategory SELECT " + nCompanyID + ", MAX(N_UserCategoryID)+1, (select X_UserCategory from Sec_UserCategory where N_CompanyID=-1 and N_AppID=10), MAX(N_UserCategoryID)+1, 12, 10 FROM Sec_UserCategory ", Params, connection, transaction);
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
                                                                                "from Sec_UserPrevileges inner join Sec_UserCategory on Sec_UserPrevileges.N_UserCategoryID = Sec_UserCategory.N_UserCategoryID where Sec_UserPrevileges.N_UserCategoryID = (-10) and N_CompanyID = -1", Params, connection, transaction);
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
        public ActionResult getPaymentType(int nFnyearID, int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select N_CompanyID,N_FnYearID,N_BranchID,[Customer Name] as X_CustomerName,[Customer Code] as X_CustomerCode,N_CustomerID,N_ServiceCharge,N_ServiceChargeLimit,N_LedgerID,X_LedgerCode,X_LedgerName,N_TaxCategoryID,X_CategoryName,0 as N_Amount,X_TypeName,N_PaymentMethodID as N_TypeID,I_Image  from vw_PaymentType_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 and (N_BranchID=@p3 or N_BranchID = @p4)  and N_EnablePopup=1 ";
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
                    return Ok(api.Notice("No Results Found"));
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
        public ActionResult DeleteData(int nCustomerID, int nCompanyID, int nFnYearID)
        {

            int Results = 0;
            try
            {
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nFormID", 51);
                QueryParams.Add("@nCustomerID", nCustomerID);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (myFunctions.getBoolVAL(myFunctions.checkProcessed("Acc_FnYear", "B_YearEndProcess", "N_FnYearID", "@nFnYearID", "N_CompanyID=@nCompanyID ", QueryParams, dLayer, connection)))
                        return Ok(api.Error(User, "Year is closed, Cannot create new Customer..."));
                    SqlTransaction transaction = connection.BeginTransaction();
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
            if (crmcustomerID > 0)
            {
                sqlCommandText = "select   X_Customer as X_CustomerName,X_Phone as X_PhoneNo1,* from vw_CRMCustomer where N_CompanyID=@nCompanyID and N_CustomerID=" + crmcustomerID + "";
            }
            else
            {
                sqlCommandText = "select * from  vw_InvCustomer  where N_CompanyID=@nCompanyID and N_CustomerID=@nCustomerID";
            }
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nCustomerID", nCustomerID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt = myFunctions.AddNewColumnToDataTable(dt, "customerKey", typeof(string), "");
                    if (crmcustomerID > 0)
                    {
                        dt.Rows[0]["x_CustomerCode"] = "@Auto";

                    }
                    else
                    {
                        string seperator = "$e$-!";
                        dt.Rows[0]["customerKey"] = myFunctions.EncryptString(myFunctions.GetCompanyID(User).ToString()) + seperator + myFunctions.EncryptString(dt.Rows[0]["n_CustomerID"].ToString());
                    }
                    dt.AcceptChanges();

                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        DataTable Attachments = myAttachments.ViewAttachment(dLayer, nCustomerID, 0, 51, nFnYearID, User, connection);
                        Attachments = api.Format(Attachments, "attachments");
                        dt = api.Format(dt, "master");
                        ds.Tables.Add(dt);
                        ds.Tables.Add(Attachments);

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
        public ActionResult GetCustomerDetail(int nCustomerID, int nFnYearID)
        {
            DataTable dt = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommmand = "";
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nCustomerID", nCustomerID);
            Params.Add("@nFnYearID", nFnYearID);
            sqlCommmand = "select sum(Cast(REPLACE(x_BillAmt,',','') as Numeric(10,2)) ) as TotalInvoiceAmount from vw_InvSalesInvoiceNo_Search where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID";
            SortedList OutPut = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommmand, Params, connection);
                    object invoiceamt = dLayer.ExecuteScalar("select sum(Cast(REPLACE(x_BillAmt,',','') as Numeric(10,2)) ) as TotalInvoiceAmount from vw_InvSalesInvoiceNo_Search where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID", Params, connection);
                    object returnamt = dLayer.ExecuteScalar("select sum(Cast(REPLACE(N_TotalPaidAmount,',','') as Numeric(10,2)) ) as TotalReturnAmount from vw_InvDebitNo_Search where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and N_CustomerID=@nCustomerID", Params, connection);
                    if (returnamt.ToString() == "")
                    {
                        returnamt = "0";
                    }
                    if (invoiceamt == null)
                    {
                        invoiceamt = "0";
                    }
                    double amount = myFunctions.getVAL(invoiceamt.ToString()) - myFunctions.getVAL(returnamt.ToString());
                    dt.Rows[0]["TotalInvoiceAmount"] = amount.ToString();
                    dt.AcceptChanges();
                    

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