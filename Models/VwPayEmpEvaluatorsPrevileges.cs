using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmpEvaluatorsPrevileges
    {
        [Column("D_PeriodFrom", TypeName = "smalldatetime")]
        public DateTime? DPeriodFrom { get; set; }
        [Column("D_PeriodTo", TypeName = "smalldatetime")]
        public DateTime? DPeriodTo { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("N_EvaluationID")]
        public int? NEvaluationId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
    }
}
