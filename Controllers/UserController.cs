using SmartxAPI.Data;
using SmartxAPI.Dtos.Login;
using Microsoft.AspNetCore.Mvc;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartxAPI.Controllers
{

    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ISec_UserRepo _repository;
        private readonly IApiFunctions _api;

        private readonly IMyFunctions myFunctions;
        private readonly IDataAccessLayer dLayer;
        private readonly string connectionString;

        public UserController(ISec_UserRepo repository, IApiFunctions api, IMyFunctions myFun, IDataAccessLayer dl, IConfiguration conf)
        {
            _repository = repository;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }
        [HttpPost("login")]
        public ActionResult Authenticate([FromBody] Sec_AuthenticateDto model)
        {
            try
            {
                string ipAddress = "";
                if (Request.Headers.ContainsKey("X-Forwarded-For"))
                    ipAddress = Request.Headers["X-Forwarded-For"];
                else
                    ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                //var password = myFunctions.EncryptString(model.Password);
                var password = model.Password;
                var user = _repository.Authenticate(model.CompanyName, model.Username, password, ipAddress);

                if (user == null) { return StatusCode(403, _api.Response(403, "Username or password is incorrect")); }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.ErrorResponse(ex));
            }
        }

        //GET api/User/list?....
        [HttpGet("list")]
        public ActionResult GetUserList(int? nCompanyId, bool bAllBranchesData, string userid, string qry)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string criteria = "";
            // if (userid != "" && userid != null)
            // {
            //     criteria = " and N_UserID =@userid ";
            //     Params.Add("@userid", userid);
            // }

            // string qryCriteria = "";
            // if (qry != "" && qry != null)
            // {
            //     Params.Add("@qry", "%" + qry + "%");
            //     qryCriteria = " and ([X_userID] like @qry or [X_UserName] like @qry ) ";

            // }
            string sqlCommandText = "Sp_UserList";
            Params.Add("N_CompanyID", nCompanyId);
            Params.Add("N_UserId", userid);
            try
            {
                dt = dLayer.ExecuteDataTablePro(sqlCommandText, Params);
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                    return StatusCode(200, new { StatusCode = 200, Message = "No Results Found" });
                else
                {
                    dt.Columns.Remove("X_Password");
                    dt.AcceptChanges();
                    return Ok(dt);
                }

            }
            catch (Exception e)
            {
                return StatusCode(403, _api.ErrorResponse(e));
            }
        }
        //Save....
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
                    SqlTransaction transaction;


                    transaction = connection.BeginTransaction();
                    int Result = dLayer.SaveData("Sec_User", "n_UserID", 0, MasterTable, connection, transaction);
                    if (Result > 0)
                    {
                        //MULTI COMPANY USER CREATION
                    }
                    else
                    {
                        transaction.Rollback();
                        return StatusCode(403, "Error");
                    }
                    transaction.Commit();
                }
                return Ok("User Saved");
            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.ErrorResponse(ex));
            }
        }
        [HttpGet("all")]
        public ActionResult GetCustomer(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from Sec_user";
            

            try{
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params);
                     if(dt.Rows.Count==0)
                    {
                       return StatusCode(200,new { StatusCode = 200 , Message= "No Results Found" });
                    }else{
                    return Ok(dt);

                    }
                }catch(Exception e){
                    return StatusCode(403,_api.ErrorResponse(e));
                }
        }
    }
}