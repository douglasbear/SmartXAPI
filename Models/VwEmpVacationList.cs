using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmpVacationList
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_VacationID")]
        public int NVacationId { get; set; }
        [Column("D_VacDateFrom", TypeName = "date")]
        public DateTime? DVacDateFrom { get; set; }
        [Column("D_VacDateTo", TypeName = "date")]
        public DateTime? DVacDateTo { get; set; }
        [Column("X_VacType")]
        [StringLength(50)]
        public string XVacType { get; set; }
        [Column("N_VacDays")]
        public double? NVacDays { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("X_VacRemarks")]
        [StringLength(250)]
        public string XVacRemarks { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(8)]
        public string XStatus { get; set; }
        [Column("N_ApprovalLevelId")]
        public int NApprovalLevelId { get; set; }
        [Required]
        [Column("X_ContactNo")]
        [StringLength(50)]
        public string XContactNo { get; set; }
        [Column("X_VacationCode")]
        [StringLength(50)]
        public string XVacationCode { get; set; }
        [Column("N_VacationStatus")]
        public int? NVacationStatus { get; set; }
    }
}
