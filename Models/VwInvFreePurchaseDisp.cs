﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvFreePurchaseDisp
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
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("D_InvoiceDate", TypeName = "datetime")]
        public DateTime? DInvoiceDate { get; set; }
        [Column("N_InvoiceAmtF", TypeName = "money")]
        public decimal? NInvoiceAmtF { get; set; }
        [Column("N_DiscountAmtF", TypeName = "money")]
        public decimal? NDiscountAmtF { get; set; }
        [Column("N_CashPaidF", TypeName = "money")]
        public decimal? NCashPaidF { get; set; }
        [Column("N_FreightAmtF", TypeName = "money")]
        public decimal? NFreightAmtF { get; set; }
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
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("X_BeneficiaryCode")]
        [StringLength(50)]
        public string XBeneficiaryCode { get; set; }
        [Column("X_BeneficiaryName")]
        [StringLength(100)]
        public string XBeneficiaryName { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_VendorCurrencyID")]
        public int? NVendorCurrencyId { get; set; }
        [Column("X_ShortName")]
        [StringLength(50)]
        public string XShortName { get; set; }
        [Column("B_default")]
        public bool? BDefault { get; set; }
        [Column("N_Decimal")]
        public int? NDecimal { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("N_TaxCategoryId")]
        public int? NTaxCategoryId { get; set; }
        [Required]
        [Column("X_CategoryName")]
        [StringLength(100)]
        public string XCategoryName { get; set; }
        [Column("N_InvoiceAmt", TypeName = "money")]
        public decimal? NInvoiceAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_CashPaid", TypeName = "money")]
        public decimal? NCashPaid { get; set; }
        [Column("N_FreightAmt", TypeName = "money")]
        public decimal? NFreightAmt { get; set; }
        [Column("X_VendorInvoice")]
        [StringLength(50)]
        public string XVendorInvoice { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("B_FreightAmountDirect")]
        public bool? BFreightAmountDirect { get; set; }
        [Column("N_TaxAmtF", TypeName = "money")]
        public decimal? NTaxAmtF { get; set; }
        [Column("N_InvDueDays")]
        public int? NInvDueDays { get; set; }
        [Column("N_taxperc", TypeName = "money")]
        public decimal NTaxperc { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
    }
}
