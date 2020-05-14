using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayDisplinaryAction
    {
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_ActionCode")]
        [StringLength(50)]
        public string XActionCode { get; set; }
        [Column("X_Reason")]
        [StringLength(200)]
        public string XReason { get; set; }
        [Column("N_Penalty", TypeName = "money")]
        public decimal? NPenalty { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("N_ActionID")]
        public int NActionId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("B_IssaveDraft")]
        public bool? BIssaveDraft { get; set; }
        [Column("X_Investigation")]
        [StringLength(500)]
        public string XInvestigation { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("D_DateOfViolation", TypeName = "datetime")]
        public DateTime? DDateOfViolation { get; set; }
        [Column("X_PlaceOfViolation")]
        [StringLength(200)]
        public string XPlaceOfViolation { get; set; }
        [Column("X_EmpStatement")]
        public string XEmpStatement { get; set; }
        [Column("X_WarningDecision")]
        public string XWarningDecision { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_PositionCode")]
        [StringLength(50)]
        public string XPositionCode { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
    }
}
