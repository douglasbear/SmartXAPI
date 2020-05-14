using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ffw_InvoiceSaleDetail")]
    public partial class FfwInvoiceSaleDetail
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_InvoiceId")]
        public int? NInvoiceId { get; set; }
        [Key]
        [Column("N_InvoiceSaleId")]
        public int NInvoiceSaleId { get; set; }
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
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_TaxAmountF", TypeName = "money")]
        public decimal? NTaxAmountF { get; set; }
        [Column("N_TaxAmount", TypeName = "money")]
        public decimal? NTaxAmount { get; set; }
        [Column("N_TaxPerc", TypeName = "money")]
        public decimal? NTaxPerc { get; set; }
        [Column("N_TaxCatId")]
        public int? NTaxCatId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
    }
}
