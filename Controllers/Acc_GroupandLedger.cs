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
            sqlCommandText = "Select * from vw_AccMastGroup Where N_CompanyID= @p1 and N_FnYearID=@p2 Order By X_GroupCode";
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
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("transactionType")]
        public ActionResult AccountTransactionType()
        {
            DataTable dt = new DataTable();
            string sqlCommandText = "select N_CategoryID,N_GenTypeId,N_SubCategoryID,N_Order,X_Description,X_Description_Ar from Acc_CashFlowCategory";
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
                return Ok(api.Error(User, e));
            }
        }



        [HttpGet("account")]
        public ActionResult MasterAccountist(int nFnYearId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = ""; 
            sqlCommandText = "Select N_LedgerID,N_GroupID,N_CompanyID,N_Reserved,B_InActive,N_UserID,X_Level,X_CashTypeBehaviour,X_Type,B_CostCenterEnabled,N_FnYearID,N_PostingBahavID,X_LedgerName_Ar,X_GroupCode,X_GroupName,N_CashBahavID,B_CostCentreRequired,N_TaxCategoryID1,X_TransactionType,X_TaxType,N_Amount,B_IsAmtEdit,N_TaxTypeID,X_TaxTypeName,N_CurrencyID,[Account Code] as X_LedgerCode,[Account] as X_LedgerName ,N_LedgerBehaviourID as n_LedgerBehavID,N_TransactionTypeID from vw_AccMastLedger Where N_CompanyID= @p1 and N_FnYearID=@p2 Order By [Account Code]";
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
                return Ok(api.Error(User, e));
            }
        }
        [HttpGet("getCode")]
        public ActionResult MasterGroupCode(int nFnYearID, int nGroupID, int nParentGroup)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                     SqlTransaction transaction = connection.BeginTransaction();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nFnYearID);
                    Params.Add("@p3", nGroupID);
                    Params.Add("@p5", nParentGroup);
                    string X_LedgerCode = "";
                    string X_GroupCode = "";
                     string level = "";
                    dt.Clear();
                    dt.Columns.Add("X_LedgerCode");
                    dt.Columns.Add("X_GroupCode");

                    if (nGroupID > 0)
                    {

                        object LedgerCodeCount = dLayer.ExecuteScalar("select COUNT(convert(nvarchar(100),X_LedgerCode)) From Acc_MastLedger where N_GroupID =@p3 and N_CompanyID =@p1 and N_FnYearID=@p2",Params, connection,transaction);
                        if (LedgerCodeCount == null)
                            return Ok(api.Error(User, "Error"));

                        object LedgerCodeObj = dLayer.ExecuteScalar("select X_GroupCode From Acc_MastGroup where N_GroupID =@p3 and N_CompanyID =@p1 and N_FnYearID=@p2", Params, connection,transaction);
          
                        int count = myFunctions.getIntVAL(LedgerCodeCount.ToString());
                        while (true)
                        {
                            count += 1;
                            X_LedgerCode = LedgerCodeObj.ToString() + count.ToString("000");
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Acc_MastLedger Where X_LedgerCode ='" + X_LedgerCode + "' and N_CompanyID=@p1 and N_FnYearID =@p2", Params, connection,transaction);
                            if (N_Result == null)
                                break;
                        }

                        DataRow row = dt.NewRow();
                        row["X_LedgerCode"] = X_LedgerCode;
                        dt.Rows.Add(row);
                    }
                    


                        //MasterTable.Rows[0]["X_GroupCode"] = X_GroupCode;
                    if (nParentGroup > 0)
                    {

                        object GroupCodeCount = dLayer.ExecuteScalar("select COUNT(convert(numeric,X_GroupCode)) From Acc_MastGroup where N_CompanyID =@p1 and  N_ParentGroup =@p5 and N_FnYearID=@p2", Params, connection,transaction);
                        if (GroupCodeCount == null)
                            return Ok(api.Error(User, "Error"));

                        object GroupCodeObj = dLayer.ExecuteScalar("Select X_GroupCode from Acc_MastGroup Where N_GroupID =@p5 and N_CompanyID= @p1 and N_FnYearID =@p2", Params, connection,transaction);

                        int count = myFunctions.getIntVAL(GroupCodeCount.ToString());
                        while (true)
                        {
                            count += 1;
                            X_GroupCode = GroupCodeObj.ToString() + count.ToString("00");
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Acc_MastGroup Where X_GroupCode ='" + X_GroupCode + "' and N_CompanyID= @p1 and N_FnYearID =@p2", Params, connection,transaction);
                            if (N_Result == null)
                                break;
                        }
                        DataRow row = dt.NewRow();
                        row["X_GroupCode"] = X_GroupCode;
                        dt.Rows.Add(row);


                    }
                      if (nParentGroup == 0)
                      {
                     object GroupCodeCount = dLayer.ExecuteScalar("select COUNT(convert(numeric,X_GroupCode)) From Acc_MastGroup where X_Level like '" + level + "%' and N_CompanyID =" + nCompanyID + " and  N_ParentGroup =" + nParentGroup + "  and N_FnYearID=" + nFnYearID, connection, transaction);
                    object GroupCodeObj = dLayer.ExecuteScalar("Select X_GroupCode from Acc_MastGroup Where N_GroupID =" + nParentGroup + " and N_CompanyID= " + nCompanyID + " and N_FnYearID =" + nFnYearID, connection, transaction);
                          if(GroupCodeObj==null)
                            GroupCodeObj=0;
                        int count = myFunctions.getIntVAL(GroupCodeCount.ToString());
                        while (true)
                        {
                            count += 1;
                            if(myFunctions.getIntVAL(GroupCodeObj.ToString())==0)
                            X_GroupCode = dLayer.ExecuteScalar("Select max(cast(X_GroupCode as numeric))+1 from Acc_MastGroup Where N_ParentGroup=0 and N_CompanyID= " + nCompanyID + " and N_FnYearID =" + nFnYearID, connection, transaction).ToString();
                            else
                            X_GroupCode = GroupCodeObj.ToString() + count.ToString("00");

                            object N_Result = dLayer.ExecuteScalar("Select 1 from Acc_MastGroup Where X_GroupCode ='" + X_GroupCode + "' and N_CompanyID= " + nCompanyID + " and N_FnYearID =" + nFnYearID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                          DataRow row = dt.NewRow();
                        row["X_GroupCode"] = X_GroupCode;
                        dt.Rows.Add(row);
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
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
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
                int N_LedgerID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_LedgerID"].ToString());
                string X_Operation = MasterTable.Rows[0]["x_Operation"].ToString();
                string X_LedgerName = MasterTable.Rows[0]["X_LedgerName"].ToString();
                if (MasterTable.Columns.Contains("x_Operation"))
                    MasterTable.Columns.Remove("x_Operation");
                string X_LedgerCode = MasterTable.Rows[0]["x_LedgerCode"].ToString();
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
                            return Ok(api.Error(User, "Error"));

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

                    if (N_LedgerID > 0)
                    {
                        object LedgerCount = dLayer.ExecuteScalar("select COUNT(X_LedgerCode) From Acc_MastLedger where N_GroupID =" + N_GroupID + " and N_CompanyID =" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and N_LedgerID<>" + N_LedgerID + " and X_LedgerCode='"+X_LedgerCode+"'", connection, transaction);
                        LedgerCount = LedgerCount == null ? 0 : LedgerCount;
                        if (myFunctions.getIntVAL(LedgerCount.ToString()) > 0)
                            return Ok(api.Error(User, "Account Code Already In Use !!"));
                    }

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and (X_LedgerCode='" + X_LedgerCode + "' OR X_LedgerName = '" + X_LedgerName + "')";
                    string X_Crieteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId;
                    int Result = dLayer.SaveData("Acc_MastLedger", "N_LedgerID", DupCriteria, X_Crieteria, MasterTable, connection, transaction);
                    if (Result <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
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
                return Ok(api.Error(User, ex));
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
                if (MasterTable.Columns.Contains("x_Operation"))
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
                        if(ReturnNewLevel(paramList, connection, transaction).ToString()=="")
                            MasterTable.Rows[0]["x_Level"]=MasterTable.Rows[0]["x_GroupCode"].ToString();

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
                            return Ok(api.Error(User, "Error"));

                        object GroupCodeObj = dLayer.ExecuteScalar("Select X_GroupCode from Acc_MastGroup Where N_GroupID =" + N_ParentGroupID + " and N_CompanyID= " + nCompanyID + " and N_FnYearID =" + nFnYearId, connection, transaction);
                          if(GroupCodeObj==null)
                            GroupCodeObj=0;
                        int count = myFunctions.getIntVAL(GroupCodeCount.ToString());
                        while (true)
                        {
                            count += 1;
                            if(myFunctions.getIntVAL(GroupCodeObj.ToString())==0)
                            X_GroupCode = dLayer.ExecuteScalar("Select max(cast(X_GroupCode as numeric))+1 from Acc_MastGroup Where N_ParentGroup=0 and N_CompanyID= " + nCompanyID + " and N_FnYearID =" + nFnYearId, connection, transaction).ToString();
                            else
                            X_GroupCode = GroupCodeObj.ToString() + count.ToString("00");

                            object N_Result = dLayer.ExecuteScalar("Select 1 from Acc_MastGroup Where X_GroupCode ='" + X_GroupCode + "' and N_CompanyID= " + nCompanyID + " and N_FnYearID =" + nFnYearId, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        MasterTable.Rows[0]["X_GroupCode"] = X_GroupCode;
                    }
                    MasterTable.AcceptChanges();

                    if (N_GroupID > 0)
                    {
                        object LedgerCount = dLayer.ExecuteScalar("select COUNT(convert(nvarchar(100),x_GroupCode)) From Acc_MastGroup where N_GroupID <>" + N_GroupID + " and N_CompanyID =" + nCompanyID + " and N_FnYearID=" + nFnYearId +" and X_GroupCode="+X_GroupCode, connection, transaction);
                        LedgerCount = LedgerCount == null ? 0 : LedgerCount;
                        if (myFunctions.getIntVAL(LedgerCount.ToString()) > 0)
                            return Ok(api.Error(User, "Group Code Already In Use !!"));
                    }

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and (X_GroupCode='" + X_GroupCode + "' OR X_GroupName='" + X_GroupName + "')";
                    string Criteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId;
                    int Result = dLayer.SaveData("Acc_MastGroup", "N_GroupID", DupCriteria, Criteria, MasterTable, connection, transaction);
                    if (Result <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
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
                return Ok(api.Error(User, ex));
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
        public ActionResult DeleteData(string accountType, int accountID, int nFnYearID)
        {
            int Result = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (accountType == "AL")
                    {

                        if(CheckPrevYearsClosed(accountID, nFnYearID, connection)==1)
                            return Ok(api.Error(User, "Previous Year Not Closed"));
                        // else if(CheckPrevYearsBalance(accountID, nFnYearID, connection)==1)
                            // return Ok(api.Error(User, "Balance exists"));
                        else if (CheckTransaction(accountID,nFnYearID, connection) == 1)
                            return Ok(api.Error(User, "Transaction Started"));
                        else if (CheckTransactionNotPosted(accountID, nFnYearID, connection) == 1)
                            return Ok(api.Error(User, "Transaction Pending"));

                        Result = dLayer.DeleteData("Acc_MastLedger", "N_LedgerID", accountID, "N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnYearID, connection);

                    }
                    else
                    {
                        Result = dLayer.DeleteData("Acc_MastGroup", "N_GroupID", accountID, "N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_FnYearID=" + nFnYearID, connection);

                    }

                }
                if (Result > 0)
                {
                    return Ok(api.Success("Deleted Successfully"));
                }
                else
                {
                    return Ok(api.Error(User, "Unable to delete"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }


        }


        private int CheckTransaction(int N_LedgerID, int nFnYearID, SqlConnection connection)
        {
            object value = dLayer.ExecuteScalar("SELECT DISTINCT 1 from Acc_MastLedger inner join Acc_VoucherDetails on Acc_MastLedger.N_CompanyID = Acc_VoucherDetails.N_CompanyID and Acc_MastLedger.N_FnYearID = Acc_VoucherDetails.N_FnYearID and Acc_MastLedger.N_LedgerID = Acc_VoucherDetails.N_LedgerID where  Acc_VoucherDetails.N_LedgerID=" + N_LedgerID + "and Acc_MastLedger.N_CompanyID=" + myFunctions.GetCompanyID(User) + " AND Acc_MastLedger.N_FnYearID="+nFnYearID, connection);
            if (value == null)
                return 0;
            else if (myFunctions.getIntVAL(value.ToString()) == 1)
                return 1;
            else
                return 0;
        }
        //check ACC_voucher details table
        private int CheckTransactionNotPosted(int N_LedgerID, int nFnYearID, SqlConnection connection)
        {
            object value = dLayer.ExecuteScalar("SELECT DISTINCT 1 from Acc_VoucherMaster_Details INNER JOIN Acc_VoucherMaster ON Acc_VoucherMaster_Details.N_VoucherID = Acc_VoucherMaster.N_VoucherID where Acc_VoucherMaster.N_FnYearID=" + nFnYearID + "and Acc_VoucherMaster_Details.N_LedgerID=" + N_LedgerID + "and Acc_VoucherMaster_Details.N_CompanyID=" + myFunctions.GetCompanyID(User) + " ", connection);
            if (value == null)
                return 0;
            else if (myFunctions.getIntVAL(value.ToString()) == 1)
                return 1;
            else
                return 0;
        }
        private int CheckPrevYearsClosed(int N_LedgerID, int nFnYearID, SqlConnection connection)
        {            
            object value = dLayer.ExecuteScalar("SELECT DISTINCT 1 FROM Acc_FnYear INNER JOIN Acc_MastLedger ON Acc_FnYear.N_CompanyID = Acc_MastLedger.N_CompanyID AND Acc_FnYear.N_FnYearID = Acc_MastLedger.N_FnYearID "+
                                                " WHERE     (Acc_FnYear.N_CompanyID = "+ myFunctions.GetCompanyID(User)+") AND Acc_MastLedger.N_LedgerID="+N_LedgerID+" and Acc_FnYear.B_YearEndProcess=0 AND Acc_FnYear.D_End< "+
                                                " (SELECT D_Start from Acc_FnYear WHERE N_CompanyID="+ myFunctions.GetCompanyID(User)+" AND N_FnYearID="+nFnYearID+")", connection);
            if (value == null)
                return 0;
            else if (myFunctions.getIntVAL(value.ToString()) == 1)
                return 1;
            else
                return 0;
        }
         private int CheckPrevYearsBalance(int N_LedgerID, int nFnYearID, SqlConnection connection)
        {  
            object value = dLayer.ExecuteScalar("select SUM(N_Amount) from Acc_VoucherDetails where N_CompanyID="+myFunctions.GetCompanyID(User)+" and N_LedgerID="+N_LedgerID+" and N_FnYearID=(select TOP 1 N_FnYearID from Acc_FnYear where N_CompanyID="+myFunctions.GetCompanyID(User)+" and D_End< "+
                                                " (select D_Start from Acc_FnYear where N_CompanyID="+myFunctions.GetCompanyID(User)+" and N_FnYearID="+nFnYearID+") order by D_Start desc)", connection);
             if (value == DBNull.Value)
              {
             value = null;
              }
   
            if (value == null)
                return 0;
            else if (myFunctions.getVAL(value.ToString()) == 0)
                return 1;
            else
                return 0;
        }

    }
}