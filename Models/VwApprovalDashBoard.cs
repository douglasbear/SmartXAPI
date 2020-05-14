using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwApprovalDashBoard
    {
        [Column("N_FormID")]
        public int NFormId { get; set; }
        [Column("X_Type")]
        [StringLength(50)]
        public string XType { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("X_TransCode")]
        [StringLength(500)]
        public string XTransCode { get; set; }
        [Column("Approved_User")]
        [StringLength(60)]
        public string ApprovedUser { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("X_PartyName")]
        [StringLength(100)]
        public string XPartyName { get; set; }
        [Column("X_PartyCode")]
        [StringLength(100)]
        public string XPartyCode { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("D_ApprovedDate", TypeName = "datetime")]
        public DateTime? DApprovedDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_IssaveDraft")]
        public bool NIssaveDraft { get; set; }
        [Column("N_NextApprovalLevelId")]
        public int? NNextApprovalLevelId { get; set; }
        [Column("X_Status")]
        [StringLength(50)]
        public string XStatus { get; set; }
        [Column("N_CurrentApprover")]
        public int? NCurrentApprover { get; set; }
        [Column("N_NextApproverID")]
        public int? NNextApproverId { get; set; }
        [Column("X_Comment")]
        [StringLength(1000)]
        public string XComment { get; set; }
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
        [Column("No of Days")]
        public int? NoOfDays { get; set; }
    }
}
