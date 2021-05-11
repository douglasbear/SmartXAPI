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
    [Route("accruedtypes")]
    [ApiController]
    public class Pay_AccruedTypes : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IApiFunctions _api;
        private readonly string connectionString;
        private readonly int N_FormID = 587;

        public Pay_AccruedTypes(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }



        //  [HttpGet("list")]
        // public ActionResult PayAccruedList(int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        // {
        //     int nCompanyId=myFunctions.GetCompanyID(User);
        //     DataTable dt = new DataTable();
        //     SortedList Params = new SortedList();
        //     int Count= (nPage - 1) * nSizeperpage;
        //     string sqlCommandText ="";
        //     string Searchkey = "";
        //     if (xSearchkey != null && xSearchkey.Trim() != "")
        //         Searchkey = "and (x_VacCode like '%" + xSearchkey + "%'or x_VacType like'%" + xSearchkey + "%' or x_Type like '%" + xSearchkey + "%' or x_Period like '%" + xSearchkey + "%' or x_Description like '%" + xSearchkey + "%' )";

        //     if (xSortBy == null || xSortBy.Trim() == "")
        //         xSortBy = " order by N_VacTypeID desc";
        //     else
        //         xSortBy = " order by " + xSortBy;

        //      if(Count==0)
        //         sqlCommandText = "select top("+ nSizeperpage +") X_VacCode,X_VacType,X_Type,X_Period,X_Description from Pay_VacationType where N_CompanyID=@p1 " + Searchkey + " " + xSortBy;
        //     else
        //         sqlCommandText = "select top("+ nSizeperpage +") X_VacCode,X_VacType,X_Type,X_Period,X_Description,N_VacTypeID from Pay_VacationType where N_CompanyID=@p1 " + Searchkey + " and N_VacTypeID not in (select top("+ Count +") N_VacTypeID from Pay_VacationType where N_CompanyID=@p1 "+Searchkey + xSortBy + " ) " + xSortBy;
        //     Params.Add("@p1", nCompanyId);

        //     SortedList OutPut = new SortedList();


        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

        //             string sqlCommandCount = "select count(*) as N_Count  from Pay_VacationType where N_CompanyID=@p1 ";
        //             object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
        //             OutPut.Add("Details", api.Format(dt));
        //             OutPut.Add("TotalCount", TotalCount);
        //             if (dt.Rows.Count == 0)
        //             {
        //                 return Ok(api.Warning("No Results Found"));
        //             }
        //             else
        //             {
        //                 return Ok(api.Success(OutPut));
        //             }

        //         }

        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(api.Error(e));
        //     }
        // }

        [HttpGet("Dashboardlist")]
        public ActionResult PayAccruedList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")

                Searchkey = "and (x_VacCode like '%" + xSearchkey + "%'or x_VacType like'%" + xSearchkey + "%' or x_Type like '%" + xSearchkey + "%' or x_Period like '%" + xSearchkey + "%' or x_Description like '%" + xSearchkey + "%' )";
            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_VacTypeID desc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_PayVacationType where N_CompanyID=@p1 ";
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_PayVacationType where N_CompanyID=@p1  and N_VacTypeID not in (select top(" + Count + ") N_VacTypeID from vw_PayVacationType where N_CompanyID=@p1 )";
            Params.Add("@p1", nCompanyId);


            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_PayVacationType where N_CompanyID=@p1 ";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return BadRequest(_api.Error(e));
            }
        }


        [HttpGet("VactionList")]
        public ActionResult AccruedTypeList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select N_CompanyID,X_VacCode,X_VacType,N_VacTypeID from Pay_VacationType where N_CompanyID=@nComapnyID and X_Type<>'T'";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
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
                return Ok(_api.Error(e));
            }
        }





        [HttpGet("details")]
        public ActionResult PayAccruedDetails(string xVacCode, int nVacTypeID)
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
                    Params.Add("@xVacCode", xVacCode);
                    Mastersql = "select * from vw_PayVacationType where N_CompanyId=@nCompanyID and x_VacCode=@xVacCode  ";

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int VacTypeID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_VacTypeID"].ToString());
                    Params.Add("@nVacTypeID", VacTypeID);

                    MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from Pay_VacationTypeDetails where N_CompanyId=@nCompanyID and N_VacTypeID=@nVacTypeID ";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }





        [HttpPost("Save")]
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

                    int n_VacTypeID = myFunctions.getIntVAL(MasterRow["N_VacTypeID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string x_VacCode = MasterRow["X_VacCode"].ToString();
                    if (n_VacTypeID > 0)
                    {
                        object objVacationStarted;
                         MasterTable.Columns.Remove("n_FnYearId");

                        objVacationStarted = dLayer.ExecuteScalar("select 1 FRom Pay_VacationDetails Where N_VacTypeID= " + n_VacTypeID + " and N_CompanyID= " + N_CompanyID + " and N_FnYearID=" + N_FnYearID, connection, transaction);
                        if (objVacationStarted != null)
                        {

                            return Ok(_api.Error("Transaction started!!!"));


                        }
                    }


                    if (x_VacCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", N_FormID);
                        x_VacCode = dLayer.GetAutoNumber("Pay_VacationType", "X_VacCode", Params, connection, transaction);
                        if (x_VacCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Accrual Code");
                        }
                        MasterTable.Rows[0]["X_VacCode"] = x_VacCode;
                        MasterTable.Columns.Remove("n_FnYearId");
                    }
                  

                    n_VacTypeID = dLayer.SaveData("Pay_VacationType", "n_VacTypeID", "", "", MasterTable, connection, transaction);
                    if (n_VacTypeID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Accrual Code");
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_VacTypeID"] = n_VacTypeID;
                    }
                    int n_VacTypeDetailsID = dLayer.SaveData("Pay_VacationTypeDetails", "n_VacTypeDetailsID", DetailTable, connection, transaction);
                    if (n_VacTypeDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Accrual Code");
                    }

                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_VacTypeID", n_VacTypeID);
                    Result.Add("x_VacCode", x_VacCode);
                    Result.Add("n_VacTypeDetailsID", n_VacTypeDetailsID);

                    return Ok(_api.Success(Result, " Accrual Code Created"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }



        [HttpDelete("delete")]
        public ActionResult DeleteData(int nVacTypeID, int nCompanyID, int fnYearID)

        {

            int Results = 0;
            object objVacationStarted;
            object objAssigned;

            try
            {
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    objAssigned = dLayer.ExecuteScalar("select 1 FRom Pay_EmpAccruls Where N_VacTypeID= " + nVacTypeID + " and N_CompanyID=" + nCompanyID + "", connection, transaction);
                    objVacationStarted = dLayer.ExecuteScalar("select 1 FRom Pay_VacationDetails Where N_VacTypeID= " + nVacTypeID + " and N_CompanyID= " + nCompanyID + " and N_FnYearID=" + fnYearID, connection, transaction);
                    if (objVacationStarted != null)
                    {

                        return Ok(_api.Error("Transaction started cannot delete Accrual Code"));


                    }
                    else if (objAssigned != null)
                    {
                        return Ok(_api.Error("can't delete Accrual Code"));

                    }
                    else
                    {

                        Results = dLayer.DeleteData("Pay_VacationType", "N_VacTypeID", nVacTypeID, "", connection, transaction);
                        transaction.Commit();
                    }
                }
                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("N_VacTypeID", nVacTypeID.ToString());
                    return Ok(api.Success(res, "Accrual Code deleted"));
                }
                else
                {
                    return Ok(api.Error("Unable to delete Accrual Code"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }



        }
    }
}