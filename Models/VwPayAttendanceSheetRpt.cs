using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayAttendanceSheetRpt
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("D_Date1", TypeName = "date")]
        public DateTime? DDate1 { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_Workhours")]
        public double? NWorkhours { get; set; }
        [Column("N_TotHours", TypeName = "money")]
        public decimal? NTotHours { get; set; }
        public double? AdditionHrs { get; set; }
        public double? DeductionHrs { get; set; }
        [Required]
        [StringLength(1)]
        public string PaidLeave { get; set; }
        [Required]
        [StringLength(1)]
        public string UnPaidLeave { get; set; }
        [Column("N_Compensate", TypeName = "money")]
        public decimal NCompensate { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
    }
}
