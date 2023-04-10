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
     [Route("whBinTransfer")]
     [ApiController]
    public class WhBinTransferList : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly string connectionString;
        private readonly IMyFunctions myFunctions;
        private readonly IApiFunctions _api;
        private readonly int nFormID = 1430;
        public WhBinTransferList(IDataAccessLayer dl,IMyFunctions myFun, IApiFunctions apiFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            _api = api;
            myFunctions=myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetDashboardList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            // if (xSearchkey != null && xSearchkey.Trim() != "")
            //     Searchkey = "and (X_PickListCode like '%" + xSearchkey + "%' or X_CustomerName like '%" + xSearchkey + "%')";

            // if (xSortBy == null || xSortBy.Trim() == "")
            //    
            // else
            // {
            //     
            // }
             xSortBy = " order by N_BinTransID desc";

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_WhBinTrans_Search where N_CompanyID=@nCompanyID" + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_WhBinTrans_Search where N_CompanyID=@nCompanyID" + Searchkey + " and N_BinTransID not in (select top(" + Count + ") N_BinTransID from vw_wh_BinTransHistoryDetails where N_CompanyID=@nCompanyID " + Searchkey + xSortBy + " ) " + xSortBy;

            Params.Add("@nCompanyID", nCompanyID);
           
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    string sqlCommandCount = "select count(1) as N_Count  from Vw_WhBinTrans_Search where N_CompanyID=@nCompanyID" + Searchkey + "";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Success(OutPut));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
       

         [HttpPost("save")]
          public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                     DataRow DetailRow = DetailTable.Rows[0];
                    SortedList Params = new SortedList();

                    int nBinTransID = myFunctions.getIntVAL(MasterRow["N_BinTransID"].ToString());
                    int nBinTransDetailsID = myFunctions.getIntVAL(DetailRow["N_BinTransDetailsID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string xBinTransferHistoryCode = MasterRow["x_BinTransHistoryCode"].ToString();
                    

                    //Parent Chaning Function
                   foreach (DataRow row in DetailTable.Rows)
                    {

                       
                        object childPattern = dLayer.ExecuteScalar("Select isnull(max(X_Pattern),'')  From Inv_Location Where N_CompanyID=" + nCompanyID + "  and N_MainLocationID=" + myFunctions.getIntVAL(row["N_ParentLocationID"].ToString()) + " ", connection, transaction);
                        if(childPattern.ToString()=="")
                        {
                            object parentMainPattern = dLayer.ExecuteScalar("select TOP 1  X_Pattern from Inv_Location where N_LocationID=" + myFunctions.getIntVAL(row["N_ParentLocationID"].ToString()) + " and N_CompanyID=" + nCompanyID + " ", connection, transaction);
                              string newpattern = parentMainPattern.ToString() + "10";
                                dLayer.ExecuteNonQuery("Update Inv_Location SET X_Pattern= '"+newpattern+"'  where N_CompanyID=" + nCompanyID + " and N_LocationID=" +myFunctions.getIntVAL(row["N_LocationID"].ToString()), Params, connection,transaction);
                        dLayer.ExecuteNonQuery("Update Inv_Location SET N_MainLocationID= '"+ myFunctions.getIntVAL(row["N_ParentLocationID"].ToString())+"'  where N_CompanyID=" + nCompanyID + " and N_LocationID=" +myFunctions.getIntVAL(row["N_LocationID"].ToString()), Params, connection,transaction);                        

                        }
                        else
                        {
                        string removingPattern =childPattern.ToString().Substring(childPattern.ToString().Length-2);
                        string addingPattern=((myFunctions.getIntVAL(removingPattern.ToString())) + 10 ).ToString();
                        childPattern=childPattern.ToString().Remove(childPattern.ToString().Length-2);
                        string Pattern=childPattern+addingPattern;
                        dLayer.ExecuteNonQuery("Update Inv_Location SET X_Pattern= '"+Pattern+"'  where N_CompanyID=" + nCompanyID + " and N_LocationID=" +myFunctions.getIntVAL(row["N_LocationID"].ToString()), Params, connection,transaction);
                        dLayer.ExecuteNonQuery("Update Inv_Location SET N_MainLocationID= '"+ myFunctions.getIntVAL(row["N_ParentLocationID"].ToString())+"'  where N_CompanyID=" + nCompanyID + " and N_LocationID=" +myFunctions.getIntVAL(row["N_LocationID"].ToString()), Params, connection,transaction);                        
                        }
                    }



                    string x_BinTransHistoryCode = "";
                    if (xBinTransferHistoryCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", nFormID);
                        x_BinTransHistoryCode = dLayer.GetAutoNumber("Wh_BinTranHistory", "X_BinTransHistoryCode", Params, connection, transaction);
                        if (x_BinTransHistoryCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate bin transfer Code");
                        }
                        MasterTable.Rows[0]["x_BinTransHistoryCode"] = x_BinTransHistoryCode;
                    }
                    else
                    {
                         dLayer.DeleteData("Wh_BinTranHistory", "N_BinTransID", nBinTransID, "", connection,transaction);
                          dLayer.DeleteData("Wh_BinTranHistoryDetails", "N_BinTransID", nBinTransID, "", connection,transaction);
                    }
                    MasterTable.Columns.Remove("n_FnYearID");

                    int n_BinTransID = dLayer.SaveData("Wh_BinTranHistory", "N_BinTransID", "", "", MasterTable, connection, transaction);
                    if (n_BinTransID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Warehouse bin");
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_BinTransID"] = n_BinTransID;
                    }
                    int n_BinTransDetailsID = dLayer.SaveData("Wh_BinTranHistoryDetails", "N_BinTransDetailsID", DetailTable, connection, transaction);
                    if (n_BinTransDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Warehouse Bin Transfer");
                    }

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_BinTransID", n_BinTransID);
                    Result.Add("x_BinTransHistoryCode", x_BinTransHistoryCode);
                    Result.Add("n_BinTransDetailsID", n_BinTransDetailsID);

                    return Ok(_api.Success(Result, "Warehouse Bin Transfer Created"));
                }
            }
            
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

         [HttpGet("details")]
        public ActionResult EmployeeEvaluation(string xBinTransHistoryCode)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable DataTable = new DataTable();
                    string Mastersql = "";
                    string DetailSql = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@xBinTransHistoryCode", xBinTransHistoryCode);
                    Mastersql = "select * from vw_wh_BinTransHistoryDetails where N_CompanyID=@nCompanyID and X_BinTransHistoryCode=@xBinTransHistoryCode ";
                   
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int nBinTransID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BinTransID"].ToString());
                    Params.Add("@nBinTransID", nBinTransID);

                    MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from vw_wh_BinTransHistoryDetails where N_CompanyID=@nCompanyID and N_BinTransID=@nBinTransID ";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

       
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nBinTransID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nBinTransID", nBinTransID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Wh_BinTranHistory", "N_BinTransID", nBinTransID, "", connection);

                    if (Results > 0)
                    {
                        dLayer.DeleteData("Wh_BinTranHistoryDetails", "N_BinTransID", nBinTransID, "", connection);
                        return Ok(_api.Success("Warehouse BinTransfer deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete"));
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
