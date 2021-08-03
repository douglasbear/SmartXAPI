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
                int nCompanyID = myFunctions.GetCompanyID(User);
                int nFnYearId = 1;
                int nMasterID = 0;
                string xTableName = "";
                SortedList Params = new SortedList();
                Params.Add("N_CompanyID", nCompanyID);
                Params.Add("N_FnYearID", nFnYearId);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt.TableName == "Customer List")
                        {
                            Mastertable = ds.Tables["Customer List"];
                            foreach (DataColumn col in Mastertable.Columns)
                            {
                                col.ColumnName = col.ColumnName.Replace(" ", "_");
                                col.ColumnName = col.ColumnName.Replace("*", "");
                            }
                            Mastertable.Columns.Add("Pkey_Code");
                            xTableName = "Mig_Customers";
                            Params.Add("X_Type", "customer");

                        }
                        if (dt.TableName == "Vendor List")
                        {

                            Mastertable = ds.Tables["Vendor List"];
                            foreach (DataColumn col in Mastertable.Columns)
                            {
                                col.ColumnName = col.ColumnName.Replace(" ", "_");
                                col.ColumnName = col.ColumnName.Replace("*", "");
                            }
                            Mastertable.Columns.Add("Pkey_Code");
                            xTableName = "Mig_Vendors";
                            Params.Add("X_Type", "vendor");

                        }
                        if (dt.TableName == "Product List")
                        {

                            Mastertable = ds.Tables["Product List"];
                            foreach (DataColumn col in Mastertable.Columns)
                            {
                                col.ColumnName = col.ColumnName.Trim().Replace(" ", "_");
                                col.ColumnName = col.ColumnName.Trim().Replace("*", "");
                                col.ColumnName = col.ColumnName.Trim().Replace("/", "_");
                            }
                            Mastertable.Columns.Add("Pkey_Code");
                            xTableName = "Mig_Items";
                            Params.Add("X_Type", "product");
                            Mastertable.Columns.Add("N_CompanyID");
                            Mastertable.Rows[0]["N_CompanyID"] = nCompanyID;

                        }
                        if (dt.TableName == "Leads List")
                        {

                            Mastertable = ds.Tables["Leads List"];
                            foreach (DataColumn col in Mastertable.Columns)
                            {
                                col.ColumnName = col.ColumnName.Trim().Replace(" ", "_");
                                col.ColumnName = col.ColumnName.Trim().Replace("*", "");
                                col.ColumnName = col.ColumnName.Trim().Replace("/", "_");
                            }
                            Mastertable.Columns.Add("Pkey_Code");
                            xTableName = "Mig_Leads";
                            Params.Add("X_Type", "Leads");
                            Mastertable.Columns.Add("N_CompanyID");
                            Mastertable.Rows[0]["N_CompanyID"] = nCompanyID;

                        }

                        if (Mastertable.Rows.Count > 0)
                        {

                            dLayer.ExecuteNonQuery("delete from " + xTableName, Params, connection, transaction);
                            nMasterID = dLayer.SaveData(xTableName, "PKey_Code", Mastertable, connection, transaction);
                            dLayer.ExecuteNonQueryPro("SP_SetupData", Params, connection, transaction);
                            if (nMasterID <= 0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(dt.TableName +" Uploaded Error"));
                            }
                            Mastertable.Clear();
                            Params.Remove("X_Type");
                            transaction.Commit();
                            return Ok(_api.Success(dt.TableName +" Uploaded"));
                        }
                    }
                    if (Mastertable.Rows.Count > 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Uploaded Completed"));
                    }
                    else
                    {
                        return Ok(_api.Error("Uploaded Error"));
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
    }
}