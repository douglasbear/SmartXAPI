using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvMrnSub
    {
        public int? QtyToStock { get; set; }
        public int? ReturnQty { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("ID")]
        public int? Id { get; set; }
        [Required]
        [Column("X_Type")]
        [StringLength(3)]
        public string XType { get; set; }
    }
}
