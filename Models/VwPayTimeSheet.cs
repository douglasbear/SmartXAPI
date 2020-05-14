using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayTimeSheet
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("D_Date", TypeName = "date")]
        public DateTime? DDate { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("D_In")]
        public TimeSpan? DIn { get; set; }
        [Column("D_Out")]
        public TimeSpan? DOut { get; set; }
        [Column("X_YearlyOffDay")]
        [StringLength(500)]
        public string XYearlyOffDay { get; set; }
        [Column("N_Workhours")]
        public double? NWorkhours { get; set; }
        [Column("N_TotHours")]
        public int? NTotHours { get; set; }
        [Column("N_Diff")]
        public double? NDiff { get; set; }
        [Column("N_SheetID")]
        public int? NSheetId { get; set; }
        [Column("N_OT")]
        public double? NOt { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("B_IsApproved")]
        public bool? BIsApproved { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_OTPayID")]
        public int? NOtpayId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
    }
}
