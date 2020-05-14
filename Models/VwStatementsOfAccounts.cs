using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwStatementsOfAccounts
    {
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_Debit")]
        [StringLength(50)]
        public string NDebit { get; set; }
        [Column("N_Credit")]
        [StringLength(50)]
        public string NCredit { get; set; }
        [Column("N_Balance")]
        [StringLength(50)]
        public string NBalance { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
    }
}
