using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeeSalaryPaymentsByBatch
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("D_TransDate")]
        [StringLength(8000)]
        public string DTransDate { get; set; }
        [Column("X_Batch")]
        [StringLength(100)]
        public string XBatch { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column(TypeName = "money")]
        public decimal? TotalSalary { get; set; }
        [Column(TypeName = "money")]
        public decimal? TotalSalaryCollected { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_BatchRemarks")]
        [StringLength(1000)]
        public string XBatchRemarks { get; set; }
    }
}
