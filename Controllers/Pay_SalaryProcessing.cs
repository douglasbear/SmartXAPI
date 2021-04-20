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
              dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 190;
        }

        [HttpGet("empList")]
        public ActionResult GetEmpList(string xBatch, int nFnYearID, string payRunID, string xDepartment, string xPosition, int nAddDedID, bool bAllBranchData, int nBranchID, int month, int year)
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
                    SortedList batchParams = new SortedList();
                    if (xBatch != null)
                    {
                        batchParams.Add("@nCompanyID", nCompanyID);
                        batchParams.Add("@nFnYearId", nFnYearID);
                        batchParams.Add("@xBatch", xBatch);
                        object nBatchID = dLayer.ExecuteScalar("select N_TransID from Pay_PaymentMaster where n_CompanyID=@nCompanyID and N_FnYearId=@nFnYearID and X_Batch=@xBatch", batchParams, connection);

                        if (nBatchID != null)
                            ProParams.Add("N_TransID", nBatchID);
                    }else{
                            ProParams.Add("N_TransID", 0);
                    }

                    mst = dLayer.ExecuteDataTablePro("SP_Pay_SelEmployeeList4Process", ProParams, connection);

                    SortedList ProParam2 = new SortedList();
                    ProParam2.Add("N_CompanyID", nCompanyID);
                    ProParam2.Add("N_PayrunID", payRunID);

                    ProParam2.Add("N_Month", month);
                    ProParam2.Add("N_Year", year);
                    ProParam2.Add("N_FnYearID", nFnYearID);
                    ProParam2.Add("N_Days", DateTime.DaysInMonth(year, month));
                    ProParam2.Add("N_BatchID", nAddDedID > 0 ? 1 : 0);

                    dt = dLayer.ExecuteDataTablePro("SP_Pay_SelSalaryDetailsForProcess", ProParam2, connection);
                    if (dt.Rows.Count > 0)
                    {
                        dt = myFunctions.AddNewColumnToDataTable(dt, "N_SaveChanges", typeof(int), 0);
                        dt = myFunctions.AddNewColumnToDataTable(dt, "N_Additions", typeof(string), "");
                        dt = myFunctions.AddNewColumnToDataTable(dt, "N_Deductions", typeof(string), "");
                    }
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        if (myFunctions.getIntVAL(dtRow["N_Type"].ToString()) == 0)
                            dtRow["N_Additions"] = dtRow["n_Payrate"].ToString();
                        else
                            dtRow["N_Deductions"] = dtRow["n_Payrate"].ToString();
                    }


                    bool B_ShowBenefitsInGrid = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Payroll", "Show Benefits", "N_Value", nCompanyID, dLayer, connection)));
                    DataTable PayPayMaster = new DataTable();
                    if (B_ShowBenefitsInGrid)
                    {
                        PayPayMaster = dLayer.ExecuteDataTable("Select N_PaymentId,N_PayID from Pay_PayMaster where N_CompanyID=" + nCompanyID + " and N_FnyearID=" + nFnYearID, connection);
                    }

                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        if (myFunctions.getVAL(dt.Rows[i]["N_PayRate"].ToString()) == 0)
                            dt.Rows[i].Delete();
                        if (B_ShowBenefitsInGrid)
                        {
                            if (ValidateBenefits(myFunctions.getIntVAL(dt.Rows[i]["N_PayID"].ToString()), myFunctions.getIntVAL(dt.Rows[i]["N_Type"].ToString()), PayPayMaster))
                            {
                                dt.Rows[i].Delete();
                            }
                        }

                    }
                    dt.AcceptChanges();
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

        private bool ValidateBenefits(int PayID, int type, DataTable PayPayMaster)
        {
            if (type == 1) return false;
            DataRow[] dr = PayPayMaster.Select("N_PayID=" + PayID);
            if (dr != null && dr.Length > 0)
            {
                string obj = dr[0]["N_PaymentId"].ToString();
                if (myFunctions.getIntVAL(obj.ToString()) == 6 || myFunctions.getIntVAL(obj.ToString()) == 264 || myFunctions.getIntVAL(obj.ToString()) == 7)
                    return true;
            }
            return false;
        }


          [HttpGet("Dashboardlist")]
        public ActionResult SalaryProcessingDashboardList(int nFnYearId,int nPage,int nSizeperpage,string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandCount = "";
            int Count= (nPage - 1) * nSizeperpage;
            string sqlCommandText ="";
            string Searchkey = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnYearId);

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (N_TransID like '%" + xSearchkey + "%'or Batch like '%" + xSearchkey + "%' or  N_PayRunID like '%" + xSearchkey + "%' )";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_TransID desc";
            else
             xSortBy = " order by " + xSortBy;
             
             if(Count==0)
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_PayTransaction_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 ";
            else
                sqlCommandText = "select top("+ nSizeperpage +") * from vw_PayTransaction_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 and N_TransID not in (select top("+ Count +") N_TransID from vw_PayTransaction_Disp where N_CompanyID=@p1 )";
            

            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    sqlCommandCount = "select count(*) as N_Count  from vw_PayTransaction_Disp where N_CompanyID=@p1 and N_FnYearID=@p2";
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
                return BadRequest(_api.Error(e));
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
                DataTable EmployeesTable = ds.Tables["employees"];

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
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nPayRunID", nPayRunID);
                    int FormID = 0;
                    int N_IsAuto = 0;
                    int N_TransDetailsID = 0;
                    if (x_Batch.Trim() == "@Auto")
                    {
                        bool OK = true;
                        int NewNo = 0, loop = 1;
                        while (OK)
                        {
                            NewNo = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(Count(*),0) + " + loop + " As Count FRom Pay_MonthlyAddOrDed Where N_CompanyID=@nCompanyID  And N_PayRunID =@nPayRunID", Params, connection, transaction).ToString());
                            x_Batch = nPayRunID + "" + NewNo.ToString("0#");
                            if (myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Isnull(Count(*),0) FRom Pay_MonthlyAddOrDed Where N_CompanyID=@nCompanyID And X_Batch = '" + x_Batch + "'", Params, connection, transaction).ToString()) == 0)
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

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " And N_FnyearID = " + nFnYearId + " and X_Batch='" + x_Batch + "'";
                    int N_TransID = dLayer.SaveData("Pay_PaymentMaster", "N_TransID", DupCriteria, "", MasterTable, connection, transaction);
                    if (N_TransID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {
                        //Delete Existing Data
                        dLayer.DeleteData("Pay_PaymentDetails", "N_TransID", N_TransID, "N_CompanyID =" + nCompanyID + " and N_FormID=190", connection, transaction);
                        dLayer.ExecuteScalar("Update Pay_LoanIssueDetails Set N_RefundAmount =Null,D_RefundDate =Null,N_PayRunID =Null,N_TransDetailsID =Null,B_IsLoanClose =Null  Where N_CompanyID =" + nCompanyID + " and N_PayrunID = " + N_TransID, connection, transaction);

                        int row = 0;
                        foreach (DataRow MasterVar in EmployeesTable.Rows)
                        {
                            double N_TotalSalary = 0;
                            double N_EOSAmt = 0;
                            foreach (DataRow var in DetailsTable.Rows)
                            {

                                if (MasterVar["N_EmpID"].ToString() != var["N_EmpID"].ToString()) continue;
                                double Amount = 0;
                                if (myFunctions.getVAL(var["N_PayRate"].ToString()) < 0)
                                    Amount = (-1) * myFunctions.getVAL(var["N_PayRate"].ToString());
                                else
                                    Amount = myFunctions.getVAL(var["N_PayRate"].ToString());

                                if (Amount == 0 && myFunctions.getVAL(var["N_Value"].ToString()) == 0) continue;
                                //if (Amount == 0)
                                //    Amount = myFunctions.getVAL(var["N_Value"].ToString());
                                if (myFunctions.getIntVAL(var["N_Type"].ToString()) == 0)
                                    N_TotalSalary += Amount;
                                else
                                    N_TotalSalary -= Amount;

                                if (myFunctions.getIntVAL(var["N_PayTypeID"].ToString()) == 11)
                                {
                                    N_EOSAmt += Amount;
                                }
                                var["N_PayRate"] = Amount;
                                var["N_TransID"] = N_TransID;

                                //dba.SaveData(ref N_TransDetailsID, "Pay_PaymentDetails", "N_TransDetailsID", myFunctions.getIntVAL(var["N_TransDetailsID"].ToString()).ToString(), FieldList, FieldValues, DupCriteria, "");

                                // if (var["N_IsLoan"].ToString() == "1")
                                // {
                                //     object N_result = null;
                                //     dba.SaveData(ref N_result, "Pay_LoanIssueDetails", "N_LoanTransDetailsID", myFunctions.getIntVAL(var["N_LoanTransDetailsID"].ToString()).ToString(), "D_RefundDate,N_RefundAmount,N_PayRunID,N_TransDetailsID,B_IsLoanClose", "'" + myFunctions.getDateVAL(dtpCreationDate.Value) + "'|" + Amount.ToString() + "|" + N_TransID + "|" + N_TransDetailsID + "|0", "", "", "N_CompanyID = " + myCompanyID._CompanyID + " and N_LoanTransID = " + var["N_LoanTransID"].ToString());
                                // }



                                //--------------------------------------------------------

                            }
                            if (N_TotalSalary < 0)
                            {
                                if (N_TotalSalary + N_EOSAmt < 0)
                                {
                                    transaction.Rollback();
                                    return Ok(_api.Error("-ve Salary"));
                                }
                            }
                        }
                        DetailsTable.Columns.Remove("n_PayTypeID");
                        DetailsTable.AcceptChanges();
                        N_TransDetailsID = dLayer.SaveData("Pay_PaymentDetails", "N_TransDetailsID", DetailsTable, connection, transaction);
                        if (myFunctions.getIntVAL(N_TransDetailsID.ToString()) <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Error"));

                        }



                        transaction.Commit();
                        return Ok(_api.Success("Saved"));

                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }


        [HttpGet("dummy")]
        public ActionResult GetDepartmentDummy()
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(connectionString))
                {
                    Con.Open();
                    string sqlCommandText = "select * from Pay_PaymentDetails ";
                    DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, Con);
                    masterTable = _api.Format(masterTable, "master");

                    if (masterTable.Rows.Count == 0) { return Ok(new { }); }
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add(masterTable);
                    return Ok(dataSet);
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

    }
}
