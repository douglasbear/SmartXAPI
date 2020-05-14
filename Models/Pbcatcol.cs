using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("pbcatcol")]
    public partial class Pbcatcol
    {
        [Required]
        [Column("pbc_tnam")]
        [StringLength(129)]
        public string PbcTnam { get; set; }
        [Column("pbc_tid")]
        public int? PbcTid { get; set; }
        [Required]
        [Column("pbc_ownr")]
        [StringLength(129)]
        public string PbcOwnr { get; set; }
        [Required]
        [Column("pbc_cnam")]
        [StringLength(129)]
        public string PbcCnam { get; set; }
        [Column("pbc_cid")]
        public short? PbcCid { get; set; }
        [Column("pbc_labl")]
        [StringLength(254)]
        public string PbcLabl { get; set; }
        [Column("pbc_lpos")]
        public short? PbcLpos { get; set; }
        [Column("pbc_hdr")]
        [StringLength(254)]
        public string PbcHdr { get; set; }
        [Column("pbc_hpos")]
        public short? PbcHpos { get; set; }
        [Column("pbc_jtfy")]
        public short? PbcJtfy { get; set; }
        [Column("pbc_mask")]
        [StringLength(31)]
        public string PbcMask { get; set; }
        [Column("pbc_case")]
        public short? PbcCase { get; set; }
        [Column("pbc_hght")]
        public short? PbcHght { get; set; }
        [Column("pbc_wdth")]
        public short? PbcWdth { get; set; }
        [Column("pbc_ptrn")]
        [StringLength(31)]
        public string PbcPtrn { get; set; }
        [Column("pbc_bmap")]
        [StringLength(1)]
        public string PbcBmap { get; set; }
        [Column("pbc_init")]
        [StringLength(254)]
        public string PbcInit { get; set; }
        [Column("pbc_cmnt")]
        [StringLength(254)]
        public string PbcCmnt { get; set; }
        [Column("pbc_edit")]
        [StringLength(31)]
        public string PbcEdit { get; set; }
        [Column("pbc_tag")]
        [StringLength(254)]
        public string PbcTag { get; set; }
    }
}
