using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCashBookDashboard
    {
        [Column("D_TransDate")]
        [StringLength(50)]
        public string DTransDate { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("N_AgainstLedgerID")]
        public int NAgainstLedgerId { get; set; }
        [Required]
        [Column("X_AgainstLedgercode")]
        [StringLength(50)]
        public string XAgainstLedgercode { get; set; }
        [Required]
        [Column("X_AgainstLedger")]
        [StringLength(100)]
        public string XAgainstLedger { get; set; }
        [Column("X_TransactionRef")]
        [StringLength(101)]
        public string XTransactionRef { get; set; }
        [Column("N_Amount")]
        [StringLength(50)]
        public string NAmount { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_CashBahavID")]
        public int? NCashBahavId { get; set; }
        [Column("X_Remarks")]
        [StringLength(1000)]
        public string XRemarks { get; set; }
    }
}
