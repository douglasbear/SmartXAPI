using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_TimeSheetMaster")]
    public partial class PayTimeSheetMaster
    {
        [Key]
        [Column("N_TimeSheetID")]
        public int NTimeSheetId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("D_DateFrom", TypeName = "date")]
        public DateTime? DDateFrom { get; set; }
        [Column("D_DateTo", TypeName = "date")]
        public DateTime? DDateTo { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_AddorDedID")]
        public int? NAddorDedId { get; set; }
        [Column("N_BatchID")]
        public int? NBatchId { get; set; }
        [Column("X_BatchCode")]
        [StringLength(20)]
        public string XBatchCode { get; set; }
        [Column("D_SalaryDate", TypeName = "date")]
        public DateTime? DSalaryDate { get; set; }
        [Column("N_TotalWorkingDays")]
        public double? NTotalWorkingDays { get; set; }
        [Column("N_TotalWorkedDays")]
        public double? NTotalWorkedDays { get; set; }
        [Column("N_CompMinutes")]
        public double? NCompMinutes { get; set; }
        [Column("N_Ded")]
        public double? NDed { get; set; }
        [Column("N_Ot")]
        public double? NOt { get; set; }
        [Column("N_GridDedTotal")]
        public double? NGridDedTotal { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("N_TotalDutyHours", TypeName = "money")]
        public decimal? NTotalDutyHours { get; set; }
    }
}
