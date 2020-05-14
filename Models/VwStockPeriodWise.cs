using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwStockPeriodWise
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_Type")]
        [StringLength(50)]
        public string XType { get; set; }
        [Column("N_InventoryID")]
        public int? NInventoryId { get; set; }
        [Column("N_InventoryDetailsID")]
        public int? NInventoryDetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_StockIn")]
        public double? NStockIn { get; set; }
        [Column("N_StockOut")]
        public double? NStockOut { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("D_DateIn", TypeName = "datetime")]
        public DateTime? DDateIn { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
    }
}
