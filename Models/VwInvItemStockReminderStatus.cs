using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemStockReminderStatus
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
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
        [StringLength(100)]
        public string XItemNameA { get; set; }
        [Column("X_PartNo")]
        [StringLength(50)]
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
        public int? NFnYearId { get; set; }
        [Column("B_IsIMEI")]
        public bool? BIsImei { get; set; }
        [Column("N_LengthID")]
        public int? NLengthId { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("Current Stock")]
        public double? CurrentStock { get; set; }
        public int? Expr1 { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
    }
}
