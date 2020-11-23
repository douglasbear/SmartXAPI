using SmartxAPI.Data;
using SmartxAPI.Dtos.Login;
using Microsoft.AspNetCore.Mvc;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
                var password = myFunctions.EncryptString(model.Password);
                //var password = model.Password;
                var user = _repository.Authenticate(model.CompanyName, model.Username, password, ipAddress,model.AppType);

                if (user == null) { return Ok(_api.Warning("Username or password is incorrect")); }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(ex));
            }
        }

        //GET api/User/list?....
        [HttpGet("list")]
        public ActionResult GetUserList(int? nCompanyId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            
            string sqlCommandText = "Sp_UserList";
            Params.Add("N_CompanyID", nCompanyId);
            // Params.Add("N_UserId", userid);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTablePro(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                    return Ok(_api.Warning("No Results Found"));
                else
                {
                    dt.Columns.Remove("X_Password");
                    dt.AcceptChanges();
                    return Ok(_api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return StatusCode(403, _api.Error(e));
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
                    int Result = dLayer.SaveData("Sec_User", "n_UserID", MasterTable, connection, transaction);
                    if (Result > 0)
                    {
                        //MULTI COMPANY USER CREATION
                    }
                    else
                    {
                        transaction.Rollback();
                         return Ok(_api.Error("Unable to save"));
                    }
                    transaction.Commit();
                }
                 return Ok(_api.Success("User Saved"));
            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.Error(ex));
            }
        }
        [HttpGet("all")]
        public ActionResult GetCustomer(int? nCompanyId,int nFnYearId)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            
            string sqlCommandText="select * from Sec_user";
            

            try{
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection);
                }
                     if(dt.Rows.Count==0)
                    {
                       return Ok(_api.Warning("No Results Found"));
                    }else{
                    return Ok(_api.Success(dt));

                    }
                }catch(Exception e){
                    return BadRequest(_api.Error(e));
                }
        }
         [HttpGet("details")]
        public ActionResult GetUserListDetails(int? nCompanyId,string xUser)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            
            string sqlCommandText = "Sp_UserList";
            Params.Add("N_CompanyID", nCompanyId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTablePro(sqlCommandText, Params, connection);
                }
                foreach (DataRow dtRow in dt.Rows)
                    {
                        if(dtRow["x_UserID"].ToString()!=xUser)
                        {
                            dtRow.Delete();
                        }
                    }

                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                    return Ok(_api.Warning("No Results Found"));
                else
                {
                    dt.Columns.Remove("X_Password");
                    dt.AcceptChanges();
                    return Ok(_api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return StatusCode(403, _api.Error(e));
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nUserId)
            {
                int Results = 0;
                SortedList Params = new SortedList();
                
                int nCompanyID = myFunctions.GetCompanyID(User);
                string sqlCategory="Select X_UserCategory from Sec_User  inner join Sec_UserCategory on Sec_User.N_UserCategoryID = Sec_UserCategory.N_UserCategoryID where Sec_User.X_UserID=@p3 and Sec_User.N_CompanyID =@p1";
                string sqlTrans="select COUNT(*) from vw_UserTransaction where n_userid=@p2";
                string sqlUser="select X_UserID from sec_user where n_userid=@p2";
                Params.Add("@p1", nCompanyID);
                Params.Add("@p2", nUserId);
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                    connection.Open();
                    object User = dLayer.ExecuteScalar(sqlUser, Params, connection);
                    Params.Add("@p3", User.ToString());
                    object Category = dLayer.ExecuteScalar(sqlCategory, Params, connection);
                    if(Category==null)
                        return Ok(_api.Error("Unable to delete User"));
                    else if (Category.ToString() == "Olivo" || Category.ToString().ToLower() == "administrator")
                        return Ok(_api.Error("Unable to delete User"));
                    else
                        {
                            int N_CountTransUser=0;
                            object CountTransUser = dLayer.ExecuteScalar(sqlTrans, Params, connection);
                            N_CountTransUser = myFunctions.getIntVAL(CountTransUser.ToString());
                            if (N_CountTransUser > 0)
                                return Ok(_api.Error("Unable to delete User"));
                        }

                    Results = dLayer.DeleteData("sec_User", "N_UserId", nUserId, "",connection);
            
                    if (Results > 0)
                    {
                        return Ok(_api.Success("User deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete User"));
                    }

                }
                }
                catch (Exception ex)
                {
                    return Ok(_api.Error(ex));
                }


            }
    }
}