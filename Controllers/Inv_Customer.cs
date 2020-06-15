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
        private readonly IDataAccessLayer _dataAccess;
        
        public Inv_Customer(IApiFunctions api,IDataAccessLayer dataAccess)
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
                    return StatusCode(403,_api.ErrorResponse(e));
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
                    _dataAccess.GerateReport();
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
                    string CustomerCode="";
                    var values = MasterTable.Rows[0]["X_CustomerCode"].ToString();
                    if(values=="@Auto"){
                        Params.Add("N_CompanyID",MasterTable.Rows[0]["n_CompanyId"].ToString());
                        Params.Add("N_YearID",MasterTable.Rows[0]["n_FnYearId"].ToString());
                        Params.Add("N_FormID",51);
                        Params.Add("N_BranchID",MasterTable.Rows[0]["n_BranchId"].ToString());
                        CustomerCode =  _dataAccess.GetAutoNumber("Inv_Customer","X_CustomerCode", Params);
                        if(CustomerCode==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Customer Code" ));}
                        MasterTable.Rows[0]["X_CustomerCode"] = CustomerCode;
                    }


                    int N_CustomerID=_dataAccess.SaveData("Inv_Customer","N_CustomerID",0,MasterTable);                    
                    if(N_CustomerID<=0){
                        _dataAccess.Rollback();
                        return StatusCode(404,_api.Response(404 ,"Unable to save" ));
                        }else{
                    _dataAccess.Commit();
                    return  GetCustomerDetails(N_CustomerID, int.Parse(MasterTable.Rows[0]["n_CompanyId"].ToString()), int.Parse(MasterTable.Rows[0]["n_FnYearId"].ToString()));
                        }
                }
                catch (Exception ex)
                {
                    _dataAccess.Rollback();
                    return StatusCode(403,_api.ErrorResponse(ex));
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
                return StatusCode(403,_api.ErrorResponse(e));
            }
        }
        [HttpGet("getdetails")]
        public ActionResult GetCustomerDetails(int nCustomerID,int nCompanyID,int nFnyearID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string X_Table = "Inv_Customer";
            string X_Fields = "*";
            string X_Crieteria = "N_CompanyID=@p1 and N_FnYearID=@p2 and N_CustomerID=@p3";
            string X_OrderBy = "";
            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nFnyearID);
            Params.Add("@p3", nCustomerID);

            try
            {
                dt = _dataAccess.Select(X_Table, X_Fields, X_Crieteria, Params, X_OrderBy);
                foreach (DataColumn c in dt.Columns)
                    c.ColumnName = String.Join("", c.ColumnName.Split());
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
                Results=_dataAccess.DeleteData("Inv_Customer","N_CustomerID",nCustomerId,"");
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