using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayLoanIssueStatus
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("Employee No")]
        [StringLength(50)]
        public string EmployeeNo { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Position { get; set; }
        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }
        [Column("Loan ID")]
        [StringLength(30)]
        public string LoanId { get; set; }
        [Column("Loan Amount")]
        [StringLength(30)]
        public string LoanAmount { get; set; }
        [Column("N_LoanTransID")]
        public int NLoanTransId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("Issue Date")]
        [StringLength(8000)]
        public string IssueDate { get; set; }
        [Column("N_PayTypeID")]
        public int? NPayTypeId { get; set; }
        [Column(TypeName = "money")]
        public decimal? RefundAmoount { get; set; }
        [StringLength(8)]
        public string Status { get; set; }
        [Column("D_LoanIssueDate", TypeName = "datetime")]
        public DateTime? DLoanIssueDate { get; set; }
        [Column("B_OpeningBal")]
        public bool? BOpeningBal { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_LoanStatus")]
        public int? NLoanStatus { get; set; }
        [Column("N_NextApprovalID")]
        public int? NNextApprovalId { get; set; }
    }
}
