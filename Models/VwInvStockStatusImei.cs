using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvStockStatusImei
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_IMEI")]
        [StringLength(50)]
        public string NImei { get; set; }
        [Required]
        [StringLength(3)]
        public string Type { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [StringLength(50)]
        public string TranNo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? TranDate { get; set; }
        [StringLength(50)]
        public string TranBy { get; set; }
        [StringLength(100)]
        public string TranByName { get; set; }
        [Column("N_Cost")]
        public double? NCost { get; set; }
        [Column("N_SPrice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_Stock_IMEIID")]
        public int? NStockImeiid { get; set; }
    }
}
