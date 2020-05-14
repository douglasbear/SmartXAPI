using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__FirstFocus_Lot_Expiry")]
    public partial class FirstFocusLotExpiry
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Catlog_No")]
        [StringLength(200)]
        public string CatlogNo { get; set; }
        [StringLength(300)]
        public string Lot { get; set; }
        [StringLength(100)]
        public string Qty { get; set; }
        [Column("Exp_Date", TypeName = "date")]
        public DateTime? ExpDate { get; set; }
        [StringLength(1000)]
        public string Vendor { get; set; }
        [StringLength(1000)]
        public string Category { get; set; }
        [Column("UOM")]
        [StringLength(100)]
        public string Uom { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
    }
}
