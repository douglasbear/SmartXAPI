using System.Collections.Generic;
using SmartxAPI.Models;
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
    [Route("products")]
    [ApiController]
    public class Inv_ItemMaster : ControllerBase
    {
        private readonly IDataAccessLayer _dataAccess;
        private readonly IApiFunctions _api;

        public Inv_ItemMaster(IDataAccessLayer data,IApiFunctions api)
        {
            _dataAccess = data;
            _api=api;
        }
       
        //GET api/Projects/list
        [HttpGet("list") ]
        public ActionResult <IEnumerable<VwInvItemSearch>> GetAllItems (int? nCompanyID)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table="Vw_InvItem_Search";
            string X_Fields = "*";
            string X_Crieteria = "N_CompanyID=@p1 and B_Inactive=@p2 and [Item Code]<> @p3 and N_ItemTypeID<>@p4";
            string X_OrderBy="[Item Code]";
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",0);
            Params.Add("@p3","001");
            Params.Add("@p4",1);

            try{
                dt=_dataAccess.Select(X_Table,X_Fields,X_Crieteria,Params,X_OrderBy);
                foreach(DataColumn c in dt.Columns)
                    c.ColumnName = String.Join("", c.ColumnName.Split());
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
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    _dataAccess.StartTransaction();
                    // Auto Gen
                    //var values = MasterTable.Rows[0]["X_CustomerCode"].ToString();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string ItemCode="";
                    var values = MasterTable.Rows[0]["X_ItemCode"].ToString();
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["N_CompanyId"].ToString());
                        Params.Add("N_YearID",MasterTable.Rows[0]["N_FnYearId"].ToString());
                        Params.Add("N_FormID",53);
                        ItemCode =  _dataAccess.GetAutoNumber("Inv_Vendor","X_ItemCode", Params);
                        if(ItemCode==""){return StatusCode(409,_api.Response(409 ,"Unable to generate product Code" ));}
                        MasterTable.Rows[0]["X_ItemCode"] = ItemCode;
                    }


                    int N_ItemID=_dataAccess.SaveData("Inv_ItemMaster","N_ItemID",0,MasterTable);                    
                    if(N_ItemID<=0){
                        _dataAccess.Rollback();
                        return StatusCode(404,_api.Response(404 ,"Unable to save" ));
                        }else{
                    _dataAccess.Commit();
                    return StatusCode(200,_api.Response(200 ,"Product Saved" ));
                        }
                }
                catch (Exception ex)
                {
                    _dataAccess.Rollback();
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
        }

       
    }
}