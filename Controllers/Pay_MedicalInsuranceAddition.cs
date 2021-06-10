using AutoMapper;
using SmartxAPI.Data;
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
    [Route("medicalInsuranceAddition")]
    [ApiController]
    public class Pay_MedicalInsuranceAddition : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;


        public Pay_MedicalInsuranceAddition(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1132;
        }

        [HttpGet("policyList")]
        public ActionResult GetPolicyList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select X_InsuranceCode,X_CardNo,X_InsuranceName,X_VendorName,X_StartDate,X_EndDate,N_MedicalInsID,N_CompanyID,N_VendorID from vw_MedicalInsurance where N_CompanyID=@nCompanyID  group By  X_InsuranceCode,X_CardNo,X_InsuranceName,X_VendorName,X_StartDate,X_EndDate,N_MedicalInsID,N_CompanyID,N_VendorID ";
            Params.Add("@nCompanyID", nCompanyID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(_api.Error(e));
            }
        }
        [HttpGet("policyAgent")]
        public ActionResult GePolicyAgent()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_MedicalInsuranceAddition_Vendor  ";
            Params.Add("@nCompanyID", nCompanyID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("employeeRelationList")]
        public ActionResult GetEmployeeRelationList(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from vw_EmployeeRealationMaster where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID  and N_Status not in(2,3)  ";
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(_api.Error(e));
            }
        }


        [HttpGet("classList")]
        public ActionResult GetClassList(int nMedicalInsID, int nDependentID, int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nMedicalInsID", nMedicalInsID);
            Params.Add("@nDependentID", nDependentID);
            string sqlCommandText = "";
            if (nDependentID > 0)
            {
                sqlCommandText = "select * from vw_InsuranceAmountCategoryWise where N_CompanyID=@nCompanyID  and N_InsuranceID =@nMedicalInsID and  EmpType=172 or EmpType=0  ";

            }
            else
            {
                sqlCommandText = "select * from vw_InsuranceAmountCategoryWise where N_CompanyID=@nCompanyID  and N_InsuranceID =@nMedicalInsID and  EmpType=171 or EmpType=0  ";
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(_api.Error(e));
            }
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nAdditionID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_AdditionID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    string X_PolicyCode = MasterTable.Rows[0]["x_PolicyCode"].ToString();
                    int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                    if (nAdditionID > 0)
                    {

                        int flag = 0;
                        for (int i = 1; i < DetailTable.Rows.Count; i++)
                        {
                            if ((DetailTable.Rows[i]["n_EmpID"]) == "") continue;
                            int EmpID = myFunctions.getIntVAL(DetailTable.Rows[i]["n_EmpID"].ToString());

                            object SalaryProc = dLayer.ExecuteScalar("select * from Pay_PaymentDetails where N_EmpID=" + myFunctions.getIntVAL(EmpID.ToString()) + " and N_PayID =46 and N_CompanyID=" + nCompanyID + " and D_DateTo>='" + myFunctions.getDateVAL(Convert.ToDateTime(DetailTable.Rows[i]["d_StartDate"].ToString())) + "'",Params,connection,transaction);
                            if (SalaryProc.ToString() != null)
                                flag += 1;
                        }
                        if (flag > 0)
                        {
                         
                            return Ok("Can't update Salary already Processed!");
                        }
                    }



                    if (nAdditionID > 0)
                    {
                        dLayer.DeleteData("Pay_MedicalInsuranceAddition", "N_ReceiptId", nAdditionID, "N_CompanyID = " + nCompanyID, connection, transaction);
                    }
                    DocNo = MasterRow["x_PolicyCode"].ToString();
                    if (X_PolicyCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", FormID);
                        Params.Add("N_YearID", nFnYearID);
                        while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Pay_MedicalInsuranceAddition Where X_PolicyCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_PolicyCode = DocNo;
                        if (X_PolicyCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate")); }
                        MasterTable.Rows[0]["x_PolicyCode"] = X_PolicyCode;

                    }
                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and X_PolicyCode='" + X_PolicyCode + "'";
                    string X_Criteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;

                    nAdditionID = dLayer.SaveData("Pay_MedicalInsuranceAddition", "N_AdditionID", DupCriteria, X_Criteria, MasterTable, connection, transaction);
                    if (nAdditionID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    }
                    for (int i = DetailTable.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow mstVar = DetailTable.Rows[i];

                        DetailTable.Rows[i]["N_AdditionID"] = nAdditionID;


                    }

                    int nAdditionDetailsID = dLayer.SaveData("Pay_MedicalInsuranceAdditionDetails", "N_AdditionDetailsID", DetailTable, connection, transaction);
                    if (nAdditionDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    }
                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
        [HttpGet("list")]
        public ActionResult SalaryPayList(int nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            //int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID = myFunctions.GetUserID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandCount = "";
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string Criteria = "";
            string Searchkey = "";
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and Receipt No like '%" + xSearchkey + "%'";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_AdditionID asc";
            else
                xSortBy = " order by " + xSortBy;

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ")  N_AdditionID,X_PolicyCode,X_PolicyNo,X_VendorName,X_CardNo,X_StartDate,X_EndDate from vw_MedicalInsuranceAdditionSearch where N_CompanyID=@nCompanyId  group By  N_AdditionID,X_PolicyCode,X_PolicyNo,X_VendorName,X_CardNo,X_StartDate,X_EndDate " + Searchkey + Criteria + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") N_AdditionID,X_PolicyCode,X_PolicyNo,X_VendorName,X_CardNo,X_StartDate,X_EndDate from vw_MedicalInsuranceAdditionSearch where N_CompanyID=@nCompanyId " + Searchkey + Criteria + " and N_AdditionID not in (select top(" + Count + ") N_AdditionID from vw_MedicalInsuranceAdditionSearch where N_CompanyID=@nCompanyId  group By  N_AdditionID,X_PolicyCode,X_PolicyNo,X_VendorName,X_CardNo,X_StartDate,X_EndDate  " + Criteria + xSortBy + " ) " + xSortBy;
            Params.Add("@nCompanyId", nCompanyId);

            SortedList OutPut = new SortedList();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    sqlCommandCount = "select count(*)  as N_Count  from vw_MedicalInsuranceAdditionSearch where N_CompanyID=@nCompanyId " + Searchkey + Criteria;
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
                return Ok(_api.Error(e));
            }
        }
        [HttpGet("employeeDetails")]
        public ActionResult GetEmpDetails(int nEmpID, string xEmployeeCode, string xType, int xDepId, int nFnYearID, int N_MedicalInsID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    SortedList EmpParams = new SortedList();
                    SortedList DepParams = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    Params.Add("@nFnYearID", nFnYearID);
                    Params.Add("@xType", xType);
                    Params.Add("@nEmpID", nEmpID);
                    Params.Add("@xEmployeeCode", xEmployeeCode);
                    Params.Add("@xDepId", xDepId);

                    EmpParams.Add("@nCompanyID", nCompanyID);
                    EmpParams.Add("@nFnYearID", nFnYearID);

                    DepParams.Add("@nCompanyID", nCompanyID);





                    DataTable EmployeeTable = new DataTable();
                    DataTable RelationTable = new DataTable();
                    DataTable FamilyTable = new DataTable();
                    DataTable FamilyRelationTable = new DataTable();

                    string EmployeeSql = "";
                    string RelationSql = "";
                    string FamilySql = "";




                    if (xType == "EMP")
                    {
                        RelationTable = _api.Format(RelationTable, "RelationTable");

                        EmployeeSql = "select * from vw_MedicalInsuranceAdditionEmp where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID  and X_EmpCode ='" + xEmployeeCode + "'";
                        EmployeeTable = dLayer.ExecuteDataTable(EmployeeSql, EmpParams, connection);
                        if (EmployeeTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }

                        EmployeeTable = myFunctions.AddNewColumnToDataTable(EmployeeTable, "N_Price", typeof(double), 0);
                        EmployeeTable = myFunctions.AddNewColumnToDataTable(EmployeeTable, "N_Cost", typeof(double), 0);
                        EmployeeTable = myFunctions.AddNewColumnToDataTable(EmployeeTable, "D_LastDate", typeof(DateTime), null);
                        EmployeeTable = myFunctions.AddNewColumnToDataTable(EmployeeTable, "PolicyDays", typeof(int), 0);

                        foreach (DataRow dvar in EmployeeTable.Rows)
                        {
                            object InsAmt = dLayer.ExecuteScalar("Select N_Price From vw_InsuranceAmountCategoryWise Where N_CompanyID =@nCompanyID  and N_InsuranceID=" + N_MedicalInsID + " and X_InsuranceClass='" + dvar["X_InsuranceClassEmp"] + "' and EmpType=171 and N_InsuranceSettingsDetailsID=" + myFunctions.getIntVAL(dvar["N_EmpInsClassID"].ToString()) + "", EmpParams, connection);
                            object InsCost = dLayer.ExecuteScalar("Select N_Cost From vw_InsuranceAmountCategoryWise Where N_CompanyID =@nCompanyID  and N_InsuranceID=" + N_MedicalInsID + " and X_InsuranceClass='" + dvar["X_InsuranceClassEmp"] + "' and EmpType=171 and N_InsuranceSettingsDetailsID=" + myFunctions.getIntVAL(dvar["N_EmpInsClassID"].ToString()) + "", EmpParams, connection);
                            object date = dLayer.ExecuteScalar("select D_EndDate from Pay_Medical_Insurance where N_MedicalInsID = " + N_MedicalInsID + " and N_CompanyID =@nCompanyID ", Params, connection);
                            object policyDays = dLayer.ExecuteScalar("SELECT  isnull(DATEDIFF(DAY,D_StartDate, D_EndDate),0) AS days from Pay_Medical_Insurance where N_MedicalInsID =" + N_MedicalInsID + " and  N_CompanyID=@nCompanyID", Params, connection);
                            if (InsAmt != null)
                            {
                                dvar["N_Price"] = myFunctions.getVAL(InsAmt.ToString());
                            }
                            if (InsCost != null)
                            {
                                dvar["N_Cost"] = myFunctions.getVAL(InsCost.ToString());
                            }
                            if (date != null)
                            {
                                dvar["D_LastDate"] = myFunctions.getDateVAL(Convert.ToDateTime(date));
                            }
                            if (policyDays != null)
                            {
                                dvar["PolicyDays"] = myFunctions.getVAL(policyDays.ToString());
                            }

                        }
                        EmployeeTable.AcceptChanges();
                        EmployeeTable = _api.Format(EmployeeTable, "EmpTable");


                        //FamilyDetails
                        FamilySql = "select * from vw_EmployeeDependenceDetails where N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID  and N_EmpID =" + nEmpID + "";
                        FamilyTable = dLayer.ExecuteDataTable(FamilySql, EmpParams, connection);

                        FamilyTable = myFunctions.AddNewColumnToDataTable(FamilyTable, "N_Price", typeof(double), 0);
                        FamilyTable = myFunctions.AddNewColumnToDataTable(FamilyTable, "N_Cost", typeof(double), 0);
                        FamilyTable = myFunctions.AddNewColumnToDataTable(FamilyTable, "D_LastDate", typeof(DateTime), null);
                        FamilyTable = myFunctions.AddNewColumnToDataTable(FamilyTable, "PolicyDays", typeof(int), 0);


                        foreach (DataRow var in FamilyTable.Rows)
                        {
                            object InsAmt = dLayer.ExecuteScalar("Select N_Price From vw_InsuranceAmountCategoryWise Where N_CompanyID =@nCompanyID and N_InsuranceID=" + N_MedicalInsID + " and X_InsuranceClass='" + var["X_InsuranceClassDep"] + "' and EmpType=172 and N_InsuranceSettingsDetailsID=" + myFunctions.getIntVAL(var["N_DepInsClassID"].ToString()) + "", EmpParams, connection);
                            object InsCost = dLayer.ExecuteScalar("Select N_Cost From vw_InsuranceAmountCategoryWise Where N_CompanyID = @nCompanyID and N_InsuranceID=" + N_MedicalInsID + " and X_InsuranceClass='" + var["X_InsuranceClassDep"] + "' and EmpType=172and N_InsuranceSettingsDetailsID=" + myFunctions.getIntVAL(var["N_DepInsClassID"].ToString()) + "", EmpParams, connection);
                            object date = dLayer.ExecuteScalar("select D_EndDate from Pay_Medical_Insurance where N_MedicalInsID = " + N_MedicalInsID + " and N_CompanyID =@nCompanyID ", Params, connection);
                            object policyDays = dLayer.ExecuteScalar("SELECT  isnull(DATEDIFF(DAY,D_StartDate, D_EndDate),0) AS days from Pay_Medical_Insurance where N_MedicalInsID =" + N_MedicalInsID + " and  N_CompanyID=@nCompanyID", Params, connection);

                            if (InsAmt != null)
                            {
                                var["N_Price"] = myFunctions.getVAL(InsAmt.ToString());
                            }
                            if (InsCost != null)
                            {
                                var["N_Cost"] = myFunctions.getVAL(InsCost.ToString());
                            }
                            if (date != null)
                            {
                                var["D_LastDate"] = myFunctions.getDateVAL(Convert.ToDateTime(date));
                            }
                            if (policyDays != null)
                            {
                                var["PolicyDays"] = myFunctions.getVAL(policyDays.ToString());
                            }


                        }
                        FamilyTable.AcceptChanges();
                        FamilyTable = _api.Format(FamilyTable, "FamilyTable");
                    }
                    else if (xType == "DEP")
                    {
                        FamilyTable = _api.Format(FamilyTable, "FamilyTable");
                        EmployeeTable = _api.Format(EmployeeTable, "EmpTable");


                        RelationSql = "select * from vw_EmployeeDependenceDetails where N_CompanyID=@nCompanyID and N_EmpID =" + nEmpID + " and  N_DependenceID=" + xDepId + " and N_FnYearId=" + nFnYearID + "";
                        RelationTable = dLayer.ExecuteDataTable(RelationSql, DepParams, connection);
                        RelationTable = myFunctions.AddNewColumnToDataTable(RelationTable, "N_Price", typeof(double), 0);
                        RelationTable = myFunctions.AddNewColumnToDataTable(RelationTable, "N_Cost", typeof(double), 0);
                        RelationTable = myFunctions.AddNewColumnToDataTable(RelationTable, "D_LastDate", typeof(DateTime), null);
                        RelationTable = myFunctions.AddNewColumnToDataTable(RelationTable, "PolicyDays", typeof(int), 0);


                        if (RelationTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        foreach (DataRow kvar in RelationTable.Rows)
                        {
                            object InsAmt = dLayer.ExecuteScalar("Select N_Price From vw_InsuranceAmountCategoryWise Where N_CompanyID =@nCompanyID and N_InsuranceID=" + N_MedicalInsID + " and X_InsuranceClass='" + kvar["X_InsuranceClassDep"] + "' and EmpType=172 and N_InsuranceSettingsDetailsID=" + myFunctions.getIntVAL(kvar["N_DepInsClassID"].ToString()) + "", EmpParams, connection);
                            object InsCost = dLayer.ExecuteScalar("Select N_Cost From vw_InsuranceAmountCategoryWise Where N_CompanyID = @nCompanyID and N_InsuranceID=" + N_MedicalInsID + " and X_InsuranceClass='" + kvar["X_InsuranceClassDep"] + "' and EmpType=172and N_InsuranceSettingsDetailsID=" + myFunctions.getIntVAL(kvar["N_DepInsClassID"].ToString()) + "", EmpParams, connection);
                            object date = dLayer.ExecuteScalar("select D_EndDate from Pay_Medical_Insurance where N_MedicalInsID = " + N_MedicalInsID + " and N_CompanyID =@nCompanyID ", Params, connection);
                            object policyDays = dLayer.ExecuteScalar("SELECT  isnull(DATEDIFF(DAY,D_StartDate, D_EndDate),0) AS days from Pay_Medical_Insurance where N_MedicalInsID =" + N_MedicalInsID + " and  N_CompanyID=@nCompanyID", Params, connection);

                            if (InsAmt != null)
                            {
                                kvar["N_Price"] = myFunctions.getVAL(InsAmt.ToString());
                            }
                            if (InsCost != null)
                            {
                                kvar["N_Cost"] = myFunctions.getVAL(InsCost.ToString());
                            }
                            if (date != null)
                            {
                                kvar["D_LastDate"] = myFunctions.getDateVAL(Convert.ToDateTime(date));
                            }
                            if (policyDays != null)
                            {
                                kvar["PolicyDays"] = myFunctions.getVAL(policyDays.ToString());
                            }



                        }
                        RelationTable.AcceptChanges();
                        RelationTable = _api.Format(RelationTable, "RelationTable");


                    }

                    dt.Tables.Add(EmployeeTable);
                    dt.Tables.Add(FamilyTable);
                    dt.Tables.Add(RelationTable);



                    return Ok(_api.Success(dt));
                }
            }


            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }

        }
        [HttpGet("fillDataVendor")]
        public ActionResult GetfillDataVendor(int nCompanyID, string xProjectName, int nVendorID, DateTime dDate)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    SortedList EmpParams = new SortedList();

                    Params.Add("@nCompanyID", nCompanyID);

                    Params.Add("@xProjectName", xProjectName);
                    Params.Add("@nVendorID", nVendorID);
                    EmpParams.Add("@nCompanyID", nCompanyID);
                    String FillDataSql = "";
                    DataTable FillVendorTable = new DataTable();
                    //var Date =dDate.ToShortDateString();
                    String X_Date = dDate.Year + "" + dDate.Month.ToString().PadLeft(2, '0') + "" + dDate.Day.ToString().PadLeft(2, '0');
                    int N_Date = Convert.ToInt32(X_Date);
                    if (xProjectName == null || xProjectName == "")
                    {
                        FillDataSql = "Select * From vw_MedicalInsuranceAddition Where N_VendorID=" + nVendorID + " and N_CEndDate<" + N_Date + "";
                        FillVendorTable = dLayer.ExecuteDataTable(FillDataSql, EmpParams, connection);
                        if (FillVendorTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }

                    }
                    else
                    {

                        FillDataSql = "Select * From vw_MedicalInsuranceAddition Where  N_VendorID=" + nVendorID + " and N_CEndDate<" + N_Date + " and X_ProjectName='" + xProjectName + "' ";
                        FillVendorTable = dLayer.ExecuteDataTable(FillDataSql, EmpParams, connection);
                        if (FillVendorTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }

                    }
                    dt.Tables.Add(FillVendorTable);
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

