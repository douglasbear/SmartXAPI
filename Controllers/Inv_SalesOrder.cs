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
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    
    [Route("salesorder")]
    [ApiController]
    public class Inv_SalesOrderController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        
        public Inv_SalesOrderController(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
       

        [HttpGet("list")]
        public ActionResult GetSalesOrderotationList(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from vw_InvSalesOrderNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 order by D_OrderDate DESC,[Order No]";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);

            try{
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                dt=_api.Format(dt);
                }
                if(dt.Rows.Count==0)
                    {
                        return Ok(_api.Notice("No Results Found" ));
                    }else{
                        return Ok(_api.Success(dt));
                    }   
            }catch(Exception e){
                return BadRequest(_api.Error(e));
            }
        }
        [HttpGet("listDetails")]
    public ActionResult GetSalesOrderDetails(int? nCompanyId,string xOrderNo,int nFnYearId,int nLocationID,bool bAllBranchData,int nBranchID)
        {
            bool B_PRSVisible=false;
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            DataTable DataTable = new DataTable();

            string Mastersql="";
            
            if (bAllBranchData == true){
                Mastersql="SP_InvSalesOrder_Disp @P1,@P3,1,0,@P2";
            }
            else
            {
                Mastersql="SP_InvSalesOrder_Disp @P1,@P3,1,@P4,@P2";
                Params.Add("@P4",nBranchID);
            }

            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);
            Params.Add("@p3",xOrderNo);
            try
            {

 using (SqlConnection connection = new SqlConnection(connectionString))
                {
                MasterTable=dLayer.ExecuteDataTable(Mastersql,Params);
                MasterTable = _api.Format(MasterTable,"Master");
                dt.Tables.Add(MasterTable);
            
            //SalesOrder Details
            string N_SOrderID = MasterTable.Rows[0]["n_SalesOrderId"].ToString();
            
            
            string DetailSql="";
            DetailSql="SP_InvSalesOrderDtls_Disp @p1,@p5,@p2,1,@p4";
            Params.Add("@p4",nLocationID);
            Params.Add("@p5",N_SOrderID);

            DetailTable=dLayer.ExecuteDataTable(DetailSql,Params);
            DetailTable=_api.Format(DetailTable,"Details");
            dt.Tables.Add(DetailTable);
                }
            return Ok(_api.Success(dt));
            }catch(Exception e){
                return BadRequest(_api.Error(e));
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
                    dLayer.setTransaction();
                    // Auto Gen
                    string xOrderNo="";
                    var values = MasterTable.Rows[0]["X_OrderNo"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",Master["n_CompanyId"].ToString());
                        Params.Add("N_YearID",Master["n_FnYearId"].ToString());
                        Params.Add("N_FormID",81);
                        Params.Add("N_BranchID",Master["n_BranchId"].ToString());
                        xOrderNo =  dLayer.GetAutoNumber("Inv_SalesOrder","X_OrderNo", Params);
                        if(xOrderNo==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Sales Order Number" ));}
                        MasterTable.Rows[0]["X_OrderNo"] = xOrderNo;
                    }

                    
                    int nSalesOrderID=dLayer.SaveData("Inv_SalesOrder","N_SalesOrderID",0,MasterTable);                    
                    if(nSalesOrderID<=0){
                        dLayer.rollBack();
                        return StatusCode(409,_api.Response(409 ,"Unable to save sales order" ));
                        }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                        {
                            DetailTable.Rows[j]["N_SalesOrderID"]=nSalesOrderID;
                        }
                    int N_QuotationDetailId=dLayer.SaveData("Inv_SalesOrderDetails","N_SalesOrderDetails",0,DetailTable);                    
                    if(N_QuotationDetailId<=0){
                        dLayer.rollBack();
                        return StatusCode(409,_api.Response(409 ,"Unable to save sales order" ));
                    }else{
                        dLayer.commit();
                    }
                    
                    return  GetSalesOrderDetails(int.Parse(Master["n_CompanyId"].ToString()),MasterTable.Rows[0]["X_OrderNo"].ToString(),int.Parse(Master["n_FnYearId"].ToString()),int.Parse(Master["N_LocationID"].ToString()),true,1);
                }
                catch (Exception ex)
                {
                    dLayer.rollBack();
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
        }
        //Delete....
         [HttpDelete("delete")]
        public ActionResult DeleteData(int nSalesOrderID)
        {
             int Results=0;
            try
            {
                dLayer.setTransaction();
                Results=dLayer.DeleteData("Inv_SalesOrder","N_SalesOrderID",nSalesOrderID,"");
                if(Results<=0){
                        dLayer.rollBack();
                        return StatusCode(409,_api.Response(409 ,"Unable to delete sales order" ));
                        }
                        else{
                Results=dLayer.DeleteData("Inv_SalesOrderDetails","N_SalesOrderID",nSalesOrderID,"");
                }
                
                if(Results>0){
                    dLayer.commit();
                    return StatusCode(200,_api.Response(200 ,"Sales order deleted" ));
                }else{
                    dLayer.rollBack();
                    return StatusCode(409,_api.Response(409 ,"Unable to delete sales order" ));
                }
                
                }
            catch (Exception ex)
                {
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
            

        }
        
    }
}