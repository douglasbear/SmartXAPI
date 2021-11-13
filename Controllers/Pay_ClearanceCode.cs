using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("clearanceCode")]
    [ApiController]
    public class Pay_ClearanceCode : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Pay_ClearanceCode(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetClearanceCode (int nBranchID, bool bAllBranchData)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            
            DataTable dt=new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nBranchID);
            if (bAllBranchData == true)
            {
                sqlCommandText="Select N_CompanyID, N_ClearanceSettingsID, X_ClearanceCode, X_PurposeName, N_PurposeID, N_BranchID, N_UserID, D_EntryDate from Pay_EmployeeClearanceSettings where N_CompanyID=@p1 order by X_ClearanceCode";
            }
            else
                sqlCommandText="Select N_CompanyID, N_ClearanceSettingsID, X_ClearanceCode, X_PurposeName, N_PurposeID, N_BranchID, N_UserID, D_EntryDate from Pay_EmployeeClearanceSettings where N_CompanyID=@p1 and N_BranchID=@p2 order by X_ClearanceCode";

            try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(_api.Notice("No Results Found"));
                        }else{
                            return Ok(_api.Success(dt));
                        }
                
            }catch(Exception e){
                return Ok(_api.Error(User,e));
            }   
        }
    }
}