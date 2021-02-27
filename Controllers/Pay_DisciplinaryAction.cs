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
    [Route("disciplinaryAction")]
    [ApiController]
    public class Pay_DisciplinaryAction : ControllerBase
    {
        private readonly IDataAccessLayer dLayer;
        private readonly IApiFunctions _api;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;
        private readonly int N_FormID = 985;


        public Pay_DisciplinaryAction(IDataAccessLayer dl, IApiFunctions api, IMyFunctions myFun, IConfiguration conf)
        {
            dLayer = dl;
            _api = api;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpGet("list")]
        public ActionResult GetAllProjectTimesheet(int nFnYearId,int nComapanyId, int nPage, int nSizeperpage, string xSearchkey, string xSortBy)
        {
            int nCompanyId = myFunctions.GetCompanyID(User);
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();

            int Count = (nPage - 1) * nSizeperpage;
            string sqlCommandText = "";
            string sqlCommandCount = "";
            string Searchkey = "";

            if (xSearchkey != null && xSearchkey.Trim() != "")
                Searchkey = "and (N_ActionID like '%" + xSearchkey + "%'or D_Date like '%" + xSearchkey + "%' or  X_ProjectName like '%" + xSearchkey + "%' or X_Name like '%" + xSearchkey + "%' or N_Hours like '%" + xSearchkey + "%' or X_Description like '%" + xSearchkey + "%')";

            if (xSortBy == null || xSortBy.Trim() == "")
                xSortBy = " order by N_ActionID desc";
            else
            
             xSortBy = " order by " + xSortBy;
            


            if (Count == 0)
                sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_Pay_displinaryAction where N_CompanyID=@p1 " + Searchkey + " " + xSortBy;
            else
                sqlCommandText = "select top(" + nSizeperpage + ") * from Vw_Pay_displinaryAction where N_CompanyID=@p1 " + Searchkey + " and N_ActionID not in (select top(" + Count + ") N_ActionID from Vw_Pay_displinaryAction where N_CompanyID=@p1 and N_FnYearID=@p2" + xSearchkey + xSortBy + " ) " + xSortBy;

            Params.Add("@p1", nCompanyId);
            Params.Add("@p2", nFnYearId);
           
            SortedList OutPut = new SortedList();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    dt = dLayer.ExecuteDataTable(sqlCommandText, Params, connection);
                    sqlCommandCount="select count(*) as N_Count from Vw_Pay_displinaryAction where N_CompanyId=@p1 and N_FnYearID=@p2 "+ Searchkey +" ";
                    DataTable Summary = dLayer.ExecuteDataTable(sqlCommandCount, Params, connection);
                    string TotalCount = "0";
                   
                    if (Summary.Rows.Count > 0)
                    {
                        DataRow drow = Summary.Rows[0];
                        TotalCount = drow["N_Count"].ToString();
                      
                    }
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
                int nCompanyID = myFunctions.getIntVAL(MasterTable.Rows[0]["n_CompanyId"].ToString());
                int nFnYearId = myFunctions.getIntVAL(MasterTable.Rows[0]["n_FnYearId"].ToString());
                int nActionId = myFunctions.getIntVAL(MasterTable.Rows[0]["N_ActionID"].ToString());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    // Auto Gen
                    string ActionCode = "";
                    var values = MasterTable.Rows[0]["X_ActionCode"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearId);
                        Params.Add("N_FormID", this.N_FormID);
                        ActionCode = dLayer.GetAutoNumber("Pay_disciplinaryAction", "X_ActionCode", Params, connection, transaction);
                        if (ActionCode == "") { return Ok(_api.Error("Unable to generate Action Code")); }
                        MasterTable.Rows[0]["X_ActionCode"] = ActionCode;
                        

                    }
                   

                   nActionId = dLayer.SaveData("Pay_disciplinaryAction", "n_ActionId", MasterTable, connection, transaction);
                   

                   if (nActionId <= 0)
                    {
                        transaction.Rollback();
                        return Ok(_api.Error("Unable to save"));
                    }
                    else
                    {
                        transaction.Commit();
                        return Ok(_api.Success("Disciplinary Action  Information Saved"));
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(_api.Error(ex));
            }
        }

     [HttpGet("details")]
        public ActionResult GetDetails(int nActionID)
        {
            DataTable dt=new DataTable();
            SortedList Params=new SortedList();
            int nCompanyID = myFunctions.GetCompanyID(User);
            string sqlCommandText="select * from Vw_Pay_displinaryAction where N_CompanyID=@nCompanyID and N_ActionID=@nActionID";
            Params.Add("@nCompanyID",nCompanyID);
            Params.Add("@nActionID",nActionID);
            try{
                using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        dt=dLayer.ExecuteDataTable(sqlCommandText,Params,connection); 
                    }
                    if(dt.Rows.Count==0)
                        {
                            return Ok(_api.Notice("No Results Found" ));
                        }else{
                            return Ok(_api.Success(dt));
                        }
            }catch(Exception e){
                return Ok(_api.Error(e));
            }
        }

          [HttpDelete("delete")]
        public ActionResult DeleteData(int nActionID)
        {
            int Results = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Results = dLayer.DeleteData("Pay_DisciplinaryAction", "n_ActionID", nActionID, "", connection);
                    if (Results > 0)
                    {
                        return Ok( _api.Success("deleted"));
                    }
                    else
                    {
                        return Ok(_api.Error("Unable to delete "));
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