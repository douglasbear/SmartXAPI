using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Jwl_Colour")]
    public partial class JwlColour
    {
        [Column("N_ColorID")]
        public long? NColorId { get; set; }
        [Column("X_Color")]
        [StringLength(50)]
        public string XColor { get; set; }
    }
}
