using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_PurchaseReturnMaster")]
    public partial class InvPurchaseReturnMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_CreditNoteId")]
        public int NCreditNoteId { get; set; }
        [Column("X_CreditNoteNo")]
        [StringLength(50)]
        public string XCreditNoteNo { get; set; }
        [Column("N_PurchaseId")]
        public int? NPurchaseId { get; set; }
        [Column("D_RetDate", TypeName = "datetime")]
        public DateTime? DRetDate { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("N_TotalReceived", TypeName = "money")]
        public decimal? NTotalReceived { get; set; }
        [Column("N_TotalReturnAmount", TypeName = "money")]
        public decimal? NTotalReturnAmount { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_TotalReceivedF", TypeName = "money")]
        public decimal? NTotalReceivedF { get; set; }
        [Column("N_TotalReturnAmountF", TypeName = "money")]
        public decimal? NTotalReturnAmountF { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("N_TaxAmtF", TypeName = "money")]
        public decimal? NTaxAmtF { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_DiscountAmtF", TypeName = "money")]
        public decimal? NDiscountAmtF { get; set; }
        [Column("N_PaymentMethod")]
        public int? NPaymentMethod { get; set; }

        [ForeignKey("NCompanyId,NVendorId,NFnYearId")]
        [InverseProperty(nameof(InvVendor.InvPurchaseReturnMaster))]
        public virtual InvVendor N { get; set; }
    }
}
