using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvStockStatus4Sae
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Required]
        [StringLength(3)]
        public string Type { get; set; }
        [Column("N_Factor")]
        public int NFactor { get; set; }
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
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("X_Remarks")]
        public string XRemarks { get; set; }
        [Column(TypeName = "money")]
        public decimal? SellingPrice { get; set; }
    }
}
