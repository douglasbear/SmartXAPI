using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("purchaseorder")]
    [ApiController]
    public class Inv_PurchaseOrderController : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;

        
        public Inv_PurchaseOrderController(IDataAccessLayer dl,IApiFunctions api,IMyFunctions myFun)
        {
            dLayer=dl;
            _api=api;
            myFunctions=myFun;
        }
       

        [HttpGet("list")]
        public ActionResult GetPurchaseOrderList(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from vw_InvPurchaseOrderNo_Search where N_CompanyID=@p1 and N_FnYearID=@p2 order by D_POrderDate DESC,[Order No]";
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
            }catch(SqlException e){
                return StatusCode(404,_api.Response(404,e.StackTrace));
            }
        }
        [HttpGet("listDetails")]
        public ActionResult GetPurchaseOrderDetails(int nCompanyId,string xPOrderId,int nFnYearId,string nLocationID,string xPRSNo,bool bAllBranchData,int nBranchID)
        {
            bool B_PRSVisible=false;
            DataSet dt=new DataSet();
            SortedList Params=new SortedList();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            DataTable DataTable = new DataTable();

            string Mastersql="";
            
            if (bAllBranchData == true){
                Mastersql="SELECT Inv_PurchaseOrder.N_CompanyID, Inv_PurchaseOrder.N_FnYearID, Inv_PurchaseOrder.X_Comments, Inv_PurchaseOrder.N_POrderID, Inv_PurchaseOrder.N_InvDueDays, Inv_PurchaseOrder.X_POrderNo, Inv_PurchaseOrder.N_VendorID, Inv_PurchaseOrder.D_EntryDate, Inv_PurchaseOrder.D_POrderDate, Inv_PurchaseOrder.N_InvoiceAmtF AS N_InvoiceAmt, Inv_PurchaseOrder.N_DiscountAmtF AS N_DiscountAmt, Inv_PurchaseOrder.N_CashPaidF AS N_CashPaid, Inv_PurchaseOrder.N_FreightAmtF AS N_FreightAmt, Inv_PurchaseOrder.N_userID, Inv_PurchaseOrder.N_Processed, Inv_PurchaseOrder.N_PurchaseID, Inv_PurchaseOrder.N_LocationID, Inv_PurchaseOrder.X_Description, Inv_PurchaseOrder.N_BranchID, Inv_PurchaseOrder.B_CancelOrder, Inv_PurchaseOrder.D_ExDelvDate, Inv_Location.X_LocationName, Inv_PurchaseOrder.X_Currency, Inv_PurchaseOrder.X_QutationNo, Inv_PurchaseOrder.X_PaymentMode, Inv_PurchaseOrder.X_DeliveryPlace, Inv_PurchaseOrder.N_DeliveryPlaceID, Inv_PurchaseOrder.N_ProcStatus, Inv_PurchaseOrder.N_CurrencyID, Inv_PurchaseOrder.N_ExchangeRate, Inv_PurchaseOrder.B_IsSaveDraft, Inv_PurchaseOrder.X_TandC, Inv_PurchaseOrder.X_Attention, Inv_PurchaseOrder.N_TaxAmt, Inv_PurchaseOrder.N_TaxAmtF, Inv_PurchaseOrder.N_NextApprovalID, Inv_PurchaseOrder.N_POType, Gen_Defaults.X_TypeName,Inv_PurchaseOrder.N_ApprovalLevelID,Inv_PurchaseOrder.N_SOId, isnull(Inv_PurchaseOrder.N_ProjectID,0) as N_ProjectID  FROM Inv_PurchaseOrder LEFT OUTER JOIN Gen_Defaults ON Inv_PurchaseOrder.N_POType = Gen_Defaults.N_TypeId LEFT OUTER JOIN Inv_Location ON Inv_PurchaseOrder.N_LocationID = Inv_Location.N_LocationID  Where Inv_PurchaseOrder.N_CompanyID=@p1 and N_FnYearID=@p2 and Inv_PurchaseOrder.X_POrderNo=@p3";
            }
            else
            {
                Mastersql="SELECT Inv_PurchaseOrder.N_CompanyID, Inv_PurchaseOrder.N_FnYearID,Inv_PurchaseOrder.X_Comments, Inv_PurchaseOrder.N_POrderID, Inv_PurchaseOrder.N_InvDueDays, Inv_PurchaseOrder.X_POrderNo, Inv_PurchaseOrder.N_VendorID, Inv_PurchaseOrder.D_EntryDate, Inv_PurchaseOrder.D_POrderDate,  Inv_PurchaseOrder.N_InvoiceAmtF AS N_InvoiceAmt, Inv_PurchaseOrder.N_DiscountAmtF AS N_DiscountAmt, Inv_PurchaseOrder.N_CashPaidF AS N_CashPaid, Inv_PurchaseOrder.N_FreightAmtF AS N_FreightAmt, Inv_PurchaseOrder.N_userID, Inv_PurchaseOrder.N_Processed, Inv_PurchaseOrder.N_PurchaseID, Inv_PurchaseOrder.N_LocationID, Inv_PurchaseOrder.X_Description, Inv_PurchaseOrder.N_BranchID, Inv_PurchaseOrder.B_CancelOrder, Inv_PurchaseOrder.D_ExDelvDate, Inv_Location.X_LocationName, Inv_PurchaseOrder.X_Currency, Inv_PurchaseOrder.X_QutationNo, Inv_PurchaseOrder.X_PaymentMode, Inv_PurchaseOrder.X_DeliveryPlace, Inv_PurchaseOrder.N_DeliveryPlaceID, Inv_PurchaseOrder.N_ProcStatus, Inv_PurchaseOrder.N_CurrencyID, Inv_PurchaseOrder.N_ExchangeRate, Inv_PurchaseOrder.B_IsSaveDraft, Inv_PurchaseOrder.X_TandC, Inv_PurchaseOrder.X_Attention, Inv_PurchaseOrder.N_TaxAmt, Inv_PurchaseOrder.N_TaxAmtF, Inv_PurchaseOrder.N_NextApprovalID, Inv_PurchaseOrder.N_POType, Gen_Defaults.X_TypeName,Inv_PurchaseOrder.N_ApprovalLevelID,Inv_PurchaseOrder.N_SOId,isnull(Inv_PurchaseOrder.N_ProjectID,0) as N_ProjectID FROM Inv_PurchaseOrder LEFT OUTER JOIN Gen_Defaults ON Inv_PurchaseOrder.N_POType = Gen_Defaults.N_TypeId LEFT OUTER JOIN Inv_Location ON Inv_PurchaseOrder.N_LocationID = Inv_Location.N_LocationID Where Inv_PurchaseOrder.N_CompanyID=@p1 and Inv_PurchaseOrder.X_POrderNo=@p3 and Inv_PurchaseOrder.N_BranchID=@nBranchID and N_FnYearID=@p2";
                Params.Add("@nBranchID",nBranchID);
            }
            
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nFnYearId);
            Params.Add("@p3",xPOrderId);

            try{

                MasterTable=dLayer.ExecuteDataTable(Mastersql,Params);
                MasterTable = _api.Format(MasterTable,"Master");
                dt.Tables.Add(MasterTable);
             
                //PurchaseOrder Details
            int N_POrderID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_POrderID"].ToString());
            
            string DetailSql="";
            bool MaterailRequestVisible = myFunctions.CheckPermission(nCompanyId,556, "Administrator",dLayer);
            bool PurchaseRequestVisible = myFunctions.CheckPermission(nCompanyId,1049, "Administrator",dLayer);
            if (MaterailRequestVisible || PurchaseRequestVisible){
                B_PRSVisible = true;
                DataColumn prsCol = new DataColumn("B_PRSVisible", typeof(System.Boolean));
                prsCol.DefaultValue=B_PRSVisible;
                DataTable.Columns.Add(prsCol);
            }
             if (B_PRSVisible)
                if (xPRSNo!= "")
                {
                    DetailSql="Select *,dbo.SP_GenGetStock(vw_InvPurchaseOrderDetailsForPRS.N_ItemID,@p4,'','Location') As N_Stock,dbo.SP_Cost(vw_InvPurchaseOrderDetailsForPRS.N_ItemID,vw_InvPurchaseOrderDetailsForPRS.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseOrderDetailsForPRS.N_ItemID,vw_InvPurchaseOrderDetailsForPRS.N_CompanyID) As N_UnitSPrice from vw_InvPurchaseOrderDetailsForPRS Where N_CompanyID=@p1 and (X_PRSNo In (Select X_PRSNo from vw_InvPurchaseOrderDetailsForPRS Where N_POrderID = @p5)  or X_PRSNo In (Select 0 from vw_InvPurchaseOrderDetailsForPRS Where N_POrderID = @p5)) and  (N_POrderID =@p5 OR N_POrderID IS Null)";
                    Params.Add("@p4",nLocationID);
                    Params.Add("@p5",N_POrderID);
                }
                else
                {
                    if (bAllBranchData == true){
                        DetailSql="Select *,dbo.SP_GenGetStock(vw_InvPurchaseOrderDetails.N_ItemID,@p4,'','Location') As N_Stock,dbo.SP_Cost(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID) As N_UnitSPrice from vw_InvPurchaseOrderDetails Where N_CompanyID=@p1 and N_POrderID=@p5";
                        Params.Add("@p4",nLocationID);
                        Params.Add("@p5",N_POrderID);
                    }
                    else{
                        DetailSql="Select *,dbo.SP_GenGetStock(vw_InvPurchaseOrderDetails.N_ItemID,@p4,'','Location') As N_Stock,dbo.SP_Cost(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID) As N_UnitSPrice from vw_InvPurchaseOrderDetails Where N_CompanyID=@p1 and N_POrderID=nPOrderID and N_BranchID=@nBranchID";
                        Params.Add("@p4",nLocationID);
                        Params.Add("@p5",N_POrderID);
                        }
                }
            else
            {

                if (bAllBranchData == true){
                    DetailSql="Select *,dbo.SP_GenGetStock(vw_InvPurchaseOrderDetails.N_ItemID,@p4,'','Location') As N_Stock,dbo.SP_Cost(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID) As N_UnitSPrice from vw_InvPurchaseOrderDetails Where N_CompanyID=@p1 and N_POrderID=@p5";
                    Params.Add("@p4",nLocationID);
                    Params.Add("@p5",N_POrderID);
                }
                else{
                    DetailSql="Select *,dbo.SP_GenGetStock(vw_InvPurchaseOrderDetails.N_ItemID,@p4,'','Location') As N_Stock,dbo.SP_Cost(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID,'') As N_UnitLPrice ,dbo.SP_SellingPrice(vw_InvPurchaseOrderDetails.N_ItemID,vw_InvPurchaseOrderDetails.N_CompanyID) As N_UnitSPrice from vw_InvPurchaseOrderDetails Where N_CompanyID=@p1 and N_POrderID=@p5 and N_BranchID=@nBranchID";
                    Params.Add("@p4",nLocationID);
                    Params.Add("@p5",N_POrderID);
                    }
            }



            DetailTable=dLayer.ExecuteDataTable(DetailSql,Params);
            DetailTable=_api.Format(DetailTable,"Details");
            dt.Tables.Add(DetailTable);
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

                    // Auto Gen
                    var values = MasterTable.Rows[0]["x_PurchaseOrderNo"].ToString();


                    dLayer.setTransaction();
                    int N_PurchaseOrderId=dLayer.SaveData("Inv_PurchaseOrder","N_PurchaseOrderId",0,MasterTable);                    
                    if(N_PurchaseOrderId<=0){
                        dLayer.rollBack();
                        }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                        {
                            DetailTable.Rows[j]["n_PurchaseOrderID"]=N_PurchaseOrderId;
                        }
                    int N_PurchaseOrderDetailId=dLayer.SaveData("Inv_PurchaseOrderDetails","n_PurchaseOrderDetailsID",0,DetailTable);                    
                    dLayer.commit();
                    return Ok("DataSaved");
                }
                catch (Exception ex)
                {
                    dLayer.rollBack();
                    return StatusCode(403,ex);
                }
        }
        //Delete....
         [HttpDelete()]
        public ActionResult DeleteData(int N_PurchaseOrderID)
        {
             int Results=0;
            try
            {
                dLayer.setTransaction();
                Results=dLayer.DeleteData("Inv_PurchaseOrder","n_PurchaseOrderID",N_PurchaseOrderID,"");
                if(Results<=0){
                        dLayer.rollBack();
                        return StatusCode(409,_api.Response(409 ,"Unable to delete sales PurchaseOrder" ));
                        }
                        else{
                Results=dLayer.DeleteData("Inv_PurchaseOrderDetails","n_PurchaseOrderID",N_PurchaseOrderID,"");
                }
                
                if(Results>0){
                    dLayer.commit();
                    return StatusCode(200,_api.Response(200 ,"Sales PurchaseOrder deleted" ));
                }else{
                    dLayer.rollBack();
                    return StatusCode(409,_api.Response(409 ,"Unable to delete sales PurchaseOrder" ));
                }
                
                }
            catch (Exception ex)
                {
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
            

        }
        
    }
}