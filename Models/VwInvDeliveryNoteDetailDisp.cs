using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvDeliveryNoteDetailDisp
    {
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("N_DeliveryNoteId")]
        public int NDeliveryNoteId { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("D_DeliveryDate", TypeName = "smalldatetime")]
        public DateTime? DDeliveryDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_DeliveryNoteDetailsID")]
        public int NDeliveryNoteDetailsId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Sprice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("Item Class")]
        [StringLength(25)]
        public string ItemClass { get; set; }
        [Column("Class Item Name")]
        [StringLength(800)]
        public string ClassItemName { get; set; }
        [Column("N_MainQty")]
        public double? NMainQty { get; set; }
        [Column("N_MainSPrice", TypeName = "money")]
        public decimal? NMainSprice { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("Class Item Code")]
        [StringLength(100)]
        public string ClassItemCode { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_PhoneNo1")]
        [StringLength(20)]
        public string XPhoneNo1 { get; set; }
        [Column("X_PhoneNo2")]
        [StringLength(20)]
        public string XPhoneNo2 { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_BaseUnitID")]
        public int? NBaseUnitId { get; set; }
        [Column("X_BaseUnit")]
        [StringLength(500)]
        public string XBaseUnit { get; set; }
        [Column("N_UnitQty")]
        public double? NUnitQty { get; set; }
        [Column("N_MinimumMargin")]
        public double? NMinimumMargin { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("X_ItemRemarks")]
        [StringLength(250)]
        public string XItemRemarks { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_SalesUnitQty")]
        public double? NSalesUnitQty { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("X_SalesmanName")]
        [StringLength(100)]
        public string XSalesmanName { get; set; }
        [Column("x_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("N_LPrice")]
        public double? NLprice { get; set; }
        [Column("N_Stock")]
        public double? NStock { get; set; }
        [Column("N_StockOnHand")]
        public double? NStockOnHand { get; set; }
        [Column("N_UnitSPrice", TypeName = "money")]
        public decimal? NUnitSprice { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("N_IMEITo")]
        [StringLength(50)]
        public string NImeito { get; set; }
        [Column("N_IMEI")]
        [StringLength(50)]
        public string NImei { get; set; }
        [Column("N_SPriceTypeID")]
        public int? NSpriceTypeId { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("N_ItemDiscAmt", TypeName = "money")]
        public decimal? NItemDiscAmt { get; set; }
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
        [Column("X_DisplayName1")]
        [StringLength(100)]
        public string XDisplayName1 { get; set; }
        [Column("X_DisplayName2")]
        [StringLength(100)]
        public string XDisplayName2 { get; set; }
    }
}
