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
        private readonly IMyFunctions myFunctions;


        public Inv_Vendor(IApiFunctions api,IDataAccessLayer dl,IMyFunctions myFun)
        {
            _api=api;
            dLayer = dl;
            myFunctions = myFun;
        }
       

        //GET api/customer/list?....
        [HttpGet("list")]
        public ActionResult GetVendorList(int? nCompanyId,int nFnYearId,bool bAllBranchesData,string vendorId,string qry)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            string criteria="";
            if (vendorId!=""&&vendorId!=null)
            {
                criteria = " and N_VendorID =@vendorId ";
                Params.Add("@vendorId",vendorId);
            }

            string qryCriteria="";
            if (qry!=""&&qry!=null)
            {
                qryCriteria = " and ([Vendor Code] like @qry or [Vendor Name] like @qry ) ";
                Params.Add("@qry","%"+qry+"%");
            }
            string sqlCommandText="select * from vw_InvVendor_Disp where B_Inactive=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3 "+criteria+" "+qryCriteria+" order by [Vendor Name],[Vendor Code]";
            Params.Add("@p1",0);
            Params.Add("@p2",nCompanyId);
            Params.Add("@p3",nFnYearId);
            try{
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params);
                    dt=_api.Format(dt);
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
                    DataRow MasterRow = MasterTable.Rows[0];
                    string VendorCode="";
                    var values = MasterRow["X_VendorCode"].ToString();
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",MasterRow["N_CompanyId"].ToString());
                        Params.Add("N_YearID",MasterRow["N_FnYearId"].ToString());
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
                    return GetVendorList(myFunctions.getIntVAL(MasterRow["N_CompanyId"].ToString()),myFunctions.getIntVAL(MasterRow["N_FnYearId"].ToString()),true,N_VendorId.ToString(),"");
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