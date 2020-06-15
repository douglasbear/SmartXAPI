using AutoMapper;
using SmartxAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("vendor")]
    [ApiController]
    public class Inv_Vendor : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer _dataAccess;
        


        public Inv_Vendor(IApiFunctions api,IDataAccessLayer dataAccess)
        {
            _api=api;
            _dataAccess = dataAccess;
        }
       

        //GET api/customer/list?....
        [HttpGet("list")]
        public ActionResult GetVendorList(int? nCompanyId,int nFnYearId,int nBranchId,bool bAllBranchesData)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table="vw_InvVendor_Disp";
            string X_Fields = "[Vendor Code] as vendorCode,[Vendor Name] as vendorName,Address,N_CompanyID,N_VendorID,B_Inactive,N_FnYearID";
            string X_Crieteria = "";
            string X_OrderBy="[Vendor Name],[Vendor Code]";
            Params.Add("@p1",0);
            Params.Add("@p2",nCompanyId);
            Params.Add("@p3",nFnYearId);
            X_Crieteria = "B_Inactive=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3";
            try{
                    dt=_dataAccess.Select(X_Table,X_Fields,X_Crieteria,Params,X_OrderBy);
                     if(dt.Rows.Count==0)
                    {
                       return StatusCode(200,new { StatusCode = 200 , Message= "No Results Found" });
                    }else{
                        return Ok(dt);
                    }
                }catch(Exception e){
                    return StatusCode(403,_api.ErrorResponse(e));
                }
        }


[HttpGet("all")]
        public ActionResult GetVendor(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table="Inv_Vendor";
            string X_Fields = "*";          
            string X_Crieteria = "";
            string X_OrderBy="";

            try{
                    dt=_dataAccess.Select(X_Table,X_Fields,X_Crieteria,Params,X_OrderBy);
                     if(dt.Rows.Count==0)
                    {
                       return StatusCode(200,new { StatusCode = 200 , Message= "No Results Found" });
                    }else{
                        return Ok(dt);
                    }
                }catch(Exception e){
                    return StatusCode(403,_api.ErrorResponse(e));
                }
        }
        
       //Save....
       [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            try{
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    _dataAccess.StartTransaction();
                    // Auto Gen
                    //var values = MasterTable.Rows[0]["X_CustomerCode"].ToString();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string VendorCode="";
                    var values = MasterTable.Rows[0]["X_VendorCode"].ToString();
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["N_CompanyId"].ToString());
                        Params.Add("N_YearID",MasterTable.Rows[0]["N_FnYearId"].ToString());
                        Params.Add("N_FormID",52);
                        VendorCode =  _dataAccess.GetAutoNumber("Inv_Vendor","X_VendorCode", Params);
                        if(VendorCode==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Vendor Code" ));}
                        MasterTable.Rows[0]["X_VendorCode"] = VendorCode;
                    }


                    int N_VendorId=_dataAccess.SaveData("Inv_Vendor","N_VendorID",0,MasterTable);                    
                    if(N_VendorId<=0){
                        _dataAccess.Rollback();
                        return StatusCode(404,_api.Response(404 ,"Unable to save" ));
                        }else{
                    _dataAccess.Commit();
                    return StatusCode(200,_api.Response(200 ,"Vendor Saved" ));
                        }
                }
                catch (Exception ex)
                {
                    _dataAccess.Rollback();
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nVendorID)
        {
             int Results=0;
            try
            {
                Results=_dataAccess.DeleteData("Inv_Vendor","N_VendorID",nVendorID,"");
                if(Results>0){
                    return StatusCode(200,_api.Response(200 ,"vendor deleted" ));
                }else{
                    return StatusCode(409,_api.Response(409 ,"Unable to delete vendor" ));
                }
                
            }
            catch (Exception ex)
                {
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
            

        }
    }
}