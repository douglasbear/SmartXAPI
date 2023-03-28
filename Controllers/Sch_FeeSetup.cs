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
    [Route("schFeeSetup")]
    [ApiController]
    public class Sch_FeeSetup : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;

        public Sch_FeeSetup(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 159;
        }
        private readonly string connectionString;
        
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable Master = ds.Tables["master"];
                    DataTable Details = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    DataRow MasterRow = Master.Rows[0];
                    int N_FeeSetupID = myFunctions.getIntVAL(MasterRow["N_FeeSetupID"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterRow["n_AcYearID"].ToString());
                    int N_CompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    int N_ClassID = myFunctions.getIntVAL(MasterRow["N_ClassID"].ToString());
                    int N_StudentTypeID = myFunctions.getIntVAL(MasterRow["N_StudentTypeID"].ToString());
                    string x_FeeSetupCode = MasterRow["x_FeeSetupCode"].ToString();

                    if (N_FeeSetupID > 0)
                    {

                        dLayer.ExecuteNonQuery("delete from Sch_ClassFeeSetupMaster Where N_CompanyID = "+N_CompanyID+" and N_AcYearID = "+N_FnYearID+" and N_ClassID = "+N_ClassID+" and N_StudentTypeID="+N_StudentTypeID+"", connection, transaction);
                        dLayer.ExecuteNonQuery("delete from Sch_ClassFeeSetup Where N_CompanyID = "+N_CompanyID+"  and N_ClassID ="+N_ClassID+" and N_StudentTypeID="+N_StudentTypeID+"", connection, transaction);

                    }

                    if (x_FeeSetupCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", 159);
                        x_FeeSetupCode = dLayer.GetAutoNumber("Sch_ClassFeeSetupMaster", "x_FeeSetupCode", Params, connection, transaction);
                        if (x_FeeSetupCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Week Code");
                        }
                        Master.Rows[0]["x_FeeSetupCode"] = x_FeeSetupCode;
                    }
                    string DupCriteria = "";


                    N_FeeSetupID = dLayer.SaveData("Sch_ClassFeeSetupMaster", "N_FeeSetupID", DupCriteria, "", Master, connection, transaction);
                    if (N_FeeSetupID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save");
                    }
                    for (int i = 0; i < Details.Rows.Count; i++)
                    {
                        Details.Rows[i]["N_FeeSetupID"] = N_FeeSetupID;
                        //Details.Rows[i]["N_FeeSetupID"] = N_FeeSetupID;

                    }

                    dLayer.SaveData("Sch_ClassFeeSetup", "N_ClassFeeID", Details, connection, transaction);
                    transaction.Commit();
                    SortedList Result = new SortedList();

                    return Ok(_api.Success(Result, "Fee Setup Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
        [HttpGet("details")]
        public ActionResult GetWeekdaysDetails(int nCourseID, int nStudentTypeID,int nFnYearID)
        {

            DataTable details = new DataTable();

            DataSet DS = new DataSet();
            SortedList Params = new SortedList();
            SortedList dParamList = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

           
            string Detailssql = "Select * from VW_CLASSFEESETUP Where N_CompanyID = @p1 and N_AcYearID = @p2 and N_ClassID = @p3 and N_StudentTypeID=@p4";

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearID);
            Params.Add("@p3", nCourseID);
            Params.Add("@p4", nStudentTypeID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    details = dLayer.ExecuteDataTable(Detailssql, Params, connection);

                }
                if(details.Rows.Count==0)
                {
                    return Ok(_api.Success(details));
                }
                else
                {
                    return Ok(_api.Success(details));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }


        
        // [HttpGet("details")]
        // public ActionResult GradeListDetails(string xFeeSetupCode,int nFnYearID)
        // {
        //     DataSet dt=new DataSet();
        //     SortedList Params=new SortedList();
        //     int nCompanyID = myFunctions.GetCompanyID(User);
        //     DataTable MasterTable = new DataTable();
        //     DataTable DetailTable = new DataTable();
  
        //     string Mastersql = "select * from vw_Sch_FeeSetup where N_CompanyID=@p1 and x_FeeSetupCode=@p2 and N_AcYearID = @p3";
        //     Params.Add("@p1", nCompanyID);
        //     Params.Add("@p2", xFeeSetupCode);
        //      Params.Add("@p3", nFnYearID);

        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             MasterTable=dLayer.ExecuteDataTable(Mastersql,Params,connection); 

        //             if (MasterTable.Rows.Count == 0)
        //             {
        //                 return Ok(_api.Warning("No Data Found !!"));
        //             }

        //             MasterTable = _api.Format(MasterTable, "Master");
        //             dt.Tables.Add(MasterTable);

        //             int x_FeeSetupCode = myFunctions.getIntVAL(MasterTable.Rows[0]["x_FeeSetupCode"].ToString());

        //             string DetailSql = "select * from VW_CLASSFEESETUP where N_CompanyID=" + nCompanyID + " and x_FeeSetupCode=" + x_FeeSetupCode ;

        //             DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
        //             DetailTable = _api.Format(DetailTable, "Details");
        //             dt.Tables.Add(DetailTable);
        //         }
        //         return Ok(_api.Success(dt));
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(_api.Error(User,e));
        //     }
        // }


        [HttpDelete("delete")]
        public ActionResult DeleteData(int nFeeSetupID)
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
                    dLayer.DeleteData("Sch_ClassFeeSetupMaster", "N_FeeSetupID", nFeeSetupID,"N_CompanyID =" + nCompanyID , connection,transaction);
                    Results = dLayer.DeleteData("Sch_ClassFeeSetup", "N_FeeSetupID", nFeeSetupID, "N_CompanyID =" + nCompanyID,connection,transaction);
                    if (Results > 0)
                    {
                      
                    
                        transaction.Commit();
                        return Ok(_api.Success("deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }



        [HttpGet("dashboardList")]
        public ActionResult GetAssignmentList(int nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy, int nAcYearID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            // if (xSearchkey != null && xSearchkey.Trim() != "")
            
            //     Searchkey = "and (N_FeeCodeID like '%" + xSearchkey + "%' OR X_FeeCode like '%" + xSearchkey + "%' OR X_FeeDescription like '%" + xSearchkey + "%')";

            // if (xSortBy == null || xSortBy.Trim() == "")
            //     xSortBy = " order by N_FeeCodeID desc";
                
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_FeeSetupCode like '%" + xSearchkey + "%' OR X_Class like '%" + xSearchkey + "%'OR X_StudentCatName like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by n_FeeSetupID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_FeeSetupCode":
                        xSortBy = "X_FeeSetupCode " + xSortBy.Split(" ")[1];
                        break;
                    case "n_FeeSetupID":
                        xSortBy = "n_FeeSetupID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }
       
            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Sch_FeeSetup where N_CompanyID=@nCompanyID and N_AcYearID=@nAcYearID " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_Sch_FeeSetup where N_CompanyID=@nCompanyID and N_AcYearID=@nAcYearID " + Searchkey + " " + xSortBy;

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nAcYearID", nAcYearID);
           

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(1) as N_Count  from vw_Sch_FeeSetup where N_CompanyID=@nCompanyID and N_AcYearID=@nAcYearID " + Searchkey + " ";
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

