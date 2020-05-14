using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwGenReminderDashboardExpired
    {
        [Column("N_ReminderId")]
        public int NReminderId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("X_Title")]
        [StringLength(500)]
        public string XTitle { get; set; }
        [Column("X_Subject")]
        [StringLength(500)]
        public string XSubject { get; set; }
        [Column("N_PartyID")]
        public int? NPartyId { get; set; }
        [Column("X_PartyName")]
        [StringLength(100)]
        public string XPartyName { get; set; }
        [Required]
        [Column("D_ExpiryDate")]
        [StringLength(30)]
        public string DExpiryDate { get; set; }
        [Column("X_FormName")]
        [StringLength(1000)]
        public string XFormName { get; set; }
        [Column("N_LanguageId")]
        public int? NLanguageId { get; set; }
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("B_IsAttachment")]
        public bool? BIsAttachment { get; set; }
        [Column("D_SMSDate")]
        [StringLength(30)]
        public string DSmsdate { get; set; }
        [Column("D_EmailDate")]
        [StringLength(30)]
        public string DEmailDate { get; set; }
        [Column("N_FileID")]
        public int? NFileId { get; set; }
        [Column("N_RemCategoryID")]
        public int? NRemCategoryId { get; set; }
        [Column("N_UserCategoryID")]
        public int NUserCategoryId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
    }
}
