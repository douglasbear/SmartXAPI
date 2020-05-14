using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("_Alforqan_stock")]
    public partial class AlforqanStock
    {
        [Column("store")]
        [StringLength(1000)]
        public string Store { get; set; }
        [Column("item")]
        [StringLength(1000)]
        public string Item { get; set; }
        [Column("unit")]
        [StringLength(20)]
        public string Unit { get; set; }
        [Column("unit_price", TypeName = "money")]
        public decimal? UnitPrice { get; set; }
        [Column("balance")]
        public double? Balance { get; set; }
        [Column("value", TypeName = "money")]
        public decimal? Value { get; set; }
    }
}
