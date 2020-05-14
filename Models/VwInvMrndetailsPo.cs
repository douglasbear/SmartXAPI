using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvMrndetailsPo
    {
        [Column("N_MRNID")]
        public int? NMrnid { get; set; }
        [Column("N_MRNDetailsID")]
        public int? NMrndetailsId { get; set; }
        [Column("N_QtyToStock")]
        public int? NQtyToStock { get; set; }
        [Column("N_ReturnQty")]
        public int? NReturnQty { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_BaseUnitID")]
        public int? NBaseUnitId { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("X_BaseUnit")]
        [StringLength(500)]
        public string XBaseUnit { get; set; }
        [Column("N_UnitQty")]
        public double? NUnitQty { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        public double? Qty { get; set; }
        public int? RemaingQty { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_POrderID")]
        public int NPorderId { get; set; }
        [Column("N_PRSDetailsID")]
        public int? NPrsdetailsId { get; set; }
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Reason")]
        [StringLength(50)]
        public string XReason { get; set; }
        [Column("N_PurchaseDetailsID")]
        public int? NPurchaseDetailsId { get; set; }
        [Column("X_PartNumber")]
        [StringLength(100)]
        public string XPartNumber { get; set; }
        public int? Expr1 { get; set; }
        [Column("N_POrderDetailsID")]
        [StringLength(100)]
        public string NPorderDetailsId { get; set; }
        public int? RetQty { get; set; }
        [Column("N_PPrice", TypeName = "money")]
        public decimal? NPprice { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_SerialFrom")]
        [StringLength(50)]
        public string NSerialFrom { get; set; }
        [Column("N_SerialTo")]
        [StringLength(50)]
        public string NSerialTo { get; set; }
        [Column("B_IsIMEI")]
        public bool? BIsImei { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
    }
}
