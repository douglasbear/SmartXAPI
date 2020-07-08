
using Microsoft.AspNetCore.Mvc;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("company")]
    [ApiController]
    public class Acc_CompanyController : ControllerBase
    { 
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;

        public Acc_CompanyController(IApiFunctions api,IDataAccessLayer dl)
        {
            _api=api;
            dLayer=dl;
        }
       
        //GET api/Company/list
        [AllowAnonymous]
        [HttpGet("list")]
        public ActionResult GetAllCompanys()
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select N_CompanyId as nCompanyId,X_CompanyName as xCompanyName,X_CompanyCode as xCompanyCode from Acc_Company where B_Inactive =@p1 order by X_CompanyName";
            Params.Add("@p1",0);
            try{
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params);
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

       [HttpGet("branchlist")]
        public ActionResult GetAllBranches(int? nCompanyId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from Acc_BranchMaster where N_CompanyId=@p1";
            Params.Add("@p1",nCompanyId);
            try{
                    //await Task.Run(() => { 
                        dt=dLayer.ExecuteDataTable(sqlCommandText,Params); 
                        //});
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
                    dLayer.setTransaction();
                    // Auto Gen
                    //var values = MasterTable.Rows[0]["X_CompanyCode"].ToString();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    object CompanyCode="";
                    var values = MasterTable.Rows[0]["X_CompanyCode"].ToString();
                    if(values=="@Auto"){
                        CompanyCode = dLayer.ExecuteScalar("Select ISNULL(MAX(N_CompanyID),0) + 100 from Acc_Company");//Need Auto Genetaion here
                        if(CompanyCode.ToString()==""){return StatusCode(409,_api.Response(409 ,"Unable to generate Company Code" ));}
                        MasterTable.Rows[0]["X_CompanyCode"] = CompanyCode;
                    }
                    int N_CompanyId=dLayer.SaveData("Acc_Company","N_CompanyID",0,MasterTable);                    
                    if(N_CompanyId<=0){
                        dLayer.rollBack();
                        return StatusCode(404,_api.Response(404 ,"Unable to save" ));
                        }else{
                            SortedList proParams1 =new SortedList(){
                                {"N_CompanyID",N_CompanyId},  
                                {"X_ModuleCode","500"},
                                {"N_UserID",GeneralTable.Rows[0]["N_UserID"].ToString()},
                                {"X_AdminName",GeneralTable.Rows[0]["X_AdminName"].ToString()},
                                {"X_AdminPwd",GeneralTable.Rows[0]["X_AdminPwd"].ToString()},
                                {"X_Currency",MasterTable.Rows[0]["X_Currency"].ToString()}};
                            dLayer.ExecuteNonQueryPro("SP_NewAdminCreation",proParams1);

                            object N_FnYearId = 0;
                            
                            SortedList proParams2 =new SortedList(){
                                {"N_CompanyID",N_CompanyId},  
                                {"X_ModuleCode",N_FnYearId},
                                {"N_UserID",GeneralTable.Rows[0]["D_FromDate"].ToString()},
                                {"X_AdminName",GeneralTable.Rows[0]["D_ToDate"].ToString()}};
                            N_FnYearId = dLayer.ExecuteScalarPro("SP_FinancialYear_Create",proParams2);

                            SortedList proParams3 =new SortedList(){
                                {"N_CompanyID",N_CompanyId},  
                                {"X_ModuleCode",N_FnYearId}};
                            dLayer.ExecuteNonQueryPro("SP_AccGruops_Accounts_Create",proParams3);

                    dLayer.commit();
                    return StatusCode(200,_api.Response(200 ,"Company created successfully" ));
                        }
                }
                catch (Exception ex)
                {
                    dLayer.rollBack();
                    return StatusCode(403,_api.ErrorResponse(ex));
                }
        }
       
    }
}