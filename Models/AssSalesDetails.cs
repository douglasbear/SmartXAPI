using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ass_SalesDetails")]
    public partial class AssSalesDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_AssetInventoryID")]
        public int? NAssetInventoryId { get; set; }
        [Key]
        [Column("N_AssetInventoryDetailsID")]
        public int NAssetInventoryDetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_PurchasePrice", TypeName = "money")]
        public decimal? NPurchasePrice { get; set; }
        [Column("N_Bookvalue", TypeName = "money")]
        public decimal? NBookvalue { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
    }
}
