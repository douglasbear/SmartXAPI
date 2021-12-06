using AutoMapper;
using SmartxAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("accountbehaviour")]
    [ApiController]
    public class Acc_AccountBehaviour : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly IApiFunctions _api;
        private readonly int FormID;

        public Acc_AccountBehaviour(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 151;
        }

        [HttpGet("behaviourList")]
        public ActionResult GetBehaviourList(int nType)
        {
            DataTable dt = new DataTable();
            SortedList Params=new SortedList();
            string sqlCommandText = "select * from Acc_LedgerBehaviour where N_Type=@p1 order by N_LedgerBehaviourID";
            Params.Add("@p1",nType);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("accountList")]
        public ActionResult GetAccountList(int nFnYearID,int nGroupID)
        {
            DataTable dt = new DataTable();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCondition="";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    bool B_PostedBehaviour = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Posting", "Electronic Account", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    if (nGroupID != 0)
                        if(B_PostedBehaviour==false)
                            sqlCondition = "N_CompanyID=@p1 and N_FnYearID=@p2 and N_PostingBahavID=11 and  B_Inactive=0 and N_GroupID=@p3";
                        else
                        sqlCondition = "N_CompanyID=@p1 and N_FnYearID=@p2 and B_Inactive=0 and N_GroupID=@p3";
                    else
                        if(B_PostedBehaviour==false)
                            sqlCondition = "N_CompanyID=@p1 and N_PostingBahavID=11 and N_FnYearID=@p2 and B_Inactive=0";
                        else
                            sqlCondition = "N_CompanyID=@p1 and N_FnYearID=@p2 and B_Inactive=0";

                    string sqlCommandText = "select * from vw_AccMastLedger where "+ sqlCondition +" order by N_LedgerID";
                    Params.Add("@p1",nCompanyID);
                    Params.Add("@p2",nFnYearID);
                    Params.Add("@p3",nGroupID);

            
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("details")]
        public ActionResult GetAccBehaviourDetails(int nFnYearID,int nGroupID)
        {
            DataTable dt=new DataTable();
            DataTable DetailTable;
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText="";
           
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nFnYearID);
            Params.Add("@p3",nGroupID);

            if(nGroupID !=0)
                sqlCommandText="SELECT Acc_MastLedger.*, ISNULL(Acc_LedgerBehaviour.X_Description,'') AS X_CashBehaviour, ISNULL(Acc_LedgerBehaviour_1.X_Description,'') AS X_TransBehaviour, ISNULL(Acc_LedgerBehaviour_2.X_Description,'') AS X_LedgerBehaviour "+
                                " FROM Acc_MastLedger LEFT OUTER JOIN Acc_LedgerBehaviour AS Acc_LedgerBehaviour_2 ON Acc_MastLedger.N_LedgerBehavID = Acc_LedgerBehaviour_2.N_LedgerBehaviourID LEFT OUTER JOIN Acc_LedgerBehaviour AS Acc_LedgerBehaviour_1 ON Acc_MastLedger.N_TransBehavID = Acc_LedgerBehaviour_1.N_LedgerBehaviourID "+
                                " LEFT OUTER JOIN Acc_LedgerBehaviour ON Acc_MastLedger.N_CashBahavID = Acc_LedgerBehaviour.N_LedgerBehaviourID "+
                                " WHERE (ISNULL(Acc_MastLedger.X_CashTypeBehaviour, '') <> '') OR (ISNULL(Acc_MastLedger.N_TransBehavID, 0) <> 0) and  Acc_MastLedger.N_CompanyID =@p1 and Acc_MastLedger.N_FnYearID=@p2 and Acc_MastLedger.N_GroupID=@p3"+
                                " ORDER BY Acc_MastLedger.X_LedgerName";
            else
                sqlCommandText="SELECT Acc_MastLedger.*, ISNULL(Acc_LedgerBehaviour.X_Description,'') AS X_CashBehaviour, ISNULL(Acc_LedgerBehaviour_1.X_Description,'') AS X_TransBehaviour, ISNULL(Acc_LedgerBehaviour_2.X_Description,'') AS X_LedgerBehaviour "+
                                " FROM Acc_MastLedger LEFT OUTER JOIN Acc_LedgerBehaviour AS Acc_LedgerBehaviour_2 ON Acc_MastLedger.N_LedgerBehavID = Acc_LedgerBehaviour_2.N_LedgerBehaviourID LEFT OUTER JOIN Acc_LedgerBehaviour AS Acc_LedgerBehaviour_1 ON Acc_MastLedger.N_TransBehavID = Acc_LedgerBehaviour_1.N_LedgerBehaviourID "+
                                " LEFT OUTER JOIN Acc_LedgerBehaviour ON Acc_MastLedger.N_CashBahavID = Acc_LedgerBehaviour.N_LedgerBehaviourID "+
                                " WHERE (ISNULL(Acc_MastLedger.X_CashTypeBehaviour, '') <> '') OR (ISNULL(Acc_MastLedger.N_TransBehavID, 0) <> 0) and  Acc_MastLedger.N_CompanyID =@p1 and Acc_MastLedger.N_FnYearID=@p2"+
                                " ORDER BY Acc_MastLedger.X_LedgerName";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection); 
                }
                if(dt.Rows.Count==0)
                {
                    return Ok(_api.Notice("No Results Found" ));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }catch(Exception e){
                return Ok(_api.Error(User,e));
            }
        }

        [HttpPost("save")]
        public ActionResult SaveData([FromBody]DataSet ds)
        { 
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    object Result = 0;
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable DetailTable;
                    DetailTable = ds.Tables["details"];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(DetailTable.Rows[0]["n_CompanyID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(DetailTable.Rows[0]["n_FnYearID"].ToString());

                    Params.Add("@p1", nCompanyID);
                    Params.Add("@p2", nFnYearID);

                    for (int i = 1; i <= DetailTable.Rows.Count; i++)
                    {
                        int nLedgerID = myFunctions.getIntVAL(DetailTable.Rows[i-1]["n_LedgerID"].ToString());
                        int nCashBahavID = myFunctions.getIntVAL(DetailTable.Rows[i-1]["n_CashBahavID"].ToString());
                        int nTransBehavID = myFunctions.getIntVAL(DetailTable.Rows[i-1]["n_TransBehavID"].ToString());
                        string xCashTypeBehaviour = DetailTable.Rows[i-1]["x_CashTypeBehaviour"].ToString();
                        string isDeleted = DetailTable.Rows[i-1]["isDeleted"].ToString();

                        if (isDeleted == "False")
                        {
                            dLayer.ExecuteNonQuery("Update Acc_MastLedger Set X_CashTypeBehaviour = '',N_CashBahavID=0,N_TransBehavID=0 Where N_LedgerID= " + nLedgerID + " And N_CompanyID = @p1 and N_FnYearID = @p2", Params, connection, transaction);
                        }
                        else
                        {
                            dLayer.ExecuteNonQuery("Update Acc_MastLedger Set X_CashTypeBehaviour = '" + xCashTypeBehaviour + "',N_CashBahavID=" + nCashBahavID + ",N_TransBehavID=" + nTransBehavID + " where N_LedgerID= " + nLedgerID + " and N_CompanyID = @p1 and N_FnYearID = @p2", Params, connection, transaction);
                        }
                        if(DetailTable.Columns.Contains("isDeleted"))
                        DetailTable.Columns.Remove("isDeleted");
                    }
                    if (DetailTable.Rows.Count < 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else {
                        transaction.Commit();
                        return Ok(_api.Success("Account Behaviour Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }
     

        }
    }
       

    

