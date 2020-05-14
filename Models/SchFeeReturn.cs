using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_FeeReturn")]
    public partial class SchFeeReturn
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_ReturnID")]
        public int NReturnId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_ReceiptID")]
        public int NReceiptId { get; set; }
        [Column("X_MemoNo")]
        [StringLength(50)]
        public string XMemoNo { get; set; }
        [Column("D_ReturnDate", TypeName = "datetime")]
        public DateTime? DReturnDate { get; set; }
        [Column("X_Paymentmethod")]
        [StringLength(50)]
        public string XPaymentmethod { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(25)]
        public string XChequeNo { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Column("N_DefLedgerID")]
        public int? NDefLedgerId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_BankName")]
        [StringLength(100)]
        public string XBankName { get; set; }
        [Column("X_Remarks")]
        [StringLength(150)]
        public string XRemarks { get; set; }
        [Column("B_IsTransfer")]
        public bool? BIsTransfer { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_PaymentMethodID")]
        public int? NPaymentMethodId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
    }
}
