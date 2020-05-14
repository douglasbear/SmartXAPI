using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwWebVacApprovalDashboard
    {
        [Column("X_VacType")]
        [StringLength(50)]
        public string XVacType { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_Sex")]
        [StringLength(50)]
        public string XSex { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("X_PassportNo")]
        [StringLength(50)]
        public string XPassportNo { get; set; }
        [Column("X_IqamaNo")]
        [StringLength(50)]
        public string XIqamaNo { get; set; }
        [Column("N_SupervisorEmpID")]
        public int? NSupervisorEmpId { get; set; }
        [Column("N_ReportToID")]
        public int? NReportToId { get; set; }
        [Required]
        [Column("N_VacRemain")]
        [StringLength(1)]
        public string NVacRemain { get; set; }
        [Column("N_AvailableDays")]
        public int NAvailableDays { get; set; }
        [Column("X_ForwardBy")]
        [StringLength(60)]
        public string XForwardBy { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_VacationID")]
        public int NVacationId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("D_VacReqestDate", TypeName = "datetime")]
        public DateTime? DVacReqestDate { get; set; }
        [Column("D_VacDateFrom", TypeName = "datetime")]
        public DateTime? DVacDateFrom { get; set; }
        [Column("D_VacDateTo", TypeName = "datetime")]
        public DateTime? DVacDateTo { get; set; }
        [Column("N_VacTypeID")]
        public int? NVacTypeId { get; set; }
        [Column("N_VacDays")]
        public double? NVacDays { get; set; }
        [Column("X_VacRemarks")]
        [StringLength(250)]
        public string XVacRemarks { get; set; }
        [Column("D_VacApprovedDate", TypeName = "datetime")]
        public DateTime? DVacApprovedDate { get; set; }
        [Required]
        [Column("X_VacApprovedComments")]
        [StringLength(250)]
        public string XVacApprovedComments { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("N_VacationStatus")]
        public int? NVacationStatus { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(11)]
        public string XStatus { get; set; }
        [Column("X_ContactNo")]
        [StringLength(50)]
        public string XContactNo { get; set; }
        [Column("X_EmailID")]
        [StringLength(100)]
        public string XEmailId { get; set; }
        [Column("N_DelegateID")]
        public int? NDelegateId { get; set; }
        public int? MaxLevel { get; set; }
        [Required]
        [Column("X_FileName")]
        public string XFileName { get; set; }
    }
}
