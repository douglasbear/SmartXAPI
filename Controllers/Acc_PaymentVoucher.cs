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
    [Route("voucher")]
    [ApiController]
    public class Acc_PaymentVoucher : ControllerBase
    {
        private readonly IDataAccessLayer _dataAccess;
        private readonly IApiFunctions _api;
        private readonly IDLayer dLayer;

        
        public Acc_PaymentVoucher(IDataAccessLayer dataaccess,IApiFunctions api,IDLayer dl)
        {
            _dataAccess=dataaccess;
            _api=api;
            dLayer=dl;
        }
       

        [HttpGet("list")]
        public ActionResult GetPaymentVoucherList(int? nCompanyId,int nFnYearId,string voucherType)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table= "vw_AccVoucher_Disp";
            string X_Fields = "*";
            string X_Crieteria = "N_CompanyID=@p1 and N_FnYearID=@p2 and X_TransType=@p3";
            string X_OrderBy="";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);
            Params.Add("@p3",voucherType);

            try{
                dt=_dataAccess.Select(X_Table,X_Fields,X_Crieteria,Params,X_OrderBy);
                foreach (DataColumn c in dt.Columns)
                    c.ColumnName = String.Join("", c.ColumnName.Split());
                if (dt.Rows.Count==0)
                    {
                        return StatusCode(200,_api.Response(200 ,"No Results Found" ));
                    }else{
                        return Ok(dt);
                    }   
            }catch(Exception e){
                return StatusCode(404,_api.Response(404,e.Message));
            }
        }
        [HttpGet("details")]
        public ActionResult GetVoucherDetails(int? nCompanyId,int nFnYearId)
        {
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            
            string X_Table="vw_InvPurchaseInvoiceNo_Search";
            string X_Fields = "*";   
            string X_Crieteria = "N_CompanyID=@p1 and N_FnYearID=@p2 and N_QuotationID=@p3";
            string X_OrderBy="";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);

            try{
                DataTable Quotation = new DataTable();
                
                Quotation=_dataAccess.Select(X_Table,X_Fields,X_Crieteria,Params,X_OrderBy);
                foreach(DataColumn c in Quotation.Columns)
                    c.ColumnName = String.Join("", c.ColumnName.Split());
                dt.Tables.Add(Quotation);
                Quotation.TableName="Master";
                
                //Quotation Details

            string  X_Table1="vw_InvQuotationDetails";
            string X_Fields1 = "*";
            string X_Crieteria1 = "N_CompanyID=@p1 and N_FnYearID=@p2 and N_QuotationID=@p3";
            string X_OrderBy1="";
            DataTable QuotationDetails = new DataTable();
            QuotationDetails=_dataAccess.Select(X_Table1,X_Fields1,X_Crieteria1,Params,X_OrderBy1);
            foreach(DataColumn c in QuotationDetails.Columns)
                    c.ColumnName = String.Join("", c.ColumnName.Split());
            dt.Tables.Add(QuotationDetails);
            QuotationDetails.TableName="Details";

            



return Ok(dt);

                // if(dt.Tables["Master"].Rows.Count==0)
                //     {
                //         return StatusCode(200,_api.Response(200 ,"No Results Found" ));
                //     }else{
                //         return Ok(dt.Tables[0]);
                //     }   
            }catch(Exception e){
                return StatusCode(404,_api.Response(404,e.Message));
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
                    var values = masterRow["x_VoucherNo"].ToString();
                    
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",masterRow["n_CompanyId"].ToString());
                        Params.Add("N_YearID",masterRow["n_FnYearId"].ToString());
                        Params.Add("N_FormID",80);
                        Params.Add("N_BranchID",masterRow["n_BranchId"].ToString());
                        InvoiceNo =  _dataAccess.GetAutoNumber("Acc_VoucherMaster","x_VoucherNo", Params);
                        if(InvoiceNo==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Invoice Number" ));}
                        MasterTable.Rows[0]["x_VoucherNo"] = InvoiceNo;
                    }

                    dLayer.setTransaction();
                    int N_InvoiceId=dLayer.SaveData("Acc_VoucherMaster","N_VoucherId",0,MasterTable);                    
                    if(N_InvoiceId<=0){
                        dLayer.rollBack();
                        }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                        {
                            DetailTable.Rows[j]["N_VoucherId"]=N_InvoiceId;
                        }
                    int N_InvoiceDetailId=dLayer.SaveData("Inv_SalesDetails","n_SalesDetailsID",0,DetailTable);                    
                    dLayer.commit();
                    return GetVoucherDetails(int.Parse(masterRow["n_CompanyId"].ToString()),int.Parse(masterRow["n_FnYearId"].ToString()));
                }
                catch (Exception ex)
                {
                    dLayer.rollBack();
                    return StatusCode(403,ex);
                }
        }
        //Delete....
         [HttpDelete()]
        public ActionResult DeleteData(int N_QuotationID)
        {
             int Results=0;
            try
            {
                _dataAccess.StartTransaction();
                Results=_dataAccess.DeleteData("Inv_SalesQuotation","n_quotationID",N_QuotationID,"");
                if(Results<=0){
                        _dataAccess.Rollback();
                        return StatusCode(409,_api.Response(409 ,"Unable to delete sales quotation" ));
                        }
                        else{
                _dataAccess.DeleteData("Inv_SalesQuotationDetails","n_quotationID",N_QuotationID,"");
                }
                
                if(Results>0){
                    _dataAccess.Commit();
                    return StatusCode(200,_api.Response(200 ,"Sales quotation deleted" ));
                }else{
                    _dataAccess.Rollback();
                    return StatusCode(409,_api.Response(409 ,"Unable to delete sales quotation" ));
                }
                
                }
            catch (Exception ex)
                {
                    return StatusCode(404,_api.Response(404,ex.Message));
                }
            

        }
        
    }
}