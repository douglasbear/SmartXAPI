using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccBankDetails
    {
        [Column("N_BankID")]
        public int NBankId { get; set; }
        [Column("X_BankName")]
        [StringLength(100)]
        public string XBankName { get; set; }
        [Column("X_BankCode")]
        [StringLength(50)]
        public string XBankCode { get; set; }
        [Column("X_SwiftNo")]
        [StringLength(50)]
        public string XSwiftNo { get; set; }
        [Column("X_AccountNo")]
        [StringLength(50)]
        public string XAccountNo { get; set; }
        [Column("X_RptPath")]
        [StringLength(500)]
        public string XRptPath { get; set; }
        [Column("B_Iscompany")]
        public bool? BIscompany { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
    }
}
