using AutoMapper;
using SmartxAPI.Data;
using SmartxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Linq;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;


namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("salesquotation")]
    [ApiController]
    public class Inv_SalesQuotationController : ControllerBase
    {
        private readonly IInv_SalesQuotationRepo _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IDataAccessLayer _dataAccess;

        
        public Inv_SalesQuotationController(IInv_SalesQuotationRepo repository, IMapper mapper,IDataAccessLayer dataaccess)
        {
            _repository = repository;
            _mapper = mapper;
            _dataAccess=dataaccess;
        }
       

        [HttpGet("list")]
        public ActionResult <VwInvSalesQuotationNoSearch> GetSalesQuotationList(int? nCompanyId,int nFnYearId)
        {
            try{
                    var QuotationList = _repository.GetSalesQuotationList(nCompanyId,nFnYearId);
                    if(!QuotationList.Any())
                    {
                       return NotFound("No Results Found");
                    }else{
                        return Ok(QuotationList);
                    }
            }catch(Exception e){
                return BadRequest(e);
            }
        }


       //POST salesquotation/User
       [HttpPost("new")]
        public ActionResult Authenticate([FromBody]DataSet ds)
        { 
            try{
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    _dataAccess.StartTransaction();
                    int N_QuotationId=_dataAccess.SaveData("Inv_SalesQuotation","N_QuotationId",0,MasterTable);                    
                    if(N_QuotationId<=0){
                        _dataAccess.Rollback();
                        }
                    for (int j = 0 ;j < DetailTable.Rows.Count;j++)
                        {
                            DetailTable.Rows[j]["n_QuotationID"]=N_QuotationId;
                        }
                    int N_QuotationDetailId=_dataAccess.SaveData("Inv_SalesQuotationDetails","n_QuotationDetailsID",0,DetailTable);                    
                    _dataAccess.Commit();
                    return Ok("DataSaved");
                }
                catch (Exception ex)
                {
                    _dataAccess.Rollback();
                    return StatusCode(403,ex);
                }
        }


        [HttpGet("get")]
        public ActionResult GetData()
        {
             
            SqlConnection _Con = new SqlConnection( _config.GetConnectionString("SmartxConnection"));  
            try{
                string sql = "select top (1) * from Inv_SalesQuotationDetails";
                _Con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = _Con.CreateCommand();
                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);
                _Con.Close();
                return Ok(ds);
            }catch(Exception e){
                return BadRequest(e);
            }
        }

   
        

        
    }
}