using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_StockAdjustmentDetails")]
    public partial class InvStockAdjustmentDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_AdjustmentID")]
        public int? NAdjustmentId { get; set; }
        [Key]
        [Column("N_AdjustmentDetailsID")]
        public int NAdjustmentDetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(200)]
        public string XItemCode { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        [Column("N_QtyOnHand")]
        public double? NQtyOnHand { get; set; }
        [Column("N_AdjustedQty")]
        public double? NAdjustedQty { get; set; }
        [Column("N_ReasonID")]
        public int? NReasonId { get; set; }
        [Column("N_NewQty")]
        public double? NNewQty { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }

        [ForeignKey(nameof(NAdjustmentId))]
        [InverseProperty(nameof(InvStockAdjustment.InvStockAdjustmentDetails))]
        public virtual InvStockAdjustment NAdjustment { get; set; }
        [ForeignKey(nameof(NReasonId))]
        [InverseProperty(nameof(InvStockAdjstmentReason.InvStockAdjustmentDetails))]
        public virtual InvStockAdjstmentReason NReason { get; set; }
    }
}
