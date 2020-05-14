using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPrePaymentSchedule
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_PrePaymentID")]
        public int NPrePaymentId { get; set; }
        [Column("N_PrePaymentDetailsID")]
        public int NPrePaymentDetailsId { get; set; }
        [Column("D_DateFrom", TypeName = "datetime")]
        public DateTime? DDateFrom { get; set; }
        [Column("D_DateTo", TypeName = "datetime")]
        public DateTime? DDateTo { get; set; }
        [Column("N_InstAmount", TypeName = "money")]
        public decimal? NInstAmount { get; set; }
        [Column("N_RefundAmount", TypeName = "money")]
        public decimal? NRefundAmount { get; set; }
        [Column("D_RefundDate", TypeName = "datetime")]
        public DateTime? DRefundDate { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_TransDetailsID")]
        public int? NTransDetailsId { get; set; }
        [Column("B_IsPaid")]
        public bool? BIsPaid { get; set; }
        [Column("B_IsProcessed")]
        public int BIsProcessed { get; set; }
        [Column("N_PrePayScheduleID")]
        public int NPrePayScheduleId { get; set; }
        [Column("X_Type")]
        [StringLength(100)]
        public string XType { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_PaycodeID")]
        public int? NPaycodeId { get; set; }
    }
}
