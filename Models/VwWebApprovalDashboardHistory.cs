using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwWebApprovalDashboardHistory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_VacationID")]
        public int NVacationId { get; set; }
        [Column("D_VacReqestDate", TypeName = "datetime")]
        public DateTime? DVacReqestDate { get; set; }
        [Column("D_VacApprovedDate", TypeName = "datetime")]
        public DateTime? DVacApprovedDate { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_ForwardBy")]
        [StringLength(60)]
        public string XForwardBy { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_VacType")]
        [StringLength(50)]
        public string XVacType { get; set; }
        [Column("X_VacDateFrom")]
        [StringLength(8000)]
        public string XVacDateFrom { get; set; }
        [Column("X_VacDateTo")]
        [StringLength(8000)]
        public string XVacDateTo { get; set; }
        [Column("N_VacDays")]
        public double? NVacDays { get; set; }
        [Required]
        [Column("N_VacRemain")]
        [StringLength(1)]
        public string NVacRemain { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(11)]
        public string XStatus { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_VacRemarks")]
        [StringLength(250)]
        public string XVacRemarks { get; set; }
        [Column("X_ContactNo")]
        [StringLength(50)]
        public string XContactNo { get; set; }
        [Column("N_VacationStatus")]
        public int? NVacationStatus { get; set; }
        [Column("N_VacTypeID")]
        public int? NVacTypeId { get; set; }
        [Column("X_EmailID")]
        [StringLength(100)]
        public string XEmailId { get; set; }
        public int? MaxLevel { get; set; }
        [Required]
        [Column("X_VacApprovedComments")]
        [StringLength(250)]
        public string XVacApprovedComments { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("N_ApprovalLevelID")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_ProcStatusID")]
        public int? NProcStatusId { get; set; }
        [Column("N_ApprovalUserID")]
        public int? NApprovalUserId { get; set; }
        [Column("N_CurrentLevel")]
        public int? NCurrentLevel { get; set; }
        [Column("X_TransCode")]
        [StringLength(50)]
        public string XTransCode { get; set; }
        [Column("N_DelegateID")]
        public int? NDelegateId { get; set; }
    }
}
