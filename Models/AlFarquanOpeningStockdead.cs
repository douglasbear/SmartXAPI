using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("___AlFarquan_OpeningStockdead")]
    public partial class AlFarquanOpeningStockdead
    {
        [Column("sr_no")]
        public int? SrNo { get; set; }
        [Column("part_no")]
        [StringLength(100)]
        public string PartNo { get; set; }
        [Column("unit")]
        [StringLength(100)]
        public string Unit { get; set; }
        [Column("unit_price")]
        public double? UnitPrice { get; set; }
        [Column("qty")]
        public double? Qty { get; set; }
        [Column("value")]
        [StringLength(100)]
        public string Value { get; set; }
    }
}
