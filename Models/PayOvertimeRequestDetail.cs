using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_OvertimeRequestDetail")]
    public partial class PayOvertimeRequestDetail
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_OvertimeRequestDetailID")]
        public int NOvertimeRequestDetailId { get; set; }
        [Column("N_OvertimeRequestID")]
        public int NOvertimeRequestId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("D_Date", TypeName = "date")]
        public DateTime? DDate { get; set; }
        [Column("D_In")]
        public TimeSpan? DIn { get; set; }
        [Column("D_Out")]
        public TimeSpan? DOut { get; set; }
        [Column("D_Shift2_In")]
        public TimeSpan? DShift2In { get; set; }
        [Column("D_Shift2_Out")]
        public TimeSpan? DShift2Out { get; set; }
        [Column("N_TotalWorkHour", TypeName = "money")]
        public decimal? NTotalWorkHour { get; set; }
        [Column("N_DutyHours", TypeName = "money")]
        public decimal? NDutyHours { get; set; }
        [Column("N_OT")]
        public double? NOt { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("N_OTPayID")]
        public int? NOtpayId { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("N_PayrunID")]
        public int? NPayrunId { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
    }
}
