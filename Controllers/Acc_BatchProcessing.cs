
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("batchPosting")]
    [ApiController]
    public class Acc_BatchProcessing : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly IMyAttachments myAttachments;
        private readonly string connectionString;
        private readonly int N_FormID;
        public Acc_BatchProcessing(IApiFunctions api, IDataAccessLayer dl, IMyFunctions fun, IConfiguration conf, IMyAttachments myAtt)
        {
            _api = api;
            dLayer = dl;
            myFunctions = fun;
            myAttachments = myAtt;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 152;
        }

        [HttpGet("transType")]
        public ActionResult TransactionTypeList(int nCompanyId, int nFnYearID,int nBranchID,bool bAllBranchData,int FormID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID=myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nFnYearID",nFnYearID);
            Params.Add("@nBranchID",nBranchID);

            string sqlCommandText="";

            if(FormID==152)
            {
                if (bAllBranchData)
                    sqlCommandText="SELECT X_ID,X_Description,X_Description_Ar,X_TransType FROM vw_BatchPosting_Disp WHERE N_CompanyID=@nCompanyID and X_ID<>'OB' and N_FnYearID=@nFnYearID group by X_ID,X_Description,X_Description_Ar,X_TransType";
                else
                    sqlCommandText="SELECT X_ID,X_Description,X_Description_Ar,X_TransType FROM vw_BatchPosting_Disp WHERE N_CompanyID=@nCompanyID and X_ID<>'OB' and N_FnYearID=@nFnYearID and N_BranchID=@nBranchID group by X_ID,X_Description,X_Description_Ar,X_TransType";
            }
            else
            {
                if (bAllBranchData)
                    sqlCommandText="SELECT X_ID,X_Description,X_Description_Ar,X_TransType FROM vw_BatchPosting_Disp WHERE N_CompanyID=@nCompanyID And B_IsAccPosted=1 and N_FnYearID=@nFnYearID group by X_ID,X_Description,X_Description_Ar,X_TransType";
                else
                    sqlCommandText="SELECT X_ID,X_Description,X_Description_Ar,X_TransType FROM vw_BatchPosting_Disp WHERE N_CompanyID=@nCompanyID And B_IsAccPosted=1 and N_FnYearID=@nFnYearID and N_BranchID=@nBranchID group by X_ID,X_Description,X_Description_Ar,X_TransType";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params , connection);
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
                return Ok(_api.Error(User,e));
            }
        }
 
        
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            
            DataTable MasterTable;
            DataTable DetailTable;
            MasterTable = ds.Tables["master"];
            DetailTable = ds.Tables["details"];
            SortedList Params = new SortedList();

            int FormID=myFunctions.getIntVAL(MasterTable.Rows[0]["n_FormID"].ToString());
            int nFnYearID=myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
            int N_UserID=myFunctions.GetUserID(User);

            // Auto Gen
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction=connection.BeginTransaction();     

                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                    {
                        if(myFunctions.getVAL(DetailTable.Rows[j]["debit"].ToString())!=myFunctions.getVAL(DetailTable.Rows[j]["debit"].ToString()))
                            continue;

                        if(FormID==152)
                        {
                            SortedList PostingParam = new SortedList();
                            PostingParam.Add("N_CompanyID", DetailTable.Rows[j]["N_CompanyID"]);
                            PostingParam.Add("X_InventoryMode", DetailTable.Rows[j]["X_TransType"]);
                            PostingParam.Add("N_InternalID", DetailTable.Rows[j]["N_VoucherID"]);
                            PostingParam.Add("N_UserID", N_UserID);
                            PostingParam.Add("X_SystemName", System.Environment.MachineName);
                            try
                            {
                                dLayer.ExecuteNonQueryPro("SP_Acc_InventoryPosting", PostingParam, connection, transaction);
                            }  
                            catch (Exception ex)
                            { 
                                transaction.Rollback();
                                if (ex.Message == "50")
                                    return Ok(_api.Error(User,"Day Closed"));
                                else if (ex.Message == "51")
                                    return Ok(_api.Error(User,"Year Closed"));
                                else if (ex.Message == "52")
                                    return Ok(_api.Error(User,"Year Exists"));
                                else if (ex.Message == "53")
                                    return Ok(_api.Error(User,"Period Closed"));
                                else if (ex.Message == "54")
                                    return Ok(_api.Error(User,"Txn Date"));
                                else
                                    return Ok(_api.Error(User,ex));
                            } 

                            dLayer.ExecuteNonQuery("update Acc_VoucherMaster set B_IsAccPosted=1 where N_VoucherID=" + DetailTable.Rows[j]["N_VoucherID"]+" and N_CompanyID="+DetailTable.Rows[j]["N_CompanyID"], connection, transaction);                                  
                        }
                        else
                        {
                            SortedList PostingParam = new SortedList();
                            PostingParam.Add("N_CompanyID", DetailTable.Rows[j]["N_CompanyID"]);
                            PostingParam.Add("N_FnYearID", nFnYearID);
                            PostingParam.Add("X_TransType", DetailTable.Rows[j]["X_TransType"]);
                            PostingParam.Add("X_ReferenceNo", DetailTable.Rows[j]["X_ReferenceNo"]);
                            PostingParam.Add("N_VoucherID", DetailTable.Rows[j]["N_VoucherID"]);
                            PostingParam.Add("N_UserID", N_UserID);
                            PostingParam.Add("X_SystemName", System.Environment.MachineName);
                            try
                            {
                                dLayer.ExecuteNonQueryPro("SP_AccCancelPosting", PostingParam, connection, transaction);
                            } 
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                 if (ex.Message == "50")
                                    return Ok(_api.Error(User,"Day Closed"));
                                else if (ex.Message == "51")
                                    return Ok(_api.Error(User,"Year Closed"));
                                else if (ex.Message == "52")
                                    return Ok(_api.Error(User,"Year Exists"));
                                else if (ex.Message == "53")
                                    return Ok(_api.Error(User,"Period Closed"));
                                else if (ex.Message == "54")
                                    return Ok(_api.Error(User,"Txn Date"));
                                else
                                    return Ok(_api.Error(User,ex));
                            } 
                        }
                    }         

                    SortedList Result = new SortedList();
                    transaction.Commit();
                    if(FormID==152)
                        return Ok(_api.Success(Result,"Batch Posted"));
                    else
                        return Ok(_api.Success(Result,"Post Cancelled"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpGet("details")]
        public ActionResult AssSalesDetails(string xDescription,int nFnYearID,int nBranchId, bool bAllBranchData,DateTime dDateFrom,DateTime dDateTo,int flag,int FormID,string xRefNo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction=connection.BeginTransaction();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();

                    string Mastersql = "";
                    string X_Trans = "";
                    if(xRefNo==null)xRefNo="";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@nFnYearID", nFnYearID);
                    Params.Add("@nBranchId", nBranchId);
                    Params.Add("@flag", flag);
                    Params.Add("@dDateFrom", dDateFrom);
                    Params.Add("@dDateTo", dDateTo);
                    //Params.Add("@xRefNo", xRefNo);
           
                    object obj1 = dLayer.ExecuteScalar("Select X_ID from Acc_VoucherTypes where (X_Description='" + xDescription + "' or X_Description_Ar='" + xDescription + "')", connection, transaction);
                    if (obj1 != null)
                        X_Trans = obj1.ToString();

                    Params.Add("@X_Trans", X_Trans);

                    if(FormID==152)
                    {
                        if (bAllBranchData)
                            Mastersql = "SP_Acc_BatchProcessing_Disp @flag,@nCompanyID,@nFnYearID,@X_Trans,@dDateFrom,@dDateTo,0";
                        else
                            Mastersql = "SP_Acc_BatchProcessing_Disp @flag,@nCompanyID,@nFnYearID,@X_Trans,@dDateFrom,@dDateTo,@nBranchId";
                    }
                    else
                    {
                        if(flag==0)
                        {
                            if (bAllBranchData)
                                Mastersql = "Select ROW_NUMBER() OVER (ORDER BY D_voucherDate) as 'S/N', N_VoucherID,X_TransType,X_VoucherNo As [Voucher No],Replace(Convert(Varchar(11),D_VoucherDate,106),' ','-') AS [Voucher Date],X_Remarks as [Remarks],X_ReferenceNo As [Reference No], " +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " Else (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount >0)" +                      
                                            " END As [Debit]," +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " else -1*(Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount <0)" +                                              
                                            " END As [Credit], CONVERT(bit,0) As [Select] from Acc_VoucherMaster"+
                                            " Where Acc_VoucherMaster.B_IsAccPosted = 1 and Acc_VoucherMaster.N_CompanyID= @nCompanyID and Acc_VoucherMaster.N_FnYearID = @nFnYearID and Acc_VoucherMaster.X_TransType=@X_Trans And X_ReferenceNo like '%"+xRefNo+"%' Order By Acc_VoucherMaster.D_VoucherDate";
                            else
                                Mastersql = "Select ROW_NUMBER() OVER (ORDER BY D_voucherDate) as 'S/N', N_VoucherID,X_TransType,X_VoucherNo As [Voucher No],Replace(Convert(Varchar(11),D_VoucherDate,106),' ','-') AS [Voucher Date],X_Remarks as [Remarks],X_ReferenceNo As [Reference No], " +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " Else (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount >0)" +                                              
                                            " END As [Debit]," +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " else -1*(Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount <0)" +        
                                            " END As [Credit], CONVERT(bit,0) As [Select] from Acc_VoucherMaster"+
                                            " Where Acc_VoucherMaster.B_IsAccPosted = 1 and Acc_VoucherMaster.N_CompanyID= @nCompanyID and Acc_VoucherMaster.N_FnYearID = @nFnYearID and Acc_VoucherMaster.X_TransType=@X_Trans And X_ReferenceNo like '%"+xRefNo+"%' and N_BranchID=@nBranchId  Order By Acc_VoucherMaster.D_VoucherDate";
                        }
                        else
                        {
                            if (bAllBranchData)
                                Mastersql = "Select ROW_NUMBER() OVER (ORDER BY D_voucherDate) as 'S/N', N_VoucherID,X_TransType,X_VoucherNo As [Voucher No],Replace(Convert(Varchar(11),D_VoucherDate,106),' ','-') AS [Voucher Date],X_Remarks as [Remarks],X_ReferenceNo As [Reference No], " +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " Else (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount >0)" +                                               
                                            " END As [Debit]," +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            "Else -1*(Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount <0)" +                                                   
                                            " END As [Credit], CONVERT(bit,0) As [Select] from Acc_VoucherMaster"+
                                            " Where Acc_VoucherMaster.B_IsAccPosted = 1 and Acc_VoucherMaster.N_CompanyID= @nCompanyID and Acc_VoucherMaster.N_FnYearID = @nFnYearID and Convert(Varchar(11),D_VoucherDate,23) Between '"+dDateFrom+"' and '"+@dDateTo+"' and Acc_VoucherMaster.X_TransType=@X_Trans And X_ReferenceNo like '%" + xRefNo + "%' Order By Acc_VoucherMaster.D_VoucherDate ";
                            else
                                Mastersql = "Select ROW_NUMBER() OVER (ORDER BY D_voucherDate) as 'S/N', N_VoucherID,X_TransType,X_VoucherNo As [Voucher No],Replace(Convert(Varchar(11),D_VoucherDate,106),' ','-') AS [Voucher Date],X_Remarks as [Remarks],X_ReferenceNo As [Reference No], " +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " Else (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount >0)" +                                               
                                            " END As [Debit]," +
                                            " Case X_TransType When 'PV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            " When 'RV' then (Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID)" +
                                            "Else -1*(Select SUM(N_Amount) FRom Acc_VoucherMaster_Details Where N_VoucherID = Acc_VoucherMaster.N_VoucherID and N_Amount <0)" +                                                   
                                            " END As [Credit], CONVERT(bit,0) As [Select] from Acc_VoucherMaster"+
                                            " Where Acc_VoucherMaster.B_IsAccPosted = 1 and Acc_VoucherMaster.N_CompanyID= @nCompanyID and Acc_VoucherMaster.N_FnYearID = @nFnYearID and Convert(Varchar(11),D_VoucherDate,23) Between '"+dDateFrom+"' and '"+@dDateTo+"' and Acc_VoucherMaster.X_TransType=@X_Trans And X_ReferenceNo like '%" + xRefNo + "%' and N_BranchID=@nBranchId Order By Acc_VoucherMaster.D_VoucherDate ";
                        }
                    }

                    MasterTable = dLayer.ExecuteDataTable(Mastersql, Params, connection,transaction);
                   // if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }

                    MasterTable = _api.Format(MasterTable, "Master");

                    dt.Tables.Add(MasterTable);
                    return Ok(_api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }


       [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID,int N_AssetInventoryID)
        {
            int Results = 0;
            var nUserID = myFunctions.GetUserID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                   if(N_AssetInventoryID>0)
                    {
                        SortedList DeleteParams = new SortedList(){
                            {"N_CompanyID",nCompanyID},                   
                            {"X_TransType","ASSET SALES"},
                            {"N_VoucherID",N_AssetInventoryID},
                            {"N_UserID",nUserID},
                            {"X_SystemName",System.Environment.MachineName}};
                        try
                        {
                            Results = dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", DeleteParams, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User,ex));
                        }
                    }
                    if (Results >= 0)
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Asset Sales deleted"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to delete Asset Sales"));
                    }


                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }
   } 
  
 }