using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayLoanDeductionBalancewise
    {
        [Column("N_LoanTransID")]
        public int NLoanTransId { get; set; }
        [Column("D_DateFrom", TypeName = "datetime")]
        public DateTime? DDateFrom { get; set; }
        [Column("D_DateTo", TypeName = "datetime")]
        public DateTime? DDateTo { get; set; }
        [Column("Amount To be Paid", TypeName = "money")]
        public decimal? AmountToBePaid { get; set; }
        [Column("N_RefundAmount", TypeName = "money")]
        public decimal NRefundAmount { get; set; }
        [Column(TypeName = "money")]
        public decimal? Balance { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        public int Expr1 { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_InstAmount", TypeName = "money")]
        public decimal? NInstAmount { get; set; }
        [Column("D_RefundDate", TypeName = "datetime")]
        public DateTime? DRefundDate { get; set; }
        [Required]
        [StringLength(5)]
        public string LoanSatus { get; set; }
    }
}
