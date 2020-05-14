using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvMrndetailsPoprs
    {
        [Column("N_MRNID")]
        public int? NMrnid { get; set; }
        [Column("N_MRNDetailsID")]
        public int NMrndetailsId { get; set; }
        [Column("N_QtyToStock")]
        public double? NQtyToStock { get; set; }
        [Column("N_ReturnQty")]
        public double? NReturnQty { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
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
        public double? RemaingQty { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_POrderID")]
        public int? NPorderId { get; set; }
        [Column("N_PRSDetailsID")]
        public int? NPrsdetailsId { get; set; }
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
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
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("N_POrderDetailsID")]
        [StringLength(100)]
        public string NPorderDetailsId { get; set; }
        public double? RetQty { get; set; }
        [Column("N_PPrice", TypeName = "money")]
        public decimal? NPprice { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
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
        [Column("OrderID")]
        public int? OrderId { get; set; }
        [StringLength(50)]
        public string OrderNo { get; set; }
        [Column("PRSID")]
        public int? Prsid { get; set; }
        [Column("PRSNo")]
        [StringLength(50)]
        public string Prsno { get; set; }
        [Column("N_ProcuredQty")]
        public double? NProcuredQty { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("X_Currency")]
        [StringLength(10)]
        public string XCurrency { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("N_PpriceF", TypeName = "money")]
        public decimal? NPpriceF { get; set; }
    }
}
