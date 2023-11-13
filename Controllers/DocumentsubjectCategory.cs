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

namespace SmartxAPI.Controllers

{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("documentsubjectcategory")]
    [ApiController]
    public class DocSubjectCategory: ControllerBase
    {
        private readonly IApiFunctions _api;
        private readonly IDataAccessLayer dLayer;
        private readonly int FormID;
        private readonly IMyFunctions myFunctions;
          public DocSubjectCategory(IApiFunctions api, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            FormID =1825 ;
        }
        private readonly string connectionString;

              
  [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    DataTable MasterTable;
                    MasterTable = ds.Tables["Master"];
                    DataRow MasterRow = MasterTable.Rows[0];
                    SortedList Params = new SortedList();
                    int nCompanyID = myFunctions.GetCompanyID(User);
                  
                    int n_CategoryID = myFunctions.getIntVAL(MasterRow["N_CategoryID"].ToString());
                    int nCategoryID=0;
                    string CityCode = "";
                    int nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    var values = MasterTable.Rows[0]["x_CategoryCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.FormID);
                      
                        CityCode = dLayer.GetAutoNumber("Inv_AttachmentCategory", "x_CategoryCode", Params, connection, transaction);
                        if (CityCode == "") { transaction.Rollback();
                        return Ok(_api.Error(User,"Unable to generate City Code")); }
                        MasterTable.Rows[0]["x_CategoryCode"] = CityCode;
                    }
                    MasterTable.Columns.Remove("n_FnYearID");
                    if (n_CategoryID>0)
                    {
                        
                         dLayer.DeleteData("Inv_AttachmentCategory","N_CategoryID", n_CategoryID, "", connection,transaction);

                    }

                    nCategoryID = dLayer.SaveData("Inv_AttachmentCategory", "N_CategoryID", MasterTable, connection, transaction);
                    if (nCategoryID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Warning("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Category Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }

         
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nCategoryID,int nCompanyID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    
                    connection.Open();
                    Results = dLayer.DeleteData("Inv_AttachmentCategory", "N_CategoryID", nCategoryID, "N_CompanyID =" + nCompanyID,  connection);
                    if (Results > 0)
                    {
                    
                        
                        return Ok(_api.Success("Catogry deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error(User,"Unable to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User,ex));
            }
        }

           [HttpGet("details")]
        public ActionResult docSubjectCategory(int nCategoryID)
        {


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataSet dt = new DataSet();
                    SortedList Params = new SortedList();
                    DataTable MasterTable = new DataTable();
                    DataTable DataTable = new DataTable();
                
                    string DetailSql = "";

                    Params.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    Params.Add("@nCategoryID",nCategoryID);
                   

                    DetailSql = "select * from Inv_AttachmentCategory where N_CompanyId=@nCompanyID and N_CategoryID=@nCategoryID ";
                    MasterTable = dLayer.ExecuteDataTable(DetailSql, Params, connection);
                    MasterTable = _api.Format(MasterTable, "Master");
                    dt.Tables.Add(MasterTable);
                    return Ok(_api.Success(dt));


                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User,e));
            }
        }


    }
}