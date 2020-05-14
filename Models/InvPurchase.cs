using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_Purchase")]
    public partial class InvPurchase
    {
        public InvPurchase()
        {
            InvPurchaseDetails = new HashSet<InvPurchaseDetails>();
            InvPurchaseReturn = new HashSet<InvPurchaseReturn>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_PurchaseID")]
        public int NPurchaseId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
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
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("N_POrderID")]
        public int? NPorderId { get; set; }
        [Column("B_BeginingBalEntry")]
        public bool? BBeginingBalEntry { get; set; }
        [Column("N_PurchaseType")]
        public int? NPurchaseType { get; set; }
        [Column("N_PurchaseRefID")]
        public int? NPurchaseRefId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_TransType")]
        [StringLength(25)]
        public string XTransType { get; set; }
        [Column("X_Description")]
        [StringLength(1000)]
        public string XDescription { get; set; }
        [Column("N_RsID")]
        public int? NRsId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_MusthakalasNo")]
        public string XMusthakalasNo { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_BeneficiaryID")]
        public int? NBeneficiaryId { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_VendorInvoice")]
        [StringLength(50)]
        public string XVendorInvoice { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("N_InvoiceAmtF", TypeName = "money")]
        public decimal? NInvoiceAmtF { get; set; }
        [Column("N_DiscountAmtF", TypeName = "money")]
        public decimal? NDiscountAmtF { get; set; }
        [Column("N_CashPaidF", TypeName = "money")]
        public decimal? NCashPaidF { get; set; }
        [Column("N_FreightAmtF", TypeName = "money")]
        public decimal? NFreightAmtF { get; set; }
        [Column("B_FreightAmountDirect")]
        public bool? BFreightAmountDirect { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("N_TaxAmtF", TypeName = "money")]
        public decimal? NTaxAmtF { get; set; }
        [Column("N_InvDueDays")]
        public int? NInvDueDays { get; set; }
        [Column("N_TaxCategoryId")]
        public int? NTaxCategoryId { get; set; }
        [Column("N_PaymentMethod")]
        public int? NPaymentMethod { get; set; }
        [Column("N_CessAmt", TypeName = "money")]
        public decimal? NCessAmt { get; set; }
        [Column("N_WorkOrderID")]
        public int? NWorkOrderId { get; set; }
        [Column("N_MedIns")]
        public int? NMedIns { get; set; }
        [Column("D_PrintDate", TypeName = "datetime")]
        public DateTime? DPrintDate { get; set; }

        [ForeignKey("NCompanyId,NVendorId,NFnYearId")]
        [InverseProperty(nameof(InvVendor.InvPurchase))]
        public virtual InvVendor N { get; set; }
        [InverseProperty("NPurchase")]
        public virtual ICollection<InvPurchaseDetails> InvPurchaseDetails { get; set; }
        [InverseProperty("NPurchase")]
        public virtual ICollection<InvPurchaseReturn> InvPurchaseReturn { get; set; }
    }
}
