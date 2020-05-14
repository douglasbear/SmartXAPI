using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Lan_Language")]
    public partial class LanLanguage
    {
        [Key]
        [Column("N_LanguageID")]
        public int NLanguageId { get; set; }
        [Column("X_Language")]
        [StringLength(50)]
        public string XLanguage { get; set; }
        [Column("B_RightToLeft")]
        public bool? BRightToLeft { get; set; }
        public bool? IsCurrent { get; set; }
    }
}
