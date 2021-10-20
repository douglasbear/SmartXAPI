using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Net.Mail;
using System.Text;
using System.IO;
using System.Threading.Tasks;
namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("employeeWorkGroup")]
    [ApiController]
    public class Pay_EmployeeWorkGroup : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int FormID;
        StringBuilder message = new StringBuilder();
        public Pay_EmployeeWorkGroup(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID = 716;

        }

        [HttpGet("Details")]
        public ActionResult GetDetails(int categoryID, string xPkeyCode)
        {
            try
            {
                using (SqlConnection Con = new SqlConnection(connectionString))
                {
                    Con.Open();
                    DataSet PayWorkGroup = new DataSet();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    DataTable WorkingHours = new DataTable();
                    DataTable AdditionalWorkingHours = new DataTable();
                    string workingHoursSql = "";
                    string AdditionalWorkingHourssql = "";
                    string sqlCommandText = "";
                    if (xPkeyCode != null && xPkeyCode != "")
                    {
                        sqlCommandText = "Select * From vw_EmpGrp_Workhours Where N_CompanyID=" + nCompanyID + " and X_PkeyCode='" + xPkeyCode + "'";
                        DataTable MasterTable = dLayer.ExecuteDataTable(sqlCommandText, Params, Con);
                        if (MasterTable.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                        MasterTable = _api.Format(MasterTable, "Master");
                        PayWorkGroup.Tables.Add(MasterTable);
                        categoryID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PkeyId"].ToString());
                    }
                    //Business Hours Table
                    workingHoursSql = "Select * from Pay_WorkingHours Where N_CompanyID =" + nCompanyID + "and N_CatagoryID=" + categoryID + " order by N_WHID";
                    WorkingHours = dLayer.ExecuteDataTable(workingHoursSql, Params, Con);
                    if (WorkingHours.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }

                    //Additional Business Hours
                    AdditionalWorkingHourssql = "Select * from Pay_AdditionalWorkingDays Where N_CompanyID =" + nCompanyID + "and N_CatagoryID=" + categoryID + " order by N_ID";
                    AdditionalWorkingHours = dLayer.ExecuteDataTable(AdditionalWorkingHourssql, Params, Con);
                    // if (AdditionalWorkingHours.Rows.Count == 0) { return Ok(_api.Warning("No data found")); }
                    PayWorkGroup.Tables.Add(WorkingHours);
                    PayWorkGroup.Tables.Add(AdditionalWorkingHours);
                    return Ok(_api.Success(PayWorkGroup));
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();
                    DataTable MasterTable;
                    DataTable WorkingHours;
                    DataTable AdditionalWorkingHours;
                    string DocNo = "";
                    MasterTable = ds.Tables["master"];
                    WorkingHours = ds.Tables["WorkingHours"];
                    AdditionalWorkingHours = ds.Tables["AdditionalWorkingHours"];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    int nPkeyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_PkeyID"].ToString());
                    string X_PkeyCode = MasterTable.Rows[0]["X_PkeyCode"].ToString();

                    if (nPkeyID > 0)
                    {
                        dLayer.DeleteData("Pay_WorkingHours", "N_CatagoryID", nPkeyID, "", con, transaction);
                        dLayer.DeleteData("Pay_AdditionalWorkingDays", "N_CatagoryID", nPkeyID, "", con, transaction);
                    }
                    DocNo = MasterTable.Rows[0]["X_PkeyCode"].ToString();
                    if (X_PkeyCode == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_FormID", this.FormID);



                        while (true)
                        {

                            object N_Result = dLayer.ExecuteScalar("Select 1 from Pay_EmployeeGroup Where X_PkeyCode ='" + DocNo + "' and N_CompanyID= " + nCompanyID, con, transaction);
                            if (N_Result == null) DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, con, transaction).ToString();
                            break;
                        }
                        X_PkeyCode = DocNo;


                        if (X_PkeyCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate")); }
                        MasterTable.Rows[0]["X_PkeyCode"] = X_PkeyCode;


                    }
                    nPkeyID = dLayer.SaveData("Pay_EmployeeGroup", "N_PkeyId", MasterTable, con, transaction);
                    if (nPkeyID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }
                    for (int i = 0; i < WorkingHours.Rows.Count; i++)
                    {
                        WorkingHours.Rows[i]["categoryID"] = nPkeyID;
                    }
                    int N_ID = dLayer.SaveData("Pay_WorkingHours", "N_ID", WorkingHours, con, transaction);
                    if (N_ID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable To Save"));
                    }
                    if (AdditionalWorkingHours.Rows.Count > 0)
                    {
                        for (int i = 0; i < AdditionalWorkingHours.Rows.Count; i++)
                        {
                            WorkingHours.Rows[i]["N_CategoryID"] = nPkeyID;
                        }
                        N_ID = dLayer.SaveData("Pay_WorkingHours", "N_ID", WorkingHours, con, transaction);
                        if (N_ID <= 0)
                        {
                            transaction.Rollback();
                            return Ok(_api.Error(User, "Unable To Save"));
                        }

                    }


                    transaction.Commit();
                    return Ok(_api.Success("Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCompanyID, int piPKeyId, int nFnyearID, int piRefId)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    SortedList Params = new SortedList();
                    Params.Add("N_CompanyID", nCompanyID);
                    Params.Add("N_FnYearID", nFnyearID);
                    Params.Add("piRefId", piRefId);
                    Params.Add("piPKeyId", piPKeyId);
                    try
                    {
                        Results = dLayer.ExecuteNonQueryPro("SP_Delete_Gen_LookupTable", Params, connection, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, ex));
                    }
                    transaction.Commit();
                    return Ok(_api.Success(" deleted"));


                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }

        }
    }
}













