using System.Collections.Generic;
using SmartxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using System.Threading.Tasks;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("products")]
    [ApiController]
    public class Inv_ItemMaster : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;

        public Inv_ItemMaster(IDataAccessLayer dl,IApiFunctions api)
        {
            dLayer = dl;
            _api=api;
        }
       
        //GET api/Projects/list
        [HttpGet("list") ]
        public ActionResult <IEnumerable<VwInvItemSearch>> GetAllItems (int? nCompanyID)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlComandText="select * from Vw_InvItem_Search where N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4 order by [Item Code]";
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",0);
            Params.Add("@p3","001");
            Params.Add("@p4",1);

            try{
                //await Task.Run(() => { 
                    dt=dLayer.ExecuteDataTable(sqlComandText,Params); 
                    dt=_api.Format(dt);
                  //  });
                    
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
       [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            try{
                    DataTable MasterTable,GeneralTable;
                    MasterTable = ds.Tables["master"];
                    GeneralTable = ds.Tables["general"];
                    dLayer.setTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string ItemCode="";
                    var values = MasterTable.Rows[0]["X_ItemCode"].ToString();
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["N_CompanyId"].ToString());
                        Params.Add("N_YearID",GeneralTable.Rows[0]["N_FnYearId"].ToString());
                        Params.Add("N_FormID",53);
                        ItemCode =  dLayer.GetAutoNumber("Inv_ItemMaster","X_ItemCode", Params);
                        if(ItemCode==""){return StatusCode(409,_api.Response(409 ,"Unable to generate product Code" ));}
                        MasterTable.Rows[0]["X_ItemCode"] = ItemCode;
                    }


                    int N_ItemID=dLayer.SaveData("Inv_ItemMaster","N_ItemID",0,MasterTable);                    
                    if(N_ItemID<=0){
                        dLayer.rollBack();
                        return StatusCode(404,_api.Response(404 ,"Unable to save" ));
                        }else{
                    dLayer.commit();
                    return StatusCode(200,_api.Response(200 ,"Product Saved" ));
                        }
                }
                catch (Exception ex)
                {
                    dLayer.rollBack();
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
        }


           
        //GET api/Projects/list
        [HttpGet("class") ]
        public ActionResult GetItemClass ()
        {
            DataTable dt=new DataTable();
            
            string sqlComandText="select * from Inv_ItemClass order by N_Order ASC";
            try{
                dt=dLayer.ExecuteDataTable(sqlComandText);
                dt=_api.Format(dt);
                if(dt.Rows.Count==0)
                    {
                        return Ok(new {});
                    }else{
                        return Ok(dt);
                    }
                
            }catch(Exception e){
                return StatusCode(403,_api.ErrorResponse(e));
            }
            
        }
        [HttpGet("all")]
        public ActionResult GetAll()
        {
            DataTable dt=new DataTable();
            
            string sqlCommandText="select * from Inv_ItemMaster";

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

       
    }

    
}