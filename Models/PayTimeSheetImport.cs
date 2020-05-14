using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_TimeSheetImport")]
    public partial class PayTimeSheetImport
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
        [Column("D_Act_Shift1_In")]
        public TimeSpan? DActShift1In { get; set; }
        [Column("D_Act_Shift1_Out")]
        public TimeSpan? DActShift1Out { get; set; }
        [Column("D_Act_Shift2_In")]
        public TimeSpan? DActShift2In { get; set; }
        [Column("D_Act_Shift2_Out")]
        public TimeSpan? DActShift2Out { get; set; }
    }
}
