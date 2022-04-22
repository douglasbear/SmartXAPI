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
    [Route("gradetype")]
    [ApiController]
    public class Pay_GradeType : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Pay_GradeType(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("list")]
        public ActionResult Gradetype(int ngradeId,int nPage,int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText ="";

            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = " and (X_GradeCode like '%" + xSearchkey + "%'or X_GradeCode like '%" + xSearchkey + "%' )";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_GradeID desc";
            else
                xSortBy = " order by " + xSortBy;
            
            Params.Add("@p1", nCompanyId);
            Params.Add("@p3", ngradeId);
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
        [HttpGet("listDetails")]
        public ActionResult GetGradeListDetails( int ngradeId)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            DataSet dt = new DataSet();
            SortedList Params = new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
         

            string Mastersql = "";

           
            Mastersql = "select * from Pay_GradeTypeDetails where N_CompanyID=@p1 and N_GradeID=@p2";
             
            
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", ngradeId);
        
            

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Data Found !!"));
                    }

                    MasterTable = api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                   
                    int N_GradeID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_GradeID"].ToString());

                    string DetailSql = "";
                    DetailSql = "Select * from Pay_GradeTypeDetails  Where N_CompanyID=@p1 and   N_GradeID=" + N_GradeID;
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
        [HttpGet("details")]
        public ActionResult GradeListDetails(string xgradecode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
  
            string sqlCommandText = "select * from Pay_GradeTypeDetails where N_CompanyID=@p1 and X_GradeCode=@p2";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", xgradecode);


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



        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
               
                int nGradeID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_GradeID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string LeadCode = "";
                    var values = MasterTable.Rows[0]["X_GradeCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", 1445);
                        LeadCode = dLayer.GetAutoNumber("Pay_GradeType", "x_GradeCode", Params, connection, transaction);
                        if (LeadCode == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Grade Code")); }
                        MasterTable.Rows[0]["X_GradeCode"] = LeadCode;
                    }

                    nGradeID = dLayer.SaveData("Pay_GradeType", "N_GradeID",  MasterTable, connection, transaction);
                    if (nGradeID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Company Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }











         [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                SortedList Params = new SortedList();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;
                    DataRow MasterRow = MasterTable.Rows[0];
                    transaction = connection.BeginTransaction();


                    int N_GradeID = myFunctions.getIntVAL(MasterRow["N_GradeID"].ToString());   
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
         
                    string X_Trasnaction = "";
                    int N_FormID =0;



                    // Auto Gen
                    string GradeId = "";
                    var values = MasterTable.Rows[0]["X_GradeCode"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", Master["n_CompanyId"].ToString());
                        Params.Add("N_FormID", N_FormID);
                   

                        GradeId = dLayer.GetAutoNumber("Pay_GradeType", "X_GradeCode", Params, connection, transaction);
                        if (GradeId == "") { transaction.Rollback(); return Ok(api.Error(User,"Unable to generate")); }
                        MasterTable.Rows[0]["X_GradeCode"] = AdjuGradeIdstmentNo;

                    }
                    else
                    {
                        if (N_GradeID > 0)
                        {
                            SortedList DeleteParams = new SortedList(){
                                {"N_CompanyID",N_CompanyID},
                                {"N_GradeID",N_GradeID}};
                            dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                        }
                    }

                    N_GradeID = dLayer.SaveData("Inv_BalanceAdjustmentMaster", "N_GradeID", MasterTable, connection, transaction);
                    if (N_GradeID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save Adjustment"));
                    }
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_GradeID"] = N_GradeID;
                    }

                    int N_AdjustmentDetailsId = dLayer.SaveData("Inv_BalanceAdjustmentMasterDetails", "N_AdjustmentDetailsId", DetailTable, connection, transaction);
                    if (N_AdjustmentDetailsId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save Adjustment"));
                    }
                    else
                    {
                        SortedList PostingParams = new SortedList(){
                                {"N_CompanyID",N_CompanyID},
                                {"X_InventoryMode",X_Trasnaction},
                                {"N_InternalID",N_GradeID},
                                {"N_UserID",myFunctions.GetUserID(User)}
                                };
                            dLayer.ExecuteNonQueryPro("SP_Acc_InventoryPosting", PostingParams, connection, transaction);
                            
                        transaction.Commit();
                    }
                    return Ok(api.Success("Saved" + ":" + N_GradeID));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

      
        // [HttpDelete("delete")]
        // public ActionResult DeleteData(int nGradeID)
        // {

        //      int Results = 0;
        //     try
        //     {                        
        //         SortedList Params = new SortedList();
        //         SortedList QueryParams = new SortedList();                
        //         QueryParams.Add("@nFormID", 1445);
        //         QueryParams.Add("@nGradeID", nGradeID);

        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();
        //             Results = dLayer.DeleteData("Pay_GradeType", "nGradeID", nGradeID, "", connection, transaction);
        //             transaction.Commit();
        //         }
        //         if (Results > 0)
        //         {
        //             Dictionary<string,string> res=new Dictionary<string, string>();
        //             res.Add("N_GradeID",nGradeID.ToString());
        //             return Ok(api.Success(res," deleted"));
        //         }
        //         else
        //         {
        //             return Ok(api.Error(User,"Unable to delete "));
        //         }

        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(api.Error(User,ex));
        //     }



        // }
    }
}
