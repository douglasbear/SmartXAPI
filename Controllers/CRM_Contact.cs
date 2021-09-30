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
    [Route("contact")]
    [ApiController]
    public class CRM_Contact : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public CRM_Contact(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        // [HttpGet("list")]
        // public ActionResult ContactList(int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        // {
        //     DataTable dt = new DataTable();
        //     SortedList Params = new SortedList();
        //     int nCompanyId=myFunctions.GetCompanyID(User);
        //     string sqlCommandCount = "";
        //     int Count= (nPage - 1) * nSizeperpage;
        //     string sqlCommandText ="";
        //     string Searchkey = "";
        //     if (xSearchkey != null && xSearchkey.Trim() != "")
        //         Searchkey = "and (X_Contact like '%" + xSearchkey + "%'or X_ContactCode like'%" + xSearchkey + "%'or X_Department like'%" + xSearchkey + "%')";

        //     if (xSortBy == null || xSortBy.Trim() == "")
        //         xSortBy = " order by n_contactID desc";
        //     else
        //     {
        //                 switch (xSortBy.Split(" ")[0])
        //                 {
        //                     case "x_ContactCode":
        //                         xSortBy = "N_contactID " + xSortBy.Split(" ")[1];
        //                         break;

        //                     default: break;
        //                 }
        //         xSortBy = " order by " + xSortBy;
        //                 }

        //      if(Count==0)
        //         sqlCommandText = "select top("+ nSizeperpage +") * from vw_CRMContact where N_CompanyID=@p1 " + Searchkey + " " + xSortBy;
        //     else
        //         sqlCommandText = "select top("+ nSizeperpage +") * from vw_CRMContact where N_CompanyID=@p1 " + Searchkey + " and N_ContactID not in (select top("+ Count +") N_ContactID from vw_CRMContact where N_CompanyID=@p1 "+Searchkey + xSortBy + " ) " + xSortBy;
        //     Params.Add("@p1", nCompanyId);

        //     SortedList OutPut = new SortedList();


        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

        //             sqlCommandCount = "select count(*) as N_Count  from vw_CRMContact where N_CompanyID=@p1 "+Searchkey;
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
        //         return Ok(api.Error(User,e));
        //     }
        // }
        [HttpGet("list")]
        public ActionResult OpportunityList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int nCompanyId = myFunctions.GetCompanyID(User);
            string UserPattern = myFunctions.GetUserPattern(User);
            string Pattern = "";
            if (UserPattern != "")
            {
                Pattern = " and Left(X_Pattern,Len(@p2))=@p2";
                Params.Add("@p2", UserPattern);
            }
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_Contact like '%" + xSearchkey + "%'or X_ContactCode like'%" + xSearchkey + "%'or X_Department like'%" + xSearchkey + "%'or x_Phone like'%" + xSearchkey + "%')";

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
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMContact where N_CompanyID=@p1 " + Pattern + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_CRMContact where N_CompanyID=@p1 " + Pattern + Searchkey + " and N_ContactID not in (select top(" + Count + ") N_ContactID from vw_CRMContact where N_CompanyID=@p1 " + xSortBy + " ) " + xSortBy;
            Params.Add("@p1", nCompanyId);

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_CRMContact where N_CompanyID=@p1" + Pattern;
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

            string sqlCommandText = "select  * from vw_CRMContact where N_CompanyID=@p1";
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

            string sqlCommandText = "select * from vw_CRMContact where N_CompanyID=@p1 and x_ContactCode=@p2";
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
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nContactID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ContactID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string ContactCode = "";
                    var values = MasterTable.Rows[0]["x_ContactCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", 1308);
                        ContactCode = dLayer.GetAutoNumber("CRM_Contact", "x_ContactCode", Params, connection, transaction);
                        if (ContactCode == "") { transaction.Rollback(); return Ok(api.Error(User, "Unable to generate Contact Code")); }
                        MasterTable.Rows[0]["x_ContactCode"] = ContactCode;
                    }
                    if (MasterTable.Columns.Contains("X_SalesmanName"))
                    {

                        MasterTable.Columns.Remove("X_SalesmanName");

                    }


                    nContactID = dLayer.SaveData("CRM_Contact", "n_ContactID", MasterTable, connection, transaction);
                    if (nContactID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Contact Created"));
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
                QueryParams.Add("@nFormID", 1308);
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