using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFfwQuotationSaleDetail
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_QuotationSaleId")]
        public int NQuotationSaleId { get; set; }
        [Column("N_QuotationId")]
        public int? NQuotationId { get; set; }
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
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_MinAmt", TypeName = "money")]
        public decimal? NMinAmt { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("X_DisplayName")]
        [StringLength(100)]
        public string XDisplayName { get; set; }
        [Column("N_TaxCatId")]
        public int? NTaxCatId { get; set; }
        [Column("N_TaxPerc", TypeName = "money")]
        public decimal? NTaxPerc { get; set; }
        [Column("N_TaxAmount", TypeName = "money")]
        public decimal? NTaxAmount { get; set; }
        [Column("N_TaxAmountF", TypeName = "money")]
        public decimal? NTaxAmountF { get; set; }
    }
}
