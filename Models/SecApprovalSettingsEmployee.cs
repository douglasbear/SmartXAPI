using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sec_ApprovalSettings_Employee")]
    public partial class SecApprovalSettingsEmployee
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_ApprovalSettingsID")]
        public int NApprovalSettingsId { get; set; }
        [Required]
        [Column("X_ApprovalSettingsCode")]
        [StringLength(50)]
        public string XApprovalSettingsCode { get; set; }
        [Required]
        [Column("X_ApprovalSettingsDesc")]
        [StringLength(100)]
        public string XApprovalSettingsDesc { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
    }
}
