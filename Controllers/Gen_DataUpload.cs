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
    [Route("dataupload")]
    [ApiController]
    public class Gen_DataUpload : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        public Gen_DataUpload(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable Mastertable = new DataTable();
                DataTable Generaltable = new DataTable();
                DataTable MappingRule = new DataTable();
                Generaltable = ds.Tables["general"];
                int nCompanyID = myFunctions.GetCompanyID(User);
                int nMasterID = 0;
                string xTableName = "";
                SortedList Params = new SortedList();
                Params.Add("N_CompanyID", nCompanyID);
                Params.Add("N_FnYearID", myFunctions.getIntVAL(Generaltable.Rows[0]["N_FnYearID"].ToString()));
                Params.Add("N_BranchID", myFunctions.getIntVAL(Generaltable.Rows[0]["N_BranchID"].ToString()));
                Params.Add("N_LocationID", myFunctions.getIntVAL(Generaltable.Rows[0]["N_LocationID"].ToString()));
                Params.Add("X_Type", "");
                int N_UserID = myFunctions.GetUserID(User);
                bool isRuleBasedImport = false;

                if (Generaltable.Columns.Contains("N_RuleID")) //Checking if it's rule bases import
                    isRuleBasedImport = true;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt.Columns.Contains("notes") && !isRuleBasedImport)
                            dt.Columns.Remove("notes");

                        Params["X_Type"] = dt.TableName;
                        Mastertable = ds.Tables[dt.TableName];
                        if (!isRuleBasedImport)
                        {
                            foreach (DataColumn col in Mastertable.Columns)
                            {
                                col.ColumnName = col.ColumnName.Replace(" ", "_");
                                col.ColumnName = col.ColumnName.Replace("*", "");
                                col.ColumnName = col.ColumnName.Replace("/", "_");
                            }
                        }
                        Mastertable.Columns.Add("Pkey_Code");



                        switch (dt.TableName.ToString().ToLower())
                        {
                            case "customer list":
                            case "customers":
                            case "customer":
                                xTableName = "Mig_Customers";
                                break;
                            case "vendor list":
                            case "vendors":
                            case "vendor":
                                xTableName = "Mig_Vendors";
                                break;
                            case "lead list":
                                xTableName = "Mig_Leads";
                                Mastertable.Columns.Add("N_UserID");
                                foreach (DataRow dtRow in Mastertable.Rows)
                                {
                                    dtRow["N_UserID"] = N_UserID;
                                }
                                break;
                            case "chart of accounts":
                                xTableName = "Mig_Accounts";

                                break;
                            case "products stock":
                                xTableName = "Mig_Stock";
                                break;
                            case "employee list":
                            case "employees":
                                xTableName = "Mig_Employee";
                                break;
                            case "product transfer":
                                xTableName = "Mig_ProductTransfer";
                                break;
                           case "fixedassets list":
                                xTableName = "Mig_AssetList";
                                Mastertable.Columns.Add("N_CompanyID");
                                foreach (DataRow dtRow in Mastertable.Rows)
                                {
                                    dtRow["N_CompanyID"] = nCompanyID;
                                }
                                break;
                            case "salary history":
                                xTableName = "Mig_EmployeeSalaryHistory";
                                Mastertable.Columns.Add("N_CompanyID");
                                foreach (DataRow dtRow in Mastertable.Rows)
                                {
                                    dtRow["N_CompanyID"] = nCompanyID;
                                }
                                break;
                            case "employee salary":
                                xTableName = "Mig_EmployeeSalary";
                                break;
                            case "leave history":
                                xTableName = "Mig_EmployeeLeaveHistory";
                                break;
                         case "student balances":
                         case  "customer balances":
                                xTableName = "Mig_CustomerOpening";
                                Mastertable.Columns.Add("N_CompanyID");
                                foreach (DataRow dtRow in Mastertable.Rows)
                                {
                                    dtRow["N_CompanyID"] = nCompanyID;
                                }
                                break;
                            case "leave adjustment":
                                xTableName = "Mig_LeaveAdjustment";
                                break;
                            case "recruitment":
                                xTableName = "Mig_Recruitment";
                                break;
                            case "vendor balances":
                                xTableName = "Mig_VendorOpening";
                                break;
                            case "product list":
                            case "products":
                            case "customer materials":
                                xTableName = "Mig_Items";
                                Mastertable.Columns.Add("N_CompanyID");
                                foreach (DataRow dtRow in Mastertable.Rows)
                                {
                                    dtRow["N_CompanyID"] = nCompanyID;
                                }
                                break;
                            case "payment voucher":
                            case "receipt voucher":   
                            case "journel voucher":
                                xTableName = "Mig_Vouchers";
                                Mastertable.Columns.Add("N_CompanyID");
                                foreach (DataRow dtRow in Mastertable.Rows)
                                {
                                    dtRow["N_CompanyID"] = nCompanyID;
                                }
                                break;
                            case "category":
                                xTableName = "Mig_POSCategory";
                                Mastertable.Columns.Add("N_CompanyID");
                                foreach (DataRow dtRow in Mastertable.Rows)
                                {
                                    dtRow["N_CompanyID"] = nCompanyID;
                                }
                                break;
                            case "delivery notes":
                                xTableName = "Mig_Deliverynote";
                                Mastertable.Columns.Add("N_CompanyID");
                                foreach (DataRow dtRow in Mastertable.Rows)
                                {
                                    dtRow["N_CompanyID"] = nCompanyID;
                                }
                                break;
                            case "goods received note":
                                xTableName = "Mig_Grn";
                                Mastertable.Columns.Add("N_CompanyID");
                                foreach (DataRow dtRow in Mastertable.Rows)
                                {
                                    dtRow["N_CompanyID"] = nCompanyID;
                                }
                                break;
                            case "package items":
                                xTableName = "Mig_PackageItem";
                                Mastertable.Columns.Add("N_CompanyID");
                                foreach (DataRow dtRow in Mastertable.Rows)
                                {
                                    dtRow["N_CompanyID"] = nCompanyID;
                                }
                                break;
                            case "warranty items":
                                xTableName = "Mig_WarrantyItem";
                                Mastertable.Columns.Add("N_CompanyID");
                                foreach (DataRow dtRow in Mastertable.Rows)
                                {
                                    dtRow["N_CompanyID"] = nCompanyID;
                                }
                                break;
                            case "students":
                                xTableName = "Mig_Students";
                                Mastertable.Columns.Add("N_CompanyID");
                                foreach (DataRow dtRow in Mastertable.Rows)
                                {
                                    dtRow["N_CompanyID"] = nCompanyID;
                                }
                                break;
                            case "sales invoice":
                                xTableName = "Mig_SalesInvoice";
                                Mastertable.Columns.Add("N_CompanyID");
                                foreach (DataRow dtRow in Mastertable.Rows)
                                {
                                    dtRow["N_CompanyID"] = nCompanyID;
                                }
                                break;
                            case "purchase invoice":
                                xTableName = "Mig_PurchaseInvoice";
                                Mastertable.Columns.Add("N_CompanyID");
                                foreach (DataRow dtRow in Mastertable.Rows)
                                {
                                    dtRow["N_CompanyID"] = nCompanyID;
                                }
                                break;
                            default: return Ok("Invalid File");
                        }

                        // Mapping Rule Configuration
                        if (isRuleBasedImport)
                        {
                            int RuleID = myFunctions.getIntVAL(Generaltable.Rows[0]["n_RuleID"].ToString());
                            SortedList ruleParams = new SortedList();
                            ruleParams.Add("@nCompanyID", nCompanyID);
                            ruleParams.Add("@nRuleID", RuleID);
                            MappingRule = dLayer.ExecuteDataTable("select X_FieldName,X_ColumnRefName from Gen_ImportRuleDetails where N_CompanyID=@nCompanyID and N_RuleID=@nRuleID", ruleParams, connection, transaction);
                        }

                        if (Mastertable.Rows.Count > 0)
                        {

                            if (Mastertable.Columns.Contains("Notes") && !isRuleBasedImport)
                                Mastertable.Columns.Remove("Notes");

                            dLayer.ExecuteNonQuery("delete from " + xTableName, Params, connection, transaction);


                            string FieldList = "";
                            string FieldValuesArray = "";
                            string IDFieldName = "PKey_Code";
                            int rowCount = 0;
                            int totalCount = 0;
                            for (int j = 0; j < Mastertable.Rows.Count; j++)
                            {
                                rowCount = rowCount + 1;
                                string FieldValues = "";
                                for (int k = 0; k < Mastertable.Columns.Count; k++)
                                {
                                    var values = "";
                                    if (Mastertable.Columns[k].ColumnName.ToString().ToLower() == IDFieldName.ToLower())
                                    {
                                        values = (j + 1).ToString();
                                    }
                                    else
                                    {
                                        if (Mastertable.Rows[j][k] == DBNull.Value)
                                        {
                                            values = "";

                                        }
                                        else
                                        {
                                            values = Mastertable.Rows[j][k].ToString();
                                        }
                                        values = values.Replace("|", " ");
                                    }


                                    if (isRuleBasedImport) // In Case of Mapping Rule
                                    {
                                        DataRow[] RuleRow = MappingRule.Select("X_ColumnRefName = '" + Mastertable.Columns[k].ColumnName.ToString() + "'");
                                        if (RuleRow.Length > 0)
                                        {
                                            FieldValues = FieldValues + "|" + values;
                                            if (j == 0)
                                                FieldList = FieldList + "," + RuleRow[0]["X_FieldName"].ToString().Replace(" ", "_").Replace("*", "").Replace("/", "_");
                                        }
                                        else if (Mastertable.Columns[k].ColumnName.ToString() == "N_CompanyID" || Mastertable.Columns[k].ColumnName.ToString() == "PKey_Code")//System Defined fields exception
                                        {
                                            FieldValues = FieldValues + "|" + values;
                                            if (j == 0)
                                                FieldList = FieldList + "," + Mastertable.Columns[k].ColumnName.ToString();
                                        }
                                    }
                                    else
                                    {
                                        FieldValues = FieldValues + "|" + values;
                                        if (j == 0)
                                            FieldList = FieldList + "," + Mastertable.Columns[k].ColumnName.ToString();
                                    }



                                }

                                if (j == 0)
                                {
                                    FieldList = FieldList.Substring(1);
                                }

                                FieldValues = ValidateString(FieldValues.Substring(1));

                                FieldValuesArray = FieldValuesArray + ",(" + FieldValues + ")";
                                if (rowCount == 1000 || Mastertable.Rows.Count == (totalCount + rowCount))
                                {
                                    totalCount = totalCount + rowCount;
                                    FieldValuesArray = FieldValuesArray.Substring(1);
                                    string inserStript = "insert into " + xTableName + " (" + FieldList + ") values" + FieldValuesArray;
                                    dLayer.ExecuteNonQuery(inserStript, connection, transaction);
                                    FieldValuesArray = "";
                                    rowCount = 0;
                                }

                            }

                            nMasterID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Count(1) from " + xTableName, connection, transaction).ToString());


                            SortedList ValidationParam = new SortedList();
                            ValidationParam.Add("N_CompanyID", nCompanyID);
                            ValidationParam.Add("N_FnYearID", myFunctions.getIntVAL(Generaltable.Rows[0]["N_FnYearID"].ToString()));
                            ValidationParam.Add("X_Type", dt.TableName);
                            try
                            {
                             dLayer.ExecuteNonQueryPro("SP_SetupData_Validation", ValidationParam, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, ex));
                            }

                            if (dt.TableName.ToString().ToLower() == "sales invoice")
                            {
                                SortedList SalesInvParam = new SortedList();
                                SalesInvParam.Add("N_CompanyID", nCompanyID);
                                SalesInvParam.Add("N_FnyearID", myFunctions.getIntVAL(Generaltable.Rows[0]["N_FnYearID"].ToString()));
                                SalesInvParam.Add("N_UserID", myFunctions.GetUserID(User));
                                SalesInvParam.Add("N_BranchID", myFunctions.getIntVAL(Generaltable.Rows[0]["N_BranchID"].ToString()));
                                SalesInvParam.Add("N_LocationID", myFunctions.getIntVAL(Generaltable.Rows[0]["N_LocationID"].ToString()));
                                dLayer.ExecuteNonQueryPro("SP_SalesInvoiceImport", SalesInvParam, connection, transaction);
                            }
                            else
                            {
                               dLayer.ExecuteNonQueryPro("SP_SetupData_cloud", Params, connection, transaction);
                            }

                            if (nMasterID <= 0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, dt.TableName + " Uploaded Error"));
                            }
                            Mastertable.Clear();
                            Params.Remove("X_Type");
                            int TotalRecords = 0, TotalSkippedRecords = 0, TotalDraftedRecords = 0;
                            DataTable skippedRows = new DataTable();
                            SortedList Result = new SortedList();
                            if (xTableName == "Mig_SalesInvoice")
                            {
                                string sqlSkipInfo = "select 'Invoice Number  |  Invoice Date  |  Reson  ' as X_SkippingRemark,1 as B_Skipped union all select X_SkippingRemark,isnull(B_Skipped,0) as B_Skipped from " + xTableName + " where X_SkippingRemark is not null group by X_SkippingRemark,B_Skipped";
                                string sqlTotalRecords = "select count(distinct Invoice_Number) as TotalRecords from Mig_SalesInvoice";
                                string sqlSkippedRecords = "select count(distinct Invoice_Number) as SkippedRecords from Mig_SalesInvoice where isnull(B_Skipped,0)=1";
                                string sqlDraftedRecords = "SELECT  Count(distinct Invoice_Number) as DraftedInvoices FROM Mig_SalesInvoice LEFT OUTER JOIN Inv_Sales ON Mig_SalesInvoice.Invoice_Number = Inv_Sales.X_CustPONo AND Mig_SalesInvoice.N_CompanyID = Inv_Sales.N_CompanyId where isnull(Inv_Sales.B_IsSaveDraft,0)=1 and isnull(Mig_SalesInvoice.B_Skipped,0)=0 and Inv_Sales.N_CompanyId=" + nCompanyID;

                                skippedRows = dLayer.ExecuteDataTable(sqlSkipInfo, Params, connection, transaction);
                                TotalRecords = myFunctions.getIntVAL(dLayer.ExecuteScalar(sqlTotalRecords, connection, transaction).ToString());
                                TotalSkippedRecords = myFunctions.getIntVAL(dLayer.ExecuteScalar(sqlSkippedRecords, connection, transaction).ToString());
                                TotalDraftedRecords = myFunctions.getIntVAL(dLayer.ExecuteScalar(sqlDraftedRecords, connection, transaction).ToString());

                            }

                            Result.Add("skippedRows", _api.Format(skippedRows, "skippedRows"));
                            Result.Add("totalRecords", TotalRecords);
                            Result.Add("totalSkippedRecords", TotalSkippedRecords);
                            Result.Add("totalDraftedRecords", TotalDraftedRecords);
                            transaction.Commit();
                            return Ok(_api.Success(Result, dt.TableName + " Uploaded"));
                        }
                    }

                    transaction.Rollback();
                    return Ok(_api.Error(User, "Uploaded Error"));

                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        public string ValidateString(string InputString)
        {
            string OutputString = InputString.Replace("'", "''");
            OutputString = OutputString.Replace("|", "','");
            OutputString = "'" + OutputString + "'";
            return OutputString;
        }


        [HttpGet("dataList")]
        public ActionResult GetDepartmentList(string parent)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    string sqlCommandText = "select * from VW_TableCount where  N_CompanyID= " + nCompanyID + "";




                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    dt = _api.Format(dt);
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
    }
}