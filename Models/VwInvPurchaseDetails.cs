using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPurchaseDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_PurchaseID")]
        public int? NPurchaseId { get; set; }
        [Column("N_PurchaseDetailsID")]
        public int NPurchaseDetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_FreeQty")]
        public double? NFreeQty { get; set; }
        [Column("N_MRP", TypeName = "money")]
        public decimal? NMrp { get; set; }
        [Column("N_PPrice", TypeName = "money")]
        public decimal? NPprice { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_SPrice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_RetQty")]
        public double? NRetQty { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_BaseUnitID")]
        public int? NBaseUnitId { get; set; }
        [Column("X_BaseUnit")]
        [StringLength(500)]
        public string XBaseUnit { get; set; }
        [Column("N_UnitQty")]
        public double? NUnitQty { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("N_POrderID")]
        public int? NPorderId { get; set; }
        [Column("N_Cost", TypeName = "decimal(20, 6)")]
        public decimal? NCost { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_IMEIFrom")]
        [StringLength(50)]
        public string NImeifrom { get; set; }
        [Column("N_IMEITo")]
        [StringLength(50)]
        public string NImeito { get; set; }
        [Column("B_IsIMEI")]
        public bool? BIsImei { get; set; }
        [Column("N_ItemUnitqty")]
        public double? NItemUnitqty { get; set; }
        [Column("N_OrderQty")]
        public double? NOrderQty { get; set; }
        [Column("X_PartNo")]
        [StringLength(50)]
        public string XPartNo { get; set; }
        [Column("N_RsID")]
        public int? NRsId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("N_CostF", TypeName = "decimal(20, 6)")]
        public decimal? NCostF { get; set; }
        [Column("N_DiscountAmtF", TypeName = "money")]
        public decimal? NDiscountAmtF { get; set; }
        [Column("N_PPriceF", TypeName = "money")]
        public decimal? NPpriceF { get; set; }
        [Column("X_FreeDescription")]
        [StringLength(500)]
        public string XFreeDescription { get; set; }
        [Column("X_Warranty")]
        [StringLength(100)]
        public string XWarranty { get; set; }
        [StringLength(50)]
        public string Duration { get; set; }
        [Column("D_WarrantyDate", TypeName = "datetime")]
        public DateTime? DWarrantyDate { get; set; }
        [Column("N_WarrantyId")]
        public int? NWarrantyId { get; set; }
        [Column("N_DurationId")]
        public int? NDurationId { get; set; }
        [Column("X_DisplayName")]
        [StringLength(100)]
        public string XDisplayName { get; set; }
        [Column("N_TaxPerc1", TypeName = "money")]
        public decimal? NTaxPerc1 { get; set; }
        [Column("N_TaxID1")]
        public int? NTaxId1 { get; set; }
        [Column("N_TaxID2")]
        public int? NTaxId2 { get; set; }
        [Column("N_TaxPerc2", TypeName = "money")]
        public decimal? NTaxPerc2 { get; set; }
        [Column("X_DisplayName2")]
        [StringLength(100)]
        public string XDisplayName2 { get; set; }
        [Column("N_POrderDetailsID")]
        public int? NPorderDetailsId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_CessID")]
        public int? NCessId { get; set; }
        [Column("N_CessAmt", TypeName = "money")]
        public decimal? NCessAmt { get; set; }
        [Column("N_CessPerc", TypeName = "money")]
        public decimal? NCessPerc { get; set; }
        [Column("X_CessDispName")]
        [StringLength(100)]
        public string XCessDispName { get; set; }
    }
}
