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
        public ActionResult PerformanceRating(int nPerformanceID,int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText ="";

            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and (X_Code like '%" + xSearchkey + "%'or X_Code like '%" + xSearchkey + "%' )";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_PerformanceID desc";
            else
                xSortBy = " order by " + xSortBy;
            
            Params.Add("@p1", nCompanyId);
            Params.Add("@p3", nPerformanceID);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    //sqlCommandCount = "select count(*) as N_Count  from vw_CRMCustomer where N_CompanyID=@p1  " + Pattern;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }

                }
                
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }




             [HttpGet("details")]
        public ActionResult GradeListDetails(string xcode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
  
            string sqlCommandText = "select * from Pay_PerformanceDetails where N_CompanyID=@p1 and X_Code=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", xcode);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                }
                dt = api.Format(dt);
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


    //  [HttpPost("save")]
    //     public ActionResult SaveData([FromBody] DataSet ds)
    //     {
    //         try
    //         {
    //             using (SqlConnection connection = new SqlConnection(connectionString))
    //             {
    //                 connection.Open();
    //                 SqlTransaction transaction = connection.BeginTransaction();
    //                 DataTable MasterTable;
    //                 DataTable DetailTable;
    //                 MasterTable = ds.Tables["master"];
    //                  DetailTable = ds.Tables["details"];
    //                 DataRow MasterRow = MasterTable.Rows[0];
    //                 SortedList Params = new SortedList();
    //                 int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
    //                  int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
    //                 int nPerformanceID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PerformanceID"].ToString());
    //                 string X_Code = MasterTable.Rows[0]["X_Code"].ToString();
      

    //                 if (X_Code == "@Auto")
    //                 {
    //                     Params.Add("N_CompanyID", nCompanyID);
    //                     Params.Add("N_FormID", this.FormID);

                        
                        
    //                     X_Code = dLayer.GetAutoNumber("Pay_Performance", "X_Code", Params, connection, transaction);
    //                     if (X_Code == "")
    //                     {
    //                         transaction.Rollback();
    //                         return Ok(api.Error(User, "Unable to generate  Code"));
    //                     }
    //                     MasterTable.Rows[0]["X_Code"] = X_Code;
    //                 }
                 

    //                 if (nPerformanceID > 0)
    //                 {
    //                     dLayer.DeleteData("Pay_Performance", "n_PerformanceID", nPerformanceID, "N_CompanyID=" + nCompanyID + " and n_PerformanceID=" + nPerformanceID, connection, transaction);
    //                     dLayer.DeleteData("Pay_PerformanceDetails", "n_PerformanceID", nPerformanceID, "N_CompanyID=" + nCompanyID + " and n_PerformanceID=" + nPerformanceID, connection, transaction);
    //                 }
                
    //                  nPerformanceID = dLayer.SaveData("Pay_Performance", "n_PerformanceID", MasterTable, connection, transaction);

    //                 if (nPerformanceID <= 0)
    //                 {
    //                     transaction.Rollback();
    //                     return Ok(api.Error(User, "Unable to save"));
    //                 }

    //                  for (int j = 0; j < DetailTable.Rows.Count; j++)
    //                 {
    //                     DetailTable.Rows[j]["n_PerformanceID"] = nPerformanceID;
    //                 }

    //                 int N_PerformanceDetailsID = dLayer.SaveData("Pay_PerformanceDetails", "N_PerformanceDetailsID", DetailTable, connection, transaction);
    //                 if (N_PerformanceDetailsID <= 0)
    //                 {
    //                     transaction.Rollback();
    //                     return Ok(api.Error(User, "Unable to save"));

    //                 }

    //                 transaction.Commit();
    //                 SortedList Result = new SortedList();
    //                 Result.Add("N_PerformanceDetailsID", nPerformanceID);
    //                 Result.Add("x_Code", X_Code);
    //                 return Ok(api.Success(Result, "Saved successfully"));

    //             }
    //         }
    //         catch (Exception ex)
    //         {
    //             return Ok(api.Error(User, ex));
    //         }
    //     }

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
                    int n_PerformanceDetailsID = dLayer.SaveData("Pay_PerformanceDetails", "n_PerformanceDetailsID", DetailTable, connection, transaction);
                    if (n_PerformanceDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save ");
                    }

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_PerformanceID", n_PerformanceID);
                    Result.Add("x_Code", x_Code);
                    Result.Add("n_PerformanceDetailsID", n_PerformanceDetailsID);

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