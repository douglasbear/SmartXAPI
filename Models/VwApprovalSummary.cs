using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwApprovalSummary
    {
        [Column("N_FormID")]
        public int NFormId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ActionID")]
        public int? NActionId { get; set; }
        [Column("X_Action")]
        [StringLength(100)]
        public string XAction { get; set; }
        [Column("N_ActionUserID")]
        public int? NActionUserId { get; set; }
        [Column("X_TransCode")]
        [StringLength(50)]
        public string XTransCode { get; set; }
        [Column("X_Type")]
        [StringLength(50)]
        public string XType { get; set; }
        [Column("N_TransID")]
        public int? NTransId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_EntryForm")]
        [StringLength(50)]
        public string XEntryForm { get; set; }
        [Column("N_ApprovalLevelID")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_ProcStatusID")]
        public int? NProcStatusId { get; set; }
        [Column("N_ApprovalUserID")]
        public int? NApprovalUserId { get; set; }
        [Column("X_Remarks")]
        [StringLength(1000)]
        public string XRemarks { get; set; }
        [Column("X_PartyName")]
        [StringLength(100)]
        public string XPartyName { get; set; }
        [Required]
        [Column("N_Amount")]
        [StringLength(50)]
        public string NAmount { get; set; }
        [Column("D_ActionDate")]
        [StringLength(8000)]
        public string DActionDate { get; set; }
        [Column("D_TransDate")]
        [StringLength(8000)]
        public string DTransDate { get; set; }
        [Column("X_ProjectName")]
        [StringLength(100)]
        public string XProjectName { get; set; }
        [Column("X_ActionUser")]
        [StringLength(60)]
        public string XActionUser { get; set; }
        [Column("N_PartyID")]
        public int NPartyId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Comment")]
        [StringLength(500)]
        public string XComment { get; set; }
        [Column("X_Status")]
        [StringLength(164)]
        public string XStatus { get; set; }
        [Column("X_SystemName")]
        [StringLength(100)]
        public string XSystemName { get; set; }
        [Column("No of Days")]
        public int? NoOfDays { get; set; }
    }
}
