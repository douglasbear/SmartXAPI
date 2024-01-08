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
    [Route("genlookup")]
    [ApiController]
    public class Gen_Lookup : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        public Gen_Lookup(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        
        [HttpGet("listDetails")]
        public ActionResult OpportunityListDetails(int nFnYearId,int nPkeyId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
  
            string sqlCommandText = "SELECT Sub.*, Parent.X_Name AS X_ParentName FROM Gen_LookupTable AS Sub LEFT OUTER JOIN Gen_LookupTable AS Parent ON Sub.N_CompanyID = Parent.N_CompanyID AND Sub.N_ParentGroupID = Parent.N_PkeyId where Sub.N_CompanyID=@p1 and Sub.N_PkeyId=@p3";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", nPkeyId);

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
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nPkeyId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PkeyId"].ToString());
                int N_FormID =  myFunctions.getIntVAL(MasterTable.Rows[0]["n_ReferId"].ToString());
                int nSort = 0;
                if(MasterTable.Columns.Contains("n_Sort"))
                 nSort = myFunctions.getIntVAL(MasterTable.Rows[0]["n_Sort"].ToString());

                if(N_FormID==1155)
                {
                    N_FormID=455;
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                     Params.Add("@nReferId",N_FormID);
                     Params.Add("@nSort",nSort);
                    // Auto Gen
                    string PkeyCode = "";
                    var values = MasterTable.Rows[0]["X_PkeyCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", N_FormID);
                        PkeyCode = dLayer.GetAutoNumber("Gen_LookupTable", "X_PkeyCode", Params, connection, transaction);
                        if (PkeyCode == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate PkeyCode Code")); }
                        MasterTable.Rows[0]["X_PkeyCode"] = PkeyCode;
                    }
                    else
                    {
                        dLayer.DeleteData("Gen_LookupTable", "N_PkeyId", nPkeyId, "", connection, transaction);
                    }
                    int Count = 0;
                    if(nSort>0){       
                    object SeqNo = dLayer.ExecuteScalar("select count(n_Sort) from Gen_LookupTable where N_ReferId=@nReferId and N_Sort=@nSort", Params,connection,transaction);
                    Count = myFunctions.getIntVAL(SeqNo.ToString());
                    }
                    
                    if (Count == 0 )
                    {
                   nPkeyId = dLayer.SaveData("Gen_LookupTable", "N_PkeyId", MasterTable, connection, transaction);
                    
                    }
                     else
                    {

                        transaction.Rollback();
                        return Ok(api.Error(User,"Seq No Already Exists"));
                    }
                                    
                
                    if (nPkeyId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                   
                        transaction.Commit();
                        return Ok(api.Success("Successfully saved"));

                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPkeyId,int nCompanyID, int nFnYearID,int formID)
        {

             int Results = 0;
            try
            {                        
                SortedList Params = new SortedList();
                SortedList QueryParams = new SortedList();                
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nFormID", 1305);
                QueryParams.Add("@nPkeyId", nPkeyId);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (myFunctions.getBoolVAL(myFunctions.checkProcessed("Acc_FnYear", "B_YearEndProcess", "N_FnYearID", "@nFnYearID", "N_CompanyID=@nCompanyID ", QueryParams, dLayer, connection)))
                        return Ok(api.Error(User,"Year is closed, Cannot create new Entry..."));
                    SqlTransaction transaction = connection.BeginTransaction();

                    if(formID==1658){
                      object count = dLayer.ExecuteScalar("Select count(1) from Pay_EmpBussinessTripRequest Where  N_CompanyID= " + nCompanyID + " and N_TravelTypeID=" + nPkeyId, connection, transaction);
                          if(myFunctions.getIntVAL(count.ToString())>0){
                         return Ok(api.Warning("Unable To delete"));
                    }

                    }

                    Results = dLayer.DeleteData("Gen_LookupTable", "N_PkeyId", nPkeyId, "", connection, transaction);
                    transaction.Commit();
                }
                if (Results > 0)
                {
                    Dictionary<string,string> res=new Dictionary<string, string>();
                    res.Add("N_PkeyId",nPkeyId.ToString());
                    return Ok(api.Success(res,"Entry deleted"));
                }
                else
                {
                    return Ok(api.Error(User,"Unable to delete Entry"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }


        }

        [HttpGet("SeqNo")]
        public ActionResult GetSeqNo()
        {
            DataTable dt=new DataTable();
            
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
        
            Params.Add("@nReferId",1310);
            Params.Add("@nCompanyID",nCompanyID);
            int N_Sort =0; 
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object SeqNo = dLayer.ExecuteScalar("select MAX(isnull(n_Sort,0)) as SeqNo from Gen_LookupTable where N_ReferId=@nReferId and N_CompanyID=@nCompanyID", Params,connection);
                         
                if (SeqNo == null)
                {
                   N_Sort=1;
                }
                else
                {
                    N_Sort=myFunctions.getIntVAL(SeqNo.ToString())+1;
                }
                  
                          SortedList Result = new SortedList();
                          Result.Add("n_Sort", N_Sort);
                          return Ok(api.Success(Result));
                }
            }
            catch(Exception e)
            {

                return Ok(api.Error(User,e));
            }
        }


        
    }
}