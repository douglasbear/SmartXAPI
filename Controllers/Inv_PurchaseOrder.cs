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
                Mastersql="SELECT Inv_PurchaseOrder.*, Inv_Location.X_LocationName,Gen_Defaults.X_TypeName, Inv_Vendor.X_VendorName FROM Inv_PurchaseOrder INNER JOIN Inv_Vendor ON Inv_PurchaseOrder.N_CompanyID = Inv_Vendor.N_CompanyID AND Inv_PurchaseOrder.N_VendorID = Inv_Vendor.N_VendorID AND  Inv_PurchaseOrder.N_FnYearID = Inv_Vendor.N_FnYearID LEFT OUTER JOIN Gen_Defaults ON Inv_PurchaseOrder.N_POType = Gen_Defaults.N_TypeId LEFT OUTER JOIN Inv_Location ON Inv_PurchaseOrder.N_LocationID = Inv_Location.N_LocationID Where Inv_PurchaseOrder.N_CompanyID=@p1 and Inv_PurchaseOrder.N_FnYearID=@p2 and Inv_PurchaseOrder.X_POrderNo=@p3";
            }
            else
            {
                Mastersql="SELECT Inv_PurchaseOrder.*, Inv_Location.X_LocationName,Gen_Defaults.X_TypeName, Inv_Vendor.X_VendorName FROM Inv_PurchaseOrder INNER JOIN Inv_Vendor ON Inv_PurchaseOrder.N_CompanyID = Inv_Vendor.N_CompanyID AND Inv_PurchaseOrder.N_VendorID = Inv_Vendor.N_VendorID AND  Inv_PurchaseOrder.N_FnYearID = Inv_Vendor.N_FnYearID LEFT OUTER JOIN Gen_Defaults ON Inv_PurchaseOrder.N_POType = Gen_Defaults.N_TypeId LEFT OUTER JOIN Inv_Location ON Inv_PurchaseOrder.N_LocationID = Inv_Location.N_LocationID Where Inv_PurchaseOrder.N_CompanyID=@p1 and Inv_PurchaseOrder.X_POrderNo=@p3 and Inv_PurchaseOrder.N_BranchID=@nBranchID and Inv_PurchaseOrder.N_FnYearID=@p2";
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
            //DetailSql="Select * from Inv_PurchaseOrderDetails Where N_CompanyID=@p1 and N_POrderID=@p5";
                    


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
                    SortedList Params = new SortedList();
                    dLayer.setTransaction();
                    // Auto Gen
                    string PorderNo="";
                    var values = MasterTable.Rows[0]["x_POrderNo"].ToString();
                    DataRow Master = MasterTable.Rows[0];
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",Master["n_CompanyId"].ToString());
                        Params.Add("N_YearID",Master["n_FnYearId"].ToString());
                        Params.Add("N_FormID",80);
                        Params.Add("N_BranchID",Master["n_BranchId"].ToString());
                        PorderNo =  dLayer.GetAutoNumber("Inv_PurchaseOrder","x_POrderNo", Params);
                        if(PorderNo==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Quotation Number" ));}
                        MasterTable.Rows[0]["x_POrderNo"] = PorderNo;
                    }

                    int N_PurchaseOrderId=dLayer.SaveData("Inv_PurchaseOrder","n_POrderID",0,MasterTable);                    
                    if(N_PurchaseOrderId<=0){
                        dLayer.rollBack();
                        }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                        {
                            DetailTable.Rows[j]["n_POrderID"]=N_PurchaseOrderId;
                        }
                    int N_PurchaseOrderDetailId=dLayer.SaveData("Inv_PurchaseOrderDetails","n_POrderDetailsID",0,DetailTable);                    
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