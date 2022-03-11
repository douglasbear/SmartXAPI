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


         [HttpGet("list")]
        public ActionResult GetAsnReceipt(int nComapanyId, int nFnYearId,int nBranchID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,bool bAllBranchData)
        {
            
            int nCompanyId = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string sqlCondition = "";
             string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_AsnDocNo like '%" + xSearchkey + "%' OR N_AsnID like '%" + xSearchkey + "%')";

             if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_AsnID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_AsnDocNo":
                        xSortBy = "X_AsnDocNo " + xSortBy.Split(" ")[1];
                        break;
                    case "N_AsnID":
                        xSortBy = "N_AsnID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nFnYearId", nFnYearId);
            Params.Add("@FormID", FormID);
             Params.Add("@nBranchID", nBranchID);
        
            SortedList OutPut = new SortedList();
    

           
         try
            {
              using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (bAllBranchData)
                        sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId";
                    else
                        sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and N_BranchID=@nBranchID";


                    if (Count == 0)
                        sqlCommandText = "select  top(" + nSizeperpage + ") * from vw_AsnReceipt where " + sqlCondition + " " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select  top(" + nSizeperpage + ") * from vw_AsnReceipt where " + sqlCondition + " " + Searchkey + " and N_AsnID not in (select top(" + Count + ") N_AsnID from vw_AsnReceipt where " + sqlCondition + " " + xSortBy + " ) " + xSortBy;


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count from vw_AsnReceipt where " + sqlCondition + " " + Searchkey + "";
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
                    SortedList Params = new SortedList();

                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int nAsnID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AsnID"].ToString());
                    int N_CustomerID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CustomerID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
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

                       if (nAsnID > 0)
                    {
                        dLayer.DeleteData("Wh_AsnDetails", "N_AsnID", nAsnID, "N_CompanyID=" + nCompanyID + " and N_AsnID=" + nAsnID, connection, transaction);
                        dLayer.DeleteData("Wh_AsnMaster", "N_AsnID", nAsnID, "N_CompanyID=" + nCompanyID + " and N_AsnID=" + nAsnID, connection, transaction);
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
                        DetailTable.Rows[j]["N_AsnID"] = nAsnID;
                    }

                     int N_AsnDetailsID = dLayer.SaveData("Wh_AsnDetails", "N_AsnDetailsID", DetailTable, connection, transaction);
                    if (N_AsnDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));

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



    }
}