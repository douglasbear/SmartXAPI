using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayPositionDispAdvanced
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_PositionID")]
        public int NPositionId { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_SalaryGradeID")]
        public int? NSalaryGradeId { get; set; }
        [Column("B_Edit")]
        public bool BEdit { get; set; }
    }
}
