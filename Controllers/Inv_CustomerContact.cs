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
    [Route("customerContact")]
    [ApiController]
    public class Customer_Contact : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;

         private readonly int FormID;
        private readonly string connectionString;

        public Customer_Contact(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1425;
        }

        [HttpGet("list")]
        public ActionResult List(int nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
             nCompanyId = myFunctions.GetCompanyID(User);
            //string UserPattern = myFunctions.GetUserPattern(User);
             //int nUserID = myFunctions.GetUserID(User);
            
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and (X_Contact like '%" + xSearchkey + "%'or x_Customer like'%" + xSearchkey + "%'or x_Phone like'%" + xSearchkey + "%'or x_Email like'%" + xSearchkey + "%'or x_SalesmanName like'%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by n_contactID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "x_ContactCode":
                        xSortBy = "N_contactID " + xSortBy.Split(" ")[1];
                        break;

                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }


            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_CustomerContact where N_CompanyID=@p1  "  + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Inv_CustomerContact where N_CompanyID=@p1  "  + Searchkey + " and N_ContactID not in (select top(" + Count + ") N_ContactID from vw_Inv_CustomerContact where N_CompanyID=@p1 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);
            //Params.Add("@p3", nFnYearId);


            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_Inv_CustomerContact where N_CompanyID=@p1 " ;
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
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("listDetails")]
        public ActionResult ContactListInner()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select  * from vw_Inv_CustomerContact where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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

            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("details")]
        public ActionResult ContactListDetails(int xContactCode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from vw_Inv_CustomerContact where N_CompanyID=@p1 and x_ContactCode=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", xContactCode);


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
                return Ok(api.Error(User, e));
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
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
                    int ncontactID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ContactID"].ToString());
                     int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    string xContactNo = MasterTable.Rows[0]["x_ContactNo"].ToString();
      

                    if (xContactNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", this.FormID);
                           Params.Add("N_YearID", nFnYearId);

                        
                        
                        xContactNo = dLayer.GetAutoNumber("Inv_Customer_Contacts", "x_ContactNo", Params, connection, transaction);
                        if (xContactNo == "")
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User, "Unable to generate Code"));
                        }
                        MasterTable.Rows[0]["x_ContactNo"] = xContactNo;
                    }

                       if (MasterTable.Columns.Contains("n_FnYearID"))
                    {

                        MasterTable.Columns.Remove("n_FnYearID");

                    }

                       if (MasterTable.Columns.Contains("X_SalesmanName"))
                    {

                        MasterTable.Columns.Remove("X_SalesmanName");

                    }

                    else
                    {
                        dLayer.DeleteData("Inv_Customer_Contacts", "N_ContactID", ncontactID, "", connection, transaction);
                    }
                    ncontactID = dLayer.SaveData("Inv_Customer_Contacts", "N_ContactID", MasterTable, connection, transaction);

                    if (ncontactID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Warning("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("save successfully"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }



        [HttpDelete("delete")]
        public ActionResult DeleteData(int nContactID)
        {

            int Results = 0;
            try
            {
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nFormID",this.FormID);
                QueryParams.Add("@nContactID", nContactID);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("CRM_Contact", "N_ContactID", nContactID, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string, string> res = new Dictionary<string, string>();
                    res.Add("N_ContactID", nContactID.ToString());
                    return Ok(api.Success(res, "Contact deleted"));
                }
                else
                {
                    return Ok(api.Error(User, "Unable to delete Contact"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }



        }
    }
}