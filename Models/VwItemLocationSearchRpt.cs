using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwItemLocationSearchRpt
    {
        [Column("N_companyID")]
        public int? NCompanyId { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("Product Code")]
        [StringLength(100)]
        public string ProductCode { get; set; }
        [StringLength(800)]
        public string Description { get; set; }
        [Required]
        [StringLength(100)]
        public string Category { get; set; }
        public double OnHand { get; set; }
        [Column(TypeName = "money")]
        public decimal? Cost { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        public int NewQty { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
    }
}
