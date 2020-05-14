using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayLoanApprovals
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
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
        public int NFnYearId { get; set; }
        [Column("N_LoanStatus")]
        public int? NLoanStatus { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(11)]
        public string XStatus { get; set; }
        [Column("B_OpeningBal")]
        public bool? BOpeningBal { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmailID")]
        [StringLength(100)]
        public string XEmailId { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_CurrentApprover")]
        public int? NCurrentApprover { get; set; }
        [Column("N_NextApproverID")]
        public int? NNextApproverId { get; set; }
        [Column("N_FormID")]
        public int NFormId { get; set; }
        [Column("N_NextApprovalLevelId")]
        public int? NNextApprovalLevelId { get; set; }
        [Column("X_ForwardBy")]
        [StringLength(60)]
        public string XForwardBy { get; set; }
        [Column("D_ApprovedDate", TypeName = "datetime")]
        public DateTime? DApprovedDate { get; set; }
    }
}
