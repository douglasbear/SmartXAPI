using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("pbcatfmt")]
    public partial class Pbcatfmt
    {
        [Required]
        [Column("pbf_name")]
        [StringLength(30)]
        public string PbfName { get; set; }
        [Column("pbf_frmt")]
        [StringLength(254)]
        public string PbfFrmt { get; set; }
        [Column("pbf_type")]
        public short? PbfType { get; set; }
        [Column("pbf_cntr")]
        public int? PbfCntr { get; set; }
    }
}
