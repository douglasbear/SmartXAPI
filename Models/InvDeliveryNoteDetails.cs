using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_DeliveryNoteDetails")]
    public partial class InvDeliveryNoteDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DeliveryNoteID")]
        public int? NDeliveryNoteId { get; set; }
        [Key]
        [Column("N_DeliveryNoteDetailsID")]
        public int NDeliveryNoteDetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Sprice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("N_ItemDiscAmt", TypeName = "money")]
        public decimal? NItemDiscAmt { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_MainItemID")]
        public int? NMainItemId { get; set; }
        [Column("N_MainQty")]
        public double? NMainQty { get; set; }
        [Column("N_MainSPrice", TypeName = "money")]
        public decimal? NMainSprice { get; set; }
        [Column("N_SalesOrderID")]
        public int? NSalesOrderId { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("X_ItemRemarks")]
        [StringLength(250)]
        public string XItemRemarks { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_IMEI")]
        [StringLength(50)]
        public string NImei { get; set; }
        [Column("N_IMEITo")]
        [StringLength(50)]
        public string NImeito { get; set; }
        [Column("X_FreeDescription")]
        [StringLength(500)]
        public string XFreeDescription { get; set; }
        [Column("X_RefNo")]
        [StringLength(50)]
        public string XRefNo { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("N_SalesQuotationID")]
        public int? NSalesQuotationId { get; set; }
        [Column("N_SPriceTypeID")]
        public int? NSpriceTypeId { get; set; }
        [Column("N_RsID")]
        public int? NRsId { get; set; }
        [Column("X_RsNo")]
        [StringLength(15)]
        public string XRsNo { get; set; }
        [Column("N_RsDetailsID")]
        public int? NRsDetailsId { get; set; }
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
        [Column("N_SOdetailsID")]
        public int? NSodetailsId { get; set; }

        [ForeignKey(nameof(NItemId))]
        [InverseProperty(nameof(InvItemMaster.InvDeliveryNoteDetails))]
        public virtual InvItemMaster NItem { get; set; }
    }
}
