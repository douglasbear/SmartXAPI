using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayHistoryMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_HistoryID")]
        public int NHistoryId { get; set; }
        [Required]
        [Column("X_HistoryCode")]
        [StringLength(50)]
        public string XHistoryCode { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("D_EffectiveDate", TypeName = "datetime")]
        public DateTime? DEffectiveDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("N_NextApprovalID")]
        public int? NNextApprovalId { get; set; }
        [Column("N_CurPositionID")]
        public int? NCurPositionId { get; set; }
        [Column("N_NewPositionID")]
        public int? NNewPositionId { get; set; }
        [Column("X_PositionCode")]
        [StringLength(50)]
        public string XPositionCode { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_NewPositionCode")]
        [StringLength(50)]
        public string XNewPositionCode { get; set; }
        [Column("X_NewPosition")]
        [StringLength(100)]
        public string XNewPosition { get; set; }
        [Column("X_Comments")]
        [StringLength(200)]
        public string XComments { get; set; }
        [Column("N_CurDeptID")]
        public int? NCurDeptId { get; set; }
        [Column("N_NewDeptID")]
        public int? NNewDeptId { get; set; }
        [Column("N_CurBranchID")]
        public int? NCurBranchId { get; set; }
        [Column("N_NewBranchID")]
        public int? NNewBranchId { get; set; }
        [StringLength(50)]
        public string CurDeptCode { get; set; }
        [StringLength(100)]
        public string CurDept { get; set; }
        [StringLength(50)]
        public string NewDeptCode { get; set; }
        [StringLength(100)]
        public string NewDept { get; set; }
        [StringLength(50)]
        public string CurBranch { get; set; }
        [StringLength(50)]
        public string CurBranchCode { get; set; }
        [StringLength(50)]
        public string NewBranch { get; set; }
        [StringLength(50)]
        public string NewBranchCode { get; set; }
    }
}
