using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_PrePaymentScheduleMaster")]
    public partial class InvPrePaymentScheduleMaster
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_PrePayScheduleID")]
        public int NPrePayScheduleId { get; set; }
        [Key]
        [Column("N_PrePaymentID")]
        public int NPrePaymentId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_Type")]
        [StringLength(100)]
        public string XType { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
    }
}
