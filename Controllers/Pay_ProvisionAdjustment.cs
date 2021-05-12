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
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("payProvisionAdjustment")]
    [ApiController]
    public class Pay_ProvisionAdjustment : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;

        public Pay_ProvisionAdjustment(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 586;
        }
        private readonly string connectionString;


        [HttpGet("adjustmentCode")]
        public ActionResult AdjustmentCode(int nCompanyId,int nFnYearID,int nType)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText ="";

            if(nType==2)
                sqlCommandText= "select * from vw_PayMaster_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 and N_Payid not in (17,18)";
            else
                sqlCommandText= "select * from vw_PayMaster_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 and N_Payid not in (17,18) and (N_PaymentId=7 OR N_PaymentId=6)";

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    connection.Open();
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else { return Ok(_api.Success(dt)); }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("fillData")]
        public ActionResult FillData(int nCompanyId,int nFnYearID,int nPayID,int nType,string xDepartment,string xPosition,DateTime dPayMonth,int nBranchId, bool bAllBranchData)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            //int nCompanyId=myFunctions.GetCompanyID(User);
            string strcondition="";

            strcondition = " and X_Department = ''" + xDepartment + "''";
            if (xPosition != "")
            {
                if (strcondition == "")

                    strcondition = " and X_Position = ''" + xPosition+ "''";
                else
                    strcondition += " and X_Position = ''" + xPosition + "''";
            }

            string PayrunID = dPayMonth.Year.ToString("00##") + dPayMonth.Month.ToString("0#");

            string sqlCommandText ="";

            if (bAllBranchData)
                sqlCommandText = "SP_Pay_EOSAdjustment @nCompanyId," + PayrunID + ",'" + strcondition + "',0,@nPayID,@nFnYearID";
            else
                sqlCommandText = "SP_Pay_EOSAdjustment @nCompanyId," + PayrunID + ",'" + strcondition + "',@nBranchId,@nPayID,@nFnYearID";

            if (nType == 2)
            {
                if (bAllBranchData)
                    sqlCommandText = "Sp_Pay_OpeningBalance_View @nCompanyId,@nFnYearID,2,0,'" + strcondition + "'";
                else
                    sqlCommandText = "Sp_Pay_OpeningBalance_View @nCompanyId,@nFnYearID,2,@nBranchId,'" + strcondition + "'";
            }

            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@nBranchId", nBranchId);
            Params.Add("@nPayID", nPayID);


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    connection.Open();
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else { return Ok(_api.Success(dt)); }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }


        [HttpGet("actionlist")]
        public ActionResult GetActionList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from gen_defaults where n_DefaultId=33 ";
            //Params.Add("@p1", nDefaultId);
            //Params.Add("@p1", nTypeId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    connection.Open();
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else { return Ok(_api.Success(dt)); }
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
                    int nApprovalID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ApprovalID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    string X_ApprovalCode = MasterTable.Rows[0]["X_ApprovalCode"].ToString();
                    MasterTable.Columns.Remove("N_FnYearID");

                    if (nApprovalID > 0)
                    {
                         dLayer.DeleteData("Gen_ApprovalCodesDetails", "N_ApprovalID", nApprovalID, "", connection,transaction);
                         dLayer.DeleteData("Gen_ApprovalCodes", "N_ApprovalID", nApprovalID, "", connection,transaction);
                    }
                   
                    DocNo = MasterRow["X_ApprovalCode"].ToString();
                    
                    if (X_ApprovalCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("N_YearID", nFnYearID);

                        while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Gen_ApprovalCodes Where X_ApprovalCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_ApprovalCode = DocNo;


                        if (X_ApprovalCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Approval Code")); }
                        MasterTable.Rows[0]["X_ApprovalCode"] = X_ApprovalCode;

                    }
                    else
                    {
                        
                        dLayer.DeleteData("Gen_ApprovalCodes", "N_ApprovalID", nApprovalID, "", connection, transaction);
                    }


                    nApprovalID = dLayer.SaveData("Gen_ApprovalCodes", "N_ApprovalID", MasterTable, connection, transaction);
                    if (nApprovalID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    }

                    for (int i = 0; i < DetailTable.Rows.Count; i++)
                    {
                        DetailTable.Rows[i]["N_ApprovalID"] = nApprovalID;
                    }
                    int N_ApprovalDetailsID = dLayer.SaveData("Gen_ApprovalCodesDetails", "N_ApprovalDetailsID", DetailTable, connection, transaction);
                    if (N_ApprovalDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    }
                    transaction.Commit();
                    SortedList Result = new SortedList();
                    Result.Add("N_ApprovalID", nApprovalID);
                    Result.Add("N_ApprovalDetailsID", N_ApprovalDetailsID);

                    return Ok(_api.Success(Result, "Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        [HttpGet("details")]
        public ActionResult GenApprovalCode(string xApprovalCode,int nApproovalID)
        {


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DetailTable = new DataTable();
                    DataTable DataTable = new DataTable();

                    string Mastersql = "";
                    string DetailSql = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@xApprovalCode", xApprovalCode);
                    Mastersql = "select * from Gen_ApprovalCodes where N_CompanyId=@nCompanyID and X_ApprovalCode=@xApprovalCode  ";

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection);
                    if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    int ApproovalID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ApprovalID"].ToString());
                    Params.Add("@nApproovalID", ApproovalID);

                    MasterTable = _api.Format(MasterTable, "Master");
                    DetailSql = "select * from vw_ApprovalCodeDetails where N_CompanyId=@nCompanyID and N_ApprovalID=@nApproovalID ";
                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = _api.Format(DetailTable, "Details");
                    dt.Tables.Add(MasterTable);
                    dt.Tables.Add(DetailTable);
                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        [HttpGet("list")]
        public ActionResult AdjustmentList(int nCompanyID,int nFnYearID,int nType,int nBranchId,bool bAllBranchData,int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string TableName = "";
            string sqlCondition = "";
            string sqlCondition1 = "";
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";
            // int nBranchId=myCompanyID._BranchID;
            // bool bAllBranchData=myCompanyID._B_AllBranchData;

            if(nType==2)
                TableName="vw_Pay_OpeningBalance_Search";
            else
                TableName="vw_Pay_EosAdjustment_Search";

            

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (Code like '% " + xSearchkey + ")";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_AdjustmentID desc";
            else
                xSortBy = " order by " + xSortBy;

            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nFnYearId", nFnYearID);
            Params.Add("@nBranchId", nBranchId);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if(!myFunctions.CheckClosedYear(nCompanyID,nFnYearID,dLayer,connection))
                    {
                        if(bAllBranchData)
                        {
                            sqlCondition = "N_CompanyID=@nCompanyID and B_YearEndProcess=0 "+xSortBy+"";
                            sqlCondition1 = "N_CompanyID=@nCompanyID and B_YearEndProcess=0 ";
                        }
                        else
                        {
                            sqlCondition = "N_CompanyID=@nCompanyID and B_YearEndProcess=0 and N_BranchID=@nBranchId "+xSortBy+"";
                            sqlCondition1 = "N_CompanyID=@nCompanyID and B_YearEndProcess=0 and N_BranchID=@nBranchId ";
                        }
                    }
                    else
                    {
                        if(bAllBranchData)
                        {
                            sqlCondition = "N_CompanyID=@nCompanyID and  N_FnYearID=@nFnYearId "+xSortBy+"";
                            sqlCondition1 = "N_CompanyID=@nCompanyID and  N_FnYearID=@nFnYearId ";
                        }
                        else
                        {
                            sqlCondition = "N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearId and N_BranchID=@nBranchId "+xSortBy+"";
                            sqlCondition1 = "N_CompanyID=@nCompanyID and N_FnYearID=@nFnYearId and N_BranchID=@nBranchId ";
                        }
                    }

                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from "+TableName+" where "+sqlCondition+"";
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from "+TableName+" where "+sqlCondition+" and  N_AdjustmentID not in (select top(" + Count + ") N_AdjustmentID from "+TableName+"  where "+sqlCondition+" )";

                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count  from "+TableName+" where "+sqlCondition1+" ";
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

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nApprovalID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    
                    connection.Open();
                    Results = dLayer.DeleteData("Gen_ApprovalCodes", "N_ApprovalID", nApprovalID, "", connection);
                    if (Results > 0)
                    {
                    
                        dLayer.DeleteData("Gen_ApprovalCodesDetails", "N_ApprovalID", nApprovalID, "", connection);
                        return Ok(_api.Success("Approval Code deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete"));
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
