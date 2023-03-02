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
    [Route("itemunit")]
    [ApiController]
    public class Inv_ItemUnit : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public Inv_ItemUnit(IApiFunctions apiFun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apiFun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }



        [HttpGet("list")]
        public ActionResult GetItemUnitList()
        {
            int nCompanyId= myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select Code,[Unit Code],Description,N_Decimal from vw_InvItemUnit_Disp where N_CompanyID=@p1 and N_ItemID is null order by ItemCode,[Unit Code]";
            Params.Add("@p1", nCompanyId);

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
                return Ok(api.Error(User,e));
            }
        }

        [HttpGet("listdetails")]
        public ActionResult GetItemUnitListDetails(int? nCompanyId,int? n_ItemUnitID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from Inv_ItemUnit where N_CompanyID=@p1 and N_ItemUnitID=@p2 ";
            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", n_ItemUnitID);

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
                return Ok(api.Error(User,e));
            }
        }


     

   [HttpGet("dashboardList")]
        public ActionResult GetProductUnitList(int nPage,bool adjustment,int nSizeperpage, string xSearchkey, string xSortBy,int nItemUnitID,int nCompanyId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                     nCompanyId = myFunctions.GetCompanyID(User);
                    string sqlCommandCount = "", xCriteria = "";
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string criteria = "";
                    string cndn = "";
                    string Searchkey = "";
                    Params.Add("@p1", nCompanyId);


                   if (xSearchkey != null && xSearchkey.Trim() != "")
                    Searchkey = "and (N_ItemUnitID like '%" + xSearchkey + "%' OR X_Description like '%" + xSearchkey + "%')";

                    
                   
                   if (xSortBy == null || xSortBy.Trim() == "")
                        xSortBy = " order by N_ItemUnitID desc";
                      


                   if (Count == 0)
                        sqlCommandText = "select top(" + nSizeperpage + ") * from Inv_ItemUnit where ISNULL(B_BaseUnit,0)=1 and N_CompanyID=@p1 and ISNULL(N_ItemID,0)=0"+ criteria + cndn + Searchkey + " " + xSortBy;
                    else
                        sqlCommandText = "select top(" + nSizeperpage + ") * from Inv_ItemUnit where ISNULL(B_BaseUnit,0)=1 and N_CompanyID=@p1 and ISNULL(N_ItemID,0)=0 and N_ItemUnitID not in (select top(" + Count + ") N_ItemUnitID from Inv_ItemUnit where ISNULL(N_BaseUnitID,0)=0 and N_CompanyID=@p1 and ISNULL(N_ItemID,0)=0 )"+ criteria + cndn + Searchkey + " " + xSortBy ;


                    SortedList OutPut = new SortedList();

                    dt = dLayer.ExecuteDataTable(sqlCommandText , Params, connection);
                   sqlCommandCount = "select count(1) as N_Count  from Inv_ItemUnit where ISNULL(B_BaseUnit,0)=1 and N_CompanyID=@p1 and ISNULL(N_ItemID,0)=0";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(api.Success(OutPut));
                    }
                }
            }
              catch (Exception e)
                {
                return BadRequest(api.Error(User, e));
                }
            

        }


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
                    string X_ItemUnit= MasterTable.Rows[0]["X_ItemUnit"].ToString();
                    string DupCriteria = "N_CompanyID=" + myFunctions.GetCompanyID(User) + " and X_ItemUnit='" + X_ItemUnit + "' and B_BaseUnit=1";
                    int N_ItemUnitID = dLayer.SaveData("Inv_ItemUnit", "N_ItemUnitID",DupCriteria,"", MasterTable, connection, transaction);
                    if (N_ItemUnitID <= 0)
                    {
                        transaction.Rollback();
                        return Ok( api.Warning("Unit Already Exist"));
                    }
                    else
                    {
                        transaction.Commit();
                    }
                    return Ok( api.Success("Unit Created"));
                }
                

            }

            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }


        [HttpGet("unitList")]
        public ActionResult unitList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            Params.Add("@nComapnyID", nCompanyID);
            SortedList OutPut = new SortedList();
            string sqlCommandText = "select * from Inv_ItemUnit where ISNULL(N_BaseUnitID,0)=0 and N_CompanyID=@nComapnyID and ISNULL(N_ItemID,0)=0 and isnull(B_BaseUnit,0)=1 order by N_ItemUnitID desc";
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
                return Ok(api.Error(User,e));
            }
        }


    

        [HttpGet("itemwiselist")]
        public ActionResult GetItemWiseUnitList( string baseUnit, int itemId)
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

// string sql = " Select Inv_ItemUnit.X_ItemUnit,Inv_ItemUnit.N_Qty,dbo.SP_SellingPrice(Inv_ItemUnit.N_ItemID,Inv_ItemUnit.N_CompanyID) as N_SellingPrice,Inv_ItemUnit.N_SellingPrice as N_UnitSellingPrice,Inv_ItemUnit.B_BaseUnit,Inv_ItemUnit.N_ItemUnitID,Inv_ItemMaster.N_PurchaseCost from Inv_ItemUnit Left Outer join Inv_ItemUnit as Base On Inv_ItemUnit.N_BaseUnitID=Base.N_ItemUnitID inner join Inv_ItemMaster ON Inv_ItemUnit.N_ItemID=Inv_ItemMaster.N_ItemID and Inv_ItemUnit.N_CompanyID=Inv_ItemMaster.N_CompanyID where Base.X_ItemUnit=@X_ItemUnit and Inv_ItemUnit.N_CompanyID=@N_CompanyID and Inv_ItemUnit.N_ItemID =@N_ItemID and isnull(dbo.Inv_ItemUnit.B_InActive,0)=0 UNION Select Inv_ItemUnit.X_ItemUnit,Inv_ItemUnit.N_Qty,dbo.SP_SellingPrice(Inv_ItemUnit.N_ItemID,Inv_ItemUnit.N_CompanyID) as N_SellingPrice,Inv_ItemUnit.N_SellingPrice as N_UnitSellingPrice,Inv_ItemUnit.B_BaseUnit,Inv_ItemUnit.N_ItemUnitID,Inv_ItemMaster.N_PurchaseCost from Inv_ItemUnit inner join Inv_ItemMaster ON Inv_ItemUnit.N_ItemID=Inv_ItemMaster.N_ItemID and Inv_ItemUnit.N_CompanyID=Inv_ItemMaster.N_CompanyID where Inv_ItemUnit.X_ItemUnit=@X_ItemUnit and Inv_ItemUnit.N_CompanyID=@N_CompanyID and Inv_ItemUnit.N_ItemID =@N_ItemID and isnull(dbo.Inv_ItemUnit.B_InActive,0)=0";
    string sql ="select * from vw_InvItemWiseUnit_Disp where N_CompanyID=@N_CompanyID and N_ItemID =@N_ItemID and isnull(B_InActive,0)=0 and isnull(B_BaseUnit,0)=1 ";

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
                return Ok(api.Error(User,e));
            }
        }


        [HttpGet("itemUnitList")]
        public ActionResult GetItemUnitList( string baseUnit, int itemId)
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
                return Ok(api.Error(User,e));
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
                Results = dLayer.DeleteData("Inv_ItemUnit", "N_ItemUnitID", nItemUnitID, "",connection);
                }
                if (Results > 0)
                {
                    return Ok(api.Success( "Product Unit deleted"));
                }
                else
                {
                    return Ok(api.Warning("Unable to delete product Unit"));
                }

            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }


        }
    }
}