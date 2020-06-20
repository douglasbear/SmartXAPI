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
        private readonly IDataAccessLayer _dataAccess;
        private readonly IApiFunctions _api;

        
        public Inv_SalesOrderController(IDataAccessLayer dataaccess,IApiFunctions api)
        {
            _dataAccess=dataaccess;
            _api=api;
        }
       

        [HttpGet("list")]
        public ActionResult GetSalesOrderotationList(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table="vw_InvSalesOrderNo_Search";
            string X_Fields = "[X_FileNo],[Order No],[Order Date],[Customer],X_PurchaseOrderNo,X_xOrderNo,N_Amount,TransType,x_Notes,N_CompanyID,N_CustomerID,N_SalesOrderId,N_FnYearID,D_OrderDate,N_BranchID,B_YearEndProcess,N_ApproveLevel,N_ProcStatus,N_Type";
            string X_Crieteria = "N_CompanyID=@p1 and N_FnYearID=@p2";
            string X_OrderBy="D_OrderDate DESC,[Order No]";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);

            try{
                dt=_dataAccess.Select(X_Table,X_Fields,X_Crieteria,Params,X_OrderBy);
                foreach(DataColumn c in dt.Columns)
                    c.ColumnName = String.Join("", c.ColumnName.Split());
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
                
                MasterTable=_dataAccess.ExecuteProcDataTable("SP_InvSalesOrder_Disp",ParamList);
                foreach(DataColumn c in MasterTable.Columns)
                    c.ColumnName = String.Join("", c.ColumnName.Split());
                dt.Tables.Add(MasterTable);
                MasterTable.TableName="Master";
                
                //Sales Order Details
                DataTable DetailsTable = new DataTable();
                ParamList.Add("N_CompanyID",nCompanyId.ToString());
                ParamList.Add("N_SalesID",MasterTable.Rows[0]["N_SalesOrderId"].ToString());
                
                MasterTable=_dataAccess.ExecuteProcDataTable("SP_InvSalesOrderDtls_Disp",ParamList);
                foreach(DataColumn c in DetailsTable.Columns)
                        c.ColumnName = String.Join("", c.ColumnName.Split());
                dt.Tables.Add(DetailsTable);
                DetailsTable.TableName="Details";
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
                    _dataAccess.StartTransaction();
                    // Auto Gen
                    string xOrderNo="";
                    var values = MasterTable.Rows[0]["X_OrderNo"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",Master["n_CompanyId"].ToString());
                        Params.Add("N_YearID",Master["n_FnYearId"].ToString());
                        Params.Add("N_FormID",81);
                        Params.Add("N_BranchID",Master["n_BranchId"].ToString());
                        xOrderNo =  _dataAccess.GetAutoNumber("Inv_SalesOrder","X_OrderNo", Params);
                        if(xOrderNo==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Sales Order Number" ));}
                        MasterTable.Rows[0]["X_OrderNo"] = xOrderNo;
                    }

                    
                    int nSalesOrderID=_dataAccess.SaveData("Inv_SalesOrder","N_SalesOrderID",0,MasterTable);                    
                    if(nSalesOrderID<=0){
                        _dataAccess.Rollback();
                        return StatusCode(409,_api.Response(409 ,"Unable to save sales order" ));
                        }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                        {
                            DetailTable.Rows[j]["N_SalesOrderID"]=nSalesOrderID;
                        }
                    int N_QuotationDetailId=_dataAccess.SaveData("Inv_SalesOrderDetails","N_SalesOrderDetails",0,DetailTable);                    
                    if(N_QuotationDetailId<=0){
                        _dataAccess.Rollback();
                        return StatusCode(409,_api.Response(409 ,"Unable to save sales order" ));
                    }else{
                        _dataAccess.Commit();
                    }
                    return  GetSalesOrderDetails(int.Parse(Master["n_CompanyId"].ToString()),MasterTable.Rows[0]["X_OrderNo"].ToString(),int.Parse(Master["n_FnYearId"].ToString()));
                }
                catch (Exception ex)
                {
                    _dataAccess.Rollback();
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
                _dataAccess.StartTransaction();
                Results=_dataAccess.DeleteData("Inv_SalesOrder","N_SalesOrderID",nSalesOrderID,"");
                if(Results<=0){
                        _dataAccess.Rollback();
                        return StatusCode(409,_api.Response(409 ,"Unable to delete sales order" ));
                        }
                        else{
                Results=_dataAccess.DeleteData("Inv_SalesOrderDetails","N_SalesOrderID",nSalesOrderID,"");
                }
                
                if(Results>0){
                    _dataAccess.Commit();
                    return StatusCode(200,_api.Response(200 ,"Sales order deleted" ));
                }else{
                    _dataAccess.Rollback();
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