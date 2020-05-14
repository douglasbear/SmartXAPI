using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_PriceLevelID")]
        public int? NPriceLevelId { get; set; }
        [Column("X_PurchaseDescription")]
        [StringLength(600)]
        public string XPurchaseDescription { get; set; }
        [Column("X_Rack")]
        [StringLength(50)]
        public string XRack { get; set; }
        [Column("N_PreferredVendorID")]
        public int? NPreferredVendorId { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_Rate", TypeName = "money")]
        public decimal? NRate { get; set; }
        [Column("N_ItemCost", TypeName = "money")]
        public decimal? NItemCost { get; set; }
        [Column("X_Color")]
        [StringLength(50)]
        public string XColor { get; set; }
        [Column("X_Base")]
        [StringLength(50)]
        public string XBase { get; set; }
        [Column("X_MaxWattage")]
        [StringLength(50)]
        public string XMaxWattage { get; set; }
        [Column("X_Unit")]
        [StringLength(50)]
        public string XUnit { get; set; }
        [Column("I_Image", TypeName = "image")]
        public byte[] IImage { get; set; }
        [Column("N_MinQty")]
        public double? NMinQty { get; set; }
        [Column("N_ReOrderQty")]
        public double? NReOrderQty { get; set; }
        [Column("X_ClassName")]
        [StringLength(25)]
        public string XClassName { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("X_ItemName_a")]
        [StringLength(800)]
        public string XItemNameA { get; set; }
        [Column("X_PartNo")]
        [StringLength(250)]
        public string XPartNo { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_ItemManufacturerID")]
        public int? NItemManufacturerId { get; set; }
        [Column("X_ItemManufacturer")]
        [StringLength(50)]
        public string XItemManufacturer { get; set; }
        [Column("N_MinimumMargin")]
        public double? NMinimumMargin { get; set; }
        [Column("X_SalesUnit")]
        [StringLength(500)]
        public string XSalesUnit { get; set; }
        [Column("X_PurchaseUnit")]
        [StringLength(500)]
        public string XPurchaseUnit { get; set; }
        [Column("X_StockUnit")]
        [StringLength(500)]
        public string XStockUnit { get; set; }
        [Column("N_StockUnitQty")]
        public double? NStockUnitQty { get; set; }
        [Column("X_Barcode")]
        [StringLength(50)]
        public string XBarcode { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("B_IsIMEI")]
        public bool? BIsImei { get; set; }
        [Column("N_LengthID")]
        public int? NLengthId { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("N_VendorID")]
        public int NVendorId { get; set; }
        [Column("N_StockUnitID")]
        public int? NStockUnitId { get; set; }
        [Column("N_SalesUnitID")]
        public int? NSalesUnitId { get; set; }
        [Column("N_PurchaseUnitID")]
        public int? NPurchaseUnitId { get; set; }
        [Column("N_Saleqty")]
        public double? NSaleqty { get; set; }
        [Column("N_PurchaseQty")]
        public double? NPurchaseQty { get; set; }
        [Column("N_AddUnitID1")]
        public int? NAddUnitId1 { get; set; }
        [Column("N_AddUnitID2")]
        public int? NAddUnitId2 { get; set; }
        [Column("X_AddUnit1")]
        [StringLength(500)]
        public string XAddUnit1 { get; set; }
        [Column("X_AddUnit2")]
        [StringLength(500)]
        public string XAddUnit2 { get; set; }
        [Column("N_AddUnitQty1")]
        public double? NAddUnitQty1 { get; set; }
        [Column("N_AddUnitQty2")]
        public double? NAddUnitQty2 { get; set; }
        [Column("N_StockSPrice", TypeName = "money")]
        public decimal? NStockSprice { get; set; }
        [Column("N_SalesSprice", TypeName = "money")]
        public decimal? NSalesSprice { get; set; }
        [Column("N_PurchaseSPrice", TypeName = "money")]
        public decimal? NPurchaseSprice { get; set; }
        [Column("N_Add1SellingPrice", TypeName = "money")]
        public decimal? NAdd1SellingPrice { get; set; }
        [Column("N_Add2SellingPrice", TypeName = "money")]
        public decimal? NAdd2SellingPrice { get; set; }
        [StringLength(50)]
        public string ClassName { get; set; }
        [Column("X_ItemBrand")]
        [StringLength(50)]
        public string XItemBrand { get; set; }
        [Column("N_ItemBrandID")]
        public int? NItemBrandId { get; set; }
        [Required]
        [Column("X_BgColor")]
        [StringLength(16)]
        public string XBgColor { get; set; }
        [Column("X_Category1")]
        [StringLength(100)]
        public string XCategory1 { get; set; }
        [Column("N_CategoryID1")]
        public int? NCategoryId1 { get; set; }
        [Column("B_IsBatch")]
        public bool? BIsBatch { get; set; }
        [Column("N_TransitDays")]
        public int? NTransitDays { get; set; }
        [Column("N_LeadDays")]
        public int? NLeadDays { get; set; }
        [Column("X_BOMItemUnit")]
        [StringLength(500)]
        public string XBomitemUnit { get; set; }
        [Column("N_ShelfExpiry")]
        public double? NShelfExpiry { get; set; }
        [Column("N_Height")]
        public double? NHeight { get; set; }
        [Column("N_Length")]
        public double? NLength { get; set; }
        [Column("N_Width")]
        public double? NWidth { get; set; }
        [Column("X_HSCode")]
        [StringLength(25)]
        public string XHscode { get; set; }
        [Column("N_DutyPerc")]
        public double? NDutyPerc { get; set; }
        [Column("N_Weight")]
        public double? NWeight { get; set; }
        [Column("X_FullLengthDescription")]
        [StringLength(2000)]
        public string XFullLengthDescription { get; set; }
        [Column("B_BarcodeSalePrint")]
        public bool? BBarcodeSalePrint { get; set; }
        [Column("B_BarcodePurPrint")]
        public bool? BBarcodePurPrint { get; set; }
        [Column("N_PurchaseCost", TypeName = "money")]
        public decimal? NPurchaseCost { get; set; }
        [Column("X_Notes")]
        [StringLength(2000)]
        public string XNotes { get; set; }
        [Column("B_ExcludeInInvoice")]
        public bool? BExcludeInInvoice { get; set; }
        [Column("N_AssItemID")]
        public int? NAssItemId { get; set; }
        [Column("X_AssItemCode")]
        [StringLength(50)]
        public string XAssItemCode { get; set; }
        [Column("X_AssItemName")]
        [StringLength(100)]
        public string XAssItemName { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
    }
}
