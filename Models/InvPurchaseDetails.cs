using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_PurchaseDetails")]
    public partial class InvPurchaseDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_PurchaseID")]
        public int? NPurchaseId { get; set; }
        [Key]
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
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("N_POrderID")]
        public int? NPorderId { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("X_ItemRemarks")]
        [StringLength(250)]
        public string XItemRemarks { get; set; }
        [Column("N_Cost", TypeName = "decimal(20, 6)")]
        public decimal? NCost { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_IMEIFrom")]
        [StringLength(50)]
        public string NImeifrom { get; set; }
        [Column("N_IMEITo")]
        [StringLength(50)]
        public string NImeito { get; set; }
        [Column("N_OrderQty")]
        public double? NOrderQty { get; set; }
        [Column("X_PartNo")]
        [StringLength(50)]
        public string XPartNo { get; set; }
        [Column("N_RsID")]
        public int? NRsId { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("N_PPriceF", TypeName = "money")]
        public decimal? NPpriceF { get; set; }
        [Column("N_DiscountAmtF", TypeName = "money")]
        public decimal? NDiscountAmtF { get; set; }
        [Column("N_CostF", TypeName = "decimal(20, 6)")]
        public decimal? NCostF { get; set; }
        [Column("X_FreeDescription")]
        [StringLength(500)]
        public string XFreeDescription { get; set; }
        [Column("N_WarrantyId")]
        public int? NWarrantyId { get; set; }
        [Column("N_DurationId")]
        public int? NDurationId { get; set; }
        [Column("D_WarrantyDate", TypeName = "datetime")]
        public DateTime? DWarrantyDate { get; set; }
        [Column("N_TaxCategoryID1")]
        public int? NTaxCategoryId1 { get; set; }
        [Column("N_TaxPercentage1", TypeName = "money")]
        public decimal? NTaxPercentage1 { get; set; }
        [Column("N_TaxAmt1", TypeName = "money")]
        public decimal? NTaxAmt1 { get; set; }
        [Column("N_TaxCategoryID2")]
        public int? NTaxCategoryId2 { get; set; }
        [Column("N_TaxPercentage2", TypeName = "money")]
        public decimal? NTaxPercentage2 { get; set; }
        [Column("N_TaxAmt2", TypeName = "money")]
        public decimal? NTaxAmt2 { get; set; }
        [Column("N_TaxAmt1F", TypeName = "money")]
        public decimal? NTaxAmt1F { get; set; }
        [Column("N_TaxAmt2F", TypeName = "money")]
        public decimal? NTaxAmt2F { get; set; }
        [Column("N_POrderDetailsID")]
        public int? NPorderDetailsId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_CessID")]
        public int? NCessId { get; set; }
        [Column("N_CessAmt", TypeName = "money")]
        public decimal? NCessAmt { get; set; }
        [Column("N_CessPerc", TypeName = "money")]
        public decimal? NCessPerc { get; set; }

        [ForeignKey(nameof(NItemId))]
        [InverseProperty(nameof(InvItemMaster.InvPurchaseDetails))]
        public virtual InvItemMaster NItem { get; set; }
        [ForeignKey(nameof(NPurchaseId))]
        [InverseProperty(nameof(InvPurchase.InvPurchaseDetails))]
        public virtual InvPurchase NPurchase { get; set; }
    }
}
