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

        // [HttpPost("Save")]
        // public ActionResult SaveData([FromBody] DataSet ds)
        // {
        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();
        //             SqlTransaction transaction = connection.BeginTransaction();



        //             DataTable Master = ds.Tables["master"];
        //             DataTable Details = ds.Tables["details"];
        //             DataTable MasterTable;
        //             MasterTable = ds.Tables["master"];
        //              string X_AmendCode = "";
        //             SortedList Params = new SortedList(){
        //                 {"N_UserID",myFunctions.GetUserID(User)},
        //             };
        //             DataRow MasterRow = Master.Rows[0];
        //             // DataRow MasterRow = MasterTable.Rows[0];
        //             string x_AmendCode = MasterTable.Rows[0]["x_AmendCode"].ToString();
        //             int N_AmendID = myFunctions.getIntVAL(MasterRow["n_AmendID"].ToString());
        // //             int nFnYearID = myFunctions.getIntVAL(MasterRow["n_FnYearID"].ToString());
        //             int nCompanyID = myFunctions.getIntVAL(MasterRow["n_CompanyID"].ToString());
        //             string xType = MasterTable.Rows[0]["x_TransType"].ToString();
        //             Params.Add("@p1", nCompanyID);
        //             Params.Add("@p2", xType);
        //             Params.Add("@p3", x_AmendCode);
                   
        //             if (x_AmendCode == "@Auto")
        //             {
        //                 Params.Add("N_CompanyID", nCompanyID);
        //                 Params.Add("N_FormID", this.N_FormID);
        //                 X_AmendCode = dLayer.GetAutoNumber("Gen_ApprovalAmend", "X_AmendCode", Params, connection, transaction);
        //                 if (X_AmendCode == "") { transaction.Rollback();return Ok(api.Error(User,"Unable to generate Amend Code")); }
        //                 MasterTable.Rows[0]["X_AmendCode"] = X_AmendCode;
        //             }



        //             // if (x_AmendCode == "@Auto")
        //             // {
        //             //     Params.Add("N_CompanyID", nCompanyID);
        //             //     x_AmendCode = dLayer.GetAutoNumber("Gen_ApprovalAmend", "X_AmendCode", Params, connection, transaction);
        //             //     if (x_AmendCode == "")
        //             //     {
        //             //         transaction.Rollback();
        //             //         return Ok("Unable to generate Amend Code");
        //             //     }
        //             //     MasterTable.Rows[0]["x_AmendCode"] = x_AmendCode;
        //             // }
        //             // x_AmendCode = MasterTable.Rows[0]["x_AmendCode"].ToString();
        //             // if (N_AmendID > 0)
        //             // {
        //             //     dLayer.DeleteData("Gen_ApprovalAmendDetails", "N_AmendID", N_AmendID, "N_CompanyID=" + nCompanyID + " and N_AmendID=" + N_AmendID, connection, transaction);
        //             //     dLayer.DeleteData("Gen_ApprovalAmend", "N_AmendID", N_AmendID, "N_CompanyID=" + nCompanyID + " and N_AmendID=" + N_AmendID, connection, transaction);
        //             // }


        //             N_AmendID = dLayer.SaveData("Gen_ApprovalAmend", "N_AmendID", "", "", Master, connection, transaction);
        //             if (N_AmendID <= 0)
        //             {
        //                 transaction.Rollback();
        //                 return Ok("Unable to save");
        //             }
        //             for (int i = 0; i < Details.Rows.Count; i++)
        //             {
        //                 Details.Rows[i]["N_AmendID"] = N_AmendID;
        //                 Details.Rows[i]["X_TransType"] = xType;

        //             }

        //             dLayer.SaveData("Gen_ApprovalAmendDetails", "N_AmendDetailsID", Details, connection, transaction);
        //             transaction.Commit();
        //             SortedList Result = new SortedList();
        //             Result.Add("N_AmendID",N_AmendID);

        //             return Ok(api.Success(Result, "Amendment Saved"));
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(api.Error(User, ex));
        //     }
        // }
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
                    string X_AmendCode = MasterTable.Rows[0]["X_AmendCode"].ToString();
                    string xType = MasterTable.Rows[0]["x_TransType"].ToString();
                    string x_AmendCode = "";
                    string xButtonAction = "";
                    if (nAmendID > 0)
                    {
                        dLayer.DeleteData("Gen_ApprovalAmendDetails", "N_AmendID", nAmendID, "N_CompanyID = " + nCompanyID, connection, transaction);
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
                    // MasterTable.Columns.Remove("N_FnYearID");
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
