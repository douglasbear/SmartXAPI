using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwReminderSettingsScreen
    {
        [Column("N_MenuID")]
        public int NMenuId { get; set; }
        [Column("X_Caption")]
        [StringLength(50)]
        public string XCaption { get; set; }
        [Column("N_ParentMenuID")]
        public int? NParentMenuId { get; set; }
        [Column("X_Text")]
        [StringLength(1000)]
        public string XText { get; set; }
        [Column("N_LanguageId")]
        public int? NLanguageId { get; set; }
    }
}
