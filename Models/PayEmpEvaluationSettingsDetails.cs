using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmpEvaluationSettingsDetails")]
    public partial class PayEmpEvaluationSettingsDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_EvaluationID")]
        public int NEvaluationId { get; set; }
        [Key]
        [Column("N_EvaluationDetailsID")]
        public int NEvaluationDetailsId { get; set; }
        [Column("X_Question")]
        [StringLength(200)]
        public string XQuestion { get; set; }
        [Column("N_Weightage")]
        public double? NWeightage { get; set; }
    }
}
