using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_LedgerBalForReporting")]
    public partial class AccLedgerBalForReporting
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("X_Level")]
        [StringLength(1000)]
        public string XLevel { get; set; }
        [Column("N_Opening", TypeName = "money")]
        public decimal? NOpening { get; set; }
        [Column("N_OpeningCr", TypeName = "money")]
        public decimal? NOpeningCr { get; set; }
        [Column("N_Balance", TypeName = "money")]
        public decimal? NBalance { get; set; }
        [Column("N_BalanceCr", TypeName = "money")]
        public decimal? NBalanceCr { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName_Ar")]
        [StringLength(100)]
        public string XLedgerNameAr { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("X_Type")]
        [StringLength(10)]
        public string XType { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_BudgetAmount", TypeName = "money")]
        public decimal? NBudgetAmount { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
