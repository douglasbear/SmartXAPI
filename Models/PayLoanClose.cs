using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_LoanClose")]
    public partial class PayLoanClose
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_LoanCloseID")]
        public int NLoanCloseId { get; set; }
        [Column("X_LoanClosingCode")]
        [StringLength(100)]
        public string XLoanClosingCode { get; set; }
        [Column("N_LoanID")]
        public int? NLoanId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PaidAmount", TypeName = "money")]
        public decimal? NPaidAmount { get; set; }
        [Column("D_PaidDate", TypeName = "datetime")]
        public DateTime? DPaidDate { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(25)]
        public string XChequeNo { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Column("X_Paymentmethod")]
        [StringLength(50)]
        public string XPaymentmethod { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_BankName")]
        [StringLength(100)]
        public string XBankName { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_RateOfAmount", TypeName = "money")]
        public decimal? NRateOfAmount { get; set; }
        [Column("N_LoanTransID")]
        public int? NLoanTransId { get; set; }
        [Column("N_PaymentMethodID")]
        public int? NPaymentMethodId { get; set; }
    }
}
