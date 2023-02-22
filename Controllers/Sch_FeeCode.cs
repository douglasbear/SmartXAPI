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

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("schFeeCode")]
    [ApiController]
    public class Sch_FeeCodes : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;

        public Sch_FeeCodes(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1056;
        }
        private readonly string connectionString;

        [HttpGet("list")]
        public ActionResult FeeCodeList()
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();



            string sqlCommandText = "select  * from [VW_SCHFEECODES] where N_CompanyID=@p1";

            Params.Add("@p1", nCompanyId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
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
                int nFeeCodeID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FeeCodeID"].ToString());
                int nLocationID=  myFunctions.getIntVAL(MasterTable.Rows[0]["N_LocationID"].ToString());
                int nItemID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ItemID"].ToString());
                if(MasterTable.Columns.Contains("N_LocationID"))
                      MasterTable.Columns.Remove("N_LocationID");


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_FeeCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("nItemID",nItemID);
                        Code = dLayer.GetAutoNumber("Sch_FeeCodes", "X_FeeCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(_api.Error(User,"Unable to generate Course Code")); }
                        MasterTable.Rows[0]["X_FeeCode"] = Code;
                        
                    object descCount = dLayer.ExecuteScalar("Select count(1) From Sch_FeeCodes Where X_FeeDescription ='" + MasterTable.Rows[0]["X_FeeDescription"].ToString() + "' and N_CompanyID= " + nCompanyID , Params, connection, transaction);
                     
                       if (myFunctions.getIntVAL(descCount.ToString()) >0)
                        {
                             transaction.Rollback();
                             return Ok(_api.Error(User, "fee code already exist"));
                        }
                    }
                    MasterTable.Columns.Remove("n_FnYearId");

                    if (nFeeCodeID> 0) 
                    {  
                        object Count = dLayer.ExecuteScalar("select count(1) from Sch_ClassFeeSetup where N_FeeCodeID="+nFeeCodeID+" and N_CompanyID="+nCompanyID+"", Params, connection,transaction); 
                        if (myFunctions.getIntVAL(Count.ToString()) >0)
                        {
                             transaction.Rollback();
                             return Ok(_api.Error(User, "transaction Started"));
                        }


                         dLayer.DeleteData("inv_itemMaster", "N_ItemID", nItemID,"N_CompanyID =" + nCompanyID,connection,transaction);
                        dLayer.DeleteData("inv_itemUnit", "N_ItemID", nItemID,"N_CompanyID =" + nCompanyID,connection,transaction);
                        dLayer.DeleteData("Inv_ItemMasterWHLink", "N_ItemID", nItemID,"N_CompanyID =" + nCompanyID,connection,transaction); 
                        dLayer.DeleteData("Sch_FeeCodes", "n_FeeCodeID", nFeeCodeID, "N_CompanyID =" + nCompanyID, connection, transaction);   
                                           
                    }

                    nFeeCodeID = dLayer.SaveData("Sch_FeeCodes", "n_FeeCodeID", MasterTable, connection, transaction);
                    if (nFeeCodeID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    


                    SortedList ProductParams = new SortedList();
                    ProductParams.Add("@N_CompanyID", nCompanyID);
                    ProductParams.Add("@N_FeeCodeID", nFeeCodeID);
                    ProductParams.Add("@N_LocationID", nLocationID);
              
                    dLayer.ExecuteNonQueryPro("SP_ProductCreation", ProductParams, connection, transaction);

                  
                       transaction.Commit();
                    SortedList Result = new SortedList();

                    return Ok(_api.Success(Result, "Fee Setup Saved"));


              
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }
        
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nFeeCodeID,int nItemID)
        {

            int Results = 0;
            int nCompanyID=myFunctions.GetCompanyID(User);
            try
            {                        
                SortedList Params = new SortedList();
              
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();


                  Params.Add("@nCompanyID", nCompanyID);  
                  Params.Add("@nFeeCodeID", nFeeCodeID);
                  Params.Add("@nItemID",nItemID);


                    object Count = dLayer.ExecuteScalar("select count(1) from Sch_ClassFeeSetup where N_FeeCodeID=@nFeeCodeID and N_CompanyID=@nCompanyID", Params, connection,transaction); 
                      
                        if (myFunctions.getIntVAL(Count.ToString()) >0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Delete"));
                    }

                    object BusCount = dLayer.ExecuteScalar("select count(1) from Sch_BusRoute where N_FeeCodeID=@nFeeCodeID and N_CompanyID=@nCompanyID", Params, connection,transaction); 
                    
                              if (myFunctions.getIntVAL(BusCount.ToString()) >0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Delete"));
                    }

                    Results = dLayer.DeleteData("Sch_FeeCodes", "n_FeeCodeID", nFeeCodeID, "N_CompanyID =" + nCompanyID, connection, transaction);
        
                
                    if (Results > 0)
                    {
                       dLayer.DeleteData("inv_itemMaster", "N_ItemID", nItemID,"N_CompanyID =" + nCompanyID,connection,transaction);
                       dLayer.DeleteData("inv_itemUnit", "N_ItemID", nItemID,"N_CompanyID =" + nCompanyID,connection,transaction);
                       dLayer.DeleteData("Inv_ItemMasterWHLink", "N_ItemID", nItemID,"N_CompanyID =" + nCompanyID,connection,transaction);

                       transaction.Commit();
                        return Ok(_api.Success("deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete FeeCode"));
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("details")]
        public ActionResult ClassDetails(string xFeeCode)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from VW_SCHFEECODES where N_CompanyID=@p1  and X_FeeCode=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", xFeeCode);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                
                    MasterTable = _api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);
                }
                return Ok(_api.Success(dt));               
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }
          [HttpGet("typeList")]
        public ActionResult TypeList()
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();



            string sqlCommandText = "select  * from [Sch_FeeCategory] where N_CompanyID=@p1";

            Params.Add("@p1", nCompanyId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
            [HttpGet("freequencyList")]
        public ActionResult FreequencyList()
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();



            string sqlCommandText = "select  * from [Sch_Frequency] where N_CompanyID=@p1";

            Params.Add("@p1", nCompanyId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


          [HttpGet("dashboardList")]
        public ActionResult GetAssignmentList(int? nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
            
                Searchkey = "and (N_FeeCodeID like '%" + xSearchkey + "%' OR X_FeeCode like '%" + xSearchkey + "%' OR X_FeeDescription like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_FeeCodeID desc";
                
       
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from VW_SCHFEECODES where N_CompanyID=@nCompanyID " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from VW_SCHFEECODES where N_CompanyID=@nCompanyID " + Searchkey + " " + xSortBy;

            Params.Add("@nCompanyID", nCompanyID);
           

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(1) as N_Count  from VW_SCHFEECODES where N_CompanyID=@nCompanyID " + Searchkey + " ";
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







    }
}





