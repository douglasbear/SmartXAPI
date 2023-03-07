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
    [Route("branch")]
    [ApiController]



    public class AccBranchController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly string masterDBConnectionString;
        private readonly int FormID;

        public AccBranchController(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
             masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 370;

        }


        [HttpGet("list")]
        public ActionResult GetAllBranches()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "SELECT Acc_BranchMaster.N_BranchID, Acc_BranchMaster.N_CompanyId, Acc_BranchMaster.X_BranchName, Acc_BranchMaster.X_BranchCode, Acc_BranchMaster.Active,isnull(Acc_BranchMaster.B_ShowAllData,0) as B_ShowAllData, Inv_Location.N_LocationID, Inv_Location.X_LocationCode,Inv_Location.X_LocationName FROM Acc_BranchMaster LEFT OUTER JOIN Inv_Location ON Acc_BranchMaster.N_CompanyID = Inv_Location.N_CompanyID AND Acc_BranchMaster.N_BranchID = Inv_Location.N_BranchID where Acc_BranchMaster.N_CompanyId=@p1 and Inv_Location.B_IsDefault=1 order by Acc_BranchMaster.X_BranchName DESC";
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
                    return Ok(_api.Notice("No Results Found"));
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
        public ActionResult GetBranchDetails(int nBranchID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from Acc_BranchMaster where N_CompanyId=@nCompanyId and N_BranchID=@nBranchID";
            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nBranchID", nBranchID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
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

        [HttpPost("change")]
        public ActionResult ChangeData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();

                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nBranchID", nBranchID);

                    dLayer.ExecuteNonQuery("update Acc_BranchMaster set IsCurrent=0 where N_CompanyID=@nCompanyID", Params, connection);
                    dLayer.ExecuteNonQuery("update Acc_BranchMaster set IsCurrent=1 where N_BranchID=@nBranchID and N_CompanyID=@nCompanyID", Params, connection);

                    return Ok(_api.Success("Branch Changed"));
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                         int ClientID = myFunctions.GetClientID(User);
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    SortedList Params1 = new SortedList();
                    SortedList ValidateParams = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    int nCurrentBranch = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CurentBranchID"].ToString());
                    string xBranchCode = MasterTable.Rows[0]["x_BranchCode"].ToString();
                    int nLocationID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                    string xLocationCode = MasterTable.Rows[0]["x_BranchCode"].ToString();
                    string xTerminalCode = MasterTable.Rows[0]["x_BranchCode"].ToString();
                    string xLocationName = MasterTable.Rows[0]["x_BranchName"].ToString();
                    string logo = myFunctions.ContainColumn("i_Logo", MasterTable) ? MasterTable.Rows[0]["i_Logo"].ToString() : "";
                    bool bDefaultBranch = false;
                    if (MasterTable.Columns.Contains("b_DefaultBranch"))
                    {
                        bDefaultBranch = myFunctions.getBoolVAL(MasterTable.Rows[0]["b_DefaultBranch"].ToString());
                      
                    }
                    Byte[] logoBitmap = new Byte[logo.Length];
                    logoBitmap = Convert.FromBase64String(logo);
                    if (myFunctions.ContainColumn("i_Logo", MasterTable))
                        MasterTable.Columns.Remove("i_Logo");
                    MasterTable.AcceptChanges();

                    if (MasterTable.Columns.Contains("n_FnYearID"))
                        MasterTable.Columns.Remove("n_FnYearID");
                     if (MasterTable.Columns.Contains("n_CurentBranchID"))
                        MasterTable.Columns.Remove("n_CurentBranchID");

                    MasterTable.AcceptChanges();
                    if (xBranchCode == "@Auto")
                    {
                        ValidateParams.Add("@N_CompanyID", nCompanyID);
                        using (SqlConnection cnn2 = new SqlConnection(masterDBConnectionString))
                        {
                         cnn2.Open();
                         object BranchCount = dLayer.ExecuteScalar("select count(N_BranchID)  from Acc_BranchMaster where N_CompanyID=@N_CompanyID", ValidateParams, connection, transaction);
                         object limit = dLayer.ExecuteScalar("select isnull(N_Value,0) from GenSettings where N_ClientID="+ClientID+" and X_Description='BRANCH LIMIT' ", cnn2);
                         if(limit==null){limit="0";}
                         if (BranchCount != null && limit != null)
                         {
                            if (myFunctions.getIntVAL(BranchCount.ToString()) >= myFunctions.getIntVAL(limit.ToString()))
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, "Branch Limit exceeded!!!"));
                            }
                         }
                        }
                      if(bDefaultBranch==true)
                       {
                       object headoffcCount = dLayer.ExecuteScalar("select count(B_DefaultBranch) from Acc_BranchMaster where N_CompanyID=" + nCompanyID,connection, transaction);
                          if (headoffcCount != null )
                        {
                            if (myFunctions.getIntVAL(headoffcCount.ToString()) >= 1)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, "Head Office Limit exceeded!!!"));
                            }
                        }
                       }
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                        xBranchCode = dLayer.GetAutoNumber("Acc_BranchMaster", "x_BranchCode", Params, connection, transaction);
                        if (xBranchCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Branch Code")); }
                        MasterTable.Rows[0]["x_BranchCode"] = xBranchCode;
                    }

                    else
                    {
                        dLayer.DeleteData("Acc_BranchMaster", "N_BranchID", nBranchID, "", connection, transaction);
                    }
                
                    nBranchID = dLayer.SaveData("Acc_BranchMaster", "N_BranchID", MasterTable, connection, transaction);
                    if (xTerminalCode == "@Auto" && nCurrentBranch >0)
                    {
                    int invoiceCounterInsert = dLayer.ExecuteNonQuery("insert into Inv_InvoiceCounter " +
                                    "select N_CompanyID,N_FormID, X_Prefix,N_StartNo,(N_StartNo-1) AS N_LastUsedNo,B_AutoInvoiceEnabled,N_MenuID,N_FnYearID,B_Yearwise,"+nBranchID+",N_MinimumLen,B_Suffix, X_Suffix, B_ResetYearly, X_Type, X_Type2  from Inv_InvoiceCounter "+
                                    "where N_BranchID=" + nCurrentBranch + " and N_CompanyID=" + nCompanyID , Params, connection, transaction);

                                if (invoiceCounterInsert <= 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Warning("invoice counter failed"));
                                }
                    }
                    if (nBranchID <= 0)
                    {

                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save Branch"));
                    }
                    else
                    {
                        if (xLocationCode == "@Auto")
                        {
                            String sql = "select N_CompanyID,0 as N_LocationID,'' as X_LocationCode,X_BranchName as X_LocationName,N_BranchID,0 as B_IsCurrent,0 as N_typeId,0 as N_WarehouseID,1 as B_IsDefault,X_Address,X_PhoneNo,0 as B_PhysicalSite,0 as N_MainLocationID from Acc_BranchMaster where N_CompanyID=@nCompanyID and N_branchID=@nBranchID";
                            SortedList BranchParams = new SortedList();
                            BranchParams.Add("@nCompanyID", nCompanyID);
                            BranchParams.Add("@nBranchID", nBranchID);
                            DataTable LocationTable = dLayer.ExecuteDataTable(sql, BranchParams, connection, transaction);
                            if (LocationTable.Rows.Count == 0)
                            {
                                transaction.Rollback(); return Ok(_api.Error(User, "Unable to Create location"));
                            }
                            Params1.Add("N_CompanyID", nCompanyID);
                            Params1.Add("N_YearID", nFnYearID);
                            Params1.Add("N_FormID", 450);
                            xLocationCode = dLayer.GetAutoNumber("Inv_Location", "x_LocationCode", Params1, connection, transaction);
                            if (xLocationCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate location Code")); }
                            LocationTable.Rows[0]["x_LocationCode"] = xLocationCode;
                            LocationTable.AcceptChanges();
                            String DupCriteria = "N_BranchID=" + nBranchID + " and X_LocationName= '" + xLocationName + "' and N_CompanyID=" + nCompanyID;
                            nLocationID = dLayer.SaveData("Inv_Location", "N_LocationID", DupCriteria, "", LocationTable, connection, transaction);
                            if (nLocationID <= 0)
                            {
                                transaction.Rollback(); return Ok(_api.Error(User, "Unable to Create location"));
                            }

                        }

                        if (xTerminalCode == "@Auto" && nLocationID>0)
                        {
                            String sql = "select N_CompanyID,0 as N_TerminalID,'' as X_TerminalCode,X_LocationName + ' Terminal' as X_TerminalName,0 as N_UserID,getDate() as D_EntryDate,null as N_PriceTypeID,N_LocationID,N_BranchID from Inv_Location where N_CompanyID=@nCompanyID and N_LocationID=@nLocationID";
                            SortedList TerminalParams = new SortedList();
                            TerminalParams.Add("@nCompanyID", nCompanyID);
                            TerminalParams.Add("@nLocationID", nLocationID);
                            DataTable TerminalTable = dLayer.ExecuteDataTable(sql, TerminalParams, connection, transaction);
                            if (TerminalTable.Rows.Count == 0)
                            {
                                transaction.Rollback(); return Ok(_api.Error(User, "Unable to Create location"));
                            }
                            SortedList Params2 = new SortedList();
                            Params2.Add("N_CompanyID", nCompanyID);
                            Params2.Add("N_YearID", nFnYearID);
                            Params2.Add("N_FormID", 895);
                            xTerminalCode = dLayer.GetAutoNumber("Inv_Terminal", "X_TerminalCode", Params2, connection, transaction);
                            if (xTerminalCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate location Code")); }
                            TerminalTable.Rows[0]["X_TerminalCode"] = xTerminalCode;
                            TerminalTable.AcceptChanges();
                            String DupCriteria = "N_BranchID=" + nBranchID + " and X_TerminalName= '" + xLocationName + "' and N_CompanyID=" + nCompanyID;
                            int nTerminalID = dLayer.SaveData("Inv_Terminal", "N_TerminalID", DupCriteria, "", TerminalTable, connection, transaction);
                            if (nTerminalID <= 0)
                            {
                                transaction.Rollback(); return Ok(_api.Error(User, "Unable to Create terminal"));
                            }

                        }


                        if (logo.Length > 0)
                            dLayer.SaveImage("Acc_BranchMaster", "I_Logo", logoBitmap, "N_BranchID", nBranchID, connection, transaction);
                        transaction.Commit();
                        return Ok(_api.Success("Branch Saved"));
                    }


                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nBranchID,int nFnYearID)
        {
            int Results = 0;

            SortedList Params = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    Params.Add("@nBranchID", nBranchID);
                    int nCompanyId = myFunctions.GetCompanyID(User);
                    object count = dLayer.ExecuteScalar("select count(1) as N_Count from Vw_Acc_BranchMaster_Disp where N_BranchID=@nBranchID and N_CompanyID=N_CompanyID", Params, connection);
                    int N_Count = myFunctions.getIntVAL(count.ToString());
                    if (N_Count <= 0)
                    {
                        Results = dLayer.DeleteData("Acc_BranchMaster", "N_BranchID", nBranchID, "", connection);

                        Results = dLayer.DeleteData("Inv_Location", "N_BranchID", nBranchID, "B_IsDefault=1", connection);
                        Results = dLayer.DeleteData("inv_invoiceCounter", "N_BranchID", nBranchID,"N_CompanyID="+nCompanyId , connection);

                    }
                    else
                    {
                        return Ok(_api.Success("unable to delete this branch"));
                    }
                    if (Results >= 0)
                    {
                        return Ok(_api.Success("Branch deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Unable to delete Branch"));
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