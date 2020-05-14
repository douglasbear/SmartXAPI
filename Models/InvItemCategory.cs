using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ItemCategory")]
    public partial class InvItemCategory
    {
        public InvItemCategory()
        {
            InvItemMaster = new HashSet<InvItemMaster>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
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
        [Column("X_CategoryCode")]
        [StringLength(50)]
        public string XCategoryCode { get; set; }
        [Column("N_Level")]
        public int? NLevel { get; set; }
        [Column("N_TaxCategoryID")]
        public int? NTaxCategoryId { get; set; }
        [Column("N_TaxCategoryID2")]
        public int? NTaxCategoryId2 { get; set; }
        [Column("N_HSN")]
        public int? NHsn { get; set; }
        [Column("N_CessCategoryID")]
        public int? NCessCategoryId { get; set; }

        [InverseProperty("NCategory")]
        public virtual ICollection<InvItemMaster> InvItemMaster { get; set; }
    }
}
