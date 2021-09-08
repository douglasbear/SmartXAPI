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
                        Params.Add("X_Type", dt.TableName);
                        Mastertable = ds.Tables[dt.TableName];
                        foreach (DataColumn col in Mastertable.Columns)
                        {
                            col.ColumnName = col.ColumnName.Replace(" ", "_");
                            col.ColumnName = col.ColumnName.Replace("*", "");
                            col.ColumnName = col.ColumnName.Replace("/", "_");
                        }
                        Mastertable.Columns.Add("Pkey_Code");

                        if (dt.TableName == "Customer List")
                            xTableName = "Mig_Customers";
                        if (dt.TableName == "Vendor List")
                            xTableName = "Mig_Vendors";
                        if (dt.TableName == "Leads List")
                            xTableName = "Mig_Leads";
                        if (dt.TableName == "Chart of Accounts")
                            xTableName = "Mig_Accounts";
                        if (dt.TableName == "Products Stock")
                            xTableName = "Mig_Stock";

                        if (dt.TableName == "Product List")
                        {
                            xTableName = "Mig_Items";
                            Mastertable.Columns.Add("N_CompanyID");
                            foreach (DataRow dtRow in Mastertable.Rows)
                            {
                                dtRow["N_CompanyID"] = nCompanyID;
                            }
                        }

                        if (Mastertable.Rows.Count > 0)
                        {

                            dLayer.ExecuteNonQuery("delete from " + xTableName, Params, connection, transaction);
                            nMasterID = dLayer.SaveData(xTableName, "PKey_Code", Mastertable, connection, transaction);
                            dLayer.ExecuteNonQueryPro("SP_SetupData", Params, connection, transaction);
                            if (nMasterID <= 0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User,dt.TableName + " Uploaded Error"));
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
                        return Ok(_api.Error(User,"Uploaded Error"));
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