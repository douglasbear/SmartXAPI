using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("_Mig_ProductsSMEH")]
    public partial class MigProductsSmeh
    {
        [StringLength(200)]
        public string StoreNo { get; set; }
        [StringLength(200)]
        public string PartNo { get; set; }
        [StringLength(200)]
        public string ExpDate { get; set; }
        [StringLength(200)]
        public string Lot { get; set; }
        [StringLength(200)]
        public string Qty { get; set; }
        [StringLength(200)]
        public string In { get; set; }
        [StringLength(200)]
        public string Out { get; set; }
        [StringLength(200)]
        public string Balance { get; set; }
        [StringLength(200)]
        public string Notes { get; set; }
        [Column(TypeName = "money")]
        public decimal? Price { get; set; }
        [Column(TypeName = "money")]
        public decimal? Cost { get; set; }
        [StringLength(200)]
        public string Vendor { get; set; }
        [StringLength(200)]
        public string Category { get; set; }
        [Column("UOM")]
        [StringLength(200)]
        public string Uom { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [StringLength(200)]
        public string Store { get; set; }
    }
}
