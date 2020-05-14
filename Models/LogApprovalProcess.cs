using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Log_ApprovalProcess")]
    public partial class LogApprovalProcess
    {
        [Column("N_ActionID")]
        public int NActionId { get; set; }
        [Column("X_Action")]
        [StringLength(100)]
        public string XAction { get; set; }
        [Column("D_ActionDate", TypeName = "datetime")]
        public DateTime? DActionDate { get; set; }
        [Column("N_ActionUserID")]
        public int? NActionUserId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TransID")]
        public int? NTransId { get; set; }
        [Column("X_TransCode")]
        [StringLength(50)]
        public string XTransCode { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_EntryForm")]
        [StringLength(50)]
        public string XEntryForm { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_ApprovalLevelID")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_ProcStatusID")]
        public int? NProcStatusId { get; set; }
        [Column("N_ApprovalUserID")]
        public int? NApprovalUserId { get; set; }
        [Column("X_Remarks")]
        [StringLength(1000)]
        public string XRemarks { get; set; }
        [Column("X_Status")]
        [StringLength(1000)]
        public string XStatus { get; set; }
        [Column("X_ProjectName")]
        [StringLength(100)]
        public string XProjectName { get; set; }
        [Column("X_SystemName")]
        [StringLength(100)]
        public string XSystemName { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("X_Comments")]
        [StringLength(500)]
        public string XComments { get; set; }
    }
}
