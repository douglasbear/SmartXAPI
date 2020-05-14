using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmpEvaluationSettings")]
    public partial class PayEmpEvaluationSettings
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Key]
        [Column("N_EvaluationID")]
        public int NEvaluationId { get; set; }
        [Column("X_EvaluationCode")]
        [StringLength(20)]
        public string XEvaluationCode { get; set; }
        [Column("X_Name")]
        [StringLength(500)]
        public string XName { get; set; }
        [Column("D_PeriodFrom", TypeName = "smalldatetime")]
        public DateTime? DPeriodFrom { get; set; }
        [Column("D_PeriodTo", TypeName = "smalldatetime")]
        public DateTime? DPeriodTo { get; set; }
        [Column("X_EmpDep")]
        [StringLength(200)]
        public string XEmpDep { get; set; }
        [Column("X_EmpDepID")]
        [StringLength(50)]
        public string XEmpDepId { get; set; }
    }
}
