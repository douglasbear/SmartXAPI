using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayTransactionDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_PayRunID")]
        [StringLength(30)]
        public string NPayRunId { get; set; }
        [StringLength(100)]
        public string Batch { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("Payrun ID")]
        [StringLength(50)]
        public string PayrunId { get; set; }
        [Column("Posting Performed")]
        public bool? PostingPerformed { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("PayRunId")]
        public int? PayRunId1 { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
    }
}
