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
using System.Security.Cryptography;
using System.Net.Mail;
using System.Text.RegularExpressions;

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
        private readonly string masterDBConnectionString;

        private readonly string seperator;

        public UserController(ISec_UserRepo repository, IApiFunctions api, IMyFunctions myFun, IDataAccessLayer dl, IConfiguration conf)
        {
            _repository = repository;
            _api = api;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
            masterDBConnectionString = conf.GetConnectionString("OlivoClientConnection");
            seperator = "$e$-!";
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
                var user = _repository.Authenticate(model.CompanyName, model.Username, password, ipAddress, model.AppID, 0);

                if (user == null) { return Ok(_api.Warning("Username or password is incorrect")); }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("logout")]
        public ActionResult Logout(int nFnYearID, int nBranchID)
        {


            int nLoginID = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string ipAddress = "";
                    if (Request.Headers.ContainsKey("X-Forwarded-For"))
                        ipAddress = Request.Headers["X-Forwarded-For"];
                    else
                        ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

                    SortedList logParams = new SortedList()
                    {
                        {"N_CompanyID",myFunctions.GetCompanyID(User)},
                        {"N_FnyearID",nFnYearID},
                        {"N_BranchId",nBranchID},
                        {"N_ActionID",2},
                        {"N_Type",1},
                        {"N_LoggedInID",myFunctions.GetLoginID(User)},
                        {"X_SystemName",ipAddress},
                        {"N_UserID",myFunctions.GetUserID(User)}
                    };
                    nLoginID = myFunctions.getIntVAL(dLayer.ExecuteScalarPro("SP_LoginDetailsInsert_Cloud", logParams, connection).ToString());

                }

                return Ok(_api.Success("LogOut"));


            }
            catch (Exception e)
            {
                return StatusCode(403, _api.Error(User, e));
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("list")]
        public ActionResult GetUserList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
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
                return StatusCode(403, _api.Error(User, e));
            }
        }

        [HttpGet("dashboardList")]
        public ActionResult GetUserList(int? nCompanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy,int userId)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";
            string exclude = " and X_UserID<>'Olivo' and X_Email LIKE '_%@__%.__%'";
            string criteria ="";


           
            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (X_UserID like '%" + xSearchkey + "%' or X_UserCategory like '%" + xSearchkey + "%' or X_BranchName like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_UserID desc";
            else
                xSortBy = " order by " + xSortBy;

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", userId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                     object usertypeID = dLayer.ExecuteScalar("SELECT isnull(N_TypeID,0)  FROM sec_User where n_UserID=@p2 and N_CompanyID=@p1 ", Params, connection);
                     if (usertypeID == null) usertypeID = 0;
                     if (myFunctions.getIntVAL(usertypeID.ToString()) > 0  )
                            {
                           if (myFunctions.getIntVAL(usertypeID.ToString()) ==1)
                           {
                            criteria="";
                           }
                            else
                           {
                            criteria=" and N_TypeID in (select N_TypeID from sec_User where  N_CompanyID=@p1 and N_TypeID>="+usertypeID+")";
                           }

                            }
               if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_UserList where N_CompanyID=@p1 " + exclude +criteria+ Searchkey + " " + xSortBy;
              else
                sqlCommandText = "select top(" + nSizeperpage + ") * from vw_UserList where N_CompanyID=@p1 " + exclude + criteria+Searchkey + " and N_UserID not in(select top(" + Count + ")  N_UserID from vw_UserList where N_CompanyID=@p1 " + exclude +criteria+ xSortBy + " ) " + xSortBy;

          
            SortedList OutPut = new SortedList();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);

                    dt = myFunctions.AddNewColumnToDataTable(dt, "X_UserCategoryNameList", typeof(string), 0);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string X_UserCategoryNameList = "";
                        string sqlText = "Select X_UserCategory From Sec_UserCategory Where N_UserCategoryID in(" + dt.Rows[i]["X_UserCategoryList"].ToString() + ") and N_CompanyID=@p1";
                        DataTable dtCategory = dLayer.ExecuteDataTable(sqlText, Params, connection);
                        for (int j = 0; j < dtCategory.Rows.Count; j++)
                        {
                            if (X_UserCategoryNameList == "")
                                X_UserCategoryNameList = dtCategory.Rows[j]["X_UserCategory"].ToString();
                            else
                                X_UserCategoryNameList += "," + dtCategory.Rows[j]["X_UserCategory"].ToString();
                        }
                        dt.Rows[i]["X_UserCategoryNameList"] = X_UserCategoryNameList.ToString();
                    }
                    dt.AcceptChanges();

                    sqlCommandCount = "select count(1) as N_Count from vw_UserList where N_CompanyID=@p1  " + exclude +criteria+ Searchkey + "";
                    object TotalCount = dLayer.ExecuteScalar(sqlCommandCount, Params, connection);
                    OutPut.Add("Details", _api.Format(dt));
                    OutPut.Add("TotalCount", TotalCount);
                    dt = _api.Format(dt);
                    if (dt.Rows.Count == 0)
                    {
                        // return Ok(_api.Warning("No Results Found"));
                        return Ok(_api.Success(OutPut));
                    }
                    else
                    {
                        dt.Columns.Remove("X_Password");
                        dt.AcceptChanges();
                        return Ok(_api.Success(OutPut));
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        //Save....
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("save")]
        public ActionResult SaveData([FromBody] DataSet ds)
        {
            try
            {
                DataTable MasterTable, globalUser,apps;
                MasterTable = ds.Tables["master"];
                globalUser = ds.Tables["globalUser"];
                apps = ds.Tables["apps"];

                int nFnYearID = 0;
                if (MasterTable.Columns.Contains("n_FnYearID"))
                {
                    nFnYearID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearID"].ToString());
                    MasterTable.Columns.Remove("n_FnYearID");
                }
                int nBranchID = myFunctions.getIntVAL(MasterTable.Rows[0]["N_BranchID"].ToString());

                int nClientID = myFunctions.GetClientID(User);
                int globalUserID, userID, nUserID, nGlobalUserID = 0;
                bool bSalesPerson = false;

                string exclude = " and X_UserID<>'Olivo' and X_Email LIKE '_%@__%.__%'";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction;


                    using (SqlConnection olivoCon = new SqlConnection(masterDBConnectionString))
                    {
                        olivoCon.Open();
                        SqlTransaction olivoTxn = olivoCon.BeginTransaction();
                        nUserID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_UserID"].ToString());


                        if (MasterTable.Columns.Contains("b_Salesperson"))
                        {
                            bSalesPerson = myFunctions.getBoolVAL(MasterTable.Rows[0]["b_Salesperson"].ToString());
                            MasterTable.Columns.Remove("b_Salesperson");
                        }


                        nGlobalUserID = myFunctions.getIntVAL(globalUser.Rows[0]["n_UserID"].ToString());


                        transaction = connection.BeginTransaction();
                        SortedList userParams = new SortedList();
                        userParams.Add("@nClientID", nClientID);
                        userParams.Add("@nAppID", MasterTable.Rows[0]["n_AppID"].ToString());
                        userParams.Add("@xUserID", MasterTable.Rows[0]["x_UserID"].ToString());
                        userParams.Add("@nCompanyID", myFunctions.GetCompanyID(User));
                        userParams.Add("@nGlobalUserID", globalUser.Rows[0]["n_UserID"].ToString());
                        userParams.Add("@xEmailID", globalUser.Rows[0]["x_EmailID"].ToString());





                        string skipUserSql = "";

                        if (nGlobalUserID == 0)
                        {
                            object glogalUserID = dLayer.ExecuteScalar("SELECT N_UserID FROM Users where x_EmailID=@xEmailID and N_ClientID=@nClientID " + skipUserSql, userParams, olivoCon, olivoTxn);
                            if (glogalUserID == null) glogalUserID = 0;
                            if (myFunctions.getIntVAL(glogalUserID.ToString()) > 0)
                            {
                                nGlobalUserID = myFunctions.getIntVAL(glogalUserID.ToString());
                                userParams["@nGlobalUserID"] = nGlobalUserID;
                                globalUser.Rows[0]["n_UserID"] = nGlobalUserID;

                                object glogalXUserID = dLayer.ExecuteScalar("SELECT X_UserID FROM Users where x_EmailID=@xEmailID and N_ClientID=@nClientID " + skipUserSql, userParams, olivoCon, olivoTxn);
                                if (glogalXUserID != null)
                                {
                                    if (glogalXUserID.ToString() != MasterTable.Rows[0]["x_UserID"].ToString())
                                    {
                                        return Ok(_api.Warning("user with this email id already exists"));
                                    }

                                    object userCountObj = dLayer.ExecuteScalar("SELECT Count(Sec_User.N_CompanyID) as Count FROM Sec_User LEFT OUTER JOIN Acc_Company ON Sec_User.N_CompanyID = Acc_Company.N_CompanyID  where Sec_User.X_UserID=@xUserID and Acc_Company.N_ClientID=@nClientID and Acc_Company.N_CompanyID=@nCompanyID", userParams, connection, transaction);
                                    if (myFunctions.getIntVAL(userCountObj.ToString()) > 0)
                                    {
                                        return Ok(_api.Warning("The user is already registered and part this company."));
                                    }
                                }



                            }
                        }

                        if (nGlobalUserID != 0)
                        {
                            skipUserSql = " and N_UserID<>@nGlobalUserID";

                        }
                        object userCountWithUserID = dLayer.ExecuteScalar("SELECT Count(N_UserID) as Count FROM Users where X_UserID=@xUserID " + skipUserSql, userParams, olivoCon, olivoTxn);
                        if (myFunctions.getIntVAL(userCountWithUserID.ToString()) > 0)
                        {
                            return Ok(_api.Warning("user with this user id already exists"));
                        }

                        object userCountWithEmailID = dLayer.ExecuteScalar("SELECT Count(N_UserID) as Count FROM Users where x_EmailID=@xEmailID " + skipUserSql, userParams, olivoCon, olivoTxn);
                        if (myFunctions.getIntVAL(userCountWithEmailID.ToString()) > 0)
                        {
                            return Ok(_api.Warning("user with this email id already exists"));
                        }
                        object nUserLimit =dLayer.ExecuteScalar("select isnull(N_Value,0) from GenSettings where N_ClientID="+nClientID+" and X_Description='USER LIMIT' ", olivoCon, olivoTxn);
                         if(nUserLimit==null){nUserLimit="0";}
                        object nUserCount = dLayer.ExecuteScalar("SELECT Count(N_UserID) as Count FROM Users where N_ClientID=@nClientID and N_UserType=1", userParams, olivoCon, olivoTxn);
                        
                        if (nGlobalUserID == 0)
                            if (myFunctions.getIntVAL(nUserLimit.ToString()) <= myFunctions.getIntVAL(nUserCount.ToString()))
                            {
                                return Ok(_api.Warning("User limit exeeded !!"));
                            }

                        if (nGlobalUserID > 0 && nUserID == 0)
                        {
                            object userCountinOtherOrg = dLayer.ExecuteScalar("SELECT Count(Sec_User.N_CompanyID) as Count FROM Sec_User LEFT OUTER JOIN Acc_Company ON Sec_User.N_CompanyID = Acc_Company.N_CompanyID  where Sec_User.X_UserID=@xUserID and Acc_Company.N_ClientID<>@nClientID", userParams, connection, transaction);
                            if (myFunctions.getIntVAL(userCountinOtherOrg.ToString()) > 0)
                            {
                                return Ok(_api.Warning("The user id is already registered with another account which is not a part of this organization."));
                            }

                            object userCountinThisCompany = dLayer.ExecuteScalar("SELECT Count(Sec_User.N_CompanyID) as Count FROM Sec_User LEFT OUTER JOIN Acc_Company ON Sec_User.N_CompanyID = Acc_Company.N_CompanyID  where Sec_User.X_UserID=@xUserID and Acc_Company.N_ClientID=@nClientID and Acc_Company.N_CompanyID=@nCompanyID", userParams, connection, transaction);
                            if (myFunctions.getIntVAL(userCountinThisCompany.ToString()) > 0)
                            {
                                return Ok(_api.Warning("The user is already registered and part this company."));
                            }
                        }

                        MasterTable.Columns.Add("x_Password", typeof(System.String));
                        DataRow MasterRow = MasterTable.Rows[0];

                        object xPwd = ".";
                        object nUserType;
                        if (nGlobalUserID > 0)
                        {
                            xPwd = dLayer.ExecuteScalar("SELECT X_Password FROM Users where x_EmailID=@xEmailID and N_ClientID=@nClientID and N_UserID=@nGlobalUserID", userParams, olivoCon, olivoTxn);
                            globalUser.Rows[0]["n_ActiveAppID"] = apps.Rows[0]["n_AppID"].ToString();
                            nUserType =dLayer.ExecuteScalar("SELECT N_UserType FROM Users where x_EmailID=@xEmailID and N_ClientID=@nClientID and N_UserID=@nGlobalUserID", userParams, olivoCon, olivoTxn);
                            globalUser.Rows[0]["N_UserType"] = myFunctions.getIntVAL(nUserType.ToString());
                        }
                        else
                        {
                            MasterTable.Rows[0]["b_Active"] = 0;
                            globalUser.Rows[0]["b_Inactive"] = 1;
                            globalUser.Rows[0]["b_EmailVerified"] = 0;
                            globalUser.Rows[0]["n_ActiveAppID"] = apps.Rows[0]["n_AppID"].ToString();
                        }

                        MasterTable.Rows[0]["x_Password"] = xPwd;
                        globalUser.Rows[0]["x_Password"] = xPwd;



                        globalUserID = dLayer.SaveData("Users", "n_UserID", globalUser, olivoCon, olivoTxn);


                        if (globalUserID <= 0)
                        {
                            transaction.Rollback();
                            olivoTxn.Rollback();
                            return Ok(_api.Error(User, "Unable to invite user"));
                        }
                        // object userCount = dLayer.ExecuteScalar("Select count(1) from Sec_User where n_UserID=" + globalUserID, connection, transaction);
                        // if (myFunctions.getIntVAL(userCount.ToString()) > 0)
                        // {
                        //     transaction.Rollback();
                        //     olivoTxn.Rollback();
                        //     return Ok(_api.Error(User,"User Already Registerd With this ID !!!"));
                        // }
                        // MasterTable.Rows[0]["n_UserID"] = globalUserID;
                        if (MasterTable.Columns.Contains("n_AppID"))
                            MasterTable.Columns.Remove("n_AppID");
                        MasterTable.AcceptChanges();
                        userID = dLayer.SaveData("Sec_User", "n_UserID", MasterTable, connection, transaction);
                        if (userID > 0)
                        {
                          foreach (DataRow row in apps.Rows)
                          {
                            row["N_UserID"]=userID;
                            row["N_GlobalUserID"]=globalUserID;

                          }
                          apps.AcceptChanges();
                          if(myFunctions.getIntVAL(MasterTable.Rows[0]["n_UserID"].ToString())>0)
                          {
                         SortedList userParam= new SortedList();
                        userParam.Add("@userID", userID);
                        apps = myFunctions.AddNewColumnToDataTable(apps, "X_LandingPage", typeof(string), null);
                        string sqlText = "Select * From sec_userApps Where N_UserID=@userID";

                        DataTable duserApps = dLayer.ExecuteDataTable( sqlText,userParam,connection,transaction);
                        foreach (DataRow Rows in duserApps.Rows){
                            foreach (DataRow drow in apps.Rows){
                                if (myFunctions.getIntVAL(Rows["N_UserID"].ToString())==myFunctions.getIntVAL(drow["N_UserID"].ToString())&&myFunctions.getIntVAL(Rows["N_AppID"].ToString())==myFunctions.getIntVAL(drow["N_AppID"].ToString()))
                                {
                                    
                                    drow["X_LandingPage"]=Rows["X_LandingPage"];
                                }

                            }

                        }
                          apps.AcceptChanges();

                          dLayer.ExecuteNonQuery("DELETE FROM sec_userApps  WHERE N_UserID="+userID , connection, transaction);
                          }
                       
                          int userAppsID = dLayer.SaveData("sec_userApps", "n_AppMappingID", apps, connection, transaction);
                            if (bSalesPerson)
                            {

                                object salesManCount = dLayer.ExecuteScalar("select count(1) from Inv_SalesMan where N_CompanyID=" + myFunctions.GetCompanyID(User) + " and N_UserID=" + userID, connection, transaction);

                                if (salesManCount == null)
                                    salesManCount = 0;

                                if (myFunctions.getIntVAL(salesManCount.ToString()) == 0)
                                {
                                    object salesManMax = dLayer.ExecuteScalar("select max(N_SalesmanID)+1 from Inv_SalesMan ", connection, transaction);
                                    object salesManCodeMax = dLayer.ExecuteScalar("select max(cast(X_SalesManCode as numeric))+1 from Inv_SalesMan ", connection, transaction);
                                    object salesManSaved = dLayer.ExecuteScalar("insert into Inv_SalesMan (N_CompanyID,N_SalesManID,X_SalesmanCode,X_SalesmanName,X_Email,N_InvDueDays,N_CommnPerc,N_FnYearID,N_UserID,N_BranchID)values(" + myFunctions.GetCompanyID(User) + "," + (myFunctions.getIntVAL(salesManMax.ToString())) + ",'" + (myFunctions.getIntVAL(salesManCodeMax.ToString())).ToString() + "','" + MasterTable.Rows[0]["X_UserName"].ToString() + "','" + MasterTable.Rows[0]["X_UserID"].ToString() + "',0,0," + nFnYearID + "," + userID + "," + nBranchID + ")", connection, transaction);
                                }
                            }





                            //

                            if (nGlobalUserID == 0)
                            {
                                object appUrl = dLayer.ExecuteScalar("select X_AppUrl from ClientApps where N_ClientID=@nClientID and N_AppID=@nAppID", userParams, olivoCon, olivoTxn);
                                if (appUrl == null)
                                {
                                    transaction.Rollback();
                                    olivoTxn.Rollback();
                                    return Ok(_api.Error(User, "Unable to invite user - App Url Not Found"));
                                }
                                // format -> userid + email + clientid + companyid + companyUserID
                                string inviteCode = myFunctions.EncryptString(globalUserID.ToString()) + seperator + myFunctions.EncryptString(MasterTable.Rows[0]["x_UserID"].ToString()) + seperator + myFunctions.EncryptString(nClientID.ToString());
                                string userName = dLayer.ExecuteScalar("select X_UserName from Users where N_ClientID=@nClientID and N_UserID=" + myFunctions.GetGlobalUserID(User), userParams, olivoCon, olivoTxn).ToString();

                                string EmailBody = "<div style='font-size: 18px;font-weight: 400;width: 600px;margin: 0 auto;color: #2d2f36;font-family: sans-serif;'><span style='font-weight: 500;margin: 56px 0 20px;'><span style='color: #2c6af6;'> "
      + "Olivo Cloud Solutions"
         + "</span><h1 style='font-size: 32px;font-weight: 600;margin:40px 0 12px;'>"
           + " Welcome, " + MasterRow["x_UserName"].ToString()
          + "</h1><p style='margin: 0 0 24px;'>" + userName + " has invited you to join the " + myFunctions.GetCompanyName(User) + ". Join now to have access!"
          + "</p><a href='" + appUrl + "/verifyUser#" + inviteCode + "' style='text-decoration: none;display: block;width: max-content;font-size: 18px;margin: 0 auto 24px;padding: 20px 40px;color: #ffffff;border-radius: 4px;background-color: #2c6af6;'>Join Now</a><p style='margin: 24px 0 0 ;padding: 17px 0;text-align: center;background: #f4f5f6;color: #86898e;font-size: 14px;'>Copyright © 2021 Olivo Cloud Solutions, All rights reserved.</p></div>";
                                string EmailSubject = myFunctions.GetCompanyName(User) + " invites you to join their Olivo Cloud Solutions";
                                myFunctions.SendMail(MasterTable.Rows[0]["x_UserID"].ToString(), EmailBody, EmailSubject, dLayer, 1, 1, 1);
                            }
                            else if (nUserID == 0)
                            {

                                object appUrl = dLayer.ExecuteScalar("select X_AppUrl from ClientApps where N_ClientID=@nClientID and N_AppID=@nAppID", userParams, olivoCon, olivoTxn);
                                if (appUrl == null)
                                {
                                    transaction.Rollback();
                                    olivoTxn.Rollback();
                                    return Ok(_api.Error(User, "Unable to invite user - App Url Not Found"));
                                }
                                // format -> userid + email + clientid + companyid + companyUserID
                                string userName = dLayer.ExecuteScalar("select X_UserName from Users where N_ClientID=@nClientID and N_UserID=" + myFunctions.GetGlobalUserID(User), userParams, olivoCon, olivoTxn).ToString();

                                string EmailBody = "<div style='font-size: 18px;font-weight: 400;width: 600px;margin: 0 auto;color: #2d2f36;font-family: sans-serif;'><span style='font-weight: 500;margin: 56px 0 20px;'><span style='color: #2c6af6;'> "
      + "Olivo Cloud Solutions"
         + "</span><h1 style='font-size: 32px;font-weight: 600;margin:40px 0 12px;'>"
           + " Welcome, " + MasterRow["x_UserName"].ToString()
          + "</h1><p style='margin: 0 0 24px;'>" + userName + " has invited you to join the " + myFunctions.GetCompanyName(User) + ". Join now to have access!"
          + "</p><a href='" + appUrl + "/login" + "' style='text-decoration: none;display: block;width: max-content;font-size: 18px;margin: 0 auto 24px;padding: 20px 40px;color: #ffffff;border-radius: 4px;background-color: #2c6af6;'>Join Now</a><p style='margin: 24px 0 0 ;padding: 17px 0;text-align: center;background: #f4f5f6;color: #86898e;font-size: 14px;'>Copyright © 2021 Olivo Cloud Solutions, All rights reserved.</p></div>";
                                string EmailSubject = myFunctions.GetCompanyName(User) + " invites you to join their Olivo Cloud Solutions";
                                myFunctions.SendMail(MasterTable.Rows[0]["x_UserID"].ToString(), EmailBody, EmailSubject, dLayer, 1, 1, 1);

                            }
                        }
                        else
                        {
                            transaction.Rollback();
                            olivoTxn.Rollback();
                            return Ok(_api.Error(User, "Unable to invite user"));
                        }
                        transaction.Commit();
                        olivoTxn.Commit();

                    }
                }
                if (nGlobalUserID == 0 || nUserID == 0)
                    return Ok(_api.Success("User Invited"));
                else
                    return Ok(_api.Success("User Updated"));

            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.Error(User, ex));
            }
        }


        private string generateRefreshToken()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        [HttpGet("verifyUser")]
        public ActionResult VerifyUser(string verificationCode, string password)
        {
            // format -> userid + email + clientid + companyid + companyUserID
            string[] cred = verificationCode.Split(seperator);

            int globalUserID = myFunctions.getIntVAL(myFunctions.DecryptString(cred[0]));
            string emailID = myFunctions.DecryptString(cred[1]);
            int clientID = myFunctions.getIntVAL(myFunctions.DecryptString(cred[2]));

            SortedList userParams = new SortedList();
            userParams.Add("@nClientID", clientID);
            userParams.Add("@xUserID", emailID);
            userParams.Add("@nglobalUserID", globalUserID);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                using (SqlConnection olivoCon = new SqlConnection(masterDBConnectionString))
                {
                    olivoCon.Open();
                    SqlTransaction olivoTxn = olivoCon.BeginTransaction();
                    object globalCount = dLayer.ExecuteScalar("SELECT Count(N_UserID) as Count FROM Users where N_UserID=@nglobalUserID and N_ClientID=@nClientID and x_EmailID=@xUserID", userParams, olivoCon, olivoTxn);
                    if (myFunctions.getIntVAL(globalCount.ToString()) == 0 || myFunctions.getIntVAL(globalCount.ToString()) > 1)
                    {
                        transaction.Rollback();
                        olivoTxn.Rollback();
                        return Ok(_api.Warning("Invitation Code Expired"));
                    }
                    string Pwd = myFunctions.EncryptString(password);
                    userParams.Add("@xPassword", Pwd);
                    object companyCount = dLayer.ExecuteScalar("SELECT Count(N_UserID) as Count FROM Sec_User where x_UserID=@xUserID", userParams, connection, transaction);
                    if (myFunctions.getIntVAL(companyCount.ToString()) > 0)
                    {
                        int res = dLayer.ExecuteNonQuery("Update Sec_User set X_Password=@xPassword,B_Active=1 where x_UserID=@xUserID", userParams, connection, transaction);
                        if (res == 0)
                        {
                            transaction.Rollback();
                            olivoTxn.Rollback();
                            return Ok(_api.Warning("Invitation Code Expired"));
                        }
                    }

                    object nTypeID = dLayer.ExecuteScalar("SELECT n_TypeID  FROM Sec_User where x_UserID=@xUserID", userParams, connection, transaction);
                    if (myFunctions.getIntVAL(nTypeID.ToString()) == 1)
                    {
                         DateTime  validDateTime=DateTime.Now;
                       
                        object nPswdDuraHours  =  dLayer.ExecuteScalar("select isnull(N_PswdDuraHours,0) ASN_PswdDuraHours  from Users where  N_ClientID="+clientID+" and X_EmailID=@xUserID",userParams, olivoCon,olivoTxn);
                        if(myFunctions.getIntVAL(nPswdDuraHours.ToString())>0)
                        {
                          int daysToAdd=myFunctions.getIntVAL(nPswdDuraHours.ToString());
                          validDateTime= DateTime.Now.AddHours(daysToAdd);
                          
                        }
                        
                    dLayer.ExecuteNonQuery("Update Sec_User set D_ExpireDate='"+validDateTime+"' where x_UserID=@xUserID", userParams, connection, transaction);
                    }



                    int res2 = dLayer.ExecuteNonQuery("Update Users set X_Password=@xPassword,B_InActive=0,B_EmailVerified=1 where N_UserID=@nglobalUserID and N_ClientID=@nClientID and X_EmailID=@xUserID", userParams, olivoCon, olivoTxn);
                    if (res2 != 1)
                    {
                        transaction.Rollback();
                        olivoTxn.Rollback();
                        return Ok(_api.Warning("Invitation Code Expired"));
                    }
                    transaction.Commit();
                    olivoTxn.Commit();
                }
            }

            return Ok(_api.Success("User Verified , please login to continue"));
        }





        [HttpGet("forgotPassword")]
        public ActionResult VerifyUser(string emailID,string senderMail,int nPswdDuraHours,DateTime dPswdResetTime)
        {
            try
            {
                using (SqlConnection olivoCon = new SqlConnection(masterDBConnectionString))
                {
                    olivoCon.Open();
                    SortedList userParams = new SortedList();
                    userParams.Add("@xEmail", emailID);

                    object clientID = dLayer.ExecuteScalar("select N_ClientID from Users where X_EmailID=@xEmail", userParams, olivoCon);
                    object globalUserID = dLayer.ExecuteScalar("select N_UserID from Users where X_EmailID=@xEmail", userParams, olivoCon);
                    object userName = dLayer.ExecuteScalar("select X_UserName from Users where X_EmailID=@xEmail", userParams, olivoCon);
                    userParams.Add("@nClientID", clientID);

                    object appUrl = dLayer.ExecuteScalar("select Top 1 X_AppUrl from ClientApps where N_ClientID=@nClientID ", userParams, olivoCon);

                    if (appUrl == null)
                    {
                        return Ok(_api.Error(User, "Unable to request - App Url Not Found"));
                    }
                    // format -> userid + email + clientid 
                    string inviteCode = myFunctions.EncryptString(globalUserID.ToString()) + seperator + myFunctions.EncryptString(emailID) + seperator + myFunctions.EncryptString(clientID.ToString());

                    string EmailBody = "<div style='font-size: 18px;font-weight: 400;width: 600px;margin: 0 auto;color: #2d2f36;font-family: sans-serif;'><span style='font-weight: 500;margin: 56px 0 20px;'><span style='color: #2c6af6;'> "
+ "Forgot Your Password?"
+ "</span><h1 style='font-size: 32px;font-weight: 600;margin:40px 0 12px;'>"
+ "</h1><p style='margin: 0 0 24px;'> That's okay, it happens! Click on the button below to reset your password."
+ "</p><a href='" +appUrl+ "/verifyUser#" + inviteCode + "' style='text-decoration: none;display: block;width: max-content;font-size: 18px;margin: 0 auto 24px;padding: 20px 40px;color: #ffffff;border-radius: 4px;background-color: #2c6af6;'>Reset Your Password</a><p style='margin: 24px 0 0 ;padding: 17px 0;text-align: center;background: #f4f5f6;color: #86898e;font-size: 14px;'>Copyright © 2021 Olivo Tech., All rights reserved.</p></div>";
                    string EmailSubject = "Olivo Cloud Solutions - Reset Password";
                    if(senderMail!="")
                    {

                     myFunctions.SendMail(senderMail.ToString(), EmailBody, EmailSubject, dLayer, 1, 1, 1);
                     dLayer.ExecuteNonQuery("update users set N_PswdDuraHours="+nPswdDuraHours+" ,D_PswdResetTime='"+dPswdResetTime+"' where N_ClientID=" + clientID+ "and  X_EmailID=@xEmail", userParams,olivoCon); 
               
                    }
                    else{
                     myFunctions.SendMail(emailID.ToString(), EmailBody, EmailSubject, dLayer, 1, 1, 1);
                    }
                   

                }
                 return Ok(_api.Success("Password Reset Mail Send"));
            }
            catch (Exception ex)
            {
                return StatusCode(403, _api.Error(User, "An error occurred while sending mail"));
            }
        }


// [HttpGet("forgotPassword")]
//         public ActionResult GenerateOTP(string emailID,int nGlobalUserID)
//         {
//             try
//             {
//                 using (SqlConnection olivoCon = new SqlConnection(masterDBConnectionString))
//                 {
//                     olivoCon.Open();
//                     SortedList userParams = new SortedList();
//                     userParams.Add("@xEmail", emailID);

//                     object clientID = dLayer.ExecuteScalar("select N_ClientID from Users where X_EmailID=@xEmail", userParams, olivoCon);
//                     object globalUserID = dLayer.ExecuteScalar("select N_UserID from Users where X_EmailID=@xEmail", userParams, olivoCon);
//                     object userName = dLayer.ExecuteScalar("select X_UserName from Users where X_EmailID=@xEmail", userParams, olivoCon);
//                     userParams.Add("@nClientID", clientID);

//                     object appUrl = dLayer.ExecuteScalar("select Top 1 X_AppUrl from ClientApps where N_ClientID=@nClientID ", userParams, olivoCon);

//                     if (appUrl == null)
//                     {
//                         return Ok(_api.Error(User, "Unable to request - App Url Not Found"));
//                     }
//                     // format -> userid + email + clientid 
//                     string inviteCode = myFunctions.EncryptString(globalUserID.ToString()) + seperator + myFunctions.EncryptString(emailID) + seperator + myFunctions.EncryptString(clientID.ToString());

//                     string EmailBody = "<div style='font-size: 18px;font-weight: 400;width: 600px;margin: 0 auto;color: #2d2f36;font-family: sans-serif;'><span style='font-weight: 500;margin: 56px 0 20px;'><span style='color: #2c6af6;'> "
// + "Forgot Your Password?"
// + "</span><h1 style='font-size: 32px;font-weight: 600;margin:40px 0 12px;'>"
// + "</h1><p style='margin: 0 0 24px;'> That's okay, it happens! Click on the button below to reset your password."
// + "</p><a href='" +appUrl+ "/verifyUser#" + inviteCode + "' style='text-decoration: none;display: block;width: max-content;font-size: 18px;margin: 0 auto 24px;padding: 20px 40px;color: #ffffff;border-radius: 4px;background-color: #2c6af6;'>Reset Your Password</a><p style='margin: 24px 0 0 ;padding: 17px 0;text-align: center;background: #f4f5f6;color: #86898e;font-size: 14px;'>Copyright © 2021 Olivo Tech., All rights reserved.</p></div>";
//                     string EmailSubject = "Olivo Cloud Solutions - Reset Password";
//                     if(senderMail!="")
//                     {

//                      myFunctions.SendMail(senderMail.ToString(), EmailBody, EmailSubject, dLayer, 1, 1, 1);
//                      dLayer.ExecuteNonQuery("update users set N_PswdDuraHours="+nPswdDuraHours+" ,D_PswdResetTime='"+dPswdResetTime+"' where N_ClientID=" + clientID+ "and  X_EmailID=@xEmail", userParams,olivoCon); 
               
//                     }
//                     else{
//                      myFunctions.SendMail(emailID.ToString(), EmailBody, EmailSubject, dLayer, 1, 1, 1);
//                     }
                   

//                 }
//                  return Ok(_api.Success("Password Reset Mail Send"));
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(403, _api.Error(User, "An error occurred while sending mail"));
//             }
//         }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("all")]
        public ActionResult GetCustomer(int nFnYearId)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            string sqlCommandText = "select * from Sec_user";


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
                return BadRequest(_api.Error(User, e));
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("details")]
        public ActionResult GetUserListDetails(string xUser)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            DataTable multiApps = new DataTable();
            int nCompanyId = myFunctions.GetCompanyID(User);
            Params.Add("@xUserID", xUser);
          
            Params.Add("@nCompanyID", nCompanyId);
            string sqlCommandText = "select * from vw_UserList where x_UserID=@xUserID and N_CompanyID=@nCompanyID";
         
            string multiqry = "SELECT * from Sec_UserApps where n_UserID=@nUserID and N_CompanyID=@nCompanyID";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    object nUserID= dLayer.ExecuteScalar("select N_UserID from vw_UserList where X_UserID=@xUserID and N_CompanyID=@nCompanyID" ,Params, connection); 
                    Params.Add("@nUserID",  myFunctions.getIntVAL(nUserID.ToString()));
                    multiApps = dLayer.ExecuteDataTable(multiqry, Params, connection);
                    


                }

                using (SqlConnection olvConn = new SqlConnection(masterDBConnectionString))
                {
                    olvConn.Open();
                    object objUser = dLayer.ExecuteScalar("select N_UserID from Users where X_UserID=@xUserID", Params, olvConn);
                    if (objUser == null)
                        return Ok(_api.Warning("User Not Found"));

                    object objByPass = dLayer.ExecuteScalar("select B_ByPassTwoFactAuth from Users where X_UserID=@xUserID", Params, olvConn);
                    // object objAuthType = dLayer.ExecuteScalar("select N_TwoFactAuthType from ClientMaster where X_EmailID=@xUserID", Params, olvConn);

                    dt = myFunctions.AddNewColumnToDataTable(dt, "N_GlobalUserID", typeof(int), myFunctions.getIntVAL(objUser.ToString()));
                    dt = myFunctions.AddNewColumnToDataTable(dt, "B_ByPassTwoFactAuth", typeof(bool), myFunctions.getBoolVAL(objByPass.ToString()));
                    // dt = myFunctions.AddNewColumnToDataTable(dt, "N_TwoFactAuthType", typeof(int), myFunctions.getIntVAL(objAuthType.ToString()));

                    dt.AcceptChanges();
                }
                DataSet dataSet = new DataSet();
                multiApps = _api.Format(multiApps, "apps");
                dt = _api.Format(dt, "master");
                dataSet.Tables.Add(dt);
                dataSet.Tables.Add(multiApps);
                //dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                    return Ok(_api.Warning("No Results Found"));
                else
                {
                    dt.Columns.Remove("X_Password");

                    dt.AcceptChanges();
                    return Ok(_api.Success(dataSet));
                }

            }
            catch (Exception e)
            {
                return Ok(_api.Error(User, e));
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("delete")]
        public ActionResult DeleteData(int nUserId)
        {
            int Results = 0;
            SortedList Params = new SortedList();
            int nClientID = myFunctions.GetClientID(User);
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCategory = "Select X_UserCategory from Sec_User  inner join Sec_UserCategory on Sec_User.X_UserCategoryList = Sec_UserCategory.N_UserCategoryID where Sec_User.X_UserID=@p3 and Sec_User.N_CompanyID =@p1";
            string sqlTrans = "select count(1) from vw_UserTransaction where n_userid=@p2";
            string sqlUser = "select X_UserID from sec_user where n_userid=@p2";

            Params.Add("@p1", nCompanyID);
            Params.Add("@p2", nUserId);
            Params.Add("@nClientID", nClientID);


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    object UserDt = dLayer.ExecuteScalar(sqlUser, Params, connection);
                    Params.Add("@p3", UserDt.ToString());
                    using (SqlConnection olivoCon = new SqlConnection(masterDBConnectionString))
                    {
                        olivoCon.Open();
                        SqlTransaction olivoTxn = olivoCon.BeginTransaction();

                        object nUseradmin = dLayer.ExecuteScalar("SELECT count(1) FROM ClientMaster where X_AdminUserID=@p3 and N_ClientID=@nClientID", Params, olivoCon, olivoTxn);
                        if (myFunctions.getIntVAL(nUseradmin.ToString()) > 0)
                        {
                            return Ok(_api.Error(User, "Unable to delete User"));
                        }


                        // nCliUserId = dLayer.ExecuteScalar("select N_UserID from Users where X_AdminUserID=@p3 and N_ClientID=@nClientID" , Params, olivoCon, olivoTxn);
                        // object Category = dLayer.ExecuteScalar(sqlCategory, Params, connection);
                        // if (Category == null)
                        //     return Ok(_api.Error(User, "Unable to delete User"));

                        // if (Category.ToString() == "Olivo" || Category.ToString().ToLower() == "administrator")
                        //     return Ok(_api.Error(User, "Unable to delete User"));
                        else
                        {
                            int N_CountTransUser = 0;
                            object CountTransUser = dLayer.ExecuteScalar(sqlTrans, Params, connection);
                            N_CountTransUser = myFunctions.getIntVAL(CountTransUser.ToString());
                            if (N_CountTransUser > 0)
                                return Ok(_api.Error(User, "Unable to delete User"));
                        }

                        Results = dLayer.DeleteData("sec_User", "N_UserId", nUserId, "", connection);

                        int N_CliUserID = 0;
                        object CountCliUser = dLayer.ExecuteScalar("select N_UserID from Users where X_EmailID=@p3 and N_ClientID=@nClientID ", Params, olivoCon, olivoTxn);
                        if (CountCliUser != null)
                        {
                            N_CliUserID = myFunctions.getIntVAL(CountCliUser.ToString());
                            Params.Add("@p4", N_CliUserID);
                            dLayer.ExecuteNonQuery("DELETE FROM Users WHERE N_UserID=@p4 ", Params, olivoCon, olivoTxn);

                        }



                        if (Results > 0)
                        {
                            dLayer.ExecuteNonQuery("delete from sec_userapps WHERE N_UserID=@p2 and N_CompanyID = @p1  ",Params, connection);
                            olivoTxn.Commit();
                            return Ok(_api.Success("User deleted"));
                        }
                        else
                        {
                            return Ok(_api.Error(User, "Unable to delete User"));
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                return Ok(_api.Error(User, ex));
            }


        }

         [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
         [HttpGet("userlistHierarchy")]
        public ActionResult GetUserHierarchyBasedList()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);

            string UserPattern = myFunctions.GetUserPattern(User);
            int nUserID = myFunctions.GetUserID(User);
            string Pattern = "";

            // if (UserPattern != "")
            //     {
            //     Pattern = " and Left(X_Pattern,Len(@UserPattern))=@UserPattern ";
            //     Params.Add("@UserPattern", UserPattern);
            //     }
            string sqlCommandText = "select * from vw_UserList where N_CompanyID="+nCompanyId+"  "+Pattern+"";
            Params.Add("N_CompanyID", nCompanyId);
            // Params.Add("N_UserId", userid);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                }
                dt = _api.Format(dt);
                if (dt.Rows.Count == 0)
                    return Ok(_api.Warning("No Results Found"));
                else
                {
             
                    return Ok(_api.Success(dt));
                }

            }
            catch (Exception e)
            {
                return StatusCode(403, _api.Error(User, e));
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("updateSign")]
        public ActionResult UpdateSign([FromBody] DataSet ds)
        {
            DataTable DtSign;
            DtSign = ds.Tables["dtSign"];
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            int nUserID=myFunctions.getIntVAL(DtSign.Rows[0]["n_UserID"].ToString());

            DataColumnCollection columns = DtSign.Columns;
            string image =myFunctions.ContainColumn("I_Sign", DtSign) ? DtSign.Rows[0]["I_Sign"].ToString() : "";
            image = Regex.Replace(image, @"^data:image\/[a-z]+;base64,", "");
            Byte[] I_Sign = new Byte[image.Length];
            I_Sign = Convert.FromBase64String(image);

            Params.Add("N_CompanyID", nCompanyId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    int a =dLayer.SaveImage("sec_user", "i_sign", I_Sign, "N_UserID", nUserID, connection, transaction);
                    transaction.Commit();
                   // dLayer.ExecuteNonQuery("update sec_user set I_Sign='"+I_Sign+"' where N_CompanyID="+ nCompanyId +" and N_UserID="+nUserID, Params, connection);
                }
                return Ok(_api.Success("Sign Updated!"));
            }
            catch (Exception e)
            {
                return StatusCode(403, _api.Error(User, e));
            }
        }

        [HttpGet("loadSign") ]
        public ActionResult LoadSign(int nCompanyID, int nUserID)
        {    
            SortedList param = new SortedList();           
            DataTable dt=new DataTable();
            string sqlCommandText="";

            sqlCommandText="select I_Sign from Sec_User where N_CompanyID=@p1 and N_UserID=@p2";
            param.Add("@p1", nCompanyID);      
            param.Add("@p2", nUserID);          
                
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
                }
                if(dt.Rows.Count==0)
                {
                    return Ok(_api.Notice("No Results Found"));
                }
                else
                {
                    return Ok(_api.Success(dt));
                }
            }
            catch(Exception e)
            {
                return Ok(_api.Error(User,e));
            }   
        }   

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("companylist")]
        public ActionResult GetUserCompanyList(string xUserID, int nClientID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            int nCompanyId = myFunctions.GetCompanyID(User);
            string sqlCommandText = "SELECT Sec_User.N_CompanyID, Sec_User.D_ExpireDate, Sec_User.X_UserCategoryList, Sec_User.X_Email, Acc_Company.N_ClientID, Acc_Company.X_CompanyName, Acc_Company.X_CompanyCode, Sec_User.X_UserName, " +
                                    " Sec_User.N_UserCategoryID, Sec_User.X_UserID, Sec_User.N_UserID, Sec_User.X_Password " +
                                    " FROM Sec_User INNER JOIN Acc_Company ON Sec_User.N_CompanyID = Acc_Company.N_CompanyID " +
                                    " where X_UserID='"+ xUserID +"' and N_ClientID="+ nClientID +" and Sec_User.N_CompanyID <>"+ nCompanyId;
            Params.Add("N_CompanyID", nCompanyId);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
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
                return StatusCode(403, _api.Error(User, e));
            }
        }
    }
}