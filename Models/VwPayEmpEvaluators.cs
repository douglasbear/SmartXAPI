using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmpEvaluators
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_EvaluationID")]
        public int? NEvaluationId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_DeptID")]
        public int? NDeptId { get; set; }
        [Column("N_PositionID")]
        public int? NPositionId { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("X_EvaluationCode")]
        [StringLength(20)]
        public string XEvaluationCode { get; set; }
        [Column("N_EvaluatorsDetailsID")]
        public int NEvaluatorsDetailsId { get; set; }
        [Column("X_Name")]
        [StringLength(500)]
        public string XName { get; set; }
        [Column("D_PeriodFrom", TypeName = "smalldatetime")]
        public DateTime? DPeriodFrom { get; set; }
        [Column("D_PeriodTo", TypeName = "smalldatetime")]
        public DateTime? DPeriodTo { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("N_Weightage")]
        public int? NWeightage { get; set; }
    }
}
