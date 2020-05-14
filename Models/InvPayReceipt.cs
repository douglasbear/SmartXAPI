using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_PayReceipt")]
    public partial class InvPayReceipt
    {
        public InvPayReceipt()
        {
            InvPayReceiptDetails = new HashSet<InvPayReceiptDetails>();
        }

        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_PayReceiptId")]
        public int NPayReceiptId { get; set; }
        [Required]
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("N_PartyID")]
        public int NPartyId { get; set; }
        [Required]
        [Column("X_Type")]
        [StringLength(10)]
        public string XType { get; set; }
        [Column("N_DefLedgerID")]
        public int? NDefLedgerId { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(100)]
        public string XProjectName { get; set; }
        [Column("X_PaymentMethod")]
        [StringLength(50)]
        public string XPaymentMethod { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(50)]
        public string XChequeNo { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("N_PaymentMethodID")]
        public int? NPaymentMethodId { get; set; }
        [Column("N_BeneficiaryID")]
        public int? NBeneficiaryId { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("B_IsDraft")]
        public bool? BIsDraft { get; set; }
        [Column("N_ApproveLevel")]
        public int? NApproveLevel { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }

        [InverseProperty("NPayReceipt")]
        public virtual ICollection<InvPayReceiptDetails> InvPayReceiptDetails { get; set; }
    }
}
