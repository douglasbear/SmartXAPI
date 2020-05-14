using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTaxCategoryDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_PkeyID")]
        public int NPkeyId { get; set; }
        [Column("X_PkeyCode")]
        [StringLength(50)]
        public string XPkeyCode { get; set; }
        [Column("X_CategoryName")]
        [StringLength(100)]
        public string XCategoryName { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_DisplayName")]
        [StringLength(100)]
        public string XDisplayName { get; set; }
        [Column("N_InwardTaxLedgerID")]
        public int? NInwardTaxLedgerId { get; set; }
        [Column("N_OutwardTaxLedgerID")]
        public int? NOutwardTaxLedgerId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_InwardLedgerCode")]
        [StringLength(50)]
        public string XInwardLedgerCode { get; set; }
        [Column("X_InwardLedger")]
        [StringLength(100)]
        public string XInwardLedger { get; set; }
        [Column("X_OutwardLedgerCode")]
        [StringLength(50)]
        public string XOutwardLedgerCode { get; set; }
        [Column("X_OutwardLedger")]
        [StringLength(100)]
        public string XOutwardLedger { get; set; }
        [Column("B_IsCess")]
        public bool BIsCess { get; set; }
    }
}
