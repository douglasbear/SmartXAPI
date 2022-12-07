using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("posterminal")]
    [ApiController]
    public class Inv_Terminal : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly string ReportLocation;



        public Inv_Terminal(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            ReportLocation = conf.GetConnectionString("ReportLocation");
        }


        //GET api/productcategory/list?....
        // [HttpGet("list")]
        // public ActionResult GetTerminalList(DateTime dDate)
        // {
        //     DataTable dt = new DataTable();
        //     SortedList Params = new SortedList();
        //     int nCompanyId = myFunctions.GetCompanyID(User);

        //     string sqlCommandText = "select * from vw_InvTerminal_Disp where N_CompanyID=@p1";
        //     Params.Add("@p1", nCompanyId);
        //     Params.Add("@p2", dDate);

        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
        //         }
        //         if (dt.Rows.Count == 0)
        //         {
        //             return Ok(_api.Warning(""));
        //         }
        //         else
        //         {
        //             return Ok(_api.Success(dt));
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(_api.Error(User, e));
        //     }
        // }

        [HttpGet("list")]
        public ActionResult GetTerminalList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select N_TerminalID,X_TerminalName,X_TerminalCode,N_CompanyID from Vw_InvTerminal_Disp where N_CompanyID=@p1 Group by  N_TerminalID,X_TerminalName,X_TerminalCode,N_CompanyID";
            Params.Add("@p1", nCompanyId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning(""));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        [HttpGet("setTerminal")]
        public ActionResult SetTerminal(int n_TerminalID, int n_SessionID, int n_BranchID, int n_LocationID, int n_FnYearID, double n_CashOpening, string d_SessionDate, string d_SessionStartTime)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);



            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlCommandText = "";
                    connection.Open();
                    DataTable OutPut;
                    SqlTransaction transaction = connection.BeginTransaction();
                     Params.Add("@nTerminalID", n_TerminalID);
                        Params.Add("@nCompanyID", nCompanyId);
                        Params.Add("@sessionTime", DateTime.ParseExact(d_SessionStartTime, "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture));

                    if (n_SessionID == 0)
                    {

                      
                        string presessionsql = "select max(N_SessionID) from Acc_PosSession where N_CompanyID= "+nCompanyId+"";
                        object preSessionID = dLayer.ExecuteScalar(presessionsql, Params, connection,transaction);
                        string cashBalancesql = "select cast(isnull(N_CashBalance,0) as decimal(10,2)) from Acc_PosSession where N_CompanyID= "+nCompanyId+" and N_SessionID="+myFunctions.getIntVAL(preSessionID.ToString())+"";
                        object cashBalance = dLayer.ExecuteScalar(cashBalancesql, Params, connection,transaction);
                        sqlCommandText = "select Top(1) N_CompanyID," + n_FnYearID + " as N_FnYearID," + n_BranchID + " as N_BranchID,cast(@sessionTime as datetime) as D_SessionDate,0 as N_SessionID,N_TerminalID, "+myFunctions.getVAL(cashBalance.ToString())+" as n_CashBalance,cast(@sessionTime as datetime) as D_EntryDate," + nUserID + " as N_UserID,0 as B_Closed,"+myFunctions.getVAL(cashBalance.ToString())+" as n_CashOpening,'" + HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() + "' as X_SessionStartIP,cast(@sessionTime as datetime) as D_SessionStartTime,'" + System.Net.Dns.GetHostName() + "' as X_SystemName from vw_InvTerminal_Disp where N_CompanyID=@nCompanyID and N_TerminalID=" + n_TerminalID;
                        
                        // Params.Add("@sessionTime", DateTime.ParseExact(d_SessionStartTime, "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture));

                       // sqlCommandText = "select Top(1) N_CompanyID," + n_FnYearID + " as N_FnYearID," + n_BranchID + " as N_BranchID,cast(@sessionTime as datetime) as D_SessionDate,0 as N_SessionID,N_TerminalID, " + n_CashOpening + " as n_CashBalance,cast(@sessionTime as datetime) as D_EntryDate," + nUserID + " as N_UserID,0 as B_Closed,"+myFunctions.getVAL(cashBalance.ToString())+" as n_CashOpening,'" + HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() + "' as X_SessionStartIP,cast(@sessionTime as datetime) as D_SessionStartTime,'" + System.Net.Dns.GetHostName() + "' as X_SystemName from vw_InvTerminal_Disp where N_CompanyID=@nCompanyID and N_TerminalID=" + n_TerminalID;
                      
                        // Close Current active sessions

                        Double nCashWithdrawal = 0;
                       
                        string sessionsql = "select max(N_SessionID) from Acc_PosSession where N_CompanyID=@nCompanyID and N_TerminalID=@nTerminalID and isnull(B_Closed,0)=0";
                        object sessionID = dLayer.ExecuteScalar(sessionsql, Params, connection,transaction);
                        if (sessionID != null)
                        {
                            n_SessionID = myFunctions.getIntVAL(sessionID.ToString());
                            Params.Add("@nSessionID", n_SessionID);
                            Params.Add("@nCashWithdrawal", nCashWithdrawal);
                            string balance = "select cast(isnull(sum(Credit)-sum(debit),0) as decimal(10,2)) as Balance from Vw_POSTxnSummery where N_CompanyID=@nCompanyID and N_SessionID=@nSessionID";
                            object balAmt = dLayer.ExecuteScalar(balance, Params, connection,transaction);

                            Double balanceAmount = myFunctions.getFloatVAL(balAmt.ToString());
                            balanceAmount = balanceAmount - nCashWithdrawal;
                            Params.Add("@nCashBalance", balanceAmount);
                            balance = "select cast(isnull(sum(Credit),0) as decimal(10,2)) as Balance from Vw_POSTxnSummery where N_CompanyID=@nCompanyID and N_SessionID=@nSessionID and X_BalanceType<>'Cash Opening'";
                            Double credit = myFunctions.getFloatVAL(dLayer.ExecuteScalar(balance, Params, connection,transaction).ToString());
                            Params.Add("@nCredit", credit);
                            balance = "select cast(isnull(sum(Debit),0) as decimal(10,2)) as Balance from Vw_POSTxnSummery where N_CompanyID=@nCompanyID and N_SessionID=@nSessionID";
                            Double debit = myFunctions.getFloatVAL(dLayer.ExecuteScalar(balance, Params, connection,transaction).ToString());
                            Params.Add("@nDebit", debit);

                            Params.Add("@xHostName", System.Net.Dns.GetHostName());
                            Params.Add("@xEndIp", HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());

                            string updateSql = "Update Acc_PosSession set X_SessionEndIP='" + HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() + "', X_SystemName ='" + System.Net.Dns.GetHostName() + "', N_CashBalance=" + balanceAmount + ",N_CashCr=" + credit + ",N_CashDr=" + debit + ",B_closed=1 ,D_SessionEndTime='" + DateTime.ParseExact(d_SessionStartTime, "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture) + "',N_CashWithdrawal=" + nCashWithdrawal + " where N_CompanyID=@nCompanyID and N_TerminalID=@nTerminalID and N_SessionID=@nSessionID";
                            object result = dLayer.ExecuteNonQuery(updateSql, Params, connection,transaction);

                        }



                        // End




                        dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection, transaction);

                        int N_SessionID = dLayer.SaveData("Acc_PosSession", "N_SessionID", dt, connection, transaction);
                        if (N_SessionID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to create session"));
                        }
                        sqlCommandText = "select * from Acc_PosSession where N_CompanyID=@nCompanyID and N_SessionID=" + N_SessionID;
                        OutPut = dLayer.ExecuteDataTable(sqlCommandText, Params, connection, transaction);
                        transaction.Commit();
                        return Ok(_api.Success(OutPut));
                    }
                    else
                    {
                        sqlCommandText = "select isnull(B_Closed,0) as B_Closed from vw_InvTerminal_Disp where  N_CompanyID=@p1 and N_SessionID=" + n_SessionID;
                        int closed = myFunctions.getIntVAL(dLayer.ExecuteScalar(sqlCommandText, Params, connection, transaction).ToString());
                        if (closed == 1)
                        {
                            sqlCommandText = "select * from vw_InvTerminal_Disp where N_CompanyID=@p1";
                            OutPut = dLayer.ExecuteDataTable(sqlCommandText, Params, connection, transaction);
                            transaction.Commit();
                            return Ok(_api.Success(OutPut));

                        }
                        else
                        {
                            sqlCommandText = "select * from Acc_PosSession where N_CompanyID=@p1 and N_SessionID=" + n_SessionID;
                            OutPut = dLayer.ExecuteDataTable(sqlCommandText, Params, connection, transaction);
                            transaction.Commit();
                            return Ok(_api.Success(OutPut));
                        }
                    }
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        [HttpGet("printlist")]
        public ActionResult GetTerminalPrintList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            dt = myFunctions.AddNewColumnToDataTable(dt, "x_PrintTemplate", typeof(string), "");
            Params.Add("@p1", nCompanyId);
            string Path = ReportLocation + "printing/Salesinvoice/VAT/";

            try
            {
                string[] files = Directory.GetFiles(Path);
                int row = 0;
                foreach (string file in files)
                {

                    dt.Rows.Add();
                    dt.Rows[row]["x_PrintTemplate"] = System.IO.Path.GetFileName(file);
                    row = row + 1;
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetTerminalDetails(int nTerminalID, bool bAllBranchData, int nBranchID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            string Mastersql = "";

            if (bAllBranchData == true)
                Mastersql = "select * from vw_InvTerminalMaster where N_CompanyID=@p1 and N_TerminalID=@p3 ";
            else
                Mastersql = "select * from vw_InvTerminalMaster where N_CompanyID=@p1 and N_BranchID=@p2 and N_TerminalID=@p3 ";

            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nBranchID);
            Params.Add("@p3", nTerminalID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Data Found !!"));
                    }

                    MasterTable = _api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                    int n_TerminalID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TerminalID"].ToString());

                    string DetailSql = "";

                    DetailSql = "Select * from vw_InvTerminal where N_CompanyID=@p1 and N_TerminalID=" + n_TerminalID;
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);
                }
                return Ok(_api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpPost("closeSession")]
        public ActionResult CloseSession([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataRow dRow = MasterTable.Rows[0];
                    int nUserId = myFunctions.GetUserID(User);
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nTerminalID = myFunctions.getIntVAL(dRow["n_TerminalID"].ToString());
                    int nSessionID = myFunctions.getIntVAL(dRow["n_SessionID"].ToString());
                    Double nCashWithdrawal = myFunctions.getFloatVAL(dRow["n_CashWithdrawal"].ToString());
                    SortedList Params = new SortedList();
                    Params.Add("@endTime", DateTime.ParseExact(dRow["d_ClosedDate"].ToString(), "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture));
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nTerminalID", nTerminalID);
                    Params.Add("@nSessionID", nSessionID);
                    Params.Add("@nCashWithdrawal", nCashWithdrawal);
                    string balance = "select cast(isnull(sum(Credit)-sum(debit),0) as decimal(10,2)) as Balance from Vw_POSTxnSummery where N_CompanyID=@nCompanyID and N_SessionID=@nSessionID";
                    object balAmt = dLayer.ExecuteScalar(balance, Params, connection);

                    Double balanceAmount = myFunctions.getFloatVAL(balAmt.ToString());
                    balanceAmount = balanceAmount - nCashWithdrawal;
                    Params.Add("@nCashBalance", balanceAmount);
                    balance = "select cast(isnull(sum(Credit),0) as decimal(10,2)) as Balance from Vw_POSTxnSummery where N_CompanyID=@nCompanyID and N_SessionID=@nSessionID and X_BalanceType<>'Cash Opening'";
                    Double credit = myFunctions.getFloatVAL(dLayer.ExecuteScalar(balance, Params, connection).ToString());
                    Params.Add("@nCredit", credit);
                    balance = "select cast(isnull(sum(Debit),0) as decimal(10,2)) as Balance from Vw_POSTxnSummery where N_CompanyID=@nCompanyID and N_SessionID=@nSessionID";
                    Double debit = myFunctions.getFloatVAL(dLayer.ExecuteScalar(balance, Params, connection).ToString());
                    Params.Add("@nDebit", debit);

                    Params.Add("@xHostName", System.Net.Dns.GetHostName());
                    Params.Add("@xEndIp", HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());

                    string updateSql = "Update Acc_PosSession set X_SessionEndIP='" + HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() + "', X_SystemName ='" + System.Net.Dns.GetHostName() + "', N_CashBalance=" + balanceAmount + ",N_CashCr=" + credit + ",N_CashDr=" + debit + ",B_closed=1 ,D_SessionEndTime='" + DateTime.ParseExact(dRow["d_ClosedDate"].ToString(), "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture) + "',N_CashWithdrawal=" + nCashWithdrawal + " where N_CompanyID=@nCompanyID and N_TerminalID=@nTerminalID and N_SessionID=@nSessionID";
                    object result = dLayer.ExecuteNonQuery(updateSql, Params, connection);
                    if (result == null)
                        result = 0;
                    if (myFunctions.getIntVAL(result.ToString()) > 0)
                        return Ok(_api.Success("Session closed"));
                    else
                        return Ok(_api.Error(User, "Unable to end Session"));


                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nTerminalID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_TerminalID"].ToString());
                int nSettingsID = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();

                    // Auto Gen
                    string xTerminalCode = "";
                    var values = MasterTable.Rows[0]["x_TerminalCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", 895);
                        Params.Add("N_TerminalID", nTerminalID);
                        xTerminalCode = dLayer.GetAutoNumber("Inv_Terminal", "X_TerminalCode", Params, connection, transaction);
                        if (xTerminalCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Code")); }
                        MasterTable.Rows[0]["X_TerminalCode"] = xTerminalCode;
                    }
                    MasterTable.Columns.Remove("N_FnYearId");
                    nTerminalID = dLayer.SaveData("Inv_Terminal", "N_TerminalID", MasterTable, connection, transaction);
                    if (nTerminalID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                    dLayer.DeleteData("Inv_TerminalDetails", "N_TerminalID", nTerminalID, "", connection, transaction);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_TerminalID"] = nTerminalID;
                    }
                    nSettingsID = dLayer.SaveData("Inv_TerminalDetails", "N_SettingsID", DetailTable, connection, transaction);
                    if (nSettingsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable To Save"));
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Terminal Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nTerminalID)
        {
            int Results = 0;
            try
            {
                int nCompanyID = myFunctions.GetCompanyID(User);
                SortedList Params = new SortedList();
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", nTerminalID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Inv_Terminal", "N_TerminalID", nTerminalID, "", connection);

                    if (Results > 0)
                    {
                        dLayer.DeleteData("Inv_TerminalDetails", "N_TerminalID", nTerminalID, "", connection);
                        return Ok(_api.Success("Terminal deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
    }
}