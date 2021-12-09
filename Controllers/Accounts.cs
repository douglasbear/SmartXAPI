using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("accounts")]
    [ApiController]
    public class Accounts : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Accounts(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("glaccount/list")]
        public ActionResult GetGLAccountList(int? nFnYearId, string xType, int nCashBahavID,int nGroupID)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            if (nFnYearId == null) { return Ok(_api.Notice("FnYear ID Required"));}

            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            if (nCashBahavID > 0)
            {
                Params.Add("@nCashBahavID", nCashBahavID);

                sqlCommandText = "select [Account Code] as accountCode,Account,N_CompanyID,N_LedgerID,X_Level,N_FnYearID,N_CashBahavID,X_Type from vw_AccMastLedger where N_CompanyID=@p1 and N_FnYearID=@p2 and N_CashBahavID =@nCashBahavID and B_Inactive = 0  order by [Account Code]";
            }
            else if(nGroupID > 0)
            {
               Params.Add("@nGroupID", nGroupID);

                sqlCommandText = "select [Account Code] as accountCode,Account,N_CompanyID,N_LedgerID,X_Level,N_FnYearID,N_CashBahavID,X_Type from vw_AccMastLedger where N_CompanyID=@p1 and N_FnYearID=@p2 and N_GroupID =@nGroupID and B_Inactive = 0  order by [Account Code]";  
            }
            else
            if (xType.ToLower() != "all")
            {
                Params.Add("@p3", xType);
                sqlCommandText = "select [Account Code] as accountCode,Account,N_CompanyID,N_LedgerID,X_Level,N_FnYearID,N_CashBahavID,X_Type from vw_AccMastLedger where N_CompanyID=@p1 and N_FnYearID=@p2 and X_Type =@p3 and B_Inactive = 0  order by [Account Code]";
            }
            else
                sqlCommandText = "select [Account Code] as accountCode,Account,N_CompanyID,N_LedgerID,X_Level,N_FnYearID,N_CashBahavID,X_Type from vw_AccMastLedger where N_CompanyID=@p1 and N_FnYearID=@p2 and B_Inactive = 0  order by [Account Code]";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        [HttpGet("list") ]
        public ActionResult GetAccountList (int nFnYearId, string type,int vendorTypeID)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            if (nFnYearId == 0) { return Ok(_api.Notice("FnYear ID Required")); }

            string criteria=null;
            switch(type.ToLower()){
                case "loan": criteria=" and X_Level like '1%' and B_Inactive=0 ";
                break;
                case "payable": criteria=" and X_Level like '2%' and B_Inactive=0 ";
                break;
                case "vendor":
                    if(vendorTypeID==2)
                        criteria=" and ( X_Type ='A' or  X_Type ='L') ";
                    else
                        criteria=" and ( X_Type ='A' or  X_Type ='L') and (N_CashBahavID=4 or N_CashBahavID=5) ";
                break;
                case "inventory": criteria= " and X_Type ='A'";
                break;
                case "income": criteria= " and X_Type ='I'";
                break;
                case "cost": criteria= " and X_Type ='E'";
                break;
                default: return Ok("Invalid Type");
            }
            string X_Criteria=criteria;
            SortedList Params = new SortedList(){{"@p3",criteria}};
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            
            DataTable dt=new DataTable();
            
            sqlCommandText="select [Account Code] as x_LedgerCode,Account as x_LedgerName,N_CompanyID,N_LedgerID,X_Level,N_FnYearID,N_CashBahavID,X_Type from vw_AccMastLedger where N_CompanyID=@p1 and N_FnYearID=@p2 "+ X_Criteria +" order by [Account Code]";
                
            try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                    // if(dt.Rows.Count==0)
                    //     {
                    //         return Ok(_api.Success(dt));
                    //     }else{
                            return Ok(_api.Success(dt));
                        // }
                
            }catch(Exception e){
                return Ok(_api.Error(User,e));
            }   
        }

        [HttpGet("defaultAccounts")]
        public ActionResult GetDefultAccounts(int nFnYearID, int nLangID, int AccountTypeID)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    DataTable QList = myFunctions.GetSettingsTable();

                    QList.Rows.Add("DEFAULT_ACCOUNTS", "V " + AccountTypeID);

                    QList.AcceptChanges();

                    DataTable Details = dLayer.ExecuteSettingsPro("SP_GenSettings_Disp", QList, myFunctions.GetCompanyID(User), nFnYearID, connection);


                    SortedList OutPut = new SortedList(){
                            {"DefaultAccounts",_api.Format(Details)}
                        };
                    return Ok(_api.Success(OutPut));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }

        // [HttpPost("Save")]
        // public ActionResult SaveData([FromBody] DataSet ds)
        // {
        //     try
        //     {
        //         DataTable MasterTable;
        //         MasterTable = ds.Tables["master"];

        //         SortedList Params = new SortedList();

        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();
        //             int N_ItemUnitID = dLayer.SaveData("Inv_ItemUnit", "N_ItemUnitID", MasterTable, connection, transaction);
        //             if (N_ItemUnitID <= 0)
        //             {
        //                 transaction.Rollback();
        //                 return Ok( api.Warning("Unable to save ItemUnit"));
        //             }
        //             else
        //             {
        //                 transaction.Commit();
        //             }
        //             return GetItemUnitListDetails(int.Parse(MasterTable.Rows[0]["n_CompanyId"].ToString()), N_ItemUnitID);
        //         }
                

        //     }

        //     catch (Exception ex)
        //     {
        //         return Ok(api.Error(User,ex));
        //     }
        // }

        [HttpGet("balance") ]
        public ActionResult GetBalance (int nFnYearId, string xLedgerCode)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "";
            if (nFnYearId == 0) { return Ok(_api.Notice("FnYear ID Required")); }
            
            SortedList Params = new SortedList();
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
            Params.Add("@p3", xLedgerCode);
            
            DataTable dt=new DataTable();
            
            sqlCommandText="Select isnull(Sum(Acc_VoucherDetails.N_Amount),0) as X_Balance,Acc_MastLedger.X_LedgerCode,isnull(Acc_MastLedger.X_LedgerName,'') as X_LedgerName from Acc_MastLedger left outer  join Acc_VoucherDetails on Acc_VoucherDetails.N_LedgerID = Acc_MastLedger.N_LedgerID and Acc_VoucherDetails.N_CompanyID=Acc_MastLedger.N_CompanyID and Acc_MastLedger.N_FnYearID=Acc_VoucherDetails.N_FnYearID Where Acc_MastLedger.N_CompanyID=@p1 and Acc_MastLedger.X_LedgerCode=@p3 AND Acc_MastLedger.N_FnYearID=@p2 group by Acc_MastLedger.X_LedgerName,Acc_MastLedger.X_LedgerCode";
                
            try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                    
                    return Ok(_api.Success(dt));
                
            }catch(Exception e){
                return Ok(_api.Error(User,e));
            }   
        }

    }
}