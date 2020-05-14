using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemCategory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_InventoryLedgerID")]
        public int? NInventoryLedgerId { get; set; }
        [Column("N_SalesLedgerID")]
        public int? NSalesLedgerId { get; set; }
        [Column("N_CostOfSalesLedgerID")]
        public int? NCostOfSalesLedgerId { get; set; }
        [Column("X_InvLedger")]
        [StringLength(100)]
        public string XInvLedger { get; set; }
        [Column("X_SalesLedger")]
        [StringLength(100)]
        public string XSalesLedger { get; set; }
        [Column("X_CostLedger")]
        [StringLength(100)]
        public string XCostLedger { get; set; }
        [Column("X_InvLedgerCode")]
        [StringLength(50)]
        public string XInvLedgerCode { get; set; }
        [Column("X_CostLedgerCode")]
        [StringLength(50)]
        public string XCostLedgerCode { get; set; }
        [Column("X_SalesLedgerCode")]
        [StringLength(50)]
        public string XSalesLedgerCode { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_Level")]
        public int? NLevel { get; set; }
        [Column("N_PkeyID")]
        public int? NPkeyId { get; set; }
        [Column("X_PkeyCode")]
        [StringLength(50)]
        public string XPkeyCode { get; set; }
        [Required]
        [Column("X_CategoryName")]
        [StringLength(100)]
        public string XCategoryName { get; set; }
        [Required]
        [Column("X_DisplayName")]
        [StringLength(100)]
        public string XDisplayName { get; set; }
        [Column("Outward_Ledger")]
        [StringLength(100)]
        public string OutwardLedger { get; set; }
        [Column("Inward_Ledger")]
        [StringLength(100)]
        public string InwardLedger { get; set; }
        [Column("TaxCategory_1_N_PkeyID")]
        public int? TaxCategory1NPkeyId { get; set; }
        [Column("TaxCategory_1_X_PkeyCode")]
        [StringLength(50)]
        public string TaxCategory1XPkeyCode { get; set; }
        [Column("TaxCategory_1_X_CategoryName")]
        [StringLength(100)]
        public string TaxCategory1XCategoryName { get; set; }
        [Column("TaxCategory_1_X_DisplayName")]
        [StringLength(100)]
        public string TaxCategory1XDisplayName { get; set; }
        [Column("TaxCategory_1_Inward_Ledger")]
        [StringLength(100)]
        public string TaxCategory1InwardLedger { get; set; }
        [Column("TaxCategory_1_Outward_Ledger")]
        [StringLength(100)]
        public string TaxCategory1OutwardLedger { get; set; }
        [Column("N_HSN")]
        public int? NHsn { get; set; }
        [Column("X_CessCode")]
        [StringLength(50)]
        public string XCessCode { get; set; }
        [StringLength(100)]
        public string CessCategoryName { get; set; }
        [Column("X_CessName")]
        [StringLength(100)]
        public string XCessName { get; set; }
        [Column("X_CessInwardLedgerCode")]
        [StringLength(50)]
        public string XCessInwardLedgerCode { get; set; }
        [Column("X_CessInwardLedgerName")]
        [StringLength(100)]
        public string XCessInwardLedgerName { get; set; }
        [Column("X_CessOutwardLedgerCode")]
        [StringLength(50)]
        public string XCessOutwardLedgerCode { get; set; }
        [Column("X_CessOutwardLedgerName")]
        [StringLength(100)]
        public string XCessOutwardLedgerName { get; set; }
        [Column("N_CessId")]
        public int? NCessId { get; set; }
    }
}
