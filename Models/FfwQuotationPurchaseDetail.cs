using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ffw_QuotationPurchaseDetail")]
    public partial class FfwQuotationPurchaseDetail
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_QuotationId")]
        public int? NQuotationId { get; set; }
        [Key]
        [Column("N_QuotationPurchaseId")]
        public int NQuotationPurchaseId { get; set; }
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
        [Column("N_MinAmt", TypeName = "money")]
        public decimal? NMinAmt { get; set; }
        [Column("N_EnterCurrencyID")]
        public int? NEnterCurrencyId { get; set; }
        [Column("N_EnterROE", TypeName = "money")]
        public decimal? NEnterRoe { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("X_VendorinvoiceNo")]
        [StringLength(50)]
        public string XVendorinvoiceNo { get; set; }
        [Column("N_TaxAmountF", TypeName = "money")]
        public decimal? NTaxAmountF { get; set; }
        [Column("N_TaxAmount", TypeName = "money")]
        public decimal? NTaxAmount { get; set; }
        [Column("N_TaxPerc", TypeName = "money")]
        public decimal? NTaxPerc { get; set; }
        [Column("N_TaxCatId")]
        public int? NTaxCatId { get; set; }
    }
}
