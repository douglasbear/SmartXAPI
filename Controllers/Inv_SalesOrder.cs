using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("salesorder")]
    [ApiController]
    public class Inv_SalesOrderController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;

        
        public Inv_SalesOrderController(IDataAccessLayer dl,IApiFunctions api)
        {
            dLayer=dl;
            _api=api;
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
                dt=dLayer.ExecuteDataTable(sqlCommandText,Params);
                dt=_api.Format(dt);
                if(dt.Rows.Count==0)
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
    public ActionResult GetSalesOrderDetails(int? nCompanyId,string xOrderNo,int nFnYearId)
        {
            DataSet dt=new DataSet();
            SortedList ParamList=new SortedList();

            try{
                //Sales Order Master
                DataTable MasterTable = new DataTable();
                ParamList.Add("N_CompanyID",nCompanyId.ToString());
                ParamList.Add("X_OrderNo",xOrderNo);
                ParamList.Add("N_Type",1);
                ParamList.Add("N_BranchId",0);
                ParamList.Add("N_FnYearID",nFnYearId.ToString());
                
                MasterTable=dLayer.ExecuteDataTablePro("SP_InvSalesOrder_Disp",ParamList);
                MasterTable=_api.Format(MasterTable,"Master");
                dt.Tables.Add(MasterTable);
                
                //Sales Order Details
                DataTable DetailsTable = new DataTable();
                SortedList ParamList1=new SortedList();
                ParamList1.Add("N_CompanyID",nCompanyId.ToString());
                ParamList1.Add("N_SalesID",MasterTable.Rows[0]["N_SalesOrderId"].ToString());
                
                DetailsTable=dLayer.ExecuteDataTablePro("SP_InvSalesOrderDtls_Disp",ParamList1);
                DetailsTable=_api.Format(DetailsTable,"Details");
                dt.Tables.Add(DetailsTable);
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
                    return  GetSalesOrderDetails(int.Parse(Master["n_CompanyId"].ToString()),MasterTable.Rows[0]["X_OrderNo"].ToString(),int.Parse(Master["n_FnYearId"].ToString()));
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