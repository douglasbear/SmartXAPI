using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFfwItemCategory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [StringLength(100)]
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
        public int? TaxCategoryId { get; set; }
        [StringLength(100)]
        public string DisplayName { get; set; }
    }
}
