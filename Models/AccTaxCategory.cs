using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_TaxCategory")]
    public partial class AccTaxCategory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
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
        [Column("B_IsExempt")]
        public bool? BIsExempt { get; set; }
        [Column("B_IsCess")]
        public bool? BIsCess { get; set; }
    }
}
