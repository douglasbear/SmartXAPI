using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Web_Pay_LoanIssue")]
    public partial class WebPayLoanIssue
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Key]
        [Column("N_LoanTransID")]
        public int NLoanTransId { get; set; }
        [Column("D_LoanIssueDate", TypeName = "datetime")]
        public DateTime? DLoanIssueDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_Remarks")]
        [StringLength(100)]
        public string XRemarks { get; set; }
        [Column("D_LoanPeriodFrom", TypeName = "datetime")]
        public DateTime? DLoanPeriodFrom { get; set; }
        [Column("D_LoanPeriodTo", TypeName = "datetime")]
        public DateTime? DLoanPeriodTo { get; set; }
        [Column("N_LoanAmount", TypeName = "money")]
        public decimal? NLoanAmount { get; set; }
        [Column("N_LoanID")]
        public int? NLoanId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_Installments")]
        public int? NInstallments { get; set; }
        [Column("N_DefLedgerID")]
        public int? NDefLedgerId { get; set; }
        [Column("X_Paymentmethod")]
        [StringLength(50)]
        public string XPaymentmethod { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(25)]
        public string XChequeNo { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_BankName")]
        [StringLength(100)]
        public string XBankName { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_LoanStatus")]
        public int? NLoanStatus { get; set; }
        [Column("B_OpeningBal")]
        public bool? BOpeningBal { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
