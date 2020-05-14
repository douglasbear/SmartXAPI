using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ass_AssetCategory")]
    public partial class AssAssetCategory
    {
        public AssAssetCategory()
        {
            AssPurchaseDetails = new HashSet<AssPurchaseDetails>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("N_Depreciation")]
        public double? NDepreciation { get; set; }
        [Column("N_DepreciationLedgerID")]
        public int? NDepreciationLedgerId { get; set; }
        [Column("N_SaleIncomeLedgerID")]
        public int? NSaleIncomeLedgerId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_CategoryPrefix")]
        [StringLength(20)]
        public string XCategoryPrefix { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_AccuDepreciationLedgerID")]
        public int? NAccuDepreciationLedgerId { get; set; }
        [Column("X_CategoryCode")]
        [StringLength(50)]
        public string XCategoryCode { get; set; }
        [Column("N_AssetMainID")]
        public int? NAssetMainId { get; set; }

        [ForeignKey("NCompanyId,NLedgerId,NFnYearId")]
        [InverseProperty(nameof(AccMastLedger.AssAssetCategory))]
        public virtual AccMastLedger N { get; set; }
        [InverseProperty("N")]
        public virtual ICollection<AssPurchaseDetails> AssPurchaseDetails { get; set; }
    }
}
