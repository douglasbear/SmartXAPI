using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("paymenttype")]
    [ApiController]
    
    
    
    public class paymentTypeController : ControllerBase
    {
         private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;

        public paymentTypeController(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 898;

        }

       
        

               [HttpGet("details")]
        public ActionResult GetPaymentTypeDetails(int nTypeID)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText="select * from Inv_Customer where N_CompanyId=@nCompanyId and N_TypeID=@nTypeID";
            Params.Add("@nCompanyId",nCompanyId);
            Params.Add("@nTypeID",nTypeID);
            try{
                        using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();
                                dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection); 
                            }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(_api.Notice("No Results Found" ));
                        }else{
                            return Ok(_api.Success(dt));
                        }
            }catch(Exception e){
                return Ok(_api.Error(e));
            }
          
        }

          //Save....
       [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    MasterTable = ds.Tables["master"];
                    SortedList Params = new SortedList();
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nTypeID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TypeID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                int nCustomerID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CustomerId"].ToString());               
                string CustomerCode = MasterTable.Rows[0]["X_CustomerCode"].ToString();
             
                MasterTable.AcceptChanges();
                

                 if (CustomerCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", 51);
                        Params.Add("N_BranchID", 1);
                       
                        CustomerCode = dLayer.GetAutoNumber("Inv_Customer", "X_CustomerCode", Params, connection, transaction);
                        if (CustomerCode == "") { transaction.Rollback(); return Ok(_api.Error("Unable to generate Customer Code")); }
                        MasterTable.Rows[0]["X_CustomerCode"] = CustomerCode;
                    }
                    string DupCriteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID + " and X_CustomerCode='" + CustomerCode + "'";
                    string X_Criteria = "N_CompanyID=" + nCompanyID + " and N_FnYearID=" + nFnYearID;
                    nCustomerID = dLayer.SaveData("Inv_Customer", "n_CustomerID", DupCriteria,X_Criteria,MasterTable, connection, transaction);
                    if (nCustomerID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        // return GetCustomerList(nCompanyID, nFnYearId, nBranchId, true, nCustomerID.ToString(), "");
                        return Ok(_api.Success("Customer Saved") );
                    }
                
                
                }
                
            }
                   
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }
       
    }
}