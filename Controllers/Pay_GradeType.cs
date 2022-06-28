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
    [Route("payGradeType")]
    [ApiController]
    public class Pay_GradeType : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

         private readonly int N_FormID = 1445;

        public Pay_GradeType(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult Gradetype(int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and (X_GradeCode like '%" + xSearchkey + "%'or X_GradeCode like '%" + xSearchkey + "%' )";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_GradeID desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from Pay_GradeType where N_CompanyID=@p1" + Searchkey + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from Pay_GradeType where N_CompanyID=@p1" + Searchkey +" and  N_GradeID not in (select top(" + Count + ") N_GradeID from Pay_GradeType where N_CompanyID=@p1 )" + Searchkey + xSortBy;
            Params.Add("@p1", nCompanyId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    sqlCommandCount = "select count(*) as N_Count from Pay_GradeType where N_CompanyID=@p1" + Searchkey;
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
        public ActionResult GradeListDetails(int nGradeID)
        {
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
  
            string Mastersql = "select * from Pay_GradeType where N_CompanyID=@p1 and N_GradeID=@p2";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nGradeID);

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

                    int N_GradeID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_GradeID"].ToString());

                    string DetailSql = "select * from Pay_GradeTypeDetails where N_CompanyID=" + nCompanyID + " and N_GradeID=" + N_GradeID ;

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

 [HttpGet("gradeWiselist")]
        public ActionResult GetGradeWiseList(int nGradeID)
        {
            int nCompanyId= myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from Pay_GradeTypeDetails where N_CompanyID=" + nCompanyId + " and N_GradeID=" + nGradeID ;
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nGradeID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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

   [HttpPost("save")]
        public ActionResult SavePay([FromBody] DataSet ds)
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

                    int nGradeID = myFunctions.getIntVAL(MasterRow["n_GradeID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string xGradeCode = MasterRow["x_GradeCode"].ToString();

                    string x_GradeCode = "";
                    if (xGradeCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                       Params.Add("N_YearID", nFnYearID);
                          Params.Add("N_FormID", this.N_FormID);
                        x_GradeCode = dLayer.GetAutoNumber("Pay_GradeType", "X_GradeCode", Params, connection, transaction);
                        if (x_GradeCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Pay Grade Type");
                        }
                        MasterTable.Rows[0]["x_GradeCode"] = x_GradeCode;
                    }

                      if (MasterTable.Columns.Contains("n_FnYearID"))
                    {

                        MasterTable.Columns.Remove("n_FnYearID");

                    }
                     if (nGradeID>0)
                    {
                        
                         dLayer.DeleteData("Pay_GradeType", "N_GradeID", nGradeID, "N_CompanyID=" + nCompanyID + " and N_GradeID=" + nGradeID, connection, transaction);
                         dLayer.DeleteData("Pay_GradeTypeDetails", "N_GradeID", nGradeID, "N_CompanyID=" + nCompanyID + " and N_GradeID=" + nGradeID, connection, transaction);

                    }
                    int n_GradeID = dLayer.SaveData("Pay_GradeType", "N_GradeID", "", "", MasterTable, connection, transaction);
                    
                    if (n_GradeID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Pay Grade Type");
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_GradeID"] = n_GradeID;
                    }
                    int n_GradeDetailsID = dLayer.SaveData("Pay_GradeTypeDetails", "n_GradeDetailsID", DetailTable, connection, transaction);
                    if (n_GradeDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save ");
                    }

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_GradeID", n_GradeID);
                    Result.Add("x_GradeCode", x_GradeCode);
                    Result.Add("n_GradeDetailsID", n_GradeDetailsID);

                    return Ok(api.Success(Result, "Pay Grade Created"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }




        [HttpDelete("delete")]
        public ActionResult DeleteData(int nGradeID)
        {

             int Results = 0;
            try
            {                        
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();                
                QueryParams.Add("@nFormID", 1445);
                QueryParams.Add("@nGradeID", nGradeID);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Pay_GradeType", "n_GradeID", nGradeID, "", connection,transaction);
                
                    if (Results > 0)
                    {
                        dLayer.DeleteData("Pay_GradeTypeDetails", "n_GradeID", nGradeID, "", connection,transaction);
                        transaction.Commit();
                        return Ok(api.Success("grade type deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User,"Unable to delete "));
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