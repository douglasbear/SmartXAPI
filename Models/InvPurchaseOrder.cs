using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_PurchaseOrder")]
    public partial class InvPurchaseOrder
    {
        public InvPurchaseOrder()
        {
            InvPurchaseOrderDetails = new HashSet<InvPurchaseOrderDetails>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_POrderID")]
        public int NPorderId { get; set; }
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("D_POrderDate", TypeName = "datetime")]
        public DateTime? DPorderDate { get; set; }
        [Column("N_InvoiceAmt", TypeName = "money")]
        public decimal? NInvoiceAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_CashPaid", TypeName = "money")]
        public decimal? NCashPaid { get; set; }
        [Column("N_FreightAmt", TypeName = "money")]
        public decimal? NFreightAmt { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("N_PurchaseID")]
        public int? NPurchaseId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("X_Description")]
        public string XDescription { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("B_CancelOrder")]
        public bool? BCancelOrder { get; set; }
        [Column("D_ExDelvDate", TypeName = "datetime")]
        public DateTime? DExDelvDate { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_Currency")]
        [StringLength(10)]
        public string XCurrency { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("X_QutationNo")]
        [StringLength(40)]
        public string XQutationNo { get; set; }
        [Column("X_PaymentMode")]
        [StringLength(75)]
        public string XPaymentMode { get; set; }
        [Column("X_DeliveryPlace")]
        [StringLength(100)]
        public string XDeliveryPlace { get; set; }
        [Column("N_ApproveLevel")]
        public int? NApproveLevel { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("N_QuotationID")]
        public int? NQuotationId { get; set; }
        [Column("N_InvoiceAmtF", TypeName = "money")]
        public decimal? NInvoiceAmtF { get; set; }
        [Column("N_DiscountAmtF", TypeName = "money")]
        public decimal? NDiscountAmtF { get; set; }
        [Column("N_CashPaidF", TypeName = "money")]
        public decimal? NCashPaidF { get; set; }
        [Column("N_FreightAmtF", TypeName = "money")]
        public decimal? NFreightAmtF { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_DeliveryPlaceID")]
        public int? NDeliveryPlaceId { get; set; }
        [Column("X_TandC")]
        [StringLength(2500)]
        public string XTandC { get; set; }
        [Column("X_Attention")]
        [StringLength(50)]
        public string XAttention { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("N_TaxAmtF", TypeName = "money")]
        public decimal? NTaxAmtF { get; set; }
        [Column("N_InvDueDays")]
        public int? NInvDueDays { get; set; }
        [Column("N_NextApprovalID")]
        public int? NNextApprovalId { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_POType")]
        public int? NPotype { get; set; }
        [Column("X_Comments")]
        [StringLength(200)]
        public string XComments { get; set; }
        [Column("N_WSID")]
        public int? NWsid { get; set; }
        [Column("N_SOId")]
        public int? NSoid { get; set; }
        [Column("N_POTypeID")]
        public int? NPotypeId { get; set; }

        [ForeignKey("NCompanyId,NVendorId,NFnYearId")]
        [InverseProperty(nameof(InvVendor.InvPurchaseOrder))]
        public virtual InvVendor N { get; set; }
        [InverseProperty("NPorder")]
        public virtual ICollection<InvPurchaseOrderDetails> InvPurchaseOrderDetails { get; set; }
    }
}
