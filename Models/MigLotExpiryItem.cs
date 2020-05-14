using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("_Mig_LotExpiryItem")]
    public partial class MigLotExpiryItem
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Store_No")]
        [StringLength(500)]
        public string StoreNo { get; set; }
        [Column("Part_No")]
        [StringLength(500)]
        public string PartNo { get; set; }
        [StringLength(500)]
        public string Lot { get; set; }
        [StringLength(100)]
        public string Qty { get; set; }
        [Column("Exp_Date", TypeName = "date")]
        public DateTime? ExpDate { get; set; }
        [StringLength(100)]
        public string Notes { get; set; }
        [StringLength(1000)]
        public string Vendor { get; set; }
        [StringLength(500)]
        public string Category { get; set; }
        [Column("UOM")]
        [StringLength(500)]
        public string Uom { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
    }
}
