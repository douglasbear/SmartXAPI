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
    [Route("accountMaster")]
    [ApiController]
    public class Acc_GroupandLedger : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Acc_GroupandLedger(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("group")]
        public ActionResult MasterGroupList(int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            sqlCommandText = "Select * from Acc_MastGroup Where N_CompanyID= @p1 and N_FnYearID=@p2 Order By X_GroupCode";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(api.Format(dt)));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpGet("transactionType")]
        public ActionResult AccountTransactionType()
        {
            DataTable dt = new DataTable();
            string sqlCommandText = "select N_CategoryID,N_GenTypeId,N_SubCategoryID,N_Order,X_Description from Acc_CashFlowCategory";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, connection);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(api.Format(dt)));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        

        [HttpGet("account")]
        public ActionResult MasterAccountist(int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            sqlCommandText = "Select *,[Account Code] as X_LedgerCode,[Account] as X_LedgerName from vw_AccMastLedger Where N_CompanyID= @p1 and N_FnYearID=@p2 Order By N_GroupID";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(api.Format(dt)));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpPost("saveAccount")]
        public ActionResult SaveAccount([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int N_GroupID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_GroupID"].ToString());
                string X_Operation = MasterTable.Rows[0]["x_Operation"].ToString();
                string X_LedgerName= MasterTable.Rows[0]["X_LedgerName"].ToString();
                MasterTable.Columns.Remove("x_Operation");
                string X_LedgerCode = "";
                MasterTable.AcceptChanges();
                SortedList paramList = new SortedList();
                paramList.Add("@nCompanyID", nCompanyID);
                paramList.Add("@nFnYearID", nFnYearId);
                paramList.Add("@nGroupLevelID", N_GroupID);


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();

                    if (X_Operation == "Save")
                    {

                        object LedgerCodeCount = dLayer.ExecuteScalar("select COUNT(convert(nvarchar(100),X_LedgerCode)) From Acc_MastLedger where N_GroupID =" + N_GroupID + " and N_CompanyID =" + nCompanyID + " and N_FnYearID=" + nFnYearId, connection, transaction);
                        if (LedgerCodeCount == null)
                            return Ok(api.Error("Error"));

                        object LedgerCodeObj = dLayer.ExecuteScalar("select X_GroupCode From Acc_MastGroup where N_GroupID =" + N_GroupID + " and N_CompanyID =" + nCompanyID + " and N_FnYearID=" + nFnYearId, connection, transaction);

                        int count = myFunctions.getIntVAL(LedgerCodeCount.ToString());
                        while (true)
                        {
                            count += 1;
                            X_LedgerCode = LedgerCodeObj.ToString() + count.ToString("000");
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Acc_MastLedger Where X_LedgerCode ='" + X_LedgerCode + "' and N_CompanyID= " + nCompanyID + " and N_FnYearID =" + nFnYearId, connection, transaction);
                            if (N_Result == null)
                                break;
                        }

                        MasterTable.Rows[0]["X_LedgerCode"] = X_LedgerCode;
                    }
                    MasterTable.AcceptChanges();
                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and (X_LedgerCode='" + X_LedgerCode + "' OR X_LedgerName = '" + X_LedgerName + "')";
                    string X_Crieteria="N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId;
                    int Result = dLayer.SaveData("Acc_MastLedger", "N_LedgerID",DupCriteria,X_Crieteria, MasterTable, connection, transaction);
                    if (Result <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        if (X_Operation == "Save")
                            return Ok(api.Success("Ledger Created"));
                        else
                            return Ok(api.Success("Ledger Updated"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }


        }


        [HttpPost("saveAccountGroup")]
        public ActionResult SaveAccountGroup([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int N_GroupID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_GroupID"].ToString());
                int N_ParentGroupID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ParentGroup"].ToString());
                string X_Operation = MasterTable.Rows[0]["x_Operation"].ToString();
                string x_Type = MasterTable.Rows[0]["x_Type"].ToString();
                MasterTable.Columns.Remove("x_Operation");
                string X_GroupCode = MasterTable.Rows[0]["x_GroupCode"].ToString();
                string X_GroupName = MasterTable.Rows[0]["x_GroupName"].ToString();
                MasterTable.AcceptChanges();
                SortedList paramList = new SortedList();
                paramList.Add("@nCompanyID", nCompanyID);
                paramList.Add("@nFnYearID", nFnYearId);
                paramList.Add("@nGroupLevelID", myFunctions.getIntVAL(MasterTable.Rows[0]["n_ParentGroup"].ToString()));


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();

                    if (X_Operation == "Save")
                    {
                        MasterTable.Rows[0]["x_Level"] = ReturnNewLevel(paramList, connection, transaction).ToString();
                        string level = "";

                        if (x_Type == "A")
                            level = "1";
                        else if (x_Type == "L")
                            level = "2";
                        else if (x_Type == "I")
                            level = "3";
                        else if (x_Type == "E")
                            level = "4";


                        object GroupCodeCount = dLayer.ExecuteScalar("select COUNT(convert(numeric,X_GroupCode)) From Acc_MastGroup where X_Level like '" + level + "%' and N_CompanyID =" + nCompanyID + " and  N_ParentGroup =" + N_ParentGroupID + "  and N_FnYearID=" + nFnYearId, connection, transaction);
                        if (GroupCodeCount == null)
                            return Ok(api.Error("Error"));

                        object GroupCodeObj = dLayer.ExecuteScalar("Select X_GroupCode from Acc_MastGroup Where N_GroupID =" + N_ParentGroupID + " and N_CompanyID= " + nCompanyID + " and N_FnYearID =" + nFnYearId, connection, transaction);

                        int count = myFunctions.getIntVAL(GroupCodeCount.ToString());
                        while (true)
                        {
                            count += 1;
                            X_GroupCode = GroupCodeObj.ToString() + count.ToString("00");
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Acc_MastGroup Where X_GroupCode ='" + X_GroupCode + "' and N_CompanyID= " + nCompanyID + " and N_FnYearID =" + nFnYearId, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        MasterTable.Rows[0]["X_GroupCode"] = X_GroupCode;
                    }
                    MasterTable.AcceptChanges();
                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and (X_GroupCode='" + X_GroupCode + "' OR X_GroupName='" + X_GroupName + "')";
                    string Criteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId;
                    int Result = dLayer.SaveData("Acc_MastGroup", "N_GroupID",DupCriteria,Criteria, MasterTable, connection, transaction);
                    if (Result <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error("Unable to save"));
                    }
                    else
                    {
                        if (N_GroupID == 0)
                        {
                            SortedList gruopsParam = new SortedList();
                            gruopsParam.Add("N_GroupID", N_GroupID);
                            gruopsParam.Add("N_FnyearID", nFnYearId);
                            gruopsParam.Add("N_CompanyID", nCompanyID);
                            dLayer.ExecuteScalarPro("SP_AccGruops_Create", gruopsParam, connection, transaction);
                        }
                        transaction.Commit();
                        if (X_Operation == "Save")
                            return Ok(api.Success("Group Created"));
                        else
                            return Ok(api.Success("Group Updated"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }


        }

        private string ReturnNewLevel(SortedList paramList, SqlConnection connection, SqlTransaction transaction)
        {

            DataTable Acc_MastGroupLevel = dLayer.ExecuteDataTable("Select Temp.X_Level, (select isnull(Max(Isnull( convert(numeric,X_Level),0)),Temp.X_Level) From Acc_MastGroup where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID and X_Level like Temp.X_Level+'%' and LEN(Temp.X_Level)+ 2 = LEN(X_Level))   From Acc_MastGroup as Temp  where Temp.N_GroupID =@nGroupLevelID and Temp.N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", paramList, connection, transaction);
            if (Acc_MastGroupLevel.Rows.Count == 0)
                return "";

            DataRow drow = Acc_MastGroupLevel.Rows[0];
            if (Convert.ToInt64(drow[0].ToString()) == Convert.ToInt64(drow[1].ToString()))
                return drow[0].ToString() + "01";
            else
            {
                long count = Convert.ToInt64(drow[1].ToString()) + 1;
                return count.ToString();
            }

        }


              [HttpDelete("delete")]
        public ActionResult DeleteData(string accountType,int accountID,int nFnYearID)
            {
                int Result = 0;
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (accountType == "AL")
                    {

                        if (CheckTransaction(accountID,connection) == 1)
                        {
                            return Ok(api.Error("Transaction Started"));
                        }
                        else if (CheckTransactionNotPosted(accountID,nFnYearID,connection) == 1)
                        {
                            return Ok(api.Error("Transaction Pending"));
                        }
                        Result = dLayer.DeleteData("Acc_MastLedger", "N_LedgerID", accountID, "N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnYearID,connection);

                    }
                    else
                    {
                        Result = dLayer.DeleteData("Acc_MastGroup", "N_GroupID", accountID, "N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnYearID ,connection);

                    }
            
                }
                    if (Result > 0)
                    {
                         return Ok(api.Success("Deleted Successfully"));
                    }
                    else
                    {
                        return Ok(api.Error("Unable to delete"));
                    }

                }
                catch (Exception ex)
                {
                    return Ok(api.Error(ex));
                }


            }


        private int CheckTransaction(int N_LedgerID,SqlConnection connection )
        {
             object value = dLayer.ExecuteScalar("SELECT DISTINCT 1 from Acc_MastLedger inner join Acc_VoucherDetails on Acc_MastLedger.N_CompanyID = Acc_VoucherDetails.N_CompanyID and Acc_MastLedger.N_FnYearID = Acc_VoucherDetails.N_FnYearID and Acc_MastLedger.N_LedgerID = Acc_VoucherDetails.N_LedgerID where  Acc_VoucherDetails.N_LedgerID=" + N_LedgerID + "and Acc_MastLedger.N_CompanyID=" + myFunctions.GetCompanyID(User) + " ",connection);
             if (value == null)
                 return 0;
             else if (myFunctions.getIntVAL(value.ToString()) == 1)
                 return 1;
             else
                 return 0;
        }
        //check ACC_voucher details table
        private int CheckTransactionNotPosted(int N_LedgerID,int nFnYearID,SqlConnection connection )
        {
            object value = dLayer.ExecuteScalar("SELECT DISTINCT 1 from Acc_VoucherMaster_Details INNER JOIN Acc_VoucherMaster ON Acc_VoucherMaster_Details.N_VoucherID = Acc_VoucherMaster.N_VoucherID where Acc_VoucherMaster.N_FnYearID=" + nFnYearID + "and Acc_VoucherMaster_Details.N_LedgerID=" + N_LedgerID + "and Acc_VoucherMaster_Details.N_CompanyID=" + myFunctions.GetCompanyID(User) + " ",connection);
            if (value == null)
                return 0;
            else if (myFunctions.getIntVAL(value.ToString()) == 1)
                return 1;
            else
                return 0;
        }

    }
}