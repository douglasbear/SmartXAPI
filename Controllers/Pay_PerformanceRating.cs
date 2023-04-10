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
    [Route("performanceRating")]
    [ApiController]

    public class Pay_PerformanceRating : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1443;

        public Pay_PerformanceRating(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult PerformanceRating()
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
 
            string sqlCommandText = "select X_Code as X_PerformanceCode,* from Pay_Performance where N_CompanyID=@p1 order by N_PerformanceID";
            Params.Add("@p1", nCompanyID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("details")]
        public ActionResult GradeListDetails(string xCode)
        {
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
  
            string Mastersql = "select * from Pay_Performance where N_CompanyID=@p1 and X_Code=@p2";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", xCode);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable=dLayer.ExecuteDataTable(Mastersql,Params,connection); 

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Data Found !!"));
                    }

                    MasterTable = api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                    int N_PerformanceID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PerformanceID"].ToString());

                    string DetailSql = "select * from Pay_PerformanceDetails where N_CompanyID=" + nCompanyID + " and N_PerformanceID=" + N_PerformanceID ;

                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);
                }
                return Ok(api.Success(dt));
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
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
                    SortedList Params = new SortedList();

                    int nPerformanceID = myFunctions.getIntVAL(MasterRow["n_PerformanceID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string xCode = MasterRow["x_Code"].ToString();

                    string x_Code = "";
                    if (xCode == "@Auto")
                    {
                         Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearID);
                         Params.Add("N_FormID", this.N_FormID);
                        x_Code = dLayer.GetAutoNumber("Pay_Performance", "X_Code", Params, connection, transaction);
                        if (x_Code == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Performance Rating");
                        }
                        MasterTable.Rows[0]["x_Code"] = x_Code;
                    }

                      if (MasterTable.Columns.Contains("n_FnYearID"))
                    {

                        MasterTable.Columns.Remove("n_FnYearID");

                    }
                      if (nPerformanceID>0)
                    {
                        
                         dLayer.DeleteData("Pay_Performance", "n_PerformanceID", nPerformanceID, "N_CompanyID=" + nCompanyID + " and n_PerformanceID=" + nPerformanceID, connection, transaction);
                         dLayer.DeleteData("Pay_PerformanceDetails", "n_PerformanceID", nPerformanceID, "N_CompanyID=" + nCompanyID + " and n_PerformanceID=" + nPerformanceID, connection, transaction);

                    }

                    int n_PerformanceID = dLayer.SaveData("Pay_Performance", "N_PerformanceID", "", "", MasterTable, connection, transaction);
                    if (n_PerformanceID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save performance rating");
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_PerformanceID"] = n_PerformanceID;
                    }
                    int n_PerformanceDetailsID = dLayer.SaveData("Pay_PerformanceDetails", "N_PerformanceDetailsID", DetailTable, connection, transaction);
                    if (n_PerformanceDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_PerformanceID", n_PerformanceID);
                    Result.Add("x_Code", x_Code);
                    Result.Add("N_PerformanceDetailsID", n_PerformanceDetailsID);

                    return Ok(api.Success(Result, "Performance Rating Created"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        
       


          [HttpDelete("delete")]
        public ActionResult DeleteData(int nPerformanceID, int nCompanyID, int nFnYearID)
        {
            int Results = 0;
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nPerformanceID", nPerformanceID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Pay_Performance", "N_PerformanceID", nPerformanceID, "", connection);

                    if (Results > 0)
                    {
                        dLayer.DeleteData("Pay_PerformanceDetails", "N_PerformanceID", nPerformanceID, "", connection);
                        return Ok(api.Success("Performance deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User,"Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }
    }
}