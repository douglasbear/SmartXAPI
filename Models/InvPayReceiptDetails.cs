using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_PayReceiptDetails")]
    public partial class InvPayReceiptDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_PayReceiptDetailsId")]
        public int NPayReceiptDetailsId { get; set; }
        [Column("N_PayReceiptId")]
        public int NPayReceiptId { get; set; }
        [Column("N_InventoryId")]
        public int NInventoryId { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Column("N_AmtPaidFromAdvance", TypeName = "money")]
        public decimal? NAmtPaidFromAdvance { get; set; }
        [Column("N_CreditInvID")]
        public int? NCreditInvId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("N_DiscountAmtF", TypeName = "money")]
        public decimal? NDiscountAmtF { get; set; }
        [Column("N_AmountF", TypeName = "money")]
        public decimal? NAmountF { get; set; }
        [Column("N_AmtPaidFromAdvanceF", TypeName = "money")]
        public decimal? NAmtPaidFromAdvanceF { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }

        [ForeignKey(nameof(NPayReceiptId))]
        [InverseProperty(nameof(InvPayReceipt.InvPayReceiptDetails))]
        public virtual InvPayReceipt NPayReceipt { get; set; }
    }
}
