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
            FormID = 208;
        }

        [HttpGet("empList")]
        public ActionResult GetJobTitle(int nBatchID, int nFnYearID, string payRunID, string xDepartment, string xPosition, bool bAllBranchData, int nBranchID)
        {
            DataTable mst = new DataTable();
            DataTable dt = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);

            string X_Cond = "";
            if (xDepartment != null && xDepartment != "")
                X_Cond = " and X_Department = ''" + xDepartment + "''";
            if (xPosition != null && xPosition != "")
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
            ProParams.Add("N_FnYearID", nFnYearID);
            if (bAllBranchData == false)
                ProParams.Add("N_BranchID", nBranchID);
            else
                ProParams.Add("N_BranchID", 0);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    mst = dLayer.ExecuteDataTablePro("SP_Pay_AdditionDeductionEmployeeList", ProParams, connection);

                    SortedList ProParam2 = new SortedList();
                    ProParam2.Add("N_CompanyID", nCompanyID);
                    ProParam2.Add("N_PayrunID", payRunID);
                    ProParam2.Add("N_FnYearID", nFnYearID);
                    ProParam2.Add("N_BatchID", nBatchID);
                    dt = dLayer.ExecuteDataTablePro("SP_Pay_SelAddOrDed", ProParam2, connection);
                    if (dt.Rows.Count > 0)
                    {
                        dt.Columns.Add("N_SaveChanges");
                        dt.Columns.Add("N_Type");
                    }

                    SortedList paytypeParam = new SortedList();
                    paytypeParam.Add("@nCompanyID", nCompanyID);
                    DataTable payType = dLayer.ExecuteDataTable("Select N_PayTypeID,N_Type from Pay_PayType Where  N_CompanyID=@nCompanyID", paytypeParam, connection);

                    mst = myFunctions.AddNewColumnToDataTable(mst, "details", typeof(DataTable), null);
                    mst.AcceptChanges();

                    foreach (DataRow mstVar in mst.Rows)
                    {
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
                        if (drEmpDetails.Length > 0)
                        {
                            foreach (DataRow empVar in drEmpDetails)
                            {
                                DataRow[] payTypeRow = payType.Select("N_PayTypeID = " + empVar["N_PayTypeID"]);
                                if (payTypeRow.Length > 0)
                                {
                                    empVar["N_Type"] = payTypeRow[0]["N_Type"];
                                }
                            }

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

        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable = ds.Tables["master"];
                DataTable DetailsTable = ds.Tables["details"];

                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nPayRunID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_PayrunID"].ToString());
                string x_Batch = MasterTable.Rows[0]["x_Batch"].ToString();
                int N_OldTransID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_TransID"].ToString());
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    Params.Add("@nCompanyID",nCompanyID);
                    Params.Add("@nPayRunID",nPayRunID);
                    int FormID=0;
                    int N_IsAuto=0;
                    int N_TransDetailsID = 0;
                    if (x_Batch.Trim() == "@Auto")
                    {
                        bool OK = true;
                        int NewNo = 0, loop = 1;
                        while (OK)
                        {
                            NewNo = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(Count(*),0) + " + loop + " As Count FRom Pay_MonthlyAddOrDed Where N_CompanyID=@nCompanyID  And N_PayRunID =@nPayRunID",Params,connection,transaction).ToString());
                            x_Batch = nPayRunID +"" + NewNo.ToString("0#");
                            if (myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(Count(*),0) FRom Pay_MonthlyAddOrDed Where N_CompanyID=@nCompanyID And X_Batch = '" + x_Batch + "'", Params,connection,transaction).ToString()) == 0)
                            {
                                OK = false;
                            }
                            loop += 1;
                        }
                        if (x_Batch == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable to generate batch"));
                            
                        }
                        MasterTable.Rows[0]["x_Batch"] = x_Batch;
                    }


                    int N_TransID = dLayer.SaveData("Pay_MonthlyAddOrDed", "N_TransID", MasterTable, connection, transaction);
                    if (N_TransID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {

                    foreach (DataRow mstVar in DetailsTable.Rows)
                    {

                            // if (var["N_SaveChanges"].ToString().Trim() == "" && (var["n_TransDetailsID"].ToString()).Trim() != "") 
                            //     continue;

                            
                            double Amount = myFunctions.getVAL(mstVar["n_PayRate"].ToString());

                            if (Amount == 0 && myFunctions.getVAL(mstVar["n_Value"].ToString()) == 0)
                            {
                                if (myFunctions.getIntVAL(mstVar["n_TransDetailsID"].ToString()) != 0)
                                    dLayer.DeleteData("Pay_MonthlyAddOrDedDetails", "N_TransDetailsID", myFunctions.getIntVAL(mstVar["n_TransDetailsID"].ToString()), "N_CompanyID = " +nCompanyID ,connection,transaction);
                                continue;
                            }
                            else if (myFunctions.getIntVAL(mstVar["n_TransDetailsID"].ToString()) != 0 && mstVar["n_FormID"].ToString().Trim() != "")
                            {
                                N_TransDetailsID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select N_TransDetailsID from Pay_MonthlyAddOrDedDetails Where N_PayID=" + myFunctions.getVAL(mstVar["n_PayID"].ToString()) + " and N_TransID=" + N_OldTransID.ToString() + " and n_EmpID=" + mstVar["n_EmpID"].ToString() + " and N_FormID=" + mstVar["n_FormID"].ToString() + " and N_CompanyID= " +nCompanyID,connection,transaction).ToString());
                                FormID = myFunctions.getIntVAL(mstVar["N_FormID"].ToString());
                            }
                            else if (mstVar["n_FormID"].ToString().Trim() == "" || myFunctions.getIntVAL(mstVar["n_FormID"].ToString()) == 0)
                                FormID = this.FormID;

                            if (Amount == 0)
                                Amount = myFunctions.getVAL(mstVar["n_Value"].ToString());

                            if (Amount < 0)
                                Amount = Amount * -1;

                            N_IsAuto = 0;
                            object N_ResultObj = dLayer.ExecuteScalar("Select B_TimeSheetEntry from Pay_MonthlyAddOrDedDetails Where N_PayID =" + myFunctions.getVAL(mstVar["n_PayID"].ToString()) + " and N_TransID=" + N_OldTransID.ToString() + " and N_TransDetailsID=" + myFunctions.getVAL(mstVar["n_TransDetailsID"].ToString()) + " and N_EmpID=" + mstVar["n_EmpID"].ToString() + " and N_CompanyID= " + nCompanyID + " and N_FormID=216",connection,transaction);
                            if (N_ResultObj != null)
                            {
                                N_IsAuto = 1;
                            }
                        mstVar["n_TransID"] = N_TransID;
                        mstVar["n_PayRate"] = Amount;
                        mstVar["b_TimeSheetEntry"] = N_IsAuto;
                        mstVar["n_FormID"] = FormID;
                        
                    }

                    N_TransDetailsID = myFunctions.getIntVAL(dLayer.SaveData("Pay_MonthlyAddOrDedDetails", "N_TransDetailsID",DetailsTable,connection,transaction).ToString());
                    if (myFunctions.getIntVAL(N_TransDetailsID.ToString()) <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Unable to save"));
                    }

                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

    }
}
