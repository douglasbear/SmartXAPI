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
    [Route("schInvoiceGeneration")]
    [ApiController]
    public class Sch_InvoiceGeneration : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID =1657 ;
        public Sch_InvoiceGeneration(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = 
            conf.GetConnectionString("SmartxConnection");
        }

        //Filling Fee schedules from Sch_Sales
        [HttpGet("feeSchedules")]
        public ActionResult FeeScheduleFilling(int n_BranchID, int N_LocationID,int N_FnYearID , DateTime d_Date)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCondition="";

            if(d_Date!=null)
                sqlCondition= " and Sch_Sales.D_SalesDate <= @d_Date";

            string sqlCommandText = "SELECT     Sch_Sales.D_SalesDate AS Invoice_Date, ROW_NUMBER() over(order by Sch_Sales.N_SalesID)+(select ISNULL(MAX(N_SalesID),0) from Inv_Sales where N_CompanyID=@N_CompanyID) AS Invoice_Number, "+
				                            " Inv_Customer.X_CustomerName AS Customer_Name,vw_InvItemMaster.X_ItemName AS Item_Name,vw_InvItemMaster.X_ItemUnit AS unit,1 AS Qty,Sch_Sales.N_SalesAmt AS Price,'' AS Tax_Perc,Sch_Sales.N_DiscountAmt AS Discount, "+
				                            " (select X_BranchName from Acc_BranchMaster where N_CompanyID=@N_CompanyID and N_BranchID=@N_BranchID) AS Branch,(select X_LocationName from Inv_Location where N_CompanyID=@N_CompanyID and N_LocationID=@N_LocationID) AS Location, "+
				                            " @N_CompanyID AS N_CompanyID,vw_InvItemMaster.X_ItemCode AS Item_Code,Inv_Customer.X_CustomerCode AS Customer_Code,Sch_Sales.N_SalesId AS SchSalesID ,Sch_Admission.N_AdmissionID,Sch_Admission.X_Name,Sch_Admission.N_ClassID, "+
                                            " Sch_Class.X_Class, Sch_Admission.N_DivisionID,Sch_ClassDivision.X_ClassDivision,Sch_Sales.X_InvoiceNo,Sch_SalesDetails.N_ItemID AS N_FeeCodeID "+
	                                    " FROM         Sch_SalesDetails INNER JOIN "+
					                        " Sch_Sales ON Sch_SalesDetails.N_CompanyID = Sch_Sales.N_CompanyId AND Sch_SalesDetails.N_SalesID = Sch_Sales.N_SalesId INNER JOIN "+
					                        " Sch_FeeCodes ON Sch_SalesDetails.N_CompanyID = Sch_FeeCodes.N_CompanyID AND Sch_SalesDetails.N_ItemID = Sch_FeeCodes.N_FeeCodeID INNER JOIN "+
					                        " vw_InvItemMaster ON Sch_FeeCodes.N_ItemID = vw_InvItemMaster.N_ItemID AND Sch_FeeCodes.N_CompanyID = vw_InvItemMaster.N_CompanyID INNER JOIN "+
					                        " Inv_Customer ON Sch_Sales.N_CompanyId=Inv_Customer.N_CompanyID AND Sch_Sales.N_FnYearId=Inv_Customer.N_FnYearID AND Sch_Sales.N_CustomerID=Inv_Customer.N_CustomerID INNER JOIN "+
					                        " Sch_Admission ON Sch_Admission.N_CompanyID=Sch_Sales.N_CompanyId AND Sch_Admission.N_AcYearID=Sch_Sales.N_FnYearId AND Sch_Admission.N_CustomerID=Sch_Sales.N_CustomerID INNER JOIN "+						                    
						                    " Sch_Class ON Sch_Admission.N_CompanyID=Sch_Class.N_CompanyId AND Sch_Admission.N_ClassID=Sch_Class.N_ClassID INNER JOIN "+
						                    " Sch_ClassDivision ON Sch_Admission.N_CompanyID=Sch_ClassDivision.N_CompanyID AND Sch_Admission.N_DivisionID=Sch_ClassDivision.N_ClassDivisionID"+
	                                    " WHERE Sch_Sales.N_CompanyID=@N_CompanyID and Sch_Sales.N_FnYearID=@N_AcYearID and ISNULL(Sch_Sales.N_RefSalesID,0)=0 " + sqlCondition + "";

            Params.Add("@N_CompanyID", nCompanyId);  
            Params.Add("@N_BranchID", n_BranchID);
            Params.Add("@N_LocationID", N_LocationID);
            Params.Add("@N_AcYearID", N_FnYearID);
            Params.Add("@d_Date", d_Date);
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



        //Invoice Generation....
        [HttpPost("invoiceGenerate")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["details"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nLocationID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_LocationID"].ToString());
                int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_BranchID"].ToString());
                int nUserID = myFunctions.GetUserID(User);
                int nMigSalesID=0;
                DateTime d_InvoiceDate = Convert.ToDateTime(MasterTable.Rows[0]["d_InvoiceDate"].ToString());
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();

                    MasterTable.Columns.Remove("n_FnYearId");
                    MasterTable.Columns.Remove("n_LocationID");
                    MasterTable.Columns.Remove("n_BranchID");
                    MasterTable.Columns.Remove("d_InvoiceDate");
                    
                    dLayer.ExecuteNonQuery("delete from Mig_SalesInvoice where N_CompanyID= "+nCompanyID+"", Params, connection, transaction);                        

                    string DupCriteria = "";
                    string X_Criteria = "";

                    foreach (DataRow drow in MasterTable.Rows){
                        drow["Invoice_Date"] = d_InvoiceDate;
                    }

                    nMigSalesID = dLayer.SaveData("Mig_SalesInvoice", "Invoice_Number",DupCriteria,X_Criteria, MasterTable, connection, transaction);

                    if (nMigSalesID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        SortedList InsertParam = new SortedList();
                        InsertParam.Add("N_CompanyID", nCompanyID);
                        InsertParam.Add("N_AcYearID", nFnYearID);
                        InsertParam.Add("N_BranchID", nBranchID);
                        InsertParam.Add("N_LocationID", nLocationID);
                        InsertParam.Add("N_UserID", nUserID);

                        dLayer.ExecuteNonQueryPro("[SP_StudentAdmFee_InvoiceGenerate]", InsertParam, connection, transaction);

                        transaction.Commit();
                        return Ok(api.Success("Course Created"));
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

