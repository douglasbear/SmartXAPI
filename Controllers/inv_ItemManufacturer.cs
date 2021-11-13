using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("itemManufacturer")]
    [ApiController]
    public class Inv_ItemManufacturer : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;



        public Inv_ItemManufacturer(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        //GET api/itemManufacturer/list?....
        [HttpGet("list")]
        public ActionResult GetItemCategory()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from Inv_ItemManufacturer where N_CompanyID=@p1 ";
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
                    return Ok(_api.Warning("No Results Found"));
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

       
         [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable;
                MasterTable = ds.Tables["master"];
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_CompanyID"].ToString());
              
                int nManufacturerID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ItemManufacturerID"].ToString());
              

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                  
                    nManufacturerID = dLayer.SaveData("Inv_ItemManufacturer", "N_ItemManufacturerID", MasterTable, connection, transaction);
                    if (nManufacturerID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Brand Master Created"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(User,ex));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nManufacturerID)
        {
             int Results=0;
            try
            {
              using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                Results=dLayer.DeleteData("Inv_ItemManufacturer","N_ItemManufacturerID",nManufacturerID,"",connection);
                if(Results>0){
                    return Ok(_api.Success("Deleted Sucessfully" ));
                }else{
                    return Ok(_api.Warning("Unable to delete" ));
                }
                }
                
            }
            catch (Exception ex)
                {
                    return Ok(_api.Error(User,ex));
                }
        }

          [HttpGet("Details") ]
        public ActionResult GetManufacturerDetails (int nManufacturerID,int nCompanyID)
          
        {   DataTable dt=new DataTable();
            SortedList Params = new SortedList();
           //  int nCompanyID=myFunctions.GetCompanyID(User);
               string sqlCommandText="select * from Inv_ItemManufacturer where N_CompanyID=@nCompanyID  and N_ItemManufacturerID=@nManufacturerID ";
               Params.Add("@nCompanyID",nCompanyID);
               Params.Add("@nManufacturerID",nManufacturerID);
            
            try{
                    using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(_api.Notice("No Results Found"));
                        }else{
                            return Ok(_api.Success(dt));
                        }
                
            }catch(Exception e){
                return Ok(_api.Error(User,e));
            }   
        }
    }
}