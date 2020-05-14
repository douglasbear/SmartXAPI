using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwJwlPurchaseBarcode
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PurchaseID")]
        public int NPurchaseId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("D_InvoiceDate", TypeName = "datetime")]
        public DateTime? DInvoiceDate { get; set; }
        [Column("N_InvoiceAmt", TypeName = "money")]
        public decimal? NInvoiceAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_CashPaid", TypeName = "money")]
        public decimal? NCashPaid { get; set; }
        [Column("N_FreightAmt", TypeName = "money")]
        public decimal? NFreightAmt { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_PurchaseDetailsID")]
        public int NPurchaseDetailsId { get; set; }
        [Column("X_Model")]
        [StringLength(100)]
        public string XModel { get; set; }
        [Column("N_Karat")]
        public double? NKarat { get; set; }
        [Column("N_GoldWeight")]
        public double? NGoldWeight { get; set; }
        [Column("N_MCPerc")]
        public double? NMcperc { get; set; }
        [Column("N_MCRate", TypeName = "money")]
        public decimal? NMcrate { get; set; }
        [Column("N_UnitRate", TypeName = "money")]
        public decimal? NUnitRate { get; set; }
        [Column("N_Quantity")]
        public int? NQuantity { get; set; }
        [Column("N_GoldAmount", TypeName = "money")]
        public decimal? NGoldAmount { get; set; }
        [Column("N_Discount", TypeName = "money")]
        public decimal? NDiscount { get; set; }
        [Column("N_TotalAmount", TypeName = "money")]
        public decimal? NTotalAmount { get; set; }
        [Column("N_MCMinimumPerc")]
        public double? NMcminimumPerc { get; set; }
        [Column("N_OtherExp", TypeName = "money")]
        public decimal? NOtherExp { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_StockID")]
        public int NStockId { get; set; }
        [Column("X_Barcode")]
        [StringLength(50)]
        public string XBarcode { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("B_Status")]
        public bool? BStatus { get; set; }
        [Column("X_Comments")]
        [StringLength(50)]
        public string XComments { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
    }
}
