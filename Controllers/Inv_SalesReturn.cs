using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("salesreturn")]
    [ApiController]
    public class Inv_SalesReturn : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        
        public Inv_SalesReturn(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
       

        [HttpGet("list")]
        public ActionResult GetSalesReturn(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText= "select * from vw_InvDebitNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);

            try{
                                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                dt=_api.Format(dt);
                if (dt.Rows.Count==0)
                    {
                        return StatusCode(200,_api.Response(200 ,"No Results Found" ));
                    }else{
                        return Ok(dt);
                    }   
            }catch(Exception e){
                return StatusCode(403,_api.ErrorResponse(e));
            }
        }
        [HttpGet("details")]
        public ActionResult GetSalesReturnDetails(int? nCompanyId,string xDebitNoteNo,int nFnYearId,bool bAllBranchData,int nBranchId)
        {
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            string sqlCommandText="";
            if (bAllBranchData == true)
            {
                sqlCommandText = "Select * from vw_SalesReturnMasterWithoutSale_Disp Where N_CompanyID=@CompanyID and X_DebitNoteNo=@DebitNoteNo and N_FnYearID=@FnYearID and B_Invoice=0";
            }
            else
            {
                sqlCommandText="Select * from vw_SalesReturnMasterWithoutSale_Disp Where N_CompanyID=@CompanyID and X_DebitNoteNo=@DebitNoteNo and N_FnYearID=@FnYearID and N_BranchID=@BranchID and B_Invoice=0";

            }
            
            Params.Add("@CompanyID",nCompanyId);
            Params.Add("@FnYearID",nFnYearId);
            Params.Add("@DebitNoteNo",xDebitNoteNo);
            Params.Add("@BranchID",nBranchId);

            try{
                DataTable SalesReturn = new DataTable();
                                                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                SalesReturn=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                SalesReturn=_api.Format(SalesReturn,"Master");
                dt.Tables.Add(SalesReturn);
                
            int N_DebitNoteId = myFunctions.getIntVAL(SalesReturn.Rows[0]["N_DebitNoteId"].ToString());
            Params.Add("@DebitNoteID",N_DebitNoteId);
            string  sqlCommandText2="SELECT   * from  vw_SalesReturnWithoutSale_Disp Where N_DebitNoteId=@DebitNoteID and N_CompanyID=@CompanyID and N_FnYearID=@FnYearID";

            DataTable SalesReturnDetails = new DataTable();
            SalesReturnDetails=dLayer.ExecuteDataTable(sqlCommandText2,Params,connection);
            SalesReturnDetails=_api.Format(SalesReturnDetails,"Details");
            dt.Tables.Add(SalesReturnDetails);

                }



return Ok(dt);
            }catch(Exception e){
                return StatusCode(403,_api.ErrorResponse(e));
            }
        }

       //Save....
       [HttpPost("Save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            
            try{
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string InvoiceNo="";
                    DataRow masterRow=MasterTable.Rows[0];
                    var values = masterRow["X_DebitNoteNo"].ToString();
                    
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",masterRow["n_CompanyId"].ToString());
                        Params.Add("N_YearID",masterRow["n_FnYearId"].ToString());
                        Params.Add("N_FormID",80);
                        Params.Add("N_BranchID",masterRow["n_BranchId"].ToString());
                        InvoiceNo =  dLayer.GetAutoNumber("Inv_SalesReturnMaster","X_DebitNoteNo", Params);
                        if(InvoiceNo==""){return StatusCode(409,_api.Response(409 ,"Unable to generate sales return" ));}
                        MasterTable.Rows[0]["X_DebitNoteNo"] = InvoiceNo;
                    }

                    dLayer.setTransaction();
                    int N_InvoiceId=dLayer.SaveData("Inv_SalesReturnMaster","N_DebitNoteId",0,MasterTable);                    
                    if(N_InvoiceId<=0){
                        dLayer.rollBack();
                        }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                        {
                            DetailTable.Rows[j]["N_DebitNoteId"]=N_InvoiceId;
                        }
                    int N_InvoiceDetailId=dLayer.SaveData("Inv_SalesReturnDetails","N_DebitnoteDetailsID",0,DetailTable);                    
                    dLayer.commit();
                    return Ok("Sales Return Saved");
                }
                catch (Exception ex)
                {
                    dLayer.rollBack();
                    return StatusCode(403,ex);
                }
        }
        //Delete....
         
        
    }
}