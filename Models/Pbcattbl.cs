using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("pbcattbl")]
    public partial class Pbcattbl
    {
        [Required]
        [Column("pbt_tnam")]
        [StringLength(129)]
        public string PbtTnam { get; set; }
        [Column("pbt_tid")]
        public int? PbtTid { get; set; }
        [Required]
        [Column("pbt_ownr")]
        [StringLength(129)]
        public string PbtOwnr { get; set; }
        [Column("pbd_fhgt")]
        public short? PbdFhgt { get; set; }
        [Column("pbd_fwgt")]
        public short? PbdFwgt { get; set; }
        [Column("pbd_fitl")]
        [StringLength(1)]
        public string PbdFitl { get; set; }
        [Column("pbd_funl")]
        [StringLength(1)]
        public string PbdFunl { get; set; }
        [Column("pbd_fchr")]
        public short? PbdFchr { get; set; }
        [Column("pbd_fptc")]
        public short? PbdFptc { get; set; }
        [Column("pbd_ffce")]
        [StringLength(18)]
        public string PbdFfce { get; set; }
        [Column("pbh_fhgt")]
        public short? PbhFhgt { get; set; }
        [Column("pbh_fwgt")]
        public short? PbhFwgt { get; set; }
        [Column("pbh_fitl")]
        [StringLength(1)]
        public string PbhFitl { get; set; }
        [Column("pbh_funl")]
        [StringLength(1)]
        public string PbhFunl { get; set; }
        [Column("pbh_fchr")]
        public short? PbhFchr { get; set; }
        [Column("pbh_fptc")]
        public short? PbhFptc { get; set; }
        [Column("pbh_ffce")]
        [StringLength(18)]
        public string PbhFfce { get; set; }
        [Column("pbl_fhgt")]
        public short? PblFhgt { get; set; }
        [Column("pbl_fwgt")]
        public short? PblFwgt { get; set; }
        [Column("pbl_fitl")]
        [StringLength(1)]
        public string PblFitl { get; set; }
        [Column("pbl_funl")]
        [StringLength(1)]
        public string PblFunl { get; set; }
        [Column("pbl_fchr")]
        public short? PblFchr { get; set; }
        [Column("pbl_fptc")]
        public short? PblFptc { get; set; }
        [Column("pbl_ffce")]
        [StringLength(18)]
        public string PblFfce { get; set; }
        [Column("pbt_cmnt")]
        [StringLength(254)]
        public string PbtCmnt { get; set; }
    }
}
