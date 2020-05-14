using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayHistoryMasterRpt
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
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("Eff_Date", TypeName = "datetime")]
        public DateTime? EffDate { get; set; }
        [Column("N_AmountOld", TypeName = "money")]
        public decimal? NAmountOld { get; set; }
        [Column("N_Percentage")]
        public double? NPercentage { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
    }
}
