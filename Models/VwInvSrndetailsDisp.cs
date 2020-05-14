using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSrndetailsDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_SRNDetailsID")]
        public int NSrndetailsId { get; set; }
        [Column("N_SRNID")]
        public int? NSrnid { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_Qty")]
        public int? NQty { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_QtyDisp")]
        public int? NQtyDisp { get; set; }
        [Column("X_BaseUnit")]
        [StringLength(500)]
        public string XBaseUnit { get; set; }
        [Column("X_BaseQty")]
        public double? XBaseQty { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        public double? ItemUnitQty { get; set; }
    }
}
