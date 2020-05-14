using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ffw_ItemCategory")]
    public partial class FfwItemCategory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("X_CategoryCode")]
        [StringLength(100)]
        public string XCategoryCode { get; set; }
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
        [Column("N_Level")]
        public int? NLevel { get; set; }
        [Column("N_TaxCategoryId")]
        public int? NTaxCategoryId { get; set; }
    }
}
