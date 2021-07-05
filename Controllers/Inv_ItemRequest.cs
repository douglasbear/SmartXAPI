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
    [Route("itemRequest")]
    [ApiController]
    public class Inv_ItemRequest : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Inv_ItemRequest(IApiFunctions apiFun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apiFun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }



        [HttpGet("list")]
        public ActionResult GetRequestList(int nCompanyId, int nFnYearId, int nBranchID, bool bAllBranchData, int FormID, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string sqlCondition = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and ([PRS No] like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_PRSID desc";
            else
            {
                switch (xSortBy.Split(" ")[0])
                {
                    case "PRSNo":
                        xSortBy = "[PRS No] " + xSortBy.Split(" ")[1];
                        break;
                    case "N_PRSID":
                        xSortBy = "N_PRSID " + xSortBy.Split(" ")[1];
                        break;
                    default: break;
                }
                xSortBy = " order by " + xSortBy;
            }

            Params.Add("@nCompanyId", nCompanyId);
            Params.Add("@nFnYearId", nFnYearId);
            Params.Add("@FormID", FormID);
            Params.Add("@nBranchID", nBranchID);
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (!myFunctions.CheckClosedYear(nCompanyId, nFnYearId, dLayer, connection))
                    {
                        if (bAllBranchData)
                            sqlCondition = "N_CompanyID=@nCompanyId and B_YearEndProcess=0 and N_FormID=@FormID";
                        else
                            sqlCondition = "N_CompanyID=@nCompanyId and N_BranchID=@nBranchID and B_YearEndProcess=0 and N_FormID=@FormID";
                    }
                    else
                    {
                        if (bAllBranchData)
                            sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and N_FormID=@FormID";
                        else
                            sqlCondition = "N_CompanyID=@nCompanyId and N_FnYearID=@nFnYearId and N_BranchID=@nBranchID and N_FormID=@FormID";
                    }

                    if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvPRSNo_UCSearch where " + sqlCondition + " " + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from vw_InvPRSNo_UCSearch where " + sqlCondition + " " + Searchkey + " and N_PRSID not in (select top(" + Count + ") N_PRSID from vw_InvPRSNo_UCSearch where " + sqlCondition + " " + xSortBy + " ) " + xSortBy;


                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount = "select count(*) as N_Count from vw_InvPRSNo_UCSearch where " + sqlCondition + " " + Searchkey + "";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                    string TotalSum = "0";
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                        //TotalSum = drow["TotalAmount"].ToString();
                    }
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    //OutPut.Add("TotalSum", TotalSum);
                }
                // dt = api.Format(dt);
                if (dt.Rows.Count == 0)
                {
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(OutPut));
                }
            }
            catch (Exception e)
            {
                return StatusCode(403, api.Error(e));
            }
        }

        [HttpGet("settings")]
        public ActionResult CheckSettings(string FormID, int nBranchID, bool bAllBranchData)
        {
            double N_decimalPlace = 0;
            bool B_MultipleLocation = false, B_LocationRequired = false, B_IsPartNoInGrid = false, B_DeptEnable = false, B_DelDays = false, B_Remarks = false, B_CustomerProjectEnabled = false;
            bool B_FileNoVisible = false, B_ShortcutKeyF2 = false, B_ShowProject = false, B_ShowProjectInGrid = false, B_FreeDescription = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    Params.Add("@nCompanyID", nCompanyID);
                    //Params.Add("@nFnYearID", nFnYearID);

                    dt.Clear();
                    dt.Columns.Add("N_decimalPlace");
                    dt.Columns.Add("B_MultipleLocation");
                    dt.Columns.Add("B_LocationRequired");
                    dt.Columns.Add("B_IsPartNoInGrid");
                    dt.Columns.Add("B_DeptEnable");
                    dt.Columns.Add("B_DelDays");
                    dt.Columns.Add("B_Remarks");
                    dt.Columns.Add("B_CustomerProjectEnabled");
                    dt.Columns.Add("B_FileNoVisible");
                    // dt.Columns.Add("B_ShortcutKeyF2");
                    dt.Columns.Add("B_ShowProject");
                    dt.Columns.Add("B_ShowProjectInGrid");
                    dt.Columns.Add("B_FreeDescription");

                    bool B_EmpFilterByPrj = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("371", "EnablePrjWiseEmp", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    N_decimalPlace = myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "Decimal_Place", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection));
                    B_MultipleLocation = LocationCount(nBranchID, bAllBranchData, dLayer, connection);
                    B_LocationRequired = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("64", "Location_InGrid", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_IsPartNoInGrid = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings(FormID, "IsPartNoEnable", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_DeptEnable = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("PRS", "Dep_Enable", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    B_DelDays = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings(FormID, "IdDelDaysingrid", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_Remarks = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings(FormID, "IsRemarksingrid", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_CustomerProjectEnabled = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Inventory", "CustomerProject Enabled", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    B_FileNoVisible = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("FileNo", "FileNo_Visible", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    // B_ShortcutKeyF2 = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("64", "ShortcutKeyF2", "N_Value", "N_UserCategoryID", myCompanyID._UserCategoryID.ToString(),myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    B_ShowProject = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Purchase", "Enable Project", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_ShowProjectInGrid = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("Purchase", "Enable Project In Request Grid", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));
                    B_FreeDescription = Convert.ToBoolean(myFunctions.getIntVAL(myFunctions.ReturnSettings("82", "FreeDescription_InPurchase", "N_Value", myFunctions.getIntVAL(nCompanyID.ToString()), dLayer, connection)));

                    DataRow row = dt.NewRow();
                    row["N_decimalPlace"] = myFunctions.getVAL(N_decimalPlace.ToString());
                    row["B_MultipleLocation"] = B_MultipleLocation;
                    row["B_LocationRequired"] = B_LocationRequired;
                    row["B_IsPartNoInGrid"] = B_IsPartNoInGrid;
                    row["B_DeptEnable"] = B_DeptEnable;
                    row["B_DelDays"] = B_DelDays;
                    row["B_Remarks"] = B_Remarks;
                    row["B_CustomerProjectEnabled"] = B_CustomerProjectEnabled;
                    row["B_FileNoVisible"] = B_FileNoVisible;
                    //row["B_ShortcutKeyF2"] = B_ShortcutKeyF2;
                    row["B_ShowProject"] = B_ShowProject;
                    row["B_ShowProjectInGrid"] = B_ShowProjectInGrid;
                    row["B_FreeDescription"] = B_FreeDescription;

                    dt.Rows.Add(row);

                    dt = api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Notice("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(dt));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }

        }
        private bool LocationCount(int nBranchID, bool bAllBranchData, IDataAccessLayer dLayer, SqlConnection connection)
        {
            object branch_count = null;
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);
            Params.Add("@nBranchID", nBranchID);

            if (bAllBranchData)
                branch_count = dLayer.ExecuteScalar("SELECT count(*) FROm Inv_Location  Where N_CompanyID=@nCompanyID", Params, connection);
            else
                branch_count = dLayer.ExecuteScalar("SELECT count(*) FROm Inv_Location  Where N_CompanyID=@nCompanyID and N_BranchID=@nBranchID", Params, connection);


            if (myFunctions.getIntVAL(branch_count.ToString()) == 1)
            {

                return false;
            }
            return true;
        }

        [HttpGet("listdetails")]
        public ActionResult GetItemUnitListDetails(int? nCompanyId, int? nItemUnitID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from vw_InvItemUnit_Disp where N_CompanyID=@p1 and N_ItemUnitID=@p2 order by ItemCode";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nItemUnitID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
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
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        //Save....
        [HttpPost("Save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];

                SortedList Params = new SortedList();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    string X_ItemUnit = MasterTable.Rows[0]["X_ItemUnit"].ToString();
                    string DupCriteria = "N_CompanyID=" + myFunctions.GetCompanyID(User) + " and X_ItemUnit='" + X_ItemUnit + "'";
                    int N_ItemUnitID = dLayer.SaveData("Inv_ItemUnit", "N_ItemUnitID", DupCriteria, "", MasterTable, connection, transaction);
                    if (N_ItemUnitID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Warning("Unit Already Exist"));
                    }
                    else
                    {
                        transaction.Commit();
                    }
                    return Ok(api.Success("Unit Created"));
                }


            }

            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }
        }


        [HttpGet("itemwiselist")]
        public ActionResult GetItemWiseUnitList(string baseUnit, int itemId)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            if (baseUnit == null) { baseUnit = ""; }
            try
            {
                SortedList mParamsList = new SortedList()
                    {
                        {"@N_CompanyID",nCompanyId},
                        {"@X_ItemUnit",baseUnit},
                        {"@N_ItemID",itemId}
                    };
                DataTable masterTable = new DataTable();

                string sql = " Select Inv_ItemUnit.X_ItemUnit,Inv_ItemUnit.N_Qty,dbo.SP_SellingPrice(Inv_ItemUnit.N_ItemID,Inv_ItemUnit.N_CompanyID) as N_SellingPrice,Inv_ItemUnit.N_SellingPrice as N_UnitSellingPrice,Inv_ItemUnit.B_BaseUnit,Inv_ItemUnit.N_ItemUnitID,Inv_ItemMaster.N_PurchaseCost from Inv_ItemUnit Left Outer join Inv_ItemUnit as Base On Inv_ItemUnit.N_BaseUnitID=Base.N_ItemUnitID inner join Inv_ItemMaster ON Inv_ItemUnit.N_ItemID=Inv_ItemMaster.N_ItemID and Inv_ItemUnit.N_CompanyID=Inv_ItemMaster.N_CompanyID where Base.X_ItemUnit=@X_ItemUnit and Inv_ItemUnit.N_CompanyID=@N_CompanyID and Inv_ItemUnit.N_ItemID =@N_ItemID and isnull(dbo.Inv_ItemUnit.B_InActive,0)=0 UNION Select Inv_ItemUnit.X_ItemUnit,Inv_ItemUnit.N_Qty,dbo.SP_SellingPrice(Inv_ItemUnit.N_ItemID,Inv_ItemUnit.N_CompanyID) as N_SellingPrice,Inv_ItemUnit.N_SellingPrice as N_UnitSellingPrice,Inv_ItemUnit.B_BaseUnit,Inv_ItemUnit.N_ItemUnitID,Inv_ItemMaster.N_PurchaseCost from Inv_ItemUnit inner join Inv_ItemMaster ON Inv_ItemUnit.N_ItemID=Inv_ItemMaster.N_ItemID and Inv_ItemUnit.N_CompanyID=Inv_ItemMaster.N_CompanyID where Inv_ItemUnit.X_ItemUnit=@X_ItemUnit and Inv_ItemUnit.N_CompanyID=@N_CompanyID and Inv_ItemUnit.N_ItemID =@N_ItemID and isnull(dbo.Inv_ItemUnit.B_InActive,0)=0";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    masterTable = dLayer.ExecuteDataTable(sql, mParamsList, connection);
                    // masterTable = dLayer.ExecuteDataTablePro("SP_FillItemUnit", mParamsList, connection);
                }


                if (masterTable.Rows.Count == 0) { return Ok(api.Notice("No Data Found")); }
                return Ok(api.Success(masterTable));
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }


        [HttpGet("itemUnitList")]
        public ActionResult GetItemUnitList(string baseUnit, int itemId)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            if (baseUnit == null) { baseUnit = ""; }
            try
            {
                SortedList mParamsList = new SortedList()
                    {
                        {"@N_CompanyID",nCompanyId},
                        {"@X_ItemUnit",baseUnit},
                        {"@N_ItemID",itemId}
                    };
                DataTable masterTable = new DataTable();

                string sql = " Select Inv_ItemUnit.X_ItemUnit,Inv_ItemUnit.N_Qty,dbo.SP_SellingPrice(Inv_ItemUnit.N_ItemID,Inv_ItemUnit.N_CompanyID) as N_SellingPrice,Inv_ItemUnit.N_SellingPrice as N_UnitSellingPrice,Inv_ItemUnit.B_BaseUnit from Inv_ItemUnit Left Outer join Inv_ItemUnit as Base On Inv_ItemUnit.N_BaseUnitID=Base.N_ItemUnitID where Base.X_ItemUnit=@X_ItemUnit and Inv_ItemUnit.N_CompanyID=@N_CompanyID and Inv_ItemUnit.N_ItemID =@N_ItemID and isnull(dbo.Inv_ItemUnit.B_InActive,0)=0 UNION Select X_ItemUnit,Inv_ItemUnit.N_Qty,dbo.SP_SellingPrice(Inv_ItemUnit.N_ItemID,Inv_ItemUnit.N_CompanyID) as N_SellingPrice,Inv_ItemUnit.N_SellingPrice as N_UnitSellingPrice,Inv_ItemUnit.B_BaseUnit from Inv_ItemUnit where X_ItemUnit=@X_ItemUnit and N_CompanyID=@N_CompanyID and N_ItemID =@N_ItemID and isnull(dbo.Inv_ItemUnit.B_InActive,0)=0";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    masterTable = dLayer.ExecuteDataTable(sql, mParamsList, connection);
                    // masterTable = dLayer.ExecuteDataTablePro("SP_FillItemUnit", mParamsList, connection);
                }


                if (masterTable.Rows.Count == 0) { return Ok(api.Notice("No Data Found")); }
                return Ok(api.Success(masterTable));
            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nItemUnitID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Inv_ItemUnit", "N_ItemUnitID", nItemUnitID, "", connection);
                }
                if (Results > 0)
                {
                    return Ok(api.Success("Product Unit deleted"));
                }
                else
                {
                    return Ok(api.Warning("Unable to delete product Unit"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(ex));
            }


        }
        [HttpGet("empUserList")]
        public ActionResult GetEmpUser(int nDepartmentID, int nFnYearID, bool bAllBranchData, int nBranchID)

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);

            string sqlCommandText = "";
            if (nDepartmentID > 0)
            {
                if (bAllBranchData == true)
                    sqlCommandText = "Select *  from vw_PayEmployeeUser where N_CompanyID=" + nCompanyID + " and (N_Status = 0 OR N_Status = 1) and N_FnYearID=" + nFnYearID + " and N_DepartmentID =" + nDepartmentID;
                else
                    sqlCommandText = "Select *  from vw_PayEmployeeUser where N_CompanyID=" + nCompanyID + " and (N_Status = 0 OR N_Status = 1) and N_FnYearID=" + nFnYearID + " and (N_BranchID=0 OR N_BranchID=" + nBranchID + ") and N_DepartmentID =" + nDepartmentID;
            }
            else
            {
                if (bAllBranchData == true)
                    sqlCommandText = "Select *  from vw_PayEmployeeUser N_CompanyID=" + nCompanyID + " and (N_Status = 0 OR N_Status = 1) and N_FnYearID=" + nFnYearID + "";
                else
                    sqlCommandText = "Select *  from vw_PayEmployeeUser N_CompanyID=" + nCompanyID + " and (N_Status = 0 OR N_Status = 1) and N_FnYearID=" + nFnYearID + " and (N_BranchID=0 OR N_BranchID=" + nBranchID + ")";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
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
                return Ok(api.Error(e));
            }
        }
        [HttpGet("department")]
        public ActionResult GetDepartment(string xLevelPattern)

        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nCompanyID", nCompanyID);


            string sqlCommandText = "";
            

            if (xLevelPattern != "")
               sqlCommandText = "Select *  from vw_PayDepartment_Disp Where N_CompanyID= " + nCompanyID + " and  X_LevelPattern like '" + xLevelPattern + "%' and isnull(B_Inactive,0)<>1";
            else
                sqlCommandText = "Select *  from vw_PayDepartment_Disp Where N_CompanyID= " + nCompanyID + " and isnull(B_Inactive,0)<>1";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = api.Format(dt);
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
                return Ok(api.Error(e));
            }

        }














    }
}