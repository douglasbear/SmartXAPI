using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwWebApprovalDashboard
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
        [Column("D_VacDateFrom")]
        [StringLength(11)]
        public string DVacDateFrom { get; set; }
        [Column("D_VacDateTo")]
        [StringLength(11)]
        public string DVacDateTo { get; set; }
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
        [Column("N_CurrentApprover")]
        public int? NCurrentApprover { get; set; }
        [Column("N_FormID")]
        public int NFormId { get; set; }
        [Column("N_IssaveDraft")]
        public bool NIssaveDraft { get; set; }
        [Required]
        [Column("X_Type")]
        [StringLength(30)]
        public string XType { get; set; }
        [Column("N_NextApproverID")]
        public int? NNextApproverId { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("N_VacationStatus")]
        public int? NVacationStatus { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
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
        [Column("X_TransCode")]
        [StringLength(500)]
        public string XTransCode { get; set; }
        [Column("N_DelegateID")]
        public int? NDelegateId { get; set; }
        [Required]
        [Column("X_FileName")]
        public string XFileName { get; set; }
        [Column("N_AvailableDays")]
        public int NAvailableDays { get; set; }
        [Column("X_VacationCode")]
        [StringLength(500)]
        public string XVacationCode { get; set; }
    }
}
