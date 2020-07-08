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
        private readonly IDataAccessLayer dLayer;
        


        public Inv_Vendor(IApiFunctions api,IDataAccessLayer dl)
        {
            _api=api;
            dLayer = dl;
        }
       

        //GET api/customer/list?....
        [HttpGet("list")]
        public ActionResult GetVendorList(int? nCompanyId,int nFnYearId,int nBranchId,bool bAllBranchesData)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            string sqlCommandText="select [Vendor Code] as vendorCode,[Vendor Name] as vendorName,Address,N_CompanyID,N_VendorID,B_Inactive,N_FnYearID from vw_InvVendor_Disp where B_Inactive=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3 order by [Vendor Name],[Vendor Code]";
            Params.Add("@p1",0);
            Params.Add("@p2",nCompanyId);
            Params.Add("@p3",nFnYearId);
            try{
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params);
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
            
            string sqlCommandText="select * from Inv_Vendor";

            try{
                    dt=dLayer.ExecuteDataTable(sqlCommandText);
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
                    dLayer.setTransaction();
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
                        VendorCode =  dLayer.GetAutoNumber("Inv_Vendor","X_VendorCode", Params);
                        if(VendorCode==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Vendor Code" ));}
                        MasterTable.Rows[0]["X_VendorCode"] = VendorCode;
                    }


                    int N_VendorId=dLayer.SaveData("Inv_Vendor","N_VendorID",0,MasterTable);                    
                    if(N_VendorId<=0){
                        dLayer.rollBack();
                        return StatusCode(404,_api.Response(404 ,"Unable to save" ));
                        }else{
                    dLayer.commit();
                    return StatusCode(200,_api.Response(200 ,"Vendor Saved" ));
                        }
                }
                catch (Exception ex)
                {
                    dLayer.rollBack();
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nVendorID)
        {
             int Results=0;
            try
            {
                Results=dLayer.DeleteData("Inv_Vendor","N_VendorID",nVendorID,"");
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