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
    [Route("accCostCenterPopup")]
    [ApiController]
    public class Acc_CostCenterPopup : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Acc_CostCenterPopup(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 217;
        }

        [HttpGet("fieldList")]
        public ActionResult GetFieldList()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    //Params.Add("@nFnYearID", nFnYearID);

                    dt.Clear();
                    dt.Columns.Add("N_BranchCount");
                    dt.Columns.Add("N_CCCount");
                    dt.Columns.Add("B_Project");
                    dt.Columns.Add("B_Employee");
                    dt.Columns.Add("B_CostCentre");
                    dt.Columns.Add("B_Asset");
                    dt.Columns.Add("B_RepaymentDate");
                    dt.Columns.Add("B_EmpFilterByPrj");
                   




                    object N_BranchCount = dLayer.ExecuteScalar("Select count(1) from Acc_BranchMaster where  N_CompanyID=" + nCompanyID, Params, connection);
                    object N_CCCount = dLayer.ExecuteScalar("Select count(1) from Acc_CostCentreMaster where  N_CompanyID=" + nCompanyID, Params, connection);
                    bool B_Project = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("371", "ProjectSettings", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    bool B_Employee = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("371", "EmployeeSettings", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    bool B_CostCentre = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("371", "EnableCCDistribution", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    bool B_Asset = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("371", "EnableAsset", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    bool B_RepaymentDate = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("371", "EnableRepaymentDate", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    bool B_EmpFilterByPrj = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("371", "EnablePrjWiseEmp", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

      

                    DataRow row = dt.NewRow();
                    row["N_BranchCount"] =myFunctions.getIntVAL(N_BranchCount.ToString()) ;
                    row["N_CCCount"] =  myFunctions.getIntVAL(N_CCCount.ToString());
                    row["B_Project"] = B_Project;
                    row["B_Employee"] = B_Employee ;
                    row["B_CostCentre"] = B_CostCentre;
                    row["B_Asset"] = B_Asset;
                    row["B_RepaymentDate"] = B_RepaymentDate   ;
                    row["B_EmpFilterByPrj"] = B_EmpFilterByPrj;
             
                      dt.Rows.Add(row);

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
                return Ok(_api.Error(e));
            }
        }
    }
}