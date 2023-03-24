using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartxAPI.GeneralFunctions;
using System;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("rentalunit")]
    [ApiController]
    public class Inv_RentalUnit : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 1686;
        public Inv_RentalUnit(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetRentalUnitList()
        {
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable(); 
            SortedList Params = new SortedList();
            string sqlCommandText = "select * from Inv_RentalUnit where N_CompanyID=@p1 order by N_RentalUnitID";  
          
            Params.Add("@p1", nCompanyID);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                if (dt.Rows.Count == 0)
                {
                    return Ok(_api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok( _api.Error(User,e));
            }
        }

        [HttpGet("dashboardList")]
        public ActionResult GetRentalUnitDashboardList(int nPage,int nSizeperpage, string xSearchkey, string xSortBy,int nRentalUnitID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                    string sqlCommandCount = "", xCriteria = "";
                    int Count = (nPage - 1) * nSizeperpage;
                    string sqlCommandText = "";
                    string criteria = "";
                    string cndn = "";
                    string Searchkey = "";
                    Params.Add("@p1", nCompanyID);

                   if (xSearchkey != null && xSearchkey.Trim() != "")
                    Searchkey = "and (N_RentalUnitID like '%" + xSearchkey + "%' OR X_RentalUnit like '%" + xSearchkey + "%')";
                   
                   if (xSortBy == null || xSortBy.Trim() == "")
                    xSortBy = " order by N_RentalUnitID desc";

                   if (Count == 0)
                    sqlCommandText = "select top(" + nSizeperpage + ") * from Inv_RentalUnit where N_CompanyID=@p1 and ISNULL(N_ItemID,0)=0"+ criteria + cndn + Searchkey + " " + xSortBy;
                   else
                    sqlCommandText = "select top(" + nSizeperpage + ") * from Inv_RentalUnit where N_CompanyID=@p1 and ISNULL(N_ItemID,0)=0 and N_RentalUnitID not in (select top(" + Count + ") N_RentalUnitID from Inv_RentalUnit where N_CompanyID=@p1 and ISNULL(N_ItemID,0)=0 )"+ criteria + cndn + Searchkey + " " + xSortBy ;


                    SortedList OutPut = new SortedList();

                    dt = dLayer.ExecuteDataTable(sqlCommandText , Params, connection);
                    sqlCommandCount = "select count(1) as N_Count  from Inv_RentalUnit where N_CompanyID=@p1 and ISNULL(N_ItemID,0)=0";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(_api.Warning("No Results Found"));
                    }
                    else
                    {
                        return Ok(_api.Success(OutPut));
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(_api.Error(User, e));
            }
        }

        [HttpGet("details") ]
        public ActionResult GetRentalUnitDetails (int nRentalUnitID)
        {   
            int nCompanyID = myFunctions.GetCompanyID(User);
            DataTable dt=new DataTable();
            SortedList Params = new SortedList();
            
            string sqlCommandText="select * from Inv_RentalUnit where N_CompanyID=@p1 and N_RentalUnitID=@p2 ";
            Params.Add("@p1",nCompanyID);
            Params.Add("@p2",nRentalUnitID);
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                if(dt.Rows.Count==0)
                {
                    return Ok(_api.Notice("No Results Found"));
                } else {
                    return Ok(_api.Success(dt));
                }
            } 
            catch(Exception e)
            {
                return Ok(_api.Error(User,e));
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
                    string x_RentalUnit= MasterTable.Rows[0]["x_RentalUnit"].ToString();
                    string DupCriteria = "N_CompanyID=" + myFunctions.GetCompanyID(User) + " and X_RentalUnit='" + x_RentalUnit + "'";
                    int N_RentalUnitID = dLayer.SaveData("Inv_RentalUnit", "N_RentalUnitID",DupCriteria,"", MasterTable, connection, transaction);
                    if (N_RentalUnitID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("Rental Unit Already Exist"));
                    }
                    else
                    {
                        transaction.Commit();
                    }
                    return Ok(_api.Success("Rental Unit Created"));
                }
            }

            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteData(int nRentalUnitID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                Results = dLayer.DeleteData("Inv_RentalUnit", "N_RentalUnitID", nRentalUnitID, "",connection);
                }
                if (Results > 0)
                {
                    return Ok(_api.Success( "Rental Unit deleted"));
                }
                else
                {
                    return Ok(_api.Warning("Unable to delete rental Unit"));
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }
    }
}