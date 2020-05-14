using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sec_ApprovalSettings_General")]
    public partial class SecApprovalSettingsGeneral
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
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
    }
}
