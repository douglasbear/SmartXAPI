using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrePaymentCategoryDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("X_CategoryCode")]
        [StringLength(100)]
        public string XCategoryCode { get; set; }
        [Column("X_CategoryName")]
        [StringLength(100)]
        public string XCategoryName { get; set; }
        [Column("X_CategoryPrefix")]
        [StringLength(20)]
        public string XCategoryPrefix { get; set; }
        [Column("N_Duration")]
        public double? NDuration { get; set; }
        [Column("N_Frequency")]
        [StringLength(20)]
        public string NFrequency { get; set; }
        [Column("N_AssetLedgerID")]
        public int? NAssetLedgerId { get; set; }
        [Column("N_ExpenseLedgerID")]
        public int? NExpenseLedgerId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Required]
        [Column("X_AssetLedgerCode")]
        [StringLength(50)]
        public string XAssetLedgerCode { get; set; }
        [Required]
        [Column("X_AssetLedgerName")]
        [StringLength(500)]
        public string XAssetLedgerName { get; set; }
        [Required]
        [Column("X_ExpenseLedgerCode")]
        [StringLength(50)]
        public string XExpenseLedgerCode { get; set; }
        [Required]
        [Column("X_ExpenseLedgerName")]
        [StringLength(500)]
        public string XExpenseLedgerName { get; set; }
    }
}
