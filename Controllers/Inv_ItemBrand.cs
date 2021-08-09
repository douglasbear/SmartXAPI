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
    [Route("itemBrand")]
    [ApiController]
    public class Inv_ItemBrand : ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;



        public Inv_ItemBrand(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        //GET api/productcategory/list?....
        [HttpGet("list")]
        public ActionResult GetItemCategory()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId=myFunctions.GetCompanyID(User);

            string sqlCommandText = "select * from Inv_ItemBrand where N_CompanyID=@p1 ";
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
                return Ok(_api.Error(e));
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
              
                int nBrandID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ItemBrandID"].ToString());
              

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                  
                    nBrandID = dLayer.SaveData("Inv_ItemBrand", "N_ItemBrandID", MasterTable, connection, transaction);
                    if (nBrandID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
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
                return BadRequest(_api.Error(ex));
            }
        }
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nBrandID)
        {
             int Results=0;
            try
            {
              using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                Results=dLayer.DeleteData("Inv_ItemBrand","N_ItemBrandID",nBrandID,"",connection);
                if(Results>0){
                    return Ok(_api.Success("Brand deleted" ));
                }else{
                    return Ok(_api.Warning("Unable to delete Brand" ));
                }
                }
                
            }
            catch (Exception ex)
                {
                    return Ok(_api.Error(ex));
                }
        }

          [HttpGet("Details") ]
        public ActionResult GetBrandDetails (int nBrandID,int nCompanyID)
          
        {   DataTable dt=new DataTable();
            SortedList Params = new SortedList();
           //  int nCompanyID=myFunctions.GetCompanyID(User);
               string sqlCommandText="select * from Inv_ItemBrand where N_CompanyID=@nCompanyID  and N_ItemBrandID=@nBrandID ";
               Params.Add("@nCompanyID",nCompanyID);
               Params.Add("@nBrandID",nBrandID);
            
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
                return Ok(_api.Error(e));
            }   
        }
    }
}