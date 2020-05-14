using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmploanList
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_LoanTransID")]
        public int NLoanTransId { get; set; }
        [Column("D_LoanIssueDate", TypeName = "date")]
        public DateTime? DLoanIssueDate { get; set; }
        [Column("N_Installments")]
        public int? NInstallments { get; set; }
        [Column("D_LoanPeriodFrom", TypeName = "date")]
        public DateTime? DLoanPeriodFrom { get; set; }
        [Column("D_LoanPeriodTo", TypeName = "date")]
        public DateTime? DLoanPeriodTo { get; set; }
        [Column("N_LoanStatus")]
        public int? NLoanStatus { get; set; }
        [Column("N_LoanAmount", TypeName = "money")]
        public decimal? NLoanAmount { get; set; }
        [Required]
        [StringLength(8)]
        public string Status { get; set; }
        [Column("N_LoanID")]
        public int NLoanId { get; set; }
        [Column("N_LevelID")]
        public int NLevelId { get; set; }
    }
}
