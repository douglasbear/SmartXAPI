using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemSearchByBranch
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("Item Code")]
        [StringLength(50)]
        public string ItemCode { get; set; }
        [StringLength(600)]
        public string Description { get; set; }
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
        [StringLength(50)]
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
        [StringLength(50)]
        public string ProductCode { get; set; }
        [Column("N_WarehouseID")]
        public int? NWarehouseId { get; set; }
    }
}
