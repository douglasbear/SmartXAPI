using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_AnytimeRequest")]
    public partial class PayAnytimeRequest
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Key]
        [Column("N_RequestID")]
        public int NRequestId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_Time")]
        public double? XTime { get; set; }
        [Column("D_RequestDate", TypeName = "datetime")]
        public DateTime? DRequestDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_Notes")]
        [StringLength(500)]
        public string XNotes { get; set; }
        [Column("N_ApprovalLevelID")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("X_FileName")]
        [StringLength(200)]
        public string XFileName { get; set; }
        [Column("B_IsAttach")]
        public bool? BIsAttach { get; set; }
        [Column("N_RequestType")]
        public int? NRequestType { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_Comments")]
        [StringLength(100)]
        public string XComments { get; set; }
        [Column("D_Time")]
        public TimeSpan? DTime { get; set; }
        [Column("D_Shift1_In")]
        public TimeSpan? DShift1In { get; set; }
        [Column("D_Shift1_Out")]
        public TimeSpan? DShift1Out { get; set; }
        [Column("D_Shift2_In")]
        public TimeSpan? DShift2In { get; set; }
        [Column("D_Shift2_Out")]
        public TimeSpan? DShift2Out { get; set; }
    }
}
