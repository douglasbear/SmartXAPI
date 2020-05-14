using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_DisciplinaryAction")]
    public partial class PayDisciplinaryAction
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ActionID")]
        public int NActionId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_ActionCode")]
        [StringLength(50)]
        public string XActionCode { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_Reason")]
        [StringLength(200)]
        public string XReason { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("N_Penalty", TypeName = "money")]
        public decimal? NPenalty { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
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
    }
}
