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
    [Route("location")]
    [ApiController]
    public class Inv_Location : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        
        public Inv_Location(IApiFunctions api,IDataAccessLayer dl)
        {
            _api=api;
            dLayer = dl;
        }
         [HttpGet("list")]
        public ActionResult GetLocationDetails(int? nCompanyId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from vw_InvLocation_Disp where N_CompanyID=@p1 order by N_LocationID DESC";
            Params.Add("@p1",nCompanyId);

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

       

        [HttpGet("listdetails")]
        public ActionResult GetLocationDetails(int? nCompanyId,int? nLocationId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from vw_InvLocation_Disp where N_CompanyID=@p1 and N_LocationID=@p2 order by N_LocationID DESC";
            Params.Add("@p1",nCompanyId);
            Params.Add("@p2",nLocationId);

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



        
       //Save....
       [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            try{
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    dLayer.setTransaction();
                   
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string LocationCode="";
                    var values = MasterTable.Rows[0]["X_LocationCode"].ToString();
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID",MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID",450);
                        Params.Add("N_BranchID",MasterTable.Rows[0]["n_BranchId"].ToString());
                        LocationCode =  dLayer.GetAutoNumber("Inv_Location","X_LocationCode", Params);
                        if(LocationCode==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Location Code" ));}
                        MasterTable.Rows[0]["X_LocationCode"] = LocationCode;
                    }

                    MasterTable.Columns.Remove("n_FnYearId");

                    int N_LocationID=dLayer.SaveData("Inv_Location","N_LocationID",0,MasterTable);                    
                    if(N_LocationID<=0){
                        dLayer.rollBack();
                        return StatusCode(404,_api.Response(404 ,"Unable to save" ));
                        }else{
                    dLayer.commit();
                    return  GetLocationDetails(int.Parse(MasterTable.Rows[0]["n_CompanyId"].ToString()),N_LocationID);
                        }
                }
                catch (Exception ex)
                {
                    dLayer.rollBack();
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nLocationId)
        {
             int Results=0;
            try
            {
                Results=dLayer.DeleteData("Inv_Location","N_LocationID",nLocationId,"");
                if(Results>0){
                    return StatusCode(200,_api.Response(200 ,"Location deleted" ));
                }else{
                    return StatusCode(409,_api.Response(409 ,"Unable to delete Location" ));
                }
                
            }
            catch (Exception ex)
                {
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
            

        }
    }
}