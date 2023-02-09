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
    [Route("schBatch")]
    [ApiController]
    public class Sch_Batch : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =166 ;


        public Sch_Batch(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = 
            conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("details")]
        public ActionResult BatchDetails(int n_ClassDivisionID)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_Sch_ClassDivision where N_CompanyID=@p1 and n_ClassDivisionID=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", n_ClassDivisionID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                
                    MasterTable = api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);
                }
                return Ok(api.Success(dt));               
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
                int nDivisionID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ClassDivisionID"].ToString());
                string xclassDivision = MasterTable.Rows[0]["x_classDivision"].ToString();
                int nclassID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_classID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["x_DivisionCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Sch_ClassDivision", "x_DivisionCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Course Code")); }
                        MasterTable.Rows[0]["x_DivisionCode"] = Code;

                      object batchCount = dLayer.ExecuteScalar("select COUNT(*) from Sch_ClassDivision  where N_CompanyID="+ nCompanyID +" and n_classID = "+nclassID + " and  x_ClassDivision='"+xclassDivision+"'", Params, connection, transaction);
                         
                           if (myFunctions.getIntVAL(batchCount.ToString()) > 0){
                             return Ok(api.Error(User, "Batch Already exist"));
                           }
                    }
                    MasterTable.Columns.Remove("n_FnYearId");

                    // if (nDivisionID > 0) 
                    // {  
                    //     dLayer.DeleteData("Sch_ClassDivision", "N_ClassDivisionID", nDivisionID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    // }

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and X_DivisionCode='" + Code + "'";
                    string X_Criteria = "N_CompanyID=" + nCompanyID + "";

                    nDivisionID = dLayer.SaveData("Sch_ClassDivision", "N_ClassDivisionID",DupCriteria,X_Criteria, MasterTable, connection, transaction);
                    if (nDivisionID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Batch Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        [HttpGet("list") ]
        public ActionResult BatchList(int nCompanyID,int nClassID)
        {    
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="";

            if(nClassID>0)
                sqlCommandText="select * from vw_Sch_ClassDivision where N_CompanyID=@p1 and N_ClassID=@p2";
            else    
                sqlCommandText="select * from vw_Sch_ClassDivision where N_CompanyID=@p1";

            param.Add("@p1", nCompanyID);  
            param.Add("@p2", nClassID);                
                
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
                }
                // if(dt.Rows.Count==0)
                // {
                //     return Ok(api.Notice("No Results Found"));
                // }
                // else
                // {
                    return Ok(api.Success(dt));
                //}
                
            }
            catch(Exception e)
            {
                return Ok(api.Error(User,e));
            }   
        }   
      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nClassDivisionID)
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
                    Results = dLayer.DeleteData("Sch_ClassDivision ", "n_ClassDivisionID", nClassDivisionID, "N_CompanyID =" + nCompanyID, connection, transaction);                
                
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("Batch deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User,"Unable to delete Batch"));
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

