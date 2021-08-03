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

        public Inv_Customer(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
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
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
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

        [HttpGet("dashboardList")]
        public ActionResult GetDashboardList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
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
                xSortBy = " order by N_CustomerID desc";
            else
            {
                 switch (xSortBy.Split(" ")[0])
                        {
                            case "x_CustomerCode":
                                xSortBy = "N_CustomerID " + xSortBy.Split(" ")[1];
                                break;
                           
                            default: break;
                        }
             xSortBy = " order by " + xSortBy;
            }
              
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") N_CustomerID,X_CustomerCode,X_CustomerName,N_CountryID,X_Country,N_TypeID,X_TypeName,N_BranchID,X_BranchName,X_ContactName,X_Address,X_PhoneNo1 from vw_InvCustomer where N_CompanyID=@p1 and B_Inactive=@p2 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_CustomerID,X_CustomerCode,X_CustomerName,N_CountryID,X_Country,N_TypeID,X_TypeName,N_BranchID,X_BranchName,X_ContactName,X_Address,X_PhoneNo1 from vw_InvCustomer where N_CompanyID=@p1 and B_Inactive=@p2 " + Searchkey + " and N_CustomerID not in (select top(" + Count + ") N_CustomerID from vw_InvCustomer where N_CompanyID=@p1 and B_Inactive=@p2 " + Searchkey + xSortBy + " ) " + xSortBy;

            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", 0);

            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    string sqlCommandCount = "select count(*) as N_Count  from vw_InvCustomer where N_CompanyID=@p1 and B_Inactive=@p2 " + Searchkey + "";
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
                return Ok(api.Error(e));
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
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nBranchId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchId"].ToString());
                int nCustomerID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CustomerId"].ToString());
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
                        if (CustomerCode == "") { transaction.Rollback(); return Ok(api.Error("Unable to generate Customer Code")); }
                        MasterTable.Rows[0]["X_CustomerCode"] = CustomerCode;
                    }

                    
                    if(MasterTable.Columns.Contains("b_DirPosting")){
                        MasterTable.Rows[0]["b_DirPosting"] = 0;
                    }else{
                       MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable,"b_DirPosting",typeof(int),0); 
                    }
                    
                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and X_CustomerCode='" + CustomerCode + "'";
                    string X_Criteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId;
                    nCustomerID = dLayer.SaveData("Inv_Customer", "n_CustomerID", DupCriteria,X_Criteria,MasterTable, connection, transaction);
                    if (nCustomerID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        // return GetCustomerList(nCompanyID, nFnYearId, nBranchId, true, nCustomerID.ToString(), "");
                        return Ok(api.Success("Customer Saved") );
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
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
                return Ok(api.Error(e));
            }
        }

        [HttpGet("paymentType")]
        public ActionResult getPaymentType(int nFnyearID,int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select N_CompanyID,N_FnYearID,N_BranchID,[Customer Name] as X_CustomerName,[Customer Code] as X_CustomerCode,N_CustomerID,N_ServiceCharge,N_ServiceChargeLimit,N_LedgerID,X_LedgerCode,X_LedgerName,N_TaxCategoryID,X_CategoryName,0 as N_Amount,X_TypeName,N_PaymentMethodID as N_TypeID,I_Image  from vw_PaymentType_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 and (N_BranchID=@p3 or N_BranchID = @p4)  and N_EnablePopup=1 ";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnyearID);
            Params.Add("@p3", nBranchID);
            Params.Add("@p4",0);
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
                return Ok(api.Error(e));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCustomerID,int nCompanyID, int nFnYearID)
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
                        return Ok(api.Error("Year is closed, Cannot create new Customer..."));
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Inv_Customer", "N_CustomerID", nCustomerID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("n_CustomerID",nCustomerID.ToString());
                    return Ok(api.Success("Customer deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete Customer"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }



        }

        [HttpGet("details")]
        public ActionResult GetCustomerDetails(int nCustomerID,int crmcustomerID)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
               string sqlCommandText = "";
            if (crmcustomerID > 0)
            {
                sqlCommandText = "select  X_Customer as X_CustomerName,X_Website as X_WebSite,* from vw_CRMCustomer where N_CompanyID=@nCompanyID and N_CustomerID=" + crmcustomerID + "";
            }
            else
            {
             sqlCommandText="select * from vw_InvCustomer where N_CompanyID=@nCompanyID and N_CustomerID=@nCustomerID";
            }
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nCustomerID",nCustomerID);
            try{
                using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection); 
                    }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(api.Notice("No Results Found" ));
                        }else{
                            return Ok(api.Success(dt));
                        }
            }catch(Exception e){
                return Ok(api.Error(e));
            }
          
        }


         [HttpGet("default")]
        public ActionResult GetDefault(int nFnYearID,int nLangID,int nFormID)
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

                    DataTable Details = dLayer.ExecuteSettingsPro("SP_GenSettings_Disp", QList, myFunctions.GetCompanyID(User),nFnYearID, connection);

                        SortedList OutPut = new SortedList(){
                            {"settings",api.Format(Details)}
                        };
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