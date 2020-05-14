using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("pbcatedt")]
    public partial class Pbcatedt
    {
        [Required]
        [Column("pbe_name")]
        [StringLength(30)]
        public string PbeName { get; set; }
        [Column("pbe_edit")]
        [StringLength(254)]
        public string PbeEdit { get; set; }
        [Column("pbe_type")]
        public short? PbeType { get; set; }
        [Column("pbe_cntr")]
        public int? PbeCntr { get; set; }
        [Column("pbe_seqn")]
        public short PbeSeqn { get; set; }
        [Column("pbe_flag")]
        public int? PbeFlag { get; set; }
        [Column("pbe_work")]
        [StringLength(32)]
        public string PbeWork { get; set; }
    }
}
