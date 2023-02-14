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
    [Route("schBusRegistration")]
    [ApiController]
    public class Sch_BusRegistration : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID =744 ;


        public Sch_BusRegistration(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetBusRegList(int? nCompanyId, int nAcYearID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_RegistrationCode like '%" + xSearchkey + "%' or X_AdmissionNo like '%" + xSearchkey + "%' or X_Name like '%" + xSearchkey + "%' or X_RouteName like '%" + xSearchkey + "%' or DropRoute like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by X_RegistrationCode desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "X_RegistrationCode":
                        xSortBy = "X_RegistrationCode " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_SchReg_Disp where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID  " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_SchReg_Disp where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + Searchkey + " and N_RegistrationID not in (select top(" + Count + ") N_RegistrationID from vw_SchReg_Disp where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + xSortBy + " ) " + " " + xSortBy;

            Params.Add("@nCompanyId", nCompanyID);
            Params.Add("@nAcYearID", nAcYearID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    SortedList OutPut = new SortedList();

                    sqlCommandCount = "select count(1) as N_Count  from vw_SchReg_Disp where N_CompanyID=@nCompanyId and N_AcYearID=@nAcYearID " + Searchkey + "";
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
        public ActionResult BusRegDetails(string xRegistrationCode)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from vw_SchReg_Disp where N_CompanyID=@p1  and X_RegistrationCode=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", xRegistrationCode);
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

        //Save....
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nRegistrationID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_RegistrationID"].ToString());
                int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_BranchID"].ToString());
                int nLocationID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LocationID"].ToString());
                int nUserID = myFunctions.GetUserID(User);

                if (MasterTable.Columns.Contains("N_BranchID"))
                    MasterTable.Columns.Remove("N_BranchID");
                if (MasterTable.Columns.Contains("N_LocationID"))
                    MasterTable.Columns.Remove("N_LocationID");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string Code = "";
                    var values = MasterTable.Rows[0]["X_RegistrationCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                         Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        Code = dLayer.GetAutoNumber("Sch_BusRegistration", "X_RegistrationCode", Params, connection, transaction);
                        if (Code == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Course Code")); }
                        MasterTable.Rows[0]["X_RegistrationCode"] = Code;
                    }

                    if(nRegistrationID>0)
                    {
                        object PayCount = dLayer.ExecuteScalar("select COUNT(Inv_PayReceiptDetails.N_InventoryID) from Inv_PayReceiptDetails INNER JOIN Sch_Sales ON Sch_Sales.N_CompanyID=Inv_PayReceiptDetails.n_companyid and Sch_Sales.N_RefSalesID=Inv_PayReceiptDetails.N_InventoryID where Inv_PayReceiptDetails.N_CompanyID="+ nCompanyID +" and Inv_PayReceiptDetails.X_TransType='SALES' and Sch_Sales.N_Type=2 and Sch_Sales.N_RefId="+ nRegistrationID, Params, connection, transaction);
                        if (PayCount != null)
                        {
                            if(myFunctions.getIntVAL(PayCount.ToString())==0)
                            {
                                DataTable dtSch_Sales=dLayer.ExecuteDataTable("select N_CompanyId,N_FnYearId,N_RefSalesID,N_SalesID from Sch_Sales where N_CompanyId="+ nCompanyID +" and N_FnYearId="+nFnYearId+" and N_RefId="+nRegistrationID+" and N_Type=2",Params,connection,transaction);

                                for (int j = 0; j < dtSch_Sales.Rows.Count; j++)
                                {
                                    SortedList DeleteParams = new SortedList(){
                                    {"N_CompanyID",nCompanyID},
                                    {"X_TransType","SALES"},
                                    {"N_VoucherID",myFunctions.getIntVAL(dtSch_Sales.Rows[j]["N_RefSalesID"].ToString())}};
                                    try
                                    {
                                        dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                                    }
                                    catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        return Ok(api.Error(User, ex));
                                    }

                                    dLayer.DeleteData("Sch_SalesDetails", "N_SalesID", myFunctions.getIntVAL(dtSch_Sales.Rows[j]["N_SalesID"].ToString()), "N_CompanyID =" + nCompanyID, connection, transaction);                   
                                }
                                dLayer.DeleteData("Sch_Sales", "N_RefId", nRegistrationID, "N_CompanyID =" + nCompanyID+" and N_FnYearId="+nFnYearId+" and N_Type=2", connection, transaction);                                                  
                            }
                        }

                    }

                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId + " and X_RegistrationCode='" + MasterTable.Rows[0]["X_RegistrationCode"]  + "'";
                    string X_Criteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearId;

                    nRegistrationID = dLayer.SaveData("Sch_BusRegistration", "n_RegistrationID", MasterTable, connection, transaction);
                    if (nRegistrationID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }

                    //Bus Fee Payment
                    object PayCount1 = dLayer.ExecuteScalar("select COUNT(Inv_PayReceiptDetails.N_InventoryID) from Inv_PayReceiptDetails INNER JOIN Sch_Sales ON Sch_Sales.N_CompanyID=Inv_PayReceiptDetails.n_companyid and Sch_Sales.N_RefSalesID=Inv_PayReceiptDetails.N_InventoryID where Inv_PayReceiptDetails.N_CompanyID="+ nCompanyID +" and Inv_PayReceiptDetails.X_TransType='SALES'  and Sch_Sales.N_Type=2 and Sch_Sales.N_RefId="+ myFunctions.getIntVAL(MasterTable.Rows[0]["N_AdmissionID"].ToString()), Params, connection, transaction);
                    if (PayCount1 != null)
                    {
                        if(myFunctions.getIntVAL(PayCount1.ToString())==0)
                        {

                            //--------------------------------------FEES - Posting--------------------------------------
                            SortedList SalesParam = new SortedList();
                            SalesParam.Add("N_CompanyID", nCompanyID);
                            SalesParam.Add("N_AcYearID", nFnYearId);
                            SalesParam.Add("N_BranchID", nBranchID);
                            SalesParam.Add("N_LocationID ", nLocationID);
                            SalesParam.Add("N_StudentID ", myFunctions.getIntVAL(MasterTable.Rows[0]["N_AdmissionID"].ToString()));
                            SalesParam.Add("D_AdmDate ", "");
                            SalesParam.Add("N_UserID ", nUserID);
                            SalesParam.Add("N_Type ", 2);
                            SalesParam.Add("N_BusRegID ", nRegistrationID);
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
                    }

                    transaction.Commit();
                    return Ok(api.Success("Bus Registration Completed"));

                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        [HttpGet("detailList") ]
        public ActionResult BusRegList(int nCompanyID)
        {    
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            
            string sqlCommandText="";

            sqlCommandText="select * from vw_SchReg_Disp where N_CompanyID=@p1";

            param.Add("@p1", nCompanyID);             
                
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
        public ActionResult DeleteData(int nRegistrationID)
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

                    if(nRegistrationID>0)
                    {
                        object PayCount = dLayer.ExecuteScalar("select COUNT(Inv_PayReceiptDetails.N_InventoryID) from Inv_PayReceiptDetails INNER JOIN Sch_Sales ON Sch_Sales.N_CompanyID=Inv_PayReceiptDetails.n_companyid and Sch_Sales.N_RefSalesID=Inv_PayReceiptDetails.N_InventoryID where Inv_PayReceiptDetails.N_CompanyID="+ nCompanyID +" and Inv_PayReceiptDetails.X_TransType='SALES' and Sch_Sales.N_Type=2 and Sch_Sales.N_RefId="+ nRegistrationID, Params, connection, transaction);
                        if (PayCount != null)
                        {
                            if(myFunctions.getIntVAL(PayCount.ToString())==0)
                            {
                                DataTable dtSch_Sales=dLayer.ExecuteDataTable("select N_CompanyId,N_FnYearId,N_RefSalesID,N_SalesID from Sch_Sales where N_CompanyId="+ nCompanyID +" and N_RefId="+nRegistrationID+" and N_Type=2",Params,connection,transaction);

                                for (int j = 0; j < dtSch_Sales.Rows.Count; j++)
                                {
                                    SortedList DeleteParams = new SortedList(){
                                    {"N_CompanyID",nCompanyID},
                                    {"X_TransType","SALES"},
                                    {"N_VoucherID",myFunctions.getIntVAL(dtSch_Sales.Rows[j]["N_RefSalesID"].ToString())}};
                                    try
                                    {
                                        dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_SaleAccounts", DeleteParams, connection, transaction);
                                    }
                                    catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        return Ok(api.Error(User, ex));
                                    }

                                    dLayer.DeleteData("Sch_SalesDetails", "N_SalesID", myFunctions.getIntVAL(dtSch_Sales.Rows[j]["N_SalesID"].ToString()), "N_CompanyID =" + nCompanyID, connection, transaction);                   
                                }
                                dLayer.DeleteData("Sch_Sales", "N_RefId", nRegistrationID, "N_CompanyID =" + nCompanyID+" and N_Type=2", connection, transaction);                                                  
                            }
                        }

                    }

                    Results = dLayer.DeleteData("Sch_BusRegistration", "n_RegistrationID", nRegistrationID, "N_CompanyID =" + nCompanyID, connection, transaction);                   
                
                    if (Results > 0)
                    {
                        transaction.Commit();
                        return Ok(api.Success("Bus Registration deleted"));
                    }
                    else
                    {
                        return Ok(api.Error(User,"Unable to delete Bus Registration"));
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }



        }
    }
}

