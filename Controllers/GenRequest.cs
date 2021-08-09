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
using System.IO;
using System.Threading.Tasks;

namespace SmartxAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("general")]
    [ApiController]



    public class GenDefults : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;


        public GenDefults(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }


        //GET api/Projects/list
        [HttpGet("defults/{type}")]
        public ActionResult GetDefults(int type, string X_TypeCode)
        {
            int id = type;
           
            string X_Criteria = "N_DefaultId=@p1";
            SortedList param = new SortedList() { { "@p1", id } };
            if (X_TypeCode != "" && X_TypeCode!= null)
            {
                X_Criteria = X_Criteria + " and X_TypeCode=@p2";
                param.Add("@p2", X_TypeCode);
            }

            DataTable dt = new DataTable();

            string sqlCommandText = "select * from Gen_Defaults where " + X_Criteria;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, param, connection);
                }
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

        [HttpGet("lookup/{type}")]
        public ActionResult GetLookup(int type)
        {
            int N_FormID = type;

            string X_Criteria = " Sub.N_ReferId=@p1 and  Sub.N_CompanyID=@nCompanyID order by  Sub.n_Sort ASC";

            SortedList param = new SortedList() { { "@p1", N_FormID },{"@nCompanyID",myFunctions.GetCompanyID(User)} };

            DataTable dt = new DataTable();

            string sqlCommandText = "SELECT Sub.*, Parent.X_Name AS X_ParentName FROM Gen_LookupTable AS Sub LEFT OUTER JOIN Gen_LookupTable AS Parent ON Sub.N_CompanyID = Parent.N_CompanyID AND Sub.N_ParentGroupID = Parent.N_PkeyId where " + X_Criteria;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, param, connection);
                }
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




        [HttpGet("file")]
        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SortedList param = new SortedList();
                    param.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                    DataTable tblDetails = dLayer.ExecuteDataTable("select ISNULL(X_Value,'') AS X_Value from Gen_Settings where X_Description ='EmpDocumentLocation' and N_CompanyID =@nCompanyID", param, connection);
                    path = tblDetails.Rows[0]["X_Value"].ToString();
                }


            }
            catch (Exception e)
            {
                return Ok(api.Error(e));
            }
            path = path + filename;

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, api.GetContentType(path), Path.GetFileName(path));
        }



    }

}