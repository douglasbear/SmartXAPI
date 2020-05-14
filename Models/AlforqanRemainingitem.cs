using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("___Alforqan_remainingitem")]
    public partial class AlforqanRemainingitem
    {
        [Column("itemId")]
        public int? ItemId { get; set; }
        [Column("itemName")]
        [StringLength(500)]
        public string ItemName { get; set; }
        [Column("unit")]
        [StringLength(500)]
        public string Unit { get; set; }
        [Column("rate")]
        [StringLength(500)]
        public string Rate { get; set; }
    }
}
