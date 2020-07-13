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
    [Route("salesman")]
    [ApiController]
    
    
    
    public class Inv_Salseman : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        

        public Inv_Salseman(IDataAccessLayer dl,IApiFunctions api)
        {
            dLayer = dl;
            _api=api;
        }

       
        //List
        [HttpGet("list") ]
        public ActionResult GetAllSalesExecutives (int? nCompanyID,int? nFnyearID)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from vw_InvSalesman_Disp where N_CompanyID=@p1 and N_FnYearID=@p2";
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nFnyearID);
                
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

        //List
        [HttpGet("listdetails") ]
        public ActionResult GetAllSalesExecutivesDetails (int? nCompanyID,int? nFnyearID,int? n_SalesmanID)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from vw_InvSalesman_Disp where N_CompanyID=@p1 and N_FnYearID=@p2 and n_SalesmanID=@p3";
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nFnyearID);
            Params.Add("@p3",n_SalesmanID);
                
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

          //Save....
       [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            try
            {
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    dLayer.setTransaction();
                    // Auto Gen
                    SortedList Params = new SortedList();
                    string ExecutiveCode="";
                    var values = MasterTable.Rows[0]["X_SalesmanCode"].ToString();
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID",MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID",290);
                        Params.Add("N_BranchID",MasterTable.Rows[0]["n_BranchId"].ToString());
                        ExecutiveCode =  dLayer.GetAutoNumber("inv_salesman","X_SalesmanCode", Params);
                        if(ExecutiveCode==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Sales Executive Code" ));}
                        MasterTable.Rows[0]["X_SalesmanCode"] = ExecutiveCode;
                    
                    }
                    int N_SalesmanID=dLayer.SaveData("inv_salesman","N_SalesmanID",0,MasterTable);                    
                    if(N_SalesmanID<=0){
                        dLayer.rollBack();
                        return StatusCode(404,_api.Response(404 ,"Unable to save" ));
                        }else{
                    dLayer.commit();
                    return  GetAllSalesExecutivesDetails(int.Parse(MasterTable.Rows[0]["n_CompanyId"].ToString()),int.Parse(MasterTable.Rows[0]["n_FnYearId"].ToString()),N_SalesmanID);
                        }
                }
                catch (Exception ex)
                {
                    dLayer.rollBack();
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
        }

         [HttpDelete("delete")]
        public ActionResult DeleteData(int nSalesmanID)
        {
             int Results=0;
            try
            {
                Results=dLayer.DeleteData("inv_salesman","N_SalesmanID",nSalesmanID,"");
                if(Results>0){
                    return StatusCode(200,_api.Response(200 ,"Sales Executive deleted" ));
                }else{
                    return StatusCode(409,_api.Response(409 ,"Unable to delete Sales Executive" ));
                }
                
            }
            catch (Exception ex)
                {
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
            

        }

       

       
    }
}