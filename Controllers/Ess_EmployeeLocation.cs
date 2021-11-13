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
    [Route("employeelocation")]
    [ApiController]



    public class Ess_EmployeeLocation : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly string xTransType;
        private readonly int FormID;


        public Ess_EmployeeLocation(IDataAccessLayer dl, IApiFunctions apiFun, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            api = apiFun;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 1333;
            xTransType = "WORK LOCATION";
        }


        [HttpGet("listlocation")] 
        public ActionResult OpportunityList(int nEmpID, int nFnYearID,string deviceID)
        {
            DataTable location = new DataTable();
            DataTable devices = new DataTable();
            DataTable Approvals= new DataTable();
            // Approvals = ds.Tables["approval"];
            // DataRow ApprovalRow = Approvals.Rows[0];
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);


            string sqlLocation = "SELECT Pay_WorkLocation.* FROM  Pay_WorkLocation LEFT OUTER JOIN  Pay_Employee ON Pay_WorkLocation.N_LocationId = Pay_Employee.N_WorkLocationID AND Pay_WorkLocation.N_CompanyId = Pay_Employee.N_CompanyID where Pay_Employee.N_CompanyID=@p1 and Pay_Employee.N_EmpID=@p2 and Pay_Employee.N_FnYearID=@nFnYearID ";
            string sqlDevices = "SELECT * FROM  Pay_EmpDeviceIDRegistration where N_CompanyID=@p1 and N_EmpID=@p2 and X_DeviceID=@deviceID and B_Active=1";
           
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nEmpID);
            Params.Add("@nFnYearID", nFnYearID);
            Params.Add("@deviceID", deviceID);



            SortedList OutPut = new SortedList();


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    location = dLayer.ExecuteDataTable(sqlLocation, Params, connection);
                    devices = dLayer.ExecuteDataTable(sqlDevices, Params, connection);

                    OutPut.Add("locations", api.Format(location));
                    OutPut.Add("devices", api.Format(devices));

                    return Ok(api.Success(OutPut));

                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User,e));
            }
        }
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                DataTable Approvals;
                Approvals = ds.Tables["approval"];
                DataRow ApprovalRow = Approvals.Rows[0];

                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nLocationId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_LocationId"].ToString());
                int N_NextApproverID = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    string LocationCode = "";
                    var values = MasterTable.Rows[0]["X_LocationCode"].ToString();

                    if ((!myFunctions.getBoolVAL(ApprovalRow["isEditable"].ToString())) && nLocationId>0)
                    {
                        int N_PkeyID = nLocationId;
                        string X_Criteria = "N_LocationId=" + N_PkeyID + " and N_CompanyID=" + nCompanyID;
                        myFunctions.UpdateApproverEntry(Approvals, "Pay_WorkLocation", X_Criteria, N_PkeyID, User, dLayer, connection, transaction);
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearId, this.xTransType, N_PkeyID, values, 1,"", 0, "", User, dLayer, connection, transaction);
                        transaction.Commit();
                        myFunctions.SendApprovalMail(N_NextApproverID, FormID, N_PkeyID, this.xTransType, values, dLayer, connection, transaction, User);
                        return Ok(api.Success("Work Location Approved " + "-" + values));
                    }

                    // Auto Gen
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.FormID);
                        LocationCode = dLayer.GetAutoNumber("Pay_WorkLocation", "X_LocationCode", Params, connection, transaction);
                        if (LocationCode == "") { transaction.Rollback(); return Ok(api.Error(User,"Unable to generate Location Code")); }
                        MasterTable.Rows[0]["X_LocationCode"] = LocationCode;
                    }
                    MasterTable.Columns.Remove("n_FnYearId");
                    MasterTable.AcceptChanges();

                    MasterTable = myFunctions.SaveApprovals(MasterTable, Approvals, dLayer, connection, transaction);
                    nLocationId = dLayer.SaveData("Pay_WorkLocation", "N_LocationId", MasterTable, connection, transaction);
                    if (nLocationId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        N_NextApproverID = myFunctions.LogApprovals(Approvals, nFnYearId, this.xTransType, nLocationId, LocationCode, 1, "", 0, "", User, dLayer, connection, transaction);

                        transaction.Commit();

                        myFunctions.SendApprovalMail(N_NextApproverID, FormID, nLocationId, this.xTransType, LocationCode, dLayer, connection, transaction, User);

                        return Ok(api.Success("Location Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nLocationID, int nFnYearID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable TransData = new DataTable();
                    SortedList ParamList = new SortedList();
                    ParamList.Add("@nLocationID", nLocationID);
                    ParamList.Add("@nFnYearID", nFnYearID);
                    ParamList.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    string Sql = "select isNull(N_UserID,0) as N_UserID,isNull(N_ProcStatus,0) as N_ProcStatus,isNull(N_ApprovalLevelId,0) as N_ApprovalLevelId,0 as N_EmpID,X_LocationCode from Pay_WorkLocation where N_CompanyId=@nCompanyID and N_LocationId=@nLocationID";
                    TransData = dLayer.ExecuteDataTable(Sql, ParamList, connection);
                    if (TransData.Rows.Count == 0)
                    {
                        return Ok(api.Error(User,"Transaction not Found"));
                    }
                    DataRow TransRow = TransData.Rows[0];

                    DataTable Approvals = myFunctions.ListToTable(myFunctions.GetApprovals(-1, this.FormID, nLocationID, myFunctions.getIntVAL(TransRow["N_UserID"].ToString()), myFunctions.getIntVAL(TransRow["N_ProcStatus"].ToString()), myFunctions.getIntVAL(TransRow["N_ApprovalLevelId"].ToString()), 0, 0, 1, nFnYearID,0, 0, User, dLayer, connection));
                    Approvals = myFunctions.AddNewColumnToDataTable(Approvals, "comments", typeof(string), "");
                    SqlTransaction transaction = connection.BeginTransaction(); ;

                    string X_Criteria = "N_LocationId=" + nLocationID + " and N_CompanyID=" + myFunctions.GetCompanyID(User) ;

                    string ButtonTag = Approvals.Rows[0]["deleteTag"].ToString();
                    int ProcStatus = myFunctions.getIntVAL(ButtonTag.ToString());
                    string status = myFunctions.UpdateApprovals(Approvals, nFnYearID, this.xTransType, nLocationID, TransRow["X_LocationCode"].ToString(), ProcStatus, "Pay_WorkLocation", X_Criteria, "", User, dLayer, connection, transaction);
                    if (status != "Error")
                    {
                        transaction.Commit();
                        return Ok(api.Success("Loan Request " + status + " Successfully"));
                    }
                    else
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to delete Loan request"));
                    }


                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

    }

}