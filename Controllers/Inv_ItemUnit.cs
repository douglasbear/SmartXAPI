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
    [Route("itemunit")]
    [ApiController]
    public class Inv_ItemUnit : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;

        
        public Inv_ItemUnit(IDataAccessLayer dl,IApiFunctions api)
        {
            dLayer=dl;
            _api=api;
        }
       

        [HttpGet("list")]
        public ActionResult GetItemUnitList(int? nCompanyId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select Code,[Unit Code],Description from vw_InvItemUnit_Disp where N_CompanyID=@p1 and N_ItemID is null order by ItemCode,[Unit Code]";
            Params.Add("@p1",nCompanyId);

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
        
       //Save....
       [HttpPost("Save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            try{
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    
                    SortedList Params = new SortedList();
                    dLayer.setTransaction();
                    int N_ItemUnitID=dLayer.SaveData("Inv_ItemUnit","N_ItemUnitID",0,MasterTable);                    
                    if(N_ItemUnitID<=0){
                        dLayer.rollBack();
                        return StatusCode(409,_api.Response(409 ,"Unable to save ItemUnit" ));
                        }
                   else{
                        dLayer.commit();
                    }
                    // return  GetSalesQuotationDetails(int.Parse(Master["n_CompanyId"].ToString()),N_QuotationId,int.Parse(Master["n_FnYearId"].ToString()));
                return Ok();
                }

                catch (Exception ex)
                {
                    dLayer.rollBack();
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
        }
        
    }
}