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
    [Route("schCourseTransfer")]
    [ApiController]
    public class Sch_CourseTransfer : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int nFormID =1520 ;


        public Sch_CourseTransfer(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        // [HttpGet("list")]
        // public ActionResult GetBusRegList(int? nCompanyId, int nAcYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        // {
        //     int nCompanyID = myFunctions.GetCompanyID(User);
        //     DataTable dt = new DataTable();
        //     SortedList Params = new SortedList();
        //     int Count = (nPage - 1) * nSizeperpage;
        //     string sqlCommandText = "";
        //     string sqlCommandCount = "";
        //     string Searchkey = "";

        //     if (xSearchkey != null && xSearchkey.Trim() != "")
        //         Searchkey = "and (X_RegistrationCode like '%" + xSearchkey + "%' or X_AdmissionNo like '%" + xSearchkey + "%' or X_Name like '%" + xSearchkey + "%' or X_RouteName like '%" + xSearchkey + "%' or DropRoute like '%" + xSearchkey + "%')";

        //     if (xSortBy == null || xSortBy.Trim() == "")
        //         xSortBy = " order by X_RegistrationCode desc";
        //     else
        //     {
        //         switch (xSortBy.Split(" ")[0])
        //         {
        //             case "X_RegistrationCode":
        //                 xSortBy = "X_RegistrationCode " + xSortBy.Split(" ")[1];
        //                 break;
        //             default: break;
        //         }
        //         xSortBy = " order by " + xSortBy;
        //     }

        //     if (Count == 0)
        //         sqlCommandText = "select top(" + nSizeperpage + ") * from vw_SchReg_Disp where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID  " + Searchkey + " " + xSortBy;
        //     else
        //         sqlCommandText = "select top(" + nSizeperpage + ") * from vw_SchReg_Disp where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + Searchkey + " and N_RegistrationID not in (select top(" + Count + ") N_RegistrationID from vw_SchReg_Disp where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + xSortBy + " ) " + " " + xSortBy;

        //     Params.Add("@nCompanyId", nCompanyID);
        //     Params.Add("@nAcYearID", nAcYearID);

        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
        //             SortedList OutPut = new SortedList();

        //             sqlCommandCount = "select count(*) as N_Count  from vw_SchReg_Disp where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + Searchkey + "";
        //             object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
        //             OutPut.Add("Details", api.Format(dt));
        //             OutPut.Add("TotalCount", TotalCount);
        //             if (dt.Rows.Count == 0)
        //             {
        //                 return Ok(api.Warning("No Results Found"));
        //             }
        //             else
        //             {
        //                 return Ok(api.Success(OutPut));
        //             }
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(api.Error(User, e));
        //     }
        // }

        // [HttpGet("details")]
        // public ActionResult BusRegDetails(string xRegistrationCode)
        // {
        //     DataSet dt=new DataSet();
        //     DataTable MasterTable = new DataTable();
        //     SortedList Params = new SortedList();
        //     int nCompanyId=myFunctions.GetCompanyID(User);
        //     string sqlCommandText = "select * from vw_SchReg_Disp where N_CompanyID=@p1  and X_RegistrationCode=@p2";
        //     Params.Add("@p1", nCompanyId);  
        //     Params.Add("@p2", xRegistrationCode);
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

        //             if (MasterTable.Rows.Count == 0)
        //             {
        //                 return Ok(api.Warning("No Results Found"));
        //             }
                
        //             MasterTable = api.Format(MasterTable, "Master");
        //             dt.Tables.Add(MasterTable);
        //         }
        //         return Ok(api.Success(dt));               
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(api.Error(User,e));
        //     }
        // }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();

                    int nTransferID = myFunctions.getIntVAL(MasterRow["n_TransferID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
                    int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
                    string xTransferCode = MasterRow["x_TransferCode"].ToString();

                    if (xTransferCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", nFormID);
                        xTransferCode = dLayer.GetAutoNumber("Sch_CourseTransfer", "x_TransferCode", Params, connection, transaction);
                        if (xTransferCode == "")
                        {
                            transaction.Rollback();
                            return Ok("Unable to generate Employee Evaluation");
                        }
                        MasterTable.Rows[0]["x_TransferCode"] = xTransferCode;
                    }
                    MasterTable.Columns.Remove("n_FnYearID");

                    nTransferID = dLayer.SaveData("Sch_CourseTransfer", "N_TransferID", "", "", MasterTable, connection, transaction);
                    if (nTransferID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Course Transfer");
                    }
                    dLayer.DeleteData("Sch_CourseTransferStudents", "N_TransferID", nTransferID, "", connection, transaction);
                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["n_TransferID"] = nTransferID;
                    }
                    int nTransferStudentID = dLayer.SaveData("Sch_CourseTransferStudents", "N_TransferStudentID", DetailTable, connection, transaction);
                    if (nTransferStudentID <= 0)
                    {
                        transaction.Rollback();
                        return Ok("Unable to save Course Transfer");
                    }
                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("n_TransferID", nTransferID);
                    Result.Add("x_TransferCode", xTransferCode);
                    Result.Add("n_TransferStudentID", nTransferStudentID);

                    return Ok(api.Success(Result, "Course Transfer saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        // [HttpGet("detailList") ]
        // public ActionResult BusRegList(int nCompanyID)
        // {    
        //     SortedList param = new SortedList();           
        //     DataTable dt=new DataTable();
            
        //     string sqlCommandText="";

        //     sqlCommandText="select * from vw_SchReg_Disp where N_CompanyID=@p1";

        //     param.Add("@p1", nCompanyID);             
                
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();

        //             dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
        //         }
        //         if(dt.Rows.Count==0)
        //         {
        //             return Ok(api.Notice("No Results Found"));
        //         }
        //         else
        //         {
        //             return Ok(api.Success(dt));
        //         }              
        //     }
        //     catch(Exception e)
        //     {
        //         return Ok(api.Error(User,e));
        //     }   
        // }   
      
        // [HttpDelete("delete")]
        // public ActionResult DeleteData(int nRegistrationID)
        // {

        //     int Results = 0;
        //     int nCompanyID=myFunctions.GetCompanyID(User);
        //     try
        //     {                        
        //         SortedList Params = new SortedList();
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();
        //             Results = dLayer.DeleteData("Sch_BusRegistration", "n_RegistrationID", nRegistrationID, "N_CompanyID =" + nCompanyID, connection, transaction);                   
                
        //             if (Results > 0)
        //             {
        //                 transaction.Commit();
        //                 return Ok(api.Success("Bus Registration deleted"));
        //             }
        //             else
        //             {
        //                 return Ok(api.Error(User,"Unable to delete Bus Registration"));
        //             }
        //         }

        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(api.Error(User,ex));
        //     }



        // }
    }
}

