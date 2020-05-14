using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAssetPurchasefromPoDetails
    {
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_POrderID")]
        public int NPorderId { get; set; }
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_FreeQty")]
        public double? NFreeQty { get; set; }
        [Column("N_PPrice", TypeName = "money")]
        public decimal? NPprice { get; set; }
        [Column("N_SPrice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_POrderDetailsID")]
        public int NPorderDetailsId { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("N_PPriceF", TypeName = "money")]
        public decimal? NPpriceF { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("Item_LocationID")]
        public int? ItemLocationId { get; set; }
        [Column("N_POType")]
        public int? NPotype { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("N_ItemTypeID")]
        public int? NItemTypeId { get; set; }
        [Column("X_FreeDescription")]
        [StringLength(5000)]
        public string XFreeDescription { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
    }
}
