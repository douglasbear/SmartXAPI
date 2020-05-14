using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_PurchaseOrderDetails")]
    public partial class InvPurchaseOrderDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_POrderID")]
        public int? NPorderId { get; set; }
        [Key]
        [Column("N_POrderDetailsID")]
        public int NPorderDetailsId { get; set; }
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
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_PRSID")]
        public int? NPrsid { get; set; }
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Column("N_PRSDetailsID")]
        public int? NPrsdetailsId { get; set; }
        [Column("X_PartNumber")]
        public string XPartNumber { get; set; }
        [Column("X_Remarks")]
        public string XRemarks { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("N_PPriceF", TypeName = "money")]
        public decimal? NPpriceF { get; set; }
        [Column("N_DiscountAmtF", TypeName = "money")]
        public decimal? NDiscountAmtF { get; set; }
        [Column("N_CashPaidF", TypeName = "money")]
        public decimal? NCashPaidF { get; set; }
        [Column("N_FreightAmtF", TypeName = "money")]
        public decimal? NFreightAmtF { get; set; }
        [Column("N_DeliveryDays")]
        public int? NDeliveryDays { get; set; }
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
        [Column("X_FreeDescription")]
        [StringLength(5000)]
        public string XFreeDescription { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_RFQId")]
        public int? NRfqid { get; set; }
        [Column("D_DeMobDate", TypeName = "datetime")]
        public DateTime? DDeMobDate { get; set; }

        [ForeignKey(nameof(NItemId))]
        [InverseProperty(nameof(InvItemMaster.InvPurchaseOrderDetails))]
        public virtual InvItemMaster NItem { get; set; }
        [ForeignKey(nameof(NPorderId))]
        [InverseProperty(nameof(InvPurchaseOrder.InvPurchaseOrderDetails))]
        public virtual InvPurchaseOrder NPorder { get; set; }
    }
}
