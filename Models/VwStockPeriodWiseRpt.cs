using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwStockPeriodWiseRpt
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
        [Column("D_DateIn", TypeName = "datetime")]
        public DateTime? DDateIn { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        [Column("N_ItemUnitID")]
        public int NItemUnitId { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
    }
}
