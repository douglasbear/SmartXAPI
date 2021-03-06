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
    [Route("salaryProcessing")]
    [ApiController]
    public class Pay_SalaryProcessing : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Pay_SalaryProcessing(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 190;
        }

        [HttpGet("empList")]
        public ActionResult GetEmpList(int nBatchID, int nFnYearID, string payRunID, string xDepartment, string xPosition,int nAddDedID, bool bAllBranchData, int nBranchID,int month,int year)
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

            ProParams.Add("N_AddBatchID", nAddDedID);
            

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    mst = dLayer.ExecuteDataTablePro("SP_Pay_SelEmployeeList4Process", ProParams, connection);

                    SortedList ProParam2 = new SortedList();
                    ProParam2.Add("N_CompanyID", nCompanyID);
                    ProParam2.Add("N_PayrunID", payRunID);

                    ProParam2.Add("N_Month", month);
                    ProParam2.Add("N_Year", year);
                    ProParam2.Add("N_FnYearID", nFnYearID);
                    ProParam2.Add("N_Days", DateTime.DaysInMonth(year, month));
                    ProParam2.Add("N_BatchID", nFnYearID);
                    ProParam2.Add("N_BatchID", nAddDedID>0?1:0);

                    dt = dLayer.ExecuteDataTablePro("SP_Pay_SelSalaryDetailsForProcess", ProParam2, connection);
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
                    for (int i = DetailsTable.Rows.Count - 1; i >= 0; i--) 
                    {
                        DataRow mstVar = DetailsTable.Rows[i];
                            // if (var["N_SaveChanges"].ToString().Trim() == "" && (var["n_TransDetailsID"].ToString()).Trim() != "") 
                            //     continue;

                            
                            double Amount = myFunctions.getVAL(mstVar["n_PayRate"].ToString());

                            if (Amount == 0 )
                            {
                                if (myFunctions.getIntVAL(mstVar["n_TransDetailsID"].ToString()) != 0)
                                    {dLayer.DeleteData("Pay_MonthlyAddOrDedDetails", "N_TransDetailsID", myFunctions.getIntVAL(mstVar["n_TransDetailsID"].ToString()), "N_CompanyID = " +nCompanyID ,connection,transaction);}
                                DetailsTable.Rows[i].Delete();
                                continue;
                            }
                            else if (myFunctions.getIntVAL(mstVar["n_TransDetailsID"].ToString()) != 0 && mstVar["n_FormID"].ToString().Trim() != "")
                            {
                                N_TransDetailsID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select N_TransDetailsID from Pay_MonthlyAddOrDedDetails Where N_PayID=" + myFunctions.getVAL(mstVar["n_PayID"].ToString()) + " and N_TransID=" + N_OldTransID.ToString() + " and n_EmpID=" + mstVar["n_EmpID"].ToString() + " and N_FormID=" + mstVar["n_FormID"].ToString() + " and N_CompanyID= " +nCompanyID,connection,transaction).ToString());
                                FormID = myFunctions.getIntVAL(mstVar["N_FormID"].ToString());
                            }
                            else if (mstVar["n_FormID"].ToString().Trim() == "" || myFunctions.getIntVAL(mstVar["n_FormID"].ToString()) == 0)
                                FormID = this.FormID;


                            if (Amount < 0)
                                Amount = Amount * -1;

                            N_IsAuto = 0;
                            object N_ResultObj = dLayer.ExecuteScalar("Select B_TimeSheetEntry from Pay_MonthlyAddOrDedDetails Where N_PayID =" + myFunctions.getVAL(mstVar["n_PayID"].ToString()) + " and N_TransID=" + N_OldTransID.ToString() + " and N_TransDetailsID=" + myFunctions.getVAL(mstVar["n_TransDetailsID"].ToString()) + " and N_EmpID=" + mstVar["n_EmpID"].ToString() + " and N_CompanyID= " + nCompanyID + " and N_FormID=216",connection,transaction);
                            if (N_ResultObj != null)
                            {
                                N_IsAuto = 1;
                            }
                        DetailsTable.Rows[i]["n_TransID"] = N_TransID;
                        DetailsTable.Rows[i]["n_PayRate"] = Amount;
                        DetailsTable.Rows[i]["b_TimeSheetEntry"] = N_IsAuto;
                        DetailsTable.Rows[i]["n_FormID"] = FormID;
                    }
                    DetailsTable.AcceptChanges();
                    N_TransDetailsID = myFunctions.getIntVAL(dLayer.SaveData("Pay_MonthlyAddOrDedDetails", "N_TransDetailsID",DetailsTable,connection,transaction).ToString());
                    if (myFunctions.getIntVAL(N_TransDetailsID.ToString()) <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Saved"));
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
