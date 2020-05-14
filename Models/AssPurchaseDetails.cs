using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ass_PurchaseDetails")]
    public partial class AssPurchaseDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_AssetInventoryID")]
        public int? NAssetInventoryId { get; set; }
        [Key]
        [Column("N_AssetInventoryDetailsID")]
        public int NAssetInventoryDetailsId { get; set; }
        [Column("X_ItemName")]
        [StringLength(1000)]
        public string XItemName { get; set; }
        [Column("X_Description")]
        [StringLength(150)]
        public string XDescription { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_PurchaseQty")]
        public double? NPurchaseQty { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("N_LifePeriod")]
        public double? NLifePeriod { get; set; }
        [Column("D_PurchaseDate", TypeName = "datetime")]
        public DateTime? DPurchaseDate { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("B_BegningbalEntry")]
        public bool? BBegningbalEntry { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_Bookvalue", TypeName = "money")]
        public decimal? NBookvalue { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Column("N_DepreciationAmt", TypeName = "money")]
        public decimal? NDepreciationAmt { get; set; }
        [Column("N_TaxCategoryId")]
        public int? NTaxCategoryId { get; set; }
        [Column("N_TaxPercentage1")]
        public int? NTaxPercentage1 { get; set; }
        [Column("N_TaxAmt1", TypeName = "money")]
        public decimal? NTaxAmt1 { get; set; }
        [Column("N_POrderID")]
        public int? NPorderId { get; set; }
        [Column("N_POrderDetailsID")]
        public int? NPorderDetailsId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }

        [ForeignKey("NCategoryId,NFnYearId")]
        [InverseProperty(nameof(AssAssetCategory.AssPurchaseDetails))]
        public virtual AssAssetCategory N { get; set; }
    }
}
