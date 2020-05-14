using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPurchaseDetailsViewRpt
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("D_InvoiceDate", TypeName = "datetime")]
        public DateTime? DInvoiceDate { get; set; }
        [Column("N_PurchaseID")]
        public int NPurchaseId { get; set; }
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("N_PPrice", TypeName = "money")]
        public decimal? NPprice { get; set; }
        [Column("N_SPrice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_IMEIFrom")]
        [StringLength(50)]
        public string NImeifrom { get; set; }
        [Column("N_IMEITo")]
        [StringLength(50)]
        public string NImeito { get; set; }
        [Column("X_PartNo")]
        [StringLength(50)]
        public string XPartNo { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("N_Cost", TypeName = "decimal(20, 6)")]
        public decimal? NCost { get; set; }
        [Column("ItemWise_Discount", TypeName = "money")]
        public decimal? ItemWiseDiscount { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_FreeQty")]
        public double? NFreeQty { get; set; }
        [Column("N_MRP", TypeName = "money")]
        public decimal? NMrp { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("N_InvoiceAmt", TypeName = "money")]
        public decimal? NInvoiceAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_FreightAmt", TypeName = "money")]
        public decimal? NFreightAmt { get; set; }
        [Column("X_VendorInvoice")]
        [StringLength(50)]
        public string XVendorInvoice { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("N_FreightAmtF", TypeName = "money")]
        public decimal? NFreightAmtF { get; set; }
        [Column("N_CashPaidF", TypeName = "money")]
        public decimal? NCashPaidF { get; set; }
        [Column("N_DiscountAmtF", TypeName = "money")]
        public decimal? NDiscountAmtF { get; set; }
        [Column("N_InvoiceAmtF", TypeName = "money")]
        public decimal? NInvoiceAmtF { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("X_ShortName")]
        [StringLength(50)]
        public string XShortName { get; set; }
        [Column("B_default")]
        public bool? BDefault { get; set; }
        [Column("N_PPriceF", TypeName = "money")]
        public decimal? NPpriceF { get; set; }
        [Column("N_CostF", TypeName = "decimal(20, 6)")]
        public decimal? NCostF { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_MRNNo")]
        [StringLength(50)]
        public string XMrnno { get; set; }
        [Column("N_InvDueDays")]
        public int? NInvDueDays { get; set; }
        [Column("N_TaxAmt1", TypeName = "money")]
        public decimal? NTaxAmt1 { get; set; }
        [Column("N_TaxAmt2", TypeName = "money")]
        public decimal? NTaxAmt2 { get; set; }
        [Column("N_TaxCategoryID1")]
        public int? NTaxCategoryId1 { get; set; }
        [Column("N_TaxCategoryID2")]
        public int? NTaxCategoryId2 { get; set; }
        [Column("X_TaxRegistrationNo")]
        [StringLength(50)]
        public string XTaxRegistrationNo { get; set; }
        [Column("D_InvDueDate", TypeName = "datetime")]
        public DateTime? DInvDueDate { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("X_ItemName_a")]
        [StringLength(800)]
        public string XItemNameA { get; set; }
        [Column(TypeName = "money")]
        public decimal? ItemDiscount { get; set; }
        [Column("N_TaxAmtF", TypeName = "money")]
        public decimal? NTaxAmtF { get; set; }
        [Column("N_PurchaseDetailsID")]
        public int NPurchaseDetailsId { get; set; }
    }
}
