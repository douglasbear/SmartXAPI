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
    [Route("schLibraryRegistration")]
    [ApiController]
    public class Sch_LibraryRegistration : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =1759 ;


        public Sch_LibraryRegistration(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

          [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int n_FnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nLibraryRegID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_LibraryRegID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    
                    string Code = "";
                    string x_LibraryCode = MasterTable.Rows[0]["x_LibraryCode"].ToString();
                    if (x_LibraryCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", n_FnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        x_LibraryCode = dLayer.GetAutoNumber("Sch_LibraryRegistration", "x_LibraryCode", Params, connection, transaction);
                        if (x_LibraryCode == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Reason Code")); }
                        MasterTable.Rows[0]["x_LibraryCode"] = x_LibraryCode;
                    }
                     MasterTable.Columns.Remove("n_FnYearId");
                     MasterTable.Columns.Remove("x_Class");
                     MasterTable.Columns.Remove("n_ClassID");
                     MasterTable.Columns.Remove("x_CustomerName");
                     MasterTable.Columns.Remove("n_StudentCode");
                     MasterTable.Columns.Remove("x_AdmissionNo");
                     MasterTable.Columns.Remove("n_ClassDivisionID");

                    if (nLibraryRegID > 0) 
                    {  
                        dLayer.DeleteData("Sch_LibraryRegistration", "n_LibraryRegID", nLibraryRegID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    }

                    nLibraryRegID = dLayer.SaveData("Sch_LibraryRegistration", "n_LibraryRegID", MasterTable, connection, transaction);
                    if (nLibraryRegID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(api.Success("Admission Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

   [HttpGet("details")]
        public ActionResult LibraryDetails(string x_LibraryCode)
        {
           
            DataTable dt=new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from VW_LibraryRegistration where N_CompanyID=@p1 and X_LibraryCode=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", x_LibraryCode);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                   if(dt.Rows.Count==0)
                {
                    return Ok(api.Notice("No Results Found"));
                } else {
                    return Ok(api.Success(dt));
                }
                }
                            
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }


    }
}