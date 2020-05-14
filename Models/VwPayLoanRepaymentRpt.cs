using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayLoanRepaymentRpt
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_LoanAmount", TypeName = "money")]
        public decimal? NLoanAmount { get; set; }
        [Column("N_LoanID")]
        public int? NLoanId { get; set; }
        [Column("N_LoanTransID")]
        public int NLoanTransId { get; set; }
        [Column("N_LoanTransDetailsID")]
        public int NLoanTransDetailsId { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("N_RefundAmount", TypeName = "money")]
        public decimal? NRefundAmount { get; set; }
        [Column("N_TransDetailsID")]
        public int? NTransDetailsId { get; set; }
        [Column("X_Batch")]
        [StringLength(100)]
        public string XBatch { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
    }
}
