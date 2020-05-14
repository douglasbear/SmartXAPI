using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_StockStatus_Rpt")]
    public partial class InvStockStatusRpt
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [StringLength(5)]
        public string Type { get; set; }
        [Column("N_Factor")]
        public int? NFactor { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [StringLength(100)]
        public string TranNo { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [StringLength(50)]
        public string TranBy { get; set; }
        [StringLength(500)]
        public string TranByName { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("N_Sellingprice")]
        public double? NSellingprice { get; set; }
    }
}
