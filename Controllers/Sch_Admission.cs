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
    [Route("schAdmission")]
    [ApiController]
    public class Sch_Admission : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =155 ;


        public Sch_Admission(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetAdmissionList(int? nCompanyId, int nAcYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int nRegID)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_AdmissionNo like '%" + xSearchkey + "%' or X_Name like '%" + xSearchkey + "%' or X_PFamilyName like '%" + xSearchkey + "%' or X_PMotherName like '%" + xSearchkey + "%' or X_GaurdianName like '%" + xSearchkey + "%'  or X_RegNo like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_AdmissionNo desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_AdmissionNo":
                        xSortBy = "X_AdmissionNo " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }


            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_SchAdmission_Dashboard where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID  " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_SchAdmission_Dashboard where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + Searchkey + " and N_AdmissionID not in (select top(" + Count + ") N_AdmissionID from vw_SchAdmission where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + xSortBy + " ) " + " " + xSortBy;

            Params.Add("@nCompanyId", nCompanyID);
            Params.Add("@nAcYearID", nAcYearID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(*) as N_Count  from vw_SchAdmission_Dashboard  where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + Searchkey + "";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("details")]
        public ActionResult AdmissionDetails(string xAdmissionNo, int nAcYearID,int nRegID)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable BusDetails = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "";

           if (xAdmissionNo!=null)
           {
            Params.Add("@p2", xAdmissionNo);
             sqlCommandText=" select * from vw_SchAdmission where N_CompanyID=@p1 and x_AdmissionNo=@p2 and N_AcYearID=@p4";
           }

            if(nRegID>0)
            {
                 Params.Add("@nRegID", nRegID);
                sqlCommandText=" select * from vw_StudentRegToAdmission where N_CompanyId=@p1 and N_RegID=@nRegID";
            }
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p4", nAcYearID);
            //  Params.Add("@nRegID", nRegID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                       if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                
                    MasterTable = api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);


             

                 

                    int N_AdmissionID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AdmissionID"].ToString());
                    Params.Add("@p3", N_AdmissionID);

                    string BusDetailSql = "select * from vw_SchReg_Disp where N_CompanyID=@p1 and N_AdmissionID=@p3 and N_AcYearID=@p4";

                    BusDetails = dLayer.ExecuteDataTable(BusDetailSql, Params, connection);
                    BusDetails = api.Format(BusDetails, "BusDetails");
                    dt.Tables.Add(BusDetails);
                }
                return Ok(api.Success(dt));

                
                          
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                DataTable dtCustomer;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nAcYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AcYearID"].ToString());
                int nAdmissionID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AdmissionID"].ToString());
                int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_BranchID"].ToString());
                int nLocationID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LocationID"].ToString());
                int nUserID = myFunctions.GetUserID(User);
                int nAdmID=nAdmissionID;

                if (MasterTable.Columns.Contains("N_BranchID"))
                    MasterTable.Columns.Remove("N_BranchID");
                if (MasterTable.Columns.Contains("N_LocationID"))
                    MasterTable.Columns.Remove("N_LocationID");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    SortedList CustParams = new SortedList();

                    // Auto Gen
                    string Code = "",CustCode="";
                    var values = MasterTable.Rows[0]["X_AdmissionNo"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Sch_Admission", "X_AdmissionNo", Params, connection, transaction);
                        if (Code == "") {
                             transaction.Rollback();
                             return Ok(api.Error(User,"Unable to generate Admission No")); }
                        MasterTable.Rows[0]["X_AdmissionNo"] = Code;

                        //Generating Customer Code
                        CustParams.Add("N_CompanyID", nCompanyID);
                        CustParams.Add("N_YearID", nFnYearId);
                        CustParams.Add("N_FormID", 51);
                        CustCode = dLayer.GetAutoNumber("Inv_Customer", "X_CustomerCode", CustParams, connection, transaction);
                        if (CustCode == "") 
                        { 
                            transaction.Rollback();
                        return Ok(api.Error(User,"Unable to generate Customer Code")); 
                        }
                    }
                    
                     MasterTable.Columns.Remove("n_FnYearId");
                     string image = myFunctions.ContainColumn("i_Photo", MasterTable) ? MasterTable.Rows[0]["i_Photo"].ToString() : "";
                     Byte[] photoBitmap = new Byte[image.Length];
                     photoBitmap = Convert.FromBase64String(image);
                    if (myFunctions.ContainColumn("i_Photo", MasterTable))
                        MasterTable.Columns.Remove("i_Photo");
                        MasterTable.AcceptChanges();

                    // if (nAdmissionID > 0) 
                    // {  
                    //     dLayer.DeleteData("Sch_Admission", "N_AdmissionID", nAdmissionID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    // }

                    string DupCriteriaAd = "N_CompanyID=" + nCompanyID + " and N_AcYearID=" + nAcYearID + " and X_AdmissionNo='" + Code + "'";
                    string X_CriteriaAd = "N_CompanyID=" + nCompanyID + " and N_AcYearID=" + nAcYearID;

                    nAdmissionID = dLayer.SaveData("Sch_Admission", "N_AdmissionID",DupCriteriaAd,X_CriteriaAd, MasterTable, connection, transaction);
                    if (nAdmissionID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }

                    if (image.Length > 0)
                    {
                        dLayer.SaveImage("Sch_Admission", "I_Photo", photoBitmap, "N_AdmissionID",nAdmissionID, connection, transaction);
                    }

                    
                    //----------------------------------Customer Insert-------------------------------------------------------
                    string sqlCommandText = "SELECT N_CompanyID,N_CustomerID,'"+CustCode+"' AS X_CustomerCode,X_Name AS X_CustomerName,X_GaurdianName AS X_ContactName,X_StudentMobile AS X_PhoneNo1,0 AS N_CreditLimit,0 AS B_Inactive," +
                                            " (select N_FieldValue from Acc_AccountDefaults where x_fielddescr='Debtor Account' and N_CompanyID=vw_SchAdmission.N_CompanyID and N_FnYearID=vw_SchAdmission.N_AcYearID) AS N_LedgerID," +
                                            " N_AcYearID AS N_FnYearID,GETDATE() AS D_EntryDate,D_DOB,2 AS N_TypeID,N_BranchId,"+
                                            " (select N_CountryID from Acc_Company where N_CompanyID=vw_SchAdmission.N_CompanyID) AS N_CountryID," +
                                            " (select N_CurrencyID from Acc_Company where N_CompanyID=vw_SchAdmission.N_CompanyID) AS N_CurrencyID" +
                                            " FROM vw_SchAdmission where N_CompanyID="+nCompanyID+" and N_AcYearID="+nAcYearID+" and N_AdmissionID="+ nAdmissionID;

                    dtCustomer = dLayer.ExecuteDataTable(sqlCommandText, Params,connection,transaction);

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and X_CustomerCode='" + CustCode + "'";
                    string X_Criteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId;
                    int nCustomerID = dLayer.SaveData("Inv_Customer", "n_CustomerID", DupCriteria, X_Criteria, dtCustomer, connection, transaction);
                    if (nCustomerID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to Genrate Customer"));
                    }
                        
                    object N_GroupID = dLayer.ExecuteScalar("Select Isnull(N_FieldValue,0) From Acc_AccountDefaults Where N_CompanyID=" + nCompanyID + " and X_FieldDescr ='Customer Account Group' and N_FnYearID=" + nFnYearId, Params, connection, transaction);
                    string X_LedgerName = "";

                    bool b_AutoGenerate =true;
                    b_AutoGenerate = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("155", "AutoGenerate_CustomerAccount", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection,transaction)));

                    if (b_AutoGenerate)
                    {
                        X_LedgerName = MasterTable.Rows[0]["X_Name"].ToString();
                        if (N_GroupID != null)
                        {
                            object N_LedgerID = dLayer.ExecuteScalar("Select Isnull(N_LedgerID,0) From Acc_MastLedger Where N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and X_LedgerName='" + X_LedgerName + "' and N_GroupID=" + myFunctions.getIntVAL(N_GroupID.ToString()), Params, connection, transaction);
                            if (N_LedgerID != null)
                            {
                                dLayer.ExecuteNonQuery("Update Inv_Customer Set N_LedgerID =" + myFunctions.getIntVAL(N_LedgerID.ToString()) + " Where N_CustomerID =" + nCustomerID + " and N_CompanyID=" + nCompanyID + " and N_FnyearID= " + nFnYearId, Params, connection, transaction);
                            }
                            else
                            {
                                dLayer.ExecuteNonQuery("SP_Inv_CreateCustomerAccount " + nCompanyID + "," + nCustomerID + ",'" + CustCode + "','" + X_LedgerName + "'," + myFunctions.GetUserID(User) + "," + nFnYearId + "," + "Customer", Params, connection, transaction);
                            }
                        }
                    }
                    //--------------------------------------------^^^^^^^^^^^^---------------------------------------------------- 
                    dLayer.ExecuteNonQuery("update Sch_Admission set N_CustomerID="+nCustomerID+"  where N_CompanyID="+nCompanyID+" and N_AcYearID="+nAcYearID+" and N_AdmissionID="+nAdmissionID, Params, connection, transaction);

                    if(nAdmID==0)
                    {
                        //--------------------------------------Sch_Sales - SALES - Posting--------------------------------------
                        SortedList SalesParam = new SortedList();
                        SalesParam.Add("N_CompanyID", nCompanyID);
                        SalesParam.Add("N_AcYearID", nFnYearId);
                        SalesParam.Add("N_BranchID", nBranchID);
                        SalesParam.Add("N_LocationID ", nLocationID);
                        SalesParam.Add("N_StudentID ", nAdmissionID);
                        SalesParam.Add("N_CustomerID ", nCustomerID);
                        SalesParam.Add("D_AdmDate ", Convert.ToDateTime(MasterTable.Rows[0]["D_AdmissionDate"].ToString()));
                        SalesParam.Add("N_UserID ", nUserID);
                        try
                        {
                            dLayer.ExecuteNonQueryPro("SP_StudentAdmFee_Insert", SalesParam, connection, transaction);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User, ex));
                        }
                      
                        //----------------------------------------^^^^^^^^^^^^^^^^^^^^^^^^-------------------------------------
                    }

                    transaction.Commit();
                    return Ok(api.Success("Admission Completed"));

                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        [HttpGet("detailList") ]
        public ActionResult AdmissionList(int nCompanyID,int nAcYearID,int n_ClassID,int n_DivisionID)
        {    
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="";

            if(n_ClassID!=0)
            {
                if(n_DivisionID!=0)
                    sqlCommandText="select * from vw_SchAdmission where N_CompanyID=@p1 and n_AcYearID=@p2 and n_ClassID=@p3 and n_DivisionID=@p4";
                else                    
                    sqlCommandText="select * from vw_SchAdmission where N_CompanyID=@p1 and n_AcYearID=@p2 and n_ClassID=@p3 ";
            }
            else
            {
                if(n_DivisionID!=0)
                    sqlCommandText="select * from vw_SchAdmission where N_CompanyID=@p1 and n_AcYearID=@p2 and n_DivisionID=@p4";
                else                    
                    sqlCommandText="select * from vw_SchAdmission where N_CompanyID=@p1 and n_AcYearID=@p2";
            }

            param.Add("@p1", nCompanyID);             
            param.Add("@p2", nAcYearID);             
            param.Add("@p3", n_ClassID);             
            param.Add("@p4", n_DivisionID);             
                
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
                }
                if(dt.Rows.Count==0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }              
            }
            catch(Exception e)
            {
                return Ok(api.Error(User,e));
            }   
        }   
      
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nAdmissionID)
        {

            int Results = 0;
            int nCompanyID=myFunctions.GetCompanyID(User);
            try
            {                        
                SortedList Params = new SortedList();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    Results = dLayer.DeleteData("Sch_Admission", "n_AdmissionID", nAdmissionID, "N_CompanyID =" + nCompanyID, connection, transaction);                   
                
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("Student deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User,"Unable to delete Student"));
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        [HttpGet("feeDetails")]
        public ActionResult GetFeeDetails(int nAcYearID,int nClassID,int nStudentTypeID,int nAdmissionID)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            
            if(nAdmissionID==0)
                sqlCommandText = "select N_CompanyID,N_AcYearID,X_FeeDescription,N_Amount,N_DiscountAmt,N_FeeCodeID,N_FrequencyID,N_Frequency,N_Installment from vw_SchStudentFee where N_CompanyID=@p1 and N_AcYearID=@p2 and N_ClassID=@p3 and N_StudentTypeID=@p4 order by N_Sort ASC";
            else
                sqlCommandText = "select N_CompanyID,N_AcYearID,X_FeeDescription,N_Amount,N_DiscountAmt,N_FeeCodeID,N_FrequencyID,N_Frequency,N_Installment from vw_SchStudentFee_view where N_CompanyID=@p1 and N_AcYearID=@p2 and N_RefID=@p5 order by N_Sort ASC";

            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", nAcYearID);
            Params.Add("@p3", nClassID);
            Params.Add("@p4", nStudentTypeID);
            Params.Add("@p5", nAdmissionID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                
                    MasterTable = api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);
                }
                return Ok(api.Success(dt));               
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
    }
}

