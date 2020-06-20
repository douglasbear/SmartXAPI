using System.Collections.Generic;
using AutoMapper;
using SmartxAPI.Data;
using SmartxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using SmartxAPI.Dtos;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using System;

namespace SmartxAPI.Controllers
{
    [Route("company")]
    [ApiController]
    public class Acc_CompanyController : ControllerBase
    {
        private readonly IDataAccessLayer _dataAccess; 
        private readonly IApiFunctions _api;

        public Acc_CompanyController(IDataAccessLayer data,IApiFunctions api)
        {
            _dataAccess=data;
            _api=api;
        }
       
        //GET api/Company/list
        [HttpGet("list")]
        public ActionResult GetAllCompanys()
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string X_Table="Acc_Company";
            string X_Fields = "N_CompanyId as nCompanyId,X_CompanyName as xCompanyName,X_CompanyCode as xCompanyCode";
            string X_Crieteria = "B_Inactive =@p1";
            string X_OrderBy="X_CompanyName";
            Params.Add("@p1",0);
            try{
                    dt=_dataAccess.Select(X_Table,X_Fields,X_Crieteria,Params,X_OrderBy);
                    if(dt.Rows.Count==0)
                        {
                            return StatusCode(200,_api.Response(200 ,"No Results Found" ));
                        }else{
                            return Ok(dt);
                        }
                
            }catch(Exception e){
                return StatusCode(404,_api.ErrorResponse(e));
            }
          
        }
        [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            try{
                    DataTable MasterTable;
                    DataTable GeneralTable;
                    MasterTable = ds.Tables["master"];
                    GeneralTable = ds.Tables["general"];
                    _dataAccess.StartTransaction();
                    // Auto Gen
                    //var values = MasterTable.Rows[0]["X_CompanyCode"].ToString();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    object CompanyCode="";
                    var values = MasterTable.Rows[0]["X_CompanyCode"].ToString();
                    if(values=="@Auto"){
                        CompanyCode = _dataAccess.ExecuteScalar("Select ISNULL(MAX(N_CompanyID),0) + 100 from Acc_Company");//Need Auto Genetaion here
                        if(CompanyCode.ToString()==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Company Code" ));}
                        MasterTable.Rows[0]["X_CompanyCode"] = CompanyCode;
                    }
                    int N_CompanyId=_dataAccess.SaveData("Acc_Company","N_CompanyID",0,MasterTable);                    
                    if(N_CompanyId<=0){
                        _dataAccess.Rollback();
                        return StatusCode(404,_api.Response(404 ,"Unable to save" ));
                        }else{
                            _dataAccess.ExecuteProcedure("SP_NewAdminCreation",N_CompanyId+",500,"+GeneralTable.Rows[0]["N_UserID"].ToString()+","+GeneralTable.Rows[0]["X_AdminName"].ToString()+","+GeneralTable.Rows[0]["X_AdminPwd"].ToString()+","+MasterTable.Rows[0]["X_Currency"].ToString());

                            object N_FnYearId = 0;

                            N_FnYearId = _dataAccess.ExecuteProcedure("SP_NewAdminCreation",N_CompanyId+","+N_FnYearId+","+GeneralTable.Rows[0]["D_FromDate"].ToString()+","+GeneralTable.Rows[0]["D_ToDate"].ToString());
                            _dataAccess.ExecuteProcedure("SP_AccGruops_Accounts_Create",N_CompanyId+""+N_FnYearId);

                    _dataAccess.Commit();
                    return StatusCode(200,_api.Response(200 ,"Company created successfully" ));
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