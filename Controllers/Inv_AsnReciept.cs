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
    [Route("invAsnReciept")]
    [ApiController]

    public class Inv_AsnReciept : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public Inv_AsnReciept(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1406;

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
                    SortedList Params = new SortedList();

                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int nAsnID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AsnID"].ToString());
                    int N_CustomerID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CustomerID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    int N_BranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_BranchID"].ToString());
                    string X_AsnDocNo = "";
                    var values = MasterTable.Rows[0]["X_AsnDocNo"].ToString();

                       if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", 370);
                        Params.Add("N_YearID", N_FnYearID);


                        X_AsnDocNo = dLayer.GetAutoNumber("Wh_AsnMaster", "X_AsnDocNo", Params, connection, transaction);
                        if (X_AsnDocNo == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Warning("Unable to generate"));
                        }
                        MasterTable.Rows[0]["X_AsnDocNo"] = X_AsnDocNo;
                    }

                     nAsnID = dLayer.SaveData("Wh_AsnMaster", "N_AsnID", MasterTable, connection, transaction);

                      if (nAsnID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }

                    

                    //Inv_ItemMaster Creation
                    
                    

                     for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_AsnDetailsID"] = nAsnID;
                    }

                     int N_AsnDetailsID = dLayer.SaveData("Wh_AsnDetails", "N_AsnDetailsID", DetailTable, connection, transaction);
                    if (N_AsnDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));

                    }

                     SortedList ProductParams = new SortedList();
                            ProductParams.Add("N_CompanyID", nCompanyID);
                            ProductParams.Add("N_ASNID", nAsnID);
                            ProductParams.Add("N_FnYearID", N_FnYearID);
                            ProductParams.Add("N_BranchID", N_BranchID);
                            try
                            {
                                dLayer.ExecuteNonQueryPro("SP_ASNProductInsert", ProductParams, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, ex));
                            }


                     transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("N_AsnDetailsID", nAsnID);
                    Result.Add("X_AsnDocNo", X_AsnDocNo);
                    return Ok(_api.Success(Result, "Saved successfully"));
                }
            }
               catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
          }

       
          [HttpGet("details")]
        public ActionResult GetDetails(string xAsnDocNo, int nFnYearID, int nBranchID, bool bShowAllBranchData)
        {
            DataTable Master = new DataTable();
            DataTable Detail = new DataTable();
            DataSet ds = new DataSet();
            SortedList Params = new SortedList();
            SortedList QueryParams = new SortedList();

            int companyid = myFunctions.GetCompanyID(User);

            QueryParams.Add("@nCompanyID", companyid);
            QueryParams.Add("@xAsnDocNo", xAsnDocNo);
            QueryParams.Add("@nBranchID", nBranchID);
            QueryParams.Add("@nFnYearID", nFnYearID);
            string Condition = "";
            string _sqlQuery = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (bShowAllBranchData == true)
                        Condition = "n_Companyid=@nCompanyID and X_AsnDocNo =@xAsnDocNo ";
                    else
                        Condition = "n_Companyid=@nCompanyID and X_AsnDocNo =@xAsnDocNo and N_BranchID=@nBranchID";


                    _sqlQuery = "Select * from vw_Wh_AsnMaster_Disp Where " + Condition + "";

                    Master = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                    Master = _api.Format(Master, "master");

                    if (Master.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                         QueryParams.Add("@N_AsnID", Master.Rows[0]["N_AsnID"].ToString());

                        // QueryParams.Add("@N_AsnDetailsID",N_AsnDetailsID);


                        ds.Tables.Add(Master);
        

                        _sqlQuery = "Select * from vw_Wh_AsnDetails_Disp Where N_CompanyID=@nCompanyID and N_AsnID=@N_AsnID";
                        Detail = dLayer.ExecuteDataTable(_sqlQuery, QueryParams, connection);

                        Detail = _api.Format(Detail, "details");
                        if (Detail.Rows.Count == 0)
                        {
                            return Ok(_api.Notice("No Results Found"));
                        }
                        ds.Tables.Add(Detail);

                        return Ok(_api.Success(ds));
                    }


                }


            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
       
       
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nAsnID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    dLayer.DeleteData("Wh_AsnDetails", "N_AsnID", nAsnID, "N_CompanyID=" + nCompanyID + " and N_AsnID=" + nAsnID, connection, transaction);
                    Results = dLayer.DeleteData("Wh_AsnMaster", "N_AsnID", nAsnID, "N_CompanyID=" + nCompanyID + " and N_AsnID=" + nAsnID, connection, transaction);

                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("deleted Successfully"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("Unable to delete Request"));
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