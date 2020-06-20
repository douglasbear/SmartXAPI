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
        public int N_CurrencyID { get; set; }
        public string N_BranchID { get; set; }
		public string X_BranchName { get; set; }
        public string X_LocationName { get; set; }
		public string N_LocationID { get; set; }
		public string I_CompanyLogo { get; set; }
        public int N_TaxType { get; set; }
    }
}