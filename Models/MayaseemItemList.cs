using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Mayaseem_ItemList")]
    public partial class MayaseemItemList
    {
        [Column("Item_Code")]
        [StringLength(200)]
        public string ItemCode { get; set; }
        [Column("Item_DescriptionE")]
        [StringLength(2000)]
        public string ItemDescriptionE { get; set; }
        [Column("Item_DescriptionAr")]
        [StringLength(2000)]
        public string ItemDescriptionAr { get; set; }
        [Column("unit")]
        [StringLength(100)]
        public string Unit { get; set; }
        [Column("Qty_Loc1")]
        public int? QtyLoc1 { get; set; }
        [Column("cost", TypeName = "money")]
        public decimal? Cost { get; set; }
        [Column("R_Price", TypeName = "money")]
        public decimal? RPrice { get; set; }
        [Column("W_Price", TypeName = "money")]
        public decimal? WPrice { get; set; }
        [Column("T_Price", TypeName = "money")]
        public decimal? TPrice { get; set; }
        [Column("Qty_Loc2")]
        public int? QtyLoc2 { get; set; }
        [Column("Qty_Loc3")]
        public int? QtyLoc3 { get; set; }
    }
}
