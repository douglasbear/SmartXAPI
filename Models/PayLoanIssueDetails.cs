using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_LoanIssueDetails")]
    public partial class PayLoanIssueDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_LoanTransID")]
        public int NLoanTransId { get; set; }
        [Key]
        [Column("N_LoanTransDetailsID")]
        public int NLoanTransDetailsId { get; set; }
        [Column("D_DateFrom", TypeName = "datetime")]
        public DateTime? DDateFrom { get; set; }
        [Column("D_DateTo", TypeName = "datetime")]
        public DateTime? DDateTo { get; set; }
        [Column("N_InstAmount", TypeName = "money")]
        public decimal? NInstAmount { get; set; }
        [Column("N_RefundAmount", TypeName = "money")]
        public decimal? NRefundAmount { get; set; }
        [Column("D_RefundDate", TypeName = "datetime")]
        public DateTime? DRefundDate { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_TransDetailsID")]
        public int? NTransDetailsId { get; set; }
        [Column("B_IsLoanClose")]
        public bool? BIsLoanClose { get; set; }
        [Column("N_InstActualAmt", TypeName = "money")]
        public decimal? NInstActualAmt { get; set; }
    }
}
