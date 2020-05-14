using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrePaymentSchedule
    {
        public long? SlNo { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnyearId")]
        public int? NFnyearId { get; set; }
        [Column("N_PrepaymentID")]
        public int? NPrepaymentId { get; set; }
        [Column("X_PaymentNo")]
        [StringLength(100)]
        public string XPaymentNo { get; set; }
        [Required]
        [Column("D_Date")]
        [StringLength(20)]
        public string DDate { get; set; }
        [Column("N_Amount")]
        [StringLength(20)]
        public string NAmount { get; set; }
        [Required]
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        [Column("X_Startmonth")]
        [StringLength(50)]
        public string XStartmonth { get; set; }
        [Column("N_Duration")]
        public int? NDuration { get; set; }
        [Column("N_Frequency")]
        public int? NFrequency { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("B_IsSavedraft")]
        public bool? BIsSavedraft { get; set; }
        [Column("X_Notes")]
        [StringLength(500)]
        public string XNotes { get; set; }
        [Required]
        [Column("X_DateFrom")]
        [StringLength(20)]
        public string XDateFrom { get; set; }
        [Required]
        [Column("X_DateTo")]
        [StringLength(20)]
        public string XDateTo { get; set; }
        [Column("D_DateFrom", TypeName = "datetime")]
        public DateTime DDateFrom { get; set; }
        [Column("D_DateTo", TypeName = "datetime")]
        public DateTime DDateTo { get; set; }
        [Column("N_InstAmount", TypeName = "money")]
        public decimal? NInstAmount { get; set; }
        [Column("N_RefundAmount", TypeName = "money")]
        public decimal NRefundAmount { get; set; }
        [Column("D_RefundDate", TypeName = "datetime")]
        public DateTime DRefundDate { get; set; }
        [Column("D_ScheduleEntrydate", TypeName = "datetime")]
        public DateTime DScheduleEntrydate { get; set; }
        [Column("N_PrePaymentDetailsID")]
        public int NPrePaymentDetailsId { get; set; }
        [Column("B_IsPaid")]
        public bool BIsPaid { get; set; }
        [Required]
        [Column("X_Month")]
        [StringLength(36)]
        public string XMonth { get; set; }
    }
}
