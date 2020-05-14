using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("pbcatvld")]
    public partial class Pbcatvld
    {
        [Required]
        [Column("pbv_name")]
        [StringLength(30)]
        public string PbvName { get; set; }
        [Column("pbv_vald")]
        [StringLength(254)]
        public string PbvVald { get; set; }
        [Column("pbv_type")]
        public short? PbvType { get; set; }
        [Column("pbv_cntr")]
        public int? PbvCntr { get; set; }
        [Column("pbv_msg")]
        [StringLength(254)]
        public string PbvMsg { get; set; }
    }
}
