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
        public string X_UserFullName { get; set; }
        public string X_UserCategoryIDList { get; set; }
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
        public string X_CurrencyName { get; set; }
        [NotMapped]
        public int N_EmpID { get; set; }
        [NotMapped]
        public string X_EmpCode { get; set; }
        [NotMapped]
        public string X_EmpName { get; set; }
        [NotMapped]
        public int N_PositionID { get; set; }
        [NotMapped]
        public string X_Position { get; set; }
        [NotMapped]
        public int N_DepartmentID { get; set; }
        [NotMapped]
        public string X_Department { get; set; }
        [NotMapped]
        public string X_EmpNameLocale { get; set; }
        [NotMapped]
        public int N_CountryID { get; set; }
        [NotMapped]
        public int N_SalesmanID { get; set; }
        [NotMapped]
        public string X_SalesmanCode { get; set; }
        [NotMapped]
        public string X_SalesmanName { get; set; }
        [NotMapped]
        public int N_CurrencyDecimal { get; set; }
        [NotMapped]
        public int N_AppID { get; set; }
        [NotMapped]
        public DataTable GlobalUserInfo { get; set; }

        public string X_BranchCode { get; set; }


    }
}
