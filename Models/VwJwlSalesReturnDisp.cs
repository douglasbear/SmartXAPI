using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwJwlSalesReturnDisp
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("D_SalesDate", TypeName = "smalldatetime")]
        public DateTime? DSalesDate { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_BillAmt", TypeName = "money")]
        public decimal? NBillAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_FreightAmt", TypeName = "money")]
        public decimal? NFreightAmt { get; set; }
        [Column("N_CashReceived", TypeName = "money")]
        public decimal? NCashReceived { get; set; }
        [Column("N_ChequeReceived", TypeName = "money")]
        public decimal? NChequeReceived { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(50)]
        public string XChequeNo { get; set; }
        [Column("N_BankID")]
        public int? NBankId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("N_GoldReceived", TypeName = "money")]
        public decimal? NGoldReceived { get; set; }
        [Column("N_CreditCard", TypeName = "money")]
        public decimal? NCreditCard { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("N_SalesDetailsID")]
        public int NSalesDetailsId { get; set; }
        [Column("N_StockID")]
        public int? NStockId { get; set; }
        [Column("N_UnitRate", TypeName = "money")]
        public decimal? NUnitRate { get; set; }
        [Column("N_MCUnitRate", TypeName = "money")]
        public decimal? NMcunitRate { get; set; }
        [Column("N_WastageUnitRate", TypeName = "money")]
        public decimal? NWastageUnitRate { get; set; }
        [Column("N_Qty")]
        public int? NQty { get; set; }
        [Column("N_StoneCharges", TypeName = "money")]
        public decimal? NStoneCharges { get; set; }
        [Column("N_MCDiscPerc")]
        public double? NMcdiscPerc { get; set; }
        [Column("N_MCDiscount", TypeName = "money")]
        public decimal? NMcdiscount { get; set; }
        [Column("N_TotalAmount", TypeName = "money")]
        public decimal? NTotalAmount { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_GoldWeight")]
        public double? NGoldWeight { get; set; }
        [Column("N_StoneWeight")]
        public double? NStoneWeight { get; set; }
        [Column("N_MCRate", TypeName = "money")]
        public decimal? NMcrate { get; set; }
        [Column("N_MCPerc")]
        public double? NMcperc { get; set; }
        [Column("X_Barcode")]
        [StringLength(100)]
        public string XBarcode { get; set; }
        [Column("X_Type")]
        [StringLength(50)]
        public string XType { get; set; }
        [Column("X_Model")]
        [StringLength(50)]
        public string XModel { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_UnitPPrice", TypeName = "money")]
        public decimal? NUnitPprice { get; set; }
        [Column("N_UnitSPrice", TypeName = "money")]
        public decimal? NUnitSprice { get; set; }
        [Column("N_Karat")]
        public double? NKarat { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_Rate", TypeName = "money")]
        public decimal? NRate { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("X_TypeName")]
        [StringLength(100)]
        public string XTypeName { get; set; }
        [Column("N_RetAmount", TypeName = "money")]
        public decimal? NRetAmount { get; set; }
        [Column("N_DebitNoteId")]
        public int? NDebitNoteId { get; set; }
    }
}
