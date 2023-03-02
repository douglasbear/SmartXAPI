using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using SmartxAPI.Dtos.Login;
using SmartxAPI.Models;

namespace SmartxAPI.Dtos.SP
{
    public partial class SP_LOGIN_CLOUD
    {
        [Key]
        public int N_UserID { get; set; }
        public string X_UserName { get; set; }
        public string X_FnYearDescr { get; set; }
        public int N_FnYearID { get; set; }
        public DateTime D_Start { get; set; }
        public DateTime D_End { get; set; }
        public string X_AcYearDescr { get; set; }
        public int N_AcYearID { get; set; }
        public DateTime D_AcStart { get; set; }
        public DateTime D_AcEnd { get; set; }
        public string X_CompanyName { get; set; }
        public string X_CompanyName_Ar { get; set; }
        public string X_CompanyCode { get; set; }
        public int N_CompanyID { get; set; }
        public string X_UserCategory { get; set; }
        public int N_UserCategoryID { get; set; }
        public string X_Country { get; set; }
        public int N_CurrencyID { get; set; }
        public DateTime D_LoginDate { get; set; }
        public string X_Language { get; set; }
        public int N_LanguageID { get; set; }
        public string N_BranchID { get; set; }
        public string X_BranchName { get; set; }
        public String X_LocationName { get; set; }
        public string N_LocationID { get; set; }
        public byte[] I_Logo { get; set; }
        public bool B_AllBranchesData { get; set; }
        public int N_TaxType { get; set; }
        public int N_Decimal { get; set; }
        public string X_UserFullName { get; set; }
        public string X_BranchCode { get; set; }
        public int N_DecimalPlace { get; set; }
        public string X_UserCategoryIDList { get; set; }
        public int N_CurrencyDecimal { get; set; }
        public int N_EmpID { get; set; }
        public string X_EmpCode { get; set; }
        public string X_EmpName { get; set; }
        public int N_PositionID { get; set; }
        public string X_Position { get; set; }
        public int N_DepartmentID { get; set; }
        public string X_Department { get; set; }
        public string X_EmpNameLocale { get; set; }
        public int N_SalesmanID { get; set; }
        public string X_SalesmanCode { get; set; }
        public string X_SalesmanName { get; set; }
        public bool B_AllowEdit { get; set; }
        public string X_CurrencyName { get; set; }
        public int N_CustomerID { get; set; }
        public string X_CustomerCode { get; set; }
        public string X_CustomerName { get; set; }
        public int N_ParentID { get; set; }
        public string X_ParentCode { get; set; }
        public string X_ParentName { get; set; }
        public int N_StudentID { get; set; }
        public string X_StudentCode { get; set; }
        public string X_StudentName { get; set; }
        public int N_TeacherID { get; set; }
        public string X_TeacherCode { get; set; }
        public string X_TeacherName { get; set; }
        public int N_DefaultStudentID { get; set; }
        public int N_TypeID { get; set; }
        public string X_UtcOffSet { get; set; }
        public string X_ZoneName { get; set; }
        public int N_TimeZoneID { get; set; }
        [NotMapped]
        public string Token { get; set; }
        [NotMapped]
        public string RefreshToken { get; set; }
        [NotMapped]
        public DateTime Expiry { get; set; }
        [NotMapped]
        public List<MenuDto> MenuList { get; set; }
        [NotMapped]
        public List<MenuDto> AccessList { get; set; }
        [NotMapped]
        public string I_CompanyLogo { get; set; }
        [NotMapped]
        public int N_CountryID { get; set; }

        [NotMapped]
        public int N_AppID { get; set; }

        [NotMapped]
        public DataTable GlobalUserInfo { get; set; }
        [NotMapped]
        public int N_LoginID { get; set; }
        [NotMapped]
        public string Warning { get; set; }







    }
}
