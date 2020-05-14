using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmpAnyRequest")]
    public partial class PayEmpAnyRequest
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_RequestID")]
        public int NRequestId { get; set; }
        [Column("N_RequestType")]
        public int NRequestType { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("D_RequestDate", TypeName = "datetime")]
        public DateTime? DRequestDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("D_DateFrom", TypeName = "datetime")]
        public DateTime? DDateFrom { get; set; }
        [Column("D_DateTo", TypeName = "datetime")]
        public DateTime? DDateTo { get; set; }
        [Column("X_Notes")]
        [StringLength(500)]
        public string XNotes { get; set; }
        [Column("N_ApprovalLevelID")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_FileName")]
        [StringLength(200)]
        public string XFileName { get; set; }
        [Column("B_IsAttach")]
        public bool? BIsAttach { get; set; }
        [Column("X_Comments")]
        [StringLength(100)]
        public string XComments { get; set; }
        [Column("N_RequestStatus")]
        public int? NRequestStatus { get; set; }
        [Column("N_EntryUserID")]
        public int? NEntryUserId { get; set; }
    }
}
