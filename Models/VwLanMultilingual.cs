using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwLanMultilingual
    {
        [Column("N_FormID")]
        public int NFormId { get; set; }
        [Required]
        [Column("X_ControlNo")]
        [StringLength(50)]
        public string XControlNo { get; set; }
        [StringLength(500)]
        public string English { get; set; }
        [StringLength(500)]
        public string Arabic { get; set; }
        [Column("EnglishID")]
        public int? EnglishId { get; set; }
        [Column("ArabicID")]
        public int? ArabicId { get; set; }
    }
}
