using System;
namespace SmartxAPI.Dtos.Login
{
    public class CompanyDto
    {
        public string X_CompanyName { get; set; }
		public string X_CompanyName_Ar { get; set; }
		public string X_CompanyCode { get; set; }
		public int N_CompanyID { get; set; }
        public string X_Country { get; set; }
        public int N_CountryID { get; set; }
        public int N_CurrencyID { get; set; }
        public string X_CurrencyName {get;set;}
        public string N_BranchID { get; set; }
		public string X_BranchName { get; set; }
        public string X_LocationName { get; set; }
		public string N_LocationID { get; set; }
		public string I_CompanyLogo { get; set; }
        public int N_TaxType { get; set; }
        public int N_CurrencyDecimal { get; set; }
        public int N_Decimal { get; set; }
        public string X_BranchCode { get; set; }
        public int N_DefaultStudentID { get; set; }
        public int N_TypeID { get; set; }
        public string X_UtcOffSet { get; set; }
        public string X_ZoneName { get; set; }
        public bool B_IsConsolidated { get; set; }
        public bool B_EnableZatcaValidation { get; set; }


        
    }
}