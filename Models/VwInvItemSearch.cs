using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemSearch
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("Item Code")]
        [StringLength(100)]
        public string ItemCode { get; set; }
        [StringLength(800)]
        public string Description { get; set; }
        [Column("Description_Ar")]
        [StringLength(800)]
        public string DescriptionAr { get; set; }
        [Required]
        [StringLength(100)]
        public string Category { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("Item Class")]
        [StringLength(25)]
        public string ItemClass { get; set; }
        [Column("N_Rate", TypeName = "money")]
        public decimal? NRate { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [Column("Part No")]
        [StringLength(250)]
        public string PartNo { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("B_BaseUnit")]
        public bool? BBaseUnit { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_BaseUnitID")]
        public int? NBaseUnitId { get; set; }
        [Column("N_MinimumMargin")]
        public double? NMinimumMargin { get; set; }
        [Column("N_ItemManufacturerID")]
        public int? NItemManufacturerId { get; set; }
        [Column("X_ItemManufacturer")]
        [StringLength(50)]
        public string XItemManufacturer { get; set; }
        [Column("X_SalesUnit")]
        [StringLength(500)]
        public string XSalesUnit { get; set; }
        [Column("X_PurchaseUnit")]
        [StringLength(500)]
        public string XPurchaseUnit { get; set; }
        [Column("X_Barcode")]
        [StringLength(50)]
        public string XBarcode { get; set; }
        [Column("B_BarcodewithQty")]
        public bool? BBarcodewithQty { get; set; }
        [Column("X_StockUnit")]
        [StringLength(500)]
        public string XStockUnit { get; set; }
        [Column("N_StockUnitQty")]
        public double? NStockUnitQty { get; set; }
        [Column("B_IsIMEI")]
        public bool? BIsImei { get; set; }
        [Column("N_LengthID")]
        public int? NLengthId { get; set; }
        [Column("N_PurchaseUnitQty")]
        public double? NPurchaseUnitQty { get; set; }
        [Column("N_SalesUnitQty")]
        public double? NSalesUnitQty { get; set; }
        public string Stock { get; set; }
        public string Rate { get; set; }
        [Column("N_StockUnitID")]
        public int? NStockUnitId { get; set; }
        [Column("X_Rack")]
        [StringLength(50)]
        public string XRack { get; set; }
        [Column("Product Code")]
        [StringLength(100)]
        public string ProductCode { get; set; }
        [Column("B_IsBatch")]
        public bool? BIsBatch { get; set; }
        [Column("N_LeadDays")]
        public int? NLeadDays { get; set; }
        [Column("N_TransitDays")]
        public int? NTransitDays { get; set; }
        [Column("N_DeliveryDays")]
        public int? NDeliveryDays { get; set; }
        [Column("X_BOMItemUnit")]
        [StringLength(500)]
        public string XBomitemUnit { get; set; }
        [Column("N_BOMUnitID")]
        public int? NBomunitId { get; set; }
        [Column("N_TaxCategoryID")]
        public int? NTaxCategoryId { get; set; }
        [Column("X_DisplayName")]
        [StringLength(100)]
        public string XDisplayName { get; set; }
        [Column("N_PkeyID")]
        public int? NPkeyId { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("X_DisplayName2")]
        [StringLength(100)]
        public string XDisplayName2 { get; set; }
        [Column("N_TaxAmt2", TypeName = "money")]
        public decimal? NTaxAmt2 { get; set; }
        [Column("N_TaxID2")]
        public int? NTaxId2 { get; set; }
        [Column("N_PurchaseCost", TypeName = "money")]
        public decimal? NPurchaseCost { get; set; }
        [Column("X_CategoryCode")]
        [StringLength(50)]
        public string XCategoryCode { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_CessID")]
        public int? NCessId { get; set; }
        [Column("N_CessAmt", TypeName = "money")]
        public decimal? NCessAmt { get; set; }
        [Column("X_CessName")]
        [StringLength(100)]
        public string XCessName { get; set; }
        [Column("N_ItemTypeID")]
        public int NItemTypeId { get; set; }
        [Column("N_PreferredVendorID")]
        public int? NPreferredVendorId { get; set; }
    }
}
