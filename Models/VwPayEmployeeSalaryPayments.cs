using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeeSalaryPayments
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
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
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_Salary", TypeName = "money")]
        public decimal? NSalary { get; set; }
        [Column("N_SalaryCollected", TypeName = "money")]
        public decimal NSalaryCollected { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_BatchRemarks")]
        [StringLength(1000)]
        public string XBatchRemarks { get; set; }
    }
}
