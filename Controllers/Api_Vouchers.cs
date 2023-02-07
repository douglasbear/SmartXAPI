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
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("journal-voucher")]
    [ApiController]
    public class Api_Vouchers : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly ISec_UserRepo _repository;

        public Api_Vouchers(ISec_UserRepo repository, IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _repository = repository;
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString =
            conf.GetConnectionString("SmartxConnection");
        }

        [HttpPost()]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable dt;
                MasterTable = ds.Tables["master"];
                string VoucherID = "";
                string Auth = "";
                if (Request.Headers.ContainsKey("Authorization"))
                    Auth = Request.Headers["Authorization"];
                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "pkeyID", typeof(int), 0);
                MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "X_Type", typeof(string), "jv");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    dt = dLayer.ExecuteDataTable("select * from sec_user where  X_Token='" + Auth + "'", Params, connection, transaction);
                    if (dt.Rows.Count > 0)
                    {
                        object N_FnyearID = dLayer.ExecuteScalar("select MAX(N_FnyearID) from Acc_Fnyear where N_CompanyID=" + dt.Rows[0]["N_CompanyID"], connection, transaction);
                        Params.Add("N_CompanyID", dt.Rows[0]["N_CompanyID"]);
                        Params.Add("X_Type", "jv");
                        Params.Add("N_FnYearID", N_FnyearID);
                        Params.Add("D_Date", MasterTable.Rows[0]["D_Date"]);
                        Params.Add("N_BranchID", dt.Rows[0]["N_BranchID"]);
                        Params.Add("N_UserID", dt.Rows[0]["N_UserID"]);

                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_FnyearID", typeof(int), N_FnyearID);

                        dLayer.ExecuteNonQuery("delete from Mig_Vouchers", Params, connection, transaction);
                        MasterTable = myFunctions.AddNewColumnToDataTable(MasterTable, "N_CompanyID", typeof(int), dt.Rows[0]["N_CompanyID"]);
                        dLayer.SaveData("Mig_Vouchers", "pkeyID", MasterTable, connection, transaction);
                        dLayer.ExecuteNonQuery("Update sec_user Set X_Token= '' where N_UserID = " + dt.Rows[0]["N_UserID"], Params, connection, transaction);
                        VoucherID =dLayer.ExecuteScalarPro("SP_VouchersImport", Params, connection, transaction).ToString();

                        //Posting...
                        SortedList PostingParams = new SortedList();
                        PostingParams.Add("N_CompanyID", dt.Rows[0]["N_CompanyID"]);
                        PostingParams.Add("X_InventoryMode", "jv");
                        PostingParams.Add("N_InternalID", VoucherID);
                        PostingParams.Add("N_UserID", dt.Rows[0]["N_UserID"]);
                        PostingParams.Add("X_SystemName", "ERP Cloud");
                        object posting = dLayer.ExecuteScalarPro("SP_Acc_InventoryPosting", PostingParams, connection, transaction);


                    }
                    else
                        return Ok(api.Error(User, "Invalid Token"));



                    if (myFunctions.getIntVAL(VoucherID) <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Journal Voucher Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }
    }
}

