using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("_Alforqan_stock_mainstore")]
    public partial class AlforqanStockMainstore
    {
        [Column("item")]
        [StringLength(1000)]
        public string Item { get; set; }
        [Column("unit")]
        [StringLength(20)]
        public string Unit { get; set; }
        [Column("stock")]
        public double? Stock { get; set; }
        [Column("unit_price")]
        public double? UnitPrice { get; set; }
        [Column("stock_value", TypeName = "money")]
        public decimal? StockValue { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
    }
}
