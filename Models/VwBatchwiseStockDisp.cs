using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwBatchwiseStockDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Required]
        [Column("D_ExpiryDate")]
        [StringLength(8000)]
        public string DExpiryDate { get; set; }
        [StringLength(50)]
        public string Stock { get; set; }
        public double? CurrentStock { get; set; }
    }
}
