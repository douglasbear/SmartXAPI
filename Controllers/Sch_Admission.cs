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
    [Route("schAdmission")]
    [ApiController]
    public class Sch_Admission : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =155 ;


        public Sch_Admission(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetAdmissionList(int? nCompanyId, int nAcYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_AdmissionNo like '%" + xSearchkey + "%' or X_Name like '%" + xSearchkey + "%' or X_PFamilyName like '%" + xSearchkey + "%' or X_PMotherName like '%" + xSearchkey + "%' or X_GaurdianName like '%" + xSearchkey + "%'  or X_RegNo like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_AdmissionNo desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_AdmissionNo":
                        xSortBy = "X_AdmissionNo " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_SchAdmission where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID  " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_SchAdmission where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + Searchkey + " and N_AdmissionID not in (select top(" + Count + ") N_AdmissionID from vw_SchAdmission where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + xSortBy + " ) " + " " + xSortBy;

            Params.Add("@nCompanyId", nCompanyID);
            Params.Add("@nAcYearID", nAcYearID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(*) as N_Count  from vw_SchAdmission  where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + Searchkey + "";
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

        [HttpGet("details")]
        public ActionResult AdmissionDetails(string xAdmissionNo)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_SchAdmission where N_CompanyID=@p1  and x_AdmissionNo=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", xAdmissionNo);
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
                int nAdmissionID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AdmissionID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_AdmissionNo"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Sch_Admission", "X_AdmissionNo", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Admission No")); }
                        MasterTable.Rows[0]["X_AdmissionNo"] = Code;
                    }
                     MasterTable.Columns.Remove("n_FnYearId");
                    if (nAdmissionID > 0) 
                    {  
                        dLayer.DeleteData("Sch_Admission", "N_AdmissionID", nAdmissionID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    }

                    nAdmissionID = dLayer.SaveData("Sch_Admission", "N_AdmissionID", MasterTable, connection, transaction);
                    if (nAdmissionID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Admission Completed"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        [HttpGet("detailList") ]
        public ActionResult AdmissionList(int nCompanyID,int nAcYearID,int n_ClassID,int n_DivisionID)
        {    
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="";

            if(n_ClassID!=0)
            {
                if(n_DivisionID!=0)
                    sqlCommandText="select * from vw_SchAdmission where N_CompanyID=@p1 and nAcYearID=@p2 and n_ClassID=@p3 and n_DivisionID=@p4";
                else                    
                    sqlCommandText="select * from vw_SchAdmission where N_CompanyID=@p1 and nAcYearID=@p2 and n_ClassID=@p3 ";
            }
            else
            {
                if(n_DivisionID!=0)
                    sqlCommandText="select * from vw_SchAdmission where N_CompanyID=@p1 and nAcYearID=@p2 and n_DivisionID=@p4";
                else                    
                    sqlCommandText="select * from vw_SchAdmission where N_CompanyID=@p1 and nAcYearID=@p2";
            }

            param.Add("@p1", nCompanyID);             
            param.Add("@p2", nAcYearID);             
            param.Add("@p3", n_ClassID);             
            param.Add("@p4", n_DivisionID);             
                
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
        public ActionResult DeleteData(int nAdmissionID)
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
                    Results = dLayer.DeleteData("Sch_Admission", "n_AdmissionID", nAdmissionID, "N_CompanyID =" + nCompanyID, connection, transaction);                   
                
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("Student deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User,"Unable to delete Student"));
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

