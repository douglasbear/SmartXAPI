using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFfwPurchasedetail
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_InvoiceId")]
        public int? NInvoiceId { get; set; }
        [Column("N_InvoicePurchaseId")]
        public int NInvoicePurchaseId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Rate", TypeName = "money")]
        public decimal? NRate { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_AmountSAR", TypeName = "money")]
        public decimal? NAmountSar { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("X_CurrencyName")]
        [StringLength(50)]
        public string XCurrencyName { get; set; }
        [Column("X_CurrencyCode")]
        [StringLength(50)]
        public string XCurrencyCode { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("X_PurchaseDescription")]
        [StringLength(600)]
        public string XPurchaseDescription { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("X_ShortName")]
        [StringLength(50)]
        public string XShortName { get; set; }
        [Column("N_MinAmt", TypeName = "money")]
        public decimal? NMinAmt { get; set; }
        [Column("N_EnterCurrencyID")]
        public int? NEnterCurrencyId { get; set; }
        [Column("X_EnterCurrencyCode")]
        [StringLength(50)]
        public string XEnterCurrencyCode { get; set; }
        [Column("X_EnterCurrencyName")]
        [StringLength(50)]
        public string XEnterCurrencyName { get; set; }
        [Column("N_EnterROE", TypeName = "money")]
        public decimal? NEnterRoe { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        public int Expr1 { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_VendorinvoiceNo")]
        [StringLength(50)]
        public string XVendorinvoiceNo { get; set; }
        [Column("N_TaxCatId")]
        public int? NTaxCatId { get; set; }
        [Column("N_TaxPerc", TypeName = "money")]
        public decimal? NTaxPerc { get; set; }
        [Column("N_TaxAmount", TypeName = "money")]
        public decimal? NTaxAmount { get; set; }
        [Column("N_TaxAmountF", TypeName = "money")]
        public decimal? NTaxAmountF { get; set; }
        [Column("X_DisplayName")]
        [StringLength(100)]
        public string XDisplayName { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
    }
}
