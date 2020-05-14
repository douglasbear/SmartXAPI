using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwStockStatus1
    {
        [Required]
        [Column("type")]
        [StringLength(2)]
        public string Type { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        public double? Stock { get; set; }
        [Column("date", TypeName = "datetime")]
        public DateTime? Date { get; set; }
    }
}
