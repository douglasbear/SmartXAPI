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
    [Route("schBusRoute")]
    [ApiController]
    public class Sch_BusRoute : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =741 ;


        public Sch_BusRoute(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = 
            conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("details")]
        public ActionResult BusRouteDetails(int nRouteID)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_Sch_BusRouteDisp where N_CompanyID=@p1  and n_RouteID=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", nRouteID);
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

                    string DetailSql = "select * from Sch_BusRouteDetail where N_CompanyID=@p1 and n_RouteID=@p2";

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



        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nRouteID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_RouteID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_RouteNo"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Sch_BusRoute", "X_RouteNo", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Route Code")); }
                        MasterTable.Rows[0]["X_RouteNo"] = Code;
                    }
                    MasterTable.Columns.Remove("n_FnYearId");

                    if (nRouteID > 0) 
                    {  
                        dLayer.DeleteData("Sch_BusRouteDetail", "N_RouteID", nRouteID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                        dLayer.DeleteData("Sch_BusRoute", "N_RouteID", nRouteID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    }

                    nRouteID = dLayer.SaveData("Sch_BusRoute", "N_RouteID", MasterTable, connection, transaction);
                    if (nRouteID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_RouteID"] = nRouteID;
                    }
                    int nRouteDetailID = dLayer.SaveData("Sch_BusRouteDetail", "N_RouteDetailID", DetailTable, connection, transaction);
                    if (nRouteDetailID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save ");
                    }
                    transaction.Commit();
                    return Ok(api.Success("Route Created"));

                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        [HttpGet("list") ]
        public ActionResult RouteList(int nCompanyID,int nBusID)
        {    
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="";

            if(nBusID>0)
                sqlCommandText="select * from vw_Sch_BusRouteDisp where N_CompanyID=@p1 and N_BusID=@p2";
            else    
                sqlCommandText="select * from vw_Sch_BusRouteDisp where N_CompanyID=@p1";

            param.Add("@p1", nCompanyID);  
            param.Add("@p2", nBusID);                
                
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
                }
                if(dt.Rows.Count==0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
                
            }
            catch(Exception e)
            {
                return Ok(api.Error(User,e));
            }   
        } 

        [HttpGet("pickUpList") ]
        public ActionResult PickUpList(int nCompanyID,int nRouteID)
        {    
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="";

            if(nRouteID>0)
                sqlCommandText="select * from Sch_BusRouteDetail where N_CompanyID=@p1 and n_RouteID=@p2";
            else    
                sqlCommandText="select * from Sch_BusRouteDetail where N_CompanyID=@p1";

            param.Add("@p1", nCompanyID);  
            param.Add("@p2", nRouteID);                
                
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
                }
                if(dt.Rows.Count==0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
                
            }
            catch(Exception e)
            {
                return Ok(api.Error(User,e));
            }   
        }    
      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nRouteID,int nFnYearId)
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
                     if (nRouteID > 0)
                    {
                        object RouteCount = dLayer.ExecuteScalar("select COUNT(*) From Sch_BusRegistration where N_PickRoute =" + nRouteID + " and N_CompanyID =" + nCompanyID + " and N_FnYearID=" + nFnYearId , connection, transaction);
                        RouteCount = RouteCount == null ? 0 : RouteCount;
                        if (myFunctions.getIntVAL(RouteCount.ToString()) > 0)
                            return Ok(api.Error(User, "Bus Route Already In Use !!"));
                    }
                    Results = dLayer.DeleteData("Sch_BusRoute ", "N_RouteID", nRouteID, "N_CompanyID =" + nCompanyID, connection, transaction);
                                  
                    if (Results > 0)
                    {

                        dLayer.DeleteData("Sch_BusRouteDetail", "N_RouteID", nRouteID, "N_CompanyID =" + nCompanyID, connection, transaction); 

                        transaction.Commit();
                        return Ok(api.Success("Route deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User,"Unable to delete Route"));
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

