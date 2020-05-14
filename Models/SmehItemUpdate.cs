using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__SMEH_ItemUpdate")]
    public partial class SmehItemUpdate
    {
        [StringLength(500)]
        public string Description { get; set; }
        [Column("Part_No")]
        [StringLength(200)]
        public string PartNo { get; set; }
        [Column("UOM")]
        [StringLength(300)]
        public string Uom { get; set; }
        [Column("Selling_Price", TypeName = "money")]
        public decimal? SellingPrice { get; set; }
    }
}
