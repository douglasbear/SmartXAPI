using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwGenFormNameDisp
    {
        [Column("N_FormID")]
        public int NFormId { get; set; }
        [Required]
        [Column("X_ControlNo")]
        [StringLength(50)]
        public string XControlNo { get; set; }
        [Column("X_Text")]
        [StringLength(500)]
        public string XText { get; set; }
        [Column("N_LanguageId")]
        public int? NLanguageId { get; set; }
    }
}
