using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ItemLength")]
    public partial class InvItemLength
    {
        [Column("N_LengthID")]
        public int? NLengthId { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("N_Length")]
        public int? NLength { get; set; }
        [Column("N_NumOnly")]
        public int? NNumOnly { get; set; }
    }
}
