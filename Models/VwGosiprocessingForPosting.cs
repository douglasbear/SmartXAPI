using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwGosiprocessingForPosting
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        public int Expr1 { get; set; }
        [Column("N_ProcessID")]
        public int? NProcessId { get; set; }
        [Column("N_TransDetailsID")]
        public int NTransDetailsId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_Payrate", TypeName = "money")]
        public decimal? NPayrate { get; set; }
        [Column("N_EmpAmount", TypeName = "money")]
        public decimal? NEmpAmount { get; set; }
        [Column("N_CompAmount", TypeName = "money")]
        public decimal? NCompAmount { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("Empl_Branch")]
        public int? EmplBranch { get; set; }
    }
}
