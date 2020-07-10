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
    [Route("customer")]
    [ApiController]
    public class Inv_Customer : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        
        public Inv_Customer(IApiFunctions api,IDataAccessLayer dl)
        {
            _api=api;
            dLayer = dl;
        }
       

        //GET api/customer/list?....
        [HttpGet("list")]
        public ActionResult GetCustomerList(int? nCompanyId,int nFnYearId,int nBranchId,bool bAllBranchesData)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            string X_Crieteria="";
            if (bAllBranchesData == true)
                {X_Crieteria = " where B_Inactive=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3";}
                else
                {X_Crieteria = " where B_Inactive=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3  and (N_BranchID=@p4 or N_BranchID=@p5)"; 
                Params.Add("@p4",0);
                Params.Add("@p5",nBranchId);}
            string sqlCommandText="select [Customer Code] as customerCode,[Customer Name] as customerName,[Contact Person] as contactPerson,Address,N_CompanyID,N_CustomerId,B_Inactive,N_FnYearID,N_BranchID from vw_InvCustomer_Disp " +X_Crieteria+ " order by [Customer Name],[Customer Code]";
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
        public ActionResult GetCustomer(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from Inv_Customer";
            

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
                    string CustomerCode="";
                    var values = MasterTable.Rows[0]["X_CustomerCode"].ToString();
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID",MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID",51);
                        Params.Add("N_BranchID",MasterTable.Rows[0]["n_BranchId"].ToString());
                        CustomerCode =  dLayer.GetAutoNumber("Inv_Customer","X_CustomerCode", Params);
                        if(CustomerCode==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Customer Code" ));}
                        MasterTable.Rows[0]["X_CustomerCode"] = CustomerCode;
                    }


                    int N_CustomerID=dLayer.SaveData("Inv_Customer","N_CustomerID",0,MasterTable);                    
                    if(N_CustomerID<=0){
                        dLayer.rollBack();
                        return StatusCode(404,_api.Response(404 ,"Unable to save" ));
                        }else{
                    dLayer.commit();
                    return  GetCustomerDetails(N_CustomerID, int.Parse(MasterTable.Rows[0]["n_CompanyId"].ToString()), int.Parse(MasterTable.Rows[0]["n_FnYearId"].ToString()));
                        }
                }
                catch (Exception ex)
                {
                    dLayer.rollBack();
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
        }

[HttpGet("paymentmethod")]
        public ActionResult GetPayMethod()
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from Inv_CustomerType order by X_TypeName";

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
        [HttpGet("getdetails")]
        public ActionResult GetCustomerDetails(int nCustomerID,int nCompanyID,int nFnyearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from Inv_Customer where N_CompanyID=@p1 and N_FnYearID=@p2 and N_CustomerID=@p3 ";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnyearID);
            Params.Add("@p3", nCustomerID);

            try
            {
                dt = dLayer.ExecuteDataTable(sqlCommandText,Params);
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return StatusCode(200, _api.Response(200, "No Results Found"));
                }
                else
                {
                    return Ok(dt);
                }

            }
            catch (Exception e)
            {
                return StatusCode(403, _api.ErrorResponse(e));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCustomerId)
        {
             int Results=0;
            try
            {
                Results=dLayer.DeleteData("Inv_Customer","N_CustomerID",nCustomerId,"");
                if(Results>0){
                    return StatusCode(200,_api.Response(200 ,"customer deleted" ));
                }else{
                    return StatusCode(409,_api.Response(409 ,"Unable to delete customer" ));
                }
                
            }
            catch (Exception ex)
                {
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
            

        }
    }
}