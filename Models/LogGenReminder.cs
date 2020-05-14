using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Log_GenReminder")]
    public partial class LogGenReminder
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_ReminderId")]
        public int NReminderId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
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
        [Column("D_RemainderEntryDate", TypeName = "datetime")]
        public DateTime? DRemainderEntryDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("B_IsAttachment")]
        public bool? BIsAttachment { get; set; }
        [Column("X_Status")]
        [StringLength(20)]
        public string XStatus { get; set; }
    }
}
