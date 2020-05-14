using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwLoanPaycodes
    {
        [Column("N_LoanID")]
        public int? NLoanId { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_LoanTransID")]
        public int NLoanTransId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("D_LoanIssueDate", TypeName = "datetime")]
        public DateTime? DLoanIssueDate { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
    }
}
