using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwItemStockMasterMaintenance
    {
        public long? Srl { get; set; }
        [Column("Item Code")]
        [StringLength(50)]
        public string ItemCode { get; set; }
        [Column("Item Name")]
        [StringLength(600)]
        public string ItemName { get; set; }
        [StringLength(100)]
        public string Category { get; set; }
        [StringLength(50)]
        public string Manufacturer { get; set; }
        [Column("Current Stock")]
        public double? CurrentStock { get; set; }
        [Column("UOM")]
        [StringLength(500)]
        public string Uom { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
    }
}
