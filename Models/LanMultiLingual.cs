using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Lan_MultiLingual")]
    public partial class LanMultiLingual
    {
        [Column("N_FormID")]
        public int NFormId { get; set; }
        [Required]
        [Column("X_ControlNo")]
        [StringLength(50)]
        public string XControlNo { get; set; }
        [Column("X_Text")]
        [StringLength(1000)]
        public string XText { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_LanguageId")]
        public int? NLanguageId { get; set; }
    }
}
