using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSisSub
    {
        public double? QtyIssued { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_PRSID")]
        public int? NPrsid { get; set; }
    }
}
