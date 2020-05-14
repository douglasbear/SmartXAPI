using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ass_AssetMainCategory")]
    public partial class AssAssetMainCategory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_MainCategoryID")]
        public int NMainCategoryId { get; set; }
        [Column("X_MainCategory")]
        [StringLength(100)]
        public string XMainCategory { get; set; }
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
        [Column("X_MainCategoryPrefix")]
        [StringLength(20)]
        public string XMainCategoryPrefix { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_AccuDepreciationLedgerID")]
        public int? NAccuDepreciationLedgerId { get; set; }
        [Column("X_MainCategoryCode")]
        [StringLength(50)]
        public string XMainCategoryCode { get; set; }
    }
}
