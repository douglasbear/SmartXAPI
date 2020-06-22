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
    [Route("itemcategory")]
    [ApiController]
    public class Inv_ItemCategory : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        


        public Inv_ItemCategory(IApiFunctions api,IDataAccessLayer dl)
        {
            _api=api;
            dLayer = dl;
        }
       

        //GET api/productcategory/list?....
        [HttpGet("list")]
        public ActionResult GetItemCategory(int? nCompanyId)
        {
           DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select X_CategoryCode,X_Category from Inv_ItemCategory where N_CompanyID=@p1 order by X_CategoryCode";
            Params.Add("@p1",nCompanyId);

            try{
                dt=dLayer.ExecuteDataTable(sqlCommandText,Params);
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


[HttpGet("all")]
        public ActionResult GetAllItemCategory(int? nCompanyId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from Inv_ItemCategory";

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
                    // Auto Gen
                    //var values = MasterTable.Rows[0]["X_CustomerCode"].ToString();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string CategoryCode="";
                    var values = MasterTable.Rows[0]["X_CategoryCode"].ToString();
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["N_CompanyId"].ToString());
                       // Params.Add("N_YearID",MasterTable.Rows[0]["N_FnYearId"].ToString());
                        Params.Add("N_FormID",73);
                        CategoryCode =  dLayer.GetAutoNumber("Inv_ItemCategory","X_CategoryCode", Params);
                        if(CategoryCode==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Category Code" ));}
                        MasterTable.Rows[0]["X_CategoryCode"] = CategoryCode;
                    }


                    int N_CategoryID=dLayer.SaveData("Inv_ItemCategory","N_CategoryID",0,MasterTable);                    
                    if(N_CategoryID<=0){
                        dLayer.rollBack();
                        return StatusCode(404,_api.Response(404 ,"Unable to save" ));
                        }else{
                    dLayer.commit();
                    return StatusCode(200,_api.Response(200 ,"Product category Saved" ));
                        }
                }
                catch (Exception ex)
                {
                    dLayer.rollBack();
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCategoryID)
        {
             int Results=0;
            try
            {
                Results=dLayer.DeleteData("Inv_ItemCategory","N_CategoryID",nCategoryID,"");
                if(Results>0){
                    return StatusCode(200,_api.Response(200 ,"Product category deleted" ));
                }else{
                    return StatusCode(409,_api.Response(409 ,"Unable to delete product category" ));
                }
                
            }
            catch (Exception ex)
                {
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
            

        }
    }
}