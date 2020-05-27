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
        public Inv_SalesQuotationController(IInv_SalesQuotationRepo repository, IMapper mapper,IConfiguration config,IDataAccessLayer dataaccess)
        {
            _repository = repository;
            _mapper = mapper;
            _config = config;
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
                    DataTable table;
                    table = ds.Tables["table"];
                    _dataAccess.StartTransaction();
                    int res=_dataAccess.SaveData("Inv_SalesQuotation","N_QuotationId",0,table);                    
                    return Ok(res);
                }
                catch (Exception ex)
                {
                    
                    return StatusCode(403,ex);
                }
        }


        [HttpGet("get")]
        public ActionResult GetData()
        {
             
            SqlConnection _Con = new SqlConnection( _config.GetConnectionString("SmartxConnection"));  
            try{
                string sql = "select top (1) SELECT  N_CompanyId, N_FnYearId,0, X_QuotationNo, D_QuotationDate, D_EntryDate, N_CustomerId, N_BillAmt, N_DiscountAmt, N_FreightAmt, N_CashReceived, x_Notes, N_UserID, N_Processed, N_LocationID, N_BranchId, N_SalesmanID, N_ProjectID, N_SalesID, X_CustomerName, N_CRMID, B_Revised, X_ActualQuotationNo, X_RfqRefNo, N_FreightAmtF, N_ValidDays, D_QuotationExpiry, D_RfqRefDate, X_TandC, X_RequestedBy, N_TaxAmt, X_Subject, N_OthTaxAmt, N_OthTaxCategoryID, N_OthTaxPercentage, D_DelDate, X_Location FROM            Inv_SalesQuotation from Inv_SalesQuotation";
    _Con.Open();
    SqlDataAdapter da = new SqlDataAdapter();
    SqlCommand cmd = _Con.CreateCommand();
    cmd.CommandText = sql;
    da.SelectCommand = cmd;
    DataSet ds = new DataSet();
    DataTable table;
table = ds.Tables["table"];

string Res="";
string FieldList="";
string FieldValues="";
for (int i = 0; i < table.Columns.Count; i++)
{
    FieldList = FieldList +","+ table.Columns[i].ColumnName.ToString();
}
for (int j = 0 ;j < table.Rows.Count;j++)
{
    for (int k = 0; k < table.Columns.Count; k++)
    {
        var value= table.Rows[j][k].ToString();
        if(value==""){value="''";}
        FieldValues = FieldValues +"|"+value ;
    }
        SqlDataReader rdr  = null;
    	SqlCommand cmds  = new SqlCommand("SAVE_DATA", _Con);

			cmds.CommandType = CommandType.StoredProcedure;

			cmds.Parameters.Add(new SqlParameter("@X_TableName", "Inv_SalesQuotation"));
            cmds.Parameters.Add(new SqlParameter("@X_IDFieldName", "N_QuotationId"));
            cmds.Parameters.Add(new SqlParameter("@N_IDFieldValue", "0"));
            cmds.Parameters.Add(new SqlParameter("@X_FieldList", FieldList));
            cmds.Parameters.Add(new SqlParameter("@X_FieldValue", FieldValues));

			rdr = cmds.ExecuteReader();

			while (rdr.Read())
			{
				Res=rdr.ToString();
			}

    FieldValues="";
}
_Con.Close();
                    return Ok(Res);
            }catch(Exception e){
                return BadRequest(e);
            }
        }

   
        

        
    }
}