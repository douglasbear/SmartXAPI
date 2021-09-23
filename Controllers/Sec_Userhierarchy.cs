using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;


namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("secUserHierarchy")]
    [ApiController]
    public class Sec_Userhierarchy : ControllerBase
    {


        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID;
        public Sec_Userhierarchy(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            N_FormID = 1349;//form id of cost center
        }

        [HttpGet("chart")]
        public ActionResult GetCategoryChart()

        {
            DataTable dt = new DataTable();
            DataTable Images = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);


            string sqlCommandText = "Select *  from [vw_Sec_UserHierarchy] Where N_CompanyID= " + nCompanyID + " Order By X_Pattern";


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
                return Ok(_api.Error(User,e));
            }

        }
        private string GetNextChildCode(int nParentID, SortedList ParamList, SqlConnection connection, SqlTransaction transaction)
        {
            // string[] NodeValues = CurrentNode.Tag.ToString().Split('|');
            int ParentID = nParentID;
            string DeptCode = "";
            DataTable dtDept = new DataTable();
            dtDept = dLayer.ExecuteDataTable("select COUNT(convert(nvarchar(100),X_CategoryCode)) From Inv_ItemCategoryDisplay where N_ParentID =" + ParentID + " and N_CompanyID =@nCompanyID ", ParamList, connection, transaction);
            int count = myFunctions.getIntVAL(dtDept.Rows[0][0].ToString());
            dtDept = dLayer.ExecuteDataTable("select X_CategoryCode From Inv_ItemCategoryDisplay where N_CategoryDisplayID =" + ParentID + " and N_CompanyID =@nCompanyID ", ParamList, connection, transaction);
            while (true)
            {
                count += 1;
                DeptCode = dtDept.Rows[0][0].ToString() + count.ToString("100");
                object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_ItemCategoryDisplay Where X_CategoryCode ='" + DeptCode.Trim() + "' and N_CompanyID= @nCompanyID ", ParamList, connection, transaction);
                if (N_Result == null)
                    break;
            }
            return DeptCode;
        }


        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {

                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    SortedList QueryParams = new SortedList();
                    DataRow MasterRow = MasterTable.Rows[0];
                    string DocNo = "";
                    string X_Pattern = MasterTable.Rows[0]["x_Pattern"].ToString();
                    string n_UserID = MasterTable.Rows[0]["n_UserID"].ToString();
                    string N_FnYearID = MasterTable.Rows[0]["n_FnYearId"].ToString();
                    string N_CompanyID = MasterTable.Rows[0]["n_CompanyId"].ToString();
                    int N_HierarchyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_HierarchyID"].ToString());
                    int N_ParentID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ParentID"].ToString());
                    int N_Position = myFunctions.getIntVAL(MasterTable.Rows[0]["n_Position"].ToString());
                    string patternNo = "";
                    QueryParams.Add("@nCompanyID", N_CompanyID);
                    QueryParams.Add("@nHierarchyID", N_HierarchyID);
                    QueryParams.Add("@nParentID", N_ParentID);

                    if (N_HierarchyID == 0)
                    {
                        if (X_Pattern == "10" && N_ParentID == 0)
                        {
                            MasterTable.Rows[0]["X_Pattern"] = "10";
                        }
                        else
                        {
                            object xHierarchyID = dLayer.ExecuteScalar("Select max(N_HierarchyID)  From Sec_Userhierarchy Where N_CompanyID=" + N_CompanyID + " and N_ParentID=" + N_ParentID + " ", connection, transaction);
                            if (myFunctions.getIntVAL(xHierarchyID.ToString()) >0 )
                            {
                                object xPattern = dLayer.ExecuteScalar("Select X_Pattern  From Sec_Userhierarchy Where N_CompanyID=" + N_CompanyID + " and N_HierarchyID=" + myFunctions.getIntVAL(xHierarchyID.ToString()) + " ", connection, transaction);
                                if (xPattern != null)
                                {
                                    patternNo = xPattern.ToString();
                                    int length=X_Pattern.Length;
                                    string removingPattern = patternNo.Substring(length);
                                    int pattern = myFunctions.getIntVAL(removingPattern);
                                    pattern = pattern + 1;
                                    patternNo = pattern.ToString();
                                    if (removingPattern.Length>(pattern.ToString().Length))
                                    {
                                        patternNo =X_Pattern +"0" + patternNo;
                                    }
                                    else {
                                        patternNo=X_Pattern+patternNo;

                                    }

                                }
                                MasterTable.Rows[0]["X_Pattern"] = patternNo;

                            }
                            else
                            {
                                
                                    MasterTable.Rows[0]["X_Pattern"] =X_Pattern+ "01";

                               

                                

                            }




                        }
                    }
                    else if(N_HierarchyID>0)
                    {
                          dLayer.DeleteData("Sec_Userhierarchy", "N_HierarchyID", N_HierarchyID, "", connection, transaction);
                    }
                    MasterTable.Columns.Remove("n_FnYearID");
                    MasterTable.Columns.Remove("n_Position");
                    N_HierarchyID = dLayer.SaveData("Sec_Userhierarchy", "N_HierarchyID", MasterTable, connection, transaction);
                    if (N_HierarchyID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    transaction.Commit();
                    // }

                    return Ok(_api.Success("User Hierarchy Created"));

                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }


        }
         [HttpDelete("delete")]
        public ActionResult DeleteData(int nHierarchyID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nHierarchyID", nHierarchyID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object Objcount = dLayer.ExecuteScalar(" select max(N_HierarchyID) from Sec_UserHierarchy where N_ParentID="+nHierarchyID+" and N_CompanyID="+nCompanyID+" ", QueryParams, connection);
                    if (myFunctions.getIntVAL(Objcount.ToString())>0)
                    {
                        return Ok(_api.Error(User,"Unable to delete"));
                    }
                    else
                    {
                        Results = dLayer.DeleteData("Sec_UserHierarchy", "N_HierarchyID", nHierarchyID, "N_CompanyID=" + nCompanyID + "", connection);
                    }
                }
                 if (Results > 0)
                {
                    return Ok(_api.Success("User deleted"));
                }
                else
                {
                    return Ok(_api.Error(User,"Unable to delete"));
                }
                
     
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }

        }
    }
}










