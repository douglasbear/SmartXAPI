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
    [Route("schGuardian")]
    [ApiController]
    public class Sch_Guardian : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =181 ;


        public Sch_Guardian(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = 
            conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("dashboardList")]
        public ActionResult GetGuardianList(int? nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_ParentCode like '%" + xSearchkey + "%' or X_GaurdianName like '%" + xSearchkey + "%' or X_GaurdianName_Ar like '%" + xSearchkey + "%' or X_PFatherName like '%" + xSearchkey + "%' or X_PMotherName like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ParentID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "N_ParentID":
                        xSortBy = "N_ParentID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Sch_ParentDetails_Disp where N_CompanyID=@nCompanyId  " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Sch_ParentDetails_Disp where N_CompanyID=@nCompanyId  " + Searchkey + " and n_ParentID not in (select top(" + Count + ") n_ParentID from vw_Sch_ParentDetails_Disp where N_CompanyID=@nCompanyId " + xSortBy + " ) " + " " + xSortBy;

            Params.Add("@nCompanyId", nCompanyID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(1) as N_Count  from vw_Sch_ParentDetails_Disp where N_CompanyID=@nCompanyId " + Searchkey + "";
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
        public ActionResult GuardianDetails(string xParentCode)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_Sch_ParentDetails_Disp where N_CompanyID=@p1  and X_ParentCode=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", xParentCode);
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
                int nParentID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ParentID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["x_ParentCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Sch_ParentDetails", "x_ParentCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Guardian Details Code")); }
                        MasterTable.Rows[0]["x_ParentCode"] = Code;
                    }
                    MasterTable.Columns.Remove("n_FnYearId");
                    string fatherImage = myFunctions.ContainColumn("i_FatherImage", MasterTable) ? MasterTable.Rows[0]["i_FatherImage"].ToString() : "";
                    Byte[] photoBitmapF = new Byte[fatherImage.Length];
                    photoBitmapF = Convert.FromBase64String(fatherImage);
                    if (myFunctions.ContainColumn("i_FatherImage", MasterTable))
                        MasterTable.Columns.Remove("i_FatherImage");

                    string motherImage = myFunctions.ContainColumn("i_MotherImage", MasterTable) ? MasterTable.Rows[0]["i_MotherImage"].ToString() : "";
                    Byte[] photoBitmapM = new Byte[motherImage.Length];
                    photoBitmapM = Convert.FromBase64String(motherImage);
                    if (myFunctions.ContainColumn("i_MotherImage", MasterTable))
                        MasterTable.Columns.Remove("i_MotherImage");

                    // if (nParentID > 0) 
                    // {  
                    //     dLayer.DeleteData("Sch_ParentDetails", "n_ParentID", nParentID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    // }

                    string DupCriteria = "N_CompanyID=" + nCompanyID + "  and X_ParentCode='" + Code + "'";
                    string X_Criteria = "N_CompanyID=" + nCompanyID ;

                    nParentID = dLayer.SaveData("Sch_ParentDetails", "n_ParentID",DupCriteria,X_Criteria, MasterTable, connection, transaction);
                    if (nParentID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        if (fatherImage.Length > 0)
                        {
                            dLayer.SaveImage("Sch_ParentDetails", "I_FatherImage", photoBitmapF, "N_ParentID",nParentID, connection, transaction);
                        }
                        if (motherImage.Length > 0)
                        {
                            dLayer.SaveImage("Sch_ParentDetails", "I_MotherImage", photoBitmapM, "N_ParentID",nParentID, connection, transaction);
                        }
                        transaction.Commit();
                        return Ok(api.Success("Guardian Details Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        [HttpGet("list") ]
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
      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nParentID)
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
                    Results = dLayer.DeleteData("Sch_ParentDetails ", "N_ParentID", nParentID, "N_CompanyID =" + nCompanyID, connection, transaction);
                
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("Guardian details deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User,"Unable to delete Guardian details"));
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("REFERENCE constraint"))
                    return Ok(api.Error(User, "Unable to delete guardian! It has been used."));
                else
                    return Ok(api.Error(User, ex));
            }



        }
    }
}

