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
    [Route("department")]
    [ApiController]
    public class Pay_Department : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID;

        public Pay_Department(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 202;//form id of cost center
        }

        [HttpGet("list")]
        public ActionResult GetDepartmentList(int nFnYearID, string division, string allowTransaction, bool isDepartment)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            string sqlCommandText = "";
            string viewName, tableName = "";
            string tablcode = "";
            if (isDepartment)
            {
                tableName = "Pay_Department";
                tablcode = "X_DepartmentCode";
                viewName = "vw_PayDepartmentMaster_Disp";
            }
            else
            {
                tableName = "Acc_CostCentreMaster";
                viewName = "vw_AccCostCentreMaster_Disp";
                tablcode = "X_CostCentreCode";
            }
            if (division == "" || division == null)
            {

                sqlCommandText = "Select *  from " + tableName + " Where N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + "  Order By " + tablcode + "";
            }
            else
            {

                sqlCommandText = "Select *  from " + tableName + " Where N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + " and isnull(N_GroupID,0)=0   Order By " + tablcode + "";
            }
            if (allowTransaction != "" && allowTransaction != null)

            {
                sqlCommandText = "Select [Cost Centre Name] as X_CostCentreName,*  from " + viewName + " Where N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + " and B_AllowTransactionPosting=1  Order By X_CostCentreCode";
            }

            // if (nDivisionID > 0)
            // {
            //     Params.Add("@nDivisionID", nDivisionID);
            //     sqlCommandText = "Select N_CompanyID,N_DepartmentID,N_DivisionID,Code,Description from vw_PayDepartment_Disp Where N_CompanyID=@nCompanyID and N_DivisionID=@nDivisionID order by Code";
            // }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(_api.Error(User, e));
            }
        }
        private void GetNextLevelPattern(int higerlevelid, ref int N_LevelPattern, ref string X_LevelPattern, SortedList QueryParams, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                object ObjChildN_LevelPattern = null;
                object ObjChildX_LevelPattern = null;
                object ObjRowCount = null;
                object ObjChildCount = null;

                ObjRowCount = dLayer.ExecuteScalar("Select count(1) FROM Acc_CostCentreMaster Where N_CompanyID= @nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection, transaction);
                if (higerlevelid > 0)
                    ObjChildCount = dLayer.ExecuteScalar("Select count(1) FROM Acc_CostCentreMaster Where N_GroupID=" + higerlevelid + " and N_CompanyID= @nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection, transaction);
                else
                    ObjChildCount = 0;
                if (ObjRowCount != null)
                {
                    if (myFunctions.getIntVAL(ObjRowCount.ToString()) > 0)
                    {

                        //parent
                        if (higerlevelid == 0)
                        {
                            N_LevelPattern = 0;
                            object ObjLevelPattern = dLayer.ExecuteScalar("SELECT MAX(X_LevelPattern)+1 FROM Acc_CostCentreMaster Where N_LevelID=1 and N_CompanyID= @nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection, transaction);
                            if (ObjLevelPattern != null)
                                X_LevelPattern = ObjLevelPattern.ToString();
                        }

                        //child                        
                        else if (higerlevelid > 0)
                        {
                            if (ObjChildCount != null)
                            {
                                if (myFunctions.getIntVAL(ObjChildCount.ToString()) > 0)
                                {
                                    ObjChildN_LevelPattern = dLayer.ExecuteScalar("SELECT MAX(X_LevelPattern) FROM Acc_CostCentreMaster where N_CompanyID= @nCompanyID and N_FnYearID=@nFnYearID and N_GroupID=" + higerlevelid + " ", QueryParams, connection, transaction);
                                    string lastThreePattern = ObjChildN_LevelPattern.ToString().Substring(ObjChildN_LevelPattern.ToString().Length - 3);
                                    lastThreePattern = (myFunctions.getIntVAL(lastThreePattern.ToString()) + 1).ToString();
                                    N_LevelPattern = myFunctions.getIntVAL(lastThreePattern.ToString());
                                }
                                else
                                {
                                    object objN_level = dLayer.ExecuteScalar("SELECT X_LevelPattern FROM Acc_CostCentreMaster Where N_CostCentreID=" + higerlevelid + " and N_CompanyID= @nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection, transaction);
                                    if (myFunctions.getIntVAL(objN_level.ToString()) != 0)
                                    {
                                        string addingPattern = "101";
                                        ObjChildN_LevelPattern = objN_level.ToString() + addingPattern;
                                    }
                                    else
                                        ObjChildN_LevelPattern = dLayer.ExecuteScalar("SELECT X_LevelPattern FROM Acc_CostCentreMaster Where N_CostCentreID=" + higerlevelid + " and N_CompanyID= @nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection, transaction);
                                    if (ObjChildN_LevelPattern != null)
                                    {
                                        string lastThreePattern2 = ObjChildN_LevelPattern.ToString().Substring(ObjChildN_LevelPattern.ToString().Length - 3);
                                        N_LevelPattern = myFunctions.getIntVAL(lastThreePattern2.ToString());
                                    }

                                }
                            }

                            ObjChildX_LevelPattern = dLayer.ExecuteScalar("SELECT X_LevelPattern FROM Acc_CostCentreMaster Where N_CostCentreID=" + higerlevelid + " and N_CompanyID= @nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection, transaction);
                            // if (ObjChildX_LevelPattern != null && myFunctions.getIntVAL(ObjChildCount.ToString()) > 0 )
                            // {
                            //     X_LevelPattern= N_LevelPattern.ToString();
                            // }
                            // else 
                            if (ObjChildX_LevelPattern != null)
                                X_LevelPattern = ObjChildX_LevelPattern.ToString() + N_LevelPattern.ToString();
                        }
                    }
                    else
                    {
                        N_LevelPattern = 0;
                        X_LevelPattern = "101";
                    }
                }
            }
            catch (Exception ex)
            {
                //return Ok(_api.Error(User,ex.Message));
            }

        }




        [HttpGet("chart")]
        public ActionResult GetDepartmentChart(int nFnYearID, bool isDepartment)

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);
            string sqlCommandText = "";
            if (isDepartment)
            {
                sqlCommandText = "Select *  from vw_PayDepartmentMaster Where N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + "  Order By X_CostCentreCode";

            }
            else
            {
                sqlCommandText = "Select *  from vw_CostCentreMaster Where N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + " Order By X_CostCentreCode";

            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(_api.Error(User, e));
            }
        }


        [HttpGet("evaluationList")]
        public ActionResult GetEvaluationList(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);

            string sqlCommandText = "Select *  from Pay_EmpEvaluationSettings Where N_CompanyID= " + nCompanyID + " and N_FnYearID=" + nFnYearID + "";


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("divisionList")]
        public ActionResult GetDivisionList(int nFnYearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearID", nFnYearID);

            string sqlCommandText = "Select N_CostCentreID as N_DivisionID,X_CostCentreName as X_DivisionName,*  from Acc_CostCentreMaster Where N_CompanyID= " + nCompanyID + " and  N_GroupID=0 and N_FnYearID=" + nFnYearID + "";


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return Ok(_api.Error(User, e));
            }
        }

        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    SortedList QueryParams = new SortedList();
                    string X_CostCentreCode = MasterTable.Rows[0]["x_CostCentreCode"].ToString();
                    string X_CostcentreName = MasterTable.Rows[0]["x_CostcentreName"].ToString();
                    string N_CompanyID = MasterTable.Rows[0]["n_CompanyId"].ToString();
                    string N_FnYearID = MasterTable.Rows[0]["n_FnYearId"].ToString();
                    int N_CostCentreID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CostCentreID"].ToString());
                    int N_GroupID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_GroupID"].ToString());
                    int N_LevelID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_LevelID"].ToString());
                    int N_LevelPattern = myFunctions.getIntVAL(MasterTable.Rows[0]["n_LevelPattern"].ToString());
                    string X_LevelPattern = "";
                    bool isDepartment = false;
                    if (MasterTable.Columns.Contains("x_LevelPattern"))
                        X_LevelPattern = MasterTable.Rows[0]["x_LevelPattern"].ToString();
                    // MasterTable.Rows[0]["n_ManagerID"]=myFunctions.getIntVAL(MasterTable.Rows[0]["n_Empid"].ToString());  // commented by rks
                    if (!MasterTable.Columns.Contains("x_LevelPattern"))
                    {
                        myFunctions.AddNewColumnToDataTable(MasterTable, "x_LevelPattern", typeof(string), "");
                    }
                    if (MasterTable.Columns.Contains("isDepartment"))
                    {
                        isDepartment = myFunctions.getBoolVAL(MasterTable.Rows[0]["isDepartment"].ToString());
                        MasterTable.Columns.Remove("isDepartment");
                    }
                    QueryParams.Add("@nCompanyID", N_CompanyID);
                    QueryParams.Add("@nFnYearID", N_FnYearID);
                    QueryParams.Add("@nCostCentreID", N_CostCentreID);
                    QueryParams.Add("@nGroupID", N_GroupID);
                    QueryParams.Add("@nLevelID", N_LevelID);

                    string tableName = "";
                    string tablcode, tablid = "";
                    if (isDepartment)
                    {
                        tableName = "Pay_Department";
                        tablid = "N_DepartmentID";
                        tablcode = "X_DepartmentCode";

                    }
                    else
                    {
                        tableName = "Acc_CostCentreMaster";
                        tablid = "N_CostCentreID";
                        tablcode = "X_CostCentreCode";
                    }

                    if (N_CostCentreID == 0)
                    {
                        if (N_GroupID == 0)
                            N_LevelID = 1;
                        else
                        {
                            object ObjNextLevelID = dLayer.ExecuteScalar("select isnull(N_LevelID,0)+1 from " + tableName + " where " + tablid + "=" + N_GroupID + " and N_FnYearID=@nFnYearID and N_CompanyID= @nCompanyID ", QueryParams, connection, transaction);
                            if (ObjNextLevelID == null)
                                N_LevelID = 0;
                            else
                                N_LevelID = myFunctions.getIntVAL(ObjNextLevelID.ToString());
                        }
                        GetNextLevelPattern(N_GroupID, ref N_LevelPattern, ref X_LevelPattern, QueryParams, connection, transaction);
                        MasterTable.Rows[0]["N_LevelID"] = N_LevelID;
                        MasterTable.Rows[0]["x_LevelPattern"] = X_LevelPattern;

                    }

                    if (X_CostCentreCode == "@Auto" && N_CostCentreID == 0)
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_YearID", N_FnYearID);
                        Params.Add("N_FormID", this.N_FormID);
                        //Params.Add("N_BranchID", MasterTable.Rows[0]["n_BranchId"].ToString());
                        X_CostCentreCode = dLayer.GetAutoNumber("" + tableName + "", "" + tablcode + "", Params, connection, transaction);
                        if (X_CostCentreCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate Department/Cost Centre Code")); }
                        MasterTable.Rows[0]["x_CostCentreCode"] = X_CostCentreCode;


                    }
                    else if (X_CostCentreCode == "@Auto" && N_CostCentreID > 0)
                    {

                        X_CostCentreCode = GetNextChildCode(N_CostCentreID, QueryParams, connection, transaction);
                        MasterTable.Rows[0]["x_CostCentreCode"] = X_CostCentreCode;
                        MasterTable.Rows[0]["N_CostCentreID"] = 0;
                        dLayer.DeleteData("" + tableName + "", "" + tablid + "", N_CostCentreID, "N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection, transaction);

                    }
                    // else
                    // {
                    //     dLayer.DeleteData(""+tableName+"", ""+tablid+"",N_CostCentreID, "N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID + "", connection, transaction);
                    // }
                    if (MasterTable.Columns.Contains("n_empid"))
                        MasterTable.Columns.Remove("n_empid");

                    if (MasterTable.Columns.Contains("B_AllowTransactionPosting"))
                        MasterTable.Rows[0]["B_AllowTransactionPosting"] = 1;
                    else
                    {
                        myFunctions.AddNewColumnToDataTable(MasterTable, "B_AllowTransactionPosting", typeof(bool), 1);
                    }


                    if (isDepartment)
                    {
                        if (MasterTable.Columns.Contains("N_CostCentreID"))
                        {
                            myFunctions.AddNewColumnToDataTable(MasterTable, "N_DepartmentID", typeof(int), N_CostCentreID);

                        }
                        if (MasterTable.Columns.Contains("X_CostcentreName"))
                        {
                            myFunctions.AddNewColumnToDataTable(MasterTable, "X_Department", typeof(string), X_CostcentreName);
                            myFunctions.AddNewColumnToDataTable(MasterTable, "X_DepartmentLocale", typeof(string), X_CostcentreName);
                            MasterTable.Columns.Remove("X_CostcentreName");

                        }
                        if (MasterTable.Columns.Contains("x_CostCentreCode"))
                        {
                            myFunctions.AddNewColumnToDataTable(MasterTable, "X_DepartmentCode", typeof(string), MasterTable.Rows[0]["x_CostCentreCode"].ToString());
                            MasterTable.Columns.Remove("x_CostCentreCode");
                        }
                        MasterTable.AcceptChanges();


                    }

                    if (isDepartment)
                    {
                        // string DupCriteria = "X_Department='" + X_CostcentreName.Replace("'", "''") + "' and N_CompanyID=" + N_CompanyID + "and  X_DepartmentCode='" + X_CostCentreCode + "'";
                        string DupCriteria = "X_Department='" + X_CostcentreName.Replace("'", "''") + "' and N_CompanyID=" + N_CompanyID;

                        string X_Criteria = "N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID;

                        N_CostCentreID = dLayer.SaveData("" + tableName + "", "" + tablid + "", DupCriteria, X_Criteria, MasterTable, connection, transaction);

                    }
                    else
                    {
                        N_CostCentreID = dLayer.SaveData("" + tableName + "", "" + tablid + "", MasterTable, connection, transaction);
                    }

                    if (N_CostCentreID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save.The DepartMent/Group Already Exist"));
                    }
                    else
                    {
                        if (isDepartment)
                        {
                            dLayer.ExecuteNonQuery("update Pay_Department set N_CostCentreID=" + N_CostCentreID + " where N_DepartmentID=" + N_CostCentreID + " and N_CompanyID=" + N_CompanyID + " and N_FnYearID=" + N_FnYearID, Params, connection, transaction);

                        }

                        transaction.Commit();
                    }

                    return Ok(_api.Success("Entry Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }



        [HttpDelete("delete")]
        public ActionResult DeleteData(int nDepartmentID, int nFnYearID, bool isDepartment)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nFnYearID", nFnYearID);
                QueryParams.Add("@nCostCentreID", nDepartmentID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (isDepartment)
                    {
                        object ObjDepcount = dLayer.ExecuteScalar("Select count(1) From vw_Pay_Department_List where N_CostCentreID=@nCostCentreID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection);
                        if (ObjDepcount != null)
                        {
                            if (myFunctions.getIntVAL(ObjDepcount.ToString()) <= 0)
                            {
                                Results = dLayer.DeleteData("pay_Department", "N_DepartmentID", nDepartmentID, "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection);
                            }
                            else
                            {
                                return Ok(_api.Error(User, "Department Allready in use"));
                            }
                        }
                    }
                    else
                    {
                        object Objcount = dLayer.ExecuteScalar("Select count(1) From vw_Acc_CostCentreMaster_List where N_CostCentreID=@nCostCentreID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection);
                        object count = dLayer.ExecuteScalar("Select count(1) From Acc_VoucherMaster_Details_Segments where N_Segment_2=@nCostCentreID and N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearID", QueryParams, connection);
                        if (Objcount != null)
                        {
                            if (myFunctions.getIntVAL(Objcount.ToString()) <= 0 || myFunctions.getIntVAL(count.ToString()) <= 0)
                            {
                                Results = dLayer.DeleteData("Acc_CostCentreMaster", "N_CostCentreID", nDepartmentID, "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + "", connection);
                            }
                            else
                            {
                                return Ok(_api.Error(User, "Department/Costcenter Allready in use"));
                            }
                        }

                    }

                }
                if (Results > 0)
                {
                    return Ok(_api.Success("Entry deleted"));
                }
                else
                {
                    return Ok(_api.Error(User, "Unable to delete"));
                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }

        }


        [HttpGet("dummy")]
        public ActionResult GetDepartmentDummy(int? nDepartmentID)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(connectionString))
                {
                    Con.Open();
                    string sqlCommandText = "select * from Acc_CostCentreMaster where N_CostCentreID=@p1";
                    SortedList mParamList = new SortedList() { { "@p1", nDepartmentID } };
                    DataTable masterTable = dLayer.ExecuteDataTable(sqlCommandText, mParamList, Con);
                    masterTable = _api.Format(masterTable, "master");

                    if (masterTable.Rows.Count == 0) { return Ok(new { }); }
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add(masterTable);
                    return Ok(dataSet);

                }
            }
            catch (Exception e)
            {
                return StatusCode(403, _api.Error(User, e));
            }
        }

        private string GetNextChildCode(int nParentID, SortedList ParamList, SqlConnection connection, SqlTransaction transaction)
        {
            // string[] NodeValues = CurrentNode.Tag.ToString().Split('|');
            int ParentID = nParentID;
            string DeptCode = "";
            DataTable dtDept = new DataTable();
            dtDept = dLayer.ExecuteDataTable("select COUNT(convert(nvarchar(100),X_CostCentreCode)) From Acc_CostCentreMaster where N_GroupID =" + ParentID + " and N_CompanyID =@nCompanyID and N_FnYearID=@nFnYearID", ParamList, connection, transaction);
            int count = myFunctions.getIntVAL(dtDept.Rows[0][0].ToString());
            dtDept = dLayer.ExecuteDataTable("select X_CostCentreCode From Acc_CostCentreMaster where N_CostCentreID =" + ParentID + " and N_CompanyID =@nCompanyID and N_FnYearID=@nFnYearID", ParamList, connection, transaction);
            while (true)
            {
                count += 1;
                DeptCode = dtDept.Rows[0][0].ToString() + count.ToString("100");
                object N_Result = dLayer.ExecuteScalar("Select 1 from Acc_CostCentreMaster Where X_CostCentreCode ='" + DeptCode.Trim() + "' and N_CompanyID= @nCompanyID and N_FnYearID=@nFnYearID", ParamList, connection, transaction);
                if (N_Result == null)
                    break;
            }
            return DeptCode;
        }
    }
}
