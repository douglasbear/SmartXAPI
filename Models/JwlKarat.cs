using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Jwl_karat")]
    public partial class JwlKarat
    {
        [Column("N_karat")]
        public int? NKarat { get; set; }
        [Column("X_karat")]
        [StringLength(50)]
        public string XKarat { get; set; }
    }
}
