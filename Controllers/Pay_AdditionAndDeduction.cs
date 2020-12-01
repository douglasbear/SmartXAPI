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
    [Route("additionAndDeduction")]
    [ApiController]
    public class Pay_AdditionAndDeduction : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Pay_AdditionAndDeduction(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 0;
        }

        [HttpGet("empList")]
        public ActionResult GetJobTitle(int nBatchID,int nFnYearId,string payRunID,string xDepartment,string xPosition,bool bAllBranchData,int nBranchID)
        {
            DataTable mst = new DataTable();
            DataTable dt = new DataTable();
            int nCompanyID=myFunctions.GetCompanyID(User);

            string X_Cond = "";
            if (xDepartment!=null && xDepartment != "")
                X_Cond = " and X_Department = ''" + xDepartment + "''";
            if (xPosition !=null && xPosition != "")
            {
                if (X_Cond == "")
                    X_Cond = " and X_Position = ''" + xPosition + "''";
                else
                    X_Cond += " and X_Position = ''" + xPosition + "''";
            }

                    SortedList ProParams = new SortedList();
                    ProParams.Add("N_CompanyID", nCompanyID);
                    ProParams.Add("N_TransID", nBatchID);
                    ProParams.Add("N_PAyrunID", payRunID);
                    ProParams.Add("X_Cond", X_Cond);
                    ProParams.Add("N_FnYearID", nFnYearId);
                    if (bAllBranchData == false)
                    ProParams.Add("N_BranchID", nBranchID);
                    else
                    ProParams.Add("N_BranchID", 0);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    mst = dLayer.ExecuteDataTablePro("SP_Pay_AdditionDeductionEmployeeList", ProParams , connection);

                    SortedList ProParam2 = new SortedList();
                    ProParam2.Add("N_CompanyID", nCompanyID);
                    ProParam2.Add("N_PayrunID", payRunID);
                    ProParam2.Add("N_FnYearID", nFnYearId);
                    ProParam2.Add("N_BatchID", nBatchID);
                    dt = dLayer.ExecuteDataTablePro("SP_Pay_SelAddOrDed", ProParam2 , connection);
                    if (dt.Rows.Count > 0)
                        dt.Columns.Add("N_SaveChanges");

                    mst = myFunctions.AddNewColumnToDataTable(mst,"details",typeof(DataTable),null);
                    mst.AcceptChanges();
                    
                    foreach (DataRow mstVar in mst.Rows){
                        DataTable dtNode = new DataTable();
                    //     foreach (DataRow dtVar in dt.Rows){
                    //         if ( dtVar["N_EmpID"].ToString() != mstVar["N_EmpID"].ToString()) continue;

                    //         // if (myFunctions.getIntVAL(dtVar["N_PayMethod"].ToString()) == 4)
                    //         // {
                    //         //     object objRate = null;
                    //         //     SortedList param = new SortedList();
                    //         //     param.Add("@nCompanyID", nCompanyID);
                    //         //     param.Add("@nEmpID", dtVar["N_EmpID"].ToString());
                    //         //     param.Add("@nFnYearId", nFnYearId);
                    //         //     param.Add("@nPayID", dtVar["N_PayID"].ToString());
                    //         //     objRate = dLayer.ExecuteScalar("Select isnull(N_Value,0) as N_Amount from vw_EmpPayInformation Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearId and N_EmpID=@nEmpID and N_PayID=@nPayID",param,connection);
                    //         //     if (objRate != null)
                    //         //     {
                    //         //         dtVar["N_Percentage"] = myFunctions.getFloatVAL(objRate.ToString()).ToString();
                    //         //     }
                    //         // }
                    //         // SortedList param2 = new SortedList();
                    //         //     param2.Add("@nCompanyID", nCompanyID);
                    //         //     param2.Add("@nPayTypeID", dtVar["N_PayTypeID"].ToString());
                    //         // object N_Result = dLayer.ExecuteScalar("Select N_Type from Pay_PayType Where N_PayTypeID =@nPayTypeID and N_CompanyID=@nCompanyID",param2,connection);
                    //         // if (N_Result != null)
                    //         // {
                    //         //     if (myFunctions.getIntVAL(N_Result.ToString()) == 1){
                    //         //     dtVar["N_PayRate"] = -1 * myFunctions.getVAL( dtVar["N_PayRate"].ToString());
                    //         //     dtVar["N_Value"] = -1 * myFunctions.getVAL( dtVar["N_PayRate"].ToString());
                    //         //     }
                    //         // }
                    //         // dtNode.Rows.Add(dtVar.ItemArray);
                    //          //dtNode = dtVar.CopyToDataTable();
                    // }

                    DataRow[] drEmpDetails = dt.Select("N_EmpID = " + mstVar["N_EmpID"].ToString());
                    if(drEmpDetails.Length>0){
                    dtNode = drEmpDetails.CopyToDataTable();
                    dtNode.AcceptChanges();
                    mstVar["details"] = dtNode;
                    }
                }
                mst.AcceptChanges();
                mst = _api.Format(mst);
                if (mst.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(mst));
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
