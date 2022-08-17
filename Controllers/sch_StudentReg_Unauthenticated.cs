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
    [Route("schStudentRegUnauthenticated")]
    [ApiController]
    public class sch_StudentReg_Unauthenticated : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =1517 ;

        public sch_StudentReg_Unauthenticated(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
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
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nRegID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_RegID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Stud = "";
                    var values = MasterTable.Rows[0]["X_RegNo"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.N_FormID);
                        Stud = dLayer.GetAutoNumber("Sch_Registration", "X_RegNo", Params, connection, transaction);
                        if (Stud == "")
                         { 
                            transaction.Rollback();
                            return Ok(api.Error(User,"Unable to generate student generation")); 
                            }
                        MasterTable.Rows[0]["X_RegNo"] = Stud;
                    }
                    string image = myFunctions.ContainColumn("i_Photo", MasterTable) ? MasterTable.Rows[0]["i_Photo"].ToString() : "";

                    Byte[] photoBitmap = new Byte[image.Length];
                    photoBitmap = Convert.FromBase64String(image);

                    if (myFunctions.ContainColumn("i_Photo", MasterTable))
                        MasterTable.Columns.Remove("i_Photo");
                        MasterTable.AcceptChanges();

                    if (nRegID > 0) 
                    {  
                        dLayer.DeleteData("Sch_Registration", "N_RegID", nRegID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    }
                  
                    nRegID = dLayer.SaveData("Sch_Registration", "N_RegID", MasterTable, connection, transaction);
                    if (nRegID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        if (image.Length > 0)
                    {
                        dLayer.SaveImage("Sch_Registration", "I_Photo", photoBitmap, "N_RegID",nRegID, connection, transaction);
                    }
                        transaction.Commit();
                        return Ok(api.Success("Student Registration Completed"));
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
