using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAssetCategoryTotal
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_AssetInventoryID")]
        public int? NAssetInventoryId { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("N_Amount")]
        public double? NAmount { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_DepreciationLedgerID")]
        public int? NDepreciationLedgerId { get; set; }
        [Column("N_SaleIncomeLedgerID")]
        public int? NSaleIncomeLedgerId { get; set; }
        [Column("N_AccuDepreciationLedgerID")]
        public int? NAccuDepreciationLedgerId { get; set; }
    }
}
