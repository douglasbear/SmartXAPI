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
        public ActionResult GetJobTitle(string xBatch, int nFnYearID, string payRunID, string xDepartment, string xPosition, bool bAllBranchData, int nBranchID)
        {
            DataTable mst = new DataTable();
            DataTable dt = new DataTable();
            DataTable node = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int Year = myFunctions.getIntVAL(payRunID.Substring(0, 4));
            int Month = myFunctions.getIntVAL(payRunID.Substring(4, 2));

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

                    object nTransID = 0;
                    object nBatchID = 0;

                    SortedList batchParams = new SortedList();
                    if (xBatch != null)
                    {
                        batchParams.Add("@nCompanyID", nCompanyID);
                        batchParams.Add("@nFnYearId", nFnYearID);
                        batchParams.Add("@xBatch", xBatch);
                        mst = dLayer.ExecuteDataTable("select * from Pay_MonthlyAddOrDed where n_CompanyID=@nCompanyID and N_FnYearId=@nFnYearID and X_Batch=@xBatch", batchParams, connection);
                        
                        if(mst.Rows.Count==0){
                             return Ok(_api.Notice("No Results Found"));
                        }
                           nTransID = myFunctions.getIntVAL(mst.Rows[0]["N_TransID"].ToString());
                           payRunID = mst.Rows[0]["N_PayRunID"].ToString();

                        // nTransID = dLayer.ExecuteScalar("select N_TransID from Pay_MonthlyAddOrDed where n_CompanyID=@nCompanyID and N_FnYearId=@nFnYearID and X_Batch=@xBatch", batchParams, connection);
                        // payRunID = dLayer.ExecuteScalar("select N_PayRunID from Pay_MonthlyAddOrDed where n_CompanyID=@nCompanyID and N_FnYearId=@nFnYearID and X_Batch=@xBatch", batchParams, connection).ToString();

                        if (nTransID != null)
                            ProParams.Add("N_TransID", nTransID);
                    }
                    else
                    {
                        ProParams.Add("N_TransID", 0);
                    }
                    ProParams.Add("N_PAyrunID", payRunID);


                    dt = dLayer.ExecuteDataTablePro("SP_Pay_AdditionDeductionEmployeeList", ProParams, connection);

if (xBatch != null)
{
                    SortedList ProParam2 = new SortedList();
                    ProParam2.Add("N_CompanyID", nCompanyID);
                    ProParam2.Add("N_PayrunID", payRunID);
                    ProParam2.Add("N_FnYearID", nFnYearID);
                    ProParam2.Add("N_BatchID", nTransID);
                    node = dLayer.ExecuteDataTablePro("SP_Pay_SelAddOrDed", ProParam2, connection);
                    if (node.Rows.Count > 0)
                    {
                        node.Columns.Add("N_SaveChanges");
                        node.Columns.Add("N_Type");
                        node = myFunctions.AddNewColumnToDataTable(node, "N_Amount", typeof(string), null);
                    }

                    SortedList paytypeParam = new SortedList();
                    paytypeParam.Add("@nCompanyID", nCompanyID);
                    DataTable payType = dLayer.ExecuteDataTable("Select N_PayTypeID,N_Type from Pay_PayType Where  N_CompanyID=@nCompanyID", paytypeParam, connection);

                    dt = myFunctions.AddNewColumnToDataTable(dt, "details", typeof(DataTable), null);
                    dt.AcceptChanges();
                    node.AcceptChanges();
////////////
                    object SalaryProcess = myFunctions.ReturnSettings("HR", "Salary Process", "N_Value", nCompanyID, dLayer, connection);
                    object Periodvalue = myFunctions.ReturnSettings("Payroll", "Period Settings", "N_Value", nCompanyID, dLayer, connection);
                 
                                int daysinWork = 0;
                                int days = 0;
                                double TotalHrs = 0;
                                if (Periodvalue == null) daysinWork = 0;
                                else
                                    daysinWork = myFunctions.getIntVAL(Periodvalue.ToString());
                                DateTime dtStartDate = new DateTime(Year, Month, 1);
                                if (SalaryProcess != null && myFunctions.getIntVAL(SalaryProcess.ToString()) == 1)
                                    days = 30;
                                else
                                    days = DateTime.DaysInMonth(Year, Month) - myFunctions.getIntVAL(Periodvalue.ToString());
                                DateTime dt1, dt2;
                                dt2 = dtStartDate.AddDays(myFunctions.getIntVAL(days.ToString()) - 1);
                                int lastdays = myFunctions.getIntVAL(Periodvalue.ToString());
                                dt1 = dtStartDate.AddDays(-lastdays);
//////////////////                           

                    node = myFunctions.AddNewColumnToDataTable(node,"TotalHrs",typeof(int),0);

                    foreach (DataRow dtVar in dt.Rows)
                    {
                        DataTable dtNode = new DataTable();
                        foreach (DataRow nodeVar in node.Rows)
                        {
                            if (dtVar["N_EmpID"].ToString() != nodeVar["N_EmpID"].ToString()) continue;
                            bool B_PostedAccount = false;
                            if (myFunctions.getIntVAL(nodeVar["N_Processed"].ToString()) > 0)
                                B_PostedAccount = true;
                            if (myFunctions.getIntVAL(nodeVar["N_PayMethod"].ToString()) == 4)
                            {
                                object objRate = null;
                                SortedList param = new SortedList();
                                param.Add("@nCompanyID", nCompanyID);
                                param.Add("@nEmpID", nodeVar["N_EmpID"].ToString());
                                param.Add("@nFnYearId", nFnYearID);
                                param.Add("@nPayID", nodeVar["N_PayID"].ToString());
                                objRate = dLayer.ExecuteScalar("Select isnull(N_Value,0) as N_Amount from vw_EmpPayInformation Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearId and N_EmpID=@nEmpID and N_PayID=@nPayID and D_EffectiveDate=(select MAX(D_EffectiveDate) from vw_EmpPayInformation where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearId and N_EmpID=@nEmpID and N_PayID=@nPayID and D_EffectiveDate<=@dDate)", param, connection);
                                if (objRate != null)
                                {
                                    nodeVar["N_Percentage"] = myFunctions.getFloatVAL(objRate.ToString()).ToString();
                                }
                            }


                            if (myFunctions.getIntVAL(nodeVar["N_PayTypeID"].ToString()) != 17)
                            {
                                object obj =  dLayer.ExecuteScalar("SELECT  [dbo].[SP_TimeSheetCalc_TotalHours](" + nCompanyID + ",'" + myFunctions.getDateVAL(dt1) + "','" + myFunctions.getDateVAL(dt2) + "'," + myFunctions.getIntVAL(dtVar["N_EmpID"].ToString()) + ")", connection);
                                if (obj != null)
                                    TotalHrs = myFunctions.getVAL(obj.ToString());
                                else
                                    TotalHrs = 240;
                                nodeVar["TotalHrs"] = TotalHrs;

                            }

                            if (B_PostedAccount)
                            {
                                nodeVar["N_Amount"] = nodeVar["N_PayRate"].ToString();

                            }
                            else
                            {
                                if (myFunctions.getVAL(nodeVar["N_PayRate"].ToString()) == 0)
                                {
                                    nodeVar["N_Amount"] = nodeVar["N_Value"].ToString();
                                }
                                else
                                {
                                    nodeVar["N_Amount"] = nodeVar["N_PayRate"].ToString();
                                }
                            }

                            DataRow[] payTypeRow = payType.Select("N_PayTypeID = " + nodeVar["N_PayTypeID"].ToString());
                            if (payTypeRow.Length > 0)
                            {
                                if (myFunctions.getIntVAL(payTypeRow[0]["N_Type"].ToString()) == 1)
                                {
                                    nodeVar["N_Amount"] = -1 * myFunctions.getVAL(nodeVar["N_Amount"].ToString());
                                    nodeVar["N_Type"] = payTypeRow[0]["N_Type"];
                                }
                            }
                        }
                        dt.AcceptChanges();

                        DataRow[] drEmpDetails = node.Select("N_EmpID = " + dtVar["N_EmpID"].ToString());
                        if (drEmpDetails.Length > 0)
                        {
                            dtNode = drEmpDetails.CopyToDataTable();
                            dtNode.AcceptChanges();
                            dtVar["details"] = dtNode;
                        }
                    }

}
                    mst.AcceptChanges();
                    dt.AcceptChanges();
                    dt = _api.Format(dt);
                    mst = _api.Format(mst);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Notice("No Results Found"));
                    }
                    else
                    {
                        SortedList Output = new SortedList();
                        Output.Add("master",mst);
                        Output.Add("details",dt);
                        return Ok(_api.Success(Output));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }



        [HttpGet("empElements")]
        public ActionResult GetEmpElements(int nEmpID,string payRunID,int nFnYearID,int nTransID,DateTime payrunDate)
        {
            DataTable dt = new DataTable();
            int nCompanyID = myFunctions.GetCompanyID(User);
            int Year = myFunctions.getIntVAL(payRunID.Substring(0, 4));
            int Month = myFunctions.getIntVAL(payRunID.Substring(4, 2));
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SortedList ProParam2 = new SortedList();
                    ProParam2.Add("N_CompanyID", nCompanyID);
                    ProParam2.Add("N_PayrunID", payRunID);
                    ProParam2.Add("N_FnYearID", nFnYearID);
                    ProParam2.Add("N_BatchID", nTransID);
                    dt = dLayer.ExecuteDataTablePro("SP_Pay_SelAddOrDed", ProParam2, connection);
                    if (dt.Rows.Count > 0)
                    {
                        dt.Columns.Add("N_SaveChanges");
                        dt.Columns.Add("N_Type");
                        dt = myFunctions.AddNewColumnToDataTable(dt, "N_Amount", typeof(string), null);
                    }

                    SortedList paytypeParam = new SortedList();
                    paytypeParam.Add("@nCompanyID", nCompanyID);
                    DataTable payType = dLayer.ExecuteDataTable("Select N_PayTypeID,N_Type from Pay_PayType Where  N_CompanyID=@nCompanyID", paytypeParam, connection);
                    dt.AcceptChanges();
////////////
                    object SalaryProcess = myFunctions.ReturnSettings("HR", "Salary Process", "N_Value", nCompanyID, dLayer, connection);
                    object Periodvalue = myFunctions.ReturnSettings("Payroll", "Period Settings", "N_Value", nCompanyID, dLayer, connection);
                 
                    int daysinWork = 0;
                    int days = 0;
                    double TotalHrs = 0;
                    if (Periodvalue == null) daysinWork = 0;
                    else
                        daysinWork = myFunctions.getIntVAL(Periodvalue.ToString());
                    DateTime dtStartDate = new DateTime(Year, Month, 1);
                    if (SalaryProcess != null && myFunctions.getIntVAL(SalaryProcess.ToString()) == 1)
                        days = 30;
                    else
                        days = DateTime.DaysInMonth(Year, Month) - myFunctions.getIntVAL(Periodvalue.ToString());
                    DateTime dt1, dt2;
                    dt2 = dtStartDate.AddDays(myFunctions.getIntVAL(days.ToString()) - 1);
                    int lastdays = myFunctions.getIntVAL(Periodvalue.ToString());
                    dt1 = dtStartDate.AddDays(-lastdays);
//////////////////                           

                    dt = myFunctions.AddNewColumnToDataTable(dt,"TotalHrs",typeof(int),0);
                        DataTable dtNode = new DataTable();
                        foreach (DataRow dtVar in dt.Rows)
                        {
                            if (dtVar["N_EmpID"].ToString() != nEmpID.ToString() ) continue;
                            bool B_PostedAccount = false;
                            if (myFunctions.getIntVAL(dtVar["N_Processed"].ToString()) > 0)
                                B_PostedAccount = true;
                            if (myFunctions.getIntVAL(dtVar["N_PayMethod"].ToString()) == 4)
                            {
                                object objRate = null;
                                SortedList param = new SortedList();
                                param.Add("@nCompanyID", nCompanyID);
                                param.Add("@nEmpID", dtVar["N_EmpID"].ToString());
                                param.Add("@nFnYearId", nFnYearID);
                                param.Add("@nPayID", dtVar["N_PayID"].ToString());
                                param.Add("@dDate", payrunDate);
                                objRate = dLayer.ExecuteScalar("Select isnull(N_Value,0) as N_Amount from vw_EmpPayInformation Where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearId and N_EmpID=@nEmpID and N_PayID=@nPayID and D_EffectiveDate=(select MAX(D_EffectiveDate) from vw_EmpPayInformation where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearId and N_EmpID=@nEmpID and N_PayID=@nPayID and D_EffectiveDate<=@dDate)", param, connection);
                                if (objRate != null)
                                {
                                    dtVar["N_Percentage"] = myFunctions.getFloatVAL(objRate.ToString()).ToString();
                                }
                            }


                            if (myFunctions.getIntVAL(dtVar["N_PayTypeID"].ToString()) != 17)
                            {
                                object obj =  dLayer.ExecuteScalar("SELECT  [dbo].[SP_TimeSheetCalc_TotalHours](" + nCompanyID + ",'" + myFunctions.getDateVAL(dt1) + "','" + myFunctions.getDateVAL(dt2) + "'," + myFunctions.getIntVAL(dtVar["N_EmpID"].ToString()) + ")", connection);
                                if (obj != null)
                                    TotalHrs = myFunctions.getVAL(obj.ToString());
                                else
                                    TotalHrs = 240;
                                dtVar["TotalHrs"] = TotalHrs;

                            }

                            if (B_PostedAccount)
                            {
                                dtVar["N_Amount"] = dtVar["N_PayRate"].ToString();

                            }
                            else
                            {
                                if (myFunctions.getVAL(dtVar["N_PayRate"].ToString()) == 0)
                                {
                                    dtVar["N_Amount"] = dtVar["N_Value"].ToString();
                                }
                                else
                                {
                                    dtVar["N_Amount"] = dtVar["N_PayRate"].ToString();
                                }
                            }

                            DataRow[] payTypeRow = payType.Select("N_PayTypeID = " + dtVar["N_PayTypeID"].ToString());
                            if (payTypeRow.Length > 0)
                            {
                                if (myFunctions.getIntVAL(payTypeRow[0]["N_Type"].ToString()) == 1)
                                {
                                    dtVar["N_Amount"] = -1 * myFunctions.getVAL(dtVar["N_Amount"].ToString());
                                    dtVar["N_Type"] = payTypeRow[0]["N_Type"];
                                }
                            }
                        }
                        dt.AcceptChanges();

                        DataRow[] drEmpDetails = dt.Select("N_EmpID = " + nEmpID.ToString());
                        if (drEmpDetails.Length > 0)
                        {
                            dtNode = drEmpDetails.CopyToDataTable();
                            dtNode.AcceptChanges();
                        }
                        dtNode = _api.Format(dtNode);
                        return Ok(_api.Success(dtNode));

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
<<<<<<< HEAD
=======
                           // double d2 = (double)DetailsTable.Rows[i]["n_PayRate"];
>>>>>>> ab17d68f9476cc14b054af6d974a0ec9347db496

                            if (Amount == 0)
                            {
                                if (myFunctions.getIntVAL(mstVar["n_TransDetailsID"].ToString()) != 0)
                                { dLayer.DeleteData("Pay_MonthlyAddOrDedDetails", "N_TransDetailsID", myFunctions.getIntVAL(mstVar["n_TransDetailsID"].ToString()), "N_CompanyID = " + nCompanyID, connection, transaction); }
                                DetailsTable.Rows[i].Delete();
                                continue;
                            }
                            else if (myFunctions.getIntVAL(mstVar["n_TransDetailsID"].ToString()) != 0 && mstVar["n_FormID"].ToString().Trim() != "")
                            {
                                N_TransDetailsID = myFunctions.getIntVAL(dLayer.ExecuteScalar("Select N_TransDetailsID from Pay_MonthlyAddOrDedDetails Where N_PayID=" + myFunctions.getVAL(mstVar["n_PayID"].ToString()) + " and N_TransID=" + N_OldTransID.ToString() + " and n_EmpID=" + mstVar["n_EmpID"].ToString() + " and N_FormID=" + mstVar["n_FormID"].ToString() + " and N_CompanyID= " + nCompanyID, connection, transaction).ToString());
                                FormID = myFunctions.getIntVAL(mstVar["N_FormID"].ToString());
                            }
                            else if (mstVar["n_FormID"].ToString().Trim() == "" || myFunctions.getIntVAL(mstVar["n_FormID"].ToString()) == 0)
                                FormID = this.FormID;


                            if (Amount < 0)
                                Amount = Amount * -1;

                            N_IsAuto = 0;
                            object N_ResultObj = dLayer.ExecuteScalar("Select B_TimeSheetEntry from Pay_MonthlyAddOrDedDetails Where N_PayID =" + myFunctions.getVAL(mstVar["n_PayID"].ToString()) + " and N_TransID=" + N_OldTransID.ToString() + " and N_TransDetailsID=" + myFunctions.getVAL(mstVar["n_TransDetailsID"].ToString()) + " and N_EmpID=" + mstVar["n_EmpID"].ToString() + " and N_CompanyID= " + nCompanyID + " and N_FormID=216", connection, transaction);
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
                        N_TransDetailsID = myFunctions.getIntVAL(dLayer.SaveData("Pay_MonthlyAddOrDedDetails", "N_TransDetailsID", DetailsTable, connection, transaction).ToString());
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

        [HttpGet("dashboardList")]
        public ActionResult PayyAddDedList(int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (x_Batch like '%" + xSearchkey + "%' or right(REPLACE(CONVERT(CHAR(11), x_PayrunText, 106),' ','-'),8) like '%" + xSearchkey + "%' or X_Notes like '%" + xSearchkey + "%' or REPLACE(CONVERT(CHAR(11), D_TransDate, 106),' ','-') like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_Batch desc,D_TransDate desc";
            else
                xSortBy = " order by " + xSortBy;


            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") n_CompanyID,N_TransID,x_Batch,right(REPLACE(CONVERT(CHAR(11), x_PayrunText, 106),' ','-'),8) as x_PayrunText,D_TransDate,X_Notes from Pay_MonthlyAddOrDed where N_CompanyID=@nCompanyId " + Searchkey + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") n_CompanyID,N_TransID,x_Batch,right(REPLACE(CONVERT(CHAR(11), x_PayrunText, 106),' ','-'),8) as x_PayrunText,D_TransDate,X_Notes from Pay_MonthlyAddOrDed where N_CompanyID=@nCompanyId " + Searchkey +" and  N_PayRunID not in (select top(" + Count + ") N_PayRunID from Pay_MonthlyAddOrDed  where N_CompanyID=@nCompanyId ) "+Searchkey+xSortBy;

            Params.Add("@nCompanyId", nCompanyId);
            // Params.Add("@nFnYearId", nFnYearId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from Pay_MonthlyAddOrDed where N_CompanyID=@nCompanyId "+ Searchkey;
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Success(OutPut));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }

                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }



        [HttpDelete("delete")]
        public ActionResult DeleteData(int nTransID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                        Results = dLayer.DeleteData("Pay_MonthlyAddOrDed", "N_TransID", nTransID, "N_CompanyID=" + myFunctions.GetCompanyID(User),connection,transaction);
                        
                        if (Results <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable to delete batch"));
                        }
                        Results = dLayer.DeleteData("Pay_MonthlyAddOrDedDetails", "N_TransID", nTransID, "N_CompanyID=" + myFunctions.GetCompanyID(User),connection,transaction);

                        if (Results <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error("Unable to delete batch"));
                        }
                    
                    transaction.Commit();
                    return Ok(_api.Success("Batch deleted"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }

        }



    }
}
