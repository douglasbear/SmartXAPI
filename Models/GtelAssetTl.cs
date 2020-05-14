using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("_GTEL_ASSET_TL")]
    public partial class GtelAssetTl
    {
        [Column("N_Number")]
        public int? NNumber { get; set; }
        [Column("Purchase_Date", TypeName = "datetime")]
        public DateTime? PurchaseDate { get; set; }
        [StringLength(50)]
        public string Item { get; set; }
        [StringLength(50)]
        public string Brand { get; set; }
        [StringLength(200)]
        public string Model { get; set; }
        [Column(TypeName = "money")]
        public decimal? Spec { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Qty { get; set; }
        [Column("U_Price", TypeName = "money")]
        public decimal? UPrice { get; set; }
        [Column("T_Price")]
        [StringLength(50)]
        public string TPrice { get; set; }
        [StringLength(50)]
        public string Supplier { get; set; }
        [StringLength(50)]
        public string Location { get; set; }
        [StringLength(50)]
        public string PurchaseBy { get; set; }
        [StringLength(50)]
        public string ToolCategory { get; set; }
        [Column("MOP")]
        [StringLength(50)]
        public string Mop { get; set; }
        [StringLength(50)]
        public string Serial { get; set; }
        [StringLength(50)]
        public string Project { get; set; }
        [StringLength(50)]
        public string Remarks { get; set; }
    }
}
