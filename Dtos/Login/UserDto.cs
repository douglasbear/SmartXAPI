using System;
namespace SmartxAPI.Dtos.Login
{
    public class UserDto
    {
        public int N_UserID { get; set; }
		public string X_UserName { get; set; }
		
        public string X_UserCategory { get; set; }
        public int N_UserCategoryID { get; set; }
		
        public DateTime D_LoginDate { get; set; }
		public string X_Language { get; set; }
		public int N_LanguageID { get; set; }
		
        public bool B_AllBranchesData { get; set; }
		
		public string X_UserFullName { get; set; }
		public int N_EmpID {get;set;}
		public string X_EmpCode {get;set;}
		public string X_EmpName {get;set;}
    }
}
