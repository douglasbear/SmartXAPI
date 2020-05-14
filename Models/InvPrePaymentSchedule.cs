using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_PrePaymentSchedule")]
    public partial class InvPrePaymentSchedule
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_PrePaymentID")]
        public int NPrePaymentId { get; set; }
        [Key]
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
        public bool? BIsProcessed { get; set; }
        [Column("N_PrePayScheduleID")]
        public int NPrePayScheduleId { get; set; }
        [Column("N_RefID")]
        public int? NRefId { get; set; }
        [Column("N_PaycodeID")]
        public int? NPaycodeId { get; set; }
    }
}
