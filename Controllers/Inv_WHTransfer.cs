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
    [Route("Inventorytransfer")]
    [ApiController]
    public class Inv_WHTransfer : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
        
        public Inv_WHTransfer(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
             _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1056;
        }   
        private readonly string connectionString;

         [HttpGet("list")]
        public ActionResult GetLocationDetails(int? nCompanyId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_InvLocation_Disp where N_CompanyID=@p1 order by N_LocationID DESC";
            Params.Add("@p1", nCompanyId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found" ));
                }
                else
                {
                    return Ok(_api.Success(_api.Format(dt)));
                }
            }
            catch (Exception e)
            {
                return Ok( _api.Error(e));
            }
        }


         [HttpGet("productInformation")]
        public ActionResult ProductInfo(int? nCompanyID,int nLocationIDFrom)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nLocationIDFrom", nLocationIDFrom);
            string sqlCommandText = "";

            sqlCommandText = "select * from vw_UC_ItemWithStockQty where N_CompanyID=@nCompanyID and B_Inactive=0 and [Product Code]<>'001' and N_ClassID<>4 and N_ClassID<>5 and N_LocationID=@nLocationIDFrom and B_Inactive=0 "; 

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
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
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
                    int nTransferId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_TransferId"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_FnYearID"].ToString());
                    string X_ReferenceNo = MasterTable.Rows[0]["X_ReferenceNo"].ToString();
                    string X_TransType = "TRANSFER";
                
                // int nUsercategoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserCategoryID"].ToString());
                // int nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_UserID"].ToString());
                // int nLevelID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_Level"].ToString());
                // int nActionID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ActionTypeID"].ToString());
                    if (nTransferId > 0)
                    {
                      SortedList deleteParams = new SortedList()
                            {
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType",X_TransType},
                                {"N_TransferId",nTransferId}
                            };
                        dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", deleteParams, connection, transaction);
                    }
                    DocNo = MasterRow["X_ReferenceNo"].ToString();
                    if (X_ReferenceNo == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", this.FormID);
                        Params.Add("N_YearID", nFnYearID);                       
                        
                      while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_TransferStock Where X_ReferenceNo ='" + DocNo + "' and N_CompanyID= " + nCompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_ReferenceNo=DocNo;


                        if (X_ReferenceNo == "") { transaction.Rollback();return Ok(_api.Error("Unable to generate")); }
                        MasterTable.Rows[0]["X_ReferenceNo"] = X_ReferenceNo;

                    }
                    else
                    {
                        dLayer.DeleteData("Inv_TransferStock", "N_TransferId", nTransferId, "", connection, transaction);
                    }

                    MasterTable.Columns.Remove("N_FnYearID");
                    
                    nTransferId=dLayer.SaveData("Inv_TransferStock","N_TransferId",MasterTable,connection,transaction);
                    if (nTransferId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    } 

                     for (int i = 0; i < DetailTable.Rows.Count; i++)
                     {
                        DetailTable.Rows[0]["N_TransferId"] = nTransferId;
                     }
                    int nTransferDetailsID = dLayer.SaveData("Inv_TransferStockDetails", "N_TransferDetailsID", DetailTable, connection, transaction);
                    if (nTransferDetailsID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable To Save"));
                    } 

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
        public ActionResult DeleteData(int nCompanyID,int nTransferId)
        {
            int Results = 0;
            string xTransType="TRANSFER";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    
                 
                       SortedList deleteParams = new SortedList()
                            {
                                {"N_CompanyID",nCompanyID},
                                {"X_TransType",xTransType},
                                {"N_VoucherID",nTransferId}
                                
                            };
                        Results=dLayer.ExecuteNonQueryPro("SP_Delete_Trans_With_Accounts", deleteParams, connection, transaction);
                    if (Results > 0)
                    {
                        return Ok( _api.Success("Deleted"));
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