using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayLoanClose
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_LoanCloseID")]
        public int NLoanCloseId { get; set; }
        [StringLength(100)]
        public string Code { get; set; }
        [Column("N_LoanID")]
        public int? NLoanId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("Paid Amount")]
        [StringLength(30)]
        public string PaidAmount { get; set; }
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
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_RateOfAmount", TypeName = "money")]
        public decimal? NRateOfAmount { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_LoanAmount", TypeName = "money")]
        public decimal? NLoanAmount { get; set; }
        [Column("Employee Code")]
        [StringLength(50)]
        public string EmployeeCode { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [Column("N_LoanTransID")]
        public int NLoanTransId { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("N_PaymentMethodID")]
        public int? NPaymentMethodId { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("N_CashTypeID")]
        public int? NCashTypeId { get; set; }
        [Column("B_IsCheque")]
        public bool? BIsCheque { get; set; }
    }
}
