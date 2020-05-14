using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__SMEH_Lot_Expiry_Al")]
    public partial class SmehLotExpiryAl
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Store_No")]
        [StringLength(200)]
        public string StoreNo { get; set; }
        [Column("Part_No")]
        [StringLength(300)]
        public string PartNo { get; set; }
        [StringLength(300)]
        public string Lot { get; set; }
        [StringLength(100)]
        public string Qty { get; set; }
        [Column("Exp_Date", TypeName = "date")]
        public DateTime? ExpDate { get; set; }
        [StringLength(100)]
        public string Notes { get; set; }
        [StringLength(100)]
        public string Vendor { get; set; }
        [StringLength(100)]
        public string Category { get; set; }
        [Column("UOM")]
        [StringLength(100)]
        public string Uom { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
    }
}
