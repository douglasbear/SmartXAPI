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
using System.Security.Claims;


namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("dataupload")]
    [ApiController]
    public class Gen_DataUpload : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        public Gen_DataUpload(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        //Save....


       [HttpPost("uploadscreendata")]
        public ActionResult uploadscreendata([FromBody] DataSet ds)
        {

            try
            {
                DataTable MasterTable;
                DataTable DetailTable;
                DataTable InfoTable;
                DataTable CostCenterTable;
                MasterTable = ds.Tables["master"];
                DetailTable = ds.Tables["details"];
                CostCenterTable = ds.Tables["CostCenterTable"];
                InfoTable = ds.Tables["info"];
                SortedList Params = new SortedList();

                DataTable Approvals;
                Approvals = ds.Tables["approval"];
                DataRow ApprovalRow = Approvals.Rows[0];

                DataRow masterRow = MasterTable.Rows[0];
                var xVoucherNo = masterRow["x_VoucherNo"].ToString();
                var xTransType = masterRow["x_TransType"].ToString();
                var InvoiceNo = masterRow["x_TransType"].ToString();
                var nCompanyId = masterRow["n_CompanyId"].ToString();
                var nFnYearId = masterRow["n_FnYearId"].ToString();
                int N_VoucherID = myFunctions.getIntVAL(masterRow["n_VoucherID"].ToString());
                var nUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var nFormID = 0;
                int N_NextApproverID=0;
                int nCompanyID = myFunctions.GetCompanyID(User);
                string xTableName = "";
                DataTable Mastertable = new DataTable();
                DataTable Generaltable = new DataTable();
                Mastertable = ds.Tables[dt.TableName];
                int N_SaveDraft = 0;// myFunctions.getIntVAL(masterRow["b_IssaveDraft"].ToString());
                // int N_SaveDraft = 0;
                // if(saveDraft) N_SaveDraft=1;



                if (xTransType.ToLower() == "pv")
                    nFormID = 44;
                else if (xTransType.ToLower() == "rv")
                    nFormID = 45;
                else if (xTransType.ToLower() == "jv")
                    nFormID = 46;

                var xAction = "INSERT";
                if (N_VoucherID > 0)
                {
                    xAction = "UPDATE";
                }

                string ipAddress = "";
                if (Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    
                //  if (!myFunctions.CheckActiveYearTransaction(myFunctions.getIntVAL(nCompanyId.ToString()),myFunctions.getIntVAL(nFnYearId.ToString()), DateTime.ParseExact(MasterTable.Rows[0]["D_VoucherDate"].ToString(), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture), dLayer, connection, transaction))
                //     {
                //         object DiffFnYearID = dLayer.ExecuteScalar("select N_FnYearID from Acc_FnYear where N_CompanyID="+nCompanyId+" and convert(date ,'" + MasterTable.Rows[0]["D_VoucherDate"].ToString() + "') between D_Start and D_End", connection, transaction);
                //         if (DiffFnYearID != null)
                //         {
                //             MasterTable.Rows[0]["n_FnYearID"] = DiffFnYearID.ToString();
                //             nFnYearId = DiffFnYearID.ToString();
                //            // QueryParams["@nFnYearID"] = N_FnYearID;
                //         }
                //         else
                //         {
                //             transaction.Rollback();
                //             return Ok(api.Error(User, "Transaction date must be in the active Financial Year."));
                //         }
                //     }
                    if (!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString()) && N_VoucherID > 0)
                    {
                        int N_PkeyID = N_VoucherID;
                        string X_Criteria = "N_VoucherID=" + N_VoucherID + " and N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId;
                        myFunctions.UpdateApproverEntry(Approvals, "Acc_VoucherMaster", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        N_NextApproverID = myFunctions.LogApprovals(Approvals,myFunctions.getIntVAL(nFnYearId.ToString()), xTransType, N_PkeyID, xVoucherNo, 1, "", 0, "", User, dLayer, connection, transaction);
                        //myAttachments.SaveAttachment(dLayer, Attachment, InvoiceNo, N_SalesID, objCustName.ToString().Trim(), objCustCode.ToString(), N_CustomerID, "Customer Document", User, connection, transaction);

                        N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Acc_VoucherMaster where N_VoucherID=" + N_VoucherID + " and N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId, connection, transaction).ToString());
                        if (N_SaveDraft == 0)
                        {
                            SortedList PostingParams = new SortedList();
                            PostingParams.Add("N_CompanyID", nCompanyId);
                            PostingParams.Add("X_InventoryMode", xTransType);
                            PostingParams.Add("N_InternalID", N_VoucherID);
                            PostingParams.Add("N_UserID", nUserId);
                            PostingParams.Add("X_SystemName", "ERP Cloud");
                            // object posting = dLayer.ExecuteScalarPro("SP_Acc_InventoryPosting", PostingParams, connection, transaction);
                        }

                        //myFunctions.SendApprovalMail(N_NextApproverID, nFormID, N_PkeyID, xTransType, xVoucherNo, dLayer, connection, transaction, User);
                        transaction.Commit();
                        return Ok(_api.Success("Voucher Approved " + "-" + xVoucherNo));
                    }
                    
                    if (xVoucherNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyId);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", nFormID);
                        // Params.Add("N_BranchID", masterRow["n_BranchId"].ToString());

                        while (true)
                        {


                            xVoucherNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Acc_VoucherMaster Where X_VoucherNo ='" + xVoucherNo + "' and N_CompanyID= " + nCompanyId + " and X_TransType ='" + xTransType + "' and N_FnYearID =" + nFnYearId, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        if (xVoucherNo == "")
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable to generate Invoice Number"));
                        }
                        // xVoucherNo = dLayer.GetAutoNumber("Acc_VoucherMaster", "x_VoucherNo", Params, connection, transaction);
                        // if (xVoucherNo == "") { return Ok(api.Error(User,"Unable to generate Invoice Number")); }

                        MasterTable.Rows[0]["x_VoucherNo"] = xVoucherNo;
                    }
                    else
                    {
                        if (N_VoucherID > 0)
                        {
                            int dltRes = dLayer.DeleteData("Acc_VoucherDetails", "N_InventoryID", N_VoucherID, "x_transtype='" + xTransType + "' and x_voucherno ='" + xVoucherNo + "' and N_CompanyID =" + nCompanyId + " and N_FnYearID =" + nFnYearId, connection, transaction);
                            if (dltRes <= 0){transaction.Rollback();return Ok(_api.Error(User,"Unable to Update"));}
                            dltRes = dLayer.DeleteData("Acc_VoucherMaster_Details_Segments", "N_VoucherID", N_VoucherID, "N_VoucherID= " + N_VoucherID + " and N_CompanyID = " + nCompanyId + " and N_FnYearID=" + nFnYearId, connection, transaction);
                            // if (dltRes <= 0){transaction.Rollback();return Ok(api.Error(User,"Unable to Update"));}
                            dltRes = dLayer.DeleteData("Acc_VoucherMaster_Details", "N_VoucherID", N_VoucherID, "N_VoucherID= " + N_VoucherID + " and N_CompanyID = " + nCompanyId, connection, transaction);
                            if (dltRes <= 0){transaction.Rollback();return Ok(_api.Error(User,"Unable to Update"));}

                        }
                    }

                    string DupCriteria = "N_CompanyID = " + nCompanyId + " and X_VoucherNo = '" + xVoucherNo + "' and N_FnYearID=" + nFnYearId + " and X_TransType = '" + xTransType + "'";

                    MasterTable.Rows[0]["n_UserID"] = myFunctions.GetUserID(User);
                    MasterTable.AcceptChanges();

                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);

                    N_VoucherID = dLayer.SaveData("Acc_VoucherMaster", "N_VoucherId", DupCriteria, "", MasterTable, connection, transaction);
                    if (N_VoucherID > 0)
                    {

                        N_NextApproverID = myFunctions.LogApprovals(Approvals,myFunctions.getIntVAL(nFnYearId.ToString()), xTransType, N_VoucherID, xVoucherNo, 1, "", 0, "", User, dLayer, connection, transaction);
                        N_SaveDraft = myFunctions.getIntVAL(dLayer.ExecuteScalar("select CAST(B_IssaveDraft as INT) from Acc_VoucherMaster where N_VoucherId=" + N_VoucherID + " and N_CompanyID=" + nCompanyId + " and N_FnYearID=" + nFnYearId, connection, transaction).ToString());

                       
                        if (dt.TableName.ToString().ToLower() == "warranty items")
                        {
                            xTableName = "Mig_WarrantyItem";
                            Mastertable.Columns.Add("N_CompanyID");
                            foreach (DataRow dtRow in Mastertable.Rows)
                            {
                                dtRow["N_CompanyID"] = nCompanyID;
                            }
                        }

                        if (Mastertable.Rows.Count > 0)
                        {

                            if (Mastertable.Columns.Contains("Notes"))
                                Mastertable.Columns.Remove("Notes");

                            dLayer.ExecuteNonQuery("delete from " + xTableName, Params, connection, transaction);


                            string FieldList = "";
                            string FieldValuesArray = "";
                            string IDFieldName = "PKey_Code";
                            int rowCount = 0;
                            int totalCount = 0;
                            for (int j = 0; j < Mastertable.Rows.Count; j++)
                            {
                                rowCount = rowCount + 1;
                                string FieldValues = "";
                                for (int k = 0; k < Mastertable.Columns.Count; k++)
                                {
                                    var values = "";
                                    if (Mastertable.Columns[k].ColumnName.ToString().ToLower() == IDFieldName.ToLower())
                                    {
                                        values = (j + 1).ToString();
                                    }
                                    else
                                    {
                                         if (Mastertable.Rows[j][k] == DBNull.Value) 
                                         {
                                        values = "";

                                         }else{
                                        values = Mastertable.Rows[j][k].ToString();
                                         }
                                        values = values.Replace("|", " ");
                                    }

                                    FieldValues = FieldValues + "|" + values;
                                    if (j == 0)
                                        FieldList = FieldList + "," + Mastertable.Columns[k].ColumnName.ToString();


                                }

                                if (j == 0)
                                {
                                    FieldList = FieldList.Substring(1);
                                }

                                FieldValues = ValidateString(FieldValues.Substring(1));

                                FieldValuesArray = FieldValuesArray + ",(" + FieldValues + ")";
                                if (rowCount == 1000 || Mastertable.Rows.Count == (totalCount + rowCount))
                                {
                                    totalCount = totalCount + rowCount;
                                    FieldValuesArray = FieldValuesArray.Substring(1);
                                    string inserStript = "insert into " + xTableName + " (" + FieldList + ") values" + FieldValuesArray;
                                    dLayer.ExecuteNonQuery(inserStript, connection, transaction); 
                                    FieldValuesArray = "";
                                    rowCount = 0;
                                }

                            }


 
                            //  nMasterID = dLayer.SaveData(xTableName, "PKey_Code", Mastertable, connection, transaction);
                            nMasterID= myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Count(1) from "+xTableName, connection, transaction).ToString());

                            object ValFlag;
                            SortedList ValidationParam = new SortedList();
                            ValidationParam.Add("N_CompanyID", nCompanyID);
                            ValidationParam.Add("N_FnYearID", myFunctions.getIntVAL(Generaltable.Rows[0]["N_FnYearID"].ToString()));
                            ValidationParam.Add("X_Type", dt.TableName);
                            try
                            {
                                dLayer.ExecuteNonQueryPro("SP_SetupData_Validation", ValidationParam, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, ex));
                            }
                            // if(ValFlag==null)return Ok(_api.Error(User, "Uploaded Error"));
                            // if(myFunctions.getIntVAL(ValFlag.ToString())==0)return Ok(_api.Error(User, "Uploaded Error"));
                            dLayer.ExecuteNonQueryPro("SP_SetupData_cloud", Params, connection, transaction);
                            if (nMasterID <= 0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, dt.TableName + " Uploaded Error"));
                            }
                            Mastertable.Clear();
                            Params.Remove("X_Type");
                            transaction.Commit();
                            return Ok(_api.Success(dt.TableName + " Uploaded"));
                        }
                    }
                    if (Mastertable.Rows.Count > 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Uploaded Completed"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Uploaded Error"));
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        public string ValidateString(string InputString)
        {
            string OutputString = InputString.Replace("'", "''");
            OutputString = OutputString.Replace("|", "','");
            OutputString = "'" + OutputString + "'";
            return OutputString;
        }

       

        
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable Mastertable = new DataTable();
                DataTable Generaltable = new DataTable();
                Generaltable = ds.Tables["general"];
                int nCompanyID = myFunctions.GetCompanyID(User);
                int nMasterID = 0;
                string xTableName = "";
                SortedList Params = new SortedList();
                Params.Add("N_CompanyID", nCompanyID);
                Params.Add("N_FnYearID", myFunctions.getIntVAL(Generaltable.Rows[0]["N_FnYearID"].ToString()));
                Params.Add("N_BranchID", myFunctions.getIntVAL(Generaltable.Rows[0]["N_BranchID"].ToString()));
                Params.Add("N_LocationID", myFunctions.getIntVAL(Generaltable.Rows[0]["N_LocationID"].ToString()));
                int N_UserID = myFunctions.GetUserID(User);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt.Columns.Contains("notes"))
                            dt.Columns.Remove("notes");
                        Params.Add("X_Type", dt.TableName);
                        Mastertable = ds.Tables[dt.TableName];
                        foreach (DataColumn col in Mastertable.Columns)
                        {
                            col.ColumnName = col.ColumnName.Replace(" ", "_");
                            col.ColumnName = col.ColumnName.Replace("*", "");
                            col.ColumnName = col.ColumnName.Replace("/", "_");
                        }
                        Mastertable.Columns.Add("Pkey_Code");

                        if (dt.TableName.ToString().ToLower() == "customer list" || dt.TableName.ToString().ToLower() == "customers" || dt.TableName.ToString().ToLower() == "customer")
                            xTableName = "Mig_Customers";
                        if (dt.TableName.ToString().ToLower() == "vendor list" || dt.TableName.ToString().ToLower() == "vendors" || dt.TableName.ToString().ToLower() == "vendor")
                            xTableName = "Mig_Vendors";
                        if (dt.TableName.ToString().ToLower() == "lead list")
                        {
                            xTableName = "Mig_Leads";
                            Mastertable.Columns.Add("N_UserID");
                            foreach (DataRow dtRow in Mastertable.Rows)
                            {
                                dtRow["N_UserID"] = N_UserID;
                            }
                        }
                        if (dt.TableName.ToString().ToLower() == "chart of accounts")
                            xTableName = "Mig_Accounts";
                        if (dt.TableName.ToString().ToLower() == "products stock")
                            xTableName = "Mig_Stock";
                        if (dt.TableName.ToString().ToLower() == "employee list" || dt.TableName.ToString().ToLower() == "employees")
                            xTableName = "Mig_Employee";
                        if (dt.TableName.ToString().ToLower() == "products stock")
                            xTableName = "Mig_Stock";
                        if (dt.TableName.ToString().ToLower() == "fixedassets list")
                            xTableName = "_Mig_AssetList";
                        if (dt.TableName.ToString().ToLower() == "salary history")
                            xTableName = "Mig_EmployeeSalaryHistory";
                        if (dt.TableName.ToString().ToLower() == "employee salary")
                            xTableName = "Mig_EmployeeSalary";
                        if (dt.TableName.ToString().ToLower() == "leave history")
                            xTableName = "Mig_EmployeeLeaveHistory";
                        if (dt.TableName.ToString().ToLower() == "customer balances")
                            xTableName = "Mig_CustomerOpening";
                        if (dt.TableName.ToString().ToLower() == "vendor balances")
                            xTableName = "Mig_VendorOpening";
                            

                        if (dt.TableName.ToString().ToLower() == "product list" || dt.TableName.ToString().ToLower() == "products")
                        {
                            xTableName = "Mig_Items";
                            Mastertable.Columns.Add("N_CompanyID");
                            foreach (DataRow dtRow in Mastertable.Rows)
                            {
                                dtRow["N_CompanyID"] = nCompanyID;
                            }
                        }


                        if (dt.TableName.ToString().ToLower() == "category")
                        {
                            xTableName = "Mig_POSCategory";
                            Mastertable.Columns.Add("N_CompanyID");
                            foreach (DataRow dtRow in Mastertable.Rows)
                            {
                                dtRow["N_CompanyID"] = nCompanyID;
                            }
                        }

                           if (dt.TableName.ToString().ToLower() == "delivery notes")
                        {
                            xTableName = "Mig_Deliverynote";
                            
                            Mastertable.Columns.Add("N_CompanyID");
                            foreach (DataRow dtRow in Mastertable.Rows)
                            {
                                dtRow["N_CompanyID"] = nCompanyID;
                            }
                        }

                        if (dt.TableName.ToString().ToLower() == "package items")
                        {
                            xTableName = "Mig_PackageItem";
                            Mastertable.Columns.Add("N_CompanyID");
                            foreach (DataRow dtRow in Mastertable.Rows)
                            {
                                dtRow["N_CompanyID"] = nCompanyID;
                            }
                        }


                        if (dt.TableName.ToString().ToLower() == "warranty items")
                        {
                            xTableName = "Mig_WarrantyItem";
                            Mastertable.Columns.Add("N_CompanyID");
                            foreach (DataRow dtRow in Mastertable.Rows)
                            {
                                dtRow["N_CompanyID"] = nCompanyID;
                            }
                        }

                        if (Mastertable.Rows.Count > 0)
                        {

                            if (Mastertable.Columns.Contains("Notes"))
                                Mastertable.Columns.Remove("Notes");

                            dLayer.ExecuteNonQuery("delete from " + xTableName, Params, connection, transaction);


                            string FieldList = "";
                            string FieldValuesArray = "";
                            string IDFieldName = "PKey_Code";
                            int rowCount = 0;
                            int totalCount = 0;
                            for (int j = 0; j < Mastertable.Rows.Count; j++)
                            {
                                rowCount = rowCount + 1;
                                string FieldValues = "";
                                for (int k = 0; k < Mastertable.Columns.Count; k++)
                                {
                                    var values = "";
                                    if (Mastertable.Columns[k].ColumnName.ToString().ToLower() == IDFieldName.ToLower())
                                    {
                                        values = (j + 1).ToString();
                                    }
                                    else
                                    {
                                         if (Mastertable.Rows[j][k] == DBNull.Value) 
                                         {
                                        values = "";

                                         }else{
                                        values = Mastertable.Rows[j][k].ToString();
                                         }
                                        values = values.Replace("|", " ");
                                    }

                                    FieldValues = FieldValues + "|" + values;
                                    if (j == 0)
                                        FieldList = FieldList + "," + Mastertable.Columns[k].ColumnName.ToString();


                                }

                                if (j == 0)
                                {
                                    FieldList = FieldList.Substring(1);
                                }

                                FieldValues = ValidateString(FieldValues.Substring(1));

                                FieldValuesArray = FieldValuesArray + ",(" + FieldValues + ")";
                                if (rowCount == 1000 || Mastertable.Rows.Count == (totalCount + rowCount))
                                {
                                    totalCount = totalCount + rowCount;
                                    FieldValuesArray = FieldValuesArray.Substring(1);
                                    string inserStript = "insert into " + xTableName + " (" + FieldList + ") values" + FieldValuesArray;
                                    dLayer.ExecuteNonQuery(inserStript, connection, transaction); 
                                    FieldValuesArray = "";
                                    rowCount = 0;
                                }

                            }


 
                            //  nMasterID = dLayer.SaveData(xTableName, "PKey_Code", Mastertable, connection, transaction);
                            nMasterID= myFunctions.getIntVAL(dLayer.ExecuteScalar("Select Count(1) from "+xTableName, connection, transaction).ToString());

                            object ValFlag;
                            SortedList ValidationParam = new SortedList();
                            ValidationParam.Add("N_CompanyID", nCompanyID);
                            ValidationParam.Add("N_FnYearID", myFunctions.getIntVAL(Generaltable.Rows[0]["N_FnYearID"].ToString()));
                            ValidationParam.Add("X_Type", dt.TableName);
                            try
                            {
                                dLayer.ExecuteNonQueryPro("SP_SetupData_Validation", ValidationParam, connection, transaction);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, ex));
                            }
                            // if(ValFlag==null)return Ok(_api.Error(User, "Uploaded Error"));
                            // if(myFunctions.getIntVAL(ValFlag.ToString())==0)return Ok(_api.Error(User, "Uploaded Error"));
                            dLayer.ExecuteNonQueryPro("SP_SetupData_cloud", Params, connection, transaction);
                            if (nMasterID <= 0)
                            {
                                transaction.Rollback();
                                return Ok(_api.Error(User, dt.TableName + " Uploaded Error"));
                            }
                            Mastertable.Clear();
                            Params.Remove("X_Type");
                            transaction.Commit();
                            return Ok(_api.Success(dt.TableName + " Uploaded"));
                        }
                    }
                    if (Mastertable.Rows.Count > 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Uploaded Completed"));
                    }
                    else
                    {
                        return Ok(_api.Error(User, "Uploaded Error"));
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

        public string ValidateString(string InputString)
        {
            string OutputString = InputString.Replace("'", "''");
            OutputString = OutputString.Replace("|", "','");
            OutputString = "'" + OutputString + "'";
            return OutputString;
        }


        [HttpGet("dataList")]
        public ActionResult GetDepartmentList(string parent)
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
                    string sqlCommandText = "select * from VW_TableCount where  N_CompanyID= " + nCompanyID + "";




                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    dt = _api.Format(dt);
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
    }
}