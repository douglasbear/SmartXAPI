using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("jobtitle")]
    [ApiController]
    public class Pay_JobTitle : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Pay_JobTitle(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 195;
        }

        [HttpGet("list")]
        public ActionResult GetJobTitle()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "Select N_CompanyID,N_PositionID,N_EmpID,isNull(N_SalaryGradeID,0) as N_SalaryGradeID,B_Edit,Code,Description,isNull(X_GradeCode,'') as X_GradeCode,isNull(X_Gradename,'') as X_Gradename,Description as X_FunctionalJobTitle,ISNULL(N_SalaryFrom,0) AS N_SalaryFrom,ISNULL(N_SalaryTo,0) AS N_SalaryTo from vw_PayPosition_DispAdvanced Where N_CompanyID=@nCompanyID Group By N_CompanyID,N_PositionID,N_EmpID,N_SalaryGradeID,B_Edit,Code,Description,X_GradeCode,X_Gradename, N_SalaryFrom,N_SalaryTo";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable dtSupervisor;
                MasterTable = ds.Tables["master"];
                dtSupervisor = ds.Tables["supervisor"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    SortedList QueryParams = new SortedList();
                    string X_PositionCode = MasterTable.Rows[0]["x_PositionCode"].ToString();
                    string X_Position = MasterTable.Rows[0]["x_Position"].ToString();
                    int N_CompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                    int N_FnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                    int N_PositionID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PositionID"].ToString());
                    int N_SupervisorID = myFunctions.getIntVAL(dtSupervisor.Rows[0]["n_SupervisorID"].ToString());
                    int N_IsSupervisor = myFunctions.getIntVAL(MasterTable.Rows[0]["b_IsSupervisor"].ToString());
                    string x_FunctionalJobTitle ="";
                    bool B_IsSupervisor = false;
                if( MasterTable.Columns.Contains("x_FunctionalJobTitle"))
                {
                    x_FunctionalJobTitle=MasterTable.Rows[0]["x_FunctionalJobTitle"].ToString();

                }
                    if (N_IsSupervisor == 1) B_IsSupervisor = true;
                    QueryParams.Add("@nCompanyID", N_CompanyID);
                    QueryParams.Add("@nPositionID", N_PositionID);

                    if (X_PositionCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("X_Position",X_Position);
                        Params.Add("x_FunctionalJobTitle",x_FunctionalJobTitle);

                        X_PositionCode = dLayer.GetAutoNumber("Pay_Position", "x_PositionCode", Params, connection, transaction);
                        if (X_PositionCode == "") { transaction.Rollback(); return Ok(_api.Error(User,"Unable to generate Job title Code")); }
                        MasterTable.Rows[0]["x_PositionCode"] = X_PositionCode;

                        if(x_FunctionalJobTitle!=""){
                             object nCount = dLayer.ExecuteScalar("Select count(*) From Pay_Position where N_CompanyID=@N_CompanyID and X_Position=@X_Position and x_FunctionalJobTitle=@x_FunctionalJobTitle", Params, connection,transaction);
                             object status = dLayer.ExecuteScalar("select x_Position from Pay_Position where N_CompanyID=@N_CompanyID and X_Position=@X_Position and x_FunctionalJobTitle=@x_FunctionalJobTitle",Params, connection,transaction);

                                  if(myFunctions.getIntVAL(nCount.ToString())>0){
                                 return Ok(_api.Error(User, x_FunctionalJobTitle+" Under " + status + " Already Exist"));
                                 }
                        }
                       if(x_FunctionalJobTitle==""){
                            object jobtitle = dLayer.ExecuteScalar("Select count(*) From Pay_Position where N_CompanyID=@N_CompanyID and X_Position=@X_Position and ISNULL(Pay_Position.X_FunctionalJobTitle,'')=''", Params, connection,transaction);
                       if(myFunctions.getIntVAL(jobtitle.ToString())>=1){
                       return Ok(_api.Error(User,"Job Title Already Exist"));
                       }
                       }
                     
                    }

                
                    MasterTable.Columns.Remove("N_FnYearID");


                    //string DupCriteria = "N_CompanyID=" + N_CompanyID + " and(X_PositionCode='" + X_PositionCode + "' OR X_Position='" + X_Position + "')";
                   
                    N_PositionID = dLayer.SaveData("Pay_Position", "N_PositionID", MasterTable, connection, transaction);
                    if (N_PositionID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Duplicate Exist"));
                    }
                    else
                    {

                        dtSupervisor.Rows[0]["n_SupervisorID"] = N_SupervisorID;
                        dtSupervisor.Rows[0]["x_SupervisorCode"] = X_PositionCode;

                        if (B_IsSupervisor)
                        {
                            dLayer.ExecuteNonQuery("DELETE FROM Pay_Supervisor WHERE N_CompanyID =" + N_CompanyID + " and x_SupervisorCode= "+X_PositionCode+" and N_PositionID="+N_PositionID+"", Params, connection, transaction);
                            N_SupervisorID = dLayer.SaveData("Pay_Supervisor", "N_SupervisorID", dtSupervisor, connection, transaction);
                            //  object N_EmpID=dLayer.ExecuteScalar("select N_EmpID  from ")


                        }
                        else
                            dLayer.DeleteData("Pay_Supervisor", "N_PositionID", N_PositionID, "N_CompanyID=" + N_CompanyID + "", connection, transaction);

                        transaction.Commit();
                    }

                    return Ok(_api.Success("Job title Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("details")]
        public ActionResult GetJobTitleDetails(int nPositionID)
        {
            DataTable dt = new DataTable();
            DataTable dtSupervisor = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_Position_Display where N_CompanyID=@nCompanyID and N_PositionID=@nPositionID";
            string sqlCommandText2 = "select * from Pay_Supervisor where N_CompanyID=@nCompanyID and N_PositionID=@nPositionID";
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nPositionID", nPositionID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dtSupervisor = dLayer.ExecuteDataTable(sqlCommandText2, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nPositionID, int nCompanyID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    object objEmp = dLayer.ExecuteScalar("select count(1) from Pay_Employee where N_CompanyID="+nCompanyID+" and N_PositionID="+nPositionID, connection);
                    object objSupervisor = dLayer.ExecuteScalar("select count(1) from Pay_Supervisor where N_CompanyID="+nCompanyID+" and N_PositionID="+nPositionID, connection);
                    object objReg = dLayer.ExecuteScalar("select count(1) from Rec_Registration where N_CompanyID="+nCompanyID+" and N_PositionID="+nPositionID, connection);
                    object objJobOfr = dLayer.ExecuteScalar("select count(1) from Rec_JobOfferAdditionalDetails where N_CompanyID="+nCompanyID+" and N_PositionID="+nPositionID, connection);
                    if (objEmp == null) objEmp = 0; if (objSupervisor == null) objSupervisor = 0;
                    if (objReg == null) objReg = 0; if (objJobOfr == null) objJobOfr = 0;

                    if(myFunctions.getIntVAL(objEmp.ToString()) > 0 || myFunctions.getIntVAL(objSupervisor.ToString()) > 0 || myFunctions.getIntVAL(objReg.ToString()) > 0 || myFunctions.getIntVAL(objJobOfr.ToString()) > 0)
                    {
                        return Ok(_api.Error(User, "Already Used! Unable to delete."));
                    }

                    dLayer.DeleteData("Pay_Supervisor", "N_PositionID", nPositionID, "N_CompanyID=" + nCompanyID + "", connection);
                    Results = dLayer.DeleteData("Pay_Position", "N_PositionID", nPositionID, "N_CompanyID=" + nCompanyID + "", connection);
                    if (Results > 0)
                    {
                        return Ok(_api.Success("Job Title deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete Job Title"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }
         [HttpGet("chart")]
        public ActionResult GetCategoryChart()

        {
            DataTable dt = new DataTable();
            DataTable Images = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);


            string sqlCommandText = "Select *  from vw_Position_Display Where N_CompanyID= " + nCompanyID + " ";


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                }

                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }

        }

  [HttpGet("childDetails")]
        public ActionResult GetChildDetails(int nPositionID)

        {
            DataTable dt = new DataTable();
           
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);


            string sqlCommandText = "Select *  from Pay_Position Where N_CompanyID= " + nCompanyID + " and N_PositionID=" + nPositionID + "";


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                  
                    dt.AcceptChanges();
                    dt = _api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(dt));
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