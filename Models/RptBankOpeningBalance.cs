using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class RptBankOpeningBalance
    {
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
    }
}
