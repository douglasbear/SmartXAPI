using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmpEvaluators")]
    public partial class PayEmpEvaluators
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_EvaluatorsDetailsID")]
        public int NEvaluatorsDetailsId { get; set; }
        [Column("N_EvaluationID")]
        public int? NEvaluationId { get; set; }
        [Column("N_Weightage")]
        public int? NWeightage { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_DeptID")]
        public int? NDeptId { get; set; }
        [Column("N_PositionID")]
        public int? NPositionId { get; set; }
    }
}
