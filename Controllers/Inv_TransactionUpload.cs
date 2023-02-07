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
    [Route("transactionUpload")]
    [ApiController]
    public class Gen_TransactionUpload : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        public Gen_TransactionUpload(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
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
                Generaltable = ds.Tables["general"];
                int nCompanyID = myFunctions.GetCompanyID(User);
                int nMasterID = 0;
                string xTableName = "";
                SortedList Params = new SortedList();
                Params.Add("N_CompanyID", nCompanyID);
                Params.Add("N_FnYearID", myFunctions.getIntVAL(Generaltable.Rows[0]["N_FnYearID"].ToString()));
                Params.Add("N_BranchID", myFunctions.getIntVAL(Generaltable.Rows[0]["N_BranchID"].ToString()));
                Params.Add("N_LocationID", myFunctions.getIntVAL(Generaltable.Rows[0]["N_LocationID"].ToString()));
                int N_UserID = myFunctions.GetUserID(User);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt.Columns.Contains("notes"))
                            dt.Columns.Remove("notes");
                        Params.Add("X_Type", dt.TableName);
                        Mastertable = ds.Tables[dt.TableName];
                        foreach (DataColumn col in Mastertable.Columns)
                        {
                            col.ColumnName = col.ColumnName.Replace(" ", "_");
                            col.ColumnName = col.ColumnName.Replace("*", "");
                            col.ColumnName = col.ColumnName.Replace("/", "_");
                        }
                        Mastertable.Columns.Add("Pkey_Code");

                        if (dt.TableName.ToString().ToLower() == "customer list" || dt.TableName.ToString().ToLower() == "customers" || dt.TableName.ToString().ToLower() == "customer")
                            xTableName = "Mig_Customers";
                        if (dt.TableName.ToString().ToLower() == "vendor list" || dt.TableName.ToString().ToLower() == "vendors" || dt.TableName.ToString().ToLower() == "vendor")
                            xTableName = "Mig_Vendors";
                        if (dt.TableName.ToString().ToLower() == "lead list")
                        {
                            xTableName = "Mig_Leads";
                            Mastertable.Columns.Add("N_UserID");
                            foreach (DataRow dtRow in Mastertable.Rows)
                            {
                                dtRow["N_UserID"] = N_UserID;
                            }
                        }
                        if (dt.TableName.ToString().ToLower() == "chart of accounts")
                            xTableName = "Mig_Accounts";
                        if (dt.TableName.ToString().ToLower() == "products stock")
                            xTableName = "Mig_Stock";
                        if (dt.TableName.ToString().ToLower() == "employee list" || dt.TableName.ToString().ToLower() == "employees")
                            xTableName = "Mig_Employee";
                        if (dt.TableName.ToString().ToLower() == "products stock")
                            xTableName = "Mig_Stock";
                        if (dt.TableName.ToString().ToLower() == "fixedassets list")
                            xTableName = "_Mig_AssetList";
                        if (dt.TableName.ToString().ToLower() == "salary history")
                            xTableName = "Mig_EmployeeSalaryHistory";
                        if (dt.TableName.ToString().ToLower() == "employee salary")
                            xTableName = "Mig_EmployeeSalary";
                        if (dt.TableName.ToString().ToLower() == "leave history")
                            xTableName = "Mig_EmployeeLeaveHistory";
                        if (dt.TableName.ToString().ToLower() == "customer balances")
                            xTableName = "Mig_CustomerOpening";
                        if (dt.TableName.ToString().ToLower() == "vendor balances")
                            xTableName = "Mig_VendorOpening";
                            

                        if (dt.TableName.ToString().ToLower() == "product list" || dt.TableName.ToString().ToLower() == "products" || dt.TableName.ToString().ToLower() == "customer materials" )
                        {
                            xTableName = "Mig_Items";
                            Mastertable.Columns.Add("N_CompanyID");
                            foreach (DataRow dtRow in Mastertable.Rows)
                            {
                                dtRow["N_CompanyID"] = nCompanyID;
                            }
                        }


                        if (dt.TableName.ToString().ToLower() == "category")
                        {
                            xTableName = "Mig_POSCategory";
                            Mastertable.Columns.Add("N_CompanyID");
                            foreach (DataRow dtRow in Mastertable.Rows)
                            {
                                dtRow["N_CompanyID"] = nCompanyID;
                            }
                        }

                           if (dt.TableName.ToString().ToLower() == "delivery notes")
                        {
                            xTableName = "Mig_Deliverynote";
                            
                            Mastertable.Columns.Add("N_CompanyID");
                            foreach (DataRow dtRow in Mastertable.Rows)
                            {
                                dtRow["N_CompanyID"] = nCompanyID;
                            }
                        }

                           if (dt.TableName.ToString().ToLower() == "goods received note")
                        {
                            xTableName = "Mig_Grn";
                            
                            Mastertable.Columns.Add("N_CompanyID");
                            foreach (DataRow dtRow in Mastertable.Rows)
                            {
                                dtRow["N_CompanyID"] = nCompanyID;
                            }
                        }
                        

                        if (dt.TableName.ToString().ToLower() == "package items")
                        {
                            xTableName = "Mig_PackageItem";
                            Mastertable.Columns.Add("N_CompanyID");
                            foreach (DataRow dtRow in Mastertable.Rows)
                            {
                                dtRow["N_CompanyID"] = nCompanyID;
                            }
                        }


                        if (dt.TableName.ToString().ToLower() == "warranty items")
                        {
                            xTableName = "Mig_WarrantyItem";
                            Mastertable.Columns.Add("N_CompanyID");
                            foreach (DataRow dtRow in Mastertable.Rows)
                            {
                                dtRow["N_CompanyID"] = nCompanyID;
                            }
                        }

                        if (Mastertable.Rows.Count > 0)
                        {

                            if (Mastertable.Columns.Contains("Notes"))
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

                                         }else{
                                        values = Mastertable.Rows[j][k].ToString();
                                         }
                                        values = values.Replace("|", " ");
                                    }

                                    FieldValues = FieldValues + "|" + values;
                                    if (j == 0)
                                        FieldList = FieldList + "," + Mastertable.Columns[k].ColumnName.ToString();


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


 
                            //  nMasterID = dLayer.SaveData(xTableName, "PKey_Code", Mastertable, connection, transaction);
                            nMasterID= myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Count(1) from "+xTableName, connection, transaction).ToString());

                            object ValFlag;
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
                            // if(ValFlag==null)return Ok(_api.Error(User, "Uploaded Error"));
                            // if(myFunctions.getIntVAL(ValFlag.ToString())==0)return Ok(_api.Error(User, "Uploaded Error"));
                            dLayer.ExecuteNonQueryPro("SP_SetupData_cloud", Params, connection, transaction);
                            if (nMasterID <= 0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, dt.TableName + " Uploaded Error"));
                            }
                            Mastertable.Clear();
                            Params.Remove("X_Type");
                            transaction.Commit();
                            return Ok(_api.Success(dt.TableName + " Uploaded"));
                        }
                    }
                    if (Mastertable.Rows.Count > 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Uploaded Completed"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Uploaded Error"));
                    }
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