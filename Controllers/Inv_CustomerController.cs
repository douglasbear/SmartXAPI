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
    public class Inv_CustomerController : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer _dataAccess;


        public Inv_CustomerController(IApiFunctions api,IDataAccessLayer dataAccess)
        {
            _api=api;
            _dataAccess = dataAccess;
        }
       

        //GET api/customer/list?....
        [HttpGet("list")]
        public ActionResult GetCustomerList(int? nCompanyId,int nFnYearId,int nBranchId,bool bAllBranchesData)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table="vw_InvCustomer_Disp";
            string X_Fields = "[Customer Code] as customerCode,[Customer Name] as customerName,[Contact Person] as contactPerson,Address,N_CompanyID,N_CustomerId,B_Inactive,N_FnYearID,N_BranchID";
            string X_Crieteria = "";
            string X_OrderBy="[Customer Name],[Customer Code]";
            Params.Add("@p1",0);
            Params.Add("@p2",nCompanyId);
            Params.Add("@p3",nFnYearId);

            if (bAllBranchesData == true)
                {X_Crieteria = "B_Inactive=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3";}
                else
                {X_Crieteria = "B_Inactive=@p1 and N_CompanyID=@p2 and N_FnYearID=@p3  and (N_BranchID=@p4 or N_BranchID=@p5)"; 
                Params.Add("@p4",0);
                Params.Add("@p5",nBranchId);}

            try{
                    dt=_dataAccess.Select(X_Table,X_Fields,X_Crieteria,Params,X_OrderBy);
                     if(dt.Rows.Count==0)
                    {
                       return StatusCode(200,new { StatusCode = 200 , Message= "No Results Found" });
                    }else{
                        return Ok(dt);
                    }
                }catch(Exception e){
                    return StatusCode(404,_api.Response(404,e.Message));
                }
        }


[HttpGet("all")]
        public ActionResult GetCustomer(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table="Inv_Customer";
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
                    return StatusCode(404,_api.Response(404,e.Message));
                }
        }
        
       //Save....
       [HttpPost("Save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            try{
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];

                    // Auto Gen
                    //var values = MasterTable.Rows[0]["X_CustomerCode"].ToString();


                    _dataAccess.StartTransaction();
                    int N_QuotationId=_dataAccess.SaveData("Inv_Customer","N_CustomerID",0,MasterTable);                    
                    if(N_QuotationId<=0){
                        _dataAccess.Rollback();
                        return StatusCode(404,_api.Response(404 ,"Unable to save" ));
                        }else{
                    _dataAccess.Commit();
                    return StatusCode(200,_api.Response(200 ,"Customer Saved" ));
                        }
                }
                catch (Exception ex)
                {
                    _dataAccess.Rollback();
                    return StatusCode(403,"Error");
                }
        }

[HttpGet("paymentmethod")]
        public ActionResult GetPayMethod()
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table="Inv_CustomerType";
            string X_Fields = "*";
            string X_Crieteria = "";
            string X_OrderBy="X_TypeName";

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
                return StatusCode(404,_api.Response(404,e.Message));
            }
        }
    }
}