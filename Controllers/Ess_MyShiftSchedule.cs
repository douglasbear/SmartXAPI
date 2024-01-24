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
    [Route("myShiftSchedule")]
    [ApiController]
    public class Ess_MyShiftSchedule : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Ess_MyShiftSchedule(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1862;
        }


        
        [HttpGet("MyShiftDetails")]
        public ActionResult EmphiftDetails(int nEmpID, int nFnyearID, DateTime dtpFromdate,DateTime dtpTodate,bool lastDayFlag)
        {
    
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                                DataSet dt = new DataSet();
                                DataTable MasterTable = new DataTable();
                                SortedList Params = new SortedList();
                               int nCompanyId = myFunctions.GetCompanyID(User);
                             string Sql = "";
                             string sqlCommandText="";
                             string sqlCommandCount = "";
                             object TotalCount=0;
                               SortedList OutPut = new SortedList();
                                Params.Add("@p1", nCompanyId);
                                Params.Add("@p2", nFnyearID);
                                Params.Add("@p3", nEmpID);
                                 DateTime Start = DateTime.Now;
                              DateTime NewDate = Convert.ToDateTime(Start);
                      if(lastDayFlag==true){
                           SortedList mParamsList = new SortedList(){
                                {"N_CompanyID",nCompanyId},
                                {"N_FnYearID",nFnyearID},
                                {"N_EmpID",nEmpID},
                                {"dtpFromdate",dtpFromdate},
                                {"dtpTodate",dtpTodate},
                                 {"lastDayFlag",lastDayFlag==true?1:0}};
                        dLayer.ExecuteDataTablePro("SP_Pay_EmpShiftDetails", mParamsList, connection) ;
                        sqlCommandText = "select  * from pay_employeeShiftDetails where N_EmpID=@p3 and N_CompanyID=@p1 order by D_Date asc";
                      }
                      else{
                                SortedList proParams1 =new SortedList(){
                                {"N_CompanyID",nCompanyId},
                                {"N_FnYearID",nFnyearID},
                                {"N_EmpID",nEmpID}};
                        dLayer.ExecuteDataTablePro("SP_Pay_EmpShiftDetails", proParams1, connection) ;
                        sqlCommandText = "select  * from pay_employeeShiftDetails where N_EmpID=@p3 and N_CompanyID=@p1 and MONTH(Cast(pay_employeeShiftDetails.D_Date as DATE)) = MONTH(CURRENT_TIMESTAMP) and YEAR(pay_employeeShiftDetails.D_Date)= YEAR(CURRENT_TIMESTAMP)  order by D_Date asc";
                      }
                            
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                        OutPut.Add("Master", _api.Format(MasterTable));
                        return Ok(_api.Success(OutPut));
               }
            }

                catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
          }

    }
}