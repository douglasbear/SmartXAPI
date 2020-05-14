using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayLoanStatus
    {
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_LoanTransID")]
        public int NLoanTransId { get; set; }
        [Column("N_LoanID")]
        public int? NLoanId { get; set; }
        [Column("D_LoanPeriodFrom", TypeName = "datetime")]
        public DateTime? DLoanPeriodFrom { get; set; }
        [Column("D_LoanPeriodTo", TypeName = "datetime")]
        public DateTime? DLoanPeriodTo { get; set; }
        [Column("N_LoanAmt", TypeName = "money")]
        public decimal? NLoanAmt { get; set; }
        [Column("N_RefundAmount", TypeName = "money")]
        public decimal? NRefundAmount { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Required]
        [Column("X_LoanStatus")]
        [StringLength(6)]
        public string XLoanStatus { get; set; }
    }
}
