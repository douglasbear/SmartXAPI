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
    [Route("multiCategory")]
    [ApiController]
    public class Inv_MultiCategory : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID;


        public Inv_MultiCategory(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
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
            DataTable dt1 = new DataTable();
            DataTable Images = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);


            string sqlCommandText = "Select N_CompanyID,N_CategoryDisplayID,N_ParentID,X_CategoryDisplay,X_CategoryCode,'' as X_ImageURL  from Inv_ItemCategoryDisplay Where N_CompanyID= " + nCompanyID + " Order By X_CategoryCode";
            string sqlCommandText1 = "Select *  from Inv_DisplayImages Where N_CompanyID= " + nCompanyID + " ";


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    dt1 = dLayer.ExecuteDataTable(sqlCommandText1, Params, connection);


                    foreach (DataRow dr in dt.Rows)
                    {
                        foreach (DataRow dr1 in dt1.Rows)
                        {

                            if (dr["n_CategoryDisplayID"].ToString() == dr1["N_ItemID"].ToString())
                            {
                                object fileName = dr1["X_ImageName"];
                                if(fileName==null)fileName="";
                                dr["x_ImageURL"] = myFunctions.GetTempFileName(User,"productcategory",fileName.ToString());

                            }
                        }
                    }


                }
                dt.AcceptChanges();
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
                return Ok(_api.Error(User, e));
            }

        }
        [HttpGet("childDetails")]
        public ActionResult GetChildDetails(int nCategoryID)

        {
            DataTable dt = new DataTable();
            object Images = "";
            object ImageLocation = "";

            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);


            string sqlCommandText = "Select *  from Inv_ItemCategoryDisplay Where N_CompanyID= " + nCompanyID + " and N_CategoryDisplayID=" + nCategoryID + "";


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    string _sqlImageQuery = "SELECT X_ImageName from Inv_DisplayImages where N_ItemID=" + nCategoryID + " and N_CompanyID=" + nCompanyID;
                    Images = dLayer.ExecuteScalar(_sqlImageQuery, Params, connection);

                    dt = myFunctions.AddNewColumnToDataTable(dt, "I_Image", typeof(System.String), "");

                    if (Images != null)
                    {

                        var path =  myFunctions.GetUploadsPath(User,"productcategory") + Images.ToString();
                        if (System.IO.File.Exists(path))
                        {
                            Byte[] bytes = System.IO.File.ReadAllBytes(path);
                            dt.Rows[0]["I_Image"] = Convert.ToBase64String(bytes);

                        }
                    }
                    dt.AcceptChanges();
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

                // dt = myFunctions.AddNewColumnToDataTable(dt, "I_Image", typeof(int), 0);

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }

        [HttpGet("list")]
        public ActionResult GetDepartmentList(string parent)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    string sqlCommandText = "";
                    bool B_ShowChild = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "ShowChild", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    if (parent == "" || parent == null)
                    {
                        if (B_ShowChild == true)
                        {

                            sqlCommandText = "select * from Inv_ItemCategoryDisplay where N_CategoryDisplayID not in(select N_ParentID from Inv_ItemCategoryDisplay  ) and    N_ParentID >0 and N_CompanyID= " + nCompanyID + "";
                        }
                        else
                        {
                            sqlCommandText = "Select *  from Inv_ItemCategoryDisplay Where N_CompanyID= " + nCompanyID + "";
                        }
                    }
                    else
                    {

                        sqlCommandText = "Select *  from Inv_ItemCategoryDisplay Where N_CompanyID= " + nCompanyID + "  and isnull(N_ParentID,0)=0   Order By X_CategoryCode";
                    }



                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

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
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    SortedList QueryParams = new SortedList();
                    DataRow MasterRow = MasterTable.Rows[0];
                    DataTable ImageTable = new DataTable();
                    string DocNo = "";
                    string X_CategoryCode = MasterTable.Rows[0]["x_CategoryCode"].ToString();
                    string X_CategoryDisplay = MasterTable.Rows[0]["x_CategoryDisplay"].ToString();
                    string N_FnYearID = MasterTable.Rows[0]["n_FnYearId"].ToString();
                    string N_CompanyID = MasterTable.Rows[0]["n_CompanyId"].ToString();
                    int N_CategoryDisplayID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CategoryDisplayID"].ToString());
                    int N_ParentID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_ParentID"].ToString());
                    int N_Position = myFunctions.getIntVAL(MasterTable.Rows[0]["n_Position"].ToString());
                    string i_Image = MasterTable.Rows[0]["I_Image"].ToString();
                    if (MasterTable.Columns.Contains("I_Image"))
                        MasterTable.Columns.Remove("I_Image");

                    //  if(MasterTable.Columns.Contains)
                    // MasterTable.Rows[0]["n_ManagerID"]=myFunctions.getIntVAL(MasterTable.Rows[0]["n_Empid"].ToString());  // commented by rks

                    QueryParams.Add("@nCompanyID", N_CompanyID);
                    QueryParams.Add("@nCategoryDisplayID", N_CategoryDisplayID);
                    QueryParams.Add("@nParentID", N_ParentID);


                    if (N_CategoryDisplayID == 0)
                    {
                        if (N_ParentID == 0)
                            N_Position = 1;
                        else
                        {
                            object ObjNextLevelID = dLayer.ExecuteScalar("select isnull(max(N_Position),0)+1 from Inv_ItemCategoryDisplay where N_CategoryDisplayID=" + N_ParentID + "  and N_CompanyID= @nCompanyID ", QueryParams, connection, transaction);
                            if (ObjNextLevelID == null)
                                N_Position = 0;
                            else
                                N_Position = myFunctions.getIntVAL(ObjNextLevelID.ToString());
                        }
                        // GetNextLevelPattern(N_ParentID, ref N_LevelPattern, ref X_LevelPattern, QueryParams, connection);
                        // MasterTable.Rows[0]["n_LevelPattern"] = N_LevelPattern;
                        //  MasterTable.Rows[0]["x_LevelPattern"] = X_LevelPattern;

                    }
                    DocNo = MasterRow["x_CategoryCode"].ToString();

                    if (X_CategoryCode == "@Auto" && N_CategoryDisplayID == 0)
                    {
                        Params.Add("N_CompanyID", N_CompanyID);
                        Params.Add("N_FormID", 1349);
                        Params.Add("N_YearID", N_FnYearID);

                        while (true)
                        {
                            DocNo = dLayer.ExecuteScalarPro("SP_AutoNumberGenerate", Params, connection, transaction).ToString();
                            object N_Result = dLayer.ExecuteScalar("Select 1 from Inv_ItemCategoryDisplay Where X_CategoryCode ='" + DocNo + "' and N_CompanyID= " + N_CompanyID, connection, transaction);
                            if (N_Result == null)
                                break;
                        }
                        X_CategoryCode = DocNo;


                        if (X_CategoryCode == "") { transaction.Rollback(); return Ok(_api.Error(User, "Unable to generate")); }
                        MasterTable.Rows[0]["x_CategoryCode"] = X_CategoryCode;

                        // Params.Add("N_CompanyID", N_CompanyID);
                        // Params.Add("N_YearID", N_FnYearID);
                        // Params.Add("N_FormID", 1349);
                        // //Params.Add("N_BranchID", MasterTable.Rows[0]["n_BranchId"].ToString());
                        // X_CategoryCode = dLayer.GetAutoNumber("Inv_ItemCategoryDisplay", "x_CategoryCode", Params, connection, transaction);
                        // if (X_CategoryCode == "") { transaction.Rollback(); return Ok(_api.Error(User,"Unable to generate Category")); }
                        // MasterTable.Rows[0]["x_CategoryCode"] = X_CategoryCode;


                    }
                    else if (X_CategoryCode == "@Auto" && N_CategoryDisplayID > 0)
                    {
                        X_CategoryCode = GetNextChildCode(N_CategoryDisplayID, QueryParams, connection, transaction);
                        MasterTable.Rows[0]["x_CategoryCode"] = X_CategoryCode;
                        MasterTable.Rows[0]["n_CategoryDisplayID"] = 0;
                    }

                    MasterTable.Columns.Remove("n_FnYearID");
                    N_CategoryDisplayID = dLayer.SaveData("Inv_ItemCategoryDisplay", "N_CategoryDisplayID", MasterTable, connection, transaction);
                    if (N_CategoryDisplayID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User, "Unable to save"));
                    }


                    dLayer.DeleteData("Inv_DisplayImages", "N_ItemID", N_CategoryDisplayID, "", connection, transaction);

                    if (i_Image != null || i_Image != "")
                    {
                        DataTable dt = new DataTable();
                        dt.Clear();

                        dt.Columns.Add("X_ImageName", typeof(System.String));
                        dt.Columns.Add("X_ImageLocation", typeof(System.String));
                        dt.Columns.Add("N_ImageID", typeof(System.Int32));
                        dt.Columns.Add("N_CompanyID", typeof(int));
                        dt.Columns.Add("N_ItemID", typeof(int));
                        dt.Columns.Add("N_Type", typeof(int));
                        //ImageTable.Columns.Add("D_EntryDate", typeof(int));


                        DataRow row = dt.NewRow();
                        myFunctions.writeImageFile(i_Image.ToString(), myFunctions.GetUploadsPath(User,"productcategory"), X_CategoryCode + "-Category");
                        row["X_ImageName"] = X_CategoryCode + "-Category.jpg";
                        row["X_ImageLocation"] =  myFunctions.GetUploadsPath(User,"productcategory");
                        row["N_ImageID"] = 0;
                        row["N_CompanyID"] = N_CompanyID;
                        row["N_ItemID"] = N_CategoryDisplayID;
                        row["N_Type"] = 2;
                        dt.Rows.Add(row);



                        dLayer.SaveData("Inv_DisplayImages", "N_ImageID", dt, connection, transaction);

                    }

                    // int nAdditionDetailsID = dLayer.SaveData("Inv_ItemCategoryDisplayMaster", "N_ItemID", DetailTable, connection, transaction);
                    // if (nAdditionDetailsID <= 0)
                    // {
                    //     transaction.Rollback();
                    //     return Ok(_api.Error(User,"Unable To Save"));
                    // }
                    // else
                    // {
                    transaction.Commit();
                    // }

                    return Ok(_api.Success("Category Saved"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
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

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCategoryDisplayID)
        {
            int Results = 0;
            int nCompanyID = myFunctions.GetCompanyID(User);
            try
            {
                SortedList QueryParams = new SortedList();
                QueryParams.Add("@nCompanyID", nCompanyID);
                QueryParams.Add("@nCategoryDisplayID", nCategoryDisplayID);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    object Objcount = dLayer.ExecuteScalar("Select count(*) From Inv_ItemCategoryDisplayMaster where N_CategoryDisplayID=" + nCategoryDisplayID + " and N_CompanyID=" + nCompanyID + " ", QueryParams, connection);
                    if (Objcount != null)
                    {
                        if (myFunctions.getIntVAL(Objcount.ToString()) <= 0)
                        {
                            Results = dLayer.DeleteData("Inv_ItemCategoryDisplay", "N_CategoryDisplayID", nCategoryDisplayID, "N_CompanyID=" + nCompanyID + "", connection);
                        }
                        else
                        {
                            return Ok(_api.Error(User, "Category Allready Used"));
                        }
                    }
                }
                if (Results > 0)
                {
                    return Ok(_api.Success("Category deleted"));
                }
                else
                {
                    return Ok(_api.Error(User, "Unable to delete"));
                }

            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }

        }


    }
}


