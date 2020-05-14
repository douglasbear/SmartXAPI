using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_TimeSheet")]
    public partial class PayTimeSheet
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_SheetID")]
        public int? NSheetId { get; set; }
        [Column("D_Date", TypeName = "date")]
        public DateTime? DDate { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("D_In")]
        public TimeSpan? DIn { get; set; }
        [Column("D_Out")]
        public TimeSpan? DOut { get; set; }
        [Column("N_OT")]
        public double? NOt { get; set; }
        [Column("N_OTPayID")]
        public int? NOtpayId { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("B_IsApproved")]
        public bool? BIsApproved { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("N_AddorDedDetailID")]
        public int? NAddorDedDetailId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_EntryDate")]
        public int? NEntryDate { get; set; }
        [Column("D_Shift2_In")]
        public TimeSpan? DShift2In { get; set; }
        [Column("D_Shift2_Out")]
        public TimeSpan? DShift2Out { get; set; }
        [Column("N_TotalWorkHour", TypeName = "money")]
        public decimal? NTotalWorkHour { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("N_Compensate", TypeName = "money")]
        public decimal? NCompensate { get; set; }
        [Column("N_TimeSheetID")]
        public int? NTimeSheetId { get; set; }
        [Column("N_BatchID")]
        public int? NBatchId { get; set; }
        [Column("N_diff")]
        public double? NDiff { get; set; }
        [Column("N_DutyHours", TypeName = "money")]
        public decimal? NDutyHours { get; set; }
    }
}
