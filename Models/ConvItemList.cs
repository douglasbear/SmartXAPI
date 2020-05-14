using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Conv_ItemList")]
    public partial class ConvItemList
    {
        [Column("Item_Code")]
        [StringLength(200)]
        public string ItemCode { get; set; }
        [Column("Item_Name")]
        [StringLength(300)]
        public string ItemName { get; set; }
        [StringLength(100)]
        public string CatelogNo { get; set; }
        [StringLength(100)]
        public string Manufacture { get; set; }
        [StringLength(100)]
        public string Unit { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Stock { get; set; }
    }
}
