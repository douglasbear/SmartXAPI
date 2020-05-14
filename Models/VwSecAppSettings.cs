using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSecAppSettings
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_SecApprovalID")]
        public int NSecApprovalId { get; set; }
        [Column("N_ModuleID")]
        public int NModuleId { get; set; }
        [Column("N_FormID")]
        public int NFormId { get; set; }
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("X_ApprovalCode")]
        [StringLength(50)]
        public string XApprovalCode { get; set; }
        [Column("X_Module")]
        [StringLength(8000)]
        public string XModule { get; set; }
        [Column("X_Screen")]
        [StringLength(1000)]
        public string XScreen { get; set; }
        [Column("X_ApprovalDescription")]
        [StringLength(100)]
        public string XApprovalDescription { get; set; }
    }
}
