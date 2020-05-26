using System;
namespace SmartxAPI.Dtos.Login
{
    public class FnYearDto
    {
		public string X_FnYearDescr { get; set; }
		public int N_FnYearID { get; set; }
		public DateTime D_Start { get; set; }
		public DateTime D_End { get; set; }
		public string X_AcYearDescr { get; set; }
		public int N_AcYearID { get; set; }
		public DateTime D_AcStart { get; set; }
		public DateTime D_AcEnd { get; set; }
    }
}