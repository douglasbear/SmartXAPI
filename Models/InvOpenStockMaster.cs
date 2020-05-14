using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_OpenStockMaster")]
    public partial class InvOpenStockMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_InventoryID")]
        public int? NInventoryId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_IMEIFrom")]
        [StringLength(50)]
        public string NImeifrom { get; set; }
        [Column("N_IMEITo")]
        [StringLength(50)]
        public string NImeito { get; set; }
        [Column("N_Quantity")]
        public double? NQuantity { get; set; }
        [Column("N_LPrice", TypeName = "money")]
        public decimal? NLprice { get; set; }
        [Column("N_SPrice", TypeName = "money")]
        public decimal? NSprice { get; set; }
    }
}
