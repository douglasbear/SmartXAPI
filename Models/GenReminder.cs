using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_Reminder")]
    public partial class GenReminder
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_ReminderId")]
        public int NReminderId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_PartyID")]
        public int? NPartyId { get; set; }
        [Column("X_Subject")]
        [StringLength(500)]
        public string XSubject { get; set; }
        [Column("X_Title")]
        [StringLength(500)]
        public string XTitle { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("N_RemCategoryID")]
        public int? NRemCategoryId { get; set; }
        [Column("B_IsAttachment")]
        public bool? BIsAttachment { get; set; }
        [Column("N_SettingsID")]
        public int? NSettingsId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
    }
}
