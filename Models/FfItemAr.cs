using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__FF_Item_ar")]
    public partial class FfItemAr
    {
        [Column("Item Code")]
        [StringLength(100)]
        public string ItemCode { get; set; }
        [Column("Arabic item Name")]
        [StringLength(1000)]
        public string ArabicItemName { get; set; }
        [Column("ENGLISH Item  Name")]
        [StringLength(1000)]
        public string EnglishItemName { get; set; }
    }
}
