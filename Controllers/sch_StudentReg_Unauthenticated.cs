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

        [HttpPost("unauthstdregsave")]
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

        [HttpGet("unauthnationalitylist")]
        public ActionResult GetNationality()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "select N_NationalityID, X_Nationality, X_NationalityLocale, X_NationalityCode, D_Entrydate, X_Country, B_Default, X_Currency, N_CountryID from Pay_Nationality";
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
                    return Ok(api.Notice("No Results Found"));
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

        [HttpGet("unauthguardianlist") ]
        public ActionResult GuardianList(int nCompanyID)
        {    
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="";

            sqlCommandText="select * from vw_Sch_ParentDetails_Disp where N_CompanyID=@p1";

            param.Add("@p1", nCompanyID);              
                
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

        [HttpGet("unauthrelationlist")]
        public ActionResult GetRelationList(int nCompanyID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "Select n_RelationID,x_Relation from Pay_Relation Where N_CompanyID=@nCompanyID order by n_RelationID";
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
                    return Ok(api.Notice("No Results Found"));
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

        [HttpGet("unauthcountrylist")]
        public ActionResult GetCountryList(int nCompanyID,int N_AllowCompany)
        {
            DataTable dt = new DataTable(); 
            SortedList Params = new SortedList();
            string sqlCommandText ="";

            if(N_AllowCompany!=0)
                sqlCommandText = "select X_CountryCode,X_CountryName,x_Currency,N_CompanyID,N_CountryID,B_TaxImplement from Acc_Country where N_CompanyID=@p1 and ISNULL(B_AllowCompany,0)=1 order by N_CountryID";
            else
                sqlCommandText = "select X_CountryCode,X_CountryName,x_Currency,N_CompanyID,N_CountryID,B_TaxImplement from Acc_Country where N_CompanyID=@p1  order by N_CountryID";
            Params.Add("@p1", nCompanyID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
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
                return Ok( api.Error(User,e));
            }
        }

        [HttpGet("unauthstdreglist")]
        public ActionResult GetRegistrationList(int? nCompanyID, int nAcYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_RegNo like '%" + xSearchkey + "%' or X_FullName like '%" + xSearchkey + "%' or X_GuardianName like '%" + xSearchkey + "%' or D_DOB like '%" + xSearchkey + "%' or X_CountryName like '%" + xSearchkey + "%'  or X_MobileNo like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_RegNo desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_RegNo":
                        xSortBy = "X_RegNo " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Sch_Registration where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID  " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Sch_Registration where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + Searchkey + " " + xSortBy;

            Params.Add("@nCompanyId", nCompanyID);
            Params.Add("@nAcYearID", nAcYearID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(*) as N_Count  from vw_Sch_Registration  where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + Searchkey + "";
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

    }
}
