using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvAssetInventoryDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_AssetInventoryID")]
        public int? NAssetInventoryId { get; set; }
        [Column("N_AssetInventoryDetailsID")]
        public int NAssetInventoryDetailsId { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_Description")]
        [StringLength(150)]
        public string XDescription { get; set; }
        [Column("X_ItemName")]
        [StringLength(100)]
        public string XItemName { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_LifePeriod")]
        public double? NLifePeriod { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PurchaseQty")]
        public double? NPurchaseQty { get; set; }
        [Column("B_BegningbalEntry")]
        public bool? BBegningbalEntry { get; set; }
        [Column("N_Bookvalue", TypeName = "money")]
        public decimal? NBookvalue { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("D_PurchaseDate", TypeName = "datetime")]
        public DateTime? DPurchaseDate { get; set; }
        [Column("X_CategoryPrefix")]
        [StringLength(20)]
        public string XCategoryPrefix { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("N_TaxPercentage1")]
        public int? NTaxPercentage1 { get; set; }
        [Column("N_POrderID")]
        public int NPorderId { get; set; }
        [Column("N_POrderDetailsID")]
        public int NPorderDetailsId { get; set; }
    }
}
