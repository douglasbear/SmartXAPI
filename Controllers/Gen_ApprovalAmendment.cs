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
    [Route("approvalAmendment")]
    [ApiController]
    public class Gen_ApprovalAmendment : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1811 ;

        public Gen_ApprovalAmendment(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        [HttpGet("transType")]
        public ActionResult TransTypeList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            
           
            string sqlCommandText = "select X_Type from vw_ActiveApprovals where N_CompanyID=@p1 GROUP BY X_Type";
            Params.Add("@p1", nCompanyId);
           
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                    dt = api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(dt));
                    }

                }
                
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("actionUser")]
        public ActionResult ActionUserList(string xTransType)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            
           
            string sqlCommandText = "select X_UserName,N_UserID from vw_ActiveApprovals where N_CompanyID=@p1 and X_Type=@p2 GROUP BY X_UserName,N_UserID";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", xTransType);
           
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                    dt = api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(dt));
                    }

                }
                
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("transNo")]
        public ActionResult TransNoList(string xTransType,int nUserID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            
           
            string sqlCommandText = "select X_TransCode from vw_ActiveApprovals where N_CompanyID=@p1 and X_Type=@p2 and N_UserID=@p3 GROUP BY X_TransCode";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", xTransType);
            Params.Add("@p3", nUserID);
           
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                    dt = api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(dt));
                    }

                }
                
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("approvalPending")]
        public ActionResult PendingApprovalList(string xTransType,int nUserID,string xTransCode)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string xCond ="";
            if(xTransCode==null)xTransCode="";
            if(xTransCode!="")
            {
                xCond=" and X_TransCode=@p4";
                Params.Add("@p4", xTransCode);
            }
            string sqlCommandText = "select * from vw_ActiveApprovals where N_CompanyID=@p1 and X_Type=@p2 and N_UserID=@p3 "+xCond+"";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", xTransType);
            Params.Add("@p3", nUserID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                    dt = api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(dt));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
        
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    DataTable DetailTable;
                    MasterTable = ds.Tables["master"];
                    DetailTable = ds.Tables["details"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyID"].ToString());
                    int nAmendID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_AmendID"].ToString());
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    int nActionTypeID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ActionTypeID"].ToString());
                    string X_AmendCode = MasterTable.Rows[0]["X_AmendCode"].ToString();
                    string xType = MasterTable.Rows[0]["x_TransType"].ToString();
                    string x_AmendCode = "";
                    string xButtonAction = "";

                    if (nAmendID > 0)
                    {
                        dLayer.DeleteData("Gen_ApprovalAmendDetails", "N_AmendID", nAmendID, "N_CompanyID = " + nCompanyID, connection, transaction);
                        dLayer.DeleteData("Gen_ApprovalAmend", "N_AmendID", nAmendID, "N_CompanyID = " + nCompanyID, connection, transaction);
                        xButtonAction = "Update";
                    }

                    if (X_AmendCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", MasterTable.Rows[0]["N_CompanyID"].ToString());
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", 1811);
                        x_AmendCode = dLayer.GetAutoNumber("Gen_ApprovalAmend", "X_AmendCode", Params, connection, transaction);
                        xButtonAction = "update";

                        if (x_AmendCode == "")
                        {
                            transaction.Rollback();
                            return Ok(api.Error(User, "Unable to generate Amend Code"));
                        }

                        MasterTable.Rows[0]["X_AmendCode"] = x_AmendCode;
                    }
                    X_AmendCode = MasterTable.Rows[0]["x_AmendCode"].ToString();
                    MasterTable.Columns.Remove("N_FnYearID");

                    nAmendID = dLayer.SaveData("Gen_ApprovalAmend", "N_AmendID", MasterTable, connection, transaction);
                    if (nAmendID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User, "Unable To Save"));
                    }

                    for (int j = 0; j < DetailTable.Rows.Count; j++)
                    {
                        DetailTable.Rows[j]["N_AmendID"] = nAmendID;
                        DetailTable.Rows[j]["X_TransType"] = xType;
                    }

                    if (DetailTable.Rows.Count > 0)
                    {
                        int nAmendDetailsID = dLayer.SaveData("Gen_ApprovalAmendDetails", "N_AmendDetailsID", DetailTable, connection, transaction);
                        if (nAmendDetailsID <= 0)
                        {
                            transaction.Rollback();
                            return Ok("Unable To Save");
                        }

                        
                        for (int i = 0; i < DetailTable.Rows.Count; i++)
                        {
                            int N_Status=1;
                            DetailTable.Rows[i]["N_AmendID"] = nAmendID;
                            DetailTable.Rows[i]["X_TransType"] = xType;

                            if(nActionTypeID==198)
                            {
                                object objStatus = dLayer.ExecuteScalar("SELECT N_Status FROM Gen_ApprovalCodesTrans WHERE N_CompanyID="+myFunctions.getIntVAL(DetailTable.Rows[i]["N_CompanyID"].ToString())+" AND N_FormID="+myFunctions.getIntVAL(DetailTable.Rows[i]["N_FormID"].ToString())+" AND N_HierarchyID="+myFunctions.getIntVAL(DetailTable.Rows[i]["N_HierarchyID"].ToString())+" AND N_TransID="+myFunctions.getIntVAL(DetailTable.Rows[i]["N_TransID"].ToString()), Params, connection,transaction);
                                if (objStatus != null)
                                    N_Status=myFunctions.getIntVAL(objStatus.ToString());
                            }
                        }

                        string ipAddress = "";
                        if (Request.Headers.ContainsKey("X-Forwarded-For"))
                            ipAddress = Request.Headers["X-Forwarded-For"];
                        else
                            ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                        myFunctions.LogScreenActivitys(nFnYearID, nAmendID, X_AmendCode, 1811, xButtonAction, ipAddress, "", User, dLayer, connection, transaction);
                    }

                    transaction.Commit();
                    return Ok(api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nAmendID, int nCompanyID)
        {
            int Results = 0;
            // nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


        //           object N_ServiceInfoID = dLayer.ExecuteScalar("select (N_ServiceInfoID) from Inv_ServiceInfo  Where N_CompanyID=" + nCompanyID + " and N_DeviceID="+nDeviceID, connection);
        //           if(N_ServiceInfoID!=null){
        //                            object serialNoCount = dLayer.ExecuteScalar("select count(N_ServiceID) from Inv_SalesOrderDetails  Where N_CompanyID=" + nCompanyID + " and N_ServiceID="+N_ServiceInfoID, connection);

        //               if( myFunctions.getIntVAL(serialNoCount.ToString())>0){

        //                      return Ok(api.Error(User, "Unable To Delete!"));
        //                  }

        //           }

                    Results = dLayer.DeleteData("Gen_ApprovalAmend", "N_AmendID", nAmendID, "N_CompanyID=" + nCompanyID + "", connection);
                    dLayer.DeleteData("Gen_ApprovalAmendDetails", "N_AmendID", nAmendID, "N_CompanyID=" + nCompanyID + "", connection);

                }
                if (Results > 0)
                {
                    return Ok(api.Success("Amendment deleted"));
                }
                else
                {
                    return Ok(api.Error(User, "Unable to delete"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User, ex));
            }

        }

        [HttpGet("details")]
        public ActionResult ApprovalAmendmentDetails(string xAmendCode)
        {
            DataSet dt=new DataSet();
            DataTable MasterTable = new DataTable();
            DataTable DetailTable = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            string sqlCommandText = "select * from VW_Gen_ApprovalAmendMaster where N_CompanyID=@p1 and X_AmendCode=@p2";
            Params.Add("@p1", nCompanyId);  
            Params.Add("@p2", xAmendCode);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);

                    if (MasterTable.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                
                    MasterTable = api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);

                    int N_AmendID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_AmendID"].ToString());
                    Params.Add("@p3", N_AmendID);

                    string DetailSql = "select * from VW_Gen_ApprovalAmendDetails where N_CompanyID=@p1 and N_AmendID=@p3";

                    DetailTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    DetailTable = api.Format(DetailTable, "Details");
                    dt.Tables.Add(DetailTable);


                     int N_AmendDetailsID = myFunctions.getIntVAL(DetailTable.Rows[0]["N_AmendDetailsID"].ToString());

                }
                return Ok(api.Success(dt));               
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("list")]
        public ActionResult ApprovalAmendmentList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);
            
           
            string sqlCommandText = "select * from VW_Gen_ApprovalAmend_List_Cloud where N_CompanyID=@p1 order by X_AmendCode desc";
            Params.Add("@p1", nCompanyId);
           
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params,connection);
                    dt = api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(dt));
                    }

                }
                
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }

        




    }
}
