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
    [Route("vacationReturn")]
    [ApiController]
    public class Pay_VacationReturn : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Pay_VacationReturn(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("vacationList")]
        public ActionResult GetVacationList(int nBranchID,bool bAllBranchData,int nEmpID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nBranchID",nBranchID);
            Params.Add("@nEmpID",nEmpID);
            string strBranch=(bAllBranchData==false)?" and N_BranchID=@nBranchID ":"";
            string sqlCommandText="select X_VacationGroupCode,VacationREquestDate,X_VacType,N_CompanyID,N_EmpID,N_BranchID,N_VacationGroupID,N_Transtype,N_VacTypeID,B_IsSaveDraft from vw_PayVacationMaster_Disp where N_CompanyID=@nCompanyID And N_Transtype =1 and N_EmpID=@nEmpID and B_IsSaveDraft=0 "+strBranch;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
                return Ok(_api.Error(e));
            }
        }

        }
    }