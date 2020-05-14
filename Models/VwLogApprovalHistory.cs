using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwLogApprovalHistory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_VacationID")]
        public int NVacationId { get; set; }
        [Column("D_VacReqestDate", TypeName = "datetime")]
        public DateTime? DVacReqestDate { get; set; }
        [Column("X_VacDateFrom")]
        [StringLength(8000)]
        public string XVacDateFrom { get; set; }
        [Column("X_VacDateTo")]
        [StringLength(8000)]
        public string XVacDateTo { get; set; }
        [Column("N_VacDays")]
        public double? NVacDays { get; set; }
        [Column("N_VacationStatus")]
        public int? NVacationStatus { get; set; }
        [Required]
        [Column("X_status")]
        [StringLength(8)]
        public string XStatus { get; set; }
        [Column("X_VacType")]
        [StringLength(50)]
        public string XVacType { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_ActionDate", TypeName = "datetime")]
        public DateTime? DActionDate { get; set; }
        [Column("N_ApproveLevel")]
        public int? NApproveLevel { get; set; }
    }
}
