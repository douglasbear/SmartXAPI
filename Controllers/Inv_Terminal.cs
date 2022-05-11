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
        [HttpGet("list")]
        public ActionResult GetTerminalList(DateTime dDate)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from vw_InvTerminal_Disp where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", dDate);

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
<<<<<<< HEAD
        public ActionResult SetTerminal(int n_TerminalID, int n_SessionID, int n_BranchID, int n_LocationID, int n_FnYearID,int n_CashOpening,string d_SessionDate,string d_SessionStartTime)
=======
        public ActionResult SetTerminal(int n_TerminalID, int n_SessionID, int n_BranchID, int n_LocationID, int n_FnYearID, int n_CashOpening, string d_SessionDate, string d_SessionStartTime)
>>>>>>> b44c06facc1326f68fcd41846a388e135fa11864
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
                    if (n_SessionID == 0)
                    {
<<<<<<< HEAD
                        sqlCommandText = "select N_CompanyID," + n_FnYearID + " as N_FnYearID," + n_BranchID + " as N_BranchID,cast(@p2 as datetime) as D_SessionDate,0 as N_SessionID,N_TerminalID,cast(@p2 as datetime) as D_EntryDate," + nUserID + " as N_UserID,0 as B_Closed,"+n_CashOpening+" as n_CashOpening,'"+HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString()+"' as X_SessionStartIP,cast(@p2 as datetime) as D_SessionStartTime,'"+System.Net.Dns.GetHostName()+"' as X_SystemName from vw_InvTerminal_Disp where N_CompanyID=@p1 and N_TerminalID=" + n_TerminalID;
                        Params.Add("@p1", nCompanyId);
                        Params.Add("@p2", DateTime.ParseExact(d_SessionStartTime, "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture));
=======
                        sqlCommandText = "select Top(1) N_CompanyID," + n_FnYearID + " as N_FnYearID," + n_BranchID + " as N_BranchID,cast(@sessionTime as datetime) as D_SessionDate,0 as N_SessionID,N_TerminalID,cast(@sessionTime as datetime) as D_EntryDate," + nUserID + " as N_UserID,0 as B_Closed," + n_CashOpening + " as n_CashOpening,'" + HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() + "' as X_SessionStartIP,cast(@sessionTime as datetime) as D_SessionStartTime,'" + System.Net.Dns.GetHostName() + "' as X_SystemName from vw_InvTerminal_Disp where N_CompanyID=@nCompanyID and N_TerminalID=" + n_TerminalID;
                        Params.Add("@nCompanyID", nCompanyId);
                        Params.Add("@sessionTime", DateTime.ParseExact(d_SessionStartTime, "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture));


                        // Close Current active sessions

                        Double nCashWithdrawal = 0;
                        Params.Add("@nTerminalID", n_TerminalID);
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



>>>>>>> b44c06facc1326f68fcd41846a388e135fa11864

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
        public ActionResult GetTerminalDetails(int nTerminalID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_InvTerminal where N_CompanyID=@p1 and N_TerminalID=@p2";

            Params.Add("@p1", myFunctions.GetCompanyID(User));
            Params.Add("@p2", nTerminalID);


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Data Found"));
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


        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    int N_FnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearId"].ToString());
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string TerminalCode = "";
                    var values = MasterTable.Rows[0]["X_TerminalCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterTable.Rows[0]["N_CompanyId"].ToString());
                        Params.Add("N_YearID", N_FnYearId);
                        Params.Add("N_FormID", 895);
                        TerminalCode = dLayer.GetAutoNumber("Inv_Terminal", "X_TerminalCode", Params, connection, transaction);
                        if (TerminalCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Terminal Code")); }
                        MasterTable.Rows[0]["X_TerminalCode"] = TerminalCode;
                    }
                    MasterTable.Columns.Remove("N_FnYearId");
                    int N_TerminalID = dLayer.SaveData("Inv_Terminal", "N_TerminalID", MasterTable, connection, transaction);
                    if (N_TerminalID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }
                    // else{
                    // transaction.Commit();
                    // return Ok(_api.Success("Terminal Saved"));
                    // }

                    else
                    {
                        DetailTable.Rows[0]["N_TerminalID"] = N_TerminalID;
                        dLayer.SaveData("Inv_Terminaldetails", "N_SettingsID", DetailTable, connection, transaction);
                        transaction.Commit();
                        return Ok(_api.Success("Terminal Saved"));
                    }
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Results = dLayer.DeleteData("Inv_Terminal", "N_TerminalID", nTerminalID, "", connection);
                    if (Results > 0)
                    {
                        return Ok(_api.Success("Terminal deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Unable to delete Terminal"));
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