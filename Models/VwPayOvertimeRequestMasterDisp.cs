using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayOvertimeRequestMasterDisp
    {
        [Column("N_OvertimeRequestID")]
        public int NOvertimeRequestId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("X_ReferenceCode")]
        [StringLength(50)]
        public string XReferenceCode { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("D_DateFrom", TypeName = "date")]
        public DateTime? DDateFrom { get; set; }
        [Column("D_DateTo", TypeName = "date")]
        public DateTime? DDateTo { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_AddorDedID")]
        public int? NAddorDedId { get; set; }
        [Column("N_PayrunID")]
        public int? NPayrunId { get; set; }
        [Column("D_SalaryDate", TypeName = "date")]
        public DateTime? DSalaryDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("B_IssaveDraft")]
        public bool? BIssaveDraft { get; set; }
    }
}
