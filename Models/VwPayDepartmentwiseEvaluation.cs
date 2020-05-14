using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayDepartmentwiseEvaluation
    {
        [Column("X_Question")]
        [StringLength(200)]
        public string XQuestion { get; set; }
        [Column("N_Weightage")]
        public double? NWeightage { get; set; }
        [Column("N_EvaluationDetailsID")]
        public int NEvaluationDetailsId { get; set; }
        [Column("N_EvaluationID")]
        public int NEvaluationId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DepartmentID")]
        public int NDepartmentId { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
    }
}
