using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmpTimeSheetRequest
    {
        [Column("X_BatchCode")]
        [StringLength(20)]
        public string XBatchCode { get; set; }
        [Column("N_BatchID")]
        public int? NBatchId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [StringLength(20)]
        public string BatchCode { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TimeSheetID")]
        public int NTimeSheetId { get; set; }
        [Column("N_CompMinutes")]
        public double? NCompMinutes { get; set; }
        [Column("N_Ded")]
        public double? NDed { get; set; }
        [Column("N_Ot")]
        public double? NOt { get; set; }
        [Column("D_SalaryDate", TypeName = "date")]
        public DateTime? DSalaryDate { get; set; }
        [Column("D_DateFrom", TypeName = "date")]
        public DateTime? DDateFrom { get; set; }
        [Column("D_DateTo", TypeName = "date")]
        public DateTime? DDateTo { get; set; }
        [Column("N_AddorDedID")]
        public int? NAddorDedId { get; set; }
        [Column("D_Date", TypeName = "date")]
        public DateTime? DDate { get; set; }
        [Column("D_In")]
        public TimeSpan? DIn { get; set; }
        [Column("D_Out")]
        public TimeSpan? DOut { get; set; }
        [Column("DailyOT")]
        public double? DailyOt { get; set; }
        [Column("N_OTPayID")]
        public int? NOtpayId { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("B_IsApproved")]
        public bool? BIsApproved { get; set; }
        [Column("N_AddorDedDetailID")]
        public int? NAddorDedDetailId { get; set; }
        [Column("N_Compensate", TypeName = "money")]
        public decimal? NCompensate { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("N_TotalWorkHour", TypeName = "money")]
        public decimal? NTotalWorkHour { get; set; }
        [Column("D_Shift2_Out")]
        public TimeSpan? DShift2Out { get; set; }
        [Column("D_Shift2_In")]
        public TimeSpan? DShift2In { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_CatagoryId")]
        public int? NCatagoryId { get; set; }
        [Column("N_SheetID")]
        public int? NSheetId { get; set; }
        [Column("N_diff")]
        public double? NDiff { get; set; }
        [Column("N_DutyHours", TypeName = "money")]
        public decimal? NDutyHours { get; set; }
        [Column("N_OvertimeRequestID")]
        public int? NOvertimeRequestId { get; set; }
        [Column("N_OvertimeRequestDetailID")]
        public int? NOvertimeRequestDetailId { get; set; }
    }
}
