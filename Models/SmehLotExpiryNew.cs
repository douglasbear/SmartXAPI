using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__SMEH_Lot_ExpiryNew")]
    public partial class SmehLotExpiryNew
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Store_No")]
        [StringLength(200)]
        public string StoreNo { get; set; }
        [Column("Part_No")]
        [StringLength(500)]
        public string PartNo { get; set; }
        [StringLength(300)]
        public string Lot { get; set; }
        [StringLength(100)]
        public string Qty { get; set; }
        [Column("Qty_in")]
        [StringLength(100)]
        public string QtyIn { get; set; }
        [Column("Qty_out")]
        [StringLength(100)]
        public string QtyOut { get; set; }
        [Column("Qty_Balance")]
        [StringLength(100)]
        public string QtyBalance { get; set; }
        [Column("Exp_Date", TypeName = "date")]
        public DateTime? ExpDate { get; set; }
        [StringLength(1000)]
        public string Note { get; set; }
        [StringLength(1000)]
        public string Vendor { get; set; }
        [StringLength(1000)]
        public string Category { get; set; }
        [Column("UOM")]
        [StringLength(100)]
        public string Uom { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        [StringLength(1000)]
        public string Location { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
    }
}
