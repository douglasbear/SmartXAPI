using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAttachmentsSync
    {
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("N_Days")]
        public int? NDays { get; set; }
        [Column("X_Recipient")]
        [StringLength(100)]
        public string XRecipient { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ReminderDate { get; set; }
        [StringLength(500)]
        public string DocumentName { get; set; }
        [Column("X_Subject")]
        [StringLength(500)]
        public string XSubject { get; set; }
        [Column("N_ReminderId")]
        public int NReminderId { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
    }
}
