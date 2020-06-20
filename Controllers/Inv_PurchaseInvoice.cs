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
    [Route("purchaseinvoice")]
    [ApiController]
    public class Inv_PurchaseInvoice : ControllerBase
    {
        private readonly IDataAccessLayer _dataAccess;
        private readonly IApiFunctions _api;
        private readonly IDLayer dLayer;
        
        public Inv_PurchaseInvoice(IDataAccessLayer dataaccess,IApiFunctions api,IDLayer dl)
        {
            _dataAccess=dataaccess;
            _api=api;
            dLayer=dl;
        }
       

        [HttpGet("list")]
        public ActionResult GetPurchaseInvoiceList(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table= "vw_InvPurchaseInvoiceNo_Search";
            string X_Fields = "N_PurchaseID,[Invoice No],[Vendor Code],Vendor,[Invoice Date],InvoiceNetAmt";
            string X_Crieteria = "N_CompanyID=@p1 and N_FnYearID=@p2";
            string X_OrderBy="";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);

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
        public ActionResult GetPurchaseInvoiceDetails(int? nCompanyId,int nFnYearId,int nPurchaseId,bool showAllBranch,int nBranchId)
        {
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            
            string X_Table="vw_Inv_PurchaseDisp";
            string X_Fields = "*";   
            string X_Crieteria = "";
            string X_OrderBy="";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);
            Params.Add("@p3",nPurchaseId);
            Params.Add("@p4","PURCHASE");
            if (showAllBranch == true ){
            X_Crieteria = "N_CompanyID=@p1 and N_FnYearID=@p2 and N_PurchaseID=@p3 and X_TransType=@p4";
            }
            else{
            Params.Add("@p5",nBranchId);
            X_Crieteria = "N_CompanyID=@p1 and N_FnYearID=@p2 and N_PurchaseID=@p3 and X_TransType=@p4 and  N_BranchId=@p5";
            }
            


            try{
                DataTable PurchaseInvoice = new DataTable();
                
                PurchaseInvoice=_dataAccess.Select(X_Table,X_Fields,X_Crieteria,Params,X_OrderBy);

                if(PurchaseInvoice.Rows.Count==0){return Ok(new {});}

                foreach(DataColumn c in PurchaseInvoice.Columns)
                    c.ColumnName = String.Join("", c.ColumnName.Split());
                dt.Tables.Add(PurchaseInvoice);
                PurchaseInvoice.TableName="Master";
                
                //PurchaseInvoice Details

            string  X_Table1="vw_InvPurchaseDetails Left Outer Join Inv_PurchaseOrder On vw_InvPurchaseDetails.N_POrderID=Inv_PurchaseOrder.N_POrderID";
            string X_Fields1 = "vw_InvPurchaseDetails.*,Inv_PurchaseOrder.X_POrderNo,dbo.SP_Cost(vw_InvPurchaseDetails.N_ItemID,vw_InvPurchaseDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseDetails.N_ItemID,vw_InvPurchaseDetails.N_CompanyID) As N_UnitSPrice";
            string X_Crieteria1 = "vw_InvPurchaseDetails.N_CompanyID=@p1 and vw_InvPurchaseDetails.N_PurchaseID=@p3";
            string X_OrderBy1="";
            DataTable PurchaseInvoiceDetails = new DataTable();
            PurchaseInvoiceDetails=_dataAccess.Select(X_Table1,X_Fields1,X_Crieteria1,Params,X_OrderBy1);
            foreach(DataColumn c in PurchaseInvoiceDetails.Columns)
                    c.ColumnName = String.Join("", c.ColumnName.Split());
            dt.Tables.Add(PurchaseInvoiceDetails);
            PurchaseInvoiceDetails.TableName="Details";

            



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
                    var values = masterRow["x_ReceiptNo"].ToString();
                    
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",masterRow["n_CompanyId"].ToString());
                        Params.Add("N_YearID",masterRow["n_FnYearId"].ToString());
                        Params.Add("N_FormID",80);
                        Params.Add("N_BranchID",masterRow["n_BranchId"].ToString());
                        InvoiceNo =  _dataAccess.GetAutoNumber("Inv_Purchase","x_InvoiceNo", Params);
                        if(InvoiceNo==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Invoice Number" ));}
                        MasterTable.Rows[0]["x_ReceiptNo"] = InvoiceNo;
                    }

                    dLayer.setTransaction();
                    int N_InvoiceId=dLayer.SaveData("Inv_Purchase","N_SalesId",0,MasterTable);                    
                    if(N_InvoiceId<=0){
                        dLayer.rollBack();
                        }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                        {
                            DetailTable.Rows[j]["N_SalesId"]=N_InvoiceId;
                        }
                    int N_InvoiceDetailId=dLayer.SaveData("Inv_SalesDetails","n_SalesDetailsID",0,DetailTable);                    
                    dLayer.commit();
                    return GetSalesInvoiceDetails(int.Parse(masterRow["n_CompanyId"].ToString()),int.Parse(masterRow["n_FnYearId"].ToString()),int.Parse(masterRow["n_BranchId"].ToString()),InvoiceNo);
                }
                catch (Exception ex)
                {
                    dLayer.rollBack();
                    return StatusCode(403,ex);
                }
        }
        //Delete....
         [HttpDelete()]
        public ActionResult DeleteData(int N_PurchaseInvoiceID)
        {
             int Results=0;
            try
            {
                _dataAccess.StartTransaction();
                Results=_dataAccess.DeleteData("Inv_SalesPurchaseInvoice","n_PurchaseInvoiceID",N_PurchaseInvoiceID,"");
                if(Results<=0){
                        _dataAccess.Rollback();
                        return StatusCode(409,_api.Response(409 ,"Unable to delete sales PurchaseInvoice" ));
                        }
                        else{
                _dataAccess.DeleteData("Inv_SalesPurchaseInvoiceDetails","n_PurchaseInvoiceID",N_PurchaseInvoiceID,"");
                }
                
                if(Results>0){
                    _dataAccess.Commit();
                    return StatusCode(200,_api.Response(200 ,"Sales PurchaseInvoice deleted" ));
                }else{
                    _dataAccess.Rollback();
                    return StatusCode(409,_api.Response(409 ,"Unable to delete sales PurchaseInvoice" ));
                }
                
                }
            catch (Exception ex)
                {
                    return StatusCode(404,_api.Response(404,ex.Message));
                }
            

        }
        
    }
}