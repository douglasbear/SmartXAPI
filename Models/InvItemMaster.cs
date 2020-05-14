using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ItemMaster")]
    public partial class InvItemMaster
    {
        public InvItemMaster()
        {
            InvAssemblyDetails = new HashSet<InvAssemblyDetails>();
            InvDeliveryNoteDetails = new HashSet<InvDeliveryNoteDetails>();
            InvItemDetails = new HashSet<InvItemDetails>();
            InvPurchaseDetails = new HashSet<InvPurchaseDetails>();
            InvPurchaseOrderDetails = new HashSet<InvPurchaseOrderDetails>();
            InvSalesDetails = new HashSet<InvSalesDetails>();
            InvStockMaster = new HashSet<InvStockMaster>();
            InvTransferStockDetails = new HashSet<InvTransferStockDetails>();
            PrjProjectTransferDetails = new HashSet<PrjProjectTransferDetails>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("X_PurchaseDescription")]
        [StringLength(600)]
        public string XPurchaseDescription { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_Rate", TypeName = "money")]
        public decimal? NRate { get; set; }
        [Column("N_ItemCost", TypeName = "money")]
        public decimal? NItemCost { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_ItemManufacturerID")]
        public int? NItemManufacturerId { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_MinimumMargin")]
        public double? NMinimumMargin { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [Column("N_PreferredVendorID")]
        public int? NPreferredVendorId { get; set; }
        [Column("X_Rack")]
        [StringLength(50)]
        public string XRack { get; set; }
        [Column("X_ItemName_a")]
        [StringLength(800)]
        public string XItemNameA { get; set; }
        [Column("N_PriceLevelID")]
        public int? NPriceLevelId { get; set; }
        [Column("X_Color")]
        [StringLength(50)]
        public string XColor { get; set; }
        [Column("X_Base")]
        [StringLength(50)]
        public string XBase { get; set; }
        [Column("X_MaxWattage")]
        [StringLength(50)]
        public string XMaxWattage { get; set; }
        [Column("X_Unit")]
        [StringLength(50)]
        public string XUnit { get; set; }
        [Column("I_Image", TypeName = "image")]
        public byte[] IImage { get; set; }
        [Column("N_MinQty")]
        public double? NMinQty { get; set; }
        [Column("N_ReOrderQty")]
        public double? NReOrderQty { get; set; }
        [Column("X_PartNo")]
        [StringLength(250)]
        public string XPartNo { get; set; }
        [Column("N_StockUnitID")]
        public int? NStockUnitId { get; set; }
        [Column("N_SalesUnitID")]
        public int? NSalesUnitId { get; set; }
        [Column("N_PurchaseUnitID")]
        public int? NPurchaseUnitId { get; set; }
        [Column("X_Barcode")]
        [StringLength(50)]
        public string XBarcode { get; set; }
        [Column("B_BarcodewithQty")]
        public bool? BBarcodewithQty { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("B_IsIMEI")]
        public bool? BIsImei { get; set; }
        [Column("N_LengthID")]
        public int? NLengthId { get; set; }
        [Column("N_PurchasePrice", TypeName = "money")]
        public decimal? NPurchasePrice { get; set; }
        [Column("N_Weight")]
        public double? NWeight { get; set; }
        [Column("N_LabourChargePerc")]
        public double? NLabourChargePerc { get; set; }
        [Column("N_LabourCharge", TypeName = "money")]
        public decimal? NLabourCharge { get; set; }
        [Column("N_StoneRate", TypeName = "money")]
        public decimal? NStoneRate { get; set; }
        [Column("N_StoneProfitPerc")]
        public double? NStoneProfitPerc { get; set; }
        [Column("N_WastageRate", TypeName = "money")]
        public decimal? NWastageRate { get; set; }
        [Column("N_WastageRatePerc")]
        public double? NWastageRatePerc { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("N_Karat")]
        public double? NKarat { get; set; }
        [Column("N_MinMCPerc")]
        public double? NMinMcperc { get; set; }
        [Column("N_ColorID")]
        public int? NColorId { get; set; }
        [Column("N_AddUnitID1")]
        public int? NAddUnitId1 { get; set; }
        [Column("N_AddUnitID2")]
        public int? NAddUnitId2 { get; set; }
        [Column("N_ItemBrandID")]
        public int? NItemBrandId { get; set; }
        [Column("N_CategoryID1")]
        public int? NCategoryId1 { get; set; }
        [Column("B_IsBatch")]
        public bool? BIsBatch { get; set; }
        [Column("N_LeadDays")]
        public int? NLeadDays { get; set; }
        [Column("N_TransitDays")]
        public int? NTransitDays { get; set; }
        [Column("N_BOMUnitID")]
        public int? NBomunitId { get; set; }
        [Column("N_ShelfExpiry")]
        public double? NShelfExpiry { get; set; }
        [Column("N_Height")]
        public double? NHeight { get; set; }
        [Column("N_Length")]
        public double? NLength { get; set; }
        [Column("N_Width")]
        public double? NWidth { get; set; }
        [Column("X_HSCode")]
        [StringLength(25)]
        public string XHscode { get; set; }
        [Column("N_DutyPerc")]
        public double? NDutyPerc { get; set; }
        [Column("X_FullLengthDescription")]
        [StringLength(2000)]
        public string XFullLengthDescription { get; set; }
        [Column("B_BarcodeSalePrint")]
        public bool? BBarcodeSalePrint { get; set; }
        [Column("B_BarcodePurPrint")]
        public bool? BBarcodePurPrint { get; set; }
        [Column("N_PurchaseCost", TypeName = "money")]
        public decimal? NPurchaseCost { get; set; }
        [Column("X_Notes")]
        [StringLength(2000)]
        public string XNotes { get; set; }
        [Column("B_ExcludeInInvoice")]
        public bool? BExcludeInInvoice { get; set; }
        [Column("N_ItemTypeID")]
        public int? NItemTypeId { get; set; }
        [Column("N_AssItemID")]
        public int? NAssItemId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }

        [ForeignKey(nameof(NCategoryId))]
        [InverseProperty(nameof(InvItemCategory.InvItemMaster))]
        public virtual InvItemCategory NCategory { get; set; }
        [ForeignKey(nameof(NClassId))]
        [InverseProperty(nameof(InvItemClass.InvItemMaster))]
        public virtual InvItemClass NClass { get; set; }
        [ForeignKey(nameof(NItemUnitId))]
        [InverseProperty(nameof(InvItemUnit.InvItemMaster))]
        public virtual InvItemUnit NItemUnit { get; set; }
        [InverseProperty("NItem")]
        public virtual ICollection<InvAssemblyDetails> InvAssemblyDetails { get; set; }
        [InverseProperty("NItem")]
        public virtual ICollection<InvDeliveryNoteDetails> InvDeliveryNoteDetails { get; set; }
        [InverseProperty("NMainItem")]
        public virtual ICollection<InvItemDetails> InvItemDetails { get; set; }
        [InverseProperty("NItem")]
        public virtual ICollection<InvPurchaseDetails> InvPurchaseDetails { get; set; }
        [InverseProperty("NItem")]
        public virtual ICollection<InvPurchaseOrderDetails> InvPurchaseOrderDetails { get; set; }
        [InverseProperty("NItem")]
        public virtual ICollection<InvSalesDetails> InvSalesDetails { get; set; }
        [InverseProperty("NItem")]
        public virtual ICollection<InvStockMaster> InvStockMaster { get; set; }
        [InverseProperty("NItem")]
        public virtual ICollection<InvTransferStockDetails> InvTransferStockDetails { get; set; }
        [InverseProperty("NItem")]
        public virtual ICollection<PrjProjectTransferDetails> PrjProjectTransferDetails { get; set; }
    }
}
