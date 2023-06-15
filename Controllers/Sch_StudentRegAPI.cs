using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using SmartxAPI.GeneralFunctions;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartxAPI.Controllers
{
    [Route("studRegAPI")]
    [ApiController]
    public class Sch_StudentRegAPI : ControllerBase
    {
        private readonly IApiFunctions api;
        private readonly IDataAccessLayer dLayer;
        private readonly IMyFunctions myFunctions;
        private readonly string connectionString;

        private readonly int N_FormID = 1517;

        public Sch_StudentRegAPI(IApiFunctions apifun, IDataAccessLayer dl, IMyFunctions myFun, IConfiguration conf)
        {
            api = apifun;
            dLayer = dl;
            myFunctions = myFun;
            connectionString = conf.GetConnectionString("SmartxConnection");
        }

        [HttpPost("register")]
        public ActionResult SaveData([FromBody] DataModel data)
        {
                try
            {
                DataTable MasterTable= data.Master;

                    string seperator = "$$";
                    string[] cred = myFunctions.DecryptStringFromUrl(data.ApiKey, System.Text.Encoding.Unicode).Split(seperator);
                    int nCompanyID = myFunctions.getIntVAL(cred[0]);
                    int clientID = myFunctions.getIntVAL(cred[1]);

                    int nFnYearID =0;
                    int nRegID =0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SortedList Params = new SortedList();
                    SortedList ValidParams = new SortedList();
ValidParams.Add("@nClientID",clientID);
ValidParams.Add("@nCompanyID",nCompanyID);


                    object companyCount = dLayer.ExecuteNonQuery("select count(1) from Acc_Company where N_ClientID=@nClientID and N_CompanyID=@nCompanyID", ValidParams, connection, transaction);
                    if(myFunctions.getIntVAL(companyCount.ToString())==0){
                        transaction.Rollback();
                return Ok(api.Error(User,"Invalid API Key"));
                }

                 
                nFnYearID = myFunctions.getIntVAL(dLayer.ExecuteScalar("select MAX(N_FnyearID) from Acc_Fnyear where N_CompanyID=@nCompanyID", ValidParams, connection, transaction).ToString());   
myFunctions.AddNewColumnToDataTable(MasterTable,"N_CompanyID",typeof(int),nCompanyID);
myFunctions.AddNewColumnToDataTable(MasterTable,"N_AcYearID",typeof(int),nFnYearID);
myFunctions.AddNewColumnToDataTable(MasterTable,"N_RegID",typeof(int),0);
myFunctions.AddNewColumnToDataTable(MasterTable,"X_RegNo",typeof(string),"@Auto");
                    // Auto Gen
                    string Stud = "";
                    var values = MasterTable.Rows[0]["X_RegNo"].ToString();
                    if (values == "@Auto")
                    {
                        Params.Add("N_CompanyID", nCompanyID);
                        Params.Add("N_YearID", nFnYearID);
                        Params.Add("N_FormID", this.N_FormID);
                        Stud = dLayer.GetAutoNumber("Sch_Registration", "X_RegNo", Params, connection, transaction);
                        if (Stud == "")
                         { 
                            transaction.Rollback();
                            return Ok(api.Error(User,"Unable to generate student generation")); 
                            }
                        MasterTable.Rows[0]["X_RegNo"] = Stud;
                    }
                    string image ="";
                    
                    if( MasterTable.Columns.Contains("i_Photo"))
                    image = myFunctions.ContainColumn("i_Photo", MasterTable) ? MasterTable.Rows[0]["i_Photo"].ToString() : "";

                    Byte[] photoBitmap = new Byte[image.Length];
                    photoBitmap = Convert.FromBase64String(image);

                    if (myFunctions.ContainColumn("i_Photo", MasterTable))
                        MasterTable.Columns.Remove("i_Photo");
                        MasterTable.AcceptChanges();

                    // if (nRegID > 0) 
                    // {  
                    //     dLayer.DeleteData("Sch_Registration", "N_RegID", nRegID, "N_CompanyID =" + nCompanyID, connection, transaction);                        
                    // }

                     if( !MasterTable.Columns.Contains("N_ClassID"))
                    myFunctions.AddNewColumnToDataTable(MasterTable,"N_ClassID",typeof(int),0);


                    nRegID = dLayer.SaveData("Sch_Registration", "N_RegID", MasterTable, connection, transaction);
                    if (nRegID <= 0)
                    {
                        transaction.Rollback();
                        return Ok(api.Error(User,"Unable to save"));
                    }
                    else
                    {
                        if (image.Length > 0)
                    {
                        dLayer.SaveImage("Sch_Registration", "I_Photo", photoBitmap, "N_RegID",nRegID, connection, transaction);
                    }
                        transaction.Commit();
                        return Ok(api.Success("Student Registration Completed"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(api.Error(User,ex));
            }
        }

        public class DataModel
        {
            public DataTable Master { get; set; }
            public string ApiKey { get; set; }
        }

        // public class MasterModel
        // {
        //     public string X_FullName { get; set; }
        //     public DateTime D_DOB { get; set; }
        //     public string X_PlaceOfBirth { get; set; }
        //     public string X_Email { get; set; }
        //     public string X_MobileNo { get; set; }
        //     public string X_PassportNo { get; set; }
        //     public string X_ResidenceID { get; set; }
        //     public string X_HouseName { get; set; }
        //     public string X_BuildingNo { get; set; }
        //     public string X_StreetNo { get; set; }
        //     public string X_ZoneNo { get; set; }
        //     public string X_BuildingName { get; set; }
        //     public string X_StreetName { get; set; }
        //     public string X_ZoneName { get; set; }
        //     public string X_Landmark { get; set; }
        //     public string X_InstitutionName { get; set; }
        //     public string X_QualifyingExamName { get; set; }
        //     public string X_ExamRollNo { get; set; }
        //     public int N_FinalScore { get; set; }
        //     public string X_FatherName { get; set; }
        //     public string X_MobileNoF { get; set; }
        //     public string X_EducationF { get; set; }
        //     public string X_JobF { get; set; }
        //     public string X_ResidenceIDF { get; set; }
        //     public string X_PassportNoF { get; set; }
        //     public string X_OfficeNoF { get; set; }
        //     public int N_NationalityIDF { get; set; }
        //     public string X_CompanyF { get; set; }
        //     public DateTime D_NationalExpiryF { get; set; }
        //     public DateTime D_PExpiryF { get; set; }
        //     public string X_MotherName { get; set; }
        //     public string X_MobileNoM { get; set; }
        //     public string X_EducationM { get; set; }
        //     public string X_JobM { get; set; }
        //     public string X_ResidenceIDM { get; set; }
        //     public string X_PassportNoM { get; set; }
        //     public string X_OfficeNoM { get; set; }
        //     public int N_NationalityIDM { get; set; }
        //     public string X_CompanyM { get; set; }
        //     public DateTime D_NationalExpiryM { get; set; }
        //     public DateTime D_PExpiryM { get; set; }
        // }

        // [HttpPost("register")]
        // public ActionResult SaveData(object Obj)
        // {
        
        // }

        [HttpGet("nationality")]
        public ActionResult GetNationality()
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "select N_NationalityID, X_Nationality, X_NationalityLocale, X_NationalityCode, D_Entrydate, X_Country, B_Default, X_Currency, N_CountryID from Pay_Nationality";
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
                return Ok(api.Error(User, e));
            }
        }

        // [HttpGet("unauthguardianlist") ]
        // public ActionResult GuardianList(int nCompanyID)
        // {    
        //     SortedList param = new SortedList();           
        //     DataTable dt=new DataTable();

        //     string sqlCommandText="";

        //     sqlCommandText="select * from vw_Sch_ParentDetails_Disp where N_CompanyID=@p1";

        //     param.Add("@p1", nCompanyID);              

        //     try
        //     {
        //         using (SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             connection.Open();

        //             dt=dLayer.ExecuteDataTable(sqlCommandText,param,connection);
        //         }
        //         if(dt.Rows.Count==0)
        //         {
        //             return Ok(api.Notice("No Results Found"));
        //         }
        //         else
        //         {
        //             return Ok(api.Success(dt));
        //         }

        //     }
        //     catch(Exception e)
        //     {
        //         return Ok(api.Error(User,e));
        //     }   
        // }   

        [HttpGet("relation")]
        public ActionResult GetRelationList(int nCompanyID)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            Params.Add("@nCompanyID", nCompanyID);
            string sqlCommandText = "Select n_RelationID,x_Relation from Pay_Relation Where N_CompanyID=@nCompanyID order by n_RelationID";
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
                return Ok(api.Error(User, e));
            }
        }

        [HttpGet("country")]
        public ActionResult GetCountryList(int nCompanyID, int N_AllowCompany)
        {
            DataTable dt = new DataTable();
            SortedList Params = new SortedList();
            string sqlCommandText = "";

            if (N_AllowCompany != 0)
                sqlCommandText = "select X_CountryCode,X_CountryName,x_Currency,N_CompanyID,N_CountryID,B_TaxImplement from Acc_Country where N_CompanyID=@p1 and ISNULL(B_AllowCompany,0)=1 order by N_CountryID";
            else
                sqlCommandText = "select X_CountryCode,X_CountryName,x_Currency,N_CompanyID,N_CountryID,B_TaxImplement from Acc_Country where N_CompanyID=@p1  order by N_CountryID";
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
                    return Ok(api.Warning("No Results Found"));
                }
                else
                {
                    return Ok(api.Success(dt));
                }
            }
            catch (Exception e)
            {
                return Ok(api.Error(User, e));
            }
        }



    }
}
