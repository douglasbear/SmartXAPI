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

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("approvalcodes")]
    [ApiController]
    public class Gen_Approvalcodes : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        
        public Gen_Approvalcodes(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1056;
        }   
        private readonly string connectionString;

        
        [HttpGet("usercategorylist")]
        public ActionResult GetUser(int nCompanyId)
        {
            DataTable dt = new DataTable();
            //test
            int abc=0;
            SortedList Params = new SortedList();
            //int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from Sec_UserCategory where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyId);
          

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    connection.Open();
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else { return Ok(_api.Success(dt)); }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }

        [HttpGet("userlist")]
        public ActionResult GetUserlist(int nCompanyId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            //int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from Sec_User where N_CompanyID=@p1";
            Params.Add("@p1", nCompanyId);
                      

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    connection.Open();
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else { return Ok(_api.Success(dt)); }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }
        

        [HttpGet("actionlist")]
        public ActionResult GetActionList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from gen_defaults where n_DefaultId=33 ";
            //Params.Add("@p1", nDefaultId);
            //Params.Add("@p1", nTypeId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    connection.Open();
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else { return Ok(_api.Success(dt)); }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(e));
            }
        }  

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
                    DataTable DetailTable;
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                int nApprovalID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ApprovalID"].ToString());
                int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                string X_ApprovalCode = MasterTable.Rows[0]["X_ApprovalCode"].ToString();

                
                // int nUsercategoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserCategoryID"].ToString());
                // int nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());
                // int nLevelID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_Level"].ToString());
                // int nActionID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ActionTypeID"].ToString());
                if (nApprovalID > 0)
                    {
                        SortedList deleteParams = new SortedList()
                            {
                                {"N_CompanyID",nCompanyID},
                                {"X_ApprovalCode",X_ApprovalCode},
                                {"N_VoucherID",nApprovalID}
                            };
                        dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", deleteParams, connection, transaction);
                    }
                 DocNo = MasterRow["X_ApprovalCode"].ToString();
                 if (X_ApprovalCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("N_YearID", nFnYearID);                       
                        
                      while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Gen_ApprovalCodes Where X_ApprovalCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_ApprovalCode=DocNo;


                        if (X_ApprovalCode == "") { transaction.Rollback();return Ok(_api.Error("Unable to generate Approval Code")); }
                        MasterTable.Rows[0]["X_ApprovalCode"] = X_ApprovalCode;

                    }
                    else
                    {
                        dLayer.DeleteData("Gen_ApprovalCodes", "N_ApprovalID", nApprovalID, "", connection, transaction);
                    }

                    MasterTable.Columns.Remove("N_FnYearID");
                    
                    nApprovalID=dLayer.SaveData("Gen_ApprovalCodes","N_ApprovalID",MasterTable,connection,transaction);
                    if (nApprovalID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    } 

                     for (int i = 0; i < DetailTable.Rows.Count; i++)
                     {
                        DetailTable.Rows[0]["N_ApprovalID"] = nApprovalID;
                     }
                    int N_ApprovalDetailsID = dLayer.SaveData("Gen_ApprovalCodesDetails", "N_ApprovalDetailsID", DetailTable, connection, transaction);


                    transaction.Commit();
                    return Ok(_api.Success("Saved")) ;
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        } 

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nApprovalID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    
                    connection.Open();
                    Results = dLayer.DeleteData("Gen_ApprovalCodes", "N_ApprovalID", nApprovalID, "", connection);
                    if (Results > 0)
                    {
                        return Ok( _api.Success("deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete"));
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
//       